using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class DevolutionAndProductsViewModel
    {
        public int DevolutionId { get; set; }

        public List<Product> Products { get; set; }

        public List<DevolutionImage>? Images { get; set; }
    }
}
