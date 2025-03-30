using System.ComponentModel.DataAnnotations;
using SV21T1020218.DomainModels;

namespace SV21T1020218.Shop.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public decimal TotalAmount 
        { 
            get 
            {
                return Cart.Sum(m => m.TotalPrice);
            } 
        }

        // Thông tin khách hàng
        public string CustomerName { get; set; } = "";
        public string CustomerContactName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string DeliveryProvince { get; set; } = "";

        // Nhập địa chỉ giao hàng
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string DeliveryAddress { get; set; } = "";


        public Order ToOrder()
        {
           var  order =  new Order
            {
                OrderTime = DateTime.Now,
                CustomerName = this.CustomerName,
                CustomerContactName = this.CustomerContactName,
                CustomerAddress = this.CustomerAddress,
                CustomerPhone = this.CustomerPhone,
                CustomerEmail = this.CustomerEmail,
                DeliveryProvince = this.DeliveryProvince,
                DeliveryAddress = this.DeliveryAddress,
                Status = Constants.ORDER_INIT,
                Details = Cart.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    Photo = item.Photo,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                }).ToList()
            };

            return order;
        }
    }
} 