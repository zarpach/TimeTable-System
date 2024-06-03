using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
    {
        public void Configure(EntityTypeBuilder<Proposal> builder)
        {
            builder.HasOne(x => x.Department)
                .WithMany(x => x.Proposals)
                .HasForeignKey(x => x.DepartmentId);

            builder.HasOne(x => x.Semester)
                .WithMany(x => x.Proposals)
                .HasForeignKey(x => x.SemesterId);
        }
    }
}
