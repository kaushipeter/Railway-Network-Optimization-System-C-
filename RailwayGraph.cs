using System;
using System.Collections.Generic;
using System.Linq;

namespace PartB_Scenario2
{
    public class RailwayGraph
    {
     
        private Dictionary<string, List<Edge>> adjacencyList;

        public RailwayGraph()
        {
            adjacencyList = new Dictionary<string, List<Edge>>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddStation(string name)
        {
            if (!adjacencyList.ContainsKey(name))
            {
                adjacencyList[name] = new List<Edge>();
            }
        }

        public void AddRoute(string from, string to, int cost)
        {
            AddStation(from);
            AddStation(to);

            // check if edge already exists to update or add
            var edges = adjacencyList[from];
            var existing = edges.FirstOrDefault(e => e.Destination.Equals(to, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Weight = cost; // Update if exists
            }
            else
            {
                edges.Add(new Edge(to, cost)); // Add new directed edge
            }
        }

        public void RemoveStation(string name)
        {
            if (adjacencyList.ContainsKey(name))
            {
                // Remove the station itself
                adjacencyList.Remove(name);

                // Remove all edges pointing TO this station
                foreach (var station in adjacencyList)
                {
                    station.Value.RemoveAll(e => e.Destination.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                Console.WriteLine($"Station '{name}' and associated routes deleted.");
            }
            else
            {
                Console.WriteLine("Station not found.");
            }
        }
        
        public void RemoveRoute(string from, string to)
        {
            if (adjacencyList.ContainsKey(from))
            {
                int removed = adjacencyList[from].RemoveAll(e => e.Destination.Equals(to, StringComparison.OrdinalIgnoreCase));
                if (removed > 0) Console.WriteLine("Route removed.");
                else Console.WriteLine("Route not found.");
            }
        }

        public void DisplayNetwork()
        {
            Console.WriteLine("\n--- Railway Network (Directed) ---");
            foreach (var station in adjacencyList)
            {
                Console.Write($"{station.Key} -> ");
                if (station.Value.Count == 0)
                {
                    Console.WriteLine("[End of Line]");
                }
                else
                {
                    foreach (var edge in station.Value)
                    {
                        Console.Write($"[{edge.Destination} ({edge.Weight}km)] ");
                    }
                    Console.WriteLine();
                }
            }
        }

        public bool StationExists(string name)
        {
            return adjacencyList.ContainsKey(name);
        }

        // Dijkstra's Algorithm
        public (List<string> Path, int Cost) GetShortestPath(string start, string end)
        {
            if (!adjacencyList.ContainsKey(start) || !adjacencyList.ContainsKey(end))
                return (null, -1);

            var distances = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var previous = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var priorityQueue = new PriorityQueue<string, int>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var station in adjacencyList.Keys)
            {
                distances[station] = int.MaxValue;
            }
            distances[start] = 0;
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                string current = priorityQueue.Dequeue();

                if (current.Equals(end, StringComparison.OrdinalIgnoreCase))
                    break; // Reached destination

                if (visited.Contains(current)) continue;
                visited.Add(current);
                
               
                if (distances[current] == int.MaxValue) continue;

                if (adjacencyList.ContainsKey(current))
                {
                    foreach (var edge in adjacencyList[current])
                    {
                        int newDist = distances[current] + edge.Weight;
                        if (newDist < distances[edge.Destination])
                        {
                            distances[edge.Destination] = newDist;
                            previous[edge.Destination] = current;
                            priorityQueue.Enqueue(edge.Destination, newDist);
                        }
                    }
                }
            }

            // Reconstruct path
            if (distances[end] == int.MaxValue) return (null, -1); // Unreachable

            var path = new List<string>();
            string curr = end;
            while (curr != null)
            {
                path.Insert(0, curr);
                previous.TryGetValue(curr, out curr);
            }

            return (path, distances[end]);
        }

        // Multi-stop optimization
        public (List<string> FullPath, int TotalCost) GetOptimalRouteWithStops(string start, string end, List<string> stops)
        {
            
            var permutations = GetPermutations(stops, stops.Count).ToList();
            
            List<string> bestPath = null;
            int minCost = int.MaxValue;

            foreach (var perm in permutations)
            {
                var currentPath = new List<string>();
                int currentCost = 0;
                string currentStart = start;
                bool validPermutation = true;

                List<string> sequence = new List<string>();
                sequence.AddRange(perm);
                sequence.Add(end);

                foreach (var target in sequence)
                {
                    var result = GetShortestPath(currentStart, target);
                    if (result.Path == null)
                    {
                        validPermutation = false;
                        break;
                    }

                    
                    if (currentPath.Count > 0)
                    {
         
                        currentPath.RemoveAt(currentPath.Count - 1);
                    }
                    currentPath.AddRange(result.Path);
                    currentCost += result.Cost;
                    currentStart = target;
                }

                if (validPermutation && currentCost < minCost)
                {
                    minCost = currentCost;
                    bestPath = currentPath;
                }
            }

            return (bestPath, minCost == int.MaxValue ? -1 : minCost);
        }

        // Helper for permutations
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        // Initialize with prompt data
        public void InitializeSampleData()
        {
            AddRoute("Colombo", "Kandy", 115);
            AddRoute("Colombo", "Galle", 120);
            AddRoute("Kandy", "Anuradhapura", 138);
            AddRoute("Anuradhapura", "Jaffna", 193);
            AddRoute("Kandy", "Trincomalee", 174);
            AddRoute("Galle", "Colombo", 120);
            AddStation("Trincomalee"); 
            AddStation("Jaffna");
        }
    }
}
