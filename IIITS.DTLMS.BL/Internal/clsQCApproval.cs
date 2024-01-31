using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Configuration;

namespace IIITS.DTLMS.BL
{
   public  class clsQCApproval
    {

        string strFormCode = "clsQCApproval";
        //CustOledbConnection ObjCon = new CustOledbConnection(Constants.Password);
        public string sPendingforQC { get; set; }
        public string sLocationType { get; set; }
        public string sOperator { get; set; }
        public string sSupervisor { get; set; }
        public string sDivision { get; set; }
        public string sSubDivision { get; set; }
        public string sOffcode { get; set; }
        public string sFeeder { get; set; }
        public string sStore { get; set; }
        public string sRepairer { get; set; }

        public string sOperator1 { get; set; }
        public string sOperator2 { get; set; }
        public string sSerialNo { get; set; }

        public string sEnumDetailsId { get; set; }
        public string sStatusFlag { get; set; }
        public string sComments { get; set; }

        public bool  sMobileEntry { get; set; }

        public string sDtrCode { get; set; }
        public string sDtcCode { get; set; }
        public string sQcDoneBy { get; set; }
        public string sDeEnteredBy { get; set; }
        public string sDtcName { get; set; }

       public DataTable LoadEnumearionDetails(clsQCApproval objQC)
       {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable dt = new DataTable();
           try
           {
               string strQry = string.Empty;
                

                strQry = " SELECT * FROM (SELECT \"ED_ID\", CASE WHEN \"ED_LOCTYPE\"=1 THEN 'STORE' WHEN \"ED_LOCTYPE\"=2 THEN 'FIELD' WHEN \"ED_LOCTYPE\"=3 THEN 'REPAIRER' WHEN \"ED_LOCTYPE\"=5 THEN 'TRANSFORMER BANK' END AS \"TYPE\",";
                strQry += " \"DTE_DTCCODE\",UPPER(\"DTE_NAME\") \"DTE_NAME\",\"DTE_TC_CODE\" \"DTE_TC_CODE\", ";
                strQry += " (SELECT UPPER(\"IU_FULLNAME\")  FROM \"TBLINTERNALUSERS\" WHERE CAST(\"ED_OPERATOR1\" AS TEXT) = CAST(\"IU_ID\" AS TEXT))  AS \"OPERATOR1\", ";
                strQry += " (SELECT UPPER(\"TM_NAME\")  FROM \"TBLTRANSMAKES\" WHERE CAST(\"TM_ID\" AS TEXT)=CAST(\"DTE_MAKE\" AS TEXT)) AS \"MAKE\", ";
                strQry += " (SELECT UPPER(\"IU_FULLNAME\")  FROM \"TBLINTERNALUSERS\" WHERE \"IU_ID\" = (SELECT \"IU_SUPERVISORID\" FROM \"TBLINTERNALUSERS\" WHERE  CAST(\"IU_ID\" AS TEXT) = CAST(\"ED_OPERATOR1\" AS TEXT))) AS \"SUPERVISOR\",\"ED_LOCTYPE\",\"ED_ENUM_TYPE\",\"ED_STATUS_FLAG\",\"DTE_TC_SLNO\" ";
                strQry += " FROM \"TBLENUMERATIONDETAILS\", \"TBLDTCENUMERATION\" WHERE CAST(\"ED_ID\" AS TEXT) = CAST(\"DTE_ED_ID\" AS TEXT) AND \"ED_STATUS_FLAG\" = '" + objQC.sPendingforQC + "' ";
                //strQry += " ";
                if (objQC.sFeeder != null)
                {
                    strQry += " AND CAST(\"ED_FEEDERCODE\" AS TEXT) LIKE '" + objQC.sFeeder + "'";
                }

                if (objQC.sOperator != null)
                {
                    strQry += " AND (\"ED_OPERATOR1\" = '" + objQC.sOperator + "' OR \"ED_OPERATOR2\" = '" + objQC.sOperator + "')";
                }

                if (objQC.sLocationType != null && objQC.sLocationType != "")
                {
                    strQry += " AND \"ED_LOCTYPE\" IN (" + objQC.sLocationType + ")";
                }

                if (objQC.sOffcode != null)
                {
                    strQry += " AND CAST(\"ED_OFFICECODE\"  AS TEXT)LIKE '" + objQC.sOffcode + "%'";
                }

                if (objQC.sRepairer != null)
                {
                    strQry += " AND CAST(\"ED_LOCNAME\" AS TEXT) LIKE '" + objQC.sRepairer + "'";
                }

                if (objQC.sStore != null)
                {
                    strQry += " AND CAST(\"ED_LOCNAME\" AS TEXT) LIKE '" + objQC.sStore + "'";
                }
                if (objQC.sSupervisor != null)
                {
                    strQry += " AND (\"ED_OPERATOR2\" IN (SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_SUPERVISORID\"='" + objQC.sSupervisor + "'))";
                    strQry += " OR (\"ED_OPERATOR1\" IN (SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_SUPERVISORID\"='" + objQC.sSupervisor + "'))";
                }

                if (objQC.sMobileEntry == true)
                {
                    strQry += " AND CAST(\"ED_RECORD_BY\"  AS TEXT)LIKE 'MOBILE%'";
                }

                strQry += " ORDER BY \"DTE_DTCCODE\")A ";
                strQry += " WHERE  ";/* ROWNUM < 20 AND*/
                strQry += "  CAST(\"DTE_TC_CODE\" AS TEXT) LIKE '" + objQC.sDtrCode + "%' AND CAST(\"OPERATOR1\" AS TEXT) LIKE '" + objQC.sOperator1 + "%'";
                if (objQC.sDtcCode != "")
                {
                    strQry += " AND CAST(\"DTE_DTCCODE\" AS TEXT) LIKE '" + objQC.sDtcCode + "%'";
                }
                if (objQC.sDtcName != "")
                {
                    strQry += " AND CAST(\"DTE_NAME\" AS TEXT) LIKE '" + objQC.sDtcName + "%'";
                }

                if (objQC.sSerialNo != "")
                {
                    strQry += " AND UPPER(CAST(\"DTE_TC_SLNO\" AS TEXT))  LIKE '" + objQC.sSerialNo + "%'";
                }


                // AND ED_OFFICECODE<>'8888'
                dt = ObjCon.FetchDataTable(strQry);
               return dt;

           }
           catch (Exception ex)
           {
               clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "GetEnumearionDetails");
               return dt;
           }

          
       }


