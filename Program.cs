using System;
using System.Linq;

namespace JustAnotherWednesday;

class Program
{
	static void Main(string[] args)
	{
		if (args.Length > 0 && args[0] == "sim")
		{
			Simulator.Run(10000);
			return;
		}

		Console.WriteLine("The Wednesday Dilemma - Menu");
		Console.WriteLine("1) Run Simulator");
		Console.WriteLine("2) Human Mode");
		Console.WriteLine("3) Human Duel (you vs random)");
		Console.Write("Choose: ");
		var key = Console.ReadLine();
		if (key == "1")
		{
			Simulator.Run(10000);
		}
		else if (key == "2")
		{
			RunHumanMode();
		}
		else if (key == "3")
		{
			RunHumanDuel();
		}
		else
		{
			Console.WriteLine("Unknown choice. Exiting.");
		}
	}

	static void RunHumanMode()
	{
		var role = AskEnum<Role>("Choose Role");
		var location = AskEnum<Location>("Choose Location");
		var globalTime = AskEnum<GlobalTime>("Choose Global Time");
		var action = AskEnum<Action>("Choose Action");

		var options = Enum.GetValues(typeof(Option)).Cast<Option>().ToArray();
		Console.WriteLine();
		Console.WriteLine("Results for your choices:");
		for (int i = 0; i < options.Length; i++)
		{
			var opt = options[i];
			int destiny = DestinyCalculator.Calculate(Scenario.PrintedMoney, role, location, globalTime, opt, action);
			// We don't know the hidden scenario; show per-scenario table
			Console.WriteLine($"Option {opt}:");
			foreach (Scenario sc in Enum.GetValues(typeof(Scenario)))
			{
				int d = DestinyCalculator.Calculate(sc, role, location, globalTime, opt, action);
				var ppu = PPUResolver.Resolve(action, sc, opt);
				int dayIdx = opt == Option.A ? 0 : (opt == Option.B ? 2 : 4);
				Console.WriteLine($"  If {sc}: Destiny={d} PPU={ppu:F2} (market={JustAnotherWednesday.Data.WorldTimeline.GetMarketCondition(sc, dayIdx)})");
			}
			Console.WriteLine();
		}

		Console.WriteLine("Human session complete.");
	}

	static void RunHumanDuel()
	{
		var role1 = AskEnum<Role>("Player 1 - Choose Role");
		var location1 = AskEnum<Location>("Player 1 - Choose Location");
		var globalTime1 = AskEnum<GlobalTime>("Player 1 - Choose Global Time");
		var action1 = AskEnum<Action>("Player 1 - Choose Action");

		// randomize player 2
		var rand = new System.Random();
		var roleVals = Enum.GetValues(typeof(Role)).Cast<Role>().ToArray();
		var locVals = Enum.GetValues(typeof(Location)).Cast<Location>().ToArray();
		var timeVals = Enum.GetValues(typeof(GlobalTime)).Cast<GlobalTime>().ToArray();
		var actVals = Enum.GetValues(typeof(Action)).Cast<Action>().ToArray();

		var role2 = roleVals[rand.Next(roleVals.Length)];
		var location2 = locVals[rand.Next(locVals.Length)];
		var globalTime2 = timeVals[rand.Next(timeVals.Length)];
		var action2 = actVals[rand.Next(actVals.Length)];

		Console.WriteLine();
		Console.WriteLine($"Player 2 (random) - Role: {role2}, Location: {location2}, Time: {globalTime2}, Action: {action2}");

		// Show location real info if available
		if (Data.LocationInfos.Map.TryGetValue(location1, out var info1))
			Console.WriteLine($"Player1 location: {info1.DisplayName}, UTC{(info1.UtcOffsetHours>=0?"+":"")}{info1.UtcOffsetHours}, COL mult={info1.CostOfLivingMultiplier}");
		if (Data.LocationInfos.Map.TryGetValue(location2, out var info2))
			Console.WriteLine($"Player2 location: {info2.DisplayName}, UTC{(info2.UtcOffsetHours>=0?"+":"")}{info2.UtcOffsetHours}, COL mult={info2.CostOfLivingMultiplier}");

		var options = Enum.GetValues(typeof(Option)).Cast<Option>().ToArray();
		var scenarios = Enum.GetValues(typeof(Scenario)).Cast<Scenario>().ToArray();

		// For each option, compare PPUs across scenarios and count wins
		for (int i = 0; i < options.Length; i++)
		{
			var opt = options[i];
			int p1wins = 0, p2wins = 0, ties = 0;
			double p1avg = 0, p2avg = 0;
			foreach (var sc in scenarios)
			{
				int d1 = DestinyCalculator.Calculate(sc, role1, location1, globalTime1, opt, action1);
				int d2 = DestinyCalculator.Calculate(sc, role2, location2, globalTime2, opt, action2);
				double ppu1 = PPUResolver.Resolve(action1, sc, opt);
				double ppu2 = PPUResolver.Resolve(action2, sc, opt);
				p1avg += ppu1; p2avg += ppu2;
				if (ppu1 > ppu2) p1wins++; else if (ppu2 > ppu1) p2wins++; else ties++;
			}
			p1avg /= scenarios.Length; p2avg /= scenarios.Length;
			Console.WriteLine($"Option {opt}: P1 avg PPU {p1avg:F2}, P2 avg PPU {p2avg:F2} — P1wins:{p1wins} P2wins:{p2wins} ties:{ties}");
		}

		Console.WriteLine("Duel complete.");
	}

	static T AskEnum<T>(string prompt) where T : struct, Enum
	{
		var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
		Console.WriteLine(prompt + ":");
		for (int i = 0; i < values.Length; i++) Console.WriteLine($"  {i+1}) {values[i]}");
		Console.Write("Choose number: ");
		var line = Console.ReadLine();
		if (int.TryParse(line, out var n) && n >= 1 && n <= values.Length) return values[n-1];
		Console.WriteLine("Invalid choice, using first.");
		return values[0];
	}
}
