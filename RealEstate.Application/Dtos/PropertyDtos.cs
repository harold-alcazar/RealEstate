
namespace RealEstate.Application.DTOs
{
    public record CreatePropertyRequest(
        string Name, string Address, decimal Price, int CodeInternal, int Year, int IdOwner);

    public record UpdatePropertyRequest(
        string Name, string Address, int Year, int IdOwner);

    public record ChangePriceRequest(decimal NewPrice, decimal Tax = 0m);

    public record AddImageRequest(int IdProperty, string FileName);

    public record PropertyResponse(
        int IdProperty, string Name, string Address, decimal Price, int Year, int IdOwner, string OwnerName,
        IReadOnlyList<string> Images);

    public record PropertyFilter(string? Name, string? Address, int? YearFrom, int? YearTo,
                                 decimal? PriceMin, decimal? PriceMax, int? OwnerId,
                                 int Page = 1, int PageSize = 20);
}
