using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using WebManageEmployee.Models;


namespace WebManageEmployee.Controllers
{
    [CheckSessionFilter]
    public class RecycleController : Controller
    {
        private readonly ILogger<RecycleController> _logger;
        private readonly IConfiguration _configuration;

        public RecycleController(ILogger<RecycleController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RecycleEmployee(string searchQuery, int page = 1)
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
                var items = GetEmpFromDB(searchQuery,page, itemsPerPage);
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
        public IActionResult RecycleDepartment(string searchQuery, int page = 1)
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
                var items = GetDepFromDB(searchQuery, page, itemsPerPage);

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
        public IActionResult RecyclePosition(string searchQuery, int page = 1)
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

        public IActionResult UpdateRecycleDepartment(string searchquery, int id, int page = 1)
        {
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            var model = GetDepartmentByIdFromDB(id);
            model.CurrentPage = page;

            return View(model);

        }
        public IActionResult UpdateRecyclePosition(string searchquery, int id, int page = 1)
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
        public IActionResult UpdateRecycleEmployee(string searchquery, int id, int page = 1)
        {
            var employee = GetEmployeeByIdFromDB(id);
            var positions = GetPositionsFromDatabase();
            ViewBag.SearchQuery = System.Web.HttpUtility.UrlEncode(searchquery);

            PositionModel positionModel = new PositionModel();
            positionModel.Positions = positions;
            positionModel.Employee = employee;
            positionModel.CurrentPage = page;
           
            return View(positionModel);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //Start Method About Getting
        private IEnumerable<Department> GetDepFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Department> department = new List<Department>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;

                string sqlSelect = "SELECT * FROM tb_department WHERE dep_status != '1' " +
                    " AND  dep_name LIKE @SearchQuery " +
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
        private IEnumerable<Employee> GetEmpFromDB(string searchQuery ,int page, int itemsPerPage)
        {
            List<Employee> employee = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;
                string sqlSelect = "SELECT e.*, p.* FROM tb_employee AS e  JOIN tb_position AS p ON e.pos_id = p.pos_id WHERE e.emp_status != '1' " +
                     "AND (e.emp_number LIKE @SearchQuery OR e.emp_name LIKE @SearchQuery  OR  e.email LIKE @SearchQuery  OR  p.pos_name LIKE @SearchQuery ) " +
                      "ORDER BY e.last_update_date DESC " +
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
                        StartDate = Convert.ToDateTime(reader["start_date"]),
                        PosName = reader["pos_name"].ToString(),
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
        private IEnumerable<Position> GetPositionFromDB(string searchQuery, int page, int itemsPerPage)
        {
            List<Position> position = new List<Position>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                int offset = (page - 1) * itemsPerPage;
                string sqlSelect = "SELECT p.*,d.* FROM tb_position AS p JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE p.pos_status != '1'   " +
                               "AND (p.pos_name LIKE @SearchQuery OR d.dep_name LIKE @SearchQuery ) " +
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
                                PName = reader["name_th"].ToString(),


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
                                AName = reader["name_th"].ToString(),
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
                                DName = reader["name_th"].ToString(),
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
        //End Method About Getting



        //Start Method About Getting Codition
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

                string sqlSelect = $"SELECT p.* ,d.* FROM tb_position AS p JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE p.pos_id = {id}";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    position = new Position
                    {
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString(),
                        PosPermissionsManage = reader["pos_permissions_manage"].ToString(),

                        DepId = Convert.ToInt32(reader["dep_id"]),
                        DepName = reader["dep_name"].ToString()



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


                string sqlSelect = $"SELECT e.* ,p.*,pv.*,a.*,d.* FROM tb_employee AS e " +
                   $"LEFT JOIN tb_position AS p ON e.pos_id = p.pos_id " +
                   $"LEFT JOIN provinces AS pv ON e.province_id = pv.id " +
                   $"LEFT JOIN amphures AS a ON e.amphure_id = a.id " +
                   $"LEFT JOIN districts AS d ON e.district_id = d.id " +
                   $"WHERE e.emp_id = {id} ";
                  


                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    employee = new Employee
                    {
                        EmpName = reader["emp_name"].ToString(),
                        EmpId = Convert.ToInt32(reader["emp_id"]),
                        DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                        Gender = reader["gender"].ToString(),
                        Address = reader["address"].ToString(),
                        Email = reader["email"].ToString(),
                        PhoneNumber = reader["phone_number"].ToString(),
                        Salary = Convert.ToDecimal(reader["salary"].ToString()),
                        StartDate = Convert.ToDateTime(reader["start_date"]),
                        EndDate = Convert.ToDateTime(reader["end_date"]),
                        CreateBy = reader["create_by"].ToString(),
                        CreateDate = Convert.ToDateTime(reader["create_date"]),
                        LastUpdateBy = reader["last_update_by"].ToString(),
                        LastUpdateDate = Convert.ToDateTime(reader["last_update_date"]),
                        PosId = Convert.ToInt32(reader["pos_id"]),
                        PosName = reader["pos_name"].ToString(),
                        ProId = Convert.ToInt32(reader["province_id"]),
                        AmpId = Convert.ToInt32(reader["amphure_id"]),
                        DisId = Convert.ToInt32(reader["district_id"]),
                        PName = reader["name_th_pro"].ToString(),
                        AName = reader["name_th_amp"].ToString(),
                        DName = reader["name_th_dis"].ToString()

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
        //End Method About Getting Codition



        //Start Method About Updating
        [HttpPost]
        public IActionResult UpdateDepInDB(Department updatedDepartment)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);


            string sqlUpdate = "UPDATE tb_department SET dep_name = @dep_name ,dep_status = @dep_status WHERE dep_id = @dep_id";

            if (string.IsNullOrWhiteSpace(updatedDepartment.DepStatus))
            {
                updatedDepartment.DepStatus = "0";
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

            string sqlUpdate = "UPDATE tb_position SET pos_status = @pos_status WHERE pos_id = @pos_id";

            if (string.IsNullOrWhiteSpace(updatedPosition.PosPermissionsManage))
            {
                updatedPosition.PosPermissionsManage = "0";
            }

            if (string.IsNullOrWhiteSpace(updatedPosition.PosStatus))
            {
                updatedPosition.PosStatus = "0";
            }

            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@pos_id", updatedPosition.PosId);
            
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
        public IActionResult UpdateEmpInDB(Employee updatedEmployee)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using MySqlConnection connection = new MySqlConnection(connectionString);

            string sqlUpdate = "UPDATE tb_employee SET emp_status = @emp_status WHERE emp_id = @emp_id";

            if (string.IsNullOrWhiteSpace(updatedEmployee.EmpStatus))
            {
                updatedEmployee.EmpStatus = "0";
            }

            using MySqlCommand command = new MySqlCommand(sqlUpdate, connection);
            command.Parameters.AddWithValue("@emp_id", updatedEmployee.EmpId);
            command.Parameters.AddWithValue("@emp_status", updatedEmployee.EmpStatus);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                var result = new { success = true, message = "Employee restore successfully!" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { success = false, message = "Error restore employee: " + ex.Message };
                return Json(result);
            }
            finally
            {
                connection.Close();
            }
        }
        //End method about Updateing




        //Start method about pagination
        public int GetTotalItemCount(string searchQuery)
        {

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            {
                connection.Open();

                // สร้างคำสั่ง SQL เพื่อนับจำนวนรายการทั้งหมด
                string sql = "SELECT COUNT(*) FROM tb_employee AS e JOIN tb_position AS p ON e.pos_id = p.pos_id WHERE e.emp_status != '1'   " +
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
                string sql = "SELECT COUNT(*) FROM tb_department  WHERE dep_status != '1'  " +
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
                string sql = "SELECT COUNT(*) FROM tb_position AS p JOIN tb_department AS d ON p.dep_id = d.dep_id WHERE p.pos_status != '1'   " +
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
