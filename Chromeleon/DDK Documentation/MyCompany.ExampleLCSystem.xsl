<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="lang"/>

  <xsl:include href="PanelSelectorCommon.xsl" />

  <xsl:template match="//Symbols/Device[Property[@Name='DriverID' and @Value='MyCompany.ExampleLCSystem']]">

    <xsl:element name="DevicePanels">
      <xsl:element name="Panel">
        <xsl:attribute name="Name">ExampleLCSystem.DevicePanel</xsl:attribute>
        <xsl:attribute name="Priority">1</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Main')"/>
          <xsl:with-param name="deviceName" select="@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Detector')" />
          <xsl:with-param name="deviceName" select="./Device[Property[@Name='Wavelength']]/@Name" />
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Pump')" />
          <xsl:with-param name="deviceName" select="./Device[Struct[@Name='Flow']]/@Name" />
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Sampler')" />
          <xsl:with-param name="deviceName" select="./Device[Command[@Name='Inject']]/@Name" />
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

    <xsl:element name="HomePanels">
      <xsl:element name="Panel">
        <xsl:attribute name="Name">ExampleLCSystem.HomePanel</xsl:attribute>
        <xsl:attribute name="Priority">1</xsl:attribute>
        <xsl:attribute name="Block">1</xsl:attribute>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Main')"/>
          <xsl:with-param name="deviceName" select="@Name"/>
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Detector')" />
          <xsl:with-param name="deviceName" select="./Device[Property[@Name='Wavelength']]/@Name" />
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Pump')" />
          <xsl:with-param name="deviceName" select="./Device[Struct[@Name='Flow']]/@Name" />
        </xsl:call-template>
        <xsl:call-template name="addDeviceMacro">
          <xsl:with-param name="macroName" select="string('Sampler')" />
          <xsl:with-param name="deviceName" select="./Device[Command[@Name='Inject']]/@Name" />
        </xsl:call-template>
      </xsl:element>
    </xsl:element>

  </xsl:template>

</xsl:stylesheet>
