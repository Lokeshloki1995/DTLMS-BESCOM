using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using System.Data;
using System.Data.OleDb;

namespace IIITS.DTLMS.BL
{
    public class clsApkDownload
    {
        string strFormCode = "clsApkDownload";
        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

        public string RetrieveLatestApkDetails()
        {
            string sFolderName = string.Empty;
            clsApkDownload objReturnApk = new clsApkDownload();
            try
            {
                sFolderName = ObjCon.get_value("SELECT MAX(\"AP_FOLDER_PATH\") FROM \"TBLDTLMSAPK\" ");
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sFolderName;
            }

            return sFolderName;
        }
    }
}
