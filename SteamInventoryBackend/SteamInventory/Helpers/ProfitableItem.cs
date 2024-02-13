using SteamInventory.Models;

namespace SteamInventory.Helpers;

public static class ProfitableItem
{
    public static string GetProfitable(IEnumerable<SteamItemHistory> steamItemHistory)
    {
        List<SteamItemHistory> steamItemHistorySorted = steamItemHistory.OrderBy(x => x.Date).ToList();

        double percentageChange = CalculatePercentageChange(steamItemHistorySorted.First().Average, steamItemHistorySorted.Last().Average);
        return GetResponse(percentageChange);
    }
    private static double CalculatePercentageChange(double initialValue, double finalValue)
    {
        return ((finalValue - initialValue) / initialValue) * 100.0;
    }

    private static string GetResponse(double percentageChange)
    {
        if (percentageChange > 0)
            return ($"Percentage Change increasing: {percentageChange:F2}%.");
        else if (percentageChange < 0)
            return ($"Percentage Change Decreasing: {percentageChange:F2}%.");
        else
            return ($"Percentage Change Stable: {percentageChange:F2}%.");
    }
}
