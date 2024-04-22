namespace WebManageEmployee.Models
{
    public class Employee
    {
        public int EmpId { get; set; }
        public int EmpNumber { get; set; }
        public string EmpName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Salary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmpStatus { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int PosId { get; set; }
        public string PosName { get; set; }
        public string CreateByUsername { get; set; }
        public string LastByUsername { get; set; }
        public string Username { get; set; }
        public string ProfileImage { get; set; }

        public int ProId { get; set; }
        public int AmpId { get; set; }
        public int DisId { get; set; }
        public string PName { get; set; }
        public string AName { get; set; }
        public string DName { get; set; }
        public string PosPermissionsManage { get; set; }
        public string PosStatus { get; set; }
        public string Password { get; set; }



        public Position Position { get; set; }
        public Login Login { get; set; }


    }
}
