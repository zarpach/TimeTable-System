namespace iuca.Domain.Entities.Common
{
    public class StudentSemesterGPA
    {
        public string StudentUserId { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        public float GPA { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
