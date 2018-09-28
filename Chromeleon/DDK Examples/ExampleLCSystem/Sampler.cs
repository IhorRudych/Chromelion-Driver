/////////////////////////////////////////////////////////////////////////////
//
// Sampler.cs
// //////////
//
// ExampleLCSystem Chromeleon DDK Code Example
//
// Sampler device class for the ExampleLCSystem.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;
using System.Threading;

namespace MyCompany.ExampleLCSystem
{
    internal class Sampler
    {
        #region Data Members

        private IDDK m_DDK;
        private IDevice m_Device;

        private IInjectHandler m_InjectHandler;
        private IIntProperty m_ReadyProperty;

        private double m_Volume;
        private int m_Position;

        #endregion

        internal IDevice Device
        {
            get { return m_Device; }
        }

        internal void Create(IDDK cmDDK, string deviceName)
        {
            m_DDK = cmDDK;
            m_Device = m_DDK.CreateDevice(deviceName, "Sampler device");

            IStringProperty typeProperty =
                m_Device.CreateProperty("DeviceType",
                "The DeviceType property tells us which component we are talking to.",
                m_DDK.CreateString(20));
            typeProperty.Update("Sampler");


            ITypeDouble tVolume = m_DDK.CreateDouble(0.1, 10.0, 1);
            ITypeInt tPosition = m_DDK.CreateInt(1, 10);

            m_InjectHandler = m_Device.CreateInjectHandler(tVolume, tPosition);

            m_InjectHandler.PositionProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetPosition);

            m_InjectHandler.VolumeProperty.OnSetProperty +=
                new SetPropertyEventHandler(OnSetVolume);

            m_InjectHandler.InjectCommand.OnCommand += new CommandEventHandler(OnInject);

            //Create ready property
            ITypeInt tReady = m_DDK.CreateInt(0, 1);
            //Add named values
            tReady.AddNamedValue("NotReady", 0);
            tReady.AddNamedValue("Ready", 1);
            m_ReadyProperty = m_Device.CreateStandardProperty(StandardPropertyID.Ready, tReady);
            m_ReadyProperty.Update(1);

            m_Device.OnBroadcast += new BroadcastEventHandler(OnBroadcast);
        }

        void OnBroadcast(BroadcastEventArgs args)
        {
            if (args != null)
            {
                switch (args.Broadcast)
                {
                    case Broadcast.Sampleend: m_ReadyProperty.Update(1); //signal sampler ready
                        break;
                }
            }
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
                m_Device.AuditMessage(AuditLevel.Error, "Invalid position.");
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
                m_Device.AuditMessage(AuditLevel.Error, "Invalid volume.");
            }
        }

        private void OnInject(CommandEventArgs args)
        {
            m_ReadyProperty.Update(0); //set ready state to not ready
            IDoubleParameterValue vVolume =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Volume"))
                as IDoubleParameterValue;

            if (vVolume != null &&			// if this parameter is set...
                vVolume.Value.HasValue)		// ... to some value
            {
                m_Volume = vVolume.Value.Value;
                m_InjectHandler.VolumeProperty.Update(m_Volume);
            }

            IIntParameterValue vPosition =
                args.ParameterValue(m_InjectHandler.InjectCommand.FindParameter("Position"))
                as IIntParameterValue;

            if (vPosition != null &&		// if this parameter is set...
                vPosition.Value.HasValue)	// ... to some value
            {
                m_Position = vPosition.Value.Value;
                m_InjectHandler.PositionProperty.Update(m_Position);
            }

            m_Device.AuditMessage(AuditLevel.Message,
                "Injecting " + m_Volume.ToString() +
                " ml from Position: " + m_Position.ToString());

            // The injection takes a while...
            Thread.Sleep(5000);

            m_Device.AuditMessage(AuditLevel.Message, "Injection done.");

            // After the injection has finished, we send the inject response. 
            m_InjectHandler.NotifyInjectResponse();
        }

        internal void OnConnect()
        {
            m_InjectHandler.PositionProperty.Update(1);
            m_InjectHandler.VolumeProperty.Update(1.0);
        }

        internal void OnDisconnect()
        {
        }
    }
}
