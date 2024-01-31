using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Data.OleDb;
using System.IO;
using System.Configuration;

namespace IIITS.DTLMS.BL
{

    public class clsDTrRepairActivity
    {

        string strFormCode = "clsDTrRepairActivity";
        PGSqlConnection objCon = new PGSqlConnection(Constants.Password);

        //Tc Details
        public string newdivcode { get; set; }
        public string sTcCode { get; set; }
        public string sTcSlno { get; set; }
        public string sMakeName { get; set; }
        public string sManfDate { get; set; }
        public string sCapacity { get; set; }

        public string sWarrantyPeriod { get; set; }
        public string sSupplierName { get; set; }
        public string sGuarantyType { get; set; }
        public string sMakeId { get; set; }
        public string sStoreId { get; set; }
        public string sSupplierId { get; set; }
        public string sRepairerId { get; set; }
        public string sTcId { get; set; }
        public string sRefString { get; set; }

        //To send to Repairer/Supplier
        public string sSupRepId { get; set; }
        public string sIssueDate { get; set; }
        public string sPurchaseOrderNo { get; set; }
        public string sPurchaseDate { get; set; }
        public string sInvoiceNo { get; set; }
        public string sInvoiceDate { get; set; }
        public string sManualInvoiceNo { get; set; }
        public string sCrby { get; set; }
        public string sType { get; set; }

        public string sRepairDetailsId { get; set; }
        public string sRepairMasterId { get; set; }
        public string sQty { get; set; }

        //Testing Activity       
        public string sPass { get; set; }
        public string sFail { get; set; }

        public string sTestingDone { get; set; }
        public string sTestedBy { get; set; }
        public string sTestedOn { get; set; }
        public string sTestLocation { get; set; }
        public string sInspRemarks { get; set; }
        public string sTestResult { get; set; }
        public string sTestInspectionId { get; set; }

        public DataTable dtTestDone { get; set; }

