using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.Repositories;
using Xunit;

namespace RealEstate.Tests.Unit.Repositories
{
    public class PropertyRepositoryTests
    {
        private static RealEstateDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<RealEstateDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new RealEstateDbContext(options);
        }

        [Fact]
        public async Task AddAndGetById_ShouldWork()
        {
            var ctx = CreateContext("db_add_get");
            var repo = new PropertyRepository(ctx);

            var owner = new Owner
            {
                Name = "Owner",
                IdOwner = 1,
                Address = "test",
                Birthday = DateTime.Now,
                Photo = "photo.jpg"
            };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();

            var p = new Property
            {
                Name = "Casa",
                Address = "C",
                Price = 10m,
                Year = 2020,
                CodeInternal = 111,
                IdOwner = owner.IdOwner
            };
            await repo.AddAsync(p);
            await ctx.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(p.IdProperty);
            fetched.Should().NotBeNull();
            fetched.Name.Should().Be("Casa");
        }

        [Fact]
        public async Task SearchAsync_ShouldFilterByName()
        {
            var ctx = CreateContext("db_search");
            var repo = new PropertyRepository(ctx);

            var owner = new Owner
            {
                Name = "Owner",
                IdOwner = 1,
                Address = "test",
                Birthday = DateTime.Now,
                Photo = "photo.jpg"
            };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();

            ctx.Properties.Add(new Property { Name = "Casa Roja", Address = "A", Price = 10m, Year = 2020, CodeInternal = 1, IdOwner = owner.IdOwner });
            ctx.Properties.Add(new Property { Name = "Casa Azul", Address = "B", Price = 20m, Year = 2021, CodeInternal = 2, IdOwner = owner.IdOwner });
            await ctx.SaveChangesAsync();

            var (items, total) = await repo.SearchAsync("Roja", null, null, null, null, null, null, 1, 10);
            total.Should().Be(1);
            items[0].Name.Should().Contain("Roja");
        }
    }
}
