// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using Dionex.Chromeleon.DDK;

namespace MyCompany.Demo
{
    public sealed partial class Driver
    {
        private Demo m_Demo;
        private Heater m_Heater;
        private Pump m_Pump;
        private AutoSampler m_AutoSampler;
        private Detector m_Detector;

        private readonly List<Device> m_Devices = new List<Device>();

        string IDriver.Configuration
        {
            get
            {
                string result;
                if (m_Config != null)
                {
                    result = m_Config.XmlText;
                }
                else
                {
                    result = Config.Driver.DefaultXmlText();
                }
                return result;
            }
            set
            {
                m_Config = new Config.Driver(value);
                m_IsSimulated = m_Config.Demo.IsSimulated;
            }
        }

        void IDriver.Init(IDDK ddk)
        {
            m_DDK = ddk;

            string driverFolder = AppDomain.CurrentDomain.BaseDirectory;
            Log.TaskBegin(Id, "Driver Folder \"" + driverFolder + "\"");
            try
            {
                #region Instrument Data List
                long instrumentsMap = ddk.InstrumentMap.Value;
                if (instrumentsMap == (long)InstrumentID.None)
                    throw new InvalidOperationException("DDK.InstrumentMap = " + instrumentsMap.ToString() + " must be != " + ((long)InstrumentID.None).ToString());

                string instrumentsMapBinary = InstrumentData.GetBitMapBinary(instrumentsMap);
                Log.WriteLine(Id, "DDK.InstrumentMap = " + instrumentsMap.ToString() + " = " + instrumentsMapBinary);

                InstrumentDataList instrumentDataList = new InstrumentDataList();
                instrumentDataList.Init(ddk, instrumentsMap);
                if (instrumentDataList.Count < 1)
                    throw new InvalidOperationException("Failed to get the instruments from the DDK.InstrumentMap = " + instrumentsMap.ToString() + " = " + instrumentsMapBinary);
                #endregion

                m_Demo = new Demo((IDriverEx)this, ddk, m_Config.Demo, Config.Driver.DeviceId.Demo, instrumentDataList, driverFolder);
                AddDevice(m_Demo);

                m_Heater = new Heater((IDriverEx)this, ddk, m_Config.Heater, Config.Driver.DeviceId.Heater);
                AddDevice(m_Heater);

                m_Pump = new Pump((IDriverEx)this, ddk, m_Config.Pump, Config.Driver.DeviceId.Pump);
                AddDevice(m_Pump);

                m_AutoSampler = new AutoSampler((IDriverEx)this, ddk, m_Config.AutoSampler, Config.Driver.DeviceId.AutoSampler);
                AddDevice(m_AutoSampler);

                m_Detector = new Detector((IDriverEx)this, ddk, m_Config.Detector, Config.Driver.DeviceId.Detector);
                AddDevice(m_Detector);

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        private void AddDevice(Device device)
        {
            foreach (Device item in m_Devices)
            {
                if (string.Equals(device.Id, item.Id, StringComparison.CurrentCultureIgnoreCase))
                    throw new ArgumentException("Duplicated device ID \"" + device.Id + "\"");
            }
            m_Devices.Add(device);
        }

        void IDriver.Connect()
        {
            Connect();
        }

        void IDriver.Disconnect()
        {
            Disconnect();
        }

        void IDriver.Exit()
        {
        }
    }
}
