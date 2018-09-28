using System;
using System.Diagnostics;
using System.IO;

using Dionex.Chromeleon.DDK;	// Chromeleon DDK Interface
using Dionex.Examples.Utility;  // Utility class to simplify the driver configuration access

namespace MyCompany.Preflight
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.Preflight")]
    public class PreflightDriver : IDriver
    {
        private string m_Configuration;
        private int m_NumberOfDevices = 4;
        private PreflightDevice[] m_Devices;

        /// <summary>
        /// Driver class construction
        /// </summary>
        public PreflightDriver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.Preflight.DefaultConfig.xml");
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

            m_Devices = new PreflightDevice[m_NumberOfDevices];

            for (int i = 1; i <= m_NumberOfDevices; i++)
            {
                m_Devices[i - 1] = new PreflightDevice();
                m_Devices[i - 1].Create(cmDDK, configurationParser.GetDeviceName("Preflight Device " + i));
            }
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
            foreach (PreflightDevice device in m_Devices)
                device.OnConnect();
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
