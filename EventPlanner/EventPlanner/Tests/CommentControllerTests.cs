using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class CommentControllerTests
{
  private readonly Mock<IRepositoryWrapper> _mockRepo = new();
  private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

  public CommentControllerTests()
  {
    var store = new Mock<IUserStore<ApplicationUser>>();
    _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
  }

  [Fact]
  public async Task Index_ReturnsViewWithComments()
  {
    // Arrange
    var comments = new List<Comment> { new Comment { CommentId = 1 } };
    _mockRepo.Setup(r => r.Comment.GetAllAsync()).ReturnsAsync(comments);
    var controller = new CommentController(_mockRepo.Object, _mockUserManager.Object);

    // Act
    var result = await controller.Index();

    // Assert
    result.Should().BeOfType<ViewResult>()
          .Which.Model.Should().BeEquivalentTo(comments);
  }
}