        //Deliver or Recieve DTR
        public string sDeliverDate { get; set; }
        public string sDeliverChallenNo { get; set; }
        public string sVerifiedby { get; set; }
        public string sOfficeCode { get; set; }
        public string sRVNo { get; set; }
        public string sRVDate { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFDataId { get; set; }
        public string sWFObjectId { get; set; }

        //Document
        public string sFileName { get; set; }
        public string sStatus { get; set; }
        public string sOldPONo { get; set; }
        public string sPORemarks { get; set; }


        public string sroletype { get; set; }
        public string sroleid { get; set; }

        public string UserId { get; set; }
        public string rWaId { get; set; }

        #region Fault TC Search and Send to Repair
        NpgsqlCommand NpgsqlCommand;
        /// <summary>
        /// This method used to fetch the faulty tc records
        /// </summary>
        /// <param name="objTcRepair"></param>
        /// <returns></returns>
        public DataTable LoadFaultTC(clsDTrRepairActivity objTcRepair)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                if (objTcRepair.sroletype != "2" && objTcRepair.sroleid ==
                    Convert.ToString(ConfigurationManager.AppSettings["SupAdminRole"]) || objTcRepair.sOfficeCode == "")
                {
                    strQry = " SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE ";
                    strQry += " cast(\"STO_OFF_CODE\" as text)=(SELECT substr(\"US_OFFICE_CODE\",1,3) ";
                    strQry += " from \"TBLUSER\" WHERE \"US_ID\"='" + objTcRepair.UserId + "')";
                    objTcRepair.sStoreId = objCon.get_value(strQry);
                }

                strQry = "SELECT* FROM (SELECT DISTINCT \"TC_ID\",(select \"RESTD_ID\" from \"TBLREPAIRERESTIMATIONDETAILS\" ";
                strQry += " WHERE \"RESTD_TC_CODE\"=\"TC_CODE\" limit 1) \"RESTD_ID\", \"TC_CODE\", \"TC_SLNO\",\"TC_PREV_OFFCODE\", ";
                strQry += " \"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\", 'DD-MON-YYYY') \"TC_MANF_DATE\", ";
                strQry += "  CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", (SELECT COUNT(\"RSD_TC_CODE\") ";
                strQry += " FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" =  \"TC_CODE\" AND \"RSD_DELIVARY_DATE\" IS NOT NULL) \"RCOUNT\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\", 'DD-MON-YYYY') \"TC_PURCHASE_DATE\", ";
                strQry += " TO_CHAR(\"TC_WARANTY_PERIOD\", 'DD-MON-YYYY') \"TC_WARANTY_PERIOD\", ";
                strQry += " (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE  \"TC_SUPPLIER_ID\" = \"TS_ID\")";
                strQry += " \"TS_NAME\", CASE WHEN \"TC_WARANTY_PERIOD\" IS NOT NULL THEN ";
                strQry += " ((SELECT CASE WHEN NOW() <  \"TC_WARANTY_PERIOD\" THEN \"RSD_GUARRENTY_TYPE\" ELSE 'AGP'  END ";
                strQry += " FROM  \"TBLREPAIRSENTDETAILS\" WHERE \"TC_CODE\" = \"RSD_TC_CODE\"  AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL AND";
                strQry += " \"RSD_WARENTY_PERIOD\" IS NOT NULL  AND \"RSD_ID\" IN(SELECT FIRST_VALUE(\"RSD_ID\") ";
                strQry += " OVER(ORDER BY \"RSD_ID\" DESC) AS \"RSD_ID\" FROM \"TBLREPAIRSENTDETAILS\" WHERE ";
                strQry += " \"RSD_TC_CODE\" = \"TC_CODE\")) )  ELSE(SELECT \"RESTD_GUARANTEETYPE\" FROM \"TBLREPAIRERESTIMATIONDETAILS\"  ";
                strQry += " WHERE \"RESTD_ID\" IN (SELECT  MAX(\"RESTD_ID\") FROM \"TBLREPAIRERESTIMATIONDETAILS\" WHERE  ";
                strQry += " \"RESTD_TC_CODE\" = \"TC_CODE\")) END \"TC_GUARANTY_TYPE\",  (CASE WHEN CAST(\"TC_CODE\" AS TEXT) IN ";
                strQry += " (SELECT UNNEST(STRING_TO_ARRAY(STRING_AGG(\"WO_DATA_ID\", ','), ',')) FROM \"TBLWORKFLOWOBJECTS\",\"TBLWO_OBJECT_AUTO\"";
                strQry += " WHERE \"WO_ID\"=\"WOA_PREV_APPROVE_ID\"  AND \"WOA_BFM_ID\"=33 AND (\"WO_BO_ID\" ='71' or \"WO_BO_ID\" ='72') ";
                strQry += " AND \"WO_DATA_ID\" IS NOT NULL  and \"RESTD_ID\" is not null) THEN 'ALREADY SENT' ELSE 'PENDING' END) \"STATUS\" ";
                strQry += " FROM  \"TBLTCMASTER\" inner join  \"TBLTRANSMAKES\"  on \"TC_MAKE_ID\" = \"TM_ID\"  inner join ";
                strQry += " \"TBLSTOREMAST\" on \"SM_ID\" = \"TC_STORE_ID\" left join ";
                strQry += " \"TBLREPAIRERESTIMATIONDETAILS\" on  \"TC_CODE\"=\"RESTD_TC_CODE\" where  \"TC_STATUS\" = 3 and \"TC_CURRENT_LOCATION\"=1 ";

                if (objTcRepair.sroleid == Convert.ToString(ConfigurationManager.AppSettings["SupAdminRole"])
                    || objTcRepair.sOfficeCode == "")
                {
                    strQry += " and \"TC_PREV_OFFCODE\" like  '" + objTcRepair.sOfficeCode + "%'";
                }
                else
                {
                    strQry += "  and \"TC_PREV_OFFCODE\"= '" + objTcRepair.sOfficeCode + "'";
                }
                if (objTcRepair.sTcId != null)
                {
                    strQry += " AND \"TC_ID\" IN (" + objTcRepair.sTcId + ")";
                }
                else
                {
                    strQry += " AND \"TC_CURRENT_LOCATION\" =1";
                }

                if (objTcRepair.sCapacity != null)
                {
                    strQry += " AND \"TC_CAPACITY\" ='" + objTcRepair.sCapacity + "' ";
                }

                if (objTcRepair.sTcCode != null)
                {
                    strQry += " AND cast(\"TC_CODE\" as text) LIKE '%" + objTcRepair.sTcCode.Trim() + "%'";
                }

                if (objTcRepair.sTcSlno != null)
                {
                    strQry += "AND \"TC_SLNO\" LIKE '%" + objTcRepair.sTcSlno.Trim() + "%'";
                }
                if (objTcRepair.sMakeId != null)
                {
                    strQry += " AND \"TC_MAKE_ID\" ='" + objTcRepair.sMakeId + "'";
                }
                if (objTcRepair.sSupplierId != null)
                {
                    strQry += " AND ((\"TS_ID\" ='" + objTcRepair.sSupplierId
                    + "' AND \"TC_LAST_REPAIRER_ID\" is null) or \"TC_LAST_REPAIRER_ID\" ='" + objTcRepair.sSupplierId + "')";

                }
                strQry += " ORDER BY \"TC_CODE\" )A ";

                if (objTcRepair.sGuarantyType != null)
                {
                    if (objTcRepair.sGuarantyType == "AGP")
                        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='AGP'";
                    else if (objTcRepair.sGuarantyType == "WGP")
                        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='WGP'";
                    else if (objTcRepair.sGuarantyType == "WRGP")
                        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='WRGP'";
                }
                dt = objCon.FetchDataTable(strQry);
                return dt;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public DataTable LoadFaultTCsearch(clsDTrRepairActivity objTcRepair)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                string Superadminuserid = Convert.ToString(ConfigurationManager.AppSettings["SupAdminUserId"]);
                if (objTcRepair.sroletype != "2")
                {
                    if (objTcRepair.UserId != Superadminuserid && objTcRepair.UserId!=null)
                    {
                        objTcRepair.sStoreId = objCon.get_value(" SELECT \"STO_SM_ID\" from \"TBLSTOREOFFCODE\" WHERE cast(\"STO_SM_ID\" as text)=(SELECT \"US_OFFICE_CODE\" from \"TBLUSER\" WHERE \"US_ID\"='" + objTcRepair.UserId + "')");
                    }
                }
                #region
                //strQry = " SELECT* FROM (SELECT \"TC_ID\", \"TC_CODE\", \"TC_SLNO\", \"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\", 'DD-MON-YYYY') \"TC_MANF_DATE\", ";
                //strQry += " CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\", (SELECT COUNT(\"RSD_TC_CODE\") FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" = ";
                //strQry += " \"TC_CODE\" AND \"RSD_DELIVARY_DATE\" IS NOT NULL) \"RCOUNT\",  TO_CHAR(\"TC_PURCHASE_DATE\", 'DD-MON-YYYY') \"TC_PURCHASE_DATE\", ";
                //strQry += " TO_CHAR(\"TC_WARANTY_PERIOD\", 'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",  (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE ";
                //strQry += " \"TC_SUPPLIER_ID\" = \"TS_ID\") \"TS_NAME\", CASE WHEN \"TC_WARANTY_PERIOD\" IS NOT NULL THEN ((SELECT CASE WHEN NOW() < ";
                //strQry += " \"TC_WARANTY_PERIOD\" THEN \"RSD_GUARRENTY_TYPE\" ELSE 'AGP'  END FROM  \"TBLREPAIRSENTDETAILS\" WHERE \"TC_CODE\" = \"RSD_TC_CODE\" ";
                //strQry += " AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL AND \"RSD_WARENTY_PERIOD\" IS NOT NULL  AND \"RSD_ID\" IN(SELECT FIRST_VALUE(\"RSD_ID\") ";
                //strQry += " OVER(ORDER BY \"RSD_ID\" DESC) AS \"RSD_ID\" FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\" = \"TC_CODE\")) ) ";
                //strQry += " ELSE(SELECT \"RESTD_GUARANTEETYPE\" FROM \"TBLREPAIRERESTIMATIONDETAILS\"  WHERE \"RESTD_ID\" IN (SELECT  MAX(\"RESTD_ID\") FROM \"TBLREPAIRERESTIMATIONDETAILS\" WHERE ";
                //strQry += " \"RESTD_TC_CODE\" = \"TC_CODE\")) END \"TC_GUARANTY_TYPE\", \"RWOA_NO\" as \"WORK_AWARD\" ,  (CASE WHEN CAST(\"TC_CODE\" AS TEXT) IN ";
                //strQry += " (SELECT UNNEST(STRING_TO_ARRAY(STRING_AGG(\"WO_DATA_ID\", ','), ',')) FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\" = '30' ";
                //strQry += " AND \"WO_APPROVE_STATUS\" = '0' AND \"WO_DATA_ID\" IS NOT NULL) THEN 'ALREADY SENT' ELSE 'PENDING' END) \"STATUS\" FROM ";
                //strQry += " \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLREPAIRERWORKORDER\",\"TBLREPAIRWORKAWARDDETAILS\",";
                //strQry += " \"TBLREPAIRERWORKAWARD\" WHERE \"RWO_SLNO\"=\"RWAD_WO_SLNO\" and \"RWAO_ID\"=\"RWAD_WA_ID\" and \"TC_RWAO_ID\"=\"RWAO_ID\"  and (SELECT  MAX(\"RESTD_ID\") FROM \"TBLREPAIRERESTIMATIONDETAILS\"";
                //strQry += " WHERE  \"RESTD_TC_CODE\" = \"TC_CODE\")=\"RWO_EST_ID\" and \"TC_RWAO_ID\" is not null  and \"TC_MAKE_ID\" = \"TM_ID\"  AND \"SM_ID\" = \"TC_STORE_ID\" AND \"TC_STATUS\" = 3  ";


                //if (objTcRepair.UserId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                //{
                //       strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + objTcRepair.sStoreId + "' ";
                //}
                //else
                //{
                //    strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + objTcRepair.sOfficeCode + "' ";
                //}

                //if (objTcRepair.sTcId != null)
                //{
                //    strQry += " AND \"TC_ID\" IN (" + objTcRepair.sTcId + ")";
                //}
                //else
                //{
                //    strQry += " AND \"TC_CURRENT_LOCATION\" =1";
                //}

                //if (objTcRepair.sCapacity != null)
                //{
                //    strQry += " AND \"TC_CAPACITY\" ='" + objTcRepair.sCapacity + "' ";
                //}

                //if (objTcRepair.sTcCode != null)
                //{
                //    strQry += "AND \"TC_CODE\" LIKE '%" + objTcRepair.sTcCode.Trim() + "%'";
                //}

                //if (objTcRepair.sTcSlno != null)
                //{
                //    strQry += "AND \"TC_SLNO\" LIKE '%" + objTcRepair.sTcSlno.Trim() + "%'";
                //}
                //if (objTcRepair.sMakeId != null)
                //{
                //    strQry += " AND \"TC_MAKE_ID\" ='" + objTcRepair.sMakeId + "'";
                //}
                //if (objTcRepair.sStoreId != null || objTcRepair.UserId!= Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                //{
                //    strQry += " AND \"SM_ID\" ='" + objTcRepair.sStoreId + "'";
                //}
                //if (objTcRepair.sSupplierId != null)
                //{
                //    strQry += " AND ((\"TS_ID\" ='" + objTcRepair.sSupplierId + "' AND \"TC_LAST_REPAIRER_ID\" is null) or \"TC_LAST_REPAIRER_ID\" ='" + objTcRepair.sSupplierId + "')";
                //}
                //strQry += "ORDER BY \"TC_CODE\" )A  ";

                //if (objTcRepair.sGuarantyType != null)
                //{
                //    if (objTcRepair.sGuarantyType == "AGP")
                //        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='AGP'";
                //    else if (objTcRepair.sGuarantyType == "WGP")
                //        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='WGP'";
                //    else if (objTcRepair.sGuarantyType == "WRGP")
                //        strQry += " WHERE \"TC_GUARANTY_TYPE\" ='WRGP'";
                //}
                //dt = objCon.FetchDataTable(strQry);
                //return dt;
                #endregion
                NpgsqlCommand cmd = new NpgsqlCommand("proc_get_load_fault_tcsearch");
                cmd.Parameters.AddWithValue("i_office_code", Convert.ToString(objTcRepair.sOfficeCode) == null ? "" : Convert.ToString(objTcRepair.sOfficeCode));
                cmd.Parameters.AddWithValue("i_store_id", Convert.ToString(objTcRepair.sStoreId) == null ? "" : Convert.ToString(objTcRepair.sStoreId));
                cmd.Parameters.AddWithValue("i_tc_id", objTcRepair.sTcId == null ? "" : objTcRepair.sTcId);
                cmd.Parameters.AddWithValue("i_capacity", objTcRepair.sCapacity == null ? "" : objTcRepair.sCapacity);
                cmd.Parameters.AddWithValue("i_tc_code", objTcRepair.sTcCode == null ? "" : objTcRepair.sTcCode);
                cmd.Parameters.AddWithValue("i_tc_slno", objTcRepair.sTcSlno == null ? "" : objTcRepair.sTcSlno);
                cmd.Parameters.AddWithValue("i_make_id", objTcRepair.sMakeId == null ? "" : objTcRepair.sMakeId);
                cmd.Parameters.AddWithValue("i_supplier_id", objTcRepair.sSupplierId == null ? "" : objTcRepair.sSupplierId);
                cmd.Parameters.AddWithValue("i_guarantytype", objTcRepair.sGuarantyType == null ? "" : objTcRepair.sGuarantyType);
                cmd.Parameters.AddWithValue("i_superadmin_id", Superadminuserid == null ? "" : Superadminuserid);
                cmd.Parameters.AddWithValue("i_user_id", objTcRepair.UserId == null ? "" : objTcRepair.UserId);
                dt = objCon.FetchDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }



        public clsDTrRepairActivity GetFaultTCDetails(clsDTrRepairActivity objTcRepair)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') TC_PURCHASE_DATE,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') TC_WARANTY_PERIOD,(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\" = \"TS_ID\") TS_NAME , ";
                strQry += " (CASE WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD')<TO_CHAR(NOW(),'YYYYMMDD') THEN 'AGP' ";
                strQry += " WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD') > TO_CHAR(NOW(),'YYYYMMDD') THEN 'WGP' END )TC_GUARANTY_TYPE";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\"  ";
                strQry += " AND \"TC_CURRENT_LOCATION\" <>3 AND \"TC_CODE\" =:sTcCode";

                if (objTcRepair.sRefString != null)
                {
                    strQry += " AND \"TC_STATUS\" =4 ";
                }
                else
                {
                    strQry += " AND \"TC_STATUS\" =7 ";
                }

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode", Convert.ToDouble(objTcRepair.sTcCode));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objTcRepair.sTcId = dt.Rows[0]["TC_ID"].ToString();
                    objTcRepair.sTcCode = dt.Rows[0]["TC_CODE"].ToString();
                    objTcRepair.sTcSlno = dt.Rows[0]["TC_SLNO"].ToString();
                    objTcRepair.sMakeName = dt.Rows[0]["TM_NAME"].ToString();
                    objTcRepair.sManfDate = dt.Rows[0]["TC_MANF_DATE"].ToString();
                    objTcRepair.sCapacity = dt.Rows[0]["TC_CAPACITY"].ToString();
                    objTcRepair.sPurchaseDate = dt.Rows[0]["TC_PURCHASE_DATE"].ToString();
                    objTcRepair.sWarrantyPeriod = dt.Rows[0]["TC_WARANTY_PERIOD"].ToString();
                    objTcRepair.sSupplierName = dt.Rows[0]["TS_NAME"].ToString();
                    objTcRepair.sGuarantyType = dt.Rows[0]["TC_GUARANTY_TYPE"].ToString();

                }
                return objTcRepair;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTcRepair;
            }
        }

