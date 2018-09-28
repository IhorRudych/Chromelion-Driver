// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyCompany.Demo.Config
{
    public static class Util
    {
        public static void ShowError(Control ctrl, Exception ex)
        {
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");
            if (ex == null)
                throw new ArgumentNullException("ex");

            string errText = ex.Message;
            if (ex.InnerException != null)
            {
                errText += Environment.NewLine + Environment.NewLine + ex.InnerException.Message;
            }
            ShowError(ctrl, errText);
        }

        public static void ShowError(Control ctrl, string errText)
        {
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");

            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke((Action)(() =>
                {
                    MessageBoxErrorShow(errText);
                }));
            }
            else
            {
                MessageBoxErrorShow(errText);
            }
        }

        private static void MessageBoxErrorShow(string errText)
        {
            MessageBox.Show(errText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
