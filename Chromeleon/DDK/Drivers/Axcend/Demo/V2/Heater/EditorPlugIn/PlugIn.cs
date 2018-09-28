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

    [DriverID(ModuleNo.Heater)]
    public class PlugIn : IInitEditorPlugIn
    {
        void IInitEditorPlugIn.Initialize(IEditorPlugIn plugIn)
        {
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, Resources.Heater);
            IPage page = deviceModel.CreatePage(new HeaterPage(), plugIn.Symbol.Name, plugIn.Symbol);
            IEditorDeviceView view = deviceModel.EditorDeviceViews.Add(EditorViewOrder.MiscDetectorViews);
            view.Pages.Add(page);
            deviceModel.WizardPages.Add(page, WizardPageOrder.MiscDetectorPages);

            DemoCode(plugIn, page);  // Code just for demo - not necessary for this plug-in
        }

        #region Demo Code
        private static void DemoCode(IEditorPlugIn plugIn, IPage page)
        {
            IList<ISymbol> deviceNodes = plugIn.Symbol.SelectSymbols("//Device[Properties[@Name='ModelNo' and @Value='Pump']]");

            IStruct temperatureSrut = Util.Properties.GetStruct(page, "Temperature");
            double temperatureNominal = Util.Properties.GetNumericValue(temperatureSrut, "Nominal");
            double temperatureValue = Util.Properties.GetNumericValue(temperatureSrut, "Value");
            double temperatureMin = Util.Properties.GetNumericValue(temperatureSrut, "LowerLimit");
            double temperatureMax = Util.Properties.GetNumericValue(temperatureSrut, "UpperLimit");

            bool ready = Util.Properties.GetBoolValue(page, "Ready");

            foreach (ISymbol symbol in deviceNodes)
            {
                ISymbol symbolFlowStruct = symbol.Child("Flow");
                if (symbolFlowStruct != null)
                {
                    IStruct structure = symbolFlowStruct as IStruct;
                    if (structure == null)
                    {
                        string errText = "Symbol " + symbolFlowStruct.Name + " must always be of type " + typeof(IStruct).FullName + ", but is " + symbolFlowStruct.GetType().FullName;
                        throw new InvalidCastException(errText);
                    }

                    ISymbol symbolFlow = structure.Child("Value");
                    if (symbolFlow == null)
                        throw new InvalidOperationException("Could not find the property Value in structure " + symbolFlowStruct.Name);

                    double flow = (symbolFlow as INumericProperty).Value.GetValueOrDefault();
                    //              Pump_Name            Flow                          Value
                    Trace.WriteLine(symbol.Name + "." + symbolFlowStruct.Name + "." + plugIn.Symbol.Name + " = " + flow.ToString());
                }

                ISymbol symbolReady = symbol.Child(SymbolName.Ready);
                if (symbolReady != null)
                {
                    INumericProperty propertyReady = symbolReady as INumericProperty;
                    if (propertyReady != null)
                    {
                        ready = Util.GetBool(propertyReady);
                        Trace.WriteLine(propertyReady.Name + " = " + ready.ToString());
                        if (propertyReady.Writeable)
                        {
                            //              Pump_Name           Ready
                            Trace.WriteLine(symbol.Name + "." + propertyReady.Name + " is Writable");
                        }
                    }
                }
            }
        }
        #endregion
    }
}
