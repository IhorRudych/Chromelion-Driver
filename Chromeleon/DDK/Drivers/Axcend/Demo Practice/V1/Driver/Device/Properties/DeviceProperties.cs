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
        }
    }
}
