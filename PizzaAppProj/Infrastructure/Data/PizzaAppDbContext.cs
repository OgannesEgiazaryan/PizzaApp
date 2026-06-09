using Microsoft.EntityFrameworkCore;
using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Infrastructure.Data
{
    public sealed class PizzaAppDbContext(DbContextOptions<PizzaAppDbContext> options) : DbContext(options)
    {
        public DbSet<Pizza> Pizzas => Set<Pizza>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>(entity =>
            {
                entity.ToTable("Pizzas");
                entity.HasKey(pizza => pizza.Id);
                entity.Property(pizza => pizza.Name).HasMaxLength(120).IsRequired();
                entity.Property(pizza => pizza.Description).HasMaxLength(300).IsRequired();
                entity.Property(pizza => pizza.Ingredients).HasMaxLength(400).IsRequired();
                entity.Property(pizza => pizza.Price).HasColumnType("numeric(10,2)").IsRequired();
                entity.HasIndex(pizza => pizza.Name).IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(order => order.Id);
                entity.Property(order => order.OrderNumber).IsRequired();
                entity.Property(order => order.CustomerName).HasMaxLength(120).IsRequired();
                entity.Property(order => order.Status).HasConversion<int>().IsRequired();
                entity.Property(order => order.TotalCost).HasColumnType("numeric(10,2)").IsRequired();
                entity.HasIndex(order => order.OrderNumber).IsUnique();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(item => item.Id);
                entity.Property(item => item.UnitPrice).HasColumnType("numeric(10,2)").IsRequired();

                entity.HasOne(item => item.Order)
                    .WithMany(order => order.Items)
                    .HasForeignKey(item => item.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(item => item.Pizza)
                    .WithMany(pizza => pizza.OrderItems)
                    .HasForeignKey(item => item.PizzaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
