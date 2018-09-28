// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Pump : Device
    {
        #region Fields
        private readonly PumpProperties m_Properties;

        private readonly IFlowHandler m_FlowHandler;
        private bool m_IsRamped;

        private double m_FlowRequested;
        private bool m_IsFlowRequested;

        private double m_EluentPercentA;
        private double m_EluentPercentB;
        private double m_EluentPercentC;
        private double m_EluentPercentD;

        private readonly PumpChannelPressure m_ChannelPressure;

        private ISequencePreflight m_CurrentSequence;
        #endregion

        #region Constructor
        public Pump(IDriverEx driver, IDDK ddk, Config.Pump config, string id)
            : base(driver, ddk, typeof(Pump).Name, id, config.Name)
        {
            Log.TaskBegin(Id);
            try
            {
                m_Properties = new PumpProperties(m_DDK, m_Device);

                m_Properties.PressureLowerLimit.OnSetProperty += OnPropertyPressureLowerLimitSet;
                m_Properties.PressureUpperLimit.OnSetProperty += OnPropertyPressureUpperLimitSet;

                // Properties of the flow handler:
                //     m_FlowHandler.FlowNominalProperty                         - Flow.Nominal
                //     m_FlowHandler.FlowValueProperty                           - Flow.Value
                //     m_FlowHandler.ComponentProperties[i] (4 eluent component) - %A.Equate, %B.Equate, %C.Equate, %D.Equate
                //                                                                 %A.Valuue, %B.Valuue; %C.Valuue; %D.Valuue
                ITypeDouble flowType = m_DDK.CreateDouble(0, 10, 3);
                flowType.Unit = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_MilliLiterPerMin); // ml/min
                m_FlowHandler = m_Device.CreateFlowHandler(flowType, 4, 2);

                m_FlowHandler.FlowNominalProperty.OnPreflightSetProperty += OnPropertyFlowNominalSetPreflight;
                m_FlowHandler.FlowNominalProperty.OnSetProperty += OnPropertyFlowNominalSet;
                m_FlowHandler.FlowNominalProperty.OnSetRamp += OnPropertyFlowNominalSetRamp;
                //m_FlowHandler.FlowNominalProperty.RampSyntax = true; // m_FlowHandler.FlowNominalProperty.IsRampSyntax is already True
                m_FlowHandler.FlowNominalProperty.Update(0);

                //m_FlowHandler.FlowValueProperty.RampSyntax = true; // m_FlowHandler.FlowValueProperty.IsRampSyntax is already True
                m_FlowHandler.FlowValueProperty.Update(0);

                m_EluentPercentA = 100;
                m_FlowHandler.ComponentProperties[0].Update(m_EluentPercentA);  // Partial flow of this component, expressed in percent of the total flow. m_FlowHandler.EquateProperties[i].HelpText = "User-selectable designation of this solvent component."
                m_FlowHandler.ComponentProperties[1].Update(m_EluentPercentB);
                m_FlowHandler.ComponentProperties[2].Update(m_EluentPercentC);
                m_FlowHandler.ComponentProperties[3].Update(m_EluentPercentD);

                m_FlowHandler.EquateProperties[0].Update("Water");

                m_FlowHandler.ComponentProperties[1].OnSetProperty += OnPropertyFlowHandler_ComponentProperty_2_Set;
                m_FlowHandler.ComponentProperties[2].OnSetProperty += OnPropertyFlowHandler_ComponentProperty_3_Set;
                m_FlowHandler.ComponentProperties[3].OnSetProperty += OnPropertyFlowHandler_ComponentProperty_4_Set;

                double pressureSignalMin = m_Properties.PressureLowerLimit.Value.GetValueOrDefault();
                double pressureSignalMax = m_Properties.PressureUpperLimit.Value.GetValueOrDefault();
                int pressureSignalDigits = 3;
                string pressureUnitName = m_Properties.PressureValue.DataType.Unit;
                UnitConversion.PhysUnitEnum pressureUnit = UnitConversionEx.PhysUnitFindName(pressureUnitName);
                m_ChannelPressure = new PumpChannelPressure(driver, ddk, m_Device, "Channel_Pressure_Id", "Channel_Pressure_Name", pressureSignalMin, pressureSignalMax, pressureSignalDigits, pressureUnit);

                m_Device.OnBatchPreflightBegin += OnDeviceBatchPreflightBegin;
                m_Device.OnBatchPreflightSample += OnDeviceBatchPreflightSample;
                m_Device.OnBatchPreflightEnd += OnDeviceBatchPreflightEnd;

                m_Device.OnPreflightBegin += OnDevicePreflightBegin;
                m_Device.OnPreflightLatch += OnDevicePreflightLatch;
                m_Device.OnPreflightSync += OnDevicePreflightSync;
                m_Device.OnPreflightBroadcast += OnDevicePreflightBroadcast;
                m_Device.OnPreflightEnd += OnDevicePreflightEnd;

                m_Device.OnTransferPreflightToRun += OnDeviceTransferPreflightToRun;

                m_Device.OnLatch += OnDeviceLatch;
                m_Device.OnSync += OnDeviceSync;

                // See these in the AutoSampler
                m_Device.OnSequenceStart += OnDeviceSequenceStart;
                m_Device.OnSequenceChange += OnDeviceSequenceChange;
                m_Device.OnSequenceEnd += OnDeviceSequenceEnd;

                m_Device.OnBroadcast += OnDeviceBroadcast;

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
        public bool Ready
        {
            get { return Property.GetBool(m_Properties.Ready.Value.GetValueOrDefault()); }
            private set
            {
                if (Ready == value)
                {
                    return;
                }
                int valueNumber = Property.GetBoolNumber(value);
                m_Properties.Ready.Update(valueNumber);
                Log.PropertyChanged(Id, m_Properties.Ready.Name, value, CallerMethodName);
            }
        }

        private double FlowRequested
        {
            [DebuggerStepThrough]
            get { return m_FlowRequested; }
            set
            {
                m_FlowRequested = value;
                m_IsFlowRequested = true;
                Log.PropertyChanged(Id, "FlowRequested", value, CallerMethodName);
            }
        }

        public double FlowNominal
        {
            get { return m_FlowHandler.FlowNominalProperty.Value.GetValueOrDefault(); }
            private set
            {
                if (FlowNominal == value)
                {
                    return;
                }
                m_FlowHandler.FlowNominalProperty.Update(value);
                Log.PropertyChanged(Id, m_FlowHandler.FlowNominalProperty.DataType.Unit + " Flow.Nominal", value, CallerMethodName);
            }
        }

        public double Flow
        {
            get { return m_FlowHandler.FlowValueProperty.Value.GetValueOrDefault(); }
            private set
            {
                if (Flow == value)
                {
                    return;
                }
                m_FlowHandler.FlowValueProperty.Update(value);
                Log.PropertyChanged(Id, m_FlowHandler.FlowValueProperty.DataType.Unit + " Flow.Value", value, CallerMethodName);

                if (IsSimulated)
                {
                    Pressure = value * 10;
                }
            }
        }

        public double Pressure
        {
            get { return m_Properties.PressureValue.Value.GetValueOrDefault(); }
            private set
            {
                if (Pressure == value)
                {
                    return;
                }
                m_Properties.PressureValue.Update(value);
                Log.PropertyChanged(Id, m_Properties.PressureValue.DataType.Unit + " Pressure.Value", value, CallerMethodName);
            }
        }
        #endregion

        #region Events Properties
        private void OnPropertyPressureLowerLimitSet(SetPropertyEventArgs args)
        {
            double value = Property.GetDouble(args);
            Log.WriteLine(Id, "On Property PressureLowerLimit Set " + value.ToString());
            if (m_Properties.PressureLowerLimit.Value.GetValueOrDefault() == value)
            {
                return;
            }
            m_Properties.PressureLowerLimit.Update(value);
            AuditMessage(AuditLevel.Normal, m_Properties.PressureLowerLimit.Name + " set to " + value.ToString() + " " + m_Properties.PressureLowerLimit.DataType.Unit);
        }

        private void OnPropertyPressureUpperLimitSet(SetPropertyEventArgs args)
        {
            double value = Property.GetDouble(args);
            Log.WriteLine(Id, "On Property PressureUpperLimit Set " + value.ToString());
            if (m_Properties.PressureUpperLimit.Value.GetValueOrDefault() == value)
            {
                return;
            }
            m_Properties.PressureUpperLimit.Update(value);
            AuditMessage(AuditLevel.Normal, m_Properties.PressureUpperLimit.Name + " set to " + value.ToString() + " " + m_Properties.PressureUpperLimit.DataType.Unit);
        }

        private void OnPropertyFlowNominalSetPreflight(SetPropertyEventArgs args)
        {
            double value = Property.GetDouble(args);
            Log.WriteLine(Id, "Flow.Nominal " + value.ToString());
        }

        private void OnPropertyFlowNominalSet(SetPropertyEventArgs args)
        {
            double value = Property.GetDouble(args);
            FlowNominal = value;
            if (IsSimulated)
            {
                Flow = value;
            }
        }

        private void OnPropertyFlowNominalSetRamp(SetRampEventArgs args)
        {
            SetDoubleRampEventArgs ramp = Property.GetDoubleRampEventArgs(args);
            string text = Property.GetDoubleRampEventArgsText(ramp);
            Log.WriteLine(Id, text);

            if (ramp.StartValue == null)
            {
                return;
            }
            FlowRequested = ramp.StartValue.GetValueOrDefault();
        }

        private void OnPropertyFlowHandler_ComponentProperty_2_Set(SetPropertyEventArgs args)
        {
            m_EluentPercentB = Property.GetDouble(args);
            UpdateEluents();
        }

        private void OnPropertyFlowHandler_ComponentProperty_3_Set(SetPropertyEventArgs args)
        {
            m_EluentPercentC = Property.GetDouble(args);
            UpdateEluents();
        }

        private void OnPropertyFlowHandler_ComponentProperty_4_Set(SetPropertyEventArgs args)
        {
            m_EluentPercentD = Property.GetDouble(args);
            UpdateEluents();
        }

        private void UpdateEluents()
        {
            m_EluentPercentA = 100 - m_EluentPercentB - m_EluentPercentC - m_EluentPercentD;
            if (m_EluentPercentA < 0)
            {
                m_EluentPercentA = 0;
                m_EluentPercentB = 100 - m_EluentPercentC - m_EluentPercentD;
                if (m_EluentPercentB < 0)
                {
                    m_EluentPercentB = 0;
                    m_EluentPercentC = 100 - m_EluentPercentD;
                    if (m_EluentPercentC < 0)
                    {
                        m_EluentPercentC = 0;
                        if (m_EluentPercentD > 100)
                        {
                            m_EluentPercentD = 100;
                        }
                    }
                }
            }

            m_FlowHandler.ComponentProperties[0].Update(m_EluentPercentA);
            m_FlowHandler.ComponentProperties[1].Update(m_EluentPercentB);
            m_FlowHandler.ComponentProperties[2].Update(m_EluentPercentC);
            m_FlowHandler.ComponentProperties[3].Update(m_EluentPercentD);
        }
        #endregion

        #region Events Device
        private void OnDeviceBatchPreflightBegin(BatchPreflightEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnDeviceBatchPreflightSample(SamplePreflightEventArgs args)
        {
            ISamplePreflight injection = args.SamplePreflight;
            Log.WriteLine(Id, "Injection \"" + injection.Name + "\"");
        }

        private void OnDeviceBatchPreflightEnd(BatchPreflightEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnDevicePreflightBegin(PreflightEventArgs args)
        {
            Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());
        }

        private void OnDevicePreflightLatch(PreflightEventArgs args)
        {
            Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());
        }

        private void OnDevicePreflightSync(PreflightEventArgs args)
        {
            try
            {
                Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());

                Property.Preflight preflight = new Property.Preflight(args.RunContext);

                Nullable<double> pressureLimitMin = preflight.GetCurrentValue(m_Properties.PressureLowerLimit);
                Nullable<double> pressureLimitMax = preflight.GetCurrentValue(m_Properties.PressureUpperLimit);
                if (pressureLimitMin == null)
                    throw new InvalidOperationException(m_Properties.PressureLowerLimit.Name + " is not set");
                if (pressureLimitMax == null)
                    throw new InvalidOperationException(m_Properties.PressureUpperLimit.Name + " is not set");
                if (pressureLimitMin > pressureLimitMax)
                    throw new InvalidOperationException("Invalid pressure limits: " + m_Properties.PressureLowerLimit.Name + " " + pressureLimitMin.ToString() + " > " +
                                                                                      m_Properties.PressureUpperLimit.Name + " " + pressureLimitMax.ToString());
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
            }
        }

        private void OnDevicePreflightBroadcast(BroadcastEventArgs args)
        {
            if (args.RunContext == null)
            {
                Log.WriteLine(Id, "args.Broadcast = " + args.Broadcast.ToString());
            }
            else
            {
                Log.WriteLine(Id, "args.Broadcast = " + args.Broadcast.ToString() + ", ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());
            }
        }

        private void OnDevicePreflightEnd(PreflightEventArgs args)
        {
            Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());

            // Check everything
            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                IRampStep rampStep = step as IRampStep;
                if (rampStep == null)
                {
                    continue;
                }

                m_IsRamped = true;
                RetentionTime duration = rampStep.Duration;
                IDoublePropertyValue startValue = rampStep.StartValue as IDoublePropertyValue;
                IDoublePropertyValue endValue = rampStep.EndValue as IDoublePropertyValue;
            }
        }

        private void OnDeviceTransferPreflightToRun(PreflightEventArgs args)
        {
            Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());

            m_IsRamped = false;

            // Iterate  through each IProgramStep from the instrument method
            bool isIsocratic = false;
            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                IPropertyAssignmentStep propertyAssignmentStep = step as IPropertyAssignmentStep;
                if (propertyAssignmentStep == null)
                {
                    continue;
                }

                if (propertyAssignmentStep.Property == m_FlowHandler.FlowNominalProperty)
                {
                    isIsocratic = true;
                    FlowRequested = (propertyAssignmentStep.Value as IDoublePropertyValue).Value.GetValueOrDefault();
                }
                else if (propertyAssignmentStep.Property == m_FlowHandler.ComponentProperties[1])
                {
                    isIsocratic = true;
                    m_EluentPercentB = (propertyAssignmentStep.Value as IDoublePropertyValue).Value.GetValueOrDefault();
                }
                else if (propertyAssignmentStep.Property == m_FlowHandler.ComponentProperties[2])
                {
                    isIsocratic = true;
                    m_EluentPercentC = (propertyAssignmentStep.Value as IDoublePropertyValue).Value.GetValueOrDefault();
                }
                else if (propertyAssignmentStep.Property == m_FlowHandler.ComponentProperties[3])
                {
                    isIsocratic = true;
                    m_EluentPercentD = (propertyAssignmentStep.Value as IDoublePropertyValue).Value.GetValueOrDefault();
                }
            }

            if (isIsocratic)
            {
                Log.WriteLine(Id, "Isocratic - FlowRequested = " + FlowRequested.ToString());
                UpdateEluents();
                FlowNominal = FlowRequested;
                if (IsSimulated)
                {
                    Flow = FlowRequested;
                }
                return;
            }

            // In a real driver we would need to build some kind of time table and send it to the hardware.
            // In this example we create a list instead and write it to the audit trail.
            // Note that the property is not updated, as this would be done asynchronously during the run.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Table of timed events:");
            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                IRampStep rampStep = step as IRampStep;
                if (rampStep == null)
                {
                    continue;
                }

                m_IsRamped = true;
                RetentionTime duration = rampStep.Duration;
                IDoublePropertyValue startValue = rampStep.StartValue as IDoublePropertyValue;
                IDoublePropertyValue endValue = rampStep.EndValue as IDoublePropertyValue;

                const string sprt = "\t";
                sb.Append("Retention " + step.Retention.Minutes.ToString("F3") + sprt);
                sb.Append(startValue.Property.Owner.Name + "." + startValue.Property.Name + " = " + startValue.Value.Value.ToString("F3") + ", ");
                sb.Append(endValue.Property.Owner.Name + "." + endValue.Property.Name + " = " + endValue.Value.Value.ToString("F3") + ", ");
                sb.AppendLine("Duration = " + duration.Minutes.ToString("F3"));
            }

            if (m_IsRamped)
            {
                string text = "Table of timed events:" + Environment.NewLine + sb.ToString();
                Log.WriteLine(Id, "Gradient - " + text);

                // SendTimeTableCommand
                if (args.RunContext.IsManual)
                {
                    // SendStartCommand();
                }
            }
            else
            {
                string text = "Table of timed events: None";
                Log.WriteLine(Id, "Gradient - " + text);
            }
        }

        private void OnDeviceLatch(RuntimeEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnDeviceSync(RuntimeEventArgs args)
        {
            Log.WriteLine(Id);
            if (m_IsFlowRequested)
            {
                Log.WriteLine(Id, "Send SendFlowCommand FlowRequested = " + FlowRequested.ToString());
                if (IsSimulated)
                {
                    Flow = FlowRequested;
                }
                else
                {
                    //SendFlowCommand(FlowRequested, m_EluentPercentA, m_EluentPercentB, m_EluentPercentC, m_EluentPercentD, m_PressureMin, m_PressureMax);
                }
                m_IsFlowRequested = false;
            }
        }

        // See AutoSampler for OnDeviceSequenceXxx
        private void OnDeviceSequenceStart(SequencePreflightEventArgs args)
        {
            Log.WriteLine(Id);
            m_CurrentSequence = args.SequencePreflight;
        }

        private void OnDeviceSequenceChange(SequencePreflightEventArgs oldSequenceArgs, SequencePreflightEventArgs newSequenceArgs)
        {
            Log.WriteLine(Id);
            m_CurrentSequence = newSequenceArgs.SequencePreflight;
        }

        private void OnDeviceSequenceEnd(SequencePreflightEventArgs args)
        {
            Log.WriteLine(Id);
            m_CurrentSequence = null;  // After this event the current sequence becomes invalid.
        }

        private void OnDeviceBroadcast(BroadcastEventArgs args)
        {
            Log.WriteLine(Id, "args.Broadcast = " + args.Broadcast.ToString());  // args.RunContext is always null
            switch (args.Broadcast)
            {
                case Broadcast.Inject:
                    {
                        break;
                    }
                case Broadcast.Hold:      // Pause
                    {
                        // Can be caused by:
                        //   - Wait <boolean condition> (in Instrument Method)
                        //   - Message displayed to the user
                        //   - Sampler.Inject
                        //   - System.Hold

                        // The clock that controls script execution is paused and no more script lines are processed for the time being.

                        // Pump drivers need to inform the firmware that it should temporarily pause gradient execution and
                        // flow /composition should remain unchanged for the time being (if there is a change in progress).
                        break;
                    }
                case Broadcast.Continue:  // Resume
                    {
                        // Can be caused by:
                        //   - Wait resolves when <boolean condition> becomes true
                        //   - User has confirmed message that has been displayed
                        //   - Sampler driver has notified system about the inject response
                        //   - System.Continue

                        // Resume changing flow/composition where it left off (if there is a change in progress).
                        break;
                    }
                case Broadcast.Stopflow:  // Can be caused by: System.Stopflow
                    {
                        // Do whatever is done on Broadcast.Hold and set Flow = 0
                        if (IsSimulated)
                        {
                            Flow = 0;
                        }
                        break;
                    }
                case Broadcast.InjectResponse:
                    {
                        //if (m_IsRamped) SendStartCommand();
                        break;
                    }
                case Broadcast.InjectBlankRun:
                    {
                        //if (m_IsRamped) SendStartCommand();
                        break;
                    }
                case Broadcast.RetentionZeroCrossed:
                    {
                        //if (m_IsRamped) SendStartCommand();
                        break;
                    }
                case Broadcast.AbortRequested:
                    {
                        //SendAbortCommand();
                        break;
                    }
                case Broadcast.AbortError:
                    {
                        //SendAbortCommand();
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion
    }
}
