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
using System.Configuration;
using System.Data.OleDb;

namespace IIITS.DTLMS.Transaction
{
    public partial class Bufferstock : System.Web.UI.Page
    {
        string strFormCode = "Bufferstock";
        clsSession objSession;

        string sFileServerPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
        string sMainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);
        string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
        string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
        string sVirtualdocs = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["VirtualDirectoryDocs"]);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                objSession = (clsSession)Session["clsSession"];
                lblMessage.Text = string.Empty;
                if (!IsPostBack)
                {
                    CheckAccessRights("2");
                }

            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "Bufferstock";
                objApproval.sRoleId = objSession.RoleId;
                objApproval.sAccessType = "1" + "," + sAccessType;
                bool bResult = objApproval.CheckAccessRights(objApproval);
                if (bResult == false)
                {
                    Response.Redirect("~/UserRestrict.aspx", false);

                }
                return bResult;

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return false;

            }
        }


        protected void cmdUpload_click(object sender, EventArgs e)
        {
            try
            {


                if (fupUpload.PostedFile.ContentLength == 0)
                {
                    ShowMsgBox("Please Select the File");
                    fupUpload.Focus();
                    return;
                }
                DateTime currentDateTime = DateTime.Now;
                string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["UploadFormat"]);
                string sUploadFileExt = System.IO.Path.GetExtension(fupUpload.FileName).ToString().ToLower();
                sUploadFileExt = ";" + sUploadFileExt.Remove(0, 1) + ";";

                if (!sFileExt.Contains(sUploadFileExt))
                {
                    ShowMsgBox("Invalid File Format");
                    return;
                }
                string excelPath = Server.MapPath("~/DTLMSDocs/") + Path.GetFileName(fupUpload.PostedFile.FileName);
                fupUpload.SaveAs(excelPath);
                string conString = string.Empty;
                string extension = Path.GetExtension(fupUpload.PostedFile.FileName);
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                        break;
                }
                conString = string.Format(conString, excelPath);
                DataTable dtExcelData = new DataTable();
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    excel_con.Open();
                    string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
                    using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
                    {
                        oda.Fill(dtExcelData);

                    }
                    
                     

                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    FTPUpload(sender, e, objbufferstock);

                      objbufferstock.sUserid = objSession.UserId;
                      objbufferstock.sofficecode = objSession.OfficeCode;

                    if(objbufferstock.sFilepath !="")
                        {
                        bool result = objbufferstock.SaveBufferStockDetails(objbufferstock);
                        if (result == true)
                        {
                            ShowMsgBox("Uploaded Successfully");
                        }
                        else
                        {
                            ShowMsgBox("Something went Wrong Please Try again....!!!");
                        }
                        excel_con.Close();
                        System.IO.File.Delete(excelPath);
                        //excelPath
                    }

                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                ShowMsgBox("Something went Wrong Please Try again....!!!");

            }
        }

        protected void FTPUpload(object sender, EventArgs e, clsBufferstockdetails objbufferstock)
        {
            try
            {

                //FTP Folder name. Leave blank if you want to upload to root folder.
                // string ftpFolder = "Uploads/";       
                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPmainfolder"]);
                string sFtpvirtual = Convert.ToString(ConfigurationSettings.AppSettings["VirtualDirectoryDocs"]);

                DateTime currentDateTime =  DateTime.Now ;


                string fileName = (currentDateTime.ToString("ddMMyyyyHHmm") + Path.GetFileName(fupUpload.FileName)).Trim();

                objbufferstock.sfilename= fileName;
                if (fileName == "" || fileName == null)
                {
                    ShowMsgBox("Please select the File!");
                    return;
                }

                string sFileExt = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["UploadFormat"]);
                string sAnxFileExt = System.IO.Path.GetExtension(fupUpload.FileName).ToString().ToLower();
                sAnxFileExt = ";" + sAnxFileExt.Remove(0, 1) + ";";
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                //   clsFtp objFtp = new clsFtp(sFileServerPath, sUserName, sPassword);
                bool Isuploaded;
                bool IsFileExiest;
                string sMainFolderName = sMainfolder + "BUFFERSTOCKDETAILS" + "/" + "UPLOAD_BUFFERSTOCK_DETAILS/"; 
                if (!sFileExt.Contains(sAnxFileExt))
                {
                    ShowMsgBox("Invalid Image Format");
                    return;
                }

                fupUpload.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + fupUpload.FileName));
                string sDirectory = Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + fupUpload.FileName);


                if (File.Exists(sDirectory))
                {

                    bool IsExists = objFtp.FtpDirectoryExists( sMainFolderName);
                    if (IsExists == false)
                    {
                        objFtp.createDirectory( sMainFolderName);
                    }

                    //IsFileExiest = objFtp.IsfileExiest(sFileServerPath + "/" + sMainFolderName + "/");
                    //if (IsFileExiest == false)
                    //{
                    Isuploaded = objFtp.upload(sMainFolderName, fileName, sDirectory);

                   
                    if (Isuploaded == true & File.Exists(sDirectory))
                    {
                        objbufferstock.sFilepath = sMainfolder + "BUFFERSTOCKDETAILS/" + "UPLOAD_BUFFERSTOCK_DETAILS/" + fileName;
                        File.Delete(sDirectory);
                        sDirectory = fileName;
                        ShowMsgBox("File Uploaded Successfully");         
                        return ;
                    }
                    //}

                }

            }
            catch (WebException ex)
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
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public string GetFileName(string sFTPLink, string sFTPUserName, string sFTPPassword)
        {
            string fileName = string.Empty;
            try
            {
                WebRequest request = (WebRequest)WebRequest.Create(sFTPLink);
                request.Credentials = new NetworkCredential(sFTPUserName, sFTPPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {
                    using (request.GetResponse())
                    {
                        StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
                        fileName = streamReader.ReadLine();
                        streamReader.Close();
                        return fileName;
                    }
                }
                catch (WebException)
                {
                    return fileName;
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return fileName;
            }
        }

        protected void DownloadFile(object sender, EventArgs e)
        {


            bool endRequest = false;
            // string fileName1 = (sender as LinkButton).CommandArgument;

            clsSFTP objFtp = new clsSFTP(sFileServerPath, sUserName, sPassword);
            string fileName1 = objFtp.GetFileName(sMainfolder + "BUFFERSTOCKDETAILS" + "/" + "DOWNLOADTEMPLATE/");
            if (fileName1 != null && fileName1 != "")
            {
                try
                {


                    //Create a stream for the file
                    Stream stream = null;

                    //This controls how many bytes to read at a time and send to the client
                    int bytesToRead = 10000;

                    // Buffer to read bytes in chunk size specified above
                    byte[] buffer = new Byte[bytesToRead];

                    // The number of bytes read
                    try
                    {
                        string SFTPmainfolder = Convert.ToString(ConfigurationSettings.AppSettings["VirtualDirectoryDocs"]);

                        string url = SFTPmainfolder + sMainfolder + "BUFFERSTOCKDETAILS" + "/" + "DOWNLOADTEMPLATE/" + fileName1;
                        //Create a WebRequest to get the file
                        HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(url);

                        //Create a response for this request
                        HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

                        if (fileReq.ContentLength > 0)
                            fileResp.ContentLength = fileReq.ContentLength;

                        //Get the Stream returned from the response
                        stream = fileResp.GetResponseStream();

                        // prepare the response to the client. resp is the client Response
                        var resp = HttpContext.Current.Response;

                        //Indicate the type of data being sent
                        resp.ContentType = "application/octet-stream";

                        //Name the file 
                        resp.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName1 + "\"");
                        resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

                        int length;
                        do
                        {
                            // Verify that the client is connected.
                            if (resp.IsClientConnected)
                            {
                                // Read data into the buffer.
                                length = stream.Read(buffer, 0, bytesToRead);

                                // and write it out to the response's output stream
                                resp.OutputStream.Write(buffer, 0, length);

                                // Flush the data
                                resp.Flush();

                                //Clear the buffer
                                buffer = new Byte[bytesToRead];
                            }
                            else
                            {
                                // cancel the download if client has disconnected
                                length = -1;
                            }
                        } while (length > 0); //Repeat until no data is read
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            //Close the input stream
                            stream.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("(404) Not Found"))
                    {
                        ShowMsgBox("File Not Found");
                    }
                    else
                    {
                        lblMessage.Text = clsException.ErrorMsg();
                        clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                    }

                }
            }

            }
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (objSession.sRoleType == "2")
                {
                    Response.Redirect("~/StoreDashboard.aspx", false);
                }
                else
                {
                    Response.Redirect("~/Dashboard.aspx", false);
                }
            }
            catch (Exception ex)
            {
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}