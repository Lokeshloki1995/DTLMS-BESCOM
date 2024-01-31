﻿<%@ Page Title="" Language="C#" MasterPageFile="~/DTLMS.Master" AutoEventWireup="true" CodeBehind="DownLoad.aspx.cs" Inherits="IIITS.DTLMS.DashboardForm.DownLoad" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function openTab(th) {
            window.open(th.name, '_blank');
        }
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <style>
        table#ContentPlaceHolder1_gvFiles {
    width: 100%;
    text-align: center;
    margin-top:15px!important;
    margin-bottom:15px!important;
}
    </style>

         <div class="container-fluid">
            <!-- BEGIN PAGE HEADER-->
            <div class="row-fluid">
               <div class="span8">
                   <!-- BEGIN THEME CUSTOMIZER-->
                 
                   <!-- END THEME CUSTOMIZER-->
                  <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                   <h3 class="page-title">
                  Downloads
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
              
                          
            </div>
            <!-- END PAGE HEADER-->
            <!-- BEGIN PAGE CONTENT-->
            <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Downloads</h4>
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
                       <asp:ImageButton ID="imgBtnEdit" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                      
                            <div class="input-append"  color: #0099FF">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="lnkDownload" runat="server" 
                               style="font-size:12px;color:Blue;font-weight:bold" 
                             onclick="lnkDownload_Click" ForeColor="#3399FF">DTLMS Web Application User Manual</asp:LinkButton>                     
                               
                           
                        </div>
                    </div>
     
                 <div class="control-group">
                       <asp:ImageButton ID="ImageButton1" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                       
                            <div class="input-append">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="lnkAndroidManual" runat="server" 
                                   style="font-size:12px;color:Blue;font-weight:bold" onclick="lnkAndroidManual_Click" 
                                      >DTLMS Android Application User Manual</asp:LinkButton>                     
                            </div>
                        
                    </div>
                </div>
            <div class="span5">
                    <div class="control-group">
                       <asp:ImageButton ID="ImgTansactionFlow" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                       
                            <div class="input-append">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LnkImgTansactionFlow" runat="server" 
                                   style="font-size:12px;color:Blue;font-weight:bold" OnClick="LnkImgTansactionFlow_Click" >DTLMS Transaction FLow</asp:LinkButton>                     
                            </div>
                        
                    </div>

                  <div class="control-group">
                       <asp:ImageButton ID="ImageButton2" runat="server" Height="60px" ImageUrl="~/img/Manual/FolderImg.png"
                         Width="60px" />
                 
                     
                            <div class="input-append">
                                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="lnkAndroidApk" runat="server"
                                    style="font-size:12px;color:Blue;font-weight:bold" 
                                       onclick="lnkAndroidApk_Click" >Android APK file</asp:LinkButton>                     
                               
                           
                        </div>
                      <div>
<%--                          <asp:HyperLink ID="lnkPlayStoreLin" style="font-size:12px;color:Blue;font-weight:bold" NavigateUrl="https://play.google.com/store/apps/details?id=com.iiits.dtlms" runat="server"> Click Here </asp:HyperLink><b>to Download via Google PlayStore</b>--%>
                          <%--<asp:LinkButton ID="lnkPlayStoreLink" runat="server"
                                    style="font-size:12px;color:Blue;font-weight:bold" PostBackUrl="https://www.google.com" > https://play.google.com/store/apps/details?id=com.iiits.dtlms
</asp:LinkButton>      lnkAndroidManual_Click1                           lnkAndroidApk_Click   --%>
                      </div>
                    </div>



                        </div>

