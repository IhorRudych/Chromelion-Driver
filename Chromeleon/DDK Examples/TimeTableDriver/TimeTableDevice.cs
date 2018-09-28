/////////////////////////////////////////////////////////////////////////////
//
// TimeTableDevice.cs
// //////////////////
//
// TimeTableDriver Chromeleon DDK Code Example
//
// This driver creates a device with a few ramped properties and documents 
// how to use interfaces for SetProperty, SetRamp and SetTimeTable events.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

// Set this define to show all preflight messages to Chromeleon.
// If not defined, the messages are dumped to debugger output only.
#define PreflightMessagesToCM

using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Configuration;
using System.Text;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.TimeTableDriver
{
    /// <summary>
    /// TimeTableDevice class implementation
    /// </summary>
    internal class TimeTableDevice
    {
        #region Data Members

        /// Our IDDK
        private IDDK m_MyCmDDK;

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// Our Flow Handler containing properties with ramp syntax
        private IFlowHandler m_FlowHandler;

        /// A property simulating an additional valve
        /// for timed property assignments without ramp syntax
        private IIntProperty m_ValveState;

        /// A command to switch the valve for timed commands
        private ICommand m_ValveCommandTo1;

        /// A command to switch the valve for timed commands
        private ICommand m_ValveCommandTo2;

        /// A more complex command to switch the valve 
        /// for timed commands with parameters
        private ICommand m_ValveCommandWithPar;
        private IIntParameter m_ValveStateParameter;
        private IStringParameter m_ValveLogParameter;

        #endregion

        #region Create the device

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            m_MyCmDDK = cmDDK;

            // Create the Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is an example device.");

            // create a few example properties

            // A Data type for a flow ranging from 0.000 - 10.000
            ITypeDouble tFlow = cmDDK.CreateDouble(0, 10, 3);
            tFlow.Unit = "ml/min";

            // Create our flow handler. The flow handler creates a Flow.Nominal,
            // a Flow.Value and 2 eluent component properties for us.
            m_FlowHandler = m_MyCmDevice.CreateFlowHandler(tFlow, 2, 2);

            // initialize the flow
            m_FlowHandler.FlowNominalProperty.Update(0);

            // initialize the components
            m_FlowHandler.ComponentProperties[0].Update(100.0);
            m_FlowHandler.ComponentProperties[1].Update(0);

            // All properties support ramps
            m_FlowHandler.FlowNominalProperty.RampSyntax = true;
            m_FlowHandler.ComponentProperties[0].RampSyntax = true;
            m_FlowHandler.ComponentProperties[1].RampSyntax = true;


            // Attach various handlers
            m_FlowHandler.FlowNominalProperty.OnSetProperty += new SetPropertyEventHandler(m_RampedFlowProperty_OnSetProperty);
            m_FlowHandler.ComponentProperties[1].OnSetProperty += new SetPropertyEventHandler(m_RampedPercentageProperty_OnSetProperty);

            m_FlowHandler.FlowNominalProperty.OnSetRamp += new SetRampEventHandler(m_RampedFlowProperty_OnSetRamp);
            m_FlowHandler.ComponentProperties[1].OnSetRamp += new SetRampEventHandler(m_RampedPercentageProperty_OnSetRamp);

            m_FlowHandler.FlowNominalProperty.OnPreflightSetProperty += new SetPropertyEventHandler(m_RampedFlowProperty_OnPreflightSetProperty);
            m_FlowHandler.ComponentProperties[1].OnPreflightSetProperty += new SetPropertyEventHandler(m_RampedPercentageProperty_OnPreflightSetProperty);

            m_FlowHandler.FlowNominalProperty.OnPreflightSetRamp += new SetRampEventHandler(m_RampedFlowProperty_OnPreflightSetRamp);
            m_FlowHandler.ComponentProperties[1].OnPreflightSetRamp += new SetRampEventHandler(m_RampedPercentageProperty_OnPreflightSetRamp);

            m_MyCmDevice.OnSetTimeTable += new SetTimeTableEventHandler(m_MyCmDevice_OnSetTimeTable);
            m_MyCmDevice.OnPreflightSetTimeTable += new SetTimeTableEventHandler(m_MyCmDevice_OnPreflightSetTimeTable);

            // now create the properties and command for valve simulation
            ITypeInt valvePropType = cmDDK.CreateInt(1, 2);
            valvePropType.LegalValues = new[] {1, 2};
            m_ValveState = m_MyCmDevice.CreateProperty("ValveState", "The state of the simulated valve, can be '1' or '2'.", valvePropType);
            m_ValveState.OnSetProperty += new SetPropertyEventHandler(m_ValveState_OnSetProperty);
            m_ValveState.OnPreflightSetProperty += new SetPropertyEventHandler(m_ValveState_OnPreflightSetProperty);

            m_ValveCommandTo1 = m_MyCmDevice.CreateCommand("SwitchValveTo1", "Switch the simulated valve to '1' state.");
            m_ValveCommandTo1.OnCommand += new CommandEventHandler(m_ValveCommandTo1_OnCommand);
            m_ValveCommandTo1.OnPreflightCommand += new CommandEventHandler(m_ValveCommandTo1_OnPreflightCommand);

            m_ValveCommandTo2 = m_MyCmDevice.CreateCommand("SwitchValveTo2", "Switch the simulated valve to '2' state.");
            m_ValveCommandTo2.OnCommand += new CommandEventHandler(m_ValveCommandTo2_OnCommand);
            m_ValveCommandTo2.OnPreflightCommand += new CommandEventHandler(m_ValveCommandTo2_OnPreflightCommand);

            m_ValveCommandWithPar = m_MyCmDevice.CreateCommand("SwitchValve", "Switch the simulated valve according to parameters.");
            m_ValveStateParameter = m_ValveCommandWithPar.AddParameter("NewState", "The new state of the simulated valve, can be '1' or '2'.", valvePropType);
            m_ValveStateParameter.Required = true;
            m_ValveLogParameter = m_ValveCommandWithPar.AddParameter("Message", "A message to be written when executing the command.", m_MyCmDDK.CreateString(64));
            m_ValveLogParameter.Required = false;
            m_ValveCommandWithPar.OnCommand += new CommandEventHandler(m_ValveCommandWithPar_OnCommand);
            m_ValveCommandWithPar.OnPreflightCommand += new CommandEventHandler(m_ValveCommandWithPar_OnPreflightCommand);

            return m_MyCmDevice;
        }

        #endregion

        #region Connect, Disconnect

        /// <summary>
        /// OnConnect we set up some initial values.
        /// In a real driver, OnConnect would establish the hardware connection first and
        /// retrieve the actual hardware status from the hardware.
        /// </summary>
        internal void OnConnect()
        {
            // Write a message to the audit trail
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "TimeTableDevice.OnConnect()");

            // Send the initial flow to Chromeleon.
            m_FlowHandler.FlowNominalProperty.Update(0.1);

            // Send the initial percentage to Chromeleon.
            m_FlowHandler.ComponentProperties[1].Update(11.11);
        }

        internal void OnDisconnect()
        {
        }

        #endregion

        #region Preflight event handler

        private void m_RampedFlowProperty_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            if ((doubleArgs == null) || !doubleArgs.NewValue.HasValue)
            {
                return;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedFlowProperty_OnPreflightSetProperty(time: {1} newValue: {0})", doubleArgs.NewValue, RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_RampedPercentageProperty_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            if ((doubleArgs == null) || !doubleArgs.NewValue.HasValue)
            {
                return;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedPercentageProperty_OnPreflightSetProperty(time: {1} newValue: {0})", doubleArgs.NewValue, RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_RampedFlowProperty_OnPreflightSetRamp(SetRampEventArgs args)
        {
            SetDoubleRampEventArgs doubleArgs = args as SetDoubleRampEventArgs;
            if (doubleArgs == null)
            {
                return;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedFlowProperty_OnPreflightSetRamp(time: {3} from: {0} to: {1} in: {2})", doubleArgs.StartValue, doubleArgs.EndValue, RetentionToString(doubleArgs.Duration), RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_RampedPercentageProperty_OnPreflightSetRamp(SetRampEventArgs args)
        {
            SetDoubleRampEventArgs doubleArgs = args as SetDoubleRampEventArgs;
            if (doubleArgs == null)
            {
                return;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedPercentageProperty_OnPreflightSetRamp(time: {3} from: {0} to: {1} in: {2})", doubleArgs.StartValue, doubleArgs.EndValue, RetentionToString(doubleArgs.Duration), RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_MyCmDevice_OnPreflightSetTimeTable(SetTimeTableEventArgs args)
        {
            // Write a message to the preflight results
#if (PreflightMessagesToCM)
            DumpTimeTable(args.TimeTable, "m_MyCmDevice_OnPreflightSetTimeTable()", true, AuditLevel.Warning);
#else
            DumpTimeTable(args.TimeTable, "m_MyCmDevice_OnPreflightSetTimeTable()", false, AuditLevel.Message);
#endif
        }

        private void m_ValveState_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            if ((intArgs == null) || !intArgs.NewValue.HasValue)
            {
                return;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveState_OnPreflightSetProperty(time: {1} newValue: {0})", intArgs.NewValue, RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_ValveCommandTo1_OnPreflightCommand(CommandEventArgs args)
        {
            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandTo1_OnPreflightCommand(time: {0})", RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_ValveCommandTo2_OnPreflightCommand(CommandEventArgs args)
        {
            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandTo2_OnPreflightCommand(time: {0})", RetentionToString(args.RetentionTime));
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        private void m_ValveCommandWithPar_OnPreflightCommand(CommandEventArgs args)
        {
            string stateValueFormatted = "<noValue>";
            string logValueFormatted = "<noValue>";
            IIntParameterValue stateValue = args.ParameterValue(m_ValveStateParameter) as IIntParameterValue;
            if ((stateValue != null) && stateValue.Value.HasValue)
            {
                stateValueFormatted = stateValue.Value.Value.ToString(CultureInfo.InvariantCulture);
            }
            IStringParameterValue logValue = args.ParameterValue(m_ValveLogParameter) as IStringParameterValue;
            if ((logValue != null) && !String.IsNullOrEmpty(logValue.Value))
            {
                logValueFormatted = logValue.Value;
            }

            // Write a message to the preflight results
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandWithPar_OnPreflightCommand(time: {0}, state {1}, log {2})", RetentionToString(args.RetentionTime), stateValueFormatted, logValueFormatted);
#if (PreflightMessagesToCM)
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message);
#else
            Debug.WriteLine("PreflightMessage: " + message);
#endif
        }

        #endregion

        #region Runtime event handler

        /// <summary>
        /// Called when the Flow property is set by Chromeleon.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void m_RampedFlowProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            Debug.Assert((doubleArgs != null) && doubleArgs.NewValue.HasValue);

            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedFlowProperty_OnSetProperty(time: {1} newValue: {0})", doubleArgs.NewValue, RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);

            m_FlowHandler.FlowNominalProperty.Update(doubleArgs.NewValue.Value);
        }

        /// <summary>
        /// Called when a Percentage property is set by Chromeleon.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void m_RampedPercentageProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            Debug.Assert((doubleArgs != null) && doubleArgs.NewValue.HasValue);

            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedPercentageProperty_OnSetProperty(time: {1} newValue: {0})", doubleArgs.NewValue, RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);

            m_FlowHandler.ComponentProperties[1].Update(doubleArgs.NewValue.Value);
        }

        /// <summary>
        /// Called when a Flow ramp step is set by Chromeleon.
        /// </summary>
        /// <param name="args">The SetDoubleRampEventArgs contain 
        ///   the start value, the end value and the duration of the ramp.</param>
        private void m_RampedFlowProperty_OnSetRamp(SetRampEventArgs args)
        {
            SetDoubleRampEventArgs doubleArgs = args as SetDoubleRampEventArgs;
            Debug.Assert(doubleArgs != null);

            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedFlowProperty_OnSetRamp(time: {3} from: {0} to: {1} in: {2})", doubleArgs.StartValue, doubleArgs.EndValue, RetentionToString(doubleArgs.Duration), RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);
            m_FlowHandler.FlowNominalProperty.Update(doubleArgs.EndValue);
        }

        /// <summary>
        /// Called when a Percentage ramp step is set by Chromeleon.
        /// </summary>
        /// <param name="args">The SetDoubleRampEventArgs contain 
        ///   the start value, the end value and the duration of the ramp.</param>
        private void m_RampedPercentageProperty_OnSetRamp(SetRampEventArgs args)
        {
            SetDoubleRampEventArgs doubleArgs = args as SetDoubleRampEventArgs;
            Debug.Assert(doubleArgs != null);

            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_RampedPercentageProperty_OnSetRamp(time: {3} from: {0} to: {1} in: {2})", doubleArgs.StartValue, doubleArgs.EndValue, RetentionToString(doubleArgs.Duration), RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);
            m_FlowHandler.ComponentProperties[1].Update(doubleArgs.EndValue);
        }

        /// <summary>
        /// Called when a time table is sent to this device.
        /// </summary>
        /// <param name="args">The SetTimeTableEventArgs contain 
        ///   an ITimeTable reference accessing all the time table data.</param>
        private void m_MyCmDevice_OnSetTimeTable(SetTimeTableEventArgs args)
        {
            // Write a message to the audit trail
            DumpTimeTable(args.TimeTable, "m_MyCmDevice_OnSetTimeTable()", true, AuditLevel.Message);
        }

        /// <summary>
        /// Called when the ValveState property is set by Chromeleon.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void m_ValveState_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            Debug.Assert((intArgs != null) && (intArgs.NewValue.HasValue));

            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveState_OnSetProperty(time: {1} newValue: {0})", intArgs.NewValue, RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);

            // As we simulate the valve only, we update the property direcly.
            m_ValveState.Update(intArgs.NewValue.Value);
        }

        /// <summary>
        /// Called when the SwitchValveTo1 command is triggered by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain the parameter values - none in this case.</param>
        private void m_ValveCommandTo1_OnCommand(CommandEventArgs args)
        {
            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandTo1_OnCommand(time: {0})", RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);

            // As we simulate the valve only, we update the property direcly.
            m_ValveState.Update(1);
        }

        /// <summary>
        /// Called when the SwitchValveTo2 command is triggered by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain the parameter values - none in this case.</param>
        private void m_ValveCommandTo2_OnCommand(CommandEventArgs args)
        {
            // Write a message to the audit trail
            String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandTo1_OnCommand(time: {0})", RetentionToString(args.RetentionTime));
            m_MyCmDevice.AuditMessage(AuditLevel.Message, message);

            // As we simulate the valve only, we update the property direcly.
            m_ValveState.Update(2);
        }

        /// <summary>
        /// Called when the SwitchValve command is triggered by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain the parameter values.</param>
        private void m_ValveCommandWithPar_OnCommand(CommandEventArgs args)
        {
            IIntParameterValue stateValue = args.ParameterValue(m_ValveStateParameter) as IIntParameterValue;
            if ((stateValue == null) || !stateValue.Value.HasValue)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Error, "ValveState is a required parameter, should have a value!");
                return;
            }
            IStringParameterValue logValue = args.ParameterValue(m_ValveLogParameter) as IStringParameterValue;
            if ((logValue != null) && !String.IsNullOrEmpty(logValue.Value))
            {
                // Write a the message from command parameter to the audit trail
                m_MyCmDevice.AuditMessage(AuditLevel.Message, logValue.Value);
            }
            else
            {
                // Write a default message to the audit trail
                String message = String.Format(CultureInfo.InvariantCulture, "m_ValveCommandWithPar_OnCommand(time: {1}, state {0})", stateValue.Value.Value, RetentionToString(args.RetentionTime));
                m_MyCmDevice.AuditMessage(AuditLevel.Message, message);
            }

            // As we simulate the valve only, we update the property direcly.
            m_ValveState.Update(stateValue.Value.Value);
        }

        #endregion

        #region Private helpers

        private void DumpTimeTable(ITimeTable timeTable, string header, bool toAuditTrail, AuditLevel level)
        {
            StringBuilder sb = new StringBuilder(header);
            if (timeTable.StartTime.HundredthSeconds != RetentionTime.InvalidValue)
                sb.AppendFormat(CultureInfo.InvariantCulture, " StartTime: {0}", RetentionToString(timeTable.StartTime));
            if (timeTable.EndTime.HundredthSeconds != RetentionTime.InvalidValue)
                sb.AppendFormat(CultureInfo.InvariantCulture, " EndTime: {0}", RetentionToString(timeTable.EndTime));

            if (toAuditTrail)
                m_MyCmDevice.AuditMessage(level, sb.ToString());
            else
                Debug.WriteLine(sb.ToString());

            DumpEntries(timeTable, toAuditTrail, level, m_FlowHandler.FlowNominalProperty);
            DumpEntries(timeTable, toAuditTrail, level, m_FlowHandler.ComponentProperties[1]);
        }

        private void DumpEntries(ITimeTable timeTable, bool toAuditTrail, AuditLevel level, IProperty property)
        {
            if (!timeTable.Contains(property))
                return;

            String message = String.Format(CultureInfo.InvariantCulture, "Entries for property {0}", property.Name);
            if (toAuditTrail)
                m_MyCmDevice.AuditMessage(level, message);
            else
                Debug.WriteLine(message);

            for (Int32 index = 0; index < timeTable.Size; index++)
            {
                ITimeTableEntry entry = timeTable.Entry(index);
                if (entry.Property == property)
                {
                    message = String.Format(CultureInfo.InvariantCulture, "Time: {0} Value: {1}", RetentionToString(entry.Time), entry.Value);
                    if (toAuditTrail)
                        m_MyCmDevice.AuditMessage(level, message);
                    else
                        Debug.WriteLine(message);
                }
            }
        }

        private string RetentionToString(RetentionTime time)
        {
            if (time.Initial)
            {
                return "{Initial Time}";
            }
            else
            {
                String result = String.Format(CultureInfo.InvariantCulture, "{0:F3} min", time.Minutes);
                return result;
            }
        }

        #endregion
    }
}
