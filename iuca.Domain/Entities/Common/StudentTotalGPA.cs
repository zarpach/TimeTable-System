namespace iuca.Domain.Entities.Common
{
    public class StudentTotalGPA
    {
        public string StudentUserId { get; set; }
        public float TotalGPA { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
