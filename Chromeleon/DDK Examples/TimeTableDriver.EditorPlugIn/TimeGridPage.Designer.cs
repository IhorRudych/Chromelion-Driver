namespace Dionex.DDK.V2.TimeTableDriver.EditorPlugIn
{
    partial class TimeGridPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeGridPage));
            this.m_TimeGrid = new Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components.TimeGridControl();
            this.m_ButtonSort = new System.Windows.Forms.Button();
            this.m_ButtonInsert = new System.Windows.Forms.Button();
            this.m_ButtonRemove = new System.Windows.Forms.Button();
            this.m_ButtonAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_TimeGrid
            // 
            resources.ApplyResources(this.m_TimeGrid, "m_TimeGrid");
            this.m_TimeGrid.Name = "m_TimeGrid";
            // 
            // m_ButtonSort
            // 
            resources.ApplyResources(this.m_ButtonSort, "m_ButtonSort");
            this.m_ButtonSort.Name = "m_ButtonSort";
            this.m_ButtonSort.UseVisualStyleBackColor = true;
            this.m_ButtonSort.Click += new System.EventHandler(this.OnSort);
            // 
            // m_ButtonInsert
            // 
            resources.ApplyResources(this.m_ButtonInsert, "m_ButtonInsert");
            this.m_ButtonInsert.Name = "m_ButtonInsert";
            this.m_ButtonInsert.UseVisualStyleBackColor = true;
            this.m_ButtonInsert.Click += new System.EventHandler(this.OnInsert);
            // 
            // m_ButtonRemove
            // 
            resources.ApplyResources(this.m_ButtonRemove, "m_ButtonRemove");
            this.m_ButtonRemove.Name = "m_ButtonRemove";
            this.m_ButtonRemove.UseVisualStyleBackColor = true;
            this.m_ButtonRemove.Click += new System.EventHandler(this.OnRemove);
            // 
            // m_ButtonAdd
            // 
            resources.ApplyResources(this.m_ButtonAdd, "m_ButtonAdd");
            this.m_ButtonAdd.Name = "m_ButtonAdd";
            this.m_ButtonAdd.UseVisualStyleBackColor = true;
            this.m_ButtonAdd.Click += new System.EventHandler(this.OnAdd);
            // 
            // TimeGridPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_ButtonSort);
            this.Controls.Add(this.m_ButtonInsert);
            this.Controls.Add(this.m_ButtonRemove);
            this.Controls.Add(this.m_ButtonAdd);
            this.Controls.Add(this.m_TimeGrid);
            this.Name = "TimeGridPage";
            this.ResumeLayout(false);

        }

        #endregion

        private Chromeleon.DDK.V2.InstrumentMethodEditor.Components.TimeGridControl m_TimeGrid;
        private System.Windows.Forms.Button m_ButtonSort;
        private System.Windows.Forms.Button m_ButtonInsert;
        private System.Windows.Forms.Button m_ButtonRemove;
        private System.Windows.Forms.Button m_ButtonAdd;



    }
}
