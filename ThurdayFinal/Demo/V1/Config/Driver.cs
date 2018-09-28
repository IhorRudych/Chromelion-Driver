// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dionex.Chromeleon.DDK;

namespace MyCompany.Demo.Config
{
    public class Driver
    {
        #region Fields
        public const string Id = "MyCompany.Demo";

        public class DeviceId
        {
            public static readonly string Demo = "Demo_Id";
            public static readonly string Heater = "Heater_Id";
            public static readonly string Pump = "Pump_Id";
            public static readonly string AutoSampler = "AutoSampler_Id";
            public static readonly string Detector = "Detector_Id";
        }

        private readonly XDocument m_Doc;
        private readonly XElement m_Root;

        private readonly Demo m_Demo;
        private readonly Heater m_Heater;
        private readonly Pump m_Pump;
        private readonly AutoSampler m_AutoSampler;
        private readonly Detector m_Detector;

        private readonly List<Device> m_Devices;

        private string m_Description;

        #region XML Tags
        private static class Attribute
        {
            public const string Id = "Id";
        }

        private static class Element
        {
            public const string Configuration = "Configuration";
            public const string Driver = "Driver";
            public const string Common = "Common";
            public const string Description = "Description";
        }
        #endregion
        #endregion

        #region Constructor
        public Driver(string xmlText)
        {
            m_Devices = new List<Device>();

            if (string.IsNullOrEmpty(xmlText))
            {
                xmlText = DefaultXmlText();
            }

            m_Doc = XDocument.Parse(xmlText);
            XElement configElement = m_Doc.Element(Element.Configuration);
            if (configElement == null)
                throw new ArgumentException("Cannot find the root element \"" + Element.Configuration + "\"");

            XElement driverElement = configElement.Element(Element.Driver);
            if (driverElement == null)
            {
                driverElement = new XElement(Element.Driver);
                driverElement.Add(new XAttribute(Attribute.Id, Id));
                configElement.Add(driverElement);
            }

            m_Root = driverElement.Element(Element.Common);
            if (m_Root == null)
            {
                m_Root = new XElement(Element.Common);
                driverElement.Add(m_Root);
            }

            m_Description = Xml.GetElementValueText(m_Root, Element.Description, "This is an example driver", true);

            m_Demo = new Demo(driverElement, DeviceId.Demo);
            m_Devices.Add(m_Demo);

            m_Heater = new Heater(driverElement, DeviceId.Heater);
            m_Devices.Add(m_Heater);

            m_Pump = new Pump(driverElement, DeviceId.Pump);
            m_Devices.Add(m_Pump);

            m_AutoSampler = new AutoSampler(driverElement, DeviceId.AutoSampler);
            m_Devices.Add(m_AutoSampler);

            m_Detector = new Detector(driverElement, DeviceId.Detector);
            m_Devices.Add(m_Detector);

            m_Devices.Sort((Device device1, Device device2) => device1.Id.CompareTo(device2.Id));
        }
        #endregion

        #region Get Default XML Text
        public static string DefaultXmlText()
        {
            XDocument doc = XDocument.Parse("<" + Element.Configuration + ">" + Environment.NewLine + "</" + Element.Configuration + ">");
            XElement configElement = doc.Element(Element.Configuration);
            if (configElement == null)
                throw new ArgumentException("Cannot find the root element \"" + Element.Configuration + "\"");

            XElement driverElement = configElement.Element(Element.Driver);
            if (driverElement == null)
            {
                driverElement = new XElement(Element.Driver);
                driverElement.Add(new XAttribute(Attribute.Id, Id));
                configElement.Add(driverElement);
            }

            string result = doc.ToString();
            /*
            <Configuration>
              <Driver Id="MyCompany.Driver-Demo">
              </Driver>
            </Configuration>
            */
            return result;
        }
        #endregion

        #region Properties
        public string ID
        {
            [DebuggerStepThrough]
            get { return Id; }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return ID; }
        }

        public string XmlText
        {
            [DebuggerStepThrough]
            get { return m_Doc.ToString(); }
        }

        public string Description
        {
            get { return m_Description; }
            set
            {
                Xml.SetElementValue(m_Root, Element.Description, value);
                m_Description = value;
            }
        }

        public Demo Demo
        {
            [DebuggerStepThrough]
            get { return m_Demo; }
        }

        public Heater Heater
        {
            [DebuggerStepThrough]
            get { return m_Heater; }
        }

        public Pump Pump
        {
            [DebuggerStepThrough]
            get { return m_Pump; }
        }

        public AutoSampler AutoSampler
        {
            [DebuggerStepThrough]
            get { return m_AutoSampler; }
        }

        public Detector Detector
        {
            [DebuggerStepThrough]
            get { return m_Detector; }
        }

        public string GetReportText(IInstrumentInfo instrumentInfo, long instrumentsMap)
        {
            if (instrumentInfo == null)
                throw new ArgumentNullException("instrumentInfo");

            const string sprt = "\t";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Driver     " + sprt + Id);
            sb.AppendLine("Description" + sprt + Description);
            sb.AppendLine("IsSimulated" + sprt + Demo.IsSimulated.ToString());
            sb.AppendLine("Firmware Version" + sprt + Demo.FirmwareVersion);
            sb.AppendLine("Serial Number" + sprt + Demo.SerialNo);
            sb.AppendLine("USB Address" + sprt + Demo.FirmwareUsbAddress);
            sb.AppendLine();

            AddReportDevice(sb, sprt, Demo);
            sb.AppendLine("Name" + sprt + Demo.Name);
            sb.AppendLine();

            AddReportDevice(sb, sprt, Heater);
            sb.AppendLine("Product Description" + sprt + Heater.ProductDescription);
            sb.AppendLine();

            AddReportDevice(sb, sprt, Detector);
            sb.AppendLine("Property B" + sprt + Detector.ChannelsNumber.ToString(CultureInfo.InvariantCulture));

            InstrumentDataList instruments = new InstrumentDataList(instrumentInfo, instrumentsMap);
            if (instruments.Count > 1)
            {
                string instrumentsText = instruments.GetNames();
                AddReportParam(sb, sprt, "Shared in instruments", instrumentsText);
            }
            sb.AppendLine();

            string result = sb.ToString();
            return result;
        }

        private static void AddReportDevice(StringBuilder sb, string sprt, Device device)
        {
            sb.AppendLine("Device" + sprt + device.Id);
            sb.AppendLine("Type" + sprt + device.Type);
            sb.AppendLine("Name" + sprt + device.Name);
        }

        private static void AddReportParam(StringBuilder sb, string sprt, string name, string value)
        {
            sb.AppendLine(sprt + sprt + name + sprt + value);
        }
        #endregion

        #region Validate Configuration
        public void Check()
        {
            List<string> names = m_Devices.Select(item => item.Name).ToList();
            List<string> namesDistinct = names.Distinct().ToList();
            if (names.Count != namesDistinct.Count)
            {
                throw new InvalidDataException("Duplicate device name");
            }
        }
        #endregion
    }
}
