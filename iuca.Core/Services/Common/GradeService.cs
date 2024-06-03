using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class GradeService : IGradeService
    {
        private readonly IApplicationDbContext _db;

        public GradeService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get grade list
        /// </summary>
        /// <returns>Grade list</returns>
        public IEnumerable<GradeDTO> GetGrades()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Grade, GradeDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Grade>, IEnumerable<GradeDTO>>(_db.Grades);
        }

        /// <summary>
        /// Get grade by id
        /// </summary>
        /// <param name="id">Id of grade</param>
        /// <returns>Grade model</returns>
        public GradeDTO GetGrade(int id)
        {
            Grade grade = _db.Grades.FirstOrDefault(x => x.Id == id);
            if (grade == null)
                throw new Exception($"Grade with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Grade, GradeDTO>()).CreateMapper();
            return mapper.Map<Grade, GradeDTO>(grade);
        }

        /// <summary>
        /// Create grade
        /// </summary>
        /// <param name="gradeDTO">Grade model</param>
        public void Create(GradeDTO gradeDTO)
        {
            if (gradeDTO == null)
                throw new Exception($"gradeDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Grade, GradeDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<GradeDTO, Grade>()).CreateMapper();

            Grade newGrade = mapperFromDTO.Map<GradeDTO, Grade>(gradeDTO);

            _db.Grades.Add(newGrade);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit grade
        /// </summary>
        /// <param name="id">Id of grade</param>
        /// <param name="gradeDTO">Grade model</param>
        public void Edit(int id, GradeDTO gradeDTO)
        {
            if (gradeDTO == null)
                throw new Exception($"gradeDTO is null");

            Grade grade = _db.Grades.FirstOrDefault(x => x.Id == id);
            if (grade == null)
                throw new Exception($"Grade with id {id} not found");

            grade.GradeMark = gradeDTO.GradeMark;
            grade.Gpa = gradeDTO.Gpa;
            grade.NameEng = gradeDTO.NameEng;
            grade.NameRus = gradeDTO.NameRus;
            grade.NameKir = gradeDTO.NameKir;

            _db.Grades.Update(grade);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete grade by id
        /// </summary>
        /// <param name="id">Id of grade</param>
        public void Delete(int id)
        {
            Grade grade = _db.Grades.FirstOrDefault(x => x.Id == id);
            if (grade == null)
                throw new Exception($"Grade with id {id} not found");

            _db.Grades.Remove(grade);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get grade SelectList
        /// </summary>
        /// <param name="gradeId">Selected grade id</param>
        /// <returns>SelectList of grade</returns>
        public List<SelectListItem> GetGradeSelectList(int? gradeId)
        {
            List<SelectListItem> list = new SelectList(_db.Grades, "Id", "GradeMark", gradeId)
                .OrderBy(x => x.Text).ToList();

            return list;
        }

        /// <summary>
        /// Get grade SelectList for transfer courses
        /// </summary>
        /// <param name="gradeId">Selected grade id</param>
        /// <returns>SelectList of grade for transfer courses</returns>
        public List<SelectListItem> GetGradeSelectListForTransferCourses(int? gradeId)
        {
            return new SelectList(_db.Grades.Where(x => x.GradeMark == "A" || x.GradeMark == "B" ||
                        x.GradeMark == "C" || x.GradeMark == "D" ||
                        x.GradeMark == "F").ToList(), "Id", "GradeMark", gradeId)
                .OrderBy(x => x.Text).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
