/////////////////////////////////////////////////////////////////////////////
//
// ChannelTestDriver.cs
// ////////////////////
//
// ChannelTest Chromeleon DDK Code Example
//
// Driver class for the ChannelTest example driver.
// The ChannelTest driver creates three channel devices that
// generate a cosinus waveform during acquisition and
// a PDA channel device.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.ChannelTest
{
    /// <summary>
    /// class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.ChannelTest")]
    public class ChannelTestDriver : IDriver
    {
        #region Data Members

        /// The DDK instance
        private IDDK m_CmDDK;

        /// Our master device
        private ChannelTestDevice m_MasterDevice;

        /// A channel device sending data at a fixed rate
        private FixedRateChannel m_FixedRateChannel;

        /// A channel device sending time stamped data points (double, double)
        private TimestampedChannelDouble m_TimestampedChannelDouble;

        /// A channel device sending time stamped data points (Int64, Int64)
        private TimestampedChannelInt64 m_TimestampedChannelInt64;

        /// A channel device sending spectra
        private PDAChannel m_PDAChannel;

        /// Our configuration
        private string m_Configuration;

        #endregion

        /// <summary>
        /// Construction
        /// </summary>
        public ChannelTestDriver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.ChannelTest.DefaultConfiguration.xml");
                using (StreamReader xmlStreamReader = new StreamReader(xmlStream))
                {
                    m_Configuration = xmlStreamReader.ReadToEnd();
                }
            }
            finally
            {
                if (xmlStream != null)
                    xmlStream.Close();
            }
        }


        #region IDriver Members

        /// <summary>
        /// IDriver implementation: Init
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        public void Init(IDDK cmDDK)
        {
            m_CmDDK = cmDDK;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(m_Configuration);

            // create the master device
            m_MasterDevice = new ChannelTestDevice();
            IDevice master = m_MasterDevice.OnCreate(this, cmDDK, DeviceName(doc, "ChannelTest"));

            // create the fixed rate channel
            m_FixedRateChannel = new FixedRateChannel();
            m_FixedRateChannel.OnCreate(cmDDK, master, DeviceName(doc, "FixedRateChannel"));

            // create the timestamped channel (double, double)
            m_TimestampedChannelDouble = new TimestampedChannelDouble();
            m_TimestampedChannelDouble.OnCreate(cmDDK, master, DeviceName(doc, "TimestampedChannel"));

            // create the timestamped channel (Int64, Int64)
            m_TimestampedChannelInt64 = new TimestampedChannelInt64();
            m_TimestampedChannelInt64.OnCreate(cmDDK, master, DeviceName(doc, "Int64Channel"));

            // create the spectral channel
            m_PDAChannel = new PDAChannel();
            m_PDAChannel.OnCreate(cmDDK, master, DeviceName(doc, "PDAChannel"));
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
            m_MasterDevice.OnConnect();
            m_FixedRateChannel.OnConnect();
            m_TimestampedChannelDouble.OnConnect();
            m_TimestampedChannelInt64.OnConnect();
            m_PDAChannel.OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// </summary>
        public void Disconnect()
        {
            m_MasterDevice.OnDisconnect();
            m_FixedRateChannel.OnDisconnect();
            m_TimestampedChannelDouble.OnDisconnect();
            m_TimestampedChannelInt64.OnDisconnect();
            m_PDAChannel.OnDisconnect();
        }

        /// <summary>
        /// Get/set the driver configuration
        /// </summary>
        public string Configuration
        {
            get { return m_Configuration; }
            set { m_Configuration = value; }
        }

        #endregion

        #region Internals

        /// <summary>
        /// Get the device name from the default configuration resource
        /// </summary>
        /// <param name="doc">Default configuration loaded into an XML document</param>
        /// <param name="name">Identifier of the device</param>
        /// <returns>Device name as configured by the user</returns>
        private string DeviceName(XmlDocument doc, string name)
        {
            string xPath =
                "Configuration/Driver/Device[@name=\"" + name + "\"]/Parameter[@name=\"Device Name\"]";
            return doc.SelectSingleNode(xPath).InnerText;
        }

        #endregion

        /// <summary>
        /// Returns a data value for the current time. This function is used by both channel classes.
        /// The data value is calculated from a cosinus curve
        /// The time axis starts with 0 at the first data point
        /// Note that the rate of the cosinus is independent from our channel parameters.
        /// This allows us to investigate how parameter changes affect the data.
        /// </summary>
        /// <param name="dCurrentTime">Current channel time in minutes</param>
        /// <returns></returns>
        internal static int CurrentDataValue(double dCurrentTime)
        {
            return (int)(1000.0 * (100.0 * Math.Cos(dCurrentTime * Math.PI * 10)) + 0.5);
        }

        /// <summary>
        /// Called by the masterdevice to simulate a hardware error, e.g.
        /// caused by lost communication. In this case, we must stop
        /// sending data as a real hardware would do and then disconnect.
        /// </summary>
        internal void OnHardwareError()
        {
            // Stop all channels
            m_FixedRateChannel.OnHardwareError();
            m_TimestampedChannelDouble.OnHardwareError();
            m_TimestampedChannelInt64.OnHardwareError();
            m_PDAChannel.OnHardwareError();

            // Disconnect
            m_CmDDK.Disconnect();
        }
    }
}
