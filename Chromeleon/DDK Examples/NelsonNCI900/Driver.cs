/////////////////////////////////////////////////////////////////////////////
//
// Driver.cs
// /////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Driver class for the NCI 900 driver.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface
using Dionex.Examples.Utility;					// ConfigurationParser

namespace MyCompany.NelsonNCI900
{
    #region ENUMERATIONS

    enum NumberOf
    {
        Relays = 6,
        Channels = 2,
    };


    #endregion

    /// <summary>
    /// Driver class implementation
    /// </summary>
    [DriverIDAttribute("MyCompany.NelsonNCI900")]
    public class Driver : IDriver
    {
        #region Data Members

        private IDDK m_DDK;		// the IDDK object, that created us
        private NelsonBoxComm m_Comm;		// our hardware communication layer
        private Master m_MasterDev;		// our master device
        private Channel[] m_Channel;		// our channel device
        private Relay[] m_RelayDev;			// our relays
        private string m_Configuration;		// Our configuration
        private string m_ComPort;			// Our Com port

        #endregion

        /// <summary>
        /// Driver class construction
        /// </summary>
        public Driver()
        {
            Stream xmlStream = null;
            try
            {
                // Get the default configuration from the manifest
                xmlStream = this.GetType().Assembly.GetManifestResourceStream
                    ("MyCompany.NelsonNCI900.DefaultConfig.xml");
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
            cmDDK.AuditMessage(AuditLevel.Message, "MyCompany.NelsonNCI900.Driver.Init()");

            m_DDK = cmDDK;

            ConfigurationParser configurationParser =
                new ConfigurationParser(m_Configuration);

            // Which COM port should be used?
            m_ComPort =
                configurationParser.Parameter("Communication", "Port", "COM1");

            // create our master device
            m_MasterDev = new Master(this);
            IDevice iMaster = m_MasterDev.Create(cmDDK, configurationParser.GetDeviceName("Interface"));

            // create our relays
            m_RelayDev = new Relay[(int)NumberOf.Relays];
            for (int i = 0; i < (int)NumberOf.Relays; i++)
            {
                m_RelayDev[i] = new Relay(i, this);
                IDevice pDevice = m_RelayDev[i].Create(cmDDK, configurationParser.GetDeviceName("Relay " + (i + 1).ToString()));
                pDevice.SetOwner(iMaster);
            }

            m_Channel = new Channel[(int)NumberOf.Channels];

            // create our channel
            m_Channel[0] = new Channel(this, 0);
            IChannel channel = m_Channel[0].Create(cmDDK, configurationParser.GetDeviceName("Channel A"));
            channel.SetOwner(iMaster);

            // create our channel
            m_Channel[1] = new Channel(this, 1);
            channel = m_Channel[1].Create(cmDDK, configurationParser.GetDeviceName("Channel B"));
            channel.SetOwner(iMaster);
        }

        /// <summary>
        /// IDriver implementation: Exit
        /// </summary>
        public void Exit()
        {
            // Nothing to do.
        }

        /// <summary>
        /// IDriver implementation: Connect our hardware
        /// </summary>
        public void Connect()
        {
            // Create our hardware communication layer.
            m_Comm = new SerialImpl(this, m_ComPort);

            m_Comm.Connect();
            m_MasterDev.OnConnect();
            m_Channel[0].OnConnect();
            m_Channel[1].OnConnect();
        }

        /// <summary>
        /// IDriver implementation: Disconnect our hardware.
        /// </summary>
        public void Disconnect()
        {
            m_MasterDev.OnDisconnect();
            m_Comm.Disconnect();
        }

        /// <summary>
        /// Get/set the driver configuration
        /// </summary>
        public string Configuration
        {
            get { return m_Configuration; }
            set { m_Configuration = value; }
        }

        #region Properties

        public IDDK DDK
        {
            get { return m_DDK; }
        }

        internal NelsonBoxComm Comm
        {
            get { return m_Comm; }
        }

        internal Channel Channel(int i)
        {
            return m_Channel[i];
        }

        #endregion
    }
}
