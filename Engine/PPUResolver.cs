namespace JustAnotherWednesday;

using JustAnotherWednesday.Data;

public static class PPUResolver
{
    // Resolve returns real purchasing power (in currency units) after world timeline and action effectiveness
    public static double Resolve(Action action, Scenario scenario, Option option)
    {
        // day determined by option
        var day = option switch
        {
            Option.A => 0,
            Option.B => 2,
            Option.C => 4,
            _ => 2
        };

        var nominalReal = WorldTimeline.GetRealValue(option, scenario);
        var actionEffect = WorldTimeline.GetActionEffectiveness(action, scenario, day);
        return nominalReal * actionEffect;
    }
}
