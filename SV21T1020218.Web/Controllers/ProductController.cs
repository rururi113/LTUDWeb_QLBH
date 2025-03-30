﻿using Microsoft.AspNetCore.Mvc;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020218.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSeachCondition";

        public IActionResult Index()
        {
            // Lấy điều kiện tìm kiếm từ session
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);

            // Nếu không có điều kiện tìm kiếm, khởi tạo một đối tượng mới
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = 30,
                    SearchValue = ""

                };
            }

            var categories = CommonDataService.ListOfCategories(out _, 1, 0, "");
            var suppliers = CommonDataService.ListOfSupplier(out _, 1, 0, "");

            condition.Categories = categories;
            condition.Suppliers = suppliers;
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {   //Định dạng giá tiền
            int rowCount;
            int minPrice = Int32.Parse(Regex.Replace(condition.MinPrice, "[,.]", ""));
            int maxPrice = Int32.Parse(Regex.Replace(condition.MaxPrice, "[,.]", ""));
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, minPrice, maxPrice);

            ProductSearchResult model = new ProductSearchResult()
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
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Mặt hàng";
            Product data = new() { ProductID = 0, IsSelling = true };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Mặt hàng";
            Product? data = ProductDataService.GetProduct(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);

        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

            // TODO: Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Vui lòng nhập tên sản phẩm");

            if (string.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Vui lòng nhập mô tả của sản phẩm");


            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính của sản phẩm");

            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Giá của sản phẩm phải lớn hơn 0");

            if (!ModelState.IsValid)
                return View("Edit", data);

            // Xử lý
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                return RedirectToAction("Index", new { id });
            }
            else
            {
                ProductDataService.UpdateProduct(data);
                return RedirectToAction("Index", new { id = data.ProductID });
            }


        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto data = new() { ProductID = id, PhotoID = 0, DisplayOrder = 1, IsHidden = false };
                    return View(data);

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    ProductPhoto? productPhoto = ProductDataService.GetPhoto(photoId);
                    if (productPhoto == null)
                    {
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(productPhoto);

                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        //{
        //    switch (method)
        //    {
        //        case "save":
        //            return Ok();
        //        default: return BadRequest();
        //    }

        //    return Ok();
        //}

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    ProductAttribute addProductAttribute = new() { AttributeID = attributeId, ProductID = id, DisplayOrder = 1 };
                    return View(addProductAttribute);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    ProductAttribute? editProductAttribute = ProductDataService.GetAttribute(attributeId);
                    if (editProductAttribute == null)
                    {
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(editProductAttribute);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto productPhoto, IFormFile uploadPhoto)
        {
            ViewBag.Title = productPhoto.PhotoID == 0 ? "Bổ sung ảnh của mặt hàng" : "Thay đổi ảnh của mặt hàng";

            // Validation
            if (uploadPhoto == null && productPhoto.PhotoID == 0)
                ModelState.AddModelError(nameof(productPhoto.Photo), "Vui lòng chọn ảnh của mặt hàng");

            if (string.IsNullOrWhiteSpace(productPhoto.Description))
                ModelState.AddModelError(nameof(productPhoto.Description), "Vui lòng nhập mô tả của sản phẩm");

            if (productPhoto.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(productPhoto.DisplayOrder), "Vui lòng nhập thứ tự hiển thị lớn hơn 0");

            if (productPhoto.PhotoID == 0
                    && ProductDataService.ExistsDisplayOrderPhoto(
                            productPhoto.ProductID,
                            productPhoto.DisplayOrder,
                            productPhoto.PhotoID
                        ))
                ModelState.AddModelError(nameof(productPhoto.DisplayOrder), "Vui lòng nhập thứ tự hiện thị không trùng");

            if (!ModelState.IsValid)
                return View("Photo", productPhoto);

            // Xử lý upload ảnh

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto?.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                productPhoto.Photo = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto?.CopyTo(stream);
                }
            }
            else
            {
                ProductPhoto? updatingPhoto = ProductDataService.GetPhoto(productPhoto.PhotoID);
                productPhoto.Photo = updatingPhoto.Photo;
            }


            if (productPhoto.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(productPhoto);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(productPhoto.Photo), "Vui lòng chọn lại ảnh của mặt hàng");
                    return View("Photo", productPhoto);
                }


            }
            else
            {
                long duplicatePhotoID = ProductDataService.GetConflictingPhotoID(
                    productPhoto.ProductID,
                    productPhoto.DisplayOrder,
                    productPhoto.PhotoID
                );
                if (duplicatePhotoID > 0)
                {
                    // Lấy ra dữ liệu ảnh đang cập nhật trong CSDL
                    ProductPhoto? updatingPhoto = ProductDataService.GetPhoto(productPhoto.PhotoID);

                    // Lấy ra dữ liệu ảnh bị trùng
                    ProductPhoto? duplicatePhoto = ProductDataService.GetPhoto(duplicatePhotoID);

                    // Gán VTHT của ảnh bị trùng bằng VTHT ảnh đang cập nhật
                    duplicatePhoto.DisplayOrder = updatingPhoto.DisplayOrder;

                    // Cập nhật dữ liệu ảnh bị trùng
                    ProductDataService.UpdatePhoto(duplicatePhoto);

                    ProductDataService.HiddenPhoto(productPhoto.PhotoID, productPhoto.IsHidden);
                }

                ProductDataService.UpdatePhoto(productPhoto);

                ProductDataService.HiddenPhoto(productPhoto.PhotoID, productPhoto.IsHidden);

            }

            return RedirectToAction("Edit", new { id = productPhoto.ProductID });
        }

        public IActionResult SaveAttribute(ProductAttribute productAttribute)
        {
            ViewBag.Title = productAttribute.AttributeID == 0 ? "Bổ sung thuộc tính của mặt hàng" : "Thay đổi thuộc tính của mặt hàng";

            if (string.IsNullOrWhiteSpace(productAttribute.AttributeName))
                ModelState.AddModelError(nameof(productAttribute.AttributeName), "Tên thuộc tính không được để trống");

            if (string.IsNullOrWhiteSpace(productAttribute.AttributeValue))
                ModelState.AddModelError(nameof(productAttribute.AttributeValue), "Giá trị thuộc tính không được để trống");

            if (productAttribute.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(productAttribute.DisplayOrder), "Vui lòng nhập thứ tự hiển thị của thuộc tính phải lớn hơn 0");

            if (productAttribute.AttributeID == 0
                    && ProductDataService.GetConflictingAttributeID(
                        productAttribute.ProductID,
                        productAttribute.DisplayOrder,
                        productAttribute.AttributeID) > 0
                )
                ModelState.AddModelError(nameof(productAttribute.DisplayOrder), "Vui lòng nhập thứ tự hiện thị không trùng");

            if (ProductDataService.ExistsAttribute(productAttribute.ProductID, productAttribute.AttributeName, productAttribute.AttributeValue, productAttribute.AttributeID))
                ModelState.AddModelError(nameof(productAttribute.AttributeValue), "Vui lòng nhập giá trị thuộc tính không trùng");

            if (ModelState.IsValid == false)
                return View("Attribute", productAttribute);

            if (productAttribute.AttributeID == 0)
            {
                long AttributeID = ProductDataService.AddAttribute(productAttribute);
                if (AttributeID <= 0)
                {
                    return View("Attribute", productAttribute);
                }
            }
            else
            {
                long duplicateAttributeID = ProductDataService.GetConflictingAttributeID(
                    productAttribute.ProductID,
                    productAttribute.DisplayOrder,
                    productAttribute.AttributeID
                );
                if (duplicateAttributeID > 0)
                {
                    // Lấy ra dữ liệu thuộc tính đang cập nhật trong CSDL
                    ProductAttribute? updatingAttribute = ProductDataService.GetAttribute(productAttribute.AttributeID);

                    // Lấy ra thuộc tính bị trùng
                    ProductAttribute? duplicateAttribute = ProductDataService.GetAttribute(duplicateAttributeID);

                    // Gán VTHT của thuộc tính bị trùng bằng thuộc tính đang cập nhật
                    duplicateAttribute.DisplayOrder = updatingAttribute.DisplayOrder;

                    // Cập nhật thuộc tính bị trùng
                    ProductDataService.UpdateAttribute(duplicateAttribute);
                }
                ProductDataService.UpdateAttribute(productAttribute);
            }

            return RedirectToAction("Edit", new { id = productAttribute.ProductID });
        }
    }
}