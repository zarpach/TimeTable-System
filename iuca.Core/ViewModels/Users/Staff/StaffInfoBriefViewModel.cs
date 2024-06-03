using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Staff
{
    public class StaffInfoBriefViewModel
    {
        public string StaffUserId { get; set; }
        public string FullNameEng { get; set; }
        public bool IsReadOnly { get; set; }
        public string StaffBasicInfo { get; set; }
        public bool BasicInfoExists { get; set; }
        public string StaffInfo { get; set; }
    }
}
