using Microsoft.AspNetCore.Http;

namespace Ecommerce.Domain.Interfaces.Services;

public interface IUploadImageService
{
    Task<List<string>> AddImageAsync(IFormFileCollection files);
}
