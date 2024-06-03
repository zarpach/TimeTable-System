using iuca.Domain.Entities.Common;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentMinorInfo
    {
        public int Id { get; set; }
        public StudentBasicInfo StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
