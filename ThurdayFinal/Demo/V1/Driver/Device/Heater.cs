// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Threading;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Heater : Device
    {
        public enum TemperatureControl
        {
            Off,
            On,
        };

        private readonly HeaterProperties m_Properties;

        public Heater(IDriverEx driver, IDDK ddk, Config.Heater config, string id)
            : base(driver, ddk, config, typeof(Heater).Name, id)
        {
            Log.TaskBegin(Id);
            try
            {
                m_Properties = new HeaterProperties(m_DDK, config, m_Device);

                // Enable the scenario below:
                // Heater.TemperatureNominal N
                // Wait                      Heater.Ready
                m_Properties.TemperatureNominal.ImmediateNotReady = true;

                m_Properties.TemperatureControl.OnSetProperty += OnPropertyTemperatureControlSet;

                m_Properties.TemperatureNominal.OnPreflightSetProperty += OnPropertyTemperatureNominalSetPreflight;
                m_Properties.TemperatureNominal.OnSetProperty += OnPropertyTemperatureNominalSet;

                m_Device.OnPreflightSync += OnDevicePreflightSync;

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

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

        public bool IsTemperatureControlOn
        {
            get
            {
                TemperatureControl value = (TemperatureControl)m_Properties.TemperatureControl.Value.GetValueOrDefault();
                return value != TemperatureControl.Off;
            }
            set
            {
                Debug.Assert(TemperatureControl.Off == 0);
                if (IsTemperatureControlOn == value)
                {
                    return;
                }
                int valueNumber = Property.GetBoolNumber(value);
                m_Properties.TemperatureControl.Update(valueNumber);
                Log.PropertyChanged(Id, "Temperature.Control", value, CallerMethodName);
                if (!value)
                {
                    AuditMessage(AuditLevel.Warning, "The temperature control is Off");
                }
            }
        }

        private void CheckIsTemperatureControlOn()
        {
            if (!IsTemperatureControlOn)
                throw new InvalidOperationException("The temperature control if Off");
        }

        public double TemperatureMin
        {
            [DebuggerStepThrough]
            get { return m_Properties.TemperatureMin.Value.GetValueOrDefault(); }
        }

        public double TemperatureMax
        {
            [DebuggerStepThrough]
            get { return m_Properties.TemperatureMax.Value.GetValueOrDefault(); }
        }

        public double TemperatureNominal
        {
            [DebuggerStepThrough]
            get { return m_Properties.TemperatureNominal.Value.GetValueOrDefault(); }
            private set
            {
                m_Properties.TemperatureNominal.Update(value);
                Log.PropertyChanged(Id, "Temperature.Nominal", value, CallerMethodName);
            }
        }

        public double Temperature
        {
            [DebuggerStepThrough]
            get { return m_Properties.TemperatureValue.Value.GetValueOrDefault(); }
            private set
            {
                if (m_Properties.TemperatureValue.Value == value)
                {
                    return;
                }
                m_Properties.TemperatureValue.Update(value);
                Log.PropertyChanged(Id, "Temperature.Value", value, CallerMethodName);
            }
        }
        #endregion

        private void OnPropertyTemperatureControlSet(SetPropertyEventArgs args)
        {
            try
            {
                TemperatureControl value = (TemperatureControl)Property.GetInt(args);
                if (value == TemperatureControl.Off)
                {
                    IsTemperatureControlOn = false;
                }
                else
                {
                    IsTemperatureControlOn = true;
                }
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
            }
        }

        private void OnPropertyTemperatureNominalSetPreflight(SetPropertyEventArgs args)
        {
            try
            {
                double value = Property.GetDouble(args);
                CheckTemperature(args, value);
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
            }
        }

        private void OnPropertyTemperatureNominalSet(SetPropertyEventArgs args)
        {
            // Do not throw exceptions from the event handler
            if (!IsSimulated)
                throw new InvalidOperationException("This works only in simulation mode.");  // Aborts the injection and the sequence and is not going to be logged by Log.WriteLine
            try
            {
                if (!IsSimulated)
                    throw new InvalidOperationException("This works only in simulation mode.");  // Whether to abort is up to the exception handler

                double value = Property.GetDouble(args);
                SetTemperature(args, value);
            }
            catch (InvalidOperationException ex)
            {
                AuditMessage(AuditLevel.Abort, ex.Message);  // Aborts the injection
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);  // Does not abort the injection
            }
        }

        private void OnDevicePreflightSync(PreflightEventArgs args)
        {
            try
            {
                Property.Preflight preflight = new Property.Preflight(args.RunContext);

                // These values are taken from the script
                Nullable<double> temperatureMin = preflight.GetCurrentValue(m_Properties.TemperatureMin);
                Nullable<double> temperatureMax = preflight.GetCurrentValue(m_Properties.TemperatureMax);
                if (temperatureMin == null)
                    throw new InvalidOperationException(m_Properties.TemperatureMin.Name + " is not set");
                if (temperatureMax == null)
                    throw new InvalidOperationException(m_Properties.TemperatureMax.Name + " is not set");
                if (temperatureMin > temperatureMax)
                    throw new InvalidOperationException("Invalid temperature limits: " + m_Properties.TemperatureMin.Name + " " + temperatureMin.ToString() + " > " +
                                                                                         m_Properties.TemperatureMax.Name + " " + temperatureMax.ToString());
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, ex.Message);
            }
        }

        private void CheckTemperature(SetPropertyEventArgs args, double value)
        {
            if (args.RunContext.IsManual)              // If this is requested manually by the user
            {
                if (!args.RunContext.IsSemanticCheck)  // This is not just a check - the command will be executed after the preflight is successful
                {
                    m_Driver.CheckIsCommunicating();
                    CheckIsTemperatureControlOn();
                }
            }

            ITypeDouble type = m_Properties.TemperatureNominal.DataType;

            if (args.RunContext.IsManual)
            {
                // This check can be made only in manual mode, because the script can have steps that modify
                // TemperatureMin and/or TemperatureMax and therefor the current Min/Max values are not the ones the script sets
                if (value < TemperatureMin || value > TemperatureMax)
                    throw new InvalidOperationException("Cannot set TemperatureValue = " + value.ToString() + " " + type.Unit + ". Must be in [" + TemperatureMin + ", " + TemperatureMax.ToString() + "]");
            }

            double valueMin = type.Minimum.GetValueOrDefault();
            double valueMax = type.Maximum.GetValueOrDefault();
            if (value < valueMin || value > valueMax)
                throw new InvalidOperationException("Cannot set TemperatureValue = " + value.ToString() + " " + type.Unit + ". Must be in [" + valueMin + ", " + valueMax.ToString() + "]");
        }

        private void SetTemperature(SetPropertyEventArgs args, double value)
        {
            m_Driver.CheckIsCommunicating();

            if (args.RunContext.IsManual)
            {
                // Only in manual. The script 1st sets the Temperature and turns the Heater On or
                // Turns the Heater Off and then sets the Temperature
                CheckIsTemperatureControlOn();
            }
            CheckTemperature(args, value);

            Ready = false;
            TemperatureNominal = value;  // Set the user requested temperature

            // Perform Asynchronously
            Action task = (() =>
            {
                const int stepsCount = 5;
                double valueStep = (value - Temperature) / stepsCount;
                for (int i = 0; i < stepsCount - 1; i++)
                {
                    Thread.Sleep(1000);
                    Temperature += valueStep;
                    if (TemperatureNominal != value)
                    {
                        // Start over
                        value = TemperatureNominal;
                        valueStep = (value - Temperature) / stepsCount;
                        i = -1;
                        continue;
                    }
                }
                Thread.Sleep(1000);
                Temperature = value;
                Ready = true;
            });

            AsyncCallback taskCallback = (ar =>
            {
                try
                {
                    task.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    AuditMessage(AuditLevel.Error, "Failed to set Temperature = " + value.ToString() + ". Error: " + ex.Message);
                }
            });

            task.BeginInvoke(taskCallback, null);
        }
    }
}
