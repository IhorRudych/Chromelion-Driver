// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Reflection;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    public abstract class Device
    {
        #region Fields
        private readonly bool m_IsSimulated;
        private readonly string m_Id;
        private readonly string m_Name;

        protected readonly IDDK m_DDK;
        protected readonly IDevice m_Device;
        protected readonly IDriverEx m_Driver;

        private readonly DeviceProperties m_Properties;
        #endregion

        #region Constructor
        public Device(IDriverEx driver, IDDK ddk, Config.Device config, string deviceType, string id, IDevice owner = null, IDevice device = null)
            : this(driver, ddk, deviceType, id, config.Name, owner, device)
        {
        }

        public Device(IDriverEx driver, IDDK ddk, string deviceType, string id, string name, IDevice owner = null, IDevice device = null)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (string.IsNullOrEmpty(deviceType))
                throw new ArgumentNullException("deviceType");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (id.Length != id.Trim().Length)
                throw new ArgumentException("id \"" + id + "\" is invalid - it starts or ends with a space");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (name.Length != name.Trim().Length)
                throw new ArgumentException("name \"" + name + "\" is invalid - it starts or ends with a space");

            m_Driver = driver;
            m_DDK = ddk;
            m_Id = id;
            m_Name = name;
            Log.TaskBegin(Id, "Name = \"" + Name + "\"");
            try
            {
                m_IsSimulated = m_Driver.IsSimulated;

                m_Device = device;
                if (m_Device == null)
                {
                    m_Device = m_DDK.CreateDevice(Name, "Help text for device " + Id + " with name \"" + Name + "\"");
                }

                if (owner != null)
                {
                    m_Device.SetOwner(owner);
                }

                m_Properties = new DeviceProperties(m_DDK, m_Device, Id, deviceType, Name);

                m_Properties.Description.OnPreflightSetProperty += OnPropertyDescriptionSetPreflight;
                m_Properties.Description.OnSetProperty += OnPropertyDescriptionSet;

                // The Debug.Assert problem fixed in VS 2017, but the debugger must be already attached - else nothing happens
                Debug.Assert(!string.IsNullOrEmpty(Name));

                bool debuggerBreak = false;
                if (debuggerBreak)
                {
                    DebuggerBreak("Test debugger break");
                }

                bool breakIfDebuggerIsAttached = false;
                if (breakIfDebuggerIsAttached)
                {
                    DebuggerBreakIfIsAttached("Test debugger break is it's attached");
                }

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }
        #endregion

        #region Properties
        public string Id
        {
            [DebuggerStepThrough]
            get { return m_Id; }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return m_Name; }
        }

        public bool IsSimulated
        {
            [DebuggerStepThrough]
            get { return m_IsSimulated; }
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

        protected static string MethodName
        {
            [DebuggerStepThrough]
            get { return GetMethodName((new StackFrame(1, false)).GetMethod()); }
        }

        protected static string CallerMethodName
        {
            [DebuggerStepThrough]
            get { return GetMethodName((new StackFrame(2, false)).GetMethod()); }
        }
        #endregion

        #region Debugger
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DebuggerBreak(string text = null)
        {
            Trace.WriteLine("DEBUG DebuggerBreak" + (string.IsNullOrEmpty(text) ? string.Empty : " - " + text));
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                Debugger.Launch();
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DebuggerBreakIfIsAttached(string text = null)
        {
            Trace.WriteLine("DEBUG DebuggerBreakIfIsAttached" + (string.IsNullOrEmpty(text) ? string.Empty : " - " + text));
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
        #endregion

        #region Get Timeout
        public static int GetTimeout(int value)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                value = int.MaxValue;
            }
#endif
            return value;
        }
        #endregion

        #region Events Properties
        private void OnPropertyDescriptionSetPreflight(SetPropertyEventArgs args)
        {
            try
            {
                string value = Property.GetString(args);

                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("Property " + args.Property.Name + " cannot be empty");

                // or
                if (value == "a")
                {
                    AuditMessage(AuditLevel.Error, "Property " + args.Property.Name + " cannot be empty \"a\"");
                    return;
                }
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
            }
        }

        private void OnPropertyDescriptionSet(SetPropertyEventArgs args)
        {
            try
            {
                string value = Property.GetString(args);
                if (value == null)
                {
                    value = Property.EmptyString;  // Warning: Do not set Null to string properties
                }

                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("Property " + args.Property.Name + " value cannot be empty");

                if (value == m_Properties.Description.Value)
                {
                    return;
                }

                m_Properties.Description.Update(value);

                string userName = Property.GetUserName(args);
                Log.WriteLine(Id, "Property changed: " + args.Property.Name + " = \"" + value + "\"" + (string.IsNullOrEmpty(userName) ? string.Empty : " - requested by " + userName));
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
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
            m_Device.AuditMessage(level, text);
            Log.WriteLine(Id, level, text, callerMethodName);
        }
        #endregion
    }
}
