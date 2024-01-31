using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace IIITS.DTLMS.BL
{
    public class clsUser
    {
        string strFormCode = "clsUser";
        public string lSlNo { get; set; }
        public string sOfficeCode { get; set; }
        public string sFullName { get; set; }
        public string sLoginName { get; set; }
        public string sPassword { get; set; }
        public string sMobileNo { get; set; }
        public string sDesignation { get; set; }
        public string sRole { get; set; }

        public string sEmail { get; set; }
        public string sPhoneNo { get; set; }
        public string sAddress { get; set; }
        public string sUserType { get; set; }
        public string sCrby { get; set; }
        public string sStatus { get; set; }

        public Byte[] sSignImage { get; set; }

        public string sEffectFrom { get; set; }
        public string sReason { get; set; }
        public string sOffCode { get; set; }
        public string sRoleType { get; set; }
        public string sOTP { get; set; }
        public string struserId { get; set; }

        public string zone { get; set; }
        public string circle { get; set; }
        public string division { get; set; }
        public string subdivision { get; set; }
        public string section { get; set; }
        public string rolename { get; set; }

        PGSqlConnection objCon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;

        public string[] SaveUpdateUserDetails(clsUser objUser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[2];

            string strQry = string.Empty;
            try
            {

                if (objUser.sOfficeCode != "")
                {
                    String sOffCode = String.Empty;
                    if (objUser.sRole == "2" || objUser.sRole == "5")
                    {
                        sOffCode = objUser.sOfficeCode;
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("offcode", Convert.ToInt32(objUser.sOfficeCode));
                        sOffCode = objCon.get_value("SELECT \"OFF_CODE\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" =:offcode", NpgsqlCommand);
                    }
                    if (sOffCode.Length <= 0)
                    {
                        Arr[0] = "Enter Valid Office Code";
                        Arr[1] = "4";
                        return Arr;
                    }

                    //drUser = objCon.Fetch("SELECT OFF_CODE FROM VIEW_ALL_OFFICES WHERE OFF_CODE='" + objUser.sOfficeCode + "'");
                    //if (!drUser.Read())
                    //{
                    //    drUser.Close();
                    //    Arr[0] = "Enter Valid Office Code";
                    //    Arr[1] = "4";
                    //    return Arr;
                    //}
                    //drUser.Close();
                }

                if (objUser.lSlNo == "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("LoginName", objUser.sLoginName.ToUpper());
                    String sOffCode = objCon.get_value("SELECT \"US_LG_NAME\" FROM \"TBLUSER\" WHERE UPPER(\"US_LG_NAME\") =:LoginName", NpgsqlCommand);
                    if (sOffCode.Length > 0)
                    {
                        Arr[0] = "Login Name Already Exists";
                        Arr[1] = "4";
                        return Arr;
                    }

                    //drUser = objCon.Fetch("select * from TBLUSER where  UPPER(US_LG_NAME)='" + objUser.sLoginName.ToUpper() + "'  ");
                    //if (drUser.Read())
                    //{
                    //    drUser.Close();
                    //    Arr[0] = "Login Name Already Exists";
                    //    Arr[1] = "4";
                    //    return Arr;
                    //}
                    //drUser.Close();

                    string sMaxNo = Convert.ToString(objCon.Get_max_no("US_ID", "TBLUSER"));


                    NpgsqlCommand.Parameters.AddWithValue("MaxNo", Convert.ToInt32(sMaxNo));
                    NpgsqlCommand.Parameters.AddWithValue("FullName", objUser.sFullName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("LoginName1", objUser.sLoginName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode", objUser.sOfficeCode);
                    NpgsqlCommand.Parameters.AddWithValue("Password", Genaral.EncryptPassword(objUser.sPassword));
                    NpgsqlCommand.Parameters.AddWithValue("Role", Convert.ToInt16(objUser.sRole));
                    NpgsqlCommand.Parameters.AddWithValue("Email", objUser.sEmail);
                    NpgsqlCommand.Parameters.AddWithValue("MobileNo", Convert.ToDouble(objUser.sMobileNo));
                    NpgsqlCommand.Parameters.AddWithValue("PhoneNo", objUser.sPhoneNo);
                    NpgsqlCommand.Parameters.AddWithValue("Address", objUser.sAddress);
                    NpgsqlCommand.Parameters.AddWithValue("Crby", Convert.ToInt16(objUser.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("Designation", Convert.ToSByte(objUser.sDesignation));

                    strQry = "INSERT INTO \"TBLUSER\" (\"US_ID\",\"US_FULL_NAME\",\"US_LG_NAME\",\"US_OFFICE_CODE\",\"US_PWD\",\"US_ROLE_ID\",\"US_EMAIL\",\"US_MOBILE_NO\",";
                    strQry += " \"US_PHONE_NO\",\"US_ADDRESS\",\"US_CR_ON\",\"US_CRBY\",\"US_DESG_ID\",\"US_SIGN_IMAGE\") ";
                    strQry += "values (:MaxNo,:FullName,:LoginName1,";
                    strQry += ":OfficeCode,:Password,";
                    strQry += " :Role,:Email,";
                    strQry += ":MobileNo,:PhoneNo,:Address,NOW(),";
                    strQry += " :Crby,:Designation,':Photo')";


                    //NpgsqlParameter docPhoto = new NpgsqlParameter();
                    // NpgsqlCommand comd = new NpgsqlCommand();
                    // docPhoto.DbType = DbType.Binary;

                    //OleDbParameter docPhoto = new OleDbParameter();
                    //OleDbCommand comd = new OleDbCommand();
                    //docPhoto.DbType = DbType.Binary;

                    //if (objUser.sSignImage != null)
                    //{
                    //    docPhoto.ParameterName = "Photo";
                    //    docPhoto.Value = objUser.sSignImage;
                    //}

                    //comd = new NpgsqlCommand(strQry);
                    ////comd = new OleDbCommand(strQry, objCon.Con);
                    //if (objUser.sSignImage != null)
                    //{
                    //    comd.Parameters.Add(docPhoto);
                    //}
                    //comd.ExecuteNonQuery();

                    // objCon.Execute(strQry);
                    objCon.ExecuteQry(strQry, NpgsqlCommand);

                    // To send the Mail for successfull creation
                    SendMailUserSuccCreate(objUser);
                    SendSMSUserSuccCreate(objUser);

                    Arr[0] = sMaxNo;
                    Arr[1] = "0";
                    return Arr;
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("LoginName2", objUser.sLoginName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("slno", Convert.ToInt32(objUser.lSlNo));
                    String sOffCode = objCon.get_value("SELECT \"US_LG_NAME\" FROM \"TBLUSER\" WHERE UPPER(\"US_LG_NAME\") =:LoginName2 AND \"US_ID\" <>:slno", NpgsqlCommand);
                    if (sOffCode.Length > 0)
                    {
                        Arr[0] = "Login Name Already Exists";
                        Arr[1] = "4";
                        return Arr;
                    }

                    //drUser = objCon.Fetch("SELECT * FROM TBLUSER WHERE  UPPER(US_LG_NAME)='" + objUser.sLoginName.ToUpper() + "'  AND US_ID<>'" + objUser.lSlNo + "'");
                    //if (drUser.Read())
                    //{
                    //    drUser.Close();
                    //    Arr[0] = "Login Name Already Exists";
                    //    Arr[1] = "4";
                    //    return Arr;
                    //}
                    //drUser.Close();


                    NpgsqlCommand.Parameters.AddWithValue("FullName1", objUser.sFullName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("LoginName3", objUser.sLoginName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode1", objUser.sOfficeCode);
                    // NpgsqlCommand.Parameters.AddWithValue("Password1", Genaral.EncryptPassword(objUser.sPassword));
                    NpgsqlCommand.Parameters.AddWithValue("Role1", Convert.ToInt16(objUser.sRole));
                    NpgsqlCommand.Parameters.AddWithValue("Email1", objUser.sEmail);
                    NpgsqlCommand.Parameters.AddWithValue("MobileNo1", Convert.ToDouble(objUser.sMobileNo));
                    NpgsqlCommand.Parameters.AddWithValue("PhoneNo1", objUser.sPhoneNo);
                    NpgsqlCommand.Parameters.AddWithValue("Address1", objUser.sAddress);

                    NpgsqlCommand.Parameters.AddWithValue("Designation1", Convert.ToSByte(objUser.sDesignation));

                    strQry = "UPDATE \"TBLUSER\" SET \"US_FULL_NAME\" =:FullName1, \"US_LG_NAME\" =:LoginName3, \"US_OFFICE_CODE\" =:OfficeCode1,";
                    strQry += " \"US_ROLE_ID\"  =:Role1,\"US_EMAIL\" =:Email1, \"US_MOBILE_NO\" =:MobileNo1, \"US_PHONE_NO\" =:PhoneNo1,";
                    strQry += " \"US_ADDRESS\" =:Address1,\"US_DESG_ID\" =:Designation1";

                    if (objUser.sSignImage != null)
                    {
                        strQry += " , \"US_SIGN_IMAGE\" =':Photo' ";
                    }

                    NpgsqlCommand.Parameters.AddWithValue("UsId", Convert.ToInt32(objUser.lSlNo));
                    strQry += " WHERE \"US_ID\" =:UsId";

                    //OleDbParameter docPhoto = new OleDbParameter();
                    //OleDbCommand comd = new OleDbCommand();

                    //NpgsqlParameter docPhoto = new NpgsqlParameter();
                    //NpgsqlCommand comd = new NpgsqlCommand();               


                    //if (objUser.sSignImage != null)
                    //{
                    //    docPhoto.DbType = DbType.Binary;
                    //    docPhoto.ParameterName = "Photo";
                    //    docPhoto.Value = objUser.sSignImage;
                    //}

                    //comd = new NpgsqlCommand(strQry);
                    //if (objUser.sSignImage != null)
                    //{
                    //    comd.Parameters.Add(docPhoto);
                    //}
                    //comd.ExecuteNonQuery();

                    objCon.ExecuteQry(strQry, NpgsqlCommand);
                    Arr[0] = "Updated Successfully";
                    Arr[1] = "1";
                    return Arr;
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }

        }

        public DataTable LoadUserGrid(clsUser objuser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtUserDetails = new DataTable();
            string strQry = string.Empty;
            try
            {             
                strQry = " select * from (SELECT \"US_ID\", \"US_FULL_NAME\", UPPER(\"DM_NAME\") AS \"US_DESG_ID\", \"US_EMAIL\", \"US_MOBILE_NO\",";
                strQry += " \"RO_NAME\", \"OFF_NAME\", \"US_STATUS\", CASE  WHEN TO_CHAR(\"US_EFFECT_FROM\", 'YYYYMMDD') > TO_CHAR(NOW(), 'YYYYMMDD') ";
                strQry += " AND \"US_STATUS\" = 'D' THEN 'A'  ELSE  \"US_STATUS\"  END  \"US_STATUS1\" from \"TBLUSER\" inner join \"TBLROLES\" on  \"RO_ID\" = \"US_ROLE_ID\" inner join \"TBLDESIGNMAST\" on \"DM_DESGN_ID\" = \"US_DESG_ID\"";
                strQry += " left join \"VIEW_ALL_OFFICES\"  on  CAST(\"OFF_CODE\" AS TEXT) = \"US_OFFICE_CODE\" ";
                if (objuser.sOffCode == null || objuser.sOffCode == "--Select--")
                {
                    strQry += " ORDER BY \"US_ID\" DESC)A";
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcode", objuser.sOffCode);
                    NpgsqlCommand.Parameters.AddWithValue("offcode1", objuser.sOffCode);
                    strQry += " where ( \"US_OFFICE_CODE\" like :offcode||'%' OR cast(\"US_OFFICE_CODE\" as text)= (SELECT cast(\"STO_SM_ID\" as text) FROM \"TBLSTOREOFFCODE\" WHERE cast(\"STO_OFF_CODE\" as text)= substr(cast(:offcode1 as text),1,3))) ORDER BY \"US_ID\" DESC) as A";
                }

                dtUserDetails = objCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtUserDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtUserDetails;
            }
        }

        public object GetUserDetails(clsUser objuser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtUserDetails = new DataTable();
            try
            {


                string strQry = string.Empty;
                if (objuser.sRoleType == "2")
                {
                    NpgsqlCommand.Parameters.AddWithValue("slno", Convert.ToInt32(objuser.lSlNo));
                    strQry = "SELECT   '' as zone,''  as circle,'' as  subdivision,\"SM_NAME\" AS DIVISION,'' as section,\"US_ID\", \"US_FULL_NAME\", \"US_LG_NAME\", \"US_OFFICE_CODE\", \"US_PWD\", \"US_EMAIL\", ";
                    strQry += "\"US_MOBILE_NO\", \"US_PHONE_NO\", \"US_ROLE_ID\",\"US_ADDRESS\", \"US_USER_TYPE\", \"US_DESG_ID\",\"RO_TYPE\",\"RO_NAME\" FROM \"TBLUSER\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"US_ROLE_ID\" INNER JOIN ";
                    strQry += " \"TBLSTOREMAST\" ON \"US_OFFICE_CODE\"=CAST(\"SM_ID\" AS TEXT) WHERE \"US_ID\" =:slno";

                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("slno6", objuser.lSlNo);
                    strQry = "SELECT (SELECT \"ZO_NAME\"  from \"TBLZONE\" WHERE SUBSTR(cast(\"ZO_CO_ID\" as text),1,1)=SUBSTR(cast(\"US_OFFICE_CODE\" as text),1,1)  LIMIT 1)\"zone\",(SELECT \"CM_CIRCLE_NAME\"  from \"TBLCIRCLE\" WHERE SUBSTR(cast(\"CM_CIRCLE_CODE\" as text),1,2)=SUBSTR(cast(\"US_OFFICE_CODE\" as text),1,2) LIMIT 1)circle,(SELECT  \"DIV_NAME\"";
                    strQry += "from \"TBLDIVISION\" WHERE SUBSTR(cast(\"DIV_CODE\" as text),1,3)=SUBSTR(cast(\"US_OFFICE_CODE\" as text),1,3)  LIMIT 1)DIVISION,(SELECT  \"SD_SUBDIV_NAME\"  from \"TBLSUBDIVMAST\" WHERE ";
                    strQry += "SUBSTR(cast(\"SD_SUBDIV_CODE\" as text),1,4)=SUBSTR(cast(\"US_OFFICE_CODE\" as text),1,4) LIMIT 1)subdivision,(SELECT  \"OM_NAME\"  from \"TBLOMSECMAST\" WHERE SUBSTR(cast(\"OM_CODE\" as text),1,5)=SUBSTR(cast(\"US_OFFICE_CODE\" as text),1,5)";
                    strQry += "LIMIT 1)section,\"US_ID\", \"US_FULL_NAME\", \"US_LG_NAME\", \"US_OFFICE_CODE\", \"US_PWD\", \"US_EMAIL\", \"US_MOBILE_NO\",";
                    strQry += " \"US_PHONE_NO\", \"US_ROLE_ID\", \"US_ADDRESS\", \"US_USER_TYPE\", \"US_DESG_ID\",\"RO_TYPE\",\"RO_NAME\" FROM \"TBLUSER\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"US_ROLE_ID\" WHERE CAST(\"US_ID\" as text)=:slno6";

                }



                dtUserDetails = objCon.FetchDataTable(strQry, NpgsqlCommand);
                //dtUserDetails.Load(drUser);
                if (dtUserDetails.Rows.Count > 0)
                {
                    objuser.lSlNo = Convert.ToString(dtUserDetails.Rows[0]["US_ID"]);
                    objuser.sOfficeCode = Convert.ToString(dtUserDetails.Rows[0]["US_OFFICE_CODE"]);
                    objuser.sFullName = Convert.ToString(dtUserDetails.Rows[0]["US_FULL_NAME"]);
                    objuser.sLoginName = Convert.ToString(dtUserDetails.Rows[0]["US_LG_NAME"]);
                    objuser.sPassword = Convert.ToString(dtUserDetails.Rows[0]["US_PWD"]);
                    objuser.sRole = Convert.ToString(dtUserDetails.Rows[0]["US_ROLE_ID"]);
                    objuser.sEmail = Convert.ToString(dtUserDetails.Rows[0]["US_EMAIL"]);
                    objuser.sMobileNo = Convert.ToString(dtUserDetails.Rows[0]["US_MOBILE_NO"]);
                    objuser.sPhoneNo = Convert.ToString(dtUserDetails.Rows[0]["US_PHONE_NO"]);
                    objuser.sAddress = Convert.ToString(dtUserDetails.Rows[0]["US_ADDRESS"]);
                    objuser.sUserType = Convert.ToString(dtUserDetails.Rows[0]["US_USER_TYPE"]);
                    objuser.sDesignation = Convert.ToString(dtUserDetails.Rows[0]["US_DESG_ID"]);
                    objuser.sRoleType = Convert.ToString(dtUserDetails.Rows[0]["RO_TYPE"]);

                    objuser.rolename = Convert.ToString(dtUserDetails.Rows[0]["RO_NAME"]);
                    objuser.zone = Convert.ToString(dtUserDetails.Rows[0]["zone"]);
                    objuser.circle = Convert.ToString(dtUserDetails.Rows[0]["circle"]);
                    objuser.division = Convert.ToString(dtUserDetails.Rows[0]["DIVISION"]);
                    objuser.subdivision = Convert.ToString(dtUserDetails.Rows[0]["subdivision"]);
                    objuser.section = Convert.ToString(dtUserDetails.Rows[0]["section"]);

                }
                return objuser;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objuser;
            }
        }

        public void SendMailUserSuccCreate(clsUser objUser)
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
            strmailFormat = String.Format(strMailMsg, objUser.sFullName, objUser.sLoginName, objUser.sPassword);
            objComm.SendMail("DTLMS – User Created Successfully", objUser.sEmail, strmailFormat, objUser.sCrby);
        }

        public void SendSMSUserSuccCreate(clsUser objUser)
        {
            string strSms = string.Empty;
          
            clsCommunication objcomm = new clsCommunication();
            objcomm.sSMSkey = "SMStoUserSuccCreat";
            objcomm = objcomm.GetsmsTempalte(objcomm);           
            strSms = String.Format(objcomm.sSMSTemplate, objUser.sFullName, objUser.sLoginName, objUser.sPassword);
            //objComm.sendSMS(strSms, objUser.sMobileNo, objUser.sFullName);
            if (objcomm.sSMSTemplateID != null && objcomm.sSMSTemplateID != "")
            {
                objcomm.DumpSms(sMobileNo, strSms, objcomm.sSMSTemplateID, "WEB");
            }

         
        }

        public bool ActiveDeactiveUser(clsUser objUser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("Status", objUser.sStatus);
                NpgsqlCommand.Parameters.AddWithValue("EffectFrom", objUser.sEffectFrom);
                NpgsqlCommand.Parameters.AddWithValue("Reason", objUser.sReason);
                NpgsqlCommand.Parameters.AddWithValue("UsID", Convert.ToInt32(objUser.lSlNo));


                strQry = "UPDATE \"TBLUSER\" SET \"US_STATUS\" =:Status,";
                strQry += " \"US_EFFECT_FROM\" = TO_DATE(:EffectFrom,'dd/MM/yyyy'), \"US_REASON\" =:Reason";
                strQry += " WHERE \"US_ID\" =:UsID";
                objCon.ExecuteQry(strQry, NpgsqlCommand);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;

            }
        }

        public string GetRoleType(string sRoID)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("roid", Convert.ToInt16(sRoID));
                strQry = "SELECT \"RO_TYPE\" FROM \"TBLROLES\" WHERE \"RO_ID\"=:roid";
                return objCon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string[] UpdatePwd(clsUser objUser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[2];
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("Password", Genaral.EncryptPassword(objUser.sPassword));
                NpgsqlCommand.Parameters.AddWithValue("lSlNo", Convert.ToInt32(objUser.lSlNo));

                strQry = "UPDATE \"TBLUSER\" set \"US_PWD\"=:Password WHERE \"US_ID\"=:lSlNo";
                objCon.ExecuteQry(strQry, NpgsqlCommand);

                NpgsqlCommand.Parameters.AddWithValue("OTP", objUser.sOTP);
                NpgsqlCommand.Parameters.AddWithValue("lSlNo1", Convert.ToInt16(objUser.lSlNo));

                strQry = "UPDATE tblotp set otp_no=:OTP,otp_change_pwd_on=now(),otp_sent_flag='1' WHERE otp_us_id=:lSlNo1 and otp_sent_flag='0'";
                //strQry = "INSERT into tblotp (otp_us_id,otp_no,otp_cron,otp_sent_flag) values ('"+ objUser.lSlNo + "','"+ objUser.sOTP + "',now(),'1')";
                objCon.ExecuteQry(strQry, NpgsqlCommand);

                Arr[0] = "1";
                Arr[1] = "Password Changed Succesfully";
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                Arr[0] = "0";
                Arr[1] = "Something went wrong";
                return Arr;
            }
        }

        public string[] UpdateProfile(clsUser objUser)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] ArrResult = new string[2];

            string strQry = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("FullName", objUser.sFullName.ToUpper());
                NpgsqlCommand.Parameters.AddWithValue("Email", objUser.sEmail);
                NpgsqlCommand.Parameters.AddWithValue("MobileNo", Convert.ToInt64(objUser.sMobileNo));
                NpgsqlCommand.Parameters.AddWithValue("truserId", Convert.ToInt32(objUser.struserId));

                strQry = "UPDATE \"TBLUSER\" SET \"US_FULL_NAME\" =:FullName,";
                strQry += " \"US_EMAIL\" =:Email,\"US_MOBILE_NO\"=:MobileNo";
                strQry += " WHERE \"US_ID\"=:truserId";


                objCon.ExecuteQry(strQry, NpgsqlCommand);
                ArrResult[0] = "Updated Successfully";
                ArrResult[1] = "1";

                return ArrResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ArrResult;
            }
        }

    }
}


