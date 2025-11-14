using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data.Configurations;

public class CardHomeConfiguration : IEntityTypeConfiguration<CardHome>
{
    public void Configure(EntityTypeBuilder<CardHome> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ImagemUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.AudioUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Categoria)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.CategoriaHomeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
