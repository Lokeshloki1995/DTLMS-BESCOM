<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="TCRepairIssue.aspx.cs" Inherits="IIITS.DTLMS.TCRepair.TCRepairIssue" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>

     <script type="text/javascript">
 
function preventMultipleSubmissions() {
    <%-- $('#<%=cmdSave.ClientID %>').prop('disabled', false);--%>
    $('#<%=cmdSave.ClientID %>').prop('disabled', true);
}
 
window.onbeforeunload = preventMultipleSubmissions;
 
</script>
    <script type="text/javascript" >

        function ValidateMyForm() {

            if (document.getElementById('<%= cmbGuarantyType.ClientID %>').value == "-Select-") {
                alert('Select Guarantee Type')
                document.getElementById('<%= cmbGuarantyType.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= ddlType.ClientID %>').value == "-Select-") {
                alert('Select Type (Supplier/Repairer)')
                document.getElementById('<%= ddlType.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbRepairer.ClientID %>').value == "--Select--") {
                alert('Select Repairer / Supplier')
                document.getElementById('<%= cmbRepairer.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtIssueDate.ClientID %>').value.trim() == "") {
                alert('Enter Valid Issue Date')
                document.getElementById('<%= txtIssueDate.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= txtPONo.ClientID %>').value.trim() == "") {
                alert('Enter Valid Purchase Order No.')
                document.getElementById('<%= txtPONo.ClientID %>').focus()
                return false
            }
           // if (document.getElementById('<%= txtPODate.ClientID %>').value.trim() == "") {
              //  alert('Enter Valid Purchase Order Date')
               // document.getElementById('<%= txtPODate.ClientID %>').focus()
               // return false
           // }
            if (document.getElementById('<%= txtInvoiceNo.ClientID %>').value.trim() == "") {
                alert('Enter Valid Invoice No.')
                document.getElementById('<%= txtInvoiceNo.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= txtInvoiceDate.ClientID %>').value.trim() == "") {
                alert('Enter Valid Invoice Date')
                document.getElementById('<%= txtInvoiceDate.ClientID %>').focus()
                return false
            }

           
        }

    

        function ConfirmDelete() {
            var result = confirm('Are you sure you want to Remove?');
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
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ajax:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </ajax:ToolkitScriptManager>
<div>
      <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
               <div class="span8">
                   <!-- BEGIN THEME CUSTOMIZER-->
                 
                   <!-- END THEME CUSTOMIZER-->
                  <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                   <h3 class="page-title">
                    Fault Transformer Issue                    
                                      
                   </h3>
                   
                   <ul class="breadcrumb" style="display:none">
                       
                       <li class="pull-right search-wrap">
                           <form action="" class="hidden-phone">
                               <div class="input-append search-input-area">
                                   <input class="" id="appendedInputButton" type="text"/>
                                   <button class="btn" type="button"><i class="icon-search"></i> </button>
                               </div>
                           </form>
                       </li>
                   </ul>
                   <!-- END PAGE TITLE & BREADCRUMB-->
               </div>
                <div style="float:right;margin-top:20px;margin-right:12px" >
                      <asp:Button ID="cmdClose" runat="server" Text="Close"  
                                       CssClass="btn btn-primary" onclick="cmdClose_Click" />
                </div>
            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Supplier / Repairer Details</h4>
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

                                        <label class="control-label">Guarantee Type
                                         
                                                <span class="Mandotary"> *</span>
                                          </label>
                                            <div class="controls">
                                                <div class="input-append">
                                                         <asp:DropDownList ID="cmbGuarantyType" runat="server">
                                                           <asp:ListItem >-Select-</asp:ListItem>
                                                                <asp:ListItem Value="AGP">AGP</asp:ListItem>
                                                            <asp:ListItem Value="WGP">WGP</asp:ListItem>
                                                            <asp:ListItem Value="WRGP">WRGP</asp:ListItem>
                                                         </asp:DropDownList>
                                                      
                                                </div>
                                            </div>
                                           
                                        </div>
                                        <div class="control-group">

                                        <label class="control-label">Type
                                         
                                                <span class="Mandotary"> *</span>
                                           </label>
                                            <div class="controls">
                                                <div class="input-append">
                                                         <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" onselectedindexchanged="ddlType_SelectedIndexChanged" >
                                                           <asp:ListItem >-Select-</asp:ListItem>
                                                                <asp:ListItem Value="2">Repairer</asp:ListItem>
                                                            <asp:ListItem Value="1">Supplier</asp:ListItem>
                                                         </asp:DropDownList>
                                                      
                                                </div>
                                            </div>
                                           
                                        </div>



                                        <div class="control-group">
                                             <label class="control-label">
                                                <asp:Label ID="lblSuppRep" runat="server" Text="Supplier/Repairer"></asp:Label>
                                            
                                           
                                                <span class="Mandotary"> *</span>
                                           </label>
                                                <div class="controls">
                                                    <div class="input-append">      
                                                        <asp:TextBox ID="txtStoreId" runat="server" Visible="false" Width="20px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfTccode" runat="server" />        
                                                        <asp:HiddenField ID="hdftcQnty" runat="server" />    
                                                                                     
                                                         <asp:DropDownList ID="cmbRepairer" runat="server" AutoPostBack="true"
                                                             onselectedindexchanged="cmbRepairer_SelectedIndexChanged" TabIndex="1"> </asp:DropDownList>    
                                                    </div>
                                                </div>

                                             <div class="space5"></div>
                                            <div class="space5"></div>
                                            <div class="space5"></div>
                                            
                                            <label class="control-label">Name</label>
                                             <div class="span1">
                                               
                                            </div>
                                            <div class="controls">
                                                <div class="input-append">                                                       
                                                    <asp:TextBox ID="txtName" runat="server" MaxLength="50" TabIndex="2" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                          

                                        </div>

                                    </div>
                                       
                                    <div class="span5">
                                        <div class="control-group">
                                            <label class="control-label">Phone</label>
                                           
                                                <div class="controls">
                                                    <div class="input-append">
                                                          <asp:TextBox ID="txtPhone" runat="server" MaxLength="10" TabIndex="3" ReadOnly="true"></asp:TextBox>                                             
                                                    </div>
                                                </div>
                                        </div>

                                         <div class="control-group">
                                            <label class="control-label">Address</label>
                                            
                                            <div class="controls">
                                                <div class="input-append">                                                       
                                                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="50" TabIndex="4" TextMode="MultiLine" style="resize:none" 
                                                    onkeyup="javascript:ValidateTextlimit(this,100)" ReadOnly="true"></asp:TextBox>
                                                    
                                                </div>
                                            </div>
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
        </div>
            <!-- END PAGE CONTENT-->

               <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue" >
                        <div class="widget-title" >
                            <h4><i class="icon-reorder"></i>Issue Details</h4>
                            <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                            
                            </span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                 <asp:Panel ID="pnlApproval" runat="server" >
                                    <div class="row-fluid">
                                    <div class="span1"></div>
                                    <div class="span5">
                                        <div class="control-group">
                                            <label class="control-label">Issue Date
                                            
                                                <span class="Mandotary"> *</span></label>
                                           
                                                <div class="controls">
                                                    <div class="input-append">                                                       
                                                         <asp:TextBox ID="txtIssueDate" runat="server"  MaxLength="10" TabIndex="5" ></asp:TextBox>
                                                          <ajax:CalendarExtender ID="CalendarExtender1" runat="server" CssClass="cal_Theme1" TargetControlID="txtIssueDate" Format="dd/MM/yyyy"></ajax:CalendarExtender> 
                                                    </div>
                                                </div>
                                        </div>

                                        <div class="control-group">
                                            <label class="control-label">Reference No
                                            
                                                <span class="Mandotary"> *</span>
                                           </label>
                                            <div class="controls">
                                                <div class="input-append">                                                       
                                                    <asp:TextBox ID="txtPONo" runat="server" MaxLength="25" TabIndex="6" onkeypress="javascript:return RestrictSpace(event)" ></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="control-group">
                                            <label class="control-label">Reference Date
                                             
                                                <span class="Mandotary"> *</span></label>
                                           
                                                <div class="controls">
                                                    <div class="input-append">
                                                          <asp:TextBox ID="txtPODate" runat="server" MaxLength="10" TabIndex="7"></asp:TextBox> 
                                                           <ajax:CalendarExtender ID="CalendarExtender_txtPODate" runat="server" CssClass="cal_Theme1" TargetControlID="txtPODate" Format="dd/MM/yyyy" ></ajax:CalendarExtender>                                             
                                                    </div>
                                                </div>
                                        </div>

                                        <div class="control-group">
                                            <label class="control-label">Invoice Date
                                            
                                                <span class="Mandotary"> *</span></label>
                                           
                                                <div class="controls">
                                                    <div class="input-append">
                                                          <asp:TextBox ID="txtInvoiceDate" runat="server" MaxLength="10" TabIndex="9"></asp:TextBox> 
                                                           <ajax:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="cal_Theme1" TargetControlID="txtInvoiceDate" Format="dd/MM/yyyy" ></ajax:CalendarExtender>                                             
                                                    </div>
                                                </div>
                                        </div> 

                                    </div>
                                       
                                    <div class="span5">

                                        <div class="control-group">
                                            <label class="control-label">Invoice No
                                             
                                                <span class="Mandotary"> *</span></label>
                                           
                                            <div class="controls">
                                                <div class="input-append">                                                       
                                                    <asp:TextBox ID="txtInvoiceNo" runat="server" MaxLength="20" TabIndex="8" ></asp:TextBox>
                                                    <asp:TextBox ID="txtRepairMasterId" runat="server" MaxLength="20" TabIndex="8" Visible="false"></asp:TextBox>
                                                    
                                                </div>
                                            </div>
                                        </div>

                                        <div class="control-group">
                                            <label class="control-label">Manual Invoice NO
                                            
                                                <span class="Mandotary"> </span></label>
                                           
                                                <div class="controls">
                                                    <div class="input-append">
                                                          <asp:TextBox ID="txtManualInvoiceNo" runat="server" MaxLength="10" TabIndex="9"></asp:TextBox>                                          
                                                    </div>
                                                </div>
                                        </div>
                                        
                                        <div class="control-group">
                                            <label class="control-label">Select Old Reference NO</label>
                                            
                                                <div class="controls">
                                                    <div class="input-append">                                                       
                                                         <asp:TextBox ID="txtPonum" runat="server" MaxLength="10" TabIndex="12" ></asp:TextBox>
                                                          <asp:Button ID="cmdSearchPO" Text="S" class="btn btn-primary" runat="server" 
                                                           CausesValidation="false" TabIndex="13" onclick="cmdSearchPO_Click"  />
                                                    </div>
                                                </div>
                                        </div>
                                        
                                        <div class="control-group">
                                            <label class="control-label">Remarks
                                             
                                                <span class="Mandotary"> </span></label>
                                        
                                                <div class="controls">
                                                    <div class="input-append">
<%--                                                    <asp:TextBox ID="txtRemarks" AutoPostBack="true" runat="server" MaxLength="10" onclick="cmdSearchPO_Click" TextMode="MultiLine" TabIndex="9"></asp:TextBox>                                          --%>
                                                        <asp:TextBox ID="txtRemarks"  runat="server" MaxLength="10" TextMode="MultiLine" TabIndex="9"></asp:TextBox>                                          

                                                    </div>
                                                </div>
                                        </div>                                    
                                        
                                    </div>                                   
                                                                     
                                </div>
                                </asp:Panel>
                               <div class="space20"></div>
                                         <div class="space20"></div>
                                 <div  class="form-horizontal" align="center">

                                    <div class="span3"></div>
                                     <div class="span1">
                                        <asp:Button ID="cmdSave" runat="server" Text="Save"  OnClientClick="javascript:return ValidateMyForm()"  onchange = "javascript:preventMultipleSubmissions();"
                                        CssClass="btn btn-primary" onclick="cmdSave_Click" TabIndex="10" CausesValidation="false" />
                                      </div>
                                    <div class="span1">  
                                        <asp:Button ID="cmdReset" runat="server" Text="Reset" CausesValidation="false"
                                            CssClass="btn btn-primary" onclick="cmdReset_Click" TabIndex="11" /><br />
                                    </div>
                                      <div class="span7"></div>
                                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>                                            
                                    </div>
                            </div>
                                                     
                        </div>
                                
                        <div class="space20"></div>
                        <!-- END FORM-->                            
                    </div>
                 </div>
    <!-- END SAMPLE FORM PORTLET-->
            </div>
        </div>
            <!-- END PAGE CONTENT-->

                  <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid" >
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue" >
                        <div class="widget-title" >
                            <h4><i class="icon-reorder"></i>Selected Transformer</h4>
                            <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                            
                            </span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form" >
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <div class="row-fluid">
                                    <div class="span1"></div>
                                        <div id="selectedtc" style="visibility:hidden">
                                    <div class="span5">
                                        
                                        <div id="DTrCODE" runat="server" class="control-group">
                                            <label class="control-label">DTr Code</label>
                                             
                                                <div class="controls">
                                                    <div class="input-append">                                                       
                                                         <asp:TextBox ID="txtTcCode" runat="server" MaxLength="10" TabIndex="12" ></asp:TextBox>
                                                          <asp:Button ID="cmdSearchTC" Text="S" class="btn btn-primary" runat="server" 
                                                           CausesValidation="false"   onclick="cmdSearchTC_Click" TabIndex="13" />
                                                    </div>
                                                </div>
                                        </div>

                                    </div>
                                       
                                    <div class="span4">
                                         <div id="MAKE" runat="server" class="control-group">
                                            <label class="control-label">Make</label>                                            
                                           <div class="controls">
                                                <div class="input-append">                                                       
                                                    <asp:TextBox ID="txtMake" runat="server" MaxLength="50" TabIndex="14" ></asp:TextBox>
                                                      <asp:TextBox ID="txtSelectedTcId" runat="server" Visible="false" Width="20px" ></asp:TextBox>
                                                       <asp:Button ID="cmdLoad" Text="Load" class="btn btn-primary" runat="server" 
                                                        onclick="cmdLoad_Click" TabIndex="15" CausesValidation="false" 
                                                              />
                                                </div>
                                            </div>
                                        </div>                                   
                                        
                                    </div>                                   
                                         </div>                            
                                </div>
                              
                            </div>
                                                     
                             </div>
                                
                        <div class="space20"></div>
                        <!-- END FORM-->    
                        
                              <asp:GridView ID="grdFaultTC" ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found"
                                AutoGenerateColumns="false"  PageSize="10" DataKeyNames="TC_ID"
                                CssClass="table table-striped table-bordered table-advance table-hover"
                                runat="server" TabIndex="16" onrowcommand="grdFaultTC_RowCommand" >
                                <Columns>

                                 <asp:TemplateField AccessibleHeaderText="TC_ID" HeaderText="TC ID" Visible="false">                                
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblTCId" runat="server" Text='<%# Bind("TC_ID") %>'></asp:Label>
                                  
                                        </ItemTemplate>
                                   </asp:TemplateField>

                                   <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTr Code" >                                
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblTCCode" runat="server" Text='<%# Bind("TC_CODE") %>'></asp:Label>
                                  
                                        </ItemTemplate>
                                   </asp:TemplateField>

                                     <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC SlNo" Visible="false">                                
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblTCSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>'></asp:Label>
                                  
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    
                                    <asp:TemplateField AccessibleHeaderText="TM_NAME" HeaderText="Make Name">                                
                                        <ItemTemplate>                                       
                                            <asp:Label ID="lblMakeName" runat="server" Text='<%# Bind("TM_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="Capacity(in KVA)">
                                      
                                        <ItemTemplate>
                                            <asp:Label ID="lblCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    
                                    <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="Manf. Date">                                      
                                        <ItemTemplate>                                          
                                            <asp:Label ID="lblManfDate" runat="server" Text='<%# Bind("TC_MANF_DATE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   
                                    
                                    <asp:TemplateField AccessibleHeaderText="TC_PURCHASE_DATE" HeaderText="Purchase Date">                                      
                                        <ItemTemplate>
                                            <asp:Label ID="lblPurchaseDate" runat="server" Text='<%# Bind("TC_PURCHASE_DATE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField AccessibleHeaderText="TC_WARANTY_PERIOD" HeaderText="Guarantee " Visible="false">                                      
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarrenty" runat="server" Text='<%# Bind("TC_WARANTY_PERIOD") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField AccessibleHeaderText="TS_NAME" HeaderText="Supplier" Visible="false" >
                                       
                                        <ItemTemplate>
                                            <asp:Label ID="lblSupplier" runat="server" Text='<%# Bind("TS_NAME") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField AccessibleHeaderText="TC_GUARANTY_TYPE" HeaderText="Guaranty Type" Visible="false">
                                       
                                        <ItemTemplate>
                                            <asp:Label ID="lblGuarantyType" runat="server" Text='<%# Bind("TC_GUARANTY_TYPE") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                  
                                  <asp:TemplateField  HeaderText="Remove" >
                                    <ItemTemplate>
                                        <center>
                                            <asp:ImageButton  ID="imgBtnDelete" runat="server" Height="12px" ImageUrl="~/Styles/images/delete64x64.png" 
                                              CommandName="Remove"  Width="12px" OnClientClick="return ConfirmDelete();" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                    <ItemTemplate>
                                        <center>
                                     
                                            <asp:ImageButton  ID="imgBtnEdit" runat="server" Height="12px" ImageUrl="~/Styles/images/edit64x64.png" 
                                              CommandName="Submit"   Width="12px" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>

                               
                                </Columns>

                            </asp:GridView>
                              
                                            
                                    </div>                  
                    </div>
                 </div>

                  <div class="row-fluid" runat="server" id="dvComments" style="display:none">
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
                                                           
                                                            <asp:HiddenField ID="hdfRepairId" runat="server" />
                                                            <asp:TextBox ID="txtComment" runat="server" MaxLength="200" TabIndex="4"  TextMode="MultiLine" 
                                                              Width="550px" Height="125px" style="resize:none"  onkeyup="javascript:ValidateTextlimit(this,200)"></asp:TextBox>
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

                    <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue" >
                        <div class="widget-title" >
                            <h4><i class="icon-reorder"></i>GatePass</h4>
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
                        <label class="control-label">Vehicle No
                         
                           <span class="Mandotary"> *</span></label>
                      
                        <div class="controls">
                            <div class="input-append">
                                  <asp:TextBox ID="txtGpId" runat="server" Visible="false"></asp:TextBox>                   
                                <asp:TextBox ID="txtVehicleNo" runat="server" MaxLength="50"></asp:TextBox>
                                <asp:HiddenField ID="hdfInvoiceNo" runat="server" />
                                <asp:TextBox ID="txtActiontype" runat="server" MaxLength="50" Visible="false"></asp:TextBox>
                            </div>
                        </div>
                    </div>
         
                         <label class="control-label">Receipient Name
                         
                           <span class="Mandotary"> *</span></label>
                      
                        <div class="controls">
                            <div class="input-append">
                                 <asp:HiddenField ID="hdfWFOId" runat="server" />
                            <asp:HiddenField ID="hdfWFDataId" runat="server" />
                            <asp:HiddenField ID="hdfWFOAutoId" runat="server" />                      
                                <asp:TextBox ID="txtReciepient" runat="server" MaxLength="50"></asp:TextBox>
                                                                              
                            </div>
                        </div>
                    </div>

                        <label class="control-label">Challen Number
                       
                           <span class="Mandotary"> *</span></label>
                      
                        <div class="controls">
                            <div class="input-append">
                                                   
                                <asp:TextBox ID="txtChallen" runat="server" MaxLength="50"></asp:TextBox>
                            
                            </div>
                        </div>
                    </div>
                            <div class="space20"></div>
                                        
                                    <div  class="form-horizontal" align="center">

                                        <div class="span3"></div>
                                       
                                        <div class="span1">
                                        <asp:Button ID="cmdGatePass" runat="server" Text="Print GatePass" 
                                      CssClass="btn btn-primary" onclick="cmdGatePass_Click" Enabled="false"
                                                />
                                         </div>

                                          <div class="space20"></div>
         
         </div>



        
         
      </div>



    </div>

     
    </div>
    </div>
    </div>


    
    </div>
    <!-- END SAMPLE FORM PORTLET-->
            </div>

                       
            <!-- END PAGE CONTENT-->

      </div>         
</div>
</asp:Content>
