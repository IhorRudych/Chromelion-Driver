// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;
using MyCompany.Demo.Properties;

namespace MyCompany
{
    internal static class Property
    {
        #region Names
        public static class Name
        {
            public static readonly string Connected = StandardPropertyID.Connected.ToString();
        }

        // Properties names which values that can be used in the ePanel selector XSL file to identify a device
        public static class ConstantName
        {
            public static readonly string DeviceType = "DeviceType";
            public static readonly string DriverID = "DriverID";
            public static readonly string LicenseProvider = "LicenseProvider";
            public static readonly string ModelNo = StandardPropertyID.ModelNo.ToString();
            public static readonly string ModelVariant = "ModelVariant";
            public static readonly string SampleLoadingPump = "SampleLoadingPump";
            public static readonly string Location = "Location";
            public static readonly string ValveType = "ValveType";
            public static readonly string FirmwareVersion = "FirmwareVersion";
            public static readonly string HardwareVersion = "HardwareVersion";
            public static readonly string PumpDeviceName = "PumpDeviceName";
            public static readonly string ModuleHardwareRevision = "ModuleHardwareRevision";
            public static readonly string InternalName = "InternalName";
        }
        #endregion

        #region Valid Values
        // Setting an arbitrary property to null (property.Update(null)), causes DDK.RequestPropertyValue to never return a response
        //public static readonly string EmptyString = null;
        //public static readonly Nullable<int> EmptyNumber = null;

        public static readonly string EmptyString = string.Empty;
        #endregion

        #region Get/Set Value
        public static string GetString(SetPropertyEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetStringPropertyEventArgs stringArgs = args as SetStringPropertyEventArgs;
            if (stringArgs == null)
                throw new ArgumentException("Parameter args type " + args.GetType().FullName + " is not " + typeof(SetStringPropertyEventArgs));
            return stringArgs.NewValue;
        }

        public static int GetInt(SetPropertyEventArgs args)
        {
            Nullable<int> value = GetIntNullable(args);
            return value.GetValueOrDefault();
        }

        public static Nullable<int> GetIntNullable(SetPropertyEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetIntPropertyEventArgs intAgrs = args as SetIntPropertyEventArgs;
            if (intAgrs == null)
                throw new ArgumentException("Parameter args type " + args.GetType().FullName + " is not " + typeof(SetIntPropertyEventArgs));
            return intAgrs.NewValue;
        }

        public static bool GetBool(SetPropertyEventArgs args)
        {
            int number = GetInt(args);
            bool result = GetBool(number);
            return result;
        }

        public static double GetDouble(SetPropertyEventArgs args)
        {
            Nullable<double> value = GetDoubleNullable(args);
            return value.GetValueOrDefault();
        }

        public static Nullable<double> GetDoubleNullable(SetPropertyEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            if (doubleArgs == null)
                throw new ArgumentException("Parameter args type " + args.GetType().FullName + " is not " + typeof(SetDoublePropertyEventArgs));
            return doubleArgs.NewValue;
        }

        public static SetDoubleRampEventArgs GetDoubleRampEventArgs(SetRampEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetDoubleRampEventArgs rampEventArgs = args as SetDoubleRampEventArgs;
            if (rampEventArgs == null)
                throw new ArgumentException("Parameter args type " + args.GetType().FullName + " is not " + typeof(SetDoubleRampEventArgs));
            return rampEventArgs;
        }

        public static string GetDoubleRampEventArgsText(SetDoubleRampEventArgs ramp)
        {
            string result = "RetentionTime = " + Property.GetDoubleValueText(ramp.RetentionTime) + ", " +
                            "Start = " + Property.GetDoubleValueText(ramp.StartValue) + ", " +
                            "End = " + Property.GetDoubleValueText(ramp.EndValue) + ", " +
                            "Duration = " + Property.GetDoubleValueText(ramp.Duration);
            return result;
        }

        public static string GetDoubleValueText(Nullable<double> value)
        {
            if (value == null)
            {
                return "Null";
            }
            return value.GetValueOrDefault().ToString("F3");
        }

        public static string GetDoubleValueText(RetentionTime retentionTime)
        {
            if (retentionTime == null)
            {
                return "Null";
            }
            return retentionTime.Minutes.ToString("F3");
        }

