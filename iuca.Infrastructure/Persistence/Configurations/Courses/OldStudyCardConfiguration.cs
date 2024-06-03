using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class OldStudyCardConfiguration : IEntityTypeConfiguration<OldStudyCard>
    {
        public void Configure(EntityTypeBuilder<OldStudyCard> builder) 
        {

            builder.HasOne(x => x.Semester)
                .WithMany(x => x.OldStudyCards)
                .HasForeignKey(x => x.SemesterId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.OldStudyCards)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.DepartmentGroup)
                .WithMany(x => x.OldStudyCards)
                .HasForeignKey(x => x.DepartmentGroupId);
        }
    }
}
