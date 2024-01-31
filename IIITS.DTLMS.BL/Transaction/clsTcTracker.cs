﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.OleDb;
namespace IIITS.DTLMS.BL
{
    public class clsTcTracker
    {
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);

        string strFormCode = "clsTcTracker";
        public string sTCCode { get; set; }
        public string sFromDate { get; set; }
        public string sToDate { get; set; }
        public string sTCSlno { get; set; }
        public string sCapacity { get; set; }
        public string sMake { get; set; }
        public string sFailureType { get; set; }
        public string sTaskType { get; set; }
        public DataTable dTracker { get; set; }
        public string sRepairCount { get; set; }

        NpgsqlCommand NpgsqlCommand;
        //public clsTcTracker GetTcTrackstatus(clsTcTracker objTracker)
        //{
        //    NpgsqlCommand = new NpgsqlCommand();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        string strQry = string.Empty;
        //        NpgsqlCommand.Parameters.AddWithValue("TCCode",  objTracker.sTCCode.ToUpper() );
        //        strQry = " SELECT \"TC_CODE\", \"TC_SLNO\", CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" = \"TC_MAKE_ID\" ) ";
        //        strQry += " MAKE FROM \"TBLTCMASTER\" WHERE UPPER(CAST(\"TC_CODE\" AS TEXT)) =:TCCode";
        //        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
        //        if (dt.Rows.Count > 0)
        //        {
        //            objTracker.sTCSlno = Convert.ToString(dt.Rows[0]["TC_SLNO"]);
        //            objTracker.sCapacity = Convert.ToString(dt.Rows[0]["TC_CAPACITY"]);
        //            objTracker.sMake = Convert.ToString(dt.Rows[0]["MAKE"]);
        //        }

        //        NpgsqlCommand.Parameters.AddWithValue("TCCode1", Convert.ToDouble(objTracker.sTCCode));
        //        strQry = "SELECT COUNT(*) FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_DTR_CODE\" =:TCCode1 AND \"DRT_ACT_REFTYPE\" ='5'";
        //        objTracker.sRepairCount = objcon.get_value(strQry, NpgsqlCommand);


        //        // LOC_TYPE  1--> Store  2---> Field  3----> Repairer

        //        //DRT_ACT_REFTYPE   1---> Against Purchase Order  2---> After RI  3-->Failure  4--->Sent To Repairer 
        //        //                  5----> Recieve from Repairer  6----> Scarp Entry   7---> Scrap Disposal    8------> Inspection
        //        //   9-----> DTR Allocation from DTR Allocation Form  

        //        NpgsqlCommand.Parameters.AddWithValue("TCCode2", Convert.ToDouble(objTracker.sTCCode));
        //        strQry = "SELECT TO_CHAR(\"DRT_TRANS_DATE\",'DD-MON-YYYY') AS TRANSDATE, UPPER(CAST(\"DRT_DTR_CODE\" AS TEXT)) DRT_DTR_CODE, ";
        //        strQry += " CASE WHEN \"DRT_LOC_TYPE\" = 2 THEN (SELECT 'SECTION : ' || \"OM_NAME\"   FROM \"TBLOMSECMAST\" WHERE \"OM_CODE\" = \"DRT_LOC_ID\") ";
        //        strQry += " WHEN  \"DRT_LOC_TYPE\" = 1 THEN (SELECT 'STORE : ' || (SELECT  \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE  \"SM_ID\" = \"DRT_LOC_ID\")) ";
        //        //strQry += " WHEN  \"DRT_LOC_TYPE\" = 1 THEN (SELECT 'DIVISION : ' || (SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"SM_OFF_CODE\" = \"DIV_CODE\") || '; ' || 'STORE NAME : ' || \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\" = \"DRT_LOC_ID\")  ";
        //        strQry += " WHEN  \"DRT_LOC_TYPE\" = 3 THEN (SELECT 'DIVISION : ' || (SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"TRO_OFF_CODE\" = \"DIV_CODE\" ) || '; ' ||'REPAIRER NAME : ' || \"TR_NAME\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\" = \"DRT_LOC_ID\" and \"TR_ID\"=\"TRO_TR_ID\")  ";
        //        strQry += " WHEN  \"DRT_LOC_TYPE\" = 4 THEN (SELECT 'SUPPLIER NAME : ' || \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TS_ID\" = \"DRT_LOC_ID\")";
        //        strQry += " ELSE '' END AS LOCATION, \"DRT_DESC\" AS STATUS,\"DRT_ACT_REFTYPE\",\"DRT_ACT_REFNO\",\"DRT_DTR_STATUS\" FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_DTR_CODE\" =:TCCode2";

