using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class StudyCardConfiguration : IEntityTypeConfiguration<StudyCard>
    {
        public void Configure(EntityTypeBuilder<StudyCard> builder) 
        {
            builder.HasOne(x => x.Semester)
                .WithMany(x => x.StudyCards)
                .HasForeignKey(x => x.SemesterId);

            builder.HasOne(x => x.DepartmentGroup)
                .WithMany(x => x.StudyCards)
                .HasForeignKey(x => x.DepartmentGroupId);

            builder.HasIndex(x => new { x.SemesterId, x.DepartmentGroupId })
                .IsUnique();
        }
    }
}
