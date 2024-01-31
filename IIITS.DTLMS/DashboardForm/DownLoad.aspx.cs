using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.Data;
using System.Net;
using System.IO;
using System.Configuration;



namespace IIITS.DTLMS.DashboardForm
{
    public partial class DownLoad : System.Web.UI.Page
    {
        string strFormCode = "DownLoad";
        clsSession objSession;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["clsSession"] == null || Convert.ToString(Session["clsSession"]) == "")
                {
                    Response.Redirect("~/Login.aspx", false);
                }
                else
                {
                    objSession = (clsSession)Session["clsSession"];
                    lblMessage.Text = string.Empty;
                    string bindviewPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["CircularsVirtualPath"]);
                    string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
                    string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);

                  //  BindgridView(bindviewPath, sUserName, sPassword);
                }
                
               
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }


        }
        protected void lnkAndroidManual_Click_apk(object sender, EventArgs e)
        {
            //string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
            //string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
            //string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
            string SFTPmainfolder = Convert.ToString(ConfigurationSettings.AppSettings["VirtualDirectoryDocs_apk"]);
            String ApkFileName = ConfigurationManager.AppSettings["ApkFileName"].ToString();

            bool endRequest = false;
            string fileName1 = (sender as LinkButton).CommandArgument;
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
                    clsApkDownload objApk = new clsApkDownload();
                    //ShowMsgBox("before");
                    string sFoldername = objApk.RetrieveLatestApkDetails();
                    // string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
                    // clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                    //string url = SFTPmainfolder + "ANDROID_APK_DOWNLOAD/" + sFoldername + "/" + fileName1;
                    //  ShowMsgBox(SFTPmainfolder);

                    string url = SFTPmainfolder + "ANDROID_APK_DOWNLOAD/" + sFoldername+ ApkFileName;
                   // string fileName = getFilename(url);
                    // ShowMsgBox(url);
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
                    resp.AddHeader("Content-Disposition", "attachment; filename=\"" + ApkFileName + "\"");
                    resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());
                    // ShowMsgBox("middle");
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
                // ShowMsgBox("finallu");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(404) Not Found"))
                {
                    lblMessage.Text="File Not Found";
                }
                else
                {

                    lblMessage.Text = clsException.ErrorMsg();
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                }

            }

        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/WebApp.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " WebUser Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkAndroidManual_Click(object sender, EventArgs e)
        {
            try
            {
                string Filename = MapPath("~/UserManual/Android.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Android Manual Download ");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkAndroidManual_Click1(object sender, EventArgs e)
        {
            //string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
            //string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
            //string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
            string SFTPmainfolder = Convert.ToString(ConfigurationSettings.AppSettings["FTP_HOST"]);
            String ApkFileName = ConfigurationManager.AppSettings["ApkFileName"].ToString();
            bool endRequest = false;
            string fileName1 = (sender as LinkButton).CommandArgument;
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
                    clsApkDownload objApk = new clsApkDownload();
                    //ShowMsgBox("before");
                    string sFoldername = objApk.RetrieveLatestApkDetails();
                    // string PoNo = Regex.Replace(txtPoNumber.Text, @"[^0-9a-zA-Z]+", "");
                    // clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);

                    //string url = SFTPmainfolder + "ANDROID_APK_DOWNLOAD/" + sFoldername + "/" + fileName1;
                    //  ShowMsgBox(SFTPmainfolder);

                    string url = SFTPmainfolder + sFoldername +"/"+ ApkFileName;
                    
                    // ShowMsgBox(url);
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
                    resp.AddHeader("Content-Disposition", "attachment; filename=\"" + ApkFileName + "\"");
                    resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());
                    // ShowMsgBox("middle");
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
                // ShowMsgBox("finallu");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(404) Not Found"))
                {
                    lblMessage.Text="File Not Found";
                }
                else
                {

                    lblMessage.Text = clsException.ErrorMsg();
                    clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                }

            }

        }
        protected void lnkAndroidApk_Click(object sender, EventArgs e)
        {
            try
            {
                clsApkDownload objApk = new clsApkDownload();   
                String FTP_HOST = ConfigurationManager.AppSettings["FTP_HOST"].ToString();
                String FTP_USER = ConfigurationManager.AppSettings["FTP_USER"].ToString();
                String FTP_PASS = ConfigurationManager.AppSettings["FTP_PASS"].ToString();
                String ApkFileName = ConfigurationManager.AppSettings["ApkFileName"].ToString();
                string sFoldername = objApk.RetrieveLatestApkDetails();
                FTP_HOST = FTP_HOST + sFoldername;


                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPapk_download"]);

                bool status = false;
                clsSFTP objFtp = new clsSFTP(SFTPPath, FTP_USER, FTP_PASS);
                status = objFtp.Download(SFTPPath + "/" + SFTPmainfolder + sFoldername + "/" + ApkFileName, ApkFileName);
                //status = objFtp.Download(SFTPmainfolder + sFoldername+"/"+ ApkFileName, ApkFileName);

                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_HOST + ApkFileName);
                //    request.Method = WebRequestMethods.Ftp.DownloadFile;

                //    //Enter FTP Server credentials.
                //    request.Credentials = new NetworkCredential(FTP_USER, FTP_PASS);
                //    request.UsePassive = true;
                //    request.UseBinary = true;
                //    request.EnableSsl = false;

                //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //    using (MemoryStream stream = new MemoryStream())
                //    {
                //        //Stream responseStream = response.GetResponseStream();
                //        response.GetResponseStream().CopyTo(stream);
                //        Response.AddHeader("content-disposition", "attachment;filename=" + ApkFileName);
                //        Response.ContentType = "application/msi";
                //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //        Response.BinaryWrite(stream.ToArray());
                //        Response.OutputStream.Close();
                //    }


                //string Filename = MapPath("~/UserManual/DTLMSAPK.zip");
                // This is an important header part that informs the client to download this file.
                //Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                //Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                //Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Android APK Download ");
                }
            }
            catch (Exception ex)
            {
             //   lblMessage.Text = clsException.ErrorMsg();
               // clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void LnkImgTansactionFlow_Click(object sender, EventArgs e)
        {
            try
            {
                string Filename = MapPath("~/UserManual/TransactionFlow.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
       

        protected void lnkCirDownload_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Circular.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Circular Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkProDownload_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Procedure.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Procedure Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public DataTable BindgridView(string FtpServer, string username, string password)
        {
            try
            {
                DataTable dtFiles = new DataTable();
                dtFiles.Columns.AddRange(new DataColumn[1] { new DataColumn("Name", typeof(string)) });
                WebRequest request = (WebRequest)WebRequest.Create(FtpServer);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream());
                string fileName3 = streamReader.ReadLine();

                List<string> directories = new List<string>();



                while (fileName3 != null && fileName3 != "testing12")
                {
                    dtFiles.Rows.Add(fileName3);
                    directories.Add(fileName3);
                    fileName3 = streamReader.ReadLine();


                }

                streamReader.Close();


                using (WebClient ftpClient = new WebClient())
                {
                    ftpClient.Credentials = new System.Net.NetworkCredential(username, password);

                    for (int i = 0; i < directories.Count; i++)
                    {
                        if (directories[i].Contains("."))
                        {

                            // string path = FtpServer + directories[i].ToString();
                            //string trnsfrpth = localpath + directories[i].ToString();
                            //ftpClient.DownloadFile(path, trnsfrpth);
                            gvFiles.DataSource = dtFiles;
                            gvFiles.DataBind();
                        }

                    }

                    
                }
                return dtFiles;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                return dtFiles;
            }
        }

        protected void DownloadFile(object sender, EventArgs e)
        {
            string sFileDownloadPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["CircularsVirtualPath"]);
            string sUserName = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_USER"]);
            string sPassword = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["FTP_PASS"]);
            string fileName = (sender as LinkButton).CommandArgument;

            try
            {

                // string sFileName = Path.GetFileName(fupAnx.PostedFile.FileName).Replace(",", "");

                //  FileUpload1.SaveAs(Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + fileName));
                // string sDirectory = Server.MapPath("~/DTLMSDocs" + "/" + objSession.UserId + "~" + fileName);
                // string localpath = "/Circulars/";
                //    DataTable dtDocs = new DataTable();
                //    dtDocs = (DataTable)ViewState["DOCUMENTS"];
                //    clsFtp objFtp = new clsFtp(sFileDownloadPath, sUserName, sPassword);

                //    objFtp.download(fileName,   + fileName);

                //    Response.Redirect("UploadCirculars.aspx", false);  
                //    }
                //    catch (WebException ex)
                //    {
                //        throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                //    }
                //}


                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sFileDownloadPath + fileName);
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
        protected void lnkAnx1Download_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Annexure1.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Annexure1RFT Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkAnx2Download_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Annexure2.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Annexure2JIRFT Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkAnx3Download_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Annexure3.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Annexure3JIRFT Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void lnkAnx4Download_Click(object sender, EventArgs e)
        {
            try
            {
                //To open the PDF and also download
                //string pdfPath = Server.MapPath("~/UserManual/1.pdf");
                //WebClient client = new WebClient();
                //Byte[] buffer = client.DownloadData(pdfPath);
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-length", buffer.Length.ToString());
                //Response.BinaryWrite(buffer);

                //Only To Download Pdf
                string Filename = MapPath("~/UserManual/Annexure4.pdf");
                // This is an important header part that informs the client to download this file.
                Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(Filename));
                Response.ContentType = "Application/pdf";
                //Write the file directly to the HTTP content output stream.
                Response.WriteFile(Filename);

                if (objSession.sTransactionLog == "1")
                {
                    Genaral.TransactionLog(objSession.sClientIP, objSession.UserId, " Annexure4AFT Manual Download ");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }




        public DataTable dtFiles { get; set; }
    }
}