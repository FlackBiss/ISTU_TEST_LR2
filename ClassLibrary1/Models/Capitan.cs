namespace ClassLibrary1.Models;

public enum CaptainState
{
    Idle,       // Бездействует
    Commanding  // Командует
}

public class Captain
{
    private CaptainState state;

    public Captain()
    {
        this.state = CaptainState.Idle;
    }

    public CaptainState State => state;

    public void GiveCommand(string command)
    {
        state = CaptainState.Commanding;
        state = CaptainState.Idle;
    }
}