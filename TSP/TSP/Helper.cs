using System;
using System.Text;

namespace TSP
{
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
            int distance = 0;

            // StringBuilder is much more efficient when it comes to appending new stuff to string. With small numbers it's not much
            // of a deal, but having nearly n! appends it does matter      
            StringBuilder strPath;

            if (Program.Testing) strPath = new StringBuilder($"0 -> {path[0]}");

            //dist from 0 -> X
            distance += Program.CitiesArray[0, path[0]];

            for (int i = 0; i < path.Length - 1; i++)
            {
                if (Program.CitiesArray[path[i], path[i + 1]] <= 0) return new Tuple<int, string>(Int32.MaxValue, "Something's wrong.");
                distance += Program.CitiesArray[path[i], path[i + 1]];


                if (Program.Testing) strPath.Append($" -> {path[i + 1]}");
            }

            //dist from Z -> 0
            if (Program.Testing) strPath.Append($" -> 0");

            distance += Program.CitiesArray[path[path.Length - 1], 0];


            if (Program.Testing) return new Tuple<int, string>(distance, strPath.ToString());
            return new Tuple<int, string>(distance, String.Empty);
        }

        /// <summary>
        /// Function filling our road array with cities.
        /// </summary>
        /// <returns>Filled road array</returns>
        public static int[] FillRoadArray()
        {
            // Array containing only one path, ie 0 -> 1 -> 2 -> 4 -> 3 -> 0 to operate on CitiesArray.
            int[] RoadArray = new int[Program.TotalCities - 1]; // We know first city we travel from and last city we travel to, therefore -1
            for (int i = 0; i < Program.TotalCities - 1; i++) { RoadArray[i] = i + 1; } // Fill RoadArray with next cities

            return RoadArray;
        }

        /// <summary>
        /// Extension method shuffling the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rng"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        #endregion
    }
}
