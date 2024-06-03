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
    public class CyclePartConfiguration : IEntityTypeConfiguration<CyclePart>
    {
        public void Configure(EntityTypeBuilder<CyclePart> builder) 
        {
            builder.HasOne(x => x.AcademicPlan)
                .WithMany(x => x.CycleParts)
                .HasForeignKey(x => x.AcademicPlanId);

            builder.HasOne(x => x.Cycle)
                .WithMany(x => x.CycleParts)
                .HasForeignKey(x => x.CycleId);
        }
    }
}
