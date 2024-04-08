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

        

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                product.IsHidden = true;
                _context.SaveChanges();
                return RedirectToAction("AdminView") ;
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

        [HttpGet]
        public IActionResult AddProduct()
        {

            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(string title, string description, int weight, int cost, string acidity, string density, string growth,  string type)
        {
            if( title != null && description != null && weight != null && cost != null && acidity != null && density != null && growth != null && type != null)
            {
                using (var con = new ApplicationContext())
                {
                    con.Products.AddRange(new[]
                    {
                        new Product{
                            Name = title,
                            Description = description, 
                            Cost = cost, 
                            Weight = weight, 
                            Image = "", 
                            StockQuantity = 100},
                    });
                    con.SaveChanges();
                    var prod = _context.Products.FirstOrDefault(x=>x.Name ==title);
                    int prodID = prod.Id;
                    con.CategoryProducts.AddRange(new[]
                    {
                        new CategoryProduct{
                            CategoryID = 1,
                            ProductID = prodID,
                            Value = acidity
                        },
                        new CategoryProduct{
                            CategoryID = 2,
                            ProductID = prodID,
                            Value = density
                        },
                        new CategoryProduct{
                            CategoryID = 3,
                            ProductID = prodID,
                            Value = growth
                        },
                        new CategoryProduct{
                            CategoryID = 4,
                            ProductID = prodID,
                            Value = type
                        },
                        new CategoryProduct{
                            CategoryID = 5,
                            ProductID = prodID,
                            Value = 0.ToString()
                        },
                        new CategoryProduct{
                            CategoryID = 6,
                            ProductID = prodID,
                            Value = 0.ToString()
                        },
                    });
                    con.SaveChanges();
                }
            }

            return View("AdminView");
        }

        [HttpGet]
        public IActionResult CreateTestUser()
        {

            return View();
        }
        [HttpPost]
        public IActionResult CreateTestUser(string firstn, string secondn, string patronymic, string pass, string log, string number, string city, string street, string home, string flat)
        {
            if (firstn != null && secondn != null && patronymic != null && pass != null && log != null && number != null && city != null && street != null && home != null && flat != null)
            {
                using (var con = new ApplicationContext())
                {
                    con.Users.AddRange(new[]
                    {
                        new User{
                            FIO = firstn + " " + secondn.Substring(0,1) + "." + patronymic.Substring(0,1) + ".",
                            Password = pass,
                            Login = log,
                            Address = "г." + city + ", " + "ул." + street + ", " + "д." + home + ", " + "кв." + flat, 
                            PhoneNumber = number, 
                            role = 2, 
                        }
                    });
                    con.SaveChanges();
                }
            }
            return View("AdminView");
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
        public IActionResult RegistrationUser()
        {
            return View("Index");
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

