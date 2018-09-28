// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using CmHelpers;
using Dionex.Chromeleon.DDK;
using Dionex.Chromeleon.Symbols;

namespace MyCompany.Demo
{
    internal class AutoSampler : Device  // Also see examples AutoSampler, PreparingSampler
    {
        #region Fields
        public enum BlankInjectionAction
        {
            Skip,
            Inject,
        }

        private readonly AutoSamplerProperties m_Properties;

        private readonly IInjectHandler m_InjectHandler;
        private int m_Position;
        private string m_PositionName;
        private double m_Volume;
        private readonly UnitConversion.PhysUnitEnum m_VolumeUnit;
        private readonly string m_VolumeUnitName;
        private readonly IStringProperty m_RackDesciptionProperty;

        #region Rack Info
        private class RackInfo
        {
            public readonly string TubePositionNamePrefix;
            public readonly int TubeColumnsCount;
            public readonly int TubeRowsCount;
            public readonly int TubeFirstNumber;

            public RackInfo(string tubePositionNamePrefix, int tubeFirstNumber, int tubeColumnsCount, int tubeRowsCount)
            {
                TubeFirstNumber = tubeFirstNumber;
                TubeColumnsCount = tubeColumnsCount;
                TubeRowsCount = tubeRowsCount;
                TubePositionNamePrefix = tubePositionNamePrefix;
            }
        }

        private readonly List<RackInfo> m_RackInfos = new List<RackInfo>()
        {
            new RackInfo("A", 151, 10, 15),
            new RackInfo("B",   1,  8, 12),
        };
        #endregion

        private ISequencePreflight m_CurrentSequence;
        private double m_InstrumentMethodToalTimeMin;
        #endregion

        #region Constructor
        public AutoSampler(IDriverEx driver, IDDK ddk, Config.AutoSampler config, string id)
            : base(driver, ddk, typeof(AutoSampler).Name, id, config.Name)
        {
            Log.TaskBegin(Id);
            try
            {
                m_Properties = new AutoSamplerProperties(m_DDK, m_Device);

                ITypeDouble volumeType = m_DDK.CreateDouble(0.1, 30, 3);
                volumeType.Unit = UnitConversionEx.PhysUnitName(UnitConversion.PhysUnitEnum.PhysUnit_MicroLiter); // µl

                int positionMax = m_RackInfos.Sum(item => item.TubeColumnsCount * item.TubeRowsCount);
                ITypeInt positionType = m_DDK.CreateInt(1, positionMax);
                positionType.NamedValuesOnly = false;
                int position = positionType.Minimum.GetValueOrDefault() - 1;
                foreach (RackInfo rack in m_RackInfos)
                {
                    int tubeNumber = rack.TubeFirstNumber - 1;

                    for (int row = 1; row <= rack.TubeRowsCount; row++)
                    {
                        for (int col = 1; col <= rack.TubeColumnsCount; col++)
                        {
                            position++;
                            tubeNumber++;
                            string positionName = rack.TubePositionNamePrefix + tubeNumber.ToString();
                            positionType.AddNamedValue(positionName, tubeNumber);
                        }
                    }
                }

                m_InjectHandler = m_Device.CreateInjectHandler(volumeType, positionType);

                m_RackDesciptionProperty = m_Device.CreateStandardProperty(StandardPropertyID.TrayDescription, m_DDK.CreateString(1000));
                m_RackDesciptionProperty.AuditLevel = AuditLevel.Service;
                string rackDescription = GetRackDescription();
                if (m_RackDesciptionProperty.DataType.Length < rackDescription.Length)
                    throw new InvalidOperationException("m_RackDesciptionProperty length " + m_RackDesciptionProperty.DataType.Length + " is less than the needed " + rackDescription.Length.ToString());
                m_RackDesciptionProperty.Update(rackDescription);

                m_Position = -1;
                m_VolumeUnitName = m_InjectHandler.VolumeProperty.DataType.Unit;  // µl
                m_VolumeUnit = UnitConversionEx.PhysUnitFindName(m_VolumeUnitName);

                m_InjectHandler.PositionProperty.OnSetProperty += OnPropertyPositionSet;
                m_InjectHandler.VolumeProperty.OnSetProperty += OnPropertyVolumeSet;

                m_InjectHandler.InjectCommand.OnCommand += OnCommandInjectHandlerInject;

                m_Device.OnBatchPreflightBegin += OnDeviceBatchPreflightBegin;

                m_Device.OnPreflightEnd += OnDevicePreflightEnd;

                m_Device.OnTransferPreflightToRun += OnDeviceTransferPreflightToRun;

                m_Device.OnSequenceStart += OnDeviceSequenceStart;
                m_Device.OnSequenceChange += OnDeviceSequenceChange;
                m_Device.OnSequenceEnd += OnDeviceSequenceEnd;

                m_Device.OnBroadcast += OnDeviceBroadcast;

                m_Driver.OnConnected += OnDriverConnected;
                m_Driver.OnDisconnected += OnDriverDisconnected;

                Log.TaskEnd(Id);
            }
            catch (Exception ex)
            {
                Log.TaskEnd(Id, ex);
                throw;
            }
        }
        #endregion

