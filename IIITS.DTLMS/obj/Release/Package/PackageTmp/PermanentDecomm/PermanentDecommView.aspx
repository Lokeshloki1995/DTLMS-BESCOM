﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="PermanentDecommView.aspx.cs" Inherits="IIITS.DTLMS.PermanentDecomm.PermanentDecommView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfirmDecomm() {
            var sTypeValue = document.getElementById('<%= cmbType.ClientID %>');
        var selectedText = sTypeValue.options[sTypeValue.selectedIndex].innerHTML;
        var result = confirm('Are you sure,Do you want to Decommision for ' + selectedText + ' ?');
        if (result) {
            return true;
        }
        else {
            return false;
        }
    }
</script>
     <style type="text/css">
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }

         .ascending th a {
        background:url(/img/sort_asc.png) no-repeat;
        display: block;
        padding: 0px 4px 0 20px;
    }

    .descending th a {
        background:url(/img/sort_desc.png) no-repeat;
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
    <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
               <div class="span12">
                   <!-- BEGIN THEME CUSTOMIZER-->
                 
                   <!-- END THEME CUSTOMIZER-->
                  <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                   <h3 class="page-title">
                  Permanent  Decommissioning Details View
                   </h3>
                       <a href="#" data-toggle="modal" data-target="#myModal" title="Click For Help" > <i class="fa fa-exclamation-circle" style="font-size: 36px"></i></a>
                   <ul class="breadcrumb" style="display:none">
                       
                       <li class="pull-right search-wrap">
                           <form action="" class="hidden-phone">
                               <div class="input-append search-input-area">
                                   <input class="" id="appendedInputButton" type="text">
                                   <button class="btn" type="button"><i class="icon-search"></i> </button>
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
                    <div class="widget blue" >
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i> Permanent Decommissioning Details View</h4>
                            <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                            <a href="javascript:;" class="icon-remove"></a>
                            </span>
                        </div>

                         <div class="widget-body">
                         <div class="form-horizontal">
                                    <div class="row-fluid">
                                           <div style="float:right" >
                            <div class="span1">
                         <asp:Button ID="cmbExport" runat="server" Text="Export To Excel" CssClass="btn btn-primary"
                        OnClick="Export_ClickDecomm" /><br />
                                       </div>
                           </div>
                       <%-- <div style="float:left" >--%>
                              <%--  <div class="span8">--%>

                               <div class="span4" style="display:none">
                                <asp:Label ID="lblType" runat="server" Text="Type" Font-Bold="true" 
                                        Font-Size="Medium"></asp:Label>

                                    &nbsp;&nbsp;&nbsp;&nbsp;

                                    <asp:DropDownList ID="cmbType" runat="server" AutoPostBack="true" 
                                       onselectedindexchanged="cmbType_SelectedIndexChanged" >  
                                               
                                         <asp:ListItem Text="Failure" Value="1" Selected="True"></asp:ListItem>
                                         <asp:ListItem Text="Failure With Enhancement" Value="4"></asp:ListItem>
                                          <asp:ListItem Text="Enhancement" Value="2"></asp:ListItem>
                                         <%-- <asp:ListItem Text="New DTC" Value="3"></asp:ListItem>--%>
                                    </asp:DropDownList>   
                               </div>

                              <div class="span2">
                              <asp:Label ID="Label1" runat="server" Text="Filter By :" Font-Bold="true" Visible="false"
                                        Font-Size="Medium"></asp:Label>
                              </div>
                          <div class="span1">
                            <asp:RadioButton ID="rdbViewAll" runat="server" Text="View All" CssClass="radio" 
                                  GroupName="a" Checked="false"  AutoPostBack="true" Visible="false"
                                  oncheckedchanged="rdbViewAll_CheckedChanged" />
                          </div>
                           <div class="span4">
                              <asp:RadioButton ID="rdbAlready" runat="server"  Text="Already Created" Checked="true" 
                                   CssClass="radio" GroupName="a"  AutoPostBack="true" Visible="false"
                                   oncheckedchanged="rdbAlready_CheckedChanged" />
                            </div>

                             <div style="float:right;" style="dispaly:none">
                                 <asp:Button ID="cmdNew" runat="server" Text="New" OnClientClick="return ConfirmDecomm();"
                                       CssClass="btn btn-primary" onclick="cmdNew_Click"  Visible="false" />
                             </div>

                                
                      </div>
                        </div>
                        </div>
                          
                        <div class="widget-body">
                          <div class="form-horizontal">
                              <div class="row-fluid">
                                     <asp:Label ID="lblGridType" runat="server"  Font-Bold="true" ForeColor="#4A8BC2"
                                        Font-Size="Medium"></asp:Label>
                               </div>
                           </div>
                       </div>


                      

                    <asp:GridView ID="grdReplacementDetails" AutoGenerateColumns="false" PageSize="10" AllowPaging="true"   
                      CssClass="table table-striped table-bordered table-advance table-hover"  
                      runat="server" ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found" 
                            onrowcommand="grdReplacementDetails_RowCommand" ShowFooter="true"
                            onrowdatabound="grdReplacementDetails_RowDataBound" 
                            onpageindexchanging="grdReplacementDetails_PageIndexChanging"  OnSorting="grdReplacementDetails_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" /> 
                    <Columns>
                      <asp:TemplateField AccessibleHeaderText="TR_ID" HeaderText="Id" Visible="false">                            
                            <ItemTemplate>                                                
                                <asp:Label ID="lblReplaceId" runat="server" Text='<%# Bind("PTR_ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
               

                        <asp:TemplateField AccessibleHeaderText="DT_CODE" HeaderText="Transformer Centre Code" Visible="true" SortExpression="DT_NAME">                          
                            <ItemTemplate>
                                <asp:Label ID="lblDtCode" runat="server" Text='<%# Bind("DT_CODE") %>'></asp:Label>
                            </ItemTemplate>
                             <FooterTemplate>
                              <asp:Panel ID="panel3" runat="server" DefaultButton="imgBtnSearch1" >
                                <asp:TextBox ID="txtDtcCode" runat="server" placeholder="Enter DTC Code " Width="150px" MaxLength="9" ></asp:TextBox>
                           </asp:Panel>
                             </FooterTemplate>
                        </asp:TemplateField>
      
                        <asp:TemplateField AccessibleHeaderText="DT_TC_ID" HeaderText="DTR Code">                           
                            <ItemTemplate>
                                <asp:Label ID="lblTCCode" runat="server" Text='<%# Bind("DT_TC_ID") %>'></asp:Label>
                            </ItemTemplate>
                             <FooterTemplate>
                              <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch1" >
                               <asp:TextBox ID="txtDtrCode" runat="server" placeholder="Enter DTr Code " Width="120px" MaxLength="9" ></asp:TextBox>
                           </asp:Panel>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField AccessibleHeaderText="DF_ID" HeaderText="Failure Id" Visible="false">                          
                            <ItemTemplate>
                                <asp:Label ID="lblFailureId" runat="server" Text='<%# Bind("PEST_ID") %>'></asp:Label>
                            </ItemTemplate>
                             <FooterTemplate>
                                <asp:ImageButton  ID="imgBtnSearch" runat="server"  ImageUrl="~/img/Manual/search.png"  CommandName="search" />
                             </FooterTemplate>
                        </asp:TemplateField>

                         <%-- Both Columns are same but adding for User Interface Purpose--%>
                        <asp:TemplateField AccessibleHeaderText="DF_ID" HeaderText="Enhancement ID" Visible="false">                           
                            <ItemTemplate> 
                                <asp:Label ID="lblEnhanceId" runat="server" Text='<%# Bind("PEST_ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField AccessibleHeaderText="WO_NO" HeaderText="WO No" SortExpression="WO_NO">                          
                            <ItemTemplate>
                                <asp:Label ID="lblWoNo" runat="server" Text='<%# Bind("PWO_NO_DECOM") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton  ID="imgBtnSearch1" runat="server"  ImageUrl="~/img/Manual/search.png"  CommandName="search" />
                             </FooterTemplate>
                        </asp:TemplateField>

                         <%--<asp:TemplateField AccessibleHeaderText="TI_INDENT_NO" HeaderText="Indent No" SortExpression="TI_INDENT_NO">                          
                            <ItemTemplate>
                                <asp:Label ID="lblIndentNo" runat="server" Text='<%# Bind("PTI_INDENT_NO") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton  ID="imgBtnSearch1" runat="server"  ImageUrl="~/img/Manual/search.png"  CommandName="search" />
                             </FooterTemplate>
                        </asp:TemplateField>--%>

                        <%--<asp:TemplateField AccessibleHeaderText="TI_INDENT_NO" HeaderText="Invoice No" SortExpression="IN_INV_NO" Visible="false">
                          
                            <ItemTemplate>
                                <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Bind("PTI_INDENT_NO") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>

                         <asp:TemplateField AccessibleHeaderText="" HeaderText="Decommision Done" Visible="false">           
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("STATUS") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>


                         <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <center>
                                <asp:LinkButton runat="server"  CommandName="CreateNew" ID="lnkCreate" >
                                        <img src="../Styles/images/Create.png" style="width:20px" />Decommision</asp:LinkButton>
                                    <asp:LinkButton runat="server"  CommandName="Create" ID="lnkUpdate"  visible="false" >
                                        <img src="../img/Manual/view.png" style="width:20px" />View</asp:LinkButton>
                                            
                                </center>
                            </ItemTemplate>
                             <HeaderTemplate>
                                <center>
                                    <asp:Label ID="lblHeader" runat="server" Text="Action" ></asp:Label>
                                </center>
                             </HeaderTemplate>
                       </asp:TemplateField>
                        
                    </Columns>
                                <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" />
                </asp:GridView>

                  <asp:GridView ID="grdNewDTCReplace" AutoGenerateColumns="false" PageSize="10" AllowPaging="true"                                  
                            CssClass="table table-striped table-bordered table-advance table-hover" Visible="false"
                                runat="server" ShowHeaderWhenEmpty="True" 
                            EmptyDataText="No Records Found" ShowFooter="true"
                            onpageindexchanging="grdNewDTCReplace_PageIndexChanging" 
                            onrowcommand="grdNewDTCReplace_RowCommand" 
                            onrowdatabound="grdNewDTCReplace_RowDataBound" >
                                 
         
                                <Columns>

                                 <asp:TemplateField AccessibleHeaderText="TI_ID" HeaderText="DTR Code" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTcCode1" runat="server" Text='<%# Bind("TC_CODE") %>'></asp:Label>
                                    </ItemTemplate>
                               </asp:TemplateField>

                                 <asp:TemplateField AccessibleHeaderText="WO_SLNO" HeaderText="WO Slno" Visible="false">                           
                                        <ItemTemplate> 
                                           <asp:Label ID="lblWOSlno1" runat="server" Text='<%# Bind("PWO_SLNO") %>'></asp:Label>
                                        </ItemTemplate>
                                  </asp:TemplateField>

                                   <%-- <asp:TemplateField AccessibleHeaderText="IN_NO" HeaderText="Invoice ID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoiceId1" runat="server" Text='<%# Bind("PTI_INDENT_NO") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField AccessibleHeaderText="WO_NO" HeaderText="WO No" >          
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblWoNo1" runat="server" Text='<%# Bind("PWO_NO_DECOM") %>' style="word-break: break-all;" width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                        <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch" >
                                         <asp:TextBox ID="txtWoNo" runat="server" placeholder="Enter WO No " Width="120px" MaxLength="17"></asp:TextBox>
                                      </asp:Panel>
                                       </FooterTemplate>
                                    </asp:TemplateField>

                                   
                                    <%--<asp:TemplateField AccessibleHeaderText="TI_INDENT_NO" HeaderText="Indent No" >          
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblIndentNo1" runat="server" Text='<%# Bind("PTI_INDENT_NO") %>' style="word-break: break-all;" width="120px"></asp:Label>
                                        </ItemTemplate>
                                          <FooterTemplate>
                                          <asp:Panel ID="panel4" runat="server" DefaultButton="imgBtnSearch" >
                                          <asp:TextBox ID="txtIndentNo" runat="server" placeholder="Enter Indent No " Width="120px" MaxLength="50" ></asp:TextBox>
                                      </asp:Panel>
                                       </FooterTemplate>
                                    </asp:TemplateField>       --%>                           

                                   <%-- <asp:TemplateField AccessibleHeaderText="IN_INV_NO" HeaderText="Invoice No" Visible="false">          
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblInvoiceNo1" runat="server" Text='<%# Bind("PTI_INDENT_NO") %>' style="word-break: break-all;" width="120px"></asp:Label>
                                        </ItemTemplate>
                                         <FooterTemplate>
                                         <asp:Panel ID="panel5" runat="server" DefaultButton="imgBtnSearch" >
                                         <asp:TextBox ID="txtInvoiceNo" runat="server" placeholder="Enter Invoice No " Width="120px" MaxLength="25"></asp:TextBox>
                                       </asp:Panel>                                       </FooterTemplate>
                                    </asp:TemplateField>--%>

                                     <%--<asp:TemplateField AccessibleHeaderText="IN_DATE" HeaderText="Invoice Date" >          
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblInvoiceDate" runat="server" Text='<%# Bind("PTI_DATE") %>' style="word-break: break-all;" width="80px"></asp:Label>
                                        </ItemTemplate>
                                         <FooterTemplate>
                                             <asp:ImageButton  ID="ImageButton1" runat="server"  ImageUrl="~/img/Manual/search.png"  CommandName="search" />
                                     </FooterTemplate>
                                    </asp:TemplateField>--%>

                                     <asp:TemplateField AccessibleHeaderText="" HeaderText="Decommision Done">           
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus1" runat="server" Text='<%# Bind("STATUS") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                   <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <center>
                                        
                                          <asp:LinkButton runat="server"  CommandName="CreateNew" ID="lnkCreate1" >
                                             <img src="../Styles/images/Create.png" style="width:20px" />Decommision</asp:LinkButton>
                                         <asp:LinkButton runat="server"  CommandName="Create" ID="lnkUpdate1"  visible="false" >
                                             <img src="../img/Manual/view.png" style="width:20px" />View</asp:LinkButton>
                                            
                                        </center>
                                    </ItemTemplate>
                                     <HeaderTemplate>
                                        <center>
                                            <asp:Label ID="Label2" runat="server" Text="Action" ></asp:Label>
                                        </center>
                                     </HeaderTemplate>
                                </asp:TemplateField>
                        
                                </Columns>

                            </asp:GridView>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </div>




         </div>
         </div>
         </div>

    <!-- MODAL-->
    <div class="modal fade" id="myModal" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        &times;</button>
                    <h4 class="modal-title">
                        Help</h4>
                </div>
                <div class="modal-body">
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>This Web Page Can Be Used To View Decommissioning Details of Failure, Failure and Enhancement and Enhancement.
                        </p>
                         <p style="color: Black">
                        <i class="fa fa-info-circle"></i>To View Decommissioning Details for Particular Type Select From Type DropDownList
                        </p>
                        <p style="color: Black">
                        <i class="fa fa-info-circle"></i>To Get More Details About Decommissioning click on View LinkButton 
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
