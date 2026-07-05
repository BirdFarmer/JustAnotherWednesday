using JustAnotherWednesday;

namespace JustAnotherWednesday.Data;

public static class Fibonacci
{
    public static readonly Dictionary<Scenario, int> ScenarioMap = new()
    {
        { Scenario.PrintedMoney, 1 },
        { Scenario.Redistributed, 2 },
        { Scenario.TribalCollapse, 3 },
        { Scenario.FeudalReset, 5 },
        { Scenario.BrokeBillionaires, 8 },
        { Scenario.ScorchedEarth, 13 },
        { Scenario.LateBubble, 21 }
    };

    public static readonly Dictionary<Role, int> RoleMap = new()
    {
        { Role.Striver, 34 },
        { Role.Landlord, 55 },
        { Role.Retiree, 89 },
        { Role.Farmer, 144 },
        { Role.GigWorker, 233 },
        { Role.ShopOwner, 377 }
    };

    public static readonly Dictionary<Location, int> LocationMap = new()
    {
        { Location.HighCostCity, 610 },
        { Location.MidCostCity, 987 },
        { Location.Suburban, 1597 },
        { Location.LowCostRural, 2584 },
        { Location.WarZone, 4181 },
        { Location.IsolatedIsland, 6765 }
    };

    public static readonly Dictionary<GlobalTime, int> GlobalTimeMap = new()
    {
        { GlobalTime.SyncUTC, 10946 },
        { GlobalTime.SyncLocal, 17711 },
        { GlobalTime.MidnightUTC, 28657 },
        { GlobalTime.MarketCloseUTC, 46368 },
        { GlobalTime.Random, 75025 }
    };

    public static readonly Dictionary<Option, int> OptionMap = new()
    {
        { Option.A, 121393 },
        { Option.B, 196418 },
        { Option.C, 317811 }
    };

    public static readonly Dictionary<Action, int> ActionMap = new()
    {
        { Action.BuyRealEstate, 514229 },
        { Action.BuyGold, 832040 },
        { Action.BuyStocks, 1346269 },
        { Action.PayOffDebt, 2178309 },
        { Action.EssentialSupplies, 3524578 },
        { Action.BribeProtection, 5702887 },
        { Action.SkillsEducation, 9227465 },
        { Action.EscapeVehicle, 14930352 },
        { Action.StartBusiness, 24157817 },
        { Action.HoldCash, 39088169 }
    };
}