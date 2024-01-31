using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;

namespace IIITS.DTLMS.BL
{
    public class clsAprovalHistory
    {
        string strFormCode = "clsAprovalHistory";
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);

        public string sRecordId { get; set; }
        public string sBOId { get; set; }
        public string sDescription { get; set; }
        public string sStatus { get; set; }

        public string sDTCCode { get; set; }
        public string sDTCName { get; set; }
        public string sDTRCode { get; set; }
        public string sroletype { get; set; }
        NpgsqlCommand NpgsqlCommand;
        public DataTable LoadApprovalHistory(string sRecordId, string sBOId)
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string strQry = string.Empty;
                strQry = " SELECT \"WO_ID\", (SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"WO_CR_BY\" = \"US_ID\") INITIATOR , TO_CHAR(\"WO_CR_ON\",'DD-MON-YY HH:MI AM') WO_CR_ON, ";
                strQry += " (SELECT \"WO_USER_COMMENT\"  FROM \"TBLWORKFLOWOBJECTS\" A  WHERE A.\"WO_ID\" = B.\"WO_PREV_APPROVE_ID\") WO_USER_COMMENT FROM \"TBLWORKFLOWOBJECTS\" B  ";
                strQry += "  WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" ";
                NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(sRecordId));
                NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public clsAprovalHistory GetStatusofApproval(clsAprovalHistory objHistory)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                if (objHistory.sroletype != "2" )
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    //strQry = "SELECT \"STATUS\",\"WO_DESCRIPTION\" FROM (";
                    //strQry += " SELECT CASE WHEN \"WO_APPROVE_STATUS\" = 1 THEN 'APPROVED' ELSE 'PENDING WITH ' || (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\" = \"RO_ID\") || '-' ||";
                    //strQry += " (SELECT \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" = (SELECT \"WO_OFFICE_CODE\" FROM \"TBLWORKFLOWOBJECTS\" B WHERE A.\"WO_PREV_APPROVE_ID\" = B.\"WO_ID\" )) ";
                    //strQry += " END \"STATUS\",\"WO_DESCRIPTION\" FROM \"TBLWORKFLOWOBJECTS\" A WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC)A LIMIT 1";
                    strQry = "SELECT \"STATUS\",\"WO_DESCRIPTION\" FROM (";
                    strQry += " SELECT CASE WHEN \"WO_APPROVE_STATUS\" = 1 THEN 'APPROVED' ELSE 'PENDING WITH ' || (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\" = \"RO_ID\") || '-' ||";
                    strQry += " (SELECT case when \"WO_PREV_APPROVE_ID\" ='0' then (select \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" = \"WO_OFFICE_CODE\") else (SELECT \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" = (SELECT \"WO_OFFICE_CODE\" FROM \"TBLWORKFLOWOBJECTS\" B WHERE A.\"WO_PREV_APPROVE_ID\" = B.\"WO_ID\" )) end ) ";
                    strQry += " END \"STATUS\",\"WO_DESCRIPTION\" FROM \"TBLWORKFLOWOBJECTS\" A WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC)A LIMIT 1";

                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                    if (dt.Rows.Count > 0)
                    {
                        objHistory.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                        objHistory.sStatus = Convert.ToString(dt.Rows[0]["STATUS"]);
                    }

                }
                else
                {
                    if (objHistory.sBOId == "23" || objHistory.sBOId == "24")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        //strQry = "SELECT \"STATUS\",\"WO_DESCRIPTION\" FROM (";
                        //strQry += " SELECT CASE WHEN \"WO_APPROVE_STATUS\" = 1 THEN 'APPROVED' ELSE 'PENDING WITH ' || (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\" = \"RO_ID\") || '-' ||";
                        //strQry += " (SELECT \"SM_NAME\" FROM \"TBLSTOREOFFCODE\",\"TBLSTOREMAST\"  WHERE \"STO_SM_ID\"=\"SM_ID\" and  \"STO_OFF_CODE\"  = (SELECT \"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" B WHERE A.\"WO_PREV_APPROVE_ID\" = B.\"WO_ID\" )) ";
                        //strQry += " END \"STATUS\",\"WO_DESCRIPTION\" FROM \"TBLWORKFLOWOBJECTS\" A WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC)A LIMIT 1";

                        strQry = "SELECT \"STATUS\",\"WO_DESCRIPTION\" FROM (";
                        strQry += " SELECT CASE WHEN \"WO_APPROVE_STATUS\" = 1 THEN 'APPROVED' ELSE 'PENDING WITH ' || (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\" = \"RO_ID\") || '-' ||";
                        strQry += " (SELECT case when \"WO_PREV_APPROVE_ID\" ='0' then (select \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" = (select \"SM_CODE\" from \"TBLSTOREMAST\"";
                        if(objHistory.sBOId == "23")
                        {
                            strQry += " where \"SM_OFF_CODE\"=\"WO_OFFICE_CODE\"))";
                        }
                        else
                        {
                            strQry += " where \"SM_ID\"=\"WO_OFFICE_CODE\"))";
                        }

                        strQry +=" else (SELECT \"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE \"OFF_CODE\" = (SELECT \"WO_OFFICE_CODE\" FROM \"TBLWORKFLOWOBJECTS\" B WHERE A.\"WO_PREV_APPROVE_ID\" = B.\"WO_ID\" )) end ) ";
                        strQry += " END \"STATUS\",\"WO_DESCRIPTION\" FROM \"TBLWORKFLOWOBJECTS\" A WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC)A LIMIT 1";
                        NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                        NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objHistory.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                            objHistory.sStatus = Convert.ToString(dt.Rows[0]["STATUS"]);
                        }

                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"STATUS\",\"WO_DESCRIPTION\" FROM (";
                        strQry += " SELECT CASE WHEN \"WO_APPROVE_STATUS\" = 1 THEN 'APPROVED' ELSE 'PENDING WITH ' || (SELECT \"RO_NAME\" FROM \"TBLROLES\" WHERE \"WO_NEXT_ROLE\" = \"RO_ID\") || '-' ||";
                        strQry += " (SELECT \"SM_NAME\" FROM \"TBLSTOREOFFCODE\",\"TBLSTOREMAST\"  WHERE \"STO_SM_ID\"=\"SM_ID\" and  cast(\"STO_OFF_CODE\" as text)=substr(cast(A.\"WO_REF_OFFCODE\" as text),'1','3')) ";
                        strQry += " END \"STATUS\",\"WO_DESCRIPTION\",\"WO_REF_OFFCODE\" FROM \"TBLWORKFLOWOBJECTS\" A WHERE \"WO_RECORD_ID\" =:sRecordId AND \"WO_BO_ID\" =:sBOId ORDER BY \"WO_ID\" DESC)A LIMIT 1";
                        NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                        NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objHistory.sDescription = Convert.ToString(dt.Rows[0]["WO_DESCRIPTION"]);
                            objHistory.sStatus = Convert.ToString(dt.Rows[0]["STATUS"]);
                        }
                    }

                }

                return objHistory;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objHistory;
            }
        }

        public clsAprovalHistory GetDTCDetails(clsAprovalHistory objHistory)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                if (objHistory.sBOId == "9" || objHistory.sBOId == "10")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",(SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\"=\"DT_TC_ID\") TC_CODE ";
                    strQry += " FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"=\"DT_CODE\" AND \"WO_RECORD_ID\" =:sRecordId and \"WO_BO_ID\" =:sBOId";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "11")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    //strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" TC_CODE FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\",";
                    //strQry += " \"TBLDTCFAILURE\" WHERE \"WO_DATA_ID\"=\"DF_ID\" AND \"WO_RECORD_ID\" ='" + objHistory.sRecordId + "' and \"WO_BO_ID\" ='" + objHistory.sBOId + "' AND \"DF_DTC_CODE\"=\"DT_CODE\"";
                    strQry = " SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" TC_CODE FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\",";
                    strQry += " \"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"WO_DATA_ID\"=CAST(\"EST_FAILUREID\" AS TEXT) AND \"DF_ID\"=\"EST_FAILUREID\" ";
                    strQry += " AND \"WO_RECORD_ID\" =:sRecordId and \"WO_BO_ID\" =:sBOId AND \"DF_DTC_CODE\"=\"DT_CODE\"";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "12")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" TC_CODE FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\",";
                    strQry += " \"TBLDTCFAILURE\",\"TBLWORKORDER\" WHERE \"WO_DATA_ID\"=CAST(\"WO_SLNO\" as text) AND \"WO_RECORD_ID\" =:sRecordId ";
                    strQry += " and \"WO_BO_ID\" =:sBOId AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"DF_ID\"=\"WO_DF_ID\"";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "13")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" TC_CODE FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\",";
                    strQry += " \"TBLDTCFAILURE\",\"TBLWORKORDER\",\"TBLINDENT\" WHERE \"WO_DATA_ID\"=CAST(\"TI_ID\" as text) AND \"WO_RECORD_ID\" =:sRecordId ";
                    strQry += " and \"WO_BO_ID\" =:sBOId AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\"";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "14")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",(SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\"=\"DT_TC_ID\") TC_CODE ";
                    strQry += " FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"=\"DT_CODE\" AND \"WO_RECORD_ID\"=:sRecordId and \"WO_BO_ID\" =:sBOId";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "15")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",(SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\"=\"DT_TC_ID\") TC_CODE ";
                    strQry += " FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"=\"DT_CODE\" AND \"WO_RECORD_ID\" =:sRecordId and \"WO_BO_ID\" =:sBOId";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "26")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = " SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" TC_CODE FROM \"TBLDTCMAST\",\"TBLWORKFLOWOBJECTS\", ";
                    strQry += " \"TBLDTCFAILURE\",\"TBLDTCINVOICE\",\"TBLTCREPLACE\",\"TBLTCDRAWN\" WHERE \"WO_RECORD_ID\"=\"TR_ID\" AND \"WO_RECORD_ID\"=:sRecordId ";
                    strQry += " AND \"WO_BO_ID\" =:sBOId AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"DF_ID\"=\"TD_DF_ID\" AND \"IN_NO\"=\"TD_INV_NO\" AND \"TR_IN_NO\"=\"IN_NO\"";
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                }
                if (objHistory.sBOId == "45")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" AS \"TC_CODE\" FROM \"TBLDTCFAILURE\",\"TBLWORKFLOWOBJECTS\",";
                    strQry += " \"TBLDTCMAST\" WHERE \"WO_DATA_ID\"=CAST(\"DF_ID\" AS TEXT) AND \"DF_DTC_CODE\"=\"DT_CODE\" AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId";
                   
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                }
                if (objHistory.sBOId == "46" || objHistory.sBOId == "48")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" AS \"TC_CODE\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\",";
                    strQry += " \"TBLWORKFLOWOBJECTS\",\"TBLESTIMATIONDETAILS\",\"TBLDTCMAST\" WHERE \"DF_ID\"=\"EST_FAILUREID\" ";
                    strQry += " AND \"WO_DATA_ID\"=CAST(\"WO_SLNO\" AS TEXT) AND \"DF_ID\"=\"WO_DF_ID\" AND  \"DF_DTC_CODE\"=\"DT_CODE\" ";
                    strQry += " AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                }
                if (objHistory.sBOId == "47")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DF_EQUIPMENT_ID\" AS \"TC_CODE\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\",";
                    strQry += " \"TBLWORKFLOWOBJECTS\",\"TBLESTIMATIONDETAILS\",\"TBLRECEIVEDTR\",\"TBLDTCMAST\" WHERE \"DF_ID\"=\"EST_FAILUREID\" ";
                    strQry += " AND \"WO_DATA_ID\"=CAST(\"RD_ID\" AS TEXT) AND \"DF_ID\"=\"WO_DF_ID\" AND  \"WO_SLNO\"=\"RD_WO_SLNO\" AND ";
                    strQry += " \"DF_DTC_CODE\" =\"DT_CODE\" AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId";
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                }

                //if (objHistory.sBOId == "62")
                //{
                //    NpgsqlCommand = new NpgsqlCommand();
                //    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                //    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                //    string stc_id = objcon.get_value("SELECT \"WO_DATA_ID\" from \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId", NpgsqlCommand);
                //    NpgsqlCommand = new NpgsqlCommand();
                //    NpgsqlCommand.Parameters.AddWithValue("stc_id", Convert.ToInt32(stc_id));
                //    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                //    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                //    strQry = " SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"TC_CODE\" FROM \"TBLWORKFLOWOBJECTS\", \"TBLDTCMAST\",\"TBLTCMASTER\" WHERE \"TC_ID\"=:stc_id  AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId ";

                //}
                // modified by santhosh
                if (objHistory.sBOId == "62" || objHistory.sBOId == "63" || objHistory.sBOId == "65" || objHistory.sBOId == "66" || objHistory.sBOId == "67")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));
                    string stc_id = objcon.get_value("SELECT \"WO_DATA_ID\" from \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId", NpgsqlCommand);
                    if (objHistory.sBOId == "63")
                    {
                        stc_id = objcon.get_value("SELECT DISTINCT \"WO_DATA_ID\" from \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"= 62 and \"WO_RECORD_ID\" ='" + stc_id + "';", NpgsqlCommand);
                        //NpgsqlCommand.Parameters.AddWithValue("stc_id", Convert.ToString(stc_idofPWo));
                    }
                    NpgsqlCommand = new NpgsqlCommand();

                    NpgsqlCommand.Parameters.AddWithValue("stc_id", Convert.ToString(stc_id));

                    NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(objHistory.sBOId));
                    NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt64(objHistory.sRecordId));

                    //strQry = " SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"TC_CODE\" FROM \"TBLWORKFLOWOBJECTS\", \"TBLDTCMAST\",\"TBLTCMASTER\" WHERE \"TC_ID\"=:stc_id  AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId ";
                    strQry = "SELECT DISTINCT \"DT_NAME\",\"DT_CODE\",\"DT_TC_ID\" AS \"TC_CODE\" FROM \"TBLWORKFLOWOBJECTS\", \"TBLDTCMAST\" WHERE \"DT_CODE\"=:stc_id  AND \"WO_BO_ID\"=:sBOId AND \"WO_RECORD_ID\"=:sRecordId ;";
                }

                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);                

                if (dt.Rows.Count > 0)
                {
                    objHistory.sDTCCode = Convert.ToString(dt.Rows[0]["DT_CODE"]);
                    objHistory.sDTCName = Convert.ToString(dt.Rows[0]["DT_NAME"]);
                    objHistory.sDTRCode = Convert.ToString(dt.Rows[0]["TC_CODE"]);
                }
                return objHistory;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objHistory;
            }
        }

        public DataTable LoadApprovalFullHistory(string sRecordId, string sBOId, string sDataId, string sAutoID)
        {
            DataTable dt = new DataTable();
            DataTable dtFinalDetails = new DataTable();
            try
            {
                string sQry = string.Empty;
                string sFetch = string.Empty;

                if(sDataId == "0")
                {
                  //  NpgsqlCommand = new NpgsqlCommand();
                    sQry = "SELECT \"WO_RECORD_ID\"||'~' ||\"WO_BO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" ='"+ Convert.ToInt64(sAutoID) + "'";
                  //  NpgsqlCommand.Parameters.AddWithValue("sAutoID", Convert.ToInt32(sAutoID));
                    string sValue = objcon.get_value(sQry);
                    if(sValue.Length > 0)
                    {
                        sRecordId = sValue.Split('~').GetValue(0).ToString();
                        sBOId = sValue.Split('~').GetValue(1).ToString();
                    }
                }

                if (sRecordId == "" || sRecordId == null|| sRecordId== string.Empty)
                {
                    sRecordId = "0";
                }
                if (sBOId == "" || sBOId == null || sBOId == string.Empty)
                {
                    sBOId = "0";
                }
                LOOP:
              //  NpgsqlCommand = new NpgsqlCommand();
                sQry = " SELECT \"WO_ID\", (SELECT \"US_FULL_NAME\" FROM \"TBLUSER\" WHERE \"WO_CR_BY\" = \"US_ID\") \"INITIATOR\" , ";
                sQry += " (SELECT \"BO_NAME\" FROM \"TBLBUSINESSOBJECT\" WHERE \"BO_ID\" = \"WO_BO_ID\" ) \"BONAME\" ,  ";
                sQry += " TO_CHAR(\"WO_CR_ON\",'DD-MON-YY HH:MI AM') WO_CR_ON, (SELECT \"WO_USER_COMMENT\"  FROM \"TBLWORKFLOWOBJECTS\" A  ";
                sQry += " WHERE A.\"WO_ID\" = B.\"WO_PREV_APPROVE_ID\") WO_USER_COMMENT FROM \"TBLWORKFLOWOBJECTS\" B  ";
                sQry += " WHERE \"WO_RECORD_ID\" ='"+ Convert.ToInt64(sRecordId) + "' AND \"WO_BO_ID\" ='"+ Convert.ToInt32(sBOId) + "' ORDER BY \"WO_ID\" ";

               
              //  NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt32(sRecordId));
              //  NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                dt = objcon.FetchDataTable(sQry);

                if(dtFinalDetails.Rows.Count == 0)
                {
                    dtFinalDetails = objcon.FetchDataTable(sQry);
                }
                else
                {                  
                    dtFinalDetails.Merge(dt);
                }

                //  NpgsqlCommand = new NpgsqlCommand();
                if (sRecordId == "" || sRecordId == null|| sRecordId==string.Empty)
                {
                    sRecordId = "0";
                }

                if (sBOId == "" || sBOId == null || sBOId == string.Empty)
                {
                    sBOId = "0";
                }
                sFetch = "SELECT MIN(\"WO_ID\")  FROM \"TBLWORKFLOWOBJECTS\" WHERE  \"WO_RECORD_ID\" ='"+ Convert.ToInt64(sRecordId) + "' AND \"WO_BO_ID\" ='"+ Convert.ToInt32(sBOId) + "'";
               
               // NpgsqlCommand.Parameters.AddWithValue("sRecordId", Convert.ToInt32(sRecordId));
               // NpgsqlCommand.Parameters.AddWithValue("sBOId", Convert.ToInt32(sBOId));
                string sMinWoId = objcon.get_value(sFetch);
                if (sMinWoId == "")
                {
                    
                    sFetch = "SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" = '0' ";
                }
                else
                {
                    //NpgsqlCommand = new NpgsqlCommand();
                    sFetch = "SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" ='"+ Convert.ToInt64(sMinWoId) + "' ";
                  //  NpgsqlCommand.Parameters.AddWithValue("sMinWoId", Convert.ToInt32(sMinWoId));

                }

                string sPrevAppId = objcon.get_value(sFetch);

                if(sPrevAppId.Length == 0)
                {
                    DataView dv = dtFinalDetails.DefaultView;

                    if (dv.Count>0)
                    {
                        dv.Sort = "WO_ID ASC";
                        dtFinalDetails = dv.ToTable();
                    }
                    

                    return dtFinalDetails;
                }
              //  NpgsqlCommand = new NpgsqlCommand();
                sFetch = "SELECT \"WO_BO_ID\" || '~' || \"WO_RECORD_ID\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE  \"WO_ID\" ='"+ Convert.ToInt64(sPrevAppId) + "'";
              //  NpgsqlCommand.Parameters.AddWithValue("sPrevAppId", Convert.ToInt32(sPrevAppId));
                string sdetails = objcon.get_value(sFetch);
                if(sdetails.Length > 0)
                {
                    sBOId = sdetails.Split('~').GetValue(0).ToString();
                    sRecordId = sdetails.Split('~').GetValue(1).ToString();
                }

                goto LOOP;                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
    }
}
