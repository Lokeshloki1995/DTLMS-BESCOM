using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIITS.PGSQL.DAL;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsNewDTCCR : clsCRReport
    {
        string strFormCode = "clsNewDTCCR";

        public string sStatus { get; set; }
        public string sTcCode { get; set; }
        public string sFeederCode { get; set; }
        public string sTCSlno { get; set; }
        public string sTCCapacity { get; set; }
        public string sTCManfDate { get; set; }
        public string sTCMake { get; set; }
        public string sRating { get; set; }
        public string sDTCCode { get; set; }
        public string sDTCName { get; set; }
        public string sStarRate { get; set; }
        public string sMakeName { get; set; }
        public string sWOSlno { get; set; }
        public string sCrBy { get; set; }
        public string sRefOfficeCode { get; set; }
        public string sEnumDetailsID { get; set; }
        public string sTransCommDate { get; set; }
        public string sRecordId { get; set; }
        public string sOldCodePhotoPath { get; set; }
        public string sNamePlatePhotoPath { get; set; }
        public string sSSPlatePhotoPath { get; set; }
        public string sDTCPhoto2Path { get; set; }
        public string sDTLMSCodePhotoPath { get; set; }
        public string sIPEnumCodePhotoPath { get; set; }
        public string sInfosysCodePhotoPath { get; set; }
        public string sDTCPhoto1Path { get; set; }
        public string sInfosysAsset { get; set; }
        public Int16 sLevel { get; set; }

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);

        int SubDiv_code = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["SubDiv_code"]);
        int Section_code = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["Section_code"]);
        public string[] SaveCompletionReport(clsNewDTCCR objCR)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            try
            {
                #region Workflow

                string strQry1 = "UPDATE \"TBLWORKORDER\" SET \"WO_REPLACE_FLG\" =1,\"WO_NEW_DTC_CR_DATE\"=TO_DATE('" + objCR.sCRDate + "','dd/MM/yyyy') WHERE \"WO_SLNO\" ='" + objCR.sWOSlno + "'; UPDATE \"TBLDTCMAST\" SET \"DT_NEWDTC_CR_FLAG\" =1 WHERE \"DT_CODE\" ='" + objCR.sDTCCode + "'";

                strQry1 = strQry1.Replace("'", "''");

                clsApproval objApproval = new clsApproval();
                objApproval.sFormName = objCR.sFormName;
                objApproval.sRecordId = objCR.sRecordId;
                objApproval.sOfficeCode = objCR.sOfficeCode;
                objApproval.sClientIp = objCR.sClientIP;
                objApproval.sCrby = objCR.sCrby;
                objApproval.sWFObjectId = objCR.sWFObjectId;
                objApproval.sWFAutoId = objCR.sWFAutoId;
                objApproval.sDataReferenceId = objCR.sRecordId;
                objApproval.sRefOfficeCode = objCR.sRefOfficeCode;
                objApproval.sQryValues = strQry1;

                objApproval.sDescription = "Completion Report For DTC Code " + objCR.sDTCCode;

                objApproval.sColumnNames = "CR_DATE,EP_NAMEPLATE_PATH,EP_SSPLATE_PATH,EP_DTC_PATH,EP_DTLMSDTC_PATH,EP_INFOSYSDTC_PATH,ED_IP_ENUM_DONE,EP_OLDDTC_PATH,ED_ID,ED_APPROVALPRIORITY";
                objApproval.sColumnValues = "" + objCR.sCRDate + "," + objCR.sNamePlatePhotoPath + "," + objCR.sSSPlatePhotoPath + "," + objCR.sDTCPhoto1Path + ",";
                objApproval.sColumnValues += "" + objCR.sDTLMSCodePhotoPath + "," + objCR.sInfosysCodePhotoPath + "," + objCR.sIPEnumCodePhotoPath + "," + objCR.sDTCPhoto2Path + "," + objCR.sEnumDetailsID + "," + objCR.sLevel + "";
                objApproval.sTableNames = "TBLTEMPTABLE";

                bool bResult = objApproval.CheckDuplicateApprove(objApproval);
                if (bResult == false)
                {
                    Arr[0] = "Selected Record Already Approved";
                    Arr[1] = "2";
                    return Arr;
                }

                //objApproval.SaveWorkflowObjects(objApproval);

                if (objCR.sActionType == "M")
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objCR.sWFDataId = objApproval.sWFDataId;
                }
                else
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objCR.sWFDataId = objApproval.sWFDataId;
                    //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                    objApproval.SaveWorkflowObjects(objApproval);
                }


                Arr[0] = "Approved Successfully";
                Arr[1] = "0";
                return Arr;
                #endregion
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }
        public bool SaveImagePathDetails(clsNewDTCCR objCR)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;

                string sMaxNo = Convert.ToString(ObjCon.Get_max_no("EP_ID", "TBLENUMERATIONPHOTOS"));

                strQry = "INSERT INTO \"TBLENUMERATIONPHOTOS\" (\"EP_ID\",\"EP_ED_ID\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\",\"EP_OLDDTC_PATH\",\"EP_DTLMSDTC_PATH\",";
                strQry += " \"EP_IPENUMDTC_PATH\",\"EP_INFOSYSDTC_PATH\",\"EP_DTC_PATH\",\"EP_CRBY\") VALUES ('" + sMaxNo + "','" + objCR.sEnumDetailsID + "','" + objCR.sNamePlatePhotoPath + "',";
                strQry += " '" + objCR.sSSPlatePhotoPath + "','" + objCR.sDTCPhoto2Path + "','" + objCR.sDTLMSCodePhotoPath + "',";
                strQry += " '" + objCR.sIPEnumCodePhotoPath + "','" + objCR.sInfosysCodePhotoPath + "','" + objCR.sDTCPhoto1Path + "','" + objCR.sCrby + "')";
                ObjCon.ExecuteQry(strQry);
                return true;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "SaveImagePathDetails");
                return false;
            }
        }
        public string[] SaveFieldEnumerationDetails(clsNewDTCCR objCR)
        {
            string[] Arr = new string[3];
            string strQry = string.Empty;
            string strOfficeCode = string.Empty;
            string sRes = string.Empty;
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

            try
            {
                objCR.sFeederCode = objCR.sDTCCode.Substring(0, 6);
                if (objCR.sRefOfficeCode.Length >= Section_code)
                {
                    strOfficeCode = objCR.sRefOfficeCode.Substring(0, SubDiv_code);
                }
                if (objCR.sEnumDetailsID == null)
                {
                    objCR.sEnumDetailsID = "";
                }

                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_savefieldenumerationdetails");
                    cmd.Parameters.AddWithValue("stccode", objCR.sTcCode);
                    cmd.Parameters.AddWithValue("strofficecode", objCR.sOfficeCode);
                    cmd.Parameters.AddWithValue("ssubdivcode", strOfficeCode);
                    cmd.Parameters.AddWithValue("senumdetailsid", objCR.sEnumDetailsID);
                    cmd.Parameters.AddWithValue("sdtccode", objCR.sDTCCode);
                    cmd.Parameters.AddWithValue("scommisiondate", objCR.sTransCommDate);

                    cmd.Parameters.AddWithValue("stcmake", objCR.sTCMake);
                    cmd.Parameters.AddWithValue("stccapacity", objCR.sTCCapacity);
                    cmd.Parameters.AddWithValue("smakename", objCR.sTCMake);
                    cmd.Parameters.AddWithValue("stcslno", objCR.sTcSlno);
                    cmd.Parameters.AddWithValue("sfeedercode", objCR.sFeederCode);
                    cmd.Parameters.AddWithValue("soperator1", objCR.sCrby);
                    cmd.Parameters.AddWithValue("scrby", objCR.sCrby);
                    cmd.Parameters.AddWithValue("sdtcname", objCR.sDTCName);
                    cmd.Parameters.AddWithValue("srating", objCR.sRating);
                    cmd.Parameters.AddWithValue("sstarrate", objCR.sRating);
                    cmd.Parameters.AddWithValue("slevel", objCR.sLevel);
                    cmd.Parameters.AddWithValue("sstatus", '0');



                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("enumid", NpgsqlDbType.Text);
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["enumid"].Direction = ParameterDirection.Output;
                    Arr[2] = "enumid";
                    Arr[1] = "id";
                    Arr[0] = "msg";
                    Arr = ObjCon.Execute(cmd, Arr, 3);
                    if (Arr[1] == "0")
                    {
                        objCR.sEnumDetailsID = Arr[2].ToString();
                    }
                    return Arr;

                }
                catch (Exception ex)
                {

                    clsException.LogError(ex.StackTrace, ex.Message + strQry, strFormCode, "SaveFieldEnumerationDetails");
                    return Arr;
                }

                //strQry = "SELECT \"TCP_TC_CODE\" FROM \"TBLTCPLATEALLOCATION\" WHERE \"TCP_TC_CODE\"='" + objFieldEnum.sTcCode + "'";
                //string Tc_Code = ObjCon.get_value(strQry);

                //if (Tc_Code == "")
                //{
                //    Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Not Allocated to Any Vendor";
                //    Arr[1] = "2";
                //    return Arr;
                //}

                //strQry = "SELECT \"VM_ID\" || '~' ||\"VM_NAME\" FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\",\"TBLVENDORMASTER\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND ";
                //strQry += " \"VM_ID\" =\"TCPM_VENDOR_ID\" AND \"TCP_TC_CODE\"='" + objFieldEnum.sTcCode + "'";
                //string sVmDetails = ObjCon.get_value(strQry);

                //strQry = "SELECT \"IU_VENDOR_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_ID\"='" + objFieldEnum.sOperator1 + "'";
                //string sLogUs_VM_Id = ObjCon.get_value(strQry);

                //if (sLogUs_VM_Id == sVmDetails.Split('~').GetValue(0).ToString())
                //{
                //    strQry = "SELECT * FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND \"TCP_STATUS_FLAG\" IN (0,4) AND ";
                //    strQry += " \"TCP_TC_CODE\" ='" + objFieldEnum.sTcCode + "'";
                //    string res = ObjCon.get_value(strQry);

                //    if (res == "")
                //    {
                //        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Not Possible to save because entered tc code may be Approved, deleted or Rejected ";
                //        Arr[1] = "2";
                //        return Arr;
                //    }
                //}
                //else
                //{
                //    Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already allocated to vendor " + sVmDetails.Split('~').GetValue(1).ToString();
                //    Arr[1] = "2";
                //    return Arr;
                //}

                //if (objFieldEnum.sOfficeCode.Length >= Section_code)
                //{
                //    strOfficeCode = objFieldEnum.sOfficeCode.Substring(0, SubDiv_code);
                //}

                //strQry = "select \"FD_FEEDER_ID\" from \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" WHERE \"FD_FEEDER_CODE\"='" + objFieldEnum.sDTCCode.ToString().Substring(0, Feeder_code) + "' ";
                //strQry += " AND  \"FD_FEEDER_ID\"=\"FDO_FEEDER_ID\" AND cast(\"FDO_OFFICE_CODE\" as text) LIKE '" + strOfficeCode + "%'";
                //sRes = ObjCon.get_value(strQry);
                //if (sRes == "")
                //{
                //    Arr[0] = "Code Does Not Match With The Feeder Code";
                //    Arr[1] = "2";
                //    return Arr;

                //}                

                ///// //If sEnumDetailsID="" then Insert else Update;
                //if (objFieldEnum.sEnumDetailsID == "")
                //{

                //    ObjCon.BeginTransaction();


                //    strQry = "SELECT \"DTE_TC_CODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //    sRes = ObjCon.get_value(strQry);
                //    if (sRes != "")
                //    {
                //        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                //        Arr[1] = "2";
                //        return Arr;
                //    }

                //    strQry = "SELECT \"DTE_DTCCODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //    sRes = ObjCon.get_value(strQry);
                //    if (sRes != "")
                //    {
                //        Arr[0] = "DTC Code(DTLMS) " + objFieldEnum.sDTCCode + "  Already Exist";
                //        Arr[1] = "2";
                //        return Arr;
                //    }

                //    if (objFieldEnum.sTCMake != "")
                //    {
                //        strQry = "SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objFieldEnum.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //        sRes = ObjCon.get_value(strQry);
                //        if (sRes != "")
                //        {
                //            Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                //            Arr[1] = "2";
                //            return Arr;
                //        }
                //    }

                //    // Save to Enumeration Details (Basic Information in Enumeration)
                //    objFieldEnum.sEnumDetailsID = Convert.ToString(ObjCon.Get_max_no("ED_ID", "TBLENUMERATIONDETAILS"));

                //    strQry = "INSERT INTO \"TBLENUMERATIONDETAILS\" (\"ED_ID\",\"ED_OFFICECODE\",\"ED_OPERATOR1\",\"ED_OPERATOR2\",";
                //    if (objFieldEnum.sWeldDate == null)
                //    {
                //        strQry += "\"ED_FEEDERCODE\",\"ED_ENUM_TYPE\",\"ED_LOCTYPE\",\"ED_CRBY\",\"ED_IP_ENUM_DONE\",\"ED_APPROVE_STATUS\") VALUES (";
                //    }
                //    else
                //    {
                //        strQry += "\"ED_WELD_DATE\",\"ED_FEEDERCODE\",\"ED_ENUM_TYPE\",\"ED_LOCTYPE\",\"ED_CRBY\",\"ED_IP_ENUM_DONE\",\"ED_APPROVE_STATUS\") VALUES (";
                //    }

                //    strQry += " '" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sOfficeCode + "','" + objFieldEnum.sOperator1 + "','" + objFieldEnum.sOperator2 + "',";

                //    if (objFieldEnum.sWeldDate == null)
                //    {
                //        strQry += " '" + objFieldEnum.sFeederCode + "','2','2','" + objFieldEnum.sCrBy + "','" + objFieldEnum.sIsIPEnumDone + "',";
                //    }
                //    else
                //    {
                //        strQry += " TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),'" + objFieldEnum.sFeederCode + "','2','2','" + objFieldEnum.sCrBy + "','" + objFieldEnum.sIsIPEnumDone + "',";
                //    }

                //    if (objFieldEnum.sUserType == "1" || objFieldEnum.sUserType == "3" || objFieldEnum.sUserType == "5")
                //    {
                //        strQry += "'1')";
                //    }
                //    else if (objFieldEnum.sUserType == "6")
                //    {
                //        strQry += "'2')";
                //    }
                //    else if (objFieldEnum.sUserType == "2" || objFieldEnum.sUserType == "4")
                //    {
                //        strQry += "'3')";
                //    }

                //    ObjCon.ExecuteQry(strQry);

                //    //Save to DTC and TC Details
                //    objFieldEnum.sEnumDTCID = Convert.ToString(ObjCon.Get_max_no("DTE_ID", "TBLDTCENUMERATION"));                    


                //    if (objFieldEnum.bIsDTLMSDetails == true)
                //    {
                //        sIPCESCValue = "1";
                //    }
                //    if (objFieldEnum.bIsCESCDetails == true)
                //    {
                //        sIPCESCValue = "2";
                //    }
                //    if (objFieldEnum.bIsIPDetails == true)
                //    {
                //        sIPCESCValue = "3";
                //    }

                //    strQry = "INSERT INTO \"TBLDTCENUMERATION\"(\"DTE_ID\",\"DTE_ED_ID\",\"DTE_TC_CODE\",\"DTE_TC_SLNO\",\"DTE_MAKE\",\"DTE_CAPACITY\",";
                //    if (objFieldEnum.sTCManfDate == "")
                //    {
                //        strQry += "\"DTE_NAME\",\"DTE_DTCCODE\",\"DTE_CESCCODE\",\"DTE_IPCODE\",";
                //    }
                //    else
                //    {
                //        strQry += "\"DTE_TC_MANFDATE\",\"DTE_NAME\",\"DTE_DTCCODE\",\"DTE_CESCCODE\",\"DTE_IPCODE\",";
                //    }
                //    if (objFieldEnum.sEnumDate == "")
                //    {
                //        strQry += "\"DTE_TOTAL_CON_KW\",\"DTE_TOTAL_CON_HP\",\"DTE_KWH_READING\",";
                //    }
                //    else
                //    {
                //        strQry += "\"DTE_ENUM_DATE\",\"DTE_TOTAL_CON_KW\",\"DTE_TOTAL_CON_HP\",\"DTE_KWH_READING\",";
                //    }
                //    if (objFieldEnum.sCommisionDate == "")
                //    {
                //        strQry += " \"DTE_PLATFORM\",";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_TRANS_COMMISION_DATE\",\"DTE_PLATFORM\",";
                //    }
                //    if (objFieldEnum.sLastServiceDate == "")
                //    {
                //        strQry += "\"DTE_BREAKER_TYPE\",\"DTE_INTERNAL_CODE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\",\"DTE_LT_PROTECT\",\"DTE_GROUNDING\",";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_LAST_SERVICE_DATE\",\"DTE_BREAKER_TYPE\",\"DTE_INTERNAL_CODE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\",\"DTE_LT_PROTECT\",\"DTE_GROUNDING\",";
                //    }

                //    strQry += " \"DTE_ARRESTERS\",\"DTE_LOADTYPE\",\"DTE_PROJECTTYPE\",\"DTE_LT_LINE\",\"DTE_HT_LINE\",\"DTE_LONGITUDE\",\"DTE_LATITUDE\",\"DTE_DEPRECIATION\",\"DTE_CRBY\",\"DTE_ISIPCESC\",\"DTE_TANK_CAPACITY\",";
                //    strQry += " \"DTE_TC_WEIGHT\",\"DTE_INFOSYS_ASSET\",\"DTE_RATING\",\"DTE_STAR_RATE\") VALUES (";
                //    strQry += " '" + objFieldEnum.sEnumDTCID + "','" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sTcCode + "','" + objFieldEnum.sTCSlno + "',";
                //    strQry += " '" + objFieldEnum.sTCMake + "','" + objFieldEnum.sTCCapacity + "',";
                //    if (objFieldEnum.sTCManfDate == "")
                //    {
                //        strQry += " '" + objFieldEnum.sDTCName.ToUpper() + "','" + objFieldEnum.sDTCCode + "','" + objFieldEnum.sOldDTCCode + "','" + objFieldEnum.sIPDTCCode + "',";
                //    }
                //    else
                //    {
                //        strQry += "TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),'" + objFieldEnum.sDTCName + "','" + objFieldEnum.sDTCCode + "','" + objFieldEnum.sOldDTCCode + "','" + objFieldEnum.sIPDTCCode + "',";
                //    }
                //    if (objFieldEnum.sEnumDate == "")
                //    {
                //        strQry += "'" + objFieldEnum.sConnectedKW + "','" + objFieldEnum.sConnectedHP + "','" + objFieldEnum.sKWHReading + "',";
                //    }
                //    else
                //    {
                //        strQry += " TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),'" + objFieldEnum.sConnectedKW + "','" + objFieldEnum.sConnectedHP + "','" + objFieldEnum.sKWHReading + "',";
                //    }
                //    if (objFieldEnum.sCommisionDate == "")
                //    {
                //        strQry += "'" + objFieldEnum.sPlatformType + "',";
                //    }
                //    else
                //    {
                //        strQry += " TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),'" + objFieldEnum.sPlatformType + "',";
                //    }
                //    if (objFieldEnum.sLastServiceDate == "")
                //    {
                //        strQry += " '" + objFieldEnum.sBreakertype + "','" + objFieldEnum.sInternalCode + "','" + objFieldEnum.sDTCMeters + "','" + objFieldEnum.sHTProtect + "',";
                //    }
                //    else
                //    {
                //        strQry += " TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),'" + objFieldEnum.sBreakertype + "','" + objFieldEnum.sInternalCode + "','" + objFieldEnum.sDTCMeters + "','" + objFieldEnum.sHTProtect + "',";
                //    }
                //    strQry += " '" + objFieldEnum.sLTProtect + "','" + objFieldEnum.sGrounding + "','" + objFieldEnum.sArresters + "',";
                //    strQry += " '" + objFieldEnum.sLoadtype + "','" + objFieldEnum.sProjecttype + "','" + objFieldEnum.sLTlinelength + "','" + objFieldEnum.sHTlinelength + "','" + objFieldEnum.sLongitude + "',";
                //    strQry += " '" + objFieldEnum.sLatitude + "','" + objFieldEnum.sDepreciation + "','" + objFieldEnum.sCrBy + "','" + sIPCESCValue + "',";
                //    strQry += " '" + objFieldEnum.sTankCapacity + "','" + objFieldEnum.sTCWeight + "','" + objFieldEnum.sInfosysAsset + "','" + objFieldEnum.sRating + "','" + objFieldEnum.sStarRate + "')";
                //    ObjCon.ExecuteQry(strQry);

                //    ObjCon.CommitTransaction();

                //    Arr[0] = "Enumeration Details Saved Successfully";
                //    Arr[1] = "0";

                //    return Arr;
                //}
                //else
                //{
                //    ObjCon.BeginTransaction();

                //    strQry = "SELECT \"DTE_TC_CODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //    sRes = ObjCon.get_value(strQry);
                //    if (sRes != "")
                //    {
                //        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                //        Arr[1] = "2";
                //        return Arr;
                //    }

                //    strQry = "SELECT \"DTE_DTCCODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //    sRes = ObjCon.get_value(strQry);
                //    if (sRes != "")
                //    {
                //        Arr[0] = "DTC Code(DTLMS) " + objFieldEnum.sDTCCode + "  Already Exist";
                //        Arr[1] = "2";
                //        return Arr;
                //    }

                //    strQry = "SELECT \"DTE_TC_SLNO\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objFieldEnum.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')";
                //    sRes = ObjCon.get_value(strQry);
                //    if (sRes != "")
                //    {
                //        Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                //        Arr[1] = "2";
                //        return Arr;
                //    }

                //    strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_OFFICECODE\"='" + objFieldEnum.sOfficeCode + "',\"ED_OPERATOR1\"='" + objFieldEnum.sOperator1 + "',\"ED_OPERATOR2\"='" + objFieldEnum.sOperator2 + "'";
                //    if (objFieldEnum.sWeldDate == "")
                //    {
                //        strQry += ",\"ED_FEEDERCODE\"='" + objFieldEnum.sFeederCode + "',  ";
                //    }
                //    else
                //    {
                //        strQry += " ,\"ED_WELD_DATE\"=TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),\"ED_FEEDERCODE\"='" + objFieldEnum.sFeederCode + "',  ";
                //    }

                //    if (objFieldEnum.sUserType == "1" || objFieldEnum.sUserType == "3" || objFieldEnum.sUserType == "5")
                //    {
                //        strQry += " \"ED_UPDATE_BY\"='" + objFieldEnum.sCrBy + "', \"ED_UPDATE_ON\"=NOW(), ";
                //    }

                //    strQry += " \"ED_IP_ENUM_DONE\"='" + objFieldEnum.sIsIPEnumDone + "', ";

                //    //if(objFieldEnum.sUserType == "1"|| objFieldEnum.sUserType == "3" || objFieldEnum.sUserType == "5")
                //    //{
                //    //    strQry += "\"ED_APPROVE_STATUS\" = '1'";
                //    //}
                //    //else if(objFieldEnum.sUserType == "6")
                //    //{
                //    //    strQry += "\"ED_APPROVE_STATUS\" = '2'";
                //    //}
                //    //else if (objFieldEnum.sUserType == "2" || objFieldEnum.sUserType == "4")
                //    //{
                //    //    strQry += "\"ED_APPROVE_STATUS\" = '3'";
                //    //}
                //    clsPreQCApproval objpreQC = new clsPreQCApproval();
                //    string sLevel =  string.Empty;
                //    sLevel = objpreQC.GetNextApprovalLevel(objFieldEnum.sUserType);
                //    strQry += "\"ED_APPROVALPRIORITY\" = '"+ sLevel + "'";


                //    strQry += " WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                //    ObjCon.ExecuteQry(strQry);

                //    if (objFieldEnum.sUserType == "6" || objFieldEnum.sUserType == "7" || objFieldEnum.sUserType == "8")
                //    {
                //        string Qry = "INSERT INTO \"TBLINTERNALAPPROVALHISTORY\" (\"IA_ED_ID\",\"IA_ED_APPROVED_BY\",\"IA_ED_APPROVED_TYPE\") ";
                //        Qry += " VALUES('" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sCrBy + "','0')";
                //        ObjCon.ExecuteQry(Qry);
                //    }

                //    if (objFieldEnum.sProjecttype == "" || objFieldEnum.sProjecttype == null)
                //    {
                //        objFieldEnum.sProjecttype = "0";
                //    }

                //    strQry = "UPDATE \"TBLDTCENUMERATION\" SET \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "',\"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "',\"DTE_MAKE\"='" + objFieldEnum.sTCMake + "'";
                //    strQry += " ,\"DTE_CAPACITY\"='" + objFieldEnum.sTCCapacity + "',";
                //    if (objFieldEnum.sTCManfDate == "")
                //    {
                //        strQry += " \"DTE_NAME\"='" + objFieldEnum.sDTCName.Replace("'", "''").ToUpper() + "',\"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "',\"DTE_CESCCODE\"='" + objFieldEnum.sOldDTCCode + "',\"DTE_IPCODE\"='" + objFieldEnum.sIPDTCCode + "',";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_TC_MANFDATE\"=TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),\"DTE_NAME\"='" + objFieldEnum.sDTCName.Replace("'", "''") + "',\"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "',\"DTE_CESCCODE\"='" + objFieldEnum.sOldDTCCode + "',\"DTE_IPCODE\"='" + objFieldEnum.sIPDTCCode + "',";
                //    }
                //    if(objFieldEnum.sEnumDate == "")
                //    {
                //        strQry += "\"DTE_TOTAL_CON_KW\"='" + objFieldEnum.sConnectedKW + "',\"DTE_TOTAL_CON_HP\"='" + objFieldEnum.sConnectedHP + "',";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_ENUM_DATE\"=TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),\"DTE_TOTAL_CON_KW\"='" + objFieldEnum.sConnectedKW + "',\"DTE_TOTAL_CON_HP\"='" + objFieldEnum.sConnectedHP + "',";
                //    }
                //    if(objFieldEnum.sCommisionDate == "")
                //    {
                //        strQry += " \"DTE_KWH_READING\"='" + objFieldEnum.sKWHReading + "',";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_KWH_READING\"='" + objFieldEnum.sKWHReading + "',\"DTE_TRANS_COMMISION_DATE\"=TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),";
                //    }
                //    if(objFieldEnum.sLastServiceDate == "")
                //    {
                //        strQry += "\"DTE_PLATFORM\"='" + objFieldEnum.sPlatformType + "',\"DTE_BREAKER_TYPE\"='" + objFieldEnum.sBreakertype + "',";
                //    }
                //    else
                //    {
                //        strQry += " \"DTE_LAST_SERVICE_DATE\"=TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),\"DTE_PLATFORM\"='" + objFieldEnum.sPlatformType + "',\"DTE_BREAKER_TYPE\"='" + objFieldEnum.sBreakertype + "',";
                //    }

                //    strQry += " \"DTE_INTERNAL_CODE\"='" + objFieldEnum.sInternalCode + "',\"DTE_DTCMETERS\"='" + objFieldEnum.sDTCMeters + "',\"DTE_HT_PROTECT\"='" + objFieldEnum.sHTProtect + "',\"DTE_LT_PROTECT\"='" + objFieldEnum.sLTProtect + "'";
                //    strQry += " ,\"DTE_GROUNDING\"='" + objFieldEnum.sGrounding + "',";
                //    strQry += " \"DTE_ARRESTERS\"='" + objFieldEnum.sArresters + "',\"DTE_LOADTYPE\"='" + objFieldEnum.sLoadtype + "',\"DTE_PROJECTTYPE\"='" + objFieldEnum.sProjecttype + "',";
                //    strQry += " \"DTE_LT_LINE\"='" + objFieldEnum.sLTlinelength + "',\"DTE_HT_LINE\"='" + objFieldEnum.sHTlinelength + "',\"DTE_LONGITUDE\"='" + objFieldEnum.sLongitude + "',\"DTE_LATITUDE\"='" + objFieldEnum.sLatitude + "',";
                //    strQry += " \"DTE_DEPRECIATION\"='" + objFieldEnum.sDepreciation + "',\"DTE_TANK_CAPACITY\"='" + objFieldEnum.sTankCapacity + "',\"DTE_TC_WEIGHT\"='" + objFieldEnum.sTCWeight + "',\"DTE_INFOSYS_ASSET\"='" + objFieldEnum.sInfosysAsset + "',";
                //    strQry += " \"DTE_RATING\"='" + objFieldEnum.sRating + "',\"DTE_STAR_RATE\"='" + objFieldEnum.sStarRate + "' WHERE \"DTE_ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";

                //    ObjCon.ExecuteQry(strQry);

                //    if (objFieldEnum.sStatus == "2")
                //    {
                //        strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"='0' WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                //        ObjCon.ExecuteQry(strQry);
                //    }

                //    string sQry = "SELECT \"ED_INITIAL_UPDATE_ON\" FROM \"TBLENUMERATIONDETAILS\" WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                //    string sLastUpdate = ObjCon.get_value(sQry);
                //    if(sLastUpdate.Length == 0)
                //    {
                //        sQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_INITIAL_UPDATE_BY\" = '"+ objFieldEnum.sCrBy + "',  \"ED_INITIAL_UPDATE_ON\" = NOW() WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "' ";
                //        ObjCon.ExecuteQry(sQry);
                //    }

                //    ObjCon.CommitTransaction();

                //    Arr[0] = "Enumeration Details Updated Successfully";
                //    Arr[1] = "1";

                //    return Arr;
                //}

            }
            catch (Exception ex)
            {
                ObjCon.RollBackTrans();
                clsException.LogError(ex.StackTrace, ex.Message + strQry, strFormCode, "SaveFieldEnumerationDetails");
                return Arr;
            }
        }


        public bool UpdateImagePathDetails(clsNewDTCCR objCR)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;

                strQry = "SELECT \"EP_ED_ID\" FROM \"TBLENUMERATIONPHOTOS\" WHERE \"EP_ED_ID\"='" + objCR.sEnumDetailsID + "'";
                string sResult = ObjCon.get_value(strQry);
                if (sResult != "")
                {
                    strQry = "UPDATE \"TBLENUMERATIONPHOTOS\" SET \"EP_NAMEPLATE_PATH\"='" + objCR.sNamePlatePhotoPath + "',\"EP_SSPLATE_PATH\"='" + objCR.sSSPlatePhotoPath + "',";
                    strQry += " \"EP_OLDDTC_PATH\"='" + objCR.sOldCodePhotoPath + "',\"EP_DTLMSDTC_PATH\"='" + objCR.sDTLMSCodePhotoPath + "',";
                    strQry += " \"EP_IPENUMDTC_PATH\"='" + objCR.sIPEnumCodePhotoPath + "',\"EP_INFOSYSDTC_PATH\"='" + objCR.sInfosysCodePhotoPath + "',";
                    strQry += " \"EP_DTC_PATH\"='" + objCR.sDTCPhoto1Path + "' WHERE \"EP_ED_ID\"='" + objCR.sEnumDetailsID + "' ";
                    ObjCon.ExecuteQry(strQry);
                }
                else
                {
                    SaveImagePathDetails(objCR);
                }


                return true;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "UpdateImagePathDetails");
                return false;
            }
        }


        public clsNewDTCCR GetCRDetailsFromXML(clsNewDTCCR objRIApproval)
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                DataTable dt = new DataTable();

                dt = objApproval.GetDatatableFromXML(objRIApproval.sWFDataId);
                if (dt.Rows.Count > 0)
                {
                    objRIApproval.sCRDate = Convert.ToString(dt.Rows[0]["CR_DATE"]);
                    if (dt.Columns.Contains("EP_NAMEPLATE_PATH"))
                    {
                        objRIApproval.sNamePlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_NAMEPLATE_PATH"]);
                    }
                    if (dt.Columns.Contains("EP_SSPLATE_PATH"))
                    {
                        objRIApproval.sSSPlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_SSPLATE_PATH"]);
                    }
                    if (dt.Columns.Contains("EP_DTC_PATH"))
                    {
                        objRIApproval.sDTCPhoto1Path = Convert.ToString(dt.Rows[0]["EP_DTC_PATH"]);
                    }
                    if (dt.Columns.Contains("EP_OLDDTC_PATH"))
                    {
                        objRIApproval.sDTCPhoto2Path = Convert.ToString(dt.Rows[0]["EP_OLDDTC_PATH"]);
                    }
                    if (dt.Columns.Contains("EP_DTLMSDTC_PATH"))
                    {
                        objRIApproval.sDTLMSCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_DTLMSDTC_PATH"]);
                    }
                    if (dt.Columns.Contains("ED_ID"))
                    {
                        objRIApproval.sEnumDetailsID = Convert.ToString(dt.Rows[0]["ED_ID"]);
                    }
                    if (dt.Columns.Contains("EP_INFOSYSDTC_PATH"))
                    {
                        objRIApproval.sInfosysCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_INFOSYSDTC_PATH"]);
                    }
                    if (dt.Columns.Contains("ED_IP_ENUM_DONE"))
                    {
                        objRIApproval.sIPEnumCodePhotoPath = Convert.ToString(dt.Rows[0]["ED_IP_ENUM_DONE"]);
                    }
                    if (dt.Columns.Contains("ED_APPROVALPRIORITY"))
                    {
                        objRIApproval.sLevel = Convert.ToInt16(dt.Rows[0]["ED_APPROVALPRIORITY"]);
                    }
                }
                return objRIApproval;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objRIApproval;
            }
        }
    }
}
