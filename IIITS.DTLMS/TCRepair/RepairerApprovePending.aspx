<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="RepairerApprovePending.aspx.cs" Inherits="IIITS.DTLMS.TCRepair.RepairerApprovePending" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div>
        <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN THEME CUSTOMIZER-->
                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title">
                        Repairer Pending Details
                    </h3>
                    <ul class="breadcrumb" style="display: none;">
                        <li class="pull-right search-wrap">
                            <form action="" class="hidden-phone">
                            <div class="input-append search-input-area">
                                <input class="" id="Text1" type="text">
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
            <div class="row-fluid" >
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue" >
                        <div class="widget-title" >
                            <h4>
                                <i class="icon-reorder"></i>Approval Pending Details</h4>
                            <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a><a href="javascript:;"
                                class="icon-remove"></a></span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="row-fluid">

                                      <div class="span5">
                        <div class="control-group">
                            <label class="control-label">DTr Code <span class="Mandotary"> *</span></label>
                            <div class="controls">
                                <div class="input-append">                                                       
                                    <asp:TextBox ID="txtDtrcode" runat="server" MaxLength="6"  onkeypress="javascript:return OnlyNumber(event);" ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                                      

                                         <div class="span5">
                                                <asp:Button ID="cmdLoad" runat="server" Text="Search" CssClass="btn btn-primary" Width="116px"
                                                    OnClick="cmdLoad_Click" />
                                            </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="widget-body">
                           <div style="float: right">
                            </div>
                            <!-- END FORM-->
                            <div style="float: right">
                                                         </div>
                            <div class="space20">
                            </div>
                            <div style=" overflow: auto;">
                                <asp:GridView ID="grdApprovePending" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" OnPageIndexChanging="grdApprovePending_PageIndexChanging"
                                     OnSorting="grdApprovePending_Sorting" AllowSorting="true">
                               <HeaderStyle CssClass="both" />
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="dtrcode" HeaderText="DTr Code" Visible="true" SortExpression="dtrcode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("dtrcode") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="BO_NAME" HeaderText="Pending For" Visible="true" SortExpression="BO_NAME">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDf_Id" runat="server" Text='<%# Bind("BO_NAME") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="roleid" HeaderText="Pending With" Visible="true" SortExpression="roleid">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDtcName" runat="server" Text='<%# Bind("roleid") %>' Style="word-break: break-all;" Width="150px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField AccessibleHeaderText="OMSECTION" HeaderText="Location" Visible="true" SortExpression="OMSECTION">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOmSectionName" runat="server" Text='<%# Bind("OMSECTION") %>' Style="word-break: break-all;"
                                                    Width="150px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                 
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                   </div>
                    <!-- END SAMPLE FORM PORTLET-->
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
        </div>

</asp:Content>
