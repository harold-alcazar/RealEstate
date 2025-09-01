using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<(IReadOnlyList<Property> Items, int Total)> SearchAsync(
            string? name, string? address, int? yearFrom, int? yearTo,
            decimal? priceMin, decimal? priceMax, int? ownerId,
            int page, int pageSize, CancellationToken ct = default);
    }
}
