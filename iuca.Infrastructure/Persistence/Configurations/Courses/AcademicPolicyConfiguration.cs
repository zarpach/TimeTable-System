using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class AcademicPolicyConfiguration : IEntityTypeConfiguration<AcademicPolicy>
    {
        public void Configure(EntityTypeBuilder<AcademicPolicy> builder)
        {
            builder.HasOne(x => x.Syllabus)
                .WithMany(x => x.AcademicPolicies)
                .HasForeignKey(x => x.SyllabusId);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(5000)
                .IsRequired();

            builder.HasIndex(x => new { x.SyllabusId, x.Name })
                .IsUnique();
        }
    }
}
