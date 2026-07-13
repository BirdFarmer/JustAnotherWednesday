using System.Text;
using System.Text.Json;

namespace JustAnotherWednesday;

public static class HumanDuel
{
    // --- GameAction enum ---
    private enum GameAction
    {
        AcquireTransport,
        SecureFood,
        StrengthenShelter,
        AcquireEnergy,
        BuildInfluence,
        PreserveLiquidity
    }

    // --- Player State ---
    private class PlayerState
    {
        public int Liquidity { get; set; } = 50;
        public int Food { get; set; } = 50;
        public int Shelter { get; set; } = 50;
        public int Security { get; set; } = 50;
        public int Mobility { get; set; } = 50;
        public int Influence { get; set; } = 50;
        public int Energy { get; set; } = 50;
        public Option MoneyOption { get; set; } = Option.B;
        public int CashReceived { get; set; } = 0;
        public bool MoneyReceived { get; set; } = false;

        public string GetSummary()
        {
            return $"💧 Liquidity: {Liquidity}  🍖 Food: {Food}  🏠 Shelter: {Shelter}  🔒 Security: {Security}  🚗 Mobility: {Mobility}  🤝 Influence: {Influence}  ⚡ Energy: {Energy}";
        }
    }

    // --- Location database ---
    private static readonly Dictionary<Location, List<string>> LocationNames = new()
    {
        { Location.HighCostCity, new List<string> {
            "New York City, USA", "London, UK", "Singapore", "Tokyo, Japan",
            "Sydney, Australia", "Paris, France", "Hong Kong, China",
            "Dubai, UAE", "San Francisco, USA", "Zurich, Switzerland"
        } },
        { Location.MidCostCity, new List<string> {
            "Buenos Aires, Argentina", "Prague, Czech Republic", "Omaha, USA",
            "Kuala Lumpur, Malaysia", "Mexico City, Mexico", "Cape Town, South Africa",
            "Bangkok, Thailand", "Ho Chi Minh City, Vietnam", "Bogota, Colombia",
            "Lisbon, Portugal", "Helsingborg, Sweden"
        } },
        { Location.Suburban, new List<string> {
            "suburban Ohio, USA", "suburban Surrey, UK", "suburban Ontario, Canada",
            "suburban Melbourne, Australia", "suburban Auckland, New Zealand",
            "suburban Florida, USA", "suburban California, USA",
            "suburban New South Wales, Australia", "suburban British Columbia, Canada"
        } },
        { Location.WarZone, new List<string> {
            "Donetsk, Ukraine", "Mosul, Iraq", "Gaza City, Palestine",
            "Aleppo, Syria", "Kabul, Afghanistan", "Mogadishu, Somalia",
            "Kyiv, Ukraine", "Sanaa, Yemen", "Khartoum, Sudan", "Port-au-Prince, Haiti"
        } },
        { Location.LowCostRural, new List<string> {
            "rural Kansas, USA", "rural Vietnam (Mekong Delta)",
            "rural Brazil (Mato Grosso)", "rural Kenya (Rift Valley)",
            "rural Romania (Transylvania)", "rural Mongolia (steppes)",
            "rural Bolivia (Altiplano)", "rural Philippines (Mindanao)",
            "rural Poland (Masuria)"
        } },
        { Location.IsolatedIsland, new List<string> {
            "Easter Island, Chile", "St. Helena, South Atlantic",
            "Tasmania, Australia", "Santorini, Greece",
            "Marshall Islands", "Falkland Islands",
            "Svalbard, Norway", "Faroe Islands, Denmark",
            "Chatham Islands, New Zealand", "Koh Phangan, Thailand"
        } }
    };

