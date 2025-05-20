using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using EventPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EventPlanner.Tests
{
  public class EventControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IEventService> _mockEventService;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly EventController _controller;

    public EventControllerTests()
    {
      _mockRepo = new Mock<IRepositoryWrapper>();

      var userStore = new Mock<IUserStore<ApplicationUser>>();
      _mockUserManager = new Mock<UserManager<ApplicationUser>>(
          userStore.Object, null, null, null, null, null, null, null, null);

      _mockEventService = new Mock<IEventService>();
      _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

      _controller = new EventController(
          _mockRepo.Object,
          _mockUserManager.Object,
          _mockEventService.Object,
          _mockWebHostEnvironment.Object);

      _controller.TempData = new TempDataDictionary(
          new DefaultHttpContext(),
          Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task Index_ReturnsViewWithEvents()
    {
      // Arrange
      var events = new List<Event>
            {
                new Event { EventId = 1, Title = "Test Event 1" },
                new Event { EventId = 2, Title = "Test Event 2" }
            }.AsQueryable();

      _mockRepo.Setup(repo => repo.Event.GetAllAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>(),
          It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
          .ReturnsAsync(events.ToList());

      _mockEventService.Setup(s => s.SearchEvents(It.IsAny<IQueryable<Event>>(), It.IsAny<string>()))
          .Returns(events);
      _mockEventService.Setup(s => s.SortEvents(It.IsAny<IQueryable<Event>>(), It.IsAny<string>()))
          .Returns(events);

      // Act
      var result = await _controller.Index(null, null);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<Event>>(viewResult.Model);
      Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithEvent()
    {
      // Arrange
      var eventId = 1;
      var eventEntity = new Event { EventId = eventId, Title = "Test Event" };

      _mockRepo.Setup(repo => repo.Event.GetByIdAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>(),
          It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
          .ReturnsAsync(eventEntity);

      // Act
      var result = await _controller.Details(eventId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Event>(viewResult.Model);
      Assert.Equal(eventId, model.EventId);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      var eventId = 999;

      _mockRepo.Setup(repo => repo.Event.GetByIdAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>(),
          It.IsAny<Func<IQueryable<Event>, IQueryable<Event>>>()))
          .ReturnsAsync((Event)null);

      // Act
      var result = await _controller.Details(eventId);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Register_WithValidIdAndLoggedInUser_AddsRegistrationAndRedirectsToIndex()
    {
      // Arrange
      var eventId = 1;
      var userId = "user123";
      var user = new ApplicationUser { UserName = userId };

      _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
          .ReturnsAsync(user);

      _mockRepo.Setup(repo => repo.Registration.GetAllAsync())
          .ReturnsAsync(new List<Registration>());

      // Act
      var result = await _controller.Register(eventId);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Index", redirectResult.ActionName);

      _mockRepo.Verify(repo => repo.Registration.AddAsync(It.IsAny<Registration>()), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
      Assert.Equal("Successfully registered!", _controller.TempData["Message"]);
    }
  }
}
