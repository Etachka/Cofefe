using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class UserProductViewmodel
    {
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
        public List<Category> Categories { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
    }
}
