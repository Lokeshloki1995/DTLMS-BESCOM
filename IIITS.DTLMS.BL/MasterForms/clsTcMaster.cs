using System;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Data;
using IIITS.PGSQL.DAL;
using Npgsql;
using NpgsqlTypes;

namespace IIITS.DTLMS.BL
{
    public class clsTcMaster
    {
        string strFormCode = "clsTcMaster";
        string sWarranty = string.Empty;
        PGSqlConnection ObjCon = new PGSqlConnection(Constants.Password);
        public DataTable dtTable { get; set; }
        public string sTcId { get; set; }
        public string sTcMakeId { get; set; }
        public string sTcSlNo { get; set; }
        public string sTcCapacity { get; set; }
        public string sTcLifeSpan { get; set; }
        public string sManufacDate { get; set; }
        public string sAllotementDate { get; set; }
        public string sAltNo { get; set; }
        public string sDINo { get; set; }
        public string sPoNo { get; set; }
        public string sPrice { get; set; }
        public string sSupplierId { get; set; }
        public string sWarrentyPeriod { get; set; }
        public string sLastServiceDate { get; set; }
        public string sQuantity { get; set; }
        public string sAltId { get; set; }

        public string sTcCode { get; set; }
        public string sTcLiveFlag { get; set; }
        public int sStatus { get; set; }
        public string sCurrentLocation { get; set; }
        public string sDivId { get; set; }
        public string sLocationId { get; set; }
        public string sLastRepairerId { get; set; }
        public string sUpdatedEvent { get; set; }
        public string sUpdateEventId { get; set; }
        public string sCrBy { get; set; }
        public string sOfficeCode { get; set; }
        public string sStoreId { get; set; }

        public string sRating { get; set; }
        public string sStarRate { get; set; }
        public string sOilCapacity { get; set; }
        public string sOilType { get; set; }
        public string sWeight { get; set; }

        public string srepairOffCode { get; set; }
        public string sDtcCodes { get; set; }



        public string sColumnNames { get; set; }
        public string sColumnValues { get; set; }
        public string sTableNames { get; set; }

        public string sQryValues { get; set; }
        public string sDescription { get; set; }
        public string sParameterValues { get; set; }

        public string sWFDataId { get; set; }
        public string sXmlData { get; set; }
        public string sBOId { get; set; }
        public string sFormName { get; set; }

        public string sClientIP { get; set; }
        public string sLastFaildate { get; set; }
        public string sLastFailuretype { get; set; }
        public string sLastRepaircost { get; set; }
        public string sLastRepaircount { get; set; }
        public string sConditiontc { get; set; }
        public string sCooling { get; set; }
        public string sType { get; set; }
        public string sCore { get; set; }
        public string sTapeCharger { get; set; }
        public string sDepreciation { get; set; }
        public string sInsurance { get; set; }
        public string sOriginalCost { get; set; }
        public string sFailCount { get; set; }
        public string sComponentId { get; set; }
        public string sInfosysId { get; set; }
        public string sDTrImagePath { get; set; }
        public string sroletype { get; set; }
        public string sNamePlateImagePath { get; set; }
        public string sDTRcommissionYear { get; set; }


        NpgsqlCommand NpgsqlCommand;
        public string[] SaveUpdateTransformerDetails(clsTcMaster objTcMaster)
        {
            string[] Arr = new string[2];
            string strQry = string.Empty;
           
            try
            {
                if (objTcMaster.sAllotementDate != null && objTcMaster.sAllotementDate != "")
                {
                    DateTime dPurchaseDate = DateTime.ParseExact(objTcMaster.sAllotementDate, "dd/MM/yyyy", null);

                    if (objTcMaster.sWarrentyPeriod != "")
                    {
                        sWarranty = Convert.ToString(dPurchaseDate.AddYears(Convert.ToInt32(objTcMaster.sWarrentyPeriod)));
                        sWarranty = Convert.ToDateTime(sWarranty).ToString("dd/MM/yyyy");
                    }
                }

                //Get Store Id
                if (objTcMaster.sLocationId != "")
                    if (objTcMaster.sroletype=="2")
                    {
                        sStoreId = objTcMaster.sLocationId;
                    }
                    else{
                    sStoreId = GetStoreId(objTcMaster.sLocationId);
                        if (sStoreId == "")
                        {
                            sStoreId = "0";
                        }
                    }
                else
                    sStoreId = "0";
                
                NpgsqlCommand cmd = new NpgsqlCommand("proc_saveupdatetransfdetails");
                cmd.Parameters.AddWithValue("tc_id", objTcMaster.sTcId);
                cmd.Parameters.AddWithValue("tc_code", objTcMaster.sTcCode);
                cmd.Parameters.AddWithValue("tc_serialno", objTcMaster.sTcSlNo);
                cmd.Parameters.AddWithValue("tc_makeid", objTcMaster.sTcMakeId==""?"0":objTcMaster.sTcMakeId);
                cmd.Parameters.AddWithValue("tc_capacity", objTcMaster.sTcCapacity);
                cmd.Parameters.AddWithValue("tc_manf_date", objTcMaster.sManufacDate);
                cmd.Parameters.AddWithValue("tc_purchase_date", objTcMaster.sAllotementDate);
                cmd.Parameters.AddWithValue("tc_supplier_id", objTcMaster.sSupplierId);
                cmd.Parameters.AddWithValue("tc_po_no", objTcMaster.sPoNo);
                cmd.Parameters.AddWithValue("tc_price", objTcMaster.sPrice);
                cmd.Parameters.AddWithValue("tc_warrantyperiod", objTcMaster.sWarranty==""?"0":objTcMaster.sWarranty);
                cmd.Parameters.AddWithValue("tc_life_span", objTcMaster.sTcLifeSpan);
                cmd.Parameters.AddWithValue("tc_last_servicedate", objTcMaster.sLastServiceDate);
                cmd.Parameters.AddWithValue("tc_curr_loc", objTcMaster.sCurrentLocation);
                cmd.Parameters.AddWithValue("tc_crby", objTcMaster.sCrBy);
                cmd.Parameters.AddWithValue("tc_warranty", objTcMaster.sWarrentyPeriod == "" ? "0" : objTcMaster.sWarrentyPeriod);
                cmd.Parameters.AddWithValue("tc_store_id", objTcMaster.sStoreId);
                cmd.Parameters.AddWithValue("tc_loc_id", objTcMaster.sLocationId);
                cmd.Parameters.AddWithValue("tc_rating", objTcMaster.sRating);
                cmd.Parameters.AddWithValue("tc_star_rate", objTcMaster.sStarRate);
                cmd.Parameters.AddWithValue("tc_oil_capacity", objTcMaster.sOilCapacity);
                cmd.Parameters.AddWithValue("tc_weight", objTcMaster.sWeight);
                cmd.Parameters.AddWithValue("tc_condition", objTcMaster.sConditiontc);
                cmd.Parameters.AddWithValue("tc_cooling", objTcMaster.sCooling);
                cmd.Parameters.AddWithValue("tc_core", objTcMaster.sCore);
                cmd.Parameters.AddWithValue("tc_type", objTcMaster.sType);
                cmd.Parameters.AddWithValue("tc_tap_charger", objTcMaster.sTapeCharger);

                cmd.Parameters.AddWithValue("tc_depreciation", objTcMaster.sDepreciation);
                cmd.Parameters.AddWithValue("tc_insurance", objTcMaster.sInsurance);
                cmd.Parameters.AddWithValue("tc_original_cost", objTcMaster.sOriginalCost);
                cmd.Parameters.AddWithValue("tc_component_id", objTcMaster.sComponentId);
                cmd.Parameters.AddWithValue("tc_asset_id", objTcMaster.sInfosysId);
                cmd.Parameters.AddWithValue("tc_oil_type", objTcMaster.sOilType);  

                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);

                cmd.Parameters["msg"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;

                Arr[0] = "msg";
                Arr[1] = "op_id";
                
                Arr = ObjCon.Execute(cmd, Arr, 2);
                return Arr;               
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;              
            }
        }

