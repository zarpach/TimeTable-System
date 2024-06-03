using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Identity.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.LastNameEng)
                .HasMaxLength(50);
            builder.Property(x => x.FirstNameEng)
                .HasMaxLength(50);
            builder.Property(x => x.MiddleNameEng)
                .HasMaxLength(50)
                .IsRequired(false);
        }
    }
}
