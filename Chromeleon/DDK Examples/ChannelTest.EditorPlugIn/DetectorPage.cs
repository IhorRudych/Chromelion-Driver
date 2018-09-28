using System.Collections.Generic;
using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.Chromeleon.DDK.V2.Symbols;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.ChannelTest.EditorPlugIn.Properties;
using System;

namespace Dionex.DDK.V2.ChannelTest.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class DetectorPage : UserControl, IInitPage
    {
        //Create internal class for storing the names of the used properties (name of a symbol
        //from the symbol table).
        internal static class PropertyNames
        {
            public const string RefWavelength = "RefWavelength";
            public const string BunchWidth = "BunchWidth";
            public const string ThreeDFieldMinWavelength = "MinWavelength";
            public const string ThreeDFieldMaxWavelength = "MaxWavelength";
            public const string FixedRate = "FixedRate";
        }

        #region Data Fields
        private readonly IEditorPlugIn m_EditorPlugIn;
        #endregion

        #region Construction
        /// <summary>
        /// Create a new detector page. The page will display up to two channel grids. One grid is for
        /// 2D channels only and will have two columns for the properties FixedDataRate and PacketSize. The
        /// other grid will display the 3D field is available with all wavelength and bunchwidth properties.
        /// </summary>
        public DetectorPage(IEditorPlugIn plugIn)
        {
            InitializeComponent();
            m_EditorPlugIn = plugIn;
        }
        #endregion

        #region IInitPage Implementation
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            // initialize page content
            Fill2DChannelGrid(page.Component);
            Fill3DChannelGrid(page.Component);

            // page events
            page.OnRequestClipboardOperation += page_OnRequestClipboardOperation;
            page.ShowHelp += page_ShowHelp;

            // component events
            page.Component.DeviceViewSessionEnterEvent += Component_DeviceViewSessionEnterEvent;
            page.Component.DeviceViewSessionLeaveEvent += Component_DeviceViewSessionLeaveEvent;
            page.Component.EnabledChanged += Component_EnabledChanged;
            page.Component.IsActiveChanged += Component_IsActiveChanged;
            page.Component.WizardFinishEvent += Component_WizardFinishEvent;
            page.Component.PageEnterEvent += new System.EventHandler<PageEnterArgs>(Component_PageEnterEvent);
            page.Component.PageValidationEvent += new System.EventHandler<PageValidationArgs>(Component_PageValidationEvent);
            page.Component.PageLeaveEvent += new System.EventHandler<PageLeaveArgs>(Component_PageLeaveEvent);

            // control events for validation
            m_ChannelGrid3D.PropertyValueChanged += new System.EventHandler<ChannelGridPropertyChangedArgs>(ChannelGrid3D_PropertyValueChanged);
        }
        #endregion

        #region Private initialization functions
        private void Fill2DChannelGrid(IEditorComponent comp)
        {
            //Get all 2D channels of the detector
            var channels2D = Get2DChannels();
            if (channels2D != null && channels2D.Count > 0)
            {
                //For initialization we need for each channel a channel description.
                //We can use plugIn.System.DataAcquisition for creating a description for each channel.
                var channelDescriptionList = new List<IChannelDescription>();
                channelDescriptionList.AddRange(CreateChannelDescriptions(channels2D));

                //Create a channel grid column info for each column that you want to display
                //in the grid. Since these channels have a FixedRate property, one channel grid
                //column info object is created. Using this approach allows you to localize the column headers.
                var channelGridColumnInfo = CreateChannelGridColumnInfo();

                //Now init channel grid.
                m_ChannelGrid2D.AcquisitionOnEnabled = true;
                m_ChannelGrid2D.AcquisitionTimesVisible = true;
                m_ChannelGrid2D.Init(comp, channelDescriptionList, channelGridColumnInfo);
            }
            else
            {
                m_ChannelGrid2D.Enabled = false; //No 2D channels were found, so disable grid
            }
        }

        private void Fill3DChannelGrid(IEditorComponent comp)
        {
            //Get all 3D channels of the detector
            var channels3D = Get3DChannels();
            if (channels3D != null && channels3D.Count > 0)
            {
                //For initialization we need for each channel a channel description.
                //We can use plugIn.System.DataAcquisition for creating a description for each channel.
                var channelDescriptionList = new List<IChannelDescription>();
                channelDescriptionList.AddRange(CreateChannelDescriptions(channels3D));

                //Create a channel grid column info for each column/property that you want to display
                //in the grid. 
                var channelGridColumnInfo = Create3DChannelGridColumnInfo();

                //Init channel grid.
                m_ChannelGrid3D.AcquisitionOnEnabled = false;
                m_ChannelGrid3D.AcquisitionTimesVisible = false;
                m_ChannelGrid3D.Init(comp, channelDescriptionList, channelGridColumnInfo);
            }
            else
            {
                m_ChannelGrid3D.Enabled = false; //No 3d channels were found, so disable grid
            }
        }

        //Function will return an IEnumerable<IChannelDescription> that contains a channel description
        //for each channel passed in by parameter channels.
        private IEnumerable<IChannelDescription> CreateChannelDescriptions(IEnumerable<ISymbol> channels)
        {
            foreach (var channel in channels)
            {
                yield return m_EditorPlugIn.System.DataAcquisition.CreateChannelDescription(channel);
            }
        }

        //Function return a list of ChannelGridColumnInfo.
        private IList<ChannelGridColumnInfo> CreateChannelGridColumnInfo()
        {
             return new List<ChannelGridColumnInfo>
                        {
                            //Create for each column (property) a new ChannelGridColumnInfo. If you do not want to 
                            //localize the column headers and do not care about displaying physical units you can 
                            //simply call ChannelGridColumnInfo(PropertyNames.FixedRate).
                            new ChannelGridColumnInfo(PropertyNames.FixedRate, Resources.Channel2D_ColFixedRate, true),
                        };
        }

        //Function return a list of ChannelGridColumnInfo.
        private IList<ChannelGridColumnInfo> Create3DChannelGridColumnInfo()
        {
            return new List<ChannelGridColumnInfo>
                        {
                            new ChannelGridColumnInfo(PropertyNames.FixedRate, Resources.Channel2D_ColFixedRate, true),
                            new ChannelGridColumnInfo(PropertyNames.ThreeDFieldMinWavelength, Resources.Channel3D_MinWaveLength, true),
                            new ChannelGridColumnInfo(PropertyNames.ThreeDFieldMaxWavelength, Resources.Channel3D_MaxWaveLength, true),
                            new ChannelGridColumnInfo(PropertyNames.BunchWidth, Resources.Channel3D_BunchWidth, true),
                            new ChannelGridColumnInfo(PropertyNames.RefWavelength, Resources.Channel3D_RefWaveLength, true),
                        };
        }

        //Return a list of all 2D channels for this device, which are of type Channel an have the properties FixedRate
        //and PacketSize.
        private IList<ISymbol> Get2DChannels()
        {
            var list = new List<ISymbol>(3);
            //iterate over all channel symbols
            foreach (ISymbol symbol in m_EditorPlugIn.Symbol.ChildrenOfType(SymbolType.Channel))
            {
                // get only 2D Channels. 2D Channels have NO BunchWidth property
                if (symbol.Child(PropertyNames.BunchWidth) == null)
                     list.Add(symbol);
            }
            return list;
        }

        //Return a list of all 3D channels for this device. A 3D channel is a symbl of type channel with an
        //child symbol named BunchWidth.
        private IList<ISymbol> Get3DChannels()
        {
            var list = new List<ISymbol>(1);
            //iterate over all channel symbols
            foreach (ISymbol symbol in m_EditorPlugIn.Symbol.ChildrenOfType(SymbolType.Channel))
            {
                //Get only the 3D channels. 3D Channels have a BunchWidth property 
                if (symbol.Child(PropertyNames.BunchWidth) != null)
                    list.Add(symbol);
            }
            return list;
        }
        #endregion

        #region Page events
        private void page_ShowHelp(object sender, System.ComponentModel.HandledEventArgs e)
        {
        }

        private void page_OnRequestClipboardOperation(object sender, RequestClipboardOperationEventArgs e)
        {
        }
        #endregion

        #region Component events

        private void Component_WizardFinishEvent(object sender, EventArgs e)
        {
        }

        private void Component_IsActiveChanged(object sender, ActiveChangedEventArgs e)
        {
        }

        private void Component_EnabledChanged(object sender, EnabledChangedEventArgs e)
        {
        }

        private void Component_DeviceViewSessionLeaveEvent(object sender, SessionEventArgs e)
        {
        }

        private void Component_DeviceViewSessionEnterEvent(object sender, SessionEventArgs e)
        {
        }

        private void Component_PageValidationEvent(object sender, PageValidationArgs e)
        {
        }

        private void Component_PageLeaveEvent(object sender, PageLeaveArgs e)
        {
        }

        void Component_PageEnterEvent(object sender, PageEnterArgs e)
        {
            if (e.Context == PageEventContext.Editor)
            {
                // Check wavelength limits when entering the page in editor mode 
                // Values might have be changed in script view
                for (int channelIndex = 0; channelIndex < m_ChannelGrid3D.Channels.Count; channelIndex++)
                {
                    CheckWavelengthLimits(channelIndex);
                }
                m_ChannelGrid3D.Refresh();
            }
        }
        #endregion

        #region Validation
        private void ChannelGrid3D_PropertyValueChanged(object sender, ChannelGridPropertyChangedArgs e)
        {
            if ((e.PropertyName == PropertyNames.ThreeDFieldMinWavelength) || (e.PropertyName == PropertyNames.ThreeDFieldMaxWavelength))
            {
                CheckWavelengthLimits(e.ChannelIndex);
            }
        }

        private void CheckWavelengthLimits(int channelIndex)
        {
            // Validation: Max. wavelength must be greater than min. wavelength
            INumericSymbolValue symbolValueMin = m_ChannelGrid3D.GetSymbolValue(PropertyNames.ThreeDFieldMinWavelength, channelIndex) as INumericSymbolValue;
            INumericSymbolValue symbolValueMax = m_ChannelGrid3D.GetSymbolValue(PropertyNames.ThreeDFieldMaxWavelength, channelIndex) as INumericSymbolValue;
            if ((symbolValueMin != null) && (symbolValueMin.Value.HasValue)
             && (symbolValueMax != null) && (symbolValueMax.Value.HasValue))
            {
                // both values are assigned in method - do check 
                if (symbolValueMin.Value.Value > symbolValueMax.Value.Value)
                {
                    // validation error, set error 
                    symbolValueMin.ConstraintError.SetError(Resources.Error_Invalid3DWavelengthLimits);
                    symbolValueMax.ConstraintError.SetError(Resources.Error_Invalid3DWavelengthLimits);
                    return;
                }
            }

            // no validation error, reset previous error 
            if ((symbolValueMin != null) && symbolValueMin.ConstraintError.HasError)
                symbolValueMin.ConstraintError.ResetError();
            if ((symbolValueMax != null) && symbolValueMax.ConstraintError.HasError)
                symbolValueMax.ConstraintError.ResetError();
        }
        #endregion
    }
}
