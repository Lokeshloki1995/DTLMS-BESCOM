﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IIITS.PGSQL.DAL;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Collections;
using System.Configuration;

namespace IIITS.DTLMS.BL
{
    public class clsFieldEnumeration
    {
        string strFormCode = "clsFieldEnumeration";
        //  CustOledbConnection ObjCon = new CustOledbConnection(Constants.Password);
        public string sTcId { get; set; }
        public string sCrBy { get; set; }
        public string sTimeId { get; set; }

        public string sOfficeCode { get; set; }
        public string sFeederCode { get; set; }
        public string sWeldDate { get; set; }
        public string sOperator1 { get; set; }
        public string sOperator2 { get; set; }

        public string sEnumDetailsID { get; set; }
        public string sEnumDTCID { get; set; }

        //TC Details
        public string sTcCode { get; set; }
        public string sTCSlno { get; set; }
        public string sTCCapacity { get; set; }
        public string sTCManfDate { get; set; }
        public string sTCMake { get; set; }
        public string sNamePlatePhotoPath { get; set; }
        public string sSSPlatePhotoPath { get; set; }
        public string sMakeName { get; set; }
        public string sTankCapacity { get; set; }
        public string sTCWeight { get; set; }
        public string sRating { get; set; }
        public string sStarRate { get; set; }

        public bool bTCSlNoNotExists { get; set; }

        //DTC Details
        public string sDTCName { get; set; }
        public string sDTCCode { get; set; }
        public string sOldDTCCode { get; set; }
        public string sIPDTCCode { get; set; }
        public string sEnumDate { get; set; }
        public string sOldCodePhotoPath { get; set; }
        public string sDTLMSCodePhotoPath { get; set; }
        public string sIPEnumCodePhotoPath { get; set; }
        public string sInfosysCodePhotoPath { get; set; }
        public string sDTCPhotoPath { get; set; }
        public string sInfosysAsset { get; set; }

        //Other DTC Details       
        public string sConnectedKW { get; set; }
        public string sConnectedHP { get; set; }
        public string sInternalCode { get; set; }
        public string sPlatformType { get; set; }
        public string sConnectionDate { get; set; }
        public string sInspectionDate { get; set; }
        public string sLastServiceDate { get; set; }
        public string sCommisionDate { get; set; }
        public string sKWHReading { get; set; }
        public string sLTlinelength { get; set; }
        public string sArresters { get; set; }
        public string sGrounding { get; set; }
        public string sHTProtect { get; set; }
        public string sLTProtect { get; set; }
        public string sDTCMeters { get; set; }
        public string sBreakertype { get; set; }
        public string sLatitude { get; set; }
        public string sLongitude { get; set; }
        public string sProjecttype { get; set; }
        public string sLoadtype { get; set; }
        public string sDepreciation { get; set; }

        public bool bIsIPDetails { get; set; }
        public bool bIsCESCDetails { get; set; }
        public bool bIsDTLMSDetails { get; set; }
        public string sIPCESCValue { get; set; }

        //Reject & Pending
        public string sRemark { get; set; }
        public string sQcRejectId { get; set; }
        public string sPendingForClarId { get; set; }
        public string sQCApprovalId { get; set; }

        //Store
        public string sLocType { get; set; }
        public string sLocName { get; set; }
        public string sLocAddress { get; set; }
        public string sTCType { get; set; }

        public string sEnumType { get; set; }
        public string sStatus { get; set; }

        public string sIsIPEnumDone { get; set; }

        int Circle_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Circle_code"]);
        int Division_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
        int SubDiv_code = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);
        int Section_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Section_code"]);
        int Feeder_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Feeder_code"]);
        public string[] SaveFieldEnumerationDetails(clsFieldEnumeration objFieldEnum)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            string strOfficeCode = string.Empty;
            Npgsql.NpgsqlDataReader dr;
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

