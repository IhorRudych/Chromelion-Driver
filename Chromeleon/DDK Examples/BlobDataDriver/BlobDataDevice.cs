/////////////////////////////////////////////////////////////////////////////
//
// BlobDataDevice.cs
// /////////////////
//
// Blob data driver Chromeleon DDK Code Example
//
// Device class for the Blob data driver.
// The "BlobDataDriver" example driver illustrates how Chromeleon handles embedded binary data objects 
// which are used to transport data from the instrument method editor plug-in to the driver. 
// The driver needs to define a unique ID in order to access the embedded binary data blob, which is part of the run context. 
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.BlobDataDriver
{
    /// <summary>
    /// Device class implementation
    /// </summary>
    internal class BlobDataDevice
    {
        #region Data Members
        private IDevice m_MyCmDevice;
        private IBlobDataID m_BlobDataID;
        #endregion

        /// <summary>
        /// Create the Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is a device using an embedded binary data block (blob).");

            // Let the DDK create a data block id. 
            // This id will be associated with the driver id and the symbol name of your driver instance.
            m_BlobDataID = m_MyCmDevice.CreateBlobId("MyBlobDataType");

            // Attach to TransferPreflightToRun and OnPreflightEnd in order to report the content of the blob to the audit trail.
            m_MyCmDevice.OnTransferPreflightToRun += OnTransferPfToRun;
            m_MyCmDevice.OnPreflightEnd += OnPreflightEnd;

            m_MyCmDevice.StoreAdditionalInformation("AdditionalInformation1");
            cmDDK.AuditMessage(AuditLevel.Normal, m_MyCmDevice.GetAdditionalInformation());

            m_MyCmDevice.CustomData = "Some custom data, here, a string, but could be any object";

            return m_MyCmDevice;
        }

        void OnPreflightEnd(PreflightEventArgs args)
        {
            // Report the blob content
            if (args.RunContext.IsSemanticCheck || args.RunContext.IsReadyCheck)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Message, "OnPreflightEnd handler");

                ReportBlob(args.RunContext);
            }
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize our properties.
        /// </summary>
        internal void OnConnect()
        {
            string newData = m_MyCmDevice.GetAdditionalInformation() + "_Connect";
            m_MyCmDevice.UpdateAdditionalInformation(newData);
            m_MyCmDevice.AuditMessage(AuditLevel.Normal, String.Format("AdditionalInformation Size={0:d}", m_MyCmDevice.GetAdditionalInformation().Length));
            string customData = m_MyCmDevice.CustomData as string;
            m_MyCmDevice.AuditMessage(AuditLevel.Normal, customData);
        }

        /// <summary>
        /// OnTransferPfToRun is called when the previously preflighted instrument method is actually started.
        /// </summary>
        /// <param name="args">The PreflightEventArgs contain the IRunContext with the ProgramSteps.</param>
        private void OnTransferPfToRun(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "OnTransferPreflightToRun.");

            // Report the blob content
            if (args.RunContext.IsSample)
            {
                ReportBlob(args.RunContext);
            }
        }

        /// <summary>
        /// Reports the binary data block stored in the instrument method.
        /// </summary>
        /// <param name="runContext"></param>
        private void ReportBlob(IRunContext runContext)
        {
            // Usually there should be exactly one data block with our blob id in the instrument method.
            if (runContext.IsSample)
            {
                // Check if there is more than one embedded data block with your blob id:
                var plugInData = runContext.GetBlobData(m_BlobDataID);
                if (plugInData.Count > 1)
                {
                    // More than one embedded data block found. Report an error.
                    m_MyCmDevice.AuditMessage(AuditLevel.Error,
                        string.Format("More than one data block found for block ID {0}.", m_BlobDataID));

                    return;
                }

                var binaryData = plugInData.FirstOrDefault();
                if (binaryData != null)
                {
                    // Report the content of the blob.
                    ReportBlobContent(binaryData);
                }
                else
                {
                    // No embedded data block found. Report an error.
                    m_MyCmDevice.AuditMessage(AuditLevel.Error,
                        string.Format("No data block found for block ID {0}.", m_BlobDataID));
                }
            }
        }

        /// <summary>
        /// Reports the content of the blob.
        /// </summary>
        /// <param name="data"></param>
        private void ReportBlobContent(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    var xmlDocument = XDocument.Load(reader);
                    m_MyCmDevice.AuditMessage(AuditLevel.Message, string.Format("Text = {0}", xmlDocument.Root.Element("Text").Value));
                    m_MyCmDevice.AuditMessage(AuditLevel.Message, string.Format("NumericValue = {0}", xmlDocument.Root.Element("NumericValue").Value));
                }
            }
        }

        internal string DoSomething(string foo)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Message, foo);
            return new string(foo.Reverse().ToArray());
        }
    }
}
