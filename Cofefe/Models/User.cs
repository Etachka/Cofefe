namespace Cofefe.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string PhoneNumber { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int role { get; set; }
    }
}
