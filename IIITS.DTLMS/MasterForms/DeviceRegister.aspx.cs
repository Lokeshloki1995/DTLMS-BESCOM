using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IIITS.DTLMS.BL;

namespace IIITS.DTLMS.MasterForms
{
    public partial class DeviceRegister : System.Web.UI.Page
    {
        string strFormCode = "DeviceRegister";
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
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    //txtEffectFrom.Attributes.Add("readonly", "readonly");

                    //CalendarExtender1.EndDate = System.DateTime.Now;
                    if (!IsPostBack)
                    {
                        AdminAccess();
                        LoadDeviceRegistrerDetails("", "");

                    }
                }
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        public void LoadDeviceRegistrerDetails(string sDeviceNo = "", string sName = "")
        {
            try
            {

                clsDeviceRegister objDevice = new clsDeviceRegister();
                DataTable dtDeviceDetails = new DataTable();
                objDevice.sDeviceId = sDeviceNo;
                objDevice.sFullName = sName;

                dtDeviceDetails = objDevice.LoadDeviceGrid(objDevice);

               if (dtDeviceDetails.Rows.Count <= 0)
                {
                    DataTable dtDevDetails = new DataTable();
                    DataRow newRow = dtDevDetails.NewRow();
                    dtDevDetails.Rows.Add(newRow);
                    dtDevDetails.Columns.Add("US_ID");
                    dtDevDetails.Columns.Add("MR_REQUEST_BY");
                    dtDevDetails.Columns.Add("MR_ID");
                      
                    dtDevDetails.Columns.Add("MR_DEVICE_ID");
                    dtDevDetails.Columns.Add("US_FULL_NAME");
                    dtDevDetails.Columns.Add("MR_APPROVE_STATUS");
                    dtDevDetails.Columns.Add("MR_CRON");
                    
                    grdDeviceRegister.DataSource = dtDevDetails;
                    grdDeviceRegister.DataBind();

                    int iColCount = grdDeviceRegister.Rows[0].Cells.Count;
                    grdDeviceRegister.Rows[0].Cells.Clear();
                    grdDeviceRegister.Rows[0].Cells.Add(new TableCell());
                    grdDeviceRegister.Rows[0].Cells[0].ColumnSpan = iColCount;
                    grdDeviceRegister.Rows[0].Cells[0].Text = "No Records Found";

                    ViewState["Device"] = dtDeviceDetails;
                }
                else
                {
                    grdDeviceRegister.DataSource = dtDeviceDetails;
                    grdDeviceRegister.DataBind();
                    ViewState["Device"] = dtDeviceDetails;
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
        protected void grdDeviceRegister_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {

                grdDeviceRegister.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["Device"];
                dt.Columns["MR_DEVICE_ID"].AllowDBNull = true;
                dt.Columns["US_FULL_NAME"].AllowDBNull = true;
                grdDeviceRegister.DataSource = SortDataTable(dt as DataTable, true);
                grdDeviceRegister.DataBind();

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
                        ViewState["Device"] = dataView.ToTable();



                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());

                        ViewState["Device"] = dataView.ToTable();
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

        protected void grdDeviceRegister_Sorting(object sender, GridViewSortEventArgs e)
        {

            GridViewSortExpression = e.SortExpression;
            int pageIndex = grdDeviceRegister.PageIndex;
            DataTable dt = (DataTable)ViewState["Device"];
            string sortingDirection = string.Empty;

            Image sortImage = new Image();
            Image sortImageboth = new Image();


            if (dt.Rows.Count > 0)
            {
                grdDeviceRegister.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                grdDeviceRegister.DataSource = dt;
            }
            grdDeviceRegister.DataBind();
            grdDeviceRegister.PageIndex = pageIndex;
        }

        protected void grdDeviceRegister_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try

            {
                if (e.CommandName == "search")
                {
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    TextBox txtSearchName = (TextBox)row.FindControl("txtsFullName");
                    TextBox txtSearchDevice = (TextBox)row.FindControl("txtIvDeviceId");
                    LoadDeviceRegistrerDetails(txtSearchDevice.Text, txtSearchName.Text);
                }
               
                if (e.CommandName == "status")
                {
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    string sUserId = ((Label)row.FindControl("lblUserId")).Text;
                    string sMuId = ((Label)row.FindControl("lblMrId")).Text;

                    ImageButton imgBtnApproval;
                    imgBtnApproval = (ImageButton)row.FindControl("imgBtnApproval");

                    clsDeviceRegister objDevice = new clsDeviceRegister();
                    objDevice.sRequestedBy = sUserId;
                    objDevice.sMuId = sMuId;
                    objDevice.sApprovedBy = objSession.UserId;
                    bool status = objDevice.UpdateDeviceStatus(objDevice);
                    if(status == true)
                    {
                        ShowMsgBox("Device Approved SuccessFully");

                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Device Register successfully");
                        }
                    }
                    else
                    {
                        ShowMsgBox("Device Approval Failed");
                        if (objSession.sTransactionLog == "1")
                        {
                            Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Device Register Failed");
                        }
                    }

                    DataTable dtDeviceDetails = new DataTable();
                    dtDeviceDetails = objDevice.LoadDeviceGrid(objDevice);
                    grdDeviceRegister.DataSource = dtDeviceDetails;
                    grdDeviceRegister.DataBind();

                    //Response.Redirect(Request.RawUrl);
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdDeviceRegister_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblStatus;
                    lblStatus = (Label)e.Row.FindControl("lblStatus");
                    ImageButton imgBtnApproval;
                    imgBtnApproval = (ImageButton)e.Row.FindControl("imgBtnApproval");

                    if (lblStatus.Text == "APPROVED")
                    {
                        imgBtnApproval.Enabled = false;
                        imgBtnApproval.ToolTip = "User is DeActivated,You Cannot Edit";
                    }
                    else
                    {
                        imgBtnApproval.Enabled = true;
                        imgBtnApproval.ToolTip = "";
                    }
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);


            }
        }

        protected void Export_clickDeviceRegister(object sender, EventArgs e)
        {

            //clsDeviceRegister objDevice = new clsDeviceRegister();
            //DataTable dtDeviceDetails = new DataTable();
            //string sDeviceNo = "";
            //string sName = "";
            //objDevice.sDeviceId = sDeviceNo;
            //objDevice.sFullName = sName;

            //dtDeviceDetails = objDevice.LoadDeviceGrid(objDevice);

            DataTable dtDeviceDetails = (DataTable)ViewState["Device"];
            if (dtDeviceDetails.Rows.Count > 0)
            {

                dtDeviceDetails.Columns["MR_DEVICE_ID"].ColumnName = "DEVICE ID";
                dtDeviceDetails.Columns["US_FULL_NAME"].ColumnName = "NAME";
                dtDeviceDetails.Columns["MR_APPROVE_STATUS"].ColumnName = "APPROVAL STATUS";
                dtDeviceDetails.Columns["MR_CRON"].ColumnName = "CREATED ON";


                List<string> listtoRemove = new List<string> { "MR_REQUEST_BY", "MR_ID", "US_ID" };
                string filename = "DeviceRegisterDetails" + DateTime.Now + ".xls";
                string pagetitle = "Device Register Details";

                Genaral.getexcel(dtDeviceDetails, listtoRemove, filename, pagetitle);
            }
            else
            {
                ShowMsgBox("No record found");

                DataTable dtDevDetails = new DataTable();
                DataRow newRow = dtDevDetails.NewRow();
                dtDevDetails.Rows.Add(newRow);
                dtDevDetails.Columns.Add("US_ID");
                dtDevDetails.Columns.Add("MR_REQUEST_BY");
                dtDevDetails.Columns.Add("MR_ID");

                dtDevDetails.Columns.Add("MR_DEVICE_ID");
                dtDevDetails.Columns.Add("US_FULL_NAME");
                dtDevDetails.Columns.Add("MR_APPROVE_STATUS");
                dtDevDetails.Columns.Add("MR_CRON");

                grdDeviceRegister.DataSource = dtDevDetails;
                grdDeviceRegister.DataBind();

                int iColCount = grdDeviceRegister.Rows[0].Cells.Count;
                grdDeviceRegister.Rows[0].Cells.Clear();
                grdDeviceRegister.Rows[0].Cells.Add(new TableCell());
                grdDeviceRegister.Rows[0].Cells[0].ColumnSpan = iColCount;
                grdDeviceRegister.Rows[0].Cells[0].Text = "No Records Found";
            }



        }

        #region Access Rights

        public void AdminAccess()
        {
            try
            {
                if (objSession.RoleId != "8")
                {
                    Response.Redirect("~/UserRestrict.aspx", false);
                }
                //if (objSession.RoleId != "11")
                //{
                //    Response.Redirect("~/UserRestrict.aspx", false);
                //}
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        #endregion

       
    }
}