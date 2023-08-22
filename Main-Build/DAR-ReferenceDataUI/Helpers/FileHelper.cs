using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DAR_ReferenceDataUI.Helpers
{
    public class FileHelper
    {
        public void CleanupFolder(string targetFolder, string pattern, int numberOfDays = 7)
        {
            
       
            DirectoryInfo di = new DirectoryInfo(targetFolder);

            FileInfo[] fileList = di.GetFiles($"{pattern}");
            foreach(var f in fileList)
            {
                if(f.LastWriteTime < DateTime.Today.AddDays(numberOfDays*-1))
                {
                    f.Delete();
                }
            }


          
        }
    }
}