/////////////////////////////////////////////////////////////////////////////
//
// DownloadDevice.cs
// /////////////////
//
// Download Chromeleon DDK Code Example
//
// Device class for the Download driver.
// The "Download" example driver illustrates how preflighting
// in Chromeleon can be used to implement a download driver.
// Open the "DownloadTest.cmbx" to restore a sequence and instrument method for testing the driver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Text;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.Download
{
    /// <summary>
    /// DownloadDevice class implementation
    /// </summary>
    class DownloadDevice
    {
        #region Data Members

        private IDevice m_MyCmDevice;

        private IIntProperty m_EventAProperty;
        private IIntProperty m_EventBProperty;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is download device.");

            // Create two event properties. These properties may be switched several times during
            // an instrument method. Before the run is started all timed events must be collected into
            // a download method.
            m_EventAProperty =
                m_MyCmDevice.CreateBooleanProperty("EventA", "An event property.", "Off", "On");

            // We need to set Writeable to allow CM to change the value. If we had a handler
            // for the property, this would happen automatically.
            m_EventAProperty.Writeable = true;

            m_EventBProperty =
                m_MyCmDevice.CreateBooleanProperty("EventB", "An event property.", "Off", "On");
            m_EventBProperty.Writeable = true;

            // The complete method will be sent to the hardware in one go in the OnTransferPfToRun handler.
            m_MyCmDevice.OnTransferPreflightToRun += new PreflightEventHandler(OnTransferPfToRun);

            return m_MyCmDevice;
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize our properties.
        /// </summary>
        internal void OnConnect()
        {
            m_EventAProperty.Update(0);
            m_EventBProperty.Update(0);
        }

        /// <summary>
        /// OnTransferPfToRun is called when the previously preflighted instrument method is actually started.
        /// </summary>
        /// <param name="args">The PreflightEventArgs contain the IRunContext with the ProgramSteps.</param>
        private void OnTransferPfToRun(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "OnTransferPreflightToRun handler OnTransferPfToRun, please wait...");

            // We use the IProgramStep interface to walk the list of events in the instrument method.
            // In a real driver we would need to build some kind of time table and send it to the hardware.
            // In this example we create a list instead and write it to the audit trail.
            // Note that the property is not updated, as this would be done asynchronously during the run.

            StringBuilder sb = new StringBuilder("Table of timed events:\n");

            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                IPropertyAssignmentStep propertyAssignment = step as IPropertyAssignmentStep;

                if (propertyAssignment != null)
                {
                    if (propertyAssignment.Value.Property == m_EventAProperty ||
                        propertyAssignment.Value.Property == m_EventBProperty)
                    {
                        IIntPropertyValue value = propertyAssignment.Value as IIntPropertyValue;

                        sb.Append("Retention ");
                        sb.Append(step.Retention.Minutes.ToString("F3"));
                        sb.Append(": ");
                        sb.Append(propertyAssignment.Value.Property.Name);
                        sb.Append("=");
                        sb.Append(value.Value.ToString());
                        sb.Append("\n");
                    }
                }
            }

            m_MyCmDevice.AuditMessage(AuditLevel.Message, sb.ToString());

            m_MyCmDevice.AuditMessage(AuditLevel.Message, "OnTransferPreflightToRun handler OnTransferPfToRun has finished.");
        }
    }
}
