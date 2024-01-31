using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using IIITS.DTLMS.BL.DataBase;
using System.IO;

namespace IIITS.DTLMS.BL
{
    public class clsEstimation
    {
        string strFormCode = "clsEstimation";

        public string sOfficeCode { get; set; }
        public string sFailureId { get; set; }
        public string sEstimationId { get; set; }
        public string sEstimationNo { get; set; }
        public string sFaultCapacity { get; set; }
        public string sInsulationtype { get; set; }
        public string sCore { get; set; }

        public string sStatusFlag { get; set; }
        public string sReplaceCapacity { get; set; }
        public string sUnit { get; set; }
        public string sQuantity { get; set; }
        public string sUnitPrice { get; set; }
        public string sAmount { get; set; }
        public string sUnitLabour { get; set; }
        public string sTotalLabour { get; set; }
        public string sLabourCharge { get; set; }
        public string s10PercLabourCharge { get; set; }
        public string sContig2Perc { get; set; }
        public string sTotal { get; set; }
        public string sDecommUnitPrice { get; set; }
        public string sDecommUnitLabour { get; set; }
        public string sDecommTotalLabour { get; set; }
        public string sDecommLabourCharge { get; set; }
        public string sDecomm10PercLabourCharge { get; set; }
        public string sDecommContig2Perc { get; set; }
        public string sDecommTotal { get; set; }
        public string sLastRepair { get; set; }

        public string sCrby { get; set; }

        public string sEmployeeCost { get; set; }
        public string sESI { get; set; }
        public string ServiceTax { get; set; }
        public string DecomLabourCost { get; set; }

        public string sFormName { get; set; }
        public string sClientIP { get; set; }


        public string sMaterialID { get; set; }
        public string sMaterialName { get; set; }
        public string sMaterialQnty { get; set; }
        public string sMaterialRate { get; set; }
        public string sMaterialTax { get; set; }
        public string sMaterialTotal { get; set; }
        public string sMaterialunit { get; set; }
        public string sMaterialunitName { get; set; }

        public DataTable dtMaterial { get; set; }
        public DataTable dtLabour { get; set; }
        public DataTable dtSalvage { get; set; }
        public DataTable dtDocuments { get; set; }
        public string sWFO_id { get; set; }
        public string sFailType { get; set; }
        public string sEstComment { get; set; }
        public string sDtrCode { get; set; }
        public string sActionType { get; set; }
        public string sWFDataId { get; set; }
        public string sDtcCode { get; set; }

        public string sWoundType { get; set; }
        public string srateType { get; set; }
        public string sFileId { get; set; }
        public string sFileName { get; set; }
        public string sFileType { get; set; }
        public string sFilePath { get; set; }
        public string sGuaranteetype { get; set; }
        public string sremarks { get; set; }
        public string sEstDate { get; set; }
        public string sFinalTotalAmount { get; set; }
        public string sMaterialItemId { get; set; }

        public string soiltxtvalue { get; set; }

        public string soiltotalvalue { get; set; }

        public string soiltype { get; set; }

