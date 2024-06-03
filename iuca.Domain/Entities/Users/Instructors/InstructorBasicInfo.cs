using iuca.Domain.Entities.Courses;
using System.Collections.Generic;
using iuca.Domain.Entities.Users.UserInfo;

namespace iuca.Domain.Entities.Users.Instructors
{
    public class InstructorBasicInfo
    {
        public int Id { get; set; }
        public bool IsMainOrganization { get; set; }
        public bool IsMarried { get; set; }
        public int ChildrenQty { get; set; }
        public string InstructorUserId { get; set; }
        public int ImportCode { get; set; }
        public bool IsChanged { get; set; }

        public List<InstructorOtherJobInfo> InstructorOtherJobInfo { get; set; }
        public List<InstructorEducationInfo> InstructorEducationInfo { get; set; }
        public InstructorContactInfo InstructorContactInfo { get; set; }
        public virtual List<InstructorOrgInfo> InstructorOrgInfo { get; set; }
        public virtual List<OldStudyCardCourse> OldStudyCardCourses { get; set; }
    }
}
