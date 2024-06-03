using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class CyclePartCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Cycle part")]
        public CyclePartDTO CyclePart { get; set; }

        [Display(Name = "Cycle part")]
        public int CyclePartId { get; set; }

        [Display(Name = "Course")]
        public CourseDTO Course { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Group Id")]
        public Guid GroupId { get; set; }

        [Display(Name = "Group name")]
        public string GroupName { get; set; }

        [Display(Name = "Points")]
        public int Points { get; set; }

        [Display(Name = "Points course 1 semester 1")]
        public int PtsCrs1Sem1 { get; set; }

        [Display(Name = "Points course 1 semester 2")]
        public int PtsCrs1Sem2 { get; set; }

        [Display(Name = "Points course 2 semester 1")]
        public int PtsCrs2Sem1 { get; set; }

        [Display(Name = "Points course 2 semester 2")]
        public int PtsCrs2Sem2 { get; set; }

        [Display(Name = "Points course 3 semester 1")]
        public int PtsCrs3Sem1 { get; set; }

        [Display(Name = "Points course 3 semester 2")]
        public int PtsCrs3Sem2 { get; set; }

        [Display(Name = "Points course 4 semester 1")]
        public int PtsCrs4Sem1 { get; set; }

        [Display(Name = "Points course 4 semester 2")]
        public int PtsCrs4Sem2 { get; set; }
    }
}
