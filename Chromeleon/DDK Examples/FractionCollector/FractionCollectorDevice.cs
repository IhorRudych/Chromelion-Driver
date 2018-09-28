/////////////////////////////////////////////////////////////////////////////
//
// FractionCollectorDevice.cs
// /////////////////////////
//
// FractionCollector Chromeleon DDK Code Example
//
// Device class for the FractionCollector.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;
using System.Globalization;				// Chromeleon Symbol Interface

namespace MyCompany.FractionCollector
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class FractionCollectorDevice
    {
        #region Data Members

        /// Our IFractionCollection
        private IFractionCollection m_MyCmDevice;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device. Note that all fraction collection settings are provided under a separate device which is always named "FractionCollection".</param>
        /// <param name="maximumNumberOfDetectionChannels">The maximum number of dectection channels</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name, int maximumNumberOfDetectionChannels)
        {
            // Create our Dionex.Chromeleon.Symbols.IFractionCollection
            m_MyCmDevice = cmDDK.CreateFractionCollection(name, "FractionCollection.", false, maximumNumberOfDetectionChannels);

            m_MyCmDevice.OnGetCurrentTubeNumberAsPositionString += new FractionCollectionEventHandlerS(OnGetCurrentTubeNumberAsPositionString);
            m_MyCmDevice.OnStartCollect += new FractionCollectionEventHandler(OnStartCollect);
            m_MyCmDevice.OnSwitchTube += new FractionCollectionEventHandler(OnSwitchTube);
            m_MyCmDevice.OnEndCollect += new FractionCollectionEventHandler(OnEndCollect);

            ICommand commandGenerateAuditTrail = m_MyCmDevice.CreateCommand("GenerateAuditTrail", "Writes some example messages to the audit trail");
            commandGenerateAuditTrail.OnCommand += new CommandEventHandler(OnCommand_GenerateAuditTrail);

            ICommand commandSetTotalNumberOnRacks = m_MyCmDevice.CreateCommand("SetTotalNumberOnRacks", "Demonstrates how to change this value");
            commandSetTotalNumberOnRacks.OnCommand += new CommandEventHandler(OnCommand_SetTotalNumberOnRacks);

            ICommand commandSetTotalNumberInstalled = m_MyCmDevice.CreateCommand("SetTotalNumberInstalled", "Demonstrates how to change this value");
            commandSetTotalNumberInstalled.OnCommand += new CommandEventHandler(OnCommand_SetTotalNumberInstalled);

            return m_MyCmDevice;
        }

        string OnGetCurrentTubeNumberAsPositionString(FractionCollectionEventArgs args)
        {
            // Do some useful work here
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "Get position string for tube: " + args.TubeNumber.ToString(CultureInfo.InvariantCulture));
            return String.Format(CultureInfo.InvariantCulture, "Tube{0}", args.TubeNumber);
        }

        void OnStartCollect(FractionCollectionEventArgs args)
        {
            // Do some useful work here
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "Start collecting in tube: " + args.TubeNumber.ToString(CultureInfo.InvariantCulture));
        }

        void OnSwitchTube(FractionCollectionEventArgs args)
        {
            // Do some useful work here
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "Switch to tube: " + args.TubeNumber.ToString(CultureInfo.InvariantCulture));
        }

        void OnEndCollect(FractionCollectionEventArgs args)
        {
            // Do some useful work here
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "End collecting. Switch to tube: " + args.TubeNumber.ToString(CultureInfo.InvariantCulture));
        }

        void OnCommand_GenerateAuditTrail(CommandEventArgs args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(new string('*', 4100));
            sb.Append(">");
            m_MyCmDevice.WriteInjectionTrayDescription(null);
            m_MyCmDevice.WriteInjectionTrayDescription(String.Empty);
            m_MyCmDevice.WriteInjectionTrayDescription("InjectionTrayDescription");
            m_MyCmDevice.WriteInjectionTrayDescription(sb.ToString());

            m_MyCmDevice.WriteFractionTrayDescription(null);
            m_MyCmDevice.WriteFractionTrayDescription(String.Empty);
            m_MyCmDevice.WriteFractionTrayDescription("FractionTrayDescription");
            m_MyCmDevice.WriteFractionTrayDescription(sb.ToString());

            Nullable<int> fractionID = 1;
            string fractionPosition = "2";
            Nullable<double> fractionVolume = 3.33; // Value is in milliliters.
            Nullable<double> maximumVolume = 9.99999999; //  Value is in milliliters.
            Nullable<double> fractionStartTime = 4.444; // Value is in minutes.
            Nullable<double> fractionEndTime = 5.5555; // Value is in minutes.
            FractionTriggerReason fractionStartReason = (FractionTriggerReason)6;
            FractionTriggerReason fractionEndReason = (FractionTriggerReason)7;
            PeakDetectorFractionInfo[] peakDetectorInfos = new PeakDetectorFractionInfo[4];
            peakDetectorInfos[0] = new PeakDetectorFractionInfo();
            peakDetectorInfos[0].DelayTime = 11.11;
            peakDetectorInfos[0].PeakDetected = true;
            peakDetectorInfos[0].PeakDetectorDeviceName = "UV.UV_VIS_1";
            peakDetectorInfos[1] = new PeakDetectorFractionInfo();
            peakDetectorInfos[1].DelayTime = 22.22;
            peakDetectorInfos[1].PeakDetected = false;
            peakDetectorInfos[1].PeakDetectorDeviceName = "UV.UV_VIS_2";
            peakDetectorInfos[2] = new PeakDetectorFractionInfo();
            peakDetectorInfos[2].DelayTime = 33.33;
            peakDetectorInfos[2].PeakDetected = true;
            peakDetectorInfos[2].PeakDetectorDeviceName = String.Empty;
            peakDetectorInfos[3] = new PeakDetectorFractionInfo();
            peakDetectorInfos[3].DelayTime = 44.44;
            peakDetectorInfos[3].PeakDetected = false;
            peakDetectorInfos[3].PeakDetectorDeviceName = null;

            m_MyCmDevice.WriteTubeInformation(fractionID, fractionPosition, fractionVolume, maximumVolume, fractionStartTime, fractionEndTime, fractionStartReason, fractionEndReason, peakDetectorInfos);

            peakDetectorInfos = new PeakDetectorFractionInfo[1];
            peakDetectorInfos[0] = new PeakDetectorFractionInfo();
            peakDetectorInfos[0].DelayTime = 11.11;
            peakDetectorInfos[0].PeakDetected = true;
            peakDetectorInfos[0].PeakDetectorDeviceName = "UV.UV_VIS_1";

            m_MyCmDevice.WriteTubeInformation(fractionID, fractionPosition, fractionVolume, maximumVolume, fractionStartTime, fractionEndTime, fractionStartReason, fractionEndReason, peakDetectorInfos);

            peakDetectorInfos = null;

            m_MyCmDevice.WriteTubeInformation(fractionID, fractionPosition, fractionVolume, maximumVolume, fractionStartTime, fractionEndTime, fractionStartReason, fractionEndReason, peakDetectorInfos);

            fractionID = null;
            fractionPosition = null;
            fractionVolume = null;
            maximumVolume = null;
            fractionStartTime = null;
            fractionEndTime = null;
            fractionStartReason = FractionTriggerReason.Unknown;
            fractionEndReason = FractionTriggerReason.Unknown;
            peakDetectorInfos = null;

            m_MyCmDevice.WriteTubeInformation(fractionID, fractionPosition, fractionVolume, maximumVolume, fractionStartTime, fractionEndTime, fractionStartReason, fractionEndReason, peakDetectorInfos);

            fractionID = null;
            fractionPosition = String.Empty;
            fractionVolume = null;
            maximumVolume = null;
            fractionStartTime = null;
            fractionEndTime = null;
            fractionStartReason = FractionTriggerReason.Unknown;
            fractionEndReason = FractionTriggerReason.Unknown;
            peakDetectorInfos = null;

            m_MyCmDevice.WriteTubeInformation(fractionID, fractionPosition, fractionVolume, maximumVolume, fractionStartTime, fractionEndTime, fractionStartReason, fractionEndReason, peakDetectorInfos);
        }

        void OnCommand_SetTotalNumberOnRacks(CommandEventArgs args)
        {
            m_MyCmDevice.SetTotalNumberOnRacks(12);
        }

        void OnCommand_SetTotalNumberInstalled(CommandEventArgs args)
        {
            m_MyCmDevice.SetTotalNumberInstalled(8);
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize properties that are visible in the connected state only.
        /// </summary>
        internal void OnConnect()
        {
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// When we are disconnected, we clear properties that are visible in the connected state only.
        /// </summary>
        internal void OnDisconnect()
        {
        }
    }
}
