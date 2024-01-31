using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Collections;
using IIITS.PGSQL.DAL;
using NpgsqlTypes;
using Npgsql;


namespace IIITS.DTLMS.BL
{
   public class clsWorkOrder
    {
       string strFormCode = "clsWorkOrder";
        //CustOledbConnection ObjCon = new CustOledbConnection(Constants.Password);
        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

        public string sdtccode { get; set; }
        public string sWOId { get; set; }
        public string sFailureId { get; set; }
        public string sFailureDate { get; set; }
        public string sLocationCode { get; set; }
        public string sAccCode { get; set; }
        public string sCrBy { get; set; }
        public string sCommWoNo { get; set; }
        public string sCommDate { get; set; }
        public string sCommAmmount { get; set; }
        public string sDeWoNo { get; set; }
        public string sDeCommDate { get; set; }
        public string sDeCommAmmount { get; set; }
       
        public string sDecomAccCode { get; set; }
        public string sCrAccCode { get; set; }
        public string sIssuedBy { get; set; }
        public string sCapacity { get; set; }
        public string sNewCapacity { get; set; }
        public string sEnhanceAccCode { get; set; }
        public string sEnhancedCapacity { get; set; }

        public string sTaskType { get; set; }
        public string sOfficeCode { get; set; }

        public string sDTCName { get; set; }
        public string sTCSlno { get; set; }
        public string sTCCode { get; set; }
        public string sRequestLoc { get; set; }
        public string sDTCCode { get; set; }
        public string sDTCId { get; set; }
        public string sTCId { get; set; }