        public string sstarrating { get; set; }
        public string sroleId { get; set; }

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        public string GenerateEstimationNo(string sOfficeCode)
        {
            try
            {
                string sMaxNo = string.Empty;
                if (sOfficeCode == null || sOfficeCode == "")
                {
                    sOfficeCode = "";
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                // LIKE :officecode1||'%'


                string sEstNo = objcon.get_value("SELECT CAST(MAX(\"EST_NO\")+1 AS TEXT) FROM \"VIEW_ESTIMATION\" WHERE CAST(\"EST_NO\" AS TEXT) LIKE :sOfficeCode||'%' ", NpgsqlCommand);
                if (sEstNo == "")
                {
                    //sMaxNo = "001";
                    sEstNo = sOfficeCode + "001";
                }
                //else
                //{
                //    sEstNo = objcon.get_value("SELECT MAX(EST_ID)+1 FROM TBLESTIMATION WHERE EST_NO LIKE '" + sOfficeCode + "%'");
                //}            

                return sEstNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GenerateEstimationNo1(string sOfficeCode, DataBseConnection objDatabse)
        {
            try
            {
                string sMaxNo = string.Empty;
                if (sOfficeCode == null || sOfficeCode == "")
                {
                    sOfficeCode = "";
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                // LIKE :officecode1||'%'


               // string sEstNo = objcon.get_value("SELECT CAST(MAX(\"EST_NO\")+1 AS TEXT) FROM \"VIEW_ESTIMATION\" WHERE CAST(\"EST_NO\" AS TEXT) LIKE :sOfficeCode||'%' ", NpgsqlCommand);
                string sEstNo = objDatabse.get_value("SELECT CAST(MAX(\"EST_NO\")+1 AS TEXT) FROM \"VIEW_ESTIMATION\" WHERE CAST(\"EST_NO\" AS TEXT) LIKE '"+ sOfficeCode + "%' ");

                if (sEstNo == "")
                {
                    //sMaxNo = "001";
                    sEstNo = sOfficeCode + "001";
                }
                //else
                //{
                //    sEstNo = objcon.get_value("SELECT MAX(EST_ID)+1 FROM TBLESTIMATION WHERE EST_NO LIKE '" + sOfficeCode + "%'");
                //}            

                return sEstNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        public void SaveEstimationDetails(clsEstimation objEstimation)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);
            string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGERROR"]) + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(sFolderPath))
            {
                Directory.CreateDirectory(sFolderPath);
            }
            string sPath = sFolderPath + "//" + "Main" + DateTime.Now.ToString("yyyyMMdd") + "-ErrorLog.txt";
            try
            {
                int NoofTimes = 0;

                LOOP:
                objDatabse.BeginTransaction();
                string strQry = string.Empty;

                string sEstId = objDatabse.get_value("SELECT \"EST_ID\" FROM \"TBLESTIMATION\" WHERE \"EST_DF_ID\" =" + Convert.ToInt64(objEstimation.sFailureId) + "");
                if (sEstId.Length > 0)
                {
                    return;
                }
                GetCommEstimatedDetails(objEstimation, objDatabse);
                GetDecomEstimatedDetails(objEstimation, objDatabse);

                objEstimation.sEstimationNo = GenerateEstimationNo1(objEstimation.sOfficeCode, objDatabse);

                if (objEstimation.sLastRepair == null || objEstimation.sLastRepair == "")
                {
                    objEstimation.sLastRepair = "0";
                }
                string[] Arr = new string[3];

                //File.AppendAllText(sPath, Environment.NewLine + "SaveEstimationDetails" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss")
                //    + " sFailureId = " + sFailureId + Environment.NewLine + "DTCCODE = " + objEstimation.sDtcCode + Environment.NewLine
                //    + "Estimation No= " + sEstimationNo + Environment.NewLine + "sfaultcapacity = " + sFaultCapacity + Environment.NewLine
                //    + "sreplacecapacity = " + sReplaceCapacity + Environment.NewLine + "sUnit = " + sUnit + Environment.NewLine
                //    + "sQuantity = " + sQuantity + Environment.NewLine + "sUnitPrice = " + sUnitPrice + Environment.NewLine
                //    + "sAmount = " + sAmount + Environment.NewLine + "sUnitLabour = " + sUnitLabour + Environment.NewLine
                //    + "sTotalLabour = " + sTotalLabour + Environment.NewLine + "sTotalLabour = " + sTotalLabour + Environment.NewLine
                //    + "sLabourCharge = " + sLabourCharge + Environment.NewLine + "s10PercLabourCharge= " + s10PercLabourCharge + Environment.NewLine
                //    + "sContig2Perc = " + sContig2Perc + Environment.NewLine + "sTotal = " + sTotal + Environment.NewLine
                //    + "sDecommUnitPrice = " + sDecommUnitPrice + Environment.NewLine + "sDecommUnitLabour = " + sDecommUnitLabour + Environment.NewLine
                //    + "sDecommTotalLabour = " + sDecommTotalLabour + Environment.NewLine + "sDecommLabourCharge = " + sDecommLabourCharge 
                //    + Environment.NewLine
                //    + "sDecomm10PercLabourCharge = " + sDecomm10PercLabourCharge + Environment.NewLine + "sDecommContig2Perc = " + sDecommContig2Perc 
                //    + Environment.NewLine
                //    + "sDecommTotal = " + sDecommTotal + Environment.NewLine + "sCrby = " + sCrby + Environment.NewLine
                //    + "sLastRepair = " + sLastRepair + Environment.NewLine + "sEstDate = " + sEstDate + Environment.NewLine
                //    + "sInsulationtype = " + sInsulationtype + Environment.NewLine + "soiltype = " + soiltype + Environment.NewLine
                //    + "soiltxtvalue = " + soiltxtvalue + Environment.NewLine + "sstarrating = " + sstarrating + Environment.NewLine
                //    + "#######################" + Environment.NewLine);

                //NpgsqlCommand cmd = new NpgsqlCommand("sp_saveestimationdetails"); // old sp kept for reference
                NpgsqlCommand cmd = new NpgsqlCommand("failure_saveestimationdetails");

                cmd.Parameters.AddWithValue("sfailureid", sFailureId);
                cmd.Parameters.AddWithValue("sestimationno", sEstimationNo);
                cmd.Parameters.AddWithValue("sfaultcapacity", sFaultCapacity == "" ? sFaultCapacity : "0");
                cmd.Parameters.AddWithValue("sreplacecapacity", sReplaceCapacity);
                cmd.Parameters.AddWithValue("sunit", sUnit);
                cmd.Parameters.AddWithValue("squantity", sQuantity);
                cmd.Parameters.AddWithValue("sunitprice", sUnitPrice);
                cmd.Parameters.AddWithValue("samount", sAmount);
                cmd.Parameters.AddWithValue("sunitlabour", sUnitLabour);
                cmd.Parameters.AddWithValue("stotallabour", sTotalLabour);
                cmd.Parameters.AddWithValue("slabourcharge", sLabourCharge);
                cmd.Parameters.AddWithValue("s10perclabourcharge", s10PercLabourCharge);
                cmd.Parameters.AddWithValue("scontig2perc", sContig2Perc);
                cmd.Parameters.AddWithValue("stotal", sTotal);
                cmd.Parameters.AddWithValue("sdecommunitprice", sDecommUnitPrice);
                cmd.Parameters.AddWithValue("sdecommunitlabour", sDecommUnitLabour);
                cmd.Parameters.AddWithValue("sdecommtotallabour", sDecommTotalLabour);
                cmd.Parameters.AddWithValue("sdecommlabourcharge", sDecommLabourCharge);
                cmd.Parameters.AddWithValue("sdecomm10perclabourcharge", sDecomm10PercLabourCharge);
                cmd.Parameters.AddWithValue("sdecommcontig2perc", sDecommContig2Perc);
                cmd.Parameters.AddWithValue("sdecommtotal", sDecommTotal);
                cmd.Parameters.AddWithValue("scrby", sCrby);
                cmd.Parameters.AddWithValue("slastrepair", sLastRepair);
                cmd.Parameters.AddWithValue("sestdate", sEstDate == null ? "" : sEstDate);
                cmd.Parameters.AddWithValue("sestinsulationtype", sInsulationtype == null ? "0" : sInsulationtype);
                cmd.Parameters.AddWithValue("oiltype", soiltype == null ? "0" : soiltype);
                cmd.Parameters.AddWithValue("oiltotal", soiltotalvalue == null ? "0" : soiltotalvalue);
                cmd.Parameters.AddWithValue("oilqnty", soiltxtvalue == null ? "0" : soiltxtvalue);
                cmd.Parameters.AddWithValue("oil_star_rating", sstarrating == null ? "0" : sstarrating);

                cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);

                cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                Arr[0] = "pk_id";
                Arr[1] = "op_id";
                Arr[2] = "msg";
                try
                {
                    Arr = objDatabse.Execute(cmd, Arr, 3);
                    objDatabse.CommitTransaction();
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(100);
                    NoofTimes++;
                    if (NoofTimes < 3)
                    {
                        objDatabse.RollBackTrans();
                        goto LOOP;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();
                File.AppendAllText(sPath, Environment.NewLine + "SaveEstimationDetails" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss")
                + Environment.NewLine + "Failureid = " + sFailureId + "Dtccode = " + objEstimation.sDtcCode + Environment.NewLine
                + ex.Message + "#######################" + Environment.NewLine);
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
        }


        public clsEstimation GetCommEstimatedDetails(clsEstimation objEstimation, DataBseConnection objDatabse)
        {

            DataTable dtDetailedReport = new DataTable();
            string strQry = string.Empty;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                sEmployeeCost = ConfigurationSettings.AppSettings["EmployeeCost"];
                sESI = ConfigurationSettings.AppSettings["ESI"];
                ServiceTax = ConfigurationSettings.AppSettings["ServiceTax"];
                DecomLabourCost = ConfigurationSettings.AppSettings["DecomLabourCost"];


                int Division = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
                int SubDivision = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);
                int Section = Convert.ToInt32(ConfigurationSettings.AppSettings["Section_code"]);
                int tcrating;

                strQry = "SELECT \"DF_ENHANCE_CAPACITY\" FROM \"TBLDTCFAILURE\" WHERE CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'";
                string sEnhanceCapacity = objDatabse.get_value(strQry);

                strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'";
                  sStatusFlag  = objDatabse.get_value(strQry);

                if (objEstimation.sstarrating != "0")
                {
                    tcrating = Convert.ToInt32(objEstimation.sstarrating);

                }
                else
                {
                    tcrating = Convert.ToInt32(objDatabse.get_value("SELECT \"TC_RATING\" from \"TBLTCMASTER\",\"TBLDTCFAILURE\"  WHERE  cast(\"DF_EQUIPMENT_ID\" as numeric)=\"TC_CODE\" AND CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'"));

                }


                strQry = " select \"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",TO_CHAR(\"DF_DATE\",'dd/MM/yyyy')DF_DATE,\"DF_LOC_CODE\",(SELECT \"SD_SUBDIV_NAME\" FROM ";
                strQry += " \"TBLSUBDIVMAST\" WHERE CAST(\"SD_SUBDIV_CODE\" AS TEXT) =SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + SubDivision + ")) as SubDivision,(SELECT \"OM_NAME\" FROM \"TBLOMSECMAST\" ";
                strQry += " where CAST(\"OM_CODE\" AS TEXT) =SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + Section + ")) as Location,'No' AS Unit,'1' as Quantity,(select CAST(\"TC_CAPACITY\" AS TEXT) ";
                strQry += " from \"TBLTCMASTER\" where \"TC_CODE\"=\"DF_EQUIPMENT_ID\") Capacity,";
                strQry += " \"TE_RATE\" as Price,1* \"TE_RATE\" AS TotalAmount,\"TE_COMMLABOUR\" as labourcharge,(\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100 as EmployeeCost,";

                // coded by rudra //on 04-03-2020 // total amount count mistake saving in db //  replacing new formule insted of commented Query // only commented for( as FinalTotal) field formule
                //  (\"TE_RATE\"+\"TE_COMMLABOUR\" +((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+((\"TE_COMMLABOUR\" *'" + sESI + "')/100)+((\"TE_COMMLABOUR\" *'" + ServiceTax + "')/100)+((\"TE_RATE\" +((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+ \"TE_COMMLABOUR\" )/100)*2) as FinalTotal

                strQry += " ((\"TE_RATE\"+((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+ \"TE_COMMLABOUR\" )/100)*2 as ContingencyCost, (\"TE_RATE\"+\"TE_COMMLABOUR\" +((\"TE_COMMLABOUR\" *'20')/100)+((\"TE_RATE\" +((\"TE_COMMLABOUR\" *'20')/100)+ \"TE_COMMLABOUR\" )/100)*2) as FinalTotal";
                strQry += " FROM \"TBLDTCFAILURE\",\"TBLITEMMASTER\",\"TBLTCMASTER\",\"TBLTRANSINSULATIONTYPE\"  where CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "' AND \"TC_CODE\"=\"DF_EQUIPMENT_ID\"";
                if (sStatusFlag == "2" || sStatusFlag == "4")
                {
                    strQry += "   AND  "+ sEnhanceCapacity + "=\"TE_CAPACITY\"  and \"TE_TIT_ID\"='" + objEstimation.sInsulationtype + "' and \"TE_TIT_ID\"=\"TIT_ID\"";
                }
                else
                {
                    strQry += " AND  \"TC_CAPACITY\"=\"TE_CAPACITY\"  and \"TE_TIT_ID\"='" + objEstimation.sInsulationtype + "' and \"TE_TIT_ID\"=\"TIT_ID\"";
                }
                if (tcrating > 3)
                {

                    strQry += " AND '0'=COALESCE(\"TE_STAR_RATE\",0)";
                }
                else
                {
                    if (objEstimation.sstarrating != "0")
                    {
                        strQry += " AND '" + objEstimation.sstarrating + "'=COALESCE(\"TE_STAR_RATE\",0)";
                    }
                    else
                    {
                        strQry += " AND COALESCE(\"TC_RATING\",0)=COALESCE(\"TE_STAR_RATE\",0)";
                    }
                }

                dtDetailedReport = objDatabse.FetchDataTable(strQry);

                if (dtDetailedReport.Rows.Count > 0)
                {
                    objEstimation.sFaultCapacity = Convert.ToString(dtDetailedReport.Rows[0]["Capacity"]);
                    if (sStatusFlag == "2" || sStatusFlag == "4")
                    {
                        objEstimation.sReplaceCapacity = Convert.ToString(sEnhanceCapacity);
                    }
                    else
                    {
                        objEstimation.sReplaceCapacity = Convert.ToString(dtDetailedReport.Rows[0]["Capacity"]);
                    }                       
                    objEstimation.sUnit = Convert.ToString(dtDetailedReport.Rows[0]["Unit"]);
                    objEstimation.sQuantity = Convert.ToString(dtDetailedReport.Rows[0]["Quantity"]);
                    objEstimation.sUnitPrice = Convert.ToString(dtDetailedReport.Rows[0]["Price"]);
                    objEstimation.sAmount = Convert.ToString(dtDetailedReport.Rows[0]["TotalAmount"]);
                    objEstimation.sUnitLabour = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.sTotalLabour = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.sLabourCharge = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.s10PercLabourCharge = Convert.ToString(dtDetailedReport.Rows[0]["EmployeeCost"]);
                    objEstimation.sContig2Perc = Convert.ToString(dtDetailedReport.Rows[0]["ContingencyCost"]);
                    objEstimation.sTotal = Convert.ToString(dtDetailedReport.Rows[0]["FinalTotal"]);
                    objEstimation.sDtcCode = Convert.ToString(dtDetailedReport.Rows[0]["DF_DTC_CODE"]);
                    // objEstimation.sInsulationtype = Convert.ToString(dtDetailedReport.Rows[0]["Insulation"]);
                    //objEstimation.sCore = Convert.ToString(dtDetailedReport.Rows[0]["Core"]);
                }
                else
                {
                    objEstimation.sFaultCapacity = "0";
                    objEstimation.sReplaceCapacity = "0";
                    objEstimation.sUnit = "0";
                    objEstimation.sQuantity = "0";
                    objEstimation.sUnitPrice = "0";
                    objEstimation.sAmount = "0";
                    objEstimation.sUnitLabour = "0";
                    objEstimation.sTotalLabour = "0";
                    objEstimation.sLabourCharge = "0";
                    objEstimation.s10PercLabourCharge = "0";
                    objEstimation.sContig2Perc = "0";
                    objEstimation.sTotal = "0";
                    // objEstimation.sInsulationtype = "0";
                    // objEstimation.sCore = "0";
                }

                return objEstimation;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objEstimation;

            }
        }

        public string GetItemDetails(clsEstimation objEstimation)
        {
            DataTable dtDetailedReport = new DataTable();
            string strQry = string.Empty;
            string sResult = string.Empty;
            try
            {
                if(objEstimation.sStatusFlag=="2" || objEstimation.sStatusFlag == "4")
                {
                    strQry = "SELECT \"DF_ENHANCE_CAPACITY\" as \"CAPACITY\",\"TC_RATING\" FROM  \"TBLDTCFAILURE\",\"TBLTCMASTER\" WHERE \"DF_EQUIPMENT_ID\"=\"TC_CODE\" AND  \"DF_ID\"=" + objEstimation.sFailureId + " ";
                    dtDetailedReport = objcon.FetchDataTable(strQry);                  
                }
                else
                {
                    strQry = "SELECT \"TC_CAPACITY\" as \"CAPACITY\",\"TC_RATING\" FROM  \"TBLDTCFAILURE\",\"TBLTCMASTER\" WHERE \"DF_EQUIPMENT_ID\"=\"TC_CODE\" AND  \"DF_ID\"=" + objEstimation.sFailureId + " ";
                    dtDetailedReport = objcon.FetchDataTable(strQry);                 
                }
                 
                if(dtDetailedReport.Rows.Count>0)
                {
                    strQry = "select \"TE_ID\" from \"TBLITEMMASTER\" where \"TE_TIT_ID\"=" + objEstimation.sInsulationtype + " and \"TE_CAPACITY\"="+ Convert.ToInt16(dtDetailedReport.Rows[0]["CAPACITY"])+ " ";
                    if(objEstimation.soiltype != "0")
                    {
                        strQry += " AND '" + objEstimation.sstarrating + "'=COALESCE(\"TE_STAR_RATE\",0)";
                    }
                    else
                    {                                                                      
                     
                        if (Convert.ToInt16(dtDetailedReport.Rows[0]["TC_RATING"]) > 3)
                        {
                            strQry += " AND '0'=COALESCE(\"TE_STAR_RATE\",0)";
                        }
                        else if(Convert.ToString(dtDetailedReport.Rows[0]["TC_RATING"])!=null && Convert.ToString(dtDetailedReport.Rows[0]["TC_RATING"])!="")
                        {
                            strQry += " AND '" + Convert.ToInt16(dtDetailedReport.Rows[0]["TC_RATING"]) + "'= COALESCE(\"TE_STAR_RATE\",0)";
                        }
                        sResult = objcon.get_value(strQry);
                        return sResult;
                    }
                        sResult = objcon.get_value(strQry);
                        return sResult;
                }
                    
                
                return sResult;
            }         
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sResult;

            }
        }
        public clsEstimation GetDecomEstimatedDetails(clsEstimation objEstimation, DataBseConnection objDatabse)
        {
            DataTable dtDetailedReport = new DataTable();
            string strQry = string.Empty;
            try
            {
                int Division = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
                int SubDivision = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);
                int Section = Convert.ToInt32(ConfigurationSettings.AppSettings["Section_code"]);

                strQry = "SELECT \"DF_ENHANCE_CAPACITY\" FROM \"TBLDTCFAILURE\" WHERE CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'";
                string sEnhanceCapacity = objDatabse.get_value(strQry);

                strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'";
                sStatusFlag = objDatabse.get_value(strQry);

                int tcrating;
                if (objEstimation.sstarrating != "0")
                {
                    tcrating = Convert.ToInt32(objEstimation.sstarrating);
                }
                else
                {
                    tcrating = Convert.ToInt32(objDatabse.get_value("SELECT \"TC_RATING\" from \"TBLTCMASTER\",\"TBLDTCFAILURE\"  WHERE  cast(\"DF_EQUIPMENT_ID\" as numeric)=\"TC_CODE\" AND CAST(\"DF_ID\" AS TEXT) ='" + objEstimation.sFailureId + "'"));

                }


                strQry = " SELECT \"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd/MM/yyyy')DF_DATE,\"DF_REASON\",\"DF_LOC_CODE\",TO_CHAR(\"DF_CRON\",'dd/MM/yyyy')DF_CRON,";
                strQry += " \"TE_RATE\" as Price ,(1* \"TE_COMMLABOUR\" *'" + DecomLabourCost + "') as labourcharge,CAST(\"TC_CAPACITY\" AS TEXT)TC_CAPACITY,\"TC_CODE\",\"TC_SLNO\",'OLD' AS Rep,";
                //strQry += " (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\")TM_NAME,\"DT_TOTAL_CON_KW\" ,(\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100 as EmployeeCost,";
                strQry += " (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TC_MAKE_ID\"=\"TM_ID\")TM_NAME,\"DT_TOTAL_CON_KW\" ,(\"TE_COMMLABOUR\" *'" + DecomLabourCost + "' *'" + sEmployeeCost + "')/100 as EmployeeCost,";
                //strQry += " ((((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+(\"TE_COMMLABOUR\" *'" + DecomLabourCost + "'))/100)*2 as ContingencyCost, ";
                strQry += " (((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "'))/100)*2 as ContingencyCost, ";
                //strQry += " ((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "')+((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+((\"TE_COMMLABOUR\" *'" + sESI + "')/100)+ ";
                //strQry += " ((\"TE_COMMLABOUR\" *'" + ServiceTax + "')/100)+((((\"TE_COMMLABOUR\" *'" + sEmployeeCost + "')/100)+(\"TE_COMMLABOUR\" * ";
                //strQry += " '" + DecomLabourCost + "'))/100)*2) as FinalTotal,";
                strQry += " ((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "')+((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "' *'" + sEmployeeCost + "')/100)+((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "' *'" + sESI + "')/100)+ ";
                strQry += " ((\"TE_COMMLABOUR\" *'" + DecomLabourCost + "' *'" + ServiceTax + "')/100)+(((\"TE_COMMLABOUR\" * '" + DecomLabourCost + "'))/100)*2) as FinalTotal,";
                strQry += " 'No' as Unit,'1' as Quantity,(SELECT \"SD_SUBDIV_NAME\" FROM \"TBLSUBDIVMAST\"  WHERE CAST(\"SD_SUBDIV_CODE\" AS TEXT) =SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + SubDivision + ")) as SubDivision ,";
                strQry += " (SELECT \"OM_NAME\" FROM \"TBLOMSECMAST\" where CAST(\"OM_CODE\" AS TEXT)=SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1," + Section + ")) as Location ";
                strQry += "  from  \"TBLDTCFAILURE\",\"TBLITEMMASTER\",\"TBLTCMASTER\",\"TBLDTCMAST\",\"TBLTRANSINSULATIONTYPE\" WHERE \"DF_DTC_CODE\"=\"DT_CODE\" AND \"DF_EQUIPMENT_ID\"=\"TC_CODE\"";
                if(sStatusFlag=="2" || sStatusFlag == "4")
                {
                    strQry += "  AND "+ sEnhanceCapacity + "=\"TE_CAPACITY\"  ";
                }
                    else
                {
                    strQry += "AND \"TC_CAPACITY\"=\"TE_CAPACITY\"  ";
                }                  
                strQry += " AND \"DF_ID\"='" + objEstimation.sFailureId + "' and \"TE_TIT_ID\"='" + objEstimation.sInsulationtype + "' and \"TE_TIT_ID\"=\"TIT_ID\" ";
                if (tcrating > 3)
                {

                    strQry += " AND '0'=COALESCE(\"TE_STAR_RATE\",0)";
                }
                else
                {

                    if (objEstimation.sstarrating != "0")
                    {
                        strQry += " AND '" + objEstimation.sstarrating + "'=COALESCE(\"TE_STAR_RATE\",0) ";
                    }
                    else
                    {
                        strQry += " AND COALESCE(\"TC_RATING\",0)=COALESCE(\"TE_STAR_RATE\",0)";
                    }
                }

                // AND COALESCE(\"TC_RATING\",0)=COALESCE(\"TE_STAR_RATE\",0)";
                dtDetailedReport = objDatabse.FetchDataTable(strQry);
                if (dtDetailedReport.Rows.Count > 0)
                {

                    objEstimation.sDecommUnitPrice = Convert.ToString(dtDetailedReport.Rows[0]["Price"]);
                    //objEstimation.s = Convert.ToString(dtDetailedReport.Rows[0]["TotalAmount"]);
                    objEstimation.sDecommUnitLabour = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.sDecommTotalLabour = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.sDecommLabourCharge = Convert.ToString(dtDetailedReport.Rows[0]["labourcharge"]);
                    objEstimation.sDecomm10PercLabourCharge = Convert.ToString(dtDetailedReport.Rows[0]["EmployeeCost"]);
                    objEstimation.sDecommContig2Perc = Convert.ToString(dtDetailedReport.Rows[0]["ContingencyCost"]);
                    objEstimation.sDecommTotal = Convert.ToString(dtDetailedReport.Rows[0]["FinalTotal"]);
                }
                else
                {
                    objEstimation.sDecommUnitPrice = "0";
                    objEstimation.sDecommUnitLabour = "0";
                    objEstimation.sDecommTotalLabour = "0";
                    objEstimation.sDecommLabourCharge = "0";
                    objEstimation.sDecomm10PercLabourCharge = "0";
                    objEstimation.sDecommContig2Perc = "0";
                    objEstimation.sDecommTotal = "0";

                }
                return objEstimation;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objEstimation;

            }
        }

