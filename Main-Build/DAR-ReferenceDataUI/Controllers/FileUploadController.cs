using DARReferenceData.DatabaseHandlers;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAR_ReferenceDataUI.Controllers
{
    public class FileUploadController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);


        // GET: FileUpload
        public ActionResult Index()
        {
            try
            {
                var r = ValidateUser();

                if (r == null)
                {
                    return View();
                }
                else
                {
                    return r;
                }
            }
            catch(Exception ex)
            {
                return RedirectToInsufficientAcess(ex.Message);
            }
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Basic_Usage_Submit(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                try
                {
                    Basic_Usage_Get_File_Info(files);
                }
                catch (Exception ex)
                {
                    Logger.Fatal("Failed to upload file.", ex);
                }
            }

            return Content("");
        }

        



        public ActionResult Events_Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        private IEnumerable<string> Basic_Usage_Get_File_Info(IEnumerable<HttpPostedFileBase> files)
        {
            List<string> uploadedFiles = new List<string>();
            string filePath = @"C:\temp\UploadedFiles";

            //Path.Combine(Server.MapPath("~/UploadedFiles"));
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (!di.Exists)
                di.Create();

            CryptoEvent ce = new CryptoEvent();
            string error;

            foreach (var f in files)
            {
                string p = Path.Combine(filePath, f.FileName);
                f.SaveAs(p);
                uploadedFiles.Add(f.FileName);
                try
                {
                    ce.LoadDataFromExcelFile(p, out error);
                }
                catch(Exception ex)
                {
                    //TODO return this error to frontend
                    Logger.Fatal(ex);
                }
                     

            }
            return uploadedFiles;
            
        }
    }
}