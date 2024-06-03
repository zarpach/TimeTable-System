using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iuca.Application.Services.Common
{
    public class SemesterService : ISemesterService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public SemesterService(IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Get semester list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Semester list</returns>
        public IEnumerable<SemesterDTO> GetSemesters(int selectedOrganizationId)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Semester>, IEnumerable<SemesterDTO>>(
                _db.Semesters.Where(x => x.OrganizationId == selectedOrganizationId)
                .OrderBy(x => x.Year)
                .ThenByDescending(x => x.Season)
                );
        }

        /// <summary>
        /// Get semester list starting from admission year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="admissionYear">Year of admission</param>
        /// <returns>Semester list</returns>
        public List<SemesterDTO> GetSemestersByAdmissionYear(int selectedOrganizationId, int admissionYear)
        {
            List<Semester> semesters = new List<Semester>();

            int currentYear = DateTime.Now.Year;
            int currentSeason = (int)GetCurrentSeason();
            var seasons = Enum.GetValues(typeof(enu_Season)).Cast<int>().ToList();

            for (; admissionYear <= currentYear; admissionYear++) 
            {
                for (int i = 0; i < seasons.Count; i++) 
                {
                    if (admissionYear == currentYear && seasons[i] > currentSeason)
                        break;

                    var semester = _db.Semesters.FirstOrDefault(x => x.Year == admissionYear && x.Season == seasons[i]
                                    && x.OrganizationId == selectedOrganizationId);
                    if (semester != null)
                        semesters.Add(semester);
                }
            }

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<List<Semester>, List<SemesterDTO>>(semesters);
        }

        /// <summary>
        /// Get semester by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        /// <returns>Semester model</returns>
        public SemesterDTO GetSemester(int selectedOrganizationId, int id)
        {
            Semester semester = _db.Semesters.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semester == null)
                throw new Exception($"Semester with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<Semester, SemesterDTO>(semester);
        }

        /// <summary>
        /// Get semester by id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Semester</returns>
        public SemesterDTO GetSemester(int semesterId)
        {
            if (semesterId == 0)
                throw new ArgumentException($"The semester id is 0.");

            Semester semester = _db.Semesters.FirstOrDefault(x => x.Id == semesterId);
            if (semester == null)
                throw new Exception($"Semester with id {semesterId} not found");

            return _mapper.Map<SemesterDTO>(semester);
        }

        /// <summary>
        /// Get semester by year and season
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="year">Year of semester</param>
        /// <param name="season">Season of semester</param>
        /// <param name="generateException">If true and semester not found generate exception</param>
        /// <returns>Semester model</returns>
        public SemesterDTO GetSemester(int selectedOrganizationId, int year, int season, bool generateException = true)
        {
            Semester semester = _db.Semesters.FirstOrDefault(x => x.Year == year && x.Season == season &&
                x.OrganizationId == selectedOrganizationId);

            if (semester == null && generateException)
                throw new Exception($"Semester for year {year} and season {season} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<Semester, SemesterDTO>(semester);
        }

        /// <summary>
        /// Create semester
        /// </summary>
        /// <param name="semesterDTO">Semester model</param>
        public void Create(SemesterDTO semesterDTO)
        {
            if (semesterDTO == null)
                throw new Exception($"semesterDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<SemesterDTO, Semester>()).CreateMapper();

            Semester newSemester = mapperFromDTO.Map<SemesterDTO, Semester>(semesterDTO);

            _db.Semesters.Add(newSemester);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit semester
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        /// <param name="semesterDTO">Semester model</param>
        public void Edit(int selectedOrganizationId, int id, SemesterDTO semesterDTO)
        {
            if (semesterDTO == null)
                throw new Exception($"semesterDTO is null");

            Semester semester = _db.Semesters.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semester == null)
                throw new Exception($"Semester with id {id} not found");

            semester.Season = semesterDTO.Season;
            semester.Year = semesterDTO.Year;

            _db.Semesters.Update(semester);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete semester by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            Semester semester = _db.Semesters.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (semester == null)
                throw new Exception($"Semester with id {id} not found");

            _db.Semesters.Remove(semester);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get current semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="generateException">If true generates exception when record not found</param>
        /// <returns>Current semester</returns>
        public SemesterDTO GetCurrentSemester(int selectedOrganizationId, bool generateException = true) 
        {
            int season = (int)GetCurrentSeason();
            int year = DateTime.Now.Year;

            Semester semester = _db.Semesters.FirstOrDefault(x => x.Year == year && x.Season == season &&
                x.OrganizationId == selectedOrganizationId);

            if (semester == null && generateException)
                throw new Exception($"Semester for year {year} and season {season} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<Semester, SemesterDTO>(semester);

        }

        /// <summary>
        /// Get current educational (fall or spring) semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="generateException">If true generates exception when the record not found</param>
        /// <returns>Current educational semester</returns>
        public SemesterDTO GetCurrentEducationalSemester(int selectedOrganizationId, bool generateException = true)
        {
            int season = (int)GetCurrentEducationalSeason();
            int year = DateTime.Now.Year;

            Semester semester = _db.Semesters.FirstOrDefault(x => x.Year == year && x.Season == season &&
                x.OrganizationId == selectedOrganizationId);

            if (semester == null && generateException)
                throw new Exception($"Semester for year {year} and season {season} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<Semester, SemesterDTO>(semester);

        }

        /// <summary>
        /// Get next educational (fall or spring) semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Next educational semester</returns>
        public SemesterDTO GetNextEducationalSemester(int selectedOrganizationId)
        {
            int year = DateTime.Now.Year;
            enu_Season season = GetNextEducationalSeason();
            if (season == enu_Season.Spring)
                year++;

            Semester semester = _db.Semesters.FirstOrDefault(x => x.Year == year && x.Season == (int)season &&
                x.OrganizationId == selectedOrganizationId);

            /*if (semester == null)
                throw new Exception($"Semester for year {year} and season {season} not found");*/

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Semester, SemesterDTO>()).CreateMapper();
            return mapper.Map<Semester, SemesterDTO>(semester);

        }

        /// <summary>
        /// Get season select list
        /// </summary>
        /// <param name="season">Selected season</param>>
        /// <returns>Season select list</returns>
        public List<SelectListItem> GetSeasonList(int season)
        {
            var seasons = from enu_Season d in Enum.GetValues(typeof(enu_Season))
                             select new { Id = (int)d, Name = d.GetDisplayName() };
            return new SelectList(seasons, "Id", "Name", season).ToList();
        }

        /// <summary>
        /// Get current season for current date
        /// </summary>
        /// <returns>Current season</returns>
        private enu_Season GetCurrentSeason() 
        {
            int month = DateTime.Now.Month;
            enu_Season season = enu_Season.Winter;
            if (month < 12 && month > 8)
                season = enu_Season.Fall;
            else if (month < 9 && month > 5)
                season = enu_Season.Summer;
            else if (month < 6 && month > 2)
                season = enu_Season.Spring;

            return season;
        }

        /// <summary>
        /// Get current educational (fall or spring) season for current date
        /// </summary>
        /// <returns>Current educational season</returns>
        private enu_Season GetCurrentEducationalSeason()
        {
            int month = DateTime.Now.Month;
            enu_Season season = enu_Season.Fall;
            if (month < 8 && month > 1)
                season = enu_Season.Spring;

            return season;
        }

        /// <summary>
        /// Get next educational (fall or spring) season for current date
        /// </summary>
        /// <returns>Next educational season</returns>
        private enu_Season GetNextEducationalSeason()
        {
            int month = DateTime.Now.Month;
            enu_Season season = enu_Season.Fall;
            if (month > 8)
                season = enu_Season.Spring;

            return season;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
