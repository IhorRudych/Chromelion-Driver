/////////////////////////////////////////////////////////////////////////////
//
// Master.cs
// /////////
//
// NelsonNCI900 Chromeleon DDK Code Example
//
// Master device class for the NCI 900 driver.
// Illustration code, not a complete implementation.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.NelsonNCI900
{
    /// <summary>
    /// Master class implementation
    /// This device is the master device for the Nelson box.
    /// All other devices are subdevices of it.
    /// </summary>
    internal class Master
    {
        #region Data Members

        private IDevice m_MyCmDevice;
        private Driver m_Driver;

        // my properties
        private IStringProperty m_ModelNoProperty;
        private IStringProperty m_DebugCommand;

        #endregion

        #region Construction

        public Master(Driver driver)
        {
            m_Driver = driver;
        }

        #endregion

        #region IDevice

        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our IDevice object
            m_MyCmDevice = cmDDK.CreateDevice(name, "Nelson NCI 900 Master Device");

            // Create our properties

            // ModelNo
            m_ModelNoProperty =
                m_MyCmDevice.CreateStandardProperty(StandardPropertyID.ModelNo, cmDDK.CreateString(20));

            // DebugCommand
            ITypeString tString = cmDDK.CreateString(255);
            m_DebugCommand = m_MyCmDevice.CreateProperty("DebugCommand",
                "For internal use only.", tString);
            m_DebugCommand.AuditLevel = AuditLevel.Message;
            m_DebugCommand.OnSetProperty += new SetPropertyEventHandler(OnDebugCommand);

            ITypeDouble tDouble = cmDDK.CreateDouble(0, 100, 1);
            tDouble.Unit = "%";

            IProperty dblProp =
                m_MyCmDevice.CreateProperty("DoubleProp", "DoubleHelp", tDouble);

            return m_MyCmDevice;
        }

        internal void OnConnect()
        {
            m_ModelNoProperty.Update("Nelson NCI 900");
        }

        internal void OnDisconnect()
        {
        }

        /// OnDebugCommand will be called when the DebugCommand property is set by CM.
        private void OnDebugCommand(SetPropertyEventArgs args)
        {
            Debug.Assert(args.Property == m_DebugCommand);
            SetStringPropertyEventArgs stringPropertyArgs = args as SetStringPropertyEventArgs;
            Driver.Comm.SendLine(stringPropertyArgs.NewValue);
            m_DebugCommand.Update(stringPropertyArgs.NewValue);
        }

        #endregion

        #region Properties

        public Driver Driver
        {
            get { return m_Driver; }
        }

        #endregion
    }
}
