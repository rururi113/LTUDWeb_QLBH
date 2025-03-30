using Dapper;
using SV21T1020218.DomainModels;
using System.Data;
using System.Data.SqlClient;

namespace SV21T1020218.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = Openconnection())
            {
                var sql = @" select CustomerID as UserId,
                                    Email as UserName,
                                    CustomerName as DisplayName,
                                    N'' as Photo,
                                    N'' as RoleNames
                             from   Customers
                             where  Email = @Email and Password = @Password";
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

        public bool ChangePassword(string username, string password)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"UPDATE Customers 
                            SET Password = @Password 
                            WHERE Email = @Email";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public UserAccount? GetUserAccountByUsername(string username)
        {
            UserAccount? data = null;
            using (var connection = Openconnection())
            {
                var sql = @" select CustomerID as UserId,
                                    Email as UserName,
                                    CustomerName as DisplayName,
                                    N'' as Photo,
                                    N'' as RoleNames
                             from   Customers
                             where  Email = @Email";
                var parameters = new { Email = username };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool Register(string email, string password, string customerName, string address, string phone)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"INSERT INTO Customers(CustomerName, ContactName, Email, Password, Address, Phone)
                           VALUES(@CustomerName, @CustomerName, @Email, @Password, @Address, @Phone)";
                var parameters = new
                {
                    Email = email,
                    Password = password,
                    CustomerName = customerName,
                    Address = address ?? "",
                    Phone = phone ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool IsEmailExists(string email)
        {
            using (var connection = Openconnection())
            {
                var sql = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
                var parameters = new { Email = email };
                int count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
                return count > 0;
            }
        }
    }
}
