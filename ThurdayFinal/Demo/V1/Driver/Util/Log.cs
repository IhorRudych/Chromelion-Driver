// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Dionex.Chromeleon.Symbols;

namespace MyCompany
{
    internal static class Log
    {
        #region Fields
        private static readonly string s_Sprt = "\t";
        private static readonly string s_DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public static readonly string TraceCategory = "CmDrv";

        private static bool s_LogInFile;
        private static bool s_CanWriteToFile;
        private static readonly object s_LockObject = new object();

        private static readonly string s_FileNameExt = "log";
        private static readonly string s_FileNameDateTimeFormat = "yyyy-MM-dd HH mm ss";
        private static string s_FileNamePrefix = string.Empty;

        private static string s_Folder;
        private static StreamWriter s_File;

        private static readonly int s_MaxFiles = 300;
        private static readonly int s_FileLinesCountMax = 50000;
        private static int s_FileLinesCount;

        public static readonly string POI = "MyCompany" + s_Sprt;  // Point Of Interest
        #endregion

        #region Initialize
        public static void Init(string callerId, bool logInFile = false, string fileNamePrefix = null, string folder = null)
        {
            try
            {
                TaskBegin(callerId);
                InitFile(callerId, folder, fileNamePrefix);
                InitFileLogging(callerId, logInFile);
                TaskEnd(callerId);
            }
            catch (Exception ex)
            {
                TaskEnd(callerId, ex);
            }
        }
        #endregion

        #region Properties
        public static bool LogInFile
        {
            [DebuggerStepThrough]
            get { return s_LogInFile; }
        }

        public static string Folder
        {
            [DebuggerStepThrough]
            get { return s_Folder; }
        }

        private static string CallerMethodName
        {
            [DebuggerStepThrough]
            get { return GetMethodName((new StackFrame(2, false)).GetMethod()); }
        }

        [DebuggerStepThrough]
        public static string GetMethodName(MethodBase method)
        {
            if (method == null)
                return string.Empty;

            Type declaringType = method.DeclaringType;
            if (declaringType == null)
                return method.Name;

            return method.DeclaringType.Name + "." + method.Name;
        }
        #endregion

        #region File
        private static void InitFile(string callerId, string folder, string fileNamePrefix)
        {
            lock (s_LockObject)
            {
                s_CanWriteToFile = false;
                if (string.IsNullOrEmpty(folder))
                {
                    folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Dionex\Chromeleon\Log\" + callerId);
                }

                if (string.IsNullOrEmpty(fileNamePrefix))
                {
                    s_FileNamePrefix = string.Empty;
                }
                else
                {
                    s_FileNamePrefix = fileNamePrefix + " ";
                }

                if (string.IsNullOrEmpty(folder))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    s_Folder = Path.GetDirectoryName(assembly.Location);
                    s_Folder = Path.Combine(s_Folder, "Log");
                }
                else
                {
                    s_Folder = folder;
                }
                WriteLine(callerId, "Log folder: \"" + s_Folder + "\"");
            }
        }

