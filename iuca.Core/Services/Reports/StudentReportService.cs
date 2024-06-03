using iuca.Application.Enums;
using iuca.Application.Interfaces.Reports;
using iuca.Application.ViewModels.Reports;
using iuca.Application.ViewModels.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Reports
{
    public class StudentReportService : IStudentReportService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public StudentReportService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager) 
        {
            _db = db;
            _userManager = userManager;
        }

    }
}
