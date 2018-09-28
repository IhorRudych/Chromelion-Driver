namespace MyCompany.Demo.Detector.EditorPlugIn
{
    partial class DetectorPage
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_ChannelControl = new Dionex.DDK.V2.CommonPages.Channels.ChannelControl();
            this.m_CommandAutoZeroLabel = new System.Windows.Forms.Label();
            this.m_CommandAutoZeroOption = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // m_ChannelControl
            // 
            this.m_ChannelControl.Location = new System.Drawing.Point(29, 93);
            this.m_ChannelControl.Margin = new System.Windows.Forms.Padding(5);
            this.m_ChannelControl.Name = "m_ChannelControl";
            this.m_ChannelControl.NoDetector = false;
            this.m_ChannelControl.Size = new System.Drawing.Size(443, 176);
            this.m_ChannelControl.TabIndex = 2;
            // 
            // m_CommandAutoZeroLabel
            // 
            this.m_CommandAutoZeroLabel.AutoSize = true;
            this.m_CommandAutoZeroLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_CommandAutoZeroLabel.Location = new System.Drawing.Point(36, 33);
            this.m_CommandAutoZeroLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_CommandAutoZeroLabel.Name = "m_CommandAutoZeroLabel";
            this.m_CommandAutoZeroLabel.Size = new System.Drawing.Size(67, 17);
            this.m_CommandAutoZeroLabel.TabIndex = 0;
            this.m_CommandAutoZeroLabel.Text = "&AutoZero";
            // 
            // m_CommandAutoZeroOption
            // 
            this.m_CommandAutoZeroOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_CommandAutoZeroOption.FormattingEnabled = true;
            this.m_CommandAutoZeroOption.Location = new System.Drawing.Point(110, 30);
            this.m_CommandAutoZeroOption.Name = "m_CommandAutoZeroOption";
            this.m_CommandAutoZeroOption.Size = new System.Drawing.Size(98, 24);
            this.m_CommandAutoZeroOption.TabIndex = 1;
            // 
            // DetectorPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_CommandAutoZeroOption);
            this.Controls.Add(this.m_CommandAutoZeroLabel);
            this.Controls.Add(this.m_ChannelControl);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DetectorPage";
            this.Size = new System.Drawing.Size(500, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Dionex.DDK.V2.CommonPages.Channels.ChannelControl m_ChannelControl;
        private System.Windows.Forms.Label m_CommandAutoZeroLabel;
        private System.Windows.Forms.ComboBox m_CommandAutoZeroOption;
    }
}
