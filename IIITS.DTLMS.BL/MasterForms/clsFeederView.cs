using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;


namespace IIITS.DTLMS.BL
{
    public class clsFeederView
    {
     

        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        string strFormCode = "clsFeederView";

        NpgsqlCommand NpgsqlCommand;
        public DataTable LoadFeederMastDet(string sOfficeCode, string strFeederName = "", string strFeederCode = "", string sStationName = "")
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable DtFeederfDet = new DataTable();
            string strQry = string.Empty;
            try
            {
                if (sOfficeCode.Length >= 4)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.SubDivision);
                }

                NpgsqlCommand.Parameters.AddWithValue("offcode", sOfficeCode);
                strQry = "SELECT * FROM (SELECT \"FD_FEEDER_NAME\",\"FD_FEEDER_CODE\",\"FD_FEEDER_ID\", SUBSTR(\"OFF_NAME\",POSITION(':' IN \"OFF_NAME\")+1,LENGTH(\"OFF_NAME\"))  \"OFF_NAME\",(SELECT \"FC_NAME\" FROM \"TBLFEEDERCATEGORY\" WHERE \"FC_ID\"=\"FD_FC_ID\") \"FD_TYPE\",";
                strQry += " (SELECT \"ST_NAME\" FROM \"TBLSTATION\" WHERE \"ST_ID\"=\"FD_ST_ID\" ) \"ST_NAME\"";
                strQry += " FROM \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\",\"VIEW_ALL_OFFICES\" WHERE \"FD_FEEDER_CODE\" IS NOT NULL";
                strQry += " AND \"FDO_FEEDER_ID\"=\"FD_FEEDER_ID\" AND \"OFF_CODE\"=\"FDO_OFFICE_CODE\" AND cast(\"FDO_OFFICE_CODE\" as TEXT) LIKE :offcode||'%' Order BY \"FD_FEEDER_ID\")A WHERE \"FD_FEEDER_ID\" IS NOT NULL AND \"ST_NAME\" IS NOT NULL";
                if (strFeederName != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("feedname", strFeederName.ToUpper());
                    strQry += " AND UPPER(CAST(\"FD_FEEDER_NAME\"AS TEXT)) like :feedname||'%'";
                }
                if (strFeederCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("feedcode", strFeederCode.ToUpper());
                    strQry += " AND CAST(\"FD_FEEDER_CODE\"AS TEXT) like :feedcode||'%'";
                }
                if (sStationName != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("stationname", sStationName.ToUpper());
                    strQry += " AND UPPER(CAST(\"ST_NAME\"AS TEXT)) like :stationname||'%'";
                }
                ///   OleDbDataReader drcorp = Objcon.Fetch(strQry);
                /////     DtFeederfDet.Load(drcorp);
                DtFeederfDet = Objcon.FetchDataTable(strQry, NpgsqlCommand);
                return DtFeederfDet;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtFeederfDet;
            }
        }

        //public DataTable LoadFeederMastDet(string sOfficeCode, string strFeederName = "", string strFeederCode = "", string sStationName = "")
        //{
        //    DataTable DtFeederfDet = new DataTable();
        //    try
        //    {
        //        NpgsqlCommand cmd = new NpgsqlCommand("proc_get_dtcdetails");
        //        cmd.Parameters.AddWithValue("office_code", sOfficeCode);
        //        cmd.Parameters.AddWithValue("feeder_name", strFeederName.ToUpper());
        //        cmd.Parameters.AddWithValue("feeder_code", strFeederCode.ToUpper());
        //        cmd.Parameters.AddWithValue("station_name", sStationName.ToUpper());

        //        DtFeederfDet = Objcon.FetchDataTable(cmd);
        //    }
        //    catch(Exception ex)
        //    {
        //        clsException.LogError("Procedure - proc_get_dtcdetails", ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return DtFeederfDet;
        //}
        }
    }
