// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    [DriverID(Config.Driver.Id)]
    public sealed partial class Driver : IDriver, IDriverEx, IDriverSendReceive
    {
        #region Fields
        private bool m_IsSimulated;
        private CommunicationStatus m_CommunicationState;

        private BehaviorStatus m_BehaviorStatePrev;
        private BehaviorStatus m_BehaviorState;

        private string m_TaskName;
        private string m_TaskNamePrevious;
        private string m_TaskPreviousErrorText;

        private readonly object m_LockObject = new object();

        private string m_FirmwareUsbAddress;
        private string m_FirmwareVersion;
        private string m_SerialNo;

        private Config.Driver m_Config;

        private IDDK m_DDK;
        #endregion

        #region Constructor
        public Driver()
        {
#if DEBUG
            Debugger.Launch();
#endif
            Log.Init(Id);
        }
        #endregion

        #region Properties
        public static string Id
        {
            [DebuggerStepThrough]
            get { return Config.Driver.Id; }
        }

        public string FirmwareUsbAddress
        {
            [DebuggerStepThrough]
            get { return m_FirmwareUsbAddress; }
            private set
            {
                // Warning: Do not set Null to string properties
                if (value == null)
                {
                    value = Property.EmptyString;
                }

                m_FirmwareUsbAddress = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.FirmwareUsbAddress.Update(value);
                }
                Log.PropertyChanged(Id, "FirmwareUsbAddress", value, CallerMethodName);
            }
        }

        public string FirmwareVersion
        {
            [DebuggerStepThrough]
            get { return m_FirmwareVersion; }
            private set
            {
                // Warning: Do not set Null to string properties
                if (value == null)
                {
                    value = Property.EmptyString;
                }

                m_FirmwareVersion = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.FirmwareVersion.Update(value);
                }
                Log.PropertyChanged(Id, "FirmwareVersion", value, CallerMethodName);
            }
        }

        public string SerialNo
        {
            [DebuggerStepThrough]
            get { return m_SerialNo; }
            private set
            {
                // Warning: Do not set Null to string properties
                if (value == null)
                {
                    value = Property.EmptyString;
                }

                m_SerialNo = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.SerialNo.Update(value);
                }
                Log.PropertyChanged(Id, "SerialNo", value, CallerMethodName);
            }
        }

        public string TaskName
        {
            [DebuggerStepThrough]
            get { return m_TaskName; }
            private set
            {
                // Warning: Do not set Null to string properties
                if (value == null)
                {
                    value = Property.EmptyString;
                }

                m_TaskName = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.TaskName.Update(value);
                }
                Log.PropertyChanged(Id, "TaskName", value, CallerMethodName);
            }
        }

        public string TaskNamePrevious
        {
            [DebuggerStepThrough]
            get { return m_TaskNamePrevious; }
            private set
            {
                m_TaskNamePrevious = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.TaskNamePrevious.Update(value);
                }
                Log.PropertyChanged(Id, "TaskNamePrevious", value, CallerMethodName);
            }
        }

        public string TaskPreviousErrorText
        {
            [DebuggerStepThrough]
            get { return m_TaskPreviousErrorText; }
            private set
            {
                m_TaskPreviousErrorText = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.TaskPreviousErrorText.Update(value);
                }
                Log.PropertyChanged(Id, "TaskNamePreviousErrorText", value, CallerMethodName);
            }
        }

        [DebuggerStepThrough]
        private static string GetMethodName(MethodBase method)
        {
            if (method == null)
                return string.Empty;

            Type declaringType = method.DeclaringType;
            if (declaringType == null)
                return method.Name;

            return method.DeclaringType.Name + "." + method.Name;
        }

        private static string MethodName
        {
            [DebuggerStepThrough]
            get { return GetMethodName((new StackFrame(1, false)).GetMethod()); }
        }

        private static string CallerMethodName
        {
            [DebuggerStepThrough]
            get { return GetMethodName((new StackFrame(2, false)).GetMethod()); }
        }
        #endregion

        #region IDriverEx
        public object LockObject
        {
            [DebuggerStepThrough]
            get { return m_LockObject; }
        }

        public bool IsSimulated
        {
            [DebuggerStepThrough]
            get { return m_IsSimulated; }
        }

        public BehaviorStatus BehaviorStatePrev
        {
            [DebuggerStepThrough]
            get { return m_BehaviorStatePrev; }
        }

        /// <summary>Gets the behavior state <see cref="BehaviorStatus"/>.</summary>
        public BehaviorStatus BehaviorState
        {
            [DebuggerStepThrough]
            get { return m_BehaviorState; }
            private set
            {
                if (m_BehaviorState == value)
                {
                    return;
                }

                BehaviorStatus valuePrev = m_BehaviorState;
                m_BehaviorStatePrev = valuePrev;
                m_BehaviorState = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.BehaviorStatePrev.Update((int)valuePrev);
                    m_Demo.Properties.BehaviorState.Update((int)value);
                }
                Log.PropertyChanged(Id, "BehaviorState", value.ToString() + " <- " + valuePrev.ToString(), CallerMethodName);
            }
        }

        public bool IsStopping
        {
            get { return BehaviorState != BehaviorStatus.Busy; }
        }

        public void Stop()
        {
            lock (LockObject)
            {
                if (BehaviorState == BehaviorStatus.Idle || BehaviorState == BehaviorStatus.Stopping || BehaviorState == BehaviorStatus.Aborting)
                {
                    return;
                }
                Log.WriteLine(Id, "Stopping", CallerMethodName);
                BehaviorState = BehaviorStatus.Stopping;
            }
        }

        /// <summary>Gets the communication state <see cref="CommunicationStatus"/>.</summary>
        public CommunicationStatus CommunicationState
        {
            [DebuggerStepThrough]
            get { return m_CommunicationState; }
            private set
            {
                if (m_CommunicationState == value)
                {
                    return;
                }

                m_CommunicationState = value;
                if (m_Demo != null)
                {
                    m_Demo.Properties.CommunicationState.Update((int)value);
                }
                Log.PropertyChanged(Id, "CommunicationState", value.ToString(), CallerMethodName);
            }
        }

        public bool IsCommunicating
        {
            [DebuggerStepThrough]
            get { return CommunicationState == CommunicationStatus.Communicating; }
        }

        public void CheckIsCommunicating()
        {
            if (!IsCommunicating)
                throw new ExceptionAbort("Not Communicating");
        }

        public void SetBusy(string taskName)
        {
            lock (m_LockObject)
            {
                if (BehaviorState != BehaviorStatus.Idle)
                    throw new ExceptionBusy(string.Format(CultureInfo.CurrentCulture, "Cannot {0} while the device is busy", taskName));
                BehaviorState = BehaviorStatus.Busy;
                TaskName = taskName;
            }
        }

        public void SetIdle(string taskName, Exception ex = null)
        {
            lock (m_LockObject)
            {
                TaskNamePrevious = taskName;
                TaskPreviousErrorText = ex != null ? ex.Message : string.Empty;
                TaskName = string.Empty;
                if (ex != null)
                {
                    string errText = (string.IsNullOrEmpty(taskName) ? string.Empty : taskName + " ended with error: ") + ex.Message;
                    AuditMessage(AuditLevel.Error, errText);
                }
                BehaviorState = BehaviorStatus.Idle;
            }
        }

        void IDriverEx.Connect()
        {
            Log.TaskBegin(Id);
            try
            {
                Connect();
                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        void IDriverEx.Disconnect()
        {
            Log.TaskBegin(Id);
            try
            {
                Disconnect();
                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }
        #endregion

        #region Audit Message
        public void AuditMessage(AuditLevel level, string text, string callerMethodName = null)
        {
            if (string.IsNullOrEmpty(callerMethodName))
            {
                callerMethodName = CallerMethodName;
            }

            if (m_Demo != null)
            {
                m_Demo.AuditMessage(level, text, callerMethodName);
            }
            else
            {
                if (m_DDK != null)
                {
                    m_DDK.AuditMessage(level, text);
                }
                Log.WriteLine(Id, level, text, callerMethodName);
            }
        }
        #endregion
    }
}
