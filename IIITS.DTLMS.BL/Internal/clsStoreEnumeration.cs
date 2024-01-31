﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using System.Data.OleDb;
using IIITS.PGSQL.DAL;

namespace IIITS.DTLMS.BL
{
    public class clsStoreEnumeration
    {
        string strFormCode = "clsStoreEnumeration";
      

        public string sEnumDetailsId { get; set; }
        public string sEnumDTCId { get; set; }

        public string sCrBy { get; set; }

        public string sDivCode { get; set; }
        public string sLocName { get; set; }
        public string sLocAdd { get; set; }
        public string sLoctype { get; set; }
        public string sWeldDate { get; set; }
        public string sTcCode { get; set; }
        public string sTCSlno { get; set; }
        public string sTCCapacity { get; set; }
        public string sTCManfDate { get; set; }
        public string sTCMake { get; set; }
        public string sTCType { get; set; }
        public string sEnumDate { get; set; }
        public string sOperator1 { get; set; }
        public string sOperator2 { get; set; }
        public string sAddress { get; set; }
        public string sValue { get; set; }
        public string sSelectedValue { get; set; }
        public string sEnumType { get; set; }
        public string sMakeName { get; set; }

        public string sTankCapacity { get; set; }
        public string sTCWeight { get; set; }
        public string sRating { get; set; }
        public string sStarRate { get; set; }

        public bool bTCSlNoNotExists { get; set; }
        public string sNamePlatePhotoPath { get; set; }
        public string sSSPlatePhotoPath { get; set; }

        public string sStatus { get; set; }

        public string[] SaveStoreEnumerationDetails(clsStoreEnumeration objEnumeration)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
           NpgsqlDataReader dr = null;
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

