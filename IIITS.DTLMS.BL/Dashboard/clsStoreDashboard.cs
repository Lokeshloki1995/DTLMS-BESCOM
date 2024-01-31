using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.OleDb;

namespace IIITS.DTLMS.BL.Dashboard
{
    public class clsStoreDashboard
    {
        String strFormCode = "clsStoreDashboard";
        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

        public string sOfficeCode { get; set; }
        public string roletype { get; set; }


        /// <summary>
        /// For diplaying good tc count in store
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetNewTCCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\"='1' AND \"TC_CURRENT_LOCATION\" = 1   ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) = '" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// For diplaying repaire good tc count in store
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetRepaireGoodCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\"='2'AND \"TC_CURRENT_LOCATION\" =1  ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) = '" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// For diplaying release good tc count in store
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetReleaseGoodCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\"='5'AND \"TC_CURRENT_LOCATION\" =1  ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) = '" + objSDashboard.sOfficeCode + "' ";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// For diplaying fauty tc count in store 
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetFaultyCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\"='3' AND \"TC_CURRENT_LOCATION\" =1  ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) ='" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// For diplaying scarp tc count in store  
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>

        public string GetScrapCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STATUS\" IN ('6','4') AND \"TC_CURRENT_LOCATION\" =1  ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text)= '" + objSDashboard.sOfficeCode + "' ";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// for displaying mob transformer count in store
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetMobileTCCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_CONDITION\"='MOBILE TRANSFORMER' AND \"TC_CURRENT_LOCATION\" =1";
                    strQry += "  AND  cast(\"TC_LOCATION_ID\" as text) = '" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// for displaying the count of capacity less than 25
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetCapacityless25(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" < 25 AND  ";
                    strQry += " cast(\"TC_LOCATION_ID\" as text) ='" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// for displaying the count of capacity from 25 to 100 
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetCapacity25_100(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" BETWEEN 25 AND 100 ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text) = '" + objSDashboard.sOfficeCode + "' ";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// for displaying the count of capacity from 125 to 250
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetCapacity125_250(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" BETWEEN 125 AND 250 ";
                    strQry += " AND cast(\"TC_LOCATION_ID\" as text)= '" + objSDashboard.sOfficeCode + "' ";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// for displaying the count of greater than 250 capacity
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetCapacitygreater250(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                if (objSDashboard.sOfficeCode != null || objSDashboard.sOfficeCode != string.Empty)
                {
                    strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_CURRENT_LOCATION\" =1 AND \"TC_CAPACITY\" > 250 AND  ";
                    strQry += " cast(\"TC_LOCATION_ID\" as text)= '" + objSDashboard.sOfficeCode + "'";
                    return ObjCon.get_value(strQry);
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        /// For diplaying count of tc pending for issue(replacement inv) to field  
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>

        public string GetIssuePendingCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                // invoice pending
                string strQry = string.Empty;
                //query changed for count mismatch issue 
                //strQry = "SELECT COUNT(*) FROM \"TBLPENDINGTRANSACTION\" WHERE  \"TRANS_BO_ID\" in(29,13) and";  
                 //strQry = "\"TRANS_BO_ID\"<>10 and \"TRANS_NEXT_ROLE_ID\"<>0 and ";
                strQry = " SELECT COUNT(*) FROM \"TBLPENDINGTRANSACTION\",\"TBLTCMASTER\",\"TBLDTCFAILURE\",\"TBLMASTERDATA\",";
                strQry += " \"TBLTRANSMAKES\",\"TBLBUSINESSOBJECT\", \"TBLROLES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"TRANS_BO_ID\" ";
                strQry += " in(29,13)  AND \"TRANS_BO_ID\"<>10 AND \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR' AND ";
                strQry += " \"TRANS_DTC_CODE\"=\"DF_DTC_CODE\" and \"DF_EQUIPMENT_ID\"=\"TC_CODE\" and  \"TRANS_NEXT_ROLE_ID\"<>0 and ";
                strQry += "\"BO_ID\"=\"TRANS_BO_ID\" and \"RO_ID\"=\"TRANS_NEXT_ROLE_ID\" and  \"DF_REPLACE_FLAG\"=0 and";
                if (objSDashboard.roletype == "2")
                {
                    string sOffCode = clsStoreOffice.GetOfficeCode(objSDashboard.sOfficeCode, "TRANS_REF_OFF_CODE");
                    strQry += sOffCode;
                }
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }

        /// <summary>
        ///  For diplaying faulty tc count pending in repairer 
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>

        public string GetRepairPendingCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                //query changed for count mismatch issue for pending in repairer flow 
                //strQry = " SELECT count(\"RSD_TC_CODE\") ";
                //strQry += " FROM \"TBLREPAIRSENTMASTER\",\"TBLREPAIRSENTDETAILS\" WHERE \"RSM_ID\"=\"RSD_RSM_ID\" and \"RSD_RV_NO\" is ";
                //strQry += " null and \"RSM_DIV_CODE\" = '" + objSDashboard.sOfficeCode + "'";

                strQry = " SELECT count(*)  FROM \"TBLTCMASTER\" ,\"TBLMASTERDATA\",\"TBLTRANSMAKES\" WHERE \"TC_CURRENT_LOCATION\" =3 ";
                strQry += "AND \"TC_MAKE_ID\"=\"TM_ID\" AND  \"TC_STATUS\" = 3 AND \"TC_RATING\"=\"MD_ID\" AND \"MD_TYPE\"='SR' ";
                strQry += " AND \"TC_LOCATION_ID\"='" + sOfficeCode + "' and \"TC_CODE\" in ( SELECT \"RSD_TC_CODE\"  FROM ";
                strQry += "\"TBLREPAIRSENTMASTER\",\"TBLREPAIRSENTDETAILS\" WHERE ";
                strQry += " \"RSM_ID\"=\"RSD_RSM_ID\" and \"RSD_RV_NO\" is null and  \"RSM_DIV_CODE\" = '" + objSDashboard.sOfficeCode + "')";
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }
        /// <summary>
        /// For diplaying pending in RI(RV) count 
        /// </summary>
        /// <param name="objSDashboard"></param>
        /// <returns></returns>
        public string GetRecivePendingCount(clsStoreDashboard objSDashboard)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT count(\"TR_ID\") FROM \"TBLDTCFAILURE\" LEFT JOIN \"TBLWORKORDER\" on \"DF_ID\"=\"WO_DF_ID\" LEFT JOIN  ";
                strQry += " \"TBLINDENT\" on \"WO_SLNO\"=\"TI_WO_SLNO\" LEFT JOIN \"TBLDTCINVOICE\" on \"TI_ID\"=\"IN_TI_NO\" LEFT JOIN  ";
                strQry += " \"TBLTCREPLACE\" on \"WO_SLNO\"=\"TR_WO_SLNO\" and \"TR_RV_NO\" is null and \"TR_RI_NO\" is not null and  ";
                strQry += " \"WO_SLNO\" is not null and \"TR_STORE_SLNO\"='" + objSDashboard.sOfficeCode + "' and \"DF_REPLACE_FLAG\"='0'";
                return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ex.Message;
            }
        }
    }
}

