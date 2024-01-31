using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Npgsql;
using System.Reflection;
using NpgsqlTypes;
using IIITS.PGSQL.DAL;

namespace IIITS.DTLMS.BL
{
    public class clsTaluk
    {
        string strFormCode = "ClsTaluk";
        //CustOledbConnection objcon = new CustOledbConnection(Constants.Password);
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);
        public string sTalukId { get; set; }
        public string sDistrictName { get; set; }
        public string sTalukCode { get; set; }
        public string sTalukName { get; set; }
        public string sButtonName { get; set; }

        //public string[] SaveDetails(clsTaluk objTlk)
        //{
        //    string[] Arr = new string[3];
        //    string strQry = string.Empty;
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        if (objTlk.sButtonName == "Save")
        //        {
        //            if (objTlk.sTalukId == "")
        //            {
        //                strQry = "select * from TBLTALQ where UPPER(TQ_CODE)='" + objTlk.sTalukCode.ToUpper() + "'";
        //                dt = objcon.getDataTable(strQry);
        //                if (dt.Rows.Count > 0)
        //                {
        //                    Arr[0] = "Taluk Code Exist ";
        //                    Arr[1] = "1";
        //                    return Arr;
        //                }

        //                strQry = "select * from TBLTALQ where UPPER(TQ_NAME)='" + objTlk.sTalukName.ToUpper() + "'";
        //                dt = objcon.getDataTable(strQry);
        //                if (dt.Rows.Count > 0)
        //                {
        //                    Arr[0] = "Taluk Name Exist ";
        //                    Arr[1] = "1";
        //                    return Arr;
        //                }

        //                string SMaxid = objcon.Get_max_no("TQ_SLNO", "TBLTALQ").ToString();
        //                strQry = "select DT_CODE from TBLDIST where DT_CODE='" + objTlk.sDistrictName + "'";
        //                string sDistcode = objcon.get_value(strQry);
        //                strQry = "insert into TBLTALQ (TQ_CODE,TQ_NAME,TQ_SLNO,TQ_DT_ID) ";
        //                strQry += " values('" + objTlk.sTalukCode + "','" + objTlk.sTalukName.Trim().Replace(" ", "") + "',";
        //                strQry += " '" + SMaxid + "','" + sDistcode + "')";
        //                objcon.Execute(strQry);
        //                Arr[0] = SMaxid.ToString();
        //                Arr[1] = "2";
        //                Arr[2] = "Saved Succesfully";
        //            }

        //            else
        //            {

        //                    strQry = "select * from TBLDIST where DT_CODE='" + objTlk.sDistrictName + "'";
        //                    dt = objcon.getDataTable(strQry);
        //                    string sDistCode = Convert.ToString(dt.Rows[0]["DT_CODE"].ToString());

        //                    strQry = "select * from TBLTALQ where UPPER(TQ_CODE)='" + objTlk.sTalukCode.ToUpper() + "' and TQ_DT_ID='" + sDistCode + "'";
        //                    dt = objcon.getDataTable(strQry);

        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        if (dt.Rows[0]["TQ_NAME"].ToString() == objTlk.sTalukName.Trim().Replace(" ", ""))
        //                        {
        //                            Arr[0] = "Taluk Name Exist ";
        //                            Arr[1] = "1";
        //                            return Arr;
        //                        }
        //                        else
        //                        {
        //                            strQry = "select DT_CODE from TBLDIST where DT_CODE='" + objTlk.sDistrictName + "'";
        //                            string sDistcode = objcon.get_value(strQry);
        //                            string updateQry = "update TBLTALQ set TQ_CODE='" + objTlk.sTalukCode + "',TQ_NAME='" + objTlk.sTalukName.Trim().Replace(" ", "") + "',";
        //                           updateQry +=" TQ_DT_ID='" + sDistcode + "' where UPPER(TQ_SLNO)='" + objTlk.sTalukId + "'";
        //                            objcon.Execute(updateQry);
        //                            Arr[0] = "Updated Successfully ";
        //                            Arr[1] = "3";
        //                            return Arr;
        //                        }
        //                    }

