using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models
{
    public class LogStatus
    {

        public LogStatus() { }

        private string eventLog;
        public string EventLog
        {
            get { return "Event"; }
            set { this.eventLog = "Event"; }
        }

        private string errorLog;
        public string ErrorLog
        {
            get { return "Error"; }
            set { this.errorLog = "Error"; }
        }
    }
}