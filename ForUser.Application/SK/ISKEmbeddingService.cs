using ForUser.Domains.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK
{
    public interface ISKEmbeddingService
    {
        [UnitOfWork]
        Task MessageEmbeddingAsync(IFormFile file);
    }
}
