using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class CourseRequirementConfiguration : IEntityTypeConfiguration<CourseRequirement>
    {
        public void Configure(EntityTypeBuilder<CourseRequirement> builder)
        {
            builder.HasOne(x => x.Syllabus)
                .WithMany(x => x.CourseRequirements)
                .HasForeignKey(x => x.SyllabusId);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Points)
                .IsRequired();
        }
    }
}
