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
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder) 
        {
            builder.Property(x => x.Code)
                .HasMaxLength(50);

            builder.Property(x => x.NameEng)
                .HasMaxLength(100);

            builder.Property(x => x.NameRus)
                .HasMaxLength(100);

            builder.Property(x => x.NameKir)
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.Departments)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasData(new Department
            {
                Id = 1,
                NameEng = "Not assigned",
                NameRus = "Не указано",
                Code = "NA",
                OrganizationId = 1,
                ImportCode = 0
            });
        }
    }
}
