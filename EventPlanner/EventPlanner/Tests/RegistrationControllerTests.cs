using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EventPlanner.Tests
{
  public class RegistrationControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly RegistrationController _controller;

    public RegistrationControllerTests()
    {
      _mockRepo = new Mock<IRepositoryWrapper>();

      var userStore = new Mock<IUserStore<ApplicationUser>>();
      _mockUserManager = new Mock<UserManager<ApplicationUser>>(
          userStore.Object, null, null, null, null, null, null, null, null);

      _controller = new RegistrationController(_mockRepo.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task Index_WithLoggedInUser_ReturnsViewWithUserRegistrations()
    {
      // Arrange
      var userId = 1;
      var user = new ApplicationUser { Id = userId };
      var registrations = new List<Registration>
            {
                new Registration { RegistrationId = 1, UserId = userId, EventId = 1 },
                new Registration { RegistrationId = 2, UserId = userId, EventId = 2 }
            };

      _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
          .ReturnsAsync(user);

      _mockRepo.Setup(repo => repo.Registration.GetAllAsync(
          It.IsAny<System.Linq.Expressions.Expression<Func<Registration, bool>>>(),
          It.IsAny<Func<IQueryable<Registration>, IQueryable<Registration>>>()))
          .ReturnsAsync(registrations);

      // Act
      var result = await _controller.Index();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<Registration>>(viewResult.Model);
      Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Index_WithNoLoggedInUser_ReturnsUnauthorized()
    {
      // Arrange
      _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
          .ReturnsAsync((ApplicationUser)null);

      // Act
      var result = await _controller.Index();

      // Assert
      Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Unregister_WithValidId_DeletesRegistrationAndRedirectsToIndex()
    {
      // Arrange
      var registrationId = 1;
      var registration = new Registration { RegistrationId = registrationId };

      _mockRepo.Setup(repo => repo.Registration.GetByIdAsync(registrationId))
          .ReturnsAsync(registration);

      // Act
      var result = await _controller.Unregister(registrationId);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Index", redirectResult.ActionName);

      _mockRepo.Verify(repo => repo.Registration.Delete(registration), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Unregister_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      var registrationId = 999;

      _mockRepo.Setup(repo => repo.Registration.GetByIdAsync(registrationId))
          .ReturnsAsync((Registration)null);

      // Act
      var result = await _controller.Unregister(registrationId);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }
  }
}
