// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class ChannelProperties
    {

        public ChannelProperties(IDDK ddk, IChannel channel)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            if (channel == null)
                throw new ArgumentNullException("channel");

        }
    }
}
