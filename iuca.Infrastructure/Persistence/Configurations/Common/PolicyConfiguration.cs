using iuca.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.Property(x => x.NameRus)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameEng)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameKir)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.DescriptionRus)
                .HasMaxLength(5000)
                .IsRequired();

            builder.Property(x => x.DescriptionEng)
                .HasMaxLength(5000)
                .IsRequired();

            builder.Property(x => x.DescriptionKir)
                .HasMaxLength(5000)
                .IsRequired();
        }
    }
}
