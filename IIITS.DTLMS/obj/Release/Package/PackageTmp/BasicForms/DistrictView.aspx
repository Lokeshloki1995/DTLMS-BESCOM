﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="DistrictView.aspx.cs" Inherits="IIITS.DTLMS.BasicForms.DistrictView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
    </script>
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
    <style type="text/css">
        .modalBackground {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }

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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div>

        <div class="container-fluid">
            <%--BEGIN PAGE HEADER--%>
            <div class="row-fluid">
                <div class="span8">
                    <!-- BEGIN THEME CUSTOMIZER-->

                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title">District View
                    </h3>
                    <a href="#" data-toggle="modal" data-target="#myModal" title="Click For Help"><i class="fa fa-exclamation-circle" style="font-size: 36px"></i></a>

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

            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>District View</h4>

                            <span class="tools">
                                <a href="javascript:;" class="icon-chevron-down"></a>
                                <a href="javascript:;" class="icon-remove"></a>
                            </span>
                        </div>
                        <div class="widget-body">



                            <div style="float: right">
                                <div class="span6">
                                    <asp:Button ID="cmdNewDistrict" class="btn btn-primary" Text="New District"
                                        runat="server" OnClick="cmdNewDistrict_Click" />
                                    <br />
                                </div>
                                <div class="span1">
                                    <asp:Button ID="cmdexport" runat="server" Text="Export Excel" CssClass="btn btn-primary"
                                        OnClick="Export_clickDistrict" /><br />
                                </div>





                                <br />
                            </div>

                        </div>


                        <div class="space20"></div>

                        <div>

                            <div>
                                <asp:GridView ID="grdDistrictdetails" AutoGenerateColumns="false" PageSize="10" AllowPaging="true"
                                    ShowFooter="true" EmptyDataText="No Records Found"
                                    CssClass="table table-striped table-bordered table-advance table-hover" runat="server"
                                    ShowHeaderWhenEmpty="True" AlternatingRowStyle-BorderStyle="None"
                                    OnPageIndexChanging="grdDistrictdetails_PageIndexChanging"
                                    OnRowCommand="grdDistrictdetails_RowCommand" OnSorting="grdDistrictdetails_Sorting" AllowSorting="true">
                                    <HeaderStyle CssClass="both" />
                                    <AlternatingRowStyle BorderStyle="None"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="Dist_ID" HeaderText="Id" Visible="false">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDistId" runat="server" Text='<%# Bind("DT_ID") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDistId" runat="server" Text='<%# Bind("DT_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>



                                        <asp:TemplateField AccessibleHeaderText="Dist_CODE" HeaderText="District Code" Visible="true" SortExpression="DT_CODE">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDistCode" runat="server" Text='<%# Bind("DT_CODE") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDistCode" runat="server" Text='<%# Bind("DT_CODE") %>' Style="word-break: break-all;"
                                                    Width="200px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txtDistrictCode" runat="server" placeholder="Enter District Code" Width="150px" CssClass="span12" ></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField AccessibleHeaderText="Dist_NAME" HeaderText="District Name" Visible="true" SortExpression="DT_NAME">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDistName" runat="server" Text='<%# Bind("DT_NAME") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDistName" runat="server" Text='<%# Bind("DT_NAME") %>' Style="word-break: break-all;"
                                                    Width="200px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txtDistrictName" runat="server" placeholder="Enter District Name" Width="150px" CssClass="span12"  ></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png" CommandName="search" />
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Edit">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnEdit" runat="server" Height="12px" OnClick="imgBtnEdit_Click"
                                                        ImageUrl="~/Styles/images/edit64x64.png" Width="12px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                                </asp:GridView>
                            </div>







                        </div>



                    </div>
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
        </div>

        <!-- END FORM-->

        <!-- END PAGE CONTENT-->
    </div>



    </div>

    <!-- MODAL-->
    <div class="modal fade" id="myModal" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        &times;</button>
                    <h4 class="modal-title">Help</h4>
                </div>
                <div class="modal-body">
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>This Web Page Can Be Used To View District Details and To Add New District 
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>To Edit Existing Details Click On <u>Edit</u> LiknkButton
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>To Add New District Click On <u>New District</u> LiknkButton
                    </p>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">
                        Close</button>
                </div>
            </div>
        </div>
    </div>
    <!-- MODAL-->

</asp:Content>
