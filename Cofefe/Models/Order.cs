namespace Cofefe.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public User User { get; set; }
        public int UserID { get; set; }

        public Product Product { get; set; }
        public int ProductID { get; set; }

        public Status Status { get; set; }
        public int StatusID { get; set; }

        public int ProductCount { get; set; }

        public int ProductCostAtTimeOrder { get; set; }


    }
}
