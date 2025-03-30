using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Shop.Models;

namespace SV21T1020218.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 12;

        /// <summary>
        /// Hiển thị danh sách sản phẩm trong _Section
        /// </summary>
        public IActionResult Index(int page = 1, string searchValue = "", 
            int categoryID = 0, int supplierID = 0, 
            decimal minPrice = 0, decimal maxPrice = decimal.MaxValue)
        {
            try
            {
                // Nếu maxPrice là 0, gán giá trị mặc định là decimal.MaxValue
                if (maxPrice == 0)
                    maxPrice = decimal.MaxValue;

                int rowCount;
                var data = ProductDataService.ListProducts(
                    out rowCount,
                    page,
                    PAGE_SIZE,
                    searchValue,
                    categoryID,
                    supplierID,
                    minPrice,
                    maxPrice
                );

                var result = new ProductSearchResult()
                {
                    Page = page,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue,
                    RowCount = rowCount,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice == decimal.MaxValue ? 0 : maxPrice,
                    Data = data
                };

                return View(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Index: {ex.Message}");
                return View(new ProductSearchResult()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue,
                    RowCount = 0,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Data = new List<Product>()
                });
            }
        }

        /// <summary>
        /// Xem chi tiết sản phẩm
        /// </summary>
        public IActionResult Details(int id)
        {
            var product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");

            // Lấy danh sách ảnh của sản phẩm
            var photos = ProductDataService.ListPhotos(id);
            // Lấy danh sách thuộc tính của sản phẩm
            var attributes = ProductDataService.ListAttributes(id);

            var model = new ProductDetailsViewModel()
            {
                Product = product,
                Photos = photos,
                Attributes = attributes
            };

            return View(model);
        }

        /// <summary>
        /// API lấy danh sách sản phẩm cho phân trang
        /// </summary>
        [HttpGet]
        public IActionResult List(ProductSearchInput input)
        {
            try
            {
                int rowCount;
                var data = ProductDataService.ListProducts(
                    out rowCount,
                    input.Page,
                    PAGE_SIZE,
                    input.SearchValue,
                    input.CategoryID,
                    input.SupplierID,
                    decimal.Parse(input.MinPrice),
                    decimal.Parse(input.MaxPrice)
                );

                var result = new ProductSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue,
                    RowCount = rowCount,
                    CategoryID = input.CategoryID,
                    SupplierID = input.SupplierID,
                    MinPrice = decimal.Parse(input.MinPrice),
                    MaxPrice = decimal.Parse(input.MaxPrice),
                    Data = data.ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}