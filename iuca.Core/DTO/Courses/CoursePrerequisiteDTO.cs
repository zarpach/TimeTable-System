using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class CoursePrerequisiteDTO
    {
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Prerequisite")]
        public int PrerequisiteId { get; set; }

        [Display(Name = "Prerequisite")]
        public CourseDTO Prerequisite { get; set; }

    }
}
