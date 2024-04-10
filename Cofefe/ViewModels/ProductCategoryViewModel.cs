using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class ProductCategoryViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set;}
        public List<CategoryProduct> CategoryProducts { get; set; }
        public List<int> FavoriteProductIDs { get; set;}
    }
}
