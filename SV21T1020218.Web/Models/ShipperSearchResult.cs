using SV21T1020218.DomainModels;

namespace SV21T1020218.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
