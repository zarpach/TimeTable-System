using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface ICourseRequirementService
    {
        /// <summary>
        /// Create course requirement
        /// </summary>
        /// <param name="courseRequirementDTO">Course requirement</param>
        void CreateCourseRequirement(CourseRequirementDTO courseRequirementDTO);

        /// <summary>
        /// Edit course requirement by id
        /// </summary>
        /// <param name="courseRequirementId">Course requirement id</param>
        /// <param name="courseRequirementDTO">Course requirement</param>
        void EditCourseRequirement(int courseRequirementId, CourseRequirementDTO courseRequirementDTO);

        /// <summary>
        /// Delete course requirement by id
        /// </summary>
        /// <param name="courseRequirementId">Course requirement id</param>
        void DeleteCourseRequirement(int courseRequirementId);
    }
}
