using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;

namespace SV21T1020218.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập tên tài khoản và mật khẩu.");
                return View();
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Tên tài khoản hoặc mật khẩu không đúng.");
                return View();
            }

            // Tạo WebUserData từ thông tin tài khoản
            var userData = new WebUserData
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                Displayname = userAccount.Displayname
            };

            // Tạo principal từ WebUserData
            var principal = userData.CreatePrincipal();

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(360)
            };

            // Đăng nhập với principal đã tạo
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            // Lưu thông tin vào session
            ApplicationContext.SetSessionData("UserData", userData);

            // Chuyển hướng về trang yêu cầu ban đầu nếu có
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string username, string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin.");
                    return View();
                }

                // Kiểm tra mật khẩu mới có giống mật khẩu cũ không
                if (oldPassword == newPassword)
                {
                    ModelState.AddModelError("Error", "Mật khẩu mới không được giống mật khẩu cũ.");
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("Error", "Mật khẩu mới không khớp.");
                    return View();
                }

                var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, oldPassword);
                if (userAccount == null)
                {
                    ModelState.AddModelError("Error", "Mật khẩu hiện tại không chính xác.");
                    return View();
                }

                bool result = UserAccountService.ChangedPassword(username, newPassword);
                if (result)
                {
                    TempData["ChangedPassword"] = "Đổi mật khẩu thành công!";
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Error", "Đổi mật khẩu thất bại.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Có lỗi xảy ra: " + ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string email, string password, string confirmPassword, 
            string customerName, string address, string phone)
        {
            try
            {
                // Kiểm tra định dạng email
                if (!email.Contains("@"))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập đúng định dạng email.");
                    return View();
                }

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập email.");
                    return View();
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập mật khẩu.");
                    return View();
                }
                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ModelState.AddModelError("Error", "Vui lòng xác nhận mật khẩu.");
                    return View();
                }
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập họ và tên.");
                    return View();
                }

                // Kiểm tra độ dài mật khẩu
                if (password.Length < 6)
                {
                    ModelState.AddModelError("Error", "Mật khẩu phải có ít nhất 6 ký tự.");
                    return View();
                }

                // Kiểm tra password và confirmPassword
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("Error", "Xác nhận mật khẩu không khớp.");
                    return View();
                }

                // Thực hiện đăng ký
                bool result = UserAccountService.Register(email, password, customerName, address, phone);
                if (result)
                {
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Error", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", $"Đã xảy ra lỗi: {ex.Message}");
                return View();
            }
        }
    }
}
