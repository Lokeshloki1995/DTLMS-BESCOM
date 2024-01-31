﻿<%@ Page Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="Permanentestimationview.aspx.cs" Inherits="IIITS.DTLMS.PermanentDecomm.Permanentestimationview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">


        <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN THEME CUSTOMIZER-->

                <!-- END THEME CUSTOMIZER-->
                <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                <h3 class="page-title">Permanent Decomm View
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

        <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN SAMPLE FORMPORTLET-->
                <div class="widget blue">
                    <div class="widget-title">
                        <h4><i class="icon-reorder"></i>Permanent Decomm View</h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                            <a href="javascript:;" class="icon-remove"></a>
                        </span>
                    </div>

                    <div class="widget-body">
                        <div class="form-horizontal">
                            <div class="row-fluid">
                                <%-- <div style="float:left" >--%>
                                <%--  <div class="span8">--%>
                                <div class="span2">
                                    <asp:Label ID="lblStatus" runat="server" Text="Filter By :" Font-Bold="true"
                                        Font-Size="Medium"></asp:Label>
                                </div>
                                <div class="span1">
                                    <asp:RadioButton ID="rdbViewAll" runat="server" Text="View All" CssClass="radio"
                                        GroupName="a" Checked="true" AutoPostBack="true"
                                        oncheckedchanged="rdbViewAll_CheckedChanged"/>
                                </div>
                                <div class="span4">
                                    <asp:RadioButton ID="rdbAlready" runat="server" Text="Already Created"
                                        CssClass="radio" GroupName="a" AutoPostBack="true"
                                         oncheckedchanged="rdbAlready_CheckedChanged" />
                                </div>
                                <div style="float: right;">
                                    <div class="span4" >
                                        <asp:Button ID="cmdNew" runat="server" Text="New"
                                            CssClass="btn btn-primary" onclick="cmdNew_Click" />
                                    </div>

                                    <div class="span1">
                                    </div>


                                </div>
                            </div>
                        </div>
                    </div>

                  <%--code--%>
                     <asp:GridView ID="grdPermanentDecommDetails" AutoGenerateColumns="false" PageSize="10" 
                  AllowPaging="true"  ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found" 
                 CssClass="table table-striped table-bordered table-advance table-hover" ShowFooter="true"
                  runat="server"  onpageindexchanging="grdPermanentDecomm_PageIndexChanging" 
                                                onrowcommand="grdPermanentDecomm_RowCommand" 
                            onrowdatabound="grdPermanentDecomm_RowDataBound" OnSorting="grdPermanentDecomm_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" /> 
    
                    <Columns>
                        <asp:TemplateField HeaderText="SL NO" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="PEST_ID" HeaderText="Id" Visible="false">
           
                            <ItemTemplate>                                       
                                <asp:Label ID="lblEstId" runat="server" Text='<%# Bind("PEST_ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>


                    <asp:TemplateField AccessibleHeaderText="DT_ID" HeaderText="Id" Visible="false">
                            <ItemTemplate>              
                                <asp:Label ID="lblId" runat="server" Text='<%# Bind("DT_ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transfromer Centre Code"  SortExpression="DT_CODE">
                            <ItemTemplate>

                                <asp:LinkButton runat="server"  CommandName="CreateNew" ID="lnkCreateDTC" >
                                <asp:Label ID="lblDtcCode" runat="server" Text='<%# Bind("DT_CODE") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                              <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch" >
                                <asp:TextBox ID="txtDtCode" runat="server" placeholder="Enter DTC Code " Width="100px" MaxLength="9" ></asp:TextBox>
                             </asp:Panel>
                             </FooterTemplate>
                        </asp:TemplateField>

                          <asp:TemplateField AccessibleHeaderText="DT_TIMS_CODE" HeaderText="Tims Code"  SortExpression="DT_CODE">
                             <ItemTemplate>
                                <asp:Label ID="lblTimscode" runat="server" Text='<%# Bind("DT_TIMS_CODE") %>' style="word-break: break-all;" width="150px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                              <asp:Panel ID="paneltims" runat="server" DefaultButton="imgBtnSearch" >
                                <asp:TextBox ID="txtTimsCode" runat="server" placeholder="Enter Tims Code " Width="100px" MaxLength="13" ></asp:TextBox>
                             </asp:Panel>
                             </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField AccessibleHeaderText="DT_NAME" HeaderText="Transfromer Centre Name" SortExpression="DT_NAME">
                            <ItemTemplate>
                                <asp:Label ID="lblname" runat="server" Text='<%# Bind("DT_NAME") %>' style="word-break: break-all;" width="150px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                              <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch" >
                                  <asp:TextBox ID="txtDtName" runat="server" placeholder="Enter DTC Name" Width="150px" MaxLength="50" ></asp:TextBox>
                            </asp:Panel>
                             </FooterTemplate>
                        </asp:TemplateField>

                          <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code">          
                            <ItemTemplate> 
                                <asp:LinkButton runat="server"  CommandName="CreateNew" ID="lnkCreateDTR" >                                                
                                <asp:Label ID="lblTcCode" runat="server" Text='<%# Bind("TC_CODE") %>' style="word-break: break-all;" width="100px"></asp:Label>
                                    </asp:LinkButton>
                            </ItemTemplate>
                              <FooterTemplate>
                                <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch" >
                                 <asp:TextBox ID="txtDtrCode" runat="server"  placeholder="Enter DTr Code" Width="75px" MaxLength="10" ></asp:TextBox>
                             </asp:Panel>
                             </FooterTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="DTr Slno">
                            <ItemTemplate>
                                <asp:Label ID="lblslno" runat="server" Text='<%# Bind("TC_SLNO") %>' style="word-break: break-all;" width="100px"></asp:Label>
                            </ItemTemplate>
                            
                        </asp:TemplateField>

                        <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="Make Name" SortExpression="TM_NAME">
                            <ItemTemplate>
                                <asp:Label ID="lbltmname" runat="server" Text='<%# Bind("TM_NAME") %>' style="word-break: break-all;" width="150px"></asp:Label>
                            </ItemTemplate>
                             <FooterTemplate>
                                   <asp:ImageButton  ID="imgBtnSearch" runat="server"  ImageUrl="~/img/Manual/search.png"  CommandName="search" />
                             </FooterTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity(in KVA)" >
                           
                            <ItemTemplate>
                                <asp:Label ID="lblcapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>



                        <asp:TemplateField AccessibleHeaderText="" HeaderText="Failure Entry" Visible="false">           
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("STATUS") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
     
                        <asp:TemplateField HeaderText="Action" >
                            <ItemTemplate>
                                <center>
                                  <%--  <asp:ImageButton ID="imgBtnEdit" runat="server" Height="12px" ImageUrl="~/Styles/images/edit64x64.png"
                                        Width="12px" />--%>
                                      <asp:LinkButton runat="server"  CommandName="CreateNew" ID="lnkCreate" >
                                      <img src="../Styles/images/Create.png" style="width:20px" />Declare</asp:LinkButton>
                                      <asp:LinkButton runat="server"  CommandName="Preview1" ID="lnkUpdate"  visible="false" >
                                      <img src="../img/Manual/view.png" style="width:20px" />View</asp:LinkButton>
                                      <asp:LinkButton runat="server"  CommandName="Preview"  ID="lnkWaiting"  visible="false" >
                                      <img id="lnkbtnwait" src="../img/Manual/Wait.png" style="width:20px" />Waiting for Approval</asp:LinkButton>
                                    

                                </center>
                            </ItemTemplate>
                            <HeaderTemplate>
                                <center>
                                    <asp:Label ID="lblHeader" runat="server" Text="Action" ></asp:Label>
                                </center>
                            </HeaderTemplate>
                        </asp:TemplateField>

                         <asp:TemplateField AccessibleHeaderText="DT_PROJECTTYPE" HeaderText="Project Type" Visible="false">                                       
                            <ItemTemplate>
                                <asp:Label ID="lblProjectType" runat="server" Text='<%# Bind("DT_PROJECTTYPE") %>' style="word-break: break-all;" width="60px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                                <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" PageButtonCount="5" PreviousPageText="Last" />
                </asp:GridView>
                    <div class="span7"></div>
                                 <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>

                </div>
            </div>
        </div>

    </div>

</asp:Content>


