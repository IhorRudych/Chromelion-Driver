/////////////////////////////////////////////////////////////////////////////
//
// Channel.cs
// //////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Channel class for the NCI 900 driver.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.NelsonNCI900
{
    /////////////////////////////////////////////////////////////////////////////
    /// Channel Device Class

    internal class Channel
    {
        #region Data Members

        private IChannel m_MyCmDevice;
        private Driver m_Driver;

        private int m_ChannelIndex;

        #endregion

        #region Construction

        public Channel(Driver driver, int channelIndex)
        {
            m_Driver = driver;
            m_ChannelIndex = channelIndex;
        }

        #endregion

        #region IChannel

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IChannel and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IChannel Create(IDDK cmDDK, string name)
        {
            ITypeInt tSignal = cmDDK.CreateInt(-1000, 1000);
            m_MyCmDevice = cmDDK.CreateChannel(name, "Nelson NCI 900 Channel", tSignal);

            m_MyCmDevice.AcquisitionOffCommand.OnCommand += new CommandEventHandler(OnAcqOff);
            m_MyCmDevice.AcquisitionOnCommand.OnCommand += new CommandEventHandler(OnAcqOn);

            m_MyCmDevice.TimeStepFactorProperty.DataType.Minimum = 100;
            m_MyCmDevice.TimeStepFactorProperty.DataType.Maximum = 100;
            m_MyCmDevice.TimeStepFactorProperty.Writeable = false;

            m_MyCmDevice.TimeStepDivisorProperty.DataType.Minimum = 1;
            m_MyCmDevice.TimeStepDivisorProperty.DataType.Maximum = 100;
            m_MyCmDevice.TimeStepDivisorProperty.OnSetProperty += new SetPropertyEventHandler(OnTimeStepDivisor);

            return m_MyCmDevice;
        }

        internal void OnConnect()
        {
            m_MyCmDevice.TimeStepFactorProperty.Update(100);
            m_MyCmDevice.TimeStepDivisorProperty.Update(100);
            m_MyCmDevice.AcquisitionStateProperty.Update((int)AcquisitionState.Idle);
        }

        internal void OnDisconnect()
        {
        }

        /// OnAcqOn will be called when CM calls StartAcq
        private void OnAcqOn(CommandEventArgs args)
        {
            Driver.Comm.Acq(m_ChannelIndex, true);
        }
        private void OnAcqOff(CommandEventArgs args)
        {
            Driver.Comm.Acq(m_ChannelIndex, false);
        }

        private void OnTimeStepDivisor(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intPropertyArgs = args as SetIntPropertyEventArgs;
            m_MyCmDevice.TimeStepDivisorProperty.Update(intPropertyArgs.NewValue);

            Driver.Comm.SetSamplingInterval((double)intPropertyArgs.NewValue);
        }

        #endregion

        /// <summary>
        /// Forward incoming data to our Chromeleon channel device
        /// </summary>
        /// <param name="data">array of data points</param>
        internal void UpdateData(int[] data)
        {
            if (m_MyCmDevice.AcquisitionStateProperty.Value != (int)AcquisitionState.Idle)
                m_MyCmDevice.UpdateData(0, data);
        }

        #region Properties

        public Driver Driver
        {
            get { return m_Driver; }
        }

        #endregion
    }
}
