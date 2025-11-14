using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data.Configurations;

public class CardClickStatConfiguration : IEntityTypeConfiguration<CardClickStat>
{
    public void Configure(EntityTypeBuilder<CardClickStat> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CardNome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.IsHomeCard)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Stats)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
