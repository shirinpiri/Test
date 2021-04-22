using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_PostType
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_PostType> Get_AllPostTypes()
        {
            var qPostTypes = db.Tbl_PostType.OrderBy(a => a.PostType_ID).ToList();
            return qPostTypes.AsEnumerable();
        }
    }
}