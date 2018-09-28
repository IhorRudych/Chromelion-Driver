// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.Helpers;

namespace MyCompany.Demo.Detector.EditorPlugIn
{
    public partial class DetectorPage : UserControl, IInitPage
    {
        private readonly IDetector m_Detector;
        private IPage m_Page;
        private IEditMethod m_EditMethod;
        private ISymbol m_DeviceSymbol;
        private Util m_Util;

        private static readonly SeparationMethodStage m_CommandAutoZeroStageType = SeparationMethodStage.InjectPreparation;
        private static readonly string[] m_CommandAutoZeroOptions =
        {
            "No",
            "Yes",
        };
        private static readonly int m_CommandAutoZeroOption_Index_No = 0;
        private static readonly int m_CommandAutoZeroOption_Index_Yes = 1;

        public DetectorPage(IDetector detector)
        {
            InitializeComponent();
            m_Detector = detector;
            m_CommandAutoZeroOption.DataSource = m_CommandAutoZeroOptions;
        }

        public void Initialize(IPage page, IEditMethod editMethod)
        {
            m_Page = page;
            m_EditMethod = editMethod;
            m_DeviceSymbol = m_Page.Component.Symbol;
            m_Util = new Util(page, editMethod);

            InitializeCommandAutoZero();
            m_ChannelControl.InitializeChannels(m_Page, m_EditMethod);

            // Not used in this example. Can be used if there are more than 1 detector.
            m_Detector.SelectedChanged += OnDetectorSelectedChanged;

            m_Page.Component.PageEnterEvent += OnPageEnter;
            //m_Page.Component.PageLeaveEvent += OnPageLeave;
            m_Page.Component.PageValidationEvent += OnPageValidation;
            //m_Page.Component.WizardFinishEvent += OnPageWizardFinish;
        }

        private void InitializeCommandAutoZero()
        {
            ICommand commandAutoZero = m_DeviceSymbol.Children[CommandName.AutoZero] as ICommand;
            bool enableCommandAutoZero = commandAutoZero != null;  // Is Command Available

            if (m_EditMethod.Mode == EditMode.Wizard)
            {
            }
            else //                  EditMode.Editor
            {
                if (enableCommandAutoZero)
                {
                    enableCommandAutoZero = m_Util.Command.IsInScript(m_CommandAutoZeroStageType, commandAutoZero);
                }
            }

            m_CommandAutoZeroOption.SelectedIndex = enableCommandAutoZero ? m_CommandAutoZeroOption_Index_Yes : m_CommandAutoZeroOption_Index_No;
        }

        private void OnDetectorSelectedChanged(object sender, EventArgs e)
        {
            if (m_Page.Component.EditMethod.Mode == EditMode.Wizard)
            {
                m_Page.Visible = m_Detector.Selected;
                m_ChannelControl.NoDetector = !m_Detector.Selected;
            }
        }

        private void OnPageEnter(object sender, PageEnterArgs e)
        {
            InitializeCommandAutoZero();
        }

        private void OnPageValidation(object sender, PageValidationArgs e)
        {
            if (m_ChannelControl.ChannelCount > 0 && m_ChannelControl.NoChannelSelected)
            {
                if (!m_ChannelControl.UserWantContinue())
                {
                    e.Fail("No Channel Selected!");
                    return;
                }
            }

            // Demo: Create Equilibration stage with time step = -1 minutes and (optional) add a command
            //m_Util.Stage.Create(SeparationMethodStage.Equilibration, -1, CommandName.AutoZero);

            m_ChannelControl.WriteScripts();

            bool addCommand = m_CommandAutoZeroOption.SelectedIndex != m_CommandAutoZeroOption_Index_No;
            m_Util.Command.ScriptUpdate(m_CommandAutoZeroStageType, CommandName.AutoZero, addCommand);
            m_Util.Command.ScriptUpdateWait(m_CommandAutoZeroStageType, SymbolName.Ready, addCommand);
        }
    }
}
