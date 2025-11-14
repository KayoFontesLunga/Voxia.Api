using Microsoft.EntityFrameworkCore;
using Voxia.Api.Domain.Entities;

namespace VoxiasApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CategoriaHome> CategoriasHome { get; set; }
    public DbSet<CardHome> CardsHome { get; set; }
    public DbSet<CategoriaFavorito> CategoriasFavoritos { get; set; }
    public DbSet<CardFavorito> CardsFavoritos { get; set; }
    public DbSet<CardClickStat> CardStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var frutasId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var animaisId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var macaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var bananaId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var cachorroId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        modelBuilder.Entity<CategoriaHome>().HasData(
            new CategoriaHome { Id = frutasId, Nome = "Frutas" },
            new CategoriaHome { Id = animaisId, Nome = "Animais" }
        );

        modelBuilder.Entity<CardHome>().HasData(
            new CardHome
            {
                Id = macaId,
                Nome = "Maçã",
                ImagemUrl = "/assets/system/images/maca.png",
                AudioUrl = "/assets/system/audio/maca.mp3",
                CategoriaHomeId = frutasId
            },
            new CardHome
            {
                Id = bananaId,
                Nome = "Banana",
                ImagemUrl = "/assets/system/images/banana.png",
                AudioUrl = "/assets/system/audio/banana.mp3",
                CategoriaHomeId = frutasId
            },
            new CardHome
            {
                Id = cachorroId,
                Nome = "Cachorro",
                ImagemUrl = "/assets/system/images/cachorro.png",
                AudioUrl = "/assets/system/audio/cachorro.mp3",
                CategoriaHomeId = animaisId
            }
        );
    }
}
