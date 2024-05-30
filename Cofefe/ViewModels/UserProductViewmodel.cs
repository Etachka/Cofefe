using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class UserProductViewmodel
    {
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
        public List<Category> Categories { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
        public List<Order> AllOrders {  get; set; }
        public List<Order> SentOrders { get; set; }
    }
}
