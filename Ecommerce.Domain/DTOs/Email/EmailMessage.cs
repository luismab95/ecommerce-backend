namespace Ecommerce.Domain.DTOs.Email;

public class EmailMessage
{
    public string From { get; set; } = string.Empty;
    public List<string> To { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}
