using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iuca.Application.Enums;

namespace iuca.Application.Services.Common
{
    public class SemesterPeriodService : ISemesterPeriodService
    {
        private readonly IApplicationDbContext _db;

        public SemesterPeriodService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get semester period list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Semester period list</returns>
        public IEnumerable<SemesterPeriodDTO> GetSemesterPeriods(int selectedOrganizationId)
        {
            var semesterPeriods = _db.SemesterPeriods.Include(x => x.Semester).Where(x => x.OrganizationId == selectedOrganizationId);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<SemesterPeriod, SemesterPeriodDTO>();
            }).CreateMapper();

            return mapper.Map<IEnumerable<SemesterPeriod>, IEnumerable<SemesterPeriodDTO>>(semesterPeriods);
        }

        /// <summary>
        /// Get semester period by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period/param>
        /// <returns>Semester period model</returns>
        public SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int id)
        {
            SemesterPeriod semesterPeriod = _db.SemesterPeriods.Include(x => x.Semester).FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semesterPeriod == null)
                throw new Exception($"Semester period with id {id} not found");

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<SemesterPeriod, SemesterPeriodDTO>();
            }).CreateMapper();

            return mapper.Map<SemesterPeriod, SemesterPeriodDTO>(semesterPeriod);
        }


        /// <summary>
        /// Get semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="period">Semester period value</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Semester period model</returns>
        public SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int period, int semesterId)
        {
            SemesterPeriod semesterPeriod = _db.SemesterPeriods.Include(x => x.Semester).FirstOrDefault(x => x.Period == period
                                                && x.SemesterId == semesterId && x.OrganizationId == selectedOrganizationId);
            /*if (semesterPeriod == null)
                throw new Exception($"Semester period with period {((enu_Period)period).GetDisplayName()} not found");*/

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<SemesterPeriod, SemesterPeriodDTO>();
            }).CreateMapper();

            return mapper.Map<SemesterPeriod, SemesterPeriodDTO>(semesterPeriod);
        }

        /// <summary>
        /// Get semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="period">Semester period value</param>
        /// <param name="date">Date included in the period</param>
        /// <returns>Semester period model</returns>
        public SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int period, DateTime date)
        {
            SemesterPeriod semesterPeriod = _db.SemesterPeriods.Include(x => x.Semester).Where(x => x.Period == period
                                                && x.DateBegin <= date && x.DateEnd >= date && x.OrganizationId == selectedOrganizationId)
                                                .OrderByDescending(x => x.DateBegin).FirstOrDefault();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<SemesterPeriod, SemesterPeriodDTO>();
            }).CreateMapper();

            return mapper.Map<SemesterPeriod, SemesterPeriodDTO>(semesterPeriod);
        }

        /// <summary>
        /// Create semester period
        /// </summary>
        /// <param name="semesterPeriodDTO">Semester period model</param>
        public void Create(SemesterPeriodDTO semesterPeriodDTO)
        {
            if (semesterPeriodDTO == null)
                throw new Exception($"semesterPeriodDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<SemesterPeriod, SemesterPeriodDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SemesterDTO, Semester>();
                cfg.CreateMap<SemesterPeriodDTO, SemesterPeriod>();
            }).CreateMapper();

            SemesterPeriod newSemesterPeriod = mapperFromDTO.Map<SemesterPeriodDTO, SemesterPeriod>(semesterPeriodDTO);

            _db.SemesterPeriods.Add(newSemesterPeriod);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period</param>
        /// <param name="semesterPeriodDTO">Semester period model</param>
        public void Edit(int selectedOrganizationId, int id, SemesterPeriodDTO semesterPeriodDTO)
        {
            if (semesterPeriodDTO == null)
                throw new Exception($"semesterPeriodDTO is null");

            SemesterPeriod semesterPeriod = _db.SemesterPeriods.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semesterPeriod == null)
                throw new Exception($"Semester period with id {id} not found");

            semesterPeriod.SemesterId = semesterPeriodDTO.SemesterId;
            semesterPeriod.Period = semesterPeriodDTO.Period;
            semesterPeriod.DateBegin = semesterPeriodDTO.DateBegin;
            semesterPeriod.DateEnd = semesterPeriodDTO.DateEnd;

            _db.SemesterPeriods.Update(semesterPeriod);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete semester period by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            SemesterPeriod semesterPeriod = _db.SemesterPeriods.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semesterPeriod == null)
                throw new Exception($"Semester period with id {id} not found");

            _db.SemesterPeriods.Remove(semesterPeriod);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
