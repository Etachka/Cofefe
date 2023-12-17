namespace Cofefe.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public int StockQuantity { get; set; }
        public int Cost { get; set; }
        public string Image { get; set; }
    }
}
