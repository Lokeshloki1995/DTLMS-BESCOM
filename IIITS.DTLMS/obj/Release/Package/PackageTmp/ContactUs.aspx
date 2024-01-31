﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactUs.aspx.cs" Inherits="IIITS.DTLMS.ContactUs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>Transformer Centre Life Cycle Management Software</title>
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta content="" name="description" />
    <meta content="Mosaddek" name="author" />
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="assets/bootstrap/css/bootstrap-fileupload.css" rel="stylesheet" />
    <link href="assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/style-responsive.css" rel="stylesheet" />
    <link href="css/style-default.css" rel="stylesheet" id="style_color" />
    <link href="assets/fullcalendar/fullcalendar/bootstrap-fullcalendar.css" rel="stylesheet" />
    <link href="assets/jquery-easy-pie-chart/jquery.easy-pie-chart.css" rel="stylesheet"
        type="text/css" media="screen" />
    <link href="Styles/calendar.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/functions.js"></script>
    <%--<script src='<%= ResolveUrl("~/Scripts/functions.js") %>' type="text/javascript"></script>--%>
</head>
<body class="fixed-top">
    <form id="form1" runat="server">
    <div>
        <!-- BEGIN HEADER -->
        <div id="header" class="navbar navbar-inverse navbar-fixed-top">
            <!-- BEGIN TOP NAVIGATION BAR -->
            <div class="navbar-inner">
                <div class="container-fluid">
                    <!--BEGIN SIDEBAR TOGGLE-->
                    <div class="sidebar-toggle-box hidden-phone">
                        <div class="icon-reorder tooltips" data-placement="right" data-original-title="Toggle Navigation">
                        </div>
                    </div>
                    <!--END SIDEBAR TOGGLE-->
                    <!-- BEGIN LOGO -->
                    <a class="brand" style="width: 500px; color: White">Transformer Centre Life Cycle Management Software
                        </a><!-- END LOGO --><!-- BEGIN RESPONSIVE MENU TOGGLER --><a class="btn btn-navbar collapsed"
                            id="main_menu_trigger" data-toggle="collapse" data-target=".nav-collapse"><span class="icon-bar"></span><span
                                class="icon-bar"></span><span class="icon-bar"></span><span class="arrow"></span></a><!-- END RESPONSIVE MENU TOGGLER --><!-- END  NOTIFICATION --><div
                                    class="top-nav ">
                                    <ul class="nav pull-right top-menu">
                                        <!-- BEGIN SUPPORT -->
                                        <!-- END SUPPORT -->
                                        <!-- BEGIN USER LOGIN DROPDOWN -->
                                        <li style="margin-right: 80px !important; padding-top: 11px" runat="server" id="liOffDesg">
                                            <%-- <span style="font-weight:bold"> Login Page </span>
                                               <span> <asp:Label ID="lblOfficeName" style="font-size:12px;color:White"  runat="server"></asp:Label></span>
                                            --%>
                                            <asp:LinkButton ID="lknLoginPage" runat="server" Style="font-size: 16px; color: White;
                                                font-weight: bold" OnClick="lknLoginPage_Click">Home</asp:LinkButton>
                                            <br />
                                        </li>
                                    </ul>
                                    <!-- END TOP NAVIGATION MENU -->
                                </div>
                </div>
            </div>
            <!-- END TOP NAVIGATION BAR -->
        </div>
        <!-- END HEADER -->
    </div>
    <div>
        <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
                <div class="span8">
                    <!-- BEGIN THEME CUSTOMIZER-->
                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <!-- END PAGE TITLE & BREADCRUMB-->
                </div>
                <div>
                </div>
            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="space20">
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span1">
                                        </div>

                                         <div class="span5">
                                            <div class="control-group">
                                                <label style="font-size: 24px; font-style: normal; font-weight: bold;">
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    Contact for Support :&nbsp;&nbsp; 

                                                </label>
                                            </div>
                                        </div>

                                        <div class=" pull-right">
                                                            <img src="../img/animated-hand-image-0010.gif" />
                                                                        <asp:LinkButton ID="lnkOSTicket" runat="server" Font-Underline="false" 
                                                                    style=" color:Blue;font-style: normal; font-size:20px; font-weight: bold;" 
                                                                    onclick="lnkOSTicket_Click">Click Here to Raise Ticket</asp:LinkButton>
                                                            </div>

                                         <div class="span5">
                                         </div>
                                        <br />
                                        <br />
                                        <br />
                                        <div class="span1">
                                        </div>
                                        <div class="span5">
                                            <div class="control-group">
                                                <label style="font-size: large; font-style: normal; font-weight: bold; color: #3399FF;">
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    First Level </label>
                                            </div>
                                        </div>
                                        <div align="center">
                                            <asp:GridView ID="grdFirstContactDetails" AutoGenerateColumns="false" PageSize="15"
                                                ShowHeaderWhenEmpty="true" EmptyDataText="No records Found" CssClass="table table-striped table-bordered table-advance table-hover"
                                                AllowPaging="true" runat="server" TabIndex="16" Width="800px">
                                                <Columns>
                                                    <asp:TemplateField AccessibleHeaderText="circle" HeaderText="Circle" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCircle" runat="server" Text='<%# Bind("circle") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="Div" HeaderText="Division" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDiv" runat="server" Text='<%# Bind("Div") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_NAME" HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("CC_NAME") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_MOBILENO" HeaderText="Phone Number">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFirstPhNo" runat="server" Text='<%# Bind("CC_MOBILENO") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_EMAI" HeaderText="Email Id" >
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFirstEmailId" runat="server" Text='<%# Bind("CC_EMAIL") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="space20">
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span1">
                                        </div>
                                        <div class="span5">
                                            <div>
                                                <label style="font-size: large; font-style: normal; font-weight: bold; color: #3399FF;">
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    Second Level </label>
                                            </div>
                                        </div>
                                        <div align="center">
                                            <asp:GridView ID="grdSecondContactDetails" AutoGenerateColumns="false" PageSize="10"
                                                ShowHeaderWhenEmpty="true" EmptyDataText="No records Found" CssClass="table table-striped table-bordered table-advance table-hover"
                                                AllowPaging="true" runat="server" TabIndex="16" Width="800px">
                                                <Columns>
                                                    <asp:TemplateField AccessibleHeaderText="CC_NAME" HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSecondName" runat="server" Text='<%# Bind("CC_NAME") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_MOBILENO" HeaderText="Phone Number">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSecondPhNo" runat="server" Text='<%# Bind("CC_MOBILENO") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_EMAI" HeaderText="Email Id">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSecEmailId" runat="server" Text='<%# Bind("CC_EMAIL") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="space20">
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span1">
                                        </div>
                                        <div class="span5">
                                            <div class="control-group">
                                                <label style="font-size: large; font-style: normal; font-weight: bold; color: #3399FF;">
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    Final Level </label>
                                            </div>
                                        </div>
                                        <div align="center">
                                            <asp:GridView ID="grdThirdGrid" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="true"
                                                EmptyDataText="No records Found" CssClass="table table-striped table-bordered table-advance table-hover"
                                                AllowPaging="true" runat="server" TabIndex="16" Width="800px">
                                                <Columns>
                                                    <asp:TemplateField AccessibleHeaderText="CC_NAME" HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblThirdName" runat="server" Text='<%# Bind("CC_NAME") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_MOBILENO" HeaderText="Phone Number">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblThirdPhNo" runat="server" Text='<%# Bind("CC_MOBILENO") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="CC_EMAI" HeaderText="Email Id">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblThirdEmailId" runat="server" Text='<%# Bind("CC_EMAIL") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div class="space20">
                                    </div>
                                    <div class="form-horizontal" align="center">
                                        <div class="span3">
                                        </div>
                                        <%-- <div class="span1"></div>--%>
                                        <div class="span1">
                                            <br />
                                        </div>
                                        <div class="span7">
                                        </div>
                                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="space20">
                            </div>
                            <!-- END FORM-->
                        </div>
                    </div>
                    <!-- END SAMPLE FORM PORTLET-->
                </div>
            </div>
            <!-- END PAGE CONTENT-->
        </div>
    </div>
    </form>
</body>
</html>
