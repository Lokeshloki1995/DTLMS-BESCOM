using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;

namespace IIITS.DTLMS.BL
{
    public class clsPoMaster
    {
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        string strFormCode = "clsPoMaster";
        public DataTable ddtCapacityGrid { get; set; }
        public string sPoId { get; set; }
        public string sPoNo { get; set; }
        public string sDate { get; set; }
        public string sTcCapacity { get; set; }
        public string sTcMake { get; set; }
        public string sTcQuantity { get; set; }
        public string sCapacity { get; set; }
        public string sSupplierId { get; set; }
        public string sPoRate { get; set; }
        public string sPoDlvrdate { get; set; }
        public string sPbId { get; set; }
        public string sCrBy { get; set; }
        public string sFileName { get; set; }
        public string sDeliveryDate { get; set; }
        public string sFileExtension { get; set; }
        public Int32  sRowIndex { get; set; }
        NpgsqlCommand NpgsqlCommand;
        public string[] SavePoMaster(clsPoMaster objPoMaster,byte[] Buffer)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            string[] Arr = new string[3];
            string str;
            byte[] imageData = null;
            NpgsqlParameter docPhoto = new NpgsqlParameter();
            NpgsqlCommand comd = new NpgsqlCommand();
            try
            {
                if (objPoMaster.sPoId == "")
                {
                    //Objcon.BeginTransaction();
                    NpgsqlCommand.Parameters.AddWithValue("pono", objPoMaster.sPoNo.ToUpper());
                    str = Objcon.get_value("select \"PO_NO\" from \"TBLPOMASTER\" where\"PO_NO\"=:pono", NpgsqlCommand);
                    if (str != null && str != "")
                    {
                        
                        Arr[0] = "Entered PO Number Already Exists";
                        Arr[1] = "2";
                        return Arr;
                    }
                    

                    imageData = Buffer;
                    if (imageData != null)
                    {
                        docPhoto.ParameterName = "Document";
                        docPhoto.Value = imageData;
                    }
                    else
                    {
                        docPhoto.ParameterName = "Document";
                        docPhoto.Value = null;
                    }

                    //objPoMaster.sPoId = Convert.ToString(objCon.Get_max_no("PO_ID", "TBLPOMASTER"));
                    //strQry = "INSERT INTO TBLPOMASTER(PO_ID,PO_NO,PO_DATE,PO_CRBY,PO_CRON,PO_SUPPLIER_ID,PO_RATE)";
                    //strQry += " VALUES('" + objPoMaster.sPoId + "','" + objPoMaster.sPoNo + "',TO_DATE('" + objPoMaster.sDate + "','DD/MM/YYYY')";
                    //strQry += ",'" + objPoMaster.sCrBy + "',SYSDATE,'" + objPoMaster.sSupplierId + "','"+ objPoMaster.sPoRate +"')";
                    //objCon.Execute(strQry);

                    NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_po_master");
                    cmd.Parameters.AddWithValue("pomaster_id", objPoMaster.sPoId);
                    cmd.Parameters.AddWithValue("pomaster_number", objPoMaster.sPoNo.ToUpper());
                    cmd.Parameters.AddWithValue("pomaster_crby", objPoMaster.sCrBy);
                    cmd.Parameters.AddWithValue("pomaster_date", objPoMaster.sDate);
                    cmd.Parameters.AddWithValue("pomaster_supplier_id", objPoMaster.sSupplierId);
                    cmd.Parameters.AddWithValue("pomaster_rate", objPoMaster.sPoRate);
                    cmd.Parameters.AddWithValue("podelivery_date", objPoMaster.sPoDlvrdate);



                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);

                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                    Arr[0] = "msg";
                    Arr[1] = "op_id";
                    Arr[2] = "pk_id";
                    Arr = Objcon.Execute(cmd, Arr, 3);
                    objPoMaster.sPoId = Arr[2].ToString();


                    strQry = " UPDATE \"TBLPOMASTER\" SET \"PO_DOC_EXT\"='" + objPoMaster.sFileExtension + "'   WHERE \"PO_ID\" = '" + objPoMaster.sPoId + "'";
                    NpgsqlConnection objconn = new NpgsqlConnection();
                    string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + "Idea@12345+";
                    objconn.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=TestTRM;User Id=postgres;Password =Idea123";  
                    objconn.Open();
                    comd = new NpgsqlCommand(strQry, objconn);
                    comd.Parameters.Add(docPhoto);
                    comd.ExecuteNonQuery();
                    objconn.Close();


                    for (int i = 0; i < objPoMaster.ddtCapacityGrid.Rows.Count; i++)
                    {
                        string[] Arr1 = new string[3];
                        NpgsqlCommand cmd1 = new NpgsqlCommand("proc_saveupdate_po_objects");
                        objPoMaster.sPbId = Convert.ToString(Objcon.Get_max_no("PB_ID", "TBLPOOBJECTS"));
                        //strQry = "INSERT INTO TBLPOOBJECTS(PB_ID,PB_PO_ID,PB_MAKE,PB_CAPACITY,PB_QUANTITY,PB_CRBY,PB_CRON)";
                        //strQry += " VALUES('" + objPoMaster.sPbId + "','" + objPoMaster.sPoId + "','" + objPoMaster.ddtCapacityGrid.Rows[i]["MAKE_ID"] + "'";
                        //strQry += ",'" + objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY"] + "','" + objPoMaster.ddtCapacityGrid.Rows[i]["PB_QUANTITY"] + "','" + objPoMaster.sCrBy + "',SYSDATE)";
                        //objCon.Execute(strQry);
                        cmd1.Parameters.AddWithValue("pbobj_id", " ");
                        cmd1.Parameters.AddWithValue("pbobj_mas_id", objPoMaster.sPoId);
                        cmd1.Parameters.AddWithValue("pbobj_make", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["MAKE_ID"]));
                        cmd1.Parameters.AddWithValue("pbobj_capacity", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY"]));
                        cmd1.Parameters.AddWithValue("pbobj_capacityid", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY_ID"]));
                        cmd1.Parameters.AddWithValue("pbobj_quantity", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_QUANTITY"]));
                        cmd1.Parameters.AddWithValue("pbobj_crby", objPoMaster.sCrBy);
                        cmd1.Parameters.AddWithValue("pbobj_rating", objPoMaster.ddtCapacityGrid.Rows[i]["PB_STARRATE"]);
                        cmd1.Parameters.AddWithValue("pbobj_status", "1");
                        Objcon.Execute(cmd1, Arr1, 0);
                    }
                    //Objcon.CommitTransaction();
                    Arr[0] = "Saved Successfully";
                    Arr[1] = "0";
                    return Arr;

                }
                else
                {
                    //Objcon.BeginTransaction();
                    NpgsqlCommand.Parameters.AddWithValue("PoId",Convert.ToInt32(objPoMaster.sPoId));
                    strQry = "UPDATE \"TBLPOOBJECTS\" SET \"PB_PO_STATUS\"=0 WHERE \"PB_PO_ID\"=:PoId";
                    Objcon.ExecuteQry(strQry, NpgsqlCommand);

                    NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_po_master");
                    cmd.Parameters.AddWithValue("pomaster_id", objPoMaster.sPoId);
                    cmd.Parameters.AddWithValue("pomaster_number", objPoMaster.sPoNo.ToUpper());
                    cmd.Parameters.AddWithValue("pomaster_crby", objPoMaster.sCrBy);
                    cmd.Parameters.AddWithValue("pomaster_date", objPoMaster.sDate);
                    cmd.Parameters.AddWithValue("pomaster_supplier_id", objPoMaster.sSupplierId);
                    cmd.Parameters.AddWithValue("pomaster_rate", objPoMaster.sPoRate);
                    cmd.Parameters.AddWithValue("podelivery_date", objPoMaster.sPoDlvrdate);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);

                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                    Arr[0] = "msg";
                    Arr[1] = "op_id";
                    Arr[2] = "pk_id";
                    Arr = Objcon.Execute(cmd, Arr, 3);
                    objPoMaster.sPoId = Arr[2].ToString();

                    for (int i = 0; i < objPoMaster.ddtCapacityGrid.Rows.Count; i++)
                    {

                        objPoMaster.sPbId = Convert.ToString(Objcon.Get_max_no("PB_ID", "TBLPOOBJECTS"));
                        //strQry = "INSERT INTO TBLPOOBJECTS(PB_ID,PB_PO_ID,PB_MAKE,PB_CAPACITY,PB_QUANTITY,PB_CRBY,PB_CRON)";
                        //strQry += " VALUES('" + objPoMaster.sPbId + "','" + objPoMaster.sPoId + "','" + objPoMaster.ddtCapacityGrid.Rows[i]["MAKE_ID"] + "'";
                        //strQry += ",'" + objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY"] + "','" + objPoMaster.ddtCapacityGrid.Rows[i]["PB_QUANTITY"] + "','" + objPoMaster.sCrBy + "',SYSDATE)";
                        //objCon.Execute(strQry);
                        string[] Arr1 = new string[3];
                        NpgsqlCommand cmd1 = new NpgsqlCommand("proc_saveupdate_po_objects");
                        cmd1.Parameters.AddWithValue("pbobj_id", " ");
                        cmd1.Parameters.AddWithValue("pbobj_mas_id", objPoMaster.sPoId);
                        cmd1.Parameters.AddWithValue("pbobj_make", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["MAKE_ID"]));
                        cmd1.Parameters.AddWithValue("pbobj_capacity", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY"]));
                        cmd1.Parameters.AddWithValue("pbobj_capacityid", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_CAPACITY_ID"]));
                        cmd1.Parameters.AddWithValue("pbobj_quantity", Convert.ToString(objPoMaster.ddtCapacityGrid.Rows[i]["PB_QUANTITY"]));
                        cmd1.Parameters.AddWithValue("pbobj_crby", objPoMaster.sCrBy);
                        cmd1.Parameters.AddWithValue("pbobj_rating", objPoMaster.ddtCapacityGrid.Rows[i]["PB_STARRATE"]);
                        cmd1.Parameters.AddWithValue("pbobj_status","1");
                        Objcon.Execute(cmd1, Arr1, 0);
                    }

                    //Objcon.CommitTransaction();
                    //Arr[0] = "Updated Successfully";
                    //Arr[1] = "0";
                    return Arr;

                }
            }

            catch (Exception ex)
            {
                // Objcon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public DataTable GetPODoc(string sPoid)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable DtPODoc = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("poid",Convert.ToInt32(sPoid));
                string strqry = "SELECT \"PO_DOC\",\"PO_DOC_EXT\" FROM \"TBLPOMASTER\" WHERE \"PO_ID\"=:poid";
                return Objcon.FetchDataTable(strqry, NpgsqlCommand);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtPODoc;
            }
        }

