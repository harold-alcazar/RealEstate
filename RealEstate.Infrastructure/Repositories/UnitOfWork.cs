
using RealEstate.Application.Interfaces;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Context;

namespace RealEstate.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RealEstateDbContext _db;

        public UnitOfWork(RealEstateDbContext db,
            IPropertyRepository properties,
            IGenericRepository<Owner> owners,
            IGenericRepository<PropertyImage> images,
            IGenericRepository<PropertyTrace> traces)
        {
            _db = db;
            Properties = properties;
            Owners = owners;
            PropertyImages = images;
            PropertyTraces = traces;
        }

        public IPropertyRepository Properties { get; }
        public IGenericRepository<Owner> Owners { get; }
        public IGenericRepository<PropertyImage> PropertyImages { get; }
        public IGenericRepository<PropertyTrace> PropertyTraces { get; }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
