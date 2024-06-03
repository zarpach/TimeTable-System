using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class TransferCourseDTO
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public UniversityDTO University { get; set; }
        public int UniversityId { get; set; }
        public int Year { get; set; }
        public int Season { get; set; }
        public GradeDTO Grade { get; set; }
        public int? GradeId { get; set; }
        public bool IsAcademicPlanCourse { get; set; }
        public CyclePartCourseDTO CyclePartCourse { get; set; }
        public int? CyclePartCourseId { get; set; }

        [MaxLength(100)]
        public string NameEng { get; set; }

        [MaxLength(100)]
        public string NameRus { get; set; }

        [MaxLength(100)]
        public string NameKir { get; set; }

        public float Points { get; set; }
        public OrganizationDTO Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
