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
    public class SemesterPeriodConfiguration: IEntityTypeConfiguration<SemesterPeriod>
    {
        public void Configure(EntityTypeBuilder<SemesterPeriod> builder) 
        {
            builder.HasOne(x => x.Semester)
                .WithMany(x => x.SemesterPeriods)
                .HasForeignKey(x => x.SemesterId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.SemesterPeriods)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
