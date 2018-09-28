using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;

namespace Dionex.DDK.V2.ChannelTest.EditorPlugIn
{
    ///Instrument Method editor plug-in for the ChannelTest example driver. 
    ///The plug-in consists of one page which will allow the user to set a value for each property.
    /// <seealso cref="IInitEditorPlugIn"/>
    [DriverIDAttribute("MyCompany.ChannelTest")]
    public class PlugIn : IInitEditorPlugIn
    {
        #region IInitEditorPlugIn Members
        /// <seealso cref="IInitEditorPlugIn.Initialize"/>
        public void Initialize(IEditorPlugIn plugIn)
        {
            // Register the .Ready property of the device.
            // It will be added to condition clause of the wait command before injection.
            plugIn.System.InjectorSubsystem.ReadyForInjectDevices.Add(plugIn.Symbol);

            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.LcSystem);

            //Create page for detector device.
            var detectorPage = new DetectorPage(plugIn);
            IPage iDetectorPage = deviceModel.CreatePage(detectorPage, "ChannelTest Detector Settings", plugIn.Symbol);
            //Add iDetectorPage to Wizard page collection. 
            deviceModel.WizardPages.Add(iDetectorPage, WizardPageOrder.MiscDetectorPages);
            //Add detector page to Editor page collection.
            IEditorDeviceView editorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.MiscDetectorViews);
            editorView.Pages.Add(iDetectorPage);
        }
        #endregion
    }
}
