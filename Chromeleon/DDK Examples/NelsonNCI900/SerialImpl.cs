/////////////////////////////////////////////////////////////////////////////
//
// SerialImpl.cs
// /////////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Serial communication layer implementation.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.IO.Ports;

// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.NelsonNCI900
{
    /////////////////////////////////////////////////////////////////////////////
    /// LanImpl Lan Communication Layer

    internal class SerialImpl : NelsonBoxComm
    {
        #region CONSTANTS

        #endregion

        #region Data Members

        private SerialPort m_SerialPort;
        private string m_ReadBuffer = "";
        private char m_SendSequenceNumber = '0';
        private AutoResetEvent m_GotLineEvent = new AutoResetEvent(false);
        private System.Timers.Timer m_DataPollTimer = new System.Timers.Timer(1000);
        private Mutex m_ReadBufferMutex = new Mutex();

        #endregion

        #region Construction

        public SerialImpl(Driver driver, string comPort) :
            base(driver)
        {
            m_SerialPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
            m_SerialPort.Handshake = Handshake.RequestToSend;
            m_SerialPort.ReadTimeout = 2000;
            m_SerialPort.WriteTimeout = 2000;
            m_DataPollTimer.Elapsed += new ElapsedEventHandler(OnPollData);
        }

        #endregion

        #region Low Level Communication

        public override bool llConnect()
        {
            try
            {
                m_SerialPort.Open();
                m_SerialPort.NewLine = CRLF;
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(m_SerialPort_ReceivedEvent);
                m_DataPollTimer.Enabled = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception caught.", e.ToString());
                return false;
            }
            return true;
        }

        public override bool llDisconnect()
        {
            m_DataPollTimer.Enabled = false;
            m_SerialPort.Close();

            return true;
        }

        public override bool SendLine(string szToSend)
        {
            return SendLineEx(szToSend, false);
        }

        public bool SendLineEx(string szToSend, bool bReplyExpected)
        {
            ++m_SendSequenceNumber;
            if (m_SendSequenceNumber > '9')
                m_SendSequenceNumber = '0';

            string szSendBuffer = "c";
            szSendBuffer += m_SendSequenceNumber;
            szSendBuffer += szToSend;

            uint checkSum = 0;
            foreach (char c in szSendBuffer)
                checkSum += ((byte)c);

            szSendBuffer += "|";
            szSendBuffer += StoreN64(checkSum, 3);
            szSendBuffer += CRLF;

            bool done = false;
            while (!done)
            {
                if (RawSend(szSendBuffer))
                {
                    string szReply = "";

                    m_ReadBufferMutex.WaitOne();
                    while (m_ReadBuffer.IndexOf(CRLF) >= 0)
                    {
                        // if this is an AsyncReply, remove and handle it
                        if (IsAsyncReply(m_ReadBuffer))
                        {
                            Trace.WriteLine("OnPollData async reply");
                            llReadLine(out szReply, false);
                            HandleAsyncReply(szReply);
                        }
                        else break;
                    }
                    m_ReadBufferMutex.ReleaseMutex();

                    if (llReadLine(out szReply, true))
                    {
                        if (szReply.Length == 1 && szReply[0] == '+')
                            done = true;
                        else
                            if (szReply.Length == 1 && szReply[0] == '-')
                                Trace.WriteLine("CheckSum Error!");
                            else
                                if (bReplyExpected && szReply[0] == 'r' &&
                                IsCheckSumOK(szReply))
                                {
                                    done = true;
                                }
                                else
                                {
                                    llRawSend("-" + CRLF);
                                }
                    }
                }
            }
            llRawSend("+" + CRLF);

            return done;
        }

        public override bool SendReceive(string szToSend, out string szReply)
        {
            szReply = "";

            if (!SendLine(szToSend))
                return false;

            return llReadLine(out szReply, true);
        }

        public override bool llRawSend(string szToSend)
        {
            Driver.DDK.AuditMessage(AuditLevel.Message,
                "<S>" + szToSend.Substring(0, szToSend.Length - 2));
            byte[] buf = m_SerialPort.Encoding.GetBytes(szToSend);
            m_SerialPort.Write(buf, 0, szToSend.Length);
            return true;
        }

        public override bool llReadLine(out string szReply, bool wait)
        {
            m_ReadBufferMutex.WaitOne();

            szReply = "";

            int nIndex = m_ReadBuffer.IndexOf(CRLF);

            if (nIndex < 0)
            {
                if (!wait)
                {
                    m_ReadBufferMutex.ReleaseMutex();
                    return false;
                }

                m_ReadBufferMutex.ReleaseMutex();

                m_GotLineEvent.Reset();
                if (!m_GotLineEvent.WaitOne(1000, true))
                {
                    Trace.WriteLine("Timeout");
                    return false;
                }

                m_ReadBufferMutex.WaitOne();
                nIndex = m_ReadBuffer.IndexOf(CRLF);
            }

            szReply = m_ReadBuffer.Substring(0, nIndex);
            m_ReadBuffer = m_ReadBuffer.Substring(nIndex + 2);

            Driver.DDK.AuditMessage(AuditLevel.Message, "<R>" + szReply);
            m_ReadBufferMutex.ReleaseMutex();
            return true;
        }

        public override void llFlushInBuffer()
        {
            m_ReadBufferMutex.WaitOne();
            m_ReadBuffer = "";
            m_SerialPort.DiscardInBuffer();
            m_ReadBufferMutex.ReleaseMutex();
        }

        #endregion

        #region Hardware Communication

        public override void GetHwRelayState()
        {
        }

        public override void SetHwRelayState(bool bState)
        {
            string szCommand = "A";

            byte m;
            for (m = 1 << ((int)NumberOf.Relays - 1); m > 0; m >>= 1)
            {
                if ((m_RelayState & m) != 0)
                    szCommand += '1';
                else
                    szCommand += '0';
            }
            if (bState)
                szCommand += '+';
            else
                szCommand += '*';

            Driver.Comm.SendLine(szCommand);
        }

        public override void Acq(bool bState)
        {
            string szCommand = "";

            if (bState)
            {
                szCommand += "BB";
                //_DataPollTimer.Enabled = true;
            }
            else
            {
                szCommand += 'E';
                //_DataPollTimer.Enabled = false;
            }

            SendLine(szCommand);

            string szReply;
            SendReceive("V", out szReply);
        }

        #endregion

        private void m_SerialPort_ReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            m_ReadBufferMutex.WaitOne();

            // we received any data, append them to our read buffer
            m_ReadBuffer += m_SerialPort.ReadExisting();

            // Check for complete packets
            while (m_ReadBuffer.IndexOf(CRLF) >= 0)
            {
                // if this is an AsyncReply, remove and handle it
                if (IsAsyncReply(m_ReadBuffer))
                {
                    Trace.WriteLine("ReceivedEvent async reply");
                    string szReply;
                    llReadLine(out szReply, false);
                    HandleAsyncReply(szReply);
                }
                else
                {
                    Trace.WriteLine("ReceivedEvent got line");
                    m_GotLineEvent.Set();
                    break;
                }
            }
            m_ReadBufferMutex.ReleaseMutex();
        }

        private void OnPollData(object source, ElapsedEventArgs e)
        {
            m_ReadBufferMutex.WaitOne();

            // Check for complete packets
            while (m_ReadBuffer.IndexOf(CRLF) >= 0)
            {
                // if this is an AsyncReply, remove and handle it
                if (IsAsyncReply(m_ReadBuffer))
                {
                    Trace.WriteLine("OnPollData async reply");
                    string szReply;
                    llReadLine(out szReply, false);
                    HandleAsyncReply(szReply);
                }
                else
                    if (m_ReadBuffer[0] == '+')
                    {
                        string szReply;
                        llReadLine(out szReply, false);
                    }
                    else break;
            }
            m_ReadBufferMutex.ReleaseMutex();
        }

        #region Implementation Helpers

        const int n64_bits = 6;
        const uint n64_mask = ((1 << n64_bits) - 1);
        const char n64_baseChar = ';';
        const char n64_lastChar = (char)(n64_baseChar + ((1 << n64_bits) - 1));

        string StoreN64(uint value, int digits)
        {
            string szN64 = "";

            while (digits-- > 0)
                szN64 += (char)(n64_baseChar + ((value >> (n64_bits * digits)) & n64_mask));

            return szN64;
        }

        bool IsCheckSumOK(string szReply)
        {
            uint checkSum = 0;
            int nIndex = szReply.IndexOf('|');
            if (nIndex <= 0)
                return false;

            foreach (char c in szReply.Substring(0, nIndex))
                checkSum += (byte)c;

            return (checkSum == ReadN64(szReply.Substring(nIndex + 1), 3));
        }

        uint ReadN64(string szCheckSum, int digits)
        {
            int nIndex = 0;
            uint result = 0;
            while (digits-- > 0)
            {
                if (szCheckSum[nIndex] < n64_baseChar || szCheckSum[nIndex] > n64_lastChar)
                    Driver.DDK.AuditMessage(AuditLevel.Error, "CheckSum Error!");
                result = (uint)((result << n64_bits) + (szCheckSum[nIndex++] - n64_baseChar));
            }
            return result;
        }

        bool IsAsyncReply(string szReply)
        {
            bool result = false;
            if (szReply == null || szReply.Length == 0)
                return false;

            switch (szReply[0])
            {
                case 's':
                case 'a':
                case 'b':
                case 'e':
                    result = true;
                    break;

                default:
                    break;
            }
            return result;
        }

        void HandleAsyncReply(string szReply)
        {
            if (IsCheckSumOK(szReply))
            {
                llRawSend("+" + CRLF);
                try
                {
                    switch (szReply[0])
                    {
                        case 's':
                            /*
                            //TRACE ("Start @%x: %s\n", GetTickCount (), m_rawReadBuf);
                            if (MyDriver ()->InjectPort ()->WaitingForInjectResponse ())
                            {
                                //TRACE ("S-Inject Response @%x\n", GetTickCount ());
                                m_gotInjectResponse = true;
                                MyDriver ()->InjectPort ()->InjectResponse ();
                                STORE_MARKER (-4);
                            }
                            m_expectingData = true;
                            */
                            break;

                        case 'a':
                            StoreRawData(szReply);
                            // Driver.DDK.AuditMessage(0, AuditLevel.Message, "<RawData>" + szReply);
                            // StoreRawData(chl_A);
                            break;

                        case 'b':
                            StoreRawData(szReply);
                            // Driver.DDK.AuditMessage(0, AuditLevel.Message, "<RawData>" + szReply);
                            // StoreRawData(chl_B);
                            break;

                        case 'e':
                            /*
                            //TRACE ("End @%x: %s\n", GetTickCount (), m_rawReadBuf);
                            m_expectingData = false;
                            if (m_acquiringData)
                                TaskStoreEnd();
                            */
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception caught.", e.ToString());
                }
            }
            else
                llRawSend("-" + CRLF);

            //TRACE ("HandleAsyncReply ==> E = %d, A = %d\n", m_expectingData, m_acquiringData);
        }

        private void StoreRawData(string szReply)
        {
            string szData = szReply.Substring(2, szReply.Length - 6);

            int count = szData.Length / 4;

            int[] data = new int[count];
            int[] data2 = new int[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = (int)ReadN64(szData, 4);
                data2[i] = -data[i];
                szData = szData.Substring(4);
            }

            Driver.Channel(0).UpdateData(data);
            Driver.Channel(1).UpdateData(data2);
            Driver.DDK.AuditMessage(AuditLevel.Message, "Data(" + count.ToString() + ") " + data[0].ToString());
        }

        #endregion
    }
}
