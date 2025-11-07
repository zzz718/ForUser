using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.ObjectFunc
{
    public class ObjectFuncEntity : Entity
    {

        public ObjectFuncEntity()
        {

        }
        public ObjectFuncEntity(long Id) : base(Id)
        {

        }

        public long ObjectId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Method { get; set; }
    }
}
