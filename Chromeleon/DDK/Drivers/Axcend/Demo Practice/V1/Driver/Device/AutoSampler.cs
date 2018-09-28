// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class AutoSampler : Device  // Also see examples AutoSampler, PreparingSampler
    {
        public AutoSampler(IDriverEx driver, IDDK ddk, Config.AutoSampler config, string id)
            : base(driver, ddk, typeof(AutoSampler).Name, id, config.Name)
        {
        }
    }
}
