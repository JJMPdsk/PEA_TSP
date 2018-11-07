namespace TSP
{
    public class Algorithm
    {
        #region Properties

        public static int BestRoad { get; set; } = int.MaxValue;
        public static string BestPath { get; set; } // i.e. 0 -> 1 -> 2 -> ...

        #endregion

        #region Methods

        /// <summary>
        /// BrufeForce algorithm
        /// We get all permutations of Road Array to get all possible paths.
        /// Then we calculate distance for each path.
        /// </summary>
        /// <param name="path">Road array</param>
        /// <param name="i">Index we start at</param>
        /// <param name="n">Length of the collection</param>
        public static void BruteForce(int[] path, int i, int n)
        {
            int j;
            if (i == n)
            {
                int tempDist = Helper.CalculateDistance(path).Item1;
                string strPath = Helper.CalculateDistance(path).Item2;
                if (tempDist >= BestRoad) return;
                BestPath = "";
                BestPath = strPath;
                BestRoad = tempDist;
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    Helper.Swap(path, i, j);
                    BruteForce(path, i + 1, n);
                    Helper.Swap(path, i, j); //backtrack
                }
            }
        }

        #endregion
    }
}
