using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using IIITS.DTLMS.BL.TCRepair;
using System.Data;
using System.Collections;

namespace IIITS.DTLMS.TCRepair
{
    public partial class TCRepairIssue : System.Web.UI.Page
    {
        string strFormCode = "TCRepairIssue";
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
                lblMessage.Text = string.Empty;
                Form.DefaultButton = cmdSave.UniqueID;
                txtIssueDate.Attributes.Add("readonly", "readonly");
                txtPODate.Attributes.Add("readonly", "readonly");
                txtInvoiceDate.Attributes.Add("readonly", "readonly");

                CalendarExtender2.EndDate = System.DateTime.Now;
                CalendarExtender_txtPODate.EndDate = System.DateTime.Now;
                CalendarExtender1.EndDate = System.DateTime.Now;

                if (!IsPostBack)
                {
                    if (Request.QueryString["StoreId"] != null && Request.QueryString["StoreId"].ToString() != "")
                    {
                        //DisableCopy();
                        //Session["TcId"]
                        if (Session["TcId"] != null && Session["TcId"].ToString() != "")
                        {
                            txtSelectedTcId.Text = Session["TcId"].ToString();
                            Session["TcId"] = null;
                        }

                        txtStoreId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["StoreId"]));

                        GenerateInvoiceNo();
                        txtInvoiceDate.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
                        CalendarExtender1.EndDate = System.DateTime.Now.AddDays(0);
                        CalendarExtender_txtPODate.EndDate = System.DateTime.Now.AddDays(0);
                        LoadFaultTc();
                    }

