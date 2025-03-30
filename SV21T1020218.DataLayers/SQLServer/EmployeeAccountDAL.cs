using Dapper;
using SV21T1020218.DomainModels;
using System.Data;

namespace SV21T1020218.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = Openconnection())
            {
                var sql = @"select EmployeeID as UserId,
									Email as UserName,
									FullName as DisplayName,
									Photo,
									RoleNames
							 from	Employees
							 where	Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string newpassword)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"UPDATE Employees
                    SET Password = @NewPassword
                    WHERE Email = @UserName";
                var parameters = new
                {
                    UserName = username,
                    NewPassword = newpassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;

        }

        public UserAccount? GetUserAccountByUsername(string username)
        {
            UserAccount? data = null;
            using (var connection = Openconnection())
            {
                var sql = @"select EmployeeID as UserId,
                              Email as UserName,
                              FullName as DisplayName,
                              Photo,
                              RoleNames
                       from   Employees
                       where  Email = @Email";
                var parameters = new { Email = username };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool Register(string email, string password, string customerName, string address, string phone)
        {
            throw new NotImplementedException("Không thể đăng ký tài khoản nhân viên từ website");
        }

        public bool IsEmailExists(string email)
        {
            using (var connection = Openconnection())
            {
                var sql = "SELECT COUNT(*) FROM Employees WHERE Email = @Email";
                var parameters = new { Email = email };
                int count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
                return count > 0;
            }
        }
    }
}