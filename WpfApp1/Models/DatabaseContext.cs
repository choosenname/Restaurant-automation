﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models.Database;

namespace WpfApp1.Models
{
    public class DatabaseContext : DbContext
    {
        string connectionString = @"Server=DESKTOP-P1K6VF8;Database=wpfApp;Integrated Security=True;TrustServerCertificate=True";

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishCategory> DishCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DishInOrder> DishInOrders { get; set; }
        public DbSet<CancellationReport> CancellationReports { get; set; }

        public DbSet<Kassa> Kassa { get; set; }
        public DatabaseContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            optionsBuilder.UseSqlServer(builder.ConnectionString);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeType>().HasData(
                new EmployeeType { Id = 1, Name = "Администратор системы" },
                new EmployeeType { Id = 2, Name = "Администратор ресторана" },
                new EmployeeType { Id = 3, Name = "Официант" },
                new EmployeeType { Id = 4, Name = "Менеджер" });
            modelBuilder.Entity<Employee>()
                .HasOne(r => r.EmployeeType)
                .WithMany()
                .HasForeignKey(r => r.TypeId);

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = "100000", Code= "100000", Name = "Систесный администратор", TypeId = 1, StartWork = "09:00", EndWork = "17:00", WorkDays = [] });

            modelBuilder.Entity<Kassa>().HasData(
                new Kassa { Id = 1, Nalichny = 0, Card = 0, Return = 0});
        }
    }
}