                    //From DTR Tracker
                    if (Request.QueryString["TransId"] != null && Request.QueryString["TransId"].ToString() != "")
                    {
                        txtRepairMasterId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["TransId"]));
                        GetRepairSentDetails();
                        ddlType_SelectedIndexChanged(sender, e);
                        cmbRepairer.SelectedValue = hdfRepairId.Value;
                        cmbRepairer_SelectedIndexChanged(sender, e);
                        LoadRepairSentDTR();
                    }
                
                    string strQry = string.Empty;
                    //strQry = "Title=Search and Select DTC Failure Details&";
                    //strQry += "Query=select TC_CODE,TC_SLNO FROM TBLTCMASTER WHERE TC_STATUS=3 AND  TC_CURRENT_LOCATION=1 AND ";
                    //strQry += " TC_LOCATION_ID LIKE '" + objSession.OfficeCode + "%' AND  {0} like %{1}% order by TC_CODE&";
                    //strQry += "DBColName=TC_CODE~TC_SLNO&";
                    //strQry += "ColDisplayName=DTr Code~DTr SlNo&";
                    if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        strQry = "Title=Search and Select DTC Failure Details&";
                        strQry += "Query=select \"TC_CODE\",\"TC_SLNO\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" NOT IN ";
                        strQry += "(SELECT \"RSD_TC_CODE\" from \"TBLREPAIRSENTDETAILS\" where \"RSD_DELIVARY_DATE\" is NULL ) AND \"TC_STATUS\"=3 AND  \"TC_CURRENT_LOCATION\"=1 AND ";
                        strQry += " CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + txtStoreId.Text + "' AND  {0} like %{1}% order by \"TC_CODE\" &";
                        strQry += "DBColName= CAST(\"TC_CODE\" AS TEXT)~CAST(\"TC_SLNO\" AS TEXT) &";
                        strQry += "ColDisplayName=DTr Code~DTr SlNo&";

                    }
                    else
                    {
                        strQry = "Title=Search and Select DTC Failure Details&";
                        strQry += "Query=select \"TC_CODE\",\"TC_SLNO\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" NOT IN ";
                        strQry += "(SELECT \"RSD_TC_CODE\" from \"TBLREPAIRSENTDETAILS\" where \"RSD_DELIVARY_DATE\" is NULL ) AND \"TC_STATUS\"=3 AND  \"TC_CURRENT_LOCATION\"=1 AND ";
                        strQry += " CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + objSession.sStoreID + "' AND  {0} like %{1}% order by \"TC_CODE\" &";
                        strQry += "DBColName= CAST(\"TC_CODE\" AS TEXT)~CAST(\"TC_SLNO\" AS TEXT) &";
                        strQry += "ColDisplayName=DTr Code~DTr SlNo&";
                    }
                    strQry = strQry.Replace("'", @"\'");

                    cmdSearchTC.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtTcCode.ClientID + "&btn=" + cmdSearchTC.ClientID + "',520,520," + txtTcCode.ClientID + ")");
                    if (objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        strQry = "Title=Search and Select Already Entered Reference No&";
                        strQry += "Query=SELECT UPPER(\"RSM_PO_NO\")RSM_PO_NO FROM \"TBLREPAIRSENTMASTER\" WHERE CAST(\"RSM_DIV_CODE\" AS TEXT) = '" + txtStoreId.Text + "' AND {0} like %{1}% &";
                        strQry += "DBColName=CAST(\"RSM_PO_NO\" AS TEXT)&";
                        strQry += "ColDisplayName=Repairer Reference No&";
                    }
                    else
                    {
                        strQry = "Title=Search and Select Already Entered Reference No&";
                        strQry += "Query=SELECT UPPER(\"RSM_PO_NO\")RSM_PO_NO FROM \"TBLREPAIRSENTMASTER\" WHERE CAST(\"RSM_DIV_CODE\" AS TEXT) = '" + objSession.OfficeCode + "' AND {0} like %{1}% &";
                        strQry += "DBColName=CAST(\"RSM_PO_NO\" AS TEXT)&";
                        strQry += "ColDisplayName=Repairer Reference No&";
                    }
                    strQry = strQry.Replace("'", @"\'");

                    cmdSearchPO.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtPonum.ClientID + "&btn=" + cmdSearchPO.ClientID + "',520,520," + txtPonum.ClientID + ")");

                    txtInvoiceDate.Attributes.Add("onblur", "return ValidateDate(" + txtInvoiceDate.ClientID + ");");
                    txtIssueDate.Attributes.Add("onblur", "return ValidateDate(" + txtIssueDate.ClientID + ");");
                    txtPODate.Attributes.Add("onblur", "return ValidateDate(" + txtPODate.ClientID + ");");
                    //txtManualInvoiceNo.Attributes.Add("onblur", "return ValidateDate(" + txtPODate.ClientID + ");");
                    CheckAccessRights("2");
                    //WorkFlow / Approval
                    WorkFlowConfig();
                }

                if (objSession.RoleId == "2")
                {
                    DTrCODE.Visible = false;
                    MAKE.Visible = false;
                }
                else
                {
                    DTrCODE.Visible = true;
                    MAKE.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdSearchTC_Click(object sender, EventArgs e)
        {
            try
            {
                clsDtcMaster objDtcMaster = new clsDtcMaster();
                objDtcMaster.sTcCode = txtTcCode.Text;

                objDtcMaster.GetTCDetails(objDtcMaster);

                txtMake.Text = objDtcMaster.sTCMakeName;

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

                txtIssueDate.Attributes.Add("onmousedown", "return noCopyMouse(event);");
                txtInvoiceDate.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                txtIssueDate.Attributes.Add("onkeydown", "return noCopyKey(event);");
                txtInvoiceDate.Attributes.Add("onkeydown", "return noCopyKey(event);");

               cmbGuarantyType.Attributes.Add("onmousedown", "return noCopyMouse(event);");

                cmbRepairer.Attributes.Add("onkeydown", "return noCopyKey(event);");
                ddlType.Attributes.Add("onkeydown", "return noCopyKey(event);");
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        protected void cmbRepairer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbRepairer.SelectedIndex > 0)
                {
                    if (ddlType.SelectedValue == "2")
                    {
                        clsTransRepairer objRepair = new clsTransRepairer();
                        objRepair.RepairerId = cmbRepairer.SelectedValue;

                        objRepair.GetRepairerDetails(objRepair);

                        txtAddress.Text = objRepair.RegisterAddress;
                        txtName.Text = objRepair.RepairerName;
                        txtPhone.Text = objRepair.RepairerPhoneNo;
                    }
                    else
                    {
                        clsTransSupplier objSupplier = new clsTransSupplier();
                        objSupplier.SupplierId = cmbRepairer.SelectedValue;

                        objSupplier.GetSupplierDetails(objSupplier);

                        txtAddress.Text = objSupplier.RegisterAddress;
                        txtName.Text = objSupplier.SupplierName;
                        txtPhone.Text = objSupplier.SupplierPhoneNo;
                    }
                }
                else
                {
                    txtAddress.Text = string.Empty;
                    txtPhone.Text = string.Empty;
                    txtName.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadFaultTc()
        {
            try
            {
                DataTable dt = new DataTable();
                clsDTrRepairActivity objRepair = new clsDTrRepairActivity();
                if (txtSelectedTcId.Text != "")
                {
                    txtSelectedTcId.Text = txtSelectedTcId.Text.Replace("~", ",");
                    if (!txtSelectedTcId.Text.StartsWith(","))
                    {
                        txtSelectedTcId.Text = "," + txtSelectedTcId.Text;
                    }
                    if (!txtSelectedTcId.Text.EndsWith(","))
                    {
                        txtSelectedTcId.Text = txtSelectedTcId.Text + ",";
                    }

                    txtSelectedTcId.Text = txtSelectedTcId.Text.Substring(1, txtSelectedTcId.Text.Length - 1);
                    txtSelectedTcId.Text = txtSelectedTcId.Text.Substring(0, txtSelectedTcId.Text.Length - 1);
                    objRepair.sOfficeCode = objSession.OfficeCode;
                    objRepair.sTcId = txtSelectedTcId.Text;
                    objRepair.UserId = objSession.UserId;
                }
                if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    objRepair.sStoreId = txtStoreId.Text;
                }
                dt = objRepair.LoadFaultTCsearch(objRepair);
                grdFaultTC.DataSource = dt;
                ViewState["FaultTC"] = dt;
                grdFaultTC.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadRepairSentDTR()
        {
            try
            {
                DataTable dt = new DataTable();
                clsDTrRepairActivity objRepair = new clsDTrRepairActivity();

                dt = objRepair.LoadRepairSentDTR(txtRepairMasterId.Text);
                grdFaultTC.DataSource = dt;
                ViewState["FaultTC"] = dt;
                grdFaultTC.DataBind();
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
                clsDTrRepairActivity objTcRepair = new clsDTrRepairActivity();
                string[] Arr = new string[2];

                //Check AccessRights
                bool bAccResult = CheckAccessRights("2");
                if (bAccResult == false)
                {
                    return;
                }

                if (ValidateForm() == true)
                {
                    objTcRepair.sSupRepId = cmbRepairer.SelectedValue;
                    objTcRepair.sInvoiceDate = txtInvoiceDate.Text;
                    objTcRepair.sInvoiceNo = txtInvoiceNo.Text;
                    objTcRepair.sManualInvoiceNo = txtManualInvoiceNo.Text;
                    objTcRepair.sIssueDate = txtIssueDate.Text;
                    objTcRepair.sPurchaseDate = txtPODate.Text;
                    objTcRepair.sPurchaseOrderNo = txtPONo.Text;
                    objTcRepair.sCrby = objSession.UserId;
                    objTcRepair.sStoreId = txtStoreId.Text;
                    objTcRepair.sOfficeCode = objSession.OfficeCode;
                    objTcRepair.sGuarantyType = cmbGuarantyType.SelectedValue;
                    objTcRepair.sOldPONo = txtPonum.Text;
                    objTcRepair.sPORemarks = txtRemarks.Text;

                    objTcRepair.sType = ddlType.SelectedValue;


                    objTcRepair.newdivcode = cmbRepairer.SelectedItem.Text;

                    //if (ddlType.SelectedValue == "Repairer")
                    //{
                    //    objTcRepair.sType = "2";
                    //}
                    //else if (ddlType.SelectedValue == "Supplier")
                    //{
                    //    objTcRepair.sType = "1";
                    //}


                    if (txtActiontype.Text == "A" || txtActiontype.Text == "R")
                    {
                        if (hdfWFDataId.Value != "0")
                        {
                            ApproveRejectAction();

                            if (objSession.sTransactionLog == "1")
                            {
                                Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (TCRepairerIssue) Repair ");
                            }

                            return;
                        }
                    }

                    //To check Selected Transformers Already Sent for Supplier/Repair and Waiting For Approval
                    clsApproval objApproval = new clsApproval();
                    //string sResult = objApproval.GetDataReferenceId("30");

                    //if (!sResult.StartsWith(","))
                    //{
                    //    sResult = "," + sResult;
                    //}
                    //if (!sResult.EndsWith(","))
                    //{
                    //    sResult = sResult + ",";
                    //}


                    int i = 0;
                    string[] strQryVallist = new string[grdFaultTC.Rows.Count];
                    bool bDataExist = false;
                    foreach (GridViewRow row in grdFaultTC.Rows)
                    {
                        strQryVallist[i] = ((Label)row.FindControl("lblTCCode")).Text.Trim() + "~" + ((Label)row.FindControl("lblGuarantyType")).Text.Trim();
                        i++;
                        objTcRepair.sQty = Convert.ToString(grdFaultTC.Rows.Count);
                        string sTCCode = ((Label)row.FindControl("lblTCCode")).Text.Trim();

                        //if (sResult.Contains("," + sTCCode + ","))
                        //{
                        //    ShowMsgBox("Selected DTr " + sTCCode + "Already sent for Supplier/Repairer, Waiting for Approval");
                        //    return;
                        //}


                        bDataExist = true;
                    }

                    if (bDataExist == false)
                    {
                        ShowMsgBox("No Transformer Exists to Issue for Repairer/Supplier");
                        return;
                    }


                    //Workflow
                    WorkFlowObjects(objTcRepair);



                    Arr = objTcRepair.SaveRepairIssueDetails(strQryVallist, objTcRepair);


                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " (TCRepairerIssue) Repair ");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        ShowMsgBox(Arr[0].ToString());
                        string strParam = "id=RepairGatepass&InvoiceId=" + txtInvoiceNo.Text +"&TransId=" + txtRepairMasterId.Text ;
                        RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");

                        cmdSave.Enabled = false;
                        cmdGatePass.Enabled = false;
                        return;
                    }


                    if (Arr[1].ToString() == "0")
                    {
                        //ShowMsgBox(Arr[0].ToString());

                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), UniqueID, "alert('Transformers Issued Sucessfully to Repairer/Supplier'); location.href='FaultTCSearch.aspx';", true);
                        Reset();
                        txtInvoiceNo.Text = string.Empty;
                        txtTcCode.Text = string.Empty;
                        txtMake.Text = string.Empty;
                        grdFaultTC.DataSource = null;
                        grdFaultTC.DataBind();
                        ViewState["FaultTC"] = null;
                        txtSelectedTcId.Text = string.Empty;
                        cmdGatePass.Enabled = true;
                        cmdSave.Enabled = false;
                        return;
                    }
                    else if (Arr[1].ToString() == "2")
                    {
                        ShowMsgBox(Arr[0].ToString());
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

        public void AddTCtoGrid(string sTcCode)
        {
            try
            {
                clsDTrRepairActivity objTCRepair = new clsDTrRepairActivity();

                if (ValidateGridValue(sTcCode) == true)
                {
                    objTCRepair.sTcCode = sTcCode;
                    objTCRepair.AddFaultTCDetails(objTCRepair);

                    if (ViewState["FaultTC"] != null)
                    {
                        DataTable dtFaultTc = (DataTable)ViewState["FaultTC"];
                        DataRow drow;
                        if (objTCRepair.sTcId != null)
                        {
                            if (dtFaultTc.Rows.Count > 0)
                            {
                                drow = dtFaultTc.NewRow();

                                drow["TC_ID"] = objTCRepair.sTcId;
                                drow["TC_CODE"] = objTCRepair.sTcCode;
                                drow["TC_SLNO"] = objTCRepair.sTcSlno;
                                drow["TM_NAME"] = objTCRepair.sMakeName;
                                drow["TC_CAPACITY"] = objTCRepair.sCapacity;
                                drow["TC_MANF_DATE"] = objTCRepair.sManfDate;
                                drow["TC_PURCHASE_DATE"] = objTCRepair.sPurchaseDate;
                                drow["TC_WARANTY_PERIOD"] = objTCRepair.sWarrantyPeriod;
                                drow["TS_NAME"] = objTCRepair.sSupplierName;
                                drow["TC_GUARANTY_TYPE"] = objTCRepair.sGuarantyType;

                                dtFaultTc.Rows.Add(drow);
                                grdFaultTC.DataSource = dtFaultTc;
                                grdFaultTC.DataBind();
                                ViewState["FaultTC"] = dtFaultTc;
                            }
                        }

                        ShowMsgBox("TC is not in Store or Good Condition");
                        txtTcCode.Text = "";
                    }
                    else
                    {
                        DataTable dtFaultTc = new DataTable();
                        DataRow drow;

                        dtFaultTc.Columns.Add(new DataColumn("TC_ID"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_CODE"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_SLNO"));
                        dtFaultTc.Columns.Add(new DataColumn("TM_NAME"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_CAPACITY"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_MANF_DATE"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_PURCHASE_DATE"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_WARANTY_PERIOD"));
                        dtFaultTc.Columns.Add(new DataColumn("TS_NAME"));
                        dtFaultTc.Columns.Add(new DataColumn("TC_GUARANTY_TYPE"));

                        drow = dtFaultTc.NewRow();

                        drow["TC_ID"] = objTCRepair.sTcId;
                        drow["TC_CODE"] = objTCRepair.sTcCode;
                        drow["TC_SLNO"] = objTCRepair.sTcSlno;
                        drow["TM_NAME"] = objTCRepair.sMakeName;
                        drow["TC_CAPACITY"] = objTCRepair.sCapacity;
                        drow["TC_MANF_DATE"] = objTCRepair.sManfDate;
                        drow["TC_PURCHASE_DATE"] = objTCRepair.sPurchaseDate;
                        drow["TC_WARANTY_PERIOD"] = objTCRepair.sWarrantyPeriod;
                        drow["TS_NAME"] = objTCRepair.sSupplierName;
                        drow["TC_GUARANTY_TYPE"] = objTCRepair.sGuarantyType;

                        dtFaultTc.Rows.Add(drow);
                        grdFaultTC.DataSource = dtFaultTc;
                        grdFaultTC.DataBind();
                        ViewState["FaultTC"] = dtFaultTc;

                    }
                }
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

                foreach (GridViewRow row in grdFaultTC.Rows)
                {
                    objArrlist.Add(((Label)row.FindControl("lblTCCode")).Text.Trim());
                }

                if (objArrlist.Contains(sTcCode))
                {
                    ShowMsgBox("Transformer Already Added");
                    DataTable dtFaultTc = (DataTable)ViewState["FaultTC"];
                    grdFaultTC.DataSource = dtFaultTc;
                    grdFaultTC.DataBind();
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

        protected void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTcCode.Text.Trim() == "")
                {
                    ShowMsgBox("Select Transformer Code");
                    return;
                }
                AddTCtoGrid(txtTcCode.Text);
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
                //txtInvoiceNo.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void Reset()
        {
            try
            {
                if (cmbRepairer.SelectedIndex > 0)
                {
                    cmbRepairer.SelectedIndex = 0;
                }
                txtAddress.Text = string.Empty;
                txtInvoiceDate.Text = string.Empty;
                //txtInvoiceNo.Text = string.Empty;
                txtIssueDate.Text = string.Empty;
                txtName.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtPODate.Text = string.Empty;
                txtPONo.Text = string.Empty;
                txtTcCode.Text = string.Empty;
                txtMake.Text = string.Empty;
                hdfTccode.Value = string.Empty;
                cmbGuarantyType.SelectedIndex = 0;
                ddlType.SelectedIndex = 0;

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
            try
            {

                if (cmbGuarantyType.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Guaranty Type");
                    cmbGuarantyType.Focus();
                    return bValidate;
                }
                if (ddlType.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Type(Repairer/Supplier)");
                    ddlType.Focus();
                    return bValidate;
                }

                if (cmbRepairer.SelectedIndex == 0)
                {
                    ShowMsgBox("Select Repairer / Supplier");
                    cmbRepairer.Focus();
                    return bValidate;
                }
                if (txtIssueDate.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Issue Date");
                    txtIssueDate.Focus();
                    return bValidate;
                }
                //string sResult = Genaral.DateComparision(txtIssueDate.Text, "", true, false);
                //if (sResult == "1")
                //{
                //    ShowMsgBox("Issue Date should be Less than Current Date");
                //    return bValidate;
                //}
                if (txtPONo.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Purchase Order No");
                    txtPONo.Focus();
                    return bValidate;
                }
                if (txtPODate.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Purchase Order Date");
                    txtPODate.Focus();
                    return bValidate;
                }
                //sResult = Genaral.DateComparision(txtWODate.Text, "", true, false);
                //if (sResult == "1")
                //{
                //    ShowMsgBox("Work Order Date should be Less than Current Date");
                //    return bValidate;
                //}
                if (txtInvoiceNo.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Invoice No.");
                    txtInvoiceNo.Focus();
                    return bValidate;
                }
                if (txtInvoiceDate.Text.Trim() == "")
                {
                    ShowMsgBox("Enter Valid Invoice Date");
                    txtInvoiceDate.Focus();
                    return bValidate;
                }
                //sResult = Genaral.DateComparision(txtInvoiceDate.Text, "", true, false);
                //if (sResult == "1")
                //{
                //    ShowMsgBox("Invoice Date should be Less than Current Date");
                //    return bValidate;
                //}
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


        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlType.SelectedIndex > 0)
                {

                    if (ddlType.SelectedValue == "2")
                    {
                        string stroffCode = string.Empty;
                        string stroffCode1 = string.Empty;
                        stroffCode = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId);
                        stroffCode1 = stroffCode;
                        if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                           string stroffCodes=txtStoreId.Text;
                           string  stroffCodess = clsStoreOffice.GetZone_Circle_Div_Offcode(txtStoreId.Text, objSession.RoleId);
                         //   string stroffCodess = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId);

                            Genaral.Load_Combo("SELECT  \"TR_ID\",\"TR_NAME\"||'~'||(SELECT \"DIV_NAME\" from \"TBLDIVISION\" WHERE  \"DIV_CODE\"=\"TRO_OFF_CODE\") as \"TR_NAME\"  FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE  \"TR_ID\"=\"TRO_TR_ID\" and \"TR_STATUS\" ='A' AND \"TR_ID\" NOT IN (SELECT \"TR_ID\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TR_BLACK_LISTED\"=1 AND \"TR_BLACKED_UPTO\" >= NOW()) AND CAST(\"TRO_OFF_CODE\" AS TEXT) LIKE '" + stroffCodess.Substring(0, 3) + "%' ORDER BY \"TR_NAME\"", "--Select--", cmbRepairer);
                            lblSuppRep.Text = "Repairer";
                        }
                        else
                        {
                            Genaral.Load_Combo("SELECT  \"TR_ID\",\"TR_NAME\"||'~'||(SELECT \"DIV_NAME\" from \"TBLDIVISION\" WHERE  \"DIV_CODE\"=\"TRO_OFF_CODE\") as \"TR_NAME\"  FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE  \"TR_ID\"=\"TRO_TR_ID\" and \"TR_STATUS\" ='A' AND \"TR_ID\" NOT IN (SELECT \"TR_ID\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TR_BLACK_LISTED\"=1 AND \"TR_BLACKED_UPTO\" >= NOW()) AND CAST(\"TRO_OFF_CODE\" AS TEXT) LIKE '" + stroffCode.Substring(0, 3) + "%' ORDER BY \"TR_NAME\"", "--Select--", cmbRepairer);
                            lblSuppRep.Text = "Repairer";
                        }
                        //Genaral.Load_Combo("SELECT  \"TR_ID\",\"TR_NAME\"||'~'||(SELECT \"DIV_NAME\" from \"TBLDIVISION\" WHERE  \"DIV_CODE\"=\"TRO_OFF_CODE\") as \"TR_NAME\"  FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE  \"TR_ID\"=\"TRO_TR_ID\" and \"TR_STATUS\" ='A' AND \"TR_ID\" NOT IN (SELECT \"TR_ID\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\"  WHERE \"TR_ID\"=\"TRO_TR_ID\" AND \"TR_BLACK_LISTED\"=1 AND \"TR_BLACKED_UPTO\" >= NOW()) AND CAST(\"TRO_OFF_CODE\" AS TEXT) LIKE '" + stroffCode.Substring(0, 2) + "%' ORDER BY \"TR_NAME\"", "--Select--", cmbRepairer);
                        //lblSuppRep.Text = "Repairer";
                    }
                    else if (ddlType.SelectedValue == "1")
                    {
                        Genaral.Load_Combo("SELECT \"TS_ID\",\"TS_NAME\"  FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_STATUS\" ='A' AND \"TS_ID\" NOT IN (SELECT \"TS_ID\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TS_BLACK_LISTED\" =1 AND \"TS_BLACKED_UPTO\" >= NOW()) ORDER BY \"TS_NAME\" ", "--Select--", cmbRepairer);
                        lblSuppRep.Text = "Supplier";
                    }
                }
                else
                {
                    cmbRepairer.Items.Clear();
                    txtAddress.Text = string.Empty;
                    txtPhone.Text = string.Empty;
                    txtName.Text = string.Empty;
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

                objApproval.sFormName = "FaultTCSearch";
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

        protected void grdFaultTC_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
               
                  if (e.CommandName == "Remove")
                  {
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int iRowIndex = row.RowIndex;

                    DataTable dt = (DataTable)ViewState["FaultTC"];
                    dt.Rows[iRowIndex].Delete();
                    if (dt.Rows.Count == 0)
                    {
                        ViewState["FaultTC"] = null;
                    }
                    else
                    {
                        ViewState["FaultTC"] = dt;
                    }

                    grdFaultTC.DataSource = dt;
                    grdFaultTC.DataBind();
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GenerateInvoiceNo()
        {
            try
            {
                clsInvoice objInvoice = new clsInvoice();
                string sRoletype = objSession.sRoleType;
                if(objSession.RoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                   // txtInvoiceNo.Text = objInvoice.GenerateInvoiceNo(objSession.OfficeCode, sRoletype);
                   // txtInvoiceNo.Text = objInvoice.GenerateInvoiceNoSA(txtStoreId.Text, sRoletype,objSession.RoleId);
                    txtInvoiceNo.Text = objInvoice.GenerateInvoiceNoSARSM(objSession.OfficeCode, sRoletype, objSession.RoleId);
                }
                else
                {
                    txtInvoiceNo.Text = objInvoice.GenerateInvoiceNoRSM(objSession.OfficeCode, sRoletype);
                }
              //  txtInvoiceNo.Text = objInvoice.GenerateInvoiceNo(objSession.OfficeCode,sRoletype );
                txtInvoiceNo.ReadOnly = true;
                hdfInvoiceNo.Value = txtInvoiceNo.Text;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        bool ValidateGatePass()
        {
            bool bValidate = false;

            try
            {

                if (txtVehicleNo.Text.Length == 0)
                {
                    txtVehicleNo.Focus();
                    ShowMsgBox("Enter Vehicle No");
                    return bValidate;
                }
                if (txtChallen.Text.Length == 0)
                {
                    txtChallen.Focus();
                    ShowMsgBox("Enter Challen Number");
                    return bValidate;
                }

                if (txtReciepient.Text.Trim().Length == 0)
                {
                    txtReciepient.Focus();
                    ShowMsgBox("Enter Reciepient Name");
                    return bValidate;
                }



                bValidate = true;
                return bValidate;
            }
            catch (Exception ex)
            {
                bValidate = false;
                return bValidate;
            }
        }

        protected void cmdGatePass_Click(object sender, EventArgs e)
        {
            try
            {
                clsInvoice objInvoice = new clsInvoice();
                string[] Arr = new string[2];
                if(txtActiontype.Text=="V")
                {
                    string strParam = "id=RepairGatepass&InvoiceId=" + txtInvoiceNo.Text + "&TransId=" + txtRepairMasterId.Text;
                    RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");
                     //LoadRepairSentDTR();
                    return;
                }
                if (ValidateGatePass() == true)
                {
                    objInvoice.sGatePassId = txtGpId.Text;
                    objInvoice.sVehicleNumber = txtVehicleNo.Text.Replace("'", "");
                    objInvoice.sReceiptientName = txtReciepient.Text.Replace("'", "");
                    objInvoice.sChallenNo = txtChallen.Text.Replace("'", "");
                    objInvoice.sCreatedBy = objSession.UserId;
                    //objInvoice.sTcCode = txtTCCode.Text.Replace("'", "");

                    if(ViewState["FaultTC"] !=null)
                    {
                        DataTable dt =(DataTable)ViewState["FaultTC"];                    
                        string sTCCode="";
                        foreach (GridViewRow row in grdFaultTC.Rows)
                        {
                            sTCCode += ((Label)row.FindControl("lblTCCode")).Text.Trim() + ",";                    
                            objInvoice.sIssueQty = Convert.ToString(grdFaultTC.Rows.Count);
                            
                            //if (sResult.Contains("," + sTCCode + ","))
                            //{
                            //    ShowMsgBox("Selected DTr " + sTCCode + "Already sent for Supplier/Repairer, Waiting for Approval");
                            //    return;
                            //}               
                        }
                        objInvoice.sTcCode = sTCCode;
                    }

                    objInvoice.sInvoiceNo = hdfInvoiceNo.Value.Replace("'", "");

                    Arr = objInvoice.SaveUpdateGatePassDetails(objInvoice);

                    if (Arr[0].ToString() == "0")
                    {
                        txtGpId.Text = objInvoice.sGatePassId;
                        string strParam = "id=RepairGatepass&InvoiceId=" + hdfInvoiceNo.Value + "&TransId="+ txtRepairMasterId.Text;
                        RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");

                        return;
                    }
                    if (Arr[0].ToString() == "1")
                    {
                        string strParam = "id=RepairGatepass&InvoiceId=" + txtInvoiceNo.Text + "&TransId=" + txtRepairMasterId.Text; 
                        RegisterStartupScript("PrintD", "<script>window.open('/Reports/ReportView.aspx?" + strParam + "','Print','addressbar=no, scrollbars =yes, resizable=yes')</script>");

                        return;
                    }

                    if (Arr[0].ToString() == "2")
                    {
                        ShowMsgBox(Arr[1]);
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

        public void GetRepairSentDetails()
        {
            try
            {
                clsDTrRepairActivity objRepair = new clsDTrRepairActivity();
                objRepair.sRepairMasterId = txtRepairMasterId.Text;
                objRepair.GetRepairSentDetails(objRepair);

                cmbGuarantyType.SelectedValue = objRepair.sGuarantyType;
                ddlType.SelectedValue = objRepair.sType;

                hdfRepairId.Value = objRepair.sSupRepId;

                txtIssueDate.Text = objRepair.sIssueDate;
                txtPONo.Text = objRepair.sPurchaseOrderNo;
                txtPODate.Text = objRepair.sPurchaseDate;
                txtInvoiceNo.Text = objRepair.sInvoiceNo;
                txtInvoiceDate.Text = objRepair.sInvoiceDate;
                txtManualInvoiceNo.Text = objRepair.sManualInvoiceNo;
                txtStoreId.Text = objRepair.sStoreId;
                txtPonum.Text = objRepair.sOldPONo;
                txtRemarks.Text = objRepair.sPORemarks;
                hdfInvoiceNo.Value = objRepair.sInvoiceNo;
                cmdSave.Enabled = false;
                cmdGatePass.Enabled = true;
         
                cmdReset.Enabled = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void WorkFlowObjects(clsDTrRepairActivity objDTRRepair)
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


                objDTRRepair.sFormName = "TCRepairIssue";
                objDTRRepair.sOfficeCode = objSession.OfficeCode;
                objDTRRepair.sClientIP = sClientIP;
                objDTRRepair.sWFObjectId = hdfWFOId.Value;
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
                    cmdSave.Text = "Approve";
                    pnlApproval.Enabled = false;
                    cmdSave.Enabled = true;
                }
                if (txtActiontype.Text == "R")
                {
                    cmdSave.Text = "Reject";
                    pnlApproval.Enabled = false;
                }
                if (txtActiontype.Text == "M")
                {
                    cmdSave.Text = "Modify and Approve";
                    pnlApproval.Enabled = true;
                }

                if (objSession.RoleId == "2" || objSession.RoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    dvComments.Style.Add("display", "block");

                    grdFaultTC.Columns[10].Visible = false;
                   // grdFaultTC.Columns.RemoveAt(10);    //Index is the index of the column you want to remove
                   // grdFaultTC.DataBind();

                   
                }
                //cmdReset.Enabled = false;

                if (hdfWFOAutoId.Value != "0")
                {
                    cmdSave.Text = "Save";

                }

                // Check for Creator of Form
                bool bResult = CheckFormCreatorLevel();
                if (bResult == true)
                {
                    cmdSave.Text = "Save";
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

                if (objSession.RoleId != "5")
                {
                    if (txtComment.Text.Trim() == "")
                    {
                        ShowMsgBox("Enter Comments/Remarks");
                        txtComment.Focus();
                        return;

                    }
                }


                objApproval.sCrby = objSession.UserId;
                objApproval.sOfficeCode = objSession.OfficeCode;
               // objApproval.sOfficeCode = clsStoreOffice.Getofficecode(objSession.OfficeCode);
                objApproval.sApproveComments = txtComment.Text.Trim();
                objApproval.sWFObjectId = hdfWFOId.Value;

                if (txtActiontype.Text == "A")
                {
                    objApproval.sApproveStatus = "1";
                }
                if (txtActiontype.Text == "R")
                {
                    objApproval.sApproveStatus = "3";
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


                bool bResult = objApproval.ApproveWFRequest(objApproval);
                if (bResult == true)
                {

                    if (objApproval.sApproveStatus == "1")
                    {
                        ShowMsgBox("Approved Successfully");

                        if (objSession.RoleId == "2")
                        {
                            clsRIApproval objRI = new clsRIApproval();
                            // objRI.SendSMStoSectionOfficer(txtDtrCode.Text, txtDecommId.Text, txtFailureId.Text);

                        }


                        cmdSave.Enabled = false;
                    }
                    else if (objApproval.sApproveStatus == "3")
                    {

                        ShowMsgBox("Rejected Successfully");
                        cmdSave.Enabled = false;
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
            try
            {
                if (Request.QueryString["ActionType"] != null && Request.QueryString["ActionType"].ToString() != "")
                {


                    if (Session["WFOId"] != null && Session["WFOId"].ToString() != "")
                    {
                        hdfWFDataId.Value = Session["WFDataId"].ToString();
                        hdfWFOId.Value = Convert.ToString(Session["WFOId"]);
                        hdfWFOAutoId.Value = Convert.ToString(Session["WFOAutoId"]);

                        Session["WFDataId"] = null;
                        Session["WFOId"] = null;
                        Session["WFOAutoId"] = null;
                    }
                    else if((hdfWFOId.Value.ToString() ?? "").Length == 0)
                    {
                        Response.Redirect("/Approval/ApprovalInbox.aspx", false);
                        return;
                    }

                    //if (hdfWFDataId.Value != "0")
                    //{
                    //    GetRIDetailsFromXML(hdfWFDataId.Value);
                    //}
                    SetControlText();
                    if (txtActiontype.Text == "V")
                    {
                        cmdSave.Text = "View";
                        dvComments.Style.Add("display", "none");
                        txtVehicleNo.Enabled = false;
                        txtChallen.Enabled = false;
                        txtReciepient.Enabled = false;
                    }
                }
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
                string sResult = objApproval.GetFormCreatorLevel("", objSession.RoleId, "TCRepairIssue");
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
        #endregion

        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["ActionType"] != null && Request.QueryString["ActionType"].ToString() != "")
                {
                    Response.Redirect("/Approval/ApprovalInbox.aspx", false);
                }
                else
                {
                    Response.Redirect("FaultTCSearch.aspx", false);
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdSearchPO_Click(object sender, EventArgs e)
        {
            try
            {
                string[] Arr = new string[2];
                clsDtcMaster objDtcMaster = new clsDtcMaster();
                //objDtcMaster.sTcCode = txtPonum.Text;

                Arr = objDtcMaster.GetPONo(txtPonum.Text);
                if (Arr[1] != "1")
                {
                    ShowMsgBox(Arr[0]);
                    txtPonum.Text = "";
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
