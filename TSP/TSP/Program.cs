using System;
using System.IO;
using System.Linq;

namespace TSP
{
    public class Program
    {

        #region Properties

        public static int TotalCities { get; set; }
        public static int[,] CitiesArray { get; set; }
        public static string FileName { get; set; } = "dane.txt";

        private static FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        private static StreamReader sr = new StreamReader(fs);

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
}
