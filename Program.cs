using System;

namespace JustAnotherWednesday
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to The Wednesday Dilemma!");
            Console.WriteLine("1. Run Simulator (10,000 rounds)");
            Console.WriteLine("2. Run Simulator with custom rounds");
            Console.WriteLine("3. Human Duel Mode (coming soon)");
            Console.Write("Choose an option: ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Simulator.Run();
                    break;
                case "2":
                    Console.Write("Enter number of rounds: ");
                    if (int.TryParse(Console.ReadLine(), out int rounds) && rounds > 0)
                        Simulator.Run(rounds);
                    else
                        Console.WriteLine("Invalid input. Using default 10,000.");
                    break;
                case "3":
                    await HumanDuel.StartAsync();
                    break;
                default:
                    Console.WriteLine("Invalid option. Running simulator with 10,000 rounds.");
                    Simulator.Run();
                    break;
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}