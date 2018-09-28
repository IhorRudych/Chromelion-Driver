// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace MyCompany
{
    public abstract class XmlCommand
    {
        public enum CommandId
        {
            Unknown,
            GetIsIdle,
            GetFirmwareData,
        }

        private static class Tag
        {
            public static readonly string Command = "Command";
            public static readonly string Name = "Name";
            public static readonly string IsSimulated = "IsSimulated";

            public static readonly string Request = "Request";
            public static readonly string Response = "Response";

            public static readonly string Exception = "Exception";
            public static readonly string Type = "Type";
            public static readonly string Text = "Text";
        }

        public readonly CommandId Id;

        // Request
        public readonly bool IsSimulated;

        // Response
        public readonly string ExceptionType;
        public readonly string ExceptionText;

        private readonly XDocument m_Doc;
        private readonly XElement m_Command;
        protected readonly XElement m_Request;
        protected readonly XElement m_Response;

        private XmlCommand(CommandId commandId)
        {
            Id = commandId;
            m_Doc = new XDocument();
        }

        protected XmlCommand(CommandId commandId, Exception ex)
            : this(commandId)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            ExceptionType = ex.GetType().FullName;
            ExceptionText = ex.Message;

            m_Command = new XElement(Tag.Command);
            m_Doc.Add(m_Command);
            m_Command.Add(new XElement(Tag.Name, commandId.ToString()));

            m_Response = new XElement(Tag.Response);
            m_Command.Add(m_Response);

            XElement exceptionElement = new XElement(Tag.Exception);
            m_Response.Add(exceptionElement);
            exceptionElement.Add(new XElement(Tag.Type, ExceptionType));
            exceptionElement.Add(new XElement(Tag.Text, ExceptionText));
        }

        protected XmlCommand(CommandId commandId, bool isSimulated)  // Request
            : this(commandId)
        {
            IsSimulated = isSimulated;

            m_Command = new XElement(Tag.Command);
            m_Doc.Add(m_Command);
            m_Command.Add(new XElement(Tag.Name, commandId.ToString()));
            m_Command.Add(new XElement(Tag.IsSimulated, isSimulated.ToString(CultureInfo.InvariantCulture)));

            m_Request = new XElement(Tag.Request);
            m_Command.Add(m_Request);
        }

        protected XmlCommand(string xmlText)  // Response
        {
            if (string.IsNullOrEmpty(xmlText))
                throw new ArgumentNullException("xmlText");

            m_Doc = XDocument.Parse(xmlText);

            m_Command = m_Doc.Root;
            if (m_Command.Name != Tag.Command)
                throw new InvalidOperationException("Invalid XML text request: " + Environment.NewLine + xmlText);

            XElement element = m_Command.Element(Tag.Name);
            if (element == null)
                throw new InvalidOperationException("Cannot find command tag \"" + Tag.Name + "\"");
            string commandName = element.Value;
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentNullException("Command Name is empty");

            // .Net 3.5
            if (!Enum.IsDefined(typeof(CommandId), commandName))
                throw new InvalidOperationException("Cannot find command name \"" + commandName + "\" in " + typeof(CommandId).FullName + " names");
            Id = (CommandId)Enum.Parse(typeof(CommandId), commandName, true);
            //if (!Enum.TryParse<CommandId>(commandName, true, out Id))
            //    throw new InvalidOperationException("Cannot convert command name \"" + commandName + "\" to " + typeof(CommandId).FullName);

            m_Request = m_Command.Element(Tag.Request);
            if (m_Request == null)
            {
                m_Request = new XElement(Tag.Request);
                m_Command.Add(m_Request);
            }

            m_Response = m_Command.Element(Tag.Response);
            if (m_Response == null)
            {
                m_Response = new XElement(Tag.Response);
                m_Command.Add(m_Response);
            }
            else
            {
                XElement exceptionElement = m_Response.Element(Tag.Exception);
                if (exceptionElement != null)
                {
                    element = exceptionElement.Element(Tag.Type);
                    if (element != null)
                    {
                        ExceptionType = element.Value;
                    }

                    element = exceptionElement.Element(Tag.Text);
                    if (element != null)
                    {
                        ExceptionText = element.Value;
                    }
                }
            }

            bool hasError = !string.IsNullOrEmpty(ExceptionType) || !string.IsNullOrEmpty(ExceptionText);
            if (!hasError)
            {
                element = m_Command.Element(Tag.IsSimulated);
                if (element == null)
                    throw new InvalidOperationException("Cannot find tag \"" + Tag.IsSimulated + "\"");
                if (string.IsNullOrEmpty(element.Value))
                    throw new InvalidOperationException("Tag \"" + Tag.IsSimulated + "\" value is empty");
                IsSimulated = bool.Parse(element.Value);
            }
        }

        public string XmlText
        {
            [DebuggerStepThrough]
            get { return m_Doc.ToString(); }
        }

        protected static void AddParameter(XElement requestResponse, string name, bool value)
        {
            AddParameter(requestResponse, name, value.ToString(CultureInfo.InvariantCulture));
        }

        protected static void AddParameter(XElement requestResponse, string name, int value)
        {
            AddParameter(requestResponse, name, value.ToString(CultureInfo.InvariantCulture));
        }

        protected static void AddParameter(XElement requestResponse, string name, double value)
        {
            AddParameter(requestResponse, name, value.ToString(CultureInfo.InvariantCulture));
        }

        protected static void AddParameter(XElement requestResponse, string name, string value = null)
        {
            if (requestResponse == null)
                throw new ArgumentNullException("requestResponse");
            requestResponse.Add(new XElement(name, value));
        }

        public void AddException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            if (m_Response == null)
                throw new ArgumentNullException("Exception can be added in a response only");


            string exType = ex.GetType().FullName;
            string exText = ex.Message;

            XElement exception = new XElement(Tag.Exception);
            exception.Add(new XElement(Tag.Type, exType));
            exception.Add(new XElement(Tag.Text, exText));

            m_Response.Add(exception);
        }
    }

    public class CommandRequest : XmlCommand
    {
        public CommandRequest(string xmlText)
            : base(xmlText)
        {
        }
    }

    public class CommandResponse : XmlCommand
    {
        public CommandResponse(string xmlText)
            : base(xmlText)
        {
            if (!string.IsNullOrEmpty(ExceptionText))
                throw new Exception(ExceptionText);
            if (!string.IsNullOrEmpty(ExceptionType))
                throw new Exception(ExceptionType);
        }

        public CommandResponse(CommandId commandId, Exception ex)
            : base(commandId, ex)
        {
        }
    }

    public class CommandGetIsIdle : XmlCommand
    {
        private class ParamName
        {
            public static readonly string IsIdle = "IsIdle";
        }

        public readonly bool IsIdle;

        public CommandGetIsIdle(bool isSimulated)
            : base(CommandId.GetIsIdle, isSimulated)
        {
        }

        public CommandGetIsIdle(string xmlText, bool isIdle)
            : base(xmlText)
        {
            IsIdle = isIdle;
            AddParameter(m_Response, ParamName.IsIdle, IsIdle);
        }

        public CommandGetIsIdle(string xmlText)
            : base(xmlText)
        {
            XElement element = m_Response.Element(ParamName.IsIdle);
            if (element != null && !string.IsNullOrEmpty(element.Value))
            {
                IsIdle = bool.Parse(element.Value);
            }
        }
    }

    public class CommandGetFirmwareData : XmlCommand
    {
        private class ParamName
        {
            public static readonly string FirmwareUsbAddress = "FirmwareUsbAddress";
            public static readonly string FirmwareVersion = "FirmwareVersion";
            public static readonly string SerialNo = "SerialNo";
        }

        public readonly string FirmwareUsbAddress;
        public readonly string FirmwareVersion;
        public readonly string SerialNo;

        public CommandGetFirmwareData(bool isSimulated)
            : base(CommandId.GetFirmwareData, isSimulated)
        {
        }

        public CommandGetFirmwareData(string xmlText, string firmwareUsbAddress, string firmwareVersion, string serialNo)
            : base(xmlText)
        {
            FirmwareUsbAddress = firmwareUsbAddress;
            FirmwareVersion = firmwareVersion;
            SerialNo = serialNo;

            AddParameter(m_Response, ParamName.FirmwareUsbAddress, FirmwareUsbAddress);
            AddParameter(m_Response, ParamName.FirmwareVersion, FirmwareVersion);
            AddParameter(m_Response, ParamName.SerialNo, SerialNo);
        }

        public CommandGetFirmwareData(string xmlText)
            : base(xmlText)
        {
            XElement element = m_Response.Element(ParamName.FirmwareUsbAddress);
            if (element == null)
                throw new InvalidOperationException("Parameter " + ParamName.FirmwareUsbAddress + " is not found");
            FirmwareUsbAddress = element.Value;

            element = m_Response.Element(ParamName.FirmwareVersion);
            if (element == null)
                throw new InvalidOperationException("Parameter " + ParamName.FirmwareVersion + " is not found");
            FirmwareVersion = element.Value;

            element = m_Response.Element(ParamName.SerialNo);
            if (element == null)
                throw new InvalidOperationException("Parameter " + ParamName.SerialNo + " is not found");
            SerialNo = element.Value;
        }
    }
}
