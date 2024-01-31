using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL.Dashboard;
using System.Data;
using System.Configuration;
using System.Web.UI.DataVisualization.Charting;
using IIITS.DTLMS.BL;


namespace IIITS.DTLMS
{
    public partial class StoreDashboard : System.Web.UI.Page
    {
        string strFormCode = "StoreDashboard";
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

                if (!IsPostBack)
                {
                    if (objSession != null)
                    {
                        lblLocation.Text = objSession.OfficeName;
                        hdfLocationCode.Value = objSession.OfficeCode;
                    }
                    GetConditionOfTC();
                    GetCapacityWiseTC();
                    GetPendingTC();
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        /// <summary>
        /// for fetch details to dashboard of tc counts
        /// </summary>
        public void DashboardFunctions()
        {
            try
            {
                GetConditionOfTC();
                GetCapacityWiseTC();
                GetPendingTC();
            }
            catch (Exception ex)
            {
                //clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        /// <summary>
        /// getting the count of condition of tc's
        /// </summary>

        public void GetConditionOfTC()
        {
            try
            {

                clsStoreDashboard objSDashboard = new clsStoreDashboard();
                objSDashboard.sOfficeCode = hdfLocationCode.Value.ToString() == null ? "" : hdfLocationCode.Value.ToString();
                if (objSDashboard.sOfficeCode == "0")
                {
                    objSDashboard.sOfficeCode = null;
                }
                lblNewTC.Text = objSDashboard.GetNewTCCount(objSDashboard);
                lblRepairGood.Text = objSDashboard.GetRepaireGoodCount(objSDashboard);
                lblReleaseGood.Text = objSDashboard.GetReleaseGoodCount(objSDashboard);
                lblFaulty.Text = objSDashboard.GetFaultyCount(objSDashboard);
                lblscarp.Text = objSDashboard.GetScrapCount(objSDashboard);
                lblMobileTC.Text = objSDashboard.GetMobileTCCount(objSDashboard);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        /// <summary>
        /// getting count of capacity wise tc's
        /// </summary>
        public void GetCapacityWiseTC()
        {
            try
            {
                clsStoreDashboard objSDashboard = new clsStoreDashboard();
                objSDashboard.sOfficeCode = hdfLocationCode.Value;
                if (objSDashboard.sOfficeCode == "0")
                {
                    objSDashboard.sOfficeCode = null;
                }
                lblCapacityless25.Text = objSDashboard.GetCapacityless25(objSDashboard);
                lblCapacity25_100.Text = objSDashboard.GetCapacity25_100(objSDashboard);
                lblCapacity125_250.Text = objSDashboard.GetCapacity125_250(objSDashboard);
                lblCapacitygreater250.Text = objSDashboard.GetCapacitygreater250(objSDashboard);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        /// <summary>
        /// getting the count of pending in repairer,replacement inv and RI(rv) of tc's
        /// </summary>
        public void GetPendingTC()
        {
            try
            {
                clsStoreDashboard objSDashboard = new clsStoreDashboard();
                objSDashboard.sOfficeCode = hdfLocationCode.Value;
                if (objSDashboard.sOfficeCode == "0")
                {
                    objSDashboard.sOfficeCode = null;
                }
                objSDashboard.roletype = objSession.sRoleType;
                lblTotalPendingfor_Issue.Text = objSDashboard.GetIssuePendingCount(objSDashboard);
                lblTotalPendingfor_Repair.Text = objSDashboard.GetRepairPendingCount(objSDashboard);
                lblTotalPendingto_Recive.Text = objSDashboard.GetRecivePendingCount(objSDashboard);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        protected void lnkConditionPending(object sender, EventArgs e)
        {
            try
            {
                string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
                string url = "/DashboardForm/FailurePendingOverview.aspx?RefId=Condition&OfficeCode=" + sOfficeCode;
                string s = "window.open('" + url + "','_blank');"; // '_newtab'
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying new dtr details in grid with values as conditions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NewTC_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=NewTCcount&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying repaired good dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RepairGood_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=RepairTCcount&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying released good dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ReleaseGood_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=ReleaseTCcount&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying faulty dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Faulty_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=FaultyTCcount&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying mob dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MobileTC_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=mobileTCcount&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
        /// <summary>
        /// to get capacity wise tc details to grid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkCapacityWiseTransformer_Click(object sender, EventArgs e)
        {
            try
            {
                string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
                string url = "/DashboardForm/FailurePendingOverview.aspx?RefId=CapacityWise&OfficeCode=" + sOfficeCode;
                string s = "window.open('" + url + "','_blank');"; // '_newtab'
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying 125 to 250 capacity dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Capacity125_250_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TC125_250_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying greater than 250 capacity dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Capacitygreater250_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TCgreater250_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying 25 to 100 capacity dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Capacity25_100_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TC25_100_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying less tha 25 capacity dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Capacityless25_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TCless25_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying scarp dtr details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Scarp_click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=Scarp&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
        /// <summary>
        /// to fetch transformer details 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkTransformer_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?RefId=TCpending&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        /// Redirected to FailurePendingOverview.aspx.cs for displaying replacement inv pend details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TotalPendingfor_Issue_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TCpending_issue_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        ///  Redirected to FailurePendingOverview.aspx.cs for displaying repairer pend details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TotalPendingfor_Repair_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TCpending_repair_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }

        /// <summary>
        ///  Redirected to FailurePendingOverview.aspx.cs for displaying RI(rv) pend details in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TotalPendingto_Recive_Click(object sender, EventArgs e)
        {
            string sOfficeCode = HttpUtility.UrlEncode(Genaral.UrlEncrypt(hdfLocationCode.Value));
            string url = "/DashboardForm/FailurePendingOverview.aspx?Value=TCpending_release_count&OfficeCode=" + sOfficeCode;
            string s = "window.open('" + url + "','_blank');"; // '_newtab'
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        }
        /// <summary>
        /// to redirect for MD Dashboard and to fetch the details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkMD_Dashboard_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "/DashboardForm/MdDashboard.aspx";
                string s = "window.open('" + url + "','_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}