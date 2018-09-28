// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal partial class Demo
    {
        private void CommandStartStopInit()
        {
            ICommand command = m_Device.CreateCommand("Start", string.Empty);
            command.OnCommand += OnCommandStart;

            command = m_Device.CreateCommand("Stop", string.Empty);
            command.OnCommand += OnCommandStop;
        }

        private int ProcessStep
        {
            get { return m_Properties.ProcessStep.Value.GetValueOrDefault(); }
            set
            {
                m_Properties.ProcessStep.Update((int)value);
                Log.PropertyChanged(Id, m_Properties.ProcessStep, CallerMethodName);
            }
        }

        private void OnCommandStart(CommandEventArgs args)
        {
            const string taskName = "Process All Steps";
            try
            {
                // This command is only allowed to be executed manual by the user
                if (!args.RunContext.IsManual)
                    throw new InvalidOperationException(args.Command.Name + " can be executed by the user in manual mode only");

                if (!args.RunContext.IsSemanticCheck)
                {
                    m_Driver.CheckIsCommunicating();
                }

                Action task = (() =>
                {
                    ProcessAllSteps(taskName);
                });

                AsyncCallback taskStartCallback = (ar =>
                {
                    try
                    {
                        task.EndInvoke(ar);
                    }
                    catch (Exception ex)
                    {
                        AuditMessage(AuditLevel.Error, "Task \"" + taskName + "\" error: " + ex.Message);
                    }
                });

                string cmdText = "Command " + args.Command.Name;
                string userName = Property.GetUserName(args);
                Log.SendCommand(Id, cmdText + (string.IsNullOrEmpty(userName) ? string.Empty : " - requested by " + userName));

                lock (m_Driver.LockObject)
                {
                    m_Driver.SetBusy(taskName);
                    task.BeginInvoke(taskStartCallback, null);
                }
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, string.Format(CultureInfo.CurrentCulture, "Command {0} error: {1}", args.Command.Name, ex.Message));
            }
        }

        private void OnCommandStop(CommandEventArgs args)
        {
            m_Driver.Stop();
        }

        private void ProcessAllSteps(string taskName)
        {
            try
            {
                Log.TaskBegin(Id);
                AuditMessage(AuditLevel.Message, "Task \"" + taskName + "\" Begin");
                ProcessStep = 0;
                while (true)
                {
                    lock (m_Driver.LockObject)
                    {
                        if (m_Driver.IsStopping)
                        {
                            break;
                        }
                    }

                    m_Driver.CheckIsCommunicating();
                    ProcessStep++;
                    Log.WriteLine(Id, "Executing " + ProcessStep.ToString());
                    System.Threading.Thread.Sleep(500);
                }

                AuditMessage(AuditLevel.Message, "Task \"" + taskName + "\" End");
                m_Driver.SetIdle(taskName);
                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                m_Driver.SetIdle(taskName, ex);
                Log.TaskEnd(Id, ex);
            }
        }
    }
}
