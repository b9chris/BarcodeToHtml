using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BarcodeDemo.Models;


namespace BarcodeDemo.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
			var products = Products.FindAll();
            return View(products);
        }

		public ViewResult ProductLabel(string id)
		{
			var product = Products.Find(id);
			return View(product);
		}
    }
}
