// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    public enum CommunicationStatus
    {
        NotCommunicating,
        Connecting,
        Communicating,
        Disconnecting,
    }

    public enum BehaviorStatus
    {
        Idle,
        Busy,
        Stopping,
        Aborting,
    }

    public sealed partial class Driver
    {
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        public void Connect()
        {
            const string taskName = "Connect";
            Log.TaskBegin(Id, "Called by " + CallerMethodName);
            try
            {
                lock (m_LockObject)
                {
                    if (CommunicationState == CommunicationStatus.Disconnecting)
                    {
                        Device.DebuggerBreak("This should not have happened. All commands are executed sequentially.");
                        throw new InvalidOperationException("Connect operation cannot be performed while disconnecting");
                    }

                    if (CommunicationState == CommunicationStatus.Connecting)
                    {
                        //throw new ExceptionTaskAlreadyRunning("Request to connect is skipped - already attempting to connect");
                        Log.TaskEnd(Id, "Request to connect is skipped - already connecting");
                        return;
                    }

                    if (CommunicationState == CommunicationStatus.Communicating)
                    {
                        Log.TaskEnd(Id, "Request to connect is skipped - already connected");
                        return;
                    }

                    SetBusy(taskName);
                    CommunicationState = CommunicationStatus.Connecting;
                }
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, "Cannot execute \"Connect\": " + ex.Message);
                Log.TaskEnd(Id, ex);
                throw;
            }

            try
            {
                // Connect
                System.Threading.Thread.Sleep(5000);

                if (IsSimulated)
                {
                    SetSimulatedFirmwareData();
                }
                else
                {
                    FirmwareUsbAddress = "USB-1234567890AB";
                    FirmwareVersion = "1.0.0";
                    SerialNo = "1000000";
                }

                // The connect operation will fails if any event subscriber throws an exception
                Util.RaiseEvent(Id, "Connected", OnConnected, this, EventArgs.Empty, true);

                lock (m_LockObject)
                {
                    CommunicationState = CommunicationStatus.Communicating;
                    SetIdle(taskName);
                }
                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                lock (m_LockObject)
                {
                    CommunicationState = CommunicationStatus.NotCommunicating;
                    SetIdle(taskName, ex);
                }
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        private void SetSimulatedFirmwareData()
        {
            FirmwareUsbAddress = "USB-111111111111";
            FirmwareVersion = "1.2.3";
            SerialNo = "1234567";
        }

        public void Disconnect()
        {
            lock (m_LockObject)
            {
                if (CommunicationState == CommunicationStatus.NotCommunicating ||
                    CommunicationState == CommunicationStatus.Disconnecting)
                {
                    return;
                }
                Log.TaskBegin(Id, "Called by " + CallerMethodName + ", CommunicationState = " + CommunicationState.ToString());
                CommunicationState = CommunicationStatus.Disconnecting;
            }

            try
            {
                try
                {
                    try
                    {
                        // Disconnect
                        System.Threading.Thread.Sleep(3000);
                    }
                    catch (Exception ex)
                    {
                        AuditMessage(AuditLevel.Error, "Disconnect error: " + ex.Message);
                    }

                    try
                    {
                        Util.RaiseEvent(Id, "Disconnected", OnDisconnected, this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        AuditMessage(AuditLevel.Error, "Disconnect event raise error: " + ex.Message);
                    }
                }
                finally
                {
                    Log.WriteLine(Id, "DDK.Disconnect is called");
                    m_DDK.Disconnect();  // Sets the standard driver property Connected = False and calls AuditMessage(AuditLevel.Abort)
                }
                Log.TaskEnd(Id);
            }
            finally
            {
                lock (m_LockObject)
                {
                    CommunicationState = CommunicationStatus.NotCommunicating;
                }
                Log.TaskEnd(Id);
            }
        }
    }
}
