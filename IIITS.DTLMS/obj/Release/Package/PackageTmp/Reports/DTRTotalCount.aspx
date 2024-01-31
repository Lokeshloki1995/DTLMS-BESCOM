<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="DTRTotalCount.aspx.cs" Inherits="IIITS.DTLMS.Reports.DTRTotalCount" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <br />
   <br />
    <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN SAMPLE FORMPORTLET-->

                <div class="widget blue">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-reorder"></i>DISTRIBUTION TRANSFORMERS </h4>
                        <a href="#" data-toggle="modal" data-target="#myModal" title="Click For Help"><i class="fa fa-exclamation-circle" style="font-size: 30px; color: white"></i></a>
                       
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <!-- BEGIN FORM-->
                            <div class="form-horizontal">
                                <div class="row-fluid">
                                    <div class="span1">
                                    </div>
                                    <div class="span5">
                                  
                                          <div class="control-group">
                                                <label class="control-label">
                                                    Zone </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbZone" runat="server" AutoPostBack="true" TabIndex="1"
                                                            OnSelectedIndexChanged="cmbZone_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>


                                 <div class="control-group">
                                    <label class="control-label" >
                                    Division</label>
                                    <div class="controls">
                                       <div class="input-append">
                                          <asp:DropDownList ID="cmbDiv" runat="server" AutoPostBack="true" TabIndex="4"
                                              OnSelectedIndexChanged="cmbLocation_SelectedIndexChanged">
                                          </asp:DropDownList>
                                       </div>
                                    </div>
                                 </div>
                                
                               
                                         </div>
                           
                                  <%-- another span--%>
                                         <div class="control-group">
                                                <label class="control-label">
                                                    Circle </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbCircle" runat="server" AutoPostBack="true" TabIndex="1"
                                                            OnSelectedIndexChanged="cmbCircle_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>



                                    </div>
                                     </div>
                            <div class="span5">
                            </div>
                            <div class="row-fluid">
                                <div class="span12">
                                    <div class="text-center" align="center">


                                        <asp:Button ID="Export" runat="server" Text="Generate Report For Field" CssClass="btn btn-primary"
                                            TabIndex="12" OnClick="Export_Click" />
                                         <asp:Button ID="Export1" runat="server" Text="Generate Report For Store" CssClass="btn btn-primary"
                                            TabIndex="12" OnClick="Export_Click1" />
                                         <asp:Button ID="Export2" runat="server" Text="Generate Report For Repairer" CssClass="btn btn-primary"
                                            TabIndex="12" OnClick="Export_Click2" />
                                        <asp:Button ID="Export3" runat="server" Text="Generate Report For Transformer Bank" CssClass="btn btn-primary"
                                            TabIndex="12" OnClick="Export_Click3" /><br />
                                        <br />

                                        <asp:Label ID="Label2" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <!-- END FORM-->
                        </div>
                    </div>
                </div>
                <!-- END SAMPLE FORM PORTLET-->
            </div>
        </div>
</asp:Content>
