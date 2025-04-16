using WeatherApp.Models;

namespace WeatherApp.Interfaces;

public interface ICargoSystem
{
    double CurrentWeight { get; }
    double MaxCapacity { get; }
    CargoState State { get; }
    void Load(double weight);
    void Unload();
}