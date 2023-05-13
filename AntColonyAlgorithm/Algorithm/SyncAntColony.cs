using AntColonyAlgorithm.General;

namespace AntColonyAlgorithm.Algorithm
{
    public class SyncAntColony
    {
        public Constants Constants { get; set; }
        public double[,] DistanceMap { get; set; }
        public double[,] PheromonesMap { get; set; }
        public int CitiesCount { get; set; }
        public Ant Best { get; set; }

        public SyncAntColony(Constants constants, double[,] distanceMap)
        {
            if (distanceMap.GetLength(0) != distanceMap.GetLength(1))
                throw new ArgumentException("The distance map is not square");

            Constants = constants;
            DistanceMap = distanceMap;

            CitiesCount = distanceMap.GetLength(0);

            PheromonesMap = new double[CitiesCount, CitiesCount];
            for (int i = 0; i < CitiesCount; i++)
                for (int j = 0; j < CitiesCount; j++)
                    if (i != j)
                        PheromonesMap[i, j] = 0.2;

            Best = new Ant(double.MaxValue, null);
        }

        public void Iteration()
        {
            Ant[] ants = new Ant[CitiesCount];
            for (int i = 0; i < ants.Length; i++)
                ants[i] = new Ant();

            for (int i = 0; i < ants.Length; i++)
            {
                int firstCityIndex = i % CitiesCount;
                GoThroughAllCities(ants[i], firstCityIndex);
            }
            
            var bestAnt = ants.MinBy(ant => ant.Result);
            if (bestAnt.Result < Best.Result)
                Best = bestAnt;

            UpdatePheromone(ants);
        }

        private void GoThroughAllCities(Ant ant, int fromCityIndex)
        {
            while (ant.Sequence.Count < CitiesCount - 1)
            {
                ant.Sequence.Add(fromCityIndex);

                var transitionProbabilities = GetTransitionProbabilities(ant, fromCityIndex);
                int toCityIndex = SelectNextCityToGo(transitionProbabilities, new Random().NextDouble());

                ant.Result += DistanceMap[fromCityIndex, toCityIndex];
                fromCityIndex = toCityIndex;
            }
            ant.Sequence.Add(fromCityIndex);
        }
        private (int, double)[] GetTransitionProbabilities(Ant ant, int fromCityIndex)
        {
            var transitionProbabilities = new (int City, double Probability)[CitiesCount - ant.Sequence.Count];

            double sumOfTransitionProbabilities = 0.0;
            int tpIndex = 0;
            for (int i = 0; i < CitiesCount; i++)
            {
                if (!ant.Sequence.Contains(i))
                {
                    double probability = Math.Pow(PheromonesMap[fromCityIndex, i], Constants.Alpha)
                        * Math.Pow(1 / DistanceMap[fromCityIndex, i], Constants.Beta);
                    sumOfTransitionProbabilities += probability;
                    transitionProbabilities[tpIndex] = new (i, probability);

                    tpIndex++;
                }
            }

            for (int i = 0; i < transitionProbabilities.Length; i++)
                transitionProbabilities[i].Probability /= sumOfTransitionProbabilities;

            return transitionProbabilities;
        }
        private int SelectNextCityToGo((int CityIndex, double Probability)[] sequence, double randomValue)
        {
            double sum = 0;

            for (int i = 0; i < sequence.Length; i++)
            {
                sum += sequence[i].Probability;

                if (randomValue <= sum)
                    return sequence[i].CityIndex;
            }

            return sequence[sequence.Length - 1].CityIndex;
        }
        private void UpdatePheromone(Ant[] ants)
        {
            for (int i = 0; i < PheromonesMap.GetLength(0); i++)
                for (int j = 0; j < PheromonesMap.GetLength(1); j++)
                    if (i != j)
                        PheromonesMap[i, j] = Constants.P * PheromonesMap[i, j];

            foreach (var ant in ants)
                for (int i = 0; i < ant.Sequence.Count - 1; i++)
                    PheromonesMap[ant.Sequence[i], ant.Sequence[i + 1]] += Constants.Q / ant.Result;
        }
    }
}
