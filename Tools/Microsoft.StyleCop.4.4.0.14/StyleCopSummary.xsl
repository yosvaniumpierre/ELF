<?xml version="1.0"?>
<!-- Designed by Yves Tremblay of ProgiNet Inc. and SBG International Inc. -->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:template match="/">

    <xsl:variable name="report.root" select="cruisecontrol//StyleCopViolations" />
    <!-- Get unique Source files -->
    <xsl:variable name="unique.source" select="$report.root/Violation[not(@Source = preceding-sibling::Violation/@Source)]" />

    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td class="sectionheader" colspan="2">
          StyleCop Violation Count: <xsl:value-of select="count($report.root/Violation)"/>
        </td>
      </tr>
      <tr>
        <td colspan="2"> </td>
      </tr>
      <xsl:if test="count($report.root/Violation) > 0" >
        <xsl:apply-templates select="$unique.source" />
      </xsl:if>
    </table>

  </xsl:template>

  <xsl:template match="Violation">
    <xsl:variable name="source" select="./@Source"/>
    <xsl:variable name="violations" select="count(//Violation[@Source=$source])"/>
    <tr>
      <td colspan="2">
        <xsl:value-of select="concat ($source, ': ', $violations)" />
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>