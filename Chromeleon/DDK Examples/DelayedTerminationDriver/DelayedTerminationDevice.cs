/////////////////////////////////////////////////////////////////////////////
//
// DelayedTerminationDevice.cs
// ///////////////////////////
//
// DelayedTerminationDriver Chromeleon DDK Code Example
//
// Device class for the DelayedTerminationDriver.
// The "DelayedTerminationDriver" creates one device with a "DelayTermination"
// and a "TerminationTimeout" property reflecting the values of
// IDevice.DelayTermination and IDevice.TerminationTimeout.
// Note that these values can be set by the driver internally; they don't
// need to be connected to properties.
// In this example driver we use the properties for illustration purposes.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.DelayedTerminationDriver
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class DelayedTerminationDevice
    {
        #region Data Members

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// For illustration we create a property
        /// for the value of IDevice.DelayTermination.
        private IIntProperty m_DelayTerminationProperty;

        /// ... and also for IDevice.TerminationTimeout.
        private IIntProperty m_TerminationTimeoutProperty;

        /// Create a property to decide if the driver should
        /// terminate in time or timeout.
        private IIntProperty m_TerminateInTimeProperty;

        private RetentionTime m_EndTime;

        private System.Timers.Timer m_ProgramTimer = new System.Timers.Timer(1000);

        /// Real time when the RetentionTime has been zero
        private DateTime m_StartTime;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is device that illustrates the usage of IDevice.DelayTermination.");

            m_DelayTerminationProperty =
                m_MyCmDevice.CreateBooleanProperty("DelayTermination", "Enable/Disable termination delay.", "Off", "On");

            m_DelayTerminationProperty.OnSetProperty += new SetPropertyEventHandler(m_DelayTerminationProperty_OnSetProperty);

            ITypeInt tTimeout = cmDDK.CreateInt(0, 30);
            tTimeout.Unit = "s";
            m_TerminationTimeoutProperty =
                m_MyCmDevice.CreateProperty("TerminationTimeout", "The timeout for the delayed termination.", tTimeout);

            m_TerminationTimeoutProperty.OnSetProperty += new SetPropertyEventHandler(m_TerminationTimeoutProperty_OnSetProperty);

            m_TerminateInTimeProperty =
                m_MyCmDevice.CreateBooleanProperty("TerminateInTime", "Enable/Disable proper termination.", "Off", "On");
            m_TerminateInTimeProperty.OnSetProperty += new SetPropertyEventHandler(m_TerminateInTimeProperty_OnSetProperty);

            m_MyCmDevice.OnTransferPreflightToRun += new PreflightEventHandler(m_MyCmDevice_OnTransferPreflightToRun);

            m_MyCmDevice.OnBroadcast += new BroadcastEventHandler(m_MyCmDevice_OnBroadcast);

            m_ProgramTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_ProgramTimer_Elapsed);

            return m_MyCmDevice;
        }

        /// <summary>
        /// We use a timer to determine if the runtime is already finished.
        /// If TerminateInTime = On we will allow the instrument method to terminate
        /// properly by setting IDevice.DelayTermination=false half the
        /// interval given by IDevice.TerminationTimeout later.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_ProgramTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_TerminateInTimeProperty.Value.HasValue &&
                m_TerminateInTimeProperty.Value.Value == 1)
            {
                TimeSpan runTime = DateTime.Now - m_StartTime;

                TimeSpan expectedRunTime =
                    new TimeSpan(0, 0, m_EndTime.HundredthSeconds / 100) +
                    new TimeSpan(0, 0, m_TerminationTimeoutProperty.Value.Value / 2);

                if (runTime >= expectedRunTime)
                {
                    m_MyCmDevice.DelayTermination = false;
                    m_DelayTerminationProperty.Update(m_MyCmDevice.DelayTermination ? 1 : 0);
                }
            }
        }

        void m_MyCmDevice_OnBroadcast(BroadcastEventArgs args)
        {
            if (args.Broadcast == Broadcast.RetentionZeroCrossed)
            {
                m_ProgramTimer.Enabled = true;
                m_StartTime = DateTime.Now;
            }
            if (args.Broadcast == Broadcast.Sampleend)
            {
                m_ProgramTimer.Enabled = false;
            }
        }

        /// <summary>
        /// We use OnTransferPreflightToRun to find out the end time of the instrument method.
        /// Usually our run should end shortly after that time.
        /// </summary>
        /// <param name="args"></param>
        void m_MyCmDevice_OnTransferPreflightToRun(PreflightEventArgs args)
        {
            m_EndTime = args.RunContext.ProgramTime;
        }

        void m_TerminateInTimeProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs setIntArgs = args as SetIntPropertyEventArgs;
            if (setIntArgs.NewValue.HasValue)
            {
                m_TerminateInTimeProperty.Update(setIntArgs.NewValue.Value);
            }
        }

        void m_TerminationTimeoutProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs setIntArgs = args as SetIntPropertyEventArgs;
            if (setIntArgs.NewValue.HasValue)
            {
                m_TerminationTimeoutProperty.Update(setIntArgs.NewValue.Value);
                m_MyCmDevice.TerminationTimeout = setIntArgs.NewValue.Value;
            }
        }

        void m_DelayTerminationProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs setIntArgs = args as SetIntPropertyEventArgs;
            if (setIntArgs.NewValue.HasValue)
            {
                m_DelayTerminationProperty.Update(setIntArgs.NewValue.Value);
                m_MyCmDevice.DelayTermination =
                    (setIntArgs.NewValue.Value != 0) ? true : false;
            }
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize properties that are visible in the connected state only.
        /// </summary>
        internal void OnConnect()
        {
            m_DelayTerminationProperty.Update(m_MyCmDevice.DelayTermination ? 1 : 0);
            m_TerminationTimeoutProperty.Update(m_MyCmDevice.TerminationTimeout);
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// When we are disconnected, we clear properties that are visible in the connected state only.
        /// </summary>
        internal void OnDisconnect()
        {
        }
    }
}
