using WeatherApp.Interfaces;

namespace WeatherApp.Models;

public enum ShipState
{
    Stopped,    // Останов
    Waiting,    // Ожидание
    Moving,     // Движение
    Loading,    // Погрузка
    Unloading   // Разгрузка
}

public class Ship : IShip
{
    private readonly string identifier; // Идентификатор судна (константа)
    private ShipState state; // Текущее состояние судна
    private readonly IEngine engine; // Ссылка на двигатель
    private readonly ICargoSystem cargoSystem; // Ссылка на грузовую систему
    private readonly NavigationSystem navigationSystem; // Ссылка на навигационную систему
    private readonly OperationLog operationLog; // Ссылка на журнал операций

    public Ship(string identifier, IEngine engine, ICargoSystem cargoSystem, NavigationSystem navigationSystem,
        OperationLog operationLog)
    {
        this.identifier = identifier;
        this.engine = engine;
        this.cargoSystem = cargoSystem;
        this.navigationSystem = navigationSystem;
        this.operationLog = operationLog;
        this.state = ShipState.Stopped; // Начальное состояние — Останов
    }

    public string Identifier => identifier;
    public virtual ShipState State => state;

    public void Start()
    {
        if (state == ShipState.Stopped)
        {
            engine.TurnOn();
            state = ShipState.Waiting;
            operationLog.Log("Ship started and moved to Waiting state.");
        }
    }

    public void Stop()
    {
        if (state == ShipState.Moving || state == ShipState.Waiting)
        {
            engine.TurnOff();
            state = ShipState.Stopped;
            operationLog.Log("Ship stopped.");
        }
    }
    
    public void Wait()
    {
        if (state != ShipState.Waiting)
        {
            engine.TurnOn();
            state = ShipState.Waiting;
            operationLog.Log("Ship Waiting.");
        }
    }

    public void LoadCargo(double weight)
    {
        if (state == ShipState.Waiting)
        {
            state = ShipState.Loading;
            cargoSystem.Load(weight);
            state = ShipState.Waiting;
            operationLog.Log($"Loaded {weight} tons of cargo.");
        }
    }

    public void UnloadCargo()
    {
        if (state == ShipState.Waiting)
        {
            state = ShipState.Unloading;
            cargoSystem.Unload();
            state = ShipState.Waiting;
            operationLog.Log("Cargo unloaded.");
        }
    }

    public void NavigateTo(string destination)
    {
        if (state == ShipState.Waiting)
        {
            state = ShipState.Moving;
            navigationSystem.SetDestination(destination);
            operationLog.Log($"Navigating to {destination}.");
        }
    }

    public string GenerateReport()
    {
        return
            $"Ship {identifier}: State={state}, Cargo={cargoSystem.CurrentWeight} tons, Destination={navigationSystem.CurrentDestination}";
    }
}