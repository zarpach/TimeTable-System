
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;

namespace iuca.Application.ViewModels.Reports
{
    public class DepartmentStudentViewModel
    {
        public DepartmentDTO Department { get; set; }
        public DepartmentGroupDTO DepartmentGroup { get; set; }
        public string StudentName { get; set; }
        public string StudentState { get; set; }
        public bool IsPrep { get; set; }
    }
}
