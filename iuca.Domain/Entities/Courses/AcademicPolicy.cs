namespace iuca.Domain.Entities.Courses
{
    public class AcademicPolicy
    {
        public int Id { get; set; }
        public Syllabus Syllabus { get; set; }
        public int SyllabusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
