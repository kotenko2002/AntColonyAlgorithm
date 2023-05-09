﻿using AntColonyAlgorithm.General;

namespace AntColonyAlgorithm.Algorithm
{
    public class ParallelAntColony
    {
        public Constants Constants { get; set; }
        public double[,] DistanceMap { get; set; }
        public double[,] PheromonesMap { get; set; }
        public int[] Cities { get; set; }
        public Ant Best { get; set; }

        public ParallelAntColony(Constants constants, double[,] distanceMap)
        {
            if (distanceMap.GetLength(0) != distanceMap.GetLength(1))
                throw new ArgumentException("The distance map is not square");

            Constants = constants;
            DistanceMap = distanceMap;

            Cities = new int[distanceMap.GetLength(0)];
            for (int i = 0; i < Cities.Length; i++)
                Cities[i] = i;

            PheromonesMap = new double[Cities.Length, Cities.Length];
            for (int i = 0; i < Cities.Length; i++)
                for (int j = 0; j < Cities.Length; j++)
                    if (i != j)
                        PheromonesMap[i, j] = 0.2;

            Best = new Ant(double.MaxValue, null);
        }

        public void Iteration()
        {
            Ant[] ants = Enumerable.Range(0, Constants.M)
                .Select(_ => new Ant())
                .ToArray();

            Parallel.For(0, ants.Length, i =>
            {
                int firstCityIndex = i % Cities.Length;
                GoThroughAllCities(ants[i], firstCityIndex);
            });

            var bestAnt = ants.MinBy(ant => ant.Result);
            if (bestAnt.Result < Best.Result)
                Best = bestAnt;

            UpdatePheromone(ants);
        }

        private void GoThroughAllCities(Ant ant, int fromCityIndex)
        {
            while (ant.Sequence.Count < Cities.Length - 1)
            {
                ant.Sequence.Add(fromCityIndex);

                var transitionProbabilities = GetTransitionProbabilities(ant, fromCityIndex);
                int toCityIndex = SelectNextCityToGo(transitionProbabilities, new Random().NextDouble());

                ant.Result += DistanceMap[fromCityIndex, toCityIndex];
                fromCityIndex = toCityIndex;
            }
            ant.Sequence.Add(fromCityIndex);
        }
        private (int city, double probability)[] GetTransitionProbabilities(Ant ant, int fromCityIndex)
        {
            (int city, double probability)[] transitionProbabilities = Cities
                .ExceptBy(ant.Sequence, city => city)
                .Select(city => (city, 0.0))
                .ToArray();

            for (int i = 0; i < transitionProbabilities.Length; i++)
            {
                var toCityIndex = transitionProbabilities[i].city;

                transitionProbabilities[i].probability =
                    Math.Pow(PheromonesMap[fromCityIndex, toCityIndex], Constants.Alpha)
                    * Math.Pow(1 / DistanceMap[fromCityIndex, toCityIndex], Constants.Beta);
            }

            double sumOfTransitionProbabilities = transitionProbabilities.Sum(item => item.probability);
            for (int i = 0; i < transitionProbabilities.Length; i++)
            {
                transitionProbabilities[i].probability =
                    transitionProbabilities[i].probability / sumOfTransitionProbabilities;
            }

            return transitionProbabilities;
        }
        private int SelectNextCityToGo((int city, double probability)[] sequence, double randomValue)
        {
            double sum = 0;

            for (int i = 0; i < sequence.Length; i++)
            {
                sum += sequence[i].probability;

                if (randomValue <= sum)
                    return sequence[i].city;
            }

            return sequence[sequence.Length - 1].city;
            //throw new ArgumentException("Data passed as a parameter to the method is not valid");
        }
        private void UpdatePheromone(Ant[] ants)
        {
            // випарюєм
            for (int i = 0; i < PheromonesMap.GetLength(0); i++)
                for (int j = 0; j < PheromonesMap.GetLength(1); j++)
                    if (i != j)
                        PheromonesMap[i, j] = Constants.P * PheromonesMap[i, j];

            //досипаємо
            foreach (var ant in ants)
                for (int i = 0; i < ant.Sequence.Count - 1; i++)
                    PheromonesMap[ant.Sequence[i], ant.Sequence[i + 1]] += Constants.Q / ant.Result;
        }
    }
}
