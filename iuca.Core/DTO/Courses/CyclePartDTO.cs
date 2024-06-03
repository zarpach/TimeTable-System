using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class CyclePartDTO
    {
        public int Id { get; set; }

        [Display(Name = "Academic plan")]
        public int AcademicPlanId { get; set; }

        [Display(Name = "Academic plan")]
        public AcademicPlanDTO AcademicPlan { get; set; }

        [Display(Name = "Cycle")]
        public int CycleId { get; set; }

        [Display(Name = "Cycle")]
        public CycleDTO Cycle { get; set; }

        [Display(Name = "Part")]
        public int AcademicPlanPart { get; set; }

        [Display(Name = "Required points")]
        public int ReqPts { get; set; }

        [Display(Name = "Required points course 1 semester 1")]
        public int ReqPtsCrs1Sem1 { get; set; }

        [Display(Name = "Required points course 1 semester 2")]
        public int ReqPtsCrs1Sem2 { get; set; }

        [Display(Name = "Required points course 2 semester 1")]
        public int ReqPtsCrs2Sem1 { get; set; }

        [Display(Name = "Required points course 2 semester 2")]
        public int ReqPtsCrs2Sem2 { get; set; }

        [Display(Name = "Required points course 3 semester 1")]
        public int ReqPtsCrs3Sem1 { get; set; }

        [Display(Name = "Required points course 3 semester 2")]
        public int ReqPtsCrs3Sem2 { get; set; }

        [Display(Name = "Required points course 4 semester 1")]
        public int ReqPtsCrs4Sem1 { get; set; }

        [Display(Name = "Required points course 4 semester 2")]
        public int ReqPtsCrs4Sem2 { get; set; }

        public virtual List<CyclePartCourseDTO> CyclePartCourses { get; set; }
    }
}
