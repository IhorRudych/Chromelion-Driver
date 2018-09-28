/////////////////////////////////////////////////////////////////////////////
//
// PDAChannel.cs
// /////////////
//
// ChannelTest Chromeleon DDK Code Example
//
// PDA Channel class for the ChannelTest example driver.
// The ChannelTest driver creates two channel devices that
// generate a cosinus waveform during acquisition and
// a PDA channel device.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Timers;

using Dionex.Chromeleon.DDK;                    // Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;                // Chromeleon Symbol Interface

namespace MyCompany.ChannelTest
{
    /////////////////////////////////////////////////////////////////////////////
    /// PDAChannel Device Class

    internal class PDAChannel
    {
        #region Data Members

        /// our Chromeleon DDK channel interface
        private IPDAChannel m_MyCmDevice;

        /// We use a timer to create our data points.
        private System.Timers.Timer m_Timer;

        // The absolute time of the AcQOn command
        private DateTime m_AcquisitionOnTime;

        // The retention time of the AcQOn command
        private double m_AcquisitionOnRetention;

        // How many spectra will be sent each second?
        private IDoubleProperty m_RateProperty;

        // The data packet to be sent
        private int[] m_Spectrum;

        // total data point index
        private int m_DataIndex;

        // We expose the data index as a read-only property
        private IIntProperty m_SpectraIndexProperty;

        // the internal channel time
        private IDoubleProperty m_ChannelTimeProperty;

        // total packet index
        private int m_PacketIndex;

        // the time interval between two data point sets
        private TimeSpan m_DataPointSetInterval;

        // boolean property to inform the data generation timer that no more data is necessary
        private bool m_GotDataFinished;

        #endregion

        #region IPDAChannel

        /// Create our Chromeleon symbols
        internal IPDAChannel OnCreate(IDDK cmDDK, IDevice master, string name)
        {
            // Create a data type for the wavelength range
            ITypeDouble tWavelength = cmDDK.CreateDouble(200.0, 600.0, 1);
            tWavelength.Unit = "nm";
            // Create a data type for the reference wavelength range
            ITypeDouble tRefWavelength = cmDDK.CreateDouble(200.0, 600.0, 1);
            tRefWavelength.Unit = "nm";
            tRefWavelength.AddNamedValue("Off", 0.0);
            // Create a data type for the reference bandwidth
            ITypeDouble tReferenceBandwidth = cmDDK.CreateDouble(0.0, 20.0, 1);
            tReferenceBandwidth.Unit = "nm";
            // Create a data type for the bunch width
            ITypeDouble tBunchWidth = cmDDK.CreateDouble(0.0, 10.0, 1);
            tBunchWidth.Unit = "nm";

            // Create the channel symbol
            m_MyCmDevice = cmDDK.CreatePDAChannel(name, "This is a channel that can generate a spectrum.",
                tWavelength, tRefWavelength, tReferenceBandwidth, tBunchWidth,
                400, PDAWriteMode.AbsorbanceDirect);
            // We will be a subdevice of the master device
            m_MyCmDevice.SetOwner(master);

            // Attach our handlers to the AcquisitionOffCommand and AcquisitionOnCommand of the channel
            m_MyCmDevice.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_MyCmDevice.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            m_MyCmDevice.OnDataFinished += new DataFinishedEventHandler(m_MyCmDevice_OnDataFinished);

            // Usually, TimeStepFactor and TimeStepFactor are read-only properties and the driver updates them.
            // In this driver we allow to change these values manually for illustration purposes.
            m_MyCmDevice.TimeStepFactorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepFactorProperty.DataType.Maximum = 10000;
            m_MyCmDevice.TimeStepFactorProperty.OnSetProperty += new SetPropertyEventHandler(TimeStepFactorProperty_OnSetProperty);

            m_MyCmDevice.TimeStepDivisorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepDivisorProperty.DataType.Maximum = 10000;
            m_MyCmDevice.TimeStepDivisorProperty.OnSetProperty += new SetPropertyEventHandler(TimeStepDivisorProperty_OnSetProperty);

            ITypeDouble tRateType = cmDDK.CreateDouble(0.1, 1000.0, 1);
            tRateType.Unit = "Hz";
            m_RateProperty = m_MyCmDevice.CreateProperty("FixedRate", "The data collection rate in Hz.", tRateType);
            m_RateProperty.OnSetProperty += new SetPropertyEventHandler(m_RateProperty_OnSetProperty);

            ITypeInt tDataIndexType = cmDDK.CreateInt(0, int.MaxValue);
            m_SpectraIndexProperty =
                m_MyCmDevice.CreateProperty("TotalSpectra", "Total number of spectra.", tDataIndexType);

            ICommand noMoreDataCommand = m_MyCmDevice.CreateCommand("NoMoreData", "Send IChannel.NoMoreData to stop data acquisition immediately.");
            noMoreDataCommand.OnCommand += new CommandEventHandler(noMoreDataCommand_OnCommand);

            ITypeDouble tChannelTimeType = cmDDK.CreateDouble(0, 1000, 3);
            tChannelTimeType.Unit = "min";
            m_ChannelTimeProperty =
                m_MyCmDevice.CreateProperty("ChannelTime", "Internal time of the channel.", tChannelTimeType);

            return m_MyCmDevice;
        }

