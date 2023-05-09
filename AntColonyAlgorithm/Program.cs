using AntColonyAlgorithm.Algorithm;
using AntColonyAlgorithm.General;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AntColonyAlgorithm
{
    internal class Program
    {
        

        static public void Main(string[] args)
        {
            ParameterSelectionExperiment(100, 100);
            //PrintExperimentResult();
        }
        public static void ParameterSelectionExperiment(int countOfCities, int iterations)
        {
            int counter = 0;
            bool observerIsAlive = true;

            var distanseMap = JsonConvert.DeserializeObject<double[,]>(File.ReadAllText($"{countOfCities}cities.txt"));

            List<Constants> stats = new List<Constants>();

            for (int Alpha = 1; Alpha < 5; Alpha++)
                for (int Beta = 1; Beta < 5; Beta++)
                    for (double P = 0.1; P < 1; P += 0.1)
                        for (int Q = 1; Q < 5; Q++)
                        {
                            stats.Add(new Constants()
                            {
                                Alpha = Alpha,
                                Beta = Beta,
                                M = countOfCities,
                                Q = Q,
                                P = P
                            });
                            stats.Add(new Constants()
                            {
                                Alpha = Alpha,
                                Beta = Beta,
                                M = countOfCities / 2,
                                Q = Q,
                                P = P
                            });
                        }

            new Thread(() =>
            {
                while (observerIsAlive)
                {
                    Console.WriteLine($"{counter}/{stats.Count}");
                    Thread.Sleep(10000);
                    Console.Clear();
                }
                Console.WriteLine($"{counter}/{stats.Count}");
            }).Start();

            ConcurrentBag<TestResult> tests = new ConcurrentBag<TestResult>();
            Parallel.ForEach(stats, stat =>
            {
                ParallelAntColony parallelAntColony = new ParallelAntColony(stat, distanseMap);

                for (int i = 0; i < iterations; i++)
                    parallelAntColony.Iteration();

                tests.Add(new TestResult()
                {
                    Alpha = stat.Alpha,
                    Beta = stat.Beta,
                    M = stat.M,
                    Q = stat.Q,
                    P = Math.Round(stat.P, 1),
                    Result = parallelAntColony.Best.Result
                });
                Interlocked.Increment(ref counter);
            });
            observerIsAlive = false;

            string text = JsonConvert.SerializeObject(tests.ToArray());
            File.WriteAllText("testResults.txt", text);
        }
        public static void PrintExperimentResult()
        {
            var list = JsonConvert.DeserializeObject<TestResult[]>(File.ReadAllText("testResults2.txt"));

            var printList = list.OrderByDescending(i => i.Result)
                .ThenByDescending(i => i.Alpha)
                .ThenByDescending(i => i.M)
                .ThenByDescending(i => i.Q)
                .ThenByDescending(i => i.P);

            foreach (var item in printList)
            {
                Console.WriteLine($"When A: {item.Alpha}, B: {item.Beta}, M:{item.M}, Q: {item.Q}, P: {Math.Round(item.P, 1)} " +
                    $"Result: {item.Result}");
            }
        }
        public static void TestSyncAlg(Constants constants, double[,] distanseMap)
        {
            SyncAntColony syncAntColony = new SyncAntColony(constants, distanseMap);

            for (int i = 0; i < 100; i++)
                syncAntColony.Iteration();

            //Console.WriteLine($"синхронний: {syncAntColony.Best.Result}");
            //foreach (var item in syncAntColony.Best.Sequence)
            //{
            //    Console.Write($"{item} -> ");
            //}
            //Console.WriteLine();
        }

        public static void ParallelSyncAlg(Constants constants, double[,] distanseMap)
        {
            ParallelAntColony syncAntColony = new ParallelAntColony(constants, distanseMap);

            for (int i = 0; i < 100; i++)
                syncAntColony.Iteration();

            //Console.WriteLine($"паралельний: {syncAntColony.Best.Result}");
            //foreach (var item in syncAntColony.Best.Sequence)
            //{
            //    Console.Write($"{item} -> ");
            //}
            //Console.WriteLine();
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
    
    class TestResult
    {
        public double Alpha { get; set; }
        public double Beta { get; set; }

        public int M { get; set; } 

        public double Q { get; set; }
        public double P { get; set; } 

        public double Result { get; set; }

    }
}
