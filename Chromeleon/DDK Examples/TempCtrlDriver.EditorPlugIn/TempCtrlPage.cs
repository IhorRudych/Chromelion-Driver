
using System.Windows.Forms;

using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;


namespace Dionex.DDK.V2.TempCtrlDriver.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class TempCtrlPage : UserControl, IInitPage
    {
        /// <summary>
        /// Page contains only of the TemperatureControlComponent which will handle all settings and checks.
        /// </summary>
        public TempCtrlPage()
        {
            InitializeComponent();
        }

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            //Use enable controller for activating/deactivating nominal, upper limit and lower limit for
            //temperature property
            var enableCtrl = new EnableController(page.Component, m_CheckBoxTemperatureControl.Controller);
            //Add controls which should be activated or deactivated depending on check box check state
            enableCtrl.ControlledItems.AddRange(new Control[]{m_TextBoxTemperatureNominal, m_TextBoxTemperatureLowerLimit, m_TextBoxTemperatureUpperLimit});

            //Create a new MinMaxNominalController which will handle the check for the property values. 
            //Executed checks: lowerLimit <= nominal <= upperLimit
            //                 lowerLimit <= upperLimit
            new MinMaxNominalController(page.Component,page.Component.Symbol,
                m_TextBoxTemperatureLowerLimit.Controller,
                m_TextBoxTemperatureUpperLimit.Controller,
                m_TextBoxTemperatureNominal.Controller);
        }
        #endregion
    }
}
