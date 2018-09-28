<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

	<xsl:template name="addDeviceMacro">
		<xsl:param name="macroName"/>
		<xsl:param name="deviceName"/>
		<xsl:element name="Macro">
			<xsl:attribute name="Name">
				<xsl:value-of select="$macroName"/>
			</xsl:attribute>
			<xsl:attribute name="DeviceName">
				<xsl:value-of select="$deviceName"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

    <xsl:template name="checkVersion">
        <xsl:param name="version"/>
        <xsl:param name="minVersion"/>

        <xsl:variable name="first" select="substring-before($version, '.')"/>
        <xsl:variable name="minFirst" select="substring-before($minVersion, '.')"/>
        <xsl:variable name="nFirst">
            <xsl:choose>
                <xsl:when test="string-length($first) &gt; 0">
                    <xsl:value-of select="number($first)"/>
                </xsl:when>
                <xsl:when test="string-length($version) &gt; 0">
                    <xsl:value-of select="number($version)"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:number value="0"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="nMinFirst">
            <xsl:choose>
                <xsl:when test="string-length($minFirst) &gt; 0">
                    <xsl:value-of select="number($minFirst)"/>
                </xsl:when>
                <xsl:when test="string-length($minVersion) &gt; 0">
                    <xsl:value-of select="number($minVersion)"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:number value="0"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="result">
            <xsl:choose>
                <xsl:when test="$nFirst &gt; $nMinFirst">1</xsl:when>
                <xsl:when test="$nFirst &lt; $nMinFirst">0</xsl:when>
                <xsl:otherwise>
                    <xsl:variable name="second" select="substring-after($version, '.')"/>
                    <xsl:variable name="minSecond" select="substring-after($minVersion, '.')"/>
                    <xsl:variable name="continue" select="string-length($second) + string-length($minSecond) &gt; 0"/>
                    <xsl:variable name="next">
                        <xsl:choose>
                            <xsl:when test="$continue">
                                <xsl:call-template name="checkVersion">
                                    <xsl:with-param name="version" select="$second"/>
                                    <xsl:with-param name="minVersion" select="$minSecond"/>
                                </xsl:call-template>
                            </xsl:when>
                            <xsl:otherwise>1</xsl:otherwise>
                        </xsl:choose>
                    </xsl:variable>
                    <xsl:value-of select="$next"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>

        <xsl:value-of select="$result"/>
    </xsl:template>

</xsl:stylesheet>
