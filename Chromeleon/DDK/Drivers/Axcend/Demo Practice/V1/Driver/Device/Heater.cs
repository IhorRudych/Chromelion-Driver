// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Threading;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Heater : Device
    {
        public enum TemperatureControl
        {
            Off,
            On,
        };


        public Heater(IDriverEx driver, IDDK ddk, Config.Heater config, string id)
            : base(driver, ddk, config, typeof(Heater).Name, id)
        {
        }
    }
}
