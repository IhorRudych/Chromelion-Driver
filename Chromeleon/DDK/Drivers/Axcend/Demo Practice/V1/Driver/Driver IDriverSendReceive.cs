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

            xmlTextResponse = string.Empty;
            xmlTextResponse = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                //Log.TaskEnd(Id, ex);
                //CommandResponse commandResponse = new CommandResponse(commandId, ex);
                //xmlTextResponse = commandResponse.XmlText;
                //// In general ddk.AuditMessage is not need here. Let the UI display the error from the xmlTextResponse.
                //bool showErrorAsAuditTrail = false;
                //if (showErrorAsAuditTrail)
                //{
                //    ddk.AuditMessage(AuditLevel.Error, ex.Message);
                //}
            }
        }
    }
}
