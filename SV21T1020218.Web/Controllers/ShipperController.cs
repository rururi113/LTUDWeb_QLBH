using Microsoft.AspNetCore.Mvc;
using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;
using SV21T1020218.BusinessLayers;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020218.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 5;
        public const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0,

            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";

            //Kiểm tra dữ liệu đầu vào, nếu không hợp lệ thì tạo ra thông báo lỗi và lưu trữ trong ModelState sử dụng lệnh: 
            // ModelState.AddModelError(key,message
            //      -key:  Chuỗi tên lỗi/mã lỗi 
            //      -message: Thông báo lỗi mà ta muốn chuyển đến người sd trên View

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại người giao hàng không được để trống");

            // Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (!ModelState.IsValid)
            {
                return View("Edit", data); // Trả dữ liệu về cho View, kèm theo các thông báo lỗi 
            }


            if (data.ShipperID == 0)
            {
                int id = CommonDataService.AddShipper(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateShipper(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại  bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}