using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Data.Entities;

public sealed class CineVaultDbContext : DbContext
{
    public required DbSet<Movie> Movies { get; set; }
    public required DbSet<Review> Reviews { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Comment> Comments { get; set; }
    public required DbSet<CommentLike> CommentLikes { get; set; }
    public required DbSet<Actor> Actors { get; set; }

    public CineVaultDbContext(DbContextOptions<CineVaultDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasQueryFilter(m => !m.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Comment>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<CommentLike>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Actor>().HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(m => m.Description)
                .HasMaxLength(10000);
            entity.Property(m => m.Genre)
                .HasMaxLength(100);
            entity.Property(m => m.Director).HasMaxLength(200);
            entity.HasIndex(m => m.Title);
        });
        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(r => r.Rating).IsRequired();
            entity.Property(r => r.Comment).HasMaxLength(5000);
             entity.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
             entity.HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(r => new { r.MovieId, r.UserId }).IsUnique();
        });
        modelBuilder.Entity<User>(entity =>
        {           
            entity.Property(entity => entity.Username)
                .IsRequired()
                .HasMaxLength(150);
                entity.Property(entity => entity.Email)
                .IsRequired()
                .HasMaxLength(200);
                entity.Property(entity => entity.Password)
                .IsRequired()
                .HasMaxLength(500);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });
        
        modelBuilder.Entity<Comment>(entity => {             entity.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(3000);
            entity.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(c => c.Review)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(a => a.Biography)
                .HasMaxLength(5000);
            entity.HasIndex(a => a.FullName);
        });
        modelBuilder.Entity<MovieActor>(entity =>
        {
            entity.HasKey(ma => new { ma.MovieId, ma.ActorId });
            entity.HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CommentLike>(entity => {
            entity.HasKey(ma => new { ma.CommentId, ma.UserId });
            entity.HasOne(cl => cl.User)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}