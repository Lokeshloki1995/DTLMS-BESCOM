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
    public class clsReceiveTrans
    {
        string strQry = string.Empty;
        string strFormCode = "clsReceiveTrans";
        public string sInvoiceId { get; set; }
        public string sInvoiceNo { get; set; }
        public string sInvoiceDate { get; set; }
        public string sFromStoreId { get; set; }
        public string sCreatedBy { get; set; }
        public string sQuantity { get; set; }
        public string sRemarks { get; set; }
        public string sOfficeCode { get; set; }
        public string sOfficeCodestore { get; set; }

        public string sIndentNo { get; set; }
        public string sIndentDate { get; set; }
        public string sIndentId { get; set; }
        public string sRefofficecode { get; set; }

        public string sRVNo { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFOId { get; set; }

        //CustOledbConnection objCon = new CustOledbConnection(Constants.Password);
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
   
        public DataTable LoadReceiveTcGrid(string sOfficeCode)
        {
            DataTable dtTcReceive = new DataTable();
            try
            {

                //strQry = "SELECT IS_ID,IS_NO,TO_CHAR(IS_DATE,'DD-MON-YYYY')IS_DATE,(SELECT SM_NAME FROM TBLSTOREMAST WHERE  SM_ID=SI_TO_STORE) AS SI_FROM_STORE,COUNT(IO_TCCODE)IO_TCCODE,IO_IS_ID,SI_NO";
                //strQry += " FROM TBLSTOREINVOICE,TBLSTOREINDENT,TBLSINVOICEOBJECTS,TBLSTOREMAST WHERE IS_SI_ID=SI_ID AND IS_APPROVE_FLAG = 0 AND ";
                //strQry += " IO_IS_ID=IS_ID and SM_ID=SI_FROM_STORE AND SM_OFF_CODE LIKE '" + sOfficeCode + "%' ";
                //strQry += " GROUP BY IO_IS_ID,IS_ID,IS_NO,IS_DATE,SM_NAME,SI_NO,SI_TO_STORE ORDER BY IS_NO DESC";

                ////AND SM_OFF_CODE='" + sOfficeCode.Substring(0, 2) + "' ";
                //dtTcReceive = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadreceivetcgrid");
                cmd.Parameters.AddWithValue("sofficecode", sOfficeCode);
                dtTcReceive = objcon.FetchDataTable(cmd);

                return dtTcReceive;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcReceive;
            }

        }

        public DataTable LoadComplededReceiveTcGrid(string sOfficeCode)
        {
            DataTable dtTcReceive = new DataTable();
            try
            {

                //strQry = " SELECT IS_ID,IS_NO,TO_CHAR(IS_DATE,'DD-MON-YYYY')IS_DATE,(SELECT SM_NAME FROM TBLSTOREMAST WHERE  SM_ID=SI_TO_STORE) AS SI_FROM_STORE,COUNT(IO_TCCODE)IO_TCCODE,IO_IS_ID,SI_NO";
                //strQry += " FROM TBLSTOREINVOICE,TBLSTOREINDENT,TBLSINVOICEOBJECTS,TBLSTOREMAST WHERE IS_SI_ID=SI_ID AND IS_APPROVE_FLAG = 1 AND ";
                //strQry += " IO_IS_ID=IS_ID and SM_ID=SI_FROM_STORE AND SM_OFF_CODE LIKE '" + sOfficeCode + "%' ";
                //strQry += " GROUP BY IO_IS_ID,IS_ID,IS_NO,IS_DATE,SM_NAME,SI_NO,SI_TO_STORE ORDER BY IS_NO DESC";

                ////AND SM_OFF_CODE='" + sOfficeCode.Substring(0, 2) + "' ";
                //dtTcReceive = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadcomplededreceivetcgrid");
                cmd.Parameters.AddWithValue("sofficecode", sOfficeCode);
                dtTcReceive = objcon.FetchDataTable(cmd);

                return dtTcReceive;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcReceive;
            }

        }


        public object LoadInvoiceDetails(clsReceiveTrans objInvoice)
        {
            string strQry = string.Empty;
            DataTable dtIndentDetails = new DataTable();
            try
            {
                //strQry = "select IS_ID,IS_NO,IS_SI_ID,TO_CHAR(IS_DATE,'DD-MON-YYYY')IS_DATE,SI_TO_STORE,SI_NO,TO_CHAR(SI_DATE,'DD-MON-YYYY')SI_DATE,SI_ID, ";
                //strQry += " (select COUNT(IO_TCCODE)AS SI_TO_STORE  from TBLSINVOICEOBJECTS where IO_IS_ID=IS_ID ) QUANTITY ";
                //strQry += " from TBLSTOREINVOICE,TBLSTOREINDENT  where IS_SI_ID=SI_ID ";
                //if (objInvoice.sInvoiceId != "")
                //{
                //    strQry += " AND IS_ID='" + objInvoice.sInvoiceId + "'";
                //}

                //strQry += " GROUP BY IS_ID,IS_NO,IS_SI_ID,IS_DATE,SI_FROM_STORE,SI_TO_STORE,SI_NO,SI_DATE,SI_ID";
                //dtIndentDetails = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_LoadInvoiceDetails");
                cmd.Parameters.AddWithValue("sinvoiceid", objInvoice.sInvoiceId);
                dtIndentDetails = objcon.FetchDataTable(cmd);


                if (dtIndentDetails.Rows.Count > 0)
                {
                    objInvoice.sInvoiceId = Convert.ToString(dtIndentDetails.Rows[0]["IS_ID"]);
                    objInvoice.sInvoiceNo = Convert.ToString(dtIndentDetails.Rows[0]["IS_NO"]);
                    objInvoice.sInvoiceDate = Convert.ToString(dtIndentDetails.Rows[0]["IS_DATE"]);
                    objInvoice.sQuantity = Convert.ToString(dtIndentDetails.Rows[0]["QUANTITY"]);
                    objInvoice.sFromStoreId = Convert.ToString(dtIndentDetails.Rows[0]["SI_TO_STORE"]);

                    objInvoice.sIndentNo = Convert.ToString(dtIndentDetails.Rows[0]["SI_NO"]);
                    objInvoice.sIndentDate = Convert.ToString(dtIndentDetails.Rows[0]["SI_DATE"]);
                    objInvoice.sIndentId = Convert.ToString(dtIndentDetails.Rows[0]["SI_ID"]);
                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }



        public DataTable LoadCapacityGrid(clsReceiveTrans objInvoice)
        {
            string strQry = string.Empty;
            DataTable dtCapacityDetails = new DataTable();
            try
            {
                //strQry = " SELECT TC_CODE,TC_SLNO,TO_CHAR(TC_CAPACITY) TC_CAPACITY,(SELECT TM_NAME FROM TBLTRANSMAKES WHERE ";
                //strQry += " TM_ID=TC_MAKE_ID) TM_NAME,CASE WHEN TC_STATUS='1' THEN 'BRAND NEW' WHEN TC_STATUS='2' THEN 'REPAIR GOOD' WHEN TC_STATUS='3' THEN 'FAULTY' END STATUS FROM TBLTCMASTER,TBLSINVOICEOBJECTS ";
                //strQry += "WHERE IO_IS_ID='" + objInvoice.sInvoiceId + "' AND IO_TCCODE=TC_CODE ";
                //dtCapacityDetails = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadcapacitygrid");
                cmd.Parameters.AddWithValue("sinvoiceid", objInvoice.sInvoiceId);
                dtCapacityDetails = objcon.FetchDataTable(cmd);

                return dtCapacityDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCapacityDetails;
            }
        }
        NpgsqlCommand NpgsqlCommand;
        public string[] RecieveTransformer(clsReceiveTrans objInvoice)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            string[] Arr = new string[2];
            DataTable dt = new DataTable();
             try
             {
                 NpgsqlCommand.Parameters.AddWithValue("InvoiceId",Convert.ToInt32(objInvoice.sInvoiceId));
                //strQry = "SELECT DISTINCT (SELECT DISTINCT \"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"DIV_CODE\"=\"TC_LOCATION_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" IN (SELECT \"IO_TCCODE\" FROM ";
                //strQry += " \"TBLSINVOICEOBJECTS\" WHERE \"IO_IS_ID\" =:InvoiceId)";
                //string sOld_Store_name = objcon.get_value(strQry, NpgsqlCommand);

                strQry = "SELECT DISTINCT (SELECT DISTINCT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\"=\"TC_LOCATION_ID\") FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" IN (SELECT \"IO_TCCODE\" FROM ";
                strQry += " \"TBLSINVOICEOBJECTS\" WHERE \"IO_IS_ID\" =:InvoiceId)";
                string sOld_Store_name = objcon.get_value(strQry, NpgsqlCommand);

                NpgsqlCommand.Parameters.AddWithValue("OfficeCode", Convert.ToInt32(objInvoice.sOfficeCode));
                strQry = "SELECT \"SM_NAME\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\" =:OfficeCode";
                string sNew_Store_name = objcon.get_value(strQry, NpgsqlCommand);


                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadtccode");
                cmd.Parameters.AddWithValue("sinvoiceid", objInvoice.sInvoiceId);
                dt = objcon.FetchDataTable(cmd);

                //strQry = "SELECT \"IO_TCCODE\" FROM \"TBLSINVOICEOBJECTS\" WHERE \"IO_IS_ID\" ='" + objInvoice.sInvoiceId + "'";
                //dt = objcon.getDataTable(strQry);

                // objcon.BeginTrans();

                //if (objInvoice.sOfficeCode.Length > 1)
                //{
                //    objInvoice.sOfficeCode = objInvoice.sOfficeCode.Substring(0, Constants.Division);
                //}
                NpgsqlCommand.Parameters.AddWithValue("OfficeCode1", Convert.ToInt32(objInvoice.sOfficeCode));
                if (objInvoice.sOfficeCode=="2")
                {
                    objInvoice.sOfficeCodestore = objcon.get_value("SELECT \"SM_CODE\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\" =:OfficeCode1", NpgsqlCommand);
                }
                else
                {
                    objInvoice.sOfficeCodestore = objcon.get_value("SELECT \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" =:OfficeCode1", NpgsqlCommand);
                }

                clsTcMaster objTcMaster = new clsTcMaster();
                string sStoreId = objTcMaster.GetStoreId(objInvoice.sOfficeCodestore);

                string[] sArr = new string[2];
                NpgsqlCommand cmd1 = new NpgsqlCommand("sp_saveestimationdetails");
                cmd1.Parameters.AddWithValue("srvno", objInvoice.sRVNo);
                cmd1.Parameters.AddWithValue("scrby", objInvoice.sCreatedBy);
                cmd1.Parameters.AddWithValue("sremarks", objInvoice.sRemarks);
                cmd1.Parameters.AddWithValue("sinvoiceid", objInvoice.sInvoiceId);
                cmd1.Parameters.AddWithValue("soffcode", objInvoice.sOfficeCode);
                cmd1.Parameters.AddWithValue("sstoreid", sStoreId);
                objcon.Execute(cmd1, sArr, 0);

                //strQry = "UPDATE TBLSTOREINVOICE set IS_APPROVE_FLAG=1,IS_APPROVE_DATE=sysdate,IS_RV_NO='"+ objInvoice.sRVNo +"',";
                //strQry+=" IS_APPROVE_BY='" + objInvoice.sCreatedBy + "', IS_APPROVE_REMARKS='" + objInvoice.sRemarks + "' where IS_ID='" + objInvoice.sInvoiceId + "' ";
                //objCon.Execute(strQry);

                //strQry = "Update TBLTCMASTER SET TC_CURRENT_LOCATION=1,TC_LOCATION_ID='" + objInvoice.sOfficeCode + "',TC_STORE_ID='"+ sStoreId +"' WHERE TC_CODE IN ";
                //strQry += " (SELECT IO_TCCODE FROM TBLSINVOICEOBJECTS WHERE IO_IS_ID='" + objInvoice.sInvoiceId + "')";
                //objCon.Execute(strQry);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    string[] sArry = new string[2];
                    NpgsqlCommand cmd2 = new NpgsqlCommand("sp_savedtrtransaction");
                    cmd2.Parameters.AddWithValue("stccode", dt.Rows[i]["IO_TCCODE"].ToString());
                    cmd2.Parameters.AddWithValue("sstoreid", sStoreId);
                    cmd2.Parameters.AddWithValue("sold_store_name", sOld_Store_name);
                    cmd2.Parameters.AddWithValue("snew_store_name", sNew_Store_name);
                    objcon.Execute(cmd2, sArry, 0);

                    //strQry = "INSERT INTO TBLDTRTRANSACTION VALUES ((SELECT COALESCE(max(DRT_ID), 0) + 1 FROM TBLDTRTRANSACTION),'" + dt.Rows[i]["IO_TCCODE"].ToString() + "',";
                    //strQry += "'" + sStoreId + "','1',SYSDATE,'','1','IUT MOVED FROM STORE : " + sOld_Store_name + "  TO STORE : " + sNew_Store_name + "','1',";
                    //strQry += "SYSDATE,'0','','')";
                    //objCon.Execute(strQry);
                }

                //objCon.CommitTrans();

                //Workflow / Approval
                clsApproval objApproval = new clsApproval();
                objApproval.sFormName = objInvoice.sFormName;
                objApproval.sRecordId = objInvoice.sInvoiceId;
                objApproval.sOfficeCode = objInvoice.sOfficeCode;
                objApproval.sClientIp = objInvoice.sClientIP;
                objApproval.sCrby = objInvoice.sCreatedBy;
                objApproval.sWFObjectId = objInvoice.sWFOId;
                objApproval.sRefOfficeCode = objInvoice.sRefofficecode;
                 
                NpgsqlCommand.Parameters.AddWithValue("InvoiceId1",Convert.ToInt32(objInvoice.sInvoiceId));
                string sResult = objcon.get_value("SELECT \"SI_NO\" ||'~' || \"IS_NO\" FROM \"TBLSTOREINDENT\",\"TBLSTOREINVOICE\" WHERE \"IS_ID\" =:InvoiceId1 AND \"SI_ID\"=\"IS_SI_ID\"", NpgsqlCommand);

                objApproval.sDescription = "Response for Store Indent No " + sResult.Split('~').GetValue(0).ToString() + " with Store Invoice Number " + sResult.Split('~').GetValue(1).ToString();
             
                objApproval.SaveWorkflowObjects(objApproval);

                Arr[0] = "Recieved Successfully";
                Arr[1] = "0";
                return Arr;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public clsReceiveTrans LoadReceivedInvoiceDetails(clsReceiveTrans objInvoice)
        {
            string strQry = string.Empty;
            DataTable dtIndentDetails = new DataTable();
            try
            {
                //strQry = "select IS_ID,IS_NO,IS_SI_ID,TO_CHAR(IS_DATE,'DD-MON-YYYY')IS_DATE,SI_TO_STORE,SI_NO,TO_CHAR(SI_DATE,'DD-MON-YYYY')SI_DATE,SI_ID,";
                //strQry += " (select COUNT(IO_TCCODE)AS SI_TO_STORE  from TBLSINVOICEOBJECTS where IO_IS_ID=IS_ID ) QUANTITY,IS_APPROVE_REMARKS,IS_RV_NO ";
                //strQry += " from TBLSTOREINVOICE,TBLSTOREINDENT  WHERE IS_SI_ID=SI_ID ";
                //if (objInvoice.sInvoiceId != "")
                //{
                //    strQry += " AND IS_ID='" + objInvoice.sInvoiceId + "'";
                //}

                /*dtIndentDetails = objCon.getDataTable(strQry);*/

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadreceivedinvoicedetails");
                cmd.Parameters.AddWithValue("sinvid", objInvoice.sInvoiceId);
                dtIndentDetails = objcon.FetchDataTable(cmd);

                if (dtIndentDetails.Rows.Count > 0)
                {
                    objInvoice.sInvoiceId = Convert.ToString(dtIndentDetails.Rows[0]["IS_ID"]);
                    objInvoice.sInvoiceNo = Convert.ToString(dtIndentDetails.Rows[0]["IS_NO"]);
                    objInvoice.sInvoiceDate = Convert.ToString(dtIndentDetails.Rows[0]["IS_DATE"]);
                    objInvoice.sQuantity = Convert.ToString(dtIndentDetails.Rows[0]["QUANTITY"]);
                    objInvoice.sFromStoreId = Convert.ToString(dtIndentDetails.Rows[0]["SI_TO_STORE"]);
                    objInvoice.sRemarks = Convert.ToString(dtIndentDetails.Rows[0]["IS_APPROVE_REMARKS"]);
                    objInvoice.sRVNo = Convert.ToString(dtIndentDetails.Rows[0]["IS_RV_NO"]);

                    objInvoice.sIndentNo = Convert.ToString(dtIndentDetails.Rows[0]["SI_NO"]);
                    objInvoice.sIndentDate = Convert.ToString(dtIndentDetails.Rows[0]["SI_DATE"]);
                    objInvoice.sIndentId = Convert.ToString(dtIndentDetails.Rows[0]["SI_ID"]); 
                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
        }


        public DataTable LoadIndentDetGrid(clsReceiveTrans objInvoice)
        {
            string strQry = string.Empty;
            DataTable dtIndentDetDetails = new DataTable();
            try
            {
                //strQry = " SELECT SO_QNTY,TO_CHAR(SO_CAPACITY)SO_CAPACITY ";
                //strQry += "  FROM TBLSTOREINDENT,TBLSINDENTOBJECTS ";
                //strQry += "WHERE SO_SI_ID='" + objInvoice.sIndentId + "' AND SO_SI_ID=SI_ID ";
                //dtIndentDetDetails = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadindentdetgrid");
                cmd.Parameters.AddWithValue("sindentid", objInvoice.sIndentId);
                dtIndentDetDetails = objcon.FetchDataTable(cmd);

                return dtIndentDetDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetDetails;
            }
        }

    }
}
