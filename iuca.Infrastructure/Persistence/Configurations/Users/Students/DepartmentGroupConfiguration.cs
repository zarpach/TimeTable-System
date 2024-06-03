using iuca.Domain.Entities.Users.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class DepartmentGroupConfiguration : IEntityTypeConfiguration<DepartmentGroup>
    {
        public void Configure(EntityTypeBuilder<DepartmentGroup> builder) 
        {
            builder.HasOne(x => x.Department)
                .WithMany(x => x.DepartmentGroups)
                .HasForeignKey(x => x.DepartmentId);

            builder.HasData(new DepartmentGroup
            {
                Id = 1,
                DepartmentId = 1,
                OrganizationId = 1,
                Code = "NA"
            });
        }
    }
}
