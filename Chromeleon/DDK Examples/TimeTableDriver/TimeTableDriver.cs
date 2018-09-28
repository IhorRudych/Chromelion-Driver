/////////////////////////////////////////////////////////////////////////////
//
// TimeTableDriver.cs
// //////////////////
//
// TimeTableDriver Chromeleon DDK Code Example
//
// This driver creates a device with a few ramped properties and documents 
// how to use interfaces for SetProperty, SetRamp and SetTimeTable events.
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

namespace MyCompany.TimeTableDriver
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.TimeTableDriver")]
    public class Driver : IDriver
    {
        #region Data Members

        /// Our device.
        private TimeTableDevice m_Device;

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
                    ("MyCompany.TimeTableDriver.DefaultConfig.xml");
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
            cmDDK.AuditMessage(AuditLevel.Message, "MyCompany.TimeTableDriver.Driver.Init()");

            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Create our device.
            m_Device = new TimeTableDevice();
            m_Device.Create(cmDDK, configurationParser.GetDeviceName("Time Table Device"));
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
            m_Device.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// </summary>
        public void Disconnect()
        {
            // Disconnect all our devices
            m_Device.OnDisconnect();
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



