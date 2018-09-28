// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DetectorProperties
    {
        public readonly IIntProperty Ready;
        public readonly IIntProperty IsAutoZeroRunning;

        public DetectorProperties(IDDK ddk, IDevice device, Config.Detector config)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");
            if (config == null)
                throw new ArgumentNullException("config");

            Ready = Property.CreateReady(ddk, device);

            IsAutoZeroRunning = Property.CreateBool(ddk, device, "IsAutoZeroRunning");
            IsAutoZeroRunning.Update(Property.GetBoolNumber(false));

            IIntProperty channelsNumber = Property.CreateInt(ddk, device, "ChannelsNumber");
            channelsNumber.Update(config.ChannelsNumber);
        }
    }
}
