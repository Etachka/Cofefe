using Cofefe.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cofefe.ViewModels;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

using Cofefe.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

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

            ViewBag.CatalogType = "Products";
            return View("AdminView", VM);
        }
        [HttpPost]
        public IActionResult GetOrders()
        {
            ViewBag.CatalogType = "Orders";
            UserProductViewmodel VM = new UserProductViewmodel();
            VM.Users = _context.Users.ToList();
            VM.Products = _context.Products.ToList();
            VM.CategoryProducts = _context.CategoryProducts.ToList();
            VM.Categories = _context.Categoryes.ToList();
            VM.AllOrders = _context.Orders.Where(o => o.StatusID == 1).ToList();
            return View("AdminView", VM);
        }

        [HttpPost]
        public IActionResult GetSentOrders()
        {
            ViewBag.CatalogType = "SentOrders"; 
            UserProductViewmodel VM = new UserProductViewmodel();
            VM.Users = _context.Users.ToList();
            VM.Products = _context.Products.ToList();
            VM.CategoryProducts = _context.CategoryProducts.ToList();
            VM.Categories = _context.Categoryes.ToList();
            VM.SentOrders = _context.Orders.Where(o => o.StatusID == 2).ToList();
            return View("AdminView", VM);
        }

        

        [HttpPost]
        public IActionResult GetUsers()
        {
            ViewBag.CatalogType = "Users";
            UserProductViewmodel VM = new UserProductViewmodel();
            VM.Users = _context.Users.ToList();
            VM.Products = _context.Products.ToList();
            VM.CategoryProducts = _context.CategoryProducts.ToList();
            VM.Categories = _context.Categoryes.ToList();
            return View("AdminView", VM);
        }


        public ViewResult Index(string searchString, string acidity, string density, string growth, string type)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            List<int> favoriteProductIds = _context.FavoriteProducts
                .Where(fp => fp.UserID == userId)
                .Select(fp => fp.ProductID)
                .ToList();

            IQueryable<Product> query = _context.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(acidity))
            {
                var acidityProductIds = _context.CategoryProducts
                    .Where(cp => cp.CategoryID == 1 && cp.Value == acidity)
                    .Select(cp => cp.ProductID)
                    .ToList();

                if (acidityProductIds.Any())
                {
                    query = query.Where(p => acidityProductIds.Contains(p.Id));
                }
            }

            var densityProductIds = _context.CategoryProducts
                .Where(cp => cp.CategoryID == 2 && cp.Value == density)
                .Select(cp => cp.ProductID)
                .ToList();

            if (densityProductIds.Any())
            {
                query = query.Where(p => densityProductIds.Contains(p.Id));
            }

            var growthProductIds = _context.CategoryProducts
                .Where(cp => cp.CategoryID == 3 && cp.Value == growth)
                .Select(cp => cp.ProductID)
                .ToList();

            if (growthProductIds.Any())
            {
                query = query.Where(p => growthProductIds.Contains(p.Id));
            }

            var roastProductIds = _context.CategoryProducts
                .Where(cp => cp.CategoryID == 4 && cp.Value == type)
                .Select(cp => cp.ProductID)
                .ToList();

            if (roastProductIds.Any())
            {
                query = query.Where(p => roastProductIds.Contains(p.Id));
            }

            List<Product> searchResults = query.ToList();

            ProductCategoryViewModel VM = new ProductCategoryViewModel
            {
                Products = searchResults,
                CategoryProducts = _context.CategoryProducts.ToList(),
                Categories = _context.Categoryes.ToList(),
                FavoriteProductIDs = favoriteProductIds.ToList(),
            };

            return View(VM);
        }


        public IActionResult Address()
        {
            var usr = _context.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("UserId"));
            var userAddress = usr.Address;

            return View("Address", ParseAddress(userAddress));
            
        }


        public static Address ParseAddress(string addressString)
        {
            Address address = new Address();

            string[] parts = addressString.Split(',');

            foreach (var part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("г."))
                {
                    address.City = trimmedPart.Substring(2).Trim();
                }
                else if (trimmedPart.StartsWith("ул."))
                {
                    address.Street = trimmedPart.Substring(3).Trim();
                }
                else if (trimmedPart.StartsWith("д."))
                {
                    address.Home = trimmedPart.Substring(2).Trim();
                }
                else if (trimmedPart.StartsWith("кв."))
                {
                    address.Flat = trimmedPart.Substring(3).Trim();
                }
            }

            return address;
        }

        [HttpPost]
        public IActionResult AddressEdit(string city, string street, string home, string flat)
        {
           
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("UserId"));

            if (currentUser != null)
            {
                string address = "";

                if (!string.IsNullOrEmpty(city))
                    address += "г." + city;

                if (!string.IsNullOrEmpty(street))
                    address += ", ул." + street;

                if (!string.IsNullOrEmpty(home))
                    address += ", д." + home;

                if (!string.IsNullOrEmpty(flat))
                    address += ", кв." + flat;

                currentUser.Address = address;

                _context.SaveChanges();

                return View("Cabinet", currentUser);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> AddOrRemoveFavorite(int productId, bool isFavorite)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

                var favoriteProduct = await _context.FavoriteProducts.FirstOrDefaultAsync(fp => fp.ProductID == productId && fp.UserID == userId);
                if (favoriteProduct != null)
                {
                    _context.FavoriteProducts.Remove(favoriteProduct);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.FavoriteProducts.Add(new FavoriteProduct { ProductID = productId, UserID = userId.Value });
                    await _context.SaveChangesAsync();
                }

            return RedirectToAction("Index");
        }



        public ViewResult Favorite(string searchString, string acidity, string density, string growth, string type)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int userId = HttpContext.Session.GetInt32("UserId").Value;

                var favoriteProductIds = _context.FavoriteProducts
                    .Where(fp => fp.UserID == userId)
                    .Select(fp => fp.ProductID)
                    .ToList();

                var favoriteProducts = _context.Products
                    .Where(p => favoriteProductIds.Contains(p.Id))
                    .ToList();

                IQueryable<Product> query = favoriteProducts.AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(acidity))
                {
                    query = query.Where(p => _context.CategoryProducts
                        .Any(cp => cp.CategoryID == 1 && cp.ProductID == p.Id && cp.Value == acidity));
                }

                if (!string.IsNullOrEmpty(density))
                {
                    query = query.Where(p => _context.CategoryProducts
                        .Any(cp => cp.CategoryID == 2 && cp.ProductID == p.Id && cp.Value == density));
                }

                if (!string.IsNullOrEmpty(growth))
                {
                    query = query.Where(p => _context.CategoryProducts
                        .Any(cp => cp.CategoryID == 3 && cp.ProductID == p.Id && cp.Value == growth));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(p => _context.CategoryProducts
                        .Any(cp => cp.CategoryID == 4 && cp.ProductID == p.Id && cp.Value == type));
                }

                List<Product> searchResults = query.ToList();

                ProductCategoryViewModel VM = new ProductCategoryViewModel
                {
                    Products = searchResults,
                    CategoryProducts = _context.CategoryProducts.ToList(),
                    Categories = _context.Categoryes.ToList(),
                    FavoriteProductIDs = favoriteProductIds.ToList(),
                };

                return View(VM);
            }
            else
            {
                return View("Login");
            }

        }

        public IActionResult OrderList()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            OrderStatusVM VM = new OrderStatusVM
            {
                Orders = _context.Orders
                    .Where(o => o.UserID == userId)
                    .Include(o => o.Product)
                    .ToList(),
                Statuses = _context.Statuses.ToList(),
            
            };
            return View(VM);
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
                    UserAuth = _context.Users.FirstOrDefault(x=>x.Id == userId),
                    Products = _context.Products.ToList(),
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
            if (title != null && description != null && weight != null && cost != null && acidity != null && density != null && growth != null && type != null)
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
                    var prod = _context.Products.FirstOrDefault(x => x.Name == title);
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
                return View("AdminView");
            }
            else return View();
        }

        [HttpGet]
        public IActionResult CreateTestUser()
        {

            return View();
        }
        [HttpPost]
        public IActionResult CreateTestUser(string firstn, string secondn, string patronymic, string pass, string log, string number, string city, string street, string home, string flat)
        {
            string phone = Regex.Replace(number, @"[^\d]", "");
            if (firstn != null && secondn != null && patronymic != null && pass != null && log != null && phone != null && phone.Length == 11)
            {
                using (var con = new ApplicationContext())
                {
                    string address = "";
                    if (city != null || street != null || home != null || flat != null)
                    {
                        address = "г." + city + ", " + "ул." + street + ", " + "д." + home + ", " + "кв." + flat;
                    }
                    con.Users.AddRange(new[]
                    {
                        new User{
                            FIO = firstn + " " + secondn.Substring(0,1) + "." + patronymic.Substring(0,1) + ".",
                            Password = pass,
                            Login = log,
                            Address = address,
                            PhoneNumber = phone,
                            role = 2,
                        }
                    });
                    con.SaveChanges();
                }
            }
            return View("AdminView");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var existingItem = await _context.ShoppingCarts.FirstOrDefaultAsync(item => item.ProductID == productId && item.UserID == userId);

            if (existingItem != null)
            {
                existingItem.ProductCount++;
                _context.Update(existingItem);
            }
            else
            {
                ShoppingCart newItem = new ShoppingCart
                {
                    ProductID = productId,
                    UserID = userId.Value,
                    ProductCount = 1
                };
                _context.Add(newItem);
            }

            await _context.SaveChangesAsync();

            TempData["message"] = "Товар добавлен в корзину!";
            TempData["messageType"] = "success";

            return RedirectToAction(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var cartItem = await _context.ShoppingCarts.FirstOrDefaultAsync(item => item.ProductID == productId && item.UserID == userId);
            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var ordersCount = await _context.Orders.CountAsync(o => o.UserID == userId);
            if (ordersCount == 0)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminView");
            }
            else
            {
                TempData["ErrorMessage"] = "У пользователя есть заказы";
                return RedirectToAction("AdminView");
            }
            
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int UID = HttpContext.Session.GetInt32("UserId").Value;
                var User = _context.Users.FirstOrDefault(u => u.Id == UID);
                return View("Cabinet", User);
            }
            else { return View("Login"); }
        }

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var userService = new UserService(_context);
            var user = userService.GetUser(login, password);
            if(user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
            }
			

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

        [HttpGet]
        public IActionResult UpdateUser()
        {
            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var user = _context.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateUser(User updatedUser, string OldPassword)
        {
            var existingUser = _context.Users.Find(updatedUser.Id);
            if (existingUser != null)
            {
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.Login = updatedUser.Login;
                if (existingUser.Password == OldPassword && OldPassword != null && updatedUser.Password !=null)
                {
                    if (!string.IsNullOrEmpty(updatedUser.Password))
                    {
                        existingUser.Password = updatedUser.Password;
                        _context.SaveChanges();
                        return View("Cabinet", existingUser);
                    }
                }
                
                
            }
        
            return View(updatedUser); 
        }

        public IActionResult Cabinet()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult Registration()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Registration(string firstn, string secondn, string patronymic, string pass, string log, string number, string city, string street, string home, string flat)
        {
            if (firstn != null && secondn != null && patronymic != null && pass != null && log != null && number != null && number.Length == 11)
            {
                using (var con = new ApplicationContext())
                {
                    string address = "";
                    if (city != null || street != null || home != null || flat != null)
                    {
                        address = "г." + city + ", " + "ул." + street + ", " + "д." + home + ", " + "кв." + flat;
                    }
                    con.Users.AddRange(new[]
                    {
                        new User{
                            FIO = firstn + " " + secondn.Substring(0,1) + "." + patronymic.Substring(0,1) + ".",
                            Password = pass,
                            Login = log,
                            Address = address,
                            PhoneNumber = number,
                            role = 2,
                        }
                    });
                    con.SaveChanges();
                }
                
                return View("Login");
            }

            else
            {
                TempData["ErrorMessage"] = "Возникла ошибка при создании пользователя.";
                return View();
            }
                
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


        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCarts
                .Where(item => item.UserID == userId)
                .ToListAsync();

            if (cartItems.Any())
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == userId && x.Address != "");
                if (user != null)
                {
                    int orderId = GenerateOrderId();

                    int statusId = 1;

                    foreach (var item in cartItems)
                    {
                        var cartItem = _context.Products.FirstOrDefault(x => x.Id == item.ProductID);
                        int PCATO = cartItem.Cost;
                        var order = new Order
                        {
                            UserID = userId.Value,
                            ProductID = item.ProductID,
                            StatusID = statusId,
                            OrderId = orderId,
                            ProductCount = item.ProductCount,
                            ProductCostAtTimeOrder = PCATO,
                        };
                        _context.Orders.Add(order);
                    }

                    await _context.SaveChangesAsync();

                    _context.ShoppingCarts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("Cart");
            }
            
        }

        private int GenerateOrderId()
        {
            return (int)DateTime.Now.Ticks;
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(int orderId, string status)
        {
            var orders = _context.Orders.Where(o => o.OrderId == orderId).ToList();
            if (orders == null)
            {
                return RedirectToAction("Index");
            }
            switch (status)
            {
                case "send":
                    foreach (var order in orders)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == order.ProductID && order.StatusID == 1);
                        product.StockQuantity -= order.ProductCount;
                        _context.Products.Update(product);
                        order.StatusID = 2;
                    }
                    break;
                case "cancel":
                    foreach (var order in orders)
                    {
                        order.StatusID = 3;
                        
                    }
                    break;
                default:
                    
                    break;
            }

            _context.SaveChanges();

            return RedirectToAction("AdminView");
        }


        [HttpPost]
        public IActionResult DecreaseProductCount(int productID, int userId)
        {
            var shoppingCartItem = _context.ShoppingCarts
                .FirstOrDefault(item => item.ProductID == productID && item.UserID == userId);

            if (shoppingCartItem != null)
            {
                shoppingCartItem.ProductCount--;

                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult IncreaseProductCount(int productID, int userId)
        {
            var shoppingCartItem = _context.ShoppingCarts
                .FirstOrDefault(item => item.ProductID == productID && item.UserID == userId);

            if (shoppingCartItem != null)
            {
                shoppingCartItem.ProductCount++;

                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateProductCount(int productId, int userId, int newCount)
        {
            var shoppingCartItem = _context.ShoppingCarts
                .FirstOrDefault(item => item.ProductID == productId && item.UserID == userId);

            if (shoppingCartItem != null)
            {
                shoppingCartItem.ProductCount = newCount;

                _context.SaveChanges();
            }

            return Ok();
        }




        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            var product = _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductUpdateViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Weight = p.Weight,
                    Cost = p.Cost,
                    Acidity = _context.CategoryProducts.FirstOrDefault(cp => cp.ProductID == id && cp.CategoryID == 1).Value,
                    Density = _context.CategoryProducts.FirstOrDefault(cp => cp.ProductID == id && cp.CategoryID == 2).Value,
                    Growth = _context.CategoryProducts.FirstOrDefault(cp => cp.ProductID == id && cp.CategoryID == 3).Value,
                    Type = _context.CategoryProducts.FirstOrDefault(cp => cp.ProductID == id && cp.CategoryID == 4).Value
                }).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateProduct(ProductUpdateViewModel model)
        {
            if (model.Name != null && model.Description != null && model.Weight != null && model.Cost != null && model.Acidity != null && model.Density != null && model.Growth != null && model.Type != null)
            {
                using (var con = new ApplicationContext())
                {
                    var product = con.Products.FirstOrDefault(p => p.Id == model.Id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Cost = model.Cost;
                    product.Weight = model.Weight;

                    con.SaveChanges();

                    var categoryProducts = con.CategoryProducts.Where(cp => cp.ProductID == model.Id).ToList();

                    UpdateCategoryProductValue(categoryProducts, 1, model.Acidity);
                    UpdateCategoryProductValue(categoryProducts, 2, model.Density);
                    UpdateCategoryProductValue(categoryProducts, 3, model.Growth);
                    UpdateCategoryProductValue(categoryProducts, 4, model.Type);

                    con.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        private void UpdateCategoryProductValue(List<CategoryProduct> categoryProducts, int categoryId, string value)
        {
            var categoryProduct = categoryProducts.FirstOrDefault(cp => cp.CategoryID == categoryId);
            if (categoryProduct != null)
            {
                categoryProduct.Value = value;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