        //                    else
        //                    {
        //                        string SMaxid = objcon.Get_max_no("TQ_SLNO", "TBLTALQ").ToString();
        //                        strQry = "select DT_CODE from TBLDIST where UPPER(DT_NAME)='" + objTlk.sDistrictName.ToUpper() + "'";
        //                        string sDistcode = objcon.get_value(strQry);
        //                        strQry = "insert into TBLTALQ (TQ_CODE,TQ_NAME,TQ_SLNO,TQ_DT_ID) values('" + objTlk.sTalukCode + "',";
        //                       strQry +=" '" + objTlk.sTalukName.Trim().Replace(" ", "") + "','" + SMaxid + "','" + sDistcode + "')";
        //                        objcon.Execute(strQry);
        //                        Arr[0] = SMaxid.ToString();
        //                        Arr[1] = "2";
        //                        Arr[2] = "Created new taluk code and name";
        //                    }
        //                }
        //            }


        //        else
        //        {


        //            strQry = "select DT_CODE from TBLDIST where DT_CODE='" + objTlk.sDistrictName + "'";
        //            string sDistcode = objcon.get_value(strQry);
        //            string updateQry = "update TBLTALQ set TQ_CODE='" + objTlk.sTalukCode + "',TQ_NAME='" + objTlk.sTalukName.Trim().Replace(" ", "") + "',";
        //            updateQry += " TQ_DT_ID='" + sDistcode + "' where UPPER(TQ_SLNO)='" + objTlk.sTalukId + "'";
        //            objcon.Execute(updateQry);
        //            Arr[0] = "Updated Successfully ";
        //            Arr[1] = "3";
        //            return Arr;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "SaveDetails");
        //    }
        //    return Arr;
        //}

