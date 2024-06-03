using System;
namespace iuca.Domain.Entities.Slots
{
    public class LessonPeriod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan TimeBegin { get; set; }
        public TimeSpan TimeEnd { get; set; }
    }
}

