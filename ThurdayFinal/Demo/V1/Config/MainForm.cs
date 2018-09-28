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
    public sealed partial class MainForm : Form, IConfigurationPlugin, IConfigurationDescriptors, IConfigurationPluginSharedInstruments
    {
        #region Fields
        private Driver m_Driver;
        private IConfigSendReceive m_SendReceive;
        private IInstrumentInfo m_InstrumentInfo;
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
        string IConfigurationPlugin.Configuration
        {
            get
            {
                string result;
                if (m_Driver != null)
                {
                    result = m_Driver.XmlText;
                }
                else
                {
                    result = Driver.DefaultXmlText();
                }
                return result;
            }
            set
            {
                m_Driver = new Driver(value);
            }
        }

        string IConfigurationPlugin.ConfigurationReport
        {
            get { return m_Driver.GetReportText(m_InstrumentInfo, m_SharedInstrumentsView.InstrumentsMap); }
        }

        bool IConfigurationPlugin.ShowConfigurationDialog(IConfigDriverExchange configDriverExchange)
        {
            if (m_Driver == null)
                return false;

            m_SendReceive = configDriverExchange as IConfigSendReceive;
            Debug.Assert(m_SendReceive != null);

            UpdateControls();

            ShowDialog(configDriverExchange.ParentWindow);
            return DialogResult == DialogResult.OK;
        }
        #endregion

        #region Get Driver Data
        private void GetDriverData()
        {
            if (m_SendReceive == null)
                throw new InvalidOperationException("m_SendReceive is not initialized");

            Cursor = Cursors.WaitCursor;
            try
            {
                // Get Device Types: pump, sampler, detector - Not implemented

                // Get Is Idle
                CommandGetIsIdle commandGetIsIdle_Request = new CommandGetIsIdle(m_Driver.Demo.IsSimulated);
                string xmlTextRequest = commandGetIsIdle_Request.XmlText;
                string xmlTextResponse;
                m_SendReceive.SendReceive(xmlTextRequest, out xmlTextResponse);
                CommandResponse commandResponseReceived = new CommandResponse(xmlTextResponse);
                CommandGetIsIdle commandGetIsIdle_Respond = new CommandGetIsIdle(xmlTextResponse);
                Trace.WriteLine("IsIdle = " + commandGetIsIdle_Respond.IsIdle);

                GetFirmwareData();
            }
            finally
            {
                Cursor = Cursors.Default;
                UpdateControls();
            }
        }

        private void GetFirmwareData()
        {
            // This may take a while

            Action task = (() =>
            {
                // Get Firmware Data
                CommandGetFirmwareData commandGetFirmwareData_Request = new CommandGetFirmwareData(m_Driver.Demo.IsSimulated);
                string xmlTextRequest = commandGetFirmwareData_Request.XmlText;
                string xmlTextResponse;
                m_SendReceive.SendReceive(xmlTextRequest, out xmlTextResponse);
                CommandResponse commandResponseReceived = new CommandResponse(xmlTextResponse);
                CommandGetFirmwareData commandGetFirmwareData_Respond = new CommandGetFirmwareData(xmlTextResponse);
                m_Driver.Demo.FirmwareUsbAddress = commandGetFirmwareData_Respond.FirmwareUsbAddress;
                m_Driver.Demo.FirmwareVersion = commandGetFirmwareData_Respond.FirmwareVersion;
                m_Driver.Demo.SerialNo = commandGetFirmwareData_Respond.SerialNo;
            });

            Enabled = false;
            try
            {
                IAsyncResult ar = task.BeginInvoke(null, null);
                // If there is timeout here it should be bigger that the driver timeout
                // Provide cancel - like in case the user wants close the configuration
                while (!ar.AsyncWaitHandle.WaitOne(100))
                {
                    Application.DoEvents();
                }
                task.EndInvoke(ar);
            }
            finally
            {
                Enabled = true;
            }
        }
        #endregion

        #region IConfigurationDescriptors - Instrument Configuration Manager
        string IConfigurationDescriptors.InstrumentInfo(InstrumentID instrumentId)  // Required for drivers that are shared among a few instruments
        {
            // This is added to text where the instrument name is displayed
            return string.Empty;
        }

        string IConfigurationDescriptors.ConnectInfo
        {
            get
            {
                // This is added to text where the driver (module) name is displayed
                string result = m_Driver.Demo.IsSimulated ? "Simulated" : string.Empty;
                return result;
            }
        }
        #endregion

        #region IConfigurationPluginSharedInstruments
        // DDK calls the property to set the current value
        IInstrumentInfo IConfigurationPluginSharedInstruments.InstrumentConfiguration
        {
            set
            {
                if (m_InstrumentInfo == value)
                {
                    return;
                }
                m_InstrumentInfo = value;
                m_SharedInstrumentsView.Init(m_InstrumentInfo);
            }
        }

        // The user configures m_SharedInstrumentsView.InstrumentsMap and DDK reads it
        // All instruments IDs the driver is attached to
        // If AttachedTo = 0000 0000 0000 0011, then the driver is attached to 2 instruments with IDs 0001 and 0010
        long IConfigurationPluginSharedInstruments.AttachedTo
        {
            get { return m_SharedInstrumentsView.InstrumentsMap; }
            set
            {
                if (m_SharedInstrumentsView.InstrumentsMap == value)
                {
                    return;
                }
                m_SharedInstrumentsView.InstrumentsMap = value;
            }
        }
        #endregion

        #region Update Controls, Configuration & Validate
        private void UpdateControls()
        {
            Text = m_Driver.ID;

            gbDriver.Text = m_Driver.ID;

            Demo demo = m_Driver.Demo;
            chkDemo_IsSimulated.Checked = demo.IsSimulated;
            gbDemo.Text = demo.Id;
            txtDemo_Name.Text = demo.Name;
            txtDemo_FirmwareUsbAddress.Text = demo.FirmwareUsbAddress;
            lblFirmwareVersion.Text = demo.FirmwareVersion;
            lblSerialNo.Text = demo.SerialNo;

            Heater heater = m_Driver.Heater;
            gbHeater.Text = heater.Id;
            txtHeater_Name.Text = heater.Name;
            txtHeater_ProductDescription.Text = heater.ProductDescription;

            Detector detector = m_Driver.Detector;
            gbDetector.Text = detector.Id;
            txtDetector_Name.Text = detector.Name;
            txtDetector_ChannelsNumber.Text = detector.ChannelsNumber.ToString();

            ValidateControls();
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

            m_Driver.Demo.Name = txtDemo_Name.Text;
            m_Driver.Demo.IsSimulated = chkDemo_IsSimulated.Checked;
            m_Driver.Demo.FirmwareUsbAddress = txtDemo_FirmwareUsbAddress.Text;

            m_Driver.Heater.Name = txtHeater_Name.Text;
            m_Driver.Heater.ProductDescription = txtHeater_ProductDescription.Text;

            m_Driver.Detector.Name = txtDetector_Name.Text;

            string numberText = txtDetector_ChannelsNumber.Text;
            int number;
            if (!int.TryParse(numberText, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                throw new FormatException("\"" + numberText + "\" is not a number");
            m_Driver.Detector.ChannelsNumber = number;

            return true;
        }
        #endregion

        #region Control Events
        private void OnDemo_IsSimulated_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDemo_IsSimulated.Checked == m_Driver.Demo.IsSimulated)
            {
                return;
            }

            try
            {
                GetDriverData();
            }
            catch (Exception ex)
            {
                Util.ShowError(this, ex);
            }
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

                m_Driver.Check();

                UpdateConfig();

                Debug.WriteLine(m_Driver.XmlText);

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
