using System;
using System.Windows.Forms;
using System.Xml;
using Dionex.Chromeleon.DDK;

namespace MyCompany.SendReceiveConfig
{
    /// <summary>
    /// This class is an example, how to communicate with a "SendReceiveDriver" object 
    /// using the IConfigSendReceive/IDriverSendReceive interface.
    /// </summary>
    [DriverIDAttribute("MyCompany.SendReceive")]
    [CustomIconAttribute("SendReceive.bmp")]
    public partial class SendReceiveConfig : Form, IConfigurationPlugin, IConfigurationDescriptors
    {
        #region Data Members

        private XmlNode m_ConfigurationNode;
        private IConfigSendReceive m_SendReceive;

        #endregion // Data Members

        #region Construction

        /// <summary>
        /// Creates a new instance of a <see cref="SendReceiveConfig"/> class.
        /// </summary>
        public SendReceiveConfig()
        {
            InitializeComponent();
        }

        #endregion // Construction

        #region Events

        /// <summary>
        /// This method is called when the user clicks on the "OK" button.
        /// </summary>
        /// <remarks>
        /// The entered device name string is merged in the driver configuration.
        /// </remarks>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The arguments of the event</param>
        private void bu_Ok_Click(object sender, EventArgs e)
        {
            // Read the new device node name and modify the configuration
            XmlNode deviceNameNode = tBoxDeviceName.Tag as XmlNode;
            deviceNameNode.InnerText = tBoxDeviceName.Text;
            // Set the dialog result to OK - this is important for further procedures
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// This method is called when the user clicks on the "Cancel" button.
        /// </summary>
        /// <remarks>
        /// The entered device name string will NOT be merged in the driver configuration.
        /// </remarks>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The arguments of the event</param>
        private void bu_Cancel_Click(object sender, EventArgs e)
        {
            // Set the dialog result to Cancel - this is important for further procedures
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion // Events

        #region IConfigSendReceive usage
        /// <summary>
        /// This method is called when the user clicks on the "Send" button.
        /// </summary>
        /// <remarks>
        /// Uses the ISendReceive interface to send a string to the driver.
        /// The received answer will be displayed.
        /// </remarks>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The arguments of the event</param>
        private void butSend_Click(object sender, EventArgs e)
        {
            string outputString;
            m_SendReceive.SendReceiveEx(tBoxSend.Text, out outputString);
            tBoxReceive.Text = outputString;
        }
        #endregion // SendReceive usage

        #region IConfigurationPlugin Members

        /// <summary>
        /// This interface property is used to get the configuration report
        /// string
        /// </summary>
        string IConfigurationPlugin.ConfigurationReport
        {
            get
            {
                string deviceName = String.Empty;
                XmlNode deviceNameNode = m_ConfigurationNode.SelectSingleNode
                    ("Driver/" +
                    "Device[@name=\"Send/Receive Device\"]/" +
                    "Parameter[@name=\"Device Name\"]");
                if (deviceNameNode != null)
                {
                    deviceName = deviceNameNode.InnerText;
                }
                return "\tDevice Name: " + deviceName;
            }
        }

        /// <summary>
        /// This interface property is used to set or respectively get the
        /// driver configuration.
        /// </summary>
        string IConfigurationPlugin.Configuration
        {
            get
            {
                return m_ConfigurationNode.OuterXml;
            }
            set
            {
                XmlDocument configDoc = new XmlDocument();
                // Load the configuration string which is well-formed xml
                configDoc.LoadXml(value);
                // Extract the configuration node
                m_ConfigurationNode = configDoc.SelectSingleNode("/Configuration");
                // Get the device name node
                XmlNode deviceNameNode = m_ConfigurationNode.SelectSingleNode
                    ("Driver/" +
                    "Device[@name=\"Send/Receive Device\"]/" +
                    "Parameter[@name=\"Device Name\"]");

                tBoxDeviceName.Tag = deviceNameNode;
                tBoxDeviceName.Text = deviceNameNode.InnerText;
            }
        }

        /// <summary>
        /// This interface method is called to show the GUI of the driver configuration module.
        /// So the ShowDialog method of the <see cref="SendReceiveConfig"/> class must be
        /// called here.
        /// </summary>
        /// <param name="configDriverExchange">
        /// Base interface for communication between a driver and its configuration assembly.
        /// If the driver implements the IDriverSendReceive the configuration assembly 
        /// can use the "as" operator to access the IConfigSendReceive interface.
        /// A reference to this interface will be stored in m_SendReceive for later access.
        /// </param>
        /// <returns>
        /// TRUE if the user clicks the OK button of the configuration dialog. Otherwise FALSE.
        /// </returns>
        public bool ShowConfigurationDialog(IConfigDriverExchange configDriverExchange)
        {
            m_SendReceive = configDriverExchange as IConfigSendReceive;

            this.ShowDialog(configDriverExchange.ParentWindow);

            if (this.DialogResult == DialogResult.Cancel)
                return false;

            return true;
        }

        #endregion

        #region IConfigurationDescriptors Members

        public string ConnectInfo
        {
            get
            {
                // One usually returns a specific connection info here, e.g. COM port name, USB or
                // ethernet ID. If the driver is in simulation mode, use the keyword 'Simulation'.
                return "Simulation";
            }
        }

        public string InstrumentInfo(InstrumentID instID)
        {
            // Only required for drivers shared between instruments (timebases)
            // A 'simple' should just return nothing
            return String.Empty;
        }

        #endregion
    }
}