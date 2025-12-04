using Pgvector;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class EmbeddingEntity : Entity
    {
        public EmbeddingEntity()
        {

        }
        public EmbeddingEntity(long Id) : base(Id)
        {

        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Doc_Id { get; set; }
        public string Doc_Name { get; set; }
        public string Doc_Content { get; set; }
        public Vector? Embedding { get; set; }
    }
 }
