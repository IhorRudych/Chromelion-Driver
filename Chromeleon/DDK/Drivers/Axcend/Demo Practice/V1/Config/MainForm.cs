//// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CmHelpers;
using Dionex.Chromeleon.DDK;

namespace MyCompany.Demo.Config
{
    [DriverID(Driver.Id)]
    [CustomIcon(Driver.Id + ".bmp")]
    public sealed partial class MainForm : Form // , IConfigurationPlugin, IConfigurationDescriptors, IConfigurationPluginSharedInstruments
    {
        #region Fields
        //private Driver m_Driver;
        //private IConfigSendReceive m_SendReceive;
        //private IInstrumentInfo m_InstrumentInfo;
        #endregion

        #region Constructor
        public MainForm()
        {
#if DEBUG
            //Debugger.Launch();
#endif
            InitializeComponent();

            gbDemo.Text = string.Empty;
            txtDemo_Name.Text = string.Empty;
            txtDemo_FirmwareUsbAddress.Text = string.Empty;
            lblFirmwareVersion.Text = string.Empty;
            lblSerialNo.Text = string.Empty;

            chkDemo_IsSimulated.CheckedChanged += OnDemo_IsSimulated_CheckedChanged;

            txtDemo_Name.Validating += OnDemo_Name_Validating;
            txtHeater_Name.Validating += OnHeater_Name_Validating;
            txtDetector_Name.Validating += OnDetector_Name_Validating;

            HelpButtonClicked += OnMainFormHelp;

            btnOK.Click += OnBtnOK;

            // Conversion example
            const double valuePsi = 100;
            double valueMegaPascal = UnitConversionEx.PhysUnitConvert(UnitConversion.PhysUnitEnum.PhysUnit_Psi, UnitConversion.PhysUnitEnum.PhysUnit_MegaPascal, valuePsi);  // 0.68947569999999991
            double valueBar = UnitConversionEx.PhysUnitConvert(UnitConversion.PhysUnitEnum.PhysUnit_Psi, UnitConversion.PhysUnitEnum.PhysUnit_Bar, valuePsi);                // 6.8947569999999994

            // Not localized
            string unitPsi = UnitConversionEx.PhysUnitLatin(UnitConversion.PhysUnitEnum.PhysUnit_Psi);                // psi
            string unitMegaPascal = UnitConversionEx.PhysUnitLatin(UnitConversion.PhysUnitEnum.PhysUnit_MegaPascal);  // MPa
            string unitBar = UnitConversionEx.PhysUnitLatin(UnitConversion.PhysUnitEnum.PhysUnit_Bar);                // bar

            // Localized
            string unitPsiLocalized = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_Psi);
            string unitMegaPascalLocalized = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_MegaPascal);
            string unitBarLocalized = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_Bar);

            string text = valuePsi.ToString() + " " + unitPsi + " " + unitPsiLocalized + " = " +
                          valueMegaPascal.ToString() + " " + unitMegaPascal + " " + unitMegaPascalLocalized + " = " +
                          valueBar.ToString() + " " + unitBar + " " + unitBarLocalized;
            Trace.WriteLine(text);
        }
        #endregion

        #region Help
        private void OnMainFormHelp(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        #endregion

        #region IConfigurationPlugin
        #endregion

        #region Get Driver Data
        #endregion

        #region IConfigurationDescriptors - Instrument Configuration Manager
        #endregion

        #region IConfigurationPluginSharedInstruments
        #endregion

        #region Update Controls, Configuration & Validate
        private void UpdateControls()
        {
        }

        private bool ValidateControls()
        {
            bool success = true;

            if (!Demo_Name_Validate())
                success = false;

            if (!Heater_Name_Validate())
                success = false;

            if (!Detector_Name_Validate())
                success = false;

            return success;
        }

        private bool Demo_Name_Validate()
        {
            bool result = ValidateDeviceName(txtDemo_Name, m_ErrorProvider_Demo_Name);
            if (result)
            {
                m_ErrorProvider_Demo_Name.SetError(txtDemo_Name, string.Empty);
            }
            return result;
        }

        private bool Heater_Name_Validate()
        {
            bool result = ValidateDeviceName(txtHeater_Name, m_ErrorProvider_Heater_Name);
            if (result)
            {
                m_ErrorProvider_Heater_Name.SetError(txtHeater_Name, string.Empty);
            }
            return result;
        }

        private bool Detector_Name_Validate()
        {
            bool result = ValidateDeviceName(txtDetector_Name, m_ErrorProvider_Detector_Name);
            if (result)
            {
                m_ErrorProvider_Detector_Name.SetError(txtDetector_Name, string.Empty);
            }
            return result;
        }

        private static readonly Regex m_IncorrectDeviceNameRegex = new Regex(@"[^a-zA-Z0-9_]");  // Allowed characters: letters, digits, '_'
        private static readonly int m_MaxDeviceNameLength = 30;

        private static bool ValidateDeviceName(TextBox textBox, ErrorProvider errorProvider)
        {
            if (textBox == null)
                throw new ArgumentNullException("textBox");
            if (errorProvider == null)
                throw new ArgumentNullException("errorProvider");

            string errText;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                errText = "Must have a value";
            }
            else if (textBox.Text.Length > m_MaxDeviceNameLength)
            {
                errText = "The length exceeds the maximum of " + m_MaxDeviceNameLength.ToString() + " characters";
            }
            else if (m_IncorrectDeviceNameRegex.Match(textBox.Text).Success)
            {
                errText = "Only a-z, A-Z, 0-9, '_' characters are allowed";
            }
            else
            {
                errText = string.Empty;
            }

            bool result = string.IsNullOrEmpty(errText);

            if (!result)
            {
                errorProvider.SetError(textBox, errText);
            }

            return result;
        }

        private bool UpdateConfig()
        {
            if (!ValidateControls())
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Control Events
        private void OnDemo_IsSimulated_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkDemo_IsSimulated.Checked == m_Driver.Demo.IsSimulated)
            //{
            //    return;
            //}

            //try
            //{
            //    GetDriverData();
            //}
            //catch (Exception ex)
            //{
            //    Util.ShowError(this, ex);
            //}
        }

        private void OnDemo_Name_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Demo_Name_Validate();
        }

        private void OnHeater_Name_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Heater_Name_Validate();
        }

        private void OnDetector_Name_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Detector_Name_Validate();
        }
        #endregion

        #region Button OK
        private void OnBtnOK(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // Throws Exception
                m_SharedInstrumentsView.ValidateData();

                // Does not Throw Exception
                if (!UpdateConfig())
                {
                    Util.ShowError(this, "Enter valid values");
                    return;
                }

                //m_Driver.Check();

                UpdateConfig();

                //Debug.WriteLine(m_Driver.XmlText);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Util.ShowError(this, ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        #endregion
    }
}
