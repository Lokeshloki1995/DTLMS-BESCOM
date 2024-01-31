﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Configuration;


namespace IIITS.DTLMS.TCRepair
{
    public partial class DeliverTC : System.Web.UI.Page
    {

        string strFormCode = "DeliverTC";
        clsSession objSession;
        string SDtrCode;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                Form.DefaultButton = cmdSave.UniqueID;
                objSession = (clsSession)Session["clsSession"];
                lblMessage.Text = string.Empty;
               txtDeliverdate.Attributes.Add("readonly", "readonly");
                txtRVDate.Attributes.Add("readonly", "readonly");

                RVDateCalender.EndDate = System.DateTime.Now;
                DeliverCalender.EndDate = System.DateTime.Now;

                if (!IsPostBack)
                {

                    if (Request.QueryString["RepairMasterId"] != null && Request.QueryString["RepairMasterId"].ToString() != "")
                    {
                        txtRepairMasterId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["RepairMasterId"]));
                        txtInsResultId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["InsResult"]));
                    }

                    string sOfficeCode = string.Empty;
                    int Division = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
                    if (objSession.OfficeCode.Length > 2)
                    {
                        sOfficeCode = objSession.OfficeCode.Substring(0, Division);
                    }
                    else
                    {
                        sOfficeCode = objSession.OfficeCode;
                    }

                    if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        Genaral.Load_Combo("SELECT  CAST(\"SM_ID\" AS TEXT) StoreID,\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\" ='A'  and \"SM_ID\"='" + clsStoreOffice.GetStoreID(objSession.OfficeCode) + "' ORDER BY \"SM_NAME\" ", cmbStore);
                    }
                    else
                    {
                        Genaral.Load_Combo("SELECT  CAST(\"SM_ID\" AS TEXT) StoreID,\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_STATUS\" ='A' ORDER BY \"SM_NAME\" ", "--Select--", cmbStore);
                    }
                    Genaral.Load_Combo("SELECT \"US_ID\",\"US_FULL_NAME\" FROM \"TBLUSER\" WHERE COALESCE(\"US_EFFECT_FROM\",NOW()- 1 * interval '1' day)<=NOW() AND \"US_STATUS\" ='A' AND \"US_OFFICE_CODE\" LIKE '" + sOfficeCode + "%'", "--Select--", cmbVerifiedBy);
                    LoadRecievePending();


                    //From DTR Tracker
                    if (Request.QueryString["TransId"] != null && Request.QueryString["TransId"].ToString() != "")
                    {
                        string sRepairDetailsId = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TransId"]));

                        GetRepairRecieveDetails(sRepairDetailsId);

                        cmdSave.Enabled = false;
                    }

                    LoadRecievedDTr();

                    //txtDeliverdate.Attributes.Add("onblur", "return ValidateDate(" + txtDeliverdate.ClientID + ");");
                    //DeliverCalender.StartDate = System.DateTime.Now;
                    // txtRIDate.Attributes.Add("onblur", "return ValidateDate(" + txtRIDate.ClientID + ");");
                    clsDTrRepairActivity objDeliver = new clsDTrRepairActivity();

                    //string res = objDeliver.getWarentyStatus(SDtrCode);

                    //if (res == "" || res == null)
                    //{
                    //    txtWarrentyPeriod.ReadOnly = true;
                    //    cmbGuarantyType.Enabled = true;
                    //}
                    //else
                    //{
                    //    txtWarrentyPeriod.ReadOnly = false;
                    //    txtWarrentyPeriod.Text = res;
                    //    cmbGuarantyType.Enabled = false;
                    //    cmbGuarantyType.SelectedValue = "WRGP";
                    //}

                    GetStoreId();
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                lblMessage.Text = clsException.ErrorMsg();
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                string[] Arr = new string[2];

                //Check AccessRights
                bool bAccResult = CheckAccessRights("2");
                if (bAccResult == false)
                {
                    return;
                }


                if (ValidateForm() == true)
                {

                    clsDTrRepairActivity objDeliver = new clsDTrRepairActivity();
                    objDeliver.sDeliverDate = txtDeliverdate.Text;
                    objDeliver.sDeliverChallenNo = txtChallenNo.Text;
                    objDeliver.sVerifiedby = cmbVerifiedBy.SelectedValue;
                    objDeliver.sStoreId = cmbStore.SelectedValue;
                    objDeliver.sCrby = objSession.UserId;
                    objDeliver.sRVNo = txtRVNo.Text;
                    objDeliver.sRVDate = txtRVDate.Text;


                    //objDeliver.sWarrantyPeriod = ConfigurationManager.AppSettings["WarrentyPeriod"].ToString();
                    //objDeliver.sGuarantyType = ConfigurationManager.AppSettings["WarrentyTypeWRGP"].ToString();
                    //objDeliver.sStatus = "1";



                    if (grdReceivePending.Rows.Count == 0)
                    {
                        ShowMsgBox("No Transformer Exists to Recieve");
                        return;
                    }

                    int i = 0;
                    bool bChecked = false;
                    string[] strQryVallist = new string[grdReceivePending.Rows.Count];
                    string sInsRes = string.Empty;
                    string sGuarantyType = string.Empty;
                    foreach (GridViewRow row in grdReceivePending.Rows)
                    {

                        if (((CheckBox)row.FindControl("chkSelect")).Checked == true)
                        {

                            //sInsRes = (((Label)row.FindControl("lblInsResult")).Text.ToUpper() == "NONE") ? "4" : (((Label)row.FindControl("lblInsResult")).Text.ToUpper() == "PASS") ? "1" : "3";                            
                            //strQryVallist[i] = ((Label)row.FindControl("lblRepairDetailsId")).Text.Trim() + "~" + ((Label)row.FindControl("lblTcCode")).Text.Trim() + "~" + sInsRes + "~" + ((TextBox)row.FindControl("txtTCWarrenty")).Text.Trim() + "~" + ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;

                            sInsRes = (((Label)row.FindControl("lblInsResult")).Text.ToUpper() == "NONE") ? "4" : (((Label)row.FindControl("lblInsResult")).Text.ToUpper() == "PASS") ? "1" : "3";
                            sGuarantyType = (((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue == "--Select--") ? "" : ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;
                            strQryVallist[i] = ((Label)row.FindControl("lblRepairDetailsId")).Text.Trim() + "~" + ((Label)row.FindControl("lblTcCode")).Text.Trim() + "~" + sInsRes + "~" + ((TextBox)row.FindControl("txtTCWarrenty")).Text.Trim() + "~" + sGuarantyType;


                            bChecked = true;
                        }
                        i++;

                    }

                    if (bChecked == false)
                    {
                        ShowMsgBox("Please Select any Transformers");
                        return;
                    }

                    //string[] strQrylist = new string[grdReceivePending.Rows.Count];
                    //foreach (GridViewRow row in grdReceivePending.Rows)
                    //{
                    //    strQrylist[i] = ((Label)row.FindControl("lbltcid")).Text.Trim() + "~" + ((Label)row.FindControl("lbltransid")).Text.Trim();
                    //    i++;
                    //}

                    Arr = objDeliver.SaveDeliverTCDetails(strQryVallist, objDeliver);

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (DeliverTC) Repair ");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        //ShowMsgBox(Arr[0].ToString());
                        grdReceivePending.DataSource = null;
                        grdReceivePending.DataBind();
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), UniqueID, "alert('" + Arr[0].ToString() + "'); location.href='DeliverPendingSearch.aspx';", true);
                        Reset();

                        return;
                    }
                    else
                    {
                        ShowMsgBox("No Transformer Exists to Recieve");
                    }
                }


            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                lblMessage.Text = clsException.ErrorMsg();
            }
        }


        private void LoadRecievePending()
        {
            try
            {
                clsDTrRepairActivity objDeliverpending = new clsDTrRepairActivity();

                DataTable dt = new DataTable();

                dt = objDeliverpending.LoadPendingForRecieve(txtRepairMasterId.Text);
                if(dt.Rows.Count > 0)
                {
                    string startDate = Convert.ToString(dt.Rows[0]["RSM_ISSUE_DATE"]);
                    string State = Convert.ToString(dt.Rows[0]["state"]);
                    SDtrCode = dt.Rows[0]["TC_CODE"].ToString();
                    DateTime Issuedate = Convert.ToDateTime(startDate);
                    int Isuedate = Convert.ToInt32((DateTime.Now.AddDays(-1) - Issuedate).TotalDays);
                    DeliverCalender.StartDate = DateTime.Today.AddDays(-Isuedate);
                    DeliverCalender.EndDate = System.DateTime.Now.AddDays(0);
                    
                    RVDateCalender.StartDate = DateTime.Today.AddDays(-Isuedate);
                    RVDateCalender.EndDate = System.DateTime.Now.AddDays(0);
                    grdReceivePending.DataSource = dt;
                    ViewState["RecvPending"] = dt;
                    grdReceivePending.DataBind();
                    foreach (GridViewRow row in grdReceivePending.Rows)
                    {
                        State = ((Label)row.FindControl("lblInsResult")).Text;
                        TextBox lblstatus = (TextBox)row.FindControl("txtWarrenty");
                        if (lblstatus.Text == "1")
                        {
                            ((TextBox)row.FindControl("txtTCWarrenty")).Enabled = false;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedItem.Text = dt.Rows[0]["WARRENTY_TYPE"].ToString();
                            objDeliverpending.sGuarantyType = ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).Enabled = false;
                        }
                        else
                        {
                            ((TextBox)row.FindControl("txtTCWarrenty")).Enabled = true;
                            objDeliverpending.sWarrantyPeriod = lblstatus.Text;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).Enabled = true;
                            objDeliverpending.sGuarantyType = ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;
                        }
                        if(State== "SCRAP")
                        {
                            //txtTCWarrenty.visible = false;
                            txtDeliverdate.Text = DeliverCalender.SelectedDate.ToString();
                            txtRVDate.Text= RVDateCalender.SelectedDate.ToString();
                            ((TextBox)row.FindControl("txtTCWarrenty")).Enabled = false;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedItem.Text = dt.Rows[0]["WARRENTY_TYPE"].ToString();
                            ((DropDownList)row.FindControl("cmbGuarantyType")).Enabled = false;
                        }
                    }

                    foreach (GridViewRow row in grdReceivePending.Rows)
                    {
                        Label lblstate = (Label)row.FindControl("lblInsResult");
                        if (lblstate.Text == "NONE")
                        {
                            ((TextBox)row.FindControl("txtTCWarrenty")).Enabled = false;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedItem.Text = dt.Rows[0]["WARRENTY_TYPE"].ToString();
                            objDeliverpending.sGuarantyType = ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;
                            ((DropDownList)row.FindControl("cmbGuarantyType")).Enabled = false;
                        }
                    }

                    }
                else
                {
                    ShowEmptyGrid();
                }
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                lblMessage.Text = clsException.ErrorMsg();
            }

        }

        public void ShowEmptyGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow newRow = dt.NewRow();
                dt.Rows.Add(newRow);
                dt.Columns.Add("RSD_ID");
                dt.Columns.Add("STATE");
                dt.Columns.Add("TC_CODE");
                dt.Columns.Add("TC_SLNO");
                dt.Columns.Add("MAKE");
                dt.Columns.Add("CAPACITY");
                dt.Columns.Add("TC_MANF_DATE");
                dt.Columns.Add("SUP_REPNAME");
                dt.Columns.Add("TC_WARANTY_PERIOD");
                dt.Columns.Add("STATUS");
                dt.Columns.Add("TC_WARRENTY");

                grdReceivePending.DataSource = dt;
                grdReceivePending.DataBind();
                int iColCount = grdReceivePending.Rows[0].Cells.Count;
                grdReceivePending.Rows[0].Cells.Clear();
                grdReceivePending.Rows[0].Cells.Add(new TableCell());
                grdReceivePending.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdReceivePending.Rows[0].Cells[0].Text = "No Records Found";

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void LoadRecievedDTr()
        {
            try
            {
                clsDTrRepairActivity objDeliverpending = new clsDTrRepairActivity();

                DataTable dt = new DataTable();

                dt = objDeliverpending.LoadRecievedTransformers(txtRepairMasterId.Text);
                grdRecievedDTr.DataSource = dt;
                grdRecievedDTr.DataBind();
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                lblMessage.Text = clsException.ErrorMsg();
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

        public bool ValidateForm()
        {
            bool bValidate = false;
            string selecteditem = string.Empty;
            string txtPeriod = string.Empty;
            string State = string.Empty;
            try
            {
                if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    if ( cmbStore.SelectedValue == "0")
                    {
                        ShowMsgBox("Please Select the Store");
                        cmbStore.Focus();
                        return bValidate;
                    }
                }
                else
                {
                    if (cmbStore.SelectedIndex == 0 || cmbStore.SelectedValue == "--SELECT--")
                    {
                        ShowMsgBox("Please Select the Store");
                        cmbStore.Focus();
                        return bValidate;
                    }
                }
            
                if (cmbVerifiedBy.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Verified By");
                    cmbVerifiedBy.Focus();
                    return bValidate;
                }

                if (txtChallenNo.Text.Length == 0)
                {
                    ShowMsgBox("Please Enter the Deliver Challen Number");
                    txtChallenNo.Focus();
                    return bValidate;
                }

                foreach (GridViewRow row in grdReceivePending.Rows)
                {
                    State = ((Label)row.FindControl("lblInsResult")).Text;
                    selecteditem = ((DropDownList)row.FindControl("cmbGuarantyType")).SelectedValue;
                    txtPeriod = ((TextBox)row.FindControl("txtTCWarrenty")).Text;
                    if (((CheckBox)row.FindControl("chkSelect")).Checked == true)
                    {
                        if (State != "SCRAP")
                        {
                            //txtWarrenty

                            if (selecteditem == "--Select--")
                            {
                                ShowMsgBox("Please Select GUARENTEE TYPE");
                                ((DropDownList)row.FindControl("cmbGuarantyType")).Focus();
                                return bValidate;
                            }
                            if (txtPeriod == "" || txtPeriod == null)
                            {
                                if (((TextBox)row.FindControl("txtTCWarrenty")).Enabled == true)
                                {
                                    ShowMsgBox("Please Enter warranty Period");
                                    ((TextBox)row.FindControl("txtTCWarrenty")).Focus();
                                    return bValidate;
                                }

                            }
                        }
                    }
                }

                //int index = grdReceivePending.SelectedIndex;
                //GridViewRow row = grdReceivePending.Rows[index];

                //DropDownList ddlEmployee = grdReceivePending.Rows(< index >).FindControls("ddlEmployee") as DropDownList;
                //DropDownList ddl = (DropDownList)grdReceivePending.Rows[e.RowIndex].FindControl("YourDropDownList");

                //string selecteditem = ddl.SelectedValue.ToString();



                //if (txtRIDate.Text.Length == 0)
                //{
                //    ShowMsgBox("Please Select the RI Date");
                //    txtRIDate.Focus();
                //    return bValidate;
                //}
                //string sResult = Genaral.DateComparision(txtRIDate.Text, "", true, false);
                //if (sResult == "1")
                //{
                //    ShowMsgBox("RI Date should be Less than Current Date");
                //    return bValidate;
                //}
                if (txtDeliverdate.Text.Length == 0)
                {
                    ShowMsgBox("Please Select the Deliver Date");
                    txtDeliverdate.Focus();
                    return bValidate;
                }
                string sResult = Genaral.DateComparision(txtDeliverdate.Text, "", true, false);
                if (sResult == "1")
                {
                    ShowMsgBox("Deliver Date should be Less than Current Date");
                    return bValidate;
                }

                if (txtRVNo.Text.Trim().Length == 0)
                {
                    ShowMsgBox("Enter Valid RV No");
                    txtRVNo.Focus();
                    return bValidate;
                }
                if (txtRVDate.Text.Trim().Length == 0)
                {
                    ShowMsgBox("Enter Valid RV Date");
                    txtRVDate.Focus();
                    return bValidate;
                }
                sResult = Genaral.DateComparision(txtRVDate.Text, "", true, false);
                if (sResult == "1")
                {
                    ShowMsgBox("RV Date should be Less than Current Date");
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
        void Reset()
        {
            try
            {
                //cmbStore.SelectedIndex = 0;
                txtDeliverdate.Text = string.Empty;
                txtChallenNo.Text = string.Empty;
                cmbVerifiedBy.SelectedIndex = 0;
                txtRVNo.Text = string.Empty;
                txtRVDate.Text = string.Empty;
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
                cmbStore.SelectedValue = objSession.OfficeCode;
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

                objApproval.sFormName = "TCRepairIssue";
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


        public void GetRepairRecieveDetails(string sRepairDetailsId)
        {
            try
            {
                clsDTrRepairActivity objRepair = new clsDTrRepairActivity();

                objRepair.sRepairDetailsId = sRepairDetailsId;
                objRepair.GetRepairRecieveDetails(objRepair);

                txtDeliverdate.Text = objRepair.sDeliverDate;
                txtChallenNo.Text = objRepair.sDeliverChallenNo;
                cmbVerifiedBy.SelectedValue = objRepair.sVerifiedby;

                txtRVNo.Text = objRepair.sRVNo;
                txtRVDate.Text = objRepair.sRVDate;

                txtRepairMasterId.Text = objRepair.GetRepairDetailsId(sRepairDetailsId);

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdReceivePending_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //string sTcStatus = ((Label)e.Row.FindControl("lblTCStatus")).Text;
                    string sTcCode = ((Label)e.Row.FindControl("lblTcCode")).Text;

                    DataTable dt = (DataTable)ViewState["RecvPending"];
                    if (dt != null)
                    {
                        DataRow[] dtrow = dt.Select("TC_CODE like '%" + sTcCode + "%'");
                        LinkButton lnkDnld = ((LinkButton)e.Row.FindControl("lnkDwnld"));
                        LinkButton lnkNodnld = ((LinkButton)e.Row.FindControl("lnkNodownload"));
                        if (dtrow[0]["IND_DOC"].ToString() == null || dtrow[0]["IND_DOC"].ToString() == "")
                        {
                            lnkDnld.Visible = false;
                            lnkNodnld.Visible = true;
                            lnkNodnld.CssClass = "blockpointer";


                        }
                        else
                        {
                            lnkDnld.Enabled = true;
                            lnkNodnld.Visible = false;
                            lnkDnld.CssClass = "handPointer";
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

        protected void grdReceivePending_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Download")
                {

                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string sTcCode = ((Label)row.FindControl("lblTcCode")).Text;
                    download(sTcCode);

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            finally
            {
                Response.End();
            }

        }


        private void download(string sTcCode)
        {
            try
            {
                DataTable dt = (DataTable)ViewState["RecvPending"];
                DataRow[] dtrow = dt.Select("TC_CODE like '%" + sTcCode + "%'");
                Byte[] bytes = (Byte[])dtrow[0]["IND_DOC"];


                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "image/png";

                Response.AppendHeader("Content-Disposition", "attachment; filename=" + dtrow[0]["TC_CODE"].ToString() + ".png");

                Response.BinaryWrite(bytes);
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }


        }
    }
}