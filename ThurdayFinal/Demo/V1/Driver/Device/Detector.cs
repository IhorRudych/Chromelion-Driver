// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class Detector : Device
    {
        private readonly DetectorProperties m_Properties;

        private readonly List<DetectorChannel> m_Channels;

        public Detector(IDriverEx driver, IDDK ddk, Config.Detector config, string id)
            : base(driver, ddk, config, typeof(Detector).Name, id)
        {
            Log.TaskBegin(Id);
            try
            {
                m_Properties = new DetectorProperties(m_DDK, m_Device, config);

                m_Channels = new List<DetectorChannel>();
                for (int i = 0; i < config.ChannelsNumber; i++)
                {
                    int channelNumber = i + 1;

                    UnitConversion.PhysUnitEnum unit = UnitConversion.PhysUnitEnum.PhysUnit_MilliAU;  // mAU - Milli-Absorbance
                    string channelPhysicalQuantity = "Absorbance";  // What do we actually measure
                    bool channelNeedsIntegration = true;
                    if (channelNumber == 2)
                    {
                        unit = UnitConversion.PhysUnitEnum.PhysUnit_MilliVolt;
                        channelPhysicalQuantity = "Voltage";
                        channelNeedsIntegration = false;   // This channel doesn't have peaks and, we don't want to have it integrated
                    }

                    string channelId = "Channel_" + channelNumber.ToString(CultureInfo.InvariantCulture);
                    string channelName = channelId + "_Name";
                    DetectorChannel channel = new DetectorChannel(driver, ddk, m_Device, channelId, channelName, channelNumber, unit, channelNeedsIntegration, channelPhysicalQuantity);
                    m_Channels.Add(channel);
                }

                ICommand command = m_Device.CreateCommand("AutoZero", string.Empty);
                command.OnCommand += OnCommandAutoZero;
                // Enable the scenario below:
                // Detector.Autozero
                // Wait     Detector.Ready
                command.ImmediateNotReady = true;

                IsAutoZeroRunning = false;

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        #region Properties
        public bool Ready
        {
            get { return Property.GetBool(m_Properties.Ready.Value.GetValueOrDefault()); }
            private set
            {
                if (Ready == value)
                {
                    return;
                }
                int valueNumber = Property.GetBoolNumber(value);
                m_Properties.Ready.Update(valueNumber);
                Log.PropertyChanged(Id, m_Properties.Ready.Name, value, CallerMethodName);
            }
        }

        public bool IsAutoZeroRunning
        {
            get { return Property.GetBool(m_Properties.IsAutoZeroRunning.Value.GetValueOrDefault()); }
            private set
            {
                if (IsAutoZeroRunning == value)
                {
                    return;
                }
                int valueNumber = Property.GetBoolNumber(value);
                m_Properties.IsAutoZeroRunning.Update(valueNumber);
                Log.PropertyChanged(Id, m_Properties.IsAutoZeroRunning, CallerMethodName);
            }
        }
        #endregion

        private void OnCommandAutoZero(CommandEventArgs args)
        {
            if (IsAutoZeroRunning)
            {
                return;
            }

            Ready = false;

            Action task = (() =>
            {
                IsAutoZeroRunning = true;
                Thread.Sleep(3000);
                IsAutoZeroRunning = false;
                Ready = true;
            });

            AsyncCallback taskCallback = (ar =>
            {
                try
                {
                    task.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    AuditMessage(AuditLevel.Error, "Command " + args.Command.Name + " failed: " + ex.Message);
                }
            });

            task.BeginInvoke(taskCallback, null);
        }
    }
}
