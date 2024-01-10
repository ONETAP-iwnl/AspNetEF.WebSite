using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

using System.Text.RegularExpressions;
using System;
using Microsoft.AspNetCore.Identity;
using WebSiteCollege.Identity;
using WebSiteCollege.Models;

namespace WebSiteCollege
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasNoKey(); // Сущность без первичного ключа

            modelBuilder.Entity<StudentModel>().HasKey(s => s.StudentID);
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<StudentModel> Students { get; set; }

    }

}