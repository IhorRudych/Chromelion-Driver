// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DetectorProperties
    {

        public DetectorProperties(IDDK ddk, IDevice device, Config.Detector config)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");
            if (config == null)
                throw new ArgumentNullException("config");

        }
    }
}
