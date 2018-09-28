namespace MyCompany.SendReceiveConfig
{
    partial class SendReceiveConfig
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendReceiveConfig));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelDeviceName = new System.Windows.Forms.Label();
			this.tBoxDeviceName = new System.Windows.Forms.TextBox();
			this.bu_Cancel = new System.Windows.Forms.Button();
			this.bu_Ok = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butSend = new System.Windows.Forms.Button();
			this.tBoxReceive = new System.Windows.Forms.TextBox();
			this.labelReceive = new System.Windows.Forms.Label();
			this.labelSend = new System.Windows.Forms.Label();
			this.tBoxSend = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelDeviceName);
			this.groupBox1.Controls.Add(this.tBoxDeviceName);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// labelDeviceName
			// 
			resources.ApplyResources(this.labelDeviceName, "labelDeviceName");
			this.labelDeviceName.Name = "labelDeviceName";
			// 
			// tBoxDeviceName
			// 
			resources.ApplyResources(this.tBoxDeviceName, "tBoxDeviceName");
			this.tBoxDeviceName.Name = "tBoxDeviceName";
			// 
			// bu_Cancel
			// 
			this.bu_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.bu_Cancel, "bu_Cancel");
			this.bu_Cancel.Name = "bu_Cancel";
			this.bu_Cancel.UseVisualStyleBackColor = true;
			this.bu_Cancel.Click += new System.EventHandler(this.bu_Cancel_Click);
			// 
			// bu_Ok
			// 
			this.bu_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.bu_Ok, "bu_Ok");
			this.bu_Ok.Name = "bu_Ok";
			this.bu_Ok.UseVisualStyleBackColor = true;
			this.bu_Ok.Click += new System.EventHandler(this.bu_Ok_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butSend);
			this.groupBox2.Controls.Add(this.tBoxReceive);
			this.groupBox2.Controls.Add(this.labelReceive);
			this.groupBox2.Controls.Add(this.labelSend);
			this.groupBox2.Controls.Add(this.tBoxSend);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// butSend
			// 
			resources.ApplyResources(this.butSend, "butSend");
			this.butSend.Name = "butSend";
			this.butSend.UseVisualStyleBackColor = true;
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// tBoxReceive
			// 
			resources.ApplyResources(this.tBoxReceive, "tBoxReceive");
			this.tBoxReceive.Name = "tBoxReceive";
			// 
			// labelReceive
			// 
			resources.ApplyResources(this.labelReceive, "labelReceive");
			this.labelReceive.Name = "labelReceive";
			// 
			// labelSend
			// 
			resources.ApplyResources(this.labelSend, "labelSend");
			this.labelSend.Name = "labelSend";
			// 
			// tBoxSend
			// 
			resources.ApplyResources(this.tBoxSend, "tBoxSend");
			this.tBoxSend.Name = "tBoxSend";
			// 
			// SendReceiveConfig
			// 
			this.AcceptButton = this.bu_Ok;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bu_Cancel;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.bu_Ok);
			this.Controls.Add(this.bu_Cancel);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "SendReceiveConfig";
			this.ShowInTaskbar = false;
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelDeviceName;
        private System.Windows.Forms.TextBox tBoxDeviceName;
        private System.Windows.Forms.Button bu_Cancel;
        private System.Windows.Forms.Button bu_Ok;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelReceive;
		private System.Windows.Forms.Label labelSend;
		private System.Windows.Forms.TextBox tBoxSend;
		private System.Windows.Forms.Button butSend;
		private System.Windows.Forms.TextBox tBoxReceive;
    }
}