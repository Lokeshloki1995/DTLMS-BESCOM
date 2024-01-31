using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsDeviceRegister
    {
        string strFormCode = "clsUser";
        public string sUserId { get; set; }
        public string sFullName { get; set; }
        public string sMuId { get; set; }
        public string sDeviceId { get; set; }
        public string sRequestedBy { get; set; }
        public string sApprovalStatus { get; set; }
        public string sApprovedBy { get; set; }
        public string sCrOn { get; set; }



        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);

        //public DataTable LoadDeviceGrid(clsDeviceRegister objdevice)
        //{
        //    DataTable dtUserDetails = new DataTable();

        //    string strQry = string.Empty;
        //    try
        //    {
        //        strQry = "select US_ID,MR_REQUEST_BY,MR_ID,MR_DEVICE_ID,US_FULL_NAME, DECODE(MR_APPROVE_STATUS,'1','APPROVED','PENDING')MR_APPROVE_STATUS,TO_CHAR(MR_CRON,'DD-MON-YYYY')MR_CRON ";
        //        strQry += " FROM TBLMOBILEREGISTER,TBLUSER  where  ";


        //        if (objdevice.sDeviceId != null)
        //        {
        //            strQry += " UPPER(MR_DEVICE_ID) like '%" + objdevice.sDeviceId.ToUpper() + "%' and";
        //        }
        //        if (objdevice.sFullName != null)
        //        {
        //            strQry += " UPPER(US_FULL_NAME) like '%" + objdevice.sFullName.ToUpper() + "%' and";
        //        }

        //        strQry += " US_ID=MR_REQUEST_BY";
        //        dtUserDetails = objCon.getDataTable(strQry);
        //        return dtUserDetails;

        //    }
        //    catch (Exception ex)
        //    {

        //        clsException.LogError(ex.StackTrace,ex.Message, strFormCode, "LoadDeviceGrid");
        //        return dtUserDetails;

        //    }
        //}
        NpgsqlCommand NpgsqlCommand;
        public DataTable LoadDeviceGrid(clsDeviceRegister objdevice)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                
                sQry = "SELECT \"US_ID\",\"MR_REQUEST_BY\",\"MR_ID\",\"MR_DEVICE_ID\",\"US_FULL_NAME\",CASE WHEN \"MR_APPROVE_STATUS\"='1' THEN 'APPROVED' ELSE 'PENDING' END AS \"MR_APPROVE_STATUS\","+
                       " TO_CHAR(\"MR_CRON\",'DD-MON-YYYY')\"MR_CRON\" FROM \"TBLMOBILEREGISTER\" INNER JOIN \"TBLUSER\" ON \"US_ID\" = \"MR_REQUEST_BY\"";

                if (objdevice.sDeviceId != null)
                {
                    NpgsqlCommand.Parameters.AddWithValue("mrdeviceId", objdevice.sDeviceId.ToUpper());
                    sQry += " WHERE UPPER(\"MR_DEVICE_ID\") LIKE :mrdeviceId||'%' AND";
                }
                if (objdevice.sFullName != null)
                {
                    if (sQry.Contains("WHERE"))
                    {
                        NpgsqlCommand.Parameters.AddWithValue("usfullName", objdevice.sFullName.ToUpper());
                        sQry += " UPPER(\"US_FULL_NAME\") LIKE :usfullName||'%'";
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("usfullName1", objdevice.sFullName.ToUpper());
                        sQry += " WHERE UPPER(\"US_FULL_NAME\") LIKE :usfullName1||'%'";
                    }                    
                }
                dt = Objcon.FetchDataTable(sQry, NpgsqlCommand);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return dt;
        }


        public bool UpdateDeviceStatus(clsDeviceRegister objdevice)
        {

            string[] iReturn = new string[0];
            bool bRes = false;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_update_device_details");
                cmd.Parameters.AddWithValue("requested_by", objdevice.sRequestedBy);
                cmd.Parameters.AddWithValue("approved_by", objdevice.sApprovedBy);
                cmd.Parameters.AddWithValue("mu_id", objdevice.sMuId);

                 Objcon.Execute(cmd, iReturn,0);
                 bRes = true;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                bRes = false;
            }
            return bRes;
        }
    }
}
