using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_Bill
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_UserBills> Get_NewBills()
        {
            var newBills = db.Tbl_UserBills.OrderByDescending(a=>a.UserBills_ID).Take(7);
            return newBills.AsEnumerable();
        }

        public int Get_Bill_Count()
        {
            int count = db.Tbl_UserBills.Distinct().Count();
            return count;
        }

        public IEnumerable<Tbl_Perfume> Get_BestSellers()
        {
            List<int> perfume_IDs = new List<int>();

            string connectionString = ConfigurationManager.ConnectionStrings["PD_DB1"].ConnectionString;

            DataTable dt = new DataTable();
            string commandText = "select TOP 4 Tbl_Perfume.Perfume_ID ,sum(Tbl_InterimBill.InterimBill_Count) as num"
                                    + " from Tbl_Perfume, Tbl_InterimBill "
                                    + " where Tbl_Perfume.Perfume_ID = Tbl_InterimBill.InterimBill_Perfume_ID"
                                    + " group by Tbl_Perfume.Perfume_ID"
                                    + " order by num desc";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd ) )
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                int rows_returned = sda.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    perfume_IDs.Add((int)dr[0]);
                }
                conn.Close();
            }

            List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();
            Tbl_Perfume p = new Tbl_Perfume();
            foreach (var id in perfume_IDs)
            {
                p = db.Tbl_Perfume.Where(a=>a.Perfume_ID == id).SingleOrDefault();
                perfumes.Add(p);
            }
            return perfumes.AsEnumerable();
        }
    }
}