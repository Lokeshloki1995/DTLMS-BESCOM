<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/DTLMS.Master" CodeBehind="RepairerBulkWorkorder.aspx.cs" Inherits="IIITS.DTLMS.TCRepair.RepairerBulkWorkorder" %>

<%@ Register Src="/ApprovalHistoryView.ascx" TagName="ApprovalHistoryView" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>
    <script type="text/javascript">

        function preventMultipleSubmissions() {
            $('#<%=cmdSave.ClientID %>').prop('disabled', true);
        }

        window.onbeforeunload = preventMultipleSubmissions;

    </script>
    <script type="text/javascript">
        function MutExChkList(chk) {
            var chkList = chk.parentNode.parentNode.parentNode;
            var chks = chkList.getElementsByTagName("input");
            for (var i = 0; i < chks.length; i++) {
                if (chks[i] != chk && chk.checked) {
                    chks[i].checked = false;
                }
            }
        }
    </script>
    <style>
        .MyClass label {
            float: right;
            margin: -5px 0px 0px 8px;
        }

        .MyClass input {
            margin-left: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ajax:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </ajax:ToolkitScriptManager>

    <div class="container-fluid">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <div class="span8">
                <!-- BEGIN THEME CUSTOMIZER-->

                <!-- END THEME CUSTOMIZER-->
                <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                <h3 class="page-title">Faulty Bulk Work Order</h3>
                <ul class="breadcrumb" style="display: none">

                    <li class="pull-right search-wrap">
                        <form action="" class="hidden-phone">
                            <div class="input-append search-input-area">
                                <input class="" id="appendedInputButton" type="text">
                                <button class="btn" type="button"><i class="icon-search"></i></button>
                            </div>
                        </form>
                    </li>
                </ul>
                <!-- END PAGE TITLE & BREADCRUMB-->
            </div>
            <div style="float: right; margin-top: 20px; margin-right: 12px">
                <asp:Button ID="cmdClose" runat="server" Text="Close"
                    CssClass="btn btn-primary" OnClick="cmdClose_Click" />
            </div>
        </div>


        <!-- END PAGE HEADER-->

        <!-- BEGIN PAGE CONTENT-->
        <div class="row-fluid" runat="server" id="dvBasic">
            <div class="span12">
                <!-- BEGIN SAMPLE FORMPORTLET-->
                <div class="widget blue">
                    <div class="widget-title">
                        <h4><i class="icon-reorder"></i>Basic Details</h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>

                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <!-- BEGIN FORM-->
                            <div class="form-horizontal">

                                <div class="row-fluid">
                                    <div class="span1"></div>
                                    <div class="span5">
                                        <div class="control-group">
                                            <label class="control-label">Issued By<span class="Mandotary"> *</span></label>

                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:DropDownList ID="cmbIssuedBy" runat="server" TabIndex="3" Enabled="false">
                                                    </asp:DropDownList>
                                                    <asp:TextBox ID="txtWOId" runat="server" Visible="false" Width="20px"
                                                        MaxLength="100"></asp:TextBox>
                                                    <asp:TextBox ID="txtDTCId" runat="server" Visible="false" Width="20px"
                                                        MaxLength="100"></asp:TextBox>
                                                    <asp:TextBox ID="txtTCId" runat="server" Visible="false" Width="20px"
                                                        MaxLength="100"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="control-group">
                                            <label class="control-label">Bulk Work Order No<span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtBulkWoNo1" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharNotanySpecial(event)"></asp:TextBox>
                                                    <asp:TextBox ID="txtBulkWoNo2" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharHyphen(event)"></asp:TextBox>
                                                    <asp:TextBox ID="txtBulkWoNo3" runat="server" MaxLength="6" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return isAlphaNumeric(event)"></asp:TextBox>

                                                </div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Date <span class="Mandotary">*</span> </label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtActiontype" runat="server" MaxLength="10" Visible="false" Width="20px"></asp:TextBox>
                                                    <asp:TextBox ID="txtRepdate" runat="server" MaxLength="10" TabIndex="6" CssClass="auto-style1"></asp:TextBox>
                                                    <ajax:CalendarExtender ID="CalendarExtender_txtRepdate" runat="server" CssClass="cal_Theme1"
                                                        Format="dd/MM/yyyy" TargetControlID="txtRepdate">
                                                    </ajax:CalendarExtender>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">WO A/C Code<span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtAcCode" runat="server" onkeypress="javascript:return OnlyNumber(event);"
                                                        MaxLength="15" TabIndex="8" ReadOnly="true"></asp:TextBox>
                                                </div>

                                            </div>
                                            <div class="control-group">


                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:HiddenField ID="hdfApproveStatus" runat="server" />
                                                        <asp:HiddenField ID="hdfWFOId" runat="server" />
                                                        <asp:HiddenField ID="hdfRejectApproveRef" runat="server" />
                                                        <asp:HiddenField ID="hdfWFDataId" runat="server" />
                                                        <asp:HiddenField ID="hdfSubdivName" runat="server" />
                                                        <asp:HiddenField ID="hdfdivName" runat="server" />
                                                        <asp:HiddenField ID="hdfWFOAutoId" runat="server" />
                                                        <asp:HiddenField ID="hdfAppDesc" runat="server" />
                                                        <asp:HiddenField ID="hdfGuarenteeType" runat="server" />
                                                        <asp:HiddenField ID="hdfboid" runat="server" />
                                                        <asp:HiddenField ID="hdfCrBy" runat="server" />
                                                        <asp:TextBox ID="txtType" runat="server" Width="20px" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtFailType" runat="server" Width="20px" Visible="false"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="span5">

                                        <div class="control-group">
                                            <label class="control-label">Declared By</label>

                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtDeclaredBy" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                                <br />
                                                <asp:LinkButton ID="lnkBudgetstat" runat="server"
                                                    Style="font-size: 12px; color: Blue; cursor: pointer" OnClick="lnkBudgetstat_Click">View Budget Status</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Sale of scrap A/C Code<span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtSSAcCode" runat="server" onkeypress="javascript:return OnlyNumber(event);"
                                                        MaxLength="15" TabIndex="8" ReadOnly="true"></asp:TextBox>
                                                </div>

                                            </div>
                                        </div>
                                        <div class="span10">
                                            <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Text="" ForeColor="Red"></asp:Label>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>

                        <div class="space20"></div>

                        <!-- END FORM-->
                    </div>
                </div>
                <!-- END SAMPLE FORM PORTLET-->
            </div>

            <!-- END PAGE CONTENT-->
            <div class="row-fluid" runat="server" id="dvWorkOrder">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Bulk Work Order Details</h4>
                            <span class="tools">
                                <a href="javascript:;" class="icon-chevron-down"></a>
                            </span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="row-fluid">
                                        <div>
                                            <asp:GridView ID="grdBulkWo" AutoGenerateColumns="false" PageSize="10" AllowPaging="false"
                                                ShowFooter="true" EmptyDataText="No Records Found" CssClass="table table-striped table-bordered table-advance table-hover"
                                                runat="server" ShowHeaderWhenEmpty="True" OnRowCommand="grdBulkWo_RowCommand"
                                                AllowSorting="true" OnRowDataBound="grdBulkWo_RowDataBound">
                                                <HeaderStyle CssClass="both" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select Data" ItemStyle-Width="20px">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:CheckBox ID="chkmaterial" runat="server" />
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkmaterial" runat="server" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="RESTD_ID" HeaderText="Est Id" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEstId" runat="server" Text='<%# Bind("RESTD_ID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbltccode" runat="server" Text='<%# Bind("RESTD_TC_CODE") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbltccap" runat="server" Text='<%# Bind("RWO_DTC_CAP") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="RESTD_NO" HeaderText="EST No" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEstNo" runat="server" Text='<%# Bind("RESTD_NO") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="RESTD_ITEM_TOTAL" HeaderText="EST Amount">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEstAmt" runat="server" Text='<%# Bind("RESTD_ITEM_TOTAL") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="WO No">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtEWoNo1" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharNotanySpecial(event)"></asp:TextBox>
                                                                <asp:TextBox ID="txtEWoNo2" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharHyphen(event)"></asp:TextBox>
                                                                <asp:TextBox ID="txtEWoNo3" runat="server" MaxLength="10" TabIndex="5" Width="75px" AutoComplete="off" onkeypress="javascript:return isAlphaNumeric(event)"></asp:TextBox>
                                                                <%--   <asp:TextBox ID="txtMQuantity" runat="server" Width="100px" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>--%>
                                                            </center>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtWoNo1" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharNotanySpecial(event)"></asp:TextBox>
                                                            <asp:TextBox ID="txtWoNo2" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharHyphen(event)"></asp:TextBox>
                                                            <asp:TextBox ID="txtWoNo3" runat="server" MaxLength="10" TabIndex="5" Width="75px" AutoComplete="off" onkeypress="javascript:return isAlphaNumeric(event)"></asp:TextBox>
                                                            </center>                                
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="WO No" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblWoNumber" runat="server" Text='<%# Bind("RWO_NO") %>'
                                                                Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="WO Amount">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtEWoAmount" runat="server" Width="100px" Text='<%# Bind("RWO_AMT") %>' onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtWoAmount" runat="server" Width="100px" MaxLength="7" Text='<%# Bind("RWO_AMT") %>' onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                            <%-- <asp:RequiredFieldValidator ID="rfWoAmt" ControlToValidate="txtWoAmount" runat="server"
                                                                  ErrorMessage="Enter WorkOrder Amount" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="WO Amount" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblWoAmount" runat="server" Text='<%# Bind("RWO_AMT") %>'
                                                                Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Inncured Cost">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtEIncAmt" runat="server" Width="100px" Text='<%# Bind("RWO_INNC_COST") %>' onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtIncAmt" runat="server" Width="100px" MaxLength="7" Text='<%# Bind("RWO_INNC_COST") %>' onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Inncured Cost" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIncCost" runat="server" Text='<%# Bind("RWO_INNC_COST") %>'
                                                                Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SS WO No">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtESSWoNo1" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharNotanySpecial(event)"></asp:TextBox>
                                                                <asp:TextBox ID="txtESSWoNo2" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharHyphen(event)"></asp:TextBox>
                                                                <asp:TextBox ID="txtESSWoNo3" runat="server" MaxLength="10" TabIndex="5" Width="75px" AutoComplete="off" onkeypress="javascript:return isAlphaNumeric(event)"></asp:TextBox>
                                                                <%--   <asp:TextBox ID="txtMQuantity" runat="server" Width="100px" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>--%>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtSSWoNo1" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharNotanySpecial(event)"></asp:TextBox>
                                                            <asp:TextBox ID="txtSSWoNo2" runat="server" MaxLength="5" TabIndex="5" Width="45px" AutoComplete="off" onkeypress="javascript:return AllowOnlyCharHyphen(event)"></asp:TextBox>
                                                            <asp:TextBox ID="txtSSWoNo3" runat="server" MaxLength="10" TabIndex="5" Width="75px" AutoComplete="off" onkeypress="javascript:return isAlphaNumeric(event)"></asp:TextBox>
                                                            <%--<asp:TextBox ID="txtMqty" runat="server" Width="100px" MaxLength="5" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>--%>
                                                        </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SS WO No" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSSWoNo" runat="server" Text='<%# Bind("RWO_SS_WO_NO") %>'
                                                                Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SS Amount">
                                                        <EditItemTemplate>
                                                            <center>
                                                                <asp:TextBox ID="txtESSAmt" runat="server" Width="100px" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtSSAmt" runat="server" Width="100px" MaxLength="7" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SS Amount" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSSAmount" runat="server" Text='<%# Bind("RWO_SS_AMT") %>'
                                                                Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="View">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:LinkButton runat="server" CommandName="View" ID="lnkView" ToolTip="View Estimation">
                                                             <img src="../img/Manual/view.png" style="width:20px" /></asp:LinkButton>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <uc1:ApprovalHistoryView ID="ApprovalHistoryView" runat="server" />

            <div class="row-fluid" runat="server" id="dvComments" style="display: none">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Comments for Approve/Reject</h4>
                            <span class="tools">
                                <a href="javascript:;" class="icon-chevron-down"></a>
                                <a href="javascript:;" class="icon-remove"></a>
                            </span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="row-fluid">
                                        <div class="span1"></div>
                                        <div class="span5">

                                            <div class="control-group">
                                                <label class="control-label">Comments<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtComment" runat="server" MaxLength="200" TabIndex="4" TextMode="MultiLine"
                                                            Width="550px" Height="125px" Style="resize: none" onkeyup="javascript:ValidateTextlimit(this,200)"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>


                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="text-center" align="center">



                <asp:Button ID="cmdSave" runat="server" Text="Save"
                    CssClass="btn btn-primary" onchange="javascript:preventMultipleSubmissions();"
                    OnClick="cmdSave_Click" TabIndex="13" />

                <%-- <asp:Button ID="cmdViewEstimate" runat="server" Text="View Estimate"
                    CssClass="btn btn-primary"
                    OnClick="cmdViewEstimate_Click" Visible="false" TabIndex="13" />--%>

                <asp:Button ID="cmdReset" runat="server" Text="Reset" Visible="true"
                    CssClass="btn btn-primary" OnClick="cmdReset_Click" TabIndex="14" /><br />
            </div>

        </div>
        <!-- END PAGE CONTENT-->
</asp:Content>
