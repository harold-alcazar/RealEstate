
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.IServices;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService) => _propertyService = propertyService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req, CancellationToken ct)
        {
            var result = await _propertyService.CreateAsync(req, ct);
            return Ok(result.Value);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePropertyRequest req, CancellationToken ct)
        {
            var result = await _propertyService.UpdateAsync(id, req, ct);
            return Ok(result);
        }

        [HttpPatch("{id:int}/price")]
        public async Task<IActionResult> ChangePrice([FromRoute] int id, [FromBody] ChangePriceRequest req, CancellationToken ct)
        {
            var result = await _propertyService.ChangePriceAsync(id, req, ct);
            return Ok(result);
        }

        [HttpPost("{id:int}/images")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> AddImage([FromRoute] int id, IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length == 0) return BadRequest("File is required");

            await using var stream = file.OpenReadStream();
            var result = await _propertyService.AddImageAsync(id, file.FileName, stream, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? address,
                                                [FromQuery] int? yearFrom, [FromQuery] int? yearTo,
                                                [FromQuery] decimal? priceMin, [FromQuery] decimal? priceMax,
                                                [FromQuery] int? ownerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
                                                CancellationToken ct = default)
        {
            var filter = new PropertyFilter(name, address, yearFrom, yearTo, priceMin, priceMax, ownerId, page, pageSize);
            var result = await _propertyService.SearchAsync(filter, ct);
            return Ok(result);
        }
    }
}
