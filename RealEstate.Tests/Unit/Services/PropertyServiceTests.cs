using Moq;
using RealEstate.Application.DTOs;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using Xunit;

namespace RealEstate.Tests.Unit.Services
{
    public class PropertyServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IImageStorage> _storageMock;
        private readonly Mock<IGenericRepository<PropertyImage>> _imgRepoMock;
        private readonly Mock<IGenericRepository<Owner>> _ownerRepoMock;
        private readonly Mock<IGenericRepository<PropertyTrace>> _traceRepoMock;

        private readonly PropertyService _service;

        public PropertyServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _storageMock = new Mock<IImageStorage>();
            _imgRepoMock = new Mock<IGenericRepository<PropertyImage>>();
            _traceRepoMock = new Mock<IGenericRepository<PropertyTrace>>();
            _ownerRepoMock = new Mock<IGenericRepository<Owner>>();
            _service = new PropertyService(_uowMock.Object, _storageMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreate_WhenOwnerExists()
        {
            // Arrange
            var req = new CreatePropertyRequest("Test", "Addr", 1000, 123, 2024, 1);
            _uowMock.Setup(r => r.Owners.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Owner { IdOwner = 1 });
            _uowMock.Setup(r => r.Owners.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new Owner { IdOwner = 1 });
            _uowMock.Setup(r => r.Properties.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>())).Verifiable();

            // Act
            var result = await _service.CreateAsync(req, default);

            // Assert
            Assert.True(result.Succeeded);
            _uowMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenOwnerNotFound()
        {
            // Arrange
            var req = new CreatePropertyRequest("Test", "Addr", 1000, 123, 2024, 1);
            _uowMock.Setup(r => r.Owners.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Owner?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(req, default));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenPropertyAndOwnerExist()
        {
            // Arrange
            var req = new UpdatePropertyRequest("NewName", "NewAddr", 2023, 2);
            var property = new Property { IdProperty = 1, IdOwner = 1, Name = "Old" };

            _uowMock.Setup(r => r.Properties.GetByIdAsync(1, default)).ReturnsAsync(property);
            _uowMock.Setup(r => r.Owners.GetByIdAsync(2, default)).ReturnsAsync(new Owner { IdOwner = 2 });

            // Act
            var result = await _service.UpdateAsync(1, req, default);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("NewName", property.Name);
            Assert.Equal(2, property.IdOwner);
            _uowMock.Verify(r => r.Properties.Update(property), Times.Once);
            _uowMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenPropertyNotFound()
        {
            _uowMock.Setup(r => r.Properties.GetByIdAsync(1, default))
                    .ReturnsAsync((Property?)null);

            var req = new UpdatePropertyRequest("Name", "Addr", 2023, 1);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(1, req, default));
        }

        [Fact]
        public async Task ChangePriceAsync_ShouldChangePrice_AndSaveTrace()
        {
            var property = new Property { IdProperty = 1, Price = 100, Name = "House" };
            _uowMock.Setup(r => r.Properties.GetByIdAsync(1, default)).ReturnsAsync(property);
            _uowMock.SetupGet(u => u.PropertyTraces).Returns(_traceRepoMock.Object);

            var req = new ChangePriceRequest(200, 20);

            var result = await _service.ChangePriceAsync(1, req, default);

            Assert.True(result.Succeeded);
            Assert.Equal(200, property.Price);
            _uowMock.Verify(r => r.PropertyTraces.AddAsync(It.IsAny<PropertyTrace>(), default), Times.Once);
            _uowMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddImageAsync_ShouldSaveImage_WhenPropertyExists()
        {
            var property = new Property { IdProperty = 1 };
            _uowMock.SetupGet(u => u.PropertyImages).Returns(_imgRepoMock.Object);
            _uowMock.Setup(r => r.Properties.GetByIdAsync(1, default)).ReturnsAsync(property);
            _storageMock.Setup(s => s.SaveAsync("test.jpg", It.IsAny<Stream>(), default))
                        .ReturnsAsync("saved/path/test.jpg");

            using var stream = new MemoryStream();
            var result = await _service.AddImageAsync(1, "test.jpg", stream, default);

            Assert.True(result.Succeeded);
            Assert.Equal("saved/path/test.jpg", result.Value);
            _uowMock.Verify(r => r.PropertyImages.AddAsync(It.IsAny<PropertyImage>(), default), Times.Once);
            _uowMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnPagedResults()
        {
            var filter = new PropertyFilter(null, null, null, null, null, null, null, 1, 10);
            var properties = new List<Property>
            {
                new Property { IdProperty = 1, Name = "House", Address = "Addr", Price = 100, Year = 2020, IdOwner = 1, Owner = new Owner{ Name = "Owner" }, PropertyImages = new List<PropertyImage>() }
            };

            _uowMock.Setup(r => r.Properties.SearchAsync(null, null, null, null, null, null, null, 1, 10, default))
                    .ReturnsAsync((properties, 1));

            var result = await _service.SearchAsync(filter, default);

            Assert.True(result.Succeeded);
            Assert.Single(result.Value.Items);
            Assert.Equal("House", result.Value.Items[0].Name);
        }
    }
}
