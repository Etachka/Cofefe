using Cofefe.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cofefe.ViewModels;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

using Cofefe.Data;

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
            VM.CategoryProducts = _context.CategoryProducts.ToList();
            VM.Categories = _context.Categoryes.ToList();

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
            VM.CategoryProducts = _context.CategoryProducts.ToList();
            VM.Categories = _context.Categoryes.ToList();
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
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View();
            }
            else { return View("Login"); }
            
        }
        public ViewResult Cart()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var cartItems = _context.ShoppingCarts
                    .Where(cart => cart.UserID == userId)
                    .Include(cart => cart.Product)
                    .ToList();
                UserProductCartViewModel VM = new UserProductCartViewModel
                {
                    ShoppingCartItems = cartItems,
                };
                return View(VM);
            }
            else { return View("Login"); }
        }
        public ViewResult AdminView()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View();
            }
            else { return View("Login"); }
        }
        public ViewResult Order()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return View("Cabinet");
            }
            else { return View("Login"); }
        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                product.IsHidden = true;
                _context.SaveChanges();
                TempData["Message"] = "Product deleted successfully!";
                return RedirectToAction("AdminView");
            }
            else
            {
                return Content("Product not found!");
            }
        }

        [HttpPost]
        public IActionResult VisibleProduct(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                product.IsHidden = false;
                _context.SaveChanges();
                TempData["Message"] = "Product visible successfully!";
                return RedirectToAction("AdminView");
            }
            else
            {
                return Content("Product not found!");
            }
        }

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var userService = new UserService(_context);
            var user = userService.GetUser(login, password);
			HttpContext.Session.SetInt32("UserId", user.Id);

			if (HttpContext.Session.GetInt32("UserId") != 1 && user != null)
            {
                return RedirectToAction("Index", "Home");
            }
			else if (HttpContext.Session.GetInt32("UserId") == 1)
			{
				return View("AdminView");
			}

			else
			{
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }
        public ViewResult Cabinet()
        {

            return View();
        }
        public ViewResult Registration()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

