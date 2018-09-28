/////////////////////////////////////////////////////////////////////////////
//
// DownloadDriver.cs
// /////////////////
//
// Download Chromeleon DDK Code Example
//
// Driver class for the Download driver.
// The "Download" example driver illustrates how preflighting 
// in Chromeleon can be used to implement a download driver.
// Open the "DownloadTest.cmbx" to restore a sequence and instrument method for testing the driver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Examples.Utility;					// ConfigurationParser

namespace MyCompany.Download
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.Download")]
    public class DownloadDriver : IDriver
    {
        #region Data Members

        private string m_Configuration;
        private DownloadDevice m_Device1;
        private DownloadDevice m_Device2;

        #endregion

        /// <summary>
        /// Driver class construction
        /// </summary>
        public DownloadDriver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.Download.DefaultConfig.xml");
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
            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            m_Device1 = new DownloadDevice();
            m_Device1.Create(cmDDK, configurationParser.GetDeviceName("Download Device 1"));

            m_Device2 = new DownloadDevice();
            m_Device2.Create(cmDDK, configurationParser.GetDeviceName("Download Device 2"));
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
            m_Device1.OnConnect();
            m_Device2.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect 
        /// </summary>
        public void Disconnect()
        {
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
