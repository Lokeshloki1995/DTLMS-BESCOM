using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.Collections;
using System.Configuration;

namespace IIITS.DTLMS.TCRepair
{
    public partial class FaultTCSearch : System.Web.UI.Page
    {
        string strFormCode = "FaultTCSearch";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }

                lblMessage.Text = string.Empty;
                objSession = (clsSession)Session["clsSession"];
                if (!IsPostBack)
                {

                    LoadComboFiled();
                    GetStoreId();
                    //Genaral.Load_Combo("SELECT TS_ID,TS_NAME FROM TBLTRANSSUPPLIER where TS_ID NOT IN (SELECT TS_ID FROM TBLTRANSSUPPLIER WHERE TS_BLACK_LISTED=1 AND TS_BLACKED_UPTO>=NOW())", "--Select--", cmbSupplier);
                 
                   CheckAccessRights("4");
                   //CheckAccessRights("1");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadComboFiled()
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TM_ID\",\"TM_NAME\" FROM \"TBLTRANSMAKES\" ORDER BY \"TM_NAME\"";

                int Division = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
                string sOfficeCode = string.Empty;
                if(objSession.sRoleType != "2")
                {
                    if (objSession.OfficeCode.Length > 1)
                    {
                        sOfficeCode = objSession.OfficeCode.Substring(0, Division);
                    }
                    else
                    {
                        sOfficeCode = objSession.OfficeCode;
                    }
                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\",\"TBLSTOREOFFCODE\" WHERE \"SM_ID\"=\"STO_SM_ID\" AND CAST(\"SM_STATUS\" AS TEXT)='A' AND CAST(\"STO_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' ORDER BY \"SM_NAME\"", "--Select--", cmbStore);
                }
                else
                {
                    sOfficeCode = objSession.OfficeCode;
                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE CAST(\"SM_STATUS\" AS TEXT)='A' AND CAST(\"SM_ID\" AS TEXT) LIKE '" + sOfficeCode + "%' ORDER BY \"SM_NAME\"", "--Select--", cmbStore);
                }
                
                
                Genaral.Load_Combo(strQry, "--Select--", cmbMake);
                Genaral.Load_Combo("SELECT \"MD_NAME\",\"MD_NAME\" from \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='C' ORDER BY \"MD_ORDER_BY\"", "--Select--", cmbCapacity);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFaultTc();
              
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
                clsDTrRepairActivity objTcFailure = new clsDTrRepairActivity();                

                if (cmbCapacity.SelectedIndex > 0)
                {
                    objTcFailure.sCapacity = cmbCapacity.SelectedValue;
                }
                if (cmbMake.SelectedIndex > 0)
                {
                    objTcFailure.sMakeId = cmbMake.SelectedValue;
                }
                if (cmbStore.SelectedIndex > 0)
                {
                    objTcFailure.sStoreId = cmbStore.SelectedValue;
                }
                //if (cmbSupplier.SelectedIndex > 0)
                //{
                //    objTcFailure.sSupplierId = cmbSupplier.SelectedValue;
                //}
                if (cmbGuarantyType.SelectedIndex > 0)
                {
                    objTcFailure.sGuarantyType = cmbGuarantyType.SelectedValue;
                }
                objTcFailure.sOfficeCode = objSession.OfficeCode;
                objTcFailure.UserId = objSession.UserId;
                objTcFailure.sroletype = objSession.sRoleType;

                dt = objTcFailure.LoadFaultTCsearch(objTcFailure);
                if (dt.Rows.Count > 0)
                {
                    ViewState["FaultTC"] = dt;
                    grdFaultTC.DataSource = SortDataTable(dt as DataTable, true);
                   
                    grdFaultTC.DataBind();
                    foreach (GridViewRow row in grdFaultTC.Rows)
                    {
                        Label lblstatus = (Label)row.FindControl("lblStatus");
                        if (lblstatus.Text == "Already Sent")
                        {
                            ((CheckBox)row.FindControl("chkSelect")).Enabled = false;
                        }
                    }
                    grdFaultTC.Visible = true;
                    cmdSend.Visible = true ;
                }
                else
                {
                    grdFaultTC.Visible = true;
                    cmdSend.Visible = false;
                    ViewState["FaultTC"] = dt;
                    grdFaultTC.DataSource = dt;  //sort datatable
                    grdFaultTC.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdFaultTC_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridViewSortExpression = e.SortExpression;
            int pageIndex = grdFaultTC.PageIndex;
            DataTable dt = (DataTable)ViewState["FaultTC"];
            string sortingDirection = string.Empty;

            if (dt.Rows.Count > 0)
            {
                grdFaultTC.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                grdFaultTC.DataSource = dt;
            }
            grdFaultTC.DataBind();
            grdFaultTC.PageIndex = pageIndex;
        }

