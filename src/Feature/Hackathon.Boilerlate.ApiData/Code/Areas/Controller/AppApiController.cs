using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Mvc.Controllers;

namespace Hackathon.Boilerlate.Api.Areas.Controller
{
    
    public class AppApiController : SitecoreController
    {
        // GET: Controller/AppApi
        public static Database GetMaster()
        {
            return Factory.GetDatabase("master");
        }
        public JsonResult GetProducts()
        {
            List<Product> products = new List<Product>();
            var master = GetMaster();
            var ProductFolderItem = master.GetItem(new ID("{BAB788B2-5964-46A7-9431-5B8385AC1C0C}"));
            var productchildren = ProductFolderItem.GetChildren();
            foreach (var product in productchildren.ToList())
            {
                Product productItem = new Product();
                productItem.ID = product.Fields["ID"].ToString();
                productItem.Name = product.Fields["Name"].ToString();
                productItem.Description = product.Fields["Description"].ToString();
                productItem.Price = product.Fields["Price"].ToString();
                products.Add(productItem);
            }
            return Json(new { ProductList = products }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNews()
        {
            List<News> news = new List<News>();
            var master = GetMaster();
            var NewsFolderItem = master.GetItem(new ID("{F7D269DF-8678-4059-A7E1-7CA19DBE6B23}")); //News Id to be replaced
            var newsChildren = NewsFolderItem.GetChildren();
            foreach (var newss in newsChildren.ToList())
            {
                News newsItem = new News();
                newsItem.ID = newss.Fields["ID"].ToString();
                newsItem.Name = newss.Fields["Name"].ToString();
                newsItem.Description = newss.Fields["Description"].ToString();
                newsItem.Date = newss.Fields["Date"].ToString(); // Date to be changed
                news.Add(newsItem);
            }
            return Json(new { News = news }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlogs()
        {
            List<Blogs> BlogsList = new List<Blogs>();
            var master = GetMaster();
            var BlogFolderItem = master.GetItem(new ID("{AD844F71-1317-4943-92E7-30BCD12BE36D}")); // Id to be replaced
            var BlogChildren = BlogFolderItem.GetChildren();
            foreach (var blog in BlogChildren.ToList())
            {
                Blogs blogItem = new Blogs();
                blogItem.ID = blog.Fields["ID"].ToString();
                blogItem.Name = blog.Fields["Name"].ToString();
                blogItem.Description = blog.Fields["Description"].ToString();
                blogItem.Date = blog.Fields["Date"].ToString(); // Date to be changed
                BlogsList.Add(blogItem);
            }
            return Json(new { Blogs = BlogsList }, JsonRequestBehavior.AllowGet);
        }
    }

    public class Product
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
    }

    public class News
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }

    public class Blogs
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }
}