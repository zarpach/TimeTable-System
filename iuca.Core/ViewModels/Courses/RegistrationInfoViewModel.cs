using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class RegistrationInfoViewModel
    {
        public bool IsStudent { get; set; }
        public bool IsCourseRegistrationOpened { get; set; }
        public bool IsAddDropOpened { get; set; }
        public bool IsDebt { get; set; }
        public bool IsRegistrationSubmitted { get; set; }
        public string RegistrationState { get; set; }
        public bool IsAddDropAllowed { get; set; }
        public bool IsAddDropSubmitted { get; set; }
        public string AddDropState { get; set; }
        public List<DebtInfoViewModel> DebtList { get; set; } 
    }
}
