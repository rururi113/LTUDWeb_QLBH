using SV21T1020218.DomainModels;

namespace SV21T1020218.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
