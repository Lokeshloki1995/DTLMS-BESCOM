using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.IO;
using System.Data;
using System.Configuration;
using System.Net.Mail;

namespace IIITS.DTLMS
{
    public partial class Login : System.Web.UI.Page
    {
        string strFormCode = "Login";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = string.Empty;
                Form.DefaultButton = cmdLogin.UniqueID;

                if ((Session["clsSession"] != null))
                {
                    Response.Redirect("Dashboard.aspx", false);
                }
                if (!IsPostBack)
                {
                    ViewState["OTP"] = null;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {

                clsLogin objLogin = new clsLogin();
                clsSession objSession = new clsSession();
                DataTable dtConfiguration = new DataTable();
                objLogin.sLoginName = txtUsername.Text.Trim().ToUpper();
                objLogin.sPassword = txtPassword.Text.Trim();

                if (ValidateForm() == true)
                {
                    objLogin.UserLogin(objLogin);

                    if (objLogin.sMessage == null)
                    {
                        Session["FullName"] = objLogin.sFullName;
                        Session["ChangPwd"] = objLogin.sChangePwd;
                        Session["sOfficeCode"] = objLogin.sOfficeCode;
                        if (objLogin.sOfficeCode == "0")
                        {
                            objLogin.sOfficeCode = "";
                        }
                        objSession.UserId = objLogin.sUserId;
                        objSession.FullName = objLogin.sFullName;
                        objSession.RoleId = objLogin.sRoleId;
                        objSession.OfficeCode = objLogin.sOfficeCode;
                        objSession.OfficeName = objLogin.sOfficeNamewithType;
                        objSession.Designation = objLogin.sDesignation;
                        objSession.OfficeNameWithType = objLogin.sOfficeNamewithType;
                        objSession.sRoleType = objLogin.sRoleType;
                        objLogin.sEmail = objLogin.sEmail;
                        objSession.sClientIP = Genaral.WorkFlowObjects();

                        //String sStoreId = clsStoreOffice.GetStoreID(objLogin.sOfficeCode);
                        String sStoreId = objLogin.sOfficeCode;

                        if (objSession.sRoleType == "1")
                        {
                            objSession.sStoreID = sStoreId;
                        }
                        else
                        {
                            objSession.sStoreID = objLogin.sOfficeCode;
                        }


                        dtConfiguration = objLogin.GetConfiguration();
                        //objSession.sGeneralLog = Convert.ToString(dtConfiguration.Rows[0]["CG_GEN_LOG"]);
                        objSession.sTransactionLog = Convert.ToString(dtConfiguration.Rows[0]["CG_TRANS_LOG"]);


                        objLogin = objLogin.PasswordExpiryCheck(objLogin);

                        if (objLogin.LoginExpireCheck == "-1" && objLogin.LoginUserType == "2")//2 for External Employee Like CESC Employees
                        {
                            if (objSession.sGeneralLog == "1") // Login Logout Log
                            {
                                Genaral.GeneralLog(objSession.sClientIP, objSession.UserId, "LOGIN");
                            }
                            Session["ChangPwd"] = "";
                            Session["clsSession"] = objSession;
                            Response.Redirect("~/MasterForms/ChangePassword.aspx", false);
                        }
                        else if (objLogin.LoginExpireCheck == "-1" && objLogin.LoginUserType == "1")//1 for internal operators 
                        {
                            if (objLogin.sEmail != "" && objLogin.sEmail != null)
                            {
                                double MailExpiryTime = Convert.ToDouble(ConfigurationSettings.AppSettings["MAILSESSIONTIME"]);
                                objLogin.MailSessionExpiryTime = Convert.ToString(MailExpiryTime);
                                objLogin = objLogin.SaveEmailAndPassWordExpireLog(objLogin);

                                if (objLogin.SubmitClick > 0)
                                {
                                    SendMail(objLogin);
                                }
                                lblMsg.Text = objLogin.PwdMessage;
                            }
                            else
                            {
                                lblMsg.Text = "Update Your Mail Id to get Change Password Link";
                            }
                        }

                        else
                        {

                            //if (dtConfiguration.Rows.Count > 0)
                            //{
                            //    if (objSession.sGeneralLog == "1") // Login Logout Log
                            //    {
                            //        Genaral.GeneralLog(objSession.sClientIP, objSession.UserId, "LOGIN");
                            //    }
                            //    if (objSession.sPasswordChangeRequest == "1") // check password by days
                            //    {
                            //        string numberOfDays = objLogin.GetPasswordDetails(objSession.UserId);
                            //        if (numberOfDays != null && numberOfDays != "")
                            //        {
                            //            if (Convert.ToInt32(numberOfDays) > Convert.ToInt32(objSession.sPasswordChangeInDays))
                            //            {
                            //                Session["ChangPwd"] = "";
                            //                Response.Redirect("~/MasterForms/ChangePassword.aspx", false);
                            //            }
                            //        }
                            //    }
                            //}


                            Session["clsSession"] = objSession;
                            //  string guid = Guid.NewGuid().ToString();
                            //  Session["AuthToken"] = guid;
                            // now create a new cookie with this guid value
                            //  Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                            if (Session["ChangPwd"] == null || Session["ChangPwd"].ToString() == "")
                            {
                                Response.Redirect("~/MasterForms/ChangePassword.aspx", false);
                            }
                            else if (objSession.sRoleType == "1" || objSession.sRoleType == "3")
                            {
                                Response.Redirect("Dashboard.aspx", false);
                            }
                            else
                            {
                                Response.Redirect("StoreDashboard.aspx", false);
                                //Response.Redirect("~/MasterForms/ChangePassword.aspx", false);
                            }
                            //clsAprovalHistory obj = new clsAprovalHistory();
                            //obj.LoadApprovalFullHistory("20", "12");
                            //HttpUtility.UrlEncode(General.Encrypt(e.Item.Cells(0).Text))
                            //General.Decrypt(HttpUtility.UrlDecode(Request.QueryString("EmpId")))

                        }
                    }
                    else
                    {
                        lblMsg.Text = objLogin.sMessage;
                    }
                }



            }
            catch (Exception ex)
            {
                Genaral.GeneralLog("Error in login :" + ex.Message, ex.StackTrace, "LOGIN");
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public string SendMail(clsLogin login)
        {
            List<string> uniques = login.sEmail.Split(',').Reverse().Take(100).Distinct().Reverse().ToList();
            login.sEmail = string.Join(",", uniques);
            Console.WriteLine(login.sEmail);
            double MailExpiryTime = Convert.ToDouble(ConfigurationSettings.AppSettings["MAILSESSIONTIME"]);
            string DomainIP = Convert.ToString(ConfigurationSettings.AppSettings["DomainIP"]);

            string strbody = string.Empty;
            DateTime dtTodayDate = DateTime.Now;
            string strTodayFormat = dtTodayDate.ToString("dd-MMM-yyyy HH:mm");

            DateTime dtExpireDate = DateTime.Now.AddMinutes(MailExpiryTime);
            string strExpireFormat = dtExpireDate.ToString("dd-MMM-yyyy HH:mm");
            strbody = "<html>";
            strbody += "<head > </head>";
            strbody += "<body> Dear " + login.sFullName + ",<br>";
            strbody += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; As per organization's password policy and guidelines, your password for the application DTLMS has expired on " + strTodayFormat + ". Please Click on <b><a style='font-weight:bold;color:blue'href='" + DomainIP + "/MasterForms/ChangePwdInternal.aspx?pkid=" + HttpUtility.UrlEncode(Genaral.UrlEncrypt(Convert.ToString(login.SubmitClick))) + "'> Link </a></b> to reset your password.";


            strbody += "<p> <span style='font-weight:bold'>Note:</span> This link will expire by <span>" + strExpireFormat + " </span></p>";

            strbody += "</p> <p> Regards, <br> Team IT-Admin </p> </body> </html>";
            try
            {
                if (Convert.ToString(ConfigurationSettings.AppSettings["SENDEMAIL"]).ToUpper().Equals("ON"))
                {
                    string sendpwd = Convert.ToString(ConfigurationSettings.AppSettings["SENDPWD"]);
                    string sendmailid = Convert.ToString(ConfigurationSettings.AppSettings["SENDMAILID"]);
                    MailMessage mail = new MailMessage();
                    mail.To.Add(login.sEmail);
                    mail.From = new MailAddress(sendmailid, "Idea Infinity");
                    mail.Subject = "Change Password - DTLMS - " + login.sFullName + "";
                    mail.IsBodyHtml = true;
                    string Body = strbody;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    var varbody = AlternateView.CreateAlternateViewFromString(Body, new System.Net.Mime.ContentType("text/html"));
                    mail.AlternateViews.Add(varbody);

                    //   SmtpClient smtp = new SmtpClient("smtp.bizmail.yahoo.com", 587);
                    SmtpClient smtp = new SmtpClient(Convert.ToString(ConfigurationSettings.AppSettings["SENDSMTP"]), Convert.ToInt32(ConfigurationSettings.AppSettings["SENDSMTPPORT"]));
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    //smtp.Credentials = new System.Net.NetworkCredential("bescomdtlms@ideainfinityit.com", "ctwubdqrhpphfclz");
                    smtp.Credentials = new System.Net.NetworkCredential(Convert.ToString(ConfigurationSettings.AppSettings["SENDMAILID"]), Convert.ToString(ConfigurationSettings.AppSettings["SENDPWD"]));
                    smtp.EnableSsl = true;
                    smtp.Timeout = 500000;
                    smtp.Send(mail);
                    mail.Dispose();

                }
            }
            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return "";
        }
        public bool ValidateForm()
        {
            bool bValidate = true;
            try
            {
                if (txtUsername.Text == "" || txtUsername.Text == null)
                {
                    if (!txtEmail.Text.Contains("@"))
                    {
                        if (txtEmail.Text.Length == 10)
                        {
                            int Mob_First_Digit = Convert.ToInt32(txtEmail.Text.Substring(0, 1));
                            if (Mob_First_Digit <= 6)
                            {
                                txtEmail.Focus();
                                ShowMsgBox("Please Enter Valid Mobile Number");
                                return bValidate;
                            }
                        }
                        else
                        {
                            txtEmail.Focus();
                            ShowMsgBox("Please Enter Valid 10 Digit Mobile Number");
                            return bValidate;
                        }
                    }
                    else
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, "^\\s*[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+[a-zA-Z0-9]\\s*$"))
                        {
                            txtEmail.Focus();
                            ShowMsgBox("Please enter Valid Email (xyz@aaa.com)");
                            return bValidate;
                        }
                    }

                    if (txtEmail.Text == "")
                    {
                        ShowMsgBox("Please Enter Register Mail Id / Mobile number to get OTP");
                        txtEmail.Focus();
                    }
                }



                bValidate = true;
                return bValidate;
            }
            catch (Exception ex)
            {
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return bValidate;
            }
        }

