// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.Helpers;

namespace MyCompany.Demo
{
    #region Command Names
    public static class CommandName
    {
        public static readonly string AutoZero = "AutoZero";
        public static readonly string Wait = "Wait";
    }
    #endregion

    #region Symbol Names
    public static class SymbolName
    {
        public static readonly string Ready = "Ready";
    }
    #endregion

    public class Util
    {
        #region Fields
        private readonly IPage m_Page;
        private readonly IEditMethod m_EditMethod;
        private readonly ISymbol m_DeviceSymbol;

        private readonly Stages m_Stage;
        private readonly Properties m_Property;
        private readonly Commands m_Command;
        #endregion

        #region Constructor
        public Util(IPage page, IEditMethod editMethod)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            if (editMethod == null)
                throw new ArgumentNullException("m_EditMethod");

            m_Page = page;
            m_EditMethod = editMethod;
            m_DeviceSymbol = m_Page.Component.Symbol;

            m_Property = new Properties(m_Page, m_EditMethod, m_DeviceSymbol);
            m_Command = new Commands(m_Page, m_EditMethod, m_DeviceSymbol);
            m_Stage = new Stages(m_Page, m_EditMethod, m_DeviceSymbol, m_Property, m_Command);
        }
        #endregion

        #region Properties
        public Properties Property
        {
            [DebuggerStepThrough]
            get { return m_Property; }
        }

        public Commands Command
        {
            [DebuggerStepThrough]
            get { return m_Command; }
        }

        public Stages Stage
        {
            [DebuggerStepThrough]
            get { return m_Stage; }
        }
        #endregion

        #region Static Functions
        public static bool GetBool(Nullable<int> value)
        {
            return GetBool(value.GetValueOrDefault());
        }

        public static bool GetBool(int value)
        {
            return value != 0;
        }

        public static int GetBoolNumber(bool value)
        {
            return value ? 1 : 0;
        }

        public static bool GetBool(INumericProperty property)
        {
            return property.Value.GetValueOrDefault() != 0;
        }

        public static string GetSymbolPath(IPage page, string symbolName)
        {
            return GetSymbolPath(page.Component.Symbol.Path, symbolName);
        }

        public static string GetSymbolPath(string symbolPath, string symbolName)
        {
            return symbolPath + "." + symbolName;
        }
        #endregion

        #region Properties
        public class Properties
        {
            private readonly IPage m_Page;
            private readonly IEditMethod m_EditMethod;
            private readonly ISymbol m_DeviceSymbol;

            public Properties(IPage page, IEditMethod editMethod, ISymbol deviceSymbol)
            {
                if (page == null)
                    throw new ArgumentNullException("page");
                if (editMethod == null)
                    throw new ArgumentNullException("m_EditMethod");
                if (deviceSymbol == null)
                    throw new ArgumentNullException("deviceSymbol");

                m_Page = page;
                m_EditMethod = editMethod;
                m_DeviceSymbol = deviceSymbol;
            }

            public static ISymbol GetSymbol(IPage page, string name)
            {
                if (page == null)
                    throw new ArgumentNullException("page");
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                ISymbol deviceSymbol = page.Component.Symbol;
                ISymbol result = deviceSymbol.Children[name];
                if (result == null)
                    throw new InvalidOperationException("Cannot find symbol \"" + name + "\"");
                return result;
            }

            public static IProperty GetProperty(IPage page, string name)
            {
                ISymbol symbol = GetSymbol(page, name);
                IProperty result = symbol as IProperty;
                if (result == null)
                    throw new InvalidOperationException("Symbol " + symbol.Name + " type " + symbol.GetType().FullName + " is not the expected " + typeof(IProperty).FullName);
                return result;
            }

            public static IStruct GetStruct(IPage page, string name)
            {
                ISymbol symbol = GetSymbol(page, name);
                IStruct result = symbol as IStruct;
                if (result == null)
                    throw new InvalidOperationException("Symbol " + symbol.Name + " type " + symbol.GetType().FullName + " is not the expected " + typeof(IStruct).FullName);
                return result;
            }

            public static IStringProperty GetString(IPage page, string name)
            {
                IProperty property = GetProperty(page, name);
                IStringProperty result = property as IStringProperty;
                if (result == null)
                    throw new InvalidOperationException("Property " + property.Name + " type " + property.GetType().FullName + " is not the expected " + typeof(IStringProperty).FullName);
                return result;
            }

            public static INumericProperty GetNumeric(IPage page, string name)
            {
                IProperty property = GetProperty(page, name);
                INumericProperty result = property as INumericProperty;
                if (result == null)
                    throw new InvalidOperationException("Property " + property.Name + " type " + property.GetType().FullName + " is not the expected " + typeof(INumericProperty).FullName);
                return result;
            }

            public static string GetStringValue(IPage page, string name)
            {
                IStringProperty property = Properties.GetString(page, name);
                return property.Value;
            }

            public static double GetNumericValue(IPage page, string name)
            {
                INumericProperty property = Properties.GetNumeric(page, name);
                return property.Value.GetValueOrDefault();
            }

            public static bool GetBoolValue(IPage page, string name)
            {
                INumericProperty property = Properties.GetNumeric(page, name);
                return property.Value.GetValueOrDefault() != 0;
            }

