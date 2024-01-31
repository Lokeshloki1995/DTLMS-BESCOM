using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IIITS.DTLMS.BL;
using System.Globalization;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Configuration;


namespace IIITS.DTLMS.DTCFailure
{
    public partial class WorkOrder : System.Web.UI.Page
    {
        string strFormCode = "WorkOrder";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                if ((Convert.ToString(Session["clsSession"]) ?? "").Length == 0)
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    Form.DefaultButton = cmdSave.UniqueID;
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    string sWo_id = string.Empty;
                    bool sViewRecord = false;
                    string sFailId = string.Empty;
                    txtCommdate.Attributes.Add("readonly", "readonly");
                    txtDeDate.Attributes.Add("readonly", "readonly");
                    txtDeCrDate.Attributes.Add("readonly", "readonly");

                    txtOFDate.Attributes.Add("readonly", "readonly");
                    txtCreditDate.Attributes.Add("readonly", "readonly");
                    txtWOdate.Attributes.Add("readonly", "readonly");
                    txtDWADate.Attributes.Add("readonly", "readonly");

                    CalendarExtender_txtCommdate.EndDate = System.DateTime.Now;
                    CalendarExtender_txtDeDate.EndDate = System.DateTime.Now;
                    CalendarExtender_txtDeCrDate.EndDate = System.DateTime.Now;

                    CalendarExtender1.EndDate = System.DateTime.Now;
                    CalendarExtender2.EndDate = System.DateTime.Now;
                    CalendarExtender3.EndDate = System.DateTime.Now;
                    CalendarExtender4.EndDate = System.DateTime.Now;

                    if (!IsPostBack)
                    {
                        Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' ORDER BY \"MD_ORDER_BY\" ", "-Select-", cmbRating);
                        Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" from \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='I' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbIssuedBy);
                        Genaral.Load_Combo("SELECT \"MD_NAME\",\"MD_NAME\" from \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='C' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbCapacity);
                        if (HttpUtility.UrlDecode(Request.QueryString["FailType"]) == "" || HttpUtility.UrlDecode(Request.QueryString["FailType"]) == null)
                        {
                            txtFailType.Text = "";
                        }
                        else
                        {
                            txtFailType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["FailType"]));

                        }
                        if (txtFailType.Text == "1" || txtFailType.Text == "1~")
                        {
                            Genaral.Load_Combo("SELECT \"SCHM_ID\",\"SCHM_NAME\" FROM \"TBLDTCSCHEME\"  WHERE \"SCHM_TYPE\"='1' ORDER BY \"SCHM_ID\" ", "--Select--", cmbDtc_Scheme_Type);
                            // cmbDtc_Scheme_Type.SelectedValue = "17";
                        }
                        else if (txtFailType.Text == "2")
                        {
                            Genaral.Load_Combo("SELECT \"SCHM_ID\",\"SCHM_NAME\" FROM \"TBLDTCSCHEME\" WHERE \"SCHM_TYPE\"='2' ORDER BY \"SCHM_ID\" ", "--Select--", cmbDtc_Scheme_Type);

                            Genaral.Load_Combo("select \"SCHM_ACCCODE\", \"SCHM_ACCCODE\" || '~' || \"SCHM_NAME\" AS \"SCHEME\"  from \"TBLDTCSCHEME\" ", "--Select--", cmbAcCode);

                            //cmbDtc_Scheme_Type.SelectedValue = "18";
                        }
                        //else if (txtFailType.Text == "3")
                        //{
                        //    Genaral.Load_Combo("SELECT \"SCHM_ID\",\"SCHM_NAME\" FROM \"TBLDTCSCHEME\" WHERE \"SCHM_TYPE\"='3' ORDER BY \"SCHM_ID\" ", "--Select--", cmbDtc_Scheme_Type);
                        //    //cmbDtc_Scheme_Type.SelectedValue = "18";
                        //}
                        else
                        {
                            Genaral.Load_Combo("SELECT \"SCHM_ID\",\"SCHM_NAME\" FROM \"TBLDTCSCHEME\" ORDER BY \"SCHM_ID\" ", "--Select--", cmbDtc_Scheme_Type);

                        }
                        bool bAccResult;
                        if (objSession.RoleId == "7")
                        {
                            bAccResult = CheckAccessRights("2");
                        }
                        else
                        {
                            bAccResult = CheckAccessRights("4");
                        }

                        if (bAccResult == false)
                        {
                            return;
                        }

                        DisableCopy();

                        CalendarExtender_txtCommdate.EndDate = System.DateTime.Now;
                        CalendarExtender_txtDeDate.EndDate = System.DateTime.Now;
                        CalendarExtender_txtDeCrDate.EndDate = System.DateTime.Now;

                        //if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                        if ((Request.QueryString["ActionType"] ?? "").Length > 0)
                        {
                            txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                        }

