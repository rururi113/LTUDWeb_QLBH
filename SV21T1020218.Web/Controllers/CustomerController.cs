using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020218.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class CustomerController : Controller
    {
        public const int PAGE_SIZE = 30;
        public const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfCustomer(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";

            //Kiểm tra dữ liệu đầu vào, nếu không hợp lệ thì tạo ra thông báo lỗi và lưu trữ trong ModelState sử dụng lệnh: 
            // ModelState.AddModelError(key,message
            //      -key:  Chuỗi tên lỗi/mã lỗi 
            //      -message: Thông báo lỗi mà ta muốn chuyển đến người sd trên View

            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Bạn chưa chọn tỉnh/thành khách hàng");

            // Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (!ModelState.IsValid)
            {
                return View("Edit", data); // Trả dữ liệu về cho View, kèm theo các thông báo lỗi 
            }


            if (data.CustomerID == 0)
            {
                int id = CommonDataService.AddCustomer(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCustomer(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
    }
}
