using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data.Configurations;

public class CategoriaHomeConfiguration : IEntityTypeConfiguration<CategoriaHome>
{
    public void Configure(EntityTypeBuilder<CategoriaHome> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasMany(x => x.Cards)
            .WithOne(x => x.Categoria)
            .HasForeignKey(x => x.CategoriaHomeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
