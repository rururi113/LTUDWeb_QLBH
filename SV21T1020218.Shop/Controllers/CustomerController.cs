using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;

namespace SV21T1020218.Shop.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        public IActionResult EditProfile()
        {
            try
            {
                // Lấy thông tin user từ session
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var customer = CommonDataService.GetCustomer(customerId);
                if (customer == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Lấy danh sách tỉnh thành từ CommonDataService có sẵn
                ViewBag.Provinces = CommonDataService.ListOfProvinces();
                return View(customer);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Provinces = CommonDataService.ListOfProvinces();
                    return View("EditProfile", data);
                }

                // Lấy thông tin user từ session
                var userData = ApplicationContext.GetSessionData<WebUserData>("UserData");
                if (userData == null || !int.TryParse(userData.UserId, out int customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Đảm bảo CustomerID khớp với user đang đăng nhập
                data.CustomerID = customerId;

                // Cập nhật thông tin khách hàng
                if (CommonDataService.UpdateCustomer(data))
                {
                    TempData["Message"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("EditProfile");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại!");
                    ViewBag.Provinces = CommonDataService.ListOfProvinces();
                    return View("EditProfile", data);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                ViewBag.Provinces = CommonDataService.ListOfProvinces();
                return View("EditProfile", data);
            }
        }
    }
}



