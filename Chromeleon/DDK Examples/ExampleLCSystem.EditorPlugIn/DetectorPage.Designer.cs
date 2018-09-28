namespace Dionex.DDK.V2.ExampleLCSystem.EditorPlugIn
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
            this.m_ChannelGrid = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl();
            this.m_SepLineChannels = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine();
            this.SuspendLayout();
            // 
            // m_ChannelGrid
            // 
            resources.ApplyResources(this.m_ChannelGrid, "m_ChannelGrid");
            this.m_ChannelGrid.Name = "m_ChannelGrid";
            // 
            // m_SepLineChannels
            // 
            this.m_SepLineChannels.BackColor = System.Drawing.Color.Transparent;
            this.m_SepLineChannels.LineColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.m_SepLineChannels, "m_SepLineChannels");
            this.m_SepLineChannels.Name = "m_SepLineChannels";
            this.m_SepLineChannels.TabStop = false;
            // 
            // DetectorPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_SepLineChannels);
            this.Controls.Add(this.m_ChannelGrid);
            this.Name = "DetectorPage";
            this.ResumeLayout(false);

        }

        #endregion

        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.ChannelGridControl m_ChannelGrid;
        private Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.SeparatorLine m_SepLineChannels;

    }
}
