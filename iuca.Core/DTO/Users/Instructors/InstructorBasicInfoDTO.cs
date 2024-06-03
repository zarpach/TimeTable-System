using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Instructors
{
    public class InstructorBasicInfoDTO
    {
        public int Id { get; set; }

        public bool IsMainOrganization { get; set; }

        public bool IsMarried { get; set; }
        
        [Display(Name = "Children quantity")]
        [Range(0, int.MaxValue)]
        public int ChildrenQty { get; set; }
        public string InstructorUserId { get; set; }
        public int ImportCode { get; set; }
        public bool IsChanged { get; set; }
        public List<InstructorOtherJobInfoDTO> InstructorOtherJobInfo { get; set; }
        public List<InstructorEducationInfoDTO> InstructorEducationInfo { get; set; }
        public InstructorContactInfoDTO InstructorContactInfo { get; set; }
        public virtual List<InstructorOrgInfoDTO> InstructorOrgInfo { get; set; }
    }
}
