using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TSP
{
    public class Program
    {

        #region Properties

        public static int TotalCities { get; set; } // amount of cities
        public static int[,] CitiesArray { get; set; } // 2D array of distances
        //public static string FileName { get; set; } =
        //    @"C:\Users\JCVUMP\Desktop\C#\PEA\PEA_TSP\TSP\TSP\txt\data21.txt";

        public static string FileName = @"..\..\txt\data21.txt";
        public static int IterationCounter { get; set; } = 0; // iteration counter for BF 

        public const bool Testing = true; // 1 - enables text appending and route showing

        public static Stopwatch Stopwatch = new Stopwatch();
        private static readonly FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        private static readonly StreamReader sr = new StreamReader(fs);
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

            //for (int i = 0; i < 50; i++)
            //    TabuSearchAlgorithm(roadArray);


            //BruteForceAlgorithm(roadArray);

            GeneticAlgorithm();

            Console.WriteLine("THE END");
            Console.ReadKey();

        }

        private static void TabuSearchAlgorithm(int[] roadArray)
        {
            //parameters for tabuSearch
            int cadency = TotalCities * 2;
            double time = 3;
            double aspiration = 0.95; // [0.0 - 1.0]
            Helper.Shuffle(new Random(), roadArray);


            var tabuSearch = new TabuSearch(cadency, aspiration, time, roadArray);
            tabuSearch.Run();
            Console.WriteLine(tabuSearch.BestSolution.Distance);
        }

        /// <summary>
        /// Execution of BF algorithm
        /// </summary>
        /// <param name="roadArray"></param>
        private static void BruteForceAlgorithm(int[] roadArray)
        {
            // Testing - prints all info
            if (Testing)
            {
                PrintCities(CitiesArray);

                Stopwatch.Reset();
                Stopwatch.Start();
                Algorithm.BruteForce(roadArray, 0, roadArray.Length - 1);
                Stopwatch.Stop();
                Console.WriteLine($"Iterations: {IterationCounter}");
                Console.WriteLine($"Elapsed: {Stopwatch.Elapsed}");
                Console.WriteLine($"Solution: {Algorithm.BestRoad}");
                Console.WriteLine($"Path: {Algorithm.BestPath}");
            }
            else // Only calculations
            {
                int N = 500;
                for (int i = 0; i < N; i++)
                {
                    Stopwatch.Reset();
                    Stopwatch.Start();
                    Algorithm.BruteForce(roadArray, 0, roadArray.Length - 1);
                    Stopwatch.Stop();
                    Console.WriteLine(Stopwatch.Elapsed);
                }
            }
        }


        private static void GeneticAlgorithm()
        {
            int populationCount = TotalCities*3;
            int generationCount = 1000000;
            double crossoverChance = 0.3;
            double mutationChance = 0.15;

            var geneticAlgorithm = new GeneticAlgorithm(populationCount, generationCount, mutationChance, crossoverChance);
            
            geneticAlgorithm.Run();
        }

    }
}
