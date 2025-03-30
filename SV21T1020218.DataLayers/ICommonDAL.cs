namespace SV21T1020218.DataLayers
{
    //Định nghĩa các phép xữ lý dữ liệu "Chung chung"
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy dách sách dữ liệu dưới dạng phân trang (pagination)
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dần hiển thị trên mỗi trang (Bằng 0 tức là không phân trang)</param>
        /// <param name="searchValue">Giá trị cần tìm kiếm (Chuỗi rỗng tức là lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 10,String searchValue =  "");
        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm được
        /// </summary>
        /// <param name="searchValue">Giá trị cần tìm ( chuỗi rỗng nếu đếm toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(String searchValue = "");
        /// <summary>
        /// Lấy một dòng dữ liệu vào khóa chính/id (trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Bổ sung một bản ghi vào CSDL. Hàm trả về ID của dữ liệu vừa được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cập nhập dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xóa một dòng dữ liệu vào giá trị của khóa chính/id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem một dòng dữ liệu có khóa là id hiện dữ liệu tham chiếu hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