            try
            {
               
                //if (objEnumeration.sTCManfDate != "")
                //{
                //    objEnumeration.sTCManfDate = "01/" + objEnumeration.sTCManfDate;
                //}

                /// coded by pradeep - start
                //strQry = "SELECT \"VM_NAME\" FROM \"TBLINTERNALUSERS\",\"TBLVENDORMASTER\",\"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE";
                //strQry += " \"IU_VENDOR_ID\"=\"VM_ID\" AND \"VM_ID\"=\"TCPM_VENDOR_ID\" AND \"TCPM_ID\"=\"TCP_TCPM_ID\" AND \"IU_ID\"= '" + objEnumeration.sOperator1 + "' AND \"TCP_STATUS_FLAG\"='0' AND \"TCP_TC_CODE\" = '" + objEnumeration.sTcCode + "'";
                //dr = ObjCon.Fetch(strQry);
                //if (!dr.Read())
                //{
                //    dr.Close();
                //    strQry = "SELECT \"VM_NAME\" FROM \"TBLINTERNALUSERS\",\"TBLVENDORMASTER\",\"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE";
                //    strQry += " \"IU_VENDOR_ID\"=\"VM_ID\" AND \"VM_ID\"=\"TCPM_VENDOR_ID\" AND \"TCPM_ID\"=\"TCP_TCPM_ID\" AND \"TCP_TC_CODE\" = '" + objEnumeration.sTcCode + "'";
                //    string name = ObjCon.get_value(strQry);
                //    if (!name.Trim().Equals(""))
                //    {
                //        Arr[0] = "Plate Number " + objEnumeration.sTcCode + "  Already allocated to vendor " + name;
                //        Arr[1] = "2";
                //        return Arr;
                //    }
                //}
                //dr.Close();



                strQry = "SELECT \"VM_ID\" || '~' ||\"VM_NAME\" FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\",\"TBLVENDORMASTER\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND ";
                strQry += " \"VM_ID\" =\"TCPM_VENDOR_ID\" AND \"TCP_TC_CODE\"='" + objEnumeration.sTcCode + "'";
                string sVmDetails = ObjCon.get_value(strQry);

                strQry = "SELECT \"IU_VENDOR_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_ID\"='" + objEnumeration.sOperator1 + "'";
                string sLogUs_VM_Id = ObjCon.get_value(strQry);

                if (sLogUs_VM_Id == sVmDetails.Split('~').GetValue(0).ToString())
                {
                    strQry = "SELECT * FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND \"TCP_STATUS_FLAG\" IN (0,4) AND ";
                    strQry += " \"TCP_TC_CODE\" ='" + objEnumeration.sTcCode + "'";
                    string res = ObjCon.get_value(strQry);

                    if (res == null)
                    {
                        Arr[0] = "DTR Code " + objEnumeration.sTcCode + "  Not Possible to save because entered DTR Code may be Approved, deleted or Rejected ";
                        Arr[1] = "2";
                        return Arr;
                    }
                }
                else
                {
                    Arr[0] = "DTR Code " + objEnumeration.sTcCode + "  Already allocated to vendor " + sVmDetails.Split('~').GetValue(1).ToString();
                    Arr[1] = "2";
                    return Arr;
                }



                //Arr[1] = "1"; //delete
                //return Arr; //delete
                ///coded by pradeep -end

                if (objEnumeration.sEnumDetailsId == "")
                {
                   


                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objEnumeration.sTcCode + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objEnumeration.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    if (objEnumeration.sTCMake != "")
                    {
                        dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objEnumeration.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objEnumeration.sTCSlno + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                        if (dr.Read())
                        {
                            Arr[0] = "Combination of Transformer Sl No " + objEnumeration.sTCSlno + " and Make Name  " + objEnumeration.sMakeName + " Already Exist";
                            Arr[1] = "2";
                            dr.Close();
                            return Arr;
                        }
                        dr.Close();
                    }
                    //ObjCon.BeginTransaction();
                    objEnumeration.sEnumDetailsId = Convert.ToString(ObjCon.Get_max_no("ED_ID", "TBLENUMERATIONDETAILS"));

                    strQry = "INSERT INTO \"TBLENUMERATIONDETAILS\"(\"ED_ID\",\"ED_OFFICECODE\",\"ED_ENUM_TYPE\",\"ED_LOCTYPE\",\"ED_LOCNAME\",\"ED_LOCADDRESS\",\"ED_OPERATOR1\",\"ED_OPERATOR2\",\"ED_WELD_DATE\",\"ED_CRBY\",\"ED_CRON\") ";
                    strQry += " VALUES ('" + objEnumeration.sEnumDetailsId + "','" + objEnumeration.sDivCode + "','" + objEnumeration.sLoctype + "','" + objEnumeration.sLoctype + "','" + objEnumeration.sLocName + "', ";
                    strQry += " '" + objEnumeration.sLocAdd + "','" + objEnumeration.sOperator1 + "', ";
                    strQry += " '" + objEnumeration.sOperator2 + "',TO_DATE('" + objEnumeration.sWeldDate + "','dd/mm/yyyy'),'" + objEnumeration.sCrBy + "',now())";

                    ObjCon.ExecuteQry(strQry);

                    objEnumeration.sEnumDTCId = Convert.ToString(ObjCon.Get_max_no("DTE_ID", "TBLDTCENUMERATION"));


                    //If Make is NNP, Save max No of TC_SLno+1
                    //if (objEnumeration.sTCMake == "1")
                    //{
                    //    objEnumeration.sTCSlno = ObjCon.get_value("SELECT COALESCE(MAX(CAST(DTE_TC_SLNO AS NUMBER(10))),0)+1 FROM TBLDTCENUMERATION WHERE DTE_MAKE='" + objEnumeration.sTCMake + "'");
                    //}

                    if(objEnumeration.sTCManfDate == null)
                    {
                        strQry = "INSERT INTO \"TBLDTCENUMERATION\" (\"DTE_ID\",\"DTE_ED_ID\",\"DTE_TC_CODE\",\"DTE_MAKE\",\"DTE_CAPACITY\",\"DTE_TC_TYPE\",\"DTE_TC_SLNO\",\"DTE_CRBY\",";
                        strQry += " \"DTE_CRON\",\"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_RATING\",\"DTE_STAR_RATE\")";
                        strQry += " VALUES ('" + objEnumeration.sEnumDTCId + "','" + objEnumeration.sEnumDetailsId + "','" + objEnumeration.sTcCode + "', ";
                        strQry += " '" + objEnumeration.sTCMake + "'," + objEnumeration.sTCCapacity + ", ";
                        strQry += " " + objEnumeration.sTCType + ",'" + objEnumeration.sTCSlno + "',";
                        strQry += " '" + objEnumeration.sCrBy + "',now(),'" + objEnumeration.sTankCapacity + "','" + objEnumeration.sTCWeight + "',";
                        strQry += " '" + objEnumeration.sRating + "','" + objEnumeration.sStarRate + "')";
                    }
                    else
                    {
                        strQry = "INSERT INTO \"TBLDTCENUMERATION\" (\"DTE_ID\",\"DTE_ED_ID\",\"DTE_TC_CODE\",\"DTE_MAKE\",\"DTE_CAPACITY\",\"DTE_TC_TYPE\",\"DTE_TC_SLNO\",\"DTE_TC_MANFDATE\",\"DTE_CRBY\",";
                        strQry += " \"DTE_CRON\",\"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_RATING\",\"DTE_STAR_RATE\")";
                        strQry += " VALUES ('" + objEnumeration.sEnumDTCId + "','" + objEnumeration.sEnumDetailsId + "','" + objEnumeration.sTcCode + "', ";
                        strQry += " '" + objEnumeration.sTCMake + "'," + objEnumeration.sTCCapacity + ", ";
                        strQry += " " + objEnumeration.sTCType + ",'" + objEnumeration.sTCSlno + "',TO_DATE('" + objEnumeration.sTCManfDate + "','dd/MM/yyyy'),";
                        strQry += " '" + objEnumeration.sCrBy + "',now(),'" + objEnumeration.sTankCapacity + "','" + objEnumeration.sTCWeight + "',";
                        strQry += " '" + objEnumeration.sRating + "','" + objEnumeration.sStarRate + "')";
                    }

                    

                    ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - start
                    //strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE \"TCP_TC_CODE\" ='" + objEnumeration.sTcCode + "'";
                    //ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end

                    //ObjCon.CommitTransaction();


                    Arr[0] = "Store Enumeration Details Saved Successfully";
                    Arr[1] = "0";
                    ObjCon.close();
                    return Arr;
                }


                else
                {
                    //ObjCon.BeginTransaction();

                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objEnumeration.sTcCode + "' AND \"DTE_ED_ID\" <>'" + objEnumeration.sEnumDetailsId + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objEnumeration.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();


                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objEnumeration.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objEnumeration.sTCSlno + "' AND \"DTE_ED_ID\" <>'" + objEnumeration.sEnumDetailsId + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Combination of Transformer Sl No " + objEnumeration.sTCSlno + " and Make Name  " + objEnumeration.sMakeName + " Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_OFFICECODE\"='" + objEnumeration.sDivCode + "',\"ED_ENUM_TYPE\"='" + objEnumeration.sLoctype + "',";
                    strQry += " \"ED_LOCTYPE\"='" + objEnumeration.sLoctype + "',\"ED_LOCNAME\"='" + objEnumeration.sLocName + "',\"ED_LOCADDRESS\"= ";
                    strQry += " '" + objEnumeration.sLocAdd + "',\"ED_OPERATOR1\"='" + objEnumeration.sOperator1 + "',\"ED_OPERATOR2\"='" + objEnumeration.sOperator2 + "', ";
                    strQry += " \"ED_WELD_DATE\"=TO_DATE('" + objEnumeration.sWeldDate + "','dd/mm/yyyy'),\"ED_UPDATE_BY\"='" + objEnumeration.sCrBy + "', ";
                    strQry += " \"ED_UPDATE_ON\"=now() WHERE \"ED_ID\"='" + objEnumeration.sEnumDetailsId + "'";
                    ObjCon.ExecuteQry(strQry);

                    ////If Make is NNP, Saving Enumeration DTC Id
                    //if (objEnumeration.sTCMake == "1")
                    //{
                    //    objEnumeration.sTCSlno = ObjCon.get_value("SELECT DTE_ID FROM TBLDTCENUMERATION WHERE DTE_ED_ID='" + objEnumeration.sEnumDetailsId + "'");
                    //}


                    //if (objEnumeration.sTCSlno == "")
                    //{
                    //    if (objEnumeration.sTCMake == "1" && objEnumeration.sTCSlno == "")
                    //    {
                    //        objEnumeration.sTCSlno = ObjCon.get_value("SELECT DTE_ID FROM TBLDTCENUMERATION WHERE DTE_ED_ID='" + objEnumeration.sEnumDetailsId + "'");
                    //    }
                    //}

                    strQry = "UPDATE \"TBLDTCENUMERATION\" SET \"DTE_TC_CODE\"='" + objEnumeration.sTcCode + "',\"DTE_MAKE\"='" + objEnumeration.sTCMake + "',";
                    strQry += " \"DTE_CAPACITY\"='" + objEnumeration.sTCCapacity + "',\"DTE_TC_TYPE\"='" + objEnumeration.sTCType + "',\"DTE_TC_SLNO\"= ";
                    strQry += " '" + objEnumeration.sTCSlno + "',\"DTE_TC_MANFDATE\"=TO_DATE('" + objEnumeration.sTCManfDate + "','dd/MM/yyyy'), ";
                    strQry += " \"DTE_TANK_CAPACITY\"='" + objEnumeration.sTankCapacity + "',\"DTE_TC_WEIGHT\"='" + objEnumeration.sTCWeight + "', ";
                    strQry += " \"DTE_RATING\"='" + objEnumeration.sRating + "',\"DTE_STAR_RATE\"='" + objEnumeration.sStarRate + "' WHERE \"DTE_ED_ID\"='" + objEnumeration.sEnumDetailsId + "'";
                    ObjCon.ExecuteQry(strQry);

                    if (objEnumeration.sStatus == "2")
                    {
                        strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"='0' WHERE \"ED_ID\"='" + objEnumeration.sEnumDetailsId + "'";
                        ObjCon.ExecuteQry(strQry);
                    }

                    //coded by pradeep - start
                    strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE \"TCP_TC_CODE\" ='" + objEnumeration.sTcCode + "'";
                    ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end


                    //ObjCon.CommitTransaction();
                    ObjCon.close();
                    Arr[0] = "Store Enumeration Details Updated Successfully";
                    Arr[1] = "1";
                    return Arr;
                }

            }
            catch (Exception ex)
            {
                ObjCon.RollBackTrans();
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "SaveFieldEnumerationDetails");
                return Arr;
            }
        }

        public DataTable LoadStoreEnumeration(string sOperator = "")
        {
           
            DataTable dt = new DataTable();
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;


                strQry = "select  \"ED_ID\", \"DTE_ED_ID\",(select \"MD_NAME\" from \"TBLMASTERDATA\" where \"MD_ID\"= \"ED_LOCTYPE\" and \"MD_TYPE\"='TCL') \"ED_LOCTYPE\",";
                strQry += "  \"ED_LOCNAME\",TO_CHAR(\"ED_WELD_DATE\",'dd/MM/yyyy') \"ED_WELD_DATE\",(SELECT  \"TM_NAME\" FROM  \"TBLTRANSMAKES\" WHERE  \"DTE_MAKE\"= \"TM_ID\") MAKE, \"DTE_CAPACITY\" from ";
                strQry += "  \"TBLENUMERATIONDETAILS\", \"TBLDTCENUMERATION\" where  \"ED_ID\"= \"DTE_ED_ID\" and  \"ED_ENUM_TYPE\" IN ('1','3','5') and  \"ED_CANCEL_FLAG\"='0' ";
                if (sOperator != "")
                {
                    strQry += " AND  (\"ED_OPERATOR1\"='" + sOperator + "' OR \"ED_OPERATOR2\"='" + sOperator + "')";
                }
                 return ObjCon.FetchDataTable(strQry);

             //   return dt;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "LoadStoreEnumeration");
                return dt;
            }
        }
        public clsStoreEnumeration GetStoreEnumerationDetails(clsStoreEnumeration objstore)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;
                //strQry = "SELECT \"ED_OFFICECODE\",\"ED_LOCNAME\",\"ED_LOCADDRESS\",\"ED_LOCTYPE\",TO_CHAR(\"ED_WELD_DATE\",'dd/mm/yyyy') \"ED_WELD_DATE\",\"DTE_TC_CODE\",\"DTE_MAKE\",\"DTE_TC_SLNO\",";
                //strQry += "TO_CHAR(\"DTE_TC_MANFDATE\",'MM/yyyy') AS \"DTE_TC_MANFDATE\",\"DTE_CAPACITY\",\"DTE_TC_TYPE\",To_char(\"DTE_ENUM_DATE\",'dd/mm/yyyy') \"DTE_ENUM_DATE\",\"ED_OPERATOR1\",\"ED_OPERATOR2\",";
                //strQry += " \"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_RATING\",\"DTE_STAR_RATE\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\" from";
                //strQry += " \"TBLENUMERATIONDETAILS\",\"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\" where \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_ID=EP_ED_ID\"(+) and \"ED_ID\" = '" + objstore.sEnumDetailsId + "'";
                strQry= "SELECT \"ED_OFFICECODE\",\"ED_LOCNAME\",\"ED_LOCADDRESS\",\"ED_LOCTYPE\",TO_CHAR(\"ED_WELD_DATE\",'dd/mm/yyyy') \"ED_WELD_DATE\",\"DTE_TC_CODE\",\"DTE_MAKE\",\"DTE_TC_SLNO\",TO_CHAR(\"DTE_TC_MANFDATE\",'MM/yyyy') AS \"DTE_TC_MANFDATE\",\"DTE_CAPACITY\" ";
                strQry += " ,\"DTE_TC_TYPE\",To_char(\"DTE_ENUM_DATE\",'dd/mm/yyyy') \"DTE_ENUM_DATE\",\"ED_OPERATOR1\",\"ED_OPERATOR2\", \"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_RATING\",\"DTE_STAR_RATE\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\" from \"TBLENUMERATIONDETAILS\",";
                strQry += "\"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\" where  \"ED_ID\" = \"DTE_ED_ID\" and  \"ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"='" + objstore.sEnumDetailsId + "' ";
                DataTable dt = new DataTable();
                dt = ObjCon.FetchDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    objstore.sDivCode = Convert.ToString(dt.Rows[0]["ED_OFFICECODE"]);
                    objstore.sLocName = Convert.ToString(dt.Rows[0]["ED_LOCNAME"]);
                    objstore.sLocAdd = Convert.ToString(dt.Rows[0]["ED_LOCADDRESS"]);
                    objstore.sLoctype = Convert.ToString(dt.Rows[0]["ED_LOCTYPE"]);
                    objstore.sWeldDate = Convert.ToString(dt.Rows[0]["ED_WELD_DATE"]);
                    objstore.sTcCode = Convert.ToString(dt.Rows[0]["DTE_TC_CODE"]);
                    objstore.sTCMake = Convert.ToString(dt.Rows[0]["DTE_MAKE"]);
                    objstore.sTCSlno = Convert.ToString(dt.Rows[0]["DTE_TC_SLNO"]);
                    objstore.sTCManfDate = Convert.ToString(dt.Rows[0]["DTE_TC_MANFDATE"]);
                    objstore.sTCCapacity = Convert.ToString(dt.Rows[0]["DTE_CAPACITY"]);
                    objstore.sTCType = Convert.ToString(dt.Rows[0]["DTE_TC_TYPE"]);
                    objstore.sOperator1 = Convert.ToString(dt.Rows[0]["ED_OPERATOR1"]);
                    objstore.sOperator2 = Convert.ToString(dt.Rows[0]["ED_OPERATOR2"]);

                    objstore.sTankCapacity = Convert.ToString(dt.Rows[0]["DTE_TANK_CAPACITY"]);
                    objstore.sTCWeight = Convert.ToString(dt.Rows[0]["DTE_TC_WEIGHT"]);

                    objstore.sRating = Convert.ToString(dt.Rows[0]["DTE_RATING"]);
                    objstore.sStarRate = Convert.ToString(dt.Rows[0]["DTE_STAR_RATE"]);

                    objstore.sNamePlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_NAMEPLATE_PATH"]);
                    objstore.sSSPlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_SSPLATE_PATH"]);
                }
                return objstore;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetDetails");
                return objstore;
            }
        }

        public bool DeleteEnumerationDetails(clsStoreEnumeration objFieldEnum)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;
                strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_CANCEL_FLAG\"='1' WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsId + "'";
                ObjCon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "DeleteEnumerationDetails");
                return false;
            }
        }

        public string GetAddress(clsStoreEnumeration objstore)
        {
            try
            {

                string strQry = string.Empty;
                DataTable dt = new DataTable();
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                if (objstore.sValue == "3")
                {
                    strQry = "select \"TR_ADDRESS\" as ADDRESS from \"TBLTRANSREPAIRER\" where \"TR_ID\"='" + objstore.sSelectedValue + "'";
                }
                else if (objstore.sValue == "1")
                {
                    strQry = "select \"SM_ADDRESS\"  as ADDRESS from \"TBLSTOREMAST\" where \"SM_ID\"='" + objstore.sSelectedValue + "'";
                }
                ///return "";
              return ObjCon.get_value(strQry);
            }

            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetAddress");
                return ex.Message;
            }

        }

        public bool SaveImagePathDetails(clsStoreEnumeration objStoreEnum)
        {
            try
            {
                string strQry = string.Empty;
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string sMaxNo =  Convert.ToString(ObjCon.Get_max_no("EP_ID", "TBLENUMERATIONPHOTOS"));

                strQry = "INSERT INTO \"TBLENUMERATIONPHOTOS\" (\"EP_ID\",\"EP_ED_ID\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\",\"EP_CRBY\") VALUES ('" + sMaxNo + "','" + objStoreEnum.sEnumDetailsId + "',";
                strQry += " '" + objStoreEnum.sNamePlatePhotoPath + "','" + objStoreEnum.sSSPlatePhotoPath + "','" + objStoreEnum.sCrBy + "')";
               ObjCon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "SaveImagePathDetails");
                return false;
            }
        }

        public bool UpdateImagePathDetails(clsStoreEnumeration objStoreEnum)
        {
            try
            {
                string strQry = string.Empty;
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                strQry = "UPDATE \"TBLENUMERATIONPHOTOS\" SET \"EP_NAMEPLATE_PATH\"='" + objStoreEnum.sNamePlatePhotoPath + "',\"EP_SSPLATE_PATH\"='" + objStoreEnum.sSSPlatePhotoPath + "'";
                strQry += "  WHERE \"EP_ED_ID\"='" + objStoreEnum.sEnumDetailsId + "' ";

              ObjCon.ExecuteQry(strQry);
                return true;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "UpdateImagePathDetails");
                return false;
            }
        }


    }
}
