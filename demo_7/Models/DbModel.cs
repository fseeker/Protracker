using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using demo_7.Models;

namespace demo_7.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }

    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public string Version { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee ProjectHead { get; set; }
        public List<ProjectEmployeeRelation> InvolvedEmployees { get; set; }

        public List<UpdateProject> Updates { get; set; }

        public List<Tasks> Tasks { get; set; }
    }

    public class UpdateProject
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PushDate { get; set; }
        public string Description { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

    }

    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Designation { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Department { get; set; }

        public int PermissionLevel { get; set; }

        public List<ProjectEmployeeRelation> InvolvedProjects { get; set; }
        public List<TaskEmployeeRelation> InvolvedTasks { get; set; }
    }

    public class Tasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        public Employee CreatedBy { get; set; }
        public List<TaskEmployeeRelation> InvolvedEmployees { get; set; }
    }
    public class ProjectEmployeeRelation
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public EmployeeRoles Role { get; set; }
    }

    public class EmployeeRoles
    {
        [Key]
        public int RoleId { get; set; }
        public string Role { get; set; }
        public bool IsProjectRole { get; set; }
        public bool IsTaskRole { get; set; }
    }

    public class TaskEmployeeRelation
    {
        public int TaskId { get; set; }
        public Tasks Task { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public EmployeeRoles Role { get; set; }
    }


    public class ProTrackerDbContext : DbContext
    {
        public ProTrackerDbContext(DbContextOptions<ProTrackerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UpdateProject>().HasKey(up => up.Id); // this line, and [Key] attribute is the same thing


            modelBuilder.Entity<ProjectEmployeeRelation>().HasKey(pe => new { pe.EmployeeId, pe.ProjectId }); //Also known as Composite Primary Key.

            modelBuilder.Entity<ProjectEmployeeRelation>()
                .HasOne<Project>(sc => sc.Project)
                .WithMany(s => s.InvolvedEmployees)
                .HasForeignKey(sc => sc.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ProjectEmployeeRelation>()
                .HasOne<Employee>(sc => sc.Employee)
                .WithMany(s => s.InvolvedProjects)
                .HasForeignKey(sc => sc.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskEmployeeRelation>().HasKey(te => new { te.EmployeeId, te.TaskId });
            modelBuilder.Entity<TaskEmployeeRelation>()
                .HasOne<Tasks>(t => t.Task)
                .WithMany(e => e.InvolvedEmployees)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TaskEmployeeRelation>()
                .HasOne<Employee>(e => e.Employee)
                .WithMany(e => e.InvolvedTasks)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UpdateProject> UpdateProjects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProjectEmployeeRelation> ProjectEmployeeRelations { get; set; }
        public DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        public DbSet<TaskEmployeeRelation> TaskEmployeeRelations { get; set; }
        public DbSet<demo_7.Models.Tasks> Tasks { get; set; }

    }
}
