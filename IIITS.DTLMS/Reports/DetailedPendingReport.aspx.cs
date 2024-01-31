using ClosedXML.Excel;
using IIITS.DTLMS.BL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IIITS.DTLMS.Reports
{
    public partial class DetailedPendingReport : System.Web.UI.Page
    {
        clsSession objSession;

        /// <summary>
        /// page load method to get login user details which is in session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }

                objSession = (clsSession)Session["clsSession"];
                if (!IsPostBack)
                {

                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// to Generate EXpor excel of detailed pending details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdReport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtrep = new DataTable();
                DataTable dtstr = new DataTable();
                DataTable dtff = new DataTable();
                DataTable dttr = new DataTable();
                clsReports objReport = new clsReports();

                dtrep = objReport.getRepairerDetails();
                dtstr = objReport.getStoreDetails();
                dtff = objReport.getfaultyfieldDetails();
                dttr = objReport.gettobereplacedDetails();

                if (dtrep.Rows.Count > 0)
                {
                    string[] arrAlpha = Genaral.getalphanew();
                    string sMergeRange = arrAlpha[dtrep.Columns.Count - 1];
                    string sMergeRange1 = arrAlpha[dtff.Columns.Count - 1];
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        //RENAMING TO SHEET 1
                        dtrep.Columns["DIV_NAME"].ColumnName = "Division Name";
                        dtrep.Columns["TR_NAME"].ColumnName = "Repairer Name";

                        dtrep.Columns["agp_10 KVA"].ColumnName = "AGP_10 KVA";
                        dtrep.Columns["agp_15 KVA"].ColumnName = "AGP_15 KVA";
                        dtrep.Columns["agp_25 KVA"].ColumnName = "AGP_25 KVA";
                        dtrep.Columns["agp_50 KVA"].ColumnName = "AGP_50 KVA";
                        dtrep.Columns["agp_63 KVA"].ColumnName = "AGP_63 KVA";
                        dtrep.Columns["agp_100 KVA"].ColumnName = "AGP_100 KVA";
                        dtrep.Columns["agp_125 KVA"].ColumnName = "AGP_125 KVA";
                        dtrep.Columns["agp_150 KVA"].ColumnName = "AGP_150 KVA";
                        dtrep.Columns["agp_160 KVA"].ColumnName = "AGP_160 KVA";
                        dtrep.Columns["agp_200 KVA"].ColumnName = "AGP_200 KVA";
                        dtrep.Columns["agp_250 KVA"].ColumnName = "AGP_250 KVA";
                        dtrep.Columns["agp_300 KVA"].ColumnName = "AGP_300 KVA";
                        dtrep.Columns["agp_315 KVA"].ColumnName = "AGP_315 KVA";
                        dtrep.Columns["agp_400 KVA"].ColumnName = "AGP_400 KVA";
                        dtrep.Columns["agp_500 KVA"].ColumnName = "AGP_500 KVA";
                        dtrep.Columns["agp_630 KVA"].ColumnName = "AGP_630 KVA";
                        dtrep.Columns["agp_750 KVA"].ColumnName = "AGP_750 KVA";
                        dtrep.Columns["agp_960 KVA"].ColumnName = "AGP_960 KVA";
                        dtrep.Columns["agp_990 KVA"].ColumnName = "AGP_990 KVA";
                        dtrep.Columns["agp_1000 KVA"].ColumnName = "AGP_1000 KVA";
                        dtrep.Columns["agp_1250 KVA"].ColumnName = "AGP_1250 KVA";
                        dtrep.Columns["AGP_TOTAL_COUNT"].ColumnName = "AGP Total NO OF TC's";

                        dtrep.Columns["wgp_10 KVA"].ColumnName = "WGP_10 KVA";
                        dtrep.Columns["wgp_15 KVA"].ColumnName = "GP_15 KVA";
                        dtrep.Columns["wgp_25 KVA"].ColumnName = "WGP_25 KVA";
                        dtrep.Columns["wgp_50 KVA"].ColumnName = "WGP_50 KVA";
                        dtrep.Columns["wgp_63 KVA"].ColumnName = "WGP_63 KVA";
                        dtrep.Columns["wgp_100 KVA"].ColumnName = "WGP_100 KVA";
                        dtrep.Columns["wgp_125 KVA"].ColumnName = "WGP_125 KVA";
                        dtrep.Columns["wgp_150 KVA"].ColumnName = "WGP_150 KVA";
                        dtrep.Columns["wgp_160 KVA"].ColumnName = "WGP_160 KVA";
                        dtrep.Columns["wgp_200 KVA"].ColumnName = "WGP_200 KVA";
                        dtrep.Columns["wgp_250 KVA"].ColumnName = "WGP_250 KVA";
                        dtrep.Columns["wgp_300 KVA"].ColumnName = "WGP_300 KVA";
                        dtrep.Columns["wgp_315 KVA"].ColumnName = "WGP_315 KVA";
                        dtrep.Columns["wgp_400 KVA"].ColumnName = "WGP_400 KVA";
                        dtrep.Columns["wgp_500 KVA"].ColumnName = "WGP_500 KVA";
                        dtrep.Columns["wgp_630 KVA"].ColumnName = "WGP_630 KVA";
                        dtrep.Columns["wgp_750 KVA"].ColumnName = "WGP_750 KVA";
                        dtrep.Columns["wgp_960 KVA"].ColumnName = "WGP_960 KVA";
                        dtrep.Columns["wgp_990 KVA"].ColumnName = "WGP_990 KVA";
                        dtrep.Columns["wgp_1000 KVA"].ColumnName = "WGP_1000 KVA";
                        dtrep.Columns["wgp_1250 KVA"].ColumnName = "WGP_1250 KVA";
                        dtrep.Columns["WGP_TOTAL_COUNT"].ColumnName = "WGP Total NO OF TC's";

                        dtrep.Columns["wrgp_10 KVA"].ColumnName = "WRGP_10 KVA";
                        dtrep.Columns["wrgp_15 KVA"].ColumnName = "WRGP_15 KVA";
                        dtrep.Columns["wrgp_25 KVA"].ColumnName = "WRGP_25 KVA";
                        dtrep.Columns["wrgp_50 KVA"].ColumnName = "WRGP_50 KVA";
                        dtrep.Columns["wrgp_63 KVA"].ColumnName = "WRGP_63 KVA";
                        dtrep.Columns["wrgp_100 KVA"].ColumnName = "WRGP_100 KVA";
                        dtrep.Columns["wrgp_125 KVA"].ColumnName = "WRGP_125 KVA";
                        dtrep.Columns["wrgp_150 KVA"].ColumnName = "WRGP_150 KVA";
                        dtrep.Columns["wrgp_160 KVA"].ColumnName = "WRGP_160 KVA";
                        dtrep.Columns["wrgp_200 KVA"].ColumnName = "WRGP_200 KVA";
                        dtrep.Columns["wrgp_250 KVA"].ColumnName = "WRGP_250 KVA";
                        dtrep.Columns["wrgp_300 KVA"].ColumnName = "WRGP_300 KVA";
                        dtrep.Columns["wrgp_315 KVA"].ColumnName = "WRGP_315 KVA";
                        dtrep.Columns["wrgp_400 KVA"].ColumnName = "WRGP_400 KVA";
                        dtrep.Columns["wrgp_500 KVA"].ColumnName = "WRGP_500 KVA";
                        dtrep.Columns["wrgp_630 KVA"].ColumnName = "WRGP_630 KVA";
                        dtrep.Columns["wrgp_750 KVA"].ColumnName = "WRGP_750 KVA";
                        dtrep.Columns["wrgp_960 KVA"].ColumnName = "WRGP_960 KVA";
                        dtrep.Columns["wrgp_990 KVA"].ColumnName = "WRGP_990 KVA";
                        dtrep.Columns["wrgp_1000 KVA"].ColumnName = "WRGP_1000 KVA";
                        dtrep.Columns["wrgp_1250 KVA"].ColumnName = "WRGP_1250 KVA";
                        dtrep.Columns["WRGP_TOTAL_COUNT"].ColumnName = "WRGP Total NO OF TC's";
                        dtrep.Columns["REPAIRER_TOTAL COUNT"].ColumnName = "Total NO OF TC's";

                        //RENAMING TO SHEET 2
                        dtstr.Columns["SM_NAME"].ColumnName = "Store Name";
                        dtstr.Columns["DIV_NAME"].ColumnName = "Division Name";
                        dtstr.Columns["FAULTY_TOTAL_COUNT"].ColumnName = "Faulty Total NO OF TC's";
                        dtstr.Columns["BRANDNEW_TOTAL_COUNT"].ColumnName = "BRAND NEW Total NO OF TC's";
                        dtstr.Columns["REPAIRGOOD_TOTAL_COUNT"].ColumnName = "REPAIR GOOD Total NO OF TC's";
                        dtstr.Columns["STORE_TOTAL COUNT"].ColumnName = "Total NO OF TC's";

                        //RENAMING TO SHEET 3
                        dtff.Columns["DIV_NAME"].ColumnName = "Division Name";
                        dtff.Columns["SD_SUBDIV_NAME"].ColumnName = "Sub Division Name";
                        dtff.Columns["FF_10 KVA"].ColumnName = "10 KVA";
                        dtff.Columns["FF_15 KVA"].ColumnName = "15 KVA";
                        dtff.Columns["FF_25 KVA"].ColumnName = "25 KVA";
                        dtff.Columns["FF_50 KVA"].ColumnName = "50 KVA";
                        dtff.Columns["FF_63 KVA"].ColumnName = "63 KVA";
                        dtff.Columns["FF_100 KVA"].ColumnName = "100 KVA";
                        dtff.Columns["FF_125 KVA"].ColumnName = "125 KVA";
                        dtff.Columns["FF_150 KVA"].ColumnName = "150 KVA";
                        dtff.Columns["FF_160 KVA"].ColumnName = "160 KVA";
                        dtff.Columns["FF_200 KVA"].ColumnName = "200 KVA";
                        dtff.Columns["FF_250 KVA"].ColumnName = "250 KVA";
                        dtff.Columns["FF_300 KVA"].ColumnName = "300 KVA";
                        dtff.Columns["FF_315 KVA"].ColumnName = "315 KVA";
                        dtff.Columns["FF_400 KVA"].ColumnName = "400 KVA";
                        dtff.Columns["FF_500 KVA"].ColumnName = "500 KVA";
                        dtff.Columns["FF_630 KVA"].ColumnName = "630 KVA";
                        dtff.Columns["FF_750 KVA"].ColumnName = "750 KVA";
                        dtff.Columns["FF_960 KVA"].ColumnName = "960 KVA";
                        dtff.Columns["FF_990 KVA"].ColumnName = "990 KVA";
                        dtff.Columns["FF_1000 KVA"].ColumnName = "1000 KVA";
                        dtff.Columns["FF_1250 KVA"].ColumnName = "1250 KVA";
                        dtff.Columns["TOTAL_COUNT"].ColumnName = "Total NO OF TC's";

                        //RENAMING TO SHEET 4
                        dttr.Columns["DIV_NAME"].ColumnName = "Division Name";
                        dttr.Columns["SD_SUBDIV_NAME"].ColumnName = "Sub Division Name";
                        dttr.Columns["TOTAL_COUNT"].ColumnName = "Total NO OF TC's";


                        // adding sheets for excel 
                        wb.Worksheets.Add(dtrep, "Repairer_Details");
                        wb.Worksheets.Add(dtstr, "Store_Details");
                        wb.Worksheets.Add(dtff, "Faulty_Field_Details");
                        wb.Worksheets.Add(dttr, "ToBeReplaced_Details");


                        //CHANGES FOR SHEET 1 
                        wb.Worksheet(1).Row(1).InsertRowsAbove(3);

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue(ConfigurationManager.AppSettings["ProjectName"]);

                        var rangeReporthaed = wb.Worksheet(1).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed.SetValue("List of Detailed Repairer Pending Details ");

                        var rangeReportrep1 = wb.Worksheet(1).Range("C3:X3");
                        rangeReportrep1.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep1.Merge().Style.Fill.BackgroundColor = XLColor.BrilliantLavender;
                        rangeReportrep1.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep1.SetValue("AGP ");

                        var rangeReportrep2 = wb.Worksheet(1).Range("Y3:AT3");
                        rangeReportrep2.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep2.Merge().Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        rangeReportrep2.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep2.SetValue("WGP ");

                        var rangeReportrep3 = wb.Worksheet(1).Range("AU3:BQ3");
                        rangeReportrep3.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep3.Merge().Style.Fill.BackgroundColor = XLColor.Apricot;
                        rangeReportrep3.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep3.SetValue("WRGP ");


                        //CHANGES FOR SHEET 2 
                        wb.Worksheet(2).Row(1).InsertRowsAbove(3);

                        var rangeheader1 = wb.Worksheet(2).Range("A1:" + sMergeRange + "1");
                        rangeheader1.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader1.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader1.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader1.SetValue(ConfigurationManager.AppSettings["ProjectName"]);

                        var rangeReporthaed2 = wb.Worksheet(2).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed2.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed2.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed2.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed2.SetValue("List of Detailed Store Pending Details ");

                        var rangeReportrep11 = wb.Worksheet(2).Range("C3:X3");
                        rangeReportrep11.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep11.Merge().Style.Fill.BackgroundColor = XLColor.Apricot;
                        rangeReportrep11.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep11.SetValue("FAULTY");

                        var rangeReportrep21 = wb.Worksheet(2).Range("Y3:AT3");
                        rangeReportrep21.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep21.Merge().Style.Fill.BackgroundColor = XLColor.Brass;
                        rangeReportrep21.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep21.SetValue("BRAND NEW");

                        var rangeReportrep31 = wb.Worksheet(2).Range("AU3:BQ3");
                        rangeReportrep31.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrep31.Merge().Style.Fill.BackgroundColor = XLColor.BubbleGum;
                        rangeReportrep31.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrep31.SetValue("REPAIR GOOD");


                        //CHANGES FOR SHEET 3
                        wb.Worksheet(3).Row(1).InsertRowsAbove(3);

                        var rangeheader3 = wb.Worksheet(3).Range("A1:" + sMergeRange1 + "1");
                        rangeheader3.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader3.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader3.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader3.SetValue(ConfigurationManager.AppSettings["ProjectName"]);

                        var rangeReporthaed4 = wb.Worksheet(3).Range("A2:" + sMergeRange1 + "2");
                        rangeReporthaed4.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed4.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed4.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed4.SetValue("List of Detailed Faulty Field Pending Details ");

                        var rangeReportff1 = wb.Worksheet(3).Range("C3:X3");
                        rangeReportff1.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportff1.Merge().Style.Fill.BackgroundColor = XLColor.Apricot;
                        rangeReportff1.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportff1.SetValue("TC  CAPACITY ");

                        


                        // //CHANGES FOR SHEET 4
                        wb.Worksheet(4).Row(1).InsertRowsAbove(3);

                        var rangeheader5 = wb.Worksheet(4).Range("A1:" + sMergeRange1 + "1");
                        rangeheader5.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader5.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader5.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader5.SetValue(ConfigurationManager.AppSettings["ProjectName"]);

                        var rangeReporthaed6 = wb.Worksheet(4).Range("A2:" + sMergeRange1 + "2");
                        rangeReporthaed6.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed6.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed6.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed6.SetValue("List of Detailed To Be Replaced Pending Details ");

                        var rangeReportrg11 = wb.Worksheet(4).Range("C3:X3");
                        rangeReportrg11.Merge().Style.Font.SetBold().Font.FontSize = 14;
                        rangeReportrg11.Merge().Style.Fill.BackgroundColor = XLColor.Apricot;
                        rangeReportrg11.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReportrg11.SetValue("TC  CAPACITY");

                       


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "DetailedPendingDetails " + DateTime.Now + ".xls";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + FileName);

                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
                else
                {
                    ShowMsgBox("No record found");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        /// <summary>
        /// to show alert messages based on conditions
        /// </summary>
        /// <param name="Msg"></param>
        private void ShowMsgBox(string Msg)
        {
            try
            {
                string ShowMsg = string.Empty;
                ShowMsg = "<script language=javascript> alert ('" + Msg + "')</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "Msg", ShowMsg);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}