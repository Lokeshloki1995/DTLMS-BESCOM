﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="OmSecMast.aspx.cs" Inherits="IIITS.DTLMS.BasicForms.OmSecMast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>

    <script type="text/javascript">
        window.onload = function () {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=lblErrormsg.ClientID %>").style.display = "none";
         }, seconds * 1000);
     };
    </script>

    <script type="text/javascript">
        function ValidateMyForm() {
            if (document.getElementById('<%= cmbCircle.ClientID %>').value == "--Select--") {
                 alert('Select Circle Name')
                 document.getElementById('<%= cmbCircle.ClientID %>').focus()
                 return false
             }

             if (document.getElementById('<%= cmbDivision.ClientID %>').value.trim() == "--Select--") {
                 alert('Select Division Name')
                 document.getElementById('<%= cmbDivision.ClientID %>').focus()
                 return false
             }
             if (document.getElementById('<%= cmbSubDiv.ClientID %>').value.trim() == "--Select--") {
                 alert('Select sub Division Name')
                 document.getElementById('<%= cmbSubDiv.ClientID %>').focus()
                 return false
             }

             if (document.getElementById('<%= txtOmSecCode.ClientID %>').value.trim() == "") {
                 alert('Please Enter OM Section Code')
                 document.getElementById('<%= txtOmSecCode.ClientID %>').focus()
                 return false
             }

             if (document.getElementById('<%= txtOmSecName.ClientID %>').value.trim() == "") {
                 alert('Enter  OM Unit Name')
                 document.getElementById('<%= txtOmSecName.ClientID %>').focus()
                 return false
             }



             if (document.getElementById('<%= txtSecHead.ClientID %>').value.trim() == "") {
                 alert('Enter Section Head Name')
                 document.getElementById('<%= txtSecHead.ClientID %>').focus()
                 return false
             }

            // if (document.getElementById('<%= txtPhoneNo.ClientID %>').value.trim() == "") {
               //  alert('Enter Valid Mobile Number')
                // document.getElementById('<%= txtPhoneNo.ClientID %>').focus()
                // return false
            // }
         }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>

        <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
                <div class="span8">
                    <!-- BEGIN THEME CUSTOMIZER-->

                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title" runat="server" id="Create">Create OMU
                    </h3>
                    <h3 class="page-title" runat="server" id="Update">Update OMU
                    </h3>
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
                </div>
                <div style="float: right; margin-right: 10px; margin-top: 20px" class="span2">
                    <asp:Button ID="cmdOmSectionView" class="btn btn-primary" Text="OMU View" OnClientClick="javascript:window.location.href='OmSecView.aspx'; return false;" runat="server" />
                </div>
            </div>
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
                    <h4><i class="icon-reorder"></i>OMU  Entry</h4>
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
                                        <label class="control-label">Circle<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:DropDownList ID="cmbCircle" AutoPostBack="true" runat="server"
                                                    OnSelectedIndexChanged="cmbCircle_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="control-group">
                                        <label class="control-label">Division<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:DropDownList ID="cmbDivision" AutoPostBack="true" runat="server"
                                                    OnSelectedIndexChanged="cmbDivision_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="control-group">
                                        <label class="control-label">SubDiv Code<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:DropDownList ID="cmbSubDiv" AutoPostBack="true" runat="server"
                                                    OnSelectedIndexChanged="cmbSubDiv_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="control-group">
                                        <label class="control-label">OMU Code<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">

                                                <asp:TextBox ID="txtOmSecCode" runat="server" MaxLength="4"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>


                                </div>
                                <div class="span5">

                                    <div class="control-group">
                                        <label class="control-label">OMU Name<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">

                                                <asp:TextBox ID="txtOmSecName" runat="server" MaxLength="25" ></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Sec Head Emp<span class="Mandotary"> *</span></label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:TextBox ID="txtSecHead" runat="server" MaxLength="25"  ></asp:TextBox>
                                                <asp:TextBox ID="txtOMId" runat="server" Visible="false" Width="20px"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Mobile No</label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:TextBox ID="txtPhoneNo" runat="server" MaxLength="10" onkeypress="javascript:return OnlyNumber(event);"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                     <div class="control-group">
                                        <label class="control-label">Address</label>
                                        <div class="controls">
                                            <div class="input-append">
                                                <asp:TextBox ID="txtaddress" runat="server" MaxLength="500" Height="60px"
                                                        TextMode="MultiLine" Style="resize: none"
                                                        onkeyup="return ValidateTextlimit(this,500);" ></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>


                                </div>
                                <div class="span1"></div>
                            </div>
                            <div class="space20"></div>

                            <div class="form-horizontal" align="center">

                                <div class="span3"></div>
                                <div class="span1">
                                    <asp:Button ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click"
                                        OnClientClick="javascript:return ValidateMyForm()" CssClass="btn btn-primary" />
                                </div>
                                <%-- <div class="span1"></div>--%>
                                <div class="span1">
                                    <asp:Button ID="cmdReset" runat="server" Text="Reset"
                                        OnClick="cmdReset_Click" CssClass="btn btn-primary" /><br />
                                </div>
                                <div class="span7"></div>
                                <asp:Label ID="lblErrormsg" runat="server" ForeColor="Red"></asp:Label>

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
         
      </div>
</asp:Content>
