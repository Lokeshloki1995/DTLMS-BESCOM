using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsLogin
    {
        public string sLoginName { get; set; }
        public string sFullName { get; set; }
        public string sOfficeCode { get; set; }
        public string sUserType { get; set; }
        public string sUserId { get; set; }
        public string sPassword { get; set; }
        public string sMessage { get; set; }
        public string sEmail { get; set; }
        public string sRoleId { get; set; }
        public string sOfficeName { get; set; }
        public string sDesignation { get; set; }
        public string sMobileNo { get; set; }
        public string sOfficeNamewithType { get; set; }
        public string sChangePwd { get; set; }
        public string sRoleType { get; set; }
        public string sOTP { get; set; }
        public string sResult { get; set; }

        public string LoginUserType { set; get; }
        public string LoginExpireCheck { set; get; }
        public string PwdMessage { set; get; }

        public string MailSessionExpiryTime { set; get; }

        public int SubmitClick { set; get; }
        public int SMSDumppkId { set; get; }


        string strFormCode = "clsLogin";
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);


        public static string Encrypt(string pwd)
        {
            //this will return the encrypted string
            int n, i;
            string temp;
            temp = "";
            n = pwd.Length;
            for (i = 0; i < n; i++)
            {
                temp = temp + (char)((int)pwd[i] + 123);
            }
            //temp[i]='\0';
            return (temp);
        }

        NpgsqlCommand NpgsqlCommand;

        // to get encrypted password 
        public string Encryptmtd(string pwd)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[pwd.Length];
            encode = Encoding.UTF8.GetBytes(pwd);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        public clsLogin UserLogin(clsLogin objLogin)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {

                string sQry = string.Empty;
                DataTable dt = new DataTable();
                bool bActiveResult = false;
              //  for password encrypt
                 string pswd = Encryptmtd("Jan@2024");
                string decptpswd = Genaral.DecryptPassword("KjXl2KTbzdQrtAiOopLPfGqj6PSjVp2V7wyINXdIhwqhf0iI");
                //   string pswd = Encryptmtd("jul@2022+");

                sQry = "SELECT \"US_ID\",\"US_FULL_NAME\",\"US_OFFICE_CODE\",\"US_ROLE_ID\",TO_CHAR(\"US_EFFECT_FROM\",'DD/MM/YYYY') US_EFFECT_FROM,\"US_STATUS\",\"US_PWD\",";
                sQry += " \"US_CHPWD_ON\",(SELECT \"DM_NAME\" FROM \"TBLDESIGNMAST\" WHERE \"DM_DESGN_ID\"=\"US_DESG_ID\") DM_NAME,(SELECT \"RO_TYPE\" FROM \"TBLROLES\" WHERE \"RO_ID\"=\"US_ROLE_ID\")RO_TYPE,\"US_EMAIL\"";
                sQry += " FROM \"TBLUSER\" ";

                sQry += " WHERE UPPER(\"US_LG_NAME\")=:LoginName ";
                NpgsqlCommand.Parameters.AddWithValue("LoginName", objLogin.sLoginName.ToUpper());

                dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);

                bool Passwordstatus = false;
                if (dt.Rows.Count > 0)
                {
                    Passwordstatus = Genaral.CompareLogin(Convert.ToString(dt.Rows[0]["US_PWD"]), objLogin.sPassword);
                }
                if (Passwordstatus == false)
                {
                    dt = null;

                }


                if (dt != null)
                {
                    //Check for EffectFrom Condition
                    string sEffectFrom = Convert.ToString(dt.Rows[0]["US_EFFECT_FROM"]);
                    string sStatus = Convert.ToString(dt.Rows[0]["US_STATUS"]);
                    if (sEffectFrom != "" && sStatus == "D")
                    {
                        string sResult = Genaral.DateComparision(sEffectFrom, "", true, false);
                        if (sResult == "1")
                        {
                            bActiveResult = true;
                            sStatus = "A";
                        }
                    }

                    if (sStatus == "A" || bActiveResult == true)
                    {
                        objLogin.sUserId = dt.Rows[0]["us_id"].ToString();
                        objLogin.sFullName = dt.Rows[0]["US_FULL_NAME"].ToString();
                        objLogin.sOfficeCode = dt.Rows[0]["US_OFFICE_CODE"].ToString();
                        objLogin.sRoleType = dt.Rows[0]["RO_TYPE"].ToString();

                        objLogin.sRoleId = dt.Rows[0]["US_ROLE_ID"].ToString();
                        objLogin.sOfficeName = Getofficename(dt.Rows[0]["US_OFFICE_CODE"].ToString(), objLogin.sRoleType);
                        objLogin.sDesignation = dt.Rows[0]["DM_NAME"].ToString();
                        objLogin.sChangePwd = dt.Rows[0]["US_CHPWD_ON"].ToString();

                        if (dt.Columns.Contains("US_EMAIL"))
                        {
                            objLogin.sEmail = dt.Rows[0]["US_EMAIL"].ToString();
                        }
                        objLogin.sOfficeNamewithType = GetofficeNameWithType(objLogin.sOfficeCode, objLogin.sRoleType);

                        NpgsqlCommand.Parameters.AddWithValue("UserID", objLogin.sUserId);
                        sQry = "UPDATE tbluserloginattempt set ula_status=1 WHERE cast(ula_user_id as text)=:UserID and ula_status =0";
                        Objcon.ExecuteQry(sQry, NpgsqlCommand);

                    }
                    else
                    {
                        objLogin.sMessage = "User is Disabled,Please contact Administrator";
                    }

                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("LoginName1", objLogin.sLoginName.ToUpper());
                    string strQry = "SELECT \"US_ID\" FROM \"TBLUSER\" WHERE UPPER(\"US_LG_NAME\")=:LoginName1 AND \"US_STATUS\"='A'";
                    string userID = Objcon.get_value(strQry, NpgsqlCommand);
                    int loginattempts = 0;
                    int TotalAttempts = Convert.ToInt32(ConfigurationSettings.AppSettings["LoginAttempts"]);
                    int TotalSeconds = Convert.ToInt32(ConfigurationSettings.AppSettings["LoginAttemptsTimeRange"]);
                    int LoginAttemptsApply = Convert.ToInt32(ConfigurationSettings.AppSettings["LoginAttemptsApply"]);
                    int pendingAttempts = 0;
                    string res = string.Empty;
                    int dateDifference = 0;

                    if (userID != "" && userID != null)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("UserID1", Convert.ToInt16(userID));
                        strQry = "SELECT abs(round(extract(epoch from now() - ula_cron)))  FROM (SELECT ula_id,ula_cron,row_number() over (PARTITION by ula_user_id ORDER BY ula_attempts desc) FROM tbluserloginattempt WHERE ula_user_id=:UserID1 and ula_status = 0)A WHERE row_number=1";
                        res = Objcon.get_value(strQry, NpgsqlCommand);
                        if (res != null && res != "")
                        {
                            dateDifference = Convert.ToInt32(res);
                        }

                        if (dateDifference < TotalSeconds)
                        {
                            NpgsqlCommand.Parameters.AddWithValue("UserID2", Convert.ToInt16(userID));
                            strQry = "SELECT COALESCE(max(ula_attempts),0)+1 FROM tbluserloginattempt WHERE ula_user_id=:UserID2 AND ula_status =0";
                            loginattempts = Convert.ToInt32(Objcon.get_value(strQry, NpgsqlCommand));

                            NpgsqlCommand.Parameters.AddWithValue("Userid", Convert.ToInt16(userID));
                            NpgsqlCommand.Parameters.AddWithValue("loginattempt", Convert.ToInt16(loginattempts));
                            strQry = "INSERT into tbluserloginattempt (ula_id,ula_user_id,ula_attempts,ula_cron) VALUES((SELECT COALESCE(max(ula_id),0)+1 FROM tbluserloginattempt),";
                            strQry += " :Userid,:loginattempt,now())";
                            Objcon.ExecuteQry(strQry, NpgsqlCommand);
                        }
                        else
                        {
                            NpgsqlCommand.Parameters.AddWithValue("Userid1", Convert.ToInt16(userID));
                            strQry = "UPDATE tbluserloginattempt set ula_status=1 WHERE ula_user_id=:Userid1 and ula_status =0";
                            Objcon.ExecuteQry(strQry, NpgsqlCommand);

                            NpgsqlCommand.Parameters.AddWithValue("UserID12", Convert.ToInt16(userID));
                            strQry = "SELECT COALESCE(max(ula_attempts),0)+1 FROM tbluserloginattempt WHERE ula_user_id=:UserID12 AND ula_status =0";
                            loginattempts = Convert.ToInt32(Objcon.get_value(strQry, NpgsqlCommand));


                            NpgsqlCommand.Parameters.AddWithValue("Userid2", Convert.ToInt16(userID));
                            NpgsqlCommand.Parameters.AddWithValue("loginattempt1", Convert.ToInt16(loginattempts));
                            strQry = "INSERT into tbluserloginattempt (ula_id,ula_user_id,ula_attempts,ula_cron) VALUES((SELECT COALESCE(max(ula_id),0)+1 FROM tbluserloginattempt),";
                            strQry += " :Userid2,:loginattempt1,now())";
                            Objcon.ExecuteQry(strQry, NpgsqlCommand);
                        }
                    }
                    if (loginattempts != 0 && (loginattempts > LoginAttemptsApply && loginattempts <= (TotalAttempts - 1)))
                    {
                        pendingAttempts = TotalAttempts - loginattempts;
                        objLogin.sMessage = "You had " + pendingAttempts + " more Attempts left ,Enter Valid User Name and Password";
                    }
                    else if (loginattempts == TotalAttempts)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("UserID3", Convert.ToInt16(userID));
                        strQry = "UPDATE \"TBLUSER\" set \"US_STATUS\"='D' WHERE \"US_ID\"=:UserID3 AND \"US_STATUS\"='A'";
                        Objcon.ExecuteQry(strQry, NpgsqlCommand);


                        NpgsqlCommand.Parameters.AddWithValue("UserID4", Convert.ToInt16(userID));
                        strQry = "UPDATE tbluserloginattempt set ula_status=1 WHERE ula_user_id=:UserID4 and ula_status =0";
                        Objcon.ExecuteQry(strQry, NpgsqlCommand);

                        objLogin.sMessage = "Your Accout has been Locked, kindly contact DTLMS Support";
                    }
                    else
                    {
                        objLogin.sMessage = "Enter Valid User Name and Password";
                    }

                }
                return objLogin;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                objLogin.sMessage = "An exception occurred while processing your request.";
                return objLogin;
            }
        }

        public string Getofficename(string sOfficecode, string sRoleType)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                if (sRoleType == "1" || sRoleType == "3")
                {
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode", sOfficecode);
                    strQry = "SELECT split_part(\"OFF_NAME\", ':',2) AS OFFICENAME  FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT)=:OfficeCode";

                    string Offname = Objcon.get_value(strQry, NpgsqlCommand);

                    if (Offname == null || Offname == "")
                    {
                        Offname = "CORPORATE OFFICE";
                    }
                    return Offname;
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode1", sOfficecode);
                    strQry = "SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE CAST(\"SM_ID\" AS TEXT) =:OfficeCode1";
                    string Offname = Objcon.get_value(strQry, NpgsqlCommand);
                    return Offname;
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        public string GetofficeNameWithType(string sOfficecode, string sRoleType)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                if (sRoleType == "1" || sRoleType == "3")
                {
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode", sOfficecode);
                    strQry = "SELECT \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT) =:OfficeCode";

                    string Offname = Objcon.get_value(strQry, NpgsqlCommand);

                    if (Offname == null || Offname == "")
                    {
                        Offname = "CORPORATE OFFICE";
                    }
                    return Offname;
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode1", sOfficecode);
                    strQry = "SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE CAST(\"SM_ID\" AS TEXT) =:OfficeCode1";
                    string Offname = Objcon.get_value(strQry, NpgsqlCommand);
                    return Offname;
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }

        }
        public clsLogin ForgtPassword(clsLogin objLogin)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string sSMSText = String.Empty;

                System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex(@"^\d+$");
                string sPattern = @"^\d{10}$";
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(sPattern);
                if (r.IsMatch(objLogin.sEmail))
                {
                    NpgsqlCommand.Parameters.AddWithValue("Email", objLogin.sEmail);
                    sQry = "SELECT \"US_FULL_NAME\",\"US_LG_NAME\",\"US_PWD\",\"US_ID\",\"US_MOBILE_NO\" FROM \"TBLUSER\" where \"US_STATUS\"='A' AND CAST(\"US_MOBILE_NO\" AS TEXT)=:Email";
                    dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                    if (dt.Rows.Count > 0)
                    {
                        objLogin.sPassword = dt.Rows[0]["US_PWD"].ToString();
                        objLogin.sFullName = dt.Rows[0]["US_FULL_NAME"].ToString();
                        objLogin.sLoginName = dt.Rows[0]["US_LG_NAME"].ToString();
                        objLogin.sUserId = dt.Rows[0]["US_ID"].ToString();
                        objLogin.sMobileNo = dt.Rows[0]["US_MOBILE_NO"].ToString();
                        //char[] randomnumber = dt.Rows[0]["PRESENT_TIME"].ToString()
                        //SendMailForgotPwd(objLogin);

                        clsCommunication objComm = new clsCommunication();

                        Random generator = new Random();
                        int OTP = generator.Next(100000, 1234567);
                        Random random = new Random();
                        int num = random.Next(0, 26);
                        int num1 = random.Next(0, 26);
                        char let = (char)('A' + num);
                        char let1 = (char)('A' + num1);
                        objLogin.sOTP = Convert.ToString(OTP) + let + let1;

                        objLogin.sOTP = Shuffle(objLogin.sOTP);
                        //string 


                        objComm.sSMSkey = "SMStoOTP";
                        objComm = objComm.GetsmsTempalte(objComm);

                        if (objComm.sSMSTemplate != null)
                        {
                            sSMSText = String.Format(objComm.sSMSTemplate, objLogin.sFullName, objLogin.sOTP);
                            //File.AppendAllText("D:\\ERRORLOG\\SMS.text", "SendMail before  SendSMS ()");
                            //objComm.sendSMS(sSMSText, objLogin.sMobileNo, objLogin.sFullName);
                            //File.AppendAllText("D:\\ERRORLOG\\SMS.text", "SendMail After  SendSMS ()");
                            objLogin.sMessage = sSMSText;

                            //sQry = "SELECT otp_sent_flag FROM tblotp WHERE otp_us_id='"+ objLogin.sUserId + "'";
                            NpgsqlCommand.Parameters.AddWithValue("userId", Convert.ToInt16(objLogin.sUserId));
                            sQry = "SELECT to_char(otp_cron,'yyyy-MM-dd HH24:mi') || '~' || otp_sent_flag FROM (SELECT otp_cron,otp_sent_flag, otp_id,\"row_number\"() over(partition by otp_us_id ORDER BY ";
                            sQry += " otp_id desc) as rownum FROM tblotp WHERE otp_us_id=:userId)A WHERE rownum=1";
                            string sSentFlag = Objcon.get_value(sQry, NpgsqlCommand);

                            if (sSentFlag == "")
                            {
                                NpgsqlCommand.Parameters.AddWithValue("userId1", Convert.ToInt16(objLogin.sUserId));
                                NpgsqlCommand.Parameters.AddWithValue("OTP", objLogin.sOTP);
                                sQry = "INSERT INTO tblotp (otp_us_id,otp_no,otp_cron,otp_sent_flag,otp_change_pwd_on) VALUES(:userId1,:OTP,now(),'0',now())";
                                Objcon.ExecuteQry(sQry, NpgsqlCommand);

                                if (objComm.sSMSTemplateID != null && objComm.sSMSTemplateID != "")
                                {
                                    objComm.DumpSms(objLogin.sMobileNo, sSMSText, objComm.sSMSTemplateID, "WEB");
                                }
                            }
                            else
                            {
                                if (Convert.ToString(sSentFlag.Split('~').GetValue(1)) == "1" || Convert.ToString(sSentFlag.Split('~').GetValue(1)) == "")
                                {
                                    NpgsqlCommand.Parameters.AddWithValue("userId2", Convert.ToInt16(objLogin.sUserId));
                                    NpgsqlCommand.Parameters.AddWithValue("OTP1", objLogin.sOTP);

                                    sQry = "INSERT INTO tblotp (otp_us_id,otp_no,otp_cron,otp_sent_flag,otp_change_pwd_on) VALUES(:userId2,:OTP1,now(),'0',now())";
                                    Objcon.ExecuteQry(sQry, NpgsqlCommand);
                                    if (objComm.sSMSTemplateID != null && objComm.sSMSTemplateID != "")
                                    {
                                        objComm.DumpSms(objLogin.sMobileNo, sSMSText, objComm.sSMSTemplateID, "WEB");
                                    }
                                }
                                else
                                {
                                    DateTime PrevOTP_DATE = DateTime.ParseExact(Convert.ToString(sSentFlag.Split('~').GetValue(0)), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                                    DateTime Now_DATE = DateTime.Now;
                                    TimeSpan finalres = Now_DATE - PrevOTP_DATE;
                                    int iTotalSeconds = Convert.ToInt32(ConfigurationSettings.AppSettings["TotalSeconds"]);

                                    if (finalres.TotalSeconds < iTotalSeconds)
                                    {
                                        objLogin.sMessage = "OTP Already Sent to your mobile number please try later";
                                        objLogin.sResult = "1";
                                    }
                                    else
                                    {
                                        NpgsqlCommand.Parameters.AddWithValue("mobileNo", Convert.ToDouble(objLogin.sMobileNo));
                                        sQry = "SELECT \"TS_ID\" FROM \"TBLSMSDUMP\" WHERE \"TS_MOBILE_NUMBER\"=:mobileNo AND \"TS_CONTENT\" LIKE '%OTP%' AND \"TS_SENT_FLAG\"='0'";
                                        string sDump_id = Objcon.get_value(sQry, NpgsqlCommand);

                                        if (sDump_id != "" && sDump_id != null)
                                        {
                                            NpgsqlCommand.Parameters.AddWithValue("Dump_id", Convert.ToInt32(sDump_id));
                                            sQry = "UPDATE \"TBLSMSDUMP\" SET \"TS_OTP_FLAG\"='1',\"TS_SENT_FLAG\"='1' WHERE \"TS_ID\"=:Dump_id";
                                            Objcon.ExecuteQry(sQry, NpgsqlCommand);
                                        }

                                        NpgsqlCommand.Parameters.AddWithValue("UserId", Convert.ToInt16(objLogin.sUserId));
                                        sQry = "UPDATE tblotp set otp_cancel_flag='1',otp_sent_flag='1' WHERE otp_us_id=:UserId";
                                        Objcon.ExecuteQry(sQry, NpgsqlCommand);

                                        NpgsqlCommand.Parameters.AddWithValue("UserId1", Convert.ToInt16(objLogin.sUserId));
                                        NpgsqlCommand.Parameters.AddWithValue("OTP2", objLogin.sOTP);
                                        sQry = "INSERT INTO tblotp (otp_us_id,otp_no,otp_cron,otp_sent_flag,otp_change_pwd_on) VALUES(:UserId1,:OTP2,now(),'0',now())";
                                        Objcon.ExecuteQry(sQry, NpgsqlCommand);

                                        if (objComm.sSMSTemplateID != null && objComm.sSMSTemplateID != "")
                                        {
                                            objComm.DumpSms(objLogin.sMobileNo, sSMSText, objComm.sSMSTemplateID, "WEB");
                                        }
                                    }
                                }
                            }
                        }

                        else
                        {
                            objLogin.sMessage = "SMS FAILS";
                            objLogin.sResult = "1";
                        }
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("Email11", objLogin.sEmail);
                        sQry = "SELECT \"US_FULL_NAME\",\"US_LG_NAME\",\"US_PWD\",\"US_ID\",\"US_MOBILE_NO\" FROM \"TBLUSER\" where \"US_STATUS\"='D' AND CAST(\"US_MOBILE_NO\" AS TEXT)=:Email11";
                        dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objLogin.sMessage = "USER IS DEACTIVATED, PLEASE CONTACT SUPPORT TEAM";
                            objLogin.sResult = "1";
                        }
                        else
                        {
                            objLogin.sMessage = "SMS KEY INVALID";
                            objLogin.sResult = "1";
                        }
                    }
                }
                else
                {
                    if (r1.IsMatch(objLogin.sEmail))
                    {
                        objLogin.sMessage = "Enter Valid MobileNo";
                        objLogin.sResult = "1";
                    }
                    else
                    {
                        Random generator = new Random();
                        int OTP = generator.Next(1000000, 1234567);

                        NpgsqlCommand.Parameters.AddWithValue("emailId", objLogin.sEmail);
                        sQry = "SELECT \"US_FULL_NAME\",\"US_LG_NAME\",\"US_PWD\",\"US_ID\",\"US_MOBILE_NO\" FROM \"TBLUSER\" where \"US_EMAIL\"=:emailId";
                        dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objLogin.sPassword = dt.Rows[0]["US_PWD"].ToString();
                            objLogin.sFullName = dt.Rows[0]["US_FULL_NAME"].ToString();
                            objLogin.sLoginName = dt.Rows[0]["US_LG_NAME"].ToString();
                            objLogin.sUserId = dt.Rows[0]["US_ID"].ToString();
                            objLogin.sMobileNo = dt.Rows[0]["US_MOBILE_NO"].ToString();

                            Random random = new Random();
                            int num = random.Next(0, 26);
                            int num1 = random.Next(0, 26);
                            char let = (char)('A' + num);
                            char let1 = (char)('A' + num1);
                            objLogin.sOTP = Convert.ToString(OTP) + let + let1;

                            objLogin.sOTP = Convert.ToString(objLogin.sOTP);
                            objLogin.sOTP = Shuffle(objLogin.sOTP);

                            // string sSMSText = String.Format(Convert.ToString(ConfigurationSettings.AppSettings["SMStoOTP"]), objLogin.sFullName, objLogin.sOTP);
                            objLogin.sMessage = sSMSText;

                            SendMailForgotPwd(objLogin);

                            NpgsqlCommand.Parameters.AddWithValue("userId5", Convert.ToInt16(objLogin.sUserId));
                            sQry = "SELECT otp_sent_flag FROM tblotp WHERE otp_us_id=:userId5";
                            string sSentFlag = Objcon.get_value(sQry, NpgsqlCommand);

                            if (sSentFlag == "1")
                            {
                                NpgsqlCommand.Parameters.AddWithValue("userId6", Convert.ToInt16(objLogin.sUserId));
                                NpgsqlCommand.Parameters.AddWithValue("OTP3", objLogin.sOTP);
                                sQry = "INSERT INTO tblotp (otp_us_id,otp_no,otp_cron,otp_sent_flag,otp_change_pwd_on) VALUES(:userId6,:OTP3,now(),'0',now())";
                                Objcon.ExecuteQry(sQry, NpgsqlCommand);
                            }
                            else
                            {
                                NpgsqlCommand.Parameters.AddWithValue("OTP4", objLogin.sOTP);
                                NpgsqlCommand.Parameters.AddWithValue("UserId7", Convert.ToInt16(objLogin.sUserId));

                                sQry = "UPDATE tblotp set otp_no=:OTP4,otp_cron=now() WHERE otp_us_id=:UserId7";
                                Objcon.ExecuteQry(sQry, NpgsqlCommand);
                            }
                        }
                        else
                        {
                            objLogin.sMessage = "Enter Valid Email Id";
                            objLogin.sResult = "1";
                        }
                    }
                }

                return objLogin;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objLogin;
            }
        }

        public static string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        //public clsLogin ForgtPassword(clsLogin objLogin)
        //{
        //    DataTable dt = new DataTable();
        //    string sPattern = string.Empty;
        //    string sMobile_Mail = string.Empty;
        //    try
        //    {
        //        System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex(@"^\d+$");
        //        sPattern = @"^\d{10}$";
        //        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(sPattern);
        //        if (r.IsMatch(objLogin.sEmail))
        //        {
        //            sMobile_Mail = "Y";
        //            NpgsqlCommand cmd = new NpgsqlCommand("proc_get_dtcdetails");
        //            cmd.Parameters.AddWithValue("office_code", objLogin.sEmail);
        //            cmd.Parameters.AddWithValue("feeder_name", sMobile_Mail);
        //            dt = Objcon.FetchDataTable(cmd);
        //            if (dt.Rows.Count > 0)
        //            {
        //               objLogin = GetUserDetailsToView(objLogin, dt);
        //                clsCommunication objComm = new clsCommunication();
        //                string sSMSText = String.Format(Convert.ToString(ConfigurationSettings.AppSettings["SMStoForgotPwd"]), objLogin.sFullName, Genaral.Decrypt(objLogin.sPassword));
        //                objComm.sendSMS(sSMSText, objLogin.sMobileNo, objLogin.sFullName);
        //            }
        //            else
        //            {
        //                objLogin.sMessage = "Enter Valid Mobile Number.";
        //            }
        //        }
        //        else if (r1.IsMatch(objLogin.sEmail))
        //        {
        //            sMobile_Mail = "N";
        //            NpgsqlCommand cmd = new NpgsqlCommand("proc_get_dtcdetails");
        //            cmd.Parameters.AddWithValue("office_code", objLogin.sEmail);
        //            cmd.Parameters.AddWithValue("feeder_name", sMobile_Mail);
        //            dt = Objcon.FetchDataTable(cmd);
        //            if (dt.Rows.Count > 0)
        //            {
        //              objLogin =  GetUserDetailsToView(objLogin, dt);
        //              SendMailForgotPwd(objLogin);
        //            }
        //            else
        //            {
        //                objLogin.sMessage = "Enter Valid Email Id.";
        //            }
        //        }
        //        else
        //        {
        //            objLogin.sMessage = "Enter Valid Mobile Number or Email Id.";
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        clsException.LogError("Procedure - proc_forgot_password", ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return objLogin;
        //}

        public clsLogin GetUserDetailsToView(clsLogin objLogin, DataTable dt)
        {
            try
            {
                objLogin.sPassword = Convert.ToString(dt.Rows[0]["US_PWD"]);
                objLogin.sFullName = Convert.ToString(dt.Rows[0]["US_FULL_NAME"]);
                objLogin.sLoginName = Convert.ToString(dt.Rows[0]["US_LG_NAME"]);
                objLogin.sUserId = Convert.ToString(dt.Rows[0]["US_ID"]);
                objLogin.sMobileNo = Convert.ToString(dt.Rows[0]["US_MOBILE_NO"]);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return objLogin;
        }

        public void SendMailForgotPwd(clsLogin objLogin)
        {
            string strMailMsg = string.Empty;
            string strmailFormat = string.Empty;
            clsCommunication objComm = new clsCommunication();

            using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailFormats/CreateUser.txt")))
            {
                String line;
                // Read and display lines from the file until the end of
                // the file is reached.                
                while ((line = sr.ReadLine()) != null)
                {
                    strMailMsg += line;
                }
            }
            strmailFormat = String.Format(strMailMsg, objLogin.sFullName, objLogin.sLoginName, objLogin.sMessage);
            objComm.SendMail("DTLMS – Forgot Password", objLogin.sEmail, strmailFormat, objLogin.sUserId);
        }

        public DataTable GetOTPDetails(string sOtp)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtOTPDetails = new DataTable();
            string sQry = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("Otp", sOtp);
                sQry = "SELECT * FROM tblotp WHERE otp_no=:Otp and otp_sent_flag='0'";
                dtOTPDetails = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                return dtOTPDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtOTPDetails;
            }
        }

        public DataTable GetConfiguration()
        {
            DataTable dtConfiguration = new DataTable();
            string strQry = string.Empty;
            try
            {
                strQry = "SELECT * FROM \"TBLCONFIGURATION\"";
                return Objcon.FetchDataTable(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtConfiguration;
            }
        }

        public string GetPasswordDetails(string sUserId)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("userId", sUserId);
                strQry = "SELECT to_char(CURRENT_DATE -\"US_CHPWD_ON\",'dd') AS \"Days\" FROM \"TBLUSER\" WHERE \"US_ID\"=:userId";
                return Convert.ToString(Objcon.get_value(strQry, NpgsqlCommand));
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public bool GetStatus(string sOldPassword, string sUserId)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("psw", sOldPassword);
                NpgsqlCommand.Parameters.AddWithValue("usid", sUserId);
                strQry = "SELECT \"UOP_ID\" FROM \"TBLUSER_OLD_PASSWORD\" WHERE \"UOP_PWD\"=:psw AND \"UOP_US_ID\"=:usid";
                string sRes = Objcon.get_value(strQry, NpgsqlCommand);

                if (sRes != null && sRes != "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }


        public clsLogin PasswordExpiryCheck(clsLogin objLogin)
        {

            string[] PwdCheck = new string[3];
            DataTable dtDropdown = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_check_user_password_expiry_check");
                cmd.Parameters.AddWithValue("username", Convert.ToString(objLogin.sLoginName));


                cmd.Parameters.Add("id", NpgsqlDbType.Text);
                cmd.Parameters.Add("usertype", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);

                cmd.Parameters["id"].Direction = ParameterDirection.Output;
                cmd.Parameters["usertype"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                PwdCheck[0] = "id";
                PwdCheck[1] = "usertype";
                PwdCheck[2] = "msg";

                PwdCheck = Objcon.Execute(cmd, PwdCheck, 3);
                objLogin.LoginExpireCheck = Convert.ToString(PwdCheck[0]);
                objLogin.LoginUserType = Convert.ToString(PwdCheck[1]);
                objLogin.PwdMessage = Convert.ToString(PwdCheck[2]);

                return objLogin;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            finally
            {
                Objcon.close();
            }
            return objLogin;
        }

        public clsLogin SaveEmailAndPassWordExpireLog(clsLogin objLogin)
        {

            try
            {
                string[] strResult = new string[2];

                // PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationManager.AppSettings["pgSQLPassword"]));
                NpgsqlCommand cmdSaveLog = new NpgsqlCommand("sp_insert_password_history_maillog");
                cmdSaveLog.Parameters.AddWithValue("usid", Convert.ToInt32(objLogin.sUserId));
                cmdSaveLog.Parameters.AddWithValue("mailid", Convert.ToString(objLogin.sEmail));
                cmdSaveLog.Parameters.AddWithValue("expirytime", Convert.ToInt32(MailSessionExpiryTime));
                cmdSaveLog.Parameters.AddWithValue("apptype", 1);

                cmdSaveLog.Parameters.Add("pkid", NpgsqlDbType.Text);
                cmdSaveLog.Parameters.Add("status", NpgsqlDbType.Text);
                cmdSaveLog.Parameters["pkid"].Direction = ParameterDirection.Output;
                cmdSaveLog.Parameters["status"].Direction = ParameterDirection.Output;
                strResult[0] = "pkid";
                strResult[1] = "status";
                strResult = Objcon.Execute(cmdSaveLog, strResult, 2);
                objLogin.SubmitClick = Convert.ToInt32(strResult[0]);
                objLogin.PwdMessage = Convert.ToString(strResult[1]);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return objLogin;
        }


        public string FetchMailHistory(clsLogin objLogin)
        {


            try
            {
                string sQry = "SELECT concat(phm_cron,'~',\"US_FULL_NAME\",'~',\"US_ID\")  FROM tbl_password_history_maillog inner join \"TBLUSER\" on \"US_ID\" = phm_us_id where phm_id = '" + objLogin.SubmitClick + "' and phm_status=1 and phm_app_type=1";
                return Objcon.get_value(sQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                Objcon.close();
            }
        }


        public string FetchLastChangedPassowrdCrOn(clsLogin objLogin)
        {


            try
            {
                string sQry = "select \"US_CHPWD_ON\" from \"TBLUSER\" inner join tbl_password_history_maillog on \"US_ID\" = phm_us_id where phm_id = '" + objLogin.SubmitClick + "'";
                return Objcon.get_value(sQry);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                Objcon.close();
            }
        }



    }
}
