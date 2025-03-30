using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;


namespace SV21T1020218.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên đầy đủ và mật khẩu");
                return View();

            }
            //TODO: Kiểm tra xem username và password (của Employee) có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            //ĐĂNG NHẬP THÀNH CÔNG
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                Displayname = userAccount.Displayname,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //2. Ghi nhận trạng thái đăng nhập
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3 Quay vể trang chủ
            return RedirectToAction("Index", "Home");


        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassWord()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string UserName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới không khớp");
                return View();
            }
            if (ModelState.IsValid == false)// !ModelState.IsValid
            {
                return View();
            }

            try
            {
                var userAccount = UserAccountService.Authorize(UserTypes.Employee, UserName, oldPassword);
                if (userAccount == null)
                {
                    ModelState.AddModelError("Error", "Mật khẩu cũ không chính xác");
                    return View();
                }
                else
                {
                    bool result = UserAccountService.ChangedPassword(UserName, newPassword);
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
            }
            catch (Exception)
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View();
            }
        }


        public IActionResult ForgotPassWord()
        {
            return View();
        }
        public IActionResult AccessDenined()
        {
            return View();
        }


    }
}