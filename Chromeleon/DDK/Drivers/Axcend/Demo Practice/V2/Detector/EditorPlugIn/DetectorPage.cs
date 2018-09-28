// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Dionex.Chromeleon.DDK.V2.InstrumentMethodEditor;
using Dionex.Chromeleon.DDK.V2.Symbols.Client;
using Dionex.DDK.V2.Helpers;

namespace MyCompany.Demo.Detector.EditorPlugIn
{
    public partial class DetectorPage : UserControl //, IInitPage
    {
        public DetectorPage(IDetector detector)
        {
            InitializeComponent();
        }
    }
}
