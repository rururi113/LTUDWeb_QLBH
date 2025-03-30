using System.Security.Claims;
using SV21T1020218.DataLayers;
using SV21T1020218.DomainModels;

namespace SV21T1020218.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            string connectionString = "Server=DESKTOP-3JV0MGT;user id=sa;Password=123;database=PL;TrustServerCertificate=true";

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }

        public static bool ChangedPassword(string username, string newpassword)
        {
            var customerAccount = customerAccountDB.GetUserAccountByUsername(username);
            if (customerAccount != null)
            {
                return customerAccountDB.ChangePassword(username, newpassword);
            }
            
            var employeeAccount = employeeAccountDB.GetUserAccountByUsername(username);
            if (employeeAccount != null)
            {
                return employeeAccountDB.ChangePassword(username, newpassword);
            }

            return false;
        }

        public static UserAccount? GetCurrentUser(ClaimsPrincipal userPrincipal)
        {
            var username = userPrincipal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            //lấy thông tin người dùng từ cơ sở dữ liệu.
            // Ví dụ:
            return customerAccountDB.GetUserAccountByUsername(username);
        }

        public static bool Register(string email, string password, string customerName, string address, string phone)
        {
            if (customerAccountDB.IsEmailExists(email))
                return false;

            return customerAccountDB.Register(email, password, customerName, address, phone);
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}
