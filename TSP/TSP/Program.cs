using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

/* 
 Źle liczy dystans
 Lub wczytuje dane

    Ok, powinno iść wierszami a nie kolumnami
     */
namespace TSP
{
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    public class City
    {
        public int Id { get; set; }
        public List<Neighbor> Neighbors { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
    //---------------------------------------------------------------------------------------------------------------//-----------------------------------------------------------------------------------------------------------------------------------------------------//-----------------------------------------------------------------------------------------------------------------------------------------------------
    public struct Neighbor
    {
        public int Id { get; set; }
        public int Distance { get; set; }
        public bool WasVisited { get; set; }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    public class Program
    {

        //private static int totalCities;

        #region Properties

        //public static int TotalCities
        //{
        //    get { return totalCities; }
        //    set
        //    {
        //        var tmp = sr.ReadLine();
        //        sr.Close();
        //        totalCities = Int32.Parse(tmp);
        //    }
        //}

        public static int TotalCities { get; set; }
        public static List<City> cities = new List<City>();
        public static int[,] CitiesArray { get; set; }// = new int[TotalCities, TotalCities];

        public static string FileName { get; set; } = "dane.txt";
        private static FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        private static StreamReader sr = new StreamReader(fs);

        #endregion


        static void ReadFromFileAsMatrix()
        {
            var tmp = sr.ReadToEnd().Replace(System.Environment.NewLine, " ").Split(' ');
            sr.Close();

            // print what we loaded
            //foreach (var item in tmp)
            //{
            //    Console.WriteLine(item);
            //}

            var readInts = Array.ConvertAll(tmp, int.Parse);

            TotalCities = readInts[0];
            int[,] myArr = new int[TotalCities, TotalCities];

            readInts = readInts.Skip(1).ToArray();
            int currentIndexOnReadInts = 0;
            // load array
            for (int i = 0; i < TotalCities; i++)
            {
                for (int j = 0; j < TotalCities; j++)
                {
                    myArr[i, j] = readInts[currentIndexOnReadInts];
                    currentIndexOnReadInts++;
                }
            }

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


        #region Methods



        static void ReadCitiesFromFile()
        {
            string fileName = "dane.txt";

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            var tmp = sr.ReadToEnd().Replace(System.Environment.NewLine, " ").Split(' ');
            sr.Close();

            //for testing
            //foreach (var item in tmp)
            //{
            //    Console.WriteLine(item);
            //}


            var readInts = Array.ConvertAll(tmp, int.Parse);

            TotalCities = readInts[0];

            for (int i = 0; i < TotalCities; i++)
            {
                var tmpCity = new City();
                tmpCity.Id = i;
                tmpCity.Neighbors = new List<Neighbor>();

                for (int j = 0; j < TotalCities; j++)
                {
                    tmpCity.Neighbors.Add(new Neighbor { Id = j, Distance = readInts[(i * TotalCities) + j + 1], WasVisited = false });
                }
                cities.Add(tmpCity);
            }
        }


        #endregion

        static void Main(string[] args)
        {

            //ReadCitiesFromFile();
            ReadFromFileAsMatrix();

            Console.WriteLine();
            // print array (for testing)
            for (int i = 0; i < TotalCities; i++)
            {
                for (int j = 0; j < TotalCities; j++)
                {
                    Console.Write(CitiesArray[i, j] + " ");
                }
                Console.WriteLine();
            }

            // Algorithm.BruteForce(cities, 0, cities.Count - 1);

            //Console.WriteLine(Algorithm.BestRoad);
            //Console.WriteLine(Algorithm.BestPath);
        }

    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    public class Algorithm
    {
        public static int BestRoad { get; set; } = int.MaxValue;
        public static string BestPath { get; set; } // i.e. 0 -> 1 -> 2 -> ...
        /// <summary>
        /// BrufeForce algorithm
        /// </summary>
        /// <param name="cities">Collection of cities</param>
        /// <param name="i">Index we start at</param>
        /// <param name="n">Length of the collection</param>
        public static void BruteForce(List<City> cities, int i, int n)
        {
            int j;
            if (i == n)
            {
                //PrintCities(cities);
                //Console.WriteLine(CalculateDistance(cities));
                int tempDist = CalculateDistance(cities).Item1;
                string path = CalculateDistance(cities).Item2;
                if (tempDist < BestRoad)
                {
                    BestPath = "";
                    BestPath = path;
                    BestRoad = tempDist;
                }
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    Swap(cities, i, j);
                    BruteForce(cities, i + 1, n);
                    Swap(cities, i, j); //backtrack
                }
            }
        }

        static void PrintCities(List<City> cities)
        {
            foreach (var city in cities)
                Console.Write(city.Id + " ");
            Console.WriteLine();
        }
        static void Swap(List<City> cities, int a, int b)
        {
            var tmp = cities[a];
            cities[a] = cities[b];
            cities[b] = tmp;
        }
        static Tuple<int,string> CalculateDistance(List<City> cities)
        {
            int distance = 0, tempId;
            string str = "";
            ResetNeighborList(cities);


            for (int i = 0; i < cities.Count; i++)
            {
                tempId = cities[i].Id;
                str = "";
                ResetNeighborList(cities); //set "wasvisited" to false on every single position
                distance = 0;
                for (int j = 0; j < cities[i].Neighbors.Count; j++) //we count distance here
                {

                    //---------------------------------------------------
                    //we get the ID of the current city and we set on each cities's(?XD) neighbor list, that this city was visited
                    foreach (var city in cities)
                    {
                        for (int k = 0; k < city.Neighbors.Count; k++)
                        {
                            if (tempId == city.Neighbors[k].Id)
                            {
                                var tmpNeighbor = city.Neighbors[k];
                                city.Neighbors[k] = new Neighbor() { Id = tmpNeighbor.Id, Distance = tmpNeighbor.Distance, WasVisited = true };
                            }
                        }
                    }
                    //---------------------------------------------------
                    if (cities[i].Neighbors[j].Id != tempId)
                    if (cities[i].Neighbors[j].WasVisited == false)
                    {
                        distance += cities[i].Neighbors[j].Distance;
                            str += $"{cities[i].Neighbors[j].Id} -> ";
                    }
                }
            }
            return new Tuple<int,string>(distance, str);
        }
        static public void ResetNeighborList(List<City> cities)
        {
            foreach (var city in cities)
                for (int i = 0; i < city.Neighbors.Count; i++)
                {
                    var tmpNeighbor = city.Neighbors[i];
                    city.Neighbors[i] = new Neighbor() { Id = tmpNeighbor.Id, Distance = tmpNeighbor.Distance, WasVisited = false };
                }
        }
    }

}
//-----------------------------------------------------------------------------------------------------------------------------------------------------