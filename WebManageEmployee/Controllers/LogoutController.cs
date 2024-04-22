using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WebManageEmployee.Controllers
{

    public class LogoutController : Controller
    {
        public IActionResult Logout()
        {
            // Expire the cookie by setting its expiration date to a past date
            Response.Cookies.Delete("LoggedInUsername_Hr");
            Response.Cookies.Delete("LoggedInFullName_Hr");
            Response.Cookies.Delete("LoggedInEmp_Id_Hr");

            // Redirect to the login page or any other page as needed
            return RedirectToAction("Login", "Login");
        }

    }

}
