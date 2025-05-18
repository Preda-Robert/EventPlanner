//use Host from EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventPlanner.Tests
{
  public class HostControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly HostController _controller;

    public HostControllerTests()
    {
      _mockRepo = new Mock<IRepositoryWrapper>();
      _controller = new HostController(_mockRepo.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithHosts()
    {
      // Arrange
      var hosts = new List<EventPlanner.Models.Host>
            {
                new EventPlanner.Models.Host { HostId = 1, Name = "Host 1", ContactInfo = "contact1@example.com" },
                new EventPlanner.Models.Host { HostId = 2, Name = "Host 2", ContactInfo = "contact2@example.com" }
            };

      _mockRepo.Setup(repo => repo.Host.GetAllAsync())
          .ReturnsAsync(hosts);

      // Act
      var result = await _controller.Index();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<EventPlanner.Models.Host>>(viewResult.Model);
      Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithHost()
    {
      // Arrange
      var hostId = 1;
      var host = new EventPlanner.Models.Host { HostId = hostId, Name = "Test Host", ContactInfo = "contact@example.com" };

      _mockRepo.Setup(repo => repo.Host.GetByIdAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<EventPlanner.Models.Host, bool>>>(),
          It.IsAny<Func<IQueryable<EventPlanner.Models.Host>, IQueryable<EventPlanner.Models.Host>>>()))
          .ReturnsAsync(host);

      // Act
      var result = await _controller.Details(hostId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<EventPlanner.Models.Host>(viewResult.Model);
      Assert.Equal(hostId, model.HostId);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      var hostId = 999;

      _mockRepo.Setup(repo => repo.Host.GetByIdAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<EventPlanner.Models.Host, bool>>>(),
          It.IsAny<Func<IQueryable<EventPlanner.Models.Host>, IQueryable<EventPlanner.Models.Host>>>()))
          .ReturnsAsync((EventPlanner.Models.Host)null);

      // Act
      var result = await _controller.Details(hostId);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
      // Act
      var result = _controller.Create();

      // Assert
      Assert.IsType<ViewResult>(result);
    }

    //[Fact]
    //public async Task Create_Post_WithValidModel_AddsHostAndRedirectsToIndex()
    //{
    //  // Arrange
    //  var host = new EventPlanner.Models.Host
    //  {
    //    Name = "New Host",
    //    EventsHosted = new List<EventPlanner.Models.Event>(),
    //    ContactInfo = "newhost@example.com"
    //  };

    //  // Act
    //  var result = await _controller.Create(host);

    //  // Assert
    //  var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    //  Assert.Equal("Index", redirectResult.ActionName);

    //  _mockRepo.Verify(repo => repo.Host.AddAsync(host), Times.Once);
    //  _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    //}
  }
}