<div class="span1"></div>
                                        
                                        <div class="space20"></div>
                         
                                    </div>
                                </div>
                                
                                <div class="space20"></div>
                                <!-- END FORM-->
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                           
                        
                            
                        </div>
                    </div>
                    <!-- END SAMPLE FORM PORTLET-->
                </div>
            </div>
          

            <!-- END PAGE CONTENT-->
         </div>

             <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i> BESCOM Circulars/Procedures Download </h4>
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
                                        <div class="span1"></div>
                                        
                                        <div class="space20"></div>
      
 <div class="span4">

                 <div class="control-group">
                       <asp:ImageButton ID="ImageButton3" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                      
                            <div class="input-append"  >
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton1" runat="server" 
                               style="font-size:12px;color:Blue;font-weight:bold" 
                             onclick="lnkCirDownload_Click" ForeColor="#3399FF">BESCOM DTLMS Circulars</asp:LinkButton>                     
                               
                           
                        </div>
                    </div>
     
                 <div class="control-group">
                       <asp:ImageButton ID="ImageButton4" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                       
                            <div class="input-append">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton2" runat="server" 
                                   style="font-size:12px;color:Blue;font-weight:bold"   onclick="lnkProDownload_Click" 
                                      >BESCOM DTLMS Procedures</asp:LinkButton>                     
                            </div>
                        
                    </div>
     </div>
<div class="span6">
     <div class="control-group">
                       <asp:ImageButton ID="ImageButton5" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                       
                            <div class="input-append">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton3" runat="server" 
                                   style="font-size:12px;color:Blue;font-weight:bold" OnClick="lnkAnx1Download_Click" >Annexure 1 - Reporting of Failure of Transformer</asp:LinkButton>                     
                            </div>
                        
         
                           
                    </div>
     <div class="control-group">
                       <asp:ImageButton ID="ImageButton7" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                       
                            <div class="input-append">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton4" runat="server" 
                                   style="font-size:12px;color:Blue;font-weight:bold" OnClick="lnkAnx2Download_Click" >Annexure 2 - Joint Inspection Report of Failed Transformer (PHYSICAL) </asp:LinkButton>                     
                            </div>
                        
                    </div>

                  <div class="control-group">
                       <asp:ImageButton ID="ImageButton6" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                     
                            <div class="input-append">
                                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton5" runat="server"
                                    style="font-size:12px;color:Blue;font-weight:bold" 
                                       onclick="lnkAnx3Download_Click" >Annexure 3 - Joint Inspection Report of Failed Transformer (INTERNAL)</asp:LinkButton>                     
                               
                           
                        </div>
                     
                    </div>

      <div class="control-group">
                       <asp:ImageButton ID="ImageButton8" runat="server" Height="60px" ImageUrl="~/img/Manual/pdfimage.jpg"
                         Width="60px" />
                 
                     
                            <div class="input-append">
                                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                   <asp:LinkButton ID="LinkButton6" runat="server"
                                    style="font-size:12px;color:Blue;font-weight:bold" 
                                       onclick="lnkAnx4Download_Click" >Annexure 4 - Analysis of Failed Transformer within WGP</asp:LinkButton>                     
                               
                           
                        </div>
                     
                    </div>

                        </div>

<div class="span1"></div>
                                        
                                       
                                </div>
                                
                                <div class="space20"></div>
                                <!-- END FORM-->
                            <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label>
                           
                        
                            
                        </div>
                    </div>
                    <!-- END SAMPLE FORM PORTLET-->
                </div>
            </div>
          

            <!-- END PAGE CONTENT-->
         </div>
         </div>
              <div class="row-fluid">
                <div class="span12">
                    <!-- BEGIN SAMPLE FORMPORTLET-->
                    <div class="widget blue">
                        <div class="widget-title">
                            <h4><i class="icon-reorder"></i>Circular Downloads</h4>
                            <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                            <a href="javascript:;" class="icon-remove"></a>
                            </span>
                        </div>

        

       <%--  <asp:Button ID="btnGetFiles" Text="Get Filenames from MyFiles Folder" runat="server" onclick="GetFiles" />--%>
        <div class="container-fluid">
        <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="false">
<Columns>
    <asp:BoundField DataField="Name" ItemStyle-ForeColor="BlueViolet" HeaderText="File Name" ItemStyle-Width="700"   />     
    <asp:TemplateField>
        <ItemTemplate>
                    <asp:LinkButton ID="lnkDownload" runat="server" ForeColor="Blue" Text="Download" OnClick="DownloadFile"
            CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
        </ItemTemplate>
    </asp:TemplateField>
              

                 </Columns>
             </asp:GridView>
            </div>
    
    </div>
                      <asp:Label ID="Label2" runat="server" ForeColor="Red"></asp:Label>
                    
            <!-- END PAGE CONTENT-->
         </div>
         </div>
</asp:Content>