        #region Get Rack Description
        private string GetRackDescription()
        {
            // Convert rack layout information to Chromeleon's rack description language.
            // The rack layout can either be part of your configuration XML, in which case this is static information,
            // or the driver can update the rack description dynamically when communication with the hardware is established
            // and the hardware reports a new rack layout.
            // DDKV1.chm - search for Rack
            // C:\Thermo\Chromeleon\Bin\RackView.TestHarness.exe - The example below is the "Rectangle Tray" rack

            IRackLayoutDescriptionBuilder builder = m_InjectHandler.CreateRackLayoutDescriptionBuilder();
            if (builder == null)
                return string.Empty;

            ITray rack = builder.AddTray(string.Empty, 0, 0);
            rack.SetRectangularForm(0, 0, 332.3, 250.5);

            // Optional
            //rack.SetBorderStyle(double lineWidth, double red, double green, double blue);
            //rack.SetFillColor(double red, double green, double blue);

            int rackIndex = -1;
            RackAddGroupLeft(rack, m_RackInfos[++rackIndex]);
            RackAddGroupRight(rack, m_RackInfos[++rackIndex]);

            // tray{"TwoSubRacks";pos{0;0};form{rect;0;0;332.3;250.5};tubes{form{elli;14.3;14.3};rect{10;15;4;6;15.8;15.9;enum{SawH;"A%p%";151;151}}};tubes{form{elli;16.2;16.2};rect{8;12;177.1;17;18;18;enum{SawH;"B%p%";1;1}}}}
            string result = builder.Description;
            return result;
        }

        private static void RackAddGroupLeft(ITray rack, RackInfo rackInfo)
        {
            ITubesGroup group = rack.AddTubesGroup();
            group.SetEllipsoidForm(14.3, 14.3);  // Each tube/position is represented by a circle

            //                                                        10 tubes on X (horizontal)
            //                                                                                   15 tubes on Y (vertical)
            //                                                                                                           x, y position of the 1st tube
            //                                                                                                                 x   , y distance between tubes in x- and y-direction
            IRectangularTubeCollection rectangle = group.AddRectangle(rackInfo.TubeColumnsCount, rackInfo.TubeRowsCount, 4, 6, 15.8, 15.9);

            // EnumerationScheme.SawH means the tubes are enumerated from left to right, top to bottom
            //                                                                             A        151 - 1st tube number   , 151 - the global numerical index starts from 151 (the 1st tube)
            rectangle.AddEnumeration(EnumerationScheme.SawH, rackInfo.TubePositionNamePrefix + "%p%", rackInfo.TubeFirstNumber, rackInfo.TubeFirstNumber);
            /*
                10 columns x 15 rows (defined in group.AddRectangle(10, 15)

                151 152 153 154 155 156 157 158 159 160
                161 162 163 164 165 166 167 168 169 170
                . . .
                291 292 293 294 295 296 297 298 299 300

                Also, see int Position setter
            */
        }

