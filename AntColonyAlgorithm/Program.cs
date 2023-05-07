using AntColonyAlgorithm.Algorithm;
using AntColonyAlgorithm.General;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AntColonyAlgorithm
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            var distanseMap = JsonConvert.DeserializeObject<double[,]>(File.ReadAllText($"100cities.txt"));
            Constants constants = new Constants()
            {
                Alpha = 1,
                Beta = 2,
                M = 100,
                StartPheramonValue = 0.2,
                Q = 4,
                P = 1 - 0.2
            };

            var parallerTime = MeasureTime(() => ParallelSyncAlg(constants, distanseMap)); 
            var syncTime = MeasureTime(() => TestSyncAlg(constants, distanseMap));

            Console.WriteLine($"паралельний: {parallerTime.Elapsed}");
            Console.WriteLine($"синхронний: {syncTime.Elapsed}");
        }

        public static void TestSyncAlg(Constants constants, double[,] distanseMap)
        {
            SyncAntColony syncAntColony = new SyncAntColony(constants, distanseMap);

            for (int i = 0; i < 100; i++)
                syncAntColony.Iteration();

            //foreach (var item in syncAntColony.BestPath)
            //{
            //    Console.Write($"{item} -> ");
            //}
        }

        public static void ParallelSyncAlg(Constants constants, double[,] distanseMap)
        {
            ParallelAntColony syncAntColony = new ParallelAntColony(constants, distanseMap);

            for (int i = 0; i < 100; i++)
                syncAntColony.Iteration();

            Console.WriteLine(syncAntColony.BestPathLenght);

            //foreach (var item in syncAntColony.BestPath)
            //{
            //    Console.Write($"{item} -> ");
            //}
        }

        public static Stopwatch MeasureTime(Action method)
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            method();
            stopwatch.Stop();

            return stopwatch;
        }
    }
}
