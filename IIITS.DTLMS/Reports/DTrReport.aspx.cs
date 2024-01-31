using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using System.Configuration;

namespace IIITS.DTLMS.Reports
{
    public partial class DTrReport : System.Web.UI.Page
    {

        string strFormCode = "DTrReport";
        clsSession objSession;
        int Zone_code = Convert.ToInt32(ConfigurationManager.AppSettings["Zone_code"]);
        int Circle_code = Convert.ToInt32(ConfigurationManager.AppSettings["Circle_code"]);
        int Division_code = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
        int SubDiv_code = Convert.ToInt32(ConfigurationManager.AppSettings["SubDiv_code"]);
        int Section_code = Convert.ToInt32(ConfigurationManager.AppSettings["Section_code"]);
        int Feeder_code = Convert.ToInt32(ConfigurationManager.AppSettings["feeder_code"]);
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
                    string strQry = string.Empty;
                    objSession = (clsSession)Session["clsSession"];
                    string stroffCode = string.Empty;
                    if (objSession.OfficeCode.Length <= 1 && objSession.OfficeCode.Length != 0)
                    {
                        stroffCode = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId).Substring(0, Constants.Zone);
                    }
                    else if (objSession.OfficeCode.Length <= 2 && objSession.OfficeCode.Length != 0)
                    {
                        stroffCode = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId).Substring(0, Constants.Circle);
                    }
                    else
                    {
                        stroffCode = objSession.OfficeCode;
                    }
                    string stroffCode1 = stroffCode;
                    strQry = "SELECT \"TM_ID\",\"TM_NAME\" FROM \"TBLTRANSMAKES\" ORDER BY \"TM_NAME\"";
                    Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='TCL'", "--Select--", cmbLocation);
                    Genaral.Load_Combo("SELECT \"TM_ID\",\"TM_NAME\" FROM \"TBLTRANSMAKES\" ORDER BY \"TM_NAME\"", "--Select--", cmbMake);
                    Genaral.Load_Combo("SELECT \"MD_ORDER_BY\",\"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='C' ORDER BY \"MD_ORDER_BY\"", "--Select--", cmbCapacity);
                    cmbSubDiv.Visible = false;
                    cmbSec.Visible = false;
                    cmbFeederName.Visible = false;
                    lblSubDiv.Visible = false;
                    lblSec.Visible = false;
                    lblfed.Visible = false;
                    if (stroffCode == null || stroffCode == "")
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbAbstractZone);
                    }
                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbAbstractZone);
                        stroffCode = stroffCode.Substring(0, Zone_code);
                        cmbZone.Items.FindByValue(stroffCode).Selected = true;
                        cmbZone.Enabled = false;
                        cmbAbstractZone.Items.FindByValue(stroffCode).Selected = true;
                        cmbAbstractZone.Enabled = false;
                        stroffCode = stroffCode1;
                    }
                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'","--Select--", cmbCircle);
                        Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbAbstractZone.SelectedValue + "'","--Select--", cmbAbstractCircle);
                        if (stroffCode.Length >= 2)
                        {
                            stroffCode = stroffCode.Substring(0, Circle_code);
                            cmbCircle.Items.FindByValue(stroffCode).Selected = true;
                            cmbCircle.Enabled = false;
                            cmbAbstractCircle.Items.FindByValue(stroffCode).Selected = true;
                            cmbAbstractCircle.Enabled = false;
                            stroffCode = stroffCode1;
                        }
                    }

                    if (stroffCode.Length >= 2)
                    {
                        Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbCircle.SelectedValue + "'","--Select--", cmbDiv);
                        Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbAbstractCircle.SelectedValue + "'","--Select--", cmbAbstractDivision);
                        if (stroffCode.Length >= 3)
                        {
                            stroffCode = stroffCode.Substring(0, Division_code);
                            cmbDiv.Items.FindByValue(stroffCode).Selected = true;
                            cmbDiv.Enabled = false;
                            cmbAbstractDivision.Items.FindByValue(stroffCode).Selected = true;
                            cmbAbstractDivision.Enabled = false;
                            stroffCode = stroffCode1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// to generate crystal report based on conditions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdReport_Click(object sender, EventArgs e)
        {
            try
            {
                string strOfficeCode = string.Empty;
                string strParam = string.Empty;
                string strValue = string.Empty;
                string stroffcode = string.Empty;

                clsReports objReport = new clsReports();
                if (cmbLocation.SelectedIndex != 2)
                {
                    if (cmbZone.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select Zone");
                        cmbZone.Focus();
                        return;
                    }
                    if (cmbCircle.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select Circle");
                        cmbCircle.Focus();
                        return;
                    }
                    if (cmbDiv.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select Division");
                        cmbDiv.Focus();
                        return;
                    }
                }

                if (cmbCapacity.SelectedIndex != 0)
                {
                    objReport.sCapacity = cmbCapacity.SelectedItem.Text;
                }
                if (cmbMake.SelectedIndex != 0)
                {
                    objReport.sMake = cmbMake.SelectedValue;
                }
                if (cmbCapacity.SelectedIndex != 0)
                {
                    objReport.sCapacity = cmbCapacity.SelectedValue;
                }
                if (cmbLocation.SelectedIndex != 0)
                {
                    objReport.sLocType = cmbLocation.SelectedValue;
                }
                if (cmbFeederName.SelectedIndex != 0)
                {
                    objReport.sFeeder = cmbFeederName.SelectedValue;
                }

                if (cmbSec.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbSec.SelectedValue;
                }
                else if (cmbSubDiv.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbSubDiv.SelectedValue;
                }
                else if (cmbDiv.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbDiv.SelectedValue;
                }
                else if (cmbCircle.SelectedIndex != 0)
                {
                    objReport.sOfficeCode = cmbCircle.SelectedValue;
                }
                else if (cmbZone.SelectedIndex > -1)
                {
                    objReport.sOfficeCode = cmbZone.SelectedValue;
                }
                else
                {
                    stroffcode = GetOfficeID();
                    objReport.sOfficeCode = stroffcode;
                }
                strParam = "id=DTrReportMake&OfficeCode=" + objReport.sOfficeCode + "&FeederName=" + objReport.sFeeder + "&Make=" + objReport.sMake
                    + "&Capacity=" + objReport.sCapacity + "&Location=" + objReport.sLocType;
                ClientScript.RegisterStartupScript(this.GetType(),"Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// to Reset the fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdReset_Click(object sender, EventArgs e)
        {
            try
            {
                cmbMake.SelectedIndex = 0;
                cmbCapacity.SelectedIndex = 0;
                cmbMake.SelectedIndex = 0;
                cmbCapacity.SelectedIndex = 0;
                cmbCapacity.SelectedIndex = 0;
                cmbFeederName.SelectedValue = null;
                cmbLocation.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// on change function for zone dropdown to load circle details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbZone.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'","--Select--", cmbCircle);
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
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// on change function for circle dropdown to load division details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCircle.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbCircle.SelectedValue + "'","--Select--", cmbDiv);
                }
                else
                {
                    cmbDiv.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// on change function to filter based on Location type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbLocation.SelectedIndex == 2)
                {
                    cmbSubDiv.Visible = true;
                    cmbSec.Visible = true;
                    cmbFeederName.Visible = true;
                    lblSubDiv.Visible = true;
                    lblSec.Visible = true;
                    lblfed.Visible = true;
                    if (cmbDiv.SelectedIndex > 0)
                    {
                        Genaral.Load_Combo("SELECT \"SD_SUBDIV_CODE\",\"SD_SUBDIV_NAME\" FROM \"TBLSUBDIVMAST\" WHERE CAST(\"SD_DIV_CODE\" AS TEXT)='" + cmbDiv.SelectedValue + "'","--Select--", cmbSubDiv);
                    }
                    else
                    {
                        cmbSubDiv.Items.Clear();
                        cmbSec.Items.Clear();
                        cmbFeederName.Items.Clear();
                    }
                }
                else
                {
                    cmbSubDiv.Visible = false;
                    cmbSec.Visible = false;
                    cmbFeederName.Visible = false;
                    lblSubDiv.Visible = false;
                    lblSec.Visible = false;
                    lblfed.Visible = false;
                    cmbSubDiv.Items.Clear();
                    cmbSec.Items.Clear();
                    cmbFeederName.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// on change function for subdivision dropdown to load section details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbSubDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSubDiv.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"OM_CODE\",\"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE  CAST(\"OM_SUBDIV_CODE\" AS TEXT)='" + cmbSubDiv.SelectedValue + "'","--Select--", cmbSec);
                }
                else
                {
                    cmbSec.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// on change function for section dropdown to load feeder details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbsec_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSec.SelectedIndex > 0)
                {
                    string qry = "SELECT \"FD_FEEDER_CODE\",\"FD_FEEDER_NAME\" FROM \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" where \"FDO_FEEDER_ID\"= ";
                    qry += " \"FD_FEEDER_ID\" and CAST(\"FDO_OFFICE_CODE\"AS TEXT) like '" + cmbSubDiv.SelectedValue + "%'  ORDER BY \"FD_FEEDER_CODE\" ";
                    Genaral.Load_Combo(qry, "--Select--", cmbFeederName);
                }
                else
                {
                    cmbFeederName.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        ///  to get store id from division office code
        /// </summary>
        /// <returns></returns>
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
            if (cmbDiv.SelectedIndex > 0 && cmbLocation.SelectedValue == "2")
            {
                strOfficeId = cmbDiv.SelectedValue.ToString();
            }
            else
            {
                strOfficeId = clsStoreOffice.GetStoreID(cmbDiv.SelectedValue.ToString());
            }
            return (strOfficeId);
        }
        /// <summary>
        /// to show alert messages based on conditions
        /// </summary>
        /// <param name="sMsg"></param>
        private void ShowMsgBox(string sMsg)
        {
            try
            {
                string sShowMsg = string.Empty;
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// to generate export excel based on fillering conditions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Export_click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            clsReports objReport = new clsReports();
            if (cmbMake.SelectedIndex != 0)
            {
                objReport.sMake = cmbMake.SelectedValue;
            }
            if (cmbCapacity.SelectedIndex != 0)
            {
                objReport.sCapacity = cmbCapacity.SelectedValue;
            }
            if (cmbLocation.SelectedIndex != 0)
            {
                objReport.sLocType = cmbLocation.SelectedValue;
            }
            if (cmbSec.SelectedIndex > 0)
            {
                objReport.sOfficeCode = cmbSec.SelectedValue;
            }
            else if (cmbSubDiv.SelectedIndex > 0)
            {
                objReport.sOfficeCode = cmbSubDiv.SelectedValue;
            }
            else if (cmbDiv.SelectedIndex > 0)
            {
                objReport.sOfficeCode = cmbDiv.SelectedValue;
            }
            else if (cmbCircle.SelectedIndex != 0)
            {
                objReport.sOfficeCode = cmbCircle.SelectedValue;
            }
            else if (cmbZone.SelectedIndex > -1)
            {
                objReport.sOfficeCode = cmbZone.SelectedValue;
            }
            else
            {
                string stroffcode = GetOfficeID();
                objReport.sOfficeCode = stroffcode;
            }
            dt = objReport.PrintDTrReport(objReport);

            if (dt.Rows.Count > 0)
            {
                string[] arrAlpha = Genaral.getalpha();
                string sMergeRange = arrAlpha[dt.Columns.Count - 1];
                using (XLWorkbook wb = new XLWorkbook())
                {
                    dt.Columns["TC_CODE"].ColumnName = "DTR Code";
                    dt.Columns["TC_SLNO"].ColumnName = "Tc Sl No";
                    dt.Columns["TC_MAKE_ID"].ColumnName = "Make Name";
                    dt.Columns["TC_CAPACITY"].ColumnName = "Capacity";
                    dt.Columns["TC_MANF_DATE"].ColumnName = "Manufacturing Date";
                    dt.Columns["LOCATIONNAME"].ColumnName = "Tc Current Location";
                    dt.Columns["CIRCLE"].SetOrdinal(0);
                    dt.Columns["CIRCLE"].ColumnName = "Circle";
                    dt.Columns["DIVISION"].SetOrdinal(1);
                    dt.Columns["DIVISION"].ColumnName = "Division";
                    dt.Columns["SUBDIVISION"].SetOrdinal(2);
                    dt.Columns["SUBDIVISION"].ColumnName = "SubDivision";
                    dt.Columns["SECTION"].SetOrdinal(3);
                    dt.Columns["SECTION"].ColumnName = "Section";
                    dt.Columns["FEEDER"].ColumnName = "FeederName";

                    wb.Worksheets.Add(dt, "DtrReport");

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
                    rangeReporthaed.SetValue("List of Transformers with details ");

                    wb.Worksheet(1).Cell(3, 10).Value = DateTime.Now;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    string FileName = "DtrReport" + DateTime.Now + ".xls";
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
        }

       
        /// <summary>
        ///  to show Abstract dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdDtrAbstract_Click(object sender, EventArgs e)
        {
            clsReports objReport = new clsReports();
            DataTable dtAbstractDTrDetails = new DataTable();
            try
            {
                if (cmbAbstractSection.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbAbstractSection.SelectedValue;
                }
                else if (cmbAbstractSubDiv.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbAbstractSubDiv.SelectedValue;
                }
                else if (cmbAbstractDivision.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbAbstractDivision.SelectedValue;
                }
                else if (cmbAbstractCircle.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbAbstractCircle.SelectedValue;
                }
                else if (cmbAbstractZone.SelectedIndex > 0)
                {
                    objReport.sOfficeCode = cmbAbstractZone.SelectedValue;
                }
                dtAbstractDTrDetails = objReport.getAbstractDtrDetails(objReport.sOfficeCode);
                ViewState["AbstractDTrDetails"] = dtAbstractDTrDetails;

                double totalSalary = 0;
                foreach (DataRow dr in dtAbstractDTrDetails.Rows)
                {
                    totalSalary += Convert.ToInt32(dr["TOTAL_COUNT"]);
                }
                grdAbstractDtrDetails.Columns[0].FooterText = "Total";
                grdAbstractDtrDetails.Columns[0].FooterStyle.ForeColor = System.Drawing.Color.Black;
                grdAbstractDtrDetails.Columns[0].FooterStyle.HorizontalAlign = HorizontalAlign.Left;
                grdAbstractDtrDetails.Columns[0].FooterStyle.Font.Bold = true;
                grdAbstractDtrDetails.Columns[5].FooterText = totalSalary.ToString();
                grdAbstractDtrDetails.DataSource = dtAbstractDTrDetails;
                grdAbstractDtrDetails.DataBind();
                MergeRows(grdAbstractDtrDetails);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// to merge rows in a grid
        /// </summary>
        /// <param name="GridView1"></param>
        public static void MergeRows(GridView GridView1)
        {
            for (int i = GridView1.Rows.Count - 1; i > 0; i--)
            {
                GridViewRow row = GridView1.Rows[i];
                GridViewRow previousRow = GridView1.Rows[i - 1];
                int j = row.Cells.Count - 1;
                if (i != 0)
                {
                    if (((Label)row.FindControl("lblSubdivCount")).Text == ((Label)previousRow.FindControl("lblSubdivCount")).Text)
                    {
                        if (previousRow.Cells[j].RowSpan == 0)
                        {
                            if (row.Cells[j].RowSpan == 0)
                            {
                                previousRow.Cells[j].RowSpan += 2;
                            }
                            else
                            {
                                previousRow.Cells[j].RowSpan = row.Cells[j].RowSpan + 1;
                            }
                            row.Cells[j].Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// to reset a fields in abstract report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdAbstractReset_Click(object sender, EventArgs e)
        {
            cmbAbstractZone.SelectedIndex = 0;
            cmbAbstractCircle.Items.Clear();
            cmbAbstractDivision.Items.Clear();
            cmbAbstractSubDiv.Items.Clear();
            cmbAbstractSection.Items.Clear();
            emptyGridView();
        }
        /// <summary>
        /// on change function for zone dropdown to load circle details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbAbstractZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAbstractZone.SelectedIndex > 0)
            {
                Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbAbstractZone.SelectedValue + "'",
                    "--Select--", cmbAbstractCircle);
                cmbAbstractDivision.Items.Clear();
                cmbAbstractSubDiv.Items.Clear();
                cmbAbstractSection.Items.Clear();
            }
            else
            {
                cmbAbstractCircle.Items.Clear();
                cmbAbstractDivision.Items.Clear();
                cmbAbstractSubDiv.Items.Clear();
                cmbAbstractSection.Items.Clear();
            }
        }
        /// <summary>
        /// on change function for circle dropdown to load division details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbAbstractCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAbstractCircle.SelectedIndex > 0)
            {
                Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbAbstractCircle.SelectedValue + "'", "--Select--", cmbAbstractDivision);
                cmbAbstractSubDiv.Items.Clear();
                cmbAbstractSection.Items.Clear();
            }
            else
            {
                cmbAbstractDivision.Items.Clear();
                cmbAbstractSubDiv.Items.Clear();
                cmbAbstractSection.Items.Clear();
            }
        }
        /// <summary>
        /// on change function for division dropdown to load subdivision details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbAbstractDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAbstractDivision.SelectedIndex > 0)
            {
                Genaral.Load_Combo("SELECT \"SD_SUBDIV_CODE\",\"SD_SUBDIV_NAME\" FROM \"TBLSUBDIVMAST\" WHERE CAST(\"SD_DIV_CODE\" AS TEXT)='" + cmbAbstractDivision.SelectedValue + "'","--Select--", cmbAbstractSubDiv);
                cmbAbstractSection.Items.Clear();
            }
            else
            {
                cmbAbstractSubDiv.Items.Clear();
                cmbAbstractSection.Items.Clear();
            }
        }
        /// <summary>
        /// on change function for subdivision dropdown to load section details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbAbstractSubDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAbstractSubDiv.SelectedIndex > 0)
            {
                Genaral.Load_Combo("SELECT \"OM_CODE\",\"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE  CAST(\"OM_SUBDIV_CODE\" AS TEXT)='" + cmbAbstractSubDiv.SelectedValue + "'","--Select--", cmbAbstractSection);
            }
            else
            {
                cmbAbstractSection.Items.Clear();
            }
        }
        /// <summary>
        /// to show empty grid for dtr abstract details
        /// </summary>
        public void emptyGridView()
        {
            grdAbstractDtrDetails.DataSource = null;
            grdAbstractDtrDetails.DataBind();
        }
    }
}