using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.Object
{
    public class ObjectEntity : Entity
    {

        public ObjectEntity()
        {

        }
        public ObjectEntity(long Id):base(Id)
        {
         
        }

        public string Code { get; set; }

        public string Name { get; set; }

    }
}
