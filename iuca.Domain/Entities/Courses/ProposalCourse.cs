using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class ProposalCourse : AuditableEntity
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public Proposal Proposal { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public float Credits { get; set; }
        public bool IsForAll { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public string Schedule { get; set; }
        public IEnumerable<string> InstructorsJson { get; set; }
        public IEnumerable<int> YearsOfStudyJson { get; set; }
    }
}