        NpgsqlCommand NpgsqlCommand;
        public string[] SaveDetails(clsTaluk objTlk)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            string strId = string.Empty;
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("DistrictName",Convert.ToInt32(objTlk.sDistrictName));
                strQry = "select \"DT_CODE\" from \"TBLDIST\" where \"DT_CODE\" = :DistrictName";
                string sDistcode = Objcon.get_value(strQry, NpgsqlCommand);
                if (objTlk.sButtonName == "Save")
                {
                    if (objTlk.sTalukId == "")
                    {
                        //insert
                        NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_taluk");
                        cmd.Parameters.AddWithValue("taluk_id", strId);
                        cmd.Parameters.AddWithValue("taluk_code", objTlk.sTalukCode);
                        cmd.Parameters.AddWithValue("taluk_name", objTlk.sTalukName);
                        cmd.Parameters.AddWithValue("dis_code", sDistcode);

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
                        NpgsqlCommand.Parameters.AddWithValue("DistrictName1", Convert.ToInt32(objTlk.sDistrictName));
                        strId = "SELECT * from \"TBLDIST\" where \"DT_CODE\" = :DistrictName1";
                        dt = Objcon.FetchDataTable(strId, NpgsqlCommand);
                        string sDistCode = Convert.ToString(dt.Rows[0]["DT_CODE"]);

                        NpgsqlCommand.Parameters.AddWithValue("TalukCode", Convert.ToInt32(objTlk.sTalukCode.ToUpper()));
                        NpgsqlCommand.Parameters.AddWithValue("DistCode", Convert.ToInt32(sDistCode));
                        strId = "SELECT * FROM \"TBLTALQ\" WHERE \"TQ_CODE\" = :TalukCode and \"TQ_DT_ID\" = :DistCode";
                        dt = Objcon.FetchDataTable(strId, NpgsqlCommand);

                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["TQ_NAME"].ToString() == objTlk.sTalukName.Trim().Replace(" ", ""))
                            {
                                Arr[0] = "Taluk Name Exist ";
                                Arr[1] = "1";

                            }
                            else
                            {
                                //update
                                NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_taluk");
                                cmd.Parameters.AddWithValue("taluk_id", objTlk.sTalukId);
                                cmd.Parameters.AddWithValue("taluk_code", objTlk.sTalukCode);
                                cmd.Parameters.AddWithValue("taluk_name", objTlk.sTalukName);
                                cmd.Parameters.AddWithValue("dis_code", sDistcode);
                                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);


                                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                                Arr[0] = "msg";
                                Arr[1] = "op_id";

                                Arr = Objcon.Execute(cmd, Arr, 2);

                            }
                        }

                        else
                        {
                            //insert
                            NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_taluk");
                            cmd.Parameters.AddWithValue("taluk_id", strId);
                            cmd.Parameters.AddWithValue("taluk_code", objTlk.sTalukCode);
                            cmd.Parameters.AddWithValue("taluk_name", objTlk.sTalukName);
                            cmd.Parameters.AddWithValue("dis_code", sDistcode);
                            cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                            cmd.Parameters.Add("op_id", NpgsqlDbType.Text);


                            cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                            cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                            Arr[0] = "Created new taluk code and name";
                            Arr[1] = "op_id";

                            Arr = Objcon.Execute(cmd, Arr, 2);


                        }
                    }
                }


                else
                {
                    //update
                    NpgsqlCommand cmd = new NpgsqlCommand("proc_save_update_taluk");
                    cmd.Parameters.AddWithValue("taluk_id", objTlk.sTalukId);
                    cmd.Parameters.AddWithValue("taluk_code", objTlk.sTalukCode);
                    cmd.Parameters.AddWithValue("taluk_name", objTlk.sTalukName);
                    cmd.Parameters.AddWithValue("dis_code", sDistcode);

                    cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd.Parameters.Add("op_id", NpgsqlDbType.Text);


                    cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                    Arr[0] = "msg";
                    Arr[1] = "op_id";
                    ;
                    Arr = Objcon.Execute(cmd, Arr, 2);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Arr;
        }
        //public OleDbDataReader Fetchdata()
        //{
        //    OleDbDataReader dr = null;
        //    try
        //    {
        //        string selQry = "select DT_NAME from TBLDIST";
        //        dr = objcon.Fetch(selQry);
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "Fetchdata");
        //    }
        //    return dr;
        //}
        public NpgsqlDataReader Fetchdata()
        {
            NpgsqlDataReader dr = null;
            string sQry = string.Empty;
            try
            {
                sQry = "select \"DT_NAME\" from \"TBLDIST\"";
                dr = Objcon.Fetch(sQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return dr;
        }

        //public object GetTlkDetails(clsTaluk objTaluk)
        //{
        //    string sDistId = string.Empty;
        //    DataTable dtDetails = new DataTable();
        //    try
        //    {

        //        String strQry = "SELECT * FROM TBLTALQ WHERE TQ_SLNO='"+objTaluk.sTalukId+"'";
        //        dtDetails = objcon.getDataTable(strQry);

        //        if (dtDetails.Rows.Count > 0)
        //        {
        //            sDistId = Convert.ToString(dtDetails.Rows[0]["TQ_DT_ID"].ToString());
        //            objTaluk.sTalukCode = Convert.ToString(dtDetails.Rows[0]["TQ_CODE"].ToString());
        //            objTaluk.sTalukName = Convert.ToString(dtDetails.Rows[0]["TQ_NAME"].ToString());
        //            objTaluk.sDistrictName = Convert.ToString(dtDetails.Rows[0]["TQ_DT_ID"].ToString());
        //            //strQry = "SELECT * FROM TBLDIST WHERE DT_CODE='" + sDistId + "'";
        //            //dtDetails = objcon.getDataTable(strQry);

        //            //objTaluk.sDistrictName = dtDetails.Rows[0]["DT_NAME"].ToString();
        //        }
        //        return objTaluk;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "GetTalkDetails");
        //        return objTaluk;
        //    }
        //}

        public object GetTlkDetails(clsTaluk objTaluk)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            string sDistId = string.Empty;
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("TalukId",Convert.ToInt32( objTaluk.sTalukId));
                strQry = "SELECT * FROM \"TBLTALQ\" WHERE \"TQ_SLNO\"= :TalukId";
                dtDetails = Objcon.FetchDataTable(strQry, NpgsqlCommand);

                if (dtDetails.Rows.Count > 0)
                {
                    sDistId = Convert.ToString(dtDetails.Rows[0]["TQ_DT_ID"]);
                    objTaluk.sTalukCode = Convert.ToString(dtDetails.Rows[0]["TQ_CODE"]);
                    objTaluk.sTalukName = Convert.ToString(dtDetails.Rows[0]["TQ_NAME"]);
                    objTaluk.sDistrictName = Convert.ToString(dtDetails.Rows[0]["TQ_DT_ID"]);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return objTaluk;
        }

        //public DataTable LoadAllTalkDetails()
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        string strQry = string.Empty;
        //        strQry = "SELECT TQ_SLNO,TQ_CODE,TQ_NAME,DT_NAME FROM TBLTALQ,TBLDIST WHERE DT_CODE=TQ_DT_ID ORDER BY TQ_SLNO";
        //        dt = objcon.getDataTable(strQry);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "LoadAllTalkDetails");
        //        return dt;
        //    }

        //}

        public DataTable LoadAllTalkDetails()
        {
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                strQry = string.Empty;
                strQry = "SELECT \"TQ_SLNO\",CAST(\"TQ_CODE\" AS TEXT),\"TQ_NAME\",\"DT_NAME\" FROM \"TBLTALQ\",\"TBLDIST\" WHERE \"DT_CODE\"=\"TQ_DT_ID\" ORDER BY \"TQ_SLNO\"";
                dt = Objcon.FetchDataTable(strQry);

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return dt;

        }
        //public string GenerateTalukCode(clsTaluk objtaluk)
        //{
        //    try
        //    {
        //        string sMaxNo = string.Empty;
        //        string sFinancialYear = string.Empty;
        //        string sCircleCodeNo = objcon.get_value(" SELECT NVL(MAX(TQ_CODE),0)+1 FROM TBLTALQ  where TQ_DT_ID='" + objtaluk.sDistrictName + "'");
        //        if (sCircleCodeNo.Length > 0)
        //        {
        //            sTalukCode = sCircleCodeNo.ToString();
        //        }
        //        return sCircleCodeNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsException.LogError(ex.StackTrace, ex.Message, sformcode, "GenerateTalukCode");
        //        return "";
        //    }
        //}

        public string GenerateTalukCode(clsTaluk objtaluk)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sDisCodeNo = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("DistName", objtaluk.sDistrictName);
                sDisCodeNo = Objcon.get_value(" SELECT SUBSTR(CAST(MAX(\"TQ_CODE\")AS TEXT),2,2) FROM \"TBLTALQ\"  where CAST(\"TQ_DT_ID\" AS TEXT) like :DistName||'%'", NpgsqlCommand);
                if (isNumber(sDisCodeNo))
                {
                    NpgsqlCommand.Parameters.AddWithValue("DistName1", objtaluk.sDistrictName);
                    sDisCodeNo = Objcon.get_value(" SELECT COALESCE(CAST(MAX(\"TQ_CODE\")AS INT) ,0)+1 FROM \"TBLTALQ\"  where CAST(\"TQ_DT_ID\" AS TEXT) like :DistName1||'%'", NpgsqlCommand);
                    if (Convert.ToInt16(sDisCodeNo) <= 1)
                    {
                        sDisCodeNo = objtaluk.sDistrictName + "1";
                        return sDisCodeNo;
                    }
                    else
                    {
                        NpgsqlCommand.Parameters.AddWithValue("DistName12", objtaluk.sDistrictName);
                        sDisCodeNo = Objcon.get_value(" SELECT SUBSTR(CAST(MAX(\"TQ_CODE\")AS TEXT),2,2) FROM \"TBLTALQ\"  where CAST(\"TQ_DT_ID\" AS TEXT) like :DistName12||'%'", NpgsqlCommand);
                    }

                    if (Convert.ToInt16(sDisCodeNo) < 9)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("DistNm12", objtaluk.sDistrictName);
                        sDisCodeNo = Objcon.get_value(" SELECT COALESCE(CAST(MAX(\"TQ_CODE\")AS INT) ,0)+1 FROM \"TBLTALQ\"  where CAST(\"TQ_DT_ID\" AS TEXT) like :DistNm12||'%'", NpgsqlCommand);

                    }
                    else if (isNumber(sDisCodeNo) && Convert.ToInt16(sDisCodeNo) == 9)
                    {
                        char AsciiChar = Convert.ToChar(65);
                        sDisCodeNo = objtaluk.sDistrictName + AsciiChar;
                    }
                    else
                    {
                        int AsciiInt = Convert.ToInt32(sDisCodeNo);
                        AsciiInt = AsciiInt + 1;
                        char AsciiChar = Convert.ToChar(AsciiInt);
                        sDisCodeNo = objtaluk.sDistrictName + AsciiChar;
                    }
                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("distNm", objtaluk.sDistrictName);
                    sDisCodeNo = Objcon.get_value(" SELECT SUBSTR(CAST(MAX(\"TQ_CODE\")AS TEXT),2,2) FROM \"TBLTALQ\"  where CAST(\"TQ_DT_ID\" AS TEXT) like :distNm||'%'", NpgsqlCommand);
                    char AsciiChar = Convert.ToChar(sDisCodeNo);
                    int AsciiInt = (int)(AsciiChar);
                    AsciiInt = AsciiInt + 1;
                    AsciiChar = Convert.ToChar(AsciiInt);
                    sDisCodeNo = objtaluk.sDistrictName + AsciiChar;

                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return sDisCodeNo;
        }

        static bool isNumber(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (char.IsDigit(s[i]) == false)
                    return false;

            return true;
        }
    }
}