        public clsEstimation GetCommAndDecommAmount(clsEstimation objEst)
        {
            try
            {
                string strQry = string.Empty;

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getcommanddecommamount");
                cmd.Parameters.AddWithValue("sfailureid", objEst.sFailureId);
                DataTable dt = objcon.FetchDataTable(cmd);

                // strQry = "SELECT EST_TOTAL,EST_DECOM_TOTAL FROM TBLESTIMATION WHERE EST_DF_ID='"+ objEst.sFailureId +"'";
                //DataTable dt = objcon.getDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    objEst.sTotal = Convert.ToString(dt.Rows[0]["EST_TOTAL"]);
                    objEst.sDecommTotal = Convert.ToString(dt.Rows[0]["EST_DECOM_TOTAL"]);
                }

                return objEst;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objEst;

            }
        }

        public string[] SaveEstimation(clsEstimation objFailureDetails, string[] sMaterial, string[] sLabour, string[] sSalvage, string statusflag)
        {
            string[] Arr = new string[4];
            try
            {
                objcon.BeginTransaction();
                string sQry = string.Empty;
                if (objFailureDetails.soiltotalvalue == null || objFailureDetails.soiltotalvalue == string.Empty || objFailureDetails.soiltotalvalue == "")
                {
                    objFailureDetails.soiltotalvalue = "0";
                }

                if (objFailureDetails.soiltype == null || objFailureDetails.soiltype == string.Empty || objFailureDetails.soiltype == "")
                {
                    objFailureDetails.soiltype = "1";
                }
                if (statusflag != "2" && objFailureDetails.sFailType != "2") // Not For Enhancement Record
                {

                    objFailureDetails.sInsulationtype = "0";
                    sQry = "INSERT INTO \"TBLESTIMATIONDETAILS\" (\"EST_ID\", \"EST_FAILUREID\", \"EST_NO\",\"EST_CAPACITY\",\"EST_WOUNDTYPE\",\"EST_REPAIRER\",\"EST_CRBY\",\"EST_FAIL_TYPE\", \"EST_GUARANTEETYPE\",\"EST_ITEM_TOTAL\",\"EST_DATE\",\"EST_INSULATIONTPYE\",\"EST_RATETYPE\",\"EST_TOTALOILVAL\",\"EST_OILTYPE\",\"EST_OIL_QNTY\",\"EST_STAR_RATING\")";
                    sQry += " VALUES('{0}','" + objFailureDetails.sFailureId + "',(SELECT \"ESTIMATIONNUMBER\"('" + objFailureDetails.sOfficeCode + "')), ";
                    sQry += " '" + objFailureDetails.sFaultCapacity + "', '" + objFailureDetails.sWoundType + "', '" + objFailureDetails.sLastRepair + "', '" + objFailureDetails.sCrby + "','" + objFailureDetails.sFailType + "','" + objFailureDetails.sGuaranteetype + "','" + objFailureDetails.sFinalTotalAmount + "', TO_DATE('" + objFailureDetails.sEstDate + "','dd/MM/yyyy'),'" + objFailureDetails.sInsulationtype + "','" + objFailureDetails.srateType + "','" + objFailureDetails.soiltotalvalue + "','" + objFailureDetails.soiltype + "'";
                    sQry += ",'" + objFailureDetails.soiltxtvalue + "','" + objFailureDetails.sstarrating + "')";
                    sQry = sQry.Replace("'", "''");
                }



                string[] sMaterialVal = sMaterial.ToArray();
                string[] sLabourVal = sLabour.ToArray();
                string[] sSalvageVal = sSalvage.ToArray();

                StringBuilder sbQuery = new StringBuilder();

                string sMatID = string.Empty;
                string sMatQnty = string.Empty;
                string sMatRate = string.Empty;
                string sMatTax = string.Empty;
                string sMatTotal = string.Empty;
                string sMatUnit = string.Empty;
                string sMatName = string.Empty;
                string sMatUnitName = string.Empty;
                string sMattaxamount = string.Empty;
                string sMatamount = string.Empty;
                string sMaterialremarks = string.Empty;
                string sLabMaterialremarks = string.Empty;
                string slabtaxamount = string.Empty;
                string slabamount = string.Empty;
                string sMatItemId = string.Empty;

                string sQry1 = string.Empty;
                for (int i = 0; i < sMaterialVal.Length; i++)
                {
                    if (sMaterialVal[i] != null)
                    {
                        if (sMaterialVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(4).ToString()) -
                                (Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(1).ToString()) *
                                Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(2).ToString()));

                            //"ESTM_ITEM_TOTAL" - "ESTM_ITEM_TAX" "AMOUNT"
                            double Amount = 0;
                            Amount = Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry1 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry1 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sMaterialVal[i].Split('~').GetValue(0).ToString() + "', '" + sMaterialVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry1 += " '" + sMaterialVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry1 += " '" + sMaterialVal[i].Split('~').GetValue(4).ToString() + "')";

                            sbQuery.Append(sQry1);
                            sbQuery.Append(";");

                            sMatID += sMaterialVal[i].Split('~').GetValue(0).ToString() + "`";
                            sMatQnty += sMaterialVal[i].Split('~').GetValue(1).ToString() + "`";
                            sMatRate += sMaterialVal[i].Split('~').GetValue(2).ToString() + "`";
                            sMatTax += sMaterialVal[i].Split('~').GetValue(3).ToString() + "`";
                            sMatTotal += sMaterialVal[i].Split('~').GetValue(4).ToString() + "`";
                            sMatUnit += sMaterialVal[i].Split('~').GetValue(5).ToString() + "`";
                            sMatName += sMaterialVal[i].Split('~').GetValue(6).ToString() + "`";
                            sMatUnitName += sMaterialVal[i].Split('~').GetValue(7).ToString() + "`";
                            sMaterialremarks += sMaterialVal[i].Split('~').GetValue(8).ToString() + "`";
                            sMattaxamount += Convert.ToString(TaxAmount) + "`";
                            sMatamount += Convert.ToString(Amount) + "`";
                            sMatItemId += sMaterialVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }


                string sLabID = string.Empty;
                string sLabQnty = string.Empty;
                string sLabRate = string.Empty;
                string sLabTax = string.Empty;
                string sLabTotal = string.Empty;
                string sLabUnit = string.Empty;
                string sLabName = string.Empty;
                string sLabUnitName = string.Empty;
                string sLabItemId = string.Empty;

                string sQry2 = string.Empty;
                for (int i = 0; i < sLabourVal.Length; i++)
                {
                    if (sLabourVal[i] != null)
                    {
                        if (sLabourVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sLabourVal[i].Split('~').GetValue(4).ToString()) -
                                (Convert.ToDouble(sLabourVal[i].Split('~').GetValue(1).ToString()) *
                                Convert.ToDouble(sLabourVal[i].Split('~').GetValue(2).ToString()));

                            double Amount = 0;
                            Amount = Convert.ToDouble(sLabourVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry2 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry2 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sLabourVal[i].Split('~').GetValue(0).ToString() + "', '" + sLabourVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry2 += " '" + sLabourVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry2 += " '" + sLabourVal[i].Split('~').GetValue(4).ToString() + "')";

                            sbQuery.Append(sQry2);
                            sbQuery.Append(";");

                            sLabID += sLabourVal[i].Split('~').GetValue(0).ToString() + "`";
                            sLabQnty += sLabourVal[i].Split('~').GetValue(1).ToString() + "`";
                            sLabRate += sLabourVal[i].Split('~').GetValue(2).ToString() + "`";
                            sLabTax += sLabourVal[i].Split('~').GetValue(3).ToString() + "`";
                            sLabTotal += sLabourVal[i].Split('~').GetValue(4).ToString() + "`";
                            sLabUnit += sLabourVal[i].Split('~').GetValue(5).ToString() + "`";
                            sLabName += sLabourVal[i].Split('~').GetValue(6).ToString() + "`";
                            sLabUnitName += sLabourVal[i].Split('~').GetValue(7).ToString() + "`";
                            sLabMaterialremarks += sLabourVal[i].Split('~').GetValue(8).ToString() + "`";
                            slabtaxamount += Convert.ToString(TaxAmount) + "`";
                            slabamount += Convert.ToString(Amount) + "`";
                            sLabItemId += sLabourVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }


                string sSalID = string.Empty;
                string sSalQnty = string.Empty;
                string sSalRate = string.Empty;
                string sSalTax = string.Empty;
                string sSalTotal = string.Empty;
                string sSalUnit = string.Empty;
                string sSalName = string.Empty;
                string sSalUnitName = string.Empty;
                string sslaveMaterialremarks = string.Empty;
                string sSaltaxamount = string.Empty;
                string sSalamount = string.Empty;
                string sSalItemId = string.Empty;

                string sQry3 = string.Empty;
                for (int i = 0; i < sSalvageVal.Length; i++)
                {
                    if (sSalvageVal[i] != null)
                    {
                        if (sSalvageVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(4).ToString()) -
                            (Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(1).ToString()) *
                            Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(2).ToString()));

                            double Amount = 0;
                            Amount = Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry3 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry3 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sSalvageVal[i].Split('~').GetValue(0).ToString() + "', '" + sSalvageVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry3 += " '" + sSalvageVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry3 += " '" + sSalvageVal[i].Split('~').GetValue(4).ToString() + "')";


                            sbQuery.Append(sQry3);
                            sbQuery.Append(";");

                            sSalID += sSalvageVal[i].Split('~').GetValue(0).ToString() + "`";
                            sSalQnty += sSalvageVal[i].Split('~').GetValue(1).ToString() + "`";
                            sSalRate += sSalvageVal[i].Split('~').GetValue(2).ToString() + "`";
                            sSalTax += sSalvageVal[i].Split('~').GetValue(3).ToString() + "`";
                            sSalTotal += sSalvageVal[i].Split('~').GetValue(4).ToString() + "`";
                            sSalUnit += sSalvageVal[i].Split('~').GetValue(5).ToString() + "`";
                            sSalName += sSalvageVal[i].Split('~').GetValue(6).ToString() + "`";
                            sSalUnitName += sSalvageVal[i].Split('~').GetValue(7).ToString() + "`";
                            sslaveMaterialremarks += sSalvageVal[i].Split('~').GetValue(8).ToString() + "`";
                            sSaltaxamount += Convert.ToString(TaxAmount) + "`";
                            sSalamount += Convert.ToString(Amount) + "`";
                            sSalItemId += sSalvageVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }

                if (objFailureDetails.sFailType == "1") // 1 single coil , 2 multi coil
                {

                    string LocCode = objFailureDetails.sOfficeCode.Substring(0, 3);

                    string sQry4 = string.Empty;
                    sQry4 = "UPDATE \"TBLTCMASTER\" SET \"TC_LOCATION_ID\"='" + clsStoreOffice.GetStoreID(LocCode) + "',\"TC_CURRENT_LOCATION\"='3',\"TC_LAST_FAILURE_TYPE\"='" + objFailureDetails.sFailType + "' WHERE \"TC_CODE\"='" + objFailureDetails.sDtrCode + "'";
                    sbQuery.Append(sQry4);
                    sbQuery.Append(";");
                }
                else
                {
                    string sQry4 = string.Empty;
                    sQry4 = "UPDATE \"TBLTCMASTER\" SET \"TC_LAST_FAILURE_TYPE\"='" + objFailureDetails.sFailType + "' WHERE \"TC_CODE\"='" + objFailureDetails.sDtrCode + "'";
                    sbQuery.Append(sQry4);
                    sbQuery.Append(";");
                }


                string sFileID = string.Empty;
                string sFileName = string.Empty;
                string sFilePath = string.Empty;
                string sFileType = string.Empty;
                string sQry5 = string.Empty;
                DataTable dt = new DataTable();

                if (objFailureDetails.dtDocuments != null)
                {
                    dt = objFailureDetails.dtDocuments;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sQry5 = "INSERT INTO \"TBLESTIMATIONDOCS\" (\"ESD_ID\" , \"ESD_EST_ID\" , \"ESD_FAILURE_ID\", \"ESD_DOC_NAME\", \"ESD_DOC_PATH\" , \"ESD_DOC_TYPE\" ) ";
                            sQry5 += " VALUES ((SELECT COALESCE(MAX(\"ESD_ID\"),0)+1 FROM \"TBLESTIMATIONDOCS\"), '{0}', '" + objFailureDetails.sFailureId + "', ";
                            sQry5 += " '" + Convert.ToString(dt.Rows[i]["NAME"]) + "', '" + Convert.ToString(dt.Rows[i]["PATH"]) + "', '" + Convert.ToString(dt.Rows[i]["TYPE"]) + "' ) ";

                            sbQuery.Append(sQry5);
                            sbQuery.Append(";");

                            sFileID += Convert.ToString(dt.Rows[i]["ID"]) + "`";
                            sFileName += Convert.ToString(dt.Rows[i]["NAME"]) + "`";
                            sFilePath += Convert.ToString(dt.Rows[i]["PATH"]) + "`";
                            sFileType += Convert.ToString(dt.Rows[i]["TYPE"]) + "`";

                        }
                    }
                }



