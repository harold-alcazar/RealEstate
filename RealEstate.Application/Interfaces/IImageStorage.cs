
namespace RealEstate.Application.Interfaces
{
    public interface IImageStorage
    {
        Task<string> SaveAsync(string fileName, Stream content, CancellationToken ct = default);
    }
}