        public string sWOFilePath { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFOId { get; set; }
        public string sWFDataId { get; set; }
        public string sActionType { get; set; }
        public string sWFAutoId { get; set; }
        public string sWFObjectId { get; set; }
        public string sApprovalDesc { get; set; }
        public int sDtcScheme { get; set; }
        public string sFailType { get; set; }
        public string sRepairer { get; set; }
        public string sOFCommWoNo { get; set; }
        public string sOFCommDate { get; set; }
        public string sOFCommAmmount { get; set; }
        public string sOFAccCode { get; set; }
        public string sGuarentyType { get; set; }
        public string sCreditWO { get; set; }
        public string sCreditDate { get; set; }
        public string sCreditAmount { get; set; }
        public string sCreditAccCode { get; set; }

        public string sDeCreditWO { get; set; }
        public string sDeCreditDate { get; set; }
        public string sDeCreditAmount { get; set; }
        public string sDeCreditAccCode { get; set; }
        public string sTtkStatus { get; set; }

        public string sboid { get; set; }

        NpgsqlCommand NpgsqlCommand;
        public string sTtkAutoNo { get; set; }
        public string sTtkVendorName { get; set; }
        public string sTtkManual { get; set; }

        public string sRating { get; set; }
        public string sDWAname { get; set; }
        public string sDWAdate { get; set; }

        public string[] SaveUpdateWorkOrder(clsWorkOrder objWorkOrder)
        {
            string strQry = string.Empty;
            string[] Arr = new string[3];

            try
            {
               
                ObjCon.BeginTransaction();
                if (objWorkOrder.sTaskType != "3")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt64(objWorkOrder.sFailureId));
                    //Check Failure ID is exists or not
                    string sId = ObjCon.get_value("SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"=:sFailureId", NpgsqlCommand);
                    if (sId.Length == 0)
                    {
                        Arr[0] = "Enter Valid Failure ID";
                        Arr[1] = "2";
                        return Arr;
                    }
                }

                if (objWorkOrder.sWOId == "")
                {
                    if (objWorkOrder.sTtkStatus != "1")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sCommWoNo", objWorkOrder.sCommWoNo);
                        NpgsqlCommand.Parameters.AddWithValue("sLocationCode", Convert.ToInt32(objWorkOrder.sLocationCode));
                        string sId = ObjCon.get_value("SELECT \"WO_NO\" FROM \"TBLWORKORDER\" WHERE \"WO_NO\"=:sCommWoNo AND \"WO_OFF_CODE\" =:sLocationCode", NpgsqlCommand);
                        if (sId.Length > 0)
                        {
                            Arr[0] = "Work Order No. Already Exists";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }
                    if (objWorkOrder.sActionType != "M")
                    {
                        DataTable dt = new DataTable();
                        NpgsqlCommand cmd = new NpgsqlCommand("sp_check_workorder_alredyexiest");
                        cmd.Parameters.AddWithValue("offCode", objWorkOrder.sLocationCode);
                        cmd.Parameters.AddWithValue("wo_no", objWorkOrder.sCommWoNo);
                        dt = ObjCon.FetchDataTable(cmd);
                        if (dt.Rows.Count > 0)
                        {
                            Arr[0] = "Work Order No. Already Exists";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }
                    //NpgsqlCommand = new NpgsqlCommand();
                    //NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt64(objWorkOrder.sFailureId));
                    ////Check Failure ID is exists or not
                    //string sId1 = ObjCon.get_value("SELECT \"WO_DF_ID\" FROM \"TBLWORKORDER\" WHERE \"WO_DF_ID\"=:sFailureId", NpgsqlCommand);
                    //if (sId1.Length == 0)
                    //{
                    //    Arr[0] = "Failure ID Already Exists";
                    //    Arr[1] = "2";
                    //    return Arr;
                    //}

                    //OleDbDataReader dr = ObjCon.Fetch("SELECT WO_NO FROM TBLWORKORDER WHERE WO_NO='" + objWorkOrder.sCommWoNo + "' AND WO_OFF_CODE='"+ objWorkOrder.sLocationCode + "'");
                    //if (dr.Read())
                    //{
                    //    Arr[0] = "Work Order No. Already Exists";
                    //    Arr[1] = "2";
                    //    dr.Close();
                    //    return Arr;
                    //}
                    //dr.Close();

                    //ObjCon.BeginTrans();
                    // DF_STATUS_FLAG--->1 Failure Entry ;  DF_STATUS_FLAG--->2 Enhancement Entry

                    objWorkOrder.sWOId = Convert.ToString(ObjCon.Get_max_no("WO_SLNO", "TBLWORKORDER"));
                    strQry = "Insert INTO TBLWORKORDER (WO_SLNO,WO_NO,WO_DF_ID,WO_DATE,WO_AMT,WO_NO_DECOM,WO_DATE_DECOM,WO_AMT_DECOM,";
                    strQry += " WO_ACC_CODE,WO_OFF_CODE,WO_CRBY,WO_CRON,WO_ACCCODE_DECOM,WO_ISSUED_BY,WO_DTC_CAP,WO_NEW_CAP,WO_REQUEST_LOC,WO_NO_CREDIT,WO_AMT_CREDIT,WO_ACC_CRERDIT,WO_DATE_CREDIT) VALUES";
                    strQry += "('" + objWorkOrder.sWOId + "','" + objWorkOrder.sCommWoNo.ToUpper() + "','" + objWorkOrder.sFailureId + "',";
                    strQry += " TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                    strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                    strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                    strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "','" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "',";

                    if (objWorkOrder.sDeCreditDate != "0")
                    {
                        strQry += " '" + objWorkOrder.sDeCreditWO.ToUpper() + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ";
                    }
                    else
                    {
                        strQry += " '" + objWorkOrder.sCreditWO.ToUpper() + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ";

                    }
                    if (objWorkOrder.sTtkStatus == "1")
                    {
                        strQry += " '" + objWorkOrder.sTtkStatus + "','" + objWorkOrder.sTtkAutoNo + "','" + objWorkOrder.sTtkVendorName + "','" + objWorkOrder.sTtkManual + "'";
                    }
                    else
                    {
                        strQry += ",null,null,'',null,'','','')";
                    }

                    //ObjCon.Execute(strQry);


                    //SaveWOFilePath(objWorkOrder);

                    //************************ WORK FLOW *************************************



                    //SendSMStoSectionStoreOfficer();

                    //ObjCon.CommitTrans();

                    #region Workflow
                    if (objWorkOrder.sFailureId == "")
                    {
                        strQry = "Insert into \"TBLWORKORDER\" (\"WO_SLNO\",\"WO_NO\",\"WO_DF_ID\",\"WO_DATE\",\"WO_AMT\",\"WO_NO_DECOM\",\"WO_DATE_DECOM\",\"WO_AMT_DECOM\",";
                        strQry += " \"WO_ACC_CODE\",\"WO_OFF_CODE\",\"WO_CRBY\",\"WO_CRON\",\"WO_ACCCODE_DECOM\",\"WO_ISSUED_BY\",\"WO_DTC_CAP\",\"WO_NEW_CAP\",\"WO_REQUEST_LOC\",\"WO_AUTO_NO\",";
                        strQry += " \"WO_NO_OF\",";

                        if (objWorkOrder.sOFCommDate == "0")
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",";
                        }
                        else
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",";
                        }
                        strQry += "  \"WO_NO_CREDIT\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\", \"WO_TTK_STATUS\",\"WO_TTK_AUTO_NO\",\"WO_TTK_VENDOR_NAME\",\"WO_TTK_MANUAL_NO\",\"WO_DWA_NAME\",\"WO_DWA_DATE\",\"WO_RATING\" ) VALUES";

                        strQry += "('{0}','" + objWorkOrder.sCommWoNo.ToUpper() + "',NULL,";
                        strQry += " TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                        strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                        strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "',";
                        strQry += " '" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "','{1}','" + objWorkOrder.sOFCommWoNo + "',";



                        if (objWorkOrder.sOFCommDate == "0")
                        {
                            strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "' , ";
                        }
                        else
                        {
                            strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "' , ";
                        }
                        if (objWorkOrder.sDeCreditDate != "0")
                        {
                            strQry += " '" + objWorkOrder.sDeCreditWO.ToUpper() + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ";
                        }
                        else
                        {
                            strQry += " '" + objWorkOrder.sCreditWO.ToUpper() + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ";

                        }
                        // coded by rudra for new TTk concept
                        if (objWorkOrder.sTtkStatus == "1" || objWorkOrder.sTtkStatus == "0")
                        {
                            strQry += ",'" + objWorkOrder.sTtkStatus + "','" + objWorkOrder.sTtkAutoNo + "','" + objWorkOrder.sTtkVendorName + "','" + objWorkOrder.sTtkManual + "','" + objWorkOrder.sDWAname + "',TO_DATE('" + objWorkOrder.sDWAdate + "','dd/MM/yyyy'),'" + objWorkOrder.sRating + "')";
                        }
                        else
                        {
                            strQry += ",null,null,'',null,'',null,'')";
                        }
                    }
                    else
                    {
                        strQry = "Insert into \"TBLWORKORDER\" (\"WO_SLNO\",\"WO_NO\",\"WO_DF_ID\",\"WO_DATE\",\"WO_AMT\",\"WO_NO_DECOM\",\"WO_DATE_DECOM\",\"WO_AMT_DECOM\",";
                        strQry += " \"WO_ACC_CODE\",\"WO_OFF_CODE\",\"WO_CRBY\",\"WO_CRON\",\"WO_ACCCODE_DECOM\",\"WO_ISSUED_BY\",\"WO_DTC_CAP\",\"WO_NEW_CAP\",\"WO_REQUEST_LOC\",\"WO_AUTO_NO\",";
                        strQry += " \"WO_NO_OF\",\"WO_NO_CREDIT\", ";



                        if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\") VALUES";
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\") VALUES";
                        }

                        else if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\") VALUES";
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\") VALUES";
                        }


                        strQry += "('{0}','" + objWorkOrder.sCommWoNo.ToUpper() + "','" + objWorkOrder.sFailureId + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                        strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                        strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "',";
                        strQry += " '" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "','{1}','" + objWorkOrder.sOFCommWoNo + "',";

                        if (sDeCreditDate != "0")
                        {
                            strQry += "'" + objWorkOrder.sDeCreditWO + "',";
                        }
                        else
                        {
                            strQry += "'" + objWorkOrder.sCreditWO + "',";
                        }

                        if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "' ) ";

                            }
                            else
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "' ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "' ) ";

                            }
                            else
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "' ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ) ";
                            }
                            else
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ) ";
                            }
                            else
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ) ";

                            }
                        }

                    }



                    strQry = strQry.Replace("'", "''");

                    string sParam = "SELECT COALESCE(MAX(\"WO_SLNO\"),0)+1 FROM \"TBLWORKORDER\"";
                    string sParam1 = "SELECT WONUMBER('" + objWorkOrder.sOfficeCode + "')";

                    sParam1 = sParam1.Replace("'", "''");

                    //Workflow / Approval
                    clsApproval objApproval = new clsApproval();
                    if (objWorkOrder.sboid == "74")
                    {

                        objApproval.sFormName = "WorkOrder_sdo";
                        objApproval.sBOId = "74";
                        objApproval.sOfficeCode = objWorkOrder.sOfficeCode;
                        objApproval.sClientIp = objWorkOrder.sClientIP;
                        objApproval.sCrby = objWorkOrder.sCrBy;
                        objApproval.sWFObjectId = objWorkOrder.sWFOId;
                        objApproval.sWFAutoId = objWorkOrder.sWFAutoId;
                        objApproval.sFailType = objWorkOrder.sFailType;
                        objApproval.sQryValues = strQry;
                        objApproval.sParameterValues = sParam + ";" + sParam1;
                        objApproval.sMainTable = "TBLWORKORDER";
                        objApproval.sGuarentyType = objWorkOrder.sGuarentyType;
                        objApproval.sdtccode = objWorkOrder.sdtccode;
                        objApproval.sDataReferenceId = objWorkOrder.sFailureId;
                    }
                    else
                    {


                        objApproval.sFormName = objWorkOrder.sFormName;
                        objApproval.sBOId = "11";
                        //objApproval.sRecordId = objWorkOrder.sWOId;
                        objApproval.sOfficeCode = objWorkOrder.sOfficeCode;
                        objApproval.sClientIp = objWorkOrder.sClientIP;
                        objApproval.sCrby = objWorkOrder.sCrBy;
                        objApproval.sWFObjectId = objWorkOrder.sWFOId;
                        objApproval.sWFAutoId = objWorkOrder.sWFAutoId;
                        objApproval.sFailType = objWorkOrder.sFailType;
                        objApproval.sQryValues = strQry;
                        objApproval.sParameterValues = sParam + ";" + sParam1;
                        objApproval.sMainTable = "TBLWORKORDER";
                        objApproval.sGuarentyType = objWorkOrder.sGuarentyType;
                        objApproval.sdtccode = objWorkOrder.sdtccode;
                        objApproval.sDataReferenceId = objWorkOrder.sFailureId;
                    }

                    //string sDtcCode = ObjCon.get_value("SELECT DF_DTC_CODE FROM TBLDTCFAILURE WHERE DF_ID='" + objWorkOrder.sFailureId + "'");
                    if ((objWorkOrder.sTaskType == "1" || objWorkOrder.sTaskType == "4") && objWorkOrder.sFailType != "1")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sFailureId1", Convert.ToInt64(objWorkOrder.sFailureId));
                        //NpgsqlCommand.Parameters.AddWithValue("sLocationCode", objWorkOrder.sLocationCode);
                        objApproval.sRefOfficeCode = ObjCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" =:sFailureId1", NpgsqlCommand);
                        objApproval.sDescription = "Work Order For Multi Coil Failure Entry of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " + objWorkOrder.sDTCName + " with WO No " + objWorkOrder.sCommWoNo + "";

                    }
                    else if (objWorkOrder.sTaskType == "1" && objWorkOrder.sFailType == "1")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sFailureId2", Convert.ToInt64(objWorkOrder.sFailureId));
                        objApproval.sRefOfficeCode = ObjCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" =:sFailureId2", NpgsqlCommand);
                        objApproval.sDescription = "Work Order For Minor Coil Failure Entry of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " + objWorkOrder.sDTCName + " with WO No " + objWorkOrder.sCommWoNo + "";
                    }
                    else if (objWorkOrder.sTaskType == "2")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        NpgsqlCommand.Parameters.AddWithValue("sFailureId3", Convert.ToInt64(objWorkOrder.sFailureId));
                        objApproval.sRefOfficeCode = ObjCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" =:sFailureId3", NpgsqlCommand);
                        objApproval.sDescription = "Work Order For Capacity Enhancement of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " + objWorkOrder.sDTCName + " with WO No " + objWorkOrder.sCommWoNo + "";
                    }
                    else if (objWorkOrder.sTaskType == "3")
                    {
                        if (objWorkOrder.sTtkStatus == "1")
                        {
                            objApproval.sRefOfficeCode = objWorkOrder.sRequestLoc;
                            objApproval.sDescription = "Work Order For New DTC Commission with TTK FLOW Work Order No: " + objWorkOrder.sCommWoNo;
                        }
                        else
                        {
                            objApproval.sRefOfficeCode = objWorkOrder.sRequestLoc;
                            objApproval.sDescription = "Work Order For New DTC Commission with PTK FLOW Work Order NO : " + objWorkOrder.sCommWoNo;
                        }
                    }


                    string sPrimaryKey = "{0}";


                    objApproval.sColumnNames = "WO_SLNO,WO_NO,WO_DF_ID,WO_DATE,WO_AMT,WO_NO_DECOM,WO_DATE_DECOM,WO_AMT_DECOM,";
                    objApproval.sColumnNames += "WO_ACC_CODE,WO_OFF_CODE,WO_CRBY,WO_CRON,WO_ACCCODE_DECOM,WO_ISSUED_BY,WO_DTC_CAP,";
                    objApproval.sColumnNames += "WO_NEW_CAP,WO_REQUEST_LOC,WO_DTC_SCHEME,WO_REPAIRER,WO_NO_OF,WO_DATE_OF,WO_AMT_OF,WO_ACC_OF,";
                    objApproval.sColumnNames += "WO_NO_CREDIT,WO_AMT_CREDIT,WO_ACC_CRERDIT,WO_DATE_CREDIT,WO_TTK_STATUS,WO_TTK_AUTO_NO,WO_TTK_VENDOR_NAME,WO_TTK_MANUAL_NO,WO_DWA_NAME,WO_DWA_DATE,WO_RATING";

                    objApproval.sColumnValues = "" + sPrimaryKey + "," + objWorkOrder.sCommWoNo.ToUpper() + "," + objWorkOrder.sFailureId + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sCommDate + "," + objWorkOrder.sCommAmmount + ", " + objWorkOrder.sDeWoNo.ToUpper() + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sDeCommDate + "," + objWorkOrder.sDeCommAmmount + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sAccCode + "," + objWorkOrder.sLocationCode + "," + objWorkOrder.sCrBy + ",NOW(),";
                    objApproval.sColumnValues += "" + objWorkOrder.sDecomAccCode + "," + objWorkOrder.sIssuedBy + "," + objWorkOrder.sCapacity + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sNewCapacity + "," + objWorkOrder.sRequestLoc + "," + objWorkOrder.sDtcScheme + "," + objWorkOrder.sRepairer + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sOFCommWoNo + "";

                    if (objWorkOrder.sOFCommDate == "0")
                    {
                        objWorkOrder.sOFCommDate = "";
                        objWorkOrder.sOFCommAmmount = "0";
                        objApproval.sColumnValues += "," + objWorkOrder.sOFCommDate + "," + objWorkOrder.sOFCommAmmount + "," + objWorkOrder.sOFAccCode + ",";
                    }
                    else
                    {
                        objApproval.sColumnValues += "," + objWorkOrder.sOFCommDate + "," + objWorkOrder.sOFCommAmmount + "," + objWorkOrder.sOFAccCode + ",";
                    }

                    if (objWorkOrder.sCreditDate != "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    }
                    else if (objWorkOrder.sDeCreditDate != "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sDeCreditWO + "," + objWorkOrder.sDeCreditAmount + "," + objWorkOrder.sDeCreditAccCode + "," + objWorkOrder.sDeCreditDate + "";
                    }
                    else
                    {
                        objWorkOrder.sCreditDate = "";
                        objWorkOrder.sCreditAmount = "0";
                        objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";

                    }
                    // coded by rudra for new TTk concept
                    if (objWorkOrder.sTtkStatus == "1" || objWorkOrder.sTtkStatus == "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sTtkStatus + "," + objWorkOrder.sTtkAutoNo + "," + objWorkOrder.sTtkVendorName + "," + objWorkOrder.sTtkManual + "," + objWorkOrder.sDWAname + "," + objWorkOrder.sDWAdate + "," + objWorkOrder.sRating + "";
                    }
                    else
                    {
                        objApproval.sColumnValues += ",null,null,'',null,'','',''";
                    }

                    //if (objWorkOrder.sCreditDate == "0")
                    //{
                    //    objWorkOrder.sCreditDate = "";
                    //    objWorkOrder.sCreditAmount = "0";
                    //    objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    //}
                    //else
                    //{
                    //    objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    //}


                    objApproval.sTableNames = "TBLWORKORDER";


                    //Check for Duplicate Approval
                    bool bApproveResult = objApproval.CheckDuplicateApprove(objApproval);
                    if (bApproveResult == false)
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }


                    if (objWorkOrder.sActionType == "M")
                    {
                        objApproval.SaveWorkFlowData(objApproval);
                        objWorkOrder.sWFDataId = objApproval.sWFDataId;
                        objWorkOrder.sApprovalDesc = objApproval.sDescription;

                        Arr[2] = objApproval.sBOId;
                    }
                    else
                    {

                        objApproval.SaveWorkFlowData(objApproval);
                        objWorkOrder.sWFDataId = objApproval.sWFDataId;    //******new code for workorder report
                        objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow();
                        objApproval.SaveWorkflowObjects(objApproval);
                        objWorkOrder.sWFObjectId = objApproval.sWFObjectId; //new code for workorder report
                    }

                    #endregion

                    //string sOfficeCode = ObjCon.get_value("SELECT DF_LOC_CODE FROM TBLDTCFAILURE WHERE DF_ID='"+ objWorkOrder.sFailureId +"'");
                    //SendSMStoSectionOfficer(sOfficeCode, objWorkOrder.sDTCCode, objWorkOrder.sCommWoNo,objWorkOrder.sDTCName);

                    Arr[0] = "Work Order Created Successfully";
                    Arr[1] = "0";
                  
                    ObjCon.CommitTransaction();
                    return Arr;

                }

                else
                {
                    //ObjCon.BeginTrans();


                    //OleDbDataReader dr = ObjCon.Fetch("SELECT WO_NO FROM TBLWORKORDER WHERE WO_NO='" + objWorkOrder.sCommWoNo + "' AND WO_SLNO<> '" + objWorkOrder.sWOId + "' AND WO_OFF_CODE='"+ objWorkOrder.sLocationCode + "'");
                    //if (dr.Read())
                    //{
                    //    Arr[0] = "Work Order No. Already Exists";
                    //    Arr[1] = "2";
                    //    dr.Close();
                    //    return Arr;
                    //}
                    //dr.Close();

                    //strQry = "UPDATE TBLWORKORDER SET WO_NO ='" + objWorkOrder.sCommWoNo.ToUpper() + "',";
                    //strQry += " WO_DF_ID= '" + objWorkOrder.sFailureId + "',";
                    //strQry += " WO_OFF_CODE= '" + objWorkOrder.sLocationCode + "',";
                    //strQry += " WO_ACC_CODE= '" + objWorkOrder.sAccCode + "',WO_DATE= TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),WO_AMT= '" + objWorkOrder.sCommAmmount + "',WO_NO_DECOM= '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                    //strQry += " WO_DATE_DECOM= TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),WO_AMT_DECOM='" + objWorkOrder.sDeCommAmmount + "',";
                    //strQry += " WO_ACCCODE_DECOM='" + objWorkOrder.sDecomAccCode + "',WO_ISSUED_BY='" + objWorkOrder.sIssuedBy + "' ,";
                    //strQry += "  WO_DTC_CAP='" + objWorkOrder.sCapacity + "',WO_NEW_CAP='" + objWorkOrder.sNewCapacity  + "' WHERE WO_SLNO= '" + objWorkOrder.sWOId + "'";

                    //ObjCon.Execute(strQry);

                    ////SaveWOFilePath(objWorkOrder);

                    //ObjCon.CommitTrans();
                    //Arr[0] = "Work Order Details Updated Successfully";
                    //Arr[1] = "1";
                    //return Arr;
                    ObjCon.BeginTransaction();

                    string[] strResult = new string[2];
                    string[] strArray = new string[2];
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_updateworkorder");


                    cmd.Parameters.AddWithValue("sfailureid", Convert.ToString(objWorkOrder.sFailureId));
                    cmd.Parameters.AddWithValue("scommwono", Convert.ToString(objWorkOrder.sCommWoNo));
                    cmd.Parameters.AddWithValue("slocationcode", Convert.ToString(objWorkOrder.sLocationCode));
                    cmd.Parameters.AddWithValue("swoid", Convert.ToString(objWorkOrder.sWOId));
                    cmd.Parameters.AddWithValue("scommdate", Convert.ToString(objWorkOrder.sCommDate));
                    cmd.Parameters.AddWithValue("scommammount", Convert.ToString(objWorkOrder.sCommAmmount));
                    cmd.Parameters.AddWithValue("sdewono", Convert.ToString(objWorkOrder.sDeWoNo));
                    cmd.Parameters.AddWithValue("sdecommdate", Convert.ToString(objWorkOrder.sDeCommDate));
                    cmd.Parameters.AddWithValue("sdecommammount", Convert.ToString(objWorkOrder.sDeCommAmmount));
                    cmd.Parameters.AddWithValue("sacccode", Convert.ToString(objWorkOrder.sAccCode));
                    cmd.Parameters.AddWithValue("scrby", Convert.ToString(objWorkOrder.sCrBy));
                    cmd.Parameters.AddWithValue("sdecomacccode", Convert.ToString(objWorkOrder.sDecomAccCode));
                    cmd.Parameters.AddWithValue("sissuedby", Convert.ToString(objWorkOrder.sIssuedBy));
                    cmd.Parameters.AddWithValue("scapacity", Convert.ToString(objWorkOrder.sCapacity));
                    cmd.Parameters.AddWithValue("snewcapacity", Convert.ToString(objWorkOrder.sNewCapacity));
                    cmd.Parameters.AddWithValue("srequestloc", Convert.ToString(objWorkOrder.sRequestLoc));
                    if (objWorkOrder.sDeCreditDate != "0")
                    {
                        cmd.Parameters.AddWithValue("sdecreditwO", Convert.ToString(objWorkOrder.sDeCreditWO));
                        cmd.Parameters.AddWithValue("sdecreditamount", Convert.ToString(objWorkOrder.sDeCreditAmount));
                        cmd.Parameters.AddWithValue("sdecreditaccCode", Convert.ToString(objWorkOrder.sDeCreditAccCode));
                        cmd.Parameters.AddWithValue("sdecreditdate", Convert.ToString(objWorkOrder.sDeCreditDate));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("sdecreditwO", Convert.ToString(objWorkOrder.sCreditWO));
                        cmd.Parameters.AddWithValue("sdecreditamount", Convert.ToString(objWorkOrder.sCreditAmount));
                        cmd.Parameters.AddWithValue("sdecreditaccCode", Convert.ToString(objWorkOrder.sCreditAccCode));
                        cmd.Parameters.AddWithValue("sdecreditdate", Convert.ToString(objWorkOrder.sCreditDate));

                    }
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = ObjCon.Execute(cmd, strArray, 2);
                    ObjCon.CommitTransaction();
                    return strResult;
                }
            }
            catch (Exception ex)
            {
                // ObjCon.RollBack();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }

        }

        public string[] SaveUpdateWorkOrder1(clsWorkOrder objWorkOrder)
        {
            string strQry = string.Empty;
            string[] Arr = new string[3];
            DataBase.DataBseConnection objDatabse = new DataBase.DataBseConnection(Constants.Password);

            try
            {
                objDatabse.BeginTransaction();
               
                if (objWorkOrder.sTaskType != "3")
                {
                    //Check Failure ID is exists or not
                    string sId = objDatabse.get_value("SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"="+ Convert.ToInt64(objWorkOrder.sFailureId) + "");
                    if(sId.Length == 0)
                    {
                        Arr[0] = "Enter Valid Failure ID";
                        Arr[1] = "2";
                        return Arr;
                    }
                }

                if (objWorkOrder.sWOId == "")
                {
                    if (objWorkOrder.sTtkStatus != "1")
                    {
                        string sId = objDatabse.get_value("SELECT \"WO_NO\" FROM \"TBLWORKORDER\" WHERE \"WO_NO\"='"+ objWorkOrder.sCommWoNo + "' AND \"WO_OFF_CODE\" ="+ Convert.ToInt32(objWorkOrder.sLocationCode) + "");
                        if (sId.Length > 0)
                        {
                            Arr[0] = "Work Order No. Already Exists";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }
                    if (objWorkOrder.sActionType != "M")
                    {
                        DataTable dt = new DataTable();
                        NpgsqlCommand cmd = new NpgsqlCommand("sp_check_workorder_alredyexiest");
                        cmd.Parameters.AddWithValue("offCode", objWorkOrder.sLocationCode);
                        cmd.Parameters.AddWithValue("wo_no", objWorkOrder.sCommWoNo);
                        dt = objDatabse.FetchDataTable(cmd);
                        if (dt.Rows.Count > 0)
                        {
                            Arr[0] = "Work Order No. Already Exists";
                            Arr[1] = "2";
                            return Arr;
                        }
                    }

                    //string sParam = Convert.ToString(System.DateTime.Now.ToString("yyMMddHHmmssff"));
                    //string sParam1 = "SELECT WONUMBER('" + objWorkOrder.sOfficeCode + "')";

                    //sParam1 = sParam1.Replace("'", "''");
                    objWorkOrder.sWOId = Convert.ToString(objDatabse.Get_max_no("WO_SLNO", "TBLWORKORDER"));
                   //  objWorkOrder.sWOId = sParam;
                    strQry = "Insert INTO TBLWORKORDER (WO_SLNO,WO_NO,WO_DF_ID,WO_DATE,WO_AMT,WO_NO_DECOM,WO_DATE_DECOM,WO_AMT_DECOM,";
                    strQry += " WO_ACC_CODE,WO_OFF_CODE,WO_CRBY,WO_CRON,WO_ACCCODE_DECOM,WO_ISSUED_BY,WO_DTC_CAP,WO_NEW_CAP,WO_REQUEST_LOC,WO_NO_CREDIT,WO_AMT_CREDIT,WO_ACC_CRERDIT,WO_DATE_CREDIT) VALUES";
                    strQry += "('" + objWorkOrder.sWOId + "','" + objWorkOrder.sCommWoNo.ToUpper() + "','" + objWorkOrder.sFailureId + "',";
                    strQry+=" TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                    strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                    strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                    strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "','" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "',";

                    if (objWorkOrder.sDeCreditDate != "0")
                    {
                        strQry += " '" + objWorkOrder.sDeCreditWO.ToUpper() + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ";
                    }
                    else
                    {
                        strQry += " '" + objWorkOrder.sCreditWO.ToUpper() + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ";

                    }
                    if (objWorkOrder.sTtkStatus == "1")
                    {
                        strQry += " '" + objWorkOrder.sTtkStatus + "','" + objWorkOrder.sTtkAutoNo + "','" + objWorkOrder.sTtkVendorName + "','" + objWorkOrder.sTtkManual + "'";
                    }
                    else
                    {
                        strQry += ",null,null,'',null,'','','')";
                    }


                    #region Workflow
                    if (objWorkOrder.sFailureId == "")
                    {
                        strQry = "Insert into \"TBLWORKORDER\" (\"WO_SLNO\",\"WO_NO\",\"WO_DF_ID\",\"WO_DATE\",\"WO_AMT\",\"WO_NO_DECOM\",\"WO_DATE_DECOM\",\"WO_AMT_DECOM\",";
                        strQry += " \"WO_ACC_CODE\",\"WO_OFF_CODE\",\"WO_CRBY\",\"WO_CRON\",\"WO_ACCCODE_DECOM\",\"WO_ISSUED_BY\",\"WO_DTC_CAP\",\"WO_NEW_CAP\",\"WO_REQUEST_LOC\",\"WO_AUTO_NO\",";
                        strQry += " \"WO_NO_OF\",";
                            
                        if(objWorkOrder.sOFCommDate == "0")
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",";
                        }
                        else
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",";
                        }
                        strQry += "  \"WO_NO_CREDIT\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\", \"WO_TTK_STATUS\",\"WO_TTK_AUTO_NO\",\"WO_TTK_VENDOR_NAME\",\"WO_TTK_MANUAL_NO\",\"WO_DWA_NAME\",\"WO_DWA_DATE\",\"WO_RATING\" ) VALUES";
                            
                        strQry += "('{0}','" + objWorkOrder.sCommWoNo.ToUpper() + "',NULL,";
                        strQry += " TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                        strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                        strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "',";
                        strQry += " '" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "','{1}','" + objWorkOrder.sOFCommWoNo + "',";
                      


                        if (objWorkOrder.sOFCommDate == "0")
                        {
                            strQry += " '"+ objWorkOrder.sOFCommAmmount + "','"+ objWorkOrder.sOFAccCode +"' , ";
                        }
                        else
                        {
                            strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "' , ";
                        }
                        if (objWorkOrder.sDeCreditDate != "0")
                        {
                            strQry += " '" + objWorkOrder.sDeCreditWO.ToUpper() + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ";
                        }
                        else
                        {
                            strQry += " '" + objWorkOrder.sCreditWO.ToUpper() + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ";

                        }
                        // coded by rudra for new TTk concept
                        if (objWorkOrder.sTtkStatus == "1" || objWorkOrder.sTtkStatus == "0")
                        {
                            strQry += ",'" + objWorkOrder.sTtkStatus + "','" + objWorkOrder.sTtkAutoNo + "','" + objWorkOrder.sTtkVendorName + "','" + objWorkOrder.sTtkManual + "','" + objWorkOrder.sDWAname + "',TO_DATE('" + objWorkOrder.sDWAdate + "','dd/MM/yyyy'),'" + objWorkOrder.sRating + "')";
                        }
                        else
                        {
                            strQry += ",null,null,'',null,'',null,'')";
                        }
                    }
                    else
                    {
                        strQry = "Insert into \"TBLWORKORDER\" (\"WO_SLNO\",\"WO_NO\",\"WO_DF_ID\",\"WO_DATE\",\"WO_AMT\",\"WO_NO_DECOM\",\"WO_DATE_DECOM\",\"WO_AMT_DECOM\",";
                        strQry += " \"WO_ACC_CODE\",\"WO_OFF_CODE\",\"WO_CRBY\",\"WO_CRON\",\"WO_ACCCODE_DECOM\",\"WO_ISSUED_BY\",\"WO_DTC_CAP\",\"WO_NEW_CAP\",\"WO_REQUEST_LOC\",\"WO_AUTO_NO\",";
                        strQry += " \"WO_NO_OF\",\"WO_NO_CREDIT\", ";



                        if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate=="0"))
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\") VALUES";
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\") VALUES";
                        }

                        else if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate != "0"|| objWorkOrder.sDeCreditDate != "0"))
                        {
                            strQry += " \"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\") VALUES";
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            strQry += " \"WO_DATE_OF\",\"WO_AMT_OF\",\"WO_ACC_OF\",\"WO_AMT_CREDIT\",\"WO_ACC_CRERDIT\",\"WO_DATE_CREDIT\") VALUES";
                        }


                        strQry += "('{0}','" + objWorkOrder.sCommWoNo.ToUpper() + "','" + objWorkOrder.sFailureId + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sCommAmmount + "', '" + objWorkOrder.sDeWoNo.ToUpper() + "',";
                        strQry += " TO_DATE('" + objWorkOrder.sDeCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sDeCommAmmount + "',";
                        strQry += " '" + objWorkOrder.sAccCode + "','" + objWorkOrder.sLocationCode + "','" + objWorkOrder.sCrBy + "',NOW(),";
                        strQry += " '" + objWorkOrder.sDecomAccCode + "','" + objWorkOrder.sIssuedBy + "','" + objWorkOrder.sCapacity + "',";
                        strQry += " '" + objWorkOrder.sNewCapacity + "','" + objWorkOrder.sRequestLoc + "','{1}','" + objWorkOrder.sOFCommWoNo + "',";

                        if (sDeCreditDate != "0")
                        {
                            strQry += "'" + objWorkOrder.sDeCreditWO + "',";
                        }
                        else
                        {
                            strQry += "'" + objWorkOrder.sCreditWO + "',";
                        }

                        if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            if(objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "' ) ";

                            }
                            else
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "' ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate == "0" || objWorkOrder.sDeCreditDate == "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "' ) ";

                            }
                            else
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "' ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate == "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ) ";
                            }
                            else
                            {
                                strQry += " '" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ) ";

                            }
                        }
                        else if (objWorkOrder.sOFCommDate != "0" && (objWorkOrder.sCreditDate != "0" || objWorkOrder.sDeCreditDate != "0"))
                        {
                            if (objWorkOrder.sDeCreditDate != "0")
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sDeCreditAmount + "','" + objWorkOrder.sDeCreditAccCode + "',TO_DATE('" + objWorkOrder.sDeCreditDate + "','dd/MM/yyyy') ) ";
                            }
                            else
                            {
                                strQry += " TO_DATE('" + objWorkOrder.sOFCommDate + "','dd/MM/yyyy'),'" + objWorkOrder.sOFCommAmmount + "','" + objWorkOrder.sOFAccCode + "','" + objWorkOrder.sCreditAmount + "','" + objWorkOrder.sCreditAccCode + "',TO_DATE('" + objWorkOrder.sCreditDate + "','dd/MM/yyyy') ) ";

                            }
                        }

                    }

                    

                    strQry = strQry.Replace("'", "''");

                    string sParam = "SELECT COALESCE(MAX(\"WO_SLNO\"),0)+1 FROM \"TBLWORKORDER\"";
                    string sParam1 = "SELECT WONUMBER('" + objWorkOrder.sOfficeCode + "')";

                    sParam1 = sParam1.Replace("'", "''");
                    //Workflow / Approval
                    clsApproval objApproval = new clsApproval();
                    if (objWorkOrder.sboid == "74")
                    {

                        objApproval.sFormName = "WorkOrder_sdo";
                        objApproval.sBOId = "74";
                        objApproval.sOfficeCode = objWorkOrder.sOfficeCode;
                        objApproval.sClientIp = objWorkOrder.sClientIP;
                        objApproval.sCrby = objWorkOrder.sCrBy;
                        objApproval.sWFObjectId = objWorkOrder.sWFOId;
                        objApproval.sWFAutoId = objWorkOrder.sWFAutoId;
                        objApproval.sFailType = objWorkOrder.sFailType;
                        objApproval.sQryValues = strQry;
                        objApproval.sParameterValues = sParam + ";" + sParam1;
                        objApproval.sMainTable = "TBLWORKORDER";
                        objApproval.sGuarentyType = objWorkOrder.sGuarentyType;
                        objApproval.sdtccode = objWorkOrder.sdtccode;
                        objApproval.sDataReferenceId = objWorkOrder.sFailureId;
                    }
                    else
                    {


                        objApproval.sFormName = objWorkOrder.sFormName;
                        objApproval.sBOId = "11";
                        //objApproval.sRecordId = objWorkOrder.sWOId;
                        objApproval.sOfficeCode = objWorkOrder.sOfficeCode;
                        objApproval.sClientIp = objWorkOrder.sClientIP;
                        objApproval.sCrby = objWorkOrder.sCrBy;
                        objApproval.sWFObjectId = objWorkOrder.sWFOId;
                        objApproval.sWFAutoId = objWorkOrder.sWFAutoId;
                        objApproval.sFailType = objWorkOrder.sFailType;
                        objApproval.sQryValues = strQry;
                        objApproval.sParameterValues = sParam + ";" + sParam1;
                        objApproval.sMainTable = "TBLWORKORDER";
                        objApproval.sGuarentyType = objWorkOrder.sGuarentyType;
                        objApproval.sdtccode = objWorkOrder.sdtccode;
                        objApproval.sDataReferenceId = objWorkOrder.sFailureId;
                    }

                    //string sDtcCode = ObjCon.get_value("SELECT DF_DTC_CODE FROM TBLDTCFAILURE WHERE DF_ID='" + objWorkOrder.sFailureId + "'");
                    if ((objWorkOrder.sTaskType == "1" || objWorkOrder.sTaskType == "4") && objWorkOrder.sFailType != "1")
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sFailureId1", Convert.ToInt64(objWorkOrder.sFailureId));
                        //NpgsqlCommand.Parameters.AddWithValue("sLocationCode", objWorkOrder.sLocationCode);
                        objApproval.sRefOfficeCode = objDatabse.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ="+ Convert.ToInt64(objWorkOrder.sFailureId) + "");
                        objApproval.sDescription = "Work Order For Multi Coil Failure Entry of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " +objWorkOrder.sDTCName + " with WO No "+ objWorkOrder.sCommWoNo +"";

                    }
                    else if(objWorkOrder.sTaskType == "1" && objWorkOrder.sFailType == "1")
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sFailureId2", Convert.ToInt64(objWorkOrder.sFailureId));
                        objApproval.sRefOfficeCode = objDatabse.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ="+ Convert.ToInt64(objWorkOrder.sFailureId) + "");
                        objApproval.sDescription = "Work Order For Minor Coil Failure Entry of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " + objWorkOrder.sDTCName + " with WO No " + objWorkOrder.sCommWoNo + "";
                    }
                    else if (objWorkOrder.sTaskType == "2")
                    {
                        //NpgsqlCommand = new NpgsqlCommand();
                        //NpgsqlCommand.Parameters.AddWithValue("sFailureId3", Convert.ToInt64(objWorkOrder.sFailureId));
                        objApproval.sRefOfficeCode = objDatabse.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" ="+ Convert.ToInt64(objWorkOrder.sFailureId) + "");
                        objApproval.sDescription = "Work Order For Capacity Enhancement of DTC Code " + objWorkOrder.sDTCCode + " and DTC Name " + objWorkOrder.sDTCName + " with WO No " + objWorkOrder.sCommWoNo + "";
                    }
                    else if (objWorkOrder.sTaskType == "3")
                    {
                        if (objWorkOrder.sTtkStatus == "1")
                        {
                            objApproval.sRefOfficeCode = objWorkOrder.sRequestLoc;
                            objApproval.sDescription = "Work Order For New DTC Commission with TTK FLOW Work Order No: " + objWorkOrder.sCommWoNo;
                        }
                        else
                        {
                            objApproval.sRefOfficeCode = objWorkOrder.sRequestLoc;
                            objApproval.sDescription = "Work Order For New DTC Commission with PTK FLOW Work Order NO : " + objWorkOrder.sCommWoNo;
                        }
                    }


                    string sPrimaryKey = "{0}";


                    objApproval.sColumnNames = "WO_SLNO,WO_NO,WO_DF_ID,WO_DATE,WO_AMT,WO_NO_DECOM,WO_DATE_DECOM,WO_AMT_DECOM,";
                    objApproval.sColumnNames += "WO_ACC_CODE,WO_OFF_CODE,WO_CRBY,WO_CRON,WO_ACCCODE_DECOM,WO_ISSUED_BY,WO_DTC_CAP,";
                    objApproval.sColumnNames += "WO_NEW_CAP,WO_REQUEST_LOC,WO_DTC_SCHEME,WO_REPAIRER,WO_NO_OF,WO_DATE_OF,WO_AMT_OF,WO_ACC_OF,";
                    objApproval.sColumnNames += "WO_NO_CREDIT,WO_AMT_CREDIT,WO_ACC_CRERDIT,WO_DATE_CREDIT,WO_TTK_STATUS,WO_TTK_AUTO_NO,WO_TTK_VENDOR_NAME,WO_TTK_MANUAL_NO,WO_DWA_NAME,WO_DWA_DATE,WO_RATING";

                    objApproval.sColumnValues = "" + sPrimaryKey + "," + objWorkOrder.sCommWoNo.ToUpper() + "," + objWorkOrder.sFailureId + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sCommDate + "," + objWorkOrder.sCommAmmount + ", " + objWorkOrder.sDeWoNo.ToUpper() + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sDeCommDate + "," + objWorkOrder.sDeCommAmmount + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sAccCode + "," + objWorkOrder.sLocationCode + "," + objWorkOrder.sCrBy + ",NOW(),";
                    objApproval.sColumnValues += "" + objWorkOrder.sDecomAccCode + "," + objWorkOrder.sIssuedBy + "," + objWorkOrder.sCapacity + ",";
                    objApproval.sColumnValues += "" + objWorkOrder.sNewCapacity + "," + objWorkOrder.sRequestLoc + ","+ objWorkOrder.sDtcScheme +","+objWorkOrder.sRepairer+",";
                    objApproval.sColumnValues += "" + objWorkOrder.sOFCommWoNo + "";

                    if (objWorkOrder.sOFCommDate == "0")
                    {
                        objWorkOrder.sOFCommDate = "";
                        objWorkOrder.sOFCommAmmount = "0";
                        objApproval.sColumnValues += ","+ objWorkOrder.sOFCommDate + ","+ objWorkOrder.sOFCommAmmount + "," + objWorkOrder.sOFAccCode + ",";
                    }
                    else
                    {
                        objApproval.sColumnValues += "," + objWorkOrder.sOFCommDate + "," + objWorkOrder.sOFCommAmmount + "," + objWorkOrder.sOFAccCode + ",";
                    }

                    if (objWorkOrder.sCreditDate != "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    }
                    else if (objWorkOrder.sDeCreditDate != "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sDeCreditWO + "," + objWorkOrder.sDeCreditAmount + "," + objWorkOrder.sDeCreditAccCode + "," + objWorkOrder.sDeCreditDate + "";
                    }
                    else
                    {
                        objWorkOrder.sCreditDate = "";
                           objWorkOrder.sCreditAmount = "0";
                          objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";

                    }
                    // coded by rudra for new TTk concept
                    if (objWorkOrder.sTtkStatus == "1" || objWorkOrder.sTtkStatus == "0")
                    {
                        objApproval.sColumnValues += "" + objWorkOrder.sTtkStatus + "," + objWorkOrder.sTtkAutoNo + "," + objWorkOrder.sTtkVendorName + "," + objWorkOrder.sTtkManual + "," + objWorkOrder.sDWAname + "," + objWorkOrder.sDWAdate + "," + objWorkOrder.sRating + "";
                    }
                    else
                    {
                        objApproval.sColumnValues += ",null,null,'',null,'','',''";
                    }

                    //if (objWorkOrder.sCreditDate == "0")
                    //{
                    //    objWorkOrder.sCreditDate = "";
                    //    objWorkOrder.sCreditAmount = "0";
                    //    objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    //}
                    //else
                    //{
                    //    objApproval.sColumnValues += "," + objWorkOrder.sCreditWO + "," + objWorkOrder.sCreditAmount + "," + objWorkOrder.sCreditAccCode + "," + objWorkOrder.sCreditDate + "";
                    //}


                    objApproval.sTableNames = "TBLWORKORDER";


                    //Check for Duplicate Approval
                    bool bApproveResult = objApproval.CheckDuplicateApprove1(objApproval, objDatabse);
                    if (bApproveResult == false)
                    {
                        Arr[0] = "Selected Record Already Approved";
                        Arr[1] = "2";
                        return Arr;
                    }


                    if (objWorkOrder.sActionType == "M")
                    {
                        objApproval.SaveWorkFlowData1(objApproval,objDatabse);
                        objWorkOrder.sWFDataId = objApproval.sWFDataId;
                        objWorkOrder.sApprovalDesc = objApproval.sDescription;

                        Arr[2] = objApproval.sBOId;
                    }
                    else
                    {
                       
                        objApproval.SaveWorkFlowData1(objApproval,objDatabse);
                        objWorkOrder.sWFDataId = objApproval.sWFDataId;    //******new code for workorder report
                        objApproval.sRecordId = objApproval.GetRecordIdForWorkFlow1(objDatabse);
                        objApproval.SaveWorkflowObjects1(objApproval,objDatabse);
                        objWorkOrder.sWFObjectId = objApproval.sWFObjectId; //new code for workorder report
                    }

                    #endregion

  
                    Arr[0] = "Work Order Created Successfully";
                    Arr[1] = "0";
                    objDatabse.CommitTransaction();
                  
                    return Arr;

                }

                else
                {
    
                    ObjCon.BeginTransaction();

                    string[] strResult = new string[2];
                    string[] strArray = new string[2];
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_updateworkorder");


                    cmd.Parameters.AddWithValue("sfailureid", Convert.ToString(objWorkOrder.sFailureId));
                    cmd.Parameters.AddWithValue("scommwono", Convert.ToString(objWorkOrder.sCommWoNo));
                    cmd.Parameters.AddWithValue("slocationcode", Convert.ToString(objWorkOrder.sLocationCode));
                    cmd.Parameters.AddWithValue("swoid", Convert.ToString(objWorkOrder.sWOId));
                    cmd.Parameters.AddWithValue("scommdate", Convert.ToString(objWorkOrder.sCommDate));
                    cmd.Parameters.AddWithValue("scommammount", Convert.ToString(objWorkOrder.sCommAmmount));
                    cmd.Parameters.AddWithValue("sdewono", Convert.ToString(objWorkOrder.sDeWoNo));
                    cmd.Parameters.AddWithValue("sdecommdate", Convert.ToString(objWorkOrder.sDeCommDate));
                    cmd.Parameters.AddWithValue("sdecommammount", Convert.ToString(objWorkOrder.sDeCommAmmount));
                    cmd.Parameters.AddWithValue("sacccode", Convert.ToString(objWorkOrder.sAccCode));
                    cmd.Parameters.AddWithValue("scrby", Convert.ToString(objWorkOrder.sCrBy));
                    cmd.Parameters.AddWithValue("sdecomacccode", Convert.ToString(objWorkOrder.sDecomAccCode));
                    cmd.Parameters.AddWithValue("sissuedby", Convert.ToString(objWorkOrder.sIssuedBy));
                    cmd.Parameters.AddWithValue("scapacity", Convert.ToString(objWorkOrder.sCapacity));
                    cmd.Parameters.AddWithValue("snewcapacity", Convert.ToString(objWorkOrder.sNewCapacity));
                    cmd.Parameters.AddWithValue("srequestloc", Convert.ToString(objWorkOrder.sRequestLoc));

                    if (objWorkOrder.sDeCreditDate!="0")
                    {
                        cmd.Parameters.AddWithValue("sdecreditwO", Convert.ToString(objWorkOrder.sDeCreditWO));
                        cmd.Parameters.AddWithValue("sdecreditamount", Convert.ToString(objWorkOrder.sDeCreditAmount));
                        cmd.Parameters.AddWithValue("sdecreditaccCode", Convert.ToString(objWorkOrder.sDeCreditAccCode));
                        cmd.Parameters.AddWithValue("sdecreditdate", Convert.ToString(objWorkOrder.sDeCreditDate));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("sdecreditwO", Convert.ToString(objWorkOrder.sCreditWO));
                        cmd.Parameters.AddWithValue("sdecreditamount", Convert.ToString(objWorkOrder.sCreditAmount));
                        cmd.Parameters.AddWithValue("sdecreditaccCode", Convert.ToString(objWorkOrder.sCreditAccCode));
                        cmd.Parameters.AddWithValue("sdecreditdate", Convert.ToString(objWorkOrder.sCreditDate));

                    }
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    strArray[0] = "op_id";
                    strArray[1] = "msg";
                    strResult = ObjCon.Execute(cmd, strArray, 2);
                    ObjCon.CommitTransaction();
                    return strResult;     
                }
            }
            catch (Exception ex)
            {
                // ObjCon.RollBack();
                
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
                throw ex;
                //return Arr;
            }

        }
        public string GenerateWOautoNo(string sOfficeCode)
        {
            try
            {
                PGSqlConnection objCon = new PGSQL.DAL.PGSqlConnection(Constants.Password);
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sOfficeCode", sOfficeCode);
                string sWOAutoNo = objCon.get_value("SELECT  COALESCE(MAX(CAST(\"WO_TTK_AUTO_NO\" AS INT8)),0)+1 FROM \"VIEW_WORKORDER\" WHERE CAST(\"WO_TTK_AUTO_NO\" AS TEXT) LIKE :sOfficeCode||'%' ", NpgsqlCommand);
                //System.IO.File.AppendAllText("D:\\ERRORLOG\\indent.txt", sIndentNo);
                if (sWOAutoNo.Length == 1)
                {
                    //4 digit Section Code
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        sFinancialYear = System.DateTime.Now.ToString("yy");
                    }
                    else
                    {
                        sFinancialYear = System.DateTime.Now.AddYears(-1).ToString("yy");
                    }

                    sWOAutoNo = sOfficeCode + sFinancialYear + "001";
                }
                else
                {
                    //4 digit Section Code 13111802336
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                    {
                        if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) > 03)
                        {
                            sFinancialYear = System.DateTime.Now.ToString("yy");
                        }
                        if (sFinancialYear == sWOAutoNo.Substring(5, 2))
                        {
                            return sWOAutoNo;
                        }
                        else
                        {
                            sWOAutoNo = sOfficeCode + sFinancialYear + "001";
                        }
                    }
                    if (Convert.ToInt32(System.DateTime.Now.ToString("MM")) < 03)
                    {
                        return sWOAutoNo;
                    }


                }

                return sWOAutoNo;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public bool SaveWOFilePath(clsWorkOrder objWO)
        {
            try
            {

                //FTP Parameter
                string sFTPLink = string.Empty;
                string sFTPUserName = string.Empty;
                string sFTPPassword = string.Empty;
                string sFolderName = "WORKORDER";
                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);

                string sWOFileName = string.Empty;
                string sDirectory = string.Empty;

                //  Photo Save DTLMSDocs

                clsCommon objComm = new clsCommon();
                DataTable dt = objComm.GetAppSettings();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToString(dt.Rows[i]["AP_KEY"]) == "FTPMAINLINK")
                    {
                        sFTPLink = Convert.ToString(dt.Rows[i]["AP_VALUE"]);
                    }
                    else if (Convert.ToString(dt.Rows[i]["AP_KEY"]) == "FTPUSERNAME")
                    {
                        sFTPUserName = Convert.ToString(dt.Rows[i]["AP_VALUE"]);
                    }
                    else if (Convert.ToString(dt.Rows[i]["AP_KEY"]) == "FTPPASSWORD")
                    {
                        sFTPPassword = Convert.ToString(dt.Rows[i]["AP_VALUE"]);
                    }
                }

                //clsFtp objFtp = new clsFtp(sFTPLink, sFTPUserName, sFTPPassword);
                clsSFTP objFtp = new clsSFTP(SFTPPath, sFTPUserName, sFTPPassword);
                bool Isuploaded;
                sDirectory = objWO.sWOFilePath;

                // Create Directory

                bool IsExists = objFtp.FtpDirectoryExists(SFTPmainfolder + sFolderName + "/");
                if (IsExists == false)
                {

                    objFtp.createDirectory(objWO.sWOId);
                }

                sWOFileName = Path.GetFileName(objWO.sWOFilePath);

                if (sWOFileName != "")
                {

                    if (File.Exists(sDirectory))
                    {
                        IsExists = objFtp.FtpDirectoryExists(SFTPmainfolder + sFolderName + "/" + objWO.sWOId + "/");
                        if (IsExists == false)
                        {

                            objFtp.createDirectory(SFTPmainfolder + sFolderName + "/" + objWO.sWOId );
                        }

                        Isuploaded = objFtp.upload(SFTPmainfolder + sFolderName + "/" +objWO.sWOId , sWOFileName, sDirectory);
                        if (Isuploaded == true & File.Exists(sDirectory))
                        {
                            File.Delete(sDirectory);
                            objWO.sWOFilePath = SFTPmainfolder + sFolderName + "/" + objWO.sWOId + "/" + sWOFileName;

                        }
                    }
                }

                string strQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();

                strQry = "UPDATE \"TBLWORKORDER\" SET \"WO_FILE_PATH\" =:sWOFilePath WHERE \"WO_SLNO\" =:sWOId";
                NpgsqlCommand.Parameters.AddWithValue("sWOFilePath", objWO.sWOFilePath);
                NpgsqlCommand.Parameters.AddWithValue("sWOId",Convert.ToInt32(objWO.sWOId));
                ObjCon.ExecuteQry(strQry, NpgsqlCommand);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

       
        public DataTable LoadAlreadyWorkOrder(clsWorkOrder objWorkOrder)
        {
            DataTable dt = new DataTable();
            string strQry = string.Empty;
            try
            {
               //Type=1----> Failure ;  Type=2-----------> Enhancement

                //strQry = "SELECT  DT_CODE, DT_NAME, TO_CHAR(TC_CODE) TC_CODE, TO_CHAR(DF_ID) DF_ID ,WO_NO,'YES' AS STATUS FROM TBLDTCMAST,TBLTCMASTER, TBLDTCFAILURE ,";
                //strQry += " TBLWORKORDER WHERE DF_EQUIPMENT_ID = TC_CODE  AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_ID =  WO_DF_ID AND";
                //strQry += " DF_STATUS_FLAG='" + objWorkOrder.sTaskType + "'  ";
                //strQry += " AND DF_LOC_CODE LIKE '" + objWorkOrder.sOfficeCode + "%' ";
                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadalreadyworkorder");
                cmd.Parameters.AddWithValue("stasktype", objWorkOrder.sTaskType);
                cmd.Parameters.AddWithValue("sofficecode", objWorkOrder.sOfficeCode);
                dt = ObjCon.FetchDataTable(cmd);                
                //AND DF_APPROVE_STATUS='1'
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;

            }
        }



        public DataTable LoadAllWorkOrder(clsWorkOrder objWorkOrder)
        {
           
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                //Type=1----> Failure ;  Type=2-----------> Enhancement
                //strQry = "SELECT  DT_CODE, DT_NAME, TC_SLNO, DF_ID, '' AS WO_NO   FROM TBLDTCMAST,TBLTCMASTER,TBLTRANSMAKES,";
                //strQry += " TBLDTCFAILURE WHERE DT_TC_ID = TC_CODE AND TM_ID = TC_MAKE_ID AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_STATUS_FLAG='"+ sType  +"' AND ";
                //strQry += " DF_ID NOT IN (SELECT WO_DF_ID FROM  TBLWORKORDER  )";

                //strQry = "SELECT  DT_CODE, DT_NAME, TO_CHAR(TC_CODE) TC_CODE, TO_CHAR(DF_ID) DF_ID ,WO_NO,'YES' AS STATUS FROM TBLDTCMAST,TBLTCMASTER, TBLDTCFAILURE ,";
                //strQry += " TBLWORKORDER WHERE DF_EQUIPMENT_ID = TC_CODE  AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_ID =  WO_DF_ID AND ";
                //strQry += " DF_STATUS_FLAG='" + objWorkOrder.sTaskType + "'  ";
                //strQry += " AND DF_LOC_CODE LIKE '" + objWorkOrder.sOfficeCode + "%'  ";
                //strQry += " UNION ALL";
                //strQry += " SELECT  DT_CODE, DT_NAME, TO_CHAR(TC_CODE) TC_CODE, TO_CHAR(DF_ID) DF_ID, '' AS WO_NO,'NO' AS STATUS   FROM TBLDTCMAST,TBLTCMASTER,";
                //strQry += " TBLDTCFAILURE WHERE DF_EQUIPMENT_ID = TC_CODE AND DT_CODE = DF_DTC_CODE AND DF_REPLACE_FLAG = 0 AND DF_STATUS_FLAG='" + objWorkOrder.sTaskType + "'  AND ";
                //strQry += " DF_ID NOT IN (SELECT WO_DF_ID FROM  TBLWORKORDER WHERE WO_DF_ID IS NOT NULL)";
                //strQry += " AND DF_LOC_CODE LIKE '" + objWorkOrder.sOfficeCode + "%' ";

                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadallworkorder");
                cmd.Parameters.AddWithValue("stasktype", objWorkOrder.sTaskType);
                cmd.Parameters.AddWithValue("sofficecode", objWorkOrder.sOfficeCode);
                dt = ObjCon.FetchDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;

            }
        }


        public object GetWorkOrderDetails(clsWorkOrder objWorkOrder)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dtWODetails = new DataTable();

                //strQry = "SELECT  WO_SLNO,WO_NO,WO_DF_ID,TO_CHAR(WO_DATE,'dd/MM/yyyy')WO_DATE,TO_CHAR(WO_AMT)WO_AMT,WO_NO_DECOM,WO_ACC_CODE,WO_ACCCODE_DECOM,";
                //strQry += " WO_OFF_CODE,WO_CRBY,TO_CHAR(WO_CRON,'dd/MM/yyyy')WO_CRON,TO_CHAR(WO_DATE_DECOM,'dd/MM/yyyy')WO_DATE_DECOM,TO_CHAR(WO_AMT_DECOM)WO_AMT_DECOM,WO_ISSUED_BY,  ";
                //strQry += " TO_CHAR(DF_DATE,'DD/MM/YYYY')DF_FAILED_DATE,WO_NEW_CAP,WO_REQUEST_LOC,DF_ENHANCE_CAPACITY ";
                //strQry+= " FROM TBLWORKORDER,TBLDTCFAILURE WHERE DF_ID=WO_DF_ID AND DF_ID='" + objWorkOrder.sFailureId + "' ";

                //dtWODetails = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getworkorderdetails");
                cmd.Parameters.AddWithValue("sfailureid", objWorkOrder.sFailureId);
                dtWODetails = ObjCon.FetchDataTable(cmd);

                if (dtWODetails.Rows.Count > 0)
               {

                   objWorkOrder.sWOId = Convert.ToString(dtWODetails.Rows[0]["WO_SLNO"]).Trim();
                   objWorkOrder.sAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CODE"]).Trim();
                   objWorkOrder.sCrBy = Convert.ToString(dtWODetails.Rows[0]["WO_CRBY"]).Trim();
                   objWorkOrder.sCommWoNo = Convert.ToString(dtWODetails.Rows[0]["WO_NO"]).Trim();
                   objWorkOrder.sCommDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE"]).Trim();
                   objWorkOrder.sCommAmmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT"]).Trim();
                   objWorkOrder.sDeWoNo = Convert.ToString(dtWODetails.Rows[0]["WO_NO_DECOM"]).Trim();
                   objWorkOrder.sDeCommDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_DECOM"]).Trim();
                   objWorkOrder.sDeCommAmmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_DECOM"]).Trim();
                   objWorkOrder.sIssuedBy = Convert.ToString(dtWODetails.Rows[0]["WO_ISSUED_BY"]).Trim();
                   objWorkOrder.sDecomAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACCCODE_DECOM"]).Trim();
                   objWorkOrder.sFailureId = Convert.ToString(dtWODetails.Rows[0]["WO_DF_ID"]).Trim();
                   objWorkOrder.sFailureDate = Convert.ToString(dtWODetails.Rows[0]["DF_FAILED_DATE"]).Trim();
                   objWorkOrder.sNewCapacity = Convert.ToString(dtWODetails.Rows[0]["WO_NEW_CAP"]).Trim();
                   objWorkOrder.sRequestLoc = Convert.ToString(dtWODetails.Rows[0]["WO_REQUEST_LOC"]).Trim();
                   objWorkOrder.sEnhancedCapacity = Convert.ToString(dtWODetails.Rows[0]["DF_ENHANCE_CAPACITY"]).Trim();
                    if (Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]).Trim()=="2")
                    {
                        objWorkOrder.sDeCreditWO = Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]).Trim();
                        objWorkOrder.sDeCreditAmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_CREDIT"]).Trim();
                        objWorkOrder.sDeCreditAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CRERDIT"]).Trim();
                        objWorkOrder.sDeCreditDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_CREDIT"]).Trim();
                    }
                    else
                    {
                        objWorkOrder.sCreditWO = Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]).Trim();
                        objWorkOrder.sCreditAmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_CREDIT"]).Trim();
                        objWorkOrder.sCreditAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CRERDIT"]).Trim();
                        objWorkOrder.sCreditDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_CREDIT"]).Trim();

                    }

                    NpgsqlCommand = new NpgsqlCommand();
                   strQry = "SELECT \"WO_WFO_ID\" FROM (SELECT \"WO_ID\",\"WO_WFO_ID\",row_number() over (PARTITION by \"WO_SLNO\" ";
                   strQry += " ORDER BY \"WO_ID\" desc) as \"RNUM\" FROM \"TBLWORKORDER\",\"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"";
                   strQry += " =\"WO_SLNO\" AND \"WO_BO_ID\"='11' AND \"WO_SLNO\"=:sWOId)A WHERE \"RNUM\" = 1";
                   NpgsqlCommand.Parameters.AddWithValue("sWOId", Convert.ToInt32(objWorkOrder.sWOId));
                   objWorkOrder.sWFDataId = ObjCon.get_value(strQry, NpgsqlCommand);
                 
                 
                   GetWODetailsFromXML(objWorkOrder);
                }
                return objWorkOrder;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWorkOrder;

            }

        }

        public bool ValidateUpdate(string strFailureId,string sWOSlno,string sType)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbDataReader dr;
                string strQry = string.Empty;
                if (sType != "3")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    strQry = " SELECT \"TI_WO_SLNO\" FROM \"TBLWORKORDER\",\"TBLINDENT\",\"TBLDTCFAILURE\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND ";
                    strQry += " \"DF_ID\"=\"WO_DF_ID\" AND \"WO_DF_ID\" =:strFailureId";
                    NpgsqlCommand.Parameters.AddWithValue("strFailureId", Convert.ToInt32(strFailureId));
                }
                else
                {
                    strQry = "select \"TI_WO_SLNO\" from \"TBLWORKORDER\",\"TBLINDENT\" WHERE \"WO_SLNO\"=\"TI_WO_SLNO\" AND ";
                    strQry += "  \"WO_SLNO\" =:sWOSlno";
                    NpgsqlCommand.Parameters.AddWithValue("sWOSlno", Convert.ToInt32(sWOSlno));
                }

                string sSLNo = ObjCon.get_value(strQry, NpgsqlCommand);
                if(sSLNo.Length > 0)
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

        public clsWorkOrder GetWOBasicDetails(clsWorkOrder objWO)
        {
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                //strQry = "SELECT DF_ID,to_char(DF_DATE,'dd/MM/yyyy') DF_FAILED_DATE,WO_SLNO,WO_NO,to_char(WO_DATE,'dd/MM/yyyy') WO_DATE,WO_NEW_CAP, ";
                //strQry += " DT_NAME,TC_CODE,(SELECT US_FULL_NAME FROM TBLUSER WHERE WO_CRBY=US_ID) US_FULL_NAME,DT_CODE,TC_ID,DT_ID,DF_LOC_CODE  FROM TBLDTCFAILURE,TBLWORKORDER,TBLDTCMAST,TBLTCMASTER";
                //strQry += " WHERE WO_DF_ID=DF_ID AND DF_EQUIPMENT_ID=TC_CODE AND  DF_DTC_CODE=DT_CODE AND WO_SLNO='" + objWO.sWOId + "'";
                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getwobasicdetails");
                cmd.Parameters.AddWithValue("swoid", objWO.sWOId);
                dt = ObjCon.FetchDataTable(cmd);

                if (dt.Rows.Count > 0)
                {
                    objWO.sCommWoNo = dt.Rows[0]["WO_NO"].ToString();
                    objWO.sCommDate = dt.Rows[0]["WO_DATE"].ToString();
                    objWO.sDTCName = dt.Rows[0]["DT_NAME"].ToString();
                    objWO.sTCCode = dt.Rows[0]["TC_CODE"].ToString();
                    objWO.sCrBy = dt.Rows[0]["US_FULL_NAME"].ToString();
                    objWO.sFailureId = dt.Rows[0]["DF_ID"].ToString();
                    objWO.sFailureDate = dt.Rows[0]["DF_FAILED_DATE"].ToString();
                    objWO.sNewCapacity = dt.Rows[0]["WO_NEW_CAP"].ToString();
                    objWO.sDTCCode = dt.Rows[0]["DT_CODE"].ToString();
                    objWO.sTCId = dt.Rows[0]["TC_ID"].ToString();
                    objWO.sDTCId = dt.Rows[0]["DT_ID"].ToString();
                    objWO.sLocationCode = dt.Rows[0]["DF_LOC_CODE"].ToString();
                }

                return objWO;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWO;
            }
        }


        public clsWorkOrder GetCommDecommAccCode(clsWorkOrder objWO)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                //strQry = "SELECT MD_COMM_ACCCODE,MD_DECOMM_ACCCODE,MD_ENHANCE_ACCCODE FROM TBLMASTERDATA WHERE MD_TYPE='C' AND MD_NAME='" + objWO.sCapacity + "'";
                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getcommdecommacccode");
                cmd.Parameters.AddWithValue("scapacity", objWO.sCapacity);
                dt = ObjCon.FetchDataTable(cmd);

                if (dt.Rows.Count > 0)
                {
                    objWO.sAccCode = Convert.ToString(dt.Rows[0]["MD_COMM_ACCCODE"]);
                    objWO.sDecomAccCode = Convert.ToString(dt.Rows[0]["MD_DECOMM_ACCCODE"]);
                    objWO.sDeCreditAccCode = Convert.ToString(dt.Rows[0]["MD_CREDIT_ACCCODE"]);
                    objWO.sEnhanceAccCode = Convert.ToString(dt.Rows[0]["MD_ENHANCE_ACCCODE"]);
                }
                return objWO;
               
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWO;
            }
        }

        public clsWorkOrder GetDTCAccCode(clsWorkOrder objWO)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand = new NpgsqlCommand();
                strQry = "SELECT \"SCHM_ACCCODE\" FROM \"TBLDTCSCHEME\" WHERE \"SCHM_ID\" =:sDtcScheme";
                NpgsqlCommand.Parameters.AddWithValue("sDtcScheme", objWO.sDtcScheme);
                objWO.sAccCode = ObjCon.get_value(strQry, NpgsqlCommand);
                
                return objWO;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWO;
            }
        }

        #region NewDTC
     
        public DataTable LoadNewDTCWO(clsWorkOrder objWorkOrder)
        {
            DataTable dt = new DataTable();
            string strQry = string.Empty;
            try
            {
                //strQry = "SELECT WO_SLNO,WO_NO,TO_CHAR(WO_DATE,'DD-MON-YYYY') WO_DATE,WO_ACC_CODE FROM TBLWORKORDER WHERE WO_DF_ID IS NULL ";
                //strQry += " AND WO_OFF_CODE LIKE '" + objWorkOrder.sOfficeCode + "%' AND WO_REPLACE_FLG='0'";
                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getcommdecommacccode");
                cmd.Parameters.AddWithValue("sofficecode", objWorkOrder.sOfficeCode);
                dt = ObjCon.FetchDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public object GetWODetailsForNewDTC(clsWorkOrder objWorkOrder)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dtWODetails = new DataTable();

                //strQry = "SELECT WO_SLNO,WO_NO,TO_CHAR(WO_DATE,'dd/MM/yyyy') WO_DATE,WO_AMT,WO_ACC_CODE,WO_ISSUED_BY,WO_NEW_CAP,";
                //strQry += " (SELECT US_FULL_NAME FROM TBLUSER WHERE WO_CRBY=US_ID) US_FULL_NAME,WO_REQUEST_LOC FROM TBLWORKORDER WHERE ";
                //strQry += " WO_SLNO='" + objWorkOrder.sWOId + "' ";
                //dtWODetails = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getwodetailsfornewdtc");
                cmd.Parameters.AddWithValue("swoid", objWorkOrder.sWOId);
                dtWODetails = ObjCon.FetchDataTable(cmd);

                if (dtWODetails.Rows.Count > 0)
                {

                    objWorkOrder.sWOId = dtWODetails.Rows[0]["WO_SLNO"].ToString();
                    objWorkOrder.sAccCode = dtWODetails.Rows[0]["WO_ACC_CODE"].ToString();
                    objWorkOrder.sCommWoNo = dtWODetails.Rows[0]["WO_NO"].ToString();
                    objWorkOrder.sCommDate = dtWODetails.Rows[0]["WO_DATE"].ToString();
                    objWorkOrder.sCommAmmount = dtWODetails.Rows[0]["WO_AMT"].ToString();                 
                    objWorkOrder.sIssuedBy = dtWODetails.Rows[0]["WO_ISSUED_BY"].ToString();                   
                    objWorkOrder.sNewCapacity = dtWODetails.Rows[0]["WO_NEW_CAP"].ToString();
                    objWorkOrder.sCrBy = dtWODetails.Rows[0]["US_FULL_NAME"].ToString();
                    objWorkOrder.sRequestLoc = dtWODetails.Rows[0]["WO_REQUEST_LOC"].ToString();
                    objWorkOrder.sRating = dtWODetails.Rows[0]["WO_RATING"].ToString();
                }
                return objWorkOrder;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWorkOrder;

            }

        }

        #endregion

        public void SendSMStoSectionOfficer(string sFailureId, string sDTCCode,string sWONo,string sDTCName)
        {
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sFailureId", sFailureId);
                string sOfficeCode = ObjCon.get_value("SELECT \"DF_LOC_CODE\" FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\" =:sFailureId", NpgsqlCommand);

                //strQry = "SELECT US_FULL_NAME,US_MOBILE_NO,US_EMAIL FROM TBLUSER WHERE US_ROLE_ID IN (4) AND US_OFFICE_CODE='" + sOfficeCode + "'";
                //dt = ObjCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getsocontact");
                cmd.Parameters.AddWithValue("sofficecode", sOfficeCode);
                dt = ObjCon.FetchDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sFullName = Convert.ToString(dt.Rows[i]["US_FULL_NAME"]);
                    string sMobileNo = Convert.ToString(dt.Rows[i]["US_MOBILE_NO"]);

                    clsCommunication objcomm = new clsCommunication();
                    objcomm.sSMSkey = "SMStoWorkOrder";
                    objcomm = objcomm.GetsmsTempalte(objcomm);

                    string sSMSText = String.Format(objcomm.sSMSTemplate, sDTCCode,sWONo,sDTCName);
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


        #region WorkFlow XML

        public clsWorkOrder GetWODetailsFromXML(clsWorkOrder objWorkOrder)
        {
            try
            {
                clsApproval objApproval = new clsApproval();
                DataTable dtWODetails = new DataTable();

                dtWODetails = objApproval.GetDatatableFromXML(objWorkOrder.sWFDataId);
                if (dtWODetails.Rows.Count > 0)
                {
                   // objWorkOrder.sWOId = Convert.ToString(dtWODetails.Rows[0]["WO_SLNO"]).Trim();
                    objWorkOrder.sAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CODE"]).Trim();
                    objWorkOrder.sCrBy = Convert.ToString(dtWODetails.Rows[0]["WO_CRBY"]).Trim();
                    objWorkOrder.sCommWoNo = Convert.ToString(dtWODetails.Rows[0]["WO_NO"]).Trim();
                    objWorkOrder.sCommDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE"]).Trim();
                    objWorkOrder.sCommAmmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT"]).Trim();
                    objWorkOrder.sDeWoNo = Convert.ToString(dtWODetails.Rows[0]["WO_NO_DECOM"]).Trim();
                    objWorkOrder.sDeCommDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_DECOM"]).Trim();
                    objWorkOrder.sDeCommAmmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_DECOM"]).Trim();
                    objWorkOrder.sIssuedBy = Convert.ToString(dtWODetails.Rows[0]["WO_ISSUED_BY"]).Trim();
                    objWorkOrder.sDecomAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACCCODE_DECOM"]).Trim();
                    objWorkOrder.sFailureId = Convert.ToString(dtWODetails.Rows[0]["WO_DF_ID"]).Trim();
                    //objWorkOrder.sFailureDate = Convert.ToString(dtWODetails.Rows[0]["DF_FAILED_DATE"]).Trim();
                    objWorkOrder.sNewCapacity = Convert.ToString(dtWODetails.Rows[0]["WO_NEW_CAP"]).Trim();
                    objWorkOrder.sRequestLoc = Convert.ToString(dtWODetails.Rows[0]["WO_REQUEST_LOC"]).Trim();
                    objWorkOrder.sDtcScheme = Convert.ToInt32(dtWODetails.Rows[0]["WO_DTC_SCHEME"]);
                    objWorkOrder.sRepairer = Convert.ToString(dtWODetails.Rows[0]["WO_REPAIRER"]);

                    if (dtWODetails.Columns.Contains("WO_NO_OF"))
                    {
                        if (Convert.ToString(dtWODetails.Rows[0]["WO_NO_OF"]).Trim() != "0")
                        {
                            objWorkOrder.sOFCommWoNo = Convert.ToString(dtWODetails.Rows[0]["WO_NO_OF"]).Trim();
                        }
                    }
                    if (dtWODetails.Columns.Contains("WO_DATE_OF"))
                    {
                        if (Convert.ToString(dtWODetails.Rows[0]["WO_DATE_OF"]).Trim() != "0")
                        {
                            objWorkOrder.sOFCommDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_OF"]).Trim();
                        }
                    }
                    if (dtWODetails.Columns.Contains("WO_AMT_OF"))
                    {
                        objWorkOrder.sOFCommAmmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_OF"]).Trim();
                    }
                    if (dtWODetails.Columns.Contains("WO_TTK_STATUS"))
                    {
                        objWorkOrder.sTtkStatus = Convert.ToString(dtWODetails.Rows[0]["WO_TTK_STATUS"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_TTK_VENDOR_NAME"))
                    {
                        objWorkOrder.sTtkVendorName = Convert.ToString(dtWODetails.Rows[0]["WO_TTK_VENDOR_NAME"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_TTK_MANUAL_NO"))
                    {
                        objWorkOrder.sTtkManual = Convert.ToString(dtWODetails.Rows[0]["WO_TTK_MANUAL_NO"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_TTK_AUTO_NO"))
                    {
                        objWorkOrder.sTtkAutoNo = Convert.ToString(dtWODetails.Rows[0]["WO_TTK_AUTO_NO"]);

                    }
                    if (dtWODetails.Columns.Contains("WO_DWA_NAME"))
                    {
                        objWorkOrder.sDWAname = Convert.ToString(dtWODetails.Rows[0]["WO_DWA_NAME"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_DWA_DATE"))
                    {
                        objWorkOrder.sDWAdate = Convert.ToString(dtWODetails.Rows[0]["WO_DWA_DATE"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_RATING"))
                    {
                        objWorkOrder.sRating = Convert.ToString(dtWODetails.Rows[0]["WO_RATING"]);
                    }
                    if (dtWODetails.Columns.Contains("WO_ACC_OF"))
                    {
                        objWorkOrder.sOFAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_OF"]).Trim();
                    }
                    string failtype = string.Empty;
                    string guaranteetype = string.Empty;
                    if (objWorkOrder.sFailureId != null && objWorkOrder.sFailureId != "")
                    {
                        failtype = ObjCon.get_value("SELECT \"EST_FAIL_TYPE\" from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"='" + objWorkOrder.sFailureId + "'");
                        guaranteetype = ObjCon.get_value("SELECT \"EST_GUARANTEETYPE\" from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"='" + objWorkOrder.sFailureId + "'");
                    }


                    if (failtype == "2")
                    {
                        objWorkOrder.sDeCreditWO = Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]);
                        objWorkOrder.sDeCreditAmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_CREDIT"]);
                        objWorkOrder.sDeCreditAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CRERDIT"]);
                        objWorkOrder.sDeCreditDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_CREDIT"]);
                    }
                    else
                    {
                        ////  if(failtype=="1" )
                        // // {
                        //      objWorkOrder.sDeCreditWO = Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]);
                        //      objWorkOrder.sDeCreditAmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_CREDIT"]);
                        //      objWorkOrder.sDeCreditAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CRERDIT"]);

                        //      objWorkOrder.sDeCreditDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_CREDIT"]);

                        //  }
                        //  else
                        // { 
                        objWorkOrder.sCreditWO = Convert.ToString(dtWODetails.Rows[0]["WO_NO_CREDIT"]);
                        objWorkOrder.sCreditAmount = Convert.ToString(dtWODetails.Rows[0]["WO_AMT_CREDIT"]);
                        objWorkOrder.sCreditAccCode = Convert.ToString(dtWODetails.Rows[0]["WO_ACC_CRERDIT"]);
                        objWorkOrder.sCreditDate = Convert.ToString(dtWODetails.Rows[0]["WO_DATE_CREDIT"]);
                        //  }
                    }
                }
                return objWorkOrder;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objWorkOrder;
            }
        }

        #endregion


        public ArrayList getCreatedByUserName(string sDataId, string sOffCode)
        {
            ArrayList strQrylist = new ArrayList();
            string sWoid = string.Empty;
            DataTable dt = new DataTable();
            try
            {

                sWoid = ObjCon.get_value("SELECT MAX(\"WO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"='11' AND \"WO_DATA_ID\"='" + sDataId + "' AND CAST(\"WO_OFFICE_CODE\" AS TEXT) LIKE SUBSTR('" + sOffCode + "',1,3)");
                //dt = ObjCon.getDataTable("SELECT (SELECT US_FULL_NAME  FROM TBLUSER WHERE US_ID=WO_CR_BY) FROM TBLWORKFLOWOBJECTS WHERE  WO_INITIAL_ID ='" + sWoid + "' ORDER BY WO_ID");
                //dt = ObjCon.getDataTable("SELECT US_FULL_NAME,(CASE WHEN US_ROLE_ID='7' THEN 1 WHEN US_ROLE_ID ='2' THEN 2 WHEN US_ROLE_ID ='6' THEN 3 ELSE 4 END )SLEVEL  FROM TBLUSER WHERE US_ROLE_ID IN(7,2,6,3) AND US_OFFICE_CODE LIKE '" + sOffCode + "%' AND US_MMS_ID IS NULL AND US_STATUS='A' ORDER BY SLEVEL");

                NpgsqlCommand cmd = new NpgsqlCommand("sp_getcreatedbyusername");
                cmd.Parameters.AddWithValue("sofficecode", sOffCode);
                dt = ObjCon.FetchDataTable(cmd);

                for (int i = 0; i < 4; i++)
                {
                    if (dt.Rows.Count > i)
                    {
                        if (dt.Rows[i][0].ToString() != "" || dt.Rows[i][0].ToString() != null)
                            strQrylist.Add(dt.Rows[i][0].ToString());
                    }
                    else
                        strQrylist.Add("");

                }
                return strQrylist;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return strQrylist;
            }


        }

        public string getWoDataId(string sFailureId, string sWOSLno)
        {
            string sQry = string.Empty;
            string sWoDataId = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();

            if (sFailureId != null && sFailureId != "")
            {
                sQry = "SELECT MAX(\"WO_WFO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_DATA_ID\"='" + Convert.ToInt32(sFailureId) + "' AND \"WO_BO_ID\"='11'";
                // NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(sFailureId));
                sWoDataId = ObjCon.get_value(sQry);
                return sWoDataId;
            }
            else if (sWOSLno != null && sWOSLno != "")
            {
                sQry = "SELECT MAX(\"WO_WFO_ID\") FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"='" + Convert.ToInt32(sWOSLno) + "' AND \"WO_BO_ID\"='11'";
                // NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(sFailureId));
                sWoDataId = ObjCon.get_value(sQry);
                return sWoDataId;
            }
            return sWoDataId;
        }

        public string getofficeName(string offcode)
        {
            try
            {
                string sQry = string.Empty;

                sQry = "SELECT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CODE\" AS TEXT)=SUBSTR(CAST('" + offcode +"' AS TEXT),1,'"+Constants.Division+"')";
                return ObjCon.get_value(sQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public string GetRating(string rating)
        {
            try
            {
                string strQry = string.Empty;
                string Rating = string.Empty;
                strQry += " SELECT \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\"='SR' and \"MD_ID\"=" + rating + " ORDER BY \"MD_ORDER_BY\"";
                Rating = ObjCon.get_value(strQry);
                return Rating;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
        public string getsubdivName(string df_id)
        {
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                sQry = "SELECT \"SD_SUBDIV_NAME\" FROM \"TBLDTCFAILURE\",\"TBLSUBDIVMAST\" WHERE ";
                sQry += " CAST(\"SD_SUBDIV_CODE\" AS TEXT) = SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,'" + Constants.SubDivision + "') AND \"DF_ID\" =:df_id";
                NpgsqlCommand.Parameters.AddWithValue("df_id", Convert.ToInt32(df_id));
                return ObjCon.get_value(sQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public DataTable GetofficeName(string df_id)
        {
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                sQry = "SELECT (SELECT SUBSTR(\"OFF_NAME\", STRPOS(\"OFF_NAME\", ':') + 1) FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT) = SUBSTR(CAST(\"DF_LOC_CODE\" as TEXT), 1,3) )DIV,(SELECT SUBSTR(\"OFF_NAME\", STRPOS(\"OFF_NAME\", ':') + 1) FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT) = SUBSTR(CAST(\"DF_LOC_CODE\" as TEXT), 1,4) )SUBDIV FROM \"TBLDTCFAILURE\" WHERE \"DF_ID\"=:df_id";
                NpgsqlCommand.Parameters.AddWithValue("df_id", Convert.ToInt64(df_id));
                dt = ObjCon.FetchDataTable(sQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        public DataTable GetofficeNameBySectionCode(string sOM_Code)
        {
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;
                sQry = "SELECT (SELECT SUBSTR(\"OFF_NAME\", STRPOS(\"OFF_NAME\", ':') + 1) FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT) = SUBSTR(CAST('"+ sOM_Code + "' as TEXT), 1,3) )DIV,(SELECT SUBSTR(\"OFF_NAME\", STRPOS(\"OFF_NAME\", ':') + 1) FROM \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT) = SUBSTR(CAST('"+ sOM_Code + "' as TEXT), 1,4) )SUBDIV FROM \"TBLOMSECMAST\" WHERE \"OM_CODE\"='" + sOM_Code + "'";
                dt = ObjCon.FetchDataTable(sQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        #region before pgrs Add code changed by rudra 
        //public DataTable FailDetails(string sFailId, string sFailType,string sCoilType,string ActionType,string sGuarenteeType,string sWoRecordId="")
        //{
        //    DataTable dtWODetails = new DataTable();
        //    string sQry = string.Empty;
        //    try
        //    {
        //        if(ActionType == "V")
        //        {
        //            if(sWoRecordId.Contains('-'))
        //            {
        //                if (sFailType != "2")
        //                {
        //                    NpgsqlCommand = new NpgsqlCommand();
        //                    sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId";
        //                    NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
        //                }
        //                else
        //                {
        //                    NpgsqlCommand = new NpgsqlCommand();
        //                    sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId1";
        //                    NpgsqlCommand.Parameters.AddWithValue("sFailId1", Convert.ToInt32(sFailId));
        //                }
        //            }
        //            else
        //            {
        //                if (sFailType != "2" && sCoilType!="2")
        //                {
        //                    if(sFailType=="1")
        //                    { 
        //                    NpgsqlCommand = new NpgsqlCommand();
        //                    sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"DF_ID\"=:sFailId";
        //                    NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
        //                    }
        //                    else
        //                    {
        //                        NpgsqlCommand = new NpgsqlCommand();
        //                    sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId";
        //                    NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
        //                    }

        //                    //NpgsqlCommand = new NpgsqlCommand();
        //                    //sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    //sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    //sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLWORKORDER\" ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"WO_SLNO\"=:sFailId2";
        //                    //NpgsqlCommand.Parameters.AddWithValue("sFailId2", Convert.ToInt32(sFailId));
        //                }
        //                else
        //                {
        //                    NpgsqlCommand = new NpgsqlCommand();
        //                    sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                    sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                    sQry += " =\"EST_FAILUREID\"  INNER JOIN \"TBLWORKORDER\" ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"WO_SLNO\"=:sFailId3";
        //                    NpgsqlCommand.Parameters.AddWithValue("sFailId3", Convert.ToInt32(sFailId));
        //                }
        //            }

        //        }
        //        else
        //        {
        //            if (sFailType != "2" && sCoilType != "2")
        //            {
        //                NpgsqlCommand = new NpgsqlCommand();
        //                sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId4";
        //                NpgsqlCommand.Parameters.AddWithValue("sFailId4", Convert.ToInt32(sFailId));
        //            }
        //            else
        //            {
        //                NpgsqlCommand = new NpgsqlCommand();
        //                sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
        //                sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
        //                sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId5";
        //                NpgsqlCommand.Parameters.AddWithValue("sFailId5", Convert.ToInt32(sFailId));
        //            }
        //        }


        //        dtWODetails = ObjCon.FetchDataTable(sQry, NpgsqlCommand);
        //        return dtWODetails;
        //    }
        //    catch(Exception ex)
        //    {
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return dtWODetails;
        //    }
        //}
        #endregion
        // changed by rudra on 09-06-2020 for display pgrs  Docket number in work order
        public DataTable FailDetails(string sFailId, string sFailType, string sCoilType, string ActionType, string sGuarenteeType, string sWoRecordId = "")
        {
            DataTable dtWODetails = new DataTable();
            string sQry = string.Empty;
            try
            {
                if (ActionType == "V")
                {
                    if (sWoRecordId.Contains('-'))
                    {
                        if (sFailType != "2")
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                            sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                            sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId";
                            NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
                        }
                        else
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                            sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                            sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId1";
                            NpgsqlCommand.Parameters.AddWithValue("sFailId1", Convert.ToInt32(sFailId));
                        }
                    }
                    else
                    {
                        if (sFailType != "2" && sCoilType != "2")
                        {
                            if (sFailType == "1")
                            {
                                NpgsqlCommand = new NpgsqlCommand();
                                sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                                sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                                sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"DF_ID\"=:sFailId";
                                NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
                            }
                            else
                            {
                                NpgsqlCommand = new NpgsqlCommand();
                                sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                                sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                                sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId";
                                NpgsqlCommand.Parameters.AddWithValue("sFailId", Convert.ToInt32(sFailId));
                            }

                            //NpgsqlCommand = new NpgsqlCommand();
                            //sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                            //sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                            //sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLWORKORDER\" ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"WO_SLNO\"=:sFailId2";
                            //NpgsqlCommand.Parameters.AddWithValue("sFailId2", Convert.ToInt32(sFailId));
                        }
                        else
                        {
                            NpgsqlCommand = new NpgsqlCommand();
                            sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                            sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                            sQry += " =\"EST_FAILUREID\"  INNER JOIN \"TBLWORKORDER\" ON \"DF_ID\"=\"WO_DF_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"WO_SLNO\"=:sFailId3";
                            NpgsqlCommand.Parameters.AddWithValue("sFailId3", Convert.ToInt32(sFailId));
                        }
                    }

                }
                else
                {
                    if (sFailType != "2" && sCoilType != "2")
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                        sQry += " ,\"TR_ID\",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",\"TR_NAME\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                        sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLTRANSREPAIRER\" ON \"EST_REPAIRER\" = \"TR_ID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId4";
                        NpgsqlCommand.Parameters.AddWithValue("sFailId4", Convert.ToInt32(sFailId));
                    }
                    else
                    {
                        NpgsqlCommand = new NpgsqlCommand();
                        sQry = "SELECT \"DF_ID\",\"DF_DTC_CODE\",\"DF_EQUIPMENT_ID\",\"DF_PGRS_DOCKET\",to_char(\"DF_DATE\",'dd-mm-yyyy')DF_DATE,to_char(\"DF_DATE\",'dd-MON-yyyy')DF_MONTH_DATE";
                        sQry += ",\"EST_CAPACITY\",\"EST_FAIL_TYPE\",UPPER(\"DT_NAME\")DT_NAME,SUBSTR(CAST(\"DF_LOC_CODE\" AS TEXT),1,4)DF_LOC_CODE FROM \"TBLDTCFAILURE\" INNER JOIN \"TBLESTIMATIONDETAILS\" ON \"DF_ID\"";
                        sQry += " =\"EST_FAILUREID\" INNER JOIN \"TBLDTCMAST\" ON \"DF_DTC_CODE\"=\"DT_CODE\" WHERE \"EST_ID\"=:sFailId5";
                        NpgsqlCommand.Parameters.AddWithValue("sFailId5", Convert.ToInt32(sFailId));
                    }
                }


                dtWODetails = ObjCon.FetchDataTable(sQry, NpgsqlCommand);
                return dtWODetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtWODetails;
            }
        }
        public string FailureId(string sWo_Slno,string sWoRecordID="")
        {
            string sQry = string.Empty;
            try
            {
                #region Old Code when work order at only one level
                //sQry = "SELECT \"EST_ID\" FROM \"TBLWORKORDER\",\"TBLESTIMATIONDETAILS\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND ";
                //sQry += " \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\" ='" + sWo_Slno + "'";
                #endregion

                if (sWo_Slno.Contains('-'))
                {
                    clsFormValues objForm = new clsFormValues();
                    NpgsqlCommand = new NpgsqlCommand();
                    sQry = "SELECT \"WO_RECORD_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_ID\" in (SELECT \"WOA_PREV_APPROVE_ID\" FROM ";
                    sQry += " \"TBLWORKFLOWOBJECTS\",\"TBLWO_OBJECT_AUTO\" WHERE \"WO_INITIAL_ID\"=\"WOA_INITIAL_ACTION_ID\" and ";
                    sQry += " \"WO_BO_ID\" ='11' and \"WO_RECORD_ID\"=:sWo_Slno and \"WOA_BFM_ID\"='3')";
                    NpgsqlCommand.Parameters.AddWithValue("sWo_Slno", Convert.ToInt32(sWo_Slno));
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    if (sWoRecordID.Contains('-'))
                    {
                        return sWo_Slno ;
                    }
                    else
                    {
                        sQry = "SELECT \"EST_ID\" FROM \"TBLWORKORDER\",\"TBLESTIMATIONDETAILS\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND ";
                        sQry += " \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\" =:sWo_Slno1";
                        NpgsqlCommand.Parameters.AddWithValue("sWo_Slno1", Convert.ToInt32(sWo_Slno));
                    }
                    
                }


                return ObjCon.get_value(sQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GetFailCoilType(string sWo_Slno,string sActionType)
        {
            string sQry = string.Empty;
            try
            {
                if(sActionType == "V")
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    sQry = "SELECT cast(\"EST_FAIL_TYPE\" as text)|| '~' ||cast(\"WO_SLNO\"as text) FROM \"TBLESTIMATIONDETAILS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"DF_ID\"=\"EST_FAILUREID\" AND \"WO_DF_ID\" =:sWo_Slno";
                    NpgsqlCommand.Parameters.AddWithValue("sWo_Slno",Convert.ToInt32(sWo_Slno));
                }
                else
                {
                    NpgsqlCommand = new NpgsqlCommand();
                    sQry = "SELECT cast(\"EST_FAIL_TYPE\" as text)|| '~' ||cast(\"WO_SLNO\"as text) FROM \"TBLESTIMATIONDETAILS\",\"TBLWORKORDER\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"WO_DF_ID\" AND \"DF_ID\"=\"EST_FAILUREID\" AND \"WO_DF_ID\" =:sWo_Slno1";
                    NpgsqlCommand.Parameters.AddWithValue("sWo_Slno1", Convert.ToInt32(sWo_Slno));
                }

                return ObjCon.get_value(sQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GetUntilOrEmpty(string text, string stopAt = "~")
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(text))
                {
                    int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                    if (charLocation > 0)
                    {
                        return text.Substring(0, charLocation);
                    }
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string getestimatedate(string failid)
        {
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand = new NpgsqlCommand();
                if (failid!="")
                {
                    sQry = " SELECT TO_CHAR(\"EST_DATE\",'dd/mm/yyyy') from \"TBLESTIMATIONDETAILS\" WHERE \"EST_FAILUREID\"=:failid ";
                    NpgsqlCommand.Parameters.AddWithValue("failid", Convert.ToInt64(failid));
                    return ObjCon.get_value(sQry, NpgsqlCommand);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string getfailid(string sFailureId)
        {
            string sQry = string.Empty;
            string sWoDataId = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();
            sQry = "SELECT DISTINCT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_RECORD_ID\"=:sFailureId AND \"WO_BO_ID\"='11'";
            NpgsqlCommand.Parameters.AddWithValue("sFailureId", Convert.ToInt32(sFailureId));
            sWoDataId = ObjCon.get_value(sQry, NpgsqlCommand);
            return sWoDataId;
        }
        //public DataTable GetWorkOrderDetails(string sWO_SLNO)
        //{
        //    DataTable dtWODetails = new DataTable();
        //    string sQry = string.Empty;
        //    try
        //    {
        //        sQry = "SELECT \"EST_ID\" FROM \"TBLWORKORDER\",\"TBLESTIMATIONDETAILS\",\"TBLDTCFAILURE\" WHERE \"DF_ID\"=\"EST_FAILUREID\" AND ";
        //        sQry += " \"DF_ID\"=\"WO_DF_ID\" AND \"WO_SLNO\" ='" + sWO_SLNO + "'";
        //        dtWODetails = ObjCon.FetchDataTable(sQry);
        //        return dtWODetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "FailDetails");
        //        return dtWODetails;
        //    }
        //}
        public string getFailureId(string swoSlno,string type)
        {
             string sQry = string.Empty;
            if (type == "1")
            {
                NpgsqlCommand = new NpgsqlCommand();
                sQry = "SELECT \"DF_ID\" FROM \"TBLDTCFAILURE\",\"TBLWORKORDER\" WHERE \"DF_ID\"=\"WO_DF_ID\" and \"WO_SLNO\"=:swoSlno";
                NpgsqlCommand.Parameters.AddWithValue("swoSlno", Convert.ToInt32(swoSlno));
            }
            else
            {
                NpgsqlCommand = new NpgsqlCommand();
                sQry = "SELECT DISTINCT \"WO_DATA_ID\" FROM \"TBLWORKFLOWOBJECTS\" WHERE \"WO_BO_ID\"='11' and \"WO_RECORD_ID\"=:swoSlno1";
                NpgsqlCommand.Parameters.AddWithValue("swoSlno1", Convert.ToInt32(swoSlno));
            }

            return ObjCon.get_value(sQry, NpgsqlCommand); 
        }
    }
}
