// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Detector : Device
    {
        //private readonly DetectorProperties m_Properties;

        //private readonly List<DetectorChannel> m_Channels;

        public Detector(IDriverEx driver, IDDK ddk, Config.Detector config, string id)
            : base(driver, ddk, config, typeof(Detector).Name, id)
        {
        }
    }
}
