/////////////////////////////////////////////////////////////////////////////
//
// BlobDataDriver.cs
// /////////////////
//
// Download Chromeleon DDK Code Example
//
// Device class for the Blob data driver.
// The "BlobDataDriver" example driver illustrates how Chromeleon handles embedded binary data objects 
// which are used to transport data from the instrument method editor plug-in to the driver. 
// The driver needs to define a unique ID in order to access the data blob, which is part of the run context. 
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Examples.Utility;					// ConfigurationParser

namespace MyCompany.BlobDataDriver
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.BlobDataDriver")]
    public class BlobDataDriver : IDriver, IDriverGenericXmlRequest
    {
        #region Data Members

        private string m_Configuration;
        private BlobDataDevice m_Device;
        #endregion

        /// <summary>
        /// Driver class construction
        /// </summary>
        public BlobDataDriver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.BlobDataDriver.DefaultConfig.xml");
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

            m_Device = new BlobDataDevice();
            m_Device.Create(cmDDK, configurationParser.GetDeviceName("Blob data device"));

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
            m_Device.OnConnect();
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

        #region IDriverGenericXmlRequest Members

        public void OnGenericXmlRequest(IDDK cmDDK, string inputString, out string outputString)
        {
            XmlDocument xmlDocIn = new XmlDocument();
            xmlDocIn.LoadXml(inputString);
            XmlNode customXmlNode = xmlDocIn.SelectSingleNode("//CustomXML");
            if (customXmlNode == null)
            {
                outputString = "<Reply><Error>Unknown custom XML</Error></Reply>";
                return;
            }

            if (customXmlNode.InnerText == "...") // for round trip testing
            {
                outputString = "<Reply><Success>" + inputString + "</Success></Reply>";
            }
            else
            {
                String response = m_Device.DoSomething(customXmlNode.InnerText);

                XmlDocument xmlDocOut = new XmlDocument();
                xmlDocOut.PreserveWhitespace = true;
                XmlNode replyNode = xmlDocOut.CreateNode(XmlNodeType.Element, "Reply", "");
                xmlDocOut.AppendChild(replyNode);
                XmlNode successNode = xmlDocOut.CreateNode(XmlNodeType.Element, "Success", "");
                successNode.InnerText = response;
                replyNode.AppendChild(successNode);
                outputString = xmlDocOut.OuterXml;
            }
        }

        #endregion
    }
}
