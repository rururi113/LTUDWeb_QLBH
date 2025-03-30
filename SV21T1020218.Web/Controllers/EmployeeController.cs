using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;
using SV21T1020218.Web;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020218.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN}")]
    public class EmployeeController : Controller
    {
        public const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";


        public IActionResult Index()
        {
            // Lấy điều kiện tìm kiếm từ session
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);

            // Nếu không có điều kiện tìm kiếm, khởi tạo một đối tượng mới
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 12,
                    SearchValue = ""
                };
            }

            return View(condition);
        }


        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");

            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            if (id == 0)
            {

                ViewBag.Title = "Bổ sung khách hàng";
                var newEmployee = new Employee()
                {
                    EmployeeID = 0
                };
                return View("Edit", newEmployee);
            }
            else
            {

                ViewBag.Title = "Cập nhật thông tin khách hàng";
                var data = CommonDataService.GetEmployee(id);
                if (data == null)
                {
                    return RedirectToAction("Index");
                }
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult Save(Employee data, string _birthDate, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            //Kiểm tra dữ liệu đầu vào, nếu không hợp lệ thì tạo ra thông báo lỗi và lưu trữ trong ModelState sử dụng lệnh: 
            // ModelState.AddModelError(key,message
            //      -key:  Chuỗi tên lỗi/mã lỗi 
            //      -message: Thông báo lỗi mà ta muốn chuyển đến người sd trên View

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(_birthDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh của nhân viên");

            //Xu ly ngay sinh 
            DateTime? d = _birthDate.ToDateTime();
            if (d != null)
                data.BirthDate = d.Value;
            else
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.Phone))
                data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Address))
                data.Address = "";

            if (!ModelState.IsValid)
            {
                return View("Edit", data); // Trả dữ liệu về cho View, kèm theo các thông báo lỗi 
            }
            //xu ly anh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
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
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}