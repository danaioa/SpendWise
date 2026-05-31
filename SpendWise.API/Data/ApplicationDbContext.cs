using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpendWise.API.Models;

namespace SpendWise.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<MonthlySaving> MonthlySavings { get; set; }
        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<BudgetPlan> BudgetPlans { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<BudgetPlanCategory> BudgetPlanCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<MonthlySaving>()
            .Property(s => s.Amount)
            .HasPrecision(18, 2);

            base.OnModelCreating(builder);

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Budget>()
                .Property(b => b.Percentage)
                .HasPrecision(5, 2);

            builder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            builder.Entity<RecurringTransaction>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            builder.Entity<ApplicationUser>()
                .Property(u => u.MonthlyIncome)
                .HasPrecision(18, 2);

            builder.Entity<BudgetPlanCategory>()
    .HasKey(bpc => new { bpc.BudgetPlanId, bpc.CategoryId });

            builder.Entity<BudgetPlanCategory>()
                .HasOne(bpc => bpc.BudgetPlan)
                .WithMany(bp => bp.BudgetPlanCategories)
                .HasForeignKey(bpc => bpc.BudgetPlanId);

            builder.Entity<BudgetPlanCategory>()
                .HasOne(bpc => bpc.Category)
                .WithMany(c => c.BudgetPlanCategories)
                .HasForeignKey(bpc => bpc.CategoryId);

            builder.Entity<BudgetPlanCategory>()
                .Property(bpc => bpc.Percentage)
                .HasPrecision(5, 2);

            builder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            builder.Entity<Category>()
             .HasOne(c => c.User)
             .WithMany()
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                    }
    }
}