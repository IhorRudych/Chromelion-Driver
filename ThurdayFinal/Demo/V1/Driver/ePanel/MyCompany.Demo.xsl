<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="lang"/>
  <xsl:param name="resourceNode"/>
  <xsl:variable name="Resources" select="$resourceNode"/>

  <xsl:include href="PanelSelectorCommon.xsl"/>

  <!--
    Properties which values can be evaluated (defined in class Symbol, class ConstantName)
      "DeviceType"
      "DriverID"
      "LicenseProvider"
      "ModelNo"
      "ModelVariant"
      "SampleLoadingPump"
      "Location"
      "ValveType"
      "FirmwareVersion"
      "HardwareVersion"
      "PumpDeviceName"
      "ModuleHardwareRevision"
      "InternalName"
  -->

  <xsl:template match="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]">

    <!-- Home
    -->
    <xsl:element name="HomePanels">
      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo</xsl:attribute>
        <xsl:attribute name="Priority">10</xsl:attribute>
        <xsl:attribute name="Block">Right</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Heater</xsl:attribute>
        <xsl:attribute name="Priority">20</xsl:attribute>
        <xsl:attribute name="Block">Right</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Heater')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Heater']]/@Name"/>
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

    <!-- Device
    -->
    <xsl:element name="DevicePanels">
      <!-- Home ePanel for editing
      -->
      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo</xsl:attribute>
        <xsl:attribute name="Priority">100</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Heater</xsl:attribute>
        <xsl:attribute name="Priority">200</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Heater')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Heater']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Pump</xsl:attribute>
        <xsl:attribute name="Priority">300</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Pump')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Pump']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.AutoSampler</xsl:attribute>
        <xsl:attribute name="Priority">400</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('AutoSampler')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='AutoSampler']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Detector</xsl:attribute>
        <xsl:attribute name="Priority">500</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Detector')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Detector']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Detector_Channel_1')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device/Device[Property[@Name='DeviceType'   and @Value='DetectorChannel'] and
                                                                            Property[@Name='Location'     and @Value='1'              ]]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Detector_Channel_2')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device/Device[Property[@Name='DeviceType'   and @Value='DetectorChannel'] and
                                                                            Property[@Name='Location'     and @Value='2'              ]]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Detector_Channel_3')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device/Device[Property[@Name='DeviceType'   and @Value='DetectorChannel'] and
                                                                            Property[@Name='Location'     and @Value='3'              ]]/@Name"/>
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

    <!-- Startup
    -->
    <xsl:element name="StartupPanels">
      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo</xsl:attribute>
        <xsl:attribute name="Priority">1000</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Heater</xsl:attribute>
        <xsl:attribute name="Priority">1100</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Heater')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Heater']]/@Name"/>
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

    <!-- Status - used by Xcalibur
    -->
    <xsl:element name="StatusPanels">
      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo</xsl:attribute>
        <xsl:attribute name="Priority">2000</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
      </xsl:element>

      <xsl:element name="Panel">
        <xsl:attribute name="Name">MyCompany.Demo.Heater</xsl:attribute>
        <xsl:attribute name="Priority">2100</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Demo')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Demo']]/@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName"  select="string('Heater')"/>
          <xsl:with-param name="deviceName" select="//Symbols/Device[Property[@Name='DeviceType' and @Value='Heater']]/@Name"/>
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

  </xsl:template>
</xsl:stylesheet>
