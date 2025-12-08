using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories
{
    public interface IImageRepository
    {
        Task AddAsync(Image image);
        Task UpdateAsync(Image image);
        Task<Image?> GetByIdAsync(int imageId);
    }
}