       //Will Be Called at the time of FiledEnumeraton Data Update to check the Access.
       public bool CheckEnumerationUpdateAuthority(string sUserId)
       {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            try
           {
               string strQry = string.Empty;
                //strQry = "SELECT ED_ID FROM TBLENUMERATIONDETAILS WHERE (ED_OPERATOR1='" + sUserId + "' OR ED_OPERATOR2='" + sUserId + "' OR ED_CRBY='" + sUserId + "') OR ";
                //strQry += " (ED_CRBY IN (SELECT IU_ID FROM TBLINTERNALUSERS WHERE IU_SUPERVISORID='" + sUserId + "')) ";
                //strQry += "  OR   (ED_OPERATOR2 IN (SELECT IU_ID FROM TBLINTERNALUSERS WHERE IU_SUPERVISORID='" + sUserId + "'))";
                //strQry += " OR (ED_OPERATOR1 IN (SELECT IU_ID FROM TBLINTERNALUSERS WHERE IU_SUPERVISORID='" + sUserId + "'))";
                //strQry += "  UNION ALL ";
                //strQry += " SELECT IU_ID FROM TBLINTERNALUSERS WHERE (IU_ID='" + sUserId + "' OR IU_SUPERVISORID='" + sUserId + "') AND IU_USERTYPE='5'";

                strQry = "SELECT \"ED_ID\" FROM \"TBLENUMERATIONDETAILS\" WHERE (\"ED_OPERATOR1\"='" + sUserId + "' OR \"ED_OPERATOR2\"='" + sUserId + "' OR \"ED_CRBY\"='" + sUserId + "') OR ";
                strQry += " (CAST (\"ED_CRBY\" as int) IN (SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_SUPERVISORID\"='" + sUserId + "')) ";
                strQry += "  OR   ( CAST (\"ED_OPERATOR2\" as int) IN (SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_SUPERVISORID\"='" + sUserId + "'))";
                strQry += " OR (CAST (\"ED_OPERATOR1\" as int) IN (SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_SUPERVISORID\"='" + sUserId + "'))";
                strQry += "  UNION ALL ";
                strQry += " SELECT \"IU_ID\" FROM \"TBLINTERNALUSERS\" WHERE (\"IU_ID\"='" + sUserId + "' OR \"IU_SUPERVISORID\"='" + sUserId + "') AND \"IU_USERTYPE\"='5'";

                string sResult = ObjCon.get_value(strQry);
               if (sResult == "")
               {
                   return false;
               }
               else
               {
                   return true;
               }

           }
           catch (Exception ex)
           {
               clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "CheckEnumerationUpdateAuthority");
               return false;
           }
       }

       public string GetPendingRejectRemarks(clsQCApproval objQC)
       {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            try
           {
               string strQry = string.Empty;
               if (objQC.sStatusFlag == "3")
               {
                   strQry = "SELECT \"QR_REMARKS\" FROM \"TBLQCREJECT\" WHERE \"QR_ED_ID\"='" + objQC.sEnumDetailsId + "'";
                   
               }
               else if(objQC.sStatusFlag == "2")
               {
                   strQry = "SELECT \"QP_REMARKS\" FROM \"TBLQCPENDING\" WHERE \"QP_ED_ID\"='" + objQC.sEnumDetailsId + "'";
               }
               return ObjCon.get_value(strQry);
           }
           catch (Exception ex)
           {
               clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "GetPendingRejectRemarks");
               return ex.Message;
           }
       }

       public DataTable LoadQcDoneBy(clsQCApproval objQC)
       {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable dt = new DataTable();
           try
           {
               string strQry = string.Empty;
                //strQry = " SELECT ED_ID, DECODE(ED_LOCTYPE,'1','STORE','2','FIELD','3','REPAIRER','5','TRANSFORMER BANK') as TYPE,DTE_DTCCODE, DTE_NAME,  TO_CHAR(DTE_TC_CODE)DTE_TC_CODE, ";
                //strQry += " (SELECT IU_FULLNAME FROM TBLINTERNALUSERS WHERE ED_OPERATOR2 = IU_ID) AS OPERATOR2, ";
                //strQry += " (SELECT IU_FULLNAME FROM TBLINTERNALUSERS WHERE IU_ID =ED_OPERATOR1) AS ED_OPERATOR1,(SELECT IU_FULLNAME FROM TBLINTERNALUSERS WHERE IU_ID =ED_APPROVED_BY) AS ED_APPROVED_BY";
                //strQry += " FROM TBLENUMERATIONDETAILS, TBLDTCENUMERATION WHERE ED_ID = DTE_ED_ID and ED_STATUS_FLAG=1";

                //if (objQC.sDtcCode != "")
                //{
                //    strQry += " AND DTE_DTCCODE LIKE '" + objQC.sDtcCode + "%'";
                //}

                //if (objQC.sDtrCode != "")
                //{
                //    strQry += " AND DTE_TC_CODE LIKE '" + objQC.sDtrCode + "%'";
                //}

                //if (objQC.sDeEnteredBy != null)
                //{
                //    strQry += " AND ED_OPERATOR1='" + objQC.sDeEnteredBy + "'";
                //}

                //if (objQC.sQcDoneBy != null)
                //{
                //    strQry += " AND ED_APPROVED_BY ='" + objQC.sQcDoneBy + "'";
                //}


                strQry = " SELECT \"ED_ID\", CASE WHEN \"ED_LOCTYPE\"=1 THEN 'STORE' WHEN \"ED_LOCTYPE\"=2 THEN 'FIELD' WHEN \"ED_LOCTYPE\"=3 THEN 'REPAIRER' WHEN \"ED_LOCTYPE\"=5 THEN 'TRANSFORMER BANK' END AS \"TYPE\",\"DTE_DTCCODE\", \"DTE_NAME\", \"DTE_TC_CODE\" \"DTE_TC_CODE\", ";
                strQry += " (SELECT \"IU_FULLNAME\" FROM \"TBLINTERNALUSERS\" WHERE CAST(\"ED_OPERATOR2\" AS TEXT) = CAST(\"IU_ID\" AS TEXT)) AS \"OPERATOR2\", ";
                strQry += " (SELECT \"IU_FULLNAME\" FROM \"TBLINTERNALUSERS\" WHERE CAST(\"IU_ID\" AS TEXT) =CAST(\"ED_OPERATOR1\" AS TEXT)) AS \"ED_OPERATOR1\",(SELECT \"IU_FULLNAME\" FROM \"TBLINTERNALUSERS\" WHERE CAST(\"IU_ID\" AS TEXT) =CAST(\"ED_APPROVED_BY\" AS TEXT)) AS \"ED_APPROVED_BY\"";
                strQry += " FROM \"TBLENUMERATIONDETAILS\", \"TBLDTCENUMERATION\" WHERE CAST(\"ED_ID\" AS TEXT) = CAST(\"DTE_ED_ID\" AS TEXT) and \"ED_STATUS_FLAG\"=1";

                if (objQC.sDtcCode != "")
                {
                    strQry += " AND CAST(\"DTE_DTCCODE\" AS TEXT) LIKE '" + objQC.sDtcCode + "%'";
                }

                if (objQC.sDtrCode != "")
                {
                    strQry += " AND CAST(\"DTE_TC_CODE\" AS TEXT) LIKE '" + objQC.sDtrCode + "%'";
                }

                if (objQC.sDeEnteredBy != null)
                {
                    strQry += " AND \"ED_OPERATOR1\"='" + objQC.sDeEnteredBy + "'";
                }

                if (objQC.sQcDoneBy != null)
                {
                    strQry += " AND \"ED_APPROVED_BY\" ='" + objQC.sQcDoneBy + "'";
                }

                dt = ObjCon.FetchDataTable(strQry);
               return dt;
           }
           catch (Exception ex)
           {
               clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "LoadQcDoneBy");
               return dt;
           }
       }
    }
}
