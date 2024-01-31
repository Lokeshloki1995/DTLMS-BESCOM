using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.DTLMS.BL;
using System.Data.OleDb;
using System.Data;
using IIITS.PGSQL.DAL;
using NpgsqlTypes;
using Npgsql;

namespace IIITS.DTLMS.BL
{
   public class clsStoreInvoice
    {
        //CustOledbConnection objCon = new CustOledbConnection(Constants.Password);
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);

        string strFormCode = "clsStoreInvoice";

        public string sIssueQty { get; set; }
        public string sFilepath { get; set; }

        public string sQuantity { get; set; }
        public string sTcCapacity { get; set; }
        public string sIndentNo { get; set; }
        public string sIndentDate { get; set; }
        public string sDescription { get; set; }
        public string sInvoiceNo { get; set; }
        public string sInvoiceDate { get; set; }
        public string sRemarks{ get; set; }
        public string sInvoiceId { get; set; }
        public string sCrBy { get; set; }
        public DataTable ddtTcGrid { get; set; }
        public string sIndentId { get; set; }
        public string sTcSlNo { get; set; }
        public string sInvoiceObjectId { get; set; }
        public string sSiId { get; set; }
        public string sFromStoreId { get; set; }
        public string sTcCode{ get; set; }
        public string sTcName { get; set; }
        public string sTcId { get; set; }
        public DataTable dIndentTcGrid { get; set; }

        // Workflow
        public string sFormName { get; set; }
        public string sClientIP { get; set; }
        public string sWFDataId { get; set; }
        public string sActionType { get; set; }
        public string sWFOId { get; set; }

        public string sOfficeCode { get; set; }
        public string sRefOfficeCode { get; set; }
        public string sRecordId { get; set; }
        public string sCrby { get; set; }
        public string sRoleId { get; set; }
        

        public DataTable dtDocuments { get; set; }

        //To Load Indent details to grid      
       public DataTable LoadInvoiceGrid(string sOfficeCode)
       {
           string strQry = string.Empty;
           DataTable dtIndentDetails = new DataTable();
           try
           {
               //if (sOfficeCode.Length >= 3)
               //{
               //    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
               //}

                //strQry = "SELECT SI_ID, SI_DATE,SI_NO, REQ_QNTY, SI_FROM_STORE, (COALESCE(REQ_QNTY,0) - COALESCE(SENT_QNT,0)) AS PENDINGCOUNT,IS_NO  ";
                //strQry += "FROM (SELECT SI_ID,TO_CHAR(SI_DATE,'dd-MON-yyyy')SI_DATE, SI_NO, SUM(SO_QNTY) REQ_QNTY, ";
                //strQry += "(SELECT SM_NAME FROM TBLSTOREMAST WHERE SM_ID=SI_TO_STORE) SI_TO_STORE ,";
                //strQry += " (SELECT SM_NAME FROM TBLSTOREMAST WHERE SM_ID=SI_FROM_STORE) SI_FROM_STORE ";
                //strQry += "FROM TBLSTOREINDENT, TBLSINDENTOBJECTS WHERE SI_ID = SO_SI_ID and SI_TRANSFER_FLAG=0  ";
                //strQry += "AND (SELECT SM_OFF_CODE FROM TBLSTOREMAST WHERE SM_ID=SI_TO_STORE) like '" + sOfficeCode + "%' GROUP BY SI_ID,SI_DATE,SI_TO_STORE, SI_NO,SI_FROM_STORE)A,";
                //strQry += "(SELECT IS_SI_ID, COUNT(IO_CAPACITY) AS SENT_QNT,IS_NO ";
                //strQry += "FROM TBLSTOREINVOICE, TBLSINVOICEOBJECTS WHERE IS_ID = IO_IS_ID GROUP BY IS_SI_ID,IS_NO )B WHERE A.SI_ID= B.IS_SI_ID(+) AND B.IS_NO IS NULL ORDER BY SI_NO DESC";

                //dtIndentDetails=objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadinvoicegrid");
                cmd.Parameters.AddWithValue("offcode", sOfficeCode);
                dtIndentDetails = objcon.FetchDataTable(cmd);

                return dtIndentDetails;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
           }
       }



