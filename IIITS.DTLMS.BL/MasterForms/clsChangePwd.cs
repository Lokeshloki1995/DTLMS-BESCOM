using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using Npgsql;
using System.Reflection;


namespace IIITS.DTLMS.BL
{
    public class clsChangePwd
    {
        PGSqlConnection Objcon = new PGSqlConnection(Constants.Password);

        string strFormCode = "clsChangePwd";
        public string strOldPwd { get; set; }
        public string strNewPwd { get; set; }
        public string strConfirmPwd { get; set; }
        public string struserId { get; set; }

        NpgsqlCommand NpgsqlCommand;
        public String[] ChangePwd(clsChangePwd objChangepwd)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string sQry = string.Empty;
            string[] Arr = new string[2];
            string sOldPwd = string.Empty;
            try
            {

                NpgsqlCommand.Parameters.AddWithValue("uid", Convert.ToInt32(objChangepwd.struserId));
                sQry = "SELECT \"US_PWD\" FROM \"TBLUSER\" WHERE \"US_ID\" =:uid ";
                sOldPwd = Objcon.get_value(sQry, NpgsqlCommand);
                //sOldPwd == Genaral.Encrypt(objChangepwd.strOldPwd)
                if (Genaral.CompareLogin(Convert.ToString(sOldPwd), objChangepwd.strOldPwd) == true)
                {
                    //if (sOldPwd == Genaral.Encrypt(objChangepwd.strNewPwd))
                    //{
                    //    Arr[0] = "Old and New password Should not be same";
                    //    Arr[1] = "0";
                    //}
                    // else
                    // {
                    NpgsqlCommand.Parameters.AddWithValue("NewPwd", Genaral.EncryptPassword(objChangepwd.strNewPwd));
                    NpgsqlCommand.Parameters.AddWithValue("userId", Convert.ToInt32(objChangepwd.struserId));
                    NpgsqlCommand.Parameters.AddWithValue("NewPassword",objChangepwd.strNewPwd);
                    //sQry = "UPDATE \"TBLUSER\" SET \"US_PWD\" =:NewPwd, \"US_CHPWD_ON\"=NOW(),\"US_PWD_REF\"=:NewPassword WHERE \"US_ID\" =:userId";
                    sQry = "UPDATE \"TBLUSER\" SET \"US_PWD\" =:NewPwd, \"US_CHPWD_ON\"=NOW() WHERE \"US_ID\" =:userId";
                    Objcon.ExecuteQry(sQry, NpgsqlCommand);

                    NpgsqlCommand.Parameters.AddWithValue("userId1", Convert.ToInt32(objChangepwd.struserId));
                    NpgsqlCommand.Parameters.AddWithValue("OldPwd", sOldPwd);
                    NpgsqlCommand.Parameters.AddWithValue("userId2", Convert.ToInt32(objChangepwd.struserId));

                    sQry = "INSERT INTO \"TBLUSER_OLD_PASSWORD\" (\"UOP_ID\",\"UOP_US_ID\",\"UOP_PWD\",\"UOP_CR_ON\",\"UOP_CR_BY\") VALUES ";
                    sQry += " ((SELECT COALESCE(MAX(\"UOP_ID\"),0)+1 FROM \"TBLUSER_OLD_PASSWORD\"),:userId1,";
                    sQry += " :OldPwd,NOW(),:userId2)";
                    Objcon.ExecuteQry(sQry, NpgsqlCommand);

                    Arr[0] = "Password successfully changed.";
                    Arr[1] = "1";
                    // }
                }
                else
                {
                    Arr[0] = "Invalid Old Password";
                    Arr[1] = "0";
                }



            }
            catch (Exception ex)
            {
                Arr[0] = "Invalid User Details";
                Arr[1] = "0";
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return Arr;
        }


    }
}
