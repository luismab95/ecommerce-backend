using Ecommerce.Infrastructure.Mongo.Documents;
using Ecommerce.Infrastructure.Mongo.Mappers;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Domain.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MongoDB.Bson;


namespace Ecommerce.Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IMongoCollection<ShoppingCartDocument> _collection;
    private readonly IDistributedCache _cache;


    public ShoppingCartRepository(IMongoCollection<ShoppingCartDocument> collection, IDistributedCache cache)
    {
        _collection = collection;
        _cache = cache;
    }

    public async Task<ShoppingCart?> GetByIdAsync(int userId)
    {
        var cacheKey = $"shoppingcart:{userId}";

        // 1. Buscar en Redis primero
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedData);
        }

        // 2. Buscar en la base (Mongo)
        var cart = await _collection.Find(sc => sc.UserId == userId).FirstOrDefaultAsync();
        if (cart == null)
            return null;

        var result = ShoppingCartMapper.ToDomain(cart);

        // 3. Guardar resultado en Redis
        var json = JsonSerializer.Serialize(result);

        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // TTL
            });

        return result;
    }

    public async Task<bool> CreateOrUpdateAsync(ShoppingCart shoppingCart, int userId)
    {
        // Buscar carrito existente
        var existing = await _collection.Find(sc => sc.UserId == userId).FirstOrDefaultAsync();
        var doc = ShoppingCartMapper.ToDocument(shoppingCart);

        if (existing == null)
        {
            // Creación
            doc.Id = ObjectId.GenerateNewId();
            doc.UserId = userId;
            doc.CreatedAt = DateTime.UtcNow;
            doc.UpdatedAt = DateTime.UtcNow;

            await _collection.InsertOneAsync(doc);

            return doc.Id != null;
        }
        else
        {
            // Actualización, preservar datos que no deben perderse
            doc.Id = existing.Id;
            doc.UserId = existing.UserId;
            doc.CreatedAt = existing.CreatedAt;
            doc.UpdatedAt = DateTime.UtcNow;

            // Reemplazar documento completo
            var result = await _collection.ReplaceOneAsync(
                sc => sc.UserId == userId,
                doc
            );

            // Limpiar cache
            var cacheKey = $"shoppingcart:{userId}";
            await _cache.RemoveAsync(cacheKey);

            return result.ModifiedCount > 0;
        }
    }


    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(p => p.Id.ToString() == id);
        return result.DeletedCount > 0;
    }
}
