using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIITS.PGSQL.DAL;
using System.Data;
using Npgsql;
using System.Configuration;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
   
    public class clsDelivery
    {
        public string sTotalTC { get; set; }
        public string sPoId { get; set; }
        public string sDIid { get; set; }
        public string sDINo { get; set; }
        public string sPONumber { get; set; }
        public DataTable dtDelivery { get; set; }
        public Byte[] POFile { get; set; }
        public string sCapacity { get; set; }
        public string sTcMake { get; set; }
        public string sStore { get; set; }
        public string sFileExt { get; set; }
        public string sCrby { get; set; }

        public string strFormCode = "clsDelivery";

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;

        public object GetPurchaseCount(clsDelivery obj)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("phno", obj.sPONumber);
                sQry = "SELECT SUM(\"PB_QUANTITY\") FROM \"TBLPOOBJECTS\",\"TBLPOMASTER\" WHERE \"PO_ID\" = \"PB_PO_ID\" AND \"PB_PO_STATUS\"=1  AND \"PO_NO\" =:phno";
                obj.sTotalTC = objcon.get_value(sQry, NpgsqlCommand);
                               
                return obj;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }

        public string GetPOId(clsDelivery obj)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("phno", obj.sPONumber);
                sQry = "SELECT \"PO_ID\" FROM \"TBLPOMASTER\" WHERE  \"PO_NO\" =:phno";
                obj.sPoId = objcon.get_value(sQry, NpgsqlCommand);
                return obj.sPoId;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public DataTable GetAllotmentCount(clsDelivery objDelivery)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtIndentDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("DINo", Convert.ToString(objDelivery.sDINo));

                strQry = " select \"ALT_NO\", sum(\"ALT_QUANTITY\") as \"ALLOTED\" from \"TBLALLOTEMENT\" where \"ALT_DI_NO\"=:DINo  and \"ALT_CAPACITY\"='" + objDelivery.sCapacity + "'  AND  \"ALT_MAKE_ID\" ='" + objDelivery.sTcMake + "' and \"ALT_STORE_ID\"= '" + objDelivery.sStore + "'    GROUP BY \"ALT_NO\"";

                dtIndentDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtIndentDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
            }
        }
        public DataTable GetDIDetails(clsDelivery objDelivery)
        {
              PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
              NpgsqlCommand = new NpgsqlCommand();
              DataTable dtIndentDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("DINo", Convert.ToString(objDelivery.sDINo));
                strQry = "select \"DI_ID\",\"DI_NO\",\"DI_CONSIGNEE\",TO_CHAR(\"DI_DUEDATE\",'dd-MM-yyyy')\"DI_DUEDATE\",\"DI_STORE_ID\",TO_CHAR(\"DI_DATE\",'dd-MM-yyyy')\"DI_DATE\",\"DI_MAKE_ID\", \"DI_CAPACITY\",\"DI_CAPACITY_ID\", \"DI_STARTTYPE\", \"DI_QUANTITY\"  from \"TBLDELIVERYINSTRUCTION\" ";
                strQry += " WHERE  \"DI_NO\"=:DINo";

                dtIndentDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtIndentDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
            }
        }

        public DataTable GetDeliveredDetails(string sDI_no)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("DIno", sDI_no.ToUpper());

                sQry = " SELECT \"DI_ID\",\"DI_NO\",\"DI_PO_ID\", TO_CHAR(\"DI_DATE\",'DD/MM/YYYY') AS \"DI_DATE\", TO_CHAR(\"DI_DUEDATE\",'DD/MM/YYYY') AS \"DI_DUEDATE\",\"DI_CONSIGNEE\",\"DI_STORE_ID\",\"SM_NAME\" AS \"DI_STORE\",\"DI_MAKE_ID\",\"TM_NAME\" AS \"DI_MAKE\",\"DI_CAPACITY\",\"DI_CAPACITY_ID\",";
                sQry += "\"MD_NAME\"AS \"DI_STARRATENAME\", \"DI_STARTTYPE\" AS \"DI_STARRATE\", \"DI_QUANTITY\" ,\"DI_FILE_EXT\" FROM  \"TBLDELIVERYINSTRUCTION\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLMASTERDATA\" WHERE \"DI_STORE_ID\"=\"SM_ID\" AND";
                sQry += " \"TM_ID\"=\"DI_MAKE_ID\" AND \"MD_TYPE\"='SR' AND \"DI_STATUS\"=1 AND \"MD_ID\"=\"DI_STARTTYPE\" and \"DI_NO\" LIKE :DIno||'%'  ORDER BY \"DI_ID\"";
                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public DataTable GetDeliveryDetails(clsDelivery objDel)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;
               
                sQry = "SELECT \"PB_MAKE\" ,\"PO_NO\", \"TM_NAME\" \"MAKE\",\"PB_CAPACITY\" \"CAPACITY\", \"DI_CAPACITY_ID\", \"PO_RATING\", \"MD_NAME\" \"RATING\", ";
                sQry += "(\"PB_QUANTITY\") \"TOTAL\", COALESCE((\"PB_QUANTITY\") - SUM(COALESCE(\"DI_QUANTITY\",0)),0) \"PENDING\" FROM \"TBLPOMASTER\" ";
                sQry += " INNER JOIN \"TBLPOOBJECTS\" ON \"PO_ID\" = \"PB_PO_ID\" LEFT JOIN \"TBLDELIVERYINSTRUCTION\" ON \"PO_ID\" = \"DI_PO_ID\" AND \"DI_STATUS\"=1 AND ";
                sQry += " \"PB_MAKE\" = \"DI_MAKE_ID\" AND \"PB_CAPACITY\" = \"DI_CAPACITY\"  AND \"PO_RATING\" = CAST(\"DI_STARTTYPE\" AS TEXT) JOIN ";
                sQry += " (SELECT \"MD_ID\", \"MD_NAME\" FROM \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'SR')A ON CAST(\"MD_ID\" AS TEXT) = \"PO_RATING\" JOIN ";
                sQry += " \"TBLTRANSMAKES\" ON \"TM_ID\" = \"PB_MAKE\" WHERE ";

                if (objDel.sPONumber != "" && objDel.sPONumber !=null  )
                {
                    NpgsqlCommand.Parameters.AddWithValue("PoNO", objDel.sPONumber);
                    sQry += " cast(\"PO_NO\" as text)=:PoNO AND  "; 
                }
                if (objDel.sCapacity != "" && objDel.sCapacity != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("Capacty", objDel.sCapacity);
                    sQry += " cast(\"PB_CAPACITY\" as text)=:Capacty AND  ";
                }
                if (objDel.sPoId != "" && objDel.sPoId !=null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("PoID", objDel.sPoId);
                    sQry += "cast(\"PO_ID\" as text)=:PoID AND "; 
                }
                sQry += " \"TM_ID\" = \"PB_MAKE\" AND \"PB_PO_STATUS\"=1 GROUP BY  \"PO_NO\",\"PB_MAKE\", \"TM_NAME\",\"PB_CAPACITY\",\"DI_CAPACITY_ID\",\"PO_RATING\", \"MD_NAME\",\"PB_CAPACITY\",\"PB_QUANTITY\"";
                     

                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                return dt;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public string[] SaveDeliveryDetails(clsDelivery objdelivery)
        {
          PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            try
            {                
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                dt = objdelivery.dtDelivery;
                    string str;
                                  
                for(int i=0; i<dt.Rows.Count; i++)          
                {
                    string sDi_Id = Convert.ToString(dt.Rows[i]["DI_ID"]);
                    string sDiNo = Convert.ToString(dt.Rows[i]["DI_NO"]);
                    string sPoNo = Convert.ToString(dt.Rows[i]["DI_PO_ID"]);
                    string sStoreId = Convert.ToString(dt.Rows[i]["DI_STORE_ID"]);
                    string sDueDate = Convert.ToString(dt.Rows[i]["DI_DUEDATE"]);
                    string sMake = Convert.ToString(dt.Rows[i]["DI_MAKE_ID"]);
                    string sCapacity = Convert.ToString(dt.Rows[i]["DI_CAPACITY"]);
                    string sCapacityID = Convert.ToString(dt.Rows[i]["DI_CAPACITY_ID"]);
                    string sStartype = Convert.ToString(dt.Rows[i]["DI_STARRATE"]);
                    string sQuantity = Convert.ToString(dt.Rows[i]["DI_QUANTITY"]);
                    string sConsignee = Convert.ToString(dt.Rows[i]["DI_CONSIGNEE"]);
                    string sDiDate = Convert.ToString(dt.Rows[i]["DI_DATE"]);
                    string sFileExt = Convert.ToString(dt.Rows[i]["DI_FILE_EXT"]);
                    long Id = objcon.Get_max_no("DI_ID", "TBLDELIVERYINSTRUCTION");
                   
                    if (sDi_Id =="")
                    {
                        //Objcon.BeginTransaction();
                        NpgsqlCommand Npgsql = new NpgsqlCommand();
                        Npgsql.Parameters.AddWithValue("DI_no", sDiNo.ToUpper());
                        Npgsql.Parameters.AddWithValue("poId", sPoNo.ToUpper());
                        str = objcon.get_value(" select * from \"TBLDELIVERYINSTRUCTION\" where cast(\"DI_NO\"as text)=:DI_no  AND cast(\"DI_PO_ID\" as text)<> :poId", Npgsql);
                        if (str != null && str != "")
                        {

                            Arr[0] = "Entered DI Number Already mapped with some other PO Number";
                            Arr[1] = "2";
                            return Arr;
                        }
                        NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_dimaster");

                        cmd.Parameters.AddWithValue("di_id", sDi_Id);  
                        cmd.Parameters.AddWithValue("di_number", sDiNo.ToUpper());
                        cmd.Parameters.AddWithValue("di_consignee", sConsignee);
                        cmd.Parameters.AddWithValue("di_po_id", sPoNo);
                        cmd.Parameters.AddWithValue("di_crby", objdelivery.sCrby);
                        cmd.Parameters.AddWithValue("di_date", sDiDate);
                        cmd.Parameters.AddWithValue("di_store_id",sStoreId);
                        cmd.Parameters.AddWithValue("di_star_rate", sStartype);
                        cmd.Parameters.AddWithValue("di_duedate", sDueDate);
                        cmd.Parameters.AddWithValue("di_make_id", sMake);
                        cmd.Parameters.AddWithValue("di_quantity",sQuantity);
                        cmd.Parameters.AddWithValue("di_capacity",sCapacity);
                        cmd.Parameters.AddWithValue("di_capacity_id", sCapacityID);
                        cmd.Parameters.AddWithValue("di_status", "1");
                              
                       // cmd.Parameters.AddWithValue("FileExt", sFileExt);
                        cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                        cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                        cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);

                        cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                        cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                        cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                        Arr[0] = "msg";
                        Arr[1] = "op_id";
                        Arr[2] = "pk_id";
                        Arr = objcon.Execute(cmd, Arr, 3);
                        objdelivery.sDIid = Arr[2].ToString();

                        //sQry = "INSERT INTO \"TBLDELIVERYINSTRUCTION\" (\"DI_ID\", \"DI_NO\", \"DI_PO_ID\", \"DI_STORE_ID\", \"DI_DUEDATE\", \"DI_MAKE_ID\", ";
                        //sQry += " \"DI_CAPACITY\", \"DI_STARTTYPE\", \"DI_QUANTITY\",\"DI_CRON\", \"DI_CRBY\",\"DI_DATE\",\"DI_CONSIGNEE\",\"DI_FILE_EXT\",\"DI_CAPACITY_ID\")";
                        //sQry += " VALUES((SELECT COALESCE(MAX(\"DI_ID\"),0)+1 FROM \"TBLDELIVERYINSTRUCTION\"),:DiNo,:PoNo,:StoreId, ";
                        //sQry += " TO_DATE(:DueDate,'DD/MM/YYYY'),:Make,:Capacity,:Startype,:Quantity,NOW(),:Crby,";
                        //sQry += "  TO_DATE(:DiDate,'DD/MM/YYYY') ,:Consignee,:FileExt,:CapacityID)";
                        //objcon.ExecuteQry(sQry, cmd);


                    }
                    else
                    {
                     NpgsqlCommand.Parameters.AddWithValue("DiNO", sDiNo.ToUpper());
                    // str = "DELETE FROM \"TBLDELIVERYINSTRUCTION\" WHERE \"DI_NO\"=:DiNO";
                     str = "UPDATE \"TBLDELIVERYINSTRUCTION\" SET \"DI_STATUS\"=0 WHERE \"DI_NO\"=:DiNO";
                    objcon.ExecuteQry(str, NpgsqlCommand);


                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        sDi_Id = Convert.ToString(dt.Rows[j]["DI_ID"]);
                        sDiNo = Convert.ToString(dt.Rows[j]["DI_NO"]);
                        sConsignee = Convert.ToString(dt.Rows[j]["DI_CONSIGNEE"]);
                        sPoNo = Convert.ToString(dt.Rows[j]["DI_PO_ID"]);
                        sStoreId = Convert.ToString(dt.Rows[j]["DI_STORE_ID"]);
                        sDueDate = Convert.ToString(dt.Rows[j]["DI_DUEDATE"]);
                        sMake = Convert.ToString(dt.Rows[j]["DI_MAKE_ID"]);
                        sCapacity = Convert.ToString(dt.Rows[j]["DI_CAPACITY"]);
                        sCapacityID = Convert.ToString(dt.Rows[j]["DI_CAPACITY_ID"]);
                        sStartype = Convert.ToString(dt.Rows[j]["DI_STARRATE"]);
                        sQuantity = Convert.ToString(dt.Rows[j]["DI_QUANTITY"]);
                        sDiDate = Convert.ToString(dt.Rows[j]["DI_DATE"]);
                        sFileExt = Convert.ToString(dt.Rows[j]["DI_FILE_EXT"]);

                        NpgsqlCommand cmd1 = new NpgsqlCommand("proc_saveupdate_dimaster");

                        cmd1.Parameters.AddWithValue("di_id", sDi_Id);
                        cmd1.Parameters.AddWithValue("di_number", sDiNo.ToUpper());
                        cmd1.Parameters.AddWithValue("di_consignee", sConsignee);
                        cmd1.Parameters.AddWithValue("di_po_id", sPoNo);
                        cmd1.Parameters.AddWithValue("di_crby", objdelivery.sCrby);
                        cmd1.Parameters.AddWithValue("di_date", sDiDate);
                        cmd1.Parameters.AddWithValue("di_store_id", sStoreId);
                        cmd1.Parameters.AddWithValue("di_star_rate", sStartype);
                        cmd1.Parameters.AddWithValue("di_duedate", sDueDate);
                        cmd1.Parameters.AddWithValue("di_make_id", sMake);
                        cmd1.Parameters.AddWithValue("di_quantity", sQuantity);
                        cmd1.Parameters.AddWithValue("di_capacity", sCapacity);
                        cmd1.Parameters.AddWithValue("di_capacity_id", sCapacityID);
                        cmd1.Parameters.AddWithValue("di_status", "1");
                              
                       // cmd1.Parameters.AddWithValue("FileExt", sFileExt);

                        cmd1.Parameters.Add("msg", NpgsqlDbType.Text);
                        cmd1.Parameters.Add("op_id", NpgsqlDbType.Text);
                        cmd1.Parameters.Add("pk_id", NpgsqlDbType.Text);

                        cmd1.Parameters["msg"].Direction = ParameterDirection.Output;
                        cmd1.Parameters["op_id"].Direction = ParameterDirection.Output;
                        cmd1.Parameters["pk_id"].Direction = ParameterDirection.Output;
                        Arr[0] = "msg";
                        Arr[1] = "op_id";
                        Arr[2] = "pk_id";
                        Arr = objcon.Execute(cmd1, Arr, 3);
                        objdelivery.sDIid = Arr[2].ToString();


                        //sQry = "INSERT INTO \"TBLDELIVERYINSTRUCTION\" (\"DI_ID\", \"DI_NO\", \"DI_PO_ID\", \"DI_STORE_ID\", \"DI_DUEDATE\", \"DI_MAKE_ID\", ";
                        //sQry += " \"DI_CAPACITY\", \"DI_STARTTYPE\", \"DI_QUANTITY\",\"DI_CRON\", \"DI_CRBY\",\"DI_DATE\",\"DI_CONSIGNEE\",\"DI_FILE_EXT\",\"DI_CAPACITY_ID\")";
                        //sQry += " VALUES((SELECT COALESCE(MAX(\"DI_ID\"),0)+1 FROM \"TBLDELIVERYINSTRUCTION\"),:DiNo,:PoNo,:StoreId, ";
                        //sQry += " TO_DATE(:DueDate,'DD/MM/YYYY'),:Make,:Capacity,:Startype,:Quantity,NOW(),:Crby,";
                        //sQry += "  TO_DATE(:DiDate,'DD/MM/YYYY') ,:Consignee,:FileExt,:CapacityID)";

                        //objcon.ExecuteQry(sQry, cmd1);
                        if (objdelivery.sFileExt.Length > 0)
                        {
                            NpgsqlParameter DeliveryNote = new NpgsqlParameter();
                            NpgsqlCommand comd = new NpgsqlCommand();


                            sQry = " UPDATE \"TBLDELIVERYINSTRUCTION\" SET \"DI_FILE_EXT\"='" + objdelivery.sFileExt + "' WHERE \"DI_ID\" = '" + objdelivery.sDIid + "'";
                            NpgsqlConnection objconn = new NpgsqlConnection();
                            string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + "Idea@12345+";
                            objconn.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=TestTRM;User Id=postgres;Password =Idea123";  
                            objconn.Open();
                            objcon.ExecuteQry(sQry, NpgsqlCommand);
                            //comd = new NpgsqlCommand(sQry, objconn);
                            //comd.Parameters.Add(DeliveryNote);
                            //comd.ExecuteNonQuery();
                            objconn.Close();
                        }

                    }
                    Arr[0] = "Updated Successfully";
                    Arr[1] = "0";
                    return Arr;
                        
                    }

                    if (objdelivery.sFileExt.Length > 0)
                    {
                        NpgsqlParameter DeliveryNote = new NpgsqlParameter();
                        NpgsqlCommand comd = new NpgsqlCommand();
                         
                        sQry = " UPDATE \"TBLDELIVERYINSTRUCTION\" SET \"DI_FILE_EXT\"='" + objdelivery.sFileExt + "' WHERE \"DI_ID\" = '" + objdelivery.sDIid + "'";
                        NpgsqlConnection objconn = new NpgsqlConnection();
                        string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + "Idea@12345+";
                        objconn.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=TestTRM;User Id=postgres;Password =Idea123";  
                        objconn.Open();
                        objcon.ExecuteQry(sQry, NpgsqlCommand);
                        //comd = new NpgsqlCommand(sQry, objconn);
                        //comd.Parameters.Add(DeliveryNote);
                        //comd.ExecuteNonQuery();
                        objconn.Close();
                    }
                   

                }
                Arr[0] = "Saved Successfully";
                Arr[1] = "0";
                return Arr;

            }
            catch(Exception ex)
            {
                Arr[0] = "Failed to Save";
                Arr[1] = "1";
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public object GetPOImage(clsDelivery obj)
        {
            NpgsqlCommand = new NpgsqlCommand();
            Byte[] POImage = null;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("phno", obj.sPONumber);
                string sQry = "SELECT \"PO_DOC\", \"PO_DOC_EXT\" FROM \"TBLPOMASTER\" WHERE  \"PO_DOC\" IS NOT NULL AND \"PO_ID\" = (SELECT DISTINCT \"DI_PO_ID\" FROM  \"TBLDELIVERYINSTRUCTION\" WHERE \"DI_NO\" =:phno)";
                DataTable dt = new DataTable();
                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                if(dt.Rows.Count > 0)
                {
                    Byte[] bytes = (Byte[])dt.Rows[0]["PO_DOC"];
                    obj.sFileExt = Convert.ToString(dt.Rows[0]["PO_DOC_EXT"]);
                    obj.POFile = bytes;
                }    
                else
                {
                    obj.sFileExt = "";
                    obj.POFile = null;
                }           
                return obj;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return obj;
            }
        }

        public object GetDIImage(clsDelivery obj)
        {
            NpgsqlCommand = new NpgsqlCommand();
            Byte[] POImage = null;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("phno", obj.sPONumber);
                string sQry = "SELECT DISTINCT \"DI_INST_FILE\", \"DI_INST_FILE_EXT\" FROM \"TBLDELIVERYINSTRUCTION\" WHERE \"DI_NO\" =:phno AND \"DI_INST_FILE\" IS NOT NULL";
                DataTable dt = new DataTable();
                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {
                    Byte[] bytes = (Byte[])dt.Rows[0]["DI_INST_FILE"];
                    obj.sFileExt = Convert.ToString(dt.Rows[0]["DI_INST_FILE_EXT"]);
                    obj.POFile = bytes;
                }
                else
                {
                    obj.sFileExt = "";
                    obj.POFile = null;
                }
                return obj;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return POImage;
            }
        }
    }
}
