using System;
namespace iuca.Domain.Entities.Slots
{
    public class LessonRoom
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public int? RoomCapacity { get; set; }
        public string? Floor { get; set; }
    }
}

