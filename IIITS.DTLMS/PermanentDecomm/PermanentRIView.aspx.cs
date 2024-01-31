﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;

namespace IIITS.DTLMS.PermanentDecomm
{
    public partial class PermanentRIView : System.Web.UI.Page
    {
        clsSession objSession;
        string strFormCode = "PermanentRIView";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Session["clsSession"].ToString() == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }

                objSession = (clsSession)Session["clsSession"];
                lblMessage.Text = string.Empty;
                if (!IsPostBack)
                {
                    CheckAccessRights("4");
                    LoadAllDecomm("1");

                }
            }
            catch (Exception ex)
            {

                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void Export_ClickRI(object sender, EventArgs e)
        {
           
            DataTable dt = (DataTable)ViewState["Decomm"];

            if (dt.Rows.Count > 0)
            {
                if (cmbType.SelectedValue == "1" || cmbType.SelectedValue == "4")
                {

                    dt.Columns["DT_CODE"].ColumnName = "Transformer Centre Code";
                    dt.Columns["DT_TC_ID"].ColumnName = "DTR Code";
                    dt.Columns["PWO_NO_DECOM"].ColumnName = "WorkOrder No";
                    //dt.Columns["PTI_INDENT_NO"].ColumnName = "Indent No";
                   // dt.Columns["PTI_INDENT_NO"].ColumnName = "Invoice No";
                    dt.Columns["PEST_ID"].ColumnName = "Failure Id";

                    dt.Columns["Failure Id"].SetOrdinal(3);

                }

                if (cmbType.SelectedValue == "2")
                {
                    dt.Columns["DT_CODE"].ColumnName = "Transformer Centre Code";
                    dt.Columns["DT_TC_ID"].ColumnName = "DTR Code";
                    dt.Columns["PWO_NO_DECOM"].ColumnName = "WorkOrder No";
                    //dt.Columns["PTI_INDENT_NO"].ColumnName = "Indent No";
                   // dt.Columns["PTI_INDENT_NO"].ColumnName = "Invoice No";
                    dt.Columns["PEST_ID"].ColumnName = "Enhancement ID";

                    dt.Columns["Enhancement ID"].SetOrdinal(3);
                }

                List<string> listtoRemove = new List<string> { "TR_ID", "TC_SLNO","TR_ID1", "STATUS", "DT_TC_ID1" };
                string filename = "PermanentRI" + DateTime.Now + ".xls";
                string pagetitle = "Permanent RI Acknowledgement View";

                Genaral.getexcel(dt, listtoRemove, filename, pagetitle);
            }
            else
            {
                ShowMsgBox("No record found");
                ShowEmptyGrid();
            }



        }


        protected void grdReplacementDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {

                if (e.CommandName == "Create")
                {
                    //Check AccessRights
                    bool bAccResult = CheckAccessRights("2");
                    if (bAccResult == false)
                    {
                        return;
                    }

                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

                    Label lblFailureId = (Label)row.FindControl("lblFailureId");
                    Label lblReplaceId = (Label)row.FindControl("lblReplaceId");

                    string sFailureId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(lblFailureId.Text));
                    string sType = HttpUtility.UrlEncode(Genaral.UrlEncrypt(cmbType.SelectedValue));
                    string sDecommId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(lblReplaceId.Text));

                    Response.Redirect("PermanentRI.aspx?DecommId=" + sDecommId + "&FailureId=" + sFailureId + "&TypeValue=" + sType, false);


                }
                if (e.CommandName == "search")
                {
                    string sFilter = string.Empty;
                    DataView dv = new DataView();

                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    TextBox txtDtcCode = (TextBox)row.FindControl("txtDtcCode");




                    DataTable dt = (DataTable)ViewState["Decomm"];
                    dv = dt.DefaultView;
                    if (txtDtcCode.Text != "")
                    {
                        sFilter = "DT_CODE Like '%" + txtDtcCode.Text.Replace("'", "`") + "%' AND";
                    }



                    if (sFilter.Length > 0)
                    {
                        sFilter = sFilter.Remove(sFilter.Length - 3);
                        grdReplacementDetails.PageIndex = 0;
                        dv.RowFilter = sFilter;
                        if (dv.Count > 0)
                        {
                            grdReplacementDetails.DataSource = dv;
                            ViewState["Decomm"] = dv.ToTable();
                            grdReplacementDetails.DataBind();

                        }
                        else
                        {
                            ShowEmptyGrid();
                            ViewState["Decomm"] = dv.ToTable();
                        }
                    }
                    else
                    {

                        LoadAllDecomm("1");
                    }
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
                dt.Columns.Add("PTR_ID");
                dt.Columns.Add("DT_CODE");
                dt.Columns.Add("DT_TC_ID");
                dt.Columns.Add("PEST_ID");
                //dt.Columns.Add("PTI_INDENT_NO");
                //dt.Columns.Add("PTI_INDENT_NO");
                dt.Columns.Add("PWO_NO_DECOM");

                grdReplacementDetails.DataSource = dt;
                grdReplacementDetails.DataBind();

                int iColCount = grdReplacementDetails.Rows[0].Cells.Count;
                grdReplacementDetails.Rows[0].Cells.Clear();
                grdReplacementDetails.Rows[0].Cells.Add(new TableCell());
                grdReplacementDetails.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdReplacementDetails.Rows[0].Cells[0].Text = "No Records Found";

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }




        public void LoadAllDecomm(string sType)
        {

            try
            {
                clsPermanentRI objRIApprov = new clsPermanentRI();
                string sMsg = string.Empty;

                objRIApprov.sTasktype = sType;
                // objRIApprov.sOfficeCode = objSession.OfficeCode;
                if (objSession.sRoleType == "2")
                {

                    string sLocCode = clsStoreOffice.Getofficecode(objSession.OfficeCode);

                    objRIApprov.sOfficeCode = sLocCode;

                }
                else
                {
                    objRIApprov.sOfficeCode = objSession.OfficeCode;
                }

                DataTable dt = objRIApprov.LoadAlreadyRI(objRIApprov);

                //To show the Type of Gridview
                if (sType == "1")
                {
                    //Gridview column visible true/false based on conditions
                    grdReplacementDetails.Columns[3].Visible = true;
                    grdReplacementDetails.Columns[4].Visible = false;

                    lblGridType.Text = "Transformer Centre Permanent RI Acknowledgement Details :";
                    sMsg = "PRI";
                }
                else if (sType == "4")
                {
                    //Gridview column visible true/false based on conditions
                    grdReplacementDetails.Columns[3].Visible = true;
                    grdReplacementDetails.Columns[4].Visible = false;

                    lblGridType.Text = "Transformer Centre Permanent RI Acknowledgement Details :";
                    sMsg = "PRI";
                }
                else
                {
                    //Gridview column visible true/false based on conditions
                    grdReplacementDetails.Columns[3].Visible = false;
                    grdReplacementDetails.Columns[4].Visible = true;

                    lblGridType.Text = "Transformer Centre Permanent RI Acknowledgement Details :";
                    sMsg = "PRI";
                }

                if (dt.Rows.Count > 0)
                {
                    grdReplacementDetails.DataSource = dt;
                    grdReplacementDetails.DataBind();
                    ViewState["Decomm"] = dt;
                }
                else
                {
                    lblMessage.Text = "Note : No " + sMsg + " Transformer Centre Available,,Please Decommission the Transformer Centre before Approval";
                    grdReplacementDetails.DataSource = dt;
                    grdReplacementDetails.DataBind();
                }


            }
            catch (Exception ex)
            {

                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        protected void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbType.SelectedValue == "1")
                {
                    LoadAllDecomm(cmbType.SelectedValue);

                }
                else if (cmbType.SelectedValue == "2")
                {
                    LoadAllDecomm(cmbType.SelectedValue);

                }
                else if (cmbType.SelectedValue == "4")
                {
                    LoadAllDecomm(cmbType.SelectedValue);

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdReplacementDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdReplacementDetails.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["Decomm"];
                grdReplacementDetails.DataSource = SortDataTable(dt as DataTable, true);
                grdReplacementDetails.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdReplacementDetails_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridViewSortExpression = e.SortExpression;
            int pageIndex = grdReplacementDetails.PageIndex;
            DataTable dt = (DataTable)ViewState["Decomm"];
            string sortingDirection = string.Empty;

            if (dt.Rows.Count > 0)
            {
                grdReplacementDetails.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                grdReplacementDetails.DataSource = dt;
            }
            grdReplacementDetails.DataBind();
            grdReplacementDetails.PageIndex = pageIndex;
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
                        ViewState["Decomm"] = dataView.ToTable();


                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());
                        ViewState["Decomm"] = dataView.ToTable();

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

        #region Access Rights
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "PermanentRI";
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

    }
}