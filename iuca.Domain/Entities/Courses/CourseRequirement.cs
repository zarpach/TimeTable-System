namespace iuca.Domain.Entities.Courses
{
    public class CourseRequirement
    {
        public int Id { get; set; }
        public Syllabus Syllabus { get; set; }
        public int SyllabusId { get; set; }
        public int Name { get; set; }
        public string Description { get; set; }
        public float Points { get; set; }
    }
}
