/////////////////////////////////////////////////////////////////////////////
//
// LCSystem.cs
// ///////////
//
// ExampleLCSystem Chromeleon DDK Code Example
//
// Main device class for the ExampleLCSystem.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.ExampleLCSystem
{
    class LCSystem
    {
        #region Data Members

        private IDDK m_DDK;
        private IDevice m_Device;

        #endregion

        internal IDevice Device
        {
            get { return m_Device; }
        }

        internal void Create(IDDK cmDDK, string deviceName)
        {
            m_DDK = cmDDK;
            m_Device = m_DDK.CreateDevice(deviceName, "LCSystem device. This is our master device.");

            IStringProperty typeProperty =
                m_Device.CreateProperty("DeviceType",
                "The DeviceType property tells us which component we are talking to.",
                m_DDK.CreateString(20));
            typeProperty.Update("LCSystem");
        }
        internal void OnConnect()
        {
        }

        internal void OnDisconnect()
        {
        }
    }
}
