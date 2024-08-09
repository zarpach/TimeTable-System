using System;
using iuca.Application.DTO.Slots;
using System.Collections.Generic;
using System.Linq;
using iuca.Domain.Entities.Slots;

namespace iuca.Application.Interfaces.Slots
{
    public interface ISlotService
    {
        public IEnumerable<SlotDTO> GetAllSlots();

        public IEnumerable<IGrouping<string, SlotDTO>> GetSlotsForDepartment(int departmentId, int semesterId, int dayOfWeek, int groupId = 0);

        public SlotDTO GetSlotDTO(Guid Id);

        public Slot GetSlot(Guid Id);

        void SwapSlots(string draggedId, string droppedOnId, int newLessonPeriod, int newDepartmentGroupId);

        void Create(SlotDTO slot);

        void Edit(Guid Id, SlotDTO slot);

        void Delete(Guid id);

        void UnDelete(Guid id);

        void Dispose();
    }
}

