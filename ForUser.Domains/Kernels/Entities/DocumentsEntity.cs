using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class DocumentsEntity : Entity
    {
        public DocumentsEntity()
        {

        }
        public DocumentsEntity(long Id) : base(Id)
        {

        }

        public long Id { get; set; }

        public string FileName { get; set; }

        public DateTime UploadTime { get; set; }

        public int ContentBlockCount { get; set; }

        public int ImageCount { get; set; }
    }
}
