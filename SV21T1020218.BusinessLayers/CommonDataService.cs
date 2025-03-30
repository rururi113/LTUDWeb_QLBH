using DataLayers.SQLServer;
using SV21T1020218.DataLayers.SQLServer;
using SV21T1020218.DataLayers;
using SV21T1020218.DomainModels;



namespace SV21T1020218.BusinessLayers
{
    public class CommonDataService
    {
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Category> categoriesDB;
        private static readonly ICommonDAL<Supplier> suppliersDB;
        private static readonly ICommonDAL<Shipper> shippersDB;
        private static readonly ICommonDAL<Employee> employeesDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {

            string connectionString = Configuration.Connectionstring;

            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            categoriesDB = new CategoryDAL(connectionString);
            suppliersDB = new SupplierDAL(connectionString);
            shippersDB = new ShipperDAL(connectionString);
            employeesDB = new EmployeeDAL(connectionString);

        }
        public static List<Province> ListOfProvinces()
        {

            return provinceDB.List();
        }
        public static List<Customer> ListOfCustomers()
        {

            return customerDB.List();
        }
        /// <summary>
        /// lấy một khách hàng có mã là id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// bổ sung một khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return suppliersDB.Get(id);
        }

        public static Shipper? GetShipper(int id)
        {
            return shippersDB.Get(id);
        }

        public static Employee? GetEmployee(int id)
        {
            return employeesDB.Get(id);
        }

        public static Category? GetCategory(int id)
        {
            return categoriesDB.Get(id);
        }





        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return suppliersDB.Add(data);
        }

        public static int AddShipper(Shipper data)
        {
            return shippersDB.Add(data);
        }

        public static int AddEmployee(Employee data)
        {
            return employeesDB.Add(data);
        }
        public static int AddCategory(Category data)
        {
            return categoriesDB.Add(data);
        }






        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// xóa thông tin khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return suppliersDB.Update(data);
        }

        public static bool UpdateShipper(Shipper data)
        {
            return shippersDB.Update(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return employeesDB.Update(data);
        }
        public static bool UpdateCategory(Category data)
        {
            return categoriesDB.Update(data);
        }

        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }
        /// <summary>
        /// xóa thông tin khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            return suppliersDB.Delete(id);
        }

        public static bool DeleteShipper(int id)
        {
            return shippersDB.Delete(id);
        }

        public static bool DeleteEmployee(int id)
        {
            return employeesDB.Delete(id);
        }
        public static bool DeleteCategoty(int id)
        {
            return categoriesDB.Delete(id);
        }






        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        public static bool InUsedSupplier(int id)
        {
            return suppliersDB.InUsed(id);
        }

        public static bool InUsedShipper(int id)
        {
            return shippersDB.InUsed(id);
        }
        public static bool InUsedEmployee(int id)
        {
            return employeesDB.InUsed(id);
        }
        public static bool InUsedCategory(int id)
        {
            return categoriesDB.InUsed(id);
        }





        public static List<Customer> ListOfCustomer(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }


        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoriesDB.Count(searchValue);
            return categoriesDB.List(page, pageSize, searchValue);
        }
        public static List<Category> ListOfCategories(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return categoriesDB.List(page, pageSize, searchValue);
        }


        public static List<Supplier> ListOfSupplier(out int rowCount, int page = 1, int pageSize = 0, string seachValue = "")
        {
            rowCount = suppliersDB.Count(seachValue);
            return suppliersDB.List(page, pageSize, seachValue);
        }
        public static List<Supplier> ListOfSuppliers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return suppliersDB.List(page, pageSize, searchValue);
        }
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string seachValue = "")
        {
            rowCount = shippersDB.Count(seachValue);
            return shippersDB.List(page, pageSize, seachValue);
        }
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string seachValue = "")
        {
            rowCount = employeesDB.Count(seachValue);
            return employeesDB.List(page, pageSize, seachValue);
        }
    }
}