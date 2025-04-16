using WeatherApp.Interfaces;

namespace WeatherApp.Models;

public enum EngineState
{
    Off, // Выключен
    On, // Работает
    Faulty // Неисправен
}

public class Engine : IEngine
{
    private double power; // Мощность двигателя
    private EngineState state; // Состояние двигателя

    public Engine(double power)
    {
        this.power = power;
        this.state = EngineState.Off; // Начальное состояние — Выключен
    }

    public double Power => power;
    public EngineState State => state;

    public void TurnOn()
    {
        if (state == EngineState.Off)
        {
            state = EngineState.On;
        }
    }

    public void TurnOff()
    {
        if (state == EngineState.On)
        {
            state = EngineState.Off;
        }
    }

    public void CheckState()
    {
        if (power < 100)
        {
            state = EngineState.Faulty;
        }
    }
}