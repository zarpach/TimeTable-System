using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class TransferCourseService : ITransferCourseService
    {
        private readonly IApplicationDbContext _db;

        public TransferCourseService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get transfer course list for student
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Transfer course list for student</returns>
        public IEnumerable<TransferCourseDTO> GetTransferCourses(int selectedOrganizationId, string studentUserId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<Grade, GradeDTO>();
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<TransferCourse, TransferCourseDTO>();
            }).CreateMapper();

            return mapper.Map<IEnumerable<TransferCourse>, IEnumerable<TransferCourseDTO>>(_db.TransferCourses
                .Where(x => x.OrganizationId == selectedOrganizationId && x.StudentUserId == studentUserId)
                .Include(x => x.University)
                .Include(x => x.Grade)
                .Include(x => x.CyclePartCourse).ThenInclude(x => x.Course));
        }

        /// <summary>
        /// Edit transfer courses of student
        /// </summary>
        /// <param name="selectedOrganizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="transferCourses">Transfer courses list</param>
        public void EditStudentTransferCourses(int selectedOrganizationId, string studentUserId, 
            List<TransferCourseDTO> transferCourses) 
        {
            var dbTransferCourses = _db.TransferCourses.Where(x => x.OrganizationId == selectedOrganizationId
                                                && x.StudentUserId == studentUserId).ToList();

            //If id exists - course exists in database and should be modified 
            var existingTransferCourses = transferCourses.Where(x => x.Id != 0).ToList();
            foreach (var existingTransferCourse in existingTransferCourses) 
            {
                EditTransferCourse(selectedOrganizationId, existingTransferCourse.Id, existingTransferCourse);
                transferCourses.Remove(existingTransferCourse);
                dbTransferCourses.RemoveAll(x => x.Id == existingTransferCourse.Id);
            }

            //Rest of courses are probably new
            foreach (var newTransferCourse in transferCourses) 
            {
                //Should check if course was removed and added again with zero id in form
                var existingTransferCourse = _db.TransferCourses
                    .FirstOrDefault(x => x.UniversityId == newTransferCourse.UniversityId &&
                        x.Year == newTransferCourse.Year && x.Season == newTransferCourse.Season &&
                        x.CyclePartCourseId == newTransferCourse.CyclePartCourseId &&
                        x.NameEng == newTransferCourse.NameEng && x.NameRus == newTransferCourse.NameRus);

                if (existingTransferCourse != null)
                {
                    newTransferCourse.Id = existingTransferCourse.Id;
                    EditTransferCourse(selectedOrganizationId, existingTransferCourse.Id, newTransferCourse);
                    dbTransferCourses.RemoveAll(x => x.Id == existingTransferCourse.Id);
                }
                else 
                {
                    newTransferCourse.OrganizationId = selectedOrganizationId;
                    newTransferCourse.StudentUserId = studentUserId;
                    CreateTransferCourse(newTransferCourse);
                }
            }

            //Remove transfer courses if some of them left
            if (dbTransferCourses.Count > 0)
            {
                foreach (var dbTransferCourse in dbTransferCourses)
                {
                    DeleteTransferCourse(selectedOrganizationId, dbTransferCourse.Id);
                }
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Create transfer course
        /// </summary>
        /// <param name="transferCourseDTO">Transfer course model</param>
        private void CreateTransferCourse(TransferCourseDTO transferCourseDTO)
        {
            if (transferCourseDTO == null)
                throw new Exception($"transferCourseDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransferCourse, TransferCourseDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<TransferCourseDTO, TransferCourse>();
            }).CreateMapper();

            TransferCourse newTransferCourse = mapperFromDTO.Map<TransferCourseDTO, TransferCourse>(transferCourseDTO);

            _db.TransferCourses.Add(newTransferCourse);
        }

        /// <summary>
        /// Edit transfer course
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of transfer course</param>
        /// <param name="transferCourseDTO">Transfer course model</param>
        private void EditTransferCourse(int selectedOrganizationId, int id, TransferCourseDTO transferCourseDTO)
        {
            if (transferCourseDTO == null)
                throw new Exception($"transferCourseDTO is null");

            TransferCourse transferCourse = _db.TransferCourses.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (transferCourse == null)
                throw new Exception($"Transfer course with id {id} not found");

            transferCourse.UniversityId = transferCourseDTO.UniversityId;
            transferCourse.Year = transferCourseDTO.Year;
            transferCourse.Season = transferCourseDTO.Season;
            transferCourse.GradeId = transferCourseDTO.GradeId;
            transferCourse.IsAcademicPlanCourse = transferCourseDTO.IsAcademicPlanCourse;
            transferCourse.CyclePartCourseId = transferCourseDTO.CyclePartCourseId;
            transferCourse.NameEng = transferCourseDTO.NameEng;
            transferCourse.NameRus = transferCourseDTO.NameRus;
            transferCourse.NameKir = transferCourseDTO.NameKir;

            _db.TransferCourses.Update(transferCourse);
        }

        /// <summary>
        /// Delete transfer course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of transfer course</param>
        private void DeleteTransferCourse(int selectedOrganizationId, int id)
        {
            TransferCourse transferCourse = _db.TransferCourses.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (transferCourse == null)
                throw new Exception($"Transfer course with id {id} not found");

            _db.TransferCourses.Remove(transferCourse);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
