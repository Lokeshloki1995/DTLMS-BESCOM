﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="UserCreate.aspx.cs" Inherits="IIITS.DTLMS.MasterForms.UserCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>
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
    <script type="text/javascript">
        function ValidateMyForm() {

            if (document.getElementById('<%= txtFullName.ClientID %>').value.trim() == "") {
                alert('Enter Full Name')
                document.getElementById('<%= txtFullName.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtLoginName.ClientID %>').value.trim() == "") {
                alert('Enter Login Name')
                document.getElementById('<%= txtLoginName.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtEmailId.ClientID %>').value.trim() == "") {
                alert('Enter EmailId')
                document.getElementById('<%= txtEmailId.ClientID %>').focus()
                return false
            }

            var EmailId = document.getElementById('<%= txtEmailId.ClientID %>').value;
            var EmailIdcon = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            if (!EmailId.match(EmailIdcon)) {
                alert('Enter valid  EmailId')
                document.getElementById('<%= txtEmailId.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtMobile.ClientID %>').value.trim() == "") {
                alert('Enter MobileNo.')
                document.getElementById('<%= txtMobile.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbRole.ClientID %>').value == "-Select-") {
                alert('Select Role.')
                document.getElementById('<%= cmbRole.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= cmbDesignation.ClientID %>').value == "-Select-") {
                alert('Select Designation')
                document.getElementById('<%= cmbDesignation.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtPassword.ClientID %>').value.trim() == "") {
                alert('Enter Password.')
                document.getElementById('<%=txtPassword.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtPassword.ClientID %>').value.trim() != "") {
                var pass = document.getElementById('<%= txtPassword.ClientID %>').value
                if (!pass.match(/^(?=.{8,})(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[#!*()$%^&+-={}@@]).*$/)) {
                    alert("Password Length Should 8 Character and  contains at least 1 Capital Letter or 1 Small Letter,1 Digit, 1 Special Character")
                    document.getElementById('<%=txtPassword.ClientID %>').focus()
                    return false;
                }

            }

            if (document.getElementById('<%= txtAddress.ClientID %>').value.trim() == "") {
                alert('Enter Address.')
                document.getElementById('<%=txtAddress.ClientID %>').focus()
                return false
            }
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

                    <h3 class="page-title" runat="server" id="Create">Create User                                    
                    </h3>
                    <h3 class="page-title" runat="server" id="Update">Update User                                    
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
                    <!-- END PAGE TITLE & BREADCRUMB-->
                </div>
                <div style="float: right; margin-top: 20px; margin-right: 12px">
                    <asp:Button ID="Button1" runat="server" Text="User View"
                        OnClientClick="javascript:window.location.href='UserView.aspx'; return false;"
                        CssClass="btn btn-primary" />
                </div>

            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4 id="CreateUser" runat="server"><i class="icon-reorder"></i>Create User</h4>
                            <h4 id="UpdateUser" runat="server"><i class="icon-reorder"></i>Update User</h4>
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
                                                <label class="control-label">Full Name<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">

                                                        <asp:TextBox ID="txtFullName" runat="server" AutoComplete="off" MaxLength="100"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Login Name<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtLoginName" runat="server" AutoComplete="off"
                                                            MaxLength="100"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Role<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbRole" runat="server" AutoPostBack="true"
                                                            OnSelectedIndexChanged="cmbRole_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Designation<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbDesignation" runat="server">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <%--<div class="control-group">
                        <label class="control-label">Office Code<span class="Mandotary"> *</span></label>
                        <div class="controls">
                            <div class="input-append">
                                                       
                                <asp:TextBox ID="txtOffCode"  runat="server" onkeypress="javascript:return OnlyNumber(event);" MaxLength="5"></asp:TextBox>
                                  <asp:Button ID="btnSearch" runat="server" Text="S"  
                                       CssClass="btn btn-primary" onclick="btnSearch_Click" />
                               
                                 <asp:TextBox ID="txtuserID"  runat="server" Width="20px" Visible="false" ></asp:TextBox>
                                  <asp:TextBox ID="txtSignImagePath"  runat="server" Width="20px" Visible="false" ></asp:TextBox>
                            </div>
                        </div>
                    </div>--%>
                                            <div class="control-group">
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtSignImagePath" runat="server" Width="20px" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtuserID" runat="server" Width="20px" Visible="false"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <%-- <div class="control-group">
                                                <label class="control-label">Office Name</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtOfficeName" runat="server" MaxLength="100" ReadOnly="true"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>--%>

                                            <div class="control-group">
                                                <label class="control-label">Email Id<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtEmailId" AutoComplete="off" runat="server" MaxLength="50" onkeypress="javascript:return validateEmail(txtEmailId);"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Mobile<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtMobile" AutoComplete="off" runat="server" onkeypress="javascript:return OnlyNumber(event);" MaxLength="10"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Phone</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtPhone" AutoComplete="off" runat="server" MaxLength="11" onkeypress="javascript:return OnlyNumber(this,event);"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="span5">



                                            <div id="divcircle" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">
                                                        Zone
                                                    </label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbZone" runat="server" AutoPostBack="true" TabIndex="1"
                                                                OnSelectedIndexChanged="cmbZone_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">
                                                        Circle
                                                    </label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbCircle" runat="server" AutoPostBack="true" TabIndex="1"
                                                                OnSelectedIndexChanged="cmbCircle_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">
                                                        Division</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbDiv" runat="server" AutoPostBack="true" TabIndex="1" OnSelectedIndexChanged="cmbDiv_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">
                                                        Sub Division</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbSubDiv" runat="server" AutoPostBack="true" TabIndex="1"
                                                                OnSelectedIndexChanged="cmbSubDiv_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>



                                                <div class="control-group">
                                                    <label class="control-label">
                                                        O & M Section</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbOMSection" runat="server" TabIndex="1">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>


                                            <div id="divstore" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">
                                                        Store</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbStore" runat="server" AutoPostBack="true" TabIndex="1">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <br />
                                            <div class="control-group">
                                                <label class="control-label" id="lblpwd" runat="server">Password<span class="Mandotary"> *</span></label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtPassword" runat="server" MaxLength="50" TextMode="Password" onkeypress="return AvoidSpace(event);"></asp:TextBox>





                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                            ControlToValidate="txtPassword" ErrorMessage="Enter new password"
                                                            ValidationGroup="reg" ForeColor="Red"></asp:RequiredFieldValidator>

                                                    </div>
                                                </div>
                                            </div>
                                            <div style="padding-left: 180px" class="space1">
                                                <div id="cnttxt" runat="server">
                                                    <label style="font-size: small">

                                                        <span class="Mandotary">*Password should be greater than or equal to 8 digit (It should Contain at least 1 Capital Letter or 1 Small Letter,1 Digit, 1 Special Character)</span></label>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Address<span class="Mandotary"> *</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtAddress" runat="server" MaxLength="200" onkeyup="return ValidateTextlimit(this,200);" Style="resize: none" TextMode="MultiLine"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group" style="display: none">
                                                <label class="control-label">Sign Copy</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:FileUpload ID="fupSign" runat="server" AllowMultiple="False" />
                                                    </div>
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

    </div>
</asp:Content>
