using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class CyclePartCourse
    {
        public int Id { get; set; }
        public CyclePart CyclePart { get; set; }
        public int CyclePartId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public int Points { get; set; }
        public int PtsCrs1Sem1 { get; set; }
        public int PtsCrs1Sem2 { get; set; }
        public int PtsCrs2Sem1 { get; set; }
        public int PtsCrs2Sem2 { get; set; }
        public int PtsCrs3Sem1 { get; set; }
        public int PtsCrs3Sem2 { get; set; }
        public int PtsCrs4Sem1 { get; set; }
        public int PtsCrs4Sem2 { get; set; }

        public virtual List<OldStudyCard> OldStudyCards { get; set; }
        public virtual List<OldStudyCardCourse> OldStudyCardCourses { get; set; }
        public virtual List<TransferCourse> TransferCourses { get; set; }
    }
}
