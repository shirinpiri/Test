using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace perfumedecant.Models
{
    public class Log
    {
        public Boolean addLog(String Message, String ActionName, String ViewName, String status)
        {
            String finalPath = System.Web.HttpContext.Current.Server.MapPath("/Files/");

            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(finalPath, "Logs.txt"), true))
                {

                    outputFile.WriteLine("Status: " + status + " ** Action name: " + ActionName + " ** ViewName: " + ViewName + " ** Message: " + Message);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}