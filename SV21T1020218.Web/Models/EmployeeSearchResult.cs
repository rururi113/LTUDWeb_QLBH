using SV21T1020218.DomainModels;
using SV21T1020218.Web.Models;

namespace SV21T1020218.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
