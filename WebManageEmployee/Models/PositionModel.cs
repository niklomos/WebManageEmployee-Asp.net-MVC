namespace WebManageEmployee.Models
{
    public class PositionModel
    {
        public List<Position> Positions { get; set;}
         public List<Province> Provinces { get; set; }
        public List<Amphure> Amphures { get; set; }
        public List<District> Districts { get; set; }
        public Employee Employee { get; set;}
        public Login Logins { get; set; }
        public int CurrentPage { get; set; }


    }
}
