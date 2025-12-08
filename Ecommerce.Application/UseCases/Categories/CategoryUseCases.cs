using Ecommerce.Domain.DTOs.Categories;
using Ecommerce.Domain.DTOs.Pagination;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;

namespace Ecommerce.Application.UseCases.Categories;

public class CategoryUseCases
{
    private readonly ICategoryRepository _categoryRepository;


    public CategoryUseCases(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<object> GetCategoryByIdAsync(int categoryId)
    {
        var findCategory = await _categoryRepository.GetByIdAsync(categoryId) ??
            throw new InvalidOperationException("Categoría no encontrada.");

        return Category.ToSafeResponse(findCategory);
    }


    public async Task<object> GetCategoriesAsync(PaginationRequest request)
    {
        var result = await _categoryRepository.GetCategoriesAsync(request);

        var safeCategoryResponse = new List<object>();
        result.Items.ForEach(category =>
        {
            safeCategoryResponse.Add(Category.ToSafeResponse(category));
        });

        return new
        {
            Items = safeCategoryResponse,
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage,
        };
    }

    public async Task<string> AddCategoryAsync(CategoryRequest request)
    {
        var addCategory = Category.Create(request.Name, request.Description);
        await _categoryRepository.AddAsync(addCategory);

        return "Categoría agregada exitosamente.";

    }

    public async Task<string> UpdateCategoryAsync(int userId, CategoryRequest category)
    {
        var findCategory = await _categoryRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Categoría no encontrada.");

        var updateCategory = Category.Update(findCategory, category.Name, category.Description);
        await _categoryRepository.UpdateAsync(updateCategory);

        return "Categoría actualizada exitosamente.";

    }

    public async Task<string> DeleteCategoryAsync(int userId)
    {
        var findCategory = await _categoryRepository.GetByIdAsync(userId) ??
            throw new InvalidOperationException("Categoría no encontrada.");

        var updateCategory = Category.Delete(findCategory);
        await _categoryRepository.UpdateAsync(updateCategory);

        return "Categoría eliminada exitosamente.";
    }
}
