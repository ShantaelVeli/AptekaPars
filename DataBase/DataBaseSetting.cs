using Microsoft.EntityFrameworkCore;
using DataBase.Contexts;

namespace DataBase.ModBuilder
{
    public static class TaskKeyBuilder
    {
        public static ModelBuilder AddHasKey(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasAlternateKey(u => u.UrlCategory);
            modelBuilder.Entity<Product>().HasAlternateKey(u => u.ProdUrl);

            return modelBuilder;
        }

        public static ModelBuilder AddForiginKey(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(с => с.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
            }
            );
            return modelBuilder;
        }
    }
}