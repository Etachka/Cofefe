namespace Cofefe.Models
{
    public class CategoryProduct
    {
        public int Id { get; set; }

        public Product Product { get; set; }
        public int ProductID { get; set; }

        public Category Category { get; set; }
        public int CategoryID { get; set; }

        public string Value { get; set; }
    }
}
