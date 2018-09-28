// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal partial class Demo : Device
    {
        private readonly DemoProperties m_Properties;

        public Demo(IDriverEx driver, IDDK ddk, Config.Demo config, string id, InstrumentDataList instrumentDataList, string driverFolder)
            : base(driver, ddk, config, typeof(Demo).Name, id)
        {
            Log.TaskBegin(Id);
            try
            {
                if (string.IsNullOrEmpty(driverFolder))
                    throw new ArgumentNullException("driverFolder");
                if (instrumentDataList == null)
                    throw new ArgumentNullException("instrumentDataList");

                m_Properties = new DemoProperties(driver, m_DDK, config, m_Device);

                //-------------------------------------------------------------------------------------------------------------------
                if (m_DDK.InstrumentMap == null)
                    throw new InvalidOperationException("DDK.InstrumentMap is Null");
                long instrumentsMap = m_DDK.InstrumentMap.Value;
                if (instrumentsMap == (long)InstrumentID.None)
                    throw new InvalidOperationException("DDK.InstrumentMap = " + instrumentsMap.ToString() + " must be != " + ((long)InstrumentID.None).ToString());
                string instrumentsMapBinary = InstrumentData.GetBitMapBinary(instrumentsMap);
                string instrumentsNames = instrumentDataList.GetNames();

                bool logInFile = LogInFile;
                string fileNamePrefix = instrumentDataList[0].Name;
                Log.Init(Id, logInFile, fileNamePrefix);

                // Logging in a file starts here, if logInFile is True
                const string indent = "\t";
                string text = Environment.NewLine + Environment.NewLine +
                                  indent + "Driver Folder \"" + driverFolder + "\"" + Environment.NewLine +
                                  indent + "Name        \"" + Name + "\", IsSimulated = " + IsSimulated.ToString() + Environment.NewLine +
                                  indent + "USB Address \"" + UsbAddress + "\"" + Environment.NewLine +
                                  indent + "DDK.InstrumentMap = " + instrumentsMap.ToString() + " = " + instrumentsMapBinary + " = " + instrumentsNames +
                              Environment.NewLine;
                Log.WriteLine(Id, text);

                //-------------------------------------------------------------------------------------------------------------------
                m_Properties.LogInFile.OnSetProperty += OnPropertyLogInFileSet;

                //-------------------------------------------------------------------------------------------------------------------
                CommandTestInit();
                CommandStartStopInit();

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }

        public DemoProperties Properties
        {
            [DebuggerStepThrough]
            get { return m_Properties; }
        }

        public string UsbAddress
        {
            get { return m_Properties.FirmwareUsbAddress.Value; }
        }

        public string FirmwareVersion
        {
            get { return m_Properties.FirmwareVersion.Value; }
        }

        public string SerialNo
        {
            get { return m_Properties.SerialNo.Value; }
        }

        public bool LogInFile
        {
            get { return Property.GetBool(m_Properties.LogInFile.Value.GetValueOrDefault()); }
            private set
            {
                if (LogInFile == value && Log.LogInFile == value)
                {
                    return;
                }
                m_Properties.LogInFile.Update(Property.GetBoolNumber(value));
                Log.PropertyChanged(Id, m_Properties.LogInFile, CallerMethodName);
                Log.InitFileLogging(Id, value);
            }
        }

        private void OnPropertyLogInFileSet(SetPropertyEventArgs args)
        {
            bool enable = Property.GetBool(args);
            LogInFile = enable;
        }
    }
}
