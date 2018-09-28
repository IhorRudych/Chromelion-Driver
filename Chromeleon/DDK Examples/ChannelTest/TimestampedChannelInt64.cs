/////////////////////////////////////////////////////////////////////////////
//
// TimestampedChannelInt64.cs
// //////////////////////////
//
// ChannelTest Chromeleon DDK Code Example
//
// Channel class for the ChannelTest example driver.
// The ChannelTest driver creates three channel devices that
// generate a cosinus waveform during acquisition and
// a PDA channel device.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Timers;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.ChannelTest
{
    /////////////////////////////////////////////////////////////////////////////
    /// Int64Channel Device Class

    internal class TimestampedChannelInt64
    {
        #region Data Members

        /// our Chromeleon DDK channel interface
        private IChannel m_MyCmDevice;

        /// We use a timer to create our data points.
        private System.Timers.Timer m_Timer;

        // The absolute time of the AcQOn command
        private DateTime m_AcquisitionOnTime;

        // The retention time of the AcQOn command
        private double m_AcquisitionOnRetention;

        // How many data points will be sent each second?
        private IDoubleProperty m_RateProperty;

        // The data packet to be sent
        private DataPointInt64[] m_DataPacket;

        // total data point index
        private int m_DataIndex;

        // We expose the data index as a read-only property
        private IIntProperty m_DataIndexProperty;

        // the internal channel time
        private IDoubleProperty m_ChannelTimeProperty;

        // total packet index
        private int m_PacketIndex;

        // the time interval between two data points
        private TimeSpan m_DataPointInterval;

        // boolean property to inform the data generation timer that no more data is necessary
        private bool m_GotDataFinished;

        #endregion

        #region IChannel

        /// Create our Chromeleon symbols
        internal IChannel OnCreate(IDDK cmDDK, IDevice master, string name)
        {
            // Create a data type for our signal
            ITypeDouble tSignal = cmDDK.CreateDouble(-100.0, 100.0, 1);
            tSignal.Unit = "mV";

            // Create the channel symbol
            m_MyCmDevice = cmDDK.CreateChannel(name, "Channel that can generate both sinus curve and sawtooth signal as Int64 data points.", tSignal);
            // We will be a subdevice of the master device
            m_MyCmDevice.SetOwner(master);

            // Attach our handlers to the AcquisitionOffCommand and AcquisitionOnCommand of the channel
            m_MyCmDevice.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_MyCmDevice.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            m_MyCmDevice.OnDataFinished += new DataFinishedEventHandler(m_MyCmDevice_OnDataFinished);

            // These two parameters are not longer relevant when using the new UpdateDataEx function
            // in contradiction to the old UpdateData interface. So leave these parameters unchanged; 
            // the DDK will initialize them appropriately.
            m_MyCmDevice.TimeStepFactorProperty.Writeable = false;
            m_MyCmDevice.TimeStepDivisorProperty.Writeable = false;

            ITypeDouble tRateType = cmDDK.CreateDouble(0.1, 1000.0, 1);
            tRateType.Unit = "Hz";
            m_RateProperty = m_MyCmDevice.CreateProperty("FixedRate", "The data collection rate in Hz.", tRateType);
            m_RateProperty.OnSetProperty += new SetPropertyEventHandler(m_RateProperty_OnSetProperty);

            ITypeInt tDataIndexType = cmDDK.CreateInt(0, int.MaxValue);
            m_DataIndexProperty =
                m_MyCmDevice.CreateProperty("TotalDataPoints", "Total number of data points.", tDataIndexType);

            ICommand noMoreDataCommand = m_MyCmDevice.CreateCommand("NoMoreData", "Send IChannel.NoMoreData to stop data acquisition immediately.");
            noMoreDataCommand.OnCommand += new CommandEventHandler(noMoreDataCommand_OnCommand);

            ITypeDouble tChannelTimeType = cmDDK.CreateDouble(0, 1000, 3);
            tChannelTimeType.Unit = "min";
            m_ChannelTimeProperty =
                m_MyCmDevice.CreateProperty("ChannelTime", "Internal time of the channel.", tChannelTimeType);

            // This channel doesn't have peaks, we don't want to have it integrated.
            m_MyCmDevice.NeedsIntegration = false;

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

            // For the time stamped channel rate and timestep are different.
            // The rate determines at which rate data points are to be sent;
            // The timestep determines how the timestamps are encoded.
        }

        internal void OnConnect()
        {
            // initilaize our properties
            m_MyCmDevice.SignalFactorProperty.Update(0.001);

            m_RateProperty.Update(10);
        }

        internal void OnDisconnect()
        {
        }

        private void OnAcqOn(CommandEventArgs args)
        {
            m_GotDataFinished = false;

            // Reset the total data point index
            m_DataIndex = 0;

            // Reset the total packet index
            m_PacketIndex = 0;

            // Calculate the data point interval (in ticks)
            // 1 tick = 100 nanoseconds
            //       = 0.1 microseconds
            //       = 0.0001 milliseconds
            //       = 0.0000001 seconds
            m_DataPointInterval = new TimeSpan((long)(10000000.0 / m_RateProperty.Value.Value));

            // Create a timer with 500ms, to create packets of data points
            m_Timer = new Timer(500);

            m_Timer.AutoReset = false;
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // enable the timer
            m_Timer.Enabled = true;

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
                m_DataIndexProperty.Update(null);
                m_ChannelTimeProperty.Update(null);
            }
        }

        #endregion


        private void OnTimedEvent(object source, ElapsedEventArgs args)
        {
            // we use the timer to update our data.

            if (m_GotDataFinished)
            {
                HandleTimer();
                return;
            }

            TimeSpan elapsedTime = DateTime.UtcNow - m_AcquisitionOnTime;


            if (elapsedTime.TotalMilliseconds > m_DataPointInterval.TotalMilliseconds * (m_PacketIndex + 1))
            {
                // create as many data points as necessary
                Int32 numberOfDataPointsToGenerate = Convert.ToInt32((elapsedTime.TotalMilliseconds - m_DataPointInterval.TotalMilliseconds * (m_DataIndex + 1)) / m_DataPointInterval.TotalMilliseconds);
                m_DataPacket = new DataPointInt64[numberOfDataPointsToGenerate];
                for (int i = 0; i < numberOfDataPointsToGenerate; i++)
                {
                    // in minutes
                    double dCurrentTime = m_AcquisitionOnRetention + (double)m_DataIndex / m_RateProperty.Value.Value / 60.0;
                    m_ChannelTimeProperty.Update(dCurrentTime);

                    // timestamp is sent in ns
                    double timestamp = m_AcquisitionOnRetention * 60.0 * 1.0E9 + (double)m_DataIndex * 1.0E9 / (m_RateProperty.Value.Value);

                    m_DataPacket[i] = new DataPointInt64(Convert.ToInt64(timestamp), Convert.ToInt64(ChannelTestDriver.CurrentDataValue(dCurrentTime)));
                    m_DataIndex++;
                }
                // update the total number of data points already acquired
                m_DataIndexProperty.Update(m_DataIndex);

                m_PacketIndex++;
                m_MyCmDevice.UpdateDataEx(m_DataPacket);
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

