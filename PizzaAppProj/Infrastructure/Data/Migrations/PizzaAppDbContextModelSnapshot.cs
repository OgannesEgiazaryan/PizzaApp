using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PizzaAppProj.Domain.Enums;

namespace PizzaAppProj.Infrastructure.Data.Migrations
{
    [DbContext(typeof(PizzaAppDbContext))]
    public partial class PizzaAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.Order", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("CustomerName")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("character varying(120)");

                b.Property<DateTimeOffset?>("IssuedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTimeOffset>("OrderedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<long>("OrderNumber")
                    .HasColumnType("bigint");

                b.Property<DateTimeOffset>("ReadyAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<OrderStatus>("Status")
                    .HasColumnType("integer")
                    .HasConversion<int>();

                b.Property<decimal>("TotalCost")
                    .HasColumnType("numeric(10,2)");

                b.HasKey("Id");

                b.HasIndex("OrderNumber")
                    .IsUnique();

                b.ToTable("Orders");
            });

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.Pizza", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<int>("CaloriesPer100Grams")
                    .HasColumnType("integer");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnType("character varying(300)");

                b.Property<string>("Ingredients")
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasColumnType("character varying(400)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("character varying(120)");

                b.Property<decimal>("Price")
                    .HasColumnType("numeric(10,2)");

                b.Property<int>("WeightGrams")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("Pizzas");
            });

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.OrderItem", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<int>("OrderId")
                    .HasColumnType("integer");

                b.Property<int>("PizzaId")
                    .HasColumnType("integer");

                b.Property<int>("Quantity")
                    .HasColumnType("integer");

                b.Property<decimal>("UnitPrice")
                    .HasColumnType("numeric(10,2)");

                b.HasKey("Id");

                b.HasIndex("OrderId");

                b.HasIndex("PizzaId");

                b.ToTable("OrderItems");
            });

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.OrderItem", b =>
            {
                b.HasOne("PizzaAppProj.Domain.Entities.Order", "Order")
                    .WithMany("Items")
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("PizzaAppProj.Domain.Entities.Pizza", "Pizza")
                    .WithMany("OrderItems")
                    .HasForeignKey("PizzaId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Order");

                b.Navigation("Pizza");
            });

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.Order", b =>
            {
                b.Navigation("Items");
            });

            modelBuilder.Entity("PizzaAppProj.Domain.Entities.Pizza", b =>
            {
                b.Navigation("OrderItems");
            });
#pragma warning restore 612, 618
        }
    }
}
