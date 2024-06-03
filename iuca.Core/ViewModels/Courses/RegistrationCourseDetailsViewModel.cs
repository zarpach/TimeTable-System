using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class RegistrationCourseDetailsViewModel
    {
        public RegistrationCourseDTO RegistrationCourse { get; set; }
        public string InstructorName { get; set; }

        public List<RegistrationCourseStudentViewModel> CourseStudents { get; set; } = new List<RegistrationCourseStudentViewModel>();
    }

    public class RegistrationCourseStudentViewModel 
    {
        public string StudentUserId { get; set; }
        public int StudentCourseId { get; set; }
        public string StudentName { get; set; }
        public string Group { get; set; }
        public enu_StudentState StudentState { get; set; }
        public int Queue { get; set; }
        public bool IsDeleted { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
    }
}
