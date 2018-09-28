// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    public abstract class Device
    {
        #region Fields
        protected readonly XElement m_Root;

        private readonly string m_Id;
        private string m_Type;
        private string m_Name;

        private static class Attribute
        {
            public const string Id = "Id";
        }

        private static class Element
        {
            public const string Device = "Device";
            public const string Name = "Name";
        }
        #endregion

        #region Constructor
        protected Device(XElement docRoot, string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (docRoot == null)
                throw new ArgumentNullException("docRoot");

            m_Id = id;

            m_Root = docRoot.Elements(Element.Device).FirstOrDefault(item =>
            {
                XAttribute attribute = item.Attribute(Attribute.Id);
                string value = attribute != null ? attribute.Value : null;
                return value == Id;
            });
            if (m_Root == null)
            {
                m_Root = new XElement(Element.Device);
                Root.Add(new XAttribute(Attribute.Id, Id));
                docRoot.Add(Root);
            }

            string name = m_Id.Replace('-', '_');
            const string idSuffix = "_Id";
            if (name.EndsWith(idSuffix))
            {
                name = name.Substring(0, name.Length - idSuffix.Length);
            }
            name += "_Name";

            m_Name = Xml.GetElementValueText(Root, Element.Name, name);
        }
        #endregion

        #region Properties
        protected XElement Root
        {
            [DebuggerStepThrough]
            get { return m_Root; }
        }

        public string Id
        {
            [DebuggerStepThrough]
            get { return m_Id; }
        }

        public string Type
        {
            [DebuggerStepThrough]
            get { return m_Type; }
            set
            {
                m_Type = value;
            }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return m_Name; }
            set
            {
                if (m_Name == value)
                {
                    return;
                }
                Xml.SetElementValue(Root, Element.Name, value);
                m_Name = value;
            }
        }
        #endregion
    }
}
