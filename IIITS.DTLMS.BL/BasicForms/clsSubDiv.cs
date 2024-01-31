using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using Npgsql;
using System.Reflection;
using NpgsqlTypes;
using IIITS.PGSQL.DAL;

namespace IIITS.DTLMS.BL
{
   public class clsSubDiv
    {
       string strQry = string.Empty;
       string strFormCode = "clsSubDiv";
       PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
       public string sDivCode { get; set; }


       //public string[] SaveUpdateSubDivisionDetails(string strSubDivID, string strDivCode, string strSubDivCode, string strName, string strHead, string strMobile, string strPhone, string strEmail, bool IsSave, string strUserLogged)
       // {
       //     string[] Arrmsg = new string[2];
       //     try
       //     {
               
       //         if (IsSave)
       //         {
       //             OleDbDataReader drchk = objCon.Fetch("select * from TBLSUBDIVMAST where SD_SUBDIV_CODE='" + strSubDivCode + "'");
       //             if (drchk.Read())
       //             {
       //                 Arrmsg[0] = "Sub Division Code Already exists";
       //                 Arrmsg[1] = "4";
       //                 return Arrmsg;
       //             }
       //             drchk.Close();
       //             drchk = objCon.Fetch("select * from TBLSUBDIVMAST where UPPER(SD_SUBDIV_NAME)='" + strName.ToUpper().Replace("'", "''") + "'and SD_DIV_CODE ='" + strDivCode + "'  ");
       //             if (drchk.Read())
       //             {
       //                 Arrmsg[0] = "Sub Division Name Already exists";
       //                 Arrmsg[1] = "4";
       //                 return Arrmsg;
       //             }
       //             drchk.Close();

                   
       //             long MaxNo = objCon.Get_max_no("SD_ID", "TBLSUBDIVMAST");

       //             string strInsQry = string.Empty;
       //             strInsQry = "INSERT  into TBLSUBDIVMAST (SD_ID,SD_SUBDIV_CODE,SD_SUBDIV_NAME,SD_DIV_CODE,SD_HEAD_EMP,SD_MOBILE,SD_PHONE,SD_EMAIL,SD_ENTRY_AUTH) VALUES";
       //             strInsQry += " ('" + MaxNo + "','" + strSubDivCode + "','" + strName.Trim().ToUpper().Replace("'", "''") + "','" + strDivCode + "','" + strHead.Trim().ToUpper() + "','" + strMobile + "',";
       //             strInsQry += " '" + strPhone + "','" + strEmail + "','"+strUserLogged+"')";
       //             objCon.Execute(strInsQry);

       //             Arrmsg[0] = "SubDivision Details Saved Sucessfully";
       //             Arrmsg[1] = "0";
       //             return Arrmsg;
                  
       //         }
       //         else
       //         {



       //             OleDbDataReader drchk = objCon.Fetch("select * from TBLSUBDIVMAST where UPPER(SD_SUBDIV_NAME)='" + strName.ToUpper().Replace("'", "''") + "'and SD_DIV_CODE ='" + strDivCode + "' and SD_ID <>'" + strSubDivID + "' ");
       //             if (drchk.Read())
       //             {
       //                 Arrmsg[0] = "Sub Division Name Already exists";
       //                 Arrmsg[1] = "4";
       //                 return Arrmsg;
       //             }
       //             drchk.Close();
       //             string strUpdQuery = "Update TBLSUBDIVMAST set SD_SUBDIV_NAME='" + strName.Trim().ToUpper().Replace("'", "''") + "',SD_DIV_CODE='" + strDivCode + "',SD_HEAD_EMP='" + strHead.Trim().ToUpper().Replace("'", "''") + "',";
       //             strUpdQuery += "SD_MOBILE='" + strMobile + "',SD_PHONE='" + strPhone + "',SD_EMAIL='" + strEmail + "',SD_SUBDIV_CODE='" + strSubDivCode + "',SD_ENTRY_AUTH='" + strUserLogged + "',SD_ENTRY_DATE=SYSDATE  where SD_ID='" + strSubDivID + "' ";
       //             objCon.Execute(strUpdQuery);

       //             Arrmsg[0] = "Sub Division Details Update Sucessfully";
       //             Arrmsg[1] = "0";
       //             return Arrmsg;
                    
       //         }
       //     }
       //     catch (Exception ex)
       //     {
       //         clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "SaveUpdateSubDivisionDetails");
       //         return Arrmsg;
       //     }

       // }


