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
    public class clsFormValues
    {
        string strFormCode = "clsFormValues";
        public string sFailureId { get; set; }
        public string sWorkOrderId { get; set; }
        public string sWOTTKstatus { get; set; }
        public string sIndentId { get; set; }
        public string sInvoiceId { get; set; }
        public string sDecommisionId { get; set; }

        public string sWFInitialId { get; set; }
        public string sTaskType { get; set; }
        public string sTCcode { get; set; }
        public string sFailType { get; set; }

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        public string GetDTCId(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string dtid = string.Empty;
                if (objForm.sFailureId!=null)
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT (SELECT \"DT_ID\" FROM \"TBLDTCMAST\" WHERE \"DT_CODE\"=\"DF_DTC_CODE\") DT_ID FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"=:sFailureId";
                    NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(objForm.sFailureId));
                    dtid = objcon.get_value(strQry, NpgsqlCommand);
                }
                return dtid;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        #region WorkOrder

        public string GetStatusFlagForWO(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') FROM \"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND \"EST_ID\"=:sFailureId";
                NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(objForm.sFailureId));
                //strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ='" + objForm.sFailureId + "'";
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetFailType(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"EST_FAIL_TYPE\" || '~' || \"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND \"EST_ID\"=:sFailureId";
                NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(objForm.sFailureId));
                //strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ='" + objForm.sFailureId + "'";
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        //public clsFormValues GetStatusFlagForWOFromWF(clsFormValues objForm)
        //{
        //    try
        //    {
        //        string strQry = string.Empty;
        //        DataTable dt = new DataTable();
        //        #region old code when work order had only 1 level
        //        //if (objForm.sFailureId.Contains("-"))
        //        //{
        //        //    strQry = "SELECT \"DF_STATUS_FLAG\",\"EST_ID\" AS WO_RECORD_ID,\"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
        //        //    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" ='" + objForm.sWFInitialId + "')= \"WO_ID\" ";
        //        //    strQry += " AND \"DF_ID\"=\"EST_FAILUREID\" AND  CAST(\"DF_ID\" AS TEXT)=\"WO_DATA_ID\"";
        //        //}
        //        //else
        //        //{
        //        //    strQry = "SELECT \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\" WHERE ";
        //        //    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" ='" + objForm.sWFInitialId + "')= \"WO_ID\" ";
        //        //    strQry += " AND  \"DF_ID\"=\"WO_RECORD_ID\"";
        //        //}
        //        #endregion

        //        if (objForm.sFailureId.Contains("-"))
        //        {
        //            strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
        //            strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
        //            strQry += " AND cast(\"DF_ID\" as text)=cast(\"WO_DATA_ID\"as text) AND cast(\"EST_ID\" as text)=cast(\"WO_RECORD_ID\"as text)";

        //            //else
        //            //{
        //            //    strQry = "SELECT \"DF_STATUS_FLAG\",\"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\" WHERE \"DF_ID\"=\"WO_DF_ID\" and \"WO_SLNO\"='"+ objForm.sFailureId + "'";
        //            //}
        //            NpgsqlCommand = new NpgsqlCommand();
        //            NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
        //            dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

        //            if (dt.Rows.Count > 0)
        //            {
        //                objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
        //                objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
        //                if (dt.Columns.Contains("EST_FAIL_TYPE"))
        //                {
        //                    objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
        //                }
        //                else
        //                {
        //                    strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\",\"EST_ID\" AS WO_RECORD_ID,\"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
        //                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
        //                    strQry += " AND \"DF_ID\"=\"EST_FAILUREID\" AND  CAST(\"DF_ID\" AS TEXT)=\"WO_DATA_ID\"";
        //                    NpgsqlCommand = new NpgsqlCommand();
        //                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
        //                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
        //                        objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
        //                        objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
        //                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
        //                NpgsqlCommand = new NpgsqlCommand();
        //                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
        //                objForm.sFailureId = objcon.get_value(strQry, NpgsqlCommand);
        //                objForm.sTaskType = "3";
        //            }
        //        }

        //        else /*(dt.Rows.Count == 0)*/
        //        {
        //            strQry = "SELECT \"EST_FAIL_TYPE\",\"EST_ID\",\"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLESTIMATIONDETAILS\",";
        //            strQry += " \"TBLWO_OBJECT_AUTO\",\"TBLDTCFAILURE\" WHERE \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"EST_FAILUREID\" AND ";
        //            strQry += " \"WOA_INITIAL_ACTION_ID\" =\"WO_ID\" AND \"WO_DF_ID\"=\"EST_FAILUREID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"WO_ID\"=:sWFInitialId";
        //            NpgsqlCommand = new NpgsqlCommand();
        //            NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
        //            dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

        //            if (dt.Rows.Count > 0)
        //            {
        //                objForm.sFailureId = Convert.ToString(dt.Rows[0]["EST_ID"]);
        //                objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
        //                objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
        //            }
        //        }



        //        return objForm;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return objForm;
        //    }
        //}
        #endregion
        public clsFormValues GetStatusFlagForWOFromWF(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                #region old code when work order had only 1 level
                //if (objForm.sFailureId.Contains("-"))
                //{
                //    strQry = "SELECT \"DF_STATUS_FLAG\",\"EST_ID\" AS WO_RECORD_ID,\"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
                //    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" ='" + objForm.sWFInitialId + "')= \"WO_ID\" ";
                //    strQry += " AND \"DF_ID\"=\"EST_FAILUREID\" AND  CAST(\"DF_ID\" AS TEXT)=\"WO_DATA_ID\"";
                //}
                //else
                //{
                //    strQry = "SELECT \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\" WHERE ";
                //    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" ='" + objForm.sWFInitialId + "')= \"WO_ID\" ";
                //    strQry += " AND  \"DF_ID\"=\"WO_RECORD_ID\"";
                //}
                #endregion

                if (objForm.sFailureId.Contains("-"))
                {
                    strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" ";
                    strQry += " FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                    strQry += " AND cast(\"DF_ID\" as text)=cast(\"WO_DATA_ID\"as text) AND cast(\"EST_ID\" as text)=cast(\"WO_RECORD_ID\"as text)";
                    
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

                    if (dt.Rows.Count > 0)
                    {
                        objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                        objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
                        if (dt.Columns.Contains("EST_FAIL_TYPE"))
                        {
                            objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
                        }
                        else
                        {
                            strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\",\"EST_ID\" ";
                            strQry += " AS WO_RECORD_ID,\"EST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE ";
                            strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                            strQry += " AND \"DF_ID\"=\"EST_FAILUREID\" AND  CAST(\"DF_ID\" AS TEXT)=\"WO_DATA_ID\"";
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                            dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                            if (dt.Rows.Count > 0)
                            {
                                objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                                objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
                                objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
                            }
                        }

                    }
                    else
                    {
                        strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                        strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                        objForm.sFailureId = objcon.get_value(strQry, NpgsqlCommand);
                        objForm.sTaskType = "3";
                    }
                }

                else /*(dt.Rows.Count == 0)*/
                {
                    strQry = "SELECT \"EST_FAIL_TYPE\",\"EST_ID\",\"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') ";
                    strQry += " AS \"DF_STATUS_FLAG\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLESTIMATIONDETAILS\",";
                    strQry += " \"TBLWO_OBJECT_AUTO\",\"TBLDTCFAILURE\" WHERE \"WO_RECORD_ID\"=\"WO_SLNO\" AND \"WO_DF_ID\"=\"EST_FAILUREID\" AND ";
                    strQry += " \"WOA_INITIAL_ACTION_ID\" =\"WO_ID\" AND \"WO_DF_ID\"=\"EST_FAILUREID\" AND \"DF_ID\"=\"WO_DF_ID\" AND \"WO_ID\"=:sWFInitialId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

                    if (dt.Rows.Count > 0)
                    {
                        objForm.sFailureId = Convert.ToString(dt.Rows[0]["EST_ID"]);
                        objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
                        objForm.sFailType = Convert.ToString(dt.Rows[0]["EST_FAIL_TYPE"]);
                    }
                    else
                    {
                        strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                        strQry += "  \"WO_ID\"=:sWFInitialId ";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                        objForm.sWorkOrderId = objcon.get_value(strQry, NpgsqlCommand);
                        objForm.sTaskType = "3";
                    }
                }



                return objForm;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }
        #region Indent


        public string GetStatusFlagForIndent(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;

                strQry = "SELECT \"DF_STATUS_FLAG\"|| '~' ||COALESCE(\"EST_GUARANTEETYPE\",'') AS \"DF_STATUS_FLAG\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLESTIMATIONDETAILS\" WHERE \"DF_ID\"=\"WO_DF_ID\" ";
                strQry += " AND \"DF_ID\"=\"EST_FAILUREID\" AND \"WO_SLNO\" =:sWorkOrderId ";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWorkOrderId", Convert.ToInt32(objForm.sWorkOrderId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult == "")
                {
                    strQry = "SELECT \"WO_NO\" FROM \"TBLWORKORDER\" WHERE  \"WO_DF_ID\" IS NULL AND \"WO_SLNO\" =:sWorkOrderId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWorkOrderId", Convert.ToInt32(objForm.sWorkOrderId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                    if (sResult != "")
                    {
                        sResult = "3";
                    }
                }
                else
                {
                    return sResult;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }


        }

        public clsFormValues GetStatusFlagForIndentFromWF(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();


                strQry = "SELECT \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLWORKORDER\" WHERE ";
                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId limit 1)= \"WO_ID\"";
                strQry += " AND  \"WO_SLNO\"=\"WO_RECORD_ID\" AND \"DF_ID\"=\"WO_DF_ID\"";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objForm.sWorkOrderId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
                }
                else
                {

                    strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\" WHERE ";
                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId limit 1)= \"WO_ID\" ";
                    strQry += " AND  \"WO_SLNO\"=\"WO_RECORD_ID\" AND \"WO_DF_ID\" IS NULL";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    objForm.sWorkOrderId = objcon.get_value(strQry, NpgsqlCommand);

                    objForm.sTaskType = "3";

                }
                return objForm;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }


        }

        #endregion


        #region Invoice

        public string GetStatusFlagForInvoiceFromIndent(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;

                strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLINDENT\" WHERE \"DF_ID\"=\"WO_DF_ID\"  ";
                strQry += " AND \"WO_SLNO\"=\"TI_WO_SLNO\"  AND \"TI_ID\" =:sIndentId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sIndentId", Convert.ToInt32(objForm.sIndentId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult == "")
                {
                    strQry = "SELECT \"WO_NO\" FROM \"TBLWORKORDER\",\"TBLINDENT\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"WO_DF_ID\" IS NULL ";
                    strQry += " AND \"TI_ID\" =:sIndentId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sIndentId", Convert.ToInt32(objForm.sIndentId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                    if (sResult != "")
                    {
                        sResult = "3";
                    }
                }
                else
                {
                    return sResult;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public clsFormValues GetStatusFlagForInvoiceFromWF(clsFormValues objForm)
        {
            try
            {

                string strQry = string.Empty;
                DataTable dt = new DataTable();

                strQry = "SELECT \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLWORKORDER\",\"TBLINDENT\" WHERE ";
                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                strQry += " AND  \"TI_ID\"=\"WO_RECORD_ID\" AND \"DF_ID\" = \"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" ";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objForm.sIndentId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                    objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);
                }
                else
                {

                    strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLINDENT\" WHERE ";
                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\"";
                    strQry += " AND  \"TI_ID\"=\"WO_RECORD_ID\" AND \"WO_DF_ID\" IS NULL AND \"WO_SLNO\"=\"TI_WO_SLNO\" ";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    objForm.sIndentId = objcon.get_value(strQry, NpgsqlCommand);

                    objForm.sTaskType = "3";

                }
                return objForm;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }

        #endregion

        #region Decommission
        //public string GetStatusFlagForDecommissionFromInvoice(clsFormValues objForm)
        //{
        //    try
        //    {
        //        string strQry = string.Empty;
        //        string sResult = string.Empty;


        //        strQry = "SELECT \"DF_STATUS_FLAG\" from \"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"WO_DF_ID\" and \"WO_SLNO\"=:sWorkOrderId";
        //       // strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\"  ";
        //       // strQry += " AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\"  AND \"IN_NO\" =:sInvoiceId";
        //        NpgsqlCommand = new NpgsqlCommand();
        //        NpgsqlCommand.Parameters.AddWithValue("sWorkOrderId", Convert.ToInt32(objForm.sWorkOrderId));
        //        sResult = objcon.get_value(strQry, NpgsqlCommand);
        //        if (sResult == "")
        //        {
        //            //string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);
        //            strQry = "SELECT \"WO_NO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"WO_DF_ID\" IS NULL ";
        //            strQry += " AND \"IN_NO\" =:sInvoiceId AND \"TI_ID\"=\"IN_TI_NO\" ";
        //            NpgsqlCommand = new NpgsqlCommand();
        //            NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
        //            sResult = objcon.get_value(strQry, NpgsqlCommand);
        //            if (sResult != "")
        //            {
        //                sResult = "3";
        //            }
        //        }
        //        else
        //        {
        //            return sResult;
        //        }

        //        return sResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return ex.Message;
        //    }
        //}
        public string GetStatusFlagForDecommissionFromInvoice(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;


                strQry = "SELECT \"DF_STATUS_FLAG\" from \"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"WO_DF_ID\" and \"WO_SLNO\"=:sWorkOrderId";
                // strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\"  ";
                // strQry += " AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\"  AND \"IN_NO\" =:sInvoiceId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWorkOrderId", Convert.ToInt32(objForm.sWorkOrderId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult == "")
                {
                    //string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);
                    strQry = "SELECT \"WO_NO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"WO_DF_ID\" IS NULL ";
                    strQry += " AND \"IN_NO\" =:sInvoiceId AND \"TI_ID\"=\"IN_TI_NO\" ";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                    if (sResult != "")
                    {
                        sResult = "3";
                    }
                    else
                    {
                        strQry = "SELECT \"WO_TTK_STATUS\" FROM \"TBLWORKORDER\" WHERE \"WO_SLNO\"=:sInvoiceId";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
                        sResult = objcon.get_value(strQry, NpgsqlCommand);
                        if (sResult == "1")
                        {
                            objForm.sWOTTKstatus = sResult;
                            sResult = "3";
                        }
                    }
                }
                else
                {
                    return sResult;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public clsFormValues GetStatusFlagForDecommFromWF(clsFormValues objForm)
        {
            try
            {

                string strQry = string.Empty;
                DataTable dt = new DataTable();

                strQry = "SELECT \"DF_STATUS_FLAG\" ,\"DF_ID\" from \"TBLDTCFAILURE\" WHERE  cast (\"DF_ID\" as varchar)in   (SELECT \"WO_DATA_ID\"  FROM \"TBLWORKFLOWOBJECTS\" , \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_PREV_APPROVE_ID\" = \"WO_ID\" and \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId limit 1) ";
               // strQry = "SELECT \"DF_STATUS_FLAG\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLDTCFAILURE\",\"TBLTCDRAWN\" WHERE ";
               // strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\"";
                //strQry += " AND  \"TD_INV_NO\"=\"WO_RECORD_ID\" AND \"DF_ID\"=\"TD_DF_ID\"";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    objForm.sInvoiceId = Convert.ToString(dt.Rows[0]["DF_ID"]);
                    objForm.sTaskType = Convert.ToString(dt.Rows[0]["DF_STATUS_FLAG"]);


                }
                else
                {

                    strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE ";
                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId limit 1)= \"WO_ID\"";
                    strQry += " AND  \"IN_NO\"=\"WO_RECORD_ID\" AND \"WO_DF_ID\" IS NULL AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" ";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    objForm.sInvoiceId = objcon.get_value(strQry, NpgsqlCommand);

                    objForm.sTaskType = "3";

                }
                return objForm;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }


        public string GetStatusFlagForDecommission(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;


                string woslno = objcon.get_value("SELECT \"TR_WO_SLNO\" from \"TBLTCREPLACE\" WHERE cast(\"TR_ID\" as text) ='"+objForm.sDecommisionId + "'");

                string failid = objcon.get_value("SELECT \"WO_DF_ID\" from \"TBLWORKORDER\" WHERE cast(\"WO_SLNO\" as text) = '"+ woslno + "'");
                strQry = "SELECT DISTINCT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE cast(\"DF_ID\" as text)='"+ failid + "'  ";
               // strQry += " AND \"TR_ID\" =:sDecommisionId";
                //NpgsqlCommand = new NpgsqlCommand();
               // NpgsqlCommand.Parameters.AddWithValue("sDecommisionId", Convert.ToInt32(objForm.sDecommisionId));
                sResult = objcon.get_value(strQry);
                if (sResult == "")
                {
                    string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);
                    strQry = "SELECT \"WO_NO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"WO_DF_ID\" IS NULL ";
                    strQry += " AND \"IN_NO\" =:sInvoiceId AND  \"TI_ID\"=\"IN_TI_NO\" ";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(sInvoiceId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                    if (sResult != "")
                    {
                        sResult = "3";
                    }
                }
                else
                {
                    return sResult;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        //public clsFormValues GetWOnoForDTCCommission(clsFormValues objForm)
        //{
        //    try
        //    {
        //        string strQry = string.Empty;
        //        DataTable dt = new DataTable();

        //        //string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);

        //        strQry = "SELECT \"TD_TC_NO\",\"WO_SLNO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\",\"TBLTCDRAWN\" WHERE ";
        //        strQry += " \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND \"IN_NO\"=\"TD_INV_NO\" AND \"IN_NO\" =:sInvoiceId";
        //        NpgsqlCommand = new NpgsqlCommand();
        //        NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
        //        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
        //        if (dt.Rows.Count > 0)
        //        {
        //            objForm.sWorkOrderId = Convert.ToString(dt.Rows[0]["WO_SLNO"]);
        //            objForm.sTCcode = Convert.ToString(dt.Rows[0]["TD_TC_NO"]);
        //        }
        //        return objForm;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return objForm;
        //    }
        //}
        public clsFormValues GetWOnoForDTCCommission(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                //string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);
                if (objForm.sInvoiceId!="")
                {
                    if (objForm.sWOTTKstatus != "1")
                    {
                        strQry = "SELECT \"TD_TC_NO\",\"WO_SLNO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCINVOICE\",\"TBLTCDRAWN\" WHERE ";
                        strQry += " \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND \"IN_NO\"=\"TD_INV_NO\" AND \"IN_NO\" =:sInvoiceId";
                        NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
                        NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt64(objForm.sInvoiceId));
                        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objForm.sWorkOrderId = Convert.ToString(dt.Rows[0]["WO_SLNO"]);
                            objForm.sTCcode = Convert.ToString(dt.Rows[0]["TD_TC_NO"]);
                        }
                    }
                    else
                    {
                        strQry = "SELECT \"WO_SLNO\" FROM \"TBLWORKORDER\" WHERE ";
                        strQry += " \"WO_SLNO\"=:sInvoiceId";
                        NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
                        NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt64(objForm.sInvoiceId));
                        dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                        if (dt.Rows.Count > 0)
                        {
                            objForm.sWorkOrderId = Convert.ToString(dt.Rows[0]["WO_SLNO"]);

                        }
                    }
                }
                return objForm;
            }
            catch (Exception ex)
            {
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                
                return objForm;
            }
        }
        public string GetDTCIdFromWO(string sWOSlno)
        {
            try
            {
                string strQry = string.Empty;
                string dtid = string.Empty;
                if (sWOSlno!="")
                {
                    strQry = "SELECT \"DT_ID\" FROM \"TBLDTCMAST\" WHERE \"DT_WO_ID\" =:sWOSlno";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWOSlno", Convert.ToInt32(sWOSlno));
                    dtid= objcon.get_value(strQry, NpgsqlCommand);
                }
                return dtid;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        #endregion


        public string GetInvoiceId(string sDecommId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"IN_NO\" FROM \"TBLTCREPLACE\",\"TBLDTCINVOICE\" WHERE \"TR_IN_NO\"=\"IN_NO\" AND \"TR_ID\" =:sDecommId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDecommId", Convert.ToInt32(sDecommId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetWorkOrderId(string sIndentId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TI_WO_SLNO\" FROM \"TBLINDENT\" WHERE \"TI_ID\" =:sIndentId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sIndentId", Convert.ToInt32(sIndentId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetFailureIdFromWO(string sWOId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"WO_DF_ID\" FROM \"TBLWORKORDER\" WHERE \"WO_SLNO\" =:sWOId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWOId", Convert.ToInt32(sWOId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetIndentId(string sInvoiceId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"IN_TI_NO\" FROM \"TBLDTCINVOICE\" WHERE  \"IN_NO\" =:sInvoiceId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(sInvoiceId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        public string GetFailureIdFromInvoice(string sWorkOrderId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"DF_ID\" from \"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"WO_DF_ID\" and \"WO_SLNO\"=:sWorkOrderId";
               // strQry = "SELECT \"DF_ID\" FROM \"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLINDENT\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\"  ";
               // strQry += " AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND \"IN_NO\" =:sInvoiceId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWorkOrderId", Convert.ToInt32(sWorkOrderId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetFailureIdFromDecommId(string sDecomm)
        {
            try
            {
                string strQry = string.Empty;
                string woslno = string.Empty;
                if (sDecomm!="")
                {
                     woslno = objcon.get_value("SELECT \"TR_WO_SLNO\" from \"TBLTCREPLACE\" WHERE cast(\"TR_ID\" as text) ='" + sDecomm + "'");
                }
                if (woslno!="")
                {
                    strQry = "SELECT \"WO_DF_ID\" from \"TBLWORKORDER\" WHERE \"WO_SLNO\" = '" + woslno + "'";
                }
                // strQry = "SELECT DISTINCT \"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLTCDRAWN\",\"TBLTCREPLACE\" WHERE \"DF_ID\"=\"TD_DF_ID\" AND ";
                // strQry += " \"TD_INV_NO\"=\"TR_IN_NO\" AND \"TR_ID\" =:sDecomm";
                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sDecomm", Convert.ToInt32(sDecomm));

                return objcon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        #region Store Invoice

        public string GetStoreIndentIdFromWF(string sWFInitialId, string sWFObjectId)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;
                strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\"";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(sWFInitialId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                if (sResult == "")
                {
                    strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt32(sWFObjectId));
                    sResult = objcon.get_value(strQry, NpgsqlCommand);
                }
                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetStoreInvoiceIdFromWF(string sWFObjectId)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;

                strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" =:sWFObjectId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFObjectId", Convert.ToInt32(sWFObjectId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }
        #endregion

        public clsFormValues GetStatusFlagForWOFromWFperdecomm(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();


                if (objForm.sFailureId.Contains("-"))
                {
                    strQry = "SELECT \"PEST_GUARANTEETYPE\" AS \"PEST_GUARANTEETYPE\",\"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE ";
                    strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                    strQry += "  AND cast(\"PEST_ID\" as text)=cast(\"WO_RECORD_ID\"as text)";

                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

                    if (dt.Rows.Count > 0)
                    {
                        objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                        objForm.sTaskType = Convert.ToString(dt.Rows[0]["PEST_GUARANTEETYPE"]);
                        if (dt.Columns.Contains("PEST_FAIL_TYPE"))
                        {
                            objForm.sFailType = Convert.ToString(dt.Rows[0]["PEST_FAIL_TYPE"]);
                        }
                        else
                        {
                            strQry = "SELECT \"PEST_GUARANTEETYPE\" AS \"PEST_GUARANTEETYPE\",\"PEST_ID\" AS WO_RECORD_ID,\"PEST_FAIL_TYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE ";
                            strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                            strQry += " AND   CAST(\"PEST_ID\" AS TEXT)=cast(\"WO_RECORD_ID\"as text)";
                            NpgsqlCommand = new NpgsqlCommand();
                            NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                            dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
                            if (dt.Rows.Count > 0)
                            {
                                objForm.sFailureId = Convert.ToString(dt.Rows[0]["WO_RECORD_ID"]);
                                objForm.sTaskType = Convert.ToString(dt.Rows[0]["PEST_GUARANTEETYPE"]);
                                objForm.sFailType = Convert.ToString(dt.Rows[0]["PEST_FAIL_TYPE"]);
                            }
                        }

                    }
                    else
                    {
                        strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE ";
                        strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\" ";
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                        objForm.sFailureId = objcon.get_value(strQry, NpgsqlCommand);
                        objForm.sTaskType = "3";
                    }
                }

                else /*(dt.Rows.Count == 0)*/
                {
                    strQry = "SELECT \"PEST_FAIL_TYPE\",\"PEST_ID\",\"PEST_GUARANTEETYPE\" AS \"PEST_GUARANTEETYPE\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTWORKORDER\",\"TBLPERMANENTESTIMATIONDETAILS\",";
                    strQry += " \"TBLWO_OBJECT_AUTO\" WHERE \"WO_RECORD_ID\"=\"PWO_SLNO\" AND \"PWO_PEF_ID\"=\"PEST_ID\" AND ";
                    strQry += " \"WOA_INITIAL_ACTION_ID\" =\"WO_ID\"  AND \"WO_ID\"=:sWFInitialId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                    dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

                    if (dt.Rows.Count > 0)
                    {
                        objForm.sFailureId = Convert.ToString(dt.Rows[0]["PEST_ID"]);
                        objForm.sTaskType = Convert.ToString(dt.Rows[0]["PEST_GUARANTEETYPE"]);
                        objForm.sFailType = Convert.ToString(dt.Rows[0]["PEST_FAIL_TYPE"]);
                    }
                }



                return objForm;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }

        public string GetEstIdFromDecommId(string sDecomm)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT DISTINCT \"PEST_ID\" FROM \"TBLPERMANENTESTIMATIONDETAILS\",\"TBLPERMANENTTCREPLACE\",\"TBLPERMANENTWORKORDER\" WHERE \"PEST_ID\"=\"PWO_PEF_ID\"";
                strQry += " AND \"PTR_WO_SLNO\" = \"PWO_SLNO\" AND \"PTR_ID\" =:sDecomm";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDecomm", Convert.ToInt32(sDecomm));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        public string GetEstIdFromIndent(string sInvoiceId)
        {
            try
            {
                string strQry = string.Empty;
                if (sInvoiceId!="")
                {
                    strQry = "SELECT \"PEST_ID\" FROM \"TBLPERMANENTWORKORDER\",\"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"PEST_ID\"=\"PWO_PEF_ID\" AND \"PWO_SLNO\"=:sInvoiceId";
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(sInvoiceId));

                    return objcon.get_value(strQry, NpgsqlCommand);
                }
                return "";
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        public string Getwoid(string sIndentId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"PTI_WO_SLNO\" FROM \"TBLPERMANENTINDENT\" WHERE  \"PTI_ID\" =:sIndentId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sIndentId", Convert.ToInt32(sIndentId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        public string GetIndentIdbyestimateid(string sEstId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"PTI_ID\" from \"TBLPERMANENTINDENT\" INNER join \"TBLPERMANENTWORKORDER\" on \"PTI_WO_SLNO\"=\"PWO_SLNO\" INNER JOIN \"TBLPERMANENTESTIMATIONDETAILS\" on \"PEST_ID\"=\"PWO_PEF_ID\" WHERE \"PEST_ID\"=:sEstId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sEstId", Convert.ToInt32(sEstId));
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetStatusFlagForWOperdecomm(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"PEST_GUARANTEETYPE\" FROM \"TBLPERMANENTESTIMATIONDETAILS\" WHERE \"PEST_ID\"=:sFailureId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(objForm.sFailureId));
                //strQry = "SELECT \"DF_STATUS_FLAG\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ='" + objForm.sFailureId + "'";
                return objcon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }

        public string GetStatusFlagForDecommissionFromInvoiceper(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;

                //string sInvoiceId = GetInvoiceId(objForm.sDecommisionId);
                strQry = "SELECT \"PWO_SLNO\" FROM \"TBLPERMANENTWORKORDER\" WHERE \"PWO_SLNO\"=:sInvoiceId ";

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(objForm.sInvoiceId));
                sResult = objcon.get_value(strQry, NpgsqlCommand);

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }


        public clsFormValues GetStatusFlagForDecommFromWFper(clsFormValues objForm)
        {
            try
            {

                string strQry = string.Empty;
                DataTable dt = new DataTable();



                strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTWORKORDER\" WHERE ";
                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId)= \"WO_ID\"";
                strQry += " AND  \"PWO_SLNO\"=\"WO_RECORD_ID\"";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                objForm.sIndentId = objcon.get_value(strQry, NpgsqlCommand);

                objForm.sTaskType = "3";


                return objForm;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }



        public string GetStatusFlagForDecommissionper(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                string sResult = string.Empty;


                string sIndentid = getIndentid(objForm.sDecommisionId);
                strQry = "SELECT \"PWO_NO\" FROM \"TBLPERMANENTWORKORDER\" WHERE \"PWO_SLNO\"=:sIndentid";

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sIndentid", Convert.ToInt32(sIndentid));
                sResult = objcon.get_value(strQry, NpgsqlCommand);
                sResult = "3";


                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return ex.Message;
            }
        }



        public clsFormValues GetStatusFlagForIndentFromWFper(clsFormValues objForm)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();




                strQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\",\"TBLPERMANENTWORKORDER\" WHERE ";
                strQry += " (SELECT \"WOA_PREV_APPROVE_ID\" FROM \"TBLWO_OBJECT_AUTO\" WHERE \"WOA_INITIAL_ACTION_ID\" =:sWFInitialId limit 1)= \"WO_ID\" ";
                strQry += " AND  \"PWO_SLNO\"=\"WO_RECORD_ID\" ";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sWFInitialId", Convert.ToInt32(objForm.sWFInitialId));
                objForm.sWorkOrderId = objcon.get_value(strQry, NpgsqlCommand);

                objForm.sTaskType = "3";


                return objForm;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objForm;
            }
        }

        public string getIndentid(string sDecommId)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"PWO_SLNO\" FROM \"TBLPERMANENTTCREPLACE\",\"TBLPERMANENTWORKORDER\" WHERE \"PTR_WO_SLNO\"=\"PWO_SLNO\" AND \"PTR_ID\" =:sDecommId";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDecommId", Convert.ToInt32(sDecommId));
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
