using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class TransferOrderConfiguration : IEntityTypeConfiguration<GroupTransferOrder>
    {
        public void Configure(EntityTypeBuilder<GroupTransferOrder> builder)
        {
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.GroupTransferOrders)
                .HasForeignKey(x => x.StudentUserId);

            builder.Property(x => x.StudentUserId)
                .IsRequired();

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.GroupTransferOrders)
                .HasForeignKey(x => x.OrganizationId);

            builder.Property(x => x.OrganizationId)
                .IsRequired();

            builder.HasOne(x => x.Semester)
                .WithMany(x => x.GroupTransferOrders)
                .HasForeignKey(x => x.SemesterId);

            builder.Property(x => x.SemesterId)
                .IsRequired();

            builder.HasOne(x => x.SourceGroup)
                .WithMany(x => x.SourseGroupTransferOrders)
                .HasForeignKey(x => x.SourceGroupId);

            builder.Property(x => x.SourceGroupId)
                .IsRequired();

            builder.HasOne(x => x.TargetGroup)
                .WithMany(x => x.TargetGroupTransferOrders)
                .HasForeignKey(x => x.TargetGroupId);

            builder.Property(x => x.TargetGroupId)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(x => x.PreviousAdvisorsJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.FutureAdvisorsJson)
                .HasColumnType("jsonb");
        }
    }
}