    // --- Timezone helper ---
    private static int GetUtcOffset(string location)
    {
        if (location.Contains("Tokyo") || location.Contains("Japan")) return 9;
        if (location.Contains("Sydney") || location.Contains("Melbourne") || location.Contains("NSW")) return 11;
        if (location.Contains("Auckland") || location.Contains("Chatham")) return 12;
        if (location.Contains("Singapore") || location.Contains("Hong Kong") || location.Contains("Beijing") || location.Contains("Kuala Lumpur")) return 8;
        if (location.Contains("Bangkok") || location.Contains("Ho Chi Minh") || location.Contains("Vietnam") || location.Contains("Koh Phangan")) return 7;
        if (location.Contains("Dubai") || location.Contains("Moscow")) return 4;
        if (location.Contains("Cape Town") || location.Contains("Khartoum") || location.Contains("Sanaa")) return 2;
        if (location.Contains("London") || location.Contains("UK") || location.Contains("St. Helena") || location.Contains("Lisbon") || location.Contains("Faroe")) return 0;
        if (location.Contains("Paris") || location.Contains("Zurich") || location.Contains("Prague") || location.Contains("Rome") || location.Contains("Berlin")) return 1;
        if (location.Contains("Helsingborg") || location.Contains("Sweden")) return 1;
        if (location.Contains("Buenos Aires") || location.Contains("Brazil") || location.Contains("Sao Paulo") || location.Contains("Bogota")) return -3;
        if (location.Contains("New York") || location.Contains("Ohio") || location.Contains("Omaha") || location.Contains("Florida") || location.Contains("Ontario")) return -5;
        if (location.Contains("California") || location.Contains("San Francisco") || location.Contains("British Columbia")) return -8;
        if (location.Contains("Mexico City")) return -6;
        if (location.Contains("Easter Island") || location.Contains("Falkland")) return -6;
        if (location.Contains("Santorini") || location.Contains("Greece")) return 2;
        if (location.Contains("Mogadishu")) return 3;
        if (location.Contains("Kyiv") || location.Contains("Donetsk") || location.Contains("Aleppo") || location.Contains("Gaza")) return 2;
        if (location.Contains("Kabul")) return 4;
        return 0;
    }

    private static string GetMondayMorningLocalInfo(string location)
    {
        return $"It's Monday morning, approximately 8:00 AM local time in {location}. Banks are open. Markets are open.";
    }

    private static string GetInfrastructureContext(Location locationEnum)
    {
        return locationEnum switch
        {
            Location.HighCostCity => "High-speed internet, reliable power, functioning banks, ATMs everywhere, 24/7 emergency services.",
            Location.MidCostCity => "Good internet, mostly reliable power, banks and ATMs available, but slower outside downtown.",
            Location.Suburban => "Good residential infrastructure, reliable power, internet, nearby shopping centers, but car-dependent.",
            Location.WarZone => "Destroyed infrastructure. No working banks. Internet is spotty or offline. Power is intermittent. Looting and curfews common.",
            Location.LowCostRural => "Limited infrastructure. Dirt roads, no ATMs nearby, internet via satellite. Barter and cash are common.",
            Location.IsolatedIsland => "Very limited infrastructure. No local stock exchange. Internet is satellite-only. Supply chains are irregular. Self-sufficiency is key.",
            _ => "Moderate infrastructure typical of a mid-sized town."
        };
    }

    // --- NEWS FEED ---
    private static readonly Dictionary<Scenario, List<string>> ScenarioNews = new()
    {
        { Scenario.FeudalReset, new List<string> {
            "Large banks report unusual cash withdrawals.",
            "Gold dealers report unprecedented demand.",
            "Land registry offices overwhelmed with transfer requests.",
            "Law firms report a surge in trust and estate inquiries."
        } },
        { Scenario.Redistributed, new List<string> {
            "Government officials deny any pending economic reforms.",
            "Social media fills with rumors of wealth redistribution.",
            "High-value asset sales spiked 40% in the last hour.",
            "Central bank announces routine system maintenance."
        } },
        { Scenario.ScorchedEarth, new List<string> {
            "Multiple supply chain disruptions reported.",
            "Fuel prices jump 15% on global markets.",
            "Military activity observed near contested zones.",
            "News networks switch to round-the-clock coverage."
        } },
        { Scenario.TribalCollapse, new List<string> {
            "Border crossings experience long delays.",
            "Local militias reportedly bolstering their positions.",
            "International flights suspended in contested regions.",
            "Government officials relocate to undisclosed locations."
        } },
        { Scenario.LateBubble, new List<string> {
            "Markets open with unprecedented volatility.",
            "Tech stocks surge 12% in pre-market trading.",
            "Analysts warn of a 'classic bubble' forming.",
            "Retail investors flood trading platforms."
        } }
    };

