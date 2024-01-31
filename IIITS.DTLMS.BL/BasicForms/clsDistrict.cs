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
   public class clsDistrict
    {
       string strFormCode = "ClsDistrict";
       //CustOledbConnection objcon = new CustOledbConnection(Constants.Password);
       PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
       public string sDistId { get; set; }
       public DataTable sDt = new DataTable();
       public string sDistrictCode { get; set; }
       public string sDistrictName { get; set; }
       public string sButtonname { get; set; }
       public string sOfficeCode { get; set; }
       public string sOfficeName { get; set; }
       public string sSlNo { get; set; }

       NpgsqlCommand NpgsqlCommand;                                                               
        //public string[] SaveDetails(clsDistrict objDis)
        //{
        //    string strQry=string.Empty;
        //    string[] Arr = new string[3];
        //    OleDbDataReader dr = null;
        //    try
        //    {

        //            if (objDis.sDistId == "")
        //            {
        //                strQry = "select * from tbldist where UPPER(DT_CODE)='" + objDis.sDistrictCode.ToUpper() + "'";
        //                dr = objcon.Fetch(strQry);
        //                if (dr.Read())
        //                {
        //                    Arr[0] = "District Code AlreadyExist ";
        //                    Arr[1] = "1";
        //                    return Arr;
        //                }
        //                dr.Close();

        //                strQry = "select * from tbldist where UPPER(DT_NAME)='" + objDis.sDistrictName.ToUpper() + "'";
        //                dr = objcon.Fetch(strQry);
        //                if (dr.Read())
        //                {
        //                    Arr[0] = "District Name AlreadyExist ";
        //                    Arr[1] = "1";
        //                    return Arr;
        //                }
        //                dr.Close();

        //                objDis.sDistId = objcon.Get_max_no("DT_ID", "TBLDIST").ToString();
        //                strQry = "insert into tbldist (DT_CODE,DT_NAME,DT_ID) values('" + objDis.sDistrictCode + "',";
        //                strQry += "'" + objDis.sDistrictName.Trim().Replace(" ", "") + "','" + objDis.sDistId + "')";
        //                objcon.Execute(strQry);
        //                Arr[0] = "Saved Succesfully";
        //                Arr[1] = "0";
        //                return Arr;
        //            }
        //            else
        //            {

        //                strQry = "select * from tbldist where UPPER(DT_NAME)='" + objDis.sDistrictName.ToUpper() + "' and DT_CODE <> '" + objDis.sDistrictCode + "'";
        //                dr = objcon.Fetch(strQry);
        //                if (dr.Read())
        //                {
        //                    Arr[0] = "District Name AlreadyExist ";
        //                    Arr[1] = "1";
        //                    return Arr;
        //                }
        //                dr.Close();

        //                strQry = "update tbldist set ";
        //                strQry += " DT_NAME='" + objDis.sDistrictName.Trim().Replace(" ", "") + "' where DT_ID='" + objDis.sDistId + "'";
        //                objcon.Execute(strQry);
        //                Arr[0] = "Updated Successfully ";
        //                Arr[1] = "1";
        //                return Arr;
        //            }
              
        //    }
        //    catch (Exception ex)
        //    {
        //     //  clsException.LogError("Procedure - "+ ex.StackTrace, ex.Message, sformcode, "GetDistDetails");
        //        clsException.LogError("Query - "+strQry, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return Arr;
        //}
       
        public string[] SaveDetails(clsDistrict objDis)                                               
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            try
            { 

                string[] strQryVallist = null;

                if (objDis.sOfficeCode != "")
                {
                    strQryVallist = objDis.sOfficeCode.Split(',');
                }

                if (objDis.sDistId.Length == 0)
                {
                    foreach (string OfficeCode in strQryVallist)
                    {
                        string sQry = string.Empty;
                        NpgsqlCommand.Parameters.AddWithValue("offcode",Convert.ToInt32(OfficeCode));
                        sQry = "SELECT \"DD_ID\" FROM \"TBLDISTTODIVMAPPING\" WHERE \"DD_DIV_ID\"  = :offcode";
                        string sResult = Objcon.get_value(sQry, NpgsqlCommand);
                        if (sResult.Length > 0)
                        {
                            Arr[0] = "Location Already Allocated to some other District";
                            Arr[1] = "4";
                            return Arr;
                        }
                    }
                }
                else
                {
                    foreach (string OfficeCode in strQryVallist)
                    {
                        string sQry = string.Empty;
                        NpgsqlCommand.Parameters.AddWithValue("offcode1",Convert.ToInt32(OfficeCode));
                        NpgsqlCommand.Parameters.AddWithValue("slno", Convert.ToInt32(objDis.sDistId));
                        sQry = "SELECT \"DD_ID\" FROM \"TBLDISTTODIVMAPPING\" WHERE  \"DD_DIV_ID\" =:offcode1 AND \"DD_DIST_ID\" <> :slno";
                        string sResult = Objcon.get_value(sQry, NpgsqlCommand);
                        if (sResult.Length > 0)
                        {
                            Arr[0] = "Location Already Allocated to some other District";
                            Arr[1] = "4";
                            return Arr;
                        }
                    }
                }

                    NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_district");
                    cmd.Parameters.AddWithValue("dt_code", objDis.sDistrictCode);
                    cmd.Parameters.AddWithValue("dt_name", objDis.sDistrictName.ToUpper());    
                    cmd.Parameters.AddWithValue("dt_id", objDis.sDistId);

                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                    cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);

                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                    cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;

                    Arr[0] = "msg";
                    Arr[1] = "op_id";
                    Arr[2] = "pk_id";
                   
                    Arr = Objcon.Execute(cmd, Arr, 3);
                    if ((Arr[1] == "0" || Arr[1] == "2") && Arr[2].Length > 0)
                    {
                        if (strQryVallist.Length > 0 )
                        {
                            string sQry = string.Empty;
                            NpgsqlCommand.Parameters.AddWithValue("arr", Convert.ToInt32(Arr[2]));
                            sQry = "DELETE FROM \"TBLDISTTODIVMAPPING\" WHERE \"DD_DIST_ID\" =:arr";
                            Objcon.ExecuteQry(sQry, NpgsqlCommand);
                        }

                        foreach (string OfficeCode in strQryVallist)
                        {
                            string sQry = string.Empty;
                            int sMaxNo = Convert.ToInt32(Objcon.Get_max_no("DD_ID", "TBLDISTTODIVMAPPING"));
                            // NpgsqlCommand.Parameters.AddWithValue("Getmaxno",Convert.ToInt32( Objcon.Get_max_no("STO_ID", "TBLSTOREOFFCODE")));
                            // NpgsqlCommand.Parameters.AddWithValue("arr1",Convert.ToInt32(Arr[2]));
                            //NpgsqlCommand.Parameters.AddWithValue("offcode",Convert.ToInt32( OfficeCode));
                            sQry = "INSERT INTO \"TBLDISTTODIVMAPPING\"(\"DD_ID\", \"DD_DIST_ID\", \"DD_DIV_ID\")";
                            sQry += " VALUES('" + sMaxNo + "','" + Convert.ToInt32(Arr[2]) + "','" + Convert.ToInt32(OfficeCode) + "')";
                            Objcon.ExecuteQry(sQry);
                            //NpgsqlCommand.Parameters.AddWithValue("Getmaxno", Convert.ToInt32(Objcon.Get_max_no("DD_ID", "TBLDISTTODIVMAPPING")));
                            //NpgsqlCommand.Parameters.AddWithValue("arr1", Convert.ToInt32(Arr[2]));
                            //NpgsqlCommand.Parameters.AddWithValue("offcode", Convert.ToInt32(OfficeCode));
                            //sQry = "INSERT INTO \"TBLDISTTODIVMAPPING\"(\"DD_ID\", \"DD_DIST_ID\", \"DD_DIV_ID\")";
                            //sQry += " VALUES(:Getmaxno,:arr1,:offcode)";
                            //Objcon.ExecuteQry(sQry, NpgsqlCommand);
                        }
                    }
                    return Arr;
                   
            }
            catch (Exception ex)
            {

                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Arr;
        }
                      


        //public object GetDistDetails(clsDistrict objDistrict)
        //{

        //    DataTable dtDetails = new DataTable();
        //    try
        //    {

        //        String strQry = "SELECT * FROM TBLDIST ";
        //        strQry += " WHERE DT_ID='" + objDistrict.sDistId + "'";
        //        dtDetails = objcon.getDataTable(strQry);

        //        if (dtDetails.Rows.Count > 0)
        //        {
        //            objDistrict.sDistrictCode = Convert.ToString(dtDetails.Rows[0]["DT_CODE"].ToString());
        //            objDistrict.sDistrictName = Convert.ToString(dtDetails.Rows[0]["DT_NAME"].ToString());
        //        }
        //        return objDistrict;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "GetDistDetails");
        //        return objDistrict;
        //    }
        //}

       public object GetDistDetails(clsDistrict objDistrict)
        {
           
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_load_district_detailsbyid");
                cmd.Parameters.AddWithValue("dt_id", objDistrict.sDistId);
                dtDetails = Objcon.FetchDataTable(cmd);
                if (dtDetails.Rows.Count > 0)
                {
                    objDistrict.sDistrictCode = Convert.ToString(dtDetails.Rows[0]["dt_code"]);
                    objDistrict.sDistrictName = Convert.ToString(dtDetails.Rows[0]["dt_name"]);
                    
                }
                String strQry = "SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" ,\"TBLDISTTODIVMAPPING\"  where  \"DIV_CODE\"=\"DD_DIV_ID\" AND \"DD_DIST_ID\"='"+objDistrict.sDistId+"' ";

                objDistrict.sDt = Objcon.FetchDataTable(strQry);      
             
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
            return objDistrict;
        }

        //public DataTable LoadAllDistDetails()
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        string strQry = string.Empty;
        //        strQry = "SELECT DT_ID,To_char(DT_CODE)DT_CODE,DT_NAME FROM TBLDIST ORDER BY DT_ID";
        //        dt = objcon.getDataTable(strQry);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "LoadAllDistDetails");
        //        return dt;
        //    }

        //}
       public DataTable LoadOfficeDet(clsDistrict objDist)
       {
           NpgsqlCommand = new NpgsqlCommand();
           DataTable dtLocation = new DataTable();
           try
           {
               string strQry = string.Empty;


               strQry = "select \"OFF_CODE\",\"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE  \"OFF_NAME\" IS NOT NULL AND LENGTH(CAST(\"OFF_CODE\" AS TEXT)) = 3 ";
               if (objDist.sOfficeCode != "")
               {
                   NpgsqlCommand.Parameters.AddWithValue("offcode", objDist.sOfficeCode);
                   strQry += " AND CAST(\"OFF_CODE\" AS TEXT) LIKE :offcode||'%'";
               }
               if (objDist.sOfficeName != "")
               {
                   NpgsqlCommand.Parameters.AddWithValue("offname", objDist.sOfficeName.ToUpper());
                   strQry += " AND UPPER(CAST(\"OFF_NAME\" AS TEXT)) LIKE :offname||'%'";
               }
               strQry += " order by \"OFF_CODE\"";
               dtLocation = Objcon.FetchDataTable(strQry, NpgsqlCommand);
               return dtLocation;
           }
           catch (Exception ex)
           {
               clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               return dtLocation;
           }
       }
        public DataTable LoadAllDistDetails()
        {
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("proc_load_district_details");
                dt = Objcon.FetchDataTable(cmd); 
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

            }
            return dt;
        }

       /* public string GenerateDistrictCode()
        {
            try
            {
                string sMaxNo = string.Empty;
                string sFinancialYear = string.Empty;
                string sDistCode = objcon.get_value(" SELECT NVL(MAX(DT_CODE),0)+1 FROM TBLDIST ");
                if (sDistCode.Length > 0)
                {
                    sDistrictCode = sDistCode.ToString();
                }
                return sDistCode;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, sformcode, "GenerateCircleCode");
                return "";
            }
        }*/

       public string GenerateDistrictCode()
       {
           string sDistCode = string.Empty;
           try
            { 
                 sDistCode =  Convert.ToString(Objcon.Get_max_no("DT_CODE", "TBLDIST"));
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
           return sDistCode;
        }
    }
}
