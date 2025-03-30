using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;

namespace SV21T1020218.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
