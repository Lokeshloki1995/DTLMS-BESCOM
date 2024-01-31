using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using System.Data;
using System.Data.OleDb;
using System.Web;
using Npgsql;
using NpgsqlTypes;
using System.Text.RegularExpressions;

namespace IIITS.DTLMS.BL
{
    public class clsDTCCommision
    {
        string strFormCode = "clsDTCCommision";
        public long lGetMaxMap { get; set; }
        public string lDtcId { get; set; }
        public string sDtcCode { get; set; }
        public string sDtcName { get; set; }
        public string sPoNo { get; set; }
        public string sOMSectionName { get; set; }
        public string iConnectedKW { get; set; }
        public string iConnectedHP { get; set; }
        public string sInternalCode { get; set; }
        public string sPlatformType { get; set; }
        public string sConnectionDate { get; set; }
        public string sInspectionDate { get; set; }
        public string sServiceDate { get; set; }
        public string sCommisionDate { get; set; }
        public string sManufactureDate { get; set; }
        public string sFeederChangeDate { get; set; }
        public string iKWHReading { get; set; }
        public string sTCMakeName { get; set; }
        public string sTCMakeId { get; set; }
        public string sTcRating { get; set; }
        public string sTcStoreId { get; set; }
        public string sTCCapacity { get; set; }
        public string sTcCode { get; set; }
        public string sTcSlno { get; set; }
        public string sTcLastserviceDate { get; set; }
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
        public string sLatitude { get; set; }
        public string sLongitude { get; set; }
        public string sProjecttype { get; set; }
        public string sLoadtype { get; set; }
        public string sDepreciation { get; set; }
        public string sWOslno { get; set; }
        public string sOfficeCode { get; set; }
        public string sDTCImagePath { get; set; }
        public string sDTrImagePath { get; set; }
        public string sFeedercode { get; set; }
        public string sGOS { get; set; }
        public string sRAPDRP { get; set; }
        public string sDtAvgLoad { get; set; }
        public string sDtPeakLoad { get; set; }
        public string sDtsurpluscap { get; set; }



        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFOId { get; set; }


        public string sColumnNames { get; set; }
        public string sColumnValues { get; set; }
        public string sTableNames { get; set; }

        public string sQryValues { get; set; }
        public string sDescription { get; set; }
        public string sParameterValues { get; set; }

        public string sWFDataId { get; set; }
        public string sXmlData { get; set; }

        public string sBOId { get; set; }
        public string sTims_Code { get; set; }

        public string sMLAConst { get; set; }
        public string sMPConst { get; set; }
        public string sCircuit1 { get; set; }
        public string Circuit2 { get; set; }
        public string Circuit3 { get; set; }
        public string Circuit4 { get; set; }
        public string sEleInsRateNo { get; set; }
        public string sEleInsDate { get; set; }

        public string sDTCNewcommTTk_flow { get; set; }
        public string sServiceStatus { get; set; }

        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);

