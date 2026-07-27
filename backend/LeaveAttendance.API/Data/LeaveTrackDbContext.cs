using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Models;
using System;

namespace LeaveAttendance.API.Data
{
    public class LeaveTrackDbContext : DbContext
    {
        public LeaveTrackDbContext(DbContextOptions<LeaveTrackDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<LeaveApproval> LeaveApprovals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                
                entity.HasOne(u => u.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Employee)
                    .WithOne()
                    .HasForeignKey<User>(u => u.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
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

                modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedBy)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<LeaveApproval>()
                .HasOne(la => la.LeaveRequest)
                .WithMany(lr => lr.Approvals)
                .HasForeignKey(la => la.LeaveRequestId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<LeaveApproval>()
                .HasOne(la => la.Approver)
                .WithMany()
                .HasForeignKey(la => la.ApproverId)
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
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Employee" },
                new Role { Id = 4, Name = "HR" }
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

            // 3. Users (Password hashing will run when creating migration/database update)
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    RoleId = 1, // Admin
                    EmployeeId = null, 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") 
                },
                new User 
                { 
                    Id = 2, 
                    Username = "manager1", 
                    RoleId = 2, // Manager
                    EmployeeId = 1, 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123") 
                },
                new User 
                { 
                    Id = 3, 
                    Username = "manager2", 
                    RoleId = 2, // Manager
                    EmployeeId = 2, 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123") 
                },
                new User 
                { 
                    Id = 4, 
                    Username = "employee1", 
                    RoleId = 3, // Employee
                    EmployeeId = 3, 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123") 
                },
                new User 
                { 
                    Id = 5, 
                    Username = "employee2", 
                    RoleId = 3, // Employee
                    EmployeeId = 4, 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123") 
                }
            );

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
