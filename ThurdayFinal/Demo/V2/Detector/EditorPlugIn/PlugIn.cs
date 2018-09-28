// Copyright 2018 Thermo Fisher Scientific Inc.
using System.Diagnostics;
using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;

namespace MyCompany.Demo.Detector.EditorPlugIn
{
    internal static class ModuleNo
    {
        public const string Detector = "Detector";
    }

    [DriverID(ModuleNo.Detector)]
    public class PlugIn : IInitEditorPlugIn
    {
        private IDetector m_Detector;

        void IInitEditorPlugIn.Initialize(IEditorPlugIn plugIn)
        {
            m_Detector = plugIn.System.DataAcquisition.Detectors.Add(plugIn.Symbol, true);

            foreach (ISymbol symbol in plugIn.Symbol.ChildrenOfType(SymbolType.Channel))
            {
                //if (symbol.AuditLevel <= AuditLevel.Advanced)
                //{
                //    continue;
                //}

                m_Detector.Channels.Add(symbol);
            }

            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.GenericDetector);
            IPage page = deviceModel.CreatePage(new DetectorPage(m_Detector), plugIn.Symbol.Name, plugIn.Symbol);
            IEditorDeviceView view = deviceModel.EditorDeviceViews.Add(EditorViewOrder.CDDetectorViews);
            view.Pages.Add(page);
            deviceModel.WizardPages.Add(page, WizardPageOrder.CDDetectorPages);
        }
    }
}
