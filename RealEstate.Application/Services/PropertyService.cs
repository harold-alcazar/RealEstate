using RealEstate.Application.DTOs;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces;
using RealEstate.Application.IServices;
using RealEstate.Application.Results;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public class PropertyService: IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorage _storage;

        public PropertyService(IUnitOfWork uow, IImageStorage storage)
        {
            _unitOfWork = uow;
            _storage = storage;
        }

        public async Task<Result<int>> CreateAsync(CreatePropertyRequest req, CancellationToken ct)
        {
            var owner = await _unitOfWork.Owners.GetByIdAsync(req.IdOwner, ct);
            _ = owner ?? throw new NotFoundException("Owner not found");

            var entity = new Property
            {
                Name = req.Name,
                Address = req.Address,
                Price = req.Price,
                CodeInternal = req.CodeInternal,
                Year = req.Year,
                IdOwner = req.IdOwner
            };

            await _unitOfWork.Properties.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<int>.Ok(entity.IdProperty);
        }

        public async Task<Result> UpdateAsync(int idProperty, UpdatePropertyRequest req, CancellationToken ct)
        {
            var entity = await _unitOfWork.Properties.GetByIdAsync(idProperty, ct);
            _ = entity ?? throw new NotFoundException("Property not found");

            var owner = await _unitOfWork.Owners.GetByIdAsync(req.IdOwner, ct);
            _ = owner ?? throw new NotFoundException("Owner not found");

            entity.Name = req.Name;
            entity.Address = req.Address;
            entity.Year = req.Year;
            entity.IdOwner = req.IdOwner;

            _unitOfWork.Properties.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> ChangePriceAsync(int idProperty, ChangePriceRequest req, CancellationToken ct)
        {
            var entity = await _unitOfWork.Properties.GetByIdAsync(idProperty, ct);
            _ = entity ?? throw new NotFoundException("Property not found");

            var trace = new PropertyTrace
            {
                IdProperty = idProperty,
                Value = entity.Price,
                Tax = req.Tax,
                DateSale = DateTime.UtcNow,
                Name = entity.Name
            };
            await _unitOfWork.PropertyTraces.AddAsync(trace, ct);

            entity.Price = req.NewPrice;
            _unitOfWork.Properties.Update(entity);

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<string>> AddImageAsync(int idProperty, string fileName, Stream content, CancellationToken ct)
        {
            var entity = await _unitOfWork.Properties.GetByIdAsync(idProperty, ct);
            _ = entity ?? throw new NotFoundException("Property not found");

            var path = await _storage.SaveAsync(fileName, content, ct);

            var img = new PropertyImage
            {
                IdProperty = idProperty,
                File = path,
                Enabled = true
            };
            await _unitOfWork.PropertyImages.AddAsync(img, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(path);
        }

        public async Task<Result<PagedResult<PropertyResponse>>> SearchAsync(PropertyFilter filter, CancellationToken ct)
        {
            var (items, total) = await _unitOfWork.Properties.SearchAsync(
                filter.Name, filter.Address, filter.YearFrom, filter.YearTo,
                filter.PriceMin, filter.PriceMax, filter.OwnerId,
                filter.Page, filter.PageSize, ct);

            var dtos = items.Select(p => new PropertyResponse(
                p.IdProperty, p.Name, p.Address, p.Price, p.Year,
                p.IdOwner, p.Owner?.Name ?? "",
                p.PropertyImages.Where(i => i.Enabled).Select(i => i.File).ToList()
            )).ToList();

            return Result<PagedResult<PropertyResponse>>.Ok(new PagedResult<PropertyResponse>(dtos, total, filter.Page, filter.PageSize));
        }
    }
}