        protected DataView SortDataTable(DataTable dataTable, bool isPageIndexChanging)
        {
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                if (GridViewSortExpression != string.Empty)
                {
                    if (isPageIndexChanging)
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GridViewSortDirection);
                        if (dataView.Sort.ToString() == "TC_CAPACITY ASC")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderBy(r => int.Parse(r.Field<String>("TC_CAPACITY")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["FaultTC"] = dataView.ToTable();
                        }
                        else if (dataView.Sort.ToString() == "TC_CAPACITY DESC")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderByDescending(r => int.Parse(r.Field<String>("TC_CAPACITY")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["FaultTC"] = dataView.ToTable();
                        }
                        else
                        {
                            ViewState["FaultTC"] = dataView.ToTable();
                        }


                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());
                        if (dataView.Sort.ToString() == "TC_CAPACITY ASC")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderBy(r => int.Parse(r.Field<String>("TC_CAPACITY")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["FaultTC"] = dataView.ToTable();
                        }
                        else if (dataView.Sort.ToString() == "TC_CAPACITY DESC")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderByDescending(r => int.Parse(r.Field<String>("TC_CAPACITY")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["FaultTC"] = dataView.ToTable();
                        }
                        else
                        {
                            ViewState["FaultTC"] = dataView.ToTable();
                        }

                    }


                }
                return dataView;
            }
            else
            {
                return new DataView();
            }

        }
        private string GridViewSortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }


        }
        private string GridViewSortExpression
        {
            get { return ViewState["SortExpression"] as string ?? string.Empty; }
            set { ViewState["SortExpression"] = value; }
        }

        private string GetSortDirection()
        {
            switch (GridViewSortDirection)
            {
                case "ASC":
                    GridViewSortDirection = "DESC";

                    break;
                case "DESC":
                    GridViewSortDirection = "ASC";

                    break;
            }


            return GridViewSortDirection;
        }


        protected void Export_ClickTcsearch(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //clsDTrRepairActivity objTcFailure = new clsDTrRepairActivity();

            //if (cmbCapacity.SelectedIndex > 0)
            //{
            //    objTcFailure.sCapacity = cmbCapacity.SelectedValue;
            //}
            //if (cmbMake.SelectedIndex > 0)
            //{
            //    objTcFailure.sMakeId = cmbMake.SelectedValue;
            //}
            //if (cmbStore.SelectedIndex > 0)
            //{
            //    objTcFailure.sStoreId = cmbStore.SelectedValue;
            //}

            //if (cmbGuarantyType.SelectedIndex > 0)
            //{
            //    objTcFailure.sGuarantyType = cmbGuarantyType.SelectedValue;
            //}
            //objTcFailure.sOfficeCode = objSession.OfficeCode;

            //dt = objTcFailure.LoadFaultTC(objTcFailure);
            DataTable dt = (DataTable)ViewState["FaultTC"];

            if (dt.Rows.Count > 0 || dt == null )
            {

                dt.Columns["TC_CODE"].ColumnName = "DTR CODE";
                dt.Columns["TC_SLNO"].ColumnName = "DTR SLNO";
                dt.Columns["TM_NAME"].ColumnName = "MAKE NAME";
                dt.Columns["TC_CAPACITY"].ColumnName = "CAPACITY(IN KVA)";
                dt.Columns["TC_MANF_DATE"].ColumnName = "MANF. DATE";
                dt.Columns["RCOUNT"].ColumnName = "SENT TO REPAIRER";
                dt.Columns["TC_GUARANTY_TYPE"].ColumnName = "GUARANTEE TYPE";
                dt.Columns["STATUS"].ColumnName = "STATUS";


                //dtLoadDetails.Columns["FEEDER NAME"].SetOrdinal(0);

                List<string> listtoRemove = new List<string> { "TC_ID", "TC_PURCHASE_DATE", "TC_WARANTY_PERIOD", "TS_NAME"};
                string filename = "FaultTCDetails" + DateTime.Now + ".xls";
                string pagetitle = "Fault TC Details";

                Genaral.getexcel(dt, listtoRemove, filename, pagetitle);
            }
            else
            {
                ShowMsgBox("No record found");
                
            }



        }

        protected void grdFaultTC_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Submit")
                {
                    GridViewRow row = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                    int rowindex = row.RowIndex;
                    Label lblTCId = (Label)grdFaultTC.Rows[rowindex].FindControl("lblTCId");
                }

                if (e.CommandName == "search")
                {
                    string sFilter = string.Empty;
                    DataView dv = new DataView();

                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    TextBox txtDtrCode = (TextBox)row.FindControl("txtDTRCode");
                    TextBox txtDtrSlNo = (TextBox)row.FindControl("txtSlNo");

                    clsDTrRepairActivity objTcRepair = new clsDTrRepairActivity();
                    if (txtDtrCode.Text != "")
                    {
                        objTcRepair.sTcCode = txtDtrCode.Text;
                    }
                    if(txtDtrSlNo.Text != "")
                    {
                        objTcRepair.sTcSlno = txtDtrSlNo.Text;
                    }

                    SaveCheckedValues();
                    objTcRepair.sOfficeCode = objSession.OfficeCode;
                    objTcRepair.sStoreId = cmbStore.SelectedValue;
                    //DataTable dt = objTcRepair.LoadFaultTC(objTcRepair);
                    DataTable dt = (DataTable)ViewState["FaultTC"];
                    dv = dt.DefaultView;
                    //sFilter = string.Format("convert(DME_EXISTING_DTR_CODE , 'System.String') Like '%{0}%' ",txttcCode.Text);
                    if (txtDtrCode.Text != "")
                    {
                        sFilter = string.Format("convert(TC_CODE , 'System.String') Like '%{0}%' ", txtDtrCode.Text.Replace("'", "'"));
                        //sFilter = "TC_CODE Like '%" + txtDtrCode.Text.Replace("'", "'") + "%' AND";
                    }
                    if (txtDtrSlNo.Text != "")
                    {
                        sFilter = string.Format("convert(TC_SLNO , 'System.String') Like '%{0}%' ", txtDtrSlNo.Text.Replace("'", "'"));
                        //sFilter += " TC_SLNO Like '%" + txtDtrSlNo.Text.Replace("'", "'") + "%' AND";
                    }

                    if (sFilter.Length > 0)
                    {
                        
                        grdFaultTC.PageIndex = 0;
                        dv.RowFilter = sFilter;
                        if (dv.Count > 0)
                        {
                            grdFaultTC.DataSource = dv;
                            ViewState["FaultTC"] = dv.ToTable();
                            grdFaultTC.DataBind();
                            foreach (GridViewRow rows in grdFaultTC.Rows)
                            {
                                Label lblstatus = (Label)rows.FindControl("lblStatus");
                                if (lblstatus.Text == "Already Sent")
                                {
                                    ((CheckBox)rows.FindControl("chkSelect")).Enabled = false;
                                }
                            }

                        }
                        else
                        {
                            ViewState["FaultTC"] = dv.ToTable();
                            ShowEmptyGrid();
                        }
                    }
                    else
                    {
                         dt = objTcRepair.LoadFaultTCsearch(objTcRepair);
                        grdFaultTC.DataSource = dt;
                        grdFaultTC.DataBind();
                        foreach (GridViewRow rows in grdFaultTC.Rows)
                        {
                            Label lblstatus = (Label)rows.FindControl("lblStatus");
                            if (lblstatus.Text == "Already Sent")
                            {
                                ((CheckBox)rows.FindControl("chkSelect")).Enabled = false;
                            }
                        }
                        ViewState["FaultTC"] = dt;

                    }
                    //if (dt.Rows.Count == 0)
                    //{
                    //    ShowEmptyGrid();
                    //    ViewState["FaultTC"] = dt;
                    //}
                    //else
                    //{
                    //    grdFaultTC.DataSource = dt;
                    //    grdFaultTC.DataBind();
                    //    foreach (GridViewRow rows in grdFaultTC.Rows)
                    //    {
                    //        Label lblstatus = (Label)rows.FindControl("lblStatus");
                    //        if (lblstatus.Text == "Already Sent")
                    //        {
                    //            ((CheckBox)rows.FindControl("chkSelect")).Enabled = false;
                    //        }
                    //    }
                    //    ViewState["FaultTC"] = dt;
                    //}
                    PopulateCheckedValues();
                    
                    
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ShowEmptyGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow newRow = dt.NewRow();
                dt.Rows.Add(newRow);
                dt.Columns.Add("TC_ID");
                dt.Columns.Add("TC_CODE");
                dt.Columns.Add("TC_SLNO");
                dt.Columns.Add("TM_NAME");
                dt.Columns.Add("TC_CAPACITY");
                dt.Columns.Add("TC_MANF_DATE");
                dt.Columns.Add("TC_PURCHASE_DATE");
                dt.Columns.Add("TC_WARANTY_PERIOD");
                dt.Columns.Add("TS_NAME");
                dt.Columns.Add("RCOUNT");
                dt.Columns.Add("TC_GUARANTY_TYPE");
                dt.Columns.Add("STATUS");

                grdFaultTC.DataSource = dt;
                grdFaultTC.DataBind();
                int iColCount = grdFaultTC.Rows[0].Cells.Count;
                grdFaultTC.Rows[0].Cells.Clear();
                grdFaultTC.Rows[0].Cells.Add(new TableCell());
                grdFaultTC.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdFaultTC.Rows[0].Cells[0].Text = "No Records Found";

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        protected void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {

                //Check AccessRights
                //bool bAccResult = CheckAccessRights("2");
                //if (bAccResult == false)
                //{
                //    return;
                //}

                bool AtleastOneApp = false;
                int i = 0;
                string[] Arr = new string[3];
                grdFaultTC.AllowPaging = false;
                SaveCheckedValues();
                LoadFaultTc();

                PopulateCheckedValues();

                //To check Selected Transformers Already Sent for Supplier/Repair and Waiting For Approval
                clsApproval objApproval = new clsApproval();
                string sResult = objApproval.GetDataReferenceId("30");

                if (!sResult.StartsWith(","))
                {
                    sResult = "," + sResult;
                }
                if (!sResult.EndsWith(","))
                {
                    sResult = sResult + ",";
                }

                string[] strQryVallist = new string[grdFaultTC.Rows.Count];
                foreach (GridViewRow row in grdFaultTC.Rows)
                {
                    if (((CheckBox)row.FindControl("chkSelect")).Checked == true)
                    {
                        strQryVallist[i] = ((Label)row.FindControl("lblTCId")).Text.Trim();

                        string sTCCode = ((Label)row.FindControl("lblTCCode")).Text.Trim();

                        if (sResult.Contains("," + sTCCode + ","))
                        {
                            ShowMsgBox("Selected DTr " + sTCCode + "Already sent for Supplier/Repairer, Waiting for Approval");
                            return;
                        }

                        AtleastOneApp = true;
                    }
                    i++;

                }

                grdFaultTC.AllowPaging = true;
                SaveCheckedValues();
                LoadFaultTc();
                PopulateCheckedValues();

                if (!AtleastOneApp)
                {
                    ShowMsgBox("Please Select DTr to Send for Repairer/Supplier");
                    SaveCheckedValues();
                    LoadFaultTc(); 
                    PopulateCheckedValues();
                    return;
                }

                string sSelectedValue = string.Empty;
                for (int j = 0; j < strQryVallist.Length; j++)
                {
                    if (strQryVallist[j] != null)
                    {
                        sSelectedValue += strQryVallist[j].ToString() + "~";
                    }
                }

                string sTCId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sSelectedValue));
                Session["TcId"] = sSelectedValue;

                string sStoreId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(cmbStore.SelectedValue));
                //string sGuaranteType = HttpUtility.UrlEncode(Genaral.UrlEncrypt(cmbGuarantyType.SelectedValue));
                Response.Redirect("TCRepairIssue.aspx?StoreId=" + sStoreId, false);

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void PopulateCheckedValues()
        {
            try
            {              
                    ArrayList arrCheckedValues = (ArrayList)ViewState["CHECKED_ITEMS"];
                    if (arrCheckedValues != null && arrCheckedValues.Count > 0)
                    {
                        foreach (GridViewRow gvrow in grdFaultTC.Rows)
                        {
                            if (grdFaultTC.DataKeys[gvrow.RowIndex].Values[0].ToString() != "")
                            {
                                int index = Convert.ToInt32(grdFaultTC.DataKeys[gvrow.RowIndex].Values[0]);
                                if (arrCheckedValues.Contains(index))
                                {
                                    CheckBox myCheckBox = (CheckBox)gvrow.FindControl("chkSelect");
                                    myCheckBox.Checked = true;
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


        //This method is used to save the checkedstate of values
        private void SaveCheckedValues()
        {
            try
            {

                ArrayList arrCheckedValues = new ArrayList();
                    int index = -1;
                    foreach (GridViewRow gvrow in grdFaultTC.Rows)
                    {
                    if(grdFaultTC.DataKeys[gvrow.RowIndex].Values[0].ToString() != "")
                    {
                        index = Convert.ToInt32(grdFaultTC.DataKeys[gvrow.RowIndex].Values[0]); ;

                        bool result = ((CheckBox)gvrow.FindControl("chkSelect")).Checked;

                        // Check in the viewstate
                        if (ViewState["CHECKED_ITEMS"] != null)
                            arrCheckedValues = (ArrayList)ViewState["CHECKED_ITEMS"];
                        if (result)
                        {
                            if (!arrCheckedValues.Contains(index))
                                arrCheckedValues.Add(index);
                        }
                        else
                            arrCheckedValues.Remove(index);
                    }
                    if (arrCheckedValues != null && arrCheckedValues.Count > 0)
                        ViewState["CHECKED_ITEMS"] = arrCheckedValues;
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

        protected void grdFaultTC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                SaveCheckedValues();
                grdFaultTC.PageIndex = e.NewPageIndex;
                //LoadFaultTc();
                DataTable dt = new DataTable();
                dt = (DataTable)ViewState["FaultTC"];
                grdFaultTC.DataSource = dt;
                grdFaultTC.DataBind();
                foreach (GridViewRow rows in grdFaultTC.Rows)
                {
                    Label lblstatus = (Label)rows.FindControl("lblStatus");
                    if (lblstatus.Text == "Already Sent")
                    {
                        ((CheckBox)rows.FindControl("chkSelect")).Enabled = false;
                    }
                }
                PopulateCheckedValues();
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
                cmbCapacity.SelectedIndex = 0;
                cmbMake.SelectedIndex = 0;
                //cmbStore.SelectedIndex = 0;
                //cmbSupplier.SelectedIndex = 0;
                grdFaultTC.Visible = false;
                cmdSend.Visible = false;

                if (cmbStore.Enabled == true)
                {
                    cmbStore.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void GetStoreId()
        {
            string strId = string.Empty;
            try
            {
                clsTcMaster objTcMaster = new clsTcMaster();

                if (objSession.sRoleType == "2")
                {
                    strId = objSession.OfficeCode;
                }
                else
                {
                    strId = objTcMaster.GetStoreId(objSession.OfficeCode);
                }

                //string strId= objTcMaster.GetStoreId(objSession.OfficeCode);
                cmbStore.SelectedValue = strId;

                if (objSession.OfficeCode == "" || objSession.OfficeCode.Length==1)
                {
                    cmbStore.Enabled = true;
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
    }
}