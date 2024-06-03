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
    public class AcademicPlanConfiguration : IEntityTypeConfiguration<AcademicPlan>
    {
        public void Configure(EntityTypeBuilder<AcademicPlan> builder) 
        {
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.AcademicPlans)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.AcademicPlans)
                .HasForeignKey(x => x.DepartmentId);
        }
    }
}
