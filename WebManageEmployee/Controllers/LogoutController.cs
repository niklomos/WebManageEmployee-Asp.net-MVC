using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WebManageEmployee.Controllers
{

    public class LogoutController : Controller
    {
        private readonly ILogger<LogoutController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public LogoutController(ILogger<LogoutController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }
        // Method สำหรับ logout
        public IActionResult Logout()
        {
            try
            {
                // ลบ session ทั้งหมด
                _contextAccessor.HttpContext.Session.Clear();

                Response.Cookies.Delete("RememberMeUsername");
                Response.Cookies.Delete("RememberMePassword");
                Response.Cookies.Delete("EmpIdCookie");
                Response.Cookies.Delete("EmpNameCookie");

                // Redirect กลับไปยังหน้า login
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                // กรณีเกิดข้อผิดพลาด
                ViewBag.Error = "Error : " + ex.Message;
                return View("Error"); // สามารถเลือกที่จะส่งไปยังหน้า error หรือหน้าอื่นที่เหมาะสม
            }
        }

    }

}
