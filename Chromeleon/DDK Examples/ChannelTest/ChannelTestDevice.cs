/////////////////////////////////////////////////////////////////////////////
//
// ChannelTestDevice.cs
// ////////////////////
//
// ChannelTest Chromeleon DDK Code Example
//
// Master device class for the ChannelTest example driver.
// The ChannelTest driver creates two channel devices that
// generate a cosinus waveform during acquisition and
// a PDA channel device.
//
// Copyright (C) 2005-2016 Thermo Fisher Scientific
//
/////////////////////////////////////////////////////////////////////////////

using Dionex.Chromeleon.DDK;					// Chromeleon DDK Interface
using Dionex.Chromeleon.Symbols;				// Chromeleon Symbol Interface

namespace MyCompany.ChannelTest
{
    /// <summary>
    /// Device class implementation
    /// The IDevice instances are actually created by the DDK using IDDK.CreateDevice.
    /// It is recommended to implement an internal class for each IDevice that a
    /// driver creates.
    /// </summary>
    internal class ChannelTestDevice
    {
        #region Data Members

        // the standard property Ready 
        internal IIntProperty ReadyProperty;

        /// Our driver instance
        private ChannelTestDriver m_Driver;

        /// Our IDevice
        private IDevice m_MyCmDevice;

        /// This command allows to simulate a hardware error during
        /// acquisition.
        private ICommand m_HardwareErrorCommand;

        #endregion

        /// <summary>
        /// Create our Dionex.Chromeleon.Symbols.IDevice and our Properties and Commands
        /// </summary>
        /// <param name="cmDDK">The DDK instance</param>
        /// <param name="driver">The driver instance</param>
        /// <param name="name">The name for our device</param>
        /// <returns>our IDevice object</returns>
        internal IDevice OnCreate(ChannelTestDriver driver, IDDK cmDDK, string name)
        {
            m_Driver = driver;

            // Create our Dionex.Chromeleon.Symbols.IDevice
            m_MyCmDevice = cmDDK.CreateDevice(name, "This is the master device of the ChannelTest driver.");
            m_MyCmDevice.ImmediateNotReady = true;
            // NOTE:
            //      Setting 'ImmediateNotReady' to true on the main device toggles the 'Ready' property for 5s(default) to false when entering TransferPreflightToRun, unless one updates the ready property explicitly.
            //      Therefore it's also necessary to create a standard property 'Ready' which can be toggled and which can be added to instrument methods in a Wait statement.
            // More background:		
            //      In common LC/GC environments an injector is used in the instrument configuration and it's Inject command in the method would cause a natural delay.
            //       In our example driver we have no delay like this and can run into a kind of data exchange delay issue. 
            //      Setting the parameter FixedRate in a method updates also the channel parameters TimeStepFactorProperty and TimeStepDevisorProperty. 
            //      If setting the parameter FixedRate is immediately followed by an AcqOn command, the 'Real Time Kernel' might not be updated via interprocess communication soon enough and
            //      calculates the number of expected datapoints with old settings of TimeStepFactorProperty and TimeStepDevisorProperty. This might stop the data acquisition by 'OnDataFinished' when faulty 'enough' data points have been dumped.
            //      To avoid this in this example, following to steps in combination are necessary: 
            //          1) introducing a standard property 'Ready' which can be checked in the instrument method. 
            //          2) Set ChannelTest.ImmediateNotReady to true
            //          3) Adding a Wait ChannelTest.Ready in the method before AcqOn. 
            //      This provides enough time when setting FixedRate in our example to update the 'Real Time Kernel's TimeStepFactorProperty and TimeStepDevisorProperty.

            // Create a command for the hardware error simulation.
            m_HardwareErrorCommand =
                m_MyCmDevice.CreateCommand("HardwareError", "This command simulates a hardware error.");

            m_HardwareErrorCommand.OnCommand += new CommandEventHandler(m_HardwareErrorCommand_OnCommand);

            //Create ready property
            ITypeInt tReady = cmDDK.CreateInt(0, 1);
            //Add named values
            tReady.AddNamedValue("NotReady", 0);
            tReady.AddNamedValue("Ready", 1);
            ReadyProperty = m_MyCmDevice.CreateStandardProperty(StandardPropertyID.Ready, tReady);
            ReadyProperty.Update(1);

            return m_MyCmDevice;
        }

        void m_HardwareErrorCommand_OnCommand(CommandEventArgs args)
        {
            m_Driver.OnHardwareError();
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
    }
}