            try
            {

                /// coded by pradeep - start
                //strQry = "SELECT VM_NAME FROM TBLINTERNALUSERS,TBLVENDORMASTER,TBLTCPLATEALLOCATIONMASTER,TBLTCPLATEALLOCATION WHERE";
                //strQry += " IU_VENDOR_ID=VM_ID AND VM_ID=TCPM_VENDOR_ID AND TCPM_ID=TCP_TCPM_ID AND IU_ID= '" + objFieldEnum.sOperator1 + "' AND TCP_STATUS_FLAG='0' AND TCP_TC_CODE = '" + objFieldEnum.sTcCode + "'";
                //dr = ObjCon.Fetch(strQry);
                //if (!dr.Read())
                //{
                //    dr.Close();
                //    strQry = "SELECT VM_NAME FROM TBLINTERNALUSERS,TBLVENDORMASTER,TBLTCPLATEALLOCATIONMASTER,TBLTCPLATEALLOCATION WHERE";
                //    strQry += " IU_VENDOR_ID=VM_ID AND VM_ID=TCPM_VENDOR_ID AND TCPM_ID=TCP_TCPM_ID AND TCP_TC_CODE = '" + objFieldEnum.sTcCode + "'";
                //    string name = ObjCon.get_value(strQry);
                //    if (!name.Trim().Equals(""))
                //    {
                //        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already allocated to vendor " + name;
                //        Arr[1] = "2";
                //        return Arr;
                //    }
                //}
                //dr.Close();
                //Arr[1] = "1"; //delete
                //return Arr; //delete
                ///coded by pradeep -end
                ///

                strQry = "SELECT \"TCP_TC_CODE\" FROM \"TBLTCPLATEALLOCATION\" WHERE \"TCP_TC_CODE\"='" + objFieldEnum.sTcCode + "'";
                string Tc_Code = ObjCon.get_value(strQry);

                if (Tc_Code == "")
                {
                    Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Not Allocated to Any Vendor";
                    Arr[1] = "2";
                    return Arr;
                }

                strQry = "SELECT \"VM_ID\" || '~' ||\"VM_NAME\" FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\",\"TBLVENDORMASTER\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND ";
                strQry += " \"VM_ID\" =\"TCPM_VENDOR_ID\" AND \"TCP_TC_CODE\"='" + objFieldEnum.sTcCode + "'";
                string sVmDetails = ObjCon.get_value(strQry);

                strQry = "SELECT \"IU_VENDOR_ID\" FROM \"TBLINTERNALUSERS\" WHERE \"IU_ID\"='" + objFieldEnum.sOperator1 + "'";
                string sLogUs_VM_Id = ObjCon.get_value(strQry);

                if (sLogUs_VM_Id == sVmDetails.Split('~').GetValue(0).ToString())
                {
                    strQry = "SELECT * FROM \"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE \"TCPM_ID\"=\"TCP_TCPM_ID\" AND \"TCP_STATUS_FLAG\" IN (0,4) AND ";
                    strQry += " \"TCP_TC_CODE\" ='" + objFieldEnum.sTcCode + "'";
                    string res = ObjCon.get_value(strQry);

                    if (res == "")
                    {
                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Not Possible to save because entered tc code may be Approved, deleted or Rejected ";
                        Arr[1] = "2";
                        return Arr;
                    }
                }
                else
                {
                    Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already allocated to vendor " + sVmDetails.Split('~').GetValue(1).ToString();
                    Arr[1] = "2";
                    return Arr;
                }

                if (objFieldEnum.sOfficeCode.Length >= Section_code)
                {
                    strOfficeCode = objFieldEnum.sOfficeCode.Substring(0, SubDiv_code);
                }

                strQry = "select \"FD_FEEDER_ID\" from \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" WHERE \"FD_FEEDER_CODE\"='" + objFieldEnum.sDTCCode.ToString().Substring(0, Feeder_code) + "' ";
                strQry += " AND  \"FD_FEEDER_ID\"=\"FDO_FEEDER_ID\" AND cast(\"FDO_OFFICE_CODE\" as text) LIKE '" + strOfficeCode + "%'";
                dr = ObjCon.Fetch(strQry);
                if (!dr.Read())
                {
                    dr.Close();
                    Arr[0] = "Code Does Not Match With The Feeder Code";
                    Arr[1] = "2";
                    return Arr;

                }
                dr.Close();

                //if (objFieldEnum.sTCManfDate != "")
                //{
                //    objFieldEnum.sTCManfDate = "01/" + objFieldEnum.sTCManfDate;
                //}

                /// //If sEnumDetailsID="" then Insert else Update;
                if (objFieldEnum.sEnumDetailsID == "")
                {

                    ObjCon.BeginTransaction();



                    dr = ObjCon.Fetch("SELECT \"DTE_TC_CODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    dr = ObjCon.Fetch("SELECT \"DTE_DTCCODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "DTC Code(DTLMS) " + objFieldEnum.sDTCCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    if (objFieldEnum.sTCMake != "")
                    {
                        dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objFieldEnum.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                        if (dr.Read())
                        {
                            Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                            Arr[1] = "2";
                            dr.Close();
                            return Arr;
                        }
                        dr.Close();
                    }

                    // Save to Enumeration Details (Basic Information in Enumeration)
                    objFieldEnum.sEnumDetailsID = Convert.ToString(ObjCon.Get_max_no("ED_ID", "TBLENUMERATIONDETAILS"));

                    strQry = "INSERT INTO \"TBLENUMERATIONDETAILS\" (\"ED_ID\",\"ED_OFFICECODE\",\"ED_OPERATOR1\",\"ED_OPERATOR2\",";
                    if (objFieldEnum.sWeldDate == null)
                    {
                        strQry += "\"ED_FEEDERCODE\",\"ED_ENUM_TYPE\",\"ED_LOCTYPE\",\"ED_CRBY\",\"ED_IP_ENUM_DONE\") VALUES (";
                    }
                    else
                    {
                        strQry += "\"ED_WELD_DATE\",\"ED_FEEDERCODE\",\"ED_ENUM_TYPE\",\"ED_LOCTYPE\",\"ED_CRBY\",\"ED_IP_ENUM_DONE\") VALUES (";
                    }

                    strQry += " '" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sOfficeCode + "','" + objFieldEnum.sOperator1 + "','" + objFieldEnum.sOperator2 + "',";

                    if (objFieldEnum.sWeldDate == null)
                    {
                        strQry += " '" + objFieldEnum.sFeederCode + "','2','2','" + objFieldEnum.sCrBy + "','" + objFieldEnum.sIsIPEnumDone + "')";
                    }
                    else
                    {
                        strQry += " TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),'" + objFieldEnum.sFeederCode + "','2','2','" + objFieldEnum.sCrBy + "','" + objFieldEnum.sIsIPEnumDone + "')";
                    }
                    ObjCon.ExecuteQry(strQry);

                    //Save to DTC and TC Details
                    objFieldEnum.sEnumDTCID = Convert.ToString(ObjCon.Get_max_no("DTE_ID", "TBLDTCENUMERATION"));

                    ////If Make is NNP, Save max No of TC_SLno+1
                    //if (objFieldEnum.sTCMake == "1")
                    //{
                    //    objFieldEnum.sTCSlno = ObjCon.get_value("SELECT NVL(MAX(CAST(DTE_TC_SLNO AS NUMBER(10))),0)+1 FROM TBLDTCENUMERATION WHERE DTE_MAKE='" + objFieldEnum.sTCMake + "'"); 
                    //}


                    if (objFieldEnum.bIsDTLMSDetails == true)
                    {
                        sIPCESCValue = "1";
                    }
                    if (objFieldEnum.bIsCESCDetails == true)
                    {
                        sIPCESCValue = "2";
                    }
                    if (objFieldEnum.bIsIPDetails == true)
                    {
                        sIPCESCValue = "3";
                    }

                    strQry = "INSERT INTO \"TBLDTCENUMERATION\"(\"DTE_ID\",\"DTE_ED_ID\",\"DTE_TC_CODE\",\"DTE_TC_SLNO\",\"DTE_MAKE\",\"DTE_CAPACITY\",";
                    if (objFieldEnum.sTCManfDate == "")
                    {
                        strQry += "\"DTE_NAME\",\"DTE_DTCCODE\",\"DTE_CESCCODE\",\"DTE_IPCODE\",";
                    }
                    else
                    {
                        strQry += "\"DTE_TC_MANFDATE\",\"DTE_NAME\",\"DTE_DTCCODE\",\"DTE_CESCCODE\",\"DTE_IPCODE\",";
                    }
                    if (objFieldEnum.sEnumDate == "")
                    {
                        strQry += "\"DTE_TOTAL_CON_KW\",\"DTE_TOTAL_CON_HP\",\"DTE_KWH_READING\",";
                    }
                    else
                    {
                        strQry += "\"DTE_ENUM_DATE\",\"DTE_TOTAL_CON_KW\",\"DTE_TOTAL_CON_HP\",\"DTE_KWH_READING\",";
                    }
                    if (objFieldEnum.sCommisionDate == "")
                    {
                        strQry += " \"DTE_PLATFORM\",";
                    }
                    else
                    {
                        strQry += " \"DTE_TRANS_COMMISION_DATE\",\"DTE_PLATFORM\",";
                    }
                    if (objFieldEnum.sLastServiceDate == "")
                    {
                        strQry += "\"DTE_BREAKER_TYPE\",\"DTE_INTERNAL_CODE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\",\"DTE_LT_PROTECT\",\"DTE_GROUNDING\",";
                    }
                    else
                    {
                        strQry += " \"DTE_LAST_SERVICE_DATE\",\"DTE_BREAKER_TYPE\",\"DTE_INTERNAL_CODE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\",\"DTE_LT_PROTECT\",\"DTE_GROUNDING\",";
                    }

                    strQry += " \"DTE_ARRESTERS\",\"DTE_LOADTYPE\",\"DTE_PROJECTTYPE\",\"DTE_LT_LINE\",\"DTE_LONGITUDE\",\"DTE_LATITUDE\",\"DTE_DEPRECIATION\",\"DTE_CRBY\",\"DTE_ISIPCESC\",\"DTE_TANK_CAPACITY\",";
                    strQry += " \"DTE_TC_WEIGHT\",\"DTE_INFOSYS_ASSET\",\"DTE_RATING\",\"DTE_STAR_RATE\") VALUES (";
                    strQry += " '" + objFieldEnum.sEnumDTCID + "','" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sTcCode + "','" + objFieldEnum.sTCSlno + "',";
                    strQry += " '" + objFieldEnum.sTCMake + "','" + objFieldEnum.sTCCapacity + "',";
                    if (objFieldEnum.sTCManfDate == "")
                    {
                        strQry += " '" + objFieldEnum.sDTCName + "','" + objFieldEnum.sDTCCode + "','" + objFieldEnum.sOldDTCCode + "','" + objFieldEnum.sIPDTCCode + "',";
                    }
                    else
                    {
                        strQry += "TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),'" + objFieldEnum.sDTCName + "','" + objFieldEnum.sDTCCode + "','" + objFieldEnum.sOldDTCCode + "','" + objFieldEnum.sIPDTCCode + "',";
                    }
                    if (objFieldEnum.sEnumDate == "")
                    {
                        strQry += "'" + objFieldEnum.sConnectedKW + "','" + objFieldEnum.sConnectedHP + "','" + objFieldEnum.sKWHReading + "',";
                    }
                    else
                    {
                        strQry += " TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),'" + objFieldEnum.sConnectedKW + "','" + objFieldEnum.sConnectedHP + "','" + objFieldEnum.sKWHReading + "',";
                    }
                    if (objFieldEnum.sCommisionDate == "")
                    {
                        strQry += "'" + objFieldEnum.sPlatformType + "',";
                    }
                    else
                    {
                        strQry += " TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),'" + objFieldEnum.sPlatformType + "',";
                    }
                    if (objFieldEnum.sLastServiceDate == "")
                    {
                        strQry += " '" + objFieldEnum.sBreakertype + "','" + objFieldEnum.sInternalCode + "','" + objFieldEnum.sDTCMeters + "','" + objFieldEnum.sHTProtect + "',";
                    }
                    else
                    {
                        strQry += " TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),'" + objFieldEnum.sBreakertype + "','" + objFieldEnum.sInternalCode + "','" + objFieldEnum.sDTCMeters + "','" + objFieldEnum.sHTProtect + "',";
                    }
                    strQry += " '" + objFieldEnum.sLTProtect + "','" + objFieldEnum.sGrounding + "','" + objFieldEnum.sArresters + "',";
                    strQry += " '" + objFieldEnum.sLoadtype + "','" + objFieldEnum.sProjecttype + "','" + objFieldEnum.sLTlinelength + "','" + objFieldEnum.sLongitude + "',";
                    strQry += " '" + objFieldEnum.sLatitude + "','" + objFieldEnum.sDepreciation + "','" + objFieldEnum.sCrBy + "','" + sIPCESCValue + "',";
                    strQry += " '" + objFieldEnum.sTankCapacity + "','" + objFieldEnum.sTCWeight + "','" + objFieldEnum.sInfosysAsset + "','" + objFieldEnum.sRating + "','" + objFieldEnum.sStarRate + "')";
                    ObjCon.ExecuteQry(strQry);

                    //coded by pradeep - start
                    //strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE \"TCP_TC_CODE\" ='" + objFieldEnum.sTcCode + "'";
                    //ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end

                    ObjCon.CommitTransaction();

                    Arr[0] = "Enumeration Details Saved Successfully";
                    Arr[1] = "0";

                    return Arr;
                }
                else
                {
                    ObjCon.BeginTransaction();

                    dr = ObjCon.Fetch("SELECT \"DTE_TC_CODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    dr = ObjCon.Fetch("SELECT \"DTE_DTCCODE\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "DTC Code(DTLMS) " + objFieldEnum.sDTCCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    dr = ObjCon.Fetch("SELECT \"DTE_TC_SLNO\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE \"DTE_MAKE\"='" + objFieldEnum.sTCMake + "' AND \"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "' AND \"DTE_ED_ID\" <>'" + objFieldEnum.sEnumDetailsID + "' AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\" NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_OFFICECODE\"='" + objFieldEnum.sOfficeCode + "',\"ED_OPERATOR1\"='" + objFieldEnum.sOperator1 + "',\"ED_OPERATOR2\"='" + objFieldEnum.sOperator2 + "'";
                    if (objFieldEnum.sWeldDate == "")
                    {
                        strQry += ",\"ED_FEEDERCODE\"='" + objFieldEnum.sFeederCode + "',\"ED_UPDATE_BY\"='" + objFieldEnum.sCrBy + "',  ";
                    }
                    else
                    {
                        strQry += " ,\"ED_WELD_DATE\"=TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),\"ED_FEEDERCODE\"='" + objFieldEnum.sFeederCode + "',\"ED_UPDATE_BY\"='" + objFieldEnum.sCrBy + "',  ";
                    }
                    
                    strQry += " \"ED_UPDATE_ON\"=NOW(),\"ED_IP_ENUM_DONE\"='" + objFieldEnum.sIsIPEnumDone + "' WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                    ObjCon.ExecuteQry(strQry);

                    ////If Make is NNP, Saving Enumeration DTC Id
                    //if (objFieldEnum.sTCMake == "1")
                    //{
                    //    objFieldEnum.sTCSlno = ObjCon.get_value("SELECT DTE_ID FROM TBLDTCENUMERATION WHERE DTE_ED_ID='" + objFieldEnum.sEnumDetailsID + "'");
                    //}

                    //if (objFieldEnum.sTCSlno == "")
                    //{
                    //    if (objFieldEnum.sTCMake == "1" && objFieldEnum.sTCSlno == "")
                    //    {
                    //        objFieldEnum.sTCSlno = ObjCon.get_value("SELECT DTE_ID FROM TBLDTCENUMERATION WHERE DTE_ED_ID='" + objFieldEnum.sEnumDetailsID + "'");
                    //    }
                    //}
                    if (objFieldEnum.sProjecttype == "" || objFieldEnum.sProjecttype == null)
                    {
                        objFieldEnum.sProjecttype = "0";
                    }

                    strQry = "UPDATE \"TBLDTCENUMERATION\" SET \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "',\"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "',\"DTE_MAKE\"='" + objFieldEnum.sTCMake + "'";
                    strQry += " ,\"DTE_CAPACITY\"='" + objFieldEnum.sTCCapacity + "',";
                    if (objFieldEnum.sTCManfDate == "")
                    {
                        strQry += " \"DTE_NAME\"='" + objFieldEnum.sDTCName.Replace("'", "''") + "',\"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "',\"DTE_CESCCODE\"='" + objFieldEnum.sOldDTCCode + "',\"DTE_IPCODE\"='" + objFieldEnum.sIPDTCCode + "',";
                    }
                    else
                    {
                        strQry += " \"DTE_TC_MANFDATE\"=TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),\"DTE_NAME\"='" + objFieldEnum.sDTCName.Replace("'", "''") + "',\"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "',\"DTE_CESCCODE\"='" + objFieldEnum.sOldDTCCode + "',\"DTE_IPCODE\"='" + objFieldEnum.sIPDTCCode + "',";
                    }
                    if(objFieldEnum.sEnumDate == "")
                    {
                        strQry += "\"DTE_TOTAL_CON_KW\"='" + objFieldEnum.sConnectedKW + "',\"DTE_TOTAL_CON_HP\"='" + objFieldEnum.sConnectedHP + "',";
                    }
                    else
                    {
                        strQry += " \"DTE_ENUM_DATE\"=TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),\"DTE_TOTAL_CON_KW\"='" + objFieldEnum.sConnectedKW + "',\"DTE_TOTAL_CON_HP\"='" + objFieldEnum.sConnectedHP + "',";
                    }
                    if(objFieldEnum.sCommisionDate == "")
                    {
                        strQry += " \"DTE_KWH_READING\"='" + objFieldEnum.sKWHReading + "',";
                    }
                    else
                    {
                        strQry += " \"DTE_KWH_READING\"='" + objFieldEnum.sKWHReading + "',\"DTE_TRANS_COMMISION_DATE\"=TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),";
                    }
                    if(objFieldEnum.sLastServiceDate == "")
                    {
                        strQry += "\"DTE_PLATFORM\"='" + objFieldEnum.sPlatformType + "',\"DTE_BREAKER_TYPE\"='" + objFieldEnum.sBreakertype + "',";
                    }
                    else
                    {
                        strQry += " \"DTE_LAST_SERVICE_DATE\"=TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),\"DTE_PLATFORM\"='" + objFieldEnum.sPlatformType + "',\"DTE_BREAKER_TYPE\"='" + objFieldEnum.sBreakertype + "',";
                    }
                    
                    strQry += " \"DTE_INTERNAL_CODE\"='" + objFieldEnum.sInternalCode + "',\"DTE_DTCMETERS\"='" + objFieldEnum.sDTCMeters + "',\"DTE_HT_PROTECT\"='" + objFieldEnum.sHTProtect + "',\"DTE_LT_PROTECT\"='" + objFieldEnum.sLTProtect + "'";
                    strQry += " ,\"DTE_GROUNDING\"='" + objFieldEnum.sGrounding + "',";
                    strQry += " \"DTE_ARRESTERS\"='" + objFieldEnum.sArresters + "',\"DTE_LOADTYPE\"='" + objFieldEnum.sLoadtype + "',\"DTE_PROJECTTYPE\"='" + objFieldEnum.sProjecttype + "',";
                    strQry += " \"DTE_LT_LINE\"='" + objFieldEnum.sLTlinelength + "',\"DTE_LONGITUDE\"='" + objFieldEnum.sLongitude + "',\"DTE_LATITUDE\"='" + objFieldEnum.sLatitude + "',";
                    strQry += " \"DTE_DEPRECIATION\"='" + objFieldEnum.sDepreciation + "',\"DTE_TANK_CAPACITY\"='" + objFieldEnum.sTankCapacity + "',\"DTE_TC_WEIGHT\"='" + objFieldEnum.sTCWeight + "',\"DTE_INFOSYS_ASSET\"='" + objFieldEnum.sInfosysAsset + "',";
                    strQry += " \"DTE_RATING\"='" + objFieldEnum.sRating + "',\"DTE_STAR_RATE\"='" + objFieldEnum.sStarRate + "' WHERE \"DTE_ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";

                    ObjCon.ExecuteQry(strQry);

                    if (objFieldEnum.sStatus == "2")
                    {
                        strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"='0' WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                        ObjCon.ExecuteQry(strQry);
                    }
                    //coded by pradeep - start
                    //strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE \"TCP_TC_CODE\" ='" + objFieldEnum.sTcCode + "'";
                    //ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end

                    ObjCon.CommitTransaction();

                    Arr[0] = "Enumeration Details Updated Successfully";
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

        public DataTable LoadFieldEnumeration(string sOperator = "")
        {
            DataTable dt = new DataTable();
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;
                strQry = "SELECT \"ED_ID\",\"ED_OFFICECODE\",\"ED_FEEDERCODE\",TO_CHAR(\"ED_WELD_DATE\",'DD-MON-YYYY') \"ED_WELD_DATE\",\"DTE_TC_CODE\",(SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"DTE_MAKE\"=\"TM_ID\") \"MAKE\",";
                strQry += " \"DTE_CAPACITY\",DTE_DTCCODE,DTE_CESCCODE,DTE_IPCODE ";
                strQry += " FROM \"TBLENUMERATIONDETAILS\",\"TBLDTCENUMERATION\" WHERE \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_CANCEL_FLAG\"='0' AND \"ED_ENUM_TYPE\"='2' ";
                if (sOperator != "")
                {
                    strQry += " AND  (\"ED_OPERATOR1\"='" + sOperator + "' OR \"ED_OPERATOR2\"='" + sOperator + "')";
                }

                return ObjCon.FetchDataTable(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "LoadFieldEnumeration");
                return dt;
            }
        }

        public bool DeleteEnumerationDetails(clsFieldEnumeration objFieldEnum)
        {

            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;
                strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"='5',\"ED_NOTES\"='DELETE FROM ENUMERATION FORM',\"ED_NOTES_ON\"=now() ";
                strQry += " WHERE \"ED_ID\" ='" + objFieldEnum.sEnumDetailsID + "'";
                ObjCon.ExecuteQry(strQry);
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "DeleteEnumerationDetails");
                return false;
            }
        }



        public bool SaveImagePathDetails(clsFieldEnumeration objFieldEnum)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;

                string sMaxNo = Convert.ToString(ObjCon.Get_max_no("EP_ID", "TBLENUMERATIONPHOTOS"));

                strQry = "INSERT INTO \"TBLENUMERATIONPHOTOS\" (\"EP_ID\",\"EP_ED_ID\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\",\"EP_OLDDTC_PATH\",\"EP_DTLMSDTC_PATH\",";
                strQry += " \"EP_IPENUMDTC_PATH\",\"EP_INFOSYSDTC_PATH\",\"EP_DTC_PATH\",\"EP_CRBY\") VALUES ('" + sMaxNo + "','" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sNamePlatePhotoPath + "',";
                strQry += " '" + objFieldEnum.sSSPlatePhotoPath + "','" + objFieldEnum.sOldCodePhotoPath + "','" + objFieldEnum.sDTLMSCodePhotoPath + "',";
                strQry += " '" + objFieldEnum.sIPEnumCodePhotoPath + "','" + objFieldEnum.sInfosysCodePhotoPath + "','" + objFieldEnum.sDTCPhotoPath + "','" + objFieldEnum.sCrBy + "')";
                ObjCon.ExecuteQry(strQry);
                return true;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "SaveImagePathDetails");
                return false;
            }
        }


        public bool UpdateImagePathDetails(clsFieldEnumeration objFieldEnum)
        {
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;

                strQry = "SELECT \"EP_ED_ID\" FROM \"TBLENUMERATIONPHOTOS\" WHERE \"EP_ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                string sResult = ObjCon.get_value(strQry);
                if (sResult != "")
                {
                    strQry = "UPDATE \"TBLENUMERATIONPHOTOS\" SET \"EP_NAMEPLATE_PATH\"='" + objFieldEnum.sNamePlatePhotoPath + "',\"EP_SSPLATE_PATH\"='" + objFieldEnum.sSSPlatePhotoPath + "',";
                    strQry += " \"EP_OLDDTC_PATH\"='" + objFieldEnum.sOldCodePhotoPath + "',\"EP_DTLMSDTC_PATH\"='" + objFieldEnum.sDTLMSCodePhotoPath + "',";
                    strQry += " \"EP_IPENUMDTC_PATH\"='" + objFieldEnum.sIPEnumCodePhotoPath + "',\"EP_INFOSYSDTC_PATH\"='" + objFieldEnum.sInfosysCodePhotoPath + "',";
                    strQry += " \"EP_DTC_PATH\"='" + objFieldEnum.sDTCPhotoPath + "' WHERE \"EP_ED_ID\"='" + objFieldEnum.sEnumDetailsID + "' ";
                    ObjCon.ExecuteQry(strQry);
                }
                else
                {
                    SaveImagePathDetails(objFieldEnum);
                }


                return true;

            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "UpdateImagePathDetails");
                return false;
            }
        }


        public object GetEnumerationDetails(clsFieldEnumeration objField)
        {
            try
            {
                string strQry = string.Empty;
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                //strQry = "SELECT ED_ID,ED_OFFICECODE,ED_OPERATOR1,DTE_NAME,ED_OPERATOR2,DTE_DTCCODE,DTE_CESCCODE,To_char(ED_WELD_DATE,'dd/mm/yyyy')ED_WELD_DATE,";
                //strQry += " ED_FEEDERCODE,ED_LOCTYPE,ED_LOCNAME,ED_LOCADDRESS,ED_ENUM_TYPE,ED_CRON,ED_CRBY,";
                //strQry += " DTE_ID,DTE_ED_ID,DTE_TC_CODE,DTE_TC_SLNO,DTE_MAKE,DTE_CAPACITY,DTE_TC_TYPE,TO_CHAR(DTE_TC_MANFDATE,'MM/yyyy') AS DTE_TC_MANFDATE";
                //strQry += ", DTE_INTERNAL_CODE,TO_CHAR(DTE_KWH_READING) DTE_KWH_READING, TO_CHAR(DTE_TOTAL_CON_HP) DTE_TOTAL_CON_HP,TO_CHAR(DTE_TOTAL_CON_KW) DTE_TOTAL_CON_KW,To_char(DTE_TRANS_COMMISION_DATE,'dd/mm/yyyy')DTE_TRANS_COMMISION_DATE,To_char(DTE_LAST_SERVICE_DATE,'dd/mm/yyyy')DTE_LAST_SERVICE_DATE ";
                //strQry += " ,DTE_PLATFORM,DTE_BREAKER_TYPE,DTE_DTCMETERS,DTE_HT_PROTECT,DTE_LT_PROTECT ,DTE_GROUNDING, DTE_ARRESTERS,DTE_LOADTYPE ";
                //strQry += ", DTE_LT_LINE,DTE_DEPRECIATION, DTE_LATITUDE,DTE_LONGITUDE ";
                //strQry += " ,DTE_IPCODE,TO_CHAR(DTE_ENUM_DATE,'dd/MM/yyyy') DTE_ENUM_DATE,ED_OPERATOR1,ED_OPERATOR2,DTE_CRON,DTE_ISIPCESC,DTE_TANK_CAPACITY,DTE_TC_WEIGHT,DTE_INFOSYS_ASSET,";
                //strQry += " EP_NAMEPLATE_PATH,EP_SSPLATE_PATH,EP_OLDDTC_PATH,EP_DTLMSDTC_PATH,EP_IPENUMDTC_PATH,EP_INFOSYSDTC_PATH,EP_DTC_PATH,";
                //strQry += " DTE_RATING,DTE_STAR_RATE,ED_IP_ENUM_DONE FROM ";
                //strQry += " TBLENUMERATIONDETAILS,TBLDTCENUMERATION,TBLENUMERATIONPHOTOS  WHERE ED_ID=DTE_ED_ID AND ED_ID=EP_ED_ID(+) AND ED_ID = '" + objField.sEnumDetailsID + "'";
                strQry = " SELECT \"ED_ID\",\"ED_OFFICECODE\",\"ED_OPERATOR1\",\"DTE_NAME\",\"ED_OPERATOR2\",\"DTE_DTCCODE\",\"DTE_CESCCODE\",To_char(\"ED_WELD_DATE\",'dd/mm/yyyy') \"ED_WELD_DATE\",\"ED_FEEDERCODE\",\"ED_LOCTYPE\",\"ED_LOCNAME\",\"ED_LOCADDRESS\",\"ED_ENUM_TYPE\",\"ED_CRON\",\"ED_CRBY\", ";
                strQry += " \"DTE_ID\",\"DTE_ED_ID\",\"DTE_TC_CODE\",\"DTE_TC_SLNO\",\"DTE_MAKE\",\"DTE_CAPACITY\",\"DTE_TC_TYPE\",TO_CHAR(\"DTE_TC_MANFDATE\",'dd/MM/yyyy') AS \"DTE_TC_MANFDATE\",\"DTE_INTERNAL_CODE\", CAST(\"DTE_KWH_READING\" AS text) \"DTE_KWH_READING\", CAST(\"DTE_TOTAL_CON_HP\" AS text) \"DTE_TOTAL_CON_HP\", ";
                strQry += " CAST(\"DTE_TOTAL_CON_KW\" AS text)\"DTE_TOTAL_CON_KW\",To_char(\"DTE_TRANS_COMMISION_DATE\",'dd/mm/yyyy') \"DTE_TRANS_COMMISION_DATE\",To_char(\"DTE_LAST_SERVICE_DATE\",'dd/mm/yyyy') \"DTE_LAST_SERVICE_DATE\",\"DTE_PLATFORM\",\"DTE_BREAKER_TYPE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\", ";
                strQry += " \"DTE_LT_PROTECT\",\"DTE_GROUNDING\",\"DTE_ARRESTERS\",\"DTE_LOADTYPE\",\"DTE_LT_LINE\",\"DTE_DEPRECIATION\",\"DTE_LATITUDE\",\"DTE_LONGITUDE\"  ,\"DTE_IPCODE\",TO_CHAR(\"DTE_ENUM_DATE\",'dd/MM/yyyy') \"DTE_ENUM_DATE\",\"ED_OPERATOR1\",\"ED_OPERATOR2\",\"DTE_CRON\",\"DTE_ISIPCESC\",  ";
                strQry += " \"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_INFOSYS_ASSET\",\"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\",\"EP_OLDDTC_PATH\",\"EP_DTLMSDTC_PATH\",\"EP_IPENUMDTC_PATH\",\"EP_INFOSYSDTC_PATH\",\"EP_DTC_PATH\",\"DTE_RATING\",\"DTE_STAR_RATE\",\"ED_IP_ENUM_DONE\" ";
                strQry += " FROM \"TBLENUMERATIONDETAILS\" INNER JOIN \"TBLDTCENUMERATION\" ON \"ED_ID\"=\"DTE_ED_ID\" INNER JOIN \"TBLENUMERATIONPHOTOS\" ON \"ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"='" + objField.sEnumDetailsID + "' ";

                DataTable dt = new DataTable();
                dt = ObjCon.FetchDataTable(strQry);

                if (dt.Rows.Count > 0)
                {
                    objField.sEnumDetailsID = Convert.ToString(dt.Rows[0]["ED_ID"]);
                    objField.sOfficeCode = Convert.ToString(dt.Rows[0]["ED_OFFICECODE"]);
                    objField.sFeederCode = Convert.ToString(dt.Rows[0]["ED_FEEDERCODE"]);
                    objField.sWeldDate = Convert.ToString(dt.Rows[0]["ED_WELD_DATE"]);
                    objField.sOperator1 = Convert.ToString(dt.Rows[0]["ED_OPERATOR1"]);
                    objField.sOperator2 = Convert.ToString(dt.Rows[0]["ED_OPERATOR2"]);

                    objField.sEnumDTCID = Convert.ToString(dt.Rows[0]["DTE_ID"]);
                    objField.sTcCode = Convert.ToString(dt.Rows[0]["DTE_TC_CODE"]);
                    objField.sTCMake = Convert.ToString(dt.Rows[0]["DTE_MAKE"]);
                    objField.sTCSlno = Convert.ToString(dt.Rows[0]["DTE_TC_SLNO"]);
                    objField.sTCManfDate = Convert.ToString(dt.Rows[0]["DTE_TC_MANFDATE"]);
                    objField.sTCCapacity = Convert.ToString(dt.Rows[0]["DTE_CAPACITY"]);

                    objField.sDTCName = Convert.ToString(dt.Rows[0]["DTE_NAME"]).Replace("'", "");
                    objField.sDTCCode = Convert.ToString(dt.Rows[0]["DTE_DTCCODE"]);
                    objField.sOldDTCCode = Convert.ToString(dt.Rows[0]["DTE_CESCCODE"]);
                    objField.sIPDTCCode = Convert.ToString(dt.Rows[0]["DTE_IPCODE"]);
                    objField.sEnumDate = Convert.ToString(dt.Rows[0]["DTE_ENUM_DATE"]);
                    objField.sInfosysAsset = Convert.ToString(dt.Rows[0]["DTE_INFOSYS_ASSET"]);

                    objField.sInternalCode = Convert.ToString(dt.Rows[0]["DTE_INTERNAL_CODE"]);
                    objField.sKWHReading = Convert.ToString(dt.Rows[0]["DTE_KWH_READING"]);
                    objField.sConnectedHP = Convert.ToString(dt.Rows[0]["DTE_TOTAL_CON_HP"]);
                    objField.sConnectedKW = Convert.ToString(dt.Rows[0]["DTE_TOTAL_CON_KW"]);
                    objField.sCommisionDate = Convert.ToString(dt.Rows[0]["DTE_TRANS_COMMISION_DATE"]);
                    objField.sLastServiceDate = Convert.ToString(dt.Rows[0]["DTE_LAST_SERVICE_DATE"]);


                    objField.sPlatformType = Convert.ToString(dt.Rows[0]["DTE_PLATFORM"]);
                    objField.sBreakertype = Convert.ToString(dt.Rows[0]["DTE_BREAKER_TYPE"]);
                    objField.sDTCMeters = Convert.ToString(dt.Rows[0]["DTE_DTCMETERS"]);
                    objField.sHTProtect = Convert.ToString(dt.Rows[0]["DTE_HT_PROTECT"]);
                    objField.sLTProtect = Convert.ToString(dt.Rows[0]["DTE_LT_PROTECT"]);

                    objField.sGrounding = Convert.ToString(dt.Rows[0]["DTE_GROUNDING"]);
                    objField.sArresters = Convert.ToString(dt.Rows[0]["DTE_ARRESTERS"]);
                    objField.sLoadtype = Convert.ToString(dt.Rows[0]["DTE_LOADTYPE"]);
                    objField.sLTlinelength = Convert.ToString(dt.Rows[0]["DTE_LT_LINE"]);
                    objField.sDepreciation = Convert.ToString(dt.Rows[0]["DTE_DEPRECIATION"]);
                    objField.sLatitude = Convert.ToString(dt.Rows[0]["DTE_LATITUDE"]);
                    objField.sLongitude = Convert.ToString(dt.Rows[0]["DTE_LONGITUDE"]);

                    objField.sIPCESCValue = Convert.ToString(dt.Rows[0]["DTE_ISIPCESC"]);

                    objField.sTankCapacity = Convert.ToString(dt.Rows[0]["DTE_TANK_CAPACITY"]);
                    objField.sTCWeight = Convert.ToString(dt.Rows[0]["DTE_TC_WEIGHT"]);

                    objField.sRating = Convert.ToString(dt.Rows[0]["DTE_RATING"]);
                    objField.sStarRate = Convert.ToString(dt.Rows[0]["DTE_STAR_RATE"]);

                    objField.sNamePlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_NAMEPLATE_PATH"]);
                    objField.sSSPlatePhotoPath = Convert.ToString(dt.Rows[0]["EP_SSPLATE_PATH"]);
                    objField.sOldCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_OLDDTC_PATH"]);
                    objField.sDTLMSCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_DTLMSDTC_PATH"]);
                    objField.sIPEnumCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_IPENUMDTC_PATH"]);
                    objField.sInfosysCodePhotoPath = Convert.ToString(dt.Rows[0]["EP_INFOSYSDTC_PATH"]);
                    objField.sDTCPhotoPath = Convert.ToString(dt.Rows[0]["EP_DTC_PATH"]);


                    objField.sLocType = Convert.ToString(dt.Rows[0]["ED_LOCTYPE"]);
                    objField.sLocName = Convert.ToString(dt.Rows[0]["ED_LOCNAME"]);
                    objField.sLocAddress = Convert.ToString(dt.Rows[0]["ED_LOCADDRESS"]);
                    objField.sTCType = Convert.ToString(dt.Rows[0]["DTE_TC_TYPE"]);

                    objField.sEnumType = Convert.ToString(dt.Rows[0]["ED_ENUM_TYPE"]);

                    objField.sIsIPEnumDone = Convert.ToString(dt.Rows[0]["ED_IP_ENUM_DONE"]);

                }
                return objField;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetDetails");
                return objField;
            }
        }

        public clsFieldEnumeration GetIPEnumerationData(clsFieldEnumeration objFieldEnum)
        {
            DataTable dt = new DataTable();
            try
            {
                string strQry = string.Empty;
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                strQry = "SELECT \"IP_LONGITUDE\",\"IP_LATITUDE\",\"IP_ARRESTERS\",\"IP_BREAKER_TYPE\",\"IP_HT_PROTECT\",\"IP_LT_PROTECT\",\"IP_DTCMETERS\",";
                strQry += " \"IP_GROUNDING\",\"IP_TOTAL_CON_KW\",\"IP_TOTAL_CON_HP\",\"IP_DTC_NAME\" FROM \"TBLIPENUMERATION\" WHERE \"IP_DTC_CODE\"='" + objFieldEnum.sIPDTCCode + "'";
                dt = ObjCon.FetchDataTable(strQry);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objFieldEnum.sLatitude = Convert.ToString(dt.Rows[i]["IP_LATITUDE"]);
                    objFieldEnum.sLongitude = Convert.ToString(dt.Rows[i]["IP_LONGITUDE"]);
                    objFieldEnum.sArresters = Convert.ToString(dt.Rows[i]["IP_ARRESTERS"]);
                    objFieldEnum.sBreakertype = Convert.ToString(dt.Rows[i]["IP_BREAKER_TYPE"]);
                    objFieldEnum.sHTProtect = Convert.ToString(dt.Rows[i]["IP_HT_PROTECT"]);
                    objFieldEnum.sLTProtect = Convert.ToString(dt.Rows[i]["IP_LT_PROTECT"]);
                    objFieldEnum.sDTCMeters = Convert.ToString(dt.Rows[i]["IP_DTCMETERS"]);
                    objFieldEnum.sGrounding = Convert.ToString(dt.Rows[i]["IP_GROUNDING"]);
                    objFieldEnum.sConnectedKW = Convert.ToString(dt.Rows[i]["IP_TOTAL_CON_KW"]);
                    objFieldEnum.sConnectedHP = Convert.ToString(dt.Rows[i]["IP_TOTAL_CON_HP"]);
                    objFieldEnum.sDTCName = Convert.ToString(dt.Rows[i]["IP_DTC_NAME"]);
                }
                return objFieldEnum;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetIPEnumerationData");
                return objFieldEnum;
            }
        }



        public clsFieldEnumeration GetCESCOldData(clsFieldEnumeration objFieldEnum)
        {
            DataTable dt = new DataTable();
            try
            {
                PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
                string strQry = string.Empty;
                strQry = " SELECT \"CE_DTC_CODE\",\"CE_DTC_NAME\",\"CE_LONGITUDE\",\"CE_LATITUDE\",CE_ARRESTERS,CE_BREAKER_TYPE,CE_HT_PROTECT,CE_LT_PROTECT,CE_DTCMETERS,CE_GROUNDING,";
                strQry += " \"CE_TOTAL_CON_KW\",\"CE_TOTAL_CON_HP\",\"CE_KWH_READING\",\"CE_INTERNAL_CODE\",\"CE_LOADTYPE\",\"CE_PROJECTTYPE\",\"CE_LT_LINE\",\"CE_DEPRECIATION\",";
                strQry += " \"CE_INFIASSETID\",\"CE_CTRATIO\" FROM \"TBLCESCENUMERATION\" WHERE \"CE_DTC_CODE\"='" + objFieldEnum.sOldDTCCode + "'";

                dt = ObjCon.FetchDataTable(strQry);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    objFieldEnum.sDTCMeters = Convert.ToString(dt.Rows[i]["CE_DTCMETERS"]);
                    objFieldEnum.sDTCName = Convert.ToString(dt.Rows[i]["CE_DTC_NAME"]).Replace("'", "");

                    objFieldEnum.sLatitude = Convert.ToString(dt.Rows[i]["CE_LATITUDE"]);
                    objFieldEnum.sLongitude = Convert.ToString(dt.Rows[i]["CE_LONGITUDE"]);
                    objFieldEnum.sArresters = Convert.ToString(dt.Rows[i]["CE_ARRESTERS"]);
                    objFieldEnum.sBreakertype = Convert.ToString(dt.Rows[i]["CE_BREAKER_TYPE"]);
                    objFieldEnum.sHTProtect = Convert.ToString(dt.Rows[i]["CE_HT_PROTECT"]);
                    objFieldEnum.sLTProtect = Convert.ToString(dt.Rows[i]["CE_LT_PROTECT"]);
                    objFieldEnum.sDTCMeters = Convert.ToString(dt.Rows[i]["CE_DTCMETERS"]);
                    objFieldEnum.sGrounding = Convert.ToString(dt.Rows[i]["CE_GROUNDING"]);
                    objFieldEnum.sConnectedKW = Convert.ToString(dt.Rows[i]["CE_TOTAL_CON_KW"]);
                    objFieldEnum.sConnectedHP = Convert.ToString(dt.Rows[i]["CE_TOTAL_CON_HP"]);
                    objFieldEnum.sKWHReading = Convert.ToString(dt.Rows[i]["CE_KWH_READING"]);
                    objFieldEnum.sInternalCode = Convert.ToString(dt.Rows[i]["CE_INTERNAL_CODE"]);
                    objFieldEnum.sLoadtype = Convert.ToString(dt.Rows[i]["CE_LOADTYPE"]);
                    objFieldEnum.sProjecttype = Convert.ToString(dt.Rows[i]["CE_PROJECTTYPE"]);
                    objFieldEnum.sLTlinelength = Convert.ToString(dt.Rows[i]["CE_LT_LINE"]);
                    objFieldEnum.sDepreciation = Convert.ToString(dt.Rows[i]["CE_DEPRECIATION"]);
                    objFieldEnum.sInfosysAsset = Convert.ToString(dt.Rows[i]["CE_INFIASSETID"]);


                }
                return objFieldEnum;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetIPEnumerationData");
                return objFieldEnum;
            }
        }


        #region QC Approval / Reject

        //ED_STATUS---------->0.Pending For Approval 1.Approved 2.Pending For Clarification 3.Reject
        public bool RejectEnumerationDetails(clsFieldEnumeration objFieldEnum)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);

            try
            {

                ObjCon.BeginTransaction();
                string strQry = string.Empty;

                objFieldEnum.sQcRejectId = Convert.ToString(ObjCon.Get_max_no("QR_ID", "TBLQCREJECT"));
                strQry = "INSERT INTO \"TBLQCREJECT\" (\"QR_ID\",\"QR_ED_ID\",\"QR_REMARKS\",\"QR_CRON\",\"QR_CRBY\") values ('" + objFieldEnum.sQcRejectId + "',";
                strQry += " '" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sRemark.Replace("'", "") + "',NOW(),";
                strQry += " '" + objFieldEnum.sCrBy + "')";
                ObjCon.ExecuteQry(strQry);

                strQry = "UPDATE \"TBLENUMERATIONDETAILS\" set \"ED_STATUS_FLAG\"=3 where CAST(\"ED_ID\" AS TEXT)='" + objFieldEnum.sEnumDetailsID + "'";
                ObjCon.ExecuteQry(strQry);
                ObjCon.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                ObjCon.RollBackTrans();
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "RejectEnumerationDetails");
                return false;

            }
        }

        public bool PendingForClarification(clsFieldEnumeration objFieldEnum)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
            try
            {

                ObjCon.BeginTransaction();
                string strQry = string.Empty;

                objFieldEnum.sPendingForClarId = Convert.ToString(ObjCon.Get_max_no("QP_ID", "TBLQCPENDING"));
                strQry = "INSERT INTO \"TBLQCPENDING\" (\"QP_ID\",\"QP_ED_ID\",\"QP_REMARKS\",\"QP_CRON\",\"QP_CRBY\") values ('" + objFieldEnum.sPendingForClarId + "','" + objFieldEnum.sEnumDetailsID + "',";
                strQry += " '" + objFieldEnum.sRemark.Replace("'", "") + "',NOW(),";
                strQry += " '" + objFieldEnum.sCrBy + "')";
                ObjCon.ExecuteQry(strQry);

                strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"=2 WHERE CAST(\"ED_ID\" AS TEXT)='" + objFieldEnum.sEnumDetailsID + "'";
                ObjCon.ExecuteQry(strQry);

                ObjCon.CommitTransaction();
                return true;

            }
            catch (Exception ex)
            {
                ObjCon.RollBackTrans();
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "PendingForClarification");
                return false;
            }
        }


        public string[] ApproveQCEnumerationDetails(clsFieldEnumeration objFieldEnum)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            Npgsql.NpgsqlDataReader dr;
            string strOfficeCode = string.Empty;
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
            try
            {

                strQry = "SELECT COALESCE(MAX(\"TL_ID\"),0)+1 FROM \"TBLTIMELOG\"";
                string ApproveTimeid = ObjCon.get_value(strQry);

                /// coded by pradeep - start
                strQry = "SELECT \"VM_NAME\" FROM \"TBLINTERNALUSERS\",\"TBLVENDORMASTER\",\"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE";
                strQry += " CAST(\"IU_VENDOR_ID\" AS TEXT)=CAST(\"VM_ID\" AS TEXT) AND CAST(\"VM_ID\" AS TEXT)=CAST(\"TCPM_VENDOR_ID\" AS TEXT) AND CAST(\"TCPM_ID\" AS TEXT)=CAST(\"TCP_TCPM_ID\" AS TEXT) AND CAST(\"IU_ID\" AS TEXT)= '" + objFieldEnum.sOperator1 + "' AND \"TCP_STATUS_FLAG\"='0' AND \"TCP_TC_CODE\" = '" + objFieldEnum.sTcCode + "'";
                string Vendorname = ObjCon.get_value(strQry);
                if (Vendorname == "")
                {
                    strQry = "SELECT \"VM_NAME\" FROM \"TBLVENDORMASTER\",\"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE";
                    strQry += " CAST(\"VM_ID\" AS TEXT)=CAST(\"TCPM_VENDOR_ID\" AS TEXT) AND CAST(\"TCPM_ID\" AS TEXT)=CAST(\"TCP_TCPM_ID\" AS TEXT) AND CAST(\"TCP_TC_CODE\" AS TEXT) = '" + objFieldEnum.sTcCode + "'";
                    string Actual_Vendorname = ObjCon.get_value(strQry);
                    if (!Actual_Vendorname.Trim().Equals(""))
                    {

                        strQry = "SELECT DISTINCT \"VM_NAME\" FROM \"TBLINTERNALUSERS\",\"TBLVENDORMASTER\",\"TBLTCPLATEALLOCATIONMASTER\",\"TBLTCPLATEALLOCATION\" WHERE";
                        strQry += " CAST(\"IU_VENDOR_ID\" AS TEXT)=CAST(\"VM_ID\" AS TEXT) AND CAST(\"VM_ID\" AS TEXT)=CAST(\"TCPM_VENDOR_ID\" AS TEXT) AND CAST(\"TCPM_ID\" AS TEXT)=CAST(\"TCP_TCPM_ID\" AS TEXT) AND CAST(\"IU_ID\" AS TEXT)= '" + objFieldEnum.sOperator1 + "'";
                        string Login_Vendorname = ObjCon.get_value(strQry);

                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already allocated to vendor " + Actual_Vendorname + " you can not Approve the same ss plate code to vendor " + Login_Vendorname;
                        Arr[1] = "2";
                        return Arr;
                    }
                    else
                    {
                        Arr[0] = "SS Plate Number " + objFieldEnum.sTcCode + "  Not allocated to any vendor. please Allocate ss plate and then approve the record .";
                        Arr[1] = "2";
                        return Arr;
                    }
                }

                //Arr[1] = "1"; //delete
                //return Arr; //delete
                ///coded by pradeep -end

                objFieldEnum.sTimeId = ApproveTimeid;

                DateTime starttime = DateTime.Now;
                strQry = "INSERT INTO \"TBLTIMELOG\" (\"TL_ID\",\"TL_PAGE_NAME\",\"TL_FUNCTION\",\"TL_START_TIME\",\"TL_TRANSACTION\")VALUES('" + ApproveTimeid + "','" + strFormCode + "',";
                strQry += "'ApproveQCEnumerationDetails',TO_DATE('" + starttime + "','mm/dd/yyyy HH:MI:SSAM'),'" + objFieldEnum.sTimeId + "')";
                ObjCon.ExecuteQry(strQry);

                if (objFieldEnum.sOfficeCode.Length >= Section_code)
                {
                    strOfficeCode = objFieldEnum.sOfficeCode.Substring(0, SubDiv_code);
                }
                if (objFieldEnum.sDTCCode != null)
                {

                    strQry = "select \"FD_FEEDER_ID\" from \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" WHERE CAST(\"FD_FEEDER_CODE\" AS TEXT)='" + objFieldEnum.sDTCCode.ToString().Substring(0, Feeder_code) + "' ";
                    strQry += " AND  CAST(\"FD_FEEDER_ID\" AS TEXT)=CAST(\"FDO_FEEDER_ID\" AS TEXT) AND CAST(\"FDO_OFFICE_CODE\" AS TEXT) LIKE '" + strOfficeCode + "%'";
                    dr = ObjCon.Fetch(strQry);
                    if (!dr.Read())
                    {
                        dr.Close();
                        Arr[0] = "Code Does Not Match With The Feeder Code";
                        Arr[1] = "2";
                        return Arr;

                    }
                    dr.Close();
                }


                if (objFieldEnum.sQCApprovalId == "")
                {

                    ObjCon.BeginTransaction();


                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE CAST(\"DTE_TC_CODE\" AS TEXT)='" + objFieldEnum.sTcCode + "' AND \"DTE_ED_ID\"<>'" + objFieldEnum.sEnumDetailsID + "' AND CAST(\"ED_ID\" AS TEXT)=CAST(\"DTE_ED_ID\" AS TEXT) AND CAST(\"ED_STATUS_FLAG\" AS TEXT) NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE CAST(\"DTE_TC_CODE\" AS TEXT)='" + objFieldEnum.sTcCode + "' AND \"DTE_ED_ID\"<>'" + objFieldEnum.sEnumDetailsID + "' AND CAST(\"ED_ID\" AS TEXT)=CAST(\"DTE_ED_ID\" AS TEXT) AND CAST(\"ED_STATUS_FLAG\" AS TEXT) NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "DTC Code(DTLMS) " + objFieldEnum.sDTCCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();

                    dr = ObjCon.Fetch("SELECT * FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONDETAILS\" WHERE CAST(\"DTE_MAKE\" AS TEXT)='" + objFieldEnum.sTCMake + "' AND CAST(\"DTE_TC_SLNO\" AS TEXT)='" + objFieldEnum.sTCSlno + "' AND CAST(\"DTE_ED_ID\" AS TEXT)<>'" + objFieldEnum.sEnumDetailsID + "' AND CAST(\"ED_ID\" AS TEXT)=CAST(\"DTE_ED_ID\" AS TEXT) AND CAST(\"ED_STATUS_FLAG\" AS TEXT) NOT IN ('3','5')");
                    if (dr.Read())
                    {
                        Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        return Arr;
                    }
                    dr.Close();



                    dr = ObjCon.Fetch("SELECT \"QA_ID\" FROM \"TBLQCAPPROVED\",\"TBLQCAPPROVEDOBJECTS\" WHERE CAST(\"QAO_QA_ID\" AS TEXT)=CAST(\"QA_ID\" AS TEXT) AND \"QAO_TC_CODE\"='" + objFieldEnum.sTcCode + "' ");
                    if (dr.Read())
                    {
                        Arr[0] = "Plate Number " + objFieldEnum.sTcCode + "  Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        ObjCon.RollBackTrans();
                        return Arr;
                    }
                    dr.Close();

                    if (objFieldEnum.sEnumType == "2")
                    {

                    }



                    dr = ObjCon.Fetch("SELECT \"QA_ID\" FROM \"TBLQCAPPROVED\",\"TBLQCAPPROVEDOBJECTS\" WHERE CAST(\"QAO_QA_ID\" AS TEXT)=CAST(\"QA_ID\" AS TEXT) AND \"QAO_MAKE\"='" + objFieldEnum.sTCMake + "' AND \"QAO_TC_SLNO\"='" + objFieldEnum.sTCSlno + "' ");
                    if (dr.Read())
                    {
                        Arr[0] = "Combination of Transformer Sl No " + objFieldEnum.sTCSlno + " and Make Name  " + objFieldEnum.sMakeName + " Already Exist";
                        Arr[1] = "2";
                        dr.Close();
                        ObjCon.RollBackTrans();
                        return Arr;
                    }
                    dr.Close();

                    string sApproveObjectId = string.Empty;

                    if (objFieldEnum.sEnumType == "2")
                    {
                        objFieldEnum.sLocType = "2";
                    }

                    // Save to Enumeration Details (Basic Information in Enumeration)
                    objFieldEnum.sQCApprovalId = Convert.ToString(ObjCon.Get_max_no("QA_ID", "TBLQCAPPROVED"));

                    strQry = "INSERT INTO \"TBLQCAPPROVED\" (\"QA_ID\",\"QA_ED_ID\",\"QA_OFFICECODE\",\"QA_OPERATOR1\",\"QA_OPERATOR2\",\"QA_WELD_DATE\",\"QA_FEEDERCODE\",";
                    strQry += " \"QA_LOCTYPE\",\"QA_LOCNAME\",\"QA_LOCADDRESS\",\"QA_TYPE\",\"QA_CRBY\",\"QA_CRON\") VALUES (";
                    strQry += " '" + objFieldEnum.sQCApprovalId + "','" + objFieldEnum.sEnumDetailsID + "','" + objFieldEnum.sOfficeCode + "',";
                    strQry += " '" + objFieldEnum.sOperator1 + "','" + objFieldEnum.sOperator2 + "',";
                    strQry += " TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),'" + objFieldEnum.sFeederCode + "','" + objFieldEnum.sEnumType + "',";
                    strQry += " '" + objFieldEnum.sLocName + "','" + objFieldEnum.sLocAddress + "','" + objFieldEnum.sEnumType + "','" + objFieldEnum.sCrBy + "',NOW())";
                    ObjCon.ExecuteQry(strQry);

                    //ObjCon.CommitTransaction();

                    //Save to DTC and TC Details
                    sApproveObjectId = Convert.ToString(ObjCon.Get_max_no("QAO_ID", "TBLQCAPPROVEDOBJECTS"));

                    //if (objFieldEnum.sTCManfDate != "")
                    //{
                    //    objFieldEnum.sTCManfDate = "01/" + objFieldEnum.sTCManfDate;
                    //}

                    //strQry = "INSERT INTO TBLQCAPPROVEDOBJECTS (QAO_ID,QAO_QA_ID,QAO_TC_CODE,QAO_TC_SLNO,QAO_MAKE,QAO_CAPACITY,QAO_TC_MANFDATE,QAO_TC_TYPE,";
                    //strQry += " QAO_NAME,QAO_DTCCODE,QAO_CESCCODE,QAO_IPCODE,QAO_ENUM_DATE,QAO_TOTAL_CON_KW,QAO_TOTAL_CON_HP,QAO_KWH_READING,QAO_TRANS_COMMISION_DATE,";
                    //strQry += " QAO_LAST_SERVICE_DATE,QAO_PLATFORM,QAO_BREAKER_TYPE,QAO_INTERNAL_CODE,QAO_DTCMETERS,QAO_HT_PROTECT,QAO_LT_PROTECT,QAO_GROUNDING,";
                    //strQry += " QAO_ARRESTERS,QAO_LOADTYPE,QAO_PROJECTTYPE,QAO_LT_LINE,QAO_LONGITUDE,QAO_LATITUDE,QAO_DEPRECIATION,QAO_CRBY,";
                    //strQry += " QAO_TANK_CAPACITY,QAO_TC_WEIGHT,QAO_INFOSYS_ASSET,QAO_RATING,QAO_STAR_RATE) VALUES (";
                    if (objFieldEnum.sTcCode == null)
                    {
                        objFieldEnum.sTcCode = "0";
                    }
                    if (objFieldEnum.sTCMake == null)
                    {
                        objFieldEnum.sTCMake = "0";
                    }
                    if (objFieldEnum.sTCCapacity == null)
                    {
                        objFieldEnum.sTCCapacity = "0";
                    }
                    if (objFieldEnum.sTCType == null)
                    {
                        objFieldEnum.sTCType = "0";
                    }

                    if (objFieldEnum.sConnectedKW == null)
                    {
                        objFieldEnum.sConnectedKW = "0";
                    }
                    if (objFieldEnum.sConnectedHP == null)
                    {
                        objFieldEnum.sConnectedHP = "0";
                    }
                    if (objFieldEnum.sKWHReading == null)
                    {
                        objFieldEnum.sKWHReading = "0";
                    }

                    if (objFieldEnum.sPlatformType == null)
                    {
                        objFieldEnum.sPlatformType = "0";
                    }
                    if (objFieldEnum.sBreakertype == null)
                    {
                        objFieldEnum.sBreakertype = "0";
                    }
                    if (objFieldEnum.sDTCMeters == null)
                    {
                        objFieldEnum.sDTCMeters = "0";
                    }

                    if (objFieldEnum.sHTProtect == null)
                    {
                        objFieldEnum.sHTProtect = "0";
                    }
                    if (objFieldEnum.sLTProtect == null)
                    {
                        objFieldEnum.sLTProtect = "0";
                    }
                    if (objFieldEnum.sGrounding == null)
                    {
                        objFieldEnum.sGrounding = "0";
                    }

                    if (objFieldEnum.sArresters == null)
                    {
                        objFieldEnum.sArresters = "0";
                    }

                    if (objFieldEnum.sLoadtype == null)
                    {
                        objFieldEnum.sLoadtype = "0";
                    }

                    if (objFieldEnum.sProjecttype == null)
                    {
                        objFieldEnum.sProjecttype = "0";
                    }

                    if (objFieldEnum.sRating == null || objFieldEnum.sRating == "")
                    {
                        objFieldEnum.sRating = "0";
                    }
                    if (objFieldEnum.sStarRate == null || objFieldEnum.sStarRate == "")
                    {
                        objFieldEnum.sStarRate = "0";
                    }


                    strQry = "INSERT INTO \"TBLQCAPPROVEDOBJECTS\" (\"QAO_ID\",\"QAO_QA_ID\",\"QAO_TC_CODE\",\"QAO_TC_SLNO\",\"QAO_MAKE\",\"QAO_CAPACITY\",\"QAO_TC_MANFDATE\",\"QAO_TC_TYPE\",";
                    strQry += " \"QAO_NAME\",\"QAO_DTCCODE\",\"QAO_CESCCODE\",\"QAO_IPCODE\",\"QAO_ENUM_DATE\",\"QAO_TOTAL_CON_KW\",\"QAO_TOTAL_CON_HP\",\"QAO_KWH_READING\",\"QAO_TRANS_COMMISION_DATE\",";
                    strQry += " \"QAO_LAST_SERVICE_DATE\",\"QAO_PLATFORM\",\"QAO_BREAKER_TYPE\",\"QAO_INTERNAL_CODE\",\"QAO_DTCMETERS\",\"QAO_HT_PROTECT\",\"QAO_LT_PROTECT\",\"QAO_GROUNDING\",";
                    strQry += " \"QAO_ARRESTERS\",\"QAO_LOADTYPE\",\"QAO_PROJECTTYPE\",\"QAO_LT_LINE\",\"QAO_LONGITUDE\",\"QAO_LATITUDE\",\"QAO_DEPRECIATION\",\"QAO_CRBY\",";
                    strQry += " \"QAO_TANK_CAPACITY\",\"QAO_TC_WEIGHT\",\"QAO_INFOSYS_ASSET\",\"QAO_RATING\",\"QAO_STAR_RATE\") VALUES (";
                    strQry += " '" + sApproveObjectId + "','" + objFieldEnum.sQCApprovalId + "','" + objFieldEnum.sTcCode + "','" + objFieldEnum.sTCSlno + "',";
                    strQry += " '" + objFieldEnum.sTCMake + "','" + objFieldEnum.sTCCapacity + "',TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),'" + objFieldEnum.sTCType + "',";
                    strQry += " '" + objFieldEnum.sDTCName + "','" + objFieldEnum.sDTCCode + "','" + objFieldEnum.sOldDTCCode + "','" + objFieldEnum.sIPDTCCode + "',";
                    strQry += " TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),'" + objFieldEnum.sConnectedKW + "','" + objFieldEnum.sConnectedHP + "','" + objFieldEnum.sKWHReading + "',";
                    strQry += " TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),'" + objFieldEnum.sPlatformType + "',";
                    strQry += " '" + objFieldEnum.sBreakertype + "','" + objFieldEnum.sInternalCode + "','" + objFieldEnum.sDTCMeters + "','" + objFieldEnum.sHTProtect + "',";
                    strQry += " '" + objFieldEnum.sLTProtect + "','" + objFieldEnum.sGrounding + "','" + objFieldEnum.sArresters + "',";
                    strQry += " '" + objFieldEnum.sLoadtype + "','" + objFieldEnum.sProjecttype + "','" + objFieldEnum.sLTlinelength + "','" + objFieldEnum.sLongitude + "',";
                    strQry += " '" + objFieldEnum.sLatitude + "','" + objFieldEnum.sDepreciation + "','" + objFieldEnum.sCrBy + "',";
                    strQry += " '" + objFieldEnum.sTankCapacity + "','" + objFieldEnum.sTCWeight + "','" + objFieldEnum.sInfosysAsset + "',";
                    strQry += " '" + objFieldEnum.sRating + "','" + objFieldEnum.sStarRate + "')";
                    ObjCon.ExecuteQry(strQry);



                    // Save To Main Table

                    #region TC Details

                    // Transformer Details
                    clsTcMaster objTcMaster = new clsTcMaster();

                    objTcMaster.sTimeId = objFieldEnum.sTimeId;

                    objTcMaster.sTcId = "";
                    objTcMaster.sTcSlNo = objFieldEnum.sTCSlno;
                    objTcMaster.sTcMakeId = objFieldEnum.sTCMake;

                    objTcMaster.sTcCode = objFieldEnum.sTcCode;
                    objTcMaster.sTcCapacity = objFieldEnum.sTCCapacity;
                    objTcMaster.sManufacDate = objFieldEnum.sTCManfDate;
                    //objTcMaster.sPurchaseDate = txtPurchaseDate.Text;
                    //if (cmbSupplier.SelectedIndex > 0)
                    //{
                    //    objTcMaster.sSupplierId = cmbSupplier.SelectedValue;
                    //}
                    objTcMaster.sPoNo = "";
                    objTcMaster.sPrice = "";
                    objTcMaster.sWarrentyPeriod = "";
                    objTcMaster.sLastServiceDate = "";
                    objTcMaster.sCurrentLocation = objFieldEnum.sEnumType;
                    objTcMaster.sTcLifeSpan = "";
                    objTcMaster.sCrBy = objFieldEnum.sCrBy;
                    objTcMaster.sOfficeCode = objFieldEnum.sOfficeCode;

                    objTcMaster.sRating = objFieldEnum.sRating;
                    objTcMaster.sStarRate = objFieldEnum.sStarRate;

                    //ObjCon.CommitTransaction();

                    Arr = objTcMaster.SaveUpdateTransformerDetails(objTcMaster);

                    if (Arr[1] == "2")
                    {
                        ObjCon.RollBackTrans();
                        return Arr;
                    }
                    #endregion


                    //EnumType-------> 1.Store 2.Field 3.Repairer
                    if (objFieldEnum.sEnumType == "2")
                    {
                        // DTC Details

                        #region DTC Details

                        clsDTCCommision objDtcCommision = new clsDTCCommision();

                        objDtcCommision.sTimeId = objFieldEnum.sTimeId; ;
                        objDtcCommision.lDtcId = "";
                        objDtcCommision.sDtcName = objFieldEnum.sDTCName;
                        objDtcCommision.iConnectedHP = Convert.ToString(objFieldEnum.sConnectedHP);
                        objDtcCommision.iConnectedKW = Convert.ToString(objFieldEnum.sConnectedKW);
                        objDtcCommision.sInternalCode = objFieldEnum.sInternalCode;
                        //objDtcCommision.sFeederChangeDate = objFieldEnum.sFeederCode;
                        objDtcCommision.sServiceDate = objFieldEnum.sLastServiceDate;
                        objDtcCommision.sOMSectionName = objFieldEnum.sOfficeCode;
                        objDtcCommision.sDtcCode = objFieldEnum.sDTCCode;
                        objDtcCommision.iKWHReading = objFieldEnum.sKWHReading;
                        objDtcCommision.sCommisionDate = objFieldEnum.sCommisionDate;
                        //objDtcCommision.sConnectionDate = txtConnectionDate.Text;
                        objDtcCommision.sTcCode = objFieldEnum.sTcCode;
                        objDtcCommision.sCrBy = objFieldEnum.sCrBy;
                        // objDtcCommision.sOldTcCode = txtOldTCCode.Text;

                        // objDtcCommision.sWOslno = txtWOslno.Text;
                        objDtcCommision.sOfficeCode = objFieldEnum.sOfficeCode;

                        Arr = objDtcCommision.SaveUpdateDtcDetails(objDtcCommision);

                        if (Arr[1] == "2")
                        {
                            ObjCon.RollBackTrans();
                            return Arr;
                        }


                        #endregion


                        //Other Details

                        #region DTC Other Details

                        objDtcCommision.lDtcId = objDtcCommision.lDtcId;
                        objDtcCommision.sCrBy = objFieldEnum.sCrBy;
                        //objDtcCommision.sHtlinelength = objFieldEnum.sLTlinelength;
                        objDtcCommision.sLtlinelength = objFieldEnum.sLTlinelength;
                        objDtcCommision.sPlatformType = objFieldEnum.sPlatformType;
                        objDtcCommision.sArresters = objFieldEnum.sArresters;
                        objDtcCommision.sGrounding = objFieldEnum.sGrounding;
                        objDtcCommision.sLTProtect = objFieldEnum.sLTProtect;
                        objDtcCommision.sHTProtect = objFieldEnum.sHTProtect;
                        objDtcCommision.sDTCMeters = objFieldEnum.sDTCMeters;
                        objDtcCommision.sBreakertype = objFieldEnum.sBreakertype;
                        objDtcCommision.sLoadtype = objFieldEnum.sLoadtype;
                        objDtcCommision.sProjecttype = objFieldEnum.sProjecttype;

                        objDtcCommision.sLongitude = objFieldEnum.sLongitude;
                        objDtcCommision.sLatitude = objFieldEnum.sLatitude;
                        objDtcCommision.sDepreciation = objFieldEnum.sDepreciation;

                        Arr = objDtcCommision.SaveUpdateDtcSpecification(objDtcCommision);

                        #endregion
                    }


                    #region Approve Flag

                    strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_STATUS_FLAG\"=1,\"ED_APPROVED_BY\"='" + objFieldEnum.sCrBy + "', ";
                    strQry += " \"ED_APPROVED_ON\"=NOW() WHERE \"ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";
                    ObjCon.ExecuteQry(strQry);

                    #endregion

                    //coded by pradeep - start
                    strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE CAST(\"TCP_TC_CODE\" AS TEXT) ='" + objFieldEnum.sTcCode + "'";
                    ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end

                    //ObjCon.CommitTransaction();

                    DateTime endtime = DateTime.Now;
                    strQry = "UPDATE \"TBLTIMELOG\" SET \"TL_END_TIME\"= TO_DATE('" + endtime + "','mm/dd/yyyy HH:MI:SSAM') WHERE \"TL_ID\"='" + ApproveTimeid + "'";
                    ObjCon.ExecuteQry(strQry);

                    Arr[0] = "Enumeration Details Approved Successfully";
                    Arr[1] = "0";
                    ObjCon.CommitTransaction();
                    return Arr;
                }
                else
                {
                    ObjCon.BeginTransaction();

                    strQry = "UPDATE \"TBLENUMERATIONDETAILS\" SET \"ED_OFFICECODE\"='" + objFieldEnum.sOfficeCode + "',\"ED_OPERATOR1\"='" + objFieldEnum.sOperator1 + "',\"ED_OPERATOR2\"='" + objFieldEnum.sOperator2 + "'";
                    strQry += " ,\"ED_WELD_DATE\"=TO_DATE('" + objFieldEnum.sWeldDate + "','dd/MM/yyyy'),\"ED_FEEDERCODE\"='" + objFieldEnum.sFeederCode + "' WHERE ";
                    strQry += " CAST(\"ED_ID\" AS TEXT)='" + objFieldEnum.sEnumDetailsID + "'";
                    ObjCon.ExecuteQry(strQry);

                    //If TC Slno Not Exists, Saving Enumeration DTC Id
                    if (bTCSlNoNotExists == true)
                    {
                        objFieldEnum.sTCSlno = ObjCon.get_value("SELECT \"DTE_ID\" FROM \"TBLDTCENUMERATION\" WHERE CAST(\"DTE_ED_ID\" AS TEXT)='" + objFieldEnum.sEnumDetailsID + "'");
                    }

                    strQry = "UPDATE \"TBLDTCENUMERATION\" SET \"DTE_TC_CODE\"='" + objFieldEnum.sTcCode + "',\"DTE_TC_SLNO\"='" + objFieldEnum.sTCSlno + "',\"DTE_MAKE\"='" + objFieldEnum.sTCMake + "'";
                    strQry += " ,\"DTE_CAPACITY\"='" + objFieldEnum.sTCCapacity + "',\"DTE_TC_MANFDATE\"=TO_DATE('" + objFieldEnum.sTCManfDate + "','dd/MM/yyyy'),";
                    strQry += " \"DTE_NAME\"='" + objFieldEnum.sDTCName + "',\"DTE_DTCCODE\"='" + objFieldEnum.sDTCCode + "',\"DTE_CESCCODE\"='" + objFieldEnum.sOldDTCCode + "',\"DTE_IPCODE\"='" + objFieldEnum.sIPDTCCode + "',";
                    strQry += " \"DTE_ENUM_DATE\"=TO_DATE('" + objFieldEnum.sEnumDate + "','dd/MM/yyyy'),\"DTE_TOTAL_CON_KW\"='" + objFieldEnum.sConnectedKW + "',\"DTE_TOTAL_CON_HP\"='" + objFieldEnum.sConnectedHP + "',";
                    strQry += " \"DTE_KWH_READING\"='" + objFieldEnum.sKWHReading + "',\"DTE_TRANS_COMMISION_DATE\"=TO_DATE('" + objFieldEnum.sCommisionDate + "','dd/MM/yyyy'),";
                    strQry += " \"DTE_LAST_SERVICE_DATE\"=TO_DATE('" + objFieldEnum.sLastServiceDate + "','dd/MM/yyyy'),\"DTE_PLATFORM\"='" + objFieldEnum.sPlatformType + "',\"DTE_BREAKER_TYPE\"='" + objFieldEnum.sBreakertype + "',";

                    strQry += " \"DTE_INTERNAL_CODE\"='" + objFieldEnum.sInternalCode + "',\"DTE_DTCMETERS\"='" + objFieldEnum.sDTCMeters + "',\"DTE_HT_PROTECT\"='" + objFieldEnum.sHTProtect + "',\"DTE_LT_PROTECT\"='" + objFieldEnum.sLTProtect + "'";

                    strQry += " ,\"DTE_GROUNDING\"='" + objFieldEnum.sGrounding + "',";
                    strQry += " \"DTE_ARRESTERS\"='" + objFieldEnum.sArresters + "',\"DTE_LOADTYPE\"='" + objFieldEnum.sLoadtype + "',\"DTE_PROJECTTYPE\"='" + objFieldEnum.sProjecttype + "',";
                    strQry += " \"DTE_LT_LINE\"='" + objFieldEnum.sLTlinelength + "',\"DTE_LONGITUDE\"='" + objFieldEnum.sLongitude + "',\"DTE_LATITUDE\"='" + objFieldEnum.sLatitude + "',";
                    strQry += " \"DTE_DEPRECIATION\"='" + objFieldEnum.sDepreciation + "' WHERE \"DTE_ED_ID\"='" + objFieldEnum.sEnumDetailsID + "'";

                    ObjCon.ExecuteQry(strQry);

                    //coded by pradeep - start
                    strQry = "UPDATE \"TBLTCPLATEALLOCATION\" SET  \"TCP_STATUS_FLAG\" = '1' WHERE CAST(\"TCP_TC_CODE\" AS TEXT) ='" + objFieldEnum.sTcCode + "'";
                    ObjCon.ExecuteQry(strQry);
                    //coded by pradeep - end

                    //ObjCon.CommitTransaction();

                    DateTime endtime = DateTime.Now;
                    strQry = "UPDATE \"TBLTIMELOG\" SET \"TL_END_TIME\"= TO_DATE('" + endtime + "','mm/dd/yyyy HH:MI:SSAM') WHERE \"TL_ID\"='" + ApproveTimeid + "'";
                    ObjCon.ExecuteQry(strQry);



                    Arr[0] = "Enumeration Details Updated Successfully";
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


        public string[] GetEnumerationInfoForApprove(clsFieldEnumeration objFieldEnum, ArrayList sEnumIdList)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
            DataTable dt = new DataTable();
            PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
            try
            {
                strQry = "DELETE FROM \"TBLTIMELOG\"";
                ObjCon.ExecuteQry(strQry);

                strQry = "SELECT COALESCE(MAX(\"TL_ID\"),0)+1 FROM \"TBLTIMELOG\"";
                string Timeid = ObjCon.get_value(strQry);
                objFieldEnum.sTimeId = Timeid;
                DateTime starttime = DateTime.Now;
                strQry = "INSERT INTO \"TBLTIMELOG\" (\"TL_ID\",\"TL_PAGE_NAME\",\"TL_FUNCTION\",\"TL_START_TIME\",\"TL_TRANSACTION\")VALUES('" + Timeid + "','" + strFormCode + "',";
                strQry += "'GetEnumerationInfoForApprove',TO_DATE('" + starttime + "','mm/dd/yyyy HH:MI:SSAM'),'" + Timeid + "')";
                ObjCon.ExecuteQry(strQry);
                for (int i = 0; i < sEnumIdList.Count; i++)
                {

                    //strQry = "SELECT ED_ID,ED_OFFICECODE,ED_OPERATOR1,ED_OPERATOR2,TO_CHAR(ED_WELD_DATE,'DD/MM/YYYY') ED_WELD_DATE,ED_FEEDERCODE,ED_LOCTYPE,ED_LOCNAME,ED_LOCADDRESS,ED_ENUM_TYPE,";
                    //strQry += " DTE_TC_CODE,DTE_TC_SLNO,DTE_MAKE,DTE_CAPACITY,DTE_TC_TYPE,TO_CHAR(DTE_TC_MANFDATE,'MM/YYYY') DTE_TC_MANFDATE,DTE_NAME,DTE_DTCCODE,";
                    //strQry += " DTE_CESCCODE,DTE_IPCODE,DTE_TOTAL_CON_KW,DTE_TOTAL_CON_HP,DTE_KWH_READING,TO_CHAR(DTE_TRANS_COMMISION_DATE,'DD/MM/YYYY') DTE_TRANS_COMMISION_DATE,TO_CHAR(DTE_LAST_SERVICE_DATE,'DD/MM/YYYY') DTE_LAST_SERVICE_DATE,";
                    //strQry += " DTE_BREAKER_TYPE,DTE_INTERNAL_CODE,DTE_DTCMETERS,DTE_HT_PROTECT,DTE_LT_PROTECT,DTE_GROUNDING,DTE_ARRESTERS,DTE_PLATFORM,";
                    //strQry += " DTE_LOADTYPE,DTE_PROJECTTYPE,DTE_LT_LINE,DTE_DEPRECIATION,ED_IP_ENUM_DONE, ";
                    //strQry += " DTE_ISIPCESC,DTE_TANK_CAPACITY,DTE_TC_WEIGHT,DTE_INFOSYS_ASSET,DTE_RATING,DTE_STAR_RATE,TO_CHAR(DTE_ENUM_DATE,'DD/MM/YYYY') DTE_ENUM_DATE,DTE_LATITUDE,DTE_LONGITUDE ";
                    //strQry += " FROM TBLENUMERATIONDETAILS,TBLDTCENUMERATION ";
                    //strQry += " WHERE ED_ID=DTE_ED_ID AND ED_ID='" + sEnumIdList[i] + "'";

                    strQry = "SELECT \"ED_ID\",\"ED_OFFICECODE\",\"ED_OPERATOR1\",\"ED_CRBY\",\"ED_OPERATOR2\",TO_CHAR(\"ED_WELD_DATE\",'DD/MM/YYYY') \"ED_WELD_DATE\",\"ED_FEEDERCODE\",\"ED_LOCTYPE\",\"ED_LOCNAME\",\"ED_LOCADDRESS\",\"ED_ENUM_TYPE\",";
                    strQry += " \"DTE_TC_CODE\",\"DTE_TC_SLNO\",\"DTE_MAKE\",\"DTE_CAPACITY\",\"DTE_TC_TYPE\",TO_CHAR(\"DTE_TC_MANFDATE\",'MM/YYYY') \"DTE_TC_MANFDATE\",\"DTE_NAME\",\"DTE_DTCCODE\",";
                    strQry += " \"DTE_CESCCODE\",\"DTE_IPCODE\",\"DTE_TOTAL_CON_KW\",\"DTE_TOTAL_CON_HP\",\"DTE_KWH_READING\",TO_CHAR(\"DTE_TRANS_COMMISION_DATE\",'DD/MM/YYYY')\"DTE_TRANS_COMMISION_DATE\",TO_CHAR(\"DTE_LAST_SERVICE_DATE\",'DD/MM/YYYY') \"DTE_LAST_SERVICE_DATE\",";
                    strQry += " \"DTE_BREAKER_TYPE\",\"DTE_INTERNAL_CODE\",\"DTE_DTCMETERS\",\"DTE_HT_PROTECT\",\"DTE_LT_PROTECT\",\"DTE_GROUNDING\",\"DTE_ARRESTERS\",\"DTE_PLATFORM\",";
                    strQry += " \"DTE_LOADTYPE\",\"DTE_PROJECTTYPE\",\"DTE_LT_LINE\",\"DTE_DEPRECIATION\",\"ED_IP_ENUM_DONE\",\"";
                    strQry += "DTE_ISIPCESC\",\"DTE_TANK_CAPACITY\",\"DTE_TC_WEIGHT\",\"DTE_INFOSYS_ASSET\",\"DTE_RATING\",\"DTE_STAR_RATE\",TO_CHAR(\"DTE_ENUM_DATE\",'DD/MM/YYYY') \"DTE_ENUM_DATE\",\"DTE_LATITUDE\",\"DTE_LONGITUDE\" ";
                    strQry += " FROM \"TBLENUMERATIONDETAILS\",\"TBLDTCENUMERATION\" ";
                    strQry += " WHERE CAST(\"ED_ID\" AS TEXT)=CAST(\"DTE_ED_ID\" AS TEXT) AND \"ED_ID\"='" + sEnumIdList[i] + "'";

                    dt = ObjCon.FetchDataTable(strQry);
                    if (dt.Rows.Count > 0)
                    {
                        objFieldEnum.sQCApprovalId = "";
                        objFieldEnum.sEnumDetailsID = Convert.ToString(dt.Rows[0]["ED_ID"]);

                        objFieldEnum.sWeldDate = Convert.ToString(dt.Rows[0]["ED_WELD_DATE"]);
                        objFieldEnum.sOperator1 = Convert.ToString(dt.Rows[0]["ED_OPERATOR1"]);
                        objFieldEnum.sOperator2 = Convert.ToString(dt.Rows[0]["ED_OPERATOR2"]);

                        //TC Details

                        objFieldEnum.sTcCode = Convert.ToString(dt.Rows[0]["DTE_TC_CODE"]);
                        objFieldEnum.sTCMake = Convert.ToString(dt.Rows[0]["DTE_MAKE"]);
                        objFieldEnum.sTCCapacity = Convert.ToString(dt.Rows[0]["DTE_CAPACITY"]);
                        objFieldEnum.sTCSlno = Convert.ToString(dt.Rows[0]["DTE_TC_SLNO"]);
                        objFieldEnum.sTCManfDate = Convert.ToString(dt.Rows[0]["DTE_TC_MANFDATE"]);

                        objFieldEnum.sEnumType = Convert.ToString(dt.Rows[0]["ED_ENUM_TYPE"]);

                        if (objFieldEnum.sEnumType == "2")
                        {

                            objFieldEnum.sOfficeCode = Convert.ToString(dt.Rows[0]["ED_OFFICECODE"]);
                            objFieldEnum.sFeederCode = Convert.ToString(dt.Rows[0]["ED_FEEDERCODE"]);

                            // DTC Details
                            objFieldEnum.sDTCName = Convert.ToString(dt.Rows[0]["DTE_NAME"]).Replace("'", "");
                            objFieldEnum.sDTCCode = Convert.ToString(dt.Rows[0]["DTE_DTCCODE"]);
                            objFieldEnum.sOldDTCCode = Convert.ToString(dt.Rows[0]["DTE_CESCCODE"]);
                            objFieldEnum.sIPDTCCode = Convert.ToString(dt.Rows[0]["DTE_IPCODE"]);
                            objFieldEnum.sEnumDate = Convert.ToString(dt.Rows[0]["DTE_ENUM_DATE"]);


                            // DTC Other Details
                            objFieldEnum.sInternalCode = Convert.ToString(dt.Rows[0]["DTE_INTERNAL_CODE"]);
                            objFieldEnum.sConnectedKW = Convert.ToString(dt.Rows[0]["DTE_TOTAL_CON_KW"]);
                            objFieldEnum.sConnectedHP = Convert.ToString(dt.Rows[0]["DTE_TOTAL_CON_HP"]);
                            objFieldEnum.sKWHReading = Convert.ToString(dt.Rows[0]["DTE_KWH_READING"]);
                            objFieldEnum.sCommisionDate = Convert.ToString(dt.Rows[0]["DTE_TRANS_COMMISION_DATE"]);
                            objFieldEnum.sLastServiceDate = Convert.ToString(dt.Rows[0]["DTE_LAST_SERVICE_DATE"]);

                            //Other Info about DTC
                            objFieldEnum.sPlatformType = Convert.ToString(dt.Rows[0]["DTE_PLATFORM"]);
                            objFieldEnum.sBreakertype = Convert.ToString(dt.Rows[0]["DTE_BREAKER_TYPE"]);
                            objFieldEnum.sDTCMeters = Convert.ToString(dt.Rows[0]["DTE_DTCMETERS"]);
                            objFieldEnum.sHTProtect = Convert.ToString(dt.Rows[0]["DTE_HT_PROTECT"]);
                            objFieldEnum.sLTProtect = Convert.ToString(dt.Rows[0]["DTE_LT_PROTECT"]);
                            objFieldEnum.sGrounding = Convert.ToString(dt.Rows[0]["DTE_GROUNDING"]);
                            objFieldEnum.sArresters = Convert.ToString(dt.Rows[0]["DTE_ARRESTERS"]);
                            objFieldEnum.sLoadtype = Convert.ToString(dt.Rows[0]["DTE_LOADTYPE"]);
                            objFieldEnum.sProjecttype = Convert.ToString(dt.Rows[0]["DTE_PROJECTTYPE"]);
                            objFieldEnum.sLTlinelength = Convert.ToString(dt.Rows[0]["DTE_LT_LINE"]);
                            objFieldEnum.sDepreciation = Convert.ToString(dt.Rows[0]["DTE_DEPRECIATION"]);
                            objFieldEnum.sLatitude = Convert.ToString(dt.Rows[0]["DTE_LATITUDE"]);
                            objFieldEnum.sLongitude = Convert.ToString(dt.Rows[0]["DTE_LONGITUDE"]);

                            objFieldEnum.sIsIPEnumDone = Convert.ToString(dt.Rows[0]["ED_IP_ENUM_DONE"]);

                        }
                        else if (objFieldEnum.sEnumType == "1" || objFieldEnum.sEnumType == "3" || objFieldEnum.sEnumType == "5")
                        {
                            objFieldEnum.sOfficeCode = Convert.ToString(dt.Rows[0]["ED_OFFICECODE"]);
                            objFieldEnum.sLocName = Convert.ToString(dt.Rows[0]["ED_LOCNAME"]);
                            objFieldEnum.sLocAddress = Convert.ToString(dt.Rows[0]["ED_LOCADDRESS"]).Trim().Replace("'", "");
                            objFieldEnum.sTCType = Convert.ToString(dt.Rows[0]["ED_LOCTYPE"]);
                        }

                        objFieldEnum.sCrBy = Convert.ToString(dt.Rows[0]["ED_CRBY"]);
                        //objFieldEnum.sEnumType = objFieldEnum.sEnumType;

                        objFieldEnum.sInfosysAsset = Convert.ToString(dt.Rows[0]["DTE_INFOSYS_ASSET"]);
                        objFieldEnum.sTCWeight = Convert.ToString(dt.Rows[0]["DTE_TC_WEIGHT"]);
                        objFieldEnum.sTankCapacity = Convert.ToString(dt.Rows[0]["DTE_TANK_CAPACITY"]);
                        objFieldEnum.sRating = Convert.ToString(dt.Rows[0]["DTE_RATING"]);
                        objFieldEnum.sStarRate = Convert.ToString(dt.Rows[0]["DTE_STAR_RATE"]);


                        Arr = ApproveQCEnumerationDetails(objFieldEnum);

                        if (Arr[1] == "2")
                        {
                            //ObjCon.RollBack();
                            return Arr;
                        }

                    }

                }

                DateTime endtime = DateTime.Now;
                strQry = "UPDATE \"TBLTIMELOG\" SET \"TL_END_TIME\"= TO_DATE('" + endtime + "','mm/dd/yyyy HH:MI:SSAM') WHERE \"TL_ID\"='" + Timeid + "'";
                ObjCon.ExecuteQry(strQry);


                //if (Arr[1] == "0")
                //{
                //    Arr[0] = "Enumeration Details Approved Successfully";
                //    Arr[1] = "1";
                //}

                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(ex.StackTrace, ex.Message, strFormCode, "GetEnumerationInfoForApprove");
                return Arr;
            }
        }

        #endregion

    }
}
