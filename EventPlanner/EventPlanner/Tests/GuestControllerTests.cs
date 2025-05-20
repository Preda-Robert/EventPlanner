using EventPlanner.Enums;
using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace EventPlanner.Tests.Controllers
{
  public class GuestControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IRepository<Guest>> _mockGuestRepo;
    private readonly Mock<IRepository<Event>> _mockEventRepo;
    private readonly Mock<IRepository<EventGuest>> _mockEventGuestRepo;
    private readonly GuestController _controller;

    public GuestControllerTests()
    {
      // Setup individual repositories
      _mockRepoWrapper = new Mock<IRepositoryWrapper>();
      _mockGuestRepo = new Mock<IRepository<Guest>>();
      _mockEventRepo = new Mock<IRepository<Event>>();
      _mockEventGuestRepo = new Mock<IRepository<EventGuest>>();

      _mockRepoWrapper.Setup(r => r.Guest).Returns(_mockGuestRepo.Object);
      _mockRepoWrapper.Setup(r => r.Event).Returns(_mockEventRepo.Object);
      _mockRepoWrapper.Setup(r => r.EventGuest).Returns(_mockEventGuestRepo.Object);

      _controller = new GuestController(_mockRepoWrapper.Object);

      var httpContext = new DefaultHttpContext();
      var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
      _controller.TempData = tempData;
      _controller.ControllerContext = new ControllerContext()
      {
        HttpContext = httpContext
      };
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

      _mockGuestRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(guests);
      _mockEventRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

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
      int eventId = 1;
      var guests = new List<Guest>
            {
                new Guest { GuestId = 1, Name = "Guest 1" },
                new Guest { GuestId = 2, Name = "Guest 2" }
            };

      var eventGuests = new List<EventGuest>
            {
                new EventGuest { EventId = eventId, GuestId = 1 }
            };

      var events = new List<Event>
            {
                new Event { EventId = eventId, Title = "Event 1" }
            };

      _mockGuestRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(guests);
      _mockEventGuestRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(eventGuests);
      _mockEventRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

      // Act
      var result = await _controller.Index(eventId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<GuestViewModel>>(viewResult.Model);
      Assert.Single(model);
      Assert.Equal(1, model.First().GuestId);
    }

    [Fact]
    public async Task Details_ValidId_ReturnsViewWithGuest()
    {
      // Arrange
      var guest = new Guest { GuestId = 1, Name = "Test Guest" };
      _mockGuestRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(guest);

      // Act
      var result = await _controller.Details(1);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Guest>(viewResult.Model);
      Assert.Equal(1, model.GuestId);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
      _mockGuestRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Guest)null);

      var result = await _controller.Details(999);

      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_GET_ReturnsViewWithViewModel()
    {
      var result = _controller.Create();

      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<GuestCreateViewModel>(viewResult.Model);
      Assert.NotNull(model.GuestTypes);
      Assert.Equal(2, model.GuestTypes.Count);
    }

    [Fact]
    public async Task Create_POST_ValidModel_AddsGuestAndRedirects()
    {
      // Arrange
      var viewModel = new GuestCreateViewModel
      {
        Name = "New Guest",
        Role = "Speaker",
        Type = GuestType.Single
      };

      _controller.ModelState.Clear();

      // Act
      var result = await _controller.Create(viewModel);

      // Assert
      var redirect = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Index", redirect.ActionName);


    }
  }
}