        //        if (objTracker.sFromDate.Length > 0)
        //        {
        //            DateTime dFromDate = DateTime.ParseExact(objTracker.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //            strQry += " AND TO_CHAR(\"DRT_TRANS_DATE\",'YYYYMMDD') >=:FromDate ";
        //            NpgsqlCommand.Parameters.AddWithValue("FromDate", dFromDate.ToString("yyyyMMdd"));
        //        }
        //        if (objTracker.sToDate.Length > 0)
        //        {
        //            DateTime dToDate = DateTime.ParseExact(objTracker.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //            strQry += " AND TO_CHAR(\"DRT_TRANS_DATE\",'YYYYMMDD') <=:ToDate";
        //            NpgsqlCommand.Parameters.AddWithValue("ToDate", dToDate.ToString("yyyyMMdd"));
        //        }

        //        //Failure
        //        if (objTracker.sTaskType != null && objTracker.sTaskType == "3")
        //        {
        //            strQry += " AND \"DRT_ACT_REFTYPE\" = '3' ";
        //        }
        //        // Commissioning
        //        else if (objTracker.sTaskType != null && objTracker.sTaskType == "1")
        //        {
        //            strQry += "  AND \"DRT_ACT_REFTYPE\" = '1' ";
        //        }
        //        // Dispatch Tc to repairer
        //        else if (objTracker.sTaskType != null && objTracker.sTaskType == "4")
        //        {
        //            strQry += "  AND \"DRT_ACT_REFTYPE\" = '4' ";
        //        }
        //        // Receive DTR from Repairer
        //        else if (objTracker.sTaskType != null && objTracker.sTaskType == "5")
        //        {
        //            strQry += "  AND \"DRT_ACT_REFTYPE\" = '5' ";
        //        }
        //        // De-Commissioning
        //        else if (objTracker.sTaskType != null && objTracker.sTaskType == "2")
        //        {
        //            strQry += " AND \"DRT_ACT_REFTYPE\" = '2' ";
        //        }

        //        strQry += " ORDER BY \"DRT_ID\" DESC";
        //        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
        //        objTracker.dTracker = dt;
        //        return objTracker;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return objTracker;

        //    }
        //}



