using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;


namespace Dionex.DDK.V2.SimpleDriver.EditorPlugIn
{
    ///Create a editor plug-in for the Simple Driver example. 
    ///The plug-in consists of one page which will allow the user to set a value for each property.
    ///  <seealso cref="IInitEditorPlugIn"/>
    [DriverIDAttribute("MyCompany.SimpleDriver")]
    public class PlugIn : IInitEditorPlugIn
    {
        #region IInitEditorPlugIn Members
        /// <seealso cref="IInitEditorPlugIn.Initialize"/>
        public void Initialize(IEditorPlugIn plugIn)
        {
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.LcSystem);
            //Create page for Simple Driver.
            var simpleDriverPage = new SimpleDriverPage();
            IPage iSimpleDriverPage = deviceModel.CreatePage(simpleDriverPage, "Simple Driver Settings", plugIn.Symbol);
            //Add iSimpleDriverPage to Wizard page collection. Set order to LcSystemPages .
            deviceModel.WizardPages.Add(iSimpleDriverPage, WizardPageOrder.LCSystemPages);
            //Add Simple Driver page to Editor page collection.
            IEditorDeviceView editorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.LCSystemViews);
            editorView.Pages.Add(iSimpleDriverPage);
        }
        #endregion
    }
}
