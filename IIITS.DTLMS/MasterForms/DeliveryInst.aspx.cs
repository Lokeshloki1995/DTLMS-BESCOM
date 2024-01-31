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
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Renci.SshNet;

namespace IIITS.DTLMS.MasterForms
{
    public partial class DeliveryInst : System.Web.UI.Page
    {
        public string strFormCode = "DeliveryInst";
        clsSession objSession;
        string sFileServerPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["EstimatioinVirtualPath"]);
        string sFileUploadloadPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["CircularsVirtualPath"]);

        string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
        string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
        string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
            {
                Response.Redirect("~/Login.aspx", false);
            }
            else
            {
                objSession = (clsSession)Session["clsSession"];
                txtDueDate.Attributes.Add("readonly", "readonly");
                txtDIDate.Attributes.Add("readonly", "readonly");

                ManufactureCalender.EndDate = System.DateTime.Now;
                //DeliveryCalendar.EndDate = System.DateTime.Now;

                if (!IsPostBack)
                {
                    LoadSearchWindow();
        
                    if (Request.QueryString["QryDIid"] != null && Request.QueryString["QryDIid"].ToString() != "")
                    {
                         string DI_id = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryDIid"]));
                         string DI_No = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryDiNo"]));
                         LoadDeliveredIst(DI_No);
                        Create.Visible = true;
                        CreateDI.Visible = true;
                        //lnkDwnld.Visible = true;
                       // UpdateDI.Visible = true;                       
                        fupFile.Visible = true;
                        btnSave.Visible = false;
                        btnUpdate.Visible = true;
                    }
                    if (Request.QueryString["QryPoId"] != null && Request.QueryString["QryPoId"].ToString() != "")
                    {
                          txtPoId.Text = Genaral.UrlDecrypt(HttpUtility.UrlDecode(Request.QueryString["QryPoId"]));
                          DataTable dt = new DataTable();
                          clsDelivery obj = new clsDelivery();
                          obj.sPoId = txtPoId.Text;
                          //cmdSearch_Click(sender, e);
                          dt = obj.GetDeliveryDetails(obj);
                          if (dt.Rows.Count > 0)
                          {
                              ViewState["TOTALTC"] = dt;
                              cmdAdd.Enabled = true;
                          }
                          txtPoNumber.Text = Convert.ToString(dt.Rows[0]["PO_NO"]);
                          obj.sPONumber = txtPoNumber.Text;
                          obj.GetPurchaseCount(obj);
                          txtTotalQuantity.Text = obj.sTotalTC;
                          BindgridView(sFileServerPath, sUserName, sPassword);
                          BindDIdocs(sFileServerPath, sUserName, sPassword);
                          grdPendingTC.DataSource = dt;
                          grdPendingTC.DataBind();
                          LoadComboField();
                    }
                    txtDIDate.Attributes.Add("onblur", "return ValidateDate(" + txtDIDate.ClientID + ");");
                }
            }
            
        }

        public void LoadComboField()
        {
            try
            {
                string strQry = string.Empty;
                strQry = "SELECT \"TM_ID\",\"TM_NAME\" FROM \"TBLTRANSMAKES\",\"TBLPOMASTER\",\"TBLPOOBJECTS\" WHERE \"TM_ID\"=\"PB_MAKE\" AND \"PO_ID\"=\"PB_PO_ID\" AND \"PO_NO\"='" + txtPoNumber.Text + "' and \"PB_PO_STATUS\"=1 GROUP BY \"TM_ID\",\"TM_NAME\"";
                
                Genaral.Load_Combo(strQry, "-Select-", cmbMake);

                Genaral.Load_Combo("SELECT \"SM_ID\", \"SM_NAME\" FROM \"TBLSTOREMAST\" ORDER BY \"SM_NAME\"", "-Select-", cmbStore);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
         protected void cmbCapacity_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\",\"TBLPOMASTER\",\"TBLPOOBJECTS\" WHERE \"MD_TYPE\"='SR' AND \"PO_NO\"='" + txtPoNumber.Text + "' AND \"PO_ID\"=\"PB_PO_ID\" and \"PB_PO_STATUS\"=1  AND CAST(\"MD_ID\" AS TEXT)=\"PO_RATING\" AND \"PB_MAKE\"=" + cmbMake.SelectedValue + "  AND \"PB_CAPACITY\"=" + cmbCapacity.SelectedItem.Text + " GROUP BY \"MD_ID\",\"MD_NAME\" ", "-Select-", cmbRating);
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
                 Genaral.Load_Combo("SELECT \"MD_ID\",\"MD_NAME\" FROM \"TBLMASTERDATA\",\"TBLPOMASTER\",\"TBLPOOBJECTS\" WHERE \"MD_TYPE\"='C' AND \"PO_NO\"='" + txtPoNumber.Text + "' AND \"PO_ID\"=\"PB_PO_ID\"  and \"PB_MAKE\"=" + cmbMake.SelectedValue + " AND \"MD_ID\"=\"PB_CAPACITY_ID\" and \"PB_PO_STATUS\"=1  GROUP BY \"MD_ID\",\"MD_NAME\"", "--Select--", cmbCapacity);
             }
             catch (Exception ex)
             {
                 clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
             }
         }
        public void LoadDeliveredIst(string DI_No)
        {
            try
            {

                clsDelivery objDelivery = new clsDelivery();
                DataTable dt = new DataTable();
                objDelivery.sDINo = DI_No;
                dt = objDelivery.GetDeliveredDetails(DI_No);
                ViewState["DiDetails"] = dt;
                txtDIId.Text = Convert.ToString(dt.Rows[0]["DI_ID"]);
                txtDIId.Visible = false;
                txtDINumber.Text = Convert.ToString(dt.Rows[0]["DI_NO"]);
                txtDINumber.Enabled = false;
                grdDelivery.DataSource = dt;
                grdDelivery.DataBind();

               // txtConsignee.Text = Convert.ToString(dt.Rows[0]["DI_CONSIGNEE"]);
               // txtDueDate.Text = Convert.ToString(dt.Rows[0]["DI_DUEDATE"]);
               // txtDIDate.Text = Convert.ToString(dt.Rows[0]["DI_DATE"]);
               // cmbStore.SelectedValue = Convert.ToString(dt.Rows[0]["DI_STORE_ID"]);
               // cmbMake.SelectedValue = Convert.ToString(dt.Rows[0]["DI_MAKE_ID"]);
               // txtQuantity.Text = Convert.ToString(dt.Rows[0]["DI_QUANTITY"]);
               // cmbCapacity.SelectedValue = Convert.ToString(dt.Rows[0]["DI_CAPACITY_ID"]);
               //// cmbCapacity.Enabled = false;
               // cmbRating.SelectedValue = Convert.ToString(dt.Rows[0]["DI_STARTTYPE"]);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void LoadSearchWindow()
        {
            try
            {
                string strQry = string.Empty;
                strQry = "Title=Search and Select Purchase Order Details&";
                strQry += "Query=SELECT  \"PO_NO\",\"PO_ID\" FROM \"TBLPOMASTER\" ";
                strQry += "WHERE {0} like %{1}% order by \"PO_NO\" &";
                strQry += "DBColName=\"PO_NO\" &";
                strQry += "ColDisplayName=PO Number &";
                strQry = strQry.Replace("'", @"\'");
                cmdSearch.Attributes.Add("onclick", "javascript:return OpenWindow('/SearchWindow.aspx?" + strQry + "tb=" + txtPoNumber.ClientID + "&btn=" + cmdSearch.ClientID + "',520,520," + txtPoNumber.ClientID + ")");
                                
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        //public DataTable BindgridView(string FtpServer, string username, string password)
        //{
        //    DataTable dtFiles = new DataTable();
        //    try
        //    {
        //        int port = 22;
        //        // clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
        //       // SftpClient objFtp = new SftpClient(sFileServerPath, port, sUserName, sPassword);

        //        clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

        //        string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
        //        FtpServer += "/PO_DOCS/" + PoNo;
        //        //  taking ftp server path for get file
        //       // bool IsExists = objFtp.FtpDirectoryExists(FtpServer, sUserName, sPassword);
        //        bool IsExists = objFtp.FtpDirectoryExists(sFileUploadloadPath);
        //        // checking related ponumber directory is there are not!
        //        if (IsExists == false)
        //        {
        //            gvFiles.Visible = false;
        //            return dtFiles;
        //        }

        //        dtFiles.Columns.AddRange(new DataColumn[1] { new DataColumn("Name", typeof(string)) });
        //        WebRequest request = (WebRequest)WebRequest.Create(FtpServer);
        //        request.Credentials = new NetworkCredential(username, password);
        //        request.Method = WebRequestMethods.Ftp.ListDirectory;
        //        StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
        //        string fileName3 = streamReader.ReadLine();

        //        List<string> directories = new List<string>();
        //        while (fileName3 != null && fileName3 != "testing12")
        //        {
        //            dtFiles.Rows.Add(fileName3);
        //            directories.Add(fileName3);
        //            fileName3 = streamReader.ReadLine();
        //        }
        //        streamReader.Close();
        //        using (WebClient ftpClient = new WebClient())
        //        {
        //            ftpClient.Credentials = new System.Net.NetworkCredential(username, password);

        //            for (int i = 0; i < directories.Count; i++)
        //            {
        //                if (directories[i].Contains("."))
        //                {

        //                    // string path = FtpServer + directories[i].ToString();
        //                    //string trnsfrpth = localpath + directories[i].ToString();
        //                    //ftpClient.DownloadFile(path, trnsfrpth);
        //                    gvFiles.DataSource = dtFiles;

        //                }

        //            }
        //            ViewState["PoDocs"] = dtFiles;
        //            gvFiles.DataBind();
        //        }
        //        return dtFiles;
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = clsException.ErrorMsg();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return dtFiles;
        //    }
        //}
        public DataTable BindgridView(string FtpServer, string username, string password)
        {
            DataTable dtFiles = new DataTable();
            try
            {
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
                FtpServer += "/PO_DOCS/" + PoNo;
                //  taking ftp server path for get file
                bool IsExists = objFtp.FtpDirectoryExists(FtpServer);
                // checking related ponumber directory is there are not!
                if (IsExists == false)
                {
                    gvFiles.Visible = false;
                    return dtFiles;
                }

                dtFiles = objFtp.GetListOfFiles(FtpServer);


                if (dtFiles.Rows.Count > 0)
                {
                    gvFiles.DataSource = dtFiles;
                    gvFiles.DataBind();
                }

                //dtFiles.Columns.AddRange(new DataColumn[1] { new DataColumn("Name", typeof(string)) });
                //WebRequest request = (WebRequest)WebRequest.Create(FtpServer);
                //request.Credentials = new NetworkCredential(username, password);
                //request.Method = WebRequestMethods.Ftp.ListDirectory;
                //StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
                //string fileName3 = streamReader.ReadLine();

                //List<string> directories = new List<string>();
                //while (fileName3 != null && fileName3 != "testing12")
                //{
                //    dtFiles.Rows.Add(fileName3);
                //    directories.Add(fileName3);
                //    fileName3 = streamReader.ReadLine();
                //}
                //streamReader.Close();
                //using (WebClient ftpClient = new WebClient())
                //{
                //    ftpClient.Credentials = new System.Net.NetworkCredential(username, password);

                //    for (int i = 0; i < directories.Count; i++)
                //    {
                //        if (directories[i].Contains("."))
                //        {

                //            // string path = FtpServer + directories[i].ToString();
                //            //string trnsfrpth = localpath + directories[i].ToString();
                //            //ftpClient.DownloadFile(path, trnsfrpth);
                //            gvFiles.DataSource = dtFiles;

                //        }

                // }
                ViewState["PoDocs"] = dtFiles;
                gvFiles.DataBind();
                //}
                return dtFiles;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtFiles;
            }
        }

        //public DataTable BindDIdocs(string FtpServer, string username, string password)
        //{
        //    DataTable dtFiles = new DataTable();
        //    try
        //    {
        //        clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
        //        string DiNo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");
        //        FtpServer += "/DI_DOCS/" + DiNo;
        //        //path for get files from ftp
        //        bool IsExists = objFtp.FtpDirectoryExists(FtpServer, sUserName, sPassword);
        //        // checking related ponumber directory is there are not!
        //        if (IsExists == false)
        //        {
        //            grdDIdocs.Visible = false;
        //            return dtFiles;
        //        }

        //        dtFiles.Columns.AddRange(new DataColumn[1] { new DataColumn("Name", typeof(string)) });
        //        WebRequest request = (WebRequest)WebRequest.Create(FtpServer);
        //        request.Credentials = new NetworkCredential(username, password);
        //        request.Method = WebRequestMethods.Ftp.ListDirectory;
        //        StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
        //        string fileName3 = streamReader.ReadLine();

        //        List<string> directories = new List<string>();
        //        while (fileName3 != null && fileName3 != "testing12")
        //        {
        //            dtFiles.Rows.Add(fileName3);
        //            directories.Add(fileName3);
        //            fileName3 = streamReader.ReadLine();
        //        }
        //        streamReader.Close();
        //        using (WebClient ftpClient = new WebClient())
        //        {
        //            ftpClient.Credentials = new System.Net.NetworkCredential(username, password);

        //            for (int i = 0; i < directories.Count; i++)
        //            {
        //                if (directories[i].Contains("."))
        //                {

        //                    // string path = FtpServer + directories[i].ToString();
        //                    //string trnsfrpth = localpath + directories[i].ToString();
        //                    //ftpClient.DownloadFile(path, trnsfrpth);
        //                    grdDIdocs.DataSource = dtFiles;

        //                }

        //            }

        //            ViewState["DiDocs"] = dtFiles;

        //            grdDIdocs.DataBind();
        //        }
        //        return dtFiles;
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = clsException.ErrorMsg();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return dtFiles;
        //    }
        //}

        public DataTable BindDIdocs(string FtpServer, string username, string password)
        {
            DataTable dtFiles = new DataTable();
            try
            {
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                // clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
                string DiNo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");
                FtpServer += "/DI_DOCS/" + DiNo;
                //path for get files from ftp
                bool IsExists = objFtp.FtpDirectoryExists(FtpServer);
                // checking related ponumber directory is there are not!

                dtFiles = objFtp.GetListOfFiles(FtpServer);
                if (dtFiles.Rows.Count > 0)
                {
                    grdDIdocs.DataSource = dtFiles;
                    grdDIdocs.DataBind();
                }


                if (IsExists == false)
                {
                    grdDIdocs.Visible = false;
                    return dtFiles;
                }

                //dtFiles.Columns.AddRange(new DataColumn[1] { new DataColumn("Name", typeof(string)) });
                //WebRequest request = (WebRequest)WebRequest.Create(FtpServer);
                //request.Credentials = new NetworkCredential(username, password);
                //request.Method = WebRequestMethods.Ftp.ListDirectory;
                //StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
                //string fileName3 = streamReader.ReadLine();

                //List<string> directories = new List<string>();
                //while (fileName3 != null && fileName3 != "testing12")
                //{
                //    dtFiles.Rows.Add(fileName3);
                //    directories.Add(fileName3);
                //    fileName3 = streamReader.ReadLine();
                //}
                //streamReader.Close();
                //using (WebClient ftpClient = new WebClient())
                //{
                //    ftpClient.Credentials = new System.Net.NetworkCredential(username, password);

                //    for (int i = 0; i < directories.Count; i++)
                //    {
                //        if (directories[i].Contains("."))
                //        {

                //            // string path = FtpServer + directories[i].ToString();
                //            //string trnsfrpth = localpath + directories[i].ToString();
                //            //ftpClient.DownloadFile(path, trnsfrpth);
                //            grdDIdocs.DataSource = dtFiles;

                //        }

                //    }

                ViewState["DiDocs"] = dtFiles;

                grdDIdocs.DataBind();
                //}
                return dtFiles;
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return dtFiles;
            }
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                clsDelivery obj = new clsDelivery();
                string temp = txtPoNumber.Text;
                obj.sPONumber = temp.ToUpper();
                txtPoNumber.Text = temp.ToUpper();
                obj.GetPurchaseCount(obj);
                txtTotalQuantity.Text = obj.sTotalTC;
                BindgridView(sFileServerPath, sUserName, sPassword);
                txtPoId.Text = obj.GetPOId(obj);
                DataTable dt = new DataTable();
                dt = obj.GetDeliveryDetails(obj);
                LoadComboField();
                if(dt.Rows.Count > 0)
                {
                    ViewState["TOTALTC"] = dt;
                    cmdAdd.Enabled = true;
                }
                grdPendingTC.DataSource = dt;
                grdPendingTC.DataBind();
            }
            catch(Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void Reset()
        {
            try
            {
               // txtDINumber.Text = "";
                txtDIDate.Text = "";
                txtConsignee.Text = "";
                cmbStore.SelectedIndex = 0;
                txtDueDate.Text = "";
                cmbMake.SelectedIndex = 0;
                cmbCapacity.ClearSelection();
                txtQuantity.Text = "";
                cmbRating.ClearSelection();
                
            }
            catch(Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public string[] isCountCapacity(string strCapacity,string strmake)
        {
            string[] Arr = new string[2];
            try
            {
                DataTable dtPoDetails;
                DataTable DiDetailsGrid;
                int Count = 0;
                int pending = 0;
                dtPoDetails = (DataTable)ViewState["TOTALTC"];
                DiDetailsGrid = (DataTable)ViewState["DiDetails"];
                string po_num,po_make,po_capacity,po_rating,po_total,Po_pending;
                
                if (DiDetailsGrid !=null)
                {

                    for (int i = 0; i < DiDetailsGrid.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtPoDetails.Rows.Count; j++)
                        {
                            if (Convert.ToString(DiDetailsGrid.Rows[i]["DI_CAPACITY"]) == Convert.ToString(dtPoDetails.Rows[j]["CAPACITY"]) && Convert.ToString(DiDetailsGrid.Rows[i]["DI_MAKE"]) == Convert.ToString(dtPoDetails.Rows[j]["MAKE"])
                                && strmake == Convert.ToString(dtPoDetails.Rows[j]["MAKE"]) && strCapacity == Convert.ToString(dtPoDetails.Rows[j]["CAPACITY"]))
                            {
                                 
                                po_num = Convert.ToString(dtPoDetails.Rows[j]["PO_NO"]);
                                po_make = Convert.ToString(dtPoDetails.Rows[j]["MAKE"]);
                                po_capacity = Convert.ToString(dtPoDetails.Rows[j]["CAPACITY"]);
                                po_rating = Convert.ToString(dtPoDetails.Rows[j]["RATING"]);
                                po_total = Convert.ToString(dtPoDetails.Rows[j]["TOTAL"]);
                                Po_pending = Convert.ToString(dtPoDetails.Rows[j]["PENDING"]);
                            }
                        }
                        Count = pending + Convert.ToInt16(txtQuantity.Text);
                        //To check whether selected transformers doesnot exceed requested number of transformers
                        if (Convert.ToString(DiDetailsGrid.Rows[i]["DI_CAPACITY"]) == cmbCapacity.SelectedItem.Text)
                        {
                            if (Convert.ToInt32(dtPoDetails.Rows[i]["PENDING"]) > Count)
                            {
                                Arr[0] = "Accept";
                                Arr[1] = "0";
                                return Arr;
                            }
                            else
                            {
                                Arr[0] = "Please Select Available Quantity of " +(Convert.ToInt32(dtPoDetails.Rows[i]["PENDING"]) - pending) + " ";
                                Arr[1] = "1";
                                return Arr;
                            }
                            
                        }
                        else
                        {
                            if (Convert.ToInt32(dtPoDetails.Rows[i]["PENDING"]) > Count)
                            {
                                Arr[0] = "Accept";
                                Arr[1] = "0";
                                return Arr;
                            }
                            else
                            {
                                Arr[0] = "Please Select Available Quantity of " + (Convert.ToInt32(dtPoDetails.Rows[i]["PENDING"]) - pending) + " ";
                                Arr[1] = "1";
                                return Arr;
                            }
                        }

                    }
                }
                else
                {
                    for (int j = 0; j < dtPoDetails.Rows.Count; j++)
                    {
                        if (strCapacity == Convert.ToString(dtPoDetails.Rows[j]["CAPACITY"]) && strmake == Convert.ToString(dtPoDetails.Rows[j]["MAKE"]))
                        {
                            po_num = Convert.ToString(dtPoDetails.Rows[j]["PO_NO"]);
                            po_make = Convert.ToString(dtPoDetails.Rows[j]["MAKE"]);
                            po_capacity = Convert.ToString(dtPoDetails.Rows[j]["CAPACITY"]);
                            po_rating = Convert.ToString(dtPoDetails.Rows[j]["RATING"]);
                            po_total = Convert.ToString(dtPoDetails.Rows[j]["TOTAL"]);
                            Po_pending = Convert.ToString(dtPoDetails.Rows[j]["PENDING"]);

                           Count += Convert.ToInt32(txtQuantity.Text);
                        }
                    }
                    Arr[0] = "Accept";
                    Arr[1] = "0";
                    return Arr;
                }
                return Arr;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Arr;
            }

        }
       

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {

                 
                 string[] Arr = new string[2];
                    DataTable dt = new DataTable();
                    if (validate() == false)
                    {
                        return;
                    }

                    string sDIid = txtDIId.Text;
                    string sDINo = txtDINumber.Text;
                    string sDIDate = txtDIDate.Text;
                    string sPoId = txtPoId.Text;
                    string sCOnsignee = txtConsignee.Text;
                    string sStoreId = cmbStore.SelectedValue;
                    string sStoreName = Convert.ToString(cmbStore.SelectedItem);
                    string sDueDate = txtDueDate.Text;
                    string sMakeId = cmbMake.SelectedValue;
                    string sMakeName = Convert.ToString(cmbMake.SelectedItem);
                    string sCapacity = Convert.ToString(cmbCapacity.SelectedItem);
                    string sCapacityID = cmbCapacity.SelectedValue;
                    string sQuantity = txtQuantity.Text;
                    string sRatingID = cmbRating.SelectedValue;
                    string sRating = Convert.ToString(cmbRating.SelectedItem);
                    int pending = 0;
                    int oldQuantity = 0; 
                    int AvailQuantity= 0;
                    int totalQunty = 0;

                    DataTable dtpending = new DataTable();

                    if (Session["FileUpload"] == null && fupFile.HasFile)
                    {
                        Session["FileUpload"] = fupFile;
                        lblFilename.Text = fupFile.FileName; // get the name 
                        fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/"  + fupFile.FileName));
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
                        fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/"  + fupFile.FileName));
                        Session["FileUpload"] = fupFile;
                        lblFilename.Text = fupFile.FileName;
                    }
                    clsDelivery objDeli = new clsDelivery();
                    objDeli.sDIid = sDIid;
                    objDeli.sDINo = sDINo.ToUpper();
                    if (objDeli.sDIid != "" && objDeli.sDIid != null)
                    {
                        objDeli.sCapacity = sCapacity;
                        objDeli.sTcMake = sMakeId;
                        objDeli.sStore = sStoreId;
                        dt = objDeli.GetAllotmentCount(objDeli);
                        if (dt.Rows.Count > 0)
                        {
                            int delivered = Convert.ToInt32(dt.Rows[0]["ALLOTED"]);
                            int Quantty = Convert.ToInt32(txtQuantity.Text);

                            if (Quantty < delivered)
                            {
                                string Msg = Convert.ToString(delivered);
                                ShowMsgBox("This Capacity Already Alloted To Some Division  Quantity " + Msg + ", So You Can`t Reduce The Count Below  :" + Msg + " For Reference Allotment Number:" + Convert.ToString(dt.Rows[0]["ALT_NO"]));
                                dt = null;
                                return;
                            }
                        }
                    }
                    objDeli.sCapacity = sCapacity;
                    objDeli.sPoId = sPoId;
                    dtpending = objDeli.GetDeliveryDetails(objDeli);
                    for (int i = 0; i < dtpending.Rows.Count; i++)
                    {
                        if (Convert.ToString(dtpending.Rows[i]["CAPACITY"]) == cmbCapacity.SelectedItem.Text && cmbMake.SelectedItem.Text == Convert.ToString(dtpending.Rows[i]["MAKE"]) && cmbRating.SelectedValue == Convert.ToString(dtpending.Rows[i]["PO_RATING"]))
                        {
                            pending = Convert.ToInt32(dtpending.Rows[i]["PENDING"]);
                            totalQunty = Convert.ToInt32(dtpending.Rows[i]["TOTAL"]);
                           
                        }
                        else
                        {
                            continue;
                        }
                        if (sDIid == "")
                        {
                            if (Convert.ToInt32(sQuantity) > Convert.ToInt32(pending))
                            {
                                ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity of " + pending + " ");
                                return;
                            }
                        }
                        else
                        {
                            if (ViewState["DiDetails"] != null)
                            {
                                DataTable dtQunty = (DataTable)ViewState["DiDetails"];
                               
                                for (int j = 0; j < dtQunty.Rows.Count; j++)
                                {
                                    if (cmbMake.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["DI_MAKE"]) && cmbCapacity.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["DI_CAPACITY"]) && cmbRating.SelectedValue == Convert.ToString(dtQunty.Rows[j]["DI_STARRATE"]))
                                    {
                                        oldQuantity += Convert.ToInt32(dtQunty.Rows[j]["DI_QUANTITY"]);
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
                        if (ViewState["DiDetails"] != null)
                        {
                            DataTable dtQunty = (DataTable)ViewState["DiDetails"];
                            for (int j = 0; j < dtQunty.Rows.Count; j++)
                            {
                                if (cmbMake.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["DI_MAKE"]) && cmbCapacity.SelectedItem.Text == Convert.ToString(dtQunty.Rows[j]["DI_CAPACITY"]) && cmbRating.SelectedValue == Convert.ToString(dtQunty.Rows[j]["DI_STARRATE"]))
                                {
                                    oldQuantity += Convert.ToInt32(dtQunty.Rows[j]["DI_QUANTITY"]);
                                }
                            }
                            AvailQuantity=(Convert.ToInt32(pending)-Convert.ToInt32(oldQuantity));

                                if (Convert.ToInt32(sQuantity) > AvailQuantity)
                                {
                                    ShowMsgBox(" Capacity Quantity Should not greater than the Available Quantity of " + AvailQuantity + " ");
                                    return;
                                }
                               
                        }
                    }
                    if (ViewState["DiDetails"] != null)
                    {
                        DataTable dtCap = (DataTable)ViewState["DiDetails"];
                        for (int i = 0; i < dtCap.Rows.Count; i++)
                        {
                             
                            if (sDINo == Convert.ToString(dtCap.Rows[i]["DI_NO"]) && sCOnsignee == Convert.ToString(dtCap.Rows[i]["DI_CONSIGNEE"])
                                && sStoreId == Convert.ToString(dtCap.Rows[i]["DI_STORE_ID"]) && sMakeId == Convert.ToString(dtCap.Rows[i]["DI_MAKE_ID"]) && sRatingID == Convert.ToString(dtCap.Rows[i]["DI_STARRATE"])
                                && sCapacity == Convert.ToString(dtCap.Rows[i]["DI_CAPACITY"]))
                            {
                                ShowMsgBox("Consignee - make - Store- Star Rate - Capacity - Start Rate Combination Already Added");
                                return;
                            }
                            

                        }
                    }

                    if (ViewState["DiDetails"] == null)
                    {


                        dt.Columns.Add("DI_ID");
                        dt.Columns.Add("DI_PO_ID");
                        dt.Columns.Add("DI_NO");
                        dt.Columns.Add("DI_DATE");
                        dt.Columns.Add("DI_CONSIGNEE");
                        dt.Columns.Add("DI_STORE_ID");
                        dt.Columns.Add("DI_STORE");
                        dt.Columns.Add("DI_DUEDATE");
                        dt.Columns.Add("DI_MAKE_ID");
                        dt.Columns.Add("DI_MAKE");
                        dt.Columns.Add("DI_CAPACITY");
                        dt.Columns.Add("DI_CAPACITY_ID");
                        dt.Columns.Add("DI_STARRATE");
                        dt.Columns.Add("DI_STARRATENAME");
                        dt.Columns.Add("DI_QUANTITY");
                        dt.Columns.Add(new DataColumn("DI_FILE", typeof(byte[])));
                        dt.Columns.Add("DI_FILE_EXT");

                    }
                    else
                    {
                        //load datatble from viewstate
                        dt = (DataTable)ViewState["DiDetails"];
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
                         
                    }

                    dRow["DI_PO_ID"] = sPoId;
                    dRow["DI_NO"] = sDINo.ToUpper();
                    dRow["DI_DATE"] = sDIDate;
                    dRow["DI_CONSIGNEE"] = sCOnsignee;
                    dRow["DI_STORE_ID"] = sStoreId;
                    dRow["DI_STORE"] = sStoreName;
                    dRow["DI_DUEDATE"] = sDueDate;
                    dRow["DI_MAKE_ID"] = sMakeId;
                    dRow["DI_MAKE"] = sMakeName;
                    dRow["DI_CAPACITY"] = sCapacity;
                    dRow["DI_CAPACITY_ID"] = sCapacityID;
                    dRow["DI_STARRATE"] = sRatingID;
                    dRow["DI_STARRATENAME"] = sRating;
                    dRow["DI_QUANTITY"] = sQuantity;
                    //  dRow["DI_FILE"] = BufferImg;
                    dRow["DI_FILE_EXT"] = strExt;

                    if (sDIid == "")
                    {
                       //Arr = isCountCapacity(sCapacity, sMakeName);
                       // if (Arr[1]=="0")
                       // {
                            dRow["DI_ID"] = Id;
                            dRow["DI_PO_ID"] = sPoId;
                            dRow["DI_NO"] = sDINo.ToUpper();
                            dRow["DI_DATE"] = sDIDate;
                            dRow["DI_CONSIGNEE"] = sCOnsignee;
                            dRow["DI_STORE_ID"] = sStoreId;
                            dRow["DI_STORE"] = sStoreName;
                            dRow["DI_DUEDATE"] = sDueDate;
                            dRow["DI_MAKE_ID"] = sMakeId;
                            dRow["DI_MAKE"] = sMakeName;
                            dRow["DI_CAPACITY"] = sCapacity;
                            dRow["DI_CAPACITY_ID"] = sCapacityID;
                            dRow["DI_STARRATE"] = sRatingID;
                            dRow["DI_STARRATENAME"] = sRating;
                            dRow["DI_QUANTITY"] = sQuantity;
                            //  dRow["DI_FILE"] = BufferImg;
                            dRow["DI_FILE_EXT"] = strExt;
                            dt.Rows.Add(dRow);
                            dt.AcceptChanges();
                            ViewState["DiDetails"] = dt;
                            LoadCapacity(dt);
                        //}
                        //else
                        //{
                             
                        //    ShowMsgBox(Arr[0]);
                        //    // ViewState["TC"] = dtTc;
                        //    return;
                        //}
                        return;
                    }
                    #region//grd row edit
                    //if (ViewState["gridRowId"] != null)
                    //{
                    //    dt = (DataTable)ViewState["DiDetails"];
                    //    // gridid.Text = ViewState["gridRowId"].ToString();
                    //    int i = Convert.ToInt32(ViewState["gridRowId"]);

                    //    dt.Rows[i].SetField("DI_PO_ID", txtDIId.Text);
                    //    dt.Rows[i].SetField("DI_NO", txtDINumber.Text);
                    //    dt.Rows[i].SetField("DI_DATE", txtDIDate.Text);
                    //    dt.Rows[i].SetField("DI_DUEDATE", txtDueDate.Text);
                    //    dt.Rows[i].SetField("DI_CONSIGNEE", txtConsignee.Text);
                    //    dt.Rows[i].SetField("DI_STORE_ID", sStoreId);
                    //    dt.Rows[i].SetField("DI_MAKE_ID", cmbMake.SelectedValue);
                    //    dt.Rows[i].SetField("DI_MAKE", sMakeName);
                    //    dt.Rows[i].SetField("DI_STORE", sStoreName);
                    //    dt.Rows[i].SetField("DI_CAPACITY", cmbCapacity.SelectedItem.Text);
                    //    dt.Rows[i].SetField("DI_CAPACITY_ID", cmbCapacity.SelectedValue);
                    //    dt.Rows[i].SetField("DI_STARRATE", cmbRating.SelectedValue);
                    //    dt.Rows[i].SetField("DI_QUANTITY", txtQuantity.Text);
                    //    dt.Rows[i].SetField("DI_FILE_EXT", fupFile.FileName);
                    //    ViewState["gridRowId"] = null;
                    //    ViewState["DiDetails"] = dt;
                    //    LoadCapacity(dt);
                    //}
                    //else
                    //{
                    //    dt.Rows.Add(dRow);
                    //    dt.AcceptChanges();
                    //    ViewState["DiDetails"] = dt;
                    //    LoadCapacity(dt);
                    //}
                    //}
                    #endregion
                    else
                    {
                        // Arr= isCountCapacity(sCapacity, sMakeName);
                        //if (Arr[1]=="0")
                        //{

                            dRow["DI_ID"] = sDIid;
                            dRow["DI_PO_ID"] = sPoId;
                            dRow["DI_NO"] = sDINo.ToUpper();
                            dRow["DI_DATE"] = sDIDate;
                            dRow["DI_CONSIGNEE"] = sCOnsignee;
                            dRow["DI_STORE_ID"] = sStoreId;
                            dRow["DI_STORE"] = sStoreName;
                            dRow["DI_DUEDATE"] = sDueDate;
                            dRow["DI_MAKE_ID"] = sMakeId;
                            dRow["DI_MAKE"] = sMakeName;
                            dRow["DI_CAPACITY"] = sCapacity;
                            dRow["DI_CAPACITY_ID"] = sCapacityID;
                            dRow["DI_STARRATE"] = sRatingID;
                            dRow["DI_STARRATENAME"] = sRating;
                            dRow["DI_QUANTITY"] = sQuantity;
                            // dRow["DI_FILE"] = BufferImg;
                            //dRow["DI_FILE_EXT"] = strExt;
                            dt.Rows.Add(dRow);
                            dt.AcceptChanges();
                            ViewState["DiDetails"] = dt;
                            LoadCapacity(dt);
                    //    }
                    //    else
                    //    {                      
                    //        ShowMsgBox(Arr[0]);
                    //        // ViewState["TC"] = dtTc;
                    //        return;
                    //    }
                    }
               
            }
            catch(Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadCapacity(DataTable dt)
        {
            try
            {                
                grdDelivery.DataSource = dt;
                grdDelivery.DataBind();
                grdDelivery.Visible = true;
                Reset();
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

        protected bool validate()
        {
            bool Status = false;
            try
            {
                if (txtDINumber.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Delivery Number");
                    txtDINumber.Focus();
                    return Status;
                }
                if (txtDIDate.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Delivery Date");
                    txtDIDate.Focus();
                    return Status;
                }
                //DateTime DIDate = DateTime.ParseExact(txtDIDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime DUEDate = DateTime.ParseExact(txtDueDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //if (DIDate > DUEDate)
                //{
                //    txtDueDate.Focus();
                //    ShowMsgBox("Due Date Should Greater Than DI Date");
                //    return false;
                //}
             
                if (txtConsignee.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Consigneee");
                    txtConsignee.Focus();
                    return Status;
                }
                if (cmbStore.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Store");
                    txtConsignee.Focus();
                    return Status;
                }
                if (cmbMake.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Make");
                    cmbMake.Focus();
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
                if (txtQuantity.Text.Trim() == "0")
                {
                    ShowMsgBox("Quantity Should be Greater than Zero ");
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
            catch(Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return Status;
            }
        }

        protected void ResetGrid()
        {
            DataTable dt = (DataTable)ViewState["DiDetails"];
            if (dt.Rows.Count > 0)
            {
                int counter = 0;
                foreach (DataRow row1 in dt.Rows)
                {
                    counter++;
                    row1["ID"] = counter;
                }
                ViewState["DiDetails"] = dt;
            }
            else
            {
                ViewState["DiDetails"] = null;
            }
            dt = (DataTable)ViewState["DiDetails"];
            grdDelivery.DataSource = dt;
            grdDelivery.DataBind();
        }

        protected void grdDelivery_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            clsDelivery objDeli = new clsDelivery();
            try
            {
                if (e.CommandName == "Remove")
                {
                    DataTable dt = (DataTable)ViewState["DiDetails"];
                    DataTable ALtCount;
                    GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int sRowIndex = row.RowIndex;

                    Label lblDiId = (Label)row.FindControl("lblDiId");
                    Label lblpoId = (Label)row.FindControl("lblpoId");
                    Label lblDiNo = (Label)row.FindControl("lblDiNo");
                    Label lblDIDate = (Label)row.FindControl("lblDIDate");
                    Label lblConsignee = (Label)row.FindControl("lblConsignee");
                    Label lblStoreId = (Label)row.FindControl("lblStoreId");
                    Label lblDuedate = (Label)row.FindControl("lblDuedate");
                    Label lblMakeId = (Label)row.FindControl("lblMakeId");
                    Label lblCapacity = (Label)row.FindControl("lblCapacity");
                    Label lblRating = (Label)row.FindControl("lblRating");
                    Label lblQuantity = (Label)row.FindControl("lblQuantity");

                    objDeli.sDINo = lblDiNo.Text;
                    objDeli.sCapacity = lblCapacity.Text;
                    objDeli.sTcMake = lblMakeId.Text;
                    objDeli.sStore = lblStoreId.Text;
                    ALtCount = objDeli.GetAllotmentCount(objDeli);
                    // TO Check Before Deleting Whether Dispatch No Alloted or not 
                    if (ALtCount.Rows.Count > 0)
                    {
                        int delivered = Convert.ToInt32(ALtCount.Rows[0]["ALLOTED"]);
                       
                        string Msg = Convert.ToString(delivered);
                        ShowMsgBox("This Capacity Already Alloted To Some Division  Quantity " + Msg + ", So You Can`t Delete This Record , For Reference Allotment Number:" + Convert.ToString(ALtCount.Rows[0]["ALT_NO"]));
                        ALtCount = null;
                        return;
                    }
                    else
                    {
                        //to remove selected Capacity from grid
                        dt.Rows[sRowIndex].Delete();
                        dt.AcceptChanges();
                    }
                    if (dt.Rows.Count == 0)
                    {
                        ViewState["DiDetails"] = null;
                    }
                    else
                    {
                        ViewState["DiDetails"] = dt;
                    }
                    grdDelivery.DataSource = dt;
                    grdDelivery.DataBind();

                    LoadCapacity(dt);
                }
                else
                {
                    DataTable dt = (DataTable)ViewState["DiDetails"];
                    LoadCapacity(dt);
                }
                   if (e.CommandName == "EditQNTY")
                {
                  
                       DataTable dt = (DataTable)ViewState["DiDetails"];
                        GridViewRow row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        //clsPoMaster objPoMast = new clsPoMaster();
                        int sRowIndex = row.RowIndex;
                        
                        Label lblDiId = (Label)row.FindControl("lblDiId");
                        Label lblpoId = (Label)row.FindControl("lblpoId");
                        Label lblDiNo = (Label)row.FindControl("lblDiNo");
                        Label lblDIDate = (Label)row.FindControl("lblDIDate");
                        Label lblConsignee = (Label)row.FindControl("lblConsignee");
                        Label lblStoreId = (Label)row.FindControl("lblStoreId");
                        Label lblDuedate = (Label)row.FindControl("lblDuedate");
                        Label lblMakeId = (Label)row.FindControl("lblMakeId");
                        Label lblCapacity = (Label)row.FindControl("lblCapacityID");
                        Label lblRating = (Label)row.FindControl("lblRating");
                        Label lblQuantity = (Label)row.FindControl("lblQuantity");
                        
                        
                       
                        txtDINumber.Text = lblDiNo.Text;
                        txtDIId.Text = lblpoId.Text;
                        txtDIId.Visible = false;
                        txtConsignee.Text = lblConsignee.Text;
                        txtDIDate.Text = lblDIDate.Text;
                        txtDueDate.Text = lblDuedate.Text;
                        txtDINumber.Enabled = false;
                        //cmbCapacity.SelectedValue = lblCapacity.Text;
                        cmbMake.SelectedValue = lblMakeId.Text;
                        //cmbRating.SelectedValue = lblRating.Text; ;
                        txtQuantity.Text = lblQuantity.Text ;
                        cmbStore.SelectedValue = lblStoreId.Text;
                        cmbMake_SelectedIndexChanged(sender, e);
                       // cmbCapacity_SelectedIndexChanged(sender, e);
                       
                        dt.Rows[sRowIndex].Delete();
                        dt.AcceptChanges();
                        if (dt.Rows.Count == 0)
                        {
                            ViewState["DiDetails"] = null;
                        }
                        else
                        {
                            ViewState["DiDetails"] = dt;
                        }
                        grdDelivery.DataSource = dt;
                        grdDelivery.DataBind();

                                                          
               }
                    
            }           
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }    

        protected void grdDelivery_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dtTcCapacity = new DataTable();
                grdDelivery.PageIndex = e.NewPageIndex;
                dtTcCapacity = (DataTable)ViewState["DiDetails"];
                LoadCapacity(dtTcCapacity);
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
                Response.Redirect("DeliveryInstView.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["DiDetails"] != null)
                {
                    clsDelivery obj = new clsDelivery();
                    clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                    // clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
                    obj.sCrby = objSession.UserId;
                    obj.dtDelivery = (DataTable)ViewState["DiDetails"];
                    obj.sDINo = Convert.ToString(obj.dtDelivery.Rows[0]["DI_NO"]);
                    bool Isuploaded;
                    bool IsFileExiest;
                    string sMainFolderName = "DI_DOCS";
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
                            ShowMsgBox("Please upload the Delivery Note ");
                            fupFile.Focus();
                            return;
                        }

                    }
                    string strpath = System.IO.Path.GetExtension(fupFile.FileName);

                    string filename = Path.GetFileName(fupFile.PostedFile.FileName);
                    obj.sDINo = Regex.Replace(obj.sDINo, @"[^0-9a-zA-Z]+", "");

                    obj.sFileExt = sFileServerPath + sMainFolderName + "/" + obj.sDINo + "/" + filename;

                    if (ValidateForm() == true)
                    {
                        if (fupFile.PostedFile.FileName != null && fupFile.PostedFile.FileName != "")
                        {
                            sArr = obj.SaveDeliveryDetails(obj);
                        }
                        else
                        {
                            ShowMsgBox("Please upload the File");
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

                            //fupFile.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" + obj.sDINo + "~" + filename));
                            string sDirectory = Server.MapPath("~/DTLMSDocs" + "/" + filename);



                            if (File.Exists(sDirectory))
                            {

                                bool IsExists = objFtp.FtpDirectoryExists(sFileServerPath + "/" + sMainFolderName);
                                if (IsExists == false)
                                {

                                    objFtp.createDirectory(sFileServerPath + "/" + sMainFolderName);
                                }
                                IsExists = objFtp.FtpDirectoryExists(sFileServerPath + "/" + sMainFolderName + "/" + obj.sDINo);
                                if (IsExists == false)
                                {

                                    objFtp.createDirectory(sFileServerPath + sMainFolderName + "/" + obj.sDINo);
                                }
                                // IsFileExiest = objFtp.IsfileExiest(sMainFolderName + "/" + cmbDivision.Text + "/" + cmbRepairer.Text);
                                //if (IsFileExiest == false)
                                //{
                                Isuploaded = objFtp.upload(sFileServerPath + sMainFolderName + "/" + obj.sDINo, filename, sDirectory);
                                if (Isuploaded == true & File.Exists(sDirectory))
                                {
                                    File.Delete(sDirectory);
                                    sDirectory = obj.sDINo + "/" + filename;

                                    ShowMsgBox(sArr[0]);


                                }
                            }


                        }
                        Session["FileUpload"] = null;
                        btnSave.Enabled = false;
                        ShowMsgBox(sArr[0]);
                        Reset();
                        grdDelivery.DataSource = null;
                        grdDelivery.DataBind();
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
                    ShowMsgBox("Please ADD Delivery Details!");
                    return;
                }
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
                DataTable dt = (DataTable)ViewState["PoDocs"];
                gvFiles.DataSource = SortDataTable(dt as DataTable, true);
                gvFiles.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdPendingTC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvFiles.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["TOTALTC"];
                gvFiles.DataSource = SortDataTable(dt as DataTable, true);
                gvFiles.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void grdDIdocs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdDIdocs.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["DiDocs"];
                grdDIdocs.DataSource = SortDataTable(dt as DataTable, true);
                grdDIdocs.DataBind();
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
        bool ValidateForm()
        {

            bool bValidate = false;
            if (ViewState["DiDetails"] == null)
            {
                if (txtDINumber.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Delivery Number");
                    txtDINumber.Focus();
                    return bValidate;
                }
                if (txtDIDate.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Delivery Date");
                    txtDIDate.Focus();
                    return bValidate;
                }
                if (txtConsignee.Text.Trim() == string.Empty)
                {
                    ShowMsgBox("Please Enter the Consigneee");
                    txtConsignee.Focus();
                    return bValidate;
                }
                if (cmbStore.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Store");
                    cmbStore.Focus();
                    return bValidate;
                }
                if (cmbMake.SelectedIndex == 0)
                {
                    ShowMsgBox("Please Select the Make");
                    cmbMake.Focus();
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
        //protected void DownloadFile(object sender, EventArgs e)
        //{

        //    string fileName = (sender as LinkButton).CommandArgument;

        //    try
        //    {
        //        string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
        //        //Create FTP Request.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/PO_DOCS/" + PoNo + "/" + fileName);
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;

        //        //Enter FTP Server credentials.
        //        request.Credentials = new NetworkCredential(sUserName, sPassword);
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.EnableSsl = false;

        //        //Fetch the Response and read it into a MemoryStream object.
        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            //Download the File.
        //            response.GetResponseStream().CopyTo(stream);
        //            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //            Response.BinaryWrite(stream.ToArray());
        //            Response.End();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        lblMessage.Text = clsException.ErrorMsg();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }

        //}

        protected void DownloadFile(object sender, EventArgs e)
        {
            bool status = false;
            string fileName = (sender as LinkButton).CommandArgument;

            try
            {
                string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                status = objFtp.Download(sFileServerPath + "/PO_DOCS/" + PoNo + "/" + fileName, fileName);



                //Create FTP Request.
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/PO_DOCS/" + PoNo + "/" + fileName);
                //request.Method = WebRequestMethods.Ftp.DownloadFile;

                ////Enter FTP Server credentials.
                //request.Credentials = new NetworkCredential(sUserName, sPassword);
                //request.UsePassive = true;
                //request.UseBinary = true;
                //request.EnableSsl = false;

                ////Fetch the Response and read it into a MemoryStream object.
                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    //Download the File.
                //    response.GetResponseStream().CopyTo(stream);
                //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //    Response.BinaryWrite(stream.ToArray());
                //    Response.End();
                //}
            }
            catch (WebException ex)
            {
                //  lblMessage.Text = clsException.ErrorMsg();
                // clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        //protected void DownloadFile1(object sender, EventArgs e)
        //{

        //    string fileName = (sender as LinkButton).CommandArgument;

        //    try
        //    {
        //        string DINo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");
        //        //Create FTP Request.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/DI_DOCS/" + DINo + "/" + fileName);
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;

        //        //Enter FTP Server credentials.
        //        request.Credentials = new NetworkCredential(sUserName, sPassword);
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.EnableSsl = false;

        //        //Fetch the Response and read it into a MemoryStream object.
        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            //Download the File.
        //            response.GetResponseStream().CopyTo(stream);
        //            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //            Response.BinaryWrite(stream.ToArray());
        //            Response.End();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        lblMessage.Text = clsException.ErrorMsg();
        //        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }

        //}

        protected void DownloadFile1(object sender, EventArgs e)
        {

            string fileName = (sender as LinkButton).CommandArgument;
            bool status = false;
            try
            {
                string DINo = Regex.Replace(txtDINumber.Text, @"[^0-9a-zA-Z]+", "");

                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                status = objFtp.Download(sFileServerPath + "/DI_DOCS/" + DINo + "/" + fileName, fileName);

                ////Create FTP Request.
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileServerPath + "/DI_DOCS/" + DINo + "/" + fileName);
                //request.Method = WebRequestMethods.Ftp.DownloadFile;

                ////Enter FTP Server credentials.
                //request.Credentials = new NetworkCredential(sUserName, sPassword);
                //request.UsePassive = true;
                //request.UseBinary = true;
                //request.EnableSsl = false;

                ////Fetch the Response and read it into a MemoryStream object.
                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    //Download the File.
                //    response.GetResponseStream().CopyTo(stream);
                //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //    Response.BinaryWrite(stream.ToArray());
                //    Response.End();
                //}
            }
            catch (WebException ex)
            {
                // lblMessage.Text = clsException.ErrorMsg();
                // clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        //protected void lnkPoDownload_Click(object sender, EventArgs e)
        //{
        //    Byte[] POImage = (Byte[])ViewState["POIMAGE"];
        //    string sExt = (string)ViewState["FILEXT"];
        //    Response.Buffer = true;
        //    Response.Charset = "";
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.ContentType = "image/png";

        //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + txtPoNumber.Text + sExt);

        //    Response.BinaryWrite(POImage);
        //    Response.Flush();
        //    HttpContext.Current.ApplicationInstance.CompleteRequest();
        //}
    }
}