                sbQuery = sbQuery.Replace("'", "''");

                string sParam = "SELECT COALESCE(MAX(\"EST_ID\"),0)+1 FROM \"TBLESTIMATIONDETAILS\"";

                clsApproval objApproval = new clsApproval();
                if (statusflag == "2")//for enhancement
                {
                    objApproval.sbfm_type = "1";
                }
                //FOR THE  CHANGE FROM  NORMAL TO SUBDIVFLOW
                if (Convert.ToInt32(objFailureDetails.sFaultCapacity) < 100 && Convert.ToDouble(objFailureDetails.sFinalTotalAmount) < 10000 && objFailureDetails.sFailType == "1")
                {
                    // objApproval.sPrevWFOId = objFailureDetails.sWFO_id;

                    //FOR THE WOA_BFM_ID CHANGE FROM  NORMAL TO SUBDIVFLOW
                    NpgsqlCommand = new NpgsqlCommand();
                    string strQry1 = "SELECT \"WOA_PREV_APPROVE_ID\"    from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_DESCRIPTION\"  LIKE '%'|| :sDtcCode ||'%' ";

                    NpgsqlCommand.Parameters.AddWithValue("sDtcCode", objFailureDetails.sDtcCode);
                    string sPrevWFOId = objcon.get_value(strQry1, NpgsqlCommand);

                    NpgsqlCommand = new NpgsqlCommand();
                    string strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_BFM_ID\" ='31' WHERE \"WOA_PREV_APPROVE_ID\" =:sPrevWFOId";

                    NpgsqlCommand.Parameters.AddWithValue("sPrevWFOId", Convert.ToInt32(sPrevWFOId));
                    objcon.ExecuteQry(strQry, NpgsqlCommand);


                    objApproval.sFormName = "EstimationCreation_sdo";
                    objApproval.sOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sClientIp = objFailureDetails.sClientIP;
                    objApproval.sCrby = objFailureDetails.sCrby;
                    objApproval.sQryValues = sQry + ";" + sbQuery;
                    objApproval.sParameterValues = sParam;
                    objApproval.sBOId = "73";
                    objApproval.sMainTable = "TBLESTIMATIONDETAILS";
                    objApproval.sDataReferenceId = objFailureDetails.sFailureId;
                    objApproval.sDescription = "Estimation Request for DTC CODE  " + objFailureDetails.sDtcCode;
                    objApproval.sRefOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sWFObjectId = objFailureDetails.sWFO_id;
                }
                else
                {
                    //FOR THE WOA_BFM_ID CHANGE FROM   SUBDIVFLOW TO NORMAL 
                    NpgsqlCommand = new NpgsqlCommand();
                    string strQry1 = "SELECT \"WOA_PREV_APPROVE_ID\"    from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_DESCRIPTION\"  LIKE '%'|| :sDtcCode ||'%' ";

                    NpgsqlCommand.Parameters.AddWithValue("sDtcCode", objFailureDetails.sDtcCode);
                    string sPrevWFOId = objcon.get_value(strQry1, NpgsqlCommand);

                    if (statusflag != "2")//for enhancement
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        string strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_BFM_ID\" ='1' WHERE \"WOA_PREV_APPROVE_ID\" =:sPrevWFOId";

                        NpgsqlCommand.Parameters.AddWithValue("sPrevWFOId", Convert.ToInt32(sPrevWFOId));
                        objcon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    objApproval.sFormName = "EstimationCreation";
                    objApproval.sBOId = "45";
                    // objApproval.sRecordId = objFailureDetails.sFailureId;
                    objApproval.sOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sClientIp = objFailureDetails.sClientIP;
                    objApproval.sCrby = objFailureDetails.sCrby;
                    objApproval.sQryValues = sQry + ";" + sbQuery;
                    objApproval.sParameterValues = sParam;
                    objApproval.sMainTable = "TBLESTIMATIONDETAILS";
                    objApproval.sDataReferenceId = objFailureDetails.sFailureId;
                    objApproval.sDescription = "Estimation Request for DTC CODE  " + objFailureDetails.sDtcCode;
                    objApproval.sRefOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sWFObjectId = objFailureDetails.sWFO_id;
                }

                string sPrimaryKey = "{0}";
                string sSecPrimaryKey = "{1}";


                //" + objFailureDetails.sCore + ",EST_CORE,
                objApproval.sColumnNames = "EST_ID,EST_FAILUREID,EST_NO,EST_INSULATIONTPYE,EST_CORE,EST_CAPACITY,EST_WOUNDTYPE,EST_RATETYPE,EST_REPAIRER,EST_CRBY,EST_FAIL_TYPE,EST_GUARANTEETYPE,EST_DATE,EST_TOTALOILVAL,EST_OILTYPE,EST_OIL_QNTY,EST_STAR_RATING";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ID,NAME,TYPE,PATH";
                objApproval.sColumnValues = "" + sPrimaryKey + "," + objFailureDetails.sFailureId + "," + objFailureDetails.sEstimationNo + "," + objFailureDetails.sInsulationtype + "," + objFailureDetails.sCore + ",";
                objApproval.sColumnValues += "" + objFailureDetails.sFaultCapacity + "," + objFailureDetails.sWoundType + "," + objFailureDetails.srateType + "," + objFailureDetails.sLastRepair + "," + objFailureDetails.sCrby + "," + objFailureDetails.sFailType + "," + objFailureDetails.sGuaranteetype + "," + objFailureDetails.sEstDate + "," + objFailureDetails.soiltotalvalue + "," + objFailureDetails.soiltype + "," + objFailureDetails.soiltxtvalue + "," + objFailureDetails.sstarrating + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sMatID + "," + sMatQnty + "," + sMatRate + "," + sMatTax + "," + sMatTotal + "," + sMatName + "," + sMatUnit + "," + sMatUnitName + "," + sMaterialremarks + "," + sMattaxamount + ", " + sMatamount + "," + sMatItemId + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sLabID + "," + sLabQnty + "," + sLabRate + "," + sLabTax + "," + sLabTotal + "," + sLabName + "," + sLabUnit + "," + sLabUnitName + "," + sLabMaterialremarks + "," + slabtaxamount + "," + slabamount + "," + sLabItemId + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sSalID + "," + sSalQnty + "," + sSalRate + "," + sSalTax + "," + sSalTotal + "," + sSalName + "," + sSalUnit + "," + sSalUnitName + "," + sslaveMaterialremarks + "," + sSaltaxamount + "," + sSalamount + "," + sSalItemId + "";
                objApproval.sColumnValues += ";" + sFileID + "," + sFileName + "," + sFileType + "," + sFilePath + "";
                objApproval.sTableNames = "TBLESTIMATIONDETAILS;TBLESTIMATIONMATERIAL;TBLESTIMATIONLABMATERIAL;TBLESTIMATIONSALMATERIAL;TBLESTIMATIONDOCS";

                if (objFailureDetails.sActionType == "M")
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objFailureDetails.sWFDataId = objApproval.sWFDataId;

