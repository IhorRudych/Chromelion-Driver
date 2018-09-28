using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;

namespace Dionex.DDK.V2.TimeTableDriver.EditorPlugIn
{
    ///Create a editor plug-in for the TempCtrlDriver example driver. 
    ///The plug-in consists of one page which will allow the user to specify the temperature settings.
    ///  <seealso cref="IInitEditorPlugIn"/>
    [DriverIDAttribute("MyCompany.TimeTableDriver")]
    public class PlugIn : IInitEditorPlugIn
    {
        #region IInitEditorPlugIn Members
        /// <seealso cref="IInitEditorPlugIn.Initialize"/>
        public void Initialize(IEditorPlugIn plugIn)
        {
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.Pump);

            // Create gradient page for TimeTableDriver.
            //Information like flow symbol and solvent symbols are stored in IPumpDescription.
            //See CM7 DDK V2 help for further information.
            IPumpDescription pumpDescription = plugIn.System.PumpSubsystem.CreatePumpDescription(plugIn.Symbol as IDevice);
            plugIn.System.PumpSubsystem.Pumps.Add(pumpDescription);
            IPage iGradientPage = deviceModel.CreatePage(new PumpGradientPage(pumpDescription), "Gradient Settings", plugIn.Symbol);
            //Add iGradientPage to Wizard page collection. Set order to OvenPages .
            deviceModel.WizardPages.Add(iGradientPage, WizardPageOrder.PumpPages);
            //Add iGradientPage page to Editor page collection.
            IEditorDeviceView deviceEditorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.PumpViews);
            deviceEditorView.Pages.Add(iGradientPage);

            // Create time grid page for TimeTableDriver.
            IPage iValvePage = deviceModel.CreatePage(new TimeGridPage(), "Valve Settings", plugIn.Symbol);
            //Add iTempCtrlPage to Wizard page collection. Set order to OvenPages .
            deviceModel.WizardPages.Add(iValvePage, WizardPageOrder.PumpPages);
            //Add TempCtrl page to Editor page collection.
            deviceEditorView.Pages.Add(iValvePage);
        }
        #endregion
    }
}
