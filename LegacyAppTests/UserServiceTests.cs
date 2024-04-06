using LegacyApp;
using Xunit;

namespace LegacyAppTests;

public class UserServiceTests
{
    [Fact]
    public void AddUser_Should_Return_False_When_Email_Without_At_And_Dot()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string email = "doe";
        DateTime birthDate = new DateTime(1980, 1, 1);
        int clientId = 1;
        var userService = new UserService();

        // Act
        bool result = userService.AddUser(firstName, lastName, email, birthDate, clientId);

        // Assert
        Assert.Equal(false, result);
    }
}