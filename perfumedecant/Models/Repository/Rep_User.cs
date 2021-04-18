using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_User
    {
        PD_DB db = new PD_DB();

        public Tbl_User Get_User(int User_ID)
        {
            Tbl_User user = new Tbl_User();
            user = db.Tbl_User.Find(User_ID);
            return user;
        }

        public IEnumerable<Tbl_User> Get_AllUsers()
        {
            var users = db.Tbl_User.OrderByDescending(a => a.User_ID).ToList();
            return users.AsEnumerable();
        }

        public int Get_UserCount()
        {
            int count = db.Tbl_User.Distinct().Count();
            return count;
        }

        public Tbl_User Get_AdminUser()
        {
            var admin_role_id = db.Tbl_Role.Where(a => a.Role_Name == "Admin").SingleOrDefault().Role_ID;
            var admin = db.Tbl_User.Where(a => a.User_Role_ID == admin_role_id).SingleOrDefault();
            return admin;
        }

        public Tbl_User Get_User(string username)
        {
            Tbl_User user = new Tbl_User();
            user = db.Tbl_User.Where(a=>a.User_Username == username).SingleOrDefault();
            return user;
        }
    }
}