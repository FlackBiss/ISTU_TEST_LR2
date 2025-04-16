using WeatherApp.Models;

namespace WeatherApp.Interfaces;

public interface IShip
{
    string Identifier { get; }
    ShipState State { get; }
    void Start();
    void Stop();
    void LoadCargo(double weight);
    void UnloadCargo();
    void NavigateTo(string destination);
    string GenerateReport();
}