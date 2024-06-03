using System;
using System.Collections.Generic;
using iuca.Application.DTO.Slots;

namespace iuca.Application.Interfaces.Slots
{
	public interface ILessonPeriodService
	{
		public IEnumerable<LessonPeriodDTO> GetLessonPeriods();

		public LessonPeriodDTO GetLessonPeriod(int Id);

		void Create(LessonPeriodDTO lessonPeriodDTO);

		void Delete(int Id);

		void UnDelete(int Id);

		void Edit(LessonPeriodDTO lessonPeriodDTO, int Id);

		void Dispose();
	}
}

