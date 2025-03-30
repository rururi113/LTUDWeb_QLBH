using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;

namespace SV21T1020218.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.SALE}")]
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        private const string SHOPPING_CART = "ShoppingCart";

        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-700).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }


        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View(model);
        }

        /// <summary>
        /// Xóa 1 mặt hàng trong đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productID">Mã mặt hàng</param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productID);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id });
        }



        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái được duyệt 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
                ViewBag.Message = "Không thể duyệt đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái kết thúc
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                ViewBag.Message = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái từ chối
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                ViewBag.Message = "Không thể từ chối đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Xóa 1 đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
                ViewBag.Message = "Không thể xóa đơn hàng này!";
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái bị hủy
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                ViewBag.Message = "Không thể hủy đơn hàng này!";
            return RedirectToAction("Details", new { id });
        }


        /// <summary>
        /// Giao diện chọn người giao hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderId = id;
            return View();
        }
        /// <summary>
        /// Ghi nhận người giao hàng và chuyển trạng thái đơn hàng sang đang giao
        /// Trả về chuỗi khác rỗng nếu có lỗi hoặc đầu vào không hợp lệ
        /// Trả về chuỗi rỗng nếu thành công
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="shipperID">Mã người giao hàng</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọn người giao hàng");

            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng");

            return Json("");

        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            // Kiểm tra giá bán và số lượng có hợp lệ hay không
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            // Lấy giỏ hàng từ session
            var shoppingCart = GetShoppingCart();

            // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);

            if (existsProduct == null)
            {
                // Nếu sản phẩm chưa có, thêm vào giỏ hàng
                shoppingCart.Add(item);
            }
            else
            {
                // Nếu sản phẩm đã tồn tại, cập nhật số lượng và giá bán
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }

            // Lưu giỏ hàng vào session
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            ApplicationContext.SetSessionData("Status", 1);
            // Trả về kết quả JSON rỗng
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            // Lấy giỏ hàng từ session
            var shoppingCart = GetShoppingCart();

            // Tìm vị trí của sản phẩm trong giỏ hàng theo ProductID
            int index = shoppingCart.FindIndex(m => m.ProductID == id);

            // Nếu sản phẩm tồn tại trong giỏ hàng, xóa nó
            if (index >= 0)
                shoppingCart.RemoveAt(index);

            // Cập nhật giỏ hàng trong session
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            // Trả về kết quả JSON rỗng
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống.Vui lòng chọn mặt hàng cần bán");

            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");

            int employeeID = 1; //TODO: Thay bởi ID của nhân viên đang login vào hệ thống

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }

        /// <summary>
        /// Cập nhật giá bán và số lượng mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="orderID">Mã đơn hàng</param>
        /// <param name="productID">Mã sản phẩm</param>
        /// <param name="Quantity">Số lượng bán</param>
        /// <param name="salePrice">Giá bán</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(int orderID = 0, int productID = 0, int Quantity = 0, decimal salePrice = 0)
        {
            if (Quantity <= 0)
                return Json("Số lượng phải lớn hơn 0");
            if (salePrice <= 0)
                return Json("Giá sản phẩm phải lớn hơn 0");

            bool result = OrderDataService.SaveOrderDetail(orderID, productID, Quantity, salePrice);
            if (!result)
                return Json("Không thể cập nhật chi tiết đơn hàng này");


            return Json("");
        }
    }
}
