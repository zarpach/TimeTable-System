using iuca.Application.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface ICoursePrerequisiteService
    {
        /// <summary>
        /// Create course prerequisite record
        /// </summary>
        /// <param name="coursePrerequisiteDTO">Course prerequisite model</param>
        void Create(CoursePrerequisiteDTO coursePrerequisiteDTO);

        /// <summary>
        /// Edit course prerequisite by id
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="prerequisiteId">Course prerequisite id</param>
        /// <param name="coursePrerequisiteDTO">Course prerequisite model</param>
        void Edit(int courseId, int prerequisiteId, CoursePrerequisiteDTO coursePrerequisiteDTO);

        /// <summary>
        /// Delete course prerequisite by id
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="prerequisiteId">Course prerequisite id</param>
        void Delete(int courseId, int prerequisiteId);

        void Dispose();
    }
}
