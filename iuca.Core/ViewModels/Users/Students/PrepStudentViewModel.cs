using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Students
{
    public class PrepStudentViewModel
    {
        public string StudentName { get; set; }
        public int StudentBasicInfoId { get; set; }
        public int OrganizationId { get; set; }
        public string DepartmentGroup { get; set; }
        public int? PrepDepartmentGroupId { get; set; }
        public enu_StudentState StudentState { get; set; }
    }
}
