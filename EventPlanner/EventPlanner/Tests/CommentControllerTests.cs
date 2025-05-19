using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EventPlanner.Tests.Controllers
{
  public class CommentControllerTests
  {
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly CommentController _controller;
    private readonly Mock<IRepository<Comment>> _mockCommentRepo;
    private readonly Mock<IRepository<Event>> _mockEventRepo;

    public CommentControllerTests()
    {
      // Setup repositories
      _mockRepo = new Mock<IRepositoryWrapper>();
      _mockCommentRepo = new Mock<IRepository<Comment>>();
      _mockEventRepo = new Mock<IRepository<Event>>();

      _mockRepo.Setup(repo => repo.Comment).Returns(_mockCommentRepo.Object);
      _mockRepo.Setup(repo => repo.Event).Returns(_mockEventRepo.Object);

      // Setup UserManager mock
      var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
      _mockUserManager = new Mock<UserManager<ApplicationUser>>(
          userStoreMock.Object, null, null, null, null, null, null, null, null);

      // Initialize controller
      _controller = new CommentController(_mockRepo.Object, _mockUserManager.Object);

      // Setup controller context
      var httpContext = new DefaultHttpContext();
      var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
      _controller.TempData = tempData;
      _controller.ControllerContext = new ControllerContext()
      {
        HttpContext = httpContext
      };
    }

    [Fact]
    public async Task Index_ReturnsViewWithComments()
    {
      // Arrange
      var comments = new List<Comment> { new Comment(), new Comment() };
      _mockCommentRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(comments);

      // Act
      var result = await _controller.Index();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<IEnumerable<Comment>>(viewResult.Model);
      Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithComment()
    {
      // Arrange
      var commentId = 1;
      var comment = new Comment { CommentId = commentId };
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

      // Act
      var result = await _controller.Details(commentId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Comment>(viewResult.Model);
      Assert.Equal(commentId, model.CommentId);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment)null);

      // Act
      var result = await _controller.Details(1);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_GET_ReturnsView()
    {
      // Act
      var result = _controller.Create();

      // Assert
      Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_POST_ValidModel_RedirectsToEventDetails()
    {
      // Arrange
      var userId = "user123";
      var eventId = 5;
      var testUser = new ApplicationUser { Id = 1, UserName = "testuser" };
      var testEvent = new Event { EventId = eventId };

      var comment = new Comment { EventId = eventId };

      // Setup User identity
      var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
      var identity = new ClaimsIdentity(claims);
      var claimsPrincipal = new ClaimsPrincipal(identity);
      _controller.ControllerContext.HttpContext.User = claimsPrincipal;

      // Setup mocks
      _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
          .ReturnsAsync(testUser);
      _mockEventRepo.Setup(repo => repo.GetByIdAsync(eventId))
          .ReturnsAsync(testEvent);

      // Setup ModelState to be valid
      _controller.ModelState.Clear();

      // Act
      var result = await _controller.Create(comment);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Details", redirectResult.ActionName);
      Assert.Equal("Event", redirectResult.ControllerName);
      Assert.Equal(eventId, redirectResult.RouteValues["id"]);

      // Verify repository calls
      _mockCommentRepo.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Edit_GET_WithValidId_ReturnsViewWithComment()
    {
      // Arrange
      var commentId = 1;
      var comment = new Comment { CommentId = commentId };
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

      // Act
      var result = await _controller.Edit(commentId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Comment>(viewResult.Model);
      Assert.Equal(commentId, model.CommentId);
    }

    [Fact]
    public async Task Edit_GET_WithInvalidId_ReturnsNotFound()
    {
      // Arrange
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment)null);

      // Act
      var result = await _controller.Edit(1);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_POST_ValidModel_RedirectsToIndex()
    {
      // Arrange
      var commentId = 1;
      var comment = new Comment { CommentId = commentId };

      // Setup ModelState to be valid
      _controller.ModelState.Clear();

      // Act
      var result = await _controller.Edit(commentId, comment);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Index", redirectResult.ActionName);

      // Verify repository calls
      _mockCommentRepo.Verify(repo => repo.Update(It.IsAny<Comment>()), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_GET_WithValidId_ReturnsViewWithComment()
    {
      // Arrange
      var commentId = 1;
      var comment = new Comment { CommentId = commentId };
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

      // Act
      var result = await _controller.Delete(commentId);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsType<Comment>(viewResult.Model);
      Assert.Equal(commentId, model.CommentId);
    }

    [Fact]
    public async Task DeleteConfirmed_WithValidId_RedirectsToEventDetails()
    {
      // Arrange
      var commentId = 1;
      var eventId = 5;
      var comment = new Comment { CommentId = commentId, EventId = eventId };
      _mockCommentRepo.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(comment);

      // Act
      var result = await _controller.DeleteConfirmed(commentId);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Details", redirectResult.ActionName);
      Assert.Equal("Event", redirectResult.ControllerName);
      Assert.Equal(eventId, redirectResult.RouteValues["id"]);

      // Verify repository calls
      _mockCommentRepo.Verify(repo => repo.Delete(comment), Times.Once);
      _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }
  }
}