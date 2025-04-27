using ClassLibrary1.Models;

namespace TestProject2.UnitTests;

public class CaptainTests
{
    private readonly Captain captain;

    public CaptainTests()
    {
        captain = new Captain();
    }

    // Тестирование свойства
    [Fact]
    public void State_InitiallyIdle()
    {
        Assert.Equal(CaptainState.Idle, captain.State);
    }

    // Тестирование методов и состояний
    [Fact]
    public void GiveCommand_ChangesStateToCommandingThenIdle()
    {
        captain.GiveCommand("navigate");
        Assert.Equal(CaptainState.Idle, captain.State);
    }
}