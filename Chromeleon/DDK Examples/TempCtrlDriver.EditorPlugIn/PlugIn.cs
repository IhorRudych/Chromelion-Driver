using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;

namespace Dionex.DDK.V2.TempCtrlDriver.EditorPlugIn
{
    ///Create a editor plug-in for the TempCtrlDriver example driver. 
    ///The plug-in consists of one page which will allow the user to specify the temperature settings.
    ///  <seealso cref="IInitEditorPlugIn"/>
    [DriverIDAttribute("MyCompany.TempCtrlDriver")]
    public class PlugIn : IInitEditorPlugIn
    {
        #region IInitEditorPlugIn Members
        /// <seealso cref="IInitEditorPlugIn.Initialize"/>
        public void Initialize(IEditorPlugIn plugIn)
        {
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.LcSystem);
            //Create page for Simple Driver.
            var tempCtrlPage = new TempCtrlPage();
            IPage iTempCtrlPage = deviceModel.CreatePage(tempCtrlPage, "Temperature Control Settings", plugIn.Symbol);
            //Add iTempCtrlPage to Wizard page collection. Set order to OvenPages .
            deviceModel.WizardPages.Add(iTempCtrlPage, WizardPageOrder.OvenPages);
            //Add TempCtrl page to Editor page collection.
            IEditorDeviceView editorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.OvenViews);
            editorView.Pages.Add(iTempCtrlPage);
        }
        #endregion
    }
}
