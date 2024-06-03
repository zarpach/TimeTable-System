using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudentCourseRegistrationService
    {
        /// <summary>
        /// Get student course registration list
        /// </summary>
        /// <param name="organizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="minCredits">Minimum registration credits</param>
        /// <param name="registrationState">Registration state</param>
        /// <param name="studentState">Student state</param>
        /// <returns>Student course registration list</returns>
        public IEnumerable<StudentCourseRegistrationBriefViewModel> GetStudentCourseRegistrations(int organizationId,
            int semesterId, int? departmentId, int? minCredits, enu_RegistrationState registrationState,
            enu_StudentState studentState);

        /// <summary>
        /// Set no credit limitation flag to registration
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="noCreditLimitation">No credit limitation flag</param>
        void SetNoCreditsLimitation(int registrationId, bool noCreditLimitation);

        /// <summary>
        /// Get student course registration by id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student course registration model</returns>
        StudentCourseRegistrationDTO GetStudentCourseRegistration(int id);

        /// <summary>
        /// Get student course registration by semester id and student id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student course registration model</returns>
        StudentCourseRegistrationDTO GetStudentCourseRegistration(int semesterId, string studentUserId);

        /// <summary>
        /// Set registration state
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="state">State</param>
        void SetRegistrationState(int registrationId, enu_RegistrationState state);

        /// <summary>
        /// Set add/drop state
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="state">State</param>
        void SetAddDropState(int registrationId, enu_RegistrationState state);

        /// <summary>
        /// Create student course registration
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        void Create(int organizationId, int semesterId, string studentUserId);

        /// <summary>
        /// Create student course registration
        /// </summary>
        /// <param name="studentCourseRegistrationDTO">Student course registration model</param>
        void Create(StudentCourseRegistrationDTO studentCourseRegistrationDTO);

        /// <summary>
        /// Edit student course registration
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <param name="studentCourseRegistrationDTO">Student course registration model</param>
        void Edit(int id, StudentCourseRegistrationDTO studentCourseRegistrationDTO);

        /// <summary>
        /// Delete student course registration by id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        void Delete(int id);

        /// <summary>
        /// Get registration courses model for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="onlySelectedCourses">If true gets courses selected by student only</param>
        /// <returns>Registration courses model</returns>
        RegistrationSelectionCourseViewModel GetCoursesForSelectionTemp(int organizationId, int semesterId, string studentUserId, bool onlySelectedCourses);

        /// <summary>
        /// Get courses list from study card
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentGroupId">Student user id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Courses list from study card</returns>
        public StudyCardCoursesViewModel GetCoursesForSelectionFromStudyCard(int semesterId,
            int departmentGroupId, string studentUserId, int studentCourseRegistrationId);

        /// <summary>
        /// Get registration courses model for selection for add/drop period
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Registration courses model</returns>
        RegistrationSelectionCourseViewModel GetCoursesForAddDropSelectionTemp(int organizationId, int semesterId, string studentUserId);

        /// <summary>
        /// Get registration courses model for selection
        /// </summary>
        /// <param name="semesterId">Semester id to select study card</param>
        /// <param name="departmentGroupId">Group id for courses selection</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="onlySelectedCourses">If true gets courses selected by student only</param>
        /// <returns>Registration courses model</returns>
        RegistrationSelectionCourseViewModel GetCoursesForSelection(int semesterId, int departmentGroupId, string studentUserId, bool onlySelectedCourses);

        /// <summary>
        /// Get student queue for registration course
        /// </summary>
        /// <param name="registrationCourseId"></param>
        /// <param name="places"></param>
        /// <param name="studentUserId"></param>
        /// <returns>Student queue</returns>
        int GetStudentQueue(int registrationCourseId, int places, string studentUserId);

        /// <summary>
        /// Get current student course registration step. Create registration if not exists
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Model with current step, registration id and state </returns>
        CourseRegistrationStepsViewModel GetStudentCourseRegistrationStep(int selectedOrganizationId, string studentUserId);

        /// <summary>
        /// Get current student add/drop course step.
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Model with current step, registration id and state </returns>
        CourseRegistrationStepsViewModel GetStudentAddDropCourseStep(int selectedOrganizationId, string studentUserId);

        /// <summary>
        /// Save student comment
        /// </summary>
        /// <param name="id">Student course registration id</param>
        /// <param name="comment">Student comment</param>
        void SaveStudentComment(int id, string comment);

        /// <summary>
        /// Save student comment for add/drop period
        /// </summary>
        /// <param name="id">Student course registration id</param>
        /// <param name="comment">Student comment</param>
        void SaveStudentAddDropComment(int id, string comment);

        /// <summary>
        /// Send registration on approval to advisor
        /// </summary>
        /// <param name="id">Student course registration id</param>
        void SendOnApproval(int id);

        /// <summary>
        /// Send add/drop registration on approval to advisor
        /// </summary>
        /// <param name="id">Student course registration id</param>
        void SendAddDropOnApproval(int id);

        /// <summary>
        /// Submit registration by student
        /// </summary>
        /// <param name="id">Student course registration id</param>
        void SubmitRegistration(int id);

        /// <summary>
        /// Submit add/drop form of a student
        /// </summary>
        /// <param name="id">Student course registration id</param>
        public void SubmitAddDropForm(int id);

        /// <summary>
        /// Get student registration information
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="userId">User id</param>
        /// <returns>Inforamtion model</returns>
        RegistrationInfoViewModel GetStudentRegistrationInfo(int organizationId, string userId);

        void Dispose();
    }
}
