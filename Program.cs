using System;
using System.Collections.Generic;
using System.Linq;

namespace PartB_Scenario2
{
    class Program
    {
        static RailwayGraph graph = new RailwayGraph();

        static void Main(string[] args)
        {
            graph.InitializeSampleData();  

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=================================================");
                Console.WriteLine("       Railway Network Analysis System           ");
                Console.WriteLine("=================================================");
                Console.WriteLine("A. Build/Modify Network (Add/Remove)");
                Console.WriteLine("B. Display Network");
                Console.WriteLine("C. Find Shortest Path (with Optional Stops)");
                Console.WriteLine("D. Search for Station");
                Console.WriteLine("Q. Quit");
                Console.WriteLine("=================================================");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "A":
                        ManageNetwork();
                        break;
                    case "B":
                        graph.DisplayNetwork();
                        Pause();
                        break;
                    case "C":
                        FindPath();
                        break;
                    case "D":
                        SearchStation();
                        break;
                    case "Q":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        static void ManageNetwork()
        {
            Console.Clear();
            Console.WriteLine("1. Add Station");
            Console.WriteLine("2. Add Route");
            Console.WriteLine("3. Remove Station");
            Console.WriteLine("4. Remove Route");
            Console.Write("Choice: ");
            string subChoice = Console.ReadLine();

            if (subChoice == "1")
            {
                Console.Write("Station Name: ");
                graph.AddStation(Console.ReadLine());
                Console.WriteLine("Station added.");
            }
            else if (subChoice == "2")
            {
                Console.Write("From: ");
                string from = Console.ReadLine();
                Console.Write("To: ");
                string to = Console.ReadLine();
                Console.Write("Cost (km): ");
                if (int.TryParse(Console.ReadLine(), out int cost))
                {
                    graph.AddRoute(from, to, cost);
                    Console.WriteLine("Route added/updated.");
                }
            }
            else if (subChoice == "3")
            {
                Console.Write("Station Name to remove: ");
                graph.RemoveStation(Console.ReadLine());
            }
            else if (subChoice == "4")
            {
                Console.Write("From: ");
                string from = Console.ReadLine();
                Console.Write("To: ");
                string to = Console.ReadLine();
                graph.RemoveRoute(from, to);
            }
            Pause();
        }

        static void FindPath()
        {
            Console.Clear();
            Console.WriteLine("--- Find Shortest Path ---");
            Console.Write("Source Station: ");
            string start = Console.ReadLine();
            Console.Write("Destination Station: ");
            string end = Console.ReadLine();

            Console.WriteLine("Enter mandatory stops separated by comma (leave empty for none):");
            string stopsInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(stopsInput))
            {
                // Simple Dijkstra
                var result = graph.GetShortestPath(start, end);
                DisplayResult(result.Path, result.Cost);
            }
            else
            {
                // With Stops
                List<string> stops = stopsInput.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                var result = graph.GetOptimalRouteWithStops(start, end, stops);
                DisplayResult(result.FullPath, result.TotalCost);
            }
            Pause();
        }

        static void DisplayResult(List<string> path, int cost)
        {
            if (path == null || cost == -1)
            {
                Console.WriteLine("\nNo valid route found.");
            }
            else
            {
                Console.WriteLine("\n>>> Optimal Route Found:");
                Console.WriteLine(string.Join(" -> ", path));
                Console.WriteLine($"Total Distance: {cost} km");
            }
        }

        static void SearchStation()
        {
            Console.Clear();
            Console.Write("Enter Station Name: ");
            string name = Console.ReadLine();
            if (graph.StationExists(name))
                Console.WriteLine("Station exists in the network.");
            else
                Console.WriteLine("Station NOT found.");
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadLine();
        }
    }
}
