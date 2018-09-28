// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal partial class Demo
    {
        // private const string m_CommandTest_Param_1_Int = "Param_1_Int";
        private void CommandTransferData()
        {
            ICommand command = m_Device.CreateCommand("CommandTest1", "Test command description");
            command.AddParameter("CommandTest1", "Test command description nice", m_DDK.CreateDouble(0, 1000, 3));
            command.OnPreflightCommand += OnCommandTestPreflight;
            //command.OnCommand += O
        }
    }
}
