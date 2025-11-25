using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class BlockImagesEntity : Entity
    {
        public BlockImagesEntity()
        {

        }
        public BlockImagesEntity(long Id) : base(Id)
        {

        }
        public long BlockId { get; set; }
        public long ImageId { get; set; }

    }
}
