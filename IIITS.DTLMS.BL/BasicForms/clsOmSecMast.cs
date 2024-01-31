using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{

    public class clsOmSecMast
    {
        
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        string strQry = string.Empty;
        string strFormCode = "clsOmSecMast";
        public string sSubDivCode { get; set; }



        //public string[] SaveOmSecMastDetails(string strOmCode, string strOmName, string strSubDivCode, string strOmHeadEmp, string strOmMobile, string strUserLogged)
        //{
        //    string[] Arrmsg = new string[2];
        //    try
        //    {

        //        OleDbDataReader dr = objCon.Fetch("select * from TBLOMSECMAST where OM_NAME='" + strOmName.Replace("'", "''") + "'");
        //        if (dr.Read())
        //        {
        //            Arrmsg[0] = "OmSec With This Name Already Exists";
        //            Arrmsg[1] = "4";
        //            return Arrmsg;
                    
        //        }
        //        dr.Close();
        //        OleDbDataReader dr1 = objCon.Fetch("select * from TBLOMSECMAST where OM_CODE='" + strOmCode + "'");
        //        if (dr1.Read())
        //        {
        //            Arrmsg[0] = "OmSec With This Code Already Exists";
        //            Arrmsg[1] = "4";
        //            return Arrmsg;
                   
        //        }
        //        dr1.Close();

              

        //        string strInsqry = "insert into TBLOMSECMAST(OM_SLNO,OM_CODE,OM_NAME,OM_SUBDIV_CODE,OM_HEAD_EMP,OM_MOBILE_NO,OM_ENTRY_AUTH)";
        //        strInsqry += " values(" + objCon.Get_max_no("OM_SLNO", "TBLOMSECMAST") + ",'" + strOmCode.ToUpper() + "','" + strOmName.ToUpper().Replace("'", "''") + "',";
        //       strInsqry += "  '" + strSubDivCode.ToUpper() + "','" + strOmHeadEmp.ToUpper().Replace("'", "''") + "','" + strOmMobile.ToUpper() + "','" + strUserLogged + "')";
        //        objCon.Execute(strInsqry);

        //        Arrmsg[0] = "O&M Section Information has been Saved Sucessfully.";
        //        Arrmsg[1] = "0";
        //        return Arrmsg;
               
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "SaveOmSecMastDetails");
        //        return Arrmsg;
        //    }
        //}

        public string[] SaveOmSecMastDetails(string strOmCode, string strOmName, string strSubDivCode, string strOmHeadEmp, string strOmMobile, string strUserLogged,string Adress)
        {
            string[] Arr = new string[2];
            string strId = string.Empty;
        
            try
            {

                NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_omSecMast");
                cmd.Parameters.AddWithValue("om_id", strId);
                cmd.Parameters.AddWithValue("om_name", strOmName);
                cmd.Parameters.AddWithValue("om_code", strOmCode);
                cmd.Parameters.AddWithValue("om_heademp", strOmHeadEmp);
                cmd.Parameters.AddWithValue("subdiv_code", strSubDivCode);
                

                cmd.Parameters.AddWithValue("om_mobile", strOmMobile);
                cmd.Parameters.AddWithValue("user_logged", strUserLogged);
                cmd.Parameters.AddWithValue("om_address", Adress);

                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);


                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                Arr[0] = "msg";
                Arr[1] = "op_id";
                
                Arr = Objcon.Execute(cmd, Arr, 2);
               
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Arr;

        }
        //public string[] UpdateOmSecMastDetails(string strOldId, string strOmCode, string strOmName, string strSubDivCode, string strOmHeadEmp, string strOmMobile, string strUserLogged)
        //{
        //    string[] Arrmsg = new string[2];
        //    try
        //    {
        //        OleDbDataReader dr = objCon.Fetch("select * from TBLOMSECMAST where OM_NAME='" + strOmName.Replace("'", "''") + "' and OM_SLNO<> '" + strOldId + "'");                
        //        if (dr.Read())
        //        {
        //            Arrmsg[0] = "OmSec With This Name Already Exists";
        //            Arrmsg[1] = "4";
        //            return Arrmsg;
        //        }
        //        OleDbDataReader dr1 = objCon.Fetch("select * from TBLOMSECMAST where OM_CODE='" + strOmCode + "' and OM_SLNO<> '" + strOldId + "'");
        //        if (dr1.Read())
        //        {
        //            Arrmsg[0] = "OmSec With This Code Already Exists";
        //            Arrmsg[1] = "4";
        //            return Arrmsg;
        //        }
        //        string strUpdqry = "update TBLOMSECMAST set OM_CODE='" + strOmCode.ToUpper() + "',OM_NAME ='" + strOmName.ToUpper().Replace("'", "''") + "',OM_SUBDIV_CODE='" + strSubDivCode.ToUpper() + "',OM_HEAD_EMP='" + strOmHeadEmp.ToUpper().Replace("'", "''") + "',OM_MOBILE_NO='" + strOmMobile.ToUpper() + "',OM_ENTRY_AUTH='" + strUserLogged + "',OM_ENTRY_DATE=SYSDATE where OM_SLNO = '" + strOldId.ToUpper() + "'";
        //        objCon.Execute(strUpdqry);

        //        Arrmsg[0] = "O&M Section Information has been Updated Sucessfully.";
        //        Arrmsg[1] = "0";
        //        return Arrmsg;
                            
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "UpdateOmSecMastDetails");
        //        return Arrmsg;
        //    }
        //}

        public string[] UpdateOmSecMastDetails(string strOldId, string strOmCode, string strOmName, string strSubDivCode, string strOmHeadEmp, string strOmMobile, string strUserLogged,string adress)
        {
            string[] Arr = new string[2];
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_omSecMast");
                cmd.Parameters.AddWithValue("om_id", strOldId);
                cmd.Parameters.AddWithValue("om_name", strOmName);
                cmd.Parameters.AddWithValue("om_code", strOmCode);
                cmd.Parameters.AddWithValue("om_heademp", strOmHeadEmp);
                cmd.Parameters.AddWithValue("subdiv_code", strSubDivCode);
                cmd.Parameters.AddWithValue("om_mobile", strOmMobile);
                cmd.Parameters.AddWithValue("user_logged", strUserLogged);
                cmd.Parameters.AddWithValue("om_address", adress);

                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);


                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                Arr[0] = "msg";
                Arr[1] = "op_id";

                Arr = Objcon.Execute(cmd, Arr, 2);
            

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Arr;

        }
        //public DataTable LoadOmSecvOffDet(string strOmSecID = "")
        //{
        //    DataTable DtDivOffDet = new DataTable();
        //    try
        //    {
        //        strQry = "SELECT to_char(OM_SLNO) OM_SLNO,To_char(OM_CODE)OM_CODE,OM_NAME,OM_SUBDIV_CODE,";
        //       strQry += " SD_SUBDIV_NAME,OM_HEAD_EMP,OM_MOBILE_NO FROM TBLSUBDIVMAST,TBLOMSECMAST where OM_SUBDIV_CODE=SD_SUBDIV_CODE ";
        //        if (strOmSecID != "")
        //        {
        //            strQry += " and OM_SLNO='" + strOmSecID + "'";
        //        }
        //        strQry += "  order by OM_CODE";
        //        OleDbDataReader drcorp = objCon.Fetch(strQry);
        //        DtDivOffDet.Load(drcorp);

        //        return DtDivOffDet;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "LoadOmSecvOffDet");
        //        return DtDivOffDet;
        //    }
        //  }

        public DataTable LoadOmSecvOffDet(string strOmSecID = "")
        {
            DataTable Dt = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_load_omsecoff_details");
                cmd.Parameters.AddWithValue("om_sec_id", strOmSecID);
                Dt = Objcon.FetchDataTable(cmd);
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Dt;
        }

        //public string GenerateOmSecCode(clsOmSecMast objOmSec)
        //{
        //    try
        //    {
        //        string sMaxNo = string.Empty;
        //        string sFinancialYear = string.Empty;
        //        string sOmSecCode = objCon.get_value(" SELECT NVL(MAX(OM_CODE),0)+1 FROM TBLOMSECMAST  where OM_SUBDIV_CODE='" + objOmSec.sSubDivCode + "'");
        //        if (sOmSecCode.Length > 0)
        //        {
        //            sSubDivCode = sOmSecCode.ToString();
        //        }
        //        return sOmSecCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GenerateOmSecCode");
        //        return "";
        //    }
        //}
        NpgsqlCommand NpgsqlCommand;
        public string GenerateOmSecCode(clsOmSecMast objOmSec)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sOmSecCode = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("subdivcode", objOmSec.sSubDivCode);
                sOmSecCode = Objcon.get_value(" SELECT CAST(COALESCE(MAX(\"OM_CODE\"),0)+1 AS TEXT) FROM \"TBLOMSECMAST\"  where CAST(\"OM_SUBDIV_CODE\"AS TEXT) like :subdivcode||'%'", NpgsqlCommand);
                //sOmSecCode = Convert.ToString(Objcon.Get_max_no("OM_CODE", "TBLOMSECMAST"));
               if (Convert.ToInt16(sOmSecCode) <= 1)
               {
                   sOmSecCode = objOmSec.sSubDivCode + "1";
               }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return sOmSecCode;
        }
    }
}




