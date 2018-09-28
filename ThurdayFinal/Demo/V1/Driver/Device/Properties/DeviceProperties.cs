// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class DeviceProperties
    {
        public readonly IStringProperty Description;

        public DeviceProperties(IDDK ddk, IDevice device, string deviceId, string deviceType, string deviceName)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (device == null)
                throw new ArgumentNullException("device");
            if (string.IsNullOrEmpty(deviceType))
                throw new ArgumentNullException("deviceType");
            if (string.IsNullOrEmpty(deviceName))
                throw new ArgumentNullException("deviceName");
            if (deviceName.Length != deviceName.Trim().Length)
                throw new ArgumentException("Device Name is invalid - it starts or ends with space");

            // ModelNo is used to identify the device in the instrument method editor plug-in
            Debug.Assert(StandardPropertyID.ModelNo.ToString() == Property.ConstantName.ModelNo);
            IStringProperty stringProperty = device.CreateStandardProperty(StandardPropertyID.ModelNo, ddk.CreateString(100));
            stringProperty.Update(deviceType);

            stringProperty = Property.CreateString(ddk, device, Property.ConstantName.InternalName);
            stringProperty.Update(deviceId);
            stringProperty.AuditLevel = AuditLevel.Expert;

            stringProperty = Property.CreateString(ddk, device, Property.ConstantName.DeviceType);
            stringProperty.Update(deviceType);

            stringProperty = Property.CreateString(ddk, device, "DeviceName");
            stringProperty.Update(deviceName);

            Description = Property.CreateString(ddk, device, "DeviceDescription");
            Description.Update("Description for " + deviceId);
            // Description.Writeable = true; - not needed, because of m_Properties.DeviceDesription.OnSetProperty += OnPropertyDeviceDesriptionSet;
        }
    }
}
