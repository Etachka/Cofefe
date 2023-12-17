namespace Cofefe.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public User user { get; set; }
        public int UserID { get; set; }

        public Product product { get; set; }
        public int ProductID { get; set; }

        public string Text { get; set; }
    }
}
