﻿using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Users.Students
{
    public class ReinstatementExpulsionOrderConfiguration : IEntityTypeConfiguration<ReinstatementExpulsionOrder>
    {
        public void Configure(EntityTypeBuilder<ReinstatementExpulsionOrder> builder)
        {
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.ReinstatementExpulsionOrders)
                .HasForeignKey(x => x.StudentUserId);

            builder.Property(x => x.StudentUserId)
                .IsRequired();

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.ReinstatementExpulsionOrders)
                .HasForeignKey(x => x.OrganizationId);

            builder.Property(x => x.OrganizationId)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(255)
                .IsRequired(false);
        }
    }
}
