using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_Article
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_Article> Get_AllArticles()
        {
            var article = db.Tbl_Article.OrderByDescending(a=>a.Article_ID).ToList();
            return article.AsEnumerable();
        }
    }
}