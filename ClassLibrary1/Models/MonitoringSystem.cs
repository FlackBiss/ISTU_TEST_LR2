using WeatherApp.Interfaces;

namespace WeatherApp.Models;

public class MonitoringSystem
{
    private bool isMonitoring; // Состояние мониторинга

    public MonitoringSystem()
    {
        this.isMonitoring = false; // Начальное состояние — Неактивна
    }

    public bool IsMonitoring => isMonitoring;

    public void StartMonitoring()
    {
        this.isMonitoring = true;
    }

    public void StopMonitoring()
    {
        this.isMonitoring = false;
    }

    public string CollectData(IShip ship, IEngine engine, ICargoSystem cargoSystem)
    {
        if (isMonitoring)
        {
            return $"Ship State: {ship.State}, Engine State: {engine.State}, Cargo: {cargoSystem.CurrentWeight} tons";
        }
        return "Monitoring is not active.";
    }
}