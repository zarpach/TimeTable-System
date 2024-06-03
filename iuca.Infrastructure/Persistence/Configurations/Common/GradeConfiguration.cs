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
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder) 
        {
            builder.Property(x => x.GradeMark)
                .HasMaxLength(10);

            builder.Property(x => x.NameEng)
                .HasMaxLength(50);

            builder.Property(x => x.NameRus)
                .HasMaxLength(50);

            builder.Property(x => x.NameKir)
                .HasMaxLength(50);
        }
    }
}
