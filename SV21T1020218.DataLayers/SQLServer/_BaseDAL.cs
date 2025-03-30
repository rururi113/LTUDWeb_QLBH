using Microsoft.Data.SqlClient;

namespace SV21T1020218.DataLayers.SQLServer
{
    public abstract class BaseDAL
    {
        protected string _connectionString = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">Chuỗi tham số kết nối dến cơ sơ dữ liệu</param>
        public BaseDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và kết nối đến CSDL
        /// </summary>
        /// <returns></returns>
        protected SqlConnection Openconnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
