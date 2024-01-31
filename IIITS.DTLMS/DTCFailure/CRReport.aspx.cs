using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;

namespace IIITS.DTLMS.DTCFailure
{
    public partial class CRReport : System.Web.UI.Page
    {
        string strFormCode = "CRReport";
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
                    txtCRDate.Attributes.Add("readonly", "readonly");
                    CalendarExtender1.EndDate = System.DateTime.Now;
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    if (!IsPostBack)
                    {
                        if (Request.QueryString["DecommId"] != null && Convert.ToString(Request.QueryString["DecommId"]) != "")
                        {
                            txtDecommId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["DecommId"]));
                        }
                        if (Request.QueryString["TypeValue"] != null && Convert.ToString(Request.QueryString["TypeValue"]) != "")
                        {
                            txtType.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TypeValue"]));

                        }


                        GetDetailsForCR();

                        //WorkFlow / Approval
                        WorkFlowConfig();

                        ApprovalHistoryView.BOID = Convert.ToString(Session["BOID"]);
                        ApprovalHistoryView.sRecordId = txtDecommId.Text;
                        ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        if (txtActiontype.Text == "M")
                        {
                            clsApproval objLevel = new clsApproval();
                            string sLevel = objLevel.sGetApprovalLevel(Convert.ToString(ViewState["BOID"]), objSession.RoleId);

                            if (sLevel != "" && sLevel != null)
                            {
                                if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                                {
                                    cmdCR.Text = "Modify and Submit";
                                }
                                else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                                {
                                    cmdCR.Text = "Modify and Approve";
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
                                    cmdCR.Text = "Submit";
                                }
                                else if (Convert.ToInt32(sLevel.Split('~').GetValue(1).ToString()) < Convert.ToInt32(sLevel.Split('~').GetValue(0).ToString()))
                                {
                                    cmdCR.Text = "Approve";
                                }
                            }
                        
                        }
                    }
                    ApprovalHistoryView.BOID = Convert.ToString(Session["BOID"]);
                    ApprovalHistoryView.sRecordId = txtDecommId.Text;
                }
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetDetailsForCR()
        {
            try
            {
                clsCRReport objRIApproval = new clsCRReport();
                objRIApproval.sDecommId = txtDecommId.Text;

                objRIApproval.GetDetailsForCR(objRIApproval);

                txtWrkOrderDate.Text = objRIApproval.sWorkOrderDate;
                txtcomWO.Text = objRIApproval.sComWorkOrder;
                txtDEcomWO.Text = objRIApproval.sDecomWorkOrder;


                txtStoreKeepName.Text = objRIApproval.sStoreKeeperName;
                txtStoreOffName.Text = objRIApproval.sStoreOfficerName;
                txtRemarksStoreKeeper.Text = objRIApproval.sCommentByStoreKeeper.Replace("ç", ",");
                txtRemStoreOfficer.Text = objRIApproval.sCommentByStoreOfficer;
                txtOilCapacity.Text = objRIApproval.sOilQuantity;
                txtAcceptDate.Text = objRIApproval.sApprovedDate;
                txtRINo.Text = objRIApproval.sRINo;
                txtRIDate.Text = objRIApproval.sRIDate;
                txtCRDate.Text = objRIApproval.sCRDate;

                txtDTCCode.Text = objRIApproval.sDTCCode;
                txtDTCId.Text = objRIApproval.sDTCId;
                txtFailureDTr.Text = objRIApproval.sTCCode;
                txtFailDTrId.Text = objRIApproval.sFailureTCId;
                txtNewDTr.Text = objRIApproval.sNewTCCode;
                txtNewDTrId.Text = objRIApproval.sNewTCId;
                hdfFailureId.Value = objRIApproval.sFailureId;

                txtInvQty.Text = objRIApproval.sInventoryQty;
                txtDecommInventry.Text = objRIApproval.sDecommInventoryQty;

                Textackno.Text = objRIApproval.sRVNo;
                Textackdate.Text = objRIApproval.sRVDate;


                if (txtType.Text == "2")
                {
                    lblFailDTr.Text = "Enhance DTr Code";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdCR_Click(object sender, EventArgs e)
        {
            try
            {

                clsApproval objAproval = new clsApproval();

                objAproval.sPrevWFOId = txtWFOId.Text;
                objAproval.sWFObjectId = txtWFOId.Text;
                
                //objAproval.UpdateWFOAutoObject(objAproval);

                if (cmdCR.Text == "View")
                {
                    if (hdfApproveStatus.Value != "")
                    {
                        if (hdfApproveStatus.Value == "1" || hdfApproveStatus.Value == "2")
                        {
                            GenerateCRReport();
                        }
                    }
                    else
                    {
                        GenerateCRReport();
                    }
                    GenerateCRReport();
                    return;
                }

                clsCRReport objCR = new clsCRReport();
                string[] Arr = new string[2];

                if (txtCRDate.Text!="")
                {
                    string sResults = Genaral.DateComparisionTransaction(txtCRDate.Text, txtWrkOrderDate.Text, false, false);
                    if (sResults == "1")
                    {
                        ShowMsgBox("CR Date should be Greater than or Equal to RI Date");
                        txtCRDate.Focus();
                        return;
                    }
                }

                objCR.sDTCCode = txtDTCCode.Text;
                objCR.sCrby = objSession.UserId;

                objCR.sInventoryQty = txtInvQty.Text;
                objCR.sDecommId = txtDecommId.Text;
                objCR.sDecommInventoryQty = txtDecommInventry.Text;
                objCR.sFailureId = hdfFailureId.Value;
                objCR.sCRDate = txtCRDate.Text;

                //Workflow
                WorkFlowObjects(objCR);

                #region Modify and Approve

                // For Modify and Approve
                if (txtActiontype.Text == "M")
                {
                    if (hdfRejectApproveRef.Value != "RA")
                    {
                        if (txtComment.Text.Trim() == "")
                        {
                            ShowMsgBox("Enter Comments/Remarks");
                            txtComment.Focus();
                            return;

                        }
                    }

                    objCR.sActionType = txtActiontype.Text;

                    Arr = objCR.SaveCompletionReport1(objCR);
                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                    }
                    if (Arr[1].ToString() == "0")
                    {
                        hdfWFDataId.Value = objCR.sWFDataId;
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

                if (txtActiontype.Text == "RA")
                {
                    ApproveRejectAction1();
                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                    }
                    return;
                }

                if (objSession.RoleId == "4"|| objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    if(txtWFOAuto.Text!="0" && objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        if (txtComment.Text.Trim() == "")
                        {
                            ShowMsgBox("Enter Comments/Remarks");
                            txtComment.Focus();
                            return;
                        }

                        Arr = objCR.SaveCompletionReport1(objCR);
                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                        }
                        GenerateCRReport();
                        cmdCR.Enabled = false;

                        if (Arr[1].ToString() == "0")
                        {
                            ShowMsgBox(Arr[0]);
                            return;
                        }
                        if (Arr[1].ToString() == "2")
                        {
                            ShowMsgBox(Arr[0]);
                            return;
                        }
                    }
                    else
                    {

                        if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {

                        }
                        else
                        {


                            if (txtComment.Text.Trim() == "")
                            {
                                ShowMsgBox("Enter Comments/Remarks");
                                txtComment.Focus();
                                return;
                            }

                            Arr = objCR.SaveCompletionReport1(objCR);
                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                            }
                            GenerateCRReport();
                            cmdCR.Enabled = false;

                            if (Arr[1].ToString() == "0")
                            {
                                ShowMsgBox(Arr[0]);
                                return;
                            }
                            if (Arr[1].ToString() == "2")
                            {
                                ShowMsgBox(Arr[0]);
                                return;
                            }
                        }
                    }
                    //if (txtComment.Text.Trim() == "")
                    //{
                    //    ShowMsgBox("Enter Comments/Remarks");
                    //    txtComment.Focus();
                    //    return;
                    //}
                 
                    //Arr = objCR.SaveCompletionReport(objCR);
                    //if (objSession.sTransactionLog == "1")
                    //{
                    //    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                    //}
                    //GenerateCRReport();
                    //cmdCR.Enabled = false;

                    //if (Arr[1].ToString() == "0")
                    //{
                    //    ShowMsgBox(Arr[0]);
                    //    return;
                    //}
                    //if (Arr[1].ToString() == "2")
                    //{
                    //    ShowMsgBox(Arr[0]);
                    //    return;
                    //}
                    //txtWFDataId.Text = objRIApproval.sWFDataId;
                }

                if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                {
                    ApproveRejectAction1();
                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(CR Report) Failure ");
                    }
                    return;
                }

               
            }
            catch (Exception ex)
            {
                ShowMsgBox("Something Went Wrong Please Aprrove Once Again");
                // lblMessage.Text = clsException.ErrorMsg();
                
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                
            }
        }


        public void GenerateCRReport()
        {
            try
            {
                //To show Report Only for Failure Entry
                if (txtType.Text == "1" || txtType.Text == "4")
                {
                    string strParam = "id=CRReport&DecommId=" + txtDecommId.Text;
                    RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
                else if (txtType.Text == "2")
                {
                    string strParam = "id=EnhanceCRReport&DecommId=" + txtDecommId.Text;
                    RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool CheckFailureEntry()
        {
            try
            {
                clsFormValues objForm = new clsFormValues();
                objForm.sDecommisionId = txtDecommId.Text;
                string sResult = objForm.GetStatusFlagForDecommission(objForm);
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

        protected void lnkFailDTrDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtFailDTrId.Text));

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

        protected void lnkNewDTr_Click(object sender, EventArgs e)
        {
            try
            {
                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtNewDTrId.Text));

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

        #region Workflow/Approval
        public void SetControlText()
        {
            try
            {
                txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));

                if (txtActiontype.Text == "A")
                {
                    cmdCR.Text = "Approve";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdCR.Text = "Reject";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    cmdCR.Text = "Modify and Approve";
                    pnlApproval.Enabled = true;
                }

               
                 dvComments.Style.Add("display", "block");
                             
                if (txtWFOAuto.Text  != "0")
                {
                    //cmdApprove.Text = "Save";

                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    
                    pnlApproval.Enabled = true;

                    // To handle Record From Reject 
                    if (txtActiontype.Text == "A" && txtWFOAuto.Text == "0")
                    {
                        txtActiontype.Text = "M";
                        hdfRejectApproveRef.Value = "RA";
                    }
                }
                if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
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


                if (txtComment.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Comments/Remarks");
                    txtComment.Focus();
                    return;
                }             
                objApproval.sCrby = objSession.UserId;
                objApproval.sOfficeCode = objSession.OfficeCode;
                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = txtWFOId.Text;
                objApproval.sWFAutoId = txtWFOAuto.Text;

                if (txtActiontype.Text == "A")
                {
                    objApproval.sApproveStatus = "1";
                }
                if (txtActiontype.Text == "R")
                {
                    objApproval.sApproveStatus = "3";
                }
                if (txtActiontype.Text == "RA")
                {
                    objApproval.sApproveStatus = "1";
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
                objApproval.sDescription = "Completion Report For Transformer Centre Code " + txtDTCCode.Text;
                bool bResult = objApproval.ApproveWFRequest(objApproval);
                if (bResult == true)
                {

                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");

                        //if (objSession.RoleId == "1" || objSession.RoleId == "3" || objSession.RoleId == "4" || objSession.RoleId == "6")
                        //{
                            GenerateCRReport();
                        //}
                        cmdCR.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {
                        ShowMsgBox("Rejected Successfully");
                        cmdCR.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        ShowMsgBox("Modified and Approved Successfully");

                        if (objSession.RoleId == "3")
                        {
                            GenerateCRReport();
                        }
                        cmdCR.Enabled = false;
                    }
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
                objApproval.sWFObjectId = txtWFOId.Text;
                objApproval.sWFAutoId = txtWFOAuto.Text;

                if (txtActiontype.Text == "A")
                {
                    objApproval.sApproveStatus = "1";
                }
                if (txtActiontype.Text == "R")
                {
                    objApproval.sApproveStatus = "3";
                }
                if (txtActiontype.Text == "RA")
                {
                    objApproval.sApproveStatus = "1";
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
                objApproval.sDescription = "Completion Report For Transformer Centre Code " + txtDTCCode.Text;
                bool bResult = objApproval.ApproveWFRequest1(objApproval);
                if (bResult == true)
                {

                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");

                        //if (objSession.RoleId == "1" || objSession.RoleId == "3" || objSession.RoleId == "4" || objSession.RoleId == "6")
                        //{
                        GenerateCRReport();
                        //}
                        cmdCR.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {
                        ShowMsgBox("Rejected Successfully");
                        cmdCR.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        ShowMsgBox("Modified and Approved Successfully");

                        if (objSession.RoleId == "3")
                        {
                            GenerateCRReport();
                        }
                        cmdCR.Enabled = false;
                    }
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
                        txtWFOId.Text = Convert.ToString(Session["WFOId"]);
                        txtWFOAuto.Text = Convert.ToString(Session["WFOAutoId"]);
                        hdfApproveStatus.Value = Convert.ToString(Session["ApproveStatus"]);

                        //Session["WFDataId"] = null;
                        Session["WFOId"] = null;
                        Session["WFOAutoId"] = null;
                        Session["ApproveStatus"] = null;
                    }


                    if (hdfWFDataId.Value != "0")
                    {
                        GetCRDetailsFromXML(hdfWFDataId.Value);
                    }
                    SetControlText();
                    if (txtActiontype.Text == "V")
                    {
                        //cmdCR.Enabled = false;
                        cmdCR.Text = "View";
                        dvComments.Style.Add("display", "none");
                        txtInvQty.ReadOnly = true;
                        txtDecommInventry.ReadOnly = true;
                    }
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


                objRIApproval.sFormName = "CRReport";
                objRIApproval.sOfficeCode = objSession.OfficeCode;
                objRIApproval.sClientIP = sClientIP;
                objRIApproval.sWFObjectId = txtWFOId.Text;
                objRIApproval.sWFAutoId = txtWFOAuto.Text;
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
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "CRReport");
                if (sResult == "1")
                {
                    if(txtActiontype.Text != "V")
                    {
                        txtcertify.Checked = false;
                        txtcertify.Enabled = true;
                    }
                    
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
        #endregion

        #region Load From XML
        public void GetCRDetailsFromXML(string sWFDataId)
        {
            try
            {

                clsCRReport objRIApproval = new clsCRReport();
                objRIApproval.sWFDataId = sWFDataId;

                objRIApproval.GetCRDetailsFromXML(objRIApproval);

                txtInvQty.Text = objRIApproval.sInventoryQty;
                txtDecommInventry.Text = objRIApproval.sDecommInventoryQty;
                txtCRDate.Text = objRIApproval.sCRDate;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        #endregion

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
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("/Approval/ApprovalInbox.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdViewRI_Click(object sender, EventArgs e)
        {
            try
            {
                clsFormValues objApproval = new clsFormValues();
                string sTaskType = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtType.Text));    
                string sFailureId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfFailureId.Value));
                string sDecommId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtDecommId.Text));

                string url = "/DTCFailure/RIApprove.aspx?TypeValue=" + sTaskType + "&DecommId=" + sDecommId + "&FailureId=" + sFailureId;
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
                string sBOId = "26";
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