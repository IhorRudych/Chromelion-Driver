// Copyright 2018 Thermo Fisher Scientific Inc.
using System.Globalization;
using System.Xml.Linq;

namespace MyCompany.Demo.Config
{
    internal static class Xml
    {
        public static bool GetElementValueBool(XElement root, string name, bool defaultValue = false, bool createElementIfMissing = true)
        {
            string text = GetElementValueText(root, name, defaultValue.ToString(CultureInfo.InvariantCulture), createElementIfMissing);
            bool result;
            if (!bool.TryParse(text, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static int GetElementValueInt(XElement root, string name, int defaultValue = 0, bool createElementIfMissing = true)
        {
            string text = GetElementValueText(root, name, defaultValue.ToString(CultureInfo.InvariantCulture), createElementIfMissing);
            int result;
            if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static double GetElementValueDouble(XElement root, string name, double defaultValue = 0, bool createElementIfMissing = true)
        {
            string text = GetElementValueText(root, name, defaultValue.ToString(CultureInfo.InvariantCulture), createElementIfMissing);
            double result;
            if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static string GetElementValueText(XElement root, string name, string defaultValue = "", bool createElementIfMissing = true)
        {
            string result;
            XElement element = root.Element(name);
            if (element != null)
            {
                result = element.Value;
            }
            else
            {
                result = defaultValue;
                if (createElementIfMissing)
                {
                    SetElementValue(root, name, result);
                }
            }
            return result;
        }

        public static string SetElementValue(XElement root, string name, bool value)
        {
            string result = value.ToString(CultureInfo.InvariantCulture);
            SetElementValue(root, name, result);
            return result;
        }

        public static string SetElementValue(XElement root, string name, int value)
        {
            string result = value.ToString(CultureInfo.InvariantCulture);
            SetElementValue(root, name, result);
            return result;
        }

        public static string SetElementValue(XElement root, string name, double value)
        {
            string result = value.ToString(CultureInfo.InvariantCulture);
            SetElementValue(root, name, result);
            return result;
        }

        public static void SetElementValue(XElement root, string name, string value)
        {
            XElement element = root.Element(name);
            if (element == null)
            {
                element = new XElement(name);
                root.Add(element);
            }
            element.Value = value;
        }
    }
}