       public DataTable LoadCompletedInvoiceGrid(string sOfficeCode)
       {
           string strQry = string.Empty;
           DataTable dtIndentDetails = new DataTable();
           try
           {
               //if (sOfficeCode.Length >= 3)
               //{
               //    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
               //}

                //strQry = "SELECT SI_ID, SI_DATE,SI_NO, REQ_QNTY, SI_FROM_STORE, (COALESCE(REQ_QNTY,0) - COALESCE(SENT_QNT,0)) AS PENDINGCOUNT,IS_NO  ";
                //strQry += "FROM (SELECT SI_ID,TO_CHAR(SI_DATE,'dd-MON-yyyy')SI_DATE, SI_NO, SUM(SO_QNTY) REQ_QNTY, ";
                //strQry += "(SELECT SM_NAME FROM TBLSTOREMAST WHERE SM_ID=SI_TO_STORE) SI_TO_STORE ,";
                //strQry += " (SELECT SM_NAME FROM TBLSTOREMAST WHERE SM_ID=SI_FROM_STORE) SI_FROM_STORE ";
                //strQry += "FROM TBLSTOREINDENT, TBLSINDENTOBJECTS WHERE SI_ID = SO_SI_ID and SI_TRANSFER_FLAG=1  ";
                //strQry += "AND (SELECT SM_OFF_CODE FROM TBLSTOREMAST WHERE SM_ID=SI_TO_STORE) like '" + sOfficeCode + "%' GROUP BY SI_ID,SI_DATE,SI_TO_STORE, SI_NO,SI_FROM_STORE)A,";
                //strQry += "(SELECT IS_SI_ID, COUNT(IO_CAPACITY) AS SENT_QNT,IS_NO ";
                //strQry += "FROM TBLSTOREINVOICE, TBLSINVOICEOBJECTS WHERE IS_ID = IO_IS_ID GROUP BY IS_SI_ID,IS_NO )B WHERE A.SI_ID= B.IS_SI_ID(+) ORDER BY SI_NO DESC";

                //dtIndentDetails = objCon.getDataTable(strQry);

                NpgsqlCommand cmd = new NpgsqlCommand("sp_loadcompletedinvoicegrid");
                cmd.Parameters.AddWithValue("offcode", sOfficeCode);
                dtIndentDetails = objcon.FetchDataTable(cmd);

                return dtIndentDetails;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
           }
       }

