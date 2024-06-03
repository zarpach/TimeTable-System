using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using iuca.Application.DTO.Slots;
using iuca.Application.Interfaces.Slots;
using iuca.Domain.Entities.Slots;
using iuca.Infrastructure.Persistence;

namespace iuca.Application.Services.Slots
{
	public class LessonRoomService : ILessonRoomService
	{
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public LessonRoomService(
            IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void Create(LessonRoomDTO lessonRoom)
        {
            if (lessonRoom == null)
                throw new Exception($"lessonPeriodDTO is null");

            var newLessonPeriod = _mapper.Map<LessonRoom>(lessonRoom);

            _db.LessonRooms.Add(newLessonPeriod);
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

        public void Edit(LessonRoomDTO lessonRoom, int Id)
        {
            throw new NotImplementedException();
        }

        public LessonRoomDTO GetLessonRoom(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LessonRoomDTO> GetLessonRooms()
        {
            var lessonRooms = _db.LessonRooms.OrderBy(p => p.RoomName);
            var result = _mapper.Map<IEnumerable<LessonRoomDTO>>(lessonRooms.AsEnumerable());

            return result;
        }

        public void UnDelete(int Id)
        {
            throw new NotImplementedException();
        }
    }
}

