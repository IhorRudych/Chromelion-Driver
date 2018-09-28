/////////////////////////////////////////////////////////////////////////////
//
// SimpleDevice.cs
// ///////////////
//
// SimpleDriver Chromeleon DDK Code Example
//
// Device class for the SimpleDriver.
// The "SimpleDriver" is reduced to the bare minimum that is required to
// implement a driver having one device with a "MyProperty" property.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.SimpleDriver
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class SimpleDevice
    {
        #region Data Members

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// Our properties.
        private IStringProperty m_MyStringProperty;
        private IDoubleProperty m_MyDoubleProperty;
        private IIntProperty m_MyIntProperty;
        private IIntProperty m_MyProperBoolean;

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
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is my first DDK device.");

            m_MyStringProperty =
                m_MyCmDevice.CreateProperty("MyStringProperty", "A string property", cmDDK.CreateString(5));
            m_MyStringProperty.OnSetProperty += new SetPropertyEventHandler(m_MyStringProperty_OnSetProperty);
            m_MyStringProperty.OnPreflightSetProperty += m_MyStringProperty_OnPreflightSetProperty;

            // Create a Property of type double.
            m_MyDoubleProperty =
                m_MyCmDevice.CreateProperty("MyDoubleProperty", "This is my first property", cmDDK.CreateDouble(0, 20, 2));

            // Set the property to writable.
            m_MyDoubleProperty.Writeable = true;
            // And provide a handler that gets called when the property is assigned.
            m_MyDoubleProperty.OnSetProperty += new SetPropertyEventHandler(m_MyDoubleProperty_OnSetProperty);

            // Update all properties that have to be readable before the device is connected.
            m_MyDoubleProperty.Update(20.0);

            // Create a Property of type int.
            m_MyIntProperty =
                m_MyCmDevice.CreateProperty("MyIntProperty", "This is my second property", cmDDK.CreateInt(-1, 2));

            // Set the property to read-only.
            m_MyIntProperty.Writeable = false;

            // No value known until the device is connected.
            m_MyIntProperty.Update(null);

            CreateMoreProperties(cmDDK);

            return m_MyCmDevice;
        }

        void m_MyStringProperty_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            SetStringPropertyEventArgs stringArgs = args as SetStringPropertyEventArgs;
            String message = String.Format("New value #{0}#", stringArgs.NewValue);
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, message); // AuditLevel.Warning so that it shows up in Ready Check Results
        }

        void m_MyStringProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetStringPropertyEventArgs stringArgs = args as SetStringPropertyEventArgs;
            String newValue = stringArgs.NewValue;
            String message = String.Format("New value #{0}#", stringArgs.NewValue);
            m_MyCmDevice.AuditMessage(AuditLevel.Normal, message);
            m_MyStringProperty.Update(stringArgs.NewValue);
            m_MyStringProperty.LogValue();
            message = String.Format("Internal value #{0}#", m_MyStringProperty.Value);
            m_MyCmDevice.AuditMessage(AuditLevel.Normal, message);
        }

        void m_MyDoubleProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            Double? newValue = doubleArgs.NewValue;
            m_MyDoubleProperty.Update(doubleArgs.NewValue);
        }

        /// <summary>
        /// This illustrates variations on the ITypeDouble that can be used for creating properties
        /// </summary>
        internal void CreateMoreProperties(IDDK cmDDK)
        {
            ITypeDouble type1 = cmDDK.CreateDouble(-1, 1, 0);
            // Range: [-1...1]
            // - any number -1 <= x <= 1 can be assigned, will be rounded to 0 decimals
            // - user input is allowed with 0 decimals only (-1|0|1)
            // - numbers outside this range (-1|0|1) cause validation errors in panel edit controls or during Instrument Method syntax checking
            // - panel edit control's spin box will spin -1,0,1
            // - F8 box's/Instrument Method Editor's drop list will contain -1,0,1
            IDoubleProperty type1Property = m_MyCmDevice.CreateProperty("Type1Property", "", type1);
            type1Property.Update(-1); //set default value
            type1Property.OnSetProperty += new SetPropertyEventHandler(type1Property_OnSetProperty);

            ITypeDouble type2 = cmDDK.CreateDouble(-1, 1, 2);
            type2.Unit = "bar";
            // Range: [-1.00..1.00 bar]
            // - any number -1 <= x <= 1 can be assigned, will be rounded to 2 decimals
            // - user input is allowed with up to 2 decimals
            // - numbers outside this range cause validation errors in panel edit controls or during Instrument Method syntax checking
            // - unit "bar" is shown in Commands overview (F8),
            //   and can be appended to text shown in panel edit controls and panel string displays
            // - unit [bar] can be added to assignments in Instrument Method scripts,
            //   system can also convert from other units if it makes sense,
            //   e.g. Type2Property = 1 [psi] is equal to Type2Property = 0.0689475729 [bar]
            // - panel edit control's spin box will spin in 0.01 incrementals
            // - F8 box's/Instrument Method Editor's drop list will be empty as it would be too long (a maximum of 12 items can be shown)
            IDoubleProperty type2Property = m_MyCmDevice.CreateProperty("Type2Property", "", type2);
            type2Property.Update(0.0); //set default value
            type2Property.OnSetProperty += new SetPropertyEventHandler(type2Property_OnSetProperty);

            ITypeDouble type3 = cmDDK.CreateDouble(0, 20, 1);
            type3.AddNamedValue("Null", 0);
            type3.AddNamedValue("OnePointFive", 1.5);
            type3.AddNamedValue("Ten", 10);
            type3.AddNamedValue("Thirty", 30);
            // Range: [Null..20.0] - when a property is displayed or logged, a named value is used instead of a pure number if a named value is available.
            // - any number 0 <= x <= 20 can be assigned, will be rounded to 1 decimal
            // - 30 can also be assigned, even if it is outside the min/max range, because it has been added as named value
            // - user input is allowed with up to 1 decimal
            // - instead of Type3Property = 0, one can write Type3Property = Null
            // - instead of Type3Property = 1.5, one can write Type3Property = OnePointFive
            // - instead of Type3Property = 10, one can write Type3Property = Ten
            // - instead of Type3Property = 30, one can write Type3Property = Thirty
            // - panel edit control's spin box will spin in 0.1 incrementals: Null, 0.1, 0.2, ..., 1.4, OnePointFive, 1.6, .... 20.0, Thirty
            // - F8 box's/Instrument Method Editor's drop list contains the four named values
            IDoubleProperty type3Property = m_MyCmDevice.CreateProperty("Type3Property", "", type3);
            type3Property.Update(4);//set default value
            type3Property.OnSetProperty += new SetPropertyEventHandler(type3Property_OnSetProperty);

            ITypeDouble type4 = cmDDK.CreateDouble(0, 20, 1);
            type4.AddNamedValue("Null", 0);
            type4.AddNamedValue("Ten", 10);
            type4.AddNamedValue("Twenty", 20);
            type4.NamedValuesOnly = true;
            // Range: [Null..Twenty] - when a property is displayed or logged, a named value is used instead of a pure number if a named value is available.
            // - numbers 0.0, 10.0 and 20.0 can be assigned only, because of the 'NamedValuesOnly' flag.
            // - instead of Type3Property = 0, one can write Type3Property = Null
            // - instead of Type3Property = 10, one can write Type3Property = Ten
            // - instead of Type3Property = 20, one can write Type3Property = Twenty
            // - panel edit control's spin box will spin Null, Ten, Twenty only. Other values will cause validation errors
            // - F8 box's/Instrument Method Editor's drop list contains the three named values, other values will cause validation errors
            IDoubleProperty type4Property = m_MyCmDevice.CreateProperty("Type4Property", "", type4);
            type4Property.Update(0);//set default value
            type4Property.OnSetProperty += new SetPropertyEventHandler(type4Property_OnSetProperty);

            ITypeDouble type5 = cmDDK.CreateDouble(0, 20, 1);
            Double[] legalValues = { 1.1, 2.2, 3.3 };
            type5.LegalValues = legalValues;
            // Range: [0.0..20.0]
            // - any (!) number 0 <= x <= 20 can be assigned, will be rounded to 1 decimal
            // - user input is allowed with up to 1 decimal
            // - panel edit control's spin box will spin to 1.1, 2.2 and 3.3 (due to LegalValues)
            // - F8 box's/Instrument Method Editor's drop list contains the three legal values
            IDoubleProperty type5Property = m_MyCmDevice.CreateProperty("Type5Property", "", type5);
            type5Property.Update(2.2);//set default value
            type5Property.OnSetProperty += new SetPropertyEventHandler(type5Property_OnSetProperty);

            ITypeDouble type6 = cmDDK.CreateDouble(0, 20, 1);
            type6.LegalValues = legalValues;
            type6.AddNamedValue("Null", 0);
            type6.AddNamedValue("OnePointFive", 1.5);
            type6.AddNamedValue("Ten", 10);
            type6.AddNamedValue("Twenty", 20);
            // Range: [Null..20.0]
            // - any (!) number 0 <= x <= 20 can be assigned, will be rounded to 1 decimal
            // - user input is allowed with up to 1 decimal
            // - panel edit control's spin box will spin to all legal and named values
            // - F8 box's/Instrument Method Editor's drop list contains the seven legal and named values
            IDoubleProperty type6Property = m_MyCmDevice.CreateProperty("Type6Property", "", type6);
            type6Property.Update(10);//set default value
            type6Property.OnSetProperty += new SetPropertyEventHandler(type6Property_OnSetProperty);

            ITypeInt badBooleanType = cmDDK.CreateInt(0, 1);
            IIntProperty badBoolean1 = m_MyCmDevice.CreateProperty("BadBoolean", "This is a Boolean type without enumerations. Please define appropriate enumerations in the driver!", badBooleanType);
            badBoolean1.OnSetProperty += new SetPropertyEventHandler(badBoolean_OnSetProperty);

            m_MyProperBoolean = m_MyCmDevice.CreateBooleanProperty("ProperBoolean", "This is how to do it", "False_No_Zero_NotReady", "True_Yes_One_Ready");
            m_MyProperBoolean.OnSetProperty += new SetPropertyEventHandler(properBoolean_OnSetProperty);

            ITypeInt explicitBooleanType = cmDDK.CreateInt(0, 1);
            explicitBooleanType.AddNamedValue("False_No_Zero_NotReady", 0);
            explicitBooleanType.AddNamedValue("True_Yes_One_Ready", 1);
            explicitBooleanType.NamedValuesOnly = true;
            IIntProperty explicitBoolean = m_MyCmDevice.CreateProperty("ExplicitBoolean", "Also ok, but way more explicit code", explicitBooleanType);
            explicitBoolean.OnSetProperty += new SetPropertyEventHandler(explicitBoolean_OnSetProperty);
        }

        void explicitBoolean_OnSetProperty(SetPropertyEventArgs args)
        {
            throw new NotImplementedException();
        }

        void properBoolean_OnSetProperty(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs intArgs = args as SetIntPropertyEventArgs;
            m_MyProperBoolean.Update(intArgs.NewValue);
        }

        void badBoolean2_OnSetProperty(SetPropertyEventArgs args)
        {
            throw new NotImplementedException();
        }

        void badBoolean_OnSetProperty(SetPropertyEventArgs args)
        {
            throw new NotImplementedException();
        }

        void type6Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void type1Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void type2Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void type3Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void type4Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void type5Property_OnSetProperty(SetPropertyEventArgs args)
        {
            genericHandler(args);
        }

        void genericHandler(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs doubleArgs = args as SetDoublePropertyEventArgs;
            Double? newValue = doubleArgs.NewValue;
            IDoubleProperty doubleProperty = args.Property as IDoubleProperty;
            doubleProperty.Update(doubleArgs.NewValue);
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize properties that are visible in the connected state only.
        /// </summary>
        internal void OnConnect()
        {
            m_MyIntProperty.Update(1);
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// When we are disconnected, we clear properties that are visible in the connected state only.
        /// </summary>
        internal void OnDisconnect()
        {
            m_MyIntProperty.Update(null);
        }
    }
}
