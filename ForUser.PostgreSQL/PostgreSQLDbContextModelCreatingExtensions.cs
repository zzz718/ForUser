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
                entity.HasKey(e=>e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd(); ;
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
            builder.Entity<MessageEntity>(entity => 
            {
                entity.ToTable("Message_Entity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever() ;
                entity.Property(e => e.Content).HasColumnName("Content");
                entity.Property(e => e.ConversationId).HasColumnName("ConversationId").IsRequired();
                entity.Property(e => e.Timestamp).HasColumnName("Timestamp").HasColumnType("timestamp"); 
                entity.Property(e => e.Sequence).HasColumnName("Sequence");
                entity.Property(e => e.Role).HasColumnName("Role");
                entity.Ignore(e => e.CreateId);
                entity.Ignore(e => e.CreateName);
                entity.Ignore(e => e.CreateTime);
                entity.Ignore(e => e.ModifierId);
                entity.Ignore(e => e.ModifilerName);
                entity.Ignore(e => e.ModifilcationTime);

            });
            builder.Entity<ConversationEntity>(entity=>
            {
                entity.ToTable("Conversation_Entity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.HasMany(e => e.Messages)
                        .WithOne()
                        .HasForeignKey(x=>x.ConversationId) // 影子外键属性
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.NoAction);
                entity.Property(e=>e.CreateTime).HasColumnType("timestamp");
                entity.Property(e => e.Title).HasColumnName("Title");
                entity.Ignore(e => e.ModifierId);
                entity.Ignore(e => e.ModifilerName);
                entity.Ignore(e => e.ModifilcationTime);

            });
        }

        
    }
}
