// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    public class Heater : Device
    {
        private static class Element
        {
            public const string ProductDescription = "ProductDescription";
        }

        private string m_ProductDescription;

        public Heater(XElement docRoot, string id)
            : base(docRoot, id)
        {
            m_ProductDescription = Xml.GetElementValueText(Root, Element.ProductDescription, "Heat");
        }

        public string ProductDescription
        {
            [DebuggerStepThrough]
            get { return m_ProductDescription; }
            set
            {
                if (m_ProductDescription == value)
                    return;
                Xml.SetElementValue(Root, Element.ProductDescription, value);
                m_ProductDescription = value;
            }
        }
    }
}
