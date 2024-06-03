using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Staff;
using iuca.Domain.Entities.Slots;

namespace iuca.Infrastructure.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<UserBasicInfo> UserBasicInfo { get; set; }
        DbSet<InstructorBasicInfo> InstructorBasicInfo { get; set; }
        DbSet<InstructorOtherJobInfo> InstructorOtherJobInfo { get; set; }
        DbSet<InstructorEducationInfo> InstructorEducationInfo { get; set; }
        DbSet<InstructorContactInfo> InstructorContactInfo { get; set; }
        DbSet<InstructorOrgInfo> InstructorOrgInfo { get; set; }
        DbSet<StaffBasicInfo> StaffBasicInfo { get; set; }
        DbSet<StudentBasicInfo> StudentBasicInfo { get; set; }
        DbSet<StudentContactInfo> StudentContactInfo { get; set; }
        DbSet<StudentLanguage> StudentLanguages { get; set; }
        DbSet<StudentOrgInfo> StudentOrgInfo { get; set; }
        DbSet<StudentParentsInfo> StudentParentsInfo { get; set; }
        DbSet<Organization> Organizations { get; set; }
        DbSet<UserTypeOrganization> UserTypeOrganizations { get; set; }
        DbSet<Nationality> Nationalities { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<University> Universities { get; set; }
        DbSet<EducationType> EducationTypes { get; set; }
        DbSet<Semester> Semesters { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Course> Courses { get; set; }
        DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        DbSet<Cycle> Cycles { get; set; }
        DbSet<AcademicPlan> AcademicPlans { get; set; }
        DbSet<CyclePart> CycleParts { get; set; }
        DbSet<CyclePartCourse> CyclePartCourses { get; set; }
        DbSet<SemesterPeriod> SemesterPeriods { get; set; }
        DbSet<DepartmentGroup> DepartmentGroups { get; set; }
        DbSet<OldStudyCard> OldStudyCards { get; set; }
        DbSet<OldStudyCardCourse> OldStudyCardCourses { get; set; }
        DbSet<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        DbSet<StudentCourse> StudentCourses { get; set; }
        DbSet<AdviserStudent> AdviserStudents { get; set; }
        DbSet<StudentDebt> StudentDebts { get; set; }
        DbSet<Grade> Grades { get; set; }
        DbSet<StudentCourseGrade> StudentCourseGrades { get; set; }
        DbSet<TransferCourse> TransferCourses { get; set; }
        DbSet<DeanDepartment> DeanDepartments { get; set; }
        DbSet<DeanAdviser> DeanAdvisers { get; set; }
        DbSet<AnnouncementSection> AnnouncementSections { get; set; }
        DbSet<StudentCourseTemp> StudentCoursesTemp { get; set; }
        DbSet<EnvarSetting> EnvarSettings { get; set; }
        DbSet<ExtraInstructor> ExtraInstructors { get; set; }
        DbSet<Syllabus> Syllabi { get; set; }
        DbSet<AcademicPolicy> AcademicPolicies { get; set; }
        DbSet<CourseRequirement> CourseRequirements { get; set; }
        DbSet<CourseCalendarRow> CourseCalendar { get; set; }
        DbSet<Policy> Policies { get; set; }
        DbSet<StudyCard> StudyCards { get; set; }
        DbSet<StudyCardCourse> StudyCardCourses { get; set; }
        DbSet<StudentMinorInfo> StudentMinorInfo { get; set; }
        DbSet<AcademicLeaveOrder> AcademicLeaveOrder { get; set; }
        DbSet<ReinstatementExpulsionOrder> ReinstatementExpulsionOrders { get; set; }
        DbSet<GroupTransferOrder> GroupTransferOrders { get; set; }
        DbSet<ProposalCourse> ProposalCourses { get; set; }
        DbSet<Proposal> Proposals { get; set; }
        DbSet<Announcement> Announcements { get; set; }
        DbSet<StudentMidterm> StudentMidterms { get; set; }
        DbSet<Attendance> Attendances { get; set; }
        DbSet<AttendanceClass> AttendanceClasses { get; set; }
        DbSet<StudentTotalGPA> StudentTotalGPAs { get; set; }
        DbSet<StudentSemesterGPA> StudentSemesterGPAs { get; set; }
        DbSet<AttendanceFolder> AttendanceFolders { get; set; }
        DbSet<Slot> Slots { get; set; }
        DbSet<LessonRoom> LessonRooms { get; set; }
        DbSet<LessonPeriod> LessonPeriods { get; set; }

        DatabaseFacade Database { get; }
        int SaveChanges();
        void Dispose();
    }
}
