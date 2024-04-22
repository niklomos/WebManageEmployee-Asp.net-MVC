namespace WebManageEmployee.Models
{
    public class EmployeeCount
    {
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Position> Positions { get; set; }

        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
    }
}
