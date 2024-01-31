using IIITS.DTLMS.BL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IIITS.DTLMS.TCRepair
{
    public partial class RepairerApprovePending : System.Web.UI.Page
    {
        string strFormCode = "RepairerPendingDetails";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
            {
                Response.Redirect("~/Login.aspx", false);
            }
            else
            {
                objSession = (clsSession)Session["clsSession"];
            }
        }

        protected void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDtrcode.Text.Length==0)
                {
                    ShowMsgBox("Enter DTr code");
                    return;
                }

                LoadApprovalPendingDetails();
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
        public void LoadApprovalPendingDetails()
        {
            try
            {
                clsDashboard objDashboard = new clsDashboard();
                DataTable dtLoadDetails = new DataTable();
                string sBOType = string.Empty;
                string sOfficeCode = string.Empty;
                sOfficeCode = objSession.OfficeCode;
                string RoleType = objSession.sRoleType;
               string dtrcode = txtDtrcode.Text;

                if (sOfficeCode != "")
                {
                    dtLoadDetails = objDashboard.LoadRepairerApprovalPendingDetails(sOfficeCode, txtDtrcode.Text, RoleType);
                }
                else
                {

                    sOfficeCode = "";
                    dtLoadDetails = objDashboard.LoadRepairerApprovalPendingDetails(sOfficeCode, txtDtrcode.Text, RoleType);
                }
                if (dtLoadDetails.Rows.Count > 0)
                {
                    grdApprovePending.DataSource = dtLoadDetails;
                    grdApprovePending.DataBind();
                    ViewState["ApprovePending"] = dtLoadDetails;
                }
                else
                {
                    ShowEmptyGrid();
                    ViewState["ApprovePending"] = dtLoadDetails;

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
                dt.Columns.Add("dtrcode");
                dt.Columns.Add("BO_NAME");
                dt.Columns.Add("roleid");
                dt.Columns.Add("OMSECTION");

                grdApprovePending.DataSource = dt;
                grdApprovePending.DataBind();

                int iColCount = grdApprovePending.Rows[0].Cells.Count;
                grdApprovePending.Rows[0].Cells.Clear();
                grdApprovePending.Rows[0].Cells.Add(new TableCell());
                grdApprovePending.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdApprovePending.Rows[0].Cells[0].Text = "No Records Found";

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }
        protected void grdApprovePending_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dtComplete = new DataTable();
                grdApprovePending.PageIndex = e.NewPageIndex;
                dtComplete = (DataTable)ViewState["ApprovePending"];
                grdApprovePending.DataSource = SortDataTable(dtComplete as DataTable, true);
                grdApprovePending.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdApprovePending_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridViewSortExpression = e.SortExpression;
            int pageIndex = grdApprovePending.PageIndex;
            DataTable dt = (DataTable)ViewState["ApprovePending"];
            string sortingDirection = string.Empty;


            if (dt.Rows.Count > 0 )
            {
                grdApprovePending.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                grdApprovePending.DataSource = dt;
            }
            grdApprovePending.DataBind();
            grdApprovePending.PageIndex = pageIndex;
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
                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());

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
    }
}