        public string[] GetTcAndSrlNumDetails(clsTcMaster objDtrCComm)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            string strQry = string.Empty;
            string sQryVal = string.Empty;
            try
            {
                Arr[1] = "0";
                NpgsqlCommand.Parameters.AddWithValue("tccd", objDtrCComm.sTcCode.ToUpper() );
                sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_CODE\" as text)=:tccd", NpgsqlCommand);
                if (sQryVal != "")
                {
                    Arr[2] = objDtrCComm.sTcCode;
                    Arr[1] = "2";
                    Arr[0] = "Transformer Code " + objDtrCComm.sTcCode + "  Already Exist";
                    return Arr;
                }
                NpgsqlCommand.Parameters.AddWithValue("slno1", objDtrCComm.sTcSlNo.ToUpper());
                sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_SLNO\" as text)=:slno1", NpgsqlCommand);
                if (sQryVal != "")
                {
                    Arr[2] = objDtrCComm.sTcSlNo;
                    Arr[1] = "2";
                    Arr[0] = "Transformer SlNo  " + objDtrCComm.sTcSlNo + "  Already Exist";
                    return Arr;
                }
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public string[] SaveTCDetails(string[] sTcDetails, clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string[] Arr = new string[3];
            string strQry = string.Empty;
            bool bResult = false;
            string sQryVal = string.Empty;
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("supId", objTcMaster.sSupplierId);
                sQryVal = ObjCon.get_value("select \"TS_ID\" from \"TBLTRANSSUPPLIER\"  WHERE \"TS_STATUS\"='A' AND  CAST(\"TS_ID\" as TEXT)=:supId", NpgsqlCommand);
                if (sQryVal == "" || sQryVal == null)
                {
                    Arr[0] = objTcMaster.sSupplierId;
                    Arr[1] = "2";
                    Arr[2] = "Enter a Valid Supplier ID";
                    return Arr;
                }
                NpgsqlCommand.Parameters.AddWithValue("altno", objTcMaster.sAltNo);
                sQryVal = ObjCon.get_value("SELECT \"ALT_ID\" FROM \"TBLALLOTEMENT\" WHERE \"ALT_NO\"=:altno", NpgsqlCommand);
                if (sQryVal == "" || sQryVal == null)
                {
                    Arr[0] = objTcMaster.sSupplierId;
                    Arr[1] = "2";
                    Arr[2] = "Enter a Valid Allotment Number";
                    return Arr;
                }
                //Get Store Id
                if (objTcMaster.sroletype == "2")
                {
                    sStoreId = objTcMaster.sOfficeCode;
                }
                else
                {
                    sStoreId = GetStoreId(objTcMaster.sOfficeCode);
                }
                string[] strDetailVal = sTcDetails.ToArray();
                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    NpgsqlCommand.Parameters.AddWithValue("tccd", strDetailVal[i].Split('~').GetValue(0).ToString());
                    sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_CODE\" as text)=:tccd", NpgsqlCommand);
                    if (sQryVal != "")
                    {
                        Arr[2] = strDetailVal[i].Split('~').GetValue(0).ToString();
                        Arr[1] = "2";
                        Arr[0] = "Transformer Code " + strDetailVal[i].Split('~').GetValue(0).ToString() + "  Already Exist";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("slno1", strDetailVal[i].Split('~').GetValue(1).ToString());
                    sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_SLNO\" as text)=:slno1", NpgsqlCommand);
                    if (sQryVal != "")
                    {
                        Arr[2] = strDetailVal[i].Split('~').GetValue(1).ToString();
                        Arr[1] = "2";
                        Arr[0] = "Transformer SlNo  " + strDetailVal[i].Split('~').GetValue(1).ToString() + "  Already Exist";
                        return Arr;
                    }
                }

                NpgsqlCommand cmd = new NpgsqlCommand("sp_savetcreceipt");
                //Insert to TBLTCRECIEPT Table
                string sRecieptID;
                cmd.Parameters.AddWithValue("tcr_id", "");
                cmd.Parameters.AddWithValue("tcr_po_no", objTcMaster.sPoNo);
                cmd.Parameters.AddWithValue("tcr_purchase_date", objTcMaster.sAllotementDate);
                cmd.Parameters.AddWithValue("tcr_qnty", objTcMaster.sQuantity);
                cmd.Parameters.AddWithValue("tcr_supplyid", objTcMaster.sSupplierId);
                cmd.Parameters.AddWithValue("tcr_crby", objTcMaster.sCrBy);
                cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);
                cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                Arr[0] = "msg";
                Arr[1] = "op_id";
                Arr[2] = "pk_id";
                Arr = ObjCon.Execute(cmd, Arr, 3);
                sRecieptID = Arr[2].ToString();

                DateTime dPurchaseDate = DateTime.ParseExact(objTcMaster.sAllotementDate, "dd/MM/yyyy", null);

                for (int i = 0; i < strDetailVal.Length; i++)
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcode", strDetailVal[i].Split('~').GetValue(0).ToString());
                    sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where  cast(\"TC_CODE\" as text)='" + strDetailVal[i].Split('~').GetValue(0).ToString() + "'", NpgsqlCommand);
                    if (sQryVal != "")
                    {
                        Arr[2] = strDetailVal[i].Split('~').GetValue(0).ToString();
                        Arr[1] = "2";
                        Arr[0] = "Transformer Code " + strDetailVal[i].Split('~').GetValue(0).ToString() + "  Already Exist";
                        return Arr;
                    }

                    NpgsqlCommand.Parameters.AddWithValue("slNo", strDetailVal[i].Split('~').GetValue(1).ToString());
                    sQryVal = ObjCon.get_value("select \"TC_ID\" from \"TBLTCMASTER\" where cast(\"TC_SLNO\" as text)='"+strDetailVal[i].Split('~').GetValue(1).ToString()+"'", NpgsqlCommand);
                    if (sQryVal != "")
                    {
                        Arr[2] = strDetailVal[i].Split('~').GetValue(1).ToString();
                        Arr[1] = "2";
                        Arr[0] = "Transformer SlNo  " + strDetailVal[i].Split('~').GetValue(1).ToString() + "  Already Exist";
                        return Arr;
                    }
                    objTcMaster.sTcId = ObjCon.Get_max_no("TC_ID", "TBLTCMASTER").ToString();
                    
                    string sWarranty = Convert.ToString(dPurchaseDate.AddMonths(Convert.ToInt32(strDetailVal[i].Split('~').GetValue(6).ToString())));
                    sWarranty = Convert.ToDateTime(sWarranty).ToString("dd/MM/yyyy");

                    //GENERATED ALWAYS AS (ADD_MONTHS(ENTERDATE,IDS))
                    NpgsqlCommand cmd1 = new NpgsqlCommand("sp_savetotcmaster");
                    cmd1.Parameters.AddWithValue("tc_id", "");
                    cmd1.Parameters.AddWithValue("inl_id", "");
                    cmd1.Parameters.AddWithValue("tc_code", strDetailVal[i].Split('~').GetValue(0).ToString());
                    cmd1.Parameters.AddWithValue("tc_serialno", strDetailVal[i].Split('~').GetValue(1).ToString());
                    cmd1.Parameters.AddWithValue("tc_makeid", strDetailVal[i].Split('~').GetValue(2).ToString());
                    cmd1.Parameters.AddWithValue("tc_capacity", strDetailVal[i].Split('~').GetValue(3).ToString());
                    cmd1.Parameters.AddWithValue("tc_manf_date", strDetailVal[i].Split('~').GetValue(4).ToString());
                    cmd1.Parameters.AddWithValue("tc_purchase_date", objTcMaster.sAllotementDate);
                    cmd1.Parameters.AddWithValue("tc_life_span", strDetailVal[i].Split('~').GetValue(5).ToString());
                    cmd1.Parameters.AddWithValue("tc_supplier_id", objTcMaster.sSupplierId);
                    cmd1.Parameters.AddWithValue("tc_po_no", objTcMaster.sPoNo);
                    cmd1.Parameters.AddWithValue("tc_warrantyperiod", sWarranty);
                    cmd1.Parameters.AddWithValue("tc_warranty", strDetailVal[i].Split('~').GetValue(6).ToString());
                    cmd1.Parameters.AddWithValue("tc_curr_loc", "1");
                    cmd1.Parameters.AddWithValue("tc_crby", objTcMaster.sCrBy);
                    cmd1.Parameters.AddWithValue("tc_store_id", sStoreId);
                    cmd1.Parameters.AddWithValue("tc_loc_id", sStoreId);
                    cmd1.Parameters.AddWithValue("tc_tcr_id", sRecieptID);
                    cmd1.Parameters.AddWithValue("tc_oil_capacity", strDetailVal[i].Split('~').GetValue(7).ToString());
                    cmd1.Parameters.AddWithValue("tc_weight", strDetailVal[i].Split('~').GetValue(8).ToString());
                    cmd1.Parameters.AddWithValue("tc_star_rate", strDetailVal[i].Split('~').GetValue(9).ToString());
                    cmd1.Parameters.AddWithValue("tc_di_no", objTcMaster.sDINo);
                    cmd1.Parameters.AddWithValue("tc_alt_no", strDetailVal[i].Split('~').GetValue(10).ToString());
                    cmd1.Parameters.AddWithValue("tc_div_id", strDetailVal[i].Split('~').GetValue(11).ToString());
                    cmd1.Parameters.AddWithValue("record_by","Web");
                    cmd1.Parameters.AddWithValue("device_id",objTcMaster.sClientIP);
                    cmd1.Parameters.AddWithValue("tc_oil_type", strDetailVal[i].Split('~').GetValue(12).ToString());

                    cmd1.Parameters.Add("msg", NpgsqlDbType.Text);
                    cmd1.Parameters.Add("op_id", NpgsqlDbType.Text);

                    cmd1.Parameters["msg"].Direction = ParameterDirection.Output;
                    cmd1.Parameters["op_id"].Direction = ParameterDirection.Output;

                    Arr[0] = "msg";
                    Arr[1] = "op_id";
                    Arr[2] = "msg";
                    Arr = ObjCon.Execute(cmd1, Arr, 3);
                    bResult = true;
                }
                if (bResult == true)
                {
                    Arr[0] = "Transformers Details Saved Successfully";
                    Arr[1] = "0";
                }
                else
                {
                    Arr[0] = "No Transformer Exists to Save";
                    Arr[1] = "2";
                }
                return Arr;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }
        }

        public DataTable LoadTcMaster(clsTcMaster objTc)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {
                strQry = "SELECT (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\"=\"TC_MAKE_ID\") AS \"TC_MAKE_ID\",\"TC_ID\",\"TC_CODE\" AS \"TC_CODE\",";
                strQry += "\"TC_SLNO\",\"TC_MAKE_ID\",\"TC_CAPACITY\" AS \"TC_CAPACITY\", \"TC_LIFE_SPAN\", \"TC_ALT_NO\" FROM \"TBLTCMASTER\" ";
                strQry += "WHERE ";
                if(objTc.sroletype == "2")
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcd",objTc.sOfficeCode);
                    strQry += "cast(\"TC_LOCATION_ID\"as TEXT)   = :offcd";
                }
                else
                {
                    if (objTc.sLocationId != null && objTc.sLocationId != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("locid",Convert.ToDouble( objTc.sLocationId));
                        strQry += "\"TC_LOCATION_ID\"  = :locid";
                    }
                    else
                    {
                        if (objTc.sCurrentLocation == "1" || objTc.sCurrentLocation == "3")
                        {
                            NpgsqlCommand.Parameters.AddWithValue("curloc1", Convert.ToDouble(objTc.sCurrentLocation));
                            strQry += "\"TC_CURRENT_LOCATION\"=:curloc1";
                        }
                        else
                        {
                            if (objTc.sOfficeCode == "" || objTc.sOfficeCode == null)
                            {
                                strQry += " cast(\"TC_LOCATION_ID\" as TEXT) like'%'";
                            }
                            else
                            {
                                NpgsqlCommand.Parameters.AddWithValue("offcode1", objTc.sOfficeCode);
                                strQry += "cast(\"TC_LOCATION_ID\" as TEXT) LIKE :offcode1||'%'";
                            }
                        }
                    }
                }
                
                if (objTc.sTcMakeId != null && objTc.sTcMakeId !="")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcmakeid", objTc.sTcMakeId.ToUpper());
                    strQry += " AND \"TC_MAKE_ID\"=(SELECT \"TM_ID\" FROM \"TBLTRANSMAKES\" WHERE \"TM_NAME\"=:tcmakeid)";
                }
                if (objTc.sTcCapacity != null && objTc.sTcCapacity !="")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tccap",Convert.ToDouble( objTc.sTcCapacity));
                    strQry += " AND \"TC_CAPACITY\"=:tccap";
                }
                if (objTc.sCurrentLocation != null && objTc.sCurrentLocation !="")
                {
                    NpgsqlCommand.Parameters.AddWithValue("curloc",Convert.ToDouble( objTc.sCurrentLocation));
                    strQry += " AND \"TC_CURRENT_LOCATION\"=:curloc";
                }                
                if(objTc.sTcCode != null && objTc.sTcCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcd",Convert.ToDouble( objTc.sTcCode));
                    strQry += "AND \"TC_CODE\" =:tcd ";
                }
                if (objTc.sTcSlNo != null && objTc.sTcSlNo != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("sTcSlNo", objTc.sTcSlNo.ToUpper());
                    strQry += "AND \"TC_SLNO\" =:sTcSlNo ";
                } 
                if (objTc.sOfficeCode != null && objTc.sOfficeCode != "")
                {
                    strQry += " ORDER BY \"TC_ID\" DESC";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC limit 100";
                }
                dtTcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtTcDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcDetails;
            }
        }

        public DataTable LoadInwardTcMaster(clsTcMaster objTc)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {

                strQry = " SELECT (SELECT \"TM_NAME\" FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\"=\"TC_MAKE_ID\") AS \"TC_MAKE\",\"TC_ID\",\"DIV_NAME\",\"TC_CODE\" AS \"TC_CODE\",";
                strQry += " \"TC_SLNO\",\"TC_MAKE_ID\",\"TC_CAPACITY\" AS \"TC_CAPACITY\", \"TC_LIFE_SPAN\",\"TC_ALT_NO\" FROM \"TBLTCMASTER\" ,\"TBLDIVISION\"  ";
                strQry += " WHERE  \"TC_ALT_NO\" is not null   AND \"TC_DIV_ID\"=CAST(\"DIV_ID\" AS TEXT) ";
                if (objTc.sroletype == "2")
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcd", objTc.sOfficeCode);
                    strQry += "and cast(\"TC_LOCATION_ID\"as TEXT)   = :offcd";
                }
                else
                {
                    if (objTc.sLocationId != null && objTc.sLocationId != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("locid", Convert.ToDouble(objTc.sLocationId));
                        strQry += "and \"TC_LOCATION_ID\"  = :locid";
                    }
                    else
                    {
                        if (objTc.sCurrentLocation == "1" || objTc.sCurrentLocation == "3")
                        {
                            NpgsqlCommand.Parameters.AddWithValue("curloc1", Convert.ToDouble(objTc.sCurrentLocation));
                            strQry += " and \"TC_CURRENT_LOCATION\"=:curloc1";
                        }
                        else
                        {
                            if (objTc.sOfficeCode == "" || objTc.sOfficeCode == null)
                            {
                                strQry += "and  cast(\"TC_LOCATION_ID\" as TEXT) like'%'";
                            }
                            else
                           if (objTc.sOfficeCode.Length>=2)
                           {
                               string StoreId = GetStoreId(objTc.sOfficeCode);
                               NpgsqlCommand.Parameters.AddWithValue("storeId", StoreId);
                               strQry += "and cast(\"TC_LOCATION_ID\" as TEXT) LIKE :storeId||'%'";
                           }
                            else
                            {
                                NpgsqlCommand.Parameters.AddWithValue("offcode1", objTc.sOfficeCode);
                                strQry += "and cast(\"TC_LOCATION_ID\" as TEXT) LIKE :offcode1||'%'";
                            }
                        }

                    }
                }


                if (objTc.sTcMakeId != null && objTc.sTcMakeId != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcmakeid", objTc.sTcMakeId.ToUpper());
                    //strQry += " AND CAST(\"TC_MAKE_ID\" AS TEXT)=:tcmakeid";
                    strQry += " AND \"TC_MAKE_ID\"=(SELECT \"TM_ID\" FROM \"TBLTRANSMAKES\" WHERE \"TM_NAME\"=:tcmakeid)";
                }
                if (objTc.sTcCapacity != null && objTc.sTcCapacity != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tccap", Convert.ToDouble(objTc.sTcCapacity));
                    strQry += " AND \"TC_CAPACITY\"=:tccap";
                }
                if (objTc.sDivId != null && objTc.sDivId != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("curloc", Convert.ToString(objTc.sDivId));
                    strQry += " AND \"TC_DIV_ID\"=:curloc";
                }
                if (objTc.sTcCode != null && objTc.sTcCode != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("tcd", Convert.ToDouble(objTc.sTcCode));
                    strQry += "AND \"TC_CODE\" =:tcd ";
                }
                if (objTc.sTcSlNo != null && objTc.sTcSlNo != "")
                {
                    NpgsqlCommand.Parameters.AddWithValue("sTcSlNo", objTc.sTcSlNo.ToUpper());
                    strQry += "AND \"TC_SLNO\" =:sTcSlNo ";
                }
                if (objTc.sOfficeCode != null && objTc.sOfficeCode != "")
                {
                    strQry += " ORDER BY \"TC_ID\" DESC";
                }
                else
                {
                    strQry += " ORDER BY \"TC_ID\" DESC limit 100";
                }
                dtTcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtTcDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcDetails;
            }
        }


        public string getTCCount(string off_Code,string sroleType,string currentlocation)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            try
            {
                if(sroleType == "2")
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcode", off_Code);
                   
                    if (currentlocation != null && currentlocation != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("currentlocation", currentlocation);
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) =:offcode and cast(\"TC_CURRENT_LOCATION\" as text)=:currentlocation";
                    }
                    else
                    {
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) =:offcode ";

                    }

                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcode1", off_Code);
                   
                    if (currentlocation != null && currentlocation != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("currentlocation1", currentlocation);
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) like :offcode1||'%' and  \"TC_CODE\"<>0 and cast(\"TC_CURRENT_LOCATION\" as text)=:currentlocation1";
                    }
                    else
                    {
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) like :offcode1||'%' and  \"TC_CODE\"<>0 ";

                    }
                }

                return ObjCon.get_value(strQry, NpgsqlCommand);
            }
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string getInwardTCCount(string off_Code, string sroleType, string sDivId)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            try
            {
                if (sroleType == "2")
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcode", off_Code);

                    if (sDivId != null && sDivId != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("currentlocation", sDivId);
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) =:offcode and  cast(\"TC_DIV_ID\" as text)=:currentlocation and \"TC_ALT_NO\" is not null";
                    }
                    else
                    {
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) =:offcode  and \"TC_ALT_NO\" is not null";

                    }

                }
                else
                {
                    NpgsqlCommand.Parameters.AddWithValue("offcode1", off_Code);

                    if (sDivId != null && sDivId != "")
                    {
                        NpgsqlCommand.Parameters.AddWithValue("currentlocation1", sDivId);
                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) like :offcode1||'%' and  \"TC_CODE\"<>0 and cast(\"TC_DIV_ID\" as text)=:currentlocation1 and \"TC_ALT_NO\" is not null";
                    }
                    else
                    {
                        string StoreId = GetStoreId(off_Code);
                        NpgsqlCommand.Parameters.AddWithValue("storeId", StoreId);

                        strQry = "SELECT count(\"TC_CODE\") FROM \"TBLTCMASTER\" WHERE cast(\"TC_LOCATION_ID\" as text) like :storeId||'%' and \"TC_ALT_NO\" is not null  and  \"TC_CODE\"<>0 ";

                    }
                }

                return ObjCon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }


        public clsTcMaster GetImagePath(clsTcMaster objDtrCComm)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                DataTable dt = new DataTable();

                NpgsqlCommand.Parameters.AddWithValue("tccode",Convert.ToInt32(objDtrCComm.sTcCode));
                strQry = "SELECT \"EP_NAMEPLATE_PATH\",\"EP_SSPLATE_PATH\" FROM \"TBLDTCENUMERATION\",\"TBLENUMERATIONPHOTOS\",\"TBLENUMERATIONDETAILS\"  ";
                strQry += " WHERE \"DTE_TC_CODE\"=:tccode AND \"DTE_ED_ID\"=\"EP_ED_ID\" AND \"ED_ID\"=\"DTE_ED_ID\" AND \"ED_STATUS_FLAG\"='1' ORDER BY \"EP_ID\" desc";
                dt = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dt.Rows.Count > 0)
                {

                    objDtrCComm.sDTrImagePath = Convert.ToString(dt.Rows[0]["EP_NAMEPLATE_PATH"]);
                    objDtrCComm.sNamePlateImagePath = Convert.ToString(dt.Rows[0]["EP_SSPLATE_PATH"]);
                }
                return objDtrCComm;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objDtrCComm;
            }
        }


        public clsTcMaster GetTCDetails(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {

                string strQry = string.Empty;
                DataTable dtStoreDetails = new DataTable();
                NpgsqlCommand.Parameters.AddWithValue("tcid",Convert.ToInt32( objTcMaster.sTcId));
                strQry = "SELECT \"TC_ID\",\"TC_CODE\",\"TC_ALT_NO\",\"TC_SLNO\",\"TC_MAKE_ID\",CAST(\"TC_CAPACITY\" as TEXT) \"TC_CAPACITY\",TO_CHAR(\"TC_MANF_DATE\",";
                strQry += " 'dd/MM/yyyy')TC_MANF_DATE, TO_CHAR(\"TC_PURCHASE_DATE\",'dd/MM/yyyy')TC_PURCHASE_DATE,CAST(\"TC_LIFE_SPAN\" AS TEXT) AS TC_LIFE_SPAN ,\"TC_OIL_TYPE\"  ";
                strQry += " ,\"TC_SUPPLIER_ID\",\"TC_PO_NO\",\"TC_PRICE\",\"TC_LOCATION_ID\", TO_CHAR(\"TC_WARANTY_PERIOD\",'dd/MM/yyyy')TC_WARANTY_PERIOD,\"TC_WARRENTY\",";
                strQry += "TO_CHAR(\"TC_LAST_SERVICE_DATE\",'dd/MM/yyyy')TC_LAST_SERVICE_DATE, \"TC_STATUS\",\"TC_CURRENT_LOCATION\",\"TC_LAST_REPAIRER_ID\"";
                strQry += ",\"TC_RATING\",\"TC_STAR_RATE\",\"TC_OIL_CAPACITY\",\"TC_WEIGHT\",TO_CHAR(\"TC_LAST_FAILURE_DATE\",'dd/MM/yyyy')TC_LAST_FAILURE_DATE,";
                strQry += " \"TC_LAST_REPAIR_COST\",\"TC_LAST_FAILURE_TYPE\",\"TC_CONDITION\",\"TC_COOLING\",\"TC_TYPE\",\"TC_CORE\",\"TC_TAP_CHARGER\",";
                strQry += " \"TC_ASSET_ID\",\"TC_COMPONENT_ID\",\"TC_ORIGINAL_COST\",\"TC_INSURANCE\",\"TC_DEPRECIATION\",(SELECT to_char(\"TM_CRON\",'dd-mm-yyyy')\"COMMISSIONED_YEAR\"";
                strQry += " FROM (SELECT \"TM_CRON\", row_number() over(ORDER BY \"TM_ID\")  FROM \"TBLTRANSDTCMAPPING\" WHERE \"TM_TC_ID\"=\"TC_CODE\")A WHERE row_number='1') ";
                strQry += " FROM \"TBLTCMASTER\" WHERE \"TC_ID\" =:tcid";
                dtStoreDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dtStoreDetails.Rows.Count > 0)
                {
                    objTcMaster.sTcId = dtStoreDetails.Rows[0]["TC_ID"].ToString();
                    objTcMaster.sTcCode = dtStoreDetails.Rows[0]["TC_CODE"].ToString();
                    objTcMaster.sTcSlNo = dtStoreDetails.Rows[0]["TC_SLNO"].ToString();
                    objTcMaster.sTcMakeId = dtStoreDetails.Rows[0]["TC_MAKE_ID"].ToString();
                    objTcMaster.sLocationId = dtStoreDetails.Rows[0]["TC_LOCATION_ID"].ToString();
                    objTcMaster.sAltNo = dtStoreDetails.Rows[0]["TC_ALT_NO"].ToString();


                    objTcMaster.sTcCapacity = dtStoreDetails.Rows[0]["TC_CAPACITY"].ToString();
                    objTcMaster.sTcLifeSpan = dtStoreDetails.Rows[0]["TC_LIFE_SPAN"].ToString();
                    objTcMaster.sManufacDate = dtStoreDetails.Rows[0]["TC_MANF_DATE"].ToString();
                    objTcMaster.sAllotementDate = dtStoreDetails.Rows[0]["TC_PURCHASE_DATE"].ToString();

                    objTcMaster.sPoNo = dtStoreDetails.Rows[0]["TC_PO_NO"].ToString();
                    objTcMaster.sPrice = dtStoreDetails.Rows[0]["TC_PRICE"].ToString();
                    objTcMaster.sSupplierId = dtStoreDetails.Rows[0]["TC_SUPPLIER_ID"].ToString();
                    objTcMaster.sWarrentyPeriod = dtStoreDetails.Rows[0]["TC_WARRENTY"].ToString();

                    objTcMaster.sLastServiceDate = dtStoreDetails.Rows[0]["TC_LAST_SERVICE_DATE"].ToString();

                    objTcMaster.sCurrentLocation = dtStoreDetails.Rows[0]["TC_CURRENT_LOCATION"].ToString();
                    objTcMaster.sLastRepairerId = dtStoreDetails.Rows[0]["TC_LAST_REPAIRER_ID"].ToString();

                    objTcMaster.sRating = Convert.ToString(dtStoreDetails.Rows[0]["TC_RATING"]);
                    objTcMaster.sStarRate = Convert.ToString(dtStoreDetails.Rows[0]["TC_STAR_RATE"]);
                    objTcMaster.sOilCapacity = Convert.ToString(dtStoreDetails.Rows[0]["TC_OIL_CAPACITY"]);
                    objTcMaster.sOilType = Convert.ToString(dtStoreDetails.Rows[0]["TC_OIL_TYPE"]);
                    objTcMaster.sWeight = Convert.ToString(dtStoreDetails.Rows[0]["TC_WEIGHT"]);

                    objTcMaster.sLastFaildate = Convert.ToString(dtStoreDetails.Rows[0]["TC_LAST_FAILURE_DATE"]);
                    objTcMaster.sLastFailuretype = Convert.ToString(dtStoreDetails.Rows[0]["TC_LAST_FAILURE_TYPE"]);
                    objTcMaster.sLastRepaircost = Convert.ToString(dtStoreDetails.Rows[0]["TC_LAST_REPAIR_COST"]);
                    objTcMaster.sConditiontc = Convert.ToString(dtStoreDetails.Rows[0]["TC_CONDITION"]);
                    objTcMaster.sCooling = Convert.ToString(dtStoreDetails.Rows[0]["TC_COOLING"]);
                    objTcMaster.sCore = Convert.ToString(dtStoreDetails.Rows[0]["TC_TYPE"]);
                    objTcMaster.sType = Convert.ToString(dtStoreDetails.Rows[0]["TC_CORE"]);
                    objTcMaster.sTapeCharger = Convert.ToString(dtStoreDetails.Rows[0]["TC_TAP_CHARGER"]);
                    objTcMaster.sDTRcommissionYear = Convert.ToString(dtStoreDetails.Rows[0]["COMMISSIONED_YEAR"]);

                    objTcMaster.sLastRepaircount = "0";

                    objTcMaster.sInfosysId = Convert.ToString(dtStoreDetails.Rows[0]["TC_ASSET_ID"]);
                    objTcMaster.sComponentId = Convert.ToString(dtStoreDetails.Rows[0]["TC_COMPONENT_ID"]);
                    objTcMaster.sOriginalCost = Convert.ToString(dtStoreDetails.Rows[0]["TC_ORIGINAL_COST"]);
                    objTcMaster.sInsurance = Convert.ToString(dtStoreDetails.Rows[0]["TC_INSURANCE"]);
                    objTcMaster.sDepreciation = Convert.ToString(dtStoreDetails.Rows[0]["TC_DEPRECIATION"]);

                    if (Convert.ToString(dtStoreDetails.Rows[0]["TC_SLNO"]) != "")
                    {
                        string tcslno = Convert.ToString(dtStoreDetails.Rows[0]["TC_SLNO"]);
                        objTcMaster.sTcSlNo = Regex.Replace(tcslno, @"[^0-9a-zA-Z_\-()/.]+", "");
                    }

                    NpgsqlCommand.Parameters.AddWithValue("tccd",Convert.ToInt32(objTcMaster.sTcCode));
                    strQry = "SELECT count(*) FROM \"TBLDTCFAILURE\" WHERE \"DF_EQUIPMENT_ID\"=:tccd";
                    objTcMaster.sFailCount = Convert.ToString(ObjCon.get_value(strQry, NpgsqlCommand));
                }
                return objTcMaster;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTcMaster;

            }

        }

        public string GetTransformerCount(string sOfficeCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                if (sOfficeCode.Length > 1)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                }
                NpgsqlCommand.Parameters.AddWithValue("offcd", sOfficeCode);
                strQry = "SELECT COUNT(*) FROM \"TBLTCMASTER\" WHERE \"TC_STORE_ID\" IN (SELECT \"SM_ID\" FROM \"TBLSTOREMAST\" WHERE  \"SM_OFF_CODE\"=:offcd)";
                return ObjCon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public string GetStoreId(string sOfficeCode)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                string strQry = string.Empty;
                if (sOfficeCode.Length > 2)
                {
                    sOfficeCode = sOfficeCode.Substring(0, Constants.Division);
                }

                NpgsqlCommand.Parameters.AddWithValue("offcd",sOfficeCode);
                strQry = "SELECT \"SM_ID\" FROM \"TBLSTOREMAST\",\"TBLSTOREOFFCODE\" WHERE \"SM_ID\" = \"STO_SM_ID\" AND  CAST(\"STO_OFF_CODE\" AS TEXT)=:offcd";
                return ObjCon.get_value(strQry, NpgsqlCommand);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return "";
            }
        }

        public bool CheckTransformerCodeExist(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("tccode",Convert.ToInt32( objTcMaster.sTcCode));
                string sQryVal = ObjCon.get_value("select * from \"TBLTCMASTER\" where \"TC_CODE\"=:tccode ", NpgsqlCommand);
                if (sQryVal != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }

        public object GetPoDetails(clsTcMaster objTc)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {

                NpgsqlCommand.Parameters.AddWithValue("pono", objTc.sAltNo.ToUpper());
                strQry = "Select \"PO_ID\",TO_CHAR(\"PO_DATE\",'DD/MM/yyyy') PO_DATE,SUM(\"PB_QUANTITY\") AS PB_QUANTITY,\"PO_SUPPLIER_ID\",\"PO_NO\" ";
                strQry += " FROM \"TBLPOMASTER\",\"TBLPOOBJECTS\" WHERE \"PB_PO_ID\"=\"PO_ID\" AND UPPER(\"PO_NO\")=:pono GROUP BY \"PO_ID\",\"PO_DATE\",\"PO_SUPPLIER_ID\",\"PO_NO\" ";
                dtTcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dtTcDetails.Rows.Count > 0)
                {
                    objTc.sAltId = dtTcDetails.Rows[0]["PO_ID"].ToString();
                    objTc.sAltNo = dtTcDetails.Rows[0]["PO_NO"].ToString();
                    objTc.sAllotementDate = dtTcDetails.Rows[0]["PO_DATE"].ToString();
                    objTc.sQuantity = dtTcDetails.Rows[0]["PB_QUANTITY"].ToString();
                    objTc.sSupplierId = dtTcDetails.Rows[0]["PO_SUPPLIER_ID"].ToString();
                }
                return objTc;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcDetails;
            }
        }
        public DataTable GetAllotedDetails(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
                    
            try
            {
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("Altno", objTcMaster.sAltNo);
                NpgsqlCommand.Parameters.AddWithValue("offcd", objTcMaster.sOfficeCode);

                sQry = "  select \"ALT_ID\",\"DI_NO\" AS \"ALT_DI_NO\",\"ALT_NO\",\"PO_NO\",TO_CHAR(\"PO_DATE\",'DD/MM/YYYY') AS \"ALT_DATE\",\"ALT_STORE_ID\",\"SM_NAME\" AS \"ALT_STORE_NAME\",\"DIV_NAME\" AS \"DIV_NAME\",\"ALT_CAPACITY\",\"ALT_CAPACITY_ID\",\"TM_NAME\" AS \"MAKE_NAME\",\"ALT_STAR_TYPE\" ,\"MD_NAME\" AS \"ALT_STARRATENAME\" ,";
                sQry += " \"ALT_DIV_ID\",\"ALT_MAKE_ID\" ,\"PO_SUPPLIER_ID\",\"TS_NAME\" AS \"SUPPLIER_NAME\"  FROM  \"TBLDIVISION\" ,\"TBLPOMASTER\" ,\"TBLALLOTEMENT\",\"TBLDELIVERYINSTRUCTION\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLMASTERDATA\",\"TBLTRANSSUPPLIER\" WHERE \"ALT_STAR_TYPE\"=\"MD_ID\" AND ";
                sQry += " \"MD_TYPE\"='SR' AND \"DI_NO\"=\"ALT_DI_NO\"  AND \"TM_ID\"=\"ALT_MAKE_ID\" and \"ALT_DI_NO\"=\"DI_NO\" AND \"PO_SUPPLIER_ID\"=\"TS_ID\" AND \"DI_PO_ID\"=\"PO_ID\"   AND \"ALT_DIV_ID\"=\"DIV_ID\"  AND cast(\"ALT_STORE_ID\" as text) =:offcd AND \"ALT_STATUS\"=1 AND  \"SM_ID\"=\"ALT_STORE_ID\" and \"ALT_NO\"=:Altno ";
                sQry += " GROUP BY  \"ALT_ID\",\"DI_NO\",\"ALT_NO\",\"PO_NO\",\"PO_DATE\",\"ALT_STORE_ID\",\"SM_NAME\",\"DIV_NAME\" ,\"ALT_CAPACITY\",\"ALT_CAPACITY_ID\",\"MD_NAME\" ,\"ALT_MAKE_ID\" ,\"TM_NAME\" ,\"ALT_STAR_TYPE\",\"ALT_QUANTITY\",\"ALT_DIV_ID\",\"PO_NO\",\"PO_SUPPLIER_ID\",\"TS_NAME\" ORDER BY \"ALT_ID\"";
                //sQry = " SELECT \"DI_ID\",\"DI_NO\",\"DI_PO_ID\", TO_CHAR(\"DI_DATE\",'DD/MON/YYYY') AS \"DI_DATE\",\"SM_NAME\" AS \"STORE_NAME\",\"TM_NAME\" AS \"MAKE_NAME\",\"DI_CAPACITY\",";
                //sQry += "\"MD_NAME\"AS \"STAR_RATE\", \"DI_QUANTITY\" FROM  \"TBLDELIVERYINSTRUCTION\",\"TBLTRANSMAKES\",\"TBLSTOREMAST\",\"TBLMASTERDATA\" WHERE \"DI_STORE_ID\"=\"SM_ID\" AND";
                //sQry += " \"TM_ID\"=\"DI_MAKE_ID\" AND \"MD_TYPE\"='SR' AND \"MD_ID\"=\"DI_STARTTYPE\" and \"DI_NO\" LIKE :DIno||'%'  ORDER BY \"DI_ID\"";
                objTcMaster.dtTable = ObjCon.FetchDataTable(sQry, NpgsqlCommand);

                return objTcMaster.dtTable;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return objTcMaster.dtTable;
            }
        }
        public string loadCountTc(clsTcMaster objTcMaster)
        {
            string sCount = string.Empty;
            try{
                
                string sQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("Altno", objTcMaster.sAltNo);
                NpgsqlCommand.Parameters.AddWithValue("offcd", objTcMaster.sOfficeCode);

                sQry = "select sum(\"ALT_QUANTITY\") FROM \"TBLALLOTEMENT\" WHERE \"ALT_NO\"=:Altno AND \"ALT_STATUS\"=1  AND cast(\"ALT_STORE_ID\" as text) =:offcd ";
                  sCount = ObjCon.get_value(sQry, NpgsqlCommand);
                return sCount;
               }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sCount;
            }
        }


        public object GetDeliveryDetails(clsTcMaster objTc)
        {

            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("pono", objTc.sAltNo.ToUpper());
                strQry = "SELECT \"DI_ID\", TO_CHAR(\"DI_DATE\", 'DD/MM/yyyy') \"DI_DATE\", SUM(\"DI_QUANTITY\") AS \"DI_QUANTITY\",\"PO_SUPPLIER_ID\", ";
                strQry += " \"DI_NO\"  FROM \"TBLPOMASTER\", \"TBLTRANSSUPPLIER\", \"TBLDELIVERYINSTRUCTION\" WHERE \"PO_ID\" = \"DI_PO_ID\" AND ";
                strQry += " \"PO_SUPPLIER_ID\" = \"TS_ID\" AND UPPER(REPLACE(\"DI_NO\",'\\','')) =:pono GROUP BY \"DI_ID\", \"DI_DATE\", \"PO_SUPPLIER_ID\", \"DI_NO\" ";

                dtTcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                if (dtTcDetails.Rows.Count > 0)
                {
                    objTc.sAltId = dtTcDetails.Rows[0]["DI_ID"].ToString();
                    objTc.sAltNo = dtTcDetails.Rows[0]["DI_NO"].ToString();
                    objTc.sAllotementDate = dtTcDetails.Rows[0]["DI_DATE"].ToString();
                    objTc.sQuantity = dtTcDetails.Rows[0]["DI_QUANTITY"].ToString();
                    objTc.sSupplierId = dtTcDetails.Rows[0]["PO_SUPPLIER_ID"].ToString();
                }
                return objTc;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcDetails;
            }
        }

        public DataTable LoadTcGrid(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            string strQry = string.Empty;
            DataTable dtTcDetails = new DataTable();
            try
            {
                #region// old query
                // strQry = "SELECT PO_ID,PO_NO,CAPACITY,REQ_QNTY,(NVL(REQ_QNTY,0) - NVL(SENT_QNT,0)) AS PENDINGCOUNT  FROM ";
                // strQry += " (SELECT PO_ID,PO_NO,PB_QUANTITY AS REQ_QNTY,TO_CHAR(PB_CAPACITY) AS CAPACITY FROM TBLPOMASTER, TBLPOOBJECTS WHERE PO_ID = PB_PO_ID GROUP BY PO_ID,PO_NO,PB_CAPACITY,PB_QUANTITY)A,";
                // strQry += " (SELECT TCR_PO_NO,TO_CHAR(PB_CAPACITY) PB_CAPACITY,SUM(TCR_QUANTITY) AS SENT_QNT FROM TBLPOMASTER,TBLTCRECIEPT,TBLPOOBJECTS,";
                // strQry += " TBLTCMASTER WHERE  PO_ID = PB_PO_ID AND TCR_PO_NO=PO_ID AND PB_PO_ID=TCR_PO_NO AND TC_TCR_ID = TCR_ID ";
                //strQry+= " AND PB_CAPACITY = TC_CAPACITY GROUP BY TCR_PO_NO,PB_CAPACITY)B WHERE A.PO_ID= B.TCR_PO_NO(+) ";
                //strQry += " AND A.CAPACITY = B.PB_CAPACITY(+) AND   (NVL(REQ_QNTY,0) - NVL(SENT_QNT,0))<>0 AND A.PO_ID='" + objTcMaster.sPoId + "'";

                //strQry = "  SELECT \"PO_ID\",\"PO_NO\",\"CAPACITY\",\"REQ_QNTY\",(COALESCE(\"REQ_QNTY\",0) - COALESCE(\"SENT_QNT\",0)) AS \"PENDINGCOUNT\",\"MAKE\" ";
                //strQry += "  FROM (SELECT \"PO_ID\",\"PO_NO\",\"PB_QUANTITY\" AS \"REQ_QNTY\",CAST(\"PB_CAPACITY\" AS TEXT)  AS \"CAPACITY\", (SELECT \"TM_NAME\"";
                //strQry += "  FROM \"TBLTRANSMAKES\" WHERE \"TM_ID\"=\"PB_MAKE\") AS \"MAKE\" FROM \"TBLPOMASTER\", \"TBLPOOBJECTS\" WHERE  \"PO_ID\" = \"PB_PO_ID\"";
                //strQry += "  GROUP BY  \"PO_ID\",\"PO_NO\",\"PB_CAPACITY\",\"PB_QUANTITY\",\"PB_MAKE\")A LEFT OUTER JOIN (SELECT \"TCR_PO_NO\",";
                //strQry += " CAST(\"PB_CAPACITY\" AS TEXT) \"PB_CAPACITY\", count(\"PB_CAPACITY\") AS \"SENT_QNT\" FROM  \"TBLPOMASTER\",\"TBLTCRECIEPT\",\"TBLPOOBJECTS\", ";
                //strQry += "  \"TBLTCMASTER\" WHERE  \"PO_ID\" = \"PB_PO_ID\" AND  \"TCR_PO_NO\"=CAST(\"PO_ID\" AS TEXT) AND CAST(\"PB_PO_ID\" AS TEXT)=\"TCR_PO_NO\"";
                //strQry += " AND \"TC_TCR_ID\" =\"TCR_ID\" AND \"PB_CAPACITY\" = \"TC_CAPACITY\" GROUP BY \"TCR_PO_NO\",\"PB_CAPACITY\")B ";
                //strQry += "ON CAST(A.\"PO_ID\" AS TEXT) = CAST(B.\"TCR_PO_NO\" AS TEXT) AND CAST(A.\"CAPACITY\" AS TEXT) = CAST(B.\"PB_CAPACITY\" ";
                //strQry += "  AS TEXT) WHERE  A.\"PO_ID\"='" + objTcMaster.sPoId + "'";
                #endregion
                NpgsqlCommand.Parameters.AddWithValue("offcd", objTcMaster.sOfficeCode);
                NpgsqlCommand.Parameters.AddWithValue("altno", objTcMaster.sAltNo);
                #region//old Query
                //strQry = " SELECT \"DI_ID\" \"PO_ID\",\"DI_NO\" \"PO_NO\",\"CAPACITY\",\"REQ_QNTY\",(COALESCE(\"REQ_QNTY\",0) - COALESCE(\"SENT_QNT\",0)) AS \"PENDINGCOUNT\",";
                //strQry += "  \"MAKE\" ,\"RATING\"   FROM (SELECT \"DI_ID\", \"DI_NO\", SUM(\"DI_QUANTITY\") AS \"REQ_QNTY\", CAST(\"DI_CAPACITY\" AS TEXT) ";
                //strQry += "  \"CAPACITY\", \"TM_NAME\"AS \"MAKE\"  , \"MD_NAME\" AS \"RATING\" FROM \"TBLDELIVERYINSTRUCTION\", \"TBLTRANSMAKES\", ";
                //strQry += "  (SELECT \"MD_ID\", \"MD_NAME\" FROM  \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'SR')A  WHERE \"TM_ID\" = \"DI_MAKE_ID\" AND ";
                //strQry += "  \"DI_STARTTYPE\" = \"MD_ID\" AND \"DI_STORE_ID\" =:offcd GROUP BY \"DI_ID\", \"DI_NO\", \"DI_CAPACITY\", \"TM_NAME\" , \"MD_NAME\")X  LEFT OUTER JOIN ";
                //strQry += "  (SELECT \"TCR_PO_NO\", CAST(\"DI_CAPACITY\" AS TEXT) \"DI_CAPACITY\", \"MD_NAME\", count(\"DI_CAPACITY\") AS \"SENT_QNT\" ";
                //strQry += "  FROM \"TBLDELIVERYINSTRUCTION\", \"TBLTCRECIEPT\", \"TBLTCMASTER\", (SELECT \"MD_ID\", \"MD_NAME\" FROM  \"TBLMASTERDATA\" ";
                //strQry += "  WHERE \"MD_TYPE\" = 'SR')A WHERE \"TCR_PO_NO\"=CAST(\"DI_ID\" AS TEXT) AND \"TC_TCR_ID\" =\"TCR_ID\" AND \"DI_CAPACITY\" = ";
                //strQry += "  \"TC_CAPACITY\" AND \"TC_RATING\" = \"MD_ID\" GROUP BY \"TCR_PO_NO\",\"DI_CAPACITY\",\"MD_NAME\")Y ON ";
                //strQry += "  CAST(X.\"DI_ID\" AS TEXT) = CAST(Y.\"TCR_PO_NO\" AS TEXT) AND CAST(X.\"CAPACITY\" AS TEXT) = CAST(Y.\"DI_CAPACITY\"   AS TEXT) ";
                //strQry += "  AND CAST(X.\"RATING\" AS TEXT) = CAST(Y.\"MD_NAME\" AS TEXT) WHERE X.\"DI_NO\"=:pono ";
                #endregion
                      	
                   strQry = "  SELECT \"ALT_ID\",\"ALT_NO\",\"ALT_DI_NO\", \"CAPACITY\", \"DIV_NAME\",\"REQ_QNTY\",(COALESCE(\"REQ_QNTY\",0) - COALESCE(\"SENT_QNT\",0)) AS \"PENDINGCOUNT\",  \"MAKE\" ,";
                   strQry += " \"RATING\"   FROM (SELECT  \"ALT_ID\", \"ALT_NO\",\"ALT_DI_NO\",\"ALT_STORE_ID\",\"ALT_DIV_ID\",\"DIV_NAME\" ,SUM(\"ALT_QUANTITY\") AS \"REQ_QNTY\", CAST(\"ALT_CAPACITY\" AS TEXT)   \"CAPACITY\","; 
                   strQry += " \"TM_NAME\"AS \"MAKE\"  , \"MD_NAME\" AS \"RATING\" FROM \"TBLALLOTEMENT\", \"TBLTRANSMAKES\",\"TBLDIVISION\",    (SELECT \"MD_ID\", \"MD_NAME\" FROM  ";
                   strQry += " \"TBLMASTERDATA\" WHERE \"MD_TYPE\" = 'SR')A  WHERE \"TM_ID\" = \"ALT_MAKE_ID\" AND \"DIV_ID\"=\"ALT_DIV_ID\" AND cast(\"ALT_STORE_ID\" as text) =:offcd AND \"ALT_STATUS\"=1 AND    \"ALT_STAR_TYPE\" = \"MD_ID\"  GROUP BY  \"ALT_ID\", \"ALT_NO\",";
                   strQry += " \"ALT_DI_NO\",\"ALT_CAPACITY\", \"TM_NAME\" , \"MD_NAME\",\"ALT_DIV_ID\",\"ALT_STORE_ID\",\"DIV_NAME\")X  LEFT OUTER JOIN   (SELECT  DISTINCT(\"ALT_CAPACITY\"), \"TCR_PO_NO\",\"ALT_DIV_ID\",\"ALT_STORE_ID\",\"TC_ALT_NO\",  ";
                   strQry += "   \"MD_NAME\", count(\"TC_CAPACITY\") AS \"SENT_QNT\"   FROM \"TBLALLOTEMENT\", \"TBLTCRECIEPT\", \"TBLTCMASTER\",\"TBLPOMASTER\", (SELECT ";
                   strQry += " \"MD_ID\", \"MD_NAME\" FROM  \"TBLMASTERDATA\"   WHERE \"MD_TYPE\" = 'SR')A WHERE \"TCR_PO_NO\"=CAST(\"PO_NO\" AS TEXT) AND \"TC_TCR_ID\" = ";
                   strQry += " \"TCR_ID\"  AND \"TC_ALT_NO\"=\"ALT_NO\" AND \"ALT_STORE_ID\"=\"TC_STORE_ID\" and \"ALT_NO\"=:altno AND \"ALT_STATUS\"=1 and \"ALT_MAKE_ID\"=\"TC_MAKE_ID\" AND \"ALT_STAR_TYPE\"=\"TC_STAR_RATE\" AND CAST(\"ALT_DIV_ID\" AS TEXT)=\"TC_DIV_ID\"  AND \"ALT_CAPACITY\" =CAST(\"TC_CAPACITY\" AS TEXT) AND \"TC_RATING\" = \"MD_ID\" GROUP BY \"TCR_PO_NO\",\"ALT_CAPACITY\",\"MD_NAME\",\"ALT_DIV_ID\",\"ALT_STORE_ID\",\"TC_ALT_NO\")Y ";
                   strQry += " ON  CAST(X.\"CAPACITY\" AS TEXT) = CAST(Y.\"ALT_CAPACITY\" AS TEXT)   AND CAST(X.\"RATING\" AS TEXT) = CAST(Y.\"MD_NAME\" AS TEXT)  ";
                   strQry += " AND CAST(X.\"ALT_DIV_ID\" AS TEXT) = CAST(Y.\"ALT_DIV_ID\" AS TEXT)  AND CAST(X.\"ALT_STORE_ID\" AS TEXT) = CAST(Y.\"ALT_STORE_ID\" AS TEXT)  WHERE  \"ALT_NO\"=:altno ";
                   strQry += " GROUP BY  \"ALT_ID\",\"ALT_NO\",\"ALT_DI_NO\", \"CAPACITY\",\"REQ_QNTY\", \"PENDINGCOUNT\",\"MAKE\" ,\"RATING\",\"DIV_NAME\" ";

                dtTcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtTcDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtTcDetails;
            }
        }


        public DataTable GetRepairerDetails(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("offcd", objTcMaster.srepairOffCode);
                String strQry = "SELECT \"TR_NAME\",\"TR_ADDRESS\",\"TR_MOBILE_NO\",\"TR_EMAIL\"";
                strQry += " FROM \"TBLTRANSREPAIRER\",\"TBLTRANSREPAIREROFFCODE\" ";
                strQry += " WHERE \"TR_ID\"=\"TRO_TR_ID\" and cast(\"TRO_OFF_CODE\" as text) Like :offcd||'%'";
                dtDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDetails;
            }

        }

        public DataTable GetStoreDetails(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtStoreDetails = new DataTable();
            try
            {
                string strQry = string.Empty;

                NpgsqlCommand.Parameters.AddWithValue("offcode", objTcMaster.srepairOffCode);
                NpgsqlCommand.Parameters.AddWithValue("tccode",objTcMaster.sTcCode);
                //if (objTcMaster.srepairOffCode != null && objTcMaster.srepairOffCode != "")
                //{
                //    strQry = "SELECT \"SM_NAME\",\"SM_STORE_INCHARGE\",\"SM_MOBILENO\",\"SM_ADDRESS\" FROM \"TBLSTOREMAST\" WHERE cast(\"SM_ID\" as TEXT) like :offcode||'%'";
                //}
                //else
                //{
                    strQry = "SELECT \"SM_NAME\",\"SM_STORE_INCHARGE\",\"SM_MOBILENO\",\"SM_ADDRESS\" FROM \"TBLSTOREMAST\" ,\"TBLTCMASTER\" WHERE CAST(\"TC_CODE\" AS TEXT)=:tccode AND  cast(\"SM_ID\" as TEXT)=cast(\"TC_STORE_ID\" as TEXT)";
                //}
                dtStoreDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtStoreDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtStoreDetails;
            }
        }

        public DataTable GetDtcDetails(clsTcMaster objtcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtDtcDetails = new DataTable();
            try
            {
                string strQry = string.Empty;
                NpgsqlCommand.Parameters.AddWithValue("dtcode", objtcMaster.sDtcCodes);
                strQry = " SELECT \"DT_CODE\",\"DT_NAME\",to_char(\"DT_LAST_SERVICE_DATE\",'dd/MM/yyyy')DT_LAST_SERVICE_DATE,to_char(\"DT_LAST_INSP_DATE\",'dd/MM/yyyy')DT_LAST_INSP_DATE from";
                strQry += " \"TBLDTCMAST\" WHERE \"DT_CODE\" like :dtcode||'%'";
                dtDtcDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtDtcDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDtcDetails;
            }
        }

        public DataTable GetFieldDetails(clsTcMaster objTcMaster)
        {
            NpgsqlCommand = new NpgsqlCommand();
            DataTable dtDetails = new DataTable();
            try
            {
                NpgsqlCommand.Parameters.AddWithValue("tccode",Convert.ToInt32( objTcMaster.sTcCode));
                String strQry = "select \"DT_CODE\",\"DT_NAME\" from \"TBLDTCMAST\",\"TBLTCMASTER\",\"TBLTRANSDTCMAPPING\" where \"TM_DTC_ID\"=\"DT_CODE\"  and \"TM_TC_ID\"=\"TC_CODE\" ";
                strQry += " and \"TM_LIVE_FLAG\"=1  and \"TC_CODE\"=:tccode ";
                dtDetails = ObjCon.FetchDataTable(strQry, NpgsqlCommand);
                return dtDetails;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtDetails;
            }

        }

        public string SaveXmlData(clsTcMaster objTcMaster)
        {
            string sTcXmlData = string.Empty;

            try
            {

                string strQry = string.Empty;
                string strTemp = string.Empty;

                string sPrimaryKey = "{0}";

                objTcMaster.sColumnNames = "TC_ID,TC_CODE,TC_SLNO,TC_MAKE_ID,TC_CAPACITY,TC_MANF_DATE,TC_PURCHASE_DATE,";
                objTcMaster.sColumnNames += "TC_LIFE_SPAN,TC_SUPPLIER_ID,TC_PO_NO,TC_PRICE,TC_WARANTY_PERIOD,TC_LAST_SERVICE_DATE,";
                objTcMaster.sColumnNames += "TC_CURRENT_LOCATION,TC_CRBY,TC_WARRENTY,TC_STORE_ID,TC_LOCATION_ID,TC_RATING,TC_STAR_RATE,TC_OIL_CAPACITY,TC_WEIGHT,TC_IOL_TYPE";
                objTcMaster.sColumnValues = "" + objTcMaster.sTcId + "," + objTcMaster.sTcCode + "," + objTcMaster.sTcSlNo + ",";
                objTcMaster.sColumnValues += "" + objTcMaster.sTcMakeId + "," + objTcMaster.sTcCapacity + "," + objTcMaster.sManufacDate + ",";
                objTcMaster.sColumnValues += "" + objTcMaster.sAllotementDate + "," + objTcMaster.sTcLifeSpan + ",";
                objTcMaster.sColumnValues += "" + objTcMaster.sSupplierId + "," + objTcMaster.sAltNo + "," + objTcMaster.sPrice + ",";
                objTcMaster.sColumnValues += "" + sWarranty + "," + objTcMaster.sLastServiceDate + ",";
                objTcMaster.sColumnValues += "" + objTcMaster.sCurrentLocation + "," + objTcMaster.sCrBy + "," + objTcMaster.sWarrentyPeriod + ",";
                objTcMaster.sColumnValues += "" + sStoreId + "," + objTcMaster.sOfficeCode + "," + objTcMaster.sRating + "," + objTcMaster.sStarRate + ",";
                objTcMaster.sColumnValues += "" + objTcMaster.sOilCapacity + "," + objTcMaster.sWeight + "," + objTcMaster.sOilType + "";

                objTcMaster.sTableNames = "TBLTCMASTER";

                sTcXmlData = CreateXml(objTcMaster.sColumnNames, objTcMaster.sColumnValues, objTcMaster.sTableNames);
                return sTcXmlData;
            }

            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return sTcXmlData;
            }

        }

        //save data in tblefodata and workflowobjects
        public bool SaveWorkFlowData(clsTcMaster objTcMaster)
        {
            
            try
            {
                string[] Arr = new string[3]; string strQry = string.Empty;
                objTcMaster.sWFDataId = Convert.ToString(ObjCon.Get_max_no("WFO_ID", "TBLWFODATA"));

                NpgsqlCommand cmd = new NpgsqlCommand("sp_savewfodata");
                cmd.Parameters.AddWithValue("wf_id", objTcMaster.sWFDataId);
                if (objTcMaster.sQryValues == ""|| objTcMaster.sQryValues==null)
                {
                    objTcMaster.sQryValues = "sQryValues";
                }
                cmd.Parameters.AddWithValue("wf_qry_vals", objTcMaster.sQryValues);

                if (objTcMaster.sParameterValues == "" || objTcMaster.sParameterValues == null)
                {
                    objTcMaster.sParameterValues = "sparamValues";
                }

                cmd.Parameters.AddWithValue("wf_param", objTcMaster.sParameterValues);

                if (objTcMaster.sXmlData == "" || objTcMaster.sXmlData == null)
                {
                    objTcMaster.sXmlData = "sXmlData";
                }

                cmd.Parameters.AddWithValue("wf_data", objTcMaster.sXmlData);
                cmd.Parameters.AddWithValue("wf_crby", objTcMaster.sCrBy);

                cmd.Parameters.Add("pk_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("op_id", NpgsqlDbType.Text);
                cmd.Parameters.Add("msg", NpgsqlDbType.Text);

                cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                Arr[0] = "pk_id";
                Arr[1] = "op_id";
                Arr[2] = "msg";
                Arr = ObjCon.Execute(cmd, Arr, 3);

                if (objTcMaster.sFormName != null && objTcMaster.sFormName != "")
                {
                    //To get Business Object Id
                    NpgsqlCommand = new NpgsqlCommand();
                    NpgsqlCommand.Parameters.AddWithValue("formname", objTcMaster.sFormName.Trim().ToUpper());
                    objTcMaster.sBOId = ObjCon.get_value("SELECT \"BO_ID\" FROM \"TBLBUSINESSOBJECT\" WHERE UPPER(\"BO_FORMNAME\")=:formname", NpgsqlCommand);
                }

                WorkFlowObjects(objTcMaster);

                string sWFlowId = Convert.ToString(ObjCon.Get_max_no("WO_ID", "TBLWORKFLOWOBJECTS"));
                NpgsqlCommand cmd1 = new NpgsqlCommand("sp_saveWFOobjects");
                cmd1.Parameters.AddWithValue("wo_id", sWFlowId);
                cmd1.Parameters.AddWithValue("wo_bo_id", objTcMaster.sBOId);
                cmd1.Parameters.AddWithValue("wo_record_id", objTcMaster.sTcId);
                cmd1.Parameters.AddWithValue("wo_prev_approve_id", "0");
                cmd1.Parameters.AddWithValue("wo_next_role", "0");
                cmd1.Parameters.AddWithValue("wo_office_code", objTcMaster.sOfficeCode);
                cmd1.Parameters.AddWithValue("wo_client_ip", objTcMaster.sClientIP);
                cmd1.Parameters.AddWithValue("wo_cr_by", objTcMaster.sCrBy);
                cmd1.Parameters.AddWithValue("wo_approved_by", objTcMaster.sCrBy);
                cmd1.Parameters.AddWithValue("wo_approve_status", "1");
                cmd1.Parameters.AddWithValue("wo_record_by", "WEB");
                cmd1.Parameters.AddWithValue("wo_description", objTcMaster.sDescription);
                cmd1.Parameters.AddWithValue("wo_user_comment", objTcMaster.sDescription);
                cmd1.Parameters.AddWithValue("wo_wfo_id", objTcMaster.sWFDataId);
                cmd1.Parameters.AddWithValue("wo_initial_id", sWFlowId);
                cmd1.Parameters.AddWithValue("wo_ref_offcode", objTcMaster.sOfficeCode);
                cmd.Parameters["pk_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["op_id"].Direction = ParameterDirection.Output;
                cmd.Parameters["msg"].Direction = ParameterDirection.Output;

                Arr[0] = "pk_id";
                Arr[1] = "op_id";
                Arr[2] = "msg";
                Arr = ObjCon.Execute(cmd, Arr, 3);
                return true;
            }
            catch (Exception ex)
            {
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
               
                return false;
            }
        }

        //To Get CLient ip
        public void WorkFlowObjects(clsTcMaster objTcMaster)
        {
            try
            {
                string sClientIP = string.Empty;

                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    int le = ipRange.Length - 1;
                    sClientIP = ipRange[0];
                }
                else
                {
                    sClientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                objTcMaster.sClientIP = sClientIP;

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //creating xml data for Wfo_data insert
        public string CreateXml(string strColumns, string strParameters, string strTableName)
        {
            try
            {
                DataTable dtXmlContent = new DataTable();

                DataTable dtnew = new DataTable();

                DataSet ds;
                if (strTableName.Contains(";"))
                {
                    ds = new DataSet(strTableName.Split(';').GetValue(0).ToString());
                }
                else
                {
                    ds = new DataSet(strTableName);
                }

                string[] strArrColumns = strColumns.Split(';');
                string[] strArrParameters = strParameters.Split(';');
                string[] strTableNames = strTableName.Split(';');

                int k = 0;
                //DataRow dRow = dt.NewRow();
                for (int i = 0; i < strArrColumns.Length; i++)
                {
                    DataTable dt = new DataTable();
                    DataRow dRow = dt.NewRow();
                    string[] strdtColumns = strArrColumns[i].Split(',');
                    string[] strdtParametres = strArrParameters[i].Split(',');
                    dt.TableName = strTableNames[i];
                    //DataRow dRow1 = dtnew.NewRow();
                    for (int j = 0; j < strdtColumns.Length; j++)
                    {
                        dt.Columns.Add(strdtColumns[j]);
                        if (k < strdtParametres.Length)
                        {
                            string strColumnName = strdtParametres[k];
                            dRow[dt.Columns[j]] = strdtParametres[k];
                            if (dt.Rows.Count == 0)
                            {
                                dt.Rows.Add(dRow);
                            }
                            dt.AcceptChanges();
                            //i--;
                        }
                        k++;

                    }

                    k = 0;

                    ds.Merge(dt);
                    dt.Clear();

                }
                return ds.GetXml();
                //dt.TableName = "Failure and Invoice";
                //////////////////////////////////////////////
                //dt.TableName = "TBLDTCFAILURE";

            }

            catch (Exception ex)
            {
                string strfailure = string.Empty;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return strfailure;
                //return ds;
            }
        }
    }
}
 


