using System;
using System.Collections.Generic;

namespace JustAnotherWednesday.Data;

public static class WorldTimeline
{
    // Days: 0=Monday,1=Tuesday,2=Wednesday,3=Thursday,4=NextMonday
    private static readonly Dictionary<Scenario, double[]> PriceMultipliers = new();
    private static readonly Dictionary<Scenario, string[]> MarketConditions = new();

    static WorldTimeline()
    {
        // Example multipliers and market condition strings per scenario
        void Add(Scenario s, double[] mul, string[] cond)
        {
            if (mul.Length != 5 || cond.Length != 5) throw new ArgumentException("Must provide 5 days");
            PriceMultipliers[s] = mul;
            MarketConditions[s] = cond;
        }

        Add(Scenario.PrintedMoney,
            new double[] { 0.95, 0.97, 1.00, 1.10, 1.20 },
            new string[] { "Rising", "Rising", "Inflation", "Hot", "Hyper" });

        Add(Scenario.Redistributed,
            new double[] { 1.00, 1.02, 1.00, 0.98, 1.01 },
            new string[] { "Stable", "Stable", "Equalized", "Adjustment", "Stable" });

        Add(Scenario.TribalCollapse,
            new double[] { 1.20, 1.50, 0.30, 0.20, 0.15 },
            new string[] { "Panic", "Panic", "Collapse", "Anarchy", "Aftermath" });

        Add(Scenario.FeudalReset,
            new double[] { 0.80, 0.85, 1.00, 1.10, 1.50 },
            new string[] { "Shock", "Consolidation", "Feudal", "RentsUp", "Entrenched" });

        Add(Scenario.BrokeBillionaires,
            new double[] { 1.10, 1.05, 1.00, 0.95, 0.90 },
            new string[] { "WealthShift", "WealthShift", "Adjustment", "Stabilizing", "Balanced" });

        Add(Scenario.ScorchedEarth,
            new double[] { 0.50, 0.40, 0.30, 0.20, 0.10 },
            new string[] { "Devastation", "Devastation", "Collapse", "Scarcity", "Scarcity" });

        Add(Scenario.LateBubble,
            new double[] { 0.90, 0.95, 1.00, 0.70, 0.60 },
            new string[] { "Calm", "Froth", "Bubble", "Burst", "Aftershock" });
    }

    public static double GetPriceMultiplier(Scenario scenario, int dayIndex)
    {
        if (!PriceMultipliers.TryGetValue(scenario, out var arr)) return 1.0;
        if (dayIndex < 0) dayIndex = 0;
        if (dayIndex >= arr.Length) dayIndex = arr.Length - 1;
        return arr[dayIndex];
    }

    public static string GetMarketCondition(Scenario scenario, int dayIndex)
    {
        if (!MarketConditions.TryGetValue(scenario, out var arr)) return "Unknown";
        if (dayIndex < 0) dayIndex = 0;
        if (dayIndex >= arr.Length) dayIndex = arr.Length - 1;
        return arr[dayIndex];
    }

    public static double GetRealValue(Option option, Scenario scenario)
    {
        var day = OptionToDayIndex(option);
        double nominal = OptionToNominal(option);
        var mult = GetPriceMultiplier(scenario, day);
        // Apply scenario-specific cash multipliers for early (A) and late (C) cash
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

        return nominal * mult * scenarioFactor; // purchasing power in nominal currency units
    }

    public static double GetActionEffectiveness(Action action, Scenario scenario, int dayIndex)
    {
        // Base effectiveness per action (example values) modified by scenario/day
        double baseEff = action switch
        {
            Action.BuyRealEstate => 0.5,
            Action.BuyGold => 1.8,
            Action.BuyStocks => 1.2,
            Action.PayOffDebt => 1.0,
            Action.EssentialSupplies => 1.0,
            Action.BribeProtection => 0.9,
            Action.SkillsEducation => 1.1,
            Action.EscapeVehicle => 1.0,
            Action.StartBusiness => 1.3,
            Action.HoldCash => 1.0,
            _ => 1.0
        };

        // modify by scenario/day: for PrintedMoney, later days worse for cash, good for assets
        var priceMult = GetPriceMultiplier(scenario, dayIndex);
        // Heuristic: effectiveness scales inversely with price multiplier for HoldCash, and proportional for assets
        double modifier = action switch
        {
            Action.BuyGold => 1.0 + (1.0 - priceMult) * 0.8,
            Action.BuyRealEstate => 1.0 + (priceMult - 1.0) * 0.25,
            Action.BuyStocks => 1.0 + (priceMult - 1.0) * 0.2,
            Action.HoldCash => 1.0 / Math.Max(0.2, priceMult),
            _ => 1.0
        };

        // Cap modifier to a reasonable range to avoid extreme multipliers
        modifier = Math.Max(0.1, Math.Min(modifier, 3.0));

        return Math.Max(0.0, baseEff * modifier);
    }

    private static int OptionToDayIndex(Option option) => option switch
    {
        Option.A => 0, // Monday
        Option.B => 2, // Wednesday
        Option.C => 4, // Next Monday
        _ => 2
    };

    private static double OptionToNominal(Option option) => option switch
    {
        Option.A => 500_000.0,
        Option.B => 1_000_000.0,
        Option.C => 2_000_000.0,
        _ => 1_000_000.0
    };
}
