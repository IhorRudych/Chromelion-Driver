namespace Dionex.DDK.V2.ChannelTest.EditorPlugIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetectorPage));
            this.m_ChannelGrid2D = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl();
            this.m_SepLineChan2D = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine();
            this.m_SepLine3DField = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine();
            this.m_ChannelGrid3D = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl();
            this.SuspendLayout();
            // 
            // m_ChannelGrid2D
            // 
            resources.ApplyResources(this.m_ChannelGrid2D, "m_ChannelGrid2D");
            this.m_ChannelGrid2D.Name = "m_ChannelGrid2D";
            // 
            // m_SepLineChan2D
            // 
            this.m_SepLineChan2D.BackColor = System.Drawing.Color.Transparent;
            this.m_SepLineChan2D.LineColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.m_SepLineChan2D, "m_SepLineChan2D");
            this.m_SepLineChan2D.Name = "m_SepLineChan2D";
            this.m_SepLineChan2D.TabStop = false;
            // 
            // m_SepLine3DField
            // 
            this.m_SepLine3DField.BackColor = System.Drawing.Color.Transparent;
            this.m_SepLine3DField.LineColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.m_SepLine3DField, "m_SepLine3DField");
            this.m_SepLine3DField.Name = "m_SepLine3DField";
            this.m_SepLine3DField.TabStop = false;
            // 
            // m_ChannelGrid3D
            // 
            resources.ApplyResources(this.m_ChannelGrid3D, "m_ChannelGrid3D");
            this.m_ChannelGrid3D.Name = "m_ChannelGrid3D";
            // 
            // DetectorPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_SepLine3DField);
            this.Controls.Add(this.m_ChannelGrid3D);
            this.Controls.Add(this.m_SepLineChan2D);
            this.Controls.Add(this.m_ChannelGrid2D);
            this.Name = "DetectorPage";
            this.ResumeLayout(false);

        }

        #endregion

        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl m_ChannelGrid2D;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine m_SepLineChan2D;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine m_SepLine3DField;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl m_ChannelGrid3D;

    }
}