        private static void RackAddGroupRight(ITray rack, RackInfo rackInfo)
        {
            ITubesGroup group = rack.AddTubesGroup();
            group.SetEllipsoidForm(16.2, 16.2);

            IRectangularTubeCollection rectangle = group.AddRectangle(rackInfo.TubeColumnsCount, rackInfo.TubeRowsCount, 177.1, 17, 18, 18);
            rectangle.AddEnumeration(EnumerationScheme.SawH, rackInfo.TubePositionNamePrefix + "%p%", rackInfo.TubeFirstNumber, rackInfo.TubeFirstNumber);
        }
        #endregion

        #region Properties
        public bool Ready
        {
            get { return Property.GetBool(m_Properties.Ready.Value.GetValueOrDefault()); }
            private set
            {
                if (value == Ready)
                {
                    return;
                }
                int valueNumber = Property.GetBoolNumber(value);
                m_Properties.Ready.Update(valueNumber);
                Log.PropertyChanged(Id, m_Properties.Ready.Name, value, CallerMethodName);
            }
        }

        public int Position
        {
            [DebuggerStepThrough]
            get { return m_Position; }
            private set
            {
                if (value == m_Position)
                {
                    return;
                }
                m_Position = value;
                m_InjectHandler.PositionProperty.Update(value);

                m_PositionName = Property.GetValueName(m_InjectHandler.PositionProperty);
                if (string.IsNullOrEmpty(m_PositionName))
                {
                    DebuggerBreakIfIsAttached(Id + " Cannot find the name for Position = " + value.ToString(CultureInfo.InvariantCulture));
                }
                Log.PropertyChanged(Id, m_InjectHandler.PositionProperty.Name, value.ToString(CultureInfo.InvariantCulture), "Name", m_PositionName, CallerMethodName);
                // Position Name
                //   1      B1
                //   2      B3
                // . . .
                //  96      B96
                // 151      A151
                // 153      A152
                // . . .
                // 300      A300
            }
        }

        public double Volume
        {
            [DebuggerStepThrough]
            get { return m_Volume; }
            private set
            {
                if (value == m_Volume)
                {
                    return;
                }
                m_Volume = value;
                m_InjectHandler.VolumeProperty.Update(value);
                Log.PropertyChanged(Id, m_InjectHandler.VolumeProperty, CallerMethodName);
            }
        }
        #endregion

        #region Events Driver Connected / Disconnected
        private void OnDriverConnected(object sender, EventArgs args)
        {
            Position = m_RackInfos[1].TubeFirstNumber;
            Volume = 1;
        }

        private void OnDriverDisconnected(object sender, EventArgs args)
        {
        }
        #endregion

        #region Events Properties
        private void OnPropertyPositionSet(SetPropertyEventArgs args)
        {
            Nullable<int> value = Property.GetIntNullable(args);
            if (value == null)
            {
                AuditMessage(AuditLevel.Error, "Invalid position.");
                return;
            }
            Position = value.GetValueOrDefault();
        }

        private void OnPropertyVolumeSet(SetPropertyEventArgs args)
        {
            Nullable<double> value = Property.GetDoubleNullable(args);
            if (value == null)
            {
                AuditMessage(AuditLevel.Error, "Invalid volume.");
                return;
            }
            Volume = value.GetValueOrDefault();
        }
        #endregion

