using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.IO;
using ClosedXML.Excel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Configuration;

namespace IIITS.DTLMS.Transaction
{
    public partial class ExistingDtrDetails : System.Web.UI.Page
    {
        string strFormCode = "ExistingDtrReport";
        clsSession objSession;
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

                    Genaral.Load_Combo("SELECT \"FY_YEARS\",\"FY_YEARS\" FROM \"TBLFINANCIALYEAR\"  WHERE \"FY_STATUS\"='1'", "--Select--", cmbFinYear);
                    CheckAccessRights("2");
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "ExistingDtrReport";
                objApproval.sRoleId = objSession.RoleId;
                objApproval.sAccessType = "1" + "," + sAccessType;
                bool bResult = objApproval.CheckAccessRights(objApproval);
                if (bResult == false)
                {
                    Response.Redirect("~/UserRestrict.aspx", false);

                }
                return bResult;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;

            }
        }

        protected void Export_Click(object sender, EventArgs e)
        {
            string strParam = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                clsReports objrep = new clsReports();
                string fnclyear = string.Empty;

                string sResult = string.Empty;
               // if (cmbFinYear.Text == "")
                    if (cmbFinYear.SelectedItem.Text == "--Select--")
                    {
                    ShowMsgBox("Please Select The Financial Year");
                    cmbFinYear.Focus();
                    return;

                }

                string previousyear = cmbFinYear.SelectedValue.Split('-').GetValue(0).ToString();
                string presentyear = cmbFinYear.SelectedValue.Split('-').GetValue(1).ToString();

                clsReports objdtrdetails = new clsReports();


                dt = objdtrdetails.getexistingdtrdetails(previousyear, presentyear);



                ViewState["allDetails"] = dt;

                if (dt.Rows.Count > 0)
                {
                    string[] arrAlpha = Genaral.getalpha();
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt.Columns["ZONE"].ColumnName = "ZONE NAME";
                        dt.Columns["CIRCLE"].ColumnName = "CIRCLE NAME";
                        dt.Columns["DIV_NAME"].ColumnName = "DIVISION NAME";
                        dt.Columns["DTC_ST"].ColumnName = "TOTAL NO OF EXISTING DTC AS AT THE END OF  MARCH-'"+ presentyear + "' AS PER STATISTICS";

                        dt.Columns["NO_OF_FIELD"].ColumnName = "TOTAL NO OF DTR'S IN  FIELD AS PER DTLMS";
                        dt.Columns["Balance_DTC"].ColumnName = "BALANCE EXISTING DTC TO BE ENTERED IN DTLMS SOFTWARE ";

                        dt.Columns["NO_OF_STORE"].ColumnName = "DTRS AT STORE";
                        dt.Columns["NO_OF_REP"].ColumnName = "DTRS AT REPAIRER";

                        dt.Columns["NO_OF_BANK"].ColumnName = "DTRS AT BANK";
                        dt.Columns["DTC_Failed_FY_2021-22_as_per_statistics"].ColumnName = "DTC_FAILED_FY_'" + cmbFinYear.SelectedValue + "'_AS_PER_STATISTICS";

                        dt.Columns["DTR_Failure_Entry"].ColumnName = "DTR FAILURE ENTRY IN DTLMS SOFTWARE  FY '" + cmbFinYear.SelectedValue + "' ";
                        dt.Columns["Balance_DTr_failure_to_be_entered_FY_2021-22"].ColumnName = "BALANCE_DTR_FAILURE_TO_BE_ENTERED_FY_'" + cmbFinYear.SelectedValue + "'";

                        dt.Columns["Pending_in_DTLMS_application"].ColumnName = "PENDING AT VARIOUS STAGES IN DTLMS APPLICATION";
                        dt.Columns["Total No of Existing DTRs"].ColumnName = "TOTAL NO OF EXISTING DTRS IN DTLMS SOFTWARE";

                        //dt.Columns["TOTAL"].ColumnName = "COMPLETED";
                        //dt.Columns["TOTALNEW"].ColumnName = "PENDING";

                        //dt.Columns["PENDING FOR LESSTHAN1DAY"].SetOrdinal(5);
                        //dt.Columns["PENDING FOR BW1TO7"].SetOrdinal(7);
                        //dt.Columns["PENDING FOR BW7TO15"].SetOrdinal(9);
                        //dt.Columns["PENDING FOR BW15TO30"].SetOrdinal(11);
                        //dt.Columns["PENDING FOR ABOVE30"].SetOrdinal(13);


                        wb.Worksheets.Add(dt, "ExistingDtrDetails");
                        wb.Worksheet(1).Row(1).InsertRowsAbove(2);
                        wb.Worksheet(1).Row(1).InsertRowsAbove(2);

                        string sMergeRange = arrAlpha[dt.Columns.Count - 1];

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue("Bangalore Electricity Supply Company Limitied, (BESCOM)");

                        wb.Worksheet(1).Cell(2, 8).Value = DateTime.Now;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "ExistingDTrDetails " + DateTime.Now + ".xls";
                        string pagetitle = "Details of Existing DTR Details during FY   '" + cmbFinYear.SelectedValue + "'";
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
                    ShowMsgBox("No Records Found");
                }

  




                //string sParam = "id=SchemeDetails&presentyear=" + presentyear + "&previosyear=" + previousyear + "&financialyear=" + cmbFinYear.SelectedValue + "";
                //RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + sParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");






            }
            catch (Exception ex)
            {
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
            }

        }

        private void ShowMsgBox(string sMsg)
        {
            try
            {
                string sShowMsg = string.Empty;
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                this.Page.RegisterStartupScript("Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
