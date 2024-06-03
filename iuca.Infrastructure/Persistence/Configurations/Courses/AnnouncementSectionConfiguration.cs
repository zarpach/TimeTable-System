using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Courses
{
    public class AnnouncementSectionConfiguration: IEntityTypeConfiguration<AnnouncementSection>
    {
        public void Configure(EntityTypeBuilder<AnnouncementSection> builder) 
        {
            builder.HasOne(x => x.Announcement)
                .WithMany(x => x.AnnouncementSections)
                .HasForeignKey(x => x.AnnouncementId);

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.AnnouncementSections)
                .HasForeignKey(x => x.OrganizationId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.AnnouncementSections)
                .HasForeignKey(x => x.CourseId);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.AnnouncementSections)
                .HasForeignKey(x => x.InstructorUserId);

            builder.HasOne(x => x.Syllabus)
                .WithOne(x => x.AnnouncementSection)
                .HasForeignKey<Syllabus>(x => x.AnnouncementSectionId);

            builder.Property(x => x.Section)
                .HasMaxLength(50);

            builder.Property(x => x.Schedule)
                .HasMaxLength(100);

            builder.Property(x => x.ExtraInstructorsJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.GroupsJson)
                .HasColumnType("jsonb");
        }
    }
}
