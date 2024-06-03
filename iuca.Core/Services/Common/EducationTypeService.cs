using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class EducationTypeService : IEducationTypeService
    {
        private readonly IApplicationDbContext _db;

        public EducationTypeService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get education type list
        /// </summary>
        /// <returns>Education type list</returns>
        public IEnumerable<EducationTypeDTO> GetEducationTypes()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<EducationType, EducationTypeDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<EducationType>, IEnumerable<EducationTypeDTO>>(_db.EducationTypes).OrderBy(x => x.NameEng);
        }

        /// <summary>
        /// Get education type by id
        /// </summary>
        /// <param name="id">Id of education type</param>
        /// <returns>Education type model</returns>
        public EducationTypeDTO GetEducationType(int id)
        {
            EducationType educationType = _db.EducationTypes.FirstOrDefault(x => x.Id == id);
            if (educationType == null)
                throw new Exception($"Education type with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<EducationType, EducationTypeDTO>()).CreateMapper();
            return mapper.Map<EducationType, EducationTypeDTO>(educationType);
        }

        /// <summary>
        /// Create education type
        /// </summary>
        /// <param name="educationTypeDTO">Education type model</param>
        public void Create(EducationTypeDTO educationTypeDTO)
        {
            if (educationTypeDTO == null)
                throw new Exception($"educationTypeDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<EducationType, EducationTypeDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<EducationTypeDTO, EducationType>()).CreateMapper();

            EducationType newEducationType = mapperFromDTO.Map<EducationTypeDTO, EducationType>(educationTypeDTO);

            _db.EducationTypes.Add(newEducationType);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit education type
        /// </summary>
        /// <param name="id">Id of education type</param>
        /// <param name="educationTypeDTO">Education type model</param>
        public void Edit(int id, EducationTypeDTO educationTypeDTO)
        {
            if (educationTypeDTO == null)
                throw new Exception($"educationTypeDTO is null");

            EducationType educationType = _db.EducationTypes.FirstOrDefault(x => x.Id == id);
            if (educationType == null)
                throw new Exception($"Education type with id {id} not found");

            educationType.NameEng = educationTypeDTO.NameEng;
            educationType.NameRus = educationTypeDTO.NameRus;
            educationType.NameKir = educationTypeDTO.NameKir;

            _db.EducationTypes.Update(educationType);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete education type by id
        /// </summary>
        /// <param name="id">Id of education type</param>
        public void Delete(int id)
        {
            EducationType educationType = _db.EducationTypes.FirstOrDefault(x => x.Id == id);
            if (educationType == null)
                throw new Exception($"Education type with id {id} not found");

            _db.EducationTypes.Remove(educationType);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
