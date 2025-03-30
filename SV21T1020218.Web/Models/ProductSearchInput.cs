using SV21T1020218.DomainModels;

namespace SV21T1020218.Web.Models
{
    /// <summary>
    /// Lớp chứa thông tin đầu vào cho chức năng tìm kiếm và phân trang sản phẩm
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// ID danh mục cần tìm kiếm
        /// </summary>
        public int CategoryID { get; set; } = 0;

        /// <summary>
        /// ID nhà cung cấp cần tìm kiếm
        /// </summary>
        public int SupplierID { get; set; } = 0;

        /// <summary>
        /// Giá tối thiểu cần tìm kiếm
        /// </summary>
        public string MinPrice { get; set; } = "";

        /// <summary>
        /// Giá tối đa cần tìm kiếm
        /// </summary>
        public string MaxPrice { get; set; } = "";

        public List<Category> Categories { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public List<Province> Provinces { get; set; }
        public List<Customer> Customers { get; set; }

    }
}
