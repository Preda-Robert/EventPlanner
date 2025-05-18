using Microsoft.AspNetCore.Mvc;
using Xunit;

public class LoginTest
{
  [Fact]
  public void Login_Get_ReturnsView()
  {
    // Arrange
    var controller = new AccountController(null, null);

    // Act
    var result = controller.Login() as ViewResult;

    // Assert
    Assert.NotNull(result);
    Assert.Null(result.ViewName);
  }

}
