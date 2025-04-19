using Moq;
using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace TestProject2.UnitTests;

public class MonitoringSystemTests
{
    private readonly MonitoringSystem monitoringSystem;
    private readonly Mock<IShip> shipMock;
    private readonly Mock<IEngine> engineMock;
    private readonly Mock<ICargoSystem> cargoSystemMock;

    public MonitoringSystemTests()
    {
        monitoringSystem = new MonitoringSystem();
        shipMock = new Mock<IShip>();
        engineMock = new Mock<IEngine>();
        cargoSystemMock = new Mock<ICargoSystem>();
    }

    [Fact]
    public void IsMonitoring_InitiallyFalse()
    {
        Assert.False(monitoringSystem.IsMonitoring);
    }

    [Fact]
    public void StartMonitoring_SetsIsMonitoringTrue()
    {
        monitoringSystem.StartMonitoring();
        Assert.True(monitoringSystem.IsMonitoring);
    }

    [Fact]
    public void CollectData_WhenMonitoring_ReturnsData()
    {
        shipMock.Setup(s => s.State).Returns(ShipState.Waiting);
        engineMock.Setup(e => e.State).Returns(EngineState.On);
        cargoSystemMock.Setup(c => c.CurrentWeight).Returns(500.0);
        monitoringSystem.StartMonitoring();
        var data = monitoringSystem.CollectData(shipMock.Object, engineMock.Object, cargoSystemMock.Object);
        Assert.Equal("Ship State: Waiting, Engine State: On, Cargo: 500 tons", data);
    }

    [Fact]
    public void CollectData_WhenNotMonitoring_ReturnsError()
    {
        var data = monitoringSystem.CollectData(shipMock.Object, engineMock.Object, cargoSystemMock.Object);
        Assert.Equal("Monitoring is not active.", data);
    }
}