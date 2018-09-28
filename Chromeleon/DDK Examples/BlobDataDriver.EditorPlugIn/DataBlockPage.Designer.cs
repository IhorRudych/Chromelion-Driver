namespace Dionex.DDK.V2.BlobDataDriver.EditorPlugIn
{
    partial class DataBlockPage
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
            Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine separatorLine;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataBlockPage));
            System.Windows.Forms.Label m_LabelDataContent;
            System.Windows.Forms.Label labelNumericalValue;
            System.Windows.Forms.Label m_LabelDeviceName;
            this.m_TextBoxData = new System.Windows.Forms.RichTextBox();
            this.m_TextBoxNumericalValue = new System.Windows.Forms.TextBox();
            this.m_DeviceName = new System.Windows.Forms.Label();
            separatorLine = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine();
            m_LabelDataContent = new System.Windows.Forms.Label();
            labelNumericalValue = new System.Windows.Forms.Label();
            m_LabelDeviceName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // separatorLine
            // 
            resources.ApplyResources(separatorLine, "separatorLine");
            separatorLine.BackColor = System.Drawing.Color.Transparent;
            separatorLine.LineColor = System.Drawing.Color.LightGray;
            separatorLine.Name = "separatorLine";
            separatorLine.TabStop = false;
            // 
            // m_LabelDataContent
            // 
            resources.ApplyResources(m_LabelDataContent, "m_LabelDataContent");
            m_LabelDataContent.Name = "m_LabelDataContent";
            // 
            // m_TextBoxData
            // 
            resources.ApplyResources(this.m_TextBoxData, "m_TextBoxData");
            this.m_TextBoxData.Name = "m_TextBoxData";
            // 
            // labelNumericalValue
            // 
            resources.ApplyResources(labelNumericalValue, "labelNumericalValue");
            labelNumericalValue.Name = "labelNumericalValue";
            // 
            // m_TextBoxNumericalValue
            // 
            resources.ApplyResources(this.m_TextBoxNumericalValue, "m_TextBoxNumericalValue");
            this.m_TextBoxNumericalValue.Name = "m_TextBoxNumericalValue";
            // 
            // m_LabelDeviceName
            // 
            resources.ApplyResources(m_LabelDeviceName, "m_LabelDeviceName");
            m_LabelDeviceName.Name = "m_LabelDeviceName";
            // 
            // m_DeviceName
            // 
            resources.ApplyResources(this.m_DeviceName, "m_DeviceName");
            this.m_DeviceName.Name = "m_DeviceName";
            // 
            // DataBlockPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_DeviceName);
            this.Controls.Add(m_LabelDeviceName);
            this.Controls.Add(this.m_TextBoxNumericalValue);
            this.Controls.Add(labelNumericalValue);
            this.Controls.Add(this.m_TextBoxData);
            this.Controls.Add(m_LabelDataContent);
            this.Controls.Add(separatorLine);
            this.Name = "DataBlockPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_TextBoxData;
        private System.Windows.Forms.TextBox m_TextBoxNumericalValue;
        private System.Windows.Forms.Label m_DeviceName;



    }
}
