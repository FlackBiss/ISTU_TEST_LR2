using WeatherApp.Models;

namespace WeatherApp.Test.UnitTests;

public class NavigationSystemTests
{
    private readonly NavigationSystem navigationSystem;

    public NavigationSystemTests()
    {
        navigationSystem = new NavigationSystem();
    }

    // Тестирование свойств
    [Fact]
    public void CurrentDestination_InitiallyNone()
    {
        Assert.Equal("None", navigationSystem.CurrentDestination);
    }

    [Fact]
    public void IsActive_InitiallyFalse()
    {
        Assert.False(navigationSystem.IsActive);
    }

    // Тестирование методов и состояний
    [Fact]
    public void SetDestination_UpdatesDestinationAndActivates()
    {
        navigationSystem.SetDestination("Port B");
        Assert.Equal("Port B", navigationSystem.CurrentDestination);
        Assert.True(navigationSystem.IsActive);
    }

    [Fact]
    public void StopNavigation_ResetsDestinationAndDeactivates()
    {
        navigationSystem.SetDestination("Port B");
        navigationSystem.StopNavigation();
        Assert.Equal("None", navigationSystem.CurrentDestination);
        Assert.False(navigationSystem.IsActive);
    }
}