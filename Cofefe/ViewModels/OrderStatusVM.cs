using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class OrderStatusVM
    {
        public List<Order> Orders { get; set; }
        public List<Status> Statuses { get; set; }
        public int CurrentPage { get; set; } // Текущая страница
        public int PageSize { get; set; } // Размер страницы
        public int TotalCount { get; set; } // Общее количество заказов
    }
}
