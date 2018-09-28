// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Pump : Device
    {
        public Pump(IDriverEx driver, IDDK ddk, Config.Pump config, string id)
            : base(driver, ddk, typeof(Pump).Name, id, config.Name)
        {
        }
    }
}
