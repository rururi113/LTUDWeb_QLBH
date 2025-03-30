using SV21T1020218.DomainModels;

namespace SV21T1020218.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Kiểm tra xem tên đăng nhập và mật khẩu có đúng hay k?
        /// Nếu đúng : trả về thông tin của User, nếu sai trả về null
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        // Thêm phương thức này để lấy thông tin tài khoản người dùng theo tên đăng nhập
        UserAccount? GetUserAccountByUsername(string username);
        bool ChangePassword(string username, string newpassword);

        // Thêm phương thức đăng ký
        bool Register(string email, string password, string customerName, string address, string phone);
        
        // Kiểm tra email đã tồn tại chưa
        bool IsEmailExists(string email);
    }
}