        public static bool GetBool(Nullable<int> value)
        {
            return GetBool(value.GetValueOrDefault());
        }

        public static bool GetBool(int value)
        {
            return GetBool((uint)value);
        }

        public static bool GetBool(uint value)
        {
            return value != 0;
        }

        public static byte GetBoolNumber(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        public static string GetValueName(IIntProperty property)
        {
            INamedIntList namedValues = property == null || property.DataType == null ? null : property.DataType.NamedValues;
            if (namedValues == null || property.Value == null)
            {
                return null;
            }

            string result = null;
            int value = property.Value.GetValueOrDefault();
            INamedInt namedValue = namedValues.Find(value);
            if (namedValue != null)
            {
                result = namedValue.Name;
            }
            return result;
        }
        #endregion

        #region Command Arguments
        public static string GetUserName(SetPropertyEventArgs args)
        {
            if (args == null)
                return null;
            return GetUserName(args.RunContext);
        }

        public static string GetUserName(CommandEventArgs args)
        {
            if (args == null)
                return null;
            return GetUserName(args.RunContext);
        }

        private static string GetUserName(IRunContext runContext)
        {
            if (runContext == null)
                return null;
            if (runContext.UserInfo == null)
                return null;
            string result = runContext.UserInfo.Name;
            return result;
        }

        public static bool TryGetParameterBool(CommandEventArgs args, string paramName, out bool value)
        {
            Nullable<int> paramValue;
            bool result = GetParameterInt(args, paramName, false, true, out paramValue);
            value = GetBool(paramValue.GetValueOrDefault());
            return result;
        }

        public static bool GetParameterBool(CommandEventArgs args, string paramName)
        {
            Nullable<int> paramValue;
            GetParameterInt(args, paramName, true, false, out paramValue);
            bool result = GetBool(paramValue.GetValueOrDefault());
            return result;
        }

        public static bool TryGetParameterInt(CommandEventArgs args, string paramName, out int value)
        {
            Nullable<int> paramValue;
            bool result = GetParameterInt(args, paramName, false, true, out paramValue);
            value = paramValue.GetValueOrDefault();
            return result;
        }

        public static int GetParameterInt(CommandEventArgs args, string paramName)
        {
            Nullable<int> paramValue;
            GetParameterInt(args, paramName, true, false, out paramValue);
            int result = paramValue.GetValueOrDefault();
            return result;
        }

        public static bool TryGetParameterIntNullable(CommandEventArgs args, string paramName, out Nullable<int> paramValue)
        {
            paramValue = null;
            bool result = GetParameterInt(args, paramName, false, true, out paramValue);
            return result;
        }

        private static bool GetParameterInt(CommandEventArgs args, string paramName, bool throwException, bool allowNull, out Nullable<int> value)
        {
            value = null;
            IParameterValue parameterValue = GetParameterValue(args, paramName, throwException);
            if (parameterValue == null)
            {
                return false;
            }

            IIntParameterValue param = parameterValue as IIntParameterValue;
            if (param == null)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterIncorrectType, args.Command.Name, paramName, parameterValue.GetType().FullName, typeof(IIntParameterValue).FullName));
                return false;
            }

