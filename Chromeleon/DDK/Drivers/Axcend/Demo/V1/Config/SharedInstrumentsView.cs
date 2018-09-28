// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using MyCompany.Demo.Config.Properties;
using Dionex.Chromeleon.DDK;

namespace MyCompany.Demo.Config
{
    #region How to use
    /*
    public partial class MainForm : Form, IConfigurationPlugin, IConfigurationDescriptors, IConfigurationPluginSharedInstruments
    {
        private SharedInstrumentsControl m_SharedInstrumentsControl;

        private IInstrumentInfo m_InstrumentInfo;

        #region IConfigurationPluginSharedInstruments
        // All instruments IDs the driver is attached to
        // If AttachedTo = 0000 0000 0000 0011, then the driver is attached to 2 instruments with IDs 0001 and 0010
        long IConfigurationPluginSharedInstruments.AttachedTo
        {
            get { return m_SharedInstrumentsControl.InstrumentsMap; }
            set
            {
                if (m_SharedInstrumentsControl.InstrumentsMap == value)
                {
                    return;
                }
                m_SharedInstrumentsControl.InstrumentsMap = value;
            }
        }

        IInstrumentInfo IConfigurationPluginSharedInstruments.InstrumentConfiguration
        {
            set
            {
                if (m_InstrumentInfo == value)
                {
                    return;
                }
                m_InstrumentInfo = value;
                m_SharedInstrumentsControl.Init(m_InstrumentInfo);
            }
        }
        #endregion

        private void OnOK(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                m_SharedInstrumentsControl.ValidateData();

                if (!m_GeneralPage.SaveGeneralPage())
                {
                    return;
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
    */
    #endregion

    internal partial class SharedInstrumentsView : UserControl
    {
        private IInstrumentInfo m_InstrumentInfo;

        private long m_InstrumentsMap;

        private int m_RequiredMinInstruments;
        private int m_RequiredMaxInstruments;

        private readonly InstrumentDataList m_Instruments = new InstrumentDataList();

        /// <summary>Initializes a new instance of the <see cref="SharedInstrumentsView"/> class.</summary>
        public SharedInstrumentsView()
        {
            InitializeComponent();
            Load += UserControlLoad;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long InstrumentsMap
        {
            [DebuggerStepThrough]
            get { return m_InstrumentsMap; }
            set
            {
                if (m_InstrumentsMap == value)
                {
                    return;
                }

                m_InstrumentsMap = value;

                if (m_InstrumentInfo != null)
                {
                    UpdateSelected();
                }
            }
        }

        public void Init(IInstrumentInfo instrumentInfo, string userText = null, int minInstruments = 0, int maxInstruments = 2)
        {
            if (instrumentInfo == null)
                throw new ArgumentNullException("instrumentInfo");
            if (minInstruments < 0)
                throw new ArgumentException("minInstruments cannot be < 0");
            if (maxInstruments > InstrumentData.MaxInstrumentCount)
                throw new ArgumentException("maxInstruments cannot be > " + InstrumentData.MaxInstrumentCount.ToString());

            m_InstrumentInfo = instrumentInfo;
            m_LabelUserText.Text = userText;
            m_RequiredMinInstruments = minInstruments;
            m_RequiredMaxInstruments = maxInstruments;

            m_Instruments.Init(m_InstrumentInfo);
        }

        private void UserControlLoad(object sender, EventArgs e)
        {
            if (m_InstrumentInfo == null)
            {
                return;
            }

            // Add all instruments to m_ListViewInstruments and check the selected ones
            m_ListViewInstruments.ItemChecked -= OnListViewInstrumentsItemChecked;
            try
            {
                m_ListViewInstruments.Items.Clear();
                foreach (InstrumentData instrument in m_Instruments)
                {
                    ListViewItem item = new ListViewItem(instrument.Name);
                    item.Tag = instrument;
                    m_ListViewInstruments.Items.Add(item);
                    item.Checked = (m_InstrumentsMap & instrument.BitMap) != 0;
                }
            }
            finally
            {
                m_ListViewInstruments.ItemChecked += OnListViewInstrumentsItemChecked;
            }
        }

        private void UpdateSelected()
        {
            m_ListViewInstruments.ItemChecked -= OnListViewInstrumentsItemChecked;
            try
            {
                foreach (ListViewItem item in m_ListViewInstruments.Items)
                {
                    InstrumentData instrument = item.Tag as InstrumentData;
                    item.Checked = (m_InstrumentsMap & instrument.BitMap) != 0;
                }
            }
            finally
            {
                m_ListViewInstruments.ItemChecked += OnListViewInstrumentsItemChecked;
            }
        }

        private void OnListViewInstrumentsItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (m_ListViewInstruments.CheckedItems.Count > m_RequiredMaxInstruments)
            {
                e.Item.Checked = false;
                //                                                      The maximum number of shared instruments is {0}.
                string text = string.Format(CultureInfo.CurrentCulture, Resources.MaximumNumberOfSharedInstruments, m_RequiredMaxInstruments);
                MessageBox.Show(text, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long selectedInstrumentsMap = 0;
            foreach (ListViewItem item in m_ListViewInstruments.CheckedItems)
            {
                InstrumentData instrument = item.Tag as InstrumentData;
                if (instrument == null)
                    throw new InvalidOperationException("ListViewItem item.Tag type " + (item.Tag == null ? "Null" : item.Tag.GetType().FullName) + " is not " + typeof(InstrumentData).FullName);

                selectedInstrumentsMap |= instrument.BitMap;
            }
            m_InstrumentsMap = selectedInstrumentsMap;
        }

        public void ValidateData()
        {
            InstrumentDataList instruments = new InstrumentDataList(m_InstrumentInfo, m_InstrumentsMap);

            if (m_RequiredMinInstruments > 0 && instruments.Count == 0)    // At least {0} instruments must be selected.
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.MinInstrumentsRequiredAreNotSelected, m_RequiredMinInstruments));

            if (m_RequiredMaxInstruments > 0 && instruments.Count > m_RequiredMaxInstruments)  // More than the maximum of {0} instruments are selected.
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.MoreThanMaxInstrumentsAreSelected, m_RequiredMaxInstruments));
        }
    }
}
