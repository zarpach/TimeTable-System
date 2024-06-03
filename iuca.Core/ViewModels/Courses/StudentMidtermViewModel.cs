using iuca.Application.DTO.Courses;

namespace iuca.Application.ViewModels.Courses
{
    public class StudentMidtermViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentStatus { get; set; }
        public string StudentMajor { get; set; }
        public string StudentGroup { get; set; }
        public StudentMidtermDTO StudentMidterm { get; set; } = new StudentMidtermDTO();
    }
}
