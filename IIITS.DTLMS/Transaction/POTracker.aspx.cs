using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.IO;
using System.Configuration;
using System.Net;

namespace IIITS.DTLMS.Transaction
{
    public partial class POTracker : System.Web.UI.Page
    {
        public string strFormCode = "POTracker";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Session["clsSession"].ToString() == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                objSession = (clsSession)Session["clsSession"];

                if (!IsPostBack)
                {
                    Genaral.Load_Combo("SELECT \"FY_ID\", \"FY_YEARS\" FROM \"TBLFINANCIALYEAR\" ORDER BY \"FY_YEARS\" ", "--Select--", cmbFinYear);
                    LoadPODetails();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void LoadPODetails(string sFyId = "")
        {
            try
            {
                clsPoTracker obj = new clsPoTracker();
                obj.sFinId = sFyId;
                obj.GetPODetails(obj);
                ViewState["PODETAILS"] = obj.dtPODetails;
                grdPODetails.DataSource = obj.dtPODetails;
                grdPODetails.DataBind();
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
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
                        ViewState["PODETAILS"] = dataView.ToTable();
                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());
                        ViewState["PODETAILS"] = dataView.ToTable();
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

        protected void grdPODetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdPODetails.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["PODETAILS"];
                grdPODetails.DataSource = SortDataTable(dt as DataTable, true);
                grdPODetails.DataBind();
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void imgBtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton imgEdit = (LinkButton)sender;
                GridViewRow rw = (GridViewRow)imgEdit.NamingContainer;
                String sPoId = ((Label)rw.FindControl("lblPoId")).Text;
                sPoId = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sPoId));
                String sPoNo = ((Label)rw.FindControl("lblPoNO")).Text;
                sPoNo = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sPoNo));
                String sPoDate = ((Label)rw.FindControl("lblPoDate")).Text;
                sPoDate = HttpUtility.UrlEncode(Genaral.UrlEncrypt(sPoDate));
                Response.Redirect("PODetails.aspx?POID=" + sPoId + "&PONO=" + sPoNo + "&PODATE=" + sPoDate + "", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdPODetails_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                GridViewSortExpression = e.SortExpression;
                int pageIndex = grdPODetails.PageIndex;
                DataTable dt = (DataTable)ViewState["PODETAILS"];
                string sortingDirection = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    grdPODetails.DataSource = SortDataTable(dt as DataTable, false);
                }
                else
                {
                    grdPODetails.DataSource = dt;
                }
                grdPODetails.DataBind();
                grdPODetails.PageIndex = pageIndex;
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string sQry = string.Empty;
                if (cmbFinYear.SelectedIndex > 0)
                {
                    string sFynId = string.Empty;
                    sFynId = cmbFinYear.SelectedValue;
                    LoadPODetails(sFynId);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}