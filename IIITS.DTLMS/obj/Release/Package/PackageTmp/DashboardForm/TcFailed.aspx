<%@ Page Title="" Language="C#"  AutoEventWireup="true" CodeBehind="TcFailed.aspx.cs" Inherits="IIITS.DTLMS.DashboardForm.TcFailed" %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FailurePendingOverview.aspx.cs" Inherits="IIITS.DTLMS.DashboardForm.FailurePendingOverview" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 <script src="../Scripts/functions.js" type="text/javascript"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta charset="utf-8" />
   <meta content="width=device-width, initial-scale=1.0" name="viewport" />
   <meta content="" name="description" />
   <meta content="Mosaddek" name="author" />
   <link href="/assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
   <link href="/assets/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
   <link href="/assets/bootstrap/css/bootstrap-fileupload.css" rel="stylesheet" />
   <link href="/assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
   <link href="/css/style.css" rel="stylesheet" />
   <link href="/css/style-responsive.css" rel="stylesheet" />
   <link href="/css/style-default.css" rel="stylesheet" id="style_color" />
   <link href="/assets/fullcalendar/fullcalendar/bootstrap-fullcalendar.css" rel="stylesheet" />
   <link href="/assets/jquery-easy-pie-chart/jquery.easy-pie-chart.css" rel="stylesheet" type="text/css" media="screen"/>
    <link href="/Styles/calendar.css" rel="stylesheet" type="text/css" />
   <script type="text/javascript" src="Scripts/functions.js"></script>

     <link href="assets/jquery-easy-pie-chart/jquery.easy-pie-chart.css" rel="stylesheet" type="text/css" media="screen" />
 <script type="text/javascript" src="https://code.jquery.com/jquery-1.12.4.js"></script>

       <script language="Javascript" type="text/javascript">

                    function onlyAlphabets(e, t) {
                        var code = ('charCode' in e) ? e.charCode : e.keyCode;
                        if ( // space
                           
                            !(code > 44 && code < 60) &&
                            !(code > 38 && code < 40) &&
                             !(code == 47) &&
                            !(code == 95) &&
                          !(code > 64 && code < 94) && // upper alpha (A-Z)
                          !(code > 96 && code < 126)) { // lower alpha (a-z)
                            e.preventDefault();
                        }
                    }

                    function onlyNums(e, t) {
                        var code = ('charCode' in e) ? e.charCode : e.keyCode;
                        if (!(code > 47 && code < 58)) {
                            e.preventDefault();
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
    <title></title>
</head>
<body>
 <form id="form1" runat="server">
    <div>
    <div>
        <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN THEME CUSTOMIZER-->
                    <!-- END THEME CUSTOMIZER-->
                    <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                    <h3 class="page-title">
                        <asp:Label ID="failure" runat="server" Text="Tc Failure Details" ForeColor="White"></asp:Label>
                    </h3>
                      <div style="float:right" >
                                
                             <div class="span1">
                                       
                                          </div>

                                            </div>
                   
                </div>
            </div>
             
            <div class="row-fluid" >
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4>
                                <i class="icon-reorder"></i><asp:Label ID="failureText" runat="server" Text="Tc Failure Details"></asp:Label></h4>
                            <%--<span class="tools"><a href="javascript:;" class="icon-chevron-down"></a><a href="javascript:;"
                                class="icon-remove"></a></span>--%>
                        </div>
                        <div class="widget-body">
                            <div style="float: right">
                            </div>

                             <asp:Button ID="cmdexport" runat="server" Text="Export Excel"  CssClass="btn btn-primary" 
                                          OnClick="Export_clickTcFailure" /><br />
                            <!-- END FORM-->
                            <div style="float: right">
                                <asp:HiddenField ID="hdfOffCode" runat="server" />
                            </div>
                            <div class="space20">
                            </div>
                           
                            <div style=" overflow:auto;">
                                
                                <%--//total tc failed--%>
                                <asp:GridView ID="grdFailuretc" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" 
                                      OnRowCommand="grdFailuretc_RowCommand" 
                                    onpageindexchanging="grdFailuretc_PageIndexChanging"
                                     Visible="false" OnSorting="grdFailuretc_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" />
                                   <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttCode" runat="server" placeholder="Enter DTR Code " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="TC_CAPACITY" Visible="true" SortExpression="TC_CAPACITY">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttcCapacity" runat="server" placeholder="Enter TC CAPACITY " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC_SLNO" Visible="true" SortExpression="TC_SLNO">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTcSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                    CommandName="search" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="TC_MANUFACTURE_DATE" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCMANFDATE" runat="server" Text='<%# Bind("TC_MANF_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    
                                    </Columns>
                                </asp:GridView>



                                  <asp:GridView ID="grdtotaldtr" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" 
                                      OnRowCommand="grdtotaldtr_RowCommand" 
                                    onpageindexchanging="grdtotaldtr_PageIndexChanging"
                                     Visible="false" OnSorting="grdtotaldtr_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" />
                                   <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttCode" runat="server" placeholder="Enter DTR Code " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="TC_CAPACITY" Visible="true" SortExpression="TC_CAPACITY">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttcCapacity" runat="server" placeholder="Enter TC CAPACITY " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC_SLNO" Visible="true" SortExpression="TC_SLNO">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTcSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                    CommandName="search" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="TC_MANUFACTURE_DATE" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCMANFDATE" runat="server" Text='<%# Bind("TC_MANF_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    
                                    </Columns>
                                </asp:GridView>


                                   <asp:GridView ID="grdfaultyfield" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" 
                                      OnRowCommand="grdfaultyfield_RowCommand" 
                                    onpageindexchanging="grdfaultyfield_PageIndexChanging"
                                     Visible="false" OnSorting="grdfaultyfield_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" />
                                   <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttCode" runat="server" placeholder="Enter DTR Code " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="TC_CAPACITY" Visible="true" SortExpression="TC_CAPACITY">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttcCapacity" runat="server" placeholder="Enter TC CAPACITY " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC_SLNO" Visible="true" SortExpression="TC_SLNO">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTcSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                    CommandName="search" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="TC_MANUFACTURE_DATE" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCMANFDATE" runat="server" Text='<%# Bind("TC_MANF_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    
                                    </Columns>
                                </asp:GridView>


                                   <asp:GridView ID="grdfaultystore" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" 
                                      OnRowCommand="grdfaultystore_RowCommand" 
                                    onpageindexchanging="grdfaultystore_PageIndexChanging"
                                     Visible="false" OnSorting="grdfaultystore_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" />
                                   <%-- <HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttCode" runat="server" placeholder="Enter DTR Code" Width="150px"  AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="TC_CAPACITY" Visible="true" SortExpression="TC_CAPACITY">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttcCapacity" runat="server" placeholder="Enter TC CAPACITY " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC_SLNO" Visible="true" SortExpression="TC_SLNO">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTcSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                    CommandName="search" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="TC_MANUFACTURE_DATE" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCMANFDATE" runat="server" Text='<%# Bind("TC_MANF_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                     <asp:TemplateField AccessibleHeaderText="DIV_NAME" HeaderText="DIVISION NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbldivname" runat="server" Text='<%# Bind("DIV_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>


                                   <asp:GridView ID="grdfaultyrepairer" AutoGenerateColumns="false" PageSize="10" ShowHeaderWhenEmpty="True"
                                    EmptyDataText="No Records Found" ShowFooter="true" CssClass="table table-striped table-bordered table-advance table-hover"
                                    AllowPaging="true" runat="server" 
                                      OnRowCommand="grdfaultyrepairer_RowCommand" 
                                    onpageindexchanging="grdfaultyrepairer_PageIndexChanging"
                                     Visible="false" OnSorting="grdfaultyrepairer_Sorting" AllowSorting="true">
                                <HeaderStyle CssClass="both" />
                                    <%--<HeaderStyle HorizontalAlign="center" CssClass="Warning" />--%>
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="TC_CODE" HeaderText="DTR Code" Visible="true" SortExpression="TC_CODE">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCode" runat="server" Text='<%# Bind("TC_CODE") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel1" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttCode" runat="server" placeholder="Enter DTR Code " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_CAPACITY" HeaderText="TC_CAPACITY" Visible="true" SortExpression="TC_CAPACITY">
                                            <ItemTemplate>
                                                <asp:Label ID="lbltcCapacity" runat="server" Text='<%# Bind("TC_CAPACITY") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Panel ID="panel2" runat="server" DefaultButton="imgBtnSearch">
                                                    <asp:TextBox ID="txttcCapacity" runat="server" placeholder="Enter TC CAPACITY " Width="150px" AutoComplete="off" onkeypress="javascript: return onlyNums(event,this);"
                                                        MaxLength="6"></asp:TextBox>
                                                </asp:Panel>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_SLNO" HeaderText="TC_SLNO" Visible="true" SortExpression="TC_SLNO">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTcSlNo" runat="server" Text='<%# Bind("TC_SLNO") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgBtnSearch" runat="server" ImageUrl="~/img/Manual/search.png"
                                                    CommandName="search" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="TC_MANF_DATE" HeaderText="TC_MANUFACTURE_DATE" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCMANFDATE" runat="server" Text='<%# Bind("TC_MANF_DATE") %>' Style="word-break: break-all;"
                                                    Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                     <asp:TemplateField AccessibleHeaderText="DIV_NAME" HeaderText="DIVISION NAME">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbldivname" runat="server" Text='<%# Bind("DIV_NAME") %>' Style="word-break: break-all;"
                                                            Width="150px"></asp:Label>
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            
                            </div>
                        </div>
                    </div>
                    <!-- END SAMPLE FORM PORTLET-->
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                </div>
                </div>
            </div>
        </div>
  
          </div>
      <script type="text/javascript" src='<%= ResolveUrl("~/assets/jquery-slimscroll/jquery-ui-1.9.2.custom.min.js") %>'></script>
            <script type="text/javascript" src='<%= ResolveUrl("~/assets/jquery-slimscroll/jquery.slimscroll.min.js") %>'></script>
           <script src='<%= ResolveUrl("~/assets/jquery-easy-pie-chart/jquery.easy-pie-chart.js") %>' type="text/javascript"></script>
            <script src='<%= ResolveUrl("~/js/jquery.sparkline.js") %>' type="text/javascript"></script>
            <script src='<%= ResolveUrl("~/js/jquery.scrollTo.min.js") %>'></script>
        
    </form>


</body>
</html>
