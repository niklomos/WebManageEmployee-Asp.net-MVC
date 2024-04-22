namespace WebManageEmployee.Models
{
    public class DepartmentModel
    {
        public int CurrentPage { get; set; }

        public List<Department> Departments { get; set; }
        public Position Position { get; set; }

        public Department Department { get; set; }

    }
}
