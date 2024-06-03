using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Staff;
using iuca.Domain.Entities.Slots;

namespace iuca.Infrastructure.Persistence
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<UserBasicInfo> UserBasicInfo { get; set; }
        public DbSet<InstructorBasicInfo> InstructorBasicInfo { get; set; }
        public DbSet<InstructorOtherJobInfo> InstructorOtherJobInfo { get; set; }
        public DbSet<InstructorEducationInfo> InstructorEducationInfo { get; set; }
        public DbSet<InstructorContactInfo> InstructorContactInfo { get; set; }
        public DbSet<InstructorOrgInfo> InstructorOrgInfo { get; set; }
        public DbSet<StaffBasicInfo> StaffBasicInfo { get; set; }
        public DbSet<StudentBasicInfo> StudentBasicInfo { get; set; }
        public DbSet<StudentOrgInfo> StudentOrgInfo { get; set; }
        public DbSet<StudentContactInfo> StudentContactInfo { get; set; }
        public DbSet<StudentLanguage> StudentLanguages { get; set; }
        public DbSet<StudentParentsInfo> StudentParentsInfo { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<UserTypeOrganization> UserTypeOrganizations { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<EducationType> EducationTypes { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<Cycle> Cycles { get; set; }
        public DbSet<AcademicPlan> AcademicPlans { get; set; }
        public DbSet<CyclePart> CycleParts { get; set; }
        public DbSet<CyclePartCourse> CyclePartCourses { get; set; }
        public DbSet<SemesterPeriod> SemesterPeriods { get; set; }
        public DbSet<DepartmentGroup> DepartmentGroups { get; set; }
        public DbSet<OldStudyCard> OldStudyCards { get; set; }
        public DbSet<OldStudyCardCourse> OldStudyCardCourses { get; set; }
        public DbSet<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<AdviserStudent> AdviserStudents { get; set; }
        public DbSet<StudentDebt> StudentDebts { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentCourseGrade> StudentCourseGrades { get; set; }
        public DbSet<TransferCourse> TransferCourses { get; set; }
        public DbSet<DeanDepartment> DeanDepartments { get; set; }
        public DbSet<DeanAdviser> DeanAdvisers { get; set; }
        public DbSet<AnnouncementSection> AnnouncementSections { get; set; }
        public DbSet<StudentCourseTemp> StudentCoursesTemp { get; set; }
        public DbSet<EnvarSetting> EnvarSettings { get; set; }
        public DbSet<ExtraInstructor> ExtraInstructors { get; set; }
        public DbSet<Syllabus> Syllabi { get; set; }
        public DbSet<AcademicPolicy> AcademicPolicies { get; set; }
        public DbSet<CourseRequirement> CourseRequirements { get; set; }
        public DbSet<CourseCalendarRow> CourseCalendar { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<StudyCard> StudyCards { get; set; }
        public DbSet<StudyCardCourse> StudyCardCourses { get; set; }
        public DbSet<StudentMinorInfo> StudentMinorInfo { get; set; }
        public DbSet<AcademicLeaveOrder> AcademicLeaveOrder { get; set; }
        public DbSet<ReinstatementExpulsionOrder> ReinstatementExpulsionOrders { get; set; }
        public DbSet<GroupTransferOrder> GroupTransferOrders { get; set; }
        public DbSet<ProposalCourse> ProposalCourses { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<StudentMidterm> StudentMidterms { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceClass> AttendanceClasses { get; set; }
        public DbSet<StudentTotalGPA> StudentTotalGPAs { get; set; }
        public DbSet<StudentSemesterGPA> StudentSemesterGPAs { get; set; }
        public DbSet<AttendanceFolder> AttendanceFolders { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<LessonRoom> LessonRooms { get; set; }
        public DbSet<LessonPeriod> LessonPeriods { get; set; }
    }
}
