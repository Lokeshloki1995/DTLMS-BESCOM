using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;

namespace IIITS.DTLMS.Reports
{
    public partial class EnumReport : System.Web.UI.Page
    {
        string strFormCode = "EnumerationReport";
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
                txtFromDate.Attributes.Add("readonly", "readonly");
                txtToDate.Attributes.Add("readonly", "readonly");
                CalendarExtender2.EndDate = System.DateTime.Now;
                CalendarExtender1.EndDate = System.DateTime.Now;
                if (!IsPostBack)
                {

                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_CODE\" || '-' || \"DIV_NAME\" FROM \"TBLDIVISION\" ORDER BY \"DIV_CODE\"", "--All--", cmbDiv);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


      

        protected void cmbDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDiv.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"SD_SUBDIV_CODE\", \"SD_SUBDIV_CODE\" || '-' || \"SD_SUBDIV_NAME\" from \"TBLSUBDIVMAST\" WHERE \"SD_DIV_CODE\"='" + cmbDiv.SelectedValue + "' ORDER BY \"SD_SUBDIV_CODE\"", "--All--", cmbSubDiv);
                    string strQry = "SELECT \"FD_FEEDER_CODE\",\"FD_FEEDER_CODE\" || '-' || \"FD_FEEDER_NAME\" FROM \"TBLFEEDERMAST\", \"TBLFEEDEROFFCODE\" WHERE  CAST(\"FD_FEEDER_ID\" AS TEXT) = CAST(\"FDO_FEEDER_ID\" AS TEXT) AND";
                    strQry += " CAST(\"FDO_OFFICE_CODE\" AS TEXT) LIKE '" + cmbDiv.SelectedValue + "%' ORDER BY \"FD_FEEDER_CODE\"";
                    Genaral.Load_Combo(strQry, "--All--", cmbFeeder);
                }
                else
                {

                    cmbSubDiv.Items.Clear();
                    cmbSection.Items.Clear();
                    cmbFeeder.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbType.SelectedIndex == 2)
                {
                    divmand.Visible = true;
                }
                else
                {
                    divmand.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbSubDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSubDiv.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"OM_CODE\", \"OM_CODE\" || '-' || \"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE \"OM_SUBDIV_CODE\" ='" + cmbSubDiv.SelectedValue + "' ORDER BY \"OM_CODE\"", "--All--", cmbSection);
                    string strQry = "SELECT \"FD_FEEDER_CODE\",\"FD_FEEDER_CODE\" || '-' || \"FD_FEEDER_NAME\" FROM \"TBLFEEDERMAST\", \"TBLFEEDEROFFCODE\" WHERE  CAST(\"FD_FEEDER_ID\" AS TEXT) = CAST(\"FDO_FEEDER_ID\" AS TEXT) AND";
                    strQry += " CAST(\"FDO_OFFICE_CODE\" AS TEXT) LIKE '" + cmbDiv.SelectedValue + "%' ORDER BY \"FD_FEEDER_CODE\"";
                    Genaral.Load_Combo(strQry, "--All--", cmbFeeder);

                }
                else
                {

                    cmbSection.Items.Clear();
                    cmbFeeder.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        protected void cmpReport_Click(object sender, EventArgs e)
        {
            try
            {
                string sResult = string.Empty;
                if (txtFromDate.Text != "")
                {
                    sResult = Genaral.DateValidation(txtFromDate.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtFromDate.Focus();
                        return;
                    }
                }

                if (txtToDate.Text != "")
                {
                    sResult = Genaral.DateValidation(txtToDate.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtToDate.Focus();
                        return;
                    }
                }

                if (txtFromDate.Text != "" && txtToDate.Text != "")
                {
                    sResult = Genaral.DateComparision(txtToDate.Text, txtFromDate.Text, false, false);
                    if (sResult == "2")
                    {
                        ShowMsgBox("To Date should be Greater than From Date");
                        txtToDate.Focus();
                        return;

                    }
                }
                string strOfficeCode = string.Empty;

                if (cmbType.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Location Type");
                    return;
                }

                if (cmbType.SelectedValue == "1")
                {
                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue;
                    }
                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select the Report");
                        return;
                    }

                    strOfficeCode = "id=StoreLoc&OfficeCode=" + strOfficeCode + "&FromDate=" + txtFromDate.Text.Trim() + "&ToDate=" + txtToDate.Text + "&Datewise=" + cmbdatewise.SelectedValue;
                    RegisterStartupScript("Print", "<script>window.open('ReportView.aspx?" + strOfficeCode + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");

                }

                else if (cmbType.SelectedValue == "2")
                {

                    if (cmbDiv.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select Division Name");
                        return;
                    }

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select the Report");
                        return;
                    }

                    //if (objSession.UserType != "2")
                    //{
                    //    if (cmbSubDiv.SelectedIndex == 0)
                    //    {
                    //        ShowMsgBox("Select Sub division Name");
                    //        return;
                    //    }

                    //    if (cmbSection.SelectedIndex == 0)
                    //    {
                    //        ShowMsgBox("Select Section Name");
                    //        return;
                    //    }

                    //    clsInterDashboard obj = new clsInterDashboard();
                    //    bool status = obj.CheckFeederCompletion(cmbSection.SelectedValue);
                    //    if (status == true)
                    //    {
                    //        ShowMsgBox("Unable to Generate, DTC Pending for Approval");
                    //        return;
                    //    }

                    //    //if (cmbFeeder.SelectedIndex == 0)
                    //    //{
                    //    //    ShowMsgBox("Select Feeder Name");
                    //    //    return;
                    //    //}
                    //}


                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue;
                    }
                    if (cmbSubDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbSubDiv.SelectedValue;
                    }

                    if (cmbSection.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbSection.SelectedValue;
                    }

                    string sFeederCode = string.Empty;

                    if (cmbFeeder.SelectedIndex > 0)
                    {
                        sFeederCode = cmbFeeder.SelectedValue;
                    }

                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }

                    strOfficeCode = "id=FieldLoc&OfficeCode=" + strOfficeCode + "&sFeeder=" + sFeederCode + "&FromDate=" + txtFromDate.Text.Trim() + "&ToDate=" + txtToDate.Text + "&subdiv=" + cmbSubDiv.SelectedValue + "&Datewise=" + cmbdatewise.SelectedValue;
                    RegisterStartupScript("Print", "<script>window.open('ReportView.aspx?" + strOfficeCode + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");

                }

                else if(cmbType.SelectedValue == "2")
                {

                    if (cmbDiv.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select Division Name");
                        return;
                    }


                    if (cmbSubDiv.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select Sub division Name");
                        return;

                    }
                    
                    if (cmbSection.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select Section Name");
                        return;
                    }

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select the Report");
                        return;
                    }

                    clsInterDashboard obj = new clsInterDashboard();
                    bool status = obj.CheckFeederCompletion(cmbSection.SelectedValue);
                    if(status == true)
                    {
                        ShowMsgBox("Unable to Generate, DTC Pending for Approval");
                        return;
                    }

                    //if (cmbFeeder.SelectedIndex == 0)
                    //{

                    //    ShowMsgBox("Select Feeder Name");
                    //    return;
                    //}


                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue; 
                    }
                    if (cmbSubDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbSubDiv.SelectedValue;
                    }

                    if (cmbSection.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbSection.SelectedValue;
                    }

                    string sFeederCode = string.Empty;

                    if (cmbFeeder.SelectedIndex > 0)
                    {
                        sFeederCode = cmbFeeder.SelectedValue;
                    }

                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }

                    strOfficeCode = "id=FieldLoc&OfficeCode=" + strOfficeCode + "&sFeeder=" + sFeederCode + "&FromDate=" + txtFromDate.Text.Trim() + "&ToDate=" + txtToDate.Text + "&subdiv=" + cmbSubDiv.SelectedValue + "&Datewise=" + cmbdatewise.SelectedValue;
                    RegisterStartupScript("Print", "<script>window.open('ReportView.aspx?" + strOfficeCode + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");


                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
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
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        protected void cmdReset_Click(object sender, EventArgs e)
        {
            try
            {

                cmbdatewise.SelectedIndex = 0;
                cmbType.SelectedIndex = 0;
                txtFromDate.Text = string.Empty;
                txtToDate.Text = string.Empty;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        protected void cmdDtrAbstract_Click(object sender, EventArgs e)
        {
            clsReports objReport = new clsReports();
            DataTable dtAbstractDTrDetails = new DataTable();
            try
            {
                string sResult = string.Empty;
                if (txtFromDate.Text != "")
                {
                    sResult = Genaral.DateValidation(txtFromDate.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtFromDate.Focus();
                        return;
                    }
                }

                if (txtToDate.Text != "")
                {
                    sResult = Genaral.DateValidation(txtToDate.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtToDate.Focus();
                        return;
                    }
                }

                if (txtFromDate.Text != "" && txtToDate.Text != "")
                {
                    sResult = Genaral.DateComparision(txtToDate.Text, txtFromDate.Text, false, false);
                    if (sResult == "2")
                    {
                        ShowMsgBox("To Date should be Greater than From Date");
                        txtToDate.Focus();
                        return;

                    }
                }
                string strOfficeCode = string.Empty;

                if (cmbType.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Report Type");
                    return;
                }

                if (cmbType.SelectedValue == "1")
                {
                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue;
                    }
                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select the Report");
                        return;
                    }
                    dtAbstractDTrDetails = objReport.PrintStoreDetailsabstract(strOfficeCode, txtFromDate.Text.Trim(), txtToDate.Text, cmbdatewise.SelectedValue);

                    //dtAbstractDTrDetails = objReport.getAbstractDtrDetails(objReport.sOfficeCode);
                    ViewState["AbstractDTrDetails"] = dtAbstractDTrDetails;
                    grdAbstractDtrDetails.DataSource = dtAbstractDTrDetails;

                    double totalSalary = 0;
                    foreach (DataRow dr in dtAbstractDTrDetails.Rows)
                    {
                        totalSalary += Convert.ToInt32(dr["DTE_TC_CODE"]);
                    }

                    //--- Here 3 is the number of column where you want to show the total.  
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.Columns[0].FooterText = "Total";
                    grdAbstractDtrDetails.Columns[0].FooterStyle.ForeColor = System.Drawing.Color.Black;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.HorizontalAlign = HorizontalAlign.Left;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.Font.Bold =true;
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.DataBind();


                    
                }

                else if (cmbType.SelectedValue == "2")
                {

                    //if (cmbDiv.SelectedIndex == 0)
                    //{
                    //    ShowMsgBox("Please Select Division Name");
                    //    return;
                    //}

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Please Select the Report");
                        return;
                    }

                    


                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue;
                    }
                   

                    string sFeederCode = string.Empty;

                    if (cmbFeeder.SelectedIndex > 0)
                    {
                        sFeederCode = cmbFeeder.SelectedValue;
                    }

                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }
                    dtAbstractDTrDetails = objReport.PrintFieldDetailsAbstract(sFeederCode, strOfficeCode,txtFromDate.Text.Trim(), txtToDate.Text,  cmbdatewise.SelectedValue);
                    ViewState["AbstractDTrDetails"] = dtAbstractDTrDetails;
                    grdAbstractDtrDetails.DataSource = dtAbstractDTrDetails;

                    double totalSalary = 0;
                    foreach (DataRow dr in dtAbstractDTrDetails.Rows)
                    {
                        totalSalary += Convert.ToInt32(dr["DTE_TC_CODE"]);
                    }

                    //--- Here 3 is the number of column where you want to show the total.  
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.Columns[0].FooterText = "Total";
                    grdAbstractDtrDetails.Columns[0].FooterStyle.ForeColor = System.Drawing.Color.Black;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.HorizontalAlign = HorizontalAlign.Left;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.Font.Bold = true;
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.DataBind();

                 
                }

                else if (cmbType.SelectedValue == "2")
                {

                    //if (cmbDiv.SelectedIndex == 0)
                    //{
                    //    ShowMsgBox("Please Select Division Name");
                    //    return;
                    //}


                    

                    if (cmbdatewise.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select the Report");
                        return;
                    }

                    clsInterDashboard obj = new clsInterDashboard();
                    bool status = obj.CheckFeederCompletion(cmbSection.SelectedValue);
                    if (status == true)
                    {
                        ShowMsgBox("Unable to Generate, DTC Pending for Approval");
                        return;
                    }

                   


                    if (cmbDiv.SelectedIndex > 0)
                    {
                        strOfficeCode = cmbDiv.SelectedValue;
                    }
                   

                    string sFeederCode = string.Empty;

                    if (cmbFeeder.SelectedIndex > 0)
                    {
                        sFeederCode = cmbFeeder.SelectedValue;
                    }

                    if (txtToDate.Text == "")
                    {
                        txtToDate.Text = txtFromDate.Text;
                    }
                    dtAbstractDTrDetails = objReport.PrintFieldDetailsAbstract(sFeederCode, strOfficeCode, txtFromDate.Text.Trim(), txtToDate.Text, cmbdatewise.SelectedValue);


                    // dtAbstractDTrDetails = objReport.getAbstractDtrDetails(objReport.sOfficeCode);
                    ViewState["AbstractDTrDetails"] = dtAbstractDTrDetails;
                    grdAbstractDtrDetails.DataSource = dtAbstractDTrDetails;

                    double totalSalary = 0;
                    foreach (DataRow dr in dtAbstractDTrDetails.Rows)
                    {
                        totalSalary += Convert.ToInt32(dr["DTE_TC_CODE"]);
                    }

                    //--- Here 3 is the number of column where you want to show the total.  
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.Columns[0].FooterText = "Total";
                    grdAbstractDtrDetails.Columns[0].FooterStyle.ForeColor = System.Drawing.Color.Black;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.HorizontalAlign = HorizontalAlign.Left;
                    grdAbstractDtrDetails.Columns[0].FooterStyle.Font.Bold = true;
                    grdAbstractDtrDetails.Columns[1].FooterText = totalSalary.ToString();

                    grdAbstractDtrDetails.DataBind();


                    
                }
                


               
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


    }
}