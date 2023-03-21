using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.ProductViewModel
{
    public class UpdateProductViewModel
    {
        public Product Product { get; set; }
        public string FileName { get; set; }
        public string BasePathImages { get; set; }
        public string Photo { get; set; }
    }
}
