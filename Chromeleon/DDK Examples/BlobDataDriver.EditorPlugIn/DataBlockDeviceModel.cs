using System.Diagnostics;

using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.DataBlocks;

namespace Dionex.DDK.V2.BlobDataDriver.EditorPlugIn
{
    /// <summary>
    /// A model for a device data block.
    /// Serves as binding element between the data and the plug-in pages.
    /// 
    /// Handles loading and saving of data from / to the instrument method.
    /// </summary>
    public class DataBlockDeviceModel
    {
        #region Fields
        private readonly IDeviceModel m_DeviceModel;
        private readonly PlugInDataBlockId m_DataBlockId;
        private bool m_IsDataModified;
        private BlockData m_BlockData;
        #endregion

        #region Construction
        public DataBlockDeviceModel(IDeviceModel deviceModel, PlugInDataBlockId dataBlockId)
        {
            m_DeviceModel = deviceModel;
            m_DataBlockId = dataBlockId;

            ReadFromDataBlock();

            deviceModel.Component.DeviceViewSessionLeaveEvent += OnDeviceViewSessionLeave;
        }
        #endregion

        #region Event Handlers

        private void OnDeviceViewSessionLeave(object sender, SessionEventArgs e)
        {
            WriteDataBlock();
        }

        #endregion

        #region Public Members

        public string DeviceName
        {
            get { return m_DeviceModel.PlugIn.Symbol.Name; }
        }

        /// <summary>
        /// Gets / sets a text value.
        /// </summary>
        public string Text
        {
            get
            {
                return m_BlockData.Text;
            }
            set
            {
                if (m_BlockData.Text != value)
                {
                    m_DeviceModel.Component.EditMethod.SetModified();
                    m_IsDataModified = true;
                    m_BlockData.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets / sets a numeric value.
        /// </summary>
        public double NumericValue
        {
            get
            {
                return m_BlockData.NumericValue;
            }
            set
            {
                if (m_BlockData.NumericValue != value)
                {
                    m_DeviceModel.Component.EditMethod.SetModified();
                    m_IsDataModified = true;
                    m_BlockData.NumericValue = value;
                }
            }
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Reads the data from the instrument method.
        /// If no data can be found a new default data block will be generated.
        /// </summary>
        private void ReadFromDataBlock()
        {
            var block = m_DeviceModel.Component.EditMethod.PlugInDataBlocks.FindData(m_DataBlockId);
            if (block != null)
            {
                m_BlockData = new BlockData(block.BinaryData);
            }
            else
            {
                Debug.Assert(m_DeviceModel.Component.EditMethod.Mode == EditMode.Wizard);
                m_BlockData = new BlockData();
                m_IsDataModified = true;
            }
        }

        /// <summary>
        /// Writes modified data to the instrument method.
        /// </summary>
        private void WriteDataBlock()
        {
            if (m_BlockData != null && m_IsDataModified)
            {
                m_DeviceModel.Component.EditMethod.PlugInDataBlocks.SetData(
                    m_DataBlockId, m_BlockData.Serialize(), m_BlockData.CreateFormattedData(false), "BlobData", "BlobDataDriver");

                m_IsDataModified = false;
            }
        }

        #endregion
    }
}
