using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using System.Linq.Expressions;
using WebManageEmployee.Models;
using Microsoft.AspNetCore.Routing;
using System.IO; // เพิ่มไลบรารีสำหรับการจัดการไฟล์


namespace WebManageEmployee.Controllers
{


    [CheckSessionFilter]

    public class HomeController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;

        }


        public IActionResult TestImage()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Summary(string searchQuery, int page = 1)
        {

            
                try
                {

                    int totalItems = GetTotalItemCount(searchQuery); // ดึงจำนวนรายการทั้งหมดจากฐานข้อมูลหรือที่เก็บข้อมูล
                    int itemsPerPage = 10; // จำนวนรายการต่อหน้า
                    int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage); // คำนวณจำนวนหน้าทั้งหมด

                    // ตรวจสอบว่าหน้าปัจจุบันไม่เกินจำนวนหน้าทั้งหมด ถ้าเกินให้กำหนดหน้าปัจจุบันเป็นหน้าสุดท้าย
                    if (page > pageCount)
                    {
                        page = pageCount;
                    }

                    // ใช้ค่า LoggedInEmpId และหน้าปัจจุบันในการดึงข้อมูลจากฐานข้อมูล
                    var items = GetEmpFromDB(searchQuery, page, itemsPerPage);

                    ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                    // ส่งข้อมูลในหน้าปัจจุบันและจำนวนหน้าทั้งหมดไปยัง View
                    var viewModel = new EmployeeCount
                    {
                        Employees = items,
                        PageCount = pageCount,
                        CurrentPage = page,
                        ItemsPerPage = itemsPerPage,
                        TotalItems = totalItems
                    };

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    // จับ error และเพิ่มข้อความ error ลงใน ModelState
                    ModelState.AddModelError("", "เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message);
                    return View(); // หรือ return RedirectToAction("ActionName");
                }
            
        }


        public IActionResult InsertDepartment()
        {

            return View();
        }
        public IActionResult InsertPosition()
        {
            var departments = GetDepartmentFromDatabase();
            return View(departments);
        }
        public IActionResult InsertEmployee()
        {

            var positions = GetPositionsFromDatabase();
            var provinces = GetProvinceFromDatabase();
            var amphures = GetAmphureFromDatabase();
            var districts = GetDistrictFromDatabase();
            AddressModel addressModel = new AddressModel();
            addressModel.Positions = positions;
            addressModel.Provinces = provinces;
            addressModel.Amphures = amphures;
            addressModel.Districts = districts;
            return View(addressModel);

        }
        public IActionResult ManageDepartment(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCountByDepartment(searchQuery); // ดึงจำนวนรายการทั้งหมดจากฐานข้อมูลหรือที่เก็บข้อมูล
                int itemsPerPage = 10; // จำนวนรายการต่อหน้า
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage); // คำนวณจำนวนหน้าทั้งหมด

                // ตรวจสอบว่าหน้าปัจจุบันไม่เกินจำนวนหน้าทั้งหมด ถ้าเกินให้กำหนดหน้าปัจจุบันเป็นหน้าสุดท้าย
                if (page > pageCount)
                {
                    page = pageCount;
                }

                // ใช้ค่า LoggedInEmpId และหน้าปัจจุบันในการดึงข้อมูลจากฐานข้อมูล
                var items = GetDepartmentFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                // ส่งข้อมูลในหน้าปัจจุบันและจำนวนหน้าทั้งหมดไปยัง View
                var viewModel = new EmployeeCount
                {
                    Departments = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // จับ error และเพิ่มข้อความ error ลงใน ModelState
                ModelState.AddModelError("", "เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message);
                return View(); // หรือ return RedirectToAction("ActionName");
            }
        }
        public IActionResult ManagePosition(string searchQuery, int page = 1)
        {
            try
            {

                int totalItems = GetTotalItemCountByPosition(searchQuery); // ดึงจำนวนรายการทั้งหมดจากฐานข้อมูลหรือที่เก็บข้อมูล
                int itemsPerPage = 10; // จำนวนรายการต่อหน้า
                int pageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage); // คำนวณจำนวนหน้าทั้งหมด

                // ตรวจสอบว่าหน้าปัจจุบันไม่เกินจำนวนหน้าทั้งหมด ถ้าเกินให้กำหนดหน้าปัจจุบันเป็นหน้าสุดท้าย
                if (page > pageCount)
                {
                    page = pageCount;
                }

                // ใช้ค่า LoggedInEmpId และหน้าปัจจุบันในการดึงข้อมูลจากฐานข้อมูล
                var items = GetPositionFromDB(searchQuery, page, itemsPerPage);

                ViewBag.SearchQuery = System.Web.HttpUtility.HtmlDecode(searchQuery);

                // ส่งข้อมูลในหน้าปัจจุบันและจำนวนหน้าทั้งหมดไปยัง View
                var viewModel = new EmployeeCount
                {
                    Positions = items,
                    PageCount = pageCount,
                    CurrentPage = page,
                    ItemsPerPage = itemsPerPage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // จับ error และเพิ่มข้อความ error ลงใน ModelState
                ModelState.AddModelError("", "เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message);
                return View(); // หรือ return RedirectToAction("ActionName");
            }
        }
        public IActionResult UpdateDepartment(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var model = GetDepartmentByIdFromDB(id);
            model.CurrentPage = page;

            return View(model);
        }
        public IActionResult UpdatePosition(string searchquery, int id, int page = 1)
        {
            var position = GetPositionByIdFromDB(id);
            var departments = GetDepartmentFromDatabase();

            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var model = new DepartmentModel
            {
                CurrentPage = page,
                Position = position,
                Departments = departments,

            };

            return View(model);
        }
        public IActionResult UpdateEmployee(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var employee = GetEmployeeByIdFromDB(id);
            var positions = GetPositionsFromDatabase();
            var provinces = GetProvinceFromDatabase();
            var amphures = GetAmphureFromDatabaseById(id);
            var districts = GetDistrictFromDatabaseById(id);
            PositionModel positionModel = new PositionModel();
            positionModel.Positions = positions;
            positionModel.Employee = employee;
            positionModel.Provinces = provinces;
            positionModel.Amphures = amphures;
            positionModel.Districts = districts;
            positionModel.CurrentPage = page;
            return View(positionModel);

        }
        public IActionResult Profile(int id)
        {

            var employee = GetEmployeeByIdFromDB(id);
            var positions = GetPositionsFromDatabase();
            var provinces = GetProvinceFromDatabase();
            var amphures = GetAmphureFromDatabaseById(id);
            var districts = GetDistrictFromDatabaseById(id);
            var logins = GetLoginByIdFromDB(id);
            PositionModel positionModel = new PositionModel();
            positionModel.Positions = positions;
            positionModel.Employee = employee;
            positionModel.Provinces = provinces;
            positionModel.Amphures = amphures;
            positionModel.Districts = districts;
            positionModel.Logins = logins;
            return View(positionModel);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //End method about view



        //Start method about Getting
        private IEnumerable<Employee> GetEmpFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                int offset = (page - 1) * itemsPerPage;

                string sqlSelect = "SELECT e.*, p.* FROM tb_employee AS e  JOIN tb_position AS p ON e.pos_id = p.pos_id WHERE e.emp_status != '0' " +
                    "AND (e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery  OR  e.email LIKE @SearchQuery  OR  p.pos_name LIKE @SearchQuery ) " +
                    "ORDER BY e.emp_id DESC " +
                    $" LIMIT {itemsPerPage}  OFFSET {offset} ";

                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee employees = new Employee
                    {
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        EmpName = reader["emp_name"].ToString(),
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        PosName = reader["pos_name"].ToString(),
                        PosStatus = reader["pos_status"].ToString(),
                        EmpNumber = Convert.ToInt32(reader["emp_number"])

                    };



                    employee.Add(employees);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return employee;
        }
        private IEnumerable<Department> GetDepartmentFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Department> department = new List<Department>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;

                string sqlSelect = "SELECT * FROM tb_department WHERE dep_status != '0' " +
                    " AND  dep_name LIKE @SearchQuery " +
                     "ORDER BY dep_id DESC " +
                    $" LIMIT {itemsPerPage}  OFFSET {offset} ";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Department departments = new Department
                    {
                        DepId = Convert.ToInt32(reader["dep_id"]),
                        DepName = reader["dep_name"].ToString(),


                    };

                    department.Add(departments);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return department;
        }
        private IEnumerable<Position> GetPositionFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Position> position = new List<Position>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;
                string sqlSelect = "SELECT p.*,d.* FROM tb_position AS p JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE p.pos_status != '0'   " +
                               "AND (p.pos_name LIKE @SearchQuery OR d.dep_name LIKE @SearchQuery ) " +
                                " ORDER BY p.pos_id DESC " +
                                $" LIMIT {itemsPerPage}  OFFSET {offset} ";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Position positions = new Position
                    {
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString(),
                        DepName = reader["dep_name"].ToString(),
                        DepStatus = reader["dep_status"].ToString(),
                        PosPermissionsManage = reader["pos_permissions_manage"].ToString()

                    };

                    position.Add(positions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return position;
        }
        private List<Department> GetDepartmentFromDatabase()
        {
            List<Department> department = new List<Department>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT Dep_id, dep_name FROM tb_department";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Department departments = new Department
                            {
                                DepId = Convert.ToInt32(reader["dep_id"]),
                                DepName = reader["dep_name"].ToString(),


                            };

                            department.Add(departments);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                department = null;
            }

            return department;
        }
        private IActionResult GetDepartmentFromDatabaseForPatail()
        {
            List<Department> department = new List<Department>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT Dep_id, dep_name FROM tb_department";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Department departments = new Department
                            {
                                DepId = Convert.ToInt32(reader["dep_id"]),
                                DepName = reader["dep_name"].ToString(),


                            };

                            department.Add(departments);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                department = null;
            }

            return PartialView("SelectDepartment", department);
        }
        private List<Position> GetPositionsFromDatabase()
        {
            List<Position> position = new List<Position>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tb_position";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Position positions = new Position
                            {
                                PosId = Convert.ToInt32(reader["pos_id"]),
                                PosName = reader["pos_name"].ToString(),
                                DepId = Convert.ToInt32(reader["dep_id"])


                            };


                            position.Add(positions);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                position = null;
            }

            return position;
        }
        private List<Province> GetProvinceFromDatabase()
        {
            List<Province> province = new List<Province>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM provinces";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Province provinces = new Province
                            {
                                ProId = Convert.ToInt32(reader["id"]),
                                PName = reader["name_th_pro"].ToString(),


                            };


                            province.Add(provinces);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                province = null;
            }

            return province;
        }
        private List<Amphure> GetAmphureFromDatabase()
        {
            List<Amphure> amphure = new List<Amphure>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM amphures";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Amphure amphures = new Amphure
                            {
                                AmpId = Convert.ToInt32(reader["id"]),
                                AName = reader["name_th_amp"].ToString(),
                                ProId = Convert.ToInt32(reader["province_id"]),


                            };


                            amphure.Add(amphures);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                amphure = null;
            }

            return amphure;
        }
        private List<District> GetDistrictFromDatabase()
        {
            List<District> district = new List<District>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM districts";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            District districts = new District
                            {
                                DisId = Convert.ToInt32(reader["id"]),
                                DName = reader["name_th_dis"].ToString(),
                                ZipCode = Convert.ToInt32(reader["zip_code"]),
                                AmpId = Convert.ToInt32(reader["amphure_id"]),

                            };


                            district.Add(districts);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                district = null;
            }

            return district;
        }
        private IEnumerable<Employee> GetPositionsFromDatabaseJoin()
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT  p.pos_id, p.pos_name FROM tb_employee as e JOIN tb_position as p ON e.pos_id = p.pos_id";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee emp = new Employee
                    {

                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString()
                    };

                    // ?????????????????? Position ???????? tb_position ?????????????
                    emp.Position = new Position
                    {
                        PosId = emp.PosId,
                        PosName = emp.PosName
                    };


                    employee.Add(emp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return employee;
        }
        //End method about Getting



        //Start Method about Inserting
        [HttpPost]
        public IActionResult InsertDepartmentToDB(Department department)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO tb_department (dep_name,dep_status) 
                                VALUES (@DepName,@DepStatus )";
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@DepName", department.DepName);
                command.Parameters.AddWithValue("@DepStatus", department.DepStatus);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    var result = new { success = true, message = "Department adding successfully!" };
                    return Json(result);

                }
                catch (Exception ex)
                {
                    var result = new { success = false, message = "Error adding department: " + ex.Message };
                    return Json(result);

                }
            }
        }
        [HttpPost]
        public IActionResult InsertPositionToDB(Position position)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO tb_position (pos_name,pos_permissions_manage,pos_status,dep_id) 
                                VALUES (@PosName,@PosPermissionsManage,@PosStatus, @DepId )";

                if (string.IsNullOrWhiteSpace(position.PosPermissionsManage))
                {
                    position.PosPermissionsManage = "0";
                }
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@PosName", position.PosName);
                command.Parameters.AddWithValue("@DepId", position.DepId);
                command.Parameters.AddWithValue("@PosPermissionsManage", position.PosPermissionsManage);
                command.Parameters.AddWithValue("@PosStatus", position.PosStatus);


                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    var result = new { success = true, message = "Position adding successfully!" };
                    return Json(result);
                }
                catch (Exception ex)
                {
                    var result = new { success = false, message = "Error adding Position: " + ex.Message };
                    return Json(result);
                }
            }
        }
        [HttpPost]
        public IActionResult InsertEmployeeToDB(Employee employee)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO tb_employee (emp_name, date_of_birth, address, email, phone_number, salary, start_date, emp_status, create_by, last_update_by, pos_id, emp_number,province_id,amphure_id,district_id,gender) 
                         VALUES (@EmpName, @DateOfBirth, @Address, @Email, @PhoneNumber, @Salary, @StartDate, @EmpStatus, @CreateBy, @LastUpdateBy, @PosId, @EmpNumber,@ProId,@AmpId,@DisId,@Gender)";
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@EmpName", employee.EmpName);
                command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@Address", employee.Address);
                command.Parameters.AddWithValue("@Email", employee.Email);
                command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@StartDate", employee.StartDate);
                command.Parameters.AddWithValue("@EmpStatus", employee.EmpStatus);
                command.Parameters.AddWithValue("@CreateBy", employee.CreateBy);
                command.Parameters.AddWithValue("@LastUpdateBy", employee.LastUpdateBy);
                command.Parameters.AddWithValue("@PosId", employee.PosId);
                command.Parameters.AddWithValue("@ProId", employee.ProId);
                command.Parameters.AddWithValue("@AmpId", employee.AmpId);
                command.Parameters.AddWithValue("@DisId", employee.DisId);
                command.Parameters.AddWithValue("@Gender", employee.Gender);

                // Generate unique emp_number starting from 900001
                string empNumber = GenerateEmpNumber();
                command.Parameters.AddWithValue("@EmpNumber", empNumber);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    var result = new { success = true, message = "Employee added successfully!" };
                    return Json(result);
                }
                catch (Exception ex)
                {
                    var result = new { success = false, message = "Error adding Employee: " + ex.Message };
                    return Json(result);
                }
            }
        }
        private string GenerateEmpNumber()
        {
            // ดึงค่าล่าสุดของ emp_number จากฐานข้อมูล
            string connectionString = _configuration.GetConnectionString("connectionStr");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT MAX(CAST(SUBSTRING(emp_number, 2) AS UNSIGNED)) AS max_emp_number FROM tb_employee";
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    int maxEmpNumber = result == DBNull.Value ? 0 : Convert.ToInt32(result);

                    // เพิ่มขึ้นทีละ 1 และนำไปใช้ในรหัส emp_number ใหม่
                    int newEmpNumber = maxEmpNumber + 1;
                    return "9" + newEmpNumber.ToString("D5");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error generating emp_number: " + ex.Message);
                }
            }
        }
        //End Method about Inserting



        //Start method about Getting Condition
        private Department GetDepartmentByIdFromDB(int id)
        {
            Department department = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = $"SELECT * FROM tb_department WHERE dep_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    department = new Department
                    {
                        DepId = Convert.ToInt32(reader["dep_id"]),
                        DepName = reader["dep_name"].ToString()



                    };
                }

                return department;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department data from database.");
            }
            finally
            {
                connection.Close();
            }

            return department;
        }
        private Position GetPositionByIdFromDB(int id)
        {
            Position position = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = $"SELECT * FROM tb_position WHERE pos_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    position = new Position
                    {
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString(),
                        PosPermissionsManage = reader["pos_permissions_manage"].ToString(),

                        DepId = Convert.ToInt32(reader["dep_id"])


                    };
                }

                return position;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching position data from database.");
            }
            finally
            {
                connection.Close();
            }

            return position;
        }
        private Employee GetEmployeeByIdFromDB(int id)
        {
            Employee employee = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT * FROM tb_employee WHERE emp_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    employee = new Employee
                    {
                        EmpName = reader["emp_name"].ToString(),
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        DateOfBirth = (DateTime)reader["date_of_birth"],
                        Gender = reader["gender"].ToString(),
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Salary = Convert.ToDecimal(reader["salary"].ToString()),
                        StartDate = (DateTime)reader["start_date"],
                        EndDate = Convert.ToDateTime(reader["end_date"]),
                        CreateBy = reader["create_by"].ToString(),
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        LastUpdateBy = reader["last_update_by"].ToString(),
                        LastUpdateDate = Convert.ToDateTime(reader["last_update_date"]),
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        ProId = Convert.ToInt32(reader["province_id"]),
                        AmpId = Convert.ToInt32(reader["amphure_id"]),
                        DisId = Convert.ToInt32(reader["district_id"]),

                    };

                }


                if (employee.CreateBy == "0" && employee.LastUpdateBy == "0")
                {


                    employee.LastByUsername = "Admin";

                    employee.CreateByUsername = "Admin";

                }
                else if (employee.CreateBy == "0")
                {
                    employee.CreateByUsername = "Admin";

                    employee.LastByUsername = GetEmployeeByLastUpDateFromDB(employee.LastUpdateBy);


                }
                else if (employee.LastUpdateBy == "0")
                {
                    LoginCreateBy employeecreateby = GetEmployeeByCreateFromDB(employee.CreateBy);
                    employee.CreateByUsername = employeecreateby.Username;

                    employee.LastByUsername = "Admin";

                }
                else
                {
                    LoginCreateBy employeecreateby = GetEmployeeByCreateFromDB(employee.CreateBy);
                    employee.CreateByUsername = employeecreateby.Username;

                    employee.LastByUsername = GetEmployeeByLastUpDateFromDB(employee.LastUpdateBy);



                }



                return employee;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return employee;
        }
        private Login GetLoginByIdFromDB(int id)
        {
            Login login = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();


                string sqlSelect = $"SELECT l.*,e.* FROM tb_login AS l " +
                    $" LEFT JOIN tb_employee AS e ON l.emp_id = e.emp_id  WHERE e.emp_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    login = new Login
                    {
                        Username = reader["username"].ToString(),
                        Password = reader["password"].ToString(),

                    };
                }

                return login;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login;
        }
        public IActionResult GetAmphureFromDatabaseByAjax(int proId)
        {
            List<Amphure> amphures = new List<Amphure>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT * FROM amphures WHERE province_id = @proId";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@proId", proId);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Amphure amphure = new Amphure
                    {
                        AmpId = Convert.ToInt32(reader["id"]),
                        AName = reader["name_th_amp"].ToString(),
                        ProId = Convert.ToInt32(reader["province_id"])
                    };

                    amphures.Add(amphure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching amphure data from database.");
            }
            finally
            {
                connection.Close();
            }

            // Return the Partial View with the amphures data
            return PartialView("SelectAmphure", amphures);
        }
        public IActionResult GetDistrictFromDatabaseByAjax(int ampId)
        {
            List<District> districts = new List<District>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT * FROM districts WHERE amphure_id = @ampId";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                command.Parameters.AddWithValue("@ampId", ampId);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    District district = new District
                    {
                        DisId = Convert.ToInt32(reader["id"]),
                        DName = reader["name_th_dis"].ToString(),
                        ZipCode = Convert.ToInt32(reader["zip_code"]),
                        AmpId = Convert.ToInt32(reader["amphure_id"])
                    };

                    // และสามารถดึงข้อมูล Position จากตาราง tb_position ได้ตามต้องการ

                    districts.Add(district);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching district data from database.");
            }
            finally
            {
                connection.Close();
            }

            return PartialView("SelectDistrict", districts);
        }
        private List<Amphure> GetAmphureFromDatabaseById(int id)
        {
            List<Amphure> amphure = new List<Amphure>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = $"SELECT a.*,e.* FROM amphures AS a JOIN tb_employee AS e ON a.province_id = e.province_id WHERE e.emp_id = {id} ";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Amphure amphures = new Amphure
                            {
                                AmpId = Convert.ToInt32(reader["id"]),
                                AName = reader["name_th_amp"].ToString(),
                                ProId = Convert.ToInt32(reader["province_id"]),


                            };


                            amphure.Add(amphures);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                amphure = null;
            }

            return amphure;
        }
        private List<District> GetDistrictFromDatabaseById(int id)
        {
            List<District> district = new List<District>();
            string connectionString = _configuration.GetConnectionString("connectionStr");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = $"SELECT d.*,e.* FROM districts AS d JOIN tb_employee AS e ON d.amphure_id = e.amphure_id WHERE e.emp_id = {id}";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            District districts = new District
                            {
                                DisId = Convert.ToInt32(reader["id"]),
                                DName = reader["name_th_dis"].ToString(),
                                ZipCode = Convert.ToInt32(reader["zip_code"]),
                                AmpId = Convert.ToInt32(reader["amphure_id"]),

                            };


                            district.Add(districts);

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                district = null;
            }

            return district;
        }
        private LoginCreateBy GetEmployeeByCreateFromDB(string createby)
        {
            LoginCreateBy login_createby = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = $"SELECT * FROM tb_login WHERE emp_id = {createby}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    login_createby = new LoginCreateBy
                    {
                        Username = reader["username"].ToString(),


                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login_createby;
        }
        private string GetEmployeeByLastUpDateFromDB(string lastupdateby)
        {
            Login employee_lastupdateby = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = $"SELECT * FROM tb_login WHERE emp_id = {lastupdateby}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    employee_lastupdateby = new Login
                    {
                        Username = reader["username"].ToString(),


                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return employee_lastupdateby.Username;
        }
        private string GetLoginByLastUpDateFromDB(string lastupdateby)
        {
            Login login_lastupdateby = null;

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = $"SELECT * FROM tb_login WHERE emp_id = {lastupdateby}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    login_lastupdateby = new Login
                    {
                        Username = reader["username"].ToString(),


                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee data from database.");
            }
            finally
            {
                connection.Close();
            }

            return login_lastupdateby.Username;
        }
        //End method about Getting Condition



        //Start method about Updateing
        [HttpPost]
        public IActionResult UpdateDepInDB(Department updatedDepartment)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);


            string sqlUpdate = "UPDATE tb_department SET dep_name = @dep_name ,dep_status = @dep_status WHERE dep_id = @dep_id";


            if (string.IsNullOrWhiteSpace(updatedDepartment.DepStatus))
            {
                updatedDepartment.DepStatus = "1";
            }

            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@dep_id", updatedDepartment.DepId);
            command.Parameters.AddWithValue("@dep_name", updatedDepartment.DepName);
            command.Parameters.AddWithValue("@dep_status", updatedDepartment.DepStatus);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                var result = new { success = true, message = "Department updated successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error updating Department: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }
        [HttpPost]
        public IActionResult UpdatePosInDB(Position updatedPosition)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);

            string sqlUpdate = "UPDATE tb_position SET pos_name = @pos_name,pos_permissions_manage = @pos_permissions_manage , dep_id = @dep_id,pos_status = @pos_status WHERE pos_id = @pos_id";

            if (string.IsNullOrWhiteSpace(updatedPosition.PosPermissionsManage))
            {
                updatedPosition.PosPermissionsManage = "0";
            }

            if (string.IsNullOrWhiteSpace(updatedPosition.PosStatus))
            {
                updatedPosition.PosStatus = "1";
            }

            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@pos_id", updatedPosition.PosId);
            command.Parameters.AddWithValue("@pos_name", updatedPosition.PosName);
            command.Parameters.AddWithValue("@pos_permissions_manage", updatedPosition.PosPermissionsManage);
            command.Parameters.AddWithValue("@dep_id", updatedPosition.DepId);
            command.Parameters.AddWithValue("@pos_status", updatedPosition.PosStatus);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                var result = new { success = true, message = "Position updated successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error updating Position: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }
        [HttpPost]
        public IActionResult UpdateEmpInDB(Employee updatedEmployee)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);

            string sqlUpdate = "UPDATE tb_employee SET emp_name = @emp_name, date_of_birth = @date_of_birth, address = @address, " +
                               "email = @email, phone_number = @phone_number, salary = @salary, start_date = @start_date, " +
                               "end_date = @end_date,emp_status = @emp_status , pos_id = @pos_id,last_update_by = @Last_update_by, last_update_date = NOW()," +
                               "province_id = @ProId,amphure_id = @AmpId , district_id = @DisId, gender = @Gender " +
                               " WHERE emp_id = @emp_id";

            if (string.IsNullOrWhiteSpace(updatedEmployee.EmpStatus))
            {
                updatedEmployee.EmpStatus = "1";
            }

            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@emp_id", updatedEmployee.EmpId);
            command.Parameters.AddWithValue("@emp_name", updatedEmployee.EmpName);
            command.Parameters.AddWithValue("@date_of_birth", updatedEmployee.DateOfBirth);
            command.Parameters.AddWithValue("@address", updatedEmployee.Address);
            command.Parameters.AddWithValue("@email", updatedEmployee.Email);
            command.Parameters.AddWithValue("@phone_number", updatedEmployee.PhoneNumber);
            command.Parameters.AddWithValue("@salary", updatedEmployee.Salary);
            command.Parameters.AddWithValue("@start_date", updatedEmployee.StartDate);
            command.Parameters.AddWithValue("@end_date", updatedEmployee.EndDate);
            command.Parameters.AddWithValue("@emp_status", updatedEmployee.EmpStatus);
            command.Parameters.AddWithValue("@pos_id", updatedEmployee.PosId);
            command.Parameters.AddWithValue("@last_update_by", updatedEmployee.LastUpdateBy);
            command.Parameters.AddWithValue("@last_update_date", updatedEmployee.LastUpdateDate);
            command.Parameters.AddWithValue("@ProId", updatedEmployee.ProId);
            command.Parameters.AddWithValue("@AmpId", updatedEmployee.AmpId);
            command.Parameters.AddWithValue("@DisId", updatedEmployee.DisId);
            command.Parameters.AddWithValue("@Gender", updatedEmployee.Gender);


            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                var result = new { success = true, message = "Employee updated successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error updating employee: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }

        [HttpPost]
        public IActionResult UpdateProEmpInDB(Employee updatedEmployee)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string sqlUpdate = "UPDATE tb_employee SET emp_name = @emp_name, date_of_birth = @date_of_birth, address = @address, " +
                                   "email = @email, phone_number = @phone_number, salary = @salary, start_date = @start_date, " +
                                   "end_date = @end_date, emp_status = @emp_status, pos_id = @pos_id, last_update_by = @last_update_by, " +
                                   "last_update_date = NOW(), province_id = @province_id, amphure_id = @amphure_id, district_id = @district_id , gender = @Gender " +
                                   "WHERE emp_id = @emp_id";

                string sqlUpdateLogin = "UPDATE tb_login SET password = @password WHERE emp_id = @emp_id";

                using MySqlCommand command = new MySqlCommand(sqlUpdate, connection, transaction);
                using MySqlCommand command2 = new MySqlCommand(sqlUpdateLogin, connection, transaction);

                // Parameters for updating employee table
                command.Parameters.AddWithValue("@emp_id", updatedEmployee.EmpId);
                command.Parameters.AddWithValue("@emp_name", updatedEmployee.EmpName);
                command.Parameters.AddWithValue("@date_of_birth", updatedEmployee.DateOfBirth);
                command.Parameters.AddWithValue("@address", updatedEmployee.Address);
                command.Parameters.AddWithValue("@email", updatedEmployee.Email);
                command.Parameters.AddWithValue("@phone_number", updatedEmployee.PhoneNumber);
                command.Parameters.AddWithValue("@salary", updatedEmployee.Salary);
                command.Parameters.AddWithValue("@start_date", updatedEmployee.StartDate);
                command.Parameters.AddWithValue("@end_date", updatedEmployee.EndDate);
                command.Parameters.AddWithValue("@emp_status", updatedEmployee.EmpStatus);
                command.Parameters.AddWithValue("@pos_id", updatedEmployee.PosId);
                command.Parameters.AddWithValue("@last_update_by", updatedEmployee.LastUpdateBy);
                command.Parameters.AddWithValue("@province_id", updatedEmployee.ProId);
                command.Parameters.AddWithValue("@amphure_id", updatedEmployee.AmpId);
                command.Parameters.AddWithValue("@district_id", updatedEmployee.DisId);
                command.Parameters.AddWithValue("@Gender", updatedEmployee.Gender);

                // Parameters for updating login table
                command2.Parameters.AddWithValue("@emp_id", updatedEmployee.EmpId);
                command2.Parameters.AddWithValue("@password", updatedEmployee.Password);

                command.ExecuteNonQuery();
                command2.ExecuteNonQuery();

                transaction.Commit();

                var result = new { success = true, message = "Profile updated successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                var result = new { success = false, message = "Error updating profile: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }

        //End method about Updateing



        //Start method about Checking
        [HttpPost]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // สร้างคำสั่ง SQL เพื่อตรวจสอบ username ในฐานข้อมูล
                var query = "SELECT COUNT(*) FROM tb_employee WHERE Username = @Username";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    // ดึงจำนวนของ username ที่มีในฐานข้อมูล
                    var count = (long)await command.ExecuteScalarAsync();

                    // ส่งค่ากลับไปยังเว็บไซต์ว่า username ซ้ำหรือไม่
                    return Json(new { username_exists = count > 0 });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckDepNameExists(string dep_name)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // สร้างคำสั่ง SQL เพื่อตรวจสอบ username ในฐานข้อมูล
                var query = "SELECT COUNT(*) FROM tb_department WHERE dep_name = @Depname";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Depname", dep_name);

                    // ดึงจำนวนของ username ที่มีในฐานข้อมูล
                    var count = (long)await command.ExecuteScalarAsync();

                    // ส่งค่ากลับไปยังเว็บไซต์ว่า username ซ้ำหรือไม่
                    return Json(new { dep_name_exists = count > 0 });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckPosNameExists(string pos_name)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // สร้างคำสั่ง SQL เพื่อตรวจสอบ username ในฐานข้อมูล
                var query = "SELECT COUNT(*) FROM tb_position WHERE pos_name = @Posname";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Posname", pos_name);

                    // ดึงจำนวนของ username ที่มีในฐานข้อมูล
                    var count = (long)await command.ExecuteScalarAsync();

                    // ส่งค่ากลับไปยังเว็บไซต์ว่า username ซ้ำหรือไม่
                    return Json(new { pos_name_exists = count > 0 });
                }
            }
        }
        //End method about Checking



        //Start method about pagination
        public int GetTotalItemCount(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // สร้างคำสั่ง SQL เพื่อนับจำนวนรายการทั้งหมด
                string sql = "SELECT COUNT(*) FROM tb_employee AS e JOIN tb_position AS p ON e.pos_id = p.pos_id WHERE e.emp_status != '0'   " +
                    "AND (e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery OR e.email LIKE @SearchQuery  OR  p.pos_name LIKE @SearchQuery ) ";

                // สร้างและประมวลผลคำสั่ง SQL
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                int totalItemCount = (int)(long)command.ExecuteScalar();

                // คืนค่าจำนวนรายการทั้งหมด
                return totalItemCount;
            }
        }
        public int GetTotalItemCountByDepartment(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // สร้างคำสั่ง SQL เพื่อนับจำนวนรายการทั้งหมด
                string sql = "SELECT COUNT(*) FROM tb_department  WHERE dep_status != '0'  " +
                    " AND dep_name LIKE @SearchQuery  ";

                // สร้างและประมวลผลคำสั่ง SQL
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                int totalItemCount = (int)(long)command.ExecuteScalar();

                // คืนค่าจำนวนรายการทั้งหมด
                return totalItemCount;
            }
        }

        public int GetTotalItemCountByPosition(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // สร้างคำสั่ง SQL เพื่อนับจำนวนรายการทั้งหมด
                string sql = "SELECT COUNT(*) FROM tb_position AS p JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE p.pos_status != '0'   " +
                    "AND (p.pos_name LIKE @SearchQuery OR d.dep_name LIKE @SearchQuery ) ";

                // สร้างและประมวลผลคำสั่ง SQL
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                int totalItemCount = (int)(long)command.ExecuteScalar();

                // คืนค่าจำนวนรายการทั้งหมด
                return totalItemCount;
            }
        }

        //End method about pagination





    }

}



