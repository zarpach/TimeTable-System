using AutoMapper;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.Students
{
    public class StudentMinorInfoService : IStudentMinorInfoService
    {
        private readonly IApplicationDbContext _db;

        public StudentMinorInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get student minor info list by student basic info id
        /// </summary>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <returns>Student minor info list</returns>
        public StudentMinorInfoViewModel GetStudentMinorInfo(int studentBasicInfoId)
        {
            /*if (studentBasicInfoId == 0)
                throw new Exception($"The student basic info id is 0.");*/

            IEnumerable<StudentMinorInfo> studentMinorInfo = _db.StudentMinorInfo.Where(x => x.StudentBasicInfoId == studentBasicInfoId);

            StudentMinorInfoViewModel studentMinorInfoViewModel = new StudentMinorInfoViewModel();
            studentMinorInfoViewModel.StudentBasicInfoId = studentBasicInfoId;
            studentMinorInfoViewModel.DepartmentIds = studentMinorInfo.Select(x => x.DepartmentId);

            return studentMinorInfoViewModel;
        }

        /// <summary>
        /// Edit student minor info
        /// </summary>
        /// <param name="studentMinorInfoViewModel">Student minor info</param>
        public void EditStudentMinorInfo(StudentMinorInfoViewModel studentMinorInfoViewModel)
        {
            if (studentMinorInfoViewModel == null)
                throw new Exception($"The student minor info is null.");
            if (studentMinorInfoViewModel.StudentBasicInfoId == 0)
                throw new Exception($"The student basic info id is 0.");

            IEnumerable<StudentMinorInfo> existingStudentMinorInfo = _db.StudentMinorInfo.Where(x => x.StudentBasicInfoId == studentMinorInfoViewModel.StudentBasicInfoId);

            List<StudentMinorInfo> itemsToDelete = studentMinorInfoViewModel.DepartmentIds != null ? existingStudentMinorInfo
                .Where(x => !studentMinorInfoViewModel.DepartmentIds.Contains(x.DepartmentId)).ToList()
                : existingStudentMinorInfo.ToList();

            List<StudentMinorInfo> itemsToCreate = studentMinorInfoViewModel.DepartmentIds != null ? studentMinorInfoViewModel.DepartmentIds
                .Where(x => !existingStudentMinorInfo.Select(x => x.DepartmentId).Contains(x))
                .Select(x => new StudentMinorInfo
                {
                    StudentBasicInfoId = studentMinorInfoViewModel.StudentBasicInfoId,
                    DepartmentId = x
                }).ToList()
                : new List<StudentMinorInfo>();

            _db.StudentMinorInfo.RemoveRange(itemsToDelete);
            _db.StudentMinorInfo.AddRange(itemsToCreate);
            _db.SaveChanges();
        }

    }
}
