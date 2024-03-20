using HelloWorld.Entities;
using Microsoft.EntityFrameworkCore;
namespace HelloWorld.Contexts
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options) { }
 

        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category("Category 1")
                {
                    ID = 1
                },
                new Category("Category 2")
                {
                    ID = 2
                },
                new Category("Category 3")
                {
                    ID = 3
                }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product("Product 1")
                {
                    ID = 1,
                    Description = "Desc of Product 1",
                    CategoryID = 1
                },
                new Product("Product 2")
                {
                    ID = 2,
                    Description = "Desc of Product 2",
                    CategoryID = 1
                },
                new Product("Product 3")
                {
                    ID = 3,
                    Description = "Desc of Product 3",
                    CategoryID = 1
                },
                new Product("Product 4")
                {
                    ID = 4,
                    Description = "Desc of Product 4",
                    CategoryID = 1
                },
                new Product("Product 5")
                {
                    ID = 5,
                    Description = "Desc of Product 5",
                    CategoryID = 2
                },
                new Product("Product 6")
                {
                    ID = 6,
                    Description = "Desc of Product 6",
                    CategoryID = 2
                },
                new Product("Product 7")
                {
                    ID = 7,
                    Description = "Desc of Product 7",
                    CategoryID = 2
                },
                new Product("Product 8")
                {
                    ID = 8,
                    Description = "Desc of Product 8",
                    CategoryID = 2
                },
                new Product("Product 9")
                { 
                    ID = 9,
                    Description = "Desc of Product 9",
                    CategoryID = 2 
                }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
