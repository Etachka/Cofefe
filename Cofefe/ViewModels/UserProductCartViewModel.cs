using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class UserProductCartViewModel
    {
        public List<ShoppingCart> ShoppingCartItems { get; set; }
        public List<Product> Products { get; set; }
        public User UserAuth { get; set; }


        public int PageSize { get; set; } // Размер страницы
        public int CurrentPage { get; set; } // Текущая страница
        public int TotalCount { get; set; } // Общее количество элементов
    }
}
