using iuca.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iuca.Infrastructure.Persistence.Configurations.Common
{
    public class EnvarSettingConfiguration : IEntityTypeConfiguration<EnvarSetting>
    {
        public void Configure(EntityTypeBuilder<EnvarSetting> builder) 
        {
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.EnvarSettings)
                .HasForeignKey(x => x.OrganizationId);
        }
    }
}
