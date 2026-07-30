using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Models;
using System;

namespace LeaveAttendance.API.Data
{
    public class LeaveTrackDbContext : IdentityDbContext<ApplicationUser>
    {
        public LeaveTrackDbContext(DbContextOptions<LeaveTrackDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee mapping to ApplicationUser
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Employee self-referencing relationship (Manager)
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.Manager)
                    .WithMany(m => m.DirectReports)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // LeaveRequest configurations
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasOne(lr => lr.Employee)
                    .WithMany()
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lr => lr.LeaveType)
                    .WithMany()
                    .HasForeignKey(lr => lr.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lr => lr.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance configurations
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

                entity.HasOne(a => a.Employee)
                    .WithMany()
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seeding data
            // 1. Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Id = "3", Name = "Employee", NormalizedName = "EMPLOYEE" },
                new IdentityRole { Id = "4", Name = "HR", NormalizedName = "HR" }
            );

            // 2. Employees (Seed before Users because User.EmployeeId references Employee)
            modelBuilder.Entity<Employee>().HasData(
                new Employee 
                { 
                    Id = 1, 
                    FullName = "John Doe", 
                    Email = "manager1@leavetrack.com", 
                    Department = "Engineering", 
                    Designation = "Engineering Manager", 
                    ManagerId = null, 
                    DateOfJoining = new DateOnly(2024, 1, 1) 
                },
                new Employee 
                { 
                    Id = 2, 
                    FullName = "Sarah Smith", 
                    Email = "manager2@leavetrack.com", 
                    Department = "Human Resources", 
                    Designation = "HR Manager", 
                    ManagerId = null, 
                    DateOfJoining = new DateOnly(2024, 2, 1) 
                },
                new Employee 
                { 
                    Id = 3, 
                    FullName = "Alice Cooper", 
                    Email = "employee1@leavetrack.com", 
                    Department = "Engineering", 
                    Designation = "Software Engineer", 
                    ManagerId = 1, // John Doe is manager
                    DateOfJoining = new DateOnly(2025, 1, 1) 
                },
                new Employee 
                { 
                    Id = 4, 
                    FullName = "Bob Johnson", 
                    Email = "employee2@leavetrack.com", 
                    Department = "Human Resources", 
                    Designation = "HR Specialist", 
                    ManagerId = 2, // Sarah Smith is manager
                    DateOfJoining = new DateOnly(2025, 2, 1) 
                }
            );

            // 3. Users seeding removed (we will use UserManager for registration)

            // 4. LeaveTypes
            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Casual Leave", DefaultDaysPerYear = 12 },
                new LeaveType { Id = 2, Name = "Sick Leave", DefaultDaysPerYear = 10 },
                new LeaveType { Id = 3, Name = "Annual Leave", DefaultDaysPerYear = 15 }
            );

            // 5. Holidays
            modelBuilder.Entity<Holiday>().HasData(
                new Holiday { Id = 1, Date = new DateOnly(2026, 1, 1), Name = "New Year's Day" },
                new Holiday { Id = 2, Date = new DateOnly(2026, 1, 26), Name = "Republic Day" },
                new Holiday { Id = 3, Date = new DateOnly(2026, 8, 15), Name = "Independence Day" },
                new Holiday { Id = 4, Date = new DateOnly(2026, 12, 25), Name = "Christmas Day" }
            );
        }
    }
}
