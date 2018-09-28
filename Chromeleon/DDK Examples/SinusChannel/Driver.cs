/////////////////////////////////////////////////////////////////////////////
//
// Driver.cs
// /////////
//
// SinusChannel Chromeleon DDK Code Example
//
// Driver class for the SinusChannel example driver.
// The SinusChannel driver creates two channel devices that
// generate a sinus waveform during acquisition.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface
using Dionex.Examples.Utility;					// Utility class to simplify the driver configuration access

namespace MyCompany.SinusChannel
{
    /// <summary>
    /// class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.SinusChannel")]
    public class Driver : IDriver
    {
        #region Data Members

        /// A channel device sending data at a fixed rate
        private Channel m_Channel;

        /// A channel device sending data at random times (using the new DDK data interface)
        private TimestampedChannelEx m_TimestampedChannelEx;

        /// Our configuration
        private string m_Configuration;

        #endregion

        /// <summary>
        /// Construction
        /// </summary>
        public Driver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.SinusChannel.DefaultConfig.xml");
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
            cmDDK.AuditMessage(AuditLevel.Message, "SinusChannel.Driver.Init()");

            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // create our channel
            m_Channel = new Channel();
            m_Channel.Create(cmDDK, configurationParser.GetDeviceName("Sinus Channel"));

            m_TimestampedChannelEx = new TimestampedChannelEx();
            m_TimestampedChannelEx.Create(cmDDK, configurationParser.GetDeviceName("Timestamped Sinus Channel Ex"));
        }

        /// <summary>
        /// IDriver implementation: Exit
        /// </summary>
        public void Exit()
        {
            // Nothing to do.
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// </summary>
        public void Connect()
        {
            m_Channel.Connect();
            m_TimestampedChannelEx.Connect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// </summary>
        public void Disconnect()
        {
            m_Channel.Disconnect();
            m_TimestampedChannelEx.Disconnect();
        }

        /// <summary>
        /// Get/set the driver configuration
        /// </summary>
        public string Configuration
        {
            get { return m_Configuration; }
            set { m_Configuration = value; }
        }

        #region Configuration Parser

        /// <summary>
        /// Retrieves the device name from the configuration. Device names must always be configurable to allow
        /// the user to insert more than one device of the same type into an instrument.
        /// </summary>
        /// <param name="desiredDevice">Our internal device name.</param>
        /// <returns></returns>
        internal string GetDeviceName(string desiredDevice)
        {
            return ConfigurationParameter(desiredDevice, "Device Name", desiredDevice);
        }

        /// <summary>
        /// Retrieve a parameter from the configuration. The configuration is built as an XML string.
        /// We find our parameters in the Configuration/Driver/Device/Parameter nodes.
        /// </summary>
        /// <param name="desiredDevice">The device of interest</param>
        /// <param name="desiredParameter">The parameter of interest</param>
        /// <param name="defaultValue">A default string to be used if the parameter is not set.</param>
        /// <returns></returns>
        internal string ConfigurationParameter(string desiredDevice, string desiredParameter, string defaultValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Configuration);

            string xPath =
                "Configuration/Driver/Device[@name=\"" +
                desiredDevice +
                "\"]/Parameter[@name=\"" +
                desiredParameter +
                "\"]";

            XmlNodeList xmlNodes = xmlDoc.SelectNodes(xPath);
            Debug.Assert(xmlNodes.Count == 1);
            if (xmlNodes.Count != 1)
            {
                return defaultValue;
            }

            return xmlNodes.Item(0).InnerText;
        }

        #endregion
    }
}
