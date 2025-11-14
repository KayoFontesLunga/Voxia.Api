using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data.Configurations;

public class CardFavoritoConfiguration : IEntityTypeConfiguration<CardFavorito>
{
    public void Configure(EntityTypeBuilder<CardFavorito> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ImagemPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.AudioPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Categoria)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.CategoriaFavoritoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
