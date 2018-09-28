using System.Linq;
using Dionex.Chromeleon.DDK.V2.Driver;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;

namespace Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn
{
    /// Plug-in will create several pages for each device one.
    /// All pages will be visible in the wizard and in the editor. The pages use the same icon called
    /// LcSystem.
    /// <seealso cref="IInitEditorPlugIn"/>
    [DriverIDAttribute("MyCompany.ExampleLCSystem")]
    public class PlugIn : IInitEditorPlugIn
    {
        #region IInitEditorPlugIn Members
        /// <seealso cref="IInitEditorPlugIn.Initialize"/>
        public void Initialize(IEditorPlugIn plugIn)
        {
            IDeviceModel deviceModel = plugIn.DeviceModels.Add(plugIn.Symbol, DeviceIcon.LcSystem);

            IEditorDeviceView editorView = null; 

            // We would like to have a page for each device. Therefore the symbol for each device needs
            // to be identified. 

            //Find the pump device symbol
            var pumpDeviceSymbol = FindPump(plugIn.Symbol);
            if (pumpDeviceSymbol != null)
            {
                //Create pages for pump device.
                var pumpPage = new PumpGeneralPage();
                IPage iPumpPage = deviceModel.CreatePage(pumpPage, "LC System Pump Settings", pumpDeviceSymbol);
                //Add iPumpPage to Wizard page collection. Set order for pump.
                deviceModel.WizardPages.Add(iPumpPage, WizardPageOrder.PumpPages);
                //Create and add pump page to Editor page collection.
                editorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.LCSystemViews);
                editorView.Pages.Add(iPumpPage);

                //Add pump gradient page.

                //The IPumpDescription object holds all relevant information of the pump which are needed by the component PumpGradientPage.
                //Information like flow symbol and solvent symbols are stored in IPumpDescription.
                //Notice: for using the IPumpDescription the pump symbol and its child need a defined structure.
                //See CM7 DDK V2 help for further information.
                IPumpDescription pumpDescription = plugIn.System.PumpSubsystem.CreatePumpDescription(pumpDeviceSymbol as IDevice);
                //Inform the system that this page handles a pump.
                plugIn.System.PumpSubsystem.Pumps.Add(pumpDescription);
                //Now create the gradient page which consists of a plot component for displaying the flow and solvent gradients
                //and a grid for defining time actions.

                iPumpPage = deviceModel.CreatePage(new PumpGradientPage(pumpDescription), "LC System Pump Gradient Settings", pumpDeviceSymbol);
                //Add iPumpPage to Wizard page collection. Set order for pump.
                deviceModel.WizardPages.Add(iPumpPage, WizardPageOrder.PumpPages);
                //Add pump page to Editor page collection.
                editorView.Pages.Add(iPumpPage);   
            }

            //Find the detector device symbol
            var detectorDeviceSymbol = FindDetector(plugIn.Symbol);
            if (detectorDeviceSymbol != null)
            {
                //Create page for detector device.
                var detectorPage = new DetectorPage(detectorDeviceSymbol, plugIn);
                IPage iDetectorPage = deviceModel.CreatePage(detectorPage, "LC System Detector Settings", detectorDeviceSymbol);
                //Add iDetectorPage to Wizard page collection. Set order for UV VIS detectors.
                deviceModel.WizardPages.Add(iDetectorPage, WizardPageOrder.UVDetectorPages);
                //Create and add detector page to Editor page collection.
                if (editorView == null)
                    editorView = deviceModel.EditorDeviceViews.Add(EditorViewOrder.LCSystemViews);
                editorView.Pages.Add(iDetectorPage);
            }

            //Since there are no settings for the sampler, we do not create a dedicated page.
            //Nevertheless an inject command and a Wait.Ready statement must be created.
            //This will be done by using the BasicInjectorComponent.
            var samplerDevice = FindSampler(plugIn.Symbol);
            if(samplerDevice != null)
            {
                new BasicInjectorComponent(deviceModel, samplerDevice);
            }
        }

        #endregion

        private ISymbol FindPump(ISymbol mainDev)
        {
            //At first get all symbols of type IDevice.
            var devices = mainDev.ChildrenOfType(SymbolType.Device);
            if (devices != null)
            {
                //A pump is a device with a symbol called Flow and %A
                return devices.FirstOrDefault(elem => elem.Child("Flow") != null && elem.Child("%A")!=null);
            }
            return null;
        }

        private ISymbol FindSampler(ISymbol mainDev)
        {
            //At first get all symbols of type IDevice.
            var devices = mainDev.ChildrenOfType(SymbolType.Device);
            if (devices != null)
            {
                //A sampler is a device with an inject command
                return devices.FirstOrDefault(elem => elem.Child("Inject") != null);
            }
            return null;
        }

        private ISymbol FindDetector(ISymbol mainDev)
        {
           //The detector is a symbol with a child named Wavelength
            if (mainDev != null)
            {
                //A detector is a device with a child that has a wavelength attribute
                return mainDev.Children.FirstOrDefault(elem => elem.Child("Wavelength") != null);
            }
            return null;
        }
    }
}
