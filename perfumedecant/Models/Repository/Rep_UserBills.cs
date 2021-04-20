using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace perfumedecant.Models.Repository
{
    public class Rep_UserBills
    {
        PD_DB db = new PD_DB();

        public IEnumerable<Tbl_UserBills> Get_AllUserBills()
        {
            var qUserBills = db.Tbl_UserBills.OrderBy(a => a.UserBills_ID).Where(a=>a.Tbl_Bill.Bill_Status == true).ToList();
            return qUserBills.AsEnumerable();
        }
    }
}
