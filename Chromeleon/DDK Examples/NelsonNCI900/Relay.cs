/////////////////////////////////////////////////////////////////////////////
//
// Relay.cs
// ////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Relay class for the NCI 900 driver.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.NelsonNCI900
{
    /////////////////////////////////////////////////////////////////////////////
    /// Relay Device Class

    internal class Relay
    {
        #region Data Members

        private IDevice m_MyCmDevice;
        private IIntProperty m_PropState;
        private int m_State;
        private Driver m_Driver;
        private int m_Index;

        #endregion

        #region Construction

        public Relay(int index, Driver driver)
        {
            m_Index = index;
            m_Driver = driver;
        }

        #endregion

        /// We will create our properties in here.
        internal IDevice Create(IDDK cmDDK, string name)
        {
            m_MyCmDevice = cmDDK.CreateDevice(name, "Relay");

            ITypeInt tState = cmDDK.CreateInt(0, 1);

            m_PropState = m_MyCmDevice.CreateProperty("State",
                "Indicates or sets the relay's state.",
                tState);
            m_PropState.OnSetProperty += new SetPropertyEventHandler(OnSetState);

            return m_MyCmDevice;
        }


        /// OnSetState will be called when the State property is set by CM.
        private void OnSetState(SetPropertyEventArgs args)
        {
            if (args.Property == m_PropState)
            {
                SetIntPropertyEventArgs intPropertyArgs = args as SetIntPropertyEventArgs;
                Debug.Assert(intPropertyArgs.NewValue.HasValue);
                m_State = intPropertyArgs.NewValue.Value;

                if (m_State == 1)
                    Driver.Comm.SetRelayState(m_Index, true);
                else
                    Driver.Comm.SetRelayState(m_Index, false);

                m_PropState.Update(m_State);
            }
        }

        #region Properties

        public Driver Driver
        {
            get { return m_Driver; }
        }

        #endregion
    }
}
