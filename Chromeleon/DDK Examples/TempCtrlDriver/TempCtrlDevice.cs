/////////////////////////////////////////////////////////////////////////////
//
// TempCtrlDevice.cs
// /////////////////
//
// TempCtrl Chromeleon DDK Code Example
//
// Device class for the TempCtrl driver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Threading;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface


namespace MyCompany.TempCtrlDriver
{
    enum TempCtrlState
    {
        Off = 0,
        On = 1
    };

    internal class TempCtrlDevice
    {
        /// Our IDevice
        private IDevice m_Device;

        /// Our properties.
        private IStruct m_TempCtrlStruct;
        private IIntProperty m_TempCtrlProperty;
        private IDoubleProperty m_NominalTempProperty;
        private IDoubleProperty m_CurrentTempProperty;
        private IDoubleProperty m_MinTempProperty;
        private IDoubleProperty m_MaxTempProperty;

        private TempCtrlState m_TempCtrl = TempCtrlState.Off;
        private double m_NominalTemp = 20.0;
        private double m_CurrentTemp = 20.0;
        private double m_MinTemp = 0.0;
        private double m_MaxTemp = 100.0;

        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create a device for temperature control
            m_Device = cmDDK.CreateDevice(name, "Example for a temperature control device");

            // Create a type for a temperature logging command
            ITypeInt tyTempLogParam = cmDDK.CreateInt(0, 2);
            tyTempLogParam.AddNamedValue("Start", 0);
            tyTempLogParam.AddNamedValue("Stop", 1);
            tyTempLogParam.AddNamedValue("Pause", 2);

            // Create a temperature logging command
            ICommand logTemperature = m_Device.CreateCommand("LogTemperature", "Switches temperature logging on / off");
            // Add a parameter for the tewmperature logging command
            logTemperature.AddParameter("LogParameter", "Starts, stops or pauses temperature logging", tyTempLogParam);

            // Create a boolean property to signal if the device is ready for an inject operation.
            ITypeInt tyReady = cmDDK.CreateInt(0, 1);
            tyReady.AddNamedValue("False", 0);
            tyReady.AddNamedValue("True", 1);
            IProperty readyProp = m_Device.CreateStandardProperty(StandardPropertyID.Ready, tyReady);

            // Create a data type that represent the activation state of the temperature control.
            ITypeInt tOnOff = cmDDK.CreateInt((int)TempCtrlState.Off, (int)TempCtrlState.On);
            tOnOff.AddNamedValue("Off", (int)TempCtrlState.Off);
            tOnOff.AddNamedValue("On", (int)TempCtrlState.On);

            // Create a property to activate / deactivate temperature control
            // This property must be writable
            m_TempCtrlProperty = m_Device.CreateProperty("TemperatureControl", "Activates /deactivates temperature control", tOnOff);
            m_TempCtrlProperty.Writeable = true;

            // Create a struct that holds standard properties for temperature and temperature limits
            m_TempCtrlStruct = m_Device.CreateStruct("Temperature", "Temperature of Column Oven.");

            // Create a data type that applies to all temperature properties
            ITypeDouble tTemperature = cmDDK.CreateDouble(0, 100, 1);
            tTemperature.Unit = "°C";

            // Create a property that holds the current temperature
            // This property is read-only
            m_CurrentTempProperty = m_TempCtrlStruct.CreateStandardProperty(StandardPropertyID.Value, tTemperature);

            // Create a property that holds the nominal temperature
            // This property must be writeable
            m_NominalTempProperty = m_TempCtrlStruct.CreateStandardProperty(StandardPropertyID.Nominal, tTemperature);
            m_NominalTempProperty.Update(30.0); //set default value
            m_NominalTempProperty.OnSetProperty += new SetPropertyEventHandler(OnSetTemperature);

            // Create a property that holds the minimal temperature
            // This property must be writeable
            m_MinTempProperty = m_TempCtrlStruct.CreateStandardProperty(StandardPropertyID.LowerLimit, tTemperature);
            m_MinTempProperty.Update(15.0); //set default value
            m_MinTempProperty.Writeable = true;

            // Create a property that holds the maximal temperature
            // This property must be writeable
            m_MaxTempProperty = m_TempCtrlStruct.CreateStandardProperty(StandardPropertyID.UpperLimit, tTemperature);
            m_MaxTempProperty.Update(110.0);//set default value
            m_MaxTempProperty.Writeable = true;

            // Set the default read and write properties for the struct
            m_TempCtrlStruct.DefaultSetProperty = m_NominalTempProperty;
            m_TempCtrlStruct.DefaultGetProperty = m_CurrentTempProperty;

            // Update property values. They must be readable before the device has been connected.
            // The device may not be connected when UI-Modules read the default values.
            m_TempCtrlProperty.Update((int)m_TempCtrl);
            m_NominalTempProperty.Update(m_NominalTemp);
            m_MinTempProperty.Update(m_MinTemp);
            m_MaxTempProperty.Update(m_MaxTemp);
            m_CurrentTempProperty.Update(m_CurrentTemp);
            return m_Device;

        }

        internal void OnConnect()
        {
        }

        internal void OnDisconnect()
        {
        }

        private void OnSetTemperature(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doublePropertyArgs = args as SetDoublePropertyEventArgs;
            Debug.Assert(doublePropertyArgs.NewValue.HasValue);
            m_NominalTempProperty.Update(doublePropertyArgs.NewValue.Value);

            // A real hardware would need some time to reach the new temperature.
            Thread.Sleep(1000);
            m_CurrentTempProperty.Update(doublePropertyArgs.NewValue.Value);
        }
    }
}
