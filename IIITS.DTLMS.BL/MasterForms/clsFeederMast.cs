using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Data.SQLite;
using IIITS.PGSQL.DAL;
using Npgsql;

namespace IIITS.DTLMS.BL
{
    public class clsFeederMast
    {
        // CustOledbConnection ObjCon = new CustOledbConnection(Constants.Password);
        string strQry = string.Empty;
        public Int64 FeederID { get; set; }
        public string FeederCode { get; set; }
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public Int64 Stationid { get; set; }
        public Int64 BankId { get; set; }
        public Int64 BusId { get; set; }
        public string FeederName { get; set; }
        public string FeederType { get; set; }
        public string FeederCategory { get; set; }
        public string FeederInterflow { get; set; }
        public string FeederDCC { get; set; }
        public string UserLogged { get; set; }
        public bool IsSave { get; set; }
        public string MDMFeedercode  { get; set; }
        public string TotalDtc { get; set; }

        string strFormCode = "clsFeederMast";

        SQLiteConnection sql_con;
        NpgsqlCommand NpgsqlCommand;
        public string[] FeederMaster(clsFeederMast objFeederMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            string[] Arrmsg = new string[2];
            try
            {
                string[] strQryVallist = null;

                if (objFeederMaster.OfficeCode != "")
                {
                    strQryVallist = objFeederMaster.OfficeCode.Split(',');

                }

                if (objFeederMaster.IsSave)
                {
                    string StrOfficeCodeExist = string.Empty;

                    //foreach (string OfficeCode in strQryVallist)
                    //{
                    NpgsqlCommand.Parameters.AddWithValue("FeederCode", objFeederMaster.FeederCode);
                    string strFeederId = ObjCon.get_value("select \"FD_FEEDER_ID\" FROM \"TBLFEEDERMAST\" WHERE CAST(\"FD_FEEDER_CODE\" AS TEXT)=:FeederCode", NpgsqlCommand);

                    if (strFeederId == "")
                    {
                        strFeederId = "0";
                    }
                    NpgsqlCommand.Parameters.AddWithValue("FeederId", Convert.ToInt32(strFeederId));
                    NpgsqlCommand.Parameters.AddWithValue("OfficeCode", Convert.ToInt32(OfficeCode));
                    if (!ObjCon.get_value("SELECT count(*) FROM \"TBLFEEDEROFFCODE\" WHERE \"FDO_FEEDER_ID\" =:FeederId and \"FDO_OFFICE_CODE\"=:OfficeCode", NpgsqlCommand).ToString().Equals("0"))
                    {
                        StrOfficeCodeExist += OfficeCode + ",";
                    }
                    //}
                    if (StrOfficeCodeExist != "")
                    {
                        //Arrmsg[0] = "Feeder Code For  Office Code/s '" + StrOfficeCodeExist + "' Already Present";
                        Arrmsg[0] = "Feeder Code  Already Present";
                        Arrmsg[1] = "4";
                        return Arrmsg;
                    }

                    string strQry = string.Empty;

                    // ***************** remove comment if required *******************
                    //To insert to Database in the format ";1;2;3;"
                    //objFeederMaster.OfficeCode = objFeederMaster.OfficeCode.Replace(",", ";");

                    //if (!objFeederMaster.OfficeCode.ToString().StartsWith(";"))
                    //{
                    //    objFeederMaster.OfficeCode = ";" + objFeederMaster.OfficeCode;
                    //}

                    //if (!objFeederMaster.OfficeCode.ToString().EndsWith(";"))
                    //{
                    //    objFeederMaster.OfficeCode = objFeederMaster.OfficeCode + ";";
                    //}

                    long slno = ObjCon.Get_max_no("FD_FEEDER_ID", "TBLFEEDERMAST");
                    //strQry = "INSERT INTO TBLFEEDERMAST(FD_FEEDER_ID,FD_FEEDER_CODE,FD_FEEDER_NAME,";
                    //strQry += " FD_CREATED_AUTH,FD_BS_ID,FD_FC_ID,FD_IS_INTERFLOW,FD_DTC_CAPACITY,FD_ST_ID)";
                    //strQry += " VALUES('" + slno + "','" + objFeederMaster.FeederCode + "','" + objFeederMaster.FeederName.Replace("'", "''") + "',";
                    //strQry+= " '" + objFeederMaster.UserLogged + "','" + objFeederMaster.BusId + "','" + objFeederMaster.FeederCategory + "',";
                    //strQry+= " '" + objFeederMaster.FeederInterflow + "','" + objFeederMaster.FeederDCC + "','"+objFeederMaster.Stationid +"')";


                    NpgsqlCommand.Parameters.AddWithValue("slno", slno);
                    NpgsqlCommand.Parameters.AddWithValue("FeederCode1", objFeederMaster.FeederCode);
                    NpgsqlCommand.Parameters.AddWithValue("FeederName", objFeederMaster.FeederName.Replace("'", "''"));
                    NpgsqlCommand.Parameters.AddWithValue("UserLogged", Convert.ToInt32(objFeederMaster.UserLogged));
                    NpgsqlCommand.Parameters.AddWithValue("BusId", objFeederMaster.BusId);
                    NpgsqlCommand.Parameters.AddWithValue("FeederCategory", Convert.ToInt32(objFeederMaster.FeederCategory));
                    NpgsqlCommand.Parameters.AddWithValue("FeederInterflow",Convert.ToInt32(objFeederMaster.FeederInterflow));
                    NpgsqlCommand.Parameters.AddWithValue("FeederDCC", Convert.ToInt32(objFeederMaster.FeederDCC ));
                    NpgsqlCommand.Parameters.AddWithValue("Stationid", objFeederMaster.Stationid);
                    NpgsqlCommand.Parameters.AddWithValue("MDMFeedercode", objFeederMaster.MDMFeedercode);

                    strQry = "INSERT INTO \"TBLFEEDERMAST\"(\"FD_FEEDER_ID\",\"FD_FEEDER_CODE\",\"FD_FEEDER_NAME\",";
                    strQry += " \"FD_CREATED_AUTH\",\"FD_BS_ID\",\"FD_FC_ID\",\"FD_IS_INTERFLOW\",\"FD_DTC_CAPACITY\",\"FD_ST_ID\",\"FD_MDM_FEEDERCODE\")";
                    strQry += " VALUES(:slno,:FeederCode1,:FeederName,";
                    strQry += " :UserLogged,:BusId,:FeederCategory,";
                    strQry += " :FeederInterflow,:FeederDCC,:Stationid,:MDMFeedercode)";

                    ObjCon.ExecuteQry(strQry, NpgsqlCommand);

                    foreach (string officeCode in strQryVallist)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("Getmaxno", ObjCon.Get_max_no("FDO_ID", "TBLFEEDEROFFCODE"));
                        NpgsqlCommand.Parameters.AddWithValue("slno1",Convert.ToInt32(slno));
                        NpgsqlCommand.Parameters.AddWithValue("OfficeCode1",Convert.ToInt32(OfficeCode));
                        strQry = "Insert into \"TBLFEEDEROFFCODE\" (\"FDO_ID\",\"FDO_FEEDER_ID\",\"FDO_OFFICE_CODE\") VALUES ";
                        strQry += " (:Getmaxno,:slno1,:OfficeCode1)";
                        ObjCon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    Arrmsg[0] = "Feeder Information Saved Successfully ";
                    Arrmsg[1] = "0";
                    return Arrmsg;
                    // return ("Feeder Information Saved Successfully " + objFeederMaster.FeederCode);
                }
                else
                {
                    string StrOfficeCodeExist = string.Empty;

                    // ***************** remove comment if required *******************

                    //foreach (string OfficeCode in strQryVallist)
                    //{
                    //    if (!ObjCon.get_value("SELECT count(*) FROM TBLFEEDERMAST WHERE FD_FEEDER_CODE='" + objFeederMaster.FeederCode + "' and FD_OFF_CODE='" + OfficeCode + "' and FD_FEEDER_ID<>'" + objFeederMaster.FeederID + "'").ToString().Equals("0"))
                    //    {
                    //        StrOfficeCodeExist += OfficeCode + ",";
                    //    }
                    //}
                    //if (StrOfficeCodeExist != "")
                    //{
                    //    Arrmsg[0] = "Feeder Code For  Office Code/s '" + StrOfficeCodeExist + "' Already Present";
                    //    Arrmsg[1] = "4";
                    //    return Arrmsg;
                    //}


                    //To insert to Database in the format ";1;2;3;"
                    //objFeederMaster.OfficeCode = objFeederMaster.OfficeCode.Replace(",", ";");

                    //if (!objFeederMaster.OfficeCode.ToString().StartsWith(";"))
                    //{
                    //    objFeederMaster.OfficeCode = ";" + objFeederMaster.OfficeCode;
                    //}

                    //if (!objFeederMaster.OfficeCode.ToString().EndsWith(";"))
                    //{
                    //    objFeederMaster.OfficeCode = objFeederMaster.OfficeCode + ";";
                    //}

                    NpgsqlCommand.Parameters.AddWithValue("FeederCode2", objFeederMaster.FeederCode);
                    NpgsqlCommand.Parameters.AddWithValue("FeederCategory1", Convert.ToInt32(objFeederMaster.FeederCategory));
                    NpgsqlCommand.Parameters.AddWithValue("FeederName1", objFeederMaster.FeederName.Replace("'", "''"));
                    NpgsqlCommand.Parameters.AddWithValue("UserLogged1", Convert.ToInt32(objFeederMaster.UserLogged));
                    NpgsqlCommand.Parameters.AddWithValue("BusId1", objFeederMaster.BusId);
                    NpgsqlCommand.Parameters.AddWithValue("FeederDCC1", Convert.ToInt32(objFeederMaster.FeederDCC));
                    NpgsqlCommand.Parameters.AddWithValue("FeederInterflow1", Convert.ToInt32(objFeederMaster.FeederInterflow));
                    NpgsqlCommand.Parameters.AddWithValue("Stationid1",  objFeederMaster.Stationid);
                    NpgsqlCommand.Parameters.AddWithValue("MDMFeedercode1", objFeederMaster.MDMFeedercode);
                    NpgsqlCommand.Parameters.AddWithValue("FeederID1", objFeederMaster.FeederID);


                    strQry = "UPDATE \"TBLFEEDERMAST\" SET \"FD_FEEDER_CODE\"=:FeederCode2,";
                    strQry += " \"FD_CREATED_DATE\"=NOW() ,\"FD_FC_ID\"=:FeederCategory1,";
                    strQry += " \"FD_FEEDER_NAME\"=:FeederName1,";
                    strQry += " \"FD_CREATED_AUTH\"=:UserLogged1,\"FD_BS_ID\"=:BusId1,\"FD_DTC_CAPACITY\"=:FeederDCC1,";
                    strQry += " \"FD_IS_INTERFLOW\"=:FeederInterflow1,\"FD_ST_ID\"=:Stationid1,\"FD_MDM_FEEDERCODE\"=:MDMFeedercode1";
                    strQry += " where \"FD_FEEDER_ID\" =:FeederID1";
                    ObjCon.ExecuteQry(strQry, NpgsqlCommand);

                    NpgsqlCommand.Parameters.AddWithValue("FeederID2", Convert.ToInt32(objFeederMaster.FeederID));
                    strQry = " DELETE FROM \"TBLFEEDEROFFCODE\" WHERE \"FDO_FEEDER_ID\" = :FeederID2";
                    ObjCon.ExecuteQry(strQry, NpgsqlCommand);

                    foreach (string OfficeCode in strQryVallist)
                    {


                        NpgsqlCommand.Parameters.AddWithValue("Getmaxno1", ObjCon.Get_max_no("FDO_ID", "TBLFEEDEROFFCODE"));
                        NpgsqlCommand.Parameters.AddWithValue("FeederID3", Convert.ToInt32(objFeederMaster.FeederID));
                        NpgsqlCommand.Parameters.AddWithValue("OfficeCode2", Convert.ToInt32(OfficeCode));

                        //strQry = "Insert into \"TBLFEEDEROFFCODE\" (\"FDO_ID\",\"FDO_FEEDER_ID\",\"FDO_OFFICE_CODE\") VALUES ";
                        //strQry += " (:Getmaxno1,:FeederID3, :OfficeCode2)";
                        strQry = "Insert into \"TBLFEEDEROFFCODE\" (\"FDO_ID\",\"FDO_FEEDER_ID\",\"FDO_OFFICE_CODE\") VALUES ";
                        strQry += " ((select max(\"FDO_ID\")+1 from \"TBLFEEDEROFFCODE\"),'" + Convert.ToInt32(objFeederMaster.FeederID) + "','" + Convert.ToInt32(OfficeCode) + "')";
                        ObjCon.ExecuteQry(strQry, NpgsqlCommand);
                    }

                    Arrmsg[0] = "Feeder Information Updated Successfully ";
                    Arrmsg[1] = "0";
                    return Arrmsg;
                    //return ("Feeder Information Updated Successfully " + objFeederMaster.FeederCode);
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arrmsg;
            }
        }

        //public string FeederMast(string txtFeederCode, string cmbstationvalue, string txtName, string txtChangDate, string txtCTLowRange, string txtCTHighRange, string txtConstant, string txtLineVoltage, string cmbFeederTypevalue, string cmbSectionvalue, string txtCurLimit, string txtKWHReading, string cmbconscatvalue, string strUser)
        //{
        //    try
        //    {

        //        if (!objCon.get_value("SELECT count(*) FROM TBLFEEDERMAST WHERE FD_FEEDER_CODE='1111' and FD_OFFICE_CODE='1112').ToString().Equals("0"))
        //        {
        //            return ("Feeder Code Already Present");
        //        }
        //        OleDbDataReader dr = objCon.Fetch("SELECT * FROM TBLMUSSMAST WHERE MU_MUSS_CODE='" + txtFeederCode.Substring(0, 3) + "'");
        //        if (!dr.Read())
        //        {
        //            return ("Muss Code Not Found " + txtFeederCode.Substring(0, 2));
        //        }
        //        MussSlno = objCon.get_value("Select MU_SLNO from TBLMUSSMAST where MU_MUSS_CODE='" + cmbstationvalue.ToString() + "'");
        //        if (cmbconscatvalue == "10")
        //        {
        //            cmbconscatvalue = "0";
        //        }

        //        string mob = objCon.get_value("SELECT MU_MOBILE_NO FROM TBLMUSSMAST WHERE MU_MUSS_CODE='" + txtFeederCode.Substring(0, 3) + "'");
        //        string qry = "INSERT INTO TBLFEEDERMAST(FD_FEEDER_ID,FD_MUSS_CODE,FD_NAME,FD_CHANGE_DT,FD_CT_LOW_RANGE,FD_CT_HIGH_RANGE,FD_CONST,FD_LINE_VOLTAGE,FD_SLNO,FD_TYPE,FD_MOBILE_NO,FD_OFFICE_CODE,FD_CUR_LIMIT,FD_KWH_READING,FD_CONS_CAT) VALUES('";
        //        qry = qry + txtFeederCode + "','";	//FDR CODE
        //        qry = qry + MussSlno + "','";				//MUSS CODE
        //        qry = qry + txtName + "',";		//FDR NAME
        //        qry = qry + "TO_DATE('" + txtChangDate + "','MM/DD/YYYY'),'";	//CT CHGD DATE3
        //        qry = qry + txtCTLowRange + "','";	//CT LOW RANGE
        //        qry = qry + txtCTHighRange + "','"; //CT HIGH RANGE
        //        qry = qry + txtConstant + "','";	//CONST
        //        qry = qry + txtLineVoltage + "','";	//LINE VOLTAGE
        //        qry = qry + slno + "','";	//FDR SLNO
        //        qry = qry + cmbFeederTypevalue + "','";	//FDR TYPE
        //        qry = qry + mob + "','";	//MOBILE NO
        //        qry = qry + cmbSectionvalue + "','";	//SECTION CODE
        //        qry = qry + txtCurLimit + "','";	//CUR LIMIT
        //        qry = qry + txtKWHReading + "','";	//KWH READNG
        //        qry = qry + cmbconscatvalue + "')";	//CONS CAT
        //        objCon.Execute(qry);


        //        // long bfslno = objCon.Get_max_no("BF_SLNO", "TBLBNKFDRLINK");
        //        //if (cmbFeederTypevalue.Equals("5") || cmbFeederTypevalue.Equals("6"))
        //        //{
        //        //   // objCon.Execute("ALTER TRIGGER  CESCDTC.BNK_FDR_VALIDATE DISABLE");
        //        //    objCon.Execute("INSERT INTO TBLBNKFDRLINK(BF_SLNO,BF_BNK_SLNO,BF_FDR_SLNO,BF_LINK_DATE,BF_CANCEL_FLAG,BF_ENTRY_AUTH) VALUES('" + bfslno + "','" + slno + "','" + slno + "',sysdate,0,'" + strUser + "')");
        //        //  //  objCon.Execute("ALTER TRIGGER  CESCDTC.BNK_FDR_VALIDATE ENABLE");
        //        //}
        //        //else if (!cmbIFPointvalue.Equals(string.Empty))
        //        //{
        //        //    objCon.Execute("INSERT INTO TBLBNKFDRLINK(BF_SLNO,BF_BNK_SLNO,BF_FDR_SLNO,BF_LINK_DATE,BF_CANCEL_FLAG,BF_ENTRY_AUTH) VALUES('" + bfslno + "','" + cmbIFPointvalue + "','" + slno + "',sysdate,0,'" + strUser + "')");
        //        //}
        //        return ("Feeder Information Saved Successfully " + txtFeederCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error:" + ex.Message;
        //    }


        //}


        //public string UpdateFeederDetails(string FdSlno, string txtFeederCode, string cmbstationvalue, string txtName, string txtChangDate, string txtCTLowRange, string txtCTHighRange, string txtConstant, string txtLineVoltage, string cmbFeederTypevalue, string cmbSectionvalue, string txtCurLimit, string txtKWHReading, string cmbconscatvalue, string strUser)
        //{
        //    try
        //    {
        //        OleDbDataReader dr;
        //        dr = objCon.Fetch("Select * from TBLFEEDERMAST where FD_SLNO='" + FdSlno + "'");
        //        if (!dr.Read())
        //        {
        //            return ("Feeder Does Not Exists");
        //        }
        //        dr.Close();

        //        dr = objCon.Fetch("SELECT * FROM TBLMUSSMAST WHERE MU_MUSS_CODE='" + txtFeederCode.Substring(0, 3) + "'");
        //        if (!dr.Read())
        //        {
        //            return ("Muss Code Not Found " + txtFeederCode.Substring(0, 2));
        //        }

        //        MussSlno = objCon.get_value("Select MU_SLNO from TBLMUSSMAST where MU_MUSS_CODE='" + cmbstationvalue.ToString() + "'");
        //        if (cmbconscatvalue == "10")
        //        {
        //            cmbconscatvalue = "0";
        //        }
        //        string mob = objCon.get_value("SELECT MU_MOBILE_NO FROM TBLMUSSMAST WHERE MU_MUSS_CODE='" + txtFeederCode.Substring(0, 3) + "'");
        //        string qry = "Update TBLFEEDERMAST set FD_FEEDER_ID='" + txtFeederCode + "',";
        //        qry += "FD_MUSS_CODE='" + MussSlno + "',";
        //        qry += "FD_NAME='" + txtName.Trim().ToUpper() + "',";
        //        if (txtChangDate.Trim().Length > 0)
        //        {
        //            qry += "FD_CHANGE_DT=TO_DATE('" + txtChangDate + "','MM/DD/YYYY'),";
        //        }
        //        qry += "FD_CT_LOW_RANGE='" + txtCTLowRange + "',";
        //        qry += "FD_CT_HIGH_RANGE='" + txtCTHighRange + "',";
        //        qry += "FD_CONST='" + txtConstant + "',";
        //        qry += "FD_LINE_VOLTAGE='" + txtLineVoltage + "',";
        //        qry += "FD_TYPE='" + cmbFeederTypevalue + "',";
        //        qry += "FD_MOBILE_NO='" + mob + "',";
        //        qry += "FD_OFFICE_CODE='" + cmbSectionvalue + "',";
        //        qry += "FD_CUR_LIMIT='" + txtCurLimit + "',";
        //        qry += "FD_KWH_READING='" + txtKWHReading + "',";
        //        qry += "FD_CONS_CAT='" + cmbconscatvalue + "' where FD_SLNO='" + FdSlno + "'";
        //        objCon.Execute(qry);
        //        return ("Feeder Information Updated Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error:" + ex.Message;
        //    }
        //}


        public DataTable LoadOfficeDet(clsFeederMast objStation)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtStationDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
               
                strQry = "select \"OFF_CODE\",\"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE  \"OFF_NAME\" IS NOT NULL AND LENGTH(cast(\"OFF_CODE\" as text))='4'";
                if (objStation.OfficeCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("officecode1",  objStation.OfficeCode );
                    strQry += " AND CAST(\"OFF_CODE\" AS TEXT) LIKE '%"+ objStation.OfficeCode + "%'";
                }
                if (objStation.OfficeName != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("officename1", objStation.OfficeName.ToUpper());
                    strQry += " AND UPPER(CAST(\"OFF_NAME\" AS TEXT)) LIKE '%"+ objStation.OfficeName.ToUpper() + "%'";
                }
                strQry += " order by \"OFF_CODE\"";
                DtStationDet = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
               
                return DtStationDet;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtStationDet;
            }
        }
        public DataTable LoadOfficeDetails(clsFeederMast objStation)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtStationDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;

