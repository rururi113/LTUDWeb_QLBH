﻿
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Numerics;
using System.Data;
using DataLayers.SQLServer;
using DataLayers;
using SV21T1020218.DataLayers.SQLServer;
using SV21T1020218.DataLayers;
using SV21T1020218.DomainModels;
namespace DataLayers.SQLServer
{
    //cai dat cac phep xu ly du lieu lien quan den bang customer(khach hang)
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = Openconnection())
            {
                var sql = @" if exists(select * from Customers where Email = @Email)
                                select -1;
                              else
                                begin 
                        insert into Customers(CustomerName, ContactName, Province, Address,Phone,Email, IsLocked)
                        values (@CustomerName, @ContactName, @Province, @Address,@Phone,@Email, @IsLocked);

                        select scope_identity();
                        end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                //thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = Openconnection())
            {
                var sql = @"select count(*)
                            from Customers
                             where (CustomerName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"delete from Customers where CustomerID=@CustomerID";
                var parameters = new
                {
                    CustomerID = id,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = Openconnection())
            {
                var sql = @"select * from Customers where CustomerId=@CustomerID";
                var parameters = new
                {
                    CustomerId = id,
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"if exists (select * from Orders where CustomerID=@CustomerID)
	                            select 1
                            else
	                            select 0";
                var parameters = new
                {
                    CustomerID = id,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var connection = Openconnection())
            {
                var sql = @"select *
                            from(
                                select *, 
                                         row_number() over(order by CustomerName) as RowNumber
                                from Customers
                                where (CustomerName like @searchValue) or (ContactName like @searchValue)
                                    ) as t
                              where (@pageSize = 0)
                                      or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                              order by RowNumber;";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue//ben trai la ten tham so trong cau lenh sql, ben phai la value truyen cho tham so
                };
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = Openconnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerId <> @CustomerId and Email = @Email)
                         begin       
                    update  Customers
                    set         CustomerName=@CustomerName,
	                            ContactName=@ContactName,
	                            Province=@Province,
	                            Address=@Address,
	                            Phone=@Phone,
	                            Email=@Email,
	                            IsLocked=@IsLocked
                    where       CustomerID = @CustomerID
                    end";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}