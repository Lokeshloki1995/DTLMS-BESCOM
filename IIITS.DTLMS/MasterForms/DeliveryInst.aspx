﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="DeliveryInst.aspx.cs" Inherits="IIITS.DTLMS.MasterForms.DeliveryInst" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>
     <script type="text/javascript">

         function AllowOnlyAlphanumericNotAllowSpecial(evt) {
             evt = (evt) ? evt : event;
             var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
            ((evt.which) ? evt.which : 0));

             if ((charCode > 47 && charCode < 58) ||
                 (charCode > 64 && charCode < 91) ||
                 (charCode > 96 && charCode < 123)) {
                 return true;
             }

             else { return false; }
         }
         function DisableSplChars(evt) {
             var charCode = (evt.which) ? evt.which : event.keyCode
             if (charCode == 92) {
                 return false;
             } else {

                 return true;
             }
         }

         function ConfirmDelete() {

             var result = confirm('Are you sure you want to Delete?');
             if (result) {
                 return true;
             }
             else {

                 return false;
             }
         }
      </script>
  <%--<script  type="text/javascript">
      function ValidateMyForm() {
          if (document.getElementById('<%= txtDINumber.ClientID %>').value.trim() == "") {
              alert('Enter  DI Number')
              document.getElementById('<%= txtDINumber.ClientID %>').focus()
              return false
          }
          if (document.getElementById('<%= txtDIDate.ClientID %>').value.trim() == "") {
              alert('Enter  DI Date')
              document.getElementById('<%= txtDIDate.ClientID %>').focus()
              return false
          }
          if (document.getElementById('<%= txtConsignee.ClientID %>').value.trim() == "") {
              alert('Enter  Consignee')
              document.getElementById('<%= txtConsignee.ClientID %>').focus()
              return false
          }
          if (document.getElementById('<%= cmbMake.ClientID %>').value == "-Select-") {
                  alert('Select the  Make')
                  document.getElementById('<%= cmbMake.ClientID %>').focus()
              return false
          }

          if (document.getElementById('<%= cmbCapacity.ClientID %>').value == "--Select--") {
              alert('Select the  Capacity')
              document.getElementById('<%= cmbCapacity.ClientID %>').focus()
                  return false
          }

          if (document.getElementById('<%= txtQuantity.ClientID %>').value.trim() == "") {
              alert('Enter the  Quantity')
              document.getElementById('<%= txtQuantity.ClientID %>').focus()
              return false
          }

          if (document.getElementById('<%= cmbRating.ClientID %>').value == "-Select-") {
              alert('Select the  Rating')
              document.getElementById('<%= cmbRating.ClientID %>').focus()
              return false
          }

      }
     </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="container-fluid">
        <!-- BEGIN PAGE HEADER-->
        <div class="row-fluid">
            <div class="span12">
                <!-- BEGIN THEME CUSTOMIZER-->

                <!-- END THEME CUSTOMIZER-->
                <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                <h3 class="page-title" runat="server" id="Create">Dispatch Instructions
               <%-- <div style="float: right">

                    <asp:Button ID="cmdClose" runat="server" Text="PO View"
                        CssClass="btn btn-primary" /><br />
                </div>--%>
                    <div style="float: right">

                    <asp:Button ID="cmdClose" runat="server" style="margin-top: 14px; margin-right: 1px;" Text="DI View"
                        CssClass="btn btn-primary" OnClick="cmdClose_Click" /><br />
                </div>

                </h3>

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
            </div>
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4 id="CreateDI"  runat="server"><i class="icon-reorder"></i>Create Dispatch Instructions</h4>
                            <span class="tools">
                                <a href="javascript:;" class="icon-chevron-down"></a>
                            </span>
                        </div>
                        
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="row-fluid">
                                       
                                        <div class="span6">
                                            <div class="control-group">
                                                <label class="control-label">PO Number <span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtPoId" runat="server" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtPoNumber" runat="server" MaxLength="20" ></asp:TextBox>
                                                        <asp:Button ID="cmdSearch" Text="S" class="btn btn-primary" runat="server" OnClick="cmdSearch_Click" /><br />
                                                    </div>
                                                </div>
                                            </div>


                                        </div>
                                        <div class="span6">
                                            <div class="control-group">
                                                <label  class="control-label">
                                                    Quantity <span class="Mandotary">*</span>
                                                </label>
                                                <div class="input-append"">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtTotalQuantity" ReadOnly="true" runat="server" MaxLength="8" onkeypress="javascript:return OnlyNumber(event);"></asp:TextBox>
                                                        
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                   <div class="space20"></div>
                                    <div class="row-fluid">
                                       
                                 <div class="span7">
                                   <asp:GridView ID="grdPendingTC" AutoGenerateColumns="false" PageSize="5"
                                    ShowHeaderWhenEmpty="true" EmptyDataText="No records Found"
                                    CssClass="table table-striped table-bordered table-advance table-hover" OnPageIndexChanging="grdPendingTC_PageIndexChanging" AllowPaging="true"
                                    runat="server" TabIndex="16" style="width:600px;">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="PO_ID" HeaderText="Po number">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPOId" runat="server" Text='<%# Bind("PO_NO") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="PB_MAKE" HeaderText="Make" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("PB_MAKE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="MAKE" HeaderText="Make">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMake" runat="server" Text='<%# Bind("MAKE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="CAPACITY" HeaderText="Capacity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCapacity" runat="server" Text='<%# Bind("CAPACITY") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="PO_RATING" HeaderText="Rating"  Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRatingId" runat="server" Text='<%# Bind("PO_RATING") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="RATING" HeaderText="Rating">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRating" runat="server" Text='<%# Bind("RATING") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                      
                                        <asp:TemplateField AccessibleHeaderText="TOTAL" HeaderText="Total Quantity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Bind("TOTAL") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="PENDING" HeaderText="Pending Quantity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPenQuantity" runat="server" Text='<%# Bind("PENDING") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                                    </asp:GridView>
                                                    </div>
                                         <div class="span5">
                                
                                      <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="false" PageSize="5" ShowHeaderWhenEmpty="true" EmptyDataText="No records Found" ShowFooter="false" 
                                                    CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"   
                                                      OnPageIndexChanging="gvFiles_PageIndexChanging"  AllowSorting="true">
                                                 <Columns>
                                                  <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                   <ItemTemplate>
                                                       <%#Container.DataItemIndex+1 %>
                                                       </ItemTemplate>
                                                 </asp:TemplateField>
                                                  <asp:BoundField DataField="Name" ItemStyle-ForeColor="BlueViolet" HeaderText="Download PO Documents" ItemStyle-Width="600"   />     
                                                 <asp:TemplateField>
                                               <ItemTemplate>
                                           <asp:LinkButton ID="lnkDownload" runat="server" ForeColor="Blue" Text="Download" OnClick="DownloadFile"
                                              CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
              

                                      </Columns>
                                      </asp:GridView>
                                       
                                           
                                <%-- <asp:LinkButton ID="lnkPoDownload" runat="server" Visible="false" OnClick="lnkPoDownload_Click">  <img src="../img/Manual/Pdficon.png" style="width:20px" />Click Here to Download PO</asp:LinkButton>--%>
                                    </div>
                            
                                </div>
                              <div class="space20"></div>
                                <div class="form-horizontal" align="center">

                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <!-- END SAMPLE FORM PORTLET-->
                    </div>
                </div>
            </div>

            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Capacity Details</h4>
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
                                                <label class="control-label">DI Number<span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtDIId" runat="server" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtDINumber" runat="server" MaxLength="20" onkeypress="javascript:return DisableSplChars(this,event);" ></asp:TextBox>
                                                     <%--<asp:regularexpressionvalidator id="regular1" controltovalidate="txtDINumber" runat="server" errormessage="only chars and nums accept" validationexpression="[a-zA-Z0-9]+"></asp:regularexpressionvalidator>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Consignee<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtConsignee" runat="server" MaxLength="25" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Due Date</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtDueDate" runat="server"  MaxLength="10"></asp:TextBox>
                                                        <ajax:CalendarExtender ID="DeliveryCalendar" runat="server" CssClass="cal_Theme1" Format="dd/MM/yyyy"
                                                            TargetControlID="txtDueDate">
                                                        </ajax:CalendarExtender>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                            <label class="control-label">Make <span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:DropDownList ID="cmbMake" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="cmbMake_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                            <div class="control-group">
                                                <label class="control-label">Store</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbStore" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="span5">

                                            <div class="control-group">
                                               <label class="control-label">DI Date<span class="Mandotary">*</span></label>
                                                <%--<label class="control-label">Rate Contract Order Date<span class="Mandotary">*</span></label>--%>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtDIDate" runat="server"  MaxLength="10"></asp:TextBox>
                                                        <ajax:CalendarExtender ID="ManufactureCalender" runat="server" CssClass="cal_Theme1" Format="dd/MM/yyyy"
                                                            TargetControlID="txtDIDate">
                                                        </ajax:CalendarExtender>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                            <label class="control-label">Capacity(in KVA) <span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:DropDownList ID="cmbCapacity" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="cmbCapacity_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                            <div class="control-group">
                                            <label class="control-label">Quantity <span class="Mandotary">*</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="8" onkeypress="javascript:return OnlyNumber(event);"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Rating<span class="Mandotary"> *</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:DropDownList ID="cmbRating" runat="server" TabIndex="15" >

                                                    </asp:DropDownList>
                                                    <asp:Button ID="cmdAdd" runat="server" Enabled ="false" Text="Add" OnClientClick="return ValidateForm1();" CssClass="btn btn-primary" OnClick="cmdAdd_Click" />
                                                </div>
                                            </div>
                                        </div>
                                              <div class="control-group">
                                            <label class="control-label">Delivery Note<span class="Mandotary"> *</span></label>
                                            <div class="controls">
                                                <div class="input-append">
                                                    <asp:FileUpload ID="fupFile" runat="server" />
                                                     <asp:Label ID="lblFilename" runat="server" Text="Initial Text"></asp:Label>
                                                   </div>
                                            </div>
                                        </div>
                                        </div>

                                    </div>
                                    
                                <div class="space20"></div>
                                    
                                <asp:GridView ID="grdDelivery" AutoGenerateColumns="false" PageSize="10"
                                    ShowHeaderWhenEmpty="true" EmptyDataText="No records Found"
                                    CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"
                                    runat="server" TabIndex="16" OnPageIndexChanging="grdDelivery_PageIndexChanging" OnRowCommand="grdDelivery_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="ID" HeaderText="ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiId" runat="server" Text='<%# Bind("DI_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="PO_ID" HeaderText="TC ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblpoId" runat="server" Text='<%# Bind("DI_PO_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_NO" HeaderText="DI number">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiNo" runat="server" Text='<%# Bind("DI_NO") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_DATE" HeaderText="DI Date" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDIDate" runat="server" Text='<%# Bind("DI_DATE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField AccessibleHeaderText="DI_CONSIGNEE" HeaderText="Consignee">
                                            <ItemTemplate>
                                                <asp:Label ID="lblConsignee" runat="server" Text='<%# Bind("DI_CONSIGNEE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField AccessibleHeaderText="DI_STORE_ID" HeaderText="StoreId" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStoreId" runat="server" Text='<%# Bind("DI_STORE_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField AccessibleHeaderText="DI_STORE" HeaderText="Store">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStoreName" runat="server" Text='<%# Bind("DI_STORE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField AccessibleHeaderText="DI_DUEDATE" HeaderText="DI DueDate" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDuedate" runat="server" Text='<%# Bind("DI_DUEDATE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                       
                                          <asp:TemplateField AccessibleHeaderText="DI_MAKE_ID" HeaderText="Make ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMakeId" runat="server" Text='<%# Bind("DI_MAKE_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_MAKE" HeaderText="Make">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMake" runat="server" Text='<%# Bind("DI_MAKE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_CAPACITY" HeaderText="Capacity" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCapacityID" runat="server" Text='<%# Bind("DI_CAPACITY_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_CAPACITY" HeaderText="Capacity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCapacity" runat="server" Text='<%# Bind("DI_CAPACITY") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_STARRATE" HeaderText="Rating" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRating" runat="server" Text='<%# Bind("DI_STARRATE") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_STARRATENAME" HeaderText="Rating">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRatingName" runat="server" Text='<%# Bind("DI_STARRATENAME") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="DI_QUANTITY" HeaderText="Quantity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Bind("DI_QUANTITY") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Edit">
                                                <ItemTemplate>
                                                    <center>
                                                        <asp:ImageButton ID="imgBtnEdit" runat="server" Height="12px" ImageUrl="~/Styles/images/edit64x64.png"
                                                            CommandName="EditQNTY" Width="12px"/>
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnDelete" runat="server" Height="12px" ImageUrl="~/Styles/images/delete64x64.png"
                                                        CommandName="Remove" Width="12px" OnClientClick="return ConfirmDelete();" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                          
                                         <div class="space20"></div>
                                            
                                         
                                        
                                              <asp:GridView ID="grdDIdocs" runat="server" AutoGenerateColumns="false" PageSize="5" ShowHeaderWhenEmpty="true" EmptyDataText="No records Found" ShowFooter="false" 
                                                    CssClass="table table-striped table-bordered table-advance table-hover" AllowPaging="true"   
                                                      OnPageIndexChanging="grdDIdocs_PageIndexChanging"  AllowSorting="true">
                                                 <Columns>
                                                  <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                   <ItemTemplate>
                                                       <%#Container.DataItemIndex+1 %>
                                                       </ItemTemplate>
                                                 </asp:TemplateField>
                                                  <asp:BoundField DataField="Name" ItemStyle-ForeColor="BlueViolet" HeaderText="Download Delivey Documents" ItemStyle-Width="700"   />     
                                                 <asp:TemplateField>
                                               <ItemTemplate>
                                           <asp:LinkButton ID="lnkDownload" runat="server" ForeColor="Blue" Text="Download" OnClick="DownloadFile1"
                                              CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
              

                                      </Columns>
                                      </asp:GridView>
                                   </div>
                                       
                                           
                                <div class="space20"></div>
                                <div class="form-horizontal" align="center">
                                    <div class="span3"></div>
                                    <div class="span1">
                                        <asp:Button ID="btnSave" runat="server" Text="Save"
                                            OnClientClick="javascript:return ValidateMyForm()" CssClass="btn btn-primary" OnClick="btnSaveUpdate_Click" />
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" Visible="false"
                                            OnClientClick="javascript:return ValidateMyForm()" CssClass="btn btn-primary" OnClick="btnSaveUpdate_Click" />
                                    </div>

                                    <div class="span1">
                                        <asp:Button ID="btnReset" runat="server" Text="Reset"
                                            CssClass="btn btn-primary" OnClick="btnReset_Click" />
                                    </div>
                                </div>

                                </div>

                            </div>

                            <div class="space20"></div>
                             <div class="form-horizontal" align="center">

                                    <asp:Label ID="lblNote" runat="server" ForeColor="Red"></asp:Label>
                                </div>

                            <!-- END FORM-->
                        </div>
                    </div>
                    <!-- END SAMPLE FORM PORTLET-->
                </div>
            </div>
            <!-- END PAGE CONTENT-->

        <style>
    a#ContentPlaceHolder1_gvFiles_lnkDownload_0 {
    white-space: nowrap;
}
</style>
</asp:Content>

