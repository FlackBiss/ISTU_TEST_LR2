using ClassLibrary1.Models;

namespace ClassLibrary1.Interfaces;

public interface IEngine
{
    double Power { get; }
    EngineState State { get; }
    void TurnOn();
    void TurnOff();
    void CheckState();
}