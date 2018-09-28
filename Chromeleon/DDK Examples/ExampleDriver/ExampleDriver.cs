/////////////////////////////////////////////////////////////////////////////
//
// ExampleDriver.cs
// ////////////////
//
// ExampleDriver Chromeleon DDK Code Example
//
// This driver creates devices with a few example properties and commands.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface
using Dionex.Examples.Utility;					// ConfigurationParser

namespace MyCompany.ExampleDriver
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.ExampleDriver")]
    public class Driver : IDriver
    {
        #region Data Members

        /// Our devices.
        private ExampleDevice m_Device1;
        private ExampleDevice m_Device2;

        /// Our configuration.
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
                    ("MyCompany.ExampleDriver.DefaultConfig.xml");
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
            // Send a message to the audit trail
            cmDDK.AuditMessage(AuditLevel.Message, "MyCompany.ExampleDriver.Driver.Init()");

            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Create our devices.
            m_Device1 = new ExampleDevice();
            m_Device1.Create(cmDDK, configurationParser.GetDeviceName("Example Device 1"));

            m_Device2 = new ExampleDevice();
            m_Device2.Create(cmDDK, configurationParser.GetDeviceName("Example Device 2"));
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
            // Connect all our devices
            m_Device1.OnConnect();
            m_Device2.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// </summary>
        public void Disconnect()
        {
            // Disconnect all our devices
            m_Device1.OnDisconnect();
            m_Device2.OnDisconnect();
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



