namespace WebManageEmployee.Models
{
    public class Login
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmpName { get; set; }
        public int EmpId { get; set; }
        public string RememberMe { get; set; }   

        public Employee Employee { get; set; }

    }
}
