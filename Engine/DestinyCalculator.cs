using JustAnotherWednesday.Data;

namespace JustAnotherWednesday;

public static class DestinyCalculator
{
    public static int Calculate(Scenario scenario, Role role, Location location, GlobalTime globalTime, Option option, Action action)
    {
        var sum = 0;
        sum += Fibonacci.ScenarioMap[scenario];
        sum += Fibonacci.RoleMap[role];
        sum += Fibonacci.LocationMap[location];
        sum += Fibonacci.GlobalTimeMap[globalTime];
        sum += Fibonacci.OptionMap[option];
        sum += Fibonacci.ActionMap[action];
        return sum;
    }
}
