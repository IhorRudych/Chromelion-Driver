/////////////////////////////////////////////////////////////////////////////
//
// LocalizedDevice.cs
// //////////////////
//
// Localization Chromeleon DDK Code Example
// 
// Device class for the Localization Example
// This example illustrates how to localize the help text in drivers.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.Localization
{
    /// <summary>
    /// LocalizedDevice class implementation
    /// </summary>
    class LocalizedDevice
    {
        #region Data Members

        private IDevice m_MyCmDevice;
        private IStringProperty m_statusProperty;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Load the help text from the resource
            string deviceHelpText =
                Properties.Resources.LocalizedDevice_HelpText;

            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, deviceHelpText);

            // Load the help text from the resource
            string statusPropertyText =
                Properties.Resources.StatusProperty_HelpText;

            // create a test Property containing a string
            m_statusProperty =
                m_MyCmDevice.CreateProperty("Status", statusPropertyText, cmDDK.CreateString(20));
            m_statusProperty.Update("Disconnected.");

            return m_MyCmDevice;
        }

        /// <summary>
        /// When we are connected, we update our status property.
        /// </summary>
        internal void OnConnect()
        {
            m_statusProperty.Update("Connected.");
        }

        /// <summary>
        /// When we are disconnected, we update our status property.
        /// </summary>
        internal void OnDisconnect()
        {
            m_statusProperty.Update("Disconnected.");
        }
    }
}
