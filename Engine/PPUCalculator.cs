using JustAnotherWednesday.Data;

namespace JustAnotherWednesday.Engine;

public static class PPUCalculator
{
    public static double Calculate(
        Scenario scenario,
        Role role,
        Location location,
        GlobalTime time,
        Option option,
        Action action)
    {
        var scenarioDef = ScenarioDefinitions.Definitions[scenario];

        // Resolve cost-of-living locally to avoid dependency on LocationDefinitions
        double costOfLiving = location switch
        {
            Location.HighCostCity => 1.8,
            Location.MidCostCity => 1.2,
            Location.Suburban => 1.0,
            Location.LowCostRural => 0.5,
            Location.WarZone => 0.8,
            Location.IsolatedIsland => 1.3,
            _ => 1.0
        };

        // 1. Cash value based on Option and Scenario
        double cashValue = GetCashValue(option, scenario, scenarioDef);

        // 2. Asset value based on Action and Scenario
        double assetValue = GetAssetValue(action, scenarioDef);

        // 3. Location adjustment
        double locationMultiplier = 1.0 / costOfLiving;

        // 4. Final PPU
        return cashValue * assetValue * locationMultiplier * 1000;
    }

    private static double GetCashValue(Option option, Scenario scenario, ScenarioDefinition scenarioDef)
    {
        // Inflation multiplies by (1 - daily loss) per day of waiting
        double daysHeld = option switch
        {
            Option.A => 2,
            Option.B => 0,
            Option.C => 5,
            _ => 0
        };

        double baseValue = option switch
        {
            Option.A => 500000,
            Option.B => 1000000,
            Option.C => 2000000,
            _ => 0
        };

        // Cash decays with inflation
        double value = baseValue * Math.Pow(1 - scenarioDef.InflationRate, daysHeld);

        // Apply scenario-specific multipliers for early (A) and late (C) cash
        double scenarioFactor = 1.0;
        if (option == Option.A)
        {
            scenarioFactor = scenario switch
            {
                Scenario.PrintedMoney => 2.0,
                Scenario.TribalCollapse => 1.8,
                Scenario.ScorchedEarth => 1.5,
                _ => 1.0
            };
        }
        else if (option == Option.C)
        {
            scenarioFactor = scenario switch
            {
                Scenario.PrintedMoney => 0.3,
                Scenario.TribalCollapse => 0.4,
                Scenario.ScorchedEarth => 0.5,
                _ => 1.0
            };
        }

        return value * scenarioFactor;
    }

    private static double GetAssetValue(Action action, ScenarioDefinition scenario)
    {
        // Multiplier based on action and scenario
        double multiplier = action switch
        {
            Action.BuyGold => scenario.AssetMultiplierGold,
            Action.BuyRealEstate => scenario.AssetMultiplierRealEstate,
            Action.BuyStocks => scenario.AssetMultiplierStocks,
            Action.StartBusiness => scenario.AssetMultiplierBusiness,
            _ => 1.0
        };

        return multiplier;
    }
}