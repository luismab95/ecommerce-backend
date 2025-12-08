namespace Ecommerce.Domain.DTOs.General;

public class GeneralResponse
{
    public dynamic? Data { get; set; } = null;
    public string Message { get; set; } = string.Empty;
}
