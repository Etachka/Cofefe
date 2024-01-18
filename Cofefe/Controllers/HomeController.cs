using Cofefe.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cofefe.ViewModels;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

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
        public ViewResult Index()
        {
            ProductCategoryViewModel VM = new ProductCategoryViewModel
            {
                Products = _context.Products.ToList(),
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

            return View();
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

