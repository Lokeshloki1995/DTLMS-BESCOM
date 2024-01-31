using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace IIITS.DTLMS.BL
{
    public class clsFailureEntry
    {
        string strFormCode = "clsFailureEntry";
        // CustOledbConnection objcon = new CustOledbConnection(Constants.Password);
        public string spgrsstatus { get; set; }
        public string sDtcId { get; set; }
        public string sDtcCode { get; set; }
        public string sDtcName { get; set; }
        public string sDtcServicedate { get; set; }
        public string sDtcLoadKw { get; set; }
        public string sDtcLoadHp { get; set; }
        public string sCommissionDate { get; set; }
        public string sDtcCapacity { get; set; }
        public string sDtcLocation { get; set; }
        public string sDtcTcSlno { get; set; }
        public string sDtcTcMake { get; set; }
        public string sFailureDate { get; set; }
        public string sFailureReasure { get; set; }
        public string sDtcReadings { get; set; }
        public string sDtcTcCode { get; set; }
        public string sFailureId { get; set; }
        public string sAlternateReplaceType { get; set; }
        public string sCrby { get; set; }
        public string sOfficeCode { get; set; }
        public string sDtrSaveCommissionDate { get; set; }
        public string sDTrCommissionDate { get; set; }
        public string sDTrEnumerationDate { get; set; }
        public string sManfDate { get; set; }
        public string sTCId { get; set; }

        public string sLastRepairedBy { get; set; }
        public string sLastRepairedDate { get; set; }
        public string sGuarantyType { get; set; }
        public string sGuarantySource { get; set; }

        public string sEnhancedCapacity { get; set; }
        public string sFailtype { get; set; }

        public string sCoreType { get; set; }
        public string sInsulationType { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sRating { get; set; }
        public string sClientIP { get; set; }
        public string sWFDataId { get; set; }
        public string sActionType { get; set; } // Approve;Reject;Modify and Approve

        //New Fields

        public string sFailureType { get; set; }
        public string sHTBusing { get; set; }
        public string sLTBusing { get; set; }
        public string sHTBusingRod { get; set; }
        public string sLTBusingRod { get; set; }
        public string sOilLevel { get; set; }
        public string sOilQuantity { get; set; }
        public string sTankCondition { get; set; }
        public string sWheel { get; set; }
        public string sExplosionValve { get; set; }
        public string sBreather { get; set; }
        public string sDrainValve { get; set; }
        public string sOilCapacity { get; set; }
        public string sRepairer { get; set; }
        public string sdocketno { get; set; }
        public string sdocketDate { get; set; }
        public string sCustName { get; set; }
        public string sCustNo { get; set; }
        public string sMeggerValue { get; set; }
        public string sStatus_flag { get; set; }
        public string sPurpose { get; set; }
        public string sConditionoftc { get; set; }
        public string sSilicaCondition { get; set; }
        public string sOilType { get; set; }
        public string sDTCType { get; set; }
        public string sModem { get; set; }
        public string sFirstGuarantyType = string.Empty;
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
     
        /// <summary>
        /// Check's Enterd PGRSNumber Exist in TBLDTCFAILURE.
        /// </summary>
        /// <param name="PGRS_Number"></param>
        /// <returns></returns>
        public string CheckPGRSNumber_Exist(string PGRS_Number)
        {
            string CheckResult = string.Empty;
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);
            try
            {
                CheckResult = objDatabse.get_value(" SELECT EXISTS(SELECT \"DF_PGRS_DOCKET\" from \"TBLDTCFAILURE\"  where \"DF_PGRS_DOCKET\" = '" + PGRS_Number + "') ");                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
            }
            return CheckResult;
        }
        public string[] SaveFailureDetails(clsFailureEntry objFailureDetails)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);
            string[] Arr = new string[2];
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                //Check DTC Code exists or not
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDtcCode", objFailureDetails.sDtcCode);
                string sResult = objDatabse.get_value("SELECT \"DT_CODE\" FROM \"TBLDTCMAST\" WHERE \"DT_CODE\" ='" + objFailureDetails.sDtcCode + "'");
                if (sResult.Length == 0)
                {
                    Arr[0] = "Enter Valid DTC Code";
                    Arr[1] = "2";
                    return Arr;
                }
                //Check Already Failure Entry Done but Decomm Pending
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDtcCode1", objFailureDetails.sDtcCode);
                sResult = objDatabse.get_value("SELECT \"DF_DTC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" ='" + objFailureDetails.sDtcCode + "' AND \"DF_REPLACE_FLAG\" =0");
                if (sResult.Length > 0)
                {
                    Arr[0] = "Already Declared Failure or Enhancement for Selected DTC Code " + objFailureDetails.sDtcCode;
                    Arr[1] = "2";
                    return Arr;
                }
                if (objFailureDetails.sFailureId == "0" || objFailureDetails.sFailureId == "")
                {
                    //Workflow / Approval
                    #region Workflow
                   
                    if (objFailureDetails.sFailtype == "1")
                    {
                        strQry = "INSERT INTO \"TBLDTCFAILURE\"(\"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_CRBY\",\"DF_CRON\",\"DF_DATE\",\"DF_REASON\",\"DF_KWH_READING\",\"DF_STATUS_FLAG\",\"DF_LOC_CODE\",";
                        strQry += " \"DF_FAILURE_TYPE\",\"DF_HT_BUSING\",\"DF_LT_BUSING\",\"DF_HT_BUSING_ROD\",\"DF_LT_BUSING_ROD\",\"DF_BREATHER\",\"DF_OIL_LEVEL\",\"DF_DRAIN_VALVE\",";
                        strQry += " \"DF_OIL_QNTY\",\"DF_TANK_CONDITION\",\"DF_EXPLOSION\",\"DF_GUARANTY_TYPE\",\"DF_GUARANTY_TYPE_SOURCE\",\"DF_DTR_COMMISSION_DATE\",\"DF_REPAIRER_ID\", ";
                        strQry += " \"DF_PGRS_DOCKET\",\"DF_CUSTOMER_NAME\",\"DF_CUSTOMER_NUMBER\",\"DF_MEGGER_VALUE\",\"DF_PURPOSE\",\"DF_ALTERNATE_RPMT\",\"DF_SILC_CONDTN\",\"DF_OIL_TYPE\",\"DF_DTC_TYPE\",\"DF_MODEM\",\"DF_PGRS_DOCKET_DATE\")";
                        strQry += "VALUES('{0}','" + objFailureDetails.sDtcCode + "','" + objFailureDetails.sDtcTcCode + "','" + objFailureDetails.sCrby + "',";
                        strQry += " NOW(),TO_DATE('" + objFailureDetails.sFailureDate + "','dd/MM/yyyy'),'" + objFailureDetails.sFailureReasure + "',";
                        strQry += " '" + objFailureDetails.sDtcReadings + "','" + objFailureDetails.sFailtype + "','" + sOfficeCode + "','" + objFailureDetails.sFailureType + "','" + objFailureDetails.sHTBusing + "',";
                        strQry += " '" + objFailureDetails.sLTBusing + "','" + objFailureDetails.sHTBusingRod + "','" + objFailureDetails.sLTBusingRod + "','" + objFailureDetails.sBreather + "',";
                        strQry += " '" + objFailureDetails.sOilLevel + "','" + objFailureDetails.sDrainValve + "','" + objFailureDetails.sOilQuantity + "','" + objFailureDetails.sTankCondition + "',";
                        strQry += " '" + objFailureDetails.sExplosionValve + "','" + objFailureDetails.sGuarantyType + "','" + objFailureDetails.sGuarantySource + "',TO_DATE('" + objFailureDetails.sDTrCommissionDate + "','dd/MM/yyyy'),";
                        strQry += " '" + objFailureDetails.sRepairer + "','" + objFailureDetails.sdocketno + "','" + objFailureDetails.sCustName + "','" + objFailureDetails.sCustNo + "', ";
                        strQry += " '" + objFailureDetails.sMeggerValue + "','" + objFailureDetails.sPurpose + "','" + objFailureDetails.sAlternateReplaceType + "','" + objFailureDetails.sSilicaCondition + "',";
                        strQry += " '" + objFailureDetails.sOilType + "','" + objFailureDetails.sDTCType + "','" + objFailureDetails.sModem + "',TO_DATE('" + objFailureDetails.sdocketDate + "','dd/MM/yyyy'))";
                    }
                    else
                    {
                        strQry = "INSERT INTO \"TBLDTCFAILURE\"(\"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_CRBY\",\"DF_CRON\",\"DF_DATE\",\"DF_REASON\",\"DF_KWH_READING\",\"DF_STATUS_FLAG\",\"DF_LOC_CODE\",";
                        strQry += " \"DF_FAILURE_TYPE\",\"DF_HT_BUSING\",\"DF_LT_BUSING\",\"DF_HT_BUSING_ROD\",\"DF_LT_BUSING_ROD\",\"DF_BREATHER\",\"DF_OIL_LEVEL\",\"DF_DRAIN_VALVE\",";
                        strQry += " \"DF_OIL_QNTY\",\"DF_TANK_CONDITION\",\"DF_EXPLOSION\",\"DF_ENHANCE_CAPACITY\",\"DF_GUARANTY_TYPE\",\"DF_GUARANTY_TYPE_SOURCE\",\"DF_DTR_COMMISSION_DATE\", ";
                        strQry += " \"DF_REPAIRER_ID\",\"DF_PGRS_DOCKET\",\"DF_CUSTOMER_NAME\",\"DF_CUSTOMER_NUMBER\",\"DF_MEGGER_VALUE\",\"DF_PURPOSE\",\"DF_ALTERNATE_RPMT\",\"DF_SILC_CONDTN\",\"DF_OIL_TYPE\",\"DF_DTC_TYPE\",\"DF_MODEM\",\"DF_PGRS_DOCKET_DATE\")";
                        strQry += "VALUES('{0}','" + objFailureDetails.sDtcCode + "','" + objFailureDetails.sDtcTcCode + "','" + objFailureDetails.sCrby + "',";
                        strQry += " NOW(),TO_DATE('" + objFailureDetails.sFailureDate + "','dd/MM/yyyy'),'" + objFailureDetails.sFailureReasure + "',";
                        strQry += " '" + objFailureDetails.sDtcReadings + "','" + objFailureDetails.sFailtype + "','" + sOfficeCode + "','" + objFailureDetails.sFailureType + "','" + objFailureDetails.sHTBusing + "',";
                        strQry += " '" + objFailureDetails.sLTBusing + "','" + objFailureDetails.sHTBusingRod + "','" + objFailureDetails.sLTBusingRod + "','" + objFailureDetails.sBreather + "',";
                        strQry += " '" + objFailureDetails.sOilLevel + "','" + objFailureDetails.sDrainValve + "','" + objFailureDetails.sOilQuantity + "','" + objFailureDetails.sTankCondition + "',";
                        strQry += " '" + objFailureDetails.sExplosionValve + "','" + objFailureDetails.sEnhancedCapacity + "','" + objFailureDetails.sGuarantyType + "', ";
                        strQry += " '" + objFailureDetails.sGuarantySource + "',TO_DATE('" + objFailureDetails.sDTrCommissionDate + "','dd/MM/yyyy'),";
                        strQry += " '" + objFailureDetails.sRepairer + "','" + objFailureDetails.sdocketno + "','" + objFailureDetails.sCustName + "', ";
                        strQry += " '" + objFailureDetails.sCustNo + "','" + objFailureDetails.sMeggerValue + "','" + objFailureDetails.sPurpose + "','" + objFailureDetails.sAlternateReplaceType + "','" + objFailureDetails.sSilicaCondition + "',";
                        strQry += " '" + objFailureDetails.sOilType + "','" + objFailureDetails.sDTCType + "','" + objFailureDetails.sModem + "',TO_DATE('" + objFailureDetails.sdocketDate + "','dd/MM/yyyy'))";
                    }

                    strQry = strQry.Replace("'", "''");

                    string strQry1 = " UPDATE \"TBLTCMASTER\" SET \"TC_STATUS\"=3 ,\"TC_UPDATED_EVENT\" ='FAILURE ENTRY',";
                    strQry1 += " \"TC_UPDATED_EVENT_ID\"='" + objFailureDetails.sDtcId + "', \"TC_LOCATION_ID\" ='" + sOfficeCode + "',\"TC_PREV_OFFCODE\"= '" + sOfficeCode + "' ";
                    strQry1 += "WHERE \"TC_CODE\" ='" + objFailureDetails.sDtcTcCode + "' ";

                    strQry1 = strQry1.Replace("'", "''");

                    string sParam = "SELECT COALESCE(MAX(\"DF_ID\"),0)+1 FROM \"TBLDTCFAILURE\" ";


                    clsApproval objApproval = new clsApproval();

                    if (objFailureDetails.sActionType == null)
                    {

                        bool bResult = objApproval.CheckAlreadyExistEntry(objFailureDetails.sDtcCode, "9");
                        if (bResult == true)
                        {
                            Arr[0] = "Failure Declare Already done for DTC Code " + objFailureDetails.sDtcCode + ", Waiting for Approval";
                            Arr[1] = "2";
                            return Arr;
                        }

                        bResult = objApproval.CheckAlreadyExistEntry(objFailureDetails.sDtcCode, "10");
                        if (bResult == true)
                        {
                            Arr[0] = "Capacity Enhancement Already done for DTC Code " + objFailureDetails.sDtcCode + ", Waiting for Approval";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }

                    objApproval.sFormName = objFailureDetails.sFormName;
                    // objApproval.sRecordId = objFailureDetails.sFailureId;
                    objApproval.sOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sClientIp = objFailureDetails.sClientIP;
                    objApproval.sCrby = objFailureDetails.sCrby;
                    objApproval.sQryValues = strQry + ";" + strQry1;
                    objApproval.sParameterValues = sParam;
                    objApproval.sMainTable = "TBLDTCFAILURE";
                    objApproval.sDataReferenceId = objFailureDetails.sDtcCode;
                    objApproval.sbfm_id = "1";

                    if (objFailureDetails.sFailtype == "1")
                    {
                        objApproval.sDescription = "Failure Entry For DTC Code " + objFailureDetails.sDtcCode;
                    }
                    else
                    {
                        objApproval.sDescription = "Failure Entry with Enhancement For DTC Code " + objFailureDetails.sDtcCode;
                    }
                    objApproval.sRefOfficeCode = objFailureDetails.sOfficeCode;

                    string sPrimaryKey = "{0}";

                    if (objFailureDetails.sFailtype == "1")
                    {
                        objApproval.sColumnNames = "DF_ID,DF_DTC_CODE,DF_EQUIPMENT_ID,DF_CRBY,DF_CRON,DF_DATE,DF_REASON,DF_KWH_READING,DF_STATUS_FLAG,DF_LOC_CODE,";
                        objApproval.sColumnNames += "DF_FAILURE_TYPE,DF_HT_BUSING,DF_LT_BUSING,DF_HT_BUSING_ROD,DF_LT_BUSING_ROD,DF_BREATHER,DF_OIL_LEVEL,DF_DRAIN_VALVE,";
                        objApproval.sColumnNames += "DF_OIL_QNTY,DF_TANK_CONDITION,DF_EXPLOSION,GUARENTEE,DF_GUARANTY_TYPE_SOURCE,DTR_COMISSION_DATE,DF_REPAIRER_ID,";
                        objApproval.sColumnNames += "DF_PGRS_DOCKET,DF_CUSTOMER_NAME,DF_CUSTOMER_NUMBER,DF_MEGGER_VALUE,DF_PURPOSE,DF_ALTERNATE_RPMT,DF_SILC_CONDTN,DF_OIL_TYPE,DF_DTC_TYPE,DF_MODEM,DF_PGRS_DOCKET_DATE";
                        objApproval.sColumnValues = "" + sPrimaryKey + "," + objFailureDetails.sDtcCode + "," + objFailureDetails.sDtcTcCode + "," + objFailureDetails.sCrby + ",NOW(),";
                        objApproval.sColumnValues += "" + objFailureDetails.sFailureDate + "," + objFailureDetails.sFailureReasure.Replace(",", "") + "," + objFailureDetails.sDtcReadings + "," + objFailureDetails.sFailtype + "," + sOfficeCode + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sFailureType + "," + objFailureDetails.sHTBusing + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sLTBusing + "," + objFailureDetails.sHTBusingRod + "," + objFailureDetails.sLTBusingRod + "," + objFailureDetails.sBreather + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sOilLevel + "," + objFailureDetails.sDrainValve + "," + objFailureDetails.sOilQuantity + "," + objFailureDetails.sTankCondition + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sExplosionValve.Trim() + "," + objFailureDetails.sGuarantyType + "," + objFailureDetails.sGuarantySource + "," + objFailureDetails.sDTrCommissionDate + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sRepairer + "," + objFailureDetails.sdocketno + "," + objFailureDetails.sCustName + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sCustNo + "," + objFailureDetails.sMeggerValue + "," + objFailureDetails.sPurpose + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sAlternateReplaceType + "," + objFailureDetails.sSilicaCondition + "," + objFailureDetails.sOilType + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sDTCType + "," + objFailureDetails.sModem + "," + objFailureDetails.sdocketDate + " ";
                        objApproval.sTableNames = "TBLDTCFAILURE";
                    }
                    else
                    {

                        objApproval.sColumnNames = "DF_ID,DF_DTC_CODE,DF_EQUIPMENT_ID,DF_CRBY,DF_CRON,DF_DATE,DF_REASON,DF_KWH_READING,DF_STATUS_FLAG,DF_LOC_CODE,";
                        objApproval.sColumnNames += "DF_FAILURE_TYPE,DF_HT_BUSING,DF_LT_BUSING,DF_HT_BUSING_ROD,DF_LT_BUSING_ROD,DF_BREATHER,DF_OIL_LEVEL,DF_DRAIN_VALVE,";
                        objApproval.sColumnNames += "DF_OIL_QNTY,DF_TANK_CONDITION,DF_EXPLOSION,DF_ENHANCE_CAPACITY,GUARENTEE,DF_GUARANTY_TYPE_SOURCE,DTR_COMISSION_DATE,";
                        objApproval.sColumnNames += "DF_REPAIRER_ID,DF_PGRS_DOCKET,DF_CUSTOMER_NAME,DF_CUSTOMER_NUMBER,DF_MEGGER_VALUE,DF_PURPOSE,DF_ALTERNATE_RPMT,DF_SILC_CONDTN,DF_OIL_TYPE,DF_DTC_TYPE,DF_MODEM,DF_PGRS_DOCKET_DATE";
                        objApproval.sColumnValues = "" + sPrimaryKey + "," + objFailureDetails.sDtcCode + "," + objFailureDetails.sDtcTcCode + "," + objFailureDetails.sCrby + ",NOW(),";
                        objApproval.sColumnValues += "" + objFailureDetails.sFailureDate + "," + objFailureDetails.sFailureReasure.Replace(",", "ç") + "," + objFailureDetails.sDtcReadings + "," + objFailureDetails.sFailtype + "," + sOfficeCode + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sFailureType + "," + objFailureDetails.sHTBusing + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sLTBusing + "," + objFailureDetails.sHTBusingRod + "," + objFailureDetails.sLTBusingRod + "," + objFailureDetails.sBreather + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sOilLevel + "," + objFailureDetails.sDrainValve + "," + objFailureDetails.sOilQuantity + "," + objFailureDetails.sTankCondition + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sExplosionValve.Trim() + "," + objFailureDetails.sEnhancedCapacity + "," + objFailureDetails.sGuarantyType + "," + objFailureDetails.sGuarantySource + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sDTrCommissionDate + "," + objFailureDetails.sRepairer + "," + objFailureDetails.sdocketno + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sCustName + "," + objFailureDetails.sCustNo + "," + objFailureDetails.sMeggerValue + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sPurpose + "," + objFailureDetails.sAlternateReplaceType + "," + objFailureDetails.sSilicaCondition + "," + objFailureDetails.sOilType + ",";
                        objApproval.sColumnValues += "" + objFailureDetails.sDTCType + "," + objFailureDetails.sModem + "," + objFailureDetails.sdocketDate + "";
                        objApproval.sTableNames = "TBLDTCFAILURE";
                    }


                    //Check for Duplicate Approval
                    bool bApproveResult = objApproval.CheckDuplicateApprove(objApproval);
                    if (bApproveResult == false)
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }
                    objDatabse.BeginTransaction();
                    if (objFailureDetails.sActionType == "M")
                    {
                        objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                        objFailureDetails.sWFDataId = objApproval.sWFDataId;
                    }
                    else
                    {
                        // File.AppendAllText(sPath, Environment.NewLine + "Start Time --" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " , After Begin Transaction " + Environment.NewLine);

                        objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                        objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow1(objDatabse);
                        objApproval.SaveWorkflowObjects1(objApproval, objDatabse);
                        objFailureDetails.sWFDataId = objApproval.sWFDataId;
                        objFailureDetails.sFailureId = objApproval.sNewRecordId;
                    }
                    //  File.AppendAllText(sPath, Environment.NewLine + "Time --" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " , Before Commit Transaction " + Environment.NewLine);

                    objDatabse.CommitTransaction();


                    #endregion
                    Arr[0] = "DTC Failure Declared Successfully";
                    Arr[1] = "0";
                    return Arr;
                }
                else
                {
                    objcon.BeginTransaction();
                    string[] strResult = new string[2];
                    string[] strArray = new string[2];
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_updatefailuredetails");
                    cmd.Parameters.AddWithValue("sfailureid", Convert.ToString(objFailureDetails.sFailureId));
                    cmd.Parameters.AddWithValue("sfailuredate", Convert.ToString(objFailureDetails.sFailureDate));
                    cmd.Parameters.AddWithValue("sAlternateReplaceType", Convert.ToString(objFailureDetails.sAlternateReplaceType));
                    cmd.Parameters.AddWithValue("sfailurereasure", Convert.ToString(objFailureDetails.sFailureReasure));
                    cmd.Parameters.AddWithValue("sdtcreadings", Convert.ToString(objFailureDetails.sDtcReadings));
                    cmd.Parameters.AddWithValue("sfailuretype", Convert.ToString(objFailureDetails.sFailureType));
                    cmd.Parameters.AddWithValue("shtbusing", Convert.ToString(objFailureDetails.sHTBusing));
                    cmd.Parameters.AddWithValue("sltbusing", Convert.ToString(objFailureDetails.sLTBusing));
                    cmd.Parameters.AddWithValue("shtbusingrod", Convert.ToString(objFailureDetails.sHTBusingRod));
                    cmd.Parameters.AddWithValue("sltbusingrod", Convert.ToString(objFailureDetails.sLTBusingRod));
                    cmd.Parameters.AddWithValue("sbreather", Convert.ToString(objFailureDetails.sBreather));
                    cmd.Parameters.AddWithValue("soillevel", Convert.ToString(objFailureDetails.sOilLevel));
                    cmd.Parameters.AddWithValue("sdrainvalve", Convert.ToString(objFailureDetails.sDrainValve));
                    cmd.Parameters.AddWithValue("soilquantity", Convert.ToString(objFailureDetails.sOilQuantity));
                    cmd.Parameters.AddWithValue("stankcondition", Convert.ToString(objFailureDetails.sTankCondition));
                    cmd.Parameters.AddWithValue("swheel", Convert.ToString(objFailureDetails.sWheel));
                    cmd.Parameters.AddWithValue("sexplosionvalve", Convert.ToString(objFailureDetails.sExplosionValve));
                    cmd.Parameters.AddWithValue("soilcapacity", Convert.ToString(objFailureDetails.sOilCapacity));
                    cmd.Parameters.AddWithValue("sdtctccode", Convert.ToString(objFailureDetails.sDtcTcCode));
                    cmd.Parameters.AddWithValue("sdtcid", Convert.ToString(objFailureDetails.sDtcId));
                    cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailureDetails.sDtcCode));
                    cmd.Parameters.AddWithValue("scilicacondt", Convert.ToString(objFailureDetails.sSilicaCondition));
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = objcon.Execute(cmd, strArray, 2);
                    objcon.CommitTransaction();
                    return strResult;
                }
            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

                throw ex;

            }

        }
        public void UpdateDtrCommDate(clsFailureEntry objFailureDetails)
        {
            try
            {
                string[] strArray = new string[3];
                NpgsqlCommand cmd = new NpgsqlCommand("sp_updatedtrcommdate");
                cmd.Parameters.AddWithValue("sdtrsavecommissiondate", Convert.ToString(objFailureDetails.sDtrSaveCommissionDate));
                cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailureDetails.sDtcCode));
                cmd.Parameters.AddWithValue("sdtctccode", Convert.ToString(objFailureDetails.sDtcTcCode));
                objcon.Execute(cmd, strArray, 0);
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public clsFailureEntry SearchFailureDetails(clsFailureEntry objFailureDetails)
        {
            DataTable dtDetails = new DataTable();
            OleDbDataReader dr = null;

            try
            {
                objFailureDetails.sFailureId = "0";
                if (objFailureDetails.sFailureId != "0")
                {

                    string[] strArray = new string[3];
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_searchallfailuredetails");
                    cmd.Parameters.AddWithValue("sfailureid", Convert.ToString(objFailureDetails.sFailureId));
                    dtDetails = objcon.FetchDataTable(cmd);
                    
                    if (dtDetails.Rows.Count > 0)
                    {
                        objFailureDetails.sDtcId = dtDetails.Rows[0]["DT_ID"].ToString();
                        objFailureDetails.sDtcCode = dtDetails.Rows[0]["DF_DTC_CODE"].ToString();
                        objFailureDetails.sDtcName = dtDetails.Rows[0]["DT_NAME"].ToString();
                        objFailureDetails.sDtcServicedate = dtDetails.Rows[0]["DT_LAST_SERVICE_DATE"].ToString();
                        objFailureDetails.sDtcLoadKw = dtDetails.Rows[0]["DT_TOTAL_CON_KW"].ToString();
                        objFailureDetails.sDtcLoadHp = dtDetails.Rows[0]["DT_TOTAL_CON_HP"].ToString();
                        objFailureDetails.sCommissionDate = dtDetails.Rows[0]["DT_TRANS_COMMISION_DATE"].ToString();
                        objFailureDetails.sDtcCapacity = dtDetails.Rows[0]["TC_CAPACITY"].ToString();
                        objFailureDetails.sEnhancedCapacity = dtDetails.Rows[0]["DF_ENHANCE_CAPACITY"].ToString();
                        objFailureDetails.sDtcLocation = dtDetails.Rows[0]["TC_LOCATION_ID"].ToString();
                        objFailureDetails.sDtcTcSlno = dtDetails.Rows[0]["TC_SLNO"].ToString();
                        objFailureDetails.sDtcTcMake = dtDetails.Rows[0]["TC_MAKE_ID"].ToString();
                        objFailureDetails.sFailureDate = dtDetails.Rows[0]["DF_FAILED_DATE"].ToString();
                        objFailureDetails.sFailureReasure = dtDetails.Rows[0]["DF_REASON"].ToString();
                        objFailureDetails.sDtcReadings = dtDetails.Rows[0]["DF_KWH_READING"].ToString();
                        objFailureDetails.sDtcTcCode = dtDetails.Rows[0]["TC_CODE"].ToString();
                        objFailureDetails.sFailureId = dtDetails.Rows[0]["DF_ID"].ToString();
                        objFailureDetails.sCrby = dtDetails.Rows[0]["US_FULL_NAME"].ToString();
                        objFailureDetails.sOfficeCode = dtDetails.Rows[0]["DF_LOC_CODE"].ToString();
                        objFailureDetails.sManfDate = dtDetails.Rows[0]["TC_MANF_DATE"].ToString();
                        objFailureDetails.sTCId = dtDetails.Rows[0]["TC_ID"].ToString();
                        objFailureDetails.sGuarantyType = dtDetails.Rows[0]["DF_GUARANTY_TYPE"].ToString();
                        objFailureDetails.sAlternateReplaceType = dtDetails.Rows[0]["DF_ALTERNATE_RPMT"].ToString();
                        GetDTRCommissionDate(objFailureDetails);

                        objFailureDetails.sFailureType = dtDetails.Rows[0]["DF_FAILURE_TYPE"].ToString();
                        objFailureDetails.sHTBusing = dtDetails.Rows[0]["DF_HT_BUSING"].ToString();
                        objFailureDetails.sLTBusing = dtDetails.Rows[0]["DF_LT_BUSING"].ToString();
                        objFailureDetails.sHTBusingRod = dtDetails.Rows[0]["DF_HT_BUSING_ROD"].ToString();
                        objFailureDetails.sLTBusingRod = dtDetails.Rows[0]["DF_LT_BUSING_ROD"].ToString();
                        objFailureDetails.sBreather = dtDetails.Rows[0]["DF_BREATHER"].ToString();
                        objFailureDetails.sOilLevel = dtDetails.Rows[0]["DF_OIL_LEVEL"].ToString();
                        objFailureDetails.sDrainValve = dtDetails.Rows[0]["DF_DRAIN_VALVE"].ToString();
                        objFailureDetails.sOilQuantity = dtDetails.Rows[0]["DF_OIL_QNTY"].ToString();
                        objFailureDetails.sTankCondition = dtDetails.Rows[0]["DF_TANK_CONDITION"].ToString();
                        objFailureDetails.sWheel = dtDetails.Rows[0]["DF_WHEEL"].ToString();
                        objFailureDetails.sExplosionValve = dtDetails.Rows[0]["DF_EXPLOSION"].ToString();
                        objFailureDetails.sOilQuantity = dtDetails.Rows[0]["DF_OIL_CAPACITY"].ToString();
                        objFailureDetails.sEnhancedCapacity = Convert.ToString(dtDetails.Rows[0]["DF_ENHANCE_CAPACITY"]).Trim();
                        objFailureDetails.sRepairer = Convert.ToString(dtDetails.Rows[0]["DF_REPAIRER_ID"]).Trim();
                        objFailureDetails.sRating = Convert.ToString(dtDetails.Rows[0]["MD_NAME"]).Trim();
                        objFailureDetails.sSilicaCondition = Convert.ToString(dtDetails.Rows[0]["DF_SILC_CONDTN"]).Trim();
                        objFailureDetails.sConditionoftc = "default";
                        GetLastRepairedDetails(objFailureDetails);

                    }

                    return objFailureDetails;
                }

                else
                {

                    string[] strArray = new string[3];
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_searchfailuredtcdetails");
                    cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailureDetails.sDtcCode));
                    dtDetails = objcon.FetchDataTable(cmd);
                    
                    if (dtDetails.Rows.Count > 0)
                    {
                        objFailureDetails.sDtcId = dtDetails.Rows[0]["DT_ID"].ToString();
                        objFailureDetails.sDtcCode = dtDetails.Rows[0]["DT_CODE"].ToString();
                        objFailureDetails.sDtcName = dtDetails.Rows[0]["DT_NAME"].ToString();
                        objFailureDetails.sDtcServicedate = dtDetails.Rows[0]["DT_LAST_SERVICE_DATE"].ToString();
                        objFailureDetails.sDtcLoadKw = dtDetails.Rows[0]["DT_TOTAL_CON_KW"].ToString();
                        objFailureDetails.sDtcLoadHp = dtDetails.Rows[0]["DT_TOTAL_CON_HP"].ToString();
                        objFailureDetails.sCommissionDate = dtDetails.Rows[0]["DT_TRANS_COMMISION_DATE"].ToString();
                        objFailureDetails.sDtcCapacity = dtDetails.Rows[0]["TC_CAPACITY"].ToString();
                        objFailureDetails.sDtcLocation = dtDetails.Rows[0]["TC_LOCATION_ID"].ToString();
                        objFailureDetails.sDtcTcSlno = dtDetails.Rows[0]["TC_SLNO"].ToString();
                        objFailureDetails.sDtcTcMake = dtDetails.Rows[0]["TC_MAKE_ID"].ToString();
                        objFailureDetails.sDtcTcCode = dtDetails.Rows[0]["TC_CODE"].ToString();
                        objFailureDetails.sManfDate = dtDetails.Rows[0]["TC_MANF_DATE"].ToString();
                        objFailureDetails.sTCId = dtDetails.Rows[0]["TC_ID"].ToString();
                        if (objFailureDetails.sEnhancedCapacity == null || objFailureDetails.sEnhancedCapacity == "")
                        {
                            objFailureDetails.sEnhancedCapacity = objFailureDetails.sDtcCapacity;
                        }

                        GetLastRepairedDetails(objFailureDetails);
                        GetDTRCommissionDate(objFailureDetails);
                        GetEnumerationDate(objFailureDetails);

                    }

                    return objFailureDetails;
                }


            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailureDetails;
            }


        }
        public DataTable LoadsearchDTCFailure(clsFailureEntry objFailure)
        {
            DataTable dtDetails = new DataTable();
            try
            {
                string squery = string.Empty;



                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadsearchdtcfailure");
                cmd.Parameters.AddWithValue("sofficecode", Convert.ToString(objFailure.sOfficeCode));
                cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailure.sDtcCode));
                cmd.Parameters.AddWithValue("stimscode", Convert.ToString(objFailure.sDtcTcSlno));
                cmd.Parameters.AddWithValue("sdtname", Convert.ToString(objFailure.sDtcName));
                cmd.Parameters.AddWithValue("stccode", Convert.ToString(objFailure.sDtcTcCode));
                dtDetails = objcon.FetchDataTable(cmd);
                return dtDetails;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDetails;

            }

        }
        public DataTable LoadAllDTCFailure(clsFailureEntry objFailure)
        {
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadalldtcfailure");
                cmd.Parameters.AddWithValue("sofficecode", Convert.ToString(objFailure.sOfficeCode));
                dtDetails = objcon.FetchDataTable(cmd);
                return dtDetails;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDetails;

            }

        }
        public DataTable LoadAlreadyFailure(clsFailureEntry objFailure)
        {
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadalreadyfailure");
                cmd.Parameters.AddWithValue("sofficecode", Convert.ToString(objFailure.sOfficeCode));
                dtDetails = objcon.FetchDataTable(cmd);
                return dtDetails;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDetails;

            }

        }
        public object GetFailureDetails(clsFailureEntry objFailureDetails)
        {

            DataTable dtDetails = new DataTable();
            OleDbDataReader dr = null;

            try
            {
                if (objFailureDetails.sFailureId != "0")
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_searchallfailuredetails");
                    cmd.Parameters.AddWithValue("sfailureid", Convert.ToString(objFailureDetails.sFailureId));
                    dtDetails = objcon.FetchDataTable(cmd);

                    if (dtDetails.Rows.Count > 0)
                    {
                        objFailureDetails.sFailureId = dtDetails.Rows[0]["DF_ID"].ToString();
                        objFailureDetails.sDtcId = dtDetails.Rows[0]["DT_ID"].ToString();
                        objFailureDetails.sDtcCode = dtDetails.Rows[0]["DF_DTC_CODE"].ToString();
                        objFailureDetails.sDtcName = dtDetails.Rows[0]["DT_NAME"].ToString();
                        objFailureDetails.sDtcServicedate = dtDetails.Rows[0]["DT_LAST_SERVICE_DATE"].ToString();
                        objFailureDetails.sDtcLoadKw = dtDetails.Rows[0]["DT_TOTAL_CON_KW"].ToString();
                        objFailureDetails.sDtcLoadHp = dtDetails.Rows[0]["DT_TOTAL_CON_HP"].ToString();
                        objFailureDetails.sCommissionDate = dtDetails.Rows[0]["DT_TRANS_COMMISION_DATE"].ToString();
                        objFailureDetails.sDtcCapacity = Convert.ToDouble(dtDetails.Rows[0]["TC_CAPACITY"]).ToString();
                        objFailureDetails.sEnhancedCapacity = dtDetails.Rows[0]["DF_ENHANCE_CAPACITY"].ToString();
                        objFailureDetails.sDtcLocation = dtDetails.Rows[0]["TC_LOCATION_ID"].ToString();
                        objFailureDetails.sDtcTcSlno = dtDetails.Rows[0]["TC_SLNO"].ToString();
                        objFailureDetails.sDtcTcMake = dtDetails.Rows[0]["TC_MAKE_ID"].ToString();
                        objFailureDetails.sFailureDate = dtDetails.Rows[0]["DF_FAILED_DATE"].ToString();
                        objFailureDetails.sFailureReasure = dtDetails.Rows[0]["DF_REASON"].ToString();
                        objFailureDetails.sDtcReadings = dtDetails.Rows[0]["DF_KWH_READING"].ToString();
                        objFailureDetails.sDtcTcCode = dtDetails.Rows[0]["TC_CODE"].ToString();
                        objFailureDetails.sFailureId = dtDetails.Rows[0]["DF_ID"].ToString();
                        objFailureDetails.sCrby = dtDetails.Rows[0]["US_FULL_NAME"].ToString();
                        objFailureDetails.sOfficeCode = dtDetails.Rows[0]["DF_LOC_CODE"].ToString();
                        objFailureDetails.sManfDate = dtDetails.Rows[0]["TC_MANF_DATE"].ToString();
                        objFailureDetails.sTCId = dtDetails.Rows[0]["TC_ID"].ToString();
         
                        objFailureDetails.sGuarantyType = dtDetails.Rows[0]["DF_GUARANTY_TYPE"].ToString();
                        objFailureDetails.sDTrCommissionDate = dtDetails.Rows[0]["DF_DTR_COMMISSION_DATE"].ToString();

                        objFailureDetails.sStatus_flag = dtDetails.Rows[0]["DF_STATUS_FLAG"].ToString();
                        objFailureDetails.sFailureType = dtDetails.Rows[0]["DF_FAILURE_TYPE"].ToString();
                        objFailureDetails.sHTBusing = dtDetails.Rows[0]["DF_HT_BUSING"].ToString();
                        objFailureDetails.sLTBusing = dtDetails.Rows[0]["DF_LT_BUSING"].ToString();
                        objFailureDetails.sHTBusingRod = dtDetails.Rows[0]["DF_HT_BUSING_ROD"].ToString();
                        objFailureDetails.sLTBusingRod = dtDetails.Rows[0]["DF_LT_BUSING_ROD"].ToString();
                        objFailureDetails.sBreather = dtDetails.Rows[0]["DF_BREATHER"].ToString();
                        objFailureDetails.sOilLevel = dtDetails.Rows[0]["DF_OIL_LEVEL"].ToString();
                        objFailureDetails.sDrainValve = dtDetails.Rows[0]["DF_DRAIN_VALVE"].ToString();
                        objFailureDetails.sOilQuantity = dtDetails.Rows[0]["DF_OIL_QNTY"].ToString();
                        objFailureDetails.sTankCondition = dtDetails.Rows[0]["DF_TANK_CONDITION"].ToString();
                        objFailureDetails.sWheel = dtDetails.Rows[0]["DF_WHEEL"].ToString();
                        objFailureDetails.sExplosionValve = dtDetails.Rows[0]["DF_EXPLOSION"].ToString();
                        objFailureDetails.sOilCapacity = dtDetails.Rows[0]["DF_OIL_CAPACITY"].ToString();
                        objFailureDetails.sEnhancedCapacity = Convert.ToString(dtDetails.Rows[0]["DF_ENHANCE_CAPACITY"]).Trim();
                        objFailureDetails.sRepairer = Convert.ToString(dtDetails.Rows[0]["DF_REPAIRER_ID"]).Trim();
                        objFailureDetails.sdocketno = Convert.ToString(dtDetails.Rows[0]["DF_PGRS_DOCKET"]);
                        objFailureDetails.sCustName = Convert.ToString(dtDetails.Rows[0]["DF_CUSTOMER_NAME"]);
                        objFailureDetails.sCustNo = Convert.ToString(dtDetails.Rows[0]["DF_CUSTOMER_NUMBER"]);
                        objFailureDetails.sMeggerValue = Convert.ToString(dtDetails.Rows[0]["DF_MEGGER_VALUE"]).Trim().Replace("ç", ",");
                        objFailureDetails.sPurpose = Convert.ToString(dtDetails.Rows[0]["DF_PURPOSE"]).Trim().Replace("ç", ",");
                        objFailureDetails.sAlternateReplaceType = dtDetails.Rows[0]["DF_ALTERNATE_RPMT"].ToString();
                        objFailureDetails.sSilicaCondition = dtDetails.Rows[0]["DF_SILC_CONDTN"].ToString();
                        objFailureDetails.sOilType = dtDetails.Rows[0]["DF_OIL_TYPE"].ToString();
                        objFailureDetails.sDTCType = dtDetails.Rows[0]["DF_DTC_TYPE"].ToString();
                        objFailureDetails.sModem = dtDetails.Rows[0]["DF_MODEM"].ToString();
                        objFailureDetails.sdocketDate = dtDetails.Rows[0]["DF_PGRS_DOCKET_DATE"].ToString();
                        objFailureDetails.sRating = dtDetails.Rows[0]["MD_NAME"].ToString();

                        objFailureDetails.sConditionoftc = dtDetails.Rows[0]["TC_CONDITION"].ToString(); 
                        string qry = string.Empty;
                        if (objFailureDetails.sConditionoftc != "" && objFailureDetails.sConditionoftc != "0")
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sConditionoftc", Convert.ToDouble(objFailureDetails.sConditionoftc));
                            qry = objcon.get_value("SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='COTC' AND \"MD_ID\"=:sConditionoftc", NpgsqlCommand);
                        }
                        else
                        {
                            qry = "Data Not Available";
                        }
                        objFailureDetails.sConditionoftc = qry;
                        GetLastRepairedDetails(objFailureDetails);

                    }

                    return objFailureDetails;
                }

                else
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_getfailuredtcdetails");
                    cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailureDetails.sDtcId));
                    dtDetails = objcon.FetchDataTable(cmd);

                    if (dtDetails.Rows.Count > 0)
                    {
                        objFailureDetails.sDtcId = dtDetails.Rows[0]["DT_ID"].ToString();
                        objFailureDetails.sDtcCode = dtDetails.Rows[0]["DT_CODE"].ToString();
                        objFailureDetails.sDtcName = dtDetails.Rows[0]["DT_NAME"].ToString();
                        objFailureDetails.sDtcServicedate = dtDetails.Rows[0]["DT_LAST_SERVICE_DATE"].ToString();
                        objFailureDetails.sDtcLoadKw = dtDetails.Rows[0]["DT_TOTAL_CON_KW"].ToString();
                        objFailureDetails.sDtcLoadHp = dtDetails.Rows[0]["DT_TOTAL_CON_HP"].ToString();
                        objFailureDetails.sCommissionDate = dtDetails.Rows[0]["DT_TRANS_COMMISION_DATE"].ToString();
                        objFailureDetails.sDtcCapacity = dtDetails.Rows[0]["TC_CAPACITY"].ToString();
                        objFailureDetails.sDtcLocation = dtDetails.Rows[0]["TC_LOCATION_ID"].ToString();
                        objFailureDetails.sDtcTcSlno = dtDetails.Rows[0]["TC_SLNO"].ToString();
                        objFailureDetails.sDtcTcMake = dtDetails.Rows[0]["TC_MAKE_ID"].ToString();
                        objFailureDetails.sDtcTcCode = dtDetails.Rows[0]["TC_CODE"].ToString();
                        objFailureDetails.sManfDate = dtDetails.Rows[0]["TC_MANF_DATE"].ToString();
                        objFailureDetails.sTCId = dtDetails.Rows[0]["TC_ID"].ToString();
                        objFailureDetails.sRating = dtDetails.Rows[0]["MD_NAME"].ToString();

                        objFailureDetails.sConditionoftc = dtDetails.Rows[0]["TC_CONDITION"].ToString();
                        string qry = string.Empty;
                        if (objFailureDetails.sConditionoftc != "" && objFailureDetails.sConditionoftc != "0")
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sConditionoftc", Convert.ToDouble(objFailureDetails.sConditionoftc));
                            qry = objcon.get_value("SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='COTC' AND \"MD_ID\"=:sConditionoftc", NpgsqlCommand);
                        }
                        else
                        {
                            qry = "Data Not Available";
                        }

                        objFailureDetails.sConditionoftc = qry;
                        if (objFailureDetails.sEnhancedCapacity == null || objFailureDetails.sEnhancedCapacity == "")
                        {
                            objFailureDetails.sEnhancedCapacity = objFailureDetails.sDtcCapacity;
                        }

                        GetLastRepairedDetails(objFailureDetails);
                        GetDTRCommissionDate(objFailureDetails);
                        GetEnumerationDate(objFailureDetails);

                    }

                    return objFailureDetails;
                }


            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailureDetails;
            }

        }
        public DataTable getFailId(string sWo_id)
        {
            string StrQry = string.Empty;
            DataTable dtEstDetails = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                StrQry = "SELECT \"WO_DATA_ID\",\"WO_WFO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE CAST(\"WO_RECORD_ID\" AS TEXT) LIKE '-%' AND \"WO_ID\"=:sWo_id";
                NpgsqlCommand.Parameters.AddWithValue("sWo_id", Convert.ToInt64(sWo_id));

                dtEstDetails = objcon.FetchDataTable(StrQry, NpgsqlCommand);
                return dtEstDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtEstDetails;
            }
        }
        public clsFailureEntry GetEnumerationDate(clsFailureEntry objFailureDetails)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_getenumerationdate");
                cmd.Parameters.AddWithValue("sdtctccode", Convert.ToString(objFailureDetails.sDtcTcCode));
                cmd.Parameters.AddWithValue("sdtccode", Convert.ToString(objFailureDetails.sDtcCode));

                cmd.Parameters.Add("senumdate", NpgsqlDbType.Text);
                cmd.Parameters["senumdate"].Direction = ParameterDirection.Output;
                DataTable dt = objcon.FetchDataTable(cmd);

                objFailureDetails.sDTrEnumerationDate = dt.Rows[0]["senumdate"].ToString();
                return objFailureDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailureDetails;
            }
        }
        public clsFailureEntry GetLastRepairedDetails(clsFailureEntry objFailureDetails)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                string val = string.Empty;

                NpgsqlCommand cmd = new NpgsqlCommand("sp_lastrepairdetails");
                cmd.Parameters.AddWithValue("sdtctccode", Convert.ToString(objFailureDetails.sDtcTcCode));
                dt = objcon.FetchDataTable(cmd);


                //dt = objcon.getDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    objFailureDetails.sLastRepairedDate = Convert.ToString(dt.Rows[0]["RSD_DELIVARY_DATE"]);
                    objFailureDetails.sLastRepairedBy = Convert.ToString(dt.Rows[0]["SUP_REPNAME"]);
                }

                if (objFailureDetails.sFailureId == "0")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT CAST(\"TC_WARANTY_PERIOD\" AS TEXT) FROM \"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS TEXT) =:sDtcTcCode";
                    NpgsqlCommand.Parameters.AddWithValue("sDtcTcCode", objFailureDetails.sDtcTcCode);
                    val = objcon.get_value(strQry, NpgsqlCommand);

                    if (val != "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN \"RSD_GUARRENTY_TYPE\" ELSE 'AGP' END FROM \"TBLTCMASTER\",\"TBLREPAIRSENTDETAILS\" WHERE ";
                        strQry += " \"TC_CODE\"=\"RSD_TC_CODE\" AND CAST(\"TC_CODE\" AS TEXT)=:sDtcTcCode1 AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL AND \"RSD_WARENTY_PERIOD\" IS NOT NULL";
                        NpgsqlCommand.Parameters.AddWithValue("sDtcTcCode1", objFailureDetails.sDtcTcCode);
                        objFailureDetails.sGuarantyType = objcon.get_value(strQry, NpgsqlCommand);

                        if (objFailureDetails.sGuarantyType == "" || objFailureDetails.sGuarantyType == null)
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN 'WGP' ELSE 'AGP' END FROM \"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS TEXT)=:sDtcTcCode2";
                            NpgsqlCommand.Parameters.AddWithValue("sDtcTcCode2", objFailureDetails.sDtcTcCode);
                            objFailureDetails.sGuarantyType = objcon.get_value(strQry, NpgsqlCommand);
                        }
                    }
                    if (objFailureDetails.sGuarantyType == "" || objFailureDetails.sGuarantyType == null)
                    {
                        objFailureDetails.sGuarantyType = sFirstGuarantyType;
                    }
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"TD_TC_NO\" FROM \"TBLTCDRAWN\" WHERE CAST(\"TD_DF_ID\" AS TEXT)=:sFailureId";
                    NpgsqlCommand.Parameters.AddWithValue("sFailureId", objFailureDetails.sFailureId);
                    string InvoicedTC = objcon.get_value(strQry, NpgsqlCommand);

                    if (InvoicedTC != "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        //strQry = "SELECT \"TC_WARANTY_PERIOD\" FROM \"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS TEXT)=:InvoicedTC";
                        strQry = "SELECT to_char(\"TC_WARANTY_PERIOD\",'DD-MM-YYYY') FROM \"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS numeric)='" + InvoicedTC + "'";
                        NpgsqlCommand.Parameters.AddWithValue("InvoicedTC", InvoicedTC);
                        val = objcon.get_value(strQry, NpgsqlCommand);
                    }

                    if (val != "")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN 'WRGP' ELSE 'AGP' END FROM \"TBLTCMASTER\",\"TBLREPAIRSENTDETAILS\" WHERE ";
                        strQry += " \"TC_CODE\"=\"RSD_TC_CODE\" AND CAST(\"TC_CODE\" AS TEXT)=:InvoicedTC1 AND \"RSD_GUARRENTY_TYPE\" IS NOT NULL AND \"RSD_WARENTY_PERIOD\" IS NOT NULL";
                        NpgsqlCommand.Parameters.AddWithValue("InvoicedTC1", InvoicedTC);
                        objFailureDetails.sGuarantyType = objcon.get_value(strQry, NpgsqlCommand);

                        if (objFailureDetails.sGuarantyType == "" || objFailureDetails.sGuarantyType == null)
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN 'WGP' ELSE 'AGP' END FROM \"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS TEXT) =:InvoicedTC2";
                            NpgsqlCommand.Parameters.AddWithValue("InvoicedTC2", InvoicedTC);
                            objFailureDetails.sGuarantyType = objcon.get_value(strQry, NpgsqlCommand);
                        }
                    }
                    if (objFailureDetails.sGuarantyType == "" || objFailureDetails.sGuarantyType == null)
                    {
                        objFailureDetails.sGuarantyType = sFirstGuarantyType;
                    }

                }
                return objFailureDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailureDetails;
            }
        }
        public bool ValidateUpdate(string strFailureId)
        {
            try
            {
                OleDbDataReader dr;
                DataTable dt = new DataTable();
             
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("strFailureId", strFailureId);
                string sResult = objcon.get_value("select \"WO_DF_ID\" from \"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"WO_DF_ID\"=\"DF_ID\" AND \"DF_ID\" =:strFailureId", NpgsqlCommand);
                if (sResult.Length > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        public clsFailureEntry GetDTRCommissionDate(clsFailureEntry objFailure)
        {
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string strQry = string.Empty;
                strQry = "SELECT TO_CHAR(\"TM_MAPPING_DATE\",'DD/MM/YYYY') TM_MAPPING_DATE FROM \"TBLTRANSDTCMAPPING\" WHERE CAST(\"TM_TC_ID\" AS TEXT)=:sDtcTcCode AND \"TM_DTC_ID\"=:sDtcCode  AND \"TM_LIVE_FLAG\"='1'";
                NpgsqlCommand.Parameters.AddWithValue("sDtcTcCode", objFailure.sDtcTcCode);
                NpgsqlCommand.Parameters.AddWithValue("sDtcCode", objFailure.sDtcCode);
                string sResult = objcon.get_value(strQry, NpgsqlCommand);
                objFailure.sDTrCommissionDate = sResult;
                return objFailure;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailure;
            }
        }
        public void SendSMStoSectionOfficer(string sOfficeCode, string sDTCCode)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                int Division = Convert.ToInt32(ConfigurationManager.AppSettings["Division_code"]);
                if (sOfficeCode.Length > 2)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Division);
                }
                NpgsqlCommand cmd = new NpgsqlCommand("sp_getsectionofficer");
                cmd.Parameters.AddWithValue("sofficecode", Convert.ToString(sOfficeCode));
                cmd.Parameters.AddWithValue("sroleid", "7");
                dt = objcon.FetchDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sFullName = Convert.ToString(dt.Rows[i]["US_FULL_NAME"]);
                    string sMobileNo = Convert.ToString(dt.Rows[i]["US_MOBILE_NO"]);

                    clsCommunication objcomm = new clsCommunication();
                    objcomm.sSMSkey = "SMStoFailure";
                    objcomm = objcomm.GetsmsTempalte(objcomm);

                    string sSMSText = String.Format(objcomm.sSMSTemplate, sDTCCode);
                    //objCommunication.sendSMS(sSMSText, sMobileNo, sFullName);
                    if (objcomm.sSMSTemplateID != null && objcomm.sSMSTemplateID != "")
                    {
                        objcomm.DumpSms(sMobileNo, sSMSText, objcomm.sSMSTemplateID, "WEB");
                    }
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void SendSMStoSDO(string sOfficeCode, string sDTCCode)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();


                int SubDivision = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);


                if (sOfficeCode.Length > 3)
                {
                    sOfficeCode = sOfficeCode.Substring(0, SubDivision);
                }

                //strQry = "SELECT US_FULL_NAME,US_MOBILE_NO,US_EMAIL FROM TBLUSER WHERE US_ROLE_ID IN (1) AND US_OFFICE_CODE='" + sOfficeCode + "'";
                NpgsqlCommand cmd = new NpgsqlCommand("sp_getsectionofficer");
                cmd.Parameters.AddWithValue("sofficecode", Convert.ToString(sOfficeCode));
                cmd.Parameters.AddWithValue("sroleid", "1");
                dt = objcon.FetchDataTable(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sFullName = Convert.ToString(dt.Rows[i]["US_FULL_NAME"]);
                    string sMobileNo = Convert.ToString(dt.Rows[i]["US_MOBILE_NO"]);

                    clsCommunication objCommunication = new clsCommunication();

                    string sSMSText = String.Format(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SMStoFailureCreate"]), sDTCCode);
                    //objCommunication.SaveSMSDetails(sMobileNo,sSMSText);

                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        #region WorkFlow XML

        public clsFailureEntry GetFailureDetailsFromXML(clsFailureEntry objFailure)
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                DataTable dt = new DataTable();

                dt = objApproval.GetDatatableFromXML(objFailure.sWFDataId);
                if (dt.Rows.Count > 0)
                {
                    objFailure.sFailureDate = Convert.ToString(dt.Rows[0]["DF_DATE"]).Trim();
                    objFailure.sFailureReasure = Convert.ToString(dt.Rows[0]["DF_REASON"]).Trim().Replace("ç", ",");
                    objFailure.sDtcCode = Convert.ToString(dt.Rows[0]["DF_DTC_CODE"]).Trim();
                    objFailure.sOfficeCode = Convert.ToString(dt.Rows[0]["DF_LOC_CODE"]);
                    objFailure.sCrby = Convert.ToString(dt.Rows[0]["DF_CRBY"]);

                    objFailure.sFailureType = Convert.ToString(dt.Rows[0]["DF_FAILURE_TYPE"]).Trim();
                    objFailure.sHTBusing = Convert.ToString(dt.Rows[0]["DF_HT_BUSING"]).Trim();
                    objFailure.sLTBusing = Convert.ToString(dt.Rows[0]["DF_LT_BUSING"]).Trim();
                    objFailure.sHTBusingRod = Convert.ToString(dt.Rows[0]["DF_HT_BUSING_ROD"]).Trim();
                    objFailure.sLTBusingRod = Convert.ToString(dt.Rows[0]["DF_LT_BUSING_ROD"]).Trim();
                    objFailure.sBreather = Convert.ToString(dt.Rows[0]["DF_BREATHER"]).Trim();
                    objFailure.sOilLevel = Convert.ToString(dt.Rows[0]["DF_OIL_LEVEL"]).Trim();
                    objFailure.sDrainValve = Convert.ToString(dt.Rows[0]["DF_DRAIN_VALVE"]).Trim();
                    objFailure.sOilQuantity = Convert.ToString(dt.Rows[0]["DF_OIL_QNTY"]).Trim();
                    objFailure.sTankCondition = Convert.ToString(dt.Rows[0]["DF_TANK_CONDITION"]).Trim();
                    objFailure.sExplosionValve = Convert.ToString(dt.Rows[0]["DF_EXPLOSION"]).Trim();
                    objFailure.sDtcReadings = Convert.ToString(dt.Rows[0]["DF_KWH_READING"]);
                    objFailure.sRepairer = Convert.ToString(dt.Rows[0]["DF_REPAIRER_ID"]);
                    objFailure.sdocketno = Convert.ToString(dt.Rows[0]["DF_PGRS_DOCKET"]);
                    objFailure.sCustName = Convert.ToString(dt.Rows[0]["DF_CUSTOMER_NAME"]);
                    objFailure.sCustNo = Convert.ToString(dt.Rows[0]["DF_CUSTOMER_NUMBER"]);
                    objFailure.sMeggerValue = Convert.ToString(dt.Rows[0]["DF_MEGGER_VALUE"]).Trim().Replace("ç", ",");
                    objFailure.sPurpose = Convert.ToString(dt.Rows[0]["DF_PURPOSE"]).Trim().Replace("ç", ",");
                    objFailure.sAlternateReplaceType = Convert.ToString(dt.Rows[0]["DF_ALTERNATE_RPMT"]);
                    objFailure.sDTrCommissionDate = Convert.ToString(dt.Rows[0]["DTR_COMISSION_DATE"]);
                    objFailure.sSilicaCondition = Convert.ToString(dt.Rows[0]["DF_SILC_CONDTN"]);
                    objFailure.sOilType = Convert.ToString(dt.Rows[0]["DF_OIL_TYPE"]);
                    objFailure.sDTCType = Convert.ToString(dt.Rows[0]["DF_DTC_TYPE"]);
                    objFailure.sModem = Convert.ToString(dt.Rows[0]["DF_MODEM"]);

                    if (dt.Columns.Contains("DF_PGRS_DOCKET_DATE"))
                    {
                        objFailure.sdocketDate = Convert.ToString(dt.Rows[0]["DF_PGRS_DOCKET_DATE"]);
                    }
                    if (dt.Columns.Contains("GUARENTEE"))
                    {
                        sFirstGuarantyType = Convert.ToString(dt.Rows[0]["GUARENTEE"]);
                    }
                    if (dt.Columns.Contains("DF_GUARANTY_TYPE_SOURCE"))
                    {
                        objFailure.sGuarantySource = Convert.ToString(dt.Rows[0]["DF_GUARANTY_TYPE_SOURCE"]);
                    }
                    else
                    {
                        objFailure.sGuarantySource = "";
                    }
                    if (dt.Columns.Contains("DF_ENHANCE_CAPACITY"))
                        objFailure.sEnhancedCapacity = Convert.ToString(dt.Rows[0]["DF_ENHANCE_CAPACITY"]);

                    objFailure.sFailureId = "0";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sDtcCode1", objFailure.sDtcCode);
                    objFailure.sDtcId = objcon.get_value("SELECT \"DT_ID\" FROM \"TBLDTCMAST\" WHERE \"DT_CODE\" =:sDtcCode1", NpgsqlCommand);
                    GetFailureDetails(objFailure);
                }
                return objFailure;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objFailure;
            }
        }

        #endregion

        public string getWoIDforEstimation(string sOffCode, string sDtcCode)
        {

            string sWoID = string.Empty;
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();

                sQry = "SELECT \"WO_WFO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_OFFICE_CODE\" =:sOffCode AND \"WO_NEXT_ROLE\" =1 AND \"WO_DATA_ID\" =:sDtcCode ";
                NpgsqlCommand.Parameters.AddWithValue("sOffCode", Convert.ToInt32(sOffCode));
                NpgsqlCommand.Parameters.AddWithValue("sDtcCode", sDtcCode);
                sWoID = objcon.get_value(sQry, NpgsqlCommand);
                return sWoID;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sWoID;
            }

        }
        public string getWfoIDforEstimationSO(string sWOId)
        {
            string sWFOID = string.Empty;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string sQry = string.Empty;
                sQry = "SELECT \"WO_WFO_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWOId ";
                NpgsqlCommand.Parameters.AddWithValue("sWOId", Convert.ToInt32(sWOId));
                sWFOID = objcon.get_value(sQry, NpgsqlCommand);
                return sWFOID;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sWFOID;
            }
        }
        public string GetPreviousApproveStatus(string sWO_ID)
        {
            string sApprove_id = string.Empty;
            //int WO_ID = Convert.ToInt32(sWO_ID) - 1;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWO_ID", sWO_ID);
                //sApprove_id = objcon.get_value("SELECT \"WO_APPROVE_STATUS\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\"='" + WO_ID + "'");
                sApprove_id = objcon.get_value("SELECT \"WO_APPROVE_STATUS\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = (SELECT \"WO_PREV_APPROVE_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE cast(\"WO_ID\" as text) =:sWO_ID)", NpgsqlCommand);
                return sApprove_id;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sApprove_id;
            }
        }
        public bool GetFailDetails_ForPGRS(string sFail_id, string pgrsdocketno, string sClientIP, string sUserId, int roleid, clsFailureEntry objfail, string consumername, string consumernum, string pgrsno = "")
        {
            DataTable dtFailDetails = new DataTable();
            string sStrQry = string.Empty;
            string docket_no = string.Empty;
            string sURL = string.Empty;
            string sURLParam = string.Empty;
            string DTCcd = string.Empty;
            // string roleid = string.Empty;
            try
            {

                //NEW PGRS GENERATION
                NpgsqlCommand = new NpgsqlCommand();

                sStrQry = "SELECT \"DT_CODE\",'" + consumernum + "' AS \"SO_MOBILE_NO\",\"DT_NAME\" AS \"CUST_ADRS\",'TC Failure Complaints' as \"FAIL_TYPE\", ";
                sStrQry += "(SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_ID\"=\"DF_PURPOSE\" and \"MD_TYPE\"='POU')\"PURPOSE\",pgrs_code AS \"SUBDIVISION\"";
                sStrQry += ",\"OM_NAME_PGRS\" AS \"SECTION\",'" + consumername + "' AS ";
                sStrQry += "\"SO_NAME\",pgrs_acc_code AS \"ACC_CODE\",pgrs_rr_num AS \"RR_NUM\",\"DF_ID\" AS \"FAILURE_ID\",'0' AS \"CALL_BACK\", ";
                sStrQry += "\"DT_LATITUDE\" AS \"LATITUDE\",\"DT_LONGITUDE\" AS \"LONGITUDE\",'0000' AS \"FIELD_ID\",\"TC_CAPACITY\",\"TM_NAME\",\"TC_SLNO\",\"DT_TIMS_CODE\",";
                sStrQry += "(SELECT \"FD_FEEDER_NAME\" FROM \"TBLFEEDERMAST\" WHERE \"FD_FEEDER_CODE\"=\"DT_FDRSLNO\")\"FD_FEEDER_NAME\",";
                sStrQry += "to_char(\"DT_TRANS_COMMISION_DATE\",'dd-mm-yyyy')\"DT_TRANS_COMMISION_DATE\",to_char(\"DF_DATE\",'dd-mm-yyyy')\"DF_DATE\",\"DF_GUARANTY_TYPE\" FROM \"TBLDTCFAILURE\" ";
                sStrQry += ",\"TBLUSER\",\"TBLMASTERDATA\",\"TBLDTCMAST\",\"TBLOMSECMAST\",\"TBLTCMASTER\",\"TBLTRANSMAKES\",tblpgrs_code WHERE \"DF_DTC_CODE\"=\"DT_CODE\" and \"DF_EQUIPMENT_ID\"=\"TC_CODE\" and \"TC_MAKE_ID\"=\"TM_ID\" and ";
                sStrQry += "\"DF_CRBY\"=\"US_ID\" and \"MD_ID\"=\"DF_FAILURE_TYPE\" and \"MD_TYPE\"='FT' and \"DF_LOC_CODE\"=\"OM_CODE\" ";
                sStrQry += " AND cast(pgrs_subdiv_code as text) = substr(cast(\"DF_LOC_CODE\" as text),1,4) AND \"DF_ID\"  =:sFail_id";
                NpgsqlCommand.Parameters.AddWithValue("sFail_id", Convert.ToInt64(sFail_id));
                dtFailDetails = objcon.FetchDataTable(sStrQry, NpgsqlCommand);

                if (pgrsdocketno.ToUpper() == "ON" && pgrsno == "")
                {
                    if (dtFailDetails.Rows.Count > 0)
                    {
                        //NEW PGRS GENERATION
                        //string subcategory = Convert.ToString(dtFailDetails.Rows[0]["PURPOSE"]).Replace('\n', ' ').Trim();
                        sURL = "https://bescompgrs.com/";
                        sURLParam = "exicutive/apis/newmithra2019/Docketcreate?action=pgrsdocketcreate_fromapp&mobilenumber=" + dtFailDetails.Rows[0]["SO_MOBILE_NO"] + "";
                        sURLParam += "&consumername=" + dtFailDetails.Rows[0]["SO_NAME"] + "&consumeraddress=" + dtFailDetails.Rows[0]["CUST_ADRS"] + "";
                        sURLParam += "&category=" + dtFailDetails.Rows[0]["FAIL_TYPE"] + "&subcategory=" + Convert.ToString(dtFailDetails.Rows[0]["PURPOSE"]).Replace('\n', ' ').Trim() + "&subdivision=" + dtFailDetails.Rows[0]["SUBDIVISION"] + "";
                        sURLParam += "&onm=" + dtFailDetails.Rows[0]["SECTION"] + "&accountid=" + dtFailDetails.Rows[0]["ACC_CODE"] + "";
                        sURLParam += "&rrnumber=" + dtFailDetails.Rows[0]["RR_NUM"] + "&remarks=" + dtFailDetails.Rows[0]["DT_TIMS_CODE"] + "," + dtFailDetails.Rows[0]["DT_CODE"] + "," + dtFailDetails.Rows[0]["TC_CAPACITY"] + "," + dtFailDetails.Rows[0]["CUST_ADRS"] + "," + sFail_id + "";
                        sURLParam += "&isrequiredcallback=0&latitude=" + dtFailDetails.Rows[0]["LATITUDE"] + "";
                        sURLParam += "&longitude=" + dtFailDetails.Rows[0]["LONGITUDE"] + "";


                        DTCcd = dtFailDetails.Rows[0]["DT_CODE"].ToString();
                        //OLD PGRS GENERATION
                        //sURL = "http://bescom.ipgrs.org/exicutive/apis/bman/api.php";
                        //sURLParam = "?action=createdocket&mnum=" + dtFailDetails.Rows[0]["SO_MOBILE_NO"] + "";
                        //sURLParam += "&caddr=" + dtFailDetails.Rows[0]["SECTION"] + "," + dtFailDetails.Rows[0]["CUST_ADRS"] + "," + dtFailDetails.Rows[0]["TC_CAPACITY"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["TM_NAME"] + "," + dtFailDetails.Rows[0]["TC_SLNO"] + "," + dtFailDetails.Rows[0]["DT_TIMS_CODE"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["PURPOSE"] + "," + dtFailDetails.Rows[0]["FD_FEEDER_NAME"] + "," + dtFailDetails.Rows[0]["DT_TRANS_COMMISION_DATE"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["DF_DATE"] + ",''," + dtFailDetails.Rows[0]["DF_GUARANTY_TYPE"] + "," + dtFailDetails.Rows[0]["SO_NAME"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["SO_MOBILE_NO"] + "&cat=" + dtFailDetails.Rows[0]["FAIL_TYPE"] + "";
                        //sURLParam += " &scat=" + dtFailDetails.Rows[0]["PURPOSE"] + "&sdiv=" + dtFailDetails.Rows[0]["DIV"] + "&onm=" + dtFailDetails.Rows[0]["SECTION"] + "";
                        //sURLParam += " &cname=" + dtFailDetails.Rows[0]["SO_NAME"] + "&caddr=" + dtFailDetails.Rows[0]["SECTION"] + "," + dtFailDetails.Rows[0]["CUST_ADRS"] + "," + dtFailDetails.Rows[0]["TC_CAPACITY"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["TM_NAME"] + "," + dtFailDetails.Rows[0]["TC_SLNO"] + "," + dtFailDetails.Rows[0]["DT_TIMS_CODE"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["PURPOSE"] + "," + dtFailDetails.Rows[0]["FD_FEEDER_NAME"] + "," + dtFailDetails.Rows[0]["DT_TRANS_COMMISION_DATE"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["DF_DATE"] + ",''," + dtFailDetails.Rows[0]["DF_GUARANTY_TYPE"] + "," + dtFailDetails.Rows[0]["SO_NAME"] + ",";
                        //sURLParam += "" + dtFailDetails.Rows[0]["SO_MOBILE_NO"] + "";
                        //sURLParam += " &acc=" + dtFailDetails.Rows[0]["ACC_CODE"] + "&rrn=" + dtFailDetails.Rows[0]["RR_NUM"] + "";
                        //sURLParam += " &remark=DTLMS_" + dtFailDetails.Rows[0]["FAILURE_ID"] + "&isrequiredcallback=" + dtFailDetails.Rows[0]["CALL_BACK"] + "";
                        //sURLParam += " &latitude=" + dtFailDetails.Rows[0]["LATITUDE"] + "&longitude=" + dtFailDetails.Rows[0]["LONGITUDE"] + "&fileid=" + dtFailDetails.Rows[0]["FIELD_ID"] + "";

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("https://bescompgrs.com/");
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        client.Timeout = TimeSpan.FromMinutes(3);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // var postTask = Genaral.PostAsyncSafe(client,"https://bescompgrs.com/", sURLParam);



                        ///      clsException.LogErrorDev(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, sURLParam, "Before");

                        //var postTask = client.GetAsync(sURLParam).Result;

                        string URL = "https://bescompgrs.com/" + sURLParam;
                        ServicePointManager.Expect100Continue = true;
                        //                 ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        //| SecurityProtocolType.Tls11
                        //| SecurityProtocolType.Tls12
                        //| SecurityProtocolType.Ssl3;


                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;


                        WebRequest request = HttpWebRequest.Create(URL);
                        WebResponse response = request.GetResponse();
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string urlText = reader.ReadToEnd();
                        ///         clsException.LogErrorDev(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, urlText + "~" + sURLParam, "After ");

                        string pgrsdocketnolog = Convert.ToString(ConfigurationSettings.AppSettings["logPgrsDocketurl"]).ToUpper();
                        if (pgrsdocketnolog.ToUpper() == "ON")
                        {
                            //clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, sURLParam, postTask.Content.ReadAsStringAsync().Result);
                            ///     clsException.LogErrorDev(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, sURLParam, urlText);
                        }

                        // if (postTask.IsSuccessStatusCode)
                        if (urlText != "")
                        {
                            // var responseBody = postTask.Content.ReadAsStringAsync().Result;
                            if (urlText.Contains("docketnumber"))
                            {
                                // docket_no = responseBody.Split(':').GetValue(1).ToString().Trim('\"');
                                docket_no = urlText.Split(':').GetValue(1).ToString().Trim('\"');
                                docket_no = Convert.ToString(docket_no.Replace('"', ' ').Split(',').GetValue(0)).Trim();
                            }

                            else
                            {
                                string sWO_ID = objcon.get_value("SELECT \"WO_ID\" || '~' ||\"WO_PREV_APPROVE_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = (SELECT max(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9' )");

                                sStrQry = "UPDATE \"TBLPENDINGTRANSACTION\" SET  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "',\"TRANS_WOA_ID\"=null,\"TRANS_NEXT_ROLE_ID\"='" + roleid + "',\"TRANS_BO_ID\"='9' where  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'  ";
                                objcon.ExecuteQry(sStrQry);
                                sStrQry = "DELETE FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" ='" + sWO_ID.Split('~').GetValue(0) + "'";
                                objcon.ExecuteQry(sStrQry);

                                sStrQry = "DELETE FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                                objcon.ExecuteQry(sStrQry);

                                sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" set \"WO_RECORD_ID\"='-" + sFail_id + "' WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9'";
                                objcon.ExecuteQry(sStrQry);

                                sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_APPROVE_STATUS\"='0' WHERE \"WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "'";
                                objcon.ExecuteQry(sStrQry);

                                sStrQry = "DELETE FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + sFail_id + "'";
                                objcon.ExecuteQry(sStrQry);

                                //if (responseBody != "" && responseBody != null && responseBody != string.Empty)
                                //{
                                //    objfail.spgrsstatus = responseBody.Split(':').GetValue(1).ToString().Trim('\"');
                                //}
                                if (urlText != "" && urlText != null && urlText != string.Empty)
                                {
                                    objfail.spgrsstatus = urlText.Split(':').GetValue(1).ToString().Trim('\"');
                                }
                                else
                                {
                                    objfail.spgrsstatus = "-1";
                                }


                                sStrQry = "DELETE FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_ACT_REFTYPE\" ='3' and  \"DRT_ACT_REFNO\"='" + sFail_id + "'";
                                objcon.ExecuteQry(sStrQry);


                                sStrQry = "DELETE FROM \"TBLDTCTRANSACTION\" WHERE \"DCT_DTR_STATUS\" ='3' and  \"DCT_ACT_REFNO\"='" + sFail_id + "'";
                                objcon.ExecuteQry(sStrQry);

                                return false;
                            }

                        }

                        string res = "";

                    }
                    else
                    {
                        string sWO_ID = objcon.get_value("SELECT \"WO_ID\" || '~' ||\"WO_PREV_APPROVE_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = (SELECT max(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9' )");

                        sStrQry = "UPDATE \"TBLPENDINGTRANSACTION\" SET  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "',\"TRANS_WOA_ID\"=null,\"TRANS_NEXT_ROLE_ID\"='" + roleid + "',\"TRANS_BO_ID\"='9' where  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                        objcon.ExecuteQry(sStrQry);

                        sStrQry = "DELETE FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" ='" + sWO_ID.Split('~').GetValue(0) + "'";
                        objcon.ExecuteQry(sStrQry);

                        sStrQry = "DELETE FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                        objcon.ExecuteQry(sStrQry);

                        sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" set \"WO_RECORD_ID\"='-" + sFail_id + "' WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9'";
                        objcon.ExecuteQry(sStrQry);

                        sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_APPROVE_STATUS\"='0' WHERE \"WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "'";
                        objcon.ExecuteQry(sStrQry);

                        sStrQry = "DELETE FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + sFail_id + "'";
                        objcon.ExecuteQry(sStrQry);


                        sStrQry = "DELETE FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + sFail_id + "'";
                        objcon.ExecuteQry(sStrQry);





                        objfail.spgrsstatus = "-1";

                        sStrQry = "DELETE FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_ACT_REFTYPE\" ='3' and  \"DRT_ACT_REFNO\"='" + sFail_id + "'";
                        objcon.ExecuteQry(sStrQry);


                        sStrQry = "DELETE FROM \"TBLDTCTRANSACTION\" WHERE \"DCT_DTR_STATUS\" ='3' and  \"DCT_ACT_REFNO\"='" + sFail_id + "'";
                        objcon.ExecuteQry(sStrQry);

                        return false;
                    }


                }
                else
                {
                    docket_no = pgrsno;
                }

                if (docket_no != "")
                {

                    NpgsqlCommand = new NpgsqlCommand();
                    sStrQry = "UPDATE \"TBLDTCFAILURE\" SET \"DF_PGRS_DOCKET\"=:docket_no WHERE \"DF_ID\"=:sFail_id";
                    NpgsqlCommand.Parameters.AddWithValue("docket_no", docket_no);
                    NpgsqlCommand.Parameters.AddWithValue("sFail_id", Convert.ToInt64(sFail_id));
                    objcon.ExecuteQry(sStrQry, NpgsqlCommand);

                    if (pgrsno != "")
                    {
                        Genaral.PGRSTransaction(sClientIP, sUserId, "PGRS Docket Number ", sFail_id, docket_no, "1");
                    }
                    else
                    {
                        Genaral.PGRSTransaction(sClientIP, sUserId, "PGRS Docket Number ", sFail_id, docket_no, "2", sURL + sURLParam);
                    }



                    return true;
                    // }
                }
                //manual is also not ter
                else
                {
                    string sWO_ID = objcon.get_value("SELECT \"WO_ID\" || '~' ||\"WO_PREV_APPROVE_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = (SELECT max(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9' )");

                    sStrQry = "UPDATE \"TBLPENDINGTRANSACTION\" SET  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "',\"TRANS_WOA_ID\"=null,\"TRANS_NEXT_ROLE_ID\"='" + roleid + "',\"TRANS_BO_ID\"='9' where  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                    objcon.ExecuteQry(sStrQry);
                    sStrQry = "DELETE FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" ='" + sWO_ID.Split('~').GetValue(0) + "'";
                    objcon.ExecuteQry(sStrQry);

                    sStrQry = "DELETE FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                    objcon.ExecuteQry(sStrQry);

                    sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" set \"WO_RECORD_ID\"='-" + sFail_id + "' WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9'";
                    objcon.ExecuteQry(sStrQry);

                    sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_APPROVE_STATUS\"='0' WHERE \"WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "'";
                    objcon.ExecuteQry(sStrQry);

                    sStrQry = "DELETE FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + sFail_id + "'";
                    objcon.ExecuteQry(sStrQry);

                    objfail.spgrsstatus = "-1";

                    sStrQry = "DELETE FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_ACT_REFTYPE\" ='3' and  \"DRT_ACT_REFNO\"='" + sFail_id + "'";
                    objcon.ExecuteQry(sStrQry);


                    sStrQry = "DELETE FROM \"TBLDTCTRANSACTION\" WHERE \"DCT_DTR_STATUS\" ='3' and  \"DCT_ACT_REFNO\"='" + sFail_id + "'";
                    objcon.ExecuteQry(sStrQry);


                    return false;

                }
                //return true;
            }
            catch (Exception ex)
            {
                //NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sFail_id", sFail_id);
                string sWO_ID = objcon.get_value("SELECT \"WO_ID\" || '~' ||\"WO_PREV_APPROVE_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" = (SELECT max(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9' )");
                sStrQry = "UPDATE \"TBLPENDINGTRANSACTION\" SET  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "',\"TRANS_WOA_ID\"=null,\"TRANS_NEXT_ROLE_ID\"='" + roleid + "',\"TRANS_BO_ID\"='9' where  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                objcon.ExecuteQry(sStrQry);

                //sStrQry = "DELETE FROM \"TBLPENDINGTRANSACTION\" where  \"TRANS_WO_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                //objcon.ExecuteQry(sStrQry);

                sStrQry = "DELETE FROM \"TBLPENDINGTRANSACTION\" where  \"TRANS_DTC_CODE\"='" + DTCcd + "' and \"TRANS_BO_ID\"='9'";
                objcon.ExecuteQry(sStrQry);

                sStrQry = "DELETE FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" ='" + sWO_ID.Split('~').GetValue(0) + "'";
                objcon.ExecuteQry(sStrQry);

                sStrQry = "DELETE FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\"='" + sWO_ID.Split('~').GetValue(0) + "'";
                objcon.ExecuteQry(sStrQry);

                sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" set \"WO_RECORD_ID\"='-" + sFail_id + "' WHERE \"WO_RECORD_ID\"='" + sFail_id + "' and \"WO_BO_ID\"='9'";
                objcon.ExecuteQry(sStrQry);

                sStrQry = "UPDATE \"TBLWORKFLOWOBJECTS\" SET \"WO_APPROVE_STATUS\"='0' WHERE \"WO_ID\"='" + sWO_ID.Split('~').GetValue(1) + "'";
                objcon.ExecuteQry(sStrQry);

                sStrQry = "DELETE FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + sFail_id + "'";
                objcon.ExecuteQry(sStrQry);

                objfail.spgrsstatus = "-1";

                sStrQry = "DELETE FROM \"TBLDTRTRANSACTION\" WHERE \"DRT_ACT_REFTYPE\" ='3' and  \"DRT_ACT_REFNO\"='" + sFail_id + "'";
                objcon.ExecuteQry(sStrQry);


                sStrQry = "DELETE FROM \"TBLDTCTRANSACTION\" WHERE \"DCT_DTR_STATUS\" ='3' and  \"DCT_ACT_REFNO\"='" + sFail_id + "'";
                objcon.ExecuteQry(sStrQry);
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message + "Line number :" + line, ex.StackTrace);
                return false;
            }
        }
        public string getFailureType(string sWo_id)
        {
            string wo_id = string.Empty;
            try
            {
                if (sWo_id != "")
                {
                    string sStrQry = "SELECT \"DF_STATUS_FLAG\" || '~' || \"DF_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\" WHERE \"WO_DATA_ID\"=cast(\"DF_ID\" as text) AND \"WO_ID\"='" + Convert.ToInt64(sWo_id) + "'";
                    wo_id = objcon.get_value(sStrQry);
                }
                return wo_id;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public bool saveDtcCommissionDate(clsFailureEntry objFailDetails)
        {
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                string sStrQry = "UPDATE \"TBLDTCMAST\" set \"DT_TRANS_COMMISION_DATE\"='" + objFailDetails.sCommissionDate + "' WHERE \"DT_CODE\"='" + objFailDetails.sDtcCode + "'";
                objcon.ExecuteQry(sStrQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        // added by santhosh for geting the permonent Decommission Entry. 
        public string CheckAlreadyPermanentEstimationEntry(string lblDtcCode)
        {
            try
            {
                string sStrQry = "SELECT \"WO_BO_ID\" from \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\" ='" + Convert.ToString(lblDtcCode) + "'";

                return objcon.get_value(sStrQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }

        }
    }
}
