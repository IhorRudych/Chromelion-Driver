/////////////////////////////////////////////////////////////////////////////
//
// TimestampedChannelEx.cs
// ///////////////////////
//
// SinusChannel Chromeleon DDK Code Example
//
// Channel class for the SinusChannel example driver.
// The SinusChannel driver creates two channel devices that
// generate a sinus or sawtooth waveform during acquisition.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Timers;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.SinusChannel
{
    //////////////////////////////////////////////////////////////////////////////////////////
    /// Channel Device Class
    ///
    /// This channel sends a sawtooth signal with variable data rate (Data_Collection_Rate)
    ///
    /// The interpolation rate can be also set (but not during acquisition)
    internal class TimestampedChannelEx
    {
        #region Data Members

        /// our Chromeleon DDK channel interface
        private IChannel m_MyCmDevice;

        /// We use a timer to send a data packet each second.
        private System.Timers.Timer m_Timer = new System.Timers.Timer(1000);

        // the retention time at which AcqOn is set (in ns)
        private double m_startTime;

        // the time at which AcqOn is set
        private DateTime m_startDateTime;

        // the time at which the latest data point was set
        private DateTime m_lastestEventCall;

        // the data collection rate property
        private IDoubleProperty m_dcr;


        // the collection mode: 
        // 0=Regular will produce equidistant data points at a given rate (may not be changed during acquisition)
        // 1=Arbitrary will produce not equidistant data points, the mean rate is given by data collection rate 
        // (may be changed during acquisition)
        private IIntProperty m_dcrmode;

        //
        // The signal type: possible values are
        // 0 = SawTooth
        // 1 = Sinus 
        // the frequency of both signals is 1 / 10s = 0.1 Hz 
        private IIntProperty m_signalType;

        /// This is our internal time used to generate the sinus curve.
        private int m_totalDataIdx;

        #endregion

        #region Construction

        public TimestampedChannelEx()
        {
            m_Timer.AutoReset = false;
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        #endregion

        #region IChannel

        internal IChannel Create(IDDK cmDDK, string name)
        {
            // Create a data type for our signal
            ITypeInt tSignal = cmDDK.CreateInt(-100, 100);

            // Create the channel symbol
            m_MyCmDevice = cmDDK.CreateChannel(name, "This is a channel that can generate a sinus curve.", tSignal);

            // Attach our handlers to the AcquisitionOffCommand and AcquisitionOnCommand of the channel
            m_MyCmDevice.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_MyCmDevice.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            m_MyCmDevice.TimeStepFactorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepFactorProperty.DataType.Maximum = 1;
            m_MyCmDevice.TimeStepFactorProperty.Writeable = false;

            m_MyCmDevice.TimeStepDivisorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepDivisorProperty.DataType.Maximum = 1;
            m_MyCmDevice.TimeStepDivisorProperty.Writeable = false;

            m_dcr = m_MyCmDevice.CreateProperty("Data_Collection_Rate", "This is the data collection rate of the device", cmDDK.CreateDouble(0.1, 300.0, 1));
            m_dcr.OnPreflightSetProperty += new SetPropertyEventHandler(m_dcr_OnPreflightSetProperty);
            m_dcr.OnSetProperty += new SetPropertyEventHandler(m_dcr_OnSetProperty);

            ITypeInt dcrModeType = cmDDK.CreateInt(0, 1);
            dcrModeType.AddNamedValue("Regular", 0);
            dcrModeType.AddNamedValue("Random", 1);

            m_dcrmode = m_MyCmDevice.CreateProperty("DCRMode", "Mode in which the data points are sent - regular interval or arbitrary", dcrModeType);
            m_dcrmode.OnSetProperty += new SetPropertyEventHandler(m_dcrmode_OnSetProperty);
            m_dcrmode.OnPreflightSetProperty += new SetPropertyEventHandler(m_dcrmode_OnPreflightSetProperty);

            ITypeInt signalTypeType = cmDDK.CreateInt(0, 1);
            signalTypeType.AddNamedValue("SawTooth", 0);
            signalTypeType.AddNamedValue("Sinus", 1);
            m_signalType = m_MyCmDevice.CreateProperty("SignalType", "The signal type of the simulation mode data - SawTooth or Sinus (frequency 0.1Hz)", signalTypeType);
            m_signalType.OnSetProperty += new SetPropertyEventHandler(m_signalType_OnSetProperty);

            return m_MyCmDevice;
        }

        void m_signalType_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            if (intArgs != null)
            {
                m_signalType.Update(intArgs.NewValue);
            }
        }

        void m_dcrmode_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            if (m_MyCmDevice.AcquisitionStateProperty.Value.HasValue &&
                m_MyCmDevice.AcquisitionStateProperty.Value.Value != (int)AcquisitionState.Idle)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Abort, "Cannot change the internal acquisition mode during data acquisition");
            }
        }

        void m_dcr_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            if (m_MyCmDevice.AcquisitionStateProperty.Value.HasValue &&
                m_MyCmDevice.AcquisitionStateProperty.Value.Value != (int)AcquisitionState.Idle &&
                m_dcrmode.Value.HasValue &&
                m_dcrmode.Value.Value == 0)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Abort, "Cannot change the data collection rate during data acquisition if in regular mode");
            }
        }

        void m_dcrmode_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            if (intArgs != null)
            {
                m_dcrmode.Update(intArgs.NewValue);
            }
        }

        void m_dcr_OnSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs dblArgs = args as SetDoublePropertyEventArgs;
            if (dblArgs != null)
            {
                m_dcr.Update(dblArgs.NewValue);
            }
        }

        internal void Connect()
        {
            // initialize our properties
            m_MyCmDevice.TimeStepFactorProperty.Update(1);
            m_MyCmDevice.TimeStepDivisorProperty.Update(1);
            m_MyCmDevice.AcquisitionStateProperty.Update((int)AcquisitionState.Idle);
            m_MyCmDevice.SignalFactorProperty.Update(1.0E-6);
            m_dcr.Update(100.0);
            m_dcrmode.Update(1);
            m_signalType.Update(1);
        }

        internal void Disconnect()
        {
        }

        /// OnAcqOn will be called when CM calls StartAcq
        private void OnAcqOn(CommandEventArgs args)
        {
            // initialize our internal time to the current retention time (ns)
            m_startTime = args.RetentionTime.Initial ? 0.0 : args.RetentionTime.HundredthSeconds * 1.0E7;
            m_startDateTime = DateTime.Now;
            m_lastestEventCall = DateTime.Now;
            m_totalDataIdx = 0;

            // enable the timer
            m_Timer.Enabled = true;
        }

        /// OnAcqOn will be called when CM calls StopAcq
        private void OnAcqOff(CommandEventArgs args)
        {
        }

        #endregion

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (m_dcrmode.Value.HasValue &&
                m_dcrmode.Value.Value == 1)
            {
                TimedEventArbitrary(source, e);
            }
            else
            {
                TimedEventRegular(source, e);
            }

            // If acquisition was stopped disable the timer.
            if (m_MyCmDevice.AcquisitionStateProperty.Value == (int)AcquisitionState.Idle)
            {
                m_Timer.Enabled = false;

                // Notify Chromeleon that there will be no more data available.
                // We could also send this if our hardware has finished data acquisition.
                m_MyCmDevice.NoMoreData();
            }
            else
            {
                m_Timer.Enabled = true;
            }
        }

        private void TimedEventArbitrary(Object source, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSincelatestCall = now - m_lastestEventCall;
            TimeSpan timeSinceAcqStart = now - m_startDateTime;

            double timeSincePrevCall = timeSincelatestCall.TotalSeconds;
            int noOfPointsToSend = Convert.ToInt32(timeSincePrevCall * m_dcr.Value);
            if (noOfPointsToSend <= 0)
            {
                Debug.WriteLine(String.Format("Nothing to send: {0:f1} ns", timeSincePrevCall));
                return;
            }

            m_lastestEventCall = now;

            DataPointInt64[] data = new DataPointInt64[noOfPointsToSend];

            double startTimeThisPackage = m_startTime + (timeSinceAcqStart.TotalMilliseconds - timeSincelatestCall.TotalMilliseconds) * 1.0E6;

            for (int i = 0; i < noOfPointsToSend; i++)
            {
                double currentTime = startTimeThisPackage + (timeSincelatestCall.TotalMilliseconds * i * 1.0E6) / noOfPointsToSend;
                data[i] = new DataPointInt64(Convert.ToInt64(currentTime), Convert.ToInt64(DemoSignal(currentTime)));
                m_totalDataIdx++;
            }

            Trace.WriteLine(String.Format("First Point: {0:f3}, Value {1:f3}, Total: {2:d}", data[0].Retention, data[0].Data, m_totalDataIdx));
            Trace.WriteLine(String.Format("Last Point : {0:f3}, Value {1:f3}, Total: {2:d}", data[noOfPointsToSend - 1].Retention, data[noOfPointsToSend - 1].Data, m_totalDataIdx));
            // Send the new data to Chromeleon
            m_MyCmDevice.UpdateDataEx(data);
        }

        private void TimedEventRegular(Object source, ElapsedEventArgs e)
        {
            // we use the timer to update our data
            // the timer occurs each second.

            TimeSpan timeSinceAcqStart = DateTime.Now - m_startDateTime;

            int actNoOfPoints = Convert.ToInt32(timeSinceAcqStart.TotalSeconds * m_dcr.Value);

            int dataToBeSent = actNoOfPoints - m_totalDataIdx;
            if (dataToBeSent <= 0)
                return;


            DataPointInt64[] data = new DataPointInt64[dataToBeSent];

            for (int i = 0; i < dataToBeSent; i++)
            {
                double currentTime = m_startTime + m_totalDataIdx * 1.0E9 / m_dcr.Value.Value; // current retention time in ns

                data[i] = new DataPointInt64(Convert.ToInt64(currentTime), Convert.ToInt64(DemoSignal(currentTime)));
                m_totalDataIdx++;
            }

            Trace.WriteLine(String.Format("First Point: {0:f3}, Value {1:f3}, Total: {2:d}", data[0].Retention, data[0].Data, m_totalDataIdx));
            Trace.WriteLine(String.Format("Last Point : {0:f3}, Value {1:f3}, Total: {2:d}", data[dataToBeSent - 1].Retention, data[dataToBeSent - 1].Data, m_totalDataIdx));
            // Send the new data to Chromeleon
            m_MyCmDevice.UpdateDataEx(data);
        }

        /// <summary>
        /// Returns a demo signal as a function of time (in ns)
        /// </summary>
        /// <param name="time"> time (ns)</param>
        /// <returns>the demo signal</returns>
        private double DemoSignal(double time)
        {
            if (m_signalType.Value.HasValue && m_signalType.Value.Value == 0)
            {
                return 1.0E6 * Convert.ToDouble(Convert.ToInt32(time) % 10000000000);
            }
            else
            {
                return 1.0E6 * Math.Sin(2 * Math.PI * time / 1.0E10);
            }
        }
    }



}
