namespace MyCompany.Demo.Config
{
    partial class SharedInstrumentsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SharedInstrumentsView));
            this.m_LabelUserText = new System.Windows.Forms.Label();
            this.m_ListViewInstruments = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            //
            // m_LabelUserText
            //
            resources.ApplyResources(this.m_LabelUserText, "m_LabelUserText");
            this.m_LabelUserText.Name = "m_LabelUserText";
            //
            // m_ListViewInstruments
            //
            resources.ApplyResources(this.m_ListViewInstruments, "m_ListViewInstruments");
            this.m_ListViewInstruments.CheckBoxes = true;
            this.m_ListViewInstruments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_ListViewInstruments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_ListViewInstruments.Name = "m_ListViewInstruments";
            this.m_ListViewInstruments.UseCompatibleStateImageBehavior = false;
            this.m_ListViewInstruments.View = System.Windows.Forms.View.Details;
            //
            // columnHeader1
            //
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            //
            // SharedInstrumentsControl
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_LabelUserText);
            this.Controls.Add(this.m_ListViewInstruments);
            this.Name = "SharedInstrumentsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_LabelUserText;
        private System.Windows.Forms.ListView m_ListViewInstruments;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
