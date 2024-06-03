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
    public class StudentLanguageConfiguration : IEntityTypeConfiguration<StudentLanguage>
    {
        public void Configure(EntityTypeBuilder<StudentLanguage> builder) 
        {
            builder.HasKey(x => new { x.StudentBasicInfoId, x.LanguageId });

            builder.HasOne(x => x.StudentBasicInfo)
                .WithMany(x => x.StudentLanguages)
                .HasForeignKey(x => x.StudentBasicInfoId);

            builder.HasOne(x => x.Language)
                .WithMany(x => x.StudentLanguages)
                .HasForeignKey(x => x.LanguageId);
        }
    }
}
