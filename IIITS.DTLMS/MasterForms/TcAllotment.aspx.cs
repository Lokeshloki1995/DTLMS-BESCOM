using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace IIITS.DTLMS.MasterForms
{
    public partial class TcAllotement : System.Web.UI.Page
    {
        public string strFormCode = "TcAllotement";
        clsSession objSession;
        string sFileServerPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["EstimatioinVirtualPath"]);
        string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
        string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
            {
                Response.Redirect("~/Login.aspx", false);
            }
            else
            {
                objSession = (clsSession)Session["clsSession"];
                txtALTDate.Attributes.Add("readonly", "readonly");

                ALTCalendar.EndDate = System.DateTime.Now;

                if (!IsPostBack)
                {
                    LoadSearchWindow();
                   // LoadComboField();
                    if (Request.QueryString["QryAltid"] != null && Request.QueryString["QryAltid"].ToString() != "")
                    {
                        string Alt_id = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryAltid"]));
                        string Alt_No = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryAltNo"]));
                        LoadAllotedIst(Alt_No);
                        Create.Visible = true;
                        CreateDI.Visible = true;
                        //lnkDwnld.Visible = true;
                        // UpdateDI.Visible = true;                       
                        fupFile.Visible = true;
                        btnSave.Visible = false;
                        btnUpdate.Visible = true;
                    }
                    if (Request.QueryString["QryDiNo"] != null && Request.QueryString["QryDiNo"].ToString() != "")
                    {
                        txtDINumber.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryDiNo"]));
                        DataTable dt = new DataTable();
                        clsAllotement obj = new clsAllotement();
                        obj.sDINo = txtDINumber.Text;
                        //cmdSearch_Click(sender, e);
                        dt = obj.GetDeliveryDetails(obj);
                        if (dt.Rows.Count > 0)
                        {
                            ViewState["TOTALTC"] = dt;
                            cmdAdd.Enabled = true;
                        }
                        txtDINumber.Text = Convert.ToString(dt.Rows[0]["DI_NO"]);
                        obj.sDINo = txtDINumber.Text;
                        obj.GetDispatchCount(obj);
                        txtTotalQuantity.Text = obj.sTotalTC;
                        LoadComboField();
                        BindgridView(sFileServerPath, sUserName, sPassword);
                        BindAllotdocs(sFileServerPath, sUserName, sPassword);
                        grdDIPendingTC.DataSource = dt;
                        grdDIPendingTC.DataBind();
                    }
                }
            }
        }
        public void LoadComboField()
        {
            try
            {
               

                Genaral.Load_Combo("select \"SM_ID\",\"SM_NAME\"   from \"TBLSTOREMAST\" ,\"TBLDELIVERYINSTRUCTION\" WHERE   \"SM_ID\"=\"DI_STORE_ID\" AND  \"DI_NO\"='" + txtDINumber.Text + "' and \"DI_STATUS\"=1  GROUP BY \"SM_ID\",\"SM_NAME\"", "--Select--", cmbStore);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TM_ID\",\"TM_NAME\" FROM \"TBLTRANSMAKES\" ,\"TBLDELIVERYINSTRUCTION\" WHERE \"TM_ID\"=\"DI_MAKE_ID\" AND  \"DI_NO\"='" + txtDINumber.Text + "'  AND \"DI_STORE_ID\"="+cmbStore.SelectedValue+" and \"DI_STATUS\"=1 GROUP BY \"TM_ID\",\"TM_NAME\"";

                Genaral.Load_Combo(strQry, "-Select-", cmbMake);
                Genaral.Load_Combo(" select  \"DIV_ID\",\"DIV_NAME\"  from \"TBLSTOREOFFCODE\", \"TBLSTOREMAST\",\"TBLDIVISION\",\"TBLDELIVERYINSTRUCTION\"  WHERE \"STO_SM_ID\"=\"SM_ID\" AND \"STO_OFF_CODE\"=\"DIV_CODE\"  AND  \"SM_ID\"=" + cmbStore.SelectedValue + " and \"DI_STATUS\"=1 AND  \"DI_NO\"='" + txtDINumber.Text + "' GROUP BY  \"DIV_ID\",\"DIV_NAME\" ", "--Select--", cmbDiv);
                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbMake_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\",\"TBLDELIVERYINSTRUCTION\" WHERE \"MD_TYPE\"='C' AND \"DI_CAPACITY_ID\"=\"MD_ID\" AND \"DI_NO\"='" + txtDINumber.Text + "' AND \"DI_STORE_ID\"=" + cmbStore.SelectedValue + " and \"DI_STATUS\"=1 and \"DI_MAKE_ID\"="+ cmbMake.SelectedValue +"  GROUP BY \"MD_ID\",\"MD_NAME\"", "--Select--", cmbCapacity);
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbCapacity_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\",\"TBLDELIVERYINSTRUCTION\" WHERE \"MD_TYPE\"='SR' AND \"DI_STARTTYPE\"=\"MD_ID\" AND \"DI_NO\"='" + txtDINumber.Text + "' AND \"DI_STORE_ID\"=" + cmbStore.SelectedValue + " and \"DI_STATUS\"=1 AND \"DI_CAPACITY\" =" + cmbCapacity.SelectedItem.Text + " and \"DI_MAKE_ID\"=" + cmbMake.SelectedValue + " GROUP BY \"MD_ID\",\"MD_NAME\"", "-Select-", cmbRating);
               
                                
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbRating_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                clsAllotement objAllot = new clsAllotement();
                objAllot.sCapacity = cmbCapacity.SelectedItem.Text;
                objAllot.sDINo = txtDINumber.Text;
                objAllot.sStoreId = cmbStore.SelectedValue;
                objAllot.sMakeId = cmbMake.SelectedValue;
                objAllot.sRatingId = cmbRating.SelectedValue;
                dt = objAllot.GetPendingTc(objAllot);
                ViewState["PENDING"] = dt;
                string count = Convert.ToString(dt.Rows[0]["PENDING"]);
                lblQuntity.Text = "==> Selected Capacity   " + count + " No.Of TC`s Available In Store ";

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void LoadAllotedIst(string Alt_No)
        {
            try
            {

                clsAllotement objAllotment= new clsAllotement();
                DataTable dt = new DataTable();

                objAllotment.sALTNumber = Alt_No;
                dt = objAllotment.GetAllotedDetails(Alt_No);
                ViewState["ALTCapacity"] = dt;
                txtALTId.Text = Convert.ToString(dt.Rows[0]["ALT_ID"]);
                txtALTId.Visible = false;
                txtALTNumber.Text = Convert.ToString(dt.Rows[0]["ALT_NO"]);
                txtALTNumber.Enabled = false;
                grdAllotement.DataSource = dt;
                grdAllotement.DataBind();

                //txtALTDate.Text = Convert.ToString(dt.Rows[0]["ALT_DATE"]);                
                //cmbStore.SelectedValue = Convert.ToString(dt.Rows[0]["ALT_STORE_ID"]);
                //txtQuantity.Text = Convert.ToString(dt.Rows[0]["ALT_QUANTITY"]);
                //cmbCapacity.SelectedValue = Convert.ToString(dt.Rows[0]["ALT_CAPACITY_ID"]);
                 
                //cmbRating.SelectedValue = Convert.ToString(dt.Rows[0]["ALT_STAR_TYPE"]);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public DataTable BindgridView(string FtpServer, string username, string password)
        {
            DataTable dtFiles = new DataTable();
            try
            {
                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);

                string DiNo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");
                FtpServer += "/DI_DOCS/" + DiNo;
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                //path for get files from ftp
                bool IsExists = objFtp.FtpDirectoryExists(FtpServer);
                // checking related ponumber directory is there are not!
                if (IsExists == false)
                {
                    gvFiles.Visible = false;
                    return dtFiles;
                }
                else
                {
                    dtFiles = objFtp.GetListOfFiles(FtpServer);
                }
                gvFiles.DataSource = dtFiles;
                gvFiles.DataBind();

                return dtFiles;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtFiles;
            }
        }
        public DataTable BindAllotdocs(string FtpServer, string username, string password)
        {
            DataTable dtFiles = new DataTable();
            try
            {
                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);

                string AltNo = Regex.Replace(txtALTNumber.Text, @"[^0-9a-zA-Z]+", "");
                FtpServer += "/ALLOTEMENT_DOCS/" + AltNo;
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                //path for get files from ftp
                bool IsExists = objFtp.FtpDirectoryExists(FtpServer);
                // checking related ponumber directory is there are not!
                if (IsExists == false)
                {
                    grdDIdocs.Visible = false;
                    return dtFiles;
                }
                else
                {
                    dtFiles = objFtp.GetListOfFiles(FtpServer);
                }
                grdDIdocs.DataSource = dtFiles;
                grdDIdocs.DataBind();

                return dtFiles;


            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtFiles;
            }
        }
        bool ValidateForm()
        {

            bool bValidate = false;
            if (ViewState["ALTCapacity"] == null)
            {
                if (txtALTNumber.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Allotement Number");
                    txtDINumber.Focus();
                    return bValidate;
                }
                if (txtDINumber.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Delivery Number");
                    txtDINumber.Focus();
                    return bValidate;
                }
                if (txtALTDate.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Allotment Date");
                    txtALTDate.Focus();
                    return bValidate;
                }
               
                if (cmbStore.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Store");
                    cmbStore.Focus();
                    return bValidate;
                }
                if (cmbDiv.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Division");
                    cmbDiv.Focus();
                    return bValidate;
                }
                if (cmbCapacity.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Capacity");
                    cmbCapacity.Focus();
                    return bValidate;
                }
                if (txtQuantity.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Quantity");
                    txtQuantity.Focus();
                    return bValidate;
                }
                if (txtQuantity.Text.Trim() == "0")
                {
                    ShowMsgBox("Quantity Should be Greater than Zero ");
                    txtQuantity.Focus();
                    return bValidate;
                }
                if (cmbRating.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Rating");
                    cmbRating.Focus();
                    return bValidate;
                }


            }
            bValidate = true;
            return bValidate;
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        protected void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["ALTCapacity"] != null)
                {

                    string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                    string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);

                    clsAllotement obj = new clsAllotement();
                    clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                    // clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
                    obj.sCrby = objSession.UserId;
                    obj.dtAllotement = (DataTable)ViewState["ALTCapacity"];
                    obj.sALTNumber = Convert.ToString(obj.dtAllotement.Rows[0]["ALT_NO"]);
                    obj.sALTNumber = Regex.Replace(obj.sALTNumber, @"[^0-9a-zA-Z]+", "");

                    bool Isuploaded;
                    bool IsFileExiest;
                    string sMainFolderName = "ALLOTEMENT_DOCS";
                    string[] sArr = new string[2];

                    if (Session["FileUpload"] != null && (!fupFile.HasFile))
                    {
                        fupFile = (FileUpload)Session["FileUpload"];
                        lblFilename.Text = fupFile.FileName;
                    }
                    else
                    {
                        if (fupFile.PostedFile.FileName != null && fupFile.PostedFile.FileName != "")
                        {
                            fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" + fupFile.FileName));
                        }
                        else
                        {
                            ShowMsgBox("Please upload the Allotment Note ");
                            fupFile.Focus();
                            return;
                        }

                    }

                    string strpath = System.IO.Path.GetExtension(fupFile.FileName);

                    string filename = Path.GetFileName(fupFile.PostedFile.FileName);

                    obj.sFileExt = sFileServerPath + sMainFolderName + "/" + obj.sALTNumber + "/" + filename;

                    if (ValidateForm() == true)
                    {
                        if (fupFile.PostedFile.FileName != null && fupFile.PostedFile.FileName != "")
                        {
                            sArr = obj.SaveDeliveryDetails(obj);
                        }
                        else
                        {
                            ShowMsgBox("Please upload the Allotment Note ");
                            return;
                        }
                    }

                    if (sArr[1].ToString() == "0")
                    {
                        if (fupFile.PostedFile.ContentLength != 0)
                        {

                            string strExt = filename.Substring(filename.LastIndexOf('.') + 1);


                            string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FileFormat"]);
                            string sAnxFileExt = System.IO.Path.GetExtension(fupFile.FileName).ToString().ToLower();
                            sAnxFileExt = ";" + sAnxFileExt.Remove(0, 1) + ";";

                            if (!sFileExt.Contains(sAnxFileExt))
                            {
                                ShowMsgBox("Invalid File Format");
                                return;
                            }


                            string sDirectory = Server.MapPath("~/DTLMSDocs" + "/" + filename);



                            if (File.Exists(sDirectory))
                            {

                                bool IsExists = objFtp.FtpDirectoryExists(sFileServerPath + "/" + sMainFolderName);
                                if (IsExists == false)
                                {

                                    objFtp.createDirectory(sFileServerPath + "/" + sMainFolderName);
                                }
                                IsExists = objFtp.FtpDirectoryExists(sFileServerPath + "/" + sMainFolderName + "/" + obj.sALTNumber);
                                if (IsExists == false)
                                {
                                    objFtp.createDirectory(sFileServerPath + "/" + sMainFolderName + "/" + obj.sALTNumber);
                                }
                                // IsFileExiest = objFtp.IsfileExiest(sMainFolderName + "/" + cmbDivision.Text + "/" + cmbRepairer.Text);
                                //if (IsFileExiest == false)
                                //{
                                Isuploaded = objFtp.upload(sFileServerPath + "/" + sMainFolderName + "/" + obj.sALTNumber, filename, sDirectory);
                                if (Isuploaded == true & File.Exists(sDirectory))
                                {
                                    File.Delete(sDirectory);
                                    sDirectory = obj.sALTNumber + "/" + filename;

                                    ShowMsgBox(sArr[0]);


                                }
                            }


                        }
                        Session["FileUpload"] = null;
                        btnSave.Enabled = false;
                        ShowMsgBox(sArr[0]);
                        Reset();
                        grdAllotement.DataSource = null;
                        grdAllotement.DataBind();
                        cmdSearch_Click(sender, e);
                        return;

                    }

                    else
                    {
                        ShowMsgBox(sArr[0]);
                        return;
                    }

                    if (objSession.sTransactionLog == "1")
                    {
                        Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " DeliveryInstruction Master ");
                    }
                }
                else
                {
                    ShowMsgBox("Please ADD Allotment Details");
                    return;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                clsAllotement Allot = new clsAllotement();
                 
                if (validate() == false)
                {
                    return;
                }

                string sAltid = txtALTId.Text;
                string sAltNo = txtALTNumber.Text;
                string sAltDate = txtALTDate.Text;
                string sDiNo = txtDINumber.Text;
                string sMakeId = cmbMake.SelectedValue;
                string sMakeName = Convert.ToString(cmbMake.SelectedItem);
                string sStoreId = cmbStore.SelectedValue;
                string sStoreName = Convert.ToString(cmbStore.SelectedItem);
                string sDivId = cmbDiv.SelectedValue;
                string sDivName = Convert.ToString(cmbDiv.SelectedItem);
                string sCapacity = Convert.ToString(cmbCapacity.SelectedItem);
                string sCapacityID = cmbCapacity.SelectedValue;
                string sQuantity = txtQuantity.Text;
                string sRatingID = cmbRating.SelectedValue;
                string sRating = Convert.ToString(cmbRating.SelectedItem);
                int pending = 0;
                int oldQuantity = 0;
                int AvailQuantity = 0;
                int totalQunty = 0;

                DataTable dtpending = (DataTable)ViewState["TOTALTC"];
                DataTable InwardCont;

                Allot.sALTNumber = sAltNo.ToUpper();
                Allot.sALTid = sAltid;
                if (Allot.sALTid != null && Allot.sALTid != "")
                {
                    Allot.sCapacity = sCapacity;
                    Allot.sMakeId = sMakeId;
                    Allot.sStoreId = sStoreId;
                    Allot.sDivId = sDivId;
                    InwardCont = Allot.GetInwardedCount(Allot);
                    if (InwardCont.Rows.Count > 0)
                    {
                        int delivered = Convert.ToInt32(InwardCont.Rows[0]["INWARDED"]);
                        int Quantty = Convert.ToInt32(txtQuantity.Text);

                        if (Quantty < delivered)
                        {
                            string Msg = Convert.ToString(delivered);
                            ShowMsgBox("This Capacity Already Inwarded To Some Division  Quantity " + Msg + ", So You Can`t Reduce The Count Below  :" + Msg);
                            InwardCont = null;
                            return;
                        }
                    }
                }
                for (int i = 0; i < dtpending.Rows.Count; i++)
                {
                    if (Convert.ToString(dtpending.Rows[i]["DI_CAPACITY"]) == cmbCapacity.SelectedItem.Text && cmbMake.SelectedItem.Text == Convert.ToString(dtpending.Rows[i]["MAKE_NAME"]) && cmbStore.SelectedItem.Text == Convert.ToString(dtpending.Rows[i]["STORE_NAME"])
                        &&  cmbRating.SelectedItem.Text == Convert.ToString(dtpending.Rows[i]["STAR_RATE"]))
                    {
                        pending = Convert.ToInt32(dtpending.Rows[i]["PENDING"]);
                        totalQunty = Convert.ToInt32(dtpending.Rows[i]["TOTAL"]);
                    }
                    else
                     {
                        continue;
                     }
                     if (sAltid == "")
                        {
                            if (Convert.ToInt32(sQuantity) > Convert.ToInt32(pending))
                            {
                                ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity of " + pending + " ");
                                return;
                            }
                        }
                        else
                        {
                            if (ViewState["ALTCapacity"] != null)
                            {
                                DataTable dtQunty = (DataTable)ViewState["ALTCapacity"];

                                for (int j = 0; j < dtQunty.Rows.Count; j++)
                                {
                                    if (Convert.ToString(dtQunty.Rows[j]["ALT_CAPACITY"]) == cmbCapacity.SelectedItem.Text && cmbMake.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["MAKE_NAME"]) && cmbStore.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["ALT_STORE_NAME"])
                                         && cmbRating.SelectedValue == Convert.ToString(dtQunty.Rows[j]["ALT_STAR_TYPE"]))
                                    {
                                        oldQuantity += Convert.ToInt32(dtQunty.Rows[j]["ALT_QUANTITY"]);
                                    }
                                }
                                AvailQuantity = (Convert.ToInt32(totalQunty) - Convert.ToInt32(oldQuantity));

                                if (Convert.ToInt32(sQuantity) > AvailQuantity)
                                {
                                    ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity of " + AvailQuantity + " ");
                                    return;
                                }

                            }
                            break;
                        }                   
                    if (ViewState["ALTCapacity"] != null)
                    {
                        DataTable dtQunty = (DataTable)ViewState["ALTCapacity"];
                        for (int j = 0; j < dtQunty.Rows.Count; j++)
                        {
                            if (Convert.ToString(dtQunty.Rows[j]["ALT_CAPACITY"]) == cmbCapacity.SelectedItem.Text && cmbMake.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["MAKE_NAME"]) && cmbStore.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["ALT_STORE_NAME"])
                                 && cmbRating.SelectedValue == Convert.ToString(dtQunty.Rows[j]["ALT_STAR_TYPE"]))
                            {
                                oldQuantity += Convert.ToInt32(dtQunty.Rows[j]["ALT_QUANTITY"]);
                            }
                        }
                        AvailQuantity = (Convert.ToInt32(pending) - Convert.ToInt32(oldQuantity));

                        if (Convert.ToInt32(sQuantity) > AvailQuantity)
                        {
                            ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity of " + AvailQuantity + " ");
                            return;
                        }

                    }
                }
                if (Session["FileUpload"] == null && fupFile.HasFile)
                {

                    Session["FileUpload"] = fupFile;
                     lblFilename.Text = fupFile.FileName; // get the name   
                     fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" +  fupFile.FileName));
                    fupFile = (FileUpload)Session["FileUpload"];
                    //lblFilename.Text = fupdDoc.FileName;
                }
                else if (Session["FileUpload"] != null && (!fupFile.HasFile))
                {
                     
                    fupFile = (FileUpload)Session["FileUpload"];
                    lblFilename.Text = fupFile.FileName;
                   
                }
                else if (fupFile.HasFile)
                {
                    fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" +  fupFile.FileName));
                    Session["FileUpload"] = fupFile;                            
                    lblFilename.Text = fupFile.FileName;
                }

                if (ViewState["ALTCapacity"] != null)
                {

                    DataTable dtCap = (DataTable)ViewState["ALTCapacity"];
                    for (int i = 0; i < dtCap.Rows.Count; i++)
                    {
                        if (sAltNo == Convert.ToString(dtCap.Rows[i]["ALT_NO"]) && sStoreId == Convert.ToString(dtCap.Rows[i]["ALT_STORE_ID"]) && sRatingID == Convert.ToString(dtCap.Rows[i]["ALT_STAR_TYPE"])
                            && sCapacity == Convert.ToString(dtCap.Rows[i]["ALT_CAPACITY"]) && sDivName == Convert.ToString(dtCap.Rows[i]["DIV_NAME"]))
                        {
                            //ShowMsgBox("Capacity("+ dt.Rows[i]["PB_CAPACITY"] + ")-MakeName(" + dt.Rows[i]["PB_MAKE"] + ") Combination Already Added");
                            ShowMsgBox("  Store- Star Rate - Capacity - Start Rate Combination Already Added");
                            return;
                        }
                         oldQuantity = Convert.ToInt32(dtCap.Rows[i]["ALT_QUANTITY"]);

                        if (Convert.ToInt32(sQuantity) > (Convert.ToInt32(pending) + Convert.ToInt32(oldQuantity)))
                        {
                            ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity  " + (Convert.ToInt32(pending) + Convert.ToInt32(oldQuantity)) + " ");
                            return;
                        }

                    }
                }
               
                DataTable dtTotalTC = new DataTable();
                dtTotalTC = (DataTable)ViewState["TOTALTC"];
              
                DataTable dt = new DataTable();
                if (ViewState["ALTCapacity"] == null)
                { 
                    dt.Columns.Add("ALT_ID");
                    dt.Columns.Add("ALT_DIV_ID");
                    dt.Columns.Add("ALT_NO");
                    dt.Columns.Add("ALT_DI_NO");
                    dt.Columns.Add("ALT_DATE");
                    dt.Columns.Add("ALT_MAKE_ID");
                    dt.Columns.Add("DIV_NAME");
                    dt.Columns.Add("MAKE_NAME");
                    dt.Columns.Add("ALT_STORE_ID");
                    dt.Columns.Add("ALT_STORE_NAME");
                    dt.Columns.Add("ALT_CAPACITY");
                    dt.Columns.Add("ALT_CAPACITY_ID");
                    dt.Columns.Add("ALT_STAR_TYPE");
                    dt.Columns.Add("ALT_STARRATENAME");  
                    dt.Columns.Add("ALT_QUANTITY");
                  //  dt.Columns.Add("ALT_FILE_PATH");
                }
                else
                {
                    //load datatble from viewstate
                    dt = (DataTable)ViewState["ALTCapacity"];
                }

                string Id = "";
                DataRow dRow = dt.NewRow();

                byte[] BufferImg = null;
                string strExt = string.Empty;
                if (fupFile.PostedFile.ContentLength != 0)
                {
                    string filename = Path.GetFileName(fupFile.PostedFile.FileName);
                    strExt = filename.Substring(filename.LastIndexOf('.') + 1);
                    strExt = Path.GetExtension(fupFile.PostedFile.FileName);
                    string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FileFormat"]);
                    string sAnxFileExt = System.IO.Path.GetExtension(fupFile.FileName).ToString().ToLower();
                    sAnxFileExt = ";" + sAnxFileExt.Remove(0, 1) + ";";

                    if (!sFileExt.Contains(sAnxFileExt))
                    {
                        ShowMsgBox("Invalid File Format");
                        return;
                    }
                    strExt = Path.GetExtension(fupFile.PostedFile.FileName);
                    Stream strm = fupFile.PostedFile.InputStream;
                    BufferImg = new byte[strm.Length];
                    strm.Read(BufferImg, 0, (int)strm.Length);
                }
                
                if (sAltid == "")
                {
                    dRow["ALT_ID"] = Id;
                    dRow["ALT_DI_NO"] = sDiNo;
                    dRow["ALT_NO"] = sAltNo.ToUpper();
                    dRow["ALT_DATE"] = sAltDate;
                    dRow["ALT_MAKE_ID"] = sMakeId;
                    dRow["MAKE_NAME"] = sMakeName; 
                    dRow["ALT_STORE_ID"] = sStoreId;
                    dRow["ALT_STORE_NAME"] = sStoreName;
                    dRow["ALT_DIV_ID"] = sDivId;
                    dRow["DIV_NAME"] = sDivName;   
                    dRow["ALT_CAPACITY"] = sCapacity;
                    dRow["ALT_CAPACITY_ID"] = sCapacityID;
                    dRow["ALT_STAR_TYPE"] = sRatingID;
                    dRow["ALT_STARRATENAME"] = sRating; 
                    dRow["ALT_QUANTITY"] = sQuantity;
                   // dRow["ALT_FILE_PATH"] = strExt;
                    dt.Rows.Add(dRow);
                    dt.AcceptChanges();
                    ViewState["ALTCapacity"] = dt;
                    LoadCapacity(dt);                     
                    return;
                }
                else
                {
                    dRow["ALT_ID"] = sAltid;
                    dRow["ALT_DI_NO"] = sDiNo;
                    dRow["ALT_NO"] = sAltNo.ToUpper();
                    dRow["ALT_DATE"] = sAltDate;
                    dRow["ALT_MAKE_ID"] = sMakeId;
                    dRow["MAKE_NAME"] = sMakeName;
                    dRow["ALT_STORE_ID"] = sStoreId;
                    dRow["ALT_STORE_NAME"] = sStoreName;
                    dRow["ALT_DIV_ID"] = sDivId;
                    dRow["DIV_NAME"] = sDivName;   
                    dRow["ALT_CAPACITY"] = sCapacity;
                    dRow["ALT_CAPACITY_ID"] = sCapacityID;
                    dRow["ALT_STAR_TYPE"] = sRatingID;
                    dRow["ALT_STARRATENAME"] = sRating;
                    dRow["ALT_QUANTITY"] = sQuantity;
                  //  dRow["ALT_FILE_PATH"] = strExt;
                    dt.Rows.Add(dRow);
                    dt.AcceptChanges();
                    ViewState["ALTCapacity"] = dt;
                    LoadCapacity(dt);                  
                    return;
                }
                    #region//old row Edit data
                    //if (ViewState["gridRowId"] != null)
                //{
                //    dt = (DataTable)ViewState["ALTCapacity"];
                //    // gridid.Text = ViewState["gridRowId"].ToString();
                //    int i = Convert.ToInt32(ViewState["gridRowId"]);

                //    dt.Rows[i].SetField("ALT_ID", sAltid);
                //    dt.Rows[i].SetField("ALT_NO", sAltNo.ToUpper());
                //    dt.Rows[i].SetField("ALT_DI_NO", sDiNo);
                //    dt.Rows[i].SetField("ALT_DATE", sAltDate);
                //    dt.Rows[i].SetField("ALT_MAKE_ID", sMakeId);
                //    dt.Rows[i].SetField("MAKE_NAME", sMakeName);
                //    dt.Rows[i].SetField("ALT_STORE_ID", sStoreId);
                //    dt.Rows[i].SetField("ALT_STORE_NAME", sStoreName);
                //    dt.Rows[i].SetField("ALT_DIV_ID", sDivId);
                //    dt.Rows[i].SetField("ALT_CAPACITY", sCapacity);
                //    dt.Rows[i].SetField("ALT_CAPACITY_ID", sCapacityID);
                //    dt.Rows[i].SetField("ALT_STAR_TYPE", sRatingID);
                //    dt.Rows[i].SetField("ALT_STARRATENAME", sRating);
                //    dt.Rows[i].SetField("ALT_QUANTITY", sQuantity);
                //    //dt.Rows[i].SetField("DI_FILE_EXT", fupFile.FileName);
                //    ViewState["gridRowId"] = null;
                //    ViewState["ALTCapacity"] = dt;
                //    LoadCapacity(dt);
                //}
                //else
                //{
                //    dt.Rows.Add(dRow);
                //    dt.AcceptChanges();
                //    ViewState["ALTCapacity"] = dt;
                //    LoadCapacity(dt);
                    //}
                    #endregion
                

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        private void ShowMsgBox(string sMsg)
        {
            try
            {
                string sShowMsg = string.Empty;
                sShowMsg = "<script language=javascript> alert ('" + sMsg + "')</script>";
                this.Page.RegisterStartupScript("Msg", sShowMsg);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("TcAllotmentView.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        
        protected void ResetGrid()
        {
            DataTable dt = (DataTable)ViewState["ALTCapacity"];
            if (dt.Rows.Count > 0)
            {
                int counter = 0;
                foreach (DataRow row1 in dt.Rows)
                {
                    counter++;
                    row1["ID"] = counter;
                }
                ViewState["ALTCapacity"] = dt;
            }
            else
            {
                ViewState["ALTCapacity"] = null;
            }
            dt = (DataTable)ViewState["ALTCapacity"];
            grdAllotement.DataSource = dt;
            grdAllotement.DataBind();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {

                clsAllotement obj = new clsAllotement();
                string temp = txtDINumber.Text;
                txtDINumber.Text = temp.ToUpper();
                obj.sDINo = temp.ToUpper();
                obj.GetDispatchCount(obj);
                txtTotalQuantity.Text = obj.sTotalTC;
                BindgridView(sFileServerPath, sUserName, sPassword);
                //txtDiId.Text = obj.GetPOId(obj);
                DataTable dt = new DataTable();
                dt = obj.GetDeliveryDetails(obj);
                if (dt.Rows.Count > 0)
                {
                    ViewState["TOTALTC"] = dt;
                    cmdAdd.Enabled = true;
                }
                grdDIPendingTC.DataSource = dt;
                grdDIPendingTC.DataBind();
                LoadComboField();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        private void Reset()
        {
            try
            {
               // txtALTNumber.Text = "";
                txtALTDate.Text = "";
                cmbStore.SelectedIndex = 0;
                cmbDiv.SelectedIndex = 0;
                cmbCapacity.ClearSelection(); 
                txtQuantity.Text = "";
                cmbRating.ClearSelection(); 
                if(!(cmbMake.SelectedIndex < 0))
                {
                    cmbMake.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected bool validate()
        {
            bool Status = false;
            try
            {
                if (txtALTNumber.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Allotement Number");
                    txtDINumber.Focus();
                    return Status;
                }
                if (txtALTDate.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Allotement Date");
                    txtALTDate.Focus();
                    return Status;
                }
               
                if (cmbStore.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Store");
                    cmbStore.Focus();
                    return Status;
                }
                if (cmbDiv.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Division");
                    cmbDiv.Focus();
                    return Status;
                }
                 
                if (cmbCapacity.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Capacity");
                    cmbCapacity.Focus();
                    return Status;
                }
                if (txtQuantity.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Quantity");
                    txtQuantity.Focus();
                    return Status;
                }
                if (cmbRating.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Rating");
                    cmbRating.Focus();
                    return Status;
                }
                Status = true;
                return Status;
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Status;
            }
        }
        protected void DownloadFile1(object sender, EventArgs e)
        {

            string fileName = (sender as LinkButton).CommandArgument;

            try
            {
                string ALTNo = Regex.Replace(txtALTNumber.Text, @"[^0-9a-zA-Z]+", "");
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/ALLOTEMENT_DOCS/" + ALTNo + "/" + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(sUserName, sPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }

        }
        public void LoadSearchWindow()
        {
            try
            {
                string strQry = string.Empty;
                strQry = "Title=Search and Select  Dispatch Instructions Details&";
                strQry += "Query=SELECT  \"DI_NO\"  FROM \"TBLDELIVERYINSTRUCTION\" ";
                strQry += "WHERE {0} like %{1}% AND \"DI_STATUS\"=1  group by \"DI_NO\" &";
                strQry += "DBColName=\"DI_NO\" &";
                strQry += "ColDisplayName=DI Number &";
                strQry = strQry.Replace("'", @"\'");
                cmdSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtDINumber.ClientID + "&btn=" + cmdSearch.ClientID + "',520,520," + txtDINumber.ClientID + ")");

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void DownloadFile(object sender, EventArgs e)
        {

            string fileName = (sender as LinkButton).CommandArgument;

            try
            {
                string DINo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/DI_DOCS/" + DINo + "/" + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(sUserName, sPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }

        }
        protected void grdAllotement_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            clsAllotement Allot = new clsAllotement();
            try
            {
                if (e.CommandName == "Remove")
                {
                    DataTable dt = (DataTable)ViewState["ALTCapacity"];
                    DataTable InwardCont;
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int sRowIndex = row.RowIndex;
                    Label lblId = (Label)row.FindControl("lblId");
                    Label lblAltDINo = (Label)row.FindControl("lblAltDINo");
                    Label lblAltNo = (Label)row.FindControl("lblAltNo");
                    Label lblAltDate = (Label)row.FindControl("lblAltDate");
                    Label lblDivId = (Label)row.FindControl("lblDivId");
                    Label lblStoreId = (Label)row.FindControl("lblStoreId");
                    Label lblMakeId = (Label)row.FindControl("lblMakeId");
                    Label lblCapacity = (Label)row.FindControl("lblCapacity");
                    Label lblRating = (Label)row.FindControl("lblRating");
                    Label lblQuantity = (Label)row.FindControl("lblQuantity");

                    Allot.sALTNumber = lblAltNo.Text;
                    Allot.sCapacity = lblCapacity.Text;
                    Allot.sMakeId = lblMakeId.Text;
                    Allot.sStoreId = lblStoreId.Text;
                    Allot.sDivId = lblDivId.Text;
                    InwardCont = Allot.GetInwardedCount(Allot);
                    // TO Check Before Deleting Whether Allotment No Inwarded or not 
                    if (InwardCont.Rows.Count > 0)
                    {
                        int delivered = Convert.ToInt32(InwardCont.Rows[0]["INWARDED"]);

                        string Msg = Convert.ToString(delivered);
                        ShowMsgBox("This Capacity Already Inwarded To Some Division  Quantity " + Msg + ", So You Can`t Delete This Record !");
                        InwardCont = null;
                        return;
                    }
                    else
                    {
                        // To Remove Row Data
                        dt.Rows[sRowIndex].Delete();
                        dt.AcceptChanges();          
                    }
                              
                    if (dt.Rows.Count > 0)
                    {                      
                        ViewState["ALTCapacity"] = dt;
                    }
                    else
                    {
                        ViewState["ALTCapacity"] = null;
                    }
                    grdAllotement.DataSource = dt;
                    grdAllotement.DataBind();
                    grdAllotement.Visible = true;
                                                      
                }
                
                 if (e.CommandName == "EditQNTY")
                {
                        DataTable dt = (DataTable)ViewState["ALTCapacity"];      
                        GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        //clsPoMaster objPoMast = new clsPoMaster();
                        //objPoMast.sRowIndex = row.RowIndex;
                        int sRowIndex = row.RowIndex;

                        Label lblId = (Label)row.FindControl("lblId");
                        Label lblAltDINo = (Label)row.FindControl("lblAltDINo");
                        Label lblAltNo = (Label)row.FindControl("lblAltNo");
                        Label lblAltDate = (Label)row.FindControl("lblAltDate");
                        Label lblDivId = (Label)row.FindControl("lblDivId");
                        Label lblStoreId = (Label)row.FindControl("lblStoreId");                      
                        Label lblMakeId = (Label)row.FindControl("lblMakeId");
                        Label lblCapacity = (Label)row.FindControl("lblCapacityId");
                        Label lblRating = (Label)row.FindControl("lblRating");
                        Label lblQuantity = (Label)row.FindControl("lblQuantity");
                                               
                        txtALTNumber.Text = lblAltNo.Text;
                        txtALTId.Text = txtALTId.Text;
                        txtALTId.Visible = false;
                        txtALTDate.Text = lblAltDate.Text;                       
                        txtDINumber.Enabled = false;                
                        cmbMake.SelectedValue = lblMakeId.Text;                        
                        txtQuantity.Text = lblQuantity.Text ;
                        cmbStore.SelectedValue = lblStoreId.Text;
                        cmbStore_SelectedIndexChanged(sender, e);

                        //to remove selected Capacity from grid
                        dt.Rows[sRowIndex].Delete();
                        dt.AcceptChanges();
                        if (dt.Rows.Count > 0)
                        {
                            ViewState["ALTCapacity"] = dt;
                        }
                        else
                        {
                            ViewState["ALTCapacity"] = null;
                        }
                        grdAllotement.DataSource = dt;
                        grdAllotement.DataBind();
                        grdAllotement.Visible = true;
                                                          
               }
                    
            }           
            
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }    

        protected void grdAllotement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dtTcCapacity = new DataTable();
                grdAllotement.PageIndex = e.NewPageIndex;
                dtTcCapacity = (DataTable)ViewState["ALTCapacity"];
                LoadCapacity(dtTcCapacity);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void LoadCapacity(DataTable dt)
        {
            try
            {
                grdAllotement.DataSource = dt;
                grdAllotement.DataBind();
                grdAllotement.Visible = true;
                Reset();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdDIPendingTC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdDIPendingTC.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["TOTALTC"];
                grdDIPendingTC.DataSource = SortDataTable(dt as DataTable, true);
                grdDIPendingTC.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void gvFiles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvFiles.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["dt"];
                gvFiles.DataSource = SortDataTable(dt as DataTable, true);
                gvFiles.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void gvFiles_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridViewSortExpression = e.SortExpression;
            int pageIndex = gvFiles.PageIndex;
            DataTable dt = (DataTable)ViewState["dt"];
            string sortingDirection = string.Empty;

            if (dt.Rows.Count > 0)
            {
                gvFiles.DataSource = SortDataTable(dt as DataTable, false);
            }
            else
            {
                gvFiles.DataSource = dt;
            }
            gvFiles.DataBind();
            gvFiles.PageIndex = pageIndex;
        }
        private string GridViewSortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }


        }
        private string GridViewSortExpression
        {
            get { return ViewState["SortExpression"] as string ?? string.Empty; }
            set { ViewState["SortExpression"] = value; }
        }

        private string GetSortDirection()
        {
            switch (GridViewSortDirection)
            {
                case "ASC":
                    GridViewSortDirection = "DESC";

                    break;
                case "DESC":
                    GridViewSortDirection = "ASC";

                    break;
            }


            return GridViewSortDirection;
        }
        protected DataView SortDataTable(DataTable dataTable, bool isPageIndexChanging)
        {
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                if (GridViewSortExpression != string.Empty)
                {
                    if (isPageIndexChanging)
                    {
                        dataView.Sort = string.Format("{0} {1} ", GridViewSortExpression, GridViewSortDirection);

                        if (Convert.ToString(dataView.Sort) == "Name ASC ")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderBy(r => int.Parse(r.Field<String>("Name")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["dt"] = dataView.ToTable();
                        }
                        else if (Convert.ToString(dataView.Sort) == "Name DESC ")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderByDescending(r => int.Parse(r.Field<String>("Name")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["dt"] = dataView.ToTable();
                        }

                        else
                        {
                            // dataView.Sort = string.Format("{0} {1} ", GridViewSortExpression, GridViewSortDirection);
                            ViewState["dt"] = dataView.ToTable();
                        }
                    }
                    else
                    {

                        dataView.Sort = string.Format("{0} {1} ", GridViewSortExpression, GetSortDirection());

                        if (Convert.ToString(dataView.Sort) == "Name ASC ")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderBy(r => int.Parse(r.Field<String>("Name")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["dt"] = dataView.ToTable();
                        }
                        else if (Convert.ToString(dataView.Sort) == "Name DESC ")
                        {

                            DataTable dt = dataTable.AsEnumerable()
                           .OrderByDescending(r => int.Parse(r.Field<String>("Name")))
                          .CopyToDataTable();

                            dataView = new DataView(dt);
                            ViewState["dt"] = dataView.ToTable();
                        }



                        else
                        {
                            // dataView.Sort = string.Format("{0} {1} ", GridViewSortExpression, GetSortDirection());
                            ViewState["dt"] = dataView.ToTable(); ;


                        }
                    }
                }
                return dataView;
            }
            else
            {
                return new DataView();
            }

        }
    }
}