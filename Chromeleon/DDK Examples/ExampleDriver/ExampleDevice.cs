/////////////////////////////////////////////////////////////////////////////
//
// ExampleDevice.cs
// ////////////////
//
// ExampleDriver Chromeleon DDK Code Example
//
// This driver creates devices with a few example properties and commands.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Text;
using System.Timers;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.ExampleDriver
{
    /// <summary>
    /// ExampleDevice class implementation
    /// </summary>
    internal class ExampleDevice
    {
        #region Data Members

        /// The following data members represent our Chromeleon symbols.

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// An example int property.
        private IIntProperty m_ClockProperty;

        /// When this is set to "true" the clock is started.
        private IIntProperty m_EnableClockProperty;

        /// An example double property
        private IDoubleProperty m_PercentageProperty;

        /// An example double property with named special value.
        private IDoubleProperty m_FilterTimeConstantProperty;

        /// An example double property with legal values.
        private IDoubleProperty m_MeasurementRangeProperty;

        /// Our example string property
        private IStringProperty m_AnyTextProperty;

        /// An example command
        private ICommand m_Command;

        /// An example command with a parameter
        private ICommand m_SetEnableClockCommand;

        /// An example command with a parameter
        private ICommand m_SetPercentageCommand;

        /// An example command with 4 parameters
        private ICommand m_CommandWith4Parameters;


        /// The following data members represent our internal status.
        /// In a real hardware driver, this would come from the hardware.

        /// Our current clock time.
        private int m_ClockTime;

        /// Enable/disable the clock.
        private int m_EnableClock;

        /// The percentage value.
        private double m_Percentage;

        /// The string value.
        private string m_MyString;

        /// We use a timer to update the clock property each second.
        private System.Timers.Timer m_clockTimer = new System.Timers.Timer(1000);

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create the Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is an example device.");

            // create a few example properties

            // A Data type for seconds ranging from 0 - 59
            ITypeInt tSeconds = cmDDK.CreateInt(0, 59);
            tSeconds.Unit = "s";

            // Create the "Clock" property
            m_ClockProperty = m_MyCmDevice.CreateProperty("Clock",
                "This is a second counter",
                tSeconds);

            // Attach the OnSetClockProperty handler to the "Clock" property
            m_ClockProperty.OnSetProperty += new SetPropertyEventHandler(OnSetClockProperty);

            // we will use a timer to update the clock property each second
            // Attach the OnTimedEvent handler to the timer.
            m_clockTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // A Data type with two values 0 and 1
            ITypeInt tEnable = cmDDK.CreateInt(0, 1);
            tEnable.AddNamedValue("false", 0);
            tEnable.AddNamedValue("true", 1);

            // Create the "EnableClock" property
            m_EnableClockProperty = m_MyCmDevice.CreateProperty("EnableClock",
                "Enable / disable the clock",
                tEnable);

            // Attach the OnEnableClock handler to the "EnableClock" property
            m_EnableClockProperty.OnSetProperty += new SetPropertyEventHandler(OnEnableClock);

            // A Data type for a percentage ranging from 0.0 - 100.0
            ITypeDouble tPercent = cmDDK.CreateDouble(0, 100, 2);
            tPercent.Unit = "%";

            // Create the "Percentage" property
            m_PercentageProperty = m_MyCmDevice.CreateProperty("Percentage",
                "This is a percentage property",
                tPercent);

            // Attach the OnSetProperty handler to the "Percentage" property
            m_PercentageProperty.OnSetProperty += new SetPropertyEventHandler(OnSetProperty);

            // A Data type for a time constant ranging from 1.0 - 10.0
            ITypeDouble tTimeConstant = cmDDK.CreateDouble(1.0, 10.0, 1);
            tTimeConstant.AddNamedValue("Off", 0.0);
            tTimeConstant.Unit = "s";

            // Create the "Percentage" property
            m_FilterTimeConstantProperty = m_MyCmDevice.CreateProperty("FilterTimeConstant",
                "This is a numeric property with one special named value.",
                tTimeConstant);

            // Attach the OnSetProperty handler to the "FilterTimeConstant" property
            m_FilterTimeConstantProperty.OnSetProperty += new SetPropertyEventHandler(OnSetProperty);

            // A Data type for the measurement range property
            ITypeDouble tMeasurementRange = cmDDK.CreateDouble(0.01, 100.0, 2);
            tMeasurementRange.LegalValues = new double[] { 0.01, 0.1, 1.0, 10.0, 100.0 };
            tMeasurementRange.EnforceLegalValues = true;
            tMeasurementRange.Unit = "V";

            // Create the "Percentage" property
            m_MeasurementRangeProperty = m_MyCmDevice.CreateProperty("MeasurementRange",
                "This is a numeric property with 5 legal (valid) values.",
                tMeasurementRange);

            // Attach the OnSetProperty handler to the "FilterTimeConstant" property
            m_MeasurementRangeProperty.OnSetProperty += new SetPropertyEventHandler(OnSetProperty);

            // A Data type for a string with 20 characters at most
            ITypeString tString = cmDDK.CreateString(20);

            // Create the "AnyText" property
            m_AnyTextProperty = m_MyCmDevice.CreateProperty("AnyText",
                "This is a string property",
                tString);

            // Attach the OnSetProperty handler to the "AnyText" property
            m_AnyTextProperty.OnSetProperty += new SetPropertyEventHandler(OnSetProperty);


            // Create the "ToggleEnableClock" example command
            m_Command = m_MyCmDevice.CreateCommand("ToggleEnableClock", "This is a simple command that toggles the EnableClock property.");

            // Attach the OnToggleEnableClock handler to the "ToggleEnableClock" command
            m_Command.OnCommand += new CommandEventHandler(OnToggleEnableClock);

            // Create the "SetEnableClock" command which uses a parameter.
            m_SetEnableClockCommand = m_MyCmDevice.CreateCommand("SetEnableClock", "This is a command with one required parameter. Set true or false to enable/disable the clock.");

            // Add the "Enable" parameter to the "SetEnableClock" command
            IParameter parameter = m_SetEnableClockCommand.AddParameter("Enable", "Set true or false to enable/disable the clock", tEnable);

            // This is a required parameter.
            parameter.Required = true;

            // Attach the OnSetEnableClock handler to the "SetEnableClock" command
            m_SetEnableClockCommand.OnCommand += new CommandEventHandler(OnSetEnableClock);

            // Create the "SetPercentage" command which uses a parameter.
            m_SetPercentageCommand = m_MyCmDevice.CreateCommand("SetPercentage", "This is a command with one required parameter to set the \"Percentage\" property.");

            // Add the "Value" parameter to the "SetPercentage" command
            parameter = m_SetPercentageCommand.AddParameter("Value", "Set to 0.00 - 100.00", tPercent);

            // This is a required parameter.
            parameter.Required = true;

            // Attach the OnSetPercentage handler to the "SetPercentage" command
            m_SetPercentageCommand.OnCommand += new CommandEventHandler(OnSetPercentage);

            // Create the "CommandWith4Parameters" command which uses 4 optional parameters.
            m_CommandWith4Parameters = m_MyCmDevice.CreateCommand("CommandWith4Parameters", "This is a command with four optional parameters.");

            // Add the parameters to the "CommandWith4Parameters" command
            m_CommandWith4Parameters.AddParameter("Param1", "Set true or false", tEnable);
            m_CommandWith4Parameters.AddParameter("Param2", "Set to 0.00 - 100.00", tPercent);
            m_CommandWith4Parameters.AddParameter("Param3", "Set true or false", tEnable);
            m_CommandWith4Parameters.AddParameter("Param4", "A string with 20 characters", tString);

            // Attach the OnCommandWith4Parameters handler to the "CommandWith4Parameters" command
            m_CommandWith4Parameters.OnCommand += new CommandEventHandler(OnCommandWith4Parameters);

            return m_MyCmDevice;
        }

        /// <summary>
        /// OnConnect we set up some initial values.
        /// In a real driver, OnConnect would establish the hardware connection first and
        /// retrieve the actual hardware status from the hardware.
        /// </summary>
        internal void OnConnect()
        {
            // Write a message to the audit trail
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "ExampleDevice.OnConnect()");

            // set up the initial values
            m_EnableClockProperty.Update(0);

            m_MyString = "Initial value";
            // Send the updated string to Chromeleon.
            m_AnyTextProperty.Update(m_MyString);

            m_Percentage = 11.11;
            // Send the updated percentage to Chromeleon.
            m_PercentageProperty.Update(m_Percentage);

            // Update the other properties
            m_FilterTimeConstantProperty.Update(0.0);
            m_MeasurementRangeProperty.Update(10.0);
        }

        /// <summary>
        /// OnDisconnect we stop our second counter if it is running.
        /// In a real driver we would reenable the instrument panel, disconnect from the hardware and
        /// release the communication resources.
        /// </summary>
        internal void OnDisconnect()
        {
            // stop the timer
            m_clockTimer.Enabled = false;
        }

        /// <summary>
        /// Called by the clock timer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs args)
        {
            m_ClockTime++;
            Trace.WriteLine("OnTimedEvent: " + m_ClockTime);

            if (m_ClockTime == 60)
                m_ClockTime = 0;

            // Send the updated clock to Chromeleon.
            m_ClockProperty.Update(m_ClockTime);
        }

        /// <summary>
        /// Called when the Clock property is set by Chromeleon, e.g. by a panel.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void OnSetClockProperty(SetPropertyEventArgs args)
        {
            Debug.Assert(args.Property == m_ClockProperty);

            // We know that our clock is an IIntProperty
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            Debug.Assert(intArgs != null);

            // read the new value. In a real driver, we would send the new value
            // to the hardware here.
            Debug.Assert(intArgs.NewValue.HasValue);
            m_ClockTime = intArgs.NewValue.Value;

            // Send the updated value to Chromeleon.
            // Note that Chromeleon does not update the value automatically, as
            // a hardware or its driver may reject the value.
            m_ClockProperty.Update(m_ClockTime);
        }

        /// <summary>
        /// Called when the EnableClock property is set by Chromeleon, e.g. by a panel.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void OnEnableClock(SetPropertyEventArgs args)
        {
            Debug.Assert(args.Property == m_EnableClockProperty);

            // We know that our EnableClock is an IIntProperty
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            Debug.Assert(intArgs != null);

            // read the new value. In a real driver, we would send the new value
            // to the hardware here.
            Debug.Assert(intArgs.NewValue.HasValue);
            m_EnableClock = intArgs.NewValue.Value;

            // Enable/disable our timer.
            m_clockTimer.Enabled = (m_EnableClock == 1);

            // Send the updated value to Chromeleon.
            // Note that Chromeleon does not update the value automatically, as
            // a hardware or its driver may reject the value.
            m_EnableClockProperty.Update(m_EnableClock);
        }

        /// <summary>
        /// Called when the string or percentage property is set by Chromeleon.
        /// Note that one handler can handle several properties as well.
        /// </summary>
        /// <param name="args">The SetPropertyEventArgs contain both a reference to
        /// the property being set and the new value.</param>
        private void OnSetProperty(SetPropertyEventArgs args)
        {
            // Find out which property was meant.
            if (args.Property == m_AnyTextProperty)
            {
                SetStringPropertyEventArgs stringArgs = args as SetStringPropertyEventArgs;
                m_MyString = stringArgs.NewValue;
                m_AnyTextProperty.Update(m_MyString);
            }
            else if (args.Property == m_PercentageProperty)
            {
                SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
                Debug.Assert(doubleArgs.NewValue.HasValue);
                m_Percentage = doubleArgs.NewValue.Value;
                m_PercentageProperty.Update(m_Percentage);
            }
            else if (args.Property == m_FilterTimeConstantProperty)
            {
                SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
                Debug.Assert(doubleArgs.NewValue.HasValue);
                m_FilterTimeConstantProperty.Update(doubleArgs.NewValue.Value);
            }
            else if (args.Property == m_MeasurementRangeProperty)
            {
                SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
                Debug.Assert(doubleArgs.NewValue.HasValue);
                m_MeasurementRangeProperty.Update(doubleArgs.NewValue.Value);
            }
            else
            {
                // This mustn't happen.
                Debug.Fail("Unknown property.");
            }
        }

        /// <summary>
        /// Called when the "ToggleEnableClock" command is sent by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain a reference to
        /// the command being called.</param>
        private void OnToggleEnableClock(CommandEventArgs args)
        {
            Debug.Assert(args.Command == m_Command);

            // We should toggle the m_EnableClock value here
            int newValue = (m_EnableClock == 1) ? 0 : 1;

            // We use the OnEnableClock handler to set the property. This avoids code duplication.
            // The last two parameters are not used by our handler, so it is safe to set them to null.
            SetIntPropertyEventArgs intArgs =
                new SetIntPropertyEventArgs(m_EnableClockProperty, newValue, null, null);
            OnEnableClock(intArgs);
        }

        /// <summary>
        /// Called when the "SetEnableClock" command is sent by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain a reference to
        /// the command being called and the argument list.</param>
        private void OnSetEnableClock(CommandEventArgs args)
        {
            Debug.Assert(args.Command == m_SetEnableClockCommand);

            // Get the parameter
            IIntParameterValue enableValue =
                args.ParameterValue(m_SetEnableClockCommand.FindParameter("Enable")) as IIntParameterValue;
            Debug.Assert(enableValue != null, "Required parameter is missing!");

            Debug.Assert(enableValue.Value.HasValue);
            int newValue = enableValue.Value.Value;

            // We use the OnEnableClock handler to set the property. This avoids code duplication.
            // The last two parameters are not used by our handler, so it is safe to set them to null.
            SetIntPropertyEventArgs intArgs =
                new SetIntPropertyEventArgs(m_EnableClockProperty, newValue, null, null);
            OnEnableClock(intArgs);
        }

        /// <summary>
        /// Called when the "SetPercentage" command is sent by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain a reference to
        /// the command being called and the argument list.</param>
        private void OnSetPercentage(CommandEventArgs args)
        {
            Debug.Assert(args.Command == m_SetPercentageCommand);

            // Get the parameter
            IDoubleParameterValue percentValue =
                args.ParameterValue(m_SetPercentageCommand.FindParameter("Value")) as IDoubleParameterValue;

            Debug.Assert(percentValue != null, "Required parameter is missing!");

            Debug.Assert(percentValue.Value.HasValue);
            double newValue = percentValue.Value.Value;

            // We use the OnSetProperty handler to set the property. This avoids code duplication.
            // The last two parameters are not used by our handler, so it is safe to set them to null.
            SetDoublePropertyEventArgs doubleArgs =
                new SetDoublePropertyEventArgs(m_PercentageProperty, newValue, null, null);
            OnSetProperty(doubleArgs);
        }

        /// <summary>
        /// Called when the "CommandWith4Parameters" command is sent by Chromeleon.
        /// </summary>
        /// <param name="args">The CommandEventArgs contain a reference to
        /// the command being called and the argument list.</param>
        private void OnCommandWith4Parameters(CommandEventArgs args)
        {
            Debug.Assert(args.Command == m_CommandWith4Parameters);

            // This command does not really do anything but retrieving the different parameters
            // and showing them in the audit trail.

            StringBuilder sb = new StringBuilder("CommandWith4Parameters was called with the following parameters:");

            // check for Param1
            IIntParameterValue param1Value =
                args.ParameterValue(m_CommandWith4Parameters.FindParameter("Param1")) as IIntParameterValue;

            sb.Append("\nParam1=");

            if (param1Value != null)
            {
                sb.Append(param1Value.Value);
            }
            else
            {
                sb.Append("<not set>");
            }

            // check for Param2
            IDoubleParameterValue param2Value =
                args.ParameterValue(m_CommandWith4Parameters.FindParameter("Param2")) as IDoubleParameterValue;

            sb.Append("\nParam2=");

            if (param2Value != null)
            {
                sb.Append(param2Value.Value);
            }
            else
            {
                sb.Append("<not set>");
            }

            // check for Param3
            IIntParameterValue param3Value =
                args.ParameterValue(m_CommandWith4Parameters.FindParameter("Param3")) as IIntParameterValue;

            sb.Append("\nParam3=");

            if (param3Value != null)
            {
                sb.Append(param3Value.Value);
            }
            else
            {
                sb.Append("<not set>");
            }


            // check for Param4
            IStringParameterValue param4Value =
                args.ParameterValue(m_CommandWith4Parameters.FindParameter("Param4")) as IStringParameterValue;

            sb.Append("\nParam4=");

            if (param4Value != null)
            {
                sb.Append(param4Value.Value);
            }
            else
            {
                sb.Append("<not set>");
            }

            m_MyCmDevice.AuditMessage(AuditLevel.Message, sb.ToString());
        }
    }
}
