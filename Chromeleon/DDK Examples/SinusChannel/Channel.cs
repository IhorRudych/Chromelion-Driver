/////////////////////////////////////////////////////////////////////////////
//
// Channel.cs
// //////////
//
// SinusChannel Chromeleon DDK Code Example
//
// Channel class for the SinusChannel example driver.
// The SinusChannel driver creates two channel devices that
// generate a sinus waveform during acquisition.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Timers;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.SinusChannel
{
    /////////////////////////////////////////////////////////////////////////////
    /// Channel Device Class

    internal class Channel
    {
        #region Data Members

        /// our Chromeleon DDK channel interface
        private IChannel m_MyCmDevice;

        /// We use a timer to send a data packet each second.
        private System.Timers.Timer m_Timer = new System.Timers.Timer(1000);

        /// This is our internal time used to generate the sinus curve.
        private int m_totalDataIdx;

        /// We can also write a spectrum
        private ISpectrumWriter m_SpectrumWriter;

        private IDoubleProperty m_WavelengthProperty;

        #endregion

        #region Construction

        public Channel()
        {
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        #endregion

        #region IChannel

        /// Create our Chromeleon symbols
        internal IChannel Create(IDDK cmDDK, string name)
        {
            // Create a data type for our signal
            ITypeInt tSignal = cmDDK.CreateInt(-100, 100);
            tSignal.Unit = "mV";

            // Create the channel symbol
            m_MyCmDevice = cmDDK.CreateChannel(name, "This is a channel that can generate a sinus curve.", tSignal);

            // Attach our handlers to the AcquisitionOffCommand and AcquisitionOnCommand of the channel
            m_MyCmDevice.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_MyCmDevice.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            m_MyCmDevice.TimeStepFactorProperty.DataType.Minimum = 100;
            m_MyCmDevice.TimeStepFactorProperty.DataType.Maximum = 100;
            m_MyCmDevice.TimeStepFactorProperty.Writeable = false;

            m_MyCmDevice.TimeStepDivisorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepDivisorProperty.DataType.Maximum = 100;
            m_MyCmDevice.TimeStepDivisorProperty.OnSetProperty += new SetPropertyEventHandler(OnTimeStepDivisor);

            ITypeDouble tWavelength = cmDDK.CreateDouble(200, 400, 1);
            tWavelength.Unit = "nm";
            m_WavelengthProperty = m_MyCmDevice.CreateStandardProperty(StandardPropertyID.Wavelength, tWavelength);

            // We want to have the wavelength available as a report variable and therefore add it to the channel info.
            m_MyCmDevice.AddPropertyToChannelInfo(m_WavelengthProperty);

            // What do we actually measure?
            m_MyCmDevice.PhysicalQuantity = "MotorCurrent";

            // This channel doesn't have peaks, we don't want to have it integrated.
            m_MyCmDevice.NeedsIntegration = false;

            // Create a command for writing a spectrum
            ICommand writeSpectrumCommand = m_MyCmDevice.CreateCommand("WriteSpectrum", "Write a spectrum");
            writeSpectrumCommand.OnCommand += new CommandEventHandler(OnWriteSpectrum);

            // Create an interface for spectrum writing
            m_SpectrumWriter = m_MyCmDevice.CreateSpectrumWriter();

            return m_MyCmDevice;
        }

        internal void Connect()
        {
            // initialize our properties
            m_MyCmDevice.TimeStepFactorProperty.Update(100);
            m_MyCmDevice.TimeStepDivisorProperty.Update(60);
            m_MyCmDevice.AcquisitionStateProperty.Update((int)AcquisitionState.Idle);
            m_WavelengthProperty.Update(200.0);
        }

        internal void Disconnect()
        {
        }

        /// OnAcqOn will be called when CM calls StartAcq
        private void OnAcqOn(CommandEventArgs args)
        {
            // initialize our internal time to the current retention time
            m_totalDataIdx = 0;

            // enable the timer
            m_Timer.Enabled = true;
        }

        /// OnAcqOn will be called when CM calls StopAcq
        private void OnAcqOff(CommandEventArgs args)
        {
            // We will stop the data acquisition in OnTimedEvent
        }

        /// Write a spectrum
        private void OnWriteSpectrum(CommandEventArgs args)
        {
            m_SpectrumWriter.Name = "Spectrum";
            m_SpectrumWriter.Comment = "This is a sample spectrum";
            m_SpectrumWriter.Retention = args.RetentionTime;
            m_SpectrumWriter.SignalFactor = 1.0;
            m_SpectrumWriter.WavelengthMinimum = 2000;
            m_SpectrumWriter.WavelengthMaximum = 4000;
            m_SpectrumWriter.Unit = "AU";

            int[] DataPoints = new int[200];

            for (int i = 0; i < 200; i++)
                DataPoints[i] = i;

            m_SpectrumWriter.DataPoints = DataPoints;

            m_SpectrumWriter.Save();
        }

        private void OnTimeStepDivisor(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intPropertyArgs = args as SetIntPropertyEventArgs;
            m_MyCmDevice.TimeStepDivisorProperty.Update(intPropertyArgs.NewValue);
        }

        #endregion

        private void OnTimedEvent(object source, ElapsedEventArgs args)
        {
            // we use the timer to update our data.
            // The timer event occurs each second.
            // We send data at a rate of 100 Hz, this means we have to create 100 data points for each timer event.

            int[] data = new int[60];

            for (int i = 0; i < 60; i++)
            {
                data[i] = (int)(m_totalDataIdx % 60) * 10000;
                m_totalDataIdx++;
            }

            // Send the new data to Chromeleon
            m_MyCmDevice.UpdateData(0, data);

            // If acquisition was stopped disable the timer.
            if (m_MyCmDevice.AcquisitionStateProperty.Value == (int)AcquisitionState.Idle)
            {
                m_Timer.Enabled = false;

                // Notify Chromeleon that there will be no more data available.
                // We could also send this if our hardware has finished data acquisition.
                m_MyCmDevice.NoMoreData();
            }
        }
    }
}
