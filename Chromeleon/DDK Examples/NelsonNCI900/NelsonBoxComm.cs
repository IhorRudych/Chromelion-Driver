/////////////////////////////////////////////////////////////////////////////
//
// NelsonBoxComm.cs
// ////////////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Abstract base class for the NCI 900 communication layer
// Derived from C++ implementation.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Threading;

namespace MyCompany.NelsonNCI900
{
    /// <summary>
    /// NelsonBoxComm Nelson Box Communication Layer
    /// </summary>
    internal abstract class NelsonBoxComm
    {
        #region CONSTANTS

        public enum EBoxStatus
        {
            boxStatus_ready = 0,
            boxStatus_overflow = 1,
            boxStatus_sampling = 2,
            boxStatus_wrapped = 4,
            boxStatus_overflowWrapped = 5,
            boxStatus_samplingWrapped = 6,
            boxStatus_notReady = 7,
        };

        public enum EAcqChannel
        {
            acqChannel_none = 0,
            acqChannel_A = 1,
            acqChannel_B = 2,
            acqChannel_both = acqChannel_A | acqChannel_B,
            acqChannel_undefined = 4,	// for preflight
        };

        public const string CRLF = "\r\n";

        #endregion

        #region Data Members

        protected byte m_RelayState = 0;
        private Driver m_Driver;
        private bool m_ChannelA;
        private bool m_ChannelB;

        #endregion

        #region Construction

        public NelsonBoxComm(Driver driver)
        {
            m_Driver = driver;
        }

        #endregion

        #region Communication Interface

        public virtual bool Connect()
        {
            if (!llConnect())
                return false;

            string szReply;

            RawSend(CRLF);
            SendReceive("I", out szReply);

            Thread.Sleep(250);

            ReadLine(out szReply);

            FlushInBuffer();

            SendReceive("@", out szReply);

            ReadLine(out szReply);

            SendReceive("XEK6", out szReply);

            SendLine("ON");

            SendLine("L0");
            SendLine("OK0");
            SendLine("OX1");


            int voltageRangeIndex = 3;

            SetSamplingInterval(100.0);

            SendLine("R " + voltageRangeIndex.ToString());
            /*
                            int n = (int)(0.50 / Math.Min(0.01, samplingStep));
                            n = Math.Min(60, Math.Max(1, n));

                            SendLine("N " + n.ToString("00x"));
            */
            SendLine("N A");

            SendLine("C 0");

            return true;
        }

        public virtual bool Disconnect()
        {
            if (!llDisconnect())
                return false;

            return true;
        }

        public virtual bool ReadLine(out string szReply)
        {
            return llReadLine(out szReply, true);
        }

        public virtual bool SendLine(string szToSend)
        {
            return RawSend(szToSend);
        }

        public virtual bool SendReceive(string szToSend, out string szReply)
        {
            szReply = "";

            if (!SendLine(szToSend))
                return false;

            return ReadLine(out szReply);
        }

        public virtual bool RawSend(string sz)
        {
            return llRawSend(sz);
        }

        public virtual void FlushInBuffer()
        {
            llFlushInBuffer();
        }

        #endregion

        #region Low Level Communication

        public abstract bool llConnect();
        public abstract bool llDisconnect();
        public abstract bool llRawSend(string sz);
        public abstract bool llReadLine(out string sz, bool wait);
        public abstract void llFlushInBuffer();

        #endregion

        #region Relay State

        public bool GetRelayState(int index)
        {
            GetHwRelayState();

            if ((m_RelayState & (1 << index)) != 0)
                return true;
            else
                return false;
        }

        public void SetRelayState(int index, bool bState)
        {
            if (bState)
                m_RelayState |= (byte)(1 << index);
            else
                m_RelayState &= (byte)(~(1 << index));

            SetHwRelayState(bState);
        }

        #endregion

        #region Hardware Communication

        public abstract void GetHwRelayState();
        public abstract void SetHwRelayState(bool bState);
        public abstract void Acq(bool bState);

        public void Acq(int index, bool bState)
        {
            if (index == 0)
                m_ChannelA = bState;

            if (index == 1)
                m_ChannelB = bState;

            if (m_ChannelA || m_ChannelB)
                Acq(true);
            else if (!m_ChannelA && !m_ChannelB)
                Acq(false);
        }



        public void SetSamplingInterval(double newVal)
        {
            SendLine("S " + (100.0 / newVal).ToString());
        }

        #endregion

        #region Properties

        protected Driver Driver
        {
            get { return m_Driver; }
        }

        #endregion
    }
}
