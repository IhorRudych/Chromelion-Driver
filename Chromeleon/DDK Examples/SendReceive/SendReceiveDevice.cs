/////////////////////////////////////////////////////////////////////////////
//
// SendReceiveDevice.cs
// ////////////////////
//
// SendReceive Chromeleon DDK Code Example
//
// Device class for the SendReceive driver.
// The "SendReceive" driver illustrates how to implement custom communication
// between a driver and its configuration module.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.SendReceive
{
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    internal class SendReceiveDevice
    {
        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// Our (only) property.
        private IStringProperty m_ModelNoProperty;

        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Property
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "SendReceive DDK Example.");

            // Create the standard Property containing our model number
            // There are standard properties with a special meaning for CM and a simplified
            // syntax. For details, see the documentation.
            m_ModelNoProperty =
                m_MyCmDevice.CreateStandardProperty(StandardPropertyID.ModelNo, cmDDK.CreateString(20));

            return m_MyCmDevice;
        }

        /// When we are connected, we update our model number.
        internal void OnConnect()
        {
            m_ModelNoProperty.Update("SendReceive Model");
        }

        /// When we are disconnected, we clear our model number.
        internal void OnDisconnect()
        {
            m_ModelNoProperty.Update("");
        }
    }
}
