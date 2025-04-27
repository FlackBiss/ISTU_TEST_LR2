using ClassLibrary1.Models;

namespace TestProject2.UnitTests;

public class EngineTests
{
    private readonly Engine engine;

    public EngineTests()
    {
        engine = new Engine(200.0);
    }

    // Тестирование свойства
    [Fact]
    public void Power_ReturnsCorrectValue()
    {
        Assert.Equal(200.0, engine.Power);
    }

    [Fact]
    public void State_InitiallyOff()
    {
        Assert.Equal(EngineState.Off, engine.State);
    }

    // Тестирование методов и состояний
    [Fact]
    public void TurnOn_ChangesStateToOn()
    {
        engine.TurnOn();
        Assert.Equal(EngineState.On, engine.State);
    }

    [Fact]
    public void TurnOff_ChangesStateToOff()
    {
        engine.TurnOn();
        engine.TurnOff();
        Assert.Equal(EngineState.Off, engine.State);
    }

    [Fact]
    public void CheckState_PowerBelow100_SetsFaulty()
    {
        var faultyEngine = new Engine(50.0);
        faultyEngine.CheckState();
        Assert.Equal(EngineState.Faulty, faultyEngine.State);
    }
}