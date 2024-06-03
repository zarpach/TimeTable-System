using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class StudentCourseGradeDTO
    {
        public int Id { get; set; }
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public CourseDTO Course { get; set; }
        public int CourseId { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        public float Points { get; set; }
        public GradeDTO Grade { get; set; }
        public int? GradeId { get; set; }
        public OrganizationDTO Organization { get; set; }
        public int OrganizationId { get; set; }
        public int ImportCode { get; set; }
    }
}
