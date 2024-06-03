using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class TransferCourse
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public University University { get; set; }
        public int UniversityId { get; set; }
        public int Year { get; set; }
        public int Season { get; set; }
        public Grade Grade { get; set; }
        public int? GradeId { get; set; }
        public bool IsAcademicPlanCourse { get; set; }
        public CyclePartCourse CyclePartCourse { get; set; }
        public int? CyclePartCourseId { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public float Points { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
