using Moq;
using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Test.UnitTests;

public class PortDispatcherTests
{
    private readonly Mock<IEngine> engineMock;
    private readonly Mock<ICargoSystem> cargoSystemMock;
    private readonly Mock<NavigationSystem> navigationSystemMock;
    private readonly Mock<OperationLog> operationLogMock;
    private readonly IShip ship;
    private readonly PortDispatcher portDispatcher;

    public PortDispatcherTests()
    {
        engineMock = new Mock<IEngine>();
        cargoSystemMock = new Mock<ICargoSystem>();
        navigationSystemMock = new Mock<NavigationSystem>();
        operationLogMock = new Mock<OperationLog>();
        ship = new Ship("SHIP001", engineMock.Object, cargoSystemMock.Object, navigationSystemMock.Object, operationLogMock.Object);
        portDispatcher = new PortDispatcher();
    }

    // Тестирование метода RequestReport
    [Fact]
    public void RequestReport_ReturnsShipReport()
    {
        cargoSystemMock.Setup(c => c.CurrentWeight).Returns(500.0);
        navigationSystemMock.Setup(n => n.CurrentDestination).Returns("Port B");
        ship.Start();
        var report = portDispatcher.RequestReport(ship);
        Assert.Equal("Ship SHIP001: State=Waiting, Cargo=500 tons, Destination=Port B", report);
    }

    // Тестирование сценария взаимодействия
    [Fact]
    public void SendCommand_Navigate_ChangesShipState()
    {
        ship.Start(); // Переходим в Waiting
        portDispatcher.SendCommand(ship, "navigate");
        Assert.Equal(ShipState.Moving, ship.State);
        navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
    }
}