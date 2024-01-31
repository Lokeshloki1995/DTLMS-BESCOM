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
    public class clsDTCTracker
    {
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        string strFormCode = "clsDTCTracker";
        public string sDTCCode { get; set; }
        public string sFromDate { get; set; }
        public string sToDate { get; set; }
        public string sDTCName { get; set; }
        public string sDTRCode { get; set; }
        public string sConnectedLoad { get; set; }
        public DataTable dTracker { get; set; }
        public string sTaskType { get; set; }

        NpgsqlCommand NpgsqlCommand;
        public object GetDTCTrackstatus(clsDTCTracker objTracker)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                //NpgsqlCommand.Parameters.AddWithValue("DTCCode", objTracker.sDTCCode.ToUpper());
                NpgsqlCommand.Parameters.AddWithValue("DTCCode", Convert.ToString(objTracker.sDTCCode.ToUpper()));
                strQry = " SELECT \"DT_CODE\", \"DT_NAME\", \"DT_TC_ID\", \"DT_TOTAL_CON_KW\" FROM \"TBLDTCMAST\" WHERE UPPER(\"DT_CODE\") =:DTCCode";
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objTracker.sConnectedLoad = Convert.ToString(dt.Rows[0]["DT_TOTAL_CON_KW"]);
                    objTracker.sDTCName = Convert.ToString(dt.Rows[0]["DT_NAME"]);
                    objTracker.sDTRCode = Convert.ToString(dt.Rows[0]["DT_TC_ID"]);
                }

                // LOC_TYPE  1--> Store  2---> Field  3----> Repairer

                //DRT_ACT_REFTYPE   1---> New DTC Entry  2---> Failure  3-->After RI

                //NpgsqlCommand.Parameters.AddWithValue("DTCCode1", objTracker.sDTCCode.ToUpper());
                NpgsqlCommand.Parameters.AddWithValue("DTCCode1", Convert.ToString(objTracker.sDTCCode.ToUpper()));
                strQry = "SELECT TO_CHAR(\"DCT_TRANS_DATE\",'DD-MON-YYYY HH:MI AM') AS TRANSDATE, UPPER( CAST(\"DCT_DTR_CODE\" AS TEXT)) DCT_DTR_CODE, \"DCT_DESC\" AS STATUS, \"DCT_ACT_REFTYPE\",";
                strQry += " \"DCT_DTR_STATUS\",\"DCT_ACT_REFNO\",UPPER(\"DCT_DTC_CODE\") DCT_DTC_CODE FROM \"TBLDTCTRANSACTION\" ";
                strQry += "  WHERE UPPER(\"DCT_DTC_CODE\") = :DTCCode1";

              

                if (objTracker.sFromDate.Length > 0)
                {

                    DateTime dFromDate = DateTime.ParseExact(objTracker.sFromDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"DCT_TRANS_DATE\",'YYYYMMDD') >=:FromDate";
                    NpgsqlCommand.Parameters.AddWithValue("FromDate", dFromDate.ToString("yyyyMMdd"));
                }
                if (objTracker.sToDate.Length > 0)
                {
                    DateTime dToDate = DateTime.ParseExact(objTracker.sToDate.Replace('-', '/'), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    strQry += " AND TO_CHAR(\"DCT_TRANS_DATE\",'YYYYMMDD') <= :ToDate ";
                    NpgsqlCommand.Parameters.AddWithValue("ToDate", dToDate.ToString("yyyyMMdd"));
                }

                //Failure
                if (objTracker.sTaskType != null )
                {
                    //NpgsqlCommand.Parameters.AddWithValue("TaskType", objTracker.sTaskType);
                    NpgsqlCommand.Parameters.AddWithValue("TaskType", Convert.ToInt32(objTracker.sTaskType));
                    strQry += " AND \"DCT_ACT_REFTYPE\" =:TaskType";
                }

                strQry += " ORDER BY \"DCT_ID\" DESC";
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


        public string GetDTCIdFromCode(string sDTCCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("DTCCode", sDTCCode);
                strQry = "SELECT \"DT_ID\" FROM \"TBLDTCMAST\" WHERE \"DT_CODE\" =:DTCCode";
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
