using Cofefe.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cofefe.ViewModels;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace Cofefe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        List<Product> products = new List<Product>();
        List<User> users = new List<User>();

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult GetProducts()
        {
            UserProductViewmodel VM = new UserProductViewmodel();
            VM.Users = _context.Users.ToList();
            VM.Products = _context.Products.ToList();
            ViewBag.IsUsersCatalog = true;
            return View("AdminView", VM);
        }
        [HttpPost]
        public IActionResult GetUsers()
        {
            ViewBag.IsUsersCatalog = false;
            UserProductViewmodel VM = new UserProductViewmodel();
            VM.Users = _context.Users.ToList();
            VM.Products = _context.Products.ToList();

            return View("AdminView", VM);
        }

       
        public ViewResult Index(string searchString)
        {
            List<Product> searchResults;
            if (string.IsNullOrEmpty(searchString))
            {
                searchResults = _context.Products.ToList();
            }
            else
            {
                searchResults = _context.Products
                    .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                    .ToList();
            }
            ProductCategoryViewModel VM = new ProductCategoryViewModel
            {
                Products = searchResults,
                CategoryProducts = _context.CategoryProducts.ToList(),
                Categories = _context.Categoryes.ToList()
            };

            return View(VM);
        }
        public ViewResult Favorite()
        {

            return View();
        }
        public ViewResult Cart()
        {
            UserProductCartViewModel VM = new UserProductCartViewModel
            {
                users = _context.Users.ToList(),
                products = _context.Products.ToList(),
                shoppingCarts = _context.ShoppingCarts.ToList()
            };
            return View(VM);
        }
        public ViewResult AdminView()
        {

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

