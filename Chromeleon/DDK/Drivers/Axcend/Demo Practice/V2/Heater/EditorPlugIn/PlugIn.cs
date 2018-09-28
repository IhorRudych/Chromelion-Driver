// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using MyCompany.Demo.Heater.EditorPlugIn.Properties;

namespace MyCompany.Demo.Heater.EditorPlugIn
{
    internal static class ModuleNo
    {
        public const string Heater = "Heater";
    }

    //[DriverID(ModuleNo.Heater)]
    public class PlugIn //: IInitEditorPlugIn
    {
        public void Initialize(IEditorPlugIn plugIn)
        {
        }
    }
}