       //Function to Populate indent grid values to textbox
       NpgsqlCommand NpgsqlCommand;
       public object LoadIndentDetails(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           DataTable dtIndentDetails = new DataTable();
           try
           {
               strQry = "SELECT \"SI_ID\",TO_CHAR(\"SI_DATE\",'dd/MM/yyyy')SI_DATE,\"SI_NO\",SUM(\"SO_QNTY\")SO_QNTY,\"SI_FROM_STORE\" FROM \"TBLSTOREINDENT\",\"TBLSINDENTOBJECTS\" where \"SI_ID\"=\"SO_SI_ID\"  ";
               if (objInvoice.sIndentId != "")
               {
                   NpgsqlCommand.Parameters.AddWithValue("IndentId",Convert.ToInt32(objInvoice.sIndentId));
                   strQry += " AND \"SI_ID\" =:IndentId"; 
               }
               if (objInvoice.sIndentNo != "")
               {
                   NpgsqlCommand.Parameters.AddWithValue("IndentNo", objInvoice.sIndentNo);
                   string strIndentNo = objcon.get_value("SELECT \"SI_NO\" FROM \"TBLSTOREINDENT\" WHERE \"SI_NO\" =:IndentNo", NpgsqlCommand);
                  if (strIndentNo != "")
                  {
                      NpgsqlCommand.Parameters.AddWithValue("sIndentNo1", objInvoice.sIndentNo);
                      strQry += " AND \"SI_NO\" =:sIndentNo1";
                  }
                  else
                  {
                      objInvoice.sIndentNo = "";
                      return objInvoice;
                  }
               }
               strQry += " GROUP BY \"SI_NO\",\"SI_ID\",\"SI_DATE\",\"SI_FROM_STORE\"";
               dtIndentDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);
               objInvoice.sIndentId = Convert.ToString(dtIndentDetails.Rows[0]["SI_ID"]);
               objInvoice.sIndentNo = Convert.ToString(dtIndentDetails.Rows[0]["SI_NO"]);
               objInvoice.sIndentDate = Convert.ToString(dtIndentDetails.Rows[0]["SI_DATE"]);
               objInvoice.sQuantity = Convert.ToString(dtIndentDetails.Rows[0]["SO_QNTY"]);
               objInvoice.sFromStoreId= Convert.ToString(dtIndentDetails.Rows[0]["SI_FROM_STORE"]);
               return objInvoice;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
           }
       }
       //Function to load capacity grid 
       public DataTable LoadCapacityGrid(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           DataTable dtCapacityDetails = new DataTable();
           try
           {
               NpgsqlCommand.Parameters.AddWithValue("IndentId",Convert.ToInt32(objInvoice.sIndentId));
               strQry = "SELECT \"SI_ID\",\"CAPACITY\",\"REQ_QNTY\",(COALESCE(\"REQ_QNTY\",0) - COALESCE(\"SENT_QNT\",0)) AS PENDINGCOUNT  FROM"; 
               strQry += " (SELECT \"SI_ID\",SUM(\"SO_QNTY\") \"REQ_QNTY\",\"SO_CAPACITY\" AS \"CAPACITY\" FROM \"TBLSTOREINDENT\", \"TBLSINDENTOBJECTS\" WHERE \"SI_ID\" = \"SO_SI_ID\" GROUP BY \"SI_ID\", \"SO_CAPACITY\")A LEFT OUTER JOIN";
               strQry += " (SELECT \"IS_SI_ID\",\"IO_CAPACITY\",COUNT(\"IO_CAPACITY\") AS \"SENT_QNT\" FROM \"TBLSTOREINVOICE\", \"TBLSINVOICEOBJECTS\" WHERE \"IS_ID\" = \"IO_IS_ID\" GROUP BY \"IS_SI_ID\",\"IO_CAPACITY\")B";
               strQry += " ON A.\"SI_ID\" = B.\"IS_SI_ID\" AND A.\"CAPACITY\" =B.\"IO_CAPACITY\" WHERE A.\"SI_ID\" =:IndentId";
               dtCapacityDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);
               return dtCapacityDetails;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCapacityDetails;
           }
       }
       //Function to save invoice details
       public string[] SaveStoreInvoice(clsStoreInvoice objInvoice)
       {
           
           string strQry = string.Empty;
           string[] Arr = new string[2];
           OleDbDataReader dr;
           clsApproval objApproval = new clsApproval();
           try
           {
               if (objInvoice.sInvoiceId == "")
               {
                    NpgsqlCommand = new NpgsqlCommand();
                    //To get the Store id of logged in user
                    NpgsqlCommand.Parameters.AddWithValue("InvoiceNo", objInvoice.sInvoiceNo);
                   string sNo = objcon.get_value("select \"IS_NO\" from \"TBLSTOREINVOICE\" where \"IS_NO\" =:InvoiceNo", NpgsqlCommand);
                   if (sNo.Length > 0)
                   {                       
                       Arr[0] = "Entered Invoice Number Already Exists";
                       Arr[1] = "2";
                       return Arr;
                   }

                 
                   DataTable dtDoc = new DataTable();
                   dtDoc = objInvoice.dtDocuments;

                   //strQry = "DELETE FROM TBLSINVOICEOBJECTS WHERE IO_IS_ID IN(select IS_ID FROM TBLSTOREINVOICE,TBLSTOREINDENT WHERE IS_SI_ID='" + objInvoice.sIndentId + "')";
                   //objCon.Execute(strQry);
                   objInvoice.sInvoiceId = Convert.ToString(objcon.Get_max_no("IS_ID", "TBLSTOREINVOICE"));

                   NpgsqlCommand.Parameters.AddWithValue("InvoiceId",Convert.ToInt32(objInvoice.sInvoiceId ));
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceNo1", objInvoice.sInvoiceNo);
                   NpgsqlCommand.Parameters.AddWithValue("IndentId1", Convert.ToInt32(objInvoice.sIndentId));
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceDate", objInvoice.sInvoiceDate);
                   NpgsqlCommand.Parameters.AddWithValue("Remarks", objInvoice.sRemarks);
                   NpgsqlCommand.Parameters.AddWithValue("CrBy",Convert.ToInt32( objInvoice.sCrBy));
                    if(dtDoc.Rows.Count>0)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("dtDoc", Convert.ToString(dtDoc.Rows[0]["PATH"]));
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("dtDoc", null);
                    }

                   strQry = "INSERT INTO \"TBLSTOREINVOICE\" (\"IS_ID\",\"IS_NO\",\"IS_SI_ID\",\"IS_DATE\",\"IS_REMARKS\",\"IS_CRBY\",\"IS_CRON\",\"IS_FILE_PATH\") VALUES ";
                   strQry += " (:InvoiceId,:InvoiceNo1,:IndentId1,";
                    if(dtDoc.Rows.Count > 0)
                    {
                        strQry += " to_date(:InvoiceDate,'dd/MM/yyyy'),:Remarks,:CrBy,NOW(),  :dtDoc)";
                    }
                    else
                    {
                        strQry += " to_date(:InvoiceDate,'dd/MM/yyyy'),:Remarks,:CrBy,NOW(), null)";
                    }
                   
                   objcon.ExecuteQry(strQry, NpgsqlCommand);
                 
                   for (int i = 0; i < objInvoice.ddtTcGrid.Rows.Count; i++)
                   {
                        NpgsqlCommand = new NpgsqlCommand();

                        objInvoice.sInvoiceObjectId = Convert.ToString(objcon.Get_max_no("IO_ID", "TBLSINVOICEOBJECTS"));

                       NpgsqlCommand.Parameters.AddWithValue("InvoiceObjectId",Convert.ToInt32(objInvoice.sInvoiceObjectId));
                       NpgsqlCommand.Parameters.AddWithValue("InvoiceId2",Convert.ToInt32(objInvoice.sInvoiceId));
                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid",Convert.ToDouble(objInvoice.ddtTcGrid.Rows[i]["TC_CAPACITY"]));
                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid1", Convert.ToDouble(objInvoice.ddtTcGrid.Rows[i]["TC_CODE"]));
                       NpgsqlCommand.Parameters.AddWithValue("CrBy1", Convert.ToInt32(objInvoice.sCrBy));

                       strQry = "INSERT INTO \"TBLSINVOICEOBJECTS\"(\"IO_ID\",\"IO_IS_ID\",\"IO_CAPACITY\",\"IO_TCCODE\",\"IO_CRBY\",\"IO_CRON\") VALUES ";
                       strQry += " (:InvoiceObjectId,:InvoiceId2,";
                       strQry += " :ddtTcGrid,:ddtTcGrid1,";
                       strQry += " :CrBy1,NOW())";
                       objcon.ExecuteQry(strQry, NpgsqlCommand);
                       //strQry = "Update TBLSTOREINDENT SET SI_TRANSFER_FLAG=1 WHERE SI_ID='" + objInvoice.sIndentId +"'";
                       //objCon.Execute(strQry);
                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid2",Convert.ToDouble( ddtTcGrid.Rows[i]["TC_CODE"]));
                       strQry = "Update \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\" =4 WHERE \"TC_CODE\" =:ddtTcGrid2";
                       objcon.ExecuteQry(strQry, NpgsqlCommand);
                   }

                   UpdateIndentStatus(objInvoice);


                   #region WorkFlow

                   //Workflow / Approval
                   
                   objApproval.sFormName = objInvoice.sFormName;
                   objApproval.sRecordId = objInvoice.sRecordId;
                   objApproval.sNewRecordId = objInvoice.sInvoiceId;
                   objApproval.sOfficeCode = objInvoice.sRefOfficeCode;
                   objApproval.sClientIp = objInvoice.sClientIP;
                   objApproval.sCrby = objInvoice.sCrBy;
                   objApproval.sWFObjectId = objInvoice.sWFOId;
                   
                  // objApproval.sRefOfficeCode = objcon.get_value("SELECT \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" ='" + objInvoice.sOfficeCode + "'");

                   objApproval.sRefOfficeCode = objInvoice.sRefOfficeCode;

                   objApproval.sDescription = "Store Invoice Creation for Indent No " + objInvoice.sIndentNo;
                   objApproval.sApproveStatus = "1";
                   objApproval.sApproveComments = "Approved";
                   objApproval.ApproveWFRequest(objApproval);

                   #endregion

                   Arr[0] = "Saved Successfully";
                   Arr[1] = "0";
                   return Arr;
               }
               else
               {
                    NpgsqlCommand = new NpgsqlCommand();
                    objcon.BeginTransaction();
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceNo2", objInvoice.sInvoiceNo);
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceId3",Convert.ToInt32(objInvoice.sInvoiceId));
                   string sNo = objcon.get_value("select \"IS_NO\" from \"TBLSTOREINVOICE\" where \"IS_NO\" =:InvoiceNo2 and \"IS_ID\" <>:InvoiceId3", NpgsqlCommand);
                   if (sNo.Length > 0)
                   {
                       Arr[0] = "Entered Invoice Number Already Exists";
                       Arr[1] = "2";
                       return Arr;
                   }

                   NpgsqlCommand.Parameters.AddWithValue("InvoiceNo3", objInvoice.sInvoiceNo);
                   NpgsqlCommand.Parameters.AddWithValue("Remarks1", objInvoice.sRemarks);
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceDate1", objInvoice.sInvoiceDate);
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceId4",Convert.ToInt32(objInvoice.sInvoiceId));
                   strQry = "UPDATE \"TBLSTOREINVOICE\" SET \"IS_NO\" =:InvoiceNo3, \"IS_REMARKS\" =:Remarks1, \"IS_DATE\" =to_date(:InvoiceDate1,'dd/MM/yyyy') WHERE \"IS_ID\" =:InvoiceId4";
                   objcon.ExecuteQry(strQry, NpgsqlCommand);
                   //deleting old records
                   NpgsqlCommand.Parameters.AddWithValue("InvoiceId5",Convert.ToInt32(objInvoice.sInvoiceId));
                   strQry = "DELETE FROM \"TBLSINVOICEOBJECTS\" WHERE \"IO_IS_ID\" =:InvoiceId5";
                   objcon.ExecuteQry(strQry, NpgsqlCommand);
                   for (int i = 0; i < objInvoice.ddtTcGrid.Rows.Count; i++)
                   {
                        NpgsqlCommand = new NpgsqlCommand();
                        //objInvoice.sInvoiceObjectId = Convert.ToString(objcon.Get_max_no("\"IO_ID\"", "\"TBLSINVOICEOBJECTS\""));
                        objInvoice.sInvoiceObjectId = Convert.ToString(objcon.Get_max_no("IO_ID", "TBLSINVOICEOBJECTS"));

                        NpgsqlCommand.Parameters.AddWithValue("InvoiceObjectId1",Convert.ToInt32(objInvoice.sInvoiceObjectId));
                       NpgsqlCommand.Parameters.AddWithValue("InvoiceId5",Convert.ToInt32(objInvoice.sInvoiceId));
                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid3",Convert.ToDouble(objInvoice.ddtTcGrid.Rows[i]["TC_CAPACITY"]));
                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid4",Convert.ToDouble(objInvoice.ddtTcGrid.Rows[i]["TC_CODE"]));
                       NpgsqlCommand.Parameters.AddWithValue("CrBy2",Convert.ToInt32( objInvoice.sCrBy));

                       strQry = "INSERT INTO \"TBLSINVOICEOBJECTS\"(\"IO_ID\",\"IO_IS_ID\",\"IO_CAPACITY\",\"IO_TCCODE\",\"IO_CRBY\",\"IO_CRON\") VALUES(:InvoiceObjectId1,:InvoiceId5,:ddtTcGrid3,:ddtTcGrid4,:CrBy2,NOW())";
                       objcon.ExecuteQry(strQry, NpgsqlCommand);

                       NpgsqlCommand.Parameters.AddWithValue("ddtTcGrid5",Convert.ToDouble(ddtTcGrid.Rows[i]["TC_CODE"] ));
                       strQry = "Update \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\"=4 WHERE \"TC_CODE\" =:ddtTcGrid5 AND \"TC_CURRENT_LOCATION\" <>4";
                       objcon.ExecuteQry(strQry, NpgsqlCommand);
                       
                   }
                   UpdateIndentStatus(objInvoice);

                   objcon.CommitTransaction();
                   Arr[0] = "Updated Successfully";
                   Arr[1] = "1";
                   return Arr;


               }

           }
           catch (Exception ex)
           {
               objcon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
           }
       }
       public void UpdateIndentStatus(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           try
           {
               NpgsqlCommand.Parameters.AddWithValue("IndentId",Convert.ToInt32(objInvoice.sIndentId));
               string strInvoiceCount = objcon.get_value("SELECT COUNT(\"IO_CAPACITY\")AS Count FROM \"TBLSINVOICEOBJECTS\",\"TBLSTOREINVOICE\" WHERE \"IO_IS_ID\"=\"IS_ID\" AND \"IS_SI_ID\" =:IndentId ", NpgsqlCommand);
          
               //if (Convert.ToInt32(strInvoiceCount) ==Convert.ToInt32(objInvoice.sQuantity))
               //{  
               NpgsqlCommand.Parameters.AddWithValue("IndentId1",Convert.ToInt32(objInvoice.sIndentId));
               strQry = "UPDATE \"TBLSTOREINDENT\" SET \"SI_TRANSFER_FLAG\" =1 WHERE \"SI_ID\" =:IndentId1";
               objcon.ExecuteQry(strQry, NpgsqlCommand);
               //}
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
       }
       public object CheckTc(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           try
           {
               NpgsqlCommand.Parameters.AddWithValue("TcCode",Convert.ToDouble(objInvoice.sTcCode));
               string strTcCode = objcon.get_value("Select \"TC_CODE\" FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:TcCode", NpgsqlCommand);
               if (strTcCode == "")
              {
                  return objInvoice.sTcCode = "";
              }
              else
              {
                  objInvoice.sTcCode = strTcCode;
                  return objInvoice.sTcCode;
              }

           }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
            }
       }
        //Function to Load Tc Details Grid
        public object LoadTcDetails(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("TcCode",Convert.ToDouble(objInvoice.sTcCode));
                string strTcCode = objcon.get_value("SELECT \"IO_TCCODE\" FROM \"TBLSINVOICEOBJECTS\",\"TBLSTOREINVOICE\" WHERE \"IO_TCCODE\" =:TcCode AND \"IS_APPROVE_FLAG\" ='0' AND \"IO_IS_ID\"=\"IS_ID\"", NpgsqlCommand);
                if (strTcCode != "")
                {
                    objInvoice.sTcCode = "";
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("TcCode1", Convert.ToDouble(objInvoice.sTcCode));
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode", Convert.ToInt32(objInvoice.sOfficeCode));
                    NpgsqlCommand.Parameters.AddWithValue("IndentId", Convert.ToInt32(objInvoice.sIndentId));
                    strQry = "SELECT \"TC_ID\",\"TC_SLNO\",\"TC_CODE\",CAST(\"TC_CAPACITY\" AS TEXT)TC_CAPACITY,(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE ";
                    strQry += " \"TM_ID\" = \"TC_MAKE_ID\" )TM_NAME FROM \"TBLTCMASTER\" WHERE \"TC_CODE\" =:TcCode1 AND ";
                    if (sRoleId== Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"]))
                    {
                        strQry += " \"TC_CURRENT_LOCATION\" =1 AND \"TC_STORE_ID\" = '"+clsStoreOffice.GetStoreID(sOfficeCode) +"' AND \"TC_CAPACITY\" IN ";
                    }
                    else
                    {
                        strQry += " \"TC_CURRENT_LOCATION\" =1 AND \"TC_STORE_ID\" = :OfficeCode AND \"TC_CAPACITY\" IN ";
                    }
                    strQry += "(SELECT \"SO_CAPACITY\" FROM \"TBLSINDENTOBJECTS\" WHERE \"SO_SI_ID\" =:IndentId)";

                    dtTcDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);
                    if (dtTcDetails.Rows.Count > 0)
                    {
                        objInvoice.sTcId = Convert.ToString(dtTcDetails.Rows[0]["TC_ID"]);
                        objInvoice.sTcSlNo = Convert.ToString(dtTcDetails.Rows[0]["TC_SLNO"]);
                        objInvoice.sTcCode = Convert.ToString(dtTcDetails.Rows[0]["TC_CODE"]);
                        objInvoice.sTcCapacity = Convert.ToString(dtTcDetails.Rows[0]["TC_CAPACITY"]);
                        objInvoice.sTcName = Convert.ToString(dtTcDetails.Rows[0]["TM_NAME"]);
                    }
                    else
                    {
                        objInvoice.sIndentId = "";
                        objInvoice.sTcId = "";
                    }
                    return objInvoice;
                }
                return objInvoice;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;

            }
        }

        public void UpdateDeleteItem(clsStoreInvoice objInvoice)
        {
            NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           try
           {
               NpgsqlCommand.Parameters.AddWithValue("TcCode",Convert.ToDouble(objInvoice.sTcCode));
               strQry = "UPDATE \"TBLTCMASTER\" SET \"TC_CURRENT_LOCATION\" =1 WHERE \"TC_CODE\"=:TcCode";
               objcon.ExecuteQry(strQry, NpgsqlCommand);

           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
       }

       public DataTable LoadDtrDetails(clsStoreInvoice objInvoice)
        {
            NpgsqlCommand = new NpgsqlCommand();
           string strQry = string.Empty;
           DataTable dtBasicDetails = new DataTable();
           try
           {
               strQry = "select \"TC_CODE\",CAST(\"TC_CAPACITY\" AS TEXT)TC_CAPACITY,\"TC_SLNO\",\"TC_MAKE_ID\",\"IS_NO\",TO_CHAR(\"IS_DATE\",'dd/MM/yyyy')IS_DATE,(select \"TM_NAME\" from \"TBLTRANSMAKES\"  where \"TM_ID\"=\"TC_MAKE_ID\") as Make ";
               strQry += "from \"TBLSINVOICEOBJECTS\",\"TBLTCMASTER\",\"TBLSTOREINVOICE\" where \"IO_TCCODE\"=\"TC_CODE\" and \"IS_ID\"=\"IO_IS_ID\"  ";
               if (objInvoice.sIndentId != "")
               {
                   NpgsqlCommand.Parameters.AddWithValue("IndentId",Convert.ToInt32(objInvoice.sIndentId));
                   strQry += " AND \"IS_SI_ID\" =:IndentId";
               }
               dtBasicDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);
               return dtBasicDetails;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtBasicDetails;
           }
       }

       public clsStoreInvoice GetStoreInvoiceDetails(clsStoreInvoice objInvoice)
       {
           NpgsqlCommand = new NpgsqlCommand();
           try
           {
               string strQry = string.Empty;
               DataTable dt = new DataTable();
               NpgsqlCommand.Parameters.AddWithValue("IndentId",Convert.ToInt32(objInvoice.sIndentId));
               strQry = "SELECT \"IS_NO\",TO_CHAR(\"IS_DATE\",'DD/MM/YYYY') IS_DATE,\"IS_REMARKS\" FROM \"TBLSTOREINVOICE\" WHERE \"IS_SI_ID\" =:IndentId";
               dt = objcon.FetchDataTable(strQry, NpgsqlCommand);
               if (dt.Rows.Count > 0)
               {
                   objInvoice.sInvoiceNo = Convert.ToString(dt.Rows[0]["IS_NO"]);
                   objInvoice.sInvoiceDate = Convert.ToString(dt.Rows[0]["IS_DATE"]);
                   objInvoice.sRemarks = Convert.ToString(dt.Rows[0]["IS_REMARKS"]);
                  
               }
               return objInvoice;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objInvoice;
           }
       }

       public string[] SaveWFOdata(clsStoreInvoice objInvoice)
       {

           string[] Arr = new string[2];
           try
           {
              
               string strQry = string.Empty;
               StringBuilder sbQuery = new StringBuilder();

               clsApproval objApproval = new clsApproval();
               DataTable dtDoc = new DataTable();
               dtDoc = objInvoice.dtDocuments;
               objApproval.sCrby = objInvoice.sCrby;

              // string sPrimaryKey = "{0}";

               objApproval.sColumnNames = "NAME,PATH";
               objApproval.sColumnValues = "" + Convert.ToString(dtDoc.Rows[0]["NAME"]) + ", " + Convert.ToString(dtDoc.Rows[0]["PATH"]) + "";
               objApproval.sTableNames = "TBLSTOREINVOICE";

               objApproval.SaveWorkFlowData(objApproval);
               objInvoice.sWFDataId = objApproval.sWFDataId;


               return Arr;
           }
           catch (Exception ex)
           {
               objcon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
           }
       }

       public DataTable GetMafFilePath(string sWFO_ID)
       {
           DataTable dt = new DataTable();
           try
           {
               clsApproval objApproval = new clsApproval();
               dt = objApproval.GetDatatableFromXML(sWFO_ID);
               return dt;
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
           }
       }
    }
     
}