        public clsTcTracker GetTcTrackstatus(clsTcTracker objTracker)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("TCCode", objTracker.sTCCode.ToUpper());
                strQry = " SELECT \"TC_CODE\", \"TC_SLNO\", CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY,(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\" = \"TC_MAKE_ID\" ) ";
                strQry += " MAKE FROM \"TBLTCMASTER\" WHERE UPPER(CAST(\"TC_CODE\" AS TEXT)) =:TCCode";
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objTracker.sTCSlno = Convert.ToString(dt.Rows[0]["TC_SLNO"]);
                    objTracker.sCapacity = Convert.ToString(dt.Rows[0]["TC_CAPACITY"]);
                    objTracker.sMake = Convert.ToString(dt.Rows[0]["MAKE"]);
                }

                NpgsqlCommand.Parameters.AddWithValue("TCCode1", Convert.ToDouble(objTracker.sTCCode));
                strQry = "SELECT COUNT(*) FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_DTR_CODE\" =:TCCode1 AND \"DRT_ACT_REFTYPE\" ='5'";
                objTracker.sRepairCount = objcon.get_value(strQry, NpgsqlCommand);


                // LOC_TYPE  1--> Store  2---> Field  3----> Repairer

                //DRT_ACT_REFTYPE   1---> Against Purchase Order  2---> After RI  3-->Failure  4--->Sent To Repairer 
                //                  5----> Recieve from Repairer  6----> Scarp Entry   7---> Scrap Disposal    8------> Inspection
                //   9-----> DTR Allocation from DTR Allocation Form  

                NpgsqlCommand.Parameters.AddWithValue("TCCode2", Convert.ToDouble(objTracker.sTCCode));
                NpgsqlCommand.Parameters.AddWithValue("TCCode3", Convert.ToDouble(objTracker.sTCCode));
                strQry = "SELECT TO_CHAR(\"DRT_TRANS_DATE\",'DD-MON-YYYY') AS TRANSDATE, UPPER(CAST(\"DRT_DTR_CODE\" AS TEXT)) DRT_DTR_CODE, ";
                strQry += " CASE WHEN \"DRT_LOC_TYPE\" = 2 THEN (SELECT 'SECTION : ' || \"OM_NAME\"   FROM \"TBLOMSECMAST\" WHERE \"OM_CODE\" = \"DRT_LOC_ID\") ";
                strQry += " WHEN  \"DRT_LOC_TYPE\" = 1 THEN (SELECT 'STORE : ' || (SELECT  \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE  \"SM_ID\" = \"DRT_LOC_ID\")) ";
                //strQry += " WHEN  \"DRT_LOC_TYPE\" = 1 THEN (SELECT 'DIVISION : ' || (SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"SM_OFF_CODE\" = \"DIV_CODE\") || '; ' || 'STORE NAME : ' || \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\" = \"DRT_LOC_ID\")  ";
             
                // code changed to fetch repairer name in tracker
                //strQry += "  WHEN  \"DRT_LOC_TYPE\" = 3 THEN (SELECT 'DIVISION : ' || (SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"TRO_OFF_CODE\" = \"DIV_CODE\" ) || '; ' ||'REPAIRER NAME : ' || \"TR_NAME\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\" = \"DRT_LOC_ID\" and \"TR_ID\"=\"TRO_TR_ID\" and ";
                //strQry += " cast(\"TRO_OFF_CODE\" as text) =(select SUBSTR(cast(\"DRT_LOC_ID\" as text),1,3) from \"TBLDTRTRANSACTION\" where \"DRT_DTR_CODE\"=:TCCode2 and  \"DRT_ACT_REFTYPE\"=2 limit 1) )  ";
                strQry += "  WHEN  \"DRT_LOC_TYPE\" = 3 THEN (SELECT 'DIVISION : ' || (SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"TRO_OFF_CODE\" = \"DIV_CODE\" ) || '; ' ||'REPAIRER NAME : ' || \"TR_NAME\" FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" WHERE \"TR_ID\" = \"DRT_LOC_ID\" and \"TR_ID\"=\"TRO_TR_ID\" and ";
                strQry += " cast(\"TR_ID\" as text) =(select cast(\"DRT_LOC_ID\" as text) from \"TBLDTRTRANSACTION\" where \"DRT_DTR_CODE\"=:TCCode2 and  \"DRT_ACT_REFTYPE\"=4 limit 1) limit 1)  ";
                strQry += " WHEN  \"DRT_LOC_TYPE\" = 4 THEN (SELECT 'SUPPLIER NAME : ' || \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TS_ID\" = \"DRT_LOC_ID\")";
                strQry += " ELSE '' END AS LOCATION, \"DRT_DESC\" AS STATUS,\"DRT_ACT_REFTYPE\",\"DRT_ACT_REFNO\",\"DRT_DTR_STATUS\" FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_DTR_CODE\" =:TCCode3";

                if (objTracker.sFromDate.Length > 0)
                {
                    DateTime dFromDate = DateTime.ParseExact(objTracker.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"DRT_TRANS_DATE\",'YYYYMMDD') >=:FromDate ";
                    NpgsqlCommand.Parameters.AddWithValue("FromDate", dFromDate.ToString("yyyyMMdd"));
                }
                if (objTracker.sToDate.Length > 0)
                {
                    DateTime dToDate = DateTime.ParseExact(objTracker.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"DRT_TRANS_DATE\",'YYYYMMDD') <=:ToDate";
                    NpgsqlCommand.Parameters.AddWithValue("ToDate", dToDate.ToString("yyyyMMdd"));
                }

                //Failure
                if (objTracker.sTaskType != null && objTracker.sTaskType == "3")
                {
                    strQry += " AND \"DRT_ACT_REFTYPE\" = '3' ";
                }
                // Commissioning
                else if (objTracker.sTaskType != null && objTracker.sTaskType == "1")
                {
                    strQry += "  AND \"DRT_ACT_REFTYPE\" = '1' ";
                }
                // Dispatch Tc to repairer
                else if (objTracker.sTaskType != null && objTracker.sTaskType == "4")
                {
                    strQry += "  AND \"DRT_ACT_REFTYPE\" = '4' ";
                }
                // Receive DTR from Repairer
                else if (objTracker.sTaskType != null && objTracker.sTaskType == "5")
                {
                    strQry += "  AND \"DRT_ACT_REFTYPE\" = '5' ";
                }
                // De-Commissioning
                else if (objTracker.sTaskType != null && objTracker.sTaskType == "2")
                {
                    strQry += " AND \"DRT_ACT_REFTYPE\" = '2' ";
                }

                strQry += " ORDER BY \"DRT_ID\" DESC";
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                objTracker.dTracker = dt;
                return objTracker;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTracker;

            }
        }
        public string  GetDTCId(string sMappingId)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("MappingId", Convert.ToInt32(sMappingId));
                strQry = "SELECT \"DT_ID\" FROM \"TBLTRANSDTCMAPPING\",\"TBLDTCMAST\" WHERE \"TM_ID\" =:MappingId AND \"TM_DTC_ID\"=\"DT_CODE\"";
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetTCIdFromCode(string sTCCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("TCCode", Convert.ToDouble(sTCCode));
                strQry = "SELECT \"TC_ID\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:TCCode";
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
      
    }

}
