using System.Collections.Generic;

namespace JustAnotherWednesday;

public static class ActionModifiers
{
    private static readonly Dictionary<Action, Dictionary<Scenario, double>> Modifiers
        = new()
    {
        { Action.BuyRealEstate, new() {
                { Scenario.PrintedMoney, 0.6 },{ Scenario.Redistributed, 0.8 },{ Scenario.TribalCollapse, 0.2 },{ Scenario.FeudalReset, 0.5 },{ Scenario.BrokeBillionaires, 0.9 },{ Scenario.ScorchedEarth, 0.3 },{ Scenario.LateBubble, 0.7 }
            } },
        { Action.BuyGold, new() {
                { Scenario.PrintedMoney, 2.0 },{ Scenario.Redistributed, 1.6 },{ Scenario.TribalCollapse, 1.2 },{ Scenario.FeudalReset, 1.8 },{ Scenario.BrokeBillionaires, 1.4 },{ Scenario.ScorchedEarth, 1.6 },{ Scenario.LateBubble, 1.1 }
            } },
        { Action.BuyStocks, new() {
                { Scenario.PrintedMoney, 0.7 },{ Scenario.Redistributed, 1.2 },{ Scenario.TribalCollapse, 0.3 },{ Scenario.FeudalReset, 0.6 },{ Scenario.BrokeBillionaires, 1.1 },{ Scenario.ScorchedEarth, 0.4 },{ Scenario.LateBubble, 1.5 }
            } },
        { Action.PayOffDebt, new() {
                { Scenario.PrintedMoney, 0.9 },{ Scenario.Redistributed, 1.0 },{ Scenario.TribalCollapse, 0.8 },{ Scenario.FeudalReset, 0.95 },{ Scenario.BrokeBillionaires, 1.0 },{ Scenario.ScorchedEarth, 0.6 },{ Scenario.LateBubble, 1.0 }
            } },
        { Action.EssentialSupplies, new() {
                { Scenario.PrintedMoney, 0.8 },{ Scenario.Redistributed, 0.9 },{ Scenario.TribalCollapse, 1.7 },{ Scenario.FeudalReset, 1.2 },{ Scenario.BrokeBillionaires, 1.3 },{ Scenario.ScorchedEarth, 1.9 },{ Scenario.LateBubble, 0.9 }
            } },
        { Action.BribeProtection, new() {
                { Scenario.PrintedMoney, 0.5 },{ Scenario.Redistributed, 0.6 },{ Scenario.TribalCollapse, 1.8 },{ Scenario.FeudalReset, 1.4 },{ Scenario.BrokeBillionaires, 0.7 },{ Scenario.ScorchedEarth, 1.6 },{ Scenario.LateBubble, 0.6 }
            } },
        { Action.SkillsEducation, new() {
                { Scenario.PrintedMoney, 1.1 },{ Scenario.Redistributed, 1.2 },{ Scenario.TribalCollapse, 0.7 },{ Scenario.FeudalReset, 1.3 },{ Scenario.BrokeBillionaires, 1.5 },{ Scenario.ScorchedEarth, 0.9 },{ Scenario.LateBubble, 1.4 }
            } },
        { Action.EscapeVehicle, new() {
                { Scenario.PrintedMoney, 0.4 },{ Scenario.Redistributed, 0.7 },{ Scenario.TribalCollapse, 1.9 },{ Scenario.FeudalReset, 0.8 },{ Scenario.BrokeBillionaires, 0.6 },{ Scenario.ScorchedEarth, 1.8 },{ Scenario.LateBubble, 0.5 }
            } },
        { Action.StartBusiness, new() {
                { Scenario.PrintedMoney, 0.6 },{ Scenario.Redistributed, 1.3 },{ Scenario.TribalCollapse, 0.4 },{ Scenario.FeudalReset, 1.1 },{ Scenario.BrokeBillionaires, 1.6 },{ Scenario.ScorchedEarth, 0.5 },{ Scenario.LateBubble, 1.2 }
            } },
        { Action.HoldCash, new() {
                { Scenario.PrintedMoney, 0.3 },{ Scenario.Redistributed, 0.9 },{ Scenario.TribalCollapse, 0.5 },{ Scenario.FeudalReset, 0.4 },{ Scenario.BrokeBillionaires, 1.2 },{ Scenario.ScorchedEarth, 0.2 },{ Scenario.LateBubble, 1.0 }
            } }
    };

    public static double GetMultiplier(Action action, Scenario scenario)
    {
        if (Modifiers.TryGetValue(action, out var map) && map.TryGetValue(scenario, out var m))
            return m;

        return 1.0;
    }
}
