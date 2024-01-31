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

namespace IIITS.DTLMS.BL
{
    public class clsInvoice
    {

        string strFormCode = "clsInvoice";

        public string sWarrentyPeriod { get; set; }
        public string sDtrWarrentyTime { get; set; }
        public string sInvoiceSlNo { get; set; }
        public string sStoreId { get; set; }
        public string sDtcFailId { get; set; }
        public string sTcSlNo { get; set; }
        public string sInvoiceNo { get; set; }
        public string sInvoiceDate { get; set; }
        public string sInvoiceDescription { get; set; }
        public string sFailDate { get; set; }
        public string sAmount { get; set; }

        public string sCreatedBy { get; set; }
        public string sTcMake { get; set; }
        public string sTcCapacity { get; set; }
        public string sWOSlno { get; set; }
        public string sTcNewCapacity { get; set; }
        public string sIndentId { get; set; }
        public string sIndentNo { get; set; }
        public string sIndentCrby { get; set; }
        public string sIndentDate { get; set; }
        public string sTcCode { get; set; }
        public string sDTCName { get; set; }
        public string sOldTcSlno { get; set; }
        public string sOldTcCode { get; set; }
        public string sTCId { get; set; }
        public string sDTCId { get; set; }
        public string sDTCCODE { get; set; }

        public string sManualInvoiceNo { get; set; }

