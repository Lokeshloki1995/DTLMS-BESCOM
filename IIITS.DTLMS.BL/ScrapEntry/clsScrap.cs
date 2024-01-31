using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IIITS.PGSQL.DAL;
using System.Data.OleDb;
using Npgsql;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsScrap
    {
        string strFormCode = "clsScrap";
        PGSqlConnection objCon = new PGSqlConnection(Constants.Password);


        public string sMakeId { get; set; }
        public string sStoreId { get; set; }
        public string sSupplierId { get; set; }
        public string sTcId { get; set; }

        public string sScrapId { get; set; }
        public string sScrapDetailsId { get; set; }
        public string sIssueDate { get; set; }
        public string sWorkOrderNo { get; set; }
        public string sWorkOrderDate { get; set; }
        public int sDTrCount { get; set; }
        public string sCrby { get; set; }
        public string sSendTo { get; set; }

        //Tc Details
        public string sTcCode { get; set; }
        public string sTcSlno { get; set; }
        public string sMakeName { get; set; }
        public string sManfDate { get; set; }
        public string sCapacity { get; set; }
        public string sPurchaseDate { get; set; }
        public string sWarrantyPeriod { get; set; }
        public string sSupplierName { get; set; }
        public string sRemarks { get; set; }

        public string sOMNo { get; set; }
        public string sAmount { get; set; }
        public string sDisposeDesc { get; set; }
        public string sStatus { get; set; }
        public string sInvoiceNo { get; set; }
        public string sInvoiceDate { get; set; }

        public string sOfficeCode { get; set; }

        public string sTestResult { get; set; }
        NpgsqlCommand NpgsqlCommand;
        public DataTable LoadTCForScrap(clsScrap objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\",TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"SM_ID\"=\"TC_STORE_ID\" ";
                strQry += " AND \"TC_STATUS\" =7 ";
                strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + objScrap.sOfficeCode + "'";
                if (objScrap.sTcId != null)
                {
                    strQry += " AND \"TC_ID\" IN (" + objScrap.sTcId + ")";
                }
                else
                {
                    //OLD CODE
                    //strQry += " AND \"TC_CURRENT_LOCATION\"=1 AND CAST(\"TC_CODE\" AS TEXT)  IN (SELECT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"='30' AND \"WO_APPROVE_STATUS\" ='0') ";
                    strQry += " AND \"TC_CURRENT_LOCATION\"=1";
                }
                if (objScrap.sCapacity != null)
                {
                    strQry += " AND CAST(\"TC_CAPACITY\" AS TEXT) ='" + objScrap.sCapacity + "' ";
                }
                if (objScrap.sMakeId != null)
                {
                    strQry += " AND CAST(\"TC_MAKE_ID\" AS TEXT) ='" + objScrap.sMakeId + "'";
                }
                if (objScrap.sStoreId != null)
                {
                    strQry += " AND CAST(\"SM_ID\" AS TEXT) ='" + objScrap.sStoreId + "'";
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

        public DataTable LoadFaultTCForScrap(clsScrap objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",CAST(\"TC_CODE\" AS TEXT) \"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\",\"TC_STATUS\",";
                strQry += " (SELECT \"IND_DOC\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_ID\"=(SELECT max(\"IND_ID\") FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_RSD_ID\" IN(SELECT max(\"RSD_ID\") FROM \"TBLREPAIRSENTDETAILS\",\"TBLTCMASTER\" WHERE \"RSD_TC_CODE\"=\"TC_CODE\" AND \"RSD_DELIVARY_DATE\" IS NOT NULL ))) \"IND_DOC\" ,";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\",TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"SM_ID\"=\"TC_STORE_ID\" ";
                strQry += " AND \"TC_STATUS\"=6 ";
                strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) LIKE '" + clsStoreOffice.GetStoreID(objScrap.sOfficeCode) + "'";
                if (objScrap.sTcId != null)
                {
                    strQry += " AND \"TC_ID\" IN (" + objScrap.sTcId + ")";
                }
                else
                {
                    //OLD CODE
                    //strQry += " AND \"TC_CURRENT_LOCATION\"=1 AND CAST(\"TC_CODE\" AS TEXT) NOT IN (SELECT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"='30' AND \"WO_APPROVE_STATUS\" ='0') ";
                    strQry += " AND \"TC_CURRENT_LOCATION\"=1 ";
                }
                if (objScrap.sCapacity != null)
                {
                    strQry += " AND CAST(CAST(\"TC_CAPACITY\" AS INT) AS TEXT) ='" + objScrap.sCapacity + "' ";
                }
                if (objScrap.sMakeId != null)
                {
                    strQry += " AND CAST(\"TC_MAKE_ID\" AS TEXT) ='" + objScrap.sMakeId + "'";
                }
                if (objScrap.sStoreId != null)
                {
                    strQry += " AND CAST(\"SM_ID\" AS TEXT) ='" + objScrap.sStoreId + "'";
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

        public DataTable LoadScrapTc(clsScrap objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                // TC_STATUS--------> 4 Scrap
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\",\"SO_ID\",\"ST_ID\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\",TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLSCRAPOBJECT\",\"TBLSCRAPTC\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"SM_ID\"=\"TC_STORE_ID\"";
                strQry += " AND \"TC_STATUS\"=4  AND \"SO_TC_CODE\" = \"TC_CODE\" AND \"ST_ID\"=\"SO_ST_ID\" ";
                strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + objScrap.sOfficeCode + "'";
                if (objScrap.sTcId != null)
                {
                    strQry += " AND \"TC_ID\" IN (" + objScrap.sTcId + ")";
                }
                else
                {
                    strQry += " AND \"TC_CURRENT_LOCATION\"=1";
                }
                if (objScrap.sCapacity != null)
                {
                    strQry += " AND CAST(\"TC_CAPACITY\" AS TEXT) ='" + objScrap.sCapacity + "' ";
                }
                if (objScrap.sMakeId != null)
                {
                    strQry += " AND CAST(\"TC_MAKE_ID\" AS TEXT) ='" + objScrap.sMakeId + "'";
                }
                if (objScrap.sStoreId != null)
                {
                    strQry += " AND CAST(\"SM_ID\" AS TEXT) ='" + objScrap.sStoreId + "'";
                }

                if (objScrap.sWorkOrderNo != null)
                {
                    strQry += " AND CAST(\"ST_OM_NO\" AS TEXT) LIKE '" + objScrap.sWorkOrderNo + "%'";
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


        public string[] SaveScrapEntry(string[] sTcCodes, clsScrap objScrap)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;

            bool bResult = false;
            try
            {
                if (objScrap.sScrapId == "")
                {
                    objCon.BeginTransaction();

                    string[] strDetailVal = sTcCodes.ToArray();

                    if (objScrap.sOfficeCode.Length > 2)
                    {
                        objScrap.sOfficeCode = objScrap.sOfficeCode.Substring(0, Constants.Division);
                    }

                    if (objScrap.sDTrCount > 0)
                    {
                        objScrap.sScrapId = objCon.Get_max_no("ST_ID", "TBLSCRAPTC").ToString();
                        strQry = "INSERT INTO \"TBLSCRAPTC\"(\"ST_ID\",\"ST_OM_NO\",\"ST_OM_DATE\",\"ST_REMARKS\",\"ST_CRBY\",\"ST_CRON\",\"ST_QTY\",\"ST_DIV_CODE\") VALUES";
                        strQry += " (:sScrapId,:sWorkOrderNo,TO_DATE(:sWorkOrderDate,'DD/MM/YYYY'),";
                        strQry += " :sRemarks,:sCrby,NOW(),:sDTrCount,:sOfficeCode)";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sScrapId", Convert.ToInt32(objScrap.sScrapId));
                        NpgsqlCommand.Parameters.AddWithValue("sWorkOrderNo", objScrap.sWorkOrderNo);
                        NpgsqlCommand.Parameters.AddWithValue("sWorkOrderDate", objScrap.sWorkOrderDate);
                        NpgsqlCommand.Parameters.AddWithValue("sRemarks", objScrap.sRemarks);
                        NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objScrap.sCrby));
                        NpgsqlCommand.Parameters.AddWithValue("sDTrCount", Convert.ToDouble(objScrap.sDTrCount));
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToDouble(objScrap.sOfficeCode));
                        objCon.ExecuteQry(strQry, NpgsqlCommand);
                    }


                    for (int i = 0; i < strDetailVal.Length; i++)
                    {

                        objScrap.sScrapDetailsId = objCon.Get_max_no("SO_ID", "TBLSCRAPOBJECT").ToString();
                        strQry = "INSERT INTO \"TBLSCRAPOBJECT\"(\"SO_ID\",\"SO_ST_ID\",\"SO_TC_CODE\",\"SO_CRBY\",\"SO_CRON\") VALUES";
                        strQry += " (:sScrapDetailsId,:sScrapId,:strDetailVal,:sCrby,NOW())";

                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sScrapDetailsId", Convert.ToInt32(objScrap.sScrapDetailsId));
                        NpgsqlCommand.Parameters.AddWithValue("sScrapId", Convert.ToInt32(objScrap.sScrapId));
                        NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i]));
                        NpgsqlCommand.Parameters.AddWithValue("sRemarks", objScrap.sRemarks);
                        NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objScrap.sCrby));
                        

                        objCon.ExecuteQry(strQry, NpgsqlCommand);

                        //Update Scrap TC Status in TC Master
                        strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_STATUS\"='4' where \"TC_CODE\"=:strDetailVal";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i]));
                       
                        objCon.ExecuteQry(strQry, NpgsqlCommand);

                        bResult = true;

                    }
                    objCon.CommitTransaction();

                    if (bResult == true)
                    {
                        Arr[0] = "Scrap Details Saved Successfully";
                        Arr[1] = "0";
                    }
                    else
                    {
                        Arr[0] = "No Transformer Exists to do Scrap Entry";
                        Arr[1] = "2";
                    }
                    return Arr;

                }
                else
                {
                    return Arr;
                }
            }
            catch (Exception ex)
            {
                objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }
        public string[] SaveScrapDispose(string[] sTcCode, clsScrap objScrap)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            bool bResult = false;
            try
            {

                string[] strDetailVal = sTcCode.ToArray();

                objCon.BeginTransaction();

                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    strQry = "UPDATE \"TBLSCRAPOBJECT\" SET \"SO_AMOUNT\"=:sAmount,\"SO_SEND_TO\" =:sSendTo,";
                    strQry += " \"SO_DISPOSAL_BY\"=:sCrby,\"SO_DISPOSAL_DATE\"= NOW() ,\"SO_DISPOSAL_DESC\" =:sDisposeDesc, ";
                    strQry += " \"SO_INV_NO\" =:sInvoiceNo, \"SO_INV_DATE\" =TO_DATE(:sInvoiceDate,'DD/MM/YYYY')";
                    strQry += " WHERE \"SO_TC_CODE\" =:strDetailVal";

                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sAmount", Convert.ToDouble(objScrap.sAmount));
                    NpgsqlCommand.Parameters.AddWithValue("sSendTo", sSendTo);
                    NpgsqlCommand.Parameters.AddWithValue("sCrby", Convert.ToInt32(objScrap.sCrby));
                    NpgsqlCommand.Parameters.AddWithValue("sDisposeDesc", objScrap.sDisposeDesc);
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceNo", objScrap.sInvoiceNo);

                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceDate", objScrap.sInvoiceDate);
                    NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i]));
                    

                    objCon.ExecuteQry(strQry, NpgsqlCommand);

                    //Update TC Status in TC MaSTER    5--> dISPOSED
                    strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_STATUS\" ='5' WHERE \"TC_CODE\" =:strDetailVal";
                    NpgsqlCommand = new NpgsqlCommand();
                     NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToDouble(strDetailVal[i]));
                    objCon.ExecuteQry(strQry, NpgsqlCommand);

                    bResult = true;

                }
                objCon.CommitTransaction();
                if (bResult == true)
                {
                    Arr[0] = "Scrap Dispose Details Saved Successfully";
                    Arr[1] = "0";
                }
                else
                {
                    Arr[0] = "No Transformer Exists to Dispose";
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
        public clsDTrRepairActivity GetFaultTCDetails(clsDTrRepairActivity objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",TO_CHAR(\"TC_CAPACITY\") \"TC_CAPACITY\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\",TO_CHAR(\"TC_WARANTY_PERIOD\",'DD_MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" ";
                strQry += " AND \"TC_STATUS\"=3 AND \"TC_CURRENT_LOCATION\"<>3 AND CAST(\"TC_CODE\" AS TEXT) =:sTcCode";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode",Convert.ToDouble(objScrap.sTcCode));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objScrap.sTcId = dt.Rows[0]["TC_ID"].ToString();
                    objScrap.sTcCode = dt.Rows[0]["TC_CODE"].ToString();
                    objScrap.sTcSlno = dt.Rows[0]["TC_SLNO"].ToString();
                    objScrap.sMakeName = dt.Rows[0]["TM_NAME"].ToString();
                    objScrap.sManfDate = dt.Rows[0]["TC_MANF_DATE"].ToString();
                    objScrap.sCapacity = dt.Rows[0]["TC_CAPACITY"].ToString();
                    objScrap.sPurchaseDate = dt.Rows[0]["TC_PURCHASE_DATE"].ToString();
                    objScrap.sWarrantyPeriod = dt.Rows[0]["TC_WARANTY_PERIOD"].ToString();
                    objScrap.sSupplierName = dt.Rows[0]["TS_NAME"].ToString();
                }
                return objScrap;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objScrap;
            }
        }

        public clsScrap GetScrapMasterDetails(clsScrap objScrap)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                strQry = "SELECT \"ST_OM_NO\",TO_CHAR(\"ST_OM_DATE\",'DD/MM/YYYY') \"ST_OM_DATE\",\"ST_REMARKS\",\"SO_AMOUNT\",\"SO_DISPOSAL_DESC\",\"SO_SEND_TO\",\"ST_QTY\" FROM \"TBLSCRAPTC\",\"TBLSCRAPOBJECT\" ";
                strQry += " WHERE \"ST_ID\"=\"SO_ST_ID\" AND \"SO_ID\" =:sScrapDetailsId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sScrapDetailsId", Convert.ToInt32(objScrap.sScrapDetailsId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objScrap.sWorkOrderNo = Convert.ToString(dt.Rows[0]["ST_OM_NO"]);
                    objScrap.sWorkOrderDate = Convert.ToString(dt.Rows[0]["ST_OM_DATE"]);
                    objScrap.sRemarks = Convert.ToString(dt.Rows[0]["ST_REMARKS"]);

                    //objScrap.sOMNo = Convert.ToString(dt.Rows[0]["SO_OM_NO"]);
                    objScrap.sAmount = Convert.ToString(dt.Rows[0]["SO_AMOUNT"]);
                    objScrap.sDisposeDesc = Convert.ToString(dt.Rows[0]["SO_DISPOSAL_DESC"]);
                    objScrap.sSendTo = Convert.ToString(dt.Rows[0]["SO_SEND_TO"]);
                    objScrap.sDTrCount = Convert.ToInt32(dt.Rows[0]["ST_QTY"]);
                }
                return objScrap;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objScrap;
            }
        }

        public DataTable LoadScrapGrid(clsScrap objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",TO_CHAR(\"TC_CAPACITY\") \"TC_CAPACITY\",\"SO_ID\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\",TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLSCRAPOBJECT\",\"TBLSCRAPTC\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"SM_ID\"=\"TC_STORE_ID\"";
                strQry += "  AND \"SO_TC_CODE\" = \"TC_CODE\" AND \"ST_ID\"=\"SO_ST_ID\" AND \"SO_ID\" =:sScrapDetailsId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sScrapDetailsId", Convert.ToInt32(objScrap.sScrapDetailsId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);

                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        //Decalre TC As Scrap

        public string[] DeclareTcScrap(string[] sTcCode, clsScrap objScrap)
        {
            string[] Arr = new string[2];
            bool bResult = false;
            try
            {

                string[] strDetailVal = sTcCode.ToArray();
                objCon.BeginTransaction();
                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    string strQry = string.Empty;
                    string sTestRes = string.Empty;
                    if (strDetailVal[i] != null)
                        sTestRes = strDetailVal[i].Split('~').GetValue(1).ToString();
                    if (sTestRes == "1")
                    {
                        //Update TC Status in TC MaSTER    7--> Declared as Scrap
                        strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_STATUS\" ='7' WHERE \"TC_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString().ToUpper() + "'";
                        objCon.ExecuteQry(strQry);
                        bResult = true;
                    }
                    else
                        if (sTestRes == "2")
                        {
                            strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_STATUS\" ='3' WHERE \"TC_ID\" ='" + strDetailVal[i].Split('~').GetValue(0).ToString().ToUpper() + "'";
                            objCon.ExecuteQry(strQry);
                            bResult = true;
                        }
                }
                objCon.CommitTransaction();
                if (bResult == true)
                {
                    Arr[0] = "Testing Done Successfully";
                    Arr[1] = "0";
                }
                else
                {
                    Arr[0] = "No Transformer Exists to Test";
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

        // Test Done For Scrap

        public DataTable LoadAlreadyDone(clsScrap objScrap)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TC_ID\",CAST(\"TC_CODE\" AS TEXT) \"TC_CODE\",\"TC_SLNO\",\"TM_NAME\", TO_CHAR(\"TC_MANF_DATE\",'DD-MON-YYYY') \"TC_MANF_DATE\",CAST(\"TC_CAPACITY\" AS TEXT) \"TC_CAPACITY\",\"TC_STATUS\",";
                strQry += " (SELECT \"IND_DOC\" FROM \"TBLINSPECTIONDETAILS\" WHERE \"IND_RSD_ID\"= (SELECT MAX(\"RSD_ID\") FROM \"TBLREPAIRSENTDETAILS\" WHERE \"RSD_TC_CODE\"=\"TC_CODE\" AND \"RSD_DELIVARY_DATE\" IS NOT NULL)) \"IND_DOC\",";
                strQry += " TO_CHAR(\"TC_PURCHASE_DATE\",'DD-MON-YYYY') \"TC_PURCHASE_DATE\" ,TO_CHAR(\"TC_WARANTY_PERIOD\",'DD-MON-YYYY') \"TC_WARANTY_PERIOD\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TC_SUPPLIER_ID\"=\"TS_ID\") \"TS_NAME\" ";
                strQry += " FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\" WHERE \"TC_MAKE_ID\"=\"TM_ID\" AND \"SM_ID\"=\"TC_STORE_ID\"";
                strQry += " AND \"TC_STATUS\"=7 ";
                strQry += " AND CAST(\"TC_LOCATION_ID\" AS TEXT) = '" + clsStoreOffice.GetStoreID(objScrap.sOfficeCode) + "'";
                if (objScrap.sTcId != null)
                {
                    strQry += " AND \"TC_ID\" IN (" + objScrap.sTcId + ")";
                }
                else
                {
                    //OLD CODE
                    //strQry += " AND \"TC_CURRENT_LOCATION\"=1 AND CAST(\"TC_CODE\" AS TEXT) NOT IN (SELECT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"='30' AND \"WO_APPROVE_STATUS\" ='0') ";
                    strQry += " AND \"TC_CURRENT_LOCATION\"=1";
                }
                if (objScrap.sCapacity != null)
                {
                    strQry += " AND \"TC_CAPACITY\" ='" + objScrap.sCapacity + "' ";
                }
                if (objScrap.sMakeId != null)
                {
                    strQry += " AND \"TC_MAKE_ID\" ='" + objScrap.sMakeId + "'";
                }
                if (objScrap.sStoreId != null)
                {
                    strQry += " AND \"SM_ID\" ='" + objScrap.sStoreId + "'";
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
    }
}
