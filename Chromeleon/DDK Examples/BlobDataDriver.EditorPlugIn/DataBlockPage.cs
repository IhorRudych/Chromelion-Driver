using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using System.Diagnostics;
using System.Globalization;

namespace Dionex.DDK.V2.BlobDataDriver.EditorPlugIn
{
    /// <summary>
    /// The page displays rich text box for entering text. This text will be stored as data block in 
    /// the Chromeleon instrument method.
    /// </summary>
    public partial class DataBlockPage : UserControl, IInitPage
    {
        #region Construction
        public DataBlockPage()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Create a new isntance of the data block page.
        /// </summary>
        public DataBlockPage(DataBlockDeviceModel deviceModel)
            : this()
        {
            DeviceModel = deviceModel;
        }
        #endregion

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            if (DeviceModel.Text != null)
            {
                m_TextBoxData.Text = DeviceModel.Text;
            }
            else
            {
                Debug.Fail("Invalid block data");
            }

            m_DeviceName.Text = DeviceModel.DeviceName;
            m_TextBoxNumericalValue.Text = DeviceModel.NumericValue.ToString(NumberFormatInfo.CurrentInfo);

            m_TextBoxData.TextChanged += OnTextBoxDataChanged;
            m_TextBoxNumericalValue.TextChanged += OnTextBoxNumericalValueChanged;
        }

        #endregion

        public DataBlockDeviceModel DeviceModel
        {
            get;
            private set;
        }

        #region Event Handlers

        private void OnTextBoxDataChanged(object sender, System.EventArgs e)
        {
            DeviceModel.Text = m_TextBoxData.Text;
        }

        private void OnTextBoxNumericalValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                DeviceModel.NumericValue = double.Parse(m_TextBoxNumericalValue.Text, NumberFormatInfo.CurrentInfo);
            }
            catch
            {
                // TODO: perform some error handling.
            }
        }

        #endregion
    }
}
