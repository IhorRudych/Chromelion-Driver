using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.Chromeleon.DDK.V2.Symbols.DeviceHelper;
using Dionex.DDK.V2.TimeTableDriver.EditorPlugIn.Properties;


namespace Dionex.DDK.V2.TimeTableDriver.EditorPlugIn
{
    /// <seealso cref="IInitPage"/>
    public partial class TimeGridPage : UserControl, IInitPage
    {
        /// <summary>
        /// Page contains only of the TemperatureControlComponent which will handle all settings and checks.
        /// </summary>
        public TimeGridPage()
        {
            InitializeComponent();
        }

        #region IInitPage Members
        /// <seealso cref="IInitPage.Initialize"/>
        public void Initialize(IPage page, IEditMethod editMethod)
        {
            ISymbol deviceSymbol = page.Component.Symbol;
            List<TimeGridColumnInfo> columnInfo = new List<TimeGridColumnInfo>();

            TimeGridColumnInfo currentColumnInfo = new TimeGridColumnInfo();
            IProperty valveState = deviceSymbol.Child("ValveState") as IProperty;
            Debug.Assert(valveState != null);
            currentColumnInfo.Property = valveState;
            currentColumnInfo.DisplayName = "Valve State";
            columnInfo.Add(currentColumnInfo);

            currentColumnInfo = new TimeGridColumnInfo();
            ICommand switchValveTo1 = deviceSymbol.Child("SwitchValveTo1") as ICommand;
            Debug.Assert(switchValveTo1 != null);
            currentColumnInfo.Command = switchValveTo1;
            currentColumnInfo.DisplayName = "Switch to '1'";
            columnInfo.Add(currentColumnInfo);

            currentColumnInfo = new TimeGridColumnInfo();
            ICommand switchValveTo2 = deviceSymbol.Child("SwitchValveTo2") as ICommand;
            Debug.Assert(switchValveTo2 != null);
            currentColumnInfo.Command = switchValveTo2;
            currentColumnInfo.DisplayName = "Switch to '2'";
            columnInfo.Add(currentColumnInfo);

            currentColumnInfo = new TimeGridColumnInfo();
            ICommand switchValve = deviceSymbol.Child("SwitchValve") as ICommand;
            Debug.Assert(switchValve != null);
            currentColumnInfo.Command = switchValve;
            currentColumnInfo.DisplayName = "Switch Valve Using Parameters";
            columnInfo.Add(currentColumnInfo);

            m_TimeGrid.Init(page.Component, columnInfo);
        }
        #endregion

        #region Event handlers
        private void OnAdd(object sender, EventArgs e)
        {
            m_TimeGrid.AddRow();
        }

        private void OnInsert(object sender, EventArgs e)
        {
            m_TimeGrid.InsertRowAtSelection();
        }

        private void OnRemove(object sender, EventArgs e)
        {
            m_TimeGrid.RemoveSelectedRows();
        }

        private void OnSort(object sender, EventArgs e)
        {
            m_TimeGrid.Sort();
        }

        #endregion
    }
}
