using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMCJ.Domain.Entities.Users;
using System;
using System.Collections.Generic;

namespace SGMCJ.Domain.Configuration.Users
{
    public partial class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD6E62048");

            entity.ToTable("Users", "users");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B973B1BC").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Users__CreatedAt__412EB0B6")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasAnnotation("Relational:DefaultConstraintName", "DF_Users_IsActive");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Users__UpdatedAt__4222D4EF")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__4316F928");

            entity.HasOne(d => d.UserNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Persons");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
    }
}