/////////////////////////////////////////////////////////////////////////////
//
// DelayedTerminationDriver.cs
// ///////////////////////////
//
// DelayedTerminationDriver Chromeleon DDK Code Example
//
// Device class for the DelayedTerminationDriver.
// The "DelayedTerminationDriver" creates one device with a "DelayTermination"
// and a "TerminationTimeout" property reflecting the values of
// IDevice.DelayTermination and IDevice.TerminationTimeout.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;

using Dionex.Chromeleon.DDK;	// Chromeleon DDK Interface
using Dionex.Examples.Utility;  // Utility class to simplify the driver configuration access


// Each Driver must have its unique namespace that should consist of
// <ManufacturerName>.<DriverName>.
// The DriverInfo.resx in the same namespace is used by the Instrument Configuration Manager
// to retrieve a FriendlyName and the DeviceManufacturer which refers to the
// hardware controlled by the driver.
// The ManufacturerSubgroup allows us to sort the drivers
// into functional groups.
// These strings may be localized.
namespace MyCompany.DelayedTerminationDriver
{
    /// <summary>
    /// Driver class implementation
    /// The driver class must be public to make it accessible for CM.

    /// The DriverIDAttribute is used to identify the driver in the Chromeleon
    /// client, e.g. to find a suitable instrument method editor plug-in.
    /// The DriverIDAttribute must be unique.
    /// </summary>
    [DriverIDAttribute("MyCompany.DelayedTerminationDriver")]
    public class Driver : IDriver
    {
        #region Data Members

        /// The driver configuration is saved as an XML string.
        private string m_Configuration;

        /// Our DelayedTerminationDevice.
        private DelayedTerminationDevice m_MyDevice;

        /// Our configuration parser.
        private ConfigurationParser m_ConfigParser;

        #endregion

        /// <summary>
        /// Construction
        /// </summary>
        public Driver()
        {
            // We need to provide a default configuration that is used
            // when we are loaded for the first time. The Instrument Configuration Manager
            // uses the default configuration to find out about configuration options
            // of the driver.
            // A driver must provide at least configurable device names to allow the
            // user to insert more than one device of the same type into an instrument.
            //
            Stream xmlStream = null;
            try
            {
                // Get the driver configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.DelayedTerminationDriver.DefaultConfig.xml");
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
        /// IDriver.Init is called after successful configuration of the driver.
        /// During IDriver.Init a driver must create its devices, properties, etc.
        /// </summary>
        /// <param name="cmDDK">The DDK instance that hosts the driver.</param>
        public void Init(IDDK cmDDK)
        {
            // This can be used as an entry point for debugging
            // System.Diagnostics.Debugger.Launch();

            m_ConfigParser = new ConfigurationParser(m_Configuration);

            // Create our device.
            m_MyDevice = new DelayedTerminationDevice();
            m_MyDevice.Create(cmDDK, m_ConfigParser.GetDeviceName("Delayed Termination Device"));
        }

        /// <summary>
        /// IDriver implementation: Exit
        /// IDriver.Exit is called before unloading the driver.
        /// </summary>
        public void Exit()
        {
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// During IDriver.Connect the driver must allocate communication
        /// resources, e.g. open a COM port and connect to the hardware.
        /// </summary>
        public void Connect()
        {
            // This can be used as an entry point for debugging
            // System.Diagnostics.Debugger.Launch();

            // Our DelayedTerminationDevice implementation implements a method OnConnect that will fill
            // a property.
            m_MyDevice.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// During IDriver.Disconnect the driver must disconnect from the
        /// hardware and free communication resources, e.g. close the COM port.
        /// </summary>
        public void Disconnect()
        {
            m_MyDevice.OnDisconnect();
        }

        /// <summary>
        /// IDriver implementation: Configuration
        /// Get/set the driver configuration
        /// </summary>
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
    }
}
