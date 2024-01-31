using IIITS.PGSQL.DAL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIITS.DTLMS.BL.Dashboard
{
   public class clsLatestUpdates
    {
        PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        public DataTable GetLatestUpdates()
        {
            DataTable dt = new DataTable();
            DataTable diDetails = new DataTable();
            string strQry = string.Empty;
            try
            {
                strQry = "SELECT \"UPDATEDESCRIPTION\",TO_CHAR(\"EFFECTFROM\",'dd-MON-yyyy') \"EFFECTFROM\" FROM \"TBLLATESTUPDATES\" WHERE \"STATUS\" = '1'";
                dt = objcon.FetchDataTable(strQry);
                return dt;
            }
            catch (Exception ex)
            {
                clsException.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }
        }
        public string getVersion()
        {
            string strQry = string.Empty;
            string Latestversion = string.Empty;
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                strQry = "select \"V_VERSION\" from \"TBLVERSIONS\" where \"V_STATUS\"='1'";
                return Latestversion = objcon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Latestversion;
            }
        }
    }
}
