using System;
using System.IO;
using System.Linq;

namespace TSP
{
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    public class Program
    {

        #region Properties

        private static FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        private static StreamReader sr = new StreamReader(fs);

        public static int TotalCities { get; set; }
        public static int[,] CitiesArray { get; set; }
        public static string FileName { get; set; } = "dane.txt";

        #endregion

        #region Methods

        static void ReadCitiesFromFileAsMatrix()
        {
            // Get whole file into string array and replace anything that is a new line with space. Then split it at every space occured.
            var tmp = sr.ReadToEnd().Replace(System.Environment.NewLine, " ").Split(' ');
            sr.Close();

            // print what we loaded
            //foreach (var item in tmp)
            //{
            //    Console.WriteLine(item);
            //}

            // Convert tmp array containing strings into int array.
            var readInts = Array.ConvertAll(tmp, int.Parse);

            // The number of cities is our first element in our int array.
            TotalCities = readInts[0];
            int[,] myArr = new int[TotalCities, TotalCities];

            // We ignore first element, which is the number of cities and get rid of first element.
            readInts = readInts.Skip(1).ToArray();

            // Indicates where we are currently on readInts array.
            int currentIndexOnReadInts = 0;

            // load 2D array
            for (int i = 0; i < TotalCities; i++)
            {
                for (int j = 0; j < TotalCities; j++)
                {
                    myArr[i, j] = readInts[currentIndexOnReadInts];
                    currentIndexOnReadInts++;
                }
            }

            // Print loaded array.
            //Console.WriteLine();
            // print array (for testing)
            //for (int i = 0; i < TotalCities; i++)
            //{
            //    for (int j = 0; j < TotalCities; j++)
            //    {
            //        Console.Write(myArr[i,j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            CitiesArray = myArr;
        }

        #endregion

        static void Main(string[] args)
        {
            ReadCitiesFromFileAsMatrix();

            // Array containing only one path, ie 0 -> 1 -> 2 -> 4 -> 3 -> 0 to operate on CitiesArray.
            int[] RoadArray = new int[Program.TotalCities - 1]; // We know first city we travel from and last city we travel to, therefore -1
            for (int i = 0; i < TotalCities - 1; i++){RoadArray[i] = i + 1;} // Fill RoadArray with next cities

            Algorithm.BruteForce(RoadArray, 0, RoadArray.Length-1);

            Console.WriteLine(Algorithm.BestRoad);
            Console.WriteLine(Algorithm.BestPath);
        }

    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
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
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    public static class Helper
    {
        #region Methods

        /// <summary>
        /// Function that exchanges 2 elements of an array
        /// </summary>
        /// <param name="path"></param>
        /// <param name="el1"></param>
        /// <param name="el2"></param>
        public static void Swap(int[] path, int el1, int el2)
        {
            var tmp = path[el1];
            path[el1] = path[el2];
            path[el2] = tmp;
        }

        /// <summary>
        /// Function calculating the distance for specific path
        /// </summary>
        /// <param name="path">Road array</param>
        /// <returns>Returns tuple containing distance(int) and path(string)</returns>
        public static Tuple<int, string> CalculateDistance(int[] path)
        {
            int distance = 0, tmpdist = 0;
            string strPath = $"0 -> {path[0]}";

            //dist from 0 -> X
            distance += Program.CitiesArray[0, path[0]];

            for (int i = 0; i < path.Length-1; i++)
            {
                distance += Program.CitiesArray[path[i], path[i+1]];
                strPath += $" -> {path[i + 1]}";
            }

            //dist from Z -> 0
            strPath += $" -> 0";
            distance += Program.CitiesArray[path[path.Length-1], 0];

            return new Tuple<int, string>(distance, strPath);
        }

        #endregion
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------------------------------------------------------------------------------------------------------
