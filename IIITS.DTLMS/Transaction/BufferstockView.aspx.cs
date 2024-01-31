using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IIITS.DTLMS.BL;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using System.Configuration;
using System.Net;


namespace IIITS.DTLMS.Transaction
{
    public partial class BufferstockView : System.Web.UI.Page
    {
        string strFormCode = "Bufferstockview";
        clsSession objSession;
        int Zone_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Zone_code"]);
        int Circle_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Circle_code"]);
        int Division_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Division_code"]);
        //int SubDiv_code = Convert.ToInt32(ConfigurationSettings.AppSettings["SubDiv_code"]);
        //int Section_code = Convert.ToInt32(ConfigurationSettings.AppSettings["Section_code"]);
        //int Feeder_code = Convert.ToInt32(ConfigurationSettings.AppSettings["feeder_code"]);

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

                if (!IsPostBack)
                {

                    objSession = (clsSession)Session["clsSession"];
                    CheckAccessRights("2");
                    string stroffCode = string.Empty;
                    if (objSession.OfficeCode.Length <= 2 && objSession.OfficeCode.Length != 0)
                    {
                        stroffCode = clsStoreOffice.GetZone_Circle_Div_Offcode(objSession.OfficeCode, objSession.RoleId).Substring(0, Constants.Circle);
                    }
                    else
                    {
                        stroffCode = objSession.OfficeCode;
                    }
                    // Genaral.Load_Combo("SELECT FD_FEEDER_CODE,FD_FEEDER_CODE || '-' || FD_FEEDER_NAME FROM TBLFEEDERMAST,TBLFEEDEROFFCODE where FDO_FEEDER_ID=FD_FEEDER_ID and FDO_OFFICE_CODE like '" + stroffCode + "%'  ORDER BY FD_FEEDER_CODE ", "--Select--", cmbFeederName);


                    string stroffCode1 = stroffCode;
                    if (stroffCode == null || stroffCode == "")
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                    }

                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"ZO_ID\",\"ZO_NAME\" FROM \"TBLZONE\" ORDER BY \"ZO_ID\"", "--Select--", cmbZone);
                        stroffCode = stroffCode.Substring(0, Zone_code);
                        cmbZone.Items.FindByValue(stroffCode).Selected = true;
                        cmbZone.Enabled = true;
                        stroffCode = stroffCode1;
                    }
                    if (stroffCode.Length >= 1)
                    {
                        Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);

                        if (stroffCode.Length >= 2)
                        {
                            stroffCode = stroffCode.Substring(0, Circle_code);
                            cmbCircle.Items.FindByValue(stroffCode).Selected = true;
                            cmbCircle.Enabled = true;
                            stroffCode = stroffCode1;

                        }
                    }

                    if (stroffCode.Length >= 2)
                    {
                        Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE CAST(\"DIV_CICLE_CODE\" AS TEXT)='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);
                        if (stroffCode.Length >= 3)
                        {
                            stroffCode = stroffCode.Substring(0, Division_code);
                            cmbDiv.Items.FindByValue(stroffCode).Selected = true;
                            cmbDiv.Enabled = true;
                            stroffCode = stroffCode1;

                            cmbZone.Enabled = false;
                            cmbCircle.Enabled = false;
                            cmbDiv.Enabled = false;
                            cmdReset.Visible = false;

                        }
                    }
                    txtFromDate1.Attributes.Add("readonly", "readonly");
                    txtToDate1.Attributes.Add("readonly", "readonly");

                    txtFromDate_CalendarExtender1.EndDate = System.DateTime.Now;
                    txtToDate_CalendarExtender1.EndDate = System.DateTime.Now;



                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    objbufferstock.sofficecode = stroffCode;
                    LoadBufferStockDetails(objbufferstock);


                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public bool CheckAccessRights(string sAccessType)
        {
            try
            {
                // 1---> ALL ; 2---> CREATE ;  3---> MODIFY/DELETE ; 4 ----> READ ONLY

                clsApproval objApproval = new clsApproval();

                objApproval.sFormName = "Bufferstockview";
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
        protected void cmbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbZone.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"CM_CIRCLE_CODE\",\"CM_CIRCLE_NAME\" FROM \"TBLCIRCLE\" WHERE \"CM_ZO_ID\"='" + cmbZone.SelectedValue + "'", "--Select--", cmbCircle);
                    cmbDiv.Items.Clear();

                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    objbufferstock.sofficecode = cmbZone.SelectedValue; ;
                    LoadBufferStockDetails(objbufferstock);
                    txtFromDate1.Text = null;
                    txtToDate1.Text = null;
                }

                else
                {
                    cmbCircle.Items.Clear();
                    cmbDiv.Items.Clear();

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmbCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (cmbCircle.SelectedIndex > 0)
                {
                    Genaral.Load_Combo("SELECT \"DIV_CODE\",\"DIV_NAME\" FROM \"TBLDIVISION\" WHERE \"DIV_CICLE_CODE\"='" + cmbCircle.SelectedValue + "'", "--Select--", cmbDiv);

                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    objbufferstock.sofficecode = cmbCircle.SelectedValue; ;
                    LoadBufferStockDetails(objbufferstock);
                    txtFromDate1.Text = null;
                    txtToDate1.Text = null;
                }
                else
                {

                    cmbDiv.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        protected void cmbDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDiv.SelectedIndex > 0)
                {
                    txtFromDate1.Text = null;
                    txtToDate1.Text = null;
                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    objbufferstock.sofficecode = cmbDiv.SelectedValue; ;
                    LoadBufferStockDetails(objbufferstock);


                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void cmdLoad_click(object sender, EventArgs e)
        {
            try
            {
                clsBufferstockdetails objbufferstock = new clsBufferstockdetails();

                string sResult = string.Empty;
                if (txtFromDate1.Text != "")
                {
                    sResult = Genaral.DateValidation(txtFromDate1.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtFromDate1.Focus();
                        return;
                    }
                }

                if (txtToDate1.Text != "")
                {
                    sResult = Genaral.DateValidation(txtToDate1.Text);
                    if (sResult != "")
                    {
                        ShowMsgBox(sResult);
                        txtToDate1.Focus();
                        return;
                    }
                }

                if (txtFromDate1.Text != "" && txtToDate1.Text != "")
                {
                    sResult = Genaral.DateComparision(txtToDate1.Text, txtFromDate1.Text, false, false);
                    if (sResult == "2")
                    {
                        ShowMsgBox("To Date should be Greater than From Date");
                        txtToDate1.Focus();
                        return;

                    }
                }

                objbufferstock.sFromDate = txtFromDate1.Text;
                objbufferstock.sTodate = txtToDate1.Text;

                if(objbufferstock.sFromDate == "" && objbufferstock.sTodate == "")
                {
                    objbufferstock.sFromDate = null;
                    objbufferstock.sTodate = null;

                }

                if (cmbZone.SelectedValue != "" && cmbZone.SelectedValue != "--Select--")
                {
                    objbufferstock.sofficecode = cmbZone.SelectedValue;
                }

                if (cmbCircle.SelectedValue != "" && cmbCircle.SelectedValue != "--Select--")
                {
                    objbufferstock.sofficecode = cmbCircle.SelectedValue;
                }


                if (cmbDiv.SelectedValue!="" && cmbDiv.SelectedValue != "--Select--")
                {
                    objbufferstock.sofficecode = cmbDiv.SelectedValue;
                }

                LoadBufferStockDetails(objbufferstock);
            }

              catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        private void LoadBufferStockDetails(clsBufferstockdetails objbufferstock)
        {
            try
            {


                //objbufferstock.sUserid = objSession.UserId;
                objbufferstock.GetBufferstockDetails(objbufferstock);
                ViewState["BUFFERSTOCKDETAILS"] = objbufferstock.dtbufferstockDetails;
                grdBufferStockDetails.DataSource = objbufferstock.dtbufferstockDetails;
                grdBufferStockDetails.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        protected void grdBufferStockDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                grdBufferStockDetails.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["BUFFERSTOCKDETAILS"];
                grdBufferStockDetails.DataSource = SortDataTable(dt as DataTable, true);
                grdBufferStockDetails.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
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
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GridViewSortDirection);
                        ViewState["PODETAILS"] = dataView.ToTable();
                    }
                    else
                    {
                        dataView.Sort = string.Format("{0} {1}", GridViewSortExpression, GetSortDirection());
                        ViewState["PODETAILS"] = dataView.ToTable();
                    }
                }
                return dataView;
            }
            else
            {
                return new DataView();
            }
        }

        protected void grdBufferStockDetails_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                GridViewSortExpression = e.SortExpression;
                int pageIndex = grdBufferStockDetails.PageIndex;
                DataTable dt = (DataTable)ViewState["BUFFERSTOCKDETAILS"];
                string sortingDirection = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    grdBufferStockDetails.DataSource = SortDataTable(dt as DataTable, false);
                }
                else
                {
                    grdBufferStockDetails.DataSource = dt;
                }
                grdBufferStockDetails.DataBind();
                grdBufferStockDetails.PageIndex = pageIndex;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
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

        protected void DownloadFile(object sender, EventArgs e)
        {

            string fileName = (sender as LinkButton).CommandArgument;

            try
            {
                bool status = false;
                string SFTPPath = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SFTPPath"]);
                string SFTPmainfolder = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["VirtualDirectoryDocs"]);

               
                clsSFTP objFtp = new clsSFTP(SFTPPath, sUserName, sPassword);
                //status = objFtp.Download(sFileServerPath + "/PO_DOCS/" + PoNo + "/" + fileName, fileName);

                //string path = SFTPmainfolder + "PO_DOCS/" + PoNo + "/" + fileName;
                string path = SFTPmainfolder + sMainfolder + "BUFFERSTOCKDETAILS/" + fileName;
                RegisterStartupScript("Print", "<script>window.open('" + path + "','_blank')</script>");

            }
            catch (WebException ex)
            {
                //  throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
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

        protected void DownloadFiledwnld(object sender, EventArgs e)
        {


            bool endRequest = false;
             string fileName1 = (sender as LinkButton).CommandArgument;

            clsSFTP objFtp = new clsSFTP(sFileServerPath, sUserName, sPassword);
            //string fileName1 = objFtp.GetFileName(sMainfolder + "BUFFERSTOCKDETAILS" + "/" + "UPLOAD_BUFFERSTOCK_DETAILS/");
            //string fileName1 = "SELECT \"FILE_PATH\" FROM \"TBLBUFFERSTOCKDETAILS\" WHERE \"LOCATION\" =  '" + objSession.OfficeCode + "' ";
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

                        string url = SFTPmainfolder + sMainfolder + "BUFFERSTOCKDETAILS/" + "UPLOAD_BUFFERSTOCK_DETAILS/" + fileName1;
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

        protected void cmdReset_Click(object sender, EventArgs e)
        {
            try
            {
                cmbZone.SelectedIndex = 0;
                //cmbCircle.SelectedIndex = 0;
                //cmbDiv.SelectedIndex = 0;
                cmbCircle.Items.Clear();
                cmbDiv.Items.Clear();
                txtFromDate1.Text = null;
                txtToDate1.Text = null;

                    clsBufferstockdetails objbufferstock = new clsBufferstockdetails();
                    //objbufferstock.sofficecode = stroffCode;
                    LoadBufferStockDetails(objbufferstock);

            }
            catch (Exception ex)
            {
                lblMessage.Text = clsException.ErrorMsg();
                clsException.LogError(strFormCode, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


    }
}