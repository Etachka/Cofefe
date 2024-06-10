using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class UserProductCartViewModel
    {
        public List<ShoppingCart> ShoppingCartItems { get; set; }
        public List<Product> Products { get; set; }
        public User UserAuth { get; set; }
    }
}
