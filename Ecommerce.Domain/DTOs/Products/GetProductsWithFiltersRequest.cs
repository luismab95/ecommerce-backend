using Ecommerce.Domain.DTOs.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Products;

public class GetProductsWithFiltersRequest : PaginationRequest
{
    [Range(0.01, 100000, ErrorMessage = "El precio máximo debe estar entre 0.01 y 100,000")]
    public decimal? PriceMax { get; set; }

    [Range(0.01, 100000, ErrorMessage = "El precio mínimo debe estar entre 0.01 y 100,000")]
    public decimal? PriceMin { get; set; }

    public int? CategoryId { get; set; }
    public bool? Featured { get; set; }
}
