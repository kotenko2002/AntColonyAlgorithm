using AntColonyAlgorithm.General;
using AntColonyAlgorithm.Sync;
using Newtonsoft.Json;
using System.Text.Json;

namespace AntColonyAlgorithm
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            var distanseMap = JsonConvert.DeserializeObject<double[,]>(File.ReadAllText($"10cities.txt"));
            Constants constants = new Constants()
            {
                Alpha = 1,
                Beta = 2,
                M = 10,
                StartPheramonValue = 0.2,
                Q = 4,
                P = 1 - 0.2
            };

            SyncAntColony syncAntColony = new SyncAntColony(constants, distanseMap);

            for (int i = 0; i < 100; i++)
            {
                syncAntColony.Iteration();
                Console.WriteLine(syncAntColony.BestPathLenght);
            }
            foreach (var item in syncAntColony.BestPath)
            {
                Console.Write($"{item} -> ");
            }
        }
    }
}