        public string sTaskType { get; set; }
        public string sOfficeCode { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFOId { get; set; }
        public string sWFDataId { get; set; }
        public string sWFAutoId { get; set; }

        //Gate Pass
        public string sVehicleNumber { get; set; }
        public string sReceiptientName { get; set; }
        public string sChallenNo { get; set; }
        public string sGatePassId { get; set; }
        public string sDTCCode { get; set; }
        public string sIssueQty { get; set; }
        public string sStoreType { get; set; }
        public string smanufactureDate { get; set; }
        public string sTcRating { get; set; }

        //CustOledbConnection objCon = new CustOledbConnection(Constants.Password);
        PGSqlConnection objCon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        public string[] SaveUpdateInvoiceDetails(clsInvoice objInvoice)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            try
            {

                string[] strResult = new string[2];
                string[] strArray = new string[2];
                //NpgsqlCommand cmd = new NpgsqlCommand("sp_validate_invoice");
                //cmd.Parameters.AddWithValue("sindentno", objInvoice.sIndentNo.ToUpper());
                //cmd.Parameters.AddWithValue("stccode", objInvoice.sTcCode);
                //cmd.Parameters.AddWithValue("sdtccode", objInvoice.sDTCCODE);
                //cmd.Parameters.AddWithValue("soldtccode", objInvoice.sOldTcCode );
                //cmd.Parameters.AddWithValue("stcnewcapacity", objInvoice.sTcNewCapacity);
                //cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                //cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                //cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                //cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                //strArray[0] = "op_id";
                //strArray[1] = "msg";
                //strResult = objCon.Execute(cmd, strArray, 2);
                //if(strResult[0]=="2")
                //{
                //    Arr[0] = strResult[1];
                //    Arr[1] = strResult[0];
                //    return Arr;
                //}


                //Check Work Order no exists or not
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sIndentNo", objInvoice.sIndentNo.ToUpper());
                string sInNo = objCon.get_value("SELECT \"TI_INDENT_NO\" FROM \"TBLINDENT\" WHERE UPPER(\"TI_INDENT_NO\")=:sIndentNo", NpgsqlCommand);
                if (sInNo.Length == 0)
                {
                    Arr[0] = "Enter Valid Indent No";
                    Arr[1] = "2";
                    return Arr;
                }

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode", Convert.ToDouble(objInvoice.sTcCode));
                string sTccode = objCon.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:sTcCode ", NpgsqlCommand);
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Enter Valid DTr Code";
                    Arr[1] = "2";
                    return Arr;
                }

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode1", Convert.ToDouble(objInvoice.sTcCode));
                sTccode = objCon.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:sTcCode1 AND \"TC_STATUS\" IN ('1','2','5') AND \"TC_CURRENT_LOCATION\" in (1,5)", NpgsqlCommand);
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Entered DTr Code not in Store or Not in good condition";
                    Arr[1] = "2";
                    return Arr;
                }

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sTcCode2", Convert.ToDouble(objInvoice.sTcCode));
                NpgsqlCommand.Parameters.AddWithValue("sTcNewCapacity", Convert.ToDouble(objInvoice.sTcNewCapacity));
                sTccode = objCon.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:sTcCode2 AND \"TC_CAPACITY\" =:sTcNewCapacity", NpgsqlCommand);
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Entered DTr Code Capacity not Matching with Requested Capacity";
                    Arr[1] = "2";
                    return Arr;
                }

                //NpgsqlCommand = new NpgsqlCommand();
                //NpgsqlCommand.Parameters.AddWithValue("sDTCCODE", objInvoice.sDTCCode);
                //NpgsqlCommand.Parameters.AddWithValue("sOldTcCode", objInvoice.sOldTcCode);
                //sInNo = objCon.get_value("SELECT \"TI_INDENT_NO\" FROM \"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND CAST(\"DF_DTC_CODE\" AS TEXT) =:sDTCCODE AND CAST(\"DF_EQUIPMENT_ID\" AS TEXT)=:sOldTcCode", NpgsqlCommand);
                //if (sInNo.Length > 0)
                //{
                //    Arr[0] = "Invoice Already done for this DTC";
                //    Arr[1] = "2";
                //    return Arr;
                //}

                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sDTCCODE", objInvoice.sDTCCode);
                NpgsqlCommand.Parameters.AddWithValue("sOldTcCode", objInvoice.sOldTcCode);
                NpgsqlCommand.Parameters.AddWithValue("sIndentId", objInvoice.sIndentId);
                sInNo = objCon.get_value("SELECT \"TI_INDENT_NO\" FROM \"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND CAST(\"DF_DTC_CODE\" AS TEXT) =:sDTCCODE AND CAST(\"DF_EQUIPMENT_ID\" AS TEXT)=:sOldTcCode  AND CAST(\"TI_ID\" AS TEXT)=:sIndentId", NpgsqlCommand);
                if (sInNo.Length > 0)
                {
                    Arr[0] = "Invoice Already done for this DTC";
                    Arr[1] = "2";
                    return Arr;
                }


                if (objInvoice.sInvoiceSlNo == "")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sInvoiceNo", objInvoice.sInvoiceNo.ToUpper());

                    //dr = objCon.Fetch("select * from TBLDTCINVOICE where  UPPER(IN_INV_NO)='" + objInvoice.sInvoiceNo.ToUpper() + "' ");
                    string sId = objCon.get_value("SELECT \"IN_NO\" FROM \"TBLDTCINVOICE\" WHERE  UPPER(\"IN_INV_NO\")=:sInvoiceNo ", NpgsqlCommand);
                    if (sId.Length > 0)
                    {
                        Arr[0] = "Invoice No. Already Exists";
                        Arr[1] = "2";
                        return Arr;
                    }
                    //if (dr.Read())
                    //{

                    //    Arr[0] = "Invoice No. Already Exists";
                    //    Arr[1] = "2";
                    //    return Arr;
                    //}
                    //dr.Close();

                    objCon.BeginTransaction();

                    NpgsqlCommand cmd = new NpgsqlCommand("sp_save_invoice");
                    cmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo.ToUpper());
                    cmd.Parameters.AddWithValue("sinvoicedate", objInvoice.sInvoiceDate);
                    cmd.Parameters.AddWithValue("sinvoicedescription", objInvoice.sInvoiceDescription);
                    cmd.Parameters.AddWithValue("samount", objInvoice.sAmount);
                    cmd.Parameters.AddWithValue("sindentid", objInvoice.sIndentId);
                    cmd.Parameters.AddWithValue("screatedby", objInvoice.sCreatedBy);
                    cmd.Parameters.AddWithValue("smanualinvoiceno", objInvoice.sManualInvoiceNo.ToUpper());
                    cmd.Parameters.AddWithValue("sdtcfailid", objInvoice.sDtcFailId);
                    cmd.Parameters.AddWithValue("stccode", objInvoice.sTcCode);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = objCon.Execute(cmd, strArray, 2);


                    objInvoice.sInvoiceSlNo = strResult[0];
                    string workorderid = objCon.get_value("SELECT \"TI_WO_SLNO\" from \"TBLINDENT\" WHERE \"TI_ID\" ='" + objInvoice.sIndentId + "' limit 1");

                    string invoicenum = objCon.get_value("SELECT \"TR_IN_NO\" from \"TBLTCREPLACE\" WHERE \"TR_WO_SLNO\" = '" + workorderid + "' limit 1");


                    if (invoicenum.Contains('-'))
                    {
                        string upqry = "UPDATE \"TBLTCREPLACE\" set \"TR_IN_NO\"='" + objInvoice.sInvoiceSlNo + "' WHERE \"TR_WO_SLNO\"='" + workorderid + "'";
                        objCon.ExecuteQry(upqry);
                    }
                    if (objInvoice.sTaskType != "3")
                    {
                        string officecode = objCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + objInvoice.sDtcFailId + "'");

                        string faildtrcode = objCon.get_value("SELECT \"DF_EQUIPMENT_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + objInvoice.sDtcFailId + "'");


                        string sParam1 = "SELECT COALESCE(MAX(\"TM_ID\"),0)+1 FROM \"TBLTRANSDTCMAPPING\"";
                        // update invoicing dtr  from store to field  
                        string strQry1 = "UPDATE \"TBLTCMASTER\" SET \"TC_UPDATED_EVENT\"='REPLACE',\"TC_UPDATED_EVENT_ID\"='" + objInvoice.sInvoiceSlNo + "',";
                        strQry1 += " \"TC_CURRENT_LOCATION\"=2,\"TC_LOCATION_ID\"='" + officecode + "' WHERE ";
                        strQry1 += " \"TC_CODE\" ='" + objInvoice.sTcCode + "'";
                        objCon.ExecuteQry(strQry1);

                        // unmap faulty dtr , get the dtr  using faild id  
                        string strQry2 = "UPDATE \"TBLTRANSDTCMAPPING\" SET \"TM_LIVE_FLAG\" = '0', \"TM_UNMAP_CRON\" = NOW() ,\"TM_UNMAP_CRBY\" ='" + objInvoice.sCreatedBy + "',";
                        strQry2 += " \"TM_UNMAP_REASON\" ='FROM FAILURE ENTRY' WHERE \"TM_TC_ID\" ='" + faildtrcode + "'";
                        strQry2 += " AND \"TM_LIVE_FLAG\"='1' AND \"TM_DTC_ID\" ='" + objInvoice.sDTCCode + "'";
                        objCon.ExecuteQry(strQry2);

                        // update invoiced dtr to dtcmast  
                        string strQry3 = "UPDATE \"TBLDTCMAST\" SET \"DT_TC_ID\" ='" + objInvoice.sTcCode + "' WHERE \"DT_CODE\" ='" + objInvoice.sDTCCode + "'";
                        objCon.ExecuteQry(strQry3);

                        // insert into  transdtcmapping  saying dtc and  new dtr has invoiced .
                        string strQry4 = "INSERT INTO \"TBLTRANSDTCMAPPING\"(\"TM_ID\",\"TM_MAPPING_DATE\",\"TM_TC_ID\",\"TM_DTC_ID\",\"TM_LIVE_FLAG\",\"TM_CRBY\",\"TM_CRON\")";
                        strQry4 += " VALUES((SELECT COALESCE(MAX(\"TM_ID\"),0)+1 FROM \"TBLTRANSDTCMAPPING\"), NOW() ,'" + objInvoice.sTcCode + "',";
                        strQry4 += " '" + objInvoice.sDTCCode + "','1','" + objInvoice.sCreatedBy + "', NOW())";
                        objCon.ExecuteQry(strQry4);



                    }

                    string strQryS = string.Empty;
                    string strTemp;



                    NpgsqlCommand = new NpgsqlCommand();
                    strQryS = "SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" =:DtcCode AND \"DF_REPLACE_FLAG\" ='0'";
                    NpgsqlCommand.Parameters.AddWithValue("DtcCode", objInvoice.sDTCCode);
                    string df_id = objCon.get_value(strQryS, NpgsqlCommand);

                    // NpgsqlCommand = new NpgsqlCommand();
                    //  strTemp = "SELECT \"TD_TC_NO\" FROM \"TBLTCDRAWN\" WHERE \"TD_DF_ID\" =:df_id";
                    // NpgsqlCommand.Parameters.AddWithValue("df_id", Convert.ToInt32(df_id));
                    string sReplaceTCCode = objInvoice.sTcCode;


                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN 1 ELSE 0 END FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:sReplaceTCCode";
                    NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode", Convert.ToDouble(sReplaceTCCode));
                    string sval = objCon.get_value(strQry, NpgsqlCommand);

                    if (sval == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT TO_CHAR(ADD_MONTHS(\"TM_MAPPING_DATE\",\"WARENTY_MONTH\"),'YYYY-MM-DD') WARENTY_DATE FROM (SELECT \"RSD_GUARRENTY_TYPE\" ,";
                        strQry += " (\"RSD_WARENTY_PERIOD\" * 12) \"WARENTY_MONTH\", \"TC_CODE\", \"TM_MAPPING_DATE\" FROM \"TBLTRANSDTCMAPPING\", \"TBLREPAIRSENTDETAILS\",";
                        strQry += " \"TBLTCMASTER\" WHERE  \"TC_CODE\"=\"RSD_TC_CODE\" AND \"RSD_TC_CODE\"=\"TM_TC_ID\" AND \"TC_CODE\"=:sReplaceTCCode1 AND \"TM_LIVE_FLAG\" =1)A";
                        NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode1", Convert.ToDouble(sReplaceTCCode));
                        sDtrWarrentyTime = objCon.get_value(strQry, NpgsqlCommand);

                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"RSD_WARENTY_PERIOD\" FROM \"TBLTRANSDTCMAPPING\", \"TBLREPAIRSENTDETAILS\", \"TBLTCMASTER\" WHERE \"TC_CODE\" = \"RSD_TC_CODE\" AND ";
                        strQry += " \"RSD_TC_CODE\" = \"TM_TC_ID\"  AND \"TC_CODE\" =:sReplaceTCCode2 AND \"TM_LIVE_FLAG\" =1";
                        NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode2", Convert.ToDouble(sReplaceTCCode));
                        sWarrentyPeriod = objCon.get_value(strQry, NpgsqlCommand);
                    }
                    //IFormatProvider culture = new CultureInfo("en-US", true);
                    //DateTime dateVal = DateTime.ParseExact(sDtrWarrentyTime, "yyyy-MM-dd", culture);

                    if (sval == "0")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_WARANTY_PERIOD\" =TO_DATE(:sDtrWarrentyTime,'yyyy-MM-dd'), \"TC_WARRENTY\" =:sWarrentyPeriod1 WHERE ";
                        strQry += " \"TC_CODE\" =:sReplaceTCCode3";
                        NpgsqlCommand.Parameters.AddWithValue("sDtrWarrentyTime", sDtrWarrentyTime);

                        if (sWarrentyPeriod == "" || sWarrentyPeriod == null)
                        {
                            sWarrentyPeriod = "0";
                        }
                        NpgsqlCommand.Parameters.AddWithValue("sWarrentyPeriod1", Convert.ToInt64(sWarrentyPeriod));
                        NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode3", Convert.ToDouble(sReplaceTCCode));
                        objCon.ExecuteQry(strQry, NpgsqlCommand);
                    }




                    //objCon.BeginTrans();

                    //strQry = "INSERT into TBLDTCINVOICE (IN_NO,IN_INV_NO,IN_DATE,IN_DESC,IN_AMT,IN_TI_NO,IN_CRBY,IN_MANUAL_INVNO) ";
                    //strQry += "values ('" + objInvoice.sInvoiceSlNo + "','" + objInvoice.sInvoiceNo.ToUpper() + "',TO_DATE('" + objInvoice.sInvoiceDate + "','dd/MM/yyyy'),";
                    //strQry += " '" + objInvoice.sInvoiceDescription + "','" + objInvoice.sAmount + "','" + objInvoice.sIndentId + "',";
                    //strQry += "'" + objInvoice.sCreatedBy + "','"+ objInvoice.sManualInvoiceNo.ToUpper() +"')";
                    //objCon.Execute(strQry);


                    ////Insert to new TC Details to TBLTCDRAWN
                    //string strMax = Convert.ToString(objCon.Get_max_no("TD_ID", "TBLTCDRAWN"));
                    //strQry = "INSERT INTO TBLTCDRAWN (TD_ID,TD_DF_ID,TD_TC_NO,TD_INV_NO,TD_DESC,TD_CRON)";
                    //strQry += " VALUES ('" + strMax + "','" + objInvoice.sDtcFailId + "','" + objInvoice.sTcCode + "', ";
                    //strQry += " '" + objInvoice.sInvoiceSlNo + "','" + objInvoice.sInvoiceDescription + "',SYSDATE)";
                    //objCon.Execute(strQry);


                    //strQry = " SELECT DF_LOC_CODE  FROM TBLDTCFAILURE WHERE DF_ID='" + objInvoice.sDtcFailId + "'";
                    //string sFailOfficeCode = objCon.get_value(strQry);               

                    ////if (objInvoice.sOfficeCode.Length > 1)
                    ////{
                    ////    objInvoice.sOfficeCode = objInvoice.sOfficeCode.Substring(0, 2);
                    ////}

                    ////Update status to TCMaster Table
                    //strQry = "UPDATE TBLTCMASTER SET TC_UPDATED_EVENT='Drawn',TC_UPDATED_EVENT_ID='" + strMax + "',";
                    //strQry += " TC_CURRENT_LOCATION=2,TC_LOCATION_ID='" + sFailOfficeCode + "' WHERE TC_CODE='" + objInvoice.sTcCode + "'";
                    //objCon.Execute(strQry);

                    ////objCon.Execute("DELETE TBLTCDRAWNBUFFER WHERE BU_FAIL_ID='" + strFailureId + "'");

                    //objCon.CommitTrans();

                    #region WorkFlow

                    //Workflow / Approval
                    clsApproval objApproval = new clsApproval();
                    objApproval.sFormName = objInvoice.sFormName;
                    objApproval.sRecordId = objInvoice.sInvoiceSlNo;
                    objApproval.sOfficeCode = objInvoice.sOfficeCode;
                    objApproval.sClientIp = objInvoice.sClientIP;
                    objApproval.sCrby = objInvoice.sCreatedBy;
                    objApproval.sWFObjectId = objInvoice.sWFOId;
                    objApproval.sDataReferenceId = objInvoice.sIndentId;
                    objApproval.sWFAutoId = objInvoice.sWFAutoId;
                    if (objInvoice.sTaskType == "3")
                    {
                        objApproval.sStoreType = "2";
                        //objApproval.sFormName = "NewDTCCommission";
                    }
                    else
                    {
                        objApproval.sStoreType = "1";
                        //objApproval.sFormName = "InvoiceCreation";
                    }

                    objApproval.sDescription = "Invoice Creation for Indent No " + objInvoice.sIndentNo;

                    if (objInvoice.sTaskType != "3")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sIndentId2", Convert.ToInt32(objInvoice.sIndentId));
                        objApproval.sRefOfficeCode = objCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\",\"TBLINDENT\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=:sIndentId2", NpgsqlCommand);
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sIndentId3", Convert.ToInt32(objInvoice.sIndentId));
                        objApproval.sRefOfficeCode = objCon.get_value("SELECT \"WO_REQUEST_LOC\" FROM \"TBLWORKORDER\",\"TBLINDENT\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=:sIndentId3", NpgsqlCommand);
                    }


                    bool bResult = objApproval.CheckDuplicateApprove(objApproval);
                    if (bResult == false)
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }
                    objApproval.SaveWorkflowObjects(objApproval);

                    #endregion

                    Arr[0] = "Invoice Created Successfully";
                    Arr[1] = "0";
                    objCon.CommitTransaction();
                    return Arr;

                }
                else
                {
                    //objCon.BeginTrans();

                    objCon.BeginTransaction();
                    NpgsqlCommand ncmd = new NpgsqlCommand("sp_update_invoice");
                    ncmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo.ToUpper());
                    ncmd.Parameters.AddWithValue("sinvoicedate", objInvoice.sInvoiceDate);
                    ncmd.Parameters.AddWithValue("sinvoicedescription", objInvoice.sInvoiceDescription);
                    ncmd.Parameters.AddWithValue("samount", objInvoice.sAmount);
                    ncmd.Parameters.AddWithValue("sinvoiceslno", objInvoice.sInvoiceSlNo);
                    ncmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    ncmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    ncmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    ncmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = objCon.Execute(ncmd, strArray, 2);
                    objCon.CommitTransaction();
                    return strResult;

                    //dr = objCon.Fetch("select * from TBLDTCINVOICE where  UPPER(IN_INV_NO)='" + objInvoice.sInvoiceNo.ToUpper() + "' and IN_NO<>'" + objInvoice.sInvoiceSlNo + "'");
                    //if (dr.Read())
                    //{

                    //    Arr[0] = "Invoice No. Already Exists";
                    //    Arr[1] = "2";
                    //    return Arr;
                    //}
                    //dr.Close();

                    //strQry = "UPDATE TBLDTCINVOICE SET IN_INV_NO='" + objInvoice.sInvoiceNo.ToUpper() + "',";
                    //strQry+= " IN_DATE=to_date('" + objInvoice.sInvoiceDate + "','dd/MM/yyyy'),IN_DESC='" + objInvoice.sInvoiceDescription + "',";
                    //strQry+= " IN_AMT='" + objInvoice.sAmount + "' where IN_NO='" + objInvoice.sInvoiceSlNo + "'";
                    //objCon.Execute(strQry);

                    //objCon.CommitTrans();
                    //Arr[0] = "Invoice Updated Successfully";
                    //Arr[1] = "1";
                    //return Arr;
                }
            }
            catch (Exception ex)
            {
                //objCon.RollBack();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public string[] SaveUpdateInvoiceDetails1(clsInvoice objInvoice)
        {

            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);

            string[] Arr = new string[2];
            string strQry = string.Empty;
            try
            {
                objDatabse.BeginTransaction();
                string[] strResult = new string[2];
                string[] strArray = new string[2];



                //Check Work Order no exists or not
                //NpgsqlCommand = new NpgsqlCommand();
                //NpgsqlCommand.Parameters.AddWithValue("sIndentNo", objInvoice.sIndentNo.ToUpper());
                string sInNo = objDatabse.get_value("SELECT \"TI_INDENT_NO\" FROM \"TBLINDENT\" WHERE UPPER(\"TI_INDENT_NO\")='" + objInvoice.sIndentNo.ToUpper() + "'");
                if (sInNo.Length == 0)
                {
                    Arr[0] = "Enter Valid Indent No";
                    Arr[1] = "2";
                    return Arr;
                }

                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sTcCode", Convert.ToDouble(objInvoice.sTcCode));
                string sTccode = objDatabse.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" ='" + Convert.ToDouble(objInvoice.sTcCode) + "' ");
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Enter Valid DTr Code";
                    Arr[1] = "2";
                    return Arr;
                }

                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sTcCode1", Convert.ToDouble(objInvoice.sTcCode));
                sTccode = objDatabse.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" ='" + Convert.ToDouble(objInvoice.sTcCode) + "' AND \"TC_STATUS\" IN ('1','2','5') AND \"TC_CURRENT_LOCATION\" in (1,5)");
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Entered DTr Code not in Store or Not in good condition";
                    Arr[1] = "2";
                    return Arr;
                }

                // NpgsqlCommand = new NpgsqlCommand();
                // NpgsqlCommand.Parameters.AddWithValue("sTcCode2", Convert.ToDouble(objInvoice.sTcCode));
                // NpgsqlCommand.Parameters.AddWithValue("sTcNewCapacity", Convert.ToDouble(objInvoice.sTcNewCapacity));
                sTccode = objDatabse.get_value("SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" ='" + Convert.ToDouble(objInvoice.sTcCode) + "' AND \"TC_CAPACITY\" ='" + Convert.ToDouble(objInvoice.sTcNewCapacity) + "'");
                if (sTccode.Length == 0)
                {
                    Arr[0] = "Entered DTr Code Capacity not Matching with Requested Capacity";
                    Arr[1] = "2";
                    return Arr;
                }


                //NpgsqlCommand = new NpgsqlCommand();
                //NpgsqlCommand.Parameters.AddWithValue("sDTCCODE", objInvoice.sDTCCode);
                //NpgsqlCommand.Parameters.AddWithValue("sOldTcCode", objInvoice.sOldTcCode);
                //NpgsqlCommand.Parameters.AddWithValue("sIndentId", objInvoice.sIndentId);
                sInNo = objDatabse.get_value("SELECT \"TI_INDENT_NO\" FROM \"TBLINDENT\",\"TBLWORKORDER\",\"TBLDTCFAILURE\",\"TBLDTCINVOICE\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=\"IN_TI_NO\" AND CAST(\"DF_DTC_CODE\" AS TEXT) ='" + objInvoice.sDTCCode + "' AND CAST(\"DF_EQUIPMENT_ID\" AS TEXT)='" + objInvoice.sOldTcCode + "'  AND CAST(\"TI_ID\" AS TEXT)='" + objInvoice.sIndentId + "'");
                if (sInNo.Length > 0)
                {
                    Arr[0] = "Invoice Already done for this DTC";
                    Arr[1] = "2";
                    return Arr;
                }


                if (objInvoice.sInvoiceSlNo == "")
                {
                    //NpgsqlCommand = new NpgsqlCommand();
                    //NpgsqlCommand.Parameters.AddWithValue("sInvoiceNo", objInvoice.sInvoiceNo.ToUpper());

                    //dr = objCon.Fetch("select * from TBLDTCINVOICE where  UPPER(IN_INV_NO)='" + objInvoice.sInvoiceNo.ToUpper() + "' ");
                    string sId = objDatabse.get_value("SELECT \"IN_NO\" FROM \"TBLDTCINVOICE\" WHERE  UPPER(\"IN_INV_NO\")='" + objInvoice.sInvoiceNo.ToUpper() + "' ");
                    if (sId.Length > 0)
                    {
                        Arr[0] = "Invoice No. Already Exists";
                        Arr[1] = "2";
                        return Arr;
                    }
                    //if (dr.Read())
                    //{

                    //    Arr[0] = "Invoice No. Already Exists";
                    //    Arr[1] = "2";
                    //    return Arr;
                    //}
                    //dr.Close();

                    // objCon.BeginTransaction();

                    NpgsqlCommand cmd = new NpgsqlCommand("sp_save_invoice");
                    cmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo.ToUpper());
                    cmd.Parameters.AddWithValue("sinvoicedate", objInvoice.sInvoiceDate);
                    cmd.Parameters.AddWithValue("sinvoicedescription", objInvoice.sInvoiceDescription);
                    cmd.Parameters.AddWithValue("samount", objInvoice.sAmount);
                    cmd.Parameters.AddWithValue("sindentid", objInvoice.sIndentId);
                    cmd.Parameters.AddWithValue("screatedby", objInvoice.sCreatedBy);
                    cmd.Parameters.AddWithValue("smanualinvoiceno", objInvoice.sManualInvoiceNo.ToUpper());
                    cmd.Parameters.AddWithValue("sdtcfailid", objInvoice.sDtcFailId);
                    cmd.Parameters.AddWithValue("stccode", objInvoice.sTcCode);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = objDatabse.Execute(cmd, strArray, 2);


                    objInvoice.sInvoiceSlNo = strResult[0];
                    string workorderid = objDatabse.get_value("SELECT \"TI_WO_SLNO\" from \"TBLINDENT\" WHERE \"TI_ID\" ='" + objInvoice.sIndentId + "' limit 1");

                    string invoicenum = objDatabse.get_value("SELECT \"TR_IN_NO\" from \"TBLTCREPLACE\" WHERE \"TR_WO_SLNO\" = '" + workorderid + "' limit 1");


                    string upqry = "UPDATE \"TBLTCREPLACE\" set \"TR_IN_NO\"='" + objInvoice.sInvoiceSlNo + "' WHERE \"TR_WO_SLNO\"='" + workorderid + "'";
                    objDatabse.ExecuteQry(upqry);


                    if (objInvoice.sTaskType != "3")
                    {
                        string officecode = objDatabse.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + objInvoice.sDtcFailId + "'");

                        string faildtrcode = objDatabse.get_value("SELECT \"DF_EQUIPMENT_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"='" + objInvoice.sDtcFailId + "'");


                        string sParam1 = "SELECT COALESCE(MAX(\"TM_ID\"),0)+1 FROM \"TBLTRANSDTCMAPPING\"";
                        // update invoicing dtr  from store to field  
                        string strQry1 = "UPDATE \"TBLTCMASTER\" SET \"TC_UPDATED_EVENT\"='REPLACE',\"TC_UPDATED_EVENT_ID\"='" + objInvoice.sInvoiceSlNo + "',";
                        strQry1 += " \"TC_CURRENT_LOCATION\"=2,\"TC_LOCATION_ID\"='" + officecode + "' WHERE ";
                        strQry1 += " \"TC_CODE\" ='" + objInvoice.sTcCode + "'";
                        objDatabse.ExecuteQry(strQry1);

                        // unmap faulty dtr , get the dtr  using faild id  
                        string strQry2 = "UPDATE \"TBLTRANSDTCMAPPING\" SET \"TM_LIVE_FLAG\" = '0', \"TM_UNMAP_CRON\" = NOW() ,\"TM_UNMAP_CRBY\" ='" + objInvoice.sCreatedBy + "',";
                        strQry2 += " \"TM_UNMAP_REASON\" ='FROM FAILURE ENTRY' WHERE \"TM_TC_ID\" ='" + faildtrcode + "'";
                        strQry2 += " AND \"TM_LIVE_FLAG\"='1' AND \"TM_DTC_ID\" ='" + objInvoice.sDTCCode + "'";
                        objDatabse.ExecuteQry(strQry2);

                        // update invoiced dtr to dtcmast  
                        string strQry3 = "UPDATE \"TBLDTCMAST\" SET \"DT_TC_ID\" ='" + objInvoice.sTcCode + "' WHERE \"DT_CODE\" ='" + objInvoice.sDTCCode + "'";
                        objDatabse.ExecuteQry(strQry3);

                        // insert into  transdtcmapping  saying dtc and  new dtr has invoiced .
                        string strQry4 = "INSERT INTO \"TBLTRANSDTCMAPPING\"(\"TM_ID\",\"TM_MAPPING_DATE\",\"TM_TC_ID\",\"TM_DTC_ID\",\"TM_LIVE_FLAG\",\"TM_CRBY\",\"TM_CRON\")";
                        strQry4 += " VALUES((SELECT COALESCE(MAX(\"TM_ID\"),0)+1 FROM \"TBLTRANSDTCMAPPING\"), NOW() ,'" + objInvoice.sTcCode + "',";
                        strQry4 += " '" + objInvoice.sDTCCode + "','1','" + objInvoice.sCreatedBy + "', NOW())";
                        objDatabse.ExecuteQry(strQry4);



                    }

                    string strQryS = string.Empty;
                    string strTemp;



                    // NpgsqlCommand = new NpgsqlCommand();
                    strQryS = "SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_DTC_CODE\" ='" + objInvoice.sDTCCode + "' AND \"DF_REPLACE_FLAG\" ='0'";
                    // NpgsqlCommand.Parameters.AddWithValue("DtcCode", objInvoice.sDTCCode);
                    string df_id = objDatabse.get_value(strQryS);

                    // NpgsqlCommand = new NpgsqlCommand();
                    //  strTemp = "SELECT \"TD_TC_NO\" FROM \"TBLTCDRAWN\" WHERE \"TD_DF_ID\" =:df_id";
                    // NpgsqlCommand.Parameters.AddWithValue("df_id", Convert.ToInt32(df_id));
                    string sReplaceTCCode = objInvoice.sTcCode;


                    //  NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT CASE WHEN NOW() < \"TC_WARANTY_PERIOD\" THEN 1 ELSE 0 END FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" ='" + Convert.ToDouble(sReplaceTCCode) + "'";
                    //  NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode", Convert.ToDouble(sReplaceTCCode));
                    string sval = objDatabse.get_value(strQry);

                    if (sval == "0")
                    {
                        //   NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT TO_CHAR(ADD_MONTHS(\"TM_MAPPING_DATE\",\"WARENTY_MONTH\"),'YYYY-MM-DD') WARENTY_DATE FROM (SELECT \"RSD_GUARRENTY_TYPE\" ,";
                        strQry += " (\"RSD_WARENTY_PERIOD\" * 12) \"WARENTY_MONTH\", \"TC_CODE\", \"TM_MAPPING_DATE\" FROM \"TBLTRANSDTCMAPPING\", \"TBLREPAIRSENTDETAILS\",";
                        strQry += " \"TBLTCMASTER\" WHERE  \"TC_CODE\"=\"RSD_TC_CODE\" AND \"RSD_TC_CODE\"=\"TM_TC_ID\" AND \"TC_CODE\"='" + Convert.ToDouble(sReplaceTCCode) + "' AND \"TM_LIVE_FLAG\" =1)A";
                        // NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode1", Convert.ToDouble(sReplaceTCCode));
                        sDtrWarrentyTime = objDatabse.get_value(strQry);

                        //NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"RSD_WARENTY_PERIOD\" FROM \"TBLTRANSDTCMAPPING\", \"TBLREPAIRSENTDETAILS\", \"TBLTCMASTER\" WHERE \"TC_CODE\" = \"RSD_TC_CODE\" AND ";
                        strQry += " \"RSD_TC_CODE\" = \"TM_TC_ID\"  AND \"TC_CODE\" ='" + Convert.ToDouble(sReplaceTCCode) + "' AND \"TM_LIVE_FLAG\" =1";
                        // NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode2", Convert.ToDouble(sReplaceTCCode));
                        sWarrentyPeriod = objDatabse.get_value(strQry);
                    }
                    //IFormatProvider culture = new CultureInfo("en-US", true);
                    //DateTime dateVal = DateTime.ParseExact(sDtrWarrentyTime, "yyyy-MM-dd", culture);

                    if (sval == "0")
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        if (sWarrentyPeriod == "" || sWarrentyPeriod == null)
                        {
                            sWarrentyPeriod = "0";
                        }
                        strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_WARANTY_PERIOD\" =TO_DATE('" + sDtrWarrentyTime + "','yyyy-MM-dd'), \"TC_WARRENTY\" ='" + Convert.ToInt64(sWarrentyPeriod) + "' WHERE ";
                        strQry += " \"TC_CODE\" ='" + Convert.ToDouble(sReplaceTCCode) + "'";
                        //NpgsqlCommand.Parameters.AddWithValue("sDtrWarrentyTime", sDtrWarrentyTime);


                        //NpgsqlCommand.Parameters.AddWithValue("sWarrentyPeriod1", Convert.ToInt64(sWarrentyPeriod));
                        //NpgsqlCommand.Parameters.AddWithValue("sReplaceTCCode3", Convert.ToDouble(sReplaceTCCode));
                        objDatabse.ExecuteQry(strQry);
                    }






                    #region WorkFlow

                    //Workflow / Approval
                    clsApproval objApproval = new clsApproval();
                    objApproval.sFormName = objInvoice.sFormName;
                    objApproval.sRecordId = objInvoice.sInvoiceSlNo;
                    objApproval.sOfficeCode = objInvoice.sOfficeCode;
                    objApproval.sClientIp = objInvoice.sClientIP;
                    objApproval.sCrby = objInvoice.sCreatedBy;
                    objApproval.sWFObjectId = objInvoice.sWFOId;
                    objApproval.sDataReferenceId = objInvoice.sIndentId;
                    objApproval.sWFAutoId = objInvoice.sWFAutoId;
                    if (objInvoice.sTaskType == "3")
                    {
                        objApproval.sStoreType = "2";
                        //objApproval.sFormName = "NewDTCCommission";
                    }
                    else
                    {
                        objApproval.sStoreType = "1";
                        //objApproval.sFormName = "InvoiceCreation";
                    }

                    objApproval.sDescription = "Invoice Creation for Indent No " + objInvoice.sIndentNo;

                    if (objInvoice.sTaskType != "3")
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sIndentId2", Convert.ToInt32(objInvoice.sIndentId));
                        objApproval.sRefOfficeCode = objDatabse.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\",\"TBLINDENT\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=" + Convert.ToInt32(objInvoice.sIndentId) + "");
                    }
                    else
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sIndentId3", Convert.ToInt32(objInvoice.sIndentId));
                        objApproval.sRefOfficeCode = objDatabse.get_value("SELECT \"WO_REQUEST_LOC\" FROM \"TBLWORKORDER\",\"TBLINDENT\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND \"TI_ID\"=" + Convert.ToInt32(objInvoice.sIndentId) + "");
                    }


                    bool bResult = objApproval.CheckDuplicateApprove1(objApproval, objDatabse);
                    if (bResult == false)
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }
                    objApproval.SaveWorkflowObjects1(objApproval, objDatabse);

                    #endregion

                    Arr[0] = "Invoice Created Successfully";
                    Arr[1] = "0";
                    objDatabse.CommitTransaction();
                    return Arr;

                }
                else
                {
                    //objCon.BeginTrans();

                    // objCon.BeginTransaction();
                    NpgsqlCommand ncmd = new NpgsqlCommand("sp_update_invoice");
                    ncmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo.ToUpper());
                    ncmd.Parameters.AddWithValue("sinvoicedate", objInvoice.sInvoiceDate);
                    ncmd.Parameters.AddWithValue("sinvoicedescription", objInvoice.sInvoiceDescription);
                    ncmd.Parameters.AddWithValue("samount", objInvoice.sAmount);
                    ncmd.Parameters.AddWithValue("sinvoiceslno", objInvoice.sInvoiceSlNo);
                    ncmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    ncmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    ncmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    ncmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = objDatabse.Execute(ncmd, strArray, 2);
                    objDatabse.CommitTransaction();
                    return strResult;


                }
            }
            catch (Exception ex)
            {
                //objCon.RollBack();
                objDatabse.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                throw ex;
                //  return Arr;
            }
        }
        public DataTable LoadAllInvoiceDetails(clsInvoice objInvoice)
        {
            DataTable dt = new DataTable();
            try
            {

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadallinvoicedetails");
                cmd.Parameters.AddWithValue("stasktype", objInvoice.sTaskType);
                cmd.Parameters.AddWithValue("sofficecode", objInvoice.sOfficeCode);
                dt = objCon.FetchDataTable(cmd);

                //string strQry = string.Empty;
                //strQry = "SELECT  DT_NAME,TO_CHAR(TC_CODE) TC_CODE ,TI_ID,TI_INDENT_NO,WO_NO,0 AS IN_NO ,'' AS IN_INV_NO ,'NO' AS STATUS  ";
                //strQry += " FROM TBLDTCMAST,TBLTCMASTER,TBLDTCFAILURE,TBLWORKORDER,TBLINDENT ";
                //strQry += " WHERE DF_EQUIPMENT_ID = TC_CODE  AND DT_CODE = objInvoice.sOfficeCode AND DF_REPLACE_FLAG = 0  ";
                //strQry += " AND DF_ID =  WO_DF_ID AND WO_SLNO = TI_WO_SLNO AND DF_STATUS_FLAG = '" + objInvoice.sTaskType + "' AND TI_ID NOT IN ";
                //strQry += " (SELECT IN_TI_NO FROM TBLDTCINVOICE)   ";
                //strQry += " AND DF_LOC_CODE LIKE '" + objInvoice.sOfficeCode + "%'";
                //strQry += " UNION ALL ";
                //strQry += " SELECT  DT_NAME,TO_CHAR(TC_CODE) TC_CODE,TI_ID,TI_INDENT_NO,WO_NO,IN_NO,IN_INV_NO ,'YES' AS STATUS FROM  ";
                //strQry += " TBLDTCMAST,TBLTCMASTER, TBLDTCFAILURE , TBLWORKORDER, TBLINDENT, TBLDTCINVOICE ";
                //strQry += " WHERE DF_EQUIPMENT_ID = TC_CODE  AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_STATUS_FLAG = '" + objInvoice.sTaskType + "' ";
                //strQry += " AND DF_ID =  WO_DF_ID AND WO_SLNO = TI_WO_SLNO AND TI_ID = IN_TI_NO  ";
                //strQry += " AND DF_LOC_CODE LIKE '" + objInvoice.sOfficeCode + "%'";
                //dt = objCon.getDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public DataTable LoadExistingInvoice(clsInvoice objInvoice)
        {
            DataTable dt = new DataTable();
            string strQry = string.Empty;
            try
            {

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadexistinginvoice");
                cmd.Parameters.AddWithValue("stasktype", objInvoice.sTaskType);
                cmd.Parameters.AddWithValue("sofficecode", objInvoice.sOfficeCode);
                dt = objCon.FetchDataTable(cmd);

                //strQry = "SELECT  DT_NAME,TO_CHAR(TC_CODE) TC_CODE,TI_ID ,TI_INDENT_NO , WO_NO, IN_NO, IN_INV_NO, 'YES' AS STATUS  FROM TBLDTCMAST,";
                //strQry += " TBLTCMASTER, TBLDTCFAILURE , TBLWORKORDER, TBLINDENT, TBLDTCINVOICE";
                //strQry += " WHERE DF_EQUIPMENT_ID = TC_CODE  AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_STATUS_FLAG = '" + objInvoice.sTaskType + "' ";
                //strQry += " AND DF_ID =  WO_DF_ID AND WO_SLNO = TI_WO_SLNO AND TI_ID = IN_TI_NO  ";
                //strQry += " AND DF_LOC_CODE LIKE '" + objInvoice.sOfficeCode + "%'";
                //dt = objCon.getDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public clsInvoice GetTCDetails(clsInvoice objInvoice)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                if (objInvoice.sTcCapacity != null)
                {
                    if (objInvoice.sStoreType == "1")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE CAST(\"TC_LOCATION_ID\" AS TEXT) =:sOfficeCode AND \"TC_STATUS\" in (1,2,5) AND ";
                        strQry += " \"TC_CURRENT_LOCATION\" =1 AND CAST(\"TC_CAPACITY\"  AS TEXT) =:sTcCapacity AND (CAST(\"TC_CODE\" AS TEXT) =:sTcCode or  CAST(\"TC_CODE\" AS TEXT) =:sTcCode1)";
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", objInvoice.sOfficeCode);
                        // NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", clsStoreOffice.GetStoreID(objInvoice.sOfficeCode));
                        NpgsqlCommand.Parameters.AddWithValue("sTcCapacity", objInvoice.sTcCapacity);
                        NpgsqlCommand.Parameters.AddWithValue("sTcCode", objInvoice.sTcCode);
                        NpgsqlCommand.Parameters.AddWithValue("sTcCode1", objInvoice.sTcCode);
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        strQry = "SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE CAST(\"TC_LOCATION_ID\" AS TEXT) =:sOfficeCode1 AND \"TC_STATUS\" in (1,2) AND ";
                        strQry += " \"TC_CURRENT_LOCATION\" =5 AND CAST(\"TC_CAPACITY\"  AS TEXT) =:sTcCapacity1 AND (cast(\"TC_CODE\" as text)  =:sTcCode2 or  CAST(\"TC_CODE\" AS TEXT) =:sTcCode3)";
                        NpgsqlCommand.Parameters.AddWithValue("sOfficeCode1", clsStoreOffice.GetStoreID(objInvoice.sOfficeCode));
                        NpgsqlCommand.Parameters.AddWithValue("sTcCapacity1", objInvoice.sTcCapacity);
                        NpgsqlCommand.Parameters.AddWithValue("sTcCode2", objInvoice.sTcCode);
                        NpgsqlCommand.Parameters.AddWithValue("sTcCode3", objInvoice.sTcCode);
                    }

                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = "SELECT \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE  \"TC_STATUS\" in (1,2) AND ";
                    strQry += " (\"TC_CODE\"  ='" + objInvoice.sTcCode + "' or  CAST(\"TC_CODE\" AS TEXT) =:sTcCode4)";
                    NpgsqlCommand.Parameters.AddWithValue("sTcCode4", objInvoice.sTcCode);
                }

                string res = objCon.get_value(strQry, NpgsqlCommand);

                if (res != "")
                {

                    //strQry = "SELECT TC_SLNO,TC_CODE,TM_NAME,TO_CHAR(TC_CAPACITY) TC_CAPACITY FROM TBLTCMASTER,TBLTRANSMAKES  ";
                    //strQry += " WHERE TC_MAKE_ID= TM_ID and TC_CODE='" + objInvoice.sTcCode + "'";
                    //dt = objCon.getDataTable(strQry);

                    NpgsqlCommand cmd = new NpgsqlCommand("sp_gettcdetails");
                    cmd.Parameters.AddWithValue("stccode", objInvoice.sTcCode);
                    dt = objCon.FetchDataTable(cmd);

                    if (dt.Rows.Count > 0)
                    {
                        objInvoice.sTcCode = dt.Rows[0]["TC_CODE"].ToString();
                        objInvoice.sTcSlNo = dt.Rows[0]["TC_SLNO"].ToString();
                        objInvoice.sTcMake = dt.Rows[0]["TM_NAME"].ToString();
                        objInvoice.sTcCapacity = dt.Rows[0]["TC_CAPACITY"].ToString();
                        objInvoice.sTcRating = dt.Rows[0]["TC_RATING"].ToString();
                        objInvoice.smanufactureDate = dt.Rows[0]["TC_MANF_DATE"].ToString();
                        objInvoice.sStoreId = dt.Rows[0]["TC_STORE_ID"].ToString();
                    }
                }
                else
                {
                    objInvoice.sTcCode = "";
                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }


        public object GetBasicDetails(clsInvoice objInvoice)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getbasicdetails");
                cmd.Parameters.AddWithValue("sindentid", objInvoice.sIndentId);
                dt = objCon.FetchDataTable(cmd);

                //strQry = " SELECT DF_ID,DT_NAME,DT_CODE, TI_ID, TO_CHAR(TI_INDENT_DATE,'DD/MM/YYYY') AS INDENTDDATE ,TI_INDENT_NO, ";
                //strQry += "(SELECT US_FULL_NAME FROM TBLUSER WHERE US_ID = TI_CRBY) USERNAME, WO_DTC_CAP, WO_NEW_CAP ,WO_AMT,";
                //strQry += "(SELECT TC_SLNO FROM TBLTCMASTER WHERE TC_CODE = DF_EQUIPMENT_ID) TC_SLNO, ";
                //strQry += " DF_EQUIPMENT_ID AS TC_CODE,DT_ID,(SELECT TC_ID FROM TBLTCMASTER WHERE DF_EQUIPMENT_ID=TC_CODE) TC_ID, TO_CHAR(DF_DATE,'DD/MM/YYYY') AS DF_DATE ";
                //strQry += " from TBLDTCMAST,TBLDTCFAILURE, TBLWORKORDER, TBLINDENT WHERE DF_DTC_CODE= DT_CODE  AND  ";
                //strQry += "WO_DF_ID = DF_ID AND TI_WO_SLNO = WO_SLNO AND TI_ID = '" + objInvoice.sIndentId + "' ";

                //dt = objCon.getDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    objInvoice.sDTCName = dt.Rows[0]["DT_NAME"].ToString(); ;
                    objInvoice.sIndentDate = dt.Rows[0]["INDENTDDATE"].ToString();
                    objInvoice.sIndentCrby = dt.Rows[0]["USERNAME"].ToString();
                    objInvoice.sDtcFailId = dt.Rows[0]["DF_ID"].ToString();
                    objInvoice.sTcNewCapacity = dt.Rows[0]["WO_NEW_CAP"].ToString();
                    objInvoice.sTcCapacity = dt.Rows[0]["WO_DTC_CAP"].ToString();
                    objInvoice.sOldTcSlno = dt.Rows[0]["TC_SLNO"].ToString();
                    objInvoice.sIndentNo = dt.Rows[0]["TI_INDENT_NO"].ToString();
                    objInvoice.sOldTcCode = dt.Rows[0]["TC_CODE"].ToString();
                    objInvoice.sDTCId = dt.Rows[0]["DT_ID"].ToString();
                    objInvoice.sTCId = dt.Rows[0]["TC_ID"].ToString();
                    objInvoice.sAmount = dt.Rows[0]["WO_AMT"].ToString();
                    objInvoice.sFailDate = dt.Rows[0]["DF_DATE"].ToString();
                    objInvoice.sDTCCode = dt.Rows[0]["DT_CODE"].ToString();


                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }

        public object GetInvoiceDetails(clsInvoice objInvoice)
        {

            try
            {
                DataTable dtInvoiceDetails = new DataTable();
                string strQry = string.Empty;

                //strQry = " SELECT IN_NO,IN_INV_NO,IN_TI_NO,IN_AMT,TO_CHAR(IN_DATE,'dd/MM/yyyy')IN_DATE,IN_DESC,TC_SLNO,TM_NAME,TC_CODE,";
                //strQry += " TO_CHAR(TC_CAPACITY) TC_CAPACITY,IN_MANUAL_INVNO FROM TBLDTCINVOICE,TBLTCMASTER,TBLTCDRAWN,TBLTRANSMAKES ";
                //strQry += " WHERE TD_TC_NO= TC_CODE AND TC_MAKE_ID= TM_ID AND IN_NO='" + objInvoice.sInvoiceSlNo + "' AND IN_NO=TD_INV_NO";
                //dtInvoiceDetails = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getinvoicedetails");
                cmd.Parameters.AddWithValue("sinvoiceslno", objInvoice.sInvoiceSlNo);
                dtInvoiceDetails = objCon.FetchDataTable(cmd);


                if (dtInvoiceDetails.Rows.Count > 0)
                {
                    objInvoice.sInvoiceNo = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_INV_NO"]);
                    objInvoice.sInvoiceDate = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_DATE"]);
                    objInvoice.sAmount = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_AMT"]);
                    objInvoice.sInvoiceDescription = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_DESC"]);

                    objInvoice.sTcCode = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_CODE"]);
                    objInvoice.sTcSlNo = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_SLNO"]);
                    objInvoice.sTcMake = Convert.ToString(dtInvoiceDetails.Rows[0]["TM_NAME"]);
                    objInvoice.sTcCapacity = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_CAPACITY"]);
                    objInvoice.sManualInvoiceNo = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_MANUAL_INVNO"]);
                }


                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;

            }
        }
        public bool ValidateUpdate(string sInvoiceId)
        {
            try
            {
                DataTable dt = new DataTable();
                NpgsqlCommand = new NpgsqlCommand();
                string sQry = " SELECT \"IN_TI_NO\" FROM \"TBLDTCINVOICE\", \"TBLTCREPLACE\" , \"TBLINDENT\" WHERE \"TR_IN_NO\"=\"IN_NO\" ";
                sQry += " AND \"IN_TI_NO\"= \"TI_ID\" AND \"IN_NO\" =:sInvoiceId";
                string sResult = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("sInvoiceId", Convert.ToInt32(sInvoiceId));
                sResult = objCon.get_value(sQry, NpgsqlCommand);
                if (sResult.Length > 0)
                {
                    return true;
                }
                return false;

                //OleDbDataReader dr;
                //dr = objCon.Fetch("SELECT IN_TI_NO FROM TBLDTCINVOICE,TBLTCREPLACE,TBLINDENT WHERE TR_IN_NO=IN_NO AND IN_TI_NO=TI_ID AND IN_NO='" + sInvoiceId + "'");
                //dt.Load(dr);
                //if (dt.Rows.Count > 0)
                //{
                //    return true;

                //}
                //return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
        public string GenerateInvoiceNoSA(string sOfficeCode, string sRoletype, string sRoleId)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IN_INV_NO\" AS INT8),1))+1 from \"TBLDTCINVOICE\" where substr (\"IN_INV_NO\",1,2) =:sOfficeCode", NpgsqlCommand);

                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GenerateInvoiceNoSARSM(string sOfficeCode, string sRoletype, string sRoleId)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(sOfficeCode));
                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"RSM_INV_NO\" AS INT8),1))+1 from \"TBLREPAIRSENTMASTER\" where \"RSM_DIV_CODE\" =:sOfficeCode", NpgsqlCommand);

                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public string GenerateInvoiceNoSAdtcinvoice(string sOfficeCode, string sRoletype, string sRoleId)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IN_INV_NO\" AS INT8),1))+1 from \"TBLDTCINVOICE\" where substr (\"IN_INV_NO\",1,2) =:sOfficeCode", NpgsqlCommand);

                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoleId == Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GenerateInvoiceNo(string sOfficeCode, string sRoletype)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoletype == "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);

                string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IN_INV_NO\" AS INT8),1))+1 from \"TBLDTCINVOICE\" where substr (\"IN_INV_NO\",1,2)=:sOfficeCode", NpgsqlCommand);
                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoletype == "2")
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GenerateInvoiceNoRSM(string sOfficeCode, string sRoletype)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoletype == "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt32(sOfficeCode));

                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);
                string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"RSM_INV_NO\" AS INT8),1))+1 from \"TBLREPAIRSENTMASTER\" where \"RSM_DIV_CODE\" =:sOfficeCode", NpgsqlCommand);
                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoletype == "2")
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public string GenerateInvoiceNointerstore(string sOfficeCode, string sRoletype)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoletype == "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IS_NO\" AS INT8),1))+1 from \"TBLSTOREINVOICE\" where substr (\"IS_NO\",1,2) =:sOfficeCode", NpgsqlCommand);

                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IN_INV_NO\" AS INT8),1))+1 from \"TBLDTCINVOICE\" where substr (\"IN_INV_NO\",1,2)=:sOfficeCode", NpgsqlCommand);
                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoletype == "2")
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }



        public string GenerateInvoiceNodtcinvoice(string sOfficeCode, string sRoletype)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoletype == "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }

                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);
                string sInvoiceNo = objCon.get_value("select max(COALESCE(CAST(\"IN_INV_NO\" AS INT8),1))+1 from \"TBLDTCINVOICE\" where substr (\"IN_INV_NO\",1,2)=:sOfficeCode", NpgsqlCommand);
                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoletype == "2")
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GenerateInvoiceNoforstoreindent(string sOfficeCode, string sRoletype)
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                string sInvoiceNo = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                int stempLength;
                if (sRoletype == "1")
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                    sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                }
                else if (sRoletype == "2")
                {
                    if (sOfficeCode.Length > 2)
                    {
                        sOfficeCode = clsStoreOffice.GetStoreID(sOfficeCode);
                    }
                }
                string offcd = sOfficeCode;
                stempLength = sOfficeCode.Length;
                if (sOfficeCode.Length == 1)
                {
                    sOfficeCode = "0" + sOfficeCode;
                }
                //NpgsqlCommand = new NpgsqlCommand();
                //NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                //string sInvoiceNo = objCon.get_value("SELECT COALESCE(MAX(CAST(\"INV_NO\" AS INT8)),0)+1  FROM \"VIEWINVOICE\" WHERE \"LOCCODE\" =:sOfficeCode", NpgsqlCommand);
                ////string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);
                //string sInvoiceNo = objCon.get_value("SELECT INVOICENUMBER('" + sOfficeCode + "')", NpgsqlCommand);


                NpgsqlCommand = new NpgsqlCommand();
                //NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt64(sOfficeCode));
                //sInvoiceNo = objCon.get_value("select COALESCE (max(cast(\"INV_NO\" as int8)),0)+1 from (SELECT \"TBLSTOREINDENT\".\"SI_NO\" AS \"INV_NO\",CASE WHEN (length((\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text) = 1) THEN ('0'::text || (\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text) ELSE (\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text END AS \"LOCCODE\"  FROM \"TBLSTOREINDENT\" where \"SI_FROM_STORE\"=:sOfficeCode union all SELECT replace((unnest(xpath('(./TBLSTOREINDENT/SI_NO)/text()'::text, (\"TBLWFODATA\".\"WFO_DATA\")::xml)))::text, ''''::text, ''::text) AS \"INV_NO\" ,replace((unnest(xpath('(./TBLSTOREINDENT/SI_FROM_STORE)/text()'::text, (\"TBLWFODATA\".\"WFO_DATA\")::xml)))::text, ''''::text, ''::text) AS \"LOCCODE\"  FROM (\"TBLWFODATA\"   left JOIN \"TBLWORKFLOWOBJECTS\" ON (((\"TBLWFODATA\".\"WFO_ID\")::text = (\"TBLWORKFLOWOBJECTS\".\"WO_WFO_ID\")::text))),   \"TBLSTOREMAST\",\"TBLSTOREINDENT\"  WHERE \"SM_ID\"=\"SI_FROM_STORE\" and \"WO_BO_ID\" = 23 AND \"WO_RECORD_ID\" < 0 AND \"WO_APPROVE_STATUS\" = 0  and \"SM_ID\"=:sOfficeCode)A", NpgsqlCommand);
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", Convert.ToInt64(sOfficeCode));
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode1", Convert.ToString(sOfficeCode));
                NpgsqlCommand.Parameters.AddWithValue("offcd", offcd);
                sInvoiceNo = objCon.get_value("select COALESCE (max(cast(\"INV_NO\" as int8)),0)+1 from (SELECT \"TBLSTOREINDENT\".\"SI_NO\" AS \"INV_NO\",CASE WHEN (length((\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text) = 1) THEN ('0'::text || (\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text) ELSE (\"TBLSTOREINDENT\".\"SI_FROM_STORE\")::text END AS \"LOCCODE\"  FROM \"TBLSTOREINDENT\" where \"SI_FROM_STORE\"=:sOfficeCode union all select  \"INV_NO\",\"LOCCODE\"   from (SELECT replace((unnest(xpath('(./TBLSTOREINDENT/SI_NO)/text()'::text, (\"TBLWFODATA\".\"WFO_DATA\")::xml)))::text, ''''::text, ''::text) AS \"INV_NO\" ,replace((unnest(xpath('(./TBLSTOREINDENT/SI_FROM_STORE)/text()'::text, (\"TBLWFODATA\".\"WFO_DATA\")::xml)))::text, ''''::text, ''::text) AS \"LOCCODE\"  FROM (\"TBLWFODATA\" left JOIN \"TBLWORKFLOWOBJECTS\" ON (((\"TBLWFODATA\".\"WFO_ID\")::text = (\"TBLWORKFLOWOBJECTS\".\"WO_WFO_ID\")::text))) WHERE \"WO_BO_ID\" = 23 AND \"WO_RECORD_ID\" < 0 AND \"WO_APPROVE_STATUS\" = 0 ) a where a. \"LOCCODE\"=:offcd)A", NpgsqlCommand);



                //DataTable dt= new DataTable();
                //string[] Arr = new string[2];
                //NpgsqlCommand cmd = new NpgsqlCommand("invoicenumberforstoreindent");
                //cmd.Parameters.AddWithValue("OFFICECODE", sOfficeCode);
                //dt= objCon.FetchDataTable(cmd);
                //if (dt.Rows.Count > 0)
                //{
                //    sInvoiceNo = Convert.ToString(dt.Rows[0]["OFFICECODE"]);
                //}


                if (stempLength == 1)
                {
                    //sInvoiceNo = "0" + sInvoiceNo;
                    if (sInvoiceNo == "1")
                    {
                        sInvoiceNo = sOfficeCode;
                    }
                    else
                    {
                        sInvoiceNo = "0" + sInvoiceNo;
                    }

                }

                if (sInvoiceNo.Length == 1)
                {

                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                    }
                    if (sRoletype == "2")
                    {
                        //string sOffCode = sOfficeCode;
                        //string sOffCode = clsStoreOffice.GetStoreID(sOfficeCode);
                        if (sOfficeCode.Length == 1)
                        {
                            sOfficeCode = "0" + sOfficeCode;
                        }

                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                    else
                    {
                        sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                    }
                }
                else
                {
                    //2 digit Office Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                                else
                                {
                                    sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                                }
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }
                        else
                        {
                            if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                            {
                                return sInvoiceNo;
                            }
                            else
                            {
                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }
                        }

                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) <= 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy") + "" + System.DateTime.Now.AddYears(1).ToString("yy");
                        }
                        else
                        {
                            sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy") + "" + System.DateTime.Now.ToString("yy");
                        }
                        if (sRoletype == "2")
                        {
                            if (sInvoiceNo.Length > 2)
                            {
                                if (sFinancialYear == sInvoiceNo.Substring(2, 4))
                                {
                                    return sInvoiceNo;
                                }
                            }
                            else
                            {
                                if (sOfficeCode.Length == 1)
                                {
                                    sOfficeCode = "0" + sOfficeCode;
                                }

                                sInvoiceNo = sOfficeCode + sFinancialYear + "00001";
                            }

                            return sInvoiceNo;
                        }


                    }
                }

                return sInvoiceNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }


        #region NewDTC

        public DataTable LoadAllNewDTCInvoice(clsInvoice objInvoice)
        {
            DataTable dt = new DataTable();
            try
            {
                //string strQry = string.Empty;
                //strQry = "SELECT WO_SLNO,WO_NO,TO_CHAR(WO_DATE,'DD-MON-YYYY') WO_DATE,TI_ID,TI_INDENT_NO,TO_CHAR(TI_INDENT_DATE,'DD-MON-YYYY') TI_INDENT_DATE,";
                //strQry += " 'YES' AS STATUS,IN_INV_NO, TO_CHAR(IN_DATE,'DD-MON-YYYY') IN_DATE,IN_NO  FROM TBLWORKORDER,TBLINDENT,TBLDTCINVOICE WHERE WO_SLNO=TI_WO_SLNO AND WO_REPLACE_FLG='0' AND ";
                //strQry += " WO_DF_ID IS NULL AND WO_OFF_CODE LIKE '" + objInvoice.sOfficeCode + "%' AND TI_ID=IN_TI_NO ";
                //strQry += " UNION ALL ";
                //strQry += " SELECT WO_SLNO,WO_NO,TO_CHAR(WO_DATE,'DD-MON-YYYY') WO_DATE,TI_ID,TI_INDENT_NO,TO_CHAR(TI_INDENT_DATE,'DD-MON-YYYY') TI_INDENT_DATE,";
                //strQry += " 'NO' AS STATUS,'' AS IN_INV_NO,'' AS IN_DATE,0 AS IN_NO   FROM TBLWORKORDER,TBLINDENT WHERE WO_SLNO=TI_WO_SLNO AND WO_REPLACE_FLG='0' AND WO_DF_ID IS NULL ";
                //strQry += " AND WO_OFF_CODE LIKE '" + objInvoice.sOfficeCode + "%' AND TI_ID NOT IN (SELECT IN_TI_NO FROM TBLDTCINVOICE)";
                //dt = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadallnewdtcinvoice");
                cmd.Parameters.AddWithValue("sofficecode", objInvoice.sOfficeCode);
                dt = objCon.FetchDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public DataTable LoadAlreadyNewDTCInvoice(clsInvoice objInvoice)
        {
            DataTable dt = new DataTable();
            try
            {
                //string strQry = string.Empty;
                //strQry = "SELECT WO_SLNO,WO_NO,TO_CHAR(WO_DATE,'DD-MON-YYYY') WO_DATE,TI_ID,TI_INDENT_NO,TO_CHAR(TI_INDENT_DATE,'DD-MON-YYYY') TI_INDENT_DATE,";
                //strQry += " 'YES' AS STATUS,IN_INV_NO, TO_CHAR(IN_DATE,'DD-MON-YYYY') IN_DATE,IN_NO  FROM TBLWORKORDER,TBLINDENT,TBLDTCINVOICE WHERE WO_SLNO=TI_WO_SLNO AND WO_REPLACE_FLG='0' AND ";
                //strQry += " WO_DF_ID IS NULL AND WO_OFF_CODE LIKE '" + objInvoice.sOfficeCode + "%' AND TI_ID=IN_TI_NO ";
                //dt = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadalreadynewdtcinvoice");
                cmd.Parameters.AddWithValue("sofficecode", objInvoice.sOfficeCode);
                dt = objCon.FetchDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        #endregion


        public clsInvoice GetGatePassDetials(clsInvoice objInvoice)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                //strQry = "SELECT GP_VEHICLE_NO,GP_RECIEPIENT_NAME,GP_CHALLEN_NO FROM TBLGATEPASS WHERE GP_IN_NO='"+ objInvoice.sInvoiceNo +"'";
                //dt = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getgatepassdetials");
                cmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo);
                dt = objCon.FetchDataTable(cmd);


                if (dt.Rows.Count > 0)
                {
                    objInvoice.sChallenNo = Convert.ToString(dt.Rows[0]["GP_CHALLEN_NO"]);
                    objInvoice.sVehicleNumber = Convert.ToString(dt.Rows[0]["GP_VEHICLE_NO"]);
                    objInvoice.sReceiptientName = Convert.ToString(dt.Rows[0]["GP_RECIEPIENT_NAME"]);
                }

                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }




        public string[] SaveUpdateGatePassDetails(clsInvoice objInvoice)
        {
            string[] Arr = new string[2];
            string[] strArray = new string[2];
            OleDbDataReader dr;
            string strQry = string.Empty;
            try
            {

                if (objInvoice.sTcCode == null || objInvoice.sTcCode == "NULL")
                {
                    objInvoice.sTcCode = "0.0";
                }
                if (objInvoice.sIssueQty == null || objInvoice.sIssueQty == "NULL")
                {
                    objInvoice.sIssueQty = "";
                }

                if (objInvoice.sDTCCode == null || objInvoice.sDTCCode == "NULL")
                {
                    objInvoice.sDTCCode = "";
                }
                NpgsqlCommand = new NpgsqlCommand();
                // coded by Rudra on 12-03-2020 due to no use of Query 

                //NpgsqlCommand.Parameters.AddWithValue("sInvoiceNo1", objInvoice.sInvoiceNo);
                //string res = objCon.get_value("SELECT \"IS_ID\" from \"TBLSTOREINVOICE\" WHERE \"IS_NO\"=:sInvoiceNo1", NpgsqlCommand);

                //if (res == "")
                //{
                //    Arr[0] = "3";
                //    Arr[1] = "Please approve the Invoice...!!!";
                //    return Arr;
                //}

                NpgsqlCommand cmd = new NpgsqlCommand("sp_saveupdategatepassdetails");
                cmd.Parameters.AddWithValue("schallenno", objInvoice.sChallenNo.ToUpper());
                cmd.Parameters.AddWithValue("sgatepassid", objInvoice.sGatePassId);
                cmd.Parameters.AddWithValue("svehiclenumber", objInvoice.sVehicleNumber.ToUpper());
                cmd.Parameters.AddWithValue("sreceiptientname", objInvoice.sReceiptientName.ToUpper());
                cmd.Parameters.AddWithValue("sinvoiceno", objInvoice.sInvoiceNo);
                cmd.Parameters.AddWithValue("stccode", objInvoice.sTcCode);
                cmd.Parameters.AddWithValue("sdtccode", objInvoice.sDTCCode);
                cmd.Parameters.AddWithValue("screatedby", objInvoice.sCreatedBy);
                cmd.Parameters.AddWithValue("sissueqty", objInvoice.sIssueQty == "" ? "0" : objInvoice.sIssueQty);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                strArray[0] = "op_id";
                strArray[1] = "msg";
                Arr = objCon.Execute(cmd, strArray, 2);
                return Arr;


                //dr = objCon.Fetch("select * from TBLGATEPASS where  UPPER(GP_CHALLEN_NO)='" + objInvoice.sChallenNo.ToUpper() + "' and GP_ID='" + objInvoice.sGatePassId + "'");
                //if (dr.Read())
                //{

                //    strQry = "UPDATE TBLGATEPASS SET GP_VEHICLE_NO='" + objInvoice.sVehicleNumber + "',";
                //    strQry += "GP_RECIEPIENT_NAME='" + objInvoice.sReceiptientName.ToUpper() + "',GP_CHALLEN_NO='" + objInvoice.sChallenNo.ToUpper() + "' where GP_ID='" + objInvoice.sGatePassId + "'";
                //    objCon.Execute(strQry);
                //    Arr[0] = "Updated Successfully";
                //    Arr[1] = "1";
                //    return Arr;
                //}
                //else
                //{
                //    if (objInvoice.sGatePassId == "")
                //    {
                //        objInvoice.sGatePassId = Convert.ToString(objCon.Get_max_no("GP_ID", "TBLGATEPASS"));


                //        strQry = "insert into TBLGATEPASS (GP_ID,GP_IN_NO,GP_TC_CODE,GP_DT_CODE,GP_RECIEPIENT_NAME,GP_VEHICLE_NO,GP_CHALLEN_NO,GP_CRBY,GP_CRON) ";
                //        strQry += "values ('" + objInvoice.sGatePassId + "','" + objInvoice.sInvoiceNo + "','" + objInvoice.sTcCode + "','" + objInvoice.sDTCCode + "',";
                //        strQry += " '" + objInvoice.sReceiptientName.ToUpper() + "','" + objInvoice.sVehicleNumber.ToUpper() + "','" + objInvoice.sChallenNo.ToUpper() + "',";
                //        strQry += "'" + objInvoice.sCreatedBy + "',SYSDATE)";
                //        objCon.Execute(strQry);

                //        Arr[0] = "Saved Successfully";
                //        Arr[1] = "0";
                //        return Arr;


                //    }
                //}
                //else
                //{
                //    strQry = "UPDATE TBLGATEPASS SET GP_VEHICLE_NO='" + objInvoice.sVehicleNumber + "',";
                //    strQry += "GP_RECIEPIENT_NAME='" + objInvoice.sReceiptientName.ToUpper() + "',GP_CHALLEN_NO='" + objInvoice.sChallenNo.ToUpper() + "' where GP_ID='" + objInvoice.sGatePassId + "'";
                //    objCon.Execute(strQry);
                //    Arr[0] = "Updated Successfully";
                //    Arr[1] = "1";
                //    return Arr;
                //}
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;


            }
        }

        #region WorkFlow XML

        public clsInvoice GetInvoiceDetailsFromXML(clsInvoice objInvoice)
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                DataTable dtInvoiceDetails = new DataTable();

                dtInvoiceDetails = objApproval.GetDatatableFromXML(objInvoice.sWFDataId);
                if (dtInvoiceDetails.Rows.Count > 0)
                {
                    objInvoice.sInvoiceNo = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_INV_NO"]);
                    objInvoice.sInvoiceDate = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_DATE"]);
                    objInvoice.sAmount = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_AMT"]);
                    objInvoice.sInvoiceDescription = Convert.ToString(dtInvoiceDetails.Rows[0]["IN_DESC"]);

                    objInvoice.sTcCode = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_CODE"]);
                    objInvoice.sTcSlNo = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_SLNO"]);
                    objInvoice.sTcMake = Convert.ToString(dtInvoiceDetails.Rows[0]["TM_NAME"]);
                    objInvoice.sTcCapacity = Convert.ToString(dtInvoiceDetails.Rows[0]["TC_CAPACITY"]);
                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }

        #endregion

    }
}
