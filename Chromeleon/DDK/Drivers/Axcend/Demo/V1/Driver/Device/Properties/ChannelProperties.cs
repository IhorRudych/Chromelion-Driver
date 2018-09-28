// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class ChannelProperties
    {
        public readonly IDoubleProperty Rate;
        public readonly IDoubleProperty Wavelength;

        public readonly IStringProperty AcquisitionTimeOn;
        public readonly IStringProperty AcquisitionTimeOff;
        public readonly IStringProperty AcquisitionTimeDiff;

        public ChannelProperties(IDDK ddk, IChannel channel)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (channel == null)
                throw new ArgumentNullException("channel");

            ITypeDouble typeDouble = Property.CreateDoubleType(ddk, UnitConversion.PhysUnitEnum.PhysUnit_Hertz, 0.1, 1000, 1);
            Rate = channel.CreateProperty("Rate", "The data collection rate in " + typeDouble.Unit, typeDouble);
            Rate.Update(100);

            typeDouble = Property.CreateDoubleType(ddk, UnitConversion.PhysUnitEnum.PhysUnit_NanoMeter, 200, 400, 1);
            Wavelength = channel.CreateStandardProperty(StandardPropertyID.Wavelength, typeDouble);

            AcquisitionTimeOn = Property.CreateString(ddk, channel, "AcquisitionTime_1_On");
            AcquisitionTimeOff = Property.CreateString(ddk, channel, "AcquisitionTime_2_Off");
            AcquisitionTimeDiff = Property.CreateString(ddk, channel, "AcquisitionTime_3_Diff");

            // Make the property available as a report variable. This is stored with the signal as meta data.
            channel.AddPropertyToChannelInfo(Rate);
            channel.AddPropertyToChannelInfo(Wavelength);
        }
    }
}
