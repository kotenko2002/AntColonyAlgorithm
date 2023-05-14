using AntColonyAlgorithm.Algorithm;
using AntColonyAlgorithm.General;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AntColonyAlgorithm
{
    internal class Program
    {
        static public async Task Main(string[] args)
        {
            await СorrectnessExperimen(100, 100);
        }
        public static async Task СorrectnessExperimen(int cities, int iterations)
        {
            var distanseMap = JsonConvert.DeserializeObject<double[,]>(
                File.ReadAllText($"{cities}cities.txt"));
            Constants constansts = new Constants()
            {
                Alpha = 1,
                Beta = 1,
                Q = 4,
                P = 0.8,
            };

            var asyncStopwatch = new Stopwatch();
            asyncStopwatch.Start();
            AsyncAntColony asyncAntColony = new AsyncAntColony(constansts, distanseMap);
            for (int j = 0; j < iterations; j++)
                await asyncAntColony.IterationAsync();
            asyncStopwatch.Stop();

            var syncStopwatch = new Stopwatch();
            syncStopwatch.Start();
            SyncAntColony syncAntColony = new SyncAntColony(constansts, distanseMap);
            for (int j = 0; j < iterations; j++)
                syncAntColony.Iteration();
            syncStopwatch.Stop();

            Console.WriteLine($"sync | time: {syncStopwatch.ElapsedMilliseconds}," +
                $" bestWay:{syncAntColony.Best.Result}");
            Console.WriteLine($"async| time: {asyncStopwatch.ElapsedMilliseconds}," +
                $" bestWay:{asyncAntColony.Best.Result}");
        }
        public static void SyncExperiment(int cities, int outerIterations)
        {
            int innerIterations = 100;
            long totalTime = 0;
            var distanseMap = JsonConvert.DeserializeObject<double[,]>(
                File.ReadAllText($"{cities}cities.txt"));
            Constants constansts = new Constants()
            {
                Alpha = 1,
                Beta = 1,
                Q = 4,
                P = 0.8,
            };

            for (int i = 0; i < outerIterations; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                SyncAntColony syncAntColony = new SyncAntColony(constansts, distanseMap);
                for (int j = 0; j < innerIterations; j++)
                    syncAntColony.Iteration();

                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
            }
            Console.WriteLine(totalTime/outerIterations);
        }
        public static async Task AsyncExperiment(int cities, int outerIterations)
        {
            int innerIterations = 100;
            long totalTime = 0;
            var distanseMap = JsonConvert.DeserializeObject<double[,]>(
                File.ReadAllText($"{cities}cities.txt"));
            Constants constansts = new Constants()
            {
                Alpha = 1,
                Beta = 1,
                Q = 4,
                P = 0.8,
            };

            for (int i = 0; i < outerIterations; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                AsyncAntColony syncAntColony = new AsyncAntColony(constansts, distanseMap);
                for (int j = 0; j < innerIterations; j++)
                    await syncAntColony.IterationAsync();

                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
            }
            Console.WriteLine(totalTime / outerIterations);
        }
    }
}
