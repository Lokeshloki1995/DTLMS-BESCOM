<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="UserView.aspx.cs" Inherits="IIITS.DTLMS.MasterForms.UserView" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <%-- <link type="text/css" href="../assets/jquery.dataTables.css" rel="stylesheet" />
    <script type="text/javascript" src="../assets/jquery.dataTables.min.js"></script>--%>

    <%--<script type="text/javascript" src="https://cdn.datatables.net/1.10.13/js/jquery.dataTables.min.js"></script>--%>
    <%--<link type="text/css" rel="stylesheet" href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" />--%>




        <link type="text/css" href="../assets/jquery.dataTables.css" rel="stylesheet" />    <script type="text/javascript" src="../assets/jquery.dataTables.min.js"></script>               <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"></script><script type="text/javascript" src="https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js"></script>

    <script type="text/javascript">$(document).ready(function () {
    $.noConflict();
    var table = $('grdUser').DataTable();
});
    </script>


    <style type="text/css">
        table {
            overflow: scroll;
        }

        td {
            border: 1px solid #ccc;
            text-align: center;
        }

        .table-advance thead tr th {
            background-color: #438eb9 !important;
            color: #fff;
        }

        table.dataTable thead th {
            border-bottom: 1px solid #111;
            font-size: 12px !important;
        }

        table.dataTable tbody th, table.dataTable tbody td {
            padding: 10px 0px !important;
            text-align: center !important;
        }

        .table-advance tr td {
            border-left-width: 1px !important;
            border: 1px solid #d4d4d4;
            font-size: 12px !important;
        }

        th {
            white-space: nowrap;
        }
    </style>

    <script type="text/javascript">        //
        $(document).ready(function () {
            $('#ContentPlaceHolder1_grdUser').prepend($("<thead></thead>").append($(this).find("tr:first"))).DataTable({

                "sPaginationType": "full_numbers"
            });
        });
    </script>

    <%-- <script type="text/javascript">
        $.expr[":"].containsNoCase = function (el, i, m) {
            var search = m[3];
            if (!search) return false;
            return eval("/" + search + "/i").test($(el).text());
        };

        $(document).ready(function () {
            $('#txtSearch').keyup(function () {
                //alert("dfgdfg");
                debugger;
                if ($('#txtSearch').val().length > 1) {
                  
                    $('table#ContentPlaceHolder1_grdUser tr').hide();
                    $('table#ContentPlaceHolder1_grdUser tr:first').show();
                    $('table#ContentPlaceHolder1_grdUser tr td:containsNoCase(\'' + $('#txtSearch').val() + '\')').parent().show();
                }
                else if ($('#txtSearch').val().length == 0) {
                    resetSearchValue();
                }
                debugger;
                if ($('table#ContentPlaceHolder1_grdUser tr:visible').length == 1) {
                    $('.norecords').remove();
                    $('table#ContentPlaceHolder1_grdUser').append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
                }
            });

            $('#txtSearch').keyup(function (event) {
                if (event.keyCode == 27) {
                    resetSearchValue();
                }
            });
        });

        function resetSearchValue() {
            $('#txtSearch').val('');
            $('table#ContentPlaceHolder1_grdUser tr').show();
            $('.norecords').remove();
            $('#txtSearch').focus();
        }

    </script>--%>
    <%-- <script type="text/javascript">
        (function () {
            var showResults;
            $('#myInput').keyup(function () {
                var searchText;
                searchText = $('#myInput').val();
                return showResults(searchText);
            });
            showResults = function (searchText) {
                $('tbody tr').hide();
                return $('tbody tr:Contains(' + searchText + ')').show();
            };
            jQuery.expr[':'].Contains = jQuery.expr.createPseudo(function (arg) {
                return function (elem) {
                    return jQuery(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
                };
            });
        }.call(this));

</script>--%>
    <%--<script type="text/javascript">
    $(document).ready(function () {

    $('table#ContentPlaceHolder1_grdUser').each(function () {
        var $table = $(this);
        var itemsPerPage = 10;
        var currentPage = 0;
        var pages = Math.ceil($table.find("tr:not(:has(th))").length / itemsPerPage);
        $table.bind('repaginate', function () {
            if (pages > 1) {
                var pager;
                if ($table.next().hasClass("pager"))
                    pager = $table.next().empty(); else
                    pager = $('<div class="pager" style="padding-top: 20px; direction:ltr; " align="center"></div>');

                $('<span class="pg-goto"></span>').text(' « First ').bind('click', function () {
                    currentPage = 0;
                    $table.trigger('repaginate');
                }).appendTo(pager);

                $('<span class="pg-goto"> « Prev </span>').bind('click', function () {
                    if (currentPage > 0)
                        currentPage--;
                    $table.trigger('repaginate');
                }).appendTo(pager);

                var startPager = currentPage > 2 ? currentPage - 2 : 0;
                var endPager = startPager > 0 ? currentPage + 3 : 5;
                if (endPager > pages) {
                    endPager = pages;
                    startPager = pages - 5; if (startPager < 0)
                        startPager = 0;
                }

                for (var page = startPager; page < endPager; page++) {
                    $('<span id="pg' + page + '" class="' + (page == currentPage ? 'pg-selected' : 'pg-normal') + '"></span>').text(page + 1).bind('click', {
                        newPage: page
                    }, function (event) {
                        currentPage = event.data['newPage'];
                        $table.trigger('repaginate');
                    }).appendTo(pager);
                }

                $('<span class="pg-goto"> Next » </span>').bind('click', function () {
                    if (currentPage < pages - 1)
                        currentPage++;
                    $table.trigger('repaginate');
                }).appendTo(pager);
                $('<span class="pg-goto"> Last » </span>').bind('click', function () {
                    currentPage = pages - 1;
                    $table.trigger('repaginate');
                }).appendTo(pager);

                if (!$table.next().hasClass("pager"))
                    pager.insertAfter($table);
                //pager.insertBefore($table);

            }// end $table.bind('repaginate', function () { ...

            $table.find(
            'tbody tr:not(:has(th))').hide().slice(currentPage * itemsPerPage, (currentPage + 1) * itemsPerPage).show();
        });

        $table.trigger('repaginate');
    });
    });
</script>--%>
    <script type="text/javascript">

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip(); // added css in font-awesome.min.css line 43 and 405
        });

        function ConfirmStatus(status) {

            var result = confirm('Are you sure,Do you want to ' + status + ' User?');
            if (result) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>

    <style type="text/css">
        table#ContentPlaceHolder1_grdUser {
            overflow: auto;
        }

        td {
            border: none;
            text-align: center;
        }

        .table-advance thead tr th {
            background-color: #438eb9 !important;
            color: #fff;
        }

        th {
            white-space: nowrap;
            text-align: center !important;
        }

        thead {
            text-align: center !important;
        }

        span {
            text-align: center;
        }

        select#ContentPlaceHolder1_cmbZone, select#ContentPlaceHolder1_cmbsubdivision, select#ContentPlaceHolder1_cmbCircle, select#ContentPlaceHolder1_cmbSection, select#ContentPlaceHolder1_cmbDivision {
            width: 216px !important;
        }

        select {
            width: 70px;
        }

        .gvPagerCss span {
            background-color: #f9f9f9;
            font-size: 18px;
        }

        .gvPagerCss td {
            padding-left: 5px;
            padding-right: 5px;
        }

        .modalBackground {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }

        .ascending th a {
            background: url(img/sort_asc.png) no-repeat;
            display: block;
            padding: 0px 4px 0 20px;
        }



        .descending th a {
            background: url(img/sort_desc.png) no-repeat;
            display: block;
            padding: 0 4px 0 20px;
        }

        .both th a {
            background: url(img/sort_both.png) no-repeat;
            display: block;
            padding: 0 4px 0 20px;
        }

        div.pager {
            text-align: center;
            margin: 1em 0;
        }

        div.pager {
            text-align: center;
            margin: 1em 0;
        }

        div.pg-goto {
            color: #000000;
            font-size: 15px;
            cursor: pointer;
            background: #D0B389;
            padding: 2px 4px 2px 4px;
        }

        div.pg-selected {
            color: #fff;
            font-size: 15px;
            background: #000000;
            padding: 2px 4px 2px 4px;
        }

        div.pg-normal {
            color: #000000;
            font-size: 15px;
            cursor: pointer;
            background: #D0B389;
            padding: 2px 4px 2px 4px;
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
                <div class="span12">
                    <!-- BEGIN THEME CUSTOMIZER-->

                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title">User View
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
                            <h4><i class="icon-reorder"></i>User View</h4>
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
                                        <div>
                                            <div style="float: right">
                                                <div class="span2">
                                                    <asp:Button ID="cmdNew" runat="server" Text="New User"
                                                        CssClass="btn btn-primary" OnClick="cmdNew_Click" /><br />
                                                </div>

                                            </div>
                                            <div class="space20">
                                            </div>

                                            <div class="span5">
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
                                                    <label class="control-label">Circle Name</label>
                                                    <div class="controls">
                                                        <div class="input-append">

                                                            <asp:DropDownList ID="cmbCircle" runat="server" AutoPostBack="true"
                                                                TabIndex="1" OnSelectedIndexChanged="cmbCircle_SelectedIndexChanged">
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Division Name</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbDivision" runat="server" AutoPostBack="true"
                                                                TabIndex="2" OnSelectedIndexChanged="cmbDivision_SelectedIndexChanged">
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>



                                            <div class="span5">

                                                <div class="control-group">
                                                    <label class="control-label">Sub Division Name</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbsubdivision" runat="server" AutoPostBack="true"
                                                                TabIndex="3" OnSelectedIndexChanged="cmbsubdivision_SelectedIndexChanged">
                                                            </asp:DropDownList>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="control-group">
                                                    <label class="control-label">Section Name</label>
                                                    <div class="controls">
                                                        <div class="input-append">
                                                            <asp:DropDownList ID="cmbSection" runat="server" AutoPostBack="true" TabIndex="4" OnSelectedIndexChanged="cmbSection_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                            <asp:HiddenField ID="hdfCirle" runat="server" />
                                                            <asp:HiddenField ID="hdfDivision" runat="server" />
                                                            <asp:HiddenField ID="hdfSubdivision" runat="server" />
                                                            <asp:HiddenField ID="hdfSection" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>




                                            </div>

                                            <div class="span20"></div>

                                            <div class="form-horizontal" align="center">

                                                <div class="span3"></div>

                                                <div class="span1">
                                                    <asp:Button ID="cmdReset" runat="server" Text="Reset" TabIndex="11"
                                                        CssClass="btn btn-primary" OnClick="cmdReset_Click" /><br />
                                                </div>
                                                <div class="span1">
                                                    <asp:Button ID="cmdexport" runat="server" Text="Export Excel" CssClass="btn btn-primary"
                                                        OnClick="Export_click" /><br />
                                                </div>


                                                <div class="space20"></div>
                                            </div>
                                            <div class="span20"></div>
                                            <br />
                                            <%--   <div>
                                                <input type="text" id="txtSearch" name="txtSearch"  maxlength="50" style="height: 25px; font: 100" />
                                           <%--    <input type="text" id="myInput" onkeyup="myFunction()" placeholder="Search for names.." title="Type in a name">
                                                <div >
                                                   
                                                <%--OnPageIndexChanging="grdUser_PageIndexChanging" AllowPaging="true" OnSorting="grdUser_Sorting" AllowSorting="true"   --%>
                                            <div class="">


                                                <asp:GridView ID="grdUser"
                                                    AutoGenerateColumns="false" ShowHeaderWhenEmpty="True" EmptyDataText="No Records Found" OnRowDataBound="grdUser_RowDataBound" OnRowCommand="grdUser_RowCommand"
                                                    CssClass="table table-striped table-bordered table-advance table-hover"
                                                    runat="server"
                                                    ShowFooter="false">
                                                    <PagerStyle CssClass="gvPagerCss " />
                                                    <HeaderStyle CssClass="both" />




                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sl No" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <%#Container.DataItemIndex+1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField AccessibleHeaderText="US_ID" HeaderText="ID" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblUserId" runat="server" Text='<%# Bind("US_ID") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>


                                                        <asp:TemplateField AccessibleHeaderText="US_FULL_NAME" HeaderText="Name" SortExpression="US_FULL_NAME">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFullName" runat="server" Text='<%# Bind("US_FULL_NAME") %>' Style="word-break: break-all;" Width="150px"></asp:Label>
                                                            </ItemTemplate>
                                                            <%-- <FooterTemplate>
                                                                <asp:TextBox ID="txtsFullName" runat="server" Width="120px" placeholder="Enter Name" ToolTip="Enter Name to Search"></asp:TextBox>
                                                            </FooterTemplate>--%>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField AccessibleHeaderText="US_EMAIL" HeaderText="Email Id">

                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("US_EMAIL") %>' Style="word-break: break-all;" Width="150px"></asp:Label>
                                                            </ItemTemplate>
                                                            <%-- <FooterTemplate>
                                                                <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/img/Manual/search.png" Height="25px" ToolTip="Search" CommandName="search" TabIndex="9" />
                                                            </FooterTemplate>--%>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField AccessibleHeaderText="US_MOBILE_NO" HeaderText="Mobile No">

                                                            <ItemTemplate>
                                                                <asp:Label ID="lblMobileNo" runat="server" Text='<%# Bind("US_MOBILE_NO") %>' Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField AccessibleHeaderText="RO_NAME" HeaderText="Role Name" SortExpression="RO_NAME">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRole" runat="server" Text='<%# Bind("RO_NAME") %>' Style="word-break: break-all;" Width="120px"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField AccessibleHeaderText="US_DESG_ID" HeaderText="Designation" SortExpression="US_DESG_ID">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblDesignation" runat="server" Text='<%# Bind("US_DESG_ID") %>' Style="word-break: break-all;" Width="100px"></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="txtsDesignation" runat="server" Width="100px" placeholder="Enter Designation" ToolTip="Enter Designation to Search" Visible="false"></asp:TextBox>
                                                            </FooterTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField AccessibleHeaderText="OFF_NAME" HeaderText="Location" SortExpression="OFF_NAME">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblOfficeName" runat="server" Text='<%# Bind("OFF_NAME") %>' Style="word-break: break-all;" Width="200px"></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="txtOfficeName" runat="server" Width="150px" placeholder="Enter Designation" ToolTip="Enter Designation to Search" Visible="false"></asp:TextBox>
                                                            </FooterTemplate>
                                                        </asp:TemplateField>


                                                        <asp:TemplateField HeaderText="Edit">
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="imgBtnEdit" runat="server" Height="12px" ImageUrl="~/Styles/images/edit64x64.png" CommandName="create"
                                                                        Width="12px" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>


                                                        <asp:TemplateField HeaderText="Status">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblStatus" runat="server" Visible="false" Text='<%# Eval("US_STATUS1") %>'></asp:Label>
                                                                <center>
                                                                    <asp:ImageButton Visible="false" ID="imgDeactive" runat="server" ImageUrl="~/img/Manual/Disable.png" CommandName="status"
                                                                        ToolTip="Click to Enable User" OnClientClick="return ConfirmStatus('Enable');" Width="10px" />
                                                                    <asp:ImageButton Visible="false" ID="imgActive" runat="server" ImageUrl="~/img/Manual/Enable.gif" CommandName="status"
                                                                        ToolTip="Click to Disable User" OnClientClick="return ConfirmStatus('Disable');" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Delete" Visible="false">
                                                            <ItemTemplate>
                                                                <center>
                                                                    <asp:ImageButton ID="imbBtnDelete" runat="server" Height="12px" ImageUrl="~/Styles/images/delete64x64.png"
                                                                        Width="12px" OnClientClick="return confirm ('Are you sure, you want to delete');"
                                                                        CausesValidation="false" />
                                                                </center>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>

                                                    <PagerSettings FirstPageText="first" LastPageText="last" Mode="NumericFirstLast" />


                                                </asp:GridView>
                                            </div>
                                        </div>

                                        <ajax:ModalPopupExtender ID="mdlPopup" runat="server" TargetControlID="btnshow" CancelControlID="cmdClose"
                                            PopupControlID="pnlControls" BackgroundCssClass="modalBackground" />
                                        <div style="width: 100%; vertical-align: middle; height: 369px;" align="center">
                                            <div style="display: none">
                                                <asp:Button ID="btnshow" runat="server" Text="Button" />
                                            </div>
                                            <asp:Panel ID="pnlControls" runat="server" BackColor="White" Height="200px" Width="550px">
                                                <div class="widget blue">
                                                    <div class="widget-title">
                                                        <h4>Give Reason </h4>
                                                        <div class="space20"></div>
                                                        <%--<div class="row-fluid">--%>
                                                        <div class="span1"></div>
                                                        <div class="space20">
                                                            <div class="span1"></div>

                                                            <div class="span5">

                                                                <div class="control-group" style="font-weight: bold">
                                                                    <label class="control-label">Reason<span class="Mandotary"> *</span></label>

                                                                    <div class="controls">
                                                                        <div class="input-append" align="center">

                                                                            <asp:TextBox ID="txtReason" runat="server" MaxLength="500" TabIndex="4" TextMode="MultiLine" Style="resize: none"
                                                                                onkeyup="javascript:ValidateTextlimit(this,100)"></asp:TextBox>

                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div align="center">
                                                                    <div class="control-group" style="font-weight: bold">
                                                                        <label class="control-label">Effect From<span class="Mandotary"> *</span></label>
                                                                        <div class="controls">
                                                                            <div class="input-append" align="center">

                                                                                <asp:TextBox ID="txtEffectFrom" runat="server" MaxLength="10" TabIndex="3"></asp:TextBox>
                                                                                <ajax:CalendarExtender ID="CalendarExtender1" runat="server"
                                                                                    CssClass="cal_Theme1" TargetControlID="txtEffectFrom" Format="dd/MM/yyyy">
                                                                                </ajax:CalendarExtender>

                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="span5">
                                                                    <div class="control-group" style="font-weight: bold">

                                                                        <div class="controls">
                                                                            <div class="input-append">

                                                                                <div class="span10">
                                                                                    <asp:Button ID="cmdSubmit" runat="server" CssClass="btn btn-primary"
                                                                                        OnClick="cmdSubmit_Click" TabIndex="10" Text="Submit" />
                                                                                </div>
                                                                                <div class="span1">
                                                                                    <asp:Button ID="cmdClose" runat="server" CssClass="btn btn-primary"
                                                                                        TabIndex="10" Text="Close" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>


                                                                <div class="space20" align="center">

                                                                    <div class="form-horizontal" align="center">
                                                                        <asp:Label ID="lblMsg" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
                                                                    </div>


                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="space20"></div>
                                                    <div class="space20"></div>

                                                </div>
                                            </asp:Panel>
                                        </div>


                                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>


                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>



            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->



            <!-- END PAGE CONTENT-->
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
                        <i class="fa fa-info-circle"></i>This Web Page Can be used To View Existing User Details and To Add New User
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>New User Can Be Added By Clicking New User Button
                    </p>
                    <p style="color: Black">
                        <i class="fa fa-info-circle"></i>User Can Be Enabled/Disabled By clicking Status Radio Button
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
