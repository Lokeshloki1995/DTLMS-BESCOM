//**************************** Work Flow Concept******************************//
//*************************** Logic By : Ramesh Sir **************************//
//*************************** Code By : Priya *********************************//
//*************************** Last Update : 24/06/2016********************** //


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using IIITS.DTLMS.BL;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;
using IIITS.DTLMS.BL.DataBase;
using System.Net;

namespace IIITS.DTLMS.BL
{
    public class clsApproval
    {
        string strFormCode = "clsApproval";

        public string sfailid { get; set; }
        public string sbfm_type { get; set; }
        public string sbfm_id { get; set; }
        public string PrevBoId { get; set; }
        public string sdtccode { get; set; }
        public string sRoleId { get; set; }
        public string sBOId { get; set; }
        public string sAccessType { get; set; }
        public string sFormName { get; set; }
        public string sRefTypeforInbox { get; set; }
        public string sRecordId { get; set; }
        public string sOfficeCode { get; set; }
        public string sApproveStatus { get; set; }
        public string sPrevApproveId { get; set; }
        public string sClientIp { get; set; }
        public string sApproveComments { get; set; }
        public string sWFObjectId { get; set; }
        public string sQryValues { get; set; }
        public string sDescription { get; set; }
        public string sParameterValues { get; set; }
        public string sXMLdata { get; set; }
        public string sTableNames { get; set; }
        public string sColumnNames { get; set; }
        public string sColumnValues { get; set; }
        public string sCrby { get; set; }
        public string sMainTable { get; set; }
        public string sRefColumnName { get; set; }
        public string sApproveColumnName { get; set; }
        public string sBOFlowMasterId { get; set; }
        public string sPrevWFOId { get; set; }
        public string sWFAutoId { get; set; }
        public string sWFDataId { get; set; }
        public string sNewRecordId { get; set; }
        public string sWFInitialId { get; set; }
        public string sDataReferenceId { get; set; }
        public string sRefOfficeCode { get; set; }
        public string sFromDate { get; set; }
        public string sToDate { get; set; }
        public string sFailType { get; set; }
        public string sRoleType { get; set; }
        public string sGuarentyType { get; set; }
        public string sStoreType { get; set; }
        public string sFilePath { get; set; }
        public string sMaterialfoloi { get; set; }
        public string sStatus { get; set; }
        public string sTTKSComstatus { get; set; }
        public string failureId { get; set; }
        public long indentno { get; set; }
        int Division;
        int SubDivision;
        int Section;
        //Approve Status
        // 0--->Pending    1---->Approved    2----> Modify and Approve   3--> Reject   4----> Abort

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        /// <summary>
        /// To check Access of Forms Based on Roles
        /// </summary>
        /// <param name="objApproval">Role Id, Form Name</param>
        /// <returns></returns>
        /// 
        NpgsqlCommand NpgsqlCommand;
        public string sTTKStatus { get; set; }

        public bool CheckAccessRights(clsApproval objApproval)
        {
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            sFailType = "0";
            try
            {
                if (Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["AccessRights"]).ToUpper().Equals("OFF"))
                {
                    return true;
                }
                NpgsqlCommand = new NpgsqlCommand();
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY
                strQry = "SELECT \"UR_ACCESSTYPE\" FROM \"TBLUSERROLEMAPPING\" WHERE \"UR_ROLEID\" ='" + objApproval.sRoleId + "' AND \"UR_BOID\" IN ";
                strQry += " (SELECT \"BO_ID\" FROM \"TBLBUSINESSOBJECT\" WHERE UPPER(\"BO_FORMNAME\")='" + objApproval.sFormName.Trim().ToUpper() + "') ";
                strQry += " AND \"UR_ACCESSTYPE\" IN (" + objApproval.sAccessType + ")  ORDER BY \"UR_ACCESSTYPE\"";

                dt = objcon.FetchDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// Load Approval Inbox with Pending Details
        /// </summary>
        /// <param name="objApproval">Office Code,Role Id</param>
        /// <returns></returns>
        public DataTable LoadPendingApprovalInbox(clsApproval objApproval)
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string strQry = string.Empty;
                strQry = "SELECT \"WO_DATA_ID\",\"WO_ID\", \"WO_RECORD_ID\", \"WO_BO_ID\", \"BO_NAME\",\"WO_REF_OFFCODE\", \"USER_NAME\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" ";
                strQry += " WHERE \"US_ID\"=( SELECT DISTINCT \"first_value\"(B. \"WO_CR_BY\") OVER(ORDER BY B.\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" ";
                strQry += " B WHERE B. \"WO_INITIAL_ID\" = A. \"WO_INITIAL_ID\" )) AS \"CREATOR\", \"CR_ON\", \"STATUS\", \"WO_APPROVE_STATUS\", ";
                strQry += " \"RO_NAME\", \"CURRENT_STATUS\", \"WO_DESCRIPTION\", \"WOA_ID\", \"WO_WFO_ID\", \"WO_INITIAL_ID\" FROM (SELECT \"WO_DATA_ID\",\"WO_ID\" ";
                strQry += " , \"WO_RECORD_ID\", \"WO_BO_ID\",\"WO_REF_OFFCODE\", \"BO_NAME\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"WO_CR_BY\"= \"US_ID\" ) ";
                strQry += " \"USER_NAME\", TO_CHAR(\"WO_CR_ON\",'DD-MON-YYYY HH24:MI') AS \"CR_ON\",";
                strQry += " (CASE \"WO_APPROVE_STATUS\" WHEN '0' THEN 'PENDING' WHEN '1' THEN 'APPROVED' WHEN '2' THEN 'MODIFY AND APPROVE' WHEN '3' THEN 'REJECTED' ELSE 'OTHERS' END) \"STATUS\", \"WO_APPROVE_STATUS\" , ";
                strQry += " (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\"=\"RO_ID\") \"RO_NAME\",'' AS \"CURRENT_STATUS\",\"WO_DESCRIPTION\",0 AS \"WOA_ID\",\"WO_WFO_ID\",\"WO_INITIAL_ID\" ";
                strQry += " FROM \"TBLWORKFLOWOBJECTS\", \"TBLBUSINESSOBJECT\" WHERE \"WO_BO_ID\"= \"BO_ID\"  ";
                strQry += " AND \"WO_APPROVE_STATUS\"='0'  ";

                if (objApproval.sRoleId != Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    strQry += " AND  \"WO_NEXT_ROLE\" ='" + objApproval.sRoleId + "' ";
                }
                if (objApproval.sRoleType == "2")
                {
                    strQry += " AND (";
                    string sOffCode = clsStoreOffice.GetOfficeCode(objApproval.sOfficeCode, "WO_REF_OFFCODE");
                    strQry += sOffCode;
                    if (objApproval.sOfficeCode == "2" && sOffCode == "")
                    {
                        string SM_CODE = objcon.get_value("SELECT \"SM_CODE\" FROM \"TBLSTOREMAST\" WHERE cast(\"SM_ID\" as text) = '" + objApproval.sOfficeCode + "'");
                        strQry += " (CAST(\"WO_REF_OFFCODE\" AS TEXT) LIKE '" + SM_CODE + "%')";
                        strQry += " AND  \"BO_ID\" NOT IN (29,15,13)";
                    }
                    //30-03-220 BY MADAN FOR STORE SPLIT 
                    if (objApproval.sOfficeCode == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWSTORE"]))
                    {
                        strQry += " or CAST(\"WO_STATUS\"  AS TEXT) ='1'";
                    }
                    if (objApproval.sOfficeCode == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["OLDSTORE"]))
                    {
                        strQry += " and (CAST(\"WO_STATUS\"  AS TEXT) is null or  cast(\"WO_STATUS\" as text)='')";
                    }
                    strQry += ")";
                }
                else
                {
                    strQry += " AND CAST(\"WO_REF_OFFCODE\"  AS TEXT) LIKE '" + objApproval.sOfficeCode + "%'";
                }
                if (objApproval.sBOId != null)
                {
                    strQry += " AND \"WO_BO_ID\" =:sBOId ";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')>=:dFromDate";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')<=:dToDate";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate", dToDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WO_DESCRIPTION\") LIKE '%'|| :sDescription ||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription", objApproval.sDescription.ToUpper());
                }
                strQry += " UNION ALL ";
                strQry += " SELECT \"WO_DATA_ID\",\"WO_ID\", \"WO_RECORD_ID\",(SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\"= \"WOA_BFM_ID\") AS ";
                strQry += " \"WO_BO_ID\",\"WO_REF_OFFCODE\", \"BO_NAME\", (SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"WOA_CRBY\" = \"US_ID\") \"USER_NAME\",";
                strQry += "  TO_CHAR(\"WOA_CRON\",'DD-MON-YYYY HH24:MI') \"CR_ON\", 'PENDING' AS \"STATUS\",0 AS \"WO_APPROVE_STATUS\", ";
                strQry += " (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WOA_ROLE_ID\" = \"RO_ID\") \"RO_NAME\",'' AS \"CURRENT_STATUS\", \"WOA_DESCRIPTION\" ";
                strQry += " AS WO_DESCRIPTION, \"WOA_ID\" ,'0' AS WO_WFO_ID, \"WO_INITIAL_ID\" FROM \"TBLWO_OBJECT_AUTO\", \"TBLBUSINESSOBJECT\", \"TBLWORKFLOWOBJECTS\" WHERE ";
                strQry += " (SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\" = \"WOA_BFM_ID\") = \"BO_ID\" AND ";
                strQry += " \"WOA_INITIAL_ACTION_ID\" IS NULL AND \"WOA_PREV_APPROVE_ID\" = \"WO_ID\" AND ";

                if (objApproval.sRoleId != Convert.ToString(ConfigurationManager.AppSettings["SupAdminRole"]))
                {
                    strQry += "  \"WOA_ROLE_ID\" ='" + objApproval.sRoleId + "' and  ";
                }
                NpgsqlCommand.Parameters.AddWithValue("sRoleId1", Convert.ToInt32(objApproval.sRoleId));

