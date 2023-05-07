﻿namespace AntColonyAlgorithm.General
{
    public struct Constants
    {
        public double Alpha { get; set; }
        public double Beta { get; set; }

        public int M { get; set; } // Кількість мурах

        public double StartPheramonValue { get; set; }
        public double Q { get; set; } // коеф. додавання ферамону
        public double P { get; set; } // 1 - коеф. випаровування ферамону
    }
}