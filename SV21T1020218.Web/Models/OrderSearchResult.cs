using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;

namespace SV21T1020218.Web.Models
{
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}
