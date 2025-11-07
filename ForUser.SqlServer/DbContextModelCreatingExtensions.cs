using ForUser.Domains.Commons.Object;
using ForUser.Domains.Commons.ObjectFunc;
using ForUser.Domains.Commons.Role;
using ForUser.Domains.Commons.RoleObject;
using ForUser.Domains.Commons.UserRole;
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
                entity.Property(e => e.CreateId);
                entity.Property(e => e.CreateName).HasMaxLength(20);
                entity.Property(e => e.CreateTime);
                entity.Property(e => e.ModifierId);
                entity.Property(e => e.ModifilerName).HasMaxLength(20);
                entity.Property(e => e.ModifilcationTime);
            });

            builder.Entity<RoleEntity>(entity =>{
                entity.ToTable("T_Role");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.ProPerty);
                entity.Property(e => e.RoleType);
                entity.Property(e => e.CreateOrg);
                entity.Property(e => e.CreateId);
                entity.Property(e => e.CreateName).HasMaxLength(20);
                entity.Property(e => e.CreateTime);
                entity.Property(e => e.ModifierId);
                entity.Property(e => e.ModifilerName).HasMaxLength(20);
                entity.Property(e => e.ModifilcationTime);
            });

            builder.Entity<ObjectEntity>(entity =>
            {
                entity.ToTable("T_Object");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.CreateId);
                entity.Property(e => e.CreateName).HasMaxLength(50);
                entity.Property(e => e.CreateTime);
                entity.Property(e => e.ModifierId);
                entity.Property(e => e.ModifilerName).HasMaxLength(50);
                entity.Property(e => e.ModifilcationTime);
            });

            builder.Entity<ObjectFuncEntity>(entity =>
            {
                entity.ToTable("T_ObjectFunc");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ObjectId);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Method).HasMaxLength(100);
                entity.Property(e => e.CreateId);
                entity.Property(e => e.CreateName).HasMaxLength(50);
                entity.Property(e => e.CreateTime);
                entity.Property(e => e.ModifierId);
                entity.Property(e => e.ModifilerName).HasMaxLength(50);
                entity.Property(e => e.ModifilcationTime);
            });

            builder.Entity<UserRoleEntity>(entity =>
            {
                entity.ToTable("T_UserRole");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId);
                entity.Property(e => e.RoleId);
            });

            builder.Entity<RoleObjectEntity>(entity =>
            {
                entity.ToTable("T_RoleObject");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ObjectId);
                entity.Property(e => e.RoleId);
            });
        }
    }
}
