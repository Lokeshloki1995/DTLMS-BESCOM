﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IIITS.DTLMS.BL;

namespace IIITS.DTLMS.MasterForms
{
    public partial class Role : System.Web.UI.Page
    {
        string strFormCode = "Role.aspx";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
            {
                Response.Redirect("~/Login.aspx", false);
            }
            Update.Visible = false;
            UpdateRole.Visible = false;

            Form.DefaultButton = cmdSave.UniqueID;
            objSession = (clsSession)Session["clsSession"];
            lblMessage.Text = string.Empty;
            // CheckAccessRights("4");
            AdminAccess();
            if (!IsPostBack)
            {
                Genaral.Load_Combo("SELECT \"DM_DESGN_ID\", \"DM_NAME\" FROM \"TBLDESIGNMAST\"", "--Select--", cmbDesignation);
                if (Request.QueryString["StrQryId"] != null && Request.QueryString["StrQryId"].ToString() != "")
                {
                    txtRoleId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["StrQryId"]));
                }

                if (txtRoleId.Text != "")
                {
                   
                    GetRoleDetails(txtRoleId.Text);
                    Create.Visible = false;
                    CreateRole.Visible = false;

                    Update.Visible = true;
                    UpdateRole.Visible = true;
                }
            }

        }
        public void AdminAccess()
        {
            try
            {
                if (objSession.RoleId != "11")
                {
                    Response.Redirect("~/UserRestrict.aspx", false);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void GetRoleDetails(string strRoleId)
        {
            try
            {
                clsRole objRole = new clsRole();
                DataTable dtDetails = new DataTable();

                objRole.sRoleId = Convert.ToString(strRoleId);
                objRole.getRoleDetails(objRole);
                txtRoleId.Text = Convert.ToString(objRole.sRoleId);
                txtRole.Text = Convert.ToString(objRole.sRoleName);
                //cmbDesignation.SelectedIndex = cmbDesignation.Items.IndexOf(cmbDesignation.Items.FindByText(objRole.sRoleDesig));
                cmbDesignation.SelectedValue = objRole.sRoleId;
                cmdSave.Text = "Update";



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

            if (txtRole.Text == "")
            {
                txtRole.Focus();
                ShowMsgBox("Please Enter the Role");
                return bValidate;
            }
            if (cmbDesignation.SelectedIndex == 0)
            {
                cmbDesignation.Focus();
                ShowMsgBox("Please Select Designation");
                return bValidate;
            }

            bValidate = true;
            return bValidate;
        }

        protected void cmdReset_Click(object sender, EventArgs e)
        {
            txtRole.Text = string.Empty;
            cmbDesignation.SelectedIndex = 0;
            txtRoleId.Text = string.Empty;
            cmdSave.Text = "Save";

        }



        #region Access Rights
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "Designation";
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

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                clsRole objRole = new clsRole();
                string[] Arr = new string[3];

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
                    objRole.sRoleId = Convert.ToString(txtRoleId.Text);

                    objRole.sRoleName = txtRole.Text;
                    objRole.sRoleDesig = cmbDesignation.SelectedValue;
                    objRole.sCrby = objSession.UserId;

                    Arr = objRole.SaveDetails(objRole);

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "Role Master");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        txtRoleId.Text = Arr[0].ToString();
                        cmdSave.Text = "Update";
                        ShowMsgBox("Saved Successfully");
                        cmdReset_Click(sender, e);

                    }
                    if (Arr[1].ToString() == "1")
                    {
                        ShowMsgBox(Arr[0]);
                        cmdReset_Click(sender, e);

                    }

                    if (Arr[1].ToString() == "2")
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
    }
}