        private void OnCommandInjectHandlerInject(CommandEventArgs args)
        {
            Nullable<int> position;
            if (Property.TryGetParameterIntNullable(args, "Position", out position))
            {
                if (position != null)
                {
                    Position = position.GetValueOrDefault();
                }
            }

            Nullable<double> volume;
            if (Property.TryGetParameterDoubleNullable(args, "Volume", out volume))
            {
                if (volume != null)
                {
                    Volume = volume.GetValueOrDefault();
                }
            }

            BlankInjectionAction blankInjectionAction = BlankInjectionAction.Skip;
            Nullable<int> blankInjection;
            if (Property.TryGetParameterIntNullable(args, "Blank", out blankInjection))
            {
                if (blankInjection != null)
                {
                    int blankInjectionNumber = blankInjection.GetValueOrDefault();
                    if (!Enum.IsDefined(typeof(BlankInjectionAction), blankInjectionNumber))
                        throw new InvalidOperationException("Invalid inject command parameter Blank = " + blankInjectionNumber.ToString());
                    blankInjectionAction = (BlankInjectionAction)blankInjectionNumber;
                }
            }

            string text = "Position " + Position.ToString() + " - " + m_PositionName + ", " +
                          "Volume " + Volume.ToString() + " " + m_InjectHandler.VolumeProperty.DataType.Unit +
                          (blankInjection != null ? ", Blank Injection " + blankInjectionAction.ToString() : string.Empty);

            AuditMessage(AuditLevel.Message, "Injection Begin " + text);
            Action task = (() =>
            {
                Thread.Sleep(5000);
                AuditMessage(AuditLevel.Message, "Injection End - m_InjectHandler.NotifyInjectResponse()");
                m_InjectHandler.NotifyInjectResponse();  // After the injection has finished, send the inject response - triggers OnBroadcast(Broadcast.InjectResponse)
            });

            AsyncCallback taskStartCallback = (ar =>
            {
                try
                {
                    task.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    AuditMessage(AuditLevel.Error, "Wash error: " + ex.Message);
                }
            });

            task.BeginInvoke(taskStartCallback, null);
        }

        private void OnDevicePreflightEnd(PreflightEventArgs args)
        {
            m_InstrumentMethodToalTimeMin = args.RunContext.ProgramTime.Minutes;
        }

        private void OnDeviceTransferPreflightToRun(PreflightEventArgs args)
        {
            Log.WriteLine(Id, "ProgramTime.Minutes = " + args.RunContext.ProgramTime.Minutes.ToString());

            if (m_CurrentSequence == null)
            {
                return;
            }

            bool hasCommandStep = false;
            foreach (IProgramStep step in args.RunContext.ProgramSteps)
            {
                hasCommandStep = step is ICommandStep;
                if (hasCommandStep)
                {
                    break;
                }
            }
            if (!hasCommandStep)
            {
                return;
            }

            bool lockNextInjection = false;
            if (lockNextInjection)
            {
                // if injection overlap - lock the next injection
                if (m_CurrentSequence.RunningIndex + 1 < m_CurrentSequence.Samples.Count)
                {
                    m_CurrentSequence.LastPreparingIndex = m_CurrentSequence.RunningIndex + 1;  // Lock next injection
                    //m_CurrentSequence.LastPreparingIndex = m_CurrentSequence.Samples.Count - 1;  // Lock all injections
                }
            }
        }

        private void OnDeviceSequenceStart(SequencePreflightEventArgs args)
        {
            Log.WriteLine(Id);
            m_CurrentSequence = args.SequencePreflight;

            foreach (IBatchEntryPreflight batchEntry in args.SequencePreflight.Entries)
            {
                ISamplePreflight injection = (ISamplePreflight)batchEntry;
                Log.WriteLine(Id, "Injection \"" + injection.Name + "\" - Position = " + injection.Position + ", Volume = " + injection.InjectVolume.ToString());
            }

            if (args.SequencePreflight.CustomSequenceVariables != null)  // Always Null
            {
                foreach (ICustomVariable customSequenceVariable in args.SequencePreflight.CustomSequenceVariables)
                {
                    string text = "Custom Sequence Variable \"" + customSequenceVariable.Name + "\", Type " + customSequenceVariable.Type + ", " +
                                                                     "Value = " + Util.GetCustomVariableValue(customSequenceVariable);
                    Log.WriteLine(Id, text);
                }
            }
        }

