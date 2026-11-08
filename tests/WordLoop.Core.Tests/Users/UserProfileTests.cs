using WordLoop.Core.Users;

namespace WordLoop.Core.Tests.Users;

public class UserProfileTests
{
    [Fact]
    public void DisplayName_FirstAndLastName_JoinsWithSpace()
    {
        var user = new UserProfile(42, "Dmitry", "Demin", "dem_in");

        Assert.Equal("Dmitry Demin", user.DisplayName);
    }

    [Fact]
    public void DisplayName_OnlyFirstName_ReturnsFirstName()
    {
        var user = new UserProfile(42, "Dmitry", null, "dem_in");

        Assert.Equal("Dmitry", user.DisplayName);
    }

    [Fact]
    public void DisplayName_OnlyLastName_ReturnsLastName()
    {
        var user = new UserProfile(42, null, "Demin", null);

        Assert.Equal("Demin", user.DisplayName);
    }

    [Fact]
    public void DisplayName_NoNames_FallsBackToUsername()
    {
        var user = new UserProfile(42, null, null, "dem_in");

        Assert.Equal("@dem_in", user.DisplayName);
    }

    [Fact]
    public void DisplayName_NothingButId_FallsBackToId()
    {
        var user = new UserProfile(42, null, null, null);

        Assert.Equal("Пользователь 42", user.DisplayName);
    }

    [Fact]
    public void DisplayName_WhitespaceParts_AreTrimmedAndTreatedAsMissing()
    {
        var user = new UserProfile(42, "  Dmitry ", "   ", " ");

        Assert.Equal("Dmitry", user.DisplayName);
    }

    [Fact]
    public void DisplayName_AllWhitespace_FallsBackToId()
    {
        var user = new UserProfile(7, " ", "", "  ");

        Assert.Equal("Пользователь 7", user.DisplayName);
    }
}
