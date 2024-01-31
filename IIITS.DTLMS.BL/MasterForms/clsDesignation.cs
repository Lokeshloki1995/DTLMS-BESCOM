using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
   public class clsDesignation
    {
        
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        public string sDesignationId { get; set; }
        public string sDesignationName { get; set; }
        public string sDesignationDesc { get; set; }
        public string sCrby { get; set; }

        string strFormCode = "clsDesignation";

        NpgsqlCommand NpgsqlCommand;
        public string[] SaveDetails(clsDesignation objDesignation)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] iReturn = new string[3];
            string sQry = string.Empty;
            string sQryValue = string.Empty;
            int PKID = 0;
            
            try
            {
                if (objDesignation.sDesignationId == "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("designationName", objDesignation.sDesignationName.ToUpper());
                    sQry = "SELECT \"DM_NAME\" FROM \"TBLDESIGNMAST\" WHERE UPPER(\"DM_NAME\") =:designationName";
                }
                else
                {
                    PKID = Convert.ToInt32(objDesignation.sDesignationId);
                    NpgsqlCommand.Parameters.AddWithValue("designationName1", objDesignation.sDesignationName.ToUpper());
                    NpgsqlCommand.Parameters.AddWithValue("designationId",Convert.ToInt32( objDesignation.sDesignationId));
                    sQry = "SELECT \"DM_NAME\" FROM \"TBLDESIGNMAST\" WHERE UPPER(\"DM_NAME\")=:designationName1 AND \"DM_DESGN_ID\" <>:designationId";
                }


                sQryValue = Objcon.get_value(sQry, NpgsqlCommand);
                if (sQryValue.Length > 0)
                {
                    iReturn[0] = "Designation Name Already Exists";
                    iReturn[1] = "2";
                }
                else
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdate_designation");
                    cmd.Parameters.AddWithValue("dm_desgn_id", Convert.ToString(PKID));
                    cmd.Parameters.AddWithValue("dm_name", objDesignation.sDesignationName.ToUpper());
                    cmd.Parameters.AddWithValue("dm_desc", objDesignation.sDesignationDesc);
                    cmd.Parameters.AddWithValue("dm_crby", objDesignation.sCrby);
                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);

                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;


                    iReturn[0] = "msg";
                    iReturn[1] = "op_id";
                    

                    iReturn = Objcon.Execute(cmd, iReturn, 2);
                }
                
             
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return iReturn;
        }

        public DataTable LoadDetails()
        {
            
            DataTable dt = new DataTable();
            string sQry = string.Empty;
            try
            {

                sQry = "SELECT \"DM_DESGN_ID\", \"DM_NAME\", \"DM_DESC\" FROM \"TBLDESIGNMAST\" ORDER BY \"DM_ORDER_BY\"";
                dt = Objcon.FetchDataTable(sQry);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return dt;
        }


        public object getDesignationDetails(clsDesignation objDesignation)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("designationId",Convert.ToInt32( objDesignation.sDesignationId));
                sQry = "SELECT \"DM_DESGN_ID\", \"DM_NAME\", \"DM_DESC\" FROM \"TBLDESIGNMAST\" WHERE \"DM_DESGN_ID\" =:designationId";
                dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
                if(dt.Rows.Count > 0)
                {
                    objDesignation.sDesignationId = Convert.ToString(dt.Rows[0]["DM_DESGN_ID"].ToString());
                    objDesignation.sDesignationName = Convert.ToString(dt.Rows[0]["DM_NAME"].ToString());
                    objDesignation.sDesignationDesc = Convert.ToString(dt.Rows[0]["DM_DESC"].ToString());
                }
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return objDesignation;
        }

   }
}
