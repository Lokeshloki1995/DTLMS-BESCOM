using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace IIITS.DTLMS.BL
{
    public class clsDashboard
    {
        string strFormCode = "clsDashboard";
        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
        public string sOfficeCode { get; set; }
        public string sBOId { get; set; }
        public string sRoleId { get; set; }
        public string RoleType { get; set; }
        public string Capacity { get; set; }
        public string sUserId { get; set; }
        public string sBoType { get; set; }
        /// <summary>
        /// this method used to fetch estimation pending counts
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetEstimationPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                if (objDashoard.sOfficeCode != "" && objDashoard.sOfficeCode != null)
                {
                    strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" WHERE \"ESTIMATION\" IS NULL ";
                    strQry += " AND LENGTH(\"FAILURE\")>= 1 AND \"WO_BO_ID\" = '9' AND ";
                    strQry += "  CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%'";
                }
                else
                {
                    strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" WHERE \"ESTIMATION\" IS NULL ";
                    strQry += " AND LENGTH(\"FAILURE\")>= 1 AND \"WO_BO_ID\" = '9'";
                }
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        ///// <summary>
        ///// Get count of pending Work Order, Indent,Invoice(it includes Failure Entry,Enhancement, New DTC counts)
        ///// </summary>
        ///// <param name="objDashoard"></param>
        ///// <returns></returns>
        public string GetFailurePendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT WFOTABLE+AUTOTABLE AS FAILURE_PENDING FROM ";
                strQry += " (SELECT COUNT(*) \"WFOTABLE\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                strQry += " \"WO_REF_OFFCODE\" LIKE '" + objDashoard.sOfficeCode + "%' ";
                strQry += " AND \"WO_BO_ID\" IN (" + objDashoard.sBOId + ") AND \"WO_APPROVE_STATUS\" ='0') A,";
                strQry += " (SELECT COUNT(*) \"AUTOTABLE\" FROM \"TBLWO_OBJECT_AUTO\",\"TBLBO_FLOW_MASTER\" ";
                strQry += " WHERE \"WOA_REF_OFFCODE\" LIKE '" + objDashoard.sOfficeCode + "%' AND ";
                strQry += " \"WOA_INITIAL_ACTION_ID\" IS NULL AND \"BFM_ID\"=\"WOA_BFM_ID\" ";
                strQry += " AND \"BFM_NEXT_BO_ID\" IN (" + objDashoard.sBOId + ")) B ";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        ///// <summary>
        ///// Get count of pending Work Order
        ///// </summary>
        ///// <param name="objDashoard"></param>
        ///// <returns></returns>
        public string GetWOPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" ";
                strQry += " WHERE \"DF_ID\"=\"EST_FAILUREID\"  AND \"WO_DATA_ID\"=\"DF_DTC_CODE\" AND \"EST_FAIL_TYPE\"='2' ";
                strQry += " AND \"DF_REPLACE_FLAG\"='0' AND \"WORKORDER\"  IS NULL AND LENGTH(\"ESTIMATION\")>= 0  ";
                strQry += " AND \"WO_BO_ID\"<>10 AND  CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%'";
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        ///// <summary>
        ///// Get count of pending Indent
        ///// </summary>
        ///// <param name="objDashoard"></param>
        ///// <returns></returns>
        public string GetIndentPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" A,\"TBLDTCFAILURE\",\"TBLWORKFLOWOBJECTS\" B,\"TBLESTIMATIONDETAILS\" ";
                strQry += " WHERE \"FAILURE\" = CAST(\"WO_ID\" AS TEXT) AND \"WO_RECORD_ID\"=\"DF_ID\" AND \"DF_ID\"=\"EST_FAILUREID\" ";
                strQry += " AND \"EST_FAIL_TYPE\"='2' AND B.\"WO_BO_ID\"='9'  AND \"INDENT\"  IS NULL AND \"WORKORDER\" IS NOT NULL ";
                strQry += " AND A.\"WO_BO_ID\"<>10 AND CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%'";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        ///// <summary>
        ///// Get count of pending Invoice
        ///// </summary>
        ///// <param name="objDashoard"></param>
        ///// <returns></returns>
        public string GetInvoicePendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" WHERE \"INVOICE\" IS NULL ";
                strQry += " AND \"INDENT\" IS NOT NULL AND \"WO_BO_ID\"<>10 AND CAST(\"OFFCODE\" AS TEXT)";
                strQry += " LIKE '" + objDashoard.sOfficeCode + "%'";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// this method used to get decommissioning penind record counts
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetDecommissionPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" WHERE \"INVOICE\" IS NOT NULL ";
                strQry += " AND \"DECOMMISION\" IS NULL AND \"WO_BO_ID\"<>10 AND CAST(\"OFFCODE\" AS TEXT) ";
                strQry += " LIKE '" + objDashoard.sOfficeCode + "%'";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        ///// <summary>
        ///// Get count of pending RI
        ///// </summary>
        ///// <param name="objDashoard"></param>
        ///// <returns></returns>
        public string GetRIPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT COUNT(*) FROM \"WORKFLOWSTATUSDUMMY\" WHERE (\"CRREPORT\" IS NULL)  AND ";
                strQry += " (\"INVOICE\" IS NOT NULL AND \"DECOMMISION\" IS NOT NULL) AND \"WO_BO_ID\"<>10 ";
                strQry += " AND CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%'";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// this method used to get total failure pending counts
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetTotalPendingCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "WITH WORKFLOWSTATUS as (SELECT * FROM \"WORKFLOWSTATUSDUMMY\" ) ";
                strQry += " SELECT sum(COALESCE(\"TOTAL_PENDING\",0)-COALESCE(\"CR_PENDING\",0) -COALESCE(\"DECOMM_PENDING\",0)) FROM ";
                strQry += " (SELECT COUNT(*)AS \"TOTAL_PENDING\" , \"OFFCODE\" FROM ";
                strQry += " WORKFLOWSTATUS,\"VIEW_MINORFAILURE_PENDING\"  WHERE  \"DT_CODE\" = \"WO_DATA_ID\" ";
                strQry += " AND \"CRREPORT\" IS NULL  AND CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%' ";
                strQry += " AND \"WO_BO_ID\"<>10 GROUP BY \"OFFCODE\")A ";
                strQry += "  left JOIN (SELECT COUNT(*) AS \"CR_PENDING\",  \"OFFCODE\" FROM WORKFLOWSTATUS ";
                strQry += " WHERE(\"CRREPORT\" IS NULL)  AND (\"INVOICE\" IS NOT NULL AND \"DECOMMISION\" IS NOT NULL) AND ";
                strQry += " CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%' AND \"WO_BO_ID\"<>10 GROUP BY \"OFFCODE\")B ";
                strQry += " ON A.\"OFFCODE\" = B.\"OFFCODE\" LEFT JOIN (SELECT COUNT(*)AS \"DECOMM_PENDING\" , \"OFFCODE\" FROM ";
                strQry += " WORKFLOWSTATUS WHERE(\"CRREPORT\" IS NULL AND \"DECOMMISION\" IS NULL ) AND(\"INVOICE\" IS NOT NULL ) ";
                strQry += " AND CAST(\"OFFCODE\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%' AND \"WO_BO_ID\"<>10 ";
                strQry += " GROUP BY \"OFFCODE\")C ON A.\"OFFCODE\"= C.\"OFFCODE\"";

                string result = ObjCon.get_value(strQry);

                return result;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetFaultyTCField(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT count(\"TC_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='2' and \"TC_STATUS\"='3' ";
                strQry += " AND cast(\"TC_LOCATION_ID\" as text) like '" + objDashoard.sOfficeCode + "%'";
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// get faulty Dtr's in store
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetFaultyTCStore(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                #region count for only failure transaction(\"
                //strQry = "SELECT \"TC_CODE\", \"TC_CAPACITY\",\"TC_SLNO\",TO_CHARTC_MANF_DATE\",'DD-MON-YYYY') AS \"TC_MANF_DATE\" FROM ";
                //strQry += " \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='1' AND \"TC_STATUS\"='3' AND \"TC_CODE\" in ";
                //strQry += " (SELECT \"DF_EQUIPMENT_ID\" FROM \"TBLDTCFAILURE\" WHERE cast(\"DF_LOC_CODE\" as text) like '" + sOfficeCode + "%')";
                #endregion

                if (objDashoard.sOfficeCode != "" && objDashoard.sOfficeCode != null)
                {
                    strQry = "SELECT COUNT(\"TC_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='1' AND \"TC_STATUS\"='3' ";
                    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashoard.sOfficeCode) + "'";
                }
                else
                {
                    strQry = "SELECT COUNT(\"TC_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='1' AND \"TC_STATUS\"='3' ";
                }

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// get Faulty Dtr's at repair
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetFaultyTCRepair(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                if (objDashoard.sOfficeCode != "" && objDashoard.sOfficeCode != null)
                {
                    strQry = "SELECT COUNT(\"TC_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"= '3' AND \"TC_STATUS\"='3'";
                    strQry += "   AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashoard.sOfficeCode) + "'";
                }
                else
                {
                    strQry = "SELECT COUNT(\"TC_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"= '3' AND \"TC_STATUS\"='3' ";
                }


                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// get total faulty dtrs count
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetTotalFaultyTC(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;
                if (objDashoard.sOfficeCode == "" || objDashoard.sOfficeCode == null)
                {
                    strQry = "SELECT \"FIELDCOUNT\"+\"STORECOUNT\"+\"REPAIRCOUNT\" FROM ";
                    strQry += " (SELECT COUNT(\"TC_ID\") \"FIELDCOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='2' AND ";
                    strQry += " \"TC_STATUS\"='3' AND \"TC_CODE\"<>0  ";
                    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%') A,";
                    strQry += " (SELECT COUNT(\"TC_ID\") \"STORECOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='1' AND ";
                    strQry += " \"TC_STATUS\"='3' AND \"TC_CODE\"<>0 ) B,";
                    strQry += " (SELECT COUNT(\"TC_ID\") \"REPAIRCOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='3' AND ";
                    strQry += " \"TC_STATUS\"='3' AND \"TC_CODE\"<>0 ) C";
                }
                else
                {
                    strQry = "SELECT \"FIELDCOUNT\"+\"STORECOUNT\"+\"REPAIRCOUNT\" FROM ";
                    strQry += " (SELECT COUNT(*) \"FIELDCOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='2' AND ";
                    strQry += " \"TC_STATUS\"='3' AND \"TC_CODE\"<>0  ";
                    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%') A,";
                    strQry += " (SELECT COUNT(*) \"STORECOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='1' ";
                    strQry += " AND \"TC_STATUS\"='3' AND \"TC_CODE\"<>0 ";
                    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objDashoard.sOfficeCode) + "%') B,";
                    strQry += " (SELECT COUNT(*) \"REPAIRCOUNT\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='3' AND \"TC_STATUS\"='3' ";
                    strQry += "   AND \"TC_CODE\"<>0 ";
                    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objDashoard.sOfficeCode) + "%') C";
                }
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// this method used to get the pending workflow records
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetPendingWorkflow(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"WFOTABLE\"+\"AUTOTABLE\" AS TOTAL_PENDING FROM ";
                strQry += " (SELECT COUNT(*) \"WFOTABLE\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%'  ";
                strQry += " AND \"WO_APPROVE_STATUS\" ='0' AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "') A, ";
                strQry += "  (SELECT COUNT(*) \"AUTOTABLE\" FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%' ";
                strQry += " AND \"WOA_INITIAL_ACTION_ID\" IS NULL AND \"WOA_ROLE_ID\" ='" + objDashoard.sRoleId + "' ) B ";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// get the approved worrkflow records
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetApprovedWorkflow(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = " SELECT \"APPROVED\"+\"APPROVED_AUTO\" FROM ";
                strQry += "(SELECT COUNT(*) \"APPROVED\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%'  ";
                strQry += " AND \"WO_APPROVE_STATUS\" IN ('1','2') AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "') A,";
                strQry += " (SELECT COUNT(*) \"APPROVED_AUTO\"  FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_REF_OFFCODE\" AS TEXT) ";
                strQry += "   LIKE '" + objDashoard.sOfficeCode + "%'";
                strQry += " AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL AND \"WOA_ROLE_ID\" ='" + objDashoard.sRoleId + "') B";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// get the rejected workflow records
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetRejectedWorkflow(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT COUNT(*) \"REJECTED\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%'  ";
                strQry += " AND \"WO_APPROVE_STATUS\" ='3' AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "'";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// get total workflow records
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetTotalWorkflow(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"TOTAL_PENDING\"+\"APPROVED\"+\"REJECTED\" FROM ";
                strQry += " (SELECT A.\"WFOTABLE\"+B.\"AUTOTABLE\" AS \"TOTAL_PENDING\" FROM ";
                strQry += " (SELECT COUNT(*) \"WFOTABLE\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "   LIKE '" + objDashoard.sOfficeCode + "%'  ";
                strQry += " AND \"WO_APPROVE_STATUS\" ='0' AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "') A,";
                strQry += " (SELECT COUNT(*) \"AUTOTABLE\" FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%' ";
                strQry += " AND \"WOA_INITIAL_ACTION_ID\" IS NULL AND \"WOA_ROLE_ID\" ='" + objDashoard.sRoleId + "' ) B ) X,";
                strQry += " (SELECT \"APPROVED\"+\"APPROVED_AUTO\" AS \"APPROVED\" FROM ";
                strQry += "(SELECT COUNT(*) \"APPROVED\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%'  ";
                strQry += " AND \"WO_APPROVE_STATUS\" IN ('1','2') AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "') A,";
                strQry += " (SELECT COUNT(*) \"APPROVED_AUTO\"  FROM \"TBLWO_OBJECT_AUTO\" WHERE CAST(\"WOA_REF_OFFCODE\" AS TEXT) ";
                strQry += "     LIKE '" + objDashoard.sOfficeCode + "%'";
                strQry += " AND \"WOA_INITIAL_ACTION_ID\" IS NOT NULL AND \"WOA_ROLE_ID\"='" + objDashoard.sRoleId + "') B) Y,";
                strQry += " (SELECT COUNT(*) AS \"REJECTED\"  FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_REF_OFFCODE\" AS TEXT) ";
                strQry += "    LIKE '" + objDashoard.sOfficeCode + "%' ";
                strQry += " AND \"WO_APPROVE_STATUS\" ='3' AND \"WO_NEXT_ROLE\" ='" + objDashoard.sRoleId + "') Z";

                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// this methid used to load bargraph
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadBarGraph(string sOfficeCode)
        {

            DataTable dtBarGraph = new DataTable();
            try
            {
                string strQry = string.Empty;

                string previousYear = ObjCon.get_value("SELECT to_char(date_trunc('YEAR', now() - '1 year'::interval), 'yyyy')");

                string presentYear = ObjCon.get_value("SELECT to_char(date_trunc('YEAR', now()- '0 year' ::interval), 'yyyy')");

                strQry = " SELECT COALESCE(\"PRESENTYEAR\",TO_CHAR(NOW(),'YYYY')) \"PRESENTYEAR\",\"MONTHS\" AS \"PRESENTMONTH\", ";
                strQry += " COALESCE(\"PRESENTCOUNT\",0) \"PRESENTCOUNT\",COALESCE(\"PREVIOUSYEAR\", ";
                strQry += "    to_char(date_trunc('YEAR', CURRENT_DATE) - INTERVAL '1 year','yyyy')) ";
                strQry += " \"PREVIOUSYEAR\",  \"MONTHS\" AS \"PREVIOUSMONTH\", COALESCE(\"PREVIOUSCOUNT\",0) \"PREVIOUSCOUNT\",";
                strQry += " '' as \"TESTYEAR\", '' as \"TESTMONTH\",'0' as \"TESTCOUNT\" FROM ";
                strQry += " (SELECT TO_CHAR(generate_series(timestamp without time zone '" + previousYear + "-04-01', ";
                strQry += " timestamp without time zone '" + presentYear + "-03-01', '1 Month'), 'MON') \"MONTHS\", ";
                strQry += " TO_CHAR(generate_series(timestamp without time ";
                strQry += " zone '" + previousYear + "-04-01', timestamp without time zone '" + presentYear + "-03-01','1 Month'), ";
                strQry += " 'MM') \"MON\", TO_CHAR(generate_series(timestamp without time  zone '" + previousYear + "-04-01', ";
                strQry += " timestamp without time zone '" + presentYear + "-03-01','1 Month'),'YYYYMM') \"YEARMONTH\")C LEFT JOIN ";
                strQry += " (SELECT TO_CHAR(\"DF_DATE\",'YYYY') AS \"PRESENTYEAR\",TO_CHAR(\"DF_DATE\",'MON') AS \"PRESENTMONTH\",  ";
                strQry += " COUNT(\"DF_DTC_CODE\") AS \"PRESENTCOUNT\" FROM \"TBLDTCFAILURE\" WHERE TO_CHAR(\"DF_DATE\",'YYYY') = ";
                strQry += " TO_CHAR(NOW(),'YYYY') AND CAST(\"DF_LOC_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' AND ";
                strQry += " \"DF_STATUS_FLAG\" IN (1,4) GROUP BY TO_CHAR(\"DF_DATE\",'MON'), ";
                strQry += " TO_CHAR(\"DF_DATE\",'YYYY'))A ON C.\"MONTHS\" = A.\"PRESENTMONTH\" LEFT JOIN (SELECT TO_CHAR(\"DF_DATE\",'YYYY') ";
                strQry += " AS \"PREVIOUSYEAR\",TO_CHAR(\"DF_DATE\",'MON') AS \"PREVIOUSMONTH\", COUNT(\"DF_DTC_CODE\")AS \"PREVIOUSCOUNT\" ";
                strQry += " FROM \"TBLDTCFAILURE\" WHERE TO_CHAR(\"DF_DATE\",'YYYY') = ";
                strQry += " to_char(date_trunc('YEAR', CURRENT_DATE) - INTERVAL '1 year','yyyy') ";
                strQry += " AND CAST(\"DF_LOC_CODE\"  AS TEXT) LIKE '" + sOfficeCode + "%' AND \"DF_STATUS_FLAG\" IN (1,4) ";
                strQry += " GROUP BY TO_CHAR(\"DF_DATE\",'MON'),TO_CHAR(\"DF_DATE\",'YYYY'))B ON ";
                strQry += " C.\"MONTHS\" = B.\"PREVIOUSMONTH\"  ORDER BY \"YEARMONTH\"";
                dtBarGraph = ObjCon.FetchDataTable(strQry);
                return dtBarGraph;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtBarGraph;
            }
        }

        /// <summary>
        /// this method used to get failure pending details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadFailurePendingDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT distinct \"DT_CODE\",\"DT_NAME\",(select substring(\"OFF_NAME\",position(':'  ";
                strQry += " in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\",\"TRANS_REF_OFF_CODE\" AS \"OM_CODE\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,";
                strQry += " length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)= ";
                strQry += " SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\",";
                strQry += "(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
                strQry += " from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) ";
                strQry += " as \"DIVSION\",\"DF_ID\",TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ";
                strQry += " ||' SINCE '||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as  \"FL_STATUS\" ";
                strQry += " from \"TBLPENDINGTRANSACTION\" inner join \"TBLROLES\" on \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" ";
                strQry += " inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\" left join  \"TBLDTCFAILURE\" ";
                strQry += " on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 where \"TRANS_BO_ID\" ";
                strQry += "NOT IN (15,26,10,71,72,75,76,77,78,79) AND CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' ";
                strQry += " and \"TRANS_NEXT_ROLE_ID\"<>0 ";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get the failure approval pending records 
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadFailureApprovalPendingDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            string strQry = string.Empty;
            strQry = "SELECT \"DT_CODE\",\"DT_NAME\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
            strQry += " from \"VIEW_ALL_OFFICES\" where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\", ";
            strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
            strQry += "  from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) ";
            strQry += " as \"SUBDIVSION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from ";
            strQry += "    \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\"";
            strQry += ",'PENDING WITH ' ||\"RO_NAME\" ||' SINCE '||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' ";
            strQry += "    as \"FL_STATUS\" from \"TBLPENDINGTRANSACTION\"";
            strQry += "  INNER JOIN \"TBLDTCMAST\" ON \"DT_CODE\"=\"TRANS_DTC_CODE\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" ";
            strQry += "    WHERE \"TRANS_BO_ID\"=9  AND CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%'";

            dt = ObjCon.FetchDataTable(strQry);
            return dt;
        }
        /// <summary>
        /// this method used to get DTC failure abstract records
        /// </summary>
        /// <param name="objDashboard"></param>
        /// <returns></returns>
        public DataTable LoadDTCFailureAbstract(clsDashboard objDashboard)
        {
            string strQry = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                int Length = 0;
                Length = objDashboard.sOfficeCode.Length + 1;

                strQry = " SELECT \"TC_CAPACITY\", \"DF_LOC_CODE\", \"SECTION\", SUM(\"FAILURECOUNTOFYEAR\") \"FAILURECOUNTOFYEAR\", ";
                strQry += " SUM(\"CURRENTQUARTER\") \"CURRENTQUARTER\", SUM(\"CURRENTMONTH\") \"CURRENTMONTH\", ";
                strQry += " SUM(\"PREVIOUSMONTH\") \"PREVIOUSMONTH\" FROM (SELECT \"TC_CAPACITY\", ";
                strQry += " SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + Length + ") \"DF_LOC_CODE\", (SELECT \"OFF_NAME\" AS \"OFFICENAME\" ";
                strQry += " FROM \"VIEW_OFFICES\" WHERE SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + Length + ") = ";
                strQry += " CAST(\"OFF_CODE\" AS TEXT)) AS \"SECTION\", COUNT(*) AS \"FAILURECOUNTOFYEAR\", ";
                strQry += " SUM(CASE WHEN TO_CHAR(\"DF_DATE\",'MONTH') IN  (SELECT \"MONTH\" FROM (SELECT TO_CHAR(\"MONTH\",'MONTH') \"MONTH\" ";
                strQry += " , CAST(DATE_PART('QUARTER', \"MONTH\")AS TEXT) \"QUARTER\" FROM (SELECT CURRENT_DATE + (INTERVAL '1' MONTH * ";
                strQry += " GENERATE_SERIES(0,11)) \"MONTH\")A)B WHERE \"QUARTER\" = (SELECT  TO_CHAR(CURRENT_DATE,'Q') \"CURRENTQUARTER\")) ";
                strQry += " THEN 1 ELSE 0 END ) \"CURRENTQUARTER\",  SUM(CASE WHEN TO_CHAR(\"DF_DATE\",'MONTH') = TO_CHAR(CURRENT_DATE,'MONTH') ";
                strQry += " THEN 1 ELSE 0 END) \"CURRENTMONTH\",SUM(CASE WHEN TO_CHAR(\"DF_DATE\",'MMYYYY') = (SELECT to_char(\"MONTH\",'MMYYYY')";
                strQry += " FROM(SELECT DATE_TRUNC('month', current_date - interval '1' month) \"MONTH\")A) THEN 1 ELSE 0 END) \"PREVIOUSMONTH\" ";
                strQry += " FROM \"TBLDTCFAILURE\", \"TBLTCMASTER\" WHERE \"DF_EQUIPMENT_ID\" = \"TC_CODE\" AND TO_CHAR(\"DF_DATE\",'YYYY') = ";
                strQry += " (SELECT TO_CHAR(\"MONTH\",'YYYY') FROM(SELECT DATE_TRUNC('month', current_date ) \"MONTH\")A)  AND ";
                strQry += " CAST(\"DF_LOC_CODE\" AS TEXT) LIKE '" + objDashboard.sOfficeCode + "%' AND \"DF_STATUS_FLAG\" IN (1,4) ";
                if (objDashboard.Capacity != null)
                {
                    strQry += " AND \"TC_CAPACITY\" = '" + objDashboard.Capacity + "' ";
                }
                strQry += " GROUP BY \"TC_CAPACITY\", \"DF_LOC_CODE\")Z GROUP BY \"TC_CAPACITY\", ";
                strQry += " \"DF_LOC_CODE\", \"SECTION\"  ORDER BY \"TC_CAPACITY\"";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get the faulty dtr records.
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadFaultyDTRDetails(string sOfficeCode)
        {
            DataTable dtCompleteDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_CODE\",\"TM_NAME\",\"TC_SLNO\",\"TC_CAPACITY\",\"DT_CODE\",\"DT_NAME\",\"TR_RI_NO\",\"TR_RI_DATE\", ";
                strQry += " \"SM_NAME\",\"SUP_REPNAME\",\"RSM_ISSUE_DATE\", \"SUP_INSP_DATE\",\"INSP_BY\" FROM ";
                strQry += " (SELECT DISTINCT \"TC_CODE\",\"TC_LOCATION_ID\",\"TM_NAME\",\"TC_SLNO\",CAST(\"TC_CAPACITY\" AS TEXT)\"TC_CAPACITY\" FROM ";
                strQry += " \"TBLTCMASTER\",\"TBLTRANSMAKES\" TM WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"TC_STATUS\"=3 )A LEFT JOIN ";
                strQry += " (SELECT \"DT_CODE\",\"DT_NAME\",\"DT_TC_ID\" FROM \"TBLDTCMAST\", \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"=2  ";
                strQry += " AND \"TC_CODE\"=\"DT_TC_ID\" AND \"TC_CODE\"<>0)B ON B.\"DT_TC_ID\"=A.\"TC_CODE\" LEFT JOIN ";
                strQry += " (SELECT \"SM_NAME\",\"TR_RI_NO\",\"FAIL_TC_CODE\",\"TR_RI_DATE\",\"TR_ID\" FROM (SELECT DISTINCT SM.\"SM_NAME\" AS ";
                strQry += " \"SM_NAME\",\"TR_RI_NO\",\"FAIL_TC_CODE\",TO_CHAR(\"TR_RI_DATE\",'DD-MON-YYYY') \"TR_RI_DATE\",\"TR_ID\" FROM ";
                strQry += " \"VIEWFAILTCCODE\" FT,\"TBLTCMASTER\",\"TBLSTOREMAST\" SM  WHERE \"TC_CURRENT_LOCATION\"<>2 AND  \"TC_CODE\" = ";
                strQry += " \"FAIL_TC_CODE\" AND \"TC_STORE_ID\"=\"SM_ID\" )A INNER JOIN (SELECT  MAX(\"TR_ID\") \"TR_IDD\",\"FAIL_TC_CODE\" AS ";
                strQry += " \"FAIL_TC_CODE1\"  FROM \"VIEWFAILTCCODE\" FT,\"TBLTCMASTER\",\"TBLSTOREMAST\" SM  WHERE \"TC_CURRENT_LOCATION\"<>2 ";
                strQry += " AND \"TC_CODE\"=\"FAIL_TC_CODE\" AND  \"TC_STORE_ID\" =\"SM_ID\" GROUP BY \"FAIL_TC_CODE\" )B ON \"TR_IDD\"=\"TR_ID\" )C ";
                strQry += " ON A.\"TC_CODE\"=C.\"FAIL_TC_CODE\"  LEFT JOIN  (SELECT DISTINCT(CASE WHEN \"RSM_SUPREP_TYPE\"='2' THEN ";
                strQry += " (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN \"RSM_SUPREP_TYPE\"='1' ";
                strQry += " THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) \"SUP_REPNAME\", ";
                strQry += " \"RSD_TC_CODE\" ,TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') \"RSM_ISSUE_DATE\", \"RSM_ID\",\"RSD_RSM_ID\" FROM ";
                strQry += " \"TBLREPAIRSENTDETAILS\",\"TBLREPAIRSENTMASTER\",\"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"=3 AND \"RSM_ID\"= ";
                strQry += " \"RSD_RSM_ID\" AND \"TC_CODE\"=\"RSD_TC_CODE\" AND \"RSD_ID\" IN (SELECT \"RSD_ID\" FROM (SELECT DISTINCT ";
                strQry += " \"RSD_TC_CODE\", MAX(\"RSD_ID\") \"RSD_ID\" FROM \"TBLREPAIRSENTDETAILS\",\"TBLREPAIRSENTMASTER\",\"TBLTCMASTER\" ";
                strQry += " WHERE \"TC_CURRENT_LOCATION\"=3 AND \"RSM_ID\" =\"RSD_RSM_ID\" AND \"TC_CODE\"=\"RSD_TC_CODE\" GROUP BY ";
                strQry += " \"RSD_TC_CODE\")Z))D ON A.\"TC_CODE\"=D.\"RSD_TC_CODE\" LEFT JOIN  (SELECT TO_CHAR(\"IND_INSP_DATE\",'DD-MON-YYYY') ";
                strQry += " AS \"SUP_INSP_DATE\",\"US_FULL_NAME\" AS \"INSP_BY\",\"RSD_TC_CODE\" FROM \"TBLREPAIRSENTDETAILS\", ";
                strQry += " \"TBLINSPECTIONDETAILS\",\"TBLUSER\", \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"=3 AND \"IND_INSP_BY\"=\"US_ID\" ";
                strQry += " AND \"IND_RSD_ID\"=\"RSD_ID\" AND \"TC_CODE\"=\"RSD_TC_CODE\")E ON E.\"RSD_TC_CODE\"=A.\"TC_CODE\" WHERE  ";
                strQry += " CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + sOfficeCode + "%' ORDER BY \"TC_CODE\"";

                dtCompleteDetails = ObjCon.FetchDataTable(strQry);
                return dtCompleteDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCompleteDetails;
            }
        }
        /// <summary>
        /// get total dtc counts
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string GetTotalDTCCount(clsDashboard objDashoard)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT COUNT(\"DT_CODE\") FROM \"TBLDTCMAST\" WHERE \"DT_PMTDECC_STATUS\" is null and ";
                strQry += " CAST(\"DT_OM_SLNO\" AS TEXT) LIKE '" + objDashoard.sOfficeCode + "%' ";
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        /// <summary>
        /// load approval pending details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <param name="sBOType"></param>
        /// <param name="sRoleType"></param>
        /// <returns></returns>
        public DataTable LoadApprovalPendingDetails(string sOfficeCode, string sBOType, string sRoleType)
        {

            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;

                Qry = " SELECT  \"TRANS_DTC_CODE\" as \"DT_CODE\",(SELECT \"DT_NAME\" from \"TBLDTCMAST\" where ";
                Qry += " \"DT_CODE\" = \"TRANS_DTC_CODE\")as \"DT_NAME\",(select substring(\"OFF_NAME\", ";
                Qry += " position(':' in \"OFF_NAME\") + 2, length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                Qry += " where \"OFF_CODE\" = \"TRANS_REF_OFF_CODE\" limit 1) as \"OMSECTION\",";
                Qry += "  \"TRANS_REF_OFF_CODE\" AS \"OM_CODE\",(SELECT \"DF_ID\" from \"TBLDTCFAILURE\" ";
                Qry += " WHERE \"DF_DTC_CODE\" = \"TRANS_DTC_CODE\" and \"DF_STATUS_FLAG\" <> '0' limit 1)\"DF_ID\", ";
                Qry += " 'PENDING WITH ' || \"RO_NAME\" || '(AT  ' || \"BO_NAME\" || ')' as  \"STATUS\" from ";
                Qry += " \"TBLPENDINGTRANSACTION\",\"TBLROLES\",\"TBLBUSINESSOBJECT\" WHERE ";
                Qry += " \"RO_ID\" = \"TRANS_NEXT_ROLE_ID\"  and \"BO_ID\" = \"TRANS_BO_ID\"";

                //FAILURE ENTRY
                if (sBOType == "9")
                {
                    Qry += " AND \"TRANS_BO_ID\"='9'";
                }

                //ENHANCEMENT ENTRY
                else if (sBOType == "10")
                {
                    Qry += " AND \"TRANS_BO_ID\"='10'";
                }

                //WORK ORDER
                else if (sBOType == "11")
                {
                    Qry += " AND \"TRANS_BO_ID\"='11'";
                }

                else if (sBOType == "74")
                {
                    Qry += " AND \"TRANS_BO_ID\"='74'";
                }

                else if (sBOType == "73")
                {
                    Qry += " AND \"TRANS_BO_ID\"='73'";
                }

                //INDENT
                else if (sBOType == "12")
                {
                    Qry += " AND \"TRANS_BO_ID\"='12'";
                }

                //INVOICE
                else if (sBOType == "13")
                {
                    Qry += " AND \"TRANS_BO_ID\"='13'";
                }

                //DECOMMISSION
                else if (sBOType == "14")
                {
                    Qry += " AND \"TRANS_BO_ID\"='14'";
                }

                //RI
                else if (sBOType == "15")
                {
                    Qry += " AND \"TRANS_BO_ID\"='15'";
                }

                //CR REPORT
                else if (sBOType == "26")
                {
                    Qry += " AND \"TRANS_BO_ID\"='26'";
                }

                if (sRoleType == "2")
                {
                    Qry += "AND ";
                    string sOffCode = clsStoreOffice.GetOfficeCode(sOfficeCode, "TRANS_REF_OFF_CODE");
                    Qry += sOffCode;
                }
                else if (sRoleType == "1")
                {
                    // before it is WO_REF_OFFCODE with like condition
                    Qry += " AND CAST(\"TRANS_REF_OFF_CODE\"  AS TEXT) LIKE '" + sOfficeCode + "%'";
                }
                else
                {
                    Qry += " AND cast(\"TRANS_REF_OFF_CODE\" as text) LIKE '%'";
                }

                Qry += " and \"TRANS_NEXT_ROLE_ID\"<>0 ORDER BY \"DF_ID\"";

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// load repairer approval pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <param name="dtrcode"></param>
        /// <param name="sRoleType"></param>
        /// <returns></returns>
        public DataTable LoadRepairerApprovalPendingDetails(string sOfficeCode, string dtrcode, string sRoleType)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                // if present in any stage (approving case)
                strQry = "select " + dtrcode + " as dtrcode,(select \"BO_NAME\" from \"TBLBUSINESSOBJECT\" where \"BO_ID\"=\"WO_BO_ID\") ";
                strQry += "  as \"BO_NAME\",(select \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\"=\"WO_NEXT_ROLE\") as roleid,";
                strQry += " (select substring(\"OFF_NAME\", position(':' in \"OFF_NAME\") + 2, length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\"";
                strQry += " where \"OFF_CODE\" = cast(SUBSTR(cast(\"WO_REF_OFFCODE\" as text),0,(select \"RO_LOC_LEVEL\" from \"TBLROLES\" ";
                strQry += " where \"RO_ID\"=\"WO_NEXT_ROLE\")+1) as int) limit 1) as \"OMSECTION\"  ";
                strQry += " from (SELECT * from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='71' and SUBSTR(\"WO_DESCRIPTION\",43)='" + dtrcode + "' ";
                strQry += " and \"WO_APPROVE_STATUS\"='0' union all SELECT * from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='72' and ";
                strQry += " SPLIT_PART(\"WO_DESCRIPTION\", ' ', 7)='" + dtrcode + "' and \"WO_APPROVE_STATUS\"='0' union all SELECT * from ";
                strQry += " \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='75' and \"WO_RECORD_ID\"<0 and \"WO_DATA_ID\" like '%" + dtrcode + "`%' ";
                strQry += " and \"WO_APPROVE_STATUS\"='0' )A";

                dt = ObjCon.FetchDataTable(strQry);

                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    //if pending in create of RWO
                    strQry = "select " + dtrcode + " as dtrcode,(select \"BO_NAME\" from \"TBLBUSINESSOBJECT\" where \"BO_ID\" = '72') as ";
                    strQry += " \"BO_NAME\",(select \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\" = (case when(select ";
                    strQry += " \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\" = \"WO_NEXT_ROLE\") is null then(select \"WM_ROLEID\" from ";
                    strQry += " \"TBLWORKFLOWMASTER\" where \"WM_LEVEL\" = '1' and \"WM_BOID\" = '72') end )  ) as roleid, ";
                    strQry += " (select substring(\"OFF_NAME\", position(':' in \"OFF_NAME\") + 2, length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                    strQry += " where \"OFF_CODE\" = cast(SUBSTR(cast(\"WO_REF_OFFCODE\" as text),0,(select \"RO_LOC_LEVEL\" from \"TBLROLES\" ";
                    strQry += " where \"RO_ID\"=(select \"WM_ROLEID\" from \"TBLWORKFLOWMASTER\" where \"WM_LEVEL\" = '1' and ";
                    strQry += " \"WM_BOID\" = '72'))+1) as int) limit 1) as \"OMSECTION\"  from ";
                    strQry += " \"TBLWORKFLOWOBJECTS\",\"TBLWO_OBJECT_AUTO\" where \"WO_BO_ID\" = '71' and ";
                    strQry += " SUBSTR(\"WO_DESCRIPTION\", 43)= '" + dtrcode + "' and \"WO_NEXT_ROLE\" = '0' and \"WO_ID\" = \"WOA_PREV_APPROVE_ID\" ";
                    strQry += "   and \"WOA_INITIAL_ACTION_ID\" is null";

                    dt = ObjCon.FetchDataTable(strQry);

                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        strQry = "SELECT \"WO_ID\" from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\" = '75' and ";
                        strQry += " \"WO_DATA_ID\" like '%" + dtrcode + "`%' limit 1";
                        string val = ObjCon.get_value(strQry);
                        if (val == "")
                        {
                            strQry = "select " + dtrcode + " as dtrcode,(select \"BO_NAME\" from \"TBLBUSINESSOBJECT\" where \"BO_ID\"='75') ";
                            strQry += "as \"BO_NAME\",(select \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\"= (case when ";
                            strQry += " (select \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\"=\"WO_NEXT_ROLE\") is null then (select \"WM_ROLEID\" ";
                            strQry += " from \"TBLWORKFLOWMASTER\" where \"WM_LEVEL\"='1' and \"WM_BOID\"='75' ) ";
                            strQry += " end )  ) as roleid,(select substring(\"OFF_NAME\", position(':' in \"OFF_NAME\") + 2, length(\"OFF_NAME\")) ";
                            strQry += " from \"VIEW_ALL_OFFICES\" where \"OFF_CODE\" = cast(SUBSTR(cast(\"WO_REF_OFFCODE\" as text),0, ";
                            strQry += " (select \"RO_LOC_LEVEL\" from \"TBLROLES\" where \"RO_ID\"=(select \"WM_ROLEID\" ";
                            strQry += "  from \"TBLWORKFLOWMASTER\" where \"WM_LEVEL\" = '1' and \"WM_BOID\" = '75'))+1) as int) limit 1) ";
                            strQry += " as \"OMSECTION\"  from \"TBLWORKFLOWOBJECTS\" where \"WO_BO_ID\"='72' and ";
                            strQry += " SPLIT_PART(\"WO_DESCRIPTION\", ' ', 7)='" + dtrcode + "' and \"WO_NEXT_ROLE\"='0' ";
                            dt = ObjCon.FetchDataTable(strQry);

                        }
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    strQry = "SELECT \"TC_STATUS\" from \"TBLTCMASTER\" where \"TC_CODE\" = '" + dtrcode + "' and \"TC_CURRENT_LOCATION\"='3' ";
                    string val = ObjCon.get_value(strQry);
                    if (val == "3")
                    {
                        strQry = "select \"RSM_ID\" from  \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\"  on \"RSM_ID\" = \"RSD_RSM_ID\" ";
                        strQry += " left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\" = \"IND_RSD_ID\" where \"RSD_TC_CODE\" = '" + dtrcode + "' ";
                        strQry += " AND \"IND_INSP_RESULT\" in ('4') ORDER BY \"RSM_ID\" desc limit 1";
                        string id = ObjCon.get_value(strQry);
                        if (id != "")
                        {
                            return dt;
                        }
                        strQry = "select " + dtrcode + " as dtrcode ,(select \"BO_NAME\" from \"TBLBUSINESSOBJECT\" where \"BO_ID\"=\"WO_BO_ID\") as ";
                        strQry += " \"BO_NAME\",(select \"RO_NAME\" from \"TBLROLES\" where \"RO_ID\"=\"WO_NEXT_ROLE\") as roleid, ";
                        strQry += " (select substring(\"OFF_NAME\", position(':' in \"OFF_NAME\") + 2, length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                        strQry += " where \"OFF_CODE\" = cast(SUBSTR(cast(\"WO_REF_OFFCODE\" as text),0,(select \"RO_LOC_LEVEL\" from \"TBLROLES\" ";
                        strQry += " where \"RO_ID\"=\"WO_NEXT_ROLE\")+1) as int) limit 1) as \"OMSECTION\" from \"TBLWORKFLOWOBJECTS\" ";
                        strQry += " WHERE \"WO_DATA_ID\"='" + dtrcode + "' and \"WO_NEXT_ROLE\"='2' and \"WO_APPROVE_STATUS\"='0' ";

                        dt = ObjCon.FetchDataTable(strQry);

                        if (dt.Rows.Count <= 0)
                        {
                            strQry = "select " + dtrcode + " as dtrcode , case when (select \"RSM_ID\" from \"TBLREPAIRSENTMASTER\" ";
                            strQry += "  inner join \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\"  ";
                            strQry += " where \"RSD_TC_CODE\"='" + dtrcode + "' limit 1) IS NULL then 'PENDING TO SEND FOR REPAIR' ";
                            strQry += " when (select \"RSM_ID\" from \"TBLREPAIRSENTMASTER\" inner join ";
                            strQry += " \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\" RIGHT join \"TBLINSPECTIONDETAILS\" ";
                            strQry += " on \"IND_RSD_ID\" =\"RSD_ID\" where \"RSD_TC_CODE\"='" + dtrcode + "'  and \"IND_RSD_ID\" IN  ";
                            strQry += " (select \"RSD_ID\" from \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" on  ";
                            strQry += " \"RSM_ID\"=\"RSD_RSM_ID\" where \"RSD_TC_CODE\"='" + dtrcode + "' ORDER BY ";
                            strQry += " \"RSD_ID\" DESC LIMIT 1 ) limit 1) is  null then 'PENDING TO INSPECT' ";
                            strQry += " WHEN  (select \"RSM_ID\" from  \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                            strQry += " on \"RSM_ID\"=\"RSD_RSM_ID\" left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" where ";
                            strQry += " \"RSD_TC_CODE\"='" + dtrcode + "' AND \"IND_INSP_RESULT\"='0' ORDER BY \"RSM_ID\" desc limit 1) ";
                            strQry += " is not null then 'PENDING TO INSPECT'  WHEN  (select \"RSM_ID\" from ";
                            strQry += " \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\" ";
                            strQry += "  left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" where \"RSD_TC_CODE\"= ";
                            strQry += " '" + dtrcode + "' AND \"IND_INSP_RESULT\" in ('1','3','4') ORDER BY ";
                            strQry += " \"RSM_ID\" desc limit 1) is not null then 'PENDING TO RECIEVE' END as \"BO_NAME\" , ";
                            strQry += " case  when (select \"RSM_ID\" from \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                            strQry += "  on \"RSM_ID\"=\"RSD_RSM_ID\"  where \"RSD_TC_CODE\"='" + dtrcode + "' limit 1) ";
                            strQry += " IS NULL then 'STORE KEEPER' when (select \"RSM_ID\" from \"TBLREPAIRSENTMASTER\" inner join ";
                            strQry += " \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\" RIGHT join \"TBLINSPECTIONDETAILS\" ";
                            strQry += " on \"IND_RSD_ID\" =\"RSD_ID\" where \"RSD_TC_CODE\"='" + dtrcode + "'  and \"IND_RSD_ID\" IN ";
                            strQry += " (select \"RSD_ID\" from \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                            strQry += " on \"RSM_ID\"=\"RSD_RSM_ID\" where \"RSD_TC_CODE\"='" + dtrcode + "' ORDER BY ";
                            strQry += " \"RSD_ID\" DESC LIMIT 1 ) limit 1) is  null then 'MT' WHEN ";
                            strQry += " (select \"RSM_ID\" from  \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                            strQry += " on \"RSM_ID\"=\"RSD_RSM_ID\" left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" ";
                            strQry += " where \"RSD_TC_CODE\"='" + dtrcode + "' AND \"IND_INSP_RESULT\"='0' ";
                            strQry += " ORDER BY \"RSM_ID\" desc limit 1) is not null then ";
                            strQry += " 'MT' WHEN  (select \"RSM_ID\" from  \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                            strQry += " on \"RSM_ID\"=\"RSD_RSM_ID\" left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" ";
                            strQry += " where \"RSD_TC_CODE\"=  '" + dtrcode + "' AND \"IND_INSP_RESULT\" in ('1','3','4') ";
                            strQry += " ORDER BY \"RSM_ID\" desc limit 1) is not null then ";
                            strQry += " 'STORE KEEPER' END as roleid,(select \"SM_NAME\" from \"TBLREPAIRSENTMASTER\" inner join ";
                            strQry += " \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\" inner join \"TBLSTOREMAST\" on ";
                            strQry += " \"SM_CODE\"=\"RSM_NEW_DIV_CODE\" where \"RSD_TC_CODE\"='" + dtrcode + "' ";
                            strQry += " ORDER BY \"RSM_ID\" desc limit 1) as \"OMSECTION\" from \"TBLREPAIRSENTMASTER\" inner join ";
                            strQry += " \"TBLREPAIRSENTDETAILS\" on \"RSM_ID\"=\"RSD_RSM_ID\"  LIMIT 1 ";
                            dt = ObjCon.FetchDataTable(strQry);
                        }

                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to load the estimation pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadEstimationPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2, ";
                strQry += " length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\",(select substring(\"OFF_NAME\", ";
                strQry += " position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\", ";
                strQry += " TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||";
                strQry += "' SINCE ' ||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" ";
                strQry += " from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\" inner join";
                strQry += " \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0  INNER JOIN ";
                strQry += " \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\"  in(45) AND ";
                strQry += " CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get the work order pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadWorkorderPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",(select substring(\"OFF_NAME\", ";
                strQry += " position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\", ";
                strQry += " \"DF_ID\",TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '";
                strQry += "  ||\"RO_NAME\" ||' SINCE '||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' ";
                strQry += " as \"DAYS_FROM_PENDING\" from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" ";
                strQry += "  on \"DT_CODE\"=\"TRANS_DTC_CODE\" left join  \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" ";
                strQry += " and  \"DF_REPLACE_FLAG\"=0 left JOIN \"TBLESTIMATIONDETAILS\" on \"DF_ID\"=\"EST_FAILUREID\" and \"EST_FAIL_TYPE\"='2'";
                strQry += "  INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\" in(11,74) and ";
                strQry += " CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get the single work order pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadSingleWorkorderPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",(select substring(\"OFF_NAME\", ";
                strQry += " position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\"";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\"";
                strQry += "  where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",\"DF_ID\", ";
                strQry += " TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||";
                strQry += " ' SINCE ' ||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as ";
                strQry += " \"DAYS_FROM_PENDING\" from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\"";
                strQry += "  inner join  \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 ";
                strQry += " INNER JOIN \"TBLESTIMATIONDETAILS\" on \"DF_ID\"=\"EST_FAILUREID\" and \"EST_FAIL_TYPE\"='1' INNER JOIN \"TBLROLES\" ";
                strQry += " ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\"  in(11,74) and ";
                strQry += " CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get receive dtr pending details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadReceiveTCPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",\"WO_NO\",to_char(\"WO_DATE\",'dd-MON-yyyy')\"WO_DATE\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)= SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",\"DF_ID\", ";
                strQry += " TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||";
                strQry += "  date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" from ";
                strQry += " \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on ";
                strQry += " \"DT_CODE\"=\"TRANS_DTC_CODE\" inner join  \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and ";
                strQry += " \"DF_REPLACE_FLAG\"=0 INNER JOIN \"TBLWORKORDER\" on \"DF_ID\"=\"WO_DF_ID\"";
                strQry += " INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\" =46 and ";
                strQry += " CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get commission pending pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadSingleComissionPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",\"WO_NO\",to_char(\"WO_DATE\",'dd-MON-yyyy')\"WO_DATE\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\"";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\",(select substring(\"OFF_NAME\", ";
                strQry += " position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
                strQry += " from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as ";
                strQry += " \"SUBDIVSION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from ";
                strQry += " \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as ";
                strQry += " \"DIVSION\",\"DF_ID\",TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')";
                strQry += " \"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||date_part('day',age(CURRENT_DATE, ";
                strQry += "  CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" from \"TBLPENDINGTRANSACTION\" ";
                strQry += "  inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\" inner join  \"TBLDTCFAILURE\" on ";
                strQry += " \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 INNER JOIN \"TBLWORKORDER\" ON ";
                strQry += "  \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\" = 47 ";
                strQry += " and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get indent pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadIndentPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",\"WO_NO\",to_char(\"WO_DATE\",'dd-MON-yyyy')\"WO_DATE\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\")";
                strQry += " as \"OMSECTION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
                strQry += " from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) ";
                strQry += " as \"SUBDIVSION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from ";
                strQry += " \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",";
                strQry += "  \"DF_ID\",TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||' ";
                strQry += " SINCE '||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" from ";
                strQry += "     \"TBLPENDINGTRANSACTION\" ";
                strQry += "  inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\" inner join  \"TBLDTCFAILURE\" on ";
                strQry += " \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 left join \"TBLWORKORDER\" ON  ";
                strQry += "    \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\"";
                strQry += "  where \"TRANS_BO_ID\" in (12,29) and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' ";
                strQry += "    order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get invoice pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadInvoicePendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",\"WO_NO\",to_char(\"WO_DATE\",'dd-MON-yyyy')\"WO_DATE\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",";
                strQry += "  \"DF_ID\",TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||date_part('day', ";
                strQry += " age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" ";
                strQry += "  from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\" inner join ";
                strQry += " \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 INNER JOIN \"TBLWORKORDER\" ";
                strQry += "  ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" where \"TRANS_BO_ID\" ='13' ";
                strQry += " and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' and \"TRANS_NEXT_ROLE_ID\"<>0 order by ";
                strQry += " CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to get decommission pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadDeCommissionPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DF_EQUIPMENT_ID\",\"WO_NO\",to_char(\"WO_DATE\",'dd-MON-yyyy')\"WO_DATE\",";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\")";
                strQry += "  as \"OMSECTION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from ";
                strQry += " \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)";
                strQry += " = SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\",(select substring(\"OFF_NAME\", ";
                strQry += " position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)= ";
                strQry += " SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",\"DF_ID\", ";
                strQry += " TO_CHAR(\"DF_DATE\",'dd-MON-yyyy')\"DF_DATE\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||";
                strQry += "  date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"DAYS_FROM_PENDING\" ";
                strQry += " from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\"";
                strQry += " inner join  \"TBLDTCFAILURE\" on \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" and  \"DF_REPLACE_FLAG\"=0 INNER JOIN ";
                strQry += " \"TBLWORKORDER\" ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLROLES\" ON \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\"";
                strQry += "  where \"TRANS_BO_ID\" ='14' and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' order by ";
                strQry += " CAST(\"DF_DATE\" AS TIMESTAMP) desc";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// this method used to load RI pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadRIPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DT_TC_ID\",\"DF_EQUIPMENT_ID\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
                strQry += " from \"VIEW_ALL_OFFICES\" where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as ";
                strQry += " \"SUBDIVSION\",(select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from ";
                strQry += " \"VIEW_ALL_OFFICES\" where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as ";
                strQry += " \"DIVSION\",CASE WHEN \"TRANS_BO_ID\"=15 THEN 'PENDING WITH '||\"RO_NAME\" ||";
                strQry += " ' SINCE ' ||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days'  END AS ";
                strQry += "  \"DAYS_FROM_PENDING\" from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\"";
                strQry += "  inner join \"TBLDTCFAILURE\" on \"TRANS_DTC_CODE\"=\"DF_DTC_CODE\" and \"DF_REPLACE_FLAG\"=0 INNER JOIN \"TBLROLES\" ";
                strQry += " ON \"TRANS_NEXT_ROLE_ID\"=\"RO_ID\" where \"TRANS_BO_ID\" in (15) and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) ";
                strQry += " LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// this method used to get CR pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadCRPendingDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"DT_CODE\",\"DT_NAME\",\"DT_TC_ID\",\"DF_EQUIPMENT_ID\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) ";
                strQry += " from \"VIEW_ALL_OFFICES\" where \"OFF_CODE\"=\"TRANS_REF_OFF_CODE\") as \"OMSECTION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,4)) as \"SUBDIVSION\", ";
                strQry += " (select substring(\"OFF_NAME\",position(':' in \"OFF_NAME\")+2,length(\"OFF_NAME\")) from \"VIEW_ALL_OFFICES\" ";
                strQry += " where cast(\"OFF_CODE\" as text)=SUBSTRING(cast(\"TRANS_REF_OFF_CODE\" as text),1,3)) as \"DIVSION\",";
                strQry += " CASE WHEN \"TRANS_BO_ID\"=26 THEN 'PENDING WITH '||\"RO_NAME\" ||";
                strQry += " ' SINCE ' ||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days'  END AS ";
                strQry += " \"CR_DAYS_FROM_PENDING\"  from \"TBLPENDINGTRANSACTION\" inner join \"TBLDTCMAST\" on \"DT_CODE\"=\"TRANS_DTC_CODE\"";
                strQry += "  inner join \"TBLDTCFAILURE\" on \"TRANS_DTC_CODE\"=\"DF_DTC_CODE\"  and \"DF_REPLACE_FLAG\"=0 INNER JOIN ";
                strQry += " \"TBLROLES\" ON \"TRANS_NEXT_ROLE_ID\"=\"RO_ID\" where \"TRANS_BO_ID\" in (26) and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) ";
                strQry += " LIKE '" + sOfficeCode + "%' order by CAST(\"DF_DATE\" AS TIMESTAMP) desc";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get failure pending records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadFailureDtrDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                if (sOfficeCode == "" || sOfficeCode == null)
                {
                    strQry = " SELECT CAST(\"TC_CODE\") AS \"TC_CODE\",\"TC_SLNO\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") AS \"OFFNAME\"  FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='2' ";
                    strQry += " AND \"TC_STATUS\" ='3'  AND \"TC_LOCATION_ID\" LIKE '" + sOfficeCode + "%' ";
                    strQry += " UNION SELECT CAST(\"TC_CODE\") AS \"TC_CODE\",\"TC_SLNO\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") \"OFFNAME\"  FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='1' ";
                    strQry += " AND \"TC_STATUS\" ='3' UNION SELECT CAST(\"TC_CODE\" AS TEXT),\"TC_SLNO\",\"TC_CAPACITY\" , ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") \"OFFNAME\"   FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='3' ";
                    strQry += " AND \"TC_STATUS\" ='3' ORDER BY \"TC_CAPACITY\"";
                }
                else
                {
                    strQry = " SELECT CAST(\"TC_CODE\") AS \"TC_CODE\",\"TC_SLNO\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") AS \"OFFNAME\"  FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='2' ";
                    strQry += " AND \"TC_STATUS\" ='3'  AND \"TC_LOCATION_ID\" LIKE '" + sOfficeCode + "%' ";
                    strQry += " UNION SELECT CAST(\"TC_CODE\") AS \"TC_CODE\",\"TC_SLNO\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") \"OFFNAME\"  FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='1' ";
                    strQry += " AND \"TC_STATUS\" ='3'  AND \"TC_LOCATION_ID\" = '" + clsStoreOffice.GetStoreID(sOfficeCode) + " ";
                    strQry += " UNION SELECT CAST(\"TC_CODE\" AS TEXT),\"TC_SLNO\",\"TC_CAPACITY\" , ";
                    strQry += " (SELECT SUBSTR(\"OFF_NAME\",INSTR(\"OFF_NAME\",':')+1) FROM \"VIEW_ALL_OFFICES\" WHERE ";
                    strQry += " \"OFF_CODE\"=\"TC_LOCATION_ID\") \"OFFNAME\"   FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='3' ";
                    strQry += " AND \"TC_STATUS\" ='3'  AND \"TC_LOCATION_ID\" = '" + clsStoreOffice.GetStoreID(sOfficeCode) + "' ";
                    strQry += " ORDER BY \"TC_CAPACITY\"";
                }

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to load the condition of Dtr's
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfTCDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;


                strQry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                strQry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" WHERE \"TC_RATING\"=\"MD_ID\" AND ";
                strQry += " \"TM_ID\"=\"TC_MAKE_ID\" AND \"TC_STATUS\" in(1,2,3) AND \"TC_CURRENT_LOCATION\" = 1 ";
                strQry += " AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying new tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfNewTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT distinct \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\", ";
                strQry += " (select \"MD_NAME\" from \"TBLMASTERDATA\" where \"MD_TYPE\"='SR' ";
                strQry += "AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" from ";
                strQry += " \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE  \"TC_STATUS\"='1' AND \"TM_ID\"=\"TC_MAKE_ID\" ";
                strQry += "AND \"TC_CURRENT_LOCATION\" = 1   AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying released good tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfREGoodTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;
                Qry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TM_ID\"=\"TC_MAKE_ID\"  AND \"MD_TYPE\"='SR' AND ";
                Qry += " \"TC_STATUS\"='5' AND \"TC_CURRENT_LOCATION\" = 1 ";
                Qry += " AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying repired good tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfRPGoodTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;
                Qry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TM_ID\"=\"TC_MAKE_ID\" AND \"MD_TYPE\"='SR' AND \"TC_STATUS\"='2' ";
                Qry += " AND \"TC_CURRENT_LOCATION\" = 1 AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying faulty tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfFaultyTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;
                Qry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += "WHERE \"TC_RATING\"=\"MD_ID\" AND \"TM_ID\"=\"TC_MAKE_ID\" AND \"MD_TYPE\"='SR' AND \"TC_STATUS\"='3' ";
                Qry += " AND \"TC_CURRENT_LOCATION\" = 1 ";
                Qry += " AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying scrap tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfScrapTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;
                Qry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TM_ID\"=\"TC_MAKE_ID\" AND \"MD_TYPE\"='SR' AND ";
                Qry += " \"TC_STATUS\" IN ('6','4') AND \"TC_CURRENT_LOCATION\" = 1  ";
                Qry += " AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying mob tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadConditionOfMobileTCDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                strQry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                strQry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TM_ID\"=\"TC_MAKE_ID\" AND \"TC_CONDITION\"='MOBILE TRANSFORMER' ";
                strQry += " AND \"MD_TYPE\"='SR' AND \"TC_STATUS\"='3' ";
                strQry += "AND \"TC_CURRENT_LOCATION\" = 1  AND \"TC_LOCATION_ID\"=" + sOfficeCode;
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get total repair good dtr's
        /// </summary>
        /// <param name="objDashoard"></param>
        /// <returns></returns>
        public string TotalRepairGoodTc(clsDashboard objDashoard)
        {
            try
            {
                string Qry = string.Empty;

                if (objDashoard.sOfficeCode == "" || objDashoard.sOfficeCode == null)
                {
                    Qry = "SELECT COUNT(\"TC_ID\") from \"TBLTCMASTER\" WHERE \"TC_STATUS\"=2 AND \"TC_CURRENT_LOCATION\" =1 ";
                }
                else
                {
                    Qry = " SELECT COUNT(\"TC_ID\") from \"TBLTCMASTER\" WHERE \"TC_STATUS\"=2 AND \"TC_CURRENT_LOCATION\" =1 ";
                    Qry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashoard.sOfficeCode) + "'";
                }


                return ObjCon.get_value(Qry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        /// <summary>
        /// This method used to get the failure dtrs records
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable TotalTcfailedview(string sOfficeCode)
        {
            DataTable dt = new DataTable();

            try
            {
                string Qry = string.Empty;

                if (sOfficeCode == "" || sOfficeCode == null)
                {
                    Qry = " SELECT \"TC_CODE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", \"TC_SLNO\", ";
                    Qry += " TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\" FROM ";
                    Qry += " \"TBLTCMASTER\" WHERE \"TC_STATUS\"=2 AND \"TC_CURRENT_LOCATION\"=1 ";
                }
                else
                {
                    Qry = "select \"TC_CODE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", \"TC_SLNO\", ";
                    Qry += " TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\" FROM ";
                    Qry += " \"TBLTCMASTER\" WHERE \"TC_STATUS\"=2 AND \"TC_CURRENT_LOCATION\"=1 AND ";
                    Qry += " \"TC_LOCATION_ID\" = '" + clsStoreOffice.GetStoreID(sOfficeCode) + "'";
                }

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// This method used to get total faulty Dtr's details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable GetTotalFaultyTCview(string sOfficeCode)
        {
            DataTable dt = new DataTable();

            try
            {
                string Qry = string.Empty;

                string storeid = clsStoreOffice.GetStoreID(sOfficeCode);
                if (storeid != null)
                {
                    Qry = " SELECT \"TC_CODE\",\"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\" ";
                    Qry += " FROM \"TBLTCMASTER\" WHERE   \"TC_STATUS\"= '3'  AND (CAST(\"TC_LOCATION_ID\" AS TEXT) ";
                    Qry += " LIKE '" + sOfficeCode + "%' or  CAST(\"TC_LOCATION_ID\" AS TEXT)='" + clsStoreOffice.GetStoreID(sOfficeCode) + "')";
                }
                else
                {
                    Qry = " SELECT \"TC_CODE\",\"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\" ";
                    Qry += " FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\"= '3'  AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + sOfficeCode + "%' ";
                }
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// This method used to get the faulty dtr's at field 
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable GetFaultyTCFieldview(string sOfficeCode)
        {
            DataTable dt = new DataTable();

            try
            {

                string Qry = string.Empty;

                Qry = "SELECT \"TC_CODE\",CAST(\"TC_CAPACITY\" as text) AS \"TC_CAPACITY\", \"TC_SLNO\", ";
                Qry += " TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') AS \"TC_MANF_DATE\" FROM \"TBLTCMASTER\" WHERE ";
                Qry += " \"TC_CURRENT_LOCATION\"='2'  and   \"TC_STATUS\"='3' AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + sOfficeCode + "%'";

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// This method used to get the details of  faulty dtr's at store
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable GetFaultyTCStoreview(string sOfficeCode)
        {
            DataTable dt = new DataTable();

            try
            {
                string strQry = string.Empty;
                if (sOfficeCode == "" || sOfficeCode == null)
                {
                    strQry = "SELECT \"TC_CODE\", \"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') AS ";
                    strQry += " \"TC_MANF_DATE\" ,(select \"DIV_NAME\" from \"TBLSTOREMAST\",\"TBLDIVISION\" where \"SM_CODE\"=\"DIV_CODE\" ";
                    strQry += " and  \"SM_ID\"=\"TC_LOCATION_ID\" ) \"DIV_NAME\" FROM \"TBLTCMASTER\" WHERE ";
                    strQry += " \"TC_CURRENT_LOCATION\" ='1' AND \"TC_STATUS\"='3' ";
                }
                else
                {
                    strQry = "SELECT \"TC_CODE\", \"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') AS ";
                    strQry += " \"TC_MANF_DATE\" ,(select \"DIV_NAME\" from \"TBLDIVISION\" where ";
                    strQry += " cast(SUBSTR(\"TC_PREV_OFFCODE\",0,4) as int4) =\"DIV_CODE\") \"DIV_NAME\" FROM ";
                    strQry += " \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" ='1' AND \"TC_STATUS\"='3' AND ";
                    strQry += " CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(sOfficeCode) + "'";
                }

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get faulty dtr's with repairer.
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable GetFaultyTCRepairview(string sOfficeCode)
        {
            DataTable dt = new DataTable();

            try
            {
                string Qry = string.Empty;
                if (sOfficeCode == "" || sOfficeCode == null)
                {
                    Qry = "SELECT \"TC_CODE\", \"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\", ";
                    Qry += " (select \"DIV_NAME\" from \"TBLDIVISION\" where cast(SUBSTR(\"TC_PREV_OFFCODE\",0,4) as int4) =\"DIV_CODE\") ";
                    Qry += " \"DIV_NAME\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='3' AND \"TC_STATUS\"='3' ";
                }
                else
                {
                    Qry = " SELECT \"TC_CODE\", \"TC_CAPACITY\",\"TC_SLNO\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\", ";
                    Qry += " (select \"DIV_NAME\" from \"TBLDIVISION\" where cast(SUBSTR(\"TC_PREV_OFFCODE\",0,4) as int4) =\"DIV_CODE\") ";
                    Qry += " \"DIV_NAME\" FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\"='3' AND \"TC_STATUS\"='3' AND ";
                    Qry += " CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(sOfficeCode) + "'";
                }

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get the Dtr's at store detials
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <param name="sWOslno"></param>
        /// <param name="sRoleType"></param>
        /// <returns></returns>
        public DataTable GetStore_TcDetails(string sOfficeCode, string sWOslno, string sRoleType)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;
                if (sRoleType != "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                Qry = "SELECT \"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",CASE WHEN \"TC_STATUS\"=1 THEN 'BRAND NEW' ";
                Qry += " WHEN \"TC_STATUS\"=2 THEN 'REPAIR GOOD' END ";
                Qry += " \"STATUS\", \"SM_NAME\" FROM \"TBLTCMASTER\",\"TBLSTOREMAST\" WHERE \"TC_LOCATION_ID\"=\"SM_ID\" ";
                Qry += " AND \"TC_LOCATION_ID\"='" + sOfficeCode + "' ";
                Qry += " AND  \"TC_CAPACITY\" IN (SELECT \"WO_NEW_CAP\" FROM \"TBLWORKORDER\" WHERE ";
                Qry += " \"WO_SLNO\"='" + sWOslno + "') AND \"TC_STATUS\" IN (1,2) AND \"TC_CURRENT_LOCATION\"=1";
                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying less than 25 capacity tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable Loadless25CapacityTCDetails(string sOfficeCode)
        {
            string strQry = string.Empty;
            try
            {
                strQry = "SELECT\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                strQry += "WHERE \"TC_RATING\"=\"MD_ID\" AND \"TC_MAKE_ID\"=\"TM_ID\"  AND \"MD_TYPE\"='SR' and ";
                strQry += " \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" < 25 ";
                strQry += "AND \"TC_LOCATION_ID\" = '" + sOfficeCode + "'";
                return ObjCon.FetchDataTable(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ObjCon.FetchDataTable(strQry);
        }

        /// <summary>
        /// For diplaying 25 to 100 capacity tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable Loadbtw25_100CapacityTCDetails(string sOfficeCode)
        {
            string Qry = string.Empty;
            try
            {
                Qry = "SELECT\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" ";
                Qry += " FROM \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += "WHERE \"TC_RATING\"=\"MD_ID\" AND \"TC_MAKE_ID\"=\"TM_ID\"  AND \"MD_TYPE\"='SR' ";
                Qry += " and \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" ";
                Qry += "BETWEEN 25 AND 100 AND \"TC_LOCATION_ID\" = '" + sOfficeCode + "'";

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ObjCon.FetchDataTable(Qry);
        }

        /// <summary>
        /// For diplaying less than 125 to 250 capacity tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable Loadbtw125_250CapacityTCDetails(string sOfficeCode)
        {
            string Qry = string.Empty;
            try
            {
                Qry = " SELECT\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TC_MAKE_ID\"=\"TM_ID\"  AND \"MD_TYPE\"='SR' and ";
                Qry += " \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" BETWEEN 125 ";
                Qry += " AND 250 AND \"TC_LOCATION_ID\" = '" + sOfficeCode + "'";
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ObjCon.FetchDataTable(Qry);
        }

        /// <summary>
        /// For diplaying greater than 250 capacity tc details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable Loadgreater250CapacityTCDetails(string sOfficeCode)
        {
            string Qry = string.Empty;
            try
            {
                Qry = " SELECT\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\" FROM ";
                Qry += " \"TBLTCMASTER\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_RATING\"=\"MD_ID\" AND \"TC_MAKE_ID\"=\"TM_ID\"  AND \"MD_TYPE\"='SR' and ";
                Qry += " \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" > 250 AND ";
                Qry += " \"TC_LOCATION_ID\" = '" + sOfficeCode + "'";
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ObjCon.FetchDataTable(Qry);
        }

        /// <summary>
        /// For diplaying replacement inv pending details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadTCpending_issue_countDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;

                Qry = " SELECT  \"DF_DTC_CODE\" ,CAST(\"TC_CODE\"AS TEXT)\"TC_CODE\" ,\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")";
                Qry += " \"TC_RATING\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||date_part('day',age(CURRENT_DATE, ";
                Qry += " CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"STATUS\"   FROM \"TBLPENDINGTRANSACTION\",\"TBLTCMASTER\",";
                Qry += " \"TBLDTCFAILURE\",\"TBLMASTERDATA\",\"TBLTRANSMAKES\",\"TBLBUSINESSOBJECT\",\"TBLROLES\" ";
                Qry += " WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"TRANS_BO_ID\" ";
                Qry += " in(29,13)  AND \"TRANS_BO_ID\"<>10 AND \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR'AND \"TRANS_DTC_CODE\"=\"DF_DTC_CODE\" and ";
                Qry += " \"DF_EQUIPMENT_ID\"=\"TC_CODE\" and  \"TRANS_NEXT_ROLE_ID\"<>0 and \"BO_ID\"=\"TRANS_BO_ID\" and ";
                Qry += " \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" and ";
                Qry += "\"DF_REPLACE_FLAG\"=0 and";
                string sOffCode = clsStoreOffice.GetOfficeCode(sOfficeCode, "TRANS_REF_OFF_CODE");
                Qry += sOffCode;

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying tc pending in repairer details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadTCpending_repair_countDetails(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string Qry = string.Empty;

                //PENDING TO TEST
                Qry = "SELECT '' as \"DF_DTC_CODE\",CAST(\"TC_CODE\"AS TEXT)\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",";
                Qry += " \"TM_NAME\",(\"MD_NAME\")\"TC_RATING\",'PENDING WITH MT AT INSPECTION' AS \"STATUS\" FROM \"TBLTCMASTER\" ";
                Qry += " ,\"TBLMASTERDATA\",\"TBLTRANSMAKES\" WHERE \"TC_CURRENT_LOCATION\" =3 AND \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                Qry += " \"TC_STATUS\" = 3 AND \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR'AND \"TC_LOCATION_ID\"='" + sOfficeCode + "' and ";
                Qry += " \"TC_CODE\" in ( SELECT \"RSD_TC_CODE\"  FROM \"TBLREPAIRSENTMASTER\" ";
                Qry += " inner join \"TBLREPAIRSENTDETAILS\"  on \"RSM_ID\"=\"RSD_RSM_ID\" ";
                Qry += "left join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" WHERE \"RSD_RV_NO\" is null and \"IND_RSD_ID\" is  null  ";
                Qry += " and  \"RSM_DIV_CODE\" = '" + sOfficeCode + "')";
                Qry += "union all ";
                // PENDING TO RECEIVE
                Qry += "SELECT '' as \"DF_DTC_CODE\",CAST(\"TC_CODE\"AS TEXT)\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",";
                Qry += " \"TM_NAME\",(\"MD_NAME\")\"TC_RATING\",'PENDING WITH SK AT RECEIVE' AS \"STATUS\" FROM \"TBLTCMASTER\"  ";
                Qry += " ,\"TBLMASTERDATA\",\"TBLTRANSMAKES\" WHERE \"TC_CURRENT_LOCATION\" =3 AND \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                Qry += " \"TC_STATUS\" = 3 AND \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR'AND \"TC_LOCATION_ID\"='" + sOfficeCode + "' ";
                Qry += " and \"TC_CODE\" in ( SELECT \"RSD_TC_CODE\"  FROM \"TBLREPAIRSENTMASTER\" inner join \"TBLREPAIRSENTDETAILS\" ";
                Qry += "  on \"RSM_ID\"=\"RSD_RSM_ID\" inner join \"TBLINSPECTIONDETAILS\" on \"RSD_ID\"=\"IND_RSD_ID\" ";
                Qry += " WHERE   \"RSD_RV_NO\" is null   and \"IND_RSD_ID\" is not null ";
                Qry += " and  \"RSM_DIV_CODE\" = '" + sOfficeCode + "')";

                dt = ObjCon.FetchDataTable(Qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }

        /// <summary>
        /// For diplaying ri(rv) pending details
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable LoadTCpending_release_countDetails(string sOfficeCode)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                //Qry = " SELECT \"DF_DTC_CODE\",CAST(\"TC_CODE\"AS TEXT)\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\", ";
                //Qry += " (\"MD_NAME\")\"TC_RATING\",'PENDING WITH '||\"RO_NAME\" ||' SINCE '||date_part('day', ";
                //Qry += " age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"STATUS\" FROM \"TBLTCMASTER\" INNER JOIN  ";
                //Qry += "  \"TBLMASTERDATA\" ON \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR' INNER JOIN \"TBLTRANSMAKES\" ON \"TC_MAKE_ID\"=\"TM_ID\" ";
                //Qry += "  INNER JOIN \"TBLDTCFAILURE\" ON \"TC_CODE\"=\"DF_EQUIPMENT_ID\"  and \"DF_REPLACE_FLAG\"=0 INNER JOIN \"TBLWORKORDER\" ";
                //Qry += "  on \"DF_ID\"=\"WO_DF_ID\"  INNER JOIN \"TBLTCREPLACE\" on  \"WO_SLNO\"=\"TR_WO_SLNO\" and \"TR_RV_NO\" is null and \"TR_RI_NO\" ";
                //Qry += " is not null INNER JOIN \"TBLPENDINGTRANSACTION\" ON \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" AND  \"TRANS_BO_ID\"=15 INNER JOIN ";
                //Qry += " \"TBLBUSINESSOBJECT\" on \"BO_ID\"=\"TRANS_BO_ID\" INNER JOIN   \"TBLROLES\" on \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" ";
                //Qry += " and \"WO_SLNO\" is not nulland  \"TR_STORE_SLNO\"='" + sOfficeCode + "' ";

                strQry = " SELECT \"DF_DTC_CODE\",CAST(\"TC_CODE\"AS TEXT)\"TC_CODE\",\"TC_SLNO\",\"TC_CAPACITY\",\"TM_NAME\",(\"MD_NAME\")\"TC_RATING\",'PENDING WITH ";
                strQry += " '||\"RO_NAME\" ||' SINCE '||date_part('day',age(CURRENT_DATE,CAST(\"TRANS_UPDATE_DATE\" AS DATE)))||' Days' as \"STATUS\"  ";
                strQry += " FROM \"TBLTCMASTER\" INNER JOIN  \"TBLMASTERDATA\" ON \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR' INNER JOIN \"TBLTRANSMAKES\" ON ";
                strQry += " \"TC_MAKE_ID\"=\"TM_ID\"  INNER JOIN \"TBLDTCFAILURE\" ON \"TC_CODE\"=\"DF_EQUIPMENT_ID\"  and \"DF_REPLACE_FLAG\"=0 INNER JOIN ";
                strQry += " \"TBLWORKORDER\" on \"DF_ID\"=\"WO_DF_ID\"  INNER JOIN \"TBLTCREPLACE\" on  \"WO_SLNO\"=\"TR_WO_SLNO\" and \"TR_RV_NO\" is null and ";
                strQry += "\"TR_RI_NO\" is not null INNER JOIN \"TBLPENDINGTRANSACTION\" ON \"DF_DTC_CODE\"=\"TRANS_DTC_CODE\" AND  \"TRANS_BO_ID\"=15 INNER JOIN ";
                strQry += "\"TBLBUSINESSOBJECT\" on \"BO_ID\"=\"TRANS_BO_ID\" INNER JOIN   \"TBLROLES\" on \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" and \"WO_SLNO\" is not null ";
                strQry += "and  \"TR_STORE_SLNO\"='" + sOfficeCode + "' ";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get Dtr detials
        /// </summary>
        /// <param name="off_code"></param>
        /// <param name="Dtrtype"></param>
        /// <returns></returns>
        public DataTable GetDtrDetails(string off_code, string Dtrtype)
        {
            DataTable dtDTRDetails = new DataTable();
            string Qry = string.Empty;
            if (Dtrtype == "0")
            {
                Qry = " SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                Qry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND ";
                Qry += " \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" ,'' as \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" ";
                Qry += " WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%'";
                if (off_code == "" || off_code == null)
                {
                    Qry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    Qry += " ORDER BY \"TC_ID\" DESC ";
                }
                // 
                dtDTRDetails = ObjCon.FetchDataTable(Qry);
            }
            else if (Dtrtype == "1")
            {
                Qry = " SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                Qry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\",";
                Qry += " (select \"DIV_NAME\" from \"TBLSTOREMAST\",\"TBLDIVISION\" where \"SM_CODE\"=\"DIV_CODE\" and ";
                Qry += " \"SM_ID\"=\"TC_LOCATION_ID\" ) \"DIV_NAME\"  FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE ";
                Qry += " \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) ";
                if ((off_code ?? "").Length > 0)
                {
                    Qry += " LIKE '" + clsStoreOffice.GetStoreID(off_code) + "' ";
                }
                else
                {
                    Qry += " LIKE '%' ";
                }
                Qry += " AND \"TC_CURRENT_LOCATION\" in (1,4,6,7) ";

                if (off_code == "" || off_code == null)
                {
                    Qry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    Qry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(Qry);
            }
            else if (Dtrtype == "3" || Dtrtype == "5")
            {
                Qry = " SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                Qry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" , ";
                Qry += " (select DISTINCT \"DIV_NAME\" from \"TBLSTOREOFFCODE\",\"TBLDIVISION\",\"TBLTRANSREPAIREROFFCODE\" where ";
                Qry += " \"TRO_OFF_CODE\"=\"DIV_CODE\" and \"TRO_OFF_CODE\"=\"STO_OFF_CODE\" and  \"STO_SM_ID\"=\"TC_LOCATION_ID\" ";
                Qry += " ORDER BY \"DIV_NAME\" limit 1) \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE ";
                Qry += " \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) ";
                if ((off_code ?? "").Length > 0)
                {
                    Qry += " LIKE '" + clsStoreOffice.GetStoreID(off_code) + "' ";
                }
                else
                {
                    Qry += " LIKE '%' ";
                }
                Qry += " AND \"TC_CURRENT_LOCATION\"='" + Dtrtype + "'";

                if (off_code == "" || off_code == null)
                {
                    Qry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    Qry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(Qry);
            }
            else
            {
                Qry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                Qry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" , ";
                Qry += " (select DISTINCT \"DIV_NAME\" from \"TBLSTOREOFFCODE\",\"TBLDIVISION\",\"TBLTRANSREPAIREROFFCODE\" where ";
                Qry += " \"TRO_OFF_CODE\"=\"DIV_CODE\" and \"TRO_OFF_CODE\"=\"STO_OFF_CODE\" and  \"STO_SM_ID\"=\"TC_LOCATION_ID\" ";
                Qry += " ORDER BY \"DIV_NAME\" limit 1) \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" ";
                Qry += " AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' AND \"TC_CURRENT_LOCATION\"='" + Dtrtype + "'";

                if (off_code == "" || off_code == null)
                {
                    Qry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    Qry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(Qry);
            }
            return dtDTRDetails;
        }
        /// <summary>
        /// This method used to get counts  of failure pending
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable getFailurePendingCounts(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\" NOT IN (15,26,10) THEN 1 ELSE 0 END ),0) AS \"TOTA_DTC_FAILURE\" ";
                strQry += " ,COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"=9 THEN 1 ELSE 0 END ),0)AS \"FAILURE_APPROVE\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\" in(45,73) THEN 1 ELSE 0 END ),0) AS \"PENDING_ESTIMATION\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"=46 THEN 1 ELSE 0 END ),0) AS \"PENDING_RECIEVE_DTR\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"=47  THEN 1 ELSE 0 END ),0) AS \"PENDING_COMMISS\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"IN (12,29) THEN 1 ELSE 0 END ),0) AS \"PENDING_INDENT\" , ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"=14  THEN 1 ELSE 0 END ) ,0) AS \"PENDING_DECOMMI\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\" IN(15)  THEN 1 ELSE 0 END ),0) AS \"PENDING_RI\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\" IN(26)  THEN 1 ELSE 0 END ),0) AS \"PENDING_CR\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"TRANS_BO_ID\"=13  THEN 1 ELSE 0 END ),0) AS \"PENDING_MAJOR_INV\", ";
                strQry += " COALESCE(SUM(CASE WHEN \"EST_FAIL_TYPE\"=2 AND \"TRANS_BO_ID\" in(11,74) THEN 1 ELSE 0 END ),0) AS ";
                strQry += " \"PEN_MULTI_COIL_WOR\",COALESCE(SUM(CASE WHEN \"EST_FAIL_TYPE\"=1 AND \"TRANS_BO_ID\" in(11,74) ";
                strQry += " THEN 1 ELSE 0 END ),0) AS \"PEN_SINGLE_COIL_WOR\" FROM \"TBLPENDINGTRANSACTION\" LEFT JOIN ";
                strQry += " (SELECT \"EST_FAIL_TYPE\",\"DF_DTC_CODE\" FROM \"TBLESTIMATIONDETAILS\" INNER JOIN \"TBLDTCFAILURE\" ";
                strQry += " ON \"DF_ID\"=\"EST_FAILUREID\" WHERE CAST(\"DF_LOC_CODE\" AS TEXT) LIKE '" + sOfficeCode + "%' AND ";
                strQry += " \"DF_REPLACE_FLAG\"='0')A ON \"TRANS_DTC_CODE\"=\"DF_DTC_CODE\" WHERE CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) ";
                strQry += " LIKE '" + sOfficeCode + "%' and  \"TRANS_NEXT_ROLE_ID\"<>0";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get failure dtc counts 
        /// </summary>
        /// <param name="sOfficeCode"></param>
        /// <returns></returns>
        public DataTable getFailureCounts(string sOfficeCode)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT  count(distinct \"TRANS_DTC_CODE\") AS \"TOTA_DTC_FAILURE\" from \"TBLPENDINGTRANSACTION\" ";
                strQry += " WHERE \"TRANS_BO_ID\"  in(45,73,46,12,29,11,74,13,47)  and CAST(\"TRANS_REF_OFF_CODE\" AS TEXT) ";
                strQry += " like '" + sOfficeCode + "%' and \"TRANS_NEXT_ROLE_ID\"<>0 ";
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get Faulty Dtr at fields
        /// </summary>
        /// <param name="objDashboard"></param>
        /// <returns></returns>
        public DataTable GetFaultyTCFields(clsDashboard objDashboard)
        {
            string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGFILEPATH"]) + DateTime.Now.ToString("yyyyMM");

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "select \"Field_Count\"+\"Store_count\"+\"Repair_Count\" AS \"total_count\",* from ";
                strQry += " (SELECT sum(case when \"TC_CURRENT_LOCATION\"='2'  and   \"TC_STATUS\"='3' AND ";
                strQry += " cast(\"TC_LOCATION_ID\" as text) like '" + objDashboard.sOfficeCode + "%' then 1 else 0 end) as ";
                strQry += " \"Field_Count\",sum(case when \"TC_CURRENT_LOCATION\"='1' AND  \"TC_STATUS\"='3'";
                if (objDashboard.sOfficeCode != "")
                {
                    strQry += " and CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashboard.sOfficeCode) + "'";
                }
                strQry += " then 1 else 0 end) as \"Store_count\",sum(case when \"TC_CURRENT_LOCATION\"='3' and   \"TC_STATUS\"='3'";
                if (objDashboard.sOfficeCode != "")
                {
                    strQry += " and CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashboard.sOfficeCode) + "'";
                }
                strQry += " then 1 else 0 end) as \"Repair_Count\",sum(case when \"TC_CURRENT_LOCATION\"='1' and   \"TC_STATUS\"='2'";
                if (objDashboard.sOfficeCode != "")
                {
                    strQry += " and CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objDashboard.sOfficeCode) + "'";
                }

                strQry += "then 1 else 0 end) as \"Repair_Good\" from \"TBLTCMASTER\" where \"TC_CODE\"<>0 )A ";

                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        /// <summary>
        /// This method used to get the total dtr's count of dtr at store,bank ,repairer,and field.
        /// </summary>
        /// <param name="objDashboard"></param>
        /// <returns></returns>
        public DataTable GetTcDetails(clsDashboard objDashboard)
        {

            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                if (objDashboard.RoleType != "2")
                {
                    if (objDashboard.sRoleId == "8" || objDashboard.sRoleId == "11")
                    {
                        #region Commented by sandeep bcz converted inline query to sp
                        //strQry = " SELECT COUNT(\"TC_ID\") as \"total_tc\",SUM(case when \"TC_CURRENT_LOCATION\" in (1,4,6,7) then 1 else 0 end) ";
                        //strQry += " as \"TotalStoreDtr\" , SUM(case when \"TC_CURRENT_LOCATION\"=2 then 1 else 0 end)  AS \"field_count\", ";
                        //strQry += " SUM(case when \"TC_CURRENT_LOCATION\"=3 then 1 else 0 end) as \"repairer_count\",SUM(case when ";
                        //strQry += " \"TC_CURRENT_LOCATION\"=5 then 1 else 0 end) as \"bank_count\"   FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" ";
                        //strQry += " WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND  \"TC_CODE\" <> '0'  and \"TC_CAPACITY\" <>0 AND \"TC_STATUS\"='1' ";
                        #endregion
                        NpgsqlCommand cmd = new NpgsqlCommand("dashboard_totaldtr_admin");
                        DataTable Totaldtrdetails = ObjCon.FetchDataTable(cmd);
                        return Totaldtrdetails;

                    }
                    else
                    {
                        string store_id = clsStoreOffice.GetStoreID(objDashboard.sOfficeCode);
                        NpgsqlCommand cmd = new NpgsqlCommand("dashboard_totaldtrcount_web");
                        cmd.Parameters.AddWithValue("officecode", Convert.ToString(objDashboard.sOfficeCode));
                        cmd.Parameters.AddWithValue("storeid", Convert.ToString(store_id));
                        DataTable Totaldtrdetails = ObjCon.FetchDataTable(cmd);
                        return Totaldtrdetails;
                    }
                }
                else
                {
                    strQry = " SELECT COUNT(\"TC_ID\") as \"total_tc\",SUM(case when \"TC_CURRENT_LOCATION\" in (1,4,6,7) then 1 else 0 end) ";
                    strQry += " as \"TotalStoreDtr\" , SUM(case when \"TC_CURRENT_LOCATION\"=2 then 1 else 0 end)  AS \"field_count\", ";
                    strQry += " SUM(case when \"TC_CURRENT_LOCATION\"=3 then 1 else 0 end) as \"repairer_count\", ";
                    strQry += " SUM(case when \"TC_CURRENT_LOCATION\"=5 then 1 else 0 end) as \"bank_count\" ";
                    strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                    strQry += " cast(\"TC_LOCATION_ID\" as text) LIKE '" + objDashboard.sOfficeCode + "%' AND \"TC_CODE\" <> '0' ";
                    strQry += " and \"TC_CAPACITY\" <>0 AND \"TC_STATUS\"='1'";
                }
                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }

        }
        /// <summary>
        /// This method used to load the dtr details for excel
        /// </summary>
        /// <param name="off_code"></param>
        /// <param name="Dtrtype"></param>
        /// <returns></returns>
        public DataTable LoadDtrDetailsexcel(string off_code, string Dtrtype)
        {
            DataTable dtDTRDetails = new DataTable();
            string strQry = string.Empty;
           string storeid = clsStoreOffice.GetStoreID(off_code);
            if (Dtrtype == "0")
            {
                strQry = " SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\", ";
                strQry += " '' as \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                strQry += " cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' AND \"TC_CODE\" <> '0'";
                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  ";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            else if (Dtrtype == "1")
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' ";
                strQry += " AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" ,(select \"DIV_NAME\" from \"TBLSTOREMAST\",\"TBLDIVISION\" ";
                strQry += " where \"SM_CODE\"=\"DIV_CODE\" and  \"SM_ID\"=\"TC_LOCATION_ID\" ) \"DIV_NAME\" FROM ";
                strQry += " \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                if((storeid ?? "").Length >0)
                {
                    strQry += " cast(\"TC_LOCATION_ID\" as text) LIKE '" + clsStoreOffice.GetStoreID(off_code) + "' ";
                }
                else
                {
                    strQry += " cast(\"TC_LOCATION_ID\" as text) LIKE '%' ";

                }
                strQry += " AND \"TC_CURRENT_LOCATION\" in (1,4,6,7) AND \"TC_CODE\" <> '0'";

                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  ";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            else if (Dtrtype == "3" || Dtrtype == "5")
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" ";
                strQry += " ,(select DISTINCT \"DIV_NAME\" from \"TBLSTOREOFFCODE\",\"TBLDIVISION\",\"TBLTRANSREPAIREROFFCODE\" ";
                strQry += " where \"TRO_OFF_CODE\"=\"DIV_CODE\" and \"TRO_OFF_CODE\"=\"STO_OFF_CODE\" and  \"STO_SM_ID\"=\"TC_LOCATION_ID\" ";
                strQry += " ORDER BY \"DIV_NAME\" limit 1) \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" ";
                if ((storeid ?? "").Length > 0)
                {
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + clsStoreOffice.GetStoreID(off_code) + "' AND ";
                }
                else
                {
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) LIKE '%' AND ";

                }
                strQry += " \"TC_CURRENT_LOCATION\"='" + Dtrtype + "' AND \"TC_CODE\" <> '0'";

                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  ";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            else
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND ";
                strQry += " \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" ,(select DISTINCT \"DIV_NAME\" from ";
                strQry += " \"TBLSTOREOFFCODE\",\"TBLDIVISION\",\"TBLTRANSREPAIREROFFCODE\" where \"TRO_OFF_CODE\"=\"DIV_CODE\" ";
                strQry += " and \"TRO_OFF_CODE\"=\"STO_OFF_CODE\" and  \"STO_SM_ID\"=\"TC_LOCATION_ID\" ORDER BY \"DIV_NAME\" limit 1) ";
                strQry += " \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND ";
                strQry += " cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' AND ";
                strQry += " \"TC_CURRENT_LOCATION\"='" + Dtrtype + "' AND \"TC_CODE\" <> '0'";

                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  ";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }

            return dtDTRDetails;
        }
        /// <summary>
        /// This method used to load the dtr details for search.
        /// </summary>
        /// <param name="off_code"></param>
        /// <param name="Dtrtype"></param>
        /// <param name="tccode"></param>
        /// <returns></returns>
        public DataTable GetDtrDetailssearch(string off_code, string Dtrtype, string tccode)
        {
            DataTable dtDTRDetails = new DataTable();
            string strQry = string.Empty;
            if (Dtrtype == "0")
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND ";
                strQry += " \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" , '' as \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE ";
                strQry += " \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' and ";
                strQry += " cast(\"TC_CODE\" AS TEXT) LIKE '" + tccode + "%' AND \"TC_CODE\" <> '0'";
                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            else if (Dtrtype == "1")
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND ";
                strQry += " \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" , '' as \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE ";
                strQry += " \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' ";
                strQry += " AND \"TC_CURRENT_LOCATION\" in (1,4,6,7) and ";
                strQry += " cast(\"TC_CODE\" AS TEXT) LIKE '" + tccode + "%' AND \"TC_CODE\" <> '0'";
                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            else
            {
                strQry = "SELECT \"TC_ID\",cast(\"TC_CODE\" as text),\"TC_SLNO\",\"TM_NAME\",\"TC_CAPACITY\", ";
                strQry += " (SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' AND ";
                strQry += " \"MD_ID\"=\"TC_RATING\")\"TC_RATING\" , '' as \"DIV_NAME\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" ";
                strQry += " WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND cast(\"TC_LOCATION_ID\" as text) LIKE '" + off_code + "%' ";
                strQry += " AND \"TC_CURRENT_LOCATION\"='" + Dtrtype + "' and ";
                strQry += " cast(\"TC_CODE\" AS TEXT) LIKE '" + tccode + "%' AND \"TC_CODE\" <> '0'";
                if (off_code == "" || off_code == null)
                {
                    strQry += " ORDER BY \"TC_ID\" DESC  LIMIT 100";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC ";
                }
                dtDTRDetails = ObjCon.FetchDataTable(strQry);
            }
            return dtDTRDetails;
        }
        /// <summary>
        /// This method used to load failure abstract counts office wise
        /// </summary>
        /// <param name="objDashboard"></param>
        /// <returns></returns>
        public DataTable LoadDTCFailureAbstractofficewise(clsDashboard objDashboard)
        {
            string strQry = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                int Length = 0;
                Length = objDashboard.sOfficeCode.Length + 1;

                strQry = " SELECT *,\"PRESENTCOUNT\"+\"PREVIOUSCOUNT\" AS \"TOTAL_DTCCOUNT\" FROM (SELECT \"OFF_CODE\",SUM(CASE WHEN ";
                strQry += " ((\"DF_DATE\" IS NULL AND TO_CHAR(\"DF_DATE\",'YYYY')=TO_CHAR(CURRENT_DATE,'YYYY')) OR (\"DF_DATE\" IS NOT";
                strQry += "  NULL AND TO_CHAR(\"DF_DATE\",'YYYY')=TO_CHAR(CURRENT_DATE,'YYYY'))) THEN 1 ELSE 0 END) AS \"PRESENTCOUNT\",SUM(CASE ";
                strQry += "  WHEN ((\"DF_DATE\" IS NULL AND TO_CHAR(\"DF_DATE\",'YYYY')=TO_CHAR(NOW()::TIMESTAMP - interval '1 YEAR','YYYY'))";
                strQry += "  OR (\"DF_DATE\" IS NOT NULL AND TO_CHAR(\"DF_DATE\",'YYYY')= ";
                strQry += " TO_CHAR(NOW()::TIMESTAMP - interval '1 YEAR','YYYY'))) THEN 1 ELSE 0 END) as \"PREVIOUSCOUNT\",";
                strQry += "  \"OFF_NAME\" from \"TBLDTCFAILURE\"  INNER JOIN \"VIEW_ALL_OFFICES\" ON cast(\"OFF_CODE\" as text)= ";
                strQry += " SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + Length + ")  ";
                strQry += " AND \"DF_STATUS_FLAG\" IN (1,4) WHERE  CAST(\"OFF_CODE\" AS text) ";
                strQry += "LIKE '" + objDashboard.sOfficeCode + "%' GROUP BY \"OFF_NAME\",\"OFF_CODE\")A ";


                dt = ObjCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

    }
}
