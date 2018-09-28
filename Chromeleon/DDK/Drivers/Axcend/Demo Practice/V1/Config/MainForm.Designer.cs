namespace MyCompany.Demo.Config
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtHeater_Name = new System.Windows.Forms.TextBox();
            this.lblHeater_Name = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbHeater = new System.Windows.Forms.GroupBox();
            this.txtHeater_ProductDescription = new System.Windows.Forms.TextBox();
            this.lblHeater_ProductDescription = new System.Windows.Forms.Label();
            this.txtDemo_FirmwareUsbAddress = new System.Windows.Forms.TextBox();
            this.lblDemo_UsbAddress = new System.Windows.Forms.Label();
            this.gbDetector = new System.Windows.Forms.GroupBox();
            this.txtDetector_ChannelsNumber = new System.Windows.Forms.TextBox();
            this.lblDetector_ChannelsNumber = new System.Windows.Forms.Label();
            this.txtDetector_Name = new System.Windows.Forms.TextBox();
            this.lblDetector_Name = new System.Windows.Forms.Label();
            this.gbDriver = new System.Windows.Forms.GroupBox();
            this.lblSerialNo = new System.Windows.Forms.Label();
            this.lblSerialNoCaption = new System.Windows.Forms.Label();
            this.lblFirmwareVersion = new System.Windows.Forms.Label();
            this.lblFirmwareVersionCaption = new System.Windows.Forms.Label();
            this.chkDemo_IsSimulated = new System.Windows.Forms.CheckBox();
            this.gbDemo = new System.Windows.Forms.GroupBox();
            this.txtDemo_Name = new System.Windows.Forms.TextBox();
            this.lblDemo_Name = new System.Windows.Forms.Label();
            this.m_ErrorProvider_Demo_Name = new System.Windows.Forms.ErrorProvider(this.components);
            this.m_ErrorProvider_Heater_Name = new System.Windows.Forms.ErrorProvider(this.components);
            this.m_ErrorProvider_Detector_Name = new System.Windows.Forms.ErrorProvider(this.components);
            this.m_SharedInstrumentsView = new MyCompany.Demo.Config.SharedInstrumentsView();
            this.gbHeater.SuspendLayout();
            this.gbDetector.SuspendLayout();
            this.gbDriver.SuspendLayout();
            this.gbDemo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Demo_Name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Heater_Name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Detector_Name)).BeginInit();
            this.SuspendLayout();
            // 
            // txtHeater_Name
            // 
            resources.ApplyResources(this.txtHeater_Name, "txtHeater_Name");
            this.txtHeater_Name.Name = "txtHeater_Name";
            // 
            // lblHeater_Name
            // 
            resources.ApplyResources(this.lblHeater_Name, "lblHeater_Name");
            this.lblHeater_Name.Name = "lblHeater_Name";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // gbHeater
            // 
            this.gbHeater.Controls.Add(this.txtHeater_ProductDescription);
            this.gbHeater.Controls.Add(this.lblHeater_ProductDescription);
            this.gbHeater.Controls.Add(this.txtHeater_Name);
            this.gbHeater.Controls.Add(this.lblHeater_Name);
            resources.ApplyResources(this.gbHeater, "gbHeater");
            this.gbHeater.Name = "gbHeater";
            this.gbHeater.TabStop = false;
            // 
            // txtHeater_ProductDescription
            // 
            resources.ApplyResources(this.txtHeater_ProductDescription, "txtHeater_ProductDescription");
            this.txtHeater_ProductDescription.Name = "txtHeater_ProductDescription";
            // 
            // lblHeater_ProductDescription
            // 
            resources.ApplyResources(this.lblHeater_ProductDescription, "lblHeater_ProductDescription");
            this.lblHeater_ProductDescription.Name = "lblHeater_ProductDescription";
            // 
            // txtDemo_FirmwareUsbAddress
            // 
            resources.ApplyResources(this.txtDemo_FirmwareUsbAddress, "txtDemo_FirmwareUsbAddress");
            this.txtDemo_FirmwareUsbAddress.Name = "txtDemo_FirmwareUsbAddress";
            // 
            // lblDemo_UsbAddress
            // 
            resources.ApplyResources(this.lblDemo_UsbAddress, "lblDemo_UsbAddress");
            this.lblDemo_UsbAddress.Name = "lblDemo_UsbAddress";
            // 
            // gbDetector
            // 
            this.gbDetector.Controls.Add(this.txtDetector_ChannelsNumber);
            this.gbDetector.Controls.Add(this.lblDetector_ChannelsNumber);
            this.gbDetector.Controls.Add(this.txtDetector_Name);
            this.gbDetector.Controls.Add(this.lblDetector_Name);
            resources.ApplyResources(this.gbDetector, "gbDetector");
            this.gbDetector.Name = "gbDetector";
            this.gbDetector.TabStop = false;
            // 
            // txtDetector_ChannelsNumber
            // 
            resources.ApplyResources(this.txtDetector_ChannelsNumber, "txtDetector_ChannelsNumber");
            this.txtDetector_ChannelsNumber.Name = "txtDetector_ChannelsNumber";
            // 
            // lblDetector_ChannelsNumber
            // 
            resources.ApplyResources(this.lblDetector_ChannelsNumber, "lblDetector_ChannelsNumber");
            this.lblDetector_ChannelsNumber.Name = "lblDetector_ChannelsNumber";
            // 
            // txtDetector_Name
            // 
            resources.ApplyResources(this.txtDetector_Name, "txtDetector_Name");
            this.txtDetector_Name.Name = "txtDetector_Name";
            // 
            // lblDetector_Name
            // 
            resources.ApplyResources(this.lblDetector_Name, "lblDetector_Name");
            this.lblDetector_Name.Name = "lblDetector_Name";
            // 
            // gbDriver
            // 
            this.gbDriver.Controls.Add(this.lblSerialNo);
            this.gbDriver.Controls.Add(this.lblSerialNoCaption);
            this.gbDriver.Controls.Add(this.lblFirmwareVersion);
            this.gbDriver.Controls.Add(this.lblFirmwareVersionCaption);
            this.gbDriver.Controls.Add(this.chkDemo_IsSimulated);
            resources.ApplyResources(this.gbDriver, "gbDriver");
            this.gbDriver.Name = "gbDriver";
            this.gbDriver.TabStop = false;
            // 
            // lblSerialNo
            // 
            resources.ApplyResources(this.lblSerialNo, "lblSerialNo");
            this.lblSerialNo.Name = "lblSerialNo";
            // 
            // lblSerialNoCaption
            // 
            resources.ApplyResources(this.lblSerialNoCaption, "lblSerialNoCaption");
            this.lblSerialNoCaption.Name = "lblSerialNoCaption";
            // 
            // lblFirmwareVersion
            // 
            resources.ApplyResources(this.lblFirmwareVersion, "lblFirmwareVersion");
            this.lblFirmwareVersion.Name = "lblFirmwareVersion";
            // 
            // lblFirmwareVersionCaption
            // 
            resources.ApplyResources(this.lblFirmwareVersionCaption, "lblFirmwareVersionCaption");
            this.lblFirmwareVersionCaption.Name = "lblFirmwareVersionCaption";
            // 
            // chkDemo_IsSimulated
            // 
            resources.ApplyResources(this.chkDemo_IsSimulated, "chkDemo_IsSimulated");
            this.chkDemo_IsSimulated.Name = "chkDemo_IsSimulated";
            this.chkDemo_IsSimulated.UseVisualStyleBackColor = true;
            // 
            // gbDemo
            // 
            this.gbDemo.Controls.Add(this.txtDemo_Name);
            this.gbDemo.Controls.Add(this.lblDemo_Name);
            this.gbDemo.Controls.Add(this.txtDemo_FirmwareUsbAddress);
            this.gbDemo.Controls.Add(this.lblDemo_UsbAddress);
            resources.ApplyResources(this.gbDemo, "gbDemo");
            this.gbDemo.Name = "gbDemo";
            this.gbDemo.TabStop = false;
            // 
            // txtDemo_Name
            // 
            resources.ApplyResources(this.txtDemo_Name, "txtDemo_Name");
            this.txtDemo_Name.Name = "txtDemo_Name";
            // 
            // lblDemo_Name
            // 
            resources.ApplyResources(this.lblDemo_Name, "lblDemo_Name");
            this.lblDemo_Name.Name = "lblDemo_Name";
            // 
            // m_ErrorProvider_Demo_Name
            // 
            this.m_ErrorProvider_Demo_Name.ContainerControl = this;
            // 
            // m_ErrorProvider_Heater_Name
            // 
            this.m_ErrorProvider_Heater_Name.ContainerControl = this;
            // 
            // m_ErrorProvider_Detector_Name
            // 
            this.m_ErrorProvider_Detector_Name.ContainerControl = this;
            // 
            // m_SharedInstrumentsView
            // 
            resources.ApplyResources(this.m_SharedInstrumentsView, "m_SharedInstrumentsView");
            this.m_SharedInstrumentsView.Name = "m_SharedInstrumentsView";
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.m_SharedInstrumentsView);
            this.Controls.Add(this.gbDetector);
            this.Controls.Add(this.gbHeater);
            this.Controls.Add(this.gbDemo);
            this.Controls.Add(this.gbDriver);
            this.DoubleBuffered = true;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.gbHeater.ResumeLayout(false);
            this.gbHeater.PerformLayout();
            this.gbDetector.ResumeLayout(false);
            this.gbDetector.PerformLayout();
            this.gbDriver.ResumeLayout(false);
            this.gbDriver.PerformLayout();
            this.gbDemo.ResumeLayout(false);
            this.gbDemo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Demo_Name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Heater_Name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorProvider_Detector_Name)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtHeater_Name;
        private System.Windows.Forms.Label lblHeater_Name;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbHeater;
        private System.Windows.Forms.GroupBox gbDetector;
        private System.Windows.Forms.TextBox txtDemo_FirmwareUsbAddress;
        private System.Windows.Forms.TextBox txtDetector_Name;
        private System.Windows.Forms.Label lblDetector_Name;
        private System.Windows.Forms.Label lblDemo_UsbAddress;
        private System.Windows.Forms.GroupBox gbDriver;
        private System.Windows.Forms.Label lblFirmwareVersionCaption;
        private System.Windows.Forms.GroupBox gbDemo;
        private System.Windows.Forms.TextBox txtDemo_Name;
        private System.Windows.Forms.Label lblDemo_Name;
        private System.Windows.Forms.CheckBox chkDemo_IsSimulated;
        private System.Windows.Forms.ErrorProvider m_ErrorProvider_Demo_Name;
        private System.Windows.Forms.ErrorProvider m_ErrorProvider_Heater_Name;
        private System.Windows.Forms.ErrorProvider m_ErrorProvider_Detector_Name;
        private System.Windows.Forms.TextBox txtHeater_ProductDescription;
        private System.Windows.Forms.Label lblHeater_ProductDescription;
        private System.Windows.Forms.TextBox txtDetector_ChannelsNumber;
        private System.Windows.Forms.Label lblDetector_ChannelsNumber;
        private System.Windows.Forms.Label lblFirmwareVersion;
        private SharedInstrumentsView m_SharedInstrumentsView;
        private System.Windows.Forms.Label lblSerialNo;
        private System.Windows.Forms.Label lblSerialNoCaption;
    }
}