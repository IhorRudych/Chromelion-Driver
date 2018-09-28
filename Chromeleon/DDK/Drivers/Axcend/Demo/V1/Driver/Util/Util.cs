// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Globalization;
using Dionex.Chromeleon.Symbols;

namespace MyCompany
{
    public static class Util
    {
        public static readonly string DateFormat = "yyyy/MM/dd";
        public static readonly string TimeFormat = "HH:mm:ss";
        public static readonly string TimeFormatWithMilliSec = TimeFormat + ".fff";
        public static readonly string DateTimeFormat = DateFormat + " " + TimeFormat;
        public static readonly string DateTimeFormatMilliSec = DateFormat + " " + TimeFormatWithMilliSec;

        public static string GetTimeDisplayText(double minutes)
        {
            int min = (int)minutes;
            int sec = (int)((minutes - min) * 60);

            string result = min.ToString() + ":" + sec.ToString("00") + " (" + minutes.ToString() + ")";
            return result;
        }

        public static void RaiseEvent(string id, string eventName, EventHandler eventHandlers, object sender, EventArgs args, bool reThrowEventHandlerException = false)
        {
            Log.TaskBegin(id, "Raise Event " + eventName);
            try
            {
                if (eventHandlers == null)
                {
                    Log.TaskEnd(id, "Raise Event " + eventName);
                    return;
                }

                Delegate[] delegates = eventHandlers.GetInvocationList();
                foreach (Delegate dlgt in delegates)
                {
                    try
                    {
                        EventHandler eventHandler = (EventHandler)dlgt;
                        eventHandler(sender, args);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(id, ex);
                        if (reThrowEventHandlerException)
                            throw;
                    }
                }

                Log.TaskEnd(id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(id, ex);
                throw;
            }
        }

        public static string GetCustomVariableValue(ICustomVariable customVariable)
        {
            string result = string.Empty;
            if (customVariable.Type == CustomVariableType.String)
            {
                result = "\"" + customVariable.Text + "\"";
            }
            if (customVariable.Type == CustomVariableType.Numeric)
            {
                if (customVariable.Number.HasValue)
                {
                    result = customVariable.Number.Value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    result = "Null";
                }
            }
            return result;
        }

        public static double GetDataPoint(double timeMinutes, int offset)
        {
            double result = 500 * (Math.Cos(timeMinutes * Math.PI) + 1) - (offset - 1) * 100;
            if (result < 0)
            {
                result = 0;
            }
            return result;
        }
    }
}