            value = param.Value;
            if (value == null && !allowNull)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterValueIsEmpty, args.Command.Name, paramName));
            }
            return true;
        }

        public static bool TryGetParameterDouble(CommandEventArgs args, string paramName, out double value)
        {
            Nullable<double> paramValue;
            bool result = GetParameterDouble(args, paramName, false, true, out paramValue);
            value = paramValue.GetValueOrDefault();
            return result;
        }

        public static double GetParameterDouble(CommandEventArgs args, string paramName)
        {
            Nullable<double> paramValue;
            GetParameterDouble(args, paramName, true, false, out paramValue);
            double result = paramValue.GetValueOrDefault();
            return result;
        }

        public static bool TryGetParameterDoubleNullable(CommandEventArgs args, string paramName, out Nullable<double> paramValue)
        {
            paramValue = null;
            bool result = GetParameterDouble(args, paramName, false, true, out paramValue);
            return result;
        }

        private static bool GetParameterDouble(CommandEventArgs args, string paramName, bool throwException, bool allowNull, out Nullable<double> value)
        {
            value = null;
            IParameterValue parameterValue = GetParameterValue(args, paramName, throwException);
            if (parameterValue == null)
            {
                return false;
            }

            IDoubleParameterValue param = parameterValue as IDoubleParameterValue;
            if (param == null)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterIncorrectType, args.Command.Name, paramName, parameterValue.GetType().FullName, typeof(IDoubleParameterValue).FullName));
                return false;
            }

            value = param.Value;
            if (value == null && !allowNull)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterValueIsEmpty, args.Command.Name, paramName));
            }
            return true;
        }

        public static bool TryGetParameterString(CommandEventArgs args, string paramName, out string value)
        {
            return GetParameterString(args, paramName, false, true, out value);
        }

        public static string GetParameterString(CommandEventArgs args, string paramName)
        {
            string value;
            GetParameterString(args, paramName, true, false, out value);
            return value;
        }

        private static bool GetParameterString(CommandEventArgs args, string paramName, bool throwException, bool allowNull, out string value)
        {
            value = null;
            IParameterValue parameterValue = GetParameterValue(args, paramName, throwException);
            if (parameterValue == null)
            {
                return false;
            }

            IStringParameterValue param = parameterValue as IStringParameterValue;
            if (param == null)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterIncorrectType, args.Command.Name, paramName, parameterValue.GetType().FullName, typeof(IStringParameterValue).FullName));
                return false;
            }

            value = param.Value;
            if (string.IsNullOrEmpty(value) && !allowNull)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterValueIsEmpty, args.Command.Name, paramName));
            }
            return true;
        }

        private static IParameterValue GetParameterValue(CommandEventArgs args, string paramName, bool throwException)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Command == null)
                throw new ArgumentNullException("args.Command");

            IParameter parameter = args.Command.FindParameter(paramName);
            if (parameter == null)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterNotProvided, args.Command.Name, paramName));
                return null;
            }

            IParameterValue result = args.ParameterValue(parameter);
            if (result == null)
            {
                if (throwException)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CommandParameterValueNotFound, args.Command.Name, paramName));
                return null;
            }

            return result;
        }
        #endregion

        #region Create Type
        public static ITypeInt CreateTypeBool(IDDK ddk)
        {
            return CreateIntType(ddk, GetBoolNumber(false), GetBoolNumber(true));
        }

        public static ITypeString CreateTypeString(IDDK ddk)
        {
            return ddk.CreateString(300);
        }
        #endregion

        #region Create Property
        public static IStringProperty CreateString(IDDK ddk, IDevice device, string name, string helpText = null, int valueMaxLength = 300)
        {
            if (valueMaxLength <= 0)
                throw new ArgumentOutOfRangeException("Parameter valueMaxLength must be > 0");
            return device.CreateProperty(name, helpText, ddk.CreateString(valueMaxLength));
        }

        public static IStringProperty CreateString(IDDK ddk, IStruct structure, string name, string helpText = null, int valueMaxLength = 300)
        {
            if (valueMaxLength <= 0)
                throw new ArgumentOutOfRangeException("Parameter valueMaxLength must be > 0");
            return structure.CreateProperty(name, helpText, ddk.CreateString(valueMaxLength));
        }

        public static IIntProperty CreateBool(IDDK ddk, IDevice device, string name, string helpText = null)
        {
            IIntProperty result = device.CreateBooleanProperty(name, helpText, false.ToString(), true.ToString());
            return result;
        }

        public static IIntProperty CreateBool(IDDK ddk, IStruct structure, string name, string helpText = null)
        {
            IIntProperty result = result = structure.CreateBooleanProperty(name, helpText, false.ToString(), true.ToString());
            return result;
        }

        public static IIntProperty CreateBoolYN(IDDK ddk, IDevice device, string name, string helpText = null)
        {
            IIntProperty result = device.CreateBooleanProperty(name, helpText, "No", "Yes");
            return result;
        }

        public static IIntProperty CreateReady(IDDK ddk, IDevice device)
        {
            ITypeInt type = CreateBoolType(ddk);
            IIntProperty result = device.CreateStandardProperty(StandardPropertyID.Ready, type);
            result.Update(GetBoolNumber(true));
            return result;
        }

        public static IIntProperty CreateBoolYN(IDDK ddk, IStruct structure, string name, string helpText = null)
        {
            IIntProperty result = result = structure.CreateBooleanProperty(name, helpText, "No", "Yes");
            return result;
        }

        public static IIntProperty CreateInt(IDDK ddk, IDevice device, string name, string helpText = null)
        {
            ITypeInt typeInt = CreateIntType(ddk);
            IIntProperty result = device.CreateProperty(name, helpText, typeInt);
            return result;
        }

        public static IIntProperty CreateInt(IDDK ddk, IStruct structure, string name, string helpText = null)
        {
            ITypeInt typeInt = CreateIntType(ddk);
            IIntProperty result = structure.CreateProperty(name, helpText, typeInt);
            return result;
        }

        private static ITypeInt CreateIntType(IDDK ddk, int minValue = int.MinValue + 1, int maxValue = int.MaxValue)
        {
            // The Real Time Kernel has one special value for a property that is used for an undefined / empty value.
            // In case of an integer property this is int.MinValue = -2147483648 = (-2147483647 - 1) == INVALID_INT == INVALID_LONG == INT_MIN == LONG_MIN.
            // This value should not be used as the minimum for the property.
            if (minValue == int.MinValue)
                throw new ArgumentOutOfRangeException("Cannot create an integer type with minimum value minValue = int.MinValue = " + int.MinValue.ToString() + ". " +  // -2147483648
                                                      "The minimum value cannot be smaller than int.MinValue + 1 = " + (int.MinValue + 1).ToString());                  // -2147483647

            ITypeInt result = ddk.CreateInt(minValue, maxValue);
            return result;
        }

        public static IDoubleProperty CreateDouble(IDDK ddk, IDevice device, string name,
                                                   Nullable<UnitConversion.PhysUnitEnum> unit = null, string helpText = null,
                                                   double minValue = double.MinValue, double maxValue = double.MaxValue, int precision = 3)
        {
            ITypeDouble typeDouble = CreateDoubleType(ddk, unit, minValue, maxValue, precision);
            IDoubleProperty result = device.CreateProperty(name, helpText, typeDouble);
            return result;
        }

        public static IDoubleProperty CreateDouble(IDDK ddk, IStruct structure, string name,
                                                   Nullable<UnitConversion.PhysUnitEnum> unit = null, string helpText = null,
                                                   double minValue = double.MinValue, double maxValue = double.MaxValue, int precision = 3)
        {
            ITypeDouble typeDouble = CreateDoubleType(ddk, unit, minValue, maxValue, precision);
            IDoubleProperty result = structure.CreateProperty(name, helpText, typeDouble);
            return result;
        }

        public static ITypeInt CreateBoolType(IDDK ddk)
        {
            int numberFalse = GetBoolNumber(false);
            int numberTrue = GetBoolNumber(true);
            ITypeInt result = CreateIntType(ddk, numberFalse, numberTrue);
            result.AddNamedValue(false.ToString(), numberFalse);
            result.AddNamedValue(true.ToString(), numberTrue);
            return result;
        }

        public static ITypeDouble CreatePercentType(IDDK ddk, double min, double max, int digits)
        {
            ITypeDouble result = ddk.CreateDouble(min, max, digits);
            result.Unit = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_Percent);
            return result;
        }

        public static ITypeDouble CreateDoubleType(IDDK ddk,
                                                   Nullable<UnitConversion.PhysUnitEnum> unit = null,
                                                   double minValue = double.MinValue, double maxValue = double.MaxValue, int precision = 3)
        {
            ITypeDouble result = ddk.CreateDouble(minValue, maxValue, precision);
            if (unit != null)
            {
                result.Unit = UnitConversionEx.PhysUnitName(unit.GetValueOrDefault());
            }
            return result;
        }

        public static IIntProperty CreateEnum<T>(IDDK ddk, IDevice device, string name, string helpText = null)
        {
            ITypeInt typeInt = CreateEnumType<T>(ddk);
            IIntProperty result = device.CreateProperty(name, string.Empty, typeInt);
            result.HelpText = helpText;
            return result;
        }

        public static IIntProperty CreateEnum<T>(IDDK ddk, IDevice device, string name, T value, string helpText = null)
        {
            ITypeInt typeInt = CreateEnumType<T>(ddk);
            IIntProperty result = device.CreateProperty(name, string.Empty, typeInt);
            UpdateEnumProperty(result, value, helpText);
            return result;
        }

        public static IIntProperty CreateEnum<T>(IDDK ddk, IStruct structure, string name, string helpText = null)
        {
            ITypeInt typeInt = CreateEnumType<T>(ddk);
            IIntProperty result = structure.CreateProperty(name, string.Empty, typeInt);
            result.HelpText = helpText;
            return result;
        }

        public static IIntProperty CreateEnum<T>(IDDK ddk, IStruct structure, string name, T value, string helpText = null)
        {
            ITypeInt typeInt = CreateEnumType<T>(ddk);
            IIntProperty result = structure.CreateProperty(name, string.Empty, typeInt);
            UpdateEnumProperty(result, value, helpText);
            return result;
        }

        private static ITypeInt CreateEnumType<T>(IDDK ddk)
        {
            Type type = typeof(T);
            List<string> valueNames = Enum.GetNames(type).ToList();
            List<int> valueNumbers = Enum.GetValues(type).Cast<int>().ToList();

            List<int> orderedStateNumbers = valueNumbers.OrderBy(item => item).ToList();
            int valueNumberMin = orderedStateNumbers[0];
            int valueNumberMax = orderedStateNumbers[valueNumbers.Count - 1];

            ITypeInt result = CreateIntType(ddk, valueNumberMin, valueNumberMax);

            for (int i = 0; i < valueNumbers.Count; i++)
            {
                string valueName = valueNames[i];
                int valueNumber = valueNumbers[i];
                result.AddNamedValue(valueName, valueNumber);
            }

            return result;
        }

        private static void UpdateEnumProperty<T>(IIntProperty property, T value, string helpText)
        {
            property.HelpText = helpText;

            int valueNumber = Convert.ToInt32(value);
            property.Update(valueNumber);
        }
        #endregion

        #region Clear Data
        public static void ClearData(IEnumerable<IStringProperty> items, string defaultValue = null)
        {
            if (items == null)
            {
                return;
            }

            foreach (IStringProperty item in items)
            {
                item.Update(defaultValue);
            }
        }

        public static void ClearData(IEnumerable<IIntProperty> items, Nullable<int> defaultValue = null)
        {
            if (items == null)
            {
                return;
            }

            foreach (IIntProperty item in items)
            {
                item.Update(defaultValue);
            }
        }

        public static void ClearData(IEnumerable<IDoubleProperty> items, Nullable<double> defaultValue = null)
        {
            if (items == null)
            {
                return;
            }

            foreach (IDoubleProperty item in items)
            {
                item.Update(defaultValue);
            }
        }
        #endregion

        #region Get Binary Display Text
        public static string GetBinaryDisplayText(uint value, int padLeftTotalWidth = 32)
        {
            if (value == 0)
            {
                return EmptyString;
            }

            string result = Convert.ToString(value, 2).PadLeft(padLeftTotalWidth, '0');

            // Formated Binary Display Text:
            // 0000 0000 0000 0001  0000 0000 0000 0001
            int position = 4;
            while (position < result.Length)
            {
                result = result.Insert(position, " ");
                position += 5;
            }
            result = result.Insert(result.Length / 2, " ");

            return result;
        }
        #endregion

        #region Get Text
        public static string GetText(IEnumerable<IStringProperty> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (IStringProperty item in items)
            {
                string value;
                if (item == null)
                {
                    value = "Null Item";
                }
                else if (item.Value == null)
                {
                    value = "null";
                }
                else
                {
                    value = item.Value;
                }

                if (sb.Length > 0)
                {
                    sb.Append("; ");
                }
                sb.Append(value);
            }

            string result = sb.ToString();
            return result;
        }

        public static string GetText(IEnumerable<IIntProperty> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (IIntProperty item in items)
            {
                string value;
                if (item == null)
                {
                    value = "Null Item";
                }
                else if (item.Value == null)
                {
                    value = "null";
                }
                else
                {
                    int number = item.Value.GetValueOrDefault();
                    value = number.ToString("N");
                }

                if (sb.Length > 0)
                {
                    sb.Append("; ");
                }
                sb.Append(value);
            }

            string result = sb.ToString();
            return result;
        }

        public static string GetText(IEnumerable<IDoubleProperty> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (IDoubleProperty item in items)
            {
                string value;
                if (item == null)
                {
                    value = "Null Item";
                }
                else if (item.Value == null)
                {
                    value = "null";
                }
                else
                {
                    double number = item.Value.GetValueOrDefault();
                    value = number.ToString("N");
                }

                if (sb.Length > 0)
                {
                    sb.Append("; ");
                }
                sb.Append(value);
            }

            string result = sb.ToString();
            return result;
        }
        #endregion

        #region Preflight
        public class Preflight
        {
            private readonly IRunContext m_runContext;

            public Preflight(IRunContext runContext)
            {
                if (runContext == null)
                    throw new ArgumentNullException("runContext");
                m_runContext = runContext;
            }

            /// <summary>Returns the value before the script assignment</summary>
            public Nullable<int> GetPreconditionValue(IIntProperty property)
            {
                IPropertyValue propertyValue = GetPreconditionPropertyValue(property);
                IIntPropertyValue intPropertyValue = propertyValue as IIntPropertyValue;
                Nullable<int> result = intPropertyValue.Value;
                return result;
            }

            /// <summary>Returns the value before the script assignment</summary>
            public Nullable<double> GetPreconditionValue(IDoubleProperty property)
            {
                IPropertyValue propertyValue = GetPreconditionPropertyValue(property);
                IDoublePropertyValue doublePropertyValue = propertyValue as IDoublePropertyValue;
                Nullable<double> result = doublePropertyValue.Value;
                return result;
            }

            private IPropertyValue GetPreconditionPropertyValue(IProperty property)
            {
                IDevice owner = property.Device;
                IPropertyValue result = m_runContext.Precondition(owner).PropertyValue(property);
                return result;
            }

            /// <summary>Returns the value in the script assignment</summary>
            public Nullable<double> GetScriptValue(IDoubleProperty property)
            {
                Nullable<double> result = GetScriptOrPreconditionValue(property, false);
                return result;
            }

            /// <summary>Returns the value in the script assignment if found, else the precondition value</summary>
            public Nullable<double> GetCurrentValue(IDoubleProperty property)
            {
                Nullable<double> result = GetScriptOrPreconditionValue(property, true);
                return result;
            }

            /// <summary>Returns the value in the script assignment</summary>
            public Nullable<int> GetScriptValue(IIntProperty property)
            {
                Nullable<int> result = GetScriptOrPreconditionValue(property, false);
                return result;
            }

            /// <summary>Returns the value in the script assignment if found, else the precondition value</summary>
            public Nullable<int> GetCurrentValue(IIntProperty property)
            {
                Nullable<int> result = GetScriptOrPreconditionValue(property, true);
                return result;
            }

            private Nullable<double> GetScriptOrPreconditionValue(IDoubleProperty property, bool ifNotFoundReturnPreconditionValue)
            {
                Nullable<double> result = null;
                IPropertyValue propertyValue = GetPropertyValue(property);
                if (propertyValue == null)
                {
                    if (ifNotFoundReturnPreconditionValue)
                    {
                        result = GetPreconditionValue(property);
                    }
                    return result;
                }
                IDoublePropertyValue doublePropertyValue = propertyValue as IDoublePropertyValue;
                result = doublePropertyValue.Value;
                return result;
            }

            private Nullable<int> GetScriptOrPreconditionValue(IIntProperty property, bool ifNotFoundReturnPreconditionValue)
            {
                Nullable<int> result = null;
                IPropertyValue propertyValue = GetPropertyValue(property);
                if (propertyValue == null)
                {
                    if (ifNotFoundReturnPreconditionValue)
                    {
                        result = GetPreconditionValue(property);
                    }
                    return result;
                }
                IIntPropertyValue intPropertyValue = propertyValue as IIntPropertyValue;
                result = intPropertyValue.Value;
                return result;
            }

            private IPropertyValue GetPropertyValue(IProperty property)
            {
                IPropertyValue result = null;
                foreach (IProgramStep step in m_runContext.ProgramSteps)
                {
                    IPropertyAssignmentStep assignmentStep = step as IPropertyAssignmentStep;
                    if (assignmentStep != null && assignmentStep.Property == property)
                    {
                        result = assignmentStep.Value;
                    }
                }
                return result;
            }
        }
        #endregion
    }
}
