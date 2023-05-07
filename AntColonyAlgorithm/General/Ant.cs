namespace AntColonyAlgorithm.General
{
    public class Ant
    {
        public double Result { get; set; }
        public List<int> Sequence { get; }

        public Ant()
        {
            Result = 0;
            Sequence = new List<int>();
        }

        public Ant(double Result, List<int> Sequence)
        {
            this.Result = Result;
            this.Sequence = Sequence;
        }
    }
}
