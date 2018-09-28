/////////////////////////////////////////////////////////////////////////////
//
// Device.cs
// /////////
//
// EventDumpDriver Chromeleon DDK Code Example
//
// Device class for the EventDumpDriver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.EventDumpDriver
{
    class Device
    {
        private IDDK m_DDK;
        private IDevice m_Device;

        public Device(IDDK cmDDK)
        {
            m_DDK = cmDDK;
            m_Device = m_DDK.CreateDevice("MyDevice", "Device that dumps all events to the audit trail");

            IIntProperty simpleProperty = m_Device.CreateBooleanProperty("SimpleProperty", "help text", "False", "True");
            simpleProperty.OnPreflightSetProperty += new SetPropertyEventHandler(simpleProperty_OnPreflightSetProperty);
            simpleProperty.OnSetProperty += new SetPropertyEventHandler(simpleProperty_OnSetProperty);

            m_Device.OnBroadcast += new BroadcastEventHandler(m_Device_OnBroadcast);
            // will be called 100 times a second ... m_Device.OnLatch += new RuntimeEventHandler(m_Device_OnLatch);
            // will be called 100 times a second ... m_Device.OnSync += new RuntimeEventHandler(m_Device_OnSync);

            m_Device.OnPreflightBegin += new PreflightEventHandler(m_Device_OnPreflightBegin);
            m_Device.OnPreflightBroadcast += new BroadcastEventHandler(m_Device_OnPreflightBroadcast);
            m_Device.OnPreflightLatch += new PreflightEventHandler(m_Device_OnPreflightLatch);
            m_Device.OnPreflightSync += new PreflightEventHandler(m_Device_OnPreflightSync);
            m_Device.OnPreflightEnd += new PreflightEventHandler(m_Device_OnPreflightEnd);

            m_Device.OnTransferPreflightToRun += new PreflightEventHandler(m_Device_OnTransferPreflightToRun);

            m_Device.OnBatchPreflightBegin += new BatchPreflightEventHandler(m_Device_OnBatchPreflightBegin);
            m_Device.OnBatchPreflightSample += new SamplePreflightEventHandler(m_Device_OnBatchPreflightSample);
            m_Device.OnBatchPreflightStandAloneProgram += new BatchEntryPreflightEventHandler(m_Device_OnBatchPreflightStandAloneProgram);
            m_Device.OnBatchPreflightEmergencyProgram += new BatchEntryPreflightEventHandler(m_Device_OnBatchPreflightEmergencyProgram);
            m_Device.OnBatchPreflightEnd += new BatchPreflightEventHandler(m_Device_OnBatchPreflightEnd);

            m_Device.OnSequenceStart += new SequencePreflightEventHandler(m_Device_OnSequenceStart);
            m_Device.OnSequenceEnd += new SequencePreflightEventHandler(m_Device_OnSequenceEnd);
            m_Device.OnSequenceChange += new SequenceChangeEventHandler(m_Device_OnSequenceChange);
        }

        void simpleProperty_OnSetProperty(SetPropertyEventArgs args)
        {
            m_Device.AuditMessage(AuditLevel.Normal, "Device.simpleProperty_OnSetProperty() called");
        }

        void simpleProperty_OnPreflightSetProperty(SetPropertyEventArgs args)
        {
            String message = String.Format("Device.simpleProperty_OnPreflightSetProperty() called");
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        public void Connect()
        {
            m_Device.AuditMessage(AuditLevel.Normal, "Device.Connect() called");
        }

        void m_Device_OnBroadcast(BroadcastEventArgs args)
        {
            String message = String.Format("Device.OnBroadcast({0}) called", args.Broadcast);
            m_Device.AuditMessage(AuditLevel.Normal, message);
        }
        void m_Device_OnLatch(RuntimeEventArgs args)
        {
            String message = String.Format("Device.OnLatch({0}) called", args.InstrumentID);
            m_Device.AuditMessage(AuditLevel.Normal, message);
        }
        void m_Device_OnSync(RuntimeEventArgs args)
        {
            String message = String.Format("Device.OnSync({0}) called", args.InstrumentID);
            m_Device.AuditMessage(AuditLevel.Normal, message);
        }

        void m_Device_OnPreflightBegin(PreflightEventArgs args)
        {
            String message = String.Format("Device.OnPreflightBegin({0}) called", args.RunContext);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }
        void m_Device_OnPreflightBroadcast(BroadcastEventArgs args)
        {
            String message = String.Format("Device.OnPreflightBroadcast({0}) called", args.Broadcast);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }
        void m_Device_OnPreflightLatch(PreflightEventArgs args)
        {
            String message = String.Format("Device.OnPreflightLatch({0}) called", args.RunContext);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }
        void m_Device_OnPreflightSync(PreflightEventArgs args)
        {
            String message = String.Format("Device.OnPreflightSync({0}) called", args.RunContext);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }
        void m_Device_OnPreflightEnd(PreflightEventArgs args)
        {
            String message = String.Format("Device.OnPreflightEnd({0}) called", args.RunContext);
            m_Device.AuditMessage(AuditLevel.Warning, message);

            IProgramSteps steps = args.RunContext.ProgramSteps;
            message = String.Format("ProgramSteps count: {0}", steps.Count);
            m_Device.AuditMessage(AuditLevel.Warning, message);
            foreach (IProgramStep step in steps)
            {
                IPropertyAssignmentStep paStep = step as IPropertyAssignmentStep;
                if (paStep != null)
                {
                    message = String.Format("Assignment at time {0} for property {1}", paStep.Retention.Minutes, paStep.Property.Name);
                }
                ILatchStep lStep = step as ILatchStep;
                if (lStep != null)
                {
                    message = String.Format("Latch at time {0}", lStep.Retention);
                }
                ISyncStep sStep = step as ISyncStep;
                if (sStep != null)
                {
                    message = String.Format("Sync at time {0}", sStep.Retention);
                }
                IRampStep rStep = step as IRampStep;
                if (rStep != null)
                {
                    message = String.Format("Ramp at time {0}", rStep.Retention);
                }
                m_Device.AuditMessage(AuditLevel.Warning, message);
            }
        }

        void m_Device_OnTransferPreflightToRun(PreflightEventArgs args)
        {
            String message = String.Format("Device.OnTransferPreflightToRun({0}) called", args.RunContext);
            m_Device.AuditMessage(AuditLevel.Normal, message);
        }

        void m_Device_OnBatchPreflightBegin(BatchPreflightEventArgs args)
        {
            String message = String.Format("Device.OnBatchPreflightBegin({0}) called", args.BatchPreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnBatchPreflightSample(SamplePreflightEventArgs args)
        {
            String message = String.Format("Device.OnBatchPreflightSample({0}) called", args.SamplePreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnBatchPreflightStandAloneProgram(BatchEntryPreflightEventArgs args)
        {
            String message = String.Format("Device.OnBatchPreflightStandAloneProgram({0}) called", args.BatchEntryPreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnBatchPreflightEmergencyProgram(BatchEntryPreflightEventArgs args)
        {
            String message = String.Format("Device.OnBatchPreflightEmergencyProgram({0}) called", args.BatchEntryPreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnBatchPreflightEnd(BatchPreflightEventArgs args)
        {
            String message = String.Format("Device.OnBatchPreflightEnd({0}) called", args.BatchPreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnSequenceStart(SequencePreflightEventArgs args)
        {
            args.SequencePreflight.UpdatesWanted = true;

            String message = String.Format("Device.OnSequenceStart({0}) called", args.SequencePreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnSequenceChange(SequencePreflightEventArgs oldSequenceArgs, SequencePreflightEventArgs newSequenceArgs)
        {
            String message = String.Format("Device.OnSequenceChange({0}, {1}) called", oldSequenceArgs.SequencePreflight, newSequenceArgs.SequencePreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        void m_Device_OnSequenceEnd(SequencePreflightEventArgs args)
        {
            String message = String.Format("Device.OnSequenceEnd({0}) called", args.SequencePreflight);
            m_Device.AuditMessage(AuditLevel.Warning, message);
        }

        public void Disconnect()
        {
            m_Device.AuditMessage(AuditLevel.Normal, "Device.Disconnect() called");
        }

        public void Exit()
        {
            m_Device.AuditMessage(AuditLevel.Normal, "Device.Exit() called");
        }
    }
}
