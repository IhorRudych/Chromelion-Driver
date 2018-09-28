using System.Diagnostics;
using System.Xml;
using System.Globalization;

namespace Dionex.Examples.Utility
{
    /// <summary>
    /// Summary description for ConfigurationParser
    /// This is a helper class for parsing the XML configuration.
    /// </summary>
    public class ConfigurationParser
    {
        #region Data Members
        private XmlDocument m_XmlDoc;
        #endregion

        /// <summary>
        /// Loads the configuration string
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigurationParser(string configuration)
        {
            m_XmlDoc = new XmlDocument();
            m_XmlDoc.LoadXml(configuration);
        }

        /// <summary>
        /// Retrieves the device name from the configuration. Device names must always be configurable to allow
        /// the user to insert more than one device of the same type into an instrument.
        /// </summary>
        /// <param name="desiredDevice">Our internal device name.</param>
        /// <returns></returns>
        public string GetDeviceName(string desiredDevice)
        {
            return DeviceParameter(desiredDevice, "Device Name", desiredDevice);
        }

        /// <summary>
        /// Retrieves the maximum number of detection channels from the configuration..
        /// </summary>
        /// <param name="desiredDevice">Our internal device name.</param>
        /// <returns></returns>
        public int GetMaximumNumberOfDectectionChannels(string desiredDevice)
        {
            return int.Parse(DeviceParameter(desiredDevice, "Maximum Number Of Detection Channels", desiredDevice), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retrieve a parameter from the configuration. The configuration is built as an XML string.
        /// We find our parameters in the Configuration/Driver/Device/Parameter nodes.
        /// </summary>
        /// <param name="desiredDevice">The device of interest</param>
        /// <param name="desiredParameter">The parameter of interest</param>
        /// <param name="defaultValue">A default string to be used if the parameter is not set.</param>
        /// <returns></returns>
        public string DeviceParameter(string desiredDevice, string desiredParameter, string defaultValue)
        {
            string xPath =
                "Configuration/Driver/Device[@name=\"" +
                desiredDevice +
                "\"]/Parameter[@name=\"" +
                desiredParameter +
                "\"]";

            XmlNodeList xmlNodes = m_XmlDoc.SelectNodes(xPath);
            Debug.Assert(xmlNodes.Count == 1);
            if (xmlNodes.Count != 1)
            {
                return defaultValue;
            }

            return xmlNodes.Item(0).InnerText;
        }

        /// <summary>
        /// Retrieve a parameter from the configuration. The configuration is built as an XML string.
        /// We find our parameters in the Configuration/Driver/desiredNode/Parameter nodes.
        /// </summary>
        /// <param name="desiredNode">The configuration node of interest</param>
        /// <param name="desiredParameter">The parameter of interest</param>
        /// <param name="defaultValue">A default string to be used if the parameter is not set.</param>
        /// <returns></returns>
        public string Parameter(string desiredNode, string desiredParameter, string defaultValue)
        {
            string xPath =
                "Configuration/Driver/" +
                desiredNode +
                "/Parameter[@name=\"" +
                desiredParameter +
                "\"]";

            XmlNodeList xmlNodes = m_XmlDoc.SelectNodes(xPath);
            Debug.Assert(xmlNodes.Count == 1);
            if (xmlNodes.Count != 1)
            {
                return defaultValue;
            }

            return xmlNodes.Item(0).InnerText;
        }
    }
}