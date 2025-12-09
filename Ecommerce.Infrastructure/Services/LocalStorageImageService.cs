using Ecommerce.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Services;

public class LocalStorageImageService : IUploadImageService
{
    private readonly ILogger<LocalStorageImageService> _logger;

    public LocalStorageImageService(ILogger<LocalStorageImageService> logger)
    {
        _logger = logger;
    }

    public async Task<List<string>> AddImageAsync(IFormFileCollection files)
    {
        try
        {
            var images = new List<string>();

            // Configuración
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".jfif", ".avif" };
            var maxFileSize = 5 * 1024 * 1024; // 5MB

            // Crear directorio si no existe
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var formFile in files)
            {
                // Validaciones
                if (formFile.Length == 0)
                    continue;

                if (formFile.Length > maxFileSize)
                    throw new InvalidOperationException($"Archivo demasiado grande: {formFile.FileName}");

                var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    throw new InvalidOperationException($"Tipo de archivo no permitido: {extension}");

                // Generar nombre único
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                images.Add(fileName);
            }

            return images;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir la imagen: {Message}", ex.Message);
            throw new InvalidOperationException("Error al subir la imagen", ex);
        }


    }
}
