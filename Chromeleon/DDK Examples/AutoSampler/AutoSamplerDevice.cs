/////////////////////////////////////////////////////////////////////////////
//
// AutoSamplerDevice.cs
// ////////////////////
//
// AutoSampler Chromeleon DDK Code Example
//
// Device class for the AutoSampler.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Data.SqlTypes;
using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.AutoSampler
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class AutoSamplerDevice
    {
        #region Data Members

        /// Our IDDK
        private IDDK m_MyCmDDK;

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// Our inject handler.
        private IInjectHandler m_InjectHandler;

        private double m_Volume;
        private int m_Position;

        private System.Timers.Timer m_InjectionTimer = new System.Timers.Timer(20000);

        IStringProperty m_TrayDesciptionProperty;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice Create(IDDK cmDDK, string name)
        {
            m_MyCmDDK = cmDDK;

            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "Autosampler device.");

            ITypeDouble tVolume = cmDDK.CreateDouble(0.1, 10.0, 1);
            tVolume.Unit = "µL";

            ITypeInt tPosition = cmDDK.CreateInt(1, 10);
            for (int i = 1; i <= 10; i++)
            {
                tPosition.AddNamedValue("RA" + i.ToString(), i);
            }
            tPosition.NamedValuesOnly = false;

            m_InjectHandler = m_MyCmDevice.CreateInjectHandler(tVolume, tPosition);

            m_InjectHandler.PositionProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetPosition);

            m_InjectHandler.VolumeProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetVolume);

            m_InjectHandler.InjectCommand.OnCommand += new CommandEventHandler(OnInject);

            ICommand simulateVialNotFoundCommand =
                m_MyCmDevice.CreateCommand("SimulateVialNotFound", "Simulate a vial not found error");
            simulateVialNotFoundCommand.OnCommand += new CommandEventHandler(simulateVialNotFoundCommand_OnCommand);

            ICommand modifyPositionTypeCommand =
                m_MyCmDevice.CreateCommand("ModifyPositionType", "Changes the data type of the Position property and the Inject.Position parameter");
            modifyPositionTypeCommand.OnCommand += new CommandEventHandler(modifyPositionTypeCommand_OnCommand);

            ICommand modifyVolumeTypeCommand =
                m_MyCmDevice.CreateCommand("ModifyVolumeType", "Changes the data type of the Volume property and the Inject.Volume parameter");
            modifyVolumeTypeCommand.OnCommand += new CommandEventHandler(modifyVolumeTypeCommand_OnCommand);

            ITypeString tTrayDescription = cmDDK.CreateString(500); // ensure length is sufficient, but don't be too generous
            m_TrayDesciptionProperty = m_MyCmDevice.CreateStandardProperty(StandardPropertyID.TrayDescription, tTrayDescription);
            m_TrayDesciptionProperty.AuditLevel = AuditLevel.Service;
            m_TrayDesciptionProperty.Update(GenerateTrayDescription());

            // In this example driver, simulating the injection process
            // is handled by a timer started by the Inject command.
            // When the timer has elapsed the inject response is generated.
            m_InjectionTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_InjectionTimer_Elapsed);
            m_InjectionTimer.AutoReset = false;

            return m_MyCmDevice;
        }

        private String GenerateTrayDescription()
        {
            // Convert tray layout information to Chromeleon's tray description language.
            // The tray layout can either be part of your configuration XML, in which case this is static information,
            // or the driver can update the tray description dynamically when communication with the hardware is established
            // and the hardware reports a new tray layout.
            // For simplicity, the code below just shows a small subset of what can be done.

            IRackLayoutDescriptionBuilder rackLayoutDescriptionBuilder = m_InjectHandler.CreateRackLayoutDescriptionBuilder();
            if (rackLayoutDescriptionBuilder == null)
                return String.Empty;

            ITray tray = rackLayoutDescriptionBuilder.AddTray("TwoSubTrays", 0, 0);
            tray.SetRectangularForm(0.0, 0.0, 332.3, 250.5);
            ITubesGroup group1 = tray.AddTubesGroup();
            group1.SetEllipsoidForm(16.2, 16.2);
            IRectangularTubeCollection rectangle1 = group1.AddRectangle(8, 12, 177.1, 17.0, 18.0, 18.0);
            rectangle1.AddEnumeration(EnumerationScheme.SawH, 1, 1);
            ITubesGroup group2 = tray.AddTubesGroup();
            group2.SetEllipsoidForm(14.3, 14.3);
            IRectangularTubeCollection rectangle2 = group2.AddRectangle(10, 15, 4.0, 6.0, 15.8, 15.9);
            rectangle2.AddEnumeration(EnumerationScheme.SawH, 151, 151);

            return rackLayoutDescriptionBuilder.Description;
        }

        void modifyPositionTypeCommand_OnCommand(CommandEventArgs args)
        {
            ITypeInt tNewPositionType = m_MyCmDDK.CreateInt(0, 20);
            for (int i = 0; i <= 20; i++)
            {
                tNewPositionType.AddNamedValue("X" + i.ToString(), i);
            }
            tNewPositionType.NamedValuesOnly = true;

            ITypeDouble tOldVolumeType = m_InjectHandler.VolumeProperty.DataType; // remains unchanged
            m_InjectHandler.UpdateDataTypes(tOldVolumeType, tNewPositionType);
        }

        void modifyVolumeTypeCommand_OnCommand(CommandEventArgs args)
        {
            ITypeDouble tNewVolumeType = m_MyCmDDK.CreateDouble(0.00, 20.00, 2);
            tNewVolumeType.Unit = "mL";
            double[] legalValues = new double[3];
            legalValues[0] = 0.00;
            legalValues[1] = 10.00;
            legalValues[2] = 20.00;
            tNewVolumeType.LegalValues = legalValues;

            ITypeInt tOldPositionType = m_InjectHandler.PositionProperty.DataType; // remains unchanged
            m_InjectHandler.UpdateDataTypes(tNewVolumeType, tOldPositionType);
        }

        /// <summary>
        /// Called when the injection has finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_InjectionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_MyCmDevice.AuditMessage(AuditLevel.Message, "The sampler has finished the injection, sending inject response.");
            m_InjectHandler.NotifyInjectResponse();
        }

        /// <summary>
        /// Called on "SimulateVialNotFound" command
        /// </summary>
        /// <param name="args"></param>
        void simulateVialNotFoundCommand_OnCommand(CommandEventArgs args)
        {
            m_InjectHandler.NotifyMissingVial("Error: Vial not found!");
            m_InjectionTimer.Stop();
        }

        /// <summary>
        /// IDriver implementation: Connect
        /// When we are connected, we initialize properties that are visible in the connected state only.
        /// </summary>
        internal void OnConnect()
        {
        }

        /// <summary>
        /// IDriver implementation: Disconnect
        /// When we are disconnected, we clear properties that are visible in the connected state only.
        /// </summary>
        internal void OnDisconnect()
        {
        }

        private void OnSetPosition(SetPropertyEventArgs args)
        {
            SetIntPropertyEventArgs setPositionArgs =
                args as SetIntPropertyEventArgs;

            if (setPositionArgs.NewValue.HasValue)
            {
                m_Position = setPositionArgs.NewValue.Value;
                m_InjectHandler.PositionProperty.Update(m_Position);
            }
            else
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Error, "Invalid position.");
            }
        }

        private void OnSetVolume(SetPropertyEventArgs args)
        {
            SetDoublePropertyEventArgs setVolumeArgs =
                args as SetDoublePropertyEventArgs;

            if (setVolumeArgs.NewValue.HasValue)
            {
                m_Volume = setVolumeArgs.NewValue.Value;
                m_InjectHandler.VolumeProperty.Update(m_Volume);
            }
            else
            {
                m_MyCmDevice.AuditMessage(AuditLevel.Error, "Invalid volume.");
            }
        }

        /// <summary>
        /// Called on "Inject" command
        /// </summary>
        /// <param name="args"></param>
        private void OnInject(CommandEventArgs args)
        {
            IDoubleParameterValue vVolume =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Volume"))
                as IDoubleParameterValue;

            if (vVolume != null && vVolume.Value.HasValue)
            {
                m_Volume = vVolume.Value.Value;
                m_InjectHandler.VolumeProperty.Update(m_Volume);
            }

            IIntParameterValue vPosition =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Position"))
                as IIntParameterValue;

            if (vPosition != null && vPosition.Value.HasValue)
            {
                m_Position = vPosition.Value.Value;
                m_InjectHandler.PositionProperty.Update(m_Position);
            }

            m_MyCmDevice.AuditMessage(AuditLevel.Message,
                "Injecting " + m_Volume.ToString() +
                " ml from Position: " + m_Position.ToString());

            // Start the injection timer that will generate the inject response after
            // after 20 seconds delay.
            m_InjectionTimer.Start();
        }
    }
}
