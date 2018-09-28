/////////////////////////////////////////////////////////////////////////////
//
// PreparingSamplerDevice.cs
// /////////////////////////
//
// PreparingSampler Chromeleon DDK Code Example
//
// Device class for the PreparingSampler.
//
// Copyright (C) 2005-2017 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Text;
using System.Threading;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.PreparingSampler
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class PreparingSamplerDevice
    {
        #region Data Members

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// Our inject handler.
        private IInjectHandler m_InjectHandler;

        private double m_Volume;
        private int m_Position;

        private ISequencePreflight m_CurrentSequence;

        private ICommand m_PrepareNextSampleCommand;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "PreparingSampler device.");

            ITypeDouble tVolume = cmDDK.CreateDouble(0.1, 10.0, 1);
            ITypeInt tPosition = cmDDK.CreateInt(1, 10);

            for (int i = 1; i < 10; i++)
            {
                tPosition.AddNamedValue("RA" + i.ToString(), i);
            }

            m_InjectHandler = m_MyCmDevice.CreateInjectHandler(tVolume, tPosition);

            m_InjectHandler.PositionProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetPosition);

            m_InjectHandler.VolumeProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetVolume);

            m_InjectHandler.InjectCommand.OnCommand += new CommandEventHandler(OnInject);

            m_PrepareNextSampleCommand = m_MyCmDevice.CreateCommand("PrepareNextSample", "Start sample preparation for the next injection.");

            m_PrepareNextSampleCommand.OnCommand += new CommandEventHandler(m_PrepareNextSampleCommand_OnCommand);

            // Note that before the batch preflight starts all contained methods are preflighted.
            m_MyCmDevice.OnPreflightBegin += new PreflightEventHandler(m_MyCmDevice_OnPreflightBegin);
            m_MyCmDevice.OnPreflightEnd += new PreflightEventHandler(m_MyCmDevice_OnPreflightEnd);
            m_MyCmDevice.OnTransferPreflightToRun += new PreflightEventHandler(m_MyCmDevice_OnTransferPreflightToRun);

            // The following events can be used to investigate injection properties and stand-alone methods
            // during batch preflight

            m_MyCmDevice.OnBatchPreflightBegin += new BatchPreflightEventHandler(m_MyCmDevice_OnBatchPreflightBegin);
            m_MyCmDevice.OnBatchPreflightSample += new SamplePreflightEventHandler(m_MyCmDevice_OnBatchPreflightSample);
            m_MyCmDevice.OnBatchPreflightStandAloneProgram += new BatchEntryPreflightEventHandler(m_MyCmDevice_OnBatchPreflightStandAloneProgram);
            m_MyCmDevice.OnBatchPreflightEmergencyProgram += new BatchEntryPreflightEventHandler(m_MyCmDevice_OnBatchPreflightEmergencyProgram);
            m_MyCmDevice.OnBatchPreflightEnd += new BatchPreflightEventHandler(m_MyCmDevice_OnBatchPreflightEnd);

            m_MyCmDevice.OnSequenceStart += new SequencePreflightEventHandler(m_MyCmDevice_OnSequenceStart);
            m_MyCmDevice.OnSequenceEnd += new SequencePreflightEventHandler(m_MyCmDevice_OnSequenceEnd);

            m_MyCmDevice.OnSequenceChange += new SequenceChangeEventHandler(m_MyCmDevice_OnSequenceChange);

            return m_MyCmDevice;
        }

        void m_MyCmDevice_OnPreflightBegin(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnPreflightBegin handler");
        }

        void m_MyCmDevice_OnPreflightEnd(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnPreflightEnd handler");
        }

        void m_MyCmDevice_OnTransferPreflightToRun(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnTransferPfToRun handler");
        }

        void m_PrepareNextSampleCommand_OnCommand(CommandEventArgs args)
        {
            // Notify Chromeleon that we prepare the next injection now.
            m_CurrentSequence.LastPreparingIndex = m_CurrentSequence.RunningIndex + 1;

            // Emit a message to the next sample's audit trail
            m_MyCmDevice.SetAuditContextSample(m_CurrentSequence.RunningIndex + 1);
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "This goes into next sample's audit trail");

            // Emit a message to the current sample's audit trail
            m_MyCmDevice.SetAuditContextSample(0);
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "This goes into current sample's audit trail");
        }

        void m_MyCmDevice_OnSequenceStart(SequencePreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnSequenceStart handler");

            // Save a reference to the current sequence for later use
            m_CurrentSequence = args.SequencePreflight;

            foreach (ICustomVariable customSequenceVariable in args.SequencePreflight.CustomSequenceVariables)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Warning, String.Format("CSV: Name: {0} Type: {1} Value: {2}", customSequenceVariable.Name, customSequenceVariable.Type, customVariableValue(customSequenceVariable)));
            }
        }

        void m_MyCmDevice_OnSequenceEnd(SequencePreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnSequenceEnd handler");

            // After this event the current sequence becomes invalid.
            m_CurrentSequence = null;
        }

        void m_MyCmDevice_OnSequenceChange(SequencePreflightEventArgs oldSequenceArgs, SequencePreflightEventArgs newSequenceArgs)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnSequenceChange handler");

            // Save a reference to the current sequence for later use
            m_CurrentSequence = newSequenceArgs.SequencePreflight;

            foreach (ICustomVariable customSequenceVariable in newSequenceArgs.SequencePreflight.CustomSequenceVariables)
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Warning, String.Format("CSV: Name: {0} Type: {1} Value: {2}", customSequenceVariable.Name, customSequenceVariable.Type, customVariableValue(customSequenceVariable)));
            }
        }

        string customVariableValue(ICustomVariable customVariable)
        {
            string value = String.Empty;
            if (customVariable.Type == CustomVariableType.String)
            {
                value = String.Format("\"{0}\"", customVariable.Text);
            }
            if (customVariable.Type == CustomVariableType.Numeric)
            {
                if (customVariable.Number.HasValue)
                {
                    value = customVariable.Number.Value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    value = "<empty>";
                }
            }
            return value;
        }

        void m_MyCmDevice_OnBatchPreflightBegin(BatchPreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnBatchPreflightBegin handler");

            // We set SequencePreflight.UpdatesWanted = true to indicate that we want to receive
            // the OnSequenceChange event.
            args.BatchPreflight.UpdatesWanted = true;
        }

        void m_MyCmDevice_OnBatchPreflightSample(SamplePreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnBatchPreflightSample handler");

            if (args.SamplePreflight.CustomInjectionVariables != null)
            {
                foreach (ICustomVariable customInjectionVariable in args.SamplePreflight.CustomInjectionVariables)
                {
                    m_MyCmDevice.AuditMessage(AuditLevel.Warning, String.Format("CIV: Name: {0} Type: {1} Value: {2}", customInjectionVariable.Name, customInjectionVariable.Type, customVariableValue(customInjectionVariable)));
                }
            }
        }

        void m_MyCmDevice_OnBatchPreflightStandAloneProgram(BatchEntryPreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnBatchPreflightStandAloneProgram handler");
        }

        void m_MyCmDevice_OnBatchPreflightEmergencyProgram(BatchEntryPreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnBatchPreflightEmergencyProgram handler");
        }

        void m_MyCmDevice_OnBatchPreflightEnd(BatchPreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, "OnBatchPreflightEnd handler");
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

        private void OnSetPosition(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs setPositionArgs =
                args as SetIntPropertyEventArgs;

            if (setPositionArgs.NewValue.HasValue)
            {
                m_Position = setPositionArgs.NewValue.Value;
                m_InjectHandler.PositionProperty.Update(m_Position);
            }
            else
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Error, "Invalid position.");
            }
        }

        private void OnSetVolume(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs setVolumeArgs =
                args as SetDoublePropertyEventArgs;

            if (setVolumeArgs.NewValue.HasValue)
            {
                m_Volume = setVolumeArgs.NewValue.Value;
                m_InjectHandler.VolumeProperty.Update(m_Volume);
            }
            else
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Error, "Invalid volume.");
            }
        }

        private void OnInject(CommandEventArgs args)
        {
            IDoubleParameterValue vVolume =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Volume"))
                as IDoubleParameterValue;

            if (vVolume != null && vVolume.Value.HasValue)
            {
                m_Volume = vVolume.Value.Value;
                m_InjectHandler.VolumeProperty.Update(m_Volume);
            }

            IIntParameterValue vPosition =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Position"))
                as IIntParameterValue;

            if (vPosition != null && vPosition.Value.HasValue)
            {
                m_Position = vPosition.Value.Value;
                m_InjectHandler.PositionProperty.Update(m_Position);
            }

            // Collect information from the injection table to display it
            StringBuilder sb = new StringBuilder("RunningSample:\n");
            sb.Append("RunningIndex=" + m_CurrentSequence.RunningIndex.ToString() + "\n");
            sb.Append("Position=" + m_CurrentSequence.RunningSample.Position + "\n");
            sb.Append("Number=" + m_CurrentSequence.RunningSample.Number + "\n");
            sb.Append("ID=" + m_CurrentSequence.RunningSample.ID + "\n");
            sb.Append("Type=" + m_CurrentSequence.RunningSample.SampleType.ToString() + "\n");
            m_MyCmDevice.AuditMessage(AuditLevel.Message, sb.ToString());

            m_MyCmDevice.AuditMessage(AuditLevel.Message,
                "Injecting " + m_Volume.ToString() +
                " ml from Position: " + m_Position.ToString());

            Thread.Sleep(5000);

            m_MyCmDevice.AuditMessage(AuditLevel.Message, "Injection done.");

            m_InjectHandler.NotifyInjectResponse();
        }
    }
}
