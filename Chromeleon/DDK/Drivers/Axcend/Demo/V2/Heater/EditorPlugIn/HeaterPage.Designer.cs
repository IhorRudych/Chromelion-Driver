namespace MyCompany.Demo.Heater.EditorPlugIn
{
    partial class HeaterPage
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
            System.Windows.Forms.Label labelLowerLimit;
            System.Windows.Forms.Label labelTemperature;
            this.rangeLabelUpperLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.rangeLabelLowerLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.rangeLabelNominalTemperature = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.labelUpperLimit = new System.Windows.Forms.Label();
            this.m_TempControlCheckBox = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepCheckBox();
            this.m_TextBoxLowerLimit = new System.Windows.Forms.TextBox();
            this.m_TextBoxUpperLimit = new System.Windows.Forms.TextBox();
            this.m_TextBoxNominal = new System.Windows.Forms.TextBox();
            labelLowerLimit = new System.Windows.Forms.Label();
            labelTemperature = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rangeLabelUpperLimit
            // 
            this.rangeLabelUpperLimit.AutoSize = true;
            this.rangeLabelUpperLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rangeLabelUpperLimit.Location = new System.Drawing.Point(281, 149);
            this.rangeLabelUpperLimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rangeLabelUpperLimit.Name = "rangeLabelUpperLimit";
            this.rangeLabelUpperLimit.Size = new System.Drawing.Size(92, 17);
            this.rangeLabelUpperLimit.SymbolPath = "Temperature.UpperLimit";
            this.rangeLabelUpperLimit.TabIndex = 9;
            this.rangeLabelUpperLimit.Text = "[012...456 xx]";
            // 
            // rangeLabelLowerLimit
            // 
            this.rangeLabelLowerLimit.AutoSize = true;
            this.rangeLabelLowerLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rangeLabelLowerLimit.Location = new System.Drawing.Point(281, 111);
            this.rangeLabelLowerLimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rangeLabelLowerLimit.Name = "rangeLabelLowerLimit";
            this.rangeLabelLowerLimit.Size = new System.Drawing.Size(92, 17);
            this.rangeLabelLowerLimit.SymbolPath = "Temperature.LowerLimit";
            this.rangeLabelLowerLimit.TabIndex = 6;
            this.rangeLabelLowerLimit.Text = "[012...456 xx]";
            // 
            // rangeLabelNominalTemperature
            // 
            this.rangeLabelNominalTemperature.AutoSize = true;
            this.rangeLabelNominalTemperature.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rangeLabelNominalTemperature.Location = new System.Drawing.Point(281, 78);
            this.rangeLabelNominalTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rangeLabelNominalTemperature.Name = "rangeLabelNominalTemperature";
            this.rangeLabelNominalTemperature.Size = new System.Drawing.Size(92, 17);
            this.rangeLabelNominalTemperature.SymbolPath = "Temperature.Nominal";
            this.rangeLabelNominalTemperature.TabIndex = 3;
            this.rangeLabelNominalTemperature.Text = "[012...456 xx]";
            // 
            // labelUpperLimit
            // 
            this.labelUpperLimit.AutoSize = true;
            this.labelUpperLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelUpperLimit.Location = new System.Drawing.Point(39, 149);
            this.labelUpperLimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUpperLimit.Name = "labelUpperLimit";
            this.labelUpperLimit.Size = new System.Drawing.Size(80, 17);
            this.labelUpperLimit.TabIndex = 7;
            this.labelUpperLimit.Text = "U&pper Limit";
            // 
            // labelLowerLimit
            // 
            labelLowerLimit.AutoSize = true;
            labelLowerLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            labelLowerLimit.Location = new System.Drawing.Point(39, 111);
            labelLowerLimit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelLowerLimit.Name = "labelLowerLimit";
            labelLowerLimit.Size = new System.Drawing.Size(79, 17);
            labelLowerLimit.TabIndex = 4;
            labelLowerLimit.Text = "&Lower Limit";
            // 
            // labelTemperature
            // 
            labelTemperature.AutoSize = true;
            labelTemperature.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            labelTemperature.Location = new System.Drawing.Point(39, 73);
            labelTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTemperature.Name = "labelTemperature";
            labelTemperature.Size = new System.Drawing.Size(90, 17);
            labelTemperature.TabIndex = 1;
            labelTemperature.Text = "&Temperature";
            // 
            // m_TempControlCheckBox
            // 
            this.m_TempControlCheckBox.AutoSize = true;
            this.m_TempControlCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_TempControlCheckBox.Location = new System.Drawing.Point(39, 28);
            this.m_TempControlCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_TempControlCheckBox.Name = "m_TempControlCheckBox";
            this.m_TempControlCheckBox.Size = new System.Drawing.Size(190, 21);
            this.m_TempControlCheckBox.SymbolPath = "Temperature.Control";
            this.m_TempControlCheckBox.TabIndex = 0;
            this.m_TempControlCheckBox.Text = "&Use Temperature Control";
            this.m_TempControlCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_TextBoxLowerLimit
            // 
            this.m_TextBoxLowerLimit.Location = new System.Drawing.Point(137, 111);
            this.m_TextBoxLowerLimit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_TextBoxLowerLimit.Name = "m_TextBoxLowerLimit";
            this.m_TextBoxLowerLimit.Size = new System.Drawing.Size(132, 22);
            this.m_TextBoxLowerLimit.TabIndex = 5;
            // 
            // m_TextBoxUpperLimit
            // 
            this.m_TextBoxUpperLimit.Location = new System.Drawing.Point(137, 149);
            this.m_TextBoxUpperLimit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_TextBoxUpperLimit.Name = "m_TextBoxUpperLimit";
            this.m_TextBoxUpperLimit.Size = new System.Drawing.Size(132, 22);
            this.m_TextBoxUpperLimit.TabIndex = 8;
            // 
            // m_TextBoxNominal
            // 
            this.m_TextBoxNominal.Location = new System.Drawing.Point(137, 73);
            this.m_TextBoxNominal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_TextBoxNominal.Name = "m_TextBoxNominal";
            this.m_TextBoxNominal.Size = new System.Drawing.Size(132, 22);
            this.m_TextBoxNominal.TabIndex = 2;
            // 
            // HeaterPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rangeLabelUpperLimit);
            this.Controls.Add(this.rangeLabelLowerLimit);
            this.Controls.Add(this.rangeLabelNominalTemperature);
            this.Controls.Add(this.labelUpperLimit);
            this.Controls.Add(labelLowerLimit);
            this.Controls.Add(labelTemperature);
            this.Controls.Add(this.m_TempControlCheckBox);
            this.Controls.Add(this.m_TextBoxLowerLimit);
            this.Controls.Add(this.m_TextBoxUpperLimit);
            this.Controls.Add(this.m_TextBoxNominal);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "HeaterPage";
            this.Size = new System.Drawing.Size(400, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel rangeLabelUpperLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel rangeLabelLowerLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel rangeLabelNominalTemperature;
        private System.Windows.Forms.Label labelUpperLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepCheckBox m_TempControlCheckBox;
        private System.Windows.Forms.TextBox m_TextBoxLowerLimit;
        private System.Windows.Forms.TextBox m_TextBoxUpperLimit;
        private System.Windows.Forms.TextBox m_TextBoxNominal;

    }
}
