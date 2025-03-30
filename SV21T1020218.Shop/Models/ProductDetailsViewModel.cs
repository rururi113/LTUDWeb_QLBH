using SV21T1020218.DomainModels;

namespace SV21T1020218.Shop.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public IList<ProductPhoto> Photos { get; set; } = new List<ProductPhoto>();
        public IList<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    }
}