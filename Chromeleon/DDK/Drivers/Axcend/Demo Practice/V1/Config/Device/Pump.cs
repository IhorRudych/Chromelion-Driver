// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    public class Pump : Device
    {
        public Pump(XElement docRoot, string id)
            : base(docRoot, id)
        {
        }
    }
}
