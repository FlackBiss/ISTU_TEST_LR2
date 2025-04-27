namespace ClassLibrary1.Models;

public class OperationLog
{
    private List<string> logs; // Список логов

    public OperationLog()
    {
        this.logs = new List<string>();
    }

    public IReadOnlyList<string> Logs => logs.AsReadOnly();

    public virtual void Log(string message)
    {
        logs.Add($"{DateTime.Now}: {message}");
    }

    public void Clear()
    {
        logs.Clear();
    }
}