/////////////////////////////////////////////////////////////////////////////
//
// Driver.cs
// /////////
//
// ExampleLCSystem Chromeleon DDK Code Example
//
// Driver class for the ExampleLCSystem.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;
using Dionex.Examples.Utility;  // Utility class to simplify the driver configuration access

namespace MyCompany.ExampleLCSystem
{
    [DriverIDAttribute("MyCompany.ExampleLCSystem")]
    public class Driver : IDriver
    {
        #region Data Members

        private IDDK m_DDK;
        private string m_Configuration;
        private ConfigurationParser m_ConfigParser;

        private LCSystem m_LCSystem;
        private Pump m_Pump;
        private Sampler m_Sampler;
        private Detector m_Detector;

        #endregion

        public Driver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.ExampleLCSystem.DefaultConfig.xml");
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
                m_Configuration = value;
            }
        }

        public void Connect()
        {
            m_DDK.AuditMessage(AuditLevel.Normal, "ExampleLCSystemDriver.OnConnect");
            m_LCSystem.OnConnect();
            m_Pump.OnConnect();
            m_Sampler.OnConnect();
            m_Detector.OnConnect();
        }

        public void Disconnect()
        {
            m_DDK.AuditMessage(AuditLevel.Normal, "ExampleLCSystemDriver.OnDisconnect");
            m_LCSystem.OnDisconnect();
            m_Pump.OnDisconnect();
            m_Sampler.OnDisconnect();
            m_Detector.OnDisconnect();
        }

        public void Exit()
        {
        }

        public void Init(IDDK cmDDK)
        {
            m_DDK = cmDDK;

            m_ConfigParser = new ConfigurationParser(m_Configuration);

            m_LCSystem = new LCSystem();
            m_LCSystem.Create(cmDDK, m_ConfigParser.GetDeviceName("LC System"));

            m_Pump = new Pump();
            m_Pump.Create(cmDDK, m_ConfigParser.GetDeviceName("Pump"));
            m_Pump.Device.SetOwner(m_LCSystem.Device);

            m_Sampler = new Sampler();
            m_Sampler.Create(cmDDK, m_ConfigParser.GetDeviceName("Sampler"));
            m_Sampler.Device.SetOwner(m_LCSystem.Device);

            m_Detector = new Detector();
            m_Detector.Create(cmDDK, m_ConfigParser.GetDeviceName("Detector"));
            m_Detector.Device.SetOwner(m_LCSystem.Device);
        }

        #endregion
    }
}
