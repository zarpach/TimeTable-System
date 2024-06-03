using System;
using System.Collections.Generic;
using iuca.Application.DTO.Slots;

namespace iuca.Application.Interfaces.Slots
{
	public interface ILessonRoomService
	{
		IEnumerable<LessonRoomDTO> GetLessonRooms();

		LessonRoomDTO GetLessonRoom(int Id);

		void Create(LessonRoomDTO lessonRoom);

		void Edit(LessonRoomDTO lessonRoom, int Id);

		void Delete(int Id);

		void UnDelete(int Id);

		void Dispose();
	}
}

