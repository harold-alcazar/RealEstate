using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealEstate.Api.Controllers;
using RealEstate.Application.DTOs;
using RealEstate.Application.IServices;
using RealEstate.Application.Results;
using System.Text;
using Xunit;


namespace RealEstate.Tests.Unit.Controllers
{
    public class PropertiesControllerTests
    {
        private readonly Mock<IPropertyService> _serviceMock;
        private readonly PropertiesController _controller;

        public PropertiesControllerTests()
        {
            _serviceMock = new Mock<IPropertyService>();
            _controller = new PropertiesController(_serviceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsOk()
        {
            // Arrange
            var req = new CreatePropertyRequest("Casa", "Calle", 100m, 123, 2020, 1);
            _serviceMock.Setup(s => s.CreateAsync(req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<int>.Ok(42));

            // Act
            var action = await _controller.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action);
            Assert.Equal(42, ok.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WithResultObject()
        {
            // Arrange
            var req = new UpdatePropertyRequest("Casa", "Calle", 2021, 1);
            var id = 5;
            var expected = Result.Ok();
            _serviceMock
                .Setup(s => s.UpdateAsync(id, req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var action = await _controller.Update(id, req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action);
            var value = Assert.IsType<Result>(ok.Value);
            Assert.True(value.Succeeded);
        }

        [Fact]
        public async Task ChangePrice_ReturnsOk_WithResultObject()
        {
            // Arrange
            var req = new ChangePriceRequest(150m, 2m);
            var id = 7;
            var expected = Result.Ok();
            _serviceMock
                .Setup(s => s.ChangePriceAsync(id, req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var action = await _controller.ChangePrice(id, req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action);
            var value = Assert.IsType<Result>(ok.Value);
            Assert.True(value.Succeeded);
        }

        [Fact]
        public async Task AddImage_ReturnsBadRequest_WhenFileNull()
        {
            // Act
            var action = await _controller.AddImage(1, null, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(action);
            Assert.Equal("File is required", bad.Value);
        }

        [Fact]
        public async Task AddImage_ReturnsOk_WhenServiceReturnsOk()
        {
            // Arrange
            var id = 3;
            var fileName = "image.jpg";
            var content = "fake-image";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.Length).Returns(ms.Length);
            formFileMock.Setup(f => f.FileName).Returns(fileName);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            var resultOk = Result<string>.Ok("saved/path/image.jpg");
            _serviceMock
                .Setup(s => s.AddImageAsync(id, fileName, It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultOk);

            // Act
            var action = await _controller.AddImage(id, formFileMock.Object, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action);
            var value = Assert.IsType<Result<string>>(ok.Value);
            Assert.True(value.Succeeded);
            Assert.Equal("saved/path/image.jpg", value.Value);
        }

        [Fact]
        public async Task Search_ReturnsOk_WithPagedResult()
        {
            // Arrange
            var filter = new PropertyFilter(null, null, null, null, null, null, null, 1, 20);
            var props = new List<PropertyResponse>
            {
                new PropertyResponse(1, "Casa A", "Calle", 100m, 2020, 1, "Owner", new List<string>())
            };
            var paged = new PagedResult<PropertyResponse>(props, 1, 1, 20);
            var result = Result<PagedResult<PropertyResponse>>.Ok(paged);

            _serviceMock
                .Setup(s => s.SearchAsync(It.IsAny<PropertyFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var action = await _controller.Search(null, null, null, null, null, null, null, 1, 20, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action);
            var returned = Assert.IsType<Result<PagedResult<PropertyResponse>>>(ok.Value);
            Assert.True(returned.Succeeded);
            Assert.Equal(1, returned.Value.Total);
        }
    }
}
