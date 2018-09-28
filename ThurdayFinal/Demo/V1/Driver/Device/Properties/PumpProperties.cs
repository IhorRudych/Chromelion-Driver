// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class PumpProperties
    {
        public readonly IIntProperty Ready;

        private readonly IStruct m_Pressure;
        public readonly IDoubleProperty PressureValue;
        public readonly IDoubleProperty PressureLowerLimit;
        public readonly IDoubleProperty PressureUpperLimit;

        public PumpProperties(IDDK ddk, IDevice device)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");

            Ready = Property.CreateReady(ddk, device);

            // Pressure.LowerLimit
            // Pressure.UpperLimit
            // Pressure.Value
            m_Pressure = device.CreateStruct("Pressure", "The pump pressure.");

            ITypeDouble pressureType = ddk.CreateDouble(0, 400, 3);
            pressureType.Unit = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_Bar);

            PressureValue = m_Pressure.CreateStandardProperty(StandardPropertyID.Value, pressureType);
            PressureValue.Update(0);

            PressureLowerLimit = m_Pressure.CreateStandardProperty(StandardPropertyID.LowerLimit, pressureType);
            PressureLowerLimit.Update(pressureType.Minimum);

            PressureUpperLimit = m_Pressure.CreateStandardProperty(StandardPropertyID.UpperLimit, pressureType);
            PressureUpperLimit.Update(pressureType.Maximum);

            m_Pressure.DefaultGetProperty = PressureValue;
        }
    }
}
