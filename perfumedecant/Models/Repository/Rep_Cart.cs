using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_Cart
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_InterimBill> Get_UserCart(string username)
        {
            int userID = db.Tbl_User.Where(a=>a.User_Username == username).SingleOrDefault().User_ID;
            var carts = db.Tbl_InterimBill.Where(a => a.InterimBill_User_ID == userID && a.InterimBill_Status == false).ToList();
            return carts.AsEnumerable();
        }
    }
}