// Copyright 2018 Thermo Fisher Scientific Inc.
using System.Collections.Generic;
using System.Diagnostics;
using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
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
            Init_1(plugIn);
            //Init_2(plugIn);
        }

        private static void Init_1(IEditorPlugIn plugIn)
        {
            PumpHelper.AddPumpPages(plugIn);
            AddChannels(plugIn);
        }

        private static void Init_2(IEditorPlugIn plugIn)
        {
            if (plugIn.DriverID == ModuleNo.DualPump)
            {
                foreach (IDevice device in PumpHelper.GetPumpDevicesFromPumpModule(plugIn))
                {
                    InitializePump(plugIn, device);
                }
            }
            else
            {
                InitializePump(plugIn, plugIn.Symbol as IDevice);
            }
            AddChannels(plugIn);
        }

        private static void AddChannels(IEditorPlugIn plugIn)
        {
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

        private static void AddChannels(IEditorPlugIn plugIn, ISymbol device)
        {
            IEnumerable<ISymbol> channels = device.ChildrenOfType(SymbolType.Channel);
            foreach (ISymbol channel in channels)
            {
                plugIn.System.DataAcquisition.Channels.Add(channel);
            }
        }

        private static void InitializePump(IEditorPlugIn plugIn, IDevice deviceSymbol)
        {
            IPage gradientPage;
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(deviceSymbol, DeviceIcon.Pump);

            IPumpDescription description = plugIn.System.PumpSubsystem.CreatePumpDescription(deviceSymbol);
            plugIn.System.PumpSubsystem.Pumps.Add(description);

            ISymbol curveSymbol = deviceSymbol.Children["Curve"];
            if (curveSymbol != null)
            {
                PumpGradientPage gradientPageControl = new PumpGradientPage(description);
                gradientPageControl.AllowEmptyGradientCells = false;
                gradientPage = deviceModel.CreatePage(gradientPageControl, "Gradient", deviceSymbol);
            }
            else
            {
                gradientPage = deviceModel.CreatePage(new PumpGradientPlotPage(description), "Gradient", deviceSymbol);
            }

            gradientPage.Description = deviceSymbol.Name + " Gradient";
            IPage pumpPage = deviceModel.CreatePage(new PumpFlowPage(gradientPage, description), "Flow", deviceSymbol);
            pumpPage.Description = deviceSymbol.Name + " Flow";

            IEditorDeviceView view = deviceModel.EditorDeviceViews.Add(EditorViewOrder.PumpViews);
            view.Pages.Add(pumpPage);
            view.Pages.Add(gradientPage);

            deviceModel.WizardPages.Add(pumpPage, WizardPageOrder.PumpPages);
            deviceModel.WizardPages.Add(gradientPage, WizardPageOrder.PumpPages);
        }
    }
}
