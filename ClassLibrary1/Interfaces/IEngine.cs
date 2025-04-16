using WeatherApp.Models;

namespace WeatherApp.Interfaces;

public interface IEngine
{
    double Power { get; }
    EngineState State { get; }
    void TurnOn();
    void TurnOff();
    void CheckState();
}