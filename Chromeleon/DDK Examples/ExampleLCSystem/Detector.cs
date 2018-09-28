/////////////////////////////////////////////////////////////////////////////
//
// Detector.cs
// ///////////
//
// ExampleLCSystem Chromeleon DDK Code Example
//
// Detector device class for the ExampleLCSystem.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;
using System.Timers;

namespace MyCompany.ExampleLCSystem
{
    class Detector
    {
        #region Data Members

        private IDDK m_DDK;
        private IChannel m_Channel;
        private System.Timers.Timer m_Timer = new System.Timers.Timer(1000);
        private int m_StartTime;
        private int m_Time;
        private IDoubleProperty m_WavelengthProperty;
        private double m_WaveLength = 200.0;

        #endregion

        internal IDevice Device
        {
            get { return m_Channel; }
        }

        internal void Create(IDDK cmDDK, string deviceName)
        {
            m_DDK = cmDDK;

            ITypeInt tSignal = m_DDK.CreateInt(0, 1000);
            tSignal.Unit = "mAU";
            m_Channel = m_DDK.CreateChannel(deviceName, "Detector device", tSignal);

            IStringProperty typeProperty =
                m_Channel.CreateProperty("DeviceType",
                "The DeviceType property tells us which component we are talking to.",
                m_DDK.CreateString(20));
            typeProperty.Update("Detector");

            m_Channel.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_Channel.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            ITypeDouble tWavelength = m_DDK.CreateDouble(200, 400, 1);
            tWavelength.Unit = "nm";
            m_WavelengthProperty = m_Channel.CreateStandardProperty(StandardPropertyID.Wavelength, tWavelength);
            m_WavelengthProperty.OnSetProperty += new SetPropertyEventHandler(OnSetWaveLength);

            // We want to have the wavelength available as a report variable and therefore add it to the channel info.
            m_Channel.AddPropertyToChannelInfo(m_WavelengthProperty);

            m_WavelengthProperty.Update(m_WaveLength);

            // What do we actually measure? 
            m_Channel.PhysicalQuantity = "Absorbance";


            m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        internal void OnConnect()
        {
            m_Channel.AcquisitionStateProperty.Update((int)AcquisitionState.Idle);

            // read this from the hardware
            m_WavelengthProperty.Update(m_WaveLength);
        }

        internal void OnDisconnect()
        {
            m_WavelengthProperty.Update(null);
        }

        public void OnSetWaveLength(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doublePropertyArgs = args as SetDoublePropertyEventArgs;
            m_WavelengthProperty.Update(doublePropertyArgs.NewValue.Value);
        }

        /// OnAcqOn will be called when CM calls StartAcq
        private void OnAcqOn(CommandEventArgs args)
        {
            // m_Time is the time in Chromeleon ticks (10 ms)
            m_Time = 0;

            // initialize our internal time to the current retention time
            // If the retention time is < 0 in this case, this is a 
            // manual data acquisition and retention time will start after the call.
            if (args.RetentionTime.HundredthSeconds > 0)
                m_Time = args.RetentionTime.HundredthSeconds;

            m_StartTime = m_Time;

            // enable the timer
            m_Timer.Enabled = true;
        }

        /// OnAcqOn will be called when CM calls StopAcq
        private void OnAcqOff(CommandEventArgs args)
        {
            // We will stop the data acquisition in OnTimedEvent
        }

        private void OnTimedEvent(object source, ElapsedEventArgs args)
        {
            // we use the timer to update our data.
            // The timer event occurs each second. 
            // We send data at a rate of 100 Hz, this means 
            // we have to create 100 data points for each timer event.

            Random rand = new Random(m_Time);

            DataPointEx[] data = new DataPointEx[100];

            for (int i = 0; i < 100; i++)
            {
                data[i] = new DataPointEx(m_Time * 10.0, Convert.ToDouble(rand.Next(0, 1000)));
                m_Time++;
            }

            // Send the new data to Chromeleon
            m_Channel.UpdateDataEx(data);

            // If acquisition was stopped disable the timer.
            if (m_Channel.AcquisitionStateProperty.Value == (int)AcquisitionState.Idle)
            {
                m_Timer.Enabled = false;

                // Notify Chromeleon that there will be no more data available.
                // We could also send this if our hardware has finished data acquisition.
                m_Channel.NoMoreData();
            }
        }
    }
}