                if (objApproval.sRoleType == "2")
                {
                    strQry += "(";
                    string sOffCode = clsStoreOffice.GetOfficeCode(objApproval.sOfficeCode, "WOA_REF_OFFCODE");
                    strQry += sOffCode;
                    if (objApproval.sOfficeCode == "2" && sOffCode == "")
                    {
                        string SM_CODE = objcon.get_value("SELECT \"SM_CODE\" FROM \"TBLSTOREMAST\" WHERE cast(\"SM_ID\" as text) = '" + objApproval.sOfficeCode + "'");
                        strQry += " (CAST(\"WOA_REF_OFFCODE\" AS TEXT) LIKE '" + SM_CODE + "%')";
                        strQry += " AND  \"BO_ID\" NOT IN (29,15,13)";
                    }
                    //30-03-2020 by madan for store split ups
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["NEWSTORE"]))
                    {
                        strQry += " or CAST(\"WOA_STATUS\"  AS TEXT) ='1'";
                    }
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["OLDSTORE"]))
                    {
                        strQry += " and (CAST(\"WOA_STATUS\"  AS TEXT) is null or  cast(\"WOA_STATUS\" as text)='')";
                    }
                    strQry += ")";
                }
                else
                {
                    strQry += " CAST(\"WOA_REF_OFFCODE\"  AS TEXT) LIKE '" + objApproval.sOfficeCode + "%'";
                }
                if (objApproval.sBOId != null)
                {
                    strQry += " AND (SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\"=\"WOA_BFM_ID\")=:sBOId1 ";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby1";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby1", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')>=:dFromDate1";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate1", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')<=:dToDate1";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate1", dToDate.ToString("yyyyMMdd"));
                }

                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName2||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName2", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WOA_DESCRIPTION\") LIKE '%'|| :sDescription2 ||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription2", objApproval.sDescription.ToUpper());
                }

                strQry += " ORDER BY \"WO_ID\" DESC)A";
                if (objApproval.sRoleId != "12")
                {
                    strQry += "  LIMIT 300";
                }

                return objcon.FetchDataTable(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        /// <summary>
        /// Load Approval Inbox with Already Approved Details
        /// </summary>
        /// <param name="objApproval">Office Code,Role Id</param>
        /// <returns></returns>
        public DataTable LoadAlreadyApprovedInbox(clsApproval objApproval)
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string strQry = string.Empty;
                strQry = " SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",\"WO_BO_ID\",\"BO_NAME\",\"WO_REF_OFFCODE\",\"USER_NAME\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE ";
                strQry += " \"US_ID\"=(SELECT DISTINCT \"first_value\"(B. \"WO_CR_BY\") OVER(ORDER BY B.\"WO_ID\" DESC) FROM \"TBLWORKFLOWOBJECTS\" B ";
                strQry += " WHERE B.\"WO_INITIAL_ID\" = A.\"WO_INITIAL_ID\")) AS CREATOR,\"CR_ON\",\"STATUS\",\"WO_APPROVE_STATUS\",\"RO_NAME\", ";
                strQry += " \"NEXT_ROLE\",\"CURRENT_STATUS\",\"WO_DESCRIPTION\",\"WOA_ID\", \"WO_WFO_ID\",\"WO_INITIAL_ID\" FROM ";
                strQry += " (SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",\"WO_BO_ID\",\"BO_NAME\",\"WO_REF_OFFCODE\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE ";
                strQry += " \"WO_CR_BY\"=\"US_ID\") \"USER_NAME\",TO_CHAR(\"WO_CR_ON\",'DD-MON-YYYY HH24:MI') \"CR_ON\",";
                strQry += " (CASE \"WO_APPROVE_STATUS\" WHEN '0' THEN 'PENDING' WHEN '1' THEN 'APPROVED' WHEN '2' THEN 'MODIFY AND APPROVE' END) \"STATUS\",\"WO_APPROVE_STATUS\",";
                strQry += " (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\"=\"RO_ID\") \"RO_NAME\",";
                strQry += " (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"=(SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"= A.\"WO_RECORD_ID\" )) \"NEXT_ROLE\",";
                strQry += " CASE (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"=(SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\" = A.\"WO_RECORD_ID\" ))";
                strQry += " WHEN 0 THEN 'APPROVED' ELSE 'PENDING' END \"CURRENT_STATUS\",\"WO_DESCRIPTION\",0 AS \"WOA_ID\",";
                strQry += " \"WO_WFO_ID\", \"WO_INITIAL_ID\" ";
                strQry += " FROM \"TBLWORKFLOWOBJECTS\" A,\"TBLBUSINESSOBJECT\" B WHERE \"WO_BO_ID\"=\"BO_ID\"  AND \"WO_PREV_APPROVE_ID\" in (SELECT \"WO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_APPROVE_STATUS\" <> 3 AND \"WO_APPROVED_BY\"=:sCrby) ";
                strQry += "  AND \"WO_APPROVE_STATUS\" <> 0  ";
                NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                if (objApproval.sRoleType == "2")
                {
                    strQry += "AND (";
                    string sOffCode = clsStoreOffice.GetOfficeCode(objApproval.sOfficeCode, "WO_REF_OFFCODE"); ////by rudra changed wo to woref on 2704
                    strQry += sOffCode;

                    //30-03-220 BY MADAN FOR STORE SPLIT 
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["NEWSTORE"]))
                    {
                        strQry += " or CAST(\"WO_STATUS\"  AS TEXT) ='1'";
                    }
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["OLDSTORE"]))
                    {
                        strQry += " and (CAST(\"WO_STATUS\"  AS TEXT) is null or  cast(\"WO_STATUS\" as text)='')";
                    }
                    strQry += ")";
                }
                if (objApproval.sRoleType == "1")
                {
                    strQry += " AND CAST(\"WO_OFFICE_CODE\"  AS TEXT) LIKE :sOfficeCode||'%'"; // before it is WO_REF_OFFCODE with like condition
                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", objApproval.sOfficeCode);
                }
                if (objApproval.sBOId != null)
                {
                    strQry += " AND \"WO_BO_ID\" =:sBOId ";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby1 ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby1", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')>=:dFromDate";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')<=:dToDate";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate", dToDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName1||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName1", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WO_DESCRIPTION\") LIKE '%'|| :sDescription1 ||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription1", objApproval.sDescription.ToUpper());
                }
                strQry += " UNION ALL  ";
                strQry += " SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",(SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\"=\"WOA_BFM_ID\") AS \"WO_BO_ID\",";
                strQry += " \"BO_NAME\",\"WO_REF_OFFCODE\", (SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE  \"US_ID\"=(SELECT DISTINCT \"first_value\"(B. \"WO_CR_BY\") ";
                strQry += " OVER(ORDER BY B.\"WO_ID\" DESC) FROM \"TBLWORKFLOWOBJECTS\" B  WHERE B.\"WO_INITIAL_ID\" = A.\"WO_INITIAL_ID\")) AS \"USER_NAME\" ,TO_CHAR(\"WOA_CRON\",'DD-MON-YYYY HH24:MI') \"CR_ON\" ,";
                strQry += " 'INITIATED' \"STATUS\",\"WO_APPROVE_STATUS\",";
                strQry += "  (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WOA_ROLE_ID\"=\"RO_ID\") \"RO_NAME\", ";
                strQry += " (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"=(SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                strQry += " \"WO_RECORD_ID\"= A.\"WO_RECORD_ID\" )) \"NEXT_ROLE\" ,CASE (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =(SELECT MAX(\"WO_ID\") ";
                strQry += " FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\" = A.\"WO_RECORD_ID\" )) WHEN 0 THEN 'APPROVED' ELSE 'PENDING' END \"CURRENT_STATUS\",\"WOA_DESCRIPTION\" ";
                strQry += " AS \"WO_DESCRIPTION\",'0' AS \"WOA_ID\",\"WO_WFO_ID\",\"WO_INITIAL_ID\"  FROM \"TBLWO_OBJECT_AUTO\",\"TBLBUSINESSOBJECT\",\"TBLWORKFLOWOBJECTS\" A ";
                strQry += " WHERE  (SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\"=\"WOA_BFM_ID\") = \"BO_ID\" AND  \"WOA_INITIAL_ACTION_ID\" IS NOT NULL ";
                strQry += " AND \"WOA_INITIAL_ACTION_ID\" = \"WO_INITIAL_ID\" ";

                if (objApproval.sRoleType == "2")
                {
                    strQry += "AND (";
                    string sOffCode = clsStoreOffice.GetOfficeCode(objApproval.sOfficeCode, "WOA_OFFICE_CODE");
                    strQry += sOffCode;
                    //30-03-2020 by madan for store split ups
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["NEWSTORE"]))
                    {
                        strQry += " or CAST(\"WOA_STATUS\"  AS TEXT) ='1'";
                    }
                    if (objApproval.sOfficeCode == Convert.ToString(ConfigurationManager.AppSettings["OLDSTORE"]))
                    {
                        strQry += " and (CAST(\"WOA_STATUS\"  AS TEXT) is null or  cast(\"WO_STATUS\" as text)='')";
                    }
                    strQry += ")";
                }
                if (objApproval.sRoleType == "1")
                {
                    strQry += " AND CAST(\"WOA_OFFICE_CODE\"  AS TEXT) LIKE :sOfficeCode2||'%'";
                    if (objApproval.sRoleId == "40" || objApproval.sRoleId == "41" || objApproval.sRoleId == "38" || objApproval.sRoleId == "31" || objApproval.sRoleId == "35" || objApproval.sRoleId == "36")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode2", objApproval.sOfficeCode);
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode2", objApproval.sOfficeCode.Substring(0, 3));                       
                    }
                }
                if (objApproval.sBOId != null)
                {
                    strQry += " AND (SELECT \"BFM_NEXT_BO_ID\" FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_ID\" = \"WOA_BFM_ID\" )=:sBOId3 ";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId3", Convert.ToInt32(objApproval.sBOId));
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby3 ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby3", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WOA_CRON\",'YYYYMMDD')>=:dFromDate1";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate1", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WOA_CRON\",'YYYYMMDD')<=:dToDate1";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate1", dToDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName4||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName4", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WOA_DESCRIPTION\") LIKE '%'||:sDescription4||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription4", objApproval.sDescription.ToUpper());
                }
                strQry += " UNION ALL SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",\"WO_BO_ID\",\"BO_NAME\",\"WO_REF_OFFCODE\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE ";
                strQry += " \"US_ID\" =\"WO_CR_BY\")AS \"USER_NAME\",TO_CHAR(\"WO_CR_ON\",'DD-MON-YYYY HH24:MI') \"CR_ON\" ,'INITIATED' \"STATUS\",";
                strQry += " \"WO_APPROVE_STATUS\",'' AS \"RO_NAME\",'0' AS \"NEXT_ROLE\",'APPROVED' AS \"CURRENT_STATUS\",\"WO_DESCRIPTION\",";
                strQry += " '0' AS \"WOA_ID\",\"WO_WFO_ID\",\"WO_INITIAL_ID\"  FROM \"TBLWORKFLOWOBJECTS\",\"TBLBUSINESSOBJECT\" WHERE ";
                strQry += " \"WO_BO_ID\" =\"BO_ID\" AND \"WO_PREV_APPROVE_ID\" IN (SELECT \"WO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                strQry += " \"WO_NEXT_ROLE\" =:sRoleId3 and \"WO_PREV_APPROVE_ID\" IN (SELECT \"WO_ID\" FROM ";
                strQry += " \"TBLWORKFLOWOBJECTS\" WHERE \"WO_APPROVE_STATUS\"='3')) ";
                NpgsqlCommand.Parameters.AddWithValue("sRoleId3", Convert.ToInt32(objApproval.sRoleId));
                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName5||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName5", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WO_DESCRIPTION\") LIKE '%'||:sDescription5||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription5", objApproval.sDescription.ToUpper());
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby4 ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby4", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')>=:dFromDate4";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate4", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')<=:dToDate4";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate4", dToDate.ToString("yyyyMMdd"));
                }

                strQry += " UNION ALL SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",\"WO_BO_ID\",\"BO_NAME\",\"WO_REF_OFFCODE\", ";
                strQry += "    (SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE  \"US_ID\" =\"WO_CR_BY\")AS \"USER_NAME\", ";
                strQry += "    TO_CHAR(\"WO_CR_ON\",'DD-MON-YYYY HH24:MI') \"CR_ON\" ,'INITIATED' \"STATUS\", ";
                strQry += "    \"WO_APPROVE_STATUS\",'' AS \"RO_NAME\",'0' AS \"NEXT_ROLE\",'APPROVED' AS \"CURRENT_STATUS\", ";
                strQry += "    \"WO_DESCRIPTION\", '0' AS \"WOA_ID\",\"WO_WFO_ID\",\"WO_INITIAL_ID\"  FROM \"TBLWORKFLOWOBJECTS\", ";
                strQry += "    \"TBLBUSINESSOBJECT\" WHERE  \"WO_BO_ID\" =\"BO_ID\" AND \"WO_BO_ID\" ='71' ";


                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName6||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName6", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WO_DESCRIPTION\") LIKE '%'||:sDescription6||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription6", objApproval.sDescription.ToUpper());
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby6 ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby6", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')>=:dFromDate6";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate6", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\",'YYYYMMDD')<=:dToDate6";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate6", dToDate.ToString("yyyyMMdd"));
                }


                strQry += " )A  ORDER BY \"WO_ID\" DESC LIMIT 300";
                return objcon.FetchDataTable(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// Load Approval Inbox with Already Approved Details
        /// </summary>
        /// <param name="objApproval">Office Code,Role Id</param>
        /// <returns></returns>
        public DataTable LoadRejectedApprovedInbox(clsApproval objApproval)
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string strQry = string.Empty;
                strQry = " SELECT * FROM (SELECT \"WO_DATA_ID\",\"WO_ID\",\"WO_RECORD_ID\",\"WO_BO_ID\",\"BO_NAME\",(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"WO_CR_BY\"=\"US_ID\") USER_NAME,TO_CHAR(\"WO_CR_ON\",'DD-MON-YYYY HH24:MI') CR_ON,(SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"US_ID\" =(SELECT DISTINCT \"first_value\"(B. \"WO_CR_BY\") OVER(ORDER BY B.\"WO_ID\" DESC) FROM \"TBLWORKFLOWOBJECTS\" B WHERE B.\"WO_INITIAL_ID\" = A.\"WO_INITIAL_ID\")) AS CREATOR,";
                strQry += " (CASE \"WO_APPROVE_STATUS\" WHEN '0' THEN 'PENDING' WHEN '1' THEN 'APPROVED' WHEN '2' THEN 'MODIFY AND APPROVE' WHEN '3' THEN 'REJECTED' ELSE 'OTHERS' END) \"STATUS\", \"WO_APPROVE_STATUS\" ,";
                strQry += " (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\"=\"RO_ID\") RO_NAME,";
                strQry += " (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =(SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\" = A.\"WO_RECORD_ID\" )) NEXT_ROLE,";
                strQry += " CASE (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =(SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\" = A.\"WO_RECORD_ID\" ))";
                strQry += " WHEN 0 THEN 'APPROVED' ELSE 'PENDING' END CURRENT_STATUS,\"WO_DESCRIPTION\",0 AS WOA_ID,";
                strQry += " \"WO_WFO_ID\", \"WO_INITIAL_ID\",\"WO_REF_OFFCODE\" ";
                strQry += " FROM \"TBLWORKFLOWOBJECTS\" A, \"TBLBUSINESSOBJECT\" B WHERE \"WO_BO_ID\" = \"BO_ID\"  AND \"WO_NEXT_ROLE\" =:sRoleId";
                strQry += "  AND \"WO_APPROVE_STATUS\" ='3'  ";
                NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));

                if (objApproval.sRoleType == "2")
                {
                    string sOffCode = clsStoreOffice.GetOfficeCode(objApproval.sOfficeCode, "WO_REF_OFFCODE");
                    strQry += "AND (" + sOffCode + ")";

                    //30-03-220 BY MADAN FOR STORE SPLIT 
                    if (objApproval.sOfficeCode == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["NEWSTORE"]))
                    {
                        strQry += " or CAST(\"WO_STATUS\"  AS TEXT) ='1')";
                    }
                    if (objApproval.sOfficeCode == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["OLDSTORE"]))
                    {
                        strQry += " and (CAST(\"WO_STATUS\"  AS TEXT) is null or  cast(\"WO_STATUS\" as text)=''))";
                    }
                }
                if (objApproval.sRoleType == "1")
                {
                    strQry += " AND CAST(\"WO_REF_OFFCODE\"  AS TEXT) LIKE :sOfficeCode||'%'";
                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", objApproval.sOfficeCode);
                }
                if (objApproval.sBOId != null)
                {
                    strQry += " AND \"WO_BO_ID\" =:sBOId1 ";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                }
                if (objApproval.sCrby != null)
                {
                    strQry += " AND \"WO_CR_BY\" =:sCrby1 ";
                    NpgsqlCommand.Parameters.AddWithValue("sCrby1", Convert.ToInt32(objApproval.sCrby));
                }
                if (objApproval.sFromDate != "")
                {
                    DateTime dFromDate = DateTime.ParseExact(objApproval.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\" ,'YYYYMMDD')>=:dFromDate";
                    NpgsqlCommand.Parameters.AddWithValue("dFromDate", dFromDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sToDate != "")
                {
                    DateTime dToDate = DateTime.ParseExact(objApproval.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"WO_CR_ON\" ,'YYYYMMDD')<=:dToDate";
                    NpgsqlCommand.Parameters.AddWithValue("dToDate", dToDate.ToString("yyyyMMdd"));
                }
                if (objApproval.sFormName != "")
                {
                    strQry += " AND UPPER(\"BO_NAME\") LIKE :sFormName2||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sFormName2", objApproval.sFormName.ToUpper());
                }
                if (objApproval.sDescription != "")
                {
                    strQry += " AND UPPER(\"WO_DESCRIPTION\") LIKE '%'||:sDescription2||'%' ";
                    NpgsqlCommand.Parameters.AddWithValue("sDescription2", objApproval.sDescription.ToUpper());
                }
                strQry += " ORDER BY \"WO_ID\" DESC) A  LIMIT 300";
                return objcon.FetchDataTable(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// Save Workflow Data to TBLWFODATA table like QueryValues,ParameterValues and XML format
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool SaveWorkFlowData1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                if (objApproval.sTableNames != "" && objApproval.sTableNames != null)
                {
                    sResult = CreateXml(objApproval.sColumnNames, objApproval.sColumnValues, objApproval.sTableNames);
                    sResult = sResult.Replace("'", "''");
                }
                objApproval.sWFDataId = Convert.ToString(objDatabse.Get_max_no("WFO_ID", "TBLWFODATA"));

                strQry = "INSERT INTO \"TBLWFODATA\" (\"WFO_ID\",\"WFO_QUERY_VALUES\",\"WFO_PARAMETER\",\"WFO_DATA\",\"WFO_CR_BY\") VALUES (";
                strQry += " '" + objApproval.sWFDataId + "','" + objApproval.sQryValues + "','" + objApproval.sParameterValues + "','" + sResult + "',";
                strQry += " '" + objApproval.sCrby + "')";
                objDatabse.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool SaveWorkFlowData(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                if (objApproval.sTableNames != "" && objApproval.sTableNames != null)
                {
                    sResult = CreateXml(objApproval.sColumnNames, objApproval.sColumnValues, objApproval.sTableNames);
                    sResult = sResult.Replace("'", "''");
                }
                objApproval.sWFDataId = Convert.ToString(objcon.Get_max_no("WFO_ID", "TBLWFODATA"));
                strQry = "INSERT INTO \"TBLWFODATA\" (\"WFO_ID\",\"WFO_QUERY_VALUES\",\"WFO_PARAMETER\",\"WFO_DATA\",\"WFO_CR_BY\") VALUES (";
                strQry += " '" + objApproval.sWFDataId + "','" + objApproval.sQryValues + "','" + objApproval.sParameterValues + "','" + sResult + "',";
                strQry += " '" + objApproval.sCrby + "')";
                objcon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Generate Temporary Record Id to Insert in TBLWORKFLOWOBJECTS Table. i.e -1,-2 etc
        /// </summary>
        /// <returns></returns>
        public string GetRecordIdForWorkFlow()
        {
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT  COALESCE(MIN(\"WO_RECORD_ID\"),0)-1 FROM \"TBLWORKFLOWOBJECTS\"";
                string sResult = objcon.get_value(strQry);
                if (Convert.ToInt32(sResult) >= 0)
                {
                    sResult = "-1";
                }
                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetRecordIdForWorkFlow1(DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT  COALESCE(MIN(\"WO_RECORD_ID\"),0)-1 FROM \"TBLWORKFLOWOBJECTS\"";
                string sResult = objDatabse.get_value(strQry);
                if (Convert.ToInt32(sResult) >= 0)
                {
                    sResult = "-1";
                }
                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }


        /// <summary>
        /// Save WorkFlow object like Who is Next Role to access the Bussiness Object.
        /// TBLWORKFLOWOBJECTS Table Contains  Transaction of WorkFlow based on Businens Object
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool SaveWorkflowObjects(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                string sApproveResult = string.Empty;
                if (Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["Approval"]).ToUpper().Equals("OFF"))
                {
                    return false;
                }

                if (objApproval.sFormName != null && objApproval.sFormName != "")
                {
                    //To get Business Object Id
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sFormName", objApproval.sFormName.Trim().ToUpper());
                    objApproval.sBOId = objcon.get_value("SELECT \"BO_ID\" FROM \"TBLBUSINESSOBJECT\" WHERE UPPER(\"BO_FORMNAME\")=:sFormName", NpgsqlCommand);
                }
                //fetching bfm_id from bo_id for updating intial id based on previosapproveid  n initialactionid
                if (objApproval.sbfm_type != null && objApproval.sbfm_type != "")
                {
                    objApproval.sbfm_id = objcon.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' and \"BFM_TYPE\"='" + objApproval.sbfm_type + "'");
                }
                else if (objApproval.sTTKSComstatus == "1")
                {
                    objApproval.sbfm_id = objcon.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' and \"BFM_BO_ID\"=11 ");
                }
                else
                {
                    objApproval.sbfm_id = objcon.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' ");
                    // added to handel two record form queary: SELECT * from "TBLBO_FLOW_MASTER" WHERE "BFM_NEXT_BO_ID" = 65
                    // this is modified on 29 - 11 - 2022.
                    // BFM_ID = 23 on BFM_NEXT_BO_ID = 65 is the correct flow BFM_ID = 24 is not used yet
                    if (objApproval.sbfm_id == "24" && objApproval.sBOId == "65")
                    {
                        objApproval.sbfm_id = "23";
                    }
                }
                if (objApproval.sBOId == "62")
                {
                    objApproval.sbfm_id = "22";
                }
                if (objApproval.sBOId == "46")
                {
                    objApproval.sbfm_id = objcon.get_value("SELECT \"WOA_BFM_ID\" from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + objApproval.sWFObjectId + "' ");
                }
                //Check Approval Exists
                bool bResult = CheckFormApprovalExists(objApproval);
                if (bResult == true)
                {
                    //To get Role Id with Priority Level                 
                    objApproval.sRoleId = GetRoleFromApprovePriority(objApproval);
                }
                else
                {
                    objApproval.sRoleId = "";
                }
                if (objApproval.sPrevApproveId == null)
                {
                    objApproval.sPrevApproveId = "0";
                }
                if (objApproval.sRoleId != "" && objApproval.sRoleId != null)
                {
                    // SaveWorkFlowData(objApproval);
                    objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    objcon.BeginTransaction();

                    string sWFlowId = Convert.ToString(objcon.get_value("SELECT COALESCE (MAX(\"WO_ID\")+1,1) FROM \"TBLWORKFLOWOBJECTS\""));
                    objApproval.sWFObjectId = sWFlowId;
                    strQry = "INSERT INTO \"TBLWORKFLOWOBJECTS\" (\"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_NEXT_ROLE\",\"WO_OFFICE_CODE\",";
                    strQry += " \"WO_CLIENT_IP\",\"WO_CR_BY\",\"WO_DESCRIPTION\",\"WO_WFO_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\",\"WO_STATUS\")";
                    strQry += " VALUES (:sWFlowId1,:sBOId2,:sRecordId2,:sPrevApproveId2,";
                    strQry += " :sRoleId3,:sOfficeCode3,:sClientIp,:sCrby4,";
                    strQry += " :sDescription4,:sWFDataId4,:sDataReferenceId3,:sRefOfficeCode4,:status)";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFlowId1", Convert.ToInt64(sWFlowId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId2", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId2", Convert.ToInt64(objApproval.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sPrevApproveId2", Convert.ToInt64(objApproval.sPrevApproveId));
                    NpgsqlCommand.Parameters.AddWithValue("sRoleId3", Convert.ToInt32(objApproval.sRoleId));
                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode3", Convert.ToInt32(objApproval.sOfficeCode));

                    if (objApproval.sClientIp == null || objApproval.sClientIp == "")
                    {
                        objApproval.sClientIp = "";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("sClientIp", objApproval.sClientIp);
                    NpgsqlCommand.Parameters.AddWithValue("sCrby4", Convert.ToInt32(objApproval.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("sDescription4", objApproval.sDescription);
                    NpgsqlCommand.Parameters.AddWithValue("sWFDataId4", objApproval.sWFDataId);
                    if (objApproval.sDataReferenceId == null || objApproval.sDataReferenceId == "")
                    {
                        objApproval.sDataReferenceId = "";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId3", objApproval.sDataReferenceId);
                    if (objApproval.sRefOfficeCode == "")
                    {
                        objApproval.sRefOfficeCode = null;
                        NpgsqlCommand.Parameters.AddWithValue("sRefOfficeCode4", null);
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sRefOfficeCode4", Convert.ToInt32(objApproval.sRefOfficeCode));
                    }
                    if (objApproval.sStatus == null || objApproval.sStatus == "")
                    {
                        objApproval.sStatus = "";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("status", objApproval.sStatus);

                    objcon.ExecuteQry(strQry, NpgsqlCommand);

                    //Insert Initial Id for WorkFlow
                    if (objApproval.sWFInitialId == "" || objApproval.sWFInitialId == null)
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\"=:sWFlowId WHERE CAST(\"WO_ID\" AS TEXT) =:sWFlowId1";
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId", Convert.ToInt64(sWFlowId));
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId1", sWFlowId);
                        objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =:sWFInitialId1 WHERE CAST(\"WO_ID\" AS TEXT) =:sWFlowId2";
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId1", Convert.ToInt64(objApproval.sWFInitialId));
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId2", sWFlowId);
                        objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    UpdateWFOAutoObject(objApproval);

                    //Saving SMS in Table to send to Next Role
                    SendSMStoRole(objApproval, "");
                    objcon.CommitTransaction();
                    return true;
                }
                else
                {
                    objcon.BeginTransaction();

                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT COUNT(*) FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId5";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId5", Convert.ToInt32(objApproval.sBOId));
                    string count = objcon.get_value(strQry, NpgsqlCommand);

                    if (Convert.ToInt16(count) > 1 || objApproval.sBOId == "48")
                    {
                        if (objApproval.sPrevApproveId == "0" && objApproval.sRoleId == "")
                        {
                            objApproval.sWFDataId = "";
                            objApproval.sWFInitialId = "";
                        }
                    }

                    string sWFlowId = Convert.ToString(objcon.get_value(" SELECT MAX(\"WO_ID\")+1 FROM \"TBLWORKFLOWOBJECTS\""));

                    strQry = "INSERT INTO \"TBLWORKFLOWOBJECTS\" (\"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_NEXT_ROLE\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",";
                    strQry += " \"WO_CR_BY\",\"WO_APPROVE_STATUS\",\"WO_DESCRIPTION\",\"WO_WFO_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\",\"WO_STATUS\")";
                    strQry += " VALUES (:sWFlowId,:sBOId,:sRecordId,:sPrevApproveId,";
                    strQry += " '0',:sOfficeCode,:sClientIp,:sCrby,";
                    strQry += " '1',:sDescription,:sWFDataId,";

                    if (objApproval.sDataReferenceId != null)
                    {
                        strQry += ":sDataReferenceId,";
                    }
                    else
                    {
                        strQry += "'',";
                    }
                    if (objApproval.sRefOfficeCode != null)
                    {
                        strQry += ":sRefOfficeCode";
                    }
                    else
                    {
                        strQry += "NULL";
                    }
                    strQry += ",:status)";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFlowId", Convert.ToInt64(sWFlowId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sPrevApproveId", Convert.ToInt64(objApproval.sPrevApproveId));
                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objApproval.sOfficeCode));
                    NpgsqlCommand.Parameters.AddWithValue("sClientIp", objApproval.sClientIp);
                    NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("sDescription", objApproval.sDescription);

                    if (objApproval.sWFDataId == null || objApproval.sWFDataId == "")
                    {
                        objApproval.sWFDataId = "";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("sWFDataId", Convert.ToString(objApproval.sWFDataId));
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId == null ? "" : objApproval.sDataReferenceId);
                    NpgsqlCommand.Parameters.AddWithValue("sRefOfficeCode", Convert.ToInt32(objApproval.sRefOfficeCode));

                    if (objApproval.sStatus == null || objApproval.sStatus == "")
                    {
                        objApproval.sStatus = "";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("status", objApproval.sStatus);

                    objcon.ExecuteQry(strQry, NpgsqlCommand);

                    //Insert Initial Id for WorkFlow
                    if (objApproval.sWFInitialId == "" || objApproval.sWFInitialId == null)
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =:sWFlowId WHERE \"WO_ID\" =:sWFlowId1";
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId", Convert.ToInt64(sWFlowId));
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId1", Convert.ToInt64(sWFlowId));
                        objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =:sWFInitialId WHERE \"WO_ID\" =:sWFlowId";
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt64(objApproval.sWFInitialId));
                        NpgsqlCommand.Parameters.AddWithValue("sWFlowId", Convert.ToInt64(sWFlowId));
                        objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    if (objApproval.sBOId == "29")
                    {
                        objApproval.sWFDataId = "";
                    }
                    UpdateToMainTable(objApproval);
                    string sPrevBoId = objApproval.sBOId;
                    if (objApproval.sStoreType != null)
                    {
                        objApproval.sFailType = objApproval.sStoreType;
                    }
                    string sResult = GetNextBOId(objApproval.sBOId, objApproval.sFailType, objApproval.sGuarentyType);
                    objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    objApproval.sWFObjectId = sWFlowId;
                    int i = 0;
                    string[] sarray = sResult.Split('|');
                    if (objApproval.sTTKStatus == "1")
                    {
                        sarray[0] = "69~39";
                    }
                    if (sResult != "")
                    {
                        for (int x = 0; x < sarray.Length; x++)
                        {
                            sResult = sarray[x];
                            objApproval.sBOFlowMasterId = sResult.Split('~').GetValue(1).ToString();
                            objApproval.sBOId = sResult.Split('~').GetValue(0).ToString();

                            //To get Role Id with Priority Level to Create Form
                            objApproval.sRoleId = GetRoleFromApprovePriorityForBOCreate(objApproval);

                            UpdateWFOAutoObject(objApproval);
                            i++;

                            bool resrult = SaveWFObjectAuto(objApproval);
                            if (resrult == false)
                            {
                                return false;
                            }
                        }
                    }
                    if (i == 0)
                    {
                        UpdateWFOAutoObject(objApproval);
                    }

                    //Saving SMS in Table to send to Next Role
                    SendSMStoRole(objApproval, sPrevBoId);
                    objcon.CommitTransaction();
                    return true;
                }
            }

            catch (Exception ex)
            {
                objcon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
            finally
            {
                objcon.close();

            }
        }
        public bool SaveWorkflowObjects1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            string strQry = string.Empty;

            try
            {
                string sApproveResult = string.Empty;

                if (Convert.ToString(ConfigurationManager.AppSettings["Approval"]).ToUpper().Equals("OFF"))
                {
                    return false;
                }

                if (objApproval.sFormName != null && objApproval.sFormName != "")
                {
                    //To get Business Object Id
                    objApproval.sBOId = objDatabse.get_value("SELECT \"BO_ID\" FROM \"TBLBUSINESSOBJECT\" WHERE UPPER(\"BO_FORMNAME\")='" + objApproval.sFormName.Trim().ToUpper() + "'");
                }

                //fetching bfm_id from bo_id for updating intial id based on previosapproveid  n initialactionid                
                if (objApproval.sbfm_type != null && objApproval.sbfm_type != "")
                {
                    objApproval.sbfm_id = objDatabse.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' and \"BFM_TYPE\"='" + objApproval.sbfm_type + "'");

                }
                else if (objApproval.sTTKSComstatus == "1")
                {
                    objApproval.sbfm_id = objDatabse.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' and \"BFM_BO_ID\"=11 ");
                }
                else
                {
                    if (objApproval.sBOId != "" && objApproval.sBOId != null)

                    {
                        objApproval.sbfm_id = objDatabse.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "' ");
                    }
                    if (objApproval.sbfm_id == "24" && objApproval.sBOId == "65")
                    {
                        objApproval.sbfm_id = "23";
                    }
                }
                if (objApproval.sBOId == "62")
                {
                    objApproval.sbfm_id = "22";
                }
                if (objApproval.sBOId == "46")
                {
                    objApproval.sbfm_id = objDatabse.get_value("SELECT \"WOA_BFM_ID\" from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + objApproval.sWFObjectId + "' ");
                }

                //Check Approval Exists
                bool bResult = CheckFormApprovalExists1(objApproval, objDatabse);
                if (bResult == true)
                {
                    //To get Role Id with Priority Level                 
                    objApproval.sRoleId = GetRoleFromApprovePriority1(objApproval, objDatabse);
                }
                else
                {
                    objApproval.sRoleId = "";
                }

                if (objApproval.sPrevApproveId == null)
                {
                    objApproval.sPrevApproveId = "0";
                }

                if (objApproval.sRoleId != "" && objApproval.sRoleId != null)
                {
                    objApproval.sPrevWFOId = objApproval.sWFObjectId;

                    string sWFlowId = Convert.ToString(objDatabse.get_value("SELECT COALESCE (MAX(\"WO_ID\")+1,1) FROM \"TBLWORKFLOWOBJECTS\""));

                    objApproval.sWFObjectId = sWFlowId;

                    if (objApproval.sClientIp == null || objApproval.sClientIp == "")
                    {
                        objApproval.sClientIp = "";
                    }

                    if (objApproval.sDataReferenceId == null || objApproval.sDataReferenceId == "")
                    {
                        objApproval.sDataReferenceId = "";
                    }

                    if (objApproval.sRefOfficeCode == "")
                    {
                        objApproval.sRefOfficeCode = null;
                    }

                    if (objApproval.sStatus == null || objApproval.sStatus == "")
                    {
                        objApproval.sStatus = "";
                    }
                    strQry = "INSERT INTO \"TBLWORKFLOWOBJECTS\" (\"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_NEXT_ROLE\",\"WO_OFFICE_CODE\",";
                    strQry += " \"WO_CLIENT_IP\",\"WO_CR_BY\",\"WO_DESCRIPTION\",\"WO_WFO_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\",\"WO_STATUS\")";
                    strQry += " VALUES (" + Convert.ToInt64(sWFlowId) + "," + Convert.ToInt32(objApproval.sBOId) + "," + Convert.ToInt64(objApproval.sRecordId) + "," + Convert.ToInt64(objApproval.sPrevApproveId) + ",";
                    strQry += " " + Convert.ToInt32(objApproval.sRoleId) + "," + Convert.ToInt32(objApproval.sOfficeCode) + ",'" + objApproval.sClientIp + "'," + Convert.ToInt32(objApproval.sCrby) + ",";
                    strQry += " '" + objApproval.sDescription + "','" + objApproval.sWFDataId + "','" + objApproval.sDataReferenceId + "'," + Convert.ToInt32(objApproval.sRefOfficeCode) + ",'" + objApproval.sStatus + "')";
                    objDatabse.ExecuteQry(strQry);

                    if (objApproval.sWFInitialId == "" || objApproval.sWFInitialId == null)
                    {
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\"=" + Convert.ToInt64(sWFlowId) + " WHERE CAST(\"WO_ID\" AS TEXT) ='" + sWFlowId + "'";
                        objDatabse.ExecuteQry(strQry);
                    }
                    else
                    {
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =" + Convert.ToInt64(objApproval.sWFInitialId) + " WHERE CAST(\"WO_ID\" AS TEXT) ='" + sWFlowId + "'";
                        objDatabse.ExecuteQry(strQry);
                    }
                    UpdateWFOAutoObject1(objApproval, objDatabse);

                    //Saving SMS in Table to send to Next Role
                    SendSMStoRole1(objApproval, "", objDatabse);
                    return true;
                }
                else
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + "";
                    string count = objDatabse.get_value(strQry);

                    if (Convert.ToInt16(count) > 1 || objApproval.sBOId == "48")
                    {
                        if (objApproval.sPrevApproveId == "0" && objApproval.sRoleId == "")
                        {
                            objApproval.sWFDataId = "";
                            objApproval.sWFInitialId = "";
                        }
                    }
                    string sWFlowId = Convert.ToString(objDatabse.get_value(" SELECT MAX(\"WO_ID\")+1 FROM \"TBLWORKFLOWOBJECTS\""));

                    #region old code
                    //strQry = "INSERT INTO \"TBLWORKFLOWOBJECTS\" (\"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_NEXT_ROLE\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",";
                    //strQry += " \"WO_CR_BY\",\"WO_APPROVE_STATUS\",\"WO_DESCRIPTION\",\"WO_WFO_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\",\"WO_STATUS\")";
                    //strQry += " VALUES (:sWFlowId,:sBOId,:sRecordId,:sPrevApproveId,";
                    //strQry += " '0',:sOfficeCode,:sClientIp,:sCrby,";
                    //strQry += " '1',:sDescription,:sWFDataId,";


                    //if (objApproval.sDataReferenceId != null)
                    //{
                    //    strQry += ":sDataReferenceId,";
                    //}
                    //else
                    //{
                    //    strQry += "'',";
                    //}

                    //if (objApproval.sRefOfficeCode != null)
                    //{
                    //    strQry += ":sRefOfficeCode";
                    //}
                    //else
                    //{
                    //    strQry += "NULL";
                    //}

                    //strQry += ",:status)";

                    //NpgsqlCommand = new NpgsqlCommand();
                    //NpgsqlCommand.Parameters.AddWithValue("sWFlowId", Convert.ToInt32(sWFlowId));
                    //NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));

                    //NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                    //NpgsqlCommand.Parameters.AddWithValue("sPrevApproveId", Convert.ToInt64(objApproval.sPrevApproveId));
                    //// NpgsqlCommand.Parameters.AddWithValue("sRoleId3", objApproval.sRoleId);
                    //NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objApproval.sOfficeCode));
                    //NpgsqlCommand.Parameters.AddWithValue("sClientIp", objApproval.sClientIp);
                    //NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                    //NpgsqlCommand.Parameters.AddWithValue("sDescription", objApproval.sDescription);

                    //if (objApproval.sWFDataId == null || objApproval.sWFDataId == "")
                    //{
                    //    objApproval.sWFDataId = "";
                    //}
                    //NpgsqlCommand.Parameters.AddWithValue("sWFDataId", Convert.ToString(objApproval.sWFDataId));
                    //NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId == null ? "" : objApproval.sDataReferenceId);
                    //NpgsqlCommand.Parameters.AddWithValue("sRefOfficeCode", Convert.ToInt32(objApproval.sRefOfficeCode));

                    //if (objApproval.sStatus == null || objApproval.sStatus == "")
                    //{
                    //    objApproval.sStatus = "";
                    //}
                    //NpgsqlCommand.Parameters.AddWithValue("status", objApproval.sStatus);
                    #endregion

                    if (objApproval.sWFDataId == null || objApproval.sWFDataId == "")
                    {
                        objApproval.sWFDataId = "";
                    }

                    if (objApproval.sBOId == null || objApproval.sBOId == "")
                    {
                        return false;
                    }
                    if (objApproval.sOfficeCode == "")
                    {
                        return false;
                    }
                    strQry = "INSERT INTO \"TBLWORKFLOWOBJECTS\" (\"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_NEXT_ROLE\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",";
                    strQry += " \"WO_CR_BY\",\"WO_APPROVE_STATUS\",\"WO_DESCRIPTION\",\"WO_WFO_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\",\"WO_STATUS\")";
                    strQry += " VALUES (" + Convert.ToInt64(sWFlowId) + "," + Convert.ToInt32(objApproval.sBOId) + "," + Convert.ToInt64(objApproval.sRecordId) + "," + Convert.ToInt64(objApproval.sPrevApproveId) + ",";
                    strQry += " '0'," + Convert.ToInt32(objApproval.sOfficeCode) + ",'" + objApproval.sClientIp + "'," + Convert.ToInt32(objApproval.sCrby) + ",";
                    strQry += " '1','" + objApproval.sDescription + "','" + Convert.ToString(objApproval.sWFDataId) + "',";

                    if (objApproval.sDataReferenceId != null)
                    {
                        strQry += "'" + objApproval.sDataReferenceId + "',";
                    }
                    else
                    {
                        strQry += "'',";
                    }
                    if (objApproval.sRefOfficeCode != null)
                    {
                        strQry += Convert.ToInt32(objApproval.sRefOfficeCode);
                    }
                    else
                    {
                        strQry += "NULL";
                    }
                    if (objApproval.sStatus == null || objApproval.sStatus == "")
                    {
                        objApproval.sStatus = "";
                    }
                    strQry += ",'" + objApproval.sStatus + "')";

                    objDatabse.ExecuteQry(strQry);

                    //Insert Initial Id for WorkFlow
                    if (objApproval.sWFInitialId == "" || objApproval.sWFInitialId == null)
                    {
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =" + Convert.ToInt64(sWFlowId) + " WHERE \"WO_ID\" =" + Convert.ToInt64(sWFlowId) + "";
                        objDatabse.ExecuteQry(strQry);
                    }
                    else
                    {
                        strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_INITIAL_ID\" =" + Convert.ToInt64(objApproval.sWFInitialId) + " WHERE \"WO_ID\" =" + Convert.ToInt64(sWFlowId) + "";
                        objDatabse.ExecuteQry(strQry);
                    }
                    if (objApproval.sBOId == "29")
                    {
                        objApproval.sWFDataId = "";
                    }
                    UpdateToMainTable1(objApproval, objDatabse);

                    string sPrevBoId = objApproval.sBOId;

                    if (objApproval.sStoreType != null)
                    {
                        objApproval.sFailType = objApproval.sStoreType;
                    }
                    objApproval.PrevBoId = sPrevBoId;
                    string Result = GetNextBOId1(objApproval.sBOId, objApproval.sFailType, objApproval.sGuarentyType, objApproval.sdtccode, objDatabse);
                    objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    objApproval.sWFObjectId = sWFlowId;

                    int i = 0;
                    string[] sarray = Result.Split('|');

                    if (objApproval.sTTKStatus == "1")
                    {
                        for (int x = 0; x < sarray.Length; x++)
                        {
                            sarray[x] = null;
                        }
                        sarray[0] = "69~39";
                    }

                    if (objApproval.sBOId == "11" && objApproval.failureId == "" && objApproval.sTTKStatus != "1")
                    {
                        for (int x = 0; x < sarray.Length; x++)
                        {
                            sarray[x] = null;
                        }
                        sarray[0] = "12~4";
                    }

                    if (Result != "")
                    {
                        for (int x = 0; x < sarray.Length; x++)
                        {
                            if (sarray[x] != null)
                            {
                                Result = sarray[x];
                                objApproval.sBOFlowMasterId = Result.Split('~').GetValue(1).ToString();
                                objApproval.sBOId = Result.Split('~').GetValue(0).ToString();
                                Result = sarray[x];
                                objApproval.sBOFlowMasterId = Result.Split('~').GetValue(1).ToString();
                                objApproval.sBOId = Result.Split('~').GetValue(0).ToString();
                                Result = sarray[x];
                                objApproval.sBOFlowMasterId = Result.Split('~').GetValue(1).ToString();
                                objApproval.sBOId = Result.Split('~').GetValue(0).ToString();

                                //To get Role Id with Priority Level to Create Form
                                objApproval.sRoleId = GetRoleFromApprovePriorityForBOCreate1(objApproval, objDatabse);
                                UpdateWFOAutoObject1(objApproval, objDatabse);
                                i++;

                                bool resrult = SaveWFObjectAuto1(objApproval, objDatabse);
                                if (resrult == false)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    if (i == 0)
                    {
                        UpdateWFOAutoObject1(objApproval, objDatabse);
                    }
                    //Saving SMS in Table to send to Next Role
                    SendSMStoRole1(objApproval, sPrevBoId, objDatabse);
                    return true;
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               // return false;
                throw ex;
            }
        }

        /// <summary>
        /// Get Approval Priority Role From TBLWORKFLOWMASTER Table 
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public string GetRoleFromApprovePriority(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                string sExistWFObject = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" =:sBOId AND \"WO_RECORD_ID\"=:sRecordId";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                sExistWFObject = objcon.get_value(strQry, NpgsqlCommand);

                if (sExistWFObject == "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId AND \"WM_LEVEL\" = (";
                    strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId1 AND \"WM_LEVEL\" <> '1')";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                }
                else if (objApproval.sApproveStatus == "3")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId AND \"WM_LEVEL\" = (";
                    strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId1 AND \"WM_LEVEL\" = '1')";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId AND \"WM_LEVEL\" = (";
                    strQry += " SELECT \"WM_LEVEL\"+1 FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId1 AND \"WM_ROLEID\" = ";
                    strQry += " (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = ";
                    strQry += " (SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" =:sBOId2 AND \"WO_RECORD_ID\"=:sRecordId)))";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId2", Convert.ToInt32(objApproval.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                }
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetRoleFromApprovePriority1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                string sExistWFObject = string.Empty;

                strQry = "SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WO_RECORD_ID\"=" + Convert.ToInt64(objApproval.sRecordId) + "";
                sExistWFObject = objDatabse.get_value(strQry);

                if (sExistWFObject == "")
                {
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = (";
                    strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" <> '1')";
                }
                else if (objApproval.sApproveStatus == "3")
                {
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = (";
                    strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = '1')";
                }
                else
                {
                    strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = (";
                    strQry += " SELECT \"WM_LEVEL\"+1 FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_ROLEID\" = ";
                    strQry += " (SELECT \"WO_NEXT_ROLE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = ";
                    strQry += " (SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WO_RECORD_ID\"=" + Convert.ToInt64(objApproval.sRecordId) + ")))";
                }
                return objDatabse.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// Get Role Id of Form Creator from Bussiness Object Id
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public string GetRoleFromApprovePriorityForBOCreate(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\"=:sBOId AND \"WM_LEVEL\" = (";
                strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId1 AND \"WM_LEVEL\" = '1')";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(objApproval.sBOId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        public string GetRoleFromApprovePriorityForBOCreate1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WM_ROLEID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\"=" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = (";
                strQry += " SELECT MIN(\"WM_LEVEL\") FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WM_LEVEL\" = '1')";
                return objDatabse.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// Check Approval Exists for Given Form/Bussiness Object
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool CheckFormApprovalExists(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WM_BOID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =:sBOId";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                string sId = objcon.get_value(strQry, NpgsqlCommand);
                if (sId.Length > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool CheckFormApprovalExists1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                if (objApproval.sBOId != "" && objApproval.sBOId != null)
                {
                    strQry = "SELECT \"WM_BOID\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_BOID\" =" + Convert.ToInt32(objApproval.sBOId) + "";
                }
                string sId = objDatabse.get_value(strQry);
                if (sId.Length > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Approve WorkFlow Object
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool ApproveWFRequest(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                string sNextRole = string.Empty;
                string sApproveResult = string.Empty;

                if (objApproval.sWFAutoId != null)
                {
                    if (objApproval.sWFAutoId == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId AND \"WO_APPROVE_STATUS\" <> 0";
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sApproveResult = objcon.get_value(strQry, NpgsqlCommand);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WOA_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_ID\" =:sWFAutoId AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL";
                        NpgsqlCommand.Parameters.AddWithValue("sWFAutoId", Convert.ToInt64(objApproval.sWFAutoId));
                        sApproveResult = objcon.get_value(strQry, NpgsqlCommand);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                }
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_USER_COMMENT\" =:sApproveComments, \"WO_APPROVE_STATUS\" =:sApproveStatus,";
                strQry += " \"WO_APPROVED_BY\" =:sCrby WHERE \"WO_ID\" =:sWFObjectId";
                NpgsqlCommand.Parameters.AddWithValue("sApproveComments", objApproval.sApproveComments.Replace("'", "''"));
                NpgsqlCommand.Parameters.AddWithValue("sApproveStatus", Convert.ToInt16(objApproval.sApproveStatus));
                NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                objcon.ExecuteQry(strQry, NpgsqlCommand);

                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",\"WO_NEXT_ROLE\",\"WO_DESCRIPTION\",";
                strQry += " \"WO_WFO_ID\",\"WO_INITIAL_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" ";
                strQry += " WHERE \"WO_ID\" =:sWFObjectId";
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objApproval.sBOId = Convert.ToString(dt.Rows[0]["WO_BO_ID"]);

                    if (objApproval.sRecordId == "" || objApproval.sRecordId == null)
                    {
                        objApproval.sRecordId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    }
                    if (objApproval.sWFDataId == "" || objApproval.sWFDataId == null)
                    {
                        objApproval.sWFDataId = Convert.ToString(dt.Rows[0]["WO_WFO_ID"]);
                    }
                    objApproval.sWFInitialId = Convert.ToString(dt.Rows[0]["WO_INITIAL_ID"]);
                    objApproval.sDataReferenceId = Convert.ToString(dt.Rows[0]["WO_DATA_ID"]);

                    if (objApproval.sRefOfficeCode == "" || objApproval.sRefOfficeCode == null)
                    {
                        objApproval.sRefOfficeCode = Convert.ToString(dt.Rows[0]["WO_REF_OFFCODE"]);
                    }

                    if (objApproval.sDescription == "" || objApproval.sDescription == null)
                    {
                        objApproval.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                    }

                    sNextRole = Convert.ToString(dt.Rows[0]["WO_NEXT_ROLE"]);
                    if (sNextRole == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"BFM_NEXT_BO_ID\"  FROM \"TBLWO_OBJECT_AUTO\",\"TBLBO_FLOW_MASTER\" WHERE \"WOA_BFM_ID\"=\"BFM_ID\" AND CAST(\"WOA_PREV_APPROVE_ID\" AS TEXT) =:sWFObjectId";
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", objApproval.sWFObjectId);
                        string sResult = objcon.get_value(strQry, NpgsqlCommand);
                        objApproval.sBOId = sResult;
                        objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    }
                    else
                    {
                        objApproval.sPrevApproveId = Convert.ToString(dt.Rows[0]["WO_ID"]);
                    }
                }
                SaveWorkflowObjects(objApproval);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ApproveWFRequest1(clsApproval objApproval)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);
            try
            {
                objDatabse.BeginTransaction();
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                string sNextRole = string.Empty;
                string sApproveResult = string.Empty;

                if (objApproval.sWFAutoId != null)
                {
                    if (objApproval.sWFAutoId == "0")
                    {
                        strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_APPROVE_STATUS\" <> 0";
                        sApproveResult = objDatabse.get_value(strQry);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (objApproval.sWFAutoId != "")
                        {
                            strQry = "SELECT \"WOA_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_ID\" =" + Convert.ToInt64(objApproval.sWFAutoId) + " AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL";
                            sApproveResult = objDatabse.get_value(strQry);
                        }
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                }
                if (objApproval.sWFObjectId != "")
                {
                    strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_USER_COMMENT\" ='" + objApproval.sApproveComments.Replace("'", "''") + "', \"WO_APPROVE_STATUS\" =" + Convert.ToInt16(objApproval.sApproveStatus) + ",";
                    strQry += " \"WO_APPROVED_BY\" =" + Convert.ToInt32(objApproval.sCrby) + " WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                    objDatabse.ExecuteQry(strQry);

                    strQry = "SELECT \"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",\"WO_NEXT_ROLE\",\"WO_DESCRIPTION\",";
                    strQry += " \"WO_WFO_ID\",\"WO_INITIAL_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" ";
                    strQry += " WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                    dt = objDatabse.FetchDataTable(strQry);
                }
                if (dt.Rows.Count > 0)
                {
                    objApproval.sBOId = Convert.ToString(dt.Rows[0]["WO_BO_ID"]);

                    if (objApproval.sRecordId == "" || objApproval.sRecordId == null)
                    {
                        objApproval.sRecordId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    }
                    if (objApproval.sWFDataId == "" || objApproval.sWFDataId == null)
                    {
                        objApproval.sWFDataId = Convert.ToString(dt.Rows[0]["WO_WFO_ID"]);
                    }

                    objApproval.sWFInitialId = Convert.ToString(dt.Rows[0]["WO_INITIAL_ID"]);
                    objApproval.sDataReferenceId = Convert.ToString(dt.Rows[0]["WO_DATA_ID"]);

                    if (objApproval.sRefOfficeCode == "" || objApproval.sRefOfficeCode == null)
                    {
                        objApproval.sRefOfficeCode = Convert.ToString(dt.Rows[0]["WO_REF_OFFCODE"]);
                    }

                    if (objApproval.sDescription == "" || objApproval.sDescription == null)
                    {
                        objApproval.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                    }
                    sNextRole = Convert.ToString(dt.Rows[0]["WO_NEXT_ROLE"]);
                    if (sNextRole == "0")
                    {
                        strQry = "SELECT \"BFM_NEXT_BO_ID\"  FROM \"TBLWO_OBJECT_AUTO\",\"TBLBO_FLOW_MASTER\" WHERE \"WOA_BFM_ID\"=\"BFM_ID\" AND CAST(\"WOA_PREV_APPROVE_ID\" AS TEXT) ='" + objApproval.sWFObjectId + "'";
                        string sResult = objDatabse.get_value(strQry);
                        objApproval.sBOId = sResult;
                        objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    }
                    else
                    {
                        objApproval.sPrevApproveId = Convert.ToString(dt.Rows[0]["WO_ID"]);
                    }
                }
                bool status = SaveWorkflowObjects1(objApproval, objDatabse);
                if (status == false)
                {
                    return false;
                }
                objDatabse.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Modify and Approve Workflow Object
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool ModifyApproveWFRequest(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                string sNextRole = string.Empty;
                string sApproveResult = string.Empty;
                if (objApproval.sBOId == "73")
                {
                    sApproveResult = "";
                    objApproval.sFormName = "EstimationCreation_sdo";
                }
                else if (objApproval.sBOId == "74")
                {
                    sApproveResult = "";
                    objApproval.sFormName = "WorkOrder_sdo";
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId AND \"WO_APPROVE_STATUS\" <>0";
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sApproveResult = objcon.get_value(strQry, NpgsqlCommand);
                }
                if (sApproveResult != "")
                {
                    return false;
                }
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_USER_COMMENT\" =:sApproveComments, \"WO_APPROVE_STATUS\" =:sApproveStatus,";
                strQry += " \"WO_APPROVED_BY\" =:sCrby WHERE \"WO_ID\" =:sWFObjectId";
                NpgsqlCommand.Parameters.AddWithValue("sApproveComments", objApproval.sApproveComments.Replace("'", "''"));
                NpgsqlCommand.Parameters.AddWithValue("sApproveStatus", Convert.ToInt16(objApproval.sApproveStatus));
                NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                objcon.ExecuteQry(strQry, NpgsqlCommand);

                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",\"WO_NEXT_ROLE\",\"WO_DESCRIPTION\",";
                strQry += " \"WO_WFO_ID\",\"WO_INITIAL_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" ";
                strQry += " WHERE \"WO_ID\" =:sWFObjectId";
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objApproval.sBOId = Convert.ToString(dt.Rows[0]["WO_BO_ID"]);
                    objApproval.sRecordId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    objApproval.sWFInitialId = Convert.ToString(dt.Rows[0]["WO_INITIAL_ID"]);
                    objApproval.sDataReferenceId = Convert.ToString(dt.Rows[0]["WO_DATA_ID"]);

                    if (objApproval.sRefOfficeCode == "" || objApproval.sRefOfficeCode == null)
                    {
                        objApproval.sRefOfficeCode = Convert.ToString(dt.Rows[0]["WO_REF_OFFCODE"]);
                    }

                    if (objApproval.sDescription == "" || objApproval.sDescription == null)
                    {
                        objApproval.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                    }

                    sNextRole = Convert.ToString(dt.Rows[0]["WO_NEXT_ROLE"]);
                    if (sNextRole == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"BFM_NEXT_BO_ID\"  FROM \"TBLWO_OBJECT_AUTO\",\"TBLBO_FLOW_MASTER\" WHERE \"WOA_BFM_ID\"=\"BFM_ID\" AND \"WOA_PREV_APPROVE_ID\" =:sWFObjectId";
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        string sResult = objcon.get_value(strQry, NpgsqlCommand);
                        objApproval.sBOId = sResult;
                        objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    }
                    else
                    {
                        objApproval.sPrevApproveId = Convert.ToString(dt.Rows[0]["WO_ID"]);
                    }
                }

                objApproval.sbfm_id = objcon.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "'");
                objApproval.sFormName = objApproval.sFormName;
                SaveWorkflowObjects(objApproval);
                UpdateWFOAutoObject(objApproval);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool ModifyApproveWFRequest1(clsApproval objApproval)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);
            try
            {
                objDatabse.BeginTransaction();
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                string sNextRole = string.Empty;
                string sApproveResult = string.Empty;
                if (objApproval.sBOId == "73")
                {
                    sApproveResult = "";
                    objApproval.sFormName = "EstimationCreation_sdo";
                }
                else if (objApproval.sBOId == "74")
                {
                    sApproveResult = "";
                    objApproval.sFormName = "WorkOrder_sdo";
                }
                else
                {
                    strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_APPROVE_STATUS\" <>0";
                    sApproveResult = objDatabse.get_value(strQry);
                }
                if (sApproveResult != "")
                {
                    return false;
                }
                strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_USER_COMMENT\" ='" + objApproval.sApproveComments.Replace("'", "''") + "', \"WO_APPROVE_STATUS\" =" + Convert.ToInt16(objApproval.sApproveStatus) + ",";
                strQry += " \"WO_APPROVED_BY\" =" + Convert.ToInt32(objApproval.sCrby) + " WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                objDatabse.ExecuteQry(strQry);

                strQry = "SELECT \"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_PREV_APPROVE_ID\",\"WO_OFFICE_CODE\",\"WO_CLIENT_IP\",\"WO_NEXT_ROLE\",\"WO_DESCRIPTION\",";
                strQry += " \"WO_WFO_ID\",\"WO_INITIAL_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" ";
                strQry += " WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                dt = objDatabse.FetchDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    objApproval.sBOId = Convert.ToString(dt.Rows[0]["WO_BO_ID"]);
                    objApproval.sRecordId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    objApproval.sWFInitialId = Convert.ToString(dt.Rows[0]["WO_INITIAL_ID"]);
                    objApproval.sDataReferenceId = Convert.ToString(dt.Rows[0]["WO_DATA_ID"]);
                    if (objApproval.sRefOfficeCode == "" || objApproval.sRefOfficeCode == null)
                    {
                        objApproval.sRefOfficeCode = Convert.ToString(dt.Rows[0]["WO_REF_OFFCODE"]);
                    }
                    if (objApproval.sDescription == "" || objApproval.sDescription == null)
                    {
                        objApproval.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                    }

                    sNextRole = Convert.ToString(dt.Rows[0]["WO_NEXT_ROLE"]);
                    if (sNextRole == "0")
                    {
                        strQry = "SELECT \"BFM_NEXT_BO_ID\"  FROM \"TBLWO_OBJECT_AUTO\",\"TBLBO_FLOW_MASTER\" WHERE \"WOA_BFM_ID\"=\"BFM_ID\" AND \"WOA_PREV_APPROVE_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                        string sResult = objDatabse.get_value(strQry);
                        objApproval.sBOId = sResult;
                        objApproval.sPrevWFOId = objApproval.sWFObjectId;
                    }
                    else
                    {
                        objApproval.sPrevApproveId = Convert.ToString(dt.Rows[0]["WO_ID"]);
                    }
                }
                objApproval.sbfm_id = objDatabse.get_value("SELECT \"BFM_ID\" from \"TBLBO_FLOW_MASTER\" WHERE \"BFM_NEXT_BO_ID\"='" + objApproval.sBOId + "'");
                objApproval.sFormName = objApproval.sFormName;
                SaveWorkflowObjects1(objApproval, objDatabse);
                UpdateWFOAutoObject1(objApproval, objDatabse);
                objDatabse.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        //#region Approval Column Update Concept

        public void UpdateApproveStatusinMainTable(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"BO_MAIN_TABLE\",\"BO_REF_COLUMN\",\"BO_REF_APPROVE\" FROM \"TBLBUSINESSOBJECT\" WHERE \"BO_ID\" =:sBOId";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objApproval.sMainTable = Convert.ToString(dt.Rows[0]["BO_MAIN_TABLE"]);
                    objApproval.sRefColumnName = Convert.ToString(dt.Rows[0]["BO_REF_COLUMN"]);
                    objApproval.sApproveColumnName = Convert.ToString(dt.Rows[0]["BO_REF_APPROVE"]);
                }
                if (objApproval.sMainTable != "")
                {
                    strQry = " UPDATE " + objApproval.sMainTable + "  SET   " + sApproveColumnName + "='1' WHERE ";
                    strQry += " " + objApproval.sRefColumnName + "='" + objApproval.sRecordId + "'";
                    objcon.ExecuteQry(strQry);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        //#endregion

        /// <summary>
        /// Update to Main Table by Fetching Queries from TBLWFODATA Table
        /// </summary>
        /// <param name="objApproval"></param>
        public void UpdateToMainTable(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                if (objApproval.sWFDataId != null)
                {
                    if (objApproval.sWFDataId.Length > 0)
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WFO_QUERY_VALUES\",\"WFO_PARAMETER\" FROM \"TBLWFODATA\" WHERE \"WFO_ID\" =:sWFDataId ";
                        NpgsqlCommand.Parameters.AddWithValue("sWFDataId", Convert.ToInt32(objApproval.sWFDataId));
                        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objApproval.sColumnNames = Convert.ToString(dt.Rows[0]["WFO_QUERY_VALUES"]);
                            objApproval.sParameterValues = Convert.ToString(dt.Rows[0]["WFO_PARAMETER"]);

                            if (objApproval.sParameterValues != "")
                            {
                                string[] sParameterQueries = objApproval.sParameterValues.Split(';');
                                string sSecondRecordId = string.Empty;

                                for (int i = 0; i < sParameterQueries.Length; i++)
                                {
                                    if (objApproval.sNewRecordId == null || objApproval.sNewRecordId == "")
                                    {
                                        if (sParameterQueries[i].ToString() != "")
                                        {
                                            objApproval.sNewRecordId = objcon.get_value(sParameterQueries[i]);
                                            objApproval.sColumnNames = objApproval.sColumnNames.Replace("{" + i + "}", objApproval.sNewRecordId);
                                        }
                                    }
                                    else
                                    {
                                        sSecondRecordId = objcon.get_value(sParameterQueries[i]);
                                        objApproval.sColumnNames = objApproval.sColumnNames.Replace("{" + i + "}", sSecondRecordId);
                                    }
                                }
                            }
                            string[] sQueries = objApproval.sColumnNames.Split(';');
                            for (int i = 0; i < sQueries.Length; i++)
                            {
                                if (sQueries[i].ToString() != "")
                                {
                                    objcon.ExecuteQry(sQueries[i]);
                                }
                            }
                        }
                    }
                }
                // && objApproval.sBOId != "45" dont know adding
                if (objApproval.sNewRecordId != null && objApproval.sNewRecordId != "")
                {
                    UpdateWorkFlowRecordId(objApproval);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void UpdateToMainTable1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                if (objApproval.sWFDataId != null)
                {
                    if (objApproval.sWFDataId.Length > 0)
                    {
                        strQry = "SELECT \"WFO_QUERY_VALUES\",\"WFO_PARAMETER\" FROM \"TBLWFODATA\" WHERE \"WFO_ID\" =" + Convert.ToInt64(objApproval.sWFDataId) + " ";
                        dt = objDatabse.FetchDataTable(strQry);
                        if (dt.Rows.Count > 0)
                        {
                            objApproval.sColumnNames = Convert.ToString(dt.Rows[0]["WFO_QUERY_VALUES"]);
                            objApproval.sParameterValues = Convert.ToString(dt.Rows[0]["WFO_PARAMETER"]);
                            if (objApproval.sParameterValues != "")
                            {
                                string[] sParameterQueries = objApproval.sParameterValues.Split(';');
                                string sSecondRecordId = string.Empty;
                                for (int i = 0; i < sParameterQueries.Length; i++)
                                {
                                    if (objApproval.sNewRecordId == null || objApproval.sNewRecordId == "")
                                    {
                                        if (sParameterQueries[i].ToString() != "")
                                        {
                                            objApproval.sNewRecordId = objDatabse.get_value(sParameterQueries[i]);
                                            objApproval.sColumnNames = objApproval.sColumnNames.Replace("{" + i + "}", objApproval.sNewRecordId);
                                        }
                                    }
                                    else
                                    {
                                        sSecondRecordId = objDatabse.get_value(sParameterQueries[i]);
                                        objApproval.sColumnNames = objApproval.sColumnNames.Replace("{" + i + "}", sSecondRecordId);
                                    }
                                }
                            }
                            string[] sQueries = objApproval.sColumnNames.Split(';');
                            for (int i = 0; i < sQueries.Length; i++)
                            {
                                if (sQueries[i].ToString() != "")
                                {
                                    objDatabse.ExecuteQry(sQueries[i]);
                                }
                            }
                        }
                    }
                }
                if (objApproval.sNewRecordId != null && objApproval.sNewRecordId != "")
                {
                    UpdateWorkFlowRecordId1(objApproval, objDatabse);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool IsNumeric(string input)
        {
            long test;
            return long.TryParse(input, out test);
        }


        /// <summary>
        /// Get Form Name Using Bussiness Object Id
        /// </summary>
        /// <param name="sBOId"></param>
        /// <returns></returns>
        public string GetFormName(string sBOId)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"BO_FORMNAME\" FROM \"TBLBUSINESSOBJECT\" WHERE \"BO_ID\" =:sBOId";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// Update Genrated/Actual Record Id to TBLWORKFLOWOBJECTS Table
        /// </summary>
        /// <param name="objApproval"></param>
        public void UpdateWorkFlowRecordId(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                if (objApproval.sNewRecordId == null || objApproval.sNewRecordId == "")
                {
                    return;
                }
                DataTable dt = objcon.FetchDataTable(" SELECT \"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\"   WHERE  \"WO_BO_ID\" ='" + Convert.ToInt32(objApproval.sBOId) + "' AND \"WO_RECORD_ID\" ='" + Convert.ToInt64(objApproval.sRecordId) + "'");

                NpgsqlCommand = new NpgsqlCommand();
                strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_RECORD_ID\" =:sNewRecordId WHERE ";
                strQry += " \"WO_BO_ID\" =:sBOId AND \"WO_RECORD_ID\" =:sRecordId";
                NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objApproval.sBOId));
                NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                objcon.ExecuteQry(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void UpdateWorkFlowRecordId1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                if (objApproval.sNewRecordId == null || objApproval.sNewRecordId == "")
                {
                    return;
                }
                DataTable dt = objcon.FetchDataTable(" SELECT \"WO_ID\",\"WO_BO_ID\",\"WO_RECORD_ID\",\"WO_DATA_ID\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\"   WHERE  \"WO_BO_ID\" ='" + Convert.ToInt32(objApproval.sBOId) + "' AND \"WO_RECORD_ID\" ='" + Convert.ToInt64(objApproval.sRecordId) + "'");
                strQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_RECORD_ID\" = " + Convert.ToInt64(objApproval.sNewRecordId) + " WHERE ";
                strQry += " \"WO_BO_ID\" =" + Convert.ToInt32(objApproval.sBOId) + " AND \"WO_RECORD_ID\" =" + Convert.ToInt64(objApproval.sRecordId) + "";
                objDatabse.ExecuteQry(strQry);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        //#region XML Concepts

        public string DatatableToXML(DataTable dtforXML, string sTableName)
        {
            string xmlstr = string.Empty;
            DataTable dt = new DataTable(sTableName);
            try
            {
                dt = dtforXML;
                if (dt != null && dt.Rows.Count == 0)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dc.DataType = typeof(String);
                    }
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < dr.ItemArray.Count(); i++)
                    {
                        dr[i] = string.Empty;
                    }
                    dt.Rows.Add(dr);
                }
                DataSet ds = new DataSet(sTableName);
                ds.Tables.Add(dt);
                return ds.GetXml();
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return xmlstr;
            }
        }


        public void CreateXMLFile(clsApproval objApproval, DataTable dt)
        {
            try
            {
                string sDirectory = System.Web.HttpContext.Current.Server.MapPath("XMLData");
                string sSubDirectory = System.Web.HttpContext.Current.Server.MapPath("XMLData" + "/" + objApproval.sCrby);
                string sPath = System.Web.HttpContext.Current.Server.MapPath("XMLData" + "/" + objApproval.sCrby + "/" + objApproval.sMainTable);
                if (!Directory.Exists(sDirectory))
                {
                    Directory.CreateDirectory(sDirectory);
                }
                if (!Directory.Exists(sSubDirectory))
                {
                    Directory.CreateDirectory(sSubDirectory);
                }
                if (File.Exists(sPath))
                {
                    File.Delete(sPath);
                }
                dt.WriteXml(sPath);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ReadXMLFile(clsApproval objApproval)
        {
            try
            {
                XmlReader objXmRdr;
                string sPath = System.Web.HttpContext.Current.Server.MapPath("XMLData" + "/" + objApproval.sCrby + "/" + objApproval.sMainTable);
                if (File.Exists(sPath))
                {
                    objXmRdr = XmlReader.Create(sPath);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Get Next Bussiness Object Id from TBLBO_FLOW_MASTER Table
        /// </summary>
        /// <param name="sBOId"></param>
        /// <returns></returns>
        public string GetNextBOId(string sBOId, string type, string sGuarentyType)
        {
            try
            {
                if (type == null)
                {
                    type = "0";
                }
                else if (type == "")
                {
                    type = "2";
                }
                else if (type == "1")
                {
                    if (sGuarentyType == "WGP" || sGuarentyType == "WRGP")
                    {
                        type = "2";
                    }
                    else
                    {
                        type = "1";
                    }
                }
                if (sBOId == "10")
                {
                    type = "1";
                }
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT string_agg(\"BFM_NEXT_BO_ID\" || '~' || \"BFM_ID\" , '|')   FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_BO_ID\" =:sBOId AND \"BFM_TYPE\"=:type";
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                NpgsqlCommand.Parameters.AddWithValue("type", Convert.ToInt16(type));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetNextBOId1(string sBOId, string type, string sGuarentyType, string sDtccode, DataBseConnection objDatabse)
        {
            try
            {
                if (type == null)
                {
                    type = "0";
                }
                else if (type == "")
                {
                    type = "2";
                }
                else if (type == "1")
                {
                    if (sGuarentyType == "WGP" || sGuarentyType == "WRGP")
                    {
                        type = "2";
                    }
                    else
                    {
                        type = "1";
                    }
                }
                if (sBOId == "10")
                {
                    type = "1";
                }
                string strQry = string.Empty;
                string sDFID = objcon.get_value("SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" ='" + sDtccode + "'");
                if (sDFID != "")
                {
                    strQry = "SELECT string_agg(\"BFM_NEXT_BO_ID\" || '~' || \"BFM_ID\" , '|')   FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_BO_ID\" =" + Convert.ToInt32(sBOId) + " AND \"BFM_TYPE\"=" + Convert.ToInt16(type) + " AND  \"BFM_NEXT_BO_ID\"<>69";
                }
                else
                {
                    strQry = "SELECT string_agg(\"BFM_NEXT_BO_ID\" || '~' || \"BFM_ID\" , '|')   FROM \"TBLBO_FLOW_MASTER\" WHERE \"BFM_BO_ID\" =" + Convert.ToInt32(sBOId) + " AND \"BFM_TYPE\"=" + Convert.ToInt16(type) + "";

                }
                return objDatabse.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// Save Initiation of Next Form for assigned Role
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool SaveWFObjectAuto(clsApproval objApproval)
        {
            try
            {
                Division = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
                SubDivision = Convert.ToInt32(ConfigurationManager.AppSettings["SubDiv_code"]);
                Section = Convert.ToInt32(ConfigurationManager.AppSettings["Section_code"]);

                string strQry = string.Empty;

                string sMaxNo = Convert.ToString(objcon.Get_max_no("WOA_ID", "TBLWO_OBJECT_AUTO"));
                if (objApproval.sBOId == "25" || objApproval.sBOId == "11")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT) and \"EST_FAILUREID\"=\"DF_ID\"", NpgsqlCommand);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sftype = objcon.get_value("SELECT \"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT) and \"EST_FAILUREID\"=\"DF_ID\"", NpgsqlCommand);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =:sWFObjectId AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT)", NpgsqlCommand);
                    if (sftype == "2" || sftype == "")
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = " Enhacement of  Work Order For DTC Code " + sRecordId;
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement of Work Order For DTC Code " + sRecordId;
                        }
                        else
                        {
                            objApproval.sDescription = " Repair and replacement of  Major faulty Work Order For DTC Code " + sRecordId;
                        }
                    }
                    else
                    {
                        objApproval.sDescription = " Repair and replacement of  Minor faulty Work Order For DTC Code " + sRecordId;
                    }

                }
                else if (objApproval.sBOId == "12")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\" and \"EST_FAILUREID\"=\"DF_ID\"", NpgsqlCommand);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sftype = objcon.get_value("SELECT \"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\" and \"EST_FAILUREID\"=\"DF_ID\"", NpgsqlCommand);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\"", NpgsqlCommand);
                    if (sRecordId == "")
                    {
                        if (objApproval.sTTKStatus == "1")
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                            sRecordId = objcon.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\"", NpgsqlCommand);
                            objApproval.sDescription = "Indent For New DTC Commission TTK Flow WO No " + sRecordId;
                        }
                        else
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                            sRecordId = objcon.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\"", NpgsqlCommand);
                            objApproval.sDescription = "Indent For New DTC Commission PTK with WO No " + sRecordId;
                        }
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();

                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement of  Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();
                        }

                        else
                        {
                            objApproval.sDescription = "Repair & replacement of Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "13" || objApproval.sBOId == "56" || objApproval.sBOId == "57")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\" = \"WO_SLNO\" AND \"DT_CODE\" = \"DF_DTC_CODE\"", NpgsqlCommand);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" || '~' || \"DT_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\" = \"WO_SLNO\" AND \"DT_CODE\" = \"DF_DTC_CODE\"", NpgsqlCommand);
                    if (sRecordId == "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sRecordId = objcon.get_value("SELECT \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\"", NpgsqlCommand);
                        objApproval.sDescription = "Invoice For New DTC Commission with Indent No " + sRecordId.Split('~').GetValue(0).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of  Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement  of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                        else
                        {
                            objApproval.sDescription = "Repair & replacement of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "14")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sRecordId = objApproval.sdtccode;
                    objApproval.sDescription = "Decommissioning For DTC Code " + sRecordId;
                    if (sRecordId == "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sRecordId = objcon.get_value("SELECT \"IN_INV_NO\" FROM \"TBLDTCINVOICE\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId AND \"WO_RECORD_ID\"=\"IN_NO\" ", NpgsqlCommand);
                        objApproval.sDescription = "Commissioning of DTC for the Invoice NO " + sRecordId;
                    }
                }
                else if (objApproval.sBOId == "15")
                {

                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" = '" + objApproval.sfailid + "'");
                    string sRecordId = objcon.get_value("SELECT \"DF_EQUIPMENT_ID\" || '~' || \"WO_NO_DECOM\" FROM \"TBLDTCFAILURE\" , \"TBLWORKORDER\" WHERE \"WO_DF_ID\" = \"DF_ID\" and \"DF_ID\" = '" + objApproval.sfailid + "'");
                    if (sEntype == "2")
                    {
                        objApproval.sDescription = "Enhacement of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    if (sEntype == "4")
                    {
                        objApproval.sDescription = "Repair and enhancement  of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        objApproval.sDescription = "Repair & replacement of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                }
                else if (objApproval.sBOId == "26")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));

                    string sRecordId = objcon.get_value("SELECT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"='" + objApproval.sWFObjectId + "'");

                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" ='" + sRecordId + "'");
                    if (sEntype == "2")
                    {
                        objApproval.sDescription = "Enhacement of Completion Report For DTC Code " + sRecordId;
                    }
                    if (sEntype == "4")
                    {
                        objApproval.sDescription = "Repair and enhancement  of Completion Report For DTC Code " + sRecordId;
                    }
                    else
                    {
                        objApproval.sDescription = "Repair & replacement of Completion Report For DTC Code " + sRecordId;
                    }
                }
                else if (objApproval.sBOId == "29")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sEntype = objcon.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\" ", NpgsqlCommand);

                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\" ", NpgsqlCommand);
                    if (sRecordId == "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sRecordId = objcon.get_value("SELECT \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\" WHERE \"WO_ID\" =:sWFObjectId AND  \"WO_RECORD_ID\"=\"TI_ID\"  AND \"TI_WO_SLNO\"=\"WO_SLNO\"", NpgsqlCommand);
                        objApproval.sDescription = "Invoice For New DTC Commission with  Indent No " + sRecordId.Split('~').GetValue(0).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement  of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        else
                        {
                            objApproval.sDescription = "Repair & replacement of  Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "24")
                {
                    string sOfficeCode = string.Empty;
                    if (Convert.ToInt32(objApproval.sRefOfficeCode) > 1)
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, Division);
                    }
                    else
                    {
                        sOfficeCode = objApproval.sRefOfficeCode;
                    }
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                    string sResult = objcon.get_value("SELECT \"SI_NO\" ||'~' || \"IS_NO\" FROM \"TBLSTOREINDENT\",\"TBLSTOREINVOICE\" WHERE \"IS_ID\" =:sNewRecordId AND \"SI_ID\"=\"IS_SI_ID\"", NpgsqlCommand);

                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(sOfficeCode));
                    string sStoreName = objcon.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\",\"TBLSTOREOFFCODE\" WHERE \"SM_ID\"=\"STO_SM_ID\" AND \"STO_OFF_CODE\" =:sOfficeCode", NpgsqlCommand);
                }
                else if (objApproval.sBOId == "32")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                    string sResult = objcon.get_value("SELECT \"SI_NO\" ||'~' || \"IS_NO\" FROM \"TBLSTOREINDENT\",\"TBLSTOREINVOICE\" WHERE \"IS_ID\" =:sNewRecordId AND \"SI_ID\"=\"IS_SI_ID\"", NpgsqlCommand);
                    objApproval.sDescription = "Response for Store Indent No " + sResult.Split('~').GetValue(0).ToString() + " with Store Invoice Number " + sResult.Split('~').GetValue(1).ToString();
                }
                else if (objApproval.sBOId == "46")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sRecordId = objcon.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=:sWFObjectId AND  \"WO_RECORD_ID\"=\"WO_SLNO\"", NpgsqlCommand);
                    objApproval.sDescription = " MinorFailure WO No " + sRecordId;
                }
                else if (objApproval.sBOId == "47" || objApproval.sBOId == "48")
                {
                    if (objApproval.sNewRecordId == null)
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                        sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLRECEIVEDTR\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"RD_WO_SLNO\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"DF_ID\" AND \"RD_ID\"=:sRecordId", NpgsqlCommand);
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                        sRecordId = objcon.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLRECEIVEDTR\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"RD_WO_SLNO\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"DF_ID\" AND \"RD_ID\"=:sNewRecordId", NpgsqlCommand);
                    }
                    objApproval.sDescription = "Commissioning of Minor Coil Failure DTC code " + sRecordId;
                }
                else if (objApproval.sBOId == "62")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sRecordId = objcon.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId", NpgsqlCommand);
                    objApproval.sDescription = "PermanentEstimation Request for DTC CODE  " + sRecordId;
                }
                else if (objApproval.sBOId == "63")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sRecordId = objcon.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId", NpgsqlCommand);
                    objApproval.sDescription = "PermanentWorkOrder Request for DTC CODE " + sDataReferenceId;
                }
                else if (objApproval.sBOId == "64")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                    sRecordId = objcon.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"PEST_ID\" =:sDataReferenceId", NpgsqlCommand);
                    objApproval.sDescription = "PermanentIndent Request for DTC CODE " + sRecordId;
                }
                else if (objApproval.sBOId == "65")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                    sRecordId = objcon.get_value("SELECT  \"PEST_DTC_CODE\" FROM \"TBLPERMANENTESTIMATIONDETAILS\",\"TBLPERMANENTWORKORDER\" WHERE \"PEST_ID\"=:sDataReferenceId and \"PEST_ID\"=\"PWO_PEF_ID\"", NpgsqlCommand);
                    objApproval.sDescription = "PermanentDecomm Request for DTC CODE " + sRecordId;
                }
                else if (objApproval.sBOId == "66")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sRecordId = objcon.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\",\"TBLPERMANENTWORKORDER\"   WHERE  \"PEST_ID\"=\"PWO_PEF_ID\" and \"PWO_SLNO\" =:sWFObjectId", NpgsqlCommand);
                    objApproval.sDescription = "PermanentRI Request for DTC CODE " + sDataReferenceId;
                }
                else if (objApproval.sBOId == "67")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    sRecordId = objcon.get_value("SELECT  distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =:sWFObjectId", NpgsqlCommand);
                    objApproval.sDescription = "PermanentCR Request for DTC CODE " + sDataReferenceId;
                }
                else if (objApproval.sBOId == "58")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sResult = objcon.get_value("SELECT \"DT_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" || '~' || \"DT_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCINVOICE\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCMAST\" WHERE \"DT_ID\" = \"WO_RECORD_ID\" AND \"WO_DATA_ID\" = CAST(\"IN_NO\" AS TEXT) AND \"WO_ID\" =:sWFObjectId AND \"TI_ID\" = \"IN_TI_NO\" AND \"TI_WO_SLNO\" = \"WO_SLNO\"", NpgsqlCommand);
                    objApproval.sDescription = "New DTC CR Request for DTC CODE " + sResult.Split('~').GetValue(0).ToString() + " with WO NO " + sResult.Split('~').GetValue(2).ToString();
                }

                else if (objApproval.sBOId == "76")
                {
                    ///rudresh 22-04-2020
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                    string sResult = objcon.get_value("SELECT \"RWOA_NO\" from \"TBLREPAIRERWORKAWARD\"  WHERE \"RWAO_ID\"=:sWFObjectId", NpgsqlCommand);
                    objApproval.sDescription = "TransilOil Replacement " + sResult.ToString() + " ";
                    string sResult1 = objcon.get_value("select \"WO_REF_OFFCODE\" from \"TBLWORKFLOWOBJECTS\" where \"WO_ID\"=:sWFObjectId", NpgsqlCommand);
                    if (sResult1 != null && sResult1 != "")
                    {
                        objApproval.sOfficeCode = sResult1;
                    }
                }
                NpgsqlCommand = new NpgsqlCommand();

                strQry = "INSERT INTO \"TBLWO_OBJECT_AUTO\" (\"WOA_ID\",\"WOA_BFM_ID\",\"WOA_PREV_APPROVE_ID\",\"WOA_ROLE_ID\",\"WOA_OFFICE_CODE\",";
                strQry += "\"WOA_CRBY\",\"WOA_DESCRIPTION\",\"WOA_REF_OFFCODE\",\"WOA_STATUS\") VALUES ((select max(\"WOA_ID\")+1 from \"TBLWO_OBJECT_AUTO\"),:sBOFlowMasterId,:sWFObjectId,";
                strQry += " :sRoleId,:sOfficeCode,:sCrby,:sDescription,:sRefOfficeCode,:status)";
                NpgsqlCommand.Parameters.AddWithValue("sMaxNo", Convert.ToInt32(sMaxNo));
                NpgsqlCommand.Parameters.AddWithValue("sBOFlowMasterId", Convert.ToInt32(objApproval.sBOFlowMasterId));
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objApproval.sOfficeCode));
                NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objApproval.sCrby));
                NpgsqlCommand.Parameters.AddWithValue("sDescription", objApproval.sDescription);
                NpgsqlCommand.Parameters.AddWithValue("sRefOfficeCode", Convert.ToInt32(objApproval.sRefOfficeCode));

                if (objApproval.sStatus == null || objApproval.sStatus == "")
                {
                    objApproval.sStatus = "";
                }
                NpgsqlCommand.Parameters.AddWithValue("status", objApproval.sStatus);
                objcon.ExecuteQry(strQry, NpgsqlCommand);

                #region WCF Methods
                //if (objApproval.sBOId == "29")
                //{
                //    bool isSuccess;
                //    strQry = string.Empty;
                //    DtmmsWebService.Service1Client objWcf = new DtmmsWebService.Service1Client();

                //    ///////////Update TBLWO_OBJECT_AUTO(WOA_FLAG) so (DTLMS) STO will not Get the PseudoIndent Record To Approve////////////
                //    strQry = "UPDATE TBLWO_OBJECT_AUTO SET WOA_FLAG='1' WHERE WOA_ID='" + sMaxNo + "'";
                //    objcon.Execute(strQry);
                //    ///////////////////////////Update TBLWO_OBJECT_AUTO////////////////////////////////////


                //    strQry = " SELECT * FROM TBLWORKORDER,TBLDTCFAILURE,TBLTCMASTER,TBLINDENT,TBLESTIMATION, ";
                //    strQry += " TBLWORKFLOWOBJECTS,TBLWO_OBJECT_AUTO  WHERE DF_EQUIPMENT_ID=TC_CODE AND WO_DF_ID=DF_ID AND WO_SLNO=TI_WO_SLNO AND EST_DF_ID=DF_ID AND ";
                //    strQry += " WO_ID=WOA_PREV_APPROVE_ID AND TI_ID=WO_RECORD_ID AND WOA_PREV_APPROVE_ID='" + objApproval.sWFObjectId + "'";

                //    DataTable dtIndentDetails = new DataTable("TableIndentDetails");
                //    dtIndentDetails = objcon.getDataTable(strQry);
                //    isSuccess = objWcf.SaveIndentDetails(dtIndentDetails);
                //    //if (isSuccess)
                //    //objcon.CommitTrans();
                //    //else
                //    //objcon.RollBack(); 
                //}

                //if (objApproval.sBOId == "15")
                //{
                //    DataTable dt2 = new DataTable();
                //    DataTable dt = new DataTable();
                //    bool IsSuccess = false;
                //    strQry = " SELECT TR_IN_NO FROM TBLTCREPLACE WHERE TR_ID =(SELECT MAX(TR_ID) FROM TBLTCREPLACE )";
                //    string res = objcon.get_value(strQry);

                //    strQry = "SELECT TR_ID,WO_SLNO,IN_DATE,TR_RI_DATE,TR_CRBY,TR_DESC,TR_STORE_SLNO,TR_OIL_QUNTY,DF_STATUS_FLAG FROM TBLWORKORDER,TBLINDENT,TBLDTCINVOICE,";
                //    strQry += "TBLTCREPLACE,TBLDTCFAILURE WHERE TR_IN_NO=IN_NO AND IN_TI_NO=TI_ID AND TI_WO_SLNO=WO_SLNO AND WO_DF_ID=DF_ID AND TR_IN_NO='" + res + "' ";
                //    dt = objcon.getDataTable(strQry);

                //    strQry = "SELECT * FROM TBLWORKFLOWOBJECTS,TBLWO_OBJECT_AUTO WHERE WO_ID=(SELECT max(WO_ID) FROM TBLWORKFLOWOBJECTS WHERE WO_RECORD_ID='" + res + "') AND WO_ID=WOA_PREV_APPROVE_ID";
                //    dt2 = objcon.getDataTable(strQry);

                //    DtmmsWebService.Service1Client objWcf = new DtmmsWebService.Service1Client();
                //    DataTable RiDetails = new DataTable("Ridetails");

                //    RiDetails.Columns.Add("FORMNAME", typeof(string));

                //    #region RI Details
                //    RiDetails.Columns.Add("TR_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_SLNO", typeof(string));
                //    RiDetails.Columns.Add("TR_IN_NO", typeof(string));
                //    RiDetails.Columns.Add("IN_DATE", typeof(string));
                //    RiDetails.Columns.Add("TR_RI_DATE", typeof(string));
                //    RiDetails.Columns.Add("TR_CRBY", typeof(string));
                //    RiDetails.Columns.Add("TR_DESC", typeof(string));
                //    RiDetails.Columns.Add("TR_STORE_SLNO", typeof(string));
                //    RiDetails.Columns.Add("TR_OIL_QUNTY", typeof(string));
                //    RiDetails.Columns.Add("DF_STATUS_FLAG", typeof(string));

                //    #endregion

                //    #region TBLWORKFLOWOBJECTS details

                //    RiDetails.Columns.Add("WO_BO_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_RECORD_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_PREV_APPROVE_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_NEXT_ROLE", typeof(string));
                //    RiDetails.Columns.Add("WO_OFFICE_CODE", typeof(string));
                //    RiDetails.Columns.Add("WO_USER_COMMENT", typeof(string));
                //    RiDetails.Columns.Add("WO_APPROVED_BY", typeof(string));
                //    RiDetails.Columns.Add("WO_APPROVE_STATUS", typeof(string));
                //    RiDetails.Columns.Add("WO_CR_BY", typeof(string));
                //    RiDetails.Columns.Add("WO_CR_ON", typeof(string));
                //    RiDetails.Columns.Add("WO_RECORD_BY", typeof(string));
                //    RiDetails.Columns.Add("WO_DEVICE_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_DESCRIPTION", typeof(string));
                //    RiDetails.Columns.Add("WO_WFO_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_INITIAL_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_DATA_ID", typeof(string));
                //    RiDetails.Columns.Add("WO_REF_OFFCODE", typeof(string));

                //    #endregion

                //    #region TBLWO_OBJECT_AUTO details

                //    RiDetails.Columns.Add("WOA_ID", typeof(string));
                //    RiDetails.Columns.Add("WOA_BFM_ID", typeof(string));
                //    RiDetails.Columns.Add("WOA_PREV_APPROVE_ID", typeof(string));
                //    RiDetails.Columns.Add("WOA_ROLE_ID", typeof(string));
                //    RiDetails.Columns.Add("WOA_OFFICE_CODE", typeof(string));
                //    RiDetails.Columns.Add("WOA_INITIAL_ACTION_ID", typeof(string));
                //    RiDetails.Columns.Add("WOA_DESCRIPTION", typeof(string));
                //    RiDetails.Columns.Add("WOA_CRBY", typeof(string));
                //    RiDetails.Columns.Add("WOA_CRON", typeof(string));
                //    RiDetails.Columns.Add("WOA_REF_OFFCODE", typeof(string));

                //    #endregion

                //    DataRow dtrow = RiDetails.NewRow();
                //    dtrow["FORMNAME"] = "ReturnInvoice";

                //    dtrow["TR_ID"] = dt.Rows[0]["TR_ID"].ToString();
                //    dtrow["WO_SLNO"] = dt.Rows[0]["WO_SLNO"].ToString();
                //    dtrow["TR_IN_NO"] = res;
                //    dtrow["IN_DATE"] = dt.Rows[0]["IN_DATE"].ToString();
                //    dtrow["TR_RI_DATE"] = dt.Rows[0]["TR_RI_DATE"].ToString();
                //    dtrow["TR_CRBY"] = dt.Rows[0]["TR_CRBY"].ToString();
                //    dtrow["TR_DESC"] = dt.Rows[0]["TR_DESC"].ToString();
                //    dtrow["TR_STORE_SLNO"] = dt.Rows[0]["TR_STORE_SLNO"].ToString();
                //    dtrow["TR_OIL_QUNTY"] = dt.Rows[0]["TR_OIL_QUNTY"].ToString();
                //    dtrow["DF_STATUS_FLAG"] = dt.Rows[0]["DF_STATUS_FLAG"].ToString();

                //    dtrow["WO_BO_ID"] = dt2.Rows[0]["WO_BO_ID"].ToString();
                //    dtrow["WO_RECORD_ID"] = dt2.Rows[0]["WO_RECORD_ID"].ToString();
                //    dtrow["WO_PREV_APPROVE_ID"] = dt2.Rows[0]["WO_PREV_APPROVE_ID"].ToString();
                //    dtrow["WO_NEXT_ROLE"] = dt2.Rows[0]["WO_NEXT_ROLE"].ToString();
                //    dtrow["WO_OFFICE_CODE"] = dt2.Rows[0]["WO_OFFICE_CODE"].ToString();
                //    dtrow["WO_CR_ON"] = dt2.Rows[0]["WO_CR_ON"].ToString();
                //    dtrow["WO_USER_COMMENT"] = dt2.Rows[0]["WO_USER_COMMENT"].ToString();
                //    dtrow["WO_APPROVED_BY"] = dt2.Rows[0]["WO_APPROVED_BY"].ToString();
                //    dtrow["WO_APPROVE_STATUS"] = dt2.Rows[0]["WO_APPROVE_STATUS"].ToString();
                //    dtrow["WO_CR_BY"] = dt2.Rows[0]["WO_CR_BY"].ToString();
                //    dtrow["WO_RECORD_BY"] = dt2.Rows[0]["WO_RECORD_BY"].ToString();
                //    dtrow["WO_DEVICE_ID"] = dt2.Rows[0]["WO_DEVICE_ID"].ToString();
                //    dtrow["WO_DESCRIPTION"] = dt2.Rows[0]["WO_DESCRIPTION"].ToString();
                //    dtrow["WO_WFO_ID"] = dt2.Rows[0]["WO_WFO_ID"].ToString();
                //    dtrow["WO_INITIAL_ID"] = dt2.Rows[0]["WO_INITIAL_ID"].ToString();
                //    dtrow["WO_DATA_ID"] = dt2.Rows[0]["WO_DATA_ID"].ToString();
                //    dtrow["WO_REF_OFFCODE"] = dt2.Rows[0]["WO_REF_OFFCODE"].ToString();

                //    dtrow["WOA_ID"] = dt2.Rows[0]["WOA_ID"].ToString();
                //    dtrow["WOA_BFM_ID"] = dt2.Rows[0]["WOA_BFM_ID"].ToString();
                //    dtrow["WOA_PREV_APPROVE_ID"] = dt2.Rows[0]["WOA_PREV_APPROVE_ID"].ToString();
                //    dtrow["WOA_ROLE_ID"] = dt2.Rows[0]["WOA_ROLE_ID"].ToString();
                //    dtrow["WOA_OFFICE_CODE"] = dt2.Rows[0]["WOA_OFFICE_CODE"].ToString();
                //    dtrow["WOA_INITIAL_ACTION_ID"] = dt2.Rows[0]["WOA_INITIAL_ACTION_ID"].ToString();
                //    dtrow["WOA_DESCRIPTION"] = dt2.Rows[0]["WOA_DESCRIPTION"].ToString();
                //    dtrow["WOA_CRBY"] = dt2.Rows[0]["WOA_CRBY"].ToString();
                //    dtrow["WOA_CRON"] = dt2.Rows[0]["WOA_CRON"].ToString();
                //    dtrow["WOA_REF_OFFCODE"] = dt2.Rows[0]["WOA_REF_OFFCODE"].ToString();


                //    RiDetails.Rows.Add(dtrow);
                //    IsSuccess = objWcf.SaveRVData(RiDetails);
                //}
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool SaveWFObjectAuto1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                Division = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
                SubDivision = Convert.ToInt32(ConfigurationManager.AppSettings["SubDiv_code"]);
                Section = Convert.ToInt32(ConfigurationManager.AppSettings["Section_code"]);

                string strQry = string.Empty;
                string sMaxNo = objDatabse.get_value("select max(\"WOA_ID\")+1 from \"TBLWO_OBJECT_AUTO\"");
                if (objApproval.sBOId == "25" || objApproval.sBOId == "11")
                {
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT) and \"EST_FAILUREID\"=\"DF_ID\"");
                    string sftype = objDatabse.get_value("SELECT \"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT) and \"EST_FAILUREID\"=\"DF_ID\"");
                    string sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"DF_ID\"=CAST(\"WO_DATA_ID\" AS BIGINT)");
                    if (sftype == "2" || sftype == "")
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = " Enhacement of  Work Order For DTC Code " + sRecordId;
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement of Work Order For DTC Code " + sRecordId;
                        }
                        else
                        {
                            objApproval.sDescription = " Repair and replacement of  Major faulty Work Order For DTC Code " + sRecordId;
                        }
                    }
                    else
                    {
                        objApproval.sDescription = " Repair and replacement of  Minor faulty Work Order For DTC Code " + sRecordId;
                    }

                }
                else if (objApproval.sBOId == "12")
                {
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\" and \"EST_FAILUREID\"=\"DF_ID\"");
                    string sftype = objDatabse.get_value("SELECT \"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\" and \"EST_FAILUREID\"=\"DF_ID\"");
                    string sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"DF_ID\"=\"WO_DF_ID\"");
                    if (sRecordId == "")
                    {
                        if (objApproval.sTTKStatus == "1")
                        {
                            sRecordId = objDatabse.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\"");
                            objApproval.sDescription = "Indent For New DTC Commission TTK Flow WO No " + sRecordId;
                        }
                        else
                        {
                            sRecordId = objDatabse.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\"");
                            objApproval.sDescription = "Indent For New DTC Commission PTK with WO No " + sRecordId;
                        }
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement of  Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        else
                        {
                            objApproval.sDescription = "Repair & replacement of Indent For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " and WO No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "13" || objApproval.sBOId == "56" || objApproval.sBOId == "57")
                {
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\" = \"WO_SLNO\" AND \"DT_CODE\" = \"DF_DTC_CODE\"");
                    string sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" || '~' || \"DT_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\" = \"WO_SLNO\" AND \"DT_CODE\" = \"DF_DTC_CODE\"");
                    if (sRecordId == "")
                    {
                        sRecordId = objDatabse.get_value("SELECT \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\"");
                        objApproval.sDescription = "Invoice For New DTC Commission with Indent No " + sRecordId.Split('~').GetValue(0).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of  Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement  of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                        else
                        {
                            objApproval.sDescription = "Repair & replacement of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + ", DTC Name " + sRecordId.Split('~').GetValue(3).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "14")
                {
                    string sRecordId = objApproval.sdtccode;
                    objApproval.sDescription = "Decommissioning For DTC Code " + sRecordId;
                    if (sRecordId == "")
                    {
                        sRecordId = objDatabse.get_value("SELECT \"IN_INV_NO\" FROM \"TBLDTCINVOICE\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_RECORD_ID\"=\"IN_NO\" ");
                        objApproval.sDescription = "Commissioning of DTC for the Invoice NO " + sRecordId;
                    }
                }
                else if (objApproval.sBOId == "15")
                {
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" = '" + objApproval.sfailid + "'");
                    string sRecordId = objDatabse.get_value("SELECT \"DF_EQUIPMENT_ID\" || '~' || \"WO_NO_DECOM\" FROM \"TBLDTCFAILURE\" , \"TBLWORKORDER\" WHERE \"WO_DF_ID\" = \"DF_ID\" and \"DF_ID\" = '" + objApproval.sfailid + "'");
                    if (sEntype == "2")
                    {
                        objApproval.sDescription = "Enhacement of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    if (sEntype == "4")
                    {
                        objApproval.sDescription = "Repair and enhancement  of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        objApproval.sDescription = "Repair & replacement of RI Approval For DTr Code " + sRecordId.Split('~').GetValue(0).ToString() + " and Work Order NO " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                }
                else if (objApproval.sBOId == "26")
                {
                    string sRecordId = objDatabse.get_value("SELECT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"='" + objApproval.sWFObjectId + "'");
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" ='" + sRecordId + "'");
                    if (sEntype == "2")
                    {
                        objApproval.sDescription = "Enhacement of Completion Report For DTC Code " + sRecordId;
                    }
                    if (sEntype == "4")
                    {
                        objApproval.sDescription = "Repair and enhancement  of Completion Report For DTC Code " + sRecordId;
                    }
                    else
                    {
                        objApproval.sDescription = "Repair & replacement of Completion Report For DTC Code " + sRecordId;

                    }
                }
                else if (objApproval.sBOId == "29")
                {
                    string sEntype = objDatabse.get_value("SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\" ");
                    string sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"TI_WO_SLNO\"=\"WO_SLNO\" ");
                    if (sRecordId == "")
                    {
                        sRecordId = objDatabse.get_value("SELECT \"TI_INDENT_NO\" || '~' || \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLINDENT\",\"TBLWORKORDER\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"TI_ID\"  AND \"TI_WO_SLNO\"=\"WO_SLNO\"");
                        objApproval.sDescription = "Invoice For New DTC Commission with  Indent No " + sRecordId.Split('~').GetValue(0).ToString() + " and WorkOrder No " + sRecordId.Split('~').GetValue(1).ToString();
                    }
                    else
                    {
                        if (sEntype == "2")
                        {
                            objApproval.sDescription = "Enhacement of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        if (sEntype == "4")
                        {
                            objApproval.sDescription = "Repair and enhancement  of Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                        else
                        {
                            objApproval.sDescription = "Repair & replacement of  Invoice For DTC Code " + sRecordId.Split('~').GetValue(0).ToString() + " with WorkOrder No " + sRecordId.Split('~').GetValue(2).ToString() + " and Indent No " + sRecordId.Split('~').GetValue(1).ToString();
                        }
                    }
                }
                else if (objApproval.sBOId == "24")
                {
                    string sOfficeCode = string.Empty;
                    if (Convert.ToInt32(objApproval.sRefOfficeCode) > 1)
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, Division);
                    }
                    else
                    {
                        sOfficeCode = objApproval.sRefOfficeCode;
                    }
                    string sResult = objDatabse.get_value("SELECT \"SI_NO\" ||'~' || \"IS_NO\" FROM \"TBLSTOREINDENT\",\"TBLSTOREINVOICE\" WHERE \"IS_ID\" =" + Convert.ToInt32(objApproval.sNewRecordId) + " AND \"SI_ID\"=\"IS_SI_ID\"");
                    string sStoreName = objDatabse.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\",\"TBLSTOREOFFCODE\" WHERE \"SM_ID\"=\"STO_SM_ID\" AND \"STO_OFF_CODE\" =" + Convert.ToInt32(sOfficeCode) + "");
                }
                else if (objApproval.sBOId == "32")
                {
                    string sResult = objDatabse.get_value("SELECT \"SI_NO\" ||'~' || \"IS_NO\" FROM \"TBLSTOREINDENT\",\"TBLSTOREINVOICE\" WHERE \"IS_ID\" =" + Convert.ToInt32(objApproval.sNewRecordId) + " AND \"SI_ID\"=\"IS_SI_ID\"");
                    objApproval.sDescription = "Response for Store Indent No " + sResult.Split('~').GetValue(0).ToString() + " with Store Invoice Number " + sResult.Split('~').GetValue(1).ToString();
                }
                else if (objApproval.sBOId == "46")
                {
                    sRecordId = objDatabse.get_value("SELECT  \"WO_NO\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + " AND  \"WO_RECORD_ID\"=\"WO_SLNO\"");
                    objApproval.sDescription = " MinorFailure WO No " + sRecordId;
                }

                else if (objApproval.sBOId == "47" || objApproval.sBOId == "48")
                {
                    if (objApproval.sNewRecordId == null)
                    {
                        sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLRECEIVEDTR\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"RD_WO_SLNO\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"DF_ID\" AND \"RD_ID\"=" + Convert.ToInt64(objApproval.sRecordId) + "");
                    }
                    else
                    {
                        sRecordId = objDatabse.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLRECEIVEDTR\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"RD_WO_SLNO\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"DF_ID\" AND \"RD_ID\"=" + Convert.ToInt32(objApproval.sNewRecordId) + "");
                    }

                    objApproval.sDescription = "Commissioning of Minor Coil Failure DTC code " + sRecordId;

                }


                else if (objApproval.sBOId == "62")
                {
                    sRecordId = objDatabse.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    objApproval.sDescription = "PermanentEstimation Request for DTC CODE  " + sRecordId;
                }
                else if (objApproval.sBOId == "63")
                {
                    sRecordId = objDatabse.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    objApproval.sDescription = "PermanentWorkOrder Request for DTC CODE " + sDataReferenceId;
                }


                else if (objApproval.sBOId == "64")
                {
                    sRecordId = objDatabse.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"PEST_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + "");
                    objApproval.sDescription = "PermanentIndent Request for DTC CODE " + sRecordId;
                }
                else if (objApproval.sBOId == "65")
                {
                    sRecordId = objDatabse.get_value("SELECT  \"PEST_DTC_CODE\" FROM \"TBLPERMANENTESTIMATIONDETAILS\",\"TBLPERMANENTWORKORDER\" WHERE \"PEST_ID\"=" + Convert.ToInt64(objApproval.sDataReferenceId) + " and \"PEST_ID\"=\"PWO_PEF_ID\"");

                    objApproval.sDescription = "PermanentDecomm Request for DTC CODE " + sRecordId;
                }

                else if (objApproval.sBOId == "66")
                {
                    sRecordId = objDatabse.get_value("SELECT distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\",\"TBLPERMANENTWORKORDER\"   WHERE  \"PEST_ID\"=\"PWO_PEF_ID\" and \"PWO_SLNO\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    objApproval.sDescription = "PermanentRI Request for DTC CODE " + sDataReferenceId;
                }
                else if (objApproval.sBOId == "67")
                {
                    sRecordId = objDatabse.get_value("SELECT  distinct \"PEST_DTC_CODE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    objApproval.sDescription = "PermanentCR Request for DTC CODE " + sDataReferenceId;
                }
                else if (objApproval.sBOId == "58")
                {
                    string sResult = objDatabse.get_value("SELECT \"DT_CODE\" || '~' || \"TI_INDENT_NO\" || '~' || \"WO_NO\" || '~' || \"DT_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCINVOICE\",\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCMAST\" WHERE \"DT_ID\" = \"WO_RECORD_ID\" AND \"WO_DATA_ID\" = CAST(\"IN_NO\" AS TEXT) AND \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"TI_ID\" = \"IN_TI_NO\" AND \"TI_WO_SLNO\" = \"WO_SLNO\"");
                    objApproval.sDescription = "New DTC CR Request for DTC CODE " + sResult.Split('~').GetValue(0).ToString() + " with WO NO " + sResult.Split('~').GetValue(2).ToString();
                }

                else if (objApproval.sBOId == "76")
                {


                    ///rudresh 22-04-2020
                    string sResult = objDatabse.get_value("SELECT \"RWOA_NO\" from \"TBLREPAIRERWORKAWARD\"  WHERE \"RWAO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    objApproval.sDescription = "TransilOil Replacement " + sResult.ToString() + " ";
                    string sResult1 = objDatabse.get_value("select \"WO_REF_OFFCODE\" from \"TBLWORKFLOWOBJECTS\" where \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + "");
                    if (sResult1 != null && sResult1 != "")
                    {
                        objApproval.sOfficeCode = sResult1;
                    }
                }
                if (objApproval.sStatus == null || objApproval.sStatus == "")
                {
                    objApproval.sStatus = "";
                }
                strQry = "INSERT INTO \"TBLWO_OBJECT_AUTO\" (\"WOA_ID\",\"WOA_BFM_ID\",\"WOA_PREV_APPROVE_ID\",\"WOA_ROLE_ID\",\"WOA_OFFICE_CODE\",";
                strQry += "\"WOA_CRBY\",\"WOA_DESCRIPTION\",\"WOA_REF_OFFCODE\",\"WOA_STATUS\") VALUES (" + Convert.ToInt64(sMaxNo) + "," + Convert.ToInt32(objApproval.sBOFlowMasterId) + "," + Convert.ToInt64(objApproval.sWFObjectId) + ",";
                strQry += " " + Convert.ToInt32(objApproval.sRoleId) + "," + Convert.ToInt32(objApproval.sOfficeCode) + "," + Convert.ToInt32(objApproval.sCrby) + ",'" + objApproval.sDescription + "'," + Convert.ToInt32(objApproval.sRefOfficeCode) + ",'" + objApproval.sStatus + "')";
                objDatabse.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// Update Initial Action Id from Workflow Object Id in TBLWO_OBJECT_AUTO Table
        /// </summary>
        /// <param name="objApproval"></param>
        /// <returns></returns>
        public bool UpdateWFOAutoObject(clsApproval objApproval)
        {
            try
            {
                string strQry = string.Empty;
                string var = string.Empty;
                if (objApproval.sbfm_id == null || objApproval.sbfm_id == string.Empty || objApproval.sbfm_id == "")
                {
                    objApproval.sbfm_id = "0";
                }
                if (objApproval.sWFObjectId == null || objApproval.sWFObjectId == string.Empty || objApproval.sWFObjectId == "")
                {
                    objApproval.sWFObjectId = "0";
                }

                if (objApproval.sPrevWFOId == null || objApproval.sPrevWFOId == string.Empty || objApproval.sPrevWFOId == "")
                {
                    objApproval.sPrevWFOId = "0";
                }
                strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_INITIAL_ACTION_ID\" ='" + Convert.ToInt64(objApproval.sWFObjectId) + "' WHERE \"WOA_PREV_APPROVE_ID\" ='" + Convert.ToInt32(objApproval.sPrevWFOId) + "' and \"WOA_BFM_ID\"='" + Convert.ToInt32(objApproval.sbfm_id) + "'";
                objcon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool UpdateWFOAutoObject1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {
                string strQry = string.Empty;
                string var = string.Empty;
                if (objApproval.sbfm_id == null || objApproval.sbfm_id == string.Empty || objApproval.sbfm_id == "")
                {
                    objApproval.sbfm_id = "0";
                }
                if (objApproval.sWFObjectId == null || objApproval.sWFObjectId == string.Empty || objApproval.sWFObjectId == "")
                {
                    objApproval.sWFObjectId = "0";
                }

                if (objApproval.sPrevWFOId == null || objApproval.sPrevWFOId == string.Empty || objApproval.sPrevWFOId == "")
                {
                    objApproval.sPrevWFOId = "0";
                }
                strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_INITIAL_ACTION_ID\" ='" + Convert.ToInt64(objApproval.sWFObjectId) + "' WHERE \"WOA_PREV_APPROVE_ID\" ='" + Convert.ToInt64(objApproval.sPrevWFOId) + "' and \"WOA_BFM_ID\"='" + Convert.ToInt32(objApproval.sbfm_id) + "'";
                objDatabse.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// To get Form Creator Access (If priority is 1, Consider as Form Creator)
        /// </summary>
        /// <param name="sBOId"></param>
        /// <param name="sRoleId"></param>
        /// <param name="sFormName"></param>
        /// <returns></returns>
        public string GetFormCreatorLevel(string sBOId, string sRoleId, string sFormName = "", string Woa_id = "")
        {
            try
            {
                string strQry = string.Empty;
                if (sBOId != "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WM_LEVEL\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_ROLEID\" =:sRoleId AND \"WM_BOID\" =:sBOId";
                    NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(sRoleId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));

                    if (sRoleId == Convert.ToString(ConfigurationManager.AppSettings["SupAdminRole"]) && Woa_id != "" && Woa_id != null)
                    {
                        strQry = " select case when \"WOA_INITIAL_ACTION_ID\" is null then 1 else 0 end  from \"TBLWO_OBJECT_AUTO\" ,\"TBLWORKFLOWOBJECTS\"  where  \"WO_ID\"=\"WOA_PREV_APPROVE_ID\" and \"WOA_ID\"=:sWoa_id";

                        NpgsqlCommand.Parameters.AddWithValue("sWoa_id", Convert.ToInt32(Woa_id));
                    }
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"WM_LEVEL\" FROM \"TBLWORKFLOWMASTER\" WHERE \"WM_ROLEID\" =:sRoleId AND \"WM_BOID\" = ";
                    strQry += " (SELECT \"BO_ID\" FROM \"TBLBUSINESSOBJECT\" WHERE UPPER(\"BO_FORMNAME\")=:sFormName)";
                    NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(sRoleId));
                    NpgsqlCommand.Parameters.AddWithValue("sFormName", sFormName.Trim().ToUpper());
                }
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        /// <summary>
        /// Get Datatable from XML string
        /// </summary>
        /// <param name="sWFDataId"></param>
        /// <returns></returns>
        public DataTable GetDatatableFromXML(string sWFDataId)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (sWFDataId != "")
                {
                    string sXMLResult = GetWFOData(sWFDataId);

                    StringReader sReader = new StringReader(sXMLResult);
                    ds.ReadXml(sReader);
                    return ds.Tables[0];
                }
                if (ds.Tables.Count == 0)
                {
                    return dt;
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ds.Tables[0];

            }
        }

        /// <summary>
        /// Get Datatable from Multiple XML string
        /// </summary>
        /// <param name="sWFDataId"></param>
        /// <returns></returns>
        public DataSet GetDatatableFromMultipleXML(string sWFDataId)
        {
            DataSet ds = new DataSet();
            DataSet dsResult = new DataSet();
            try
            {
                string sXMLResult = GetWFOData(sWFDataId);
                if (sXMLResult != "")
                {
                    StringReader sReader = new StringReader(sXMLResult);
                    dsResult.ReadXml(sReader);
                }
                return dsResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dsResult;
            }
        }


        /// <summary>
        /// Create Xml format using Table Name and Column Name with Values
        /// </summary>
        /// <param name="strColumns"></param>
        /// <param name="strParameters"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public string CreateXml(string strColumns, string strParameters, string strTableName)
        {
            try
            {
                DataTable dtXmlContent = new DataTable();

                DataTable dtnew = new DataTable();

                DataSet ds;
                if (strTableName.Contains(";"))
                {
                    ds = new DataSet(strTableName.Split(';').GetValue(0).ToString());
                }
                else
                {
                    ds = new DataSet(strTableName);
                }

                string[] strArrColumns = strColumns.Split(';');
                string[] strArrParameters = strParameters.Split(';');
                string[] strTableNames = strTableName.Split(';');

                int k = 0;
                for (int i = 0; i < strArrColumns.Length; i++)
                {
                    DataTable dt = new DataTable();
                    DataRow dRow = dt.NewRow();
                    string[] strdtColumns = strArrColumns[i].Split(',');
                    string[] strdtParametres = strArrParameters[i].Split(',');
                    dt.TableName = strTableNames[i];
                    for (int j = 0; j < strdtColumns.Length; j++)
                    {
                        dt.Columns.Add(strdtColumns[j]);
                        if (k < strdtParametres.Length)
                        {
                            string strColumnName = strdtParametres[k];
                            dRow[dt.Columns[j]] = strdtParametres[k];
                            if (dt.Rows.Count == 0)
                            {
                                dt.Rows.Add(dRow);
                            }
                            dt.AcceptChanges();
                        }
                        k++;
                    }
                    k = 0;

                    ds.Merge(dt);
                    dt.Clear();

                }
                return ds.GetXml();
            }

            catch (Exception ex)
            {
                string strfailure = string.Empty;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return strfailure;
            }
        }


        /// <summary>
        /// To get XML String from TBLWFODATA based on WFODataId corresponding to WorkflowObject Id from TBLWORKFLOWOBJECTS
        /// </summary>
        /// <param name="sWFDataId"></param>
        /// <returns></returns>
        public string GetWFOData(string sWFDataId)
        {
            try
            {
                string strQry = string.Empty;
                if (sWFDataId == "" || sWFDataId == string.Empty || sWFDataId == null)
                {
                    sWFDataId = "0";
                }
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WFO_DATA\" FROM \"TBLWFODATA\" WHERE \"WFO_ID\" =:sWFDataId";
                NpgsqlCommand.Parameters.AddWithValue("sWFDataId", Convert.ToInt64(sWFDataId));
                string sXMLResult = objcon.get_value(strQry, NpgsqlCommand);
                return sXMLResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        #region Extra Code
        public string CreateXml1(string strColumns, string strParameters)
        {
            try
            {
                DataTable dtXmlContent = new DataTable();
                DataTable dt = new DataTable();
                string[] strdtColumns = new string[10];
                string[] strdtParameterValues = new string[10];
                strdtColumns = strColumns.Split(',');
                strdtParameterValues = strParameters.Split(',');
                int j = 0;
                DataRow dRow = dt.NewRow();
                for (int i = 0; i < strdtColumns.Length; i++)
                {
                    dt.Columns.Add(strdtColumns[i]);
                    if (j < strdtParameterValues.Length)
                    {
                        string strColumnName = strdtColumns[i];
                        dRow[dt.Columns[i]] = strdtParameterValues[j];
                        if (dt.Rows.Count == 0)
                        {
                            dt.Rows.Add(dRow);
                        }
                        dt.AcceptChanges();
                    }
                    j++;
                }
                dt.TableName = "TBLDTCFAILURE";
                DataSet ds = new DataSet("TBLDTCFAILURE");
                ds.Tables.Add(dt);
                return ds.GetXml();
            }
            catch (Exception ex)
            {
                string strfailure = string.Empty;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return strfailure;
            }
        }
        #endregion

        /// <summary>
        /// Check Data already exist and Waiting for Approval to Restrict duplicate Entry
        /// </summary>
        /// <param name="sDataReferenceId"></param>
        /// <param name="sBOId"></param>
        /// <returns></returns>
        public bool CheckAlreadyExistEntry(string sDataReferenceId, string sBOId)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" =:sDataReferenceId AND \"WO_APPROVE_STATUS\" ='0' AND \"WO_BO_ID\" =:sBOId";
                NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", sDataReferenceId);
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// This method used to check the estimation record already existed
        /// </summary>
        /// <param name="sDataReferenceId"></param>
        /// <param name="sBOId"></param>
        /// <returns></returns>
        public bool CheckAlreadyExistEntryfaulty(string sDataReferenceId, string sBOId, string Officecode)
        {
            try
            {
                string Qry = string.Empty;
                string sResult = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (sBOId == "71")
                {

                    Qry = "SELECT \"WO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" =:sDataReferenceId  AND \"WO_BO_ID\" =:sBOId";
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", sDataReferenceId);
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                    sResult = objcon.get_value(Qry, NpgsqlCommand);
                    if (sResult != "")
                    {
                        Qry = " SELECT Max(\"RSD_ID\") FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" =:sTCcode ";
                        Qry += " AND \"RSD_DELIVARY_DATE\" IS NOT NULL and \"RSD_PROCESS_FLAG\" = 1";
                        NpgsqlCommand.Parameters.AddWithValue("sTCcode", Convert.ToInt32(sDataReferenceId));
                        string flag = objcon.get_value(Qry, NpgsqlCommand);
                        if (flag.Length == 0)
                        {
                            return true;
                        }
                        else
                        {
                            Qry = "SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" =:sDataReferenceId1 ";
                            Qry += " AND \"WO_BO_ID\" =:sBOId1 and \"WO_RECORD_ID\" < 0";
                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", sDataReferenceId);
                            NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(sBOId));
                            sResult = objcon.get_value(Qry, NpgsqlCommand);
                            if (sResult.Length != 0)
                            {
                                return true;
                            }
                            else
                            {
                                Qry = "SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" ";
                                Qry += " WHERE \"WO_DATA_ID\" =:sDataReferenceId1  AND \"WO_BO_ID\" =:sBOId1";
                                NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", sDataReferenceId);
                                NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(sBOId));
                                string wo_id = objcon.get_value(Qry, NpgsqlCommand);
                                Qry = "select \"WOA_INITIAL_ACTION_ID\" from \"TBLWO_OBJECT_AUTO\" ";
                                Qry += "where \"WOA_PREV_APPROVE_ID\" = '" + wo_id + "' ORDER BY \"WOA_ID\"";
                                string Initialactionid = objcon.get_value(Qry);
                                if ((Initialactionid ?? "").Length == 0)
                                {
                                    return true;
                                }
                            }
                            return false;
                        }

                    }
                    return false;
                }

                else
                {
                    Qry = "SELECT  \"WO_RECORD_ID\"  ||'~'|| MAX(\"WO_ID\")  FROM \"TBLWORKFLOWOBJECTS\" ";
                    Qry += " WHERE \"WO_DATA_ID\" =:sDataReferenceId  AND \"WO_BO_ID\" =71 ";
                    Qry += " and \"WO_RECORD_ID\" > 0  GROUP BY \"WO_RECORD_ID\"  ORDER BY \"WO_RECORD_ID\" desc ";
                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", sDataReferenceId);
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                    string EstID = objcon.get_value(Qry, NpgsqlCommand);
                    if (EstID != "")
                    {
                        Qry = "SELECT MAX(\"WO_ID\") from \"TBLWORKFLOWOBJECTS\" where \"WO_DATA_ID\" = '"
                            + EstID.Split('~').GetValue(0).ToString() + "' and \"WO_RECORD_ID\" < 0 AND \"WO_BO_ID\" =72";
                        string Wo_id = objcon.get_value(Qry);
                        if ((Wo_id ?? "").Length != 0)
                        {
                            return true;
                        }

                        Qry = "SELECT MAX(\"WO_BO_ID\") from \"TBLWORKFLOWOBJECTS\" where \"WO_DESCRIPTION\" like '% "
                            + sDataReferenceId + "%' and \"WO_RECORD_ID\" > 0 AND \"WO_REF_OFFCODE\"='" + Officecode + "' GROUP BY \"WO_ID\" ORDER BY \"WO_ID\"desc limit 1";
                        string Bo_id = objcon.get_value(Qry);
                        if ((Bo_id ?? "").Length != 0 && Bo_id == "15")
                        {
                            return false;
                        }

                        Qry = "SELECT  MAX(\"WO_BO_ID\") from \"TBLWORKFLOWOBJECTS\" where \"WO_DESCRIPTION\" like '% "
                            + sDataReferenceId + "%' and \"WO_BO_ID\" in (15,71,72)  AND \"WO_REF_OFFCODE\"='" + Officecode + "' group by \"WO_ID\" ORDER BY \"WO_ID\"desc limit 1";
                        Bo_id = objcon.get_value(Qry);
                        if ((Bo_id ?? "").Length != 0 && Bo_id == "15")
                        {
                            return false;
                        }

                        Qry = "SELECT \"RWO_SLNO\" from \"TBLREPAIRERWORKORDER\"  inner join \"TBLREPAIRWORKAWARDDETAILS\" ";
                        Qry += " on \"RWO_SLNO\" = \"RWAD_WO_SLNO\" inner join \"TBLREPAIRSENTDETAILS\" on \"RWAD_WA_ID\"= \"RSD_RWAO_ID\" ";
                        Qry += " inner join \"TBLTCMASTER\" on \"RSD_TC_CODE\"=\"TC_CODE\" where \"RWO_EST_ID\" = '"
                            + EstID.Split('~').GetValue(0).ToString() + "' AND \"TC_CURRENT_LOCATION\"='3' ";
                        string Rwoslno = objcon.get_value(Qry);
                        if ((Rwoslno ?? "").Length == 0)
                        {
                            return true;
                        }

                        Qry = " SELECT Max(\"RSD_ID\") FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" =:sTCcode ";
                        Qry += " AND \"RSD_DELIVARY_DATE\" IS NOT NULL and \"RSD_PROCESS_FLAG\" = 1";
                        NpgsqlCommand.Parameters.AddWithValue("sTCcode", Convert.ToInt32(sDataReferenceId));
                        string flag = objcon.get_value(Qry, NpgsqlCommand);
                        if (flag.Length == 0)
                        {
                            return true;
                        }
                        else
                        {
                            Qry = "SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" ='"
                                + EstID.Split('~').GetValue(0).ToString() + "'  AND \"WO_BO_ID\" =:sBOId1 ";
                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", sDataReferenceId);
                            NpgsqlCommand.Parameters.AddWithValue("sBOId1", Convert.ToInt32(sBOId));
                            sResult = objcon.get_value(Qry, NpgsqlCommand);
                            if (sResult.Length != 0)
                            {
                                Qry = " SELECT Max(\"RSD_ID\") FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" =:sTCcode ";
                                Qry += " AND \"RSD_DELIVARY_DATE\" IS NOT NULL and \"RSD_PROCESS_FLAG\" = 1";
                                NpgsqlCommand.Parameters.AddWithValue("sTCcode", Convert.ToInt32(sDataReferenceId));
                                flag = objcon.get_value(Qry, NpgsqlCommand);
                                if (flag.Length != 0)
                                {
                                    return false;
                                }
                                else
                                    return true;
                            }
                            return false;
                        }
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        public string GetDataReferenceId(string sBOId, string sWFOId = "")
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                DataTable dt = new DataTable();
                if (sBOId != "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" =:sBOId AND \"WO_APPROVE_STATUS\" ='0' AND \"WO_DATA_ID\" IS NOT NULL";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sResult += Convert.ToString(dt.Rows[i]["WO_DATA_ID"]) + ",";
                    }
                }
                else if (sWFOId != "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT  \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFOId ";
                    NpgsqlCommand.Parameters.AddWithValue("sWFOId", Convert.ToInt32(sWFOId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                }
                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public void SendSMStoRole(clsApproval objApproval, string sPreviousBoId)
        {
            try
            {
                if (Convert.ToString(ConfigurationManager.AppSettings["SendSMS"]).ToUpper().Equals("ON"))
                {
                    string strQry = string.Empty;
                    DataTable dt = new DataTable();
                    string sOfficeCode = string.Empty;
                    Division = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
                    SubDivision = Convert.ToInt32(ConfigurationManager.AppSettings["SubDiv_code"]);
                    Section = Convert.ToInt32(ConfigurationManager.AppSettings["Section_code"]);
                    clsCommunication objcomm = new clsCommunication();
                    string sFullName = string.Empty;
                    string sMobileNo = string.Empty;


                    // Create another table to store office code length with respect to the roles , take the length and substring the office code
                    if (objApproval.sRoleId == "1" || objApproval.sRoleId == "26")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, SubDivision);
                    }
                    if (objApproval.sRoleId == "2" || objApproval.sRoleId == "3" || objApproval.sRoleId == "5" || objApproval.sRoleId == "6" || objApproval.sRoleId == "7" || objApproval.sRoleId == "21" || objApproval.sRoleId == "24")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, Division);
                    }

                    if (objApproval.sRoleId == "4")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode;
                    }

                    if (sOfficeCode != "")
                    {
                        if (objApproval.sRoleId == "12")
                        {
                            sOfficeCode = null;
                        }
                        if (sOfficeCode == null)
                        {
                            strQry = "SELECT \"US_FULL_NAME\",\"US_MOBILE_NO\",\"US_EMAIL\" FROM \"TBLUSER\" WHERE \"US_ROLE_ID\" IN (" + objApproval.sRoleId + ") AND \"US_OFFICE_CODE\" is null";
                        }
                        else
                        {
                            strQry = "SELECT \"US_FULL_NAME\",\"US_MOBILE_NO\",\"US_EMAIL\" FROM \"TBLUSER\" WHERE \"US_ROLE_ID\" IN (" + objApproval.sRoleId + ") AND \"US_OFFICE_CODE\" ='" + sOfficeCode + "'";
                        }
                        dt = objcon.FetchDataTable(strQry);

                        string sSMSText = string.Empty;
                        string sQry = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sFullName = Convert.ToString(dt.Rows[i]["US_FULL_NAME"]);
                            sMobileNo = Convert.ToString(dt.Rows[i]["US_MOBILE_NO"]);


                            //Failure Entry
                            if (objApproval.sBOId == "9")
                            {
                                if (objApproval.sApproveStatus == "3")
                                {
                                    strQry = "SELECT \"BO_NAME\" || '~' || \"RO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLBUSINESSOBJECT\",\"TBLUSER\",\"TBLROLES\" WHERE \"WO_BO_ID\"=\"BO_ID\" AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" ";
                                    strQry += " AND \"WO_ID\" ='" + Convert.ToInt64(objApproval.sWFObjectId) + "' AND \"WO_BO_ID\" ='" + Convert.ToInt32(objApproval.sBOId) + "'";
                                    string sResult = objcon.get_value(strQry);

                                    objcomm.sSMSkey = "SMStoReject";
                                    objcomm = objcomm.GetsmsTempalte(objcomm);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        if (sResult != "")
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(0).ToString(),
                                    objApproval.sDataReferenceId, sResult.Split('~').GetValue(1).ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    objcomm.sSMSkey = "SMStoFailureCreate";
                                    objcomm = objcomm.GetsmsTempalte(objcomm);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate,
                                    objApproval.sDataReferenceId);
                                    }
                                }

                            }

                            //Work Order Entry
                            if (objApproval.sBOId == "11")
                            {

                                if (sPreviousBoId == "9")
                                {
                                    objcomm.sSMSkey = "SMStoFailure";
                                    objcomm = objcomm.GetsmsTempalte(objcomm);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate,
                                          objApproval.sDataReferenceId);
                                    }
                                }
                                else
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                    sQry += " \"DF_ID\" =:sDataReferenceId AND \"WO_DATA_ID\"=:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                    sQry += " \"WO_ID\"=:sWFObjectId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" ";
                                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                    string sResult = objcon.get_value(sQry, NpgsqlCommand);
                                    if (objApproval.sRoleId == "2")
                                    {
                                        objcomm.sSMSkey = "SMStoWorkOrderCreate";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (objApproval.sApproveStatus == "3")
                                        {
                                            NpgsqlCommand = new NpgsqlCommand();
                                            sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                            sQry += " \"DF_ID\" =:sDataReferenceId AND \"WO_DATA_ID\" =:sDataReferenceId12 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                            sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=:sWFObjectId";
                                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId12", objApproval.sDataReferenceId);
                                            NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                            NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                            sResult = objcon.get_value(sQry, NpgsqlCommand);

                                            objcomm.sSMSkey = "SMStoReject";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                if (sResult != "")
                                                {
                                                    sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            objcomm.sSMSkey = "SMStoWorkOrderApprover";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                if (sResult != "")
                                                {
                                                    sSMSText = String.Format(objcomm.sSMSTemplate,
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString(), sResult.Split('~').GetValue(2).ToString());
                                                }
                                            }
                                        }

                                    }
                                }

                            }

                            // Indent
                            if (objApproval.sBOId == "12")
                            {
                                if (sPreviousBoId == "11")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_SLNO\"=:sNewRecordId AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\"";
                                    NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoWorkOrder";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["DT_NAME"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_ID\" =:sDataReferenceId AND \"WO_DATA_ID\"=:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=:sWFObjectId";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                        string sResult = objcon.get_value(sQry, NpgsqlCommand);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_SLNO\" =:sDataReferenceId AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" ";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));

                                        dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoIndentCreate";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {

                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                              Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["DT_NAME"]));
                                            }
                                        }
                                    }


                                }
                            }

                            // Invoice Creation Approval
                            if (objApproval.sBOId == "29")
                            {
                                if (sPreviousBoId == "12")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\",\"TI_INDENT_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLINDENT\" WHERE ";
                                    sQry += " \"TI_ID\"=:sNewRecordId AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"WO_SLNO\"=\"TI_WO_SLNO\"";
                                    NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoIndent";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]));
                                        }
                                    }
                                }
                            }

                            // Invoice Creation
                            if (objApproval.sBOId == "13")
                            {
                                if (sPreviousBoId == "29")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\",\"TI_INDENT_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLINDENT\" WHERE ";
                                    sQry += " \"TI_ID\"=:sRecordId AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" ";
                                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoIndent";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]));
                                        }
                                    }
                                }

                            }

                            //Decommission
                            if (objApproval.sBOId == "14")
                            {
                                if (sPreviousBoId == "13")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"TI_INDENT_NO\",\"IN_INV_NO\" FROM \"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"IN_TI_NO\"=\"TI_ID\" AND \"TI_ID\" =:sDataReferenceId";
                                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoInvoice";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoInvoice"]),
                                           Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]), Convert.ToString(dt.Rows[0]["IN_INV_NO"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" =:sDataReferenceId AND \"WO_DATA_ID\" =:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\" = \"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =:sWFObjectId";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                        string sResult = objcon.get_value(sQry, NpgsqlCommand);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\" FROM \"TBLDTCFAILURE\" WHERE  \"DF_DTC_CODE\" =:sDataReferenceId ";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId);

                                        dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoDecommCreate";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoDecommCreate"]),
                                               Convert.ToString(dt.Rows[0]["DF_DTC_CODE"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]));
                                            }
                                        }
                                    }
                                }
                            }

                            // RI Acknoldgement
                            if (objApproval.sBOId == "15")
                            {
                                if (sPreviousBoId == "14")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"IN_INV_NO\",\"TR_RI_NO\" FROM \"TBLDTCINVOICE\",\"TBLTCREPLACE\" WHERE \"TR_IN_NO\"=\"IN_NO\" AND \"TR_ID\"=:sNewRecordId";
                                    NpgsqlCommand.Parameters.AddWithValue("sNewRecordId", Convert.ToInt32(objApproval.sNewRecordId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoDecomm";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoDecomm"]),
                                           Convert.ToString(dt.Rows[0]["IN_INV_NO"]), Convert.ToString(dt.Rows[0]["TR_RI_NO"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" =:sDataReferenceId AND \"WO_DATA_ID\" =:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =:sWFObjectId";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                        string sResult = objcon.get_value(sQry, NpgsqlCommand);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    //   sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                        sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =:sRecordId";
                                        NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                                        dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoRICreate";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoRICreate"]),
                                               Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]));
                                            }
                                        }
                                    }

                                }
                            }


                            // Completion Report
                            if (objApproval.sBOId == "26")
                            {
                                if (sPreviousBoId == "15")
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                    sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =:sRecordId";
                                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));
                                    dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                    if (dt.Rows.Count > 0)
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objApproval.sOfficeCode.Substring(0, Division)));

                                        string sStoreName = objcon.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_OFF_CODE\" =:sOfficeCode", NpgsqlCommand);

                                        objcomm.sSMSkey = "SMStoRI";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoRI"]),
                                           Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]), sStoreName);
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();

                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" =:sDataReferenceId AND \"WO_DATA_ID\" =:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =:sWFObjectId";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                        NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                        string sResult = objcon.get_value(sQry, NpgsqlCommand);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\",\"TD_TC_NO\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                        sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =:sRecordId";
                                        NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objApproval.sRecordId));

                                        dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                                        if (dt.Rows.Count > 0)
                                        {
                                            NpgsqlCommand = new NpgsqlCommand();
                                            NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objApproval.sOfficeCode.Substring(0, Division)));
                                            string sStoreName = objcon.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_OFF_CODE\" =:sOfficeCode", NpgsqlCommand);

                                            objcomm.sSMSkey = "SMStoCRCreate";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoCRCreate"]),
                                               Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]), Convert.ToString(dt.Rows[0]["TD_TC_NO"]));
                                            }
                                        }
                                    }

                                }
                            }

                            //Estimation
                            if (objApproval.sBOId == "45")
                            {

                                if (sPreviousBoId == "9")
                                {
                                    objcomm.sSMSkey = "SMStoFailure";
                                    objcomm = objcomm.GetsmsTempalte(objcomm);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate, objApproval.sDataReferenceId);
                                    }
                                }
                                else
                                {
                                    NpgsqlCommand = new NpgsqlCommand();

                                    sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                    sQry += " \"DF_ID\" =:sDataReferenceId AND \"WO_DATA_ID\"=:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                    //sQry += " \"WO_NEXT_ROLE\" ='" + objApproval.sRoleId + "' AND \"WO_ID\"='" + objApproval.sWFObjectId + "' AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" ";
                                    sQry += " \"WO_ID\"=:sWFObjectId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" ";
                                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                    NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                    string sResult = objcon.get_value(sQry, NpgsqlCommand);

                                    if (objApproval.sRoleId == "2")
                                    {

                                        objcomm.sSMSkey = "SMStoEstimationCreate";
                                        objcomm = objcomm.GetsmsTempalte(objcomm);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            if (sResult != "")
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                        // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoEstimationCreate"]),
                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (objApproval.sApproveStatus == "3")
                                        {
                                            NpgsqlCommand = new NpgsqlCommand();
                                            sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                            sQry += " \"DF_ID\" =:sDataReferenceId AND \"WO_DATA_ID\" =:sDataReferenceId1 AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                            sQry += " \"WO_NEXT_ROLE\" =:sRoleId AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=:sWFObjectId";
                                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));
                                            NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId1", objApproval.sDataReferenceId);
                                            NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objApproval.sRoleId));
                                            NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                                            sResult = objcon.get_value(sQry, NpgsqlCommand);

                                            objcomm.sSMSkey = "SMStoReject";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                if (sResult != "")
                                                {
                                                    sSMSText = String.Format(objcomm.sSMSTemplate,
                                                sResult.Split('~').GetValue(3).ToString(),
                                            sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            objcomm.sSMSkey = "SMStoEstimateApprover";
                                            objcomm = objcomm.GetsmsTempalte(objcomm);
                                            if ((objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null) && (sResult ?? "").Length > 0)
                                            {
                                                if (sResult!="")
                                                {
                                                    sSMSText = String.Format(objcomm.sSMSTemplate,
                                               // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoEstimateApprover"]),
                                               sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString(), sResult.Split('~').GetValue(2).ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (sSMSText == "")
                        {

                        }
                        else
                        {
                            if (objcomm.sSMSTemplateID != null && objcomm.sSMSTemplateID != "")
                            {
                                objcomm.DumpSms(sMobileNo, sSMSText, objcomm.sSMSTemplateID, "WEB");
                            }
                        }
                        //objCommunication.sendSMS(sSMSText, sMobileNo, objApproval.sRoleId);
                    }
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }



        static string GetPageContent(string FullUri)
        {
            HttpWebRequest Request;
            StreamReader ResponseReader;
            Request = ((HttpWebRequest)(WebRequest.Create(FullUri)));
            ResponseReader = new StreamReader(Request.GetResponse().GetResponseStream());
            return ResponseReader.ReadToEnd();
        }
        public void SendSMStoRole1(clsApproval objApproval, string sPreviousBoId, DataBseConnection objDatabse)
        {
            try
            {
                if (Convert.ToString(ConfigurationManager.AppSettings["SendSMS"]).ToUpper().Equals("ON"))
                {
                    string strQry = string.Empty;
                    DataTable dt = new DataTable();
                    string sOfficeCode = string.Empty;
                    Division = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
                    SubDivision = Convert.ToInt32(ConfigurationManager.AppSettings["SubDiv_code"]);
                    Section = Convert.ToInt32(ConfigurationManager.AppSettings["Section_code"]);
                    clsCommunication objcomm = new clsCommunication();

                    string sFullName = string.Empty;
                    string sMobileNo = string.Empty;


                    // Create another table to store office code length with respect to the roles , take the length and substring the office code
                    if (objApproval.sRoleId == "1" || objApproval.sRoleId == "26")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, SubDivision);
                    }
                    if (objApproval.sRoleId == "2" || objApproval.sRoleId == "3" || objApproval.sRoleId == "5" || objApproval.sRoleId == "6" || objApproval.sRoleId == "7" || objApproval.sRoleId == "21" || objApproval.sRoleId == "24")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode.Substring(0, Division);
                    }

                    if (objApproval.sRoleId == "4")
                    {
                        sOfficeCode = objApproval.sRefOfficeCode;
                    }


                    if (sOfficeCode != "")
                    {
                        if (objApproval.sRoleId == "12")
                        {
                            sOfficeCode = null;
                        }
                        if (sOfficeCode == null)
                        {
                            strQry = "SELECT \"US_FULL_NAME\",\"US_MOBILE_NO\",\"US_EMAIL\" FROM \"TBLUSER\" WHERE \"US_ROLE_ID\" IN (" + objApproval.sRoleId + ") AND \"US_OFFICE_CODE\" is null";
                        }
                        else
                        {
                            strQry = "SELECT \"US_FULL_NAME\",\"US_MOBILE_NO\",\"US_EMAIL\" FROM \"TBLUSER\" WHERE \"US_ROLE_ID\" IN (" + objApproval.sRoleId + ") AND \"US_OFFICE_CODE\" ='" + sOfficeCode + "'";
                        }
                        dt = objDatabse.FetchDataTable(strQry);

                        string sSMSText = string.Empty;
                        string sQry = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Columns.Contains("US_FULL_NAME"))
                                {
                                    sFullName = Convert.ToString(dt.Rows[i]["US_FULL_NAME"]);
                                }
                                else
                                {
                                    sFullName = "";
                                }
                                if (dt.Columns.Contains("US_MOBILE_NO"))
                                {
                                    sMobileNo = Convert.ToString(dt.Rows[i]["US_MOBILE_NO"]);
                                }
                                else
                                {
                                    sMobileNo = "";
                                }
                            }
                            else
                            {

                            }

                            //Failure Entry
                            if (objApproval.sBOId == "9")
                            {
                                if (objApproval.sApproveStatus == "3")
                                {

                                    strQry = "SELECT \"BO_NAME\" || '~' || \"RO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLBUSINESSOBJECT\",\"TBLUSER\",\"TBLROLES\" WHERE \"WO_BO_ID\"=\"BO_ID\" AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" ";
                                    strQry += " AND \"WO_ID\" ='" + Convert.ToInt64(objApproval.sWFObjectId) + "' AND \"WO_BO_ID\" ='" + Convert.ToInt32(objApproval.sBOId) + "'";
                                    string sResult = objDatabse.get_value(strQry);

                                    objcomm.sSMSkey = "SMStoReject";
                                    objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(0).ToString(),
                                    objApproval.sDataReferenceId, sResult.Split('~').GetValue(1).ToString());
                                    }
                                }
                                else
                                {
                                    objcomm.sSMSkey = "SMStoFailureCreate";
                                    objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate,
                                    objApproval.sDataReferenceId);
                                    }
                                }

                            }

                            //Work Order Entry
                            if (objApproval.sBOId == "11")
                            {

                                if (sPreviousBoId == "9")
                                {
                                    objcomm.sSMSkey = "SMStoFailure";
                                    objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate, objApproval.sDataReferenceId);
                                    }
                                }
                                else
                                {
                                    NpgsqlCommand = new NpgsqlCommand();
                                    sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                    //sQry += " \"DF_ID\" ="+ Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DATA_ID\"='"+ objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                    sQry += " \"DF_ID\" =" + Convert.ToInt64((objApproval.sDataReferenceId != "") ? objApproval.sDataReferenceId : null) + " AND \"WO_DATA_ID\"='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                    sQry += " \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" ";

                                    string sResult = objDatabse.get_value(sQry);

                                    if (objApproval.sRoleId == "2")
                                    {
                                        objcomm.sSMSkey = "SMStoWorkOrderCreate";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (objApproval.sApproveStatus == "3")
                                        {
                                            sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                            sQry += " \"DF_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DATA_ID\" ='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                            sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + "";

                                            sResult = objDatabse.get_value(sQry);

                                            objcomm.sSMSkey = "SMStoReject";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                        else
                                        {
                                            objcomm.sSMSkey = "SMStoWorkOrderApprover";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString(), sResult.Split('~').GetValue(2).ToString());

                                            }
                                        }

                                    }
                                }

                            }

                            // Indent
                            if (objApproval.sBOId == "12")
                            {
                                if (sPreviousBoId == "11")
                                {
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_SLNO\"=" + Convert.ToInt64(objApproval.sNewRecordId) + " AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\"";
                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoWorkOrder";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["DT_NAME"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DATA_ID\"='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + "";
                                        string sResult = objDatabse.get_value(sQry);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                        }
                                    }
                                    else
                                    {
                                        NpgsqlCommand = new NpgsqlCommand();
                                        sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\" WHERE \"WO_SLNO\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" ";
                                        NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", Convert.ToInt64(objApproval.sDataReferenceId));

                                        dt = objDatabse.FetchDataTable(sQry);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoIndentCreate";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                              Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["DT_NAME"]));
                                            }
                                        }
                                    }


                                }
                            }

                            // Invoice Creation Approval
                            if (objApproval.sBOId == "29")
                            {
                                if (sPreviousBoId == "12")
                                {
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\",\"TI_INDENT_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLINDENT\" WHERE ";
                                    sQry += " \"TI_ID\"=" + Convert.ToInt32(objApproval.sNewRecordId) + " AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"WO_SLNO\"=\"TI_WO_SLNO\"";
                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoIndent";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]));
                                        }
                                    }
                                }
                            }

                            // Invoice Creation
                            if (objApproval.sBOId == "13")
                            {
                                if (sPreviousBoId == "29")
                                {
                                    sQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"WO_NO\",\"TI_INDENT_NO\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLINDENT\" WHERE ";
                                    sQry += " \"TI_ID\"=" + Convert.ToInt64(objApproval.sRecordId) + " AND \"WO_DF_ID\"=\"DF_ID\" AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" ";
                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoIndent";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                          Convert.ToString(dt.Rows[0]["DT_CODE"]), Convert.ToString(dt.Rows[0]["WO_NO"]), Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]));
                                        }
                                    }
                                }

                            }

                            //Decommission
                            if (objApproval.sBOId == "14")
                            {
                                if (sPreviousBoId == "13")
                                {
                                    sQry = "SELECT \"TI_INDENT_NO\",\"IN_INV_NO\" FROM \"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"IN_TI_NO\"=\"TI_ID\" AND \"TI_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + "";
                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoInvoice";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoInvoice"]),
                                           Convert.ToString(dt.Rows[0]["TI_INDENT_NO"]), Convert.ToString(dt.Rows[0]["IN_INV_NO"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {
                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" ='" + objApproval.sDataReferenceId + "' AND \"WO_DATA_ID\" ='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\" = \"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";

                                        string sResult = objDatabse.get_value(sQry);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                        }
                                    }
                                    else
                                    {
                                        sQry = "SELECT \"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\" FROM \"TBLDTCFAILURE\" WHERE  \"DF_DTC_CODE\" ='" + objApproval.sDataReferenceId + "' ";

                                        dt = objDatabse.FetchDataTable(sQry);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoDecommCreate";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoDecommCreate"]),
                                               Convert.ToString(dt.Rows[0]["DF_DTC_CODE"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]));
                                            }
                                        }
                                    }
                                }
                            }

                            // RI Acknoldgement
                            if (objApproval.sBOId == "15")
                            {
                                if (sPreviousBoId == "14")
                                {

                                    sQry = "SELECT \"IN_INV_NO\",\"TR_RI_NO\" FROM \"TBLDTCINVOICE\",\"TBLTCREPLACE\" WHERE \"TR_IN_NO\"=\"IN_NO\" AND \"TR_ID\"=" + Convert.ToInt32(objApproval.sNewRecordId) + "";

                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {
                                        objcomm.sSMSkey = "SMStoDecomm";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoDecomm"]),
                                           Convert.ToString(dt.Rows[0]["IN_INV_NO"]), Convert.ToString(dt.Rows[0]["TR_RI_NO"]));
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {

                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" ='" + objApproval.sDataReferenceId + "' AND \"WO_DATA_ID\" ='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";

                                        string sResult = objDatabse.get_value(sQry);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    //   sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                        }
                                    }
                                    else
                                    {

                                        sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                        sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =" + Convert.ToInt64(objApproval.sRecordId) + "";

                                        dt = objDatabse.FetchDataTable(sQry);
                                        if (dt.Rows.Count > 0)
                                        {
                                            objcomm.sSMSkey = "SMStoRICreate";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //  sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoRICreate"]),
                                               Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]));
                                            }
                                        }
                                    }

                                }
                            }


                            // Completion Report
                            if (objApproval.sBOId == "26")
                            {
                                if (sPreviousBoId == "15")
                                {

                                    sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                    sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =" + Convert.ToInt64(objApproval.sRecordId) + "";

                                    dt = objDatabse.FetchDataTable(sQry);
                                    if (dt.Rows.Count > 0)
                                    {

                                        string sStoreName = objDatabse.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_OFF_CODE\" =" + Convert.ToInt32(objApproval.sOfficeCode.Substring(0, Division)) + "");

                                        objcomm.sSMSkey = "SMStoRI";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                           // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoRI"]),
                                           Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]), sStoreName);
                                        }
                                    }

                                }
                                else
                                {
                                    if (objApproval.sApproveStatus == "3")
                                    {

                                        sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                        sQry += " \"DF_DTC_CODE\" ='" + objApproval.sDataReferenceId + "' AND \"WO_DATA_ID\" ='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                        sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + "";

                                        string sResult = objDatabse.get_value(sQry);

                                        objcomm.sSMSkey = "SMStoReject";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate, sResult.Split('~').GetValue(3).ToString(),
                                                    // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]), sResult.Split('~').GetValue(3).ToString(),
                                                    sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                        }
                                    }
                                    else
                                    {

                                        sQry = "SELECT \"TR_RI_NO\",\"DF_EQUIPMENT_ID\",\"TD_TC_NO\" FROM \"TBLTCREPLACE\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                                        sQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =" + Convert.ToInt64(objApproval.sRecordId) + "";


                                        dt = objDatabse.FetchDataTable(sQry);
                                        if (dt.Rows.Count > 0)
                                        {

                                            string sStoreName = objDatabse.get_value("SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_OFF_CODE\" =" + Convert.ToInt32(objApproval.sOfficeCode.Substring(0, Division)) + "");

                                            objcomm.sSMSkey = "SMStoCRCreate";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                               //sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoCRCreate"]),
                                               Convert.ToString(dt.Rows[0]["TR_RI_NO"]), Convert.ToString(dt.Rows[0]["DF_EQUIPMENT_ID"]), Convert.ToString(dt.Rows[0]["TD_TC_NO"]));
                                            }
                                        }
                                    }

                                }
                            }

                            //Estimation
                            if (objApproval.sBOId == "45")
                            {

                                if (sPreviousBoId == "9")
                                {
                                    objcomm.sSMSkey = "SMStoFailure";
                                    objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                    if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                    {
                                        sSMSText = String.Format(objcomm.sSMSTemplate, objApproval.sDataReferenceId);
                                    }

                                }
                                else
                                {

                                    sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                    sQry += " \"DF_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DATA_ID\"='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";

                                    sQry += " \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" ";

                                    string sResult = objDatabse.get_value(sQry);

                                    if (objApproval.sRoleId == "2")
                                    {

                                        objcomm.sSMSkey = "SMStoEstimationCreate";
                                        objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                        if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                        {
                                            sSMSText = String.Format(objcomm.sSMSTemplate,
                                        sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (objApproval.sApproveStatus == "3")
                                        {
                                            NpgsqlCommand = new NpgsqlCommand();
                                            sQry = "SELECT \"DF_DTC_CODE\" || '~' || \"DT_NAME\" || '~' || \"RO_NAME\" || '~' || \"BO_NAME\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLDTCMAST\",\"TBLUSER\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                                            sQry += " \"DF_ID\" =" + Convert.ToInt64(objApproval.sDataReferenceId) + " AND \"WO_DATA_ID\" ='" + objApproval.sDataReferenceId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\" AND ";
                                            sQry += " \"WO_NEXT_ROLE\" =" + Convert.ToInt32(objApproval.sRoleId) + " AND \"WO_CR_BY\"=\"US_ID\" AND \"RO_ID\"=\"US_ROLE_ID\" AND \"WO_BO_ID\"=\"BO_ID\" AND \"WO_ID\"=" + Convert.ToInt64(objApproval.sWFObjectId) + "";

                                            sResult = objDatabse.get_value(sQry);

                                            objcomm.sSMSkey = "SMStoReject";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                                // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoReject"]),
                                                sResult.Split('~').GetValue(3).ToString(),
                                            sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }
                                        }
                                        else
                                        {
                                            objcomm.sSMSkey = "SMStoEstimateApprover";
                                            objcomm = objcomm.GetsmsTempalte1(objcomm, objDatabse);
                                            if (objcomm.sSMSTemplate != "" && objcomm.sSMSTemplate != null)
                                            {
                                                sSMSText = String.Format(objcomm.sSMSTemplate,
                                           // sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoEstimateApprover"]),
                                           sResult.Split('~').GetValue(0).ToString(), sResult.Split('~').GetValue(1).ToString(), sResult.Split('~').GetValue(2).ToString());
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        if (sSMSText == "")
                        {

                        }
                        else
                        {
                            if (objcomm.sSMSTemplateID != null && objcomm.sSMSTemplateID != "")
                            {
                                objcomm.DumpSms1(sMobileNo, sSMSText, objcomm.sSMSTemplateID, "WEB", objDatabse);
                            }
                        }
                        //objCommunication.sendSMS(sSMSText, sMobileNo, objApproval.sRoleId);
                    }
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                // throw ex;
            }
        }


        /// <summary>
        /// Check for Duplicate Approval
        /// </summary>
        /// <param name="sBOId"></param>
        /// <param name="sRoleId"></param>
        /// <param name="sFormName"></param>
        /// <returns></returns>
        public bool CheckDuplicateApprove(clsApproval objApproval)
        {
            try
            {

                string strQry = string.Empty;
                string sApproveResult = string.Empty;

                if (objApproval.sWFAutoId != null)
                {
                    if (objApproval.sWFAutoId == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId AND \"WO_APPROVE_STATUS\" <>0";
                        NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sApproveResult = objcon.get_value(strQry, NpgsqlCommand);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WOA_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_ID\" AS TEXT) =CAST(:sWFAutoId AS TEXT) AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL";
                        NpgsqlCommand.Parameters.AddWithValue("sWFAutoId", objApproval.sWFAutoId);
                        sApproveResult = objcon.get_value(strQry, NpgsqlCommand);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return true;
            }
        }

        public bool CheckDuplicateApprove1(clsApproval objApproval, DataBseConnection objDatabse)
        {
            try
            {

                string strQry = string.Empty;
                string sApproveResult = string.Empty;

                if (objApproval.sWFAutoId != null)
                {
                    if (objApproval.sWFAutoId == "0")
                    {
                        // NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =" + Convert.ToInt64(objApproval.sWFObjectId) + " AND \"WO_APPROVE_STATUS\" <>0";
                        //  NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt64(objApproval.sWFObjectId));
                        sApproveResult = objDatabse.get_value(strQry);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"WOA_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_ID\" AS TEXT) =CAST('" + objApproval.sWFAutoId + "' AS TEXT) AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL";
                        // NpgsqlCommand.Parameters.AddWithValue("sWFAutoId", objApproval.sWFAutoId);
                        sApproveResult = objDatabse.get_value(strQry);
                        if (sApproveResult != "")
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return true;
            }
        }

        public string Checkestimationduplicate(string sWFInitialId, string roleId)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (roleId == "4")
                {
                    strQry = "select \"WO_ID\" from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='45' and \"WO_NEXT_ROLE\"='26' and \"WO_APPROVE_STATUS\"='0' and \"WO_DATA_ID\"=:sWFInitialId";
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", sWFInitialId);
                    return objcon.get_value(strQry, NpgsqlCommand);
                }
                return "0";
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public string Checkeprmtstimationduplicate(string sWFInitialId, string roleId)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (roleId == "4")
                {
                    strQry = "select \"WO_ID\" from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='62' and \"WO_NEXT_ROLE\"='26' and \"WO_APPROVE_STATUS\"='0' and \"WO_DATA_ID\"=:sWFInitialId";
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", sWFInitialId);
                    return objcon.get_value(strQry, NpgsqlCommand);
                }
                return "0";
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public string Checkrepairerestimationduplicate(string sWFInitialId, string roleId)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (roleId == "4")
                {
                    if (sWFInitialId != null)
                    {
                        strQry = "select \"WO_ID\" from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='71' and \"WO_NEXT_ROLE\"='26' and \"WO_APPROVE_STATUS\"='0' and \"WO_DATA_ID\"=:sWFInitialId";
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", sWFInitialId);
                        return objcon.get_value(strQry, NpgsqlCommand);
                    }
                }
                return "0";
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public string sGetApprovalLevel(string sBoid, string sRoleId)
        {
            string val = string.Empty;
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (sBoid!=""&& sRoleId!="")
                {
                    strQry = "SELECT \"MAX_LEVEL\"|| '~' ||\"LEVEL\"  AS \"LEVELS\" FROM (SELECT MAX(\"WM_LEVEL\") \"MAX_LEVEL\",\"WM_BOID\" FROM \"TBLWORKFLOWMASTER\" WHERE ";
                    strQry += " \"WM_BOID\" =:sBoid GROUP BY \"WM_BOID\")A INNER JOIN (SELECT \"WM_LEVEL\" as \"LEVEL\",\"WM_BOID\"  FROM \"TBLWORKFLOWMASTER\" ";
                    strQry += " WHERE \"WM_BOID\"=:sBoid1 AND \"WM_ROLEID\"=:sRoleId)B ON A.\"WM_BOID\"=B.\"WM_BOID\"";
                    NpgsqlCommand.Parameters.AddWithValue("sBoid", Convert.ToInt32(sBoid));
                    NpgsqlCommand.Parameters.AddWithValue("sBoid1", Convert.ToInt32(sBoid));
                    NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(sRoleId));
                    val = objcon.get_value(strQry, NpgsqlCommand);
                }
                return val;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string getdataid(string sWFInitialId)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                string dataid = string.Empty;
                if(sWFInitialId==""&& sWFInitialId==null)
                {
                    return dataid;
                }
                strQry = "SELECT \"WO_DATA_ID\" from \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"=:sWFInitialId";
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(sWFInitialId));
                dataid= objcon.get_value(strQry, NpgsqlCommand);
                return dataid;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public bool CheckAlreadyExistEntryId(string sDataReferenceId, string sBOId)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" =:sDataReferenceId AND \"WO_APPROVE_STATUS\" ='0' AND \"WO_BO_ID\" =:sBOId";
                NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", sDataReferenceId);
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// this method used to get approved WFO id
        /// </summary>
        /// <param name="sDataReferenceId"></param>
        /// <param name="sBOId"></param>
        /// <returns></returns>
        public string[] GetApprovedpreview(string sDataReferenceId, string sBOId)
        {
            string[] Arr = new string[3];
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"WO_WFO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" =:sDataReferenceId ";
                strQry += " AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC";
                NpgsqlCommand.Parameters.AddWithValue("sDataReferenceId", sDataReferenceId);
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult != "")
                {
                    Arr[0] = "success";
                    Arr[1] = "0";
                    Arr[2] = sResult;
                    return Arr;
                }
                Arr[0] = "fail";
                Arr[1] = "2";
                Arr[2] = sResult;
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }
    }
}
