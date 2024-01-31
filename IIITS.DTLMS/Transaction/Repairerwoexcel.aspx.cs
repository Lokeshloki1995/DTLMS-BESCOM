using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.IO;
using System.Net;
using System.Configuration;
using System.Data.OleDb;

namespace IIITS.DTLMS.Transaction
{
    public partial class Repairerwoexcel : System.Web.UI.Page
    {
        string strFormCode = "Repairerwoexcel";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //protected void cmdUpload_click(object sender, EventArgs e)
        //{
        //    try
        //    {


        //        if (fupUpload.PostedFile.ContentLength == 0)
        //        {
        //            ShowMsgBox("Please Select the File");
        //            fupUpload.Focus();
        //            return;
        //        }

        //        string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["UploadFormat"]);
        //        string sUploadFileExt = System.IO.Path.GetExtension(fupUpload.FileName).ToString().ToLower();
        //        sUploadFileExt = ";" + sUploadFileExt.Remove(0, 1) + ";";

        //        if (!sFileExt.Contains(sUploadFileExt))
        //        {
        //            ShowMsgBox("Invalid File Format");
        //            return;
        //        }
        //        string excelPath = Server.MapPath("~/DTLMSDocs/") + Path.GetFileName(fupUpload.PostedFile.FileName);
        //        fupUpload.SaveAs(excelPath);
        //        string conString = string.Empty;
        //        string extension = Path.GetExtension(fupUpload.PostedFile.FileName);
        //        switch (extension)
        //        {
        //            case ".xls": //Excel 97-03
        //                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
        //                break;
        //            case ".xlsx": //Excel 07 or higher
        //                conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
        //                break;
        //        }
        //        conString = string.Format(conString, excelPath);
        //        DataTable dtExcelData = new DataTable();
        //        using (OleDbConnection excel_con = new OleDbConnection(conString))
        //        {
        //            excel_con.Open();
        //            string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[3]["TABLE_NAME"].ToString();



        //            using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
        //            {
        //                oda.Fill(dtExcelData);

        //            }

        //            ClsExcelUpload obj = new ClsExcelUpload();
        //            //string Userid = objSession.UserId;
        //            string result = obj.ExecuteUploadExeclworkordercreation(dtExcelData);
        //            if (result == "1")
        //            {
        //                ShowMsgBox("Uploaded Successfully");
        //            }
        //            else
        //            {
        //                ShowMsgBox("Something went Wrong Please Try again....!!!");
        //            }
        //            excel_con.Close();
        //            // System.IO.File.Delete(excelPath);



        //        }




        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        ShowMsgBox("Something went Wrong Please Try again....!!!");

        //    }
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