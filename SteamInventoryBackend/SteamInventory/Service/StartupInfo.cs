namespace SteamInventory.Service;

public class StartupInfo
{
    public DateTime StartTime { get; init; }
    public ServiceStatus Status { get; internal set; } = ServiceStatus.NotInitialized;

    internal StartupInfo() { StartTime = DateTime.UtcNow; }
}
