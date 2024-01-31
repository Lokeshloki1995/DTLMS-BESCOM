<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="FailureEntry.aspx.cs" Inherits="IIITS.DTLMS.DTCFailure.FailureEntry" %>


<%@ Register Src="/ApprovalHistoryView.ascx" TagName="ApprovalHistoryView" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>



    <script type="text/javascript">

        function ValidateMyForm() {

            if (document.getElementById('<%= txtDTCCode.ClientID %>').value.trim() == "") {
                alert('Select Valid Transformer Centre Code')
                document.getElementById('<%= txtDTCCode.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= cmbPurpose.ClientID %>').value.trim() == "--Select--") {
                alert("Please Select Load Type");
                document.getElementById('<%= cmbPurpose.ClientID %>').focus();
                return false;
            }
            if (document.getElementById('<%= cmbFailureType.ClientID %>').value.trim() == "--Select--") {
                alert('Select Failure Type')
                document.getElementById('<%= cmbFailureType.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= txtReason.ClientID %>').value.trim() == "") {
                alert('Enter the Failure reason')
                document.getElementById('<%= txtReason.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= txtFailedDate.ClientID %>').value.trim() == "") {
                alert('Select the Failure date')
                document.getElementById('<%= txtFailedDate.ClientID %>').focus()
                return false
            }
            else {

                var FromdateInput = document.getElementById('<%= txtFailedDate.ClientID %>').value;
                var goodDate = /^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/;

                if (!FromdateInput.match(goodDate)) {
                    alert("Please enter valid  Failure Date date format");
                    return false;
                }

            }

            if (document.getElementById('<%= cmbHtBusing.ClientID %>').value.trim() == "--Select--") {
                alert('Select HT Busing')
                document.getElementById('<%= cmbHtBusing.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= cmbLtBusing.ClientID %>').value.trim() == "--Select--") {
                alert('Select LT Busing')
                document.getElementById('<%= cmbLtBusing.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= cmbHtBusingRod.ClientID %>').value.trim() == "--Select--") {
                alert('Select H.T.Bushing Rod & Nut')
                document.getElementById('<%= cmbHtBusingRod.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbLtBusingRod.ClientID %>').value.trim() == "--Select--") {
                alert('Select L.T.Bushing Rod & Nut')
                document.getElementById('<%= cmbLtBusingRod.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbDrainValve.ClientID %>').value.trim() == "--Select--") {
                alert('Select Drain Valve')
                document.getElementById('<%= cmbDrainValve.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbOilLevel.ClientID %>').value.trim() == "--Select--") {
                alert('Select Oil Level Gauge')
                document.getElementById('<%= cmbOilLevel.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbTankCondition.ClientID %>').value.trim() == "--Select--") {
                alert('Select Condition Of Tank')
                document.getElementById('<%= cmbTankCondition.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbExplosion.ClientID %>').value.trim() == "--Select--") {
                alert('Select Explosion vent Valve')
                document.getElementById('<%= cmbExplosion.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%= cmbBreather.ClientID %>').value.trim() == "--Select--") {
                alert('Select Breather')
                document.getElementById('<%= cmbBreather.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= cmbRepairer.ClientID %>').value.trim() == "--Select--") {
                alert('Select Repairer')
                document.getElementById('<%= cmbRepairer.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtConnectionDate.ClientID %>').value.trim() == "") {
                alert('Enter Commission Date .NOTE : 1) Goto Masters -> Transformer Centre Master 2) Search DTC Code 3) Click on EDIT button 4) Enter DTC Commission Date')
                document.getElementById('<%= txtConnectionDate.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtTcCode.ClientID %>').value.trim() == "" || document.getElementById('<%= txtTcCode.ClientID %>').value.trim() == "0") {
                alert('This Transformer Centre is Currently having No TC, please contact the DTLMS Team')
                document.getElementById('<%= txtTcCode.ClientID %>').focus()
                return false
            }

            if (document.getElementById('<%= txtrate.ClientID %>').value.trim() == "") {
                alert('Select Rating .NOTE : 1) Click On View Dtr Details link 2) Select Rating And Click On Update.')
                document.getElementById('<%= txtrate.ClientID %>').focus()
                            return false
                        }

                        var FromdateInput = document.getElementById('<%= txtFailedDate.ClientID %>').value;
            var goodDate = /^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/;

            if (!FromdateInput.match(goodDate)) {
                alert("Please enter valid  Failed date");
                document.getElementById('<%= txtFailedDate.ClientID %>').focus()
                return false;
            }


            var FromdateInput = document.getElementById('<%= txtDTrCommDate.ClientID %>').value;
            var goodDate = /^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/;

            if (!FromdateInput.match(goodDate)) {
                alert("Please enter valid DTR Commission date");
                document.getElementById('<%= txtDTrCommDate.ClientID %>').focus()
                return false;
            }

            var FromdateInput = document.getElementById('<%= txtConnectionDate.ClientID %>').value;
            var goodDate = /^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/;

            if (!FromdateInput.match(goodDate)) {
                alert("Please enter valid Transformer Centre Commission date");
                document.getElementById('<%= txtConnectionDate.ClientID %>').focus()
                return false;
            }

            var FromdateInput = document.getElementById('<%= txtManfDate.ClientID %>').value;
            var goodDate = /^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/;

            if (!FromdateInput.match(goodDate)) {
                alert("Please enter valid Manf date");
                document.getElementById('<%= txtManfDate.ClientID %>').focus()
                return false;
            }

            if (document.getElementById('<%= txtCustName.ClientID %>').value.trim() == "") {
                alert("Please Enter Customer Name");
                document.getElementById('<%=txtCustName.ClientID%>').focus();
                return false;
            }

            if (document.getElementById('<%= txtQuantityOfOil.ClientID %>').value.trim() == "") {
                alert("Please Enter Quantity Of Oil");
                document.getElementById('<%=txtQuantityOfOil.ClientID%>').focus();
                return false;
            }

            if (document.getElementById('<%= cmbOilType.ClientID %>').value.trim() == "--Select--") {
                alert("Please Select Oil Type");
                document.getElementById('<%= cmbOilType.ClientID %>').focus();
                return false;
            }

            if (document.getElementById('<%= cmbDTCType.ClientID %>').value.trim() == "--Select--") {
                alert("Please Select Type of DTC");
                document.getElementById('<%= cmbDTCType.ClientID %>').focus();
                return false;
            }

            if (document.getElementById('<%= cmbModem.ClientID %>').value.trim() == "--Select--") {
                alert("Please Select Modem");
                document.getElementById('<%= cmbModem.ClientID %>').focus();
                return false;
            }

            if (document.getElementById('<%=txtCustNo.ClientID%>').value.trim() == "") {
                alert("Please Enter Customer Mobile NO");
                document.getElementById('<%= txtCustNo.ClientID %>').focus();
                return false;
            }
            if (document.getElementById('<%= cmbReplaceEntry.ClientID %>').value.trim() == "--Select--") {
                alert('Select Alternate Replacement')
                document.getElementById('<%= cmbReplaceEntry.ClientID %>').focus()
                return false
            }
            if (document.getElementById('<%=cmbGuarenteeType.ClientID %>').value.trim() == "WGP" && cmdSave.value == "Save") {
                return confirm("Are You Sure You want to continue for WGP Failure....!!!!");
            }
        }

        $(document).keydown(function (e) {

            var value = $('#txtdocket').val().length;

            if (e.keyCode == 8 && value < 2)
                e.preventDefault();
        });

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
        function onlynumformobno(e, t) {
            debugger;
            var code = ('charCode' in e) ? e.charCode : e.keyCode;
            if ( !(code > 47 && code < 58) ) { 
                e.preventDefault();
            }
        }
    </script>

    <style type="text/css">
        .auto-style1 {
            left: -4px;
            top: -5px;
        }

        .img1 {
            position: fixed;
            top: 50%;
            left: 50%;
            margin-top: -50px;
            margin-left: -100px;
            border-radius: 250px;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <div>

        <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
                <div class="span8">
                    <!-- BEGIN THEME CUSTOMIZER-->

                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title">Declare Failure
                    </h3>
                    <div class="span1">
                        <a href="#" data-toggle="modal" data-target="#myModal" title="Click For Help"><i class="fa fa-exclamation-circle" style="font-size: 36px"></i></a>
                    </div>
                    <div class="span2">
                        <asp:RadioButton ID="rdbFail" runat="server" Text="Failure"
                            CssClass="radio" GroupName="a" AutoPostBack="true" Checked="true"
                            OnCheckedChanged="rdbFailEnhance_CheckedChanged" />
                    </div>
                    <div>
                        <asp:RadioButton ID="rdbFailEnhance" runat="server" Text="Failure with Enhancement"
                            CssClass="radio" GroupName="a" AutoPostBack="true"
                            OnCheckedChanged="rdbFailEnhance_CheckedChanged" Width="200px" />
                    </div>
                    <div class="span7">
                    </div>


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
                <div style="float: right; margin-top: 6%; margin-right: 12px">
                    <asp:Button ID="cmdClose" runat="server" Text="Close"
                        CssClass="btn btn-primary" OnClick="cmdClose_Click" />
                </div>
                <br />
                <!-- Codes by HTMLcodes.ws -->
                <marquee behavior="scroll" direction="left" onmouseover="this.stop();" onmouseout="this.start();"><span style="color:red">Please Check The DTC and DTR Code Before Decalring The Failure</span></marquee>

            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Declare Failure</h4>
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
                                                <label class="control-label">Transformer Centre Code <span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:HiddenField ID="hdfDTCcode" runat="server" />

                                                        <asp:TextBox ID="txtDTCCode" runat="server" MaxLength="6"></asp:TextBox>
                                                        <asp:Button ID="cmdSearch" Text="S" class="btn btn-primary" runat="server"
                                                            OnClick="cmdSearch_Click" /><br />
                                                        <asp:LinkButton ID="lnkDTCDetails" runat="server"
                                                            Style="font-size: 12px; color: Blue" OnClick="lnkDTCDetails_Click">View Transformer Centre Details</asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>



                                            <div class="control-group">
                                                <label class="control-label">Transformer Centre Name <span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:HiddenField ID="hdfRejectApproveRef" runat="server" />
                                                        <asp:HiddenField ID="hdfWFOId" runat="server" />
                                                        <asp:HiddenField ID="hdfWFDataId" runat="server" />
                                                        <asp:HiddenField ID="hdfApproveStatus" runat="server" />
                                                        <asp:TextBox ID="txtFailureOfficCode" runat="server" MaxLength="100" Visible="false"></asp:TextBox>
                                                        <asp:TextBox ID="txtTCId" runat="server" MaxLength="100" Visible="false" Width="20px"></asp:TextBox>
                                                        <asp:TextBox ID="txtDTCName" runat="server" MaxLength="100" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Connected KW  </label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtLoadKW" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                                        <asp:TextBox ID="txtActiontype" runat="server" MaxLength="50" Width="20px" Visible="false"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Connected HP </label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:HiddenField ID="hdfCrBy" runat="server" />
                                                        <asp:TextBox ID="txtLoadHP" runat="server" ReadOnly="true" onkeypress="return OnlyNumber(event)" MaxLength="10"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Transformer Centre Commission Date <span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtConnectionDate" runat="server" MaxLength="11" onkeypress="javascript:return AllowNumber(event);" CssClass="auto-style1"></asp:TextBox>
                                                        <ajax:CalendarExtender ID="txtConnectionDate1" runat="server" CssClass="cal_Theme1" TargetControlID="txtConnectionDate" Format="dd/MM/yyyy"></ajax:CalendarExtender>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Transformer Commission Date <span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtDTrCommDate" runat="server" MaxLength="11"></asp:TextBox>
                                                        <ajax:CalendarExtender ID="CalendarExtender1" runat="server" CssClass="cal_Theme1"
                                                            TargetControlID="txtDTrCommDate" Format="dd/MM/yyyy">
                                                        </ajax:CalendarExtender>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Capacity(in KVA) </label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtCapacity" runat="server" MaxLength="15" ReadOnly="true"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfWFOAutoId" runat="server" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Section</label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtLocation" runat="server" MaxLength="10" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <%--<label class="control-label">Purpose Of Usage</label>--%>
                                                <label class="control-label">Load Type</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbPurpose" runat="server">
                                                        </asp:DropDownList>
                                                        <%--<asp:TextBox ID="txtPurpose" runat="server" MaxLength="20"></asp:TextBox>--%>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="span5">

                                            <div class="control-group">
                                                <label class="control-label">DTr Code </label>

                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtTcCode" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                                        <br />
                                                        <asp:LinkButton ID="lnkDTrDetails" runat="server"
                                                            Style="font-size: 12px; color: Blue" OnClick="lnkDTrDetails_Click">View DTr Details</asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>


                                            <div class="control-group">
                                                <label class="control-label">DTr Make </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtDtcId" runat="server" MaxLength="10" Visible="false" Width="20px"></asp:TextBox>
                                                        <asp:TextBox ID="txtFailurId" runat="server" MaxLength="10" Visible="false" Width="20px"></asp:TextBox>
                                                        <asp:TextBox ID="txtTCMake" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">DTr Serial Number </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtTCSlno" runat="server" MaxLength="10" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Manf. Date</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtManfDate" runat="server" MaxLength="11" ReadOnly="true" CssClass="auto-style1"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group" style="display: none">
                                                <label class="control-label">Guarantee Type </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbGuarenteeType" runat="server" Enabled="false">
                                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                                            <asp:ListItem Value="AGP" Text="AGP"></asp:ListItem>
                                                            <asp:ListItem Value="WRGP" Text="WRGP"></asp:ListItem>
                                                            <asp:ListItem Value="WGP" Text="WGP"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <asp:HiddenField ID="hdfGuarenteeSource" runat="server" />
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Last Repaired By </label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtLastRepairer" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Last Repaired Date</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtLastRepairDate" runat="server" MaxLength="11" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Rating <span class="Mandotary">*</span></label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtrate" runat="server" MaxLength="11" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Condition Of TC</label>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:TextBox ID="txtConditionOfTC" runat="server" MaxLength="11" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <asp:Label ID="lblEnCap" CssClass="control-label" runat="server" Text="Enhancment Capacity <span class='Mandotary'> *</span>" Visible="false"></asp:Label>
                                                <%--<label class="control-label" style="visibility:hidden">Enhancment Capacity</label>--%>
                                                <div class="controls">
                                                    <div class="input-append">
                                                        <asp:DropDownList ID="cmbEnhanceCapacity" runat="server" Enabled="false" Visible="false">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>



                                        </div>

                                    </div>

                                    <div class="span1"></div>
                                </div>

                            </div>
                        </div>

                        <!-- END FORM-->

                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4>Failure Entry Details</h4>
                            <span class="tools">
                                <a href="javascript:;" class="icon-chevron-down"></a>
                                <a href="javascript:;" class="icon-remove"></a>
                            </span>
                        </div>
                        <div class="widget-body">
                            <div class="widget-body form">
                                <!-- BEGIN FORM-->
                                <div class="form-horizontal">
                                    <asp:Panel ID="pnlApproval" runat="server">

                                        <div class="row-fluid">
                                            <div class="span1"></div>

                                            <div class="span5">

                                                <div class="control-group">
                                                    <label class="control-label">Failure Type<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbFailureType" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Alternate Replacement<span class="Mandotary">*</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbReplaceEntry" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Reason  <span class="Mandotary">*</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtReason" runat="server" MaxLength="10" TextMode="MultiLine"
                                                              autocomplete="off"  Style="resize: none" onkeyup="return ValidateTextlimit(this,100);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Failed Date <span class="Mandotary">*</span> </label>

                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtFailedDate" runat="server" autocomplete="off" MaxLength="10"></asp:TextBox>
                                                            <ajax:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="cal_Theme1"
                                                                TargetControlID="txtFailedDate" Format="dd/MM/yyyy">
                                                            </ajax:CalendarExtender>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Transformer Centre Reading </label>

                                                    <div class="controls">
                                                        <div class="input-append">

                                                            <asp:TextBox ID="txtDTCRead" runat="server" MaxLength="10" autocomplete="off"
                                                                onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">H.T.Bushing<span class="Mandotary"> *</span> </label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbHtBusing" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Good"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Bad"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">L.T.Bushing <span class="Mandotary">*</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbLtBusing" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Good"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Bad"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Drain Valve<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbDrainValve" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="No"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">H.T.Bushing Rod & Nut <span class="Mandotary">*</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbHtBusingRod" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Good"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Worn Out"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">L.T.Bushing Rod & Nut <span class="Mandotary">*</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbLtBusingRod" runat="server">

                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Good"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Worn Out"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Oil Level Gauge<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbOilLevel" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="No"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="span5">

                                                <div class="control-group">
                                                    <label class="control-label">Quantity Of Oil(as per Tank Capacity)<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtQuantityOfOil" runat="server" MaxLength="6" onpaste="return false" autocomplete="off"
                                                                onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Type of Oil<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbOilType" runat="server"></asp:DropDownList>

                                                        </div>
                                                        <p style="color: red;">In case of failed DTr oil type is ESTER kindly contact DTLMS support.</p>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Type of DTC<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbDTCType" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Modem<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbModem" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="NO"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Condition Of Tank<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbTankCondition" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Good"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Bad"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Explosion Vent Valve<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbExplosion" runat="server">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="No"></asp:ListItem>
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Breather<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbBreather" runat="server" AutoPostBack="true"
                                                                OnSelectedIndexChanged="cmbBreather_SelectedIndexChanged">
                                                                <asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="No"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Repairer<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbRepairer" runat="server">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">PGRS Docket NO<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtdocket1" autocomplete="off" runat="server" minlenth="10" Text="F" MaxLength="15" onkeypress="javascript: return onlyAlphabets(event,this);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">PGRS Docket Date<span class="Mandotary"> *</span> </label>

                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtdocketdate" runat="server" MaxLength="10" AutoComplete="Off"></asp:TextBox>
                                                            <ajax:CalendarExtender ID="CalendarExtender3" runat="server" CssClass="cal_Theme1"
                                                                TargetControlID="txtdocketdate" Format="dd/MM/yyyy">
                                                            </ajax:CalendarExtender>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Customer Name<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtCustName" runat="server" MaxLength="100" autocomplete="off" CssClass="auto-style1"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Customer Mobile NO<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtCustNo" runat="server" MaxLength="10" autocomplete="off" onkeypress="javascript:return onlynumformobno(this,event);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Meggar value(MΩ)</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:TextBox ID="txtMeggerValue" runat="server" autocomplete="off" onkeypress="javascript:return AllowNumber(this,event);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group" id="silicagel" runat="server">
                                                    <label class="control-label">Silica Gel Condition<span class="Mandotary"> *</span></label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbSilica" runat="server">
                                                                <%--<asp:ListItem Text="--Select--"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Blue"></asp:ListItem>
                                                                <asp:ListItem Value="2" Text="Yellow"></asp:ListItem>
                                                                <asp:ListItem Value="3" Text="White"></asp:ListItem>--%>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>

                                    </asp:Panel>
                                    <div class="space20"></div>


                                </div>

                            </div>

                            <!-- END FORM-->

                        </div>

                    </div>
                    <br />
                    <%--  <span style="color:red;font-size: 15px;font-weight: bold!important;">Until Further Intimation From Corporate Office, Please Obtain PGRS Docket Number Manually by calling 1912.</span>--%>
                    <br />

                    <uc1:ApprovalHistoryView ID="ApprovalHistoryView" runat="server" />

                    <div class="row-fluid" runat="server" id="dvComments" style="display: none">
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
                                                                <asp:TextBox ID="txtComment" runat="server" MaxLength="200" TabIndex="4" TextMode="MultiLine"
                                                                    Width="550px" Height="125px" Style="resize: none" onkeyup="javascript:ValidateTextlimit(this,200)"></asp:TextBox>
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

                    <!-- END SAMPLE FORM PORTLET-->
                    <div class="text-center" align="center">




                        <asp:Button ID="cmdSave" runat="server" Text="Save"
                            OnClientClick="javascript:return ValidateMyForm()" CssClass="btn btn-primary"
                            OnClick="cmdSave_Click" />


                        <asp:Button ID="cmdReset" runat="server" Text="Reset"
                            CssClass="btn btn-primary" OnClick="cmdReset_Click" />
                        <%--<uc2:Processing ID="Processing" runat="server" /> --%>


                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>

                    </div>

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
                    <h4 class="modal-title">Help</h4>
                </div>
                <div class="modal-body">
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>This Web Page Can Be Used To Declare TC as Failure and Failure with Enhancement
                        .
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>There Are Two Radio Button 1.Failure 2.Failure
                        And Enhancement Available
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>Radio Button 1.Failure (Which will be selected
                        By Default) is to Declare TC as Failure
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>Radio Button 2.Failure And Enhancement is to Declare
                        TC as Failure With Enhancement
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>There are Two Links Available <u>View Transformer Centre Details</u> & <u>View DTr Details To Get More Details about DTC & DTR</u>
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

    </div>
</asp:Content>
