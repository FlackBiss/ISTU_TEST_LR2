using ClassLibrary1.Models;

namespace TestProject2.UnitTests;

public class CargoSystemTests
{
    private readonly CargoSystem _cargoSystem;

    public CargoSystemTests()
    {
        _cargoSystem = new CargoSystem(1000.0);
    }

    // Тест атрибута MaxCapacity
    [Fact]
    public void MaxCapacity_ShouldReturnCorrectValue()
    {
        Assert.Equal(1000.0, _cargoSystem.MaxCapacity);
    }

    // Тест метода Load
    [Fact]
    public void Load_ShouldUpdateWeightAndState()
    {
        _cargoSystem.Load(500.0);
        Assert.Equal(500.0, _cargoSystem.CurrentWeight);
        Assert.Equal(CargoState.Empty, _cargoSystem.State);
    }

    // Тест метода Load при достижении максимальной вместимости
    [Fact]
    public void Load_AtMaxCapacity_ShouldSetStateToFull()
    {
        _cargoSystem.Load(1000.0);
        Assert.Equal(1000.0, _cargoSystem.CurrentWeight);
        Assert.Equal(CargoState.Full, _cargoSystem.State);
    }

    // Тест метода Unload
    [Fact]
    public void Unload_ShouldResetWeightAndState()
    {
        _cargoSystem.Load(500.0);
        _cargoSystem.Unload();
        Assert.Equal(0.0, _cargoSystem.CurrentWeight);
        Assert.Equal(CargoState.Empty, _cargoSystem.State);
    }

    // Тест исключения для Load при превышении вместимости
    [Fact]
    public void Load_WhenExceedsCapacity_ShouldThrowException()
    {
        Assert.Throws<InvalidOperationException>(() => _cargoSystem.Load(1100.0));
    }
}