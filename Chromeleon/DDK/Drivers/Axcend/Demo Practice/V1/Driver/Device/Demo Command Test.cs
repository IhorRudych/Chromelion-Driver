// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal partial class Demo
    {
        private const string m_CommandTest_Param_1_Int = "Param_1_Int";
        private const string m_CommandTest_Param_2_Double = "Param_2_Double";
        private const string m_CommandTest_Param_3_Bool = "Param_3_Bool";
        private const string m_CommandTest_Param_4_String = "Param_4_String";

        private void CommandTestInit()
        {
            ICommand command = m_Device.CreateCommand("CommandTest", "Test command description");
            command.AddParameter(m_CommandTest_Param_1_Int, "Integer > 0", m_DDK.CreateInt(0, 1000));
            command.AddParameter(m_CommandTest_Param_2_Double, "Double Percent [0, 100]", Property.CreatePercentType(m_DDK, 0, 100, 2));
            command.AddParameter(m_CommandTest_Param_3_Bool, "Bool", Property.CreateBoolType(m_DDK));
            command.AddParameter(m_CommandTest_Param_4_String, "String 1 to 20 characters", m_DDK.CreateString(20));
            command.OnPreflightCommand += OnCommandTestPreflight;
            command.OnCommand += OnCommandTest;
        }

        private void GetCommandTestParameters(CommandEventArgs args,
                                              out int intNumber,
                                              out double number,
                                              out bool enable,
                                              out string text)
        {
            intNumber = Property.GetParameterInt(args, m_CommandTest_Param_1_Int);
            number = Property.GetParameterDouble(args, m_CommandTest_Param_2_Double);
            enable = Property.GetParameterBool(args, m_CommandTest_Param_3_Bool);
            text = Property.GetParameterString(args, m_CommandTest_Param_4_String);

            // Also Perform parameter check
            if (intNumber <= 0)
                throw new InvalidOperationException("Parameter " + m_CommandTest_Param_1_Int + " = " + intNumber.ToString() + " must be greater than 0.");
            if (string.IsNullOrEmpty(text))
                throw new InvalidOperationException("Parameter " + m_CommandTest_Param_4_String + " cannot be empty.");
        }

        private void OnCommandTestPreflight(CommandEventArgs args)
        {
            int intNumber;
            double number;
            bool enable;
            string text;
            try
            {
                if (!args.RunContext.IsManual)
                    throw new InvalidOperationException(args.Command.Name + " can be executed by the user in manual mode only");

                GetCommandTestParameters(args, out intNumber, out number, out enable, out text);

                string cmdText = "Command " + args.Command.Name + "(" + intNumber.ToString() + ", " + number.ToString() + ", " + enable.ToString() + ", \"" + text + "\")";
                string userName = Property.GetUserName(args);
                Log.SendCommand(Id, cmdText + (string.IsNullOrEmpty(userName) ? string.Empty : " - requested by " + userName));

                // Display warning to the user. If the user presses the OK button, then the command gets executed
                AuditMessage(AuditLevel.Warning, "Are sure you want to execute " + cmdText);
            }
            catch (Exception ex)
            {
                // All exceptions must be handled with an audit message
                AuditMessage(AuditLevel.Error, string.Format(CultureInfo.CurrentCulture, "Command \"{0}\" error: {1}", args.Command.Name, ex.Message));
            }
        }

        private void OnCommandTest(CommandEventArgs args)
        {
            int intNumber;
            double number;
            bool enable;
            string text;
            try
            {
                m_Driver.CheckIsCommunicating();

                GetCommandTestParameters(args, out intNumber, out number, out enable, out text);

                // Log all requested commands and properties sets
                string cmdText = "Command " + args.Command.Name + "(" + intNumber.ToString() + ", " + number.ToString() + ", " + enable.ToString() + ", \"" + text + "\")";
                string userName = Property.GetUserName(args);
                Log.SendCommand(Id, cmdText + (string.IsNullOrEmpty(userName) ? string.Empty : " - requested by " + userName));

                // Execute the command
            }
            catch (Exception ex)
            {
                // All exceptions must be handled with an audit message
                AuditMessage(AuditLevel.Error, string.Format(CultureInfo.CurrentCulture, "Command {0} error: {1}", args.Command.Name, ex.Message));
            }
        }
    }
}
