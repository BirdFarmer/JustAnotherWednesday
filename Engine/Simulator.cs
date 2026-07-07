using System;
using System.Linq;

namespace JustAnotherWednesday;

public static class Simulator
{
    public static void Run(int rounds = 10000)
    {
    // record for each individual player decision across all rounds
    record PlayerRecord(Scenario Scenario, Role Role, Location Location, GlobalTime Time, Action Action, Option Option, double Ppu);

    var allPlayers = new System.Collections.Generic.List<PlayerRecord>();
        var rand = new Random();
        var scenarios = Enum.GetValues(typeof(Scenario)).Cast<Scenario>().ToArray();
        var roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToArray();
        var locations = Enum.GetValues(typeof(Location)).Cast<Location>().ToArray();
        var times = Enum.GetValues(typeof(GlobalTime)).Cast<GlobalTime>().ToArray();
        var actions = Enum.GetValues(typeof(Action)).Cast<Action>().ToArray();
        var options = new[] { Option.A, Option.B, Option.C };

        var optionChosenCounts = new long[3];
        var optionChosenPpuSum = new double[3];
        var optionWins = new long[3];
        var roleAgg = new System.Collections.Generic.Dictionary<Role, (double sum, long count)>();
        var locAgg = new System.Collections.Generic.Dictionary<Location, (double sum, long count)>();
        var actionAgg = new System.Collections.Generic.Dictionary<Action, (double sum, long count)>();
        var comboAgg = new System.Collections.Generic.Dictionary<string, (double sum, long count)>();

        for (int i = 0; i < rounds; i++)
        {
            var scenario = scenarios[rand.Next(scenarios.Length)];

            // per-round players
            var playerOptions = new Option[3];
            var playerPpus = new double[3];

            var playerRoles = new Role[3];
            var playerLocations = new Location[3];
            var playerTimes = new GlobalTime[3];
            var playerActions = new Action[3];

            for (int p = 0; p < 3; p++)
            {
                var role = roles[rand.Next(roles.Length)];
                var location = locations[rand.Next(locations.Length)];
                var time = times[rand.Next(times.Length)];
                var action = actions[rand.Next(actions.Length)];
                var opt = options[rand.Next(options.Length)];

                playerRoles[p] = role;
                playerLocations[p] = location;
                playerTimes[p] = time;
                playerActions[p] = action;
                playerOptions[p] = opt;

                // destiny is still calculable but world timeline determines real value
                // var destiny = DestinyCalculator.Calculate(scenario, role, location, time, opt, action);
                var ppu = PPUResolver.Resolve(action, scenario, opt);
                playerPpus[p] = ppu;

                // store full player record for later analysis
                allPlayers.Add(new PlayerRecord(scenario, role, location, time, action, opt, ppu));

                int oi = Array.IndexOf(options, opt);
                optionChosenCounts[oi]++;
                optionChosenPpuSum[oi] += ppu;

                // aggregate by role/location/action/combo
                if (!roleAgg.ContainsKey(role)) roleAgg[role] = (0, 0);
                var rr = roleAgg[role]; rr.sum += ppu; rr.count++; roleAgg[role] = rr;

                if (!locAgg.ContainsKey(location)) locAgg[location] = (0, 0);
                var ll = locAgg[location]; ll.sum += ppu; ll.count++; locAgg[location] = ll;

                if (!actionAgg.ContainsKey(action)) actionAgg[action] = (0, 0);
                var aa = actionAgg[action]; aa.sum += ppu; aa.count++; actionAgg[action] = aa;

                var comboKey = $"{role}|{location}|{action}";
                if (!comboAgg.ContainsKey(comboKey)) comboAgg[comboKey] = (0, 0);
                var cc = comboAgg[comboKey]; cc.sum += ppu; cc.count++; comboAgg[comboKey] = cc;
            }

            // Determine winner(s)
            double roundMax = playerPpus.Max();
            var winners = playerPpus.Select((v, idx) => (v, idx)).Where(t => Math.Abs(t.v - roundMax) < 1e-9).Select(t => t.idx).ToArray();
            if (winners.Length == 1)
            {
                int winner = winners[0];
                var winnerOpt = playerOptions[winner];
                var oi = Array.IndexOf(options, winnerOpt);
                optionWins[oi]++;
            }
            // ties count as no wins
        }

        long totalSelections = optionChosenCounts.Sum();

        Console.WriteLine($"3-player simulation rounds: {rounds} (total player-decisions: {totalSelections})");
        for (int o = 0; o < 3; o++)
        {
            var opt = options[o];
            var chosen = optionChosenCounts[o];
            var avgPpu = chosen > 0 ? optionChosenPpuSum[o] / chosen : 0.0;
            var wins = optionWins[o];
            var winRate = chosen > 0 ? wins * 100.0 / chosen : 0.0;
            Console.WriteLine($"Option {opt}: chosen {chosen} times ({chosen * 100.0 / totalSelections:0.00}%), avg PPU = {avgPpu:F2}, wins when chosen = {wins} ({winRate:0.00}% of choices)");
        }

        // Average PPU by Role
        Console.WriteLine("\nAverage PPU by Role:");
        foreach (var kv in roleAgg.OrderByDescending(k => k.Value.sum / k.Value.count))
        {
            Console.WriteLine($"  {kv.Key}: avg PPU = {kv.Value.sum / kv.Value.count:F2} (n={kv.Value.count})");
        }

        // Average PPU by Location
        Console.WriteLine("\nAverage PPU by Location:");
        foreach (var kv in locAgg.OrderByDescending(k => k.Value.sum / k.Value.count))
        {
            Console.WriteLine($"  {kv.Key}: avg PPU = {kv.Value.sum / kv.Value.count:F2} (n={kv.Value.count})");
        }

        // Average PPU by Action
        Console.WriteLine("\nAverage PPU by Action:");
        foreach (var kv in actionAgg.OrderByDescending(k => k.Value.sum / k.Value.count))
        {
            Console.WriteLine($"  {kv.Key}: avg PPU = {kv.Value.sum / kv.Value.count:F2} (n={kv.Value.count})");
        }

        // Top/Bottom combos
        var comboAvgs = comboAgg.Select(kv => new { Combo = kv.Key, Avg = kv.Value.sum / kv.Value.count, Count = kv.Value.count }).ToList();
        var top3 = comboAvgs.OrderByDescending(x => x.Avg).Take(3).ToArray();
        var bottom3 = comboAvgs.OrderBy(x => x.Avg).Take(3).ToArray();

        Console.WriteLine("\nTop 3 Role|Location|Action combos:");
        foreach (var t in top3) Console.WriteLine($"  {t.Combo} — avg PPU {t.Avg:F2} (n={t.Count})");

        Console.WriteLine("\nBottom 3 Role|Location|Action combos:");
        foreach (var t in bottom3) Console.WriteLine($"  {t.Combo} — avg PPU {t.Avg:F2} (n={t.Count})");

        // Identify best, worst, and mid players
        if (allPlayers.Count > 0)
        {
            var best = allPlayers.OrderByDescending(x => x.Ppu).First();
            var worst = allPlayers.OrderBy(x => x.Ppu).First();
            var avgPpu = allPlayers.Average(x => x.Ppu);
            var mid = allPlayers.OrderBy(x => Math.Abs(x.Ppu - avgPpu)).First();

            string Desc(Scenario s) => s switch
            {
                Scenario.PrintedMoney => "A sudden large amount of new money is injected into the economy, spiking inflation and market volatility.",
                Scenario.Redistributed => "Existing wealth is rapidly redistributed, changing who holds liquid assets without massive inflation.",
                Scenario.TribalCollapse => "Central authority collapses, local militias and informal powers take control.",
                Scenario.FeudalReset => "Wealth concentrates into land and physical assets as society reorders into feudal structures.",
                Scenario.BrokeBillionaires => "Previously wealthy elites lose liquidity and influence; power shifts toward common people.",
                Scenario.ScorchedEarth => "Conflicts escalate, infrastructure is destroyed and supply chains fail.",
                Scenario.LateBubble => "A panic-driven spike occurs mid-week followed by a crash the following week.",
                _ => "An unusual economic scenario unfolds."
            };

            string locName(Location loc)
            {
                if (JustAnotherWednesday.Data.LocationInfos.Map.TryGetValue(loc, out var info)) return info.DisplayName;
                return loc.ToString();
            }

            string fmt(double v) => v.ToString("N2");

            var sb = new System.Text.StringBuilder();
            // Build a self-contained prompt (only the prompt text)
            sb.AppendLine($"Scenario and player profiles for short realistic stories:");

            void AppendPlayer(string roleLabel, PlayerRecord r)
            {
                sb.AppendLine($"{roleLabel}:");
                sb.AppendLine($"Scenario: {r.Scenario} — {Desc(r.Scenario)}");
                sb.AppendLine($"Role: {r.Role}");
                sb.AppendLine($"Location: {locName(r.Location)} ({r.Location})");
                sb.AppendLine($"GlobalTime: {r.Time}");
                sb.AppendLine($"Action: {r.Action}");
                sb.AppendLine($"Option: {r.Option}");
                sb.AppendLine($"Final PPU: {fmt(r.Ppu)}");
                sb.AppendLine();
            }

            AppendPlayer("Winner", best);
            AppendPlayer("Mid", mid);
            AppendPlayer("Loser", worst);

            sb.AppendLine("Instructions: For each of the three players above (Winner, Mid, Loser), write a short realistic story of 2-3 paragraphs that explains how the listed Scenario and their choices (Role, Location, GlobalTime, Action, Option) led to their outcome. Focus on believable human details, motivations, and concrete events tied to the scenario. Use the location names provided for atmospheric detail. Do not add any analysis or meta commentary — only the three short stories, one per player.");

            // Print only the prompt so it's ready to copy
            Console.WriteLine();
            Console.WriteLine(sb.ToString());
        }
    }
}
