using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeDemo.Models
{
	/// <summary>
	/// Stand-in for a list of products from a database
	/// </summary>
	public class Products
	{
		public static List<Product> FindAll()
		{
			var products = new List<Product>
			{
				new Product { Name = "Dog chow", Code = "123617" },
				new Product { Name = "Cat chow", Code = "9976AAFG4459" }
			};

			return products;
		}

		public static Product Find(string code)
		{
			var products = FindAll();
			var product = products.Single(p => p.Code == code);
			return product;
		}
	}
}