       public string[] SaveUpdateSubDivisionDetails(string strSubDivID, string strDivCode, string strSubDivCode, string strName, string strHead, string strMobile, string strPhone, string strEmail, bool IsSave, string strUserLogged,string address)
       {
           string[] Arr = new string[2];
           try
           {
               string strId=string.Empty;
               if (IsSave)
               {
                   NpgsqlCommand cmd = new NpgsqlCommand("proc_save_subdiv");
                   cmd.Parameters.AddWithValue("subdiv_id", strId);
                   cmd.Parameters.AddWithValue("subdiv_code", strSubDivCode);
                   cmd.Parameters.AddWithValue("subdiv_name", strName.Trim().ToUpper().Replace("'", "''"));
                   cmd.Parameters.AddWithValue("div_code", strDivCode);
                   cmd.Parameters.AddWithValue("subdiv_head", strHead.Trim().ToUpper());
                   cmd.Parameters.AddWithValue("subdiv_mobileno", strMobile);
                   cmd.Parameters.AddWithValue("subdiv_phone", strPhone);
                   cmd.Parameters.AddWithValue("subdiv_email", strEmail);
                   cmd.Parameters.AddWithValue("userlogged", strUserLogged);
                   cmd.Parameters.AddWithValue("subdiv_address", address);
                   cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                   cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                   cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                   cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                   Arr[0] = "msg";
                   Arr[1] = "op_id";                   
                   Arr = Objcon.Execute(cmd, Arr, 2);                       
               }
               else
               {
                   NpgsqlCommand cmd = new NpgsqlCommand("proc_update_subdiv");
                   cmd.Parameters.AddWithValue("subdiv_id", strSubDivID);
                   cmd.Parameters.AddWithValue("subdiv_code", strSubDivCode);
                   cmd.Parameters.AddWithValue("subdiv_name", strName.Trim().ToUpper().Replace("'", "''"));
                   cmd.Parameters.AddWithValue("div_code", strDivCode);
                   cmd.Parameters.AddWithValue("subdiv_head", strHead.Trim().ToUpper());
                   cmd.Parameters.AddWithValue("subdiv_mobileno", strMobile);
                   cmd.Parameters.AddWithValue("subdiv_phone", strPhone);
                   cmd.Parameters.AddWithValue("subdiv_email", strEmail);
                   cmd.Parameters.AddWithValue("userlogged", strUserLogged);
                   cmd.Parameters.AddWithValue("subdiv_address", address);
                   cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                   cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                   cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                   cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                   Arr[0] = "msg";
                   Arr[1] = "op_id";                   
                   Arr = Objcon.Execute(cmd, Arr, 2);   
                 }
           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
           return Arr;

       }
        //public DataTable LoadSubDivOffDet(string strSubDivID = "")
        //{
        //    DataTable DtDivOffDet = new DataTable();
        //    try
        //    {
        //        strQry = string.Empty;
        //        strQry = "select SD_ID,To_char(SD_SUBDIV_CODE)SD_SUBDIV_CODE,SD_SUBDIV_NAME,DIV_NAME,SD_HEAD_EMP,SD_DIV_CODE,SD_TQ_ID,";
        //        strQry += "  SD_PHONE,SD_MOBILE,SD_EMAIL,CM_CIRCLE_NAME ";
        //        strQry += " from TBLDIVISION,TBLSUBDIVMAST,TBLCIRCLE where SD_DIV_CODE=DIV_CODE  AND CM_CIRCLE_CODE= DIV_CICLE_CODE   ";
        //        if (strSubDivID != "")
        //        {
        //            strQry += " and SD_ID='" + strSubDivID + "'";
        //        }
        //        strQry += " order by SD_SUBDIV_CODE";
        //        OleDbDataReader drcorp = objCon.Fetch(strQry);
        //        DtDivOffDet.Load(drcorp);

        //        return DtDivOffDet;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "LoadSubDivOffDet");
        //        return DtDivOffDet;
        //    }
        //}

       NpgsqlCommand NpgsqlCommand;
        public DataTable LoadSubDivOffDet(string strSubDivID = "")
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable DtDivOffDet = new DataTable();
            try
            {
                strQry = string.Empty;
                strQry = "SELECT \"SD_ID\",CAST(\"SD_SUBDIV_CODE\" AS TEXT)\"SD_SUBDIV_CODE\",\"SD_SUBDIV_NAME\",\"DIV_NAME\",\"SD_HEAD_EMP\",";
                strQry+="\"SD_DIV_CODE\",\"SD_TQ_ID\",\"SD_PHONE\",\"SD_MOBILE\",\"SD_EMAIL\",\"CM_CIRCLE_NAME\", \"SD_ADDRESS\" FROM \"TBLDIVISION\",";
                strQry += "\"TBLSUBDIVMAST\",\"TBLCIRCLE\" WHERE \"SD_DIV_CODE\"=\"DIV_CODE\"  AND \"CM_CIRCLE_CODE\"=\"DIV_CICLE_CODE\" ";
                if (strSubDivID != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("subDivId", strSubDivID);
                    strQry += "AND cast(\"SD_ID\" as text) like :subDivId||'%'";
                }
                strQry += "ORDER BY \"SD_SUBDIV_CODE\"";
                DtDivOffDet = Objcon.FetchDataTable(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return DtDivOffDet;
        }
        //public string GenerateSubDivCode(clsSubDiv objSubDivision)
        //{
        //    try
        //    {
        //        string sMaxNo = string.Empty;
        //        string sFinancialYear = string.Empty;
        //        string sSubDivCode = objCon.get_value(" SELECT NVL(MAX(SD_SUBDIV_CODE),0)+1 FROM TBLSUBDIVMAST  where SD_DIV_CODE='" + objSubDivision.sDivCode + "'");
        //        if (sSubDivCode.Length > 0)
        //        {
        //            sSubDivCode = sSubDivCode.ToString();
        //        }
        //        return sSubDivCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GenerateDivCode");
        //        return "";
        //    }
        //}

        public string GenerateSubDivCode(clsSubDiv objSubDivision)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sSubDivCode = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("subDivId1", objSubDivision.sDivCode);
                sSubDivCode = Objcon.get_value("SELECT COALESCE(MAX(\"SD_SUBDIV_CODE\"),0)+1 FROM \"TBLSUBDIVMAST\"  where cast(\"SD_DIV_CODE\" as text) = :subDivId1", NpgsqlCommand);
                if (Convert.ToInt16(sSubDivCode) <= 1)
                {
                    sSubDivCode = objSubDivision.sDivCode + "1";
                }
            }
            catch (Exception ex)
             {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return sSubDivCode;
        }

    }
}
