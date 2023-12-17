namespace Cofefe.Models
{
    public class Order
    {
        public int Id { get; set; }

        public User User { get; set; }
        public int UserID { get; set; }

        public Product Product { get; set; }
        public int ProductID { get; set; }

        public Status Status { get; set; }
        public int StatusID { get; set; }

        public int SumCost { get; set; }
        public int DeliveryCost { get; set; }
        public int TotalCost { get; set; }
        public DateTime DispatchDate { get; set; }
    }
}
