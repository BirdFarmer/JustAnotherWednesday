using System.Text;
using System.Text.Json;

namespace JustAnotherWednesday;

public static class HumanDuel
{
    // --- Location database (unchanged, includes Helsingborg and Koh Phangan) ---
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
            "Lisbon, Portugal",
            "Helsingborg, Sweden"
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
            "Kyiv, Ukraine", "Sanaa, Yemen", "Khartoum, Sudan",
            "Port-au-Prince, Haiti"
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
            "Chatham Islands, New Zealand",
            "Koh Phangan, Thailand"
        } }
    };

    // --- Timezone helper (unchanged) ---
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

    // --- NEW: Always Monday morning at 8 AM local time ---
    private static string GetMondayMorningLocalInfo(string location)
    {
        int offset = GetUtcOffset(location);
        // 8 AM local time is 8 - offset UTC
        double utcHour = (8 - offset + 24) % 24;
        // For display, we just show 8:00 AM local
        var hour = 8;
        var minute = 0;
        var ampm = "AM";
        var displayHour = 8;

        string timeOfDay = "morning (business hours)";
        string details = "Banks are open. Stock markets are open if it's a weekday. You can make electronic transfers and trades.";

        return $"It's Monday morning, approximately 8:00 AM local time in {location}. {details}";
    }

    private static string GetInfrastructureContext(Location locationEnum)
    {
        return locationEnum switch
        {
            Location.HighCostCity => "High-speed internet, reliable power, functioning banks, ATMs everywhere, 24/7 emergency services.",
            Location.MidCostCity => "Good internet, mostly reliable power, banks and ATMs available, but slower outside downtown.",
            Location.Suburban => "Good residential infrastructure, reliable power, internet, nearby shopping centers, but car-dependent.",
            Location.WarZone => "Destroyed infrastructure. No working banks. Internet is spotty or offline. Power is intermittent. Looting and curfews common. Physical safety is a major concern.",
            Location.LowCostRural => "Limited infrastructure. Dirt roads, no ATMs nearby, internet via satellite or weak cellular. Power may be unreliable. Barter and cash are more common than banking.",
            Location.IsolatedIsland => "Very limited infrastructure. No local stock exchange. Internet is expensive satellite-only. One small bank or none at all. Supply chains are irregular. Self-sufficiency is key.",
            _ => "Moderate infrastructure typical of a mid-sized town."
        };
    }

    // --- Main game loop ---
    public static async Task StartAsync()
    {
        bool playAgain = true;

        while (playAgain)
        {
            Console.Clear();
            Console.WriteLine("=== THE WEDNESDAY DILEMMA ===");
            Console.WriteLine("A secret message arrived at dawn: money is being printed.");
            Console.WriteLine("Everyone on Earth will receive $1,000,000 on Wednesday at the same moment.\n");
            Console.WriteLine("But right now, it's early Monday morning, and you are the ONLY person who knows.\n");
            Console.WriteLine("You have a brief window to act before the rest of the world finds out.\n");

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

            // Fixed: Monday morning, local time
            var localTimeInfo = GetMondayMorningLocalInfo(specificLocation);
            var infraInfo = GetInfrastructureContext(locationEnum);

            Console.WriteLine("========================================");
            Console.WriteLine($"You are a {gender} {role} in {specificLocation}.");
            Console.WriteLine($"Scenario: {scenario} - {GetScenarioDescription(scenario)}");
            Console.WriteLine($"\n{localTimeInfo}");
            Console.WriteLine($"\nInfrastructure: {infraInfo}");
            Console.WriteLine("========================================\n");

            Console.WriteLine("The money will arrive for everyone on Wednesday at the same moment worldwide.");
            Console.WriteLine("But you have a choice of when YOU receive yours:\n");
            Console.WriteLine("  A) $500,000 — this Monday (2 days early)");
            Console.WriteLine("  B) $1,000,000 — Wednesday (with everyone else)");
            Console.WriteLine("  C) $2,000,000 — next Monday (5 days late)");
            Console.WriteLine();

            Console.WriteLine("⏰ You have 3 minutes to decide. If you don't respond in time, you'll default to Option B.");

            // --- Option selection with 3-minute timeout ---
            var option = await GetUserChoiceWithTimeout<Option>("When do you want your money?", 180, Option.B);

            Console.Clear();

            // --- Now choose your action (no timer) ---
            Console.WriteLine("Now that you've chosen when you'll get your money, what do you do with it?\n");
            Console.WriteLine("Consider your local constraints:");
            Console.WriteLine($"  {localTimeInfo}");
            Console.WriteLine($"  {infraInfo}\n");

            var action = GetUserChoice<Action>("What's your plan?");
            Console.Clear();

            // --- Build the prompt ---
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("**The Wednesday Dilemma: One Month Later**");
            promptBuilder.AppendLine($"Scenario: {scenario} - {GetScenarioDescription(scenario)}");
            promptBuilder.AppendLine($"The player is the only one who knew about the event on Monday morning.");
            promptBuilder.AppendLine($"Local time: 8 AM Monday morning in {specificLocation}.");
            promptBuilder.AppendLine($"Local infrastructure: {infraInfo}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("The Player:");
            promptBuilder.AppendLine($"Gender: {gender}");
            promptBuilder.AppendLine($"Role: {role}");
            promptBuilder.AppendLine($"Location: {specificLocation}");
            promptBuilder.AppendLine($"Action: {action}");
            promptBuilder.AppendLine($"Option: {option} (received money on {GetOptionDescription(option)})");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Instructions:");
            promptBuilder.AppendLine("1. Write a short realistic story of 2-3 paragraphs showing how this player's choices played out over the following month in the given scenario.");
            promptBuilder.AppendLine("2. Emphasize that the player had the advantage of being the first to know – they could act before others.");
            promptBuilder.AppendLine("3. Focus on believable human details, motivations, and concrete events tied to the scenario, location, and local constraints.");
            promptBuilder.AppendLine("4. End with a clear judgment: did the player survive, thrive, or fail? Explain why.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Format your response exactly like this:");
            promptBuilder.AppendLine("Story:");
            promptBuilder.AppendLine("[story text]");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Judgment:");
            promptBuilder.AppendLine("[Survived / Thrived / Failed] - [brief explanation]");

            var prompt = promptBuilder.ToString();
            File.WriteAllText("duel_prompt.txt", prompt);

            Console.WriteLine("\n📝 Generating your story via DeepSeek API...");

            var apiKey = GetApiKey();
            var rawResponse = await GenerateStoriesWithDeepSeek(prompt, apiKey);
            File.WriteAllText("duel_stories.txt", rawResponse);

            var (story, judgment) = ParseSingleResponse(rawResponse);

            Console.WriteLine("\n=== YOUR STORY ===");
            Console.WriteLine(story);
            Console.WriteLine();
            Console.WriteLine("=== JUDGMENT ===");
            Console.WriteLine(judgment);
            Console.WriteLine();

            Console.WriteLine($"\n✅ Full response saved to duel_stories.txt");

            Console.Write("\n\nPlay another round? (Y/N): ");
            var playInput = Console.ReadLine()?.Trim().ToUpper();
            if (playInput != "Y")
                playAgain = false;
        }

        Console.WriteLine("\nThanks for playing! Press any key to return to menu...");
        Console.ReadKey();
    }

    // --- Timeout helper ---
    private static async Task<T> GetUserChoiceWithTimeout<T>(string prompt, int timeoutSeconds, T defaultChoice) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine($"{prompt} ({typeof(T).Name}):");
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {values[i]}");
        }

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        // We'll use a task to read input
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
        {
            return await inputTask;
        }
        else
        {
            Console.WriteLine($"\n⏰ Time's up! Defaulting to {defaultChoice}.");
            return defaultChoice;
        }
    }

    // --- Existing helper methods (unchanged) ---
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

        throw new Exception("API key not found. Set DEEPSEEK_API_KEY environment variable or create deepseek.key or deepseek.key.txt file in the project root.");
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
                new
                {
                    role = "system",
                    content = "You are a fiction writer and survival judge. Follow the instructions exactly. Output in the requested format."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.85,
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.deepseek.com/v1/chat/completions", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"DeepSeek API error: {response.StatusCode}\n{responseJson}");
        }

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

    private static T GetUserChoice<T>(string prompt) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine($"{prompt} ({typeof(T).Name}):");
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {values[i]}");
        }

        int choice;
        while (true)
        {
            Console.Write("Enter number: ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= values.Length)
                break;
            Console.WriteLine($"Invalid. Enter 1-{values.Length}.");
        }

        return values[choice - 1];
    }
}