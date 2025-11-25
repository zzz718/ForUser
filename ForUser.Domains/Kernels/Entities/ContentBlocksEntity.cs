using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class ContentBlocksEntity : Entity
    {
        public ContentBlocksEntity()
        {

        }
        public ContentBlocksEntity(long Id) : base(Id)
        {

        }

        public long DocumentId { get; set; }

        public string ContentInfo { get; set; }

        public int PositionInfo { get; set; }

        public string BlockType { get; set; }

        public Vector Embedding { get; set; }
    }
}
