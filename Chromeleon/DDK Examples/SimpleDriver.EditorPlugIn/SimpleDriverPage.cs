
using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;


namespace Dionex.DDK.V2.SimpleDriver.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class SimpleDriverPage : UserControl, IInitPage
    {
        /// <summary>
        /// Page contains one text box for defining a value for the double property. Another text box
        /// for the int property, which is readonly. Therefore the text box is enabled.
        /// For all properties that have legal values a combo box is presented.
        /// </summary>
        public SimpleDriverPage()
        {
            InitializeComponent();
        }

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            //Nothing to do here. The symbol path for the data binding of the controls
            //is defined in the designer (see SymbolPath property).
        }
        #endregion
    }
}
