using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Common
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }

        public virtual List<UserTypeOrganization> UserTypeOrganizations { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual List<Semester> Semesters { get; set; }
        public virtual List<SemesterPeriod> SemesterPeriods { get; set; }
        public virtual List<InstructorOrgInfo> InstructorOrgInfo { get; set; }
        public virtual List<Course> Courses { get; set; }
        public virtual List<AcademicPlan> AcademicPlans { get; set; }
        public virtual List<OldStudyCard> OldStudyCards { get; set; }
        public virtual List<StudentOrgInfo> StudentOrgInfo { get; set; }
        public virtual List<AdviserStudent> AdviserStudents { get; set; }
        public virtual List<StudentCourseGrade> StudentCourseGrades { get; set; }
        public virtual List<TransferCourse> TransferCourses { get; set; }
        public virtual List<DeanDepartment> DeanDepartments { get; set; }
        public virtual List<DeanAdviser> DeanAdvisers { get; set; }
        public virtual List<AnnouncementSection> AnnouncementSections { get; set; }
        public virtual List<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public virtual List<EnvarSetting> EnvarSettings { get; set; }
        public virtual List<ReinstatementExpulsionOrder> ReinstatementExpulsionOrders { get; set; }
        public virtual List<AcademicLeaveOrder> AcademicLeaveOrders { get; set; }
        public virtual List<GroupTransferOrder> GroupTransferOrders { get; set; }
        public virtual List<StudentTotalGPA> StudentTotalGPAs {  get; set; }
        public virtual List<StudentSemesterGPA> StudentSemesterGPAs { get; set; }
    }
}
