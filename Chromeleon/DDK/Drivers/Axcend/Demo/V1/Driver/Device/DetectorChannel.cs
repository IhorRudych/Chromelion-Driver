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
    internal class DetectorChannel : Device, IDisposable  // See also examples ChannelTest, ExampleLCSystem, etc.
    {
        #region Fields
        public enum Location
        {
            Left = 1,
            Middle,
            Right,
        }

        private readonly int m_Number;
        private readonly DetectorChannelProperties m_Properties;

        private readonly IChannel m_Channel;

        private bool m_IsAcquisitionOn;

        private DateTime m_AcquisitionTimeOn;
        private DateTime m_AcquisitionTimeOff;
        private TimeSpan m_AcquisitionTimeDiff;

        private double m_AcquisitionOnRetentionTimeMinutes;
        private double m_AcquisitionOnRetentionTimeMilliSeconds;
        private double m_AcquisitionOnRetentionTimeNanoSeconds;

        private double m_TimeIntervalBetweenTwoDataPointsMinutes;
        private double m_TimeIntervalBetweenTwoDataPointsMilliSeconds;
        private double m_TimeIntervalBetweenTwoDataPointsNanoSeconds;

        private int m_DataPacketIndex;
        private int m_DataPointIndex;

        private readonly System.Timers.Timer m_Timer;
        #endregion

        #region Constructor
        public DetectorChannel(IDriverEx driver, IDDK ddk, IDevice owner, string id, string channelName, int channelNumber, UnitConversion.PhysUnitEnum unit, bool channelNeedsIntegration, string channelPhysicalQuantity)
            : base(driver, ddk, typeof(DetectorChannel).Name, id, channelName, owner, CreateChannel(ddk, channelName, unit))
        {
            m_Number = channelNumber;
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

                m_Properties = new DetectorChannelProperties(m_DDK, m_Device, channelNumber, m_Channel);

                m_Channel.PhysicalQuantity = channelPhysicalQuantity;  // What do we actually measure
                m_Channel.NeedsIntegration = channelNeedsIntegration;  // If False then this channel doesn't have peaks and, we don't want to have it integrated

                Rate = 100;

                // Signal scaling
                m_Channel.SignalFactorProperty.Update(1);

                m_Properties.Wavelength.OnSetProperty += OnPropertyWaveLengthSet;

                m_Channel.AcquisitionOnCommand.OnPreflightCommand += OnCommandAcquisitionOnPreflight;
                m_Channel.AcquisitionOnCommand.OnCommand += OnCommandAcquisitionOn;

                m_Channel.AcquisitionOffCommand.OnPreflightCommand += OnCommandAcquisitionOffPreflight;
                m_Channel.AcquisitionOffCommand.OnCommand += OnCommandAcquisitionOff;

                m_Channel.OnDataFinished += OnChannelDataFinished;

                m_Driver.OnConnected += OnDriverConnected;
                m_Driver.OnDisconnected += OnDriverDisconnected;

                m_Timer = new System.Timers.Timer(1000);
                m_Timer.Elapsed += OnTimerElapsed;

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        private static IDevice CreateChannel(IDDK ddk, string name, UnitConversion.PhysUnitEnum unit)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (unit == UnitConversion.PhysUnitEnum.PhysUnit_Unknown)
                throw new ArgumentException("The unit mus be specified");

            ITypeInt typeSignal = ddk.CreateInt(0, 1000);
            typeSignal.Unit = UnitConversionEx.PhysUnitName(unit);
            IChannel result = ddk.CreateChannel(name, "Detector channel", typeSignal);
            return result;
        }
        #endregion

        #region Properties
        public double Rate
        {
            get { return m_Properties.Rate.Value.GetValueOrDefault(); }
            private set
            {
                if (value == Rate)
                {
                    return;
                }
                m_Properties.Rate.Update(value);
                Log.PropertyChanged(Id, m_Properties.Rate, CallerMethodName);
            }
        }

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

        public DateTime AcquisitionTimeOn
        {
            [DebuggerStepThrough]
            get { return m_AcquisitionTimeOn; }
            private set
            {
                if (value == m_AcquisitionTimeOn)
                {
                    return;
                }

                m_AcquisitionTimeOn = value;

                string displayValue;
                if (value == DateTime.MinValue)
                {
                    displayValue = Property.EmptyString;
                }
                else
                {
                    displayValue = value.ToLocalTime().ToString(Util.DateTimeFormat);
                }
                m_Properties.AcquisitionTimeOn.Update(displayValue);
                Log.PropertyChanged(Id, m_Properties.AcquisitionTimeOn.Name, displayValue, CallerMethodName);

                SetAcquisitionTimeDiff();
            }
        }

        public DateTime AcquisitionTimeOff
        {
            [DebuggerStepThrough]
            get { return m_AcquisitionTimeOff; }
            private set
            {
                if (value == m_AcquisitionTimeOff)
                {
                    return;
                }

                m_AcquisitionTimeOff = value;

                string displayValue;
                if (value == DateTime.MinValue)
                {
                    displayValue = Property.EmptyString;
                }
                else
                {
                    displayValue = value.ToLocalTime().ToString(Util.DateTimeFormat);
                }
                m_Properties.AcquisitionTimeOff.Update(displayValue);
                Log.PropertyChanged(Id, m_Properties.AcquisitionTimeOff.Name, displayValue, CallerMethodName);

                SetAcquisitionTimeDiff();
            }
        }

        private void SetAcquisitionTimeDiff()
        {
            DateTime dt1 = AcquisitionTimeOn;
            DateTime dt2 = AcquisitionTimeOff;
            if (dt1 == DateTime.MinValue || dt2 == DateTime.MinValue)
            {
                AcquisitionTimeDiff = TimeSpan.Zero;
            }
            else
            {
                AcquisitionTimeDiff = dt2 - dt1;
            }
        }

        public TimeSpan AcquisitionTimeDiff
        {
            [DebuggerStepThrough]
            get { return m_AcquisitionTimeDiff; }
            private set
            {
                if (value == m_AcquisitionTimeDiff)
                {
                    return;
                }

                m_AcquisitionTimeDiff = value;

                string displayValue;
                if (value == TimeSpan.Zero)
                {
                    displayValue = Property.EmptyString;
                }
                else
                {
                    displayValue = value.ToString();
                }

                m_Properties.AcquisitionTimeDiff.Update(displayValue);
                Log.PropertyChanged(Id, m_Properties.AcquisitionTimeDiff.Name, displayValue, CallerMethodName);
            }
        }
        #endregion

        #region Events Properties
        public void OnPropertyWaveLengthSet(SetPropertyEventArgs args)
        {
            Nullable<double> value = Property.GetDoubleNullable(args);
            m_Properties.Wavelength.Update(value);
        }
        #endregion

        #region Events Driver Connected / Disconnected
        private void OnDriverConnected(object sender, EventArgs args)
        {
            // Get/Set the values from/to the hardware

            m_Timer.Enabled = false;
            m_IsAcquisitionOn = false;
            ChannelAcquisitionState = AcquisitionState.Idle;

            AcquisitionTimeOn = DateTime.MinValue;
            AcquisitionTimeOff = DateTime.MinValue;

            m_Properties.Wavelength.Update(m_Properties.Wavelength.DataType.Minimum);
        }

        private void OnDriverDisconnected(object sender, EventArgs args)
        {
            try
            {
                // Hardware/Communications errors are possible
                ChannelDataFinished();
            }
            catch (Exception ex)
            {
                AuditMessage(AuditLevel.Error, "Channel Data Finished error: " + ex.Message);
            }

            // Should we do this?
            m_Properties.Wavelength.Update(null);
        }
        #endregion

        #region Events Channel
        private void OnCommandAcquisitionOnPreflight(CommandEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnCommandAcquisitionOn(CommandEventArgs args)  // Called when CM calls StartAcq
        {
            Log.WriteLine(Id);

            m_Driver.CheckIsCommunicating();

            m_IsAcquisitionOn = true;
            m_DataPacketIndex = 0;
            m_DataPointIndex = 0;

            AcquisitionTimeOn = DateTime.UtcNow;
            AcquisitionTimeOff = DateTime.MinValue;

            // args.RetentionTime.Minutes < 0 means it's a manual data acquisition and the retention time will start after the call
            if (args.RetentionTime == null || args.RetentionTime.Minutes < 0)
            {
                m_AcquisitionOnRetentionTimeMinutes = 0;
            }
            else
            {
                m_AcquisitionOnRetentionTimeMinutes = args.RetentionTime.Minutes;
                //double acquisitionOnRetentionTimeMilliSeconds = args.RetentionTime.HundredthSeconds * 10;  // 1 HundredthSeconds = 1/100 seconds
            }

            m_AcquisitionOnRetentionTimeMilliSeconds = m_AcquisitionOnRetentionTimeMinutes * 60 * 1000;
            m_AcquisitionOnRetentionTimeNanoSeconds = m_AcquisitionOnRetentionTimeMilliSeconds * 1000 * 1000;

            double ticks = TimeSpan.TicksPerSecond / Rate;
            TimeSpan timeIntervalBetweenTwoDataPoints = new TimeSpan((long)Math.Round(ticks, MidpointRounding.AwayFromZero));
            m_TimeIntervalBetweenTwoDataPointsMinutes = timeIntervalBetweenTwoDataPoints.TotalMinutes;
            m_TimeIntervalBetweenTwoDataPointsMilliSeconds = timeIntervalBetweenTwoDataPoints.TotalMilliseconds;
            m_TimeIntervalBetweenTwoDataPointsNanoSeconds = m_TimeIntervalBetweenTwoDataPointsMilliSeconds * 1000 * 1000;

            Log.WriteLine(Id, "Acquisition On Retention Time " + m_AcquisitionOnRetentionTimeMinutes.ToString() + " = " + Util.GetTimeDisplayText(m_AcquisitionOnRetentionTimeMinutes));

            m_Timer.Enabled = true;
        }

        private void OnCommandAcquisitionOffPreflight(CommandEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnCommandAcquisitionOff(CommandEventArgs args)
        {
            Log.WriteLine(Id);
        }

        private void OnChannelDataFinished(DataFinishedEventArgs args)  // This event is raised shortly after the Acquisition Off command
        {
            Log.WriteLine(Id);
            ChannelDataFinished(true);
        }

        private void ChannelDataFinished(bool isCalledFromOnChannelDataFinished = false)
        {
            if (!m_IsAcquisitionOn)
            {
                return;
            }

            m_IsAcquisitionOn = false;
            m_Timer.Enabled = false;
            AcquisitionTimeOff = DateTime.UtcNow;

            Log.WriteLine(Id, "ChannelAcquisitionState = " + ChannelAcquisitionState.ToString(), CallerMethodName);

            if (!isCalledFromOnChannelDataFinished)
            {
                // Notify Chromeleon that there will be no more data available.
                // This can also be send if the hardware has finished data acquisition.
                m_Channel.NoMoreData();
            }
        }

        private void OnTimerElapsed(object source, System.Timers.ElapsedEventArgs args)
        {
            if (!m_IsAcquisitionOn)
            {
                ChannelDataFinished();
                return;
            }
#if DEBUG
            if (Debugger.IsAttached)
            {
                m_Timer.Enabled = false;
            }
#endif
            ChannelUpdateData();
#if DEBUG
            if (Debugger.IsAttached)
            {
                m_Timer.Enabled = true;
            }
#endif
        }

        private void ChannelUpdateData()
        {
            DateTime timeNow = DateTime.UtcNow;
            DateTime acquisitionOnTime = AcquisitionTimeOn;

            TimeSpan elapsedTime = timeNow - acquisitionOnTime;
            double nextDataPointTimeMilliseconds = m_TimeIntervalBetweenTwoDataPointsMilliSeconds * (m_DataPacketIndex + 1);
            double elapsedTimeTotalMilliseconds = elapsedTime.TotalMilliseconds;
            if (elapsedTimeTotalMilliseconds < nextDataPointTimeMilliseconds)
            {
                return;
            }
            m_DataPacketIndex++;

            //int dataPointsAllCount = (int)Math.Round(elapsedTimeTotalMilliseconds / m_TimeIntervalBetweenTwoDataPointsMilliSeconds, MidpointRounding.AwayFromZero);
            int dataPointsAllCount = (int)(elapsedTimeTotalMilliseconds / m_TimeIntervalBetweenTwoDataPointsMilliSeconds);
            int dataPointsGeneratedCount = m_DataPointIndex + 1;
            int dataPointsToGenerateCount = dataPointsAllCount - dataPointsGeneratedCount;
            if (dataPointsToGenerateCount <= 0)
            {
                DebuggerBreak("dataPointsToGenerateCount = " + dataPointsToGenerateCount.ToString());
                return;
            }

            bool useDataPointDouble = true;
            //bool useDataPointDouble = false;
            if (useDataPointDouble)
            {
                DataPointEx[] dataPoints = new DataPointEx[dataPointsToGenerateCount];
                for (int i = 0; i < dataPoints.Length; i++)
                {
                    double dataPointTimeMilliSeconds = m_AcquisitionOnRetentionTimeMilliSeconds + m_DataPointIndex * m_TimeIntervalBetweenTwoDataPointsMilliSeconds;
                    double dataPoint = Util.GetDataPoint(m_AcquisitionOnRetentionTimeMinutes + m_DataPointIndex * m_TimeIntervalBetweenTwoDataPointsMinutes, m_Number);

                    dataPoints[i] = new DataPointEx(dataPointTimeMilliSeconds, dataPoint);

                    m_DataPointIndex++;
                }
                m_Channel.UpdateDataEx(dataPoints);  // Send the new data to Chromeleon
            }
            else  // long dataPoint
            {
                DataPointInt64[] dataPoints = new DataPointInt64[dataPointsToGenerateCount];
                for (int i = 0; i < dataPoints.Length; i++)
                {
                    long dataPointTimeNanoSeconds = Convert.ToInt64(m_AcquisitionOnRetentionTimeNanoSeconds + m_DataPointIndex * m_TimeIntervalBetweenTwoDataPointsNanoSeconds);
                    long dataPoint = Convert.ToInt64(Util.GetDataPoint(m_AcquisitionOnRetentionTimeMinutes + m_DataPointIndex * m_TimeIntervalBetweenTwoDataPointsMinutes, m_Number));

                    dataPoints[i] = new DataPointInt64(dataPointTimeNanoSeconds, dataPoint);

                    m_DataPointIndex++;
                }
                m_Channel.UpdateDataEx(dataPoints);  // Send the new data to Chromeleon
            }
        }
        #endregion

        #region IDisposable
        private bool m_IsDisposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_Timer != null)
                {
                    m_Timer.Dispose();
                }
            }

            m_IsDisposed = true;
        }
        #endregion
    }
}
