using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TSP
{
    public class Program
    {

        #region Properties

        public static int TotalCities { get; set; }
        public static int[,] CitiesArray { get; set; }
        public static string FileName { get; set; } = @"C:\Users\JCVUMP\Desktop\C#\PEA_TSP\TSP\TSP\txt\tsp_10.txt";
        public static int IterationCounter { get; set; } = 0;
        public static Stopwatch sw = new Stopwatch();
        public const int State = 2; // 1 - disables text appending so we don't calculate it
                                    // 2 - enables text appending

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


            CitiesArray = myArr;
        }

        public static void PrintCities(int[,] myArr)
        {
            Console.Write(" ");
            for (int i = 0; i < TotalCities; i++)
            {
                Console.Write($"  {i}");
            }

            Console.WriteLine();
            Console.Write(" ");
            for (int i = 0; i < TotalCities; i++)
            {
                Console.Write($"  -");
            }

            Console.WriteLine();
            for (int i = 0; i < TotalCities; i++)
            {
                Console.Write($"{i}| ");
                for (int j = 0; j < TotalCities; j++)
                {
                    Console.Write(myArr[i, j] + " ");
                }

                Console.WriteLine();
            }
        }

        #endregion

        static void Main(string[] args)
        {
            ReadCitiesFromFileAsMatrix();
            int[] roadArray = Helper.FillRoadArray();

            switch (State)
            {
                case 1:
                    {
                        int N = 500;
                        for (int i = 0; i < N; i++)
                        {
                            sw.Reset();
                            sw.Start();
                            Algorithm.BruteForce(roadArray, 0, roadArray.Length - 1);
                            sw.Stop();
                            Console.WriteLine(sw.Elapsed);
                        }

                        break;
                    }
                case 2:
                    PrintCities(CitiesArray);

                    sw.Reset();
                    sw.Start();
                    Algorithm.BruteForce(roadArray, 0, roadArray.Length - 1);
                    sw.Stop();
                    Console.WriteLine($"Iterations: {IterationCounter}");
                    Console.WriteLine($"Elapsed: {sw.Elapsed}");
                    Console.WriteLine($"Solution: {Algorithm.BestRoad}");
                    Console.WriteLine($"Path: {Algorithm.BestPath}");
                    break;
            }

            Console.ReadKey();

        }
    }
}
