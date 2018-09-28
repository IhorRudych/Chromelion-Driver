namespace Dionex.DDK.V2.TempCtrlDriver.EditorPlugIn
{
    partial class TempCtrlPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TempCtrlPage));
            this.m_SepLineTemperatureControl = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine();
            this.m_CheckBoxTemperatureControl = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepCheckBox();
            this.m_LabelTemperatureNominal = new System.Windows.Forms.Label();
            this.m_LabelTemperatureLowerLimit = new System.Windows.Forms.Label();
            this.m_LabelTemperatureUpperLimit = new System.Windows.Forms.Label();
            this.m_TextBoxTemperatureNominal = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox();
            this.m_TextBoxTemperatureLowerLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox();
            this.m_TextBoxTemperatureUpperLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox();
            this.m_RangeTemperatureNominal = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.m_RangeTemperatureLowerLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.m_RangeTemperatureUpperLimit = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel();
            this.SuspendLayout();
            // 
            // m_SepLineTemperatureControl
            // 
            this.m_SepLineTemperatureControl.BackColor = System.Drawing.Color.Transparent;
            this.m_SepLineTemperatureControl.LineColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.m_SepLineTemperatureControl, "m_SepLineTemperatureControl");
            this.m_SepLineTemperatureControl.Name = "m_SepLineTemperatureControl";
            this.m_SepLineTemperatureControl.TabStop = false;
            // 
            // m_CheckBoxTemperatureControl
            // 
            resources.ApplyResources(this.m_CheckBoxTemperatureControl, "m_CheckBoxTemperatureControl");
            this.m_CheckBoxTemperatureControl.Name = "m_CheckBoxTemperatureControl";
            this.m_CheckBoxTemperatureControl.SymbolPath = "TemperatureControl";
            this.m_CheckBoxTemperatureControl.UseVisualStyleBackColor = true;
            // 
            // m_LabelTemperatureNominal
            // 
            resources.ApplyResources(this.m_LabelTemperatureNominal, "m_LabelTemperatureNominal");
            this.m_LabelTemperatureNominal.Name = "m_LabelTemperatureNominal";
            // 
            // m_LabelTemperatureLowerLimit
            // 
            resources.ApplyResources(this.m_LabelTemperatureLowerLimit, "m_LabelTemperatureLowerLimit");
            this.m_LabelTemperatureLowerLimit.Name = "m_LabelTemperatureLowerLimit";
            // 
            // m_LabelTemperatureUpperLimit
            // 
            resources.ApplyResources(this.m_LabelTemperatureUpperLimit, "m_LabelTemperatureUpperLimit");
            this.m_LabelTemperatureUpperLimit.Name = "m_LabelTemperatureUpperLimit";
            // 
            // m_TextBoxTemperatureNominal
            // 
            resources.ApplyResources(this.m_TextBoxTemperatureNominal, "m_TextBoxTemperatureNominal");
            this.m_TextBoxTemperatureNominal.Name = "m_TextBoxTemperatureNominal";
            this.m_TextBoxTemperatureNominal.SymbolPath = "Temperature.Nominal";
            // 
            // m_TextBoxTemperatureLowerLimit
            // 
            resources.ApplyResources(this.m_TextBoxTemperatureLowerLimit, "m_TextBoxTemperatureLowerLimit");
            this.m_TextBoxTemperatureLowerLimit.Name = "m_TextBoxTemperatureLowerLimit";
            this.m_TextBoxTemperatureLowerLimit.SymbolPath = "Temperature.LowerLimit";
            // 
            // m_TextBoxTemperatureUpperLimit
            // 
            resources.ApplyResources(this.m_TextBoxTemperatureUpperLimit, "m_TextBoxTemperatureUpperLimit");
            this.m_TextBoxTemperatureUpperLimit.Name = "m_TextBoxTemperatureUpperLimit";
            this.m_TextBoxTemperatureUpperLimit.SymbolPath = "Temperature.UpperLimit";
            // 
            // m_RangeTemperatureNominal
            // 
            resources.ApplyResources(this.m_RangeTemperatureNominal, "m_RangeTemperatureNominal");
            this.m_RangeTemperatureNominal.Name = "m_RangeTemperatureNominal";
            this.m_RangeTemperatureNominal.SymbolPath = "Temperature.Nominal";
            // 
            // m_RangeTemperatureLowerLimit
            // 
            resources.ApplyResources(this.m_RangeTemperatureLowerLimit, "m_RangeTemperatureLowerLimit");
            this.m_RangeTemperatureLowerLimit.Name = "m_RangeTemperatureLowerLimit";
            this.m_RangeTemperatureLowerLimit.SymbolPath = "Temperature.LowerLimit";
            // 
            // m_RangeTemperatureUpperLimit
            // 
            resources.ApplyResources(this.m_RangeTemperatureUpperLimit, "m_RangeTemperatureUpperLimit");
            this.m_RangeTemperatureUpperLimit.Name = "m_RangeTemperatureUpperLimit";
            this.m_RangeTemperatureUpperLimit.SymbolPath = "Temperature.UpperLimit";
            // 
            // TempCtrlPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_RangeTemperatureUpperLimit);
            this.Controls.Add(this.m_RangeTemperatureLowerLimit);
            this.Controls.Add(this.m_RangeTemperatureNominal);
            this.Controls.Add(this.m_TextBoxTemperatureUpperLimit);
            this.Controls.Add(this.m_TextBoxTemperatureLowerLimit);
            this.Controls.Add(this.m_TextBoxTemperatureNominal);
            this.Controls.Add(this.m_LabelTemperatureUpperLimit);
            this.Controls.Add(this.m_LabelTemperatureLowerLimit);
            this.Controls.Add(this.m_LabelTemperatureNominal);
            this.Controls.Add(this.m_CheckBoxTemperatureControl);
            this.Controls.Add(this.m_SepLineTemperatureControl);
            this.Name = "TempCtrlPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine m_SepLineTemperatureControl;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepCheckBox m_CheckBoxTemperatureControl;
        private System.Windows.Forms.Label m_LabelTemperatureNominal;
        private System.Windows.Forms.Label m_LabelTemperatureLowerLimit;
        private System.Windows.Forms.Label m_LabelTemperatureUpperLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox m_TextBoxTemperatureNominal;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox m_TextBoxTemperatureLowerLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyStepTextBox m_TextBoxTemperatureUpperLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel m_RangeTemperatureNominal;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel m_RangeTemperatureLowerLimit;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.PropertyRangeLabel m_RangeTemperatureUpperLimit;



    }
}
