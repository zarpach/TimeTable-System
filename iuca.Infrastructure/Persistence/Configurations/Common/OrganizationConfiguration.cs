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
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder) 
        {
            builder.Property(x => x.Name)
                .HasMaxLength(50);

            builder.HasData(new Organization { Id = 1, Name = "Университет МУЦА", IsMain = true },
                            new Organization { Id = 2, Name = "Колледж МУЦА", IsMain = false } ); 
        }
    }
}
