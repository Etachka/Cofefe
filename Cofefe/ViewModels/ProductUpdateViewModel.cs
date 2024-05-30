using Cofefe.Models;

namespace Cofefe.ViewModels
{
    public class ProductUpdateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public int Cost { get; set; }
        public string Acidity { get; set; }
        public string Density { get; set; }
        public string Growth { get; set; }
        public string Type { get; set; }

    }
}
