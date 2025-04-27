using ClassLibrary1.Interfaces;

namespace ClassLibrary1.Models;

public enum CargoState
{
    Empty, // Пустая
    Loading, // Загружается
    Full // Полная
}

public class CargoSystem : ICargoSystem
{
    private double currentWeight; // Текущий вес груза
    private readonly double maxCapacity; // Максимальная вместимость
    private CargoState state; // Состояние грузовой системы

    public CargoSystem(double maxCapacity)
    {
        this.maxCapacity = maxCapacity;
        this.currentWeight = 0;
        this.state = CargoState.Empty;
    }

    public double CurrentWeight => currentWeight;
    public double MaxCapacity => maxCapacity;
    public CargoState State => state;

    public void Load(double weight)
    {
        if (currentWeight + weight <= maxCapacity)
        {
            state = CargoState.Loading;
            currentWeight += weight;
            state = currentWeight >= maxCapacity ? CargoState.Full : CargoState.Empty;
        }
        else
        {
            throw new InvalidOperationException("Maximum weight cannot be more than maximum capacity");
        }
    }

    public void Unload()
    {
        currentWeight = 0;
        state = CargoState.Empty;
    }
}