﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIITS.PGSQL.DAL;
using System.Data;

namespace IIITS.DTLMS.BL
{
    public static class clsStoreOffice
    {
        public static string strFormCode = "clsStoreOffice";

        public static string GetStoreCode(string sStoreID, string sColumn)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            try
            {
                string sResult = string.Empty;
                string sQry = string.Empty;

                sQry = "SELECT string_agg(CAST(\"SM_ID\" AS TEXT), ',') AS \"OFFCODE\" FROM \"TBLSTOREMAST\", \"TBLSTOREOFFCODE\" ";
                sQry += " WHERE \"SM_ID\" = \"STO_SM_ID\" AND cast(\"SM_ID\" as text) = '" + sStoreID + "'";
                string sStoreOffCode = objcon.get_value(sQry);

                if (sStoreOffCode.Length > 1)
                {
                    string[] sArr = new string[4];
                    sArr = sStoreOffCode.Split(',');

                    sResult = "( ";
                    for (int i = 0; i < sArr.Length; i++)
                    {
                        sResult += " CAST(\"" + sColumn + "\" AS TEXT) = '" + sArr[i] + "' ";
                        if (i < sArr.Length - 1)
                        {
                            sResult += " OR ";
                        }
                    }
                    sResult += ") ";
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
        public static string GetOfficeCode(string sStoreID, string sColumn)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            try
            {
                string sResult = string.Empty;
                string sQry = string.Empty;

                string sStoreOffCode = string.Empty;

                if (sStoreID == "2" && !sColumn.Contains("REF_OFFCODE"))
                {

                    sQry = "SELECT \"SM_CODE\" FROM \"TBLSTOREMAST\" WHERE cast(\"SM_ID\" as text) = '" + sStoreID + "'";
                    sStoreOffCode = objcon.get_value(sQry);
                }
                else
                {
                    if (sStoreID != "2")
                    {
                        sQry = "SELECT string_agg(CAST(\"STO_OFF_CODE\" AS TEXT), ',') AS \"OFFCODE\" FROM \"TBLSTOREMAST\", \"TBLSTOREOFFCODE\" ";
                        sQry += " WHERE \"SM_ID\" = \"STO_SM_ID\" AND cast(\"SM_ID\" as text) = '" + sStoreID + "'";
                    }
                    else
                    {
                        sQry = " SELECT string_agg(CAST(\"SM_OFF_CODE\" AS TEXT), ',') AS \"OFFCODE\" FROM \"TBLSTOREMAST\" WHERE cast(\"SM_ID\" as text) = '" + sStoreID + "'";
                    }
                    sStoreOffCode = objcon.get_value(sQry);
                }

                if (sStoreOffCode.Length > 1)
                {
                    string[] sArr = new string[4];
                    sArr = sStoreOffCode.Split(',');

                    sResult = "( ";
                    for (int i = 0; i < sArr.Length; i++)
                    {
                        sResult += " CAST(\""+sColumn+"\" AS TEXT) LIKE '" + sArr[i] + "%' ";
                        if(i < sArr.Length-1)
                        {
                            sResult += " OR ";
                        }
                    }
                    sResult += ") ";
                }

                return sResult;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }

        public static string GetOfficeCodenot(string sStoreID, string sColumn)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            try
            {
                string sResult = string.Empty;
                string sQry = string.Empty;
                sQry = "SELECT string_agg(CAST(\"STO_OFF_CODE\" AS TEXT), ',') AS \"OFFCODE\" FROM \"TBLSTOREMAST\", \"TBLSTOREOFFCODE\" ";
                sQry += " WHERE \"SM_ID\" = \"STO_SM_ID\" AND \"SM_ID\" = '" + sStoreID + "'";
                string sStoreOffCode = objcon.get_value(sQry);

                if (sStoreOffCode.Length > 1)
                {
                    string[] sArr = new string[4];
                    sArr = sStoreOffCode.Split(',');

                    sResult = "( ";
                    for (int i = 0; i < sArr.Length; i++)
                    {
                        sResult += " CAST(\"" + sColumn + "\" AS TEXT) <> '" + sArr[i] + "' ";
                        if (i < sArr.Length - 1)
                        {
                            sResult += " AND ";
                        }
                    }
                    sResult += ") ";
                }

                return sResult;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        } 

        public static string GetStoreID(string sOfficeCode)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            string sStoreId = string.Empty;
            try
            {
                if(sOfficeCode.Length > 3)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                }
                string sQry = string.Empty;
                if (sOfficeCode == "")
                {
                    sQry = "";
                }
                else
                {
                    sQry = "SELECT \"STO_SM_ID\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_OFF_CODE\" = '" + sOfficeCode + "' ";
                }
                sStoreId = objcon.get_value(sQry);
                return sStoreId;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sStoreId;
            }
        }


