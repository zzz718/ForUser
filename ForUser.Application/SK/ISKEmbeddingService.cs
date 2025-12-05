using ForUser.Domains.Attributes;
using ForUser.Domains.Kernels.Entities;
using Microsoft.AspNetCore.Http;
using Pgvector;

namespace ForUser.Application.SK
{
    public interface ISKEmbeddingService
    {
        [UnitOfWork]
        Task MessageEmbeddingAsync(IFormFile file);
        [DisableUnitOfWork]
        Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string text);


    }
}
