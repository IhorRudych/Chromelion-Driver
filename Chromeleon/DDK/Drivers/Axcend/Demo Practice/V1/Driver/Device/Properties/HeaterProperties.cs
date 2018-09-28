// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class HeaterProperties
    {
        public HeaterProperties(IDDK ddk, Config.Heater config, IDevice device)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (config == null)
                throw new ArgumentNullException("config");
            if (device == null)
                throw new ArgumentNullException("device");

        }
    }
}
