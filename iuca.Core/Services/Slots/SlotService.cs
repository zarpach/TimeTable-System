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
using iuca.Application.DTO.Courses;
using iuca.Domain.Entities.Courses;

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

                            var conflictingSlot = SlotExist(departmentId, groupId, slot.LessonRoomId, lessonPeriodId, dayOfWeek);

                            if (conflictingSlot != null)
                            {
                                string conflictMessage = $"Слот с данными параметрами уже существует: " +
                                    $"Предмет — {conflictingSlot.AnnouncementSection.Announcement.Course.NameEng}; " +
                                    $"День недели — {(enu_SlotDayOfWeek)dayOfWeek}; " +
                                    $"Время занятия — {conflictingSlot.LessonPeriod.Name}; " +
                                    $"Аудитория — {conflictingSlot.LessonRoom.RoomName}.";
                                throw new ExistingSlotException(conflictMessage, nameof(conflictingSlot));
                            }

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

            var slot = _db.Slots
                .FirstOrDefault(x => x.Id == Id);

            if (slot == null)
                throw new KeyNotFoundException($"Slot with ID {Id} not found");

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

        public SlotDTO SlotExist(int departmentId, int groupId, string lessonRoomId, int lessonPeriodId, int dayOfWeek)
        {
            var slots = GetAllSlots();
            return slots.FirstOrDefault(s => s.Department.Id == departmentId &&
                                                 s.Group.Id == groupId &&
                                                 s.DayOfWeek == dayOfWeek &&
                                                 s.LessonPeriod.Id == lessonPeriodId &&
                                                 s.LessonRoom.Id.ToString() == lessonRoomId);
        }

        public SlotDTO GetSlot(Guid Id)
        {
            var slots = GetAllSlots();
            var slot = slots.FirstOrDefault(x => x.Id == Id);
            return slot;
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

