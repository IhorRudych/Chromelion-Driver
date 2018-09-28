// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    // Defined just for demo - no data point are sent to Chromeleon
    internal class PumpChannelPressure : Device
    {
        #region Fields
        private readonly PumpChannelPressureProperties m_Properties;

        private readonly IChannel m_Channel;

        private bool m_IsAcquisitionOn;
        #endregion

        #region Constructor
        public PumpChannelPressure(IDriverEx driver, IDDK ddk, IDevice owner, string id, string channelName, double signalMin, double signalMax, int signalDigits, UnitConversion.PhysUnitEnum unit)
            : base(driver, ddk, typeof(DetectorChannel).Name, id, channelName, owner, CreateChannel(ddk, channelName, signalMin, signalMax, signalDigits, unit))
        {
            Log.TaskBegin(Id);
            try
            {
                if (owner == null)
                    throw new ArgumentNullException("owner");

                m_Channel = m_Device as IChannel;
                if (m_Channel == null)
                {
                    if (m_Device == null)
                        throw new InvalidOperationException("The device is not created");
                    if (m_Channel == null)
                        throw new InvalidOperationException("The device is type " + m_Device.GetType().FullName + " is not " + typeof(IChannel).FullName);
                }

                m_Properties = new PumpChannelPressureProperties(m_DDK, m_Device, m_Channel);

                m_Channel.PhysicalQuantity = "Pressure";  // What do we actually measure
                m_Channel.NeedsIntegration = false;       // If False then this channel doesn't have peaks and, we don't want to have it integrated

                m_Channel.AcquisitionOnCommand.OnCommand += OnCommandAcquisitionOn;
                m_Channel.AcquisitionOffCommand.OnCommand += OnCommandAcquisitionOff;
                m_Channel.OnDataFinished += OnChannelDataFinished;

                m_Driver.OnConnected += OnDriverConnected;
                m_Driver.OnDisconnected += OnDriverDisconnected;

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        private static IDevice CreateChannel(IDDK ddk, string name, double min, double max, int digits, UnitConversion.PhysUnitEnum unit)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (unit == UnitConversion.PhysUnitEnum.PhysUnit_Unknown)
                throw new ArgumentException("The unit mus be specified");

            ITypeDouble typeSignal = ddk.CreateDouble(min, max, digits);
            typeSignal.Unit = UnitConversionEx.PhysUnitName(unit);
            IChannel result = ddk.CreateChannel(name, "Pump Pressure Channel", typeSignal);
            return result;
        }
        #endregion

        #region Properties
        public AcquisitionState ChannelAcquisitionState
        {
            get { return (AcquisitionState)m_Channel.AcquisitionStateProperty.Value.GetValueOrDefault(); }
            private set
            {
                if (value == ChannelAcquisitionState)
                {
                    return;
                }
                m_Channel.AcquisitionStateProperty.Update((int)value);
                Log.PropertyChangedEnum(Id, m_Channel.AcquisitionStateProperty, value.ToString(), CallerMethodName);
            }
        }
        #endregion

        #region Events Driver Connected / Disconnected
        private void OnDriverConnected(object sender, EventArgs args)
        {
            m_IsAcquisitionOn = false;
            ChannelAcquisitionState = AcquisitionState.Idle;
        }

        private void OnDriverDisconnected(object sender, EventArgs args)
        {
            try
            {
                ChannelDataFinished();
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, "Channel Data Finished error: " + ex.Message);
            }
        }
        #endregion

        #region Events Channel
        private void OnCommandAcquisitionOn(CommandEventArgs args)
        {
            m_IsAcquisitionOn = true;
        }

        private void OnCommandAcquisitionOff(CommandEventArgs args)
        {
            ChannelDataFinished();
        }

        private void OnChannelDataFinished(DataFinishedEventArgs args)
        {
            ChannelDataFinished();
        }

        private void ChannelDataFinished()
        {
            if (!m_IsAcquisitionOn)
            {
                return;
            }

            m_IsAcquisitionOn = false;
            Log.WriteLine(Id, "ChannelAcquisitionState = " + ChannelAcquisitionState.ToString(), CallerMethodName);
            m_Channel.NoMoreData();
        }

        private void ChannelUpdateData()
        {
        }
        #endregion
    }
}
