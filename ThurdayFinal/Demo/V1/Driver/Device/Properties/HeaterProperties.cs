// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class HeaterProperties
    {
        public readonly IIntProperty Ready;

        private readonly IStruct m_Product;
        public readonly IStringProperty m_ProductName;
        public readonly IStringProperty m_ProductDescription;

        public readonly IDoubleProperty Power;

        // Temperature: Control (On, Off), Min, Max, Nominal (Requested/Desired), Value (Current)
        private readonly IStruct m_Temperature;
        public readonly IIntProperty TemperatureControl;
        public readonly IDoubleProperty TemperatureMin;
        public readonly IDoubleProperty TemperatureMax;
        public readonly IDoubleProperty TemperatureNominal;
        public readonly IDoubleProperty TemperatureValue;

        public HeaterProperties(IDDK ddk, Config.Heater config, IDevice device)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (config == null)
                throw new ArgumentNullException("config");
            if (device == null)
                throw new ArgumentNullException("device");

            Ready = Property.CreateReady(ddk, device);

            m_Product = device.CreateStruct("Product", "Product Help Text");
            m_ProductName = Property.CreateString(ddk, m_Product, "Name");
            m_ProductName.Writeable = true;
            m_ProductName.Update("Product Name");
            m_ProductDescription = Property.CreateString(ddk, m_Product, "Description");
            m_ProductDescription.Update(config.ProductDescription);
            // Set the default read and write properties for the structure - optional
            m_Product.DefaultGetProperty = m_ProductName;
            m_Product.DefaultSetProperty = m_ProductName;

            Power = Property.CreateDouble(ddk, device, "Power", UnitConversion.PhysUnitEnum.PhysUnit_Watt, null, 0);  // the unit W is localized

            // Temperature.Control - On/Off
            // Temperature.LowerLimit
            // Temperature.UpperLimit
            // Temperature.Nominal
            // Temperature.Value
            m_Temperature = device.CreateStruct("Temperature", "Heater Temperature");

            TemperatureControl = Property.CreateEnum(ddk, m_Temperature, "Control", Heater.TemperatureControl.Off);
            TemperatureControl.Writeable = true;

            string temperatureUnit = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_Celsius);  // °C - localized
            const int temperaturePrecision = 1;

            ITypeDouble temperatureType = ddk.CreateDouble(0, 100, temperaturePrecision);
            temperatureType.Unit = temperatureUnit;

            TemperatureMin = m_Temperature.CreateStandardProperty(StandardPropertyID.LowerLimit, temperatureType);
            TemperatureMin.Writeable = true;
            TemperatureMin.Update(20);

            TemperatureMax = m_Temperature.CreateStandardProperty(StandardPropertyID.UpperLimit, temperatureType);
            TemperatureMax.Writeable = true;
            TemperatureMax.Update(80);

            TemperatureNominal = m_Temperature.CreateStandardProperty(StandardPropertyID.Nominal, temperatureType);  // Desired (requested) temperature
            TemperatureNominal.Writeable = true;
            TemperatureNominal.Update(50);

            temperatureType = ddk.CreateDouble(double.MinValue, double.MaxValue, temperaturePrecision);
            temperatureType.Unit = temperatureUnit;
            TemperatureValue = m_Temperature.CreateStandardProperty(StandardPropertyID.Value, temperatureType);
            TemperatureValue.Update(40);

            // Set the default read and write properties for the structure
            m_Temperature.DefaultGetProperty = TemperatureValue;
            m_Temperature.DefaultSetProperty = TemperatureNominal;
        }
    }
}
