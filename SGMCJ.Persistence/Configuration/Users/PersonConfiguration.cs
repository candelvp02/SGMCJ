using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using System;
using System.Collections.Generic;

namespace SGMCJ.Domain.Configuration.Users
{
    public partial class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> entity)
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Persons__AA2FFB85C91EBDF7");

            entity.ToTable("Persons", "users");

            entity.Property(e => e.PersonId)
                .ValueGeneratedNever()
                .HasColumnName("PersonID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IdentificationNumber)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(40)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Person> entity);
    }
}