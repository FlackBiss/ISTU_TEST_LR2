namespace ClassLibrary1.Models;

public class NavigationSystem
{
    private string currentDestination;  // Текущий пункт назначения
    private bool isActive;              // Состояние активности системы

    public NavigationSystem()
    {
        this.currentDestination = "None";
        this.isActive = false; // Начальное состояние — Неактивна
    }

    public virtual string CurrentDestination => currentDestination;
    public virtual bool IsActive => isActive;

    public virtual void SetDestination(string destination)
    {
        this.currentDestination = destination;
        this.isActive = true;
    }

    public void StopNavigation()
    {
        this.currentDestination = "None";
        this.isActive = false;
    }
}