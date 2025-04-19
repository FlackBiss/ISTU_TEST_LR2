using WeatherApp.Models;

namespace WeatherApp.Test.UnitTests;

public class OperationLogTests
{
    private readonly OperationLog operationLog;

    public OperationLogTests()
    {
        operationLog = new OperationLog();
    }

    // Тестирование свойств
    [Fact]
    public void Logs_InitiallyEmpty()
    {
        Assert.Empty(operationLog.Logs);
    }

    // Тестирование методов
    [Fact]
    public void Log_AddsEntry()
    {
        operationLog.Log("Test message");
        Assert.Single(operationLog.Logs);
        Assert.Contains("Test message", operationLog.Logs[0]);
    }

    [Fact]
    public void Clear_EmptiesLogs()
    {
        operationLog.Log("Test message");
        operationLog.Clear();
        Assert.Empty(operationLog.Logs);
    }
}