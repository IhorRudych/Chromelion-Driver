/////////////////////////////////////////////////////////////////////////////
//
// Channel.cs
// //////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Lan communication layer implementation
// Illustration code, not a complete implementation.
// This code is not working!
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MyCompany.NelsonNCI900
{
    /// <summary>
    /// LanImpl Lan Communication Layer
    /// </summary>
    internal class LanImpl : NelsonBoxComm
    {
        #region CONSTANTS

        const string SZ_IP_ADDRESS = "172.20.61.107";
        const int IP_PORT = 3000;

        #endregion

        #region Data members

        private Socket m_Socket;

        private IPEndPoint m_EP =
            new IPEndPoint(IPAddress.Parse(SZ_IP_ADDRESS), IP_PORT);

        #endregion

        #region Construction

        public LanImpl(Driver driver) :
            base(driver)
        {
        }

        #endregion

        #region Low Level Communication

        public override bool llConnect()
        {
            try
            {
                m_Socket =
                    new Socket(AddressFamily.InterNetwork, SocketType.Stream, (ProtocolType)0);

                if (m_Socket != null)
                {
                    m_Socket.Connect(m_EP);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception during connect.", e.ToString());
                return false;
            }
            return true;
        }

        public override bool llDisconnect()
        {
            m_Socket.Close();

            return true;
        }

        public override bool llRawSend(string sz)
        {
            return true;
        }

        public override bool llReadLine(out string szReply, bool wait)
        {
            szReply = "";
            return true;
        }

        public override void llFlushInBuffer()
        {
        }

        #endregion

        #region Hardware Communication

        public override void GetHwRelayState()
        {
        }

        public override void SetHwRelayState(bool bState)
        {
        }

        public override void Acq(bool bState)
        {
        }

        #endregion
    }
}
