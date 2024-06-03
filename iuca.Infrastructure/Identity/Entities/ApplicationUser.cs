using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Slots;
using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Staff;
using iuca.Domain.Entities.Users.Students;
using iuca.Domain.Entities.Users.UserInfo;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public bool IsMainOrganization { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string LastNameEng { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string FirstNameEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string MiddleNameEng { get; set; }

        public string FullNameEng
        {
            get { return $"{LastNameEng} {FirstNameEng} {MiddleNameEng}"; }
        }

        public virtual UserBasicInfo UserBasicInfo { get; set; }
        public virtual InstructorBasicInfo InstructorBasicInfo { get; set; }
        public virtual StudentBasicInfo StudentBasicInfo { get; set; }
        public virtual StaffBasicInfo StaffBasicInfo { get; set; }
        public virtual List<UserTypeOrganization> UserTypeOrganizations { get; set; }
        public virtual List<OldStudyCardCourse> OldStudyCardCourses { get; set; }
        public virtual List<AdviserStudent> AdviserStudentInstructors { get; set; }
        public virtual List<AdviserStudent> AdviserStudentStudents { get; set; }
        public virtual List<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public virtual List<StudentDebt> StudentDebts { get; set; }
        public virtual List<TransferCourse> TransferCourses { get; set; }
        public virtual List<StudentCourseGrade> StudentCourseGrades { get; set; }
        public virtual List<DeanDepartment> DeanDepartments { get; set; }
        public virtual List<DeanAdviser> DeanAdviserDeans { get; set; }
        public virtual List<DeanAdviser> DeanAdviserAdvisers { get; set; }
        public virtual List<AnnouncementSection> AnnouncementSections { get; set; }
        public virtual List<ExtraInstructor> ExtraInstructors { get; set; }
        public virtual List<ReinstatementExpulsionOrder> ReinstatementExpulsionOrders { get; set; }
        public virtual List<AcademicLeaveOrder> AcademicLeaveOrders { get; set; }
        public virtual List<GroupTransferOrder> GroupTransferOrders { get; set; }
        public virtual StudentTotalGPA StudentTotalGPA { get; set; }
        public virtual List<StudentSemesterGPA> StudentSemesterGPA { get; set; }
        public virtual List<Slot> Slots { get; set; }
    }
}
