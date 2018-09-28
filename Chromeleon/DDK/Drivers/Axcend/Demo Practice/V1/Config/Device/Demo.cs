// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    public class Demo : Device
    {
        private static class Element
        {
            public const string IsSimulated = "IsSimulated";
            public const string FirmwareUsbAddress = "FirmwareUsbAddress";
            public const string FirmwareVersion = "FirmwareVersion";
            public const string SerialNo = "SerialNo";
        }

        private bool m_IsSimulated;
        private string m_FirmwareUsbAddress;
        private string m_FirmwareVersion;
        private string m_SerialNo;

        public Demo(XElement docRoot, string id)
            : base(docRoot, id)
        {
            m_IsSimulated = Xml.GetElementValueBool(Root, Element.IsSimulated, true);
            m_FirmwareUsbAddress = Xml.GetElementValueText(Root, Element.FirmwareUsbAddress, "USB-1234567890AB");
        }

        public bool IsSimulated
        {
            [DebuggerStepThrough]
            get { return m_IsSimulated; }
            set
            {
                if (m_IsSimulated == value)
                    return;
                Xml.SetElementValue(Root, Element.IsSimulated, value);
                m_IsSimulated = value;
            }
        }

        public string FirmwareUsbAddress
        {
            [DebuggerStepThrough]
            get { return m_FirmwareUsbAddress; }
            set
            {
                if (m_FirmwareUsbAddress == value)
                    return;
                Xml.SetElementValue(Root, Element.FirmwareUsbAddress, value);
                m_FirmwareUsbAddress = value;
            }
        }

        public string FirmwareVersion
        {
            [DebuggerStepThrough]
            get { return m_FirmwareVersion; }
            set
            {
                if (m_FirmwareVersion == value)
                    return;
                Xml.SetElementValue(Root, Element.FirmwareVersion, value);
                m_FirmwareVersion = value;
            }
        }

        public string SerialNo
        {
            [DebuggerStepThrough]
            get { return m_SerialNo; }
            set
            {
                if (m_SerialNo == value)
                    return;
                Xml.SetElementValue(Root, Element.SerialNo, value);
                m_SerialNo = value;
            }
        }
    }
}
