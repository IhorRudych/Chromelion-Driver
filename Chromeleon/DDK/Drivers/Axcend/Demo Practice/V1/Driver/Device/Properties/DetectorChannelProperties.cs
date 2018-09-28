// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DetectorChannelProperties : ChannelProperties
    {
        public DetectorChannelProperties(IDDK ddk, IDevice device, int deviceNumber, IChannel channel)
            : base(ddk,  channel)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");
            if (deviceNumber <= 0)
                throw new ArgumentException("Parameter deviceNumber = " + deviceNumber.ToString() + " must be > 0");

        }
    }
}