        public static void InitFileLogging(string callerId, bool logInFile)
        {
            lock (s_LockObject)
            {
                s_LogInFile = logInFile;

                if (!logInFile)
                {
                    FileDispose();
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(s_Folder))
                        throw new InvalidOperationException("Logging folder is not set");
                    CreateNewFile(callerId);
                    s_CanWriteToFile = true;
                }
                catch (Exception ex)
                {
                    WriteLine(callerId, ex);
                    WriteLine(callerId, "Log to file is Disabled - due to exception: " + ex.Message);
                }

                if (s_CanWriteToFile)
                {
                    WriteLine(callerId, "Log to file is Enabled");
                }
            }
        }

        private static void CreateNewFile(string callerId)
        {
            FileDispose();

            if (!Directory.Exists(s_Folder))
            {
                Directory.CreateDirectory(s_Folder);
            }

            ReduceFileNumberToMaxFiles(callerId);

            string prefix = s_FileNamePrefix + DateTimeOffset.Now.ToString(s_FileNameDateTimeFormat);
            //prefix = string.Empty;  // Uncomment the line to work with 1 file only

            string fileName = Path.Combine(s_Folder, prefix + "." + s_FileNameExt);

            s_File = new StreamWriter(fileName);
        }

        private static void FileDispose()
        {
            s_FileLinesCount = 0;
            if (s_File != null)
            {
                s_File.Close();
                s_File.Dispose();
                s_File = null;
            }
        }

        private static void ReduceFileNumberToMaxFiles(string callerId)
        {
            DirectoryInfo dir = new DirectoryInfo(s_Folder);
            FileInfo[] fileInfos = dir.GetFiles(s_FileNamePrefix + "*." + s_FileNameExt);

            //List<string> fileNames = fileInfos.Select(item => item.Name).ToList();
            //for (int i = 0; i < fileNames.Count; i++) { Debug.WriteLine(fileNames[i]); }
            //List<string> fileNamesOrdered = fileNames.OrderBy(name => name).ToList();
            //for (int i = 0; i < fileNamesOrdered.Count; i++) { Debug.WriteLine(fileNamesOrdered[i]); }

            int filesToDeleteCount = fileInfos.Length - s_MaxFiles + 1;
            for (int i = 0; i < filesToDeleteCount; i++)
            {
                FileInfo file = fileInfos[i];
                TraceWriteLine(callerId, "Log file delete: " + file.Name);
                file.Delete();
            }
        }

        private static void FileWriteLine(string callerId)
        {
            if (!LogInFile || !s_CanWriteToFile)
            {
                return;
            }

            try
            {
                lock (s_LockObject)
                {
                    s_File.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                s_CanWriteToFile = false;
                TraceWriteLine(callerId, "Log to file is Disabled - due to exception. " +
                                         "Log file write error: " + GetErrorText(ex));
            }
        }

        private static void FileWriteLine(string callerId, string text)
        {
            if (!LogInFile || !s_CanWriteToFile)
            {
                return;
            }

            try
            {
                lock (s_LockObject)
                {
                    if (s_File == null || s_FileLinesCount >= s_FileLinesCountMax)
                    {
                        CreateNewFile(callerId);
                    }
                    s_FileLinesCount++;
                    s_File.WriteLine(text);
                    s_File.Flush();
                }
            }
            catch (Exception ex)
            {
                s_CanWriteToFile = false;
                TraceWriteLine(callerId, "Log to file is Disabled - due to exception. " +
                                         "Log file write error: " + GetErrorText(ex) + Environment.NewLine + "Log text: " + text);
            }
        }
        #endregion

        #region Logging Methods
        public static void TaskBegin(string callerId, string text = null, string callerMethodName = null, bool addNewLine = false)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }

            if (addNewLine)
            {
                Trace.WriteLine(Environment.NewLine, TraceCategory);
                FileWriteLine(callerId);
            }

            WriteLine(callerId, "Task Begin ======= " + text, callerMethodName);
        }

        public static void TaskEnd(string callerId, string text = null, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            WriteLine(callerId, "Task End   ======= " + text, callerMethodName);
        }

        public static void TaskEnd(string callerId, Exception ex, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            WriteLine(callerId, ex, callerMethodName, "Task End with Error =======");
        }

        public static void TaskEnd(string callerId, AuditLevel auditLevel, Exception ex, string callerMethodName)
        {
            WriteLine(callerId, ex, callerMethodName, "Task End with Error ======= AuditLevel." + auditLevel.ToString());
        }

        public static void WriteLine(string callerId, AuditLevel auditLevel, Exception ex, string callerMethodName)
        {
            WriteLine(callerId, ex, callerMethodName, "AuditLevel." + auditLevel.ToString() + " ");
        }

        public static void WriteLine(string callerId, AuditLevel auditLevel, string text, string callerMethodName, string prefixText = null)
        {
            WriteLine(callerId, prefixText + "AuditLevel." + auditLevel.ToString() + " " + text, callerMethodName);
        }

        public static void SendCommand(string callerId, string text, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            string logText = TraceWriteLine(callerId, "Send command " + text, callerMethodName);
            FileWriteLine(callerId, logText);
        }

        public static void SendCommandError(string callerId, string commandName, string errText, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            string logText = TraceWriteLine(callerId, "Sent command error \"" + commandName + "\": " + errText, callerMethodName);
            FileWriteLine(callerId, logText);
        }

        public static void PropertyChanged(string callerId, IStringProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            string displayValue = property.Value == null ? "Null" : "\"" + property.Value + "\"";
            WriteLinePropertyChanged(callerId, property.Name, displayValue, callerMethodName);
        }

        public static void PropertyChangedBool(string callerId, IIntProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }

            if (property.Value == null)
            {
                WriteLinePropertyChanged(callerId, property.Name, "Null", callerMethodName);
            }
            else
            {
                WriteLinePropertyChanged(callerId, property.Name, (property.Value.GetValueOrDefault() != 0).ToString(CultureInfo.InvariantCulture), callerMethodName);
            }
        }

        public static void PropertyChangedBool(string callerId, IIntProperty property, Nullable<bool> value, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }

            if (value == null)
            {
                WriteLinePropertyChanged(callerId, property.Name, "Null", callerMethodName);
            }
            else
            {
                WriteLinePropertyChanged(callerId, property.Name, value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), callerMethodName);
            }
        }

        public static void PropertyChangedEnum(string callerId, string propertyName, int value, string valueName, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            WriteLinePropertyChanged(callerId, propertyName, value.ToString(CultureInfo.InvariantCulture) + " = " + valueName, callerMethodName);
        }

        public static void PropertyChangedEnum(string callerId, IIntProperty property, string valueName, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            Nullable<int> value = property.Value;
            WriteLinePropertyChanged(callerId, property.Name, (value == null ? "Null" : value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)) + " = " + valueName, callerMethodName);
        }

        public static void PropertyChangedBinary(string callerId, IIntProperty property, int numberOfBits = 0, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            uint value = (uint)property.Value.GetValueOrDefault();
            PropertyChangedBinary(callerId, property.Name, value, numberOfBits, callerMethodName);
        }

        public static void PropertyChanged(string callerId, IIntProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            Nullable<int> value = property.Value;
            WriteLinePropertyChanged(callerId, property.Name, value == null ? "Null" : value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string unitName, IIntProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            Nullable<int> value = property.Value;
            WriteLinePropertyChanged(callerId, property.Name, (value == null ? "Null" : value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)) + " " + unitName, callerMethodName);
        }

        public static void PropertyChanged(string callerId, IDoubleProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            Nullable<double> value = property.Value;
            WriteLinePropertyChanged(callerId, property.Name, value == null ? "Null" : value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string unitName, IDoubleProperty property, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            Nullable<double> value = property.Value;
            WriteLinePropertyChanged(callerId, property.Name, (value == null ? "Null" : value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)) + " " + unitName, callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name, bool value, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name, value.ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name, int value, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name, value.ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name, Nullable<int> value, string callerMethodName)
        {
            if (value == null)
            {
                PropertyChanged(callerId, name, (string)null, callerMethodName);
            }
            else
            {
                WriteLinePropertyChanged(callerId, name, value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), callerMethodName);
            }
        }

        public static void PropertyChanged(string callerId, string name, double value, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name, value.ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name, Nullable<double> value, string callerMethodName)
        {
            if (value == null)
            {
                PropertyChanged(callerId, name, (string)null, callerMethodName);
            }
            else
            {
                WriteLinePropertyChanged(callerId, name, value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), callerMethodName);
            }
        }

        public static void PropertyChanged(string callerId, string name, string value, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name, value == null ? "Null" : "\"" + value + "\"", callerMethodName);
        }

        private static void WriteLinePropertyChanged(string callerId, string name, string value, string callerMethodName)
        {
            WriteLine(callerId, "Property changed: " + name + " = " + value, callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name1, int value1,
                                                            string name2, bool value2, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name1, value1.ToString(CultureInfo.InvariantCulture),
                                               name2, value2.ToString(CultureInfo.InvariantCulture), callerMethodName);
        }

        public static void PropertyChanged(string callerId, string name1, string value1,
                                                            string name2, string value2, string callerMethodName)
        {
            WriteLinePropertyChanged(callerId, name1, value1, name2, value2, callerMethodName);
        }

        public static void PropertyChangedBinary(string callerId, string name, uint value, int numberOfBits = 0, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }

            if (numberOfBits <= 0)
            {
                numberOfBits = sizeof(uint) * 8;
            }

            string binaryValueText = Property.GetBinaryDisplayText(value, numberOfBits);
            PropertyChangedBinary(callerId, name, value, binaryValueText, callerMethodName);
        }

        public static void PropertyChangedBinary(string callerId, string name, uint value, string binaryValueText, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            WriteLine(callerId, "Property changed: " + name + " = " + value.ToString(CultureInfo.InvariantCulture) +
                                                       (string.IsNullOrEmpty(binaryValueText) ? string.Empty : " = " + binaryValueText),
                                                       callerMethodName);
        }

        private static void WriteLinePropertyChanged(string callerId, string name1, string value1,
                                                                      string name2, string value2, string callerMethodName)
        {
            WriteLine(callerId, "Property changed: " + name1 + " = " + value1 + ", " + name2 + " = " + value2, callerMethodName);
        }

        private static string GetErrorText(Exception ex)
        {
            string result = ex != null ? Environment.NewLine + "      " + ex.ToString() + Environment.NewLine : null;
            return result;
        }

        public static void WriteLine(string callerId, Exception ex, string callerMethodName = null, string prefixText = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            string text = "Error: " + prefixText + GetErrorText(ex);
            WriteLine(callerId, text, callerMethodName);
        }
        #endregion

        #region Write Line
        public static void WriteLine(string callerId)
        {
            WriteLine(callerId, string.Empty, CallerMethodName);
        }

        public static void WriteLine(string callerId, string text, string callerMethodName = null)
        {
            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }
            string logText = TraceWriteLine(callerId, text, callerMethodName);
            FileWriteLine(callerId, logText);
        }

        private static string TraceWriteLine(string callerId, string text, string callerMethodName = null)
        {
            string dateTimeText = DateTimeOffset.Now.ToString(s_DateTimeFormat);

            if (callerMethodName == null)
            {
                callerMethodName = CallerMethodName;
            }

            Thread thread = Thread.CurrentThread;
            string threadText = null;
            if (thread != null)
            {
                threadText = thread.GetApartmentState().ToString() + s_Sprt +
                             thread.ManagedThreadId.ToString().PadRight(7);
            }

            string result = dateTimeText + s_Sprt +
                            threadText + s_Sprt +
                            callerId + s_Sprt +
                            callerMethodName + s_Sprt +
                            text;

            Trace.WriteLine(result, TraceCategory);
            return result;
        }
        #endregion
    }
}
