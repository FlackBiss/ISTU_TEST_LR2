using ClassLibrary1.Models;

namespace ClassLibrary1.Interfaces;

public interface ICargoSystem
{
    double CurrentWeight { get; }
    double MaxCapacity { get; }
    CargoState State { get; }
    void Load(double weight);
    void Unload();
}