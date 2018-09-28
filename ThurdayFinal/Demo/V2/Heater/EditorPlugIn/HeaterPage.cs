// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor.Components;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.Helpers;

namespace MyCompany.Demo.Heater.EditorPlugIn
{
    public partial class HeaterPage : UserControl, IInitPage
    {
        private IPage m_Page;
        private IEditMethod m_EditMethod;
        private ISymbol m_DeviceSymbol;

        private MinMaxNominalController m_TemperatureController;
        private EnableController m_EnableController;

        public HeaterPage()
        {
            InitializeComponent();
        }

        public void Initialize(IPage page, IEditMethod editMethod)
        {
            m_Page = page;
            m_EditMethod = editMethod;
            m_DeviceSymbol = m_Page.Component.Symbol;

            m_TemperatureController = new MinMaxNominalController(m_Page.Component, m_DeviceSymbol.Child("Temperature"),
                                                                  m_TextBoxLowerLimit,
                                                                  m_TextBoxUpperLimit,
                                                                  m_TextBoxNominal);

            m_EnableController = new EnableController(m_Page.Component, m_TempControlCheckBox.Controller);
            m_EnableController.ControlledItems.AddRange(new Control[]
                                                        {
                                                            m_TextBoxLowerLimit,
                                                            m_TextBoxUpperLimit,
                                                            m_TextBoxNominal
                                                        });
        }
    }
}
