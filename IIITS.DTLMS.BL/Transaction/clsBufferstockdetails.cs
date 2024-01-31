using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;
using System.Configuration;
using Npgsql;

namespace IIITS.DTLMS.BL
{
    public class clsBufferstockdetails
    {

        string strFormCode = "clsBufferstockdetails";
        public string sExcelId { get; set; }
        public string sFilepath { get; set; }
        public string sofficecode { get; set;}
        public string sfilename { get; set;}
        public string sUserid { get; set; }
        public string sfoldername { get; set; }
        public DataTable dtbufferstockDetails { get; set; }
        public string sFromDate { get; set; }
        public string sTodate { get; set; }




        public static PGSqlConnection objcon = new PGSqlConnection(Constants.Password);
        NpgsqlCommand NpgsqlCommand;
        string Result = string.Empty;
        public bool SaveBufferStockDetails(clsBufferstockdetails objbufferstock)
        {
            try
            {
                string sQry = string.Empty;
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string sMaxNo = Convert.ToString(ObjCon.Get_max_no("SI_NO", "TBLBUFFERSTOCKDETAILS"));

                sQry = "INSERT INTO \"TBLBUFFERSTOCKDETAILS\" (\"SI_NO\",\"FILE_NAME\",\"FILE_PATH\",\"CR_BY\",\"CR_ON\",\"UPDATED_BY\",\"UPDATED_ON\",\"LOCATION\") VALUES ('" + sMaxNo + "','" + objbufferstock.sfilename + "',";
                sQry += " '" + objbufferstock.sFilepath + "','" + objbufferstock.sUserid + "',now(),'" + objbufferstock.sUserid + "',now(),'"+ objbufferstock.sofficecode + "')";
                ObjCon.ExecuteQry(sQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public void GetBufferstockDetails(clsBufferstockdetails objbufferstock)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string sQry = string.Empty;
                if (objbufferstock.sofficecode != null && objbufferstock.sofficecode != "--Select--")
                {

                    //sQry = "SELECT \"SI_NO\", \"FILE_NAME\", \"CR_ON\",(select \"US_ADDRESS\" FROM \"TBLUSER\" WHERE \"US_OFFICE_CODE\"= '" + objbufferstock.sofficecode + "')  \"LOCATION\"  FROM \"TBLBUFFERSTOCKDETAILS\" where \"LOCATION\"='" + objbufferstock.sofficecode + "' AND \"CR_BY\"='" + objbufferstock.sUserid + "' ";
                    sQry = "SELECT \"SI_NO\", \"FILE_NAME\",  \"CR_ON\",\"DIV_NAME\" FROM \"TBLBUFFERSTOCKDETAILS\"  inner join \"TBLDIVISION\" on cast(\"DIV_CODE\" as text) = \"LOCATION\"  where \"LOCATION\" Like '" + objbufferstock.sofficecode + "%' ";

                    if (objbufferstock.sTodate == null && (objbufferstock.sFromDate != null))
                    {
                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')>= '" + objbufferstock.sFromDate + "' and TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<=TO_CHAR(CURRENT_DATE,'DD/MM/YYYY')";


                        sQry += " CR_ON BETWEEN to_date('" + objbufferstock.sFromDate + "','DD/MM/YYYY')";
                    }
                    if (objbufferstock.sFromDate == null && (objbufferstock.sTodate != null))
                    {

                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<='" + objbufferstock.sTodate + "' ";
                    }

                    if (objbufferstock.sFromDate != null && objbufferstock.sTodate != null)
                    {
                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')>= '" + objbufferstock.sFromDate + "' AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<='" + objbufferstock.sTodate + "'  ";
                    }

                    sQry += "ORDER BY \"SI_NO\" DESC ";
                }
                else
                {
                    sQry = "SELECT \"SI_NO\", \"FILE_NAME\",  \"CR_ON\",\"DIV_NAME\" FROM \"TBLBUFFERSTOCKDETAILS\"  inner join \"TBLDIVISION\" on cast(\"DIV_CODE\" as text) = \"LOCATION\"  ";

                    if (objbufferstock.sTodate == null && (objbufferstock.sFromDate != null))
                    {
                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')>= '" + objbufferstock.sFromDate + "' and TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<=TO_CHAR(CURRENT_DATE,'DD/MM/YYYY')";


                        sQry += " CR_ON BETWEEN to_date('" + objbufferstock.sFromDate + "','DD/MM/YYYY')";
                    }
                    if (objbufferstock.sFromDate == null && (objbufferstock.sTodate != null))
                    {

                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<='" + objbufferstock.sTodate + "' ";
                    }

                    if (objbufferstock.sFromDate != null && objbufferstock.sTodate != null)
                    {
                        sQry += "AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')>= '" + objbufferstock.sFromDate + "' AND TO_CHAR(\"CR_ON\",'DD/MM/YYYY')<='" + objbufferstock.sTodate + "'  ";
                    }



                    sQry += "ORDER BY \"SI_NO\" DESC Limit 500";
                }


               objbufferstock.dtbufferstockDetails = objcon.FetchDataTable(sQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

    }
}

