using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn.Properties;

namespace Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class PumpGeneralPage : UserControl, IInitPage
    {
        /// <summary>
        /// The PumpGeneralPage handles the settings for the lower and upper pressure limit.
        /// Two PropertyStepTextBoxes and two PropertyStepRangeLables are used. The needed symbol path for
        /// the properties are defined in the designer. These components will handle the creation and delete 
        /// of the method steps.
        /// </summary>
        public PumpGeneralPage()
        {
            InitializeComponent();
        }

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            //Use a BinaryConstraintController for checking whether the lower pressure limit is <= the upper pressure limit.
            var pressureLimitChecker = new BinaryConstraintController(page.Component, page.Component.Symbol, m_TextBoxPressureLowerLimit.Controller, m_TextBoxPressureUpperLimit.Controller);

            pressureLimitChecker.Constraints.Add(
                new BinaryConstraint(BinaryConstraintController.LowerOrEqualConstraint, Resources.InvalidPressureLimits));
        }

        #endregion
    }
}
