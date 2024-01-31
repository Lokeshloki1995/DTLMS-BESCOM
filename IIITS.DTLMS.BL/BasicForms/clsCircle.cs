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
    public class clsCircle                      
    {
        
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        string strFormCode = "clsCircle";
        public string sCircleCode { get; set; }
        public string sCircleName { get; set; }
        public string sName { get; set; }
        public string sPhone { get; set; }
        public string sMobileNo { get; set; }
        public string sEmail { get; set; }
        public string sMaxid { get; set; }
        public string sZoneCode { get; set; }
        public string sAddress { get; set; }



        //public string[] SaveCircle(clsCircle objCircle)
        //{
        //    string[] Arr = new string[2];
        //    try
        //    {
        //        string strQry = string.Empty;
        //        if (objCircle.sMaxid == "")
        //        {
        //            objCircle.sMaxid = objcon.Get_max_no("CM_ID", "TBLCIRCLE").ToString();
        //            OleDbDataReader dr = objcon.Fetch("Select * from TBLCIRCLE where UPPER(CM_CIRCLE_NAME) ='" + objCircle.sCircleName.ToUpper() + "'");
        //            if (dr.Read())
        //            {
        //                dr.Close();
        //                Arr[0] = "Circle Name already exists";
        //                Arr[1] = "3";
        //                return Arr;
        //            }
        //            dr.Close();

        //            dr = objcon.Fetch("Select * from TBLCIRCLE where UPPER(CM_CIRCLE_CODE) ='" + objCircle.sCircleCode.ToUpper() + "'");
        //            if (dr.Read())
        //            {
        //                dr.Close();
        //                Arr[0] = "Circle Code already exists";
        //                Arr[1] = "3";
        //                return Arr;
        //            }
        //            dr.Close();

        //            strQry = "INSERT INTO TBLCIRCLE(CM_ID, CM_CIRCLE_CODE ,CM_CIRCLE_NAME,CM_ZO_ID,CM_HEAD_EMP,CM_MOBILE_NO,CM_PHONE,CM_EMAIL)";
        //            strQry += "VALUES('" + objCircle.sMaxid + "','" + objCircle.sCircleCode + "',";
        //            strQry += " '" + objCircle.sCircleName + "',1,'" + objCircle.sName + "',";
        //            strQry += " '" + objCircle.sMobileNo + "','" + objCircle.sPhone + "','" + objCircle.sEmail + "'  )";
        //            objcon.Execute(strQry);
        //            Arr[0] = "Saved Successfully ";
        //            Arr[1] = "0";
        //            return Arr;
        //        }
        //        else
        //        {
        //            OleDbDataReader dr = objcon.Fetch("Select * from TBLCIRCLE where UPPER(CM_CIRCLE_NAME) ='" + objCircle.sCircleName.ToUpper() + "' and CM_ID  <> '" + objCircle.sMaxid + "'");
        //            if (dr.Read())
        //            {
        //                dr.Close();
        //                Arr[0] = "Circle Name already exists";
        //                Arr[1] = "3";
        //                return Arr;
        //            }
        //            dr.Close();

        //            dr = objcon.Fetch("Select * from TBLCIRCLE where UPPER(CM_CIRCLE_CODE) ='" + objCircle.sCircleCode.ToUpper() + "' and CM_ID <> '" + objCircle.sMaxid + "'");
        //            if (dr.Read())
        //            {
        //                dr.Close();
        //                Arr[0] = "Circle Code already exists";
        //                Arr[1] = "3";
        //                return Arr;
        //            }
        //            dr.Close();

        //            strQry = " UPDATE TBLCIRCLE SET CM_CIRCLE_CODE='" + objCircle.sCircleCode + "', CM_CIRCLE_NAME= '" + objCircle.sCircleName + "', ";
        //            strQry += " CM_HEAD_EMP='" + objCircle.sName + "',CM_MOBILE_NO='" + objCircle.sMobileNo + "', CM_PHONE='" + objCircle.sPhone + "',";
        //            strQry += " CM_EMAIL='" + objCircle.sEmail + "' WHERE CM_ID = '" + objCircle.sMaxid + "' ";
        //            objcon.Execute(strQry);
        //            Arr[0] = "Updated Successfully ";
        //            Arr[1] = "1";
        //            return Arr;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "SaveCircle");
        //        return Arr;
        //    }
        //}
        
        public string[] SaveCircle(clsCircle objCircle)
        {
           
            string[] Arr = new string[3];
            try
            {                
                  NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_circle");
                    cmd.Parameters.AddWithValue("cir_id", objCircle.sMaxid);
                    cmd.Parameters.AddWithValue("cir_code", objCircle.sCircleCode);
                    cmd.Parameters.AddWithValue("cir_name", objCircle.sCircleName.ToUpper());
                    cmd.Parameters.AddWithValue("cir_head_name", objCircle.sName);                 
                    cmd.Parameters.AddWithValue("cir_mobile_number", objCircle.sMobileNo);
                    cmd.Parameters.AddWithValue("cir_phone", objCircle.sPhone);
                    cmd.Parameters.AddWithValue("cir_email", objCircle.sEmail);
                    cmd.Parameters.AddWithValue("cir_zo_id", objCircle.sZoneCode);
                    cmd.Parameters.AddWithValue("cm_address", objCircle.sAddress);
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

        //public DataTable LoadAllCircleDetails()
        //{
        //    string strQry = string.Empty;
        //    DataTable dt = new DataTable();
            
        //    try
        //    {
        //        strQry = "SELECT CM_ID, To_char(CM_CIRCLE_CODE),CM_CIRCLE_NAME FROM TBLCIRCLE";
        //        dt = objcon.getDataTable(strQry);
        //       return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //       clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "LoadAllCircleDetails");
        //      return dt;
        //        
        //    }
        //    
        //}


        public DataTable LoadAllCircleDetails()
        {
            DataTable dt = new DataTable();
          
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_get_circle_toload");
                dt = Objcon.FetchDataTable(cmd);
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
            return dt;
        }
        //public object getCircleDetails(clsCircle objCircle)
        //{
        //    DataTable dtDetails = new DataTable();
        //    //OleDbDataReader dr = null;
        //    object outcome;
        //   string strQry = string.Empty;
        //    string sQryValue = string.Empty;
        //    try
        //    {

        //        strQry = "SELECT \"CM_CIRCLE_CODE\" ,\"CM_CIRCLE_NAME\",\"CM_HEAD_EMP\",\"CM_MOBILE_NO\",\"CM_PHONE\",\"CM_EMAIL\" FROM \"TBLCIRCLE\" ";
        //        strQry += " WHERE CM_ID ='" + objCircle.sMaxid + "'";
        //        dtDetails = Objcon.FetchDataTable(strQry);
        //        //dr = Objcon.Fetch(strQry);
        //      //  dtDetails.Load(dr);

        //        if (dtDetails.Rows.Count > 0)
        //        {
        //            objCircle.sCircleName = Convert.ToString(dtDetails.Rows[0]["CM_CIRCLE_NAME"].ToString());
        //            objCircle.sName = Convert.ToString(dtDetails.Rows[0]["CM_HEAD_EMP"].ToString());
        //            objCircle.sMobileNo = Convert.ToString(dtDetails.Rows[0]["CM_MOBILE_NO"].ToString());
        //            objCircle.sPhone = Convert.ToString(dtDetails.Rows[0]["CM_PHONE"].ToString());
        //            objCircle.sEmail = Convert.ToString(dtDetails.Rows[0]["CM_EMAIL"].ToString());
        //            objCircle.sCircleCode = Convert.ToString(dtDetails.Rows[0]["CM_CIRCLE_CODE"].ToString());
        //        }
        //        outcome= objCircle;
        //    }


        //    catch (Exception ex)
        //    {
        //        //clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "getCircleDetails");
        //        clsException.LogError("Query - " + strQry, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        //        outcome=objCircle;
        //    }
        //    return outcome;
        //}

        public object getCircleDetails(clsCircle objCircle)
        {
            DataTable dtDetails = new DataTable();
            try
            {

                NpgsqlCommand cmd = new NpgsqlCommand("proc_get_circle_details");
                cmd.Parameters.AddWithValue("cir_id", objCircle.sMaxid);
                dtDetails = Objcon.FetchDataTable(cmd);
               

                if (dtDetails.Rows.Count > 0)
                {
                    objCircle.sCircleCode = Convert.ToString(dtDetails.Rows[0]["CM_CIRCLE_CODE"]);
                    objCircle.sCircleName = Convert.ToString(dtDetails.Rows[0]["CM_CIRCLE_NAME"]);
                    objCircle.sName = Convert.ToString(dtDetails.Rows[0]["CM_HEAD_EMP"]);
                    objCircle.sMobileNo = Convert.ToString(dtDetails.Rows[0]["CM_MOBILE_NO"]);
                    objCircle.sPhone = Convert.ToString(dtDetails.Rows[0]["CM_PHONE"]);
                    objCircle.sEmail = Convert.ToString(dtDetails.Rows[0]["CM_EMAIL"]);
                    objCircle.sZoneCode = Convert.ToString(dtDetails.Rows[0]["CM_ZO_ID"]);
                    objCircle.sAddress = Convert.ToString(dtDetails.Rows[0]["CM_ADDRESS"]);
                    
                }            
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
            return objCircle;
        }
        //public string GenerateCircleCode()
        //{
        //    string strQry = string.Empty;
        //    string outcome = string.Empty;
        //    try
        //    {
        //        string sMaxNo = string.Empty;
        //        string sFinancialYear = string.Empty;
        //        strQry = " SELECT COALESCE(MAX(\"CM_CIRCLE_CODE\"),0)+1 FROM \"TBLCIRCLE\" ";
        //        string sCircleCodeNo = Objcon.get_value(strQry);
        //        if (sCircleCodeNo.Length > 0)
        //        {
        //            sCircleCode = sCircleCodeNo.ToString();
        //        }
        //       outcome= sCircleCodeNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError("Query - " + strQry, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        //       // clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GenerateCircleCode");
        //        outcome=" ";
        //    }
        //    return outcome;
        //}


        public string GenerateCircleCode()
        {
            string sCircleCodeNo = string.Empty;
            
            try
            {
               sCircleCodeNo  = Convert.ToString(Objcon.Get_max_no("CM_CIRCLE_CODE", "TBLCIRCLE"));
                    
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
            return sCircleCodeNo;
        }

        NpgsqlCommand NpgsqlCommand;
        public string GenerateCirCode(clsCircle objCir)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sZoneCodeNo = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("ZoneCode", objCir.sZoneCode);
                string strQry = " SELECT CAST(COALESCE(MAX(\"CM_CIRCLE_CODE\"),0)+1 AS TEXT) FROM \"TBLCIRCLE\"  where CAST(\"CM_ZO_ID\"AS TEXT) like :ZoneCode||'%'";
                sZoneCodeNo = Objcon.get_value(strQry, NpgsqlCommand);
                if (Convert.ToInt16(sZoneCodeNo) <= 1)
                {
                    sZoneCodeNo = objCir.sZoneCode + "1";
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return sZoneCodeNo;
        }

    }
}
