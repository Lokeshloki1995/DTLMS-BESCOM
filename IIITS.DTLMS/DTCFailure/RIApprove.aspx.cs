using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using IIITS.PGSQL.DAL;

namespace IIITS.DTLMS.DTCFailure
{
    public partial class RIApprove : System.Web.UI.Page
    {

        string strFormCode = "RIApprove";
        clsSession objSession;
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
                    Form.DefaultButton = cmdApprove.UniqueID;
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    txtAckDate.Attributes.Add("readonly", "readonly");
                    txtNoOfBarrels.Enabled = false;

                    CalendarExtender2.EndDate = System.DateTime.Now;

                    if (!IsPostBack)
                    {
                        CalendarExtender2.EndDate = System.DateTime.Now;
                        txtAckDate.Attributes.Add("onblur", "return ValidateDate(" + txtAckDate.ClientID + ");");
                        if (Request.QueryString["OfficeCode"] != null && Convert.ToString(Request.QueryString["OfficeCode"]) != "")
                        {
                            txtssOfficeCode.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["OfficeCode"]));
                        }
                        if (Request.QueryString["DecommId"] != null && Convert.ToString(Request.QueryString["DecommId"]) != "")
                        {
                            txtDecommId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["DecommId"]));
                            txtFailureId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["FailureId"]));
                            txtType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TypeValue"]));

                            txtAckDate.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
                            GetTransformerDetails();
                        }
                        //if (txtAckDate.Text != "")
                        //{
                        //    histroy.Visible = true;
                        //}
                        //else
                        //{
                        //    histroy.Visible = false;
                        //}


                        //WorkFlow / Approval
                        WorkFlowConfig();
                        if (objSession.RoleId == "2")
                        {
                            Session["BOID"] = "15";
                            ViewState["BOID"] = "15";
                        }
                        else
                        {
                            ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        }
                        // ViewState["BOID"] = Session["BOID"].ToString();
                    }
                    ApprovalHistoryView.BOID = Convert.ToString(ViewState["BOID"]);
                    ApprovalHistoryView.sRecordId = txtDecommId.Text;
                    //if (objSession.RoleId == "12")
                    //{
                    //    txtActiontype.Text = "M";
                    //}
                    if (txtActiontype.Text == "M")
                    {
                        clsApproval objLevel = new clsApproval();
                        string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                        if (sLevel != "" && sLevel != null)
                        {
                            if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdApprove.Text = " Modify and Submit";
                            }
                            else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdApprove.Text = " Modify and Approve";
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
                                cmdApprove.Text = "Submit";
                            }
                            else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdApprove.Text = "Approve";
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
        protected void cmdApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    clsRIApproval objRIApproval = new clsRIApproval();
                    if (ValidateForm() == true)
                    {
                        string[] Arr = new string[3];

                        objRIApproval.sDecommId = txtDecommId.Text;
                        objRIApproval.sRemarks = txtCommentFromStoreKeeper.Text.Replace("'", "").Replace("\"", "").Replace(";", "").Replace(",", "ç");
                        objRIApproval.sFailureId = txtFailureId.Text;
                        objRIApproval.sTasktype = txtType.Text;
                        objRIApproval.sOilQuantitySK = txtOilQtySK.Text;
                  //      objRIApproval.sBarrels = txtNoOfBarrels.Text;

                        objRIApproval.sTCCode = txtDtrCode.Text;
                        objRIApproval.sCrby = objSession.UserId;
                        objRIApproval.sRVNo = txtAckNo.Text;
                        objRIApproval.sRVDate = txtAckDate.Text;
                        objRIApproval.sOilQuantity = txtOilQuantity.Text;

                        //int oil = Convert.ToInt32(txtOilQuantity.Text);
                        //int value = 0;
                        //int brl = 210;


                        //if (oil >= 210)
                        //{
                        //    value = (oil/ brl);
                        //}
                        //else
                        //{
                        //    value = '0';
                        //}
                        objRIApproval.sBarrels = txtNoOfBarrels.Text;

                        if (objSession.sRoleType == "1")
                        {
                            objRIApproval.sOfficeCode = objSession.OfficeCode;
                        }
                        else
                        {
                            if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                            {
                                objRIApproval.sOfficeCode = txtssOfficeCode.Text;
                            }
                            else
                            {
                                objRIApproval.sOfficeCode = Convert.ToString(ViewState["LOCCODE"]);
                            }
                            //objRIApproval.sOfficeCode = Convert.ToString(ViewState["LOCCODE"]);



                        }

                        //if(objSession.RoleId=="12")
                        //{
                        //    objRIApproval.sOfficeCode = objSession.OfficeCode;
                        //}
                        objRIApproval.sDTCCode = hdfDTCCode.Value;
                        objRIApproval.sManualRVACKNo = txtManualAckNo.Text;
                        objRIApproval.sDecomWorkOrder = txtDeCommWO.Text;

                        if (cmdApprove.Text == "View")
                        {
                            if (hdfApproveStatus.Value != "")
                            {
                                if (hdfApproveStatus.Value == "1" || hdfApproveStatus.Value == "2")
                                {
                                    GenerateRIAckReport();
                                }

                                if (hdfApproveStatus.Value == "0")
                                {
                                    GenerateRIAckReport();
                                }

                            }
                            else
                            {
                                GenerateRIAckReport();
                            }
                            return;
                        }

                        //Workflow
                        WorkFlowObjects(objRIApproval);

                        #region Modify and Approve

                        // For Modify and Approve
                        if (txtActiontype.Text == "M")
                        {
                            if (hdfRejectApproveRef.Value != "RA")
                            {
                                //if (txtComment.Text.Trim() == "")
                                //{
                                //    ShowMsgBox("Enter Comments/Remarks");
                                //    txtComment.Focus();
                                //    return;

                                //}

                                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                                {
                                    if (txtComment.Text.Trim() == "")
                                    {
                                        txtComment.Text = "APPROVED.";
                                    }
                                }
                                else
                                {
                                    if (txtComment.Text.Trim() == "")
                                    {
                                        ShowMsgBox("Enter Comments/Remarks");
                                        txtComment.Focus();
                                        return;

                                    }
                                }
                            }

                            objRIApproval.sActionType = txtActiontype.Text;
                            objRIApproval.sCrby = hdfCrBy.Value;
                            Arr = objRIApproval.UpdateReplaceDetails1(objRIApproval);

                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RI) Failure ");
                            }

                            if (Arr[1].ToString() == "1")
                            {
                                hdfWFDataId.Value = objRIApproval.sWFDataId;
                                //Session["WFOId"] = objRIApproval.sWFDataId;
                                ApproveRejectAction1();

                                if (objSession.sTransactionLog == "1")
                                {
                                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RI) Failure ");
                                }

                                GenerateRIAckReport();
                                return;
                            }
                            if (Arr[1].ToString() == "2")
                            {
                                ShowMsgBox(Arr[0]);
                                return;
                            }
                        }

                        #endregion

                        if (objSession.RoleId == "2" || objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                            {
                                // objRIApproval.checkapprovallevel(objRIApproval);
                                string val;
                            val=objRIApproval.checkapprovallevel(hdfWFOId.Value);
                                if (val == "0")
                                {

                                    Arr = objRIApproval.UpdateReplaceDetails1(objRIApproval);
                                    cmdApprove.Enabled = false;

                                    if (Arr[1].ToString() == "1")
                                    {
                                        ShowMsgBox(Arr[0]);
                                        //Session["WFOId"] = Arr[2];
                                        GenerateRIAckReport();
                                        return;
                                    }
                                    if (Arr[1].ToString() == "2")
                                    {
                                        ShowMsgBox(Arr[0]);
                                        return;
                                    }
                                }
                             // return;
                                else
                                {
                                    if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                                    {
                                        if (hdfWFOAutoId.Value == "0")
                                        {
                                            ApproveRejectAction1();

                                            if (objSession.sTransactionLog == "1")
                                            {
                                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RI) Failure ");
                                            }

                                            //Session["WFOId"] = objRIApproval.sWFDataId;
                                            GenerateRIAckReport();
                                            return;
                                        }
                                    }
                                }
                            }

                            Arr = objRIApproval.UpdateReplaceDetails1(objRIApproval);
                            cmdApprove.Enabled = false;

                            if (Arr[1].ToString() == "1")
                            {
                                ShowMsgBox(Arr[0]);
                                //Session["WFOId"] = Arr[2];
                                GenerateRIAckReport();
                                return;
                            }
                            if (Arr[1].ToString() == "2")
                            {
                                ShowMsgBox(Arr[0]);
                                return;
                            }
                            //txtWFDataId.Text = objRIApproval.sWFDataId;
                        }


                        if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                        {
                            if (hdfWFOAutoId.Value == "0")
                            {
                                ApproveRejectAction1();

                                if (objSession.sTransactionLog == "1")
                                {
                                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (RI) Failure ");
                                }

                                //Session["WFOId"] = objRIApproval.sWFDataId;
                                GenerateRIAckReport();
                                return;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ShowMsgBox("Something Went Wrong Please Aprrove Once Again");
                // lblMessage.Text = clsException.ErrorMsg();
             
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




        bool ValidateForm()
        {
            bool bValidate = false;
            try
            {
                if (txtOilQtySK.Text.Trim() == "")
                {
                    txtOilQtySK.Focus();
                    ShowMsgBox("Please Enter Oil Quantity");
                    return bValidate;
                }
                if (txtCommentFromStoreKeeper.Text.Trim() == "")
                {
                    txtCommentFromStoreKeeper.Focus();
                    ShowMsgBox("Please Enter the Remarks/Comments");
                    return bValidate;
                }
                if (txtAckDate.Text!="")
                {
                    string sResult = Genaral.DateComparisionTransaction(txtAckDate.Text, hdfDecommDate.Value, false, false);
                    if (sResult == "1")
                    {
                        txtAckDate.Focus();
                        ShowMsgBox("Ack Date should be Greater than Decommission Date");
                        return bValidate;
                    }
                }
                //if (txtFailureDate.Text  != "")
                //{
                //    sResult = Genaral.DateComparision(txtAckDate.Text, txtFailureDate.Text, false, false);
                //    if (sResult == "2")
                //    {
                //        txtAckDate.Focus();
                //        ShowMsgBox("Ack Date should be Greater than Failure Date");
                //        return bValidate;
                //    }
                //}

                bValidate = true;
                return bValidate;
            }
              
            catch(Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return bValidate;
            }                            
        }
       
        protected void cmdApproveView_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                {
                    Response.Redirect("/Approval/ApprovalInbox.aspx", false);
                }
                else
                {
                    Response.Redirect("RIApprovalView.aspx", false);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void GetTransformerDetails()
        {
            try
            {
                clsRIApproval objRIApproval = new clsRIApproval();
                objRIApproval.sFailureId = txtFailureId.Text;
                objRIApproval.sDecommId = txtDecommId.Text;
                objRIApproval.GetFailureTCDetails(objRIApproval);

                txtDeCommWO.Text = objRIApproval.sDecomWorkOrder;
                txtDtrCode.Text = objRIApproval.sTCCode;
                txtMake.Text = objRIApproval.sTcMake;
                txtDTrSlno.Text = objRIApproval.sTcSlno;
                txtDTrId.Text = objRIApproval.sTCId;
                txtFailureDate.Text = objRIApproval.sFailureDate;

               // txtDeCommWO.Text = objRIApproval.sDecomWorkOrder;
                txtWoCreditNo.Text = objRIApproval.sCreditWorkOrder;

                hdfDTCCode.Value = objRIApproval.sDTCCode;

                objRIApproval.sDecommId = txtDecommId.Text;

                objRIApproval.GetRIDetails(objRIApproval);

                 txtOilQuantity.Text = objRIApproval.sOilQuantity;
                txtNoOfBarrels.Text = objRIApproval.sBarrels;

                if (objRIApproval.sOilQuantity != "")
                {

                    int oil = Convert.ToInt32(objRIApproval.sOilQuantity);
                    int value = 0;
                    int brl = 210;


                    if (oil >= 210)
                    {
                        value = (oil / brl);
                        if (oil % 210 != 0)
                        {
                            value = value + 1;
                        }

                    }
                    else
                    {
                        value = 1;
                    }
                    txtNoOfBarrels.Text = value.ToString();
                }


                txtCommentFromStoreKeeper.Text = objRIApproval.sRemarks;
                hdfDecommDate.Value = objRIApproval.sDecommDate;

                if (objRIApproval.sRVNo != "")
                {
                    txtAckDate.Text = objRIApproval.sRVDate;
                    txtAckNo.Text = objRIApproval.sRVNo;
                    txtOilQtySK.Text = objRIApproval.sOilQuantitySK;
                    txtManualAckNo.Text = objRIApproval.sManualRVACKNo;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void WorkFlowObjects(clsRIApproval objRIApproval)
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


                objRIApproval.sFormName = "RIApprove";
                if (objSession.sRoleType == "1")
                {
                    objRIApproval.sOfficeCode = objSession.OfficeCode;
                }
                else
                {
                    objRIApproval.sOfficeCode = Convert.ToString(ViewState["LOCCODE"]);
                }
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                   // objRIApproval.sOfficeCode = objSession.OfficeCode;
                    objRIApproval.sOfficeCode = txtssOfficeCode.Text;
                }
                objRIApproval.sClientIP = sClientIP;
                objRIApproval.sWFObjectId = hdfWFOId.Value;
                objRIApproval.sWFAutoId = hdfWFOAutoId.Value;
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
                txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                //if (objSession.RoleId == "12")
                //{
                //    txtActiontype.Text = "M";
                //}
                if (txtActiontype.Text == "A")
                {
                    cmdApprove.Text = "Approve";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdApprove.Text = "Reject";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    //cmdApprove.Text = "Modify and Approve";                    
                    pnlApproval.Enabled = true;
                }

                if (objSession.RoleId == "5")
                {
                    dvComments.Style.Add("display", "block");
                }
                //cmdReset.Enabled = false;
              
                if (hdfWFOAutoId.Value  != "0")
                {                    

                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    cmdApprove.Text = "Save";
                    pnlApproval.Enabled = true;


                    // To handle Record From Reject 
                    if (txtActiontype.Text == "A" && hdfWFOAutoId.Value == "0")
                    {
                        txtActiontype.Text = "M";
                        hdfRejectApproveRef.Value = "RA";
                    }
                }
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    pnlApproval.Enabled = true;
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

                if (objSession.RoleId != "2")
                {
                    //if (txtComment.Text.Trim() == "")
                    //{
                    //    ShowMsgBox("Enter Comments/Remarks");
                    //    txtComment.Focus();
                    //    return;

                    //}

                    if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        if (txtComment.Text.Trim() == "")
                        {
                            txtComment.Text = "APPROVED.";
                        }
                    }
                    else
                    {


                        if (txtComment.Text.Trim() == "")
                        {
                            ShowMsgBox("Enter Comments/Remarks");
                            txtComment.Focus();
                            return;

                        }
                    }


                }

             
                objApproval.sCrby = objSession.UserId;
                if (objSession.sRoleType == "1")
                {
                    objApproval.sOfficeCode = objSession.OfficeCode;
                }
                else
                {
                    objApproval.sOfficeCode = Convert.ToString(ViewState["LOCCODE"]);
                }

                if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                    objApproval.sOfficeCode = txtssOfficeCode.Text;
                }
                PGSqlConnection objCon = new PGSQL.DAL.PGSqlConnection(Constants.Password);
                string storeid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + txtAckNo.Text.Substring(0, 3) + "'");

                if (storeid != objSession.OfficeCode)
                {
                    objApproval.sStatus = "1";
                }

                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;
                objApproval.sWFAutoId = hdfWFOAutoId.Value;

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
                objApproval.sWFDataId = hdfWFDataId.Value;
                objApproval.sNewRecordId = txtDecommId.Text;

                bool bResult = false;
                if (txtActiontype.Text == "M")
                {
                    objApproval.sWFDataId = hdfWFDataId.Value;
                    if (hdfRejectApproveRef.Value == "RA")
                    {
                        objApproval.sApproveStatus = "1";
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

                        if (objSession.RoleId == "2")
                        {
                            clsRIApproval objRI = new clsRIApproval();
                            //objRI.SendSMStoSectionOfficer(txtDtrCode.Text, txtDecommId.Text,txtFailureId.Text);

                        }
                        GenerateRIAckReport();

                        cmdApprove.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {

                        ShowMsgBox("Rejected Successfully");
                        cmdApprove.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        ShowMsgBox("Modified and Approved Successfully");

                        if (objSession.RoleId == "2")
                        {
                            clsRIApproval objRI = new clsRIApproval();
                            //objRI.SendSMStoSectionOfficer(txtDtrCode.Text, txtDecommId.Text, txtFailureId.Text);

                        }
                        GenerateRIAckReport();
                        cmdApprove.Enabled = false;
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

                if (objSession.RoleId != "2")
                {

                    if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        if (txtComment.Text.Trim() == "")
                        {
                            txtComment.Text = "APPROVED.";
                        }
                    }
                    else
                    {


                        if (txtComment.Text.Trim() == "")
                        {
                            ShowMsgBox("Enter Comments/Remarks");
                            txtComment.Focus();
                            return;

                        }
                    }


                }


                objApproval.sCrby = objSession.UserId;
                if (objSession.sRoleType == "1")
                {
                    objApproval.sOfficeCode = objSession.OfficeCode;
                }
                else
                {
                    objApproval.sOfficeCode = Convert.ToString(ViewState["LOCCODE"]);
                }

                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    objApproval.sOfficeCode = txtssOfficeCode.Text;
                }
                PGSqlConnection objCon = new PGSQL.DAL.PGSqlConnection(Constants.Password);
                string storeid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + txtAckNo.Text.Substring(0, 3) + "'");

                if (storeid != objSession.OfficeCode)
                {
                    objApproval.sStatus = "1";
                }

                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;
                objApproval.sWFAutoId = hdfWFOAutoId.Value;

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
                objApproval.sWFDataId = hdfWFDataId.Value;
                objApproval.sNewRecordId = txtDecommId.Text;

                bool bResult = false;
                if (txtActiontype.Text == "M")
                {
                    objApproval.sWFDataId = hdfWFDataId.Value;
                    if (hdfRejectApproveRef.Value == "RA")
                    {
                        objApproval.sApproveStatus = "1";
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

                        if (objSession.RoleId == "2")
                        {
                            clsRIApproval objRI = new clsRIApproval();
                            //objRI.SendSMStoSectionOfficer(txtDtrCode.Text, txtDecommId.Text,txtFailureId.Text);

                        }
                        GenerateRIAckReport();

                        cmdApprove.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {

                        ShowMsgBox("Rejected Successfully");
                        cmdApprove.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        ShowMsgBox("Modified and Approved Successfully");

                        if (objSession.RoleId == "2")
                        {
                            clsRIApproval objRI = new clsRIApproval();
                            //objRI.SendSMStoSectionOfficer(txtDtrCode.Text, txtDecommId.Text, txtFailureId.Text);

                        }
                        GenerateRIAckReport();
                        cmdApprove.Enabled = false;
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
            string WOID = string.Empty;
            string type = string.Empty; 
            try
            {
                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                {


                    if (Session["WFOId"] != null && Convert.ToString(Session["WFOId"]) != "")
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
                        GetRIDetailsFromXML(hdfWFDataId.Value);
                    }
                    SetControlText();
                    if (txtActiontype.Text == "V")
                    {
                        cmdApprove.Text = "View";
                        dvComments.Style.Add("display", "none");
                    }
                    if (objSession.sRoleType == "2")
                    {
                        //if(objSession.RoleId == "5")
                        //{
                        //    //WOID = hdfWFOAutoId.Value;
                        //    WOID = hdfWFOId.Value;
                        //    type = "2";
                        //}
                        //else
                        //{
                        //    WOID = hdfWFOId.Value;
                        //    type = "1";
                        //}
                        WOID = hdfWFOId.Value;
                        string sLocCode = clsStoreOffice.GetRICurrentOfficeCode(WOID, type);
                        if (sLocCode.Length > 3)
                        {
                            sLocCode = sLocCode.Substring(0, Constants.Division);
                        }
                        ViewState["LOCCODE"] = sLocCode;
                    }
                    GenerateAckNo();
                }
                
                else
                {
                    cmdApprove.Text = "View";
                }

                DisableControlForView();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool CheckFormCreatorLevel()
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "RIApprove");
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
                if (cmdApprove.Text.Contains("View"))
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
                    Response.Redirect("RIApprovalView.aspx", false);
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
                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtDTrId.Text));

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

        public void GenerateAckNo()
        {
            try
            {
                clsRIApproval objRI = new clsRIApproval();
                if (objSession.sRoleType == "1")
                {
                    txtAckNo.Text = objRI.GenerateAckNo(objSession.OfficeCode);
                }
                else
                {
                    if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        txtAckNo.Text = objRI.GenerateAckNo(txtssOfficeCode.Text);
                    }
                    else
                    {
                        txtAckNo.Text = objRI.GenerateAckNo(Convert.ToString(ViewState["LOCCODE"]));
                    }
                   // txtAckNo.Text = objRI.GenerateAckNo(Convert.ToString(ViewState["LOCCODE"])); 
                }
                
                txtAckNo.ReadOnly = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        public void GenerateRIAckReport()
        {
            try
            {
                if (txtDecommId.Text != "")
                {
                    string strParam = string.Empty;
                    strParam = "id=RIAckReport&DecommId=" + txtDecommId.Text;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
                else
                {
                    string sWFO_ID = hdfWFDataId.Value;
                    string strParam = string.Empty;
                    strParam = "id=RIReportso&wfoID=" + sWFO_ID + "&sDtrcode=" + txtDtrCode.Text + "&sFailurId=" + txtFailureId.Text;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        #region Load From XML
        public void GetRIDetailsFromXML(string sWFDataId)
        {
            try
            {

                clsRIApproval objRIApproval = new clsRIApproval();
                objRIApproval.sWFDataId = sWFDataId;

                objRIApproval.GetRIDetailsFromXML(objRIApproval);

                txtOilQuantity.Text = objRIApproval.sOilQuantity;
                txtCommentFromStoreKeeper.Text = objRIApproval.sRemarks;
                txtManualAckNo.Text = objRIApproval.sManualRVACKNo;

                txtAckDate.Text = objRIApproval.sRVDate;
                //txtAckNo.Text = objRIApproval.sRVNo;
                txtOilQtySK.Text = objRIApproval.sOilQuantitySK;
                hdfCrBy.Value = objRIApproval.sCrby;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        #endregion

        protected void cmdViewDecomm_Click(object sender, EventArgs e)
        {
            try
            {
                clsFormValues objApproval = new clsFormValues();
                string sTaskType = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtType.Text));              
                string sFailureId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtFailureId.Text));
                string sDecommId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtDecommId.Text));

                string url = "/DTCFailure/DeCommissioning.aspx?TypeValue=" + sTaskType + "&ReferID=" + sFailureId + "&ReplaceId=" + sDecommId;
                string s = "window.open('" + url + "', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkHistory_Click(object sender, EventArgs e)
        {
            try
            {
                string sfailureId = string.Empty;
                string sRecordId = string.Empty;
                sfailureId = txtDecommId.Text;
                string sBOId = "15";
                string sFormName = "ApprovalHistory.aspx";

                sRecordId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sfailureId));
                sBOId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sBOId));

                Response.Redirect("/Approval/" + sFormName + "?RecordId=" + sRecordId + "&BOId=" + sBOId, false);

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        
    }
}