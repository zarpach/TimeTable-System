using System;
using AutoMapper;
using iuca.Application.DTO.Slots;
using iuca.Application.Interfaces.Slots;
using System.Collections.Generic;
using iuca.Infrastructure.Persistence;
using System.Linq;
using iuca.Domain.Entities.Slots;
using Microsoft.EntityFrameworkCore;
using iuca.Application.Exceptions;
using iuca.Application.Enums;

namespace iuca.Application.Services.Slots
{
    public class SlotService : ISlotService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public SlotService(
            IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void Create(SlotDTO slot)
        {
            if (slot == null)
            {
                throw new ModelValidationException("Slot is null", nameof(slot));
            }

            if (slot.DepartmentIds == null || slot.DepartmentIds.Length == 0)
            {
                throw new ModelValidationException("Department IDs cannot be null or empty", nameof(slot.DepartmentIds));
            }

            if (slot.GroupIds == null || slot.GroupIds.Length == 0)
            {
                throw new ModelValidationException("Group IDs cannot be null or empty", nameof(slot.GroupIds));
            }

            foreach (var departmentId in slot.DepartmentIds)
            {
                foreach (var groupId in slot.GroupIds)
                {
                    foreach (var dayOfWeek in slot.SlotDaysOfWeek)
                    {
                        foreach (var lessonPeriodId in slot.LessonPeriodIds)
                        {

                            SlotExists(departmentId, groupId, slot.LessonRoomId, lessonPeriodId, dayOfWeek, slot.SemesterId);

                            Slot newSlot = _mapper.Map<Slot>(slot);
                            newSlot.DepartmentId = departmentId;
                            newSlot.GroupId = groupId;
                            newSlot.DayOfWeek = dayOfWeek;
                            newSlot.LessonPeriodId = lessonPeriodId;

                            _db.Slots.Add(newSlot);
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }


        public void Delete(Guid Id)
        {
            Slot slot = _db.Slots.FirstOrDefault(x => x.Id == Id);
            if (slot == null)
                throw new Exception($"Slot with id {Id} not found");

            _db.Slots.Remove(slot);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Edit(Guid Id, SlotDTO slotDTO)
        {
            if (slotDTO == null)
                throw new ArgumentException("SlotDTO cannot be null", nameof(slotDTO));

            Slot slot = _db.Slots
                .FirstOrDefault(x => x.Id == Id) ?? throw new KeyNotFoundException($"Slot with ID {Id} not found");
            slot.DepartmentId = slotDTO.DepartmentIds[0];
            slot.GroupId = slotDTO.GroupIds[0];
            slot.SemesterId = slotDTO.SemesterId;
            slot.InstructorUserId = slotDTO.InstructorUserId;
            slot.DayOfWeek = slotDTO.SlotDaysOfWeek[0];
            slot.LessonPeriodId = slotDTO.LessonPeriodIds[0];
            slot.LessonRoomId = Guid.Parse(slotDTO.LessonRoomId);
            slot.AnnouncementSectionId = slotDTO.AnnouncementSectionId;

            
            _db.Slots.Update(slot);
            _db.SaveChanges();
        }

        public IEnumerable<SlotDTO> GetAllSlots()
        {
            IQueryable<Slot> slots = _db.Slots
                .Include(x => x.Department)
                .Include(x => x.Group)
                .Include(x => x.LessonPeriod)
                .Include(x => x.LessonRoom)
                .Include(x => x.Semester) 
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Announcement)
                .ThenInclude(x => x.Course).AsQueryable();

            return _mapper.Map<IEnumerable<SlotDTO>>(slots.AsEnumerable());
        }

        private void SlotExists(int departmentId, int groupId, string lessonRoomId, int lessonPeriodId, int dayOfWeek, int semesterId, Guid Id = default)
        {
            var slotsForSemester = GetSlotsForSemester(semesterId, Id);

            var slot = FindSlotByRoomAndDay(slotsForSemester, lessonRoomId, dayOfWeek, lessonPeriodId);
            if (slot != null)
            {
                string errorMessage = ExistingSlotErrorMessage(slot);
                throw new ExistingSlotException(errorMessage, nameof(slot));                
            }

            slot = FindSlotByDetails(slotsForSemester, departmentId, groupId, lessonRoomId, lessonPeriodId, dayOfWeek);
            if (slot != null)
            {
                string errorMessage = ExistingSlotErrorMessage(slot);
                throw new ExistingSlotException(errorMessage, nameof(slot));
            }
        }

        private static string ExistingSlotErrorMessage(SlotDTO conflictingSlot)
        {
            string errorMessage = $"Слот с данными параметрами уже существует: " +
                                    $"Предмет — {conflictingSlot.AnnouncementSection.Announcement.Course.NameEng}; " +
                                    $"День недели — {(enu_SlotDayOfWeek)conflictingSlot.DayOfWeek}; " +
                                    $"Время занятия — {conflictingSlot.LessonPeriod.Name}; " +
                                    $"Аудитория — {conflictingSlot.LessonRoom.RoomName}.";

            return errorMessage;
        }

        private IEnumerable<SlotDTO> GetSlotsForSemester(int semesterId, Guid Id)
        {
            return GetAllSlots().Where(slot => slot.SemesterId == semesterId && slot.Id != Id);
        }

        private static SlotDTO FindSlotByRoomAndDay(IEnumerable<SlotDTO> slots, string lessonRoomId, int dayOfWeek, int lessonPeriodId)
        {
            return slots.FirstOrDefault(slot =>
            slot.LessonRoomId == lessonRoomId &&
            slot.DayOfWeek == dayOfWeek &&
            slot.LessonPeriod.Id == lessonPeriodId);
        }

        private static SlotDTO FindSlotByDetails(IEnumerable<SlotDTO> slots, int departmentId, int groupId, string lessonRoomId, int lessonPeriodId, int dayOfWeek)
        {
            return slots.FirstOrDefault(slot =>
                slot.Department.Id == departmentId &&
                slot.Group.Id == groupId &&
                slot.DayOfWeek == dayOfWeek &&
                slot.LessonPeriod.Id == lessonPeriodId &&
                slot.LessonRoom.Id.ToString() == lessonRoomId
            );
        }

        public SlotDTO GetSlotDTO(Guid Id)
        {
            IEnumerable<SlotDTO> slots = GetAllSlots();
            SlotDTO slot = slots.FirstOrDefault(x => x.Id == Id);
            return slot;
        }

        public Slot GetSlot(Guid Id)
        {
            Slot slot = _db.Slots
                .FirstOrDefault(x => x.Id == Id) ?? throw new KeyNotFoundException($"Slot with ID {Id} not found");
            return slot;
        }

        public void SwapSlots(string draggedId, string droppedOnId, int newLessonPeriod, int newDepartmentGroupId)
        {
            SlotDTO draggedSlot = GetSlotDTO(Guid.Parse(draggedId));
            SlotDTO droppedOnSlot;
            if (Guid.TryParse(droppedOnId, out var parsedGuid))
            {
                droppedOnSlot = GetSlotDTO(parsedGuid);
                SwapExistingSlots(draggedSlot, droppedOnSlot);
                return;
            }
            else
            {
                Slot slot = GetSlot(draggedSlot.Id);
                slot.LessonPeriodId = newLessonPeriod;
                slot.GroupId = newDepartmentGroupId;
                _db.SaveChanges();
            }
        }

        private void SwapExistingSlots(SlotDTO draggedSlotDTO, SlotDTO droppedOnSlotDTO)
        {
            Slot draggedSlot = GetSlot(draggedSlotDTO.Id);
            Slot droppedOnSlot = GetSlot(droppedOnSlotDTO.Id);

            (droppedOnSlot.GroupId, draggedSlot.GroupId) = (draggedSlot.GroupId, droppedOnSlot.GroupId);
            (droppedOnSlot.LessonPeriodId, draggedSlot.LessonPeriodId) = (draggedSlot.LessonPeriodId, droppedOnSlot.LessonPeriodId);
            //(droppedOnSlot.AnnouncementSectionId, draggedSlot.AnnouncementSectionId) = (draggedSlot.AnnouncementSectionId, droppedOnSlot.AnnouncementSectionId);
            //(droppedOnSlot.LessonRoomId, draggedSlot.LessonRoomId) = (draggedSlot.LessonRoomId, droppedOnSlot.LessonRoomId);
            //(droppedOnSlot.InstructorUserId, draggedSlot.InstructorUserId) = (draggedSlot.InstructorUserId, droppedOnSlot.InstructorUserId);

            SlotExists(draggedSlot.Department.Id,
                draggedSlot.Group.Id,
                draggedSlot.LessonRoom.Id.ToString(),
                draggedSlot.LessonPeriod.Id,
                draggedSlot.DayOfWeek,
                draggedSlot.Semester.Id,
                draggedSlot.Id);

            SlotExists(droppedOnSlot.Department.Id,
                droppedOnSlot.Group.Id,
                droppedOnSlot.LessonRoom.Id.ToString(),
                droppedOnSlot.LessonPeriod.Id,
                droppedOnSlot.DayOfWeek,
                droppedOnSlot.Semester.Id,
                droppedOnSlot.Id);

            _db.SaveChanges();
        }

        public IEnumerable<IGrouping<string, SlotDTO>> GetSlotsForDepartment(int departmentId, int semesterId, int dayOfWeek, int groupId = 0)
        {
            IEnumerable<SlotDTO> slots = GetAllSlots();
            slots = slots
                .Where(x => x.Department.Id == departmentId)
                .Where(x => x.SemesterId == semesterId)
                .Where(x => x.DayOfWeek == dayOfWeek);

            if (groupId != 0)
            {
                slots = slots.Where(x => x.Group.Id == groupId);
            }

            List<IGrouping<string, SlotDTO>> groupedSlots = slots
                .OrderBy(x => x.LessonPeriod.TimeBegin)
                .ThenBy(x => x.Group.Code)
                .GroupBy(x => x.LessonPeriod.Name)
                .ToList();

            return groupedSlots;
        }

        public void UnDelete(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}