        NpgsqlCommand NpgsqlCommand;
        public string[] SaveUpdateDtcDetails(clsDTCCommision objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();

            string[] Arr = new string[3];
            try
            {
                DataTable dt = new DataTable();
                string strQry = string.Empty;
                string Res = string.Empty;

                //dr = ObjCon.Fetch("select * from TBLTCMASTER where (TC_SLNO='" + objDtcMaster.sTcSlno + "' OR TC_SLNO IS NULL) AND  TC_CODE='" + objDtcMaster.sTcCode + "'");
                //if (!dr.Read())
                //{
                //    Arr[0] = "Enter Valid TC SlNo ";
                //    Arr[1] = "2";
                //    return Arr;
                //}
                //dr.Close();

                if (objDtcMaster.sOfficeCode.Length >= 4)
                {
                    objDtcMaster.sOfficeCode = objDtcMaster.sOfficeCode.Substring(0, Constants.SubDivision);
                }

                NpgsqlCommand.Parameters.AddWithValue("feedercode", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                NpgsqlCommand.Parameters.AddWithValue("officecode", objDtcMaster.sOfficeCode);
                strQry = "select * from \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" WHERE \"FD_FEEDER_CODE\"=:feedercode";
                strQry += " AND  \"FD_FEEDER_ID\"=\"FDO_FEEDER_ID\" AND CAST(\"FDO_OFFICE_CODE\" AS TEXT) LIKE :officecode||'%'";
                dt = Objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count < 0)
                {

                    Arr[0] = "Code Does Not Match With The Feeder Code";
                    Arr[1] = "2";
                    return Arr;

                }

                if (objDtcMaster.lDtcId == "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("DtcCode", objDtcMaster.sDtcCode);
                    Res = Objcon.get_value("select \"DT_CODE\" from \"TBLDTCMAST\" where \"DT_CODE\"=:DtcCode", NpgsqlCommand);
                    if (Res != "")
                    {

                        Arr[0] = "DTC Code Already Exists";
                        Arr[1] = "2";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("OMSectionName", Convert.ToInt32(objDtcMaster.sOMSectionName));
                    Res = Objcon.get_value("select \"OM_CODE\" from \"TBLOMSECMAST\" where \"OM_CODE\"=:OMSectionName", NpgsqlCommand);
                    if (Res == "")
                    {
                        Arr[0] = "Enter Valid O&M Section Code ";
                        Arr[1] = "2";
                        return Arr;

                    }
                    NpgsqlCommand.Parameters.AddWithValue("TcCode", Convert.ToDouble(objDtcMaster.sTcCode));
                    Res = Objcon.get_value("select \"TC_CODE\" from \"TBLTCMASTER\" where \"TC_CODE\"=:TcCode", NpgsqlCommand);
                    if (Res == "")
                    {
                        Arr[0] = "Enter Valid DTR Code ";
                        Arr[1] = "2";
                        return Arr;

                    }

                    //Objcon.BeginTransaction();

                    //objDtcMaster.lDtcId = Convert.ToString(Objcon.Get_max_no("\"DT_ID\"", "\"TBLDTCMAST\""));
                    //strQry = "Insert into TBLDTCMAST (DT_ID,DT_CODE,DT_NAME,DT_OM_SLNO,DT_TOTAL_CON_KW,DT_TOTAL_CON_HP,DT_KWH_READING,";
                    //strQry += " DT_INTERNAL_CODE,DT_TC_ID,DT_CON_DATE,DT_LAST_SERVICE_DATE,DT_TRANS_COMMISION_DATE,";
                    //strQry += " DT_FDRCHANGE_DATE,DT_FDRSLNO,DT_CRBY,DT_CRON,DT_WO_ID,DT_PROJECTTYPE) VALUES ('" + objDtcMaster.lDtcId + "','" + objDtcMaster.sDtcCode + "',";
                    //strQry += " '" + objDtcMaster.sDtcName + "','" + objDtcMaster.sOMSectionName + "','" + objDtcMaster.iConnectedKW + "',";
                    //strQry += "'" + objDtcMaster.iConnectedHP + "','" + objDtcMaster.iKWHReading + "',";
                    //strQry += " '" + objDtcMaster.sInternalCode + "','" + objDtcMaster.sTcCode + "',TO_DATE('" + objDtcMaster.sCommisionDate + "','dd/MM/yyyy'),";
                    //strQry += " TO_DATE('" + objDtcMaster.sServiceDate + "','dd/MM/yyyy'),";
                    //strQry += " TO_DATE('" + objDtcMaster.sCommisionDate + "','dd/MM/yyyy'),TO_DATE('" + objDtcMaster.sFeederChangeDate + "','dd/MM/yyyy'), ";
                    //strQry += " '" + objDtcMaster.sDtcCode.ToString().Substring(0, 4) + "','" + objDtcMaster.sCrBy + "',SYSDATE,'" + objDtcMaster.sWOslno + "','" + objDtcMaster.sProjecttype + "' )";
                    //Objcon.ExecuteQry(strQry);

                    NpgsqlCommand.Parameters.AddWithValue("feedercode1", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));
                    string strFeederSlno = Objcon.get_value("SELECT \"FD_FEEDER_ID\" FROM \"TBLFEEDERMAST\" WHERE \"FD_FEEDER_CODE\"=:feedercode1", NpgsqlCommand);
                    NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_dtc_master");
                    cmd.Parameters.AddWithValue("dtc_mast_id", objDtcMaster.lDtcId);
                    cmd.Parameters.AddWithValue("dtc_mast_code", objDtcMaster.sDtcCode);
                    cmd.Parameters.AddWithValue("dtc_mast_name", objDtcMaster.sDtcName);
                    cmd.Parameters.AddWithValue("dtc_mast_om_slno", objDtcMaster.sOMSectionName);
                    cmd.Parameters.AddWithValue("dtc_mast_tot_con_kw", objDtcMaster.iConnectedKW);
                    cmd.Parameters.AddWithValue("dtc_mast_tot_con_hp", objDtcMaster.iConnectedHP);
                    cmd.Parameters.AddWithValue("dtc_mast_kwh_reading", objDtcMaster.iKWHReading);
                    cmd.Parameters.AddWithValue("dtc_mast_internal_code", objDtcMaster.sInternalCode);
                    cmd.Parameters.AddWithValue("dtc_mast_tc_id", objDtcMaster.sTcCode);
                    cmd.Parameters.AddWithValue("dtc_mast_con_date", objDtcMaster.sCommisionDate);
                    cmd.Parameters.AddWithValue("dtc_mast_last_service_date", objDtcMaster.sServiceDate);
                    cmd.Parameters.AddWithValue("dtc_mast_comm_date", objDtcMaster.sCommisionDate);
                    cmd.Parameters.AddWithValue("dtc_mast_fdrchange_date", objDtcMaster.sFeederChangeDate);
                    cmd.Parameters.AddWithValue("dt_crby", objDtcMaster.sCrBy);
                    cmd.Parameters.AddWithValue("dtc_mast_wo_id", objDtcMaster.sWOslno);
                    cmd.Parameters.AddWithValue("dtc_mast_projecttype", objDtcMaster.sProjecttype);
                    cmd.Parameters.AddWithValue("dtc_mast_tm_id", "");
                    cmd.Parameters.AddWithValue("old_tc_code", objDtcMaster.sOldTcCode);
                    cmd.Parameters.AddWithValue("tims_code", objDtcMaster.sTims_Code);
                    cmd.Parameters.AddWithValue("mlaconst", objDtcMaster.sMLAConst);
                    cmd.Parameters.AddWithValue("mpconst", objDtcMaster.sMPConst);
                    cmd.Parameters.AddWithValue("eleins_rateno", objDtcMaster.sEleInsRateNo);
                    cmd.Parameters.AddWithValue("eleins_date", objDtcMaster.sEleInsDate);
                    if (objDtcMaster.sServiceStatus == null || objDtcMaster.sServiceStatus == "")
                    {
                        objDtcMaster.sServiceStatus = "0";
                    }
                    cmd.Parameters.AddWithValue("servicestatus", objDtcMaster.sServiceStatus);
                    cmd.Parameters.AddWithValue("dt_avg_load", objDtcMaster.sDtAvgLoad);
                    cmd.Parameters.AddWithValue("dt_peak_load", objDtcMaster.sDtPeakLoad);
                    cmd.Parameters.AddWithValue("dt_surplus_cap", objDtcMaster.sDtsurpluscap);
                    cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    Arr[0] = "pk_id";
                    Arr[1] = "op_id";
                    Arr[2] = "msg";
                    Arr = Objcon.Execute(cmd, Arr, 3);

                    objDtcMaster.lDtcId = Arr[0].ToString();

                    if (objDtcMaster.sEleInsDate == "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("DtcId", Convert.ToInt32(lDtcId));
                        strQry = "UPDATE \"TBLDTCMAST\" SET \"DT_ELE_DATE\"= NULL WHERE \"DT_ID\"=:DtcId";
                        Objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    if (objDtcMaster.sDTCNewcommTTk_flow == "1")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("DtcId", Convert.ToInt32(lDtcId));
                        strQry = "UPDATE \"TBLDTCMAST\" SET \"DT_NEWDTC_TTK_FLOW\"= 1 WHERE \"DT_ID\"=:DtcId";
                        Objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("DtcId", Convert.ToInt32(lDtcId));
                        strQry = "UPDATE \"TBLDTCMAST\" SET \"DT_NEWDTC_TTK_FLOW\"=0 WHERE \"DT_ID\"=:DtcId";
                        Objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }
                    //objDtcMaster.lGetMaxMap = Objcon.Get_max_no("TM_ID", "TBLTRANSDTCMAPPING");
                    //strQry = "INSERT INTO TBLTRANSDTCMAPPING (TM_ID,TM_TC_ID,TM_DTC_ID,TM_MAPPING_DATE,TM_CRBY) ";
                    //strQry += "VALUES('" + objDtcMaster.lGetMaxMap + "','" + objDtcMaster.sTcCode.ToUpper() + "','" + objDtcMaster.sDtcCode + "',";
                    //strQry += " SYSDATE,'" + objDtcMaster.sCrBy + "')";
                    //Objcon.ExecuteQry(strQry);
                    //strQry = "UPDATE TBLTCMASTER set TC_UPDATED_EVENT='DTC MASTER ENTRY',TC_UPDATED_EVENT_ID='" + lGetMaxMap + "', TC_CURRENT_LOCATION=2, ";
                    //strQry += " TC_LOCATION_ID='" + objDtcMaster.sOMSectionName + "' where TC_CODE='" + objDtcMaster.sTcCode.ToUpper() + "'";
                    //Objcon.Execute(strQry);


                    //strQry = "UPDATE TBLWORKORDER SET WO_REPLACE_FLG='1' WHERE WO_SLNO='"+ objDtcMaster.sWOslno +"'";
                    //Objcon.Execute(strQry);


                    //Workflow / Approval

                    NpgsqlCommand.Parameters.AddWithValue("WOslno", Convert.ToInt32(objDtcMaster.sWOslno));
                    string sDataReferId = Objcon.get_value("SELECT \"IN_NO\" FROM \"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"TI_ID\"=\"IN_TI_NO\" AND \"TI_WO_SLNO\"=:WOslno", NpgsqlCommand);

                    clsApproval objApproval = new clsApproval();
                    objApproval.sFormName = objDtcMaster.sFormName;
                    objApproval.sRecordId = objDtcMaster.lDtcId;
                    objApproval.sOfficeCode = objDtcMaster.sOfficeCode;
                    objApproval.sClientIp = objDtcMaster.sClientIP;
                    objApproval.sCrby = objDtcMaster.sCrBy;
                    objApproval.sWFObjectId = objDtcMaster.sWFOId;
                    objApproval.sRefOfficeCode = objDtcMaster.sOMSectionName;
                    objApproval.sDataReferenceId = sDataReferId;
                    objApproval.sTTKSComstatus= objDtcMaster.sDTCNewcommTTk_flow;
                    objApproval.sDescription = "Commissioning of DTC " + objDtcMaster.sDtcCode;

                    objApproval.SaveWorkflowObjects(objApproval);

                    //Objcon.CommitTransaction();

                    //  objcon.Con.Close();
                    //Arr[0] = "DTC Details Saved Successfully";
                    //Arr[1] = "0";
                    return Arr;
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("DtcCode1", objDtcMaster.sDtcCode);
                    NpgsqlCommand.Parameters.AddWithValue("DtcId1", Convert.ToInt32(objDtcMaster.lDtcId));
                    Res = Objcon.get_value("select \"DT_CODE\" from \"TBLDTCMAST\" where \"DT_CODE\"=:DtcCode1 AND \"DT_ID\"<>:DtcId1", NpgsqlCommand);
                    if (Res != "")
                    {
                        Arr[0] = "DTC With This Id  Already Exists";
                        Arr[1] = "2";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("OMSectionName1", Convert.ToInt32(objDtcMaster.sOMSectionName));
                    Res = Objcon.get_value("select \"OM_CODE\" from \"TBLOMSECMAST\" where \"OM_CODE\"=:OMSectionName1", NpgsqlCommand);
                    if (Res == "")
                    {
                        Arr[0] = "Enter Valid OM Section Code";
                        Arr[1] = "2";
                        return Arr;

                    }

                    NpgsqlCommand.Parameters.AddWithValue("TcCode1", Convert.ToDouble(objDtcMaster.sTcCode));
                    Res = Objcon.get_value("select \"TC_CODE\" from \"TBLTCMASTER\" where \"TC_CODE\"=:TcCode1", NpgsqlCommand);
                    if (Res == "")
                    {
                        Arr[0] = "Enter Valid DTR Code";
                        Arr[1] = "2";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("DtcCode2", objDtcMaster.sDtcCode);
                    Res = Objcon.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\"=:DtcCode2 and \"DF_REPLACE_FLAG\"=0", NpgsqlCommand);
                    if (Res != "")
                    {
                        Arr[0] = "Selected DTC Cannot be updated, due to Declared as Failure/Enhancement";
                        Arr[1] = "2";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("DtcCode3", objDtcMaster.sDtcCode);
                    Res = Objcon.get_value("SELECT COUNT(*) FROM \"TBLTRANSDTCMAPPING\" WHERE \"TM_DTC_ID\"=:DtcCode3", NpgsqlCommand);
                    if (Res != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("feedercode2", objDtcMaster.sDtcCode.ToString().Substring(0, Constants.Feeder));

                        string strFeederSlno = Objcon.get_value("SELECT \"FD_FEEDER_ID\" FROM \"TBLFEEDERMAST\" WHERE \"FD_FEEDER_CODE\"=:feedercode2", NpgsqlCommand);

                        NpgsqlCommand.Parameters.AddWithValue("TcCode2", Convert.ToDouble(objDtcMaster.sTcCode));
                        string strCount = Objcon.get_value("select count(*) from \"TBLTRANSDTCMAPPING\" where \"TM_TC_ID\"=:TcCode2 and \"TM_LIVE_FLAG\"=1", NpgsqlCommand);
                        if (Convert.ToInt32(strCount) <= 1)
                        {
                            //Objcon.BeginTransaction();

                            //strQry = "UPDATE TBLDTCMAST SET DT_CODE='" + objDtcMaster.sDtcCode + "',DT_NAME ='" + objDtcMaster.sDtcName + "',";
                            //strQry += "DT_OM_SLNO='" + objDtcMaster.sOMSectionName + "',DT_TC_ID='" + objDtcMaster.sTcCode + "',DT_INTERNAL_CODE='" + objDtcMaster.sInternalCode + "',";
                            //strQry += "DT_KWH_READING='" + objDtcMaster.iKWHReading + "',DT_TOTAL_CON_HP='" + objDtcMaster.iConnectedHP + "',DT_TOTAL_CON_KW='" + objDtcMaster.iConnectedKW + "',";
                            //strQry += "DT_LAST_SERVICE_DATE=TO_DATE('" + objDtcMaster.sServiceDate + "','DD/MM/YYYY'),DT_TRANS_COMMISION_DATE=TO_DATE('" + objDtcMaster.sCommisionDate + "','DD/MM/YYYY'),";
                            //strQry += "DT_FDRCHANGE_DATE=TO_DATE('" + objDtcMaster.sFeederChangeDate + "','DD/MM/YYYY') ,DT_FDRSLNO='" + objDtcMaster.sDtcCode.ToString().Substring(0, 4) + "', ";
                            //strQry += "DT_CON_DATE=TO_DATE('" + objDtcMaster.sConnectionDate + "','DD/MM/YYYY'),DT_PROJECTTYPE='"+ objDtcMaster.sProjecttype +"' WHERE DT_ID='" + objDtcMaster.lDtcId + "'";

                            //Objcon.ExecuteQry(strQry);

                            //strQry = "UPDATE TBLTRANSDTCMAPPING SET TM_TC_ID='" + objDtcMaster.sTcCode.ToUpper() + "',TM_CRBY='" + objDtcMaster.sCrBy + "'";
                            ////if (objDtcMaster.sConnectionDate != string.Empty)
                            ////{
                            ////    strQry += ",TM_MAPPING_DATE=TO_DATE('" + sConnectionDate + "','DD/MM/YYYY')";
                            ////}
                            //strQry += "where TM_DTC_ID='" + objDtcMaster.sDtcCode + "'";
                            //Objcon.ExecuteQry(strQry);

                            //Objcon.ExecuteQry("UPDATE TBLTCMASTER set TC_CURRENT_LOCATION=2, TC_LOCATION_ID='" + objDtcMaster.sOMSectionName + "' where TC_CODE='" + objDtcMaster.sTcCode + "'");
                            NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_dtc_master");
                            cmd.Parameters.AddWithValue("dtc_mast_id", objDtcMaster.lDtcId);
                            cmd.Parameters.AddWithValue("dtc_mast_code", objDtcMaster.sDtcCode);
                            cmd.Parameters.AddWithValue("dtc_mast_name", objDtcMaster.sDtcName);
                            cmd.Parameters.AddWithValue("dtc_mast_om_slno", objDtcMaster.sOMSectionName);
                            cmd.Parameters.AddWithValue("dtc_mast_tot_con_kw", objDtcMaster.iConnectedKW);
                            cmd.Parameters.AddWithValue("dtc_mast_tot_con_hp", objDtcMaster.iConnectedHP);
                            cmd.Parameters.AddWithValue("dtc_mast_kwh_reading", objDtcMaster.iKWHReading);
                            cmd.Parameters.AddWithValue("dtc_mast_internal_code", objDtcMaster.sInternalCode);
                            cmd.Parameters.AddWithValue("dtc_mast_tc_id", objDtcMaster.sTcCode);
                            cmd.Parameters.AddWithValue("dtc_mast_con_date", objDtcMaster.sCommisionDate);
                            cmd.Parameters.AddWithValue("dtc_mast_last_service_date", objDtcMaster.sServiceDate);
                            cmd.Parameters.AddWithValue("dtc_mast_comm_date", objDtcMaster.sCommisionDate);
                            cmd.Parameters.AddWithValue("dtc_mast_fdrchange_date", objDtcMaster.sFeederChangeDate);
                            cmd.Parameters.AddWithValue("dt_crby", objDtcMaster.sCrBy);
                            cmd.Parameters.AddWithValue("dtc_mast_wo_id", objDtcMaster.sWOslno);
                            cmd.Parameters.AddWithValue("dtc_mast_projecttype", objDtcMaster.sProjecttype);
                            cmd.Parameters.AddWithValue("dtc_mast_tm_id", "");
                            cmd.Parameters.AddWithValue("old_tc_code", objDtcMaster.sOldTcCode);
                            cmd.Parameters.AddWithValue("tims_code", objDtcMaster.sTims_Code);
                            cmd.Parameters.AddWithValue("mlaconst", objDtcMaster.sMLAConst);
                            cmd.Parameters.AddWithValue("mpconst", objDtcMaster.sMPConst);
                            cmd.Parameters.AddWithValue("eleins_rateno", objDtcMaster.sEleInsRateNo);
                            cmd.Parameters.AddWithValue("eleins_date", objDtcMaster.sEleInsDate);
                            cmd.Parameters.AddWithValue("dt_avg_load", objDtcMaster.sDtAvgLoad);
                            cmd.Parameters.AddWithValue("dt_peak_load", objDtcMaster.sDtPeakLoad);
                            cmd.Parameters.AddWithValue("dt_surplus_cap", objDtcMaster.sDtsurpluscap);
                            //cmd.Parameters.AddWithValue("dtc_mast_tc_id", "");
                            cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                            cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                            cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                            cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                            cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                            cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                            Arr[2] = "pk_id";
                            Arr[1] = "op_id";
                            Arr[0] = "msg";
                            Arr = Objcon.Execute(cmd, Arr, 3);

                            objDtcMaster.lDtcId = Arr[2].ToString();

                            if (objDtcMaster.sEleInsDate == "")
                            {
                                NpgsqlCommand.Parameters.AddWithValue("DtcId2", Convert.ToInt32(lDtcId));
                                strQry = "UPDATE \"TBLDTCMAST\" SET \"DT_ELE_DATE\"= NULL WHERE \"DT_ID\"=:DtcId2";
                                Objcon.ExecuteQry(strQry, NpgsqlCommand);
                            }
                            //if (objDtcMaster.sTcCode != objDtcMaster.sOldTcCode && objDtcMaster.sOldTcCode != "")
                            //{

                            //    Objcon.ExecuteQry("UPDATE TBLTCMASTER set TC_CURRENT_LOCATION=1 where TC_CODE='" + objDtcMaster.sOldTcCode + "'");
                            //}
                            //Objcon.CommitTransaction();

                            return Arr;
                        }
                        else
                        {
                            Arr[0] = "DTC Cannot be updated as it is not in work, due to Failure";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }

                }
                return Arr;
            }
            catch (Exception ex)
            {
                //Objcon.RollBackTrans();
                if (!ex.Message.Contains("duplicate key"))
                {
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                }
                return Arr;
            }
        }
        public string[] SaveTCDetails(clsDTCCommision objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            string strQry = string.Empty;
            bool bResult = false;
            string sQryVal = string.Empty;
            try
            {

                //GENERATED ALWAYS AS (ADD_MONTHS(ENTERDATE,IDS))
                NpgsqlCommand cmd1 = new NpgsqlCommand("sp_savetotcmaster");
                cmd1.Parameters.AddWithValue("tc_id", "");            
                cmd1.Parameters.AddWithValue("tc_code", objDtcMaster.sTcCode);
                cmd1.Parameters.AddWithValue("tc_serialno", objDtcMaster.sTcSlno);
                cmd1.Parameters.AddWithValue("tc_makeid", objDtcMaster.sTCMakeId);
                cmd1.Parameters.AddWithValue("tc_capacity", objDtcMaster.sTCCapacity);                                            
                cmd1.Parameters.AddWithValue("tc_supplier_id", "0");
                cmd1.Parameters.AddWithValue("tc_last_service_date", objDtcMaster.sTcLastserviceDate);
                cmd1.Parameters.AddWithValue("tc_manufacture_date", objDtcMaster.sManufactureDate);
                cmd1.Parameters.AddWithValue("tc_curr_loc", "2");
                cmd1.Parameters.AddWithValue("tc_crby", objDtcMaster.sCrBy);
                        
                cmd1.Parameters.AddWithValue("tc_store_id", objDtcMaster.sTcStoreId);
                cmd1.Parameters.AddWithValue("tc_loc_id", objDtcMaster.sOfficeCode);
                cmd1.Parameters.AddWithValue("tc_star_rate", objDtcMaster.sTcRating);            
                cmd1.Parameters.AddWithValue("tc_status", "1");
                cmd1.Parameters.AddWithValue("tc_po_no", objDtcMaster.sPoNo);

                cmd1.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd1.Parameters.Add("op_id", NpgsqlDbType.Text);

                cmd1.Parameters["msg"].Direction = ParameterDirection.Output;
                cmd1.Parameters["op_id"].Direction = ParameterDirection.Output;

                Arr[0] = "msg";
                Arr[1] = "op_id";
                Arr[2] = "msg";
                Arr = Objcon.Execute(cmd1, Arr, 3);

                bResult = true;

            

            if (bResult == true)
            {
                Arr[0] = "Transformers Details Saved Successfully";
                Arr[1] = "0";
            }
            else
            {
                Arr[0] = "No Transformer Exists to Save";
                Arr[1] = "2";
            }
            return Arr;

          }
    
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public string[] GetTcAndSrlNumDetails(clsDTCCommision objDtrCComm)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            string strQry = string.Empty;
            bool bResult = false;
            string sQryVal = string.Empty;
            try
            {
                Arr[1] = "0";
                NpgsqlCommand.Parameters.AddWithValue("tccd", objDtrCComm.sTcCode.ToUpper());
                sQryVal = Objcon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_CODE\" as text)=:tccd", NpgsqlCommand);
                if (sQryVal != "")
                {
                    Arr[2] = objDtrCComm.sTcCode;
                    Arr[1] = "2";
                    Arr[0] = "Transformer Code " + objDtrCComm.sTcCode + "  Already Exist";
                    return Arr;
                }

                NpgsqlCommand.Parameters.AddWithValue("slno1", objDtrCComm.sTcSlno);
                sQryVal = Objcon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_SLNO\" as text)=:slno1", NpgsqlCommand);
                if (sQryVal != "")
                {
                    Arr[2] = objDtrCComm.sTcSlno;
                    Arr[1] = "2";
                    Arr[0] = "Transformer SlNo  " + objDtrCComm.sTcSlno + "  Already Exist";
                    return Arr;

                }
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }

        }

        public string[] SaveUpdateDtcSpecification(clsDTCCommision objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[2];
            try
            {
                string strQry = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("DTCMeters",Convert.ToDouble(objDtcMaster.sDTCMeters));
                NpgsqlCommand.Parameters.AddWithValue("HTProtect", Convert.ToDouble(objDtcMaster.sHTProtect));
                NpgsqlCommand.Parameters.AddWithValue("LTProtect", Convert.ToDouble(objDtcMaster.sLTProtect));
                NpgsqlCommand.Parameters.AddWithValue("Grounding", Convert.ToDouble(objDtcMaster.sGrounding));
                NpgsqlCommand.Parameters.AddWithValue("Arresters", Convert.ToDouble(objDtcMaster.sArresters));
                NpgsqlCommand.Parameters.AddWithValue("Ltlinelength", objDtcMaster.sLtlinelength);
                NpgsqlCommand.Parameters.AddWithValue("Htlinelength", objDtcMaster.sHtlinelength);
                NpgsqlCommand.Parameters.AddWithValue("PlatformType", Convert.ToDouble(objDtcMaster.sPlatformType));
                NpgsqlCommand.Parameters.AddWithValue("Loadtype", Convert.ToDouble(objDtcMaster.sLoadtype));
                NpgsqlCommand.Parameters.AddWithValue("Longitude", objDtcMaster.sLongitude);
                NpgsqlCommand.Parameters.AddWithValue("GOS", Convert.ToDouble(objDtcMaster.sGOS));
                NpgsqlCommand.Parameters.AddWithValue("RAPDRP", Convert.ToDouble(objDtcMaster.sRAPDRP));
                NpgsqlCommand.Parameters.AddWithValue("Latitude", objDtcMaster.sLatitude);
                NpgsqlCommand.Parameters.AddWithValue("Depreciation", objDtcMaster.sDepreciation);
                NpgsqlCommand.Parameters.AddWithValue("Circuit1", objDtcMaster.sCircuit1);
                NpgsqlCommand.Parameters.AddWithValue("Circuit2", objDtcMaster.Circuit2);
                NpgsqlCommand.Parameters.AddWithValue("Circuit3", objDtcMaster.Circuit3);
                NpgsqlCommand.Parameters.AddWithValue("Circuit4",  objDtcMaster.Circuit4 );
                NpgsqlCommand.Parameters.AddWithValue("DtcId", Convert.ToInt32(objDtcMaster.lDtcId));

                strQry = "UPDATE \"TBLDTCMAST\" SET ";
                strQry += "\"DT_DTCMETERS\"= :DTCMeters, \"DT_HT_PROTECT\" = :HTProtect, ";
                strQry += "\"DT_LT_PROTECT\" = :LTProtect, \"DT_GROUNDING\" = :Grounding, \"DT_ARRESTERS\" = :Arresters, ";
                strQry += "\"DT_LT_LINE\" = :Ltlinelength, \"DT_HT_LINE\" = :Htlinelength, \"DT_PLATFORM\"=:PlatformType, ";
                strQry += "\"DT_LOADTYPE\" =:Loadtype, \"DT_LONGITUDE\" = :Longitude,\"DT_GOS\" = :GOS,\"DT_RAPDRP\"=:RAPDRP, ";
                strQry += "\"DT_LATITUDE\" = :Latitude,\"DT_DEPRECIATION\"=:Depreciation, \"DT_CIRCUIT1\"=:Circuit1, ";
                strQry += "\"DT_CIRCUIT2\"=:Circuit2,\"DT_CIRCUIT3\"=:Circuit3,\"DT_CIRCUIT4\"=:Circuit4 WHERE \"DT_ID\"=:DtcId";

                Objcon.ExecuteQry(strQry, NpgsqlCommand);

                Arr[0] = "DTC Details Saved/Updated Successfully";
                Arr[1] = "0";
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }


        public object GetDTCDetails(clsDTCCommision objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                DataTable dtDcDetails = new DataTable();
                string strQry = string.Empty;

                strQry = " SELECT \"DT_ID\",\"DT_CODE\",\"DT_NAME\",\"DT_TC_ID\",\"DT_OM_SLNO\",CAST(\"DT_TOTAL_CON_KW\" AS TEXT) DT_TOTAL_CON_KW,CAST(\"DT_TOTAL_CON_HP\" AS TEXT) \"DT_TOTAL_CON_HP\",CAST(\"DT_KWH_READING\" AS TEXT) \"DT_KWH_READING\",\"DT_PLATFORM\",";
                strQry += " \"DT_INTERNAL_CODE\",\"DT_TC_ID\",to_char(\"DT_CON_DATE\",'dd/MM/yyyy')\"DT_CON_DATE\",to_char(\"DT_LAST_INSP_DATE\",'dd/MM/yyyy')\"DT_LAST_INSP_DATE\",CAST(\"DT_AVG_LOAD\" AS TEXT) \"DT_AVG_LOAD\",CAST(\"DT_PEAK_LOAD\" AS TEXT) \"DT_PEAK_LOAD\",CAST(\"DT_SURPLUS_CAP\" AS TEXT) \"DT_SURPLUS_CAP\",";
                strQry += " to_char(\"DT_LAST_SERVICE_DATE\",'dd/MM/yyyy')\"DT_LAST_SERVICE_DATE\",to_char(\"DT_TRANS_COMMISION_DATE\",'dd/MM/yyyy')\"DT_TRANS_COMMISION_DATE\",";
                strQry += " to_char(\"DT_FDRCHANGE_DATE\",'dd/MM/yyyy')\"DT_FDRCHANGE_DATE\",to_char(\"DT_CON_DATE\",'dd/MM/yyyy') \"DT_CON_DATE\", COALESCE (\"DT_BREAKER_TYPE\",0) DT_BREAKER_TYPE,  ";
                strQry += "  COALESCE (\"DT_DTCMETERS\",0) DT_DTCMETERS,  COALESCE (\"DT_HT_PROTECT\",0) DT_HT_PROTECT, COALESCE (\"DT_LT_PROTECT\",0) DT_LT_PROTECT, COALESCE ( \"DT_GROUNDING\",0) DT_GROUNDING, ";
                strQry += " COALESCE (\"DT_ARRESTERS\",0) DT_ARRESTERS,\"DT_LT_LINE\", \"DT_HT_LINE\", COALESCE (\"DT_LOADTYPE\",0) DT_LOADTYPE, COALESCE (\"DT_PROJECTTYPE\",0) \"DT_PROJECTTYPE\", \"DT_LONGITUDE\", \"DT_LATITUDE\",\"DT_DEPRECIATION\",\"FD_FEEDER_NAME\",";
                strQry += " \"FD_FEEDER_CODE\" ||'-'|| \"FD_FEEDER_NAME\" AS \"FEEDERCODE\",\"DT_TIMS_CODE\",\"DT_GOS\",\"DT_RAPDRP\",\"DT_WO_ID\",\"DT_MP_CONST\",\"DT_MLA_CONST\",\"DT_CIRCUIT1\",\"DT_CIRCUIT2\",\"DT_CIRCUIT3\",\"DT_CIRCUIT4\",to_char(\"DT_ELE_DATE\",'dd/MM/yyyy') AS \"DT_ELE_DATE\",\"DT_ELE_RATENO\" FROM ";
                strQry += " \"TBLDTCMAST\",\"TBLFEEDERMAST\"  WHERE CAST(\"DT_ID\" AS TEXT)=:DtcId AND  \"DT_FDRSLNO\"=\"FD_FEEDER_CODE\"";

                NpgsqlCommand.Parameters.AddWithValue("DtcId",objDtcMaster.lDtcId);
                dtDcDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);

               
                if(dtDcDetails.Rows.Count > 0)
                {
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
                    objDtcMaster.sProjecttype = Convert.ToString(dtDcDetails.Rows[0]["DT_PROJECTTYPE"]);
                    objDtcMaster.sLoadtype = Convert.ToString(dtDcDetails.Rows[0]["DT_LOADTYPE"]);
                    objDtcMaster.sLongitude = Convert.ToString(dtDcDetails.Rows[0]["DT_LONGITUDE"]);
                    objDtcMaster.sLatitude = Convert.ToString(dtDcDetails.Rows[0]["DT_LATITUDE"]);
                    objDtcMaster.sDepreciation = Convert.ToString(dtDcDetails.Rows[0]["DT_DEPRECIATION"]);
                    objDtcMaster.sFeedercode = Convert.ToString(dtDcDetails.Rows[0]["FEEDERCODE"]);
                    objDtcMaster.sTims_Code = Convert.ToString(dtDcDetails.Rows[0]["DT_TIMS_CODE"]);
                    objDtcMaster.sGOS = Convert.ToString(dtDcDetails.Rows[0]["DT_GOS"]);
                    objDtcMaster.sRAPDRP = Convert.ToString(dtDcDetails.Rows[0]["DT_RAPDRP"]);
                    objDtcMaster.sWOslno = Convert.ToString(dtDcDetails.Rows[0]["DT_WO_ID"]);
                    objDtcMaster.sMPConst = Convert.ToString(dtDcDetails.Rows[0]["DT_MP_CONST"]);
                    objDtcMaster.sMLAConst = Convert.ToString(dtDcDetails.Rows[0]["DT_MLA_CONST"]);
                    objDtcMaster.sCircuit1 = Convert.ToString(dtDcDetails.Rows[0]["DT_CIRCUIT1"]);
                    objDtcMaster.Circuit2 = Convert.ToString(dtDcDetails.Rows[0]["DT_CIRCUIT2"]);
                    objDtcMaster.Circuit3 = Convert.ToString(dtDcDetails.Rows[0]["DT_CIRCUIT3"]);
                    objDtcMaster.Circuit4 = Convert.ToString(dtDcDetails.Rows[0]["DT_CIRCUIT4"]);
                    objDtcMaster.sEleInsDate = Convert.ToString(dtDcDetails.Rows[0]["DT_ELE_DATE"]);
                    objDtcMaster.sEleInsRateNo = Convert.ToString(dtDcDetails.Rows[0]["DT_ELE_RATENO"]);
                    if (Convert.ToString(dtDcDetails.Rows[0]["DT_AVG_LOAD"]) != "")
                    {
                        objDtcMaster.sDtAvgLoad = Convert.ToString(dtDcDetails.Rows[0]["DT_AVG_LOAD"]);
                    }
                    if (Convert.ToString(dtDcDetails.Rows[0]["DT_PEAK_LOAD"]) != "")
                    {
                        objDtcMaster.sDtPeakLoad = Convert.ToString(dtDcDetails.Rows[0]["DT_PEAK_LOAD"]);
                    }
                    if (Convert.ToString(dtDcDetails.Rows[0]["DT_SURPLUS_CAP"]) != "")
                    {
                        objDtcMaster.sDtsurpluscap = Convert.ToString(dtDcDetails.Rows[0]["DT_SURPLUS_CAP"]);
                    }

                    if (Convert.ToString(dtDcDetails.Rows[0]["DT_NAME"]) != "")
                    {
                        string tcname = Convert.ToString(dtDcDetails.Rows[0]["DT_NAME"]);
                        objDtcMaster.sDtcName = Regex.Replace(tcname, @"[^0-9a-zA-Z_\-()/.]+", "");
                    }


                }
                strQry = "SELECT \"TC_SLNO\" ||  '~' ||  \"TM_NAME\" || '~' || \"TC_CAPACITY\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\"  WHERE \"TC_MAKE_ID\"= \"TM_ID\" and CAST(\"TC_CODE\" AS TEXT)=:TcCode";
                NpgsqlCommand.Parameters.AddWithValue("TcCode", objDtcMaster.sTcCode);
                string sResult = Objcon.get_value(strQry, NpgsqlCommand);

                if (sResult != "")
                {
                    objDtcMaster.sTcSlno = sResult.Split('~').GetValue(0).ToString();
                    objDtcMaster.sTCMakeName = sResult.Split('~').GetValue(1).ToString();
                    objDtcMaster.sTCCapacity = sResult.Split('~').GetValue(2).ToString();
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
        public clsDtcMaster GetTCDetails(clsDtcMaster objDTCMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("TcSlno", objDTCMaster.sTcSlno);
                sQry = "SELECT \"TC_SLNO\",\"TC_CODE\",\"TM_NAME\",CAST(TC_CAPACITY AS TEXT) \"TC_CAPACITY\",to_char(\"TC_MANF_DATE\",'dd/MM/yyyy')\"TC_MANF_DATE\" FROM \"TBLTCMASTER\",\"TBLTRANSMAKES\" ";
                sQry += " WHERE \"TC_MAKE_ID\"= \"TM_ID\" and \"TC_SLNO\"=:TcSlno";
                dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objDTCMaster.sTcSlno = dt.Rows[0]["TC_SLNO"].ToString();
                    objDTCMaster.sTCMakeName = dt.Rows[0]["TM_NAME"].ToString();
                    objDTCMaster.sTCCapacity = dt.Rows[0]["TC_CAPACITY"].ToString();
                    objDTCMaster.sTcCode = dt.Rows[0]["TC_CODE"].ToString();
                    objDTCMaster.sManufactureDate = dt.Rows[0]["TC_MANF_DATE"].ToString();

                }
                return objDTCMaster;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objDTCMaster;
            }
        }


        public clsDTCCommision GetImagePath(clsDTCCommision objDTCComm)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                string sDtr_Path = string.Empty;
                DataTable dt = new DataTable();

                NpgsqlCommand.Parameters.AddWithValue("TcCode", Convert.ToInt64(objDTCComm.sTcCode) );
                if (objDTCComm.sTcCode == "0")
                {
                    NpgsqlCommand.Parameters.AddWithValue("DtcCode1", objDTCComm.sDtcCode);
                    strQry = "SELECT \"EP_DTC_PATH\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\",\"TBLENUMERATIONDETAILS\"  ";
                    strQry += " WHERE \"DTE_DTCCODE\"=:DtcCode1 AND \"DTE_ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\"='1' order by\"EP_ID\" DESC";
                    sDtr_Path = Objcon.get_value(strQry, NpgsqlCommand);

                    if (sDtr_Path != "" || sDtr_Path != null)
                    {
                        objDTCComm.sDTrImagePath = sDtr_Path;
                    }
                }
                else
                {
                    strQry = "SELECT \"EP_SSPLATE_PATH\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\",\"TBLENUMERATIONDETAILS\"  ";
                    strQry += " WHERE \"DTE_TC_CODE\"=:TcCode AND \"DTE_ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\"='1' order by\"EP_ID\" DESC";
                    sDtr_Path = Objcon.get_value(strQry, NpgsqlCommand);

                    if (sDtr_Path != "" || sDtr_Path != null)
                    {
                        objDTCComm.sDTrImagePath = sDtr_Path;
                    }
                }
                //first ep_dtc_path photo path showed later they need dtc paint photo so changed EP_DTLMSDTC_PATH Path on 20-11-2018

                //strQry = "SELECT \"EP_DTC_PATH\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\",\"TBLENUMERATIONDETAILS\"  ";
                NpgsqlCommand.Parameters.AddWithValue("DtcCode", objDTCComm.sDtcCode);
                strQry = "SELECT \"EP_DTLMSDTC_PATH\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\",\"TBLENUMERATIONDETAILS\"  ";
                strQry += " WHERE \"DTE_DTCCODE\"=:DtcCode AND \"DTE_ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\"='1' order by\"EP_ID\" DESC ";
                string sDtc_Path = Objcon.get_value(strQry, NpgsqlCommand);
                if (sDtc_Path != "" || sDtc_Path != null)
                {
                    objDTCComm.sDTCImagePath = sDtc_Path;                    
                }
                return objDTCComm;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objDTCComm;
            }
        }


        public bool CheckSelfExecutionSchemeType(clsDTCCommision objDTcComm)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                bool bResult = false;
                //if (objDTcComm.sProjecttype == "9" || objDTcComm.sProjecttype == "10")
                if (objDTcComm.sProjecttype == "11")
                {
                    //strQry = "SELECT \"DT_TRANS_COMMISION_DATE\" FROM \"TBLDTCMAST\" WHERE now()-\"DT_TRANS_COMMISION_DATE\">365 AND \"DT_CODE\"='" + objDTcComm.sDtcCode + "'";
                    strQry = "SELECT TO_CHAR(\"DT_TRANS_COMMISION_DATE\",'YYYY-MM-DD') FROM \"TBLDTCMAST\" WHERE DATE_PART('DAY',CAST(TO_CHAR(NOW(),'YYYY-MM-DD') AS TEXT)::TIMESTAMP - CAST(TO_CHAR(CAST(\"DT_TRANS_COMMISION_DATE\" AS DATE),'YYYY-MM-DD') AS TEXT)::TIMESTAMP) > 365  AND \"DT_PROJECTTYPE\" = '11' AND \"DT_CODE\" = :DtcCode";
                    NpgsqlCommand.Parameters.AddWithValue("DtcCode", objDTcComm.sDtcCode);
                    string sResult = Objcon.get_value(strQry, NpgsqlCommand);
                    if (sResult == "")
                    {
                        bResult = true;
                    }

                }
                return bResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool CheckDTRCapacity(string sDTRCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                bool bResult = false;
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("DTRCode",Convert.ToDouble(sDTRCode));
                strQry = "SELECT \"TC_CAPACITY\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\"=:DTRCode";
                int iDTRCap = Convert.ToInt16(Objcon.get_value(strQry, NpgsqlCommand));
                if(iDTRCap >= 250)
                {
                    return true;
                }
                return bResult;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public string SaveXmlData(clsDTCCommision objDTcComm)
        {
            string sTcXmlData = string.Empty;

            try
            {

                string strQry = string.Empty;
                string strTemp = string.Empty;

                string sPrimaryKey = "{0}";


                objDTcComm.sColumnNames = "DT_CODE,DT_NAME,DT_OM_SLNO,DT_TC_ID,DT_INTERNAL_CODE,";
                objDTcComm.sColumnNames += "DT_KWH_READING,DT_TOTAL_CON_HP,DT_TOTAL_CON_KW,";
                objDTcComm.sColumnNames += "DT_LAST_SERVICE_DATE,DT_TRANS_COMMISION_DATE,";
                objDTcComm.sColumnNames += "DT_FDRCHANGE_DATE,DT_FDRSLNO,DT_CON_DATE,DT_PROJECTTYPE";

                objDTcComm.sColumnValues = "" + objDTcComm.sDtcCode + "," + objDTcComm.sDtcName + ",";
                objDTcComm.sColumnValues += "" + objDTcComm.sOMSectionName + "," + objDTcComm.sTcCode + ",";
                objDTcComm.sColumnValues += "" + objDTcComm.sInternalCode + "," + objDTcComm.iKWHReading + "," + objDTcComm.iConnectedHP + ",," + objDTcComm.iConnectedKW + "";
                objDTcComm.sColumnValues += "," + objDTcComm.sServiceDate + "," + objDTcComm.sCommisionDate + "," + objDTcComm.sFeederChangeDate + "," + objDTcComm.sDtcCode.ToString().Substring(0, Constants.Feeder) + ",";
                objDTcComm.sColumnValues += "" + objDTcComm.sConnectionDate + "," + objDTcComm.sProjecttype + "";

                objDTcComm.sTableNames = "TBLDTCMAST";

                sTcXmlData = CreateXml(objDTcComm.sColumnNames, objDTcComm.sColumnValues, objDTcComm.sTableNames);
                return sTcXmlData;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sTcXmlData;
            }

        }

        //save data in tblefodata and workflowobjects
        public bool SaveWorkFlowData(clsDTCCommision objDTcComm)
        {
            try
            {
                Objcon.BeginTransaction();
                string strQry = string.Empty;
                string[] Arr = new string[3];
                //objDTcComm.sWFDataId = Convert.ToString(Objcon.Get_max_no("WFO_ID", "TBLWFODATA"));
                //strQry = "INSERT INTO TBLWFODATA (WFO_ID,WFO_QUERY_VALUES,WFO_PARAMETER,WFO_DATA,WFO_CR_BY) VALUES (";
                //strQry += " '" + objDTcComm.sWFDataId + "','" + objDTcComm.sQryValues + "','" + objDTcComm.sParameterValues + "','" + sXmlData + "',";
                //strQry += " '" + objDTcComm.sCrBy + "')";
                //Objcon.ExecuteQry(strQry);
                NpgsqlCommand cmd = new NpgsqlCommand("proc_save_wfo_data");

                cmd.Parameters.AddWithValue("wfo_data_id", "0");
                cmd.Parameters.AddWithValue("wfo_data_qry_value", "0");
                cmd.Parameters.AddWithValue("wfo_data_parameter", "0");
                cmd.Parameters.AddWithValue("wfo_data_xml_data", Convert.ToString(objDTcComm.sXmlData));
                cmd.Parameters.AddWithValue("wfo_data_crby", Convert.ToString(objDTcComm.sCrBy));
                //if (objDTcComm.sFormName != null && objDTcComm.sFormName != "")
                //{
                //    //To get Business Object Id
                //    objDTcComm.sBOId = Objcon.get_value("SELECT BO_ID FROM TBLBUSINESSOBJECT WHERE UPPER(BO_FORMNAME)='" + objDTcComm.sFormName.Trim().ToUpper() + "'");
                //}
                //string sWFlowId = Convert.ToString(Objcon.Get_max_no("WO_ID", "TBLWORKFLOWOBJECTS"));
                //strQry = "INSERT INTO TBLWORKFLOWOBJECTS (WO_ID,WO_BO_ID,WO_RECORD_ID,WO_PREV_APPROVE_ID,WO_NEXT_ROLE,WO_OFFICE_CODE,WO_CLIENT_IP,";
                //strQry += " WO_CR_BY,WO_APPROVED_BY,WO_APPROVE_STATUS,WO_RECORD_BY,WO_DESCRIPTION,WO_USER_COMMENT,WO_WFO_ID,WO_INITIAL_ID,WO_REF_OFFCODE)";
                //strQry += " VALUES ('" + sWFlowId + "','" + objDTcComm.sBOId + "','" + objDTcComm.lDtcId + "',0,";
                //strQry += " '0','" + objDTcComm.sOfficeCode + "','" + objDTcComm.sClientIP + "','" + objDTcComm.sCrBy + "',";
                //strQry += " '" + objDTcComm.sCrBy + "','1','WEB','" + objDTcComm.sDescription + "','" + objDTcComm.sDescription + "','" + objDTcComm.sWFDataId + "','" + sWFlowId + "','" + objDTcComm.sOfficeCode + "')";
                //Objcon.ExecuteQry(strQry);
                cmd.Parameters.AddWithValue("wo_obj_id", "0");
                cmd.Parameters.AddWithValue("work_obj_bo_id", "0");
                cmd.Parameters.AddWithValue("work_obj_record_id", Convert.ToString(objDTcComm.lDtcId));
                cmd.Parameters.AddWithValue("work_obj_pre_approv_id", "0");
                cmd.Parameters.AddWithValue("work_obj_next_role", "0");
                cmd.Parameters.AddWithValue("work_obj_office_code", Convert.ToString(objDTcComm.sOfficeCode));
                cmd.Parameters.AddWithValue("work_obj_pre_client_ip", Convert.ToString(objDTcComm.sClientIP));
                cmd.Parameters.AddWithValue("work_obj_crby", Convert.ToString(objDTcComm.sCrBy));
                cmd.Parameters.AddWithValue("work_obj_approve_status", "1");
                cmd.Parameters.AddWithValue("work_obj_record_by", Convert.ToString(objDTcComm.sCrBy));
                cmd.Parameters.AddWithValue("work_obj_approved_by", "0");
                cmd.Parameters.AddWithValue("work_obj_description", Convert.ToString(objDTcComm.sDescription));
                cmd.Parameters.AddWithValue("work_obj_comment", Convert.ToString(objDTcComm.sDescription));
                cmd.Parameters.AddWithValue("work_obj_wfo_id", "0");
                cmd.Parameters.AddWithValue("work_obj_intial_id", "0");
                cmd.Parameters.AddWithValue("work_obj_ref_offcode", Convert.ToString(objDTcComm.sOfficeCode));
                cmd.Parameters.AddWithValue("work_obj_form_name", Convert.ToString(objDTcComm.sFormName.Trim().ToUpper()));
                cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                Arr[2] = "pk_id";
                Arr[1] = "op_id";
                Arr[0] = "msg";
                Arr = Objcon.Execute(cmd, Arr, 3);
                //cmd.ExecuteNonQuery();

                WorkFlowObjects(objDTcComm);

                Objcon.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                Objcon.RollBackTrans();
                
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
                return false;
            }
        }

        //To Get CLient ip
        public void WorkFlowObjects(clsDTCCommision objDTcComm)
        {
            try
            {
                string sClientIP = string.Empty;

                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    int le = ipRange.Length - 1;
                    sClientIP = ipRange[0];
                }
                else
                {
                    sClientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }



                objDTcComm.sClientIP = sClientIP;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //creating xml data for Wfo_data insert
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
                //DataRow dRow = dt.NewRow();
                for (int i = 0; i < strArrColumns.Length; i++)
                {
                    DataTable dt = new DataTable();
                    DataRow dRow = dt.NewRow();
                    string[] strdtColumns = strArrColumns[i].Split(',');
                    string[] strdtParametres = strArrParameters[i].Split(',');
                    dt.TableName = strTableNames[i];
                    //DataRow dRow1 = dtnew.NewRow();
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
                            //i--;
                        }
                        k++;

                    }

                    k = 0;

                    ds.Merge(dt);
                    dt.Clear();

                }
                return ds.GetXml();
                //dt.TableName = "Failure and Invoice";
                //////////////////////////////////////////////
                //dt.TableName = "TBLDTCFAILURE";

            }

            catch (Exception ex)
            {
                string strfailure = string.Empty;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return strfailure;
                //return ds;
            }
        }

        public string GetDtcCode(string sFeederCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
           
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("FeederCode", sFeederCode);
                string strQry = "SELECT COALESCE(MAX(cast(\"DT_CODE\" as int)),0)+1 FROM \"TBLDTCMAST\" WHERE \"DT_FDRSLNO\"=:FeederCode";
                return Objcon.get_value(strQry, NpgsqlCommand);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public bool IsExistINmaintainance(clsDTCCommision objDtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
           
            string strQry = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("DtcCode", objDtcMaster.sDtcCode);
                strQry = "SELECT \"TM_DT_CODE\" FROM \"TBLDTCMAINTENANCE\" WHERE \"TM_DT_CODE\"=:DtcCode";
                string sDtCode = Objcon.get_value(strQry, NpgsqlCommand);
                if (sDtCode == "")
                {

                    return false;
                }
                else
                {

                    return true;
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}
