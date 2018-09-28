// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using MyCompany.Demo.Properties;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DemoProperties
    {
        private readonly IIntProperty m_IsSimulated;

        public readonly IIntProperty LogInFile;

        public readonly IStringProperty FirmwareUsbAddress;
        public readonly IStringProperty FirmwareVersion;
        public readonly IStringProperty SerialNo;

        public readonly IStringProperty TaskName;
        public readonly IStringProperty TaskNamePrevious;
        public readonly IStringProperty TaskPreviousErrorText;

        public readonly IIntProperty CommunicationState;
        public readonly IIntProperty BehaviorStatePrev;
        public readonly IIntProperty BehaviorState;

        public readonly IIntProperty ProcessStep;

        public DemoProperties(IDriverEx driver, IDDK ddk, Config.Demo config, IDevice device)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (config == null)
                throw new ArgumentNullException("config");
            if (device == null)
                throw new ArgumentNullException("device");

            LogInFile = Property.CreateBool(ddk, device, "LogInFile");
            LogInFile.Writeable = true;
            LogInFile.AuditLevel = AuditLevel.Service;
            LogInFile.Update(Property.GetBoolNumber(true));  // Can be configurable = config.LogInFile

            m_IsSimulated = Property.CreateBool(ddk, device, "IsSimulated");
            m_IsSimulated.Update(Property.GetBoolNumber(driver.IsSimulated));
            if (driver.IsSimulated != config.IsSimulated)
            {
                Device.DebuggerBreak();
            }

            FirmwareUsbAddress = Property.CreateString(ddk, device, "FirmwareUsbAddress");
            FirmwareUsbAddress.Update(driver.FirmwareUsbAddress);

            FirmwareVersion = Property.CreateString(ddk, device, "FirmwareVersion");
            FirmwareVersion.Update(driver.FirmwareVersion);

            SerialNo = device.CreateStandardProperty(StandardPropertyID.SerialNo, ddk.CreateString(100));
            SerialNo.Update(driver.SerialNo);

            TaskName = Property.CreateString(ddk, device, "TaskName", driver.TaskName);
            TaskNamePrevious = Property.CreateString(ddk, device, "TaskNamePrevious", driver.TaskNamePrevious);
            TaskPreviousErrorText = Property.CreateString(ddk, device, "TaskPreviousErrorText", driver.TaskPreviousErrorText);

            CommunicationState = Property.CreateEnum(ddk, device, "CommunicationState", driver.CommunicationState, null);
            BehaviorStatePrev = Property.CreateEnum(ddk, device, "BehaviorStatePrev", driver.BehaviorStatePrev);
            BehaviorState = Property.CreateEnum(ddk, device, "BehaviorState", driver.BehaviorState);

            ProcessStep = Property.CreateInt(ddk, device, "ProcessStep");
            ProcessStep.Update(0);
        }
    }
}
