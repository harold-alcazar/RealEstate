using RealEstate.Application.DTOs;
using RealEstate.Application.Results;

namespace RealEstate.Application.IServices
{
    public interface IPropertyService
    {
        Task<Result<int>> CreateAsync(CreatePropertyRequest req, CancellationToken ct);
        Task<Result> UpdateAsync(int idProperty, UpdatePropertyRequest req, CancellationToken ct);
        Task<Result> ChangePriceAsync(int idProperty, ChangePriceRequest req, CancellationToken ct);
        Task<Result<string>> AddImageAsync(int idProperty, string fileName, Stream content, CancellationToken ct);
        Task<Result<PagedResult<PropertyResponse>>> SearchAsync(PropertyFilter filter, CancellationToken ct);
    }
}
