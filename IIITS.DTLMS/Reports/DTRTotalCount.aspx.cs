using ClosedXML.Excel;
using IIITS.DTLMS.BL;
using IIITS.PGSQL.DAL;
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
    public partial class DTRTotalCount : System.Web.UI.Page
    {
       static string strFormCode = "DTRTotalCount";
        clsSession objSession;

        int Zone_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Zone_code"]);
        int Circle_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Circle_code"]);
        int Division_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);

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
                    objSession = (clsSession)Session["clsSession"];
                    string stroffCode = string.Empty;

                    if (objSession.OfficeCode.Length <= 2 && objSession.OfficeCode.Length != 0)
                    {
                        stroffCode = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId).Substring(0, Constants.Circle);
                    }
                    else
                    {
                        stroffCode = objSession.OfficeCode;
                    }
                    string stroffCode1 = stroffCode;

                    if (stroffCode == null || stroffCode == "")
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                    }

                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                        stroffCode = stroffCode.Substring(0, Zone_code);
                        cmbZone.Items.FindByValue(stroffCode).Selected = true;
                        cmbZone.Enabled = false;
                        stroffCode = stroffCode1;
                    }
                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);

                        if (stroffCode.Length >= 2)
                        {
                            stroffCode = stroffCode.Substring(0, Circle_code);
                            cmbCircle.Items.FindByValue(stroffCode).Selected = true;
                            cmbCircle.Enabled = false;
                            stroffCode = stroffCode1;

                        }
                    }

                    if (stroffCode.Length >= 2)
                    {
                        Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);
                        if (stroffCode.Length >= 3)
                        {
                            stroffCode = stroffCode.Substring(0, Division_code);
                            cmbDiv.Items.FindByValue(stroffCode).Selected = true;
                            cmbDiv.Enabled = false;
                            stroffCode = stroffCode1;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbZone.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);
                    cmbDiv.Items.Clear();

                }

                else
                {
                    cmbCircle.Items.Clear();
                    cmbDiv.Items.Clear();


                }
            }
            catch (Exception ex)
            {
              //  lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public static string GetZone_Circle_Div_Offcode(string sOfficeCode, string sRoleID)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            string offCode = string.Empty;
            try
            {
                if (sRoleID.Equals("2") || sRoleID.Equals("5"))
                {
                    string sQry = string.Empty;
                    sQry = "SELECT \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" = '" + sOfficeCode + "' limit 1 ";
                    offCode = objcon.get_value(sQry);
                    return offCode;
                }
                else
                {
                    return sOfficeCode;
                }
            }
            catch (Exception ex)
            {
                // clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                   clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

                return offCode;
            }
        }
        public string GetOfficeID()
        {
            string strOfficeId = string.Empty;

            if (cmbZone.SelectedIndex > 0)
            {
                strOfficeId = cmbZone.SelectedValue.ToString();
            }
            if (cmbCircle.SelectedIndex > 0)
            {
                strOfficeId = cmbCircle.SelectedValue.ToString();
            }
            if (cmbDiv.SelectedIndex > 0 )
            {
                strOfficeId = cmbDiv.SelectedValue.ToString();
            }
            else 
            {
                strOfficeId = clsStoreOffice.GetStoreID(cmbDiv.SelectedValue.ToString());
              //  strOfficeId = clsStoreOffice.GetStoreID(cmbSec.SelectedValue.ToString());
            }


            return (strOfficeId);
        }
        protected void cmbCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCircle.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);

                }

                else
                {
                    cmbDiv.Items.Clear();


                }
            }
            catch (Exception ex)
            {
               // lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
              

            }
            catch (Exception ex)
            {
              //  lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
       

        //public void ExportExcel(DataSet ds)        //{

        //    //File Path To Read Excel
        //    int k, RowCount, ColCount;        //    int l = 5;        //    string ExcelPath = Server.MapPath("~") + "ExcelWorkbook\\ConsolidatedTBReport.xlsx";        //    MemoryStream ms = new MemoryStream();        //    FileInfo file = new FileInfo(ExcelPath);        //    try        //    {        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;        //        using (ExcelPackage excelPackage = new ExcelPackage(file))        //        {        //            ExcelWorkbook excelWorkBook = excelPackage.Workbook;        //            ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets["PART-A"];        //            if (ds.Tables[0].Rows.Count > 0)        //            {        //                k = 7;        //                RowCount = ds.Tables[0].Rows.Count;//Number Of rows
        //                ColCount = ds.Tables[0].Columns.Count;        //                for (int i = 1; i <= RowCount; i++)        //                {        //                    for (int j = 1; j <= ColCount; j++)        //                    {        //                        excelWorksheet.Cells[k, j].Value = ds.Tables[0].Rows[i - 1][j - 1];        //                        excelWorksheet.Cells[k, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;        //                    }        //                    excelWorksheet.Cells[k, ColCount + 1].Formula = "=sum(" + excelWorksheet.Cells[k, 3].Address + ":" + excelWorksheet.Cells[k, ColCount].Address + ")";        //                    excelWorksheet.Cells[k, ColCount + 1].Style.Font.Bold = true;        //                    k++;        //                }        //                excelWorksheet.Cells[k, 2].Value = "Total-";        //                excelWorksheet.Cells[k, 2].Style.Font.Bold = true;        //                for (int i = 1; i <= ds.Tables[0].Columns.Count; i++)        //                {        //                    if (i > 2)        //                    {        //                        excelWorksheet.Cells[k, i].Formula = "=sum(" + excelWorksheet.Cells[7, i].Address + ":" + excelWorksheet.Cells[k - 1, i].Address + ")";        //                        excelWorksheet.Cells[k, i].Style.Font.Bold = true;        //                    }        //                    excelWorksheet.Column(i).AutoFit(20, 60);        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B1:L1"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B1:L1"].Value = "TRIPURA STATE ELECTRICITY  CORPORATION LIMITED";        //                    excelWorksheet.Cells["B1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B1:L1"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["A2:B2"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["A2:B2"].Value = "Time - " + DateTime.Now;        //                    excelWorksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["A2:B2"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B3:L3"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B3:L3"].Value = " FINAL TRIAL BALANCE FOR THE YEAR " + YearName;        //                    excelWorksheet.Cells["B3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B3:L3"].Style.Font.Bold = true;        //                }        //            }        //            if (ds.Tables[1].Rows.Count > 0)        //            {        //                k = 7;        //                excelWorksheet = excelWorkBook.Worksheets["PART-B"];        //                RowCount = ds.Tables[1].Rows.Count;//Number Of rows
        //                ColCount = ds.Tables[1].Columns.Count;        //                for (int i = 1; i <= RowCount; i++)        //                {        //                    for (int j = 1; j <= ColCount; j++)        //                    {
        //                        //Assign each values to rows

        //                        excelWorksheet.Cells[k, j].Value = ds.Tables[1].Rows[i - 1][j - 1];        //                        excelWorksheet.Cells[k, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;        //                    }        //                    excelWorksheet.Cells[k, ColCount + 1].Formula = "=sum(" + excelWorksheet.Cells[k, 3].Address + ":" + excelWorksheet.Cells[k, ColCount].Address + ")";        //                    excelWorksheet.Cells[k, ColCount + 1].Style.Font.Bold = true;        //                    k++;        //                }        //                excelWorksheet.Cells[k, 2].Value = "Total-";        //                excelWorksheet.Cells[k, 2].Style.Font.Bold = true;        //                for (int i = 1; i <= ds.Tables[1].Columns.Count; i++)        //                {        //                    if (i > 2)        //                    {        //                        excelWorksheet.Cells[k, i].Formula = "=sum(" + excelWorksheet.Cells[7, i].Address + ":" + excelWorksheet.Cells[k - 1, i].Address + ")";        //                        excelWorksheet.Cells[k, i].Style.Font.Bold = true;        //                    }        //                    excelWorksheet.Column(i).AutoFit(20, 60);        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B1:L1"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B1:L1"].Value = "TRIPURA STATE ELECTRICITY  CORPORATION LIMITED";        //                    excelWorksheet.Cells["B1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B1:L1"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["A2:B2"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["A2:B2"].Value = "Time - " + DateTime.Now;        //                    excelWorksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["A2:B2"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B3:L3"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B3:L3"].Value = " FINAL TRIAL BALANCE FOR THE YEAR " + YearName;        //                    excelWorksheet.Cells["B3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B3:L3"].Style.Font.Bold = true;        //                }        //            }        //            if (ds.Tables[2].Rows.Count > 0)        //            {        //                k = 7;        //                excelWorksheet = excelWorkBook.Worksheets["PART-C"];        //                RowCount = ds.Tables[2].Rows.Count;//Number Of rows
        //                ColCount = ds.Tables[2].Columns.Count;        //                for (int i = 1; i <= RowCount; i++)        //                {        //                    for (int j = 1; j <= ColCount; j++)        //                    {
        //                        //if (i == 1)
        //                        //{
        //                        //    excelWorksheet.Cells[l, j].Value = columnNamesBelow[j - 1];
        //                        //    excelWorksheet.Cells[l, j].Style.Font.Bold = true;
        //                        //    excelWorksheet.Cells[l, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                        //    continue;
        //                        //}

        //                        //Assign each values to rows

        //                        excelWorksheet.Cells[k, j].Value = ds.Tables[2].Rows[i - 1][j - 1];        //                        excelWorksheet.Cells[k, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;        //                    }        //                    excelWorksheet.Cells[k, ColCount + 1].Formula = "=sum(" + excelWorksheet.Cells[k, 3].Address + ":" + excelWorksheet.Cells[k, ColCount].Address + ")";        //                    excelWorksheet.Cells[k, ColCount + 1].Style.Font.Bold = true;        //                    k++;        //                }        //                for (int i = 1; i <= ds.Tables[2].Columns.Count; i++)        //                {        //                    if (i > 2)        //                    {        //                        excelWorksheet.Cells[k, i].Formula = "=sum(" + excelWorksheet.Cells[7, i].Address + ":" + excelWorksheet.Cells[k - 1, i].Address + ")";        //                        excelWorksheet.Cells[k, i].Style.Font.Bold = true;        //                    }        //                    excelWorksheet.Column(i).AutoFit(20, 60);        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B1:L1"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B1:L1"].Value = "TRIPURA STATE ELECTRICITY  CORPORATION LIMITED";        //                    excelWorksheet.Cells["B1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B1:L1"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["A2:B2"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["A2:B2"].Value = "Time - " + DateTime.Now;        //                    excelWorksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["A2:B2"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B3:L3"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B3:L3"].Value = " FINAL TRIAL BALANCE FOR THE YEAR " + YearName;        //                    excelWorksheet.Cells["B3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B3:L3"].Style.Font.Bold = true;        //                }        //            }        //            if (ds.Tables[3].Rows.Count > 0)        //            {        //                k = 7;        //                excelWorksheet = excelWorkBook.Worksheets["PART-D"];        //                excelWorksheet.Name = "Abstract of Part A, B & C";        //                RowCount = ds.Tables[3].Rows.Count;//Number Of rows
        //                ColCount = ds.Tables[3].Columns.Count;        //                for (int i = 1; i <= RowCount; i++)        //                {        //                    for (int j = 1; j <= ColCount; j++)        //                    {


        //                        excelWorksheet.Cells[k, j].Value = ds.Tables[3].Rows[i - 1][j - 1];        //                        excelWorksheet.Cells[k, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;        //                    }        //                    excelWorksheet.Cells[k, ColCount + 1].Formula = "=sum(" + excelWorksheet.Cells[k, 2].Address + ":" + excelWorksheet.Cells[k, ColCount].Address + ")";        //                    excelWorksheet.Cells[k, ColCount + 1].Style.Font.Bold = true;        //                    k++;        //                }        //                for (int i = 1; i <= ds.Tables[3].Columns.Count; i++)        //                {        //                    excelWorksheet.Column(i).AutoFit(20, 60);        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B1:L1"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B1:L1"].Value = "TRIPURA STATE ELECTRICITY  CORPORATION LIMITED";        //                    excelWorksheet.Cells["B1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B1:L1"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["A2:B2"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["A2:B2"].Value = "Time - " + DateTime.Now;        //                    excelWorksheet.Cells["A2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["A2:B2"].Style.Font.Bold = true;        //                }        //                using (ExcelRange rng = excelWorksheet.Cells["B3:L3"])        //                {        //                    rng.Merge = true;        //                    excelWorksheet.Cells["B3:L3"].Value = " FINAL TRIAL BALANCE FOR THE YEAR " + YearName;        //                    excelWorksheet.Cells["B3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;        //                    excelWorksheet.Cells["B3:L3"].Style.Font.Bold = true;        //                }        //            }





        //            //Export Excel- download excel
        //            Response.Clear();        //            Response.Buffer = true;        //            Response.Charset = "";        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";        //            Response.AddHeader("content-disposition", "attachment;filename=ConsolidatedTBReport_" + DateTime.Now.ToString("dd_MM_yyyy HH-mm-ss") + ".xlsx");        //            using (MemoryStream MyMemoryStream = new MemoryStream())        //            {        //                excelPackage.SaveAs(MyMemoryStream);        //                MyMemoryStream.WriteTo(Response.OutputStream);        //                Response.Flush();        //                Response.End();        //            }        //        }        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

        //    }        //}


        protected void Export_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                clsReports objrep = new clsReports();

                if (cmbDiv.SelectedIndex > 0)
                {
                    objrep.sOfficeCode = cmbDiv.SelectedValue;
                }
                else if (cmbCircle.SelectedIndex > 0)
                {
                    objrep.sOfficeCode = cmbCircle.SelectedValue;
                }
                else if (cmbZone.SelectedIndex > 0)
                {
                    objrep.sOfficeCode = cmbZone.SelectedValue;
                }

                //else
                //{
                //    string strofficecode = GetOfficeID();
                //    objrep.sOfficeCode = strofficecode;

                //}

                dt = objrep.GetDTRFldTotalCount(objrep);
             //   dt1 = objrep.GetDTRStrTotalCount(objrep);
           //     dt2 = objrep.GetDTRRprTotalCount(objrep);
            //    if (dt.Rows.Count > 0)
           //     {
                    if (dt.Rows.Count > 0)
                    {
                    string[] arrAlpha = Genaral.getalpha();
                    string sMergeRange = arrAlpha[dt.Columns.Count - 1];
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                       // dt.Columns["LOCATION"].ColumnName = "LOCATION";
                        dt.Columns["ZONE."].ColumnName = "ZONE";
                        dt.Columns["CIRCLE"].ColumnName = "CIRCLE ";
                        dt.Columns["DIVISION"].ColumnName = "DIVISION ";
                        dt.Columns["SUBDIVISION"].ColumnName = "SUBDIVISION";
                        dt.Columns["SECTION"].ColumnName = "SECTION";

                        dt.Columns["10 KVA"].ColumnName = "10 KVA";
                        dt.Columns["15 KVA"].ColumnName = "15 KVA";
                        dt.Columns["25 KVA"].ColumnName = "25 KVA";
                        dt.Columns["50 KVA"].ColumnName = "50 KVA";
                        dt.Columns["63 KVA"].ColumnName = "63 KVA";
                        dt.Columns["100 KVA"].ColumnName = "100 KVA";
                        dt.Columns["125 KVA"].ColumnName = "125 KVA";
                        dt.Columns["150 KVA"].ColumnName = "150 KVA";
                        dt.Columns["160 KVA"].ColumnName = "160 KVA";
                        dt.Columns["200 KVA"].ColumnName = "200 KVA";
                        dt.Columns["250 KVA"].ColumnName = "250 KVA";
                        dt.Columns["300 KVA"].ColumnName = "300 KVA";
                        dt.Columns["315 KVA"].ColumnName = "315 KVA";
                        dt.Columns["400 KVA"].ColumnName = "400 KVA";
                        dt.Columns["500 KVA"].ColumnName = "500 KVA";
                        dt.Columns["630 KVA"].ColumnName = "630 KVA";
                        dt.Columns["750 KVA"].ColumnName = "750 KVA";
                        dt.Columns["800 KVA"].ColumnName = "800 KVA";
                        dt.Columns["960 KVA"].ColumnName = "960 KVA";
                        dt.Columns["1000 KVA"].ColumnName = "1000 KVA";
                        dt.Columns["1250 KVA"].ColumnName = "1250 KVA";
                        dt.Columns["NO OF TC"].ColumnName = "Total NO OF TC";

                        wb.Worksheets.Add(dt, "DTR_AT_FIELD");

                        wb.Worksheet(1).Row(1).InsertRowsAbove(3);

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue("Bangalore Electricity Supply Board, (BESCOM)");

                        var rangeReporthaed = wb.Worksheet(1).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed.SetValue("List of DTR Field Count Details ");
                        wb.Worksheet(1).Cell(3, 10).Value = DateTime.Now;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "DTRTotalCountAtField " + DateTime.Now + ".xls";
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

                    //List<string> listtoRemove = new List<string> { };
                    //    string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //    string pagetitle = "DTRTotalFieldCount";

                    //    Genaral.getexcel(dt, listtoRemove, filename, pagetitle);

                    }
                    //if (dt1.Rows.Count > 0)
                    //{
                    //    dt1.Columns["STORE_NAME"].ColumnName = "STORE NAME";
                    //    dt1.Columns["NO OF CAPACITY"].ColumnName = "NO OF TC";
                    //    dt1.Columns["10 KVA"].ColumnName = "10 KVA";
                    //    dt1.Columns["15 KVA"].ColumnName = "15 KVA";
                    //    dt1.Columns["25 KVA"].ColumnName = "25 KVA";
                    //    dt1.Columns["50 KVA"].ColumnName = "50 KVA";
                    //    dt1.Columns["63 KVA"].ColumnName = "63 KVA";
                    //    dt1.Columns["100 KVA"].ColumnName = "100 KVA";
                    //    dt1.Columns["125 KVA"].ColumnName = "125 KVA";
                    //    dt1.Columns["150 KVA"].ColumnName = "150 KVA";
                    //    dt1.Columns["160 KVA"].ColumnName = "160 KVA";
                    //    dt1.Columns["200 KVA"].ColumnName = "200 KVA";
                    //    dt1.Columns["250 KVA"].ColumnName = "250 KVA";
                    //    dt1.Columns["300 KVA"].ColumnName = "300 KVA";
                    //    dt1.Columns["315 KVA"].ColumnName = "315 KVA";
                    //    dt1.Columns["400 KVA"].ColumnName = "400 KVA";
                    //    dt1.Columns["500 KVA"].ColumnName = "500 KVA";
                    //    dt1.Columns["630 KVA"].ColumnName = "630 KVA";
                    //    dt1.Columns["750 KVA"].ColumnName = "750 KVA";
                    //    dt1.Columns["800 KVA"].ColumnName = "800 KVA";
                    //    dt1.Columns["960 KVA"].ColumnName = "960 KVA";
                    //    dt1.Columns["1000 KVA"].ColumnName = "1000 KVA";
                    //    dt1.Columns["1250 KVA"].ColumnName = "1250 KVA";

                    //    List<string> listtoRemove = new List<string> { };
                    //    string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //    string pagetitle = "DTRTotalstoreCount";

                    //    Genaral.getexcel(dt, listtoRemove, filename, pagetitle);

                    //}
                    //if (dt2.Rows.Count > 0)
                    //{
                    //    dt2.Columns["STORE_NAME"].ColumnName = "STORE NAME";
                    //    dt2.Columns["NO OF CAPACITY"].ColumnName = "NO OF TC";
                    //    dt2.Columns["10 KVA"].ColumnName = "10 KVA";
                    //    dt2.Columns["15 KVA"].ColumnName = "15 KVA";
                    //    dt2.Columns["25 KVA"].ColumnName = "25 KVA";
                    //    dt2.Columns["50 KVA"].ColumnName = "50 KVA";
                    //    dt2.Columns["63 KVA"].ColumnName = "63 KVA";
                    //    dt2.Columns["100 KVA"].ColumnName = "100 KVA";
                    //    dt2.Columns["125 KVA"].ColumnName = "125 KVA";
                    //    dt2.Columns["150 KVA"].ColumnName = "150 KVA";
                    //    dt2.Columns["160 KVA"].ColumnName = "160 KVA";
                    //    dt2.Columns["200 KVA"].ColumnName = "200 KVA";
                    //    dt2.Columns["250 KVA"].ColumnName = "250 KVA";
                    //    dt2.Columns["300 KVA"].ColumnName = "300 KVA";
                    //    dt2.Columns["315 KVA"].ColumnName = "315 KVA";
                    //    dt2.Columns["400 KVA"].ColumnName = "400 KVA";
                    //    dt2.Columns["500 KVA"].ColumnName = "500 KVA";
                    //    dt2.Columns["630 KVA"].ColumnName = "630 KVA";
                    //    dt2.Columns["750 KVA"].ColumnName = "750 KVA";
                    //    dt2.Columns["800 KVA"].ColumnName = "800 KVA";
                    //    dt2.Columns["960 KVA"].ColumnName = "960 KVA";
                    //    dt2.Columns["1000 KVA"].ColumnName = "1000 KVA";
                    //    dt2.Columns["1250 KVA"].ColumnName = "1250 KVA";

                    //    List<string> listtoRemove = new List<string> { };
                    //    string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //    string pagetitle = "DTRTotalrepairerCount";

                    //    Genaral.getexcel(dt, listtoRemove, filename, pagetitle);
                    //}
             //   }
                else
                {
                    ShowMsgBox("No record found");
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }


        }
        protected void Export_Click1(object sender, EventArgs e)
        {
            try
            {
                DataTable dt1 = new DataTable();
                clsReports objrep = new clsReports();

                //string strofficecode = GetOfficeID();
                //objrep.sOfficeCode = strofficecode;

                dt1 = objrep.GetDTRStrTotalCount(objrep);
                if (dt1.Rows.Count > 0)
                {
                    string[] arrAlpha = Genaral.getalpha();
                    string sMergeRange = arrAlpha[dt1.Columns.Count - 1];
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                    dt1.Columns["STORE_NAME"].ColumnName = "STORE NAME";
                    dt1.Columns["10 KVA"].ColumnName = "10 KVA";
                    dt1.Columns["15 KVA"].ColumnName = "15 KVA";
                    dt1.Columns["25 KVA"].ColumnName = "25 KVA";
                    dt1.Columns["50 KVA"].ColumnName = "50 KVA";
                    dt1.Columns["63 KVA"].ColumnName = "63 KVA";
                    dt1.Columns["100 KVA"].ColumnName = "100 KVA";
                    dt1.Columns["125 KVA"].ColumnName = "125 KVA";
                    dt1.Columns["150 KVA"].ColumnName = "150 KVA";
                    dt1.Columns["160 KVA"].ColumnName = "160 KVA";
                    dt1.Columns["200 KVA"].ColumnName = "200 KVA";
                    dt1.Columns["250 KVA"].ColumnName = "250 KVA";
                    dt1.Columns["300 KVA"].ColumnName = "300 KVA";
                    dt1.Columns["315 KVA"].ColumnName = "315 KVA";
                    dt1.Columns["400 KVA"].ColumnName = "400 KVA";
                    dt1.Columns["500 KVA"].ColumnName = "500 KVA";
                    dt1.Columns["630 KVA"].ColumnName = "630 KVA";
                    dt1.Columns["750 KVA"].ColumnName = "750 KVA";
                    dt1.Columns["800 KVA"].ColumnName = "800 KVA";
                    dt1.Columns["960 KVA"].ColumnName = "960 KVA";
                    dt1.Columns["1000 KVA"].ColumnName = "1000 KVA";
                    dt1.Columns["1250 KVA"].ColumnName = "1250 KVA";
                    dt1.Columns["NO OF CAPACITY"].ColumnName = "Total NO OF TC";

                        wb.Worksheets.Add(dt1, "DTR_At_Store");

                        wb.Worksheet(1).Row(1).InsertRowsAbove(3);

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue("Bangalore Electricity Supply Board, (BESCOM)");

                        var rangeReporthaed = wb.Worksheet(1).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed.SetValue("List of DTR Count Details ");
                        wb.Worksheet(1).Cell(3, 10).Value = DateTime.Now;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "DTRTotalCountAtStore " + DateTime.Now + ".xls";
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

                    //List<string> listtoRemove = new List<string> { };
                    //string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //string pagetitle = "DTRTotalstoreCount";

                    //Genaral.getexcel(dt1, listtoRemove, filename, pagetitle);

                }
                else
                {
                    ShowMsgBox("No record found");
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        protected void Export_Click2(object sender, EventArgs e)
        {
            try
            {
                DataTable dt2 = new DataTable();
                clsReports objrep = new clsReports();

                dt2 = objrep.GetDTRRprTotalCount(objrep);

                if (dt2.Rows.Count > 0)
                {
                    string[] arrAlpha = Genaral.getalpha();
                    string sMergeRange = arrAlpha[dt2.Columns.Count - 1];
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                    dt2.Columns["LOCATIONNAME"].ColumnName = "LOCATION";
                    dt2.Columns["10 KVA"].ColumnName = "10 KVA";
                    dt2.Columns["15 KVA"].ColumnName = "15 KVA";
                    dt2.Columns["25 KVA"].ColumnName = "25 KVA";
                    dt2.Columns["50 KVA"].ColumnName = "50 KVA";
                    dt2.Columns["63 KVA"].ColumnName = "63 KVA";
                    dt2.Columns["100 KVA"].ColumnName = "100 KVA";
                    dt2.Columns["125 KVA"].ColumnName = "125 KVA";
                    dt2.Columns["150 KVA"].ColumnName = "150 KVA";
                    dt2.Columns["160 KVA"].ColumnName = "160 KVA";
                    dt2.Columns["200 KVA"].ColumnName = "200 KVA";
                    dt2.Columns["250 KVA"].ColumnName = "250 KVA";
                    dt2.Columns["300 KVA"].ColumnName = "300 KVA";
                    dt2.Columns["315 KVA"].ColumnName = "315 KVA";
                    dt2.Columns["400 KVA"].ColumnName = "400 KVA";
                    dt2.Columns["500 KVA"].ColumnName = "500 KVA";
                    dt2.Columns["630 KVA"].ColumnName = "630 KVA";
                    dt2.Columns["750 KVA"].ColumnName = "750 KVA";
                    dt2.Columns["800 KVA"].ColumnName = "800 KVA";
                    dt2.Columns["960 KVA"].ColumnName = "960 KVA";
                    dt2.Columns["1000 KVA"].ColumnName = "1000 KVA";
                    dt2.Columns["1250 KVA"].ColumnName = "1250 KVA";
                    dt2.Columns["NO OF CAPACITY"].ColumnName = "Total NO OF TC";
                            
                        wb.Worksheets.Add(dt2, "DTR_At_Repairer");

                        wb.Worksheet(1).Row(1).InsertRowsAbove(3);

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue("Bangalore Electricity Supply Board, (BESCOM)");

                        var rangeReporthaed = wb.Worksheet(1).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed.SetValue("List of DTR Count Details ");
                        wb.Worksheet(1).Cell(3, 10).Value = DateTime.Now;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "DTRTotalCountAtRepirer " + DateTime.Now + ".xls";
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

                    //List<string> listtoRemove = new List<string> { };
                    //string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //string pagetitle = "DTRTotalrepairerCount";

                    //Genaral.getexcel(dt2, listtoRemove, filename, pagetitle);
                }
                else
                {
                    ShowMsgBox("No record found");
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        protected void Export_Click3(object sender, EventArgs e)
        {
            try
            {
                DataTable dt3 = new DataTable();
                clsReports objrep = new clsReports();

                dt3 = objrep.GetDTRTrnsBankTotalCount(objrep);

                if (dt3.Rows.Count > 0)
                {
                    string[] arrAlpha = Genaral.getalpha();
                    string sMergeRange = arrAlpha[dt3.Columns.Count - 1];
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        dt3.Columns["LOCATIONNAME"].ColumnName = "LOCATION";
                        dt3.Columns["10 KVA"].ColumnName = "10 KVA";
                        dt3.Columns["15 KVA"].ColumnName = "15 KVA";
                        dt3.Columns["25 KVA"].ColumnName = "25 KVA";
                        dt3.Columns["50 KVA"].ColumnName = "50 KVA";
                        dt3.Columns["63 KVA"].ColumnName = "63 KVA";
                        dt3.Columns["100 KVA"].ColumnName = "100 KVA";
                        dt3.Columns["125 KVA"].ColumnName = "125 KVA";
                        dt3.Columns["150 KVA"].ColumnName = "150 KVA";
                        dt3.Columns["160 KVA"].ColumnName = "160 KVA";
                        dt3.Columns["200 KVA"].ColumnName = "200 KVA";
                        dt3.Columns["250 KVA"].ColumnName = "250 KVA";
                        dt3.Columns["300 KVA"].ColumnName = "300 KVA";
                        dt3.Columns["315 KVA"].ColumnName = "315 KVA";
                        dt3.Columns["400 KVA"].ColumnName = "400 KVA";
                        dt3.Columns["500 KVA"].ColumnName = "500 KVA";
                        dt3.Columns["630 KVA"].ColumnName = "630 KVA";
                        dt3.Columns["750 KVA"].ColumnName = "750 KVA";
                        dt3.Columns["800 KVA"].ColumnName = "800 KVA";
                        dt3.Columns["960 KVA"].ColumnName = "960 KVA";
                        dt3.Columns["1000 KVA"].ColumnName = "1000 KVA";
                        dt3.Columns["1250 KVA"].ColumnName = "1250 KVA";
                        dt3.Columns["NO OF CAPACITY"].ColumnName = "Total NO OF TC";

                        wb.Worksheets.Add(dt3, "DTR_At_Transformer_Bank"); 

                        wb.Worksheet(1).Row(1).InsertRowsAbove(3);

                        var rangeheader = wb.Worksheet(1).Range("A1:" + sMergeRange + "1");
                        rangeheader.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        rangeheader.Merge().Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        rangeheader.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeheader.SetValue("Bangalore Electricity Supply Board, (BESCOM)");

                        var rangeReporthaed = wb.Worksheet(1).Range("A2:" + sMergeRange + "2");
                        rangeReporthaed.Merge().Style.Font.SetBold().Font.FontSize = 12;
                        rangeReporthaed.Merge().Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                        rangeReporthaed.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        rangeReporthaed.SetValue("List of DTR Count Details ");
                        wb.Worksheet(1).Cell(3, 10).Value = DateTime.Now;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        string FileName = "DTRTotalCountAtTransformerBank " + DateTime.Now + ".xls";
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

                    //List<string> listtoRemove = new List<string> { };
                    //string filename = "DTRTotalCount" + DateTime.Now + ".xls";
                    //string pagetitle = "DTRTotalrepairerCount";

                    //Genaral.getexcel(dt2, listtoRemove, filename, pagetitle);
                }
                else
                {
                    ShowMsgBox("No record found");
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        //public string GetOfficeID()
        //{
        //    string strOfficeId = string.Empty;
          
        //    strOfficeId = "";
        //    return (strOfficeId);
        //}
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