        // We can use the NoMoreData command to simulate a hardware error or similar condition
        // that makes it impossible for our driver to send more data.
        void noMoreDataCommand_OnCommand(CommandEventArgs args)
        {
            m_MyCmDevice.NoMoreData();
        }

        void m_RateProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            m_RateProperty.Update(doubleArgs.NewValue);

            // Our rate determines TimeStepFactorProperty and TimeStepDivisorProperty:
            // TimeStepDivisorProperty / TimeStepFactorProperty = RateProperty /100 Hz
            // TimeStepFactorProperty and TimeStepDivisorProperty must be integers.
            // To keep it simple we set TimeStepFactorProperty = 10000 and calcutate
            // TimeStepDivisorProperty from the rate:
            // TimeStepDivisorProperty = RateProperty * 100

            m_MyCmDevice.TimeStepDivisorProperty.Update(
                (int)(doubleArgs.NewValue.Value * 100.0));
            m_MyCmDevice.TimeStepFactorProperty.Update(10000);
        }

        void TimeStepFactorProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            m_MyCmDevice.TimeStepFactorProperty.Update(intArgs.NewValue);

            UpdateRate();
        }

        void TimeStepDivisorProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            m_MyCmDevice.TimeStepDivisorProperty.Update(intArgs.NewValue);

            UpdateRate();
        }

        private void UpdateRate()
        {
            // Update the rate resulting from that change
            // TimeStepDivisorProperty / TimeStepFactorProperty = RateProperty /100 Hz
            // RateProperty = TimeStepDivisorProperty / TimeStepFactorProperty * 100;

            double newRate =
                (double)m_MyCmDevice.TimeStepDivisorProperty.Value.Value /
                (double)m_MyCmDevice.TimeStepFactorProperty.Value.Value * 100.0;

            m_RateProperty.Update(newRate);
        }

        internal void OnConnect()
        {
            // initialize our properties
            m_MyCmDevice.TimeStepFactorProperty.Update(10000);
            m_MyCmDevice.TimeStepDivisorProperty.Update(1000);

            m_MyCmDevice.SignalFactorProperty.Update(1);

            m_RateProperty.Update(10);

            m_MyCmDevice.MinimumWavelengthProperty.Update(200);
            m_MyCmDevice.MaximumWavelengthProperty.Update(600);
            m_MyCmDevice.BunchWidthProperty.Update(1);
            m_MyCmDevice.ReferenceBandwidthProperty.Update(1);
            m_MyCmDevice.ReferenceWavelengthProperty.Update(222);
        }

        internal void OnDisconnect()
        {
        }

        private void OnAcqOn(CommandEventArgs args)
        {
            m_GotDataFinished = false;

            // create a spectrum data packet
            m_Spectrum = new int[m_MyCmDevice.MaximumNumberOfDataPoints];

            // Reset the total data point index
            m_DataIndex = 0;

            // Reset the total packet index
            m_PacketIndex = 0;

            // Calculate the interval of data point sets (in ticks)
            // 1 tick = 100 nanoseconds
            //       = 0.1 microseconds
            //       = 0.0001 milliseconds
            //       = 0.0000001 seconds
            m_DataPointSetInterval = new TimeSpan((long)(10000000.0 / m_RateProperty.Value.Value));

            // Create a timer with 500ms, to create packets of data points
            m_Timer = new Timer(500);

            m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // enable the timer
            m_Timer.Enabled = true;
            m_Timer.AutoReset = false;

            m_AcquisitionOnTime = DateTime.UtcNow;

            m_AcquisitionOnRetention = args.RetentionTime.Minutes > 0 ? args.RetentionTime.Minutes : 0;
        }

        private void OnAcqOff(CommandEventArgs args)
        {
            // We will stop the data acquisition in OnDataFinished
        }

        /// <summary>
        /// We use the OnDataFinished event to stop data acquisition.
        /// When CM sends this event enough data has already been sent and we can stop immediately.
        /// </summary>
        /// <param name="args"></param>
        void m_MyCmDevice_OnDataFinished(DataFinishedEventArgs args)
        {
            if (m_Timer != null)
            {
                m_GotDataFinished = true;
                m_SpectraIndexProperty.Update(null);
                m_ChannelTimeProperty.Update(null);
            }
        }

        #endregion

        private void OnTimedEvent(object source, ElapsedEventArgs args)
        {
            if (m_GotDataFinished)
            {
                HandleTimer();
                return;
            }

            // we use the timer to update our data.
            TimeSpan elapsedTime = DateTime.UtcNow - m_AcquisitionOnTime;
            if (elapsedTime.TotalMilliseconds > m_DataPointSetInterval.TotalMilliseconds * (m_PacketIndex + 1))
            {
                Int32 numberOfSpectraToGenerate = Convert.ToInt32((elapsedTime.TotalMilliseconds - m_DataPointSetInterval.TotalMilliseconds * (m_DataIndex + 1)) / m_DataPointSetInterval.TotalMilliseconds);
                for (int indexSpectrum = 0; indexSpectrum < numberOfSpectraToGenerate; indexSpectrum++)
                {
                    // in minutes
                    double dCurrentTime = m_AcquisitionOnRetention + (double)m_DataIndex / m_RateProperty.Value.Value / 60.0;
                    m_ChannelTimeProperty.Update(dCurrentTime);
                    int signal = ChannelTestDriver.CurrentDataValue(dCurrentTime);
                    for (int indexDataPoint = 0; indexDataPoint < m_MyCmDevice.MaximumNumberOfDataPoints; indexDataPoint++)
                    {
                        m_Spectrum[indexDataPoint] = signal;
                    }
                    m_DataIndex++;
                    m_MyCmDevice.UpdateData(false, m_DataIndex, m_Spectrum);

                    // update the total number of spectra already acquired
                    m_SpectraIndexProperty.Update(m_DataIndex);
                }
                m_PacketIndex++;
            }
            HandleTimer();
        }

        internal void OnHardwareError()
        {
            // stop sending data
            m_MyCmDevice_OnDataFinished(null);
        }

        internal void HandleTimer()
        {
            if (m_Timer != null && !m_GotDataFinished)
            {
                m_Timer.Enabled = true;
            }
            else
            {
                m_Timer.Enabled = false;
                m_Timer.Close();
                m_Timer = null;
            }
        }
    }
}
