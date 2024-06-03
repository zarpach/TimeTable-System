using iuca.Application.DTO.Common;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentMinorInfoViewModel
    {
        public int StudentBasicInfoId { get; set; }
        public IEnumerable<int> DepartmentIds { get; set; }
        public IEnumerable<DepartmentDTO> Departments { get; set; }
    }
}
