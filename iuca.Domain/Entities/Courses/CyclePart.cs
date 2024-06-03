using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class CyclePart
    {
        public int Id { get; set; }
        public int AcademicPlanId { get; set; }
        public AcademicPlan AcademicPlan { get; set; }
        public int CycleId { get; set; }
        public Cycle Cycle { get; set; }
        public int AcademicPlanPart { get; set; }
        public int ReqPts { get; set; }
        public int ReqPtsCrs1Sem1 { get; set; }
        public int ReqPtsCrs1Sem2 { get; set; }
        public int ReqPtsCrs2Sem1 { get; set; }
        public int ReqPtsCrs2Sem2 { get; set; }
        public int ReqPtsCrs3Sem1 { get; set; }
        public int ReqPtsCrs3Sem2 { get; set; }
        public int ReqPtsCrs4Sem1 { get; set; }
        public int ReqPtsCrs4Sem2 { get; set; }
        public virtual List<CyclePartCourse> CyclePartCourses { get; set; }
    }
}
