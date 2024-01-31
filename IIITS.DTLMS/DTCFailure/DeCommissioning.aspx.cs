﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IIITS.DTLMS.BL;
using System.Globalization;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;

namespace IIITS.DTLMS.DTCFailure
{
    public partial class DeCommissioning : System.Web.UI.Page
    {

        string strFormCode = "DeCommissioning";
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
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    txtDecommDate.Attributes.Add("readonly", "readonly");
                    txtRIDate.Attributes.Add("readonly", "readonly");
                    TxtCommDate.Attributes.Add("readonly", "readonly");

                    CalendarExtender1.EndDate = System.DateTime.Now;
                    CalendarExtender2.EndDate = System.DateTime.Now;
                    CalendarExtender3.EndDate = System.DateTime.Now;

                    if (!IsPostBack)
                    {
                        //if (txtOilQuantity.Text != "")
                        //{

                        //int oil = Convert.ToInt32(txtOilQuantity.Text);
                        //int value = 0;
                        //int brl = 210;


                        //if (oil >= 210)
                        //{
                        //    value = (oil / brl);
                        //    if (oil % 210 != 0)
                        //    {
                        //        value = value + 1;
                        //    }

                        //}
                        //else
                        //{
                        //    value = 1;
                        //}
                        //txtbarrel.Text = value.ToString();
                        //}




                        if (Request.QueryString["TypeValue"] != null && Convert.ToString(Request.QueryString["TypeValue"]) != "")
                        {
                            txtType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TypeValue"]));
                            // txtssOfficeCode.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ssOfficeCode"]));
                            ChangeLabelText();
                            GenerateRINo();
                        }



                        if (Request.QueryString["ReferID"] != null && Convert.ToString(Request.QueryString["ReferID"]) != "")
                        {
                            txtFailureId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReferID"]));
                            if (Request.QueryString["OfficeCode"] != null && Convert.ToString(Request.QueryString["OfficeCode"]) != "")
                            {
                                txtssOfficeCode.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["OfficeCode"]));

                            }
                            if (Request.QueryString["ReplaceId"] != null && Convert.ToString(Request.QueryString["ReplaceId"]) != "")
                            {
                                txtReplaceId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ReplaceId"]));
                            }

                            hdfFailureId.Value = txtFailureId.Text;

                            GetBasicDetails();

                            if (txtReplaceId.Text != "0" && txtReplaceId.Text != "")
                            {
                                if (!txtReplaceId.Text.Contains("-"))
                                {
                                    GetReplaceDetails();
                                }

                            }
                            else
                            {
                                txtReplaceId.Text = "";
                            }
                        }

