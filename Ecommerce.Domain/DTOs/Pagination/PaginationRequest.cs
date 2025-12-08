using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Pagination;

public class PaginationRequest
{
    [Required(ErrorMessage = "El tamaño de la página es requerido")]
    [Range(5, int.MaxValue, ErrorMessage = "El tamaño de página debe ser mayor a 5")]
    public int PageSize { get; set; } = 10;

    [Required(ErrorMessage = "El número de página es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
    public int PageNumber { get; set; } = 1;

    [StringLength(50, MinimumLength = 2, ErrorMessage = "El Término de busqueda debe tener entre 2 y 50 caracteres")]
    public string? SearchTerm { get; set; }

}
