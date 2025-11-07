using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login.Dtos
{
    public class RefreshTokenRecord
    {
        public long UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