    private static readonly Dictionary<Location, List<string>> LocationNews = new()
    {
        { Location.HighCostCity, new List<string> {
            "Financial districts see heavy traffic early.",
            "Private banks report record account openings.",
            "Property developers offer last-minute discounts.",
            "Airports see a spike in last-minute business travel."
        } },
        { Location.MidCostCity, new List<string> {
            "Local markets see increased foot traffic.",
            "Currency exchange booths have long queues.",
            "Supermarkets report early morning stockpiling.",
            "Taxi and rideshare demand triples."
        } },
        { Location.Suburban, new List<string> {
            "Gas stations report long queues forming.",
            "Hardware stores busy with emergency supplies.",
            "Schools announce early closures.",
            "Neighborhood watch groups activate."
        } },
        { Location.WarZone, new List<string> {
            "Checkpoints multiply around the city.",
            "Explosions reported on the outskirts.",
            "Curfew hours extended.",
            "Humanitarian warnings issued."
        } },
        { Location.LowCostRural, new List<string> {
            "Local co-ops request immediate harvest.",
            "Veterinarians report feed shortages.",
            "Roads to major towns are blocked by fallen trees.",
            "Communal gatherings to discuss crop security."
        } },
        { Location.IsolatedIsland, new List<string> {
            "Cargo flights are fully booked.",
            "Ferry services suspended due to weather.",
            "Satellite internet speeds degrade.",
            "Bottle water sales skyrocket."
        } }
    };

    private static List<string> GetNewsForTurn(int turn, Scenario scenario, Location location, PlayerState state)
    {
        var rand = new Random();
        var news = new List<string>();

        // Global scenario news (layer 1)
        var scenarioNews = ScenarioNews[scenario];
        var selectedScenario = scenarioNews.OrderBy(x => rand.Next()).Take(1 + turn / 2).ToList();
        news.AddRange(selectedScenario);

        // Location news (layer 2)
        var locationNews = LocationNews[location];
        var selectedLocation = locationNews.OrderBy(x => rand.Next()).Take(1).ToList();
        news.AddRange(selectedLocation);

        // Player consequences (layer 3) - based on stats
        if (state.Security < 30)
            news.Add($"⚠️ Your low security has attracted unwanted attention.");
        if (state.Food < 30)
            news.Add($"⚠️ Your food supplies are running dangerously low.");
        if (state.Liquidity > 80)
            news.Add($"💰 Your large cash holdings are drawing scrutiny.");
        if (state.Influence > 80)
            news.Add($"🤝 Your influence has grown; people are seeking your advice.");
        if (state.MoneyReceived && state.CashReceived > 0)
            news.Add($"💵 Your {state.CashReceived:C} deposit has arrived. The world is changing.");

        // If the player chose a specific action, reflect it
        if (turn == 0 && state.Food > 70)
            news.Add($"🍖 Your food stockpile is the envy of your neighbors.");
        if (turn == 1 && state.Shelter > 70)
            news.Add($"🏠 Your shelter is fortified. You feel safe.");

        return news;
    }

