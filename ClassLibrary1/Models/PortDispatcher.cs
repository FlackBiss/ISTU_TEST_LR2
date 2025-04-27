using WeatherApp.Interfaces;

namespace ClassLibrary1.Models;

public class PortDispatcher
{
    public string RequestReport(IShip ship)
    {
        return ship.GenerateReport();
    }

    public void SendCommand(IShip ship, string command)
    {
        switch (command)
        {
            case "start":
                ship.Start();
                break;
            case "stop":
                ship.Stop();
                break;
            case "load":
                ship.LoadCargo(100);
                break;
            case "unload":
                ship.UnloadCargo();
                break;
            case "navigate":
                ship.NavigateTo("Port B");
                break;
        }
    }
}