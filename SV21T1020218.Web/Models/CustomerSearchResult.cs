using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;

namespace SV21T1020218.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }

    }
}
