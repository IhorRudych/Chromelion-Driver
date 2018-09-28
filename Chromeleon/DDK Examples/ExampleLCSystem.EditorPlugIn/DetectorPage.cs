using System.Collections.Generic;
using System.Windows.Forms;

using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn.Properties;

namespace Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class DetectorPage : UserControl, IInitPage
    {
        private readonly ISymbol m_DetectorSymbol; 
        private readonly IEditorPlugIn m_EditorPlugIn;

        /// <summary>
        /// Init a new detector page. DetectorSymbol is the symbol from the symbol table for the detector 
        /// device. 
        /// </summary>
        public DetectorPage(ISymbol detectorSymbol, IEditorPlugIn plugIn)
        {
            InitializeComponent();
            m_DetectorSymbol = detectorSymbol;
            m_EditorPlugIn = plugIn;
        }

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            //Get all channels of the detector
            if(m_DetectorSymbol!= null)
            {
                m_EditorPlugIn.System.DataAcquisition.Detectors.Add(m_DetectorSymbol);

                //For initialization we need for each channel a channel description.
                //We can use plugIn.System.DataAcquisition for creating a description for each channel.
                //In this case the detector symbol is of type IChannel.
                var channelDescriptionList = new List<IChannelDescription>();
                channelDescriptionList.AddRange(CreateChannelDescriptions(m_DetectorSymbol));

                //Create a channel grid column info for each column/property that you want to display
                //in the grid. Since this channel has only a wavelength property, only one channel grid
                //column info object is cerated. Using this approach allows you to localize the column headers.
                var channelGridColumnInfo = CreateChannelGridColumnInfo();

                //Init channel grid.
                m_ChannelGrid.AcquisitionOnEnabled = true;
                m_ChannelGrid.AcquisitionTimesVisible = true;
                m_ChannelGrid.Init(page.Component, channelDescriptionList, channelGridColumnInfo);
            }
            else
            {
                page.Enabled = false; //do not display page
            }
        }
        #endregion
        //Function will return an IEnumerable<IChannelDescription> that contains a channel description
        //for the channel passed in by parameter channel.
        private IEnumerable<IChannelDescription> CreateChannelDescriptions(ISymbol channel)
        {
            yield return m_EditorPlugIn.System.DataAcquisition.CreateChannelDescription(channel);
        }
        //Function return a list of ChannelGridColumnInfo.
        private IList<ChannelGridColumnInfo> CreateChannelGridColumnInfo()
        {
            return new List<ChannelGridColumnInfo>
                       {new ChannelGridColumnInfo("Wavelength", Resources.Detector_ColWavelength, true)};
        }
    }
}
