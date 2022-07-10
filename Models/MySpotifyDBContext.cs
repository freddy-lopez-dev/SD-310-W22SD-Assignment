using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SD_310_W22SD_Assignment.Models
{
    public partial class MySpotifyDBContext : DbContext
    {
        public MySpotifyDBContext()
        {
        }

        public MySpotifyDBContext(DbContextOptions<MySpotifyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Artist> Artists { get; set; } = null!;
        public virtual DbSet<Collection> Collections { get; set; } = null!;
        public virtual DbSet<Music> Musics { get; set; } = null!;
        public virtual DbSet<Song> Songs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=MySpotifyDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasOne(d => d.Music)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.MusicId)
                    .HasConstraintName("FK_Collections_Music");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Collections_Users");
            });

            modelBuilder.Entity<Music>(entity =>
            {
                entity.ToTable("Music");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Musics)
                    .HasForeignKey(d => d.ArtistId)
                    .HasConstraintName("FK_Music_Artists");

                entity.HasOne(d => d.Song)
                    .WithMany(p => p.Musics)
                    .HasForeignKey(d => d.SongId)
                    .HasConstraintName("FK_Music_Songs");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
