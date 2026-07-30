using Microsoft.EntityFrameworkCore;
using LeaveAttendance.API.Models;

namespace LeaveAttendance.API.Data
{
    public class LeaveTrackDbContext : DbContext
    {
        public LeaveTrackDbContext(DbContextOptions<LeaveTrackDbContext> options)
            : base(options)
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

            // User Configuration
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

            // Employee (Self Reference - Manager)
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.Manager)
                    .WithMany(m => m.DirectReports)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // LeaveRequest Configuration
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
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // LeaveApproval Configuration
            modelBuilder.Entity<LeaveApproval>(entity =>
            {
                entity.HasOne(la => la.LeaveRequest)
                    .WithMany(lr => lr.Approvals)
                    .HasForeignKey(la => la.LeaveRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(la => la.Approver)
                    .WithMany()
                    .HasForeignKey(la => la.ApproverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance Configuration
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

                entity.HasOne(a => a.Employee)
                    .WithMany()
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}