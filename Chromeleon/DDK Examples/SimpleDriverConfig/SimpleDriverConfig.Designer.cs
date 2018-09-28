namespace MyCompany.SimpleDriverConfig
{
    partial class SimpleDriverConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleDriverConfig));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.la_DeviceName = new System.Windows.Forms.Label();
            this.teBo_DeviceName = new System.Windows.Forms.TextBox();
            this.bu_Cancel = new System.Windows.Forms.Button();
            this.bu_Ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.la_DeviceName);
            this.groupBox1.Controls.Add(this.teBo_DeviceName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // la_DeviceName
            // 
            resources.ApplyResources(this.la_DeviceName, "la_DeviceName");
            this.la_DeviceName.Name = "la_DeviceName";
            // 
            // teBo_DeviceName
            // 
            resources.ApplyResources(this.teBo_DeviceName, "teBo_DeviceName");
            this.teBo_DeviceName.Name = "teBo_DeviceName";
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
            // SimpleDriverConfig
            // 
            this.AcceptButton = this.bu_Ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bu_Cancel;
            this.Controls.Add(this.bu_Ok);
            this.Controls.Add(this.bu_Cancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "SimpleDriverConfig";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label la_DeviceName;
        private System.Windows.Forms.TextBox teBo_DeviceName;
        private System.Windows.Forms.Button bu_Cancel;
        private System.Windows.Forms.Button bu_Ok;
    }
}