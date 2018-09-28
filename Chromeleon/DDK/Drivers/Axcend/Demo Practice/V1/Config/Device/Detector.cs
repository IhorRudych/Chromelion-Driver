// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    public class Detector : Device
    {
        public readonly static int ChannelsNumberMin = 1;
        public readonly static int ChannelsNumberMax = 3;

        private static class Element
        {
            public const string ChannelsNumber = "ChannelsNumber";
        }

        private int m_ChannelsNumber;

        public Detector(XElement docRoot, string id)
            : base(docRoot, id)
        {
            m_ChannelsNumber = Xml.GetElementValueInt(Root, Element.ChannelsNumber, ChannelsNumberMax);
        }

        public int ChannelsNumber
        {
            [DebuggerStepThrough]
            get { return m_ChannelsNumber; }
            set
            {
                if (m_ChannelsNumber == value)
                    return;

                if (value < ChannelsNumberMin || value > ChannelsNumberMax)
                    throw new InvalidOperationException("Cannot set ChannelsNumber = " + value.ToString() + ". " +
                                                        "Must in [" + ChannelsNumberMin.ToString() + ", " +
                                                                      ChannelsNumberMax.ToString() + "]");
                Xml.SetElementValue(Root, Element.ChannelsNumber, value);
                m_ChannelsNumber = value;
            }
        }
    }
}
