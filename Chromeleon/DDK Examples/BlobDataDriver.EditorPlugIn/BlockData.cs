using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Dionex.DDK.V2.BlobDataDriver.EditorPlugIn.Properties;

namespace Dionex.DDK.V2.BlobDataDriver.EditorPlugIn
{
    /// <summary>
    /// A class representing your data.
    /// 
    /// This class also provides functions to serialize / deserialize data to / from a binary format
    /// and it provides serializing to Chromeleon formatted data output.
    /// </summary>
    internal class BlockData
    {
        #region Construction

        /// <summary>
        /// Constructs data with default values.
        /// Used in wizard context.
        /// </summary>
        public BlockData()
        {
            NumericValue = 1.234;
            Text = "Wizard generated default Text";
        }

        /// <summary>
        /// Constructs data by deserializing it from a binary data block.
        /// </summary>
        /// <param name="binaryData"></param>
        public BlockData(byte[] binaryData)
        {
            Deserialize(binaryData);
        }
        #endregion

        #region Public Members

        /// <summary>
        /// Gets / sets a numeric value.
        /// </summary>
        public double NumericValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets / sets a text value.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Deserializes data from a binary data block.
        /// </summary>
        /// <param name="binaryData"></param>
        public void Deserialize(byte[] binaryData)
        {
            using (var stream = new MemoryStream(binaryData))
            {
                var xmlDocument = XDocument.Load(stream);
                Text = xmlDocument.Root.Element("Text").Value;
                NumericValue = double.Parse(xmlDocument.Root.Element("NumericValue").Value, NumberFormatInfo.InvariantInfo);
            }
        }

        /// <summary>
        /// Serializes data to a binary data block.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            var xmlDoc = new XDocument();
            var root = new XElement("Data");
            xmlDoc.Add(root);
            root.Add(new XElement("Text", Text));
            root.Add(new XElement("NumericValue", NumericValue));

            var sb = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;

            using (var writer = XmlWriter.Create(sb, settings))
            {
                xmlDoc.WriteTo(writer);
            }

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Creates Chromeleon specific formatted data.
        /// </summary>
        /// <param name="localize">True to localize display names and values. False otherwise.</param>
        /// <returns>The string containing a Chromeleon compatible formatted string.</returns>
        public string CreateFormattedData(bool localize)
        {
            var xmlDoc = new XDocument();
            var root = new XElement("CmFormattedData");
            xmlDoc.Add(root);

            var deviceObject = CreateObject("Device Properties", "MyDeviceType", "MyDeviceID");
            root.Add(deviceObject);

            if (localize)
            {
                var textProperty = CreatePropertyWithValue(Resources.TextPropertyDisplayName, Text);
                deviceObject.Add(textProperty);
            }
            else
            {
                var textProperty = CreatePropertyWithValue("Text", Text);
                deviceObject.Add(textProperty);
            }

            if (localize)
            {
                var numProperty = CreatePropertyWithValue(Resources.NumericValueDisplayName, NumericValue.ToString(NumberFormatInfo.InvariantInfo));
                deviceObject.Add(numProperty);
            }
            else
            {
                var numProperty = CreatePropertyWithValue("Numerical Value", NumericValue.ToString(NumberFormatInfo.CurrentInfo));
                deviceObject.Add(numProperty);
            }

            var sb = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;

            using (var writer = XmlWriter.Create(sb, settings))
            {
                xmlDoc.WriteTo(writer);
            }

            return sb.ToString();
        }
        #endregion

        #region Formatted Data Helpers

        /// <summary>
        /// Creates an object element with a given display name a type and an instance ID
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="type">The object type.</param>
        /// <param name="id">The instance ID.</param>
        /// <returns></returns>
        private XElement CreateObject(string displayName, string type, string id)
        {
            var objectElement = new XElement("Object");
            objectElement.SetAttributeValue("type", type);
            objectElement.SetAttributeValue("id", id);

            var displayNameElement = new XElement("DisplayName");
            displayNameElement.SetAttributeValue("value", displayName);
            objectElement.Add(displayNameElement);

            return objectElement;
        }

        /// <summary>
        /// Creates an property element with a given display name and a value
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private XElement CreatePropertyWithValue(string displayName, string value)
        {
            var propertyElement = new XElement("Property");
            var displayNameElement = new XElement("DisplayName");
            displayNameElement.SetAttributeValue("value", displayName);
            propertyElement.Add(displayNameElement);

            if (value != null)
            {
                var valueElement = new XElement("Value");
                valueElement.SetAttributeValue("value", value);
                propertyElement.Add(valueElement);
            }

            return propertyElement;
        }
        #endregion
    }
}
