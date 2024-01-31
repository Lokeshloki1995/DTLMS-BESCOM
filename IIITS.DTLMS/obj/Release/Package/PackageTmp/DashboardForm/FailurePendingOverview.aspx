﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FailurePendingOverview.aspx.cs" Inherits="IIITS.DTLMS.DashboardForm.FailurePendingOverview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script src="../Scripts/functions.js" type="text/javascript"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta content="" name="description" />
    <meta content="Mosaddek" name="author" />
    <link href="/assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="/assets/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="/assets/bootstrap/css/bootstrap-fileupload.css" rel="stylesheet" />
    <link href="/assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="/css/style.css" rel="stylesheet" />
    <link href="/css/style-responsive.css" rel="stylesheet" />
    <link href="/css/style-default.css" rel="stylesheet" id="style_color" />
    <link href="/assets/fullcalendar/fullcalendar/bootstrap-fullcalendar.css" rel="stylesheet" />
    <link href="/assets/jquery-easy-pie-chart/jquery.easy-pie-chart.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="/Styles/calendar.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/functions.js"></script>

    <link href="assets/jquery-easy-pie-chart/jquery.easy-pie-chart.css" rel="stylesheet" type="text/css" media="screen" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-1.12.4.js"></script>

    <style type="text/css">
        .ascending th a {
            background: url(/img/sort_asc.png) no-repeat;
            display: block;
            padding: 0px 4px 0 20px;
        }

        .descending th a {
            background: url(/img/sort_desc.png) no-repeat;
            display: block;
            padding: 0 4px 0 20px;
        }

        .both th a {
            background: url(/img/sort_both.png) no-repeat;
            display: block;
            padding: 0 4px 0 20px;
        }
    </style>

    <script language="Javascript" type="text/javascript">


        function onlyAlphabets(e, t) {
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if ( // space

                !(code > 44 && code < 60) &&
                !(code > 38 && code < 42) &&
                 !(code == 47) &&
                !(code == 95) &&
              !(code > 64 && code < 94) && // upper alpha (A-Z)
              !(code > 96 && code < 126)) { // lower alpha (a-z)
                e.preventDefault();
            }
        }
        function onlyNums(e, t) {
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if (!(code > 47 && code < 58) ){
                e.preventDefault();
            }
        }

        function onlyAlphabetsnum(e, t) {
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if (!(code > 47 && code < 58) &&
              !(code > 64 && code < 91) && // upper alpha (A-Z)
              !(code > 96 && code < 123)) { // lower alpha (a-z)
                e.preventDefault();
            }
        }
        function onlyAlphabetsnew(e, t) {
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if (
                //!(code > 47 && code < 58) &&
              !(code > 64 && code < 91) && // upper alpha (A-Z)
              !(code > 96 && code < 123)) { // lower alpha (a-z)
                e.preventDefault();
            }
        }
    </script>


    <script type="text/javascript">
        jQuery(document).ready(function () {
            //CERTIFICATION CONTROLS
            $('input').on('input', function () {
                $(this).val($(this).val().replace(/[^a-z0-9-@#*:^\@/+.c ]/gi, ''));
            });

            $('textarea').on('input', function () {
                $(this).val($(this).val().replace(/[^a-z0-9-!@#_+=*:&%.()@/ ]/gi, ''));
            });
            $('textbox').on('input', function () {
                $(this).val($(this).val().replace(/[^a-z0-9-!@#_+=*:&%.()@/ ]/gi, ''));
            });
            const d = new Date();
            let year = d.getFullYear();
            document.getElementById("demo").innerHTML = year;
        });
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <div class="container-fluid">
                    <!-- BEGIN PAGE HEADER-->
                    <div class="row-fluid">
                        <div class="span12">
                            <!-- BEGIN THEME CUSTOMIZER-->
                            <!-- END THEME CUSTOMIZER-->
                            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                            <h3 class="page-title">
                                <asp:Label ID="failure" runat="server" Text="Failure Pending Details" ForeColor="White"></asp:Label>
                            </h3>
                            <div style="float: right">

                                <div class="span1">
                                </div>

                            </div>
                            <%--<ul class="breadcrumb" style="display: none;">
                        <li class="pull-right search-wrap">
                            <form action="" class="hidden-phone">
                            <div class="input-append search-input-area">
                                <button class="btn" type="button">
                                    <i class="icon-search"></i>
                                </button>
                            </div>
                            </form>
                        </li>
                    </ul>--%>
                            <!-- END PAGE TITLE & BREADCRUMB-->
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span12">
                            <!-- BEGIN SAMPLE FORMPORTLET-->
                            <div class="widget blue">
                                <div class="widget-title">
                                    <h4>
                                        <i class="icon-reorder"></i>
                                        <asp:Label ID="failureText" runat="server" Text="Failure Pending Details"></asp:Label></h4>
                                    <%--                            <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a><a href="javascript:;"
                                class="icon-remove"></a></span>--%>
                                </div>
                                <div class="widget-body">
                                    <div style="float: right">
                                    </div>
                                    <asp:Button ID="cmdexport" runat="server" Text="Export Excel" CssClass="btn btn-primary"
                                        OnClick="Export_clickFailurePendingOverview" /><br />

                                    <!-- END FORM-->
                                    <div style="float: right">
                                        <asp:HiddenField ID="hdfOffCode" runat="server" />
                                    </div>
                                    <div class="space20">
                                    </div>
                                    <div style="overflow: auto;">
                                        <asp:GridView ID="grdFailurePending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" OnPageIndexChanging="grdFailurePending_PageIndexChanging"
                                            OnRowCommand="grdFailurePending_RowCommand" Visible="false" OnSorting="grdFailurePending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code" Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"
                                                                MaxLength="20"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtsection" runat="server" placeholder="Enter section name" Width="150px" AutoComplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>




                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OM_CODE" HeaderText="OM Code" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmCode" runat="server" Text='<%# Bind("OM_CODE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>






                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="DF_ID" HeaderText="Failure ID" SortExpression="DF_ID">
                                                    <ItemTemplate>
                                                        </FooterTemplate>
                                                <asp:Label ID="lblFailureNo" runat="server" Text='<%# Bind("DF_ID") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel4" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtfailid" runat="server" onkeypress="javascript:return OnlyNumber(event);" placeholder="Enter failure id" Width="150px" AutoComplete="off"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DF_DATE" HeaderText="Failure Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailureDate" runat="server" Text='<%# Bind("DF_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="FL_STATUS" HeaderText="Failure status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailurestat" runat="server" Text='<%# Bind("FL_STATUS") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdFailureApprovalPending" AutoGenerateColumns="false" PageSize="10"
                                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                            ShowFooter="true" Visible="false"
                                            CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                            runat="server"
                                            OnPageIndexChanging="grdFailureApprovalPending_PageIndexChanging" OnSorting="grdFailureApprovalPending_Sorting" 
                                            AllowSorting="true" OnRowCommand="grdFailureApprovalPending_RowCommand">
                                            <HeaderStyle CssClass="both" />
                                            <%--<HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" Autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " AutoComplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" Width="150px"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPendingFailStatus" runat="server" Text='<%# Bind("FL_STATUS") %>'
                                                            Style="word-break: break-all;" Width="250px" ForeColor="#77808a">
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdEstimationPending" AutoGenerateColumns="false" PageSize="10"
                                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                            ShowFooter="true" Visible="false"
                                            CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                            runat="server" OnRowCommand="grdEstimationPending_RowCommand"
                                            OnPageIndexChanging="grdEstimationPending_PageIndexChanging" OnSorting="grdEstimationPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%--<HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" Width="150px"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                                <%--                                        <asp:TemplateField AccessibleHeaderText="EST_NO" HeaderText="Estimation No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblEstimationNo" runat="server" Text='<%# Bind("EST_NO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DF_DATE" HeaderText="Failure Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailureDate" runat="server" Text='<%# Bind("DF_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--                                                <asp:TemplateField AccessibleHeaderText="FL_STATUS" HeaderText="Status" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailStatus" runat="server" Text='<%# Bind("EST_STATUS") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEstimationStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>'
                                                            Style="word-break: break-all;" Width="250px" ForeColor="#77808a"> </asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:GridView ID="grdWorkorderPending" AutoGenerateColumns="false" PageSize="10"
                                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                            ShowFooter="true" Visible="false"
                                            CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                            runat="server" OnRowCommand="grdWorkorderPending_RowCommand"
                                            OnPageIndexChanging="grdWorkorderPending_PageIndexChanging" OnSorting="grdWorkorderPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                               
                                                <asp:TemplateField AccessibleHeaderText="DF_DATE" HeaderText="Failure Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailureDate" runat="server" Text='<%# Bind("DF_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                             
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWOpendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a">

                                                        </asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdReceiveTC" AutoGenerateColumns="false" PageSize="10"
                                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                            ShowFooter="true" Visible="false"
                                            CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                            runat="server" OnRowCommand="grdReceiveTC_RowCommand"
                                            OnPageIndexChanging="grdReceiveTC_PageIndexChanging" OnSorting="grdReceiveTC_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="WO_NO" HeaderText="Work Order NO" SortExpression="WO_NO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWONO" runat="server" Text='<%# Bind("WO_NO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="WO_DATE" HeaderText="WO Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWODATE" runat="server" Text='<%# Bind("WO_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField AccessibleHeaderText="RECEIVE_STATUS" HeaderText="Status" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReceiveTCStatus" runat="server" Text='<%# Bind("RECEIVE_STATUS") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReceiveTCPendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdSingleComission" AutoGenerateColumns="false" PageSize="10"
                                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                            ShowFooter="true" Visible="false"
                                            CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                            runat="server" OnRowCommand="grdSingleComission_RowCommand"
                                            OnPageIndexChanging="grdSingleComission_PageIndexChanging" OnSorting="grdSingleComission_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="WO_NO" HeaderText="Work Order NO" SortExpression="WO_NO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWONO" runat="server" Text='<%# Bind("WO_NO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="WO_DATE" HeaderText="WO Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWODATE" runat="server" Text='<%# Bind("WO_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="COMMISSION_STATUS" HeaderText="Status" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblComissionStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblComissionPendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdIndentPending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false"
                                            OnRowCommand="grdIndentPending_RowCommand"
                                            OnPageIndexChanging="grdIndentPending_PageIndexChanging" OnSorting="grdIndentPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                                <%--                                        <asp:TemplateField AccessibleHeaderText="TI_INDENT_NO" HeaderText="Indent No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIndentNo" runat="server" Text='<%# Bind("TI_INDENT_NO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DF_DATE" HeaderText="Indent Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailureDate" runat="server" Text='<%# Bind("DF_DATE") %>'
                                                            Style="word-break: break-all;" Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="INDT_STATUS" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIndentStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>'
                                                            Style="word-break: break-all;" Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:GridView ID="grdinvoicePending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false"
                                            OnRowCommand="grdinvoicePending_RowCommand"
                                            OnPageIndexChanging="grdinvoicePending_PageIndexChanging" OnSorting="grdinvoicePending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%--  <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                                <%--                                        <asp:TemplateField AccessibleHeaderText="IN_INV_NO" HeaderText="Invoice No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Bind("IN_INV_NO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DF_DATE" HeaderText="Commission Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFailureDATE" runat="server" Text='<%# Bind("DF_DATE") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdDecommissionPending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false"
                                            OnRowCommand="grdDecommissionPending_RowCommand"
                                            OnPageIndexChanging="grdDecommissionPending_PageIndexChanging" OnSorting="grdDecommissionPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DIVSION" HeaderText="Division Name" SortExpression="DIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDivName" runat="server" Text='<%# Bind("DIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                                <%--                                                <asp:TemplateField AccessibleHeaderText="DECOMM_STATUS" HeaderText="Status" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDecomStatus" runat="server" Text='<%# Bind("DECOMM_STATUS") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDecomPendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdRIPending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false"
                                            OnRowCommand="grdRIPending_RowCommand"
                                            OnPageIndexChanging="grdRIPending_PageIndexChanging" OnSorting="grdRIPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField AccessibleHeaderText="TR_RI_NO" HeaderText="RI No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblriNo" runat="server" Text='<%# Bind("TR_RI_NO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <%--<asp:TemplateField AccessibleHeaderText="TR_RI_DATE" HeaderText="RI Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRiDATE" runat="server" Text='<%# Bind("TR_RI_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <%--                                                <asp:TemplateField AccessibleHeaderText="RI_STATUS" HeaderText="RI STATUS" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRIStatus" runat="server" Text='<%# Bind("RI_STATUS") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="RI STATUS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRIPendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <%--                                                <asp:TemplateField AccessibleHeaderText="CR_STATUS" HeaderText="CR STATUS" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCRStatus" runat="server" Text='<%# Bind("CR_STATUS") %>' Style="word-break: break-all;" Width="200px" ForeColor="#ab8465"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <%--<asp:TemplateField AccessibleHeaderText="CR_DAYS_FROM_PENDING" HeaderText="CR STATUS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCRPendingStatus" runat="server" Text='<%# Bind("CR_DAYS_FROM_PENDING") %>' Style="word-break: break-all;" Width="250px" ForeColor="#ab8465"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                            </Columns>
                                        </asp:GridView>



                                        <asp:GridView ID="grdCRPending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false"
                                            OnRowCommand="grdCRPending_RowCommand"
                                            OnPageIndexChanging="grdCRPending_PageIndexChanging" OnSorting="grdCRPending_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DTR_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="DTR_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDTrCode" runat="server" Text='<%# Bind("DF_EQUIPMENT_ID") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTRCode" runat="server" placeholder="Enter DTR CODE" Width="150px" autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="6"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transformer Centre Name" Visible="true" SortExpression="DT_NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDTCName" runat="server" placeholder="Enter DTC Name " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SUBDIVSION" HeaderText="Sub-Division Name" SortExpression="SUBDIVSION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubDivName" runat="server" Text='<%# Bind("SUBDIVSION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Section Name" Visible="true" SortExpression="OMSECTION">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField AccessibleHeaderText="TR_RI_NO" HeaderText="RI No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblriNo" runat="server" Text='<%# Bind("TR_RI_NO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <%--<asp:TemplateField AccessibleHeaderText="TR_RI_DATE" HeaderText="RI Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRiDATE" runat="server" Text='<%# Bind("TR_RI_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                                <%--                                                <asp:TemplateField AccessibleHeaderText="RI_STATUS" HeaderText="RI STATUS" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRIStatus" runat="server" Text='<%# Bind("RI_STATUS") %>' Style="word-break: break-all;"
                                                            Width="200px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <%-- <asp:TemplateField AccessibleHeaderText="DAYS_FROM_PENDING" HeaderText="RI STATUS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRIPendingStatus" runat="server" Text='<%# Bind("DAYS_FROM_PENDING") %>' Style="word-break: break-all;"
                                                            Width="250px" ForeColor="#77808a"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>


                                                <%--                                                <asp:TemplateField AccessibleHeaderText="CR_STATUS" HeaderText="CR STATUS" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCRStatus" runat="server" Text='<%# Bind("CR_STATUS") %>' Style="word-break: break-all;" Width="200px" ForeColor="#ab8465"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField AccessibleHeaderText="CR_DAYS_FROM_PENDING" HeaderText="CR STATUS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCRPendingStatus" runat="server" Text='<%# Bind("CR_DAYS_FROM_PENDING") %>' Style="word-break: break-all;" Width="250px" ForeColor="#ab8465"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdInvoiceTCDetails" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" Visible="false" AllowSorting="true" OnPageIndexChanging="grdInvoiceTCDetails_PageIndexChanging" OnRowCommand="grdInvoiceTCDetails_RowCommand">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltccode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="TxtTcCode" runat="server" placeholder="Enter DTR Code " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTR SERIAL NO" Visible="true">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSLNO" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtTCSLNO" runat="server" placeholder="Enter DTR SLNO " Width="150px" autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="CAPACITY" Visible="true">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="STATUS" HeaderText="STATUS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("STATUS") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <%-- getting store dashboard conditions of tc's details in grid--%>
                                        <asp:GridView ID="grdConditionOfTC" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" OnPageIndexChanging="grdConditionOfTC_PageIndexChanging"
                                            OnRowCommand="grdConditionOfTC_RowCommand" Visible="false" OnSorting="grdConditionOfTC_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtcCode" runat="server" placeholder="Enter DTR Code " Width="150px" onkeypress="javascript: return onlyNums(event,this);" autocomplete="off"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTR SLNO" Visible="true" SortExpression="TC_SLNO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                            Width="150px" MaxLength="20"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtslno" runat="server" placeholder="Enter DTR SLNo " Width="150px" onkeypress="javascript: return onlyAlphabets(event,this);" autocomplete="off" ></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="Make">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TM_NAME") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_RATING" HeaderText="Rating">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRating" runat="server" Text='<%# Bind("TC_RATING") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>

                                        <%-- getting store dashboard capacity wise of tc's details in grid--%>
                                        <asp:GridView ID="grdTCCapacityWise" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" OnPageIndexChanging="grdTCCapacityWise_PageIndexChanging"
                                            OnRowCommand="grdTCCapacityWise_RowCommand" Visible="false" OnSorting="grdTCCapacityWise_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtcCode" runat="server" placeholder="Enter DTR Code " Width="150px" onkeypress="javascript: return onlyNums(event,this);" autocomplete="off"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTR SLNO" Visible="true" SortExpression="TC_SLNO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtslno" runat="server" placeholder="Enter DTR SLNo " Width="150px" onkeypress="javascript: return onlyNums(event,this);" autocomplete="off"
                                                                MaxLength="20"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="Make">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TM_NAME") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_RATING" HeaderText="Rating">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRating" runat="server" Text='<%# Bind("TC_RATING") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>

                                        <%-- getting store dashboard failure transaction details in grid--%>
                                        <asp:GridView ID="grdTCPendingDetails" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" OnPageIndexChanging="grdTCPendingDetails_PageIndexChanging"
                                            OnRowCommand="grdTCPendingDetails_RowCommand" Visible="false" OnSorting="grdTCPendingDetails_Sorting" AllowSorting="true">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DF_DTC_CODE" HeaderText="Transformer Center Code" Visible="true" SortExpression="DF_DTC_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtlmsCode" runat="server" Text='<%# Bind("DF_DTC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel01" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtlmsCode" runat="server" placeholder="Enter DTC Code " Width="150px" onkeypress="javascript: return onlyNums(event,this);" autocomplete="off"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtcCode" runat="server" placeholder="Enter DTR Code " Width="150px" onkeypress="javascript: return onlyNums(event,this);" autocomplete="off"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTR SLNO" Visible="true" SortExpression="TC_SLNO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtslno" runat="server" placeholder="Enter DTR SLNo " Width="150px" onkeypress="javascript: return onlyAlphabets(event,this);" autocomplete="off"
                                                                MaxLength="20"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="Make">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("TM_NAME") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_RATING" HeaderText="Rating">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRating" runat="server" Text='<%# Bind("TC_RATING") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="STATUS" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("STATUS") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView ID="grdTotalDTRDetails" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                            AllowPaging="true" runat="server" OnPageIndexChanging="grdTotalDTRDetails_PageIndexChanging" OnSorting="grdTotalDTRDetails_Sorting" AllowSorting="true"
                                            OnRowCommand="grdTotalDTRDetails_RowCommand" Visible="false">
                                            <HeaderStyle CssClass="both" />
                                            <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>

                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR CODE" Visible="true" SortExpression="TC_CODE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtrCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">

                                                            <asp:TextBox ID="txtDtrCode" runat="server" placeholder="Enter DTR Code " Width="150px"  autocomplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                                MaxLength="9"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTR SLNO" Visible="true" SortExpression="TC_SLNO">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDtrslno" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                            <asp:TextBox ID="txtDtrslno" runat="server" placeholder="Enter Dtr SLNo " Width="150px"  autocomplete="off" onkeypress="javascript: return onlyAlphabets(event,this);"
                                                                MaxLength="20"></asp:TextBox>
                                                        </asp:Panel>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="CAPACITY">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                            CommandName="search" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="MAKE">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMake" runat="server" Text='<%# Bind("TM_NAME") %>' Style="word-break: break-all;"
                                                            Width="100px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="TC_RATING" HeaderText="RATING">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRating" runat="server" Text='<%# Bind("TC_RATING") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DIV_NAME" HeaderText="DIVISION NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbldivname" runat="server" Text='<%# Bind("DIV_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>


                                    </div>
                                </div>
                            </div>
                            <!-- END SAMPLE FORM PORTLET-->
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script type="text/javascript" src='<%= ResolveUrl("~/assets/jquery-slimscroll/jquery-ui-1.9.2.custom.min.js") %>'></script>
        <script type="text/javascript" src='<%= ResolveUrl("~/assets/jquery-slimscroll/jquery.slimscroll.min.js") %>'></script>
        <script src='<%= ResolveUrl("~/assets/jquery-easy-pie-chart/jquery.easy-pie-chart.js") %>' type="text/javascript"></script>
        <script src='<%= ResolveUrl("~/js/jquery.sparkline.js") %>' type="text/javascript"></script>
        <script src='<%= ResolveUrl("~/js/jquery.scrollTo.min.js") %>'></script>

    </form>
</body>
</html>
