
using iuca.Application.DTO.Common;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorCourseStudentGradeViewModel : InstructorCourseStudentViewModel
    {
        public int StudentCourseId { get; set; }
        public int? GradeId { get; set; }
        public GradeDTO Grade { get; set; }
    }
}
