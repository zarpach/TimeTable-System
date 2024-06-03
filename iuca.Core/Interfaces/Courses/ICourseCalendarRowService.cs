using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface ICourseCalendarRowService
    {
        /// <summary>
        /// Create course calendar row
        /// </summary>
        /// <param name="courseCalendarRowDTO">Course calendar row</param>
        void CreateCourseCalendarRow(CourseCalendarRowDTO courseCalendarRowDTO);

        /// <summary>
        /// Edit course calendar row by id
        /// </summary>
        /// <param name="courseCalendarRowId">Course calendar row id</param>
        /// <param name="courseCalendarRowDTO">Course calendar row</param>
        void EditCourseCalendarRow(int courseCalendarRowId, CourseCalendarRowDTO courseCalendarRowDTO);

        /// <summary>
        /// Delete course calendar row by id
        /// </summary>
        /// <param name="courseCalendarRowId">Course calendar row id</param>
        void DeleteCourseCalendarRow(int courseCalendarRowId);
    }
}
