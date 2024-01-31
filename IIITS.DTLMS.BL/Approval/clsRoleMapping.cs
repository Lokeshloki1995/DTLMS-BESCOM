using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IIITS.PGSQL.DAL;
using System.Data.OleDb;
using Npgsql;

namespace IIITS.DTLMS.BL
{
public class clsRoleMapping
{

       string strFormCode = "clsRoleMapping";
        PGSqlConnection objCon = new PGSqlConnection(Constants.Password);
        public string sModuleId { get; set; }
        public string sMappingId { get; set; }
        public string sRoleId { get; set; }
        public string sBusinessobjId { get; set; }
        public string sAccessId { get; set; }
        public string sCrby { get; set; }

        // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY
        NpgsqlCommand NpgsqlCommand;
        public DataTable LoadAllModuleDetails(clsRoleMapping objModuleDetails)
       {
           DataTable dt = new DataTable();
           string strQry = string.Empty;
           try
           {
               strQry = "SELECT \"BO_ID\",\"BO_NAME\",'' AS UR_ID  FROM \"TBLMODULES\",\"TBLBUSINESSOBJECT\" WHERE \"MO_ID\"=\"BO_MO_ID\" ";
               strQry += " and \"MO_ID\"=:sModuleId";
               NpgsqlCommand = new NpgsqlCommand();
               NpgsqlCommand.Parameters.AddWithValue("sModuleId", Convert.ToInt32(objModuleDetails.sModuleId));
               dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
               return dt;

           }
           catch (Exception ex)
           {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
           }

       }



        public DataTable LoadAllRoleDetails(clsRoleMapping objModuleDetails)
        {
            DataTable dt = new DataTable();
            string strQry = string.Empty;
            try
            {
                strQry = "Select  DISTINCT(\"BO_NAME\"), \"BO_ID\",'' AS UR_ID, \"MO_ID\",\"UR_ACCESSTYPE\" FROM \"TBLBUSINESSOBJECT\", \"TBLMODULES\",";
                strQry += " \"TBLUSERROLEMAPPING\" WHERE \"UR_ROLEID\"=:sRoleId AND \"UR_BOID\"=:sBusinessobjId ";
                strQry += " AND  \"BO_ID\"=\"UR_BOID\" AND \"MO_ID\"=\"BO_MO_ID\"";
                NpgsqlCommand = new NpgsqlCommand();
                NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objModuleDetails.sRoleId));
                NpgsqlCommand.Parameters.AddWithValue("sBusinessobjId", Convert.ToInt32(objModuleDetails.sBusinessobjId));
                dt = objCon.FetchDataTable(strQry, NpgsqlCommand);
                return dt;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dt;
            }

        }
        public string[] SaveAccessRights(clsRoleMapping objAccessDetails,string[] sAccessId)
        {
          
            string strQry = string.Empty;
            string[] sAccessTypes;
            string[] Arr = new string[2];

            try
            {
                //objCon.BeginTransaction();

                bool bResult = DeleteAccessRights(objAccessDetails);
                if (bResult == true)
                {

                    string[] strDetailVal = sAccessId.ToArray();
                    for (int i = 0; i < strDetailVal.Length; i++)
                    {
                        if (strDetailVal[i] != null)
                        {
                            sAccessTypes = strDetailVal[i].Split(';');

                            for (int j = 1; j < sAccessTypes.Length; j++)
                            {
                                if (sAccessTypes[j] != "")
                                {
                                    objAccessDetails.sMappingId = objCon.Get_max_no("UR_ID", "TBLUSERROLEMAPPING").ToString();
                                    strQry = "INSERT INTO \"TBLUSERROLEMAPPING\"(\"UR_ID\",\"UR_ROLEID\",\"UR_BOID\",\"UR_ACCESSTYPE\",\"UR_CRON\",\"UR_CRBY\",\"UR_MOID\")";
                                    strQry += "VALUES('"+ Convert.ToInt32(objAccessDetails.sMappingId) + "','"+ Convert.ToInt32(objAccessDetails.sRoleId) + "','"+ Convert.ToInt32(strDetailVal[i].Split(';').GetValue(0).ToString()) + "',";
                                    strQry += " '"+ Convert.ToDouble(sAccessTypes[j]) + "',NOW(),'"+ Convert.ToInt32(objAccessDetails.sCrby) + "','"+ Convert.ToInt32(objAccessDetails.sModuleId) + "')";
                                  //  NpgsqlCommand = new NpgsqlCommand();
                                   // NpgsqlCommand.Parameters.AddWithValue("sMappingId", Convert.ToInt32(objAccessDetails.sMappingId));
                                  //  NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objAccessDetails.sRoleId));
                                   // NpgsqlCommand.Parameters.AddWithValue("strDetailVal", Convert.ToInt32(strDetailVal[i].Split(';').GetValue(0).ToString()));
                                   // NpgsqlCommand.Parameters.AddWithValue("sAccessTypes",Convert.ToDouble(sAccessTypes[j]));
                                   // NpgsqlCommand.Parameters.AddWithValue("sCrBy", Convert.ToInt32(objAccessDetails.sCrby));
                                   // NpgsqlCommand.Parameters.AddWithValue("sModuleId", Convert.ToInt32(objAccessDetails.sModuleId));
                                    objCon.ExecuteQry(strQry);
                                }
                            }
                        }
                    }


                    objCon.CommitTransaction();
                    Arr[0] = "Saved Successfully ";
                    Arr[1] = "0";
                    
                }
                return Arr;
            }

            catch (Exception ex)
            {
                //objCon.RollBackTrans();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }


        }

        public bool  DeleteAccessRights(clsRoleMapping objRoleMap)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "DELETE FROM \"TBLUSERROLEMAPPING\" WHERE \"UR_ROLEID\"='"+ Convert.ToInt32(objRoleMap.sRoleId) + "' ";
                strQry += " AND \"UR_MOID\"='"+ Convert.ToInt32(objRoleMap.sModuleId) + "'";
                //NpgsqlCommand = new NpgsqlCommand();
              //  NpgsqlCommand.Parameters.AddWithValue("sRoleId", Convert.ToInt32(objRoleMap.sRoleId));
               // NpgsqlCommand.Parameters.AddWithValue("sModuleId", Convert.ToInt32(objRoleMap.sModuleId));
                objCon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

    }
}
