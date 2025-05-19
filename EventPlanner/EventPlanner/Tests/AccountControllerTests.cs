using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AccountControllerTests
{
  private Mock<UserManager<ApplicationUser>> _mockUserManager;
  private Mock<SignInManager<ApplicationUser>> _mockSignInManager;

  public AccountControllerTests()
  {
    var store = new Mock<IUserStore<ApplicationUser>>();
    _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

    var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
    var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

    _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
        _mockUserManager.Object,
        contextAccessor.Object,
        claimsFactory.Object,
        null, null, null, null);
  }

  [Fact]
  public async Task Login_ValidCredentials_ReturnsRedirectToHome()
  {
    // Arrange
    var model = new LoginViewModel
    {
      Username = "testuser",
      Password = "password123",
      RememberMe = false
    };

    _mockSignInManager.Setup(s => s.PasswordSignInAsync(
        model.Username,
        model.Password,
        model.RememberMe,
        false))
        .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

    var controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);

    // Act
    var result = await controller.Login(model);

    // Assert
    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirectResult.ActionName);
    Assert.Equal("Home", redirectResult.ControllerName);
  }
}
