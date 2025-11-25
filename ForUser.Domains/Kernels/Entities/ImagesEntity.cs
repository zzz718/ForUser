using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class ImagesEntity : Entity
    {
        public ImagesEntity()
        {

        }
        public ImagesEntity(long Id) : base(Id)
        {

        }

        public long Id { get; set; }
        public long DocumentId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] ImageData { get; set; }

        public string Description { get; set; }
    }
}