        public static string GetStoreIDs(string sOfficeCode)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            string sStoreId = string.Empty;
            try
            {
                if (sOfficeCode.Length > 3)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                }
                string sQry = string.Empty;
                if (sOfficeCode == "")
                {
                    sQry = "";
                }
                else
                {
                    sQry = "SELECT \"STO_SM_ID\" FROM \"TBLSTOREOFFCODE\" WHERE cast(\"STO_OFF_CODE\" as text) like '" + sOfficeCode + "%' limit 1";
                }
                sStoreId = objcon.get_value(sQry);
                return sStoreId;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sStoreId;
            }
        }

        public static string Getofficecode(string sOfficeCode)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);  
            string sStoreId = string.Empty;
            try
            {
                string sQry = string.Empty;
                sQry = "SELECT string_agg(CAST(\"STO_OFF_CODE\" AS TEXT), ',') AS \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" = '" + sOfficeCode + "' ";
                sStoreId = objcon.get_value(sQry);
                return sStoreId;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sStoreId;
            }
        }
        


        public static string GetCurrentOfficeCode(string sWoID, string sType)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            try
            {
                string sQry = string.Empty;
                if(sType == "1")
                {
                    sQry = "SELECT \"WO_OFFICE_CODE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE cast(\"WO_ID\" as text) = '" + sWoID + "'";
                }
                else
                {
                    sQry = "SELECT \"WOA_OFFICE_CODE\" FROM \"TBLWO_OBJECT_AUTO\" WHERE cast(\"WOA_ID\" as text) = '" + sWoID + "'";
                }
                string sOfficeCode = objcon.get_value(sQry);
                return sOfficeCode;
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }

        public static string GetRICurrentOfficeCode(string sWoID, string sType)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            try
            {
                string sQry = string.Empty;
                sQry = "SELECT \"WO_OFFICE_CODE\" FROM \"TBLWORKFLOWOBJECTS\" WHERE cast(\"WO_ID\" as text)= '" + sWoID + "'";                
                string sOfficeCode = objcon.get_value(sQry);
                if (sOfficeCode.Length > 3)
                {
                    sOfficeCode = sOfficeCode.Substring(0, 3);
                }
                return sOfficeCode;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "0";
            }
        }
      
        public static string GetZone_Circle_Div_Offcode(string sOfficeCode, string sRoleID)
        {
            PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
            string offCode = string.Empty;
            try
            {
                if (sRoleID.Equals("2") || sRoleID.Equals("5"))
                {
                    string sQry = string.Empty;
                    if(sOfficeCode=="2")
                    {
                        sQry = "SELECT \"SM_CODE\" FROM \"TBLSTOREMAST\" WHERE \"SM_ID\" = '" + sOfficeCode + "' limit 1 ";
                    }
                    else
                    {
                        sQry = "SELECT \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" = '" + sOfficeCode + "' limit 1 ";

                    }
                    offCode = objcon.get_value(sQry);
                    return offCode;
                }
                else
                {

                    if (sRoleID.Equals(Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SupAdminRole"])))
                    {
                        string sQry = string.Empty;
                        sQry = "SELECT \"STO_OFF_CODE\" FROM \"TBLSTOREOFFCODE\" WHERE \"STO_SM_ID\" = '" + sOfficeCode + "' limit 1 ";
                        offCode = objcon.get_value(sQry);
                        return offCode;
                    }
                    else
                    {
                        return sOfficeCode;
                    }
                       // return sOfficeCode;
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return offCode;
            }
        }
    }
}
