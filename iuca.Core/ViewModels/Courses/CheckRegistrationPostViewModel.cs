using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class CheckRegistrationPostViewModel
    {
        public int StudentCourseRegistrationId { get; set; }
        public int SemesterId { get; set; }
        public bool Disaprove { get; set; }
        public string AdviserComment { get; set; }
        public string StudentComment { get; set; }

        public List<StudentCourseViewModel> StudentCourses { get; set; } = new List<StudentCourseViewModel>();
    }
    public class StudentCourseViewModel
    {
        public int StudentCourseId { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public int State { get; set; }
    }
}
