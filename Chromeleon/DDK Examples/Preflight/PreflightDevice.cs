/////////////////////////////////////////////////////////////////////////////
//
// PreflightDevice.cs
// //////////////////
//
// Preflight Chromeleon DDK Code Example
//
// Device class for the Preflight example driver.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientfic
//
/////////////////////////////////////////////////////////////////////////////

using System.Threading;

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.Preflight
{
    class PreflightDevice
    {
        private IDevice m_MyCmDevice;
        private IStringProperty m_ModelNoProperty;
        private IIntProperty m_someProperty;

        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        internal IDevice Create(IDDK cmDDK, string name)
        {
            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is a preflight test device.");

            // create the standard Property containing our model number
            m_ModelNoProperty =
                m_MyCmDevice.CreateStandardProperty(StandardPropertyID.ModelNo, cmDDK.CreateString(20));

            m_someProperty = m_MyCmDevice.CreateBooleanProperty("Test", "Use this property to watch all preflight events.", "Off", "On");
            m_someProperty.OnPreflightSetProperty += new SetPropertyEventHandler(OnPfTest);
            m_someProperty.OnSetProperty += new SetPropertyEventHandler(OnTest);

            m_MyCmDevice.OnPreflightBegin += new PreflightEventHandler(OnPfBegin);
            m_MyCmDevice.OnPreflightEnd += new PreflightEventHandler(OnPfEnd);

            m_MyCmDevice.OnPreflightLatch += new PreflightEventHandler(OnPfLatch);
            m_MyCmDevice.OnPreflightSync += new PreflightEventHandler(OnPfSync);

            m_MyCmDevice.OnPreflightBroadcast += new BroadcastEventHandler(OnPfBroadcast);

            m_MyCmDevice.OnTransferPreflightToRun += new PreflightEventHandler(OnTransferPfToRun);

            ICommand command =
                m_MyCmDevice.CreateCommand("DoAbort", "This command sends an abort error.");
            command.OnCommand += new CommandEventHandler(OnDoAbort);

            command =
                m_MyCmDevice.CreateCommand("DoError", "This command sends an error.");
            command.OnCommand += new CommandEventHandler(OnDoError);

            return m_MyCmDevice;
        }

        /// When we are connected, we initialize our properties.
        internal void OnConnect()
        {
            m_ModelNoProperty.Update("Preflight Model");
            m_someProperty.Update(0);
        }

        private void OnPfTest(SetPropertyEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightSetProperty handler OnPfTest");
        }

        private void OnTest(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs iProp = args as SetIntPropertyEventArgs;
            m_MyCmDevice.AuditMessage(AuditLevel.Message, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnSetProperty OnTest");
            IIntProperty intProperty = iProp.Property as IIntProperty;
            intProperty.Update(iProp.NewValue);
        }

        private void OnPfBegin(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightBegin handler OnPfBegin");
        }

        private void OnPfEnd(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightEnd handler OnPfEnd");
        }

        private void OnPfLatch(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightLatch handler OnPfLatch");
        }

        private void OnPfSync(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightSync handler OnPfSync");
        }

        private void OnPfBroadcast(BroadcastEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Warning, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnPreflightBroadcast handler OnPfBroadcast(" + args.Broadcast.ToString() + ")");
        }

        private void OnTransferPfToRun(PreflightEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Message, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnTransferPreflightToRun handler OnTransferPfToRun, please wait...");

            Thread.Sleep(2000);

            m_MyCmDevice.AuditMessage(AuditLevel.Message, args.RunContext.ProgramTime.Minutes.ToString() + " min: OnTransferPreflightToRun handler OnTransferPfToRun has finished.");
        }

        private void OnDoAbort(CommandEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Abort, args.RunContext.ProgramTime.Minutes.ToString() + " min: DoAbort command!");
        }
        private void OnDoError(CommandEventArgs args)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Error, args.RunContext.ProgramTime.Minutes.ToString() + " min: DoError command!");
        }
    }
}
