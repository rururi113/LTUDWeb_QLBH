using System.Security.Claims;

namespace SV21T1020218.Shop
{
    public static class WebUserExtension
    {
        /// <summary>
        /// Đọc thông tin của người dùng được "ghi" trong giấy chứng nhận (Principal)
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;

                var userData = new WebUserData();

                userData.UserId = principal.FindFirstValue(nameof(userData.UserId)) ?? "";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.Displayname = principal.FindFirstValue(nameof(userData.Displayname)) ?? "";                

                userData.Roles = new List<string>();
                foreach (var role in principal.FindAll(ClaimTypes.Role))
                    userData.Roles.Add(role.Value);
                return userData;
            }
            catch 
            {
                return null;
            }
        }
    }
}
