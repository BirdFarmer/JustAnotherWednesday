namespace JustAnotherWednesday.Data;

public record ScenarioDefinition(
    string Description,
    double InflationRate,        // Daily cash decay (e.g., 0.10 = 10% per day)
    double CashUtilityLoss,       // How much cash loses purchasing power
    double AssetMultiplierGold,
    double AssetMultiplierRealEstate,
    double AssetMultiplierStocks,
    double AssetMultiplierBusiness
);

public static class ScenarioDefinitions
{
    public static readonly Dictionary<Scenario, ScenarioDefinition> Definitions = new()
    {
        [Scenario.PrintedMoney] = new(
            Description: "Central banks printed $8 quadrillion. Hyperinflation.",
            InflationRate: 0.10,
            CashUtilityLoss: 0.30,
            AssetMultiplierGold: 2.0,
            AssetMultiplierRealEstate: 1.2,
            AssetMultiplierStocks: 0.8,
            AssetMultiplierBusiness: 0.6
        ),
        [Scenario.Redistributed] = new(
            Description: "Billionaires liquidated. Wealth transferred to everyone.",
            InflationRate: 0.0,
            CashUtilityLoss: 0.0,
            AssetMultiplierGold: 1.0,
            AssetMultiplierRealEstate: 1.5,
            AssetMultiplierStocks: 1.3,
            AssetMultiplierBusiness: 2.0
        ),
        [Scenario.TribalCollapse] = new(
            Description: "State armies collapsed. Warlords control territory.",
            InflationRate: 0.0,
            CashUtilityLoss: 0.20,
            AssetMultiplierGold: 1.8,
            AssetMultiplierRealEstate: 0.3,
            AssetMultiplierStocks: 0.2,
            AssetMultiplierBusiness: 0.1
        ),
        [Scenario.FeudalReset] = new(
            Description: "Rich kept physical assets. Feudalism returned.",
            InflationRate: 0.0,
            CashUtilityLoss: 0.0,
            AssetMultiplierGold: 1.5,
            AssetMultiplierRealEstate: 2.0,
            AssetMultiplierStocks: 1.8,
            AssetMultiplierBusiness: 0.8
        ),
        [Scenario.BrokeBillionaires] = new(
            Description: "The rich lost everything. Power shifted.",
            InflationRate: 0.0,
            CashUtilityLoss: 0.0,
            AssetMultiplierGold: 1.2,
            AssetMultiplierRealEstate: 1.0,
            AssetMultiplierStocks: 1.5,
            AssetMultiplierBusiness: 1.8
        ),
        [Scenario.ScorchedEarth] = new(
            Description: "Wars escalated. Infrastructure destroyed.",
            InflationRate: 0.05,
            CashUtilityLoss: 0.30,
            AssetMultiplierGold: 1.8,
            AssetMultiplierRealEstate: 0.2,
            AssetMultiplierStocks: 0.1,
            AssetMultiplierBusiness: 0.1
        ),
        [Scenario.LateBubble] = new(
            Description: "Panic spike Wednesday, crash by Monday.",
            InflationRate: 0.0,
            CashUtilityLoss: 0.10,
            AssetMultiplierGold: 1.0,
            AssetMultiplierRealEstate: 1.0,
            AssetMultiplierStocks: 2.5,
            AssetMultiplierBusiness: 0.5
        )
    };
}