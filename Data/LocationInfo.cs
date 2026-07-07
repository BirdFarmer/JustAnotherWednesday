namespace JustAnotherWednesday.Data;

public record LocationInfo(string DisplayName, int UtcOffsetHours, double CostOfLivingMultiplier);

public static class LocationInfos
{
    public static readonly System.Collections.Generic.Dictionary<Location, LocationInfo> Map =
        new()
        {
            { Location.HighCostCity, new LocationInfo("London", 0, 1.6) },
            { Location.MidCostCity, new LocationInfo("Buenos Aires", -3, 0.6) },
            { Location.Suburban, new LocationInfo("Suburbia, USA", -5, 1.0) },
            { Location.LowCostRural, new LocationInfo("Rural Vietnam", 7, 0.4) },
            { Location.WarZone, new LocationInfo("Conflict Zone", 2, 0.2) },
            { Location.IsolatedIsland, new LocationInfo("New Zealand", 12, 1.2) }
        };
}