        public DataTable LoadPoDetailGrid(clsPoMaster objPoMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtPoDetails = new DataTable();
            try
            {
                
                strQry = "SELECT \"PO_ID\",\"PO_NO\",TO_CHAR(\"PO_DATE\",'dd-MON-yyyy') \"PO_DATE\",SUM(\"PB_QUANTITY\")\"PB_QUANTITY\",(SELECT \"TS_NAME\" FROM \"TBLTRANSSUPPLIER\" WHERE \"TS_ID\"=\"PO_SUPPLIER_ID\")";
                strQry += " \"PO_SUPPLIER_ID\" FROM \"TBLPOMASTER\",";
                strQry += " \"TBLPOOBJECTS\" WHERE \"PO_ID\"=\"PB_PO_ID\" and \"PB_PO_STATUS\"=1 ";
                if (objPoMaster.sPoNo != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("pono", objPoMaster.sPoNo.ToUpper());
                    strQry += " and  UPPER(\"PO_NO\") like :pono||'%'";
                }


                strQry += " GROUP BY \"PO_ID\",\"PO_NO\",\"PO_DATE\",\"PO_SUPPLIER_ID\"";

                dtPoDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);
                return dtPoDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtPoDetails;
            }
        }


        public object GetPoDetails(clsPoMaster objPoMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            try
            {
                DataTable dtIndentDetails = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("poid",Convert.ToInt32(objPoMaster.sPoId));
                strQry = "select \"PO_ID\",\"PO_NO\",TO_CHAR(\"PO_DATE\",'dd/MM/yyyy')\"PO_DATE\",\"PO_SUPPLIER_ID\",\"PO_RATE\",TO_CHAR(\"PO_DLVR_SCLD\",'dd/MM/yyyy') \"PO_DLVR_SCLD\" from \"TBLPOMASTER\" ";
                strQry += " WHERE  \"PO_ID\"=:poid";

                dtIndentDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);
                objPoMaster.sPoId = Convert.ToString(dtIndentDetails.Rows[0]["PO_ID"]);
                objPoMaster.sPoNo = Convert.ToString(dtIndentDetails.Rows[0]["PO_NO"]);
                objPoMaster.sDate = Convert.ToString(dtIndentDetails.Rows[0]["PO_DATE"]);
                objPoMaster.sSupplierId = Convert.ToString(dtIndentDetails.Rows[0]["PO_SUPPLIER_ID"]);
                objPoMaster.sPoRate = Convert.ToString(dtIndentDetails.Rows[0]["PO_RATE"]);
                objPoMaster.sDeliveryDate = Convert.ToString(dtIndentDetails.Rows[0]["PO_DLVR_SCLD"]);
                return objPoMaster;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objPoMaster;
            }
        }
        public DataTable LoadDeliveredCount(clsPoMaster objPoMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtCapacityDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("poid", Convert.ToInt16(objPoMaster.sPoId));
                strQry = "select \"DI_NO\", sum(\"DI_QUANTITY\") as \"DELIVERED\" from \"TBLDELIVERYINSTRUCTION\" where \"DI_PO_ID\"=:poid  and \"DI_CAPACITY\"=" + objPoMaster.sCapacity + " AND \"DI_STATUS\"=1  AND \"DI_MAKE_ID\"=" + objPoMaster.sTcMake+ " GROUP BY \"DI_NO\"";                
                dtCapacityDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtCapacityDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCapacityDetails;

            }
        }
        public DataTable LoadCapacityGrid(clsPoMaster objPoMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtCapacityDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("poid",Convert.ToInt32( objPoMaster.sPoId));
                strQry = "select \"PO_ID\",\"PO_NO\",TO_CHAR(\"PO_DATE\",'dd-MON-yyyy') PO_DATE,(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\"=\"PB_MAKE\") PB_MAKE ,\"PB_MAKE\" AS \"MAKE_ID\", CAST(\"PB_CAPACITY\" as TEXT) as PB_CAPACITY ,";
                strQry += " \"PB_QUANTITY\",\"PB_CAPACITY_ID\",\"PB_PO_ID\",\"PB_ID\",\"PO_RATING\" \"PB_STARRATE\",\"MD_NAME\" AS \"PB_STAR_NAME\" from \"TBLPOMASTER\",\"TBLMASTERDATA\",\"TBLPOOBJECTS\"  WHERE \"PO_ID\"= \"PB_PO_ID\" and \"MD_TYPE\"='SR' and cast(\"PO_RATING\" as int8)=\"MD_ID\" and \"PO_ID\"=:poid and \"PB_PO_STATUS\"=1 ";
                dtCapacityDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtCapacityDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtCapacityDetails;

            }
        }
    }
}
