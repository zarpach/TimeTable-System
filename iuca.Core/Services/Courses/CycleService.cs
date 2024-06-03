using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Courses
{
    public class CycleService : ICycleService
    {
        private readonly IApplicationDbContext _db;

        public CycleService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get cycle list
        /// </summary>
        /// <returns>Cycle list</returns>
        public IEnumerable<CycleDTO> GetCycles()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Cycle, CycleDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Cycle>, IEnumerable<CycleDTO>>(_db.Cycles).OrderBy(x => x.NameEng);
        }

        /// <summary>
        /// Get cycle by id
        /// </summary>
        /// <param name="id">Id of cycle</param>
        /// <returns>Cycle model</returns>
        public CycleDTO GetCycle(int id)
        {
            Cycle cycle = _db.Cycles.FirstOrDefault(x => x.Id == id);
            if (cycle == null)
                throw new Exception($"Cycle with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Cycle, CycleDTO>()).CreateMapper();
            return mapper.Map<Cycle, CycleDTO>(cycle);
        }

        /// <summary>
        /// Create cycle
        /// </summary>
        /// <param name="cycleDTO">Cycle model</param>
        public void Create(CycleDTO cycleDTO)
        {
            if (cycleDTO == null)
                throw new Exception($"cycleDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Cycle, CycleDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<CycleDTO, Cycle>()).CreateMapper();

            Cycle newCycle = mapperFromDTO.Map<CycleDTO, Cycle>(cycleDTO);

            _db.Cycles.Add(newCycle);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit cycle
        /// </summary>
        /// <param name="id">Id of cycle</param>
        /// <param name="cycleDTO">Cycle model</param>
        public void Edit(int id, CycleDTO cycleDTO)
        {
            if (cycleDTO == null)
                throw new Exception($"cycleDTO is null");

            Cycle cycle = _db.Cycles.FirstOrDefault(x => x.Id == id);
            if (cycle == null)
                throw new Exception($"Cycle with id {id} not found");

            cycle.NameEng = cycleDTO.NameEng;
            cycle.NameRus = cycleDTO.NameRus;
            cycle.NameKir = cycleDTO.NameKir;
            cycle.Code = cycleDTO.Code;

            _db.Cycles.Update(cycle);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete cycle by id
        /// </summary>
        /// <param name="id">Id of cycle</param>
        public void Delete(int id)
        {
            Cycle cycle = _db.Cycles.FirstOrDefault(x => x.Id == id);
            if (cycle == null)
                throw new Exception($"Cycle with id {id} not found");

            _db.Cycles.Remove(cycle);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
