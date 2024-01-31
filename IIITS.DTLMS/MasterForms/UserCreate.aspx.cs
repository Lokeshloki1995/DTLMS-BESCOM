using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;


namespace IIITS.DTLMS.MasterForms
{
    public partial class UserCreate : System.Web.UI.Page
    {

        string strFormCode = "UserCreate";
        clsSession objSession;
        int Zone_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Zone_code"]);
        int Circle_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Circle_code"]);
        int Division_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
        int SubDiv_code = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);
        int Section_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Section_code"]);
        int Feeder_code = Convert.ToInt32(ConfigurationSettings.AppSettings["feeder_code"]);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                Update.Visible = false;
                UpdateUser.Visible = false;

                Form.DefaultButton = cmdSave.UniqueID;
                objSession = (clsSession)Session["clsSession"];
                lblMessage.Text = string.Empty;
                CheckAccessRights("4");
                if (!IsPostBack)
                {

                    if (Request.QueryString["QryUserId"] != null && Request.QueryString["QryUserId"].ToString() != "")
                    {
                        txtuserID.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryUserId"]));
                    }
                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" ORDER BY \"SM_ID\"", "-Select-", cmbStore);
                    Genaral.Load_Combo("SELECT \"RO_ID\",\"RO_NAME\" FROM \"TBLROLES\" ORDER BY \"RO_ID\"", "-Select-", cmbRole);
                    Genaral.Load_Combo("SELECT \"DM_DESGN_ID\",\"DM_NAME\" FROM \"TBLDESIGNMAST\" ORDER BY \"DM_DESGN_ID\"", "-Select-", cmbDesignation);
                    divstore.Visible = false;
                    Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);

                    if (txtuserID.Text != "")
                    {
                        LoadUserDetails(txtuserID.Text);
                        // btnSearch_Click(sender, e);
                        Create.Visible = false;
                        CreateUser.Visible = false;
                        if (cmbRole.SelectedValue == "2" || cmbRole.SelectedValue == "5")
                        {
                            divcircle.Visible = false;
                            // divzone.Visible = false;
                            divstore.Visible = true;
                        }
                        else if (cmbRole.SelectedValue == "22")
                        {
                            divcircle.Visible = false;
                            divstore.Visible = false;

                        }
                        else
                        {
                            divcircle.Visible = true;
                            // divzone.Visible = true;
                            divstore.Visible = false;

                        }

                        Update.Visible = true;
                        UpdateUser.Visible = true;
                    }

                    string strQry = string.Empty;
                    strQry = "Title=Search and Select Office Details&";
                    strQry += "Query= select * from(select COALESCE(\"OFF_CODE\",-1) \"OFF_CODE\",LTRIM(SUBSTR(\"OFF_NAME\",POSITION(':' IN \"OFF_NAME\")+1,LENGTH(\"OFF_NAME\"))) AS \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\")A  where  {0} like %{1}% order by \"OFF_NAME\"&";
                    strQry += "DBColName=CAST(\"OFF_CODE\" AS TEXT)~CAST(\"OFF_NAME\" AS TEXT)&";
                    strQry += "ColDisplayName=OFF_CODE~OFF_NAME&";
                    strQry = strQry.Replace("'", @"\'");
                    strQry = strQry.Replace("+", @"8TT8");

                    //btnSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtOffCode.ClientID + "&btn=" + btnSearch.ClientID + "',520,520," + txtOffCode.ClientID + ")");

                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbZone.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);
                    cmbDiv.Items.Clear();
                    cmbSubDiv.Items.Clear();
                    cmbOMSection.Items.Clear();
                }

                else
                {
                    cmbCircle.Items.Clear();
                    cmbDiv.Items.Clear();
                    cmbSubDiv.Items.Clear();
                    cmbOMSection.Items.Clear();

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCircle.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"DIV_CICLE_CODE\"='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);
                    cmbSubDiv.Items.Clear();
                    cmbOMSection.Items.Clear();
                }

                else
                {
                    cmbDiv.Items.Clear();
                    cmbSubDiv.Items.Clear();
                    cmbOMSection.Items.Clear();

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDiv.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"SD_SUBDIV_CODE\",\"SD_SUBDIV_NAME\" from \"TBLSUBDIVMAST\" WHERE \"SD_DIV_CODE\"='" + cmbDiv.SelectedValue + "'", "--Select--", cmbSubDiv);
                    cmbOMSection.Items.Clear();
                }
                else
                {
                    cmbSubDiv.Items.Clear();
                    cmbOMSection.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbSubDiv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSubDiv.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"OM_CODE\",\"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE \"OM_SUBDIV_CODE\"='" + cmbSubDiv.SelectedValue + "'", "--Select--", cmbOMSection);
                }
                else
                {
                    cmbOMSection.Items.Clear();
                }
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
            try
            {


                if (txtFullName.Text.Trim().Length == 0)
                {
                    txtFullName.Focus();
                    ShowMsgBox("Enter Full Name");
                    return bValidate;
                }
                if (txtFullName.Text.Trim().StartsWith("."))
                {
                    txtFullName.Focus();
                    ShowMsgBox("Full Name not Start with DOT(.)");
                    return bValidate;
                }
                //if (!System.Text.RegularExpressions.Regex.IsMatch(txtFullName.Text, "^\\s*[a-zA-Z0-9]+\\s*[a-zA-Z0-9_.+-]\\s*$"))
                //{
                //    ShowMsgBox("Please enter Valid Full name ");
                //    return bValidate;
                //}
                if (txtLoginName.Text.Trim().Length == 0)
                {
                    txtLoginName.Focus();
                    ShowMsgBox("Enter Login Name");
                    return bValidate;
                }
                if (txtLoginName.Text.Trim().StartsWith("."))
                {
                    txtLoginName.Focus();
                    ShowMsgBox("Login Name not Start with DOT(.)");
                    return bValidate;
                }
                //if (!System.Text.RegularExpressions.Regex.IsMatch(txtLoginName.Text, "^\\s*[a-zA-Z0-9]+\\s*[a-zA-Z0-9_.+-]\\s*$"))
                //{
                //    ShowMsgBox("Please enter Valid Login name ");
                //    return bValidate;
                //}

                if (cmbRole.SelectedValue == "2" || cmbRole.SelectedValue == "5")
                {

                    if (cmbStore.SelectedIndex == 0)
                    {
                        ShowMsgBox("Select The Store");
                        return bValidate;
                    }
                }


                else if (cmbRole.SelectedValue != "2" && cmbRole.SelectedValue != "22" || cmbRole.SelectedValue != "5" && cmbRole.SelectedValue != "22")
                {
                    if (cmbRole.SelectedValue == "11" || cmbRole.SelectedValue == "8" || cmbRole.SelectedValue == "22" || cmbRole.SelectedValue == "38" )
                    {

                    }
                    else if (cmbZone.SelectedIndex == 0 && cmbCircle.SelectedIndex == -1 && cmbDiv.SelectedIndex == -1 && cmbSubDiv.SelectedIndex == -1 && cmbOMSection.SelectedIndex == -1)
                    {
                        ShowMsgBox("Select The Office code");
                        return bValidate;
                    }
                }


                //if (txtOffCode.Text.Trim().Length == 0)
                //{

                //    ShowMsgBox("Enter Office Code");
                //    return bValidate;
                //}
                //if (txtOfficeName.Text.Trim().Length == 0)
                //{
                //    txtOffCode.Focus();
                //    ShowMsgBox("Select Valid Office Code");
                //    return bValidate;
                //}
                if (txtEmailId.Text.Trim() == "")
                {
                    txtEmailId.Focus();
                    ShowMsgBox("Enter Email Id");
                    return bValidate;
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmailId.Text, "^\\s*[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+[a-zA-Z0-9]\\s*$"))
                {
                    txtEmailId.Focus();
                    ShowMsgBox("Please enter Valid Email (xyz@aaa.com)");
                    return bValidate;
                }
                if (txtMobile.Text.Trim().Length == 0)
                {
                    txtMobile.Focus();
                    ShowMsgBox("Enter Mobile Number");
                    return bValidate;
                }
                if (txtMobile.Text.Length < 10)
                {
                    ShowMsgBox("Enter Valid Mobile Number");
                    txtMobile.Focus();
                    return bValidate;
                }
                if (txtPhone.Text.Trim().Length != 0)
                {
                    if (txtPhone.Text.Trim().Length < 11)
                    {
                        ShowMsgBox("Enter Valid Phone Number");
                        txtPhone.Focus();
                        return bValidate;
                    }
                    if ((txtPhone.Text.Length - txtPhone.Text.Replace("-", "").Length) >= 2)
                    {
                        txtPhone.Focus();
                        ShowMsgBox("You cannot use more than one hyphen (-)");
                        return bValidate;
                    }
                    if (txtPhone.Text.Contains(".") == true)
                    {
                        txtPhone.Focus();
                        ShowMsgBox("You cannot enter (.) in Phone Number");
                        return bValidate;
                    }

                }

                if (cmbRole.SelectedIndex == 0)
                {
                    cmbRole.Focus();
                    ShowMsgBox("Enter Role");
                    return bValidate;
                }
                if (cmbDesignation.SelectedIndex == 0)
                {
                    cmbDesignation.Focus();
                    ShowMsgBox("Enter Designation");
                    return bValidate;
                }
                if (txtuserID.Text != "")
                {

                }
                else if (txtuserID.Text == "")
                {

                    if (txtPassword.Text.Length == 0)
                    {
                        txtPassword.Focus();
                        ShowMsgBox("Enter Password");
                        return bValidate;
                    }
                }

                if (txtAddress.Text.Trim().Length == 0)
                {
                    ShowMsgBox("Enter Address");
                    txtAddress.Focus();
                    return bValidate;
                }
                //if (!System.Text.RegularExpressions.Regex.IsMatch(txtAddress.Text, "^\\s*[a-zA-Z0-9]+\\s*[a-zA-Z0-9_.+-]\\s*$"))
                //{
                //    ShowMsgBox("Please enter Valid address ");
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

        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("UserView.aspx", false);
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
                    clsUser objUser = new clsUser();
                    string[] Arr = new string[2];
                    Byte[] Buffer;

                    string strofficecode = GetOfficeID();

                    objUser.lSlNo = txtuserID.Text;

                    objUser.sOfficeCode = strofficecode;
                    objUser.sFullName = txtFullName.Text;
                    objUser.sLoginName = txtLoginName.Text;
                    objUser.sPassword = txtPassword.Text;
                    objUser.sRole = cmbRole.SelectedValue;
                    if (objUser.sRole == "2" || objUser.sRole == "5")
                    {
                        objUser.sOfficeCode = cmbStore.SelectedValue;
                    }
                    if (objUser.sRole == "22")
                    {
                        objUser.sOfficeCode = "";
                    }
                    objUser.sEmail = txtEmailId.Text.ToLower();
                    objUser.sMobileNo = txtMobile.Text;
                    objUser.sPhoneNo = txtPhone.Text;
                    objUser.sAddress = txtAddress.Text;
                    objUser.sCrby = objSession.UserId;
                    objUser.sDesignation = cmbDesignation.SelectedValue;

                    if (fupSign.PostedFile.ContentLength != 0)
                    {
                        string filename = Path.GetFileName(fupSign.PostedFile.FileName);
                        string strExt = filename.Substring(filename.LastIndexOf('.') + 1);
                        if (strExt.ToLower().Equals("jpg") || strExt.ToLower().Equals("jpeg") || strExt.ToLower().Equals("png") || strExt.ToLower().Equals("gif"))
                        {
                            Stream strm = fupSign.PostedFile.InputStream;
                            Buffer = new byte[strm.Length];
                            strm.Read(Buffer, 0, (int)strm.Length);
                            objUser.sSignImage = Buffer;
                        }
                        else
                        {
                            lblMessage.Text = "Invalid Image File";
                            return;
                        }
                    }
                    else
                    {
                        Stream strm = fupSign.PostedFile.InputStream;
                        Buffer = new byte[strm.Length];
                        strm.Read(Buffer, 0, (int)strm.Length);
                        objUser.sSignImage = Buffer;
                    }

                    Arr = objUser.SaveUpdateUserDetails(objUser);

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, "User Create Master");
                    }

                    if (Arr[1].ToString() == "0")
                    {
                        ShowMsgBox("Saved Successfully");
                        txtuserID.Text = Arr[0].ToString();
                        cmdSave.Text = "Update";
                        return;
                    }
                    if (Arr[1].ToString() == "1")
                    {
                        ShowMsgBox(Arr[0]);
                        cmdSave.Text = "Update";
                        txtuserID.Text = Convert.ToString(objUser.lSlNo);
                        return;
                    }
                    if (Arr[1].ToString() == "4")
                    {
                        ShowMsgBox(Arr[0]);
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

        public string GetOfficeID()
        {
            string strOfficeId = string.Empty;
            if (cmbZone.SelectedIndex > 0)
            {
                strOfficeId = cmbZone.SelectedValue.ToString();
            }
            if (cmbCircle.SelectedIndex > 0)
            {
                strOfficeId = cmbCircle.SelectedValue.ToString();
            }

            if (cmbDiv.SelectedIndex > 0)
            {
                strOfficeId = cmbDiv.SelectedValue.ToString();
            }

            if (cmbSubDiv.SelectedIndex > 0)
            {
                strOfficeId = cmbSubDiv.SelectedValue.ToString();
            }
            if (cmbOMSection.SelectedIndex > 0)
            {
                strOfficeId = cmbOMSection.SelectedValue.ToString();
            }

            return (strOfficeId);
        }

        protected void cmdReset_Click(object sender, EventArgs e)
        {
            Reset();
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

        public void Reset()
        {
            try
            {
                //txtOffCode.Text = string.Empty;
                txtFullName.Text = string.Empty;
                txtLoginName.Text = string.Empty;
                txtPassword.Text = string.Empty;
                cmbRole.SelectedIndex = 0;

                txtEmailId.Text = string.Empty;
                txtMobile.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtAddress.Text = string.Empty;
                txtuserID.Text = string.Empty;
                cmdSave.Text = "Save";
                cmbDesignation.SelectedIndex = 0;
                txtPassword.Attributes.Add("Value", "");
                txtPassword.Attributes.Remove("readonly");
                txtLoginName.Enabled = true;
                //txtOfficeName.Text = string.Empty;
                cmbZone.SelectedIndex = 0;
                cmbCircle.Items.Clear();
                cmbDiv.Items.Clear();
                cmbSubDiv.Items.Clear();
                cmbOMSection.Items.Clear();
                cmbStore.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }



        public void LoadUserDetails(string strId)
        {
            try
            {
                clsUser objUser = new clsUser();
                string stroffCode = string.Empty;
                objUser.lSlNo = strId;

                objUser.GetUserDetails(objUser);
                //string strofficecode = GetOfficeID();
                txtuserID.Text = objUser.lSlNo;
                //txtOffCode.Text = strofficecode;
                txtFullName.Text = objUser.sFullName;
                txtLoginName.Text = objUser.sLoginName;
                txtPassword.Text = objUser.sPassword;
                txtPassword.Visible = false;
                lblpwd.Visible = false;
                cnttxt.Visible = false;
                // txtPassword.Attributes.Add("Value", Genaral.Decrypt(objUser.sPassword));
                cmbRole.SelectedValue = objUser.sRole;
                txtEmailId.Text = objUser.sEmail;
                txtMobile.Text = objUser.sMobileNo;
                txtPhone.Text = objUser.sPhoneNo;
                txtAddress.Text = objUser.sAddress;
                cmbDesignation.SelectedValue = objUser.sDesignation;
                cmdSave.Text = "Update";
                if (objSession.OfficeCode == "")
                {
                    //  txtPassword.Attributes.Add("readonly", "readonly");

                    // txtLoginName.Enabled = false;
                }
                else
                {
                    txtPassword.Attributes.Add("readonly", "readonly");

                    txtLoginName.Enabled = false;
                }
                if (objUser.sRoleType == "2")
                {
                    Genaral.Load_Combo("SELECT \"SM_ID\",\"SM_NAME\" FROM \"TBLSTOREMAST\" ORDER BY \"SM_ID\"", "-Select-", cmbStore);
                    cmbStore.Items.FindByValue(objUser.sOfficeCode).Selected = true;
                }
                else if (objUser.sRoleType == "1")
                {

                    if (objUser.sOfficeCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                        stroffCode = objUser.sOfficeCode.Substring(0, Zone_code);
                        cmbZone.Items.FindByValue(stroffCode).Selected = true;
                        stroffCode = string.Empty;
                        stroffCode = objUser.sOfficeCode;
                    }

                    //if (stroffCode == null || stroffCode == "" || stroffCode.Length == 1)
                    //{
                    //    Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                    //}
                    if (stroffCode.Length > 1)
                    {
                        Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);
                        if (stroffCode.Length >= 2)
                        {
                            stroffCode = stroffCode.Substring(0, Circle_code);
                            cmbCircle.Items.FindByValue(stroffCode).Selected = true;
                            stroffCode = string.Empty;
                            stroffCode = objUser.sOfficeCode;
                        }
                    }

                    if (stroffCode.Length > 2)
                    {
                        Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"DIV_CICLE_CODE\"='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);
                        if (stroffCode.Length >= 3)
                        {
                            stroffCode = stroffCode.Substring(0, Division_code);
                            cmbDiv.Items.FindByValue(stroffCode).Selected = true;
                            stroffCode = string.Empty;
                            stroffCode = objUser.sOfficeCode;
                        }
                    }
                    if (stroffCode.Length > 3)
                    {
                        Genaral.Load_Combo("SELECT \"SD_SUBDIV_CODE\",\"SD_SUBDIV_NAME\" from \"TBLSUBDIVMAST\" WHERE \"SD_DIV_CODE\"='" + cmbDiv.SelectedValue + "'", "--Select--", cmbSubDiv);
                        if (stroffCode.Length >= 4)
                        {
                            stroffCode = stroffCode.Substring(0, SubDiv_code);
                            cmbSubDiv.Items.FindByValue(stroffCode).Selected = true;
                            stroffCode = string.Empty;
                            stroffCode = objUser.sOfficeCode;
                        }
                    }
                    if (stroffCode.Length > 4)
                    {
                        Genaral.Load_Combo("SELECT \"OM_CODE\",\"OM_NAME\" FROM \"TBLOMSECMAST\" WHERE \"OM_SUBDIV_CODE\"='" + cmbSubDiv.SelectedValue + "'", "--Select--", cmbOMSection);
                        if (stroffCode.Length >= 5)
                        {
                            stroffCode = stroffCode.Substring(0, Section_code);
                            cmbOMSection.Items.FindByValue(stroffCode).Selected = true;
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

        public void RetainImageOnPostback()
        {
            try
            {
                string sDirectory = string.Empty;
                string sSSPlateFileName = string.Empty;
                string sNamePlateFileName = string.Empty;

                if (fupSign.HasFile)
                {
                    sSSPlateFileName = Path.GetFileName(fupSign.PostedFile.FileName);
                    fupSign.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + sSSPlateFileName));
                    txtSignImagePath.Text = Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + sSSPlateFileName);

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

                objApproval.sFormName = "UserCreate";
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

        protected void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                clsUser objuser = new clsUser();
                string sRoleType = objuser.GetRoleType(cmbRole.SelectedValue);
                //STO OR SK
                if (cmbRole.SelectedValue == "2" || cmbRole.SelectedValue == "5")
                {
                    divcircle.Visible = false;
                    // divzone.Visible = false;
                    divstore.Visible = true;
                }
                else if (sRoleType == "3")
                {
                    divcircle.Visible = false;
                    divstore.Visible = false;
                }
                else
                {
                    divcircle.Visible = true;
                    // divzone.Visible = true;
                    divstore.Visible = false;

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        clsLogin objLogin = new clsLogin();

        //        if (txtOffCode.Text == "-1")
        //        {
        //            txtOffCode.Text = "";
        //        }
        //        txtOfficeName.Text = objLogin.Getofficename(txtOffCode.Text);

        //        //if (txtOffCode.Text == "0")
        //        //{
        //        //    txtOffCode.Text = "";
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = clsException.ErrorMsg();
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "btnSearch_Click");
        //    }
        //}

    }
}