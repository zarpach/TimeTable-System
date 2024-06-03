using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Courses
{
    public class StudentCourseRegistrationBriefViewModel
    {
        public int StudentRegistrationId { get; set; }
        public string Department { get; set; }
        public string StudentName { get; set; }
        public enu_StudentState StudentState { get; set; }
        public string Group { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        public enu_RegistrationState AddDropState { get; set; }
        public string AdviserName { get; set; }
        public float Credits { get; set; }
        public int TotalCourses { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateChange { get; set; }
        public bool NoCreditsLimitation { get; set; }
    }
}
