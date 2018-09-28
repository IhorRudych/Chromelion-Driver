/////////////////////////////////////////////////////////////////////////////
//
// Pump.cs
// ///////
//
// ExampleLCSystem Chromeleon DDK Code Example
//
// Pump device class for the ExampleLCSystem.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Text;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.ExampleLCSystem
{
    internal class Pump
    {
        #region Data Members

        private IDDK m_DDK;
        private IDevice m_Device;

        private IFlowHandler m_FlowHandler;

        private IStruct m_PressureStruct;

        private IDoubleProperty m_PressureValue;
        private IDoubleProperty m_PressureLowerLimit;
        private IDoubleProperty m_PressureUpperLimit;

        #endregion

        internal IDevice Device
        {
            get { return m_Device; }
        }

        internal void Create(IDDK cmDDK, string deviceName)
        {
            m_DDK = cmDDK;
            m_Device = m_DDK.CreateDevice(deviceName, "Pump device");

            IStringProperty typeProperty =
                m_Device.CreateProperty("DeviceType",
                "The DeviceType property tells us which component we are talking to.",
                m_DDK.CreateString(20));
            typeProperty.Update("Pump");


            // A data type for our pump flow
            ITypeDouble tFlow = m_DDK.CreateDouble(0, 10, 1);
            tFlow.Unit = "ml/min";

            // Create our flow handler. The flow handler creates a Flow.Nominal,
            // a Flow.Value and 4 eluent component properties for us. 
            m_FlowHandler = m_Device.CreateFlowHandler(tFlow, 4, 2);

            m_FlowHandler.FlowNominalProperty.OnSetProperty += OnSetFlow;

            // initialize the flow
            m_FlowHandler.FlowNominalProperty.Update(0);

            // initialize the components
            m_FlowHandler.ComponentProperties[0].Update(100.0);
            m_FlowHandler.ComponentProperties[1].Update(0);
            m_FlowHandler.ComponentProperties[2].Update(0);
            m_FlowHandler.ComponentProperties[3].Update(0);


            // A type for our pump pressure
            ITypeDouble tPressure = m_DDK.CreateDouble(0, 400, 1);
            tPressure.Unit = "bar";

            // We create a struct for the pressure with Value, LowerLimit and UpperLimit
            m_PressureStruct = m_Device.CreateStruct("Pressure", "The pump pressure.");

            m_PressureValue = m_PressureStruct.CreateStandardProperty(StandardPropertyID.Value, tPressure);

            m_PressureLowerLimit = m_PressureStruct.CreateStandardProperty(StandardPropertyID.LowerLimit, tPressure);
            m_PressureLowerLimit.OnSetProperty += new SetPropertyEventHandler(OnSetPressureLowerLimit);
            m_PressureLowerLimit.Update(0.0);

            m_PressureUpperLimit = m_PressureStruct.CreateStandardProperty(StandardPropertyID.UpperLimit, tPressure);
            m_PressureUpperLimit.OnSetProperty += new SetPropertyEventHandler(OnSetPressureUpperLimit);
            m_PressureUpperLimit.Update(400.0);

            m_PressureStruct.DefaultGetProperty = m_PressureValue;

            m_Device.OnTransferPreflightToRun += new PreflightEventHandler(OnTransferPreflightToRun);
        }

        internal void OnConnect()
        {
            // Update these values from the hardware
            m_FlowHandler.FlowValueProperty.Update(0.0);
            m_PressureValue.Update(0.0);
        }

        internal void OnDisconnect()
        {
            // After disconnecting the hardware we simply don't know what it does
            m_FlowHandler.FlowValueProperty.Update(null);
            m_PressureValue.Update(null);
        }

        private void OnSetFlow(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;

            double newFlow = doubleArgs.NewValue.Value;

            m_Device.AuditMessage(AuditLevel.Normal, "Flow is set to " + newFlow.ToString() + " ml/min");

            m_FlowHandler.FlowNominalProperty.Update(newFlow);

            // Usually the value would be update from the hardware during some status poll.
            m_FlowHandler.FlowValueProperty.Update(newFlow);

            // The pressure would also change somehow. 
            // We emulate this by deriving a pressure form the flow.
            m_PressureValue.Update(newFlow * 40);
        }

        private void OnSetPressureLowerLimit(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            m_Device.AuditMessage(AuditLevel.Normal, "Pressure.LowerLimit is set to " + doubleArgs.NewValue.Value.ToString() + " bar");
            m_PressureLowerLimit.Update(doubleArgs.NewValue.Value);
        }

        private void OnSetPressureUpperLimit(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            m_Device.AuditMessage(AuditLevel.Normal, "Pressure.UpperLimit is set to " + doubleArgs.NewValue.Value.ToString() + " bar");
            m_PressureUpperLimit.Update(doubleArgs.NewValue.Value);
        }

        private void OnTransferPreflightToRun(PreflightEventArgs args)
        {
            // We use the IProgramStep interface to walk the list of events in the instrument method.
            // In a real driver we would need to build some kind of time table and send it to the hardware.
            // In this example we create a list instead and write it to the audit trail.
            // Note that the property is not updated, as this would be done asynchronously during the run.

            StringBuilder sb = new StringBuilder("Table of timed events:\n");

            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                IRampStep rampStep = step as IRampStep;

                if (rampStep != null)
                {
                    RetentionTime duration = rampStep.Duration;
                    IDoublePropertyValue startValue = rampStep.StartValue as IDoublePropertyValue;
                    IDoublePropertyValue endValue = rampStep.EndValue as IDoublePropertyValue;

                    sb.Append("Retention ");
                    sb.Append(step.Retention.Minutes.ToString("F3"));
                    sb.Append(": ");
                    sb.Append(startValue.Property.Owner.Name);
                    sb.Append(".");
                    sb.Append(startValue.Property.Name);
                    sb.Append("=");
                    sb.Append(startValue.Value.Value.ToString("F3"));
                    sb.Append(", ");
                    sb.Append(endValue.Property.Owner.Name);
                    sb.Append(".");
                    sb.Append(endValue.Property.Name);
                    sb.Append("=");
                    sb.Append(endValue.Value.Value.ToString("F3"));
                    sb.Append(", Duration=");
                    sb.Append(duration.Minutes.ToString("F3"));
                    sb.Append("\n");
                }
            }

            m_Device.AuditMessage(AuditLevel.Message, sb.ToString());
        }
    }
}
