namespace Cofefe.Models
{
    public class FavoriteProduct
    {
        public int Id { get; set; }

        public User User { get; set; }
        public int UserID { get; set; }

        public Product Product { get; set; }
        public int ProductID { get; set; }
    }
}
