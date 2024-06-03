using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Students
{
    public class TranscriptViewModel
    {
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Department { get; set; }
        public List<string> StudentMinors { get; set; }

        public float TotalGPA { get; set; }
        public List<TranscriptSemester> TranscriptSemesters { get; set; } = new List<TranscriptSemester>();
        public List<TranscriptTransferCourse> TransferCourses { get; set; } = new List<TranscriptTransferCourse>();
    }

    public class TranscriptSemester 
    {
        public int Year { get; set; }
        public enu_Season Season { get; set; }
        public float GPACredits { get; set; }
        public float EarnedCredits { get; set; }
        public float AttemptedCredits { get; set; }
        public float QualityPoints { get; set; }
        public float SemesterGPA { get; set; }
        public int Order { get; set; }
        public List<TranscriptCourse> TranscriptCourses { get; set; } = new List<TranscriptCourse>();

    }

    public class TranscriptCourse 
    {
        public string CourseAbbreviation { get; set; }
        public string CourseNumber { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Grade { get; set; }
        public float Credits { get; set; }
    }

    public class TranscriptTransferCourse 
    {
        public string UniversityName { get; set; }
        public string CourseName { get; set; }
        public string Season { get; set; }
        public int Year { get; set; }
        public float Credits { get; set; }
    }
}
