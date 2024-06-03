namespace iuca.Domain.Entities.Courses
{
    public class StudentMidterm
    {
        public int Id { get; set; }
        public int StudentCourseId { get; set; }
        public StudentCourseTemp StudentCourse { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public bool Attention { get; set; }
        public string Comment { get; set; }
        public string Recommendation { get; set; }
        public string AdviserComment { get; set; }
    }
}
