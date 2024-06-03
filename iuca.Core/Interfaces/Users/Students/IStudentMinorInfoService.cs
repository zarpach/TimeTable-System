using iuca.Application.ViewModels.Users.Students;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentMinorInfoService
    {
        /// <summary>
        /// Get student minor info list by student basic info id
        /// </summary>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <returns>Student minor info list</returns>
        StudentMinorInfoViewModel GetStudentMinorInfo(int studentBasicInfoId);

        /// <summary>
        /// Edit student minor info
        /// </summary>
        /// <param name="studentMinorInfoViewModel">Student minor info</param>
        void EditStudentMinorInfo(StudentMinorInfoViewModel studentMinorInfoViewModel);
    }
}
