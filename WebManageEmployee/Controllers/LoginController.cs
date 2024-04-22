using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using WebManageEmployee.Models;

namespace WebManageEmployee.Controllers
{

    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }
        public IActionResult Login()
        {
            if (TempData.ContainsKey("SessionError"))
            {
                ViewData["SessionError"] = TempData["SessionError"].ToString();
            }
            return View();
        }


        [HttpPost]
        public IActionResult Login(Login login)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(_configuration.GetConnectionString("connectionStr")))
                {

                    con.Open();

                    string sqlquery = "SELECT  l.*,a.*,p.*,e.* FROM  tb_login AS l " +
                                      " JOIN tb_access AS a ON a.log_id = l.log_id "+
                                      " JOIN tb_program AS p ON a.pg_id = p.pg_id "+
                                      " LEFT JOIN tb_employee AS e ON e.emp_id = l.emp_id "+
                                     " WHERE  a.acc_status = '1' AND  l.Username = @Username AND l.Password = @Password AND a.pg_id = '2'  ";

                    var cmd = new MySqlCommand(sqlquery.ToString(), con);
                    cmd.Parameters.AddWithValue("@Username", login.Username);
                    cmd.Parameters.AddWithValue("@Password", login.Password);
                    cmd.Parameters.AddWithValue("@EmpId", login.EmpId);
                    cmd.Parameters.AddWithValue("@EmpName", login.EmpName);

                    var reader = cmd.ExecuteReader();

                    //if (reader.HasRows)
                    if (reader.Read())
                    {

                        var session = _contextAccessor.HttpContext.Session;
                        session.SetString("LoggedInFullName_Hr", reader["emp_name"].ToString());
                        session.SetString("LoggedInUsername_Hr", reader["username"].ToString());

                        //HttpContext.Response.Cookies.Append("LoggedInUsername_Hr", login.Username);
                        //HttpContext.Response.Cookies.Append("LoggedInFullName_Hr", reader["emp_name"].ToString());


                        if (login.EmpId != null)
                        {
                            session.SetString("LoggedInEmp_Id_Hr", reader["emp_id"].ToString());

                            //HttpContext.Response.Cookies.Append("LoggedInEmp_Id_Hr", Convert.ToInt32(reader["emp_id"]).ToString());

                            return RedirectToAction("Summary", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Summary", "Home");
                        }
                    }
                    else
                    {
                        ViewData["SweetAlert"] = "<script>Swal.fire('Invalid Login Attempt', 'Please check your username and password', 'error')</script>";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View();
            }
        }



       



    }
}
