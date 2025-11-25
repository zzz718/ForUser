using ForUser.Domains.Commons.RoleObject;
using ForUser.Domains.Kernels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ForUser.PostgreSQL
{
    public static class PostgreSQLDbContextModelCreatingExtensions
    {
        public static void Configure(this ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Entity<EmbeddingEntity>(entity =>
            {
                entity.ToTable("vector_knowledge_doc_vector");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Doc_Id).HasColumnName("Doc_Id");
                entity.Property(e => e.Doc_Name).HasColumnName("Doc_Name");
                entity.Property(e => e.Doc_Content).HasColumnName("Doc_Content");
                entity.Property(e => e.Embedding).HasColumnType("vector(1024)");
                entity.Ignore(e => e.CreateId);
                entity.Ignore(e => e.CreateName);
                entity.Ignore(e => e.CreateTime);
                entity.Ignore(e => e.ModifierId);
                entity.Ignore(e => e.ModifilerName);
                entity.Ignore(e => e.ModifilcationTime);
            });
        }

        
    }
}
