// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    public interface IDriverEx
    {
        object LockObject { get; }

        string FirmwareUsbAddress { get; }
        string FirmwareVersion { get; }
        string SerialNo { get; }

        bool IsSimulated { get; }

        string TaskName { get; }
        string TaskNamePrevious { get; }
        string TaskPreviousErrorText { get; }

        BehaviorStatus BehaviorStatePrev { get; }
        BehaviorStatus BehaviorState { get; }
        bool IsStopping { get; }
        void Stop();

        bool IsCommunicating { get; }
        CommunicationStatus CommunicationState { get; }

        /// <summary>Throws <see cref="ExceptionAbort" /> if the driver is not communicating.</summary>
        void CheckIsCommunicating();

        void Connect();
        void Disconnect();

        void SetBusy(string taskName);
        void SetIdle(string taskName, Exception ex = null);

        void AuditMessage(AuditLevel level, string text, string callerMethodName = null);

        event EventHandler OnConnected;
        event EventHandler OnDisconnected;
    }
}
