using System.Collections.Generic;

namespace iuca.Application.Constants
{
    public class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
            };
        }

        #region Menu

        public static class MenuSettings
        {
            public const string View = "Permissions.MenuSettings.View";
        }

        public static class MenuCommon
        {
            public const string View = "Permissions.MenuCommon.View";
        }

        public static class MenuCourses
        {
            public const string View = "Permissions.MenuCourses.View";
        }

        public static class MenuUsers
        {
            public const string View = "Permissions.MenuUsers.View";
        }

        public static class MenuStudentDebts
        {
            public const string View = "Permissions.MenuStudentDebts.View";
        }

        public static class MenuTranscripts
        {
            public const string View = "Permissions.MenuTranscripts.View";
        }

        public static class MenuReports
        {
            public const string View = "Permissions.MenuReports.View";
        }

        #endregion

        #region Settings

        public static class EnvarSettings
        {
            public const string View = "Permissions.EnvarSettings.View";
            public const string Edit = "Permissions.EnvarSettings.Edit";
        }

        public static class ImportData
        {
            public const string View = "Permissions.ImportData.View";
            public const string Edit = "Permissions.ImportData.Edit";
        }

        public static class ExportData
        {
            public const string View = "Permissions.ExportData.View";
            public const string Edit = "Permissions.ExportData.Edit";
        }

        public static class Attendance
        {
            public const string View = "Permissions.Attendance.View";
            public const string Edit = "Permissions.Attendance.Edit";
        }

        #endregion

        #region Common

        public static class Organizations
        {
            public const string View = "Permissions.Organizations.View";
            public const string Edit = "Permissions.Organizations.Edit";
        }

        public static class Nationalities
        {
            public const string View = "Permissions.Nationalities.View";
            public const string Edit = "Permissions.Nationalities.Edit";
        }

        public static class Countries
        {
            public const string View = "Permissions.Countries.View";
            public const string Edit = "Permissions.Countries.Edit";
        }

        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Edit = "Permissions.Departments.Edit";
        }

        public static class Universities
        {
            public const string View = "Permissions.Universities.View";
            public const string Edit = "Permissions.Universities.Edit";
        }

        public static class EducationTypes
        {
            public const string View = "Permissions.EducationTypes.View";
            public const string Edit = "Permissions.EducationTypes.Edit";
        }

        public static class Semesters
        {
            public const string View = "Permissions.Semesters.View";
            public const string Edit = "Permissions.Semesters.Edit";
        }

        public static class SemesterPeriods
        {
            public const string View = "Permissions.SemesterPeriods.View";
            public const string Edit = "Permissions.SemesterPeriods.Edit";
        }

        public static class Languages
        {
            public const string View = "Permissions.Languages.View";
            public const string Edit = "Permissions.Languages.Edit";
        }

        public static class AttendanceFolders
        {
            public const string View = "Permissions.AttendanceFolders.View";
            public const string Edit = "Permissions.AttendanceFolders.Edit";
        }

        public static class DepartmentGroups
        {
            public const string View = "Permissions.DepartmentGroups.View";
            public const string Edit = "Permissions.DepartmentGroups.Edit";
        }

        public static class Grades
        {
            public const string View = "Permissions.Grades.View";
            public const string Edit = "Permissions.Grades.Edit";
        }

        public static class GradeManagement 
        {
            public const string View = "Permissions.GradeManagement.View";
            public const string Edit = "Permissions.GradeManagement.Edit";
        }

        public static class FFXXReport
        {
            public const string View = "Permissions.FFXXReport.View";
        }

        public static class GradeReport
        {
            public const string View = "Permissions.GradeReport.View";
        }

        public static class Policies
        {
            public const string View = "Permissions.Policies.View";
            public const string Edit = "Permissions.Policies.Edit";
        }

        #endregion

        #region Courses

        public static class Courses
        {
            public const string View = "Permissions.Courses.View";
            public const string Edit = "Permissions.Courses.Edit";
        }

        public static class Cycles
        {
            public const string View = "Permissions.Cycles.View";
            public const string Edit = "Permissions.Cycles.Edit";
        }

        public static class AcademicPlans
        {
            public const string View = "Permissions.AcademicPlans.View";
            public const string Edit = "Permissions.AcademicPlans.Edit";
        }

        public static class StudyCards
        {
            public const string View = "Permissions.StudyCards.View";
            public const string Edit = "Permissions.StudyCards.Edit";
        }

        public static class Proposals
        {
            public const string View = "Permissions.Proposals.View";
            public const string Edit = "Permissions.Proposals.Edit";
        }

        public static class Announcements
        {
            public const string View = "Permissions.Announcements.View";
            public const string Edit = "Permissions.Announcements.Edit";
        }

        public static class StudentsInSections
        {
            public const string View = "Permissions.StudentsInSections.View";
            public const string Edit = "Permissions.StudentsInSections.Edit";
        }

        public static class StudyCardPlaces
        {
            public const string View = "Permissions.StudyCardPlaces.View";
            public const string Edit = "Permissions.StudyCardPlaces.Edit";
        }

        public static class StudentCourseRegistrationsManagement
        {
            public const string View = "Permissions.StudentCourseRegistrationsManagement.View";
            public const string Edit = "Permissions.StudentCourseRegistrationsManagement.Edit";
        }

        public static class StudentCourseRegistrations
        {
            public const string View = "Permissions.StudentCourseRegistrations.View";
            public const string Edit = "Permissions.StudentCourseRegistrations.Edit";
        }

        public static class TransferCourses
        {
            public const string View = "Permissions.TransferCourses.View";
            public const string Edit = "Permissions.TransferCourses.Edit";
        }

        public static class InstructorCourses
        {
            public const string View = "Permissions.InstructorCourses.View";
            public const string Edit = "Permissions.InstructorCourses.Edit";
        }

        public static class StudentMidterms
        {
            public const string View = "Permissions.StudentMidterms.View";
            public const string Edit = "Permissions.StudentMidterms.Edit";
        }

        #endregion

        #region Users

        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Edit = "Permissions.Roles.Edit";
        }

        public static class UserRoles
        {
            public const string View = "Permissions.UserRoles.View";
            public const string Edit = "Permissions.UserRoles.Edit";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Edit = "Permissions.Users.Edit";
        }

        public static class Instructors
        {
            public const string View = "Permissions.Instructors.View";
            public const string Edit = "Permissions.Instructors.Edit";
        }

        public static class Students
        {
            public const string View = "Permissions.Students.View";
            public const string Edit = "Permissions.Students.Edit";
        }

        public static class Staff
        {
            public const string View = "Permissions.Staff.View";
            public const string Edit = "Permissions.Staff.Edit";
        }

        public static class Advisers
        {
            public const string View = "Permissions.Advisers.View";
            public const string Edit = "Permissions.Advisers.Edit";
        }

        public static class Deans
        {
            public const string View = "Permissions.Deans.View";
            public const string Edit = "Permissions.Deans.Edit";
        }

        public static class StudentOrders
        {
            public const string View = "Permissions.StudentOrders.View";
            public const string Edit = "Permissions.StudentOrders.Edit";
        }

        #endregion

        #region StedentDebts

        public static class DebtsAccounting
        {
            public const string View = "Permissions.DebtsAccounting.View";
            public const string Edit = "Permissions.DebtsAccounting.Edit";
        }

        public static class DebtsLibrary
        {
            public const string View = "Permissions.DebtsLibrary.View";
            public const string Edit = "Permissions.DebtsLibrary.Edit";
        }

        public static class DebtsDormitory
        {
            public const string View = "Permissions.DebtsDormitory.View";
            public const string Edit = "Permissions.DebtsDormitory.Edit";
        }

        public static class DebtsRegistarOffice
        {
            public const string View = "Permissions.DebtsRegistarOffice.View";
            public const string Edit = "Permissions.DebtsRegistarOffice.Edit";
        }

        public static class DebtsMedicineOffice
        {
            public const string View = "Permissions.DebtsMedicineOffice.View";
            public const string Edit = "Permissions.DebtsMedicineOffice.Edit";
        }

        #endregion

        #region Transcripts

        public static class TranscriptsAdmin
        {
            public const string View = "Permissions.TranscriptsAdmin.View";
        }

        public static class TranscriptsStudent
        {
            public const string View = "Permissions.TranscriptsStudent.View";
        }

        public static class TranscriptsDean
        {
            public const string View = "Permissions.TranscriptsDean.View";
        }

        public static class TranscriptsAdviser
        {
            public const string View = "Permissions.TranscriptsAdviser.View";
        }

        #endregion

        #region Syllabi

        public static class SyllabiEditor
        {
            public const string View = "Permissions.SyllabiEditor.View";
            public const string Edit = "Permissions.SyllabiEditor.Edit";
        }

        public static class SyllabiApprover
        {
            public const string View = "Permissions.SyllabiApprover.View";
            public const string Edit = "Permissions.SyllabiApprover.Edit";
        }

        #endregion

        #region Reports
        public static class RegistrationCoursesReport
        {
            public const string View = "Permissions.RegistrationCoursesReport.View";
        }

        public static class RegistrationCoursesDetailedReport
        {
            public const string View = "Permissions.RegistrationCoursesDetailedReport.View";
        }

        #endregion
    }
}
