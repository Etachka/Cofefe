using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class OrderStatusVM
    {
        public List<Order> Orders { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
