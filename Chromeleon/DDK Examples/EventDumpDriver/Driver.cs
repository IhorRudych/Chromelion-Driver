/////////////////////////////////////////////////////////////////////////////
//
// Driver.cs
// //////////
//
// EventDumpDriver Chromeleon DDK Code Example
//
// Driver class for the EventDumpDriver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;

using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.EventDumpDriver
{
    // A driver that dumps all events to the audit trail

    [DriverIDAttribute("MyCompany.Examples.EventDump")]
    public class Driver : IDriver
    {
        #region Data Members

        private String m_Configuration;
        private IDDK m_DDK;
        private Device m_Device;

        #endregion // Data Members

        #region Construction

        public Driver()
        {
            // This is the preferable way to load our default configuration
            // Note that you must not localize the driver configuration.
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.EventDumpDriver.DefaultConfiguration.xml");
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

        #endregion // Construction

        #region IDriver Members

        string IDriver.Configuration
        {
            get
            {
                if (m_DDK != null)
                    m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Configuration.get() called");
                return m_Configuration;
            }
            set
            {
                if (m_DDK != null)
                    m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Configuration.set() called");
                m_Configuration = value;
            }
        }

        void IDriver.Init(IDDK cmDDK)
        {
            m_DDK = cmDDK;
            m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Init() called");
            m_Device = new Device(m_DDK);
        }

        void IDriver.Connect()
        {
            m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Connect() called");
            m_Device.Connect();
        }

        void IDriver.Disconnect()
        {
            m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Disconnect() called");
            m_Device.Disconnect();
        }

        void IDriver.Exit()
        {
            m_DDK.AuditMessage(AuditLevel.Normal, "IDriver.Exit() called");
            m_Device.Exit();
        }

        #endregion
    }
}