        private void OnDeviceBatchPreflightBegin(BatchPreflightEventArgs args)
        {
            args.BatchPreflight.UpdatesWanted = true; // We want to have updates when the sequence has changed and the DDK to call OnDeviceSequenceChange
            Log.WriteLine(Id, "Set args.BatchPreflight.UpdatesWanted = " + args.BatchPreflight.UpdatesWanted.ToString());
        }

        private void OnDeviceSequenceChange(SequencePreflightEventArgs oldSequenceArgs, SequencePreflightEventArgs newSequenceArgs)
        {
            //  In order this event handler to be called, set args.BatchPreflight.UpdatesWanted = true in OnDeviceBatchPreflightSample
            // This is called when the current injection is completed and before starting the new one
            Log.WriteLine(Id);
            m_CurrentSequence = newSequenceArgs.SequencePreflight;

            foreach (IBatchEntryPreflight batchEntry in newSequenceArgs.SequencePreflight.Entries)
            {
                ISamplePreflight injection = (ISamplePreflight)batchEntry;
                Log.WriteLine(Id, "Injection \"" + injection.Name + "\" - Position = " + injection.Position + ", Volume = " + injection.InjectVolume.ToString());
            }
        }

        private void OnDeviceSequenceEnd(SequencePreflightEventArgs args)
        {
            Log.WriteLine(Id);
            m_CurrentSequence = null;  // After this event the current sequence becomes invalid.
        }

        private void OnDeviceBroadcast(BroadcastEventArgs args)
        {
            switch (args.Broadcast)
            {
                case Broadcast.Samplestart:
                    {
                        // The sampler may need to wash after run
                        // When the instrument method has been processed and the instrument controller has finished acquisition
                        // it waits until all devices on the instrument have set IDevice.DelayTermination = false.
                        // The instrument controller maintains a timeout for each device that has set IDevice.DelayTermination = true.
                        // If a device doesn't reset IDevice.DelayTermination after IDevice.TerminationTimeout [sec] the system emits an Abort error, which terminates a running queue (if one is running).
                        // To ensure proper operation IDevice.DelayTermination is reset to false before Broadcast.Samplestart
                        // A device must therefore set IDevice.DelayTermination = true in it's Broadcast.Samplestart handler.
                        m_Device.TerminationTimeout = 60;  // seconds
                        //m_Device.DelayTermination = true;
                        if (m_Device.DelayTermination)
                        {
                            Log.WriteLine(Id, "Set DelayTermination = " + m_Device.DelayTermination.ToString() + ", TerminationTimeout = " + m_Device.TerminationTimeout.ToString() + " sec");

                            Action task = (() =>
                            {
                                //int sleepTimeMS = (int)TimeSpan.FromMinutes(m_InstrumentMethodToalTimeMin + 0.1).TotalMilliseconds;  // 0.1 =  0.1 * 60 = 6 sec
                                int sleepTimeMS = (int)TimeSpan.FromMinutes(m_InstrumentMethodToalTimeMin).TotalMilliseconds + (m_Device.TerminationTimeout / 2) * 1000;
                                Log.WriteLine(Id, "DelayTermination - Begin Sleep " + TimeSpan.FromMilliseconds(sleepTimeMS).ToString() + " " +
                                                           "(Injection total time " + m_InstrumentMethodToalTimeMin.ToString() + " min)");
                                Thread.Sleep(sleepTimeMS);
                                m_Device.DelayTermination = false;
                                Log.WriteLine(Id, "DelayTermination - End");
                                // After this Broadcast.Sampleend is broadcast
                            });

                            AsyncCallback taskStartCallback = (ar =>
                            {
                                try
                                {
                                    task.EndInvoke(ar);
                                }
                                catch (Exception ex)
                                {
                                    AuditMessage(AuditLevel.Error, "Wash error: " + ex.Message);
                                }
                            });

                            task.BeginInvoke(taskStartCallback, null);
                        }
                        break;
                    }
                case Broadcast.Sampleend:
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
