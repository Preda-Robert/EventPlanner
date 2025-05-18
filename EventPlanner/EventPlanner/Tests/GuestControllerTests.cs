using EventPlanner.Enums;
using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventPlanner.Tests
{
  public class GuestControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly GuestController _controller;

    public GuestControllerTests()
    {
      _mockRepo = new Mock<IRepositoryWrapper>();
      _controller = new GuestController(_mockRepo.Object);
    }

    [Fact]
    public async Task Index_NoEventId_ReturnsViewWithAllGuests()
    {
      // Arrange
      var guests = new List<Guest>
            {
                new Guest { GuestId = 1, Name = "Guest 1", Role = "Speaker", Type = (int)GuestType.Single },
                new Guest { GuestId = 2, Name = "Guest 2", Role = "Performer", Type = (int)GuestType.Multiple }
            };

      var events = new List<Event>
            {
                new Event { EventId = 1, Title = "Test Event" }
            };

      _mockRepo.Setup(repo => repo.Guest.GetAllAsync())
          .ReturnsAsync(guests);

      _mockRepo.Setup(repo => repo.Event.GetAllAsync())
          .ReturnsAsync(events);

      // Act
      var result = await _controller.Index(null);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<GuestViewModel>>(viewResult.Model);
      Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Index_WithEventId_ReturnsViewWithFilteredGuests()
    {
      // Arrange
      var eventId = 1;

      var guests = new List<Guest>
            {
                new Guest { GuestId = 1, Name = "Guest 1", Role = "Speaker", Type = (int)GuestType.Single },
                new Guest { GuestId = 2, Name = "Guest 2", Role = "Performer", Type = (int)GuestType.Multiple }
            };

      var eventGuests = new List<EventGuest>
            {
                new EventGuest { EventId = eventId, GuestId = 1 }
            };

      var events = new List<Event>
            {
                new Event { EventId = eventId, Title = "Test Event" }
            };

      _mockRepo.Setup(repo => repo.Guest.GetAllAsync())
          .ReturnsAsync(guests);

      _mockRepo.Setup(repo => repo.EventGuest.GetAllAsync())
          .ReturnsAsync(eventGuests);

      _mockRepo.Setup(repo => repo.Event.GetAllAsync())
          .ReturnsAsync(events);

      // Act
      var result = await _controller.Index(eventId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<GuestViewModel>>(viewResult.Model);
      Assert.Single(model);
      Assert.Equal(1, model.First().GuestId);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithGuest()
    {
      // Arrange
      var guestId = 1;
      var guest = new Guest { GuestId = guestId, Name = "Test Guest", Role = "Speaker", Type = (int)GuestType.Single };

      _mockRepo.Setup(repo => repo.Guest.GetByIdAsync(guestId))
          .ReturnsAsync(guest);

      // Act
      var result = await _controller.Details(guestId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Guest>(viewResult.Model);
      Assert.Equal(guestId, model.GuestId);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      var guestId = 999;

      _mockRepo.Setup(repo => repo.Guest.GetByIdAsync(guestId))
          .ReturnsAsync((Guest)null);

      // Act
      var result = await _controller.Details(guestId);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_ReturnsViewWithGuestCreateViewModel()
    {
      // Act
      var result = _controller.Create();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<GuestCreateViewModel>(viewResult.Model);
      Assert.NotNull(model.GuestTypes);
      Assert.Equal(2, model.GuestTypes.Count);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_AddsGuestAndRedirectsToIndex()
    {
      // Arrange
      var viewModel = new GuestCreateViewModel
      {
        Name = "New Guest",
        Role = "Speaker",
        Type = GuestType.Single
      };

      // Act
      var result = await _controller.Create(viewModel);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Index", redirectResult.ActionName);

      _mockRepo.Verify(repo => repo.Guest.AddAsync(It.IsAny<Guest>()), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }
  }
}
