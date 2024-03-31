using Microsoft.EntityFrameworkCore;

class ClothesDb : DbContext
{
    public ClothesDb(DbContextOptions<ClothesDb> options)
        : base(options) { }

    public DbSet<ClothingArticle> Clothes => Set<ClothingArticle>();
}