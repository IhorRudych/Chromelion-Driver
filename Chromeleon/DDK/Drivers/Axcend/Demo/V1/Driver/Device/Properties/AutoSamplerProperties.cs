// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class AutoSamplerProperties
    {
        public readonly IIntProperty Ready;

        public AutoSamplerProperties(IDDK ddk, IDevice device)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");

            Ready = Property.CreateReady(ddk, device);
        }
    }
}
