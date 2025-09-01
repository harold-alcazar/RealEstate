
using Microsoft.Extensions.Configuration;
using RealEstate.Application.Interfaces;

namespace RealEstate.Infrastructure.Storage
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly string _root;

        public LocalImageStorage(IConfiguration configuration)
        {
            _root = configuration["Storage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(_root);
        }

        public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken ct = default)
        {
            var safe = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
            var full = Path.Combine(_root, safe);
            using var fs = File.Create(full);
            await content.CopyToAsync(fs, ct);
            return Path.Combine("uploads", safe).Replace("\\", "/");
        }
    }
}
