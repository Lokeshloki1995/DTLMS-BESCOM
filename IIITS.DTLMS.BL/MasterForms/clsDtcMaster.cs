using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsDtcMaster
    {
        string strFormCode = "clsDtcMaster";
        public long lGetMaxMap { get; set; }
        public string lDtcId { get; set; }
        public string sDtcCode { get; set; }
        public string sDtcName { get; set; }
        public string sOMSectionName { get; set; }
        public string iConnectedKW { get; set; }
        public string iConnectedHP { get; set; }
        public string sInternalCode { get; set; }
        public string sPlatformType { get; set; }       
        public string sConnectionDate { get; set; }
        public string sInspectionDate { get; set; }
        public string sServiceDate { get; set; }
        public string sCommisionDate { get; set; }
        public string sFeederChangeDate { get; set; }
        public string iKWHReading { get; set; }
        public string sTCMakeName { get; set; }
        public string sTCCapacity { get; set; }
        public string sTcCode { get; set; }
        public string sTcSlno { get; set; }
        public string sOldTcCode { get; set; }
        public string sCrBy { get; set; }
        public string sHtlinelength { get; set; }
        public string sLtlinelength { get; set; }
        public string sArresters { get; set; }
        public string sGrounding { get; set; }
        public string sHTProtect { get; set; }
        public string sLTProtect { get; set; }
        public string sDTCMeters { get; set; }
        public string sBreakertype { get; set; }
        public string sProjectType { get; set; }

        public string sOfficeCode { get; set; }

        public string sFeederCode { get; set; }
        public string sStation { get; set; }


        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

        NpgsqlCommand NpgsqlCommand;
        internal string sManufactureDate { get; set; }

        public string[] SaveUpdateDtcDetails(clsDtcMaster objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[2];
            try
            {
                string sQryVal = string.Empty;
                string strQry = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("tcslno", objDtcMaster.sTcSlno);
                sQryVal = ObjCon.get_value("select * from \"TBLTCMASTER\" where \"TC_SLNO\"=:tcslno ", NpgsqlCommand);
                if (sQryVal=="" || sQryVal==null)
                {
                    Arr[0] = "Enter Valid TC SlNo ";
                    Arr[1] = "4";
                    return Arr;
                }

                if (objDtcMaster.lDtcId == "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("dtcode", objDtcMaster.sDtcCode);
                    sQryVal = ObjCon.get_value("select \"DT_CODE\" from \"TBLDTCMAST\" where \"DT_CODE\"=:dtcode", NpgsqlCommand);
                    if (sQryVal!="")
                    {
                        Arr[0] = "DTC Code Already Exists";
                        Arr[1] = "4";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("feedercode", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                    sQryVal = ObjCon.get_value("select * from \"TBLFEEDERMAST\" where \"FD_FEEDER_CODE\"=:feedercode", NpgsqlCommand);
                    if (sQryVal=="" || sQryVal==null)
                    {
                        Arr[0] = "Code Does Not Match With The Feeder Code";
                        Arr[1] = "4";
                        return Arr;                        
                    }
                    NpgsqlCommand.Parameters.AddWithValue("omcode", objDtcMaster.sOMSectionName);
                    sQryVal = ObjCon.get_value("select * from \"TBLOMSECMAST\" where \"OM_CODE\"=:omcode", NpgsqlCommand);
                    if (sQryVal == "" || sQryVal == null)
                    {
                        Arr[0] = "Enter Valid O&m Sec ";
                        Arr[1] = "4";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("tccode", Convert.ToDouble(objDtcMaster.sTcCode));
                    sQryVal = ObjCon.get_value("select * from \"TBLTCMASTER\" where \"TC_CODE\"=:tccode", NpgsqlCommand);
                    if (sQryVal == "" || sQryVal == null)
                    {
                        Arr[0] = "Enter Valid TC SlNo ";
                        Arr[1] = "4";
                        return Arr;
                    }

                    objDtcMaster.lDtcId = Convert.ToString(ObjCon.Get_max_no("DT_ID", "TBLDTCMAST"));

                    NpgsqlCommand.Parameters.AddWithValue("feedercode1", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                    string strFeederSlno = ObjCon.get_value("SELECT \"FD_FEEDER_ID\" FROM \"TBLFEEDERMAST\" WHERE \"FD_FEEDER_CODE\"=:feedercode1", NpgsqlCommand);
                    objDtcMaster.lGetMaxMap = ObjCon.Get_max_no("TM_ID", "TBLTRANSDTCMAPPING");
                    string strDTCupdated = "DTC MASTER ENTRY";
                    string strCurrLoc = "2";

                    NpgsqlCommand cmd = new NpgsqlCommand("sp_save_DTCdetails");
                    cmd.Parameters.AddWithValue("dt_id", objDtcMaster.lDtcId);
                    cmd.Parameters.AddWithValue("dt_code", objDtcMaster.sDtcCode);
                    cmd.Parameters.AddWithValue("dt_name", objDtcMaster.sDtcName);
                    cmd.Parameters.AddWithValue("dt_om_slno", objDtcMaster.sOMSectionName);
                    cmd.Parameters.AddWithValue("dt_total_con_kw", objDtcMaster.iConnectedKW);
                    cmd.Parameters.AddWithValue("dt_total_con_hp", objDtcMaster.iConnectedHP);
                    cmd.Parameters.AddWithValue("dt_kwh_reading", objDtcMaster.iKWHReading);
                    cmd.Parameters.AddWithValue("dt_platform", objDtcMaster.sPlatformType);
                    cmd.Parameters.AddWithValue("dt_internal_code", objDtcMaster.sInternalCode);
                    cmd.Parameters.AddWithValue("dt_tc_id", objDtcMaster.sTcCode);
                    cmd.Parameters.AddWithValue("dt_con_date", objDtcMaster.sConnectionDate);
                    cmd.Parameters.AddWithValue("dt_last_insp_date", objDtcMaster.sInspectionDate);
                    cmd.Parameters.AddWithValue("dt_last_service_date", objDtcMaster.sServiceDate);
                    cmd.Parameters.AddWithValue("dt_trans_commision_date", objDtcMaster.sCommisionDate);
                    cmd.Parameters.AddWithValue("dt_fdrchange_date", objDtcMaster.sFeederChangeDate);
                    cmd.Parameters.AddWithValue("dt_fdrslno", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                    cmd.Parameters.AddWithValue("dt_crby", objDtcMaster.sCrBy);
                    cmd.Parameters.AddWithValue("dt_cron", DateTime.Now.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("dt_breaker_type", objDtcMaster.sBreakertype);
                    cmd.Parameters.AddWithValue("dt_dtcmeters", objDtcMaster.sDTCMeters);
                    cmd.Parameters.AddWithValue("dt_ht_protect", objDtcMaster.sHTProtect);
                    cmd.Parameters.AddWithValue("dt_lt_protect", objDtcMaster.sHTProtect);
                    cmd.Parameters.AddWithValue("dt_grounding", objDtcMaster.sGrounding);
                    cmd.Parameters.AddWithValue("dt_arresters", objDtcMaster.sArresters);
                    cmd.Parameters.AddWithValue("dt_lt_line", objDtcMaster.sLtlinelength);
                    cmd.Parameters.AddWithValue("dt_ht_line", objDtcMaster.sHtlinelength);
                    cmd.Parameters.AddWithValue("tm_id", objDtcMaster.lGetMaxMap);
                    cmd.Parameters.AddWithValue("tm_tc_id", objDtcMaster.sTcCode.ToUpper());
                    cmd.Parameters.AddWithValue("tm_dtc_id", objDtcMaster.sDtcCode);
                    cmd.Parameters.AddWithValue("tm_mapping_date", objDtcMaster.sConnectionDate);
                    cmd.Parameters.AddWithValue("tm_crby", objDtcMaster.sCrBy);
                    cmd.Parameters.AddWithValue("tc_updated_event", strDTCupdated);
                    cmd.Parameters.AddWithValue("tc_updated_event_id", lGetMaxMap);
                    cmd.Parameters.AddWithValue("tc_current_location", strCurrLoc);
                    cmd.Parameters.AddWithValue("tc_location_id", objDtcMaster.sOMSectionName);
                    cmd.Parameters.AddWithValue("tc_code", objDtcMaster.sTcCode.ToUpper());
                    cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                    Arr[2] = "pk_id";
                    Arr[1] = "op_id";
                    Arr[0] = "msg";
                    Arr = ObjCon.Execute(cmd, Arr, 3);
                    return Arr;
                }
                else
                {
                    string strCurrLoc = "2";
                    NpgsqlCommand.Parameters.AddWithValue("dtcd", objDtcMaster.sDtcCode);
                    NpgsqlCommand.Parameters.AddWithValue("dtid", Convert.ToInt32(objDtcMaster.lDtcId));
                    sQryVal = ObjCon.get_value("select * from \"TBLDTCMAST\" where \"DT_CODE\"=:dtcd AND \"DT_ID\"<>:dtid", NpgsqlCommand);
                    if (sQryVal!="")
                    {
                        Arr[0] = "DTC With This Id  Already Exists";
                        Arr[1] = "4";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("fdcd", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                    sQryVal = ObjCon.get_value("select * from \"TBLFEEDERMAST\" where \"FD_FEEDER_CODE\"=:fdcd ", NpgsqlCommand);
                    if (sQryVal=="" || sQryVal==null)
                    {
                        Arr[0] = "Code Does Not Match With The Feeder Code";
                        Arr[1] = "4";
                        return Arr;
                    }
                    NpgsqlCommand.Parameters.AddWithValue("omcd", objDtcMaster.sOMSectionName);
                    sQryVal = ObjCon.get_value("select * from \"TBLOMSECMAST\" where \"OM_CODE\"=:omcd ", NpgsqlCommand);
                    if (sQryVal=="" || sQryVal==null)
                    {
                        Arr[0] = "Enter Valid O&m Sec ";
                        Arr[1] = "4";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("tcdc", Convert.ToDouble(objDtcMaster.sTcCode));
                    sQryVal = ObjCon.get_value("select * from \"TBLTCMASTER\" where \"TC_CODE\"=:tcdc ", NpgsqlCommand);
                    if (sQryVal == "" || sQryVal == null)
                    {
                        Arr[0] = "Enter Valid TC SlNo ";
                        Arr[1] = "4";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("dtcid", objDtcMaster.sDtcCode);
                    string sQryValMap = ObjCon.get_value("SELECT COUNT(*) FROM \"TBLTRANSDTCMAPPING\"  WHERE \"TM_DTC_ID\"=:dtcid", NpgsqlCommand);
                    if (sQryValMap!="")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("feedCode1", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                        string strFeederSlno = ObjCon.get_value("SELECT \"FD_FEEDER_ID\" FROM \"TBLFEEDERMAST\" WHERE \"FD_FEEDER_CODE\"=:feedCode1", NpgsqlCommand);
                        NpgsqlCommand.Parameters.AddWithValue("TcCode1", Convert.ToDouble(objDtcMaster.sTcCode));
                        string strCount = ObjCon.get_value("select count(*) from \"TBLTRANSDTCMAPPING\" where \"TM_TC_ID\"=:TcCode1 and \"TM_LIVE_FLAG\"=1", NpgsqlCommand);
                        if (Convert.ToInt32(strCount) <= 1)
                        {
                            NpgsqlCommand cmdupdate = new NpgsqlCommand();
                            cmdupdate.Parameters.AddWithValue("dt_code", objDtcMaster.sDtcCode);
                            cmdupdate.Parameters.AddWithValue("dt_name", objDtcMaster.sDtcName);
                            cmdupdate.Parameters.AddWithValue("dt_om_slno", objDtcMaster.sOMSectionName);
                            cmdupdate.Parameters.AddWithValue("dt_tc_id", objDtcMaster.sTcCode);
                            cmdupdate.Parameters.AddWithValue("dt_internal_code", objDtcMaster.sInternalCode);
                            cmdupdate.Parameters.AddWithValue("dt_kwh_reading", objDtcMaster.iKWHReading);
                            cmdupdate.Parameters.AddWithValue("dt_platform", objDtcMaster.sPlatformType);
                            cmdupdate.Parameters.AddWithValue("dt_total_con_hp", objDtcMaster.iConnectedHP);
                            cmdupdate.Parameters.AddWithValue("dt_total_con_kw", objDtcMaster.iConnectedKW);
                            cmdupdate.Parameters.AddWithValue("dt_last_insp_date", objDtcMaster.sInspectionDate);
                            cmdupdate.Parameters.AddWithValue("dt_last_service_date", objDtcMaster.sServiceDate);
                            cmdupdate.Parameters.AddWithValue("dt_trans_commision_date", objDtcMaster.sCommisionDate                       );
                            cmdupdate.Parameters.AddWithValue("dt_fdrchange_date", objDtcMaster.sFeederChangeDate);
                            cmdupdate.Parameters.AddWithValue("dt_fdrslno", objDtcMaster.sDtcCode.ToString().Substring(0, 4));
                            cmdupdate.Parameters.AddWithValue("dt_breaker_type", objDtcMaster.sBreakertype);
                            cmdupdate.Parameters.AddWithValue("dt_dtcmeters", objDtcMaster.sDTCMeters);
                            cmdupdate.Parameters.AddWithValue("dt_ht_protect", objDtcMaster.sHTProtect);
                            cmdupdate.Parameters.AddWithValue("dt_lt_protect", objDtcMaster.sLTProtect);
                            cmdupdate.Parameters.AddWithValue("dt_grounding", objDtcMaster.sGrounding);
                            cmdupdate.Parameters.AddWithValue("dt_arresters", objDtcMaster.sArresters);
                            cmdupdate.Parameters.AddWithValue("dt_lt_line", objDtcMaster.sLtlinelength);
                            cmdupdate.Parameters.AddWithValue("dt_ht_line", objDtcMaster.sHtlinelength);
                            cmdupdate.Parameters.AddWithValue("dt_con_date", objDtcMaster.sConnectionDate);
                            cmdupdate.Parameters.AddWithValue("dt_id", objDtcMaster.lDtcId);
                            cmdupdate.Parameters.AddWithValue("tm_tc_id", objDtcMaster.sTcCode.ToUpper());
                            cmdupdate.Parameters.AddWithValue("tm_crby", objDtcMaster.sCrBy);
                            cmdupdate.Parameters.AddWithValue("tm_mapping_date", objDtcMaster.sConnectionDate);
                            cmdupdate.Parameters.AddWithValue("tm_dtc_id", objDtcMaster.sDtcCode);
                            cmdupdate.Parameters.AddWithValue("tc_current_location", strCurrLoc);
                            cmdupdate.Parameters.AddWithValue("tc_location_id", objDtcMaster.sOMSectionName);
                            cmdupdate.Parameters.AddWithValue("tc_code", objDtcMaster.sTcCode);
                            cmdupdate.Parameters["pk_id"].Direction = ParameterDirection.Output;
                            cmdupdate.Parameters["op_id"].Direction = ParameterDirection.Output;
                            cmdupdate.Parameters["msg"].Direction = ParameterDirection.Output;

                            Arr[2] = "pk_id";
                            Arr[1] = "op_id";
                            Arr[0] = "msg";
                            Arr = ObjCon.Execute(cmdupdate, Arr, 3);

                            if (objDtcMaster.sTcCode != objDtcMaster.sOldTcCode && objDtcMaster.sOldTcCode!="")
                            {
                                NpgsqlCommand.Parameters.AddWithValue("OldTcCode9", Convert.ToDouble(objDtcMaster.sOldTcCode));
                                ObjCon.ExecuteQry("update \"TBLTCMASTER\" set \"TC_CURRENT_LOCATION\"=1 where \"TC_CODE\"=:OldTcCode9", NpgsqlCommand);
                            }
                            return Arr;
                        }
                        else
                        {
                            Arr[0] = "DTC Cannot be updated as it is not in work, due to failure";
                            Arr[1] = "4";
                            return Arr;
                        }
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

         public DataTable LoadDtcGrid(clsDtcMaster objDTC)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtDtcDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                if (objDTC.sOfficeCode == "" || objDTC.sOfficeCode == null)
                {
                    objDTC.sOfficeCode = "";

                }
                NpgsqlCommand.Parameters.AddWithValue("offcode", objDTC.sOfficeCode);
                strQry = " SELECT * FROM(SELECT \"DT_ID\",cast(\"DT_CODE\" as TEXT) DT_CODE,\"DT_NAME\",CAST(\"DT_TOTAL_CON_KW\" AS TEXT) DT_TOTAL_CON_KW,";
                strQry += " CAST(\"DT_TOTAL_CON_HP\" AS TEXT) DT_TOTAL_CON_HP,CAST(\"TC_CODE\" AS TEXT) TC_CODE, CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY, ";
                strQry += " TO_CHAR(\"DT_LAST_SERVICE_DATE\",'DD-MON-YYYY') DT_LAST_SERVICE_DATE,\"DT_PROJECTTYPE\", ";
                strQry += " \"FD_FEEDER_NAME\" AS \"FEEDER_NAME\",\"DT_PMTDECC_STATUS\" FROM \"TBLDTCMAST\", \"TBLTCMASTER\",\"TBLFEEDERMAST\",\"TBLSTATION\" WHERE \"TC_CODE\" = \"DT_TC_ID\" ";
                strQry += " AND CAST(\"DT_FDRSLNO\" AS TEXT)=\"FD_FEEDER_CODE\" AND \"ST_ID\"=\"FD_ST_ID\" ";
                //strQry += "  AND cast(\"DT_OM_SLNO\" as TEXT) LIKE :offcode||'%' and  \"DT_TC_ID\" <>0 and \"TC_CAPACITY\"<>0";
                strQry += "  AND cast(\"DT_OM_SLNO\" as TEXT) LIKE :offcode||'%' ";


                if (objDTC.sFeederCode != null && objDTC.sFeederCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("feedcode", objDTC.sFeederCode);
                    // strQry += " AND \"DT_FDRSLNO\" LIKE :feedcode||'%'";
                    strQry += " AND \"FD_FEEDER_NAME\" LIKE :feedcode||'%'";
                }
                if (objDTC.sDtcName != null && objDTC.sDtcName != "")
                {
                   // NpgsqlCommand.Parameters.AddWithValue("dt_name", objDTC.sDtcName);
                    // strQry += " AND \"DT_FDRSLNO\" LIKE :feedcode||'%'";
                    strQry += " AND \"DT_NAME\" LIKE '%"+ objDTC.sDtcName + "%'";
                }
                if (objDTC.sProjectType != null && objDTC.sProjectType != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("projecttype", Convert.ToDouble(objDTC.sProjectType));
                    strQry += " AND \"DT_PROJECTTYPE\"=:projecttype";
                }
                if (objDTC.sStation != null && objDTC.sStation != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("station", Convert.ToInt32(objDTC.sStation));
                    strQry += " AND \"ST_ID\"=:station";
                }
                if (objDTC.sDtcCode != null && objDTC.sDtcCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("dtcCode", objDTC.sDtcCode);
                    strQry += " AND \"DT_CODE\"=:dtcCode ";
                }
                if (objDTC.sTcCode != null && objDTC.sTcCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcCode", Convert.ToDouble(objDTC.sTcCode));
                    strQry += " AND \"DT_TC_ID\"=:tcCode ";
                }

                if (objDTC.sOfficeCode == "" || objDTC.sOfficeCode == null)
                {
                    strQry += " ORDER BY \"DT_ID\" DESC  LIMIT 100)a WHERE \"DT_PMTDECC_STATUS\" is null;";
                }
                else
                {
                    strQry += " ORDER BY \"DT_ID\" DESC )a WHERE \"DT_PMTDECC_STATUS\" is null;";
                }

                dtDtcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtDtcDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDtcDetails;
            }

        }



        public object GetDtcDetails(clsDtcMaster objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                DataTable dtDcDetails = new DataTable();
                string strQry = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("dtcId", Convert.ToInt32(objDtcMaster.lDtcId));
                strQry = " SELECT \"DT_ID\",\"DT_CODE\",\"DT_NAME\",\"DT_TC_ID\",\"DT_OM_SLNO\",TO_CHAR(\"DT_TOTAL_CON_KW\") DT_TOTAL_CON_KW,TO_CHAR(\"DT_TOTAL_CON_HP\") DT_TOTAL_CON_HP,TO_CHAR(\"DT_KWH_READING\") DT_KWH_READING,\"DT_PLATFORM\",";
                strQry += " \"DT_INTERNAL_CODE\",\"DT_TC_ID\",to_char(\"DT_CON_DATE\",'dd/MM/yyyy')DT_CON_DATE,to_char(\"DT_LAST_INSP_DATE\",'dd/MM/yyyy')DT_LAST_INSP_DATE,";
                strQry += " to_char(\"DT_LAST_SERVICE_DATE\",'dd/MM/yyyy')DT_LAST_SERVICE_DATE,to_char(\"DT_TRANS_COMMISION_DATE\",'dd/MM/yyyy')DT_TRANS_COMMISION_DATE,";
                strQry += " to_char(\"DT_FDRCHANGE_DATE\",'dd/MM/yyyy')DT_FDRCHANGE_DATE,to_char(\"DT_CON_DATE\",'dd/MM/yyyy') DT_CON_DATE, NVL(\"DT_BREAKER_TYPE\",0) DT_BREAKER_TYPE,  ";
                strQry += "  NVL(\"DT_DTCMETERS\",0) DT_DTCMETERS,  NVL(\"DT_HT_PROTECT\",0) DT_HT_PROTECT, NVL(\"DT_LT_PROTECT\",0) DT_LT_PROTECT, NVL( \"DT_GROUNDING\",0) DT_GROUNDING, ";
                strQry += " NVL(\"DT_ARRESTERS\",0) DT_ARRESTERS, \"DT_LT_LINE\", \"DT_HT_LINE\" FROM ";
                strQry += " \"TBLDTCMAST\" WHERE \"DT_ID\"=:dtcId";

                dtDcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);

                objDtcMaster.lDtcId = Convert.ToString(dtDcDetails.Rows[0]["DT_ID"]);
                objDtcMaster.sDtcCode = Convert.ToString(dtDcDetails.Rows[0]["DT_CODE"]);
                objDtcMaster.sDtcName = Convert.ToString(dtDcDetails.Rows[0]["DT_NAME"]);
                objDtcMaster.sOMSectionName = Convert.ToString(dtDcDetails.Rows[0]["DT_OM_SLNO"]);
                objDtcMaster.iConnectedKW = Convert.ToString(dtDcDetails.Rows[0]["DT_TOTAL_CON_KW"]);
                objDtcMaster.iConnectedHP = Convert.ToString(dtDcDetails.Rows[0]["DT_TOTAL_CON_HP"]);
                objDtcMaster.iKWHReading = Convert.ToString(dtDcDetails.Rows[0]["DT_KWH_READING"]);
                objDtcMaster.sPlatformType = Convert.ToString(dtDcDetails.Rows[0]["DT_PLATFORM"]);
                objDtcMaster.sTcCode = Convert.ToString(dtDcDetails.Rows[0]["DT_TC_ID"]);
                objDtcMaster.sConnectionDate = Convert.ToString(dtDcDetails.Rows[0]["DT_CON_DATE"]);
                objDtcMaster.sInspectionDate = Convert.ToString(dtDcDetails.Rows[0]["DT_LAST_INSP_DATE"]);
                objDtcMaster.sServiceDate = Convert.ToString(dtDcDetails.Rows[0]["DT_LAST_SERVICE_DATE"]);
                objDtcMaster.sCommisionDate = Convert.ToString(dtDcDetails.Rows[0]["DT_TRANS_COMMISION_DATE"]);
                objDtcMaster.sFeederChangeDate = Convert.ToString(dtDcDetails.Rows[0]["DT_FDRCHANGE_DATE"]);
                objDtcMaster.sInternalCode = Convert.ToString(dtDcDetails.Rows[0]["DT_INTERNAL_CODE"]);

                objDtcMaster.sBreakertype = Convert.ToString(dtDcDetails.Rows[0]["DT_BREAKER_TYPE"]);
                objDtcMaster.sDTCMeters = Convert.ToString(dtDcDetails.Rows[0]["DT_DTCMETERS"]);
                objDtcMaster.sHTProtect = Convert.ToString(dtDcDetails.Rows[0]["DT_HT_PROTECT"]);
                objDtcMaster.sLTProtect = Convert.ToString(dtDcDetails.Rows[0]["DT_LT_PROTECT"]);
                objDtcMaster.sGrounding = Convert.ToString(dtDcDetails.Rows[0]["DT_GROUNDING"]);
                objDtcMaster.sArresters = Convert.ToString(dtDcDetails.Rows[0]["DT_ARRESTERS"]);
                objDtcMaster.sLtlinelength = Convert.ToString(dtDcDetails.Rows[0]["DT_LT_LINE"]);
                objDtcMaster.sHtlinelength = Convert.ToString(dtDcDetails.Rows[0]["DT_HT_LINE"]);


                NpgsqlCommand.Parameters.AddWithValue("tccd", Convert.ToDouble(objDtcMaster.sTcCode));
                strQry = "SELECT \"TC_SLNO\" ||  '~' ||  \"TM_NAME\" || '~' || TO_CHAR(\"TC_CAPACITY\") TC_CAPACITY FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\"  WHERE \"TC_MAKE_ID\"= \"TM_ID\" and \"TC_CODE\"=:tccd";

                string sResult = ObjCon.get_value(strQry, NpgsqlCommand);

                if (sResult != "")
                {
                    objDtcMaster.sTcSlno = sResult.Split('~').GetValue(0).ToString();
                    objDtcMaster.sTCMakeName = sResult.Split('~').GetValue(1).ToString();
                    objDtcMaster.sTCCapacity  = sResult.Split('~').GetValue(2).ToString();
                }


                return objDtcMaster;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objDtcMaster;
            }
            
      }

        /// <summary>
        /// To get TC Details Used in DTCMaster Form
        /// </summary>
        /// <param name="objTCMaster"></param>
        /// <returns></returns>
        public object GetTCDetails(clsDtcMaster  objDTCMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("tccd",Convert.ToDouble(objDTCMaster.sTcCode));
                sQry = "SELECT \"TC_SLNO\",\"TC_CODE\",\"TM_NAME\",CAST(\"TC_CAPACITY\" AS TEXT) TC_CAPACITY FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"= \"TM_ID\" and ";
                sQry += "\"TC_CODE\" NOT IN (SELECT \"RSD_TC_CODE\" from \"TBLREPAIRSENTDETAILS\" where \"RSD_DELIVARY_DATE\" is NULL ) AND \"TC_STATUS\"=3 AND  ";
                sQry += "\"TC_CURRENT_LOCATION\" =1 AND \"TC_CODE\"=:tccd";
                dt = ObjCon.FetchDataTable(sQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objDTCMaster.sTcSlno  = dt.Rows[0]["TC_SLNO"].ToString();
                    objDTCMaster.sTCMakeName = dt.Rows[0]["TM_NAME"].ToString();
                    objDTCMaster.sTCCapacity  = dt.Rows[0]["TC_CAPACITY"].ToString();
                    objDTCMaster.sTcCode = dt.Rows[0]["TC_CODE"].ToString();

                }
                return objDTCMaster;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objDTCMaster;
            }
        }

        public string[] GetPONo(String sPoNO)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[2];
            try
            {
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("pono", sPoNO.Trim());
                sQry = "SELECT \"RSM_PO_NO\" FROM \"TBLREPAIRSENTMASTER\" WHERE \"RSM_PO_NO\"=:pono";
                string PO_NO = ObjCon.get_value(sQry, NpgsqlCommand);
                if (PO_NO != "")
                {
                    Arr[0] = PO_NO;
                    Arr[1] = "1";
                }
                else
                {
                    Arr[0] = "Entered Purchase Order Number Not Exist";
                    Arr[1] = "2";
                }
                
                return Arr;
                 
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                Arr[0] = "Somthing went Wrong";
                Arr[1] = "3";
                return Arr;
            }
        }

    }
}
          