                        //if (Request.QueryString["TypeValue"] != null && Convert.ToString(Request.QueryString["TypeValue"]) != "")
                        if ((Request.QueryString["TypeValue"] ?? "").Length > 0)
                        {
                            txtType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TypeValue"]));
                            if (txtType.Text.Contains('~'))
                            {
                                hdfGuarenteeType.Value = txtType.Text.Split('~').GetValue(1).ToString();
                                txtType.Text = txtType.Text.Split('~').GetValue(0).ToString();
                            }

                            //if (Convert.ToString(Request.QueryString["FailType"]) != null)
                            if ((Convert.ToString(Request.QueryString["FailType"]) ?? "").Length > 0)
                            {
                                txtFailType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["FailType"]));
                                if (txtFailType.Text == "1" || txtFailType.Text == "1~")
                                {
                                    txtFailureId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                }
                            }
                            else
                            {
                                if (txtType.Text != "3")
                                {

                                    //if (Request.QueryString["ReferID"] != null && Convert.ToString(Request.QueryString["ReferID"]) != "")
                                    if ((Convert.ToString(Request.QueryString["ReferID"]) ?? "").Length > 0)
                                    {
                                        sWo_id = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                        clsWorkOrder objWO = new clsWorkOrder();
                                        if (txtActiontype.Text == "V")
                                        {
                                            sWo_id = objWO.GetFailCoilType(sWo_id, txtActiontype.Text);
                                        }
                                        else
                                        {
                                            sWo_id = objWO.GetFailCoilType(sWo_id, txtActiontype.Text);
                                        }

                                        txtFailType.Text = sWo_id.Split('~').GetValue(0).ToString();
                                        sViewRecord = true;
                                    }
                                }
                            }
                            ChangeLabelText();
                            cmbIssuedBy.SelectedValue = "2";

                        }

                        //if (Request.QueryString["ReferID"] != null && Convert.ToString(Request.QueryString["ReferID"]) != "")
                        if ((Convert.ToString(Request.QueryString["ReferID"]) ?? "").Length > 0)
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

                            //From New DTC
                            if (txtType.Text == "3")
                            {
                                txtWOId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                if (!txtWOId.Text.Contains("-"))
                                {
                                    GetWODetailsNewDTC();
                                    //cmbCapacity_SelectedIndexChanged(sender, e);
                                }

                                cmdSave.Text = "View";
                            }
                            else if (txtType.Text == "1" && txtFailType.Text == "1")
                            {
                                if (sViewRecord == true)
                                {
                                    sFailId = sWo_id;
                                }
                                else
                                {
                                    sFailId = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                }

                                if (txtActiontype.Text == "V")
                                {
                                    if (sFailId.Contains("-"))
                                    {
                                        GetFailureId(sFailId);
                                    }
                                    else
                                    {
                                        GetFailureId(sFailId);
                                        GetWorkOrderDetails();
                                    }
                                }
                                else
                                {
                                    if (sFailId.Contains("-"))
                                    {
                                        GetFailureId(sFailId);
                                    }
                                    else
                                    {
                                        GetFailureDetails(sFailId, txtType.Text);
                                    }
                                }
                            }
                            else
                            {
                                if (sViewRecord == true)
                                {
                                    txtFailureId.Text = sWo_id;
                                    GetFailureDetails(txtFailureId.Text, txtType.Text);
                                    GetWorkOrderDetails();
                                }
                                else
                                {
                                    txtFailureId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                                    if (txtActiontype.Text == "V")
                                    {
                                        clsWorkOrder objworkOrder = new clsWorkOrder();
                                        if (!txtFailureId.Text.Contains('-') && txtFailType.Text != "2")
                                        {
                                            txtFailureId.Text = objworkOrder.getFailureId(txtFailureId.Text, "1");
                                            goto FailureDetails;
                                        }
                                        else if (txtFailureId.Text.Contains('-'))
                                        {
                                            txtFailureId.Text = objworkOrder.getFailureId(txtFailureId.Text, "2");
                                            goto FailureDetails;
                                        }
                                    }
                                    FailureDetails:
                                    GetFailureDetails(txtFailureId.Text, txtType.Text);
                                }

                                cmbCapacity.Enabled = false;
                                lblDtcScheme.Visible = false;
                                cmbDtc_Scheme_Type.Visible = false;

                                //GetFailureDetails(txtFailureId.Text, txtType.Text);
                                cmdSearch_Click(sender, e);
                                cmbCapacity_SelectedIndexChanged(sender, e);

                                //if (txtActiontype.Text == "A")
                                //{
                                //    GetFailureDetails(txtFailureId.Text, txtType.Text);
                                //    cmdSearch_Click(sender, e);
                                //    cmbCapacity_SelectedIndexChanged(sender, e);
                                //}
                                //else
                                //{
                                //    GetCommAndDecommAmount();
                                //    // GetCommAndDecommAccountCode();
                                //    if (!txtFailureId.Text.Contains("-"))
                                //    {
                                //        GetFailureId(txtFailureId.Text);
                                //        GetWorkOrderDetails();
                                //        cmdSearch_Click(sender, e);
                                //        cmbCapacity_SelectedIndexChanged(sender, e);
                                //    }
                                //}


                            }

                            //Check Indent Done to Update Work Order Details If Indent Done Restrict to Update
                            //ValidateFormUpdate();
                        }

                        // Call Search Window
                        LoadSearchWindow();

                        //WorkFlow / Approval
                        WorkFlowConfig();
                        if (objSession.sRoleType == "1")
                        {
                            if (Convert.ToString(Session["BOID"]) == "74")
                            {

                                Session["BOID"] = "74";
                                ViewState["BOID"] = "74";
                            }
                            else
                            {
                                Session["BOID"] = "11";
                                ViewState["BOID"] = "11";
                            }
                        }
                        else
                        {
                            ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        }

                    }
                    ApprovalHistoryView.BOID = Convert.ToString(ViewState["BOID"]);

                    //if (Convert.ToString(Request.QueryString["sWoRecordID"]) != null && Convert.ToString(Request.QueryString["sWoRecordID"]) != "")
                    if ((Convert.ToString(Request.QueryString["sWoRecordID"]) ?? "").Length > 0)
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
                        ApprovalHistoryView.sRecordId = sWo_id;
                    }


                    if (txtActiontype.Text == "M")
                    {
                        clsApproval objLevel = new clsApproval();
                        string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                        if (sLevel != "" && sLevel != null)
                        {
                            if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdSave.Text = " Modify and Submit";
                            }
                            else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdSave.Text = " Modify and Approve";
                            }
                        }
                    }
                    else if (txtActiontype.Text == "A")
                    {
                        clsApproval objLevel = new clsApproval();
                        string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);

                        if (sLevel != "" && sLevel != null)
                        {
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
                }
            }

            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void GetFailureDetails(string sFailId, string FailType = "")
        {
            DataTable dtWODetails = new DataTable();
            try
            {
                clsWorkOrder objWorkOrder = new clsWorkOrder();

                if (Convert.ToString(Request.QueryString["sWoRecordID"]) != null && Convert.ToString(Request.QueryString["sWoRecordID"]) != "")
                {
                    string sWoRecordID = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"]));
                    if (sWoRecordID.Contains('-'))
                    {
                        dtWODetails = objWorkOrder.FailDetails(sFailId, FailType, txtFailType.Text, txtActiontype.Text, hdfGuarenteeType.Value, sWoRecordID);
                    }
                }
                else
                {
                    dtWODetails = objWorkOrder.FailDetails(sFailId, FailType, txtFailType.Text, txtActiontype.Text, hdfGuarenteeType.Value);
                }

                if (dtWODetails.Rows.Count > 0)
                {
                    cmbCapacity.SelectedValue = Convert.ToString(dtWODetails.Rows[0]["EST_CAPACITY"]);
                }

                if (FailType != "2" && txtFailType.Text != "2")
                {
                    if (dtWODetails.Rows.Count > 0)
                    {
                        if (dtWODetails.Columns.Contains("TR_ID"))
                        {
                            cmbRepairer.SelectedValue = Convert.ToString(dtWODetails.Rows[0]["TR_ID"]);
                            txtPGRSminor.Text = Convert.ToString(dtWODetails.Rows[0]["DF_PGRS_DOCKET"]);
                        }
                    }


                }
                else
                    cmbRepairer.SelectedValue = "0";
                if (dtWODetails.Rows.Count > 0)
                {
                    txtFailureId.Text = Convert.ToString(dtWODetails.Rows[0]["DF_ID"]);
                    txtDTCCode.Text = Convert.ToString(dtWODetails.Rows[0]["DF_DTC_CODE"]);
                    txtDTCName.Text = Convert.ToString(dtWODetails.Rows[0]["DT_NAME"]);
                    txtFailureDate.Text = Convert.ToString(dtWODetails.Rows[0]["DF_DATE"]);
                    txtCapacity.Text = Convert.ToString(dtWODetails.Rows[0]["EST_CAPACITY"]);
                    txtPGRS.Text = Convert.ToString(dtWODetails.Rows[0]["DF_PGRS_DOCKET"]);
                    txtFailType.Text = Convert.ToString(dtWODetails.Rows[0]["EST_FAIL_TYPE"]);
                    txtTCCode.Text = Convert.ToString(dtWODetails.Rows[0]["DF_EQUIPMENT_ID"]);
                    //hdfSubdivName.Value = dtWODetails.Rows[0]["SUBDIV"].ToString();
                    //hdfdivName.Value = dtWODetails.Rows[0]["DIV"].ToString();
                    cmbCapacity.Enabled = false;
                    //cmbRepairer.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetFailureId(string sFailId)
        {
            DataTable dtWODetails = new DataTable();
            string sFailure_id = string.Empty;
            try
            {
                clsWorkOrder objWorkOrder = new clsWorkOrder();
                if (Request.QueryString["sWoRecordID"] != null && Convert.ToString(Request.QueryString["sWoRecordID"]) != "")
                {
                    string sWoRecordID = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"]));
                    if (sWoRecordID.Contains('-'))
                    {
                        sFailure_id = objWorkOrder.FailureId(sFailId, sWoRecordID);
                    }
                }
                else
                {
                    sFailure_id = objWorkOrder.FailureId(sFailId);
                }

                if (sFailure_id != "")
                {

                    txtFailureId.Text = sFailure_id;
                }
                //if (sFailure_id != null)
                //{
                //    GetFailureDetails(sFailId);
                //}
                //else
                //{
                GetFailureDetails(sFailure_id);
                //}

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
                txtComWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtComWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtComWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                txtDeWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                txtDeCrWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeCrWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeCrWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");


                txtOFWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtOFWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtOFWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                txtComWoNo1.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtComWoNo2.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtComWoNo3.Attributes.Add("onkeydown", "return noCopyKey(event);");

                txtDeWoNo1.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtDeWoNo2.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtDeWoNo3.Attributes.Add("onkeydown", "return noCopyKey(event);");

                txtDeCrWoNo1.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeCrWoNo2.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtDeCrWoNo3.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                txtOFWoNo1.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtOFWoNo2.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtOFWoNo3.Attributes.Add("onkeydown", "return noCopyKey(event);");

                txtTTKWO1.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtTTKWO2.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtTTKWO3.Attributes.Add("onkeydown", "return noCopyKey(event);");
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetWorkOrderDetails()
        {
            try
            {
                string sWoRecordID = string.Empty;
                clsWorkOrder objWorkOrder = new clsWorkOrder();
                objWorkOrder.sFailureId = txtFailureId.Text;

                if (Convert.ToString(Request.QueryString["sWoRecordID"]) != null && Convert.ToString(Request.QueryString["sWoRecordID"]) != "")
                {
                    sWoRecordID = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["sWoRecordID"]));
                }

                if ((sWoRecordID != null && sWoRecordID != "") && !sWoRecordID.Contains('-') || (objWorkOrder.sFailureId != ""))
                {
                    objWorkOrder.GetWorkOrderDetails(objWorkOrder);

                    txtWOId.Text = objWorkOrder.sWOId;
                    txtFailureId.Text = objWorkOrder.sFailureId;
                    txtFailureDate.Text = objWorkOrder.sFailureDate;
                    hdfFailureId.Value = objWorkOrder.sFailureId;
                    if (txtType.Text == "2" || txtType.Text == "4")
                    {
                        cmbCapacity.SelectedValue = objWorkOrder.sEnhancedCapacity;
                    }
                    else
                        cmbCapacity.SelectedValue = objWorkOrder.sCapacity;

                    if (objWorkOrder.sCommWoNo != null && objWorkOrder.sCommWoNo != "0")
                    {
                        cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;
                        txtAcCode.Text = objWorkOrder.sAccCode;
                        txtOFAccCode.Text = objWorkOrder.sAccCode;

                        txtComWoNo1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(2).ToString();
                        txtComWoNo2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                        txtComWoNo3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();

                        //txtComWoNo.Text = objWorkOrder.sCommWoNo;
                        txtCommdate.Text = objWorkOrder.sCommDate;
                        txtCommAmount.Text = objWorkOrder.sCommAmmount;
                        if (objWorkOrder.sDeWoNo != null && objWorkOrder.sDeWoNo != "0")
                        {
                            txtDeWoNo1.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(2).ToString();
                            txtDeWoNo2.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(3).ToString();
                            txtDeWoNo3.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(4).ToString();
                        }

                        // txtDeWoNo.Text = objWorkOrder.sDeWoNo;
                        txtDeDate.Text = objWorkOrder.sDeCommDate;
                        txtDeAmount.Text = objWorkOrder.sDeCommAmmount;
                        txtDecAccCode.Text = objWorkOrder.sDecomAccCode;
                        if (objWorkOrder.sDeCreditWO != null && objWorkOrder.sDeCreditWO != "0")
                        {
                            txtDeCrWoNo1.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(2).ToString();
                            txtDeCrWoNo2.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(3).ToString();
                            txtDeCrWoNo3.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(4).ToString();

                            txtDeCrDate.Text = objWorkOrder.sDeCreditDate;
                            txtDeCrAmount.Text = objWorkOrder.sDeCreditAmount;
                            txtDeCrAccCode.Text = objWorkOrder.sDeCreditAccCode;
                        }


                        cmbCapacity.SelectedValue = objWorkOrder.sNewCapacity;

                        if (objWorkOrder.sOFCommWoNo != null && objWorkOrder.sOFCommWoNo != "0")
                        {
                            dvOilFileration.Visible = true;
                            dvComm.Attributes.Add("class", "span6");
                            dvOilFileration.Attributes.Add("class", "span6");
                            txtOFWoNo1.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(2).ToString();
                            txtOFWoNo2.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(3).ToString();
                            txtOFWoNo3.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(4).ToString();

                            txtOFDate.Text = objWorkOrder.sDeCommDate;
                            txtOFAmount.Text = objWorkOrder.sDeCommAmmount;
                            txtOFAccCode.Text = objWorkOrder.sDecomAccCode;
                        }
                        else
                        {
                            dvOilFileration.Visible = false;
                        }

                        if (objWorkOrder.sCreditWO != null && objWorkOrder.sCreditWO != "0" && objWorkOrder.sCreditWO != "")
                        {
                            divCreditWO.Visible = false;
                            dvComm.Attributes.Add("class", "span6");
                            divCreditWO.Attributes.Add("class", "span6");
                            txtCreditWO1.Text = objWorkOrder.sCreditWO.Split('/').GetValue(2).ToString();
                            txtCreditWO2.Text = objWorkOrder.sCreditWO.Split('/').GetValue(3).ToString();
                            txtCreditWO3.Text = objWorkOrder.sCreditWO.Split('/').GetValue(4).ToString();

                            txtCreditDate.Text = objWorkOrder.sCreditDate;
                            txtCreditAmount.Text = objWorkOrder.sCreditAmount;
                            txtCreditACCode.Text = objWorkOrder.sCreditAccCode;
                        }
                        else
                        {
                            dvOilFileration.Visible = false;
                            divCreditWO.Visible = false;
                        }

                        if (txtType.Text == "3")
                        {
                            cmbSection.SelectedValue = objWorkOrder.sRequestLoc;
                        }
                        cmbDtc_Scheme_Type.SelectedValue = Convert.ToString(objWorkOrder.sDtcScheme);
                        cmbRepairer.SelectedValue = objWorkOrder.sRepairer;

                        //cmdSave.Text = "Update";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }

                    else
                    {
                        //cmdSave.Text = "Save";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void GetWODetailsNewDTC()
        {
            try
            {
                clsWorkOrder objWorkOrder = new clsWorkOrder();
                objWorkOrder.sWOId = txtWOId.Text;
                objWorkOrder.GetWODetailsForNewDTC(objWorkOrder);

                cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;
                txtAcCode.Text = objWorkOrder.sAccCode;

                if (objWorkOrder.sCommWoNo != null)
                {
                    txtComWoNo1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(2).ToString();
                    txtComWoNo2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                    txtComWoNo3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();
                }
                //txtComWoNo.Text = objWorkOrder.sCommWoNo;
                txtCommdate.Text = objWorkOrder.sCommDate;
                txtCommAmount.Text = objWorkOrder.sCommAmmount;
                cmbCapacity.SelectedValue = objWorkOrder.sNewCapacity;
                cmbSection.SelectedValue = objWorkOrder.sRequestLoc;
                //cmdSave.Text = "Update";
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

        public bool ValidateForm()
        {
            bool bValidate = false;
            string estimatedate = string.Empty;

            try
            {
                if (txtType.Text != "3" && txtFailType.Text != "1")
                {
                    if (txtFailureId.Text.Trim().Length == 0)
                    {
                        txtFailureId.Focus();
                        ShowMsgBox("Enter Failure Id");
                        return bValidate;
                    }
                }

                if (cmbIssuedBy.SelectedIndex == 0)
                {
                    cmbIssuedBy.Focus();
                    ShowMsgBox("Select Issued By");
                    return bValidate;
                }
                if (cmbCapacity.SelectedIndex == 0)
                {
                    cmbCapacity.Focus();
                    ShowMsgBox("Select Capacity");
                    return bValidate;
                }
                if (txtType.Text == "2" || txtType.Text == "4")
                {
                    if (cmbAcCode.SelectedIndex == 0 && txtAcCode.Text == "")
                    {
                        cmbAcCode.Focus();
                        ShowMsgBox("Select A/C Code ");
                        return bValidate;
                    }
                }
                if (txtType.Text == "3")
                {
                    if (cmbSection.SelectedIndex == 0)
                    {
                        cmbSection.Focus();
                        ShowMsgBox("Select Section");
                        return bValidate;
                    }
                }
                if (txtType.Text == "3")
                {
                    if (cmbSection.SelectedIndex == 0)
                    {
                        cmbSection.Focus();
                        ShowMsgBox("Select Section");
                        return bValidate;
                    }
                    if (cmdflowType.SelectedValue == "2")
                    {

                        if (txtTTKWO1.Text.Trim().Length == 0 || txtTTKWO2.Text.Trim().Length == 0 || txtTTKWO3.Text.Trim().Length == 0)
                        {
                            //txtComWoNo.Focus();
                            ShowMsgBox("Enter TTK WO Number");
                            return bValidate;
                        }
                        if (txtttkAuto.Text.Length == 0)
                        {
                            txtttkAuto.Focus();
                            ShowMsgBox("AutoGenerate number should not be null");
                            return bValidate;
                        }

                        if (txtVendor.Text.Length == 0)
                        {
                            txtVendor.Focus();
                            ShowMsgBox("Please Enter Vendor Name");
                            return bValidate;
                        }
                        if (txtWOdate.Text.Trim() == "")
                        {
                            txtWOdate.Focus();
                            ShowMsgBox("Enter Work Order Date");
                            return bValidate;
                        }
                        if (cmbRating.SelectedIndex <= 0)
                        {
                            cmbRating.Focus();
                            ShowMsgBox("Please select Rating");
                            return bValidate;
                        }
                        if (txtACCcode.Text.Trim() == "")
                        {
                            txtACCcode.Focus();
                            ShowMsgBox("Enter Acount Code");
                            return bValidate;
                        }

                        bValidate = true;
                        return bValidate;
                    }
                    else if (cmdflowType.SelectedValue == "1")
                    {
                        if (txtTTKWO1.Text.Trim().Length == 0 || txtTTKWO2.Text.Trim().Length == 0 || txtTTKWO3.Text.Trim().Length == 0)
                        {
                            //txtComWoNo.Focus();
                            ShowMsgBox("Enter PTK WO Number");
                            return bValidate;
                        }
                        if (cmbRating.SelectedIndex <= 0)
                        {
                            cmbRating.Focus();
                            ShowMsgBox("Please select Rating");
                            return bValidate;
                        }
                        if (txtWOdate.Text.Trim() == "")
                        {
                            txtWOdate.Focus();
                            ShowMsgBox("Enter Work Order Date");
                            return bValidate;
                        }
                        if (txtACCcode.Text.Trim() == "")
                        {
                            txtACCcode.Focus();
                            ShowMsgBox("Enter Acount Code");
                            return bValidate;
                        }

                        bValidate = true;
                        return bValidate;
                    }
                }
                if (ChkOFCheck.Checked == true)
                {
                    if (txtOFWoNo1.Text.Trim().Length == 0 || txtOFWoNo2.Text.Trim().Length == 0 || txtOFWoNo3.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter Oil Filteration Wo No");
                        //txtDeWoNo.Focus();
                        return bValidate;
                    }
                    if (txtOFDate.Text.Trim().Length == 0)
                    {
                        txtOFDate.Focus();
                        ShowMsgBox("Enter Oil Filteration Date");
                        return bValidate;
                    }
                    if (txtOFAmount.Text.Trim().Length == 0)
                    {
                        txtOFAmount.Focus();
                        ShowMsgBox("Enter Oil Filteration Amount");
                        return bValidate;
                    }
                    if (txtOFAccCode.Text.Trim().Length == 0)
                    {
                        txtOFAccCode.Focus();
                        ShowMsgBox("Enter Oil Filteration Account Code");
                        return bValidate;
                    }
                    if (txtActiontype.Text == "M")
                    {
                        if (txtOFWoNo1.Text.Trim() == "0" || txtOFWoNo2.Text.Trim() == "0" || txtOFWoNo3.Text.Trim() == "0")
                        {
                            ShowMsgBox("Enter Oil Filteration Wo No");
                            //txtDeWoNo.Focus();
                            return bValidate;
                        }
                        if (txtOFDate.Text.Trim() == "0")
                        {
                            txtOFDate.Focus();
                            ShowMsgBox("Enter Oil Filteration Date");
                            return bValidate;
                        }
                        if (txtOFAmount.Text.Trim() == "0")
                        {
                            txtOFAmount.Focus();
                            ShowMsgBox("Enter Oil Filteration Amount");
                            return bValidate;
                        }
                    }

                }
                if (txtComWoNo1.Text.Trim().Length == 0 || txtComWoNo2.Text.Trim().Length == 0 || txtComWoNo3.Text.Trim().Length == 0)
                {
                    //txtComWoNo.Focus();
                    ShowMsgBox("Enter Commission WO Number");
                    return bValidate;
                }

                if (txtCommdate.Text.Trim() == "")
                {
                    txtCommdate.Focus();
                    ShowMsgBox("Enter Commissioning Date");
                    return bValidate;
                }
                if (txtType.Text != "3")
                {
                    clsWorkOrder objwo = new clsWorkOrder();
                    estimatedate = objwo.getestimatedate(txtFailureId.Text);
                }
                if (txtType.Text != "3")
                {

                    if (txtCommdate.Text != "" && txtCommdate.Text != null && txtCommdate.Text != "0")
                    {
                        string sResult = Genaral.DateComparisionTransaction(txtCommdate.Text, estimatedate, false, false);
                        if (sResult == "1")
                        {
                            ShowMsgBox("Commisioning Work Order Date should be Greater than or Equal to Estimation Date");
                            return bValidate;
                        }
                    }
                }

                if (txtCommAmount.Text.Trim().Length == 0)
                {
                    ShowMsgBox("Enter Commissioning Amount");
                    txtCommAmount.Focus();
                    return bValidate;
                }
                //if (txtType.Text != "2" || txtType.Text != "4")
                //{
                //    if (txtAcCode.Text.Trim().Length == 0)
                //    {
                //        txtAcCode.Focus();
                //        ShowMsgBox("Enter Commissioning Account Code");
                //        return bValidate;
                //    }
                //}
                if (txtType.Text != "3")
                {
                    if (txtOFAccCode.Text.Trim().Length == 0)
                    {
                        txtOFAccCode.Focus();
                        ShowMsgBox("Enter Oil Filteration Account Code");
                        return bValidate;
                    }
                }

                //txtType -> 1 - failure, 2- Enhance, 4-fail with enhance
                //txtFailType -> 1 - Single Coil , 2 - Multi Coil
                if (txtType.Text != "3" && txtFailType.Text != "1")
                {
                    if (txtDeWoNo1.Text.Trim().Length == 0 || txtDeWoNo2.Text.Trim().Length == 0 || txtDeWoNo3.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter Decommissioning Wo No");
                        //txtDeWoNo.Focus();
                        return bValidate;
                    }
                    if (txtDeCrWoNo1.Text.Trim().Length == 0 || txtDeCrWoNo2.Text.Trim().Length == 0 || txtDeCrWoNo3.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter Credited Wo No");

                        return bValidate;
                    }
                    if (txtDeDate.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter DeCommissioning Date");
                        txtDeDate.Focus();
                        return bValidate;
                    }
                    if (txtDeCrDate.Text.Trim().Length == 0)
                    {
                        ShowMsgBox("Enter Credited Date");
                        txtDeCrDate.Focus();
                        return bValidate;
                    }
                    if (txtType.Text != "3")
                    {
                        string sResult;
                        if (txtDeDate.Text != "0" && txtDeDate.Text != null && txtDeDate.Text != "")
                        {
                            sResult = Genaral.DateComparisionTransaction(txtDeDate.Text, estimatedate, false, false);
                            if (sResult == "1")
                            {
                                ShowMsgBox("DeCommisioning Work Order Date should be Greater than or Equal to Estimation Date ");
                                return bValidate;
                            }
                        }

                        if (txtOFDate.Text != "" && txtOFDate.Text != null && txtOFDate.Text != "0")
                        {
                            sResult = Genaral.DateComparisionTransaction(txtOFDate.Text, estimatedate, false, false);
                            if (sResult == "1")
                            {
                                ShowMsgBox("OilFilteration Work Order Date should be Greater than or Equal to Estimation Date ");
                                return bValidate;
                            }
                        }
                    }

                    if (txtDeAmount.Text.Length == 0)
                    {
                        txtDeAmount.Focus();
                        ShowMsgBox("Enter DeCommissioning Amount");
                        return bValidate;
                    }
                    if (txtDeCrAmount.Text.Length == 0)
                    {
                        txtDeCrAmount.Focus();
                        ShowMsgBox("Enter Credited Amount");
                        return bValidate;
                    }
                    if (txtDecAccCode.Text.Trim().Length == 0)
                    {
                        txtDecAccCode.Focus();
                        ShowMsgBox("Enter DeCommissioning Account Code");
                        return bValidate;
                    }
                    if (txtDeCrAccCode.Text.Trim().Length == 0)
                    {
                        txtDeCrAccCode.Focus();
                        ShowMsgBox("Enter Credited Account Code");
                        return bValidate;
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtDeAmount.Text, "^(\\d{1,8})?(\\.\\d{1,2})?$"))
                    {
                        ShowMsgBox("Enter Valid DeCommissioning Amount (eg:111111.00)");
                        return false;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtDeAmount.Text, "[-+]?[0-9]{0,7}\\.?[0-9]{0,2}"))
                    {
                        ShowMsgBox("Enter Valid DeCommissioning Amount (eg:111111.00)");
                        return false;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtDeCrAmount.Text, "^(\\d{1,8})?(\\.\\d{1,2})?$"))
                    {
                        ShowMsgBox("Enter Valid Credited Amount (eg:111111.00)");
                        return false;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtDeCrAmount.Text, "[-+]?[0-9]{0,7}\\.?[0-9]{0,2}"))
                    {
                        ShowMsgBox("Enter Valid Credited Amount (eg:111111.00)");
                        return false;
                    }
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtCommAmount.Text, "^(\\d{1,8})?(\\.\\d{1,2})?$"))
                {
                    ShowMsgBox("Enter Valid Commissioning Amount (eg:111111.00)");
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtCommAmount.Text, "[-+]?[0-9]{0,7}\\.?[0-9]{0,2}"))
                {
                    ShowMsgBox("Enter Valid Commissioning Amount (eg:111111.00)");
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtOFAmount.Text, "^(\\d{1,8})?(\\.\\d{1,2})?$"))
                {
                    ShowMsgBox("Enter Valid OilFilteration Amount (eg:111111.00)");
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtOFAmount.Text, "[-+]?[0-9]{0,7}\\.?[0-9]{0,2}"))
                {
                    ShowMsgBox("Enter Valid OilFilteration Amount (eg:111111.00)");
                    return false;
                }

                //  Photo Save DTLMSDocs
                string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NotAllowFileFormat"]);

                if (fupWODocument.PostedFile.ContentLength != 0)
                {
                    string sWOFileExt = string.Empty;
                    string sWOFileName = string.Empty;
                    string sDirectory = string.Empty;

                    sWOFileExt = System.IO.Path.GetExtension(fupWODocument.FileName).ToString().ToLower();
                    sWOFileExt = ";" + sWOFileExt.Remove(0, 1) + ";";

                    if (sFileExt.Contains(sWOFileExt))
                    {
                        ShowMsgBox("Invalid File Format");
                        return false;
                    }
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
                //txtFailureId.Text = string.Empty;
                //txtFailureDate.Text = string.Empty;
                //cmbIssuedBy.SelectedIndex = 0;
                //txtDTCCode.Text = string.Empty;
                //txtDTCName.Text = string.Empty;
                //txtTCCode.Text = string.Empty;
                //txtWOId.Text = string.Empty;
                //txtComWoNo1.Text = string.Empty;
                //txtComWoNo2.Text = string.Empty;
                //txtComWoNo3.Text = string.Empty;
                //txtCommdate.Text = string.Empty;
                //txtCommAmount.Text = string.Empty;                
                //txtAcCode.Text = string.Empty;
                //txtOFAccCode.Text = string.Empty;
                //txtDeDate.Text = string.Empty;
                //txtDeAmount.Text = string.Empty;
                //txtDeWoNo1.Text = string.Empty;
                //txtDeWoNo2.Text = string.Empty;
                //txtDeWoNo3.Text = string.Empty;
                //txtDecAccCode.Text = string.Empty;
                // priyanka changed below code
                //txtComWoNo2.Text = string.Empty;

                txtWOId.Text = string.Empty;
                txtComWoNo1.Text = string.Empty;
                txtComWoNo3.Text = string.Empty;
                txtCommdate.Text = string.Empty;
                txtCommAmount.Text = string.Empty;
                txtDeDate.Text = string.Empty;
                txtDeAmount.Text = string.Empty;
                txtDeWoNo1.Text = string.Empty;
                txtDeWoNo2.Text = string.Empty;
                txtDeWoNo3.Text = string.Empty;
                txtDeCrWoNo1.Text = string.Empty;
                txtDeCrWoNo2.Text = string.Empty;
                txtDeCrWoNo3.Text = string.Empty;
                txtDeCrAmount.Text = string.Empty;
                txtDeCrDate.Text = string.Empty;
                txtOFWoNo1.Text = string.Empty;
                txtOFWoNo2.Text = string.Empty;
                txtOFWoNo3.Text = string.Empty;
                txtOFAmount.Text = string.Empty;
                txtOFDate.Text = string.Empty;
                txtVendor.Text = string.Empty;

                //added newly
                txtACCcode.Text = string.Empty;
                txtDWAName.Text = string.Empty;
                txtDWADate.Text = string.Empty;
                txtTTKAmount.Text = string.Empty;
                cmbRating.ClearSelection();
                //----end 

                txtTTKWO1.Text = string.Empty;
                //txtTTKWO2.Text = string.Empty;
                txtTTKWO3.Text = string.Empty;
                txtWOdate.Text = string.Empty;
                cmbCapacity.ClearSelection();
                cmbSection.ClearSelection();
                cmbDtc_Scheme_Type.ClearSelection();
                cmdflowType.ClearSelection();

                if (cmdflowType.Text == "1")
                {
                    txtttkAuto.Visible = false;

                }

                hdfFailureId.Value = string.Empty;
                clsApproval objLevel = new clsApproval();
                string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                if (sLevel != "")
                {

                    if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = "Submit";
                    }
                    else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                    {
                        cmdSave.Text = "Approve";
                    }
                }
                cmdSearch.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                if ((Convert.ToString(Session["clsSession"]) ?? "").Length == 0)
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    string[] Arr = new string[2];
                    clsWorkOrder objWorkOrder = new clsWorkOrder();
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
                        clsWorkOrder objWrkOrder = new clsWorkOrder();
                        if (hdfApproveStatus.Value != "")
                        {

                            if (hdfApproveStatus.Value != "3")
                            {
                                objWrkOrder.sWFDataId = hdfWFDataId.Value;
                                objWrkOrder.sTaskType = txtType.Text;
                                if (txtType.Text == "3")
                                {
                                    GenerateWorkOrderReport(objWrkOrder);
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
                                objWrkOrder.sWFDataId = objWorkOrder.getWoDataId(txtFailureId.Text, txtWOId.Text);
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
                                    GenerateWorkOrderReport(objWrkOrder);
                                }
                                else
                                {
                                    GenerateWorkOrderReport(objWrkOrder);
                                }
                            }
                            else
                            {
                                objWrkOrder.sWFDataId = objWorkOrder.getWoDataId(txtFailureId.Text, txtWOId.Text);
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
                        objWorkOrder.sFailureId = txtFailureId.Text.Trim();
                        objWorkOrder.sFailureDate = txtFailureDate.Text.Replace("'", "");
                        objWorkOrder.sIssuedBy = cmbIssuedBy.SelectedValue;
                        objWorkOrder.sGuarentyType = hdfGuarenteeType.Value;
                        objWorkOrder.sdtccode = txtDTCCode.Text;
                        if (txtCapacity.Text != "")
                        {
                            objWorkOrder.sCapacity = txtCapacity.Text.Trim();
                        }
                        else
                        {
                            objWorkOrder.sCapacity = "0";
                        }
                        objWorkOrder.sDTCCode = txtDTCCode.Text.Trim();
                        objWorkOrder.sDTCName = txtDTCName.Text.Trim();
                        objWorkOrder.sFailType = txtFailType.Text.Trim();

                        objWorkOrder.sNewCapacity = cmbCapacity.SelectedValue.Trim();

                        if (txtFailureId.Text != "")
                        {
                            dtOffName = objWorkOrder.GetofficeName(txtFailureId.Text);
                        }
                        else
                        {
                            dtOffName = objWorkOrder.GetofficeNameBySectionCode(cmbSection.SelectedValue);
                        }

                        string sCommWONo = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtComWoNo1.Text.Trim().Replace("'", "") + "/" + txtComWoNo2.Text.Trim().Replace("'", "") + "/" + txtComWoNo3.Text.Trim().Replace("'", "");
                        objWorkOrder.sCommWoNo = sCommWONo.Trim().ToUpper();
                        objWorkOrder.sCommDate = txtCommdate.Text.Trim().Replace("'", "");
                        objWorkOrder.sCommAmmount = txtCommAmount.Text.Trim().Replace("'", "");
                        if (txtType.Text == "2" || txtType.Text == "4")
                        {
                            objWorkOrder.sAccCode = cmbAcCode.SelectedItem.Text.Split('~').GetValue(0).ToString();
                        }
                        else
                        {
                            objWorkOrder.sAccCode = txtAcCode.Text.Trim().Replace("'", "");
                        }
                        string sDeWoNo = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtDeWoNo1.Text.Trim().Replace("'", "") + "/" + txtDeWoNo2.Text.Trim().Replace("'", "") + "/" + txtDeWoNo3.Text.Trim().Replace("'", "");
                        objWorkOrder.sDeWoNo = sDeWoNo.Trim().ToUpper();
                        objWorkOrder.sDeCommDate = txtDeDate.Text.Trim().Replace("'", "");
                        objWorkOrder.sDeCommAmmount = txtDeAmount.Text.Trim().Replace("'", "");
                        objWorkOrder.sDecomAccCode = txtDecAccCode.Text.Trim().Replace("'", "");

                        string sDeCreditWO = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtDeCrWoNo1.Text.Trim().Replace("'", "") + "/" + txtDeCrWoNo2.Text.Trim().Replace("'", "") + "/" + txtDeCrWoNo3.Text.Trim().Replace("'", "");
                        objWorkOrder.sDeCreditWO = sDeCreditWO.Trim().ToUpper();
                        objWorkOrder.sDeCreditDate = txtDeCrDate.Text.Trim().Replace("'", "");
                        objWorkOrder.sDeCreditAmount = txtDeCrAmount.Text.Trim().Replace("'", "");
                        objWorkOrder.sDeCreditAccCode = txtDeCrAccCode.Text.Trim().Replace("'", "");

                        if (objWorkOrder.sDeCreditDate == "")
                        {
                            objWorkOrder.sDeCreditWO = "0";
                            objWorkOrder.sDeCreditDate = "0";
                            objWorkOrder.sDeCreditAmount = "0";
                            objWorkOrder.sDeCreditAccCode = "0";
                        }


                        if (ChkOFCheck.Checked == true)
                        {
                            string sOFWONo = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtOFWoNo1.Text.Trim().Replace("'", "") + "/" + txtOFWoNo2.Text.Trim().Replace("'", "") + "/" + txtOFWoNo3.Text.Trim().Replace("'", "");
                            objWorkOrder.sOFCommWoNo = sOFWONo.Trim().ToUpper();
                            objWorkOrder.sOFCommDate = txtOFDate.Text.Trim().Replace("'", "");
                            objWorkOrder.sOFCommAmmount = txtOFAmount.Text.Trim().Replace("'", "");
                            objWorkOrder.sOFAccCode = txtOFAccCode.Text.Trim().Replace("'", "");
                        }
                        else
                        {
                            objWorkOrder.sOFCommWoNo = "0";
                            objWorkOrder.sOFCommDate = "0";
                            objWorkOrder.sOFCommAmmount = "0";
                            objWorkOrder.sOFAccCode = "0";
                        }
                        //if (txtFailType.Text == "1")
                        //{
                        if (ChkCredit.Checked == true)
                        {
                            string sCreditWONO = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtCreditWO1.Text.Trim().Replace("'", "") + "/" + txtCreditWO2.Text.Trim().Replace("'", "") + "/" + txtCreditWO3.Text.Trim().Replace("'", "");
                            objWorkOrder.sCreditWO = sCreditWONO.Trim().ToUpper();
                            objWorkOrder.sCreditDate = txtCreditDate.Text.Trim().Replace("'", "");
                            objWorkOrder.sCreditAmount = txtCreditAmount.Text.Trim().Replace("'", "");
                            objWorkOrder.sCreditAccCode = txtCreditACCode.Text.Trim().Replace("'", "");
                        }
                        else
                        {
                            objWorkOrder.sCreditWO = "0";
                            objWorkOrder.sCreditDate = "0";
                            objWorkOrder.sCreditAmount = "0";
                            objWorkOrder.sCreditAccCode = "0";
                        }
                        //}
                        //else
                        //{
                        //    string sCreditWONO = dtOffName.Rows[0]["DIV"].ToString() + "/" + dtOffName.Rows[0]["SUBDIV"].ToString() + "/" + txtCreditWO1.Text.Trim().Replace("'", "") + "/" + txtCreditWO2.Text.Trim().Replace("'", "") + "/" + txtCreditWO3.Text.Trim().Replace("'", "");
                        //    objWorkOrder.sCreditWO = sCreditWONO.Trim().ToUpper();
                        //    objWorkOrder.sCreditDate = txtCreditDate.Text.Trim().Replace("'", "");
                        //    objWorkOrder.sCreditAmount = txtCreditAmount.Text.Trim().Replace("'", "");
                        //    objWorkOrder.sCreditAccCode = txtCreditACCode.Text.Trim().Replace("'", "");
                        //}
                        if (txtType.Text == "3")
                        {
                            if (cmdflowType.SelectedValue == "2")
                            {
                                string sTTKPTKWONO = dtOffName.Rows[0]["DIV"].ToString() + "/" + System.DateTime.Now.ToString("yyyy") + "-" + System.DateTime.Now.AddYears(1).ToString("yy") + "/" + txtACCcode.Text.Trim().Replace("'", "") + "/" + txtTTKWO1.Text.Trim().Replace("'", "") + "/" + txtTTKWO2.Text.Trim().Replace("'", "") + "/" + txtTTKWO3.Text.Trim().Replace("'", "");
                                objWorkOrder.sCommWoNo = sTTKPTKWONO.Trim().ToUpper();
                                objWorkOrder.sTtkStatus = "1";
                                objWorkOrder.sTtkAutoNo = txtttkAuto.Text;
                                // objWorkOrder.sTtkManual = txtttkManual.Text.ToUpper();
                                objWorkOrder.sCommDate = txtWOdate.Text;
                                objWorkOrder.sTtkVendorName = txtVendor.Text.ToUpper();
                                if (txtTTKAmount.Text.Length > 0)
                                {
                                    objWorkOrder.sCommAmmount = txtTTKAmount.Text.Trim().Replace("'", "");
                                }
                                else
                                {
                                    objWorkOrder.sCommAmmount = "0";
                                }
                                objWorkOrder.sDecomAccCode = "0";
                                objWorkOrder.sRating = cmbRating.SelectedValue;
                                if (txtDWAName.Text.Length > 0)
                                {
                                    objWorkOrder.sDWAname = txtDWAName.Text.ToUpper();
                                }
                                if (txtDWADate.Text.Length > 0)
                                {
                                    objWorkOrder.sDWAdate = txtDWADate.Text.ToUpper();
                                }
                                else
                                {
                                    objWorkOrder.sDWAdate = "0";
                                }

                            }
                            else
                            {
                                string sTTKPTKWONO = dtOffName.Rows[0]["DIV"].ToString() + "/" + System.DateTime.Now.ToString("yyyy") + "-" + System.DateTime.Now.AddYears(1).ToString("yy") + "/" + txtACCcode.Text.Trim().Replace("'", "") + "/" + txtTTKWO1.Text.Trim().Replace("'", "") + "/" + txtTTKWO2.Text.Trim().Replace("'", "") + "/" + txtTTKWO3.Text.Trim().Replace("'", "");
                                objWorkOrder.sCommWoNo = sTTKPTKWONO.Trim().ToUpper();
                                objWorkOrder.sTtkStatus = "0";
                                // objWorkOrder.sTtkAutoNo = txtttkAuto.Text;
                                // objWorkOrder.sTtkManual = txtttkManual.Text.ToUpper();
                                objWorkOrder.sCommDate = txtWOdate.Text;
                                //objWorkOrder.sTtkVendorName = txtVendor.Text.ToUpper();
                                if (txtTTKAmount.Text.Length > 0)
                                {
                                    objWorkOrder.sCommAmmount = txtTTKAmount.Text.Trim().Replace("'", "");
                                }
                                else
                                {
                                    objWorkOrder.sCommAmmount = "0";
                                }

                                objWorkOrder.sDecomAccCode = "0";
                                objWorkOrder.sRating = cmbRating.SelectedValue;
                                if (txtDWAName.Text.Length > 0)
                                {
                                    objWorkOrder.sDWAname = txtDWAName.Text.ToUpper();
                                }
                                if (txtDWADate.Text.Length > 0)
                                {
                                    objWorkOrder.sDWAdate = txtDWADate.Text.ToUpper();
                                }
                                else
                                {
                                    objWorkOrder.sDWAdate = "0";
                                }

                            }
                        }


                        if (objWorkOrder.sDeCommAmmount == "")
                        {
                            objWorkOrder.sDeCommAmmount = "0";
                        }

                        objWorkOrder.sCrBy = objSession.UserId;
                        objWorkOrder.sLocationCode = objSession.OfficeCode;
                        objWorkOrder.sTaskType = txtType.Text;
                        if (cmbDtc_Scheme_Type.SelectedValue == "--Select--" || cmbDtc_Scheme_Type.SelectedValue == "" || cmbDtc_Scheme_Type.SelectedValue == null)
                        {
                            objWorkOrder.sDtcScheme = Convert.ToInt32(cmbDtc_Scheme_Type.SelectedIndex);
                        }
                        else
                        {
                            objWorkOrder.sDtcScheme = Convert.ToInt32(cmbDtc_Scheme_Type.SelectedValue);
                        }
                        objWorkOrder.sRepairer = cmbRepairer.SelectedValue;
                        if (txtType.Text == "3")
                        {
                            objWorkOrder.sRequestLoc = cmbSection.SelectedValue.Trim();
                        }

                        if (fupWODocument.PostedFile.ContentLength != 0)
                        {

                            string sWOFileName = System.IO.Path.GetFileName(fupWODocument.PostedFile.FileName);

                            fupWODocument.SaveAs(Server.MapPath("~/DTLMSFiles" + "/" + sWOFileName));
                            string sDirectory = Server.MapPath("~/DTLMSFiles" + "/" + sWOFileName);
                            objWorkOrder.sWOFilePath = sDirectory;
                        }

                        #region Approve And Reject

                        if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                        {
                            if (hdfWFDataId.Value != "0")
                            {
                                ApproveRejectAction1();

                                if (objSession.sTransactionLog == "1")
                                {
                                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (Workorder) Failure/enhancement ");
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

                            Arr = objWorkOrder.SaveUpdateWorkOrder1(objWorkOrder);
                            if (Arr[1].ToString() == "0")
                            {
                                hdfWFDataId.Value = objWorkOrder.sWFDataId;
                                hdfAppDesc.Value = objWorkOrder.sApprovalDesc;
                                hdfboid.Value = Arr[2];
                                ApproveRejectAction1();
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
                        Arr = objWorkOrder.SaveUpdateWorkOrder1(objWorkOrder);

                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (Workorder) Failure/enhancement ");
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
                                GenerateWorkOrderReport(objWorkOrder);
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
                                ApproveRejectAction1();
                                if (objSession.sTransactionLog == "1")
                                {
                                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (Workorder) Failure/enhancement ");
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

            }

            catch (Exception ex)
            {
                ShowMsgBox("Something Went Wrong Please Approve Once Again");
                // lblMessage.Text = clsException.ErrorMsg();

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }


        }
        private void GenerateWorkOrderReport(clsWorkOrder objWorkOrder)
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
                clsWorkOrder objOffName = new clsWorkOrder();
                sOffcName = objOffName.getofficeName(objSession.OfficeCode);
            }
            else
            {
                sOffcName = objSession.OfficeName;
            }

            //sLevelOfApproval = objWorkOrder.getLevelOfApproval(objWorkOrder, txtFailureId.Text, soffCode);
            sLevelOfApproval = Convert.ToString(getApprovalLevel());
            sNameList = objWorkOrder.getCreatedByUserName(txtFailureId.Text, soffCode);
            string sSubDiv = string.Empty;
            if (txtFailureId.Text.Length > 0)
            {
                sSubDiv = objWorkOrder.getsubdivName(txtFailureId.Text);
            }
            string sWFDataId = objWorkOrder.sWFDataId;
            string sWoId = objWorkOrder.sWOId;
            string sTaskType = objWorkOrder.sTaskType;
            Session["UserNameList"] = sNameList;

            if (sTaskType != "3")
            {
                strParam = "id=WorkOrderPreview&WFDataId=" + sWFDataId + "&LApprovel=" + sLevelOfApproval + "&OffCode=" + sOffcName + "&TaskType=" + sTaskType + "&WoId=" + sWoId + "&sSubDivName=" + sSubDiv;
                RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
            }
            else
            {
                string sFlowType = cmdflowType.SelectedValue;
                string sprojName = cmdflowType.SelectedItem.Text;
                string section = cmbSection.SelectedItem.Text;
                string scheme = cmbDtc_Scheme_Type.SelectedItem.Text;
                strParam = "id=WorkOrderNewDtcCommission&WFDataId=" + sWFDataId + "&LApprovel=" + sLevelOfApproval + "&OffCode=" + sOffcName + "&TaskType=" + sFlowType + "&WoId=" + sWoId + "&sSubDivName=" + sSubDiv + "&sprojName=" + sprojName + "&section=" + section + "&scheme=" + scheme;
                RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
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

        protected void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("WorkOrderView.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ValidateFormUpdate()
        {
            try
            {
                clsWorkOrder objWorkOrder = new clsWorkOrder();

                if (objWorkOrder.ValidateUpdate(txtFailureId.Text, txtWOId.Text, txtType.Text) == true)
                {
                    cmdReset.Enabled = false;
                    cmdSave.Enabled = false;
                }
                else
                {
                    cmdReset.Enabled = true;
                    cmdSave.Enabled = true;
                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                clsFailureEntry objFailure = new clsFailureEntry();
                clsWorkOrder objwrkrder = new clsWorkOrder();
                if ((txtFailType.Text == "2" && txtActiontype.Text == "A") || (txtFailType.Text == "1" && txtActiontype.Text == "A"))
                {

                }
                else if (txtFailType.Text == "1" && txtActiontype.Text == "V")
                {

                }
                else
                {
                    if (hdfFailureId.Value != "")
                    {
                        txtFailureId.Text = hdfFailureId.Value;
                    }

                }


                objFailure.sFailureId = txtFailureId.Text;





                objFailure.GetFailureDetails(objFailure);

                txtFailureDate.Text = objFailure.sFailureDate;
                txtDTCName.Text = objFailure.sDtcName;
                txtDTCCode.Text = objFailure.sDtcCode;
                txtDeclaredBy.Text = objFailure.sCrby;
                txtTCCode.Text = objFailure.sDtcTcCode;
                cmbCapacity.SelectedValue = objFailure.sEnhancedCapacity;
                txtDTCId.Text = objFailure.sDtcId;
                txtTCId.Text = objFailure.sTCId;
                txtPGRS.Text = objFailure.sdocketno;
                //if (cmbCapacity.SelectedIndex == 0)
                //{
                //txtCapacity.Text = objFailure.sDtcCapacity;

                if (txtType.Text == "1")
                {
                    txtCapacity.Text = objFailure.sDtcCapacity;
                    cmbCapacity.SelectedValue = objFailure.sDtcCapacity;
                }
                else
                {
                    txtCapacity.Text = objFailure.sDtcCapacity;
                    cmbCapacity.SelectedValue = objFailure.sEnhancedCapacity;
                }



            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ChangeLabelText()
        {
            string Off_Code = string.Empty;
            try
            {
                // txtType = 1 -> singleCoil, 2 -> MultiCoil
                // txtFailType = 1 -> Failure , 2 -> Enhance , 4 -> Failure with Enhance

                if ((txtType.Text == "1" && txtFailType.Text != "1") || (txtType.Text == "4" && txtFailType.Text != "1"))
                {
                    lblIDText.Text = "Failure ID";
                    lblDateText.Text = "Failure Date";
                    //dvOilFileration.Style.Add("display", "none");
                    if (ChkOFCheck.Checked == false)
                    {
                        dvOilFileration.Visible = false;
                    }
                    if (ChkCredit.Checked == false)
                    {
                        divCreditWO.Visible = false;
                    }
                    dvOFCheck.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "none");
                    //dvCreditWO.Style.Add("display", "none");
                }
                else if (txtType.Text == "2" && txtFailType.Text != "2")
                {
                    dvOFCheck.Style.Add("display", "block");
                    // dvCreditWO.Style.Add("display", "block");
                    lblIDText.Text = "Enhancement ID";
                    lblDateText.Text = "Enhancement Entry Date";
                    if (ChkOFCheck.Checked == false)
                    {
                        dvOilFileration.Visible = false;
                    }
                    if (ChkCredit.Checked == false)
                    {
                        divCreditWO.Visible = false;
                    }
                    dvTTKorPTKflow.Style.Add("display", "none");
                }
                else if (txtType.Text == "2" && txtFailType.Text == "2")
                {
                    dvOFCheck.Style.Add("display", "block");
                    // dvCreditWO.Style.Add("display", "none");
                    lblIDText.Text = "Enhancement ID";
                    lblDateText.Text = "Enhancement Entry Date";
                    if (ChkOFCheck.Checked == false)
                    {
                        dvOilFileration.Visible = false;
                    }
                    if (ChkCredit.Checked == false)
                    {
                        divCreditWO.Visible = false;
                    }
                    dvTTKorPTKflow.Style.Add("display", "none");
                }
                else if (txtType.Text == "1" && txtFailType.Text == "1")
                {
                    // Single Coil with WGP then Continue same as multicoil process, so we will get decommisiion part
                    //if (hdfGuarenteeType.Value == "WGP" || hdfGuarenteeType.Value == "WRGP")
                    //{
                    //    lblIDText.Text = "Failure ID";
                    //    lblDateText.Text = "Failure Date";
                    //    //dvOilFileration.Style.Add("display", "none");
                    //    if (ChkOFCheck.Checked == false)
                    //    {
                    //        dvOilFileration.Visible = false;
                    //    }
                    //    if (ChkCredit.Checked == false)
                    //    {
                    //        divCreditWO.Visible = false;
                    //        dvCreditWO.Visible = false;
                    //    }
                    //    dvOFCheck.Style.Add("display", "none");
                    //    dvCreditWO.Style.Add("display", "block");
                    //}
                    //else
                    //{
                    // Single Coil with AGP then Continue same as Singlecoil process, so we wont get decommisiion part
                    if (ChkCredit.Checked == false)
                    {
                        divCreditWO.Visible = false;
                    }
                    dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvComm.Attributes.Add("class", "span12");
                    dvBasic.Style.Add("display", "none");
                    dvSection.Style.Add("display", "none");
                    dvRepairer.Style.Add("display", "block");
                    dvTypeofflow.Style.Add("display", "none");
                    dvTTKorPTKflow.Style.Add("display", "none");
                    //  dvCreditWO.Style.Add("display", "block");

                    if (objSession.OfficeCode != null && objSession.OfficeCode != "")
                    {
                        if (objSession.OfficeCode.Length > Constants.Division)
                        {
                            Off_Code = objSession.OfficeCode.Substring(0, Constants.Division);
                        }
                        else
                            Off_Code = objSession.OfficeCode;
                        Genaral.Load_Combo("SELECT \"TR_ID\", UPPER(\"TR_NAME\") FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TRO_OFF_CODE\" =  '" + Off_Code + "' ORDER BY \"TR_NAME\" ", "--Select--", cmbRepairer);
                    }
                    else
                    {
                        Genaral.Load_Combo("SELECT \"TR_ID\", UPPER(\"TR_NAME\") FROM \"TBLTRANSREPAIRER\"", "--Select--", cmbRepairer);
                    }


                    lnkDTCDetails.Visible = false;
                    lnkDTrDetails.Visible = false;
                    cmbRepairer.Enabled = true;
                    cmdViewEstimate.Visible = true; // for single coile before it was false
                                                    //  }

                }
                else
                {

                    dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    tltptk.Visible = true;
                    tltttk.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvComm.Style.Add("display", "none");
                    dvTypeofflow.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "block");
                    dvBasic.Style.Add("display", "none");
                    dvSection.Style.Add("display", "block");
                    if (objSession.sRoleType == "2")
                    {
                        //string abc = "SELECT \"OM_CODE\", \"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE '" + clsStoreOffice.GetOfficeCode(objSession.OfficeCode, "OM_CODE") + "' ORDER BY \"OM_CODE\" ";
                        Genaral.Load_Combo("SELECT \"OM_CODE\", \"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE " + clsStoreOffice.GetOfficeCode(objSession.OfficeCode, "OM_CODE") + " ORDER BY \"OM_CODE\" ", "--Select--", cmbSection);
                    }
                    else
                    {
                        Genaral.Load_Combo("SELECT \"OM_CODE\", \"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE CAST(\"OM_CODE\" AS TEXT) LIKE '" + objSession.OfficeCode + "%' ORDER BY \"OM_CODE\" ", "--Select--", cmbSection);
                    }

                    lnkDTCDetails.Visible = false;
                    lnkDTrDetails.Visible = false;

                    cmdViewEstimate.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        #region Access Rights
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                if (Convert.ToString(Session["BOID"]) == "74")
                {

                    objApproval.sFormName = "WorkOrder_sdo";
                    objApproval.sRoleId = objSession.RoleId;
                    objApproval.sAccessType = "1" + "," + sAccessType;
                }
                else

                {
                    objApproval.sFormName = "WorkOrder";
                    objApproval.sRoleId = objSession.RoleId;
                    objApproval.sAccessType = "1" + "," + sAccessType;
                }

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

        #endregion
        protected void cmbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmdflowType.SelectedValue == "2")
                {

                    if (cmbSection.SelectedValue == "--Select--")
                    {
                        ShowMsgBox("please select section");
                        cmbSection.Focus();
                        return;
                    }

                    clsWorkOrder objWoorkorder = new clsWorkOrder();
                    txtttkAuto.Text = objWoorkorder.GenerateWOautoNo(cmbSection.SelectedValue);
                    dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvComm.Style.Add("display", "none");
                    dvTypeofflow.Style.Add("display", "block");
                    dvBasic.Style.Add("display", "none");
                    dvSection.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "block");
                }
                else
                {
                    txtttkAuto.Text = null;
                    dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvBasic.Style.Add("display", "none");
                    dvComm.Style.Add("display", "none");
                    dvSection.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "block");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmdflowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmdflowType.SelectedValue == "2")
                {

                    if (cmbSection.SelectedValue == "--Select--")
                    {
                        ShowMsgBox("please select section");
                        cmbSection.Focus();
                        return;
                    }

                    clsWorkOrder objWorkorder = new clsWorkOrder();
                    txtttkAuto.Visible = true;
                    txtttkAuto.Text = objWorkorder.GenerateWOautoNo(cmbSection.SelectedValue);
                    dvOilFileration.Visible = false;
                    tltttk.Visible = true;
                    tltptk.Visible = false;
                    dvttk.Visible = true;
                    divCreditWO.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvComm.Style.Add("display", "none");
                    dvTypeofflow.Style.Add("display", "block");
                    dvBasic.Style.Add("display", "none");
                    dvSection.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "block");
                }
                else
                {
                    tltttk.Visible = false;
                    tltptk.Visible = true;
                    dvttk.Visible = false;
                    txtttkAuto.Text = null;
                    txtVendor.Text = null;
                    dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    dvDecomm.Style.Add("display", "none");
                    dvComm.Style.Add("display", "none");
                    dvBasic.Style.Add("display", "none");
                    dvSection.Style.Add("display", "block");
                    dvTTKorPTKflow.Style.Add("display", "block");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void WorkFlowObjects(clsWorkOrder objWorkOrder)
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


                objWorkOrder.sFormName = "WorkOrder";
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

        #region Workflow/Approval

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
                    pnlApproval.Enabled = false;
                    cmdReset.Enabled = true;
                    //cmbRepairer.Enabled = true;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdSave.Text = "Reject";
                    pnlApproval.Enabled = false;
                    cmbRepairer.Enabled = false;
                    cmdReset.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    cmdSave.Text = "Modify and Approve";
                    pnlApproval.Enabled = true;
                    cmbRepairer.Enabled = true;
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
                    pnlApproval.Enabled = true;
                    //cmbRepairer.Enabled = false;
                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {

                    pnlApproval.Enabled = true;
                    //dvComments.Style.Add("display", "none");

                    // To handle Record From Reject 
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
                objApproval.sdtccode = txtDTCCode.Text;
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

                if (cmdflowType.SelectedValue == "2")
                {
                    objApproval.sTTKStatus = "1";
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
                    if (objApproval.sBOId == "11")
                    {
                        objApproval.sFormName = "WorkOrder";
                    }
                    else
                    {
                        objApproval.sFormName = "WorkOrder_sdo";
                    }
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
                        clsWorkOrder objWO = new clsWorkOrder();
                        objWO.sWFDataId = objApproval.sWFDataId;
                        objWO.sWFObjectId = objApproval.sWFObjectId;
                        objWO.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                            GenerateWorkOrderReport(objWO);
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWO);
                        }
                        if (objSession.RoleId == "3")
                        {

                            string sCommWONo = txtComWoNo1.Text.Trim().Replace("'", "") + "/" + txtComWoNo2.Text.Trim().Replace("'", "") + "/" + txtComWoNo3.Text.Trim().Replace("'", "");
                            //objWO.SendSMStoSectionOfficer(txtFailureId.Text,txtDTCCode.Text, sCommWONo.ToUpper(), txtDTCName.Text);
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
                        clsWorkOrder objWo = new clsWorkOrder();
                        objWo.sWFDataId = objApproval.sWFDataId;
                        objWo.sWFObjectId = objApproval.sWFObjectId;
                        objWo.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                            GenerateWorkOrderReport(objWo);
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWo);
                        }
                        if (objSession.RoleId == "3")
                        {
                            clsWorkOrder objWO = new clsWorkOrder();
                            string sCommWONo = txtComWoNo1.Text.Trim().Replace("'", "") + "/" + txtComWoNo2.Text.Trim().Replace("'", "") + "/" + txtComWoNo3.Text.Trim().Replace("'", "");
                            //objWO.SendSMStoSectionOfficer(txtFailureId.Text, txtDTCCode.Text, sCommWONo.ToUpper(), txtDTCName.Text);
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


        public void ApproveRejectAction1()
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
                objApproval.failureId = txtFailureId.Text;
                objApproval.sGuarentyType = hdfGuarenteeType.Value;
                objApproval.sdtccode = txtDTCCode.Text;
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

                if (cmdflowType.SelectedValue == "2")
                {
                    objApproval.sTTKStatus = "1";
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
                    if (objApproval.sBOId == "11")
                    {
                        objApproval.sFormName = "WorkOrder";
                    }
                    else
                    {
                        objApproval.sFormName = "WorkOrder_sdo";
                    }
                    bResult = objApproval.ModifyApproveWFRequest1(objApproval);
                }
                else
                {
                    bResult = objApproval.ApproveWFRequest1(objApproval);
                }

                if (bResult == true)
                {

                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");
                        cmdSave.Enabled = false;
                        clsWorkOrder objWO = new clsWorkOrder();
                        objWO.sWFDataId = objApproval.sWFDataId;
                        objWO.sWFObjectId = objApproval.sWFObjectId;
                        objWO.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                            GenerateWorkOrderReport(objWO);
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWO);
                        }
                        if (objSession.RoleId == "3")
                        {

                            string sCommWONo = txtComWoNo1.Text.Trim().Replace("'", "") + "/" + txtComWoNo2.Text.Trim().Replace("'", "") + "/" + txtComWoNo3.Text.Trim().Replace("'", "");
                            //objWO.SendSMStoSectionOfficer(txtFailureId.Text,txtDTCCode.Text, sCommWONo.ToUpper(), txtDTCName.Text);
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
                        clsWorkOrder objWo = new clsWorkOrder();
                        objWo.sWFDataId = objApproval.sWFDataId;
                        objWo.sWFObjectId = objApproval.sWFObjectId;
                        objWo.sTaskType = txtType.Text;
                        if (txtType.Text == "3")
                        {
                            GenerateWorkOrderReport(objWo);
                        }
                        else
                        {
                            GenerateWorkOrderReport(objWo);
                        }
                        if (objSession.RoleId == "3")
                        {
                            clsWorkOrder objWO = new clsWorkOrder();
                            string sCommWONo = txtComWoNo1.Text.Trim().Replace("'", "") + "/" + txtComWoNo2.Text.Trim().Replace("'", "") + "/" + txtComWoNo3.Text.Trim().Replace("'", "");
                            //objWO.SendSMStoSectionOfficer(txtFailureId.Text, txtDTCCode.Text, sCommWONo.ToUpper(), txtDTCName.Text);
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
                // lblMessage.Text = clsException.ErrorMsg();

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

                throw ex;
            }
        }

        public void WorkFlowConfig()
        {
            string sApproveStatus = string.Empty;
            try
            {
                //WorkFlow / Approval
                //if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                if ((Convert.ToString(Request.QueryString["ActionType"]) ?? "").Length > 0)
                {

                    //if (Session["WFOId"] != null && Convert.ToString(Session["WFOId"]) != "")
                    if ((Convert.ToString(Session["WFOId"]) ?? "").Length > 0)
                    {
                        hdfWFDataId.Value = Convert.ToString(Session["WFDataId"]);
                        hdfWFOId.Value = Convert.ToString(Session["WFOId"]);
                        hdfWFOAutoId.Value = Convert.ToString(Session["WFOAutoId"]);
                        hdfApproveStatus.Value = Convert.ToString(Session["ApproveStatus"]);

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
                            clsWorkOrder objWrkOrder = new clsWorkOrder();
                            if (txtFailureId.Text != "" && txtFailureId.Text != null)
                            {
                                objWrkOrder.sWFDataId = objWrkOrder.getWoDataId(txtFailureId.Text, txtWOId.Text);
                                hdfWFDataId.Value = objWrkOrder.sWFDataId;
                                if (sApproveStatus == "3")
                                {
                                    sApproveStatus = "2";
                                }
                            }
                            else
                            {

                                objWrkOrder.sWFDataId = objWrkOrder.getWoDataId(txtFailureId.Text, txtWOId.Text);
                                hdfWFDataId.Value = objWrkOrder.sWFDataId;
                                if (sApproveStatus == "3")
                                {
                                    sApproveStatus = "2";
                                }

                            }
                        }


                        if (hdfWFDataId.Value != "")
                        {
                            GetWODetailsFromXML(hdfWFDataId.Value);
                        }
                    }
                    SetControlText();
                    ControlEnableDisable();
                    if (txtActiontype.Text == "V")
                    {
                        cmdSave.Text = "View";
                        cmdSave.Enabled = true;
                        cmdReset.Enabled = false;
                        dvComments.Style.Add("display", "none");
                        cmbRepairer.Enabled = false;
                    }
                }
                else
                {
                    //if (cmdSave.Text != "Save" && cmdSave.Text != "View")
                    //{
                    //    cmdSave.Enabled = false;
                    //}
                    if (txtType.Text != "3")
                    {
                        cmdSave.Text = "View";
                    }
                }

                DisableControlForView();
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

        public bool CheckFormCreatorLevel()
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "WorkOrder");
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

        public void DisableControlForView()
        {
            try
            {
                if (cmdSave.Text.Contains("View"))
                {
                    pnlApproval.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        #endregion

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
                    Response.Redirect("WorkOrderView.aspx", false);
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetCommAndDecommAmount()
        {
            try
            {
                clsEstimation objEst = new clsEstimation();
                objEst.sFailureId = txtFailureId.Text;
                objEst.GetCommAndDecommAmount(objEst);

                txtCommAmount.Text = objEst.sTotal;
                txtDeAmount.Text = objEst.sDecommTotal;
                //txtCrAmount.Text = objEst.sCrTotal;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetCommAndDecommAccountCode()
        {
            try
            {
                clsWorkOrder objWO = new clsWorkOrder();
                objWO.sCapacity = cmbCapacity.SelectedValue;

                objWO.GetCommDecommAccCode(objWO);

                if (txtType.Text == "1")
                {
                    txtAcCode.Text = objWO.sAccCode;
                    txtOFAccCode.Text = objWO.sAccCode;
                    txtCreditACCode.Text = "16-205";
                    txtDecAccCode.Text = objWO.sDecomAccCode;
                    txtDeCrAccCode.Text = objWO.sDeCreditAccCode;
                }
                else if (txtType.Text == "2" || txtType.Text == "4")
                {

                    txtOFAccCode.Text = objWO.sAccCode;
                    // txtAcCode.Text = objWO.sEnhanceAccCode;
                    txtDecAccCode.Text = objWO.sDecomAccCode;
                    cmbAcCode.Visible = true;
                    lblACcodedesc.Visible = true;
                    txtAcCode.Visible = false;
                    txtDeCrAccCode.Text = objWO.sDeCreditAccCode;

                }
                else if (txtType.Text == "3")
                {
                    txtAcCode.Text = objWO.sAccCode;
                    txtDecAccCode.Text = objWO.sDecomAccCode;
                    txtDeCrAccCode.Text = objWO.sDeCreditAccCode;
                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkDTrDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtTCId.Text));

                string url = "/MasterForms/TcMaster.aspx?TCId=" + sTCId;
                string s = "window.open('" + url + "', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkDTCDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string sDTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtDTCId.Text));

                string url = "/MasterForms/DTCCommision.aspx?QryDtcId=" + sDTCId;
                string s = "window.open('" + url + "', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbCapacity_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetCommAndDecommAccountCode();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ControlEnableDisable()
        {
            try
            {
                txtFailureId.Enabled = false;
                cmdSearch.Visible = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        #region Load From XML
        public void GetWODetailsFromXML(string sWFDataId)
        {
            try
            {
                // If the Data saved in Main Table then this function shd not execute, so done restriction like below
                // And commented for temprary purpose.. nee to change in future

                //if (!txtWOId.Text.Contains("-") && txtWOId.Text!="" )
                //{
                //    return;
                //}

                clsWorkOrder objWorkOrder = new clsWorkOrder();
                objWorkOrder.sWFDataId = sWFDataId;

                objWorkOrder.GetWODetailsFromXML(objWorkOrder);

                if (txtType.Text != "3" && txtFailType.Text == "1")
                {

                    //txtWOId.Text = objWorkOrder.sWOId;
                    txtFailureId.Text = objWorkOrder.sFailureId;
                    //txtFailureDate.Text = objWorkOrder.sFailureDate;
                    hdfFailureId.Value = objWorkOrder.sFailureId;
                    if (objWorkOrder.sCommWoNo != null && objWorkOrder.sCommWoNo != "")
                    {
                        cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;
                        txtAcCode.Text = objWorkOrder.sAccCode;

                        txtComWoNo1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(2).ToString();
                        txtComWoNo2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                        txtComWoNo3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();

                        //txtComWoNo.Text = objWorkOrder.sCommWoNo;
                        txtCommdate.Text = objWorkOrder.sCommDate;
                        txtCommAmount.Text = objWorkOrder.sCommAmmount;
                        if (objWorkOrder.sDeWoNo != null && objWorkOrder.sDeWoNo != "")
                        {
                            txtDeWoNo1.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(2).ToString();
                            txtDeWoNo2.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(3).ToString();
                            txtDeWoNo3.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(4).ToString();

                            // txtDeWoNo.Text = objWorkOrder.sDeWoNo;
                            txtDeDate.Text = objWorkOrder.sDeCommDate;
                            txtDeAmount.Text = objWorkOrder.sDeCommAmmount;
                            txtDecAccCode.Text = objWorkOrder.sDecomAccCode;
                        }
                        if (objWorkOrder.sDeCreditDate != "0" && objWorkOrder.sDeCreditDate != null 
                            && objWorkOrder.sDeCreditDate != "" && objWorkOrder.sDeCreditWO != null)
                        {

                            txtDeCrWoNo1.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(2).ToString();
                            txtDeCrWoNo2.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(3).ToString();
                            txtDeCrWoNo3.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(4).ToString();

                            txtDeCrAmount.Text = objWorkOrder.sDeCreditAmount;
                            txtDeCrAccCode.Text = objWorkOrder.sDeCreditAccCode;
                            txtDeCrDate.Text = objWorkOrder.sDeCreditDate;
                        }

                        else
                        {
                            txtDeCrWoNo1.Text = "0";
                            txtDeCrWoNo2.Text = "0";
                            txtDeCrWoNo3.Text = "0";

                            txtDeCrDate.Text = objWorkOrder.sDeCreditDate;
                            txtDeCrAmount.Text = "0";
                            if (objWorkOrder.sDeCreditAccCode != "0")
                            {
                                txtDeCrAccCode.Text = objWorkOrder.sDeCreditAccCode;
                            }
                            else
                            {
                                txtDeCrAccCode.Text = "16-2057";
                            }
                            //dvOilFileration.Visible = false;
                        }


                        if (objWorkOrder.sOFCommWoNo != "0" && objWorkOrder.sOFCommWoNo != null && objWorkOrder.sOFCommWoNo != "")
                        {
                            dvOilFileration.Visible = true;
                            txtOFWoNo1.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(2).ToString();
                            txtOFWoNo2.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(3).ToString();
                            txtOFWoNo3.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(4).ToString();

                            txtOFDate.Text = objWorkOrder.sOFCommDate;
                            txtOFAmount.Text = objWorkOrder.sOFCommAmmount;

                            txtOFAccCode.Text = objWorkOrder.sOFAccCode;

                        }
                        else
                        {
                            txtOFWoNo1.Text = "0";
                            txtOFWoNo2.Text = "0";
                            txtOFWoNo3.Text = "0";

                            txtOFDate.Text = objWorkOrder.sCommDate;
                            txtOFAmount.Text = "0";
                            if (objWorkOrder.sOFAccCode != "0")
                            {
                                txtOFAccCode.Text = objWorkOrder.sOFAccCode;
                            }
                            else
                            {
                                txtOFAccCode.Text = "14-1507";
                            }

                            dvOilFileration.Visible = false;
                        }

                        if (objWorkOrder.sCreditWO != "0" && objWorkOrder.sCreditWO != null && objWorkOrder.sCreditWO != "")
                        {
                            divCreditWO.Visible = false;
                            txtCreditWO1.Text = objWorkOrder.sCreditWO.Split('/').GetValue(2).ToString();
                            txtCreditWO2.Text = objWorkOrder.sCreditWO.Split('/').GetValue(3).ToString();
                            txtCreditWO3.Text = objWorkOrder.sCreditWO.Split('/').GetValue(4).ToString();

                            txtCreditDate.Text = objWorkOrder.sCreditDate;
                            txtCreditAmount.Text = objWorkOrder.sCreditAmount;
                            txtCreditACCode.Text = objWorkOrder.sCreditAccCode;
                        }
                        else
                        {
                            txtCreditWO1.Text = "0";
                            txtCreditWO2.Text = "0";
                            txtCreditWO3.Text = "0";

                            txtCreditDate.Text = objWorkOrder.sCreditDate;
                            txtCreditAmount.Text = "0";
                            if (objWorkOrder.sCreditAccCode != "0")
                            {
                                txtCreditACCode.Text = objWorkOrder.sCreditAccCode;
                            }
                            else
                            {
                                txtCreditACCode.Text = "16-2057";
                            }

                            divCreditWO.Visible = false;
                        }
                        cmbRepairer.SelectedValue = Convert.ToString(objWorkOrder.sRepairer);
                        cmbCapacity.SelectedValue = objWorkOrder.sNewCapacity;
                        cmbDtc_Scheme_Type.SelectedValue = Convert.ToString(objWorkOrder.sDtcScheme);

                        hdfCrBy.Value = objWorkOrder.sCrBy;
                        if (txtType.Text == "3")
                        {
                            cmbSection.SelectedValue = objWorkOrder.sRequestLoc;
                        }

                        //cmdSave.Text = "Update";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }

                    else
                    {
                        //cmdSave.Text = "Save";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }
                }
                else if (txtType.Text != "3")
                {
                    //txtWOId.Text = objWorkOrder.sWOId;
                    txtFailureId.Text = objWorkOrder.sFailureId;
                    //txtFailureDate.Text = objWorkOrder.sFailureDate;
                    hdfFailureId.Value = objWorkOrder.sFailureId;
                    cmbRepairer.SelectedValue = Convert.ToString(objWorkOrder.sRepairer);
                    cmbDtc_Scheme_Type.SelectedValue = Convert.ToString(objWorkOrder.sDtcScheme);
                    if (objWorkOrder.sCommWoNo != null)
                    {
                        cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;

                        if (txtActiontype.Text == "M")
                        {
                            //txtAcCode.Text = objWorkOrder.sAccCode;
                            txtAcCode.Visible = false;
                            cmbAcCode.Visible = true;
                            cmbAcCode.SelectedValue = objWorkOrder.sAccCode;
                        }
                        else
                        {
                            txtAcCode.Text = objWorkOrder.sAccCode;
                            txtAcCode.Visible = true;
                            cmbAcCode.Visible = false;
                        }
                        if (objWorkOrder.sCommWoNo != null)
                        {
                            txtComWoNo1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(2).ToString();
                            txtComWoNo2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                            txtComWoNo3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();
                        }
                        //txtComWoNo.Text = objWorkOrder.sCommWoNo;
                        txtCommdate.Text = objWorkOrder.sCommDate;
                        txtCommAmount.Text = objWorkOrder.sCommAmmount;
                        if (objWorkOrder.sDeWoNo != null)
                        {
                            txtDeWoNo1.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(2).ToString();
                            txtDeWoNo2.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(3).ToString();
                            txtDeWoNo3.Text = objWorkOrder.sDeWoNo.Split('/').GetValue(4).ToString();
                        }
                        // txtDeWoNo.Text = objWorkOrder.sDeWoNo;
                        txtDeDate.Text = objWorkOrder.sDeCommDate;
                        txtDeAmount.Text = objWorkOrder.sDeCommAmmount;
                        txtDecAccCode.Text = objWorkOrder.sDecomAccCode;
                        if (objWorkOrder.sDeCreditWO != "0" && objWorkOrder.sDeCreditWO != null && objWorkOrder.sDeCreditWO != "")
                        {
                            txtDeCrWoNo1.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(2).ToString();
                            txtDeCrWoNo2.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(3).ToString();
                            txtDeCrWoNo3.Text = objWorkOrder.sDeCreditWO.Split('/').GetValue(4).ToString();

                            // txtDeCrWoNo.Text = objWorkOrder.sDeCrWoNo;
                            txtDeCrAmount.Text = objWorkOrder.sDeCreditAmount;
                            txtDeCrAccCode.Text = objWorkOrder.sDeCreditAccCode;
                            txtDeCrDate.Text = objWorkOrder.sDeCreditDate;

                        }

                        else
                        {
                            txtDeCrWoNo1.Text = "0";
                            txtDeCrWoNo2.Text = "0";
                            txtDeCrWoNo3.Text = "0";

                            txtDeCrDate.Text = "0";
                            txtDeCrAmount.Text = "0";
                            if (objWorkOrder.sDeCreditAccCode != "0")
                            {
                                txtDeCrAccCode.Text = objWorkOrder.sDeCreditAccCode;
                            }
                            else
                            {
                                txtDeCrAccCode.Text = "16-2057";
                            }
                            dvOilFileration.Visible = false;
                        }
                        cmbCapacity.SelectedValue = objWorkOrder.sNewCapacity;

                        if (objWorkOrder.sOFCommWoNo != "0" && objWorkOrder.sOFCommWoNo != null && objWorkOrder.sOFCommWoNo != "")
                        {
                            dvOilFileration.Visible = true;
                            txtOFWoNo1.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(2).ToString();
                            txtOFWoNo2.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(3).ToString();
                            txtOFWoNo3.Text = objWorkOrder.sOFCommWoNo.Split('/').GetValue(4).ToString();

                            txtOFDate.Text = objWorkOrder.sOFCommDate;
                            txtOFAmount.Text = objWorkOrder.sOFCommAmmount;
                            txtOFAccCode.Text = objWorkOrder.sOFAccCode;
                        }
                        else
                        {
                            txtOFWoNo1.Text = "0";
                            txtOFWoNo2.Text = "0";
                            txtOFWoNo3.Text = "0";

                            txtOFDate.Text = "0";
                            txtOFAmount.Text = "0";
                            if (objWorkOrder.sOFAccCode != "0")
                            {
                                txtOFAccCode.Text = objWorkOrder.sOFAccCode;
                            }
                            else
                            {
                                txtOFAccCode.Text = "14-1507";
                            }
                            dvOilFileration.Visible = false;
                        }

                        if (objWorkOrder.sCreditWO != "0" && objWorkOrder.sCreditWO != null && objWorkOrder.sCreditWO != "")
                        {
                            divCreditWO.Visible = false;
                            txtCreditWO1.Text = objWorkOrder.sCreditWO.Split('/').GetValue(2).ToString();
                            txtCreditWO2.Text = objWorkOrder.sCreditWO.Split('/').GetValue(3).ToString();
                            txtCreditWO3.Text = objWorkOrder.sCreditWO.Split('/').GetValue(4).ToString();

                            txtCreditDate.Text = objWorkOrder.sCreditDate;
                            txtCreditAmount.Text = objWorkOrder.sCreditAmount;
                            txtCreditACCode.Text = objWorkOrder.sCreditAccCode;
                        }
                        else
                        {
                            txtCreditWO1.Text = "0";
                            txtCreditWO2.Text = "0";
                            txtCreditWO3.Text = "0";

                            txtCreditDate.Text = "0";
                            txtCreditAmount.Text = "0";
                            if (objWorkOrder.sCreditAccCode != "0")
                            {
                                txtCreditACCode.Text = objWorkOrder.sCreditAccCode;
                            }
                            else
                            {
                                txtCreditACCode.Text = "16-2057";
                            }
                            divCreditWO.Visible = false;
                            //  dvCreditWO.Visible = false;
                        }

                        hdfCrBy.Value = objWorkOrder.sCrBy;
                        if (txtType.Text == "3")
                        {
                            cmbSection.SelectedValue = objWorkOrder.sRequestLoc;
                        }

                        //cmdSave.Text = "Update";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }

                    else
                    {
                        //cmdSave.Text = "Save";
                        txtFailureId.Enabled = false;
                        txtFailureDate.Enabled = false;
                        cmdSearch.Visible = false;
                    }
                }
                else
                {
                    cmbIssuedBy.SelectedValue = objWorkOrder.sIssuedBy;
                    txtAcCode.Text = objWorkOrder.sAccCode;
                    txtACCcode.Text = objWorkOrder.sAccCode;
                    if (objWorkOrder.sTtkStatus == "1")
                    {
                        txtVendor.Text = objWorkOrder.sTtkVendorName.ToUpper();
                        txtttkAuto.Text = objWorkOrder.sTtkAutoNo;
                        // txtttkManual.Text = objWorkOrder.sTtkManual.ToUpper();
                        txtTTKWO1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                        txtTTKWO2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();
                        txtTTKWO3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(5).ToString();
                        txtWOdate.Text = objWorkOrder.sCommDate;
                        txtDWAName.Text = objWorkOrder.sDWAname;
                        txtDWADate.Text = objWorkOrder.sDWAdate;
                        cmbRating.SelectedValue = objWorkOrder.sRating;
                        txtTTKAmount.Text = objWorkOrder.sCommAmmount;


                        cmdflowType.Enabled = true;
                        cmdflowType.SelectedValue = "2";
                        dvOilFileration.Visible = false;
                        divCreditWO.Visible = false;
                        dvttk.Visible = true;
                        dvDecomm.Style.Add("display", "none");
                        dvComm.Style.Add("display", "none");
                        dvTypeofflow.Style.Add("display", "block");
                        dvBasic.Style.Add("display", "none");
                        dvSection.Style.Add("display", "block");
                        dvTTKorPTKflow.Style.Add("display", "block");
                    }
                    else
                    {
                        txtTTKWO1.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(3).ToString();
                        txtTTKWO2.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(4).ToString();
                        txtTTKWO3.Text = objWorkOrder.sCommWoNo.Split('/').GetValue(5).ToString();
                        txtWOdate.Text = objWorkOrder.sCommDate;
                        txtDWAName.Text = objWorkOrder.sDWAname;
                        txtDWADate.Text = objWorkOrder.sDWAdate;
                        cmbRating.SelectedValue = objWorkOrder.sRating;
                        txtTTKAmount.Text = objWorkOrder.sCommAmmount;


                        txtttkAuto.Text = null;
                        dvOilFileration.Visible = false;
                        divCreditWO.Visible = false;
                        dvDecomm.Style.Add("display", "none");
                        dvComm.Style.Add("display", "none");
                        dvBasic.Style.Add("display", "none");
                        dvSection.Style.Add("display", "block");
                        dvTTKorPTKflow.Style.Add("display", "block");
                    }

                    txtCommdate.Text = objWorkOrder.sCommDate;
                    txtCommAmount.Text = objWorkOrder.sCommAmmount;
                    cmbCapacity.SelectedValue = objWorkOrder.sNewCapacity;
                    cmbSection.SelectedValue = objWorkOrder.sRequestLoc;
                    hdfCrBy.Value = objWorkOrder.sCrBy;
                    cmbDtc_Scheme_Type.SelectedValue = Convert.ToString(objWorkOrder.sDtcScheme);
                    cmbSection.Enabled = false;
                    //cmdSave.Text = "Update";
                }


                if (objWorkOrder.sCreditWO != "0" && objWorkOrder.sCreditWO != "" && objWorkOrder.sCreditWO != null)
                {
                    if (objWorkOrder.sDeCommAmmount != "0" && objWorkOrder.sDeCommAmmount != "")
                    {
                        divCreditWO.Attributes.Add("class", "span11");
                    }
                    else
                    {
                        divCreditWO.Attributes.Add("class", "span6");
                        dvComm.Attributes.Add("class", "span6");
                    }

                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        #endregion

        public void LoadSearchWindow()
        {
            try
            {
                string strQry = string.Empty;

                strQry = "Title=Search and Select Transformer Centre failure Details&";
                strQry += "Query=SELECT \"DF_ID\",\"DT_NAME\",\"DT_CODE\" from \"TBLDTCMAST\",\"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\"= \"DT_CODE\" AND \"DF_REPLACE_FLAG\" =0 AND  \"DF_ID\" ";
                strQry += " NOT IN (SELECT \"WO_DF_ID\" FROM  \"TBLWORKORDER\" WHERE \"WO_DF_ID\" IS NOT NULL) ";
                strQry += " AND \"DF_LOC_CODE\" LIKE '" + objSession.OfficeCode + "%' ";
                strQry += " AND \"DF_STATUS_FLAG\" =" + txtType.Text + " AND {0} like %{1}% order by DF_ID&";
                strQry += "DBColName=\"DF_ID\"~\"DT_NAME\"~\"DT_CODE\"&";
                strQry += "ColDisplayName=" + lblIDText.Text + "~DTC_NAME~DTC_CODE&";

                strQry = strQry.Replace("'", @"\'");

                cmdSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + hdfFailureId.ClientID + "&btn=" + cmdSearch.ClientID + "',520,520," + hdfFailureId.ClientID + ")");


                txtCommdate.Attributes.Add("onblur", "return ValidateDate(" + txtCommdate.ClientID + ");");
                txtDeDate.Attributes.Add("onblur", "return ValidateDate(" + txtDeDate.ClientID + ");");

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdViewEstimate_Click(object sender, EventArgs e)
        {
            try
            {
                string strParam = string.Empty;

                clsEstimation objGetFailID = new clsEstimation();
                string sEst_ID = objGetFailID.GetFailId(txtFailureId.Text);
                string sinsulationtype = objGetFailID.Getinsulationtype(txtFailureId.Text);
                DataTable dt = objGetFailID.Getoildetails(txtFailureId.Text);
                if (Convert.ToString(dt.Rows[0]["EST_OILTYPE"]) == "2")
                {
                    sinsulationtype = "0";
                    txtFailType.Text = "2";
                }

                //oilqnty = " + Convert.ToString(dt.Rows[0]["EST_OIL_QNTY"]) +
                //    " & oiltotal = " + Convert.ToString(dt.Rows[0]["EST_TOTALOILVAL"])
                //    + " & oiltype = " + Convert.ToString(dt.Rows[0]["EST_OILTYPE"])
                //    + " & oilprice = " + oilvalue + " & starrating = " +
                //    Convert.ToString(dt.Rows[0]["EST_STAR_RATING"])




                double oilvalue = Convert.ToDouble(ConfigurationSettings.AppSettings["EsterOilValue"]);
                //string sEst_ID = txtFailureId.Text; // from preview (workorder preview) 
                if ((txtType.Text == "1" || txtType.Text == "4") && txtFailType.Text != "2")
                {
                    strParam = "id=RefinedEstimation&EstimationId=" + sEst_ID;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
                if (txtType.Text == "2" || txtFailType.Text == "2")
                {
                    strParam = "id=EnhanceEstimation&EnhanceId=" + txtFailureId.Text + "&sStatus=" + txtFailureId.Text + "&FailType=" + txtFailType.Text + "&Insulationtype=" + sinsulationtype + "&oilqnty=" + Convert.ToString(dt.Rows[0]["EST_OIL_QNTY"]) + "&oiltotal=" + Convert.ToString(dt.Rows[0]["EST_TOTALOILVAL"]) + "&oiltype=" + Convert.ToString(dt.Rows[0]["EST_OILTYPE"]) + "&oilprice=" + oilvalue + "&starrating=" + Convert.ToString(dt.Rows[0]["EST_STAR_RATING"]) + "&statusFlag=" + txtType.Text;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
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
                else if (objSession.RoleId == "2")
                {
                    Level = 2;
                }
                else if (objSession.RoleId == "6")
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

        protected void cmbDtc_Scheme_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int Scapacity = cmbCapacity.SelectedIndex;
                clsWorkOrder objWO = new clsWorkOrder();
                objWO.sDtcScheme = Convert.ToInt32(cmbDtc_Scheme_Type.SelectedValue);

                objWO.GetDTCAccCode(objWO);
                txtACCcode.Text = objWO.sAccCode;
                txtAcCode.Text = objWO.sAccCode;
                txtOFAccCode.Text = objWO.sAccCode;
                txtCreditACCode.Text = "16-205";
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void ChkOFCheck_CheckedChanged(object sender, EventArgs e)
        {
            string Off_Code = string.Empty;
            try
            {
                if (ChkOFCheck.Checked == true)
                {
                    dvDecomm.Style.Add("class", "span6");
                    dvComm.Attributes.Add("class", "span6");
                    dvOilFileration.Visible = true;
                    //dvOilFileration.Attributes.Add("display", "block");

                    if (ChkCredit.Checked == true)
                    {
                        dvOilFileration.Attributes.Add("class", "span6");
                        divCreditWO.Attributes.Add("class", "span6");
                    }
                    else
                    {
                        dvOilFileration.Attributes.Add("class", "span11");
                    }

                    //dvBasic.Style.Add("display", "none");
                    //dvSection.Style.Add("display", "none");
                    //dvRepairer.Style.Add("display", "block");
                    if (objSession.OfficeCode.Length > Constants.Division)
                    {
                        Off_Code = objSession.OfficeCode.Substring(0, Constants.Division);
                    }
                    else
                        Off_Code = objSession.OfficeCode;
                    Genaral.Load_Combo("SELECT \"TR_ID\", UPPER(\"TR_NAME\") FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TRO_OFF_CODE\" =  '" + Off_Code + "' ORDER BY \"TR_NAME\" ", "--Select--", cmbRepairer);
                    lnkDTCDetails.Visible = false;
                    lnkDTrDetails.Visible = false;
                    cmbRepairer.Enabled = true;
                    cmdViewEstimate.Visible = false;
                }
                else
                {
                    dvOilFileration.Visible = false;
                    divCreditWO.Attributes.Add("class", "span11");
                    //dvComm.Attributes.Add("class", "span12");
                    //dvOilFileration.Attributes.Add("display", "none");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void ChkCredit_CheckedChanged(object sender, EventArgs e)
        {
            string Off_Code = string.Empty;
            try
            {
                if (ChkCredit.Checked == true)
                {
                    dvDecomm.Style.Add("class", "span6");
                    dvComm.Attributes.Add("class", "span6");
                    //dvOilFileration.Visible = true;
                    divCreditWO.Visible = false;
                    //dvOilFileration.Attributes.Add("display", "block");

                    if (ChkOFCheck.Checked == true)
                    {
                        dvOilFileration.Attributes.Add("class", "span6");
                        divCreditWO.Attributes.Add("class", "span6");
                    }
                    else
                    {
                        divCreditWO.Attributes.Add("class", "span6");
                    }
                    //dvBasic.Style.Add("display", "none");
                    //dvSection.Style.Add("display", "none");
                    //dvRepairer.Style.Add("display", "block");
                    if (objSession.OfficeCode.Length > Constants.Division)
                    {
                        Off_Code = objSession.OfficeCode.Substring(0, Constants.Division);
                    }
                    else
                        Off_Code = objSession.OfficeCode;
                    // Genaral.Load_Combo("SELECT \"TR_ID\", UPPER(\"TR_NAME\") FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TRO_OFF_CODE\" =  '" + Off_Code + "' ORDER BY \"TR_NAME\" ", "--Select--", cmbRepairer);
                    lnkDTCDetails.Visible = false;
                    lnkDTrDetails.Visible = false;
                    cmbRepairer.Enabled = true;
                    cmdViewEstimate.Visible = false;
                }
                else
                {
                    //dvOilFileration.Visible = false;
                    divCreditWO.Visible = false;
                    //dvComm.Attributes.Add("class", "span12");
                    //dvOilFileration.Attributes.Add("display", "none");
                }
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

    }

}