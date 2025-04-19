using WeatherApp.Interfaces;

namespace WeatherApp.Test.UnitTests;

using Moq;
using WeatherApp.Models;

public class ShipTests
{
    private readonly Mock<IEngine> engineMock;
    private readonly Mock<ICargoSystem> cargoSystemMock;
    private readonly Mock<NavigationSystem> navigationSystemMock;
    private readonly Mock<OperationLog> operationLogMock;
    private readonly Ship ship;

    public ShipTests()
    {
        engineMock = new Mock<IEngine>();
        cargoSystemMock = new Mock<ICargoSystem>();
        navigationSystemMock = new Mock<NavigationSystem>();
        operationLogMock = new Mock<OperationLog>();
        ship = new Ship("SHIP001", engineMock.Object, cargoSystemMock.Object, navigationSystemMock.Object,
            operationLogMock.Object);
    }

    // Тестирование свойства
    [Fact]
    public void Identifier_ReturnsCorrectValue()
    {
        Assert.Equal("SHIP001", ship.Identifier);
    }

    [Fact]
    public void State_InitiallyStopped()
    {
        Assert.Equal(ShipState.Stopped, ship.State);
    }

    // Тестирование методов и переходов состояний
    [Fact]
    public void Start_ChangesStateToWaiting()
    {
        ship.Start();
        Assert.Equal(ShipState.Waiting, ship.State);
        engineMock.Verify(e => e.TurnOn(), Times.Once());
        operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
    }

    [Fact]
    public void Stop_ChangesStateToStopped()
    {
        ship.Start(); // Сначала переводим в состояние Waiting
        ship.Stop();
        Assert.Equal(ShipState.Stopped, ship.State);
        engineMock.Verify(e => e.TurnOff(), Times.Once());
    }

    [Fact]
    public void LoadCargo_ChangesStateToLoadingThenWaiting()
    {
        ship.Start(); // Переходим в Waiting
        cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
        ship.LoadCargo(100.0);
        Assert.Equal(ShipState.Waiting, ship.State); // После загрузки возвращается в Waiting
        cargoSystemMock.Verify(c => c.Load(100.0), Times.Once());
    }

    [Fact]
    public void NavigateTo_ChangesStateToMoving()
    {
        ship.Start(); // Переходим в Waiting
        ship.NavigateTo("Port B");
        Assert.Equal(ShipState.Moving, ship.State);
        navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
    }

    [Fact]
    public void GenerateReport_ReturnsCorrectReport()
    {
        cargoSystemMock.Setup(c => c.CurrentWeight).Returns(500.0);
        navigationSystemMock.Setup(n => n.CurrentDestination).Returns("Port B");
        ship.Start(); // Переходим в Waiting
        var report = ship.GenerateReport();
        Assert.Equal("Ship SHIP001: State=Waiting, Cargo=500 tons, Destination=Port B", report);
    }

    // Тестирование сценария: Останов → Ожидание → Останов
    [Fact]
    public void Scenario_StopToWaitingToStop()
    {
        Assert.Equal(ShipState.Stopped, ship.State);
        ship.Start();
        Assert.Equal(ShipState.Waiting, ship.State);
        ship.Stop();
        Assert.Equal(ShipState.Stopped, ship.State);
    }

    // Тестирование сценария: Ожидание → Погрузка → Ожидание
    [Fact]
    public void Scenario_WaitingToLoadingToWaiting()
    {
        ship.Start();
        Assert.Equal(ShipState.Waiting, ship.State);
        ship.LoadCargo(100.0);
        Assert.Equal(ShipState.Waiting, ship.State);
    }

    // Тестирование сценария: Ожидание → Движение → Ожидание
    [Fact]
    public void Scenario_WaitingToMovingToWaiting()
    {
        ship.Start();
        Assert.Equal(ShipState.Waiting, ship.State);
        ship.NavigateTo("Port B");
        Assert.Equal(ShipState.Moving, ship.State);
        ship.Wait();
        Assert.Equal(ShipState.Waiting, ship.State);
    }
}