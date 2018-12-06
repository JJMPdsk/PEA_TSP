using System;

namespace TSP
{
    public class Algorithm
    {
        #region Properties

        public static int BestRoad { get; set; } = int.MaxValue;
        public static string BestPath { get; set; } // i.e. 0 -> 1 -> 2 -> ...

        #endregion

        #region Methods

        // [1,2,3,4,5] -> [1,2,3,5,4] -> [1,2,4,3,5] -> [1,2,4,5,3] -> 
        // [1,2,5,4,3] -> [1,2,5,3,4] -> [1,3,2,4,5] -> [1,3,2,4,5] -> 
        // [1,3,2,5,4] -> []
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
            if (i == n) // condition to exit 
            {
                //Console.WriteLine();
                //for (int x = 0; x <= path.Length-1; x++)
                //{
                //    Console.Write(path[x]);
                //}


                int tempDist = Helper.CalculateDistance(path).Item1; // temp distance
                string strPath = Helper.CalculateDistance(path).Item2; // temp path
                Program.IterationCounter++;

                if (tempDist >= BestRoad) return;
                BestPath = "";
                BestPath = strPath;
                BestRoad = tempDist;
            }
            else

                for (int j = i; j <= n; j++)
                {
                    Helper.Swap(path, i, j); // swap 2 cities on path array
                    BruteForce(path, i + 1, n); // call recur.
                    Helper.Swap(path, i, j); // swap it back
                }
        }

        #endregion
    }
}
