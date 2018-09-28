// Copyright 2018 Thermo Fisher Scientific Inc.
using System.Collections.Generic;
using System.Diagnostics;
using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.CommonPages;

namespace MyCompany.Demo.Pump.EditorPlugIn
{
    internal static class ModuleNo
    {
        public const string Pump = "Pump";
        public const string DualPump = "DualPump";
    }

    [DriverID(ModuleNo.Pump)]
    [DriverID(ModuleNo.DualPump)]
    public class PlugIn : IInitEditorPlugIn
    {
        void IInitEditorPlugIn.Initialize(IEditorPlugIn plugIn)
        {
            PumpHelper.AddPumpPages(plugIn);

            if (plugIn.DriverID == ModuleNo.DualPump)
            {
                bool isChannelFound = false;
                foreach (IDevice device in PumpHelper.GetPumpDevicesFromPumpModule(plugIn))
                {
                    AddChannels(plugIn, device);
                    isChannelFound = true;
                }

                if (!isChannelFound) // when the dual pump is configured as a shared device
                    AddChannels(plugIn, plugIn.Symbol);
            }
            else
            {
                AddChannels(plugIn, plugIn.Symbol);
            }
        }

        private void AddChannels(IEditorPlugIn plugIn, ISymbol device)
        {
            IEnumerable<ISymbol> channels = device.ChildrenOfType(SymbolType.Channel);
            foreach (ISymbol channel in channels)
            {
                plugIn.System.DataAcquisition.Channels.Add(channel);
            }
        }
    }
}
