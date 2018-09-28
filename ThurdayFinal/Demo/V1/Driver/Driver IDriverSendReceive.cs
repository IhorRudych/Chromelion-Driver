// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Threading;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    public sealed partial class Driver
    {
        void IDriverSendReceive.OnSendReceive(IDDK ddk, string xmlTextRequest, out string xmlTextResponse)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");

            XmlCommand.CommandId commandId = XmlCommand.CommandId.Unknown;
            xmlTextResponse = string.Empty;
            try
            {
                bool testError = false;
                if (testError)
                    throw new Exception("IDriverSendReceive.OnSendReceive test error");

                CommandRequest commandRequest = new CommandRequest(xmlTextRequest);
                commandId = commandRequest.Id;
                bool configIsSimulated = commandRequest.IsSimulated;

                switch (commandId)
                {
                    case XmlCommand.CommandId.GetIsIdle:
                        {
                            bool isIdle;
                            if (configIsSimulated)
                            {
                                isIdle = true;
                            }
                            else
                            {
                                isIdle = true;
                            }
                            CommandGetIsIdle command = new CommandGetIsIdle(commandRequest.XmlText, isIdle);
                            xmlTextResponse = command.XmlText;
                            break;
                        }
                    case XmlCommand.CommandId.GetFirmwareData:
                        {
                            if (IsSimulated == configIsSimulated)
                            {
                                if (!IsCommunicating)
                                {
                                    Connect();
                                }
                            }
                            else
                            {
                                if (configIsSimulated)  // IsSimulated is False
                                {
                                    SetSimulatedFirmwareData();
                                }
                                else  // IsSimulated is True
                                {
                                    // Open communication - this must set the FirmwareUsbAddress
                                    try
                                    {
                                        // Send commands to get whatever data is needed, in this case: FirmwareVersion and SerialNo
                                    }
                                    finally
                                    {
                                        // Close communication
                                    }
                                }
                            }

                            CommandGetFirmwareData command = new CommandGetFirmwareData(commandRequest.XmlText, FirmwareUsbAddress, FirmwareVersion, SerialNo);
                            xmlTextResponse = command.XmlText;

                            // Wait for while to test how the configuration UI handles these cases
                            Thread.Sleep(3 * 1000);
                            break;
                        }
                    default:
                        throw new Exception("Unknown command: \"" + commandId.ToString() + "\"");
                }
                Log.TaskEnd(Id, "Response = \"" + xmlTextResponse + "\"");
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                CommandResponse commandResponse = new CommandResponse(commandId, ex);
                xmlTextResponse = commandResponse.XmlText;
                // In general ddk.AuditMessage is not need here. Let the UI display the error from the xmlTextResponse.
                bool showErrorAsAuditTrail = false;
                if (showErrorAsAuditTrail)
                {
                    ddk.AuditMessage(AuditLevel.Error, ex.Message);
                }
            }
        }
    }
}