                strQry = "select \"OFF_CODE\",\"OFF_NAME\" FROM \"VIEW_ALL_OFFICES\" WHERE  \"OFF_NAME\" IS NOT NULL  ";
                if (objStation.OfficeCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("officecode1", objStation.OfficeCode);
                   // strQry += " AND CAST(\"OFF_CODE\" AS TEXT) LIKE :officecode1||'%'";
                    strQry += " AND CAST(\"OFF_CODE\" AS TEXT) LIKE '%" + objStation.OfficeCode + "%'";
                }
                if (objStation.OfficeName != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("officename1", objStation.OfficeName.ToUpper());
                 //   strQry += " AND UPPER(CAST(\"OFF_NAME\" AS TEXT)) LIKE :officename1||'%'";
                    strQry += " AND UPPER(CAST(\"OFF_NAME\" AS TEXT)) LIKE '%" + objStation.OfficeName.ToUpper() + "%'";
                }
                strQry += " order by \"OFF_CODE\"";
                DtStationDet = ObjCon.FetchDataTable(strQry, NpgsqlCommand);

                return DtStationDet;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtStationDet;
            }
        }

        public DataTable LoadFeederMastDet(string strFeederID = "")
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtFeederfDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {

                strQry = string.Empty;


                //strQry = " SELECT FD_FEEDER_NAME,FD_FEEDER_CODE ,FD_FEEDER_ID,ST_NAME,BS_NAME,FC_NAME,LISTAGG(OFFNAME,',') WITHIN GROUP (ORDER BY OFFNAME) OFFNAME FROM";
                //strQry += " (SELECT FD_FEEDER_NAME,FD_FEEDER_CODE ,FD_FEEDER_ID,(SELECT ST_NAME FROM TBLSTATION WHERE FD_ST_ID=ST_ID ) ST_NAME,";
                //strQry += " (SELECT BS_NAME FROM TBLBUS WHERE FD_BS_ID=BS_ID) BS_NAME,(SELECT FC_NAME FROM TBLFEEDERCATEGORY WHERE  FD_FC_ID=FC_ID) FC_NAME,";
                //strQry += " (SELECT OFF_NAME from VIEW_ALL_OFFICES WHERE OFF_CODE=FDO_OFFICE_CODE ) AS OFFNAME";
                //strQry += " from TBLFEEDERMAST ,TBLFEEDEROFFCODE WHERE FD_FEEDER_ID=FDO_FEEDER_ID ";
                //if (strFeederID != "")
                //{
                //    strQry += " AND FD_FEEDER_ID='" + strFeederID + "'";
                //}
                //strQry += " ) GROUP BY FD_FEEDER_NAME,FD_FEEDER_CODE ,FD_FEEDER_ID,ST_NAME,BS_NAME,FC_NAME";

                strQry = " SELECT \"FD_FEEDER_NAME\",\"FD_FEEDER_CODE \",\"FD_FEEDER_ID\",\"ST_NAME\",\"BS_NAME\",\"FC_NAME\",LISTAGG(\"OFFNAME\",',') WITHIN GROUP (ORDER BY \"OFFNAME\") OFFNAME FROM";
                strQry += " (SELECT \"FD_FEEDER_NAME\",\"FD_FEEDER_CODE \",\"FD_FEEDER_ID\",(SELECT \"ST_NAME\" FROM \"TBLSTATION\" WHERE CAST(\"FD_ST_ID\" AS TEXT)=CAST(\"ST_ID\" AS TEXT) ) ST_NAME,";
                strQry += " (SELECT \"BS_NAME\" FROM \"TBLBUS\" WHERE \"FD_BS_ID\"=\"BS_ID\") BS_NAME,(SELECT \"FC_NAME\" FROM \"TBLFEEDERCATEGORY\" WHERE  CAST(\"FD_FC_ID\" AS TEXT)=CAST(\"FC_ID\" AS TEXT))  FC_NAME,";
                strQry += " (SELECT \"OFF_NAME\" from \"VIEW_ALL_OFFICES\" WHERE CAST(\"OFF_CODE\" AS TEXT)=CAST(\"FDO_OFFICE_CODE\" AS TEXT) ) AS OFFNAME";
                strQry += " from \"TBLFEEDERMAST \",\"TBLFEEDEROFFCODE\" WHERE CAST(\"FD_FEEDER_ID\" AS TEXT)= CAST(\"FDO_FEEDER_ID\" AS TEXT) ";
                if (strFeederID != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("feederid12", strFeederID);
                    strQry += " AND CAST(\"FD_FEEDER_ID\" AS TEXT)=:feederid12";
                }
                strQry += " ) GROUP BY \"FD_FEEDER_NAME\",\"FD_FEEDER_CODE \",\"FD_FEEDER_ID\",\"ST_NAME\",\"BS_NAME\",\"FC_NAME\"";

                 DtFeederfDet = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
               

                return DtFeederfDet;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtFeederfDet;
            }
        }

        public DataTable GetFeederDetails(string strFeederID)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtFeederfDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {

                strQry = string.Empty;

                //strQry = " SELECT FD_FEEDER_NAME, FD_DTC_CAPACITY, FD_IS_INTERFLOW, TO_CHAR(FD_FEEDER_CODE) FD_FEEDER_CODE,FD_FEEDER_ID,ST_ID,BS_ID,FC_ID,BN_ID,FC_FT_ID, LISTAGG( ";
                //strQry += "  FDO_OFFICE_CODE,',')   WITHIN GROUP (ORDER BY FDO_OFFICE_CODE) OFFCODE FROM  (SELECT FD_FEEDER_NAME,TO_CHAR(FD_FEEDER_CODE)FD_FEEDER_CODE, FD_DTC_CAPACITY,  ";
                //strQry += " FD_IS_INTERFLOW ,   FDO_OFFICE_CODE,FD_FEEDER_ID,ST_ID,BS_ID,FC_ID,BN_ID,FC_FT_ID from  TBLFEEDERMAST,TBLBUS,TBLFEEDERCATEGORY,TBLSTATION,TBLBANK, ";
                //strQry += " TBLFEEDEROFFCODE,TBLFDRTYPE  WHERE FD_FEEDER_ID = FDO_FEEDER_ID  AND  FD_ST_ID=ST_ID AND FD_BS_ID=BS_ID  AND FD_FC_ID=FC_ID AND BN_ID=BS_BN_ID AND FC_FT_ID=FT_ID  ";

                // strQry = " SELECT FD_FEEDER_NAME, FD_DTC_CAPACITY, FD_IS_INTERFLOW, TO_CHAR(FD_FEEDER_CODE) FD_FEEDER_CODE,FD_FEEDER_ID,ST_ID,";
                // strQry += " BS_ID,FC_ID,FC_FT_ID,BN_ID, LISTAGG(   FDO_OFFICE_CODE,',')   WITHIN GROUP (ORDER BY FDO_OFFICE_CODE) OFFCODE FROM  ";
                // strQry += " (SELECT FD_FEEDER_NAME,TO_CHAR(FD_FEEDER_CODE)FD_FEEDER_CODE, FD_DTC_CAPACITY,   FD_IS_INTERFLOW ,   FDO_OFFICE_CODE,";
                // strQry += " FD_FEEDER_ID,FD_ST_ID AS ST_ID,FD_BS_ID AS BS_ID,FD_FC_ID AS FC_ID,(SELECT DISTINCT FT_ID FROM TBLFDRTYPE,";
                // strQry += " TBLFEEDERCATEGORY WHERE FC_FT_ID=FT_ID AND FD_FC_ID=FC_ID) FC_FT_ID,(SELECT DISTINCT BN_ID FROM TBLBANK,TBLBUS WHERE BS_BN_ID=BN_ID AND BS_ID=FD_BS_ID) BN_ID ";
                // strQry += " from  TBLFEEDERMAST, TBLFEEDEROFFCODE  WHERE FD_FEEDER_ID = FDO_FEEDER_ID";

                //if( strFeederID.Length > 0)
                //{
                //    strQry += " AND FD_FEEDER_ID='" + strFeederID + "'";
                //}

                //strQry += " ) GROUP BY FD_FEEDER_NAME, FD_FEEDER_CODE,FD_FEEDER_ID,ST_ID,BS_ID,FC_ID,BN_ID,FC_FT_ID,FD_DTC_CAPACITY, FD_IS_INTERFLOW";

                strQry = " SELECT \"FD_MDM_FEEDERCODE\",\"FD_FEEDER_NAME\",\"FD_DTC_CAPACITY\",\"FD_IS_INTERFLOW\",\"FD_FEEDER_CODE\" AS \"FD_FEEDER_CODE\",\"FD_FEEDER_ID\",\"ST_ID\",";
                strQry += " \"BS_ID\",\"FC_ID\",\"FC_FT_ID\",\"BN_ID\",array_to_string(array(select \"FDO_OFFICE_CODE\" ), ', ') \"OFFCODE\",\"DIST_CODE\",\"TALUQ_CODE\",\"STATION_CODE\" FROM  ";
                strQry += " (SELECT \"FD_MDM_FEEDERCODE\",\"FD_FEEDER_NAME\",\"FD_FEEDER_CODE\" AS \"FD_FEEDER_CODE\",\"FD_DTC_CAPACITY\",\"FD_IS_INTERFLOW\",\"FDO_OFFICE_CODE\",";
                strQry += " \"FD_FEEDER_ID\",\"FD_ST_ID\" AS \"ST_ID\",\"FD_BS_ID\" AS \"BS_ID\",\"FD_FC_ID\" AS \"FC_ID\",(SELECT DISTINCT \"FT_ID\" FROM \"TBLFDRTYPE\",";
                strQry += " \"TBLFEEDERCATEGORY\" WHERE CAST(\"FC_FT_ID\" AS TEXT)=CAST(\"FT_ID\" AS TEXT) AND CAST(\"FD_FC_ID\" AS TEXT)=CAST(\"FC_ID\" AS TEXT)) \"FC_FT_ID\", ";
                strQry += " (SELECT DISTINCT \"BN_ID\" FROM \"TBLBANK\",\"TBLBUS\" WHERE CAST(\"BS_BN_ID\" AS TEXT)=CAST(\"BN_ID\" AS TEXT) AND ";
                strQry += " CAST(\"BS_ID\" AS TEXT)=CAST(\"FD_BS_ID\" AS TEXT)) \"BN_ID\", (SELECT \"DT_CODE\" FROM \"TBLDIST\" WHERE ";
                strQry += " cast(\"DT_CODE\" as text)=substr(cast(\"FD_FEEDER_CODE\" as text),1,1))\"DIST_CODE\",(SELECT DISTINCT \"TQ_CODE\" FROM \"TBLTALQ\" WHERE ";
                strQry += " cast(\"TQ_CODE\" as text)=substr(cast(\"FD_FEEDER_CODE\" as text),1,2))\"TALUQ_CODE\", (SELECT \"ST_ID\" FROM \"TBLSTATION\" WHERE \"ST_STATION_CODE\" = substr(cast(\"FD_FEEDER_CODE\" as text),1,4) )\"STATION_CODE\" ";
                strQry += " from  \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\"  WHERE CAST(\"FD_FEEDER_ID\" AS TEXT) = CAST(\"FDO_FEEDER_ID\" AS TEXT)";

                if (strFeederID.Length > 0)
                {
                    NpgsqlCommand.Parameters.AddWithValue("feederid", strFeederID);
                    strQry += " AND CAST(\"FD_FEEDER_ID\" AS TEXT)=:feederid";
                }

                strQry += " ) A ";


                DtFeederfDet = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                

                return DtFeederfDet;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtFeederfDet;
            }
        }

        public DataTable GetDTCDetails(string strFeederID)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtFeederfDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("feederid",Convert.ToInt32( strFeederID));
                string strQrycode = ObjCon.get_value("SELECT \"FD_FEEDER_CODE\" from \"TBLFEEDERMAST\" where \"FD_FEEDER_ID\"=:feederid", NpgsqlCommand);
                string strQryvalue = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("querycode", strQrycode);
                strQryvalue = "SELECT count(*) as TOTALDTC from \"TBLDTCMAST\" WHERE \"DT_FDRSLNO\"=:querycode";


                 DtFeederfDet = ObjCon.FetchDataTable(strQryvalue, NpgsqlCommand);
                

                return DtFeederfDet;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtFeederfDet;
            }
        }

        public DataTable LoadViewAllOffices(string strOfficeCode = "", string strOfficeName = "")
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            DataTable DtOfficeDet = new DataTable();
            NpgsqlCommand = new NpgsqlCommand();
            try
            {

                strQry = string.Empty;


                strQry = "select \"OFF_CODE\" ,\"OFF_NAME\"  FROM \"VIEW_ALL_OFFICES\"   ";
                //if (strOfficeCode != "" && strOfficeName != "")
                //{
                NpgsqlCommand.Parameters.AddWithValue("offcode", strOfficeCode);
                NpgsqlCommand.Parameters.AddWithValue("offname", strOfficeName.ToUpper());
                strQry += " where (CAST(\"OFF_CODE\" AS TEXT) like  :offcode||'%' and UPPER(CAST(\"OFF_NAME\" AS TEXT)) like  :offname||'%') ";
                //}

                //else if(strOfficeCode != ""  && strOfficeName == "") 
                //{
                //    strQry += " where (OFF_CODE like  '%" + strOfficeCode + "%') ";
                //}

                //else if (strOfficeCode == "" && strOfficeName != "")
                //{
                //    strQry += " where (UPPER(OFF_NAME) like  '%" + strOfficeName.ToUpper() + "%') ";
                //}
                strQry += " order by \"OFF_NAME\"";

                 DtOfficeDet = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
              

                return DtOfficeDet;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return DtOfficeDet;
            }
        }

        public void SetSqlLiteConnection(string sDbName)
        {
            String Exists = "FALSE";
            try
            {

                sDbName = sDbName.Substring(0, 4);
                //SQLiteConnection sql_con;
                string sURL = Convert.ToString(ConfigurationSettings.AppSettings["SQLLiteDB"]);
                string relative_path = sURL + sDbName + ".db";


                if (System.IO.File.Exists(relative_path))
                {
                    Exists = "TRUE";
                }


                sql_con = new SQLiteConnection
                ("Data Source=" + relative_path + ";Version=3;");
                sql_con.Open();
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ExecuteSqliteQuery(string Query, string sDbName)
        {
            String Exists = "INVOKE";
            try
            {
                SQLiteCommand sql_cmd;
                SetSqlLiteConnection(sDbName);
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = Query;
                sql_cmd.ExecuteNonQuery();
                sql_con.Close();
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public bool SyncFeederDetailstoApp(clsFeederMast objFeeder)
        {
            NpgsqlCommand = new NpgsqlCommand();
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            try
            {
                string[] strQryVallist = null;
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                if (objFeeder.OfficeCode != "")
                {
                    strQryVallist = objFeeder.OfficeCode.Split(',');

                    foreach (string OfficeCode in strQryVallist)
                    {
                        NpgsqlCommand.Parameters.AddWithValue("offcode", OfficeCode);
                        strQry = "SELECT \"OM_CODE\" FROM \"TBLOMSECMAST\" WHERE CAST(\"OM_SUBDIV_CODE\" AS TEXT)=:offcode";
                        dt = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        //{
                        string sSection;
                        if (dt.Rows.Count>0)
                        {
                         sSection = Convert.ToString(dt.Rows[0]["OM_CODE"]);
                        }
                        else{
                             sSection = "";
                    }
                        //SetSqlLiteConnection(sSection);
                        //SyncFeederDetails(OfficeCode, objFeeder.FeederCode, sSection);

                        //}

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public void SyncFeederDetails(string sOfficeCode, string sFeederCode, string sSection)
        {
            //CustOledbConnection ObjCon = new CustOledbConnection(Constants.Password);
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                DataTable ds = new DataTable();

                strQry = "DELETE FROM TBLFEEDERDETAILS WHERE FD_FEEDER_CODE='" + sFeederCode + "'";
                ExecuteSqliteQuery(strQry, sSection);

                NpgsqlCommand.Parameters.AddWithValue("fdcode", sFeederCode);
                NpgsqlCommand.Parameters.AddWithValue("offcode", sOfficeCode);
                strQry = "SELECT \"FD_FEEDER_CODE\",\"FD_FEEDER_NAME\",\"FDO_OFFICE_CODE\" FROM \"TBLFEEDERMAST\", \"TBLFEEDEROFFCODE\" WHERE ";
                strQry += " \"FD_FEEDER_ID\" = \"FDO_FEEDER_ID\" AND \"FD_FEEDER_CODE\"=:fdcode AND (cast(\"FDO_OFFICE_CODE\" as TEXT) LIKE :offcode||'%' OR  LENGTH( CAST (\"FDO_OFFICE_CODE\" AS TEXT)) = 0)";

                ds = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                // arrFinal.Add(OperateDBName + " -  " + OperateDivisionCode + "-FEEDER-" + DS.Tables[0].Rows.Count.ToString());
                if (ds.Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        strQry = "INSERT INTO TBLFEEDERDETAILS (FD_FEEDER_CODE, FD_FEEDER_NAME,FD_OFFICE_CODE) VALUES ";
                        strQry += " ('" + Convert.ToString(ds.Rows[i][0]).Replace("'", "") + "','" + Convert.ToString(ds.Rows[i][1]).Replace("'", "") + "',";
                        strQry += " '" + Convert.ToString(ds.Rows[i][2]).Replace("'", "") + "')";
                        ExecuteSqliteQuery(strQry, sSection);
                    }
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }


        }


        public string AutoGenerateFeederCode(clsFeederMast objFeeder)
        {
            string strQry = string.Empty;
            string stationcode = string.Empty;
            string feedercode = string.Empty;
            int twodigitFdcode;

            DataTable dt = new DataTable() ;
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("stid", objFeeder.Stationid);
                strQry = "SELECT \"ST_STATION_CODE\" FROM \"TBLSTATION\" WHERE \"ST_ID\" = :stid ";
                stationcode = ObjCon.get_value(strQry, NpgsqlCommand);

                NpgsqlCommand.Parameters.AddWithValue("stcd", stationcode);
                strQry = "SELECT \"FD_FEEDER_CODE\" FROM \"TBLFEEDERMAST\" WHERE SUBSTR(\"FD_FEEDER_CODE\",1,4) = :stcd ORDER BY \"FD_FEEDER_CODE\" DESC";
                dt = ObjCon.FetchDataTable(strQry, NpgsqlCommand);

                if (dt.Rows.Count > 0)
                {
                    feedercode = dt.Rows[0]["FD_FEEDER_CODE"].ToString();
                }
                else
                {
                    feedercode = stationcode + "01";
                    return feedercode;
                }
                if (feedercode.Length > 4)
                {
                    twodigitFdcode = Convert.ToInt16(feedercode.Substring(4, 2));
                    twodigitFdcode = twodigitFdcode + 1;
                    if (twodigitFdcode >= 100)
                    {
                        return "MAX FEEDER CODE HAS REACHED FOR STATION";
                    }

                    if (twodigitFdcode.ToString().Length == 1)
                    {
                        feedercode = feedercode.Substring(0, 4) +"0"+ twodigitFdcode;
                    }
                    else
                    {
                        feedercode = feedercode.Substring(0, 4) + twodigitFdcode;
                    }
                }
                return feedercode;


                //strQry = "SELECT COALESCE(MAX(cast(\"FD_FEEDER_CODE\" as INTEGER)),0)+1 AS \"FEEDERCODE\" FROM \"TBLFEEDERMAST\" ,";
                //strQry += " \"TBLFEEDEROFFCODE\" WHERE \"FD_FEEDER_ID\" = \"FDO_FEEDER_ID\" and cast(\"FDO_OFFICE_CODE\" as text) = '" + objFeeder.OfficeCode + "'";
                //return ObjCon.get_value(strQry);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return null;
            }
        }

        public string sGetFeederCount(string sSubdivCode)
        {
            PGSqlConnection ObjCon = new PGSqlConnection(Convert.ToString(ConfigurationSettings.AppSettings["pgSQLPassword"]));
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("subdivcd",Convert.ToInt32(sSubdivCode));
                string sSQry = "SELECT count(*) FROM \"TBLFEEDERMAST\",\"TBLFEEDEROFFCODE\" WHERE \"FD_FEEDER_ID\"=\"FDO_FEEDER_ID\" and \"FDO_OFFICE_CODE\"=:subdivcd";
                return ObjCon.get_value(sSQry, NpgsqlCommand);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }
    }
}
