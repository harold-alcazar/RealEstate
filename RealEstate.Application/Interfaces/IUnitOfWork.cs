
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IPropertyRepository Properties { get; }
        IGenericRepository<Owner> Owners { get; }
        IGenericRepository<PropertyImage> PropertyImages { get; }
        IGenericRepository<PropertyTrace> PropertyTraces { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
