/////////////////////////////////////////////////////////////////////////////
//
// FractionCollectorDriver.cs
// //////////////////////////
//
// FractionCollector Chromeleon DDK Code Example
//
// Driver class for the FractionCollector.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;

using Dionex.Chromeleon.DDK;	// Chromeleon DDK Interface
using Dionex.Examples.Utility;  // Utility class to simplify the driver configuration access

namespace MyCompany.FractionCollector
{
    [DriverIDAttribute("MyCompany.FractionCollector")]
    public class FractionCollectorDriver : IDriver
    {
        #region Data Members

        /// The driver configuration is saved as an XML string.
        private string m_Configuration;

        /// Our FractionCollectorDevice.
        private FractionCollectorDevice m_FractionCollectorDevice;

        #endregion

        /// <summary>
        /// Construction
        /// </summary>
        public FractionCollectorDriver()
        {
            // Debug.Fail("Do you want to debug FractionCollectorDriver?");

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
                    ("MyCompany.FractionCollector.DefaultConfig.xml");
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
            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Create our device.
            m_FractionCollectorDevice = new FractionCollectorDevice();
            m_FractionCollectorDevice.Create(cmDDK,
                configurationParser.GetDeviceName("FractionCollector"),
                configurationParser.GetMaximumNumberOfDectectionChannels("FractionCollector")
            );
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
            m_FractionCollectorDevice.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// During IDriver.Disconnect the driver must disconnect from the
        /// hardware and free communication resources, e.g. close the COM port.
        /// </summary>
        public void Disconnect()
        {
            m_FractionCollectorDevice.OnDisconnect();
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
