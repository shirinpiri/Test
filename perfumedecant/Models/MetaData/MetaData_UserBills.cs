using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_UserBills
    {

        public int UserBills_ID { get; set; }


        [MetadataType(typeof(MetaData_UserBills))]
        public partial class Tbl_UserBills
        { }
    }
}