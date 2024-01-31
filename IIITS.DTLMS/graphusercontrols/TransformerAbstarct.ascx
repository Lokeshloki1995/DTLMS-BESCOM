﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransformerAbstarct.ascx.cs" Inherits="IIITS.DTLMS.graphusercontrols.TransformerAbstarct" %>


<div class="row-fluid">

    <div class="span12">
        <!-- BEGIN SAMPLE FORMPORTLET-->
        
           
        <div id="TransformerAbstarctdiv" runat="server" class="span6">
              <div class="widget ">
            <div class="widget-title">
                <h4><i class="icon-reorder"></i>TC Guarantee Status</h4>
               
            </div>
             <div class="widget-body">

                <ul class="breadcrumb" style="font-weight: bold">
                    <li>

                        <a id="viewstatus" onclick="view_link_click" runat="server">Current Status 
                              <span style="color: Blue">(View Details..)</span></a>
                    </li>
                </ul>
         

            <div id="divdropdwn" class="control-group" visible="false" runat="server">
                <label class="control-label">Select Division</label>
                <div class="controls">
                    <div class="input-append">
                        <asp:DropDownList ID="cmbDivisions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cmbDivisions_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <asp:Chart ID="TransformerAbstarctchart"  CanResize="true"  CssClass="table  table-bordered table-condensed table-responsive" runat="server"  Height="300" Width="600 ">
                <Titles>
                    <asp:Title  Name="Items" />
                </Titles>
                <Series>
                </Series>
                <Legends>
                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="true" Name="Legend1"
                        LegendStyle="Row" AutoFitMinFontSize="30"/>
                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="true" Name="Legend2"
                        LegendStyle="Row" AutoFitMinFontSize="30"/>
                </Legends>
                 <Titles>
                                        <asp:Title Name="NewTitle">
                                        </asp:Title>
                                    </Titles>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" >
                   
                        </asp:ChartArea>
                </ChartAreas>
            </asp:Chart>
                    </div>
                </div>
            </div>
              <div id="TransformerAbstarctchartad" CanResize="true" CssClass="table  table-bordered table-condensed table-responsive" runat="server" class="span6">

                      <div class="widget ">
            <div class="widget-title">
                <h4><i class="icon-reorder"></i>TC Guarantee Status</h4>
               
            </div>
                   <div class="widget-body">

                <ul class="breadcrumb" style="font-weight: bold">
                    <li>

                        <a id="viewstatus1" onclick="view_link_click" runat="server">Current Status 
                              <span style="color: Blue">(View Details..)</span></a>
                    </li>
                </ul>
           

            <div id="divdropdwn1" class="control-group" visible="false" runat="server">
                <label class="control-label">Select Division</label>
                <div class="controls">
                    <div class="input-append">
                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cmbDivisions_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <asp:Chart ID="TransformerAbstarctchartadmin"  CanResize="true"  runat="server" Height="300" Width="600">
                <Titles>
                    <asp:Title  Name="Items" />
                </Titles>
                <Series>
                </Series>
                <Legends>
                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="true" Name="Legend1"
                        LegendStyle="Row"  AutoFitMinFontSize="30" />
                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="true" Name="Legend2"
                        LegendStyle="Row"  AutoFitMinFontSize="30"/>
                </Legends>
                  <Titles>
                                        <asp:Title Name="NewTitle">
                                        </asp:Title>
                                    </Titles>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />
                </ChartAreas>
            </asp:Chart>
                        </div>
                  </div>


            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>


        </div>
    </div>
</div>