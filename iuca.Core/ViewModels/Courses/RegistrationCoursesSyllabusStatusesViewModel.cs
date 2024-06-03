using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.ViewModels.Courses
{
    public class RegistrationCoursesSyllabusStatusesViewModel
    {
        [Display(Name = "Registartion Courses")]
        public IEnumerable<RegistrationCourseViewModel> RegistrationCourses { get; set; }

        [Display(Name = "Not Added Count")]
        public int notAddedCount { get; set; }

        [Display(Name = "Not Submitted Count")]
        public int notSubmittedCount { get; set; }

        [Display(Name = "Rejected Count")]
        public int rejectedCount { get; set; }

        [Display(Name = "Pending Count")]
        public int pendingCount { get; set; }

        [Display(Name = "Approved Count")]
        public int approvedCount { get; set; }
    }
}
