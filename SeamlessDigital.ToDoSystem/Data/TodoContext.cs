using Microsoft.EntityFrameworkCore;
using SeamlessDigital.ToDoSystem.Models;

namespace SeamlessDigital.ToDoSystem.Data
{
    public class TodoContext:DbContext
    {
        public  TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<Todotask>todotasks { get; set; }

        public DbSet<Category> categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todotask>()
                .HasKey(t => t.Id); // Defines primary key

            modelBuilder.Entity<Todotask>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd(); // Ensures auto-increment behavior

      //      modelBuilder.Entity<Todotask>()
      //.HasOne(t => t.Category)  // One TaskItem has one Category
      //.WithMany(c => c.Id)   // One Category has many TaskItems
      //.HasForeignKey(t => t.CategoryId)  // Define foreign key
      //.OnDelete(DeleteBehavior.Cascade); // Optional: cascade delete

      //      base.OnModelCreating(modelBuilder);
        }
    }
}
