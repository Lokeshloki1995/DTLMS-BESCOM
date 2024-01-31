﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="MajorBillingView.aspx.cs" Inherits="IIITS.DTLMS.Billing.MajorBillingView" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN THEME CUSTOMIZER-->
                <!-- END THEME CUSTOMIZER-->
                <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                <h3 class="page-title">Major Billing View
                </h3>
                <a href="#" data-toggle="modal" data-target="#myModal" title="Click For Help"><i class="fa fa-exclamation-circle" style="font-size: 36px"></i></a>
                <ul class="breadcrumb" style="display: none">
                    <li class="pull-right search-wrap">
                        <form action="" class="hidden-phone">
                            <div class="input-append search-input-area">
                                <input class="" id="appendedInputButton" type="text">
                                <button class="btn" type="button">
                                    <i class="icon-search"></i>
                                </button>
                            </div>
                        </form>
                    </li>
                </ul>
                <!-- END PAGE TITLE & BREADCRUMB-->
            </div>
        </div>
        <!-- END PAGE HEADER-->
        <!-- BEGIN PAGE CONTENT-->
        <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN SAMPLE FORMPORTLET-->
                <div class="widget blue">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-reorder"></i>Major Billing View</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a><a href="javascript:;"
                            class="icon-remove"></a></span>
                    </div>
                    <div class="widget-body">
                        <div style="float: right">
                            <div class="span6">
                                <asp:Button ID="cmdNew" runat="server" Text="New Billing" CssClass="btn btn-primary"
                                    OnClick="cmdNew_Click" /><br />
                            </div>
                           
                        </div>
                        <div class="space20">
                        </div>
                        <!-- END FORM-->
                        <asp:GridView ID="grdBilling" AutoGenerateColumns="false" PageSize="10" AllowPaging="true"
                            ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"  ShowFooter="True" AllowSorting="true" CssClass="table table-striped table-bordered table-advance table-hover" runat="server" OnPageIndexChanging="grdBilling_PageIndexChanging" OnSorting="grdBilling_Sorting" OnRowCommand="grdBilling_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="SL NO" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="RO_ID" HeaderText="Id" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("MJB_ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField AccessibleHeaderText="REST_ID" HeaderText="Id" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstId" runat="server" Text='<%# Bind("RESTD_ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField AccessibleHeaderText="RWO_NO" HeaderText="WORK ORDER No" Visible="true" SortExpression="RWO_NO">

                                    <ItemTemplate>
                                        <asp:Label ID="lblName" runat="server" Text='<%# Bind("RWO_NO") %>' Style="word-break: break-all;"
                                            ></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="panel3" runat="server" DefaultButton="btnSearch">
                                            <asp:TextBox ID="txtWo" runat="server" CssClass="input_textSearch" Width="150px"
                                                placeholder="Enter WO No Name" ToolTip="Enter WO No to Search" ></asp:TextBox>
                                        </asp:Panel>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="REST_NO" HeaderText="ESTIMATION NO"
                                    Visible="true" SortExpression="REST_NO">

                                    <ItemTemplate>
                                        <asp:Label ID="lblEstNo" runat="server" Text='<%# Bind("RESTD_NO") %>' Style="word-break: break-all;"
                                           ></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="txtEstNo" runat="server" CssClass="input_textSearch" Width="150px"
                                            placeholder="Enter Est No" ToolTip="Enter Estimation No to Search" ></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="MJB_INV_NO" HeaderText="INVOICE NO" Visible="true" SortExpression="MB_INV_NO">

                                    <ItemTemplate>
                                        <asp:Label ID="lblInvno" runat="server" Text='<%# Bind("MJB_INV_NO") %>' Style="word-break: break-all;"
                                           ></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="panel2" runat="server" DefaultButton="btnSearch">
                                            <asp:TextBox ID="txtInvNo" runat="server" CssClass="input_textSearch" Width="150px"
                                                placeholder="Enter Inv No" ToolTip="Enter Invoice No to Search" ></asp:TextBox>
                                        </asp:Panel>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="MJB_INV_AMOUNT" HeaderText="INVOICE AMOUNT" Visible="true" SortExpression="MJB_INV_AMOUNT">

                                    <ItemTemplate>
                                        <asp:Label ID="lblInvAmnt" runat="server" Text='<%# Bind("MJB_INV_AMOUNT") %>' Style="word-break: break-all;"
                                            ></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="panel1" runat="server" DefaultButton="btnSearch">

                                            <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                Height="25px" ToolTip="Search" TabIndex="9" CommandName="search" />
                                        </asp:Panel>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <center>
                                            <asp:ImageButton ID="imgBtnEdit" runat="server" Height="12px" ImageUrl="~/Styles/images/edit64x64.png"
                                                Width="12px" OnClick="imgBtnEdit_Click"/>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                            </Columns>
                        </asp:GridView>
                        <div class="span7">
                        </div>
                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
   
</asp:Content>


