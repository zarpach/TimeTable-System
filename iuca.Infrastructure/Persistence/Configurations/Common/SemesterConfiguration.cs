using iuca.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder) 
        {
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.Semesters)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