    private static void DisplayNewsFeed(List<string> newsItems)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n--- GLOBAL EVENTS FEED ---");
        foreach (var item in newsItems)
        {
            Console.WriteLine($"  {item}");
        }
        Console.WriteLine("---------------------------\n");
        Console.ResetColor();
    }

    // --- ACTION MAPPER ---
    private static string GetLocalizedActionDescription(GameAction action, string specificLocation, Location category)
    {
        return (category, action) switch
        {
            (Location.IsolatedIsland, GameAction.AcquireTransport) => $"a boat or fishing vessel in {specificLocation}",
            (Location.WarZone, GameAction.AcquireTransport) => $"an armored vehicle or escape truck in {specificLocation}",
            (Location.LowCostRural, GameAction.AcquireTransport) => $"a pickup truck or tractor in {specificLocation}",
            (Location.HighCostCity, GameAction.AcquireTransport) => $"a luxury car or electric vehicle in {specificLocation}",
            (Location.MidCostCity, GameAction.AcquireTransport) => $"a reliable used car or van in {specificLocation}",
            (Location.Suburban, GameAction.AcquireTransport) => $"an SUV or minivan in {specificLocation}",

            (Location.IsolatedIsland, GameAction.SecureFood) => $"dried fish, rice, and long-shelf-life rations in {specificLocation}",
            (Location.WarZone, GameAction.SecureFood) => $"aid-supply food kits and water purifiers in {specificLocation}",
            (Location.LowCostRural, GameAction.SecureFood) => $"grain, livestock feed, and seeds in {specificLocation}",
            (Location.HighCostCity, GameAction.SecureFood) => $"bulk organic produce and gourmet freeze-dried meals in {specificLocation}",
            (Location.MidCostCity, GameAction.SecureFood) => $"bulk supermarket dry goods and canned goods in {specificLocation}",
            (Location.Suburban, GameAction.SecureFood) => $"bulk groceries and deep freezer units in {specificLocation}",

            (Location.IsolatedIsland, GameAction.StrengthenShelter) => $"reinforcing the shutters and roof on your beach house in {specificLocation}",
            (Location.WarZone, GameAction.StrengthenShelter) => $"sandbagging and reinforcing your apartment in {specificLocation}",
            (Location.LowCostRural, GameAction.StrengthenShelter) => $"repairing your farmhouse and securing the perimeter in {specificLocation}",
            (Location.HighCostCity, GameAction.StrengthenShelter) => $"upgrading your apartment's security and basement storage in {specificLocation}",
            (Location.MidCostCity, GameAction.StrengthenShelter) => $"boarding windows and adding a generator to your home in {specificLocation}",
            (Location.Suburban, GameAction.StrengthenShelter) => $"installing storm shutters and a backup generator in {specificLocation}",

            (Location.IsolatedIsland, GameAction.AcquireEnergy) => $"solar panels, batteries, and diesel drums in {specificLocation}",
            (Location.WarZone, GameAction.AcquireEnergy) => $"fuel, batteries, and portable generators in {specificLocation}",
            (Location.LowCostRural, GameAction.AcquireEnergy) => $"a diesel generator, propane tanks, and firewood in {specificLocation}",
            (Location.HighCostCity, GameAction.AcquireEnergy) => $"home battery storage and backup generator in {specificLocation}",
            (Location.MidCostCity, GameAction.AcquireEnergy) => $"fuel drums and a portable generator in {specificLocation}",
            (Location.Suburban, GameAction.AcquireEnergy) => $"a large generator, gas cans, and solar panels in {specificLocation}",

            (Location.WarZone, GameAction.BuildInfluence) => $"bribing local militia leaders and city officials in {specificLocation}",
            (Location.LowCostRural, GameAction.BuildInfluence) => $"hosting community meetings and bartering with neighbors in {specificLocation}",
            (Location.IsolatedIsland, GameAction.BuildInfluence) => $"offering supplies to the local chief in exchange for protection in {specificLocation}",
            (Location.HighCostCity, GameAction.BuildInfluence) => $"hiring a private attorney and making connections at the country club in {specificLocation}",
            (Location.MidCostCity, GameAction.BuildInfluence) => $"donating to community funds and befriending local shop owners in {specificLocation}",
            (Location.Suburban, GameAction.BuildInfluence) => $"organizing a neighborhood watch and helping neighbors in {specificLocation}",

            (_, GameAction.PreserveLiquidity) => $"converting cash into gold, cryptocurrency, or foreign currency in {specificLocation}",
            _ => $"investing in essential assets in {specificLocation}"
        };
    }

    private static string GetActionDescription(GameAction action)
    {
        return action switch
        {
            GameAction.AcquireTransport => "Secure reliable transportation",
            GameAction.SecureFood => "Stockpile food and water",
            GameAction.StrengthenShelter => "Fortify your home or shelter",
            GameAction.AcquireEnergy => "Secure power generation and fuel",
            GameAction.BuildInfluence => "Build social capital and connections",
            GameAction.PreserveLiquidity => "Convert cash into stable assets (gold/crypto)",
            _ => "Take a strategic action"
        };
    }

    // --- UPDATE STATS ---
    private static void UpdateStats(PlayerState state, GameAction action, Scenario scenario, Location location)
    {
        switch (action)
        {
            case GameAction.AcquireTransport:
                state.Mobility = Math.Min(100, state.Mobility + 25);
                state.Liquidity = Math.Max(0, state.Liquidity - 20);
                state.Security += 5;
                break;
            case GameAction.SecureFood:
                state.Food = Math.Min(100, state.Food + 40);
                state.Liquidity = Math.Max(0, state.Liquidity - 25);
                state.Influence += 10;
                break;
            case GameAction.StrengthenShelter:
                state.Shelter = Math.Min(100, state.Shelter + 35);
                state.Security = Math.Min(100, state.Security + 15);
                state.Liquidity = Math.Max(0, state.Liquidity - 20);
                break;
            case GameAction.AcquireEnergy:
                state.Energy = Math.Min(100, state.Energy + 30);
                state.Shelter = Math.Min(100, state.Shelter + 10);
                state.Food = Math.Min(100, state.Food + 10);
                state.Liquidity = Math.Max(0, state.Liquidity - 30);
                break;
            case GameAction.BuildInfluence:
                state.Influence = Math.Min(100, state.Influence + 30);
                state.Security = Math.Min(100, state.Security + 10);
                state.Liquidity = Math.Max(0, state.Liquidity - 15);
                break;
            case GameAction.PreserveLiquidity:
                state.Liquidity = Math.Min(100, state.Liquidity + 20);
                state.Security -= 5;
                state.Influence += 5;
                break;
        }

        // Scenario modifiers
        if (scenario == Scenario.ScorchedEarth)
        {
            state.Food = Math.Max(0, state.Food - 10);
            state.Security = Math.Max(0, state.Security - 10);
        }
        if (scenario == Scenario.FeudalReset)
        {
            state.Influence = Math.Min(100, state.Influence + 10);
            state.Liquidity = Math.Max(0, state.Liquidity - 10);
        }
        if (scenario == Scenario.Redistributed)
        {
            state.Liquidity = Math.Max(0, state.Liquidity - 15);
            state.Influence = Math.Min(100, state.Influence + 5);
        }

        // Location modifiers
        if (location == Location.WarZone)
        {
            state.Security = Math.Max(0, state.Security - 20);
            state.Food = Math.Max(0, state.Food - 10);
        }
        if (location == Location.IsolatedIsland)
        {
            state.Mobility = Math.Max(0, state.Mobility - 10);
            state.Liquidity = Math.Max(0, state.Liquidity - 10);
        }

        // Clamp
        state.Liquidity = Math.Clamp(state.Liquidity, 0, 100);
        state.Food = Math.Clamp(state.Food, 0, 100);
        state.Shelter = Math.Clamp(state.Shelter, 0, 100);
        state.Security = Math.Clamp(state.Security, 0, 100);
        state.Mobility = Math.Clamp(state.Mobility, 0, 100);
        state.Influence = Math.Clamp(state.Influence, 0, 100);
        state.Energy = Math.Clamp(state.Energy, 0, 100);
    }

    // --- PROCESS WEDNESDAY ---
    private static void ProcessWednesday(PlayerState state, Option option)
    {
        state.MoneyOption = option;
        state.MoneyReceived = true;

        switch (option)
        {
            case Option.A:
                state.CashReceived = 500000;
                state.Liquidity = Math.Min(100, state.Liquidity + 30);
                break;
            case Option.B:
                state.CashReceived = 1000000;
                state.Liquidity = Math.Min(100, state.Liquidity + 40);
                break;
            case Option.C:
                state.CashReceived = 2000000;
                state.Liquidity = Math.Min(100, state.Liquidity + 50);
                break;
        }

        // Security impact of receiving money
        if (state.CashReceived > 1000000)
            state.Security = Math.Max(0, state.Security - 10);
    }

    // --- MAIN GAME LOOP ---
    public static async Task StartAsync()
    {
        bool playAgain = true;

        while (playAgain)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== JUST ANOTHER WEDNESDAY ===");
            Console.ResetColor();
            Console.WriteLine("A secret message arrived at dawn: money is being printed.");
            Console.WriteLine("Everyone on Earth will receive $1,000,000 on Wednesday at the same moment.\n");
            Console.WriteLine("But right now, it's early Monday morning, and you are the ONLY person who knows.\n");
            Console.WriteLine("You have two days to prepare before the world catches on.\n");

            Console.Write("Are you Male or Female? (M/F): ");
            var genderInput = Console.ReadLine()?.Trim().ToUpper();
            var gender = genderInput == "F" ? "Female" : "Male";
            Console.WriteLine();

            var random = new Random();
            var roles = Enum.GetValues<Role>();
            var role = roles[random.Next(roles.Length)];

            var locations = Enum.GetValues<Location>();
            var locationEnum = locations[random.Next(locations.Length)];

            var placeList = LocationNames[locationEnum];
            var specificLocation = placeList[random.Next(placeList.Count)];

            var scenarios = Enum.GetValues<Scenario>();
            var scenario = scenarios[random.Next(scenarios.Length)];

            var localTimeInfo = GetMondayMorningLocalInfo(specificLocation);
            var infraInfo = GetInfrastructureContext(locationEnum);

            Console.WriteLine("========================================");
            Console.WriteLine($"You are a {gender} {role} in {specificLocation}.");
            Console.WriteLine($"Scenario: {scenario} - {GetScenarioDescription(scenario)}");
            Console.WriteLine($"\n{localTimeInfo}");
            Console.WriteLine($"\nInfrastructure: {infraInfo}");
            Console.WriteLine("========================================\n");

            // --- Initialize Player State ---
            var playerState = new PlayerState();
            if (role == Role.Landlord) playerState.Shelter += 10;
            if (role == Role.Farmer) playerState.Food += 10;
            if (role == Role.GigWorker) playerState.Mobility += 5;
            if (role == Role.Striver) playerState.Influence += 5;
            if (role == Role.Retiree) playerState.Liquidity += 5;
            if (role == Role.ShopOwner) playerState.Influence += 5;

            // --- Choose Money Option (with timer) ---
            Console.WriteLine("The money will arrive for everyone on Wednesday at the same moment worldwide.");
            Console.WriteLine("But you have a choice of when YOU receive yours:\n");
            Console.WriteLine("  A) $500,000 — this Monday (2 days early)");
            Console.WriteLine("  B) $1,000,000 — Wednesday (with everyone else)");
            Console.WriteLine("  C) $2,000,000 — next Monday (5 days late)");
            Console.WriteLine();

            Console.WriteLine("⏰ You have 3 minutes to decide. If you don't respond in time, you'll default to Option B.");

            var option = await GetUserChoiceWithTimeout<Option>("When do you want your money?", 180, Option.B);
            playerState.MoneyOption = option;
            Console.Clear();

            // --- MULTI-TURN LOOP ---
            var turns = new List<(string time, string label)>
            {
                ("Monday 8:00 AM", "First action"),
                ("Monday 2:00 PM", "Second action"),
                ("Tuesday 9:00 AM", "Third action"),
                ("Tuesday 4:00 PM", "Final action before Wednesday")
            };

            for (int turnIndex = 0; turnIndex < turns.Count; turnIndex++)
            {
                var (time, label) = turns[turnIndex];

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"=== {time} ===");
                Console.ResetColor();

                // Show current stats
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(playerState.GetSummary());
                Console.ResetColor();
                Console.WriteLine();

                // Show news feed for this turn
                var news = GetNewsForTurn(turnIndex, scenario, locationEnum, playerState);
                DisplayNewsFeed(news);

                Console.WriteLine($"You are a {gender} {role} in {specificLocation}.");
                Console.WriteLine($"Your money option: {option} ({(option == Option.A ? "$500k Monday" : option == Option.B ? "$1M Wednesday" : "$2M next Monday")})");
                Console.WriteLine();

                Console.WriteLine($"What's your {label}?");

                // Show action descriptions
                Console.WriteLine("\nAvailable actions (with location context):");
                var actions = Enum.GetValues<GameAction>();
                for (int i = 0; i < actions.Length; i++)
                {
                    var action = actions.GetValue(i);
                    var desc = GetLocalizedActionDescription((GameAction)action, specificLocation, locationEnum);
                    var shortDesc = GetActionDescription((GameAction)action);
                    Console.WriteLine($"  {i + 1}. {shortDesc} → {desc}");
                }

                var gameAction = GetUserChoice<GameAction>("Choose your action");

                // Apply the action
                UpdateStats(playerState, (GameAction)gameAction, scenario, locationEnum);

                // If it's the first turn and they chose Option A, they get their money now
                if (turnIndex == 0 && option == Option.A)
                {
                    ProcessWednesday(playerState, option);
                }

                // If it's the last turn, process Wednesday
                if (turnIndex == turns.Count - 1)
                {
                    // If they chose Option B or C, money arrives now or later
                    if (option == Option.B)
                        ProcessWednesday(playerState, option);
                    else if (option == Option.C)
                    {
                        // Option C arrives next Monday (after the story ends)
                        // So we just note it in the prompt
                    }
                }

                Console.WriteLine($"\n✅ Action applied. Press any key to continue...");
                Console.ReadKey();
            }

            // --- Build Prompt for Final Story ---
            var awareness = 20 + (playerState.Liquidity > 70 ? 10 : 0) + (playerState.Influence > 70 ? 10 : 0);
            if (option == Option.A) awareness += 10;
            if (option == Option.C) awareness += 5;
            if (scenario == Scenario.LateBubble || scenario == Scenario.ScorchedEarth) awareness += 20;
            awareness = Math.Min(100, awareness);

            string awarenessDescription = awareness switch
            {
                < 20 => "The world is largely oblivious. Just another Wednesday.",
                < 40 => "Whispers are spreading. Economists are noticing anomalies.",
                < 60 => "News networks are covering the rumors. Markets are volatile.",
                < 80 => "Social media is in a frenzy. Banks are restricting withdrawals.",
                _ => "The world is in full panic. Everyone knows something is happening."
            };

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("**Just Another Wednesday – One Month Later**");
            promptBuilder.AppendLine($"Scenario: {scenario} - {GetScenarioDescription(scenario)}");
            promptBuilder.AppendLine($"Location: {specificLocation}");
            promptBuilder.AppendLine($"Gender: {gender}");
            promptBuilder.AppendLine($"Role: {role}");
            promptBuilder.AppendLine($"Money Option: {option} ({GetOptionDescription(option)})");
            if (playerState.MoneyReceived)
                promptBuilder.AppendLine($"Money Received: {playerState.CashReceived:C}");
            else
                promptBuilder.AppendLine($"Money Received: Not yet (arriving next Monday)");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("**Player Stats (Final):**");
            promptBuilder.AppendLine($"- Liquidity: {playerState.Liquidity}");
            promptBuilder.AppendLine($"- Food: {playerState.Food}");
            promptBuilder.AppendLine($"- Shelter: {playerState.Shelter}");
            promptBuilder.AppendLine($"- Security: {playerState.Security}");
            promptBuilder.AppendLine($"- Mobility: {playerState.Mobility}");
            promptBuilder.AppendLine($"- Influence: {playerState.Influence}");
            promptBuilder.AppendLine($"- Energy: {playerState.Energy}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("**World State:**");
            promptBuilder.AppendLine($"- Awareness: {awarenessDescription}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("**Instructions:**");
            promptBuilder.AppendLine("1. Write a 2-3 paragraph story showing how this player's choices played out over the following month.");
            promptBuilder.AppendLine("2. The story must be CONSISTENT with the player's stats and the world awareness state.");
            promptBuilder.AppendLine("3. If the stats are low, show struggle. If high, show success.");
            promptBuilder.AppendLine("4. If awareness is high, show the world reacting (panicking, banks failing, etc.).");
            promptBuilder.AppendLine("5. End with a clear judgment: Thrived, Survived, or Failed. Explain why based on the stats.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Format your response exactly like this:");
            promptBuilder.AppendLine("Story:");
            promptBuilder.AppendLine("[story text]");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Judgment:");
            promptBuilder.AppendLine("[Thrived / Survived / Failed] - [brief explanation]");

            var prompt = promptBuilder.ToString();
            File.WriteAllText("duel_prompt.txt", prompt);

            // --- Generate Story ---
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== WEDNESDAY ARRIVES ===");
            Console.ResetColor();
            Console.WriteLine("The money has hit the world. Your story unfolds...\n");

            Console.WriteLine("📝 Generating your story via DeepSeek API...");

            var apiKey = GetApiKey();
            var rawResponse = await GenerateStoriesWithDeepSeek(prompt, apiKey);
            File.WriteAllText("duel_stories.txt", rawResponse);

            var (story, judgment) = ParseSingleResponse(rawResponse);

            Console.WriteLine("\n=== YOUR STORY ===");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(story);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("=== JUDGMENT ===");
            Console.ForegroundColor = judgment.Contains("Thrived") ? ConsoleColor.Green : (judgment.Contains("Failed") ? ConsoleColor.Red : ConsoleColor.Yellow);
            Console.WriteLine(judgment);
            Console.ResetColor();

            Console.WriteLine($"\n✅ Full response saved to duel_stories.txt");

            Console.Write("\n\nPlay another round? (Y/N): ");
            var playInput = Console.ReadLine()?.Trim().ToUpper();
            if (playInput != "Y")
                playAgain = false;
        }

        Console.WriteLine("\nThanks for playing! Press any key to return to menu...");
        Console.ReadKey();
    }

    // --- HELPERS ---
    private static (string story, string judgment) ParseSingleResponse(string raw)
    {
        string story = "", judgment = "";

        var storyIndex = raw.IndexOf("Story:", StringComparison.OrdinalIgnoreCase);
        var judgmentIndex = raw.IndexOf("Judgment:", StringComparison.OrdinalIgnoreCase);

        if (storyIndex >= 0 && judgmentIndex >= 0)
        {
            story = raw.Substring(storyIndex + "Story:".Length, judgmentIndex - storyIndex - "Story:".Length).Trim();
            judgment = raw.Substring(judgmentIndex + "Judgment:".Length).Trim();
        }
        else
        {
            story = "Could not parse story. See full response below.";
            judgment = "Could not parse judgment. See full response below.";
        }

        return (story, judgment);
    }

    private static string GetOptionDescription(Option option) => option switch
    {
        Option.A => "Monday (2 days early, $500k)",
        Option.B => "Wednesday (with everyone else, $1M)",
        Option.C => "next Monday (5 days late, $2M)",
        _ => "unknown"
    };

    private static string GetApiKey()
    {
        var envKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
        if (!string.IsNullOrEmpty(envKey))
            return envKey;

        if (File.Exists("deepseek.key"))
            return File.ReadAllText("deepseek.key").Trim();

        if (File.Exists("deepseek.key.txt"))
            return File.ReadAllText("deepseek.key.txt").Trim();

        throw new Exception("API key not found.");
    }

    private static async Task<string> GenerateStoriesWithDeepSeek(string prompt, string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var requestPayload = new
        {
            model = "deepseek-v4-flash",
            messages = new[]
            {
                new { role = "system", content = "You are a fiction writer and survival judge. Output in the requested format. Never contradict the provided stats." },
                new { role = "user", content = prompt }
            },
            temperature = 0.85,
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.deepseek.com/v1/chat/completions", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"DeepSeek API error: {response.StatusCode}\n{responseJson}");

        using var doc = JsonDocument.Parse(responseJson);
        var story = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return story ?? "Failed to generate stories.";
    }

    private static string GetScenarioDescription(Scenario scenario) => scenario switch
    {
        Scenario.FeudalReset => "Wealth concentrates into land and physical assets as society reorders into feudal structures.",
        Scenario.Redistributed => "Existing wealth is rapidly redistributed, changing who holds liquid assets without massive inflation.",
        Scenario.ScorchedEarth => "Conflicts escalate, infrastructure is destroyed and supply chains fail.",
        Scenario.TribalCollapse => "Central authority collapses, local militias and informal powers take control.",
        Scenario.LateBubble => "A panic-driven spike occurs mid-week followed by a crash the following week.",
        _ => "A global economic shift occurs."
    };

    private static async Task<T> GetUserChoiceWithTimeout<T>(string prompt, int timeoutSeconds, T defaultChoice) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine($"{prompt} ({typeof(T).Name}):");
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($"  {i + 1}. {values[i]}");

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        var inputTask = Task.Run(() =>
        {
            while (true)
            {
                Console.Write("Enter number: ");
                var line = Console.ReadLine();
                if (int.TryParse(line, out int choice) && choice >= 1 && choice <= values.Length)
                    return values[choice - 1];
                Console.WriteLine($"Invalid. Enter 1-{values.Length}.");
            }
        }, cts.Token);

        var completed = await Task.WhenAny(inputTask, Task.Delay(timeoutSeconds * 1000, cts.Token));
        if (completed == inputTask)
            return await inputTask;
        else
        {
            Console.WriteLine($"\n⏰ Time's up! Defaulting to {defaultChoice}.");
            return defaultChoice;
        }
    }

    private static T GetUserChoice<T>(string prompt) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine($"{prompt} ({typeof(T).Name}):");
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($"  {i + 1}. {values[i]}");

        int choice;
        while (true)
        {
            Console.Write("Enter number: ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= values.Length)
                return values[choice - 1];
            Console.WriteLine($"Invalid. Enter 1-{values.Length}.");
        }
    }
}