namespace PartB_Scenario2
{
    public class Edge
    {
        public string Destination { get; set; }
        public int Weight { get; set; }

        public Edge(string destination, int weight)
        {
            Destination = destination;
            Weight = weight;
        }
    }
}