        public clsDTrRepairActivity AddFaultTCDetails(clsDTrRepairActivity objTcRepair)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') TC_PURCHASE_DATE,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') TC_WARANTY_PERIOD,(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") TS_NAME , ";
                strQry += " (CASE WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD')<TO_CHAR(NOW(),'YYYYMMDD') THEN 'AGP' ";
                strQry += " WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD') > TO_CHAR(NOW(),'YYYYMMDD') THEN 'WGP' END )TC_GUARANTY_TYPE";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\"  ";
                strQry += " AND \"TC_CURRENT_LOCATION\" <>3 AND \"TC_CODE\" =:sTcCode";

                if (objTcRepair.sRefString != null)
                {
                    strQry += " AND \"TC_STATUS\" =4 ";
                }
                else
                {
                    strQry += " AND \"TC_STATUS\" =3 ";
                }

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode", Convert.ToDouble(objTcRepair.sTcCode));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objTcRepair.sTcId = dt.Rows[0]["TC_ID"].ToString();
                    objTcRepair.sTcCode = dt.Rows[0]["TC_CODE"].ToString();
                    objTcRepair.sTcSlno = dt.Rows[0]["TC_SLNO"].ToString();
                    objTcRepair.sMakeName = dt.Rows[0]["TM_NAME"].ToString();
                    objTcRepair.sManfDate = dt.Rows[0]["TC_MANF_DATE"].ToString();
                    objTcRepair.sCapacity = dt.Rows[0]["TC_CAPACITY"].ToString();
                    objTcRepair.sPurchaseDate = dt.Rows[0]["TC_PURCHASE_DATE"].ToString();
                    objTcRepair.sWarrantyPeriod = dt.Rows[0]["TC_WARANTY_PERIOD"].ToString();
                    objTcRepair.sSupplierName = dt.Rows[0]["TS_NAME"].ToString();
                    objTcRepair.sGuarantyType = dt.Rows[0]["TC_GUARANTY_TYPE"].ToString();
                }
                return objTcRepair;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTcRepair;
            }
        }
        /// <summary>
        /// this method used to get the repair sent Dtr's for store officer.
        /// </summary>
        /// <param name="sRepairMasterId"></param>
        /// <returns></returns>
        public DataTable LoadRepairSentDTR(string sRepairMasterId)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                #region Commenetd by siddesh on 24-02-2023
                //strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,";
                //strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') TC_PURCHASE_DATE,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') TC_WARANTY_PERIOD, ";
                //strQry += " (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") TS_NAME,";
                //strQry += " (CASE WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD')<TO_CHAR(NOW(),'YYYYMMDD') THEN 'AGP' ";
                //strQry += " WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD') > TO_CHAR(NOW(),'YYYYMMDD') THEN 'WGP' END )TC_GUARANTY_TYPE, \"RWOA_NO\" as \"WORK_AWARD\" ";
                //strQry += " FROM \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLREPAIRSENTMASTER\",\"TBLREPAIRERWORKORDER\",\"TBLREPAIRWORKAWARDDETAILS\",\"TBLREPAIRERWORKAWARD\" ,\"TBLREPAIRERESTIMATIONDETAILS\" WHERE \"RSD_TC_CODE\"=\"TC_CODE\" AND  \"TC_MAKE_ID\"=\"TM_ID\"  ";
                //strQry += " and \"RWO_SLNO\"=\"RWAD_WO_SLNO\" and \"RWAO_ID\"=\"RWAD_WA_ID\" and  \"RESTD_ID\"=\"RWO_EST_ID\"  and  \"TC_CODE\"=\"RESTD_TC_CODE\" and \"RSD_RSM_ID\"=\"RSM_ID\" AND \"RSM_ID\" =:sRepairMasterId";
                #endregion
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\",TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,";
                strQry += " CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') TC_PURCHASE_DATE,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') TC_WARANTY_PERIOD, ";
                strQry += " (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") TS_NAME,";
                strQry += " (CASE WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD')<TO_CHAR(NOW(),'YYYYMMDD') THEN 'AGP' ";
                strQry += " WHEN TO_CHAR(\"TC_WARANTY_PERIOD\",'YYYYMMDD') > TO_CHAR(NOW(),'YYYYMMDD') THEN 'WGP' END )TC_GUARANTY_TYPE, ";
                strQry += " \"RWOA_NO\" as \"WORK_AWARD\" ";
                strQry += " FROM \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLREPAIRSENTMASTER\",\"TBLREPAIRERWORKORDER\", ";
                strQry += " \"TBLREPAIRWORKAWARDDETAILS\",\"TBLREPAIRERWORKAWARD\"  WHERE \"RSD_TC_CODE\"=\"TC_CODE\" AND  \"TC_MAKE_ID\"=\"TM_ID\"  ";
                strQry += " and \"RWO_SLNO\"=\"RWAD_WO_SLNO\" and \"RWAO_ID\"=\"RWAD_WA_ID\" and   (SELECT  MAX(\"RESTD_ID\") FROM ";
                strQry += " \"TBLREPAIRERESTIMATIONDETAILS\" WHERE  \"RESTD_TC_CODE\" = \"TC_CODE\")=\"RWO_EST_ID\" ";
                strQry += "    and \"RSD_RSM_ID\"=\"RSM_ID\" AND \"RSM_ID\" =:sRepairMasterId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        public string[] SaveRepairIssueDetails(string[] sTcCodes, clsDTrRepairActivity objTcRepair)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            string strQry1 = string.Empty;
            bool bResult = false;
            try
            {
                //objCon.BeginTransaction();

                //if (objTcRepair.sOfficeCode.Length > 3)
                //{
                //    objTcRepair.sOfficeCode = objTcRepair.sOfficeCode.Substring(0, 3);
                //}
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sPurchaseOrderNo", objTcRepair.sPurchaseOrderNo.ToUpper());
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sOfficeCode == "" ? objTcRepair.sStoreId : objTcRepair.sOfficeCode));
                string sPONo = objCon.get_value("SELECT \"RSM_ID\" FROM \"TBLREPAIRSENTMASTER\" WHERE UPPER(\"RSM_PO_NO\")=:sPurchaseOrderNo AND \"RSM_DIV_CODE\" =:sOfficeCode", NpgsqlCommand);
                if (sPONo.Length > 0)
                {
                    Arr[0] = "Purchase Order Number " + objTcRepair.sPurchaseOrderNo.ToUpper() + " Already Exists";
                    Arr[1] = "2";
                    return Arr;
                }
                string[] divcode;
                string divcodenew;

                if (objTcRepair.sSupRepId == "2")
                {
                    divcode = objTcRepair.newdivcode.Split('~');
                    divcodenew = objCon.get_value("SELECT \"DIV_CODE\" FROM \"TBLDIVISION\" WHERE \"DIV_NAME\"='" + divcode[1] + "'");

                }
                else
                {
                    string officecode;
                    if (objTcRepair.sCrby == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                    {
                        officecode = objCon.get_value("SELECT \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\"='" + objTcRepair.sStoreId + "' limit 1");
                    }
                    else
                    {
                        officecode = objCon.get_value("SELECT \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\"='" + objTcRepair.sOfficeCode + "' limit 1");
                    }
                    //   string   officecode= objCon.get_value("SELECT \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\"='" + objTcRepair.sOfficeCode + "' limit 1");

                    divcodenew = officecode;

                }
                //OleDbDataReader dr = objCon.Fetch("SELECT RSM_PO_NO FROM TBLREPAIRSENTMASTER WHERE UPPER(RSM_PO_NO)='" + objTcRepair.sPurchaseOrderNo.ToUpper() + "' AND RSM_DIV_CODE='" + objTcRepair.sOfficeCode + "'");
                //if (dr.Read())
                //{
                //    Arr[0] = "Purchase Order Number " + objTcRepair.sPurchaseOrderNo.ToUpper() + " Already Exists";
                //    Arr[1] = "2";
                //    dr.Close();
                //    return Arr;
                //}
                //dr.Close();

                // TR_TYPE :  2----> Repairer   1----> Supplier
                string sRepairMasterId = Convert.ToString(objCon.Get_max_no("RSM_ID", "TBLREPAIRSENTMASTER"));
                if (sTcCodes.Length > 0)
                {
                    strQry = "INSERT INTO \"TBLREPAIRSENTMASTER\" (\"RSM_ID\",\"RSM_ISSUE_DATE\",\"RSM_PO_NO\",\"RSM_PO_DATE\",\"RSM_INV_NO\",\"RSM_INV_DATE\",\"RSM_GUARANTY_TYPE\",\"RSM_SUPREP_TYPE\",";
                    strQry += " \"RSM_SUPREP_ID\",\"RSM_DIV_CODE\",\"RSM_CRBY\",\"RSM_PO_QNTY\",\"RSM_MANUAL_INV_NO\",\"RSM_OLD_PO_NO\",\"RSM_REMARKS\",\"RSM_NEW_DIV_CODE\") VALUES (";
                    strQry += " :sRepairMasterId,TO_DATE(:sIssueDate,'DD/MM/YYYY'),:sPurchaseOrderNo,";
                    strQry += " TO_DATE(:sPurchaseDate,'DD/MM/YYYY'),:sInvoiceNo,TO_DATE(:sInvoiceDate,'DD/MM/YYYY'),";
                    strQry += " :sGuarantyType,:sType,:sSupRepId,";
                    strQry += " :sOfficeCode,:sCrby,:sQty,:sManualInvoiceNo, ";
                    strQry += " :sOldPONo,:sPORemarks,:divcodenew)";

                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                    NpgsqlCommand.Parameters.AddWithValue("sIssueDate", objTcRepair.sIssueDate);
                    NpgsqlCommand.Parameters.AddWithValue("sPurchaseOrderNo", objTcRepair.sPurchaseOrderNo.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("sPurchaseDate", objTcRepair.sPurchaseDate);

                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceNo", objTcRepair.sInvoiceNo);
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceDate", objTcRepair.sInvoiceDate);
                    NpgsqlCommand.Parameters.AddWithValue("sGuarantyType", objTcRepair.sGuarantyType);
                    NpgsqlCommand.Parameters.AddWithValue("sType", Convert.ToDouble(objTcRepair.sType));
                    NpgsqlCommand.Parameters.AddWithValue("sSupRepId", Convert.ToInt32(objTcRepair.sSupRepId));
                    if (objTcRepair.sCrby == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sStoreId));
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sOfficeCode));
                    }
                    //NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sOfficeCode));
                    NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objTcRepair.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("sQty", Convert.ToDouble(objTcRepair.sQty));
                    NpgsqlCommand.Parameters.AddWithValue("sManualInvoiceNo", objTcRepair.sManualInvoiceNo);
                    NpgsqlCommand.Parameters.AddWithValue("sOldPONo", objTcRepair.sOldPONo);
                    NpgsqlCommand.Parameters.AddWithValue("sPORemarks", objTcRepair.sPORemarks);
                    NpgsqlCommand.Parameters.AddWithValue("divcodenew", Convert.ToInt32(divcodenew));

                    objCon.ExecuteQry(strQry, NpgsqlCommand);
                }

                string[] strDetailVal = sTcCodes.ToArray();
                string sDTrCode = string.Empty;
                string tcCode = string.Empty;

                for (int i = 0; i < strDetailVal.Length; i++)
                {

                    tcCode = strDetailVal[i].Split('~').GetValue(0).ToString();
                    strQry1 = "SELECT \"TC_RWAO_ID\" FROM \"TBLTCMASTER\" WHERE  \"TC_CODE\" = " + tcCode + " ";

                    NpgsqlCommand = new NpgsqlCommand();
                    string wrkawrdid = objCon.get_value(strQry1, NpgsqlCommand);
                    if (wrkawrdid != "")
                    {
                        objTcRepair.rWaId = wrkawrdid;
                    }
                    string sRepairMasterDetailsId = objCon.Get_max_no("RSD_ID", "TBLREPAIRSENTDETAILS").ToString();
                    strQry = " INSERT INTO \"TBLREPAIRSENTDETAILS\" (\"RSD_ID\",\"RSD_RSM_ID\",\"RSD_TC_CODE\",\"RSD_CRBY\",\"RSD_RWAO_ID\") VALUES (:sRepairMasterDetailsId,:sRepairMasterId,";
                    strQry += " :strDetailVal,:sCrby,:rWaId)";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sRepairMasterDetailsId", Convert.ToInt32(sRepairMasterDetailsId));
                    NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                    NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i].Split('~').GetValue(0).ToString()));
                    NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objTcRepair.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("rWaId", Convert.ToInt32(objTcRepair.rWaId));
                    objCon.ExecuteQry(strQry, NpgsqlCommand);





                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=3,\"TC_UPDATED_EVENT\"='REPAIRER ISSUE',\"TC_UPDATED_EVENT_ID\"=:sRepairMasterDetailsId,";
                    strQry += "\"TC_LAST_REPAIRER_ID\" =:sSupRepId where TC_CODE=:strDetailVal";
                    //objCon.Execute(strQry);
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sRepairMasterDetailsId", Convert.ToInt32(sRepairMasterDetailsId));

                    NpgsqlCommand.Parameters.AddWithValue("sSupRepId", Convert.ToInt32(objTcRepair.sSupRepId));
                    NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i].Split('~').GetValue(0).ToString()));



                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_UPDATED_EVENT_ID\" =:sRepairMasterDetailsId,";
                    strQry += "\"TC_LAST_REPAIRER_ID\" =:sSupRepId where \"TC_CODE\" =:strDetailVal";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sRepairMasterDetailsId", Convert.ToInt32(sRepairMasterDetailsId));
                    NpgsqlCommand.Parameters.AddWithValue("sSupRepId", Convert.ToInt32(objTcRepair.sSupRepId));
                    NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i].Split('~').GetValue(0).ToString()));
                    objCon.ExecuteQry(strQry, NpgsqlCommand);

                    sDTrCode += strDetailVal[i].Split('~').GetValue(0).ToString() + ",";
                    bResult = true;

                }
                //objCon.CommitTransaction();


                if (bResult == true)
                {

                    #region WorkFlow

                    clsApproval objApproval = new clsApproval();

                    if (!sDTrCode.StartsWith(","))
                    {
                        sDTrCode = "," + sDTrCode;
                    }
                    if (!sDTrCode.EndsWith(","))
                    {
                        sDTrCode = sDTrCode + ",";
                    }

                    sDTrCode = sDTrCode.Substring(1, sDTrCode.Length - 1);
                    sDTrCode = sDTrCode.Substring(0, sDTrCode.Length - 1);

                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\" =3, \"TC_UPDATED_EVENT\" ='REPAIRER ISSUE',";
                    strQry += " \"TC_LAST_REPAIRER_ID\" ='" + objTcRepair.sSupRepId + "' WHERE \"TC_CODE\" IN (" + sDTrCode + ")";

                    strQry = strQry.Replace("'", "''");


                    objApproval.sFormName = objTcRepair.sFormName;
                    objApproval.sRecordId = sRepairMasterId;
                    objApproval.sOfficeCode = objTcRepair.sOfficeCode;
                    // objApproval.sOfficeCode = clsStoreOffice.Getofficecode(objTcRepair.sOfficeCode);
                    objApproval.sClientIp = objTcRepair.sClientIP;
                    objApproval.sCrby = objTcRepair.sCrby;
                    objApproval.sWFObjectId = objTcRepair.sWFObjectId;
                    objApproval.sDataReferenceId = sDTrCode;
                    NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sOfficeCode",Convert.ToInt32(objTcRepair.sOfficeCode));
                    if (objTcRepair.sCrby == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sStoreId));
                        objApproval.sRefOfficeCode = objCon.get_value("select \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" where \"STO_SM_ID\"=" + objTcRepair.sStoreId + "", NpgsqlCommand);

                        objApproval.sOfficeCode = objTcRepair.sStoreId;
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objTcRepair.sOfficeCode));
                        objApproval.sRefOfficeCode = objCon.get_value("select \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" where \"STO_SM_ID\"=:sOfficeCode", NpgsqlCommand);
                    }
                    // objApproval.sRefOfficeCode = objCon.get_value("select \"STO_OFF_CODE\" from \"TBLSTOREOFFCODE\" where \"STO_SM_ID\"=:sOfficeCode", NpgsqlCommand);

                    objApproval.sQryValues = strQry;

                    objApproval.sMainTable = "TBLREPAIRSENTMASTER";

                    objApproval.sDescription = "Faulty Transformer issue to Supplier / Repairer with Invoice NO " + objTcRepair.sInvoiceNo;

                    objApproval.SaveWorkFlowData(objApproval);
                    //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                    objApproval.SaveWorkflowObjects(objApproval);
                    objApproval.sOfficeCode = objTcRepair.sOfficeCode;
                    #endregion

                    Arr[0] = "Transformers Issued Sucessfully to Repairer/Supplier";
                    Arr[1] = "0";
                }
                else
                {
                    Arr[0] = "No Transformer Exists to Issue for Repairer/Supplier";
                    Arr[1] = "2";
                }

                return Arr;

            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

                return Arr;
            }
        }
        #endregion

        # region Testing Activity

        public DataTable LoadTestOrDeliverPendingDTR(clsDTrRepairActivity objTestPending)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = " SELECT \"RSM_PO_NO\",TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') RSM_ISSUE_DATE, \"RSD_ID\", \"TC_CODE\",\"TC_SLNO\", (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" =  \"TC_MAKE_ID\") TM_NAME, ";
                strQry += " CAST(\"TC_CAPACITY\" AS TEXT) AS CAPACITY, TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,";
                strQry += " (CASE WHEN \"RSM_SUPREP_TYPE\" ='2' THEN (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"= \"RSM_SUPREP_ID\") WHEN ";
                strQry += " \"RSM_SUPREP_TYPE\" ='1' THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\" = \"RSM_SUPREP_ID\" ) END ) SUP_REPNAME,\"RSM_OLD_PO_NO\", \"RSM_REMARKS\"  ";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\", \"TBLTCMASTER\" WHERE  \"RSM_ID\" = \"RSD_RSM_ID\" AND \"RSD_TC_CODE\" = \"TC_CODE\" AND \"TC_CURRENT_LOCATION\" ='3' ";
                strQry += " AND \"RSD_DELIVARY_DATE\" IS NULL AND \"RSD_ID\" NOT IN (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN (1,3,4))";
                strQry += " AND CAST(\"RSM_DIV_CODE\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objTestPending.sOfficeCode) + "%'";

                //strQry = "SELECT RSM_ID,TC_ID,TC_CODE,TC_SLNO,TM_NAME,TO_CHAR(TC_CAPACITY) TC_CAPACITY,TO_CHAR(RSM_ISSUE_DATE,'DD-MON-YYYY') ";
                //strQry += " RSM_ISSUE_DATE,RSM_PO_NO,RSD_ID,";
                //strQry += " (CASE WHEN RSM_SUPREP_TYPE='2' THEN (SELECT TR_NAME FROM TBLTRANSREPAIRER TR WHERE TR.TR_ID=TR_REPAIRER_ID) WHEN ";
                //strQry += " RSM_SUPREP_TYPE='1' THEN (SELECT TS_NAME FROM TBLTRANSSUPPLIER  WHERE TS_ID=TR_REPAIRER_ID) END ) SUP_REPNAME ";
                //strQry += " FROM TBLREPAIRSENTMASTER,TBLREPAIRSENTDETAILS,TBLINSPECTIONDETAILS,TBLTCMASTER,TBLTRANSMAKES WHERE RSD_RSM_ID=RSM_ID AND IND_RSD_ID=RSD_ID AND ";
                //strQry += " TM_ID= TC_MAKE_ID AND TC_CODE=RSD_TC_CODE AND RSD_DELIVARY_DATE IS NULL AND  IND_INSP_RESULT=" + objTestPending.sTestingDone + " ";
                //strQry += " AND TC_LOCATION_ID LIKE '" + objTestPending.sOfficeCode + "%'";

                if (objTestPending.sRepairDetailsId != null)
                {
                    strQry += " AND \"RSD_ID\" IN (" + objTestPending.sRepairDetailsId + ") ";
                }
                if (objTestPending.sRepairerId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sRepairerId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='2'";
                }

                if (objTestPending.sSupplierId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sSupplierId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='1'";
                }
                if (objTestPending.sPurchaseOrderNo.Trim() != "")
                {
                    strQry += " AND UPPER(\"RSM_PO_NO\")='" + objTestPending.sPurchaseOrderNo.ToString().ToUpper() + "'";
                }
                if (objTestPending.sCapacity != null)
                {
                    strQry += " AND \"TC_CAPACITY\" ='" + objTestPending.sCapacity.ToString().ToUpper() + "'";
                }
                if (objTestPending.sMakeId != null)
                {
                    strQry += " AND \"TC_MAKE_ID\" ='" + objTestPending.sMakeId.ToString() + "'";
                }
                //if (objPending.sPendingDays.Trim() != "")
                //{
                //    strQry += " AND ROUND(SYSDATE-TR_ISSUE_DATE) >" + objPending.sPendingDays.ToString() + "";
                //}
                dt = objCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public string[] SaveTestingTCDetails(string[] strRepairDetailsIds, clsDTrRepairActivity objpending, DataTable dt)
        {
            string[] Arr = new string[2];
            string sFilePath = string.Empty;

            try
            {
                string[] strDetailVal = strRepairDetailsIds.ToArray();
                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    if (strDetailVal[i] != null)
                    {
                        byte[] imageData = null;
                        string strQry = string.Empty;




                        NpgsqlParameter docPhoto = new NpgsqlParameter();
                        NpgsqlCommand comd = new NpgsqlCommand();

                        docPhoto.DbType = DbType.Binary;
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            if (dt.Rows[k][1].ToString() == strDetailVal[i].Split('~').GetValue(0).ToString())
                            {
                                imageData = (Byte[])dt.Rows[k][0];
                                if (imageData != null)
                                {
                                    docPhoto.ParameterName = "Document";
                                    docPhoto.Value = imageData;

                                    string sInspectionId = Convert.ToString(objCon.Get_max_no("IND_ID", "TBLINSPECTIONDETAILS"));
                                    strQry = "INSERT INTO \"TBLINSPECTIONDETAILS\" (\"IND_ID\",\"IND_RSD_ID\",\"IND_INSP_BY\",\"IND_INSP_DATE\",\"IND_TEST_LOCATION\",\"IND_INSP_RESULT\",\"IND_REMARKS\",\"IND_CRBY\",\"IND_DOC\")";
                                    strQry += " VALUES ('" + sInspectionId + "','" + strDetailVal[i].Split('~').GetValue(0).ToString().ToUpper() + "','" + objpending.sTestedBy + "',TO_DATE('" + objpending.sTestedOn + "','DD/MM/YYYY'),";
                                    strQry += " '" + objpending.sTestLocation + "','" + strDetailVal[i].Split('~').GetValue(1).ToString().ToUpper() + "','" + strDetailVal[i].Split('~').GetValue(2).ToString().ToUpper() + "','" + objpending.sCrby + "',:Document)";

                                }
                                else
                                {
                                    docPhoto.ParameterName = "Document";
                                    docPhoto.Value = null;

                                    string sInspectionId = Convert.ToString(objCon.Get_max_no("IND_ID", "TBLINSPECTIONDETAILS"));
                                    strQry = "INSERT INTO \"TBLINSPECTIONDETAILS\" (\"IND_ID\",\"IND_RSD_ID\",\"IND_INSP_BY\",\"IND_INSP_DATE\",\"IND_TEST_LOCATION\",\"IND_INSP_RESULT\",\"IND_REMARKS\",\"IND_CRBY\")";
                                    strQry += " VALUES ('" + sInspectionId + "','" + strDetailVal[i].Split('~').GetValue(0).ToString().ToUpper() + "','" + objpending.sTestedBy + "',TO_DATE('" + objpending.sTestedOn + "','DD/MM/YYYY'),";
                                    strQry += " '" + objpending.sTestLocation + "','" + strDetailVal[i].Split('~').GetValue(1).ToString().ToUpper() + "','" + strDetailVal[i].Split('~').GetValue(2).ToString().ToUpper() + "','" + objpending.sCrby + "')";

                                }
                            }
                        }
                        if (docPhoto.Value == null)
                        {
                            docPhoto.ParameterName = "Document";
                            docPhoto.Value = null;

                            string sInspectionId = Convert.ToString(objCon.Get_max_no("IND_ID", "TBLINSPECTIONDETAILS"));
                            strQry = "INSERT INTO \"TBLINSPECTIONDETAILS\" (\"IND_ID\",\"IND_RSD_ID\",\"IND_INSP_BY\",\"IND_INSP_DATE\",\"IND_TEST_LOCATION\",\"IND_INSP_RESULT\",\"IND_REMARKS\",\"IND_CRBY\")";
                            strQry += " VALUES ('" + sInspectionId + "','" + strDetailVal[i].Split('~').GetValue(0).ToString().ToUpper() + "','" + objpending.sTestedBy + "',TO_DATE('" + objpending.sTestedOn + "','DD/MM/YYYY'),";
                            strQry += " '" + objpending.sTestLocation + "','" + strDetailVal[i].Split('~').GetValue(1).ToString().ToUpper() + "','" + strDetailVal[i].Split('~').GetValue(2).ToString().ToUpper() + "','" + objpending.sCrby + "')";

                        }

                        NpgsqlConnection objconn = new NpgsqlConnection();
                        string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + "zxcvb";
                        objconn.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=TestTRM;User Id=postgres;Password =Idea123";  
                        objconn.Open();
                        comd = new NpgsqlCommand(strQry, objconn);
                        if (docPhoto.Value != null)
                        {
                            comd.Parameters.Add(docPhoto);
                        }

                        comd.ExecuteNonQuery();
                        objconn.Close();
                        Arr[0] = "Testing Done Successfully";
                        Arr[1] = "0";

                    }
                }


                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public clsDTrRepairActivity LoadTestedDTR(clsDTrRepairActivity objTestPending)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = " SELECT \"RSM_PO_NO\",TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') RSM_ISSUE_DATE, \"RSD_ID\", \"TC_CODE\",\"TC_SLNO\",";
                strQry += " (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" =  \"TC_MAKE_ID\") TM_NAME, ";
                strQry += " CAST(\"TC_CAPACITY\" AS TEXT) AS CAPACITY, TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,";
                strQry += " (CASE WHEN \"RSM_SUPREP_TYPE\"='2' THEN (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN ";
                strQry += " \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME, ";
                strQry += " \"IND_INSP_BY\",TO_CHAR(\"IND_INSP_DATE\",'DD-MON-YYYY') IND_INSP_DATE,\"IND_TEST_LOCATION\",\"IND_INSP_RESULT\",\"IND_REMARKS\"";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\",\"TBLINSPECTIONDETAILS\" WHERE  \"RSM_ID\" = \"RSD_RSM_ID\" AND ";
                strQry += " \"RSD_TC_CODE\" = \"TC_CODE\" AND \"IND_INSP_RESULT\" = 1 AND \"IND_RSD_ID\"=\"RSD_ID\" AND \"TC_CODE\"=:sTcCode";
                strQry += " AND \"IND_ID\" =:sTestInspectionId";
                NpgsqlCommand.Parameters.AddWithValue("sTcCode", Convert.ToDouble(objTestPending.sTcCode));
                NpgsqlCommand.Parameters.AddWithValue("sTestInspectionId", Convert.ToInt32(objTestPending.sTestInspectionId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objTestPending.dtTestDone = dt;
                    objTestPending.sInspRemarks = Convert.ToString(dt.Rows[0]["IND_REMARKS"]);
                    objTestPending.sTestedBy = Convert.ToString(dt.Rows[0]["IND_INSP_BY"]);
                    objTestPending.sTestedOn = Convert.ToString(dt.Rows[0]["IND_INSP_DATE"]);
                    objTestPending.sTestLocation = Convert.ToString(dt.Rows[0]["IND_TEST_LOCATION"]);
                    objTestPending.sTestResult = Convert.ToString(dt.Rows[0]["IND_INSP_RESULT"]);
                }
                return objTestPending;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTestPending;
            }
        }

        #endregion

        #region Deliver DTR / Recieve DTR


        public DataTable LoadTestingPassedDetails(clsDTrRepairActivity objTestPending)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = " SELECT \"RSM_PO_NO\", \"RSM_ID\", TO_CHAR(\"RSM_PO_DATE\",'DD-MON-YY') AS PODATE, TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YY') AS ISSUEDATE,";
                strQry += " (CASE WHEN \"RSM_SUPREP_TYPE\"='2' THEN (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN ";
                strQry += " \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME,";
                strQry += " \"RSM_PO_QNTY\" PO_QUANTITY, SUM(CASE WHEN \"RSD_DELIVARY_DATE\" IS NULL THEN 1 ELSE 0 END) PENDING_QNTY,";
                strQry += " SUM(CASE WHEN \"RSD_DELIVARY_DATE\" IS NOT NULL THEN 1 ELSE 0 END) DELIVERED_QNTY";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\" WHERE \"RSM_ID\" = \"RSD_RSM_ID\" AND \"RSD_ID\" IN ";  //LIKE :sOfficeCode||'%'

                if (objTestPending.UserId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminUserId"]))
                {
                    strQry += " (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN(1,3,4)) AND CAST(\"RSM_DIV_CODE\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objTestPending.sOfficeCode) + "%'";
                }
                else
                {
                    strQry += " (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN(1,3,4)) AND CAST(\"RSM_DIV_CODE\" AS TEXT) LIKE '" + objTestPending.sOfficeCode + "%'";
                }
                // NpgsqlCommand = new NpgsqlCommand();
                //  NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", objTestPending.sOfficeCode);

                if (objTestPending.sRepairerId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sRepairerId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='2'";
                    // NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sRepairerId", objTestPending.sRepairerId.ToString().ToUpper());
                }

                if (objTestPending.sSupplierId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sSupplierId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='1'";
                    // NpgsqlCommand = new NpgsqlCommand();
                    //NpgsqlCommand.Parameters.AddWithValue("sSupplierId", objTestPending.sSupplierId.ToString().ToUpper());

                }
                if (objTestPending.sPurchaseOrderNo.Trim() != "")
                {
                    strQry += " AND  UPPER(\"RSM_PO_NO\") ='" + objTestPending.sPurchaseOrderNo.ToString().ToUpper() + "' ";
                    //NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sPurchaseOrderNo", objTestPending.sPurchaseOrderNo.ToString().ToUpper());

                }

                strQry += " GROUP BY \"RSM_PO_NO\", \"RSM_PO_QNTY\", TO_CHAR(\"RSM_PO_DATE\",'DD-MON-YY'),";
                strQry += " TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YY'),\"RSM_SUPREP_TYPE\",\"RSM_SUPREP_ID\",\"RSM_ID\"";
                dt = objCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public DataTable LoadPendingForTestingDetails(clsDTrRepairActivity objTestPending)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"RSM_PO_NO\",  TO_CHAR(\"RSM_PO_DATE\",'DD-MON-YY') AS PODATE, TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YY') AS ISSUEDATE,";
                strQry += " (CASE WHEN \"RSM_SUPREP_TYPE\"='2' THEN (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN ";
                strQry += " \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME,";
                strQry += " \"RSM_PO_QNTY\" PO_QUANTITY, SUM(CASE WHEN \"RSD_DELIVARY_DATE\" IS NULL THEN 1 ELSE 0 END) PENDING_QNTY,";
                strQry += " SUM(CASE WHEN \"RSD_DELIVARY_DATE\" IS NOT NULL THEN 1 ELSE 0 END) DELIVERED_QNTY";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\" WHERE \"RSM_ID\"= \"RSD_RSM_ID\" AND \"RSD_ID\" NOT IN ";
                strQry += " (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN (1,3)) AND CAST(\"RSM_DIV_CODE\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objTestPending.sOfficeCode) + "%'";
                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sStoreId", clsStoreOffice.GetStoreID(objTestPending.sOfficeCode));

                if (objTestPending.sRepairerId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sRepairerId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='2'";
                    //  NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sRepairerId", objTestPending.sRepairerId.ToString().ToUpper());

                }

                if (objTestPending.sSupplierId != null)
                {
                    strQry += " AND \"RSM_SUPREP_ID\" ='" + objTestPending.sSupplierId.ToString().ToUpper() + "' AND \"RSM_SUPREP_TYPE\" ='1'";
                    // NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sSupplierId", objTestPending.sSupplierId.ToString().ToUpper());


                }
                if (objTestPending.sPurchaseOrderNo.Trim() != "")
                {
                    strQry += " AND UPPER(\"RSM_PO_NO\")='" + objTestPending.sPurchaseOrderNo.ToString().ToUpper() + "'";
                    //NpgsqlCommand = new NpgsqlCommand();
                    // NpgsqlCommand.Parameters.AddWithValue("sPurchaseOrderNo", objTestPending.sPurchaseOrderNo.ToString().ToUpper());

                }

                strQry += " GROUP BY \"RSM_PO_NO\", \"RSM_PO_QNTY\", TO_CHAR(\"RSM_PO_DATE\",'DD-MON-YY'),";
                strQry += " TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YY'),\"RSM_SUPREP_TYPE\",\"RSM_SUPREP_ID\"";
                dt = objCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public DataTable LoadPendingForRecieve(string sRepairMasterId)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = " SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN '1' ELSE '' END STATUS, CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" ";
                strQry += "THEN (SELECT \"RSD_GUARRENTY_TYPE\" FROM \"TBLREPAIRSENTDETAILS\",\"TBLREPAIRSENTMASTER\" WHERE \"RSD_TC_CODE\"=\"TC_CODE\" and \"RSD_RSM_ID\"=\"RSM_ID\" and \"RSM_ID\" ='" + sRepairMasterId + "' AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL) ";
                strQry += " ELSE '' END WARRENTY_TYPE ,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY')TC_WARANTY_PERIOD,\"TC_WARRENTY\", ";
                strQry += " \"RSM_PO_NO\",TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') \"RSM_ISSUE_DATE\", \"RSD_ID\", CAST(\"TC_CODE\" AS TEXT) TC_CODE,\"TC_SLNO\", ";
                strQry += "(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" =  \"TC_MAKE_ID\") MAKE, (SELECT \"IND_DOC\" FROM \"TBLINSPECTIONDETAILS\" ";
                strQry += " WHERE \"RSD_TC_CODE\"= \"TC_CODE\" and \"RSD_ID\"=\"IND_RSD_ID\" AND \"IND_INSP_RESULT\" IN(1,3))IND_DOC, CAST(\"TC_CAPACITY\" AS TEXT) AS ";
                strQry += " CAPACITY, TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,(CASE WHEN \"RSM_SUPREP_TYPE\" ='2' THEN (SELECT \"TR_NAME\" ";
                strQry += "  FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM ";
                strQry += "  \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME, (SELECT  CASE \"IND_INSP_RESULT\" WHEN 1 THEN 'PASS'";
                strQry += " WHEN 3 THEN 'SCRAP' WHEN 4 THEN 'NONE' END AS STATE  FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_RSD_ID\"=\"RSD_ID\" AND \"IND_INSP_RESULT\" IN(1,3,4) )STATE ";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\" WHERE \"RSM_ID\" ='" + sRepairMasterId + "' AND \"RSM_ID\" = \"RSD_RSM_ID\" AND \"RSD_TC_CODE\" = \"TC_CODE\"";
                strQry += " AND \"RSD_DELIVARY_DATE\" IS NULL AND \"RSD_ID\"  IN ";
                strQry += " (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN(1,3,4))";
                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                dt = objCon.FetchDataTable(strQry);

                return dt;
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        //public DataTable LoadPendingForRecieve(string sRepairMasterId)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        string strQry = string.Empty;

        //        strQry = " SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN '1' ELSE '' END STATUS, CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" ";
        //        strQry += "THEN (SELECT \"RSD_GUARRENTY_TYPE\" FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\"=\"TC_CODE\" AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL) ";
        //        strQry += " ELSE '' END WARRENTY_TYPE ,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY')TC_WARANTY_PERIOD,\"TC_WARRENTY\", ";
        //        strQry += " \"RSM_PO_NO\",TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') \"RSM_ISSUE_DATE\", \"RSD_ID\", CAST(\"TC_CODE\" AS TEXT) TC_CODE,\"TC_SLNO\", ";
        //        strQry += "(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" =  \"TC_MAKE_ID\") MAKE, (SELECT \"IND_DOC\" FROM \"TBLINSPECTIONDETAILS\" ";
        //        strQry += " WHERE \"RSD_TC_CODE\"= \"TC_CODE\" and \"RSD_ID\"=\"IND_RSD_ID\" AND \"IND_INSP_RESULT\" IN(1,3))IND_DOC, CAST(\"TC_CAPACITY\" AS TEXT) AS ";
        //        strQry += " CAPACITY, TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,(CASE WHEN \"RSM_SUPREP_TYPE\" ='2' THEN (SELECT \"TR_NAME\" ";
        //        strQry += "  FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM ";
        //        strQry += "  \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME, (SELECT  CASE \"IND_INSP_RESULT\" WHEN 1 THEN 'PASS'";
        //        strQry += " WHEN 3 THEN 'SCRAP' WHEN 4 THEN 'NONE' END AS STATE  FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_RSD_ID\"=\"RSD_ID\" AND \"IND_INSP_RESULT\" IN(1,3,4) )STATE ";
        //        strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\" WHERE \"RSM_ID\" ='"+ sRepairMasterId + "' AND \"RSM_ID\" = \"RSD_RSM_ID\" AND \"RSD_TC_CODE\" = \"TC_CODE\"";
        //        strQry += " AND \"RSD_DELIVARY_DATE\" IS NULL AND \"RSD_ID\"  IN ";
        //        strQry += " (SELECT \"IND_RSD_ID\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_INSP_RESULT\" IN(1,3,4))";
        //       // NpgsqlCommand = new NpgsqlCommand();
        //       // NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
        //        dt = objCon.FetchDataTable(strQry);

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        objCon.RollBackTrans();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return dt;
        //    }
        //}

        //public string getWarentyStatus(string SDtrCode)
        //{
        //    string sWarentyStatus = string.Empty;
        //    string strQry = string.Empty;
        //    try
        //    {
        //        strQry = "SELECT DISTINCT STATUS FROM (SELECT CASE WHEN SYSDATE < TC_WARANTY_PERIOD THEN (SELECT DISTINCT RSD_WARENTY_PERIOD";
        //        strQry += " FROM TBLREPAIRSENTDETAILS WHERE RSD_WARENTY_PERIOD IS NOT NULL AND RSD_TC_CODE='"+ SDtrCode + "') ELSE '' END ";
        //        strQry += "STATUS FROM TBLTCMASTER,TBLREPAIRSENTDETAILS WHERE TC_CODE=RSD_TC_CODE AND TC_CODE='" + SDtrCode + "')";
        //        sWarentyStatus = objCon.get_value(strQry);
        //        return sWarentyStatus;
        //    }
        //    catch(Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "getWarentyStatus");
        //        return sWarentyStatus;
        //    }
        //}

        public DataTable LoadRecievedTransformers(string sRepairMasterId)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = " SELECT \"RSM_PO_NO\", TO_CHAR(\"RSM_ISSUE_DATE\",'DD-MON-YYYY') RSM_ISSUE_DATE, \"RSD_ID\", \"TC_CODE\",\"TC_SLNO\", (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" =  \"TC_MAKE_ID\") MAKE, ";
                strQry += " CAST(\"TC_CAPACITY\" AS TEXT) AS CAPACITY, TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') TC_MANF_DATE,";
                strQry += " (CASE WHEN \"RSM_SUPREP_TYPE\"='2' THEN (SELECT \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" TR WHERE TR.\"TR_ID\"=\"RSM_SUPREP_ID\") WHEN ";
                strQry += " \"RSM_SUPREP_TYPE\"='1' THEN (SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\"  WHERE \"TS_ID\"=\"RSM_SUPREP_ID\") END ) SUP_REPNAME, ";
                strQry += " CASE \"IND_INSP_RESULT\" WHEN 1 THEN 'PASS' WHEN 3 THEN 'SCRAP' END AS STATE,\"IND_INSP_RESULT\" ";
                strQry += " FROM \"TBLREPAIRSENTMASTER\", \"TBLREPAIRSENTDETAILS\",\"TBLINSPECTIONDETAILS\", \"TBLTCMASTER\" WHERE \"RSM_ID\" =:sRepairMasterId AND \"RSM_ID\" = \"RSD_RSM_ID\" AND \"RSD_TC_CODE\" = \"TC_CODE\"";
                strQry += " AND \"RSD_DELIVARY_DATE\" IS NOT NULL AND \"RSD_ID\"=\"IND_RSD_ID\" ";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        //public string[] SaveDeliverTCDetails(string[] sRepairDetailsId, clsDTrRepairActivity objDeliverpending)
        //{
        //    string[] Arr = new string[2];
        //    try
        //    {
        //        objCon.BeginTransaction();

        //        string[] strDetailVal = sRepairDetailsId.ToArray();
        //        for (int i = 0; i < strDetailVal.Length; i++)
        //        {
        //            if (strDetailVal[i] != null)
        //            {
        //                string strQry = string.Empty;

        //                strQry = "SELECT DISTINCT STATUS FROM (SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN (SELECT DISTINCT ";
        //                strQry += " CAST(\"RSD_WARENTY_PERIOD\" AS TEXT) FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_WARENTY_PERIOD\" IS NOT NULL AND ";
        //                strQry += " \"RSD_TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "') ELSE '' END STATUS FROM \"TBLTCMASTER\",";
        //                strQry += " \"TBLREPAIRSENTDETAILS\" WHERE \"TC_CODE\"=\"RSD_TC_CODE\" AND \"TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "') A";
        //                string res = objCon.get_value(strQry);
        //                if (res == "" || res == null)
        //                {
        //                    strQry = "Update \"TBLREPAIRSENTDETAILS\" SET ";
        //                    strQry += "\"RSD_DELIVARY_DATE\"=to_date('" + objDeliverpending.sDeliverDate + "','DD/MM/YYYY'),";
        //                    strQry += "\"RSD_DELIVER_VER_BY\"='" + objDeliverpending.sVerifiedby.Trim().ToUpper() + "',";
        //                    strQry += "\"RSD_DELIVER_LOCATION\"='" + objDeliverpending.sStoreId + "',";
        //                    strQry += "\"RSD_DELIVER_CHALLEN_NO\"='" + objDeliverpending.sDeliverChallenNo.Trim().ToUpper() + "', ";
        //                    strQry += " \"RSD_RV_NO\"='" + objDeliverpending.sRVNo + "', \"RSD_RV_DATE\" =to_date('" + objDeliverpending.sRVDate + "','DD/MM/YYYY')";
        //                    strQry += ", \"RSD_WARENTY_PERIOD\" ='" + strDetailVal[i].Split('~').GetValue(3).ToString() + "', \"RSD_GUARRENTY_TYPE\" ='" + strDetailVal[i].Split('~').GetValue(4).ToString() + "'";
        //                    strQry += "  WHERE \"RSD_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
        //                    objCon.ExecuteQry(strQry);
        //                }
        //                else
        //                {
        //                    strQry = "Update \"TBLREPAIRSENTDETAILS\" SET ";
        //                    strQry += "\"RSD_DELIVARY_DATE\" =to_date('" + objDeliverpending.sDeliverDate + "','DD/MM/YYYY'),";
        //                    strQry += "\"RSD_DELIVER_VER_BY\" ='" + objDeliverpending.sVerifiedby.Trim().ToUpper() + "',";
        //                    strQry += "\"RSD_DELIVER_LOCATION\" ='" + objDeliverpending.sStoreId + "',";
        //                    strQry += "\"RSD_DELIVER_CHALLEN_NO\" ='" + objDeliverpending.sDeliverChallenNo.Trim().ToUpper() + "', ";
        //                    strQry += " \"RSD_RV_NO\" ='" + objDeliverpending.sRVNo + "', \"RSD_RV_DATE\" =to_date('" + objDeliverpending.sRVDate + "','DD/MM/YYYY')";
        //                    //strQry += ",RSD_WARENTY_PERIOD='"+ objDeliverpending.sWarrantyPeriod +"',RSD_GUARRENTY_TYPE='"+ objDeliverpending.sGuarantyType +"'";
        //                    strQry += " where \"RSD_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
        //                    objCon.ExecuteQry(strQry);
        //                }

        //                if (strDetailVal[i].Split('~').GetValue(2).ToString() == "1")
        //                {
        //                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=1,\"TC_STORE_ID\"='" + objDeliverpending.sStoreId + "', ";
        //                    strQry += " \"TC_STATUS\"=2,\"TC_UPDATED_EVENT\"='DELIVER TC',\"TC_UPDATED_EVENT_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "' ";
        //                    strQry += " WHERE \"TC_CODE\"='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
        //                    objCon.ExecuteQry(strQry);
        //                }
        //                else if (strDetailVal[i].Split('~').GetValue(2).ToString() == "3")
        //                {
        //                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=1, \"TC_STORE_ID\" ='" + objDeliverpending.sStoreId + "', ";
        //                    strQry += " \"TC_STATUS\" =6, \"TC_UPDATED_EVENT\" ='DELIVER TC', \"TC_UPDATED_EVENT_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "' ";
        //                    strQry += " WHERE \"TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
        //                    objCon.ExecuteQry(strQry);
        //                }
        //                else if (strDetailVal[i].Split('~').GetValue(2).ToString() == "4")
        //                {
        //                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\" =1, \"TC_STORE_ID\" ='" + Convert.ToInt32(objDeliverpending.sStoreId) + "', ";
        //                    strQry += " \"TC_STATUS\"=3,\"TC_UPDATED_EVENT\"='DELIVER TC',\"TC_UPDATED_EVENT_ID\"='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
        //                    strQry += " WHERE \"TC_CODE\"='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
        //                   // NpgsqlCommand = new NpgsqlCommand();
        //                   // NpgsqlCommand.Parameters.AddWithValue("sStoreId", Convert.ToInt32(objDeliverpending.sStoreId));
        //                   // NpgsqlCommand.Parameters.AddWithValue("strDetailVal", strDetailVal[i].Split('~').GetValue(0).ToString());
        //                   // NpgsqlCommand.Parameters.AddWithValue("strDetailVal1", strDetailVal[i].Split('~').GetValue(1).ToString());

        //                    objCon.ExecuteQry(strQry);
        //                }


        //                Arr[0] = "Repaired Transformer Successfully Recieved in Store";
        //                Arr[1] = "0";
        //            }

        //        }

        //        objCon.CommitTransaction();
        //        return Arr;
        //    }
        //    catch (Exception ex)
        //    {
        //        objCon.RollBackTrans();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return Arr;
        //    }
        //}

        #endregion
        //11-09-2020

        public string[] SaveDeliverTCDetails(string[] sRepairDetailsId, clsDTrRepairActivity objDeliverpending)
        {


            string[] Arr = new string[2];
            try
            {
                objCon.BeginTransaction();

                string[] strDetailVal = sRepairDetailsId.ToArray();
                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    if (strDetailVal[i] != null)
                    {
                        string strQry = string.Empty;

                        strQry = "SELECT DISTINCT STATUS FROM (SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN (SELECT DISTINCT ";
                        strQry += " CAST(\"RSD_WARENTY_PERIOD\" AS TEXT) FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_WARENTY_PERIOD\" IS NOT NULL AND ";
                        strQry += " \"RSD_TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "' limit 1 ) ELSE '' END STATUS FROM \"TBLTCMASTER\",";
                        // strQry += " \"RSD_TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "') ELSE '' END STATUS FROM \"TBLTCMASTER\",";
                        strQry += " \"TBLREPAIRSENTDETAILS\" WHERE \"TC_CODE\"=\"RSD_TC_CODE\" AND \"TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "') A";
                        string res = objCon.get_value(strQry);

                        if (res == "" || res == null)
                        {
                            strQry = "Update \"TBLREPAIRSENTDETAILS\" SET ";
                            strQry += "\"RSD_DELIVARY_DATE\"=to_date('" + objDeliverpending.sDeliverDate + "','DD/MM/YYYY'),";
                            strQry += "\"RSD_DELIVER_VER_BY\"='" + objDeliverpending.sVerifiedby.Trim().ToUpper() + "',";
                            strQry += "\"RSD_DELIVER_LOCATION\"='" + objDeliverpending.sStoreId + "',";
                            strQry += "\"RSD_DELIVER_CHALLEN_NO\"='" + objDeliverpending.sDeliverChallenNo.Trim().ToUpper() + "', ";
                            strQry += " \"RSD_RV_NO\"='" + objDeliverpending.sRVNo + "', \"RSD_RV_DATE\" =to_date('" + objDeliverpending.sRVDate + "','DD/MM/YYYY')";
                            strQry += ", \"RSD_WARENTY_PERIOD\" ='" + strDetailVal[i].Split('~').GetValue(3).ToString() + "', \"RSD_GUARRENTY_TYPE\" ='" + strDetailVal[i].Split('~').GetValue(4).ToString() + "',";
                            strQry += "\"RSD_PROCESS_FLAG\"=1";
                            strQry += "  WHERE \"RSD_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
                            objCon.ExecuteQry(strQry);
                        }
                        else
                        {
                            strQry = "Update \"TBLREPAIRSENTDETAILS\" SET ";
                            strQry += "\"RSD_DELIVARY_DATE\" =to_date('" + objDeliverpending.sDeliverDate + "','DD/MM/YYYY'),";
                            strQry += "\"RSD_DELIVER_VER_BY\" ='" + objDeliverpending.sVerifiedby.Trim().ToUpper() + "',";
                            strQry += "\"RSD_DELIVER_LOCATION\" ='" + objDeliverpending.sStoreId + "',";
                            strQry += "\"RSD_DELIVER_CHALLEN_NO\" ='" + objDeliverpending.sDeliverChallenNo.Trim().ToUpper() + "', ";
                            strQry += " \"RSD_RV_NO\" ='" + objDeliverpending.sRVNo + "', \"RSD_RV_DATE\" =to_date('" + objDeliverpending.sRVDate + "','DD/MM/YYYY'),";
                            //strQry += ",RSD_WARENTY_PERIOD='"+ objDeliverpending.sWarrantyPeriod +"',RSD_GUARRENTY_TYPE='"+ objDeliverpending.sGuarantyType +"'";
                            strQry += "\"RSD_PROCESS_FLAG\"=1";
                            strQry += " where \"RSD_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
                            objCon.ExecuteQry(strQry);
                        }

                        if (strDetailVal[i].Split('~').GetValue(2).ToString() == "1")
                        {
                            strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=1,\"TC_RWAO_ID\"=null,\"TC_STORE_ID\"='" + objDeliverpending.sStoreId + "', ";
                            strQry += " \"TC_STATUS\"=2,\"TC_UPDATED_EVENT\"='DELIVER TC',\"TC_UPDATED_EVENT_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "' ";
                            strQry += " WHERE \"TC_CODE\"='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
                            objCon.ExecuteQry(strQry);
                        }
                        else if (strDetailVal[i].Split('~').GetValue(2).ToString() == "3")
                        {
                            strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=1,\"TC_RWAO_ID\"=null, \"TC_STORE_ID\" ='" + objDeliverpending.sStoreId + "', ";
                            strQry += " \"TC_STATUS\" =6, \"TC_UPDATED_EVENT\" ='DELIVER TC', \"TC_UPDATED_EVENT_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "' ";
                            strQry += " WHERE \"TC_CODE\" ='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
                            objCon.ExecuteQry(strQry);
                        }
                        else if (strDetailVal[i].Split('~').GetValue(2).ToString() == "4")
                        {
                            strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\" =1, \"TC_STORE_ID\" ='" + Convert.ToInt32(objDeliverpending.sStoreId) + "', ";
                            strQry += " \"TC_STATUS\"=3,\"TC_UPDATED_EVENT\"='DELIVER TC',\"TC_UPDATED_EVENT_ID\"='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'";
                            strQry += " WHERE \"TC_CODE\"='" + strDetailVal[i].Split('~').GetValue(1).ToString() + "'";
                            // NpgsqlCommand = new NpgsqlCommand();
                            // NpgsqlCommand.Parameters.AddWithValue("sStoreId", Convert.ToInt32(objDeliverpending.sStoreId));
                            // NpgsqlCommand.Parameters.AddWithValue("strDetailVal", strDetailVal[i].Split('~').GetValue(0).ToString());
                            // NpgsqlCommand.Parameters.AddWithValue("strDetailVal1", strDetailVal[i].Split('~').GetValue(1).ToString());

                            objCon.ExecuteQry(strQry);
                        }


                        Arr[0] = "Repaired Transformer Successfully Recieved in Store";
                        Arr[1] = "0";
                    }

                }

                objCon.CommitTransaction();
                return Arr;
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }
        public clsDTrRepairActivity GetRepairSentDetails(clsDTrRepairActivity objRepair)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"RSM_ID\",TO_CHAR(\"RSM_ISSUE_DATE\",'DD/MM/YYYY') RSM_ISSUE_DATE,\"RSM_PO_NO\",\"RSM_DIV_CODE\",TO_CHAR(\"RSM_PO_DATE\",'DD/MM/YYYY') RSM_PO_DATE, \"RSM_INV_NO\" ,";
                strQry += " TO_CHAR(\"RSM_INV_DATE\",'DD/MM/YYYY') RSM_INV_DATE,\"RSM_GUARANTY_TYPE\",\"RSM_SUPREP_TYPE\",\"RSM_SUPREP_ID\",\"RSM_MANUAL_INV_NO\",\"RSM_OLD_PO_NO\",\"RSM_REMARKS\" FROM \"TBLREPAIRSENTMASTER\" WHERE \"RSM_ID\" =:sRepairMasterId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objRepair.sStoreId = Convert.ToString(dt.Rows[0]["RSM_DIV_CODE"]);
                    objRepair.sIssueDate = Convert.ToString(dt.Rows[0]["RSM_ISSUE_DATE"]);
                    objRepair.sPurchaseOrderNo = Convert.ToString(dt.Rows[0]["RSM_PO_NO"]);
                    objRepair.sPurchaseDate = Convert.ToString(dt.Rows[0]["RSM_PO_DATE"]);
                    objRepair.sInvoiceNo = Convert.ToString(dt.Rows[0]["RSM_INV_NO"]);
                    objRepair.sInvoiceDate = Convert.ToString(dt.Rows[0]["RSM_INV_DATE"]);
                    objRepair.sGuarantyType = Convert.ToString(dt.Rows[0]["RSM_GUARANTY_TYPE"]);
                    objRepair.sType = Convert.ToString(dt.Rows[0]["RSM_SUPREP_TYPE"]);
                    objRepair.sSupRepId = Convert.ToString(dt.Rows[0]["RSM_SUPREP_ID"]);
                    objRepair.sOldPONo = Convert.ToString(dt.Rows[0]["RSM_OLD_PO_NO"]);
                    objRepair.sPORemarks = Convert.ToString(dt.Rows[0]["RSM_REMARKS"]);

                    objRepair.sManualInvoiceNo = Convert.ToString(dt.Rows[0]["RSM_MANUAL_INV_NO"]);
                }

                return objRepair;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objRepair;
            }
        }

        public clsDTrRepairActivity GetRepairRecieveDetails(clsDTrRepairActivity objRepair)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                strQry = "SELECT TO_CHAR(\"RSD_DELIVARY_DATE\",'DD/MM/YYYY') RSD_DELIVARY_DATE ,\"RSD_DELIVER_VER_BY\",";
                strQry += " \"RSD_DELIVER_LOCATION\",\"RSD_DELIVER_CHALLEN_NO\",\"RSD_RV_NO\",TO_CHAR(\"RSD_RV_DATE\",'DD/MM/YYYY') RSD_RV_DATE FROM ";
                strQry += " \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_ID\" =:sRepairDetailsId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairDetailsId", Convert.ToInt32(objRepair.sRepairDetailsId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objRepair.sDeliverDate = Convert.ToString(dt.Rows[0]["RSD_DELIVARY_DATE"]);
                    objRepair.sVerifiedby = Convert.ToString(dt.Rows[0]["RSD_DELIVER_VER_BY"]);
                    objRepair.sDeliverChallenNo = Convert.ToString(dt.Rows[0]["RSD_DELIVER_CHALLEN_NO"]);
                    objRepair.sRVNo = Convert.ToString(dt.Rows[0]["RSD_RV_NO"]);
                    objRepair.sRVDate = Convert.ToString(dt.Rows[0]["RSD_RV_DATE"]);

                }

                return objRepair;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objRepair;
            }
        }



        public string GetRepairDetailsId(string sRepairMasterId)
        {
            try
            {
                string strQry = string.Empty;

                strQry = "SELECT \"RSD_RSM_ID\" FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_ID\" =:sRepairMasterId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairMasterId", Convert.ToInt32(sRepairMasterId));
                return objCon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;

            }
        }

        public DataTable GetAllImages()
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"IND_DOC\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_DOC\" IS NOT NULL";
                dt = objCon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;

            }
        }

        #region Convert Image to Byte Array
        public static byte[] ConvertImg(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }
        #endregion

        public DataTable GetRepairPoDetails(clsDTrRepairActivity objRepair)
        {
            DataTable dtPoDetails = new DataTable();
            string strQry = string.Empty;
            try
            {
                if (objRepair.sOfficeCode!="")
                {
                    strQry = "SELECT \"TC_CODE\",\"TC_SLNO\",(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\"=\"TC_MAKE_ID\")TM_NAME,";
                    strQry += " CAST(\"TC_CAPACITY\" AS TEXT)TC_CAPACITY,to_char(\"TC_MANF_DATE\",'DD-MON-YYYY')TC_MANF_DATE,\"RSM_GUARANTY_TYPE\",CASE WHEN ";
                    strQry += " \"RSD_DELIVARY_DATE\" IS NULL THEN 'Repair Pending' WHEN \"RSD_DELIVARY_DATE\" IS NOT NULL THEN 'Repair Completed' END ";
                    strQry += "STATUS FROM \"TBLREPAIRSENTMASTER\" INNER JOIN \"TBLREPAIRSENTDETAILS\" ON \"RSM_ID\"=\"RSD_RSM_ID\" INNER JOIN \"TBLTCMASTER\" ON ";
                    strQry += " \"TC_CODE\"=\"RSD_TC_CODE\" AND \"RSM_DIV_CODE\" =:sOfficeCode AND \"RSM_PO_NO\" =:sPurchaseOrderNo";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(objRepair.sOfficeCode));
                    NpgsqlCommand.Parameters.AddWithValue("sPurchaseOrderNo", objRepair.sPurchaseOrderNo);
                    dtPoDetails = objCon.FetchDataTable(strQry, NpgsqlCommand);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return dtPoDetails;
        }
    }
}
