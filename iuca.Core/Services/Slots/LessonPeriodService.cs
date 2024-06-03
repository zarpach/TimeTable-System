using System;
using System.Collections.Generic;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Slots;
using iuca.Application.Interfaces.Slots;
using iuca.Domain.Entities.Slots;
using AutoMapper;
using iuca.Infrastructure.Persistence;
using iuca.Application.Models;
using static iuca.Application.Constants.Permissions;
using System.Linq;

namespace iuca.Application.Services.Slots
{
    public class LessonPeriodService : ILessonPeriodService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public LessonPeriodService(
            IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void Create(LessonPeriodDTO lessonPeriodDTO)
        {
            if (lessonPeriodDTO == null)
                throw new Exception($"lessonPeriodDTO is null");

            var newLessonPeriod = _mapper.Map<LessonPeriod>(lessonPeriodDTO);

            _db.LessonPeriods.Add(newLessonPeriod);
            _db.SaveChanges();
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Edit(LessonPeriodDTO lessonPeriodDTO, int Id)
        {
            throw new NotImplementedException();
        }

        public LessonPeriodDTO GetLessonPeriod(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LessonPeriodDTO> GetLessonPeriods()
        {
            var lessonPeriods = _db.LessonPeriods.OrderBy(p => p.TimeBegin);
            var result = _mapper.Map<IEnumerable<LessonPeriodDTO>>(lessonPeriods.AsEnumerable());

            return result;
        }

        public void UnDelete(int Id)
        {
            throw new NotImplementedException();
        }
    }
}

