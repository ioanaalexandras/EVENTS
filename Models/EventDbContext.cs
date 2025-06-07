using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RazorPagesEvents.Models;

public partial class EventDbContext : IdentityDbContext<IdentityUser>
{
    public EventDbContext()
    {
    }

    public EventDbContext(DbContextOptions<EventDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventTask> EventTasks { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<EventUser> EventUsers { get; set; }

    public virtual DbSet<Moneda> Moneda { get; set; }

    public virtual DbSet<PhotoGallery> PhotoGalleries { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskCategory> TaskCategories { get; set; }
    public virtual DbSet<EventUserAccess> UserAccesses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("Default");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Events__3214EC0737B14379");

            entity.ToTable("Event");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EndDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.EventTypeId).HasDefaultValue(1);
            entity.Property(e => e.StartDate).HasColumnType("smalldatetime");

            entity.HasOne(d => d.EventType).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_EventType");

            entity.HasOne(d => d.Moneda).WithMany(p => p.Events)
                .HasForeignKey(d => d.MonedaId)
                .HasConstraintName("FK_Event_Moneda");
        });

        modelBuilder.Entity<EventTask>(entity =>
        {
            entity.ToTable("EventTask");

            entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndDateTime).HasColumnType("smalldatetime");
            entity.Property(e => e.StartDateTime).HasColumnType("smalldatetime");

            entity.HasOne(d => d.Event).WithMany(p => p.EventTasks)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTask_Event");

            entity.HasOne(d => d.Status).WithMany(p => p.EventTasks)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_EventTask_Status");

            entity.HasOne(d => d.Task).WithMany(p => p.EventTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTask_Task");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTask_Users");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EventCategories");

            entity.ToTable("EventType");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<EventUser>(entity =>
        {
            entity.ToTable("EventUser");

            entity.HasOne(d => d.Event).WithMany(p => p.EventUsers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_Event");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_AspNetUsers");
        });

        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.Property(e => e.Cod).HasMaxLength(3);
            entity.Property(e => e.Descriere).HasMaxLength(50);
            entity.Property(e => e.Simbol)
                .HasMaxLength(5)
                .IsFixedLength();
        });

        modelBuilder.Entity<PhotoGallery>(entity =>
        {
            entity.ToTable("PhotoGallery");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.IsPublic).HasColumnName("IsPublic");

            entity.HasOne(d => d.EventTask).WithMany(p => p.PhotoGalleries)
                .HasForeignKey(d => d.EventTaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhotoGallery_EventTask");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(d => d.EventType).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_EventType");

            entity.HasOne(d => d.TaskCategory).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_TaskCategory");
        });

        modelBuilder.Entity<TaskCategory>(entity =>
        {
            entity.ToTable("TaskCategory");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
