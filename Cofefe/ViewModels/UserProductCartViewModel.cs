using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class UserProductCartViewModel
    {
        public List<User> users = new List<User>();
        public List<Product> products = new List<Product>();
        public List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
    }
}