                    Arr[2] = objApproval.sWFDataId;
                    Arr[3] = objApproval.sBOId;

                }
                else
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                    objApproval.SaveWorkflowObjects(objApproval);
                    // Arr[4]= objApproval.sWFObjectId;
                    Arr[2] = objApproval.sWFDataId;
                    Arr[3] = objApproval.sRecordId;
                }

                Arr[0] = "Saved Successfully";
                Arr[1] = "0";
                objcon.CommitTransaction();

                return Arr;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public string[] SaveEstimation1(clsEstimation objFailureDetails, string[] sMaterial, string[] sLabour, string[] sSalvage, string statusflag)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);

            string[] Arr = new string[4];
            try
            {
                objDatabse.BeginTransaction();
                string sQry = string.Empty;
                if (objFailureDetails.soiltotalvalue == null || objFailureDetails.soiltotalvalue == string.Empty || objFailureDetails.soiltotalvalue == "")
                {
                    objFailureDetails.soiltotalvalue = "0";
                }

                if (objFailureDetails.soiltype == null || objFailureDetails.soiltype == string.Empty || objFailureDetails.soiltype == "")
                {
                    objFailureDetails.soiltype = "1";
                }
                if (statusflag != "2" && objFailureDetails.sFailType != "2") // Not For Enhancement Record
                {

                    objFailureDetails.sInsulationtype = "0";
                    sQry = "INSERT INTO \"TBLESTIMATIONDETAILS\" (\"EST_ID\", \"EST_FAILUREID\", \"EST_NO\",\"EST_CAPACITY\",\"EST_WOUNDTYPE\",\"EST_REPAIRER\",\"EST_CRBY\",\"EST_FAIL_TYPE\", \"EST_GUARANTEETYPE\",\"EST_ITEM_TOTAL\",\"EST_DATE\",\"EST_INSULATIONTPYE\",\"EST_RATETYPE\",\"EST_TOTALOILVAL\",\"EST_OILTYPE\",\"EST_OIL_QNTY\",\"EST_STAR_RATING\")";
                    sQry += " VALUES('{0}','" + objFailureDetails.sFailureId + "',(SELECT \"ESTIMATIONNUMBER\"('" + objFailureDetails.sOfficeCode + "')), ";
                    sQry += " '" + objFailureDetails.sFaultCapacity + "', '" + objFailureDetails.sWoundType + "', '" + objFailureDetails.sLastRepair + "', '" + objFailureDetails.sCrby + "','" + objFailureDetails.sFailType + "','" + objFailureDetails.sGuaranteetype + "','" + objFailureDetails.sFinalTotalAmount + "', TO_DATE('" + objFailureDetails.sEstDate + "','dd/MM/yyyy'),'" + objFailureDetails.sInsulationtype + "','" + objFailureDetails.srateType + "','" + objFailureDetails.soiltotalvalue + "','" + objFailureDetails.soiltype + "'";
                    sQry += ",'" + objFailureDetails.soiltxtvalue + "','" + objFailureDetails.sstarrating + "')";
                    sQry = sQry.Replace("'", "''");
                }



                string[] sMaterialVal = sMaterial.ToArray();
                string[] sLabourVal = sLabour.ToArray();
                string[] sSalvageVal = sSalvage.ToArray();

                StringBuilder sbQuery = new StringBuilder();

                string sMatID = string.Empty;
                string sMatQnty = string.Empty;
                string sMatRate = string.Empty;
                string sMatTax = string.Empty;
                string sMatTotal = string.Empty;
                string sMatUnit = string.Empty;
                string sMatName = string.Empty;
                string sMatUnitName = string.Empty;
                string sMattaxamount = string.Empty;
                string sMatamount = string.Empty;
                string sMaterialremarks = string.Empty;
                string sLabMaterialremarks = string.Empty;
                string slabtaxamount = string.Empty;
                string slabamount = string.Empty;
                string sMatItemId = string.Empty;

                string sQry1 = string.Empty;
                for (int i = 0; i < sMaterialVal.Length; i++)
                {
                    if (sMaterialVal[i] != null)
                    {
                        if (sMaterialVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(4).ToString()) -
                                (Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(1).ToString()) *
                                Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(2).ToString()));

                            //"ESTM_ITEM_TOTAL" - "ESTM_ITEM_TAX" "AMOUNT"
                            double Amount = 0;
                            Amount = Convert.ToDouble(sMaterialVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry1 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry1 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sMaterialVal[i].Split('~').GetValue(0).ToString() + "', '" + sMaterialVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry1 += " '" + sMaterialVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry1 += " '" + sMaterialVal[i].Split('~').GetValue(4).ToString() + "')";

                            sbQuery.Append(sQry1);
                            sbQuery.Append(";");

                            sMatID += sMaterialVal[i].Split('~').GetValue(0).ToString() + "`";
                            sMatQnty += sMaterialVal[i].Split('~').GetValue(1).ToString() + "`";
                            sMatRate += sMaterialVal[i].Split('~').GetValue(2).ToString() + "`";
                            sMatTax += sMaterialVal[i].Split('~').GetValue(3).ToString() + "`";
                            sMatTotal += sMaterialVal[i].Split('~').GetValue(4).ToString() + "`";
                            sMatUnit += sMaterialVal[i].Split('~').GetValue(5).ToString() + "`";
                            sMatName += sMaterialVal[i].Split('~').GetValue(6).ToString() + "`";
                            sMatUnitName += sMaterialVal[i].Split('~').GetValue(7).ToString() + "`";
                            sMaterialremarks += sMaterialVal[i].Split('~').GetValue(8).ToString() + "`";
                            sMattaxamount += Convert.ToString(TaxAmount) + "`";
                            sMatamount += Convert.ToString(Amount) + "`";
                            sMatItemId += sMaterialVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }


                string sLabID = string.Empty;
                string sLabQnty = string.Empty;
                string sLabRate = string.Empty;
                string sLabTax = string.Empty;
                string sLabTotal = string.Empty;
                string sLabUnit = string.Empty;
                string sLabName = string.Empty;
                string sLabUnitName = string.Empty;
                string sLabItemId = string.Empty;

                string sQry2 = string.Empty;
                for (int i = 0; i < sLabourVal.Length; i++)
                {
                    if (sLabourVal[i] != null)
                    {
                        if (sLabourVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sLabourVal[i].Split('~').GetValue(4).ToString()) -
                                (Convert.ToDouble(sLabourVal[i].Split('~').GetValue(1).ToString()) *
                                Convert.ToDouble(sLabourVal[i].Split('~').GetValue(2).ToString()));

                            double Amount = 0;
                            Amount = Convert.ToDouble(sLabourVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry2 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry2 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sLabourVal[i].Split('~').GetValue(0).ToString() + "', '" + sLabourVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry2 += " '" + sLabourVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry2 += " '" + sLabourVal[i].Split('~').GetValue(4).ToString() + "')";

                            sbQuery.Append(sQry2);
                            sbQuery.Append(";");

                            sLabID += sLabourVal[i].Split('~').GetValue(0).ToString() + "`";
                            sLabQnty += sLabourVal[i].Split('~').GetValue(1).ToString() + "`";
                            sLabRate += sLabourVal[i].Split('~').GetValue(2).ToString() + "`";
                            sLabTax += sLabourVal[i].Split('~').GetValue(3).ToString() + "`";
                            sLabTotal += sLabourVal[i].Split('~').GetValue(4).ToString() + "`";
                            sLabUnit += sLabourVal[i].Split('~').GetValue(5).ToString() + "`";
                            sLabName += sLabourVal[i].Split('~').GetValue(6).ToString() + "`";
                            sLabUnitName += sLabourVal[i].Split('~').GetValue(7).ToString() + "`";
                            sLabMaterialremarks += sLabourVal[i].Split('~').GetValue(8).ToString() + "`";
                            slabtaxamount += Convert.ToString(TaxAmount) + "`";
                            slabamount += Convert.ToString(Amount) + "`";
                            sLabItemId += sLabourVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }


                string sSalID = string.Empty;
                string sSalQnty = string.Empty;
                string sSalRate = string.Empty;
                string sSalTax = string.Empty;
                string sSalTotal = string.Empty;
                string sSalUnit = string.Empty;
                string sSalName = string.Empty;
                string sSalUnitName = string.Empty;
                string sslaveMaterialremarks = string.Empty;
                string sSaltaxamount = string.Empty;
                string sSalamount = string.Empty;
                string sSalItemId = string.Empty;

                string sQry3 = string.Empty;
                for (int i = 0; i < sSalvageVal.Length; i++)
                {
                    if (sSalvageVal[i] != null)
                    {
                        if (sSalvageVal[i].Substring(0, 1) != "~")
                        {
                            double TaxAmount = 0;
                            TaxAmount = Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(4).ToString()) -
                            (Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(1).ToString()) *
                            Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(2).ToString()));

                            double Amount = 0;
                            Amount = Convert.ToDouble(sSalvageVal[i].Split('~').GetValue(4).ToString()) - TaxAmount;

                            sQry3 = "INSERT INTO \"TBLESTIMATIONMATERIAL\" (\"ESTM_ID\", \"ESTM_EST_ID\", \"ESTM_ITEM_ID\", \"ESTM_ITEM_QNTY\", \"ESTM_ITEM_RATE\", \"ESTM_ITEM_TAX\", \"ESTM_ITEM_TOTAL\")";
                            sQry3 += " VALUES ((SELECT COALESCE(MAX(\"ESTM_ID\"),0)+1 FROM \"TBLESTIMATIONMATERIAL\"), '{0}' , '" + sSalvageVal[i].Split('~').GetValue(0).ToString() + "', '" + sSalvageVal[i].Split('~').GetValue(1).ToString() + "', ";
                            sQry3 += " '" + sSalvageVal[i].Split('~').GetValue(2).ToString() + "', '" + TaxAmount + "', ";
                            sQry3 += " '" + sSalvageVal[i].Split('~').GetValue(4).ToString() + "')";


                            sbQuery.Append(sQry3);
                            sbQuery.Append(";");

                            sSalID += sSalvageVal[i].Split('~').GetValue(0).ToString() + "`";
                            sSalQnty += sSalvageVal[i].Split('~').GetValue(1).ToString() + "`";
                            sSalRate += sSalvageVal[i].Split('~').GetValue(2).ToString() + "`";
                            sSalTax += sSalvageVal[i].Split('~').GetValue(3).ToString() + "`";
                            sSalTotal += sSalvageVal[i].Split('~').GetValue(4).ToString() + "`";
                            sSalUnit += sSalvageVal[i].Split('~').GetValue(5).ToString() + "`";
                            sSalName += sSalvageVal[i].Split('~').GetValue(6).ToString() + "`";
                            sSalUnitName += sSalvageVal[i].Split('~').GetValue(7).ToString() + "`";
                            sslaveMaterialremarks += sSalvageVal[i].Split('~').GetValue(8).ToString() + "`";
                            sSaltaxamount += Convert.ToString(TaxAmount) + "`";
                            sSalamount += Convert.ToString(Amount) + "`";
                            sSalItemId += sSalvageVal[i].Split('~').GetValue(9).ToString() + "`";
                        }

                    }
                }

                if (objFailureDetails.sFailType == "1") // 1 single coil , 2 multi coil
                {

                    string LocCode = objFailureDetails.sOfficeCode.Substring(0, 3);

                    string sQry4 = string.Empty;
                    sQry4 = "UPDATE \"TBLTCMASTER\" SET \"TC_LOCATION_ID\"='" + clsStoreOffice.GetStoreID(LocCode) + "',\"TC_CURRENT_LOCATION\"='3',\"TC_LAST_FAILURE_TYPE\"='" + objFailureDetails.sFailType + "' WHERE \"TC_CODE\"='" + objFailureDetails.sDtrCode + "'";
                    sbQuery.Append(sQry4);
                    sbQuery.Append(";");
                }
                else
                {
                    string sQry4 = string.Empty;
                    sQry4 = "UPDATE \"TBLTCMASTER\" SET \"TC_LAST_FAILURE_TYPE\"='" + objFailureDetails.sFailType + "' WHERE \"TC_CODE\"='" + objFailureDetails.sDtrCode + "'";
                    sbQuery.Append(sQry4);
                    sbQuery.Append(";");
                }


                string sFileID = string.Empty;
                string sFileName = string.Empty;
                string sFilePath = string.Empty;
                string sFileType = string.Empty;
                string sQry5 = string.Empty;
                DataTable dt = new DataTable();

                if (objFailureDetails.dtDocuments != null)
                {
                    dt = objFailureDetails.dtDocuments;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sQry5 = "INSERT INTO \"TBLESTIMATIONDOCS\" (\"ESD_ID\" , \"ESD_EST_ID\" , \"ESD_FAILURE_ID\", \"ESD_DOC_NAME\", \"ESD_DOC_PATH\" , \"ESD_DOC_TYPE\" ) ";
                            sQry5 += " VALUES ((SELECT COALESCE(MAX(\"ESD_ID\"),0)+1 FROM \"TBLESTIMATIONDOCS\"), '{0}', '" + objFailureDetails.sFailureId + "', ";
                            sQry5 += " '" + Convert.ToString(dt.Rows[i]["NAME"]) + "', '" + Convert.ToString(dt.Rows[i]["PATH"]) + "', '" + Convert.ToString(dt.Rows[i]["TYPE"]) + "' ) ";

                            sbQuery.Append(sQry5);
                            sbQuery.Append(";");

                            sFileID += Convert.ToString(dt.Rows[i]["ID"]) + "`";
                            sFileName += Convert.ToString(dt.Rows[i]["NAME"]) + "`";
                            sFilePath += Convert.ToString(dt.Rows[i]["PATH"]) + "`";
                            sFileType += Convert.ToString(dt.Rows[i]["TYPE"]) + "`";

                        }
                    }
                }



                sbQuery = sbQuery.Replace("'", "''");

               // string sParam = string.Empty;
               // sParam = Convert.ToString(System.DateTime.Now.ToString("yyMMddHHmmssff"));

                string sParam = "SELECT COALESCE(MAX(\"EST_ID\"),0)+1 FROM \"TBLESTIMATIONDETAILS\"";

                clsApproval objApproval = new clsApproval();
                if (statusflag == "2")//for enhancement
                {
                    objApproval.sbfm_type = "1";
                }
                //FOR THE  CHANGE FROM  NORMAL TO SUBDIVFLOW
                if (Convert.ToInt32(objFailureDetails.sFaultCapacity) < 100 && Convert.ToDouble(objFailureDetails.sFinalTotalAmount) < 10000 && objFailureDetails.sFailType == "1")
                {                 
                    string strQry1 = "SELECT \"WOA_PREV_APPROVE_ID\"    from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_DESCRIPTION\"  LIKE '%"+ objFailureDetails.sDtcCode + "%' ";
                    string sPrevWFOId = objDatabse.get_value(strQry1);
                    string strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_BFM_ID\" ='31' WHERE \"WOA_PREV_APPROVE_ID\" ="+ Convert.ToInt32(sPrevWFOId) + "";           
                    objDatabse.ExecuteQry(strQry);

                    objApproval.sFormName = "EstimationCreation_sdo";
                    objApproval.sOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sClientIp = objFailureDetails.sClientIP;
                    objApproval.sCrby = objFailureDetails.sCrby;
                    objApproval.sQryValues = sQry + ";" + sbQuery;
                    objApproval.sParameterValues = sParam;
                    objApproval.sBOId = "73";
                    objApproval.sMainTable = "TBLESTIMATIONDETAILS";
                    objApproval.sDataReferenceId = objFailureDetails.sFailureId;
                    objApproval.sDescription = "Estimation Request for DTC CODE  " + objFailureDetails.sDtcCode;
                    objApproval.sRefOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sWFObjectId = objFailureDetails.sWFO_id;
                }
                else
                {                  
                    string strQry1 = "SELECT \"WOA_PREV_APPROVE_ID\"    from \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_DESCRIPTION\"  LIKE '%"+ objFailureDetails.sDtcCode + "%' ";             
                    string sPrevWFOId = objDatabse.get_value(strQry1);

                    if (statusflag != "2")
                    {                      
                        string strQry = "UPDATE \"TBLWO_OBJECT_AUTO\" SET \"WOA_BFM_ID\" ='1' WHERE \"WOA_PREV_APPROVE_ID\" =" + Convert.ToInt64(sPrevWFOId) + "";
                        objDatabse.ExecuteQry(strQry);
                    }

                    objApproval.sFormName = "EstimationCreation";
                    objApproval.sBOId = "45";                 
                    objApproval.sOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sClientIp = objFailureDetails.sClientIP;
                    objApproval.sCrby = objFailureDetails.sCrby;
                    objApproval.sQryValues = sQry + ";" + sbQuery;
                    objApproval.sParameterValues = sParam;
                    objApproval.sMainTable = "TBLESTIMATIONDETAILS";
                    objApproval.sDataReferenceId = objFailureDetails.sFailureId;
                    objApproval.sDescription = "Estimation Request for DTC CODE  " + objFailureDetails.sDtcCode;
                    objApproval.sRefOfficeCode = objFailureDetails.sOfficeCode;
                    objApproval.sWFObjectId = objFailureDetails.sWFO_id;
                }

                string sPrimaryKey = "{0}";
                string sSecPrimaryKey = "{1}";
  
                objApproval.sColumnNames = "EST_ID,EST_FAILUREID,EST_NO,EST_INSULATIONTPYE,EST_CORE,EST_CAPACITY,EST_WOUNDTYPE,EST_RATETYPE,EST_REPAIRER,EST_CRBY,EST_FAIL_TYPE,EST_GUARANTEETYPE,EST_DATE,EST_TOTALOILVAL,EST_OILTYPE,EST_OIL_QNTY,EST_STAR_RATING";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ESTM_ID,MRIM_ID,ESTM_ITEM_QNTY,MRI_BASE_RATE,MRI_TAX,MRI_TOTAL,MRIM_ITEM_NAME,MRI_MEASUREMENT,MD_NAME,MRIM_REMARKS,ESTM_ITEM_TAX,AMOUNT,MRIM_ITEM_ID";
                objApproval.sColumnNames += ";ID,NAME,TYPE,PATH";
                objApproval.sColumnValues = "" + sPrimaryKey + "," + objFailureDetails.sFailureId + "," + objFailureDetails.sEstimationNo + "," + objFailureDetails.sInsulationtype + "," + objFailureDetails.sCore + ",";
                objApproval.sColumnValues += "" + objFailureDetails.sFaultCapacity + "," + objFailureDetails.sWoundType + "," + objFailureDetails.srateType + "," + objFailureDetails.sLastRepair + "," + objFailureDetails.sCrby + "," + objFailureDetails.sFailType + "," + objFailureDetails.sGuaranteetype + "," + objFailureDetails.sEstDate + "," + objFailureDetails.soiltotalvalue + "," + objFailureDetails.soiltype + "," + objFailureDetails.soiltxtvalue + "," + objFailureDetails.sstarrating + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sMatID + "," + sMatQnty + "," + sMatRate + "," + sMatTax + "," + sMatTotal + "," + sMatName + "," + sMatUnit + "," + sMatUnitName + "," + sMaterialremarks + "," + sMattaxamount + ", " + sMatamount + "," + sMatItemId + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sLabID + "," + sLabQnty + "," + sLabRate + "," + sLabTax + "," + sLabTotal + "," + sLabName + "," + sLabUnit + "," + sLabUnitName + "," + sLabMaterialremarks + "," + slabtaxamount + "," + slabamount + "," + sLabItemId + "";
                objApproval.sColumnValues += ";" + sSecPrimaryKey + "," + sSalID + "," + sSalQnty + "," + sSalRate + "," + sSalTax + "," + sSalTotal + "," + sSalName + "," + sSalUnit + "," + sSalUnitName + "," + sslaveMaterialremarks + "," + sSaltaxamount + "," + sSalamount + "," + sSalItemId + "";
                objApproval.sColumnValues += ";" + sFileID + "," + sFileName + "," + sFileType + "," + sFilePath + "";
                objApproval.sTableNames = "TBLESTIMATIONDETAILS;TBLESTIMATIONMATERIAL;TBLESTIMATIONLABMATERIAL;TBLESTIMATIONSALMATERIAL;TBLESTIMATIONDOCS";

                if(objFailureDetails.sOfficeCode=="")
                {
                    Arr[0] = "Something went wrong, please approve again";
                    Arr[1] = "2";
                    return Arr;
                }
                //Check for Duplicate Approval
                bool bApproveResult = objApproval.CheckDuplicateApprove(objApproval);
                if (bApproveResult == false)
                {
                    Arr[0] = "Selected Record Already Approved";
                    Arr[1] = "2";
                    return Arr;
                }
              
                    string dupid = objApproval.Checkestimationduplicate(objApproval.sDataReferenceId, objFailureDetails.sroleId);
                    if (dupid != "" && dupid != "0")
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }
                
                if (objFailureDetails.sActionType == "M")
                {
                    objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                    objFailureDetails.sWFDataId = objApproval.sWFDataId;

                    Arr[2] = objApproval.sWFDataId;
                    Arr[3] = objApproval.sBOId;

                }
                else
                {
                    objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                    objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow1(objDatabse);
                    objApproval.SaveWorkflowObjects1(objApproval, objDatabse);
                    // Arr[4]= objApproval.sWFObjectId;
                    Arr[2] = objApproval.sWFDataId;
                    Arr[3] = objApproval.sRecordId;
                }
                objDatabse.CommitTransaction();

                Arr[0] = "Saved Successfully";
                Arr[1] = "0";
       

                return Arr;

            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();
              
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
                throw ex;
               // return Arr;
            }
        }
        public clsEstimation GetDetailsfromMainDB(clsEstimation obj)
        {
            try
            {
                //,\"EST_INSULATIONTPYE\"
                NpgsqlCommand = new NpgsqlCommand();
                String sQry = String.Empty;
                if(obj.sEstimationId=="")
                {
                    return obj;
                }
                sQry = "SELECT \"EST_FAILUREID\",\"EST_CAPACITY\", \"EST_REPAIRER\", \"EST_CRBY\", \"EST_FAIL_TYPE\",\"EST_WOUNDTYPE\",\"EST_RATETYPE\",\"EST_GUARANTEETYPE\",  ";
                sQry += " \"EST_STAR_RATING\", TO_CHAR(\"EST_DATE\", 'dd-MM-yyyy') \"EST_DATE\",\"EST_OILTYPE\",\"EST_OIL_QNTY\",\"EST_TOTALOILVAL\",\"EST_INSULATIONTPYE\" FROM \"TBLESTIMATIONDETAILS\" WHERE \"EST_ID\" = :sEstimationId";
                DataTable dt = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("sEstimationId", Convert.ToInt64(obj.sEstimationId));
                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    obj.sFailureId = Convert.ToString(dt.Rows[0]["EST_FAILUREID"]);
                    obj.sFaultCapacity = Convert.ToString(dt.Rows[0]["EST_CAPACITY"]);
                    obj.sLastRepair = Convert.ToString(dt.Rows[0]["EST_REPAIRER"]);
                    obj.sCrby = Convert.ToString(dt.Rows[0]["EST_CRBY"]);
                    obj.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
                    obj.sWoundType = Convert.ToString(dt.Rows[0]["EST_WOUNDTYPE"]);
                    obj.sEstDate = Convert.ToString(dt.Rows[0]["EST_DATE"]);
                    if (Convert.ToString(dt.Rows[0]["EST_RATETYPE"]) != "" || dt.Rows[0]["EST_RATETYPE"] != null)
                    {
                        obj.srateType = Convert.ToString(dt.Rows[0]["EST_RATETYPE"]);
                    }
                    else
                    {
                        obj.srateType = "2";
                    }


                    if (Convert.ToString(dt.Rows[0]["EST_TOTALOILVAL"] )!= "" && dt.Rows[0]["EST_TOTALOILVAL"] != null)
                    {
                        obj.soiltotalvalue = Convert.ToString(dt.Rows[0]["EST_TOTALOILVAL"]);
                    }

                    else
                    {
                        obj.soiltotalvalue = "0";
                    }

                    if (Convert.ToString(dt.Rows[0]["EST_OILTYPE"]) != "" && dt.Rows[0]["EST_OILTYPE"] != null)
                    {
                        obj.soiltype = Convert.ToString(dt.Rows[0]["EST_OILTYPE"]);
                    }

                    else
                    {
                        obj.soiltype = "0";
                    }


                    if (Convert.ToString(dt.Rows[0]["EST_OIL_QNTY"] )!= "" && dt.Rows[0]["EST_OIL_QNTY"] != null)
                    {
                        obj.soiltxtvalue = Convert.ToString(dt.Rows[0]["EST_OIL_QNTY"]);
                    }

                    else
                    {
                        obj.soiltxtvalue = "0";
                    }

                    if (Convert.ToString(dt.Rows[0]["EST_INSULATIONTPYE"]) != "" && dt.Rows[0]["EST_INSULATIONTPYE"] != null)
                    {
                        obj.sInsulationtype = Convert.ToString(dt.Rows[0]["EST_INSULATIONTPYE"]);

                        obj.sCore = objcon.get_value("SELECT \"TIT_TT_ID\" from \"TBLTRANSINSULATIONTYPE\" WHERE \"TIT_ID\" ='"+ obj.sInsulationtype + "' ");
                    }
                    else
                    {
                        obj.sInsulationtype = "1";
                        obj.sCore = "1";

                    }

                   
                        if (Convert.ToString(dt.Rows[0]["EST_STAR_RATING"]) != "" && dt.Rows[0]["EST_STAR_RATING"] != null)
                        {
                            obj.sstarrating = Convert.ToString(dt.Rows[0]["EST_STAR_RATING"]);
                        }
                    
                    else
                    {
                        obj.sstarrating = "0";
                    }
                    obj.sGuaranteetype = Convert.ToString(dt.Rows[0]["EST_GUARANTEETYPE"]);
                    // obj.sInsulationtype = Convert.ToString(dt.Rows[0]["EST_INSULATIONTPYE"]);
                }
                NpgsqlCommand = new NpgsqlCommand();
                sQry = " SELECT distinct \"ESTM_ITEM_ID\" AS \"MRIM_ID\", \"MRIM_ITEM_NAME\" \"MRIM_ITEM_NAME\" ,\"MRIM_ITEM_ID\" \"MRIM_ITEM_ID\", \"ESTM_ITEM_QNTY\" \"ESTM_ITEM_QNTY\", ";
                sQry += " \"ESTM_ITEM_RATE\" \"MRI_BASE_RATE\", \"ESTM_ITEM_TAX\" \"MRI_TAX\", \"ESTM_ITEM_TOTAL\" \"MRI_TOTAL\",  \"MRI_MEASUREMENT\" ";
                sQry += " \"MRI_MEASUREMENT\", B.\"MD_NAME\" \"MD_NAME\" FROM \"TBLESTIMATIONMATERIAL\", \"TBLMINORREPAIRERITEMMASTER\", ";
                sQry += " \"TBLESTIMATIONDETAILS\", \"TBLMINORREPAIRITEMRATEMASTER\", (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" ";
                sQry += " WHERE \"MD_TYPE\" = 'C')A, (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'MSR')B WHERE ";
                sQry += " \"ESTM_ITEM_ID\" = \"MRIM_ID\"  AND \"EST_ID\" = \"ESTM_EST_ID\" AND \"MRIM_ID\" = \"MRI_MRIM_ID\" AND \"EST_CAPACITY\" = ";
                sQry += " CAST(A.\"MD_NAME\" AS INT) AND A.\"MD_ID\" = \"MRI_CAPACITY\" AND \"ESTM_EST_ID\" =:sEstimationId1 AND \"EST_WOUNDTYPE\" = \"MRI_WOUNDTYPE\" ";
                sQry += " AND \"EST_REPAIRER\" = \"MRI_TR_ID\" AND CAST(\"MRI_MEASUREMENT\" AS INT)  = B.\"MD_ID\" ";
           //     sQry += " AND  \"MRI_STATUS_FLAG\" = 0 ";
                sQry += " AND \"EST_CRON\" BETWEEN \"MRI_EFFECTIVE_FROM\" AND \"MRI_EFFECTIVE_TO\"   AND \"MRIM_ITEM_TYPE\" = 1 ORDER BY \"MRIM_ID\" ";
                //NpgsqlCommand.Parameters.AddWithValue("sEstimationId1", Convert.ToInt64(obj.sEstimationId));
                NpgsqlCommand.Parameters.AddWithValue("sEstimationId1", Convert.ToInt32(obj.sEstimationId));
                obj.dtMaterial = objcon.FetchDataTable(sQry, NpgsqlCommand);

                NpgsqlCommand = new NpgsqlCommand();
                sQry = " SELECT distinct \"ESTM_ITEM_ID\" AS \"MRIM_ID\", \"MRIM_ITEM_NAME\" \"MRIM_ITEM_NAME\" , \"MRIM_ITEM_ID\" \"MRIM_ITEM_ID\",\"ESTM_ITEM_QNTY\" \"ESTM_ITEM_QNTY\", ";
                sQry += " \"ESTM_ITEM_RATE\" \"MRI_BASE_RATE\", \"ESTM_ITEM_TAX\" \"MRI_TAX\", \"ESTM_ITEM_TOTAL\" \"MRI_TOTAL\",  \"MRI_MEASUREMENT\" ";
                sQry += " \"MRI_MEASUREMENT\", B.\"MD_NAME\" \"MD_NAME\" FROM \"TBLESTIMATIONMATERIAL\", \"TBLMINORREPAIRERITEMMASTER\", ";
                sQry += " \"TBLESTIMATIONDETAILS\", \"TBLMINORREPAIRITEMRATEMASTER\", (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" ";
                sQry += " WHERE \"MD_TYPE\" = 'C')A, (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'MSR')B WHERE ";
                sQry += " \"ESTM_ITEM_ID\" = \"MRIM_ID\"  AND \"EST_ID\" = \"ESTM_EST_ID\" AND \"MRIM_ID\" = \"MRI_MRIM_ID\" AND \"EST_CAPACITY\" = ";
                sQry += " CAST(A.\"MD_NAME\" AS INT) AND A.\"MD_ID\" = \"MRI_CAPACITY\" AND \"ESTM_EST_ID\" =:sEstimationId2 AND \"EST_WOUNDTYPE\" = \"MRI_WOUNDTYPE\" ";
                sQry += " AND \"EST_REPAIRER\" = \"MRI_TR_ID\" AND CAST(\"MRI_MEASUREMENT\" AS INT)  = B.\"MD_ID\" ";
               // sQry += " AND  \"MRI_STATUS_FLAG\" = 0 ";
                sQry += " AND \"EST_CRON\" BETWEEN \"MRI_EFFECTIVE_FROM\" AND \"MRI_EFFECTIVE_TO\"   AND \"MRIM_ITEM_TYPE\" = 2 ORDER BY \"MRIM_ID\" ";
                //NpgsqlCommand.Parameters.AddWithValue("sEstimationId2", Convert.ToInt64(obj.sEstimationId));
                NpgsqlCommand.Parameters.AddWithValue("sEstimationId2", Convert.ToInt32(obj.sEstimationId));
                obj.dtLabour = objcon.FetchDataTable(sQry, NpgsqlCommand);

                NpgsqlCommand = new NpgsqlCommand();
                sQry = " SELECT distinct \"ESTM_ITEM_ID\" AS \"MRIM_ID\", \"MRIM_ITEM_NAME\" \"MRIM_ITEM_NAME\" ,\"MRIM_ITEM_ID\" \"MRIM_ITEM_ID\", \"ESTM_ITEM_QNTY\" \"ESTM_ITEM_QNTY\", ";
                sQry += " \"ESTM_ITEM_RATE\" \"MRI_BASE_RATE\", \"ESTM_ITEM_TAX\" \"MRI_TAX\", \"ESTM_ITEM_TOTAL\" \"MRI_TOTAL\",  \"MRI_MEASUREMENT\" ";
                sQry += " \"MRI_MEASUREMENT\", B.\"MD_NAME\" \"MD_NAME\" FROM \"TBLESTIMATIONMATERIAL\", \"TBLMINORREPAIRERITEMMASTER\", ";
                sQry += " \"TBLESTIMATIONDETAILS\", \"TBLMINORREPAIRITEMRATEMASTER\", (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" ";
                sQry += " WHERE \"MD_TYPE\" = 'C')A, (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'MSR')B WHERE ";
                sQry += " \"ESTM_ITEM_ID\" = \"MRIM_ID\"  AND \"EST_ID\" = \"ESTM_EST_ID\" AND \"MRIM_ID\" = \"MRI_MRIM_ID\" AND \"EST_CAPACITY\" = ";
                sQry += " CAST(A.\"MD_NAME\" AS INT) AND A.\"MD_ID\" = \"MRI_CAPACITY\" AND \"ESTM_EST_ID\" =:sEstimationId3 AND \"EST_WOUNDTYPE\" = \"MRI_WOUNDTYPE\" ";
                sQry += " AND \"EST_REPAIRER\" = \"MRI_TR_ID\" AND CAST(\"MRI_MEASUREMENT\" AS INT)  = B.\"MD_ID\" ";
            //    sQry += " AND  \"MRI_STATUS_FLAG\" = 0 ";
                sQry += " AND \"EST_CRON\" BETWEEN \"MRI_EFFECTIVE_FROM\" AND \"MRI_EFFECTIVE_TO\"   AND \"MRIM_ITEM_TYPE\" = 3 ORDER BY \"MRIM_ID\" ";
                //NpgsqlCommand.Parameters.AddWithValue("sEstimationId3", Convert.ToInt64(obj.sEstimationId));
                NpgsqlCommand.Parameters.AddWithValue("sEstimationId3", Convert.ToInt32(obj.sEstimationId));
                obj.dtSalvage = objcon.FetchDataTable(sQry, NpgsqlCommand);


                NpgsqlCommand = new NpgsqlCommand();
                sQry = "SELECT \"ESD_ID\" \"ID\", \"ESD_DOC_TYPE\" \"TYPE\",  \"ESD_DOC_NAME\" \"NAME\", \"ESD_DOC_PATH\" \"PATH\" FROM ";
                sQry += " \"TBLESTIMATIONDOCS\" WHERE  \"ESD_EST_ID\" =:sEstimationId4";
                //NpgsqlCommand.Parameters.AddWithValue("sEstimationId4", Convert.ToInt64(obj.sEstimationId));
                NpgsqlCommand.Parameters.AddWithValue("sEstimationId4", Convert.ToInt32(obj.sEstimationId));
                obj.dtDocuments = objcon.FetchDataTable(sQry, NpgsqlCommand);
                return obj;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return obj;
            }
        }


        public clsEstimation GetEstimateDetailsFromXML(clsEstimation obj)
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                DataTable dt = new DataTable();
                DataTable dtEstimation = new DataTable();

                DataSet ds = new DataSet();

                ds = objApproval.GetDatatableFromMultipleXML(obj.sWFO_id);

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count > 0)
                    {
                        dtEstimation = ds.Tables[i];
                        if (i == 0)
                        {
                            //objStoreIndent.sSiId = Convert.ToString(dtIndentDetails.Rows[0]["SI_ID"]);
                            if (dtEstimation.Columns.Contains("EST_FAILUREID"))
                            {
                                obj.sFailureId = Convert.ToString(dtEstimation.Rows[0]["EST_FAILUREID"]);
                                //obj.sOfficeCode = Convert.ToString(dtEstimation.Rows[0]["SI_DATE"]);
                                obj.sFaultCapacity = Convert.ToString(dtEstimation.Rows[0]["EST_CAPACITY"]);
                                obj.sLastRepair = Convert.ToString(dtEstimation.Rows[0]["EST_REPAIRER"]);
                                obj.sCrby = Convert.ToString(dtEstimation.Rows[0]["EST_CRBY"]);
                                obj.sFailType = Convert.ToString(dtEstimation.Rows[0]["EST_FAIL_TYPE"]);
                                obj.sWoundType = Convert.ToString(dtEstimation.Rows[0]["EST_WOUNDTYPE"]);
                            }
                            if (dtEstimation.Columns.Contains("EST_RATETYPE"))
                            {
                                if (dtEstimation.Rows[0]["EST_RATETYPE"] != "" || dtEstimation.Rows[0]["EST_RATETYPE"] != null)
                                {
                                    obj.srateType = Convert.ToString(dtEstimation.Rows[0]["EST_RATETYPE"]);
                                }
                            }
                            else
                            {
                                obj.srateType = "2";
                            }
                            obj.sGuaranteetype = Convert.ToString(dtEstimation.Rows[0]["EST_GUARANTEETYPE"]);
                            obj.sEstDate = Convert.ToString(dtEstimation.Rows[0]["EST_DATE"]);

                            if (dtEstimation.Columns.Contains("EST_CORE"))
                            {
                                if (dtEstimation.Rows[0]["EST_CORE"] != "" || dtEstimation.Rows[0]["EST_CORE"] != null)
                                {
                                    obj.sCore = Convert.ToString(dtEstimation.Rows[0]["EST_CORE"]);
                                }
                                if (dtEstimation.Rows[0]["EST_INSULATIONTPYE"] != "" || dtEstimation.Rows[0]["EST_INSULATIONTPYE"] != null)
                                {
                                    obj.sInsulationtype = Convert.ToString(dtEstimation.Rows[0]["EST_INSULATIONTPYE"]);
                                }

                            }
                            else
                            {
                                //default values for Core type and Insulation type if they have contain null
                                obj.sCore = "1";
                                obj.sInsulationtype = "1";
                            }

                            if (dtEstimation.Columns.Contains("EST_TOTALOILVAL"))
                            {
                                if (dtEstimation.Rows[0]["EST_TOTALOILVAL"] != "" || dtEstimation.Rows[0]["EST_TOTALOILVAL"] != null)
                                {
                                    obj.soiltotalvalue = Convert.ToString(dtEstimation.Rows[0]["EST_TOTALOILVAL"]);
                                }
                            }
                            else
                            {
                                obj.soiltotalvalue = "0";
                            }
                            if (dtEstimation.Columns.Contains("EST_OILTYPE"))
                            {
                                if (dtEstimation.Rows[0]["EST_OILTYPE"] != "" || dtEstimation.Rows[0]["EST_OILTYPE"] != null)
                                {
                                    obj.soiltype = Convert.ToString(dtEstimation.Rows[0]["EST_OILTYPE"]);
                                }
                            }
                            else
                            {
                                obj.soiltype = "0";
                            }

                            if (dtEstimation.Columns.Contains("EST_OIL_QNTY"))
                            {
                                if (dtEstimation.Rows[0]["EST_OIL_QNTY"] != "" || dtEstimation.Rows[0]["EST_OIL_QNTY"] != null)
                                {
                                    obj.soiltxtvalue = Convert.ToString(dtEstimation.Rows[0]["EST_OIL_QNTY"]);
                                }
                            }
                            else
                            {
                                obj.soiltxtvalue = "0";
                            }

                            if (dtEstimation.Columns.Contains("EST_STAR_RATING"))
                            {
                                if (dtEstimation.Rows[0]["EST_STAR_RATING"] != "" || dtEstimation.Rows[0]["EST_STAR_RATING"] != null)
                                {
                                    obj.sstarrating = Convert.ToString(dtEstimation.Rows[0]["EST_STAR_RATING"]);
                                }
                            }
                            else
                            {
                                obj.sstarrating = "0";
                            }



                        }
                        else if (i == 1)
                        {
                            obj.sMaterialID = Convert.ToString(dtEstimation.Rows[0]["MRIM_ID"]);
                            obj.sMaterialName = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_NAME"]).Replace("ç", ",");
                            obj.sMaterialQnty = Convert.ToString(dtEstimation.Rows[0]["ESTM_ITEM_QNTY"]);
                            obj.sMaterialRate = Convert.ToString(dtEstimation.Rows[0]["MRI_BASE_RATE"]);
                            obj.sMaterialTax = Convert.ToString(dtEstimation.Rows[0]["MRI_TAX"]);
                            obj.sMaterialTotal = Convert.ToString(dtEstimation.Rows[0]["MRI_TOTAL"]);
                            obj.sMaterialunit = Convert.ToString(dtEstimation.Rows[0]["MRI_MEASUREMENT"]);
                            obj.sMaterialunitName = Convert.ToString(dtEstimation.Rows[0]["MD_NAME"]);
                            obj.sAmount = Convert.ToString(dtEstimation.Rows[0]["AMOUNT"]);
                            obj.sMaterialItemId = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_ID"]);
                            obj.dtMaterial = CreateDatatableFromString(obj);
                        }
                        else if (i == 2)
                        {
                            obj.sMaterialID = Convert.ToString(dtEstimation.Rows[0]["MRIM_ID"]);
                            obj.sMaterialName = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_NAME"]).Replace("ç", ",");
                            obj.sMaterialQnty = Convert.ToString(dtEstimation.Rows[0]["ESTM_ITEM_QNTY"]);
                            obj.sMaterialRate = Convert.ToString(dtEstimation.Rows[0]["MRI_BASE_RATE"]);
                            obj.sMaterialTax = Convert.ToString(dtEstimation.Rows[0]["MRI_TAX"]);
                            obj.sMaterialTotal = Convert.ToString(dtEstimation.Rows[0]["MRI_TOTAL"]);
                            obj.sMaterialunit = Convert.ToString(dtEstimation.Rows[0]["MRI_MEASUREMENT"]);
                            obj.sMaterialunitName = Convert.ToString(dtEstimation.Rows[0]["MD_NAME"]);
                            obj.sAmount = Convert.ToString(dtEstimation.Rows[0]["AMOUNT"]);
                            obj.sMaterialItemId = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_ID"]);
                            obj.dtLabour = CreateDatatableFromString(obj);
                        }
                        else if (i == 3)
                        {
                            obj.sMaterialID = Convert.ToString(dtEstimation.Rows[0]["MRIM_ID"]);
                            obj.sMaterialName = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_NAME"]).Replace("ç", ",");
                            obj.sMaterialQnty = Convert.ToString(dtEstimation.Rows[0]["ESTM_ITEM_QNTY"]);
                            obj.sMaterialRate = Convert.ToString(dtEstimation.Rows[0]["MRI_BASE_RATE"]);
                            obj.sMaterialTax = Convert.ToString(dtEstimation.Rows[0]["MRI_TAX"]);
                            obj.sMaterialTotal = Convert.ToString(dtEstimation.Rows[0]["MRI_TOTAL"]);
                            obj.sMaterialunit = Convert.ToString(dtEstimation.Rows[0]["MRI_MEASUREMENT"]);
                            obj.sAmount = Convert.ToString(dtEstimation.Rows[0]["AMOUNT"]);
                            obj.sMaterialunitName = Convert.ToString(dtEstimation.Rows[0]["MD_NAME"]);
                            obj.sMaterialItemId = Convert.ToString(dtEstimation.Rows[0]["MRIM_ITEM_ID"]);
                            obj.dtSalvage = CreateDatatableFromString(obj);
                        }
                        else if (i == 4)
                        {
                            obj.sFileId = Convert.ToString(dtEstimation.Rows[0]["ID"]);
                            obj.sFileName = Convert.ToString(dtEstimation.Rows[0]["NAME"]);
                            obj.sFileType = Convert.ToString(dtEstimation.Rows[0]["TYPE"]);
                            obj.sFilePath = Convert.ToString(dtEstimation.Rows[0]["PATH"]);
                            obj.dtDocuments = CreateDatatable(obj);
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return obj;
            }
        }


        public DataTable CreateDatatable(clsEstimation objEst)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");
            dt.Columns.Add("TYPE");
            dt.Columns.Add("PATH");

            string[] sFileId = objEst.sFileId.Split('`');
            string[] sFileName = objEst.sFileName.Split('`');
            string[] sFileType = objEst.sFileType.Split('`');
            string[] sFilePath = objEst.sFilePath.Split('`');

            for (int i = 0; i < sFileId.Length; i++)
            {
                for (int j = 0; j < sFileName.Length; j++)
                {
                    for (int k = 0; k < sFileType.Length; k++)
                    {
                        for (int l = 0; l < sFilePath.Length; l++)
                        {
                            if (sFilePath[l] != "" && sFilePath[l] != " ")
                            {
                                DataRow dRow = dt.NewRow();
                                dRow["ID"] = sFileId[i];
                                dRow["NAME"] = sFileName[j];
                                dRow["TYPE"] = sFileType[k];
                                dRow["PATH"] = sFilePath[l];
                                dt.Rows.Add(dRow);
                                dt.AcceptChanges();
                            }
                            i++;
                            j++;
                            k++;
                        }

                    }
                }
            }
            return dt;
        }

        public DataTable CreateDatatableFromString(clsEstimation objEst)
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("MRIM_ID");
            dt.Columns.Add("MRIM_ITEM_NAME");
            dt.Columns.Add("ESTM_ITEM_QNTY");
            dt.Columns.Add("MRI_MEASUREMENT");
            dt.Columns.Add("MRI_BASE_RATE");
            dt.Columns.Add("MRI_TAX");
            dt.Columns.Add("MRI_TOTAL");
            dt.Columns.Add("MD_NAME");
            dt.Columns.Add("MRIM_ITEM_ID");
            dt.Columns.Add("AMOUNT");

            string[] sItemid = objEst.sMaterialID.Split('`');
            string[] sName = objEst.sMaterialName.Split('`');
            string[] sQnty = objEst.sMaterialQnty.Split('`');
            string[] sMeasure = objEst.sMaterialunit.Split('`');
            string[] sRate = objEst.sMaterialRate.Split('`');
            string[] sTax = objEst.sMaterialTax.Split('`');
            string[] sTotal = objEst.sMaterialTotal.Split('`');
            string[] sUnitName = objEst.sMaterialunitName.Split('`');
            string[] sAmounts = objEst.sAmount.Split('`');
            string[] sMaterialItemid = objEst.sMaterialItemId.Split('`');

            for (int i = 0; i < sItemid.Length; i++)
            {
                for (int j = 0; j < sName.Length; j++)
                {
                    for (int k = 0; k < sQnty.Length; k++)
                    {
                        for (int l = 0; l < sMeasure.Length; l++)
                        {
                            for (int m = 0; m < sRate.Length; m++)
                            {
                                for (int q = 0; q < sAmounts.Length; q++)
                                {
                                    for (int n = 0; n < sTax.Length; n++)
                                    {
                                        for (int o = 0; o < sTotal.Length; o++)
                                        {
                                            for (int r = 0; r < sMaterialItemid.Length; r++)
                                            {
                                                for (int p = 0; p < sUnitName.Length; p++)
                                                {
                                                    if (sUnitName[p] != "" && sUnitName[p] != " ")
                                                    {
                                                        DataRow dRow = dt.NewRow();
                                                        dRow["MRIM_ID"] = sItemid[i];
                                                        dRow["MRIM_ITEM_NAME"] = sName[j];
                                                        dRow["ESTM_ITEM_QNTY"] = sQnty[k];
                                                        dRow["MRI_MEASUREMENT"] = sMeasure[l];
                                                        dRow["MRI_BASE_RATE"] = sRate[m];
                                                        dRow["MRI_TAX"] = sTax[n];
                                                        dRow["MRI_TOTAL"] = sTotal[o];
                                                        dRow["MD_NAME"] = sUnitName[p];
                                                        dRow["AMOUNT"] = sAmounts[q];
                                                        dRow["MRIM_ITEM_ID"] = sMaterialItemid[r];
                                                        dt.Rows.Add(dRow);
                                                        dt.AcceptChanges();
                                                    }
                                                    i++;
                                                    j++;
                                                    k++;
                                                    l++;
                                                    m++;
                                                    q++;
                                                    n++;
                                                    o++;
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return dt;
        }

        public bool RepairDateIsValid(string sRepair_Id)
        {
            string StrQry = string.Empty;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                StrQry = "SELECT COUNT(*) FROM \"TBLMINORREPAIRITEMRATEMASTER\" WHERE \"MRI_TR_ID\"=:sRepair_Id AND NOW() BETWEEN \"MRI_EFFECTIVE_FROM\" AND \"MRI_EFFECTIVE_TO\"";
                NpgsqlCommand.Parameters.AddWithValue("sRepair_Id", Convert.ToInt32(sRepair_Id));
                int count = Convert.ToInt16(objcon.get_value(StrQry, NpgsqlCommand));
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool RepairDateIsValidnew(string sRepair_Id,string offcode)
        {
            string StrQry = string.Empty;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                StrQry = "SELECT COUNT(*) FROM \"TBLMINORREPAIRITEMRATEMASTER\" WHERE \"MRI_TR_ID\"=:sRepair_Id AND NOW() BETWEEN \"MRI_EFFECTIVE_FROM\" AND \"MRI_EFFECTIVE_TO\" and \"MRI_DIV_ID\" IN(SELECT \"DIV_ID\" FROM \"TBLDIVISION\" WHERE cast(\"DIV_CODE\" as text) = :offcode) ";
                NpgsqlCommand.Parameters.AddWithValue("sRepair_Id", Convert.ToInt32(sRepair_Id));
                NpgsqlCommand.Parameters.AddWithValue("offcode", offcode);
                int count = Convert.ToInt16(objcon.get_value(StrQry, NpgsqlCommand));
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public string GetFailId(string sFail_Id, string View = "")
        {
            string StrQry = string.Empty;
            string failureId = string.Empty;
            try
            {
                if (sFail_Id != "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    if (sFail_Id.Contains("-"))
                    {
                        StrQry = "SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"=CAST(\"DF_ID\" AS TEXT) AND \"WO_RECORD_ID\"=:sFail_Id";
                        NpgsqlCommand.Parameters.AddWithValue("sFail_Id", Convert.ToInt64(sFail_Id));
                    }
                    else
                    {
                        if (View == "")
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            StrQry = "SELECT \"EST_ID\" FROM \"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND \"DF_ID\"=:sFail_Id1";
                            NpgsqlCommand.Parameters.AddWithValue("sFail_Id1", Convert.ToInt64(sFail_Id));
                        }
                        else
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            StrQry = "SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND \"EST_ID\"=:sFail_Id2";
                            NpgsqlCommand.Parameters.AddWithValue("sFail_Id2", Convert.ToInt64(sFail_Id));
                        }
                    }

                    failureId= objcon.get_value(StrQry, NpgsqlCommand);
                }
                return failureId;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string Getinsulationtype(string sFail_Id)
        {
            string StrQry = string.Empty;
            try
            {
                NpgsqlCommand = new NpgsqlCommand();

                StrQry = "SELECT \"EST_INSULATIONTPYE\" from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"='" + sFail_Id + "'";
                return objcon.get_value(StrQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public DataTable gettcdetails(string sDtrcode)
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDtrcode", Convert.ToDouble(sDtrcode));
                dt = objcon.FetchDataTable(" SELECT \"OM_NAME\",\"TM_NAME\",\"DF_EQUIPMENT_ID\" as \"DTR_CODE\" from  \"TBLDTCFAILURE\" JOIN \"TBLTCMASTER\" ON \"DF_EQUIPMENT_ID\" = \"TC_CODE\" JOIN  \"TBLTRANSMAKES\" ON \"TC_MAKE_ID\" =  \"TM_ID\" JOIN \"TBLOMSECMAST\" ON \"DF_LOC_CODE\" = \"OM_CODE\" WHERE \"DF_EQUIPMENT_ID\"=:sDtrcode", NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }

        }

        public string GetDivId(string sOfficeCode)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(sOfficeCode));
                string sEstId = objcon.get_value("select \"DIV_ID\" from \"TBLDIVISION\" where \"DIV_CODE\"=:sFailureId", NpgsqlCommand);
                if (sEstId.Length > 0)
                {
                    return sEstId;
                }
                return sOfficeCode;
            }
            catch (Exception ex)
            {

                return sOfficeCode;
            }
        }


        public DataTable GetRepairName(string sLastRepair)
        {
            DataTable dt = new DataTable();
            try
            {

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRepairId", Convert.ToInt32(sLastRepair));
                dt = objcon.FetchDataTable("SELECT  UPPER(\"TR_NAME\") \"TR_NAME\" FROM \"TBLTRANSREPAIRER\" where \"TR_ID\" = :sRepairId", NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public string getoilqnty(string capacity, string starrating)
        {
            try
            {


                string soilqnty = objcon.get_value("SELECT \"OL_QNTY\" FROM \"TBLOILQNTY\" WHERE \"OL_CAPACITY\"='" + capacity + "' AND \"OL_STAR_RATING\"='" + starrating + "'");
                if (soilqnty == "")
                {

                    soilqnty = "0";
                }


                return soilqnty;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public DataTable Getoildetails(string sFail_Id)
        {
            string StrQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand = new NpgsqlCommand();

                StrQry = "SELECT \"EST_TOTALOILVAL\",\"EST_OILTYPE\",\"EST_OIL_QNTY\",\"EST_STAR_RATING\" from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"='" + sFail_Id + "'";
                dt = objcon.FetchDataTable(StrQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public DataTable GetEstDetails(clsEstimation obj)
        {
            DataTable dt = new DataTable();
            try
            {
                string qry = "SELECT \"EST_FAIL_TYPE\",\"EST_OILTYPE\",\"EST_OIL_QNTY\",\"EST_TOTALOILVAL\",\"EST_STAR_RATING\" from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"='" + obj.sFailureId + "'";
                dt = objcon.FetchDataTable(qry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public string getWoDataId(string sFailureId)
        {
            string sQry = string.Empty;
            string sWoDataId = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();


            sQry = "SELECT MAX(\"WO_WFO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"='" + Convert.ToInt64(sFailureId) + "' AND \"WO_BO_ID\"='45'";
            // NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(sFailureId));
            sWoDataId = objcon.get_value(sQry);
            return sWoDataId;
        }
    }
}
