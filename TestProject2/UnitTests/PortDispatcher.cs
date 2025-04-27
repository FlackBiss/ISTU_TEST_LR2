using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using WeatherApp.Interfaces;

namespace TestProject2.UnitTests;

public class PortDispatcherTests
{
    private readonly IEngine engine;
    private readonly ICargoSystem cargoSystem;
    private readonly NavigationSystem navigationSystem;
    private readonly OperationLog operationLog;
    private readonly IShip ship;
    private readonly PortDispatcher portDispatcher;

    public PortDispatcherTests()
    {
        engine = new Engine(200.0); // Power > 100 to avoid Faulty state
        cargoSystem = new CargoSystem(1000.0); // Max capacity of 1000 tons
        navigationSystem = new NavigationSystem();
        operationLog = new OperationLog();
        ship = new Ship("SHIP001", engine, cargoSystem, navigationSystem, operationLog);
        portDispatcher = new PortDispatcher();
    }

    // Original test: RequestReport_ReturnsShipReport
    [Fact]
    public void RequestReport_ReturnsShipReport()
    {
        // Arrange
        ship.Start(); // Move to Waiting
        ship.LoadCargo(500.0); // Set cargo weight
        ship.NavigateTo("Port B"); // Set destination

        // Act
        var report = portDispatcher.RequestReport(ship);

        // Assert
        Assert.Equal("Ship SHIP001: State=Moving, Cargo=500 tons, Destination=Port B", report);
    }

    // Original test: SendCommand_Navigate_ChangesShipState
    [Fact]
    public void SendCommand_Navigate_ChangesShipState()
    {
        // Arrange
        ship.Start(); // Переходим в Waiting

        // Act
        portDispatcher.SendCommand(ship, "navigate");

        // Assert
        Assert.Equal(ShipState.Moving, ship.State);
        Assert.Equal("Port B", navigationSystem.CurrentDestination);
        Assert.Contains(operationLog.Logs, log => log.Contains("Navigating to Port B."));
    }

    // Test for Sequence Diagram: Start Ship (Fig. 6.3)
    [Fact]
    public void StartShip_ShouldTransitionToWaiting()
    {
        // Arrange
        // Ship is initially in Stopped state

        // Act
        portDispatcher.SendCommand(ship, "start");

        // Assert
        Assert.Equal(ShipState.Waiting, ship.State);
        Assert.Equal(EngineState.On, engine.State);
        Assert.Contains(operationLog.Logs, log => log.Contains("Ship started and moved to Waiting state."));
    }

    // Test for Sequence Diagram: Stop Ship (Fig. 6.4)
    [Fact]
    public void StopShip_ShouldTransitionToStopped()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting

        // Act
        portDispatcher.SendCommand(ship, "stop");

        // Assert
        Assert.Equal(ShipState.Stopped, ship.State);
        Assert.Equal(EngineState.Off, engine.State);
        Assert.Contains(operationLog.Logs, log => log.Contains("Ship stopped."));
    }

    // Test for Sequence Diagram: Perform Loading (Fig. 6.5)
    [Fact]
    public void PerformLoading_ShouldLoadCargoAndReturnToWaiting()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting

        // Act
        portDispatcher.SendCommand(ship, "load"); // Loads 100 tons (per PortDispatcher)

        // Assert
        Assert.Equal(ShipState.Waiting, ship.State);
        Assert.Equal(100.0, cargoSystem.CurrentWeight);
        Assert.Equal(CargoState.Empty, cargoSystem.State); // 100 tons < maxCapacity, so state remains Empty
        Assert.Contains(operationLog.Logs, log => log.Contains("Loaded 100 tons of cargo."));
    }

    // Test for Sequence Diagram: Perform Unloading (Fig. 6.6)
    [Fact]
    public void PerformUnloading_ShouldUnloadCargoAndReturnToWaiting()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting
        portDispatcher.SendCommand(ship, "load"); // Load 100 tons

        // Act
        portDispatcher.SendCommand(ship, "unload");

        // Assert
        Assert.Equal(ShipState.Waiting, ship.State);
        Assert.Equal(0.0, cargoSystem.CurrentWeight);
        Assert.Equal(CargoState.Empty, cargoSystem.State);
        Assert.Contains(operationLog.Logs, log => log.Contains("Cargo unloaded."));
    }

    // Test for Sequence Diagram: Perform Navigation (Fig. 6.8)
    [Fact]
    public void PerformNavigation_ShouldTransitionToMoving()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting

        // Act
        portDispatcher.SendCommand(ship, "navigate");

        // Assert
        Assert.Equal(ShipState.Moving, ship.State);
        Assert.Equal("Port B", navigationSystem.CurrentDestination);
        Assert.True(navigationSystem.IsActive);
        Assert.Contains(operationLog.Logs, log => log.Contains("Navigating to Port B."));
    }

    // Test for Sequence Diagram: Complete Navigation (Fig. 6.2)
    [Fact]
    public void CompleteNavigation_ShouldReturnToWaiting()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting
        portDispatcher.SendCommand(ship, "navigate"); // Move to Moving

        // Act
        ship.Wait(); // Assuming Wait() is used to complete navigation, as no other method transitions Moving to Waiting

        // Assert
        Assert.Equal(ShipState.Waiting, ship.State);
        Assert.Equal(EngineState.On, engine.State);
        Assert.Contains(operationLog.Logs, log => log.Contains("Ship Waiting."));
    }

    // Test for Sequence Diagram: Monitor State (Fig. 6.7)
    [Fact]
    public void MonitorState_ShouldReturnCorrectReport()
    {
        // Arrange
        portDispatcher.SendCommand(ship, "start"); // Move to Waiting
        portDispatcher.SendCommand(ship, "load"); // Load 100 tons
        navigationSystem.SetDestination("Port C"); // Manually set destination for test

        // Act
        var report = portDispatcher.RequestReport(ship);

        // Assert
        Assert.Equal("Ship SHIP001: State=Waiting, Cargo=100 tons, Destination=Port C", report);
    }
}