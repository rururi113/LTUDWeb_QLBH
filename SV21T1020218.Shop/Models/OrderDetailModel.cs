﻿using SV21T1020218.DomainModels;

namespace SV21T1020218.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}