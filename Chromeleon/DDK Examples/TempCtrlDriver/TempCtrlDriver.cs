/////////////////////////////////////////////////////////////////////////////
//
// TempCtrlDriver.cs
// /////////////////
//
// SendReceive Chromeleon DDK Code Example
//
// Driver class for the TempCtrl driver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface
using Dionex.Examples.Utility;					// ConfigurationParser


namespace MyCompany.TempCtrlDriver
{
    [DriverIDAttribute("MyCompany.TempCtrlDriver")]
    public class TempCtrlDriver : IDriver
    {

        TempCtrlDevice m_TempCtrlDevice;
        /// The driver configuration is saved as an XML string.
        string m_Configuration;

        public TempCtrlDriver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.TempCtrlDriver.DefaultConfig.xml");
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


        #region IDriver Members

        public string Configuration
        {
            get
            {
                return m_Configuration;
            }
            set
            {
                // A driver should verify the configuration before setting it.
                // If the configuration is corrupted or cannot be applied
                // the driver should throw an exception in here.
                m_Configuration = value;
            }
        }

        public void Connect()
        {
            m_TempCtrlDevice.OnConnect();
        }

        public void Disconnect()
        {
            m_TempCtrlDevice.OnDisconnect();
        }

        public void Exit()
        {
        }

        public void Init(IDDK cmDDK)
        {
            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Send a message to the audit trail
            cmDDK.AuditMessage(AuditLevel.Message, "MyCompany.TempCtrlDriver.Init()");

            m_TempCtrlDevice = new TempCtrlDevice();
            m_TempCtrlDevice.Create(cmDDK, configurationParser.GetDeviceName("Oven Device"));
        }

        #endregion
    }
}
