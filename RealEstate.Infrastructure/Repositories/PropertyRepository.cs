
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Context;

namespace RealEstate.Infrastructure.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        private readonly RealEstateDbContext _db;

        public PropertyRepository(RealEstateDbContext ctx) : base(ctx) => _db = ctx;

        public async Task<(IReadOnlyList<Property> Items, int Total)> SearchAsync(
            string? name, string? address, int? yearFrom, int? yearTo,
            decimal? priceMin, decimal? priceMax, int? ownerId,
            int page, int pageSize, CancellationToken ct = default)
        {
            var q = _db.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name)) q = q.Where(p => EF.Functions.Like(p.Name, $"%{name}%"));
            if (!string.IsNullOrWhiteSpace(address)) q = q.Where(p => EF.Functions.Like(p.Address, $"%{address}%"));
            if (yearFrom.HasValue) q = q.Where(p => p.Year >= yearFrom);
            if (yearTo.HasValue) q = q.Where(p => p.Year <= yearTo);
            if (priceMin.HasValue) q = q.Where(p => p.Price >= priceMin);
            if (priceMax.HasValue) q = q.Where(p => p.Price <= priceMax);
            if (ownerId.HasValue) q = q.Where(p => p.IdOwner == ownerId);

            var total = await q.CountAsync(ct);
            var items = await q.OrderBy(p => p.IdProperty)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);
            return (items, total);
        }
    }
}
