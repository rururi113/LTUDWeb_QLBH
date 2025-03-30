using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Shop.Models;
using System.Globalization;

namespace SV21T1020218.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        /// <summary>
        /// Hiển thị danh sách đơn hàng của khách hàng
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                // Kiểm tra đăng nhập
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Debug: In ra customerId
                System.Diagnostics.Debug.WriteLine($"Customer ID: {customerId}");

                // Lấy tất cả đơn hàng
                var allOrders = OrderDataService.ListOrders(
                    out int rowCount,
                    page: 1,
                    pageSize: 0,
                    status: 0,
                    fromTime: null,
                    toTime: null,
                    searchValue: ""
                );

                // Lọc theo customerId
                var customerOrders = allOrders
                    .Where(o => o.CustomerID == customerId)
                    .OrderByDescending(o => o.OrderTime)
                    .ToList();

                // In ra số lượng đơn hàng
                System.Diagnostics.Debug.WriteLine($"Found {customerOrders.Count} orders for customer {customerId}");

                // Lấy chi tiết cho từng đơn hàng
                foreach (var order in customerOrders)
                {
                    order.Details = OrderDataService.ListOrderDetails(order.OrderID);
                    // In ra thông tin chi tiết đơn hàng
                    System.Diagnostics.Debug.WriteLine($"Order {order.OrderID} has {order.Details.Count} items");
                }

                return View(customerOrders);
            }
            catch (Exception ex)
            {
                //In ra lỗi nếu có
                System.Diagnostics.Debug.WriteLine($"Error in Index: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return RedirectToAction("Index", "Home");
            }
        }

        private const string SHOPPING_CART = "ShoppingCart";
        private const int WEB_ORDER_EMPLOYEE_ID = 1; // Thay bằng ID của nhân viên mặc định vừa thêm

        /// <summary>
        /// Hiển thị giỏ hàng
        /// </summary>
        public IActionResult ShoppingCart()
        {
            var cart = GetShoppingCart();
            return View(cart);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost]
        public IActionResult AddToCart([FromForm] CartItem item)
        {
            try
            {
                if (item.SalePrice <= 0)
                    return Json("Giá bán không hợp lệ");
                
                if (item.Quantity <= 0)
                    return Json("Số lượng phải lớn hơn 0");

                var cart = GetShoppingCart();
                var existItem = cart.FirstOrDefault(p => p.ProductID == item.ProductID);
                if (existItem == null)
                {
                    cart.Add(item);
                }
                else
                {
                    existItem.Quantity += item.Quantity;
                }

                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
                return Json("");
            }
            catch (Exception ex)
            {
                return Json($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng
        /// </summary>
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                return Json("Số lượng phải lớn hơn 0");

            var cart = GetShoppingCart();
            var item = cart.FirstOrDefault(p => p.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return Json("");
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                cart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return Json("");
        }

        /// <summary>
        /// Xóa toàn bộ giỏ hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, new List<CartItem>());
            return Json("");
        }

        /// <summary>
        /// Khởi tạo đơn hàng
        /// </summary>
        [HttpPost]
        public IActionResult Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = GetCart();
            if (!cart.Any())
                return Json("Giỏ hàng trống!");

            // Chuyển dữ liệu từ CartItem sang OrderDetail
            var orderDetails = cart.Select(item => new OrderDetail()
            {
                ProductID = item.ProductID,
                ProductName = item.ProductName,
                Photo = item.Photo,
                Unit = item.Unit,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            }).ToList();

            // Khởi tạo đơn hàng
            int employeeID = 0;
            int orderID = OrderDataService.InitOrder(
                employeeID, 
                customerID,
                deliveryProvince,
                deliveryAddress,
                orderDetails
            );

            if (orderID > 0)
            {
                ClearCart();
                return Json("");
            }

            return Json("Không thể tạo đơn hàng!");
        }

        /// <summary>
        /// Lấy số lượng sản phẩm trong giỏ hàng
        /// </summary>
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(cart?.Count ?? 0);
        }

        /// <summary>
        /// Lấy tổng tiền giỏ hàng
        /// </summary>
        public IActionResult GetCartTotal()
        {
            var cart = GetCart();
            return Json(cart.Sum(m => m.TotalPrice));
        }

        /// <summary>
        /// Lấy giỏ hàng từ session
        /// </summary>
        private List<CartItem> GetCart()
        {
            return ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART) ?? new List<CartItem>();
        }

        /// <summary>
        /// Lưu giỏ hàng vào session
        /// </summary>
        private void SaveCart(List<CartItem> cart)
        {
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
        }

        /// <summary>
        /// Lấy thông tin giỏ hàng
        /// </summary>
        [HttpGet]
        public IActionResult GetCartInfo()
        {
            var cart = GetShoppingCart();
            return Json(new { 
                count = cart.Count,
                total = cart.Sum(m => m.TotalPrice)
            });
        }

        /// <summary>
        /// Lấy giỏ hàng từ session
        /// </summary>
        private List<CartItem> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }

        public IActionResult Checkout()
        {
            // Kiểm tra giỏ hàng trước
            var cart = GetShoppingCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ShoppingCart");
            }

            // Thử lấy thông tin user từ session trước
            var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
            if (userData != null)
            {
                // Nếu có userData trong session, lấy thông tin khách hàng
                if (!int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
                }

                var customer = CommonDataService.GetCustomer(customerId);
                if (customer != null)
                {
                    var model = new CheckoutViewModel
                    {
                        Cart = cart,
                        CustomerName = customer.CustomerName,
                        CustomerContactName = customer.ContactName,
                        CustomerAddress = customer.Address,
                        CustomerPhone = customer.Phone,
                        CustomerEmail = customer.Email,
                        DeliveryProvince = customer.Province,
                        DeliveryAddress = customer.Address
                    };

                    return View(model);
                }
            }

            // Nếu không có thông tin trong session, thử lấy từ UserAccountService
            var userAccount = UserAccountService.GetCurrentUser(User);
            if (userAccount == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Chuyển đổi UserId từ string sang int
            if (!int.TryParse(userAccount.UserId, out int userId))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Lấy thông tin chi tiết khách hàng từ database
            var customerInfo = CommonDataService.GetCustomer(userId);
            if (customerInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo model cho view checkout
            var checkoutModel = new CheckoutViewModel
            {
                Cart = cart,
                CustomerName = customerInfo.CustomerName,
                CustomerContactName = customerInfo.ContactName,
                CustomerAddress = customerInfo.Address,
                CustomerPhone = customerInfo.Phone,
                CustomerEmail = customerInfo.Email,
                DeliveryProvince = customerInfo.Province,
                DeliveryAddress = customerInfo.Address
            };

            return View(checkoutModel);
        }

        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Cart = GetShoppingCart();
                return View(model);
            }

            var cart = GetShoppingCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ShoppingCart");
            }

            try
            {
                // Lấy thông tin khách hàng từ session
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Chuyển đổi giỏ hàng thành chi tiết đơn hàng
                var orderDetails = cart.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    Photo = item.Photo,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });

                // Tạo đơn hàng mới
                var order = new Order
                {
                    CustomerID = customerId,
                    OrderTime = DateTime.Now,
                    DeliveryProvince = model.DeliveryProvince,
                    DeliveryAddress = model.DeliveryAddress,
                    Status = Constants.ORDER_INIT,
                    //EmployeeID = null  // Đặt là null vì đây là đơn hàng từ khách hàng
                };

                // Lưu đơn hàng vào database
                int orderId = OrderDataService.InitOrder(
                    employeeID: WEB_ORDER_EMPLOYEE_ID,  // Sử dụng ID nhân viên mặc định
                    customerID: customerId,
                    deliveryProvince: model.DeliveryProvince,
                    deliveryAddress: model.DeliveryAddress,
                    details: orderDetails
                );

                if (orderId > 0)
                {
                    // Xóa giỏ hàng
                    ApplicationContext.RemoveSessionData(SHOPPING_CART);

                    // Chuyển đến trang thành công
                    TempData["OrderId"] = orderId;
                    TempData["CustomerName"] = model.CustomerName;
                    return RedirectToAction("OrderSuccess");
                }

                ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng thử lại!");
                model.Cart = cart;
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                model.Cart = cart;
                return View(model);
            }
        }

        /// <summary>
        /// Hiển thị trang xác nhận đặt hàng thành công
        /// </summary>
        public IActionResult OrderSuccess()
        {
            // Kiểm tra có thông tin đơn hàng trong TempData không
            if (TempData["OrderId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.OrderId = TempData["OrderId"];
            ViewBag.CustomerName = TempData["CustomerName"];
            return View();
        }

        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        public IActionResult OrderDetails(int id)
        {
            try
            {
                // Kiểm tra xem đơn hàng có thuộc về khách hàng hiện tại không
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Lấy thông tin đơn hàng và chi tiết đơn hàng
                var order = OrderDataService.GetOrder(id);
                if (order == null || order.CustomerID != customerId)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Lấy danh sách chi tiết đơn hàng sử dụng ListDetails thay vì GetOrderDetails
                order.Details = OrderDataService.ListOrderDetails(id);

                return View(order);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}