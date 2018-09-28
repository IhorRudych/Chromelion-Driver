/////////////////////////////////////////////////////////////////////////////
//
// LocalizedDriver.cs
// //////////////////
//
// Localization Chromeleon DDK Code Example
//
// Driver class for the Localization Example
// This example illustrates how to localize the help text in drivers.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Examples.Utility;					// ConfigurationParser

namespace MyCompany.Localization
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.Localization")]
    public class Driver : IDriver
    {
        #region Data Members

        /// Our device
        private LocalizedDevice m_MyDevice;

        /// Our configuration
        private string m_Configuration;

        #endregion

        /// <summary>
        /// Driver class construction
        /// </summary>
        public Driver()
        {
            // This is the preferable way to load our default configuration
            // Note that you must not localize the driver configuration.
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.Localization.DefaultConfig.xml");
                using (StreamReader xmlStreamReader = new StreamReader(xmlStream))
                {
                    m_Configuration = xmlStreamReader.ReadToEnd();
                }
            }
            catch (Exception err)
            {
                Trace.WriteLine(err.Message);
                throw;
            }
            finally
            {
                if (xmlStream != null)
                    xmlStream.Close();
            }
        }

        /// <summary>
        /// IDriver implementation: Init
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        public void Init(IDDK cmDDK)
        {
            m_MyDevice = new LocalizedDevice();

            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            m_MyDevice.Create(cmDDK, configurationParser.GetDeviceName("Localized Device"));
        }

        /// <summary>
        /// IDriver implementation: Exit
        /// </summary>
        public void Exit()
        {
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// </summary>
        public void Connect()
        {
            m_MyDevice.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// </summary>
        public void Disconnect()
        {
            m_MyDevice.OnConnect();
        }

        /// <summary>
        /// Get/set the driver configuration
        /// </summary>
        public string Configuration
        {
            get { return m_Configuration; }
            set { m_Configuration = value; }
        }
    }
}
