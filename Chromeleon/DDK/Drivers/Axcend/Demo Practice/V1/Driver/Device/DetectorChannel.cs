// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DetectorChannel : Device
    {
        #region Fields
        public enum Location
        {
            Left = 1,
            Middle,
            Right,
        }

        private readonly int m_Number;
        #endregion

        public DetectorChannel(IDriverEx driver, IDDK ddk, IDevice owner, string id, string channelName, int channelNumber, UnitConversion.PhysUnitEnum unit, bool channelNeedsIntegration, string channelPhysicalQuantity)
            : base(driver, ddk, typeof(DetectorChannel).Name, id, channelName, owner, CreateChannel(ddk, channelName, unit))
        {
            m_Number = channelNumber;
        }

        private static IDevice CreateChannel(IDDK ddk, string name, UnitConversion.PhysUnitEnum unit)
        {
            ITypeInt typeSignal = ddk.CreateInt(0, 1000);
            typeSignal.Unit = UnitConversionEx.PhysUnitName(unit);
            IChannel result = ddk.CreateChannel(name, "Detector channel", typeSignal);
            return result;
        }
    }
}
