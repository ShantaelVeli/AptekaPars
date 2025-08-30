using DataBase.ModBuilder;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Contexts
{
    public class Category
    {
        public int Id { get; set; }
        public string? NameCategory { get; set; }

        public string? UrlCategory { get; set; }

        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }

        public List<Category> ChildCategories { get; set; } = new();

        public List<Product> Products { get; set; } = new();
    }

    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public string? ImageUrl { get; set; }

        public string? ProdUrl { get; set; }

        public string? Availability { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }

    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated(); // Создаст базу, если её ещё нет
        }
        
        public DbSet<Category> Categorys { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=AptekaRU;Username=postgres;Password=468279135");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .AddHasKey()
            .AddForiginKey();
        }
    }

}