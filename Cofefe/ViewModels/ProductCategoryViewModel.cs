using Cofefe.Models;


namespace Cofefe.ViewModels
{
    public class ProductCategoryViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
        public List<int> FavoriteProductIDs { get; set; }

        // Свойства для пагинации
        public int PageSize { get; set; } // Размер страницы
        public int CurrentPage { get; set; } // Текущая страница
        public int TotalCount { get; set; } // Общее количество элементов
        public string SearchString { get; set; }
        public string Acidity { get; set; }
        public string Density { get; set; }
        public string Growth { get; set; }
        public string Type { get; set; }

    }
}
