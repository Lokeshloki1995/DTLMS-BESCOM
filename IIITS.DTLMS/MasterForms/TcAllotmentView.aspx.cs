using IIITS.DTLMS.BL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IIITS.DTLMS.MasterForms
{
    public partial class TcAllotmentView : System.Web.UI.Page
    {
        string strFormCode = "AllotmentView";
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
                if (!IsPostBack)
                {
                    CheckAccessRights("4");
                    LoadAllotmentDetails();

                 }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }       
        
        }
        public void LoadAllotmentDetails(string sDINumber ="")
        {
            try
            {
                clsAllotement objAllot = new clsAllotement();
                DataTable dt = new DataTable();
                objAllot.sDINo = sDINumber;

                dt = objAllot.GetAllotedDetails(sDINumber.ToUpper());

                if (dt.Rows.Count <= 0)
                {
                    DataTable dtPoDetails = new DataTable();
                    DataRow newRow = dtPoDetails.NewRow();
                    dtPoDetails.Rows.Add(newRow);
                    dtPoDetails.Columns.Add("ALT_ID");
                    dtPoDetails.Columns.Add("ALT_DI_NO");
                    dtPoDetails.Columns.Add("ALT_NO");
                    dtPoDetails.Columns.Add("ALT_DATE");
                    dtPoDetails.Columns.Add("ALT_STORE_NAME");
                    dtPoDetails.Columns.Add("DIV_NAME");
                    dtPoDetails.Columns.Add("ALT_CAPACITY");
                    dtPoDetails.Columns.Add("ALT_STARRATENAME");
                    dtPoDetails.Columns.Add("ALT_QUANTITY");

                    grdAllotmentView.DataSource = dtPoDetails;
                    grdAllotmentView.DataBind();

                    int iColCount = grdAllotmentView.Rows[0].Cells.Count;
                    grdAllotmentView.Rows[0].Cells.Clear();
                    grdAllotmentView.Rows[0].Cells.Add(new TableCell());
                    grdAllotmentView.Rows[0].Cells[0].ColumnSpan = iColCount;
                    grdAllotmentView.Rows[0].Cells[0].Text = "No Records Found";
                    ViewState["ALT_VIEW"] = dt;

                }
                else
                {
                    grdAllotmentView.DataSource = dt;
                    grdAllotmentView.DataBind();
                    ViewState["ALT_VIEW"] = dt;

                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdAllotmentView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdAllotmentView.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["ALT_VIEW"];
                grdAllotmentView.DataSource = SortDataTable(dt as DataTable, true);
                grdAllotmentView.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
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
                        ViewState["ALT_VIEW"] = dataView.ToTable();

                    }
                    else
                    {

                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());
                        ViewState["ALT_VIEW"] = dataView.ToTable();

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

        public SortDirection direction
        {
            get
            {
                if (ViewState["directionState"] == null)
                {
                    ViewState["directionState"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["directionState"];
            }
            set
            {
                ViewState["directionState"] = value;
            }
        }
        protected void grdAllotmentView_Sorting(object sender, GridViewSortEventArgs e)
        {

            GridViewSortExpression = e.SortExpression;
            int pageIndex = grdAllotmentView.PageIndex;
            DataTable dt = (DataTable)ViewState["ALT_VIEW"];
            string sortingDirection = string.Empty;
            if (dt.Rows.Count > 0)
            {
                grdAllotmentView.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                grdAllotmentView.DataSource = dt;
            }
            grdAllotmentView.DataBind();
            grdAllotmentView.PageIndex = pageIndex;
        }
        protected void grdAllotmentView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Submit")
                {

                    //Check AccessRights
                    bool bAccResult = CheckAccessRights("3");
                    if (bAccResult == false)
                    {
                        return;
                    }

                    GridViewRow row = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                    int rowindex = row.RowIndex;
                    Label lblAltid = (Label)grdAllotmentView.Rows[rowindex].FindControl("lblAltid");
                    Label lblAltNo = (Label)grdAllotmentView.Rows[rowindex].FindControl("lblAltNo");
                    Label lblDiNo = (Label)grdAllotmentView.Rows[rowindex].FindControl("lblDiNo");
                    string strAltid = HttpUtility.UrlEncode(Genaral.UrlEncrypt(lblAltid.Text));
                    string strAltNo = HttpUtility.UrlEncode(Genaral.UrlEncrypt(lblAltNo.Text));
                    string strDiNo = HttpUtility.UrlEncode(Genaral.UrlEncrypt(lblDiNo.Text));
                    Response.Redirect("TcAllotment.aspx?QryAltid=" + strAltid + "&QryDiNo=" + strDiNo + "&QryAltNo=" + strAltNo, false);
                }
                LoadAllotmentDetails();
                if (e.CommandName == "search")
                {
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    TextBox txtaltNumber = (TextBox)row.FindControl("txtaltNumber");
                    LoadAllotmentDetails(txtaltNumber.Text);
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
        protected void cmdNew_Click(object sender, EventArgs e)
        {
            try
            {
                //Check AccessRights
                bool bAccResult = CheckAccessRights("2");
                if (bAccResult == false)
                {
                    return;
                }
                Response.Redirect("TcAllotment.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void Export_click(object sender, EventArgs e)
        {

            //clsPoMaster objPoMaster = new clsPoMaster();
            //DataTable dt = new DataTable();
            //string sPoNumber = "";
            //objPoMaster.sPoNo = sPoNumber;

            //dt = objPoMaster.LoadPoDetailGrid(objPoMaster);
            DataTable dt = (DataTable)ViewState["ALT_VIEW"];

            if (dt.Rows.Count > 0)
            {
                //DataTable dtPoDetails = new DataTable();
                //DataRow newRow = dtPoDetails.NewRow();
                //dtPoDetails.Rows.Add(newRow);
                //dtPoDetails.Columns.Add("PO_ID");
                //dtPoDetails.Columns.Add("PO_NO");
                //dtPoDetails.Columns.Add("PO_DATE");
                //dtPoDetails.Columns.Add("PB_QUANTITY");
                //dtPoDetails.Columns.Add("PO_SUPPLIER_ID");

                //grdPoMasterView.DataSource = dtPoDetails;
                //grdPoMasterView.DataBind();

                //int iColCount = grdPoMasterView.Rows[0].Cells.Count;
                //grdPoMasterView.Rows[0].Cells.Clear();
                //grdPoMasterView.Rows[0].Cells.Add(new TableCell());
                //grdPoMasterView.Rows[0].Cells[0].ColumnSpan = iColCount;
                //grdPoMasterView.Rows[0].Cells[0].Text = "No Records Found";
                //ShowMsgBox("No record found");
                dt.Columns["ALT_ID"].ColumnName = "ALT_ID";
                dt.Columns["ALT_NO"].ColumnName = "ALLOTMENT NUMBER";
                dt.Columns["ALT_DI_NO"].ColumnName = "DISPATCH No";
                dt.Columns["ALT_DATE"].ColumnName = "DI DATE";
                dt.Columns["ALT_STORE_NAME"].ColumnName = "STORE NAME";
                dt.Columns["DIV_NAME"].ColumnName = "DIVISION NAME";
                dt.Columns["ALT_CAPACITY"].ColumnName = "CAPACITY";
               
                dt.Columns["ALT_STARRATENAME"].ColumnName = "STAR RATE";
                dt.Columns["ALT_QUANTITY"].ColumnName = "QUANTITY";

                List<string> listtoRemove = new List<string> { "ALT_ID" };
                string filename = "AllotmentDetails" + DateTime.Now + ".xls";
                string pagetitle = "Allotment Details";

                Genaral.getexcel(dt, listtoRemove, filename, pagetitle);

            }
            else
            {
                ShowMsgBox("No record found");

                DataTable dtPoDetails = new DataTable();
                DataRow newRow = dtPoDetails.NewRow();
                dtPoDetails.Rows.Add(newRow);
                dtPoDetails.Columns.Add("ALT_ID");
                dtPoDetails.Columns.Add("ALT_NO");
                dtPoDetails.Columns.Add("DI_NO");
                dtPoDetails.Columns.Add("ALT_DATE");
                dtPoDetails.Columns.Add("STORE_NAME");               
                dtPoDetails.Columns.Add("ALT_CAPACITY");
                dtPoDetails.Columns.Add("STAR_RATE");
                dtPoDetails.Columns.Add("ALT_QUANTITY");

                grdAllotmentView.DataSource = dtPoDetails;
                grdAllotmentView.DataBind();

                int iColCount = grdAllotmentView.Rows[0].Cells.Count;
                grdAllotmentView.Rows[0].Cells.Clear();
                grdAllotmentView.Rows[0].Cells.Add(new TableCell());
                grdAllotmentView.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdAllotmentView.Rows[0].Cells[0].Text = "No Records Found";

            }
            //else
            //{
            //    ShowMsgBox("No record found");
            //}



        }
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "PoMaster";
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

    }
}