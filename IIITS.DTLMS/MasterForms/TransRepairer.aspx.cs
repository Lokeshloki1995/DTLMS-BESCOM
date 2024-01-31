﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.Collections;

namespace IIITS.DTLMS.MasterForms
{
    public partial class TransRepairer : System.Web.UI.Page
    {

        string strFormCode = "TransRepairer.aspx";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                Form.DefaultButton = cmdSave.UniqueID;
                Update.Visible = false;
                UpdateRepairer.Visible = false;
                objSession = (clsSession)Session["clsSession"];
                lblMessage.Text = string.Empty;
                txtDwaDate.Attributes.Add("readonly", "readonly");
                txtStartDate.Attributes.Add("readonly", "readonly");
                txtEndDate.Attributes.Add("readonly", "readonly");
                txtBlackUpto.Attributes.Add("readonly", "readonly");
                txtExtDate.Attributes.Add("readonly", "readonly");
                txtExtStartDate.Attributes.Add("readonly", "readonly");
                txtExtEndDate.Attributes.Add("readonly", "readonly");

                txtExtEndDate_CalendarExtender2.EndDate = System.DateTime.Now;
                txtExtStartDate_CalendarExtender1.EndDate = System.DateTime.Now;
                txtExtDate_CalendarExtender1.EndDate = System.DateTime.Now;
                txtdateExtender.StartDate = System.DateTime.Now;
                //txtEndDate_CalendarExtender1.EndDate = System.DateTime.Now;
                txtStartDate_CalendarExtender1.EndDate = System.DateTime.Now;
                txtDwaDate_CalendarExtender2.EndDate = System.DateTime.Now;
                CheckAccessRights("4");
                if (!IsPostBack)
                {
                    if (Request.QueryString["StrQryId"] != null && Request.QueryString["StrQryId"].ToString() != "")
                    {
                        txtRepairerId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["StrQryId"]));
                    }
                        if (Request.QueryString["OfficeCode"] != null && Request.QueryString["OfficeCode"].ToString() != "")
                        {
                            txthdOfficeCode.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["OfficeCode"]));
                    }
                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\"", "-Select-", cmbDivision);

                    cmbIsBlack.SelectedIndex = 2;
                    txtBlackUpto.Enabled = false;
                    txtBlackUpto.Text = string.Empty;
                    if (txtRepairerId.Text != "")
                    {
                        GetRepairerDetails(txtRepairerId.Text);
                        cmbIsBlack_SelectedIndexChanged(sender, e);
                        Create.Visible = false;
                        CreateRepairer.Visible = false;

                        Update.Visible = true;
                        UpdateRepairer.Visible = true;
                    }

                    txtBlackUpto.Attributes.Add("onblur", "return ValidateDate(" + txtBlackUpto.ClientID + ");");
                    // CheckAccessRights("4");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        public void GetRepairerDetails(string strRepairerId)
        {
            try
            {
                DataTable dtBillDetails = new DataTable();
                clsTransRepairer objRepair = new clsTransRepairer();

                objRepair.RepairerId = strRepairerId;
                objRepair.sOffCode=txthdOfficeCode.Text;
            //    objRepair.GetRepairerDetails(objRepair);
                objRepair.GetRepairerDetailsnew(objRepair);

                txtRepairerId.Text = objRepair.RepairerId;
                txtRepairName.Text = objRepair.RepairerName;
                txtRepairAddress.Text = objRepair.RegisterAddress;
                txtRepairPhnNo.Text = objRepair.RepairerPhoneNo;
                txtRepairEmailId.Text = objRepair.RepairerEmail;
                //txtOfficeCode.Text = objRepair.sOffCode;
                txtOfficeCode.Text = txthdOfficeCode.Text;
                cmbIsBlack.Text = objRepair.RepairerBlacklisted;
                txtBlackUpto.Text = objRepair.RepairerBlackedupto;
                txtCommAddress.Text = objRepair.CommAddress;
                txtremarks.Text = objRepair.Remarks;
                txtContactPerson.Text = objRepair.sContactPerson;
                txtContractPeriod.Text = objRepair.sContractPeriod;
                txtFaxNo.Text = objRepair.sFax;
                txtMobileNo.Text = objRepair.sMobileNo;
                txtStartDate.Text = objRepair.sStartDate;
                txtEndDate.Text = objRepair.sEndDate;
                txtDwaNO.Text = objRepair.sDWANo;
                txtDwaDate.Text = objRepair.sDwaDate;
                cmdSave.Text = "Update";

                txtRepairName.Enabled = false;
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
                clsTransRepairer objRepairer = new clsTransRepairer();
                string[] Arr = new string[2];

                //Check AccessRights
                bool bAccResult;
                if (cmdSave.Text == "Update")
                {
                    bAccResult = CheckAccessRights("3");
                }
                else
                {
                    bAccResult = CheckAccessRights("2");
                }

                if (bAccResult == false)
                {
                    return;
                }

                if (ValidateForm() == true)
                {
                    objRepairer.sOffCode = txtOfficeCode.Text;
                    objRepairer.RepairerId = txtRepairerId.Text;
                    objRepairer.RepairerName = txtRepairName.Text.ToUpper().Replace("'", "");
                    objRepairer.RegisterAddress = txtRepairAddress.Text.Replace("'", "");
                    objRepairer.RepairerPhoneNo = txtRepairPhnNo.Text.Replace("'", "");
                    objRepairer.RepairerEmail = txtRepairEmailId.Text.Replace("'", "").ToLower();
                    // objRepairer.RepairerType = cmbType.SelectedValue;
                    if (cmbIsBlack.SelectedIndex > 0)
                    {
                        objRepairer.RepairerBlacklisted = cmbIsBlack.SelectedValue;
                    }

                    objRepairer.RepairerBlackedupto = txtBlackUpto.Text;
                    objRepairer.sContractPeriod = txtContractPeriod.Text;
                    objRepairer.CommAddress = txtCommAddress.Text.Replace("'", "");
                    objRepairer.Remarks = txtremarks.Text.Replace("'", "");
                    objRepairer.sCrby = objSession.UserId;
                    objRepairer.sContactPerson = txtContactPerson.Text.Trim().Replace("'", "");
                    objRepairer.sFax = txtFaxNo.Text.Trim();
                    objRepairer.sMobileNo = txtMobileNo.Text.Trim();
                    objRepairer.sStartDate = txtStartDate.Text;
                    objRepairer.sEndDate = txtEndDate.Text;
                    if(txtDwaNO.Text == "")
                    {
                        objRepairer.sDWANo = "0";
                    }
                    else
                    {
                        objRepairer.sDWANo = txtDwaNO.Text;
                    }
                    if(txtDwaDate.Text == "")
                    {
                        objRepairer.sDwaDate = "0";
                    }
                    else
                    {
                        objRepairer.sDwaDate = txtDwaDate.Text;
                    }
                    

                    if (chkExtension.Checked == true)
                    {
                        objRepairer.sExtOmNo = txtExtOM.Text;
                        objRepairer.sExtDate = txtExtDate.Text;
                        objRepairer.sExtPeriod = txtExtPeriod.Text;
                        objRepairer.sExtStartDate = txtExtStartDate.Text;
                        objRepairer.sExtEndDate = txtExtEndDate.Text;
                        objRepairer.sChkEnable = "1";
                    }
                    else
                    {
                        objRepairer.sExtOmNo = "0";
                        objRepairer.sExtDate = "0";
                        objRepairer.sExtPeriod = "0";
                        objRepairer.sExtStartDate = "0";
                        objRepairer.sExtEndDate = "0";
                        objRepairer.sChkEnable = "0";
                    }

                    Arr = objRepairer.SaveRepairerDetails(objRepairer);

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "Repairer Master");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        //txtRepairerId.Text = Arr[0].ToString();
                        //cmdSave.Text = "Update";
                        ShowMsgBox(Arr[0]);

                        //cmdReset_Click(sender, e);
                    }


                    if (Arr[1].ToString() == "1")
                    {

                        ShowMsgBox(Arr[0]);
                     //   Reset();

                    }

                    if (Arr[1].ToString() == "4")
                    {
                        ShowMsgBox(Arr[0]);

                    }
                }
                return;
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



        bool ValidateForm()
        {
            bool bValidate = false;
            try
            {
                if (txtRepairName.Text == "")
                {
                    txtRepairName.Focus();
                    ShowMsgBox("Please Enter the Name of Repairer");
                    return bValidate;
                }
                if (txtRepairAddress.Text == "")
                {
                    txtRepairAddress.Focus();
                    ShowMsgBox("Please Enter Valid Register Address");
                    return bValidate;
                }
                if (txtRepairPhnNo.Text == "")
                {
                    txtRepairPhnNo.Focus();
                    ShowMsgBox("Please Enter 11 Digit Valid Phone No");
                    return bValidate;
                }

                if (txtRepairPhnNo.Text.Length < 10)
                {
                    ShowMsgBox("Enter valid  Phone no");
                    txtRepairPhnNo.Focus();
                    return bValidate;
                }

                if (txtRepairEmailId.Text == "")
                {
                    txtRepairEmailId.Focus();
                    ShowMsgBox("Please Enter the EmailId");
                    return bValidate;
                }

                if (txtOfficeCode.Text == "")
                {
                    txtOfficeCode.Focus();
                    ShowMsgBox("Select Division");
                    return bValidate;
                }

                //if (cmbIsBlack.SelectedIndex == 0)
                //{
                //    ShowMsgBox("Please Select the Blacklisted condition");
                //    cmbIsBlack.Focus();
                //    return bValidate;
                //}
                if(cmbIsBlack.SelectedIndex == 0)
                {
                    ShowMsgBox("Please select the BlockList");
                    cmbIsBlack.Focus();
                }
                else
                {
                    if (cmbIsBlack.SelectedValue == "1")
                    {
                        if (txtBlackUpto.Text == "")
                        {
                            ShowMsgBox("Please select the BlockListed Upto Date");
                            txtBlackUpto.Focus();
                            return bValidate;
                        }
                        if (txtBlackUpto.Text != "")
                        {
                            string sRet = Genaral.DateValidation(txtBlackUpto.Text);
                            if (sRet != "")
                            {
                                ShowMsgBox(sRet);
                                return bValidate;
                            }
                        }
                    }
                }
                

                if (txtFaxNo.Text.Trim() != "")
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(txtFaxNo.Text, "^[0-9 \\-\\s \\( \\)]*$"))
                    {
                        ShowMsgBox("Please Enter Valid Fax No (Eg:865-934-1234)");
                        return bValidate;
                    }
                }

                if (txtRepairPhnNo.Text != "")
                {
                    if ((txtRepairPhnNo.Text.Length - txtRepairPhnNo.Text.Replace("-", "").Length) >= 2)
                    {
                        txtRepairPhnNo.Focus();
                        ShowMsgBox("You cannot use more than one hyphen (-)");
                        return bValidate;
                    }

                    if (txtRepairPhnNo.Text.Contains(".") == true)
                    {
                        txtRepairPhnNo.Focus();
                        ShowMsgBox("You cannot enter (.) in Phone Number");
                        return bValidate;
                    }

                }

                if (txtStartDate.Text.Trim().Length == 0)
                {
                    txtStartDate.Focus();
                    ShowMsgBox("Please enter the Contract Start Date");
                    return bValidate;
                }

                if (txtEndDate.Text.Trim().Length == 0)
                {
                    txtEndDate.Focus();
                    ShowMsgBox("Please enter the Contract End Date");
                    return bValidate;
                }

                string sResult = Genaral.DateComparision(txtStartDate.Text, txtEndDate.Text, false, false);
                if (sResult == "1")
                {
                    ShowMsgBox("Contract start Date should be Less than Contract End Date");
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
                txtRepairerId.Text = string.Empty;
                txtRepairName.Text = string.Empty;
                txtRepairAddress.Text = string.Empty;
                txtRepairPhnNo.Text = string.Empty;
                txtRepairEmailId.Text = string.Empty;
                txtBlackUpto.Text = string.Empty;
                cmbIsBlack.SelectedIndex = 0;
                //cmbType.SelectedIndex = 0;
                cmdSave.Text = "Save";
                txtCommAddress.Text = string.Empty;
                txtremarks.Text = string.Empty;
                txtContactPerson.Text = string.Empty;
                txtFaxNo.Text = string.Empty;
                txtMobileNo.Text = string.Empty;
                txtOfficeCode.Text = string.Empty;
                txtContractPeriod.Text = string.Empty;
                txtStartDate.Text = String.Empty;
                txtEndDate.Text = String.Empty; 
                txtRepairName.Enabled = true;
                txtDwaNO.Text = string.Empty;
                txtEndDate.Text = string.Empty;
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
                // Response.Redirect("TransRepairer.aspx", false); 

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void btnExtReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtExtDate.Text = "";
                txtExtOM.Text = "";
                txtExtPeriod.Text = "";
                txtExtStartDate.Text = "";
                txtExtEndDate.Text = "";
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbIsBlack_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbIsBlack.SelectedIndex == 1)
                {
                    txtBlackUpto.Enabled = true;
                    blocklist.Visible = true;
                }
                else
                {
                    blocklist.Visible = false;
                    txtBlackUpto.Enabled = false;
                    txtBlackUpto.Text = string.Empty;
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

                objApproval.sFormName = "TransRepairer";
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

        protected void chkExtension_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkExtension.Checked == true)
                {
                    DivExtension.Style.Add("display", "block");
                }
                else
                {
                    DivExtension.Style.Add("display", "none");
                }
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.mdlPopup.Show();
               

                if (ViewState["CHECKED_ITEMS"] != null)
                {
                    SaveCheckedValues();
                    LoadOffice(objSession.OfficeCode);
                    PopulateCheckedValues();
                }
                else
                {
                    LoadOffice(objSession.OfficeCode);
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void SaveCheckedValues()
        {
            try
            {
                ArrayList userdetails = new ArrayList();
                ArrayList tmpArrayList = new ArrayList();

                int index = -1;
                string strIndex = string.Empty;
                string strOk1 = string.Empty;
                foreach (GridViewRow gvrow in GrdOffices.Rows)
                {
                    index = Convert.ToInt32(GrdOffices.DataKeys[gvrow.RowIndex].Value);
                    CheckBox result = ((CheckBox)gvrow.FindControl("cbSelect"));
                    // Check in the Session
                    if ((ArrayList)ViewState["CHECKED_ITEMS"] != null)
                        userdetails = (ArrayList)ViewState["CHECKED_ITEMS"];


                    Label lblOff = (Label)gvrow.FindControl("lblOffCode");

                    if (result.Checked == true)
                    {
                        if (!userdetails.Contains(index))
                        {
                            userdetails.Add(index);
                        }
                    }

                    else
                    {
                        if (userdetails.Contains(index))
                        {
                            userdetails.Remove(index);
                        }
                    }
                }
                if (userdetails != null && userdetails.Count > 0)
                    ViewState["CHECKED_ITEMS"] = userdetails;
            }
            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void LoadOffice(string sOfficeCode = "", string sOffName = "")
        {
            try
            {
                DataTable dtPageDetaiils = new DataTable();
                clsStore Objstore = new clsStore();
                Objstore.sOfficeCode = sOfficeCode;
                Objstore.sOfficeName = sOffName;
                if(objSession.sRoleType=="2")
                {
                    string officecode = clsStoreOffice.GetZone_Circle_Div_Offcode(sOfficeCode,objSession.RoleId);

                    Objstore.sOfficeCode = officecode.Substring(0, 2);
                }
                else
                {
                    string officecode = sOfficeCode;
                    if (officecode=="")
                    {
                        Objstore.sOfficeCode =officecode;
                    }
                    else
                    {
                         Objstore.sOfficeCode = officecode.Substring(0, 2);
                    }

                   
                }
                dtPageDetaiils = Objstore.LoadOfficeDet(Objstore);
                //if (dtPageDetaiils.Rows.Count > 0)
                //{
                GrdOffices.DataSource = dtPageDetaiils;
                GrdOffices.DataBind();
            }

            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void PopulateCheckedValues()
        {
            try
            {
                ArrayList userdetails = (ArrayList)ViewState["CHECKED_ITEMS"];

                if (userdetails != null && userdetails.Count > 0)
                {
                    foreach (GridViewRow gvrow in GrdOffices.Rows)
                    {
                        int index = Convert.ToInt32(GrdOffices.DataKeys[gvrow.RowIndex].Value);
                        if (userdetails.Contains(index))
                        {
                            CheckBox myCheckBox = (CheckBox)gvrow.FindControl("cbSelect");
                            myCheckBox.Checked = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // lblErrormsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void GrdOffices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "search")
                {
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    TextBox txtOffCode = (TextBox)row.FindControl("txtOffCode");
                    TextBox txtOffName = (TextBox)row.FindControl("txtOffName");
                    LoadOffice(txtOffCode.Text.Trim().Replace("'", "''"), txtOffName.Text.Trim().Replace("'", "''"));
                    this.mdlPopup.Show();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void GrdOffices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                foreach (GridViewRow Row in GrdOffices.Rows)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)//except header and footer
                    {
                        TextBox txtOff = new TextBox();
                        CheckBox cbSelect = new CheckBox();
                        ArrayList arroffcode = new ArrayList();

                        cbSelect = (CheckBox)e.Row.FindControl("cbSelect");
                        Label lblOff = new Label();

                        lblOff = (Label)Row.FindControl("lblOffCode");
                    }
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        protected void GrdOffices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                SaveCheckedValues();
                GrdOffices.PageIndex = 0;
                GrdOffices.PageIndex = e.NewPageIndex;
                LoadOffice(objSession.OfficeCode);
                PopulateCheckedValues();
                this.mdlPopup.Show();
            }
            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void btnOK_Click1(object sender, EventArgs e)
        {
            try
            {
                ArrayList arrChecked = new ArrayList();

                GrdOffices.AllowPaging = false;
                SaveCheckedValues();
                LoadOffice(objSession.OfficeCode);
                PopulateCheckedValues();

                foreach (GridViewRow Row in GrdOffices.Rows)
                {
                    bool result = ((CheckBox)Row.FindControl("cbSelect")).Checked;

                    if (result == true)
                    {
                        arrChecked.Add(((Label)Row.FindControl("lblOffCode")).Text);
                    }
                }

                GrdOffices.AllowPaging = true;
                SaveCheckedValues();
                LoadOffice(objSession.OfficeCode);
                PopulateCheckedValues();


                string sOfficeCode = string.Empty;

                for (int i = 0; i < arrChecked.Count; i++)
                {
                    sOfficeCode += arrChecked[i];
                    if (sOfficeCode.StartsWith(",") == false)
                    {
                        //sOfficeCode =  sOfficeCode;
                    }
                    if (sOfficeCode.EndsWith(",") == false)
                    {
                        sOfficeCode = sOfficeCode + ",";
                    }
                }

                //txtOfficeCode.Text = strOk;
                if (sOfficeCode.EndsWith(",") == true)
                {
                    sOfficeCode = sOfficeCode.Remove(sOfficeCode.Length - 1);
                }

                txtOfficeCode.Text = sOfficeCode;
                txtOfficeCode.Enabled = false;
            }
            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
    }
}
