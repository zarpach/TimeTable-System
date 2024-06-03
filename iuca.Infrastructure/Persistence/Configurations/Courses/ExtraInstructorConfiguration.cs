using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class ExtraInstructorConfiguration : IEntityTypeConfiguration<ExtraInstructor>
    {
        public void Configure(EntityTypeBuilder<ExtraInstructor> builder) 
        {
            builder.HasKey(x => new { x.AnnouncementSectionId, x.InstructorUserId });

            builder.HasOne(x => x.AnnouncementSection)
                .WithMany(x => x.ExtraInstructors)
                .HasForeignKey(x => x.AnnouncementSectionId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.ExtraInstructors)
                .HasForeignKey(x => x.InstructorUserId);
        }
    }
}
