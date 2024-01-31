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
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text.RegularExpressions;

namespace IIITS.DTLMS.DTCFailure
{
    public partial class FailureEntry : System.Web.UI.Page
    {

        string strFormCode = "FailureEntry";
        string flag;
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
                    Form.DefaultButton = cmdSave.UniqueID;
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;

                    txtDTrCommDate.Attributes.Add("readonly", "readonly");
                    txtFailedDate.Attributes.Add("readonly", "readonly");
                    txtdocketdate.Attributes.Add("readonly", "readonly");
                    txtConnectionDate.Attributes.Add("readonly", "readonly");
                    //txtdocket.Attributes.Add("readonly", "readonly");
                    //txtdocket1.Attributes.Add("readonly", "readonly");


                    CalendarExtender1.EndDate = System.DateTime.Now;
                    CalendarExtender3.EndDate = System.DateTime.Now;
                    // CalendarExtender3.StartDate = System.DateTime.Now;

                    if (!IsPostBack)
                    {
                        //CalendarExtender2.StartDate = System.DateTime.Now.AddDays(-(System.DateTime.Now.Day - 1));
                        CalendarExtender2.EndDate = System.DateTime.Now;
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'FT' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbFailureType);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'ARM' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbReplaceEntry);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'C' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbEnhanceCapacity);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'POU' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbPurpose);
                        Genaral.Load_Combo("SELECT \"TR_ID\", UPPER(\"TR_NAME\") FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\"=\"TRO_TR_ID\" AND CAST(\"TRO_OFF_CODE\" AS TEXT) = SUBSTR(CAST('" + objSession.OfficeCode + "' AS TEXT),1,'" + Constants.Division + "') ORDER BY \"TR_NAME\" ", "--Select--", cmbRepairer);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'SG' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbSilica);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'OT' and \"MD_ID\"='1' ORDER BY \"MD_ORDER_BY\" ", cmbOilType);
                        Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'DTCT' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbDTCType);
                        if (Request.QueryString["DTCId"] != null && Convert.ToString(Request.QueryString["DTCId"]) != "")
                        {

                            txtDtcId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["DTCId"]));
                            txtFailurId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["FailureId"]));


                            if (!txtFailurId.Text.Contains("-"))
                            {
                                GetFailureDetails();
                                if (cmbGuarenteeType.SelectedIndex == 0)
                                {
                                    cmbGuarenteeType.Enabled = true;
                                }
                                else
                                    cmbGuarenteeType.Enabled = false;
                                //ValidateFormUpdate();
                            }

                            if (txtFailedDate.Text.Trim() != "")
                            {
                                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                                {
                                    txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                                    if (txtActiontype.Text == "V")
                                    {
                                        if (txtCapacity.Text != cmbEnhanceCapacity.SelectedItem.Text)
                                        {
                                            rdbFail.Checked = false;
                                            rdbFail.Enabled = false;
                                            rdbFailEnhance.Checked = true;
                                            cmbEnhanceCapacity.Enabled = false;
                                            cmbEnhanceCapacity.Visible = true;
                                            lblEnCap.Visible = true;
                                        }
                                        else
                                        {
                                            rdbFail.Checked = true;
                                            rdbFailEnhance.Enabled = false;
                                            rdbFailEnhance.Checked = false;
                                            cmbEnhanceCapacity.Visible = false;
                                            lblEnCap.Visible = false;
                                        }
                                    }
                                }
                                cmdSave.Text = "View";
                                //rdbFailEnhance.Enabled = false;

                            }
                        }
                        //Call Search Window
                        LoadSearchWindow();

                        //WorkFlow / Approval
                        WorkFlowConfig();
                        //ViewState["BOID"] = Session["BOID"].ToString();
                        if (objSession.RoleId == "4")
                        {
                            Session["BOID"] = "9";
                            ViewState["BOID"] = "9";
                        }
                        else
                        {
                            ViewState["BOID"] = Convert.ToString(Session["BOID"]);
                        }
                    }

                    ApprovalHistoryView.BOID = Convert.ToString(ViewState["BOID"]);
                    ApprovalHistoryView.sRecordId = txtFailurId.Text;

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


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        protected void cmdSave_Click(object sender, EventArgs e)
        {
            //string sFolderPath = Convert.ToString(ConfigurationSettings.AppSettings["LOGERROR"]) + DateTime.Now.ToString("yyyyMM");
            //if (!Directory.Exists(sFolderPath))
            //{
            //    Directory.CreateDirectory(sFolderPath);
            //}
            //string sPath = sFolderPath + "//" + "Main" + DateTime.Now.ToString("yyyyMMdd") + "-ErrorLog.txt";
            try
            {
                // File.AppendAllText(sPath, Environment.NewLine + "Time --" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " ,1st Click On Save Method " + Environment.NewLine);
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }

                if (cmdSave.Text == "Save")
                {
                    cmdSave.Enabled = false;
                }
                cmdReset.Enabled = false;
                if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                {
                    flag = "1";
                }
                else
                {
                    flag = "4";
                }
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
                    if (hdfApproveStatus.Value != "")
                    {
                        if (hdfApproveStatus.Value == "1" || hdfApproveStatus.Value == "2")
                        {
                            EstimationReport(flag);
                        }
                    }
                    else
                    {
                        EstimationReport(flag);
                    }
                    return;
                }
                if (ValidateForm() == true)
                {
                    clsFailureEntry objFailure = new clsFailureEntry();
                    string[] Arr = new string[2];
                    objFailure.sFailureId = txtFailurId.Text;
                    if (cmbReplaceEntry.SelectedIndex > 0)
                    {
                        objFailure.sAlternateReplaceType = cmbReplaceEntry.SelectedValue;
                    }
                    else
                    {
                        objFailure.sAlternateReplaceType = "0";
                    }
                    if (cmbSilica.SelectedIndex > 0)
                    {
                        objFailure.sSilicaCondition = cmbSilica.SelectedValue;
                    }
                    else
                    {
                        objFailure.sSilicaCondition = "0";
                    }
                    objFailure.sRepairer = cmbRepairer.SelectedValue;
                    objFailure.sDtcId = txtDtcId.Text;
                    objFailure.sDtcTcCode = txtTcCode.Text;
                    objFailure.sDtcCode = txtDTCCode.Text.Replace("'", "");
                    objFailure.sFailureDate = txtFailedDate.Text.Replace("'", "");
                    objFailure.sOilType = cmbOilType.SelectedValue;
                    objFailure.sDTCType = cmbDTCType.SelectedValue;
                    objFailure.sModem = cmbModem.SelectedValue;
                    if (txtdocket1.Text == "")
                    {
                        objFailure.sdocketno = null;
                    }
                    else
                    {
                        string temp = /*txtdocket.Text +*/ txtdocket1.Text;
                        objFailure.sdocketno = temp.ToUpper();
                    }
                    if (txtdocketdate.Text == "")
                    {
                        objFailure.sdocketDate = null;
                    }
                    else
                    {
                        objFailure.sdocketDate = txtdocketdate.Text.Replace("'", "");
                    }
                    if (cmbOilLevel.SelectedIndex > 0)
                    {
                        objFailure.sOilLevel = cmbOilLevel.SelectedValue;
                    }
                    else
                    {
                        objFailure.sOilLevel = "0";
                    }
                    if (txtCustName.Text == "")
                    {
                        objFailure.sCustName = null;
                    }
                    else
                    {
                        objFailure.sCustName = txtCustName.Text;
                    }
                    if (txtCustNo.Text == "")
                    {
                        objFailure.sCustNo = null;
                    }
                    else
                    {
                        objFailure.sCustNo = txtCustNo.Text.Trim();
                    }
                    if (txtMeggerValue.Text == "" || txtMeggerValue.Text == null)
                    {
                        objFailure.sMeggerValue = "0";
                    }
                    else
                    {
                        objFailure.sMeggerValue = txtMeggerValue.Text.Trim().Replace("'", "").Replace("\"", "").Replace(";", "").Replace(",", "ç");
                    }
                    objFailure.sFailureReasure = txtReason.Text.Trim().Replace("'", "").Replace("\"", "").Replace(";", "").Replace(",", "ç");
                    if (objFailure.sDtcReadings == null)
                    {
                        objFailure.sDtcReadings = "0";
                    }
                    else
                    {
                        objFailure.sDtcReadings = txtDTCRead.Text.Trim().Replace("'", "");
                    }
                    objFailure.sCrby = objSession.UserId;
                    objFailure.sEnhancedCapacity = cmbEnhanceCapacity.SelectedItem.Text;
                    objFailure.sDTrCommissionDate = txtDTrCommDate.Text;
                    string myString = txtDTrCommDate.Text;
                    DateTime myDateTime = DateTime.ParseExact(myString.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    string myString_new = Convert.ToDateTime(myDateTime).ToString("yyyy-MM-dd");
                    objFailure.sDtrSaveCommissionDate = myString_new;
                    if (objSession.RoleId == "4")
                    {
                        if (txtDTrCommDate.Enabled == true)
                        {
                            objFailure.UpdateDtrCommDate(objFailure);
                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "Update DTR Commission Date in Failure ");
                            }
                        }
                        if (cmbGuarenteeType.Enabled == true)
                        {
                            objFailure.sGuarantySource = "From DropDown(User Selected)";
                        }
                        else
                        {
                            objFailure.sGuarantySource = "From Query(Automatic)";
                        }
                    }
                    else
                    {
                        if (hdfGuarenteeSource.Value == "" || hdfGuarenteeSource.Value == null)
                        {

                        }
                        else
                        {
                            objFailure.sGuarantySource = hdfGuarenteeSource.Value;
                        }
                    }
                    objFailure.sGuarantyType = cmbGuarenteeType.SelectedItem.Text;
                    objFailure.sFailureType = cmbFailureType.SelectedValue.Trim();
                    objFailure.sOilQuantity = txtQuantityOfOil.Text;
                    if (cmbPurpose.SelectedIndex > 0)
                    {
                        objFailure.sPurpose = cmbPurpose.SelectedValue.Trim();
                    }
                    else
                    {
                        objFailure.sPurpose = "0";
                    }
                    if (cmbHtBusing.SelectedIndex > 0)
                    {
                        objFailure.sHTBusing = cmbHtBusing.SelectedValue.Trim();
                    }
                    if (cmbLtBusing.SelectedIndex > 0)
                    {
                        objFailure.sLTBusing = cmbLtBusing.SelectedValue.Trim();
                    }
                    if (cmbHtBusingRod.SelectedIndex > 0)
                    {
                        objFailure.sHTBusingRod = cmbHtBusingRod.SelectedValue.Trim();
                    }
                    if (cmbLtBusingRod.SelectedIndex > 0)
                    {
                        objFailure.sLTBusingRod = cmbLtBusingRod.SelectedValue.Trim();
                    }
                    if (cmbOilLevel.SelectedIndex > 0)
                    {
                        objFailure.sOilLevel = cmbOilLevel.SelectedValue.Trim();
                    }
                    if (cmbTankCondition.SelectedIndex > 0)
                    {
                        objFailure.sTankCondition = cmbTankCondition.SelectedValue.Trim();
                    }
                    if (cmbExplosion.SelectedIndex > 0)
                    {
                        objFailure.sExplosionValve = cmbExplosion.SelectedValue.Trim();
                    }
                    if (cmbDrainValve.SelectedIndex > 0)
                    {
                        objFailure.sDrainValve = cmbDrainValve.SelectedValue.Trim();
                    }
                    if (cmbBreather.SelectedIndex > 0)
                    {
                        objFailure.sBreather = cmbBreather.SelectedValue.Trim();
                    }
                    if (cmbSilica.SelectedIndex > 0)
                    {
                        objFailure.sSilicaCondition = cmbSilica.SelectedValue;
                    }
                    if (txtConnectionDate.Enabled == true)
                    {
                        objFailure.sCommissionDate = txtConnectionDate.Text;
                        bool res = objFailure.saveDtcCommissionDate(objFailure);
                        if (objSession.sTransactionLog == "1")
                        {
                            if (res == true)
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(DTC Commission date update syuccess) Failure ");
                            }
                            else
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "(DTC Commission date update Failure) Failure ");
                            }
                        }
                    }
                    if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                    {
                        ApproveRejectAction1();
                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Failure ");
                        }
                        EstimationReport(flag);
                        return;
                    }
                    if (txtActiontype.Text == "A" || txtActiontype.Text == "M")
                    {
                        if (cmbEnhanceCapacity.SelectedItem.Text == txtCapacity.Text)
                        {
                            objFailure.sFailtype = "1";
                        }
                        else
                        {
                            objFailure.sFailtype = "4";
                        }
                    }
                    else
                    {
                        if (cmbEnhanceCapacity.Enabled == false)
                        {
                            objFailure.sFailtype = "1";
                        }
                        else
                        {
                            objFailure.sFailtype = "4";
                        }
                    }                   
                    WorkFlowObjects(objFailure);      //Workflow
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
                        objFailure.sFailureId = "";
                        objFailure.sActionType = txtActiontype.Text;
                        objFailure.sOfficeCode = txtFailureOfficCode.Text;
                        objFailure.sCrby = hdfCrBy.Value;
                        Arr = objFailure.SaveFailureDetails(objFailure);
                        if (Arr[1].ToString() == "0")
                        {
                            hdfWFDataId.Value = objFailure.sWFDataId;
                            ApproveRejectAction1();
                            Session["WFOId"] = objFailure.sWFDataId;
                            EstimationReport(flag);
                            return;
                        }
                        if (Arr[1].ToString() == "2")
                        {
                            ShowMsgBox(Arr[0]);
                            return;
                        }
                    }

                    #endregion
                    Arr = objFailure.SaveFailureDetails(objFailure);
                    txtFailureOfficCode.Text = objSession.OfficeCode;
                    string sOffCode = txtFailureOfficCode.Text;
                    string sDtcCode = txtDTCCode.Text;
                    string sWoID = objFailure.getWoIDforEstimation(sOffCode, sDtcCode);
                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Failure ");
                    }
                    if (Arr[1].ToString() == "0")
                    {
                        txtFailurId.Text = objFailure.sFailureId;
                        cmdSave.Text = "Update";
                        bool res = GetFailDetails(txtFailurId.Text);
                        txtDTCCode.Enabled = false;
                        Session["WFOId"] = objFailure.sWFDataId;
                        EstimationReport(flag);
                        ShowMsgBox(Arr[0].ToString());
                        cmdSave.Enabled = false;                        
                        return;
                    }
                    if (Arr[1].ToString() == "1")
                    {
                        ShowMsgBox(Arr[0]);                     
                    }
                    if (Arr[1].ToString() == "2")
                    {
                        ShowMsgBox(Arr[0]);
                        return;
                    }
                }
                else
                {
                    cmdSave.Enabled = true;
                }
                cmdReset.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowMsgBox("Something went wrong while saving, Please Declare Failure Again.");
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }



        private void ShowMsgBox(string sMsg)
        {
            try
            {
                string sShowMsg = string.Empty;
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                //this.Page.RegisterStartupScript("Msg", sShowMsg);
                ClientScript.RegisterStartupScript(this.GetType(), "Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        public void GetFailureDetails()
        {
            try
            {
                clsFailureEntry objFailure = new clsFailureEntry();

                objFailure.sFailureId = txtFailurId.Text;
                objFailure.sDtcId = txtDtcId.Text;

                objFailure.GetFailureDetails(objFailure);

                if (objFailure.sCommissionDate == "" || objFailure.sCommissionDate == null)
                {
                    txtConnectionDate.Enabled = true;
                    txtConnectionDate.ReadOnly = false;
                }
                else
                {
                    txtConnectionDate.Enabled = false;
                    txtConnectionDate.ReadOnly = true;
                }

                if (objFailure.sDTrCommissionDate == objFailure.sDTrEnumerationDate)
                {
                    txtDTrCommDate.Enabled = true;
                    txtDTrCommDate.ReadOnly = false;
                }
                else
                {
                    txtDTrCommDate.Enabled = false;
                    txtDTrCommDate.ReadOnly = true;
                }

                txtDtcId.Text = objFailure.sDtcId;
                txtDTCCode.Text = objFailure.sDtcCode;
                txtDTCName.Text = objFailure.sDtcName;
                //txtServiceDate.Text = objFailure.sDtcServicedatc    ;
                //txtLoadKW.Text = objFailure.sDtcReadings;
                txtLoadKW.Text = objFailure.sDtcLoadKw;
                txtLoadHP.Text = objFailure.sDtcLoadHp;
                txtConnectionDate.Text = objFailure.sCommissionDate;
                txtCapacity.Text = objFailure.sDtcCapacity;
                txtLocation.Text = objFailure.sDtcLocation;

                txtConditionOfTC.Text = objFailure.sConditionoftc;


                txtTcCode.Text = objFailure.sDtcTcCode;
                txtTCSlno.Text = objFailure.sDtcTcSlno;
                txtTCId.Text = objFailure.sTCId;
                cmbRepairer.SelectedValue = objFailure.sRepairer;
                cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;
                txtdocket1.Text = objFailure.sdocketno;
                txtdocketdate.Text = objFailure.sdocketDate;
                txtCustName.Text = objFailure.sCustName;
                txtCustNo.Text = objFailure.sCustNo;
                txtMeggerValue.Text = objFailure.sMeggerValue;
                cmbOilType.SelectedValue = objFailure.sOilType;
                cmbDTCType.SelectedValue = objFailure.sDTCType;
                cmbModem.SelectedValue = objFailure.sModem;
                txtrate.Text = objFailure.sRating;
                cmbPurpose.SelectedValue = objFailure.sPurpose;
                cmbReplaceEntry.SelectedValue = objFailure.sAlternateReplaceType;
                if (objFailure.sPurpose != null && objFailure.sPurpose != "0")
                {
                    cmbPurpose.Enabled = false;
                }
                if (rdbFail.Checked)
                {
                    if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                    {
                        txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                        if (txtActiontype.Text == "V")
                        {
                            if (txtCapacity.Text != cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                rdbFailEnhance.Checked = true;
                                rdbFail.Checked = false;
                            }
                            if (cmbEnhanceCapacity.SelectedItem.Text == "--Select--" || cmbEnhanceCapacity.SelectedItem.Text == "")
                            {
                                rdbFailEnhance.Checked = false;
                                rdbFail.Checked = true;
                                cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                                lblEnCap.Visible = false;
                                cmbEnhanceCapacity.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        if (txtFailurId.Text == "0")
                        {

                        }
                        else
                        {
                            if (cmbEnhanceCapacity.SelectedItem.Text == txtCapacity.Text)
                            {
                                cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                            }
                            else
                            {
                                if (cmbEnhanceCapacity.SelectedItem.Text == "--Select--" || cmbEnhanceCapacity.SelectedItem.Text == "")
                                {
                                    cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                                    rdbFailEnhance.Checked = false;
                                    rdbFail.Checked = true;
                                    rdbFailEnhance.Enabled = false;
                                    cmbEnhanceCapacity.Visible = false;
                                    lblEnCap.Visible = false;
                                }
                                else
                                {
                                    cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;
                                    rdbFailEnhance.Checked = true;
                                    rdbFail.Checked = false;
                                    cmbEnhanceCapacity.Visible = true;
                                    cmbEnhanceCapacity.Enabled = false;
                                    rdbFail.Enabled = false;
                                    lblEnCap.Visible = true;
                                }
                            }
                        }
                    }
                }
                if (rdbFailEnhance.Checked)
                {
                    cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;
                }

                txtLastRepairDate.Text = objFailure.sLastRepairedDate;
                txtLastRepairer.Text = objFailure.sLastRepairedBy;
                cmbGuarenteeType.SelectedValue = objFailure.sGuarantyType;

                txtManfDate.Text = objFailure.sManfDate;
                txtDTrCommDate.Text = objFailure.sDTrCommissionDate;

                txtTCMake.Text = objFailure.sDtcTcMake;
                txtFailedDate.Text = objFailure.sFailureDate;
                txtReason.Text = objFailure.sFailureReasure;
                txtDTCRead.Text = objFailure.sDtcReadings;
                txtDTCCode.Enabled = false;
                txtFailureOfficCode.Text = objFailure.sOfficeCode;


                cmbFailureType.SelectedValue = objFailure.sFailureType;
                if (objFailure.sHTBusing != "")
                {
                    cmbHtBusing.SelectedValue = objFailure.sHTBusing;
                }
                if (objFailure.sLTBusing != "")
                {
                    cmbLtBusing.SelectedValue = objFailure.sLTBusing;
                }
                if (objFailure.sHTBusingRod != "")
                {
                    cmbHtBusingRod.SelectedValue = objFailure.sHTBusingRod;
                }
                if (objFailure.sLTBusingRod != "")
                {
                    cmbLtBusingRod.SelectedValue = objFailure.sLTBusingRod;
                }
                if (objFailure.sDrainValve != "")
                {
                    cmbDrainValve.SelectedValue = objFailure.sDrainValve;
                }
                if (objFailure.sOilLevel != "")
                {
                    cmbOilLevel.SelectedValue = objFailure.sOilLevel;
                }
                if (objFailure.sOilQuantity != "")
                {
                    txtQuantityOfOil.Text = objFailure.sOilQuantity;

                }
                if (objFailure.sTankCondition != "")
                {
                    cmbTankCondition.SelectedValue = objFailure.sTankCondition;
                }

                if (objFailure.sExplosionValve != "")
                {
                    cmbExplosion.SelectedValue = objFailure.sExplosionValve;
                }
                if (objFailure.sBreather != "")
                {
                    cmbBreather.SelectedValue = objFailure.sBreather;
                }
                if (objFailure.sSilicaCondition != "")
                {
                    cmbSilica.SelectedValue = objFailure.sSilicaCondition;
                }

                if (objFailure.sFailureId != "0")
                {

                    //cmdSave.Text = "Update";
                    cmdSearch.Visible = false;
                    cmdReset.Enabled = false;
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

                objFailure.sDtcCode = txtDTCCode.Text;

                objFailure.SearchFailureDetails(objFailure);

                if (objFailure.sDtcName == null)
                {

                }
                else
                {
                    txtDTCName.Text = objFailure.sDtcName;

                    if (objFailure.sDTrCommissionDate == objFailure.sDTrEnumerationDate)
                    {
                        txtDTrCommDate.Enabled = true;
                        txtDTrCommDate.ReadOnly = false;
                    }
                    else
                    {
                        txtDTrCommDate.Enabled = false;
                        txtDTrCommDate.ReadOnly = true;
                    }

                    txtDtcId.Text = objFailure.sDtcId;
                    txtDTCCode.Text = objFailure.sDtcCode;
                    txtDTCName.Text = objFailure.sDtcName;
                    txtLoadKW.Text = objFailure.sDtcLoadKw;
                    txtLoadHP.Text = objFailure.sDtcLoadHp;
                    txtConnectionDate.Text = objFailure.sCommissionDate;
                    txtCapacity.Text = objFailure.sDtcCapacity;
                    txtLocation.Text = objFailure.sDtcLocation;
                    txtTcCode.Text = objFailure.sDtcTcCode;
                    txtTCSlno.Text = objFailure.sDtcTcSlno;
                    txtTCId.Text = objFailure.sTCId;
                    cmbRepairer.SelectedValue = objFailure.sRepairer;
                    if (objFailure.sGuarantyType == "")
                    {
                        objFailure.sGuarantyType = "0";
                    }
                    cmbGuarenteeType.SelectedValue = objFailure.sGuarantyType;

                    if (cmbGuarenteeType.SelectedValue == "0")
                    {
                        if (cmbGuarenteeType.SelectedIndex == 0)
                        {
                            cmbGuarenteeType.Enabled = true;
                        }
                        else
                            cmbGuarenteeType.Enabled = false;
                    }

                    cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;

                    if (rdbFail.Checked)
                    {
                        if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                        {
                            txtActiontype.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["ActionType"]));
                            if (txtActiontype.Text == "V")
                            {
                                if (txtCapacity.Text != cmbEnhanceCapacity.SelectedItem.Text)
                                {
                                    rdbFailEnhance.Checked = true;
                                    rdbFail.Checked = false;
                                }
                                if (cmbEnhanceCapacity.SelectedItem.Text == "--Select--" || cmbEnhanceCapacity.SelectedItem.Text == "")
                                {
                                    rdbFailEnhance.Checked = false;
                                    rdbFail.Checked = true;
                                    cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                                    lblEnCap.Visible = false;
                                    cmbEnhanceCapacity.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            if (txtFailurId.Text == "0")
                            {

                            }
                            else
                            {
                                if (cmbEnhanceCapacity.SelectedItem.Text == txtCapacity.Text)
                                {
                                    cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                                }
                                else
                                {
                                    if (cmbEnhanceCapacity.SelectedItem.Text == "--Select--" || cmbEnhanceCapacity.SelectedItem.Text == "")
                                    {
                                        cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
                                        rdbFailEnhance.Checked = false;
                                        rdbFail.Checked = true;
                                        rdbFailEnhance.Enabled = false;
                                        cmbEnhanceCapacity.Visible = false;
                                        lblEnCap.Visible = false;
                                    }
                                    else
                                    {
                                        cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;
                                        rdbFailEnhance.Checked = true;
                                        rdbFail.Checked = false;
                                        cmbEnhanceCapacity.Visible = true;
                                        cmbEnhanceCapacity.Enabled = false;
                                        rdbFail.Enabled = false;
                                        lblEnCap.Visible = true;
                                    }
                                }
                            }
                        }
                    }
                    if (txtDTCName.Text.Trim() == "")
                    {
                        EmptyDTCDetails();
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
                //txtDTCCode.Text = string.Empty;
                //txtDTCName.Text = string.Empty;
                //txtServiceDate.Text = string.Empty;
                //txtLoadKW.Text = string.Empty;
                //txtLoadHP.Text = string.Empty;
                //txtConnectionDate.Text = string.Empty;
                //txtCapacity.Text = string.Empty;
                //txtLocation.Text = string.Empty;
                //txtTCSlno.Text = string.Empty;
                //txtTcCode.Text = string.Empty;
                //txtTCMake.Text = string.Empty;
                txtFailedDate.Text = string.Empty;
                txtReason.Text = string.Empty;
                txtDTCRead.Text = string.Empty;
                //txtDtcId.Text = string.Empty;
                //txtFailurId.Text = string.Empty;
                //cmdSave.Text = "Save";
                txtDTCCode.Enabled = true;
                //hdfDTCcode.Value = string.Empty;
                cmdSearch.Visible = true;
                //txtManfDate.Text = string.Empty;
                //txtDTrCommDate.Text = string.Empty;

                cmbFailureType.SelectedIndex = 0;
                cmbHtBusing.SelectedIndex = 0;
                cmbLtBusing.SelectedIndex = 0;
                cmbDrainValve.SelectedIndex = 0;
                cmbHtBusingRod.SelectedIndex = 0;
                cmbLtBusingRod.SelectedIndex = 0;
                cmbOilLevel.SelectedIndex = 0;
                txtQuantityOfOil.Text = string.Empty;
                cmbTankCondition.SelectedIndex = 0;
                cmbExplosion.SelectedIndex = 0;
                cmbBreather.SelectedIndex = 0;
                cmbSilica.SelectedIndex = 0;
                txtDTCRead.Text = string.Empty;
                cmbRepairer.SelectedIndex = 0;
                cmbPurpose.SelectedIndex = 0;
                txtMeggerValue.Text = string.Empty;
                txtdocket1.Text = string.Empty;
                txtdocketdate.Text = string.Empty;
                txtCustName.Text = string.Empty;
                txtCustNo.Text = string.Empty;
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
                    Response.Redirect("FailureEntryView.aspx", false);
                }

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
                clsFailureEntry objFailure = new clsFailureEntry();
                if (objFailure.ValidateUpdate(txtFailurId.Text) == true)
                {
                    cmdReset.Enabled = false;
                    //cmdSave.Enabled = false;
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

        /// <summary>
        /// Validate Form Details.
        /// </summary>
        /// <returns></returns>
        public bool ValidateForm()
        {
            bool bValidate = false;
            string sResultPO = string.Empty;
            clsFailureEntry Obj = new clsFailureEntry();
            try
            {
                // Regex pattern to match only alphanumeric characters and numbers of length 10 or 15
                string pattern = @"^[a-zA-Z0-9]{10}$|^[a-zA-Z0-9]{15}$";

                if (txtdocket1.Text.Trim() != "")
                {
                    if (txtdocket1.Text.Length < 10)
                    {
                        //ShowMsgBox("Please Enter valid 10 digit PGRS number");
                        ShowMsgBox("Please Enter valid PGRS number");
                        txtdocket1.Focus();
                        cmdSave.Enabled = true;
                        return bValidate;
                    }
                    else
                    {
                        bool PGRSCkeck = Regex.IsMatch(txtdocket1.Text, pattern);
                        if (PGRSCkeck == false)
                        {
                            ShowMsgBox("Please Enter valid PGRS number");
                            txtdocket1.Focus();
                            cmdSave.Enabled = true;
                            return bValidate;
                        }
                        else
                        {
                            // Regex pattern to match only alphabetic characters
                            string AlphabeticPattern = @"^[a-zA-Z]+$";
                            if (txtdocket1.Text.Length == 10 || txtdocket1.Text.Length == 15)
                            {
                                bool OnlyAlphabeticcharacters = Regex.IsMatch(txtdocket1.Text, AlphabeticPattern);
                                if (OnlyAlphabeticcharacters == true)
                                {
                                    ShowMsgBox("Please Enter valid PGRS number");
                                    return bValidate;
                                }
                                else
                                {
                                  bool CheckResult =  Convert.ToBoolean(Obj.CheckPGRSNumber_Exist(txtdocket1.Text));
                                    if (CheckResult)
                                    {
                                        ShowMsgBox("Entered PGRS number already exists.");
                                        return bValidate;
                                    }
                                }
                            }
                        }
                    }
                }

                if (txtdocketdate.Text == "" || txtdocketdate.Text == " " || txtdocketdate.Text == null)
                {
                    txtdocketdate.Focus();
                    ShowMsgBox("Please Enter PGRS Docket Date");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (cmbBreather.SelectedValue == "1" && cmbSilica.SelectedIndex == 0)
                {
                    cmbSilica.Focus();
                    ShowMsgBox("Select Silica Condition");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (txtCustName.Text.Trim() == "")
                {
                    txtCustName.Focus();
                    ShowMsgBox("Please Enter Customer Name");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtCustName.Text.Trim().StartsWith("."))
                {
                    txtCustName.Focus();
                    ShowMsgBox("Customer Name not Start with DOT(.)");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtCustNo.Text.Trim() == "")
                {
                    txtCustNo.Focus();
                    ShowMsgBox("Please Enter Customer NO");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtCustNo.Text.Trim() != "")
                {
                    string num = txtCustNo.Text.Replace(" ", "");
                    string CustNoregex = @"^\d+$";
                    Match match = Regex.Match(num, CustNoregex);
                    if (num.Length != 10 || (!match.Success))
                    {
                        txtCustNo.Focus();
                        ShowMsgBox("Please Enter 10 Digit Valid Customer NO");
                        cmdSave.Enabled = true;
                        return bValidate;
                    }
                }

                if (cmbReplaceEntry.SelectedItem.Text == "--Select--")
                {
                    cmbReplaceEntry.Focus();
                    ShowMsgBox(" Please Select Alternate Replacement");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                else
                {
                    if (txtMeggerValue.Text.Contains("."))
                    {
                        if (txtMeggerValue.Text.Length == 1)
                        {
                            ShowMsgBox("Please Enter Proper Value");
                            cmdSave.Enabled = true;
                            return bValidate;
                        }
                    }
                }
                if (txtQuantityOfOil.Text.Trim() == "")
                {
                    txtQuantityOfOil.Focus();
                    ShowMsgBox("Please Enter Oil Quantity");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (cmbOilType.SelectedItem.Text == "--Select--")
                {
                    cmbOilType.Focus();
                    ShowMsgBox(" Please Select Oil Type");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (cmbDTCType.SelectedItem.Text == "--Select--")
                {
                    cmbDTCType.Focus();
                    ShowMsgBox(" Please Select DTC Type");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (cmbModem.SelectedItem.Text == "--Select--")
                {
                    cmbModem.Focus();
                    ShowMsgBox(" Please Select Modem");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                String a = cmbGuarenteeType.SelectedItem.Text;
                if (cmbFailureType.SelectedItem.Text == "--Select--")
                {
                    cmbFailureType.Focus();
                    ShowMsgBox(" Please Select Failure Type");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (cmbRepairer.SelectedIndex == 0)
                {
                    cmbRepairer.Focus();
                    ShowMsgBox(" Select Repairer");
                    cmdSave.Enabled = true;
                    return bValidate;
                }

                if (txtDTrCommDate.Text.Trim() == "")
                {
                    txtFailedDate.Focus();
                    ShowMsgBox("Dtr Commission Date is Empty Please verify");

                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtFailedDate.Text.Trim() == "")
                {
                    txtFailedDate.Focus();
                    ShowMsgBox("Enter Failure Date");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtTcCode.Text.Trim() == "0" || txtTcCode.Text.Trim() == "" || txtTcCode.Text.Trim() == null)
                {
                    ShowMsgBox("This Transformer Centre is Currently having No TC, please contact the DTLMS Team");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                string sResult = Genaral.DateComparision(txtFailedDate.Text, txtConnectionDate.Text, false, false);
                if (sResult == "2")
                {
                    ShowMsgBox("Failure Date should be Greater than Commission Date");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                sResult = Genaral.DateComparision(txtFailedDate.Text, "", true, false);
                if (sResult == "1")
                {
                    ShowMsgBox("Failure Date should be Less than Current Date");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (txtReason.Text.Trim() == "")
                {
                    txtReason.Focus();

                    ShowMsgBox("Enter the Failure Reason");
                    cmdSave.Enabled = true;
                    return bValidate;
                }
                if (rdbFailEnhance.Checked == true)
                {
                    if (cmbEnhanceCapacity.SelectedItem.Text == "--Select--")
                    {
                        cmbEnhanceCapacity.Focus();
                        ShowMsgBox("Select Enhance Capacity");
                        cmdSave.Enabled = true;
                        return bValidate;
                    }
                    if (cmbEnhanceCapacity.SelectedItem.Text == txtCapacity.Text)
                    {
                        cmbEnhanceCapacity.Focus();
                        ShowMsgBox("Select Different Capacity");
                        cmdSave.Enabled = true;
                        return bValidate;
                    }
                }
                if (txtConnectionDate.Text.Trim() == "" || txtConnectionDate.Text.Trim() == null)
                {
                    txtConnectionDate.Focus();
                    ShowMsgBox("Enter Commission Date");
                    cmdSave.Enabled = true;
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
        public void EmptyDTCDetails()
        {
            try
            {
                txtDTCName.Text = string.Empty;
                //txtServiceDate.Text = string.Empty;
                txtLoadKW.Text = string.Empty;
                txtLoadHP.Text = string.Empty;
                txtConnectionDate.Text = string.Empty;
                txtCapacity.Text = string.Empty;
                txtLocation.Text = string.Empty;
                txtTcCode.Text = string.Empty;
                txtTCMake.Text = string.Empty;
                txtTCSlno.Text = string.Empty;
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

                objApproval.sFormName = "FailureEntry";
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

        public void WorkFlowObjects(clsFailureEntry objFailure)
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


                objFailure.sFormName = "FailureEntry";
                objFailure.sOfficeCode = objSession.OfficeCode;
                objFailure.sClientIP = sClientIP;

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

                //  if (objSession.RoleId == "1")
                // {
                txtdocket1.Enabled = true;
                txtdocketdate.Enabled = true;
                txtCustName.Enabled = true;
                txtCustNo.Enabled = true;
                txtMeggerValue.Enabled = true;
                cmbPurpose.Enabled = true;
                //}

                if (txtActiontype.Text == "A")
                {
                    if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                    {
                        rdbFail.Checked = true;
                        //rdbFailEnhance.Enabled = false;
                    }
                    else
                    {
                        if (objSession.RoleId == "4")
                        {
                            if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                rdbFail.Checked = true;
                            }
                            else
                            {
                                rdbFailEnhance.Checked = true;
                                rdbFail.Checked = false;
                                cmbEnhanceCapacity.Visible = true;
                                cmbEnhanceCapacity.Enabled = true;
                                lblEnCap.Visible = true;
                            }
                        }
                        else
                        {
                            rdbFail.Enabled = false;
                            rdbFail.Checked = false;
                            rdbFailEnhance.Checked = true;
                            lblEnCap.Enabled = false;
                            cmbEnhanceCapacity.Enabled = false;
                            lblEnCap.Visible = true;
                            cmbEnhanceCapacity.Visible = true;
                        }
                    }

                    cmdSave.Text = "Approve";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "R")
                {
                    if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                    {
                        rdbFail.Checked = true;
                        rdbFailEnhance.Enabled = false;
                    }
                    else
                    {
                        rdbFail.Enabled = false;
                        rdbFail.Checked = false;
                        rdbFailEnhance.Checked = true;
                        lblEnCap.Enabled = false;
                        cmbEnhanceCapacity.Enabled = false;
                        lblEnCap.Visible = true;
                        cmbEnhanceCapacity.Visible = true;
                    }
                    cmdSave.Text = "Reject";
                    //pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    //cmbGuarenteeType.Enabled = true;
                    if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                    {
                        rdbFail.Checked = true;
                        rdbFailEnhance.Enabled = false;
                    }
                    else
                    {
                        rdbFail.Enabled = false;
                        rdbFail.Checked = false;
                        rdbFailEnhance.Checked = true;
                        lblEnCap.Enabled = false;
                        cmbEnhanceCapacity.Enabled = true;
                        lblEnCap.Visible = true;
                        cmbEnhanceCapacity.Visible = true;
                    }
                    cmdSave.Text = "Modify and Approve";
                    pnlApproval.Enabled = true;
                }

                if (txtActiontype.Text == "V" && hdfWFDataId.Value != "")
                {
                    if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                    {
                        rdbFail.Checked = true;
                        rdbFailEnhance.Enabled = false;
                    }
                    else
                    {
                        rdbFail.Enabled = false;
                        rdbFail.Checked = false;
                        rdbFailEnhance.Checked = true;
                        lblEnCap.Enabled = false;
                        cmbEnhanceCapacity.Enabled = false;
                        lblEnCap.Visible = true;
                        cmbEnhanceCapacity.Visible = true;

                    }
                }

                dvComments.Style.Add("display", "block");
                cmdReset.Enabled = false;


                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    pnlApproval.Enabled = true;

                    // To handle Record From Reject 
                    if (txtActiontype.Text == "A" && hdfWFDataId.Value != "")
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
            bool res = false;
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
                        txtFailurId.Text = objApproval.sNewRecordId;

                        cmdSave.Enabled = false;

                        if (objSession.RoleId == "1")
                        {
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            //EstimationReport(flag);
                        }
                        if (objSession.RoleId == "4")
                        {
                            string sWoID = string.Empty;
                            string sOffCode = objSession.OfficeCode;
                            clsFailureEntry ObjFailure = new clsFailureEntry();
                            sWoID = ObjFailure.getWoIDforEstimation(sOffCode, txtDTCCode.Text);
                            //EstimationReportSO(sWoID);
                        }

                    }
                    else if (objApproval.sApproveStatus == "3")
                    {
                        ShowMsgBox("Rejected Successfully");
                        cmdSave.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        if (cmdSave.Text == "Modify and Approve" || cmdSave.Text == "Approve")
                        {
                            res = GetFailDetails(objApproval.sNewRecordId);
                        }

                        if (res == true)
                        {
                            ShowMsgBox("Modified and Approved Successfully");
                            txtFailurId.Text = objApproval.sNewRecordId;
                            cmdSave.Enabled = false;

                            //  if (objSession.RoleId == "1")
                            // {
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                flag = "1";
                            }
                            else
                            {
                                flag = "4";
                            }
                            //EstimationReport(flag);
                            // }
                        }
                        else
                        {
                            ShowMsgBox("Something went wrong PGRS Docket no not updated");
                            txtFailurId.Text = objApproval.sNewRecordId;
                            cmdSave.Enabled = false;

                            //  if (objSession.RoleId == "1")
                            //{
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                flag = "1";
                            }
                            else
                            {
                                flag = "4";
                            }
                            //EstimationReport(flag);
                            // }
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
            bool res = false;
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
                        txtFailurId.Text = objApproval.sNewRecordId;

                        cmdSave.Enabled = false;

                        if (objSession.RoleId == "1")
                        {
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            //EstimationReport(flag);
                        }
                        if (objSession.RoleId == "4")
                        {
                            string sWoID = string.Empty;
                            string sOffCode = objSession.OfficeCode;
                            clsFailureEntry ObjFailure = new clsFailureEntry();
                            sWoID = ObjFailure.getWoIDforEstimation(sOffCode, txtDTCCode.Text);
                            //EstimationReportSO(sWoID);
                        }

                    }
                    else if (objApproval.sApproveStatus == "3")
                    {
                        ShowMsgBox("Rejected Successfully");
                        cmdSave.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "2")
                    {
                        if (cmdSave.Text == "Modify and Approve" || cmdSave.Text == "Approve")
                        {
                            res = GetFailDetails(objApproval.sNewRecordId);
                        }

                        if (res == true)
                        {
                            ShowMsgBox("Modified and Approved Successfully");
                            txtFailurId.Text = objApproval.sNewRecordId;
                            cmdSave.Enabled = false;

                            //  if (objSession.RoleId == "1")
                            // {
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                flag = "1";
                            }
                            else
                            {
                                flag = "4";
                            }
                            //EstimationReport(flag);
                            // }
                        }
                        else
                        {
                            ShowMsgBox("Something went wrong PGRS Docket no not updated");
                            txtFailurId.Text = objApproval.sNewRecordId;
                            cmdSave.Enabled = false;

                            //  if (objSession.RoleId == "1")
                            //{
                            clsFailureEntry objFailure = new clsFailureEntry();
                            //objFailure.SendSMStoSectionOfficer(txtFailureOfficCode.Text, txtDTCCode.Text);
                            if (txtCapacity.Text == cmbEnhanceCapacity.SelectedItem.Text)
                            {
                                flag = "1";
                            }
                            else
                            {
                                flag = "4";
                            }
                            //EstimationReport(flag);
                            // }
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
                throw ex;
            }
        }

        public bool GetFailDetails(string sFailure_id)
        {
            bool res = false;
            try
            {
                clsFailureEntry objFailDetails = new clsFailureEntry();
                string pgrsdocketno = Convert.ToString(ConfigurationManager.AppSettings["PgrsDocketno"]).ToUpper();
                string temp = /*txtdocket.Text +*/ txtdocket1.Text;
                string pgrsno = temp.ToUpper();
                objSession = (clsSession)Session["clsSession"];
                int roleid = Convert.ToInt32(objSession.RoleId);
                string consumermobilenum = txtCustName.Text;
                string consumername = txtCustNo.Text.Trim();
                res = objFailDetails.GetFailDetails_ForPGRS(sFailure_id, pgrsdocketno, objSession.sClientIP, objSession.UserId, roleid, objFailDetails, consumermobilenum, consumername, pgrsno);
                if (res == false)
                {
                    if (objFailDetails.spgrsstatus == "0")
                    {
                        ShowMsgBox("PGRS Docket Number Already Exist");
                    }
                    if (objFailDetails.spgrsstatus == "-1")
                    {
                        ShowMsgBox("PGRS Docket Number not generated ");
                    }
                    else if (objFailDetails.spgrsstatus.Contains("under") || objFailDetails.spgrsstatus == "")
                    {
                        ShowMsgBox(objFailDetails.spgrsstatus);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return res;
            }
        }

        public void WorkFlowConfig()
        {
            try
            {
                if (Request.QueryString["ActionType"] != null && Convert.ToString(Request.QueryString["ActionType"]) != "")
                {
                    //WFDataId

                    if (Session["WFOId"] != null && Convert.ToString(Session["WFOId"]) != "")
                    {
                        hdfWFDataId.Value = Convert.ToString(Session["WFDataId"]);
                        hdfWFOId.Value = Convert.ToString(Session["WFOId"]);
                        hdfApproveStatus.Value = Convert.ToString(Session["ApproveStatus"]);
                        hdfWFOAutoId.Value = Convert.ToString(Session["WFOAutoId"]);

                        Session["WFDataId"] = null;
                        Session["WFOId"] = null;
                        Session["ApproveStatus"] = null;
                        Session["WFOAutoId"] = null;
                    }

                    if (hdfWFDataId.Value != "0")
                    {
                        GetFailureDetailsFromXML(hdfWFDataId.Value);
                    }
                    SetControlText();
                    ControlEnableDisable();
                    if (txtActiontype.Text == "V")
                    {
                        cmdSave.Text = "View";
                        cmdReset.Enabled = false;
                        dvComments.Style.Add("display", "none");
                    }
                }
                else
                {
                    if (cmdSave.Text != "Save" && cmdSave.Text != "Submit" && cmdSave.Text != "Approve" && cmdSave.Text != "View")
                    {
                        cmdSave.Enabled = false;

                    }
                    //cmdSave.Text = "View";
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
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "FailureEntry");
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

        public void EstimationReport(string flag)
        {
            string sWFO_ID = string.Empty;
            try
            {
                if (txtFailurId.Text.Contains("-") || txtFailurId.Text == "0" || txtFailurId.Text == "")
                {



                    if (txtActiontype.Text == "R" && hdfWFOId.Value != null && hdfWFOId.Value != "" || hdfApproveStatus.Value != "")
                    {
                        sWFO_ID = hdfWFDataId.Value;
                    }
                    // sWFO_ID = txtwfo_id.Text;

                    else if (hdfWFOId.Value != null || hdfWFOId.Value != "")
                    {

                        sWFO_ID = Convert.ToString(Session["WFOId"]);

                    }
                    //if (hdfApproveStatus.Value !="")
                    //{
                    //    sWFO_ID = hdfWFDataId.Value;
                    //}

                    string strParam1 = string.Empty;
                    //strParam = "id=Estimation&FailureId=" + txtFailurId.Text;
                    strParam1 = "id=PgrsDocketSO&sWFOID=" + sWFO_ID;
                    RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam1 + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                    return;
                }
                //if (cmdSave.Text == "Save")
                //{
                clsEstimation objEst = new clsEstimation();
                objEst.sOfficeCode = txtFailureOfficCode.Text;
                objEst.sFailureId = txtFailurId.Text;
                objEst.sLastRepair = txtLastRepairer.Text;
                objEst.sCrby = objSession.UserId;

                //objEst.SaveEstimationDetails(objEst); To save estimation details(now this concept is not using) 
                //}

                string strParam = string.Empty;
                //strParam = "id=Estimation&FailureId=" + txtFailurId.Text;
                strParam = "id=PgrsDocket&FailureId=" + txtFailurId.Text;
                RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EstimationReportSO(string sWoID)
        {
            try
            {


                string STCcode = txtTcCode.Text;
                string strParam = string.Empty;
                strParam = "id=EstimationSO&TCcode=" + STCcode + "&WOId=" + sWoID + "&Failtype=" + flag;
                RegisterStartupScript("Print", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
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
                string sTCId = string.Empty;

                if (txtTcCode.Text.Trim() == "" || txtTcCode.Text.Trim() == null)
                {
                    txtTcCode.Focus();
                    ShowMsgBox("Enter DTR Code ");
                }
                else
                {
                    sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtTCId.Text));
                    string url = "/MasterForms/TcMaster.aspx?TCId=" + sTCId;
                    //string s = "window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');";
                    string s = "window.open('" + url + "', '_blank');";
                    ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
                }
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
                string sDTCId = string.Empty;
                if (txtDTCCode.Text.Trim() == "" || txtDTCCode.Text.Trim() == null)
                {
                    txtDTCCode.Focus();
                    ShowMsgBox("Enter the Transformer Centre Code ");
                }
                else
                {

                    sDTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(txtDtcId.Text));
                    string url = "/MasterForms/DTCCommision.aspx?QryDtcId=" + sDTCId;
                    string s = "window.open('" + url + "', '_blank');";
                    ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
                }
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
                txtDTCCode.Enabled = false;
                cmdSearch.Visible = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        #region Load From XML
        public void GetFailureDetailsFromXML(string sWFDataId)
        {
            try
            {
                // If the Data saved in Main Table then this function shd not execute, so done restriction like below
                // And commented for temprary purpose.. nee to change in future

                //if (!txtFailurId.Text.Contains("-"))
                //{
                //    return;
                //}

                clsFailureEntry objFailure = new clsFailureEntry();
                objFailure.sWFDataId = sWFDataId;
                objFailure.GetFailureDetailsFromXML(objFailure);

                txtDtcId.Text = objFailure.sDtcId;
                txtDTCCode.Text = objFailure.sDtcCode;
                txtDTCName.Text = objFailure.sDtcName;
                //txtServiceDate.Text = objFailure.sDtcServicedate;
                txtLoadKW.Text = objFailure.sDtcLoadKw;
                txtLoadHP.Text = objFailure.sDtcLoadHp;
                txtConnectionDate.Text = objFailure.sCommissionDate;
                txtCapacity.Text = objFailure.sDtcCapacity;
                txtLocation.Text = objFailure.sDtcLocation;
                txtTcCode.Text = objFailure.sDtcTcCode;
                txtTCSlno.Text = objFailure.sDtcTcSlno;
                if (txtdocket1.Text == "" || txtdocket1.Text == null)
                {
                    txtdocket1.Text = objFailure.sdocketno;
                }

                txtCustName.Text = objFailure.sCustName;
                txtCustNo.Text = objFailure.sCustNo;
                txtMeggerValue.Text = objFailure.sMeggerValue;
                cmbPurpose.SelectedValue = objFailure.sPurpose;
                cmbReplaceEntry.SelectedValue = objFailure.sAlternateReplaceType;
                txtConditionOfTC.Text = objFailure.sConditionoftc;
                cmbSilica.SelectedValue = objFailure.sSilicaCondition;
                cmbRepairer.SelectedValue = objFailure.sRepairer;
                if (objFailure.sEnhancedCapacity != null)
                    if (txtCapacity.Text == objFailure.sEnhancedCapacity)
                    {
                        cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text;
                    }
                cmbEnhanceCapacity.SelectedItem.Text = objFailure.sEnhancedCapacity;

                if (objFailure.sEnhancedCapacity == "" || objFailure.sEnhancedCapacity == null)
                {
                    cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text;
                }

                txtTCId.Text = objFailure.sTCId;

                txtLastRepairDate.Text = objFailure.sLastRepairedDate;
                txtLastRepairer.Text = objFailure.sLastRepairedBy;
                cmbGuarenteeType.SelectedItem.Text = objFailure.sGuarantyType;
                hdfGuarenteeSource.Value = objFailure.sGuarantySource;

                txtManfDate.Text = objFailure.sManfDate;
                txtDTrCommDate.Text = objFailure.sDTrCommissionDate;

                txtTCMake.Text = objFailure.sDtcTcMake;
                txtFailedDate.Text = objFailure.sFailureDate;
                txtReason.Text = objFailure.sFailureReasure;
                txtDTCRead.Text = objFailure.sDtcReadings;
                txtDTCCode.Enabled = false;
                txtFailureOfficCode.Text = objFailure.sOfficeCode;
                hdfCrBy.Value = objFailure.sCrby;
                cmbOilType.SelectedValue = objFailure.sOilType;
                cmbDTCType.SelectedValue = objFailure.sDTCType;
                cmbModem.SelectedValue = objFailure.sModem;

                if (txtdocketdate.Text == "" || txtdocketdate.Text == null)
                {
                    txtdocketdate.Text = objFailure.sdocketDate;
                }

                cmbFailureType.SelectedValue = objFailure.sFailureType.Trim();
                if (objFailure.sHTBusing != "")
                {
                    cmbHtBusing.SelectedValue = objFailure.sHTBusing;
                }
                if (objFailure.sLTBusing != "")
                {
                    cmbLtBusing.SelectedValue = objFailure.sLTBusing;
                }
                if (objFailure.sHTBusingRod != "")
                {
                    cmbHtBusingRod.SelectedValue = objFailure.sHTBusingRod;
                }
                if (objFailure.sLTBusingRod != "")
                {
                    cmbLtBusingRod.SelectedValue = objFailure.sLTBusingRod;
                }
                if (objFailure.sDrainValve != "")
                {
                    cmbDrainValve.SelectedValue = objFailure.sDrainValve;
                }
                if (objFailure.sOilLevel != "")
                {
                    cmbOilLevel.SelectedValue = objFailure.sOilLevel;
                }
                if (objFailure.sOilQuantity != "")
                {
                    txtQuantityOfOil.Text = objFailure.sOilQuantity;

                }
                if (objFailure.sTankCondition != "")
                {
                    cmbTankCondition.SelectedValue = objFailure.sTankCondition;
                }

                if (objFailure.sExplosionValve != "")
                {
                    cmbExplosion.SelectedValue = objFailure.sExplosionValve;
                }
                if (objFailure.sBreather != "")
                {
                    cmbBreather.SelectedValue = objFailure.sBreather;
                }
                if (objFailure.sSilicaCondition != "")
                {
                    if (objFailure.sSilicaCondition == "0")
                    {
                        cmbSilica.SelectedValue = objFailure.sSilicaCondition;
                        silicagel.Visible = false;
                    }
                    else
                    {
                        cmbSilica.SelectedValue = objFailure.sSilicaCondition;
                    }

                }

                if (objFailure.sFailureId != "0")
                {

                    //cmdSave.Text = "Update";
                    cmdSearch.Visible = false;
                    cmdReset.Enabled = false;
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
                strQry = "Title=Search and Select Transformer Centre Failure Details&";
                strQry += "Query=select \"DT_CODE\",\"DT_NAME\" FROM \"TBLDTCMAST\" WHERE CAST(\"DT_OM_SLNO\" AS TEXT) LIKE '" + objSession.OfficeCode + "%' AND ";
                strQry += " \"DT_CODE\" NOT IN (SELECT \"DF_DTC_CODE\" FROM \"TBLDTCFAILURE\" WHERE  \"DF_REPLACE_FLAG\" = 0 ) AND {0} like %{1}% order by \"DT_CODE\" &";
                strQry += "DBColName=\"DT_CODE\"~\"DT_NAME\"&";
                strQry += "ColDisplayName=Transformer Centre Code~Transformer Centre Name&";

                strQry = strQry.Replace("'", @"\'");

                cmdSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtDTCCode.ClientID + "&btn=" + cmdSearch.ClientID + "',520,520," + txtDTCCode.ClientID + ")");

                txtFailedDate.Attributes.Add("onblur", "return ValidateDate(" + txtFailedDate.ClientID + ");");

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void rdbFailEnhance_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbFailEnhance.Checked == true)
            {
                lblEnCap.Visible = true;
                cmbEnhanceCapacity.Visible = true;
                cmbEnhanceCapacity.Enabled = true;
                Genaral.Load_Combo("SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'C' AND \"MD_NAME\" <>'" + txtCapacity.Text + "' ORDER BY \"MD_ORDER_BY\" ", "--Select--", cmbEnhanceCapacity);
            }
            else
            {
                lblEnCap.Visible = false;
                cmbEnhanceCapacity.Visible = false;
                cmbEnhanceCapacity.Enabled = false;
                cmbEnhanceCapacity.SelectedItem.Text = txtCapacity.Text.Trim();
            }
        }


        protected void cmbBreather_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string sBreather = cmbBreather.SelectedValue;

                if (sBreather == "2")
                {
                    silicagel.Visible = false;
                }

                else
                {
                    silicagel.Visible = true;
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