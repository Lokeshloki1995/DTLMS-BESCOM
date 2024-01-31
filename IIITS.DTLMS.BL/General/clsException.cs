using System;
using System.Collections.Generic;
using System.Text;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;
using System.IO;

namespace IIITS.DTLMS.BL
{
   public static  class clsException
    {
       
       public static void LogError(string sFormName, string sFunctionName, string sErrorMsg, string sStackTrace)
       {
            try
            {
               
                string strQry = string.Empty;

                if (sErrorMsg.StartsWith("Object")  || sErrorMsg.Contains("null value") ||  sErrorMsg.Contains("Thread")|| sErrorMsg.Contains("deadlock")
                    || sErrorMsg.Contains("duplicate key") || sErrorMsg.Contains("Root element") || sErrorMsg.Contains("Cannot find") ||
                    sErrorMsg.Contains("Load report failed") || sErrorMsg.Contains("Exception while writing to stream"))
                // || sErrorMsg.ToUpper(). Contains("SERVER"))
                {
                    return;
                }
                string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGFILEPATH"]) + DateTime.Now.ToString("yyyyMM");
                if (Convert.ToString(ConfigurationManager.AppSettings["LOGTOFILE"]) == "ON")
                {
                    WriteLogFile(sFormName, sFunctionName, sErrorMsg, sStackTrace);
                }
                if (Convert.ToString(ConfigurationManager.AppSettings["LOGTOTBL"]) == "ON")
                {
                    PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
                    long iMaxId = Objcon.Get_max_no("ERR_ID", "TBLERRORLOG");
                    strQry = "Insert into \"TBLERRORLOG\" (\"ERR_ID\",\"ERR_PAGE_NAME\",\"ERR_FUNCTION\",\"ERR_ERROR\",\"ERR_STACK_TRACE\") values (";
                    strQry += " '" + iMaxId + "','" + sFormName + "','" + sFunctionName + "','" + sErrorMsg.Replace("'", "''") + "','" + sStackTrace.Replace("'", "''") + "')";
                    Objcon.ExecuteQry(strQry);
                }
            }
            catch (Exception ex)
            {
                WriteLogFile(ex.Message, sFunctionName, "", "DB");
            }
        }
        public static void LogErrorDev(string sFormName, string sFunctionName, string sErrorMsg, string sStackTrace)
        {
            try
            {

                string strQry = string.Empty;

                if (sErrorMsg.StartsWith("Object"))
                {
                    return;
                }
                string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGFILEPATHDEV"]) + DateTime.Now.ToString("yyyyMM");

                if (Convert.ToString(ConfigurationManager.AppSettings["LOGTOFILE"]) == "ON")
                {
                    WriteLogFileDev(sFormName, sFunctionName, sErrorMsg, sStackTrace);
                }
                if (Convert.ToString(ConfigurationManager.AppSettings["LOGTOTBL"]) == "ON")
                {
                    PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
                    long iMaxId = Objcon.Get_max_no("ERR_ID", "TBLDEVERRORLOG");
                    strQry = "Insert into \"TBLDEVERRORLOG\" (\"ERR_ID\",\"ERR_PAGE_NAME\",\"ERR_FUNCTION\",\"ERR_ERROR\",\"ERR_STACK_TRACE\") values (";
                    strQry += " '" + iMaxId + "','" + sFormName + "','" + sFunctionName + "','" + sErrorMsg.Replace("'", "''") + "','" + sStackTrace.Replace("'", "''") + "')";
                  //  Objcon.ExecuteQry(strQry);
                }
            }
            catch (Exception ex)
            {
                WriteLogFileDev(ex.Message, sFunctionName, "", "DB");
            }

        }

        public static string  ErrorMsg()
       {
           string strError=string.Empty;
           if (Convert.ToString(ConfigurationManager.AppSettings["ErrorMsg"]) == "ON")
           {
               strError= "An exception occurred while processing your request.";
           }
           return strError;
       }     

        public static void WriteLogFile(string Formname, string functionName, string ErrorMessage, string stackTrace)
        {
            string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGFILEPATH"]) + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(sFolderPath))
            {
                Directory.CreateDirectory(sFolderPath);
            }
            string sPath = sFolderPath + "//" + "Main" + DateTime.Now.ToString("yyyyMMdd") + "-ErrorLog.txt";
            File.AppendAllText(sPath, Environment.NewLine + "Time --" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " , Error PageName --" + Formname + " , Error Function--" + functionName + " , Error Message--" + ErrorMessage + " , Stack Trace--" + stackTrace + Environment.NewLine);
        }

        public static void WriteLogFileDev(string Formname, string functionName, string ErrorMessage, string stackTrace)
        {
            string sFolderPath = Convert.ToString(ConfigurationManager.AppSettings["LOGFILEPATHDEV"]) + DateTime.Now.ToString("yyyyMM");
            if (!Directory.Exists(sFolderPath))
            {
                Directory.CreateDirectory(sFolderPath);
            }
            string sPath = sFolderPath + "//" + "Main" + DateTime.Now.ToString("yyyyMMdd") + "-ErrorLog.txt";
            File.AppendAllText(sPath, Environment.NewLine + "Time --" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " , Error PageName --" + Formname + " , Error Function--" + functionName + " , Error Message--" + ErrorMessage + " , Stack Trace--" + stackTrace + Environment.NewLine);
        }
        public static void SaveFunctionExecLog(string sFunctionName)
       {
           //CustOledbConnection objCon = new CustOledbConnection(Constants.Password);

           //string strQry = string.Empty;
           //strQry = "INSERT INTO TBLPERFORMANCETRACK (PT_ID,PT_FUNCTION,PT_LOG_DATE) VALUES (";
           //strQry += " '" + objCon.Get_max_no("PT_ID", "TBLPERFORMANCETRACK") + "','"+ sFunctionName +"',SYSDATE)";
           //objCon.Execute(strQry);
       }
    }
}
