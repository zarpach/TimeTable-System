using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class StudentDebtConfiguration : IEntityTypeConfiguration<StudentDebt>
    {
        public void Configure(EntityTypeBuilder<StudentDebt> builder)
        {
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.StudentDebts)
                .HasForeignKey(x => x.StudentUserId);

            builder.HasOne(x => x.Semester)
                .WithMany(x => x.StudentDebts)
                .HasForeignKey(x => x.SemesterId);

            builder.Property(x => x.Comment)
                .HasMaxLength(150);
        }
    }
}
