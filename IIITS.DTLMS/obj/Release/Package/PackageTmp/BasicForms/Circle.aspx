<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="Circle.aspx.cs" Inherits="IIITS.DTLMS.BasicForms.Circle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../Scripts/functions.js" type="text/javascript"></script>
   
     <script  type="text/javascript">

         //function AllowSpecialchar(evt) {
         //    evt = (evt) ? evt : event;
         //    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
         //   ((evt.which) ? evt.which : 0));
         //    //alert(charCode);
         //    if (charCode == 39) {
         //        return false;
         //    }

         //    else {
         //        return true;
         //    }
         //}
         function ValidateMyForm() {
             
             
             if (document.getElementById('<%= cmbZone.ClientID %>').value == "--Select--") {
                 alert('Select Zone.')
                 document.getElementById('<%= cmbZone.ClientID %>').focus()
                return false
             }



             if (document.getElementById('<%= txtCircleCode.ClientID %>').value.trim() == "0") {
                 alert('Circle Code Cannot be 0')
                 document.getElementById('<%= txtCircleCode.ClientID %>').focus()
                 return false
             }

             if (document.getElementById('<%= txtCircleCode.ClientID %>').value.trim() == "") {
                 alert('Please Enter Circle Code')
                 document.getElementById('<%= txtCircleCode.ClientID %>').focus()
                 return false
             }
             if (document.getElementById('<%= txtCircleName.ClientID %>').value.trim() == "") {
                 alert('Enter Valid Circle Name')
                 document.getElementById('<%= txtCircleName.ClientID %>').focus()
                 return false
             }

           
            // var CircleName = document.getElementById('<%= txtCircleName.ClientID %>').value;
            // var CircleNamecon = /^([a-zA-Z0-9])+(\s-\s)*([a-zA-Z0-9])+$/;
            // if (!CircleName.match(CircleNamecon)) {
               //  alert('Enter valid  Circle Name')
                // document.getElementById('<%= txtCircleName.ClientID %>').focus()
               // return false
           // }
             if (document.getElementById('<%= txtFullName.ClientID %>').value.trim() == "") {
                 alert('Please Enter Full Name')
                 document.getElementById('<%= txtFullName.ClientID %>').focus()
                 return false
             }
             if (document.getElementById('<%= txtMobile.ClientID %>').value.trim() == "") {
                 alert('Enter Mobile No')
                 document.getElementById('<%= txtMobile.ClientID %>').focus()
                 return false
             }
             if (document.getElementById('<%= txtEmailId.ClientID %>').value.trim() == "") {
                 alert('Enter Valid Mail Id')
                 document.getElementById('<%= txtEmailId.ClientID %>').focus()
                 return false
             }
         }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
               <div class="span8">
                   <!-- BEGIN THEME CUSTOMIZER-->
                 
                   <!-- END THEME CUSTOMIZER-->
                  <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                   <h3 class="page-title" runat="server" id="Create">
            Create Circle
                   </h3>
                   <h3 class="page-title" runat="server" id="Update">
            Update Circle
                   </h3>
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
               <div style="float:right;margin-top:20px;margin-right:12px" >
                   <%--   <asp:Button ID="cmdClose" runat="server" Text="Close" 
                                    CssClass="btn btn-primary" />--%></div>
                    <div style="float:right;margin-top:20px;margin-right:12px" >
                      <asp:Button ID="cmdClose" runat="server" Text="Circle View" 
                                      OnClientClick="javascript:window.location.href='CircleView.aspx'; return false;"
                            CssClass="btn btn-primary" /></div>          
            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue" >
                        <div class="widget-title" >
                            <h4 id="CreateCircle" runat="server"><i class="icon-reorder"></i>Create Circle</h4>
                            <h4 id="UpdateCircle" runat="server"><i class="icon-reorder"></i>Update Circle</h4>
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
                                                   <label class="control-label">Zone Name<span class="Mandotary"> *</span></label>
                                                   <div class="controls">   
                                                       <div class="input-append">
                                                            <asp:DropDownList ID="cmbZone" runat="server"  AutoPostBack="true" 
                                                                TabIndex="1" onselectedindexchanged="cmbZone_SelectedIndexChanged"  >                                   
                                                            </asp:DropDownList>
                                                           <asp:TextBox ID="txtZoneid" runat="server" Width="20px" Visible="false"></asp:TextBox>
                                                       </div>
                                                    </div>
                                               </div>                      
   
                 <div class="control-group">
                        <label class="control-label">Circle Code <span class="Mandotary"> *</span></label>
                 
                        <div class="controls">
                            <div class="input-append">
                                <asp:TextBox ID="txtCircleCode" runat="server" onkeypress="javascript:return OnlyNumber(event);" MaxLength="1"></asp:TextBox>
                                <asp:TextBox ID="txtCrID" runat="server" Visible="false"></asp:TextBox>
                            </div>
                        </div>
                    </div>
     
                    <div class="control-group">
                        <label class="control-label">Circle Name <span class="Mandotary"> *</span></label>
                        
                        <div class="controls">
                            <div class="input-append">
                                                       
                                <asp:TextBox ID="txtCircleName" runat="server"   onkeypress="javascript: return onlyAlphabets(event,this);" MaxLength="30" ></asp:TextBox>
                            </div>
                        </div>
                    </div>


                        <div class="control-group">
                        <label class="control-label">Circle Head<span class="Mandotary"> *</span></label>
                        <div class="controls">
                            <div class="input-append">
                                                       
                                <asp:TextBox ID="txtFullName" runat="server" MaxLength="40" onkeypress="javascript: return onlyAlphabets(event,this);" ></asp:TextBox>
                            </div>
                        </div>
                    </div>

                        </div>


               <div class="span5">
   
                 

               <div class="control-group">
                        <label class="control-label">Mobile<span class="Mandotary"> *</span></label>
                        <div class="controls">
                            <div class="input-append">
                                    <asp:TextBox ID="txtMobile" runat="server"  onkeypress="javascript:return OnlyNumber(event);" MaxLength="10" ></asp:TextBox>
                                                       
                            </div>
                        </div>
                    </div>


                      <div class="control-group">
                        <label class="control-label">Phone</label>
                        <div class="controls">
                            <div class="input-append">
                                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="11" onkeypress="javascript:return OnlyNumber(event);" ></asp:TextBox>
                                                       
                            </div>
                        </div>
                    </div>


               <div class="control-group">
                        <label class="control-label">Email Id<span class="Mandotary"> *</span></label>
                        <div class="controls">
                            <div class="input-append">
                                    <asp:TextBox ID="txtEmailId" runat="server" MaxLength="50" onkeypress="javascript: return onlyAlphabetsemail(event,this);javascript:return validateEmail(txtEmailId);"></asp:TextBox>
                                                       
                            </div>
                        </div>
                    </div>

                <div class="control-group">
                        <label class="control-label">Address</label>
                        <div class="controls">
                            <div class="input-append">
                                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="200" TextMode="MultiLine" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>                   
                            </div>
                        </div>
                    </div>
                                                       
            </div>



                                <div class="span1"></div>
                                        </div>
                                        <div class="space20"></div>
                                        
                                    <div  class="form-horizontal" align="center">
                                        <div class="span3"></div>
                                        
                                        <div class="span1">
                                    <asp:Button ID="cmdSave" runat="server" Text="Save"  
                                       OnClientClick="javascript:return ValidateMyForm()" CssClass="btn btn-primary" onclick="cmdSave_Click" 
                                                />
                                         </div>
                                      <%-- <div class="span1"></div>--%>
                                     <div class="span1">  
                                         <asp:Button ID="cmdReset" runat="server" Text="Reset" 
                                             CssClass="btn btn-primary" onclick="cmdReset_Click" /><br />
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
         </div>



</asp:Content>
