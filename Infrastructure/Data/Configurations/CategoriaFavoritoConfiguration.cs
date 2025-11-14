using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data.Configurations;

public class CategoriaFavoritoConfiguration : IEntityTypeConfiguration<CategoriaFavorito>
{
    public void Configure(EntityTypeBuilder<CategoriaFavorito> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasOne(x => x.User)
            .WithMany(x => x.CategoriasFavoritos)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Cards)
            .WithOne(x => x.Categoria)
            .HasForeignKey(x => x.CategoriaFavoritoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
