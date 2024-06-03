using System;
using iuca.Domain.Entities.Slots;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Slots
{
    public class SlotConfiguration : IEntityTypeConfiguration<Slot>
    {
        public void Configure(EntityTypeBuilder<Slot> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).HasColumnName("Id");
            builder.Property(p => p.DateCreated).HasColumnName("DateCreated").IsRequired();
            builder.Property(p => p.LastModified).HasColumnName("LastModified");

            builder.HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(fk => fk.DepartmentId);

            // Конфигурация связи "один ко многим" для Group
            builder.HasOne(p => p.Group)
                .WithMany()
                .HasForeignKey(fk => fk.GroupId);

            // Конфигурация связи "один ко многим" для InstructorUser
            builder.HasOne<ApplicationUser>()
                .WithMany(p => p.Slots)
                .HasForeignKey(fk => fk.InstructorUserId);

            // Конфигурация связи "один ко многим" для LessonRoom
            builder.HasOne(p => p.LessonRoom)
                .WithMany()
                .HasForeignKey(fk => fk.LessonRoomId);

            builder.HasOne(p => p.AnnouncementSection)
                .WithMany()
                .HasForeignKey(fk => fk.AnnouncementSectionId);

            builder.HasOne(p => p.LessonPeriod)
                .WithMany()
                .HasForeignKey(p => p.LessonPeriodId)
                .IsRequired();

            builder.HasOne(p => p.Semester)
                .WithMany()
                .HasForeignKey(fk => fk.SemesterId);

            // Ограничение уникальности для LessonPeriod и DayOfWeek
            // builder.HasIndex(i => new { i.LessonPeriod, i.DayOfWeek }).IsUnique();
        }
    }
}

