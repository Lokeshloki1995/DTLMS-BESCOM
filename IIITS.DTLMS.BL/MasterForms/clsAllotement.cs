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
  public  class clsAllotement
    {
        public string sTotalTC { get; set; }
        public string sCapacity { get; set; }
        public string sStoreId { get; set; }
        public string sRatingId { get; set; }
        public string sDivId { get; set; }
        public string sMakeId { get; set; }
        public string sDiId { get; set; }
        public string sALTid { get; set; }
        public string sDINo { get; set; }
        public string sALTNumber { get; set; }
        public DataTable dtAllotement { get; set; }
        public Byte[] ALTFile { get; set; }
        public string sFileExt { get; set; }
        public string sCrby { get; set; }

        public string strFormCode = "clsAllotement";

        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;

        public object GetDispatchCount(clsAllotement obj)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("DIno", obj.sDINo);
                sQry = "SELECT SUM(\"DI_QUANTITY\") FROM \"TBLDELIVERYINSTRUCTION\"  WHERE \"DI_NO\" =:DIno  and \"DI_STATUS\"=1 ";
                if (obj.sCapacity != "" && obj.sCapacity != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("Capacity", obj.sCapacity);
                    sQry += " AND cast(\"DI_CAPACITY\" as text)=:Capacity ";
                }
                obj.sTotalTC = objcon.get_value(sQry, NpgsqlCommand);              
                return obj;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public DataTable GetInwardedCount(clsAllotement objAllot)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtIndentDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("AltNo", Convert.ToString(objAllot.sALTNumber));

                strQry = " select  COUNT(\"TC_CAPACITY\") as \"INWARDED\" from \"TBLTCMASTER\" where \"TC_ALT_NO\"=:AltNo  and \"TC_CAPACITY\"='" + objAllot.sCapacity + "'  AND  \"TC_MAKE_ID\" ='" + objAllot.sMakeId + "' and \"TC_STORE_ID\"= '" + objAllot.sStoreId + "' AND \"TC_DIV_ID\"='" + objAllot.sDivId + "'   GROUP BY \"TC_CAPACITY\"";

                dtIndentDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtIndentDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
            }
        }
        public DataTable GetAllotedDetails(string sAlt_no)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("Altno", sAlt_no.ToUpper());


                sQry = "  select \"ALT_ID\",\"DI_NO\" AS \"ALT_DI_NO\",\"ALT_NO\",TO_CHAR(\"ALT_DATE\",'DD/MM/YYYY') AS \"ALT_DATE\",\"ALT_STORE_ID\",\"SM_NAME\" AS \"ALT_STORE_NAME\",\"DIV_NAME\" AS \"DIV_NAME\",\"ALT_CAPACITY\",\"ALT_CAPACITY_ID\",\"TM_NAME\" AS \"MAKE_NAME\",\"ALT_STAR_TYPE\" ,\"MD_NAME\" AS \"ALT_STARRATENAME\" ,";
                sQry += " \"ALT_DIV_ID\",\"ALT_MAKE_ID\",\"ALT_QUANTITY\" FROM  \"TBLDIVISION\", \"TBLALLOTEMENT\",\"TBLDELIVERYINSTRUCTION\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLMASTERDATA\" WHERE \"ALT_STAR_TYPE\"=\"MD_ID\" AND \"MD_TYPE\"='SR' AND \"DI_NO\"=\"ALT_DI_NO\"  AND \"TM_ID\"=\"ALT_MAKE_ID\"   AND\"ALT_DIV_ID\"=\"DIV_ID\" AND \"ALT_STATUS\"=1  AND \"SM_ID\"=\"ALT_STORE_ID\" " ;

                if (sAlt_no != "" && sAlt_no != null)
                {
                    sQry += "  and \"ALT_NO\"= :Altno ";
                }
                else
                {
                    sQry += "  and \"ALT_NO\" like '%' ";
                }                                              
               sQry += " GROUP BY  \"ALT_ID\",\"DI_NO\",\"ALT_NO\",\"ALT_DATE\",\"ALT_STORE_ID\",\"SM_NAME\",\"DIV_NAME\" ,\"ALT_CAPACITY\",\"ALT_CAPACITY_ID\",\"MD_NAME\" ,\"ALT_MAKE_ID\" ,\"TM_NAME\" ,\"ALT_STAR_TYPE\",\"ALT_QUANTITY\",\"ALT_DIV_ID\"  ORDER BY \"ALT_ID\"";
                //sQry = " SELECT \"DI_ID\",\"DI_NO\",\"DI_PO_ID\", TO_CHAR(\"DI_DATE\",'DD/MON/YYYY') AS \"DI_DATE\",\"SM_NAME\" AS \"STORE_NAME\",\"TM_NAME\" AS \"MAKE_NAME\",\"DI_CAPACITY\",";
                //sQry += "\"MD_NAME\"AS \"STAR_RATE\", \"DI_QUANTITY\" FROM  \"TBLDELIVERYINSTRUCTION\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLMASTERDATA\" WHERE \"DI_STORE_ID\"=\"SM_ID\" AND";
                //sQry += " \"TM_ID\"=\"DI_MAKE_ID\" AND \"MD_TYPE\"='SR' AND \"MD_ID\"=\"DI_STARTTYPE\" and \"DI_NO\" LIKE :DIno||'%'  ORDER BY \"DI_ID\"";
                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        public DataTable GetDeliveryDetails(clsAllotement objAllot)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string sQry = string.Empty;

                sQry = " SELECT a.\"DI_ID\",A.\"DI_NO\",SUM(a.\"DI_QUANTITY\") AS \"TOTAL\" ,COALESCE(SUM(a.\"DI_QUANTITY\") - SUM(COALESCE(b.\"ALT_QUANTITY\",0)),0) \"PENDING\",a.\"DI_MAKE_ID\",";
                sQry += " a.\"SM_NAME\" AS \"STORE_NAME\", a.\"TM_NAME\" as \"MAKE_NAME\",a.\"DI_CAPACITY\" ,a.\"MD_NAME\"AS \"STAR_RATE\"from (SELECT SUM(\"DI_QUANTITY\") as \"DI_QUANTITY\",\"DI_ID\" ,\"DI_NO\", \"SM_NAME\",\"DI_STATUS\",";
                sQry += " \"DI_MAKE_ID\",\"DI_STORE_ID\",\"TM_NAME\",\"DI_CAPACITY\",\"DI_CAPACITY_ID\",\"MD_NAME\",\"DI_STARTTYPE\" FROM \"TBLDELIVERYINSTRUCTION\",\"TBLSTOREMAST\", \"TBLTRANSMAKES\",\"TBLMASTERDATA\" where \"DI_STORE_ID\"=\"SM_ID\" and \"DI_MAKE_ID\" = \"TM_ID\" ";
                if (objAllot.sDINo != "" && objAllot.sDINo != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DINO", objAllot.sDINo);
                    sQry += "AND cast(\"DI_NO\" as text)=:DINO  ";
                }
                if (objAllot.sDiId != "" && objAllot.sDiId != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DiID", objAllot.sDiId);
                    sQry += "AND cast(\"DI_ID\" as text)=:DiID  ";
                }

                sQry += "  and \"DI_STARTTYPE\"=\"MD_ID\" AND \"MD_TYPE\"='SR' and \"DI_STATUS\"=1  GROUP BY \"DI_ID\",\"DI_NO\",\"DI_CAPACITY\",\"DI_QUANTITY\",\"SM_NAME\", \"TM_NAME\",\"DI_CAPACITY\",\"DI_CAPACITY_ID\" ,\"MD_NAME\",\"DI_STORE_ID\",\"DI_STARTTYPE\"  , \"DI_STATUS\",\"DI_MAKE_ID\" )a ";
                sQry += " LEFT join (SELECT sum(\"ALT_QUANTITY\") as \"ALT_QUANTITY\",\"ALT_DI_NO\" ,\"ALT_CAPACITY_ID\",\"ALT_MAKE_ID\",\"ALT_STORE_ID\",\"ALT_STAR_TYPE\",\"ALT_STATUS\" from \"TBLALLOTEMENT\" GROUP BY \"ALT_CAPACITY\",\"ALT_DI_NO\",\"ALT_CAPACITY_ID\",\"ALT_MAKE_ID\",\"ALT_STORE_ID\",\"ALT_STAR_TYPE\",\"ALT_STATUS\")b  on a.\"DI_NO\"=b.\"ALT_DI_NO\" ";
                sQry += "  AND A.\"DI_CAPACITY_ID\" =B.\"ALT_CAPACITY_ID\" AND A.\"DI_STORE_ID\"=B.\"ALT_STORE_ID\" AND A.\"DI_MAKE_ID\"=B.\"ALT_MAKE_ID\"  AND A.\"DI_STARTTYPE\"=B.\"ALT_STAR_TYPE\" AND B.\"ALT_STATUS\"=1 AND A.\"DI_STATUS\"=1 GROUP BY \"DI_ID\",\"DI_NO\" ,\"SM_NAME\", \"TM_NAME\" ,\"DI_CAPACITY\",\"MD_NAME\" ,\"DI_MAKE_ID\",\"DI_CAPACITY_ID\",\"DI_STORE_ID\"";


                #region // old query
                //sQry =" select \"DI_ID\",\"DI_NO\",  \"SM_NAME\" AS \"STORE_NAME\",\"TM_NAME\" AS \"MAKE_NAME\",\"DI_CAPACITY\",\"MD_NAME\"AS \"STAR_RATE\",";
                //sQry +="  \"DI_QUANTITY\" AS \"TOTAL\",COALESCE((\"DI_QUANTITY\") - SUM(COALESCE(\"ALT_QUANTITY\",0)),0) \"PENDING\" FROM \"TBLDELIVERYINSTRUCTION\",";
                //sQry += " \"TBLTRANSMAKES\",\"TBLALLOTEMENT\",\"TBLSTOREMAST\",\"TBLMASTERDATA\" WHERE \"DI_STORE_ID\"=\"SM_ID\" AND \"TM_ID\"=\"DI_MAKE_ID\" AND \"MD_TYPE\"='SR' ";
                // if (objDel.sDINo != "" && objDel.sDINo != null)
                //{
                //    NpgsqlCommand.Parameters.AddWithValue("DINO", objDel.sDINo);
                //    sQry += "AND \"DI_NO\"=:DINO  ";
                //}
                //if (objDel.sDiId != "" && objDel.sDiId != null)
                //{
                //    NpgsqlCommand.Parameters.AddWithValue("DiID", objDel.sDiId);
                //    sQry += "AND cast(\"DI_ID\" as text)=:DiID  ";
                //}
                //sQry += " AND \"MD_ID\"=\"DI_STARTTYPE\" GROUP BY  \"DI_ID\",\"DI_NO\", \"SM_NAME\",\"MAKE_NAME\" ,\"DI_CAPACITY\",\"MD_NAME\",\"DI_QUANTITY\" ORDER BY \"DI_ID\"";
                #endregion


                dt = objcon.FetchDataTable(sQry, NpgsqlCommand);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public DataTable GetAllotmentDetails(clsAllotement objAllotement)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtIndentDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("AltID", Convert.ToInt32(objAllotement.sALTid));
                strQry = "select \"ALT_ID\",\"ALT_NO\",\"ALT_DI_NO\",TO_CHAR(\"ALT_DATE\",'dd-MM-yyyy')\"ALT_DATE\",\"ALT_STORE_ID\",\"ALT_DIV_ID\" ,\"SM_NAME\" AS \"ALT_STORE_NAME\" , \"ALT_CAPACITY\",\"ALT_CAPACITY_ID\", \"ALT_STAR_TYPE\",\"MD_NAME\"  AS \"ALT_STARRATENAME\",\"ALT_QUANTITY\", \"ALT_FILE_PATH\" from \"TBLALLOTEMENT\" ,\"TBLSTOREMAST\",\"TBLMASTERDATA\" ";
                strQry += " WHERE \"ALT_STAR_TYPE\"=\"MD_ID\" AND \"MD_TYPE\"='SR'  AND \"SM_ID\"=\"ALT_STORE_ID\" AND  \"ALT_ID\"=:AltID  AND \"ALT_STATUS\"=1 ";

                dtIndentDetails = objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dtIndentDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtIndentDetails;
            }
        }
      
        public DataTable GetPendingTc(clsAllotement objAllotement)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;

                strQry = " SELECT a.\"DI_ID\",SUM(a.\"DI_QUANTITY\"),COALESCE(SUM(a.\"DI_QUANTITY\") - SUM(COALESCE(b.\"ALT_QUANTITY\",0)),0) \"PENDING\" from (SELECT SUM(\"DI_QUANTITY\") as \"DI_QUANTITY\",\"DI_ID\",\"DI_NO\" ,\"DI_CAPACITY_ID\",\"DI_STORE_ID\",\"DI_MAKE_ID\",\"DI_STARTTYPE\" FROM \"TBLDELIVERYINSTRUCTION\" where \"DI_STATUS\"=1 AND";
               
                 if (objAllotement.sDINo != "" && objAllotement.sDINo != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DINO", objAllotement.sDINo);
                  strQry +="  \"DI_NO\"=:DINO  and ";
                }
                if (objAllotement.sCapacity != "" && objAllotement.sCapacity != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DiCap", objAllotement.sCapacity);
                    strQry +=" cast(\"DI_CAPACITY\" as text)=:DiCap  and ";
                }
                if (objAllotement.sStoreId != "" && objAllotement.sStoreId != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("St_id", objAllotement.sStoreId);
                    strQry += " cast(\"DI_STORE_ID\" as text)=:St_id  and ";
                }
                if (objAllotement.sMakeId != "" && objAllotement.sMakeId != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DiMake", objAllotement.sMakeId);
                    strQry += " cast(\"DI_MAKE_ID\" as text)=:DiMake AND  ";
                }
                if (objAllotement.sRatingId != "" && objAllotement.sRatingId != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("DiRating", objAllotement.sRatingId);
                    strQry += " cast(\"DI_STARTTYPE\" as text)=:DiRating  ";
                }


                strQry += "  GROUP BY \"DI_ID\",\"DI_CAPACITY\",\"DI_NO\",\"DI_CAPACITY_ID\",\"DI_STORE_ID\",\"DI_MAKE_ID\",\"DI_STARTTYPE\") a left join (SELECT sum(\"ALT_QUANTITY\") as \"ALT_QUANTITY\",\"ALT_DI_NO\",\"ALT_CAPACITY_ID\" ,\"ALT_STORE_ID\",\"ALT_MAKE_ID\",\"ALT_STATUS\",\"ALT_STAR_TYPE\" from \"TBLALLOTEMENT\" ";
                strQry += " GROUP BY \"ALT_CAPACITY\",\"ALT_DI_NO\",\"ALT_CAPACITY_ID\" ,\"ALT_STORE_ID\",\"ALT_MAKE_ID\",\"ALT_STATUS\",\"ALT_STAR_TYPE\" )b on a.\"DI_NO\"=b.\"ALT_DI_NO\" AND A.\"DI_CAPACITY_ID\"=B.\"ALT_CAPACITY_ID\" AND A.\"DI_STORE_ID\"=B.\"ALT_STORE_ID\"  AND A.\"DI_STARTTYPE\"=B.\"ALT_STAR_TYPE\" AND A.\"DI_MAKE_ID\"=B.\"ALT_MAKE_ID\" AND \"ALT_STATUS\"=1 GROUP BY \"DI_ID\",\"DI_NO\" ,\"DI_CAPACITY_ID\",\"DI_STORE_ID\",\"DI_MAKE_ID\"";

                dt = objcon.FetchDataTable(strQry, NpgsqlCommand);

                return dt;
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        public string[] SaveDeliveryDetails(clsAllotement objAllotement)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            try
            {
                string sQry = string.Empty;
                DataTable dt = new DataTable();
                dt = objAllotement.dtAllotement;
                string str;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sAlt_Id = Convert.ToString(dt.Rows[i]["ALT_ID"]);
                    string sALTNo = Convert.ToString(dt.Rows[i]["ALT_NO"]);
                    string sDiNO = Convert.ToString(dt.Rows[i]["ALT_DI_NO"]);           
                    string sStoreId = Convert.ToString(dt.Rows[i]["ALT_STORE_ID"]);
                    string sDivId = Convert.ToString(dt.Rows[i]["ALT_DIV_ID"]);
                    string sAltDate = Convert.ToString(dt.Rows[i]["ALT_DATE"]);                    
                    string sCapacity = Convert.ToString(dt.Rows[i]["ALT_CAPACITY"]);
                    string sCapacityID = Convert.ToString(dt.Rows[i]["ALT_CAPACITY_ID"]);
                    string sStartype = Convert.ToString(dt.Rows[i]["ALT_STAR_TYPE"]);
                    string sQuantity = Convert.ToString(dt.Rows[i]["ALT_QUANTITY"]);
                    string sMake = Convert.ToString(dt.Rows[i]["ALT_MAKE_ID"]);


                   // string DiId = objcon.get_value(" select \"DI_ID\" from \"TBLDELIVERYINSTRUCTION\" WHERE \"DI_NO\" ='" + sDiNO + "' AND \"DI_CAPACITY\" = '"+ sCapacity + "' AND \"DI_STORE_ID\" ='" + sStoreId + "' AND \"DI_STARTTYPE\" ='" + sStartype + "' ", NpgsqlCommand);
                    long Id = objcon.Get_max_no("ALT_ID", "TBLALLOTEMENT");
                    if (sAlt_Id == "")
                    {
                        //Objcon.BeginTransaction();
                        NpgsqlCommand.Parameters.AddWithValue("Alt_no", sALTNo.ToUpper());
                        NpgsqlCommand.Parameters.AddWithValue("DiNo", sDiNO.ToUpper());
                        str = objcon.get_value(" select * from \"TBLALLOTEMENT\",\"TBLDELIVERYINSTRUCTION\" where cast(\"ALT_NO\" as text)=:Alt_no  AND \"DI_NO\"=\"ALT_DI_NO\" AND CAST(\"DI_NO\" AS text) <>:DiNo ", NpgsqlCommand);
                        if (str != null && str != "")
                        {

                            Arr[0] = "Entered Allotment Number Already mapped with some other DI  Number";
                            Arr[1] = "2";
                            return Arr;
                        }
                        NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_altmaster");
                        cmd.Parameters.AddWithValue("alt_id",sAlt_Id);
                        cmd.Parameters.AddWithValue("alt_no", sALTNo.ToUpper());
                        cmd.Parameters.AddWithValue("di_no", sDiNO.ToUpper());
                        cmd.Parameters.AddWithValue("div_id", sDivId);
                        cmd.Parameters.AddWithValue("alt_crby", objAllotement.sCrby);
                        cmd.Parameters.AddWithValue("alt_date", sAltDate);
                        cmd.Parameters.AddWithValue("alt_store_id", sStoreId);
                        cmd.Parameters.AddWithValue("alt_star_rate", sStartype);
                        cmd.Parameters.AddWithValue("alt_make_id", sMake);
                        cmd.Parameters.AddWithValue("alt_quantity", sQuantity);
                        cmd.Parameters.AddWithValue("capacity", sCapacity);
                        cmd.Parameters.AddWithValue("capacity_id", sCapacityID);
                        cmd.Parameters.AddWithValue("alt_satus", "1");
                              
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
                        objAllotement.sALTid = Arr[2].ToString();

                    
                         
                       /// NpgsqlCommand.Parameters.AddWithValue("FileExt", sFileExt);

                        //sQry = "INSERT INTO \"TBLALLOTEMENT\" (\"ALT_ID\" , \"ALT_NO\", \"ALT_DI_ID\",  \"ALT_DATE\", \"ALT_DIV_ID\" ,\"ALT_CAPACITY\",\"ALT_CAPACITY_ID\", ";
                        //sQry += " \"ALT_STAR_TYPE\", \"ALT_STORE_ID\", \"ALT_CRON\",\"ALT_QUANTITY\", \"ALT_CRBY\" )";
                        //sQry += " VALUES((SELECT COALESCE(MAX(\"ALT_ID\"),0)+1 FROM \"TBLALLOTEMENT\"),:AltNo,CAST(:DiId AS INT8), TO_DATE(:AltDate,'DD/MM/YYYY'),:DivId, ";
                        //sQry += ":Capacity,:CapacityID,:Startype,:StoreId,NOW(),:Quantity,:Crby) ";
                       
                        //objcon.ExecuteQry(sQry, NpgsqlCommand);


                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("AltNo", sALTNo.ToUpper());
                        //str = "DELETE FROM \"TBLALLOTEMENT\" WHERE \"ALT_NO\"=:AltNo";
                        str = "UPDATE \"TBLALLOTEMENT\" SET \"ALT_STATUS\"=0 WHERE \"ALT_NO\"=:AltNo";
                        objcon.ExecuteQry(str, NpgsqlCommand);
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            sAlt_Id = Convert.ToString(dt.Rows[j]["ALT_ID"]);
                            sALTNo = Convert.ToString(dt.Rows[j]["ALT_NO"]);
                            sDiNO = Convert.ToString(dt.Rows[j]["ALT_DI_NO"]);
                            sStoreId = Convert.ToString(dt.Rows[j]["ALT_STORE_ID"]);
                            sDivId = Convert.ToString(dt.Rows[j]["ALT_DIV_ID"]);
                            sAltDate = Convert.ToString(dt.Rows[j]["ALT_DATE"]);
                            sCapacity = Convert.ToString(dt.Rows[j]["ALT_CAPACITY"]);
                            sCapacityID = Convert.ToString(dt.Rows[j]["ALT_CAPACITY_ID"]);
                            sStartype = Convert.ToString(dt.Rows[j]["ALT_STAR_TYPE"]);
                            sQuantity = Convert.ToString(dt.Rows[j]["ALT_QUANTITY"]);
                            sMake = Convert.ToString(dt.Rows[j]["ALT_MAKE_ID"]);

                            NpgsqlCommand cmd1 = new NpgsqlCommand("proc_saveupdate_altmaster");
                            cmd1.Parameters.AddWithValue("alt_id", sAlt_Id);
                            cmd1.Parameters.AddWithValue("alt_no", sALTNo.ToUpper());
                            cmd1.Parameters.AddWithValue("di_no", sDiNO.ToUpper());
                            cmd1.Parameters.AddWithValue("div_id", sDivId);
                            cmd1.Parameters.AddWithValue("alt_crby", objAllotement.sCrby);
                            cmd1.Parameters.AddWithValue("alt_date", sAltDate);
                            cmd1.Parameters.AddWithValue("alt_store_id", sStoreId);
                            cmd1.Parameters.AddWithValue("alt_star_rate", sStartype);
                            cmd1.Parameters.AddWithValue("alt_make_id", sMake);
                            cmd1.Parameters.AddWithValue("alt_quantity", sQuantity);
                            cmd1.Parameters.AddWithValue("capacity", sCapacity);
                            cmd1.Parameters.AddWithValue("capacity_id", sCapacityID);
                            cmd1.Parameters.AddWithValue("alt_satus", "1");


                            //NpgsqlCommand.Parameters.AddWithValue("FileExt", sFileExt);

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
                            objAllotement.sALTid = Arr[2].ToString();


                            //sQry = "UPDATE \"TBLALLOTEMENT\" SET  \"ALT_NO\"=:AltNo, \"ALT_DI_ID\"=:DiNo, \"ALT_STORE_ID\"=:StoreId, \"ALT_DATE\"=TO_DATE(:AltDate,'DD/MM/YYYY'), \"ALT_DIV_ID\"=:DivId, ";
                            //sQry += " \"ALT_CAPACITY\"=:Capacity,\"ALT_CAPACITY_ID\"=:CapacityID, \"ALT_STAR_TYPE\" =:Startype, \"ALT_QUANTITY\" =:Quantity,\"ALT_CRON\"=NOW(), \"ALT_CRBY\"=:Crby  ";
                            //sQry += "  WHERE \"ALT_ID\" =:Alt_Id ";

                            //objcon.ExecuteQry(sQry, NpgsqlCommand);
                            if (objAllotement.sFileExt.Length > 0)
                            {
                                NpgsqlParameter DeliveryNote = new NpgsqlParameter();
                                NpgsqlCommand comd = new NpgsqlCommand();


                                sQry = " UPDATE \"TBLALLOTEMENT\" SET \"ALT_FILE_PATH\"='" + objAllotement.sFileExt + "' WHERE \"ALT_ID\" = '" + objAllotement.sALTid + "'";
                                NpgsqlConnection objconn = new NpgsqlConnection();
                                string strConnectionString = ConfigurationManager.ConnectionStrings["pgSQL"].ConnectionString + "Idea@12345+";
                                objconn.ConnectionString = strConnectionString; // "Server=localhost;Port=5432;Database=TestTRM;User Id=postgres;Password =Idea123";  
                                objconn.Open();
                                objcon.ExecuteQry(sQry, cmd1);
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

                    if (objAllotement.sFileExt.Length > 0)
                    {
                        NpgsqlParameter DeliveryNote = new NpgsqlParameter();
                        NpgsqlCommand comd = new NpgsqlCommand();
                        //byte[] imageData = (byte[])dt.Rows[i]["DI_FILE"];
                        //if (imageData != null)
                        //{
                        //    DeliveryNote.ParameterName = "Document";
                        //    DeliveryNote.Value = imageData;
                        //}
                        //else
                        //{
                        //    DeliveryNote.ParameterName = "Document";
                        //    DeliveryNote.Value = null;
                        //}

                        sQry = "  UPDATE \"TBLALLOTEMENT\" SET \"ALT_FILE_PATH\"='" + objAllotement.sFileExt + "' WHERE \"ALT_ID\" = '" + objAllotement.sALTid + "'";
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
            catch (Exception ex)
            {
                Arr[0] = "Failed to Save";
                Arr[1] = "1";
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }
    }
}
