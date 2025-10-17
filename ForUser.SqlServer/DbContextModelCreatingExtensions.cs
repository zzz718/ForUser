using ForUser.Domains.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer
{
    public static class DbContextModelCreatingExtensions
    {
        public static void Configure(this ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("T_User");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.PasswordHash).HasMaxLength(50);
                entity.Property(e => e.CreateName).HasMaxLength(20);
                entity.Property(e => e.ModifierName).HasMaxLength(20);
            });
        }
    }
}
