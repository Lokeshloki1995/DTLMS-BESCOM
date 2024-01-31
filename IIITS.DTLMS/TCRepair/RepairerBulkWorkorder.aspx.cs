using IIITS.DTLMS.BL;
using IIITS.DTLMS.BL.TCRepair;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IIITS.DTLMS.TCRepair
{
    public partial class RepairerBulkWorkorder : System.Web.UI.Page
    {

        string strFormCode = "RepairerBulkWorkOrder";
        clsSession objSession;
        bool RetriveFromXML = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    objSession = (clsSession)Session["clsSession"];
                    CheckAccessRights("4");
                    txtAcCode.Text = "74-116";
                    txtSSAcCode.Text = "62-340";
                    txtDeclaredBy.Text = "AET-REP";
                    Form.DefaultButton = cmdSave.UniqueID;
                    lblMessage.Text = string.Empty;
                    string sWo_id = string.Empty;
                    bool sViewRecord = false;
                    string sFailId = string.Empty;
                    txtRepdate.Attributes.Add("readonly", "readonly");
                    CalendarExtender_txtRepdate.StartDate = System.DateTime.Now;
                    CalendarExtender_txtRepdate.EndDate = System.DateTime.Now;

                    if (!IsPostBack)
                    {
                        Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" from \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='I' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbIssuedBy);
                        LoadBulkWoDetails();
                        if (objSession.sRoleType != "7")
                        {
                            Session["BOID"] = "84";
                            ViewState["BOID"] = "84";
                        }
                        else
                        {
                            ApprovalHistoryView.Visible = false;
                            ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        }
                        if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                        {
                            txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                            hdfWFDataId.Value = Genaral.UrlDecrypt(Convert.ToString(Request.QueryString["WFDataId"]));
                            hdfWFOId.Value = Genaral.UrlDecrypt(Convert.ToString(Request.QueryString["WFOId"]));
                            hdfWFOId.Value = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["WFOId"]));
                            // hdfWFOId.Value = Convert.ToString(Session["WFOId"]);

                        }
                        WorkFlowConfig();

                        if (Request.QueryString["ReferID"] != null && Convert.ToString(Request.QueryString["ReferID"]) != "")
                        {
                            if (txtType.Text != "3")
                            {
                                if (sViewRecord == true)
                                {
                                    sWo_id = sWo_id.Split('~').GetValue(1).ToString();
                                }
                                else
                                {
                                    sWo_id = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                }
                            }
                        }
                    }


                    ApprovalHistoryView.BOID = Convert.ToString(ViewState["BOID"]);
                    if (Convert.ToString(Request.QueryString["sWoRecordID"]) != null && Convert.ToString(Request.QueryString["sWoRecordID"]) != "")
                    {
                        if (Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"])).Contains('-'))
                        {
                            ApprovalHistoryView.sRecordId = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"]));
                        }
                        else
                        {
                            ApprovalHistoryView.sRecordId = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"]));
                        }
                    }
                    else
                    {

                        // ApprovalHistoryView.sRecordId = sWo_id;
                      //  sWo_id= hdfWFOId.Value;
                       ApprovalHistoryView.sRecordId = sWo_id;
                    }
                    cmbIssuedBy.SelectedValue = "2";
                }
                if (txtActiontype.Text == "M")
                {
                    clsApproval objLevel = new clsApproval();
                    string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                    if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = " Modify and Submit";
                    }
                    else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = " Modify and Approve";
                    }
                }
                else if (txtActiontype.Text == "A")
                {
                    clsApproval objLevel = new clsApproval();
                    string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                    if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = "Submit";
                    }
                    else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = "Approve";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void WorkFlowConfig()
        {
            string sApproveStatus = string.Empty;
            try
            {
                //WorkFlow / Approval
                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                {

                    if (Session["WFOId"] != null && Convert.ToString(Session["WFOId"]) != "")
                    {
                        hdfWFDataId.Value = Convert.ToString(Session["WFDataId"]);
                        hdfWFOId.Value = Convert.ToString(Session["WFOId"]);
                        hdfWFOAutoId.Value = Convert.ToString(Session["WFOAutoId"]);
                        hdfApproveStatus.Value = Convert.ToString(Session["ApproveStatus"]);

                        hdfWFOId.Value = Genaral.UrlDecrypt(Convert.ToString(Request.QueryString["WFOId"]));
                        hdfWFDataId.Value = Genaral.UrlDecrypt(Convert.ToString(Request.QueryString["WFDataId"]));

                        Session["WFDataId"] = null;
                        Session["WFOId"] = null;
                        Session["WFOAutoId"] = null;
                        Session["ApproveStatus"] = null;
                    }

                    if (hdfWFDataId.Value != "0")
                    {
                        if (txtActiontype.Text != "V")
                        {
                            sApproveStatus = GetPreviousApproveStatus(hdfWFOId.Value);

                            if (sApproveStatus == "3")
                            {
                                txtActiontype.Text = "M";
                            }
                        }

                        if (txtActiontype.Text == "V")
                        {
                            if (sApproveStatus == "3")
                            {
                                sApproveStatus = "2";
                            }
                        }


                        if (hdfWFDataId.Value != "")
                        {
                            GetBulkWODetailsFromXML(hdfWFDataId.Value);
                        }
                    }
                    SetControlText();

                    if (txtActiontype.Text == "V")
                    {
                        cmdSave.Text = "View";
                        cmdSave.Enabled = true;
                        cmdReset.Enabled = false;
                        dvComments.Style.Add("display", "none");

                    }

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetBulkWODetailsFromXML(string sWFDataId)
        {
            try
            {
                ClsRepairerWorkorder objWorkOrder = new ClsRepairerWorkorder();
                objWorkOrder.sWFDataId = sWFDataId;

                objWorkOrder.GetBulkWODetailsFromXML(objWorkOrder);
                RetriveFromXML = true;

                if (objWorkOrder.dtBulkWOList.Rows.Count > 0)
                {
                    ViewState["BulkWO"] = objWorkOrder.dtBulkWOList;
                    grdBulkWo.DataSource = objWorkOrder.dtBulkWOList;
                    grdBulkWo.DataBind();
                }
                double EstTotal = 0, WOTotal = 0, WOIncTotal = 0, SSWOTotal = 0;

                if (grdBulkWo.Rows.Count > 0)
                {
                    foreach (GridViewRow row in grdBulkWo.Rows)
                    {
                        String sEstTotal = "", sWOTotal = "", sIncTotal = "", sSSWoTotal = "";
                        if (((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                        {
                            if (txtActiontype.Text == "M")
                            {
                                string WoNO = ((Label)row.FindControl("lblWoNumber")).Text;
                                string SSWoNO = ((Label)row.FindControl("lblSSWoNo")).Text;

                                ((TextBox)row.FindControl("txtWoNo1")).Text = WoNO.Split('/').GetValue(2).ToString();
                                ((TextBox)row.FindControl("txtWoNo2")).Text = WoNO.Split('/').GetValue(3).ToString();
                                ((TextBox)row.FindControl("txtWoNo3")).Text = WoNO.Split('/').GetValue(4).ToString();

                                ((TextBox)row.FindControl("txtWoAmount")).Text = ((Label)row.FindControl("lblWoAmount")).Text;
                                ((TextBox)row.FindControl("txtIncAmt")).Text = ((Label)row.FindControl("lblIncCost")).Text;

                                if (SSWoNO.Length > 0)
                                {
                                    ((TextBox)row.FindControl("txtSSWoNo1")).Text = SSWoNO.Split('/').GetValue(2).ToString();
                                    ((TextBox)row.FindControl("txtSSWoNo2")).Text = SSWoNO.Split('/').GetValue(3).ToString();
                                    ((TextBox)row.FindControl("txtSSWoNo3")).Text = SSWoNO.Split('/').GetValue(4).ToString();
                                }
                                ((TextBox)row.FindControl("txtSSAmt")).Text = ((Label)row.FindControl("lblSSAmount")).Text;
                            }
                            sEstTotal = ((Label)row.FindControl("lblEstAmt")).Text;
                            sWOTotal = ((Label)row.FindControl("lblWoAmount")).Text;
                            sIncTotal = ((Label)row.FindControl("lblIncCost")).Text;
                            sSSWoTotal = ((Label)row.FindControl("lblSSAmount")).Text;
                        }
                        EstTotal = EstTotal + Convert.ToDouble(sEstTotal);
                        WOTotal = WOTotal + Convert.ToDouble(sWOTotal);
                        WOIncTotal = WOIncTotal + Convert.ToDouble(sIncTotal == "" ? "0" : sIncTotal);
                        SSWOTotal = SSWOTotal + Convert.ToDouble(sSSWoTotal == "" ? "0" : sSSWoTotal);
                    }

                    if (txtActiontype.Text == "M")
                    {
                        grdBulkWo.FooterRow.Cells[0].Text = "Total";
                        grdBulkWo.FooterRow.Cells[5].Text = Convert.ToString(EstTotal);
                        grdBulkWo.FooterRow.Cells[8].Text = Convert.ToString(WOTotal);
                        grdBulkWo.FooterRow.Cells[10].Text = Convert.ToString(WOIncTotal);
                        grdBulkWo.FooterRow.Cells[14].Text = Convert.ToString(SSWOTotal);
                    }
                    else
                    {
                        grdBulkWo.FooterRow.Cells[0].Text = "Total";
                        grdBulkWo.FooterRow.Cells[5].Text = Convert.ToString(EstTotal);
                        grdBulkWo.FooterRow.Cells[9].Text = Convert.ToString(WOTotal);
                        grdBulkWo.FooterRow.Cells[11].Text = Convert.ToString(WOIncTotal);
                        grdBulkWo.FooterRow.Cells[15].Text = Convert.ToString(SSWOTotal);
                    }
                }

                if (objWorkOrder.sCommWoNo != null)
                {
                    txtBulkWoNo1.Text = objWorkOrder.sBulkWoNo.Split('/').GetValue(2).ToString();
                    txtBulkWoNo2.Text = objWorkOrder.sBulkWoNo.Split('/').GetValue(3).ToString();
                    txtBulkWoNo3.Text = objWorkOrder.sBulkWoNo.Split('/').GetValue(4).ToString();
                    txtRepdate.Text = objWorkOrder.sCommDate;
                    if (txtActiontype.Text != "M")
                    {
                        txtBulkWoNo1.Enabled = false;
                        txtBulkWoNo2.Enabled = false;
                        txtBulkWoNo3.Enabled = false;
                        txtRepdate.Enabled = false;
                    }
                    hdfCrBy.Value = objWorkOrder.sCrBy;
                }
                cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;
                txtAcCode.Text = objWorkOrder.sAccCode;
                txtRepdate.Text = objWorkOrder.sCommDate;
                hdfCrBy.Value = objWorkOrder.sCrBy;


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        public string GetPreviousApproveStatus(string sWo_id)
        {
            string sApprove_id = string.Empty;
            try
            {
                clsFailureEntry objFailure = new clsFailureEntry();
                sApprove_id = objFailure.GetPreviousApproveStatus(sWo_id);
                return sApprove_id;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sApprove_id;
            }
        }
        public void SetControlText()
        {
            try
            {
                if (txtActiontype.Text == "")
                {
                    txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                }
                else
                {
                }
                if (txtActiontype.Text == "A")
                {
                    cmdSave.Text = "Approve";
                    cmdReset.Enabled = true;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdSave.Text = "Reject";
                    cmdReset.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    cmdSave.Text = "Modify and Approve";
                    cmdReset.Enabled = true;
                }
                if (hdfWFOAutoId.Value == "0")
                {
                    dvComments.Style.Add("display", "block");
                    cmdReset.Enabled = true;
                }

                if (hdfWFOAutoId.Value != "0")
                {
                    dvComments.Style.Add("display", "none");
                    cmdReset.Enabled = true;
                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    if (txtActiontype.Text == "A" && hdfWFOAutoId.Value == "0")
                    {
                        txtActiontype.Text = "M";
                        hdfRejectApproveRef.Value = "RA";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                {
                    Response.Redirect("/Approval/ApprovalInbox.aspx", false);
                }
                else
                {
                    Response.Redirect("/Approval/ApprovalInbox.aspx", false);
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void LoadBulkWoDetails()
        {
            try
            {
                DataTable dt = new DataTable();
                ClsRepairerWorkorder objRepWO = new ClsRepairerWorkorder();

                objRepWO.sOfficeCode = objSession.OfficeCode.Substring(0, 3);
                dt = objRepWO.LoadBulkWorkOrder(objRepWO);

                ViewState["BulkWO"] = dt;
                grdBulkWo.DataSource = dt;
                grdBulkWo.DataBind();


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        protected void grdBulkWo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if ((RetriveFromXML == true && (txtActiontype.Text == "A" || txtActiontype.Text == "V" || txtActiontype.Text == "R")) || txtActiontype.Text == "VIEW")
                {
                    foreach (GridViewRow row1 in grdBulkWo.Rows)
                    {
                        CheckBox chk = ((CheckBox)row1.FindControl("chkmaterial"));
                        chk.Checked = true;
                    }
                    grdBulkWo.Columns[6].Visible = false;
                    grdBulkWo.Columns[7].Visible = true;
                    grdBulkWo.Columns[8].Visible = false;
                    grdBulkWo.Columns[9].Visible = true;
                    grdBulkWo.Columns[10].Visible = false;
                    grdBulkWo.Columns[11].Visible = true;
                    grdBulkWo.Columns[12].Visible = false;
                    grdBulkWo.Columns[13].Visible = true;
                    grdBulkWo.Columns[14].Visible = false;
                    grdBulkWo.Columns[15].Visible = true;
                }
                if (RetriveFromXML == true && (txtActiontype.Text == "M"))
                {
                    foreach (GridViewRow row1 in grdBulkWo.Rows)
                    {
                        CheckBox chk = ((CheckBox)row1.FindControl("chkmaterial"));
                        chk.Checked = true;
                    }

                    grdBulkWo.Columns[6].Visible = true;
                    grdBulkWo.Columns[7].Visible = false;
                    grdBulkWo.Columns[8].Visible = true;
                    grdBulkWo.Columns[9].Visible = false;
                    grdBulkWo.Columns[10].Visible = true;
                    grdBulkWo.Columns[11].Visible = false;
                    grdBulkWo.Columns[12].Visible = true;
                    grdBulkWo.Columns[13].Visible = false;
                    grdBulkWo.Columns[14].Visible = true;
                    grdBulkWo.Columns[15].Visible = false;

                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                string[] Arr = new string[2];
                string[] sBulkWorkOrderlist = new string[0];
                ClsRepairerWorkorder objWorkOrder = new ClsRepairerWorkorder();
                DataTable dtOffName = new DataTable();
                //Check AccessRights
                bool bAccResult = true;
                if (cmdSave.Text == "Update")
                {
                    bAccResult = CheckAccessRights("3");
                }
                else if (cmdSave.Text == "Save" || cmdSave.Text == "Submit" || cmdSave.Text == "Approve")
                {
                    bAccResult = CheckAccessRights("2");
                }

                if (bAccResult == false)
                {
                    return;
                }


                if (cmdSave.Text == "View")
                {
                    ClsRepairerWorkorder objWrkOrder = new ClsRepairerWorkorder();
                    if (hdfApproveStatus.Value != "")
                    {

                        if (hdfApproveStatus.Value != "3")
                        {
                            objWrkOrder.sWFDataId = hdfWFDataId.Value;
                            objWrkOrder.sTaskType = txtType.Text;
                            if (txtType.Text == "3")
                            {
                            }
                            else
                            {
                                GenerateWorkOrderReport(objWrkOrder);
                            }
                        }
                    }
                    else
                    {
                        if (txtType.Text == "1" || txtType.Text == "2")
                        {
                            //  objWrkOrder.sWFDataId = objWorkOrder.getWoDataId(txtFailureId.Text);
                            objWrkOrder.sTaskType = txtType.Text;
                            GenerateWorkOrderReport(objWrkOrder);
                        }
                        else
                            if (txtType.Text == "3")
                        {
                            objWrkOrder.sWOId = txtWOId.Text;
                            objWrkOrder.sTaskType = txtType.Text;

                            if (txtType.Text == "3")
                            {
                            }
                            else
                            {
                                GenerateWorkOrderReport(objWrkOrder);
                            }
                        }
                        else
                        {
                            // objWrkOrder.sWFDataId = objWorkOrder.getWoDataId(txtFailureId.Text);
                            objWrkOrder.sWOId = txtWOId.Text;
                            objWrkOrder.sTaskType = txtType.Text;
                            GenerateWorkOrderReport(objWrkOrder);
                        }
                    }

                    return;
                }

                if (ValidateForm() == true)
                {
                    objWorkOrder.sWOId = txtWOId.Text;

                    //  objWorkOrder.sEstId = txtFailureId.Text.Trim();

                    objWorkOrder.sIssuedBy = cmbIssuedBy.SelectedValue;
                    objWorkOrder.sGuarentyType = hdfGuarenteeType.Value;
                    objWorkOrder.sFailType = txtFailType.Text.Trim();

                    int i = 0;
                    int j = 0;
                    int k = 0;
                    int totalCount = 0;
                    sBulkWorkOrderlist = new string[grdBulkWo.Rows.Count];
                    bool bChecked = false;

                    foreach (GridViewRow row in grdBulkWo.Rows)
                    {
                        // if (((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                        if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                        {

                            String EstID = ((Label)row.FindControl("lblEstId")).Text.Trim();

                            if (EstID != "")
                            {
                                dtOffName = objWorkOrder.GetofficeName(EstID);
                            }
                            else
                            {
                                dtOffName = objWorkOrder.GetofficeNameBySectionCode(objSession.OfficeCode);
                            }

                            string WoComm = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + ((TextBox)row.FindControl("txtWoNo1")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtWoNo3")).Text.Trim();
                            string SSWo = string.Empty;
                            string SSAMT = string.Empty;
                            if (((TextBox)row.FindControl("txtSSWoNo1")).Text.Trim() + ((TextBox)row.FindControl("txtSSWoNo2")).Text.Trim() + ((TextBox)row.FindControl("txtSSWoNo3")).Text.Trim() != "")
                            {
                                SSWo = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + ((TextBox)row.FindControl("txtSSWoNo1")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtSSWoNo2")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtSSWoNo3")).Text.Trim();
                            }
                            if (((TextBox)row.FindControl("txtSSAmt")).Text.Trim() != "")
                            {
                                SSAMT = ((TextBox)row.FindControl("txtSSAmt")).Text.Trim();
                            }

                            sBulkWorkOrderlist[i] = ((Label)row.FindControl("lblEstId")).Text.Trim() + "~" + ((Label)row.FindControl("lbltccode")).Text.Trim() + "~" + ((Label)row.FindControl("lbltccap")).Text.Trim() + "~" +
                                    ((Label)row.FindControl("lblEstNo")).Text.Trim() + "~" + ((Label)row.FindControl("lblEstAmt")).Text.Trim() + "~" + WoComm.ToUpper() + "~" +
                                    ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() + "~" + ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() + "~" + SSWo.ToUpper() + "~" + SSAMT;

                            // if (txtWoNo1.Text.Length.ToString() == 0 || txtWoNo2.Text.Trim().Length == 0 || txtWoNo3.Text.Trim().Length == 0) 
                            if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "")
                                
                            {
                                bChecked = true;
                            }
                           // bChecked = true;
                            i++;
                        }
                       // i++;
                    }
                    if (sBulkWorkOrderlist[0] == null)
                    {
                        ShowMsgBox("Please Select Atleast One check box");
                        return;
                    }

                    //
                    foreach (GridViewRow row in grdBulkWo.Rows)
                    {

                        // CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                        CheckBox chk = (CheckBox)row.FindControl("chkmaterial");
                        CheckBox chkRow = (row.Cells[0].FindControl("chkmaterial") as CheckBox);
                        if (chkRow.Checked == true && chk != null)
                        {                           
                            bChecked = true;
                            j++;
                        }
                        // bChecked = false;
                        // totalCount = grdBulkWo.Rows.Cast<GridViewRow>().Count(r => ((CheckBox)r.FindControl("chkSelect")).Checked);
                        k++;
                    }
                    if(i!= j)
                    {
                        ShowMsgBox("Please Select check box and Enter the WO Number");
                        return;
                    }
                    //


                    string sBulkWONO = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtBulkWoNo1.Text.Trim().Replace("'", "") + "/" + txtBulkWoNo2.Text.Trim().Replace("'", "") + "/" + txtBulkWoNo3.Text.Trim().Replace("'", "");
                    objWorkOrder.sBulkWoNo = sBulkWONO.Trim().ToUpper();


                    objWorkOrder.sCommDate = txtRepdate.Text.Trim().Replace("'", "");
                    objWorkOrder.sAccCode = txtAcCode.Text.Trim().Replace("'", "");
                    objWorkOrder.sScrapAccCode = txtSSAcCode.Text.Trim().Replace("'", "");

                    objWorkOrder.sCrBy = objSession.UserId;
                    objWorkOrder.sLocationCode = objSession.OfficeCode;
                    objWorkOrder.sTaskType = txtType.Text;


                    if (txtType.Text == "3")
                    {
                        // objWorkOrder.sRequestLoc = cmbSection.SelectedValue.Trim();
                    }



                    #region Approve And Reject

                    if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                    {
                        if (hdfWFDataId.Value != "0")
                        {
                            ApproveRejectAction();

                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RepairerWorkorder)  ");
                            }

                            return;
                        }
                    }

                    #endregion

                    //Workflow
                    WorkFlowObjects(objWorkOrder);

                    #region Modify and Approve

                    // For Modify and Approve
                    if (txtActiontype.Text == "M")
                    {
                        if (txtComment.Text.Trim() == "")
                        {
                            ShowMsgBox("Enter Comments/Remarks");
                            txtComment.Focus();
                            return;

                        }
                        objWorkOrder.sWOId = "";
                        objWorkOrder.sActionType = txtActiontype.Text;
                        objWorkOrder.sCrBy = hdfCrBy.Value;
                        objWorkOrder.sboid = Convert.ToString(Session["BOID"]);
                        // objWorkOrder.sTCCode = txtTCCode.Text;

                        Arr = objWorkOrder.SaveUpdateBULKWorkOrder(objWorkOrder, sBulkWorkOrderlist);
                        if (Arr[1].ToString() == "0")
                        {
                            hdfWFDataId.Value = objWorkOrder.sWFDataId;
                            hdfAppDesc.Value = objWorkOrder.sApprovalDesc;
                            hdfboid.Value = Arr[2];
                            ApproveRejectAction();
                            return;
                        }
                        if (Arr[1].ToString() == "2")
                        {
                            ShowMsgBox(Arr[0]);
                            return;
                        }
                    }

                    #endregion

                    objWorkOrder.sboid = Convert.ToString(Session["BOID"]);
                    //   objWorkOrder.sTCCode = txtTCCode.Text;
                    Arr = objWorkOrder.SaveUpdateBULKWorkOrder(objWorkOrder, sBulkWorkOrderlist);

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RepairerWorkorder)");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        ShowMsgBox(Arr[0]);
                        txtWOId.Text = objWorkOrder.sWOId;
                        //ApproveRejectAction();
                        cmdSave.Text = "Update";
                        cmdSave.Enabled = false;
                        if (txtType.Text == "3")
                        {
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWorkOrder);
                        }
                        return;
                    }
                    if (Arr[1].ToString() == "1")
                    {

                        if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                        {
                            ApproveRejectAction();
                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RepairerWorkorder) ");
                            }
                            return;
                        }

                        if (txtActiontype.Text == "M")
                        {
                            ShowMsgBox("Modified and Approved Successfully");
                        }
                        else
                        {
                            ShowMsgBox(Arr[0]);
                        }
                        return;
                    }
                    if (Arr[1].ToString() == "2")
                    {
                        ShowMsgBox(Arr[0]);
                        return;
                    }
                }
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
                Reset();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public bool ValidateForm()
        {
            bool bValidate = false;
            string estimatedate = string.Empty;
            string[] sBulkWorkOrderlist1 = new string[0];
            try
            {
                

                    if (cmbIssuedBy.SelectedIndex == 0)
                    {
                        cmbIssuedBy.Focus();
                        ShowMsgBox("Select Issued By");
                        return bValidate;
                    }
                    if (txtBulkWoNo1.Text.Trim().Length == 0 || txtBulkWoNo2.Text.Trim().Length == 0 || txtBulkWoNo3.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter the Bulk WO Number");

                        if (txtBulkWoNo1.Text.Trim().Length == 0)
                        {
                            txtBulkWoNo1.Focus();
                        }
                        else if (txtBulkWoNo2.Text.Trim().Length == 0)
                        {
                            txtBulkWoNo2.Focus();
                        }
                        else
                        {
                            txtBulkWoNo3.Focus();
                        }
                        return bValidate;
                    }

                    int i = 0;
                    bool bChecked = false;
                    string[] strQryVallist = new string[grdBulkWo.Rows.Count];
                    string sInsRes = string.Empty;

                    foreach (GridViewRow row in grdBulkWo.Rows)
                    {

                        if (((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                        {
                            //  sBulkWorkOrderlist1[i] = ((Label)row.FindControl("lblEstId")).Text.Trim() + "~" + ((Label)row.FindControl("lbltccode")).Text.Trim() + "~" + ((Label)row.FindControl("lbltccap")).Text.Trim() + "~" +
                            //            ((Label)row.FindControl("lblEstNo")).Text.Trim() + "~" + ((Label)row.FindControl("lblEstAmt")).Text.Trim() +  "~" +
                            //            ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() + "~" + ((TextBox)row.FindControl("txtIncAmt")).Text.Trim();

                            if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                            {
                                bChecked = true;
                            }
                        }
                        i++;

                    }

                    if (bChecked == false)
                    {
                        ShowMsgBox("Please Select DTR");
                        return bValidate;
                    }

                    foreach (GridViewRow row in grdBulkWo.Rows)
                    {
                        if (((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                        {
                            if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() == "" || ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() == "" || ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() == "" || ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() == "" || ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() == "" || ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() == "" || ((CheckBox)row.FindControl("chkmaterial")).Checked == false)
                            {
                                ShowMsgBox("Please Select check box and Enter the WO Number");
                                return bValidate;
                            }
                        }
                    }

                    //
                    //      foreach (GridViewRow row in grdBulkWo.Rows)
                    //       {

                    //           if (((CheckBox)row.FindControl("chkmaterial")).Checked == true)
                    //          {
                    //              if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "" && ((TextBox)row.FindControl("txtIncAmt")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "")
                    //
                    //                  bChecked = true;
                    //           }
                    //           i++;

                    //      }

                    //
                    if (txtRepdate.Text.Trim() == "")
                    {
                        txtRepdate.Focus();
                        ShowMsgBox("Enter Repairer Workorder Date");
                        return bValidate;
                    }

                    bValidate = ValidateGrid();

                    //   bValidate = true;
                    return bValidate;
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return bValidate;
            }
        }
        public bool ValidateGrid()
        {
            try
            {
                ViewState["GRID"] = null;
                int i = 0;
                int gridcount = 0;
                DataTable dt = new DataTable("NEWTABLE");
                foreach (GridViewRow row in grdBulkWo.Rows)
                {
                    CheckBox chkMaterial = (CheckBox)row.FindControl("chkmaterial");

                    if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                    {
                        if (((Label)row.FindControl("lblWoNumber")).Text.Trim() != "" && ((Label)row.FindControl("lblWoAmount")).Text.Trim() != "")
                        {

                            string SSWo = string.Empty;
                            string WoComm = ((Label)row.FindControl("lblWoNumber")).Text.Trim();
                            if (((Label)row.FindControl("lblSSWoNo")).Text.Trim() != "")
                            {
                                SSWo = ((Label)row.FindControl("lblSSWoNo")).Text.Trim();
                            }
                            chkMaterial.Checked = true;
                            //  sBulkWorkOrderlist[i] = WoComm.ToUpper() + "~" + SSWo.ToUpper();

                            if (ViewState["GRID"] == null)
                            {
                                dt.Columns.Add("WONO");
                                dt.Columns.Add("SSWONO");
                            }
                            else
                            {
                                dt = (DataTable)ViewState["GRID"];
                            }
                            DataRow Row = dt.NewRow();
                            Row["WONO"] = WoComm.ToUpper();
                            Row["SSWONO"] = SSWo.ToUpper();

                            //dt.Rows.Add(Row);
                            bool exists = dt.Select().ToList().Exists(a => a["WONO"].ToString() == WoComm.ToUpper());
                            if (exists)
                            {
                                ShowMsgBox("Work Order Number " + WoComm + "Already Entered");
                                return false;
                            }
                            //else if (exists = dt.Select().ToList().Exists(a => a["SSWONO"].ToString() == SSWo.ToUpper() && SSWo.ToUpper() != ""))
                            //{
                            //    ShowMsgBox("SS Work Order Number " + SSWo + "Already Entered");
                            //    return false;
                            //}
                            else
                            {
                                dt.Rows.Add(Row);
                            }
                            ViewState["GRID"] = dt;
                            gridcount++;
                        }
                        i++;
                    }
                    else
                    {
                        if (((TextBox)row.FindControl("txtWoNo1")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoNo3")).Text.Trim() != "" && ((TextBox)row.FindControl("txtWoAmount")).Text.Trim() != "")
                        {
                            string SSWo = string.Empty;
                            string WoComm = ((TextBox)row.FindControl("txtWoNo1")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtWoNo2")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtWoNo3")).Text.Trim();

                            if (((TextBox)row.FindControl("txtSSWoNo1")).Text.Trim() + ((TextBox)row.FindControl("txtSSWoNo2")).Text.Trim() + ((TextBox)row.FindControl("txtSSWoNo3")).Text.Trim() != "")
                            {
                                SSWo = ((TextBox)row.FindControl("txtSSWoNo1")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtSSWoNo2")).Text.Trim() + "/" + ((TextBox)row.FindControl("txtSSWoNo3")).Text.Trim();
                            }
                            chkMaterial.Checked = true;
                            // sBulkWorkOrderlist[i] = WoComm.ToUpper() + "~" + SSWo.ToUpper();


                            if (ViewState["GRID"] == null)
                            {
                                dt.Columns.Add("WONO");
                                dt.Columns.Add("SSWONO");
                            }
                            else
                            {
                                dt = (DataTable)ViewState["GRID"];
                            }
                            DataRow Row = dt.NewRow();
                            Row["WONO"] = WoComm.ToUpper();
                            Row["SSWONO"] = SSWo.ToUpper();

                            //dt.Rows.Add(Row);
                            bool exists = dt.Select().ToList().Exists(a => a["WONO"].ToString() == WoComm.ToUpper());
                            if (exists)
                            {
                                ShowMsgBox("Work Order Number " + WoComm + "Already Entered");
                                return false;
                            }
                            //else if (SSWo.Trim() != "")
                            //{
                            //    if (exists = dt.Select().ToList().Exists(a => a["SSWONO"].ToString() == SSWo.ToUpper()))
                            //    {
                            //        ShowMsgBox("SS Work Order Number " + SSWo + "Already Entered");
                            //        return false;
                            //    }
                            //    else
                            //    {
                            //        dt.Rows.Add(Row);
                            //    }
                            //}
                            else
                            {
                                dt.Rows.Add(Row);
                            }
                            ViewState["GRID"] = dt;
                            gridcount++;
                        }
                        i++;

                    }
                }
                ViewState["GRID"] = dt;
                if (gridcount == 0)
                {
                    ShowMsgBox("Please Fill Atleast one WorkOrder Number And Amount");
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }


        public void WorkFlowObjects(ClsRepairerWorkorder objWorkOrder)
        {
            try
            {
                string sClientIP = string.Empty;

                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    int le = ipRange.Length - 1;
                    sClientIP = ipRange[0];
                }
                else
                {
                    sClientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                objWorkOrder.sFormName = strFormCode;
                objWorkOrder.sOfficeCode = objSession.OfficeCode;
                objWorkOrder.sClientIP = sClientIP;
                objWorkOrder.sWFOId = hdfWFOId.Value;
                objWorkOrder.sWFAutoId = hdfWFOAutoId.Value;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void GenerateWorkOrderReport(ClsRepairerWorkorder objWorkOrder)
        {
            string strParam = string.Empty;
            string sAET = string.Empty;
            string sSTO = string.Empty;
            string sAO = string.Empty;
            string sDo = string.Empty;
            string sOffcName = string.Empty;
            string sLevelOfApproval = string.Empty;
            ArrayList sNameList = new ArrayList();
            string soffCode = objSession.OfficeCode;
            if (objSession.OfficeCode.Length > 3)
            {
                ClsRepairerWorkorder objOffName = new ClsRepairerWorkorder();
                sOffcName = objOffName.getofficeName(objSession.OfficeCode);
            }
            else
            {
                sOffcName = objSession.OfficeName;
            }

            //sLevelOfApproval = objWorkOrder.getLevelOfApproval(objWorkOrder, txtFailureId.Text, soffCode);
            sLevelOfApproval = Convert.ToString(getApprovalLevel());
            sNameList = objWorkOrder.getCreatedByUserName("", soffCode);
            //  string sSubDiv = objWorkOrder.getsubdivName(txtFailureId.Text);
            string sWFDataId = objWorkOrder.sWFDataId;
            string sWoId = objWorkOrder.sWOId;
            string sTaskType = objWorkOrder.sTaskType;
            // string sIncuredcost = txtInncured.Text;


            Session["UserNameList"] = sNameList;
            strParam = "id=BulkWorkOrderRepairer&WFDataId=" + sWFDataId + "&LApprovel=" + sLevelOfApproval + "&OffCode=" + sOffcName + "&TaskType=" + sTaskType + "&WoId=" + sWoId;
            RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
        }
        public void ApproveRejectAction()
        {
            try
            {
                clsApproval objApproval = new clsApproval();

                if (txtComment.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Comments/Remarks");
                    txtComment.Focus();
                    return;

                }

                objApproval.sCrby = objSession.UserId;
                objApproval.sOfficeCode = objSession.OfficeCode;
                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;
                objApproval.sWFAutoId = hdfWFOAutoId.Value;
                objApproval.sFailType = txtFailType.Text;
                objApproval.sGuarentyType = hdfGuarenteeType.Value;
                if (txtActiontype.Text == "A")
                {
                    objApproval.sApproveStatus = "1";
                }
                if (txtActiontype.Text == "R")
                {
                    objApproval.sApproveStatus = "3";
                }
                if (txtActiontype.Text == "M")
                {
                    objApproval.sApproveStatus = "2";
                    objApproval.sDescription = hdfAppDesc.Value;
                    objApproval.sBOId = hdfboid.Value;
                }

                string sClientIP = string.Empty;

                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    int le = ipRange.Length - 1;
                    sClientIP = ipRange[0];
                }
                else
                {
                    sClientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                objApproval.sClientIp = sClientIP;

                bool bResult = false;
                if (txtActiontype.Text == "M")
                {
                    objApproval.sWFDataId = hdfWFDataId.Value;
                    if (hdfRejectApproveRef.Value == "RA")
                    {
                        objApproval.sApproveStatus = "1";
                    }
                    objApproval.sBOId = hdfboid.Value;

                    objApproval.sFormName = "RepairerBulkWorkOrder";

                    bResult = objApproval.ModifyApproveWFRequest(objApproval);
                }
                else
                {
                    bResult = objApproval.ApproveWFRequest(objApproval);
                }

                if (bResult == true)
                {

                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");
                        cmdSave.Enabled = false;
                        ClsRepairerWorkorder objWO = new ClsRepairerWorkorder();
                        objWO.sWFDataId = objApproval.sWFDataId;
                        objWO.sWFObjectId = objApproval.sWFObjectId;
                        objWO.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWO);
                        }
                        if (objSession.RoleId == "3")
                        {
                            // string sCommWONo = txtRepWoNo1.Text.Trim().Replace("'", "") + "/" + txtRepWoNo2.Text.Trim().Replace("'", "") + "/" + txtRepWoNo3.Text.Trim().Replace("'", "");
                        }
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {

                        ShowMsgBox("Rejected Successfully");
                        cmdSave.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        ShowMsgBox("Modified and Approved Successfully");
                        cmdSave.Enabled = false;
                        ClsRepairerWorkorder objWo = new ClsRepairerWorkorder();
                        objWo.sWFDataId = objApproval.sWFDataId;
                        objWo.sWFObjectId = objApproval.sWFObjectId;
                        objWo.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWo);
                        }
                        if (objSession.RoleId == "3")
                        {
                            clsWorkOrder objWO = new clsWorkOrder();
                            // string sCommWONo = txtRepWoNo1.Text.Trim().Replace("'", "") + "/" + txtRepWoNo2.Text.Trim().Replace("'", "") + "/" + txtRepWoNo3.Text.Trim().Replace("'", "");
                        }
                    }
                }
                else
                {
                    ShowMsgBox("Selected Record Already Approved");
                    return;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void DisableCopy()
        {
            try
            {
                txtBulkWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtBulkWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtBulkWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool ValidateGridValue(string sTcCode)
        {
            bool bValidate = false;
            try
            {
                ArrayList objArrlist = new ArrayList();

                foreach (GridViewRow row in grdBulkWo.Rows)
                {
                    objArrlist.Add(((Label)row.FindControl("lblTCCode")).Text.Trim());
                }

                if (objArrlist.Contains(sTcCode))
                {
                    ShowMsgBox("Transformer Already Added");
                    DataTable dtFaultTc = (DataTable)ViewState["FaultTC"];
                    grdBulkWo.DataSource = dtFaultTc;
                    grdBulkWo.DataBind();
                    return bValidate;
                }

                bValidate = true;
                return bValidate;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return bValidate;
            }
        }
        public void Reset()
        {
            try
            {


                txtWOId.Text = string.Empty;


                clsApproval objLevel = new clsApproval();
                string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                {
                    cmdSave.Text = "Submit";
                }
                else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                {
                    cmdSave.Text = "Approve";
                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        protected void grdBulkWo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string strParam = string.Empty;
                if (e.CommandName == "View")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

                    string sTccode = ((Label)row.FindControl("lbltccode")).Text;

                    ClsRepairerEstimate objGetFailID = new ClsRepairerEstimate();

                    string[] Arr = new string[3];
                    Arr = GetPreviewReport(sTccode);
                    if (Arr[1].ToString() == "2")
                    {
                        ShowMsgBox(Arr[0].ToString());
                        return;
                    }


                    strParam = "id=RefinedEstimationSOrepairer&sWFOID=" + Arr[2] + "&sDtrcode=" + sTccode + "&FailType=" + "1";
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");


                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public int getApprovalLevel()
        {

            int Level = 0;
            try
            {
                if (objSession.RoleId == "7")
                {
                    Level = 1;
                }
                else if (objSession.RoleId == "24")
                {
                    Level = 2;
                }
                else if (objSession.RoleId == "21")
                {
                    Level = 3;
                }
                else
                    Level = 4;
                return Level;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Level;
            }

        }

        public string[] GetPreviewReport(string sTCCode)
        {
            string[] Arr = new string[3];
            try
            {
                clsApproval objApproval = new clsApproval();
                Arr = objApproval.GetApprovedpreview(sTCCode, "71");
                if (Arr[1] == "2")
                {
                    Arr[0] = "Data Not Available!";
                    Arr[1] = "2";
                    return Arr;
                }
                return Arr;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;

            }
        }

        private void ShowMsgBox(string sMsg)
        {
            try
            {
                String sShowMsg = string.Empty;
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                this.Page.RegisterStartupScript("Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void lnkBudgetstat_Click(object sender, EventArgs e)
        {
            try
            {

                string url = "/MasterForms/BudgetStatus.aspx";
                string s = "window.open('" + url + "','mypopup','width=1100,height=800');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();


                objApproval.sFormName = "RepairerBulkWorkOrder";
                objApproval.sRoleId = objSession.RoleId;
                objApproval.sAccessType = "1" + "," + sAccessType;


                bool bResult = objApproval.CheckAccessRights(objApproval);
                if (bResult == false)
                {
                    if (sAccessType == "4")
                    {
                        Response.Redirect("~/UserRestrict.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("~/UserRestrict.aspx", false);
                    }
                }
                return bResult;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;

            }
        }
        public bool CheckFormCreatorLevel()
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "FaultyWorkorder_bulk");
                if (sResult == "1")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

    }
}