                        CalendarExtender2.EndDate = System.DateTime.Now;
                        if (objSession.sRoleType == "2")
                        {
                            if (objSession.OfficeCode == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWSTORE"]))
                            {
                                Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\" ='A' AND \"SM_ID\"=" + objSession.OfficeCode + "", "-Select-", cmbStore);
                            }
                            else
                            {
                                Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\" ='A' AND \"SM_ID\"='" + objSession.OfficeCode + "'", "-Select-", cmbStore);
                            }
                        }
                        else
                        {
                            if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                            {
                                string officecode = txtssOfficeCode.Text;

                                if (officecode.Substring(0, 3) == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWDIV"]))
                                {
                                    //Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + " or \"SM_ID\"='" + Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWSTORE"]) + "'", "-Select-", cmbStore);
                                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + " ", "-Select-", cmbStore);
                                }
                                else
                                {
                                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(officecode) + "", "-Select-", cmbStore);
                                }
                            }
                            else
                            {
                                if (objSession.RoleId != "8")
                                {
                                    if (objSession.OfficeCode.Substring(0, 3) == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWDIV"]))
                                    {
                                        //coded by Ramya for new requirement on 19-12-2022
                                        //Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + " or \"SM_ID\"='" + Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWSTORE"]) + "'", "-Select-", cmbStore);
                                        Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + " ", cmbStore);

                                    }
                                    else
                                    {
                                        //Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + "", "-Select-", cmbStore);
                                        Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' and \"SM_ID\"=" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + "", cmbStore);


                                    }
                                }
                                else
                                {
                                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\"='A' ", cmbStore);
                                }
                            }

                        }



                        //Search Window Call
                        LoadSearchWindow();

                        //WorkFlow / Approval
                        WorkFlowConfig();
                        if (objSession.RoleId == "4")
                        {
                            Session["BOID"] = "14";
                            ViewState["BOID"] = "14";
                        }
                        else
                        {
                            ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        }

                        //   ViewState["BOID"] = Session["BOID"].ToString();

                        //OleDbDataReader dr;
                        //dr = objCon.Fetch("select IN_INV_NO from TBLDTCINVOICE,TBLTCREPLACE WHERE IN_INV_NO=TR_IN_NO AND IN_INV_NO='" + txtInvoiceInInvNo.Text + "'");
                        //dt.Load(dr);
                        //if (dt.Rows.Count > 0)
                        //{
                        //    cmdSave.Enabled = false;
                        //    cmdReset.Enabled = false;

                        //}

                    }

                    ApprovalHistoryView.BOID = Convert.ToString(ViewState["BOID"]);
                    ApprovalHistoryView.sRecordId = txtReplaceId.Text;

                    if (txtActiontype.Text == "M")
                    {
                        clsApproval objLevel = new clsApproval();
                        string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);
                        if (sLevel != "" && sLevel != null)
                        {
                            if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdSave.Text = "Modify and Submit";
                            }
                            else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                            {
                                cmdSave.Text = "Modify and Approve";
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

        protected void OilQuantity_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtOilQuantity.Text != "")
                {

                    int oil = Convert.ToInt32(txtOilQuantity.Text);
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
                    txtbarrel.Text = value.ToString();
                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetBasicDetails()
        {
            try
            {
                clsFailureEntry objFailure = new clsFailureEntry();
                clsDeCommissioning objdecomm = new clsDeCommissioning();

                // coded by rudra on 23-04-2020 for change decommission details 
                clsRIApproval objRIApproval = new clsRIApproval();
                objRIApproval.sFailureId = txtFailureId.Text;
                // objRIApproval.sDecommId = txtDecommId.Text;
                objRIApproval.GetFailureTCDetails(objRIApproval);

                txtDeCommWO.Text = objRIApproval.sDecomWorkOrder;
                txtWoCreditNo.Text = objRIApproval.sCreditWorkOrder;

                txtFailureId.Text = hdfFailureId.Value;
                objFailure.sFailureId = txtFailureId.Text;

                objFailure.GetFailureDetails(objFailure);

                txtFailureDate.Text = objFailure.sFailureDate;
                txtDTCName.Text = objFailure.sDtcName;
                txtDTCCode.Text = objFailure.sDtcCode;
                txtMake.Text = objFailure.sDtcTcMake;
                txtTCCode.Text = objFailure.sDtcTcCode;
                txtCapcity.Text = objFailure.sDtcCapacity;
                TxtCommDate.Text = objFailure.sDTrCommissionDate;

                hdfDTCId.Value = objFailure.sDtcId;
                hdfTCId.Value = objFailure.sTCId;
                objdecomm.sdtrcode = txtTCCode.Text;
                //To get Invoice ID
                GetInvoiceNo();
                txtOilQuantity.Text = objFailure.sOilQuantity;

                if (txtOilQuantity.Text != "")
                {

                    int oil = Convert.ToInt32(txtOilQuantity.Text);
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
                    txtbarrel.Text = value.ToString();
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetReplaceDetails()
        {
            try
            {
                clsDeCommissioning objReplace = new clsDeCommissioning();
                objReplace.sDecommId = txtReplaceId.Text;

                objReplace.GetDecommDetails(objReplace);

                txtRINo.Text = objReplace.sRINo;
                txtRIDate.Text = objReplace.sRIDate;
                txtRemarks.Text = objReplace.sRemarks;
                cmbStore.SelectedValue = objReplace.sStoreId;
                hdfcmdstore.Value = objReplace.sStoreId;
                txtOilQuantity.Text = objReplace.sOilQuantity;
                txtTrReading.Text = objReplace.sTRReading;
                txtDecommDate.Text = objReplace.sDecommDate;
                txtManualRINo.Text = objReplace.sManualRINo;
                if (txtOilQuantity.Text != "")
                {

                    int oil = Convert.ToInt32(txtOilQuantity.Text);
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
                    txtbarrel.Text = value.ToString();
                }

                cmdReset.Enabled = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, 
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    //Check AccessRights
                    bool bAccResult = true;
                    if (cmdSave.Text == "Update")
                    {
                        bAccResult = CheckAccessRights("3");
                    }
                    else if (cmdSave.Text == "Save")
                    {
                        bAccResult = CheckAccessRights("2");
                    }

                    if (bAccResult == false)
                    {
                        return;
                    }
                    if (cmdSave.Text == "View")
                    {
                        if (hdfApproveStatus.Value != "")
                        {
                            if (hdfApproveStatus.Value == "1" || hdfApproveStatus.Value == "2")
                            {
                                GenerateDecommReport();
                            }
                            else
                            {
                                GenerateDecommReport();
                            }
                        }
                        else
                        {
                            GenerateDecommReport();
                        }
                        return;
                    }

                    if (ValidateForm() == true)
                    {
                        clsDeCommissioning objReplace = new clsDeCommissioning();
                        string[] Arr = new string[3];
                        objReplace.sInvoiceId = objReplace.GetRecordIdForinvoiceno();
                        objReplace.sFailureId = txtFailureId.Text;
                        objReplace.sDecommId = txtReplaceId.Text;
                        objReplace.sTRReading = txtTrReading.Text.Replace("'", "");
                        objReplace.sRINo = txtRINo.Text.Replace("'", "");
                        objReplace.sRIDate = txtRIDate.Text.Replace("'", "");
                        objReplace.sRemarks = txtRemarks.Text.Replace("'", "").Replace("\"", "").Replace(";", "").Replace(",", "ç");
                        objReplace.sStoreId = cmbStore.SelectedValue;
                        objReplace.sCrby = objSession.UserId;
                        objReplace.sTaskType = txtType.Text;
                        objReplace.sbarrels = txtbarrel.Text;
                        
                        if (objSession.RoleId != Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            objReplace.sOfficeCode = objSession.OfficeCode;
                        }
                        else
                        {
                            objReplace.sOfficeCode = txtssOfficeCode.Text;
                        }
                        objReplace.sDTCCode = txtDTCCode.Text;
                        objReplace.sOilQuantity = txtOilQuantity.Text;
                        objReplace.sDecommDate = txtDecommDate.Text;
                        objReplace.sManualRINo = txtManualRINo.Text;
                        objReplace.sCommDate = TxtCommDate.Text;
                        objReplace.sdtrcode = txtTCCode.Text;
                        objReplace.sOiltype = cmbOilType.SelectedValue;
                        objReplace.sbarrels = txtbarrel.Text;
                        
                        if (txtbarrel.Text == "")
                        {
                            if (txtOilQuantity.Text != "")
                            {
                                int oil = Convert.ToInt32(txtOilQuantity.Text);
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
                                txtbarrel.Text = value.ToString();
                            }
                            objReplace.sbarrels = txtbarrel.Text;
                        }

                        if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                        {
                            if (hdfWFDataId.Value != "0")
                            {
                                ApproveRejectAction1();
                                if (objSession.sTransactionLog == "1")
                                {
                                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(Decommissioning) Failure ");
                                }
                                GenerateDecommReport();
                                return;
                            }
                        }

                        //Workflow
                        WorkFlowObjects(objReplace);

                        #region Modify and Approve

                        // For Modify and Approve
                        if (txtActiontype.Text == "M")
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
                            objReplace.sDecommId = "";
                            objReplace.sActionType = txtActiontype.Text;
                            objReplace.sCrby = hdfCrBy.Value;
                            if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                            {
                                objReplace.sOfficeCode = txtssOfficeCode.Text;
                                objReplace.sCrby = objSession.UserId;

                            }
                            else
                            {
                                objReplace.sOfficeCode = hdfOfficeCode.Value;
                            }

                            Arr = objReplace.SaveReplaceDetails1(objReplace);

                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(Decommissioning) Failure ");
                            }

                            if (Arr[1].ToString() == "0")
                            {
                                hdfWFDataId.Value = objReplace.sWFDataId;
                                ApproveRejectAction1();
                                GenerateDecommReport();
                                return;
                            }
                            if (Arr[1].ToString() == "2")
                            {
                                ShowMsgBox(Arr[0]);
                                return;
                            }
                        }

                        #endregion

                        Arr = objReplace.SaveReplaceDetails1(objReplace);

                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(Decommissioning) Failure ");
                        }

                        if (Arr[1].ToString() == "0")
                        {
                            cmdSave.Text = "Update";
                            ShowMsgBox("Decommissioning Done Successfully");
                            hdfWFDataId.Value = Arr[2];
                            cmdSave.Enabled = false;
                            GenerateDecommReport();
                            return;
                        }
                        if (Arr[1].ToString() == "1")
                        {

                            ShowMsgBox(Arr[0].ToString());
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
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool ValidateForm()
        {
            bool bValidate = false;
            try
            {
                if (txtRINo.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid RI Number");
                    return bValidate;
                }
                if (txtOilQuantity.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Oil Quantity");
                    return bValidate;
                }
                if (txtRIDate.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid RI Date");
                    return bValidate;
                }
                string sResult = Genaral.DateComparisionTransaction(txtRIDate.Text, txtFailureDate.Text, false, false);
                if (sResult == "1")
                {
                    ShowMsgBox("RI Date should be Greater than Failure Date");
                    return bValidate;
                }
                sResult = Genaral.DateComparisionTransaction(txtDecommDate.Text, txtFailureDate.Text, false, false);
                if (sResult == "1")
                {
                    ShowMsgBox("Decommissioning Date should be Greater than Failure Date");
                    return bValidate;
                }
                //if (txtRvNo.Text.Trim() == "")
                //{
                //    ShowMsgBox("Enter Valid RV Number");
                //    return bValidate;
                //}
                //if (txtRvDate.Text.Trim() == "")
                //{
                //    ShowMsgBox("Enter Valid RV Date");
                //    return bValidate;
                //}
                //if (txtTrReading.Text.Trim() == "")
                //{
                //    ShowMsgBox("Enter Valid TR Reading");
                //    return bValidate;
                //}
                if (cmbStore.SelectedIndex == 0 && cmbStore.SelectedItem.Text.Trim() == "-Select-")
                {
                    ShowMsgBox("Please Select Store");
                    cmbStore.Focus();
                    return bValidate;
                }
                if (txtRemarks.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Remarks");
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
                //txtRINo.Text = string.Empty;
                txtRIDate.Text = string.Empty;
                txtTrReading.Text = string.Empty;
                txtRemarks.Text = string.Empty;
                txtDecommDate.Text = string.Empty;
                txtManualRINo.Text = string.Empty;
                txtOilQuantity.Text = string.Empty;

                hdfFailureId.Value = string.Empty;

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
                    Response.Redirect("DeCommissioningView.aspx", false);
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
                GetBasicDetails();

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ChangeLabelText()
        {
            try
            {
                if (txtType.Text == "1" || txtType.Text == "4")
                {
                    lblIDText.Text = "Failure ID";
                    lblDateText.Text = "Failure Date";
                }
                else if (txtType.Text == "2")
                {
                    lblIDText.Text = "Enhancement ID";
                    lblDateText.Text = "Enhancement Date";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetInvoiceNo()
        {
            try
            {
                clsDeCommissioning objDecomm = new clsDeCommissioning();
                txtInvoiceId.Text = objDecomm.GetInvoiceNo(txtFailureId.Text);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetStoreId()
        {
            try
            {
                clsTcMaster objTcMaster = new clsTcMaster();
                cmbStore.SelectedValue = objTcMaster.GetStoreId(objSession.OfficeCode);
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

                objApproval.sFormName = "DeCommissioning";
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
                        ShowMsgBox("Sorry , You are not authorized to Access");
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


        public void WorkFlowObjects(clsDeCommissioning objDecomm)
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


                objDecomm.sFormName = "DeCommissioning";
                if (objSession.RoleId != Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    objDecomm.sOfficeCode = objSession.OfficeCode;
                }
                else
                {
                    objDecomm.sOfficeCode = txtssOfficeCode.Text;
                }
                objDecomm.sClientIP = sClientIP;
                objDecomm.sWFOId = hdfWFOId.Value;
                objDecomm.sWFAutoId = hdfWFOAutoId.Value;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GenerateRINo()
        {
            try
            {
                clsDeCommissioning objDecomm = new clsDeCommissioning();
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    string officecode = txtssOfficeCode.Text;
                    txtRINo.Text = objDecomm.GenerateRINo(officecode);
                }
                else
                {
                    txtRINo.Text = objDecomm.GenerateRINo(objSession.OfficeCode);
                }
                //   txtRINo.Text = objDecomm.GenerateRINo(objSession.OfficeCode);
                txtRINo.ReadOnly = true;
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

                //if(objSession.RoleId=="12")
                //{
                //    txtActiontype.Text = "M";
                //}

                if (txtActiontype.Text == "A")
                {
                    cmdSave.Text = "Approve";
                    pnlApproval.Enabled = false;
                    cmdReset.Enabled = true;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdSave.Text = "Reject";
                    pnlApproval.Enabled = false;
                    cmdReset.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    cmdSave.Text = "Modify and Approve";
                    pnlApproval.Enabled = true;
                    cmdReset.Enabled = true;
                }

                dvComments.Style.Add("display", "block");
                //cmdReset.Enabled = false;

                if (hdfWFOAutoId.Value != "0")
                {
                    cmdSave.Text = "Save";
                    dvComments.Style.Add("display", "none");
                }

                if (hdfWFOAutoId.Value == "0")
                {
                    cmdReset.Enabled = false;
                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    cmdSave.Text = "Save";
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

                objApproval.sCrby = objSession.UserId;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    objApproval.sOfficeCode = txtssOfficeCode.Text;
                }
                else
                {
                    objApproval.sOfficeCode = objSession.OfficeCode;
                }
                //   objApproval.sOfficeCode = objSession.OfficeCode;
                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;
                objApproval.sWFAutoId = hdfWFOAutoId.Value;
                objApproval.sfailid = txtFailureId.Text;

                PGSqlConnection objCon = new PGSQL.DAL.PGSqlConnection(Constants.Password);
                string presentstoreid;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    presentstoreid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + objApproval.sOfficeCode.Substring(0, 3) + "'");
                }
                else
                {
                    presentstoreid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + objSession.OfficeCode.Substring(0, 3) + "'");

                }
                // string presentstoreid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + objSession.OfficeCode.Substring(0, 3) + "'");

                if (presentstoreid != cmbStore.SelectedValue)
                {
                    objApproval.sStatus = "1";
                }
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

                bool bResult = false;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    txtActiontype.Text = "A";
                }

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
                    clsDeCommissioning objDecomm = new clsDeCommissioning();


                    if (objApproval.sNewRecordId != "" && objApproval.sNewRecordId != null)
                    {
                        txtReplaceId.Text = objApproval.sNewRecordId;
                    }
                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");
                        cmdSave.Enabled = false;

                        if (txtType.Text != "3")
                        {
                            if (objSession.RoleId == "1")
                            {
                                GenerateDecommReport();
                            }
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

                        if (txtType.Text != "3")
                        {
                            if (objSession.RoleId == "1")
                            {
                                GenerateDecommReport();
                            }
                        }
                    }

                    if (objApproval.sApproveStatus == "1" || objApproval.sApproveStatus == "2")
                    {
                        if (objSession.RoleId == "1")
                        {
                            objDecomm.updateinvnotbltcreplace(txtReplaceId.Text);
                        }

                    }
                    if (objSession.RoleId == "1")
                    {
                        objDecomm.updateRecord(txtDTCCode.Text);
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

                objApproval.sCrby = objSession.UserId;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    objApproval.sOfficeCode = txtssOfficeCode.Text;
                }
                else
                {
                    objApproval.sOfficeCode = objSession.OfficeCode;
                }
                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;
                objApproval.sWFAutoId = hdfWFOAutoId.Value;
                objApproval.sfailid = txtFailureId.Text;

                PGSqlConnection objCon = new PGSQL.DAL.PGSqlConnection(Constants.Password);
                string presentstoreid;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    presentstoreid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + objApproval.sOfficeCode.Substring(0, 3) + "'");
                }
                else
                {
                    presentstoreid = objCon.get_value("SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\"='" + objSession.OfficeCode.Substring(0, 3) + "'");

                }

                if (presentstoreid != cmbStore.SelectedValue)
                {
                    objApproval.sStatus = "1";
                }
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

                bool bResult = false;
                if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    txtActiontype.Text = "A";
                }

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
                    clsDeCommissioning objDecomm = new clsDeCommissioning();


                    if (objApproval.sNewRecordId != "" && objApproval.sNewRecordId != null)
                    {
                        txtReplaceId.Text = objApproval.sNewRecordId;
                    }
                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");
                        cmdSave.Enabled = false;

                        if (txtType.Text != "3")
                        {
                            if (objSession.RoleId == "1")
                            {
                                GenerateDecommReport();
                            }
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

                        if (txtType.Text != "3")
                        {
                            if (objSession.RoleId == "1")
                            {
                                GenerateDecommReport();
                            }
                        }
                    }

                    if (objApproval.sApproveStatus == "1" || objApproval.sApproveStatus == "2")
                    {
                        if (objSession.RoleId == "1")
                        {
                            objDecomm.updateinvnotbltcreplace(txtReplaceId.Text);
                        }

                    }
                    if (objSession.RoleId == "1")
                    {
                        objDecomm.updateRecord(txtDTCCode.Text);
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
                        GetDecommDetailsFromXML(hdfWFDataId.Value);
                    }
                    //if (objSession.RoleId == "12")
                    //{
                    //    txtActiontype.Text = "M";
                    //}
                    SetControlText();
                    if (txtActiontype.Text == "V")
                    {
                        cmdSave.Text = "View";
                        dvComments.Style.Add("display", "none");
                        cmdReset.Enabled = false;
                    }
                }
                else
                {
                    cmdSave.Text = "View";
                    if (hdfcmdstore.Value != "")
                    {
                        Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\" ='A'", "-Select-", cmbStore);
                        cmbStore.SelectedValue = hdfcmdstore.Value;
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

        public bool CheckFormCreatorLevel()
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "DeCommissioning");
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

        public void GenerateDecommReport()
        {
            try
            {
                //if (txtReplaceId.Text.Contains("-"))
                //{
                //    return;
                //}
                if (txtReplaceId.Text == "" || txtReplaceId.Text.Contains("-"))
                {
                    //string sWFO_ID = Session["WFOId"].ToString(); 
                    string sWFO_ID = hdfWFDataId.Value;
                    string strParam = string.Empty;
                    strParam = "id=RIReportso&wfoID=" + sWFO_ID + "&sDtrcode=" + txtTCCode.Text + "&sFailurId=" + txtFailureId.Text;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
                else
                {
                    string strParam = string.Empty;
                    strParam = "id=RIReport&DecommId=" + txtReplaceId.Text;
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
        public void GetDecommDetailsFromXML(string sWFDataId)
        {
            try
            {
                // If the Data saved in Main Table then this function shd not execute, so done restriction like below
                // And commented for temprary purpose.. nee to change in future

                //if (!txtReplaceId.Text.Contains("-"))
                //{
                //    return;
                //}

                clsDeCommissioning objReplace = new clsDeCommissioning();
                objReplace.sWFDataId = sWFDataId;

                objReplace.GetDecommDetailsFromXML(objReplace);

                //txtRINo.Text = objReplace.sRINo;
                txtRIDate.Text = objReplace.sRIDate;
                txtRemarks.Text = objReplace.sRemarks;

                cmbStore.SelectedValue = objReplace.sStoreId;
                txtOilQuantity.Text = objReplace.sOilQuantity;
                txtTrReading.Text = objReplace.sTRReading;
                txtDecommDate.Text = objReplace.sDecommDate;

                hdfOfficeCode.Value = objReplace.sOfficeCode;
                hdfCrBy.Value = objReplace.sCrby;
                txtManualRINo.Text = objReplace.sManualRINo;
                TxtCommDate.Text = objReplace.sCommDate;
                if (objReplace.sbarrels != null && objReplace.sbarrels != "")
                {
                    txtbarrel.Text = objReplace.sbarrels;
                }
                //txtbarrel.Text = objReplace.sbarrels;

                cmbOilType.SelectedValue = objReplace.sOiltype;
                //cmdSave.Text = "Update";

                cmdReset.Enabled = false;
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
                string sTypeName = string.Empty;
                if (txtType.Text == "1" || txtType.Text == "4")
                {
                    sTypeName = "FAILURE";
                }
                else if (txtType.Text == "2")
                {
                    sTypeName = "ENHANCEMENT";
                }

                string strQry = string.Empty;

                strQry = "Title=Search and Select Transformer Centre Failure Details&";
                strQry += "Query=SELECT \"DF_ID\",\"DT_NAME\",\"DT_CODE\" from \"TBLDTCMAST\",\"TBLDTCFAILURE\",\"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"DF_DTC_CODE\"= \"DT_CODE\" AND \"DF_REPLACE_FLAG\" =0 AND ";
                strQry += " \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND  \"IN_NO\" NOT IN (SELECT \"TR_IN_NO\" FROM  \"TBLTCREPLACE\")";
                strQry += " AND \"DF_STATUS_FLAG\" =" + txtType.Text + " ";
                strQry += " AND \"DF_LOC_CODE\" LIKE '" + objSession.OfficeCode + "%' AND {0} like %{1}% order by DF_ID&";
                strQry += "DBColName=\"DF_ID\"~\"DT_NAME\"~\"DT_CODE\"&";
                strQry += "ColDisplayName=" + sTypeName + " ID~DTC_NAME~DTC_CODE&";

                strQry = strQry.Replace("'", @"\'");

                cmdSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + hdfFailureId.ClientID + "&btn=" + cmdSearch.ClientID + "',520,520," + hdfFailureId.ClientID + ")");


                txtRIDate.Attributes.Add("onblur", "return ValidateDate(" + txtRIDate.ClientID + ");");


                // GetStoreId();
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
                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfTCId.Value));

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
                string sDTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfDTCId.Value));

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

        protected void cmdViewInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                clsFormValues objApproval = new clsFormValues();
                string sTaskType = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtType.Text));

                string sIndentId = objApproval.GetIndentId(txtInvoiceId.Text);
                sIndentId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sIndentId));
                string sInvoiceId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtInvoiceId.Text));

                string url = "/DTCFailure/InvoiceCreation.aspx?TypeValue=" + sTaskType + "&ReferID=" + sIndentId + "&InvoiceId=" + sInvoiceId;
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
                sfailureId = txtReplaceId.Text;
                string sBOId = "14";
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