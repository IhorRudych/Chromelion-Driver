/////////////////////////////////////////////////////////////////////////////
//
// SendReceiveDriver.cs
// ////////////////////
//
// SendReceive Chromeleon DDK Code Example
//
// Driver class for the SendReceive driver.
// The "SendReceive" driver illustrates how to implement custom communcation
// between a driver and its configuration module.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;

using Dionex.Chromeleon.DDK;	// Chromeleon DDK Interface
using Dionex.Examples.Utility;  // Utility class to simplify the driver configuration access

using Dionex.Chromeleon.Symbols;


// Each Driver must have its unique namespace that should consist of
// <ManufacturerName>.<DriverName>.
namespace MyCompany.SendReceive
{
    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.SendReceive")]
    public class Driver : IDriver, IDriverSendReceive
    {
        #region Data Members

        /// The driver configuration is saved as an XML string.
        private string m_Configuration;
        /// Our SendReceiveDevice.
        private SendReceiveDevice m_MyDevice;
        /// Our ddk instance.
        private IDDK m_DDK;

        #endregion // Data Members

        /// <summary>
        /// Driver class construction
        /// </summary>
        public Driver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.SendReceive.DefaultConfig.xml");
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

        /// <summary>IDriver implementation: Init</summary>
        /// <remarks>
        /// IDriver.Init is called after successful configuration of the driver.
        /// During IDriver.Init a driver must create its devices, properties, etc.
        /// </remarks>
        /// <param name="cmDDK">The DDK instance</param>
        public void Init(IDDK cmDDK)
        {
            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Remember our DDK instance.
            m_DDK = cmDDK;

            // Create our device.
            m_MyDevice = new SendReceiveDevice();
            m_MyDevice.Create(cmDDK, configurationParser.GetDeviceName("Send/Receive Device"));
        }

        /// <summary>IDriver implementation: Exit</summary>
        /// <remarks>
        /// IDriver.Exit is called before unloading the driver.
        /// </remarks>
        public void Exit()
        {
        }

        /// <summary>IDriver implementation: Connect</summary>
        /// <remarks>
        /// During IDriver.Connect the driver must allocate communication
        /// resources, e.g. open a COM port and connect to the hardware.
        /// </remarks>
        public void Connect()
        {
            // Our SendReceiveDevice implementation implements a method OnConnect that will fill
            // a property.
            m_MyDevice.OnConnect();
        }

        /// <summary>IDriver implementation: Disconnect</summary>
        /// <remarks>
        /// During IDriver.Disconnect the driver must disconnect from the
        /// hardware and free communication resources, e.g. close the COM port.
        /// </remarks>
        public void Disconnect()
        {
            m_MyDevice.OnDisconnect();
        }

        /// <summary>IDriver implementation: Configuration</summary>
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

        #region IDriverSendReceive Members

        /// <summary>
        /// This method is called to exchange data (string) with the driver. The main purpose
        /// is to communicate with the driver configuration module.
        /// </summary>
        /// <remarks>
        /// Note that OnSendReceive can be called BEFORE Init, therefore we receive the calling
        /// IDDK instance as a parameter. When a driver is initially added to the instrument,
        /// it is loaded but it will not be initialized until the user has confirmed the
        /// configuration.
        /// </remarks>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="inputString">The string which is sent to the driver</param>
        /// <param name="outputString">The answer string from the driver</param>
        public void OnSendReceive(IDDK cmDDK, string inputString, out string outputString)
        {
            // Write the input to the audit trail.
            cmDDK.AuditMessage(AuditLevel.Message, "OnSendReceive: inputString = " + inputString);

            // Fill in some response.
            outputString = DateTime.Now.ToString() + " - " + inputString;
            cmDDK.AuditMessage(AuditLevel.Message, "OnSendReceive: outputString = " + outputString);
        }

        #endregion
    }
}