            public static double GetNumericValue(IStruct structure, string name)
            {
                if (structure == null)
                    throw new ArgumentNullException("structure");

                ISymbol symbol = structure.Child(name);
                if (symbol == null)
                    throw new InvalidOperationException("Could not find the property " + name + " in structure " + structure.Name);

                INumericProperty property = symbol as INumericProperty;
                if (property == null)
                    throw new InvalidOperationException("Symbol " + symbol.Name + " type " + symbol.GetType().FullName + " is not the expected " + typeof(INumericProperty).FullName);

                double result = property.Value.GetValueOrDefault();
                return result;
            }
        }
        #endregion

        #region Commands
        public class Commands
        {
            private readonly IPage m_Page;
            private readonly IEditMethod m_EditMethod;
            private readonly ISymbol m_DeviceSymbol;

            public Commands(IPage page, IEditMethod editMethod, ISymbol deviceSymbol)
            {
                if (page == null)
                    throw new ArgumentNullException("page");
                if (editMethod == null)
                    throw new ArgumentNullException("m_EditMethod");
                if (deviceSymbol == null)
                    throw new ArgumentNullException("deviceSymbol");

                m_Page = page;
                m_EditMethod = editMethod;
                m_DeviceSymbol = deviceSymbol;
            }

            public bool IsInScript(SeparationMethodStage stageType, ICommand command)
            {
                if (command == null)
                    throw new ArgumentNullException("command");
                string commandName = command.Name;
                return IsInScript(stageType, commandName, command);
            }

            public bool IsInScript(SeparationMethodStage stageType, string commandName, ICommand command = null)
            {
                if (command == null)
                {
                    if (string.IsNullOrEmpty(commandName))
                        throw new ArgumentNullException("At least one of the parameters commandName or command must be provided");
                }

                ITimeStep stage = m_EditMethod.Stages.GetFirstTimeStepOfStage(stageType, false);
                if (stage == null)
                {
                    return false;
                }

                if (command == null)
                {
                    command = m_DeviceSymbol.Children[commandName] as ICommand;
                    if (command == null)
                    {
                        return false;
                    }
                }

                ISymbolStep symbolStep = DDKHelper.FindStepInStage(command.Path, stage);
                if (symbolStep == null)
                {
                    return false;
                }

                return true;
            }

            public void ScriptUpdate(SeparationMethodStage stageType, string commandName, bool addCommand)
            {
                ITimeStep stage = m_EditMethod.Stages.GetFirstTimeStepOfStage(stageType, true);

                ICommand command = m_DeviceSymbol.Children[commandName] as ICommand;
                ISymbolStep commandStep = DDKHelper.FindStepInStage(command.Path, stage);

                if (addCommand)
                {
                    if (commandStep == null)  // The command is not in the script
                    {
                        command = m_DeviceSymbol.Child(commandName) as ICommand;
                        commandStep = m_Page.Component.CreateStep(command) as ICommandStep;
                        stage.SymbolSteps.Add(commandStep);
                    }
                }
                else // Remove Command
                {
                    if (commandStep != null)  // The command is in the script
                    {
                        commandStep.Remove();
                    }
                }
            }

            public void ScriptUpdateWait(SeparationMethodStage stageType, string waitForSymbolName, bool addCommand)
            {
                ITimeStep stage = m_EditMethod.Stages.GetFirstTimeStepOfStage(stageType, addCommand);
                if (!addCommand && stage == null)
                {
                    return;
                }

                string waitForSymbolPath = GetSymbolPath(m_Page, waitForSymbolName);

                ISymbolStep waitForSymbolStep = DDKHelper.FindStepInStage(CommandName.Wait, stage, new string[] { waitForSymbolPath });

                if (addCommand)
                {
                    if (waitForSymbolStep == null)
                    {
                        // System Wait always exist, therefore not checking for nulls
                        ICommand command = m_EditMethod.SymbolTable.Child(CommandName.Wait) as ICommand;
                        ICommandStep commandStep = m_Page.Component.CreateStep(command) as ICommandStep;
                        string path = m_DeviceSymbol.Child(waitForSymbolName).Path;
                        commandStep.Parameters[0].ValueExpression.Parse(path, ExpressionParseOptions.None);
                        stage.SymbolSteps.Add(commandStep);
                    }
                }
                else
                {
                    if (waitForSymbolStep != null)
                    {
                        waitForSymbolStep.Remove();
                    }
                }
            }
        }
        #endregion

        #region Stages
        public class Stages
        {
            private readonly IPage m_Page;
            private readonly IEditMethod m_EditMethod;
            private readonly ISymbol m_DeviceSymbol;

            private readonly Properties m_Property;
            private readonly Commands m_Command;

            public Stages(IPage page, IEditMethod editMethod, ISymbol deviceSymbol, Properties property, Commands command)
            {
                if (page == null)
                    throw new ArgumentNullException("page");
                if (editMethod == null)
                    throw new ArgumentNullException("m_EditMethod");
                if (deviceSymbol == null)
                    throw new ArgumentNullException("deviceSymbol");
                if (property == null)
                    throw new ArgumentNullException("property");
                if (command == null)
                    throw new ArgumentNullException("command");

                m_Page = page;
                m_EditMethod = editMethod;
                m_DeviceSymbol = deviceSymbol;
                m_Property = property;
                m_Command = command;
            }

            public void Create(SeparationMethodStage stageType, double minutes, string commandName = null)
            {
                IStage stage = m_EditMethod.Stages.GetStage(stageType, true);  // If the stage exists, it's time will be set to the minutes parameter
                stage.Time = MethodTime.FromMinutes(minutes);

                if (!string.IsNullOrEmpty(commandName))
                {
                    m_Command.ScriptUpdate(stageType, commandName, true);
                }
            }
        }
        #endregion
    }
}
