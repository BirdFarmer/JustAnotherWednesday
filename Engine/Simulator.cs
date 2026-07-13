using System;
using System.Linq;
using System.Collections.Generic;

namespace JustAnotherWednesday
{
    public static class Simulator
    {
        private class PlayerRecord
        {
            public Scenario Scenario { get; }
            public Role Role { get; }
            public Location Location { get; }
            public GlobalTime Time { get; }
            public Action Action { get; }
            public Option Option { get; }
            public double Ppu { get; }

            public PlayerRecord(Scenario scenario, Role role, Location location, GlobalTime time, Action action, Option option, double ppu)
            {
                Scenario = scenario;
                Role = role;
                Location = location;
                Time = time;
                Action = action;
                Option = option;
                Ppu = ppu;
            }
        }

        public static void Run(int rounds = 10000)
        {
            var roundsData = SimulateRounds(rounds);
            PrintSummary(roundsData);
            PrintPrompt(roundsData.SelectMany(r => r).ToList());
        }

        private static List<PlayerRecord[]> SimulateRounds(int rounds)
        {
            var rand = new Random();
            var scenarios = Enum.GetValues(typeof(Scenario)).Cast<Scenario>().ToArray();
            var roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToArray();
            var locations = Enum.GetValues(typeof(Location)).Cast<Location>().ToArray();
            var times = Enum.GetValues(typeof(GlobalTime)).Cast<GlobalTime>().ToArray();
            var actions = Enum.GetValues(typeof(Action)).Cast<Action>().ToArray();
            var options = new[] { Option.A, Option.B, Option.C };

            var all = new List<PlayerRecord[]>();

            for (int i = 0; i < rounds; i++)
            {
                var scenario = scenarios[rand.Next(scenarios.Length)];
                var roundPlayers = new PlayerRecord[3];
                for (int p = 0; p < 3; p++)
                {
                    var role = roles[rand.Next(roles.Length)];
                    var location = locations[rand.Next(locations.Length)];
                    var time = times[rand.Next(times.Length)];
                    var action = actions[rand.Next(actions.Length)];
                    var opt = options[rand.Next(options.Length)];

                    // Use the existing PPUResolver (which uses Destiny + modifiers)
                    var ppu = PPUResolver.Resolve(action, scenario, opt);

                    roundPlayers[p] = new PlayerRecord(scenario, role, location, time, action, opt, ppu);
                }

                all.Add(roundPlayers);
            }

            return all;
        }

        private static void PrintSummary(List<PlayerRecord[]> roundsData)
        {
            var totalRounds = roundsData.Count;
            var totalPlayers = totalRounds * 3;

            Console.WriteLine($"\n3-player simulation rounds: {totalRounds} (total player-decisions: {totalPlayers})");

            var flat = roundsData.SelectMany(r => r).ToList();

            var options = new[] { Option.A, Option.B, Option.C };
            foreach (var opt in options)
            {
                var chosen = flat.Where(p => p.Option == opt).ToList();
                var count = chosen.Count;
                var avg = count > 0 ? chosen.Average(p => p.Ppu) : 0.0;
                Console.WriteLine($"Option {opt}: chosen {count} times ({(double)count * 100.0 / totalPlayers:0.00}%), avg PPU = {avg:F2}");
            }

            Console.WriteLine();

            // Wins per scenario
            var winsPerScenario = new Dictionary<Scenario, Dictionary<Option, int>>();
            foreach (Scenario s in Enum.GetValues(typeof(Scenario)))
                winsPerScenario[s] = new Dictionary<Option, int> { { Option.A, 0 }, { Option.B, 0 }, { Option.C, 0 } };

            int ties = 0;
            foreach (var round in roundsData)
            {
                var scenario = round[0].Scenario;
                var max = round.Max(p => p.Ppu);
                var winners = round.Where(p => Math.Abs(p.Ppu - max) < 1e-9).ToList();
                if (winners.Count == 1)
                {
                    var winOpt = winners[0].Option;
                    winsPerScenario[scenario][winOpt]++;
                }
                else ties++;
            }

            Console.WriteLine("\nWins by Scenario (most-winning option):");
            foreach (var kv in winsPerScenario)
            {
                var sc = kv.Key;
                var map = kv.Value;
                var topOpt = map.OrderByDescending(x => x.Value).First();
                Console.WriteLine($"  {sc}: Option {topOpt.Key} wins {topOpt.Value} rounds (A:{map[Option.A]} B:{map[Option.B]} C:{map[Option.C]})");
            }
            Console.WriteLine($"  Tied rounds: {ties}");
        }

        private static void PrintPrompt(List<PlayerRecord> allPlayers)
        {
            if (allPlayers == null || allPlayers.Count == 0) return;

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

            string locName(Location loc) => loc.ToString(); // keep simple for now

            string fmt(double v) => v.ToString("N2");

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("===PROMPT START===");
            sb.AppendLine("**The Wednesday Dilemma**");
            sb.AppendLine("");
            sb.AppendLine("**The Premise:**");
            sb.AppendLine("Next Wednesday, every person on Earth receives $1,000,000 deposited into their bank account.");
            sb.AppendLine("");
            sb.AppendLine("**The Twist:**");
            sb.AppendLine("You are one of a few special players who have a choice of when to receive your money:");
            sb.AppendLine("- **Option A:** $500,000 this Monday (2 days early)");
            sb.AppendLine("- **Option B:** $1,000,000 on Wednesday (with everyone else)");
            sb.AppendLine("- **Option C:** $2,000,000 next Monday (5 days late)");
            sb.AppendLine("");
            sb.AppendLine("Your choice, along with the global scenario, your personal role, location, and what you do with the money, determines your final PPU (Purchasing Power Units).");
            sb.AppendLine("");
            sb.AppendLine("---");
            sb.AppendLine("");
            sb.AppendLine("Scenario and player profiles for short realistic stories:");

            void AppendPlayer(string roleLabel, PlayerRecord r)
            {
                sb.AppendLine($"{roleLabel}:");
                sb.AppendLine($"Scenario: {r.Scenario} — {Desc(r.Scenario)}");
                sb.AppendLine($"Role: {r.Role}");
                sb.AppendLine($"Location: {locName(r.Location)}");
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
            sb.AppendLine("===PROMPT END===");

            var promptText = sb.ToString();

            // Write to file
            try
            {
                var path = System.IO.Path.Combine(Environment.CurrentDirectory, "sim_prompt.txt");
                System.IO.File.WriteAllText(path, promptText);
            }
            catch { }

            Console.WriteLine(promptText);
        }
    }
}