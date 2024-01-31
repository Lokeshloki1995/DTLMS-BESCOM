using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{

    public class clsCRReport : clsRIApproval
    {
        //CustOledbConnection objcon = new CustOledbConnection(Constants.Password);
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        string strFormCode = "clsCRReport";
        public clsRIApproval GetDetailsForCR(clsCRReport objRIApproval)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                //strQry = " SELECT TR_APPROVE_REMARKS AS COMMENT_KEEPER,TR_OIL_QUNTY,WO_USER_COMMENT AS COMMENT_OFFICER,TO_CHAR(TR_APPROVED_DATE,'DD/MM/YYYY') TR_APPROVED_DATE,WO_NO,WO_NO_DECOM,TO_CHAR(WO_DATE,'dd/mm/yyyy')WO_DATE,";
                //strQry += " (SELECT US_FULL_NAME FROM TBLUSER WHERE US_ID=WO_CR_BY) AS STORE_OFFICER, (SELECT US_FULL_NAME FROM TBLUSER WHERE US_ID=TR_APPROVED_BY) AS ";
                //strQry += " STORE_KEEPER,TD_DF_ID,TR_RI_NO,TO_CHAR(TR_RI_DATE,'DD-MON-YYYY') TR_RI_DATE,TR_INVENTORY_QTY,TR_DECOM_INV_QTY FROM TBLWORKORDER,TBLINDENT,TBLDTCINVOICE,TBLTCREPLACE,TBLWORKFLOWOBJECTS,TBLTCDRAWN WHERE TR_ID=WO_RECORD_ID AND WO_BO_ID='15' AND TR_IN_NO=IN_NO AND IN_TI_NO=TI_ID AND TI_WO_SLNO=WO_SLNO AND";
                //strQry += " WO_NEXT_ROLE='2' AND WO_RECORD_ID='" + sDecommId + "' AND TD_INV_NO=TR_IN_NO";

                //dt = objcon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getdetailsforcr");
                cmd.Parameters.AddWithValue("sdecommid", sDecommId);
                dt = objcon.FetchDataTable(cmd);

                if (dt.Rows.Count > 0)
                {
                    objRIApproval.sWorkOrderDate = Convert.ToString(dt.Rows[0]["WO_DATE"]);
                    objRIApproval.sComWorkOrder = Convert.ToString(dt.Rows[0]["WO_NO"]);
                    objRIApproval.sDecomWorkOrder = Convert.ToString(dt.Rows[0]["WO_NO_DECOM"]);
                    objRIApproval.sStoreKeeperName = Convert.ToString(dt.Rows[0]["STORE_KEEPER"]);
                    objRIApproval.sStoreOfficerName = Convert.ToString(dt.Rows[0]["STORE_OFFICER"]);
                    objRIApproval.sCommentByStoreKeeper = Convert.ToString(dt.Rows[0]["COMMENT_KEEPER"]);
                    objRIApproval.sCommentByStoreOfficer = Convert.ToString(dt.Rows[0]["COMMENT_OFFICER"]);
                    objRIApproval.sOilQuantity = Convert.ToString(dt.Rows[0]["TR_OIL_QUNTY"]);
                    objRIApproval.sApprovedDate = Convert.ToString(dt.Rows[0]["TR_APPROVED_DATE"]);
                    objRIApproval.sFailureId = Convert.ToString(dt.Rows[0]["TD_DF_ID"]);
                    objRIApproval.sRINo = Convert.ToString(dt.Rows[0]["TR_RI_NO"]);
                    objRIApproval.sRIDate = Convert.ToString(dt.Rows[0]["TR_RI_DATE"]);
                    objRIApproval.sInventoryQty = Convert.ToString(dt.Rows[0]["TR_INVENTORY_QTY"]);
                    objRIApproval.sDecommInventoryQty = Convert.ToString(dt.Rows[0]["TR_DECOM_INV_QTY"]);
                    objRIApproval.sCRDate = Convert.ToString(dt.Rows[0]["TR_CR_DATE"]);

                    objRIApproval.sRVNo= Convert.ToString(dt.Rows[0]["TR_RV_NO"]);
                    objRIApproval.sRVDate = Convert.ToString(dt.Rows[0]["TR_RV_DATE"]);

                    GetDTCTCDetailsFromFailure(objRIApproval);
                }

                return objRIApproval;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objRIApproval;
            }
        }

        public string[] SaveCompletionReport(clsRIApproval objRI)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                #region Workflow


                string strQry1 = "UPDATE \"TBLDTCFAILURE\" SET \"DF_REPLACE_FLAG\" =1, \"DF_REP_DATE\" = NOW() WHERE \"DF_ID\" ='" + objRI.sFailureId + "' ";
                strQry1 += " AND \"DF_REPLACE_FLAG\" =0";

                strQry1 = strQry1.Replace("'", "''");

                string strQry2 = "UPDATE \"TBLTCREPLACE\" SET \"TR_INVENTORY_QTY\" ='" + objRI.sInventoryQty + "',\"TR_CR_DATE\" = '"+ objRI.sCRDate + "',";
                strQry2 += " \"TR_DECOM_INV_QTY\" ='" + objRI.sDecommInventoryQty + "' WHERE \"TR_ID\" ='" + objRI.sDecommId + "'";

                strQry2 = strQry2.Replace("'", "''");

                clsApproval objApproval = new clsApproval();
                objApproval.sFormName = objRI.sFormName;
                objApproval.sRecordId = objRI.sDecommId;
                objApproval.sOfficeCode = objRI.sOfficeCode;
                objApproval.sClientIp = objRI.sClientIP;
                objApproval.sCrby = objRI.sCrby;
                objApproval.sWFObjectId = objRI.sWFObjectId;
                objApproval.sWFAutoId = objRI.sWFAutoId;
                objApproval.sDataReferenceId = objRI.sDTCCode;
                objApproval.sQryValues = strQry1 + ";" + strQry2;

                objApproval.sDescription = "Completion Report For DTC Code " + objRI.sDTCCode;
                
                strQry = "SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\", \"TBLWORKORDER\", \"TBLINDENT\", \"TBLDTCINVOICE\" , \"TBLTCREPLACE\" WHERE \"DF_ID\" = \"WO_DF_ID\" ";
                strQry += " AND \"WO_SLNO\" = \"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND \"IN_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =:sDecommId";
                NpgsqlCommand.Parameters.AddWithValue("sDecommId", Convert.ToDouble(objRI.sDecommId));
                objApproval.sRefOfficeCode = objcon.get_value(strQry, NpgsqlCommand);
                //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();


                objApproval.sColumnNames = "TR_ID,TR_INVENTORY_QTY,TR_DECOM_INV_QTY,TR_CR_DATE";
                objApproval.sColumnValues = "" + objRI.sDecommId + "," + objRI.sInventoryQty + "," + objRI.sDecommInventoryQty + ","+ objRI.sCRDate + "";
                objApproval.sTableNames = "TBLTCREPLACE";

                bool bResult = objApproval.CheckDuplicateApprove(objApproval);
                if (bResult == false)
                {
                    Arr[0] = "Selected Record Already Approved";
                    Arr[1] = "2";
                    return Arr;
                }

                //objApproval.SaveWorkflowObjects(objApproval);

                if (objRI.sActionType == "M")
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objRI.sWFDataId = objApproval.sWFDataId;
                }
                else
                {
                    objApproval.SaveWorkFlowData(objApproval);
                    objRI.sWFDataId = objApproval.sWFDataId;
                    //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                    objApproval.SaveWorkflowObjects(objApproval);
                }

              
                Arr[0] = "Approved Successfully";
                Arr[1] = "0";
                return Arr;
                #endregion
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }


        public string[] SaveCompletionReport1(clsRIApproval objRI)
        {
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);

            string[] Arr = new string[2];
            string strQry = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                #region Workflow

                objDatabse.BeginTransaction();
                string strQry1 = "UPDATE \"TBLDTCFAILURE\" SET \"DF_REPLACE_FLAG\" =1, \"DF_REP_DATE\" = NOW() WHERE \"DF_ID\" ='" + objRI.sFailureId + "' ";
                strQry1 += " AND \"DF_REPLACE_FLAG\" =0";

                strQry1 = strQry1.Replace("'", "''");

                string strQry2 = "UPDATE \"TBLTCREPLACE\" SET \"TR_INVENTORY_QTY\" ='" + objRI.sInventoryQty + "',\"TR_CR_DATE\" = '" + objRI.sCRDate + "',";
                strQry2 += " \"TR_DECOM_INV_QTY\" ='" + objRI.sDecommInventoryQty + "' WHERE \"TR_ID\" ='" + objRI.sDecommId + "'";

                strQry2 = strQry2.Replace("'", "''");

                clsApproval objApproval = new clsApproval();
                objApproval.sFormName = objRI.sFormName;
                objApproval.sRecordId = objRI.sDecommId;
                objApproval.sOfficeCode = objRI.sOfficeCode;
                objApproval.sClientIp = objRI.sClientIP;
                objApproval.sCrby = objRI.sCrby;
                objApproval.sWFObjectId = objRI.sWFObjectId;
                objApproval.sWFAutoId = objRI.sWFAutoId;
                objApproval.sDataReferenceId = objRI.sDTCCode;
                objApproval.sQryValues = strQry1 + ";" + strQry2;

                objApproval.sDescription = "Completion Report For DTC Code " + objRI.sDTCCode;

                strQry = "SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\", \"TBLWORKORDER\", \"TBLINDENT\", \"TBLDTCINVOICE\" , \"TBLTCREPLACE\" WHERE \"DF_ID\" = \"WO_DF_ID\" ";
                strQry += " AND \"WO_SLNO\" = \"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND \"IN_NO\"=\"TR_IN_NO\" AND \"TR_ID\" ='"+ Convert.ToDouble(objRI.sDecommId) + "'";
              //  NpgsqlCommand.Parameters.AddWithValue("sDecommId", Convert.ToDouble(objRI.sDecommId));
                objApproval.sRefOfficeCode = objDatabse.get_value(strQry);
                //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();


                objApproval.sColumnNames = "TR_ID,TR_INVENTORY_QTY,TR_DECOM_INV_QTY,TR_CR_DATE";
                objApproval.sColumnValues = "" + objRI.sDecommId + "," + objRI.sInventoryQty + "," + objRI.sDecommInventoryQty + "," + objRI.sCRDate + "";
                objApproval.sTableNames = "TBLTCREPLACE";

                bool bResult = objApproval.CheckDuplicateApprove1(objApproval, objDatabse);
                if (bResult == false)
                {
                    Arr[0] = "Selected Record Already Approved";
                    Arr[1] = "2";
                    return Arr;
                }

                //objApproval.SaveWorkflowObjects(objApproval);

                if (objRI.sActionType == "M")
                {
                    objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                    objRI.sWFDataId = objApproval.sWFDataId;
                }
                else
                {
                    objApproval.SaveWorkFlowData1(objApproval, objDatabse);
                    objRI.sWFDataId = objApproval.sWFDataId;
                    //objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                    objApproval.SaveWorkflowObjects1(objApproval, objDatabse);
                }


                Arr[0] = "Approved Successfully";
                Arr[1] = "0";
                objDatabse.CommitTransaction();
                return Arr;
                #endregion
            }
            catch (Exception ex)
            {
                objDatabse.RollBackTrans();
                
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
                throw ex;
               // return Arr;
            }
        }

        #region WorkFlow XML

        public clsRIApproval GetCRDetailsFromXML(clsRIApproval objRIApproval)
        {
            try
            {

                clsApproval objApproval = new clsApproval();
                DataTable dt = new DataTable();

                dt = objApproval.GetDatatableFromXML(objRIApproval.sWFDataId);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("TR_INVENTORY_QTY"))
                    {
                        objRIApproval.sInventoryQty = Convert.ToString(dt.Rows[0]["TR_INVENTORY_QTY"]);
                    }
                    if (dt.Columns.Contains("TR_DECOM_INV_QTY"))
                    {
                        objRIApproval.sDecommInventoryQty = Convert.ToString(dt.Rows[0]["TR_DECOM_INV_QTY"]);
                    }
                    if (dt.Columns.Contains("TR_CR_DATE"))
                    {
                        objRIApproval.sCRDate = Convert.ToString(dt.Rows[0]["TR_CR_DATE"]);
                    }
                }
                return objRIApproval;
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Cannot find table 0"))
                {
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                }
                return objRIApproval;
            }
        }

        #endregion

        public DataTable GetCRDetails(string DtcCode)
        {
            DataTable dtCRDetails = new DataTable();
            string strQry = string.Empty;
            try
            {
                //strQry = "SELECT DF_ID,DF_DTC_CODE,DF_EQUIPMENT_ID,WO_NO,WO_NO_DECOM,TO_CHAR(WO_DATE,'DD-MM-YYYY')WO_DATE FROM TBLDTCFAILURE,TBLWORKORDER ";
                //strQry += " WHERE DF_ID=WO_DF_ID AND DF_REPLACE_FLAG=1 AND DF_DTC_CODE='" + DtcCode + "'";
                //dtCRDetails = objcon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getcrdetails");
                cmd.Parameters.AddWithValue("dtccode", DtcCode);
                dtCRDetails = objcon.FetchDataTable(cmd);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCRDetails;
            }
            return dtCRDetails;
        }

        //public DataTable GetNewDTCDetails(string sDTCId)
        //{
        //    NpgsqlCommand = new NpgsqlCommand();
        //    DataTable dtNewDTCDetails = new DataTable();
        //    string sStrQry = string.Empty;
        //    try
        //    {
        //        sStrQry = "SELECT \"DT_CODE\", \"DT_TC_ID\" , TO_CHAR(\"WO_DATE\",'dd-mm-yyyy')\"WO_DATE\" , \"WO_NO\" , UPPER(\"DT_NAME\")\"DT_NAME\",\"WO_REF_OFFCODE\",\"WO_SLNO\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCINVOICE\" ";
        //        sStrQry += " ,\"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCMAST\" WHERE \"DT_ID\"=\"WO_RECORD_ID\" AND \"WO_DATA_ID\"=CAST(\"IN_NO\" AS TEXT) ";
        //        sStrQry += " AND \"WO_RECORD_ID\" =:sDTCId AND \"TI_ID\"=\"IN_TI_NO\" AND \"TI_WO_SLNO\"=\"WO_SLNO\" AND \"WO_BO_ID\"='69'";
        //        NpgsqlCommand.Parameters.AddWithValue("sDTCId", Convert.ToDouble(sDTCId));
        //        return objcon.FetchDataTable(sStrQry, NpgsqlCommand);
        //    }
        //    catch(Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return dtNewDTCDetails;
        //    }
        //}
        public DataTable GetNewDTCDetails(string sDTCId)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtNewDTCDetails = new DataTable();
            string sStrQry = string.Empty;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_getnewdtcdetails");
                cmd.Parameters.AddWithValue("sdtcid", sDTCId);
                return objcon.FetchDataTable(cmd);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtNewDTCDetails;
            }
        }

        public string CheckRIandInvDone(string stcreplaceID)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string tcreplaceID = string.Empty;
            string sStrQry = string.Empty;
            try
            {

                sStrQry += " SELECT \"TR_IN_NO\"  FROM \"TBLTCREPLACE\" WHERE \"TR_ID\" = '" + stcreplaceID + "' ";
                tcreplaceID = objcon.get_value(sStrQry, NpgsqlCommand);
                return tcreplaceID;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return tcreplaceID;
            }
        }

    }
}
