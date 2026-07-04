namespace JustAnotherWednesday;

public enum Scenario
{
    // Sources
    PrintedMoney,      // New money printed → inflation
    Redistributed,     // Existing money moved → no inflation

    // Consequences
    TribalCollapse,    // State armies fail, militias form
    FeudalReset,       // Rich keep physical assets, become feudal lords
    BrokeBillionaires, // Rich are truly broke—power shifts to the people
    ScorchedEarth,     // Wars escalate
    LateBubble         // Panic spike Wednesday, crash by Monday
}

public enum Role
{
    Striver,          // 29, renting, $20k debt, $15k savings
    Landlord,         // 45, owns 3 apartments, $600k mortgage
    Retiree,          // 68, owns home, $1M portfolio
    Farmer,           // 52, owns land, $120k equipment loan
    GigWorker,        // 24, lives with parents, $8k credit card debt
    ShopOwner         // 38, rents storefront, $50k inventory loan
}

public enum Location
{
    HighCostCity,     // NYC, London, Tokyo
    MidCostCity,      // Mexico City, Bangkok
    Suburban,         // Commuter belt, reliant on infrastructure
    LowCostRural,     // Rural Vietnam, India
    WarZone,          // Active conflict region
    IsolatedIsland    // New Zealand, Iceland (supply chain issues)
}

public enum GlobalTime
{
    SyncUTC,          // Everyone gets it at the same moment (09:00 UTC)
    SyncLocal,        // Everyone gets it at 09:00 in their own timezone
    MidnightUTC,      // 00:00 UTC—banks closed, markets shut
    MarketCloseUTC,   // 16:00 UTC—markets about to close in New York
    Random            // Chaotic distribution (some get it early, some late)
}

public enum Option
{
    A,  // $500k Monday (2 days early)
    B,  // $1M Wednesday (with everyone else)
    C   // $2M Next Monday (5 days late)
}

public enum Action
{
    BuyRealEstate,
    BuyGold,
    BuyStocks,
    PayOffDebt,
    EssentialSupplies,
    BribeProtection,
    SkillsEducation,
    EscapeVehicle,
    StartBusiness,
    HoldCash
}