        private void ShowMsgBox(string sMsg)
        {
            string sShowMsg = string.Empty;
            try
            {
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                this.Page.RegisterStartupScript("Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //protected void cmdFSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        clsLogin objLogin = new clsLogin();
        //        objLogin.sEmail = txtEmail.Text;
        //        objLogin.ForgtPassword(objLogin);
        //        if (objLogin.sMessage == null)
        //        {
        //            string sPattern = @"^\d{10}$";
        //            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(sPattern);
        //            if (r.IsMatch(objLogin.sEmail))
        //            {
        //                ShowMsgBox("Password has been sent to your Mobile Number");
        //            }
        //            else
        //            {
        //                ShowMsgBox("Password has been sent to your Registered Email ID");
        //            }

        //            //lblFMsg1.Text = "Password has been sent to your Registered Email ID";
        //            cmdFSave.Enabled = false;
        //            dvForgtPwd.Visible = true;
        //        }
        //        else
        //        {
        //            ShowMsgBox(objLogin.sMessage);
        //            dvForgtPwd.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMsg.Text = clsException.ErrorMsg();
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "cmdFSave_Click");
        //    }
        //}

        protected void cmdFSave_Click(object sender, EventArgs e)
        {
            try
            {
                clsLogin objLogin = new clsLogin();
                objLogin.sEmail = txtEmail.Text;

                if (ValidateForm() == true)
                {
                    objLogin.ForgtPassword(objLogin);

                    if (objLogin.sResult == "1")
                    {
                        ShowMsgBox(objLogin.sMessage);
                    }
                    else
                    {
                        if (objLogin.sMessage != null)
                        {
                            string sPattern = @"^\d{10}$";
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(sPattern);
                            if (r.IsMatch(objLogin.sEmail))
                            {
                                ShowMsgBox("OTP has been sent to your Mobile Number");
                            }
                            else
                            {
                                ShowMsgBox("OPT has been sent to your Registered Email ID");
                            }

                            //lblFMsg1.Text = "Password has been sent to your Registered Email ID";
                            cmdFSave.Enabled = true;
                            //dvForgtPwd.Visible = false;
                            //dvResetPwd.Visible = false;
                            Form2.Visible = true;
                            ResetPwd.Visible = true;
                        }
                        else
                        {
                            ShowMsgBox(objLogin.sMessage);
                            //dvForgtPwd.Visible = true;
                            //dvResetPwd.Visible = false;
                            Form2.Visible = true;
                            ResetPwd.Visible = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void btnResetPwd_Click(object sender, EventArgs e)
        {

            try
            {
                DataTable dtOtpDetails = new DataTable();
                DataTable dtConfiguration = new DataTable();
                string[] Arr = new string[2];
                if (txtNewpwd.Text == txtCnfrmPwd.Text)
                {
                    clsLogin objOTPDetails = new clsLogin();
                    dtOtpDetails = objOTPDetails.GetOTPDetails(txtOTP.Text);
                    dtConfiguration = objOTPDetails.GetConfiguration();

                    if (Convert.ToString(dtConfiguration.Rows[0]["CG_PRE_PASS_ACCEPTANCE"]) == "1")
                    {
                        //bool res = objOTPDetails.GetStatus(Genaral.Encrypt(txtCnfrmPwd.Text), Convert.ToString(dtOtpDetails.Rows[0]["otp_us_id"]));
                        //if (res == false)
                        //{
                        //    ShowMsgBox("Password Should not be same as old");
                        //    return;
                        //}
                    }

                    else
                    {
                        if (dtOtpDetails.Rows.Count > 0)
                        {
                            if (dtOtpDetails.Rows[0]["otp_no"].ToString() == txtOTP.Text)
                            {

                                clsUser objUser = new clsUser();
                                objUser.sPassword = txtCnfrmPwd.Text;
                                objUser.lSlNo = dtOtpDetails.Rows[0]["otp_us_id"].ToString();
                                objUser.sOTP = dtOtpDetails.Rows[0]["otp_no"].ToString();

                                string sClientIP = Genaral.WorkFlowObjects();

                                Genaral.PasswordChangeLog(sClientIP, Convert.ToString(dtOtpDetails.Rows[0]["otp_us_id"]));

                                Arr = objUser.UpdatePwd(objUser);
                                ShowMsgBox(Arr[1]);
                            }
                            else
                            {
                                ShowMsgBox("Your OTP Expired Please Generate New OTP");
                                txtOTP.Text = string.Empty;
                                txtNewpwd.Text = string.Empty;
                                txtCnfrmPwd.Text = string.Empty;
                            }
                        }
                        else
                        {
                            ShowMsgBox("Wrong OTP Generate New OTP");
                            txtOTP.Text = string.Empty;
                            txtNewpwd.Text = string.Empty;
                            txtCnfrmPwd.Text = string.Empty;
                            //Response.Redirect("Login.aspx", false);
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

    }
}